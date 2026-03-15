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
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// FlowDirection Enumeration
    /// </summary>
    public enum EnumFlowDirection
    {
        /// <summary>
        /// Specifies the LeftToRight Flow Direction
        /// </summary>
        LeftToRight,

        /// <summary>
        /// Specifies the RightToLeft Flow Direction
        /// </summary>
        RightToLeft
   }

    /// <summary>
    /// Represents a Domain UpDown Control
    /// </summary>
    /// <example>
    /// <b>Creating DomainUpDown Control in XAML</b>
    /// <para></para>
    /// <code>&lt;UserControl x:Class=&quot;SilverlightSampleBrowser.DomainUpDownDemo&quot;
    ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
    ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
    ///     xmlns:syncfusion=&quot;clr-     namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight&quot;     Height=&quot;600&quot; Width=&quot;850&quot;&gt;</code>
    /// <para></para>
    /// <code>&lt;StackPanel x:Name=&quot;StackPanel1&quot;&gt;
    /// &lt;syncfusion:DomainUpDown Name=&quot;domainUpDown1&quot; Width=&quot;125&quot; Height=&quot;30&quot;  VerticalAlignment=&quot;Center&quot;    &gt;&lt;/syncfusion:DomainUpDown&gt;&lt;/StackPanel&gt;
    /// &lt;/UserControl&gt;</code>
    /// <para></para>
    /// <para></para>
    /// <para><b>Creating DomainUpDown Control using C#</b></para>
    /// <para> public partial class DomainUpDownDemo : UserControl</para>
    /// <para>    {</para>
    /// <para>        public DomainUpDownDemo</para>
    /// <para>        {</para>
    /// <para>            InitializeComponent();</para>
    /// <para>             DomainUpDown domainUpDown= new DomainUpDown ();</para>
    /// <para>StackPanel1.Children.Add(domainUpDown);</para>
    /// <para>       }</para>
    /// <para>    } </para>
    /// <para>}</para>
    /// </example>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Blend;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Office2007Black;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Default;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Office2010Black;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Windows7;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.VS2010;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
   Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Metro;component/DomainUpDown.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
   Type = typeof(DomainUpDown), XamlResource = "/Syncfusion.Theming.Transparent;component/DomainUpDown.xaml")]
    public class DomainUpDown : Control
    {
        #region Public Dependency Properties
       

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.DomainUpDown.CornerRadiusProperty">CornerRadius</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DomainUpDown), new PropertyMetadata(new CornerRadius(1), OnCornerRadiusChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.DomainUpDown.FlowDirectionProperty">FlowDirection</see>
        /// dependency Property
        /// </summary>
        public new static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(EnumFlowDirection), typeof(DomainUpDown), new PropertyMetadata(EnumFlowDirection.LeftToRight, OnFlowDirectionChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.DomainUpDown.TextAlignmentProperty">TextAlignment</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(DomainUpDown), new PropertyMetadata(System.Windows.TextAlignment.Right, OnTextAlignmentChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.DomainUpDown.ValueProperty">Value</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(DomainUpDown), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        #endregion Public Dependency Properties
   
        #region PRivate Fields
        /// <summary>
        ///   The Repeat Button Object used for decreasing the value
        /// </summary>
        private RepeatButton downrepeat;

        /// <summary>
        ///   The Repeat Button Object used for increasing the value
        /// </summary>
        private RepeatButton uprepeat;

        /// <summary>
        /// Indicates the Index
        /// </summary>
        private int m_index;

        /// <summary>
        /// Indicates the list of values 
        /// </summary>
        private List<object> m_list;

        /// <summary>
        /// The TextBox Object
        /// </summary>
        private TextBox m_textBox;

        /// <summary>
        /// The Main grid
        /// </summary>
        private Grid maingrid;

        /// <summary>
        /// The Flag variable used to indicate whether mouse is over the control or not
        /// </summary>
        private bool mouseflag = false;

        /// <summary>
        /// The flag variable idenfies whether the control is in focused state or not.
        /// </summary>
        private bool isfocused = false;
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.DomainUpDown">DomainUpDown</see> class
        /// </summary>
        public DomainUpDown()
        {
            DefaultStyleKey = typeof(DomainUpDown);
           
            this.MouseEnter += new MouseEventHandler(this.DomainUpDown_MouseEnter);
            this.MouseLeave += new MouseEventHandler(this.DomainUpDown_MouseLeave);
            this.Initialize();
        }

        /// <summary>
        /// Initializes the <see cref="DomainUpDown"/> class.
        /// </summary>
        static DomainUpDown()
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
        /// cref="P:Syncfusion.Windows.Tools.Controls.DomainUpDown.CornerRadius">CornerRadius</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.DomainUpDown.TextAlignment">TextAlignment</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FlowDirectionChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.DomainUpDown.TextAlignment">TextAlignment</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback TextAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.DomainUpDown.Value">Value</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;
        #endregion Public Events

        #region Public Properties
      
        /// <summary>
        /// Gets or sets a value indicating the CornerRadius. 
        /// </summary>
        /// <remarks>
        ///  <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DomainUpdown Name=&quot;domainupdown&quot;
        /// CornerRadius=&quot;2&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>numericupdown.CornerRadius=2.0;  </para>
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
        /// Gets or sets a value indicating the FlowDirection Dependency Property
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;syncfusion:DoaminUpDown  Name=&quot;domainupdown&quot;
        /// FlowDirection=&quot;LeftToRight&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>domainupdown.FlowDirection=FlowDirection.LeftToRight; </para>
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
                if (this.maingrid != null)
                {
                    if (this.FlowDirection == EnumFlowDirection.LeftToRight)
                    {
                        this.maingrid.RowDefinitions.Clear();
                        this.maingrid.ColumnDefinitions.Clear();
                        RowDefinition row1 = new RowDefinition();
                        RowDefinition row2 = new RowDefinition();
                        ColumnDefinition column1 = new ColumnDefinition();
                        ColumnDefinition column2 = new ColumnDefinition();
                        GridLength width1 = new GridLength(1, GridUnitType.Star);
                        GridLength width2 = new GridLength(25);
                        column1.Width = width1;
                        column2.Width = width2;
                        this.maingrid.RowDefinitions.Add(row1);
                        this.maingrid.RowDefinitions.Add(row2);
                        this.maingrid.ColumnDefinitions.Add(column1);
                        this.maingrid.ColumnDefinitions.Add(column2);
                        this.m_textBox.SetValue(Grid.RowProperty, (int)0);
                        this.m_textBox.SetValue(Grid.ColumnProperty, (int)0);
                        this.m_textBox.SetValue(Grid.RowSpanProperty, (int)2);
                        this.uprepeat.SetValue(Grid.RowProperty, (int)0);
                        this.uprepeat.SetValue(Grid.ColumnProperty, (int)1);
                        this.downrepeat.SetValue(Grid.RowProperty, (int)1);
                        this.downrepeat.SetValue(Grid.ColumnProperty, (int)1);
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
                        this.m_textBox.SetValue(Grid.RowProperty, (int)0);
                        this.m_textBox.SetValue(Grid.ColumnProperty, (int)1);
                        this.m_textBox.SetValue(Grid.RowSpanProperty, (int)2);
                        this.uprepeat.SetValue(Grid.RowProperty, (int)0);
                        this.uprepeat.SetValue(Grid.ColumnProperty, (int)0);
                        this.downrepeat.SetValue(Grid.RowProperty, (int)1);
                        this.downrepeat.SetValue(Grid.ColumnProperty, (int)0);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the TextAlignment
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;syncfusion:DomainUpDown Name=&quot;domainupdown&quot;
        /// TextAlignment=&quot;Left&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>domainupdown.TextAlignment=TextAlignment.Left; </para>
        /// </remarks>
        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }

            set
            {
                SetValue(TextAlignmentProperty, value);
                if (this.m_textBox != null)
                {
                    if (this.TextAlignment == TextAlignment.Center)
                    {
                        this.m_textBox.TextAlignment = TextAlignment.Center;
                    }
                    else if (this.TextAlignment == TextAlignment.Right)
                    {
                        this.m_textBox.TextAlignment = TextAlignment.Right;
                    }
                    else
                    {
                         this.m_textBox.TextAlignment = TextAlignment.Left;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the value for the control
        /// </summary>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.string</a>
        /// </value>
        public string Value
        {
            get 
            {
                return (string)GetValue(ValueProperty); 
            }

            set 
            { 
                SetValue(ValueProperty, value);
            }
        }
        #endregion Public Properties

        /// <summary>
        /// Gets a value indicating the Indexer for the class
        /// </summary>
        /// <param name="index">Represents the index value</param>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.string</a>
        /// </value>
        public string this[int index]
        {
            get
            {
                return (string)this.m_list[index];
                
            }
        }
    
        #region Public Methods
        /// <summary>
        /// Method used to add the item
        /// </summary>
        /// <remarks>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DomainUpDown domainupdown=new DomainUpDown();</para>
        /// <para>domainupdown.Add(&quot;String&quot;);</para>
        /// </remarks>
        /// <param name="item">Item to be added to the control</param>
        public void Add(string item)
        {
            if (this.m_list.Count == 0 && this.m_textBox != null)
            {
                this.m_list.Add(item);
                this.m_textBox.Text = (string)this.m_list[0];
                this.Value = this.m_textBox.Text;
            }

            this.m_list.Add(item);
        }

        /// <summary>
        /// Method used to add the range of items to the list
        /// </summary>
        /// <remarks>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>string[] strarray = { &quot;9&quot;, &quot;10&quot;, &quot;11&quot;
        /// };</para>
        /// <para></para>
        /// <para>DomainUpDown domainupdown=new DomainUpdown();</para>
        /// <para>domainupdown.AddRange(strarray);</para>
        /// </remarks>
        /// <param name="range">Array of strings</param>
        public void AddRange(string[] range)
        {
            if (this.m_list.Count == 0 && this.m_textBox != null)
            {
                for (int i = 0; i < range.Length; i++)
                {
                    this.m_list.Add(range[i]);
                }

                this.m_textBox.Text = (string)this.m_list[0];
                this.Value = this.m_textBox.Text;
            }
            else
            {
                for (int i = 0; i < range.Length; i++)
                {
                    this.m_list.Add(range[i]);
                }
            }
        }

        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_textBox = GetTemplateChild("textbox") as TextBox;
            this.m_textBox.FontSize = FontSize;
            if (this.m_list.Count != 0)
            {
                this.m_textBox.Text = (string)this.m_list[0];
                this.Value = this.m_textBox.Text;
            }

            this.uprepeat = this.GetTemplateChild("UpButton") as RepeatButton;
            this.downrepeat = this.GetTemplateChild("DownButton") as RepeatButton;
            if (this.uprepeat != null)
            {
                this.uprepeat.Click += new RoutedEventHandler(this.UpRepeat_Click);
            }

            if (this.downrepeat != null)
            {
                this.downrepeat.Click += new RoutedEventHandler(this.DownRepeat_Click);
            }

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
                    this.m_textBox.SetValue(Grid.RowProperty, (int)0);
                    this.m_textBox.SetValue(Grid.ColumnProperty, (int)0);
                    this.m_textBox.SetValue(Grid.RowSpanProperty, (int)2);
                    this.uprepeat.SetValue(Grid.RowProperty, (int)0);
                    this.uprepeat.SetValue(Grid.ColumnProperty, (int)1);
                    this.downrepeat.SetValue(Grid.RowProperty, (int)1);
                    this.downrepeat.SetValue(Grid.ColumnProperty, (int)1);
                    this.uprepeat.BorderThickness = new Thickness(1, 0, 0, 1);
                    this.downrepeat.BorderThickness = new Thickness(1, 0, 0, 0);
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
                    this.m_textBox.SetValue(Grid.RowProperty, (int)0);
                    this.m_textBox.SetValue(Grid.ColumnProperty, (int)1);
                    this.m_textBox.SetValue(Grid.RowSpanProperty, (int)2);
                    this.uprepeat.SetValue(Grid.RowProperty, (int)0);
                    this.uprepeat.SetValue(Grid.ColumnProperty, (int)0);
                    this.downrepeat.SetValue(Grid.RowProperty, (int)1);
                    this.downrepeat.SetValue(Grid.ColumnProperty, (int)0); 
                    this.uprepeat.BorderThickness = new Thickness(0, 0, 1, 1);
                    this.downrepeat.BorderThickness = new Thickness(0, 0, 1, 0);
                }
            }

            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.AttachEvent("DOMMouseScroll", this.OnScroll);
                HtmlPage.Window.AttachEvent("onmousewheel", this.OnScroll);
                HtmlPage.Document.AttachEvent("onmousewheel", this.OnScroll);
            }
        }

        /// <summary>
        /// Method used to remove the item by specifying the item
        /// </summary>
        /// <remarks>
        /// <b>C#</b>
        /// <para></para>
        /// <para>DomainUpDown domainupdown=new DomainUpDown(); </para>
        /// <para>domainupdown.Remove(&quot;String&quot;);</para>
        /// </remarks>
        /// <param name="item">Item to remove from the list</param>
        public void Remove(string item)
        {
            this.m_list.Remove(item);
        }

        /// <summary>
        /// Method used to remove the item by specifying the index.
        /// </summary>
        /// <remarks>
        /// <b>C#</b>
        /// <para></para>
        /// <para>DomainUpDown domainupdown=new DomainUpDown(); </para>
        /// <para>domainupdown.RemoveAt(0);</para>
        /// </remarks>
        /// <param name="index">Identifies the index value of the item</param>
        public void RemoveAt(int index)
        {
            this.m_list.RemoveAt(index);
        }
        #endregion Public MEthods
     
        #region Internal Methods
        /// <summary>
        /// Provides handling for DomainUpDown control mouse enter event
        /// </summary>
        /// <param name="sender">The DomainUpDown control</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        internal void DomainUpDown_MouseEnter(object sender, MouseEventArgs e)
        {
            this.mouseflag = true;
        }

        /// <summary>
        /// Provides handling for DomainUpDown control mouse Leave event
        /// </summary>
        /// <param name="sender">The DomainUpDown control</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        internal void DomainUpDown_MouseLeave(object sender, MouseEventArgs e)
        {
            this.mouseflag = false;
        }

        /// <summary>
        /// Provides handling for Down RepeatButton Click event
        /// </summary>
        /// <param name="sender">The RepeatButton control</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        internal void DownRepeat_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateCounter(true);
        }

        /// <summary>
        /// Provides handling for Up RepeatButton Click event
        /// </summary>
        /// <param name="sender">The RepeatButton control</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        internal void UpRepeat_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateCounter(false);
        }

        #endregion Internal Methods

        #region Proctected Virtual Methods
       
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
        /// OnFlowDirectionChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFlowDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FlowDirectionChanged != null)
            {
                this.FlowDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// OnNegativeForegroundChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTextAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TextAlignmentChanged != null)
            {
                this.TextAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises OnValueChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }
        #endregion Proctected Virtual Methods

        #region Proctected Override Methods

        /// <summary>
        /// Method that handles when the key is pressed
        /// </summary>
        /// <param name="e">Event Argument</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Key.Up == e.Key)
            {
                this.UpdateCounter(false);
                e.Handled = true;
            }

            if (Key.Down == e.Key)
            {
                this.UpdateCounter(true);
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Provides handling for MouseEnter event
        /// </summary>
        /// <param name="e">contains information about the Mouse position and source</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (!isfocused)
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
            if (!isfocused)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Handles Got focus override and changes the state to focused.
        /// </summary>
        /// <param name="e">contains routed eventargs</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Focused", false);
            isfocused = true;
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Handles lost focus override and changes the state back to unfocused
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Unfocused", false);
            isfocused = false;
            base.OnLostFocus(e);
        }
        #endregion Proctected Override Method

        #region Private Static Methods

        /// <summary>
        /// Calls OnCornerRadiusChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e" >Event Argument</param>
        private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown source = (DomainUpDown)obj;
            source.OnCornerRadiusChanged(e);
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnFlowDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown source = (DomainUpDown)obj;
            source.OnFlowDirectionChanged(e);
        }

        /// <summary>
        /// Calls OnTextAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="obj" >Dependency Object</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnTextAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown source = (DomainUpDown)obj;
            source.OnTextAlignmentChanged(e);
        }

        /// <summary>
        /// Calls OnValueChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Dependency Object</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown instance = (DomainUpDown)d;
            instance.OnValueChanged(e);
        }
        #endregion Private Static Methods
    
        #region Private Methods

        /// <summary>
        /// Method used for updating the value 
        /// </summary>
        /// <param name="isUp">Indicates whether the value should be incremented or decremented</param>
        private void UpdateCounter(bool isUp)
        {
            if (m_list.Count > 0)
            {
                if (isUp)
                {
                    if (this.m_index < this.m_list.Count - 1)
                    {
                        this.m_index++;
                    }
                    else
                    {
                        this.m_index = 0;
                    }
                }
                else
                {
                    if (this.m_index > 0)
                    {
                        this.m_index--;
                    }
                    else
                    {
                        this.m_index = this.m_list.Count - 1;
                    }
                }

                this.m_textBox.Text = (string)this.m_list[this.m_index];
                this.Value = this.m_textBox.Text;
            }
        }

        /// <summary>
        /// Provides handling for control MouseScroll Event
        /// </summary>
        /// <param name="sender">The DomainUpdown Control</param>
        /// <param name="args">Event Argument</param>
        private void OnScroll(object sender, HtmlEventArgs args)
        {
            if (this.mouseflag)
            {
                double delta = 0;
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
                    this.UpdateCounter(false);
                }

                if (delta < 0)
                {
                    this.UpdateCounter(true);
                }
             }
        }

        /// <summary>
        /// Method that initializes the variables
        /// </summary>
        private void Initialize()
        {
            this.m_list = new List<object>();
            this.m_index = 0;
        }
        #endregion Private Methods
}
}

