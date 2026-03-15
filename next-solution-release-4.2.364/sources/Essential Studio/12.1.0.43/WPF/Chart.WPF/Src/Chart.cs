// <copyright file="Chart.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Linq;

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Input;
    using System.IO;
    using System.Windows.Xps;
    using System.Windows.Xps.Packaging;
    using System.IO.Packaging;
    using System.Printing;
    using System.Windows.Markup;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Collections;
    using System.Security.Permissions;
    using Microsoft.Win32;
    using Syncfusion.Licensing;
    using Syncfusion.Windows.Shared;
    using System.ComponentModel;
    using System.Xml;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using System.Text;
    using System.Collections.Generic;
using System.Collections.ObjectModel;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Automation;
    using System.Reflection;
    using System.Windows.Media.Media3D;
    

    /// <summary>
    /// Represents commands that can be invoked on <see cref="Chart"/>.
    /// </summary>
    /// <remarks>
    /// Commanding is an input mechanism in Windows Presentation Foundation 
    /// which provides input handling at a more semantic level than device input.
    /// </remarks>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public static class ChartCommands
    {
        #region Members
        /// <summary>
        /// Initializes c_switchPrinting RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_switchPrinting = new RoutedUICommand("SwitchPrinting", "SwitchPrinting", typeof(ChartCommands));
        /// <summary>
        /// Initializes c_addAnnotation RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_addAnnotation = new RoutedUICommand("AddAnnotation", "AddAnnotation", typeof(ChartCommands));

        /// <summary>
        /// Initializes c_propertiesDialogue RoutedUICommand
        /// </summary>
        private readonly static RoutedCommand c_propertiesDialogue = new RoutedUICommand("PropertiesDialogue", "PropertiesDialouge", typeof(ChartCommands));
        #endregion

        #region Properties

        /// <summary>
        /// Gets the switch printing.
        /// </summary>
        /// <value>The switch printing.</value>
        public static RoutedUICommand SwitchPrinting
        {
            get
            {
                return c_switchPrinting;
            }
        }
        /// <summary>
        /// To add the Annotation.
        /// </summary>
        /// <value>The Add Annotation.</value>
        public static RoutedCommand AddAnnotation
        {
            get
            {
                return c_addAnnotation;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties Dialogue.</value>
        public static RoutedCommand PropertiesDialogue
        {
            get
            {
                return c_propertiesDialogue;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents chart control.
    /// </summary>
    /// <remarks>
    /// A chart is a type of information graphic or graphic organizer that represents
    /// tabular numeric data and/or functions. Chart is often used to make it easier to
    /// understand large quantities of data and the relationship between different parts
    /// of the data. Char can usually be read more quickly than the raw data that they
    /// come from. <para> Certain <see cref="ChartTypes" /> are more useful for
    /// presenting a given data set than others. For example, data that presents
    /// percentages in different groups (such as "satisfied, not satisfied, unsure") are
    /// often displayed in a <see cref="ChartTypes.Pie" /> chart, but are more easily
    /// understood when presented in a horizontal <see cref="ChartTypes.Bar" /> chart.
    /// On the other hand, data that represents numbers that change over a period of
    /// time (such as "annual revenue from 1990 to 2000") might be best shown as a <see
    /// cref="ChartTypes.Line" /> chart. </para>
    /// </remarks>
    /// <example>
    /// XAML: <code language="XAML">
    /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// Width="300" Height="300"&gt;
    /// &lt;!--Adding chart control to window's content--&gt;
    /// &lt;syncfusion:Chart
    /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
    /// &lt;!--Assigning chart area--&gt;
    /// &lt;syncfusion:ChartArea&gt;
    /// &lt;!--Adding series to area--&gt;
    /// &lt;syncfusion:ChartArea.Series&gt;
    /// &lt;!--Assigning data property--&gt;
    /// &lt;syncfusion:ChartSeries Data="5 5 6 6 7 7"/&gt;
    /// &lt;/syncfusion:ChartArea.Series&gt;
    /// &lt;/syncfusion:ChartArea&gt;
    /// &lt;/syncfusion:Chart&gt;
    /// &lt;/Window&gt;
    /// </code> C#: <code language="C#">
    /// public partial class Window1 : Window
    /// {
    /// public Window1()
    /// {
    /// InitializeComponent();
    /// //Creating new chart instance.
    /// Chart chart = new Chart();
    /// //Adding new area.
    /// chart.Areas.Add(new ChartArea());
    /// //Creating chart data points.
    /// ChartListData data = new ChartListData();
    /// data.Add(new ChartPoint(1, 1));
    /// data.Add(new ChartPoint(2, 2));
    /// data.Add(new ChartPoint(3, 3));
    /// data.Add(new ChartPoint(4, 4));
    /// data.Add(new ChartPoint(5, 5));
    /// ////Adding new series.
    /// chart.Areas[0].Series.Add(new ChartSeries());
    /// ////Assigning data to series.
    /// chart.Areas[0].Series[0].Data = data;
    /// //Assigning window's content property.
    /// this.Content = chart;
    /// }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ChartArea">ChartArea</seealso>
    /// <seealso cref="ChartSeries">ChartSeries</seealso>
    /// <seealso cref="ChartTypes">ChartTypes</seealso>
    /// <seealso cref="ChartPoint">ChartPoint</seealso>

#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/VS2010Style.xaml")] 
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/ClassicStyle.xaml")] 
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(Chart), XamlResource = "/Syncfusion.Chart.WPF;component/Themes/MetroStyle.xaml")] 
    [StyleTypedProperty(Property = "AreaStyle", StyleTargetType = typeof(ChartArea))]
    [StyleTypedProperty(Property = "LegendStyle", StyleTargetType = typeof(ChartLegend))]
    [StyleTypedProperty(Property = "ToolBarStyle", StyleTargetType = typeof(ChartToolBar))]
    [ContentProperty("Areas")]
    [TemplatePart(Name = "PART_ChartDockPanel", Type = typeof(ChartDockPanel))]
   
    public class Chart : Control, IDisposable, IChartSerializer
    {

        private class ChartAutomationPeer : FrameworkElementAutomationPeer 
        {
           
            public ChartAutomationPeer(Chart control)
                : base(control)
            {

            }

            #region AutomationPeer overrides            

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Custom;
            }
              
            protected override string GetLocalizedControlTypeCore()
            {
                return "Chart";
            }
            protected override string GetClassNameCore()
            {
                return this.MyOwner.GetType().Name;
            }           
           
            public override object GetPattern(PatternInterface patternInterface)
            { 
                return this;            
            }

            #endregion  

            
           
            private Chart MyOwner
            {
                get
                {
                    return (Chart)base.Owner;
                }
            }
        
        }


        /// <summary>
        /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartAutomationPeer(this);
        }     

        #region Constants
        /// <summary>
        /// Initializes c_defaultDim
        /// </summary>
        private const int C_defaultDim = 96;

        /// <summary>
        /// Initializes c_dockPanelName
        /// </summary>
        private const string C_dockPanelName = "PART_ChartDockPanel";

        /// <summary>
        /// Initializes c_imageFilesFilter
        /// </summary>
        private const string C_imageFilesFilter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif (*.gif)|*.gif|TIFF(*.tiff)|*.tiff|PNG(*.png)|*.png|WDP(*.wdp)|*.wdp|Xps file (*.xps)|*.xps|All files (*.*)|*.*";
        ////private readonly static Uri c_printUri = new Uri("memorystream://printstream");

        /// <summary>
        /// Initializes isChartClose
        /// </summary>
        public bool isChartClose = false;
        #endregion

        #region Members
        /// <summary>
        /// Initialize the Root Parent element of Chart
        /// </summary>
        FrameworkElement rootParent = null;

        /// <summary>
        /// Initializes m_dockPanel
        /// </summary>
        private ChartDockPanel m_dockPanel;

        /// <summary>
        /// Initializes m_areas
        /// </summary>
        private ChartAreasCollection m_areas = new ChartAreasCollection();

        /// <summary>
        /// Initializes m_legends
        /// </summary>
        private ChartLegendsCollection m_legends = new ChartLegendsCollection();

        /// <summary>
        /// Initializes m_toolbars
        /// </summary>
        private ChartToolBarCollection m_toolbars = new ChartToolBarCollection();

        /// <summary>
        /// Initializes m_internalSeriesList
        /// </summary>
        /// <seealso cref="Chart"/>
        internal ChartSeriesCollection m_internalSeriesList = new ChartSeriesCollection();

        /// <summary>
        /// Initializes m_annotations
        /// </summary>
        private ChartAnnotationLabelsCollection m_annotations = new ChartAnnotationLabelsCollection();

        /// <summary>
        /// Declares the m_hiddenToolBarsCollection member;
        /// </summary>
       // private ChartToolBarCollection m_hiddenToolBarsCollection = new ChartToolBarCollection();

        double headerFonSize = 12;

        internal Point AnnotationPoint = new Point();

        internal bool m_isFontFamilySet = false;
        internal bool m_isFontSizeSet = false;
        internal bool m_isFontWeightSet = false;
        #endregion

        #region DependencyProperties
       
        /// <summary>
        /// Enables or disables the Toolbar on printing and saving
        /// </summary>
        public static readonly DependencyProperty ShowToolBarOnPrintAndSaveProperty =
         DependencyProperty.Register("ShowToolBarOnPrintAndSave", typeof(bool), typeof(Chart), new PropertyMetadata(true));

        /// <summary>
        /// Get and set the ShowToolBarOnPrintAndSave property
        /// </summary>
        public bool ShowToolBarOnPrintAndSave
        {
            get
            {
                return (bool)GetValue(ShowToolBarOnPrintAndSaveProperty);
            }

            set
            {
                SetValue(ShowToolBarOnPrintAndSaveProperty, value);
            }
        }
        
        /// <summary>
        /// Identifies the LegendStyle dependency property.
        /// </summary>
        //[StyleTypedProperty(Property = "LegendStyle", StyleTargetType = typeof(Chart))]

        public static readonly DependencyProperty LegendStyleProperty =
         DependencyProperty.Register("LegendStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnLegendStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style LegendStyle
        {
            get
            {
                return (Style)GetValue(LegendStyleProperty);
            }

            set
            {
                SetValue(LegendStyleProperty, value);
            }
        }

        DependencyPropertyDescriptor FontFamilyDescriptor = DependencyPropertyDescriptor.FromProperty(Chart.FontFamilyProperty, typeof(Chart));
        DependencyPropertyDescriptor FontSizeDescriptor = DependencyPropertyDescriptor.FromProperty(Chart.FontSizeProperty, typeof(Chart));
        DependencyPropertyDescriptor FontWeightDescriptor = DependencyPropertyDescriptor.FromProperty(Chart.FontWeightProperty, typeof(Chart));

        /// <summary>
        /// Identifies the ToolBarStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolBarStyleProperty =
        	DependencyProperty.Register("ToolBarStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnToolBarStyleChanged)));

        /// <summary>
        /// Gets or sets the ToolBarStyle value.
        /// </summary>
        /// <value>The ToolBarStyle.</value>
        public Style ToolBarStyle
        {
            get
            {
                return (Style)GetValue(ToolBarStyleProperty);
            }

            set
            {
                SetValue(ToolBarStyleProperty, value);
            }
        }

        /// <summary>
        /// Property for Remove the chart from their parent
        /// </summary>
        public static readonly DependencyProperty DisposeOnUnloadProperty =
      DependencyProperty.Register("DisposeOnUnload", typeof(bool), typeof(Chart), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the DisposeOnUnload value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public bool DisposeOnUnload
        {
            get
            {
                return (bool)GetValue(DisposeOnUnloadProperty);
            }

            set
            {
                SetValue(DisposeOnUnloadProperty, value);
            }
        }
        /// <summary>
        /// VisualStyle for Chart
        /// </summary>
        public static readonly DependencyProperty ChartVisualStyleProperty =
       DependencyProperty.Register("ChartVisualStyle", typeof(ChartStyles), typeof(Chart), new PropertyMetadata(ChartStyles.None, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public ChartStyles ChartVisualStyle
        {
            get
            {
                return (ChartStyles)GetValue(ChartVisualStyleProperty);
            }

            set
            {
                SetValue(ChartVisualStyleProperty, value);
            }
        }
        /// <summary>
        /// Style for ChartArea
        /// </summary>
        public static readonly DependencyProperty AreaStyleProperty =
         DependencyProperty.Register("AreaStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnAreaStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style AreaStyle
        {
            get
            {
                return (Style)GetValue(AreaStyleProperty);
            }

            set
            {
                SetValue(AreaStyleProperty, value);
            }
        }
        /// <summary>
        /// Identifies the AnnotationLabelTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty AnnotationLabelTemplateProperty =
            DependencyProperty.Register("AnnotationLabelTemplate", typeof(DataTemplate), typeof(Chart), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the LegendName dependency property.
        /// </summary>
        public static DependencyProperty LegendNameProperty =
            DependencyProperty.RegisterAttached("LegendName", typeof(string), typeof(Chart), new FrameworkPropertyMetadata("DefaultLegend"));

        /// <summary>
        /// Identifies the AreasPanel dependency property.
        /// </summary>
        public static readonly DependencyProperty AreasPanelProperty =
            DependencyProperty.Register("AreasPanel", typeof(ItemsPanelTemplate), typeof(Chart), new UIPropertyMetadata(new ItemsPanelTemplate(new FrameworkElementFactory(typeof(ChartGrid)))));

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty DockProperty = ChartDockPanel.DockProperty.AddOwner(typeof(Chart));

        /// <summary>
        /// Identifies the alignment dependency property.
        /// </summary>
        public static readonly DependencyProperty AlignmentProperty = ChartDockPanel.AlignmentProperty.AddOwner(typeof(Chart));

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Chart), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure), new ValidateValueCallback(IsCornerRadiusValid));


        /// <summary>
        /// Identifies the ToolBar dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolBarProperty =
            DependencyProperty.Register("ToolBar", typeof(ChartToolBar), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnToolBarChanged), new CoerceValueCallback(OnCoerceToolBar)));

        /// <summary>
        /// Identifies the PropertyDialogItem Count dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertiesDialogItemsCountProperty =
         DependencyProperty.Register("PropertiesDialogItemsCount", typeof(int), typeof(Chart), new PropertyMetadata(0, new PropertyChangedCallback(OnItemAdded)));

        /// <summary>
        /// Identifies the TabStop dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabStopProperty =
    DependencyProperty.Register("TabStop", typeof(bool), typeof(Chart), new PropertyMetadata(false, new PropertyChangedCallback(OnTabStopChanged)));


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("ChartHeader", typeof(object), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }

            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }


        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = d as Chart;
            if (chart != null)
            {
                if(chart.HeaderPresenter!=null)
                chart.HeaderPresenter.Content = e.NewValue;
            }
        }

       

        /// <summary>
        /// Proeprty for Align the ChartHeader
        /// </summary>
        public static readonly DependencyProperty HeaderAlignmentProperty =
          DependencyProperty.Register("HeaderAlignment", typeof(HorizontalAlignment), typeof(Chart), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Get and Set the HeaderAlignment proeprty
        /// </summary>
        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderAlignmentProperty); }
            set { SetValue(HeaderAlignmentProperty, value); }
        }

		/// <summary>
        /// Identifies the footer dependency property.
        /// </summary>
        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register("ChartFooter", typeof(object), typeof(Chart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        /// <value>The Footer.</value>
        public object Footer
        {
            get
            {
                return this.GetValue(FooterProperty);
            }

            set
            {
                this.SetValue(FooterProperty, value);
            }
        }

        /// <summary>
        /// Identifies the footer alignment dependency property.
        /// </summary>
        public static readonly DependencyProperty FooterAlignmentProperty =
          DependencyProperty.Register("FooterAlignment", typeof(HorizontalAlignment), typeof(Chart), new PropertyMetadata(HorizontalAlignment.Right));

        /// <summary>
        /// Gets or sets the footer alignment.
        /// </summary>
        public HorizontalAlignment FooterAlignment
        {
            get { return (HorizontalAlignment)GetValue(FooterAlignmentProperty); }
            set { SetValue(FooterAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the PropertiesDialogItemsCount.
        /// </summary>
        /// <value>The PropertiesDialogItemsCount.</value>
        private int PropertiesDialogItemsCount
        {
            get
            {
                return (int)GetValue(PropertiesDialogItemsCountProperty);
            }

            set
            {
                SetValue(PropertiesDialogItemsCountProperty, value);
            }
        }





        /// <summary>
        /// Event that is raised when CustomTab Pages Initialized.
        /// </summary>
        public event RoutedEventHandler InitializeCustomTabPages;

        /// <summary>
        /// Invoked when CustomTab Added
        /// </summary>
        /// <param name="d">The dependency object d.</param>
        /// <param name="e">The dependencyProperty changed eventargs e</param> 
        /// <seealso cref="Chart"/>
        private static void OnItemAdded(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart instance = (Chart)d;

            Console.WriteLine(e.NewValue.ToString());
            if (instance.InitializeCustomTabPages != null)
            {
                instance.InitializeCustomTabPages(instance, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Identifies the ApplyPropertyPages dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplyPropertyPagesProperty =
            DependencyProperty.Register("ApplyPropertyPages", typeof(int), typeof(Chart), new PropertyMetadata(0, new PropertyChangedCallback(OnApplied)));

        /// <summary>
        /// Gets or sets the PropertyPages.
        /// </summary>
        /// <value>The ApplyPropertyPages.</value>
        public int ApplyPropertyPages
        {
            get
            {
                return (int)GetValue(ApplyPropertyPagesProperty);
            }

            set
            {
                SetValue(ApplyPropertyPagesProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when CustomTab Pages Applied.
        /// </summary>
        public event RoutedEventHandler ApplyCustomTabPages;

        /// <summary>
        /// Invoked when CustomTab Applied
        /// </summary>
        /// <param name="d">The dependency object d.</param>
        /// <param name="e">The dependencyProperty changed eventargs e</param>  
        private static void OnApplied(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart instance = (Chart)d;

            Console.WriteLine(e.NewValue.ToString());
            if (instance.ApplyCustomTabPages != null)
            {
                instance.ApplyCustomTabPages(instance, new RoutedEventArgs());
            }
        }

         internal static DependencyObject FindInVisualTreeDown(DependencyObject obj, Type type)
        {
            if (obj != null)
            {
                if (obj.GetType() == type)
                {
                    return obj;
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, i), type);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Identifies the AnnotationIntersectAction dependency property.
        /// </summary>
        public static readonly DependencyProperty AnnotationIntersectActionProperty =
            DependencyProperty.Register("AnnotationIntersectAction", typeof(AnnotationIntersectActions), typeof(Chart), new PropertyMetadata(AnnotationIntersectActions.None));

        /// <summary>
        /// Set the Visiblility of Annotation when get intersect
        /// </summary>
        public AnnotationIntersectActions AnnotationIntersectAction
        {
            get { return (AnnotationIntersectActions)GetValue(AnnotationIntersectActionProperty); }
            set { SetValue(AnnotationIntersectActionProperty, value); }
        }
        #endregion

        #region Properties
        internal ChartAnnotationsPresenter Annot_Presenter = null;
        /// <summary>
        /// Gets or sets the LabelTemplate. This is a dependency property.
        /// </summary>
        /// <value>The LabelTemplate.</value>
        public DataTemplate AnnotationLabelTemplate
        {
            get { return (DataTemplate)GetValue(AnnotationLabelTemplateProperty); }
            set { SetValue(AnnotationLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets the <see cref="ChartAnnotationLabelsCollection"/>.
        /// </summary>
        /// <value>The Labels collection.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]        
        public ChartAnnotationLabelsCollection AnnotationLabels
        {
            get
            {
                return m_annotations;
            }
            set
            {
                foreach (ChartAnnotationLabel label in value)
                {
                    if (label.AnnotationChart == null)
                    {
                        label.AnnotationChart = this;
                    }
                }
                if (this.Annot_Presenter != null)
                {
                    if (this.Annot_Presenter.ParentChart == null)
                    {
                        this.Annot_Presenter.ParentChart = this;
                    }
                    this.Annot_Presenter.InvalidateMeasure();
                }         
				//To set the Chart to the AnnotationChart property in the ChartAnnotationLabel                            
                m_annotations = value;
            }
        }

        /// <summary>
        /// Gets or sets corner radius. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property affects chart's border corner radius. Changing chart's default
        /// template might make this property useless.
        /// </remarks>
        /// <example>
        /// XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content and setting its corner
        /// radius--&gt;
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"
        /// CornerRadius="10,1,6,2"&gt;
        /// &lt;!--Assigning chart area--&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;!--Adding series to area--&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;!--Assigning data property--&gt;
        /// &lt;syncfusion:ChartSeries Data="5 5 6 6 7 7"/&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code> C#: <code language="C#">
        /// public partial class Window1 : Window
        /// {
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Assigning CornerRadius property.
        /// chart.CornerRadius = new CornerRadius(10, 1, 6, 2);
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// ////Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// ////Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// }
        /// </code>
        /// </example>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets the collection of <see cref="ChartLegend" />.
        /// </summary>
        /// <remarks>
        /// Chart control has ability to display numerous legends. All legends are added to
        /// <see cref="ChartDockPanel" /> automatically. <para /> <see
        /// cref="LegendNameProperty" /> attached property can be used to assign series to
        /// different legends. By default, <see cref="ChartLegend" /> shows all series on
        /// chart. <para /> <see
        /// cref="ChartDockPanel.DockProperty">ChartDockPanel.Dock</see> attached property
        /// can be used to assign legend's position on dock panel.
        /// </remarks>
        /// <value>
        /// The legends collection.
        /// </value>
        /// <example>
        /// Code sample demonstrates ability to display on legend required series only.
        /// <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;!--Assigning Legends property of chart--&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;!--Adding bottom legend with "legend1" name--&gt;
        /// &lt;syncfusion:ChartLegend syncfusion:ChartDockPanel.Dock="Bottom"
        /// syncfusion:Chart.LegendName="Legend1"/&gt;
        /// &lt;!--Adding right legend with "legend2" name--&gt;
        /// &lt;syncfusion:ChartLegend syncfusion:ChartDockPanel.Dock="Right"
        /// syncfusion:Chart.LegendName="Legend2"/&gt;
        /// &lt;!--Adding left legend with "legend3" name--&gt;
        /// &lt;syncfusion:ChartLegend syncfusion:ChartDockPanel.Dock="Left"
        /// syncfusion:Chart.LegendName="Legend3"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;!--Assigning chart area--&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;!--Adding series to area--&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;!--Adding 1st series and attaching legend's name--&gt;
        /// &lt;syncfusion:ChartSeries Data="5 5 6 6 7 7" Label="series1"
        /// syncfusion:Chart.LegendName="Legend1"/&gt;
        /// &lt;!--Adding 2nd series and attaching legend's name--&gt;
        /// &lt;syncfusion:ChartSeries Data="2 5 3 6 4 1" Label="series2"
        /// syncfusion:Chart.LegendName="Legend2"/&gt;
        /// &lt;!--Adding 3rd series and attaching legend's name--&gt;
        /// &lt;syncfusion:ChartSeries Data="5 2 6 6 7 7" Label="series3"
        /// syncfusion:Chart.LegendName="Legend3"/&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code> C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating series1 instance.
        /// ChartSeries series1 = new ChartSeries();
        /// //Setting label and assigning data.
        /// series1.Label = "series1";
        /// series1.Data = this.gentrateRandomChartData();
        /// //Setting legend name.
        /// Chart.SetLegendName(series1, "Legend1");
        /// //Creating series2 instance.
        /// ChartSeries series2 = new ChartSeries();
        /// //Setting label and assigning data.
        /// series2.Label = "series2";
        /// series2.Data = this.gentrateRandomChartData();
        /// //Setting legend name.
        /// Chart.SetLegendName(series2, "Legend2");
        /// //Creating series2 instance.
        /// ChartSeries series3 = new ChartSeries();
        /// //Setting label and assigning data.
        /// series3.Label = "series3";
        /// series3.Data = this.gentrateRandomChartData();
        /// //Setting legend name.
        /// Chart.SetLegendName(series3, "Legend3");
        /// //Creating legend1 instance.
        /// ChartLegend legend1 = new ChartLegend();
        /// //Setting LegendName property.
        /// legend1.SetValue(Chart.LegendNameProperty, "Legend1");
        /// //Creating legend1 instance.
        /// ChartLegend legend2 = new ChartLegend();
        /// //Setting LegendName property.
        /// legend2.SetValue(Chart.LegendNameProperty, "Legend2");
        /// //Creating legend1 instance.
        /// ChartLegend legend3 = new ChartLegend();
        /// //Setting LegendName property.
        /// legend3.SetValue(Chart.LegendNameProperty, "Legend3");
        /// //Adding created series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// chart.Areas[0].Series.Add(series2);
        /// chart.Areas[0].Series.Add(series3);
        /// //Adding created legends to chart.
        /// chart.Legends.Add(legend1);
        /// chart.Legends.Add(legend2);
        /// chart.Legends.Add(legend3);
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// //Generates random series' points.
        /// ChartListData gentrateRandomChartData()
        /// {
        /// ChartListData returnValue = new ChartListData();
        /// Random randomizer = new Random();
        /// for (int i = 0; i &lt; 10; i++)
        /// {
        /// returnValue.Add(new ChartPoint(i, randomizer.Next((i + 1) * 10)));
        /// }
        /// return returnValue;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ChartLegend">ChartLegend</seealso>
        /// <seealso cref="ChartLegendsCollection">ChartLegendsCollection</seealso>
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartLegendsCollection Legends
        {
            get
            {
                return m_legends;
            }
            set
            {
                m_legends = value;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="ChartArea">ChartAreas</see>.
        /// </summary>
        /// <remarks>
        /// Chart control may contain multiple areas. <see cref="ChartArea"/> is main container for <see cref="ChartSeries"/>.
        /// <para/>
        /// By default, chart areas are added to <see cref="ChartGrid"/>. Default panel can be changed using <see cref="Chart.AreasPanel"/> property.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating 1st series.
        /// ChartSeries series1 = new ChartSeries();
        /// series1.Type = ChartTypes.Pie;
        /// series1.Data = data;
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// series2.Type = ChartTypes.Column;
        /// series2.Data = data;
        /// //Creating third series.
        /// ChartArea chartArea1 = new ChartArea();
        /// ChartArea chartArea2 = new ChartArea();
        /// chartArea1.Series.Add(series1);
        /// chartArea2.Series.Add(series2);
        /// chart.Areas.Add(chartArea1);
        /// chart.Areas.Add(chartArea2);
        /// </code>
        /// XAML:
        /// <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        ///    Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        ///    &lt;syncfusion:Chart.Areas&gt;
        ///        &lt;!--Adding multiple areas to collection--&gt;
        ///        &lt;syncfusion:ChartArea&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 6 6" Type="Pie"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///        &lt;syncfusion:ChartArea&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 6 6" Type="Column"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///        &lt;syncfusion:ChartArea&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 6 6" Type="Pyramid"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///    &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartAreasCollection Areas
        {
            get
            {
                return m_areas;
            }
            set
            {
                m_areas = value;
            }

        }

        /// <summary>
        /// Gets the collection of <see cref="ChartToolBar">ChartToolBar</see>.
        /// </summary>
        [XmlIgnore]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private ChartToolBarCollection ToolBars
        {
            get
            {
                return m_toolbars;
            }
        }

        /// <summary>
        /// Gets or sets the areas panel template. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// By default, chart areas are contained in <see cref="ChartGrid"/>. Property provides ability to change areas host panel.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// This property is not intended to be used from C#.
        /// </code>
        /// XAML:
        /// <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        ///    Width="300" Height="400"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        ///    &lt;syncfusion:Chart.AreasPanel&gt;
        ///        &lt;ItemsPanelTemplate&gt;
        ///            &lt;DockPanel/&gt;
        ///        &lt;/ItemsPanelTemplate&gt;
        ///    &lt;/syncfusion:Chart.AreasPanel&gt;
        ///    &lt;syncfusion:Chart.Areas&gt;
        ///        &lt;!--Adding multiple areas to collection and setting their Dock property.--&gt;
        ///        &lt;syncfusion:ChartArea DockPanel.Dock="Bottom" Height="100"&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 6 6" Type="Pie"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///        &lt;syncfusion:ChartArea DockPanel.Dock="Top" Height="100"&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 6 6" Type="Column"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///        &lt;syncfusion:ChartArea DockPanel.Dock="Right" Height="100"&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 6 6" Type="Pyramid"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///    &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <value>The areas panel template.</value>
        public ItemsPanelTemplate AreasPanel
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(AreasPanelProperty);
            }

            set
            {
                SetValue(AreasPanelProperty, value);
            }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An enumerator for logical child elements of this element.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Areas != null)
                    return Areas.GetEnumerator();
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the Chart ToolBar
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartToolBar ToolBar
        {
            get
            {
                return (ChartToolBar)GetValue(ToolBarProperty);
            }

            set
            {
                SetValue(ToolBarProperty, value);
            }
        }


        /// <summary>
        /// Get and Set the PropertyWindowTabs proeprty
        /// </summary>
        public TabItemCollection PropertyWindowTabs
        {
            get
            {
                return (TabItemCollection)GetValue(PropertyWindowTabsProperty);
            }

            set
            {
                SetValue(PropertyWindowTabsProperty, value);
            }
        }

        //internal static bool IsDefualtPropertyTabsLoaded=false;

        internal TabItemCollection GetDefaultPropertyDialogTabs()
        {
            //if (!IsDefualtPropertyTabsLoaded)
            //{   
            ChartResourceWrapper wrapper = new ChartResourceWrapper();
            TabItemCollection _tabs = new TabItemCollection();
            ChartAreaPropertiesView areaView = new ChartAreaPropertiesView();
            ChartSeriesPropertiesView seriesView = new ChartSeriesPropertiesView();
            ChartAxisPropertiesView axisView = new ChartAxisPropertiesView();
            seriesView.SetBinding(DataContextProperty, new Binding("SelectedItem") { Source = areaView.ChartAreasList });
            axisView.SetBinding(DataContextProperty, new Binding("SelectedItem") { Source = areaView.ChartAreasList });
            _tabs.Add(new TabItem() { Header = wrapper.Chart, Content = new ChartPropertiesView() });
            _tabs.Add(new TabItem() { Header = wrapper.ChartArea, Content = areaView });
            _tabs.Add(new TabItem() { Header = wrapper.ChartSeries, Content = seriesView });
            _tabs.Add(new TabItem() { Header = wrapper.ChartAxis, Content = axisView });
            _tabs.Add(new TabItem() { Header = wrapper.ChartLegend, Content = new ChartLegendPropertiesView() });

            //IsDefualtPropertyTabsLoaded = true;
            return _tabs;
            //}
            //return null;
        }

        /// <summary>
        /// PropertyWindowTabsProperty initialization
        /// </summary>
        public static readonly DependencyProperty PropertyWindowTabsProperty =
                DependencyProperty.Register("PropertyWindowTabs", typeof(TabItemCollection), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(OnPropertyWindowTabsChanged)));

        /// <summary>
        /// Method for ProeprtyWindowTab changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnPropertyWindowTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            if (chart != null)
                chart.OnPropertyWindowTabsChanged(args);
        }

        /// <summary>
        /// Method for ProeprtyWindowTab changed
        /// </summary>
        /// <param name="args"></param>
        public void OnPropertyWindowTabsChanged(DependencyPropertyChangedEventArgs args)
        {
            TabItemCollection collection = args.NewValue as TabItemCollection;
            if (collection != null && collection.Count > 0 && ChartPropertyWindow !=null)
            {
                ChartPropertyWindow.AddTab(collection);
                ChartPropertyWindow.SetChart(this);
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="Chart"/> class.
        /// </summary>
        static Chart()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(Chart));
            ////This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            ////This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));

            //// Properties
            ChartDockPanel.DockProperty.OverrideMetadata(typeof(ChartLegend), new PropertyMetadata(ChartDock.Top, new PropertyChangedCallback(OnLegendDockChanged)));
            ChartDockPanel.DockProperty.OverrideMetadata(typeof(ChartToolBar), new PropertyMetadata(ChartDock.Top, new PropertyChangedCallback(OnToolBarDockChanged)));

            //// Commands
            CommandBinding printBinding = new CommandBinding(ApplicationCommands.Print, new ExecutedRoutedEventHandler(OnPrintCommand));
            CommandBinding switchPrintingBinding = new CommandBinding(ChartCommands.SwitchPrinting, new ExecutedRoutedEventHandler(OnSwitchPrintingCommand));
            CommandBinding addAnnotationBinding = new CommandBinding(ChartCommands.AddAnnotation, new ExecutedRoutedEventHandler(OnAddAnnotationCommand));
            CommandBinding saveBinding = new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(OnSaveCommand));
            CommandBinding copyBinding = new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(OnCopyCommand));
            CommandBinding closeBinding = new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(OnCloseCommand));

            // CommandBinding propertiesDialogue = new CommandBinding(ApplicationCommands.Open, new ExecutedRoutedEventHandler(OnPropertiesDialogueCommand));
            CommandManager.RegisterClassCommandBinding(typeof(Chart), printBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), switchPrintingBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), saveBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), copyBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), closeBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), addAnnotationBinding);
        }       

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        public Chart()
        {
            if (IsSecurityGranted)
            {
                ValidateLicense();
            }
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level =System.Diagnostics.SourceLevels.Critical;
            this.DefaultStyleKey = typeof(Chart);
            this.PropertyWindowTabs = new TabItemCollection();
            this.PropertyWindowTabs.CollectionChanged += new NotifyCollectionChangedEventHandler(OnPropertyWindowTabsChanged);
            m_legends.CollectionChanged += new NotifyCollectionChangedEventHandler(OnLegendsChanged);
            m_toolbars.CollectionChanged += new NotifyCollectionChangedEventHandler(OnToolBarsChanged);           
            m_areas.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAreasChanged);
            this.CoerceValue(ToolBarProperty);

            Binding tabStopBinding = new Binding();
            tabStopBinding.Source = this;
            tabStopBinding.Path = new PropertyPath("IsTabStop");
            this.SetBinding(Chart.TabStopProperty, tabStopBinding);

            this.Unloaded += new RoutedEventHandler(Chart_Unloaded);
            Header = null;

            FontFamilyDescriptor.AddValueChanged(this, new EventHandler(FontFamilyChanged));
            FontSizeDescriptor.AddValueChanged(this, new EventHandler(FontSizeChanged));
            FontWeightDescriptor.AddValueChanged(this, new EventHandler(FontWeightChanged));

            //to supress the data binding issues.
            //System.Diagnostics.PresentationTraceSources.DataBindingSource.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
            //System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

            this.AnnotationLabels = new ChartAnnotationLabelsCollection();
            this.AnnotationLabels.CollectionChanged += AnnotationLabels_CollectionChanged;
            this.Loaded += new RoutedEventHandler(Chart_Loaded);
        }        

        private void AnnotationLabels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ChartAnnotationLabel label in this.AnnotationLabels)
            {
                if (label.AnnotationChart == null)
                {
                    label.AnnotationChart = this;
                }
            }    
            if (this.Annot_Presenter != null)
            {
                if (this.Annot_Presenter.ParentChart == null)
                {
                    this.Annot_Presenter.ParentChart = this;
                }
                this.Annot_Presenter.InvalidateMeasure();
            }            
        }

        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(Chart));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        void Chart_Loaded(object sender, RoutedEventArgs e)
        {
            if (rootParent == null)
            {
                rootParent = GetRootParentElement(this.Parent) as FrameworkElement;
                if (rootParent != null)
                {
                    rootParent.Unloaded += new RoutedEventHandler(Parent_Unloaded);
                }
            }

            //if (this.Parent != null && this.Parent as FrameworkElement != null)
            //{
            //    (this.Parent as FrameworkElement).Unloaded += new RoutedEventHandler(Parent_Unloaded);
            //}
        }

        private void FontFamilyChanged(object sender, EventArgs e)
        {
            Chart chart = sender as Chart;
            chart.m_isFontFamilySet = true;
        }

        private void FontWeightChanged(object sender, EventArgs e)
        {
            Chart chart = sender as Chart;
            chart.m_isFontWeightSet = true;
        }

        private void FontSizeChanged(object sender, EventArgs e)
        {
            Chart chart = sender as Chart;
            chart.m_isFontSizeSet = true;
        }

        void Parent_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DisposeOnUnload)
            {
                this.Dispose();
            }
            rootParent = null;
            if (sender is FrameworkElement)
            {
                (sender as FrameworkElement).Unloaded -= Parent_Unloaded;
            }
        }

        DependencyObject GetRootParentElement(DependencyObject element)
        {
            while (element != null && element is FrameworkElement && !(element is Window))
            {
                element = (element as FrameworkElement).Parent;
            }

            return element;
        }

        void Chart_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ChartPropertyWindow != null)
            {
                ChartPropertyWindowIsOpen = false;
                this.ChartPropertyWindow.Close();
            }
            if (this.DisposeOnUnload)
            {
              DisposeChart();
              this.Dispose();
            }
        }

       
        private void DisposeChart()
        {
            #region Dispose All the members        
            this.Loaded-= new RoutedEventHandler(Chart_Loaded);
            this.Unloaded-=new RoutedEventHandler(Chart_Unloaded);
            this.Loaded-=new RoutedEventHandler(Chart_Loaded);
            // this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            //{
            if (rootParent != null)
            {
                rootParent.Unloaded -= new RoutedEventHandler(Parent_Unloaded);
            }
            if (m_temparea != null)
            {
                 foreach (var areas in m_temparea)
                 {
                    if (areas is SyncChartAreas)
                        {
                            SyncChartAreas area = areas as SyncChartAreas;
                             if (area.Areas != null)
                                area.Areas.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSyncChartAreasChanged);
                        }
                        else
                        {
                            ChartArea area = areas  as ChartArea;
                            if (area.Series != null)
                                area.Series.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSeriesChanged);
                        }
                }
                this.m_temparea.Clear();
                this.m_temparea = null;
            }
            if (this.Areas != null)
            {
                m_areas.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnAreasChanged);
                foreach (ChartArea item in this.Areas)
                {
                    item.Dispose();
                    //}
                }
                this.Areas.Clear();
                this.Areas = null;
                m_areas = null;
                GC.SuppressFinalize(this);
            }
            
            if (this.m_internalSeriesList != null)
            {
                foreach (ChartSeries item in this.m_internalSeriesList)
                {
                    item.Dispose();
                }
                m_internalSeriesList.Dispose();
                this.m_internalSeriesList.Clear();
                this.m_internalSeriesList = null;
            }

            if (this.m_internalSeriesList != null)
            {                
                this.m_internalSeriesList.Dispose();
                this.m_internalSeriesList = null;
            }

            if (this.AreasPanel != null)
            {
                ChartGrid panel = this.AreasPanel.LoadContent() as ChartGrid;

                if (panel != null)
                {
                    panel = null;
                }

                this.AreasPanel = null;
            }

            this.ClearValue(Chart.DockProperty);
            this.ClearValue(ChartDockPanel.DockProperty);

            if (this.m_dockPanel != null)
            {
                this.m_dockPanel.Dispose();
                this.m_dockPanel = null;
            }

            if (this.m_legends != null)
            {
                m_legends.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnLegendsChanged);
                foreach (ChartLegend item in m_legends)
                {
                    item.Dispose();
                }
                this.m_legends.Clear();
                this.m_legends = null;

            }            

            //if (this.m_hiddenToolBarsCollection != null)
            //{

            //    this.m_hiddenToolBarsCollection.Clear();
            //    this.m_hiddenToolBarsCollection = null;
            //    this.ToolBar = null;
            //}

            if (this.m_toolbars != null)
            {
                m_toolbars.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnToolBarsChanged);
                foreach (ChartToolBar item in m_toolbars)
                {
                    item.Dispose();
                }
                m_toolbars.Clear();
                m_toolbars = null;
            }

            if (this.AnnotationLabels != null)
            {
                this.m_annotations.Dispose();
                this.m_annotations.Clear();
                this.m_annotations = null;

            }
            this.AnnotationLabelTemplate = null;


            if (this.Template != null)
            {
                if (((ControlTemplate)(this.Template)).Resources != null)
                {
                    ((ControlTemplate)(this.Template)).Resources.MergedDictionaries.Clear();
                }
            }


            if (this.Template != null && this.Template.HasContent == true)
            {
                Border obj = this.Template.LoadContent() as Border;
                obj.Child = null;
                obj = null;
            }

            if (this.Resources != null)
            {
                if (this.Resources.MergedDictionaries != null)
                    this.Resources.MergedDictionaries.Clear();              
                this.Resources.Clear();
                this.Resources = null;
            }
            this.Template = null;
            //}));

            if (this.PropertyWindowTabs != null)
            {
                foreach (TabItem tabItem in this.PropertyWindowTabs)
                {
                    if (tabItem.Content is IDisposable)
                    {
                        (tabItem.Content as IDisposable).Dispose();
                    }
                    tabItem.Content = null;
                }
            }
            if (this.PropertyWindowTabs != null)
            {                
                this.PropertyWindowTabs.Clear();
                this.PropertyWindowTabs = null;
            }
            this.ClearValue(Chart.DockProperty);
            this.ClearValue(Chart.TabStopProperty);
           
            if (this.rd != null)
            {
                this.rd.MergedDictionaries.Clear();
                this.rd.Clear();
                this.rd = null;
            }
            if (this.baseRD != null)
            {
                this.baseRD.MergedDictionaries.Clear();
                this.baseRD.Clear();
                this.baseRD = null;
            }
            //if (SharedResourceDictionary._sharedDictionaries != null)
            //{
            //    var mer_dic = SharedResourceDictionary._sharedDictionaries.Keys.GetEnumerator();
            //    while (mer_dic.MoveNext())
            //    {
            //       // SharedResourceDictionary._sharedDictionaries[mer_dic.Current].Clear();
            //        for (int i = 0;
            //             i < SharedResourceDictionary._sharedDictionaries[mer_dic.Current].MergedDictionaries.Count;
            //             i++)
            //        {
            //            SharedResourceDictionary._sharedDictionaries[mer_dic.Current].MergedDictionaries[i].
            //                MergedDictionaries.Clear();
            //            SharedResourceDictionary._sharedDictionaries[mer_dic.Current].MergedDictionaries[i].Clear();
            //        }
            //        SharedResourceDictionary._sharedDictionaries[mer_dic.Current].MergedDictionaries.Clear();
            //      //  SharedResourceDictionary._sharedDictionaries[mer_dic.Current].Clear();

            //    }
            //    mer_dic.Dispose();
            //    SharedResourceDictionary._sharedDictionaries.Clear();
            //    SharedResourceDictionary._sharedDictionaries = null;
            //}
            Assembly systemAssembly = typeof(System.ComponentModel.Component).Assembly;

            Type reflectTypeDescriptionProviderType = systemAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");

            FieldInfo _propertyCacheInfo = reflectTypeDescriptionProviderType.GetField("_propertyCache", BindingFlags.Static | BindingFlags.NonPublic);

            Hashtable _propertyCache = (Hashtable)_propertyCacheInfo.GetValue(null);
            _propertyCache.Clear();
            GC.Collect(7, GCCollectionMode.Forced);
            GC.SuppressFinalize(this);
         
            #endregion
        }
        #endregion

        #region Events

        /// <summary>
        /// Occurs when [After window is opened]. 
        /// </summary>
        public event ChartPropertyWindowEventHandler ChartPropertyWindowOpened;
        /// <summary>
        /// Occurs when [Before window is opened]. Event can be cancelled.
        /// </summary>
        public event ChartPropertyWindowCancelEventHandler ChartPropertyWindowOpening;
        /// <summary>
        /// Occurs when [After window is closed].
        /// </summary>
        public event ChartPropertyWindowEventHandler ChartPropertyWindowClosed;

        /// <summary>
        /// Occurs when [Before window is closed]. Event can be cancelled.
        /// </summary>
        public event ChartPropertyWindowCancelEventHandler ChartPropertyWindowClosing;
        

        #endregion       

        #region Public methods

        //internal void HideToolBars()
        //{
        //    if (m_hiddenToolBarsCollection.Count == 0)
        //    {
        //        foreach (ChartToolBar toolBar in this.ToolBars)
        //        {
        //            if (toolBar.Visibility == Visibility.Visible)
        //            {
        //                m_hiddenToolBarsCollection.Add(toolBar);
        //                toolBar.Visibility = Visibility.Hidden;
        //            }
        //        }
        //    }
        //}
        //internal void ShowToolBars()
        //{
        //    foreach (ChartToolBar toolbar in m_hiddenToolBarsCollection)
        //    {
        //        toolbar.Visibility = Visibility.Visible;
        //    }

        //    m_hiddenToolBarsCollection.Clear();
        //}
        /// <summary>
        /// Hides the toolbars of the chart. Action is reverted by calling <see>
        ///                                                                    <cref>ShowToolBars</cref>
        ///                                                                </see>
        ///     .
        /// </summary>
        /// <summary>
        /// Shows the tool bars after <see>
        ///                               <cref>HideToolBars</cref>
        ///                           </see>
        ///     call.
        /// </summary>
        /// <summary>
        /// Sets the name of the legend.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="legendName">Name of the legend.</param>
        /// <exception cref="ArgumentNullException"><c>null</c> has been passed as series.</exception>
        public static void SetLegendName(ChartSeries series, string legendName)
        {
            if (series == null)
            {
                throw new ArgumentNullException("series");
            }

            series.SetValue(Chart.LegendNameProperty, legendName);
        }

        /// <summary>
        /// Gets the name of the legend.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Legend name.</returns>
        /// <exception cref="ArgumentNullException"><c>null</c> has been passed as parameter.</exception>
        public static string GetLegendName(ChartSeries series)
        {
            if (series == null)
            {
                throw new ArgumentNullException("series");
            }

            return (string)series.GetValue(Chart.LegendNameProperty);
        }

        /// <summary>
        /// Sets the dock of element of <see cref="ChartDockPanel"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dock">The dock value.</param>
        /// <exception cref="ArgumentNullException"><c>null</c> has been passed as parameter.</exception>
        public static void SetDock(UIElement element, ChartDock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(DockProperty, dock);
        }

        /// <summary>
        /// Gets the dock of <see cref="ChartDockPanel"/> child.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><see cref="ChartDock"/> value.</returns>
        /// <exception cref="ArgumentNullException"><c>null</c> has been passed as element.</exception>
        public static ChartDock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (ChartDock)element.GetValue(DockProperty);
        }

        /// <summary>
        /// Sets the alignment.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="alignment">The alignment.</param>
        /// <exception cref="ArgumentNullException"><c>null</c> has been passed as element.</exception>
        public static void SetAlignment(UIElement element, ChartAlignment alignment)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(AlignmentProperty, alignment);
        }

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Element's alignment.</returns>
        /// <exception cref="ArgumentNullException"><c>null</c> has been passed as parameter.</exception>
        public static ChartAlignment GetAlignment(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (ChartAlignment)element.GetValue(AlignmentProperty);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"></see>.
        /// </summary>
        /// <seealso cref="Chart"/>
        /// 
        internal ContentPresenter HeaderPresenter;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HeaderPresenter = this.GetTemplateChild("PART_HEADER") as ContentPresenter;
            m_dockPanel = GetTemplateChild(C_dockPanelName) as ChartDockPanel;

            if (m_dockPanel != null)
            {
                if (m_dockPanel.RootElement != null)
                {
                    Binding areasBinding = new Binding();
                    areasBinding.Path = new PropertyPath("Areas");
                    areasBinding.Source = this;
                    BindingOperations.SetBinding(m_dockPanel.RootElement, ItemsControl.ItemsSourceProperty, areasBinding);
                }
            }

            if (m_dockPanel != null)
            {
                foreach (ChartLegend legend in m_legends)
                {

                    if (m_legends.IndexOf(legend) == 0)
                    {
                        SetMarginforFirstLegend(legend);
                    }

                    if (legend.Parent != null)
                    {
                        (legend.Parent as ChartDockPanel).Children.Remove(legend);
                    }
                    m_dockPanel.Children.Add(legend);

                    ChartSeriesCollection collection = new ChartSeriesCollection();

                    foreach (ChartSeries item in m_internalSeriesList)
                    {
                        if (item.IsVisibleOnLegend == true && item.VisibilityOnLegend != Visibility.Collapsed)
                        {
                            collection.Add(item);
                        }
                    }
                    if (m_internalSeriesList.Count == 0)
                    {                      
                        foreach (var area1 in Areas)
                        {
                            if (area1 is SyncChartAreas)
                            {
                                foreach (ChartArea area in (area1 as SyncChartAreas).Areas)
                                {
                                    foreach (ChartSeries ser in area.Series)
                                    {
                                        if (ser.IsVisibleOnLegend == true && ser.VisibilityOnLegend != Visibility.Collapsed)
                                        {
                                            collection.Add(ser);
                                            m_internalSeriesList.Add(ser);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (ChartSeries ser in area1.Series)
                                {
                                    if (ser.IsVisibleOnLegend == true && ser.VisibilityOnLegend != Visibility.Collapsed)
                                    {
                                        collection.Add(ser);
                                    }
                                }
                            }
                        }                        
                    }
                    if (legend.Items.Count > 0)
                    {
                        if (legend.Items.CurrentItem != null)
                        {
                            ICollectionView view = (ICollectionView)CollectionViewSource.GetDefaultView(legend.ItemsSource);
                            if (view != null)
                            {
                                ChartSeriesCollection myCollection = ((view).SourceCollection) as ChartSeriesCollection;
                                myCollection.Clear();
                                if (legend != null)
                                    legend.ItemsSource = collection;

                            }
                        }
                    }
                    else
                    {
                        if (legend != null)
                            legend.ItemsSource = collection;
                    }

                }

                foreach (ChartToolBar toolBar in m_toolbars)
                {
                    if (toolBar.Parent != null)
                    {
                        if (toolBar.Parent is ChartDockPanel)
                        {
                            (toolBar.Parent as ChartDockPanel).Children.Remove(toolBar);
                        }
                    }
                    m_dockPanel.Children.Add(toolBar);
                }
            }
        }

        private PropertyWindow ChartPropertyWindow;
        private bool ChartPropertyWindowIsOpen = false;
        private void InitializePropertyWindow()
        {
            ChartPropertyWindow = new PropertyWindow();
            //Below Code opens the proeprty window from center of Chart control..
            Point chartPoint = this.PointToScreen(new Point());
            ChartPropertyWindow.Left = chartPoint.X + (this.ActualWidth - ChartPropertyWindow.Width) / 2;
            ChartPropertyWindow.Top = chartPoint.Y + (this.ActualHeight - ChartPropertyWindow.Height) / 2;      
            if (this.rootParent != null)
            {
                ChartPropertyWindow.Owner = this.rootParent as Window;
            }
            else
            {
                UIElement obj = this as UIElement;
                while (typeof(Window) != obj.GetType().BaseType && !obj.GetType().Name.ToString().Contains("Window"))
                {
                    obj = VisualTreeHelper.GetParent(obj) as UIElement;
                }
                ChartPropertyWindow.Owner = obj as Window;
            }
            ChartResourceWrapper wrapper = new ChartResourceWrapper();
            ChartPropertyWindow.Title = wrapper.ChartPropertiesDialogTitle;

            //Below code will Set the DataContext to the property window. This is done for each default tab, instead setting to entire window.            
            foreach (TabItem tabItem in GetDefaultPropertyDialogTabs())
            {
                ChartPropertyWindow.AddTab(tabItem);
                if (!(tabItem.Content is ChartAreaPropertiesView) && !(tabItem.Content is ChartPropertiesView) && !(tabItem.Content is ChartLegendPropertiesView) && !(tabItem.Content is ChartAxisPropertiesView) && !(tabItem.Content is ChartSeriesPropertiesView))
                {
                    tabItem.DataContext = this;
                }
            }           
            if (this.PropertyWindowTabs != null)
                ChartPropertyWindow.AddTab(this.PropertyWindowTabs);
            ChartPropertyWindow.SetChart(this);
        }

        /// <summary>
        /// Display Chart property dialog 
        /// </summary>    
        /// <seealso cref="Chart"/>
        public void ShowPropertyDialog()
        {
            if (this.ChartPropertyWindowIsOpen) { return; }
            InitializePropertyWindow();
            ChartPropertyWindow.Closing += new CancelEventHandler(_window_Closing);
            ChartPropertyWindow.Closed += new EventHandler(_window_Closed);

            ChartPropertyWindowCancelEventArgs OpeningArgs = new ChartPropertyWindowCancelEventArgs(ChartPropertyWindow);

            if (ChartPropertyWindowOpening != null)
                ChartPropertyWindowOpening(OpeningArgs);
            if (OpeningArgs.Cancel && OpeningArgs != null) 
            {
                return; 
            }
            this.ChartPropertyWindowIsOpen = true;
            ChartPropertyWindow.Show();

            ChartPropertyWindowEventArgs Openedargs = new ChartPropertyWindowEventArgs(ChartPropertyWindow);

            if (ChartPropertyWindowOpened != null)
                ChartPropertyWindowOpened(Openedargs);
            
        }

        void _window_Closed(object sender, EventArgs e)
        {
            ChartPropertyWindowEventArgs args = new ChartPropertyWindowEventArgs(sender as PropertyWindow);

            if (ChartPropertyWindowClosed != null)
                ChartPropertyWindowClosed(args);
            this.ChartPropertyWindowIsOpen = false;
            
        }

        void _window_Closing(object sender, CancelEventArgs e)
        {
            ChartPropertyWindowCancelEventArgs OpeningArgs = new ChartPropertyWindowCancelEventArgs(sender as PropertyWindow);

            if (ChartPropertyWindowClosing != null)
                ChartPropertyWindowClosing(OpeningArgs);
            if (OpeningArgs.Cancel && OpeningArgs != null)
            {
                e.Cancel = true;
            }                         
        }       
     
        //Below code is an public API close the property window from coding, uncomment it whenever required.
        ///// <summary>
        ///// Close the property Window if it is visible.
        ///// </summary>                
        //public void ClosePropertyDialog()
        //{
        //    if (this.ChartPropertyWindow != null && this.ChartPropertyWindow.IsVisible)
        //    {
        //        this.ChartPropertyWindow.Close();
        //    }
        //}

        /// <summary>
        /// Shows print dialog for a whole chart.
        /// </summary>
        /// <returns><c>true</c> if print dialog exited successfully.</returns>
        /// <seealso cref="Chart"/>
        public bool Print()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                return ShowPrintDialog(Rect.Empty);
            }

            return false;
        }

        /// <summary>
        /// Prints chart by the specified print area.
        /// </summary>
        /// <param name="printArea">The print area.</param>
        /// <returns><c>true</c> if print dialog exited successfully.</returns>
        /// <seealso cref="Chart"/>
        public bool Print(Rect printArea)
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                return ShowPrintDialog(printArea);
            }

            return false;
        }

        /// <summary>
        /// Switches to the printing mode.
        /// </summary>
        /// <remarks>
        /// Printing adorner is drawn on chart if this method is called.
        /// </remarks>
        /// <seealso cref="Chart"/>
        public void SwitchPrintingMode()
        {
            bool hasAdorner = false;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            Adorner[] adorners = adornerLayer.GetAdorners(this);

            if (adorners != null)
            {
                foreach (Adorner adorner in adorners)
                {
                    if (adorner is ChartPrintingAdorner)
                    {
                        adornerLayer.Remove(adorner);
                        hasAdorner = true;
                    }
                }
            }

            if (!hasAdorner)
            {
                adornerLayer.Add(new ChartPrintingAdorner(this));
                Keyboard.Focus(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event. 
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
           ChartCartesianAxisPanel v = FindAnchestor<ChartCartesianAxisPanel>((DependencyObject)e.OriginalSource);
                if (v != null && (e.Source is ChartArea))
                    e.Handled = true;
                base.OnMouseDoubleClick(e);
        }

        internal static T FindAnchestor<T>(DependencyObject current)
                where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                //SD15853-AnnotationLabel control of RichTextBox MouseDoubleClick throw exception
                if (current is Visual || current is Visual3D)
                {
                    current = VisualTreeHelper.GetParent(current);
                }
                else
                {
                    current = LogicalTreeHelper.GetParent(current);
                }
            }
            while (current != null);
            return null;
        }

        /// <summary>
        /// Saves chart to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The area rect.</param>
        /// <param name="encoder">The encoder.</param>
        /// <seealso cref="Chart"/>
        public void Save(Stream stream, Rect saveArea, BitmapEncoder encoder)
        {
            RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)saveArea.Width, (int)saveArea.Height, C_defaultDim, C_defaultDim, PixelFormats.Default);
            VisualBrush visualBrush = new VisualBrush(this);
            RenderOptions.SetCachingHint(visualBrush, CachingHint.Cache);
            Rectangle rect = new Rectangle();
            Rectangle backgroundRect = new Rectangle();

            backgroundRect.Fill = Brushes.White;
            backgroundRect.Arrange(new Rect(saveArea.Size));

            visualBrush.Stretch = Stretch.Fill;
            visualBrush.Viewbox = new Rect(saveArea.X / this.ActualWidth, saveArea.Y / this.ActualHeight, saveArea.Width / this.ActualWidth, saveArea.Height / this.ActualHeight);

            rect.Fill = visualBrush;
            rect.Stretch = Stretch.Fill;
            rect.Arrange(new Rect(saveArea.Size));

            bmpSource.Render(backgroundRect);
            bmpSource.Render(rect);

            encoder.Frames.Add(BitmapFrame.Create(bmpSource));
            encoder.Save(stream);
        }

        /// <summary>
        /// Saves chart to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The area rect.</param>
        /// <seealso cref="Chart"/>
        public void Save(Stream stream, Rect saveArea)
        {
            this.Save(stream, saveArea, new BmpBitmapEncoder());
        }

        /// <summary>
        /// Saves chart to chart to specified file.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="saveArea">The save area.</param>
        /// <seealso cref="Chart"/>
        public void Save(string fileName, Rect saveArea)
        {
            string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

            using (Stream stream = File.Create(fileName))
            {
                if (extension == "xps")
                {
                    this.SaveToXps(stream, saveArea);
                }
                else
                {
                    this.Save(stream, saveArea, Chart.CreateBitmapEncoderByExtension(extension));
                }
            }
        }

        /// <summary>
        /// Saves chart to specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="saveArea">The save area.</param>
        /// <param name="encoder">The encoder.</param>
        /// <seealso cref="Chart"/>
        public void Save(string fileName, Rect saveArea, BitmapEncoder encoder)
        {
            using (Stream stream = File.Create(fileName))
            {
                this.Save(stream, saveArea, encoder);
            }
        }

        /// <summary>
        /// Saves chart to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoder">The encoder.</param>
        /// <exception cref="ArgumentNullException">Internal border of chart control cannot be retrieved.</exception>
        /// <seealso cref="Chart"/>
        public void Save(Stream stream, BitmapEncoder encoder)
        {
            //Visual visual = Template.FindName("PART_INTERNAL_BORDER", this) as Visual;
            VisualBrush visual = new VisualBrush(this);
            //Included for SF4743 
            ChartToolBar tl = this.ToolBar;
            if (this.ShowToolBarOnPrintAndSave == false)
            {
                this.ToolBar = null;
            }
            if (visual != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, C_defaultDim, C_defaultDim, PixelFormats.Default);
                Rectangle backgroundRect = new Rectangle();
                Rectangle rect = new Rectangle();
                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(this.RenderSize));
                visual.ViewboxUnits = BrushMappingMode.Absolute;
                visual.Viewbox = new Rect(this.VisualOffset.X, this.VisualOffset.Y, this.RenderSize.Width, this.RenderSize.Height);
                rect.Fill = visual;
                rect.Arrange(new Rect(this.RenderSize));
                rect.Stretch = Stretch.Fill;
                bmpSource.Render(backgroundRect);
                bmpSource.Render(rect);
                encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                encoder.Save(stream);
                //Making the toolbar to visible when it is nullified
                this.ToolBar = tl;
            }
            else
            {
                throw new ArgumentNullException("Internal border of chart control cannot be retrieved.");
            }
        }

        /// <summary>
        /// Saves chart to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <seealso cref="Chart"/>
        public void Save(Stream stream)
        {
            this.Save(stream, new BmpBitmapEncoder());
        }

        /// <summary>
        /// Saves chart to the file with specified filename.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <seealso cref="Chart"/>
        public void Save(string fileName)
        {
            string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

            using (Stream stream = File.Create(fileName))
            {
                if (extension == ".xps")
                {
                    //// Get the size of the chart
                    Size size = new Size(this.ActualWidth, this.ActualHeight);
                    //// Measure and arrange elements
                    this.Measure(size);
                    this.Arrange(new Rect(size));
                    SaveToXps(stream, this);
                }
                else
                {
                    this.Save(stream, Chart.CreateBitmapEncoderByExtension(extension));
                }
            }
        }

        /// <summary>
        /// Saves chart to the file with specified filename using encoder.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="encoder">The encoder.</param>
        /// <seealso cref="Chart"/>
        public void Save(string fileName, BitmapEncoder encoder)
        {
            using (Stream stream = File.Create(fileName))
            {
                this.Save(stream, encoder);
            }
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="visual">The visual.</param>
        /// <seealso cref="Chart"/>
        public static void SaveToXps(Stream stream, Visual visual)
        {
            Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
            XpsDocument doc = new XpsDocument(package, CompressionOption.Normal);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            writer.Write(visual);

            doc.Close();
            package.Close();
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <seealso cref="Chart"/>
        public void SaveToXps(string filename)
        {
            using (Stream stream = File.Create(filename))
            {
                //// Get the size of the chart
                Size size = new Size(this.ActualWidth, this.ActualHeight);
                //// Measure and arrange elements
                this.Measure(size);
                this.Arrange(new Rect(size));

                SaveToXps(stream, this);
            }
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The save area.</param>
        /// <seealso cref="Chart"/>
        public void SaveToXps(Stream stream, Rect saveArea)
        {

            Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            VisualBrush visualBrush = new VisualBrush(this);
            RenderOptions.SetCachingHint(visualBrush, CachingHint.Cache);
            Rectangle rect = new Rectangle();

            visualBrush.Stretch = Stretch.Fill;
            visualBrush.Viewbox = new Rect(saveArea.X / this.ActualWidth, saveArea.Y / this.ActualHeight, saveArea.Width / this.ActualWidth, saveArea.Height / this.ActualHeight);

            rect.Fill = visualBrush;
            rect.Stretch = Stretch.Fill;
            rect.Arrange(new Rect(saveArea.Size));

            writer.Write(rect);

            doc.Close();
            package.Close();
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="saveArea">The save area.</param>
        /// <seealso cref="Chart"/>
        public void SaveToXps(string filename, Rect saveArea)
        {
            using (Stream stream = File.Create(filename))
            {
                this.SaveToXps(stream, saveArea);
            }
        }

        /// <summary>
        /// Copies chart to clipboard.
        /// </summary>
        /// <exception cref="ArgumentNullException">Internal border of chart control cannot be retrieved.</exception>
        /// <seealso cref="Chart"/>
        public void CopyToClipboard()
        {
            Visual visual = Template.FindName("PART_INTERNAL_BORDER", this) as Visual;

            if (visual != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, C_defaultDim, C_defaultDim, PixelFormats.Pbgra32);

                Rectangle backgroundRect = new Rectangle();

                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(this.RenderSize));

                bmpSource.Render(backgroundRect);

                bmpSource.Render(visual);

                Clipboard.SetImage(bmpSource);
            }
            else
            {
                throw new ArgumentNullException("visual");
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Called when PropertyWindowTabs collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        [System.ComponentModel.Description("Called when PropertyWindowTabs collection is changed.")]
        private void OnPropertyWindowTabsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (ChartPropertyWindow != null && PropertyWindowTabs != null)
                ChartPropertyWindow.AddTab(PropertyWindowTabs);
        }
        /// <summary>
        /// Called when legends collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        [System.ComponentModel.Description("Called when legends collection is changed.")]
        private void OnLegendsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {

            if (m_dockPanel != null)
            {
                if (args.OldItems != null)
                {
                    foreach (ChartLegend legend in args.OldItems)
                    {
                        m_dockPanel.Children.Remove(legend);
                        if (args.OldStartingIndex == 0 && Header != null)
                        {
                            m_dockPanel.Margin = new Thickness(m_dockPanel.Margin.Left, 0, m_dockPanel.Margin.Right, m_dockPanel.Margin.Bottom);
                        }
                        if (legend != null)
                        {
                            legend.ItemsSource = null;
                        }
                        isHeaderUpdated = false;
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Reset)
                {
                    ChartLegendsCollection tempcollection = new ChartLegendsCollection();
                    foreach (UIElement item in m_dockPanel.Children)
                    {
                        tempcollection.Add(item as ChartLegend);
                    }

                    if (tempcollection.Count > 0)
                    {
                        foreach (var item in tempcollection)
                        {
                            m_dockPanel.Children.Remove(item);
                        }

                    }
                }

                if (args.NewItems != null)
                {

                    foreach (ChartLegend legend in args.NewItems)
                    {
                        if (legend.Parent != null)
                        {
                            var chartDockPanel = legend.Parent as ChartDockPanel;
                            if (chartDockPanel != null)
                                chartDockPanel.Children.Remove(legend);
                        }

                        m_dockPanel.Children.Add(legend);

                        if (legend.Items.Count != 0) continue;
                        var collection = new ChartSeriesCollection();

                        foreach (var item in m_internalSeriesList.Where(item => item.IsVisibleOnLegend == true && item.VisibilityOnLegend != Visibility.Collapsed))
                        {
                            collection.Add(item);
                        }

                        if (collection.Count != 0)
                            legend.ItemsSource = collection;
                    }

                    if (this.Legends != null && this.Legends.Count == 1)
                    {
                        isHeaderUpdated = true;
                    }
                    else
                    {
                        isHeaderUpdated = false;
                    }
                    if (this.Legends != null && this.Legends.Count > 0)
                    {
                        SetMarginforFirstLegend(this.Legends[0]);
                    }
                }
            }

            ChartLegendsCollection coll = sender as ChartLegendsCollection;
            //SetLegendStyle();

        }

        private ResourceDictionary rd = null;
        //private void SetLegendStyle()
        //{
        //    if (this.Legends == null)
        //        return;
        //    foreach (ChartLegend legend in this.Legends)
        //    {
        //        //if (this.StyleIndexValue >= 48)
        //        //{
        //        //    if (rd == null)
        //        //    {
        //        //        string str = this.ChartVisualStyle == ChartStyles.None || this.ChartVisualStyle == ChartStyles.Default ? "Classic" : (this.ChartVisualStyle.ToString());
        //        //        rd = new ResourceDictionary()
        //        //        {
        //        //            Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/" + str + "Style.xaml", UriKind.RelativeOrAbsolute)
        //        //        };
        //        //    }
        //        //    object obj1 = Enum.GetName(typeof(ChartStyles), this.StyleIndexValue);
        //        //    object obj = obj1 + "ChartLegendStyle";

        //        //    legend.Style = rd[obj] as Style;

        //        //}
        //    }
        //}

        internal void SetLegendItemSource(ChartLegendsCollection legends)
        {
            foreach (ChartLegend legend in legends)
            {
                ChartSeriesCollection collection = new ChartSeriesCollection();

                foreach (ChartSeries item in m_internalSeriesList)
                {
                    if (item.IsVisibleOnLegend == true && item.VisibilityOnLegend != Visibility.Collapsed)
                    {
                        collection.Add(item);
                    }
                    if (item.IsVisibleOnLegend == false)
                    {
                        collection.Remove(item);
                    }
                }

                if (legend != null)
                    legend.ItemsSource = collection;
            }

        }
        private ChartAreasCollection m_temparea;
       
        private void OnSyncChartAreasChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                if (args.NewItems != null)
                {
                    foreach (ChartArea area in args.NewItems)
                    {
                        AddLogicalChild(area);
                        area.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSeriesChanged);
                    }
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {

                if (args.OldItems != null)
                {
                    foreach (ChartArea area in args.OldItems)
                    {
                        RemoveLogicalChild(area);
                        area.Series.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSeriesChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Called when areas collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnAreasChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                if (args.NewItems != null)
                {
                    m_temparea= new ChartAreasCollection ();
                    foreach (var areas in args.NewItems)
                    {
                        if (areas is SyncChartAreas)
                        {
                            SyncChartAreas area = areas as SyncChartAreas;
                                m_temparea.Add(area);
                                AddLogicalChild(area);
                                area.Areas.CollectionChanged +=
                                    new NotifyCollectionChangedEventHandler(OnSyncChartAreasChanged);
                        }
                        else
                        {
                                ChartArea area = areas as ChartArea;
                                m_temparea.Add(area);
                                AddLogicalChild(area);
                                area.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSeriesChanged);
                        }
                    }
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {

                if (args.OldItems != null)
                {
                    foreach (var areas in args.OldItems)
                    {
                        if (areas is SyncChartAreas)
                        {
                            SyncChartAreas area = areas as SyncChartAreas;
                            RemoveLogicalChild(area);
                            area.Areas.CollectionChanged -=
                                new NotifyCollectionChangedEventHandler(OnSyncChartAreasChanged);
                        }
                        else
                        {
                            ChartArea area = areas as ChartArea;
                            RemoveLogicalChild(area);
                            area.Series.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSeriesChanged);
                        }
                    }
                }
            }

            ChartAreasCollection collections = sender as ChartAreasCollection;
            SetAreasStyle(collections);

        }

        private void SetAreasStyle(ChartAreasCollection collections)
        {
            if (collections == null)
                return;
            foreach (ChartArea chartarea in collections)
            {
                //if (chartarea != null)
                //{
                //    //chartarea.defaultPalette = chartarea.ColorModel.Palette;

                //    SetAreaStyle(this, this.AreaStyle, chartarea);
                //}

                if (AreaStyle != null && chartarea != null)
                {
                    chartarea.Style = this.AreaStyle;
                    SetAreaStyle(this, this.AreaStyle, chartarea);
                }
                if (chartarea != null)
                {
                    chartarea.SetAxesStyle(chartarea.Axes);
                }
            }
        }

        /// <summary>
        /// Called when series collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                m_internalSeriesList.Clear();

            }
            else
            {
                if (args.NewItems != null)
                {
                    foreach (ChartSeries serObj in args.NewItems)
                    {
                        if (!m_internalSeriesList.Contains(serObj))
                        {
                            if (serObj.ActualXAxis != null && serObj.ActualXAxis.IsAutoSetRange == false)
                            {
                                serObj.IsIndexed = false;
                            }

                            m_internalSeriesList.Add(serObj);
                        }
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (ChartSeries serObj in args.OldItems)
                    {
                        m_internalSeriesList.Remove(serObj);
                    }
                }
            }

            if (this.m_legends.Count > 0)
            {
                foreach (ChartLegend legend in m_legends)
                {
                    if (LegendStyle != null)
                    {
                        legend.Style = LegendStyle;
                    }
                    if (m_legends.IndexOf(legend) == 0)
                    {
                        SetMarginforFirstLegend(legend);
                    }

                    // if (legend.Items.Count == 0)
                    {
                        ChartSeriesCollection collection = new ChartSeriesCollection();

                        foreach (ChartSeries item in m_internalSeriesList)
                        {
                            if (item.IsVisibleOnLegend == true && item.VisibilityOnLegend != Visibility.Collapsed)
                            {
                                collection.Add(item);
                            }
                            if (item.IsVisibleOnLegend == false)
                            {
                                collection.Remove(item);
                            }
                        }

                        if (legend != null)
                        {
                            if (collection.Count != 0)
                                legend.ItemsSource = collection;
                            else
                                legend.ItemsSource = null;
                        }
                    }

                }
            }
        }

        bool isHeaderUpdated = false;
        private void SetMarginforFirstLegend(ChartLegend legend)
        {
            if (this.Header != null)
            {
                double margin = headerFonSize;

                try
                {
                    if (Template != null)
                    {
                        Visual visual = Template.FindName("PART_HEADER", this) as Visual;
                        if (visual != null)
                        {
                            ContentPresenter presenter = visual as ContentPresenter;
                            if (presenter != null)
                            {
                                if (presenter.Content != null)
                                {
                                    if (!(presenter.Content is string))
                                    {
                                        if ((presenter.Content is TextBlock))
                                        {
                                            margin = (presenter.Content as TextBlock).FontSize + 8;
                                            headerFonSize = (presenter.Content as TextBlock).FontSize;
                                        }
                                    }

                                    if (m_dockPanel != null && isHeaderUpdated == true)
                                    {
                                        m_dockPanel.Margin = new Thickness(m_dockPanel.Margin.Left, m_dockPanel.Margin.Top, m_dockPanel.Margin.Right, m_dockPanel.Margin.Bottom);
                                    }
                                }
                            }
                        }
                        if (Header != null)
                        {
                            if ((Header is string) && (string)Header != string.Empty)
                            {
                                legend.Margin = new Thickness(legend.Margin.Left, 0, legend.Margin.Right, legend.Margin.Bottom);
                            }
                            else if (!(Header is string))
                            {
                                legend.Margin = new Thickness(legend.Margin.Left, 0, legend.Margin.Right, legend.Margin.Bottom);
                            }

                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == Chart.HeaderProperty)
            {
                if (Header != null)
                {
                    if (m_dockPanel != null && (((e.OldValue is string) && (string)e.OldValue == string.Empty) || e.OldValue == null || !(e.OldValue is string)))
                    {
                        m_dockPanel.Margin = new Thickness(m_dockPanel.Margin.Left, 0, m_dockPanel.Margin.Right, m_dockPanel.Margin.Bottom);
                    }

                    if (m_dockPanel != null && (((e.NewValue is string) && (string)e.NewValue == string.Empty) || e.NewValue == null))
                    {
                        m_dockPanel.Margin = new Thickness(m_dockPanel.Margin.Left, 0, m_dockPanel.Margin.Right, m_dockPanel.Margin.Bottom);
                    }
                }
            }
			// To invoke the Annotation Presenter's Measure Override when there is a change in the IntersectAction
            if (e.Property == Chart.AnnotationIntersectActionProperty)
            {
                if (this.Annot_Presenter != null)
                {
                    this.Annot_Presenter.ParentChart = this;                    
                    this.Annot_Presenter.InvalidateMeasure();
                }
            }
            //if (e.Property.ToString() == "VisualStyle")
            //{

            //    if (rd == null)
            //    {
            //    string str = this.ChartVisualStyle == ChartStyles.None || this.ChartVisualStyle == ChartStyles.Default ? "ClassicStyle" : (this.ChartVisualStyle.ToString()+"Style");
            //    rd = new ResourceDictionary()
            //            {
            //                Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/" + str + ".xaml", UriKind.RelativeOrAbsolute)
            //            };
            //    }

            //    //object obj = e.NewValue.ToString();

            //    switch (e.NewValue.ToString())
            //    {
            //        case "Default":
            //            ChartVisualStyle = ChartStyles.Default;
            //            break;

            //        case "Aero.NormalColor":
            //            ChartVisualStyle = ChartStyles.AeroNormalColor;
            //            break;

            //        case "Luna.Metallic":
            //            ChartVisualStyle = ChartStyles.LunaMetallic;
            //            break;

            //        case "Luna.NormalColor":
            //            ChartVisualStyle = ChartStyles.LunaNormalColor;
            //            break;

            //        case "Luna.Homestead":
            //            ChartVisualStyle = ChartStyles.LunaHomestead;
            //            break;

            //        case "Royale.NormalColor":
            //            ChartVisualStyle = ChartStyles.RoyaleNormalColor;
            //            break;

            //        case "Zune.NormalColor":
            //            ChartVisualStyle = ChartStyles.ZuneNormalColor;
            //            break;

            //        case "CoolBlue":
            //            ChartVisualStyle = ChartStyles.CoolBlue;
            //            break;

            //        case "BlueWave":
            //            ChartVisualStyle = ChartStyles.BlueWave;
            //            break;

            //        case "ChocolateYellow":
            //            ChartVisualStyle = ChartStyles.ChocolateYellow;
            //            break;

            //        case "SpringGreen":
            //            ChartVisualStyle = ChartStyles.SpringGreen;
            //            break;

            //        case "BrightGray":
            //            ChartVisualStyle = ChartStyles.BrightGray;
            //            break;

            //      case "ForestGreen":
            //            ChartVisualStyle = ChartStyles.ForestGreen;
            //            break;

            //        case "LawnGreen":
            //            ChartVisualStyle = ChartStyles.LawnGreen;
            //            break;

            //        case "MixedGreen":
            //            ChartVisualStyle = ChartStyles.MixedGreen;
            //            break;

            //        case "Office2003":
            //            ChartVisualStyle = ChartStyles.Office2003;
            //            break;

            //        case "Office2007Black":
            //            ChartVisualStyle = ChartStyles.Office2007Black;
            //            break;

            //        case "Office2007Blue":
            //            ChartVisualStyle = ChartStyles.Office2007Blue;
            //            break;

            //        case "Office2007Silver":
            //            ChartVisualStyle = ChartStyles.Office2007Silver;
            //            break;

            //        case "OrangeRed":
            //            ChartVisualStyle = ChartStyles.OrangeRed;
            //            break;

            //        case "Blend":
            //            ChartVisualStyle = ChartStyles.Blend;
            //            break;

            //        case "VS2010":
            //            ChartVisualStyle = ChartStyles.VS2010;
            //            break;  

            //        default:
            //            break;
            //    }

                //     this.InvalidateArrange();



            //}

            if (e.Property == ChartArea.DataContextProperty && e.OldValue != e.NewValue && this.Areas!=null)
            {
                foreach (var area in this.Areas)
                {
                    SyncChartAreas m_syncarea = area as SyncChartAreas;
                    if (m_syncarea != null)
                    {
                        foreach (ChartArea syncarea in m_syncarea.Areas)
                        {
                            syncarea.m_IsUpdateArea = false;
                        }
                    }
                    else
                    {
                        area.m_IsUpdateArea = false;
                    }
                }
            }

            base.OnPropertyChanged(e);


            if (e.Property == ChartArea.DataContextProperty && e.OldValue != e.NewValue && this.Areas!=null)
            {
                foreach (var area in this.Areas)
                {
                    SyncChartAreas m_syncarea = area as SyncChartAreas;
                    if (m_syncarea != null)
                    {
                        foreach (ChartArea syncarea in m_syncarea.Areas)
                        {
                            syncarea.m_IsUpdateArea = true;
                            syncarea.EndInit();
                        }
                    }
                    else
                    {
                        area.m_IsUpdateArea = true;
                        area.EndInit();
                    }
                }
            }
            if (e.Property == Chart.BackgroundProperty || e.Property == Chart.ForegroundProperty || e.Property == Chart.ChartVisualStyleProperty)
            {
                string backgrnd, foregrnd, cornerradius;
                if (this.Background == null)
                {
                    backgrnd = string.Empty;
                }
                else
                {
                    backgrnd = this.Background.ToString();
                }
                if (this.CornerRadius == null)
                {
                    cornerradius = string.Empty;
                }
                else
                {
                    cornerradius = this.CornerRadius.ToString();
                }
                if (this.Foreground == null)
                {
                    foregrnd = string.Empty;
                }
                else
                {
                    foregrnd = this.Foreground.ToString();
                }
                AutomationProperties.SetItemStatus(this, backgrnd + ";" + foregrnd + ";" + this.ChartVisualStyle.ToString() + ";");

            }
        }

        /// <summary>
        /// Called when toolBar collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnToolBarsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (m_dockPanel != null)
            {
                if (args.NewItems != null)
                {
                    foreach (ChartToolBar toolBar in args.NewItems)
                    {
                        if (toolBar != null)
                        {
                            ChartDockPanel.SetDock(toolBar, ChartDock.Top);
                        }
                    }
                }
            }
        }

        internal string chartstylename = null;
        internal int StyleIndexValue;
        private ResourceDictionary baseRD = null;
        /// <summary>
        /// Method for VisualStyleChanged
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            //Below line is commented to apply the default style to Chart when the ChartVisualStyle is changed from some other style to "None". 
            //if (args.NewValue.ToString() == "None")
            //{
            //    return;
            //}
            //else
            //{
                Chart chart = d as Chart;
                //if (!args.OldValue.ToString().Contains("Blend") && args.NewValue.ToString().Contains("Blend") && chart != null)
                //{
                //    SkinStorage.SetVisualStyle(chart, "Blend");
                //}
                //else
                //{
                //    SkinStorage.SetVisualStyle(chart, "Default");
                //}


                if (chart != null)
                {
                    chart.StyleIndexValue = (int)Enum.Parse(typeof(ChartStyles), args.NewValue.ToString());
                    //if (chart.StyleIndexValue >= 48)
                    //{
                    //    string strStyle = chart.ChartVisualStyle == ChartStyles.None || chart.ChartVisualStyle == ChartStyles.Default ? "Classic" : args.NewValue.ToString();
                    //    //this.Resources.Clear();
                    //    chart.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/" + strStyle + "Style.xaml", UriKind.RelativeOrAbsolute) });
                    //    //chart.SetStyle(args);

                    //    //if (chart.rd == null)
                    //    //{
                    //    //string strStyle = chart.ChartVisualStyle == ChartStyles.None || chart.ChartVisualStyle == ChartStyles.Default ? "Classic" : args.NewValue.ToString();
                    //    //chart.rd = new ResourceDictionary()
                    //    //{
                    //    //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/" + strStyle + "Style.xaml", UriKind.RelativeOrAbsolute)
                    //    //};
                    //    //}
                    //    //string str = args.NewValue.ToString() + "ChartStyle";
                    //    //Style style = chart.rd[str] as Style;
                    //    //chart.Style = style;
                    //    //SetToolBarStyle(chart);
                    //    //chart.SetLegendStyle();
                    //    //chart.AreaStyle = chart.rd[args.NewValue.ToString() + "ChartAreaStyle"] as Style;
                    //    //chart.SetAreasStyle(chart.Areas);
                    //}
                    //else
                    //{
                    if (chart.baseRD == null)
                    {
                       // chart.baseRD = ChartDictionaries.GenericBaseDictionary;
                        chart.baseRD = new SharedResourceDictionary()
                        {
                            Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/ChartVisualStyles.xaml", UriKind.RelativeOrAbsolute)
                        };
                    }
                    chart.chartstylename = args.NewValue.ToString();
                    Style style = chart.baseRD[chart.chartstylename] as Style;
                    //Style sty = new Style();
                    SetToolBarStyle(chart);
                    //chart.SetLegendStyle();
                    chart.Style = style;
                    //}
                }
            //}
        }

        /// <summary>
        /// Method for AreaStyleChanged
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnAreaStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            Style style = chart.AreaStyle;

            foreach (ChartArea area in chart.Areas)
            {
                //if (area is SyncChartAreas)
                //{
                //    if ((area as SyncChartAreas) != null)
                //    {
                //        Chart parent = area.Parent as Chart;
                //        foreach (ChartArea item in (area as SyncChartAreas).Areas)
                //        {
                //            if (item != null)
                //            {
                //                chart.SetAreasStyle(parent, style, item);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                chart.SetAreaStyle(chart, style, area);
                //}
            }
        }

        private void SetAreaStyle(Chart chart, Style style, ChartArea area)
        {
            if (area != null)
            {
                if (area is SyncChartAreas)
                {
                    if ((area as SyncChartAreas).Areas != null)
                    {
                        foreach (ChartArea item in (area as SyncChartAreas).Areas)
                        {
                            SetAreasStyle(chart, style, item);
                        }
                    }
                }
                else
                {
                    SetAreasStyle(chart, style, area);
                }
            }


        }

        private void SetAreasStyle(Chart chart, Style style, ChartArea area)
        {
            if (chart == null)
                return;
            if (area == null)
                return;

            if (area != null && !area.disableIsIndexedForOLAP)
            {

                if (rd != null )
                {
                    area.Style = style;
                    if (style != null && ((System.Windows.Setter)(style.Setters[0])).Property.ToString() == "ColorModel")
                    {
                        Brush[] brushes = ((Syncfusion.Windows.Chart.ChartStyleModel)((((System.Windows.Setter)(style.Setters[0]))).Value)).CurrentPalette;
                        area.ColorModel.ApplyPalette(brushes);
                    }
                    //area.SetHeaderStyle();
                }
            }

            if (area.Legend == null)
                return;

        }

        /// <summary>
        /// Method for LegendStyleChange
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnLegendStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            Style style = chart.LegendStyle;

            foreach (ChartLegend legend in chart.m_legends)
            {
                if (legend != null)
                {
                    legend.Style = style;
                }
            }
        }

		/// <summary>
		/// Method for ToolBarstyleChanged
		/// </summary>
		/// <param name="d"></param>
		/// <param name="args"></param>
		public static void OnToolBarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            Style toolBarStyle = chart.ToolBarStyle;

            foreach (ChartToolBar toolBar in chart.m_toolbars)
            {
                if (toolBar != null)
                {
                    toolBar.Style = toolBarStyle;
                }
            }
        }

        /// <summary>
        /// Called when ToolBar is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnToolBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            if (args.OldValue != null)
            {
                ChartToolBar toolBar = args.OldValue as ChartToolBar;
                foreach (ChartToolBar toolbar in chart.m_toolbars)
                {
                    if (toolbar.Parent != null)
                    {
                        if (toolbar.Parent is ChartDockPanel)
                        {
                            (toolbar.Parent as ChartDockPanel).Children.Remove(toolBar);
                        }
                    }
                    
                }
                chart.m_toolbars.Remove(toolBar);
            }

            if (args.NewValue != null)
            {
                ChartToolBar toolBar = args.NewValue as ChartToolBar;
                chart.m_toolbars.Add(toolBar);
                
                foreach (ChartToolBar toolbar in chart.m_toolbars)
                {
                    if (chart.m_dockPanel != null)
                    {
                        if (chart.m_dockPanel is ChartDockPanel)
                        {
                            chart.m_dockPanel.Children.Remove(toolbar);
                        }
                        chart.m_dockPanel.Children.Add(toolBar);
                    }                   
                }
            }
            SetToolBarStyle(chart);
        }

        private static void SetToolBarStyle(Chart chart)
        {
            if (chart == null)
                return;
            if (chart.ToolBars == null)
                return;

            if (chart.rd == null)
            {
                chart.rd = ChartDictionaries.GenericBaseDictionary;
               // string str = chart.ChartVisualStyle == ChartStyles.None || chart.ChartVisualStyle == ChartStyles.Default ? "Classic" : chart.ChartVisualStyle.ToString();
                //chart.rd = new SharedResourceDictionary()
                //{
                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Chart.Templates.xaml", UriKind.RelativeOrAbsolute)
                //};
            }

            foreach (ChartToolBar bar in chart.ToolBars)
            {
                if (chart.StyleIndexValue >= 48)
                {

                    object obj1 = Enum.GetName(typeof(ChartStyles), chart.StyleIndexValue);

                    object obj = obj1 + "ChartToolBarStyle";

                    bar.Style = chart.rd[obj] as Style;
                    foreach (ToolBarItem item in bar.Items)
                    {
                        object obj2 = obj1 + "ToolBarItemStyle";
                        item.Style = chart.rd[obj2] as Style;
                    }
                }
                else
                {
                    bar.Style = chart.rd["DefaultToolBar"] as Style;
                    foreach (ToolBarItem item in bar.Items)
                    {
                        item.Style = chart.rd["DefaultToolBarItem"] as Style;
                    }
                }

            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.Print"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnPrintCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;

            if (chart != null)
            {
                if (args.Parameter is Rect)
                {
                    chart.Print((Rect)args.Parameter);
                }
                else
                {
                    chart.Print();
                }
            }
        }

        /// <summary>
        /// Handles the <see>
        ///                 <cref>ApplicationCommands.AddAnnotation</cref>
        ///             </see>
        ///     routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnAddAnnotationCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;

            if (chart!= null)
            {
                ChartAnnotationLabel label = new ChartAnnotationLabel();
                label.AnnotationShape = AnnotationShapes.Circle;
                label.Fill = Brushes.Black;
                label.OffsetX = chart.AnnotationPoint.X;
                label.OffsetY = chart.AnnotationPoint.Y;
                label.Content = new TextBox();
                chart.AnnotationLabels.Add(label);
                if (chart.Annot_Presenter != null)
                {
                    if (chart.Annot_Presenter.ParentChart == null)
                    {
                        chart.Annot_Presenter.ParentChart = chart;
                    }
                    chart.Annot_Presenter.InvalidateMeasure();
                }
            }
        }
        /// <summary>
        /// Handles the <see cref="ChartCommands.SwitchPrinting"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSwitchPrintingCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;

            if (chart != null)
            {
                chart.SwitchPrintingMode();
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.Save"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        /// <seealso cref="Chart"/>
        private static void OnSaveCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;

            if (chart != null)
            {
                if (args.Parameter is string)
                {
                    chart.Save((string)args.Parameter);
                }
                else if (args.Parameter is Stream)
                {
                    chart.Save((Stream)args.Parameter);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = C_imageFilesFilter;

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        chart.Save(saveFileDialog.FileName);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.Copy"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCopyCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;

            if (chart != null)
            {
                chart.CopyToClipboard();
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.Close"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCloseCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;
            if (chart != null)
            {
                chart.CloseToolBar();
            }
        }

        /// <summary>
        /// Close the Toolbar in chart
        /// </summary>
        public void CloseToolBar()
        {           
            if (this.m_dockPanel != null)
            {
                this.m_dockPanel.Children.Remove(this.ToolBar);
            }
        }

        /// <summary>
        /// Method for Showing the ToolBar
        /// </summary>
        public void ShowToolBar()
        {
            if (m_dockPanel != null)
            {
                foreach (ChartToolBar toolBar in m_toolbars)
                {
                    if (toolBar.Parent != null)
                    {
                        if (toolBar.Parent is ChartDockPanel)
                        {
                            (toolBar.Parent as ChartDockPanel).Children.Remove(toolBar);
                        }
                    }
                    m_dockPanel.Children.Add(toolBar);
                }
            }
        }

        /// <summary>
        /// Called when IsTabStop property is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabStopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart instance = (Chart)d;
            if (instance.ToolBar != null)
            {
                ChartToolBar toolbar = instance.ToolBar;
                toolbar.IsTabStop = instance.IsTabStop;
                if(toolbar.CloseButton!=null)
                toolbar.CloseButton.IsTabStop = false;
                foreach (ToolBarItem tbi in toolbar.Items)
                {
                    tbi.IsTabStop = instance.IsTabStop;
                }
            }
        }

        /// <summary>
        /// Called when legend dock is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLegendDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLegend legend = d as ChartLegend;

            if (legend != null)
            {
                switch (ChartDockPanel.GetDock(legend))
                {
                    case ChartDock.Right:
                    case ChartDock.Left:
                        if (!legend.OrientationSealedOnDock)
                        {
                            legend.Orientation = Orientation.Vertical;
                        }

                        break;
                    case ChartDock.Top:
                        {
                            legend.Orientation = Orientation.Horizontal;
                            var obj = VisualTreeHelper.GetParent(legend);
                            var panel = obj as ChartDockPanel;
                            if (panel != null && (panel.Children.Count > 2))
                            {
                                var chartDockPanel = obj as ChartDockPanel;
                                chartDockPanel.Children.Remove(legend);
                                var dockPanel = obj as ChartDockPanel;
                                dockPanel.Children.Insert(dockPanel.Children.Count - 1, legend);
                            }
                        }
                        break;
                    case ChartDock.Bottom:
                        if (!legend.OrientationSealedOnDock)
                        {
                            legend.Orientation = Orientation.Horizontal;
                            var obj = VisualTreeHelper.GetParent(legend);
                            var chartDockPanel = obj as ChartDockPanel;
                            if (chartDockPanel != null && (chartDockPanel.Children.Count > 2))
                            {
                                (obj as ChartDockPanel).Children.Remove(legend);
                                (obj as ChartDockPanel).Children.Insert((obj as ChartDockPanel).Children.Count - 2, legend);
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Corces ToolBar property.
        /// </summary>
        /// <param name="dObj">The DependencyObject dObj.</param>
        /// <param name="value">The value.</param>
        /// <returns>The ChartToolBar</returns>
        private static object OnCoerceToolBar(DependencyObject dObj, object value)
        {
            return value;
        }

        /// <summary>
        /// Called when ToolBar dock is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>  
        private static void OnToolBarDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartToolBar toolBar = d as ChartToolBar;
            if (toolBar != null)
            {
                switch (ChartDockPanel.GetDock(toolBar))
                {
                    case ChartDock.Right:
                    case ChartDock.Left:
                        toolBar.ItemsOrientation = Orientation.Vertical;
                        break;
                    case ChartDock.Top:
                    case ChartDock.Bottom:
                        toolBar.ItemsOrientation = Orientation.Horizontal;
                        break;
                }
            }
        }

        /// <summary>
        /// Creates the new instance of the <see cref="BitmapEncoder"/> class by extension of file.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The BitmapEncoder</returns>
        public static BitmapEncoder CreateBitmapEncoderByExtension(string extension)
        {
            BitmapEncoder encoder = null;

            switch (extension)
            {
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case ".wdp":
                    encoder = new WmpBitmapEncoder();
                    break;

                default:
                    encoder = new BmpBitmapEncoder();
                    break;
            }

            return encoder;
        }

        /// <summary>
        /// Determines whether passed corner radius is valid.
        /// </summary>
        /// <param name="value">The corner radius instance.</param>
        /// <returns>
        /// <c>true</c> if corner radius is valid; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCornerRadiusValid(object value)
        {
            CornerRadius radius = (CornerRadius)value;
            if (radius != null)
            {
                if ((radius.TopLeft < 0.0) || (radius.TopRight < 0.0) || (radius.BottomLeft < 0.0) || (radius.BottomRight < 0.0))
                {
                    return false;
                }

                if (Double.IsPositiveInfinity(radius.TopLeft) || Double.IsPositiveInfinity(radius.TopRight) || (Double.IsPositiveInfinity(radius.BottomLeft) || Double.IsPositiveInfinity(radius.BottomRight)))
                {
                    return false;
                }

                return (!double.IsNegativeInfinity(radius.TopLeft) && !double.IsNegativeInfinity(radius.TopRight)) && (!double.IsNegativeInfinity(radius.BottomLeft) && !double.IsNegativeInfinity(radius.BottomRight));
            }

            return false;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ////Removes printing adorner when Esc key is pressed
                Adorner[] adorners = AdornerLayer.GetAdornerLayer(this).GetAdorners(this);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner is ChartPrintingAdorner)
                        {
                            SwitchPrintingMode();
                        }
                    }
                }
                ////Removes printing adorner when Esc key is pressed
                foreach (ChartArea area in Areas)
                {
                    if (area.ZoomSwitched)
                    {
                        ChartAreaCommands.CancelZooming.Execute(null, area);
                        ChartAreaCommands.SwitchZooming.Execute(null, area);
                    }
                }
                ////Closes any popup from ToolBar
                if (this.ToolBar != null)
                {
                    ChartToolBar chartToolBar = this.ToolBar;
                    foreach (ToolBarItem item in chartToolBar.Items)
                    {
                        item.IsOpen = false;
                        if (item.Items.Count > 0)
                        {
                            Keyboard.Focus(item);
                        }
                    }
                }

                foreach (ChartArea area in this.Areas)
                {
                    if (area.DragSegment != null && area.DragSegment.Series != null && area.DragSegment.Series.Segments != null)
                    {
                        int index = area.DragSegment.Series.Segments.IndexOf(area.DragSegment);
                        if (area.DragSegment.Series.Presenter != null && area.DragSegment.Series.Segments.Count > index)
                        {
                            DependencyObject obj = VisualTreeHelper.GetChild(area.DragSegment.Series.Presenter, index);
                            obj = VisualTreeHelper.GetChild(obj, 0);
                            if (obj is Canvas)
                            {
                                Canvas canvas = obj as Canvas;
                                if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                                {
                                    Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                    popup.IsOpen = false;
                                    area.DragSegment = null;
                                }
                            }
                        }

                    }
                    area.Cursor = Cursors.Arrow;
                    area.OnSegmentDropped(new SegmentDropEventArgs(area.DragSegment));
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
			//Sets the value for the AnnotationPoint, to add the Annotation using the ContextMenu
            if (e.ChangedButton.ToString() == "Right")
            {
                this.AnnotationPoint = e.GetPosition(this);
            }
            base.OnMouseDown(e);
            Keyboard.Focus(this);
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            return ChartLayoutUtils.CheckSize(constraint);
        }

        /// <summary>
        /// Shows the print dialog.
        /// </summary>
        /// <param name="printArea">The print area.</param>
        /// <returns>True to show the Printdialog</returns>
        private bool ShowPrintDialog(Rect printArea)
        {
            ChartPrintDialog printDialog = new ChartPrintDialog();
            bool retValue = (bool)printDialog.ShowPrintDialog(this, printArea,this.Height, this.Width);
            Keyboard.Focus(this);
            return retValue;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            FontFamilyDescriptor.RemoveValueChanged(this, new EventHandler(FontFamilyChanged));
            FontSizeDescriptor.RemoveValueChanged(this, new EventHandler(FontSizeChanged));
            FontWeightDescriptor.RemoveValueChanged(this, new EventHandler(FontWeightChanged));
            // Checking if this thread has access to the object.
            if (this.Dispatcher.CheckAccess())
            {
                this.DisposeChart();
            }
            else
            {
                // This thread does not have access to the UI thread.
                // Place the dispose method on the Dispatcher of the UI thread.
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)(() =>
                    {
                		this.DisposeChart();
                    }));
            }
           GC.SuppressFinalize(this);
        }

        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Return String value from the Serialization method.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            EditorHelper.Register<MultiBindingExpression, BindingConvertor>();
            EditorHelper.Register<BindingExpression, BindingConvertor>();
            //TypeDescriptor.AddProvider(new BindingTypeDescriptionProvider(), typeof(System.Windows.Data.Binding));
            //string _xamlString;
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            //_xamlString = outstr.ToString();
            //if (outstr.ToString().Contains("</Chart>"))
            //{
            //    outstr = outstr.Remove(outstr.Length - 9, 9);
            //}
            //else
            //{
            //    outstr = outstr.Remove(outstr.Length - 2, 1);
            //}
            
            #region Areas

            if (this.Areas != null)
            {

                StringBuilder _areasString = new StringBuilder("<" + this.GetType().Name + ".Areas>");
                foreach (var _chartArea in this.Areas)
                {
                    if (_chartArea.GetType() == typeof(SyncChartAreas))
                    {
                        SyncChartAreas _areas = _chartArea as SyncChartAreas;
                        _areasString.Append(_areas.Serialize());
                    }
                    else
                    {
                        ChartArea _areas = _chartArea as ChartArea;
                        _areasString.Append(_areas.Serialize());
                    }
                }
                _areasString.Append("</" + this.GetType().Name + ".Areas>");
                 outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<" + this.GetType().Name + ".Areas>", "</" + this.GetType().Name + ".Areas>"), _areasString.ToString());
            }
           
            #endregion                     

            #region ToolBar
            if (this.ToolBar != null)
            {
                StringBuilder _toolBars = new StringBuilder("<Chart.ToolBar>");              
                _toolBars.Append(this.ToolBar.Serialize());
                _toolBars.Append("</Chart.ToolBar>");
                //_toolBars = Removexmlns(_toolBars);
                outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<Chart.ToolBar>", "</Chart.ToolBar>"), _toolBars.ToString());
            }
            #endregion        

            //outstr = outstr.Append("</Chart>");
     
            return (outstr.ToString());
        }


        /// <summary>
        /// Return the substring
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="_from"></param>
        /// <param name="_to"></param>
        /// <returns></returns>
        public static string SubString(string actual, string _from, string _to) 
        {
            int start = actual.IndexOf(_from);
            int end = actual.IndexOf(_to,(start + _from.Length)) + new StringBuilder(_to).Length;
            string temp = actual.Substring(start, (end - start));
            return temp;
        }

        private StringBuilder Removexmlns(StringBuilder xamlstring)
        {
            string xmlns = "xmlns=\"http://schemas.syncfusion.com/wpf\"";
            xamlstring =  xamlstring.Replace(xmlns, string.Empty);
            xmlns = "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"";
            xamlstring = xamlstring.Replace(xmlns, string.Empty);
            xmlns = "xmlns:av=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"";
            return xamlstring.Replace(xmlns, string.Empty);
        }

        private string Parse(string xamlstring)
        {
            StringBuilder sb = new StringBuilder();
            if (this.AnnotationLabels == null)
            {
               
            }

            else
            {
            }
            return xamlstring;
        }


        /// <summary>
        /// Return the XAML code
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }

    /// <summary>
    /// Represents the Interface as IChartInterface
    /// </summary>
    public interface IChartInterface
    {
        /// <summary>
        /// CLR property of BackGround.
        /// </summary>
        Brush Background { get; }
    }
}
