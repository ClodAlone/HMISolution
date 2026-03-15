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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.ComponentModel;
    using System.Windows.Shapes;
    using System.Windows.Data;
using Syncfusion.Windows.Controls.Theming;

    /// <summary>
    /// Represents the LinearGauge UI element. 
    /// <para>The linear gauges a perfect for showing their input values graphically along a linear scale.</para>
    /// <para>A linear gauge can have multiple scales, pointers, ranges, state indicators, labels and images. It has a bounded rectangle around it. The top-left corner of the rectangle has coordinates of (0, 0) and the bottom-right corner has coordinates of (100, 100) and every gauge element is drawn inside the bounded rectangle according to the location property. </para>
    /// </summary>
    /// <example>
    /// The following example shows how to create a LinearGauge in XAML. 
    /// <para> </para>
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge;
    /// <para></para>
    /// <para>     namespace GaugeControl </para>
    /// <para></para>
    /// <para>     { </para>
    /// <para></para>
    /// <para>       public partial class Page : UserControl </para>
    /// <para></para>
    /// <para>       { </para>
    /// <para>           public Page() </para>
    /// <para>             { </para>
    /// <para></para>
    /// <para>               InitializeComponent(); </para>
    /// <para></para>
    /// <para>                 linearGauge1.Width = 150; </para>
    /// <para></para>
    /// <para>                 linearGauge1.Height = 380; </para>
    /// <para></para>
    /// <para>                 LinearScale scale = new LinearScale(); </para>
    /// <para></para>
    /// <para>                 scale.Minimum = 0; </para>
    /// <para></para>
    /// <para>                 scale.Maximum = 100; </para>
    /// <para></para>
    /// <para>                 scale.MinorIntervalValue = 2; </para>
    /// <para></para>
    /// <para>                 scale.MajorIntervalValue = 10; </para>
    /// <para></para>
    /// <para>                 scale.scaleBarSize = 10; </para>
    /// <para></para>
    /// <para>                 scale.scaleBarLength = 260; </para>
    /// <para></para>
    /// <para>                 scale.Background = new SolidColorBrush( Colors.Orange ); </para>
    /// <para></para>
    /// <para>                 linearGauge1.Scales.Add( scale ); </para>
    /// <para></para>
    /// <para>                 LabelTickSet labelTick = new LabelTickSet(); </para>
    /// <para></para>
    /// <para>                 labelTick.FontSize = 10; </para>
    /// <para></para>
    /// <para>                 labelTick.TickStyle = TickStyle.MajorTick; </para>
    /// <para></para>
    /// <para>                 labelTick.Background = new SolidColorBrush( Colors.Red ); </para>
    /// <para></para>
    /// <para>                 labelTick.DistanceFromScale = 5; </para>
    /// <para></para>
    /// <para>                 scale.Ticks.Add( labelTick ); </para>
    /// <para></para>
    /// <para>                 MarkTickSet markTick1 = new MarkTickSet(); </para>
    /// <para></para>
    /// <para>                 markTick1.TickHeight = 9; </para>
    /// <para></para>
    /// <para>                 markTick1.TickShape = TickShape.Rectangle; </para>
    /// <para></para>
    /// <para>                 markTick1.TickStyle = TickStyle.MajorTick; </para>
    /// <para></para>
    /// <para>                 markTick1.TickWidth = 4; </para>
    /// <para></para>
    /// <para>                 markTick1.Background = new SolidColorBrush( Colors.Purple ); </para>
    /// <para></para>
    /// <para>                 scale.Ticks.Add( markTick1 ); </para>
    /// <para></para>
    /// <para>                 MarkTickSet markTick2 = new MarkTickSet(); </para>
    /// <para></para>
    /// <para>                 markTick2.TickHeight = 4; </para>
    /// <para></para>
    /// <para>                 markTick2.TickWidth = 1; </para>
    /// <para></para>
    /// <para>                 markTick2.TickStyle = TickStyle.MinorTick; </para>
    /// <para></para>
    /// <para>                 markTick2.Background = new SolidColorBrush( Colors.White ); </para>
    /// <para></para>
    /// <para>                 scale.Ticks.Add( markTick2 ); </para>
    /// <para></para>
    /// <para>                 LinearBarPointer pointer = new LinearBarPointer(); </para>
    /// <para></para>
    /// <para>                 pointer.Background = new SolidColorBrush( Colors.Blue ); </para>
    /// <para></para>
    /// <para>                 pointer.PointerWidth = 6; </para>
    /// <para></para>
    /// <para>                 pointer.Value = 50; </para>
    /// <para></para>
    /// <para>                 scale.Pointers.Add( pointer ); </para>
    /// <para></para>
    /// <para>                 LinearRange range = new LinearRange(); </para>
    /// <para></para>
    /// <para>                 range.StartValue = 70; </para>
    /// <para></para>
    /// <para>                 range.EndValue = 100; </para>
    /// <para></para>
    /// <para>                 range.StartWidth = 0; </para>
    /// <para></para>
    /// <para>                 range.EndWidth = 10; </para>
    /// <para></para>
    /// <para>                 range.RangePosition = ScalePlacement.Inside; </para>
    /// <para></para>
    /// <para>                 range.DistanceFromScale = 2; </para>
    /// <para></para>
    /// <para>                 range.Background = new SolidColorBrush( Colors.Orange ); </para>
    /// <para></para>
    /// <para></para>
    /// <para>            } </para>
    /// <para></para>
    /// <para></para>
    /// <para>        } </para>
    /// <para></para>
    /// <para></para>
    /// <para>      } </para></description></item></list>       
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>
    /// <para>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;</para>
    /// <para></para>
    /// <para>  </para>
    /// <para>      &lt;syncfusion:LinearGauge Name=&quot;linearGauge1&quot;&gt; </para>
    /// <para></para>
    /// <para>        &lt;syncfusion:LinearGauge.Scales&gt; </para>
    /// <para></para>
    /// <para>            &lt;syncfusion:LinearScale Name=&quot;LinearScale&quot; </para>
    /// <para></para>
    /// <para>                          Minimum=&quot;0&quot; </para>
    /// <para></para>
    /// <para>                          Maximum=&quot;100&quot; </para>
    /// <para></para>
    /// <para>                          MinorIntervalValue=&quot;2&quot; </para>
    /// <para></para>
    /// <para>                          MajorIntervalValue=&quot;10&quot; </para>
    /// <para></para>
    /// <para>                          scaleBarSize = &quot;10&quot; </para>
    /// <para></para>
    /// <para>                          scaleBarLength=&quot;260&quot;&gt; </para>
    /// <para></para>
    /// <para>            &lt;syncfusion:LinearScale.Ticks&gt; </para>
    /// <para></para>
    /// <para>                &lt;syncfusion:LabelTickSet FontSize=&quot;10&quot; TickStyle=&quot;MajorTick&quot; Background=&quot;Red&quot; </para>
    /// <para></para>
    /// <para>                                                    TickPlacement=&quot;Inside&quot; DistanceFromScale=&quot;5&quot;/&gt; </para>
    /// <para></para>
    /// <para>                &lt;syncfusion:MarkTickSet TickHeight=&quot;9&quot; TickShape=&quot;Rectangle&quot; TickStyle=&quot;MajorTick&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                                                    TickWidth=&quot;4&quot; Background=&quot;Purple&quot;/&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                &lt;syncfusion:MarkTickSet TickHeight=&quot;4&quot; TickWidth=&quot;1&quot; TickStyle=&quot;MinorTick&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                                                   Background=&quot;Aqua&quot;/&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>            &lt;/syncfusion:LinearScale.Ticks&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>            &lt;syncfusion:LinearScale.Pointers&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                &lt;syncfusion:LinearBarPointer Background=&quot;Blue&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                                   PointerWidth=&quot;5&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                                   Value=&quot;50&quot;/&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>            &lt;/syncfusion:LinearScale.Pointers&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>            &lt;syncfusion:LinearScale.Ranges&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                &lt;syncfusion:LinearRange StartValue=&quot;70&quot; EndValue=&quot;100&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                              StartWidth=&quot;0&quot; EndWidth=&quot;10&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                              RangePosition=&quot;Inside&quot; DistanceFromScale=&quot;2&quot; </para>
    /// <para></para>
    /// <para></para>
    /// <para>                              Background=&quot;Orange&quot;/&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>            &lt;/syncfusion:LinearScale.Ranges&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>            &lt;/syncfusion:LinearScale&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>       &lt;/syncfusion:LinearGauge.Scales&gt; </para>
    /// <para></para>
    /// <para></para>
    /// <para>      &lt;/syncfusion:LinearGauge&gt; </para>
    /// <para></para></description></item></list>    
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para>  </para>
    /// </example>
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    //   Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Blend;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
    //    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    //    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    //    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    //    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Default;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
    //    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Office2003;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    //    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.VS2010;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    //   Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Theming.Metro;component/Gauge.xaml")]
    [ContentProperty("Scales")]
    public class LinearGauge : GaugeBase
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.Orientation">Orientation</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The Default value is <see cref="F:Syncfusion.Windows.Gauge.GaugeOrientation.Vertical">GaugeOrientation.Vertical</see></para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.GaugeOrientation">GaugeOrientation</see></para>
        /// </returns>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(GaugeOrientation), typeof(LinearGauge), new PropertyMetadata(GaugeOrientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.RadiusX">RadiusX</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(LinearGauge), new PropertyMetadata(new PropertyChangedCallback(OnRadiusXChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.RadiusY">RadiusY</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(LinearGauge), new PropertyMetadata(new PropertyChangedCallback(OnRadiusYChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.RotationAngle">RotationAngle</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type :<see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        internal static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(LinearGauge), new PropertyMetadata(0d, new PropertyChangedCallback(OnRotationAngleChanged)));

        #endregion

        #region Class constants

        #endregion

        #region Private members

        /// <summary>
        /// constant default height
        /// </summary>
        private const double CdefaultHeight = 300;

        /// <summary>
        /// constant default width
        /// </summary>
        private const double CdefaultWidth = 100;

        /// <summary>
        /// Collection of linear scales.
        /// </summary>
        private LinearScaleCollection mscales;

        /// <summary>
        /// Scales panel.
        /// </summary>
        private LinearScaleLayoutPanel mscalesPanel;

        private Grid contianerBorder;

        private static bool autosize = false;

        private Path firstGlassPath;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LinearGauge">LinearGauge</see> class.
        /// </summary>
        public LinearGauge()
        {
            DefaultStyleKey = typeof(LinearGauge);
            this.Loaded += new RoutedEventHandler(this.LinearGaugeLoaded);
            this.SizeChanged += new SizeChangedEventHandler(this.LinearGaugeSizeChanged);

            this.Scales = new LinearScaleCollection();
            this.Scales.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static LinearGauge()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.Orientation">Orientation</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback OrientationChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.RadiusX">RadiusX</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusXChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.RadiusY">RadiusY</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusYChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearGauge.RotationAngle">RotationAngle</see> property is changed.
        /// </summary>
        internal event PropertyChangedCallback RotationAngleChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the orientation of the linear gauge. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type:  Default value is <see cref="F:Syncfusion.Windows.Gauge.GaugeOrientation.Vertical">GaugeOrientation.Vertical</see>.
        /// </value>
        /// <seealso cref="GaugeOrientation">GaugeOrientation</seealso>
        public GaugeOrientation Orientation
        {
            get
            {
                return (GaugeOrientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the x radius
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        public double RadiusX
        {
            get
            {
                return (double)GetValue(RadiusXProperty);
            }

            set
            {
                SetValue(RadiusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the y radius
        /// </summary>
        /// <value>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </value>
        public double RadiusY
        {
            get
            {
                return (double)GetValue(RadiusYProperty);
            }

            set
            {
                SetValue(RadiusYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of linear scales.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:Syncfusion.Windows.Gauge.LinearScaleCollection">LinearScaleCollection</see>
        /// </value>
        /// <seealso cref="LinearScaleCollection">LinearScaleCollection</seealso>
        public LinearScaleCollection Scales
        {
            get
            {
                return this.mscales;
            }

            set
            {
                this.mscales = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle of displaying the gauge. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see cref="T:System.Double">System.Double</see></para>
        /// <para> </para>
        /// </value>
        /// <seealso cref="double">double</seealso>
        internal double RotationAngle
        {
            get
            {
                return (double)GetValue(RotationAngleProperty);
            }

            set
            {
                SetValue(RotationAngleProperty, value);
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

        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.contianerBorder = this.GetTemplateChild("PART_ContainerBorder") as Grid;

             if (this.VisualStyle== GaugeVisualStyle.VS2010)
            {             
                if (this.Orientation == GaugeOrientation.Horizontal)
                {
                    ResourceDictionary r = new ResourceDictionary();
                    r.Source = new Uri("/Syncfusion.Theming.VS2010;component/Gauge.xaml", UriKind.RelativeOrAbsolute);                   
                    this.Background = r["HorizontalLinearGaugeBackgroundBrush"] as Brush;                   
                }
            }


            this.ScalePanel = this.GetTemplateChild("PART_ScalesPanel") as LinearScaleLayoutPanel;
            if (this.ScalePanel != null)
            {
                this.ChildrenCollection.VisualParent = this.ScalePanel;
                this.ChildrenCollection.UpdatePanelChildren();
            }
            this.firstGlassPath = this.GetTemplateChild("PART_GlassPath") as Path;

            if (firstGlassPath != null)
            {
                Binding firstGlassPathBinding = new Binding();
                firstGlassPathBinding.Converter = new BooleanToVisibilityConverter();
                firstGlassPathBinding.Source = this;
                firstGlassPathBinding.Mode = BindingMode.OneWay;
                firstGlassPathBinding.Path = new PropertyPath("EnableEffects");
                firstGlassPath.SetBinding(VisibilityProperty, firstGlassPathBinding);
            }

            ContentPresenter presenter = this.GetTemplateChild("PART_DigitalDisplay") as ContentPresenter;

            if (presenter != null)
            {
                Binding visibilityBinding = new Binding();
                visibilityBinding.Converter = new BooleanToVisibilityConverter();
                visibilityBinding.Source = this;
                visibilityBinding.Mode = BindingMode.OneWay;
                visibilityBinding.Path = new PropertyPath("ShowDigitalValue");
                presenter.SetBinding(VisibilityProperty, visibilityBinding);
            }


            if (this.FirstCircleBorder != null)
            {
                this.FirstCircleBorder.GaugeElementParent = this;
            }
            if (this.InnerCircleBorder != null)
            {
                this.InnerCircleBorder.GaugeElementParent = this;
                this.InnerCircleBorder.Offset = this.MiddleFrameOffset + this.OuterFrameOffset + this.InnerFrameOffset;
            }
            if (this.SecondCircleBorder != null)
            {
                this.SecondCircleBorder.GaugeElementParent = this;
                this.SecondCircleBorder.Offset = this.OuterFrameOffset + this.MiddleFrameOffset;
            }
            if (this.MiddleCircleBorder != null)
            {
                this.MiddleCircleBorder.GaugeElementParent = this;
                this.MiddleCircleBorder.Offset = this.OuterFrameOffset;
            }

            if (double.IsNaN(this.Height) && double.IsNaN(this.Width) && !autosize)
            {
                autosize = true;
            }
            this.RefreshBorders();
            if(this.contianerBorder!=null)
            this.contianerBorder.SizeChanged += new SizeChangedEventHandler(LinearGauge_SizeChanged);          

        }

        void LinearGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (autosize)
            {                
                foreach (LinearScale scale in this.Scales)
                {
                    //if (double.IsNaN(scale.barLength))
                    //{
                    //    scale.ClearScaleLabelTicks();
                    //    scale.RefreshScaleProperties();
                    //    scale.AddLabelTickSet();
                    //    scale.AddTickSet();
                    //}
                    //else
                    //{
                    //    scale.RefreshScaleProperties();
                    //    scale.RefreshLabelTickSet();
                    //    scale.RefreshTickSet();
                    //}
                }
            }
            else
            {
                foreach (LinearScale scale in this.Scales)
                {
                    scale.RefreshLabelTickSet();
                    scale.RefreshTickSet();
                }
            }
            this.RefreshBorders();

        }

        #endregion

        #region Implementation

        /// <summary>
        /// Gets the default Size
        /// </summary>
        /// <returns>Returns the size</returns>
        internal Size GetDefaultSize()
        {
            return new Size(CdefaultWidth, CdefaultHeight);
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected override void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.CollectionChanged(sender, e);
            if (e.NewItems != null)
            {
                int count = e.NewItems.Count;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (e.NewItems[i] is LinearScale)
                        {
                            LinearScale elem = e.NewItems[i] as LinearScale;
                            if (elem != null)
                            {
                                elem.ScaleBarLengthChanged += new PropertyChangedCallback(this.ScaleBarLengthChanged);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void LinearGaugeLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBorders();
            //  this.UpdateChildrenLocation();
            this.AdjustOrientationSettings();
           
        }

        /// <summary>
        /// Measure the Size
        /// </summary>
        /// <param name="availableSize">Passes available size</param>
        /// <returns>Returns the size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            try
            {
                return availableSize;
            }
            catch
            {
                return new Size(CdefaultWidth, CdefaultHeight);
            }
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
        /// Updates property value cache and raises OrientationChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            this.AdjustOrientationSettings();
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises RadiusXChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusXChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FirstCircleBorder != null)
            {
                this.FirstCircleBorder.pathonce = true;
                this.FirstCircleBorder.RefreshGaugeBorder();
            }

            if (this.SecondCircleBorder != null)
            {
                this.SecondCircleBorder.RefreshGaugeBorder();
            }

            if (this.InnerCircleBorder != null)
            {
                this.InnerCircleBorder.RefreshGaugeBorder();
            }

            if (this.RadiusXChanged != null)
            {
                this.RadiusXChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises RadiusYChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusYChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FirstCircleBorder != null)
            {
                this.FirstCircleBorder.pathonce = true;
                this.FirstCircleBorder.RefreshGaugeBorder();
            }

            if (this.SecondCircleBorder != null)
            {
                this.SecondCircleBorder.RefreshGaugeBorder();
            }

            if (this.InnerCircleBorder != null)
            {
                this.InnerCircleBorder.RefreshGaugeBorder();
            }

            if (this.RadiusYChanged != null)
            {
                this.RadiusYChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises RotationAngleChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRotationAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = this.RotationAngle;
            rotateTransform.CenterX = this.Width / 2;
            rotateTransform.CenterY = this.Height / 2;
            this.RenderTransform = rotateTransform;
            if (this.RotationAngleChanged != null)
            {
                this.RotationAngleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRotationAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnRotationAngleChanged(e);
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnOrientationChanged(e);
        }

        /// <summary>
        /// Calls OnRadiusXChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnRadiusXChanged(e);
        }

        /// <summary>
        /// Calls OnRadiusYChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnRadiusYChanged(e);
        }

        /// <summary>
        // /// Invoked when <see cref="scaleBarLength"/> property of a scale is changed.
        /// </summary>
        /// <param name="sender">The <see cref="LinearGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private void ScaleBarLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is LinearScale)
            {
                LinearScale scale = sender as LinearScale;
                double height = this.ActualHeight;
                if (scale.scaleBarLength < height)
                {
                    scale.LengthRatio = scale.scaleBarLength / height;
                }
            }
        }

        /// <summary>
        /// Set all needed settings according to the orientation.
        /// </summary>
        private void AdjustOrientationSettings()
        {
             if (this.Orientation == GaugeOrientation.Horizontal)
            {
                foreach (GaugeImage image in this.Images)
                {
                    image.Angle = -90d;
                }

                foreach (StateIndicator indicator in this.StateIndicators)
                {
                    indicator.Angle = -90d;
                }
                if (this.firstGlassPath != null)
                {
                    this.firstGlassPath.RenderTransform = new RotateTransform() { Angle = -90, CenterX = this.firstGlassPath.Width / 2, CenterY= this.firstGlassPath.Width / 2 };
                }
            }
            else
            {
                this.RotationAngle = 0d;
                foreach (GaugeImage image in this.Images)
                {
                    image.Angle = 0;
                }

                foreach (StateIndicator indicator in this.StateIndicators)
                {
                    indicator.Angle = 0;
                }
                if (this.firstGlassPath != null)
                {
                    this.firstGlassPath.RenderTransform = null;
                }
            }
            foreach (LinearScale scale in this.Scales)
            {
                scale.Orientation = this.Orientation;
            }
            
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Invoked when the Linear Gauge Size is changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void LinearGaugeSizeChanged(object sender, SizeChangedEventArgs e)
        {
            int count = this.Scales.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.Scales[i] is LinearScale)
                {
                    LinearScale scale = this.Scales[i] as LinearScale;
                    double width = this.ActualWidth;
                    double height = this.ActualHeight;
                    if (scale.LengthRatio != 0)
                    {
                        scale.scaleBarLength = height * scale.LengthRatio;
                    }
                    else if (scale.scaleBarLength < height)
                    {
                        scale.LengthRatio = scale.scaleBarLength / height;
                    }

                    scale.ScaleWidth = width;
                    scale.RefreshPointers();
                }
            }
        }

        /// <summary>
        /// Invokes to get the Gauge Size
        /// </summary>
        /// <returns>Returns the size</returns>
        internal Size GetGaugeSize()
        {
            double width =CdefaultWidth;
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
            if (tempheight <= 0)
            {
                height = ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1);
                this.Height = height;
            }
            if (tempwidth <= 0)
            {
                width = ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1);
                this.Width = width;
            }
            //height = tempheight <= 0 ? ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1) : height;
            //width = tempwidth <= 0 ? ((this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) + 1) : width;
            //this.Width = width;
            //this.Height = height;

            return new Size(width, height);
        }

        #endregion
    }
}
