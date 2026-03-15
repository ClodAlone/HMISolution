// <copyright file="CircularPointer.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the pointer visual element that moves along the circular scale of the circular gauge.
    /// </summary>
    /// <remarks>
    /// To use a pointer as a MarkerPointer, the <see cref="PointerNeedleType"/> should be set to
    /// marker and <see cref="MarkerStyle"/> can be choosed from the available styles.
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CircularPointerSample.Window1" Title="CircularPointerSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100" 
    ///                                       Minimum="0" Maximum="100" MajorIntervalValue="10" 
    ///                                       MinorIntervalValue="2" ScaleBarSize="10" 
    ///                                       GapSweepAngle="310" StartAngle="110"
    ///                                       BackgroundBrush="LightBlue"> 
    ///                 <syncfusion:CircularScale.Pointers>
    ///                     <syncfusion:CircularPointer BorderWidth="1" ointerWidth="15" 
    ///                                                 PointerLength="100" NeedleStyle="Triangle" 
    ///                                                 PointerNeedleType="Needle" 
    ///                                                 PointerPlacement="Cross" Value="50" />
    ///                 </syncfusion:CircularScale.Pointers>
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CircularPointerSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         private CircularScale m_scale;       
    ///         private CircularGauge m_gauge;
    ///         public Window1()
    ///         {
    ///             InitializeComponent();<para/>
    ///             m_scale = new CircularScale();
    ///             m_gauge = new CircularGauge();
    ///             m_scale.ShadowOffset = 1;
    ///             m_scale.Minimum = 0;
    ///             m_scale.Maximum = 100;
    ///             m_scale.MinorIntervalValue = 2;
    ///             m_scale.MajorIntervalValue = 10;
    ///             m_scale.StartAngle = 120;
    ///             m_scale.GapSweepAngle = 300;
    ///             m_scale.ScaleBarSize = 1.5;
    ///             m_scale.Radius = 116;
    ///             this.m_gauge.Scales.Add(m_scale);
    ///             CircularPointer pointer1 = new CircularPointer();          
    ///             pointer1.PointerLength = 100;
    ///             pointer1.PointerWidth = 20;
    ///             pointer1.PointerPlacement = ScalePlacement.Outside;
    ///             m_scale.Pointers.Add(pointer1);
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CircularPointer : GaugeElement
    {

        #region Private Memebers

        /// <summary>
        /// The space between the Radius of the Gauge and the Pointer.
        /// </summary>
        private double pointerRatio;

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        private Geometry m_needlePath;

        private bool flag = false;
        private Geometry rect;

        /// <summary>
        /// Event arguments used to fire the ValueChanged event.
        /// </summary>
        private DependencyPropertyChangedEventArgs m_valueArgs;

        private bool mclick = false;

        #endregion Private Members

        #region CLR Getters & Setters

        /// <summary>
        /// Gets or sets the space between the radius of the Gauge and the pointer.       
        /// </summary>
        internal double PointerRatio
        {
            get
            {
                return pointerRatio;
            }

            set
            {
                pointerRatio = value;
            }
        }

        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="AnglePosition"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AnglePositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="MarkerStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerStyleChanged;


        /// <summary>
        /// Event that will raise when <see cref="NeedleCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NeedleCustomGeometryChanged;

        /// <summary>
        /// Event that will raise when <see cref="MarkerCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerCustomGeometryChanged;

        /// <summary>
        /// Event that is raised when <see cref="NeedleStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NeedleStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerLength"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerLengthChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerNeedleType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerNeedleTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerPlacement"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerPlacementChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="AnimationDuration"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AnimationDurationChanged;

        /// <summary>
        /// Event that is raised when <see cref="AnimationDuration"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAnimationEnabledChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnablePointerInteraction"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnablePointerInteractionChanged;

        /// <summary>
        /// Event that is raised when <see cref="IncrementKey"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IncrementKeyChanged;

        /// <summary>
        /// Event that is raised when <see cref="DecrementKey"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DecrementKeyChanged;
        #endregion Events
        /// <summary>
        /// Identifies the <see cref="EnableSizeToContainer"/> dependency property.
        /// </summary>
        #region Dependency Properties

        public static readonly DependencyProperty EnableSizeToContainerProperty =
        DependencyProperty.Register("EnableSizeToContainer", typeof(bool), typeof(GaugeElement), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="AnglePosition"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty AnglePositionProperty =
            DependencyProperty.Register("AnglePosition", typeof(double), typeof(CircularPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAnglePositionChanged)));

        /// <summary>
        /// Identifies the <see cref="MarkerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerStyleProperty =
            DependencyProperty.Register("MarkerStyle", typeof(MarkerStyle), typeof(CircularPointer), new FrameworkPropertyMetadata(MarkerStyle.Rectangle, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnMarkerStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="NeedleCustomGeometry"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleCustomGeometryProperty =
            DependencyProperty.Register("NeedleCustomGeometry", typeof(Geometry), typeof(CircularPointer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnNeedleCustomGeometryChanged)));

        /// <summary>
        /// Identifies the <see cref="MarkerCustomGeometry"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerCustomGeometryProperty =
          DependencyProperty.Register("MarkerCustomGeometry", typeof(Geometry), typeof(CircularPointer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnMarkerCustomGeometryChanged)));

        /// <summary>
        /// Identifies the <see cref="NeedleStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleStyleProperty =
            DependencyProperty.Register("NeedleStyle", typeof(NeedleStyle), typeof(CircularPointer), new FrameworkPropertyMetadata(NeedleStyle.Triangle, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnNeedleStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerLengthProperty =
            DependencyProperty.Register("PointerLength", typeof(double), typeof(CircularPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPointerLengthChanged), CoercePointerLength));

        /// <summary>
        /// Identifies the <see cref="PointerNeedleType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerNeedleTypeProperty =
            DependencyProperty.Register("PointerNeedleType", typeof(PointerNeedleType), typeof(CircularPointer), new FrameworkPropertyMetadata(PointerNeedleType.Needle, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnPointerNeedleTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerPlacementProperty =
            DependencyProperty.Register("PointerPlacement", typeof(ScalePlacement), typeof(CircularPointer), new FrameworkPropertyMetadata(ScalePlacement.Inside, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnPointerPlacementChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerWidthProperty =
            DependencyProperty.Register("PointerWidth", typeof(double), typeof(CircularPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnPointerWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CircularPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnValueChanged), CoerceValue));

        /// <summary>
        /// Identifies the <see cref="AnimationDuration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(double), typeof(CircularPointer), new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAnimationDurationChanged)));
        /// <summary>
        /// Identifies the <see cref="IsAnimationEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledProperty =
            DependencyProperty.Register("IsAnimationEnabled", typeof(bool), typeof(CircularPointer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsAnimationEnabledChanged)));
        
        /// <summary>
        /// Identifies the <see cref="EnablePointerInteraction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnablePointerInteractionProperty =
            DependencyProperty.Register("EnablePointerInteraction", typeof(bool), typeof(CircularPointer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnEnablePointerInteractionChanged)));

        /// <summary>
        /// Identifies the <see cref="IncrementKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementKeyProperty =
            DependencyProperty.Register("IncrementKey", typeof(Key), typeof(CircularPointer), new FrameworkPropertyMetadata(Key.Right, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIncrementKeyChanged)));

        /// <summary>
        /// Identifies the <see cref="DecrementKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecrementKeyProperty =
            DependencyProperty.Register("DecrementKey", typeof(Key), typeof(CircularPointer), new FrameworkPropertyMetadata(Key.Left, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDecrementKeyChanged)));

        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the angle to move the pointer to.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        internal double AnglePosition
        {
            get
            {
                return (double)GetValue(AnglePositionProperty);
            }

            set
            {
                SetValue(AnglePositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable size to container].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable size to container]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSizeToContainer
        {
            get { return (bool)GetValue(EnableSizeToContainerProperty); }
            set { SetValue(EnableSizeToContainerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the shape of the marker pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MarkerStyle"/>
        /// Default value is MarkerStyle.Rectangle.
        /// </value>
        /// <seealso cref="NeedleStyle"/>
        public MarkerStyle MarkerStyle
        {
            get
            {
                return (MarkerStyle)GetValue(MarkerStyleProperty);
            }

            set
            {
                SetValue(MarkerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom geometry value for Needle.
        /// </summary>
        /// <value>
        /// Type : <see cref="Geometry"/>
        /// Default Value is Null.
        /// </value>
        public Geometry NeedleCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(NeedleCustomGeometryProperty);
            }

            set
            {
                SetValue(NeedleCustomGeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom geometry value for Marker.
        /// </summary>
        /// <value>
        /// Type : <see cref="Geometry"/>
        /// Default Value is Null.
        /// </value>
        public Geometry MarkerCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(MarkerCustomGeometryProperty);
            }

            set
            {
                SetValue(MarkerCustomGeometryProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the shape of the needle pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="NeedleStyle"/>
        /// Default value is NeedleStyle.Triangle.
        /// </value>
        /// <seealso cref="MarkerStyle"/>
        public NeedleStyle NeedleStyle
        {
            get
            {
                return (NeedleStyle)GetValue(NeedleStyleProperty);
            }

            set
            {
                SetValue(NeedleStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="PointerWidth"/>
        public double PointerLength
        {
            get
            {
                return (double)GetValue(PointerLengthProperty);
            }

            set
            {
                SetValue(PointerLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pointer needle type.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="PointerNeedleType"/>
        /// Default value is PointerNeedleType.Needle.
        /// </value>
        public PointerNeedleType PointerNeedleType
        {
            get
            {
                return (PointerNeedleType)GetValue(PointerNeedleTypeProperty);
            }

            set
            {
                SetValue(PointerNeedleTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position to place the pointer relative to the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScalePlacement"/>
        /// Default value is ScalePlacement.Inside.
        /// </value>
        public ScalePlacement PointerPlacement
        {
            get
            {
                return (ScalePlacement)GetValue(PointerPlacementProperty);
            }

            set
            {
                SetValue(PointerPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double PointerWidth
        {
            get
            {
                return (double)GetValue(PointerWidthProperty);
            }

            set
            {
                SetValue(PointerWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicated by the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the animation duration of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is Double.NaN.
        /// </value>
        public double AnimationDuration
        {
            get
            {
                return (double)GetValue(AnimationDurationProperty);
            }

            set
            {
                SetValue(AnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether animation is enabled or not.
        /// </summary>
        /// <value><see langword="true"/> if this instance ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool IsAnimationEnabled
        {
            get
            {
                return (bool)GetValue(IsAnimationEnabledProperty);
            }

            set
            {
                SetValue(IsAnimationEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the pointer interactivity of the pointer is enabled or not.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default value is true.
        /// </value>
        public bool EnablePointerInteraction
        {
            get
            {
                return (bool)GetValue(EnablePointerInteractionProperty);
            }

            set
            {
                SetValue(EnablePointerInteractionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Increment key for the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Key"/>
        /// Default value is Up key.
        /// </value>
        public Key IncrementKey
        {
            get
            {
                return (Key)GetValue(IncrementKeyProperty);
            }

            set
            {
                SetValue(IncrementKeyProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the Decrement key for the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Key"/>
        /// Default value is Down key.
        /// </value>
        public Key DecrementKey
        {
            get
            {
                return (Key)GetValue(DecrementKeyProperty);
            }

            set
            {
                SetValue(DecrementKeyProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CircularPointer"/> class.
        /// Overrides the meta-data for default template.
        /// </summary>
        static CircularPointer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularPointer), new FrameworkPropertyMetadata(typeof(CircularPointer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularPointer"/> class.
        /// </summary>
        public CircularPointer()
        {
            this.Loaded += new RoutedEventHandler(CircularPointer_Loaded);
        }

        void CircularPointer_Loaded(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                this.Value = (double)this.CoerceValue(this.Value);
            }

        }

        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Initializes the Circular Pointer.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            pointerRatio = this.PointerLength;
            NameScope.SetNameScope(this, null);
            if (this.EnablePointerInteraction)
                this.EnableMouseEvents(this);
            else
                this.DisableMouseEvents(this);

        }


        /// <summary>
        /// Raises the KeyDown event when the Key is pressed during the pointer is in focus.
        /// </summary>
        /// <param name="sender">CircularPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        void CircularPointer_KeyDown(object sender, KeyEventArgs e)
        {

            Keyboard.Focus(this);

            CircularPointer pointer;

            pointer = this as CircularPointer;

            if (this.VisualParent is ScaleBase)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                if (e.Key == Key.Left || e.Key == Key.Down || e.Key == Key.PageDown || e.Key == DecrementKey)
                {
                    e.Handled = true;
                    if (this.Value > scale.Minimum)
                        this.Value--;
                }
                else if (e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.PageUp || e.Key == IncrementKey)
                {
                    e.Handled = true;
                    if (this.Value < scale.Maximum)
                        this.Value++;
                }
                else if (e.Key == Key.End)
                {
                    e.Handled = true;
                    this.Value = scale.Maximum;
                }
                else if (e.Key == Key.Home)
                {
                    e.Handled = true;
                    this.Value = scale.Minimum;
                }
                else if (e.Key == Key.PageDown)
                {
                    e.Handled = true;
                    if (this.Value > scale.Minimum && scale.ScaleDirection == ScaleDirection.Clockwise)
                        this.Value--;
                    else if (this.Value < scale.Maximum && scale.ScaleDirection == ScaleDirection.CounterClockwise)
                        this.Value++;
                }
                else if (e.Key == Key.PageUp)
                {
                    e.Handled = true;
                    if (this.Value < scale.Maximum && scale.ScaleDirection == ScaleDirection.Clockwise)
                        this.Value++;
                    else if (this.Value > scale.Minimum && scale.ScaleDirection == ScaleDirection.CounterClockwise)
                        this.Value--;
                }
            }
        }

        /// <summary>
        /// Raises the MouseLeftButtonUp event when the mouse Left button is up on the pointer.
        /// </summary>
        /// <param name="sender">CircularPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        void CircularPointer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(CircularPointer))
            {
                e.Handled = true;
                CircularPointer el = sender as CircularPointer;
                el.ReleaseMouseCapture();
                this.mclick = false;
            }
        }

        /// <summary>
        /// Raises the  MouseLeftButtonDown event when the mouse Left button is down on the pointer.
        /// </summary>
        /// <param name="sender">CircularPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        void CircularPointer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.EnablePointerInteraction)
            {
                Keyboard.Focus(this);
            }

            if (sender.GetType() == typeof(CircularPointer))
            {
                e.Handled = true;
                CircularPointer el = sender as CircularPointer;
                el.CaptureMouse();
                if (this.VisualParent is ScaleBase)
                {
                    CircularScale scale = this.VisualParent as CircularScale;
                    Point p = e.GetPosition(scale);
                    double num, deno;
                    num = p.Y - scale.Radius;
                    deno = p.X - scale.Radius;
                    double m = num / deno;
                    double tan = (180 * Math.Atan(m)) / Math.PI;
                    if (deno < 0)
                    {
                        tan = 180 + tan;
                    }

                    double value = this.AngleToValue(tan, this.Value);

                    if (value != this.Value)
                    {
                        this.AnglePosition = this.GetAngleByValue(value);
                        this.Value = value;
                    }

                    if (this.AnglePosition > 360)
                    {
                        this.AnglePosition -= 360;
                    }

                    this.RefreshAnglePosition();
                    this.mclick = true;
                }
            }
        }

        /// <summary>
        /// Raises the MouseMove event when the mouse cursor is moved on the pointer.
        /// </summary>
        /// <param name="sender">CircularPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        void CircularPointer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.EnablePointerInteraction == true)
            {
                e.Handled = true;
                this.Cursor = Cursors.Hand;
                if (this.mclick == true)
                {
                    if (this.VisualParent is ScaleBase)
                    {
                        CircularScale scale = this.VisualParent as CircularScale;
                        Point p = e.GetPosition(scale);
                        double num, deno;
                        num = p.Y - scale.Radius;
                        deno = p.X - scale.Radius;
                        double m = num / deno;
                        double tan = (180 * Math.Atan(m)) / Math.PI;
                        if (deno < 0)
                        {
                            tan = 180 + tan;
                        }

                        double value = this.AngleToValue(tan, this.Value);
                        if (value != this.Value)
                        {
                            this.AnglePosition = this.GetAngleByValue(value);
                            this.Value = value;
                        }

                        if (this.AnglePosition > scale.GapSweepAngle + scale.StartAngle)
                        {
                            this.AnglePosition -= 360;
                        }

                        this.RefreshAnglePosition();
                    }
                }
            }
        }

        /// <summary>
        /// Method to convert Angle to Value in circularknob.
        /// </summary>
        /// <param name="angle">Angle value for conversion</param>
        /// <param name="pointervalue">previous pointer value</param>
        /// <returns>
        /// Value corresponding to that given angle
        /// </returns>
        protected internal virtual double AngleToValue(double angle, double pointervalue)
        {
            double value = 0;
            CircularScale scale = this.VisualParent as CircularScale;
            if (scale != null)
            {
                if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                {
                    if (angle < scale.StartAngle && angle > (scale.StartAngle + scale.GapSweepAngle - 360))
                    {
                        if (angle < scale.StartAngle && this.AnglePosition > scale.StartAngle + scale.GapSweepAngle)
                        {
                            angle = scale.StartAngle;
                            value = scale.Minimum;
                            this.Value = scale.Minimum;
                        }
                        else if (angle < scale.StartAngle && this.AnglePosition <= (scale.StartAngle + scale.GapSweepAngle - 360))
                        {
                            angle = scale.StartAngle + scale.GapSweepAngle - 360 + 10;
                            value = scale.Maximum;
                            this.Value = scale.Maximum;
                        }
                        else
                        {
                            return pointervalue;
                        }
                    }
                    else
                    {
                        if (angle < scale.StartAngle)
                        {
                            angle = angle + 360;
                        }

                        double ratio = scale.GapSweepAngle / (angle - scale.StartAngle);
                        value = scale.Maximum - (((scale.Maximum - scale.Minimum) / ratio) + scale.Minimum);
                    }
                }
                else
                {
                    if (angle < scale.StartAngle && angle > (scale.StartAngle + scale.GapSweepAngle - 360))
                    {
                        if (angle < scale.StartAngle && this.AnglePosition > scale.StartAngle + scale.GapSweepAngle)
                        {
                            angle = scale.StartAngle;
                            value = scale.Minimum;
                            this.Value = scale.Minimum;
                        }
                        else if (angle < scale.StartAngle && this.AnglePosition <= (scale.StartAngle + scale.GapSweepAngle - 360))
                        {
                            angle = scale.StartAngle + scale.GapSweepAngle - 360 + 10;
                            value = scale.Maximum;
                            this.Value = scale.Maximum;
                        }
                        else
                        {
                            return pointervalue;
                        }
                    }
                    else
                    {
                        if (angle < scale.StartAngle)
                        {
                            angle = angle + 360;
                        }

                        double ratio = scale.GapSweepAngle / (angle - scale.StartAngle);
                        value = ((scale.Maximum - scale.Minimum) / ratio) + scale.Minimum;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Measures the size required for child elements in layout and determines a size
        /// for the this element.
        /// </summary>
        /// <remarks>
        /// For &quot;Bar&quot; pointer, the height required is 1. For all the remaining
        /// pointer types , the size required is equal to their Height and Width.
        /// </remarks>
        /// <param name="availableSize">The available size that this element can give to
        /// child elements.</param>
        /// <returns>
        /// The size that this element determines it needs during layout.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(1, 1);
            if (this.PointerNeedleType == PointerNeedleType.Needle || this.PointerNeedleType == PointerNeedleType.Marker)
            {
                size = new Size(this.PointerLength, this.PointerWidth);
            }
            else if (this.PointerNeedleType == PointerNeedleType.Bar)
            {
                size = new Size(this.PointerWidth, 1);
            }

            return size;
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <remarks>
        /// Method returns the actual size used as the same value as that of the size given
        /// by its parent to arrange the pointer. Instead of actually arranging the pointer,
        /// it just initializes the m_needlePath according to the shape of the pointer.
        /// </remarks>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.PointerNeedleType == PointerNeedleType.Needle)
            {
                m_needlePath = this.GetNeedlePath();
            }
            else if (this.PointerNeedleType == PointerNeedleType.Marker)
            {
                m_needlePath = this.GetMarkerPath();
            }
            else if (this.PointerNeedleType == PointerNeedleType.Bar)
            {
                m_needlePath = this.GetBarPath();
            }

            this.RefreshAnglePosition();
            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <remarks>
        /// The shadow offset is calculated according to the CircularGauge and is stored. If
        /// the pointer is of type Marker, there is no need to rotate the pointer. All the
        /// remaining pointer types are rotated by an angle of 180. The actual pointer and
        /// the shadow of the pointer is drawn.(Since the pointer is drawn at the center
        /// of the Gauge and then transformed to its actual location,all non-rectangular
        /// shaped pointers should be rotated)
        /// </remarks>
        /// <param name="drawingContext">The drawing instructions for a specific
        /// element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            CircularScale scale = this.VisualParent as CircularScale;
            if (scale != null)
            {
                double shadowOffsetX = 0;
                double shadowOffsetY = 0;
                double angle = this.AnglePosition % 360;
                double pos1 = 45;
                double pos2 = 135;
                double pos3 = 225;
                double pos4 = 315;

                if (angle >= 0 && angle <= pos1)
                {
                    shadowOffsetY = scale.ShadowOffset * (pos1 - angle) / 90;
                    shadowOffsetX = scale.ShadowOffset * (angle + pos1) / 90;
                }
                else if (angle > pos1 && angle <= pos2)
                {
                    shadowOffsetY = scale.ShadowOffset * (angle - pos1) / 90;
                    shadowOffsetY *= -1;
                    shadowOffsetX = scale.ShadowOffset * (pos2 - angle) / 90;
                }
                else if (angle > pos2 && angle <= pos3)
                {
                    shadowOffsetY = scale.ShadowOffset * (pos3 - angle) / 90;
                    shadowOffsetY *= -1;
                    shadowOffsetX = scale.ShadowOffset * (angle - pos2) / 90;
                    shadowOffsetX *= -1;
                }
                else if (angle > pos3 && angle <= pos4)
                {
                    shadowOffsetY = scale.ShadowOffset * (angle - pos3) / 90;
                    shadowOffsetX = scale.ShadowOffset * (pos4 - angle) / 90;
                    shadowOffsetX *= -1;
                }
                else
                {
                    shadowOffsetY = scale.ShadowOffset * (pos4 - angle + 90) / 90;
                    shadowOffsetX = scale.ShadowOffset * (angle - pos4) / 90;
                }

                if (m_needlePath != null)
                {
                    if (this.PointerNeedleType == PointerNeedleType.Marker && this.PointerPlacement == ScalePlacement.Inside
                        && (this.MarkerStyle == MarkerStyle.Triangle || this.MarkerStyle == MarkerStyle.Pentagon || this.MarkerStyle == MarkerStyle.Trapezoid))
                    {
                        RotateTransform transform1 = new RotateTransform(180, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
                        drawingContext.PushTransform(transform1);

                        TranslateTransform transform2 = new TranslateTransform(-shadowOffsetX, -shadowOffsetY);
                        drawingContext.PushTransform(transform2);

                        // Drawing the Pointer.
                        drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(100, 123, 123, 118)), null, m_needlePath);
                        drawingContext.Pop();

                        // Drawing the Shadow.
                        drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_needlePath);
                        drawingContext.Pop();
                        rect = m_needlePath;

                        if (this.IsKeyboardFocused)
                        {
                            drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Black), 0.5), rect);
                        }
                        else
                        {
                            drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Transparent), 0.5), rect);
                        }

                    }
                    else
                    {
                        if (this.PointerNeedleType == PointerNeedleType.Bar)
                        {
                            shadowOffsetX = scale.ShadowOffset;
                            shadowOffsetY = scale.ShadowOffset;
                        }

                        TranslateTransform transform2 = new TranslateTransform(shadowOffsetX, shadowOffsetY);
                        drawingContext.PushTransform(transform2);

                        // Drawing the Pointer.
                        drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(100, 123, 123, 118)), null, m_needlePath);
                        drawingContext.Pop();

                        // Drawing the Shadow.
                        drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_needlePath);
                        rect = m_needlePath;


                        if (this.IsKeyboardFocused)
                        {
                            drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Black), 0.5), rect);
                        }
                        else
                        {
                            drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Transparent), 0.5), rect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.LostFocus"/> routed event by
        /// using the event data that is provided. 
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs"/> that contains
        /// event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.InvalidateVisual();
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/>
        /// event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.InvalidateVisual();
            //ResourceDictionary mgeneric = new ResourceDictionary();
            //mgeneric.Source = new Uri("/Syncfusion.Gauge.WPF;component/Themes/Templates.xaml", UriKind.Relative);
            //this.Style = mgeneric["listItem"] as Style;
        }


        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Gets the corresponding angle for scale value passed.
        /// </summary>
        /// <param name="value">Pointer value.</param>
        /// <returns>Calculated angle.</returns>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>Local Variables Used</listheader>
        ///     <item>
        ///         <term>ratio</term>
        ///         <description>Calculates the number of segments(between Maximum and Minimum values),
        ///         that are possible,each of "value - Minimum" length.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>angle</term>
        ///        <description>Calculate the angles, of interval("value-Minimum") relative to Sweep angle 
        ///         of the Gauge and then adding the StartAngle to get the exact angle to place the Pointer.
        ///         </description>
        ///     </item>
        /// </list></remarks>
        protected virtual double GetAngleByValue(double value)
        {
            double angle = 0;
            if (this.VisualParent is CircularScale)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                angle = scale.ScaleDirection == ScaleDirection.Clockwise ? scale.StartAngle : scale.StartAngle + scale.GapSweepAngle;
                if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                {
                    if (scale.Minimum <= value && value <= scale.Maximum)
                    {
                        double ratio = (scale.Maximum - scale.Minimum) / (scale.Maximum - value);
                        angle = (scale.GapSweepAngle / ratio) + scale.StartAngle;
                    }
                }
                else
                {
                    if (scale.Minimum != value && scale.Minimum != scale.Maximum)
                    {
                        double ratio = (scale.Maximum - scale.Minimum) / (value - scale.Minimum);
                        angle = (scale.GapSweepAngle / ratio) + scale.StartAngle;
                    }
                }
            }

            return angle;
        }

        /// <summary>
        /// Gets the geometry to draw the bar pointer.
        /// </summary>
        /// <remarks>
        /// If the angle between the StartAngle and the angle to which the pointer should be
        /// set is 360, then two Ellipses of equal X and Y radius are drawn representing the
        /// outer and inner circles of the bar pointer. Else a PathFigure of appropriate angle 
        /// is drawn from the StartAngle to the location of the pointer.
        /// </remarks>
        /// <returns>
        /// The geometry object.
        /// </returns>
        private Geometry GetBarPath()
        {
            CircularScale scale = this.VisualParent as CircularScale;
            Geometry path = null;
            if (this.VisualParent is CircularScale)
            {
                SweepDirection st = new SweepDirection();
                SweepDirection et = new SweepDirection();
                double startangle;
                if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                {
                    st = SweepDirection.Counterclockwise;
                    et = SweepDirection.Clockwise;
                    startangle = scale.StartAngle + scale.GapSweepAngle;
                }
                else
                {
                    st = SweepDirection.Clockwise;
                    et = SweepDirection.Counterclockwise;
                    startangle = scale.StartAngle;
                }

                Point centerPoint = new Point(0, -0.5);
                bool isArcLarge = this.AnglePosition - scale.StartAngle >= 180;
                double outerRadius = scale.Radius - ((scale.ScaleBarSize - this.PointerWidth) / 2);
                double innerRadius = outerRadius - this.PointerWidth;
                if (innerRadius < 0)
                {
                    innerRadius = 0;
                }

                if (outerRadius < 0)
                {
                    outerRadius = 0;
                }

                if (this.AnglePosition - scale.StartAngle == 360)
                {
                    GeometryGroup pathGeometry = new GeometryGroup();
                    EllipseGeometry ellipseGeometry = new EllipseGeometry();
                    ellipseGeometry.RadiusX = innerRadius;
                    ellipseGeometry.RadiusY = innerRadius;
                    ellipseGeometry.Center = centerPoint;
                    pathGeometry.Children.Add(ellipseGeometry);
                    ellipseGeometry = new EllipseGeometry();
                    ellipseGeometry.RadiusX = outerRadius;
                    ellipseGeometry.RadiusY = outerRadius;
                    ellipseGeometry.Center = centerPoint;
                    pathGeometry.Children.Add(ellipseGeometry);

                    path = pathGeometry;
                }
                else if (this.AnglePosition > scale.StartAngle || this.AnglePosition < scale.StartAngle + scale.GapSweepAngle)
                {
                    if (scale.ScaleDirection == ScaleDirection.Clockwise)
                    {
                        if (this.AnglePosition == scale.StartAngle + scale.GapSweepAngle)
                        {
                            this.AnglePosition = scale.StartAngle + scale.GapSweepAngle - 0.5;
                        }
                        else if (this.AnglePosition == scale.StartAngle)
                        {
                            this.AnglePosition = scale.StartAngle + 0.5;
                        }
                    }
                    else
                    {
                        if (this.AnglePosition == scale.StartAngle + scale.GapSweepAngle)
                        {
                            this.AnglePosition = scale.StartAngle + scale.GapSweepAngle - 0.5;
                        }
                        else if (this.AnglePosition > scale.StartAngle + scale.GapSweepAngle)
                        {
                            this.AnglePosition = scale.StartAngle + scale.GapSweepAngle;
                        }

                        if (scale.StartAngle + scale.GapSweepAngle - this.AnglePosition >= 180 && this.AnglePosition >= scale.StartAngle)
                        {
                            isArcLarge = true;
                        }
                        else
                        {
                            isArcLarge = false;
                        }
                    }

                    path = new PathGeometry();
                    Point startPoint1 = scale.ConvertToStageCoordinates(innerRadius, startangle, centerPoint);
                    Point endPoint1 = scale.ConvertToStageCoordinates(innerRadius, this.AnglePosition, centerPoint);
                    Point startPoint2 = scale.ConvertToStageCoordinates(outerRadius, startangle, centerPoint);
                    Point endPoint2 = scale.ConvertToStageCoordinates(outerRadius, this.AnglePosition, centerPoint);

                    path = new PathGeometry();

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = startPoint1;

                    ArcSegment arc = new ArcSegment();
                    arc.RotationAngle = 0;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = st;
                    arc.Point = startPoint1;
                    arc.Size = new Size(innerRadius, innerRadius);
                    figure1.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.RotationAngle = 0;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = st;
                    arc.Point = endPoint1;
                    arc.Size = new Size(innerRadius, innerRadius);
                    figure1.Segments.Add(arc);

                    LineSegment line = new LineSegment();
                    line.Point = endPoint2;
                    figure1.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.RotationAngle = 0;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = et;
                    arc.Point = endPoint2;
                    arc.Size = new Size(outerRadius, outerRadius);
                    figure1.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.RotationAngle = 0;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = et;
                    arc.Point = startPoint2;
                    arc.Size = new Size(outerRadius, outerRadius);
                    figure1.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = startPoint1;
                    figure1.Segments.Add(line);

                    (path as PathGeometry).Figures.Add(figure1);

                }
                else
                {
                    if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                    {
                        if (this.AnglePosition - scale.StartAngle < (scale.StartAngle + scale.GapSweepAngle) + 360 - this.AnglePosition)
                        {
                            this.AnglePosition = scale.StartAngle + scale.GapSweepAngle;
                        }
                        else
                        {
                            this.AnglePosition = scale.StartAngle;
                        }
                    }
                    else
                    {
                        if (scale.StartAngle - this.AnglePosition < this.AnglePosition - (scale.StartAngle + scale.GapSweepAngle) + 360)
                        {
                            this.AnglePosition = scale.StartAngle + 0.5;
                        }
                        else
                        {
                            this.AnglePosition = scale.StartAngle + scale.GapSweepAngle;
                        }
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Gets the geometry to draw the marker pointer.
        /// </summary>
        /// <remarks>
        /// Appropriate PathFigure for various Marker.Styles are drawn.
        /// </remarks>
        /// <returns>
        /// The geometry object.
        /// </returns>
        private Geometry GetMarkerPath()
        {
            PathGeometry path = new PathGeometry();

            if (this.MarkerStyle == MarkerStyle.Rectangle)
            {
                path.AddGeometry(new RectangleGeometry(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height)));
            }
            else if (this.MarkerStyle == MarkerStyle.Ellipse)
            {
                path.AddGeometry(new EllipseGeometry(
                    new Point(this.PointerLength / 2, this.PointerWidth / 2), this.PointerLength / 2, this.PointerWidth / 2));
            }
            else if (this.MarkerStyle == MarkerStyle.Triangle)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth), true);
                LineSegment l4 = new LineSegment(new Point(0, this.PointerWidth / 2), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4 },
                    true);

                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Diamond)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength / 2, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth / 2), true);
                LineSegment l4 = new LineSegment(new Point(this.PointerLength / 2, this.PointerWidth), true);
                LineSegment l5 = new LineSegment(new Point(0, this.PointerWidth / 2), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4 },
                    true);

                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Pentagon)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength / 2, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth / 4), true);
                LineSegment l4 = new LineSegment(new Point(this.PointerLength, 3 * this.PointerWidth / 4), true);
                LineSegment l5 = new LineSegment(new Point(this.PointerLength / 2, this.PointerWidth), true);
                LineSegment l6 = new LineSegment(new Point(0, this.PointerWidth / 2), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4, l5, l6 },
                    true);

                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Trapezoid)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 4), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth), true);
                LineSegment l4 = new LineSegment(new Point(0, 3 * this.PointerWidth / 4), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4 },
                    true);

                path.Figures.Add(figure1);
            }

            else if (this.MarkerStyle == MarkerStyle.Custom)
            {
                if (MarkerCustomGeometry != null)
                {
                    return MarkerCustomGeometry;
                }
                else
                {
                    LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 4), true);
                    LineSegment l2 = new LineSegment(new Point(this.PointerLength, 0), true);
                    LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth), true);
                    LineSegment l4 = new LineSegment(new Point(0, 3 * this.PointerWidth / 4), true);

                    PathFigure figure1 = new PathFigure(
                        new Point(0, this.PointerWidth / 2),
                        new PathSegment[] { l1, l2, l3, l4 },
                        true);

                    path.Figures.Add(figure1);
                }
            }

            return path;
        }

        /// <summary>
        /// Gets the geometry to draw the needle pointer.
        /// </summary>
        /// <remarks>
        /// Appropriate PathFigure of various NeedleStyles are set
        /// </remarks>
        /// <returns>
        /// The geometry object.
        /// </returns>
        private Geometry GetNeedlePath()
        {
            PathGeometry path = new PathGeometry();
            if (this.NeedleStyle == NeedleStyle.Triangle)
            {
                LineSegment l1 = new LineSegment(new Point(0, 0), true);
                LineSegment l2 = new LineSegment(new Point(0, this.DesiredSize.Height), true);
                LineSegment l3 = new LineSegment(new Point(this.DesiredSize.Width, this.DesiredSize.Height / 2), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, 0),
                    new PathSegment[] { l1, l2, l3 },
                    true);

                path.Figures.Add(figure1);
            }
            else if (this.NeedleStyle == NeedleStyle.Rectangle)
            {
                path.AddGeometry(new RectangleGeometry(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height)));
            }
            else if (this.NeedleStyle == NeedleStyle.Trapezoid)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength, this.PointerWidth * 0.7), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth * 0.3), true);
                LineSegment l4 = new LineSegment(new Point(0, 0), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth),
                    new PathSegment[] { l1, l2, l3, l4 },
                   true);

                path.Figures.Add(figure1);
            }
            else if (this.NeedleStyle == NeedleStyle.Arrow)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth * 0.63), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength * 0.83, this.PointerWidth * 0.63), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength * 0.78, this.PointerWidth), true);
                LineSegment l4 = new LineSegment(new Point(this.PointerLength, this.PointerWidth / 2), true);
                LineSegment l5 = new LineSegment(new Point(this.PointerLength * 0.78, 0), true);
                LineSegment l6 = new LineSegment(new Point(this.PointerLength * 0.83, this.PointerWidth * 0.37), true);
                LineSegment l7 = new LineSegment(new Point(0, this.PointerWidth * 0.37), true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 3),
                    new PathSegment[] { l1, l2, l3, l4, l5, l6, l7 },
                    true);

                path.Figures.Add(figure1);
            }
            else if (this.NeedleStyle == NeedleStyle.Custom)
            {
                if (NeedleCustomGeometry != null)
                {
                    return NeedleCustomGeometry;
                }
                else
                {
                    LineSegment l1 = new LineSegment(new Point(0, 0), true);
                    LineSegment l2 = new LineSegment(new Point(0, this.DesiredSize.Height), true);
                    LineSegment l3 = new LineSegment(new Point(this.DesiredSize.Width, this.DesiredSize.Height / 2), true);

                    PathFigure figure1 = new PathFigure(
                        new Point(0, 0),
                        new PathSegment[] { l1, l2, l3 },
                        true);
                    path.Figures.Add(figure1);
                }
            }

            return path;
        }

        /// <summary>
        /// Method to diable the MouseMove events
        /// </summary>
        /// <param name="path">path to which mouse events are added</param>
        internal void DisableMouseEvents(CircularPointer path)
        {
            this.Cursor = Cursors.Arrow;
            path.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonUp);
            path.MouseMove -= new System.Windows.Input.MouseEventHandler(CircularPointer_MouseMove);
            path.Focusable = false;
            path.KeyDown -= new KeyEventHandler(CircularPointer_KeyDown);
        }

        /// <summary>
        /// Method to enable Mousemove events.
        /// </summary>
        /// <param name="path">path to which mouse events are removed</param>
        internal void EnableMouseEvents(CircularPointer path)
        {
            this.Cursor = Cursors.Hand;
            path.Focusable = true;
            path.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonUp);
            path.MouseMove += new System.Windows.Input.MouseEventHandler(CircularPointer_MouseMove);
            path.KeyDown += new KeyEventHandler(CircularPointer_KeyDown);

        }


        /// <summary>
        /// Coerces the value of the <see cref="PointerLength"/> property.
        /// </summary>
        /// <param name="d">The <see cref="CircularPointer"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoercePointerLength(DependencyObject d, object value)
        {
            CircularPointer owner = d as CircularPointer;
            return owner.CoercePointerLength(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="PointerLength"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoercePointerLength(object value)
        {
            double val = (double)value;
            if (val < 0)
            {
                val = 0;
            }

            return val;
        }

        /// <summary>
        /// Coerces the value of the <see cref="Value"/> property.
        /// </summary>
        /// <param name="d">The <see cref="CircularPointer"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceValue(DependencyObject d, object value)
        {
            CircularPointer owner = d as CircularPointer;
            return owner.CoerceValue(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Value"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceValue(object value)
        {
            double val = (double)value;
            if (this.VisualParent is CircularScale)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                if (val < scale.Minimum)
                {
                    val = scale.Minimum;
                }

                if (val > scale.Maximum)
                {
                    val = scale.Maximum;
                }

            }
            else
            {
                flag = true;
            }

            return val;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AnglePositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAnglePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshAnglePosition();
            }

            if (AnglePositionChanged != null)
            {
                AnglePositionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IncrementKeyChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIncrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IncrementKeyChanged != null)
            {
                IncrementKeyChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DecrementKeyChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDecrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DecrementKeyChanged != null)
            {
                DecrementKeyChanged(this, e);
            }
        }


        /// <summary>
        /// Calls OnAnglePositionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAnglePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnAnglePositionChanged(e);
        }

        /// <summary>
        /// Calls OnIncrementKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIncrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnIncrementKeyChanged(e);
        }


        /// <summary>
        /// Calls OnDecrementKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDecrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnDecrementKeyChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MarkerStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMarkerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MarkerStyleChanged != null)
            {
                MarkerStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMarkerStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMarkerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnMarkerStyleChanged(e);
        }

        /// <summary>
        /// Raises <see cref="CircularPointer.NeedleCustomGeometryChanged"/> event
        /// </summary>
        /// <param name="e">Contains data related to the event</param>
        private void OnNeedleCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (NeedleCustomGeometryChanged != null)
            {
                this.OnNeedleCustomGeometryChanged(e);
            }
        }

        /// <summary>
        /// Calls OnNeedleCustomGeometryChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnNeedleCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnNeedleCustomGeometryChanged(e);
        }

        /// <summary>
        /// Raises <see cref="CircularPointer.NeedleCustomGeometryChanged"/> event
        /// </summary>
        /// <param name="e">Contains data related to the event</param>
        private void OnMarkerCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MarkerCustomGeometryChanged != null)
            {
                this.OnMarkerCustomGeometryChanged(e);
            }
        }

        /// <summary>
        /// Calls OnNeedleCustomGeometryChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMarkerCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnMarkerCustomGeometryChanged(e);
        }
        /// <summary>
        /// Calls OnNeedleStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnNeedleStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnNeedleStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="NeedleStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnNeedleStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (NeedleStyleChanged != null)
            {
                this.NeedleStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPointerLengthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnPointerLengthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerLengthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshAnglePosition();
            }

            if (PointerLengthChanged != null)
            {
                PointerLengthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerNeedleTypeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerNeedleTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshAnglePosition();
            }

            if (PointerNeedleTypeChanged != null)
            {
                PointerNeedleTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPointerNeedleTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerNeedleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnPointerNeedleTypeChanged(e);
        }

        /// <summary>
        /// Calls OnPointerPlacementChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnPointerPlacementChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshAnglePosition();
            }

            if (PointerPlacementChanged != null)
            {
                PointerPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshAnglePosition();
            }

            if (PointerWidthChanged != null)
            {
                PointerWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="EnablePointerInteractionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEnablePointerInteractionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshAnglePosition();
            }

            if (EnablePointerInteractionChanged != null)
            {
                EnablePointerInteractionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPointerWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnPointerWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="ValueChanged">ValueChanged</see> event.
        /// </summary>
        /// <remarks>
        /// Animation is provided whenever the value of the pointer changes. Animation is
        /// provided to the pointer between pointer's old position and its new position.
        /// </remarks>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                double oldValue = (double)e.OldValue;
                if (this.VisualParent is CircularScale)
                {
                    CircularScale scale = this.VisualParent as CircularScale;
                    if (oldValue < scale.Minimum)
                        oldValue = scale.Minimum;
                    if (oldValue > scale.Maximum)
                        oldValue = scale.Maximum;
                }
                //double oldAngle = this.GetAngleByValue((double)e.OldValue);
                double oldAngle = this.GetAngleByValue(oldValue);
                double newAngle = this.GetAngleByValue(this.Value);
                double duration;
                if (this.IsAnimationEnabled == true)
                {
                    DoubleAnimation positionAnimation = new DoubleAnimation();
                    positionAnimation.FillBehavior = FillBehavior.Stop;
                    positionAnimation.From = oldAngle;
                    positionAnimation.To = newAngle;

                    if (Double.IsNaN(this.AnimationDuration) == true)
                    {
                        duration = Math.Abs(newAngle - oldAngle) * 3;
                        if (duration < 300)
                        {
                            duration = 300;
                        }
                    }
                    else
                    {
                        duration = this.AnimationDuration;
                    }

                    positionAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                    positionAnimation.AccelerationRatio = 0.1;
                    positionAnimation.DecelerationRatio = 0.1;                   
                    positionAnimation.Completed += new EventHandler(PositionAnimationCompleted);
                    this.BeginAnimation(AnglePositionProperty, positionAnimation);
                }
                this.AnglePosition = newAngle;
                m_valueArgs = e;
                this.RefreshAnglePosition();
            }
            else
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnValueChanged(e);
        }

        /// <summary>
        /// Calls OnEnablePointerInteractionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnablePointerInteractionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnEnablePointerInteractionChanged(e);
            if (instance != null)
            {
                if (instance.EnablePointerInteraction == true)
                {
                    instance.EnableMouseEvents(instance);
                }
                else
                {
                    instance.DisableMouseEvents(instance);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void PositionAnimationCompleted(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, m_valueArgs);
            }
        }

        /// <summary>
        /// Updates the angle position of the pointer.
        /// </summary>
        /// <remarks>
        /// The pointer is set to appropriate angle based on the value of the pointer. If
        /// the PointerType is Needle, then a simple rotation of the needle to the
        /// appropriate angle is enough. If it is of type Marker, then based on the
        /// pointer's ScalePlacement appropriate X position is calculated(Note: All the
        /// calculations are done with regard to the original position of the pointer, i.e
        /// Center of the Gauge) and the Marker is rotated.
        /// </remarks>
        protected internal virtual void RefreshAnglePosition()
        {
            if (flag)
            {
                CheckPointerValue();
            }
            this.AnglePosition = this.GetAngleByValue(this.Value);
            if (this.PointerNeedleType == PointerNeedleType.Needle)
            {               
                this.RenderTransform = new RotateTransform(this.AnglePosition, 0, this.PointerWidth / 2);
            }
            else if (this.PointerNeedleType == PointerNeedleType.Marker)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                double angleToPlace = this.AnglePosition;
                double posX = 0;
                if (scale != null)
                {
                    if (this.PointerPlacement == ScalePlacement.Cross)
                    {
                        posX = ((scale.ScaleBarSize + this.DesiredSize.Width) / 2) - scale.Radius;
                    }
                    else if (this.PointerPlacement == ScalePlacement.Inside)
                    {
                        posX = this.DesiredSize.Width;
                        if (scale.Radius > scale.ScaleBarSize)
                        {
                            posX += scale.ScaleBarSize - scale.Radius;
                        }
                    }
                    else if (this.PointerPlacement == ScalePlacement.Outside)
                    {
                        posX = -scale.Radius;
                    }

                    this.RenderTransform = new RotateTransform(angleToPlace, posX, this.DesiredSize.Height / 2);
                }
            }
            else
            {
                this.RenderTransform = new RotateTransform(0, 0, 0);
            }
        }

        /// <summary>
        /// Calls OnPointerWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAnimationDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnAnimationDurationChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AnimationDurationChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAnimationDurationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AnimationDurationChanged != null)
            {
                AnimationDurationChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsAnimationEnabledChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsAnimationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            instance.OnIsAnimationEnabledChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsAnimationEnabledChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsAnimationEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAnimationEnabledChanged != null)
            {
                IsAnimationEnabledChanged(this, e);
            }
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="CircularGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(CircularGauge gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }

        private void CheckPointerValue()
        {
            double val = (double)this.Value;
            if (this.VisualParent is CircularScale)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                if (val < scale.Minimum)
                {
                    val = scale.Minimum;
                }

                if (val > scale.Maximum)
                {
                    val = scale.Maximum;
                }
                this.Value = val;
            }
        }
        #endregion Added Code
    }
}
