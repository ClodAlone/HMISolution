#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the RollingGauge UI element.
    /// </summary>
    /// <remarks>
    /// 	<para></para>
    /// 	<example>
    /// 		<para>The following example shows how to create a <see cref="RollingGauge">RollingGauge</see> in C#.</para>
    /// 		<para></para>
    /// 		<para></para>
    /// 		<para></para>
    /// 		<para></para>
    /// 		<list type="table">
    /// 			<listheader><term>C# :</term></listheader>
    /// 			<item>
    /// 				<description>
    /// 					<para></para>
    /// 					<para>using Syncfusion.Windows.Gauge;</para>
    /// 					<para>namespace RollingGaugeDemo</para>
    /// 					<para>{</para>
    /// 					<para>public partial class MainPage : UserControl</para>
    /// 					<para>{</para>
    /// 					<para>public MainPage()</para>
    /// 					<para>{</para>
    /// 					<para>InitializeComponent();</para>
    /// 					<para>RollingGauge rollingGauge = new RollingGauge();</para>
    /// 					<para>rollingGauge.SegmentCount = 4;</para>
    /// 					<para>rollingGauge.Value = "1000";</para>
    /// 					<para>rollingGauge.Unit = "KM";</para>
    /// 					<para>rollingGauge.UnitPosition = UnitPosition.End;</para>
    /// 					<para>RollingCharacter character = new RollingCharacter();</para>
    /// 					<para>character.CharacterIndex = 0;</para>
    /// 					<para>character.Value = "0";</para>
    /// 					<para>character.Margin = new Thickness(1);</para>
    /// 					<para>rollingGauge.Segments.Add(character);</para>
    /// 					<para>rollingGauge.Background = new SolidColorBrush(Colors.Blue);</para>
    /// 					<para>this.Content = rollingGauge;</para>
    /// 					<para>}</para>
    /// 					<para>}</para>
    /// 					<para>}</para>
    /// 				</description>
    /// 			</item>
    /// 		</list>
    /// 	</example>
    /// 	<para></para>
    /// </remarks>
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    //  Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Blend;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Default;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Office2003;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.VS2010;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    //    Type = typeof(RollingGauge), XamlResource = "/Syncfusion.Theming.Metro;component/Gauge.xaml")]
    //[StyleTypedProperty(Property = "local:RollingCharacter", StyleTargetType = typeof(RollingCharacter))]
    public class RollingGauge : Control
    {
        #region Private Members
        ///<summary>
        ///Collection of Segments.
        ///</summary>
        private SegmentCollection m_segments = new SegmentCollection();

        ///<summary>
        ///Old value
        ///</summary>
        private string m_value;

        ///<summary>
        ///Gauge value
        ///</summary>
        internal string m_GaugeValue=" ";

        ///<summary>
        ///Segment frame.
        ///</summary>
        private StackPanel frame;
        private const double CdefaultHeight = 40;
        private const double CdefaultWidth = 300;
        private int m_segmentCount = 0;
        double SegmentWidth = 0;
        private bool NumericRotate = false;
        private bool initializer=false;

        #endregion private Members

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="SegmentCount"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentCountChanged;

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
        /// Identifies the <see cref="IsAutomaticSegmentCountEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAutomaticSegmentCountEnabledProperty =
            DependencyProperty.Register("IsAutomaticSegmentCountEnabled", typeof(bool), typeof(RollingGauge), new PropertyMetadata(false, new PropertyChangedCallback(OnIsEnableAutomaticSegmentCountChanged)));

        /// <summary>
        /// Identifies the <see cref="IsNumeric"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNumericProperty =
            DependencyProperty.Register("IsNumeric", typeof(bool), typeof(RollingGauge), new PropertyMetadata(false, new PropertyChangedCallback(OnIsNumericChanged)));

        /// <summary>
        /// Identifies the <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(RollingGauge), new PropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        /// Identifies the <see cref="MaxValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(RollingGauge), new PropertyMetadata(10000.0, new PropertyChangedCallback(OnMaxValueChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentCornerRadiusProperty =
            DependencyProperty.Register("SegmentCornerRadius", typeof(CornerRadius), typeof(RollingGauge), new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Identifies the <see cref="SegmentBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBorderBrushProperty =
            DependencyProperty.Register("SegmentBorderBrush", typeof(Brush), typeof(RollingGauge), new PropertyMetadata(new SolidColorBrush(),new PropertyChangedCallback(OnAppearenceChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBorderThicknessProperty =
            DependencyProperty.Register("SegmentBorderThickness", typeof(Thickness), typeof(RollingGauge), new PropertyMetadata(new Thickness(),new PropertyChangedCallback(OnAppearenceChanged)));

        /// <summary>
        /// Identifies the <see cref="SpaceBetWeenSegment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpaceBetWeenSegmentProperty =
            DependencyProperty.Register("SpaceBetWeenSegment", typeof(Thickness), typeof(RollingGauge), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Identifies the <see cref="SegmentHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentHorizontalAlignmentProperty =
            DependencyProperty.Register("SegmentHorizontalAlignment", typeof(HorizontalAlignment), typeof(RollingGauge), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the <see cref="SegmentVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentVerticalAlignmentProperty =
            DependencyProperty.Register("SegmentVerticalAlignment", typeof(VerticalAlignment), typeof(RollingGauge), new PropertyMetadata(VerticalAlignment.Stretch));

        /// <summary>
        /// Identifies the <see cref="AnimationDelay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDelayProperty =
            DependencyProperty.Register("AnimationDelay", typeof(TimeSpan), typeof(RollingGauge), new PropertyMetadata(TimeSpan.FromSeconds(1)));

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RollingGauge), new PropertyMetadata(new CornerRadius()));

        /// <summary>
        // /// Identifies the <see cref="SegmentBakground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBackgroundProperty =
           DependencyProperty.Register("SegmentBackground", typeof(Brush), typeof(RollingGauge), new PropertyMetadata(new SolidColorBrush(Colors.Gray),new PropertyChangedCallback(OnAppearenceChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentCount"/> dependency property.
        /// </summary>

        public static readonly DependencyProperty SegmentCountProperty =
            DependencyProperty.Register("SegmentCount", typeof(int), typeof(RollingGauge), new PropertyMetadata(1, new PropertyChangedCallback(OnSegmentCountChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register("Value", typeof(string), typeof(RollingGauge), new PropertyMetadata(" ", new PropertyChangedCallback(OnValueChanged)));

       
        /// <summary>
        /// Identifies the <see cref="SegmentForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentForegroundProperty =
           DependencyProperty.Register("SegmentForeground", typeof(Brush), typeof(RollingGauge), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnSegmentForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="Direction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), typeof(RollingGauge), new PropertyMetadata(Direction.Clockwise, new PropertyChangedCallback(OnDirectionChanged)));

        /// <summary>
        /// Identifies the <see cref="Unit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(RollingGauge), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnUnitChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentTemplateProperty =
         DependencyProperty.Register("SegmentTemplate", typeof(DataTemplate), typeof(RollingGauge), new PropertyMetadata(null, new PropertyChangedCallback(OnSegmentTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="UnitPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitPositionProperty =
         DependencyProperty.Register("UnitPosition", typeof(UnitPosition), typeof(RollingGauge), new PropertyMetadata(UnitPosition.End, new PropertyChangedCallback(OnUnitPositionChanged)));

        // Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(GaugeVisualStyle), typeof(RollingGauge), new PropertyMetadata(GaugeVisualStyle.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        #endregion  Dependency Properties

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="RollingGauge"/> class.
        /// </summary>
        static RollingGauge()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingGauge"/> class.
        /// </summary>
        public RollingGauge()
        {
            DefaultStyleKey = typeof(RollingGauge);
            //this.Segments = new SegmentCollection();
            this.Segments.CollectionChanged += new NotifyCollectionChangedEventHandler(Segments_CollectionChanged);
            //this.Loaded+=new RoutedEventHandler(RollingGauge_Loaded);
        }

        void Segments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshRollingGauge();
        }
        #endregion Initialization

        #region CLR Getters & Setters

        /// <summary>
        /// Gets or sets the SIsNumeric
        /// </summary>
        /// <value>Type: <see cref="bool"/></value>
        public bool IsNumeric
        {
            set { SetValue(IsNumericProperty, value); }
            get { return (bool)GetValue(IsNumericProperty); }
        }

        /// <summary>
        /// Gets or sets the IsAutomaticSegmentCountEnabled
        /// </summary>
        /// <value>Type: <see cref="bool"/></value>
        public bool IsAutomaticSegmentCountEnabled
        {
            set { SetValue(IsAutomaticSegmentCountEnabledProperty, value); }
            get { return (bool)GetValue(IsAutomaticSegmentCountEnabledProperty); }
        }

        /// <summary>
        /// Gets or sets the MinValue
        /// </summary>
        /// <value>Type: <see cref="double"/></value>
        public double MinValue
        {
            set { SetValue(MinValueProperty, value); }
            get { return (double)GetValue(MinValueProperty); }
        }

        /// <summary>
        /// Gets or sets the MaxValue
        /// </summary>
        /// <value>Type: <see cref="double"/></value>
        public double MaxValue
        {
            set { SetValue(MaxValueProperty, value); }
            get { return (double)GetValue(MaxValueProperty); }
        }

        /// <summary>
        /// Gets or sets the SegmentCornerRadius
        /// </summary>
        /// <value>Type: <see cref="CornerRadius"/></value>
        public CornerRadius SegmentCornerRadius
        {
            get { return (CornerRadius)GetValue(SegmentCornerRadiusProperty); }
            set { SetValue(SegmentCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SegmentBorderBrush
        /// </summary>
        /// <value>Type: <see cref="Brush"/></value>
        public Brush SegmentBorderBrush
        {
            get { return (Brush)GetValue(SegmentBorderBrushProperty); }
            set { SetValue(SegmentBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SegmentBorderThickness
        /// </summary>
        /// <value>Type: <see cref="Thickness"/></value>
        public Thickness SegmentBorderThickness
        {
            get { return (Thickness)GetValue(SegmentBorderThicknessProperty); }
            set { SetValue(SegmentBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SpaceBetWeenSegment
        /// </summary>
        /// <value>Type: <see cref="Thickness"/></value>
        public Thickness SpaceBetWeenSegment
        {
            get { return (Thickness)GetValue(SpaceBetWeenSegmentProperty); }
            set { SetValue(SpaceBetWeenSegmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SegmentHorizontalAlignment
        /// </summary>
        /// <value>Type: <see cref="HorizontalAlignment"/></value>
        public HorizontalAlignment SegmentHorizontalAlignment
        {
            set { SetValue(SegmentHorizontalAlignmentProperty, value); }
            get { return (HorizontalAlignment)GetValue(SegmentHorizontalAlignmentProperty); }
        }

        /// <summary>
        /// Gets or sets the SegmentVerticalAlignment
        /// </summary>
        /// <value>Type: <see cref="VerticalAlignment"/></value>
        public VerticalAlignment SegmentVerticalAlignment
        {
            set { SetValue(SegmentHorizontalAlignmentProperty, value); }
            get { return (VerticalAlignment)GetValue(SegmentVerticalAlignmentProperty); }
        }

        /// <summary>
        /// Gets or sets the AnimationDelay
        /// </summary>
        /// <value>Type: <see cref="TimeSpan"/></value>
        public TimeSpan AnimationDelay
        {
            get { return (TimeSpan)GetValue(AnimationDelayProperty); }
            set { SetValue(AnimationDelayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CornerRadius
        /// </summary>
        /// <value>Type: <see cref="CornerRadius"/></value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

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
        /// Gets or sets the Segment Template
        /// </summary>
        //  /// <value>Type: <see cref="DateTemplate"/></value>
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


        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        /// <value>Type: <see cref="string"/></value>
        public string Value
        {
            get 
            {
                return this.m_GaugeValue; 
            }
            set 
            {
                SetValue(ValueProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the SegmentCount
        /// </summary>
        /// <value>Type: <see cref="int"/></value>
        public int SegmentCount
        {
            get{return (int)GetValue(SegmentCountProperty);}
            set { SetValue(SegmentCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the unit position.
        /// </summary>
        /// <value>The unit position.</value>
        public UnitPosition UnitPosition
        {
            get { return (UnitPosition)GetValue(UnitPositionProperty); }
            set { SetValue(UnitPositionProperty, value); }
        }

        public GaugeVisualStyle VisualStyle
        {
            get { return (GaugeVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        #endregion CLR Getters & Setters

        #region Implementation

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            frame = this.GetTemplateChild("PART_SegmentFrame") as StackPanel;
            this.UpdateVisualStyle();
            this.AddSegments();
        }

        /// <summary>
        /// Occurs when control is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        void RollingGauge_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Initialize the width and height of Segments.
        /// </summary>
        private void InitializeGauge()
        {
            if (this.BorderBrush == null)
                this.BorderBrush = new SolidColorBrush(Colors.Black);
            if (this.BorderThickness == new Thickness())
                this.BorderThickness = new Thickness(2, 2, 2, 2);
            if (this.frame != null)
            {
                this.frame.Children.Clear();
            }
            if (this.FontSize == 11 && this.Width.Equals(double.NaN) && this.Height.Equals(double.NaN))
            {
                this.FontSize = 18;
                double size = this.FontSize * 5 / 3;
                SegmentWidth = size;
            }

            else if (!this.Width.Equals(double.NaN))
            {
                SegmentWidth = (this.Width - (this.SegmentWidth/40)) / this.SegmentCount;
                this.FontSize = SegmentWidth * 3 / 5;
            }
            else if (!this.Height.Equals(double.NaN))
            {
                SegmentWidth = this.Height - (this.Height/60);
                this.FontSize = SegmentWidth * 3 / 5;
            }
            else if (this.FontSize != 11)
            {
                double size = this.FontSize * 5 / 3;
                SegmentWidth = size;
            }
        }

        /// <summary>
        /// Adds Segments to Rolling gauge.
        /// </summary>
        private void AddSegments()
        {
            
            if (!initializer)
            {
                InitializeGauge();
                initializer = true;
            }

            if (this.IsNumeric)
            {
                double NumericValue;
                if (!double.TryParse(this.Value, out NumericValue))
                {
                    this.m_GaugeValue = "0";
                }
            }

            m_segmentCount = this.SegmentCount;
            if (this.Value.Equals(string.Empty))
                this.m_GaugeValue = " ";
            
            if (this.IsAutomaticSegmentCountEnabled)
                this.m_segmentCount = this.Value.Length;

            if (m_segmentCount <= 0)
                this.m_segmentCount = 1;
            
            double panelwidth = 0;
            string valueArray = this.Value;
            int diff = 0;

            char[] newValues = new char[m_segmentCount];
            StackPanel segmentStackPanel = new StackPanel();

            RollingCharacter character = new RollingCharacter();

            if (valueArray.Length < m_segmentCount)
            {
                diff = m_segmentCount - valueArray.Length;
            }
            int characterposition = 0;
            int newcharposition = 0;

            for (int segmentindex = 0; segmentindex < this.m_segmentCount; segmentindex++)
            {
                segmentStackPanel = new StackPanel();
                character = new RollingCharacter();
                if (segmentindex < (m_segmentCount - this.Value.Length))
                {
                    character.Value = this.IsNumeric ? "0" : " ";
                    newValues[newcharposition] = this.IsNumeric ? '0' : ' '; 
                }
                else
                {
                    newValues[newcharposition] = valueArray[characterposition];
                    character.Value = valueArray[characterposition++].ToString();
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
                            if (character1.Value != "\0")
                                character.Value = character1.Value;
                            newValues[index] = character.Value[0];
                        }
                    }
                }
                character.Width = SegmentWidth;
                character.Height = SegmentWidth;

                #region Corner Radious
                if (segmentindex == 0)
                {
                    if (this.Unit == string.Empty || this.UnitPosition == UnitPosition.End)
                    {
                        double topleft = this.CornerRadius.TopLeft > character.CornerRadius.TopLeft ? this.CornerRadius.TopLeft : character.CornerRadius.TopLeft;
                        double bottomleft = this.CornerRadius.BottomLeft > character.CornerRadius.BottomLeft ? this.CornerRadius.BottomLeft : character.CornerRadius.BottomLeft;
                        double topright = character.CornerRadius.TopRight;
                        double bottomright = character.CornerRadius.BottomRight;
                        //if (this.Unit == string.Empty && this.m_segmentCount == 1)
                        //{
                        //    topright = this.CornerRadius.TopRight > character.CornerRadius.TopRight ? this.CornerRadius.TopRight : character.CornerRadius.TopRight;
                        //    bottomright = this.CornerRadius.BottomRight > character.CornerRadius.BottomRight ? this.CornerRadius.BottomRight : character.CornerRadius.BottomRight;
                        //}
                        character.CornerRadius = new CornerRadius(topleft, topright, bottomright, bottomleft);
                    }
                }
                if (segmentindex == this.m_segmentCount - 1)
                {
                    if (this.Unit == string.Empty || this.UnitPosition == UnitPosition.Start)
                    {
                        double topleft = character.CornerRadius.TopLeft;
                        double bottomleft = character.CornerRadius.BottomLeft;
                        double topright = this.CornerRadius.TopRight > character.CornerRadius.TopRight ? this.CornerRadius.TopRight : character.CornerRadius.TopRight;
                        double bottomright = this.CornerRadius.BottomRight > character.CornerRadius.BottomRight ? this.CornerRadius.BottomRight : character.CornerRadius.BottomRight;
                        //if (this.Unit == string.Empty )
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
                segmentStackPanel.Children.Add(character);
                this.frame.Children.Add(segmentStackPanel);
                panelwidth += character.Width;
                newcharposition++;
            }

            if (!this.Unit.Equals(string.Empty))
            {
                segmentStackPanel = new StackPanel();
                segmentStackPanel.Orientation = Orientation.Horizontal;

                for (int unitchar = 0; unitchar < this.Unit.Length; unitchar++)
                {
                    RollingCharacter unitcharacter = new RollingCharacter();
                    SetProperties(unitcharacter);
                    unitcharacter.Width = SegmentWidth;
                    unitcharacter.Height = SegmentWidth;
                    unitcharacter.Value = this.Unit[unitchar].ToString();

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

                    segmentStackPanel.Children.Add(unitcharacter);
                    panelwidth += character.Width;
                }
                if (this.UnitPosition == UnitPosition.Start)
                    this.frame.Children.Insert(0, segmentStackPanel);
                else
                    this.frame.Children.Add(segmentStackPanel);
            }
            this.m_GaugeValue = new string(newValues);
            this.frame.Height = character.Height + this.SpaceBetWeenSegment.Top + this.SpaceBetWeenSegment.Bottom;
            this.frame.Width = panelwidth + (((this.m_segmentCount + this.Unit.Length) * this.SpaceBetWeenSegment.Left) +
                ((this.m_segmentCount + this.Unit.Length) * this.SpaceBetWeenSegment.Right));
            this.frame.VerticalAlignment = VerticalAlignment.Center;
            this.frame.HorizontalAlignment = HorizontalAlignment.Center;
            this.Width = this.frame.Width + this.BorderThickness.Left + this.BorderThickness.Right;
            this.Height = this.frame.Height + this.BorderThickness.Top + this.BorderThickness.Bottom;
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
                    double temp = 0;
                    double.TryParse(this.Value, out temp);
                    if (this.MinValue > this.MaxValue)
                        this.MinValue = this.MaxValue;

                    if (temp < this.MinValue || temp > this.MaxValue)
                    {
                        if (temp < this.MinValue)
                            this.m_GaugeValue = this.MinValue.ToString();
                        if (temp > this.MaxValue)
                            this.m_GaugeValue = this.MaxValue.ToString();
                        this.RefreshRollingGauge();
                        return;
                    }
                }

                if (this.m_value.Length != this.Value.Length && this.IsAutomaticSegmentCountEnabled)
                {
                    this.RefreshRollingGauge();
                    return;
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
        /// <param name="newValue">Current Value</param>
        /// <param name="index">Index of the Segment in Stackpanel</param>
        /// <param name="oldValue">Old Value</param>
        private void RollClockwise(char newValue, int index, char oldValue)
        {

            if (!this.Unit.Equals(string.Empty))
            {
                if (this.UnitPosition == UnitPosition.Start)
                {
                    index = index + 1;
                }
            }

            StackPanel segmentStackPanel = (StackPanel)this.frame.Children[index];
            RollingCharacter content;

      
                if (segmentStackPanel.Children.Count == 1)
                {
                    content = segmentStackPanel.Children[0] as RollingCharacter;
                }
                else
                {
                    content = segmentStackPanel.Children[1] as RollingCharacter;
                }
                RollingCharacter character = new RollingCharacter();
                character = GetRollingCharacter(content);
                content.Value = oldValue.ToString();
                if (content.Value != newValue.ToString() || this.NumericRotate == true)
                {
                    character.Value = newValue.ToString();

                    content.RenderTransform = new TranslateTransform();
                    character.RenderTransform = new TranslateTransform();

                    TranslateTransform preTranslate = content.RenderTransform as TranslateTransform;
                    TranslateTransform curTranslate = character.RenderTransform as TranslateTransform;

                    Storyboard animator = new Storyboard();
                    DoubleAnimation prevAnim = new DoubleAnimation()
                    {
                        Duration = new Duration(AnimationDelay)
                    };

                    DoubleAnimation nextAnim = new DoubleAnimation()
                    {
                        Duration = new Duration(AnimationDelay)
                    };

                    animator.Children.Add(prevAnim);
                    animator.Children.Add(nextAnim);

                    Storyboard.SetTarget(prevAnim, preTranslate);
                    Storyboard.SetTargetProperty(prevAnim, new PropertyPath("(TranslateTransform.Y)"));

                    Storyboard.SetTarget(nextAnim, curTranslate);
                    Storyboard.SetTargetProperty(nextAnim, new PropertyPath("(TranslateTransform.Y)"));

                    prevAnim.From = (content.Height + content.Margin.Top) / 2;
                    prevAnim.To = content.Height + content.Margin.Top + content.Height / 2 + content.Margin.Top / 2;

                    nextAnim.From = -character.Height - character.Margin.Top - character.Height / 2 - character.Margin.Top / 2;
                    nextAnim.To = - (character.Height + character.Margin.Top + character.Margin.Bottom) / 2;

                    //prevAnim.From = 0;
                    //prevAnim.To = element.Height;
                    //nextAnim.From = -2 * element.Height;
                    //nextAnim.To = -element.Height;
                
                    character.GaugeParent = this;
                    segmentStackPanel.Children.Add(character);
                    animator.Begin();

                    if (segmentStackPanel.Children.Count == 3)
                    {
                        segmentStackPanel.Children.RemoveAt(0);
                    }
                
            }
        }

        /// <summary>
        /// Roll the Segment in AntiClockWise Direction
        /// </summary>
        /// <param name="newValue">CurrentValue</param>
        /// <param name="index">Index of the segment in the stackpanel</param>
        /// <param name="oldValue"></param>
        private void RollAntiClockwise(char newValue, int index, char oldValue)
        {
            if (!this.Unit.Equals(string.Empty))
            {
                if (this.UnitPosition == UnitPosition.Start)
                {
                    index = index + 1;
                }
            }

            StackPanel segmentStackPanel = (StackPanel)this.frame.Children[index];
            RollingCharacter content;


            if (segmentStackPanel.Children.Count == 1)
            {
                content = segmentStackPanel.Children[0] as RollingCharacter;
            }
            else
            {
                content = segmentStackPanel.Children[1] as RollingCharacter;
            }
            RollingCharacter character = new RollingCharacter();
            character = GetRollingCharacter(content);
            content.Value = oldValue.ToString();
            if (content.Value != newValue.ToString() || this.NumericRotate == true)
            {
                character.Value = newValue.ToString();

                content.RenderTransform = new TranslateTransform();
                character.RenderTransform = new TranslateTransform();

                TranslateTransform preTranslate = content.RenderTransform as TranslateTransform;
                TranslateTransform curTranslate = character.RenderTransform as TranslateTransform;

                Storyboard animator = new Storyboard();
                DoubleAnimation prevAnim = new DoubleAnimation()
                {
                    Duration = new Duration(AnimationDelay)
                };

                DoubleAnimation nextAnim = new DoubleAnimation()
                {
                    Duration = new Duration(AnimationDelay)
                };

                animator.Children.Add(prevAnim);
                animator.Children.Add(nextAnim);

                Storyboard.SetTarget(prevAnim, preTranslate);
                Storyboard.SetTargetProperty(prevAnim, new PropertyPath("(TranslateTransform.Y)"));

                Storyboard.SetTarget(nextAnim, curTranslate);
                Storyboard.SetTargetProperty(nextAnim, new PropertyPath("(TranslateTransform.Y)"));

                prevAnim.From = content.Height / 2 + content.Margin.Top;
                prevAnim.To = -content.Height / 2 - content.Margin.Top;

                nextAnim.From = character.Height / 2;
                nextAnim.To = -(character.Height + character.Margin.Top + character.Margin.Bottom) / 2;

                //prevAnim.From = -content.Height / 2;
                //prevAnim.To = content.Height/2;
                //nextAnim.From = -2 * content.Height;
                //nextAnim.To = -content.Height/2;

                character.GaugeParent = this;
                segmentStackPanel.Children.Add(character);
                animator.Begin();

                if (segmentStackPanel.Children.Count == 3)
                {
                    segmentStackPanel.Children.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Create the new Rolling Character
        /// </summary>
        ///<param name="character1">Specifies the Current properties</param>
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
            character.Width = character1.Width;
            return character;
        }

        /// <summary>
        /// Refresh the gauge when PropertyChanges
        /// </summary>
        private void RefreshRollingGauge()
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
            character.HorizontalAlignment = this.SegmentHorizontalAlignment;
            character.VerticalAlignment = this.SegmentVerticalAlignment;

            //character.FontSize = this.FontSize;
            //character.GaugeParent = this;
            //character.HorizontalContentAlignment = HorizontalAlignment.Center;
            //character.VerticalContentAlignment = VerticalAlignment.Center;

        }

        /// <summary>
        /// Called when [segment foreground changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSegmentForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
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
        /// Called when [value changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnValueChanged(e);
            instance.m_GaugeValue = e.NewValue.ToString();
            instance.m_value = e.OldValue.ToString();
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
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge instance = (RollingGauge)d;
            instance.OnUnitPositionChanged(e);
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
        /// Called when [max value changed].
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
                if (obj.MinValue > (double)e.NewValue)
                    obj.MinValue = (double)e.NewValue;
                if (value > (double)e.NewValue)
                    obj.m_GaugeValue = e.NewValue.ToString();
            }
        }


        /// <summary>
        /// Called when [min value changed].
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
                if (obj.MaxValue < (double)e.NewValue)
                    obj.MaxValue = (double)e.NewValue;
                if (value < (double)e.NewValue)
                    obj.m_GaugeValue = e.NewValue.ToString();
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
            if (instance != null)
                instance.RefreshRollingGauge();
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
                obj.RefreshRollingGauge();
        }

        private static void OnAppearenceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RollingGauge obj = (RollingGauge)sender;
            if (obj != null)
                obj.RefreshRollingGauge();
        }

        public static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RollingGauge instance = d as RollingGauge;
            instance.UpdateVisualStyle();
            instance.InvalidateArrange();
        }

        protected virtual void UpdateVisualStyle()
        {
            ResourceDictionary _resources = this.GetResources(this.VisualStyle);

            string _key;
            _key = this.GetResourceKey("RollingGauge", this.VisualStyle);
             this.Style = _resources[_key] as Style;
        }

        private ResourceDictionary GetResources(GaugeVisualStyle style)
        {
            ResourceDictionary res = new ResourceDictionary();
            switch (style)
            {
                case GaugeVisualStyle.Blend:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Metro:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2003:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Black:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Black.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Blue:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Blue.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Silver:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Silver.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.VS2010:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Default:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/generic.xaml", UriKind.RelativeOrAbsolute) };
                    break;
            }
            return res;
        }

        private string GetResourceKey(string element, GaugeVisualStyle currentStyle)
        {
            return currentStyle + element + "Style";
        }
        #endregion Implementation
    }
}


