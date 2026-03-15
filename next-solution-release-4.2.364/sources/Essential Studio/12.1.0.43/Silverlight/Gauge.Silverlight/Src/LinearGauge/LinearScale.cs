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
    using System.Windows.Data;

    /// <summary>
    /// Represents scale visual element.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C#:</term></listheader>
    /// <item>
    /// <description> LinearScale scale = new LinearScale(); 
    /// <para></para>
    /// <para>                 scale.Minimum = 0; </para>
    /// <para></para>
    /// <para>                 scale.Maximum = 100; </para>
    /// <para></para>
    /// <para>                 scale.MinorIntervalValue = 2; </para>
    /// <para></para>
    /// <para>                 scale.MajorIntervalValue = 10; </para>
    /// <para></para>
    /// <para>                 scale.scaleBarSize = 10; </para>
    /// <para></para>
    /// <para>                 scale.scaleBarLength = 260; </para>
    /// <para></para>
    /// <para>                 scale.Background = new SolidColorBrush( Colors.Orange ); </para>
    /// <para></para>
    /// <para>                 lineargauge.Scales.Add( scale ); </para></description></item></list>
    /// <para>  </para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:LinearGauge.Scales&gt; 
    /// <para>            &lt;syncfusion:LinearScale Name=&quot;LinearScale&quot; </para>
    /// <para>                          Minimum=&quot;0&quot; </para>
    /// <para>                          Maximum=&quot;100&quot; </para>
    /// <para>                          MinorIntervalValue=&quot;2&quot; </para>
    /// <para>                          MajorIntervalValue=&quot;10&quot; </para>
    /// <para>                          scaleBarSize = &quot;10&quot; </para>
    /// <para>                          scaleBarLength=&quot;260&quot; /&gt;</para>
    /// <para>&lt;/syncfusion:LinearGauge.Scales&gt; </para></description></item></list>
    /// <para></para>
    /// <para> </para>
    /// </example>
    [ContentProperty("Pointers")]
    public class LinearScale : ScaleBase
    {
        public static readonly DependencyProperty DisableIntersectTicksProperty =
         DependencyProperty.Register("DisableIntersectTicks", typeof(bool), typeof(LinearScale), new PropertyMetadata(false, new PropertyChangedCallback(OnDisableIntersectTicksChanged)));

        /// <summary>
        /// Gets or sets the DisableIntersectTicks.
        /// </summary>
        public bool DisableIntersectTicks
        {
            set { SetValue(DisableIntersectTicksProperty, value); }
            get { return (bool)GetValue(DisableIntersectTicksProperty); }
        }

        private static void OnDisableIntersectTicksChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            LinearScale scale = (LinearScale)obj;
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
        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.RadiusX">RadiusX</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(LinearScale), new PropertyMetadata(new PropertyChangedCallback(OnRadiusXChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.RadiusY">RadiusY</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(LinearScale), new PropertyMetadata(new PropertyChangedCallback(OnRadiusYChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.scaleBarLength">scaleBarLength</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty OuterBackgroundProperty =
            DependencyProperty.Register("OuterBackground", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.scaleBarLength">scaleBarLength</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty InnerBackgroundProperty =
            DependencyProperty.Register("InnerBackground", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.scaleBarLength">scaleBarLength</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(LinearScale), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.scaleBarLength">scaleBarLength</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty ScaleBarLengthProperty =
            DependencyProperty.Register("ScaleBarLength", typeof(double), typeof(LinearScale), new PropertyMetadata(0d, new PropertyChangedCallback(OnscaleBarLengthChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearScale.Orientation">Orientation</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default Orientation is <see cref="F:Syncfusion.Windows.Gauge.GaugeOrientation.Vertical">Vertical</see></para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.GaugeOrientation">GaugeOrientation</see></para>
        /// </returns>
        protected internal static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(GaugeOrientation), typeof(LinearScale), new PropertyMetadata(GaugeOrientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

       public static readonly DependencyProperty ScaleTypeProperty =
           DependencyProperty.Register("ScaleType", typeof(ScaleTypes), typeof(LinearScale), new PropertyMetadata(ScaleTypes.Linear, new PropertyChangedCallback(OnScaleTypeChanged)));

        #endregion

        #region Private members
        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double mlengthRatio;

        /// <summary>
        /// Collection of the linear pointers.
        /// </summary>
        private LinearPointersCollection mpointers;

        /// <summary>
        /// The path used to draw the scale.
        /// </summary>
        private Path mOuterscalePath, mInnerscalePath, mGlassPath;        

        /// <summary>
        /// Linaer scale panel
        /// </summary>
        private LinearScaleLayoutPanel mscalesPanel;

        private bool setMajorInterval = true, setMinorInterval=true;

        /// <summary>
        /// The width of the scale 
        /// </summary>
        private double mscaleWidth,mscalebarsize,mscalebarlength;
        private static double lflag = 0;
        private bool indicator = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LinearScale">LinearScale</see> class.
        /// </summary>
        public LinearScale()
        {
            DefaultStyleKey = typeof(LinearScale);

            this.Loaded += new RoutedEventHandler(this.LinearScaleLoaded);

            this.Ticks = new TicksCollection();
            this.Ranges = new RangesCollection();
            this.Pointers = new LinearPointersCollection();
            indicator = false;
            this.Ticks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.Ranges.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.Pointers.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearScale.RadiusX">RadiusX</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusXChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearScale.RadiusY">RadiusY</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusYChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearScale.scaleBarLength">scaleBarLength</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScaleBarLengthChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearScale.scaleBarLength">scaleBarLength</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearScale.Orientation">Orientation</see> property is changed.
        /// </summary>
        protected internal event PropertyChangedCallback OrientationChanged;

        protected internal event PropertyChangedCallback ScaleTypeChanged;
        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets the collection of linear pointers.
        /// </summary>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.LinearPointersCollection">LinearPointersCollection</see>
        /// </value>
        /// <seealso cref="LinearPointersCollection">LinearPointersCollection</seealso>
        public LinearPointersCollection Pointers
        {
            get
            {
                return this.mpointers;
            }

            set
            {
                this.mpointers = value;
            }
        }

        /// <summary>
        /// Gets or sets the x radius
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        public double RadiusX
        {
            get
            {
                return (double)GetValue(RadiusXProperty);
            }

            set
            {
                SetValue(RadiusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the y radius
        /// </summary>
        /// /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        public double RadiusY
        {
            get
            {
                return (double)GetValue(RadiusYProperty);
            }

            set
            {
                SetValue(RadiusYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the scale. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double ScaleBarLength
        {
            get
            {
                return (double)GetValue(ScaleBarLengthProperty);
            }

            set
            {
                SetValue(ScaleBarLengthProperty, value);
            }
        }
        
        
        internal double scaleBarLength
        {
            get
            {
                return this.mscalebarlength;
            }

            set
            {
                this.mscalebarlength = value;
            }
        }

        double frameOffset = 10;
        internal double FrameOffset
        {
            get
            {
                return frameOffset;
            }
            set
            {
                frameOffset = value;
            }
        }

        /// <summary>
        /// Gets or Sets the OuterBackground
        /// </summary>
        public Brush OuterBackground
        {
            get
            {
                return (Brush)GetValue(OuterBackgroundProperty);
            }

            set
            {
                SetValue(OuterBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the InnerBackground
        /// </summary>
        public Brush InnerBackground
        {
            get
            {
                return (Brush)GetValue(InnerBackgroundProperty);
            }

            set
            {
                SetValue(InnerBackgroundProperty, value);
            }
        }

        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }

            set
            {
                SetValue(OffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets the Scalepanel
        /// </summary>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.LinearScaleLayoutPanel">LinearScaleLayoutPanel</see>
        /// </value>
        internal LinearScaleLayoutPanel PART_ScalesPanel
        {
            get
            {
                return this.mscalesPanel;
            }
        }

        /// <summary>
        /// Gets or sets the length betweeen the scale size and gauge size.
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        internal double ScaleWidth
        {
            get
            {
                return this.mscaleWidth;
            }

            set
            {
                this.mscaleWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the length betweeen the scale size and gauge size.
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        internal double LengthRatio
        {
            get
            {
                return this.mlengthRatio;
            }

            set
            {
                this.mlengthRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the scale.
        /// </summary>
        /// <remarks>
        /// Default value is <see cref="F:Syncfusion.Windows.Gauge.GaugeOrientation.Vertical">GaugeOrientation.Vertical</see>.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:Syncfusion.Windows.Gauge.GaugeOrientation">GaugeOrientation</see>
        /// </value>
        /// <seealso cref="GaugeOrientation">GaugeOrientation</seealso>
        protected internal GaugeOrientation Orientation
        {
            get
            {
                return (GaugeOrientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the ScaleType
        /// </summary>
        public ScaleTypes ScaleType
        {
            get
            {
                return (ScaleTypes)GetValue(ScaleTypeProperty);
            }

            set
            {
                SetValue(ScaleTypeProperty, value);
            }
        }

        #endregion

        #region Overrides
        #endregion

       internal double scaleBarSize
        {
            get
            {
                return this.mscalebarsize;
            }

            set
            {
                this.mscalebarsize = value;
            }
        }

        #region Implementation
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.mscalesPanel = this.GetTemplateChild("PART_ScalesPanel") as LinearScaleLayoutPanel;
            if (this.mscalesPanel != null)
            {
                this.Ticks.VisualParent = this.mscalesPanel;
                this.Pointers.VisualParent = this.mscalesPanel;
                this.Ranges.VisualParent = this.mscalesPanel;
            }

            this.mOuterscalePath = this.GetTemplateChild("PART_OuterPathRect") as Path;
            this.mInnerscalePath = this.GetTemplateChild("PART_InnerPathRect") as Path;
            this.mGlassPath = this.GetTemplateChild("PART_GlassPath") as Path;
            if (this.Orientation == GaugeOrientation.Horizontal)
            {
                this.scaleBarLength = this.ScaleBarSize;
                this.scaleBarSize = this.ScaleBarLength;
            }
            else
            {
                this.scaleBarLength = this.ScaleBarLength;
                this.scaleBarSize = this.ScaleBarSize;
            }
            this.UpdateVisualStyle();
            this.RefreshScalePath();
            this.ClearScaleLabelTicks();
            this.ClearScaleTicks();
            this.AddTickSet();
            this.AddLabelTickSet();
            indicator = false;
            base.OnApplyTemplate();
        }

        internal NumberFormatInfo GetNumberFormatInfo(double labelValue, LabelTickSet labelTick)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            int trailDigit = this.GetMaximumDecimalDigits(this.MajorIntervalValue);
            if (labelTick.LabelFormat == null)
            {
                format.NumberDecimalDigits = labelTick.IsLogarithmic ? labelTick.GetLogDecimalCountNumber(Math.Log(labelValue, labelTick.LogBase)) : labelTick.GetDecimalCountNumber(labelValue,trailDigit);
            }
            else
            {
                format = labelTick.LabelFormat.Clone() as NumberFormatInfo;
                if (labelTick.RemoveTrailingZero)
                {
                    format.NumberDecimalDigits = labelTick.GetDecimalCountNumber(labelValue,labelTick.LabelFormat.NumberDecimalDigits);                    
                }
            }
            return format;
        }

        int GetMaximumDecimalDigits(double val)
        {
            string str = val.ToString();
            int dot=str.IndexOf('.');
            int n = dot!=-1?str.Length - dot - 1:3;
            return n>3?n:3;
        }

        /// <summary>
        /// Gets the format according to the value.
        /// </summary>
        /// <param name="labelValue">Value for the Label</param>
        /// <param name="labelTick">Value for the LableTick</param>
        /// <returns>Returns formatted value.</returns>
        internal string GetFormatedValue(double labelValue, LabelTickSet labelTick)
        {
            double value=labelValue;
            double labelformulaValue = 0;
            NumberFormatInfo format = GetNumberFormatInfo(labelValue, labelTick);            
            if (labelTick.EnableFormulaCalculation == true)
            {
                labelformulaValue = labelTick.CalculateLabelValue(labelValue, labelTick.LabelFormula);
                if (labelformulaValue == -1)
                {
                    throw new Exception("Parentheses Error.Please enter a valid formula");
                }
                else if (labelformulaValue == -2)
                {
                    throw new Exception("x variable missing.Please enter a valid formula");
                }
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
            if (trailDigits != -1 && format.NumberDecimalDigits != trailDigits)
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
            return Convert.ToDouble(this.GetFormatedValue(labelValue, labelTick), format);
        }

        /// <summary>
        /// Gets the position according to the value.
        /// </summary>
        /// <param name="value">value for getting position</param>
        /// <returns>Calculated position value</returns>
        internal double GetPositionByValue(double value)
        {
            double diff = this.FrameOffset / 2;
            double ratio;
            double scaleWidth=this.ScaleBarLength-this.frameOffset;
            if (this.IsReversed)
            {
                ratio = (this.Maximum - this.Minimum) / (this.Maximum-value);
            }
            else
            {
                ratio = (this.Maximum - this.Minimum) / (value - this.Minimum);
            }

            if (value == this.Minimum)
            {
                if (this.IsReversed)
                {
                    return this.ScaleBarLength - diff;
                }
                else
                {
                    return diff;
                }
            }
            else if (ratio != 0 && value!=this.Maximum)
            {
                if (this.IsReversed)
                {
                    return (scaleWidth / ratio)+diff;
                }
                return (scaleWidth / ratio)+diff;
            }
            else
            {
                if (this.IsReversed)
                    return diff;
                else
                    return this.ScaleBarLength - diff;
            }
        }

        /// <summary>
        /// Gets the position according to the value.
        /// </summary>
        /// <param name="value">value for getting position</param>
        /// <returns>Calculated position value</returns>
        internal double GetPointerPositionByValue(double value)
        {
            double diff = this.FrameOffset / 2;
            double ratio;
            double scaleWidth = this.ScaleBarLength - this.frameOffset;
            ratio = (this.Maximum - this.Minimum) / (value - this.Minimum);
            
            if (value == this.Minimum)
            {
                return diff;
            }
            else if (ratio != 0 && value != this.Maximum)
            {
                return (scaleWidth / ratio) + diff;
            }
            else
            {
                return this.ScaleBarLength - diff;
            }
        }
   
   
        /// <summary>
        /// Refreshes pointers position according to the values.
        /// </summary>
        internal void RefreshPointers()
        {
            foreach (LinearPointer pointer in this.Pointers)
            {
                if (pointer.Value < this.Minimum)
                {
                    pointer.Value = this.Minimum;
                }

                if (pointer.Value > this.Maximum)
                {
                    pointer.Value = this.Maximum;
                }

                pointer.Position = (pointer is LinearMarkerPointer)?this.GetPositionByValue(pointer.Value):this.GetPointerPositionByValue(pointer.Value);
                pointer.RefreshPointerPosition();
                
            }
        }

        /// <summary>
        /// Refreshes the label tickset
        /// </summary>
        protected internal override void RefreshLabelTickSet()
        {
            string value;
            double scaleLength = this.ScaleBarLength - this.FrameOffset;
            if (this.mscalesPanel != null)
            {
               
                double valueInterval = 0;
                foreach (TickSetBase tickSet in this.Ticks)
                {
                    double i = 0;
                    double labelValue = this.Minimum;
                    if (tickSet is LabelTickSet)
                    {
                        LabelTickSet labelTick = tickSet as LabelTickSet;
                        if (labelTick.TickStyle == TickStyle.MajorTick)
                        {
                            valueInterval = this.MajorIntervalValue;
                        }
                        else if (labelTick.TickStyle == TickStyle.MidTick)
                        {
                            valueInterval = this.MidIntervalValue;
                        }
                        else
                        {
                            valueInterval = this.MinorIntervalValue;
                        }

                        double ratio = (this.Maximum - this.Minimum) / valueInterval;
                        double termotemp = 0;
                       double interval = (scaleLength -termotemp)/ ratio;
                        if (interval > 0)
                        {   
                            value = this.GetFormatedValue(labelValue, labelTick);
                            value = labelTick.Prefix + value + labelTick.Suffix;
                            Size size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                            if (this.IsReversed)
                            {
                                i = (scaleLength) + (size.Height / 2);
                                
                            }
                            else
                            {
                                i = size.Height / 2;
                            }

                            if (!labelTick.IncludeFirstValue)
                            {
                                labelValue += valueInterval;
                                i += interval;
                            }

                            foreach (FrameworkElement elem in this.mscalesPanel.Children)
                            {
                                if (elem is LabelTickSet)
                                {
                                    LabelTickSet tick = elem as LabelTickSet;
                                    //tick.Visibility = Visibility.Visible;
                                    if (tick.GaugeElementParent == labelTick && (labelValue<this.Maximum+valueInterval))                     
                                    {
                                        value = this.GetFormatedValue(labelValue, labelTick);
                                        if (GetFormatedDoubleValue(labelValue,labelTick) == this.Maximum)
                                        {
                                            value = this.GetFormatedValue(this.Maximum, labelTick);
                                        }
                                        value = labelTick.Prefix + value + labelTick.Suffix;
                                        size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                        double length = size.Width;
                                        size = this.GetTextSize(this.Maximum.ToString(), labelTick.FontFamily, labelTick.FontSize);
                                        length = size.Width;

                                        if (this.Orientation == GaugeOrientation.Horizontal)
                                        {
                                            length = size.Height;
                                        }
                                        size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                        double posX = 0;
                                        if (labelTick.TickPlacement == ScalePlacement.Cross)
                                        {
                                            posX = (-length / 2) - labelTick.DistanceFromScale;
                                        }
                                        else if (labelTick.TickPlacement == ScalePlacement.Outside)
                                        {
                                            posX = (-this.ScaleBarSize / 2) - length - labelTick.DistanceFromScale;
                                        }
                                        else if (labelTick.TickPlacement == ScalePlacement.Inside)
                                        {
                                            posX = (this.ScaleBarSize / 2) + labelTick.DistanceFromScale;
                                        }

                                        double angleAdjust = 0;
                                        if (this.Orientation == GaugeOrientation.Horizontal)
                                        {
                                            posX += (size.Height);
                                        }

                                        RotateTransform transform1 = new RotateTransform();
                                        transform1.Angle = labelTick.Angle + angleAdjust;
                                        transform1.CenterX = size.Width / 2;
                                        transform1.CenterY = size.Height / 2;
                                        if (labelTick.ShowToolTip)
                                        {
                                            tick.tooltip = value;
                                        }
                                        else
                                        {
                                            tick.tooltip = null;
                                        }
                                        if (labelValue >= this.Maximum)
                                        {
                                            if (this.IsReversed)
                                            {
                                                i = size.Height / 2;
                                            }
                                            else
                                            {
                                                i = (scaleLength) + (size.Height / 2);
                                            }
                                        }
                                        if (this.Orientation == GaugeOrientation.Vertical)
                                        {                                           
                                            tick.XOffset = posX;
                                            tick.YOffset = (scaleLength / 2) - i - termotemp;
                                        }
                                        else
                                        {
                                            tick.XOffset = -(scaleLength / 2) + i - termotemp - (size.Width / 2 + length / 2);
                                            tick.YOffset = -posX;
                                        }
                                        tick.RenderTransform = transform1;
                                        if (labelTick.flag == 0 || labelTick.flag == 4)
                                            tick.LabelForeground = labelTick.LabelForeground;
                                        tick.BorderBrush = labelTick.BorderBrush;
                                        tick.BorderThickness = labelTick.BorderThickness;
                                        tick.FontSize = labelTick.FontSize;
                                        
                                        tick.Text = value;
                                        
                                        if (this.IsReversed)
                                        {
                                            i -= interval;
                                        }
                                        else
                                        {
                                            i += interval;
                                        }

                                        if (labelValue == this.Maximum)
                                        {
                                            break;
                                        }
                                        labelValue += valueInterval;
                                        if (labelValue >= this.Maximum)
                                        {
                                            labelValue = this.Maximum;
                                        }
                                        labelValue = Convert.ToDouble((decimal)(labelValue));
                                    }                                    
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the label tick
        /// </summary>
        protected internal override void RefreshLabelTick()
        {
            double labelformulaValue = 0;
            string value;
            double labelcount = 0;
            double labeltickcount = 0;
            double scaleLength = this.ScaleBarLength - this.FrameOffset;
            if (this.mscalesPanel != null)
            {
                if (indicator)
                {
                    if (lflag < this.scaleBarLength)
                    {
                        return;
                    }
                }
                double valueInterval = 0;
                foreach (TickSetBase tickSet in this.Ticks)
                {
                    double i = 0;
                    double labelValue = this.Minimum;

                    if (tickSet is LabelTickSet)
                    {
                        LabelTickSet labelTick = tickSet as LabelTickSet;
                        if (labelTick.TickStyle == TickStyle.MajorTick)
                        {
                            valueInterval = this.MajorIntervalValue;
                        }
                        else
                        {
                            valueInterval = this.MinorIntervalValue;
                        }

                        labelcount++;
                        double ratio = (this.Maximum - this.Minimum) / valueInterval;
                        double termotemp = 0;
                        double interval = scaleLength - termotemp / ratio;
                        if ((labelTick.EnableFormulaCalculation == true && labelTick.Validate == 0) || labelTick.IsLogarithmic)
                        {
                            foreach (FrameworkElement elem in this.mscalesPanel.Children)
                            {
                               if (elem is LabelTickSet)
                                {
                                    labeltickcount++;
                                    if ((labeltickcount >= ((labelcount - 1) * (ratio + 1)) + 1) && labeltickcount <= labelcount * (ratio + 1))
                                    {
                                        LabelTickSet tick = elem as LabelTickSet;
                                        tick.Visibility = Visibility.Visible;      
                                        if (labelValue <= this.Maximum || (labelValue>this.Maximum && (labelValue-this.Maximum) < valueInterval))
                                        {
                                            NumberFormatInfo format = new NumberFormatInfo();

                                            if (labelTick.LabelFormat == null)
                                            {
                                                format.NumberDecimalDigits = labelTick.IsLogarithmic ? labelTick.GetLogDecimalCountNumber(Math.Log(labelValue, 10)) : labelTick.GetDecimalCountNumber(labelValue);
                                            }
                                            else
                                            {
                                                format = labelTick.LabelFormat;
                                            } 
                                            if (labelTick.EnableFormulaCalculation == true && labelTick.Validate == 0)
                                            {
                                                labelformulaValue = labelTick.CalculateLabelValue(labelValue, labelTick.LabelFormula);

                                                if (labelTick.IsLogarithmic)
                                                {
                                                    value = labelTick.Prefix + Math.Log(labelformulaValue, labelTick.LogBase).ToString("N", format) + labelTick.Suffix;
                                                }
                                                else
                                                {
                                                    value = labelTick.Prefix + labelformulaValue.ToString("N", format) + labelTick.Suffix;
                                                }
                                            }
                                            else
                                            {
                                                if (labelTick.IsLogarithmic)
                                                {
                                                    value = labelTick.Prefix + Math.Log(labelValue, labelTick.LogBase).ToString("N", format) + labelTick.Suffix;
                                                }
                                                else
                                                {
                                                    value = labelTick.Prefix + labelValue.ToString("N", format) + labelTick.Suffix;
                                                }
                                            }

                                            Size size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                            if (i == 0)
                                            {
                                                i = size.Height / 2;
                                            }

                                            if (labelValue > this.Maximum && (labelValue - this.Maximum) < valueInterval)
                                            {
                                                labelValue = this.Maximum;
                                                i = scaleLength + (size.Height / 2);
                                            }

                                            size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                            double length = size.Width;
                                            size = this.GetTextSize(this.Maximum.ToString(), labelTick.FontFamily, labelTick.FontSize);
                                            length = size.Width;

                                            if (this.Orientation == GaugeOrientation.Horizontal)
                                            {
                                              //  length = size.Height;
                                            }
                                            size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                            double posX = 0;
                                            if (labelTick.TickPlacement == ScalePlacement.Cross)
                                            {
                                                posX = (-length / 2) - labelTick.DistanceFromScale;
                                            }
                                            else if (labelTick.TickPlacement == ScalePlacement.Inside)
                                            {
                                                posX = ((-this.scaleBarSize / 2) - length) - labelTick.DistanceFromScale;
                                            }
                                            else if (labelTick.TickPlacement == ScalePlacement.Outside)
                                            {
                                                posX = (this.scaleBarSize / 2) + labelTick.DistanceFromScale;
                                            }

                                            double angleAdjust = 0;
                                            if (this.Orientation == GaugeOrientation.Horizontal)
                                            {
                                             //   angleAdjust = -90;
                                            //    posX -= (size.Width / 2) - (size.Height / 2);
                                            }

                                            if (labelTick.ShowToolTip)
                                            {
                                                tick.tooltip = value;
                                            }
                                            else
                                            {
                                                tick.tooltip = null;
                                            }

                                            RotateTransform transform1 = new RotateTransform();
                                            transform1.Angle = labelTick.Angle + angleAdjust;
                                            transform1.CenterX = size.Width / 2;
                                            transform1.CenterY = size.Height / 2;
                                            tick.XOffset = posX;
                                            if (i > this.scaleBarLength-termotemp)
                                            {
                                                i = this.scaleBarLength+(size.Height/2)-termotemp;
                                            }

                                            tick.YOffset = (this.scaleBarLength / 2) - i-termotemp;

                                            if (this.Orientation == GaugeOrientation.Vertical)
                                            {
                                                tick.XOffset = posX;
                                                tick.YOffset = (scaleLength / 2) - i - termotemp;
                                            }
                                            else
                                            {
                                                tick.XOffset = -(scaleLength / 2) + i - termotemp - (size.Width);
                                                tick.YOffset = -posX;
                                            }
                                            tick.Text = value;
                                            tick.RenderTransform = transform1;
                                            if (labelTick.flag == 0 || labelTick.flag == 4)
                                                tick.LabelForeground = labelTick.LabelForeground;
                                            tick.FontSize = labelTick.FontSize;
                                            tick.FontSize = labelTick.FontSize;
                                            tick.BorderBrush = labelTick.BorderBrush;
                                            tick.BorderThickness = labelTick.BorderThickness;
                                            i += interval;
                                            labelValue += valueInterval;
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
        /// Refreshes Scale
        /// </summary>
        protected internal override void RefreshScale()
        {
            if (this.mscalesPanel != null)
            {
                if (indicator)
                {
                    if (lflag < this.scaleBarLength)
                    {
                        return;
                    }
                }
                this.mscalesPanel.InvalidateMeasure();
                this.mscalesPanel.InvalidateArrange();
                foreach (FrameworkElement elem in this.mscalesPanel.Children)
                {
                    if (elem is LinearRange)
                    {
                        (elem as LinearRange).RefreshRange();
                    }
                }
            }
        }
               
        /// <summary>a
        /// Refreshes tickset
        /// </summary>
        protected internal override void RefreshTickSet()
        {
            int flag = 0;
            double scaleLength = this.ScaleBarLength - this.FrameOffset;
            if (this.mscalesPanel != null)
            {
                if (indicator)
                {
                    if (lflag < this.scaleBarLength)
                    {
                        return;
                    }
                }
                foreach (TickSetBase tickSet in this.Ticks)
                {
                    if (tickSet is MarkTickSet)
                    {
                        double i = 0;
                        double valueInterval = 0;
                        MarkTickSet markTick = tickSet as MarkTickSet;
                        if (markTick.TickStyle == TickStyle.MajorTick)
                        {
                            valueInterval = this.MajorIntervalValue;
                            flag = markTick.majorflag;
                        }
                        else if (markTick.TickStyle == TickStyle.MidTick)
                        {
                            valueInterval = this.MidIntervalValue;
                            flag = markTick.middleflag;
                        }
                        else
                        {
                            valueInterval = this.MinorIntervalValue;
                            flag = markTick.minorflag;
                        }

                        double ratio = (this.Maximum - this.Minimum) / valueInterval;
                        double termotemp = 0;
                        double interval = (scaleLength - termotemp) / ratio;
                        if (interval > 0)
                        {
                            if (this.IsReversed)
                            {
                                i = scaleLength - (markTick.TickWidth / 2);
                            }
                            else
                            {
                                i = -markTick.TickWidth / 2;
                            }

                            foreach (FrameworkElement elem in this.mscalesPanel.Children)
                            {
                                if (elem is MarkTickSet)
                                {
                                    MarkTickSet tick = elem as MarkTickSet;
                                    if (tick.GaugeElementParent == markTick)
                                    {
                                        double posX = 0;
                                        if (markTick.TickPlacement == ScalePlacement.Cross)
                                        {
                                            posX = -markTick.DistanceFromScale + (markTick.TickHeight / 2);
                                        }
                                        else if (markTick.TickPlacement == ScalePlacement.Inside)
                                        {
                                            posX = (-this.ScaleBarSize/2 - (markTick.TickHeight / 2)) - markTick.DistanceFromScale;
                                        }
                                        else if (markTick.TickPlacement == ScalePlacement.Outside)
                                        {
                                            posX = this.ScaleBarSize + markTick.DistanceFromScale+markTick.TickHeight / 2;
                                        }

                                        if (markTick.ShowToolTip)
                                        {
                                            tick.ticktooltip = i.ToString();
                                        }
                                        else
                                        {
                                            tick.ticktooltip = null;
                                        }

                                        #region DisableInterScets
                                        if ((markTick.TickStyle == TickStyle.MinorTick || markTick.TickStyle == TickStyle.MidTick) && this.DisableIntersectTicks == true)
                                        {
                                            foreach (TickSetBase temp_tick in this.Ticks)
                                            {
                                                MarkTickSet temp_marktickset = temp_tick as MarkTickSet;
                                                if (temp_marktickset != null)
                                                {
                                                    if (temp_marktickset.TickStyle == TickStyle.MajorTick)
                                                    {
                                                        double ratio1 = (this.Maximum - this.Minimum) / this.MajorIntervalValue;
                                                        double interval1 = scaleLength / ratio1;

                                                        double check;
                                                        if (i < (interval1 + (temp_marktickset.TickWidth / 2)))
                                                        {
                                                            check = i - (markTick.TickWidth / 2);
                                                        }
                                                        else
                                                        {
                                                            check = (i - (markTick.TickWidth / 2)) % interval1;
                                                        }
                                                        double behind = interval1 - check;

                                                        if ((check <= (temp_marktickset.TickWidth / 2)) || (behind <= (temp_marktickset.TickWidth / 2)))
                                                        {
                                                            if (this.IsReversed)
                                                            {
                                                                i -= interval;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                i += interval;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        double rotangle = 0;
                                        if (this.Orientation == GaugeOrientation.Vertical)
                                        {
                                            tick.XOffset = posX;
                                            if (i > scaleLength && this.ShowLastValue)
                                            {
                                                i = scaleLength + (markTick.TickWidth / 2);
                                            }
                                            else
                                            {
                                                tick.YOffset = (scaleLength / 2) - i - termotemp;
                                            }
                                            rotangle = 0;
                                        }
                                        else
                                        {
                                            tick.YOffset = posX;
                                            if (i > scaleLength && this.ShowLastValue)
                                            {
                                                i = scaleLength - (markTick.TickHeight / 2);
                                                tick.XOffset =  i/2 - termotemp;
                                            }
                                            else
                                            {
                                                tick.XOffset = -(scaleLength / 2) + i - termotemp;
                                            }
                                            rotangle = 90;
                                        }

                                        RotateTransform transform1 = new RotateTransform();
                                        transform1.Angle = markTick.Angle+rotangle;                                        
                                        tick.RenderTransform = transform1;
                                        tick.RenderTransformOrigin = new Point(0.5, 0.5);
                                        if (flag == 0 || flag == 4)
                                        {
                                            tick.TickBackground = markTick.TickBackground;
                                        }
                                        tick.TickWidth = markTick.TickWidth;
                                        tick.TickHeight = markTick.TickHeight;
                                        tick.TickStyle = markTick.TickStyle;
                                        tick.TickShape = markTick.TickShape;
                                        tick.TickPlacement = markTick.TickPlacement;
                                        tick.BorderBrush = markTick.BorderBrush;
                                        tick.BorderThickness = markTick.BorderThickness;
                                        tick.RefreshTick();
                                        if (this.IsReversed)
                                        {
                                            i -= interval;
                                        }
                                        else
                                        {
                                            i += interval;
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
        /// Updates property value cache and raises scaleBarSizeChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnScaleBarSizeChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.Orientation == GaugeOrientation.Horizontal)
            {
                this.scaleBarLength = this.ScaleBarSize;
                this.scaleBarSize = this.ScaleBarLength;
            }
            else
            {
                this.scaleBarLength = this.ScaleBarLength;
                this.scaleBarSize = this.ScaleBarSize;
            }
            indicator = false;
            base.OnScaleBarSizeChanged(e);
            this.RefreshScalePath();
            this.RefreshTickSet();
            this.RefreshLabelTickSet();
            foreach (LinearPointer pointer in this.Pointers)
            {
                pointer.RefreshPointerPosition();
            }
        }

        /// <summary>
        /// Updates property value cache and raises scaleBarLengthChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleBarLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Orientation == GaugeOrientation.Horizontal)
            {
                this.scaleBarLength = this.ScaleBarSize;
                this.scaleBarSize = this.ScaleBarLength;
            }
            else
            {
                this.scaleBarLength = this.ScaleBarLength;
                this.scaleBarSize = this.ScaleBarSize;
            }
            if (this.ScaleBarLengthChanged != null)
            {
                this.ScaleBarLengthChanged(this, e);
            }
        }

        protected virtual void OnOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.OffsetChanged != null)
            {
                this.OffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises RadiusYChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusYChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.RefreshScalePath();
            if (this.RadiusYChanged != null)
            {
                this.RadiusYChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises RadiusXChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusXChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.RefreshScalePath();
            if (this.RadiusXChanged != null)
            {
                this.RadiusXChanged(this, e);
            }
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="MinorIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMinorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            setMinorInterval = false;
            this.MinorTicks = (int)(this.MajorIntervalValue / this.MinorIntervalValue);
            setMinorInterval = true;
            this.AddTickSet();
            this.AddLabelTickSet();

            base.OnMinorIntervalValueChanged(e);
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="MajorIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMajorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            if ((this.Maximum - this.Minimum) % this.MajorIntervalValue != 0)
            {
                setMajorInterval = false;
            }

            this.MajorTicks = (int)((this.Maximum - this.Minimum) / this.MajorIntervalValue);
            setMajorInterval = true;
            this.AddTickSet();
            this.AddLabelTickSet();
            base.OnMajorIntervalValueChanged(e);

        }

        /// <summary>
        //  /// Updates property value cache and raises <see cref="MidIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMidIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.MidTicks = (int)(this.MajorIntervalValue / this.MidIntervalValue);
            this.AddTickSet();
            this.AddLabelTickSet();

            base.OnMidIntervalValueChanged(e);
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="MajorTicksChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMajorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            if (setMajorInterval)
            {
                this.MajorIntervalValue = (this.Maximum - this.Minimum) / this.MajorTicks;
            }

            this.MidIntervalValue = this.MajorIntervalValue / this.MidTicks;
      // Condition added to resolve the MinorIntervalValue calculation error with minor tick default value            
        if(this.MinorTicks!=0 && this.MinorTicks !=5)
                this.MinorIntervalValue = this.MajorIntervalValue / this.MinorTicks;

            this.AddTickSet();
            this.AddLabelTickSet();

            base.OnMajorTicksChanged(e);
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="MidTicksChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMidTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            this.MidIntervalValue = this.MajorIntervalValue / this.MidTicks;
            this.AddTickSet();
            this.AddLabelTickSet();

            base.OnMidTicksChanged(e);
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="MinorTicksChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnMinorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.ClearScaleTicks();
            this.ClearScaleLabelTicks();
            if(setMinorInterval)
            this.MinorIntervalValue = this.MajorIntervalValue / this.MinorTicks;
            this.AddTickSet();
            this.AddLabelTickSet();
            base.OnMinorTicksChanged(e);
        }

        /// <summary>
        // /// Invoked when <see cref="Maximum"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            base.OnMaximumChanged(e);
            if (this.Minimum > this.Maximum)
            {
                MessageBox.Show("Maximum value should be greater than Minimum value");
            }
            else
            {
                this.ClearScaleTicks();
                this.ClearScaleLabelTicks();
                this.AddTickSet();
                this.AddLabelTickSet();
                foreach (LinearPointer pointer in this.Pointers)
                {
                    if (pointer.Value > this.Maximum)
                    {
                        pointer.Value = this.Maximum;
                    }

                    pointer.Position = (pointer is LinearBarPointer)?this.GetPointerPositionByValue(pointer.Value):this.GetPositionByValue(pointer.Value);
                    pointer.RefreshPointerPosition();
                }
            }
        }

        /// <summary>
        // /// Invoked when <see cref="Minimum"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            base.OnMinimumChanged(e);
            if (this.Minimum > this.Maximum)
            {
                MessageBox.Show("Minimum value should be lesser than Maximum value");
            }
            else
            {
                this.ClearScaleTicks();
                this.ClearScaleLabelTicks();
                this.AddTickSet();
                this.AddLabelTickSet();
                foreach (LinearPointer pointer in this.Pointers)
                {
                    if (pointer.Value < this.Minimum)
                    {
                        pointer.Value = this.Minimum;
                    }

                    pointer.Position = (pointer is LinearBarPointer)?this.GetPointerPositionByValue(pointer.Value):this.GetPositionByValue(pointer.Value);
                    pointer.RefreshPointerPosition();
                }
            }
        }

        /// <summary>
        // /// Invoked when <see cref="ShowLastValue"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnShowLastValueChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnShowLastValueChanged(e);
            indicator = false;
                this.ClearScaleTicks();
                this.ClearScaleLabelTicks();
                this.AddTickSet();
                this.AddLabelTickSet();        
        }

        protected override void OnIsReversedChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            base.OnIsReversedChanged(e);
            this.RefreshScalePath();
            this.RefreshScale();
            this.RefreshPointers();
            this.RefreshLabelTickSet();
            this.RefreshTickSet();
           
        }
        /// <summary>
        /// Updates property value cache and raises OrientationChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Orientation == GaugeOrientation.Horizontal)
            {
                this.scaleBarLength = this.ScaleBarSize;
                this.scaleBarSize = this.ScaleBarLength;
            }
            else
            {
                this.scaleBarLength = this.ScaleBarLength;
                this.scaleBarSize = this.ScaleBarSize;
            }
            indicator = false;
            this.RefreshLabelTickSet();
            this.RefreshScale();
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        protected virtual void OnScaleTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            indicator = false;
            this.RefreshScalePath();
            this.RefreshPointers();

            this.RefreshLabelTickSet();
            this.RefreshTickSet();
           
            this.RefreshScale();
            if (this.ScaleTypeChanged != null)
            {
                this.ScaleTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnOrientationChanged(e);
        }

        private static void OnScaleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnScaleTypeChanged(e);
        }

        /// <summary>
        /// Calls OnRadiusXChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnRadiusXChanged(e);
        }
        
        /// <summary>
        /// Calls OnRadiusYChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnRadiusYChanged(e);
        }

        /// <summary>
        /// Calls OnscaleBarLengthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnscaleBarLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnScaleBarLengthChanged(e);
        }

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnOffsetChanged(e);
        }

        /// <summary>
        /// Invoked to refresh the scale path
        /// </summary>
        private void RefreshScalePath()
        {
            if (this.mOuterscalePath != null && this.ScaleBarSize >= 0)
            {
                RectangleGeometry rect = new RectangleGeometry();
                if (this.Orientation == GaugeOrientation.Vertical)
                    rect.Rect = new Rect(0, 0, this.scaleBarSize, this.scaleBarLength);
                else
                    rect.Rect = new Rect(0, 0, this.scaleBarSize, this.scaleBarLength);
                rect.RadiusX = this.RadiusX;
                rect.RadiusY = this.RadiusY;
                if (this.ScaleType == ScaleTypes.Thermometer && (!this.IsReversed))
                {
                    GeometryGroup gg = new GeometryGroup();
                    if (this.Orientation == GaugeOrientation.Vertical)
                        rect.Rect = new Rect(0, -this.Offset, this.scaleBarSize, this.scaleBarLength + 2 * this.Offset);
                    else
                        rect.Rect = new Rect(-this.Offset, 0, this.scaleBarSize + 2 * this.Offset, this.scaleBarLength);
                    gg.Children.Add(rect);
                    EllipseGeometry elli = new EllipseGeometry();
                    if (this.Orientation == GaugeOrientation.Vertical)
                        elli.Center = new Point(this.scaleBarSize / 2, this.scaleBarLength + this.scaleBarSize);
                    else
                        elli.Center = new Point(this.scaleBarLength + this.scaleBarSize, this.scaleBarSize / 2);
                    elli.RadiusX = elli.RadiusY = this.scaleBarSize;
                    gg.Children.Add(elli);
                    this.mOuterscalePath.Data = gg;
                    TranslateTransform transform = new TranslateTransform();
                    if (this.Orientation == GaugeOrientation.Vertical)
                    {
                        transform.X = (this.scaleBarSize - 2 * this.Offset) / 2;
                        transform.Y = (this.scaleBarSize);
                    }
                    else
                    {
                        transform.Y = (this.scaleBarSize - 2 * this.Offset) / 2;
                        transform.X = (this.scaleBarSize);
                    }
                    this.mOuterscalePath.RenderTransform = transform;
                }
                else
                {
                    this.mOuterscalePath.Data = rect;
                    TranslateTransform transform = new TranslateTransform();
                    if (this.Orientation == GaugeOrientation.Vertical)
                    {
                        transform.X = 0;
                        transform.Y = this.Offset / 2;
                    }
                    this.mOuterscalePath.RenderTransform = transform;

                }
            }

            if (this.mInnerscalePath != null && this.scaleBarSize >= 0)
            {
                RectangleGeometry rect = new RectangleGeometry();
                if (this.Orientation == GaugeOrientation.Vertical)
                    rect.Rect = new Rect(0, 0, this.scaleBarSize - 4 * this.Offset, this.scaleBarLength-this.frameOffset);
                else
                    rect.Rect = new Rect(0, 0, this.scaleBarSize - this.FrameOffset, this.scaleBarLength - this.FrameOffset);
                rect.RadiusX = this.RadiusX;
                rect.RadiusY = this.RadiusY;
                if (this.ScaleType == ScaleTypes.Thermometer && (!this.IsReversed))
                {
                    GeometryGroup gg = new GeometryGroup();
                    if (this.Orientation == GaugeOrientation.Vertical)
                        rect.Rect = new Rect(0, -this.Offset, this.scaleBarSize - 4 * this.Offset, this.scaleBarLength + 2 * this.Offset);
                    else
                        rect.Rect = new Rect(-this.Offset, 0, this.scaleBarSize + 2 * this.Offset, this.scaleBarLength - 2 * this.Offset);
                    gg.Children.Add(rect);
                    EllipseGeometry elli = new EllipseGeometry();
                    if (this.Orientation == GaugeOrientation.Vertical)
                        elli.Center = new Point((this.scaleBarSize - this.Offset) / 2, this.scaleBarLength - 2 * this.Offset + this.scaleBarSize);
                    else
                        elli.Center = new Point(this.scaleBarLength - 2 * this.Offset + this.scaleBarSize, (this.scaleBarSize - this.Offset) / 2);
                    elli.RadiusX = elli.RadiusY = this.scaleBarSize - this.Offset;
                    gg.Children.Add(elli);
                    this.mInnerscalePath.Data = gg;
                    TranslateTransform transform = new TranslateTransform();
                    if (this.Orientation == GaugeOrientation.Vertical)
                    {
                        transform.X = (this.scaleBarSize - 2 * this.Offset) / 2;
                        transform.Y = (this.scaleBarSize);
                    }
                    else
                    {
                        transform.Y = (this.scaleBarSize - 2 * this.Offset) / 2;
                        transform.X = (this.scaleBarSize);
                    }
                    this.mInnerscalePath.RenderTransform = transform;
                }
                else
                {
                    this.mInnerscalePath.Data = rect;
                    TranslateTransform transform = new TranslateTransform();
                    transform.X = 0;
                    transform.Y = this.Orientation==GaugeOrientation.Horizontal? this.Offset / 2 : 0;
                    this.mInnerscalePath.RenderTransform = transform;
                }
            }
            double tem = 0;
            if (this.GaugeElementParent is LinearGauge)
            {
                LinearGauge lgauge = this.GaugeElementParent as LinearGauge;
                if (this.mGlassPath != null)
                {
                    if (lgauge.EnableEffects)
                    {
                        this.mGlassPath.Visibility = Visibility.Visible;
                        this.mGlassPath.Height = this.Orientation==GaugeOrientation.Horizontal?0:this.scaleBarLength - this.Offset - this.RadiusX - this.RadiusY - tem;
                        this.mGlassPath.Margin = new Thickness(-this.scaleBarSize / 3, -tem / 2, 0, 0);
                        this.mGlassPath.Width = this.scaleBarSize / 10;
                    }
                    else
                    {
                        this.mGlassPath.Visibility = Visibility.Collapsed;
                    }
                }
            }
            this.RefreshScale();

        }

        /// <summary>
        /// Invoked to get the text size
        /// </summary>
        /// <param name="text">String whose size is to be measured</param>
        /// <param name="fontFamily">Fontfamily details of the measuring string</param>
        /// <param name="fontSize">Fontsize of the measuring string</param>
        /// <returns>Returns the string size</returns>
        private Size GetTextSize(string text, FontFamily fontFamily, double fontSize)
        {
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = text;
            txtBlock.FontFamily = fontFamily;
            txtBlock.FontSize = fontSize;
            return new Size(txtBlock.ActualWidth, txtBlock.ActualHeight);
        }

        /// <summary>
        /// Occurs when the scale is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void LinearScaleLoaded(object sender, RoutedEventArgs e)
        {
            this.FrameOffset = 10;
            this.RefreshPointers();
            this.RefreshScalePath();
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

            foreach (LinearPointer pointer in this.Pointers)
            {
                if (pointer.Value < this.Minimum)
                {
                    pointer.Value = this.Minimum;
                }

                if (pointer.Value > this.Maximum)
                {
                    pointer.Value = this.Maximum;
                }
            }
        }

        /// <summary>
        /// Invoked to clear the scale label ticks
        /// </summary>
        private void ClearScaleLabelTicks()
        {
            if (this.mscalesPanel != null)
            {
                int count = this.mscalesPanel.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < this.mscalesPanel.Children.Count && this.mscalesPanel.Children[i] is LabelTickSet)
                    {
                        LabelTickSet tick = this.mscalesPanel.Children[i] as LabelTickSet;
                        if (this.mscalesPanel.Children.Contains(tick))
                        {
                            this.mscalesPanel.Children.Remove(tick);
                            i--;
                            count = this.mscalesPanel.Children.Count;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked to clear the Tick sets
        /// </summary>
        private void ClearScaleTicks()
        {
            if (this.mscalesPanel != null)
            {
                int count = this.mscalesPanel.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < this.mscalesPanel.Children.Count && this.mscalesPanel.Children[i] is MarkTickSet)
                    {
                        MarkTickSet tick = this.mscalesPanel.Children[i] as MarkTickSet;
                        if (this.mscalesPanel.Children.Contains(tick))
                        {
                            this.mscalesPanel.Children.Remove(tick);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when element is added, replaced or removed from the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (GaugeElement elem in e.NewItems)
                {
                    if (this.PART_ScalesPanel != null)
                    {
                        this.PART_ScalesPanel.Children.Add(elem);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the Labeltickset is added.
        /// </summary>
        private void AddLabelTickSet()
        {
            double scaleLength = this.ScaleBarLength - this.FrameOffset;
            string value;
            double valueInterval = 0;
            if (this.mscalesPanel != null)
            {
                int count = this.mscalesPanel.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < this.mscalesPanel.Children.Count && this.mscalesPanel.Children[i] is LabelTickSet)
                    {
                        LabelTickSet tick = this.mscalesPanel.Children[i] as LabelTickSet;
                        if (this.mscalesPanel.Children.Contains(tick))
                        {
                            this.mscalesPanel.Children.Remove(tick);
                            i--;
                            count = this.mscalesPanel.Children.Count;
                        }
                    }
                }
            }

            foreach (TickSetBase tickSet in this.Ticks)
            {
                double i = 0;
                double labelValue = this.Minimum;
                if (tickSet is LabelTickSet)
                {
                    LabelTickSet labelTick = tickSet as LabelTickSet;
                    if (labelTick.TickStyle == TickStyle.MajorTick)
                    {
                        valueInterval = this.MajorIntervalValue;
                    }
                    else if (labelTick.TickStyle == TickStyle.MidTick)
                    {
                        valueInterval = this.MidIntervalValue;
                    }
                    else
                    {
                        valueInterval = this.MinorIntervalValue;
                    }

                    double ratio = (this.Maximum - this.Minimum) / valueInterval;
                    double termotemp = 0;
                    double interval = (scaleLength - termotemp) / ratio;
                    if (interval > 0)
                    {
                        value = this.GetFormatedValue(labelValue, labelTick);
                        value = labelTick.Prefix + value + labelTick.Suffix;

                        Size size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                        if (this.IsReversed)
                        {
                            i = scaleLength + (size.Height / 2);
                        }
                        else
                        {
                            i = size.Height / 2;
                        }
                        if (this.mscalesPanel != null)
                        {
                            if (this.mscalesPanel.Children.IndexOf(labelTick) != -1)
                            {
                                this.mscalesPanel.Children.Remove(labelTick);
                            }
                            if (labelTick.Parent is Panel)
                            {
                                (labelTick.Parent as Panel).Children.Remove(labelTick);
                            }
                            this.mscalesPanel.Children.Add(labelTick);
                            labelTick.Visibility = Visibility.Collapsed;
                            labelTick.GaugeElementParent = this;
                            double formattedLabelValue = this.GetFormatedDoubleValue(labelValue, labelTick);
                            while (formattedLabelValue < this.Maximum || (formattedLabelValue == this.Maximum && (decimal)(formattedLabelValue - this.Maximum) < (decimal)(valueInterval) && this.ShowLastValue))
                            {
                                value = this.GetFormatedValue(labelValue, labelTick);
                                value = labelTick.Prefix + value + labelTick.Suffix;
                                if (formattedLabelValue >= this.Maximum && (formattedLabelValue - this.Maximum) < valueInterval)
                                {
                                    labelValue = this.Maximum;
                                    if (!this.ShowLastValue)
                                        break;
                                    value = this.GetFormatedValue(labelValue, labelTick);
                                    value = labelTick.Prefix + value + labelTick.Suffix;
                                    size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                    if (this.IsReversed)
                                    {
                                        i = size.Height / 2;
                                    }
                                    else
                                    {
                                        i = scaleLength - termotemp + (size.Height / 2);
                                    }
                                }

                                if (!labelTick.IncludeFirstValue && labelValue == this.Minimum)
                                {
                                    labelValue += valueInterval;
                                    if (this.IsReversed)
                                        i -= interval;
                                    else
                                        i += interval;
                                }


                                size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                double length = size.Width;
                                size = this.GetTextSize(this.Maximum.ToString(), labelTick.FontFamily, labelTick.FontSize);
                                length = size.Width;

                                if (this.Orientation == GaugeOrientation.Horizontal)
                                {
                                    length = size.Height;
                                }
                                size = this.GetTextSize(value, labelTick.FontFamily, labelTick.FontSize);
                                double posX = 0;
                                if (labelTick.TickPlacement == ScalePlacement.Cross)
                                {
                                    posX = (-length / 2) - labelTick.DistanceFromScale;
                                }
                                else if (labelTick.TickPlacement == ScalePlacement.Outside)
                                {
                                    posX = (-this.ScaleBarSize / 2) - length - labelTick.DistanceFromScale;
                                }
                                else if (labelTick.TickPlacement == ScalePlacement.Inside)
                                {
                                    posX = (this.ScaleBarSize / 2) + labelTick.DistanceFromScale;
                                }

                                double angleAdjust = 0;
                                if (this.Orientation == GaugeOrientation.Horizontal)
                                {
                                   posX += (size.Height);
                               }

                                RotateTransform transform1 = new RotateTransform();
                                transform1.Angle = labelTick.Angle + angleAdjust;
                                transform1.CenterX = size.Width / 2;
                                transform1.CenterY = size.Height / 2;
                                LabelTickSet tick = new LabelTickSet();
                                if (labelTick.ShowToolTip)
                                {
                                    tick.tooltip = value;
                                }
                                else
                                {
                                    tick.tooltip = null;
                                }if (this.Orientation == GaugeOrientation.Vertical)
                                {
                                    tick.XOffset = posX;
                                    tick.YOffset = (scaleLength / 2) - i - termotemp;
                                }
                                else
                                {
                                    tick.XOffset = -(scaleLength / 2) + i - termotemp - (size.Width / 2 + length / 2);
                                    tick.YOffset = -posX;
                                }
                                
                                tick.Text = value;
                                tick.RenderTransform = transform1;
                                if (labelTick.flag == 4 || labelTick.flag == 0)
                                    tick.LabelForeground = labelTick.LabelForeground;
                                tick.FontSize = labelTick.FontSize;
                                Binding fontbinding = new Binding();
                                fontbinding.Source = labelTick;
                                fontbinding.Path = new PropertyPath("FontSize");
                                fontbinding.Mode = BindingMode.TwoWay;
                                tick.SetBinding(Control.FontSizeProperty, fontbinding);
                                tick.BorderBrush = labelTick.BorderBrush;
                                tick.BorderThickness = labelTick.BorderThickness;
                                this.mscalesPanel.Children.Add(tick);
                                tick.GaugeElementParent = labelTick;
                                if (this.IsReversed)
                                {
                                    i -= interval;
                                }
                                else
                                {
                                    i += interval;
                                }
                                if (labelValue == this.Maximum)
                                {
                                    break;
                                }
                                labelValue += valueInterval;
                                if (labelValue >= this.Maximum)
                                {
                                    labelValue = this.Maximum;
                                }
                                labelValue = Convert.ToDouble((decimal)(labelValue));
                                formattedLabelValue = this.GetFormatedDoubleValue(labelValue, labelTick);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occured when Tickset is added
        /// </summary>
        private void AddTickSet()
        {
            int flag = 0;
            double scaleLength = this.ScaleBarLength - this.FrameOffset;
            if (this.mscalesPanel != null)
            {
                
                    int count = this.mscalesPanel.Children.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i < this.mscalesPanel.Children.Count && this.mscalesPanel.Children[i] is MarkTickSet)
                        {
                            MarkTickSet tick = this.mscalesPanel.Children[i] as MarkTickSet;
                            if (this.mscalesPanel.Children.Contains(tick))
                            {
                                this.mscalesPanel.Children.Remove(tick);
                                i--;
                            }
                        }
                    }
                
                foreach (TickSetBase tickSet in this.Ticks)
                {
                   // indicator = true;
                    if (tickSet is MarkTickSet)
                    {
                        double i = 0, j=0;
                        double valueInterval = 0;
                        MarkTickSet markTick = tickSet as MarkTickSet;
                       if (markTick.TickStyle == TickStyle.MajorTick)
                        {
                            valueInterval = this.MajorIntervalValue;
                            flag = markTick.majorflag;
                        }
                        else if (markTick.TickStyle == TickStyle.MidTick)
                        {
                            valueInterval = this.MidIntervalValue;
                            flag = markTick.middleflag;
                        }
                        else
                        {
                            valueInterval = this.MinorIntervalValue;
                            flag = markTick.minorflag;
                        }

                        double ratio = (this.Maximum - this.Minimum) / valueInterval;  
                        double termotemp=0;
                        double interval = (scaleLength - termotemp) / ratio;
                       if (this.IsReversed)
                       {
                           i = scaleLength - (markTick.TickWidth / 2);
                       }
                       else
                       {
                           i =  markTick.TickWidth / 2;
                       }
                        if (interval > 0)
                        {
                            j = markTick.TickWidth / 2;
                            
                            if (this.mscalesPanel.Children.IndexOf(markTick) != -1)
                            {
                                this.mscalesPanel.Children.Remove(markTick);
                            }
                            if (markTick.Parent is Panel)
                            {
                                (markTick.Parent as Panel).Children.Remove(markTick);
                            }
                            this.mscalesPanel.Children.Add(markTick);
                            markTick.Visibility = Visibility.Collapsed;
                            markTick.GaugeElementParent = this;

                            bool SkipTickSet = false;

                            while (j <= (scaleLength - termotemp) + (markTick.TickWidth / 2) + 0.0001)
                            {
                                double posX = 0;
                                if (markTick.TickPlacement == ScalePlacement.Cross)
                                {
                                    posX = -markTick.DistanceFromScale;
                                }
                                else if (markTick.TickPlacement == ScalePlacement.Inside)
                                {
                                    posX = (-this.ScaleBarSize / 2 - (markTick.TickHeight / 2)) - markTick.DistanceFromScale;
                                }
                                else if (markTick.TickPlacement == ScalePlacement.Outside)
                                {
                                    posX = this.ScaleBarSize + markTick.DistanceFromScale + markTick.TickHeight / 2;
                                }

                                #region DisableInterScets
                                SkipTickSet = false;

                                if ((markTick.TickStyle == TickStyle.MinorTick || markTick.TickStyle == TickStyle.MidTick) &&
                                    this.DisableIntersectTicks == true)
                                {
                                    foreach (TickSetBase temp_tick in this.Ticks)
                                    {
                                        MarkTickSet temp_marktickset = temp_tick as MarkTickSet;
                                        if (temp_marktickset != null)
                                        {
                                            if (temp_marktickset.TickStyle == TickStyle.MajorTick)
                                            {
                                                double ratio1 = (this.Maximum - this.Minimum) / this.MajorIntervalValue;
                                                double interval1 = scaleLength / ratio1;

                                                double check;
                                                if (i < (interval1 + (temp_marktickset.TickWidth / 2)))
                                                {
                                                    check = i - (markTick.TickWidth / 2);
                                                }
                                                else
                                                {
                                                    check = (i - (markTick.TickWidth / 2)) % interval1;
                                                }
                                                double behind = interval1 - check;

                                                if (check <= temp_marktickset.TickWidth / 2 || behind <= temp_marktickset.TickWidth / 2)
                                                {
                                                    SkipTickSet = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (!SkipTickSet)
                                {
                                    double rotangle = 0;
                                    
                                    MarkTickSet tick = new MarkTickSet();
                                    if (this.Orientation == GaugeOrientation.Vertical)
                                    {
                                        tick.XOffset = posX;
                                        if (i > scaleLength && this.ShowLastValue)
                                        {
                                            i = scaleLength + (markTick.TickWidth / 2);
                                        }
                                        else
                                        {
                                            tick.YOffset = (scaleLength / 2) - i - termotemp;
                                        }
                                        rotangle = 0;
                                    }
                                    else
                                    {
                                        tick.YOffset = posX;
                                        if (i > scaleLength && this.ShowLastValue)
                                        {
                                            i = scaleLength + (markTick.TickWidth / 2);
                                            tick.XOffset =  i - termotemp;
                                        }
                                        else
                                        {
                                            tick.XOffset = -(scaleLength / 2) + i - termotemp;
                                        }
                                        rotangle = 90;
                                    }

                                    RotateTransform transform1 = new RotateTransform();
                                    transform1.Angle = markTick.Angle+rotangle;                               
                                    tick.RenderTransform = transform1;
                                    tick.RenderTransformOrigin = new Point(0.5, 0.5);
                                    if (flag == 0 || flag == 4)
                                        tick.TickBackground = markTick.TickBackground;
                                    tick.TickWidth = markTick.TickWidth;
                                    tick.TickHeight = markTick.TickHeight;
                                    tick.TickStyle = markTick.TickStyle;
                                    tick.TickShape = markTick.TickShape;
                                    tick.TickPlacement = markTick.TickPlacement;
                                    tick.GaugeElementParent = this;
                                    this.mscalesPanel.Children.Add(tick);
                                    tick.GaugeElementParent = markTick;
                                    tick.RefreshTick();
                                }
                                j += interval;

                                if (this.IsReversed)
                                {
                                    i -= interval;
                                }
                                else
                                {
                                    i += interval;
                                }
                            }
                        }

                        if (((this.Maximum - this.Minimum) % valueInterval != 0) && (((decimal)((this.Maximum - this.Minimum) % valueInterval))!=(decimal)valueInterval) && valueInterval == this.MajorIntervalValue && this.ShowLastValue && valueInterval == this.Maximum)
                        {
                            double posX = 0;
                            if (markTick.TickPlacement == ScalePlacement.Cross)
                            {
                                posX = -markTick.DistanceFromScale;
                            }
                            else if (markTick.TickPlacement == ScalePlacement.Inside)
                            {
                                posX = (-this.ScaleBarSize / 2 - (markTick.TickHeight / 2)) - markTick.DistanceFromScale;
                            }
                            else if (markTick.TickPlacement == ScalePlacement.Outside)
                            {
                                posX = this.ScaleBarSize + markTick.DistanceFromScale + markTick.TickHeight / 2;
                            }

                            MarkTickSet tick = new MarkTickSet();
                            double angle = 0;
                            if (this.Orientation == GaugeOrientation.Vertical)
                            {
                                tick.XOffset = posX;
                                if (this.IsReversed)
                                {
                                    tick.YOffset = (scaleLength / 2) - (markTick.TickWidth / 2);
                                }
                                else
                                {
                                    tick.YOffset = -(scaleLength / 2) - (markTick.TickWidth / 2);
                                }
                            }
                            else
                            {
                                tick.YOffset = posX;
                                if (this.IsReversed)
                                {
                                    tick.XOffset = (scaleLength / 2) - (markTick.TickWidth / 2);
                                }
                                else
                                {
                                    tick.XOffset = (scaleLength) - (markTick.TickWidth / 2);
                                }
                                angle = 90;
                            }
                            RotateTransform transform1 = new RotateTransform();
                            transform1.Angle = markTick.Angle + angle;
                            tick.RenderTransform = transform1;
                            tick.RenderTransformOrigin = new Point(0.5, 0.5);
                            if (flag == 0 || flag == 4)
                                tick.TickBackground = markTick.TickBackground;
                            tick.TickWidth = markTick.TickWidth;
                            tick.TickHeight = markTick.TickHeight;
                            tick.TickStyle = markTick.TickStyle;
                            tick.TickShape = markTick.TickShape;
                            tick.TickPlacement = markTick.TickPlacement;
                            tick.GaugeElementParent = this;
                            this.mscalesPanel.Children.Add(tick);
                            tick.GaugeElementParent = markTick;
                            tick.RefreshTick();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
