#region Copyright
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;


namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the DigitalGauge UI element. &lt;remarks&gt;The digital gauge is
    /// perfect for displaying output values that uses various character symbols. A
    /// digital gauge can have multiple characters, state indicators, labels and images.
    /// The look and feel of characters can be fully customized by setting the different
    /// properties of DigitalGauge.
    /// </summary>
    /// <remarks>
    /// The digital gauge is perfect for displaying output values that uses various
    /// character symbols. A digital gauge can have multiple characters, state
    /// indicators, labels and images. The look and feel of characters can be fully
    /// customized by setting the different properties of DigitalGauge.
    /// </remarks>
    /// <example>
    /// The following example shows how to create a  in XAML. 
    /// <para>&lt;UserControl x:Class=&quot;DigitalGaugeDemo.MainPage&quot;</para>
    /// <para>
    /// xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
    /// xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;</para>
    /// <para>
    /// xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;
    ///    xmlns:d=&quot;http://schemas.microsoft.com/expression/blend/2008&quot;
    /// xmlns:mc=&quot;http://schemas.openxmlformats.org/markup-compatibility/2006&quot;
    /// </para>
    /// <para>xmlns:local=&quot;clr-namespace:Syncfusion.Silverlight.SampleBrowser.Controls;assembly=Syncfusion.SampleBrowserUtils.Silverlight&quot;</para>
    /// <para>     &gt;</para>
    /// <para><c> &lt;Grid x:Name=&quot;LayoutRoot&quot;&gt;</c></para>
    /// <para></para>
    /// <code>                            &lt;syncfusion:DigitalGauge  Name=&quot;digitalGauge&quot;
    ///                                    CharacterHeight=&quot;50&quot;  Grid.RowSpan=&quot;2&quot;
    ///                                    SegmentBrush=&quot;Brown&quot;   SegmentWidth=&quot;3&quot;
    ///                                    DimmedBrush=&quot;Transparent&quot;  SegmentSpacing=&quot;1&quot; InnerFrameBrush=&quot;CadetBlue&quot; OuterFrameBrush=&quot;Black&quot; Background=&quot;AliceBlue&quot;
    ///                                    CharacterSpacing=&quot;5&quot;  CharacterCount=&quot;10&quot;
    ///                                    Value=&quot;Syncfusion&quot; CharacterType=&quot;SegmentFourteen&quot;&gt;
    ///                             &lt;/syncfusion:DigitalGauge&gt;
    ///                              &lt;/Grid&gt;&lt;/UserControl&gt;</code>
    /// <para></para>
    /// <para>The following example shows how to create a  in C#. </para>
    /// <para></para>
    /// <code> using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Net;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Animation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Gauge;
    /// using Syncfusion.Silverlight.SampleBrowser; namespace GaugeControlTesting
    ///  {
    ///    public partial class MainPage : UserControl
    ///     {
    ///         public MainPage()
    ///         {           InitializeComponent();
    ///            this.digitalGauge1.CharacterCount = 5;
    ///            this.digitalGauge1.CharacterHeight = 50;
    ///            this.digitalGauge1.CenterFrameFillColor = Colors.Gray;
    ///            this.digitalGauge1.Foreground = new SolidColorBrush( Colors.Red );
    ///            this.digitalGauge1.DimmedBrush = new SolidColorBrush( Colors.Transparent );
    ///            this.digitalGauge1.SkewAngleX = -10;
    ///            this.digitalGauge1.SegmentSpacing = 1;
    ///            this.digitalGauge1.CharacterSpacing = 5;
    ///            this.digitalGauge1.Value = &quot;Error&quot;;
    ///            this.digitalGauge1.CharacterType = CharacterType.SegmentFourteen;
    ///        }
    ///    }
    ///  }</code>
    /// <para></para>
    /// <para>        </para>
    /// </example>
    /// <seealso cref="CircularGauge">CircularGauge</seealso>
    /// <seealso cref="LinearGauge">LinearGauge</seealso>
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Blend;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Default;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Office2003;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    //    Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.VS2010;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    //  Type = typeof(DigitalGauge), XamlResource = "/Syncfusion.Theming.Metro;component/Gauge.xaml")]
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

        /// <summary>
        /// constant default height
        /// </summary>
        private const double CdefaultHeight = 100;

        /// <summary>
        /// constant default width
        /// </summary>
        private const double CdefaultWidth = 300;

        private ContentPresenter PART_DigitalContent;

        private Path mainglasspath;
        private Path PART_SecondGlassPath;

        /// <summary>
        /// Scales panel.
        /// </summary>
        private LinearScaleLayoutPanel mscalesPanel;

        private Grid contianerBorder;

        private static bool autosize = false;

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

        internal Size GetDefaultSize()
        {
            return new Size(CdefaultWidth, CdefaultHeight);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalGauge"/> class.
        /// </summary>
        public DigitalGauge()
        {
            DefaultStyleKey = typeof(DigitalGauge);
            this.Characters = new CharacterCollection();
            this.Characters.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.InitializeDictionaryList();
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static DigitalGauge()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
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
            DependencyProperty.Register("CharacterCount", typeof(int), typeof(DigitalGauge), new PropertyMetadata(2, new PropertyChangedCallback(OnCharacterCountChanged)));

        /// <summary>
        /// Identifies the <see cref="CharacterHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterHeightProperty =
            DependencyProperty.Register("CharacterHeight", typeof(double), typeof(DigitalGauge), new PropertyMetadata(30d, new PropertyChangedCallback(OnCharacterHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
#if Silverlight5  
        public static readonly new DependencyProperty CharacterSpacingProperty =
#else
        public static readonly DependencyProperty CharacterSpacingProperty =
#endif
            DependencyProperty.Register("CharacterSpacing", typeof(double), typeof(DigitalGauge), new PropertyMetadata(2d, new PropertyChangedCallback(OnCharacterSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="CharacterType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterTypeProperty =
            DependencyProperty.Register("CharacterType", typeof(CharacterType), typeof(DigitalGauge), new PropertyMetadata(CharacterType.SegmentSeven, new PropertyChangedCallback(OnCharacterTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBrushProperty =
            DependencyProperty.Register("SegmentBrush", typeof(Brush), typeof(DigitalGauge), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnSegmentBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="DimmedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DimmedBrushProperty =
            DependencyProperty.Register("DimmedBrush", typeof(Brush), typeof(DigitalGauge), new PropertyMetadata(null, new PropertyChangedCallback(OnDimmedBrushChanged)));

         /// <summary>
        /// Identifies the <see cref="SegmentSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(DigitalGauge), new PropertyMetadata(0.5d, new PropertyChangedCallback(OnSegmentSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentWidthProperty =
            DependencyProperty.Register("SegmentWidth", typeof(double), typeof(DigitalGauge), new PropertyMetadata(2d, new PropertyChangedCallback(OnSegmentWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="SkewAngleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewAngleXProperty =
            DependencyProperty.Register("SkewAngleX", typeof(double), typeof(DigitalGauge), new PropertyMetadata(0d,  new PropertyChangedCallback(OnSkewAngleXChanged)));

        /// <summary>
        /// Identifies the <see cref="SkewAngleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewAngleYProperty =
            DependencyProperty.Register("SkewAngleY", typeof(double), typeof(DigitalGauge), new PropertyMetadata(0d,  new PropertyChangedCallback(OnSkewAngleYChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(DigitalGauge), new PropertyMetadata(string.Empty,  new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="InnerFrameContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerFrameContentProperty =
           DependencyProperty.Register("InnerFrameContent", typeof(object), typeof(DigitalGauge), new PropertyMetadata(null,  new PropertyChangedCallback(OnInnerFrameContentChanged)));

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
#if Silverlight5        
        public new double CharacterSpacing
#else
        public double CharacterSpacing
#endif
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
        /// Gets or sets the foreground brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush SegmentBrush
        {
            get
            {
                return (Brush)GetValue(SegmentBrushProperty);
            }

            set
            {
                SetValue(SegmentBrushProperty, value);
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
        public string Value
        {
            get
            {
                return (string)this.GetValue(ValueProperty);
            }

            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the the inner frame content to host any content inside the gauge. 
        /// This content will be added as inner frame content.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is null.
        /// </value>
        public object InnerFrameContent
        {
            get
            {
                return (object)GetValue(InnerFrameContentProperty);
            }

            set
            {
                SetValue(InnerFrameContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ScalePanel
        /// </summary>
        /// <value>
        /// Type: <see cref="LinearScaleLayoutPanel"/>
        /// </value>
        /// <seealso cref="LinearScaleLayoutPanel"/>
        internal LinearScaleLayoutPanel ScalePanel
        {
            get
            {
                return this.mscalesPanel;
            }

            set
            {
                this.mscalesPanel = value;
            }
        }
        #endregion DP Getters & Setters

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.contianerBorder = this.GetTemplateChild("PART_ContainerBorder") as Grid;
            if (this.FirstCircleBorder != null)
            {
                this.FirstCircleBorder.GaugeElementParent = this;
            }

            if (this.SecondCircleBorder != null)
            {
                this.SecondCircleBorder.GaugeElementParent = this;
                this.SecondCircleBorder.Offset = this.OuterFrameOffset;
            }

            if (this.InnerCircleBorder != null)
            {
                this.InnerCircleBorder.GaugeElementParent = this;
                this.InnerCircleBorder.Offset = this.SecondCircleBorder.Offset + this.InnerFrameOffset;
            }            

            this.mainglasspath = this.GetTemplateChild("PART_GlassPath") as Path;
            this.PART_SecondGlassPath = this.GetTemplateChild("PART_SecondGlassPath") as Path;
            PART_DigitalContent = this.GetTemplateChild("PART_DigitalContent") as ContentPresenter;
            this.ScalePanel = this.GetTemplateChild("PART_ScalesPanel") as LinearScaleLayoutPanel;
            if (this.ScalePanel != null)
            {
                this.ChildrenCollection.VisualParent = this.ScalePanel;
                this.ChildrenCollection.UpdatePanelChildren();
            }

            if (double.IsNaN(this.Height) && double.IsNaN(this.Width) && !autosize)
            {
                autosize = true;
            }

            Path firstGlassPath = this.GetTemplateChild("PART_GlassPath") as Path;
            Path secondGlassPath = this.GetTemplateChild("PART_SecondGlassPath") as Path;

            if (firstGlassPath != null && secondGlassPath != null)
            {
                    Binding firstGlassPathBinding = new Binding();
                    firstGlassPathBinding.Converter = new BooleanToVisibilityConverter();
                    firstGlassPathBinding.Source = this;
                    firstGlassPathBinding.Mode = BindingMode.OneWay;
                    firstGlassPathBinding.Path = new PropertyPath("EnableEffects");
                    firstGlassPath.SetBinding(VisibilityProperty, firstGlassPathBinding);

                    Binding secondGlassPathBinding = new Binding();
                    secondGlassPathBinding.Converter = new BooleanToVisibilityConverter();
                    secondGlassPathBinding.Source = this;
                    secondGlassPathBinding.Mode = BindingMode.OneWay;
                    secondGlassPathBinding.Path = new PropertyPath("EnableEffects");
                    secondGlassPath.SetBinding(VisibilityProperty, secondGlassPathBinding);
            }

            this.RefreshBorders();
            this.contianerBorder.SizeChanged += new SizeChangedEventHandler(LinearGauge_SizeChanged);     
            this.UpdateBorder();
            this.RefreshCharacterSegments();
        }

        void LinearGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshBorders();

        }
        #endregion Overrides

        #region Implementation

        internal void UpdateBorder()
        {
            if (mainglasspath != null)
            {
                if (this.Height != 0)
                {
                    double width = this.GetGaugeSize().Width;
                    double height = this.GetGaugeSize().Height;

                    this.mainglasspath.Height =  2*(height- InnerFrameOffset - OuterFrameOffset)/3;
                    this.mainglasspath.Width = width - this.InnerFrameOffset - this.OuterFrameOffset;
                    this.PART_SecondGlassPath.Width = width - this.InnerFrameOffset-this.OuterFrameOffset;
                    this.PART_SecondGlassPath.Height = (height) / 4;
                    TranslateTransform transform = new TranslateTransform();
                    transform.X = 0;
                    transform.Y = -(height-InnerFrameOffset-OuterFrameOffset)/2+height/8;
                    
                    this.PART_SecondGlassPath.RenderTransform = transform;
                    TranslateTransform transform1 = new TranslateTransform();
                    
                    transform1.X=0;
                    transform1.Y = -height / 2 + 2 * (height - InnerFrameOffset - OuterFrameOffset) / 3+(InnerFrameOffset+OuterFrameOffset)/2;
                    this.mainglasspath.RenderTransform = transform1;                   
                  
                }
            }
        }

        internal Size GetGaugeSize()
        {
            double width = CdefaultWidth;
            double height = CdefaultHeight;
            if (!double.IsNaN(this.Height) && !double.IsNaN(this.Width))
            {
                width = this.Width;
                height = this.Height;
            }
            else if (this.ActualHeight != 0 && this.ActualWidth != 0)
            {
                width = this.ActualWidth;
                height = this.ActualHeight;
            }
            double tempheight = height - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) - 1;
            double tempwidth = width - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) - 1;

            if( tempheight <= 0 )
            {
                height =((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1);
                this.Height = height;
            }
            if (tempwidth <= 0)
            {
                width = ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1);
                this.Width = width;
            }
            //height = tempheight <= 0 ? ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1) : height;
            //width = tempwidth <= 0 ? ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1) : width;
            return new Size(width, height);
        }

        /// <summary>
        /// Updates the location of children elements.
        /// </summary>
        protected override void UpdateChildrenLocation()
        {
            int count = this.ChildrenCollection.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.ChildrenCollection[i] is LocalizableGaugeElement)
                {
                    LocalizableGaugeElement elem = this.ChildrenCollection[i] as LocalizableGaugeElement;

                    double width = this.GetGaugeSize().Width;
                    double height = this.GetGaugeSize().Height;

                    Point location = this.ConvertLocation(elem.Location, width, height);
                    TranslateTransform transform = new TranslateTransform();
                    transform.X = location.X;
                    transform.Y = location.Y;
                    elem.RenderTransform = transform;

                }
            }
        }

        /// <summary>
        /// Adds characters to digital gauge if needed.
        /// </summary>
        internal void AddCharacters()
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
        }

        /// <summary>
        /// Occurs when control is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void DigitalGaugeLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshCharacterSegments();            
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
            m_characterDictionary.Add('r', new List<bool>() { true, true, true, false, false, true, true, true, false, false, false, false, false, true });
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
                this.RefreshCharacterSegments();
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
            if (this.PART_DigitalContent != null)
            {
                Grid grid = this.PART_DigitalContent.Content as Grid;
                foreach (Grid g in grid.Children)
                {
                    g.Width = (this.CharacterHeight / 3) + this.CharacterSpacing;
                }
            }         
               
            if (CharacterSpacingChanged != null)
            {
                CharacterSpacingChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises CharacterTypeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnCharacterTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.AddCharacters();
            RefreshCharacterSegments();
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
                this.RefreshCharacterSegments();
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
        private static void OnSegmentBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnSegmentBrushChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ForegroundChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSegmentBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CharacterBase ch in this.Characters)
            {
                ch.SegmentBrush = this.SegmentBrush;
                this.RefreshCharacterSegments();
            }

            //if (SegmentBrushChanged != null)
            //{
            //    this.SegmentBrushChanged(this, e);
            //}
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
                this.RefreshCharacterSegments();
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
                this.RefreshCharacterSegments();
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
            if (this.PART_DigitalContent != null)
            {
                SkewTransform transform = new SkewTransform();
                transform.AngleX = this.SkewAngleX;
                transform.AngleY = this.SkewAngleY;
                Grid grid = this.PART_DigitalContent.Content as Grid;
                foreach (Grid g in grid.Children)
                {
                    g.RenderTransform = transform;
                }
            }
            
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
            if (this.PART_DigitalContent != null)
            {
                SkewTransform transform = new SkewTransform();
                transform.AngleX = this.SkewAngleX;
                transform.AngleY = this.SkewAngleY;
                Grid grid = this.PART_DigitalContent.Content as Grid;
                foreach (Grid g in grid.Children)
                {
                    g.RenderTransform = transform;
                }
            }

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
        /// Calls OnInnerFrameContentChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerFrameContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGauge instance = (DigitalGauge)d;
            instance.OnInnerFrameContentChanged(e);
        }

        /// <summary>
        /// Updates property value cache.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnInnerFrameContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
               ////this.RemoveVisualChild(e.OldValue as Visual);
            }
            else
            {
                ////this.AddLogicalChild(e.NewValue as Visual);
            }
        }

        /// <summary>
        /// Refreshes character properties.
        /// </summary>
        internal void RefreshCharacterProperties(CharacterBase ch)
        {           
                ch.SegmentsSpacing = this.SegmentSpacing;
                ch.SegmentWidth = this.SegmentWidth;
                ch.SegmentBrush = this.SegmentBrush;
                ch.DimmedBrush = this.DimmedBrush;
                ch.CharacterHeight = this.CharacterHeight;
                ch.CharactersSpacing = this.CharacterSpacing;
                SkewTransform transform=new SkewTransform();
                transform.AngleX=this.SkewAngleX;
                transform.AngleY=this.SkewAngleY;
                ch.RenderTransform = transform;                
        }

        /// <summary>
        /// Characters are set according to the <see cref="Value"/> property.
        /// </summary>
        private void RefreshCharacterSegments()
        {
            int ct=0, spchar=0;
            string str = this.Value.ToString().ToLower();
            foreach (char c in str)
            {
                if (c == ':' || c == '.')
                {
                    spchar++;
                }
            }

            if (str.Length-spchar >= this.Characters.Count)
            {
                spchar = 0;
                while (ct+(spchar-1) <= this.Characters.Count)
                {
                    if (ct < this.Value.ToString().Length-1)
                    {
                        if (this.Value.ToString()[ct + 1] == ':' || this.Value.ToString()[ct + 1] == '.')
                        {
                            spchar++;
                        }
                    }

                    ct++;
                    str = this.Value.ToString().Substring(0, this.Characters.Count + spchar);
                }
            }     

            str=str.ToLower();
       
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
                        this.Characters[str.Length - i - 1].InvalidateArrange();
                    }
                }
            }

            if (this.PART_DigitalContent != null)
            {
                SkewTransform transform = new SkewTransform();
                transform.AngleX = this.SkewAngleX;
                transform.AngleY = this.SkewAngleY;
                int character = this.Characters.Count-1;
                if (this.CharacterType == CharacterType.SegmentSeven)
                {
                    CharacterSeven ch = new CharacterSeven();
                    this.RefreshCharacterProperties(ch); 
                    Grid grid = new Grid();                    
                    for (int i = 0; i < this.Characters.Count; i++)
                    {
                        ColumnDefinition cs = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(cs);
                        ch.DrawDot = this.Characters[character].DrawDot;
                        ch.DrawColon = this.Characters[character].DrawColon;
                        ch.Segments = this.Characters[character--].Segments;
                        ColumnDefinition cd = new ColumnDefinition();
                        Grid gr = ch.OnRender();
                        gr.Width = (this.CharacterHeight / 3) + this.CharacterSpacing;
                        gr.Margin = new Thickness(this.CharacterHeight / 4);
                        grid.ColumnDefinitions.Add(cd);
                        gr.SetValue(Grid.ColumnProperty, i);
                        gr.RenderTransform = transform;
                        grid.Children.Add(gr);
                    }

                    character = 0;
                    this.PART_DigitalContent.Content = grid;
                }
                else if (this.CharacterType == CharacterType.SegmentFourteen)
                {
                    CharacterFourteen ch = new CharacterFourteen();
                    this.RefreshCharacterProperties(ch);
                    Grid grid = new Grid();
                    for (int i = 0; i < this.Characters.Count; i++)
                    {
                        ColumnDefinition cs = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(cs);
                        ch.DrawDot = this.Characters[character].DrawDot;
                        ch.DrawColon = this.Characters[character].DrawColon;
                        ch.Segments = this.Characters[character--].Segments;
                        ColumnDefinition cd = new ColumnDefinition();
                        Grid gr = ch.OnRender();
                        gr.Width = (this.CharacterHeight / 3) + this.CharacterSpacing;
                        gr.Margin = new Thickness(this.CharacterHeight/4);
                        grid.ColumnDefinitions.Add(cd);
                        gr.RenderTransform = transform;
                        gr.SetValue(Grid.ColumnProperty, i);
                        grid.Children.Add(gr);
                    }

                    character = 0;
                    this.PART_DigitalContent.Content = grid;
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
                this.Characters[i].InvalidateArrange();
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
