#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Input;

    /// <summary>
    /// Represents the pointer visual element.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge; 
    /// <para></para>
    /// <para>CircularPointer circularpointer = new CircularPointer(); </para>
    /// <para>circularpointer.Background = new SolidColorBrush( Color.FromArgb( 255, 194, 207, 229 ) ); </para>
    /// <para>    circularpointer.PointerPlacement = ScalePlacement.Outside; </para>
    /// <para>     circularpointer.PointerLength = 100; </para>
    /// <para>     circularpointer.PointerWidth = 15; </para>
    /// <para>        circularscale.Pointers.Add( circularpointer ); </para></description></item></list>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot; 
    /// <para></para>
    /// <para>&lt;syncfusion:CircularScale.Pointers&gt; </para>
    /// <para>   &lt;syncfusion:CircularPointer Background=&quot;Blue&quot;  NeedleStyle=&quot;Triangle&quot; MarkerStyle=&quot;Diamond&quot; Name=&quot;m_pointer1&quot;  BorderBrush=&quot;Silver&quot;  PointerLength=&quot;100&quot;  PointerWidth=&quot;20&quot; </para>
    /// <para>PointerNeedleType=&quot;Needle&quot; PointerPlacement=&quot;Inside&quot;/&gt; </para>
    /// <para>    &lt;/syncfusion:CircularScale.Pointers&gt; </para></description></item></list>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// </example>
    public class CircularPointer : PointerBase
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.MarkerStyle">MarkerStyle</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Pointer can be customized Marker styles and the default MarkerStyle is Triangle.In this Pointer Style,there are various options like <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Diamond">Diamond</see>, <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Ellipse">Ellipse</see>, <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Pentagon">Pentagon</see>, <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Rectangle">Rectange</see>,<see
        /// cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Trapezoid">Trapezoid</see> and <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Triangle">Triangle</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.MarkerStyle">MarkerStyle</see></para>
        /// </returns>
        public static readonly DependencyProperty MarkerStyleProperty =
            DependencyProperty.Register("MarkerStyle", typeof(MarkerStyle), typeof(CircularPointer), new PropertyMetadata(MarkerStyle.Triangle, new PropertyChangedCallback(OnMarkerStyleChanged)));

        /// <summary>
        /// Identifies the  dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Pointer can be customized into different Needle styles and the default style is Triangle.In this Pointer Style,there are various options like <see cref="F:Syncfusion.Windows.Gauge.NeedleStyle.Triangle">Triangle</see>, <see cref="F:Syncfusion.Windows.Gauge.NeedleStyle.Rectangle">Rectangle</see>, <see cref="F:Syncfusion.Windows.Gauge.NeedleStyle.Arrow">Arrow</see> and <see cref="F:Syncfusion.Windows.Gauge.NeedleStyle.Trapezoid">Trapezoid</see>. </para>
        /// </remarks>
        /// <returns>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.NeedleStyle">NeedleStyle</see>
        /// </returns>
        public static readonly DependencyProperty NeedleStyleProperty =
            DependencyProperty.Register("NeedleStyle", typeof(NeedleStyle), typeof(CircularPointer), new PropertyMetadata(NeedleStyle.Triangle, new PropertyChangedCallback(OnNeedleStyleChanged)));

        /// <summary>
        /// Identifies the  <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerLength">PointerLength</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The Default length of the pointer is 150d.</para>
        /// </remarks>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty PointerLengthProperty =
            DependencyProperty.Register("PointerLength", typeof(double), typeof(CircularPointer), new PropertyMetadata(110d, new PropertyChangedCallback(OnPointerLengthChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerNeedleType">PointerNeedleType</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Pointer can be customized into any type of needle style.The default Pointer style is <see cref="F:Syncfusion.Windows.Gauge.PointerNeedleType.Needle">Needle</see>.The other options for Pointer needle styles are <see cref="F:Syncfusion.Windows.Gauge.PointerNeedleType.Marker">Marker</see> and <see cref="F:Syncfusion.Windows.Gauge.PointerNeedleType.Bar">Bar</see> Pointers.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type: <see cref="T:Syncfusion.Windows.Gauge.PointerNeedleType">PointerNeedleType</see></para>
        /// </returns>
        public static readonly DependencyProperty PointerNeedleTypeProperty =
            DependencyProperty.Register("PointerNeedleType", typeof(PointerNeedleType), typeof(CircularPointer), new PropertyMetadata(PointerNeedleType.Needle, new PropertyChangedCallback(OnPointerNeedleTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerPlacementProperty =
            DependencyProperty.Register("PointerPlacement", typeof(ScalePlacement), typeof(CircularPointer), new PropertyMetadata(ScalePlacement.Inside, new PropertyChangedCallback(OnPointerPlacementChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerWidth">PointerWidth</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default value for the pointer width is 20d.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty PointerWidthProperty =
            DependencyProperty.Register("PointerWidth", typeof(double), typeof(CircularPointer), new PropertyMetadata(20d, new PropertyChangedCallback(OnPointerWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.Value">Value</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Value is property to set the pointer value.The default value is 0d.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see> </para>
        /// </returns>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged)));
        
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.EnablePointerInteraction">EnablePointerInteraction</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">System.Boolean</see>
        /// </returns>
        public static readonly DependencyProperty EnablePointerInteractionProperty =
            DependencyProperty.Register("EnablePointerInteraction", typeof(bool), typeof(CircularPointer), new PropertyMetadata(true, new PropertyChangedCallback(OnEnablePointerInteractionChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.AnglePosition">AnglePostion</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type: <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        internal static readonly DependencyProperty AnglePositionProperty =
            DependencyProperty.Register("AnglePosition", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, new PropertyChangedCallback(OnAnglePositionChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerSelectionBrush">PointerSelectionBrush</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type: <see cref="T:System.Brush">Brush</see></para>
        /// </returns>


        public  static readonly DependencyProperty PointerSelectionBrushProperty =
            DependencyProperty.Register("PointerSelectionBrush", typeof(Brush), typeof(CircularPointer), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnPointerSelectionBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.IncrementKey">IncrementKey</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type: <see cref="T:System.Key">Key</see></para>
        /// </returns>

        public static readonly DependencyProperty IncrementKeyProperty =
            DependencyProperty.Register("IncrementKey", typeof(Key), typeof(CircularPointer), new PropertyMetadata(Key.Up,new PropertyChangedCallback(OnIncrementKeyChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.DecrementKey">DecrementKey</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type: <see cref="T:System.Key">Key</see></para>
        /// </returns>

        public static readonly DependencyProperty DecrementKeyProperty =
            DependencyProperty.Register("DecrementKey", typeof(Key), typeof(CircularPointer), new PropertyMetadata(Key.Down, new PropertyChangedCallback(OnDecrementKeyChanged)));


       


        #endregion

        #region Private members

        /// <summary>
        /// To check whether loaded or not
        /// </summary>
        private bool misLoaded = false;

        /// <summary>
        /// To indicate whether loaded or not
        /// </summary>
        private bool misRotated = false;

        /// <summary>
        /// Temp variable for storing clicked event. 
        /// </summary>
        private bool mclick = false;

        
        /// <summary>
        /// Temp variable for storing Pointer border brush color. 
        /// </summary>
        private Brush pointerBrush = null;

        /// <summary>
        /// Temp variable for storing Increase key. 
        /// </summary>

        private Thickness pointerThickness = new Thickness(0);

      

        private Key increaseKey;

        /// <summary>
        /// Temp variable for storing Decrease key. 
        /// </summary>
        private Key decreaseKey;

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        private Geometry mneedleGeometry;

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        private Geometry mneedleGeometryShadow;

        private Geometry prevpath;

        /// <summary>
        /// The variable user the count the level
        /// </summary>
        private int mnestLevel = 0;

        /// <summary>
        /// The path used for pointer
        /// </summary>
        private Path mpointerPath;

        /// <summary>
        /// The path used for pointer shadow
        /// </summary>
        private Path mpointerPathShadow;

        /// <summary>
        /// Event arguments used to fire the ValueChanged event.
        /// </summary>
        private RoutedPropertyChangedEventArgs<double> mvalueArgs;

        


        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.CircularPointer">CircularPointer</see> class.
        /// </summary>
        public CircularPointer()
        {

            DefaultStyleKey = typeof(CircularPointer);
            this.Loaded += new RoutedEventHandler(this.CircularPointerLoaded);
            Canvas.SetZIndex(this, 1);
            this.LostFocus+=new RoutedEventHandler(CircularPointer_LostFocus);
        }
        #endregion
              
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.AnglePosition">AnglePosition</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback AnglePositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.MarkerStyle">MarkerStyle</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.NeedleStyle">NeedleStyle</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback NeedleStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerLength">PointerLength</see>  property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> PointerLengthChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerNeedleType">PointerNeedleType</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerNeedleTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerPlacement">PointerPlacement</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerPlacementChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerWidth">PointerWidth</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.Value">Value</see> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.EnablePointerInteraction">EnablePointerInteraction</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnablePointerInteractionChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerSelectionBrush">PointerSelectionBrush</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerSelectionBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.IncrementKey">IncrementKey</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback IncrementKeyChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.DecrementKey">DecrementKey</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback DecrementKeyChanged;

        
        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets different shapes for the marker pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Triangle">MarkerStyle.Triangle</see>.</para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.MarkerStyle">MarkerStyle</see> </para>
        /// </value>
        /// <seealso cref="MarkerStyle">MarkerStyle</seealso>
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
        /// Gets or sets different shapes for the needle pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <see cref="F:Syncfusion.Windows.Gauge.NeedleStyle">NeedleStyle.Triangle</see>.</para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:Syncfusion.Windows.Gauge.NeedleStyle">NeedleStyle</see>
        /// </value>
        /// <seealso cref="NeedleStyle">NeedleStyle</seealso>
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
        /// Gets or sets the length of the pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value for pointer length is 0d.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see cref="T:System.Double">System.Double</see></para>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets different types of the pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default value for Pointer Needle Type is <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerNeedleType">PointerNeedleType.Needle</see>.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see cref="T:Syncfusion.Windows.Gauge.PointerNeedleType">PointerNeedleType</see> </para>
        /// </value>
        /// <seealso cref="PointerNeedleType">PointerNeedleType</seealso>
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
        /// Gets or sets the placement of the marker pointer relatively to the scale. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <see cref="F:Syncfusion.Windows.Gauge.ScalePlacement.Inside">ScalePlacement.Inside</see>.</para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.ScalePlacement">ScalePlacement</see>  </para>
        /// </value>
        /// <seealso cref="ScalePlacement">ScalePlacement</seealso>
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
        /// Gets or sets the width of the pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value for Pointer width is 0d. </para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets the pointer value to indicate. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value for pointer is 0d.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.Value">Value</see></para>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets a value indicating whether to enable PointerInteraction.
        /// </summary>
        /// <remarks>
        /// <para>Default value is true. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Boolean">System.Boolean</see>
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
        /// Gets or sets the angle to move the pointer to.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
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
        /// Gets or sets the Brush to Highlight Selection.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Green.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush PointerSelectionBrush
        {
            get
            {
                return (Brush)GetValue(PointerSelectionBrushProperty);
            }

            set
            {
                SetValue(PointerSelectionBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Key to Increase the value
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Key"/>
        /// Default value is Key.Up.
        /// </value>
        /// <seealso cref="Key"/>

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
        /// Gets or sets the Key to Decrease the value
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Key"/>
        /// Default value is Key.Down.
        /// </value>
        /// <seealso cref="Key"/>
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
        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.PointerNeedleType == PointerNeedleType.Needle)
            {
                this.mneedleGeometry = this.GetNeedlePath();
                this.mneedleGeometryShadow = this.GetNeedlePath();
            }
            else if (this.PointerNeedleType == PointerNeedleType.Marker)
            {
                this.mneedleGeometry = this.GetMarkerPath();
                this.mneedleGeometryShadow = this.GetMarkerPath();
            }
            else if (this.PointerNeedleType == PointerNeedleType.Bar)
            {
                this.mneedleGeometry = this.GetBarPath();
                this.mneedleGeometryShadow = this.GetBarPath();
            }

            this.mpointerPath = this.GetTemplateChild("PART_PointerPath") as Path;
            this.mpointerPathShadow = this.GetTemplateChild("PointerPathShadow") as Path;
            
            if (this.mpointerPath != null)
            {
                this.mpointerPath.Data = this.mneedleGeometry;
            }

            if (this.mpointerPathShadow != null)
            {
                this.mpointerPathShadow.Data = this.mneedleGeometryShadow;
                this.mpointerPathShadow.Fill = new SolidColorBrush(Color.FromArgb(100, 123, 123, 118));
            }

            if (this.EnablePointerInteraction)
            {
                EnableMouseEvents(this.mpointerPath);
            }
            
            //CircularScale scale = this.GaugeElementParent as CircularScale;
            //if (scale != null)
            //{
            //    CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
            //    rotateangle = Math.Tanh((gauge.OuterFrameOffset + gauge.InnerFrameOffset) / this.PointerLength);
            //}
            this.TabIndex = 0;
            this.UpdateVisualStyle();
        }

        /// <summary>
        /// This method updates visual style
        /// </summary>
        /// <remarks></remarks>
        protected override void UpdateVisualStyle()
        {
            if (this.GaugeElementParent != null)
                this.VisualStyle = ((this.GaugeElementParent as CircularScale).GaugeElementParent as GaugeBase).VisualStyle;
            base.UpdateVisualStyle();
        }

        /// <summary>
        /// Change the value of the Pointer according to the Mouse Move
        /// </summary>
        void CircularPointer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.EnablePointerInteraction == true)
            {
                if (this.mclick == true)
                {
                    if (this.GaugeElementParent is ScaleBase)
                    {
                        CircularScale scale = this.GaugeElementParent as CircularScale;
                        Point p = e.GetPosition(this.GaugeElementParent);
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
                        else
                        {
                            if ((this.Value - scale.Minimum) > (scale.Maximum - this.Value))
                            {
                                this.Value = scale.Maximum;
                            }
                            else
                            {
                                this.Value = scale.Minimum;
                            }
                        }                       

                        if (this.AnglePosition > scale.gapSweepAngle+scale.startAngle)
                        {
                            this.AnglePosition -= 360;
                        }

                        this.RefreshAnglePosition();
                    }
                }
            }
        }

       
        void CircularPointer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(Path))
            {
                Path el = sender as Path;
                el.ReleaseMouseCapture();
                this.mclick = false;
            }
        }
       

        void CircularPointer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            CircularPointer pointer = this as CircularPointer;
            CircularScale scale = pointer.GaugeElementParent as CircularScale;
            if(pointer.Focus()==false)
            pointer.Focus();
                if (sender.GetType() == typeof(Path))
                {
                    Path el = sender as Path;
                    el.CaptureMouse();
                    if (this.GaugeElementParent is ScaleBase)
                    {

                        Point p = e.GetPosition(this.GaugeElementParent);
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

      

        #endregion

        #region Implementation

        /// <summary>
        /// Refreshes the Pointer
        /// </summary>
        internal void RefreshPointer()
        {
            if (this.PointerNeedleType == PointerNeedleType.Needle)
            {
                this.mneedleGeometry = this.GetNeedlePath();
                this.mneedleGeometryShadow = this.GetNeedlePath();
            }
            else if (this.PointerNeedleType == PointerNeedleType.Marker)
            {
                this.mneedleGeometry = this.GetMarkerPath();
                this.mneedleGeometryShadow = this.GetMarkerPath();
            }
            else if (this.PointerNeedleType == PointerNeedleType.Bar)
            {

                this.mneedleGeometry = this.GetBarPath();
                this.mneedleGeometryShadow = this.GetBarPath();
            }

            if (this.mpointerPath != null)
            {
                this.mpointerPath.Data = this.mneedleGeometry;
            }

            if (this.mpointerPathShadow != null)
            {
                this.mpointerPathShadow.Data = this.mneedleGeometryShadow;
                this.mpointerPathShadow.Fill = new SolidColorBrush(Color.FromArgb(100, 123, 123, 118));
            }

            if (this.GaugeElementParent is CircularScale)
            {
                CircularScale scale = this.GaugeElementParent as CircularScale;
                scale.RefreshScale();
            }

            this.RotateMarkerPointer();
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="PointerLength"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual double CoercePointerLength(double value)
        {
            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Value"/> dependency property.
        /// </summary>
        /// <param name="val">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual double CoerceValue(double val)
        {
            CircularScale scale = this.GaugeElementParent as CircularScale;
            if (this.misLoaded)
            {
                if (scale != null)
                {
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
                    val = 0;
                }
            }

            return val;
        }

        /// <summary>
        /// Method to diable the MouseMove and Key board events
        /// </summary>
       /// <param name="path">path to which mouse and Key board events are added</param>
        internal void DisableMouseEvents(Path path)
        {
            this.Cursor = Cursors.Arrow;
            if (path != null)
            {
                path.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonDown);
                path.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonUp);
                path.MouseMove -= new System.Windows.Input.MouseEventHandler(CircularPointer_MouseMove);
                this.KeyDown -= new KeyEventHandler(CircularPointer_KeyDown);
            }
        }

        /// <summary>
        /// Method to enable Mousemove and Key board events.
        /// </summary>
        /// <param name="path">path to which mouse and key board events are removed</param>
        internal void EnableMouseEvents(Path path)
        {
            this.Cursor = Cursors.Hand;
            path.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(CircularPointer_MouseLeftButtonUp);
            path.MouseMove += new System.Windows.Input.MouseEventHandler(CircularPointer_MouseMove);
            this.KeyDown += new KeyEventHandler(CircularPointer_KeyDown);
            this.GotFocus += new RoutedEventHandler(CircularPointer_GotFocus);
            this.LostFocus += new RoutedEventHandler(CircularPointer_LostFocus);
           
        }

      
       

        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    base.OnLostFocus(e);
        //    CircularPointer pointer = this as CircularPointer;

        //    if (pointer.pointerBrush != null)
        //        pointer.BorderBrush = pointer.pointerBrush;
        //    if (pointer.pointerThickness != null)
        //        pointer.BorderThickness = pointer.pointerThickness;




        //    else { }
        //    pointer.temp = 0;
        //}
        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    CircularPointer pointer = this as CircularPointer;

        //    //if (pointer.pointerBrush != null)
        //    //{
        //        pointer.pointerBrush = pointer.BorderBrush;
        //        pointer.BorderBrush = pointer.highlightBrush;
        //    //}
        //    //if (pointer.pointerThickness != null)
        //    //{
        //        pointer.pointerThickness = pointer.BorderThickness;
        //        pointer.BorderThickness = new Thickness(1); ;
        //    //}
        //}

        /// <summary>
        /// Method to do when pointer lost the focus.
        /// </summary>

        void CircularPointer_LostFocus(object sender, RoutedEventArgs e)
        {
             CircularPointer pointer = (CircularPointer)sender;

             if (pointer.pointerBrush != null)
                 pointer.BorderBrush = pointer.pointerBrush;
             if (pointer.pointerThickness != null)
                 pointer.BorderThickness = pointer.pointerThickness;


                

             else { }
            
            


        }

        /// <summary>
        /// Method to do when pointer Got the focus.
        /// </summary>
        void CircularPointer_GotFocus(object sender, RoutedEventArgs e)
        {
            CircularPointer pointer = sender as CircularPointer;
            CircularScale scale = pointer.GaugeElementParent as CircularScale;
           


          

                pointer.pointerThickness = pointer.BorderThickness;

                pointer.BorderThickness = new Thickness(1);

                pointer.pointerBrush = pointer.BorderBrush;
                pointer.BorderBrush = pointer.PointerSelectionBrush;
                pointer.Focus();
           
            pointer.Focus();
           

        }


        /// <summary>
        /// Method to change the value of the pointer
        /// </summary>
        void CircularPointer_KeyDown(object sender, KeyEventArgs e)
        {

            this.Focus();
            CircularPointer pointer;
            pointer = sender as CircularPointer;
            if ((e.Key == Key.Tab)||((e.Key==Key.Shift)&&(e.Key==Key.Tab)))
            {
            }
            else
            {

                CircularScale scale = pointer.GaugeElementParent as CircularScale;
                if (scale != null)
                {
                    if ((e.Key == Key.Left) || (e.Key == Key.Down) || (e.Key == pointer.decreaseKey) || (e.Key == Key.PageDown))
                    {
                        e.Handled = true;
                        if (this.Value > scale.Minimum)
                            this.Value--;
                        
                    }
                    else if ((e.Key == Key.Right) || (e.Key == Key.Up) || (e.Key == pointer.increaseKey) || (e.Key == Key.PageUp))
                    {
                        e.Handled = true;
                        if (this.Value < scale.Maximum )
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
                   
                }
            }
        }



        /// <summary>
        /// Method to convert Angle to Value in circularknob.
        /// </summary>
        /// <param name="angle">Angle value for conversion</param>
        /// <param name="pointervalue">Pointer value for conversion</param>
        /// <returns>
        /// Value corresponding to that given angle
        /// </returns>
        protected internal virtual double AngleToValue(double angle, double pointervalue)
        {
            double value = 0;
            CircularScale scale = this.GaugeElementParent as CircularScale;
            if (scale != null)
            {
                if (scale.IsReversed)
                {
                    if (angle < scale.startAngle && angle > (scale.startAngle + scale.gapSweepAngle - 360))
                    {
                        if (angle < scale.startAngle && this.AnglePosition > scale.startAngle + scale.gapSweepAngle)
                        {
                            angle = scale.startAngle;
                            value = scale.Minimum;
                            this.Value = scale.Minimum;
                        }
                        else if (angle < scale.startAngle && this.AnglePosition <= (scale.startAngle + scale.gapSweepAngle - 360))
                        {
                            angle = scale.startAngle + scale.gapSweepAngle - 360 + 10;
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
                        if (angle < scale.startAngle)
                        {
                            angle = angle + 360;
                        }

                        double ratio = scale.gapSweepAngle / (angle - scale.startAngle);
                        value =scale.Maximum-(((scale.Maximum - scale.Minimum) / ratio) + scale.Minimum);
                    }
                }
                else
                {
                    if (angle < scale.startAngle && angle > (scale.startAngle + scale.gapSweepAngle - 360))
                    {
                        if (angle < scale.startAngle && this.AnglePosition > scale.startAngle + scale.gapSweepAngle)
                        {
                            angle = scale.startAngle;
                            value = scale.Minimum;
                            this.Value = scale.Minimum;
                        }
                        else if (angle < scale.startAngle && this.AnglePosition <= (scale.startAngle + scale.gapSweepAngle - 360))
                        {
                            angle = scale.startAngle + scale.gapSweepAngle - 360 + 10;
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
                        if (angle < scale.startAngle)
                        {
                            angle = angle + 360;
                        }

                        double ratio = scale.gapSweepAngle / (angle - scale.startAngle);
                        value = ((scale.Maximum - scale.Minimum) / ratio) + scale.Minimum;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Method to validate the angle value
        /// </summary>
        /// <param name="ang">angle for validation</param>
        /// <returns>
        /// Validated value
        /// </returns>
        protected internal virtual double CoerceAngle(double ang)
        {
            CircularScale scale = this.GaugeElementParent as CircularScale;
            if (this.misLoaded)
            {
                if (scale != null)
                {
                    if (ang < scale.startAngle && ang > (scale.startAngle + scale.gapSweepAngle  - 360))
                    {
                        if ((scale.startAngle - ang) <= (ang - (scale.startAngle + scale.gapSweepAngle - 360)))
                        {
                            ang = scale.startAngle;
                        }
                        else
                        {
                            ang = scale.startAngle + scale.gapSweepAngle- 360;
                        }
                    }
                }
                else
                {
                    ang = 0;
                }
            }

            return ang;
        }

        /// <summary>
        /// Gets the angle regarding the value.
        /// </summary>
        /// <param name="value">Pointer value.</param>
        /// <returns>Calculated angle.</returns>
        protected internal virtual double GetAngleByValue(double value)
        {
            double angle = 0;
            CircularScale scale = this.GaugeElementParent as CircularScale;
            if (scale != null)
            {
                angle = scale.startAngle;
                if (scale.IsReversed)
                {
                    if (scale.Minimum <= value && value <= scale.Maximum)
                    {
                        double ratio = (scale.Maximum - scale.Minimum) / (scale.Maximum-value);
                        angle = (scale.gapSweepAngle / ratio) + scale.startAngle;
                    }
                }
                else
                {
                    if (scale.Minimum <= value && value <= scale.Maximum)
                    {
                        double ratio = (scale.Maximum - scale.Minimum) / (value - scale.Minimum);
                        angle = (scale.gapSweepAngle / ratio) + scale.startAngle;
                    }
                }
            }

            return angle;
        }

        /// <summary>
        /// Updates the angle position of the pointer.
        /// </summary>
        protected internal virtual void RefreshAnglePosition()
        {
            if (this.GaugeElementParent is CircularScale)
            {
                CircularScale scale = this.GaugeElementParent as CircularScale;
                if (this.PointerNeedleType == PointerNeedleType.Needle)
                {
                    RotateTransform transform = new RotateTransform();
                    transform.Angle = this.AnglePosition;
                    transform.CenterX = 0;
                    transform.CenterY = this.PointerWidth / 2;
                    this.RenderTransform = transform;
                }
                else if (this.PointerNeedleType == PointerNeedleType.Marker)
                {
                    if (scale != null)
                    {
                        double angleToPlace = this.AnglePosition;
                        double posX = 0;
                        if (this.PointerPlacement == ScalePlacement.Cross)
                        {
                            posX = ((scale.ScaleBarSize + this.PointerLength) / 2) - scale.Radius;
                        }
                        else if (this.PointerPlacement == ScalePlacement.Inside)
                        {
                            posX = this.PointerLength;
                            if (scale.Radius > scale.ScaleBarSize)
                            {
                                posX += scale.ScaleBarSize - scale.Radius;
                            }
                        }
                        else if (this.PointerPlacement == ScalePlacement.Outside)
                        {
                            posX = -scale.Radius;
                        }

                        RotateTransform transform = new RotateTransform();
                        transform.Angle = angleToPlace;
                        transform.CenterX = posX;
                        transform.CenterY = this.PointerWidth / 2;
                        this.RenderTransform = transform;
                    }
                }
                else if (this.PointerNeedleType == PointerNeedleType.Bar)
                {
                    this.mneedleGeometry = this.GetBarPath();
                    this.mneedleGeometryShadow = this.GetBarPath();
                    if (this.mpointerPath != null)
                    {
                        this.mpointerPath.Data = this.mneedleGeometry;
                    }

                    if (this.mpointerPathShadow != null)
                    {
                        this.mpointerPathShadow.Data = this.mneedleGeometryShadow;
                    }

                    RotateTransform transform = new RotateTransform();
                    transform.Angle = 0;
                    transform.CenterX = 0;
                    transform.CenterY = 0;
                    this.RenderTransform = transform;
                }

                if (scale != null && this.mpointerPathShadow != null && !this.misRotated)
                {
                    Point shadowOffset = this.GetShadowOffset();
                    TranslateTransform transform2 = new TranslateTransform();
                    transform2.X = shadowOffset.X;
                    transform2.Y = shadowOffset.Y;
                    this.mpointerPathShadow.RenderTransform = transform2;
                }
            }
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            this.RefreshAnglePosition();
            return finalSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
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
        /// Updates property value cache and raises <see cref="AnglePositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAnglePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.misLoaded)
            {
                this.RefreshPointer();
            }

            if (this.AnglePositionChanged != null)
            {
                this.AnglePositionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MarkerStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMarkerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointer();
            if (this.MarkerStyleChanged != null)
            {
                this.MarkerStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="NeedleStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnNeedleStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointer();
            if (this.NeedleStyleChanged != null)
            {
                this.NeedleStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerLengthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerLengthChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.misLoaded)
            {
                this.RefreshPointer();
            }

            if (this.PointerLengthChanged != null)
            {
                this.PointerLengthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerNeedleTypeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerNeedleTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.misLoaded)
            {
                this.RefreshPointer();
            }

            if (this.PointerNeedleTypeChanged != null)
            {
                this.PointerNeedleTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.misLoaded)
            {
                this.RefreshPointer();
            }

            if (this.PointerPlacementChanged != null)
            {
                this.PointerPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointer();
            if (this.PointerWidthChanged != null)
            {
                this.PointerWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.misLoaded)
            {
                double oldAngle = this.GetAngleByValue(e.OldValue);
                double newAngle = this.GetAngleByValue(this.Value);
                DoubleAnimation positionAnimation = new DoubleAnimation();
                positionAnimation.From = oldAngle;
                positionAnimation.To = newAngle;
                double duration = Math.Abs(newAngle - oldAngle) * 2;
                if (duration < 300)
                {
                    duration = 300;
                }

                positionAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                positionAnimation.Completed += new EventHandler(this.PositionAnimationCompleted);
                Storyboard.SetTarget(positionAnimation, this);
                Storyboard.SetTargetProperty(positionAnimation, new PropertyPath("(CircularPointer.AnglePosition)"));

                Storyboard storyBoard = new Storyboard();
                storyBoard.Children.Add(positionAnimation);
                storyBoard.Begin();
                this.AnglePosition = newAngle;
                this.mvalueArgs = e;
                this.RefreshAnglePosition();
            }
            else
            {
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnEnablePointerInteractionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnablePointerInteractionChanged != null)
            {
                this.EnablePointerInteractionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnHighlightSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.PointerSelectionBrushChanged != null)
            {
                this.PointerSelectionBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnIncrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IncrementKeyChanged != null)
            {
                this.IncrementKeyChanged(this, e);
            }
        }
        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnDecrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DecrementKeyChanged != null)
            {
                this.DecrementKeyChanged(this, e);
            }
        }
        #endregion

        #region Private Methods

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
        /// Calls OnPointerLengthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;

            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;

            instance.mnestLevel++;
            double coercedValue = instance.CoercePointerLength(newValue);
            if (newValue != coercedValue)
            {
                instance.PointerLength = coercedValue;
            }

            instance.mnestLevel--;

            if (instance.mnestLevel == 0)
            {
                RoutedPropertyChangedEventArgs<double> args =
                    new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);

                instance.OnPointerLengthChanged(args);
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
        /// Calls OnValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer instance = (CircularPointer)d;
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            instance.mnestLevel++;
            double coercedValue = instance.CoerceValue(newValue);
            if (newValue != coercedValue)
            {
                instance.Value = coercedValue;
            }

            instance.mnestLevel--;
            if (instance.mnestLevel == 0)
            {
                RoutedPropertyChangedEventArgs<double> args =
                    new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);

                instance.OnValueChanged(args);
            }
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
            
            if (instance.mneedleGeometry != null)
            {
                if (instance.EnablePointerInteraction == true)
                {
                    instance.EnableMouseEvents(instance.mpointerPath);
                  
                    
                }
                else
                {
                    instance.DisableMouseEvents(instance.mpointerPath);
                }
            }
        }

        /// <summary>
        /// Calls OnHighlightSelectionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
     
        public static void OnPointerSelectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer pointer = (CircularPointer)d;
            pointer.OnHighlightSelectionChanged(e);
            

        }


        /// <summary>
        /// Calls OnIncreaseKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
     
        private static void OnIncrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer pointer = (CircularPointer)d;
            pointer.OnIncrementKeyChanged(e);
            pointer.increaseKey =(Key)e.NewValue;
            
        }


        /// <summary>
        /// Calls OnDecreaseKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>

        private static void OnDecrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPointer pointer = (CircularPointer)d;
            pointer.OnDecrementKeyChanged(e);
            pointer.decreaseKey = (Key)e.NewValue;

        }



        /// <summary>
        /// Invoked when the pointer is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void CircularPointerLoaded(object sender, RoutedEventArgs e)
        {
            this.misLoaded = true;
            this.RefreshPointer();
        }

        /// <summary>
        /// Gets the geometry to draw the bar pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private Geometry GetBarPath()
        {
            Geometry path = null;
            if (this.GaugeElementParent is CircularScale)
            {
                CircularScale scale = this.GaugeElementParent as CircularScale;
                SweepDirection st = new SweepDirection();
                SweepDirection et = new SweepDirection();
                double startangle;
                if (scale.IsReversed)
                {
                    st = SweepDirection.Counterclockwise;
                    et = SweepDirection.Clockwise;
                    startangle = scale.startAngle + scale.gapSweepAngle;
                }
                else
                {
                    st = SweepDirection.Clockwise;
                    et = SweepDirection.Counterclockwise;
                    startangle = scale.startAngle;
                }

                Point centerPoint = new Point(0, -0.5);
                bool isArcLarge = this.AnglePosition - scale.startAngle >= 180;
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
                
                if (this.AnglePosition - scale.startAngle == 360)
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
                else if (this.AnglePosition > scale.startAngle  || this.AnglePosition<scale.startAngle+scale.gapSweepAngle)
                {
                    if (!scale.IsReversed)
                    {
                        if (this.AnglePosition == scale.startAngle + scale.gapSweepAngle)
                        {
                            this.AnglePosition = scale.startAngle + scale.gapSweepAngle - 0.5;
                        }
                        else if (this.AnglePosition == scale.startAngle)
                        {
                            this.AnglePosition = scale.startAngle +0.5;
                        }
                    }
                    else
                    {
                        if (this.AnglePosition == scale.startAngle + scale.gapSweepAngle)
                        {
                            this.AnglePosition = scale.startAngle + scale.gapSweepAngle - 0.5;
                        }
                        else if (this.AnglePosition > scale.startAngle + scale.gapSweepAngle)
                        {
                            this.AnglePosition = scale.startAngle + scale.gapSweepAngle;
                        }

                        if (scale.startAngle + scale.gapSweepAngle - this.AnglePosition >= 180 && this.AnglePosition >= scale.startAngle)
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
                    prevpath = path;
                }
                else
                {
                    if (scale.IsReversed)
                    {
                        if (this.AnglePosition-scale.startAngle < (scale.startAngle + scale.gapSweepAngle) + 360 - this.AnglePosition)
                        {
                            this.AnglePosition = scale.startAngle + scale.gapSweepAngle;
                        }
                        else
                        {
                            this.AnglePosition = scale.startAngle;
                        }
                    }
                    else
                    {
                        if (scale.startAngle - this.AnglePosition < this.AnglePosition - (scale.startAngle + scale.gapSweepAngle) + 360)
                        {
                            this.AnglePosition = scale.startAngle + 0.5;
                        }
                        else
                        {
                            this.AnglePosition = scale.startAngle + scale.gapSweepAngle;
                        }
                    }                   
                }
            }

            return path;
        }

        /// <summary>
        /// Gets the geometry to draw the marker pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private Geometry GetMarkerPath()
        {
            Geometry path = null;

            if (this.MarkerStyle == MarkerStyle.Rectangle)
            {
                path = new RectangleGeometry();
                (path as RectangleGeometry).Rect = new Rect(0, 0, this.PointerLength, this.PointerWidth);
            }
            else if (this.MarkerStyle == MarkerStyle.Ellipse)
            {
                path = new EllipseGeometry();
                (path as EllipseGeometry).Center = new Point(this.PointerLength / 2, this.PointerWidth / 2);
                (path as EllipseGeometry).RadiusX = this.PointerLength / 2;
                (path as EllipseGeometry).RadiusY = this.PointerWidth / 2;
            }
            else if (this.MarkerStyle == MarkerStyle.Triangle)
            {
                path = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, 0);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                (path as PathGeometry).Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Diamond)
            {
                path = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, 0);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth / 2);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, this.PointerWidth);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                (path as PathGeometry).Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Pentagon)
            {
                path = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, 0);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth / 4);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, 3 * this.PointerWidth / 4);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, this.PointerWidth);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                (path as PathGeometry).Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Trapezoid)
            {
                path = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 4);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, 0);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth);
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = new Point(0, 3 * this.PointerWidth / 4);
                figure1.Segments.Add(line);

                (path as PathGeometry).Figures.Add(figure1);
            }

            return path;
        }

        /// <summary>
        /// Gets the geometry to draw the needle pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private Geometry GetNeedlePath()
        {
            Geometry path = null;
           
                if (this.NeedleStyle == NeedleStyle.Triangle)
                {
                    path = new PathGeometry();
                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(0, 0);
                    LineSegment line = new LineSegment();
                    line.Point = new Point(0, 0);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(0, this.PointerWidth);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength, this.PointerWidth / 2);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(0, 0);
                    figure1.Segments.Add(line);

                    (path as PathGeometry).Figures.Add(figure1);
                }
                else if (this.NeedleStyle == NeedleStyle.Rectangle)
                {
                    path = new RectangleGeometry();
                    (path as RectangleGeometry).Rect = new Rect(0, 0, this.PointerLength, this.PointerWidth);
                }
                else if (this.NeedleStyle == NeedleStyle.Trapezoid)
                {
                    path = new PathGeometry();

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(0, this.PointerWidth);
                    LineSegment line = new LineSegment();
                    line.Point = new Point(0, this.PointerWidth);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength, this.PointerWidth * 0.7);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength, this.PointerWidth * 0.3);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(0, 0);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(0, this.PointerWidth);
                    figure1.Segments.Add(line);

                    (path as PathGeometry).Figures.Add(figure1);
                }
                else if (this.NeedleStyle == NeedleStyle.Arrow)
                {
                    path = new PathGeometry();

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(0, this.PointerWidth * 0.37);
                    LineSegment line = new LineSegment();
                    line.Point = new Point(0, this.PointerWidth * 0.37);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(0, this.PointerWidth * 0.63);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength * 0.83, this.PointerWidth * 0.63);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength * 0.78, this.PointerWidth);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength, this.PointerWidth / 2);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength * 0.78, 0);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(this.PointerLength * 0.83, this.PointerWidth * 0.37);
                    figure1.Segments.Add(line);
                    line = new LineSegment();
                    line.Point = new Point(0, this.PointerWidth * 0.37);
                    figure1.Segments.Add(line);

                    (path as PathGeometry).Figures.Add(figure1);
                }
                else if (this.NeedleStyle == NeedleStyle.Needle)
                {
                    CircularScale scale = this.GaugeElementParent as CircularScale;
                    if (scale != null)
                    {
                        CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
                        if (gauge == null)
                        {
                            gauge = scale.getGaugeParent(scale);
                        }

                        double length = gauge == null ? 100 : gauge.Radius - (2 * gauge.InnerFrameOffset);
                        Point centerPoint = gauge == null ? new Point(55, 50) : new Point((length + gauge.InnerFrameOffset) / 2, length + (gauge.InnerFrameOffset / 2));
                        Point startPoint1 = this.ConvertToStageCoordinates(length, 90, centerPoint);
                        path = new PathGeometry();
                        PathFigure figure1 = new PathFigure();
                        figure1.StartPoint = new Point(0, 0);
                        LineSegment line = new LineSegment();
                        line.Point = new Point(-Math.Sqrt(2) * scale.PointerCap.PointerCapRadius, 0);
                        figure1.Segments.Add(line);
                        line = new LineSegment();
                        line.Point = new Point((-Math.Sqrt(2) * scale.PointerCap.PointerCapRadius), this.PointerWidth);
                        figure1.Segments.Add(line);
                        line = new LineSegment();
                        line.Point = new Point(0, this.PointerWidth);
                        figure1.Segments.Add(line);
                        line = new LineSegment();
                        line.Point = new Point(this.PointerLength, this.PointerWidth / 2);
                        figure1.Segments.Add(line);
                        line = new LineSegment();
                        line.Point = new Point(0, 0);
                        figure1.Segments.Add(line);

                        (path as PathGeometry).Figures.Add(figure1);
                    }
                }
            
            return path;
        }

        /// <summary>
        /// Converts polar coordinates to stage coordinates.
        /// </summary>
        /// <param name="radius">Radius of the border</param>
        /// <param name="angle">Angle of the border</param>
        /// <param name="pointToShift">Point to shift</param>
        /// <returns>Returns point</returns>
        internal Point ConvertToStageCoordinates(double radius, double angle, Point pointToShift)
        {
            Point point = new Point(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));
            point.X += pointToShift.X;
            point.Y += pointToShift.Y;
            return point;
        }

        /// <summary>
        /// Method invoked to get the shadow offset.
        /// </summary>
        /// <returns>Returns a point </returns>
        private Point GetShadowOffset()
        {
            Point p = new Point(0, 0);
            if (this.GaugeElementParent is CircularScale)
            {
                CircularScale scale = this.GaugeElementParent as CircularScale;
                double shadowOffsetY = 0;
                double shadowOffsetX = 0;
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

                if (this.PointerNeedleType == PointerNeedleType.Bar)
                {
                    shadowOffsetX = scale.ShadowOffset;
                    shadowOffsetY = scale.ShadowOffset;
                }

                p = new Point(shadowOffsetX, shadowOffsetY);
            }

            return p;
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void PositionAnimationCompleted(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, this.mvalueArgs);
            }
        }

        /// <summary>
        /// Method invoked to rotate the marker pointer
        /// </summary>
        private void RotateMarkerPointer()
        {
            if (this.mpointerPath != null && this.mpointerPathShadow != null)
            {
                if (this.PointerNeedleType == PointerNeedleType.Marker && this.PointerPlacement == ScalePlacement.Inside
                && (this.MarkerStyle == MarkerStyle.Triangle || this.MarkerStyle == MarkerStyle.Pentagon || this.MarkerStyle == MarkerStyle.Trapezoid))
                {
                    TransformGroup transformGroup = new TransformGroup();
                    RotateTransform transform1 = new RotateTransform();
                    transform1.Angle = 180;
                    transform1.CenterX = this.PointerLength / 2;
                    transform1.CenterY = this.PointerWidth / 2;
                    TranslateTransform transform2 = new TranslateTransform();
                    Point shadowOffset = this.GetShadowOffset();
                    transform2.X = shadowOffset.X;
                    transform2.Y = shadowOffset.Y;
                    transformGroup.Children.Add(transform1);
                    transformGroup.Children.Add(transform2);

                    this.mpointerPath.RenderTransform = transform1;
                    this.mpointerPathShadow.RenderTransform = transformGroup;
                    this.misRotated = true;
                }
                else if (this.misRotated)
                {
                    RotateTransform transform1 = new RotateTransform();
                    transform1.Angle = 0;
                    transform1.CenterX = this.DesiredSize.Width / 2;
                    transform1.CenterY = this.DesiredSize.Height / 2;
                    this.mpointerPath.RenderTransform = transform1;
                    this.misRotated = false;
                }
            }
        }

        #endregion
    }

}
