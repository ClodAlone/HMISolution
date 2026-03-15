#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Diagnostics;

    /// <summary>
    /// Represents a Numeric UpDown Control
    /// </summary>
    /// <example>
    /// <b>Creating NumericUpDown Control in XAML</b>
    /// <para></para>
    /// <code>&lt;UserControl x:Class=&quot;SilverlightSampleBrowser.NumericUpDownDemo&quot;
    ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
    ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
    ///     xmlns:syncfusion=&quot;clr-     namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight&quot;     Height=&quot;600&quot; Width=&quot;850&quot;&gt;</code>
    /// <para></para>
    /// <code>&lt;StackPanel x:Name=&quot;StackPanel1&quot;&gt;
    /// &lt;syncfusion:NumericUpDown Name=&quot;numericUpDown1&quot; Width=&quot;125&quot; Height=&quot;30&quot;  VerticalAlignment=&quot;Center&quot;    &gt;&lt;/syncfusion:NumericUpDown&gt;&lt;/StackPanel&gt;
    /// &lt;/UserControl&gt;</code>
    /// <para></para>
    /// <para></para>
    /// <para><b>Creating DomainUpDown Control using C#</b></para>
    /// <para> public partial class NumericUpDownDemo : UserControl</para>
    /// <para> {</para>
    /// <para>        publicNumericUpDownDemo</para>
    /// <para>        {</para>
    /// <para>            InitializeComponent();</para>
    /// <para>             NumericdomainUpDown= new NumericUpDown ();</para>
    /// <para>StackPanel1.Children.Add(numericUpDown);</para>
    /// <para>       }</para>
    /// <para>     } </para>
    /// <para>   }</para>
    /// </example>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Blend;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Office2007Black;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Default;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
      Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Office2010Black;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Windows7;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.VS2010;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Metro;component/NumericUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(NumericUpDown), XamlResource = "/Syncfusion.Theming.Transparent;component/NumericUpDown.xaml")]  
    public class NumericUpDown : TextBox
    {
        #region Public Dependency Properties
        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.AllowEditProperty">AllowEdit</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true, new PropertyChangedCallback(OnAllowEditChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.CornerRadiusProperty">CornerRadius</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NumericUpDown), new PropertyMetadata(new CornerRadius(1), OnCornerRadiusChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.DownButtonTemplateProperty">DownButtonTemplate</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty DownButtonTemplateProperty = DependencyProperty.Register("DownButtonTemplate", typeof(DataTemplate), typeof(NumericUpDown), new PropertyMetadata(null, new PropertyChangedCallback(OnDownButtonTemplateChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.FlowDirectionProperty">FlowDirection</see>
        /// dependency Property
        /// </summary>
        public new static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(EnumFlowDirection), typeof(NumericUpDown), new PropertyMetadata(EnumFlowDirection.LeftToRight, OnFlowDirectionChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.ForegroundProperty">Foreground</see>
        /// dependency Property
        /// </summary>
        public static new readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(NumericUpDown), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnForegroundChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.IntervalProperty">Interval</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0.1, new PropertyChangedCallback(OnIntervalChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.IsFocusedProperty">IsFocused</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register("IsFocused", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false, new PropertyChangedCallback(OnIsFocusedChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.MaxValueProperty">MaxValue</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(OnMaxValueChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.MinValueProperty">MinValue</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.NegativeForegroundProperty">NegativeForeground</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty NegativeForegroundProperty = DependencyProperty.Register("NegativeForeground", typeof(Brush), typeof(NumericUpDown), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnNegativeForegroundChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.TextAlignmentProperty">TextAlignment</see>
        /// dependency Property
        /// </summary>
        public new static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(NumericUpDown), new PropertyMetadata(TextAlignment.Left, OnTextAlignmentChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.TextProperty">Text</see>
        /// dependency Property
        /// </summary>
        public new static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(NumericUpDown), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.UpButtonTemplateProperty">UpButtonTemplate</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty UpButtonTemplateProperty = DependencyProperty.Register("UpButtonTemplate", typeof(DataTemplate), typeof(NumericUpDown), new PropertyMetadata(null, new PropertyChangedCallback(OnUpButtonTemplateChanged)));

        #endregion Public Dependency Properties

        #region Internal dependency Properties
        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.NumericUpDown.NumberFormatInfoProperty">NumberFormatInfo</see>
        /// dependency Property
        /// </summary>
      internal static readonly DependencyProperty NumberFormatInfoProperty = DependencyProperty.Register("NumberFormatInfo", typeof(NumberFormatInfo), typeof(NumericUpDown), new PropertyMetadata(CultureInfo.CurrentCulture.NumberFormat.Clone(), new PropertyChangedCallback(OnNumberFormatInfoChanged)));
      /// <summary>
      /// Identifies the <see cref="ThemeTag"/> dependency property.
      /// </summary>
      internal static readonly DependencyProperty ThemeTagProperty = DependencyProperty.Register("ThemeTag", typeof(int), typeof(NumericUpDown), new PropertyMetadata(2, new PropertyChangedCallback(OnThemeChanged)));

      /// <summary>
      /// Gets or sets the ThemeTagProperty.
      /// </summary>
      public int ThemeTag
      {
          get
          {
              return (int)GetValue(ThemeTagProperty);
          }

          set
          {
              SetValue(ThemeTagProperty, value);
          }
      }
        #endregion Internal Dependency Properties

        #region Private Fields

        /// <summary>
        /// The Repeat Button used to lowering down the value
        /// </summary>
        private RepeatButton downbutton;

        /// <summary>
        /// The Repeat Button used for increasing the value
        /// </summary>
        private RepeatButton upbutton;

        /// <summary>
        /// The TextBox used to display the value
        /// </summary>
        private TextBox textBox;

        /// <summary>
        /// The SelectedText
        /// </summary>
        private string selectedText;

        /// <summary>
        /// The boolean variable used to indicate whether cursor inside the control
        /// </summary>
        private bool mouseflag = false;

        /// <summary>
        /// The Main Grid of the control
        /// </summary>
        private Grid maingrid;

        private string latestText = string.Empty;

        private bool subflag=false;
        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.NumericUpDown">NumericUpDown</see> class
        /// </summary>
        public NumericUpDown()
        {
            base.DefaultStyleKey = typeof(NumericUpDown);
            this.AllowEdit = true;
            this.LostFocus += new RoutedEventHandler(this.OnNumericUpDownLostFocus);
            this.GotFocus += new RoutedEventHandler(this.OnNumericUpDownGotFocus);
            this.MouseEnter += new MouseEventHandler(this.NumericUpDown_MouseEnter);
            this.MouseLeave += new MouseEventHandler(this.NumericUpDown_MouseLeave);
        }       
        private void copy()
        {
            Clipboard.SetText(this.SelectText);
        }

        private void Paste()
        {
            if (this.IsReadOnly == false || this.AllowEdit)
            {
                double val1;
                int index1 = this.textBox.SelectionStart;
                NumberFormatInfo numberFormat = this.GetNumberFormatInfo();
                double.TryParse(this.textBox.Text, out val1);                
                bool seperatorFlag = false;
                if (numberFormat.NumberDecimalSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                {
                    val1 = (double.Parse(this.Text.Replace(numberFormat.NumberDecimalSeparator, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)));
                    seperatorFlag = true;
                }
                if (val1 > this.MaxValue || val1 < this.MinValue)
                {
                    if (val1 > this.MaxValue)
                    {
                        val1 = this.MaxValue;
                    }
                    if (val1 < this.MinValue)
                    {
                        val1 = this.MinValue;
                    }
                    this.Text = val1.ToString("N", numberFormat);
                    this.textBox.Text = val1.ToString("N", numberFormat);
                    this.textBox.SelectionStart = index1;
                    double.TryParse(this.Text, out val1);
                    this.Value = val1;
                }
                else
                {
                    if (!double.IsNaN(val1))
                    {
                        this.Value = val1;
                        if (seperatorFlag == true)
                        {
                            this.Text = val1.ToString("N", numberFormat);
                            this.textBox.Text = val1.ToString("N", numberFormat);
                        }
                        this.Text = val1.ToString("N", numberFormat);
                        this.textBox.SelectionStart = index1;
                    }
                }
                seperatorFlag = false;
            }

        }

        private void cut()
        {
            if (this.SelectText.Length > 0)
            {
                Clipboard.SetText(this.SelectText);
            }
        }
        /// <summary>
        /// Initializes the <see cref="NumericUpDown"/> class.
        /// </summary>
        static NumericUpDown()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }

        }
        #endregion Constructor

        #region Public Events
        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.AllowEdit">AllowEdit</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback AllowEditChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.CornerRadius">CornerRadius</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.DownButtonTemplate">DownButtonTemplate</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback DownButtonTemplateChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.FlowDirection">FlowDirection</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FlowDirectionChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.Foreground">Foreground</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback ForegroundChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.Interval">Interval</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback IntervalChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.IsFocused">IsFocused</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback IsFocusedChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.MaxValue">MaxValue</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback MaxValueChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.MinValue">MinValue</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback MinValueChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.NegativeForeground">NegativeForeground</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback NegativeForegroundChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.TextAlignment">TextAlignment</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback TextAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.Text">Text</see>
        /// is changed.
        /// </summary>
        public new event PropertyChangedCallback TextChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.UpButtonTemplate">UpButtonTemplate</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback UpButtonTemplateChanged;

        #endregion Public Events

        #region Internal Events

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.NumericUpDown.NumberFormatInfo">NumberFormatInfo</see>
        /// is changed.
        /// </summary>
        internal event PropertyChangedCallback NumberFormatInfoChanged;

        #endregion Internal Events

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the editing is allowed or not
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:NumericUpDown Name=&quot;numericupdown&quot;
        /// AllowEdit=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.AllowEdit=true;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool AllowEdit
        {
            get
            {
                return (bool)base.GetValue(AllowEditProperty);
            }

            set
            {
                base.SetValue(AllowEditProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the CornerRadius
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:NumericUpdown Name=&quot;numericupdown&quot;
        /// CornerRadius=&quot;2&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.CornerRadius=2.0;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.double.aspx">System.double</a>
        /// </value>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the  DownButtonTemplate
        /// </summary>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.windows.datatemplate.aspx">System.Windows.DataTemplate</a>
        /// </value>
        public DataTemplate DownButtonTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(DownButtonTemplateProperty);
            }

            set
            {
                base.SetValue(DownButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the FlowDirection for the control
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;syncfusion:NumericUpDown  Name=&quot;numericupdown&quot;
        /// FlowDirection=&quot;LeftToRight&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.FlowDirection=FlowDirection.LeftToRight;</para>
        /// </remarks>
        public new EnumFlowDirection FlowDirection
        {
            get
            {
                return (EnumFlowDirection)GetValue(FlowDirectionProperty);
            }

            set
            {
                SetValue(FlowDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the foreground color.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:NumericUpDown Name=&quot;numericUpDown&quot;
        /// Foreground=&quot;Red&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>NumericUpDown numericUpDown=new NumericUpDown();  </para>
        /// <para>numericUpDown.Foreground=new SolidColorBrush(Colors.Red);</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.windows.media.brush.aspx">System.Windows.Media.Brush</a>
        /// </value>
        public new Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }

            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Interval
        /// </summary>
        /// <remarks>
        ///  <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:NumericUpDown Name=&quot;numericupdown&quot;
        /// Interval=&quot;10&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.Interval=10;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.double.aspx">System.Double</a>
        /// </value>
        public double Interval
        {
            get
            {
                return (double)base.GetValue(IntervalProperty);
            }

            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether control gets focus or not.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:NumericUpDown Name=&quot;numericupdown&quot;
        /// IsFocused=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.IsFocused=true;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool IsFocused
        {
            get
            {
                return (bool)base.GetValue(IsFocusedProperty);
            }

            internal set
            {
                base.SetValue(IsFocusedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicateing the maximum value. 
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para><b> </b>&lt;Syncfusion:NumericUpDown Name=&quot;numericupdown&quot;
        /// MaxValue=&quot;100&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.MaxValue=100;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.double.aspx">System.Double</a>
        /// </value>
        public double MaxValue
        {
            get
            {
                return (double)this.GetValue(MaxValueProperty);
            }

            set
            {
                this.SetValue(MaxValueProperty, value);
                //base.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the Minimum value for the control.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;syncfusion:NumeriUpDown Name=&quot;numericupdown&quot;
        /// MinValue=&quot;0&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.MinValue=0;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.double.aspx">System.Double</a>
        /// </value>
        public double MinValue
        {
            get
            {
                return (double)this.GetValue(MinValueProperty);
            }

            set
            {                
                this.SetValue(MinValueProperty, value);
                //base.SetValue(MinimumProperty, value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0.0,new PropertyChangedCallback(OnValueChanged)));



        /// <summary>
        /// Gets or sets a value indicating the foreground color for negative values.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:NumericUpDown Name=&quot;numericUpDown&quot;
        /// NegativeForeground=&quot;Red&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>NumericUpDown numericUpDown=new NumericUpDown();  </para>
        /// <para>numericUpDown.NegativeForground=new SolidColorBrush(Colors.Red);</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.windows.media.brush.aspx">System.Windows.Media.Brush</a>
        /// </value>
        public Brush NegativeForeground
        {
            get
            {
                return (Brush)GetValue(NegativeForegroundProperty);
            }

            set
            {
                SetValue(NegativeForegroundProperty, value);              
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int NumberDecimalDigits
        {
            get 
            { 
                return (int)GetValue(NumberDecimalDigitsProperty); 
            }
            set 
            { 
                SetValue(NumberDecimalDigitsProperty, value); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NumberDecimalDigitsProperty =
            DependencyProperty.Register("NumberDecimalDigits", typeof(int), typeof(NumericUpDown), new PropertyMetadata((int)CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits));
        
        

        /// <summary>
        /// Gets Number Format information
        /// </summary>
        public NumberFormatInfo NumberFormatInfo
        {
            get
            {
                NumberFormatInfo info = (NumberFormatInfo)base.GetValue(NumberFormatInfoProperty);
                if (info != null)
                {
                    return info;
                }

                return (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            }

            private set
            {
                base.SetValue(NumberFormatInfoProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating the value of the control.
        /// </summary>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.String</a>
        /// </value>
        public new string Text
        {
            get
            {
                return (string)base.GetValue(TextProperty);
            }

            internal set
            {
                this.SetValue(TextProperty, value);
                if (this.textBox != null)
                {
                    try
                    {
                        if (double.Parse(value) < 0)
                        {
                            this.textBox.Foreground = this.NegativeForeground;
                            VisualStateManager.GoToState(this, "Negative", true);
                        }
                        else
                        {
                            if (this.ThemeTag == 1)
                            {
                                this.textBox.Foreground = new SolidColorBrush(Colors.White);
                            }
                            else
                            {
                                this.textBox.Foreground = this.Foreground;
                            }
                            VisualStateManager.GoToState(this, "Positive", true);
                        }

                        this.textBox.Text = value;
                    }
                    catch
                    {
                        this.textBox.Text = value;
                        ////this.Validate(this.textBox.Text, true);
                        ////this.ContentFormating(base.Value);
                    }
                }      
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the TextAlignment.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;syncfusion:NumericUpDown Name=&quot;numericupdown&quot;
        /// TextAlignment=&quot;Left&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.TextAlignment=TextAlignment.Left; </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.windows.textalignment.aspx">System.Windows.TextAlignment</a>
        /// </value>
        public new TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }

            set
            {
                SetValue(TextAlignmentProperty, value);                
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the template for UpButtonTemplate.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;syncfusion:NumericUpDown Name=&quot;numericupdown&quot;
        /// UpButtonContentTemplate=&quot;{StaticResource TemplateKey}&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.UpButtonContentTemplate=(DateTemplate)Application.Current.Resources[&quot;Key&quot;];
        /// </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.windows.datatemplate.aspx">System.Windows.DataTemplate</a>
        /// </value>
        public DataTemplate UpButtonContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(UpButtonTemplateProperty);
            }

            set
            {
                base.SetValue(UpButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the select text.
        /// </summary>
        /// <value>The select text.</value>
        protected string SelectText
        {
            get
            {
                return this.selectedText;
            }

            set
            {
                this.selectedText = value;
            }
        }
        #endregion Public Properties

        #region Public Override Methods
      

        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.upbutton != null)
            {
                this.upbutton.Click -= new RoutedEventHandler(this.RepeatButtonClick);
            }

            if (this.downbutton != null)
            {
                this.downbutton.Click -= new RoutedEventHandler(this.RepeatButtonClick);
            }
            if (this.NumberDecimalDigits != -1)
            {
                this.NumberFormatInfo.NumberDecimalDigits = this.NumberDecimalDigits;
            }
            else
            {
                this.NumberFormatInfo.NumberDecimalDigits = (int)CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
            }
            if (this.textBox != null)
            {
                this.textBox.SelectionChanged -= new RoutedEventHandler(textBox_SelectionChanged);
                this.textBox.RemoveHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(OnTextBoxKeyDown));
                this.textBox.KeyUp -= new KeyEventHandler(this.OnTextBoxKeyUp);
                this.textBox.GotFocus -= new RoutedEventHandler(this.OnTextBoxGotFocus);
                this.textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
            }

            this.upbutton = (RepeatButton)base.GetTemplateChild("UpButton");
            this.downbutton = (RepeatButton)base.GetTemplateChild("DownButton");
            this.upbutton.Tag = "Increment";
            this.downbutton.Tag = "Decrement";
            this.upbutton.Click += new RoutedEventHandler(this.RepeatButtonClick);
            this.downbutton.Click += new RoutedEventHandler(this.RepeatButtonClick);
           
            this.textBox = (TextBox)base.GetTemplateChild("textbox");
            if (this.textBox != null)
            {
                this.textBox.SelectionChanged += new RoutedEventHandler(textBox_SelectionChanged);
                this.textBox.AddHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(OnTextBoxKeyDown), true);
                this.textBox.KeyUp += new KeyEventHandler(this.OnTextBoxKeyUp);
                this.textBox.GotFocus += new RoutedEventHandler(this.OnTextBoxGotFocus);
                this.textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
                this.textBox.IsReadOnly = false;
            }

            this.ContentFormating(this.Value);
            this.maingrid = this.GetTemplateChild("MainGrid") as Grid;
            if (this.maingrid != null)
            {
                if (this.FlowDirection == EnumFlowDirection.LeftToRight)
                {
                    this.maingrid.RowDefinitions.Clear();
                    this.maingrid.ColumnDefinitions.Clear();
                    RowDefinition r1 = new RowDefinition();
                    RowDefinition r2 = new RowDefinition();
                    ColumnDefinition c1 = new ColumnDefinition();
                    ColumnDefinition c2 = new ColumnDefinition();
                    GridLength width1 = new GridLength(8d, GridUnitType.Star);
                    GridLength width2 = new GridLength(2d, GridUnitType.Star);
                    c1.Width = width1;
                    c2.Width = width2;
                    this.maingrid.RowDefinitions.Add(r1);
                    this.maingrid.RowDefinitions.Add(r2);
                    this.maingrid.ColumnDefinitions.Add(c1);
                    this.maingrid.ColumnDefinitions.Add(c2);
                    this.textBox.SetValue(Grid.RowProperty, (int)0);
                    this.textBox.SetValue(Grid.ColumnProperty, (int)0);
                    this.textBox.SetValue(Grid.RowSpanProperty, (int)2);
                    this.upbutton.SetValue(Grid.RowProperty, (int)0);
                    this.upbutton.SetValue(Grid.ColumnProperty, (int)1);
                    this.downbutton.SetValue(Grid.RowProperty, (int)1);
                    this.downbutton.SetValue(Grid.ColumnProperty, (int)1);
                    this.upbutton.BorderThickness = new Thickness(1, 0, 0, 1);
                    this.downbutton.BorderThickness = new Thickness(1, 0, 0, 0);
                }
                else
                {
                    this.maingrid.RowDefinitions.Clear();
                    this.maingrid.ColumnDefinitions.Clear();
                    RowDefinition r1 = new RowDefinition();
                    RowDefinition r2 = new RowDefinition();
                    ColumnDefinition c1 = new ColumnDefinition();
                    ColumnDefinition c2 = new ColumnDefinition();
                    GridLength width1 = new GridLength(8d, GridUnitType.Star);
                    GridLength width2 = new GridLength(2d, GridUnitType.Star);
                    c1.Width = width1;
                    c2.Width = width2;
                    this.maingrid.RowDefinitions.Add(r1);
                    this.maingrid.RowDefinitions.Add(r2);
                    this.maingrid.ColumnDefinitions.Add(c2);
                    this.maingrid.ColumnDefinitions.Add(c1);
                    this.textBox.SetValue(Grid.RowProperty, (int)0);
                    this.textBox.SetValue(Grid.ColumnProperty, (int)1);
                    this.textBox.SetValue(Grid.RowSpanProperty, (int)2);
                    this.upbutton.SetValue(Grid.RowProperty, (int)0);
                    this.upbutton.SetValue(Grid.ColumnProperty, (int)0);
                    this.downbutton.SetValue(Grid.RowProperty, (int)1);
                    this.downbutton.SetValue(Grid.ColumnProperty, (int)0);
                    this.upbutton.BorderThickness = new Thickness(0, 0, 1, 1);
                    this.downbutton.BorderThickness = new Thickness(0, 0, 1, 0);
                }
            }

            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.DetachEvent("DOMMouseScroll", this.OnScroll);
                HtmlPage.Window.DetachEvent("onmousewheel", this.OnScroll);
                HtmlPage.Document.DetachEvent("onmousewheel", this.OnScroll);
                HtmlPage.Window.AttachEvent("DOMMouseScroll", this.OnScroll);
                HtmlPage.Window.AttachEvent("onmousewheel", this.OnScroll);
                HtmlPage.Document.AttachEvent("onmousewheel", this.OnScroll);
            }
        }

        void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.selectedText = (sender as TextBox).SelectedText;
            this.SelectionStart = (sender as TextBox).SelectionStart;
            this.SelectionLength = (sender as TextBox).SelectionLength;
        }
        #endregion Public Override

        #region Internal Methods

        /// <summary>
        /// Method that changes the value in the control
        /// </summary>
        /// <param name="interval">Indicates the step value</param>
        internal void ChangeValue(double interval)
        {
            double tempValue = this.Value + interval;
            if ((tempValue <=this.MaxValue) && (tempValue >= this.MinValue))
            {
                this.Value += interval;
            }

            this.textBox.Text = this.Value.ToString();           
            this.Select(0, this.textBox.Text.Length);
        }

        /// <summary>
        /// Provides handling for MouseEnter Event
        /// </summary>
        /// <param name="sender">The NumeriUpDown Control</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        internal void NumericUpDown_MouseEnter(object sender, MouseEventArgs e)
        {
            this.mouseflag = true;
        }

        /// <summary>
        /// Provides handling for MouseLeave Event
        /// </summary>
        /// <param name="sender">The NumeriUpDown Control</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        internal void NumericUpDown_MouseLeave(object sender, MouseEventArgs e)
        {
            this.mouseflag = false;
        }      

        /// <summary>
        /// Provides handling for MouseEnter event
        /// </summary>
        /// <param name="e">contains information about the Mouse position and source</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (!IsFocused)
            {
                VisualStateManager.GoToState(this, "MouseOver", false);
            }

            base.OnMouseEnter(e);
        }       

        /// <summary>
        /// Provides handling for MouseLeave event
        /// </summary>
        /// <param name="e">contains information about the Mouse position and source</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!IsFocused)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }

            base.OnMouseLeave(e);
        }
        #endregion Internal Methods

        #region Protected Virtual Methods

        /// <summary>
        /// Updates property value cache and raises
        /// OnAllowEditChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnAllowEditChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.textBox != null)
            {
                if (this.AllowEdit == true)
                {
                    this.textBox.IsReadOnly = false;
                }
                else
                {
                    this.textBox.IsReadOnly = true;
                }
            }

            if (this.AllowEditChanged != null)
            {
                this.AllowEditChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CornerRadiusChanged != null)
            {
                this.CornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnDownButtonTemplateChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnDownButtonTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DownButtonTemplateChanged != null)
            {
                this.DownButtonTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnFlowDirectionChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFlowDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.maingrid != null)
            {
                if (FlowDirection == EnumFlowDirection.LeftToRight)
                {
                    this.maingrid.RowDefinitions.Clear();
                    this.maingrid.ColumnDefinitions.Clear();
                    RowDefinition r1 = new RowDefinition();
                    RowDefinition r2 = new RowDefinition();
                    ColumnDefinition c1 = new ColumnDefinition();
                    ColumnDefinition c2 = new ColumnDefinition();
                    GridLength width1 = new GridLength(1, GridUnitType.Star);
                    GridLength width2 = new GridLength(25);
                    c1.Width = width1;
                    c2.Width = width2;
                    this.maingrid.RowDefinitions.Add(r1);
                    this.maingrid.RowDefinitions.Add(r2);
                    this.maingrid.ColumnDefinitions.Add(c1);
                    this.maingrid.ColumnDefinitions.Add(c2);
                    this.textBox.SetValue(Grid.RowProperty, (int)0);
                    this.textBox.SetValue(Grid.ColumnProperty, (int)0);
                    this.textBox.SetValue(Grid.RowSpanProperty, (int)2);
                    this.upbutton.SetValue(Grid.RowProperty, (int)0);
                    this.upbutton.SetValue(Grid.ColumnProperty, (int)1);
                    this.downbutton.SetValue(Grid.RowProperty, (int)1);
                    this.downbutton.SetValue(Grid.ColumnProperty, (int)1);
                    Thickness updownborder = new Thickness(1, 0, 0, 0);
                    this.upbutton.BorderThickness = updownborder;
                    this.downbutton.BorderThickness = updownborder;
                }
                else
                {
                    this.maingrid.RowDefinitions.Clear();
                    this.maingrid.ColumnDefinitions.Clear();
                    RowDefinition r1 = new RowDefinition();
                    RowDefinition r2 = new RowDefinition();
                    ColumnDefinition c1 = new ColumnDefinition();
                    ColumnDefinition c2 = new ColumnDefinition();
                    GridLength width1 = new GridLength(1, GridUnitType.Star);
                    GridLength width2 = new GridLength(25);
                    c1.Width = width1;
                    c2.Width = width2;
                    this.maingrid.RowDefinitions.Add(r1);
                    this.maingrid.RowDefinitions.Add(r2);
                    this.maingrid.ColumnDefinitions.Add(c2);
                    this.maingrid.ColumnDefinitions.Add(c1);
                    this.textBox.SetValue(Grid.RowProperty, (int)0);
                    this.textBox.SetValue(Grid.ColumnProperty, (int)1);
                    this.textBox.SetValue(Grid.RowSpanProperty, (int)2);
                    this.upbutton.SetValue(Grid.RowProperty, (int)0);
                    this.upbutton.SetValue(Grid.ColumnProperty, (int)0);
                    this.downbutton.SetValue(Grid.RowProperty, (int)1);
                    this.downbutton.SetValue(Grid.ColumnProperty, (int)0);
                    Thickness updownborder = new Thickness(0, 0, 1, 0);
                    this.upbutton.BorderThickness = updownborder;
                    this.downbutton.BorderThickness = updownborder;
                }
            }

            if (this.FlowDirectionChanged != null)
            {
                this.FlowDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnForegroundChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.textBox != null)
            {
                if (this.Value < 0)
                {
                    this.textBox.Foreground = this.NegativeForeground;
                }
                else
                {
                    if (this.ThemeTag == 1)
                    {
                        this.textBox.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        this.textBox.Foreground = this.Foreground;
                    }
                }
            }

            if (this.ForegroundChanged != null)
            {
                this.ForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnIntercalChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            double temp = this.Interval;
            if (temp < MaxValue)
            {
                this.Interval = temp;
            }
            else
            {
                this.Interval = MaxValue;
            }

            if (this.IntervalChanged != null)
            {
                this.IntervalChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnIsFocusedChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsFocusedChanged != null)
            {
                this.IsFocusedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnMaxValueChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnMaxValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //base.Maximum = this.MaxValue;
            if (this.Value > this.MaxValue)
            {
                this.Value = this.MaxValue;
            }

            if (this.MaxValueChanged != null)
            {
                this.MaxValueChanged(this, e);
            }
            this.ContentFormating(this.Value);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnMinValueChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnMinValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //base.Minimum = this.MinValue;
            if (this.Value < this.MinValue)
            {
                this.Value = this.MinValue;
            }

            if (this.MinValueChanged != null)
            {
                this.MinValueChanged(this, e);
            }
            this.ContentFormating(this.Value);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnNegativeForegroundChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnNegativeForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.textBox != null)
            {
                if (this.Value < 0)
                {
                    this.textBox.Foreground = this.NegativeForeground;
                }
                else
                {
                    if (this.ThemeTag == 1)
                    {
                        this.textBox.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        this.textBox.Foreground = this.Foreground;
                    }
                }
            }

            if (this.NegativeForegroundChanged != null)
            {
                this.NegativeForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnNumberFormatInfoChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnNumberFormatInfoChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.NumberFormatInfoChanged != null)
            {
                this.NumberFormatInfoChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnTextAlignmentChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTextAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.textBox != null)
            {
                if (this.TextAlignment == TextAlignment.Center)
                {
                    this.textBox.TextAlignment = TextAlignment.Center;
                }
                else if (this.TextAlignment == TextAlignment.Right)
                {
                    this.textBox.TextAlignment = TextAlignment.Right;
                }
                else
                {
                    this.textBox.TextAlignment = TextAlignment.Left;
                }
            }

            if (this.TextAlignmentChanged != null)
            {
                this.TextAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnTextChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnUpButtonTemplateChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnUpButtonTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.UpButtonTemplateChanged != null)
            {
                this.UpButtonTemplateChanged(this, e);
            }
        }
        #endregion Protected Virtual Methods

        //#region Protected Override
        ///// <summary>
        ///// Called when the maximum value is changed
        ///// </summary>
        ///// <param name="oldValue">Represents the Old Value</param>
        ///// <param name="newValue">Represents the New Value</param>
        //protected override void OnMaximumChanged(double oldValue, double newValue)
        //{
        //    base.OnMaximumChanged(oldValue, newValue);
        //}
        
        ///// <summary>
        ///// Called when the minimum value is changed
        ///// </summary>
        ///// <param name="oldValue">Represents the Old Value</param>
        ///// <param name="newValue">Represents the New Value</param>
        //protected override void OnMinimumChanged(double oldValue, double newValue)
        //{            
        //    base.OnMinimumChanged(oldValue, newValue);
        //}
        //#endregion Protected Override

        #region Private Static MEthods

        /// <summary>
        /// Calls OnAllowEditChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAllowEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnAllowEditChanged(e);
        }

        /// <summary>
        /// Calls OnBorderBrushChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// <summary>
        /// Calls OnCornerRadiusChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        /// </summary>
        private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)obj;
            source.OnCornerRadiusChanged(e);
        }

        /// <summary>
        /// Calls OnDownButtonTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnDownButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnDownButtonTemplateChanged(e);
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFlowDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)obj;
            source.OnFlowDirectionChanged(e);
        }

        /// <summary>
        /// Calls OnForegroundChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)obj;
            source.OnForegroundChanged(e);
        }

        /// <summary>
        /// Calls OnIntervalChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnIntervalChanged(e);
        }

        /// <summary>
        /// Calls OnIsFocusedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnIsFocusedChanged(e);
        }

        /// <summary>
        /// Calls OnMaxValueChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnMaxValueChanged(e);
        }

        /// <summary>
        /// Calls OnMinValueChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnMinValueChanged(e);
        }

        /// <summary>
        /// Calls OnNegativeForegroundChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnNegativeForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)obj;
            source.OnNegativeForegroundChanged(e);
        }

        /// <summary>
        /// Calls OnNumberFormatInfoChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnNumberFormatInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = d as NumericUpDown;
            NumberFormatInfo numberFormat = e.NewValue as NumberFormatInfo;
            try
            {
                if (string.IsNullOrEmpty(int.MinValue.ToString(numberFormat)))
                {
                    throw new ArgumentException("Invalid NumberFormatInfo");
                }
            }
            catch
            {
                throw new ArgumentException("Invalid NumberFormatInfo");
            }

            source.ContentFormating(source.Value);
        }

        /// <summary>
        /// Calls OnTextAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTextAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)obj;
            source.OnTextAlignmentChanged(e);
        }

        /// <summary>
        /// Calls OnTextChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnTextChanged(e);
        }

        /// <summary>
        /// Calls OnUpButtonTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// ..
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnUpButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown source = (NumericUpDown)d;
            source.OnUpButtonTemplateChanged(e);
        }
        int theme = 2;
        /// <summary>
        /// Called when [theme changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown instance = (NumericUpDown)d;
            instance.theme = instance.ThemeTag;

            if (instance.theme == 2)
            {
                if (instance.Text.Contains("-"))
                {
                }
                else
                {
                    instance.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                if (instance.Text.Contains("-"))
                {
                }
                else
                {
                    instance.Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        #endregion Private Static MEthods

        #region Private Methods

        /// <summary>
        /// Method that formats the content
        /// </summary>
        /// <param name="value">Value that will be formated</param>
        private void ContentFormating(double value)
        {
            NumberFormatInfo numberFormat = this.GetNumberFormatInfo();
            string numeric = value.ToString("N", numberFormat);
            this.Text = numeric;
        }

        /// <summary>
        /// Method that unformats the content
        /// </summary>
        /// <param name="value">Value that will be unformated</param>
        private void ContentUnFormating(double value)
        {
            NumberFormatInfo numberFormat = this.GetNumberFormatInfo();
            int selStart = 0;
            int selLenght = 0;
            if (this.textBox != null)
            {
                selStart = this.textBox.SelectionStart;
                selLenght = this.textBox.SelectionLength;
            }

            this.Text = value.ToString("N", numberFormat);

            if (this.textBox != null)
            {
                this.textBox.Select(selStart, selLenght);
            }
        }

        /// <summary>
        /// Method that Returns the NumberFormat Information
        /// </summary>
        /// <returns>Returns NumeberFormatInfo Object</returns>
        private NumberFormatInfo GetNumberFormatInfo()
        {
            if (this.NumberFormatInfo != null)
            {
                if (this.NumberDecimalDigits != -1)
                {
                    this.NumberFormatInfo.NumberDecimalDigits = this.NumberDecimalDigits;
                }
                else
                {
                    this.NumberFormatInfo.NumberDecimalDigits = (int)CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
                }
                return this.NumberFormatInfo;
            }
            else
            {
                return (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            }
        }

        /// <summary>
        /// Provides hanlding for GotFocus event
        /// </summary>           
        /// <param name="sender">The Numeric UpDown control</param>
        /// <param name="e">Event Argument</param>
        private void OnNumericUpDownGotFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = true;
            this.SelectText = this.Text;
            this.selectedText = this.Text;
            this.textBox.SelectAll();
            this.SelectAll();
            VisualStateManager.GoToState(this, "Focused", false);
        }

        /// <summary>
        /// Provides hanlding for LostFocus event
        /// </summary>           
        /// <param name="sender">The Numeric UpDown control</param>
        /// <param name="e">Event Argument</param>
        private void OnNumericUpDownLostFocus(object sender, RoutedEventArgs e)
        {
            //if (this.AllowEdit)
            //{
            //    this.Validate(this.textBox.Text, true);
            //    this.ContentFormating(this.Value);
            //}

            this.IsFocused = false;
            VisualStateManager.GoToState(this, "Unfocused", false);
            VisualStateManager.GoToState(this, "Normal", false);
        }

        /// <summary>
        /// The Method that handles the MouseSCroll Event
        /// </summary>
        /// <param name="sender">The NumericUPDown control</param>
        /// <param name="args">Event Argument</param>
        private void OnScroll(object sender, HtmlEventArgs args)
        {
            if (this.mouseflag)
            {
                double delta = 0;
                int sign = 1;
                ScriptObject e = args.EventObject;
                if (e.GetProperty("detail") != null)
                {
                    delta = (double)e.GetProperty("detail");
                }
                else if (e.GetProperty("wheelDelta") != null)
                {
                    delta = (double)e.GetProperty("wheelDelta");
                }

                delta = Math.Sign(delta);
                if (delta > 0)
                {
                    sign = 1;
                }

                if (delta < 0)
                {
                    sign = -1;
                }

                this.ChangeValue(sign * this.Interval);
                ContentFormating(this.Value);
            }
        }

        /// <summary>
        /// Provides handling for TextBox Gotfocus Event
        /// </summary>
        /// <param name="sender">The TextBox of the Numeric Updown control</param>
        /// <param name="e">Event ARgument</param>
        private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = true;
            this.SelectText = this.Text;
            this.selectedText = this.Text;
            this.textBox.SelectAll();
            this.SelectAll();
            VisualStateManager.GoToState(this, "Focused", false);
            //this.ContentUnFormating(base.Value);
        }


        /// <summary>
        /// Provides handling for TextBox KeyDown Event
        /// </summary>
        /// <param name="sender">The TextBox of the Numeric Updown control</param>
        /// <param name="e">Event ARgument</param>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (this.textBox.IsReadOnly || this.AllowEdit == false)
                e.Handled = true;
            else
            {
                if (ModifierKeys.Control == Keyboard.Modifiers)
                {
                    if (e.Key == Key.V)
                    {
                        Paste();
                        e.Handled = true;
                    }
                    if (e.Key == Key.C)
                    {
                        copy();
                        e.Handled = true;
                    }                    

                    if (e.Key == Key.X)
                    {
                        cut();
                        HandleDeleteKey(sender, e);
                        e.Handled = true;
                    }
                }
                if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                {
                    if (e.Key >= Key.D0 && e.Key <= Key.D9)
                    {
                        string text = e.Key.ToString().Substring(1, 1);
                        e.Handled = MatchWithMask(this, text);
                    }
                    else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                    {
                        string text = e.Key.ToString().Substring(6);
                        e.Handled = MatchWithMask(this, text);
                    }
                    //e.Handled = false;
                }
                else if (e.Key == Key.Subtract || e.PlatformKeyCode == 189)
                {
                    e.Handled = MatchWithMask(this, "-");
                }
                else if (e.Key == Key.Add || e.PlatformKeyCode == 187)
                {
                    e.Handled = MatchWithMask(this, "+");
                }
                else if (e.Key == Key.Decimal || e.PlatformKeyCode == 190)
                {
                    if (this.textBox.Text.Contains(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator))
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else if (e.Key == Key.Up)
                {
                    this.ChangeValue(this.Interval);
                    ContentFormating(this.Value);
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    this.ChangeValue(-this.Interval);
                    ContentFormating(this.Value);
                    e.Handled = true;
                }
                else if (e.Key == Key.Tab)
                {
                    this.IsTabStop = true;
                }
                else if (e.Key == Key.Delete)
                {
                    HandleDeleteKey(sender, e);
                }
                else if (e.Key == Key.Back)
                {
                    HandleBackSpaceKey(sender, e);
                }
                else
                {
                    e.Handled = true;
                }
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public void HandleBackSpaceKey(object d, KeyEventArgs e)
        {
            if (this.textBox.IsReadOnly)
            {
                e.Handled = true;
                return;
            }
            NumberFormatInfo numberFormat = this.GetNumberFormatInfo();
            string maskedText = this.Text;
            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            int separatorStart = maskedText.IndexOf(numberFormat.NumberDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.NumberDecimalSeparator.Length;
            int negflag = 0; 
            int caretPosition=0;
            if (this.textBox.Text.Length != this.Text.Length)
            {
                if (this.SelectText == string.Empty)
                {
                    caretPosition = this.textBox.SelectionStart + 1;
                    this.textBox.Text = this.Text;  
                    this.textBox.SelectionStart = caretPosition;                      
                }          
            }
            else
                caretPosition = this.textBox.SelectionStart;
            string unmaskedText = "";
            if (this.SelectText.Length == 1)
            {
                if (!char.IsDigit(maskedText[this.textBox.SelectionStart]))
                {
                    if (maskedText[this.textBox.SelectionStart] == '-')
                    {
                        this.Value = (this.Value * -1);
                    }
                    this.textBox.SelectionLength = 0;
                    e.Handled = true;
                    return;
                }
            }
            else if (this.SelectText.Length == 0 && this.textBox.SelectionStart == 1 && this.textBox.SelectionStart != maskedText.Length - 2)
            {
                if (!char.IsDigit(maskedText[this.textBox.SelectionStart - 1]))
                {
                    if (maskedText[0] == '-')
                    {
                        this.Value = (this.Value * -1);
                        this.SelectionLength = 0;
                        e.Handled = true;
                        return;
                    }
                }
            }
            if (this.SelectText.Length == 0 && this.textBox.SelectionStart != 0)
            {
                if (maskedText[this.textBox.SelectionStart - 1] == ',')
                {
                    unmaskedText = "";
                    this.textBox.SelectionStart--;
                    e.Handled = true;
                    return;
                }
            }


            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == this.textBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (this.textBox.SelectionStart + this.SelectText.Length))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += numberFormat.NumberDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]))
                        unmaskedText += maskedText[i];
                }
            }
            selectionLength = selectionEnd - selectionStart;
            if (selectionStart <= separatorStart && selectionEnd >= separatorEnd && unmaskedText.Length > 0)
            {
                if (numberFormat != null && selectionLength < unmaskedText.Length)
                {
                    for (int decpos = 0; decpos < this.SelectText.Length; decpos++)
                    {
                        if (unmaskedText.Length > 0)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                }
                                else
                                    selectionStart++;
                            }
                        }
                    }
                }
                else if (selectionLength == unmaskedText.Length)
                {
                    unmaskedText = "";
                }

            }
            else if (selectionStart <= separatorStart && selectionEnd < separatorEnd)
            {
                if (selectionLength == 0)
                {
                    if (selectionStart != 0)
                    {
                        selectionLength = 1;
                        if (this.MinValue == Double.Parse(unmaskedText))
                        {
                            caretPosition = selectionStart - 1;
                        }
                        else if (unmaskedText.Length > 0)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, selectionLength);
                            caretPosition = selectionStart - 1;
                            if (this.textBox.SelectionStart == 2)
                                negflag = 1;
                        }

                    }
                    else
                    {
                        e.Handled = true;
                        return;
                    }
                }
                else if (unmaskedText.Length > 0)
                {
                    if (maskedText[this.textBox.SelectionStart] == '-')
                    {
                        this.Value = this.Value * -1;
                    }
                    if (selectionLength >= 0)
                    {
                        unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                        caretPosition = selectionStart;
                    }
                    if (selectionLength < 1)
                    {
                        selectionLength = 1;
                        unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                        caretPosition = selectionStart;
                    }
                    if (this.SelectionStart > 0)
                    {
                        if (this.Text[this.SelectionStart - 1] == '-')
                        {
                            negflag = 1;
                        }
                    }
                }
            }
            else if (separatorStart < 0)
            {
                if (selectionStart >= 0)
                {
                    if (selectionStart == 0 && selectionLength >= 1)
                    {
                        if (maskedText[this.textBox.SelectionStart] == '-')
                        {
                            this.Value = this.Value * -1;
                        }
                    }
                    if (selectionLength >= 1 && unmaskedText.Length > 0)
                    {
                        if (selectionLength == unmaskedText.Length && this.MinValue.ToString() != "" && this.MinValue > 0)
                        {
                            unmaskedText = this.MinValue.ToString();
                        }
                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                            caretPosition = selectionStart;
                            if (this.textBox.SelectionStart == 1)
                                negflag = 1;
                        }
                    }
                    else if (selectionLength == 0 && selectionStart == unmaskedText.Length && unmaskedText.Length > 0)
                    {
                        if (Double.Parse(unmaskedText) == this.MinValue)
                        {
                            caretPosition = selectionStart - 1;
                        }
                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                            caretPosition = selectionStart;
                        }
                    }
                    else if (selectionLength == 0 && unmaskedText.Length > 0 && selectionStart != unmaskedText.Length && selectionStart != 0 && maskedText[this.textBox.SelectionStart - 1] != ',')
                    {
                        if (this.SelectionStart == 2)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                            negflag = 1;

                        }
                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                            caretPosition = selectionStart - 1;
                        }
                    }
                    else if (selectionLength == 0 && selectionStart != unmaskedText.Length && selectionStart != 0 && maskedText[this.textBox.SelectionStart - 1] == ',')
                    {
                        unmaskedText = "";
                        this.textBox.SelectionStart--;
                        e.Handled = true;
                        return;
                    }
                    else
                    {
                        if (selectionStart != 0 && unmaskedText.Length > 0)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, 1);
                            caretPosition = selectionStart;
                        }
                    }

                }
            }
            else
            {
                if (selectionStart == selectionEnd && unmaskedText.Length > 0)
                {
                    if (selectionStart != separatorEnd)
                    {
                        unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);

                        for (int len = unmaskedText.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == numberFormat.NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    //count++;
                                }

                            }
                        }
                        caretPosition = selectionStart - 1;
                    }
                    else
                    {
                        this.textBox.SelectionStart = this.textBox.SelectionStart - 1;
                        e.Handled = true;
                        return;
                    }
                }
                else if (unmaskedText.Length > 0)
                {
                    if (selectionStart == selectionEnd)
                    {
                        if (selectionStart != unmaskedText.Length)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[selectionStart - 1].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                                }

                            }
                            caretPosition = selectionStart - 1;
                        }
                        else
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        if (this.SelectText.Length == 0)
                        {
                            if (unmaskedText.Length > 0)
                            {
                                if (numberFormat != null && unmaskedText != string.Empty)
                                {
                                    if (unmaskedText[selectionStart - 1].ToString() != numberFormat.NumberDecimalSeparator)
                                    {
                                        unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (unmaskedText.Length > 0 && this.SelectText.Length > 0)
                            {
                                if (numberFormat != null && unmaskedText != string.Empty)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart, this.SelectText.Length);
                                }
                            }
                        }
                    }
                }
            }

            double preValue;
            bool separatorflag = false;
            if (double.TryParse(unmaskedText, out preValue))
            {

                if (this.textBox.Text.Contains("-"))
                {
                    preValue = preValue * -1;
                }

                if (preValue > this.MaxValue)
                {
                    preValue = this.MaxValue;
                }
                if (preValue <= this.MinValue)
                {
                    if (preValue < this.MinValue && this.MinValue >= 0)
                    {
                        if (numberFormat != null)
                        {
                            if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >= (this.MinValue.ToString()).Length)
                            {
                                preValue = this.MinValue;
                            }
                            else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (this.MinValue.ToString()).Length)
                            {
                                if (preValue >= this.MinValue)
                                {
                                    this.Text = unmaskedText;
                                    this.textBox.Text = unmaskedText;
                                }
                                else
                                    preValue = this.MinValue;
                            }
                        }
                        else
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    else if (preValue > this.MinValue)
                    {
                        this.textBox.Text = unmaskedText;
                        this.Text = unmaskedText;
                    }
                    else if (preValue >= this.MinValue)
                    {
                        if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) > (this.MinValue.ToString()).Length)
                            preValue = this.MinValue;
                        else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (this.MinValue.ToString()).Length)
                        {
                            this.textBox.Text = unmaskedText;
                            this.Text = unmaskedText;
                        }
                        else
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    else
                        preValue = this.MinValue;
                }
                this.textBox.Text = preValue.ToString("N", numberFormat);
                this.Text = this.textBox.Text;
                maskedText = this.textBox.Text;
                this.Value = preValue;
                if (negflag == 0)
                {
                    int j = 0;
                    for (i = 0; i < unmaskedText.Length; i++)
                    {
                        if (i == caretPosition)
                            break;
                        if (j == maskedText.Length)
                            break;

                        if (char.IsDigit(maskedText[j]))
                            j++;

                        else
                        {
                            for (int k = j; k < maskedText.Length; k++)
                            {
                                if (j == maskedText.IndexOf(numberFormat.NumberDecimalSeparator))
                                    separatorflag = true;

                                if (char.IsDigit(maskedText[k]))
                                    break;
                                j++;
                            }
                            if (separatorflag == false)
                                i--;
                            separatorflag = false;
                        }
                    }

                    this.textBox.SelectionStart = j;
                }
                else
                    this.textBox.SelectionStart = 1;
                this.textBox.SelectionLength = 0;
                negflag = 0;
            }
            else if (preValue == 0)
            {
                if (this.MinValue != 0d || this.MinValue > 0)
                {
                    preValue = this.MinValue;
                }
                this.textBox.Text = preValue.ToString("N", numberFormat);
                this.Text = this.textBox.Text;
                maskedText = this.textBox.Text;
                this.Value = preValue;
            }
            if (this.AllowEdit)
            {
                this.Validate(this.textBox.Text, true);
                this.ContentFormating(preValue);
            }
            e.Handled = true;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public void HandleDeleteKey(object d,KeyEventArgs e)
        {
            if (this.textBox.IsReadOnly)
            {
                e.Handled = true;
                return;
            }
                NumberFormatInfo numberFormat = this.GetNumberFormatInfo();
                string maskedText = this.Text;
                int selectionStart = 0;
                int selectionEnd = 0;
                int selectionLength = 0;
                int separatorStart = maskedText.IndexOf(numberFormat.NumberDecimalSeparator);
                int separatorEnd = separatorStart + numberFormat.NumberDecimalSeparator.Length;
                int negflag = 0;
                int caretPosition = this.textBox.SelectionStart;
                string unmaskedText = "";
                if (this.SelectText.Length <= 1 && this.textBox.SelectionStart != maskedText.Length)
                {
                    if (!char.IsDigit(maskedText[this.textBox.SelectionStart]))
                    {
                        if (maskedText[this.textBox.SelectionStart] == '-')
                        {
                            this.Value = (this.Value * -1);
                            this.textBox.SelectionLength = 0;
                            e.Handled = true;
                            return;
                        }
                    }
                    if (numberFormat != null)
                    {
                        if (this.textBox.SelectionStart == maskedText.Length - (numberFormat.NumberDecimalDigits + 2))
                        {
                            if (maskedText[this.textBox.SelectionStart] == '0' && this.SelectionStart == 0)
                            {
                                negflag = 1;
                            }
                        }
                    }

                    if (maskedText[this.textBox.SelectionStart] == '0' && this.textBox.SelectionStart == 0)
                    {
                        negflag = 1;
                        unmaskedText = "";
                        this.textBox.Text = this.Value.ToString("N", numberFormat);
                        this.textBox.SelectionStart++;
                        e.Handled = true;
                        return;
                    }
                    if (this.textBox.SelectionStart == 1)
                    {
                        negflag = 1;
                    }

                    if (maskedText[this.textBox.SelectionStart] == ',')
                    {
                        unmaskedText = "";
                        this.textBox.Text = this.Value.ToString("N", numberFormat);
                        this.textBox.SelectionStart = caretPosition + 1;
                        e.Handled = true;
                        return;
                    }

                }

                int i;
                for (i = 0; i <= maskedText.Length; i++)
                {
                    if (i == this.textBox.SelectionStart)
                    {
                        selectionStart = unmaskedText.Length;
                        caretPosition = selectionStart;
                    }
                    if (i == (this.textBox.SelectionStart +this.SelectText.Length))
                        selectionEnd = unmaskedText.Length;

                    if (i == separatorEnd)
                        separatorEnd = unmaskedText.Length;

                    if (i == separatorStart)
                    {
                        separatorStart = unmaskedText.Length;
                        unmaskedText +=numberFormat.NumberDecimalSeparator.ToString();
                    }

                    if (i < maskedText.Length)
                    {
                        if (char.IsDigit(maskedText[i]))
                            unmaskedText += maskedText[i];
                    }
                }
                selectionLength = selectionEnd - selectionStart;
                if (separatorStart < 0)
                {
                    separatorStart = unmaskedText.Length;
                    separatorEnd = unmaskedText.Length;
                }
                if (selectionStart <= separatorStart && selectionEnd >= separatorEnd && unmaskedText.Length > 0)
                {
                    if (numberFormat != null && selectionLength < unmaskedText.Length)
                    {
                        for (int decpos = 0; decpos < this.SelectText.Length; decpos++)
                        {
                            if (unmaskedText.Length > 0)
                            {
                                if (numberFormat != null && unmaskedText != string.Empty)
                                {
                                    if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator)
                                    {
                                        unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                    }
                                    else
                                        selectionStart++;
                                }
                            }
                        }
                    }
                    else if (selectionLength == unmaskedText.Length)
                    {
                        unmaskedText = "";
                    }
                    if (selectionLength == unmaskedText.Length && this.MinValue.ToString() != "" && this.MinValue > 0)
                    {
                        unmaskedText = this.MinValue.ToString();
                    }
                    else
                    {

                    }
                }
                else if (selectionStart <= separatorStart && selectionEnd < separatorEnd && unmaskedText.Length > 0)
                {
                    if (selectionLength == 0)
                    {
                        if (selectionStart != separatorStart)
                        {
                            selectionLength = 1;
                            if (this.MinValue == Double.Parse(unmaskedText))
                            {
                                caretPosition = selectionStart + 1;
                            }
                            else
                            {
                                unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                                if (negflag == 1)
                                {
                                    caretPosition = selectionStart + 1;
                                    negflag = 0;
                                }
                            }
                            if (this.textBox.SelectionStart == 1)
                            {
                                negflag = 1;
                            }
                        }

                        else
                        {
                            int tempval;
                            this.textBox.SelectionStart = this.textBox.SelectionStart + 1;
                            tempval = this.textBox.SelectionStart;
                            this.textBox.Text = this.Value.ToString("N", numberFormat);
                            this.textBox.SelectionStart = tempval;
                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        if (selectionStart == 0)
                        {
                            if (maskedText[this.textBox.SelectionStart] == '-')
                            {
                                this.Value = this.Value * -1;
                            }
                        }
                        unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                        caretPosition = selectionStart;
                        if (this.textBox.SelectionStart == 1)
                        {
                            negflag = 1;
                        }

                    }
                }
                else if (unmaskedText.Length > 0)
                {
                    if (selectionStart == selectionEnd)
                    {
                        if (selectionStart != unmaskedText.Length)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                    if (unmaskedText[selectionStart - 1].ToString() == numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString()!="0")
                                    {
                                        caretPosition = this.textBox.SelectionStart;
                                        double tempdouble = double.Parse(unmaskedText);
                                        this.textBox.Text = tempdouble.ToString("N", numberFormat);
                                        this.Text = this.textBox.Text;
                                        maskedText = this.textBox.Text;
                                        this.Value = tempdouble;
                                        this.textBox.SelectionStart = caretPosition;
                                        e.Handled = true;
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            e.Handled = true; return;
                        }
                    }
                    else
                    {
                        if (unmaskedText.Length > 0)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart, this.SelectText.Length);
                                }
                            }
                        }

                    }
                }

                double preValue;
                if (double.TryParse(unmaskedText, out preValue))
                {
                    if (this.textBox.Text.Contains("-") || this.Text.Contains("-"))
                    {
                        preValue = preValue * -1;
                    }
                    if (preValue > this.MaxValue)
                    {
                        preValue = this.MaxValue;
                    }

                    if (preValue < this.MinValue)
                    {
                        if (preValue <= this.MinValue && this.MinValue >= 0)
                        {
                            if (numberFormat != null)
                                if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >= (this.MinValue.ToString()).Length)
                                {
                                    preValue = this.MinValue;
                                }
                                else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (this.MinValue.ToString()).Length)
                                {
                                    if (preValue >= this.MinValue)
                                    {
                                        this.textBox.Text = unmaskedText;
                                        this.Text = unmaskedText;
                                    }
                                    else
                                        preValue = this.MinValue;

                                }
                                else
                                {
                                    e.Handled = true; return;
                                }
                        }
                        else if (preValue > this.MinValue)
                        {
                            this.textBox.Text = unmaskedText;
                            this.Text = unmaskedText;
                        }
                        else if (preValue >= this.MinValue)
                        {
                            if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) > (this.MinValue.ToString()).Length)
                                preValue = this.MinValue;
                            else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (this.MinValue.ToString()).Length)
                            {
                                this.textBox.Text = unmaskedText;
                                this.Text = unmaskedText;
                            }
                            else
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                        else
                        {
                            e.Handled = true;
                            return;
                        }
                    }

                    this.textBox.Text = preValue.ToString("N", numberFormat);
                    this.Text = this.textBox.Text;
                    maskedText = this.textBox.Text;
                    this.Value = preValue;
                    if (negflag == 0)
                    {
                        int j = 0;
                        for (i = 0; i < unmaskedText.Length; i++)
                        {
                            if (i == caretPosition)
                            {
                                break;
                            }
                            if (j == maskedText.Length)
                                break;
                            if (char.IsDigit(maskedText[j]))
                                j++;
                            else
                            {
                                for (int k = j; k < maskedText.Length; k++)
                                {
                                    if (char.IsDigit(maskedText[k]))
                                        break;
                                    j++;
                                }
                                i--;
                            }
                        }
                        this.textBox.SelectionStart = j;
                        selectionStart = j;
                    }
                    else
                    {
                        if (this.Value < 0)
                        {
                            this.textBox.SelectionStart++;
                            selectionStart++;
                        }
                        else
                        {
                            this.textBox.SelectionStart = 1;
                            selectionStart = 1;
                        }
                    }
                    this.textBox.SelectionLength = 0;
                    negflag = 0;
                }
                else if (preValue == 0)
                {
                    if (this.MinValue != 0d || this.MinValue > 0)
                    {
                        preValue = this.MinValue;
                    }
                    this.textBox.Text = preValue.ToString("N", numberFormat);
                    this.Text = this.textBox.Text;
                    maskedText = this.textBox.Text;
                    this.Value = preValue;
                }
                if (this.AllowEdit)
                {
                    this.Validate(this.textBox.Text, true);
                    this.ContentFormating(preValue);
                }
                e.Handled = true;         
               
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numericupdown"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool MatchWithMask(NumericUpDown numericupdown, string text)
        {
            NumberFormatInfo numberformat = this.GetNumberFormatInfo();

            if (text == "-" || text == "+")
            {
                if (this.Text!=string.Empty)
                {
                    double tempVal = (double)this.Value * -1;

                    if (tempVal > this.MaxValue)
                    {
                        tempVal = this.MaxValue;
                    }

                    if (tempVal < this.MinValue)
                    {
                        tempVal = this.MinValue;
                    }

                    this.Value = tempVal;
                    this.textBox.Text = tempVal.ToString("N", numberformat);
                    this.Text = tempVal.ToString("N", numberformat);
                    if (this.Text.Contains("-") == true)
                        this.SelectionStart++;

                }
                return true;
            }

            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            if (numericupdown.Value == double.NaN)
                numericupdown.Text = "";
            string maskedText = numericupdown.Text;
            int separatorStart = maskedText.IndexOf(numberformat.NumberDecimalSeparator);
            int separatorEnd = separatorStart + numberformat.NumberDecimalSeparator.Length;
            int caretPosition = this.textBox.SelectionStart;
            string unmaskedText = "";

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == this.textBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (this.textBox.SelectionStart + this.textBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    //unmaskedText += ".";
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]))
                    {
                        if (unmaskedText.Length == 0)
                        {
                            if (maskedText[i] != '0')
                            {
                                unmaskedText += maskedText[i];
                            }
                        }
                        else
                        {
                            unmaskedText += maskedText[i];
                        }
                    }
                }
            }
            selectionLength = selectionEnd - selectionStart;
            if (separatorStart < 0)
            {
                separatorStart = unmaskedText.Length;
                separatorEnd = unmaskedText.Length;
            }
            if (text != string.Empty)
            {
                if (selectionStart <= separatorStart && selectionEnd >= separatorEnd && char.IsDigit(text[0]))
                {
                    for (int decpos = separatorEnd; decpos < selectionEnd; decpos++)
                    {
                        if (decpos != unmaskedText.Length)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            unmaskedText = unmaskedText.Insert(decpos, "0");
                        }
                    }
                    unmaskedText = unmaskedText.Remove(selectionStart, (separatorStart - selectionStart));
                    caretPosition = selectionStart;

                    unmaskedText = unmaskedText.Insert(selectionStart, text);
                    caretPosition = caretPosition + text.Length;
                }
                else if (selectionStart <= separatorStart && selectionEnd < separatorEnd)
                {
                    unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                    caretPosition = selectionStart;

                    unmaskedText = unmaskedText.Insert(selectionStart, text);
                    caretPosition = caretPosition + text.Length;
                }
                else
                {
                    if (selectionStart == selectionEnd)
                    {
                        if (selectionStart != unmaskedText.Length)
                        {
                            unmaskedText = unmaskedText.Insert(selectionStart, text[0].ToString());
                            if (numericupdown.Value > -1 && numericupdown.Value < 1)
                            {
                                caretPosition = selectionStart + 1;
                            }
                        }
                    }
                    else if (char.IsDigit(text[0]))
                    {
                        int textpos = 0;
                        for (int decpos = selectionStart; decpos < selectionEnd; decpos++)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            if (textpos < text.Length)
                                unmaskedText = unmaskedText.Insert(decpos, text[textpos].ToString());
                            else
                                unmaskedText = unmaskedText.Insert(decpos, "0");
                            textpos++;
                        }
                        caretPosition = selectionStart + text.Length;
                    }
                }
            }
            double preValue;
            if (double.TryParse(unmaskedText, out preValue))
            {
                if (numericupdown.textBox.Text.Contains("-"))
                {
                    preValue = preValue * -1;
                }
                if (preValue > numericupdown.MaxValue)
                {
                    preValue = numericupdown.MaxValue;
                }
                if (preValue <= numericupdown.MinValue && numericupdown.MinValue >= 0)
                {
                    if (numberformat != null)
                    {
                        if (unmaskedText.Length - (numberformat.NumberDecimalDigits + 1) >= (numericupdown.MinValue.ToString()).Length)
                            preValue = numericupdown.MinValue;
                    }
                    else if (preValue > numericupdown.MinValue)
                    {
                        numericupdown.textBox.Text = unmaskedText;
                        this.Text = unmaskedText;
                    }

                }

                if (numericupdown.textBox.MaxLength != 0)
                {
                    if (unmaskedText.Length > numericupdown.textBox.MaxLength && numericupdown.NumberDecimalDigits <= numericupdown.textBox.MaxLength)
                    {
                        int len = numericupdown.NumberDecimalDigits;
                        if (len < 0)
                        {
                            preValue = double.Parse(unmaskedText.Remove((numericupdown.textBox.MaxLength) - 3));
                            caretPosition++;
                        }
                        else
                            preValue = double.Parse(unmaskedText.Remove((numericupdown.textBox.MaxLength) - 1 - len));
                        numericupdown.Value = preValue;
                        this.textBox.Text = preValue.ToString("N", numberformat);
                        this.Text = preValue.ToString("N", numberformat);
                        return true;
                    }

                }
                this.textBox.Text = preValue.ToString("N", numberformat);
                this.Value = preValue;
                this.Text = this.textBox.Text;
                maskedText = numericupdown.Text;
               

                int j = 0;
                for (i = 0; i < unmaskedText.Length; i++)
                {
                    if (i == caretPosition)
                    {
                        break;
                    }
                    if (j == maskedText.Length)
                        break;
                    if (char.IsDigit(maskedText[j]))
                        j++;
                    else
                    {
                        for (int k = j; k < maskedText.Length; k++)
                        {
                            if (char.IsDigit(maskedText[k]))
                                break;
                            j++;
                        }
                        i--;
                    }
                }
                numericupdown.textBox.SelectionStart = j;
                numericupdown.textBox.SelectionLength = 0;
            }
            return true;
        }

        /// <summary>
        /// Provides handling for TextBox KeyUp Event
        /// </summary>
        /// <param name="sender">The TextBox of the Numeric Updown control</param>
        /// <param name="e">Event ARgument</param>
        private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((NumericUpDown)obj != null)
                ((NumericUpDown)obj).OnValueChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            NumberFormatInfo numberFormat = this.GetNumberFormatInfo();
            if (this.Value > this.MaxValue || this.Value < this.MinValue)
            {
                if (this.Value > this.MaxValue)
                {
                    this.Value = this.MaxValue;
                }
                if (this.Value < this.MinValue && this.MinValue < this.MaxValue)
                {
                    this.Value = this.MinValue;
                }
            }
            ContentFormating(this.Value);
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, args);
            }
        }

        /// <summary>
        /// Provides handling for TextBox TextChanged Event
        /// </summary>
        /// <param name="sender">The TextBox of the Numeric Updown control</param>
        /// <param name="e">Event ARgument</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.textBox.IsReadOnly == true || this.AllowEdit == false)
            {
                this.textBox.Text = this.Text;
                return;
            }
            else
            {
                NumberFormatInfo numberformat = this.GetNumberFormatInfo();
                if (this.IsFocused)
                {
                    if (this.textBox.Text == string.Empty)
                    {
                        this.textBox.Text = "0";
                    }

                    if (this.SelectText == "0" && subflag)
                    {
                        this.textBox.Text = this.SelectText;
                    }

                    int val1, val2;
                    string valueString = this.textBox.Text;
                    if (int.TryParse(this.latestText, out val1) && val1 == 0.0)
                    {
                        if (int.TryParse(valueString, out val2) && val2 >= 1)
                        {
                            valueString = valueString.TrimEnd('0');
                            //NumberFormatInfo numberformat = this.GetNumberFormatInfo();
                            int d = int.Parse(valueString);
                            valueString = d.ToString("N", numberformat);
                        }
                    }

                    string keyInput = valueString;
                    this.Text = this.textBox.Text;
                    string regexp = this.RegExpCheck();
                    if (!string.IsNullOrEmpty(regexp))
                    {
                        keyInput = regexp;
                    }

                    if (!string.IsNullOrEmpty(keyInput))
                    {
                        this.Validate(keyInput, false);
                    }

                    if (this.latestText == ".0")
                    {
                        this.textBox.SelectionStart = this.textBox.Text.Length;
                    }
                }
            }
        }      

        /// <summary>
        /// Method used to validate the value typed in the control
        /// </summary>
        /// <returns>Returns the validated text</returns>
        private string RegExpCheck()
        {
            try
            {
                double value;
                value = double.Parse(this.textBox.Text, (IFormatProvider)this.NumberFormatInfo);
                string numregpattern = @"[\d]*][\d]{0," + this.NumberFormatInfo.NumberDecimalDigits + "}";
                if (value < 0)
                {
                    numregpattern = "[-]" + numregpattern;
                }

                return Regex.Match(this.textBox.Text, numregpattern).Value;
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>
        /// Provides handling for RepeatButton click Event
        /// </summary>
        /// <param name="sender">The Repeat Button</param>
        /// <param name="e">Event Argument</param>
        private void RepeatButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.textBox.IsReadOnly || this.AllowEdit == false)
                return;
            else
            {
                this.textBox.Focus();
                RepeatButton button = (RepeatButton)sender;
                if (button.Tag.ToString() == "Increment")
                {
                    this.ChangeValue(this.Interval);
                    ContentFormating(this.Value);
                }
                else
                {
                    this.ChangeValue(-this.Interval);
                    ContentFormating(this.Value);
                }
            }
        }

        /// <summary>
        /// Method used for validating the input
        /// </summary>
        /// <param name="keyInput">Represents the input </param>
        /// <param name="changevalue">Represents whether value can be changed or not</param>
        private void Validate(string keyInput, bool changevalue)
        {
            try
            {
                double newValue;

                NumberFormatInfo info = this.NumberFormatInfo;
                newValue = double.Parse(keyInput, (IFormatProvider)this.NumberFormatInfo);

                if (newValue != this.Value)
                {
                    if (newValue < this.MinValue)
                    {
                        if (changevalue)
                        {
                            this.Value = this.MinValue;
                        }
                    }
                    else if (newValue <= this.MaxValue && newValue >= this.MinValue)
                    {
                        this.Value = newValue;
                    }
                    else if (newValue > this.MaxValue)
                    {
                        if (changevalue)
                        {
                            this.Value = this.MaxValue;
                        }
                    }
                    else
                    {
                        this.Value = newValue;
                    }
                }
                else
                {
                    this.Value = newValue;
                }
            }
            catch
            {
            }
        }
        #endregion Private Methods
    }
}

