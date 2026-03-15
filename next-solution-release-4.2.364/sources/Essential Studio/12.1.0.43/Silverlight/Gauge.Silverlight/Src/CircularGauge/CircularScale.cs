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
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Markup;
    using System.Reflection;
    using System.Windows.Data;
    using System.ComponentModel;
    //using Syncfusion.Windows.Shared;

    /// <summary>
    /// Represents scale visual element.
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
    /// <para>CircularGauge circulargauge = new CircularGauge();</para>
    /// <para>      circulargauge.FrameType = GaugeFrameType.HalfCircle;</para>
    /// <para>      LayoutRoot.Children.Add(circulargauge);</para>
    /// <para>            CircularScale circularscale = new CircularScale();</para>
    /// <para>            circularscale.Radius = 100;</para>
    /// <para>            circularscale.StartAngle = 180;</para>
    /// <para>            circularscale.GapSweepAngle = 180;</para>
    /// <para>            circularscale.ScaleBarSize = 10;</para>
    /// <para>            circularscale.Location = new Point(50, 50);</para>
    /// <para>            circularscale.ShadowOffset = 5;</para>
    /// <para>            circularscale.ShadowOffset = 5;</para>
    /// <para>            circularscale.Minimum = 0;</para>
    /// <para>            circularscale.Maximum = 100;</para>
    /// <para>            circularscale.MinorIntervalValue = 2;</para>
    /// <para>            circularscale.MajorIntervalValue = 10;</para>
    /// <para>            circularscale.Background = new SolidColorBrush(Colors.LightGray);</para>
    /// <para>            circularscale.BorderBrush = new SolidColorBrush(Colors.Blue);</para>
    /// <para>            circularscale.BorderThickness = new Thickness(0.5);</para>
    /// <para>            circulargauge.Scales.Add(circularscale); </para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot; 
    /// <para>    </para>
    /// <para>&lt;syncfusion:CircularGauge   Height=&quot;Auto&quot; Width=&quot;Auto&quot;  Radius=&quot;170&quot;  Name=&quot;circularGauge1&quot; FrameType=&quot;HalfCircle&quot; Margin=&quot;3&quot;   &gt;</para>
    /// <para>&lt;syncfusion:CircularGauge.Scales&gt;</para>
    /// <para> &lt;syncfusion:CircularScale x:Name=&quot;m_scale&quot;   Radius=&quot;100&quot; startAngle=&quot;180&quot; gapSweepAngle=&quot;180&quot; ScaleBarSize=&quot;5&quot;  ShadowOffset=&quot;5&quot; Minimum=&quot;0&quot; Maximum=&quot;100&quot; MinorIntervalValue=&quot;2&quot; MajorIntervalValue=&quot;10&quot; BorderBrush=&quot;Gray&quot;   Background=&quot;Silver&quot; Location=&quot;50,70&quot; &gt;</para>
    /// <para> &lt;/syncfusion:CircularScale&gt;</para>
    /// <para>&lt;/syncfusion:CircularGauge.Scales&gt;</para>
    /// <para>&lt;/syncfusion:CircularGauge&gt; </para></description></item></list>
    /// </example>
    [ContentProperty("Pointers")]     
    public class CircularScale : ScaleBase
    {   
        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularScale.gapSweepAngle">gapSweepAngle</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The gapSweepAngle can be between 0 to 359.The default value for gapSweepAngle is 0d. </para>
        /// </remarks>
        /// <returns>
        /// <para>Type:<see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty GapSweepAngleProperty =
            DependencyProperty.Register("gapSweepAngle", typeof(double), typeof(CircularScale), new PropertyMetadata(0d, new PropertyChangedCallback(OnGapSweepAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularScale.PointerCap">PointerCap</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.PointerCap">PointerCap</see></para>
        /// </returns>
        public static readonly DependencyProperty PointerCapProperty =
            DependencyProperty.Register("PointerCap", typeof(PointerCap), typeof(CircularScale), new PropertyMetadata(new PropertyChangedCallback(OnPointerCapChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularScale.Radius">Radius</see> dependency property.
        /// </summary>
        /// <remarks>
        /// The Default value for Scale radius is 120d.
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CircularScale), new PropertyMetadata(120d, new PropertyChangedCallback(OnRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularScale.startAngle">startAngle</see>  dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The Start angle for the scale can be set the value from 0 to 359 .The default value for start angle is 0d.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("startAngle", typeof(double), typeof(CircularScale), new PropertyMetadata(0d, new PropertyChangedCallback(OnStartAngleChanged)));

        #endregion

        #region Private members
        /// <summary>
        /// variable for counting levels
        /// </summary>
        private int mnestLevel = 0;

        /// <summary>
        /// The geometry used to draw the scale.
        /// </summary>
        private Geometry mpathData;

       
        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double mradiusRatio,startangle,gapsweepangle;

        /// <summary>
        /// The path used to draw the scale.
        /// </summary>
        private Path mscalePath;

        private double rotateangle;

        private bool setMajorInterval = true, setMinorInterval = true;

        /// <summary>
        /// Gets or sets ScalePanel.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ScalesLayoutPanel ScalePanel
        {
            get
            {
                return (ScalesLayoutPanel)GetValue(ScalePanelProperty);
            }

            set
            {
                SetValue(ScalePanelProperty, value);
            }
        }

        
        #endregion
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.CircularScale">CircularScale</see> class.
        /// </summary>
        public CircularScale()
        {
            DefaultStyleKey = typeof(CircularScale);

            this.Loaded += new RoutedEventHandler(this.CircularScaleLoaded);
           // this.Style = null;
            this.Ticks = new TicksCollection();
            this.Ranges = new RangesCollection();
            this.Pointers = new PointersCollection();
            this.Ranges.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.Pointers.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.PointerCap = new PointerCap();
           // if (scaleTemplate != null)
               // this.Template = scaleTemplate;
            //indicator = false;
        }

        internal CircularGauge getGaugeParent(DependencyObject element)
        {
            while (element != null && !(element is CircularGauge))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element as CircularGauge;
        }


        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can
        /// override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to
        /// child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be
        /// specified as a value to indicate that the object will size to whatever content
        /// is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its
        /// calculations of the allocated sizes for child objects; or based on other
        /// considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in Ticks)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            foreach (FrameworkElement element in Pointers)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            foreach (FrameworkElement element in Ranges)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can
        /// override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should
        /// use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        static CircularScale()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
                //MenuAdv menu = new MenuAdv();
                //menu = null;

            }
        }
        #endregion
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularScale.gapSweepAngle">gapSweepAngle</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback GapSweepAngleChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularScale.PointerCap">PointerCap</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularScale.Radius">Radius</see> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> RadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularScale.startAngle">startAngle</see>  property is changed.
        /// </summary>
        public event PropertyChangedCallback StartAngleChanged;
        #endregion
        #region DP getters & setters
        /// <summary>
        /// Gets or sets the number of degrees that the scale will sweep in a circle. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Value can be between 0 to 359.Default value is 0. </para>
        /// </remarks>
        /// <value>
        ///  Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double GapSweepAngle
        {
            get
            {
                return (double)GetValue(GapSweepAngleProperty);
            }

            set
            {
                SetValue(GapSweepAngleProperty, value);
            }
        }

        /// <summary>
        /// specifies DisableIntersectTicksProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DisableIntersectTicksProperty = 
            DependencyProperty.Register("DisableIntersectTicks", typeof(bool), typeof(CircularScale), new PropertyMetadata(false,new PropertyChangedCallback(OnDisableIntersectTicksChanged)));
        /// <summary>
        /// specifies ScalePanelProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ScalePanelProperty =
       DependencyProperty.Register("ScalePanel", typeof(ScalesLayoutPanel), typeof(CircularScale), new PropertyMetadata(null));
        /// <summary>
        /// specifies PointersProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty PointersProperty =
        DependencyProperty.Register("Pointers", typeof(PointersCollection), typeof(CircularScale), new PropertyMetadata(new PointersCollection()));

        static ControlTemplate scaleTemplate;

        /// <summary>
        /// Gets or sets the DisableIntersectTicks.
        /// </summary>
        public bool DisableIntersectTicks
        {
            set { SetValue(DisableIntersectTicksProperty, value); }
            get { return (bool)GetValue(DisableIntersectTicksProperty); }
        }

        /// <summary>
        /// Gets or sets angle to draw the scale from. This is a dependency property.
        /// </summary>
        /// <value>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.PointerCap">PointerCap</see></para>
        /// </value>
        /// <seealso cref="PointerCap">PointerCap</seealso>
        public PointerCap PointerCap
        {
            get
            {
                return (PointerCap)GetValue(PointerCapProperty);
            }

            set
            {
                SetValue(PointerCapProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets collection of pointers.
        /// </summary>
        /// <value>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.PointersCollection">PointersCollection</see></para>
        /// </value>
        /// <seealso cref="PointersCollection">PointersCollection</seealso>
        public PointersCollection Pointers
        {
            get
            {
                return (PointersCollection)GetValue(PointersProperty);
            }

            set
            {
                SetValue(PointersProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius of the scale. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 120d.</para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }

            set
            {
                SetValue(RadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle of rotation where the scale will begin relatively to the radius. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0. </para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// <para> </para>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double StartAngle
        {
            get
            {
                return (double)GetValue(StartAngleProperty);
            }

            set
            {
                SetValue(StartAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ratio betweeen the scale size and gauge size.
        /// </summary>
        internal double RadiusRatio
        {
            get
            {
                return this.mradiusRatio;
            }

            set
            {
                this.mradiusRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the start angle of the scale.
        /// </summary>
        internal double startAngle
        {
            get
            {
                return this.startangle;
            }

            set
            {
                this.startangle = value;
            }
        }

        /// <summary>
        /// Gets or sets the start angle of the scale.
        /// </summary>
        internal double gapSweepAngle
        {
            get
            {
                return this.gapsweepangle;
            }

            set
            {
                this.gapsweepangle = value;
            }
        }

        /// <summary>
        /// Gets the scale panel .
        /// </summary>
        public ScalesLayoutPanel PART_ScalesPanel
        {
            get
            {
                return this.ScalePanel;
            }
        }
        
        #endregion
        #region Implementation
        /// <summary>
        /// Coerces the value of the <see cref="PointerCap"/> property.
        /// </summary>
        /// <param name="d">The <see cref="CircularScale"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoercePointerCap(DependencyObject d, object value)
        {
            CircularScale owner = d as CircularScale;
            return owner.CoercePointerCap(value);
        }

       
        /// <summary>
        /// On Apply Template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
            this.ScalePanel = this.GetTemplateChild("PART_ScalesPanel") as ScalesLayoutPanel;
            
            if (this.ScalePanel != null)
            {
                this.Ticks.VisualParent = this.ScalePanel;
                this.Pointers.VisualParent = this.ScalePanel;
                this.Ranges.VisualParent = this.ScalePanel;
            }

            this.UpdateVisualStyle();
            this.RefreshScaleFrame();

            this.mscalePath = this.GetTemplateChild("PART_PathEllipse") as Path;
            this.RefreshScalePath();
            this.ClearScaleLabelTicks();
            this.ClearScaleTicks();
            this.RefreshPointerCapVisibility();
            this.AddTickSet();
            this.AddLabelTickSet();
            this.AddPointerCap();
            foreach(PointerBase pointer in this.Pointers)
            {
                if (pointer is CircularPointer)
                {
                    if (!this.ScalePanel.Children.Contains(pointer))
                    {
                        if (pointer.Parent is Panel)
                        {
                            (pointer.Parent as Panel).Children.Remove(pointer);
                        }
                        this.ScalePanel.Children.Add(pointer);
                    }
                }
            }
            this.PointerCap.UpdateCapOnTop();

            scaleTemplate = this.Template;
         
          
        }

        /// <summary>
        /// Updates visual style
        /// </summary>
        /// <remarks></remarks>
        protected override void UpdateVisualStyle()
        {
            if (this.GaugeElementParent != null)
                this.VisualStyle = (this.GaugeElementParent as GaugeBase).VisualStyle;
            base.UpdateVisualStyle();
        }

        internal CircularGauge GetGaugeParent(DependencyObject ele)
        {
            while (ele != null && !(ele is CircularGauge))
            {
                ele = VisualTreeHelper.GetParent(ele);
            }
            return ele as CircularGauge;
        }

        internal void RefreshScaleFrame()
        {
            CircularGauge gauge = this.GaugeElementParent as CircularGauge;
            if (gauge != null)
            {
                if (gauge.FrameType == GaugeFrameType.SemiCircular)
                {
                    if (this.StartAngle < 180)
                    {
                        this.startAngle = 180;
                    }

                    if (this.GapSweepAngle > 180)
                    {
                        this.gapSweepAngle = 180;
                        if (this.gapSweepAngle + this.startAngle > 360)
                        {
                            this.gapSweepAngle = 360 - this.startAngle;
                        }
                    }
                }
                else if (gauge.FrameType == GaugeFrameType.QuarterCircular)
                {
                    if (this.StartAngle < 270)
                    {
                        this.startAngle = 270;
                    }

                    if (this.GapSweepAngle > 90)
                    {
                        this.gapSweepAngle = 90;
                        if (this.gapSweepAngle + this.startAngle > 360)
                        {
                            this.gapSweepAngle = 360 - this.startAngle;
                        }
                    }
                }
                else
                {
                    this.startAngle = this.StartAngle;
                    this.gapSweepAngle = this.GapSweepAngle;
                }
            }
            else
            {
                this.startAngle = this.StartAngle;
                this.gapSweepAngle = this.GapSweepAngle;
            }

        }

        /// <summary>
        /// Invoked to remove a pointer cap
        /// </summary>
        internal void RemovePointerCap()
        {
            if (this.ScalePanel != null && this.ScalePanel.Children.Contains(this.PointerCap))
            {
                this.ScalePanel.Children.Remove(this.PointerCap);
                this.PointerCap.GaugeElementParent = null;
            }
        }

        /// <summary>
        /// Invoked for adding a Pointer cap
        /// </summary>
        internal void AddPointerCap()
        {
            if (this.ScalePanel != null && !this.ScalePanel.Children.Contains(this.PointerCap))
            {
                if (this.PointerCap.Parent != null)
                {
                    ScalesLayoutPanel panel = this.PointerCap.Parent as ScalesLayoutPanel;
                    panel.Children.Remove(this.PointerCap);
                }

                this.ScalePanel.Children.Add(this.PointerCap);
                this.PointerCap.GaugeElementParent = this;
            }
        }

        /// <summary>
        /// Converts polar coordinates to stage coordinates.
        /// </summary>
        /// <param name="radius">Radius to convert.</param>
        /// <param name="angle">Angle to convert.</param>
        /// <param name="pointToShift">Point to shift the result point by.</param>
        /// <returns>Calculated coordinates.</returns>
        internal Point ConvertToStageCoordinates(double radius, double angle, Point pointToShift)
        {
            Point point = new Point(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));
            point.X += pointToShift.X;
            point.Y += pointToShift.Y;
            return point;
        }

        /// <summary>
        /// Gets the angle according to the value.
        /// </summary>
        /// <param name="value">Value for the Angle</param>
        /// <returns>Returns Calculated angle.</returns>
        internal double GetAngleByValue(double value)
        {
            double ratio=0, angle=0;
            if (this.IsReversed)
            {
                if (this.Minimum <= value && value <= this.Maximum)
                {
                    ratio = (this.Maximum - this.Minimum) / (this.Maximum - value);
                    angle = (this.gapSweepAngle / ratio) + this.startAngle;
                }
            }
            else
            {
                ratio = (this.Maximum - this.Minimum) / (value - this.Minimum);
                angle = (this.gapSweepAngle / ratio) + this.startAngle;
            }

            return angle;
        }

        int GetMaximumDecimalDigits(double val)
        {
            string str = val.ToString();
            int dot = str.IndexOf('.');
            int n = dot != -1 ? str.Length - dot - 1 : 3;
            return n > 3 ? n : 3;
        }

        internal NumberFormatInfo GetNumberFormatInfo(double labelValue, LabelTickSet labelTick)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            int trailDigit = this.GetMaximumDecimalDigits(this.MajorIntervalValue);
            if (labelTick.LabelFormat == null)
            {
                format.NumberDecimalDigits = labelTick.IsLogarithmic ? labelTick.GetLogDecimalCountNumber(Math.Log(labelValue, 10)) : labelTick.GetDecimalCountNumber(labelValue, trailDigit);
            }
            else
            {
                format = labelTick.LabelFormat.Clone() as NumberFormatInfo;
                if (labelTick.RemoveTrailingZero)
                {
                    format.NumberDecimalDigits=labelTick.GetDecimalCountNumber(labelValue, labelTick.LabelFormat.NumberDecimalDigits);
                }
            }
            return format;
        }

        /// <summary>
        /// Gets the format according to the value.
        /// </summary>
        /// <param name="labelValue">Value for the Label</param>
        /// <param name="labelTick">Value for the LableTick</param>
        /// <returns>Returns formatted value.</returns>
        internal string GetFormatedValue(double labelValue, LabelTickSet labelTick)
        {
            double value = labelValue;
            double labelformulaValue = 0;
            NumberFormatInfo format = GetNumberFormatInfo(labelValue, labelTick);
            if (labelTick.EnableFormulaCalculation == true)
            {
                labelformulaValue = labelTick.CalculateLabelValue(labelValue, labelTick.LabelFormula);                
                if (labelTick.IsLogarithmic)
                {
                    value = Math.Log(labelformulaValue, labelTick.LogBase);
                }
                else
                {
                    value = labelformulaValue;
                }
            }
            else
            {
                if (labelTick.IsLogarithmic)
                {
                    value = Math.Log(labelValue, labelTick.LogBase);
                }
            }
            string formattedValueString = value.ToString("N", format);
            double formattedValue = Convert.ToDouble(formattedValueString, format);
            int trailDigits = -1;
            if (labelTick.LabelFormat == null && !labelTick.IsLogarithmic)
            {
                trailDigits = labelTick.GetDecimalCountNumber(formattedValue, format.NumberDecimalDigits);
            }
            else if (labelTick.LabelFormat != null && labelTick.RemoveTrailingZero)
            {
                trailDigits = labelTick.GetDecimalCountNumber(formattedValue, labelTick.LabelFormat.NumberDecimalDigits);
            }
            if (trailDigits != -1 && format.NumberDecimalDigits!=trailDigits)
            {
                format.NumberDecimalDigits = trailDigits;
                formattedValueString = formattedValue.ToString("N", format);
            }
            return formattedValueString;
        }

        /// <summary>
        /// Gets the format according to the value.
        /// </summary>
        /// <param name="labelValue">Value for the Label</param>
        /// <param name="labelTick">Value for the LableTick</param>
        /// <returns>Returns formatted value.</returns>
        internal double GetFormatedDoubleValue(double labelValue, LabelTickSet labelTick)
        {   
            NumberFormatInfo format = GetNumberFormatInfo(labelValue, labelTick);            
            return Convert.ToDouble(this.GetFormatedValue(labelValue,labelTick),format);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="PointerCap"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoercePointerCap(object value)
        {
            return value;
        }

        /// <summary>
        /// Refreshs scale
        /// </summary>
        protected internal override void RefreshScale()
        {
            if (this.ScalePanel != null)
            {
                //if (indicator)
                //{
                //    if (cflag < this.gapsweepangle)
                //    {
                //        return;
                //    }
                //}
                this.ScalePanel.InvalidateMeasure();
                this.ScalePanel.InvalidateArrange();

                foreach (FrameworkElement elem in this.ScalePanel.Children)
                {
                    if (elem is CircularRange)
                    {
                        (elem as CircularRange).RefreshRange();
                    }
                }
            }
        }

        /// <summary>
        /// Refreshs the Tickset
        /// </summary>
        protected internal override void RefreshTickSet()
        {
            double interval = 0;
            int flag = 0;
            if (this.ScalePanel != null)
            {
                //if (indicator)
                //{
                //    if (cflag < this.gapsweepangle)
                //    {
                //        return;
                //    }
                //}
                foreach (TickSetBase tickSet in this.Ticks)
                {
                    double angleToPlace;
                    if (this.IsReversed)
                    {
                        angleToPlace = this.startAngle + this.gapSweepAngle;
                    }
                    else
                    {
                        angleToPlace = this.startAngle;
                    }

                    double i = 0;
                    if (tickSet is MarkTickSet)
                    {
                        MarkTickSet markTick = tickSet as MarkTickSet;
                        
                        if (markTick.TickStyle == TickStyle.MajorTick)
                        {
                            interval = this.MajorIntervalValue;
                            flag = markTick.majorflag;
                        }
                        else if (markTick.TickStyle == TickStyle.MidTick)
                        {
                            interval = this.MidIntervalValue;
                            flag = markTick.middleflag;
                        }
                        else
                        {
                            interval = this.MinorIntervalValue;
                            flag = markTick.minorflag;
                        }

                        double ratio = (this.Maximum - this.Minimum) / interval;
                        double angleInterval = this.gapSweepAngle / ratio;
                        double posX = 0;
                        if (markTick.TickPlacement == ScalePlacement.Cross)
                        {
                            posX = ((this.ScaleBarSize - markTick.TickHeight) / 2) - markTick.DistanceFromScale;
                        }
                        else if (markTick.TickPlacement == ScalePlacement.Inside)
                        {
                            posX = this.ScaleBarSize + markTick.DistanceFromScale;
                        }
                        else if (markTick.TickPlacement == ScalePlacement.Outside)
                        {
                            posX = -markTick.TickHeight - markTick.DistanceFromScale;
                        }

                        bool SkipTickSet = false;

                        if (angleInterval > 0)
                        {
                            foreach (FrameworkElement elem in this.ScalePanel.Children)
                            {
                                if (elem is MarkTickSet)
                                {
                                    MarkTickSet tick = elem as MarkTickSet;
                                    //tick.Visibility = Visibility.Visible;
                                    if (tick.GaugeElementParent == markTick)
                                    {
                                        if (IsReversed)
                                        {
                                            if ((angleToPlace < this.startAngle) && ((this.StartAngle - angleToPlace) < angleInterval))
                                            {
                                                angleToPlace = this.startAngle;
                                            }
                                            //if (angleToPlace < (this.startAngle + this.gapSweepAngle) && 
                                            //    (angleToPlace - (this.startAngle + this.gapSweepAngle)) > angleInterval)
                                            //{
                                            //    angleToPlace = this.startAngle;
                                            //}
                                        }
                                        else
                                        {
                                            if (angleToPlace > (this.startAngle + this.gapSweepAngle) && 
                                                (angleToPlace - (this.startAngle + this.gapSweepAngle)) < angleInterval)
                                            {
                                                angleToPlace = this.startAngle + this.gapSweepAngle;
                                            }
                                        }
                                        
                                        #region DisableIntersectTicks
                                        SkipTickSet = false;
                                        if (((tickSet.TickStyle == TickStyle.MinorTick) || (tickSet.TickStyle == TickStyle.MidTick)) && (this.DisableIntersectTicks == true))
                                        {
                                            foreach (TickSetBase temp_tick in this.Ticks)
                                            {
                                                if (temp_tick.TickStyle == TickStyle.MajorTick)
                                                {
                                                    if (this.IsReversed)
                                                    {
                                                        double ratio1 = (this.Maximum - this.Minimum) / this.MajorIntervalValue;
                                                        double angleInterval1 = this.gapSweepAngle / ratio1;
                                                        double check = (this.StartAngle + this.GapSweepAngle - angleToPlace) % angleInterval1;
                                                        if (check < 0.1 || (angleInterval1 - check) < 0.1)
                                                        {
                                                            check = 0;
                                                        }

                                                        if ((check == 0) || (angleToPlace == this.startAngle + this.gapSweepAngle) || (angleToPlace == this.StartAngle))
                                                        {
                                                            SkipTickSet = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        double ratio1 = (this.Maximum - this.Minimum) / this.MajorIntervalValue;
                                                        double angleInterval1 = this.gapSweepAngle / ratio1;
                                                        double check = (angleToPlace - this.startangle) % angleInterval1;
                                                        if (check < 0.1 || (angleInterval1 - check) < 0.1)
                                                        {
                                                            check = 0;
                                                            //SkipTickSet = true;
                                                        }

                                                        if ((check == 0) || (angleToPlace == this.startAngle + this.gapSweepAngle) || (angleToPlace == this.StartAngle))
                                                        {
                                                            SkipTickSet = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        if (!SkipTickSet)
                                        {
                                            TransformGroup transformGroup = new TransformGroup();
                                            RotateTransform transform1 = new RotateTransform();
                                            transform1.Angle = markTick.Angle;
                                            transform1.CenterX = markTick.TickHeight / 2;
                                            transform1.CenterY = markTick.TickWidth / 2;

                                            RotateTransform transform2 = new RotateTransform();
                                            if (angleToPlace % 360 > (this.gapSweepAngle / 2) + this.startAngle)
                                                transform2.Angle = angleToPlace - 180 - markTick.TickWidth / 4;
                                            else if (angleToPlace % 360 == (this.gapSweepAngle / 2) + this.startAngle)
                                            {
                                                transform2.Angle = angleToPlace - 180 - markTick.TickWidth / 8;
                                            }
                                            else
                                                transform2.Angle = angleToPlace - 180;
                                            transform2.CenterX = this.Radius - posX;
                                            transform2.CenterY = markTick.TickWidth / 2;

                                            transformGroup.Children.Add(transform1);
                                            transformGroup.Children.Add(transform2);
                                            if (markTick.ShowToolTip)
                                            {
                                                tick.ticktooltip = i.ToString();
                                            }
                                            else
                                            {
                                                tick.ticktooltip = null;
                                            }

                                            tick.XOffset = posX;
                                            tick.RenderTransform = transformGroup;
                                            if (flag == 0 || flag == 4)
                                                tick.TickBackground = markTick.TickBackground;
                                            tick.TickWidth = markTick.TickWidth;
                                            tick.TickHeight = markTick.TickHeight;
                                            tick.TickStyle = markTick.TickStyle;
                                            tick.TickShape = markTick.TickShape;
                                            tick.TickPlacement = markTick.TickPlacement;
                                            tick.RefreshTick();

                                            if (this.IsReversed)
                                            {
                                                angleToPlace -= angleInterval;
                                            }
                                            else
                                            {
                                                angleToPlace += angleInterval;
                                            }

                                            i += angleInterval;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Radius"/> dependency property.
        /// </summary>
        /// <param name="val">The value that should be corrected.</param>
        /// <returns>The corrected val.</returns>
        protected internal virtual double CoerceRadius(double val)
        {
            if (val < 0)
            {
                val = 0;
            }
            
            return val;
        }

        /// <summary>
        /// Refreshes pointers position according to the values.
        /// </summary>
        internal void RefreshPointers()
        {
            foreach (PointerBase pointer in this.Pointers)
            {
                if (pointer is CircularPointer)
                {
                    CircularPointer circularpointer = pointer as CircularPointer;
                    if (circularpointer.Value < this.Minimum)
                    {
                        circularpointer.Value = this.Minimum;
                    }

                    if (circularpointer.Value > this.Maximum)
                    {
                        circularpointer.Value = this.Maximum;
                    }

                    circularpointer.AnglePosition = this.GetAngleByValue(circularpointer.Value);
                    circularpointer.RefreshAnglePosition();
                }
            }
        }


        /// <summary>
        /// Refreshes label Tickset
        /// </summary>
        protected internal override void RefreshLabelTickSet()
        {
            //if (indicator)
            //{
            //    if (cflag < this.gapsweepangle)
            //    {
            //        return;
            //    }
            //}
            double interval = 0;
            CircularGauge gauge = this.GaugeElementParent as CircularGauge;
            if (gauge != null)
            {
                if (gauge.FrameType == GaugeFrameType.QuarterCircular)
                {                    
                    switch (gauge.GaugeFrameDirection)
                    {
                        case FrameDirection.East: this.rotateangle = 45;
                            break;
                        case FrameDirection.West: this.rotateangle = -135;
                            break;
                        case FrameDirection.North: this.rotateangle = -45;
                            break;
                        case FrameDirection.South: this.rotateangle = 135;
                            break;
                        case FrameDirection.SouthEast: this.rotateangle = 90;
                            break;
                        case FrameDirection.SouthWest: this.rotateangle = 180;
                            break;
                        case FrameDirection.NorthEast: this.rotateangle = 0;
                            break;
                        case FrameDirection.NorthWest: this.rotateangle = -90;
                            break;
                    }
                }
                else if (gauge.FrameType==GaugeFrameType.SemiCircular)
                {
                    switch (gauge.GaugeFrameDirection)
                    {
                        case FrameDirection.East: this.rotateangle = 90;
                            break;
                        case FrameDirection.West: this.rotateangle = -90;
                            break;
                        case FrameDirection.North: this.rotateangle = 0;
                            break;
                        case FrameDirection.South: this.rotateangle = 180;
                            break;
                        case FrameDirection.SouthEast: this.rotateangle = 0;
                            break;
                        case FrameDirection.SouthWest: this.rotateangle = 0;
                            break;
                        case FrameDirection.NorthEast: this.rotateangle = 0;
                            break;
                        case FrameDirection.NorthWest: this.rotateangle = 0;
                            break;
                    }
                }
            }         

            foreach (TickSetBase tickSet in this.Ticks)
            {
                double angleToPlace;
                if (this.IsReversed)
                {
                    angleToPlace = this.startAngle+this.gapSweepAngle;
                }
                else
                {
                    angleToPlace = this.startAngle;
                }

                double i = 0;
                double labelValue = this.Minimum;
                string value;

                if (tickSet is LabelTickSet)
                {
                    LabelTickSet labelTick = tickSet as LabelTickSet;
                    if (labelTick.TickStyle == TickStyle.MajorTick)
                    {
                        interval = this.MajorIntervalValue;
                    }
                    else if (labelTick.TickStyle == TickStyle.MidTick)
                    {
                        interval = this.MidIntervalValue;
                    }
                    else
                    {
                        interval = this.MinorIntervalValue;
                    }
                    double ratio = (this.Maximum - this.Minimum) / interval;
                    double angleInterval = this.gapSweepAngle / ratio;
                    if (angleInterval > 0 && this.ScalePanel != null)
                    {
                        bool isLast = false;
                        for (int k = 0; k < this.ScalePanel.Children.Count;k++)
                        {
                            FrameworkElement elem = this.ScalePanel.Children[k] as FrameworkElement;
                            if (elem is LabelTickSet)
                            {                                
                                if (isLast)
                                {
                                    this.ScalePanel.Children.Remove(elem);
                                    k--;
                                }
                                else
                                {
                                LabelTickSet tick = elem as LabelTickSet;
                                tick.Visibility = Visibility.Visible;
                                if (tick.GaugeElementParent == this)
                                {
                                    if (!labelTick.IncludeFirstValue && labelValue == this.Minimum)
                                    {
                                        if (this.IsReversed)
                                        {
                                            angleToPlace -= angleInterval;
                                            labelValue += interval;
                                            i += angleInterval;
                                        }
                                        else
                                        {
                                            angleToPlace += angleInterval;
                                            labelValue += interval;
                                            i += angleInterval;
                                        }
                                    }

                                    value = this.GetFormatedValue(labelValue, labelTick);
                                    value = labelTick.Prefix + value + labelTick.Suffix;
                                    if ((Convert.ToDouble(this.GetFormatedDoubleValue(labelValue,tick)) >= this.Maximum) || ((angleToPlace > (this.startAngle + this.gapSweepAngle)) && ((angleToPlace - (this.startAngle + this.gapSweepAngle)) < angleInterval)))
                                    {
                                        if (this.IsReversed)
                                        {
                                            angleToPlace = this.startAngle;
                                        }
                                        else
                                        {
                                            angleToPlace = this.startAngle + this.gapSweepAngle;
                                        }

                                        labelValue = this.Maximum;
                                        value = this.GetFormatedValue(labelValue, labelTick);
                                        value = labelTick.Prefix + value + labelTick.Suffix;
                                    }

                                    Size size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                    double lengthX = size.Width > size.Height ? size.Width : size.Height;
                                    double lengthY = size.Width > size.Height ? size.Height : size.Width;
                                    double posX = 0;
                                    if (labelTick.TickPlacement == ScalePlacement.Cross)
                                    {
                                        posX = ((this.ScaleBarSize - lengthX) / 2) - labelTick.DistanceFromScale;
                                    }
                                    else if (labelTick.TickPlacement == ScalePlacement.Inside)
                                    {
                                        posX = this.ScaleBarSize + labelTick.DistanceFromScale;
                                    }
                                    else if (labelTick.TickPlacement == ScalePlacement.Outside)
                                    {
                                        posX = -lengthX - labelTick.DistanceFromScale;
                                    }

                                    TransformGroup transformGroup = new TransformGroup();

                                    RotateTransform transform1 = new RotateTransform();
                                    transform1.Angle = labelTick.Angle - angleToPlace - this.rotateangle - 180;
                                    transform1.CenterX = lengthX / 2;
                                    transform1.CenterY = lengthY / 2;

                                    RotateTransform transform2 = new RotateTransform();
                                    transform2.Angle = angleToPlace - 180;
                                    transform2.CenterX = this.Radius - posX;
                                    transform2.CenterY = lengthY / 2;

                                    transformGroup.Children.Add(transform1);
                                    transformGroup.Children.Add(transform2);
                                    tick.XOffset = posX;
                                    if (labelTick.ShowToolTip)
                                    {
                                        tick.tooltip = value;
                                    }
                                    else
                                    {
                                        tick.tooltip = null;
                                    }

                                    tick.Text = value;
                                    tick.RefreshLabelTick(labelTick);
                                    tick.RenderTransform = transformGroup;
                                    if (labelTick.flag == 0 || labelTick.flag == 4)
                                        tick.LabelForeground = labelTick.LabelForeground;
                                    if (this.IsReversed)
                                    {
                                        angleToPlace -= angleInterval;
                                    }
                                    else
                                    {
                                        angleToPlace += angleInterval;
                                    }
                                    if (labelValue == this.Maximum)
                                    {
                                        isLast = true;
                                    }
                                    labelValue += interval;
                                    decimal temval = Convert.ToDecimal(labelValue);
                                    labelValue = (double)temval;
                                    i += angleInterval;
                                }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GapSweepAngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGapSweepAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.gapSweepAngle = this.GapSweepAngle;
            this.RefreshScalePath();
            this.RefreshTickSet();
            this.RefreshLabelTickSet();
            for (int i = 0; i < this.Pointers.Count; i++)
            {
                if (this.Pointers[i].ToString().Contains("Pointer"))
                {
                    (this.Pointers[i] as CircularPointer).AnglePosition = (this.Pointers[i] as CircularPointer).GetAngleByValue((this.Pointers[i] as CircularPointer).Value);
                    (this.Pointers[i] as CircularPointer).RefreshAnglePosition();
                }
                else
                {
                    (this.Pointers[i] as CircularKnob).AnglePosition = (this.Pointers[i] as CircularKnob).GetAngleByValue((this.Pointers[i] as CircularKnob).KnobValue);
                    (this.Pointers[i] as CircularKnob).RefreshAnglePosition();
                }
            }

            if (this.GapSweepAngleChanged != null)
            {
                this.GapSweepAngleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMajorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            if ((this.Maximum - this.Minimum) % this.MajorIntervalValue != 0)
            {
                setMajorInterval=false;
            }
            this.RefreshScale();
            this.RefreshScalePath();
            this.AddTickSet();
            this.AddLabelTickSet();
            this.MajorTicks = (int)((this.Maximum - this.Minimum) / this.MajorIntervalValue);
            setMajorInterval = true;
           
            base.OnMajorIntervalValueChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMidIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
            this.MidTicks = (int)(this.MajorIntervalValue / this.MidIntervalValue);            
           
            base.OnMidIntervalValueChanged(e);
        }


        private static void OnDisableIntersectTicksChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CircularScale scale = (CircularScale)obj;
            scale.OnDisableIntersectTicksChanged(obj);
        }

        private void OnDisableIntersectTicksChanged(DependencyObject obj)
        {
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMajorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
            if (setMajorInterval)
            {
                this.MajorIntervalValue = (this.Maximum - this.Minimum) / this.MajorTicks;
            }

            this.MidIntervalValue = this.MajorIntervalValue / this.MidTicks;
            if(this.MinorTicks!=0)
                this.MinorIntervalValue = this.MajorIntervalValue / this.MinorTicks;
           
           
                
            base.OnMajorTicksChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMidTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
            this.MidIntervalValue = this.MajorIntervalValue / this.MidTicks;
           
            base.OnMidTicksChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMinorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
            if (setMinorInterval)
                this.MinorIntervalValue = this.MajorIntervalValue / this.MinorTicks;           
            base.OnMinorTicksChanged(e);
        }        

        /// <summary>
        /// Invoked when Maximum property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnMaximumChanged(e);
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();

            for (int i = 0; i < this.Pointers.Count; i++)
            {
                if (this.Pointers[i].ToString().Contains("Pointer"))
                {
                    if ((this.Pointers[i] as CircularPointer).Value > this.Maximum)
                    {
                        (this.Pointers[i] as CircularPointer).Value = this.Maximum;
                    }

                    (this.Pointers[i] as CircularPointer).AnglePosition = (this.Pointers[i] as CircularPointer).GetAngleByValue((this.Pointers[i] as CircularPointer).Value);
                    (this.Pointers[i] as CircularPointer).RefreshAnglePosition();
                }
                else
                {
                    if ((this.Pointers[i] as CircularKnob).KnobValue > this.Maximum)
                    {
                        (this.Pointers[i] as CircularKnob).KnobValue = this.Maximum;
                    }

                    (this.Pointers[i] as CircularKnob).AnglePosition = (this.Pointers[i] as CircularKnob).GetAngleByValue((this.Pointers[i] as CircularKnob).KnobValue);
                    (this.Pointers[i] as CircularKnob).RefreshAnglePosition();
                }
            }
        }

        /// <summary>
        /// Invoked when Minimum property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnMinimumChanged(e);
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();

            for (int i = 0; i < this.Pointers.Count; i++)
            {
                if (this.Pointers[i].ToString().Contains("Pointer"))
                {
                    if ((this.Pointers[i] as CircularPointer).Value < this.Minimum)
                    {
                        (this.Pointers[i] as CircularPointer).Value = this.Minimum;
                    }

                    (this.Pointers[i] as CircularPointer).AnglePosition = (this.Pointers[i] as CircularPointer).GetAngleByValue((this.Pointers[i] as CircularPointer).Value);
                    (this.Pointers[i] as CircularPointer).RefreshAnglePosition();
                }
                else
                {
                    if ((this.Pointers[i] as CircularKnob).KnobValue < this.Minimum)
                    {
                        (this.Pointers[i] as CircularKnob).KnobValue = this.Minimum;
                    }

                    (this.Pointers[i] as CircularKnob).AnglePosition = (this.Pointers[i] as CircularKnob).GetAngleByValue((this.Pointers[i] as CircularKnob).KnobValue);
                    (this.Pointers[i] as CircularKnob).RefreshAnglePosition();
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMinorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
            setMinorInterval = false;
            this.MinorTicks = (int)(this.MajorIntervalValue / this.MinorIntervalValue);
            setMinorInterval = true;
            base.OnMinorIntervalValueChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnIsReversedChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.RefreshLabelTickSet();
            this.RefreshTickSet();
            this.RefreshPointers();
            base.OnIsReversedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerCapChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            if (this.ScalePanel != null && !this.ScalePanel.Children.Contains(this.PointerCap))
            {
                this.ScalePanel.Children.Add(this.PointerCap);
            }

            if (this.PointerCapChanged != null)
            {
                this.PointerCapChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RadiusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            //indicator = false;
            this.RefreshTickSet();
            this.RefreshLabelTickSet();
            this.RefreshScalePath();
            if (this.GaugeElementParent is CircularGauge)
            {
                CircularGauge gauge = this.GaugeElementParent as CircularGauge;
                gauge.RefreshScalesPanel();
            }

            if (this.RadiusChanged != null)
            {
                this.RadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnScaleBarSizeChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            //indicator = false;
            this.RefreshScalePath();
            this.RefreshTickSet();
            this.RefreshLabelTickSet();
            this.RefreshScale();
            base.OnScaleBarSizeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartAngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            //indicator = false;
            this.startAngle = this.StartAngle;
            this.RefreshScalePath();
            this.RefreshTickSet();
            this.RefreshLabelTickSet();
            for (int i = 0; i < this.Pointers.Count; i++)
            {
                if (this.Pointers[i].ToString().Contains("Pointer"))
                {
                    (this.Pointers[i] as CircularPointer).AnglePosition = (this.Pointers[i] as CircularPointer).GetAngleByValue((this.Pointers[i] as CircularPointer).Value);
                    (this.Pointers[i] as CircularPointer).RefreshAnglePosition();
                }
                else
                {
                    (this.Pointers[i] as CircularKnob).AnglePosition = (this.Pointers[i] as CircularKnob).GetAngleByValue((this.Pointers[i] as CircularKnob).KnobValue);
                    (this.Pointers[i] as CircularKnob).RefreshAnglePosition();
                }
            }

            if (this.StartAngleChanged != null)
            {
                this.StartAngleChanged(this, e);
            }
        }
                
        /// <summary>
        /// Calls OnGapSweepAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGapSweepAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            CircularScale instance = (CircularScale)d;
            instance.OnGapSweepAngleChanged(e);
            //instance.indicator = false;
        }

        /// <summary>
        /// Calls OnPointerCapChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnPointerCapChanged(e);
        }

        /// <summary>
        /// Calls OnRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            //instance.indicator = false;
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;

            instance.mnestLevel++;
            double coercedValue = instance.CoerceRadius(newValue);
            if (newValue != coercedValue)
            {
                instance.Radius = coercedValue;
            }

            instance.mnestLevel--;
            if (instance.mnestLevel == 0)
            {
                RoutedPropertyChangedEventArgs<double> args =
                    new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);

                instance.OnRadiusChanged(args);
            }
        }

        /// <summary>
        /// Calls OnStartAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnStartAngleChanged(e);
            
        }

        /// <summary>
        /// Occurs when element is added, replaced or removed from the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //indicator = false;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (GaugeElement elem in e.NewItems)
                {
                    if (this.PART_ScalesPanel != null)
                    {
                        if (elem is CircularPointer)
                        {
                            (elem as CircularPointer).PointerNeedleTypeChanged += new PropertyChangedCallback(this.PointerNeedleTypeChanged);
                        }

                        //if (!this.PART_ScalesPanel.Children.Contains(elem))
                        //{
                            //FrameworkElement parent = elem.GaugeElementParent;
                            //elem.GaugeElementParent = null;
                            //this.PART_ScalesPanel.Children.Add(elem);
                            //elem.GaugeElementParent = parent;

                            if (this.PointerCap.CapOnTop == true)
                            {
                                this.RemovePointerCap();
                                elem.GaugeElementParent = this;
                                if (this.PART_ScalesPanel != null)
                                this.PART_ScalesPanel.Children.Add(elem);
                                this.AddPointerCap();
                            }
                            else
                            {
                                FrameworkElement parent = elem.GaugeElementParent;
                                elem.GaugeElementParent = null;
                                this.PART_ScalesPanel.Children.Add(elem);
                                elem.GaugeElementParent = parent;

                                //elem.GaugeElementParent = this;
                                //this.PART_ScalesPanel.Children.Add(elem);
                            }
                        //}
                    }
                }
            }
            
        }

        /// <summary>
        /// Invoked for adding Label tick set
        /// </summary>
        private void AddLabelTickSet()
        {
            double interval = 0;
            CircularGauge gauge = this.GaugeElementParent as CircularGauge;
            if (gauge!=null)
            {
            if (gauge.FrameType == GaugeFrameType.QuarterCircular && this.rotateangle == 0)
            {
                switch (gauge.GaugeFrameDirection)
                {
                    case FrameDirection.East: this.rotateangle += 45;
                        break;
                    case FrameDirection.West: this.rotateangle -= 135;
                        break;
                    case FrameDirection.North: this.rotateangle -= 45;
                        break;
                    case FrameDirection.South: this.rotateangle += 135;
                        break;
                    case FrameDirection.SouthEast: this.rotateangle += 90;
                        break;
                    case FrameDirection.SouthWest: this.rotateangle += 180;
                        break;
                    case FrameDirection.NorthEast: this.rotateangle = 0;
                        break;
                    case FrameDirection.NorthWest: this.rotateangle -= 90;
                        break;
                }
                }              
            }         

            foreach (TickSetBase tickSet in this.Ticks)
            {
                double angleToPlace = this.startAngle;
                double i = 0;
                double labelValue = this.Minimum;
                string value;

                if (tickSet is LabelTickSet)
                {

                    LabelTickSet labelTick = tickSet as LabelTickSet;
                    if (labelTick.TickStyle == TickStyle.MajorTick)
                    {
                        interval = this.MajorIntervalValue;
                    }
                    else if (labelTick.TickStyle == TickStyle.MidTick)
                    {
                        interval = this.MidIntervalValue;
                    }
                    else
                    {
                        interval = this.MinorIntervalValue;
                    }

                    double ratio = (this.Maximum - this.Minimum) / interval;
                    double angleInterval = this.gapSweepAngle / ratio;
                    double interValue = 0;
                    if (angleInterval > 0)
                    {
                        if (!labelTick.IncludeFirstValue && interval == this.Minimum)
                        {
                            angleToPlace += angleInterval;
                            labelValue += interval;
                            i += angleInterval;
                        }

                        if (this.IsReversed)
                        {
                            angleToPlace = this.startAngle + this.gapSweepAngle;
                        }
                        if (this.ScalePanel != null)
                        {
                            if (this.ScalePanel.Children.IndexOf(labelTick) != -1)
                            {
                                this.ScalePanel.Children.Remove(labelTick);
                            }
                            if (labelTick.Parent != null)
                            {
                                ScalesLayoutPanel panel = labelTick.Parent as ScalesLayoutPanel;
                                panel.Children.Remove(labelTick);
                            }
                            this.ScalePanel.Children.Add(labelTick);
                            labelTick.Visibility = Visibility.Collapsed;
                            labelTick.GaugeElementParent = this;
                            double formattedLabelValue=this.GetFormatedDoubleValue(labelValue, labelTick);

                            while (i <= this.gapSweepAngle + 0.01 || (formattedLabelValue >= this.Maximum && (formattedLabelValue - this.Maximum) < interval && this.ShowLastValue))
                            {
                                interValue += angleInterval;
                                value = this.GetFormatedValue(labelValue, labelTick);
                                value = labelTick.Prefix + value + labelTick.Suffix;
                                if ((formattedLabelValue >= this.Maximum) || ((angleToPlace > (this.startAngle + this.gapSweepAngle)) && ((angleToPlace - (this.startAngle + this.gapSweepAngle)) < angleInterval)))
                                {
                                    labelValue = this.Maximum;
                                    if (!this.ShowLastValue)
                                        break;
                                    value = this.GetFormatedValue(labelValue, labelTick);
                                    value = labelTick.Prefix + value + labelTick.Suffix;
                                    if (this.IsReversed)
                                    {
                                        angleToPlace = this.startAngle;
                                    }
                                    else
                                    {
                                        angleToPlace = this.startAngle + this.gapSweepAngle;
                                    }
                                }
                                Size size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                if (this.GapSweepAngle > 180 && this.gapsweepangle / 2 < interValue)
                                {
                                    int lVal = (int)labelValue;
                                    size = this.GetTextSize(lVal.ToString(), labelTick.FontFamily, labelTick.FontSize);
                                }
                                double lengthX = size.Width > size.Height ? size.Width : size.Height;
                                double lengthY = size.Width > size.Height ? size.Height : size.Width;
                                double posX = 0;
                                if (labelTick.TickPlacement == ScalePlacement.Cross)
                                {
                                    posX = ((this.ScaleBarSize - lengthX) / 2) - labelTick.DistanceFromScale;
                                }
                                else if (labelTick.TickPlacement == ScalePlacement.Inside)
                                {
                                    posX = this.ScaleBarSize + labelTick.DistanceFromScale;
                                }
                                else if (labelTick.TickPlacement == ScalePlacement.Outside)
                                {
                                    posX = -lengthX - labelTick.DistanceFromScale;
                                }

                                TransformGroup transformGroup = new TransformGroup();
                                RotateTransform transform1 = new RotateTransform();
                                transform1.Angle = labelTick.Angle - angleToPlace - this.rotateangle - 180;
                                transform1.CenterX = lengthX / 2;
                                transform1.CenterY = lengthY / 2;
                                RotateTransform transform2 = new RotateTransform();
                                transform2.Angle = angleToPlace - 180;
                                transform2.CenterX = this.Radius - posX;
                                transform2.CenterY = lengthY / 2;
                                transformGroup.Children.Add(transform1);
                                transformGroup.Children.Add(transform2);
                                LabelTickSet tick = new LabelTickSet();
                                tick.LabelForegroundChanged += new PropertyChangedCallback(tick_LabelForegroundChanged);
                                if (labelTick.ShowToolTip)
                                {
                                    tick.tooltip = value;
                                }
                                else
                                {
                                    tick.tooltip = null;
                                }

                                tick.XOffset = posX;
                                tick.Text = value;
                                tick.RenderTransform = transformGroup;
                               
                                this.ScalePanel.Children.Add(tick);
                                tick.GaugeElementParent = this;
                              
                                tick.RefreshLabelTick(labelTick);
                                if (labelTick.flag == 0 || labelTick.flag == 4)
                                    tick.LabelForeground = labelTick.LabelForeground;

                                if (this.IsReversed)
                                {
                                    angleToPlace -= angleInterval;
                                }
                                else
                                {
                                    angleToPlace += angleInterval;
                                }
                                if (labelValue == this.Maximum)
                                {
                                    break;
                                }
                                labelValue += interval;
                                decimal temval = Convert.ToDecimal(labelValue);
                                labelValue = (double)temval;
                                i += angleInterval;
                                formattedLabelValue = this.GetFormatedDoubleValue(labelValue, labelTick);
                            }
                        }
                    }
                }
            }
        }

        void tick_LabelForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private LabelTickSet Clone(LabelTickSet labelTick,string s)
        {
            LabelTickSet tick = new LabelTickSet();

            foreach (PropertyInfo curPropInfo in labelTick.GetType().GetProperties())
            {
                if (curPropInfo.GetGetMethod() != null
                    && (curPropInfo.GetSetMethod() != null))
                {
                    // Handle Non-indexer properties
                    if (curPropInfo.Name != "Item" && !curPropInfo.Name.Contains("Resources"))
                    {
                        // get property from source
                        object getValue = curPropInfo.GetGetMethod().Invoke(labelTick, new object[] { });

                                                   
                            // set property on cloned
                        curPropInfo.GetSetMethod().Invoke(tick, new object[] { getValue });
                    }
                }
            }
            tick.Name = s;
            return tick;
        }

        /// <summary>
        /// Invoked for adding tick set
        /// </summary>
        private void AddTickSet()
        {
            //indicator = true;
            double interval = 0;
            int flag = 0;
            if (this.ScalePanel != null)
            {
                foreach (TickSetBase tickSet in this.Ticks)
                {
                    double angleToPlace;
                    if (this.IsReversed)
                    {
                        angleToPlace = this.startAngle+this.gapSweepAngle;
                    }
                    else
                    {
                        angleToPlace = this.startAngle;
                    }

                    double i = 0;
                    int tickval = 0;
                    if (tickSet is MarkTickSet)
                    {
                        MarkTickSet markTick = tickSet as MarkTickSet;
                        if (markTick.TickStyle == TickStyle.MajorTick)
                        {
                            interval = this.MajorIntervalValue;
                            flag = markTick.majorflag;                            
                        }
                        else if (markTick.TickStyle == TickStyle.MidTick)
                        {
                            interval = this.MidIntervalValue;
                            flag = markTick.middleflag;
                        }
                        else
                        {
                            interval = this.MinorIntervalValue;
                            flag = markTick.minorflag;
                        }
                        
                        double ratio = (this.Maximum - this.Minimum) / interval;
                        double angleInterval = this.gapSweepAngle / ratio;
                        double posX = 0;
                        if (markTick.TickPlacement == ScalePlacement.Cross)
                        {
                            posX = ((this.ScaleBarSize - markTick.TickHeight) / 2) - markTick.DistanceFromScale;
                        }
                        else if (markTick.TickPlacement == ScalePlacement.Inside)
                        {
                            posX = this.ScaleBarSize + markTick.DistanceFromScale;
                        }
                        else if (markTick.TickPlacement == ScalePlacement.Outside)
                        {
                            posX = -markTick.TickHeight - markTick.DistanceFromScale;
                        }
                        if (this.ScalePanel.Children.IndexOf(markTick) != -1)
                        {
                            this.ScalePanel.Children.Remove(markTick);
                        }
                        if (markTick.Parent != null)
                        {
                            ScalesLayoutPanel panel = markTick.Parent as ScalesLayoutPanel;
                            panel.Children.Remove(markTick);
                        }
                        this.ScalePanel.Children.Add(markTick);
                        markTick.Visibility = Visibility.Collapsed;
                        markTick.GaugeElementParent = this;

                        bool SkipTickSet=false;

                        if (angleInterval > 0)
                        {
                            int count = 0;
                            while (i <= this.gapSweepAngle + 0.01 || (angleToPlace > (this.startAngle + this.gapSweepAngle) && 
                                ((int)angleToPlace - (this.startAngle + this.gapSweepAngle)) <= angleInterval && this.ShowLastValue))
                            {
                                count++;
                                int tem = 0;
                                if (this.IsReversed)
                                {
                                    if (angleToPlace <= this.startAngle)
                                    {
                                        angleToPlace = this.startAngle;
                                        tem = 1;
                                    }
                                }
                                else
                                {
                                    if (angleToPlace >= (this.startAngle + this.gapSweepAngle) && ((int)angleToPlace - (this.startAngle + this.gapSweepAngle)) < angleInterval)
                                    {
                                        angleToPlace = this.startAngle + this.gapSweepAngle;
                                        tem = 1;
                                    }
                                }

                                #region DisableIntersectTicks
                                SkipTickSet = false;
                                if (((tickSet.TickStyle == TickStyle.MinorTick) || (tickSet.TickStyle == TickStyle.MidTick)) && (this.DisableIntersectTicks == true))
                                {
                                    foreach (TickSetBase temp_tick in this.Ticks)
                                    {
                                        if (temp_tick.TickStyle == TickStyle.MajorTick)
                                        {
                                            if (this.IsReversed)
                                            {
                                                double ratio1 = (this.Maximum - this.Minimum) / this.MajorIntervalValue;
                                                double angleInterval1 = this.gapSweepAngle / ratio1;
                                                double check = (this.StartAngle + this.gapSweepAngle - angleToPlace) % angleInterval1;
                                                if (check < 0.1 || (angleInterval1 - check) < 0.1)
                                                {
                                                    check = 0;
                                                }

                                                if ((check == 0) || (angleToPlace == this.startAngle + this.gapSweepAngle) || (angleToPlace == this.StartAngle))
                                                {
                                                    SkipTickSet = true;
                                                }
                                            }
                                            else
                                            {
                                                double ratio1 = (this.Maximum - this.Minimum) / this.MajorIntervalValue;
                                                double angleInterval1 = this.gapSweepAngle / ratio1;
                                                double check = (angleToPlace - this.startangle) % angleInterval1;
                                                if (check < 0.1 || (angleInterval1 - check) < 0.1)
                                                {
                                                    check = 0;
                                                    //SkipTickSet = true;
                                                }

                                                if ((check == 0) || (angleToPlace == this.startAngle + this.gapSweepAngle) || (angleToPlace == this.StartAngle))
                                                {
                                                    SkipTickSet = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (!SkipTickSet)
                                {
                                    TransformGroup transformGroup = new TransformGroup();

                                    RotateTransform transform1 = new RotateTransform();
                                    transform1.Angle = markTick.Angle;
                                    transform1.CenterX = markTick.TickHeight / 2;
                                    transform1.CenterY = markTick.TickWidth / 2;

                                    RotateTransform transform2 = new RotateTransform();
                                    if (angleToPlace % 360 > (this.gapSweepAngle / 2) + this.startAngle)
                                        transform2.Angle = angleToPlace - 180 - markTick.TickWidth / 4;
                                    else if (angleToPlace % 360 == (this.gapSweepAngle / 2) + this.startAngle)
                                    {
                                        transform2.Angle = angleToPlace - 180 - markTick.TickWidth / 8;
                                    }
                                    else
                                        transform2.Angle = angleToPlace - 180;
                                    transform2.CenterX = this.Radius - posX;
                                    transform2.CenterY = markTick.TickWidth / 2;

                                    transformGroup.Children.Add(transform1);
                                    transformGroup.Children.Add(transform2);

                                    MarkTickSet tick = new MarkTickSet();


                                    if (tick.ShowToolTip)
                                    {
                                        tick.ticktooltip = (tickval * interval).ToString();
                                    }

                                    tickval++;
                                    tick.XOffset = posX;
                                    tick.RenderTransform = transformGroup;
                                    tick.RenderTransform = transformGroup;
                                    if (flag == 0 || flag == 4)
                                        tick.TickBackground = markTick.TickBackground;
                                    tick.TickWidth = markTick.TickWidth;
                                    tick.TickHeight = markTick.TickHeight;
                                    tick.TickStyle = markTick.TickStyle;
                                    tick.TickShape = markTick.TickShape;
                                    tick.TickPlacement = markTick.TickPlacement;
                                    tick.GaugeElementParent = this;
                                    if (angleToPlace <= this.gapSweepAngle + this.startAngle && angleToPlace >= this.startAngle)
                                        this.ScalePanel.Children.Add(tick);
                                    tick.GaugeElementParent = markTick;
                                }
                                if (this.IsReversed)
                                {
                                    if (angleToPlace == this.startAngle && tem == 1)
                                    {
                                        break;
                                    }
                                   
                                    if (angleToPlace <= (this.startAngle + this.gapSweepAngle))
                                    {
                                        angleToPlace -= angleInterval;
                                        if (this.startAngle < angleToPlace)
                                        {                                            
                                            i += angleInterval;
                                        }
                                    }                                    
                                }
                                else
                                {
                                    if (angleToPlace < (this.startAngle + this.gapSweepAngle))
                                    {
                                        angleToPlace += angleInterval;
                                        i += angleInterval;
                                    }

                                    if (angleToPlace == this.startAngle + this.gapSweepAngle && tem == 1)
                                    {
                                        break;
                                    }
                                }
                            } 
                            count--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the scale is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void CircularScaleLoaded(object sender, RoutedEventArgs e)
        {
            this.Ranges.UpdatePanelChildren();
            this.Pointers.UpdatePanelChildren();

            foreach (GaugeElement elem in this.Pointers)
            {
                elem.GaugeElementParent = this;
            }

            foreach (GaugeElement elem in this.Ticks)
            {
                elem.GaugeElementParent = this;
            }

            foreach (GaugeElement elem in this.Ranges)
            {
                elem.GaugeElementParent = this;
            }

            this.AddPointerCap();

            foreach (PointerBase pb in this.Pointers)
            {
                if (pb.ToString().Equals("Syncfusion.Windows.Gauge.CircularPointer"))
                {
                    foreach (CircularPointer pointer in this.Pointers)
                    {
                        if (pointer.Value < this.Minimum)
                        {
                            pointer.Value = this.Minimum;
                        }

                        if (pointer.Value > this.Maximum)
                        {
                            pointer.Value = this.Maximum;
                        }

                        pointer.AnglePosition = pointer.GetAngleByValue(pointer.Value);
                        pointer.RefreshAnglePosition();
                        pointer.PointerNeedleTypeChanged += new PropertyChangedCallback(this.PointerNeedleTypeChanged);
                    }
                }
                else
                {
                    foreach (CircularKnob knob in this.Pointers)
                    {
                        if (knob.KnobValue < this.Minimum)
                        {
                            knob.KnobValue = this.Minimum;
                        }

                        if (knob.KnobValue > this.Maximum)
                        {
                            knob.KnobValue = this.Maximum;
                        }

                        knob.AnglePosition = knob.GetAngleByValue(knob.KnobValue);
                        knob.RefreshAnglePosition();
                    }
                }
            }
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.RefreshPointerCapVisibility();
            this.AddTickSet();
            this.AddLabelTickSet();
        }

        /// <summary>
        /// Invoked to clear the scale label ticks
        /// </summary>
        private void ClearScaleLabelTicks()
        {
            if (this.ScalePanel != null)
            {
                int count = this.ScalePanel.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < this.ScalePanel.Children.Count && this.ScalePanel.Children[i] is LabelTickSet)
                    {
                        LabelTickSet tick = this.ScalePanel.Children[i] as LabelTickSet;
                        if (this.ScalePanel.Children.Contains(tick))
                        {
                            this.ScalePanel.Children.Remove(tick);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked to clear the scale ticks
        /// </summary>
        private void ClearScaleTicks()
        {
            if (this.ScalePanel != null)
            {
                int count = this.ScalePanel.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < this.ScalePanel.Children.Count && this.ScalePanel.Children[i] is MarkTickSet)
                    {
                        MarkTickSet tick = this.ScalePanel.Children[i] as MarkTickSet;
                        if (this.ScalePanel.Children.Contains(tick))
                        {
                            this.ScalePanel.Children.Remove(tick);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Text size for the text
        /// </summary>
        /// <param name="text">String whose siz is to be measured</param>
        /// <param name="fontFamily">Fontfamily of the measured string</param>
        /// <param name="fontSize">Fontsize of the measure string</param>
        /// <returns>Returns Text Size</returns>
        private Size GetTextSize(string text, FontFamily fontFamily, double fontSize)
        {
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = text;
            txtBlock.FontFamily = fontFamily;
            txtBlock.FontSize = fontSize;

            return new Size(txtBlock.ActualWidth, txtBlock.ActualHeight);
        }
                                
        /// <summary>
        /// Occurs when the type of any pointer is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void PointerNeedleTypeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointerCapVisibility();
        }

        /// <summary>
        /// Refreshes Pointer cap visibility
        /// </summary>
        private void RefreshPointerCapVisibility()
        {
            bool hide = true;
            for (int k = 0; k < this.Pointers.Count; k++)
            {
                if (this.Pointers[k].ToString().Equals("Syncfusion.Windows.Gauge.CircularPointer"))
                {
                    foreach (CircularPointer pointer in this.Pointers)
                    {
                        if (pointer.PointerNeedleType == PointerNeedleType.Needle)
                        {
                            hide = false;
                            break;
                        }
                    }
                }
            }

            if (hide)
            {
                this.PointerCap.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.PointerCap.Visibility = Visibility.Visible;
            }
        }
        
        /// <summary>
        /// Refreshs scale path.
        /// </summary>
        internal void RefreshScalePath()
        {
            if (this.mscalePath != null && this.ScaleBarSize >= 0)
            {
                CircularGauge gauge = this.GaugeElementParent as CircularGauge;
                this.RefreshScaleFrame();
                Point centerPoint = new Point(this.Radius, this.Radius);
                bool isArcLarge = this.gapSweepAngle >= 180;
                double innerRadius = this.Radius - this.ScaleBarSize > 0 ? this.Radius - this.ScaleBarSize : 0;
                double outerRadius = this.Radius;
                
                if (this.gapSweepAngle == 360)
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

                    this.mscalePath.Data = pathGeometry;
                }
                else
                {
                    this.mpathData = new PathGeometry();
                    Point startPoint1 = this.ConvertToStageCoordinates(innerRadius, this.startAngle, centerPoint);
                    Point endPoint1 = this.ConvertToStageCoordinates(innerRadius, this.startAngle + this.gapSweepAngle, centerPoint);
                    Point startPoint2 = this.ConvertToStageCoordinates(outerRadius, this.startAngle, centerPoint);
                    Point endPoint2 = this.ConvertToStageCoordinates(outerRadius, this.startAngle + this.gapSweepAngle, centerPoint);

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = startPoint1;

                    ArcSegment arc = new ArcSegment();
                    arc.RotationAngle = 360;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    arc.Point = startPoint1;
                    arc.Size = new Size(innerRadius, innerRadius);
                    figure1.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.RotationAngle = 360;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    arc.Point = endPoint1;
                    arc.Size = new Size(innerRadius, innerRadius);
                    figure1.Segments.Add(arc);

                    LineSegment line = new LineSegment();
                    line.Point = endPoint2;
                    figure1.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.RotationAngle = 360;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = SweepDirection.Counterclockwise;
                    arc.Point = endPoint2;
                    arc.Size = new Size(outerRadius, outerRadius);
                    figure1.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.RotationAngle = 360;
                    arc.IsLargeArc = isArcLarge;
                    arc.SweepDirection = SweepDirection.Counterclockwise;
                    arc.Point = startPoint2;
                    arc.Size = new Size(outerRadius, outerRadius);
                    figure1.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = startPoint1;
                    figure1.Segments.Add(line);

                    (this.mpathData as PathGeometry).Figures.Add(figure1);
                    this.mscalePath.Data = this.mpathData;
                }
            }

            this.RefreshScale();
        }
                
        #endregion
    }
}
