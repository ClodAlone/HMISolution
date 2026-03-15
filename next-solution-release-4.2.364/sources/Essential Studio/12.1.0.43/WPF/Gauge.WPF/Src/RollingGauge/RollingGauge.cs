// <copyright file="RollingGauge.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Windows.Media.Animation;
using System.Globalization;
using System.Windows.Data;

namespace Syncfusion.Windows.Gauge
{

    /// <summary>
    /// Represents the RollingGauge UI element.
    /// </summary>
    /// <seealso cref="LinearGauge"/>
    /// <seealso cref="DigitalGauge"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CharacterFourteenSample.Window1" Title="CharacterFourteenSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:RollingGauge SegmentBackground="Firebrick" SpaceBetWeenSegment="2" 
    ///                            Value="01" SegmentCount="2" Unit="KM" UnitPosition="End">
    ///         </syncfusion:RollingGauge>
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
    /// namespace CharacterRollingGaugeSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private DigitalGauge digitalGauge1;
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 RollingGauge rollingGauge = new RollingGauge();
    ///                 rollingGauge.SpaceBetWeenSegment = new Thickness(1);
    ///                 rollingGauge.Value = "11";
    ///                 rollingGauge.Unit = "KM";
    ///                 rollingGauge.UnitPosition = UnitPosition.End;
    ///                 this.Content = rollingGauge;
    ///             }
    ///         }
    /// }   
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2003,
  Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/MetroStyle.xaml")]
    public class RollingGauge : GaugeBase
    {
        #region Private Members
        ///<summary>
        ///Collection of Segments.
        ///</summary>
        private SegmentCollection m_segments;

        ///<summary>
        ///Old value
        ///</summary>
        private string m_value;

        ///<summary>
        ///Segment frame.
        ///</summary>
        private StackPanel frame;

        private Grid grid;
        /// <summary>
        /// Old Numeric Value
        /// </summary>
        private double NumericValue = 0;
        /// <summary>
        /// Represents Current Segment Count
        /// </summary>
        private int m_segmentCount = 0;
        /// <summary>
        /// Represents Height and Width of the Segment
        /// </summary>
        double SegmentWidth = 0;

        /// <summary>
        /// Represents Height and Width of the Segment
        /// </summary>
        double SegmentHeight = 0;

        /// <summary>
        /// Handles Direction of Rotation for Numeric Values
        /// </summary>
        private bool NumericRotate = false;
        /// <summary>
        /// Shows Segments Status
        /// </summary>
        internal bool _Initialized = false;
        #endregion private Members

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="SegmentCount"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentCountChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentCount"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentHorizontalAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentCount"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentVerticalAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="Direction"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DirectionChanged;

        /// <summary>
        /// Event that is raised when <see cref="Unit"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback UnitChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentTemplate"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="UnitPosition"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback UnitPositionChanged;





        #endregion Events

        #region  Dependency Properties
        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RollingGauge), new FrameworkPropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Identifies the <see cref="SegmentVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentVerticalAlignmentProperty =
            DependencyProperty.Register("SegmentVerticalAlignment", typeof(VerticalAlignment), typeof(RollingGauge), new PropertyMetadata(VerticalAlignment.Center, OnSegmentHorizontalAlignmentChanged));

        /// <summary>
        /// Identifies the <see cref="AnimationDelay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDelayProperty =
            DependencyProperty.Register("AnimationDelay", typeof(TimeSpan), typeof(RollingGauge), new PropertyMetadata(TimeSpan.FromSeconds(1)));

