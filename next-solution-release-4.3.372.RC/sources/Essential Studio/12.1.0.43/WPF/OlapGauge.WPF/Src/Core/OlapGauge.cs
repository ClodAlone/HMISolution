//-------------------------------------------------------------------------------------------------
// <copyright file="OlapGauge.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;
using System.Collections.Generic;
using System;
using Syncfusion.Windows.Shared.Olap;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge.Olap
{
    /// <summary>
    /// The OlapGauge method is called when the Gauge is needed for representing the data report values.
    /// This helps in highlighting the KPI information through the gauge visualization.Helps in rendering Gauges.
    /// </summary>
    public class OlapGauge : ScrollViewer
    {
        private GaugeImage _mGaugeImage;

        #region Dependency Property Implementation

        /// <summary>
        /// ColumnsCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnsCountProperty =
            DependencyProperty.Register("ColumnsCount", typeof (int), typeof (OlapGauge), new UIPropertyMetadata(0,OnColumnsCountChanged));

        /// <summary>
        /// StatusIndicator Dependency Property
        /// </summary>
        public static readonly DependencyProperty StatusIndicatorProperty =
            DependencyProperty.Register("StatusIndicator", typeof (string), typeof (OlapGauge), new UIPropertyMetadata());

        /// <summary>
        /// FrameType Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof (GaugeFrameType), typeof (OlapGauge),
                                        new UIPropertyMetadata(GaugeFrameType.FullCircle, OnFrameTypeChanged));

        /// <summary>
        /// OlapDataManager Dependency Property
        /// </summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof (IOlapDataManager), typeof (OlapGauge),
                                        new UIPropertyMetadata(null, OnOlapDataManagerChanged));

        /// <summary>
        /// PivotEngine Dependency Property
        /// </summary>
        public static readonly DependencyProperty PivotEngineProperty =
            DependencyProperty.Register("PivotEngine", typeof(PivotEngine), typeof(OlapGauge),
                                        new UIPropertyMetadata(null, new PropertyChangedCallback(
                                            (dependencyObject, args) => 
                                            {
                                                OlapGauge olapGauge = dependencyObject as OlapGauge;
                                                if (olapGauge != null)
                                                {
                                                    olapGauge.DataBind(args.NewValue as PivotEngine);
                                                }
                                            })));

        /// <summary>
        /// Radius Dependency Property
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof (double), typeof (OlapGauge), new UIPropertyMetadata(150d,OnRadiusChanged));

        /// <summary>
        /// RowsCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty RowsCountProperty =
            DependencyProperty.Register("RowsCount", typeof (int), typeof (OlapGauge), new UIPropertyMetadata(0,OnRowsCountChanged));

        /// <summary>
        /// ShowMarkersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowMarkersTooltipProperty =
            DependencyProperty.Register("ShowMarkersTooltip", typeof (bool), typeof (OlapGauge),
                                        new UIPropertyMetadata(true,OnShowMarkersTooltipChanged));

        /// <summary>
        /// ShowPointersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowPointersTooltipProperty =
            DependencyProperty.Register("ShowPointersTooltip", typeof (bool), typeof (OlapGauge),
                                        new UIPropertyMetadata(true,OnShowPointersTooltipChanged));

        /// <summary>
        /// SizeToContainer Dependency Property
        /// </summary>
        public static readonly DependencyProperty SizeToContainerProperty =
            DependencyProperty.Register("SizeToContainer", typeof (bool), typeof (OlapGauge), new UIPropertyMetadata());

        /// <summary>
        /// ShowGaugeHeaders Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowGaugeHeadersProperty =
            DependencyProperty.Register("ShowGaugeHeaders", typeof (bool), typeof (OlapGauge),
                                        new UIPropertyMetadata(true,OnShowGaugeHeadersChanged));

        /// <summary>
        /// ShowGaugeLabels Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowGaugeLabelsProperty =
            DependencyProperty.Register("ShowGaugeLabels", typeof (bool), typeof (OlapGauge),
                                        new UIPropertyMetadata(true, OnShowGaugeLabelsChanged));

        /// <summary>
        /// ShowGaugeFactors Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowGaugeFactorsProperty =
            DependencyProperty.Register("ShowGaugeFactors", typeof (bool), typeof (OlapGauge),
                                        new UIPropertyMetadata(true,OnShowGaugeFactorsChanged));

        public static readonly DependencyProperty SeriesAxisProperty =
            DependencyProperty.Register("SeriesAxis", typeof(SeriesAxis), typeof(OlapGauge), new UIPropertyMetadata(new SeriesAxis()));

        public static readonly DependencyProperty CategoricalAxisProperty =
            DependencyProperty.Register("CategoricalAxis", typeof(CategoricalAxis), typeof(OlapGauge), new UIPropertyMetadata(new CategoricalAxis()));

        public static readonly DependencyProperty SlicerAxisProperty =
           DependencyProperty.Register("SlicerAxis", typeof(SlicerAxis), typeof(OlapGauge), new UIPropertyMetadata(new SlicerAxis()));

        public static readonly DependencyProperty CalculatedMembersProperty =
            DependencyProperty.Register("CalculatedMembers", typeof(CalculatedMembers), typeof(OlapGauge), new UIPropertyMetadata(new CalculatedMembers()));

        /// <summary>
        /// Visual Style Dependency Property
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(OlapGaugeVisualStyle), typeof(OlapGauge), new UIPropertyMetadata(OlapGaugeVisualStyle.Default,
                (dependencyObject, args) => 
                {
                    OlapGauge olapGauge = dependencyObject as OlapGauge;
                    if (olapGauge != null)
                    {
                        SkinStorage.SetVisualStyle(olapGauge, args.NewValue.ToString());
                        olapGauge.DataBind();
                    }
                }
                ));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the class by applying the default style of the gauge
        /// </summary>
        static OlapGauge()
        {
            EnvironmentTest.ValidateLicense(typeof(OlapGauge));
            DefaultStyleKeyProperty.OverrideMetadata(typeof (OlapGauge),
                                                     new FrameworkPropertyMetadata(typeof (OlapGauge)));
        }


        /// <summary>
        /// Initializes the gauge content and properties.
        /// </summary>
        public OlapGauge()
        {
            EnvironmentTest.ValidateLicense(typeof(OlapGauge));
            Loaded += new RoutedEventHandler(OlapGauge_Loaded);
            EnvironmentTest.ValidateLicense(typeof (OlapGauge));
            ShowMarkersTooltip = true;
            ShowPointersTooltip = true;
            Content = DefaultGauge();
        }

        void OlapGauge_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.OlapDataManager == null)
            {
                if (!string.IsNullOrEmpty(this.SharedDataManagerName) && SharedDataManagers.Instance.DataManagers != null && SharedDataManagers.Instance.DataManagers.Count > 0)
                {
                    foreach (var dataManager in SharedDataManagers.Instance.DataManagers)
                    {
                        if (dataManager != null && !string.IsNullOrEmpty(dataManager.Name) && dataManager.OlapDataManager != null && dataManager.Name.Equals(this.SharedDataManagerName))
                        {
                            if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                            {
                                dataManager.OlapDataManager.SetCurrentReport(CreateOlapReport());
                            }

                            this.OlapDataManager = dataManager.OlapDataManager;
                        }
                    }
                }
            }
            else if (this.OlapDataManager != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)//Active Report concept
            {
                (this.OlapDataManager as OlapDataManager).ActiveReport = this.OlapDataManager.Reports[this.ReportName];
                Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                this.PivotEngine = this.OlapDataManager.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
                (this.OlapDataManager as OlapDataManager).ActiveReportChanged += new ActiveReportChangedEventHandler(OlapGauge_ActiveReportChanged);

            }

