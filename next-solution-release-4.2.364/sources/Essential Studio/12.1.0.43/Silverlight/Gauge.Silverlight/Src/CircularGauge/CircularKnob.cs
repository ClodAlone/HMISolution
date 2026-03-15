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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents the CircularKnobvisual element.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge; 
    /// <para></para>
    /// <para>CircularKnob circularknob = new CircularKnob(); </para>
    /// <para>circularknob.InnerKnobBrush = new SolidColorBrush( Color.FromArgb( 255, 194, 207, 229 ) );</para>
    /// <para>circularknob.OuterKnobBrush = new SolidColorBrush( Color.FromArgb( 255, 194, 207, 229 ) );  </para>
    /// <para>   </para>
    /// <para>     circularpointer.KnobRadius = 10; </para>
    /// <para>     circularpointer.KnobType = KnobType.Line; </para>
    /// <para>        circularscale.Pointers.Add( circularknob ); </para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot; 
    /// <para></para>
    /// <para>&lt;syncfusion:CircularScale.Pointers&gt; </para>
    /// <para>   &lt;syncfusion:CircularKnob InnerKnobBrush=&quot;Blue&quot; OuterKnobBrush=&quot;Black&quot; KnobType=&quot;Line&quot; KnobRadius=&quot;10&quot;/&gt; </para>
    /// <para>    &lt;/syncfusion:CircularScale.Pointers&gt; </para></description></item></list>
    /// </example>
    public class CircularKnob : PointerBase
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.OuterKnobBrush">OuterKnobBrush</see> Dependency property
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.Brush">System.Windows.Media.Brush</see>
        /// </returns>
        public static readonly DependencyProperty OuterKnobBrushProperty =
            DependencyProperty.Register("OuterFrameBrush", typeof(Brush), typeof(CircularKnob), new PropertyMetadata(new PropertyChangedCallback(OnOuterKnobBrushChanged)));
        
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.EnableMouseMove">EnableMouseMove</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">System.Boolean</see>
        /// </returns>
        public static readonly DependencyProperty EnableMouseMoveProperty =
            DependencyProperty.Register("EnableMouseMove", typeof(bool), typeof(CircularKnob), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableMouseMoveChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.InnerKnobBrush">InnerKnobBrush</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.Brush">System.Windows.Media.Brush</see>
        /// </returns>
        public static readonly DependencyProperty InnerKnobBrushProperty =
            DependencyProperty.Register("InnerFrameBrush", typeof(Brush), typeof(CircularKnob), new PropertyMetadata(new PropertyChangedCallback(OnInnerKnobBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.PointerCap.KnobRadius">KnobRadius</see>  dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default value for the knob radius is 20d.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty KnobRadiusProperty =
            DependencyProperty.Register("KnobRadius", typeof(double), typeof(CircularKnob), new PropertyMetadata(20d, new PropertyChangedCallback(OnKnobRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.Knob.KnobType">KnobType</see>  dependency property.
        /// </summary>
        /// <remarks>
        /// <para>There are two types of Knob i.e <see cref="F:Syncfusion.Windows.Gauge.KnobType.Default">KnobType.Line</see> and <see cref="T:Syncfusion.Windows.Gauge.KnobType">KnobType.Point</see></para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.KnobType">KnobType</see></para>
        /// </returns>
        public static readonly DependencyProperty KnobTypeProperty =
            DependencyProperty.Register("KnobType", typeof(KnobType), typeof(CircularKnob), new PropertyMetadata(KnobType.Point, new PropertyChangedCallback(OnKnobTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.KnobValue">KnobValue</see>  dependency property.
        /// </summary>
        /// <remarks>
        /// KnobValue is property to set the knob value.The default value is 0d.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty KnobValueProperty =
            DependencyProperty.Register("KnobValue", typeof(double), typeof(CircularKnob), new PropertyMetadata(new PropertyChangedCallback(OnKnobValueChanged)));
        
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.AnglePosition">AnglePosition</see> Dependency property.
        /// </summary>
        internal static readonly DependencyProperty AnglePositionProperty =
             DependencyProperty.Register("AnglePosition", typeof(double), typeof(CircularKnob), new PropertyMetadata(0d, new PropertyChangedCallback(OnAnglePositionChanged)));

        #endregion

        #region Private members

        /// <summary>
        /// Temp variable for storing clicked event. 
        /// </summary>
        private bool mclick = false;

        /// <summary>
        /// Variable for storing whether the knob is loaded or not.
        /// </summary>
        private bool misLoaded = false;

        /// <summary>
        /// Variable storing the origin point of the ellipse
        /// </summary>
        private Point origin;

        /// <summary>
        /// Variable storing template path.
        /// </summary>
        private Path knobrotate;

        /// <summary>
        /// Variable for storing template grid.
        /// </summary>
        private Grid knob;

        /// <summary>
        /// Variable storing grid's rotation angle.
        /// </summary>
        private RotateTransform knobangle;

        /// <summary>
        /// Variable storing template ellipse.
        /// </summary>
        private Ellipse knobellipse;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.CircularKnob">CircularKnob</see> class.
        /// </summary>
        public CircularKnob()
        {
            DefaultStyleKey = typeof(CircularKnob);
            this.SizeChanged += new SizeChangedEventHandler(this.KnobSizeChanged);
            this.misLoaded = true;
        }

       #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.AnglePosition">AnglePosition</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback AnglePositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.EnableMouseMove">EnableMouseMove</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableMouseMoveChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.InnerKnobBrush">InnerKnobBrush</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback InnerKnobBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.KnobRadius">KnobRadius</see>  property is changed.
        /// </summary>
        public event PropertyChangedCallback KnobRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.KnobType">KnobType</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback KnobTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.KnobType">KnobType</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback KnobValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.OuterKnobBrush">OuterKnobBrush</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback OuterKnobBrushChanged;

        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets a value indicating whether to enable mousemove.
        /// </summary>
        /// <remarks>
        /// <para>Default value is true. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Boolean">System.Boolean</see>
        /// </value>
        public bool EnableMouseMove
        {
            get
            {
                return (bool)GetValue(EnableMouseMoveProperty);
            }

            set
            {
                SetValue(EnableMouseMoveProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the InnerKnobBrush for the circular knob.
        /// </summary>
        /// <remarks>
        /// <para>Default value is Black. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">System.Windows.Media.Brush</see>
        /// </value>
        public Brush InnerKnobBrush
        {
            get
            {
                return (Brush)GetValue(InnerKnobBrushProperty);
            }

            set
            {
                SetValue(InnerKnobBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius for the cap.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double KnobRadius
        {
            get
            {
                return (double)GetValue(KnobRadiusProperty);
            }

            set
            {
                SetValue(KnobRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets different knob types.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="KnobType"/>
        /// Default value is KnobType.Default.
        /// </value>
        /// <seealso cref="KnobType"/>
        public KnobType KnobType
        {
            get
            {
                return (KnobType)GetValue(KnobTypeProperty);
            }

            set
            {
                SetValue(KnobTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets knob value.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="KnobType"/>
        /// Default value is KnobValue.Default.
        /// </value>
        /// <seealso cref="KnobType"/>
        public double KnobValue
        {
            get
            {
                return (double)GetValue(KnobValueProperty);
            }

            set
            {
                SetValue(KnobValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the OuterKnobBrush for the circular knob.
        /// </summary>
        /// <remarks>
        /// <para>Default value is Black. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">System.Windows.Media.Brush</see>
        /// </value>
        public Brush OuterKnobBrush
        {
            get
            {
                return (Brush)GetValue(OuterKnobBrushProperty);
            }

            set
            {
                SetValue(OuterKnobBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the AnglePosition the circular knob.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
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
        #endregion

        #region Overrides
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.knob = this.GetTemplateChild("GridKnob") as Grid;
            this.knobangle = this.GetTemplateChild("KnobAngle") as RotateTransform;
            this.knobellipse = this.GetTemplateChild("KnobEllipse") as Ellipse;
            this.AnglePosition = this.GetAngleByValue(this.KnobValue);
            this.UpdateVisualStyle();
             this.RefreshKnobType(this.KnobType, this);
            if (this.EnableMouseMove == true)
            {
                this.EnableMouseEvents(this.knob, this.knobrotate);
            }

            if (this.GaugeElementParent is CircularScale)
            {
                CircularScale scale = this.GaugeElementParent as CircularScale;
                scale.RefreshScale();
            }

            this.RefreshAnglePosition();
            this.origin.X = this.KnobRadius;
            this.origin.Y = this.KnobRadius;
            this.RefreshKnob();
        }

        /// <summary>
        /// This method updates the visual style
        /// </summary>
        /// <remarks></remarks>
        protected override void UpdateVisualStyle()
        {
            if (this.GaugeElementParent != null)
                this.VisualStyle = ((this.GaugeElementParent as CircularScale).GaugeElementParent as GaugeBase).VisualStyle;
            base.UpdateVisualStyle();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Method to diable the MouseMove events
        /// </summary>
        /// <param name="knobgrid">grid to which mouse events are added</param>
        /// <param name="knobpath">path to which mouse events are added</param>
        internal void DisableMouseEvents(Grid knobgrid, Path knobpath)
        {
          //  this.mrotate = false;
            knobgrid.Cursor = Cursors.Arrow;
            knobgrid.MouseMove -= new MouseEventHandler(this.Knobrotate_MouseMove);
            knobgrid.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Knobrotate_MouseLeftButtonDown);
            knobgrid.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Knobrotate_MouseLeftButtonUp);
        }

        /// <summary>
        /// Method to enable Mousemove events.
        /// </summary>
        /// <param name="knobgrid">grid to which mouse events are removed</param>
        /// <param name="knobpath">path to which mouse events are removed</param>
        internal void EnableMouseEvents(Grid knobgrid, Path knobpath)
        {
         //   this.mrotate = true;
            knobgrid.Cursor = Cursors.Hand;
            knobgrid.MouseMove += new MouseEventHandler(this.Knobrotate_MouseMove);
            knobgrid.MouseLeftButtonDown += new MouseButtonEventHandler(this.Knobrotate_MouseLeftButtonDown);
            knobgrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.Knobrotate_MouseLeftButtonUp);
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
                    if (angle < scale.StartAngle && angle > (scale.StartAngle + scale.GapSweepAngle - 360))
                    {
                        if (angle < scale.StartAngle && this.AnglePosition > scale.StartAngle + scale.GapSweepAngle)
                        {
                            angle = scale.StartAngle;
                            value = scale.Minimum;
                            this.KnobValue = scale.Minimum;
                        }
                        else if (angle < scale.StartAngle && this.AnglePosition <= (scale.StartAngle + scale.GapSweepAngle - 360))
                        {
                            angle = scale.StartAngle + scale.GapSweepAngle - 360 + 10;
                            value = scale.Maximum;
                            this.KnobValue = scale.Maximum;
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
                            this.KnobValue = scale.Minimum;
                        }
                        else if (angle < scale.StartAngle && this.AnglePosition <= (scale.StartAngle + scale.GapSweepAngle - 360))
                        {
                            angle = scale.StartAngle + scale.GapSweepAngle - 360 + 10;
                            value = scale.Maximum;
                            this.KnobValue = scale.Maximum;
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
                    if (ang < scale.StartAngle && ang > (scale.StartAngle + scale.GapSweepAngle - 360))
                    {
                        if ((scale.StartAngle - ang) < (ang - (scale.StartAngle + scale.GapSweepAngle - 360)))
                        {
                            ang = scale.StartAngle;
                        }
                        else
                        {
                            ang = scale.StartAngle + scale.GapSweepAngle - 360;
                        }
                    }
                }
                else
                {
                    ang = 0;
                }
            }

            if (ang == double.NaN)
            {
                return 0;
            }
            else
            {
                return ang;
            }
        }

        /// <summary>
        /// Method to validate the Value of the CircularKnob
        /// </summary>
        /// <param name="val">Value for validation</param>
        /// <returns>
        /// Coreced Value
        /// </returns>
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
        /// Method to convert angle to value in the circularknob
        /// </summary>
        /// <param name="value">Value to which the corresponding angle is needed</param>
        /// <returns>
        /// Corresponding angle to the value
        /// </returns>
        protected internal virtual double GetAngleByValue(double value)
        {
            double angle = 0;
            CircularScale scale = this.GaugeElementParent as CircularScale;
            if (scale != null && this.knob != null)
            {
                angle = scale.StartAngle;
                if (scale.Minimum != value && scale.Minimum != scale.Maximum)
                {
                    double ratio = (scale.Maximum - scale.Minimum) / (value - scale.Minimum);
                    angle = (scale.GapSweepAngle / ratio) + scale.StartAngle;
                }
            }

            return angle;
        }
      
        /// <summary>
        /// Method to refresh the AnglePosition.
        /// </summary>
        protected internal virtual void RefreshAnglePosition()
        {
            if (this.GaugeElementParent is CircularScale)
            {
                if (this.knobrotate != null)
                {
                    CircularScale scale = this.GaugeElementParent as CircularScale;
                    this.AnglePosition = this.CoerceAngle(this.AnglePosition);
                    if (this.AnglePosition != double.NaN)
                    {
                        this.knobrotate.RenderTransformOrigin = new Point(0.5, 0.5);
                        RotateTransform transform = new RotateTransform();
                        if (this.AnglePosition != double.NaN)
                        {
                            try
                            {
                                transform.Angle = (this.AnglePosition != double.NaN) ? this.AnglePosition : 0;
                                transform.CenterX = 0;
                                transform.CenterY = 0;
                                this.knobrotate.RenderTransform = transform;
                            }
                            catch (Exception)
                            {
                                transform.Angle = 0.0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size
        /// for the cap element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child
        /// elements.</param>
        /// <returns>
        /// The size that this element determines it needs during layout.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            return new Size(this.KnobRadius, this.KnobRadius);
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnAnglePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.AnglePositionChanged != null)
            {
                this.AnglePositionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnEnableMouseMoveChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnableMouseMoveChanged != null)
            {
                this.EnableMouseMoveChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnInnerKnobBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.InnerKnobBrushChanged != null)
            {
                this.InnerKnobBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnKnobRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.RefreshKnob();
            
            if (this.KnobRadiusChanged != null)
            {
                this.KnobRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="KnobTypeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnKnobTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshAnglePosition();
            if (this.KnobTypeChanged != null)
            {
                this.KnobTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnKnobValueChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.RefreshKnob();
            if (this.KnobValueChanged != null)
            {
                this.KnobValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnOuterKnobBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.OuterKnobBrushChanged != null)
            {
                this.OuterKnobBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEnableMouseMoveChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableMouseMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnEnableMouseMoveChanged(e);
            if (instance.knob != null)
            {
                if (instance.EnableMouseMove == true)
                {
                    instance.EnableMouseEvents(instance.knob, instance.knobrotate);
                }
                else
                {
                    instance.DisableMouseEvents(instance.knob, instance.knobrotate);
                }
            }
        }

        /// <summary>
        /// Calls OnOuterKnobBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOuterKnobBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnOuterKnobBrushChanged(e);
            if (instance.knob != null)
            {
                instance.knobellipse.Fill = (Brush)e.NewValue;
            }
        }

        /// <summary>
        /// Calls OnAnglePositionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAnglePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnAnglePositionChanged(e);
        }

        /// <summary>
        /// Calls OnInnerKnobBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerKnobBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnInnerKnobBrushChanged(e);
            if (instance.knob != null)
            {
                instance.knobrotate.Fill = (Brush)e.NewValue;
            }
        }

        /// <summary>
        /// Calls OnKnobRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKnobRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnKnobRadiusChanged(e);
        }

        /// <summary>
        /// Calls OnKnobTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKnobTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnKnobTypeChanged(e);
            if (instance.knob != null)
            {
                instance.KnobType = (KnobType)e.NewValue;
                instance.RefreshKnobType((KnobType)e.NewValue, instance);
                if (instance.EnableMouseMove == true && instance.knob != null)
                {
                    instance.EnableMouseEvents(instance.knob, instance.knobrotate);
                }
                else if (instance.EnableMouseMove == false && instance.knob != null)
                {
                    instance.DisableMouseEvents(instance.knob, instance.knobrotate);
                }

                instance.RefreshAnglePosition();
            }
        }

        /// <summary>
        /// Calls OnKnobValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKnobValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularKnob instance = (CircularKnob)d;
            instance.OnKnobValueChanged(e);
            double oldvalue = (double)e.OldValue;
            double newvalue = (double)e.NewValue;
            double coercedvalue = instance.CoerceValue(newvalue);
            instance.KnobValue = (double)e.NewValue;
            instance.AnglePosition = instance.GetAngleByValue(instance.KnobValue);
            instance.RefreshAnglePosition();
        }
       
        /// <summary>
        /// Method raised when MouseLeftButton is down on the Knobrotate.
        /// </summary>
        /// <param name="sender">Object on which the MouseLeftButton is down</param>
        /// <param name="e">Contains the Property details</param>
        private void Knobrotate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(Grid))
            {
                Grid el = sender as Grid;
                el.CaptureMouse();
                if (this.GaugeElementParent is ScaleBase)
                {
                    CircularScale scale = this.GaugeElementParent as CircularScale;

                    Point p = e.GetPosition(this.knob);
                    double num, deno;
                    num = p.Y - this.KnobRadius;
                    deno = p.X - this.KnobRadius;
                    double m = num / deno;
                    double tan = (180 * Math.Atan(m)) / Math.PI;
                    if (deno < 0)
                    {
                        tan = 180 + tan;
                    }

                    double value = this.AngleToValue(tan, this.KnobValue);

                    if (value != this.KnobValue)
                    {
                        this.AnglePosition = this.GetAngleByValue(value);
                        this.KnobValue = value;
                    }
                    else if( value<scale.Minimum && value>scale.Maximum)
                    {
                        if ((this.KnobValue - scale.Minimum) > (scale.Maximum - this.KnobValue))
                        {
                            this.KnobValue = scale.Maximum;
                        }
                        else
                        {
                            this.KnobValue = scale.Minimum;
                        }
                    }

                    if (this.AnglePosition > 360)
                    {
                        this.AnglePosition -= 360;
                    }

                    this.AnglePosition = tan;
                    if (this.AnglePosition >= 360)
                    {
                        this.AnglePosition -= 360;
                    }

                    this.RefreshAnglePosition();
                    this.mclick = true;
                }
            }
        }

        /// <summary>
        /// Method raised when MouseLeftButton is up on the Knobrotate.
        /// </summary>
        /// <param name="sender">Object on which the MouseLeftButton is up</param>
        /// <param name="e">Contains the Property details</param>
        private void Knobrotate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(Grid)) 
            {
                Grid el = sender as Grid;
                el.ReleaseMouseCapture();
                this.mclick = false;
            }
        }

        /// <summary>
        /// Method raised when MouseMoves on the Knobrotate.
        /// </summary>
        /// <param name="sender">Object on which the Mouse is moved</param>
        /// <param name="e">Contains the Property details</param>
        private void Knobrotate_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.EnableMouseMove == true)
            {
                if (this.mclick == true)
                {
                    if (this.GaugeElementParent is ScaleBase)
                    {
                        CircularScale scale = this.GaugeElementParent as CircularScale;
                        Point p = e.GetPosition(this.knob);
                        double num, deno;
                        num = p.Y - this.KnobRadius;
                        deno = p.X - this.KnobRadius;
                        double m = num / deno;
                        double tan = (180 * Math.Atan(m)) / Math.PI;
                        if (deno < 0)
                        {
                            tan = 180 + tan;
                        }

                        double value = this.AngleToValue(tan, this.KnobValue);

                        if (value != this.KnobValue)
                        {
                            this.AnglePosition = this.GetAngleByValue(value);
                            this.KnobValue = value;
                        }
                        else if (value < scale.Minimum && value > scale.Maximum)
                        {
                            if ((this.KnobValue - scale.Minimum) > (scale.Maximum - this.KnobValue))
                            {
                                this.KnobValue = scale.Maximum;
                            }
                            else
                            {
                                this.KnobValue = scale.Minimum;
                            }
                        }

                        if (this.AnglePosition > 360)
                        {
                            this.AnglePosition -= 360;
                        }

                        this.AnglePosition = tan;
                        if (this.AnglePosition >= 360)
                        {
                            this.AnglePosition -= 360;
                        }

                        this.RefreshAnglePosition();
                    }
                }
            }
        }
       
        /// <summary>
        /// Invoked when the Knob size is changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void KnobSizeChanged(object sender, RoutedEventArgs e)
        {
            if (!double.IsNaN(this.Width) && this.Width > 0)
            {
                this.KnobRadius = this.Width / 2;
            }
        }

        /// <summary>
        /// Refreshs the Pointer cap
        /// </summary>
        private void RefreshKnob()
        {
            if (this.knob != null)
            {
                this.knob.Width = this.KnobRadius * 2;
                this.knob.Height = this.KnobRadius * 2;
                CircularScale scale = this.GaugeElementParent as CircularScale;
                if (scale != null)
                {
                    TranslateTransform transform = new TranslateTransform();
                    if (scale.ShadowOffset > 1)
                    {
                        transform.X = scale.ShadowOffset - 1;
                        transform.Y = scale.ShadowOffset - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Method for refreshing the KnobType.
        /// </summary>
        /// <param name="knobtype">Parameter contains the type of the knob</param>
        /// <param name="main">Parameter contains the circular knob object</param>
        private void RefreshKnobType(KnobType knobtype, CircularKnob main)
        {
            for (int i = 0; i < main.knob.Children.Count; i++)
            {
                if (main.knob.Children[i].ToString().Equals("System.Windows.Shapes.Path"))
                {
                    main.knob.Children.RemoveAt(i);
                }
            }

            Path path;
            if (knobtype == KnobType.Line)
            {
                path = (Path)XamlReader.Load("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"  Margin=\"9,9,9,9\" Name=\"Rotate\" Stretch=\"Fill\" Stroke=\"#FF000000\" Data=\"M44.5,22.5 C44.5,34.650265 34.650265,44.5 22.5,44.5 C10.349735,44.5 0.5,34.650265 0.5,22.5 C0.5,10.349735 10.349735,0.5 22.5,0.5 C34.650265,0.5 44.5,10.349735 44.5,22.5 z M23.5,21.5 L43.5,21.5 L43.5,24.5 L23.5,24.5 z\"/>");
            }
            else if (knobtype == KnobType.Cog)
            {
                path = (Path)XamlReader.Load("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"  Margin=\"8.5,2,4,10\" Name=\"Rotate\" Stretch=\"Fill\" Data=\"M74.59993,101.93526 L68.509262,103.17271 L64.315903,110.66879 C63.520008,110.70492 62.716297,110.72738 61.906727,110.72738 L59.642078,110.67367 L59.642086,110.67368 L59.915524,110.68735 L60.198727,110.70102 L60.48193,110.70493 L60.773922,110.71371 L61.048336,110.7186 L61.340328,110.72739 L62.509274,110.72739 L62.818844,110.7186 L63.120602,110.71371 L63.713375,110.69614 L64.019043,110.68735 L64.315918,110.66879 L68.509277,103.17271 L70.063965,102.93834 L71.605957,102.65123 L73.138184,102.30943 z M43.76395,100.09102 L45.142578,100.63268 L46.585449,101.14049 L48.068359,101.60826 L49.556152,102.03111 L51.066406,102.40807 L52.599121,102.73619 L54.149414,103.0106 L55.71825,103.23521 L55.71825,103.23521 C53.614735,102.96568 51.560658,102.56113 49.557789,102.03074 z M43.704578,100.07114 L36.333485,104.46664 L32.284035,102.12994 L32.283691,102.13853 L32.782715,102.4442 L33.272461,102.74986 L33.771484,103.04674 L34.288574,103.34361 L34.787109,103.63072 L35.308594,103.91393 L35.820801,104.19712 L36.333496,104.46665 L43.70459,100.07115 L37.95166,111.52328 L43.712112,100.07367 z M80.486801,99.9188 L74.699936,101.90813 L76.141113,101.49596 L77.605957,101.01549 L79.058105,100.494 L80.486816,99.918808 L80.493362,99.922935 L86.158691,103.49596 L86.159363,103.49471 L80.493362,99.922935 L80.486816,99.918808 z M96.168808,89.342216 L91.525864,93.459824 L91.52594,93.464668 L92.734863,92.494019 L93.907715,91.477417 L95.062988,90.425659 z M23.807156,84.978439 L24.782715,86.192268 L25.802734,87.374886 L26.841309,88.511597 L27.928711,89.625854 L29.052246,90.704956 L30.20752,91.743042 L31.389648,92.744995 L32.61179,93.706879 L27.933155,89.62426 z M23.807117,84.978394 L15.213863,85.10437 L15.213867,85.104378 L23.807125,84.978401 z M100.18248,84.745583 L96.17128,89.339798 L96.172363,89.338745 L97.242676,88.223511 L98.284668,87.069221 L99.291504,85.882698 z M13.475523,82.10743 L13.456543,82.147354 L13.703613,82.605362 L14.000489,83.109261 L14.296876,83.61219 L14.598144,84.116096 L14.89502,84.61512 L15.213806,85.104279 z M14.101803,61.976017 L14.337402,63.522377 L14.625001,65.070717 L14.962403,66.60099 L15.348632,68.104401 L15.780273,69.596588 L16.256836,71.064362 L16.787109,72.520409 L17.356945,73.937927 L17.357899,73.936417 C16.56908,72.053116 15.897083,70.105736 15.350942,68.106163 z M13.873849,61.837002 L14.099113,61.962807 L14.099111,61.962803 z M6.5439448,55.363171 L6.5439448,55.668385 L6.5664063,56.573658 L6.5664063,56.874928 L6.5800772,57.171803 L6.5844731,57.477463 L6.60252,57.77599 z M6.5932407,53.093166 L6.5800772,53.378349 L6.5708003,53.659111 L6.5664063,53.944756 L6.5576162,54.223076 L6.5483389,54.515556 L6.5439448,54.798756 L6.5439448,55.362202 z M107.02067,50.920795 L107.0805,51.35342 L108.73489,52.277248 L114.34804,55.412006 C114.34804,55.425678 114.34804,55.45937 114.34804,55.474995 L108.86018,58.362682 L107.10791,59.286507 L107.00349,60.131214 L59.86697,60.91787 L59.86697,51.08456 z M17.200592,37.162296 L16.638672,38.591751 L16.130859,40.049755 L15.663573,41.524361 L15.236328,43.014107 L14.863281,44.531193 L14.544434,46.056583 L14.26123,47.607361 L14.036132,49.169373 L14.033443,49.175171 L14.036121,49.173759 C14.305897,47.075859 14.710438,45.0191 15.240834,43.013733 z M32.205624,11.300474 L32.292931,17.256344 L32.292957,17.256323 z M62.536484,4.0000196 L59.396095,9.6001043 L58.468075,11.251954 L56.580708,11.512696 C52.814297,12.013671 49.088017,13.038572 45.506618,14.512688 L43.68631,15.276846 L42.014797,14.222652 L36.775101,10.928226 C36.761395,10.943851 36.725666,10.961917 36.707554,10.961917 L36.806427,17.366686 L36.842648,19.283674 L35.328739,20.449686 C32.319035,22.786596 29.606926,25.510712 27.277573,28.561972 L26.074963,30.119585 L24.09264,30.056597 L17.879894,29.809526 C17.879894,29.843218 17.86619,29.858843 17.843674,29.876909 L21.150482,35.375919 L22.128428,37.011658 L21.398149,38.780209 C19.947386,42.301685 18.969925,46.016525 18.478996,49.848053 L18.240139,51.807522 L16.464859,52.735741 L11.00001,55.589741 C11.00001,55.619038 11.00001,55.652729 11.00001,55.68642 L16.613655,58.816784 L18.271463,59.74012 L18.532837,61.623417 C19.06929,65.450554 20.073664,69.149765 21.538136,72.639015 L22.30855,74.472992 L21.244947,76.144867 L17.947439,81.38118 C17.965061,81.398758 17.978765,81.430984 17.99247,81.448563 L24.417154,81.34993 L26.336336,81.31868 L27.50762,82.823563 C29.814459,85.794273 32.544689,88.505203 35.635143,90.873344 L37.216595,92.07354 L37.135345,94.037399 L36.887676,100.21707 C36.901382,100.23563 36.923897,100.2493 36.955223,100.26688 L42.469994,96.971962 L44.096478,95.99736 L45.866863,96.729774 C49.394417,98.172157 53.120201,99.147743 56.954655,99.633095 L58.923275,99.876251 L59.851295,101.6282 L62.744019,107.09401 C62.761639,107.09401 62.797859,107.09401 62.811562,107.09401 L65.94706,101.49832 L66.879974,99.840118 L68.75853,99.588173 C72.588089,99.04911 76.327576,98.042282 79.841919,96.581345 L81.661736,95.821579 L83.33374,96.873329 L88.572945,100.16824 C88.586655,100.15457 88.600357,100.13602 88.631683,100.11844 L88.536728,93.709274 L88.505402,91.795219 L90.018822,90.631157 C93.029007,88.306953 95.745522,85.583336 98.088097,82.536453 L99.273575,80.972 L101.25981,81.044266 L107.45053,81.268875 C107.46815,81.255203 107.48186,81.223953 107.48186,81.201492 L104.19757,75.722992 L103.21962,74.082375 L103.95381,72.315773 C105.40067,68.792343 106.37763,65.077507 106.86514,61.25037 L107.00349,60.131214 L107.67316,60.120037 L115.00001,55.808468 L107.85543,50.917896 L107.02067,50.920795 L106.82011,49.470612 C106.27877,45.659107 105.27438,41.928638 103.80991,38.423275 L103.0444,36.607361 L104.10261,34.935493 L107.3869,29.712849 C107.3869,29.699177 107.36929,29.665485 107.35558,29.64986 L100.93089,29.74654 L99.012199,29.775837 L97.840431,28.269979 C95.497856,25.249962 92.749039,22.544405 89.708504,20.222635 L88.131454,19.004866 L88.212708,17.042955 L88.451561,10.878911 C88.437859,10.865237 88.424149,10.845218 88.39283,10.831546 L82.864838,14.125972 L81.220245,15.096671 L79.44545,14.366693 C75.953629,12.923827 72.227852,11.950684 68.407097,11.462891 L66.438477,11.218263 L65.514374,9.4697313 L62.617737,4.0000196 C62.581516,4.0000186 62.56781,4.0000186 62.536484,4.0000196 z M61.906727,0 C62.671375,-7.1054274E-15 63.426258,0.017577982 64.18602,0.051756941 L68.104965,7.4965725 C72.298325,8.0312376 76.325668,9.1122904 80.104965,10.66063 L87.484848,6.2607327 C88.869614,6.9819236 90.222153,7.7597547 91.534653,8.5956917 L91.206528,17.013647 C94.505356,19.535128 97.46727,22.469694 100.01512,25.746546 L108.61375,25.623011 C109.44481,26.937462 110.21825,28.285604 110.95067,29.665485 L106.46922,36.782661 L106.46924,36.782677 L106.46924,36.782669 L110.95068,29.665495 L106.7643,37.64246 L108.47289,42.621094 C109.01781,44.621395 109.43797,46.672539 109.72801,48.76458 L117.21629,52.955978 C117.24754,53.746994 117.27,54.560471 117.27,55.362713 C117.27,56.128826 117.24754,56.881756 117.21629,57.634682 L109.78172,61.558018 L109.78172,61.558052 L109.78174,61.558025 L117.21631,57.636642 L109.5708,71.810455 L106.69936,73.323013 L106.61766,73.567276 L111.01805,80.933479 C110.28563,82.318245 109.5034,83.675659 108.67625,84.983276 L105.66231,84.870811 L108.67627,84.988167 L108.98193,84.49305 L109.29639,83.998909 L109.58838,83.495979 L109.88525,82.983284 L110.17334,82.480362 L110.45654,81.967667 L102.87451,95.900261 L102.59521,96.408073 L102.31201,96.907097 L101.71924,97.905144 L101.42236,98.404167 L101.1167,98.884628 L92.77002,98.561394 L92.77002,98.5653 L91.817871,99.774277 L91.618134,100.01029 L91.646957,102.05747 L87.602036,104.40414 L87.60199,104.40411 L87.602036,104.40414 L87.602051,104.40415 L88.631348,103.85143 L89.143066,103.56822 L90.154785,102.97447 L90.657715,102.6776 L91.152832,102.37291 L91.646973,102.06236 L84.253418,115.78402 L83.763184,116.09847 L82.778809,116.70492 L82.270996,116.98812 L81.776855,117.27621 L81.269043,117.56332 L80.756348,117.84261 L80.248535,118.11214 L73.196777,113.67563 L71.790527,114.23325 L70.356934,114.75472 L68.904785,115.2264 L67.421387,115.64925 L65.925293,116.03597 L64.414551,116.37289 L62.88232,116.66 L61.340328,116.89437 L57.191891,124.31429 L56.895992,124.32796 L56.603024,124.34163 L56.302242,124.35042 L56.01025,124.3553 L55.704586,124.36409 L55.407715,124.368 L54.243652,124.368 L53.677734,124.35921 L53.398926,124.35042 L53.120117,124.34163 L52.836914,124.33284 L52.562988,124.31917 L48.684082,116.95296 L47.128906,116.72738 L45.59668,116.45785 L44.082031,116.13851 L42.589844,115.76546 L41.106445,115.34359 L39.645996,114.8807 L38.203125,114.37289 L36.796387,113.82407 L29.492676,118.17464 L29.492676,118.17953 L28.980469,117.90511 L28.472656,117.62679 L27.960449,117.35238 L26.962402,116.7723 L26.463379,116.4764 L25.973633,116.17074 L25.479004,115.86507 L25.802734,107.52329 L32.611008,93.727333 L32.611805,93.706924 L25.802734,107.51841 L24.59375,106.56626 L23.420898,105.57212 L22.279297,104.54771 L21.168945,103.48228 L20.099609,102.3856 L19.061035,101.24889 L18.059082,100.07994 L17.097168,98.884628 L23.807129,84.978401 L17.097168,98.880722 L8.5800781,99.001816 L8.2700205,98.516472 L7.9643569,98.026237 L7.6674809,97.536003 L7.3754883,97.028191 L7.0922852,96.529167 L6.8090806,96.021355 L6.5351567,95.518425 L6.2607422,94.996941 L10.198242,86.696175 L10.139648,86.542854 L9.6225595,85.104378 L9.1420898,83.657112 L8.7148428,82.178604 L8.3286133,80.68251 L8.0004883,79.171768 L7.7124033,77.634659 L7.4746099,76.097557 L11.888184,66.670807 L7.4746099,76.088768 L0.058593493,71.945221 L0.044922233,71.648346 L0.036132377,71.351471 L0.02685512,71.054596 L0.013673113,70.758698 L0.0043940027,70.461823 L0,70.156158 L0,68.996979 L0.0043940027,68.715729 L0.013673113,68.427643 L0.022462972,68.151276 L0.036132377,67.872955 L0.036132377,67.598541 L0.058593493,67.311432 L6.5932503,53.092731 L6.5932503,53.092697 L10.309182,51.136127 L6.1977549,44.241154 L12.800875,29.797369 L12.795887,29.789021 C13.523914,28.402304 14.296864,27.05172 15.142079,25.741663 L23.560047,26.065392 L25.376064,23.981707 L25.366699,23.326149 L32.166981,8.6650581 L32.166981,8.6650267 C33.483875,7.8271375 34.83202,7.0493064 36.207508,6.3212795 L43.331532,10.809067 C47.106434,9.2338734 51.1245,8.1210814 55.309071,7.5527234 L59.50243,0.056152795 C60.298328,0.017577982 61.102039,-7.1054274E-15 61.906727,0 z\"/>");
            }
            else
            {
                path = (Path)XamlReader.Load("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"  Margin=\"9,9,9,9\" Name=\"Rotate\" Stretch=\"Fill\" Stroke=\"#FF000000\" Data=\"M38.5,20 C38.5,30.769552 29.99341,39.5 19.5,39.5 C9.0065899,39.5 0.5,30.769552 0.5,20 C0.5,9.2304478 9.0065899,0.5 19.5,0.5 C29.99341,0.5 38.5,9.2304478 38.5,20 z M33.5,20 C33.5,22.485281 31.485281,24.5 29,24.5 C26.514719,24.5 24.5,22.485281 24.5,20 C24.5,17.514719 26.514719,15.5 29,15.5 C31.485281,15.5 33.5,17.514719 33.5,20 z\"/>");
            }

            path.Fill = main.InnerKnobBrush;
            main.knob.Children.Add(path);
            main.knobrotate = path;
            main.knobellipse.Fill = main.OuterKnobBrush;
        }
   
      #endregion
    }
}