        /// <summary>
        /// Identifies the <see cref="SpaceBetWeenSegment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpaceBetWeenSegmentProperty =
                    DependencyProperty.Register("SpaceBetWeenSegment", typeof(Thickness), typeof(RollingGauge), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

        /// <summary>
        /// Identifies the <see cref="SegmentHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentHorizontalAlignmentProperty =
            DependencyProperty.Register("SegmentHorizontalAlignment", typeof(HorizontalAlignment), typeof(RollingGauge), new PropertyMetadata(HorizontalAlignment.Center, OnSegmentHorizontalAlignmentChanged));

        /// <summary>
        /// Identifies the <see cref="SegmentBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBorderBrushProperty =
            DependencyProperty.Register("SegmentBorderBrush", typeof(Brush), typeof(RollingGauge), new PropertyMetadata(Brushes.Gray, new PropertyChangedCallback(OnSegmentForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBorderThicknessProperty =
            DependencyProperty.Register("SegmentBorderThickness", typeof(Thickness), typeof(RollingGauge), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

        /// <summary>
        /// Identifies the <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(RollingGauge), new PropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        /// Identifies the <see cref="MaxValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(RollingGauge), new PropertyMetadata(1.0, new PropertyChangedCallback(OnMaxValueChanged)));



        /// <summary>
        /// Identifies the <see cref="SegmentCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentCornerRadiusProperty =
            DependencyProperty.Register("SegmentCornerRadius", typeof(CornerRadius), typeof(RollingGauge), new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Identifies the <see cref="IsNumeric"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNumericProperty =
            DependencyProperty.Register("IsNumeric", typeof(bool), typeof(RollingGauge), new PropertyMetadata(false, new PropertyChangedCallback(OnIsNumericChanged)));

        /// <summary>
        /// Identifies the <see cref="IsAutomaticSegmentCountEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAutomaticSegmentCountEnabledProperty =
            DependencyProperty.Register("IsAutomaticSegmentCountEnabled", typeof(bool), typeof(RollingGauge), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsEnableAutomaticSegmentCountChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentCountProperty =
            DependencyProperty.Register("SegmentCount", typeof(int), typeof(RollingGauge), new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnSegmentCountChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register("Value", typeof(string), typeof(RollingGauge), new FrameworkPropertyMetadata(" ", new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBackgroundProperty =
           DependencyProperty.Register("SegmentBackground", typeof(Brush), typeof(RollingGauge), new FrameworkPropertyMetadata(Brushes.Gray, new PropertyChangedCallback(OnSegmentBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentFontSizeProperty =
           DependencyProperty.Register("SegmentFontSize", typeof(double), typeof(RollingGauge), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnSegmentFontSizeChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentForegroundProperty =
           DependencyProperty.Register("SegmentForeground", typeof(Brush), typeof(RollingGauge), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnSegmentForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="Direction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), typeof(RollingGauge), new FrameworkPropertyMetadata(Direction.Clockwise, new PropertyChangedCallback(OnDirectionChanged)));

        /// <summary>
        /// Identifies the <see cref="Unit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
         DependencyProperty.Register("Unit", typeof(string), typeof(RollingGauge), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnUnitChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentTemplateProperty =
         DependencyProperty.Register("SegmentTemplate", typeof(DataTemplate), typeof(RollingGauge), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSegmentTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="UnitPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitPositionProperty =
         DependencyProperty.Register("UnitPosition", typeof(UnitPosition), typeof(RollingGauge), new FrameworkPropertyMetadata(UnitPosition.End, new PropertyChangedCallback(OnUnitPositionChanged)));


        #endregion  Dependency Properties

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="RollingGauge"/> class.
        /// </summary>
        static RollingGauge()
        {
            EnvironmentTest.ValidateLicense(typeof(RollingGauge));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollingGauge), new FrameworkPropertyMetadata(typeof(RollingGauge)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingGauge"/> class.
        /// </summary>
        public RollingGauge()
        {
            this.Segments = new SegmentCollection();
            this.Segments.CollectionChanged += new NotifyCollectionChangedEventHandler(Segments_CollectionChanged);
           System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            //Binding b = new Binding();
            // b.Source = this;
            //b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            // b.Path = new PropertyPath("FontSize");
            // this.SetBinding(RollingGauge.SegmentFontSizeProperty, b);
        }

        /// <summary>
        /// Invoked when Segments collection Changed.
        /// </summary>
        private void Segments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshRollingGauge();
        }

        #endregion Initialization

        #region CLR Getters & Setters

        /// <summary>
        /// Gets or sets IsAutomaticSegmentCountEnabled
        /// </summary>
        public bool IsAutomaticSegmentCountEnabled
        {
            set { SetValue(IsAutomaticSegmentCountEnabledProperty, value); }
            get { return (bool)GetValue(IsAutomaticSegmentCountEnabledProperty); }
        }

        /// <summary>
        /// Gets or sets IsNumeric
        /// </summary>
        public bool IsNumeric
        {
            set { SetValue(IsNumericProperty, value); }
            get { return (bool)GetValue(IsNumericProperty); }
        }

        /// <summary>
        /// Gets or sets MinValue. 
        /// </summary>
        public double MinValue
        {
            set { SetValue(MinValueProperty, value); }
            get { return (double)GetValue(MinValueProperty); }
        }

        /// <summary>
        /// Gets or sets MaxValue. 
        /// </summary>
        public double MaxValue
        {
            set { SetValue(MaxValueProperty, value); }
            get { return (double)GetValue(MaxValueProperty); }
        }

        /// <summary>
        /// Gets or sets SegmentCornerRadius
        /// </summary>
        public CornerRadius SegmentCornerRadius
        {
            get { return (CornerRadius)GetValue(SegmentCornerRadiusProperty); }
            set { SetValue(SegmentCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets SegmentBorderBrush
        /// </summary>
        public Brush SegmentBorderBrush
        {
            get { return (Brush)GetValue(SegmentBorderBrushProperty); }
            set { SetValue(SegmentBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets SegmentBorderThickness
        /// </summary>
        public Thickness SegmentBorderThickness
        {
            get { return (Thickness)GetValue(SegmentBorderThicknessProperty); }
            set { SetValue(SegmentBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or Sets SpaceBetWeenSegment
        /// </summary>
        public Thickness SpaceBetWeenSegment
        {
            get { return (Thickness)GetValue(SpaceBetWeenSegmentProperty); }
            set { SetValue(SpaceBetWeenSegmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets SegmentHorizontalAlignment
        /// </summary>
        /// <value>The segment horizontal alignment.</value>
        public HorizontalAlignment SegmentHorizontalAlignment
        {
            set { SetValue(SegmentHorizontalAlignmentProperty, value); }
            get { return (HorizontalAlignment)GetValue(SegmentHorizontalAlignmentProperty); }
        }

        /// <summary>
        /// Gets or sets SegmentVerticalAlignment
        /// </summary>
        /// <value>The segment vertical alignment.</value>
        public VerticalAlignment SegmentVerticalAlignment
        {
            set { SetValue(SegmentVerticalAlignmentProperty, value); }
            get { return (VerticalAlignment)GetValue(SegmentVerticalAlignmentProperty); }
        }

        /// <summary>
        /// Gets or sets AnimationDelay
        /// </summary>
        /// <value>The animation delay.</value>
        public TimeSpan AnimationDelay
        {
            get { return (TimeSpan)GetValue(AnimationDelayProperty); }
            set { SetValue(AnimationDelayProperty, value); }
        }


        /// <summary>
        /// Gets or sets the gauge's corner radius . This is a dependency property.
        /// </summary>
        /// <value>
        /// CornerRadius
        /// </value>
        public new CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Segment Template
        /// </summary>
        /// <value>Type: <see cref="DataTemplate"/></value>
        public DataTemplate SegmentTemplate
        {
            get { return (DataTemplate)GetValue(SegmentTemplateProperty); }
            set { SetValue(SegmentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Unit
        /// </summary>
        /// <value>Type: <see cref="string"/></value>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of segments;
        /// </summary>
        /// <value>Type: <see cref="SegmentCollection"/></value>
        public SegmentCollection Segments
        {
            get { return m_segments; }
            set { m_segments = value; }
        }

        /// <summary>
        /// Gets or sets the SegmentForeground
        /// </summary>
        /// <value>Type: <see cref="Brush"/></value>
        public Brush SegmentForeground
        {
            get { return (Brush)GetValue(SegmentForegroundProperty); }
            set { SetValue(SegmentForegroundProperty, value); }
        }

        //[Bindable(true)]
        //[Category("Brushes")]
        /// <summary>
        /// Gets or sets the SegmentBackground
        /// </summary>
        /// <value>Type: <see cref="Brush"/></value>
        public Brush SegmentBackground
        {
            get { return (Brush)GetValue(SegmentBackgroundProperty); }
            set { SetValue(SegmentBackgroundProperty, value); }
        }
        /// <summary>
        /// Gets or sets the SegmentFontSize
        /// </summary>
        /// <value>Type: <see cref="double"/></value>
        public double SegmentFontSize
        {
            get { return (double)GetValue(SegmentFontSizeProperty); }
            set { SetValue(SegmentFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        /// <value>Type: <see cref="string"/></value>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SegmentCount
        /// </summary>
        /// <value>Type: <see cref="int"/></value>
        public int SegmentCount
        {
            get { return (int)GetValue(SegmentCountProperty); }
            set { SetValue(SegmentCountProperty, value); }
        }
        /// <summary>
        /// Gets or sets the Direction
        /// </summary>
        /// <value>Type: <see cref="Direction"/></value>
        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Unitposition
        /// </summary>
        /// <value>Type: <see cref="Direction"/></value>
        public UnitPosition UnitPosition
        {
            get { return (UnitPosition)GetValue(UnitPositionProperty); }
            set { SetValue(UnitPositionProperty, value); }
        }


        #endregion CLR Getters & Setters

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(RollingGauge_Loaded);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            frame = this.Template.FindName("SegmentFrame", this) as StackPanel;
            grid = this.Template.FindName("SegmentGrid", this) as Grid;
            //partContainerBorder = this.GetTemplateChild("PART_ContainerBorder") as Border;
            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);
        }

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshRollingGauge();
        }

        #endregion Overrides

        #region Implementation

        /// <summary>
        /// Occurs when control is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void RollingGauge_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddSegments();

        }

        /// <summary>
        /// Initialize the Height and Width of the Segment
        /// </summary>
        internal void Initialize()
        {
            if (this.BorderThickness == new Thickness())
                this.BorderThickness = new Thickness(2, 2, 2, 2);
            if (this.BorderBrush == null)
                this.BorderBrush = Brushes.Black;

            if (this.FontSize == 12)
                this.FontSize = 18;
            double size = this.FontSize * 5 / 3;
            if (this.SizeToContainer)
            {
                if (this.IsAutomaticSegmentCountEnabled)
                {
                    SegmentWidth = this.ActualWidth / (this.Value.Length + this.Unit.Length);
                }
                else
                {
                    SegmentWidth = this.ActualWidth / (this.SegmentCount + this.Unit.Length);
                }
            }
            else if (SegmentWidth == 0)
            {
                if (!this.Height.Equals(double.NaN))
                {
                    SegmentWidth = this.Height;
                }
                else if (this.Width.Equals(double.NaN))
                {

                    SegmentWidth = this.FontSize * 5 / 3;

                }
                else
                {
                    SegmentWidth = size;
                }
            }
        }

        /// <summary>
        /// Adds Segments to Rolling gauge.
        /// </summary>
        internal void AddSegments()
        {
            if (this.frame == null)
            {
                return;
            }
            if (!this._Initialized && this.SizeToContainer)
                SegmentWidth = (this.ActualHeight - this.BorderThickness.Top - this.BorderThickness.Bottom) > 10 ? (this.ActualHeight - this.BorderThickness.Top - this.BorderThickness.Bottom) : 25;
            else
                this.Initialize();

            double panelwidth = 0;
            int characterposition = 0;
            int newcharposition = 0;

            if (this.frame != null)
                this.frame.Children.Clear();
            this._Initialized = true;
            SegmentHeight = this.ActualHeight;
            m_segmentCount = this.SegmentCount;
            if (this.IsNumeric)
            {
                if (!double.TryParse(this.Value, out NumericValue))
                    this.Value = "0";
                if (NumericValue < this.MinValue)
                    this.Value = this.MinValue.ToString();
                if (NumericValue > this.MaxValue)
                    this.Value = this.MaxValue.ToString();
            }
            if (this.Value.Equals(string.Empty))
                this.Value = " ";

            if (this.IsAutomaticSegmentCountEnabled)
                this.m_segmentCount = this.Value.Length;

            if (m_segmentCount <= 0)
                m_segmentCount = 1;

            char[] newValues = new char[m_segmentCount];

            StackPanel segmentStackPanel = new StackPanel();
            RollingCharacter character = new RollingCharacter();

            for (int segmentindex = 0; segmentindex < this.m_segmentCount; segmentindex++)
            {
                segmentStackPanel = new StackPanel();
                character = new RollingCharacter();
                if (segmentindex < (m_segmentCount - this.Value.Length))
                {
                    character.Value = this.IsNumeric ? '0' : ' ';
                    newValues[newcharposition] = this.IsNumeric ? '0' : ' ';
                }
                else
                {
                    newValues[newcharposition] = this.Value[characterposition];
                    character.Value = this.Value[characterposition++];
                }
                SetProperties(character);
                character.CharacterIndex = segmentindex;

                if (this.Segments != null)
                {
                    for (int item = 0; item < this.Segments.Count; item++)
                    {
                        RollingCharacter character1 = this.Segments[item];
                        int index = character1.CharacterIndex;
                        if (index == segmentindex)
                        {
                            character = GetRollingCharacter(character1);
                            if (character1.Value != '\0')
                                character.Value = character1.Value;
                            newValues[index] = character.Value;
                        }
                    }
                }

                character.Width = SegmentWidth - 2;
                character.Height = SegmentHeight;

                #region Corner Radious
                if (segmentindex == 0)
                {
                    if (this.Unit == string.Empty || this.UnitPosition == UnitPosition.End)
                    {
                        double topleft = this.CornerRadius.TopLeft > character.CornerRadius.TopLeft ? this.CornerRadius.TopLeft : character.CornerRadius.TopLeft;
                        double bottomleft = this.CornerRadius.BottomLeft > character.CornerRadius.BottomLeft ? this.CornerRadius.BottomLeft : character.CornerRadius.BottomLeft;
                        double topright = character.CornerRadius.TopRight;
                        double bottomright = character.CornerRadius.BottomRight;
                        //if (this.Unit == string.Empty)
                        //{
                        //    topright = this.CornerRadius.TopRight > character.CornerRadius.TopRight ? this.CornerRadius.TopRight : character.CornerRadius.TopRight;
                        //    bottomright = this.CornerRadius.BottomRight > character.CornerRadius.BottomRight ? this.CornerRadius.BottomRight : character.CornerRadius.BottomRight;
                        //}
                        character.CornerRadius = new CornerRadius(topleft, topright, bottomright, bottomleft);
                    }
                }
                if (segmentindex == this.m_segmentCount - 1 && this.UnitPosition == UnitPosition.Start)
                {
                    if (this.Unit == string.Empty || this.UnitPosition == UnitPosition.Start)
                    {
                        double topleft = character.CornerRadius.TopLeft;
                        double bottomleft = character.CornerRadius.BottomLeft;
                        double topright = this.CornerRadius.TopRight > character.CornerRadius.TopRight ? this.CornerRadius.TopRight : character.CornerRadius.TopRight;
                        double bottomright = this.CornerRadius.BottomRight > character.CornerRadius.BottomRight ? this.CornerRadius.BottomRight : character.CornerRadius.BottomRight;
                        //if (this.Unit == string.Empty)
                        //{
                        //    topleft = this.CornerRadius.TopLeft > character.CornerRadius.TopLeft ? this.CornerRadius.TopLeft : character.CornerRadius.TopLeft;
                        //    bottomleft = this.CornerRadius.BottomLeft > character.CornerRadius.BottomLeft ? this.CornerRadius.BottomLeft : character.CornerRadius.BottomLeft;
                        //}
                        character.CornerRadius = new CornerRadius(topleft, topright, bottomright, bottomleft);
                    }
                }
                #endregion

                segmentStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                segmentStackPanel.VerticalAlignment = VerticalAlignment.Center;
                Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
                FormattedText t = new FormattedText(character.Value.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.SegmentFontSize > 0 ? this.SegmentFontSize : ((this.SegmentHorizontalAlignment == HorizontalAlignment.Stretch || this.SegmentVerticalAlignment == VerticalAlignment.Stretch) ? this.SegmentWidth : (this.SegmentWidth / 2)), this.SegmentForeground);
                Geometry gt = t.BuildGeometry(new Point(0, 0));
                PathGeometry pt = gt.GetFlattenedPathGeometry();
                Path p = new Path();
                p.Data = pt;
                p.Fill = this.SegmentForeground;
                if (this.SegmentFontSize == 0 && (this.SegmentHorizontalAlignment == HorizontalAlignment.Stretch || this.SegmentVerticalAlignment == VerticalAlignment.Stretch))
                {
                    p.Margin = new Thickness(1, this.SegmentHeight / 5 > 20 ? 20 : this.SegmentHeight / 5, 1, this.SegmentHeight / 5 > 20 ? 20 : this.SegmentHeight / 5);
                    p.Stretch = Stretch.Fill;
                }
                else
                {
                    p.Height = this.SegmentWidth;
                }
                character.VerticalAlignment = this.SegmentVerticalAlignment;
                character.HorizontalAlignment = this.SegmentHorizontalAlignment;
                character.Content = p;
                segmentStackPanel.Children.Add(character);
                this.frame.Children.Add(segmentStackPanel);
                panelwidth += character.Width;
                newcharposition++;
            }

            if (!this.Unit.Equals(string.Empty))
            {
                StackPanel UnitStackPanel = new StackPanel();
                UnitStackPanel.Orientation = Orientation.Horizontal;
                for (int unitchar = 0; unitchar < this.Unit.Length; unitchar++)
                {
                    RollingCharacter unitcharacter = new RollingCharacter();
                    SetProperties(unitcharacter);
                    unitcharacter.Width = SegmentWidth - 5;
                    unitcharacter.Height = SegmentHeight;
                    unitcharacter.Value = this.Unit[unitchar];

                    #region Corner Radious
                    if (this.UnitPosition == UnitPosition.Start && unitchar == 0)
                    {
                        unitcharacter.CornerRadius = new CornerRadius(this.CornerRadius.TopLeft, unitcharacter.CornerRadius.TopRight,
                            unitcharacter.CornerRadius.BottomRight, this.CornerRadius.BottomLeft);
                    }
                    if (this.UnitPosition == UnitPosition.End && unitchar == this.Unit.Length - 1)
                    {
                        unitcharacter.CornerRadius = new CornerRadius(unitcharacter.CornerRadius.TopLeft, this.CornerRadius.TopRight,
                            this.CornerRadius.BottomRight, unitcharacter.CornerRadius.BottomLeft);
                    }
                    #endregion
                    Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
                    FormattedText t = new FormattedText(unitcharacter.Value.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.SegmentFontSize > 0 ? this.SegmentFontSize : ((this.SegmentHorizontalAlignment == HorizontalAlignment.Stretch || this.SegmentVerticalAlignment == VerticalAlignment.Stretch) ? this.SegmentWidth : (this.SegmentWidth / 2)), this.SegmentForeground);
                    Geometry gt = t.BuildGeometry(new Point(0, 0));
                    PathGeometry pt = gt.GetFlattenedPathGeometry();
                    Path p = new Path();
                    p.Data = pt;
                    p.Fill = this.SegmentForeground;
                    if (this.SegmentFontSize == 0 && (this.SegmentHorizontalAlignment == HorizontalAlignment.Stretch || this.SegmentVerticalAlignment == VerticalAlignment.Stretch))
                    {
                        p.Margin = new Thickness(1, this.SegmentHeight / 5 > 20 ? 20 : this.SegmentHeight / 5, 1, this.SegmentHeight / 5 > 20 ? 20 : this.SegmentHeight / 5);
                        p.Stretch = Stretch.Fill;
                    }
                    else
                    {
                        p.Height = this.SegmentWidth;
                    }
                    unitcharacter.Margin = new Thickness(0, 0, 0, 0);
                    unitcharacter.Content = p;
                    UnitStackPanel.Children.Add(unitcharacter);
                    unitcharacter.HorizontalContentAlignment = HorizontalAlignment.Center;
                    unitcharacter.VerticalContentAlignment = VerticalAlignment.Center;
                    panelwidth += unitcharacter.Width;
                }
                if (this.UnitPosition == UnitPosition.Start)
                    this.frame.Children.Insert(0, UnitStackPanel);
                else
                    this.frame.Children.Add(UnitStackPanel);
            }
            this.Value = new string(newValues);
            //if (!this.SizeToContainer)
            //{
            //    this.frame.Height = character.Height + this.SpaceBetWeenSegment.Top + this.SpaceBetWeenSegment.Bottom;
            //    this.frame.Width = panelwidth + (((this.m_segmentCount + this.Unit.Length) * this.SpaceBetWeenSegment.Left) +
            //                    ((this.m_segmentCount + this.Unit.Length) * this.SpaceBetWeenSegment.Right));

            //}
            this.frame.VerticalAlignment = VerticalAlignment.Center;
            this.frame.HorizontalAlignment = HorizontalAlignment.Center;
            //if (!this.SizeToContainer)
            //{
            //    this.Width = this.frame.Width + this.BorderThickness.Left + this.BorderThickness.Right;
            //    this.Height = character.Height + this.BorderThickness.Top + this.BorderThickness.Bottom;
            //}
            this.VerticalContentAlignment = VerticalAlignment.Center;
            this.HorizontalContentAlignment = HorizontalAlignment.Center;

        }

        private Path GetPathValue(string val)
        {
            Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
            FormattedText t = new FormattedText(val, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.SegmentFontSize > 0 ? this.SegmentFontSize : ((this.SegmentHorizontalAlignment == HorizontalAlignment.Stretch || this.SegmentVerticalAlignment == VerticalAlignment.Stretch) ? this.SegmentWidth : (this.SegmentWidth / 2)), this.SegmentForeground);
            Geometry gt = t.BuildGeometry(new Point(0, 0));
            PathGeometry pt = gt.GetFlattenedPathGeometry();
            Path p = new Path();
            p.Data = pt;
            p.Fill = this.SegmentForeground;
            if (this.SegmentFontSize == 0 && (this.SegmentHorizontalAlignment == HorizontalAlignment.Stretch || this.SegmentVerticalAlignment == VerticalAlignment.Stretch))
            {
                p.Margin = new Thickness(1, this.SegmentHeight / 5 > 20 ? 20 : this.SegmentHeight / 5, 1, this.SegmentHeight / 5 > 20 ? 20 : this.SegmentHeight / 5);
                p.Stretch = Stretch.Fill;
            }
            else
            {
                p.Height = this.SegmentWidth;
            }
            return p;

        }

        /// <summary>
        /// Refresh the change in Rolling Value
        /// </summary>
        private void RefreshSegmentValue()
        {
            if (this.frame != null && this.frame.Children.Count != 0)
            {
                if (this.IsNumeric)
                {
                    //double temp = 0;
                    double.TryParse(this.Value, out NumericValue);
                    if (this.MinValue > this.MaxValue)
                        this.MinValue = this.MaxValue;

                    if (NumericValue < this.MinValue || NumericValue > this.MaxValue)
                    {
                        if (NumericValue < this.MinValue)
                            this.Value = this.MinValue.ToString();
                        if (NumericValue > this.MaxValue)
                            this.Value = this.MaxValue.ToString();
                        this.RefreshRollingGauge();
                        return;
                    }
                }

                if (this.m_value.Length != this.Value.Length && this.IsAutomaticSegmentCountEnabled)
                {
                    this.RefreshRollingGauge();
                    // return;   
                }

                if (!this.m_value.Equals(this.Value))
                {
                    string newvalue = this.Value;
                    if (this.m_segmentCount > this.m_value.Length)
                    {
                        int diff = this.m_segmentCount - this.m_value.Length;
                        for (int tempposition = 0; tempposition < diff; tempposition++)
                        {
                            String helper = this.IsNumeric ? "0" : " ";
                            m_value = helper + m_value;
                        }
                    }
                    if (newvalue.Length < this.m_segmentCount)
                    {
                        int diff = this.m_segmentCount - newvalue.Length;
                        for (int tempposition = 0; tempposition < diff; tempposition++)
                        {
                            string helper = this.IsNumeric ? "0" : " ";
                            newvalue = helper + newvalue;
                        }
                    }
                    if (this.IsNumeric)
                    {
                        double _oldvalue;
                        double.TryParse(this.m_value, out _oldvalue);
                        double _newvalue;
                        double.TryParse(this.Value, out _newvalue);
                        for (int position = 0; position < this.m_segmentCount; position++)
                        {
                            if (newvalue[position] != this.m_value[position])
                            {
                                this.NumericRotate = true;
                            }
                            if (this.NumericRotate == true)
                            {
                                if (_oldvalue > _newvalue)
                                    RollClockwise(newvalue[position], position, this.m_value[position]);
                                else
                                    RollAntiClockwise(newvalue[position], position, this.m_value[position]);
                            }
                        }
                        this.NumericRotate = false;
                    }
                    else
                    {
                        for (int position = 0; position < this.m_segmentCount; position++)
                        {
                            if (this.Direction == Direction.Clockwise)
                                RollClockwise(newvalue[position], position, this.m_value[position]);
                            else
                                RollAntiClockwise(newvalue[position], position, this.m_value[position]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Roll the Segment in ClockWise Direction
        /// </summary>
        /// <param name="val">Current Value</param>
        /// <param name="index">Index of the Segment in Stackpanel</param>
        /// <param name="oldValue">Old Value</param>
        private void RollClockwise(char val, int index, char oldValue)
        {
            if (!this.Unit.Equals(string.Empty))
            {
                if (this.UnitPosition == UnitPosition.Start)
                {
                    index = index + 1;
                }
            }
            StackPanel segment = (StackPanel)this.frame.Children[index];
            #region Rolling ClockWise
            RollingCharacter element;
            if (segment.Children.Count == 1)
            {
                element = segment.Children[0] as RollingCharacter;
            }
            else
            {
                element = segment.Children[1] as RollingCharacter;
            }
            RollingCharacter character = new RollingCharacter();
            character = GetRollingCharacter(element);
            element.Value = oldValue;
            element.Content = GetPathValue(oldValue.ToString());
            if (element.Value != val || this.NumericRotate == true)
            {
                TransformGroup group = new TransformGroup();
                group = element.RenderTransform as TransformGroup;
                if (group == null)
                {
                    group = new TransformGroup();
                    group.Children.Add(new TranslateTransform());
                    element.RenderTransform = group;
                }
                DoubleAnimation firstanimation = new DoubleAnimation();
                //firstanimation.From = 0;
                firstanimation.From = 0 + ((element.Height + element.Margin.Top + element.Margin.Bottom) / 2);
                //firstanimation.To = -element.Height;
                firstanimation.To = -(element.Height + element.Margin.Top + element.Margin.Bottom) +
                    ((element.Height + element.Margin.Top + element.Margin.Bottom) / 2);
                firstanimation.Duration = this.AnimationDelay;//TimeSpan.FromSeconds(1);

                TranslateTransform transform = group.Children[0] as TranslateTransform;
                transform.BeginAnimation(TranslateTransform.YProperty, firstanimation);
                character.Content = GetPathValue(val.ToString());
                character.Value = val;
                character.GaugeParent = this;
                //adding new value segment to stack panel
                segment.Children.Add(character);

                TransformGroup secondgroup = new TransformGroup();
                secondgroup = character.RenderTransform as TransformGroup;
                if (secondgroup == null)
                {
                    secondgroup = new TransformGroup();
                    secondgroup.Children.Add(new TranslateTransform());
                    character.RenderTransform = secondgroup;
                }
                DoubleAnimation secondanimation = new DoubleAnimation();
                secondanimation.From = 0 + ((element.Height + element.Margin.Top + element.Margin.Bottom) / 2);
                secondanimation.To = -(element.Height + element.Margin.Top + element.Margin.Bottom) +
                    ((element.Height + element.Margin.Top + element.Margin.Bottom) / 2);
                secondanimation.Duration = this.AnimationDelay;//TimeSpan.FromSeconds(1);
                TranslateTransform trans1 = secondgroup.Children[0] as TranslateTransform;
                trans1.BeginAnimation(TranslateTransform.YProperty, secondanimation);

                if (segment.Children.Count == 3)
                {
                    segment.Children.RemoveAt(0);
                }
            }

            #endregion
        }

        /// <summary>
        /// Roll the Segment in AntiClockWise Direction
        /// </summary>
        /// <param name="val">CurrentValue</param>
        /// <param name="index">Index of the segment in the stackpanel</param>
        /// <param name="oldValue">Old Value</param>
        private void RollAntiClockwise(char val, int index, char oldValue)
        {
            if (!this.Unit.Equals(string.Empty))
            {
                if (this.UnitPosition == UnitPosition.Start)
                {
                    index = index + 1;
                }
            }
            StackPanel segment = (StackPanel)this.frame.Children[index];
            RollingCharacter content;
            #region Rolling AntiClockwise
            if (segment.Children.Count == 1)
            {
                content = segment.Children[0] as RollingCharacter;
            }
            else
            {
                content = segment.Children[1] as RollingCharacter;
            }
            RollingCharacter character = new RollingCharacter();
            character = GetRollingCharacter(content);
            content.Value = oldValue;
            content.Content = GetPathValue(oldValue.ToString());
            if (content.Value != val || this.NumericRotate == true)
            {
                character.Content = GetPathValue(val.ToString());
                character.Value = val;
                TransformGroup group = new TransformGroup();
                group = content.RenderTransform as TransformGroup;
                if (group == null)
                {
                    group = new TransformGroup();
                    group.Children.Add(new TranslateTransform());
                    content.RenderTransform = group;
                }
                DoubleAnimation firstanimation = new DoubleAnimation();
                //firstanimation.From = 0;
                firstanimation.From = (content.Height + content.Margin.Top + content.Margin.Bottom) / 2;
                //firstanimation.To = content.Height;
                firstanimation.To = content.Height + content.Margin.Top + content.Margin.Bottom + (content.Height + content.Margin.Top + content.Margin.Bottom) / 2;
                firstanimation.Duration = AnimationDelay;//TimeSpan.FromSeconds(1);

                TranslateTransform transform = group.Children[0] as TranslateTransform;
                transform.BeginAnimation(TranslateTransform.YProperty, firstanimation);


                character.GaugeParent = this;
                segment.Children.Add(character);

                TransformGroup secondgroup = new TransformGroup();
                secondgroup = character.RenderTransform as TransformGroup;
                if (secondgroup == null)
                {
                    secondgroup = new TransformGroup();
                    secondgroup.Children.Add(new TranslateTransform());
                    character.RenderTransform = secondgroup;
                }
                DoubleAnimation secondanimation = new DoubleAnimation();
                //secondanimation.From = -2 * content.Height;
                secondanimation.From = (-2 * (content.Height + content.Margin.Top + content.Margin.Bottom)) +
                    (content.Height + content.Margin.Top + content.Margin.Bottom) / 2;
                //secondanimation.To = -content.Height;
                secondanimation.To = -(content.Height + content.Margin.Top + content.Margin.Bottom) +
                    (content.Height + content.Margin.Top + content.Margin.Bottom) / 2;
                secondanimation.Duration = AnimationDelay;//TimeSpan.FromSeconds(1);
                TranslateTransform secondtransform = secondgroup.Children[0] as TranslateTransform;
                secondtransform.BeginAnimation(TranslateTransform.YProperty, secondanimation);
                if (segment.Children.Count == 3)
                {
                    segment.Children.RemoveAt(0);
                }
                //if (code)
                //{
                //    character.Focus();
                //}
            }

            #endregion
        }

        /// <summary>
        /// Create the new Rolling Character
        /// </summary>
        /// <param name="character1">Specifies the Current properties</param>
        /// <returns></returns>
        private RollingCharacter GetRollingCharacter(RollingCharacter character1)
        {
            RollingCharacter character = new RollingCharacter();
            character.Background = character1.Background;
            character.BorderBrush = character1.BorderBrush;
            character.BorderThickness = character1.BorderThickness;
            character.CharacterIndex = character1.CharacterIndex;
            character.FontFamily = character1.FontFamily;
            character.FontSize = character1.FontSize;
            character.FontStretch = character1.FontStretch;
            character.FontStyle = character1.FontStyle;
            character.FontWeight = character1.FontWeight;
            character.Foreground = character1.Foreground;
            character.CornerRadius = character1.CornerRadius;
            character.Margin = character1.Margin;
            character.Height = character1.Height;
            character.HorizontalAlignment = character1.HorizontalAlignment;
            character.HorizontalContentAlignment = character1.HorizontalContentAlignment;
            character.IsEnabled = character1.IsEnabled;
            character.VerticalAlignment = character1.VerticalAlignment;
            character.Visibility = character1.Visibility;
            character.Height = character1.Height;
            character.FontStretch = character1.FontStretch;
            character.Width = character1.Width;
            return character;
        }

        /// <summary>
        /// Refresh the gauge when PropertyChanges
        /// </summary>
        public void RefreshRollingGauge()
        {
            if (this.frame != null)
            {
                this.frame.Children.Clear();
                this.AddSegments();
            }
        }

        /// <summary>
        /// Set properties for Rolling Character
        /// </summary>
        /// <param name="character">The character.</param>
        private void SetProperties(RollingCharacter character)
        {
            character.GaugeParent = this;
            character.FontSize = this.FontSize;
            character.Background = this.SegmentBackground;
            character.BorderBrush = this.SegmentBorderBrush;
            character.BorderThickness = this.SegmentBorderThickness;
            character.CornerRadius = this.SegmentCornerRadius;
            character.Foreground = this.SegmentForeground;
            character.Margin = this.SpaceBetWeenSegment;
            character.HorizontalAlignment = HorizontalAlignment.Stretch;
            character.VerticalAlignment = VerticalAlignment.Stretch;
            character.FontStretch = this.FontStretch;
            character.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            character.VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        /// <summary>
        /// Called when [segment foreground changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            if (instance._Initialized)
                instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Called when [segment count changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnSegmentCountChanged(e);
            if (instance._Initialized)
                instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Raises the <see cref="E:SegmentCountChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSegmentCountChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentCountChanged != null)
            {
                SegmentCountChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [segment count changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnSegmentVerticalAlignmentChanged(e);
            instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Raises the <see cref="E:SegmentCountChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSegmentVerticalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentVerticalAlignmentChanged != null)
            {
                SegmentVerticalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [segment count changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnSegmentHorizontalAlignmentChanged(e);
            instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Raises the <see cref="E:SegmentCountChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSegmentHorizontalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentHorizontalAlignmentChanged != null)
            {
                SegmentHorizontalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [segment background changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            if (instance._Initialized)
                instance.RefreshRollingGauge();
        }

        private static void OnSegmentFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            //if (instance._Initialized)
            instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnValueChanged(e);
            instance.m_value = e.OldValue.ToString();
            if (instance._Initialized)
                instance.RefreshSegmentValue();
        }

        /// <summary>
        /// Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [direction changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnDirectionChanged(e);

        }

        /// <summary>
        /// Raises the <see cref="E:DirectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DirectionChanged != null)
            {
                this.DirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [unit changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnUnitChanged(e);
            if (instance._Initialized)
                instance.RefreshRollingGauge();

        }

        /// <summary>
        /// Raises the <see cref="E:UnitChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnitChanged(DependencyPropertyChangedEventArgs e)
        {
            if (UnitChanged != null)
            {
                this.UnitChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [segment template changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnSegmentTemplateChanged(e);
            if (instance._Initialized)
                instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Raises the <see cref="E:SegmentTemplateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSegmentTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentTemplateChanged != null)
            {
                SegmentTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [unit position changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnUnitPositionChanged(e);
            if (instance._Initialized)
                instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Raises the <see cref="E:UnitPositionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnitPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (UnitPositionChanged != null)
            {
                UnitPositionChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [is enable automatic segment count changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsEnableAutomaticSegmentCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)sender;
            if (instance._Initialized)
                instance.RefreshRollingGauge();
        }

        /// <summary>
        /// Called when [max value changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge obj = (RollingGauge)sender;
            if (obj.IsNumeric)
            {
                double value;
                double.TryParse(obj.Value.ToString(), out value);
                if (obj.MinValue > (double)e.NewValue)
                    obj.MinValue = (double)e.NewValue;
                if (value > (double)e.NewValue)
                    obj.Value = e.NewValue.ToString();
            }
        }

        /// <summary>
        /// Called when [min value changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMaxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge obj = (RollingGauge)sender;
            if (obj.IsNumeric)
            {
                double value;
                double.TryParse(obj.Value.ToString(), out value);
                if (obj.MaxValue < (double)e.NewValue)
                    obj.MaxValue = (double)e.NewValue;
                if (value < (double)e.NewValue)
                    obj.Value = e.NewValue.ToString();
            }
        }

        /// <summary>
        /// Called when [is numeric changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsNumericChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge obj = (RollingGauge)sender;
            if (obj.IsNumeric)
            {
                if (obj._Initialized)
                    obj.RefreshRollingGauge();
            }
        }
        #endregion Implementation
    }


}