#if !SILVERLIGHT
            if (this.OlapDataManager != null)
            {
                this.OlapDataManager.AxisElementChanged -= new AxisElementChangedEventHandler(OlapDataManagerAxisElementChanged);
                this.OlapDataManager.AxisElementChanged += new AxisElementChangedEventHandler(OlapDataManagerAxisElementChanged);
            }
#endif
        }

        void OlapGauge_ActiveReportChanged(object sender, ActiveReportChangedEventArgs e)
        {
            if (e.NewActiveReport != null && e.NewActiveReport.Name == this.ReportName && e.IsReportChanged)
            {
                Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                this.PivotEngine = this.OlapDataManager.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the report.
        /// </summary>
        /// <value>The name of the report.</value>
        public string ReportName { get; set; }

        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        /// <value>The name of the current cube.</value>
        public string CurrentCubeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the shared data manager.
        /// </summary>
        /// <value>The name of the shared data manager.</value>
        public string SharedDataManagerName { get; set; }

        /// <summary>
        /// Gets or sets the categorical axis.
        /// </summary>
        /// <value>The categorical axis.</value>
        public CategoricalAxis CategoricalAxis
        {
            get { return (CategoricalAxis)GetValue(CategoricalAxisProperty); }
            set { SetValue(CategoricalAxisProperty, value); }
        }

        
        /// <summary>
        /// Gets or sets the series axis.
        /// </summary>
        /// <value>The series axis.</value>
        public SeriesAxis SeriesAxis
        {
            get { return (SeriesAxis)GetValue(SeriesAxisProperty); }
            set { SetValue(SeriesAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the slicer axis.
        /// </summary>
        /// <value>The slicer axis.</value>
        public SlicerAxis SlicerAxis
        {
            get { return (SlicerAxis)GetValue(SlicerAxisProperty); }
            set { SetValue(SlicerAxisProperty, value); }
        }       

        /// <summary>
        /// Gets or sets the calculated members.
        /// </summary>
        /// <value>The calculated members.</value>
        public CalculatedMembers CalculatedMembers
        {
            get { return (CalculatedMembers)GetValue(CalculatedMembersProperty); }
            set { SetValue(CalculatedMembersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the columns count.
        /// </summary>
        public int ColumnsCount
        {
            get { return (int) GetValue(ColumnsCountProperty); }
            set { SetValue(ColumnsCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Status Indicator
        /// </summary>
        public string StatusIndicator
        {
            get { return (string) GetValue(StatusIndicatorProperty); }
            set { SetValue(StatusIndicatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the frame and it also sets the default frame type to Circular Center Gradient
        /// </summary>
        [DefaultValue(GaugeFrameType.CircularCenterGradient)]
        public GaugeFrameType FrameType
        {
            get { return (GaugeFrameType) GetValue(FrameTypeProperty); }
            set { SetValue(FrameTypeProperty, value); }
        }

        /// <summary>
        /// Gets the kpi info collection.
        /// </summary>
        public KpiInfoCollection KpiInfoCollection { get; internal set; }


        /// <summary>
        /// Gets or sets the olap data manager.
        /// </summary>
        public IOlapDataManager OlapDataManager
        {
            get { return (IOlapDataManager) GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pivot engine.
        /// </summary>
        public PivotEngine PivotEngine
        {
            get { return (PivotEngine) GetValue(PivotEngineProperty); }
            set { SetValue(PivotEngineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radius and default radius value is 100.0
        /// </summary>
        [DefaultValue(100.0)]
        public double Radius
        {
            get { return (double) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rows count.
        /// </summary>
        public int RowsCount
        {
            get { return (int) GetValue(RowsCountProperty); }
            set { SetValue(RowsCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show markers tooltip].
        /// </summary>
        /// <value><c>true</c> if [show markers tooltip]; otherwise, <c>false</c>.</value>
        public bool ShowMarkersTooltip
        {
            get { return (bool) GetValue(ShowMarkersTooltipProperty); }
            set { SetValue(ShowMarkersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show pointers tooltip].
        /// </summary>
        /// <value><c>true</c> if [show pointers tooltip]; otherwise, <c>false</c>.</value>
        public bool ShowPointersTooltip
        {
            get { return (bool) GetValue(ShowPointersTooltipProperty); }
            set { SetValue(ShowPointersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [size to container].
        /// </summary>
        /// <value><c>true</c> if [size to container]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool SizeToContainer
        {
            get { return (bool) GetValue(SizeToContainerProperty); }
            set { SetValue(SizeToContainerProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [show Gauge Headers].
        /// </summary>
        /// <value><c>true</c> if [show Gauge Headers]; otherwise, <c>false</c>.</value>
        public bool ShowGaugeHeaders
        {
            get { return (bool) GetValue(ShowGaugeHeadersProperty); }
            set { SetValue(ShowGaugeHeadersProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [show Gauge Labels].
        /// </summary>
        /// <value><c>true</c> if [show Gauge Labels]; otherwise, <c>false</c>.</value>
        public bool ShowGaugeLabels
        {
            get { return (bool) GetValue(ShowGaugeLabelsProperty); }
            set { SetValue(ShowGaugeLabelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [show Gauge Factors].
        /// </summary>
        /// <value><c>true</c> if [show Gauge Factors]; otherwise, <c>false</c>.</value>
        public bool ShowGaugeFactors
        {
            get { return (bool) GetValue(ShowGaugeFactorsProperty); }
            set { SetValue(ShowGaugeFactorsProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the VisualStyle for OlapGauge
        /// </summary>
        public OlapGaugeVisualStyle VisualStyle
        {
            get { return (OlapGaugeVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the default Gauge when OlapDataManager is not associated
        /// </summary>
        /// <returns>A default OlapGauge Control will be returned</returns>
        private TextBlock DefaultGauge()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            var txtBlock = new TextBlock
                               {
                                   HorizontalAlignment = HorizontalAlignment.Center,
                                   VerticalAlignment = VerticalAlignment.Top,
                                   Text = "Kpi with Default Members"
                               };
            return txtBlock;
        }

        /// <summary>
        /// Shows the Information if Kpi elements are empty.
        /// </summary>
        /// <returns>Returns a grid with Alert Message</returns>
        private static Grid ShowEmptyKpi()
        {
            var defaultGrid = new Grid();
            defaultGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(20, GridUnitType.Auto)});
            var kpiText = new TextBlock
                              {
                                  Margin = new Thickness(5d),
                                  HorizontalAlignment = HorizontalAlignment.Left,
                                  VerticalAlignment = VerticalAlignment.Center,
                                  Text = "KPI Data Not Available",
                                  FontSize = 15
                              };
            Grid.SetRow(kpiText, 0);
            defaultGrid.Children.Add(kpiText);
            return defaultGrid;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Olaps the data manager axis element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Olap.Manager.AxisElementChangedEventArgs"/> instance containing the event data.</param>
        private void OlapDataManagerAxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            if (OlapDataManager.CurrentCellSet != null)
            {
                DataBind();
            }
        }
#endif

        /// <summary>
        /// Adds the Grid with Layout specified in RowsCount and ColumnsCount Property
        /// </summary>
        private void AddGridLayout()
        {
            var multipleGaugeGrid = new Grid();
            //Positions the scroll Bars based on the boolean variable
            PositionScrollBars(false);
            multipleGaugeGrid.HorizontalAlignment = HorizontalAlignment.Left;
            multipleGaugeGrid.VerticalAlignment = VerticalAlignment.Top;
            int kpiCount = 0;
            for (int i = 0; i < RowsCount; i++)
            {
                multipleGaugeGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(20, GridUnitType.Auto)});
                for (int j = 0; j < KpiInfoCollection.Count || kpiCount < KpiInfoCollection.Count; j++)
                {
                    multipleGaugeGrid.ColumnDefinitions.Add(new ColumnDefinition
                                                                {Width = new GridLength(30, GridUnitType.Auto)});
                    if (j > (ColumnsCount - 1) || kpiCount >= KpiInfoCollection.Count)
                    {
                        //Breaks the loop Whenever the max Columns Count is reached
                        break;
                    }
                    //Returns a grid already populated with the Controls OlapGauge Controls
                    Grid gridCell = AddOlapGaugeControltoGrid(ref kpiCount, i, j);
                    //Adds the grid as child of the Main Grid Control.
                    multipleGaugeGrid.Children.Add(gridCell);
                }
            }
            SkinStorage.SetVisualStyle(multipleGaugeGrid, SkinStorage.GetVisualStyle(this));
            //Sets the MultipleGrid as the current Content
            Content = multipleGaugeGrid;
        }

        /// <summary>
        /// Adds the WrapPanel with the available Gauges in KpiInfoCollection
        /// </summary>
        private void AddWrapPanel()
        {
            //Positions the scroll Bars based on the boolean variable
            PositionScrollBars(true);
            var multipleGaugePanel = new WrapPanel();
            for (int i = 0; i < KpiInfoCollection.Count; i++)
            {
                Grid gridCell = AddGridCell();
                //Returns a grid cell already populated with the OlapGauge Controls
                AddOlapGaugeControltoWrapPanel(i, gridCell);

                //Adds the Grid as child of the Wrap Panel
                multipleGaugePanel.Children.Add(gridCell);
            }

            Content = multipleGaugePanel;
        }

        /// <summary>
        /// Positions Inbuilt ScrollBar based on the Type of Panel
        /// </summary>
        private void PositionScrollBars(bool wrapPanel)
        {
            if (wrapPanel)
            {
                //If this method is invoked from WrapPanel then horizontal Scroll Bar should be disabled
                ScrollToHome();
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                //If this method is invoked from Grid then both the scroll bars should be set to Auto
                ScrollToHome();
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        /// <summary>
        /// Creates a Grid control with the OlapGauge and OlapGauge Header Texts for Grid Control
        /// </summary>
        /// <returns>The Grid with both the above Controls</returns>
        private Grid AddOlapGaugeControltoGrid(ref int kpiCount, int i, int j)
        {
            Grid gridCell = AddGridCell();
            //Creates a Text on top of the Gauge Control called Gauge Headers
            if (ShowGaugeHeaders)
            {
                var kpiText = new TextBlock
                                  {
                                      HorizontalAlignment = HorizontalAlignment.Left,
                                      Margin = new Thickness(5, 0, 5, 0),
                                      Text =
                                          KpiInfoCollection[kpiCount].Kpi_Name + " For " +
                                          KpiInfoCollection[kpiCount].MemberName
                                  };

                //Sets the Gauge Headers to the First Row of the Grid
                Grid.SetRow(kpiText, 0);

                //Adds the Controls as the Children of the Main Grid Control
                gridCell.Children.Add(kpiText);
            }
            var olapCircularGauge = new OlapCircularGauge
                                        {
                                            KpiInfo = KpiInfoCollection[kpiCount],
                                            Radius = Radius,
                                            FrameType = FrameType,
                                            ShowMarkersTooltip = ShowMarkersTooltip,
                                            ShowPointersTooltip = ShowPointersTooltip,
                                            ShowGaugeLabels = ShowGaugeLabels
                                        };

            olapCircularGauge.KpiInfo.StatusGraphic = StatusIndicator;
            olapCircularGauge.ShowGaugeFactors = ShowGaugeFactors;
            SizeToContainer = false;
            olapCircularGauge.SizeToContainer = SizeToContainer;
            olapCircularGauge.Margin = new Thickness(10);
            if (_mGaugeImage != null)
            {
                olapCircularGauge.AddGaugeImage(_mGaugeImage);
            }
            //Sets the OlapCircularGauge to the Second Row of the Grid
            Grid.SetRow(olapCircularGauge, 1);
            //Adds the Controls as the Children of the Main Grid Control
            gridCell.Children.Add(olapCircularGauge);
            kpiCount++;
            Grid.SetColumn(gridCell, j);
            Grid.SetRow(gridCell, i);
            return gridCell;
        }

        private OlapReport CreateOlapReport()
        {
            OlapReport report = new OlapReport { Name = this.ReportName };
            try
            {
                report.CurrentCubeName = this.CurrentCubeName;

                #region Categorical Axis
                if (this.CategoricalAxis.Count > 0)
                {
                    foreach (var item in this.CategoricalAxis)
                    {
                        #region Dimension
                        if (item is Dimension)
                        {
                            Dimension dimension = item as Dimension;
                            DimensionElement dimensionElementColumn = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                            dimensionElementColumn.Hierarchy = new HierarchyElement { Name = dimension.HierarchyName };
                            dimensionElementColumn.AddLevel(dimension.HierarchyName, dimension.LevelName);
                            if (dimension.MemberProperties.Count > 0)
                            {
                                foreach (var memberProperty in dimension.MemberProperties)
                                {
                                    dimensionElementColumn.MemberProperties.Add(memberProperty.Name, memberProperty.UniqueName);
                                }
                            }

                            if (dimension.DimensionType == DimesnionType.Exclude)
                            {
                                DimensionElement excludedDimensionElementColumn = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                                excludedDimensionElementColumn.AddLevel(dimension.HierarchyName, dimension.LevelName);
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(excludedDimensionElementColumn.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.CategoricalElements.Add(dimensionElementColumn, excludedDimensionElementColumn);
                            }
                            else
                            {
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(dimensionElementColumn.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.CategoricalElements.Add(new Item { ElementValue = dimensionElementColumn });
                            }
                        }
                        #endregion
                        #region Measure
                        else if (item is Measure)
                        {
                            Measure measure = item as Measure;
                            MeasureElements measureElementsColumn = new MeasureElements();
                            measureElementsColumn.Elements.Add(new MeasureElement { Name = measure.Name });
                            report.CategoricalElements.Add(measureElementsColumn);
                        }
                        #endregion
                        #region KPI
                        else if (item is Kpi)
                        {
                            Kpi kpi = item as Kpi;
                            KpiElements kpiElements = new KpiElements();
                            kpiElements.Elements.Add(new KpiElement { Name = kpi.Name, ShowKPIGoal = kpi.ShowGoal, ShowKPIStatus = kpi.ShowStatus, ShowKPITrend = kpi.ShowTrend, ShowKPIValue = kpi.ShowValue });
                            report.CategoricalElements.Add(kpiElements);
                        }
                        #endregion
                        #region NamedSet
                        else if (item is NamedSet)
                        {
                            NamedSet namedSet = item as NamedSet;
                            NamedSetElement namedSetElement = new NamedSetElement { Name = namedSet.Name, DimensionName = namedSet.DimensionName, DimensionUniqueName = namedSet.DimensionUniqueName };
                            report.CategoricalElements.Add(namedSetElement);
                        }
                        #endregion
                        #region SortElement
                        else if (item is Sort)
                        {
                            Sort sortItem = item as Sort;
                            SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), sortItem.SortOrder.ToString(), true);
                            SortElement sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.Name = sortItem.MeasureUniqueName;
                            report.CategoricalElements.Add(sortElement);
                        }
                        #endregion
                        #region SubSet
                        else if (item is SubSet)
                        {
                            SubSet subSetItem = item as SubSet;
                            SubsetElement subSetElement;
                            if (subSetItem.StartIndex != -1 && subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.StartIndex, subSetItem.EndIndex);
                            }
                            else if (subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.EndIndex);
                            }
                            else
                            {
                                subSetElement = new SubsetElement();
                            }
                            subSetElement.Name = subSetItem.Name;
                            report.CategoricalElements.SubSetElement = subSetElement;
                        }
                        #endregion
                        #region TopCountElement
                        else if (item is TopCount)
                        {
                            TopCount topCountItem = item as TopCount;
                            TopCountElement topCountElement = new TopCountElement(AxisPosition.Categorical, topCountItem.FieldCount);
                            topCountElement.MeasureName = topCountItem.MeasureName;
                            report.CategoricalElements.Add(topCountElement);
                        }
                        #endregion

                    }
                }
                #endregion

                #region Series Axis
                if (this.SeriesAxis.Count > 0)
                {
                    foreach (var item in this.SeriesAxis)
                    {
                        #region Dimension
                        if (item is Dimension)
                        {
                            Dimension dimension = item as Dimension;
                            DimensionElement dimensionElementRow = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                            //dimensionElementRow.Hierarchy = new HierarchyElement { Name = dimension.HierarchyName };
                            dimensionElementRow.AddLevel(dimension.HierarchyName, dimension.LevelName);
                            if (dimension.MemberProperties.Count > 0)
                            {
                                foreach (var memberProperty in dimension.MemberProperties)
                                {
                                    dimensionElementRow.MemberProperties.Add(memberProperty.Name, memberProperty.UniqueName);
                                }
                            }

                            if (dimension.DimensionType == DimesnionType.Exclude)
                            {
                                DimensionElement excludedDimensionElementRow = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                                excludedDimensionElementRow.AddLevel(dimension.HierarchyName, dimension.LevelName);
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(excludedDimensionElementRow.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SeriesElements.Add(dimensionElementRow, excludedDimensionElementRow);
                            }
                            else
                            {
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(dimensionElementRow.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SeriesElements.Add(new Item { ElementValue = dimensionElementRow });
                            }
                        }
                        #endregion
                        #region Measure
                        else if (item is Measure)
                        {
                            Measure measure = item as Measure;
                            MeasureElements measureElementsRow = new MeasureElements();
                            measureElementsRow.Elements.Add(new MeasureElement { Name = measure.Name });
                            report.SeriesElements.Add(measureElementsRow);
                        }
                        #endregion
                        #region KPI
                        else if (item is Kpi)
                        {
                            Kpi kpi = item as Kpi;
                            KpiElements kpiElements = new KpiElements();
                            kpiElements.Elements.Add(new KpiElement { Name = kpi.Name, ShowKPIGoal = kpi.ShowGoal, ShowKPIStatus = kpi.ShowStatus, ShowKPITrend = kpi.ShowTrend, ShowKPIValue = kpi.ShowValue });
                            report.SeriesElements.Add(kpiElements);
                        }
                        #endregion
                        #region NamedSet
                        else if (item is NamedSet)
                        {
                            NamedSet namedSet = item as NamedSet;
                            NamedSetElement namedSetElement = new NamedSetElement { Name = namedSet.Name, DimensionName = namedSet.DimensionName, DimensionUniqueName = namedSet.DimensionUniqueName };
                            report.SeriesElements.Add(namedSetElement);
                        }
                        #endregion
                        #region SortElement
                        else if (item is Sort)
                        {
                            Sort sortItem = item as Sort;
                            SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), sortItem.SortOrder.ToString(), true);
                            SortElement sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.Name = sortItem.MeasureUniqueName;
                            report.SeriesElements.Add(sortElement);
                        }
                        #endregion
                        #region SubSet
                        else if (item is SubSet)
                        {
                            SubSet subSetItem = item as SubSet;
                            SubsetElement subSetElement;
                            if (subSetItem.StartIndex != -1 && subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.StartIndex, subSetItem.EndIndex);
                            }
                            else if (subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.EndIndex);
                            }
                            else
                            {
                                subSetElement = new SubsetElement();
                            }
                            subSetElement.Name = subSetItem.Name;
                            report.SeriesElements.SubSetElement = subSetElement;
                        }
                        #endregion
                        #region TopCountElement
                        else if (item is TopCount)
                        {
                            TopCount topCountItem = item as TopCount;
                            TopCountElement topCountElement = new TopCountElement(AxisPosition.Categorical, topCountItem.FieldCount);
                            topCountElement.MeasureName = topCountItem.MeasureName;
                            report.SeriesElements.Add(topCountElement);
                        }
                        #endregion
                    }
                }
                #endregion

                #region Slicer Axis
                if (this.SlicerAxis.Count > 0)
                {
                    foreach (var item in this.SlicerAxis)
                    {
                        #region Dimension
                        if (item is Dimension)
                        {
                            Dimension dimension = item as Dimension;
                            DimensionElement dimensionElementSlicer = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                            dimensionElementSlicer.Hierarchy = new HierarchyElement { Name = dimension.HierarchyName };
                            dimensionElementSlicer.AddLevel(dimension.HierarchyName, dimension.LevelName);
                            if (dimension.MemberProperties.Count > 0)
                            {
                                foreach (var memberProperty in dimension.MemberProperties)
                                {
                                    dimensionElementSlicer.MemberProperties.Add(memberProperty.Name, memberProperty.UniqueName);
                                }
                            }

                            if (dimension.DimensionType == DimesnionType.Exclude)
                            {
                                DimensionElement excludedDimensionElementSlicer = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                                excludedDimensionElementSlicer.AddLevel(dimension.HierarchyName, dimension.LevelName);
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(excludedDimensionElementSlicer.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SlicerElements.Add(dimensionElementSlicer, excludedDimensionElementSlicer);
                            }
                            else
                            {
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(dimensionElementSlicer.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SlicerElements.Add(dimensionElementSlicer);
                            }
                        }
                        #endregion
                        #region Measure
                        else if (item is Measure)
                        {
                            Measure measure = item as Measure;
                            MeasureElements measureElementsSlicer = new MeasureElements();
                            measureElementsSlicer.Elements.Add(new MeasureElement { Name = item.Name });
                            report.SlicerElements.Add(measureElementsSlicer);
                        }
                        #endregion
                        #region KPI
                        else if (item is Kpi)
                        {
                            Kpi kpi = item as Kpi;
                            KpiElements kpiElements = new KpiElements();
                            kpiElements.Elements.Add(new KpiElement { Name = kpi.Name, ShowKPIGoal = kpi.ShowGoal, ShowKPIStatus = kpi.ShowStatus, ShowKPITrend = kpi.ShowTrend, ShowKPIValue = kpi.ShowValue });
                            report.SlicerElements.Add(kpiElements);
                        }
                        #endregion
                        #region NamedSet
                        else if (item is NamedSet)
                        {
                            NamedSet namedSet = item as NamedSet;
                            NamedSetElement namedSetElement = new NamedSetElement { Name = namedSet.Name, DimensionName = namedSet.DimensionName, DimensionUniqueName = namedSet.DimensionUniqueName };
                            report.SlicerElements.Add(namedSetElement);
                        }
                        #endregion
                        #region SortElement
                        else if (item is Sort)
                        {
                            Sort sortItem = item as Sort;
                            SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), sortItem.SortOrder.ToString(), true);
                            SortElement sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.Name = sortItem.MeasureUniqueName;
                            report.SlicerElements.Add(sortElement);
                        }
                        #endregion
                        #region SubSet
                        else if (item is SubSet)
                        {
                            SubSet subSetItem = item as SubSet;
                            SubsetElement subSetElement;
                            if (subSetItem.StartIndex != -1 && subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.StartIndex, subSetItem.EndIndex);
                            }
                            else if (subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.EndIndex);
                            }
                            else
                            {
                                subSetElement = new SubsetElement();
                            }
                            subSetElement.Name = subSetItem.Name;
                            report.SlicerElements.SubSetElement = subSetElement;
                        }
                        #endregion
                        #region TopCountElement
                        else if (item is TopCount)
                        {
                            TopCount topCountItem = item as TopCount;
                            TopCountElement topCountElement = new TopCountElement(AxisPosition.Categorical, topCountItem.FieldCount);
                            topCountElement.MeasureName = topCountItem.MeasureName;
                            report.SlicerElements.Add(topCountElement);
                        }
                        #endregion
                    }
                }
                #endregion

                #region CalculatedMembers
                if (this.CalculatedMembers.Count > 0)
                {
                    foreach (CalculatedMember calcMemberItem in this.CalculatedMembers)
                    {
                        TypeOfMember calcMemberType = (TypeOfMember)Enum.Parse(typeof(TypeOfMember), calcMemberItem.MemberType.ToString());
                        Syncfusion.Olap.Reports.CalculatedMember calculatedMember = new Syncfusion.Olap.Reports.CalculatedMember { Expression = calcMemberItem.Expression, Name = calcMemberItem.Name };
                        if (calcMemberType == TypeOfMember.Measure)
                        {
                            calculatedMember.AddElement(new MeasureElement { Name = calcMemberItem.ElementName });
                        }
                        else if (calcMemberType == TypeOfMember.Dimension)
                        {
                            DimensionElement dimenesionElement = new DimensionElement { Name = calcMemberItem.ElementName, HierarchyName = calcMemberItem.ElementHierarchyName };
                            dimenesionElement.AddLevel(calcMemberItem.ElementHierarchyName, calcMemberItem.ElementLevelName);
                            calculatedMember.AddElement(dimenesionElement);
                        }

                        report.CalculatedMembers.Add(calculatedMember);
                        switch (calcMemberItem.AxisPosition)
                        {
                            case PositionOfAxis.Categorical:
                                report.CategoricalElements.Add(calculatedMember);
                                break;
                            case PositionOfAxis.Series:
                                report.SeriesElements.Add(calculatedMember);
                                break;
                            case PositionOfAxis.Slicer:
                                break;
                            default:
                                break;
                        }

                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return report;
        }

        private void GetIncludedMembers(LevelElement levelElement, List<string> list)
        {
            foreach (string member in list)
            {
                levelElement.Add(member.Trim());
            }
            levelElement.IncludeAvailableMembers = true;
        }

        private void AddOlapGaugeControltoWrapPanel(int i, Grid gridCell)
        {
            if (ShowGaugeHeaders)
            {
                //Creates a Text on top of the Gauge Control called Gauge Headers
                var kpiText = new TextBlock
                                  {
                                      HorizontalAlignment = HorizontalAlignment.Center,
                                      Margin = new Thickness(5, 0, 5, 0),
                                      Text = KpiInfoCollection[i].Kpi_Name + " For " + KpiInfoCollection[i].MemberName
                                  };
                Grid.SetRow(kpiText, 0);
                //Adds the Controls as the Children of the Main Grid Control
                gridCell.Children.Add(kpiText);
            }

            ////Creates a OlapCircularGauge Control
            var olapCircularGauge = new OlapCircularGauge
                                        {
                                            KpiInfo = KpiInfoCollection[i],
                                            Radius = Radius,
                                            FrameType = FrameType,
                                            ShowMarkersTooltip = ShowMarkersTooltip,
                                            ShowPointersTooltip = ShowPointersTooltip,
                                            ShowGaugeLabels = ShowGaugeLabels
                                        };
            olapCircularGauge.KpiInfo.StatusGraphic = StatusIndicator;
            olapCircularGauge.ShowGaugeFactors = ShowGaugeFactors;
            olapCircularGauge.SizeToContainer = SizeToContainer;
            olapCircularGauge.Margin = new Thickness(10);
            if (_mGaugeImage != null)
            {
                olapCircularGauge.AddGaugeImage(_mGaugeImage);
            }
            //Sets the OlapCircularGauge to the Second Row of the Grid
            Grid.SetRow(olapCircularGauge, 1);
            //Adds the Controls as the Children of the Main Grid Control
            gridCell.Children.Add(olapCircularGauge);
            Grid.SetColumn(gridCell, 0);
            Grid.SetRow(gridCell, i);
        }

        /// <summary>
        /// Creates a blank Grid with all the grid properties and definitions
        /// </summary>
        /// <returns>The Grid control with default grid properties nad definitions</returns>
        private Grid AddGridCell()
        {
            var gridCell = new Grid
                               {
                                   HorizontalAlignment = HorizontalAlignment.Center,
                                   VerticalAlignment = VerticalAlignment.Top
                               };
            if (ShowGaugeHeaders)
            {
                gridCell.RowDefinitions.Add(new RowDefinition {Height = new GridLength(20)});
            }
            gridCell.RowDefinitions.Add(new RowDefinition {Height = new GridLength(20, GridUnitType.Star)});
            gridCell.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(30, GridUnitType.Auto)});
            return gridCell;
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.OlapDataManager != null)
                {
                    if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                    {
                        this.OlapDataManager.SetCurrentReport(this.CreateOlapReport());
                        this.DataBind();
                    }                    
                }
                else if (!string.IsNullOrEmpty(this.SharedDataManagerName) && SharedDataManagers.Instance.DataManagers != null && SharedDataManagers.Instance.DataManagers.Count > 0)
                {
                    foreach (var dataManager in SharedDataManagers.Instance.DataManagers)
                    {
                        if (dataManager != null && !string.IsNullOrEmpty(dataManager.Name) && dataManager.OlapDataManager != null && dataManager.Name.Equals(this.SharedDataManagerName))
                        {
                            if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                            {
                                dataManager.OlapDataManager.SetCurrentReport(CreateOlapReport());
                            }

                            this.OlapDataManager = dataManager.OlapDataManager;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Clears the OlapGrid data.
        /// </summary>
        public void ClearData()
        {
            InvalidateVisual();
            Content = null;
            Content = ShowEmptyKpi();
        }

        /// <summary>
        /// Adds the gauge image.
        /// </summary>
        /// <param name="gaugeImage">The gauge image.</param>
        public void AddGaugeImage(GaugeImage gaugeImage)
        {
            _mGaugeImage = gaugeImage;
            DataBind();
        }

        /// <summary>
        /// Binds the OlapGauge with the information available in KpiInfoCollection
        /// </summary>
        /// <returns>The OlapGauge's embedded with Information on Above</returns>
        public void DataBind()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (OlapDataManager != null && OlapDataManager.ItemSource == null)
                {
                    Syncfusion.Olap.Data.CellSet cellSet = OlapDataManager.ExecuteCellSet();
                    PivotEngine = OlapDataManager.ExecuteOlapTable(cellSet);
                }
                DataBind(PivotEngine);
            }
        }

        private void DataBind(PivotEngine pivotEngine)
        {
            if (pivotEngine != null)
            {
                KpiInfoCollection = pivotEngine.GetValidKpis();
                if (KpiInfoCollection.Count > 0)
                {
                    //If ColumnsCount or RowsCount are not specified then render the Gauges in WrapPanel
                    if (ColumnsCount == 0 || RowsCount == 0)
                    {
                        AddWrapPanel();
                    }
                    else
                    {
                        AddGridLayout();
                    }
                }
            }
            else
            {
                //Sets the Empty KPI Content Text as the content if KpiInfoCollection doesn't contain any Data
                //Sets the Empty KPI Content Text as the content if PivotEngine doesn't contain any Data
                Content = ShowEmptyKpi();
            }
        }
#endregion

        #region Public Static Methods

        /// <summary>
        /// Called when [cube model changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnOlapDataManagerChanged(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs e)
        {
            var olapGauge = dependencyObject as OlapGauge;
            OlapDataManager newOlapDataManager = e.NewValue as OlapDataManager;
            if (olapGauge != null)
            {
                olapGauge.ClearData();
                if (newOlapDataManager != null)
                    if (newOlapDataManager.CurrentCellSet != null)
                    {
                        olapGauge.PivotEngine = newOlapDataManager.ExecuteOlapTable(newOlapDataManager.CurrentCellSet);
                        olapGauge.DataBind(olapGauge.PivotEngine);
                    }
                    else
                    {
                        olapGauge.DataBind();
                    }
            }
        }

        private static void OnFrameTypeChanged(DependencyObject dependencyObject,
                                                   DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowMarkersTooltipChanged(DependencyObject dependencyObject,
                                                 DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowPointersTooltipChanged(DependencyObject dependencyObject,
                                                DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowGaugeHeadersChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowGaugeLabelsChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowGaugeFactorsChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }
        private static void OnRowsCountChanged(DependencyObject dependencyObject,
                                              DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }
        private static void OnColumnsCountChanged(DependencyObject dependencyObject,
                                              DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnRadiusChanged(DependencyObject dependencyObject,
                                            DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }
        #endregion
    }

    /// <summary>
    /// Specifies the  Visual Style for OlapGauge
    /// </summary>
    public enum OlapGaugeVisualStyle
    {
        /// <summary>
        /// Provides Blend Style for OlapGauge
        /// </summary>
        Blend,
        /// <summary>
        /// Provides Default Style for OlapGauge
        /// </summary>
        Default,
        /// <summary>
        /// Provides Metro Style for OlapGauge
        /// </summary>
        Metro,
        /// <summary>
        /// Provides Office2003 Style for OlapGauge
        /// </summary>
        Office2003,
        /// <summary>
        /// Provides Office2007Black Style for OlapGauge
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Provides Office2007Blue Style for OlapGauge
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Provides Office2007Silver Style for OlapGauge
        /// </summary>
        Office2007Silver
    }
}