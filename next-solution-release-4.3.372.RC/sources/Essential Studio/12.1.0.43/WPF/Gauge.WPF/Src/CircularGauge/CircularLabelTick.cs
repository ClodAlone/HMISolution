// <copyright file="CircularLabelTick.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the numeric label tick of the circular scale.
    /// </summary>
    /// <remarks>
    /// The CicularLabelTick can be made to display Labels for either the <see cref="TickStyle.MajorTick"/> or 
    /// <see cref="TickStyle.MinorTick"/>. The Tick to which the labels should be placed is set by using the 
    /// <see cref="TickBase.TickStyle"/> property.
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CircularLableTickSample.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="CircularLableTickSample" Height="400" Width="400" >
    ///  <syncfusion:CircularGauge Name="circularGauge" >
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale"  
    ///                                       Radius="100"
    ///                                       Minimum="0" Maximum="100" 
    ///                                       MajorIntervalValue="10"
    ///                                       MinorIntervalValue="5"
    ///                                       ScaleBarSize = "10"
    ///                                       GapSweepAngle="310"
    ///                                       StartAngle="100">
    ///                 <syncfusion:CircularScale.Ticks>
    ///                     <syncfusion:CircularLabelTick FontSize="15"
    ///                                                   TickStyle="MajorTick"
    ///                                                   BackgroundBrush="GhostWhite"
    ///                                                   Name="CircularLabelTick1"
    ///                                                   TickPlacement="Outside"
    ///                                                   DistanceFromScale="5"/>                    
    ///                 </syncfusion:CircularScale.Ticks>
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// </example> 
    /// <example>
    /// <code lang="C#">
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CircularLableTickSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         private CircularGauge m_gauge;
    ///         private CircularScale m_scale;
    ///         private CircularLabelTick m_label;<para/>
    ///         public Window1()
    ///         {
    ///             InitializeComponent();<para/>      
    ///             m_gauge = new CircularGauge();
    ///             m_scale = new CircularScale();
    ///             m_label = new CircularLabelTick();
    ///             m_scale.Minimum = 0;
    ///             m_scale.Maximum = 100;
    ///             m_scale.MinorIntervalValue = 2;
    ///             m_scale.MajorIntervalValue = 10;
    ///             m_scale.StartAngle = 120;
    ///             m_scale.GapSweepAngle = 300;
    ///             m_scale.ScaleBarSize = 10;
    ///             m_scale.Radius = 116;
    ///             m_label.TickPlacement = ScalePlacement.Inside;
    ///             m_label.DistanceFromScale = 5;
    ///             m_label.FontSize = 13;
    ///             m_label.TickStyle = TickStyle.MajorTick;
    ///             m_label.BackgroundBrush = Brushes.GhostWhite;
    ///             m_gauge.Scales.Add(m_scale);
    ///             m_scale.Ticks.Add(m_label);
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CircularLabelTick : TickBase
    {
        #region Events

        /// <summary>
        /// Event that is raised when <see cref="NumberFormatInfo"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberFormatInfoChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsLogarithmic"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsLogarithmicChanged;

        /// <summary>
        /// Event that is raised when <see cref="LogBase"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LogBaseChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsCalculateFormulaEnabled"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsCalculateFormulaEnabledChanged;

        /// <summary>
        /// Event that is raised when <see cref="CalculateFormula"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CalculateFormulaChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsRelativeAngle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsRelativeAngleChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(CircularLabelTick), new FrameworkPropertyMetadata(new FontFamily(), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(CircularLabelTick), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(CircularLabelTick), new FrameworkPropertyMetadata(new FontWeight()));

        /// <summary>
        /// Identifies the <see cref="IncludeFirstValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncludeFirstValueProperty =
            DependencyProperty.Register("IncludeFirstValue", typeof(bool), typeof(CircularLabelTick), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="NumberFormatInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberFormatInfoProperty =
            DependencyProperty.Register("NumberFormatInfo", typeof(NumberFormatInfo), typeof(CircularLabelTick), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnNumberFormatInfoChanged)));

        /// <summary>
        /// Identifies the <see cref="IsLogarithmic"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLogarithmicProperty =
            DependencyProperty.Register("IsLogEnabled", typeof(bool), typeof(CircularLabelTick), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsLogarithmicChanged)));

        /// <summary>
        /// Identifies the <see cref="LogBase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LogBaseProperty =
           DependencyProperty.Register("LogBase", typeof(double), typeof(CircularLabelTick), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLogBaseChanged)));

        /// <summary>
        /// Identifies the <see cref="IsCalculateFormulaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCalculateFormulaEnabledProperty =
            DependencyProperty.Register("IsCalculateFormulaEnabled", typeof(bool), typeof(CircularLabelTick), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsCalculateFormulaEnabledChanged)));

        /// <summary>
        /// Identifies the <see cref="CalculateFormula"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalculateFormulaProperty =
           DependencyProperty.Register("CalculateFormula", typeof(string), typeof(CircularLabelTick), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnCalculateFormulaChanged)));

        /// <summary>
        /// Identifies the <see cref="IsRelativeAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRelativeAngleProperty =
            DependencyProperty.Register("IsRelativeAngle", typeof(bool), typeof(CircularLabelTick), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsRelativeAngleChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the font family for the labels.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/>
        /// </value>
        /// <seealso cref="FontFamily"/>
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
        /// Gets or sets the font size for the labels.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 10.
        /// </value>
        /// <seealso cref="double"/>
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
        /// Gets or sets the font weight for the labels.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontWeight"/>
        /// </value>
        /// <seealso cref="FontWeight"/>
        public new FontWeight FontWeight
        {
            get
            {
                return (FontWeight)GetValue(FontWeightProperty);
            }

            set
            {
                SetValue(FontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include first tick value.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        public bool IncludeFirstValue
        {
            get
            {
                return (bool)GetValue(IncludeFirstValueProperty);
            }

            set
            {
                SetValue(IncludeFirstValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Custom's NumberFormatInfo value
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Globalization.NumberFormatInfo"/>
        /// Default value is "null".
        /// </value>
        public NumberFormatInfo NumberFormatInfo
        {
            get
            {
                return (NumberFormatInfo)GetValue(NumberFormatInfoProperty);
            }

            set
            {
                SetValue(NumberFormatInfoProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsCalculateFormula value is true or false,
        /// so that label values calculated using a formula were displayed on the scale.This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default IsCalculateFormulaEnabled is "False".
        /// </value>
        /// <seealso cref="CalculateFormula"/>
        public bool IsCalculateFormulaEnabled
        {
            get
            {
                return (bool)GetValue(IsCalculateFormulaEnabledProperty);
            }

            set
            {
                SetValue(IsCalculateFormulaEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Formula string that can be used to calculate the label values.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default CalculateFormula is string.Empty.
        /// </value>
        /// <seealso cref="IsCalculateFormulaEnabled"/>
        public string CalculateFormula
        {
            get
            {
                return (string)GetValue(CalculateFormulaProperty);
            }

            set
            {
                SetValue(CalculateFormulaProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsLogarithmic value is true or false,
        /// so that log values can be displayed on the scale.This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default IsLogarithmic is "False".
        /// </value>
        /// <seealso cref="LogBase"/>
        public bool IsLogarithmic
        {
            get
            {
                return (bool)GetValue(IsLogarithmicProperty);
            }

            set
            {
                SetValue(IsLogarithmicProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LogBase value that can be displayed in the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default LogBase is 10.
        /// </value>
        /// <seealso cref="IsLogarithmic"/>
        public double LogBase
        {
            get
            {
                return (double)GetValue(LogBaseProperty);
            }

            set
            {
                SetValue(LogBaseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the label's angle is relative to that of the 
        /// scale's angle
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default IsRelativeAngle is "False".
        /// </value>
        /// <seealso cref="CalculateFormula"/>
        public bool IsRelativeAngle
        {
            get
            {
                return (bool)GetValue(IsRelativeAngleProperty);
            }

            set
            {
                SetValue(IsRelativeAngleProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CircularLabelTick"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static CircularLabelTick()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularLabelTick), new FrameworkPropertyMetadata(typeof(CircularLabelTick)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularLabelTick"/> class.
        /// </summary>
        public CircularLabelTick()
        {
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements and determines a size
        /// for the element.
        /// </summary>
        /// <remarks>
        /// Assuming the Scale's Maximum value will be the longest string, the required size
        /// is calculated based on the longest string's size.
        /// </remarks>
        /// <param name="constraint">The available size that this element can give to child
        /// elements.</param>
        /// <returns>
        /// The size that this element determines it needs during layout.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            CircularScale scale = this.VisualParent as CircularScale;
            Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
            FormattedText formattedText = new FormattedText(scale.Maximum.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.BackgroundBrush);
            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <remarks>
        /// Number of Segments that are possible of &quot;interval&quot; length is
        /// calculated by the variable ratio. Angleinterval calculates the angle interval
        /// between two such segments. As each tick is placed the angleToPlace and the
        /// labelvalues are incremented by angleinterval and interval respectively.If
        /// IsLogarithmic is set to True, the label values are converted into Log values
        /// before getting displayed. The labels are then rotated to the appropriate angle
        /// and to appropriate location(based on ScalePlacement).
        /// </remarks>
        /// <param name="drawingContext">The drawing instructions for a specific
        /// element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.VisualParent is CircularScale)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                double angleToPlace = scale.ScaleDirection == ScaleDirection.Clockwise ? scale.StartAngle : scale.StartAngle + scale.GapSweepAngle;
                double interval = 0;
                double labelValue = scale.Minimum;
                double newLabelValue = labelValue;
                double ratio = 0d;
                if (this.TickStyle == TickStyle.MajorTick)
                {
                    interval = scale.IsNumberDivision ? (scale.Maximum - scale.Minimum) / scale.NumberMajorDivision : scale.MajorIntervalValue;
                    ratio = (scale.Maximum - scale.Minimum) / interval;
                }
                else
                {
                    interval = scale.IsNumberDivision ? scale.NumberMajorDivision * scale.NumberMinorDivision : scale.MinorIntervalValue;
                    ratio = scale.IsNumberDivision ? interval:(scale.Maximum - scale.Minimum) / interval;
                }

                
                double angleInterval = scale.GapSweepAngle / ratio;
                Brush currentBrush = this.BackgroundBrush;

                if (angleInterval > 0)
                {
                    int errorValue = 0;
                    string value = string.Empty;
                    if (!this.IncludeFirstValue)
                    {
                        angleToPlace += angleInterval;
                        labelValue += interval;
                        newLabelValue = labelValue;
                        ratio--;
                    }

                    double k = Math.Ceiling(ratio) - ratio;
                    if (k > 0.5)
                    {
                        k = ratio - Math.Floor(ratio);
                    }

                    while (ratio >= k)
                    {
                        ratio--;
                        if (errorValue == 0)
                        {
                            if (this.IsCalculateFormulaEnabled == true)
                            {
                                if (this.CalculateFormula != string.Empty)
                                {
                                    string temp = string.Empty;
                                    if (newLabelValue < 0)
                                    {
                                        temp = GaugeBase.CalculateLabel("!" + (-newLabelValue).ToString(), this.CalculateFormula);
                                    }
                                    else
                                    {
                                        temp = GaugeBase.CalculateLabel(newLabelValue.ToString(), this.CalculateFormula);
                                    }

                                    if (temp == "() ERR" || temp == "x !PRESENT")
                                    {
                                        if (temp == "() ERR")
                                        {
                                            errorValue = 1;
                                        }
                                        else
                                        {
                                            errorValue = 2;
                                        }
                                    }
                                    else
                                    {
                                        newLabelValue = Convert.ToDouble(temp);
                                    }
                                }
                            }

                            NumberFormatInfo newformat = Thread.CurrentThread.CurrentUICulture.NumberFormat;
                            NumberFormatInfo format = (NumberFormatInfo)newformat.Clone();
                            if (this.NumberFormatInfo == null)
                            {
                                format.NumberDecimalDigits = (this.IsLogarithmic == true) ? this.GetLogDecimalCountNumber(Math.Log(newLabelValue, this.LogBase)) : this.GetDecimalCountNumber(newLabelValue);
                            }
                            else
                            {
                                format = this.NumberFormatInfo;
                            }

                            if (this.IsLogarithmic == true)
                            {
                                value = Math.Log(newLabelValue, this.LogBase).ToString("N", format);
                            }
                            else
                            {
                                value = newLabelValue.ToString("N", format);
                            }
                        }

                        if (errorValue == 1)
                        {
                            value = "() ERR";
                        }
                        else if (errorValue == 2)
                        {
                            value = "x !PRESENT";
                        }

                        if (this.RangedBrush != null)
                        {
                            if (labelValue >= RangedBrushStartValue && labelValue <= RangedBrushEndValue)
                            {
                                currentBrush = this.RangedBrush;
                            }
                            else
                            {
                                currentBrush = this.BackgroundBrush;
                            }
                        }

                        Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
                        FormattedText formattedText = new FormattedText(value, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, currentBrush);
                        double radius = 0;
                        double length = formattedText.Width > formattedText.Height ? formattedText.Width : formattedText.Height;
                        if (this.TickPlacement == ScalePlacement.Cross)
                        {
                            radius = scale.Radius - (scale.ScaleBarSize / 2) + this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Outside)
                        {
                            radius = scale.Radius + (length / 2) + this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Inside)
                        {
                            radius = scale.Radius - scale.ScaleBarSize - (length / 2) - this.DistanceFromScale;
                        }

                        Point pointToPlace = scale.ConvertToStageCoordinates(radius, FlowDirection == FlowDirection.LeftToRight ? angleToPlace : 180 - angleToPlace, new Point(-formattedText.Width / 2, -formattedText.Height / 2));

                        TransformGroup transform = new TransformGroup();
                        if (this.IsRelativeAngle == true)
                        {
                            transform.Children.Add(new RotateTransform((angleToPlace - 90) + this.Angle, pointToPlace.X + (formattedText.Width / 2), pointToPlace.Y + (formattedText.Height / 2)));
                        }
                        else
                        {
                            transform.Children.Add(new RotateTransform(this.Angle, pointToPlace.X + (formattedText.Width / 2), pointToPlace.Y + (formattedText.Height / 2)));
                        }
                        if (FlowDirection == FlowDirection.RightToLeft)
                        {
                            transform.Children.Add(new ScaleTransform { ScaleX  = -1});
                        }

                        drawingContext.PushTransform(transform);

                        drawingContext.DrawText(formattedText, pointToPlace);

                        drawingContext.Pop();
                        if (scale.ScaleDirection == ScaleDirection.Clockwise)
                            angleToPlace += angleInterval;
                        else
                            angleToPlace -= angleInterval;
                        labelValue += interval;
                        newLabelValue = labelValue;
                    }
                }
            }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Gets count of decimal digits from the double number.
        /// </summary>
        /// <remarks>
        /// For esthetic view, maximum count of decimal digits(i.e Precision) is 3.
        /// </remarks>
        /// <param name="value">Double number.</param>
        /// <returns>
        /// Count of decimal digits.
        /// </returns>
        private int GetDecimalCountNumber(double value)
        {
            int count = 3;
            double n = Math.Pow(10, count);
            value *= n;

            while (count != 0)
            {
                double lastDigit = value % 10;
                if (lastDigit != 0)
                {
                    break;
                }

                value /= 10;
                count--;
            }

            return count;
        }

        /// <summary>
        /// Count the number of decimal digits for Log values.
        /// </summary>
        /// <param name="value">Double number.</param>
        /// <returns>Actual Count of decimal digits for Log Values.</returns>
        private int GetLogDecimalCountNumber(double value)
        {
            int count;
            int dotIndex;
            int endIndex;
            string val = value.ToString();
            dotIndex = val.IndexOf(".");
            if (dotIndex != -1)
            {
                endIndex = val.Length;
                count = endIndex - (dotIndex + 1);
            }
            else
            {
                count = 0;
            }

            return count;
        }

        /// <summary>
        /// Gets the length of the text in pixels.
        /// </summary>
        /// <param name="formattedText">Text to get the length of.</param>
        /// <returns>Lenght of the text in pixels.</returns>
        private double GetTextLength(FormattedText formattedText)
        {
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = formattedText.Text;
            return 0;
        }

        /// <summary>
        /// Calls OnNumberFormatInfoChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnNumberFormatInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularLabelTick instance = (CircularLabelTick)d;
            instance.OnNumberFormatInfoChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="NumberFormatInfoChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnNumberFormatInfoChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (NumberFormatInfoChanged != null)
            {
                NumberFormatInfoChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsLogarithmicChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsLogarithmicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularLabelTick instance = (CircularLabelTick)d;
            instance.OnIsLogarithmicChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsLogarithmicChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsLogarithmicChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (IsLogarithmicChanged != null)
            {
                IsLogarithmicChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLogBaseChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLogBaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularLabelTick instance = (CircularLabelTick)d;
            instance.OnLogBaseChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LogBaseChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnLogBaseChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (LogBaseChanged != null)
            {
                LogBaseChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsCalculateFormulaEnabledChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsCalculateFormulaEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularLabelTick instance = (CircularLabelTick)d;
            instance.OnIsCalculateFormulaEnabled(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsCalculateFormulaEnabledChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsCalculateFormulaEnabled(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (IsCalculateFormulaEnabledChanged != null)
            {
                IsCalculateFormulaEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCalculateFormulaChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCalculateFormulaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularLabelTick instance = (CircularLabelTick)d;
            instance.OnCalculateFormulaChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CalculateFormulaChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCalculateFormulaChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (CalculateFormulaChanged != null)
            {
                CalculateFormulaChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsRelativeAngleChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsRelativeAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularLabelTick instance = (CircularLabelTick)d;
            instance.OnIsRelativeAngleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsRelativeAngleChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsRelativeAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (IsRelativeAngleChanged != null)
            {
                IsRelativeAngleChanged(this, e);
            }
        }
        #endregion Implementation

        #region NameScope
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
        #endregion NameScope
    }
}
