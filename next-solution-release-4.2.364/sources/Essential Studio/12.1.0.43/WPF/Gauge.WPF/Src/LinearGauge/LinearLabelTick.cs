// <copyright file="LinearLabelTick.cs" company="Syncfusion Software">
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the numeric Label ticks of the linear scale.
    /// </summary>
    /// <remarks>
    /// The LinearLabelTick can made to display Labels for either the <see cref="TickStyle.MajorTick"/> or 
    /// <see cref="TickStyle.MinorTick"/>. The Ticks to which the labels are placed is set by using the 
    /// <see cref="TickBase.TickStyle"/> property.
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearLabelTickSample.Window1" Title="LinearLabelTickSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///             <syncfusion:LinearGauge.Scales>
    ///                 <syncfusion:LinearScale Name="LinearScale" Minimum="0" Maximum="100" 
    ///                                         MinorIntervalValue="2" MajorIntervalValue="10" 
    ///                                         ScaleBarSize="20" ScaleBarLength="260">
    ///                     <syncfusion:LinearScale.Ticks>
    ///                         <syncfusion:LinearLabelTick FontSize="10" TickStyle="MajorTick" 
    ///                                                     BackgroundBrush="White" TickPlacement="Inside" 
    ///                                                     DistanceFromScale="5" /> 
    ///                     </syncfusion:LinearScale.Ticks>
    ///                 </syncfusion:LinearScale>
    ///             </syncfusion:LinearGauge.Scales>
    ///         </syncfusion:LinearGauge>
    ///     </Grid>
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
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace LinearLabelTickSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private LinearGauge linearGauge1;
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 linearGauge1 = new LinearGauge();
    ///                 this.linearGauge1.CenterFrameFillColor = Colors.Brown;<para/>
    ///                 LinearScale scale = new LinearScale();
    ///                 scale.Minimum = 0;
    ///                 scale.Maximum = 100;
    ///                 scale.MinorIntervalValue = 2;
    ///                 scale.MajorIntervalValue = 10;
    ///                 scale.ScaleBarSize = 20;
    ///                 scale.ScaleBarLength = 260;
    ///                 linearGauge1.Scales.Add(scale);<para/>
    ///                 LinearLabelTick labelTick = new LinearLabelTick();
    ///                 labelTick.FontSize = 10;
    ///                 labelTick.TickStyle = TickStyle.MajorTick;
    ///                 labelTick.BackgroundBrush = new SolidColorBrush(Colors.GhostWhite);
    ///                 labelTick.TickPlacement = ScalePlacement.Inside;
    ///                 labelTick.DistanceFromScale = 5;
    ///                 scale.Ticks.Add(labelTick);<para/>
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearLabelTick : TickBase
    {
        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_sizeRatio;

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="IsCalculateFormulaEnabled"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsCalculateFormulaEnabledChanged;

        /// <summary>
        /// Event that is raised when <see cref="CalculateFormula"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CalculateFormulaChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsLogarithmic"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsLogarithmicChanged;

        /// <summary>
        /// Event that is raised when <see cref="LogBase"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LogBaseChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberFormatInfo"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberFormatInfoChanged;

        /// <summary>
        /// Event that is raised when <see cref="IncludeFirstValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IncludeFirstValueChanged;

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
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(LinearLabelTick), new FrameworkPropertyMetadata(new FontFamily()));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(LinearLabelTick), new FrameworkPropertyMetadata(10d));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(LinearLabelTick), new FrameworkPropertyMetadata(new FontWeight()));

        /// <summary>
        /// Identifies the <see cref="IncludeFirstValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncludeFirstValueProperty =
            DependencyProperty.Register("IncludeFirstValue", typeof(bool), typeof(LinearLabelTick), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIncludeFirstValueChanged)));

        /// <summary>
        /// Identifies the <see cref="NumberFormatInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberFormatInfoProperty =
            DependencyProperty.Register("NumberFormatInfo", typeof(NumberFormatInfo), typeof(LinearLabelTick), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnNumberFormatInfoChanged)));

        /// <summary>
        /// Identifies the <see cref="IsLogarithmic"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLogarithmicProperty =
            DependencyProperty.Register("IsLogEnabled", typeof(bool), typeof(LinearLabelTick), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsLogarithmicChanged)));

        /// <summary>
        /// Identifies the <see cref="LogBase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LogBaseProperty =
           DependencyProperty.Register("LogBase", typeof(double), typeof(LinearLabelTick), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLogBaseChanged)));

        /// <summary>
        /// Identifies the <see cref="IsCalculateFormulaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCalculateFormulaEnabledProperty =
            DependencyProperty.Register("IsCalculateFormulaEnabled", typeof(bool), typeof(LinearLabelTick), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsCalculateFormulaEnabledChanged)));

        /// <summary>
        /// Identifies the <see cref="CalculateFormula"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalculateFormulaProperty =
           DependencyProperty.Register("CalculateFormula", typeof(string), typeof(LinearLabelTick), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnCalculateFormulaChanged)));

        /// <summary>
        /// Identifies the <see cref="IsRelativeAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRelativeAngleProperty =
            DependencyProperty.Register("IsRelativeAngle", typeof(bool), typeof(LinearLabelTick), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsRelativeAngleChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets a value indicating whether IsCalculateFormula value is true or false,
        /// so that label values calculated using a formula were displayed on the scale.This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default IsCalculateFormula is "False".
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
        /// Gets or sets the font family for the labels.
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
        /// Gets or sets the font size for the labels.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 10.
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
        /// Gets or sets the font weight for the labels.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontWeight"/>
        /// </value>
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

        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double SizeRatio
        {
            get
            {
                return m_sizeRatio;
            }

            set
            {
                m_sizeRatio = value;
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearLabelTick"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static LinearLabelTick()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearLabelTick), new FrameworkPropertyMetadata(typeof(LinearLabelTick)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearLabelTick"/> class.
        /// </summary>
        public LinearLabelTick()
        {
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        /// <remarks>The measuring is done by calculating the width and height of the scale's Maximum value's
        /// size, assuming Maximum label will always have the highest width and height</remarks>
        protected override Size MeasureOverride(Size constraint)
        {
            LinearScale scale = this.VisualParent as LinearScale;
            Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
            FormattedText formattedText = new FormattedText(scale.Maximum.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.BackgroundBrush);
            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>The Local Variables Used</listheader>
        ///     <item>
        ///         <term>ratio</term>
        ///         <description>stores the number of segments(relative to Maximum and Minimum values),
        ///         that can be formed,each of "valueInterval" length.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>interval</term>
        ///        <description>Stores the length of one such segment(relative to ScaleBarLength 
        ///         deducting YDistanceFromScale).
        ///         </description>
        ///     </item>
        /// </list>
        /// <list type="bullet">
        ///     <listheader>The Transforms Used</listheader>
        ///     <item>The ticks arranged at the Center of the Gauge are first transformed, to the angle set.</item>
        ///     <item>Calculate the appropriate position where the label should display itself.</item>
        ///     <item>Draw the labels at that particular position</item>
        /// </list>
        /// <list type="bullet">
        ///     <listheader>Logarithmic Details</listheader>
        ///     <item>
        ///         <term>IsLogarithmic</term>
        ///         <description>Boolean value to denote whether the label displays values in Log or in Normal 
        ///         decimal form</description>
        ///     </item>
        ///     <item>
        ///         <term>LogBase</term>
        ///         <description>Double value which stores the base value for logarithmic display of labels</description>
        ///     </item>
        /// </list>
        /// <list type="bullet">
        ///     <listheader>NumberFormatInfo</listheader>
        ///     <item>NumberDecimalDigits</item>
        ///     <description>Number of decimal digits to be displayed for the label values</description>
        /// </list>
        /// </remarks>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.VisualParent is LinearScale)
            {
                LinearScale scale = this.VisualParent as LinearScale;
                double i = 0;
                double valueInterval = 0;
                double labelValue = scale.Minimum;
                double newLabelValue = labelValue;
                double ratio = 0d;
                if (this.TickStyle == TickStyle.MajorTick)
                {
                    valueInterval = scale.IsNumberDivision ? (scale.Maximum - scale.Minimum) / scale.NumberMajorDivision : scale.MajorIntervalValue;
                    ratio = (scale.Maximum - scale.Minimum) / valueInterval;
                }
                else
                {
                    valueInterval = scale.IsNumberDivision ? scale.NumberMajorDivision * scale.NumberMinorDivision : scale.MinorIntervalValue;
                    ratio =scale.IsNumberDivision ? valueInterval: (scale.Maximum - scale.Minimum) / valueInterval;
                }

                
                double interval = (scale.ScaleBarLength - this.YDistanceFromScale) / ratio;
                Brush currentBrush = this.BackgroundBrush;

                if (interval > 0)
                {
                    int errorValue = 0;
                    if (!this.IncludeFirstValue)
                    {
                        labelValue += valueInterval;
                        newLabelValue = labelValue;
                        i += interval;
                    }

                    NumberFormatInfo newformat = CultureInfo.CurrentUICulture.NumberFormat;
                    NumberFormatInfo format = (NumberFormatInfo)newformat.Clone();
                    if (this.NumberFormatInfo == null)
                    {
                        format.NumberDecimalDigits = (this.IsLogarithmic == true) ? this.GetLogDecimalCountNumber(Math.Log(newLabelValue, this.LogBase)) : this.GetDecimalCountNumber(newLabelValue);
                    }
                    else
                    {
                        format = this.NumberFormatInfo;
                    }

                    string value;
                    if (this.IsLogarithmic == true)
                    {
                        value = Math.Log(newLabelValue, this.LogBase).ToString("N", format);
                    }
                    else
                    {
                        value = newLabelValue.ToString("N", format);
                    }

                    Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
                    FormattedText formattedText = new FormattedText(value, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.BackgroundBrush);
                    double length = 0;
                    if (scale.Orientation == GaugeOrientation.Vertical)                    
                        length = formattedText.Height;                        
                    else                   
                        length = 0;
                    i = length / 2;                    
                    while (labelValue <= scale.Maximum)
                    {
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

                        typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
                        formattedText = new FormattedText(value, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, currentBrush);                                                
                        if (scale.Orientation == GaugeOrientation.Horizontal)
                        {
                            length = formattedText.Height;
                            FormattedText temp = new FormattedText(scale.Maximum.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, currentBrush);
                            if (temp.Height > length)
                                length = temp.Height;
                            //length = formattedText.Width;
                        }
                        else
                        {
                            length = formattedText.Width;
                        }

                        double posX = 0;
                        if (this.TickPlacement == ScalePlacement.Cross)
                        {
                            posX = (-length / 2) - this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Inside)
                        {
                            posX = (-scale.ScaleBarSize / 2) - length - this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Outside)
                        {
                            posX = (scale.ScaleBarSize / 2) + this.DistanceFromScale;
                        }

                        double angleAdjust = 0;
                        if (scale.Orientation == GaugeOrientation.Horizontal)
                        {
                            FormattedText temp = new FormattedText(scale.Maximum.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, currentBrush);

                            posX -= (temp.Width / 2) - (formattedText.Height / 2);
                        }

                        Point pointToPlace = new Point(posX, scale.ScaleDirection == ScaleDirection.CounterClockwise ? (((scale.ScaleBarLength - this.YDistanceFromScale) / 2) - i) : (-((scale.ScaleBarLength - this.YDistanceFromScale) / 2) - i));
                        if (scale.Orientation == GaugeOrientation.Horizontal)
                        {
                            pointToPlace = new Point(scale.ScaleDirection == ScaleDirection.CounterClockwise ? (((scale.ScaleBarLength - this.YDistanceFromScale) / 2) - i) : (-((scale.ScaleBarLength - this.YDistanceFromScale) / 2) - i), posX);
                        }
                        TransformGroup transform = new TransformGroup();
                        if (this.IsRelativeAngle == true)
                        {
                            transform.Children.Add(new RotateTransform((this.Angle + angleAdjust) - 90, pointToPlace.X + (formattedText.Width / 2), pointToPlace.Y + (formattedText.Height / 2)));
                        }
                        else
                        {
                            transform.Children.Add(new RotateTransform(this.Angle + angleAdjust, pointToPlace.X + (formattedText.Width / 2), pointToPlace.Y + (formattedText.Height / 2)));
                        }                        
                        drawingContext.PushTransform(transform);

                        drawingContext.DrawText(formattedText, pointToPlace);
                        drawingContext.Pop();
                         if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                                i += interval;
                            else
                                i -= interval;                                              
                        labelValue += valueInterval;
                        newLabelValue = labelValue;
                    }
                }
            }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Returns the count of decimal digits from the double argument passed.<para/>
        /// Maximum count of decimal digits for decimal numbers is 3.
        /// </summary>
        /// <param name="value">Double number.</param>
        /// <returns>Count of decimal digits.</returns>
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
        /// Returns the count of decimal digits from the log argument passed.<para/> 
        /// </summary>
        /// <param name="value">Double number.</param>
        /// <returns>Count of decimal digits.</returns>
        private int GetLogDecimalCountNumber(double value)
        {
            int count;
            int dotIndex;
            int endIndex;
            string val = value.ToString();
            dotIndex = (val.IndexOf(".") == -1) ? 0 : val.IndexOf(".");
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
        /// <returns>Length of the text in pixels.</returns>
        private double GetTextLength(FormattedText formattedText)
        {
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = formattedText.Text;
            return 0;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IncludeFirstValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIncludeFirstValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IncludeFirstValueChanged != null)
            {
                this.IncludeFirstValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIncludeFirstValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIncludeFirstValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
            instance.OnIncludeFirstValueChanged(e);
        }

        /// <summary>
        /// Calls OnNumberFormatInfoChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnNumberFormatInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
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
        /// Calls OnIsLogarithmicChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsLogarithmicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
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
        /// Calls OnLogBaseChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLogBaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
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
        /// Calls OnIsCalculateFormulaEnabledChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsCalculateFormulaEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
            instance.OnIsCalculateFormulaEnabled(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsCalculateFormulaEnabledChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
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
        /// Calls OnCalculateFormulaChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCalculateFormulaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
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
        /// Calls OnIsRelativedChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsRelativeAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearLabelTick instance = (LinearLabelTick)d;
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
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="LinearGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(LinearGauge gauge)
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
        #endregion Added Code
    }
}
