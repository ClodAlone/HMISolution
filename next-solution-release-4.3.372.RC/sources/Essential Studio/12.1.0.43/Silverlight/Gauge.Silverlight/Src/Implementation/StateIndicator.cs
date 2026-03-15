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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents gauge visual element with ability to be turned on/off.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge;
    /// <para></para>
    /// <para>StateIndicator stateindicator = new StateIndicator();</para>
    /// <para>            stateindicator.StateRanges.Add(new StateRange(70, 100));</para>
    /// <para>            stateindicator.ActiveBackgroundBrush = new SolidColorBrush(Colors.Blue);</para>
    /// <para>            stateindicator.Background = new SolidColorBrush(Colors.Red);</para>
    /// <para>            stateindicator.Location = new Point(60, 80);</para>
    /// <para>            stateindicator.IndicatorHeight = 10;</para>
    /// <para>            stateindicator.IndicatorWidth = 10;</para>
    /// <para>            stateindicator.Text = &quot;Off&quot;;</para>
    /// <para>            stateindicator.ActiveText = &quot;On&quot;;</para>
    /// <para>            stateindicator.IndicatorStyle = IndicatorStyle.CircularLED</para>
    /// <para>            circulargauge.StateIndicators.Add(stateindicator);</para></description></item></list>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description> xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;
    /// <para>   </para>
    /// <para>&lt;syncfusion:CircularGauge.StateIndicators&gt;</para>
    /// <para>                &lt;syncfusion:StateIndicator Name=&quot;m_indicator&quot; Background=&quot;Green&quot; ActiveBackgroundBrush=&quot;Red&quot;  Location=&quot;50,80&quot; IndicatorHeight=&quot;6&quot; IndicatorWidth=&quot;6&quot; FontSize=&quot;12&quot; FontFamily=&quot;Verdana&quot;  IndicatorStyle=&quot;CircularLED&quot; Text=&quot;ON&quot; ActiveText=&quot;On&quot;  &gt;</para>
    /// <para>                    &lt;syncfusion:StateIndicator.StateRanges&gt;</para>
    /// <para>                        &lt;syncfusion:StateRange StartValue=&quot;70&quot; EndValue=&quot;100&quot;/&gt;</para>
    /// <para>                    &lt;/syncfusion:StateIndicator.StateRanges&gt;</para>
    /// <para>                &lt;/syncfusion:StateIndicator&gt;</para>
    /// <para>            &lt;/syncfusion:CircularGauge.StateIndicators&gt;</para></description></item></list>
    /// </example>
    public class StateIndicator : LocalizableGaugeElement
    {              
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.ActiveBackgroundBrush">ActiveBackgroundBrush</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.Windows.Media.Brush">System.Windows.Media.Brush</see></para>
        /// </returns>
        public static readonly DependencyProperty ActiveBackgroundBrushProperty =
            DependencyProperty.Register("ActiveBackgroundBrush", typeof(Brush), typeof(StateIndicator),new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.ActiveBorderBrush">ActiveBorderBrush</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Windows.Media.Brush">System.Windows.Media.Brush</see>
        /// </returns>
        public static readonly DependencyProperty ActiveBorderBrushProperty =
            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(StateIndicator),new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.ActiveText">ActiveText</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.String">System.String</see>
        /// </returns>
        public static readonly DependencyProperty ActiveTextProperty =
            DependencyProperty.Register("ActiveText", typeof(string), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnActiveTextChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.IndicatorHeight">IndicatorHeight</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty IndicatorHeightProperty =
            DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnStateIndicatorHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.IndicatorStyle">IndicatorStyle</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.IndicatorStyle">IndicatorStyle</see>
        /// </returns>
        public static readonly DependencyProperty IndicatorStyleProperty =
            DependencyProperty.Register("IndicatorStyle", typeof(IndicatorStyle), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnIndicatorStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.IndicatorWidth">IndicatorWidth</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty IndicatorWidthProperty =
            DependencyProperty.Register("IndicatorWidth", typeof(double), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnStateIndicatorWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.Text">Text</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.String">System.String</see></para>
        /// </returns>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.Value">Value</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.Angle">Angle</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        internal static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(StateIndicator), new PropertyMetadata(new PropertyChangedCallback(OnAngleChanged)));

        #endregion

        #region Private members
        
        /// <summary>
        /// indicates whether the indicator is loaded or not
        /// </summary>
        private bool misLoaded = false;

        /// <summary>
        /// The path for state indicator
        /// </summary>
        private Path mpath;

        /// <summary>
        /// Collection of state ranges used to turn on/off the state indicator.
        /// </summary>
        private StateRangeCollection mstateRanges = new StateRangeCollection();

        /// <summary>
        /// Textblock used for state display
        /// </summary>
        private TextBlock mtextBlock;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.StateIndicator">StateIndicator</see> class.
        /// </summary>
        public StateIndicator()
        {
            DefaultStyleKey = typeof(StateIndicator);
            this.Loaded += new RoutedEventHandler(this.StateIndicatorLoaded);
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.ActiveText">ActiveText</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ActiveTextChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.IndicatorStyle">IndicatorStyle</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback IndicatorStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.IndicatorHeight">StateIndicatorHeight</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback StateIndicatorHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.IndicatorWidth">StateIndicatorWidth</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback StateIndicatorWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.Text">Text</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.Value">Value</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateIndicator.Angle">Angle</see> property is changed.
        /// </summary>
        internal event PropertyChangedCallback AngleChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the background of the state indicator when it is turned on. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">System.Window.Media.Brush</see>
        /// </value>
        /// <seealso cref="Brush">Brush</seealso>
        public Brush ActiveBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(ActiveBackgroundBrushProperty);
            }
            
            set
            {
                SetValue(ActiveBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush of the state indicator when it is turned on. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">System.Window.Media.Brush</see>
        /// </value>
        /// <seealso cref="Brush">Brush</seealso>
        public Brush ActiveBorderBrush
        {
            get
            {
                return (Brush)GetValue(ActiveBorderBrushProperty);
            }

            set
            {
                SetValue(ActiveBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed in IndicatorStyle.Text mode when the state indicator is turned on. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is string.empty.
        /// </remarks>
        /// <value>
        /// Type:  <see cref="T:System.String">System.String</see>
        /// </value>
        /// <seealso cref="string">string</seealso>
        public string ActiveText
        {
            get
            {
                return (string)GetValue(ActiveTextProperty);
            }

            set
            {
                SetValue(ActiveTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the state indicator. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.String">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double IndicatorHeight
        {
            get
            {
                return (double)GetValue(IndicatorHeightProperty);
            }

            set
            {
                SetValue(IndicatorHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the different look of the state indicator. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is <see cref="F:Syncfusion.Windows.Gauge.IndicatorStyle.CircularLED">IndicatorStyle.CircularLED</see>.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.IndicatorStyle">IndicatorStyle</see>
        /// </value>
        /// <seealso cref="IndicatorStyle">IndicatorStyle</seealso>
        public IndicatorStyle IndicatorStyle
        {
            get
            {
                return (IndicatorStyle)GetValue(IndicatorStyleProperty);
            }

            set
            {
                SetValue(IndicatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the state indicator. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.String">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double IndicatorWidth
        {
            get
            {
                return (double)GetValue(IndicatorWidthProperty);
            }

            set
            {
                SetValue(IndicatorWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of state ranges used to turn on/off the state indicator.
        /// </summary>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.StateRangeCollection">StateRangeCollection</see>
        /// </value>
        /// <seealso cref="StateRangeCollection">StateRangeCollection</seealso>
        public StateRangeCollection StateRanges
        {
            get
            {
                return this.mstateRanges;
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed in indicatorStyle.Text mode when the state indicator is turned off. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is <see cref="F:System.String.Empty">String.Empty</see>.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.String">System.String</see>
        /// </value>
        /// <seealso cref="string">string</seealso>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the state indicator. If it falls within one of the state ranges the state indicator turns on. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
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
        /// Gets the indicator path
        /// </summary>
        internal Path PART_IndicatorPath
        {
            get
            {
                return this.mpath;
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle of the state indicator.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        internal double Angle
        {
            get
            {
                return (double)GetValue(AngleProperty);
            }

            set
            {
                SetValue(AngleProperty, value);
            }
        }

        /// <summary>
        /// Gets the indicator text
        /// </summary>
        internal TextBlock PART_IndicatorText
        {
            get
            {
                return this.mtextBlock;
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

            this.mpath = this.GetTemplateChild("PART_IndicatorPath") as Path;
            this.mtextBlock = this.GetTemplateChild("PART_IndicatorText") as TextBlock;

            Size indicatorSize = this.GetDesiredSize();
            switch (this.IndicatorStyle)
            {
                case IndicatorStyle.RectangularLED:
                    if (this.mpath != null)
                    {
                        this.mpath.Visibility = Visibility.Visible;
                        RectangleGeometry rectangleGeometry = new RectangleGeometry();
                        rectangleGeometry.Rect = new Rect(0, 0, indicatorSize.Width, indicatorSize.Width);
                        this.mpath.Data = rectangleGeometry;
                        this.mtextBlock.Visibility = Visibility.Collapsed;
                    }

                    break;

                case IndicatorStyle.CircularLED:
                    if (this.mpath != null)
                    {
                        this.mpath.Visibility = Visibility.Visible;
                        EllipseGeometry ellipseGeometry = new EllipseGeometry();
                        ellipseGeometry.Center = new Point(indicatorSize.Width / 2, indicatorSize.Height / 2);
                        ellipseGeometry.RadiusX = indicatorSize.Width / 2;
                        ellipseGeometry.RadiusY = indicatorSize.Height / 2;
                        this.mpath.Data = ellipseGeometry;
                        this.mtextBlock.Visibility = Visibility.Collapsed;
                    }

                    break;

                case IndicatorStyle.Text:
                    if (this.mtextBlock != null)
                    {
                        this.mtextBlock.Visibility = Visibility.Visible;
                        this.mtextBlock.Text = this.Text;
                        this.mtextBlock.FontFamily = this.FontFamily;
                        this.mtextBlock.FontSize = this.FontSize;
                        this.mtextBlock.FontWeight = this.FontWeight;
                        this.mtextBlock.Foreground = this.Background;
                        this.mpath.Visibility = Visibility.Collapsed;
                    }

                    break;
            }
            this.UpdateVisualStyle();
            this.RefreshIndicatorState();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
                
        /// <summary>
        /// Gets the value indicating whether the state indicator is turned on.
        /// </summary>
        /// <returns>The value indicating whether the state indicator is turned on.</returns>
        protected virtual bool GetIsStateActive()
        {
            foreach (StateRange range in this.StateRanges)
            {
                if (this.Value >= range.StartValue && this.Value <= range.EndValue)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual Brush GetStateBrush()
        {
            foreach (StateRange range in this.StateRanges)
            {
                if (this.Value >= range.StartValue && this.Value <= range.EndValue)
                {
                    return range.RangeColor != null ? range.RangeColor : this.ActiveBackgroundBrush;
                }
            }

            return this.Background;
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            return this.GetDesiredSize();
        }
   
        /// <summary>
        /// Updates property value cache and raises <see cref="ActiveTextChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnActiveTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ActiveTextChanged != null)
            {
                this.ActiveTextChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            RotateTransform transform = new RotateTransform();
            transform.Angle = this.Angle;
            transform.CenterX = this.GetDesiredSize().Width / 2;
            transform.CenterY = this.GetDesiredSize().Height / 2;

            if (this.IndicatorStyle == IndicatorStyle.Text)
            {
                if (this.mtextBlock != null)
                {
                    this.mtextBlock.RenderTransform = transform;
                }
            }
            else
            {
                if (this.mpath != null)
                {
                    this.mpath.RenderTransform = transform;
                }
            }

            if (this.AngleChanged != null)
            {
                this.AngleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IndicatorStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIndicatorStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshIndicator();
            this.RefreshIndicatorState();
            this.InvalidateMeasure();
            this.InvalidateArrange();
            if (this.IndicatorStyleChanged != null)
            {
                this.IndicatorStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StateIndicatorHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStateIndicatorHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
            if (this.StateIndicatorHeightChanged != null)
            {
                this.StateIndicatorHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StateIndicatorWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStateIndicatorWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
            if (this.StateIndicatorWidthChanged != null)
            {
                this.StateIndicatorWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshIndicatorState();
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnActiveTextChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnActiveTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnActiveTextChanged(e);
        }

        /// <summary>
        /// Calls OnIndicatorStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIndicatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnIndicatorStyleChanged(e);
        }

        /// <summary>
        /// Calls OnStateIndicatorHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStateIndicatorHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnStateIndicatorHeightChanged(e);
        }

        /// <summary>
        /// Calls OnStateIndicatorWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStateIndicatorWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnStateIndicatorWidthChanged(e);
        }

        /// <summary>
        /// Calls OnTextChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnTextChanged(e);
        }

        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnValueChanged(e);
        }

        /// <summary>
        /// Calls OnAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnAngleChanged(e);
        }

        /// <summary>
        /// Refreshs the indicator
        /// </summary>
        private void RefreshIndicator()
        {
            if (this.misLoaded)
            {
                Size indicatorSize = this.GetDesiredSize();
                switch (this.IndicatorStyle)
                {
                    case IndicatorStyle.RectangularLED:
                        if (this.mpath != null)
                        {
                            this.mpath.Visibility = Visibility.Visible;
                            RectangleGeometry rectangleGeometry = new RectangleGeometry();
                            rectangleGeometry.Rect = new Rect(0, 0, indicatorSize.Width, indicatorSize.Width);
                            this.mpath.Data = rectangleGeometry;
                            this.mtextBlock.Visibility = Visibility.Collapsed;
                        }

                        break;

                    case IndicatorStyle.CircularLED:
                        if (this.mpath != null)
                        {
                            this.mpath.Visibility = Visibility.Visible;
                            EllipseGeometry ellipseGeometry = new EllipseGeometry();
                            ellipseGeometry.Center = new Point(indicatorSize.Width / 2, indicatorSize.Height / 2);
                            ellipseGeometry.RadiusX = indicatorSize.Width / 2;
                            ellipseGeometry.RadiusY = indicatorSize.Height / 2;
                            this.mpath.Data = ellipseGeometry;
                            this.mtextBlock.Visibility = Visibility.Collapsed;
                        }

                        break;

                    case IndicatorStyle.Text:
                        if (this.mtextBlock != null)
                        {
                            this.mtextBlock.Visibility = Visibility.Visible;
                            this.mtextBlock.Text = this.Text;
                            this.mtextBlock.FontFamily = this.FontFamily;
                            this.mtextBlock.FontSize = this.FontSize;
                            this.mtextBlock.FontWeight = this.FontWeight;
                            this.mtextBlock.Foreground = this.Background;
                            this.mpath.Visibility = Visibility.Collapsed;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Refreshes the indicator state
        /// </summary>
        private void RefreshIndicatorState()
        {
            if (this.GetIsStateActive())
            {
                switch (this.IndicatorStyle)
                {
                    case IndicatorStyle.RectangularLED:
                    case IndicatorStyle.CircularLED:
                        if (this.mpath != null)
                        {
                            this.mpath.Fill = this.GetStateBrush();
                            this.mpath.Stroke = this.ActiveBorderBrush;
                        }

                        break;

                    case IndicatorStyle.Text:
                        if (this.mtextBlock != null)
                        {
                            this.mtextBlock.Foreground = this.GetStateBrush();
                            this.mtextBlock.Text = this.ActiveText;
                        }
                        break;
                }
            }
            else
            {
                switch (this.IndicatorStyle)
                {
                    case IndicatorStyle.RectangularLED:
                    case IndicatorStyle.CircularLED:
                        if (this.mpath != null)
                        {
                            this.mpath.Fill = this.Background;
                            this.mpath.Stroke = this.BorderBrush;
                        }

                        break;

                    case IndicatorStyle.Text:
                        if (this.mtextBlock != null)
                        {
                            this.mtextBlock.Foreground = this.Background;
                            this.mtextBlock.Text = this.Text;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Invoked when the State indicator is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void StateIndicatorLoaded(object sender, RoutedEventArgs e)
        {
            this.misLoaded = true;
        }

        /// <summary>
        /// Gets the Size of the indicator
        /// </summary>
        /// <returns>Returns the size</returns>
        private Size GetDesiredSize()
        {
            Size size = new Size(5, 5);
            if (this.IndicatorStyle == IndicatorStyle.Text)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontFamily = this.FontFamily;
                textBlock.FontSize = this.FontSize;
                textBlock.FontStyle = this.FontStyle;
                textBlock.FontWeight = this.FontWeight;
                if (this.GetIsStateActive())
                {
                    textBlock.Text = this.ActiveText;
                }
                else
                {
                    textBlock.Text = this.Text;
                }

                size = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
            }
            else
            {
                size = new Size(this.IndicatorWidth, this.IndicatorHeight);
            }

            return size;
        }

        #endregion
    }
}
