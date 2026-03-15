// <copyright file="DigitalGauge.cs" company="Syncfusion Software">
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Licensing;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the DigitalGauge UI element.
    /// <remarks>The digital gauge is perfect for displaying output values that uses various character symbols.
    /// A digital gauge can have multiple characters, state indicators, labels and images.
    /// The look and feel of characters can be fully customized by setting the different properties of 
    /// DigitalGauge.
    /// </remarks>
    /// <seealso cref="CircularGauge"/>
    /// <seealso cref="LinearGauge"/>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><Window xmlns:local="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.WPF">
    /// <local:DigitalGauge Name="DigitalGauge1"/></Window></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public partial class DigitalGauge : Control</code></example>
    /// </list>
    /// <para/>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DigitalGauge"/> in XAML.
    /// <code>
    /// <Window x:Class="GaugeControlTesting.Window1"
    ///      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///      xmlns:sync="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.WPF"
    ///      xmlns:sfshared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF" 
    ///      Title="Digital Gauge" Height="250" Width="300">    
    ///  <Grid>    
    ///    <sync:DigitalGauge
    ///                        Name="digitalGauge1"
    ///                        CharacterCount="5"
    ///                        CharacterHeight="50"
    ///                        CenterFrameFillColor="Gray"
    ///                        Foreground="Red"
    ///                        DimmedBrush="Transparent"
    ///                        SkewAngleX="-10"
    ///                        SegmentSpacing="1"
    ///                        CharacterSpacing="5"
    ///                        Value="Error"
    ///                        CharacterType="SegmentFourteen"/>
    ///  </Grid>
    /// </Window>
    /// </code>
    /// <para/>The following example shows how to create a <see cref="DigitalGauge"/> in C#.
    /// <code>
    ///  using System;
    ///  using System.Windows;
    ///  using System.Windows.Controls;
    ///  using Syncfusion.Windows.Gauge;
    ///  using System.Windows.Media;
    ///  namespace GaugeControlTesting
    ///  {
    ///    public partial class Window1 : Window
    ///    {
    ///        public Window1()
    ///        {
    ///            InitializeComponent();
    ///            this.digitalGauge1.CharacterCount = 5;
    ///            this.digitalGauge1.CharacterHeight = 50;
    ///            this.digitalGauge1.CenterFrameFillColor = Colors.Gray;
    ///            this.digitalGauge1.Foreground = new SolidColorBrush( Colors.Red );
    ///            this.digitalGauge1.DimmedBrush = new SolidColorBrush( Colors.Transparent );
    ///            this.digitalGauge1.SkewAngleX = -10;
    ///            this.digitalGauge1.SegmentSpacing = 1;
    ///            this.digitalGauge1.CharacterSpacing = 5;
    ///            this.digitalGauge1.Value = "Error";
    ///            this.digitalGauge1.CharacterType = CharacterType.SegmentFourteen;
    ///        }
    ///    }
    ///  }
    /// </code>
    /// </example>
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/MetroStyle.xaml")]
    public class DigitalGauge : GaugeBase
    {
        #region Private Members
        /// <summary>
        /// Dictionary that contains character definitions.
        /// </summary>
        private Dictionary<char, List<bool>> m_characterDictionary;

        /// <summary>
        /// Collection of character elements.
        /// </summary>
        private CharacterCollection m_characters;

        private Path mainglasspath;
        private Path secondglasspath;
        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets the collection of character elements.
        /// </summary>
        /// <value>
        /// Type: <see cref="CharacterCollection"/>
        /// </value>
        internal CharacterCollection Characters
        {
            get
            {
                return m_characters;
            }

            set
            {
                m_characters = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="DigitalGauge"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static DigitalGauge()
        {
            EnvironmentTest.ValidateLicense(typeof(DigitalGauge));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DigitalGauge), new FrameworkPropertyMetadata(typeof(DigitalGauge)));
            ForegroundProperty.OverrideMetadata(typeof(DigitalGauge), new FrameworkPropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnForegroundChanged)));
            Control.BackgroundProperty.OverrideMetadata(typeof(DigitalGauge), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundChanged)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalGauge"/> class.
        /// </summary>
        public DigitalGauge()
        {
            this.Characters = new CharacterCollection();
            this.Characters.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.InitializeDictionaryList();
        }
        #endregion Initialization

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="CharacterCount"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterCountChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterSpacing"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterSpacingChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="DimmedBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DimmedBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentSpacing"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentSpacingChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="SkewAngleX"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SkewAngleXChanged;

        /// <summary>
        /// Event that is raised when <see cref="SkewAngleY"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SkewAngleYChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;
        #endregion Events

        #region  Dependency Properties
        /// <summary>
        /// Identifies the <see cref="CharacterCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterCountProperty =
            DependencyProperty.Register("CharacterCount", typeof(int), typeof(DigitalGauge), new FrameworkPropertyMetadata(2, new PropertyChangedCallback(OnCharacterCountChanged)));

        /// <summary>
        /// Identifies the <see cref="CharacterHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterHeightProperty =
            DependencyProperty.Register("CharacterHeight", typeof(double), typeof(DigitalGauge), new FrameworkPropertyMetadata(30d, new PropertyChangedCallback(OnCharacterHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterSpacingProperty =
            DependencyProperty.Register("CharacterSpacing", typeof(double), typeof(DigitalGauge), new FrameworkPropertyMetadata(2d, new PropertyChangedCallback(OnCharacterSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="CharacterType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterTypeProperty =
            DependencyProperty.Register("CharacterType", typeof(CharacterType), typeof(DigitalGauge), new FrameworkPropertyMetadata(CharacterType.SegmentSeven, new PropertyChangedCallback(OnCharacterTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="DimmedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DimmedBrushProperty =
            DependencyProperty.Register("DimmedBrush", typeof(Brush), typeof(DigitalGauge), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDimmedBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof(DigitalGaugeFrameType), typeof(DigitalGauge), new FrameworkPropertyMetadata(DigitalGaugeFrameType.Rectangle, new PropertyChangedCallback(OnFrameTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(DigitalGauge), new FrameworkPropertyMetadata(0.5d, new PropertyChangedCallback(OnSegmentSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentWidthProperty =
            DependencyProperty.Register("SegmentWidth", typeof(double), typeof(DigitalGauge), new FrameworkPropertyMetadata(2d, new PropertyChangedCallback(OnSegmentWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="SkewAngleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewAngleXProperty =
            DependencyProperty.Register("SkewAngleX", typeof(double), typeof(DigitalGauge), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSkewAngleXChanged)));

        /// <summary>
        /// Identifies the <see cref="SkewAngleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewAngleYProperty =
            DependencyProperty.Register("SkewAngleY", typeof(double), typeof(DigitalGauge), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSkewAngleYChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(DigitalGauge), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnValueChanged)));
        #endregion  Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the count of characters.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Default value is 2.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public int CharacterCount
        {
            get
            {
                return (int)this.GetValue(CharacterCountProperty);
            }

            set
            {
                this.SetValue(CharacterCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of characters.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 30.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public double CharacterHeight
        {
            get
            {
                return (double)this.GetValue(CharacterHeightProperty);
            }

            set
            {
                this.SetValue(CharacterHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between characters.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 2.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public double CharacterSpacing
        {
            get
            {
                return (double)this.GetValue(CharacterSpacingProperty);
            }

            set
            {
                this.SetValue(CharacterSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether character should 
        /// contain seven or fourteen segments. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CharacterType"/>
        /// Default value is CharacterType.SegmentSeven.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public CharacterType CharacterType
        {
            get
            {
                return (CharacterType)this.GetValue(CharacterTypeProperty);
            }

            set
            {
                this.SetValue(CharacterTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush used for drawing of dim segments.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        public Brush DimmedBrush
        {
            get
            {
                return (Brush)this.GetValue(DimmedBrushProperty);
            }

            set
            {
                this.SetValue(DimmedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the frame style of Digital Gauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DigitalGaugeFrameType"/>
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public DigitalGaugeFrameType FrameType
        {
            get
            {
                return (DigitalGaugeFrameType)GetValue(FrameTypeProperty);
            }

            set
            {
                SetValue(FrameTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between character segments.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.5.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public double SegmentSpacing
        {
            get
            {
                return (double)this.GetValue(SegmentSpacingProperty);
            }

            set
            {
                this.SetValue(SegmentSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of character segments.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 2.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public double SegmentWidth
        {
            get
            {
                return (double)this.GetValue(SegmentWidthProperty);
            }

            set
            {
                this.SetValue(SegmentWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle to skew the characters along the x-axis.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public double SkewAngleX
        {
            get
            {
                return (double)this.GetValue(SkewAngleXProperty);
            }

            set
            {
                this.SetValue(SkewAngleXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle to skew the characters along the y-axis.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public double SkewAngleY
        {
            get
            {
                return (double)this.GetValue(SkewAngleYProperty);
            }

            set
            {
                this.SetValue(SkewAngleYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that digital gauge should display.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// </value>
        [Bindable(true)]
        [Category("Behavior")]
        public object Value
        {
            get
            {
                return (object)this.GetValue(ValueProperty);
            }

            set
            {
                this.SetValue(ValueProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Overrides
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(DigitalGaugeLoaded);
        }

        /// <summary>
        /// Builds the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainglasspath = this.GetTemplateChild("GlassPath") as Path;
            this.secondglasspath = this.GetTemplateChild("SecondGlassPath") as Path;

            this.RefreshBorders();
            this.RefreshCharacterSegments();
            this.RefreshCharacterProperties();

            if (this.PART_ContainerBorder != null)
            {
                GaugeAdorner = new CircularGaugeAdorner(this.PART_ContainerBorder);
            }
            else
            {
                GaugeAdorner = new CircularGaugeAdorner(this);
            }

            GaugeAdorner.GaugeParent = this;
            this.RefreshAdornerLayer();
        }

        /// <summary>
        /// Refreshes the Adorner layer.
        /// </summary>
        protected override void RefreshAdornerLayer()
        {
            base.RefreshAdornerLayer();
            if (mainglasspath != null)
            {
                if (this.ActualHeight != 0)
                {
                    this.mainglasspath.Height = 2 * ((this.ActualHeight - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left)) / 3;
                    this.mainglasspath.Width = this.ActualWidth - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left;
                    this.secondglasspath.Width = this.ActualWidth - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left;
                    TranslateTransform transform = new TranslateTransform(0, this.ActualHeight / 3 - this.SecondFrameThickness.Top * 2);
                    this.mainglasspath.RenderTransform = transform;
                }
                if (SkinStorage.GetVisualStyle(this).Equals("Blend"))
                {
                    this.mainglasspath.Opacity = 0.1;
                    this.secondglasspath.Opacity = 0.1;

                }
                else
                {
                    this.mainglasspath.Opacity = 0.4;
                    this.secondglasspath.Opacity = 0.4;

                }

            }
        }

        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Adds characters to digital gauge if needed.
        /// </summary>
        private void AddCharacters()
        {
            if (this.Characters.Count < this.CharacterCount)
            {
                while (this.Characters.Count != this.CharacterCount)
                {
                    if (this.CharacterType == CharacterType.SegmentSeven)
                    {
                        this.Characters.Add(new CharacterSeven());
                    }
                    else
                    {
                        this.Characters.Add(new CharacterFourteen());
                    }
                }
            }

            this.RefreshCharacterProperties();
        }

        /// <summary>
        /// Occurs when control is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void DigitalGaugeLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshCharacterSegments();
            this.RefreshCharacterProperties();
        }

        /// <summary>
        /// Initializes character definitions.
        /// </summary>
        private void InitializeDictionaryList()
        {
            m_characterDictionary = new Dictionary<char, List<bool>>();

            m_characterDictionary.Add('0', new List<bool>() { true, true, true, true, true, true, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('1', new List<bool>() { false, false, true, true, false, false, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('2', new List<bool>() { false, true, true, false, true, true, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('3', new List<bool>() { false, true, true, true, true, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('4', new List<bool>() { true, false, true, true, false, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('5', new List<bool>() { true, true, false, true, true, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('6', new List<bool>() { true, true, false, true, true, true, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('7', new List<bool>() { false, true, true, true, false, false, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('8', new List<bool>() { true, true, true, true, true, true, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('9', new List<bool>() { true, true, true, true, true, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('a', new List<bool>() { true, true, true, true, false, true, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('b', new List<bool>() { false, true, true, true, true, false, true, true, true, true, false, false, false, false });
            m_characterDictionary.Add('c', new List<bool>() { true, true, false, false, true, true, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('d', new List<bool>() { false, true, true, true, true, false, false, false, true, true, false, false, false, false });
            m_characterDictionary.Add('e', new List<bool>() { true, true, false, false, true, true, true, false, false, false, false, false, false, false });
            m_characterDictionary.Add('f', new List<bool>() { true, true, false, false, false, true, true, false, false, false, false, false, false, false });
            m_characterDictionary.Add('g', new List<bool>() { true, true, false, true, true, true, false, true, false, false, false, false, false, false });
            m_characterDictionary.Add('h', new List<bool>() { true, false, true, true, false, true, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('i', new List<bool>() { false, true, false, false, true, false, false, false, true, true, false, false, false, false });
            m_characterDictionary.Add('j', new List<bool>() { false, false, true, true, true, true, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('k', new List<bool>() { true, false, false, false, false, true, true, false, false, false, false, true, false, true });
            m_characterDictionary.Add('l', new List<bool>() { true, false, false, false, true, true, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('m', new List<bool>() { true, false, true, true, false, true, false, false, false, false, true, true, false, false });
            m_characterDictionary.Add('n', new List<bool>() { true, false, true, true, false, true, false, false, false, false, true, false, false, true });
            m_characterDictionary.Add('o', new List<bool>() { true, true, true, true, true, true, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('p', new List<bool>() { true, true, true, false, false, true, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('q', new List<bool>() { true, true, true, true, false, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('r', new List<bool>() { true, true, false, false, false, true, true, false, false, false, false, true, false, true });
            m_characterDictionary.Add('s', new List<bool>() { true, true, false, true, true, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('t', new List<bool>() { false, true, false, false, false, false, false, false, true, true, false, false, false, false });
            m_characterDictionary.Add('u', new List<bool>() { true, false, true, true, true, true, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add('v', new List<bool>() { true, false, false, false, false, true, false, false, false, false, false, true, true, false });
            m_characterDictionary.Add('w', new List<bool>() { true, false, true, true, false, true, false, false, false, false, false, false, true, true });
            m_characterDictionary.Add('x', new List<bool>() { false, false, false, false, false, false, false, false, false, false, true, true, true, true });
            m_characterDictionary.Add('y', new List<bool>() { false, false, false, false, false, false, false, false, false, true, true, true, false, false });
            m_characterDictionary.Add('z', new List<bool>() { false, true, false, false, true, false, false, false, false, false, false, true, true, false });
            m_characterDictionary.Add('-', new List<bool>() { false, false, false, false, false, false, true, true, false, false, false, false, false, false });
            m_characterDictionary.Add('+', new List<bool>() { false, false, false, false, false, false, true, true, true, true, false, false, false, false });
            m_characterDictionary.Add('*', new List<bool>() { false, false, false, false, false, false, true, false, false, false, false, false, false, false });
            m_characterDictionary.Add('|', new List<bool>() { false, false, false, false, false, false, false, false, true, true, false, false, false, false });
            m_characterDictionary.Add('\\', new List<bool>() { false, false, false, false, false, false, false, false, false, false, true, false, false, true });
            m_characterDictionary.Add('/', new List<bool>() { false, false, false, false, false, false, false, false, false, false, false, true, true, false });
            m_characterDictionary.Add('_', new List<bool>() { false, false, false, false, true, false, false, false, false, false, false, false, false, false });
            m_characterDictionary.Add(' ', new List<bool>() { false, false, false, false, false, false, false, false, false, false, false, false, false, false });
        }

        /// <summary>
        /// Updates property value cache and raises CharacterCountChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterCountChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Characters.Count > this.CharacterCount)
            {
                while (this.Characters.Count != this.CharacterCount)
                {
                    this.Characters.RemoveAt(this.Characters.Count - 1);
                }
            }
            else
            {
                this.AddCharacters();
            }

            this.RefreshCharacterSegments();
            if (CharacterCountChanged != null)
            {
                CharacterCountChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCharacterCountChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnCharacterCountChanged(e);
        }

        /// <summary>
        /// Calls OnCharacterHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnCharacterHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CharacterHeightChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.CharacterHeight = this.CharacterHeight;
                ch.InvalidateVisual();
            }

            if (CharacterHeightChanged != null)
            {
                CharacterHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCharacterSpacingChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnCharacterSpacingChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CharacterSpacingChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.CharactersSpacing = this.CharacterSpacing;
                ch.InvalidateVisual();
            }

            if (CharacterSpacingChanged != null)
            {
                CharacterSpacingChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises CharacterTypeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Characters.Clear();
            this.AddCharacters();
            this.RefreshCharacterSegments();
            if (CharacterTypeChanged != null)
            {
                this.CharacterTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCharacterTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnCharacterTypeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises DimmedBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDimmedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.DimmedBrush = this.DimmedBrush;
                ch.InvalidateVisual();
            }

            if (DimmedBrushChanged != null)
            {
                this.DimmedBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDimmedBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDimmedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnDimmedBrushChanged(e);
        }

        /// <summary>
        /// Calls OnForegroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnForegroundChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ForegroundChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.ForegroundBrush = this.Foreground;
                ch.InvalidateVisual();
            }

            //if (ForegroundChanged != null)
            //{
            //    this.ForegroundChanged(this, e);
            //}
        }

        /// <summary>
        /// Calls OnFrameTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnFrameTypeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises FrameTypeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFrameTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            this.RefreshCharacterSegments();
            this.RefreshCharacterProperties();
            if (FrameTypeChanged != null)
            {
                FrameTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSegmentSpacingChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnSegmentSpacingChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SegmentSpacingChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSegmentSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.SegmentsSpacing = this.SegmentSpacing;
                ch.InvalidateVisual();
            }

            if (SegmentSpacingChanged != null)
            {
                SegmentSpacingChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises SegmentWidthChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSegmentWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.SegmentWidth = this.SegmentWidth;
                ch.InvalidateVisual();
            }

            if (SegmentWidthChanged != null)
            {
                this.SegmentWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSegmentWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSegmentWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnSegmentWidthChanged(e);
        }

        /// <summary>
        /// Calls OnSkewAngleXChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSkewAngleXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnSkewAngleXChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SkewAngleXChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSkewAngleXChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshCharacterProperties();
            if (SkewAngleXChanged != null)
            {
                this.SkewAngleXChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSkewAngleYChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSkewAngleYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnSkewAngleYChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SkewAngleYChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSkewAngleYChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshCharacterProperties();
            if (SkewAngleYChanged != null)
            {
                this.SkewAngleYChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ValueChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshCharacterSegments();
            if (ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnValueChanged(e);
        }

        /// <summary>
        /// Refreshes character properties.
        /// </summary>
        private void RefreshCharacterProperties()
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.SegmentsSpacing = this.SegmentSpacing;
                ch.SegmentWidth = this.SegmentWidth;
                ch.ForegroundBrush = this.Foreground;
                ch.DimmedBrush = this.DimmedBrush;
                ch.CharacterHeight = this.CharacterHeight;
                ch.CharactersSpacing = this.CharacterSpacing;
                ch.RenderTransform = new SkewTransform(this.SkewAngleX, this.SkewAngleY);
            }
        }

        /// <summary>
        /// Characters are set according to the <see cref="Value"/> property.
        /// </summary>
        private void RefreshCharacterSegments()
        {
            string str = this.Value.ToString().ToLower();
            this.ResetCharacterSegments();

            for (int i = 0; i < str.Length; i++)
            {
                if (str.Length - i - 1 >= 0 && str.Length - i - 1 < this.Characters.Count)
                {
                    if (str[i] == ':' || str[i] == '.' || str[i] == ',')
                    {
                        if (!this.Characters[str.Length - i - 1].DrawColon)
                        {
                            this.ResetCharacterSegments();
                            int strIndex = i;
                            int chindex = i;
                            while (str.Length - chindex - 1 < this.Characters.Count && strIndex >= 1)
                            {
                                if (m_characterDictionary.ContainsKey(str[strIndex - 1]))
                                {
                                    this.Characters[str.Length - chindex - 1].Segments = m_characterDictionary[str[strIndex - 1]];
                                    chindex--;
                                }
                                else if (str[strIndex - 1] == ':')
                                {
                                    this.Characters[str.Length - chindex - 1].DrawColon = true;
                                }
                                else if (str[strIndex - 1] == '.' || str[strIndex - 1] == ',')
                                {
                                    this.Characters[str.Length - chindex - 1].DrawDot = true;
                                }

                                strIndex--;
                            }

                            this.Characters[str.Length - i - 1].DrawColon = str[i] == ':';
                            this.Characters[str.Length - i - 1].DrawDot = str[i] == '.' || str[i] == ',';
                        }
                    }
                    else if (m_characterDictionary.ContainsKey(str[i]))
                    {
                        this.Characters[str.Length - i - 1].Segments = m_characterDictionary[str[i]];
                        this.Characters[str.Length - i - 1].InvalidateVisual();
                    }
                }
            }
        }

        /// <summary>
        /// All character segements are dim.
        /// </summary>
        private void ResetCharacterSegments()
        {
            for (int i = 0; i < this.Characters.Count; i++)
            {
                this.Characters[i].Segments = new List<bool>() { false, false, false, false, false, false, false, false, false, false, false, false, false, false };
                this.Characters[i].DrawColon = false;
                this.Characters[i].DrawDot = false;
                this.Characters[i].InvalidateVisual();
            }
        }

        /// <summary>
        /// Invoked when <see cref="Control.BackgroundProperty"/> of the gauge is changed.
        /// </summary>
        /// <param name="sender">The <see cref="DigitalGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = sender as DigitalGauge;
            instance.RefreshBorders();
        }
        #endregion Implementation
    }
}
