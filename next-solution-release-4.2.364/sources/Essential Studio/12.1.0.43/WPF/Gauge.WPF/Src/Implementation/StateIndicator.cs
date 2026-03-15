// <copyright file="StateIndicator.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Data;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the gauge visual element that has the ability to indicate the state of the Gauge, 
    /// by turning itself "on" when the pointer reaches a stated range and vice versa. 
    /// </summary>
    /// <remarks>
    /// Inorder to make the StateIndicator work property, the <see cref="StateRange"/> values should 
    /// be properly initialized and the <see cref="ActiveBackgroundBrush"/> should be set to a 
    /// different color other than the range's <see cref="GaugeElement.BackgroundBrush"/> color inorder
    /// to make the StateIndicator's state change visible.
    /// </remarks>
    /// <seealso cref="StateRange"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="WpfApplication5.Window1" Title="Window1" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100" Minimum="0" 
    ///                                       Maximum="100" MajorIntervalValue="10" MinorIntervalValue="2"
    ///                                       ScaleBarSize="10" GapSweepAngle="310" StartAngle="110"
    ///                                       BackgroundBrush="LightBlue">
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///         <syncfusion:CircularGauge.StateIndicators>
    ///             <syncfusion:StateIndicator ActiveBackgroundBrush="Red" IndicatorHeight="20" 
    ///                                        IndicatorWidth="20" FontSize="12" FontFamily="Verdana" 
    ///                                        Name="m_indicator1" IndicatorStyle="RoundedRectangularLED" 
    ///                                        Text="Off" ActiveText="On" Location="50,80">
    ///                 <syncfusion:StateIndicator.StateRanges>
    ///                     <syncfusion:StateRange StartValue="180" EndValue="240" />
    ///                 </syncfusion:StateIndicator.StateRanges>
    ///             </syncfusion:StateIndicator>
    ///         </syncfusion:CircularGauge.StateIndicators>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace WpfApplication5<para/>
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         private CircularScale m_scale;
    ///         private StateIndicator m_indicator;
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
    ///             this.m_gauge.Scales.Add(m_scale);<para/>
    ///             m_indicator = new StateIndicator();
    ///             m_indicator.StateRanges.Add(new StateRange(10, 20));
    ///             m_indicator.StateRanges.Add(new StateRange(50, 60));
    ///             m_indicator.IndicatorStyle = IndicatorStyle.RoundedRectangularLED;
    ///             m_indicator.BackgroundBrush = new RadialGradientBrush(Colors.White,Colors.DarkGreen);
    ///             m_indicator.ActiveBackgroundBrush = new RadialGradientBrush(Colors.White, Colors.Red);
    ///             m_indicator.IndicatorWidth = 20;
    ///             m_indicator.IndicatorHeight = 20;
    ///             m_indicator.Location = new Point(50, 80);
    ///             m_gauge.StateIndicators.Add(m_indicator);
    ///             this.Content = m_gauge;<para/>
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [StyleTypedProperty(Property = "ScaleStyle", StyleTargetType = typeof(StateIndicator))]
    public class StateIndicator : LocalizableGaugeElement
    {
        #region Private Memebers
        /// <summary>
        /// Rect object used to draw the state indicator.
        /// </summary>
        private Rect m_rect;

        /// <summary>
        /// Stores the gauge's reference
        /// </summary>
        private GaugeBase gaugeReference = null;

        /// <summary>
        /// Collection of state ranges used to turn on/off the state indicator.
        /// </summary>
        private StateRangeCollection m_stateRanges = new StateRangeCollection();

        /// <summary>
        /// The ratio between the StateIndicator size and gauge size.
        /// </summary>
        private double m_startsizeRatio;

        /// <summary>
        /// The ratio between the StateIndicator size and gauge size.
        /// </summary>
        private double m_endsizeRatio;

        private double _IndicatorRadiusRatio;
        #endregion Private Memebers

        #region CLR Getters & Setters
        /// <summary>
        /// Gets the Collection of state ranges used to turn on/off the state indicator.
        /// </summary>
        /// <value>
        /// Type: <see cref="StateRangeCollection"/>
        /// </value>
        public StateRangeCollection StateRanges
        {
            get
            {
                return m_stateRanges;
            }
        }
        #endregion CLR Getters & Setters

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="ActiveText"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ActiveTextChanged;

        /// <summary>
        /// Event that is raised when <see cref="Angle"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback AngleChanged;

        /// <summary>
        /// Event that is raised when <see cref="IndicatorStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IndicatorStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="IndicatorHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback StateIndicatorHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="IndicatorWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback StateIndicatorWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="Text"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="IndicatorCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IndicatorCustomGeometryChanged;

        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="ActiveBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveBackgroundBrushProperty =
            DependencyProperty.Register("ActiveBackgroundBrush", typeof(Brush), typeof(StateIndicator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="ActiveBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveBorderBrushProperty =
            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(StateIndicator), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="ActiveText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveTextProperty =
            DependencyProperty.Register("ActiveText", typeof(string), typeof(StateIndicator), new FrameworkPropertyMetadata("ON", FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnActiveTextChanged)));

        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(StateIndicator), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(StateIndicator), new FrameworkPropertyMetadata(new FontFamily(), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(StateIndicator), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="IndicatorHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorHeightProperty =
            DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(StateIndicator), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStateIndicatorHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="IndicatorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorStyleProperty =
            DependencyProperty.Register("IndicatorStyle", typeof(IndicatorStyle), typeof(StateIndicator), new FrameworkPropertyMetadata(IndicatorStyle.CircularLED, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIndicatorStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="IndicatorWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorWidthProperty =
            DependencyProperty.Register("IndicatorWidth", typeof(double), typeof(StateIndicator), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStateIndicatorWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StateIndicator), new FrameworkPropertyMetadata("OFF", FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(StateIndicator), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="IndicatorCustomGeometryProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorCustomGeometryProperty = DependencyProperty.Register("IndicatorCustomGeometry", typeof(Geometry), typeof(StateIndicator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIndicatorCustomGeometryChanged)));


        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the background of the state indicator
        /// when it is turned on. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
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
        /// Gets or sets the border brush of the state indicator
        /// when it is turned on. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
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
        /// Gets or sets the text that is displayed in <see cref="Syncfusion.Windows.Gauge.IndicatorStyle.Text"/>
        /// mode when the state indicator is turned on.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.empty.
        /// </value>
        /// <seealso cref="Text"/>
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
        /// Gets or sets the rotation angle of the state indicator.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the font family for the text 
        /// that is displayed in <see cref="Syncfusion.Windows.Gauge.IndicatorStyle.Text"/> mode.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/>
        /// </value>
        public new FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }

            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font size for the text 
        /// that is displayed in <see cref="Syncfusion.Windows.Gauge.IndicatorStyle.Text"/> mode.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 12.
        /// </value>
        public new double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }

            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the state indicator.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the indicator style value.
        /// Specifies a different look for the state indicator.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="IndicatorStyle"/>
        /// Default value is IndicatorStyle.CircularLED.
        /// </value>
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
        /// Gets or sets the width of the state indicator.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the text that is displayed in <see cref="Syncfusion.Windows.Gauge.IndicatorStyle.Text"/>
        /// mode when the state indicator is turned off.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.empty.
        /// </value>
        /// <seealso cref="ActiveText"/>
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
        /// Gets or sets the value of the state indicator. If it falls 
        /// within one of the state ranges the state indicator turns on.
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
        /// Gets or sets a custom geometry for the State Indicator.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Geometry"/>
        /// Default value is null.
        /// </value>
        public Geometry IndicatorCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(IndicatorCustomGeometryProperty);
            }

            set
            {
                SetValue(IndicatorCustomGeometryProperty, value);
            }
        }

        internal double IndicatorRadiusRatio
        {
            get
            {
                return _IndicatorRadiusRatio;
            }
            set
            {
                _IndicatorRadiusRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of StateIndicator's Width.
        /// </summary>
        internal double startSizeRatio
        {
            get
            {
                return m_startsizeRatio;
            }

            set
            {
                m_startsizeRatio = value;
            }
        }
        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of StateIndicator's Width.
        /// </summary>
        internal double endSizeRatio
        {
            get
            {
                return m_endsizeRatio;
            }

            set
            {
                m_endsizeRatio = value;
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="StateIndicator"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static StateIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StateIndicator), new FrameworkPropertyMetadata(typeof(StateIndicator)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateIndicator"/> class.
        /// </summary>
        public StateIndicator()
        {
            this.StateRanges.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size(5, 5);
            if (this.IndicatorStyle == IndicatorStyle.Text)
            {
                Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), new FontWeight(), new FontStretch());
                FormattedText formattedText = null;
                if (this.GetIsStateActive())
                {
                    formattedText = new FormattedText(this.ActiveText, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, null);
                }
                else
                {
                    formattedText = new FormattedText(this.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, null);
                }

                size = new Size(formattedText.Width, formattedText.Height);
            }
            else
            {
                size = new Size(this.IndicatorWidth > 0 ? this.IndicatorWidth : 0, this.IndicatorHeight >0 ? this.IndicatorHeight : 0);
            }

            return size;
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_rect = new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height);
            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            RotateTransform transform = new RotateTransform(this.Angle, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
            drawingContext.PushTransform(transform);
            if (this.IndicatorStyle == IndicatorStyle.Text)
            {
                Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), new FontWeight(), new FontStretch());
                FormattedText formattedText = null;
                if (this.GetIsStateActive())
                {
                    formattedText = new FormattedText(this.ActiveText, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.ActiveBackgroundBrush);
                }
                else
                {
                    formattedText = new FormattedText(this.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.BackgroundBrush);
                }

                drawingContext.DrawText(formattedText, new Point(0, 0));
            }
            else if (this.IndicatorStyle == IndicatorStyle.CircularLED)
            {
                if (this.GetIsStateActive())
                {
                    drawingContext.DrawEllipse(this.GetActiveStateColor(), new Pen(this.ActiveBorderBrush, this.BorderWidth), new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2), m_rect.Width / 2, m_rect.Height / 2);
                }
                else
                {
                    drawingContext.DrawEllipse(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2), m_rect.Width / 2, m_rect.Height / 2);
                }
            }
            else if (this.IndicatorStyle == IndicatorStyle.RectangularLED)
            {
                if (this.GetIsStateActive())
                {
                    drawingContext.DrawRectangle(this.GetActiveStateColor(), new Pen(this.ActiveBorderBrush, this.BorderWidth), m_rect);
                }
                else
                {
                    drawingContext.DrawRectangle(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_rect);
                }
            }
            else if (this.IndicatorStyle == IndicatorStyle.RoundedRectangularLED)
            {
                if (this.GetIsStateActive())
                {
                    drawingContext.DrawRoundedRectangle(this.GetActiveStateColor(), new Pen(this.ActiveBorderBrush, this.BorderWidth), m_rect, m_rect.Height / 3, m_rect.Height / 3);
                }
                else
                {
                    drawingContext.DrawRoundedRectangle(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_rect, m_rect.Height / 3, m_rect.Height / 3);
                }
            }
            else if (this.IndicatorStyle == IndicatorStyle.Custom)
            {
                if (this.IndicatorCustomGeometry != null)
                {
                    if (this.GetIsStateActive())
                    {
                        drawingContext.DrawGeometry(this.GetActiveStateColor(), new Pen(this.BorderBrush, this.BorderWidth), IndicatorCustomGeometry);
                    }
                    else
                    {
                        drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), IndicatorCustomGeometry);
                    }
                }
                else
                {
                    if (this.GetIsStateActive())
                    {
                        drawingContext.DrawEllipse(this.GetActiveStateColor(), new Pen(this.ActiveBorderBrush, this.BorderWidth), new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2), m_rect.Width / 2, m_rect.Height / 2);
                    }
                    else
                    {
                        drawingContext.DrawEllipse(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2), m_rect.Width / 2, m_rect.Height / 2);
                    }
                }
            }

            drawingContext.Pop();
        }
        #endregion Overrides

        #region Implementation
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

        /// <summary>
        ///  This method gets the Activation state Colors of the StateIndicator
        /// </summary>
        protected virtual Brush GetActiveStateColor()
        {
            foreach (StateRange range in this.StateRanges)
            {
                if (this.Value >= range.StartValue && this.Value <= range.EndValue)
                {
                    return range.RangeColor != null ? range.RangeColor : this.ActiveBackgroundBrush;
                }
            }

            return this.BackgroundBrush;
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
        /// Updates property value cache and raises <see cref="ActiveTextChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnActiveTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ActiveTextChanged != null)
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
            if (AngleChanged != null)
            {
                this.AngleChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="IndicatorStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIndicatorStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IndicatorStyleChanged != null)
            {
                this.IndicatorStyleChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="StateIndicatorHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStateIndicatorHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StateIndicatorHeightChanged != null)
            {
                this.StateIndicatorHeightChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="StateIndicatorWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStateIndicatorWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StateIndicatorWidthChanged != null)
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
            if (TextChanged != null)
            {
                this.TextChanged(this, e);
            }
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
            instance.InvalidateVisual();
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }



        /// <summary>
        /// Calls OnIndicatorCustomGeometryChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIndicatorCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicator instance = (StateIndicator)d;
            instance.OnIndicatorCustomGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises OnIndicatorCustomGeometryChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIndicatorCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IndicatorCustomGeometryChanged != null)
            {
                this.IndicatorCustomGeometryChanged(this, e);
            }
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }

        /// <summary>
        /// Sets the scope to Gauge's scope to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="GaugeBase"/> that contains the reference to the Gauge.</param>
        internal void SetScope(GaugeBase gauge)
        {
            gaugeReference = gauge;
            if (gauge.StateIndicatorStyle != null)
                this.Style = gauge.StateIndicatorStyle;
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

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    if (e.NewItems[i] is StateRange)
                    {
                        StateRange ele = e.NewItems[i] as StateRange;
                        ele.SetScope(gaugeReference);
                        if (ele.DataContext == null)
                        {
                            BindingUtils.SetBinding(ele, this, FrameworkElement.DataContextProperty, FrameworkElement.DataContextProperty, BindingMode.OneWay);
                        }
                    }
                }
            }
        }


        #endregion Added Code
    }
}

