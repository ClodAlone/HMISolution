// <copyright file="ChartLegend.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Syncfusion.Licensing;
    using Syncfusion.Windows.Chart;
    using System.Text;
    using System.Xml;
    using System.Windows.Automation.Peers;
    using System.Collections.Generic;
    using System.Windows.Automation;

    /// <summary>
    /// Represents WPF chart legend class.
    /// </summary>
    /// <remarks>
    /// Chart legend can be added either as chart's child or as <see cref="ChartArea"
    /// />'s child. <para /> Per legend's default template, it represents <see
    /// cref="ChartSeries" /> except of cases when series' type is set to one of
    /// following <see cref="ChartTypes" />: <see cref="ChartTypes.Pie" />, <see
    /// cref="ChartTypes.Doughnut" />, <see cref="ChartTypes.Pyramid" /> and <see
    /// cref="ChartTypes.Funnel" />. For such chart types legend represents <see
    /// cref="ChartSegment" />. <para /> As either child of <see cref="Chart" /> or <see
    /// cref="ChartArea" />, legend has all abilities: such as docking and enabling or
    /// disabling corresponding series (segments).
    /// </remarks>
    /// <example>
    /// C#: <code language="C#">
    /// //Window's constructor.
    /// public Window1()
    /// {
    /// InitializeComponent();
    /// //Creating a new chart instance.
    /// Chart chart = new Chart();
    /// //Creating a chart legend.
    /// ChartLegend legend = new ChartLegend();
    /// //Setting legend's attached ChartDock property.
    /// ChartDockPanel.SetDock(legend, ChartDock.Left);
    /// //Adding legend to chart.
    /// chart.Legends.Add(legend);
    /// //Adding 2 areas on chart.
    /// chart.Areas.Add(new ChartArea());
    /// chart.Areas.Add(new ChartArea());
    /// //Assigning second area's Legend property.
    /// chart.Areas[1].Legend = new ChartLegend();
    /// ChartDockPanel.SetDock(chart.Areas[1].Legend, ChartDock.Right);
    /// //Creating series reference.
    /// ChartSeries series;
    /// //Creating collection of chart points.
    /// ChartListData data = new ChartListData();
    /// //Adding 3 series with have 2 points for each to first chart area.
    /// for (int i=1;i&lt;=6;i++)
    /// {
    /// //Adding point with X == i and Y == i;
    /// data.Add(new ChartPoint(i, i));
    /// if (i % 2 == 0)
    /// {
    /// //Creating a new chart series.
    /// series = new ChartSeries(ChartTypes.Area);
    /// //Setting series' label;
    /// series.Label = "Series " + i/2;
    /// //Setting datapoints.
    /// series.Data = data;
    /// //Adding series to area.
    /// chart.Areas[0].Series.Add(series);
    /// //Creating new data points.
    /// data = new ChartListData();
    /// }
    /// }
    /// //Creating new data points.
    /// data = new ChartListData();
    /// //Creating a new series type of Pie.
    /// series = new ChartSeries(ChartTypes.Pie);
    /// //Setting Label property as representation of series on legend.
    /// series.Label = "Series 4";
    /// //Adding 9 data points.
    /// for (int i = 1; i &lt; 10; i++)
    /// {
    /// data.Add(new ChartPoint(i,i));
    /// }
    /// //Assigning data to Pie series.
    /// series.Data = data;
    /// //Adding Pie series to area.
    /// chart.Areas[1].Series.Add(series);
    /// //Setting window's content.
    /// this.Content = chart;
    /// }
    /// </code> XAML: <code language="XAML">
    /// &lt;Window
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// Title="Legends sample" Height="500" Width="500"
    /// WindowStartupLocation="CenterScreen"&gt;
    /// &lt;syncfusion:Chart
    /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
    /// &lt;syncfusion:Chart.Legends&gt;
    /// &lt;syncfusion:ChartLegend syncfusion:ChartDockPanel.Dock="Left"/&gt;
    /// &lt;/syncfusion:Chart.Legends&gt;
    /// &lt;syncfusion:Chart.Areas&gt;
    /// &lt;syncfusion:ChartArea&gt;
    /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 1 2 2" Type="Area"/&gt;
    /// &lt;syncfusion:ChartSeries Label="Series 2" Data="3 3 4 4" Type="Area"/&gt;
    /// &lt;syncfusion:ChartSeries Label="Series 3" Data="5 5 6 6" Type="Area"/&gt;
    /// &lt;/syncfusion:ChartArea&gt;
    /// &lt;syncfusion:ChartArea&gt;
    /// &lt;syncfusion:ChartArea.Legend&gt;
    /// &lt;syncfusion:ChartLegend syncfusion:ChartDockPanel.Dock="Right"/&gt;
    /// &lt;/syncfusion:ChartArea.Legend&gt;
    /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 1 2 2 3 3 4 4 5 5 6 6 7 7 8
    /// 8 9 9" Type="Pie"/&gt;
    /// &lt;/syncfusion:ChartArea&gt;
    /// &lt;/syncfusion:Chart.Areas&gt;
    /// &lt;/syncfusion:Chart&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// </example>
    /// <seealso cref="ChartArea">ChartArea</seealso>
    /// <seealso cref="ChartSeries">ChartSeries</seealso>
    /// <seealso cref="ChartDockPanel">ChartDockPanel</seealso>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartLegend : HeaderedItemsControl, IChartSerializer
    {
        #region Coded UI
        private class ChartLegendPeer : FrameworkElementAutomationPeer
        {

            public ChartLegendPeer(ChartLegend control)
                : base(control)
            {

            }

            #region AutomationPeer overrides
            protected override string GetClassNameCore()
            {
                return "ChartLegend";
            }  

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Custom;
            }             
           
                      
            public override object GetPattern(PatternInterface patternInterface)
            {                                          
               return this;                
            }

            #endregion

           
           

            private ChartLegend MyOwner
            {
                get
                {
                    return (ChartLegend)base.Owner;
                }
            }

        }


        
        #endregion

        /// <summary>
        /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartLegendPeer(this);
        }
        #region DependencyProperties


        
        /// <summary>
        /// Identifies the OrientationSealedOnDock dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationSealedOnDockProperty =
            DependencyProperty.Register("OrientationSealedOnDock", typeof(bool), typeof(ChartLegend), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the RowsCount dependency property.
        /// </summary>
        public static DependencyProperty RowsCountProperty =
          DependencyProperty.Register("RowsCount", typeof(int), typeof(ChartLegend), new PropertyMetadata(1));

        /// <summary>
        /// Identifies the ColumnsCount dependency property.
        /// </summary>
        public static DependencyProperty ColumnsCountProperty =
          DependencyProperty.Register("ColumnsCount", typeof(int), typeof(ChartLegend), new PropertyMetadata(1));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartLegend), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(ChartLegend));

        /// <summary>
        /// Identifies the CheckBoxVisibility dependency property.
        /// </summary>
        public static DependencyProperty CheckBoxVisibilityProperty =
          DependencyProperty.RegisterAttached("CheckBoxVisibility", typeof(Visibility), typeof(ChartLegend), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the IconVisibility dependency property.
        /// </summary>
        public static DependencyProperty IconVisibilityProperty =
          DependencyProperty.RegisterAttached("IconVisibility", typeof(Visibility), typeof(ChartLegend), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the IconWidth dependency property.
        /// </summary>
        public static DependencyProperty IconWidthProperty =
          DependencyProperty.Register("IconWidth", typeof(double), typeof(ChartLegend), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the IconHeight dependency property.
        /// </summary>
        public static DependencyProperty IconHeightProperty =
          DependencyProperty.Register("IconHeight", typeof(double), typeof(ChartLegend), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the ElementMargin dependency property.
        /// </summary>
        public static DependencyProperty ElementMarginProperty =
          DependencyProperty.RegisterAttached("ElementMargin", typeof(Thickness), typeof(ChartLegend), new FrameworkPropertyMetadata(new Thickness(2), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the TextAlignment dependency property.
        /// </summary>
        public static DependencyProperty TextAlignmentProperty =
          DependencyProperty.RegisterAttached("TextAlignment", typeof(VerticalAlignment), typeof(ChartLegend), new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.Inherits));

		/// <summary>
        /// Identifies the LegacyLegendStyleEnabled dependency property
        /// </summary>
        public static readonly DependencyProperty IsLegacyLegendStyleEnabledProperty =
           DependencyProperty.Register("IsLegacyLegendStyleEnabled", typeof(bool), typeof(ChartLegend), new PropertyMetadata(true));

		/// <summary>
        /// Identifies the ItemMargin dependency property.
        /// </summary>
        public static DependencyProperty ItemMarginProperty =
          DependencyProperty.RegisterAttached("ItemMargin", typeof(Thickness), typeof(ChartLegend), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the ShowSymbol dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSymbolProperty =
        DependencyProperty.Register("ShowSymbol", typeof(Visibility), typeof(ChartLegend), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the HorizontalContentAlignment dependency property.
        /// </summary>
        public new static readonly DependencyProperty HorizontalContentAlignmentProperty =
             DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ChartLegend), new PropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Identifies the VerticalContentAlignment dependency property.
        /// </summary>
        public new static readonly DependencyProperty VerticalContentAlignmentProperty =
             DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(ChartLegend), new PropertyMetadata(VerticalAlignment.Stretch));


        /// <summary>
        /// Gets or sets the HorizontalContentAlignment.
        /// </summary>
        /// <value>Horizontal Alignement</value>
        public new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets VerticalContentAlignement.
        /// </summary>
        /// <value>ertical Alignment</value>
        public new VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }


        /// <summary>
        /// Get or Set LegendPanelProperty
        /// </summary>
        public LegendPanelTypes LegendPanel
        {
            get { return (LegendPanelTypes)GetValue(LegendPanelProperty); }
            set { SetValue(LegendPanelProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LegendPanel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LegendPanelProperty =
            DependencyProperty.Register("LegendPanel", typeof(LegendPanelTypes), typeof(ChartLegend), new UIPropertyMetadata(LegendPanelTypes.Grid, OnPanelChanged));


        /// <summary>
        /// Method implementation for Panel assigned to legend
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legend = d as ChartLegend;
            if (legend != null)
            {
                legend.SetItemsPanel();
            }

        }

        private void SetItemsPanel()
        {
            if (LegendPanel == LegendPanelTypes.Grid)
            {
                ResourceDictionary rd = ChartDictionaries.GenericLegendDictionary;
                //ResourceDictionary rd = new SharedResourceDictionary()
                //{
                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Legend.xaml", UriKind.RelativeOrAbsolute)
                //};
                this.ItemsPanel = rd["chartGridPanel"] as ItemsPanelTemplate;
            }
            else if (LegendPanel == LegendPanelTypes.WrapPanel)
            {
                this.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(UniformWrapPanel)));
            }
            else
            {
                this.ItemsPanel = this.LegendItemsPanel;
            }

        }

        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
         DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartLegend), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnOffsetXChanged)));

      
       
        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
        DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartLegend), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnOffsetYChanged)));

        /// <summary>
        /// Identifies the IsSegmentsLegend dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSegmentsLegendProperty =
        DependencyProperty.Register("IsSegmentsLegend", typeof(bool), typeof(ChartLegend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnValueChnged)));

       
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_highlightedElement
        /// </summary>
        internal ChartSeries m_highlightedElement;
        internal ChartArea m_area;
        internal bool m_isFontFamilySet = false;
        internal bool m_isFontSizeSet = false;
        internal bool m_isFontWeightSet = false;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value whether the LegacyLegendStyle remains unchanged. This is a dependency property.
        /// </summary>
        public bool IsLegacyLegendStyleEnabled
        {
            get { return (bool)GetValue(IsLegacyLegendStyleEnabledProperty); }
            set { SetValue(IsLegacyLegendStyleEnabledProperty, value); }
        }

		/// <summary>
        /// Gets or sets the legend items' margin. This is a dependency property.
        /// </summary>
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)this.GetValue(ChartLegend.ItemMarginProperty);
            }

            set
            {
                this.SetValue(ChartLegend.ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether legend's orientation remains unchanged
        /// during dock. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// By default, legend's items orientation will change with respect to legend's <see
        /// cref="ChartDockPanel">dock</see> attached property. For legend's <see
        /// cref="ChartDock.Top" /> or <see cref="ChartDock.Bottom" /> dock state, items
        /// will be placed horizontally, for <see cref="ChartDock.Right" /> or <see
        /// cref="ChartDock.Left" /> - vertically.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="300" Width="300" WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend OrientationSealedOnDock="True"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartDockPanel">ChartDockPanel</seealso>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public bool OrientationSealedOnDock
        {
            get { return (bool)GetValue(OrientationSealedOnDockProperty); }
            set { SetValue(OrientationSealedOnDockProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of rows to be used for items in the legend. This is a dependency property.
        /// </summary>
        /// <value>The rows count.</value>
        /// <example>
        /// XAML:
        /// <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="500" Width="500" WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        ///    &lt;syncfusion:Chart.Legends&gt;
        ///        &lt;syncfusion:ChartLegend RowsCount="2" ColumnsCount="3"/&gt;
        ///    &lt;/syncfusion:Chart.Legends&gt;
        ///    &lt;syncfusion:Chart.Areas&gt;
        ///        &lt;syncfusion:ChartArea&gt;
        ///            &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        ///            &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        ///            &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        ///            &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        ///            &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        ///            &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        ///        &lt;/syncfusion:ChartArea&gt;
        ///    &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea"/>
        /// <seealso cref="ChartSeries"/>
        public int RowsCount
        {
            get
            {
                return (int)this.GetValue(ChartLegend.RowsCountProperty);
            }

            set
            {
                this.SetValue(ChartLegend.RowsCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of columns to be used for items in the legend. This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// The columns count.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.RowsCount = legend.ColumnsCount = 2;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="500" Width="500" WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend RowsCount="2" ColumnsCount="3"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public int ColumnsCount
        {
            get
            {
                return (int)this.GetValue(ChartLegend.ColumnsCountProperty);
            }

            set
            {
                this.SetValue(ChartLegend.ColumnsCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of items in the legend. This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.RowsCount = legend.ColumnsCount = 2;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend Orientation="Horizontal"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        /// <seealso
        /// cref="ChartLegend.OrientationSealedOnDock">ChartLegend.OrientationSealedOnDock</seealso>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(ChartLegend.OrientationProperty);
            }

            set
            {
                this.SetValue(ChartLegend.OrientationProperty, value);
            }
        }
        
        /// <summary>
        /// The serialization using xaml writer will include the ChartSeries (ChartLegend's Item property) in the serialized string, if this property is not included with DesignerSerializationVisibility attribute. 
        /// the absence of this attribute may result in conflicting with default series of Chart Area.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ItemCollection Items
        {
            get { return base.Items; }
        }

        /// <summary>
        /// Gets or sets the CornerRadius of legend's border. This is a dependency property.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.CornerRadius = new CornerRadius(10,3,10,2);
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend CornerRadius="10,3,10,2"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ChartLegend.CornerRadiusProperty);
            }

            set
            {
                SetValue(ChartLegend.CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox of the legend items are
        /// visible. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Double clicking on legend will make a legend's window appear that can also be
        /// used to change checkboxes visibility.
        /// </remarks>
        /// <value>
        /// The check box visibility.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.CheckBoxVisibility = Visibility.Visible;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend CheckBoxVisibility="Visible"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public Visibility CheckBoxVisibility
        {
            get
            {
                return (Visibility)this.GetValue(ChartLegend.CheckBoxVisibilityProperty);
            }

            set
            {
                this.SetValue(ChartLegend.CheckBoxVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the icons of the legend items is
        /// visible. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Double clicking on legend will make a legend's dialog window appear that can
        /// also be used to change icons visibility.
        /// </remarks>
        /// <value>
        /// The icon visibility.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.IconVisibility = Visibility.Visible;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend IconVisibility="Visible"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public Visibility IconVisibility
        {
            get
            {
                return (Visibility)this.GetValue(ChartLegend.IconVisibilityProperty);
            }

            set
            {
                this.SetValue(ChartLegend.IconVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the icon that represents series (segment). This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// The width of the icon.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.IconWidth = 20;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend IconWidth="20"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public double IconWidth
        {
            get
            {
                return (double)this.GetValue(ChartLegend.IconWidthProperty);
            }

            set
            {
                this.SetValue(IconWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the icon that represents series (segment). This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// The height of the icon.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.IconHeight = 20;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend IconHeight="20"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public double IconHeight
        {
            get
            {
                return (double)this.GetValue(ChartLegend.IconHeightProperty);
            }

            set
            {
                this.SetValue(ChartLegend.IconHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the legend's items margin. This is a dependency property.
        /// </summary>
        /// <value>
        /// The element margin.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.ElementMargin = new Thickness(10,3,2,5);
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend ElementMargin="10,3,2,5"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public Thickness ElementMargin
        {
            get
            {
                return (Thickness)this.GetValue(ChartLegend.ElementMarginProperty);
            }

            set
            {
                this.SetValue(ChartLegend.ElementMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text alignment. This is a dependency property.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating a new chart instance.
        /// Chart chart = new Chart();
        /// //Creating a chart legend.
        /// ChartLegend legend = new ChartLegend();
        /// //Setting rows and columns count.
        /// legend.TextAlignment = VerticalAlignment.Bottom;
        /// //Setting legend's attached ChartDock property.
        /// ChartDockPanel.SetDock(legend, ChartDock.Left);
        /// //Adding legend to chart.
        /// chart.Legends.Add(legend);
        /// //Adding area to chart.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 3));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 2));
        /// //Creating new series.
        /// ChartSeries series1 = new ChartSeries();
        /// //Assigning series' label.
        /// series1.Label = "Series 1";
        /// //Assigning data.
        /// series1.Data = data;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series1);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 5));
        /// data.Add(new ChartPoint(2, 7));
        /// data.Add(new ChartPoint(3, 6));
        /// //Creating second series.
        /// ChartSeries series2 = new ChartSeries();
        /// //Assigning series' label.
        /// series2.Label = "Series 2";
        /// series2.Data = data;
        /// chart.Areas[0].Series.Add(series2);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 2));
        /// data.Add(new ChartPoint(2, 3));
        /// data.Add(new ChartPoint(3, 7));
        /// //Creating second series.
        /// ChartSeries series3 = new ChartSeries();
        /// //Assigning series' label.
        /// series3.Label = "Series 3";
        /// series3.Data = data;
        /// chart.Areas[0].Series.Add(series3);
        /// //Making a new data points.
        /// data = new ChartListData();
        /// data.Add(new ChartPoint(1, 7));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 5));
        /// //Creating second series.
        /// ChartSeries series4 = new ChartSeries();
        /// //Assigning series' label.
        /// series4.Label = "Series 4";
        /// series4.Data = data;
        /// chart.Areas[0].Series.Add(series4);
        /// //Setting window's content.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Title="Window1" Height="500" Width="500"
        /// WindowStartupLocation="CenterScreen"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:ChartLegend TextAlignment="Bottom"/&gt;
        /// &lt;/syncfusion:Chart.Legends&gt;
        /// &lt;syncfusion:Chart.Areas&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 1" Data="1 3 2 7 3 2"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 2" Data="1 5 2 7 3 6"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 3" Data="1 2 2 6 3 1"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 4" Data="1 5 2 3 3 8"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 5" Data="1 7 2 3 3 4"/&gt;
        /// &lt;syncfusion:ChartSeries Label="Series 6" Data="1 5 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart.Areas&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public VerticalAlignment TextAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(ChartLegend.TextAlignmentProperty);
            }

            set
            {
                this.SetValue(ChartLegend.TextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the show symbol.
        /// </summary>
        /// <value>The show symbol.</value>
        public Visibility ShowSymbol
        {
            get { return (Visibility)GetValue(ShowSymbolProperty); }
            set { SetValue(ShowSymbolProperty, value); }
        }
        /// <summary>
        /// Gets or sets the OffsetX value.
        /// </summary>
        /// <value>The OffsetX.</value>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OffsetY value.
        /// </summary>
        /// <value>The OffsetY.</value>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// Get or Set IsSegmentsLegendProperty
        /// </summary>
        public bool IsSegmentsLegend
        {
            get { return (bool)GetValue(IsSegmentsLegendProperty); }
            set { SetValue(IsSegmentsLegendProperty, value); }
        }


        /// <summary>
        /// Gets or sets the Custom Legend ItemsPanel.
        /// </summary>
        /// <value>The LegendItemsPanel</value>
        /// <remarks></remarks>
        public ItemsPanelTemplate LegendItemsPanel 
        {
            get { return (ItemsPanelTemplate)GetValue(LegendItemsPanelProperty); }
            set { SetValue(LegendItemsPanelProperty, value); }
        }


        /// <summary>
        /// Dependency Property for LegendItemsPanel
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty LegendItemsPanelProperty =
            DependencyProperty.Register("LegendItemsPanel", typeof(ItemsPanelTemplate), typeof(ChartLegend), new PropertyMetadata(null, OnLegendItemsPanelChanged));

        /// <summary>
        /// Called when LegendItemsPanel property is changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnLegendItemsPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legend = d as ChartLegend;
            if (d != null)
            {
                legend.SetItemsPanel();
            }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartLegend"/> class.
        /// </summary>
        static ChartLegend()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ChartLegend));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartLegend), new FrameworkPropertyMetadata(typeof(ChartLegend)));

            ItemsControl.ItemsSourceProperty.OverrideMetadata(typeof(ChartLegend), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegend"/> class.
        /// </summary>
        public ChartLegend()
        {
            this.DefaultStyleKey = typeof(ChartLegend);
            this.Items.Filter = new Predicate<object>(FilterByName);
        }

        private void FontFamilyChanged(object sender, EventArgs e)
        {
            ChartLegend legend = sender as ChartLegend;
            legend.m_isFontFamilySet = true;
        }

        private void FontWeightChanged(object sender, EventArgs e)
        {
            ChartLegend legend = sender as ChartLegend;
            legend.m_isFontWeightSet = true;
        }

        private void FontSizeChanged(object sender, EventArgs e)
        {
            ChartLegend legend = sender as ChartLegend;
            legend.m_isFontSizeSet = true;
        }

        internal void Dispose()
        {
            if (this.m_area != null)
            {
                this.m_area.Dispose();
                this.m_area = null;
            }
            this.m_highlightedElement = null;
            if (this.Items != null)
            {
                this.ItemsSource = null;
                this.Items.Clear();
            }

            if (this.Template != null)
            {
                if (((ControlTemplate)(this.Template)).Resources != null)
                {
                    ((ControlTemplate)(this.Template)).Resources.MergedDictionaries.Clear();
                }
            }

            this.Template = null;
            this.ItemsPanel = null;
            
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"></see>  event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (!e.Device.Target.Focusable)
            {
                if (EnvironmentTest.IsSecurityGranted)
                {
                    ShowLegendSettings();
                }

                e.Handled = true;
            }

            base.OnMouseDoubleClick(e);
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            
            base.OnPropertyChanged(e);
            if (e.Property == Chart.DockProperty || e.Property == ChartLegend.RowsCountProperty || e.Property == ChartLegend.ColumnsCountProperty || e.Property == ChartLegend.LegendPanelProperty)
            {
                AutomationProperties.SetItemStatus(this, string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + Chart.GetDock(this).ToString() + ";" + this.RowsCount.ToString() + ";" + this.ColumnsCount.ToString() + ";" + this.LegendPanel.ToString() + ";");
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (result != null)
            {
                ////Getting hit element.
                FrameworkElement hitElement = result.VisualHit as FrameworkElement;
                if (hitElement != null && hitElement.DataContext != null)
                {
                    ////Casting element's data context to ChartSeries.
                    ChartSeries hitSeries = hitElement.DataContext as ChartSeries;
                    ////We hit series.
                    if (hitSeries != null)
                    {
                        ////If there was hit sereis other then we have right now.
                        if (m_highlightedElement != null && m_highlightedElement != hitSeries)
                        {
                            ////Setting highlightins to false;
                            m_highlightedElement.Highlighted = false;
                            ////Reassigning series.
                            m_highlightedElement = hitSeries;
                        }
                        else
                        {
                            ////Just saving hit series.
                            m_highlightedElement = hitSeries;
                            ////Setting assigmed series as highlighted.
                            m_highlightedElement.Highlighted = true;
                        }
                    }
                }
                else
                {
                    ////Suppose there was no series hit, but we have a saved one.
                    if (m_highlightedElement != null)
                    {
                        ////Setting highlighting to false on saved series.
                        m_highlightedElement.Highlighted = false;
                        ////Nulling saved series.
                        m_highlightedElement = null;
                    }
                }
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Called when OffsetX is changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLegend legend = d as ChartLegend;
            if (legend.Parent is ChartDockPanel)
            {
                (legend.Parent as ChartDockPanel).InvalidateArrange();
            }
        }
        private static void OnValueChnged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLegend legend = d as ChartLegend;
            if (legend != null && legend.m_area != null)
            {
                legend.m_area.SetLegendSource(legend);
            }
        }
        /// <summary>
        /// Called when OffsetY is changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLegend legend = d as ChartLegend;
            if (legend.Parent is ChartDockPanel)
            {
                (legend.Parent as ChartDockPanel).InvalidateArrange();
            }
        }
        /// <summary>
        /// Called when items source is changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            PropertyMetadata basePM = ItemsControl.ItemsSourceProperty.GetMetadata(typeof(ItemsControl));

            if (args.NewValue is IList)
            {
                ListCollectionView listView = new ListCollectionView(args.NewValue as IList);
                args = new DependencyPropertyChangedEventArgs(args.Property, args.OldValue, listView);
            }

            if (basePM.PropertyChangedCallback != null)
            {
                basePM.PropertyChangedCallback(d, args);
            }
        }

        /// <summary>
        /// Shows the legend settings.
        /// </summary>
        private void ShowLegendSettings()
        {
            ChartLegendEditor legendEditorDialog = new ChartLegendEditor();
            legendEditorDialog.ShowDialog(this);
        }

        /// <summary>
        /// Filters the name of the by.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>Bool value</returns>
        private bool FilterByName(object obj)
        {
            DependencyObject dObj = obj as DependencyObject;
            if (dObj != null)
            {
                if (dObj is ChartAdornment)
                {
                    return false;
                }

                return object.Equals(this.GetValue(Chart.LegendNameProperty), dObj.GetValue(Chart.LegendNameProperty));
            }

            return true;
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Represents event for QtpLegendLocationChangedEventHandler.
        /// </summary>
        public event QtpLegendLocationChangedEventHandler LocationChanged;

        internal void OnLocationChanged(LegendLocationChangedeventArgs args)
        {
            if (LocationChanged != null)
            {
                LocationChanged(this, args);
            }
        }
        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            //string _xamlString = "<ChartLegend/>";
            EditorHelper.Register<BindingExpression, BindingConvertor>();
           
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            //foreach (var item in this.Items)
            //{
            //    outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<ChartSeries", "</ChartSeries>"), string.Empty);
            //}
            return outstr.ToString();
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }
}

     
