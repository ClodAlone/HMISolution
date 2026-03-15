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

    /// <summary>
    /// Represents the numeric label tick for the scale. It can be displayed as major or minor tick.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge; 
    /// <para></para>
    /// <para>LabelTickSet label = new LabelTickSet();</para>
    /// <para>            label.FontSize = 15;</para>
    /// <para>            label.Foreground = new SolidColorBrush(Colors.White);</para>
    /// <para>            label.FontFamily = new FontFamily(&quot;Ariel&quot;);</para>
    /// <para>            label.DistanceFromScale = 5;</para>
    /// <para>            label.TickPlacement = ScalePlacement.Inside;</para>
    /// <para>            label.Foreground = new SolidColorBrush(Colors.Brown);</para>
    /// <para>            scale.Ticks.Add(label);</para></description></item>
    /// <item>
    /// <description><b>Xaml :xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot; </b>
    /// <para></para>
    /// <para><b>&lt;syncfusion:LabelTickSet FontSize=&quot;13&quot; Foreground=&quot;Silver&quot; Name=&quot;label&quot; TickPlacement=&quot;Outside&quot;  VerticalContentAlignment=&quot;Bottom&quot; DistanceFromScale=&quot;0&quot;/&gt;</b></para>
    /// <para><b>    </b></para></description></item>
    /// <item>
    /// <description>
    /// <para></para></description></item></list>
    /// </example>
    public class LabelTickSet : TickSetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LabelTickSet">LabelTickSet</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public LabelTickSet()
        {
            DefaultStyleKey = typeof(LabelTickSet);

        }

        #region Dependency properties
       
        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.EnableFormulaCalculation">EnableFormulaCalculation</see> Dependency property
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">System.Boolean</see>
        /// </returns>
        public static readonly DependencyProperty EnableFormulaCalculationProperty = DependencyProperty.Register("EnableFormulaCalculation", typeof(bool), typeof(LabelTickSet), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableFormulaCalculationChanged)));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.IsLogarithmic">IsLogarithmic</see> Dependency property
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">System.Boolean</see>
        /// </returns>
        public static readonly DependencyProperty IsLogarithmicProperty = DependencyProperty.Register("IsLogarithmic", typeof(bool), typeof(LabelTickSet), new PropertyMetadata(false, new PropertyChangedCallback(OnIsLogarithmicChanged)));

        /// <summary>
        /// Identifies the <see cref="T:Syncfusion.Windows.Gauge.LabelTickSet">IncludeFirstValue</see> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncludeFirstValueProperty =
            DependencyProperty.Register("IncludeFirstValue", typeof(bool), typeof(LabelTickSet), new PropertyMetadata(true, new PropertyChangedCallback(OnIncludeFirstValueChanged)));

        /// <summary>
        /// Indentifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.Foreground">Foreground</see> property
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty =
           DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(LabelTickSet), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLabelForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.LabelFormula">LabelFormula</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.String">System.String</see>
        /// </returns>
        public static readonly DependencyProperty LabelFormulaProperty = DependencyProperty.Register("LabelFormula", typeof(string), typeof(LabelTickSet), new PropertyMetadata("x", new PropertyChangedCallback(OnLabelFormulaChanged)));
        /// <summary>
        /// Identifies Label Format Dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty = DependencyProperty.Register("LabelFormat", typeof(NumberFormatInfo), typeof(LabelTickSet), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelFormatChanged)));
        /// <summary>
        /// Identifies RemoveTrailingZero Dependency property.
        /// </summary>
        public static readonly DependencyProperty RemoveTrailingZeroProperty = DependencyProperty.Register("RemoveTrailingZero", typeof(bool), typeof(LabelTickSet), new PropertyMetadata(false, new PropertyChangedCallback(OnRemoveTrailingZeroFormatChanged)));
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.LogBase">LogBase</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.String">System.double</see>
        /// </returns>
        public static readonly DependencyProperty LogBaseProperty = DependencyProperty.Register("LogBase", typeof(double), typeof(LabelTickSet), new PropertyMetadata(10d, new PropertyChangedCallback(OnLogBaseChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.Prefix">Prefix</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.String">System.String</see>
        /// </returns>
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register("Prefix", typeof(string), typeof(LabelTickSet), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPrefixChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.Suffix">Suffix</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.String">System.String</see>
        /// </returns>
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register("Suffix", typeof(string), typeof(LabelTickSet), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnSuffixChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.GaugeLabel.Text">Text</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.String">System.String</see>
        /// </returns>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabelTickSet), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.XOffset">XOffset</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        internal static readonly DependencyProperty XOffsetProperty =
            DependencyProperty.Register("XOffset", typeof(double), typeof(LabelTickSet), new PropertyMetadata(new PropertyChangedCallback(OnXOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.YOffset">YOffset</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        internal static readonly DependencyProperty YOffsetProperty =
            DependencyProperty.Register("YOffset", typeof(double), typeof(LabelTickSet), new PropertyMetadata(new PropertyChangedCallback(OnYOffsetChanged)));
      
        #endregion

        #region variable
        /// <summary>
        /// Temp Variable to validate the given formula.
        /// </summary>
        internal int Validate = 0;

        /// <summary>
        /// The text block used to draw the text.
        /// </summary>
        private TextBlock mtextBlock;

        internal int flag = 0;

        /// <summary>
        /// Stack variable for storing formula operands.
        /// </summary>
       
        private static Stack<double> varStack = new Stack<double>();

        /// <summary>
        /// Stack variable for storing operators.
        /// </summary>
        
        private static Stack<string> operatorStack = new Stack<string>();

        /// <summary>
        /// Variable for postfix expression.
        /// </summary>
        private static string postfix = null;

        internal string tooltip;
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when EnableFormulaCaculation depedency.
        /// </summary>
        public event PropertyChangedCallback EnableFormulaCalculationChanged;

        /// <summary>
        /// Event that is raised when IsLogarithmic depedency.
        /// </summary>
        public event PropertyChangedCallback IsLogarithmicChanged;

        /// <summary>
        /// Event that is raised when <see cref="F:Syncfusion.Windows.Gauge.LabelTickSet.IncludeFirstValueProperty">IncludeFirstValue</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback IncludeFirstValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.LabelForeground">LabelForeground</see> property is changed
        /// </summary>
        public event PropertyChangedCallback LabelForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.LabelFormula">LabeFormula</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelFormulaChanged;
        /// <summary>
        /// Event that is raised when LabelFormatChanged
        /// </summary>
        public event PropertyChangedCallback LabelFormatChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.LogBase">LogBase</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback LogBaseChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.Prefix">Prefix</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PrefixChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.Suffix">Suffix</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback SuffixChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.Text">Text</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        /// <summary>
        /// Event that is raised when <see cref="XOffset"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback XOffsetChanged;

        /// <summary>
        /// Event that is raised when <see cref="YOffset"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback YOffsetChanged;

        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; EnabeFormulaCalculation=&quot;true&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.EnableFormulaCalculation =&quot;true&quot;;</para>
        /// </remarks>
        public bool EnableFormulaCalculation
        {
            get
            {
                return (bool)GetValue(EnableFormulaCalculationProperty);
            }

            set
            {
                SetValue(EnableFormulaCalculationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; IsLogarithmic=&quot;true&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.IsLogarithmic =&quot;true&quot;;</para>
        /// </remarks>
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
        /// Gets or sets a value indicating whether to include first tick value. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.Boolean">System.Boolean</see>
        /// </value>
        /// <seealso cref="bool">bool</seealso>
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
        /// Gets or sets the value of the LabelForeground. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; LabelForeground=&quot;Blue&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.LabelForeground = new SolidColorBrush(Colors.Blue);</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Windows.Media.Brush">Brush</see>
        /// </value>
        public Brush LabelForeground
        {
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }

            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LabelFormula property.This is a dependencyproperty.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; LabelFormula=&quot;(x+10)&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.LabelFormula = &quot;(x+10)&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.String">System.String</see>
        /// </value>
        public string LabelFormula
        {
            get
            {
                return (string)GetValue(LabelFormulaProperty);
            }

            set
            {
                SetValue(LabelFormulaProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Label Format.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public NumberFormatInfo LabelFormat
        {
            get
            {
                return (NumberFormatInfo)GetValue(LabelFormatProperty);
            }

            set
            {
                SetValue(LabelFormatProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether Trailing zero or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool RemoveTrailingZero
        {
            get
            {
                return (bool)GetValue(RemoveTrailingZeroProperty);
            }
            set
            {
                SetValue(RemoveTrailingZeroProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LogBase property.This is a dependencyproperty.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; LogBase=&quot;2&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.LogBase =2;</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.String">System.double</see>
        /// </value>
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
        /// Gets or sets the LabelTickSet Prefix.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; Prefix=&quot;M&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.Prefix =&quot;M&quot;;</para>
        /// </remarks>
        public string Prefix
        {
            get
            {
                return (string)GetValue(PrefixProperty);
            }

            set
            {
                SetValue(PrefixProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LabelTickSet Suffix.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; Suffix=&quot;K&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.Suffix =&quot;K&quot;;</para>
        /// </remarks>
        public string Suffix
        {
            get
            {
                return (string)GetValue(SuffixProperty);
            }

            set
            {
                SetValue(SuffixProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.String">System.String</see>
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
        /// Gets or sets the x-offset of the tick. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type:  Default value is 0.
        /// </value>
        /// <seealso cref="double">double</seealso>
        internal double XOffset
        {
            get
            {
                return (double)GetValue(XOffsetProperty);
            }

            set
            {
                SetValue(XOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the y-offset of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        internal double YOffset
        {
            get
            {
                return (double)GetValue(YOffsetProperty);
            }

            set
            {
                SetValue(YOffsetProperty, value);
            }
        }
        
        #endregion

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
            this.mtextBlock = this.GetTemplateChild("PART_LabelTextBlock") as TextBlock;
            ToolTip t = new ToolTip()
            {
                Content = this.tooltip
            };
            if (this.tooltip != null)
            {
                ToolTipService.SetToolTip(this.mtextBlock, t);
            }
            else
            {
                ToolTipService.SetToolTip(this.mtextBlock, null);
            }
            if (this.mtextBlock != null)
            {
                if (this.GaugeElementParent is CircularScale)
                {
                    CircularScale scale = this.GaugeElementParent as CircularScale;
                    foreach(TickSetBase tick in scale.Ticks)
                    {
                        if (tick is LabelTickSet)
                        {
                            LabelTickSet labeltick = tick as LabelTickSet;                            
                            this.RefreshLabelTick(labeltick);
                        }
                    }
                }
            }
            this.IsTabStop = false;
            this.UpdateVisualStyle();
        }
               

        #region Implementation
        /// <summary>
        /// Refreshs Label Tick set
        /// </summary>
        internal void RefreshLabelTick(LabelTickSet labeltick)
        {
            if (this.mtextBlock != null)
            {
                    this.mtextBlock.FontFamily = labeltick.FontFamily;                
                    this.mtextBlock.FontStyle = labeltick.FontStyle;
                    this.mtextBlock.FontSize = labeltick.FontSize;
                    this.mtextBlock.FontWeight = labeltick.FontWeight;
                    ToolTip t = new ToolTip()
                    {
                        Content = this.tooltip
                    };
                    if (this.tooltip != null)
                    {
                        ToolTipService.SetToolTip(this.mtextBlock, t);
                    }
                    else
                    {
                        ToolTipService.SetToolTip(this.mtextBlock, null);
                    }
               
            }
        }

        /// <summary>
        /// Measures Size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <returns>Returns the size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            if (this.mtextBlock != null && this.mtextBlock.Text != String.Empty)
            {
                size = new Size(this.mtextBlock.ActualWidth, this.mtextBlock.ActualHeight);
            }

            return size;
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
        /// Updates property value cache and raises <see cref="XOffsetChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnXOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.XOffsetChanged != null)
            {
                this.XOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="YOffsetChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnYOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.YOffsetChanged != null)
            {
                this.YOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnYOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnYOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnYOffsetChanged(e);
        }

        /// <summary>
        /// Calls OnXOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnXOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnXOffsetChanged(e);
        }

        /// <summary>
        /// Calls OnTextChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnTextChanged(e);
        }

        
        /// <summary>
        /// Evaluates the input in postfix notation.
        /// </summary>
        /// <param name="input">The input in postfix notation.</param>
        internal static void Evaluate(string input)
        {
            try
            {
                int firstIndex;
                char[] operators = { '|', '*', '/', '%', '+', '-', '!' };
                firstIndex = input.IndexOfAny(operators);
                while (firstIndex != -1)
                {
                    if (input[0] == '|')
                    {
                        input = input.Remove(0, 1);
                    }
                    else
                    {
                        bool isNaN = false;
                        string var = input.Substring(0, firstIndex);
                        if (var == string.Empty)
                        {
                            var = input[0].ToString();
                        }

                        foreach (char ch in operators)
                        {
                            if (var[0] == ch)
                            {
                                isNaN = true;
                                break;
                            }
                        }

                        // If a number
                        if (isNaN == false)
                        {
                            varStack.Push(Convert.ToDouble(var));
                            input = input.Remove(0, firstIndex);
                        }
                        else
                        {
                            if (var != string.Empty && var[0] != '|')
                            {
                                double temp = varStack.Pop();
                                double topStack = 0;

                                if (var[0] != '!')
                                {
                                    topStack = varStack.Pop();
                                }

                                // Find the operator and Evaluate
                                switch (var)
                                {
                                    case "+":
                                        varStack.Push(topStack + temp);
                                        break;

                                    case "-":
                                        varStack.Push(topStack - temp);
                                        break;

                                    case "*":
                                        varStack.Push(topStack * temp);
                                        break;

                                    case "/":
                                        varStack.Push(topStack / temp);
                                        break;

                                    case "%":
                                        varStack.Push(topStack % temp);
                                        break;

                                    case "!":
                                        varStack.Push(-temp);
                                        break;
                                }

                                input = input.Remove(0, 1);
                            }
                            else
                            {
                                input = input.Remove(0, 1);
                            }
                        }
                    }

                    firstIndex = input.IndexOfAny(operators);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Converts Infix notation to postfix notation.
        /// </summary>
        /// <param name="input">The input which is the Infix notation.</param>
        internal static void InfixToPostfix(string input)
        {
            int firstIndex;
            operatorStack.Clear();
            varStack.Clear();
            postfix = string.Empty;
            char[] operators = { '*', '/', '+', '-', '%', '(', ')', '!' };
            firstIndex = input.IndexOfAny(operators);
            while (firstIndex != -1)
            {
                string var = string.Empty;
                if (input[0] != '(' && input[0] != ')')
                {
                    var = input.Substring(0, firstIndex);
                }
                else
                {
                    if (input[0] == '(')
                    {
                        operatorStack.Push("(");
                    }
                    else
                    {
                        while (operatorStack.Count > 0)
                        {
                            if (operatorStack.Peek() == "(")
                            {
                                operatorStack.Pop();
                                break;
                            }

                            postfix = postfix + operatorStack.Pop();
                        }
                    }

                    input = input.Remove(0, 1);
                    firstIndex = input.IndexOfAny(operators);
                    continue;
                }

                // Added to postfix with a seperator string
                postfix = postfix + "|" + var;

                if (operatorStack.Count >= 0)
                {
                    string currentOp = input[firstIndex].ToString();
                    if (currentOp == ")" || currentOp == "(")
                    {
                        input = input.Remove(0, firstIndex);
                        firstIndex = input.IndexOfAny(operators);
                        continue;
                    }

                    if (operatorStack.Count == 0)
                    {
                        operatorStack.Push(currentOp);
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && (currentOp == "+" || currentOp == "-") && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/" || operatorStack.Peek() == "%" || operatorStack.Peek() == "!"))
                        {
                            postfix = postfix + operatorStack.Pop();
                        }

                        while (operatorStack.Count > 0 && (currentOp == "*" || currentOp == "/" || currentOp == "%") && operatorStack.Peek() == "!")
                        {
                            postfix = postfix + operatorStack.Pop();
                        }

                        operatorStack.Push(currentOp);
                    }
                }

                input = input.Remove(0, firstIndex + 1);
                firstIndex = input.IndexOfAny(operators);
            }

            postfix = postfix + "|" + input;

            while (operatorStack.Count > 0)
            {
                postfix = postfix + operatorStack.Pop();
            }
        }

        /// <summary>
        /// Method for calculating LabelTickSet value.
        /// </summary>
        /// <param name="label">Labeltick value</param>
        /// <param name="formula">input formula string</param>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        internal double CalculateLabelValue(double label, string formula)
        {
            string newFormula = string.Empty;
            if (formula == "x")
            {
                return label;
            }

            if (formula == "!x")
            {
                label = -label;
                return label;
            }

            if (formula.IndexOf("!!") != -1)
            {
                formula = formula.Replace("!!", string.Empty);
            }

            if (formula.IndexOf("x") != -1)
            {
                if (label < 0)
                {
                    string str = "(0";
                    formula = formula.Replace("x", str + label.ToString() + ")");
                }
                else
                {
                    formula = formula.Replace("x", label.ToString());
                }
            }

            string[] splitted = formula.Split(' ');

            foreach (string s in splitted)
            {
                newFormula = newFormula + s;
            }

            LabelTickSet.InfixToPostfix(newFormula);
            LabelTickSet.Evaluate(LabelTickSet.postfix);
            try
            {
                return varStack.Pop();
            }
            catch
            {
                this.LabelFormula = "x*1";
                return label;
            }
        }

        internal int GetDecimalCountNumber(double value)
        {
            return GetDecimalCountNumber(value, 3);
        }

        /// <summary>
        /// Gets count of decimal digits from the double number.
        /// </summary>
        /// <remarks>
        /// For esthetic view, maximum count of decimal digits(i.e Precision) is 3.
        /// </remarks>
        /// <param name="value">Double number.</param>
        /// <param name="digits">Integer.</param>
        /// <returns>
        /// Count of decimal digits.
        /// </returns>
        internal int GetDecimalCountNumber(double value,int digits)
        {
            int count = digits;
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
        internal int GetLogDecimalCountNumber(double value)
        {
            int count;
            int dotIndex;
            int endIndex;
            string val = Math.Round(value, 5).ToString();
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
        /// Method for validating the given formula.
        /// </summary>
        /// <param name="formula">input formula string</param>
        /// <returns>
        /// Type : <see cref="T:System.Int16">System.Int</see>
        /// </returns>
        internal int ValidateFormula(string formula)
        {
            string newFormula = string.Empty;
            int count = 0;

            for (int i = 0; i < formula.Length; i++)
            {
                if (formula[i] == '(')
                {
                    count++;
                }
                else if (formula[i] == ')')
                {
                    count--;
                }
            }

            if (count != 0)
            {
                return -1;
            }

            if (formula.IndexOf("x") == -1)
            {
                return -2;
            }

            char[] operators = { '*', '/', '+', '-', '%', '!' };
            if (formula.IndexOfAny(operators) == -1)
            {
                return -3;
            }

            return 0;
        }

        /// <summary>
        /// Method called when Label tick angle changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
            }

            base.OnAngleChanged(e);
        }

        /// <summary>
        /// Method called when Label Tick Distance from scale is changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnDistanceFromScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }

            base.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="EnableFormuaCalculationChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEnableFormulaCalculationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnableFormulaCalculationChanged != null)
            {
                this.EnableFormulaCalculationChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsLogarithmicChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsLogarithmicChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLogarithmicChanged != null)
            {
                this.IsLogarithmicChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IncludeFirstValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIncludeFirstValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IncludeFirstValueChanged != null)
            {
                this.IncludeFirstValueChanged(this, e);
            }
        }

        /// <summary>
        /// Method called when Label Foreground is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details such as old value and new value.</param>
        protected virtual void OnLabelForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            
       
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }
               
            
            if (this.LabelForegroundChanged != null)
            {
                this.LabelForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LabelFormulaChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnLabelFormulaChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.LabelFormulaChanged != null)
            {
                this.LabelFormulaChanged(this, e);
            }

            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }
        }

        /// <summary>
        /// Calls when label format is changed
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        protected virtual void OnLabelFormatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.LabelFormatChanged != null)
            {
                this.LabelFormatChanged(this, e);
            }

            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LogBaseChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnLogBaseChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.LogBaseChanged != null)
            {
                this.LogBaseChanged(this, e);
            }

            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PrefixChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPrefixChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
            }

            if (this.PrefixChanged != null)
            {
                this.PrefixChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SuffixChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSuffixChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
            }

            if (this.SuffixChanged != null)
            {
                this.SuffixChanged(this, e);
            }
        }

        /// <summary>
        /// Method called when Label TickPlacement is changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnTickPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale(); 
                if (this.Parent is LinearScaleLayoutPanel)
                    (this.Parent as LinearScaleLayoutPanel).InvalidateArrange();
            }

            base.OnTickPlacementChanged(e);
        }        

        /// <summary>
        /// Notifies the changed forground value.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLabelForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            if ((e.NewValue is SolidColorBrush) && instance.flag!=4 && instance.Tag!=null)
            {
                Color c=Color.FromArgb(255,55,93,129);
                Color c1 = Color.FromArgb(255, 0, 0, 0);
                Color c2 = Color.FromArgb(255, 255, 255, 255);
                if(((SolidColorBrush)e.NewValue).Color==c && instance.Tag.ToString()=="1")
                {
                    instance.flag = 1;
                   
                }
                else if (((SolidColorBrush)e.NewValue).Color == c2 && instance.Tag.ToString() == "2")
                {
                    instance.flag = 2;
                   
                }
                else if (((SolidColorBrush)e.NewValue).Color == c1 && instance.Tag.ToString() == "3")
                {
                    instance.flag = 3;
                }
                else
                {
                    instance.flag = 4;
                }


            }
            else
            {
                instance.flag = 4;
            }
            instance.OnLabelForegroundChanged(e);
        }

        /// <summary>
        /// Calls OnEnableFormulaCalculationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableFormulaCalculationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnEnableFormulaCalculationChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnShowToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }

            base.OnShowToolTipChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnTooltipTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }

            base.OnTooltipTextChanged(e);
        }

        /// <summary>
        /// Calls OnIsLogarithmicChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsLogarithmicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnIsLogarithmicChanged(e);
        }      

        /// <summary>
        /// Calls OnIncludeFirstValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIncludeFirstValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnIncludeFirstValueChanged(e);
        }

        /// <summary>
        /// Calls OnLabelFormulaChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLabelFormulaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //LabelTickSet instance = (LabelTickSet)d;
            //instance.Validate = instance.ValidateFormula(instance.LabelFormula);
            //if (instance.Validate == -1)
            //{
            //    MessageBox.Show("Parentheses Error.Please enter a valid formula");
            //    instance.LabelFormula = e.OldValue.ToString();
            //}
            //else if (instance.Validate == -2)
            //{
            //    MessageBox.Show("x variable missing.Please enter a valid formula");
            //    instance.LabelFormula = e.OldValue.ToString();
            //}
            //else if (instance.Validate == -3)
            //{
            //    MessageBox.Show("Binary operator missing.Please enter a valid formula");
            //    instance.LabelFormula = e.OldValue.ToString();
            //}
            //instance.OnLabelFormulaChanged(e);

            LabelTickSet instance = (LabelTickSet)d;
            instance.Validate = instance.ValidateFormula(instance.LabelFormula);
            if (instance.Validate != 0)
            {
                instance.Validate = instance.ValidateFormula(e.OldValue.ToString());
                if (instance.Validate == 0)
                {
                    instance.LabelFormula = e.OldValue.ToString();
                }
                else
                {
                    instance.LabelFormula = "x*1";
                }
            }
            instance.OnLabelFormulaChanged(e);
        }

        private static void OnLabelFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnLabelFormatChanged(e);
        }

        private static void OnRemoveTrailingZeroFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            if (instance.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = instance.GaugeElementParent as ScaleBase;
                scale.RefreshLabelTickSet();
                scale.RefreshScale();
            }
        }

        /// <summary>
        /// Calls OnLogBaseChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLogBaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnLogBaseChanged(e);
        }

        /// <summary>
        /// Calls OnPrefixChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPrefixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnPrefixChanged(e);
        }

        /// <summary>
        /// Calls OnSuffixChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSuffixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelTickSet instance = (LabelTickSet)d;
            instance.OnSuffixChanged(e);
        }

        #endregion
    }
}
