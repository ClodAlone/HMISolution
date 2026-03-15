#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
using Syncfusion.Licensing;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Manager;
using Syncfusion.Windows.Shared.Olap;
using Syncfusion.Windows.Grid.Olap.Common;
using Syncfusion.Olap.Reports;

namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Manager;
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.Silverlight.Grid.Olap.Common;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Grid;
    using System.Windows.Threading;
    using System.Windows.Input;    
    using Syncfusion.Windows;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Controls.Primitives;    
    using System.Windows.Data;
    using System.Collections.Generic;
    using Syncfusion.Windows.Shared;
    using System.Windows.Media.Animation;
#if SILVERLIGHT
    using Syncfusion.Windows.Controls.Theming;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
#endif
    /// <summary>
    /// OlapGrid Type
    /// </summary>
    [TemplatePart(Name = "PART_OlapGridBase", Type = typeof(OlapGrid))]
    #if !SILVERLIGHT
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
   Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
 Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.WPF;component/Themes/MetroStyle.xaml")]
    
    #else
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/BlendStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
       Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/Generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/Office2007BlueStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
       Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/Office2007BlackStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
  Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/Office2007SilverStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
 Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/MetroStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
    Type = typeof(OlapGrid), XamlResource = "/Syncfusion.OlapGrid.Silverlight;component/Themes/Office2010Black.xaml")]
#endif
    public class OlapGrid : System.Windows.Controls.Control   
    {
        #region Paging Members

#if !SILVERLIGHT
        private OlapPagingGrid _opg;
#else
        private bool m_ShowProcessingBar = true;
#endif

        #endregion

        #region Dependency Property Declarations

#if !SILVERLIGHT
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(IOlapDataManager), typeof(OlapGrid), new UIPropertyMetadata(null, OlapGrid.OnOlapDataManagerChanged));
#endif

        public static readonly DependencyProperty LayoutProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Layout", typeof(GridLayout), typeof(OlapGrid), new UIPropertyMetadata(GridLayout.Normal, OlapGrid.OnGridLayoutChanged));
#else
            DependencyProperty.Register("Layout", typeof(GridLayout), typeof(OlapGrid), new PropertyMetadata(GridLayout.Normal, OlapGrid.OnGridLayoutChanged));

#endif

        public static readonly DependencyProperty ColumnHeaderStyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ColumnHeaderStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnColumnHeaderStyleChanged)));
#else
            DependencyProperty.Register("ColumnHeaderStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new PropertyMetadata(null, new PropertyChangedCallback(OnColumnHeaderStyleChanged)));
#endif

        public static readonly DependencyProperty SummaryRowStyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("SummaryRowStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new UIPropertyMetadata(null, OlapGrid.OnSummaryRowStyleChanged));
#else
            DependencyProperty.Register("SummaryRowStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new PropertyMetadata(null, OlapGrid.OnSummaryRowStyleChanged));
#endif

        public static readonly DependencyProperty SummaryColumnStyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("SummaryColumnStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new UIPropertyMetadata(null, OlapGrid.OnSummaryColumnStyleChanged));
#else
            DependencyProperty.Register("SummaryColumnStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new PropertyMetadata(null, OlapGrid.OnSummaryColumnStyleChanged));
#endif

        public static readonly DependencyProperty RowHeaderStyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("RowHeaderStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new UIPropertyMetadata(null, OlapGrid.OnRowHeaderStyleChanged));
#else
            DependencyProperty.Register("RowHeaderStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new PropertyMetadata(null, OlapGrid.OnRowHeaderStyleChanged));
#endif

        public static readonly DependencyProperty ValueCellStyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ValueCellStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new UIPropertyMetadata(null, OlapGrid.OnValueCellStyleChanged));
#else
            DependencyProperty.Register("ValueCellStyle", typeof(OlapGridCellStyle), typeof(OlapGrid), new PropertyMetadata(null, OlapGrid.OnValueCellStyleChanged));
#endif

        public static readonly DependencyProperty FreezeHeadersProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FreezeHeaders", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(true, OlapGrid.OnFreezeHeadersChanged));
#else
            DependencyProperty.Register("FreezeHeaders", typeof(bool), typeof(OlapGrid), new PropertyMetadata(true, OlapGrid.OnFreezeHeadersChanged));
#endif

        public static readonly DependencyProperty ShowValueCellToolTipProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ShowValueCellToolTip", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(true, OlapGrid.OnShowValueCellTooltipChanged));
#else
            DependencyProperty.Register("ShowValueCellToolTip", typeof(bool), typeof(OlapGrid), new PropertyMetadata(true, OlapGrid.OnShowValueCellTooltipChanged));
#endif

#if !SILVERLIGHT
        public static readonly DependencyProperty ShowHeaderCellsToolTipProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ShowHeaderCellsToolTip", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(false, OlapGrid.OnShowHeaderCellsTooltipChanged));
#else
            DependencyProperty.Register("ShowHeaderCellsToolTip", typeof(bool), typeof(OlapGrid), new PropertyMetadata(false, OlapGrid.OnShowHeaderCellsTooltipChanged));
#endif

        public static readonly DependencyProperty ShowMemberPropertiesToolTipProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ShowMemberPropertiesToolTip", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(false, OlapGrid.OnShowMemberPropertiesToolTipChanged));
#else
            DependencyProperty.Register("ShowMemberPropertiesToolTip", typeof(bool), typeof(OlapGrid), new PropertyMetadata(false, OlapGrid.OnShowMemberPropertiesToolTipChanged));
#endif
#endif

        public static readonly DependencyProperty AllowSelectionProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("AllowSelection", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(false, OlapGrid.OnAllowSelectionChanged));
#else
            DependencyProperty.Register("AllowSelection", typeof(bool), typeof(OlapGrid), new PropertyMetadata(false, OlapGrid.OnAllowSelectionChanged));
#endif

        public static readonly DependencyProperty ValueCellTextAlignmentProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ValueCellTextAlignment", typeof(HorizontalAlignment), typeof(OlapGrid), new UIPropertyMetadata(HorizontalAlignment.Right, OlapGrid.OnValueCellTextAlignmentChanged));
#else
            DependencyProperty.Register("ValueCellTextAlignment", typeof(HorizontalAlignment), typeof(OlapGrid), new PropertyMetadata(HorizontalAlignment.Right, OlapGrid.OnValueCellTextAlignmentChanged));
#endif


        public static readonly DependencyProperty GridLineStrokeProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GridLineStroke", typeof(Brush), typeof(OlapGrid), new UIPropertyMetadata(null, OlapGrid.OnGridLineStrokeChange));
#else
            DependencyProperty.Register("GridLineStroke", typeof(Brush), typeof(OlapGrid), new PropertyMetadata(null, OlapGrid.OnGridLineStrokeChange));
#endif

        public static readonly DependencyProperty GridLineThicknessProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(OlapGrid), new UIPropertyMetadata(0.5, OlapGrid.OnGridLineThicknessChange));
#else
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(OlapGrid), new PropertyMetadata(0.5, OlapGrid.OnGridLineThicknessChange));
#endif

#if !SILVERLIGHT
        public static readonly DependencyProperty DesignerSettingsProperty =
            DependencyProperty.Register("DesignerSettings", typeof(DesignerSettings), typeof(OlapGrid), new UIPropertyMetadata(null));
#endif

        public static readonly DependencyProperty ResizeColumnsToFitProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ResizeColumnsToFit", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(true, OnResizeColumnsToFitChanged));
#else
            DependencyProperty.Register("ResizeColumnsToFit", typeof(bool), typeof(OlapGrid), new PropertyMetadata(true, OnResizeColumnsToFitChanged));
#endif

        public static readonly DependencyProperty ResizeRowsToFitProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ResizeRowsToFit", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(false, OnResizeRowsToFitChanged));
#else
            DependencyProperty.Register("ResizeRowsToFit", typeof(bool), typeof(OlapGrid), new PropertyMetadata(false, OnResizeRowsToFitChanged));
#endif

        public static readonly DependencyProperty AllowResizeColumnsProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("AllowResizeColumns", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(false));
#else
            DependencyProperty.Register("AllowResizeColumns", typeof(bool), typeof(OlapGrid), new PropertyMetadata(false));
#endif

        public static readonly DependencyProperty AllowResizeRowsProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("AllowResizeRows", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(false));
#else
            DependencyProperty.Register("AllowResizeRows", typeof(bool), typeof(OlapGrid), new PropertyMetadata(false));
#endif

        public static readonly DependencyProperty SelectedItemsProperty =
#if !SILVERLIGHT
             DependencyProperty.Register("SelectedItems", typeof(SelectedItems), typeof(OlapGrid), new UIPropertyMetadata(null, OnSelectedItemsChanged));
#else
             DependencyProperty.Register("SelectedItems", typeof(SelectedItems), typeof(OlapGrid), new PropertyMetadata(null, OnSelectedItemsChanged));
#endif

        public static readonly DependencyProperty ConditionalFormatsProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ConditionalFormats", typeof(FreezableCollection<OlapGridDataConditionalFormat>), typeof(OlapGrid));
#else
            DependencyProperty.Register("ConditionalFormats", typeof(ObservableCollection<OlapGridDataConditionalFormat>), typeof(OlapGrid), new PropertyMetadata(null)); 
#endif
        public static readonly DependencyProperty ShowConditionalFormatsProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ShowConditionalFormats", typeof(bool), typeof(OlapGrid), new UIPropertyMetadata(true, OnConditionalFormatVisibilityChanged));
#else
            DependencyProperty.Register("ShowConditionalFormats", typeof(bool), typeof(OlapGrid), new PropertyMetadata(true, OnConditionalFormatVisibilityChanged));
#endif

        public static readonly DependencyProperty ExpanderStyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ExpanderStyle", typeof(System.Windows.Style), typeof(OlapGrid), new FrameworkPropertyMetadata(null, OnStyleChanged));
#else
            DependencyProperty.Register("ExpanderStyle", typeof(System.Windows.Style), typeof(OlapGrid), new PropertyMetadata(null, OnStyleChanged));
#endif

#if !SILVERLIGHT

        public static readonly DependencyProperty CategoricalAxisProperty =
            DependencyProperty.Register("CategoricalAxis", typeof(CategoricalAxis), typeof(OlapGrid), new UIPropertyMetadata(new CategoricalAxis()));

        public static readonly DependencyProperty SeriesAxisProperty =
            DependencyProperty.Register("SeriesAxis", typeof(SeriesAxis), typeof(OlapGrid), new UIPropertyMetadata(new SeriesAxis()));

        public static readonly DependencyProperty SlicerAxisProperty =
            DependencyProperty.Register("SlicerAxis", typeof(SlicerAxis), typeof(OlapGrid), new UIPropertyMetadata(new SlicerAxis()));

        public static readonly DependencyProperty CalculatedMembersProperty =
            DependencyProperty.Register("CalculatedMembers", typeof(CalculatedMembers), typeof(OlapGrid), new UIPropertyMetadata(new CalculatedMembers()));
#endif


        /// <summary>
        /// Visual Style Dependency Property
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(OlapGridVisualStyle), typeof(OlapGrid), new PropertyMetadata(OlapGridVisualStyle.Default, new PropertyChangedCallback(OlapGrid.OnVisualStyleChanged)));
#else
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(OlapGridVisualStyle), typeof(OlapGrid), new PropertyMetadata(OlapGridVisualStyle.Default, new PropertyChangedCallback(OnVisualStyleChanged)));
#endif
        /// <summary>
        /// Selected Cell Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedCellProperty =
#if !SILVERLIGHT
    DependencyProperty.Register("SelectedCell", typeof(PivotCellDescriptor), typeof(OlapGrid), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("SelectedCell", typeof(PivotCellDescriptor), typeof(OlapGrid), new PropertyMetadata(null));
#endif
        /// <summary>
        /// GridStyleInfo Depenedency Property
        /// </summary>
        public static readonly DependencyProperty GridStyleInfoProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GridStyleInfo", typeof(ExportingGridStyleInfo), typeof(OlapGrid), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("GridStyleInfo", typeof(ExportingGridStyleInfo), typeof(OlapGrid), new PropertyMetadata(null));
#endif

        /// <summary>
        /// DataProvider Dependency Property
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty DataProviderProperty =
            DependencyProperty.Register("DataProvider", typeof(IOlapDataProvider), typeof(OlapGrid), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Service Uri Dependency Property
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ServiceUriProperty =
            DependencyProperty.Register("ServiceUri", typeof(Uri), typeof(OlapGrid), new PropertyMetadata(null, OnServiceUriChanged));
#endif

        /// <summary>
        /// Current Report Dependency Property
        /// </summary>

        public static readonly DependencyProperty CurrentReportProperty =
#if SILVERLIGHT
 DependencyProperty.Register("CurrentReport", typeof(OlapReport), typeof(OlapGrid), new PropertyMetadata(null, OnCurrentReportChanged));
#else
 DependencyProperty.Register("CurrentReport", typeof(OlapReport), typeof(OlapGrid), new UIPropertyMetadata(null, OnCurrentReportChanged));
#endif

        /// <summary>
        /// OlapDataManager Dependency Property
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty OlapDataManagerProperty =
 DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapGrid), new PropertyMetadata(null, OnOlapDataManagerChanged));
#endif

        #endregion

        #region Initilize/Finalize

        static OlapGrid()
        {
#if !SILVERLIGHT
            EnvironmentTest.ValidateLicense(typeof(OlapGrid));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapGrid), new FrameworkPropertyMetadata(typeof(OlapGrid)));

            CommandBinding dataBindCommandBinding = new CommandBinding(OlapGridCommands.DataBind, new ExecutedRoutedEventHandler(OnDataBindCommand));
            CommandBinding refreshCommandBinding = new CommandBinding(OlapGridCommands.Refresh, new ExecutedRoutedEventHandler(OnRefreshCommand));
            CommandBinding showStyleCommandBinding = new CommandBinding(OlapGridCommands.ShowStyleDialog, new ExecutedRoutedEventHandler(OnShowStyleDialogCommand));

            CommandManager.RegisterClassCommandBinding(typeof(OlapGrid), dataBindCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapGrid), refreshCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapGrid), showStyleCommandBinding);
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGrid"/> class.
        /// </summary>
        public OlapGrid()
        {
#if !SILVERLIGHT
            EnvironmentTest.ValidateLicense(typeof(OlapGrid));
#endif
            this.Loaded += new RoutedEventHandler(OlapGrid_Loaded);
#if SILVERLIGHT
            DefaultStyleKey = typeof(OlapGrid);
#endif

            this.IsDataBindCalled = false;
            
#if !SILVERLIGHT
            this.ConditionalFormats = new FreezableCollection<OlapGridDataConditionalFormat>();
#else
            this.ConditionalFormats = new ObservableCollection<OlapGridDataConditionalFormat>();
#endif

#if !SILVERLIGHT
            #region Paging Dispatcher Code

            this.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)(() =>
                {
                    if (this.InternalGrid != null)
                    {
                        if (this.InternalGrid.PagingInfo.FirstPagePreferredSize != 0 && this.InternalGrid.PagingInfo.ExpectedPageSize != 0)
                        {
                            this._opg = new OlapPagingGrid();
                            _opg.PagingInfo = new PagingInfo()
                            {
                                FirstPagePreferredSize = this.InternalGrid.PagingInfo.FirstPagePreferredSize,
                                ExpectedPageSize = this.InternalGrid.PagingInfo.ExpectedPageSize
                            };
                            _opg.Grid = this;
                            _opg.GeneratePagerGrid(0, NavigationMode.New);

                            //// Assigning the current olap paging grid instance to the internal gird's paging instance.
                            this.InternalGrid.OlapPagingGridInstance = this._opg;

                            //// Moving to first page of the paging grid.
                            this.InternalGrid.MoveTo(1);

                            //// Sets the total number of pages to the internal grid.
                            this.InternalGrid.TotalNumberOfPages = this._opg.TotalPageNumbers;
                        }
                    }
                }));

            #endregion
#endif
        }

        void OlapGrid_Loaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
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
            else if (this.OlapDataManager != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                (this.OlapDataManager as OlapDataManager).ActiveReport = this.OlapDataManager.Reports[this.ReportName];
                Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                this.InternalGrid.Engine = this.OlapDataManager.ExecuteOlapTable(cellSet, this.Layout);
                (this.OlapDataManager as OlapDataManager).ActiveReportChanged += new ActiveReportChangedEventHandler(OlapGrid_ActiveReportChanged);
            }
#endif
            if (this.OlapDataManager != null)
            {
                this.RaiseAfterDrillDown(new OlapGridDrillDownEventArgs());
            }
        }
#if !SILVERLIGHT
        void OlapGrid_ActiveReportChanged(object sender, ActiveReportChangedEventArgs e)
        {
            if (e.NewActiveReport != null && e.NewActiveReport.Name == this.ReportName && e.IsReportChanged)
            {
                Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                this.InternalGrid.Engine = this.OlapDataManager.ExecuteOlapTable(cellSet, this.Layout);
            }
        }
#endif

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [after refresh].
        /// </summary>
        public event OlapGridDrillDownEventHander AfterRefresh;

        /// <summary>
        /// Occurs when [before refresh].
        /// </summary>
        public event OlapGridDrillDownEventHander BeforeRefresh;

        /// <summary>
        /// Occurs when [link click].
        /// </summary>
        public event LinkLabelClickEventHander LinkClick;

        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public virtual event SelectionChanged SelectionChanged;

        #endregion

        #region Properties
#if !SILVERLIGHT

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
        [Browsable(false)]
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
        
#endif
        /// <summary>
        /// Gets or sets the Expander Style.
        /// </summary>
        /// <value>The expander style.</value>
        [Category("Style")]
        public System.Windows.Style ExpanderStyle
        {
            get { return (System.Windows.Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the grid area.
        /// </summary>
        /// <value>The grid area.</value>
        [Browsable(false)]
        public UIElement GridArea
        {
            get;
            set;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the cube model.
        /// </summary>
        /// <value>The cube model.</value>
        [Browsable(false)]
        public IOlapDataManager OlapDataManager
        {
            get { return (IOlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }
#else

        //public OlapDataManager OlapDataManager
        //{
        //    get
        //    {
        //        return _olapDataManager;
        //    }
        //    set
        //    {
        //        _olapDataManager = value;

        //        if (_olapDataManager != null)
        //        {
        //            this.WireEvent();
        //        }
        //    }
        //}

        /// <summary>
        /// Gets or sets OlapDataManager.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [Browsable(false)]
        public OlapDataManager OlapDataManager
        {
            get { return (OlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }


        /// <summary>
        /// Wires the event.
        /// </summary>
        private void WireEvent()
        {
            this.OlapDataManager.ReportChanged -= OlapDataManager_ReportChanged;
            this.OlapDataManager.ReportChanged += OlapDataManager_ReportChanged;
            this.OlapDataManager.CellSetChanged -= new CellSetChangedEventHandler(OlapDataManager_CellSetChanged);
            this.OlapDataManager.CellSetChanged += new CellSetChangedEventHandler(OlapDataManager_CellSetChanged);
        }

        /// <summary>
        /// Handles the CellSetChanged event of the OlapDataManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.CellSetChangedEventArgs"/> instance containing the event data.</param>
        void OlapDataManager_CellSetChanged(object sender, CellSetChangedEventArgs e)
        {
            if (this.OlapDataManager != null)
            {
                this.OlapDataManager.ExecuteOlapTable(e.NewCellSet, this.Layout);
                if (this.OlapDataManager.PivotEngine != null && this.InternalGrid != null)
                {
                    this.InternalGrid.DataManager = this.OlapDataManager;
                    this.RaiseAfterDrillDown(new OlapGridDrillDownEventArgs() { CellDescriptor = this.InternalGrid.CurrentCellDescriptor });
                    this.IsProcessing = false;
                    //if ((this.ColumnHeaderStyle.TextWrapping == TextWrapping.Wrap && !this.ResizeColumnsToFit ) || (this.RowHeaderStyle.TextWrapping == TextWrapping.Wrap && !this.ResizeRowsToFit))
                        //this.SetRowHeightOnTextWrapping();
                }
                else
                    this.IsProcessing = false;

            }
        }

        private void SetRowHeightOnTextWrapping()
        {
            var m_lengthiestWord = string.Empty;
            double columnWidth = this.InternalGrid.ColumnWidths[0]; double rowHeight = this.InternalGrid.RowHeights[0];
            foreach (PivotColumnDescriptor pvtColumnDescriptor in this.OlapDataManager.PivotEngine.TableColumns)
            {
                foreach (PivotCellDescriptor pvtCellDescriptor in pvtColumnDescriptor.Cells)
                {
                    if ((pvtCellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader && pvtCellDescriptor.Range.Width == 1) || (pvtCellDescriptor.CellType == PivotCellDescriptorType.RowHeader && pvtCellDescriptor.Range.Height == 1))
                        m_lengthiestWord = m_lengthiestWord.Length > pvtCellDescriptor.CellValue.Length ? m_lengthiestWord : pvtCellDescriptor.CellValue;
                }
            }
            double width = this.GetColumnHeaderTextWidth(m_lengthiestWord);
            if (width > columnWidth)
            {
                for (int i = 0; i < this.InternalGrid.Model.RowCount; i++)
                    this.InternalGrid.SetRowHeight(i, Math.Ceiling(width / columnWidth) * rowHeight);
            }
        }

        private double GetColumnHeaderTextWidth(string text)
        {
            TextBlock txtMeasure = new TextBlock();
            txtMeasure.Text = text;
            txtMeasure.FontSize = this.ColumnHeaderStyle.FontSize;
            txtMeasure.FontFamily = this.ColumnHeaderStyle.FontFamily;
            txtMeasure.FontWeight = this.ColumnHeaderStyle.FontWeight;
            double width = txtMeasure.ActualWidth;
            return width;
        }
        internal double GetRowHeaderTextWidth(string text)
        {
            TextBlock txtMeasure = new TextBlock();
            txtMeasure.Text = text;
            txtMeasure.FontSize = this.RowHeaderStyle.FontSize;
            txtMeasure.FontFamily = this.RowHeaderStyle.FontFamily;
            txtMeasure.FontWeight = this.RowHeaderStyle.FontWeight;
            double width = txtMeasure.ActualWidth;
            return width;
        }

        /// <summary>
        /// Show/Hide Grid ProcessingBar
        /// </summary>
        /// <value><c>true</c> if [show processing bar]; otherwise, <c>false</c>.</value>
        public bool ShowProcessingBar 
        { 
            get
            {
                return m_ShowProcessingBar;
            }
            set
            {
                m_ShowProcessingBar = value;
            }
        }

        ///<summary>
        ///Gets or Sets the Temporary Report
        /// </summary>
        internal OlapReport DefaultReport { get; set; }

        /// <summary>
        /// Progress bar popup
        /// </summary>
        internal ProgressBar GridProgressBar { get; set; }

        /// <summary>
        /// Grid processing progress bar
        /// </summary>
        internal Popup GridProgressBarPopup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether grid controls is processing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing; otherwise, <c>false</c>.
        /// </value>
        internal bool IsProcessing
        {
            get
            {
                if (this.GridProgressBar != null)
                    return this.GridProgressBar.IsIndeterminate;
                return false;
            }
            set
            {
                if (GridProgressBarPopup != null)
                {
                    try
                    {
                        if (ShowProcessingBar)
                        {
                            ////if its processing and ShowProcessingBar is true displaying the processing dialog
                            GridProgressBarPopup.IsOpen = value;
                            Storyboard animation1 = GridProgressBarPopup.Resources["Storyboard1"] as Storyboard;
                            animation1.Begin();
                            Storyboard animation2 = GridProgressBarPopup.Resources["Storyboard2"] as Storyboard;
                            animation2.Begin();
                        }

                        if (this.GridProgressBar != null)
                            this.GridProgressBar.IsIndeterminate = value;
                        if (value)
                        {
                            if (this != null)
                            {
                                this.IsEnabled = false;
                            }
                        }
                        else
                        {
                            if (this != null)
                            {
                                this.IsEnabled = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

#endif
        /// <summary>
        /// Gets or sets the internal grid.
        /// </summary>
        /// <value>The internal grid.</value>
        [Browsable(false)]
        public OlapGridBase InternalGrid
        {
            get;
            set;
        }
             
        /// <summary>
        /// Gets or sets the grid style info.
        /// </summary>
        /// <value>The grid style info.</value>
        [Browsable(false)]
        public ExportingGridStyleInfo GridStyleInfo
        {
            get { return (ExportingGridStyleInfo)GetValue(GridStyleInfoProperty); }
            set { SetValue(GridStyleInfoProperty, value); }
        }




        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>The window.</value>
        [Browsable(false)]
        internal FormattingWindow Window
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is data bind called.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is data bind called; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        internal bool IsDataBindCalled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        [Category("Layout")]
        public GridLayout Layout
        {
            get
            {
                return (GridLayout)GetValue(LayoutProperty);
            }
            set
            {
                SetValue(LayoutProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the column header style.
        /// </summary>
        /// <value>The column header style.</value>
        [Browsable(false)]
        public OlapGridCellStyle ColumnHeaderStyle
        {
            get { return (OlapGridCellStyle)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the summary row style.
        /// </summary>
        /// <value>The summary row style.</value>
        [Browsable(false)]
        public OlapGridCellStyle SummaryRowStyle
        {
            get { return (OlapGridCellStyle)GetValue(SummaryRowStyleProperty); }
            set { SetValue(SummaryRowStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the summary column style.
        /// </summary>
        /// <value>The summary column style.</value>
        [Browsable(false)]
        public OlapGridCellStyle SummaryColumnStyle
        {
            get { return (OlapGridCellStyle)GetValue(SummaryColumnStyleProperty); }
            set { SetValue(SummaryColumnStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the row header style.
        /// </summary>
        /// <value>The row header style.</value>
        [Browsable(false)]
        public OlapGridCellStyle RowHeaderStyle
        {
            get { return (OlapGridCellStyle)GetValue(RowHeaderStyleProperty); }
            set { SetValue(RowHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value cells style.
        /// </summary>
        /// <value>The value cells style.</value>
        [Browsable(false)]
        public OlapGridCellStyle ValueCellStyle
        {
            get { return (OlapGridCellStyle)GetValue(ValueCellStyleProperty); }
            set { SetValue(ValueCellStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [freeze headers].
        /// </summary>
        /// <value><c>true</c> if [freeze headers]; otherwise, <c>false</c>.</value>
        [Category("Cell")]
        public bool FreezeHeaders
        {
            get { return (bool)GetValue(FreezeHeadersProperty); }
            set { SetValue(FreezeHeadersProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show value cells tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show value cells tool tip]; otherwise, <c>false</c>.
        /// </value>
        [Category("Cell")]
        public bool ShowValueCellToolTip
        {
            get { return (bool)GetValue(ShowValueCellToolTipProperty); }
            set { SetValue(ShowValueCellToolTipProperty, value); }
        }

#if SILVERLIGHT
        /// <summary>
        /// Gets or Sets the Service Path for OlapGrid
        /// </summary>
        [Browsable(false)]
        public Uri ServiceUri
        {
            get { return (Uri)GetValue(ServiceUriProperty); }
            set { SetValue(ServiceUriProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the CurrentReport for OlapDataManager
        /// </summary>
        [Browsable(false)]
        public OlapReport CurrentReport
        {
            get { return (OlapReport)GetValue(CurrentReportProperty); }
            set { SetValue(CurrentReportProperty, value); }
        }


        /// <summary>
        /// Gets or Sets DataProvider
        /// </summary>
        [Browsable(false)]
        public IOlapDataProvider DataProvider
        {
            get { return (IOlapDataProvider)GetValue(DataProviderProperty); }
            set { SetValue(DataProviderProperty, value); }
        }
#else
        /// <summary>
        /// Gets or Sets the CurrentReport for OlapDataManager
        /// </summary>
        public OlapReport CurrentReport
        {
            get { return (OlapReport)GetValue(CurrentReportProperty); }
            set { SetValue(CurrentReportProperty, value); }
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether [show header cells tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show header cells tool tip]; otherwise, <c>false</c>.
        /// </value>
        [Category("Cell")]
        public bool ShowHeaderCellsToolTip
        {
            get { return (bool)GetValue(ShowHeaderCellsToolTipProperty); }
            set
            {
                SetValue(ShowHeaderCellsToolTipProperty, value);
                if (value)
                {
                    this.ShowMemberPropertiesToolTip = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show member properties tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show member properties tool tip]; otherwise, <c>false</c>.
        /// </value>
        [Category("Cell")]
        public bool ShowMemberPropertiesToolTip
        {
            get { return (bool)GetValue(ShowMemberPropertiesToolTipProperty); }
            set
            {
                SetValue(ShowMemberPropertiesToolTipProperty, value);
                if (value)
                {
                    this.ShowHeaderCellsToolTip = false;
                }
            }
        }

#endif

        /// <summary>
        /// Gets or sets a value indicating whether [allow selection].
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        [Category("Cell Selection")]
        public bool AllowSelection
        {
            get { return (bool)GetValue(AllowSelectionProperty); }
            set { SetValue(AllowSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Selected items which is an IEnumerable collection of Columns, Rows and Value.
        /// </summary>
        /// <value>The selected items.</value>
        [Browsable(false)]
        public SelectedItems SelectedItems
        {
            get { return (SelectedItems)GetValue(SelectedItemsProperty); }
            internal set { SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value cell text alignment.
        /// </summary>
        /// <value>The value cell text alignment.</value>
        [Category("Cell")]
        public HorizontalAlignment ValueCellTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(ValueCellTextAlignmentProperty); }
            set { SetValue(ValueCellTextAlignmentProperty, value); }
        }


        /// <summary>
        /// Gets or sets the GridLineStroke.
        /// </summary>
        /// <value>The GridLineStroke.</value>
        [Category("Appearance")]
        public Brush GridLineStroke
        {
            get
            {
                return (Brush)GetValue(GridLineStrokeProperty);
            }
            set
            {
                SetValue(GridLineStrokeProperty, value);                
            }
        }

        /// <summary>
        /// Gets or sets the border thickness of a control.
        /// </summary>
        /// <value></value>
        /// <returns>A thickness value; the default is a thickness of 0 on all four sides.</returns>
        [Category("Appearance")]
        public double GridLineThickness
        {
            get
            {
                return (double)GetValue(GridLineThicknessProperty);
            }
            set
            {
                SetValue(GridLineThicknessProperty, value);
            }
        }
        
#if SILVERLIGHT
        /// <summary>
        /// Gets or Sets the VisualStyle of OlapGrid
        /// </summary>
        /// <value>The Visual Style</value>
        [Category("Appearance")]
        public OlapGridVisualStyle VisualStyle
        {
            get
            {
                return (OlapGridVisualStyle)GetValue(VisualStyleProperty);
            }
            set
            {
                SetValue(VisualStyleProperty, value);
            }
        }
#else
        public OlapGridVisualStyle VisualStyle
        {
            get { return (OlapGridVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
#endif

        /// <summary>
        /// Gets or Sets the SelecedCell of OlapGrid
        /// </summary>
        /// <value>Selected Cell</value>
        [Category("Cell")]
        [Browsable(false)]
        public PivotCellDescriptor SelectedCell
        {
            get
            {
                return (PivotCellDescriptor)GetValue(SelectedCellProperty);
            }
            set
            {
                SetValue(SelectedCellProperty, value);
            }
        }


#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the designer settings.
        /// </summary>
        /// <value>The designer settings.</value>
        [Browsable(false)]
        public DesignerSettings DesignerSettings
        {
            get { return (DesignerSettings)GetValue(DesignerSettingsProperty); }
            set { SetValue(DesignerSettingsProperty, value); }
        }
#endif
        /// <summary>
        /// Gets or sets a value indicating whether to resize columns to fit.
        /// </summary>
        /// <value><c>true</c> if [resize columns to fit]; otherwise, <c>false</c>.</value>
        [Category("Resize")]
        public bool ResizeColumnsToFit
        {
            get { return (bool)GetValue(ResizeColumnsToFitProperty); }
            set { SetValue(ResizeColumnsToFitProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to resize rows to fit.
        /// </summary>
        /// <value><c>true</c> if [resize rows to fit]; otherwise, <c>false</c>.</value>    
        [Category("Resize")]
        public bool ResizeRowsToFit
        {
            get { return (bool)GetValue(ResizeRowsToFitProperty); }
            set { SetValue(ResizeRowsToFitProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [allow resize columns].
        /// </summary>
        /// <value><c>true</c> if [allow resize columns]; otherwise, <c>false</c>.</value>
        [Category("Resize")]
        public bool AllowResizeColumns
        {
            get { return (bool)GetValue(AllowResizeColumnsProperty); }
            set { SetValue(AllowResizeColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow resize rows].
        /// </summary>
        /// <value><c>true</c> if [allow resize rows]; otherwise, <c>false</c>.</value>
        [Category("Resize")]
        public bool AllowResizeRows
        {
            get { return (bool)GetValue(AllowResizeRowsProperty); }
            set { SetValue(AllowResizeRowsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable context menu for column header.
        /// NOTE: Applicable only for OLAP cube information.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable column header context menu]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool EnableColumnHeaderContextMenu
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable context menu for row header.
        /// NOTE: Applicable only for OLAP cube information.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable row header context menu]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool EnableRowHeaderContextMenu
        {
            get;
            set;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the conditional formats to be applied for OlapGrid.
        /// </summary>
        /// <value>The conditional formats.</value>
        [Category("Style")]
        public FreezableCollection<OlapGridDataConditionalFormat> ConditionalFormats
        {
            get
            {
                return (FreezableCollection<OlapGridDataConditionalFormat>)GetValue(OlapGrid.ConditionalFormatsProperty);
            }
            set
            {
                SetValue(OlapGrid.ConditionalFormatsProperty, value);
            }
        }
#else
        /// <summary>
        /// Gets or sets the conditional formats to be applied for OlapGrid.
        /// </summary>
        /// <value>The conditional formats.</value>
        [Category("Style")]
        public ObservableCollection<OlapGridDataConditionalFormat> ConditionalFormats
        {
            get
            {
                return (ObservableCollection<OlapGridDataConditionalFormat>)GetValue(OlapGrid.ConditionalFormatsProperty);
            }
            set
            {
                SetValue(OlapGrid.ConditionalFormatsProperty, value);
            }
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether [show conditional formats].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show conditional formats]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowConditionalFormats
        {
            get { return (bool)GetValue(ShowConditionalFormatsProperty); }
            set { SetValue(ShowConditionalFormatsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the grid settings.
        /// </summary>
        /// <value>The grid settings.</value>
        [Browsable(false)]
        public GridAppearanceSettings GridSettings
        {
            get
            {
                return GetGridAppearanceSettings();
            }
            set
            {
                SetGridAppearanceSettings(value);
            }

        }
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Called when [cube model changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnOlapDataManagerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && !DesignerProperties.GetIsInDesignMode(olapGrid))
            {
#if SILVERLIGHT
                if (e.NewValue != null)
                {
                    olapGrid.IsProcessing = true;
                    olapGrid.WireEvent();
                }
#else
                if (e.NewValue != null)
                {
                    olapGrid.OlapDataManager.ReportChanged -= olapGrid.OlapDataManager_ReportChanged;
                    olapGrid.OlapDataManager.ReportChanged += olapGrid.OlapDataManager_ReportChanged;
                }
#endif
                if (olapGrid.InternalGrid != null)
                {
                    olapGrid.InternalGrid.DataManager = e.NewValue as OlapDataManager;
                }
            }
        }

        private void OlapDataManager_ReportChanged(object sender, ReportChangedEventArgs e)
        {
            this.GridSettings = e.NewReport.GridSettings;  
        }

        public static void OnGridLayoutChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.Layout = (GridLayout)e.NewValue;
                olapGrid.DataBind();
            }
        }

        public static void OnFreezeHeadersChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.FreezeHeaders = (bool)e.NewValue;
            }
        }

        public static void OnShowValueCellTooltipChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ShowValueCellToolTip = (bool)e.NewValue;
                olapGrid.InternalGrid.InvalidateCells();
                olapGrid.DataBind();
            }
        }

#if !SILVERLIGHT
        public static void OnShowHeaderCellsTooltipChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ShowHeaderCellsToolTip = (bool)e.NewValue;
                olapGrid.InternalGrid.InvalidateCells();
                olapGrid.DataBind();
            }
        }

        public static void OnShowMemberPropertiesToolTipChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ShowMemberPropertiesToolTip = (bool)e.NewValue;
                olapGrid.InternalGrid.InvalidateCells();
            }
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Called when [Visual Style Changed]
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnVisualStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null)
            {
                string visualstyle = string.Empty;
                ResourceDictionary resource = new ResourceDictionary();

                switch ((OlapGridVisualStyle)e.NewValue)
                {
                    case OlapGridVisualStyle.Default:
                        visualstyle = "Classic.";
                        resource.Source = new Uri("/Syncfusion.OlapGrid.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                        olapGrid.FontSize = 11;
                        olapGrid.Background = resource[visualstyle + "GridBackground"] as SolidColorBrush;
                        break;
                    case OlapGridVisualStyle.Blend:
                    case OlapGridVisualStyle.Office2007Black:
                    case OlapGridVisualStyle.Office2007Blue:
                    case OlapGridVisualStyle.Office2007Silver:
                    case OlapGridVisualStyle.Office2010Black:
                    case OlapGridVisualStyle.Office2010Blue:
                    case OlapGridVisualStyle.Office2010Silver:
                    case OlapGridVisualStyle.Transparent:
                        visualstyle = e.NewValue + ".";
                        resource.Source = new Uri("/Syncfusion.OlapGrid.Silverlight;component/Themes/" + e.NewValue + "Style.xaml", UriKind.RelativeOrAbsolute);
                        olapGrid.Background = resource[e.NewValue + "GridBackground"] as SolidColorBrush;
                        break;
                    case OlapGridVisualStyle.Metro:
                        {
                            resource.Source = new Uri("/Syncfusion.OlapGrid.Silverlight;component/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                            olapGrid.Background = resource["MetroGridBackground"] as SolidColorBrush;
                            olapGrid.ColumnHeaderStyle = new OlapGridCellStyle() { Background = resource["MetroColumnHeaderBackground"] as SolidColorBrush, Foreground = resource["MetroColumnHeaderForeground"] as SolidColorBrush };
                            olapGrid.RowHeaderStyle = new OlapGridCellStyle() { Background = resource["MetroRowHeaderBackground"] as SolidColorBrush, Foreground = resource["MetroRowHeaderForeground"] as SolidColorBrush };
                            olapGrid.ExpanderStyle = resource["ExpanderStyle"] as Style;
                            olapGrid.ValueCellStyle = new OlapGridCellStyle() { Background = resource["MetroValueCellBackground"] as SolidColorBrush, Foreground = resource["MetroValueCellForeground"] as SolidColorBrush };
                            olapGrid.SummaryColumnStyle = new OlapGridCellStyle() { Background = resource["MetroSummaryHeaderBackground"] as SolidColorBrush, Foreground = resource["MetroSummaryHeaderForeground"] as SolidColorBrush };
                            olapGrid.SummaryRowStyle = new OlapGridCellStyle() { Background = resource["MetroSummaryHeaderBackground"] as SolidColorBrush, Foreground = resource["MetroSummaryHeaderForeground"] as SolidColorBrush };
                            olapGrid.GridLineStroke = resource["MetroBorderBrush"] as SolidColorBrush;
                        }
                        break;
                    default:
                        visualstyle = e.NewValue.ToString();
                        break;
                }
                if ((OlapGridVisualStyle)e.NewValue != OlapGridVisualStyle.Metro)
                {
                    olapGrid.ExpanderStyle = resource["ExpanderStyle"] as Style;
                    if (resource[visualstyle + "ColumnHeaderBackgroundBrush"] is LinearGradientBrush)
                        olapGrid.ColumnHeaderStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "ColumnHeaderBackgroundBrush"] as LinearGradientBrush, Foreground = resource[visualstyle + "ColumnHeaderForegroundBrush"] as SolidColorBrush };
                    else
                        olapGrid.ColumnHeaderStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "ColumnHeaderBackgroundBrush"] as SolidColorBrush, Foreground = resource[visualstyle + "ColumnHeaderForegroundBrush"] as SolidColorBrush };
                    olapGrid.RowHeaderStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "RowHeaderBackgroundBrush"] as SolidColorBrush, Foreground = resource[visualstyle + "RowHeaderForegroundBrush"] as SolidColorBrush };
                    olapGrid.ValueCellStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "ValueCellBackgroundBrush"] as SolidColorBrush, Foreground = resource[visualstyle + "ValueCellForegroundBrush"] as SolidColorBrush };
                    if (resource[visualstyle + "SummaryColumnBackgroundBrush"] is LinearGradientBrush)
                        olapGrid.SummaryColumnStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "SummaryColumnBackgroundBrush"] as LinearGradientBrush, Foreground = resource[visualstyle + "SummaryColumnForegroundBrush"] as SolidColorBrush };
                    else
                        olapGrid.SummaryColumnStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "SummaryColumnBackgroundBrush"] as SolidColorBrush, Foreground = resource[visualstyle + "SummaryColumnForegroundBrush"] as SolidColorBrush };

                    if (resource[visualstyle + "SummaryRowBackgroundBrush"] is LinearGradientBrush)
                        olapGrid.SummaryRowStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "SummaryRowBackgroundBrush"] as LinearGradientBrush, Foreground = resource[visualstyle + "SummaryRowForegroundBrush"] as SolidColorBrush };
                    else
                        olapGrid.SummaryRowStyle = new OlapGridCellStyle() { Background = resource[visualstyle + "SummaryRowBackgroundBrush"] as SolidColorBrush, Foreground = resource[visualstyle + "SummaryRowForegroundBrush"] as SolidColorBrush };
                    olapGrid.GridLineStroke = resource[visualstyle + "GridLineStroke"] as SolidColorBrush;
                }

            }
        }


        /// <summary>
        /// Called when [Service Uri changed]
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnServiceUriChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null)
            {
                System.ServiceModel.Channels.Binding customBinding = new System.ServiceModel.Channels.CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement { MaxReceivedMessageSize = 2147483647 });
                EndpointAddress address = new EndpointAddress((Uri)e.NewValue);
                ChannelFactory<IOlapDataProvider> channel = new ChannelFactory<IOlapDataProvider>(customBinding, address);
                olapGrid.DataProvider = channel.CreateChannel();
                if (olapGrid.DefaultReport != null)
                {
                    OlapDataManager olapDataManager = new OlapDataManager();
                    olapDataManager.DataProvider = olapGrid.DataProvider;
                    olapDataManager.SetCurrentReport(olapGrid.DefaultReport);
                    olapGrid.OlapDataManager = olapDataManager;
                    olapGrid.DataBind();
                }
            }
        }

        /// <summary>
        /// Called when [Current Report changed]
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCurrentReportChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.DataProvider != null)
            {
                OlapDataManager olapDataManager = new OlapDataManager();
                olapDataManager.DataProvider = olapGrid.DataProvider;
                olapDataManager.SetCurrentReport((OlapReport)e.NewValue);
                olapGrid.OlapDataManager = olapDataManager;
                olapGrid.DataBind();
            }
            else
                olapGrid.DefaultReport = (OlapReport)e.NewValue;
        }
#else
        /// <summary>
        /// Called when [Visual Style Changed]
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnVisualStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null)
            {
                SkinStorage.SetVisualStyle(olapGrid, e.NewValue.ToString());
            }
        }

        /// <summary>
        /// Called when [Current Report changed]
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCurrentReportChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null)
            {
                OlapDataManager olapDataManager = new OlapDataManager();
                olapDataManager.SetCurrentReport((OlapReport)e.NewValue);
                olapGrid.OlapDataManager = olapDataManager;
                olapGrid.DataBind();
            }
        }
#endif
#if !SILVERLIGHT
        private static FreezableCollection<OlapGridDataConditionalFormat> _savedConditionalFormats; 
#else
        private static ObservableCollection<OlapGridDataConditionalFormat> _savedConditionalFormats; 
#endif
        static void OnConditionalFormatVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapGrid olapGrid = d as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null && olapGrid.ConditionalFormats != null && args.NewValue is bool)
            {
                if (_savedConditionalFormats == null)
                {
#if !SILVERLIGHT
                    _savedConditionalFormats = new FreezableCollection<OlapGridDataConditionalFormat>();
#else
                    _savedConditionalFormats = new ObservableCollection<OlapGridDataConditionalFormat>();
#endif
                }

                if ((bool)args.NewValue)
                {
                    olapGrid.ConditionalFormats = _savedConditionalFormats;
                }
                else
                {
                    _savedConditionalFormats = olapGrid.ConditionalFormats;
#if !SILVERLIGHT
                    olapGrid.ConditionalFormats = new FreezableCollection<OlapGridDataConditionalFormat>();
#else
                    olapGrid.ConditionalFormats = new ObservableCollection<OlapGridDataConditionalFormat>(); 
#endif
                }

                olapGrid.InternalGrid.InvalidateCells();
            }
        }

        public static void OnAllowSelectionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.AllowSelection = (bool)e.NewValue;
                olapGrid.InternalGrid.InvalidateCells();
            }
        }

        public static void OnColumnHeaderStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ColumnHeaderStyle = e.NewValue as OlapGridCellStyle;
                olapGrid.ColumnHeaderStyle.InternalSourceStyle = olapGrid.InternalGrid.ColumnHeaderStyle;
            }
        }

        public static void OnRowHeaderStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.RowHeaderStyle = e.NewValue as OlapGridCellStyle;
                olapGrid.RowHeaderStyle.InternalSourceStyle = olapGrid.InternalGrid.RowHeaderStyle;
            }
        }

        public static void OnSummaryRowStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.SummaryRowStyle = e.NewValue as OlapGridCellStyle;
                olapGrid.SummaryRowStyle.InternalSourceStyle = olapGrid.InternalGrid.SummaryRowStyle;
            }
        }

        public static void OnSummaryColumnStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.SummaryColumnStyle = e.NewValue as OlapGridCellStyle;
                olapGrid.SummaryColumnStyle.InternalSourceStyle = olapGrid.InternalGrid.SummaryColumnStyle;
            }
        }

        public static void OnValueCellStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependencyObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ValueCellStyle = e.NewValue as OlapGridCellStyle;
                olapGrid.ValueCellStyle.InternalSourceStyle = olapGrid.InternalGrid.ValueCellStyle;
            }
        }

        public static void OnValueCellTextAlignmentChanged(DependencyObject dependectObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependectObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ValueCellTextAlignment = (HorizontalAlignment)e.NewValue;
                olapGrid.InternalGrid.InvalidateCells();
            }
        }

        public static void OnGridLineStrokeChange(DependencyObject dependectObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependectObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.GridLineStroke = (Brush)e.NewValue;
            }
        }

        public static void OnGridLineThicknessChange(DependencyObject dependectObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependectObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.GridLineThickness = (double)e.NewValue;
            }
        }

        public static void OnResizeColumnsToFitChanged(DependencyObject dependectObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependectObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ResizeColumnsToFit = (bool)e.NewValue;
            }
        }

        public static void OnResizeRowsToFitChanged(DependencyObject dependectObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGrid olapGrid = dependectObject as OlapGrid;
            if (olapGrid != null && olapGrid.InternalGrid != null)
            {
                olapGrid.InternalGrid.ResizeRowsToFit = (bool)e.NewValue;
            }
        }

        public static void OnSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            OlapGrid gridControl = (OlapGrid)dependencyObject;
            if (gridControl != null)
            {
                gridControl.SelectedItems = gridControl.InternalGrid.SelectedItems;
            }
        }

        public static void OnStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            OlapGrid gridControl = (OlapGrid)dependencyObject;
            if (gridControl.InternalGrid != null)
            {
                gridControl.InternalGrid.InvalidateCells();
            }
        }

        #endregion

        #region Static Methods
#if !SILVERLIGHT

        /// <summary>
        /// Calls DataBind method of OlapGrid
        /// </summary>
        /// <param name="target">OlapGrid</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnDataBindCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapGrid olapGrid = target as OlapGrid;
            if (olapGrid != null)
            {
                olapGrid.DataBind();
            }
        }

        /// <summary>
        /// Calls Refresh method of OlapGrid
        /// </summary>
        /// <param name="target">OlapGrid</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnRefreshCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapGrid olapGrid = target as OlapGrid;
            if (olapGrid != null)
            {
                olapGrid.Refresh();
            }
        }

        /// <summary>
        /// Opens the ShowStyleDialog window
        /// </summary>
        /// <param name="target">OlapGrid</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnShowStyleDialogCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapGrid olapGrid = target as OlapGrid;
            if (olapGrid != null)
            {
                olapGrid.ShowStyleDialog();
            }
        }
#endif
        #endregion

        #region Public Methods

        /// <summary>
        /// Binds a data source to the invoked control and all its child controls.
        /// </summary>
        public void DataBind()
        {
#if SILVERLIGHT
            this.IsProcessing = true;
#else     
            if (this.OlapDataManager != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager && this.InternalGrid != null)
            {
                PivotEngine _engine = null;
                if (this.OlapDataManager.ItemSource == null)
                {
                    Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                    _engine = this.OlapDataManager.ExecuteOlapTable(cellSet, this.Layout);
                }
                else
                {
                    _engine = this.OlapDataManager.ExecuteOlapTable(this.Layout);
                }
                this.InternalGrid.DataBind(_engine);
            }
            else
            {
#endif
                if (this.InternalGrid != null)
                {
                    this.InternalGrid.DataBind();
                }
                else
                {
                    IsDataBindCalled = true;
                }
#if !SILVERLIGHT
            }
#endif
        }

        public void Refresh()
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.InvalidateCells();
                this.InternalGrid.RaiseAfterRefresh(new OlapGridDrillDownEventArgs());
            }
            
        }

#if !SILVERLIGHT
        /// <summary>
        /// Displays a dialog which provides option to change the appearance of Grid
        /// </summary>
        public void ShowStyleDialog()
        {
            this.GridStyleInfo = this.GridStyleInfo ?? new ExportingGridStyleInfo();
            FormattingWindow window = new FormattingWindow(this);
            SkinStorage.SetVisualStyle(window, SkinStorage.GetVisualStyle(this));
            window.ShowDialog();
        }
        
        [Obsolete("Use ShowStyleDialog() method.")]
        /// <summary>
        /// Displays a dialog which provides option to change the appearance of Grid
        /// </summary>
        /// <param name="skin">Skin for style dialog </param>
        public void ShowStyleDialog(object skin)
        {
            this.GridStyleInfo = this.GridStyleInfo ?? new ExportingGridStyleInfo();
            FormattingWindow window = new FormattingWindow(this);
            SkinStorage.SetVisualStyle(window, SkinStorage.GetVisualStyle(this));
            window.ShowDialog();
        }

#else
        /// <summary>
        /// Displays a dialog which provides option to change the appearance of Grid
        /// </summary>
        public void ShowStyleDialog()
        {
            this.GridStyleInfo = this.GridStyleInfo ?? new ExportingGridStyleInfo();
            Window = new FormattingWindow(this);
            Window.DefaultStyles();
            Window.Show();
        }

        #endif

        #endregion

#if !SILVERLIGHT
        #region ICommand Implemantation

        public static readonly DependencyProperty CommandParameterProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("CommandParameter", typeof(object[]), typeof(OlapGrid));
#else
            DependencyProperty.Register("CommandParameter",typeof(object[]),typeof(OlapGrid),new PropertyMetadata(null));
#endif


        public static readonly DependencyProperty CommandProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Command", typeof(ICommand), typeof(OlapGrid));
#else
            DependencyProperty.Register("Command", typeof(ICommand), typeof(OlapGrid),new PropertyMetadata(null));
#endif


#if !SILVERLIGHT
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(OlapGrid));
#endif
        
#if !SILVERLIGHT
        [Localizability(LocalizationCategory.NeverLocalize), Category("Action"), Bindable(true)]
#endif
        [Browsable(false)]
        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }

        }

        [Bindable(true), Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
        [Browsable(false)]
        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }

#if !SILVERLIGHT
        [Category("Action"), Bindable(true)]
        [Browsable(false)]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)base.GetValue(CommandTargetProperty);
            }
            set
            {
                base.SetValue(CommandTargetProperty, value);
            }
        }

#endif
        #endregion
#endif

        #region On Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //this.GridArea = GetTemplateChild("PART_GridArea") as UIElement;
            //RefreshCommand = new DelegateCommand(new Action<object>(ExecuteRefresh), new Predicate<object>(CanExecuteRefresh));
            this.InternalGrid = GetTemplateChild("PART_OlapGridBase") as OlapGridBase;
#if SILVERLIGHT
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.GridProgressBar = GetTemplateChild("PART_OlapGridProgressbar") as ProgressBar;
                this.GridProgressBarPopup = GetTemplateChild("PART_OlapGridProgressbarPopup") as Popup;
                if(this.OlapDataManager != null && !this.OlapDataManager.IsProcessing)
                this.IsProcessing = true;
            }
#endif
            if (this.InternalGrid != null)
            {
                this.InternalGrid.OlapGrid = this;
                this.InternalGrid.FreezeHeaders = this.FreezeHeaders;
                this.InternalGrid.Layout = this.Layout;
                this.InternalGrid.ShowValueCellToolTip = this.ShowValueCellToolTip;
#if !SILVERLIGHT
                this.InternalGrid.ShowHeaderCellsToolTip = this.ShowHeaderCellsToolTip;
                this.InternalGrid.ShowMemberPropertiesToolTip = this.ShowMemberPropertiesToolTip;
#endif
                this.InternalGrid.ResizeColumnsToFit = this.ResizeColumnsToFit;
                this.InternalGrid.ResizeRowsToFit = this.ResizeRowsToFit;
            
                this.InternalGrid.ValueCellTextAlignment = this.ValueCellTextAlignment;
                

                this.InternalGrid.AllowSelection = this.AllowSelection;
                this.InternalGrid.GridLineStroke = this.GridLineStroke;
                this.InternalGrid.GridLineThickness = this.GridLineThickness;

                this.InternalGrid.ColumnHeaderStyle = this.ColumnHeaderStyle;
                if (this.ColumnHeaderStyle != null)
                {
                    this.ColumnHeaderStyle.InternalSourceStyle = this.InternalGrid.ColumnHeaderStyle;
                }

                this.InternalGrid.RowHeaderStyle = this.RowHeaderStyle;
                if (this.RowHeaderStyle != null)
                {
                    this.RowHeaderStyle.InternalSourceStyle = this.InternalGrid.RowHeaderStyle;
                }

                this.InternalGrid.SummaryColumnStyle = this.SummaryColumnStyle;
                if (this.SummaryColumnStyle != null)
                {
                    this.SummaryColumnStyle.InternalSourceStyle = this.InternalGrid.SummaryColumnStyle;
                }

                this.InternalGrid.SummaryRowStyle = this.SummaryRowStyle;
                if (this.SummaryRowStyle != null)
                {
                    this.SummaryRowStyle.InternalSourceStyle = this.InternalGrid.SummaryRowStyle;
                }

                this.InternalGrid.ValueCellStyle = this.ValueCellStyle;
                if (this.ValueCellStyle != null)
                {
                    this.ValueCellStyle.InternalSourceStyle = this.InternalGrid.ValueCellStyle;
                }

                this.InternalGrid.AfterRefresh += new OlapGridDrillDownEventHander(InternalGrid_AfterRefresh);
                this.InternalGrid.BeforeRefresh += new OlapGridDrillDownEventHander(InternalGrid_BeforeRefresh);
                this.InternalGrid.LinkClick += new LinkLabelClickEventHander(InternalGrid_LinkClick);

#if !SILVERLIGHT
                if (this.DesignerSettings != null && !DesignerProperties.GetIsInDesignMode(this))
                {
                    OlapDataManager olapDataManager = new OlapDataManager(this.DesignerSettings.ConnectionString);
                    olapDataManager.SetCurrentReport(this.DesignerSettings.GetOlapReport());
                    this.OlapDataManager = olapDataManager;
                }
#endif

                if (!DesignerProperties.GetIsInDesignMode(this))
                {
#if !SILVERLIGHT
                    if (this.OlapDataManager != null)
                    {
                        if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                        {
                            this.OlapDataManager.SetCurrentReport(this.CreateOlapReport());
                        }
                        if (!(this.OlapDataManager as OlapDataManager).UseSharedDataManager)
                            this.InternalGrid.DataManager = this.OlapDataManager as OlapDataManager;
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
#else
                    this.InternalGrid.DataManager = this.OlapDataManager as OlapDataManager;
#endif
                }
                if (this.IsDataBindCalled)
                {
                    this.DataBind();
                }
            }
        }
#if !SILVERLIGHT
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
#endif
        #endregion      

        #region Private Events


        void InternalGrid_LinkClick(object sender, LinkLabelEventArgs e)
        {
            SelectedCell = e.CellDescriptor;
            this.RaiseLinkLabelClick(sender, e);
        }

        void InternalGrid_BeforeRefresh(object sender, OlapGridDrillDownEventArgs e)
        {
            this.RaiseBeforeDrillDown(e);
        }

        void InternalGrid_AfterRefresh(object sender, OlapGridDrillDownEventArgs e)
        {
            this.RaiseAfterDrillDown(e);
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Gets the grid appearance settings.
        /// </summary>
        /// <returns></returns>
        private GridAppearanceSettings GetGridAppearanceSettings()
        {
            GridAppearanceSettings gridSettings = new GridAppearanceSettings();
            if (this.Background is SolidColorBrush)
            {
                gridSettings.Background = (this.Background as SolidColorBrush).Color.ToString();
            }
            gridSettings.AllowResizeColumns = this.AllowResizeColumns;
            gridSettings.AllowResizeRows = this.AllowResizeRows;
            gridSettings.AllowSelection = this.AllowSelection;
            gridSettings.FreezeHeader = this.FreezeHeaders;
            gridSettings.GridLayout = this.Layout.ToString();
            gridSettings.ResizeRowsToFit = this.ResizeRowsToFit;
            gridSettings.ResizeColumnsToFit = this.ResizeColumnsToFit;
            gridSettings.ShowValueCellTooltip = this.ShowValueCellToolTip;
#if !SILVERLIGHT
            gridSettings.ShowHeaderCellTooltip = this.ShowHeaderCellsToolTip;
            gridSettings.ShowMemberPropertiesToolTip = this.ShowMemberPropertiesToolTip; 
#endif
            gridSettings.ValueCellHorizontalAlignment = this.ValueCellTextAlignment.ToString();
            if (this.GridStyleInfo != null)
            {
                gridSettings.GridStyles = gridSettings.GridStyles ?? new GridStyles();
                gridSettings.GridStyles.ColumnHeaderBackground = this.GridStyleInfo.HeaderBackgroundColor.ToString();
                gridSettings.GridStyles.ColumnHeaderForeground = this.GridStyleInfo.HeaderForeGroundColor.ToString();
                gridSettings.GridStyles.ColumnSummaryBackground = this.GridStyleInfo.SummaryColumnBackgroundColor.ToString();
                gridSettings.GridStyles.ColumnSummaryForeground = this.GridStyleInfo.SummaryColumnForegroundColor.ToString();
#if SILVERLIGHT
                gridSettings.GridStyles.GridLineStroke = this.GridStyleInfo.GridlineColor.ToString();
                gridSettings.GridStyles.GridLineThickness = this.GridStyleInfo.GridlineThickness; 
#else
                gridSettings.GridStyles.GridLineStroke = this.GridStyleInfo.GridBorderColor.ToString();
                gridSettings.GridStyles.GridLineThickness = this.GridStyleInfo.GridThickness; 
#endif
                gridSettings.GridStyles.HeaderCellFontFamily = this.GridStyleInfo.HeaderFontName;
                gridSettings.GridStyles.HeaderCellFontSize = this.GridStyleInfo.HeaderFontSize;
                gridSettings.GridStyles.RowHeaderBackground = this.GridStyleInfo.HeaderRowBackgroundColor.ToString();
                gridSettings.GridStyles.RowHeaderForeground = this.GridStyleInfo.HeaderRowForegroundColor.ToString();
                gridSettings.GridStyles.RowSummaryBackground = this.GridStyleInfo.SummaryRowBackgroundColor.ToString();
                gridSettings.GridStyles.RowSummaryForeground = this.GridStyleInfo.SummaryRowForegroundColor.ToString();
                gridSettings.GridStyles.SummaryCellFontFamily = this.GridStyleInfo.SummaryFontName;
                gridSettings.GridStyles.SummaryCellFontSize = this.GridStyleInfo.SummaryFontSize;
                gridSettings.GridStyles.ValueCellFontFamily = this.GridStyleInfo.CellFontName;
                gridSettings.GridStyles.ValueCellFontSize = this.GridStyleInfo.CellFontSize;
                gridSettings.GridStyles.ValueCellFontStyle = this.GridStyleInfo.CellFontStyle;
#if SILVERLIGHT
                gridSettings.GridStyles.ValueCellBackground = this.GridStyleInfo.GridBackColor.ToString();
#else
                gridSettings.GridStyles.ValueCellBackground = this.GridStyleInfo.GridBackGround.ToString();

                gridSettings.GridStyles.ApplyColumnHeaderStyle = this.GridStyleInfo.ApplyColumnHeaderStyle;
                gridSettings.GridStyles.ApplyHeaderFontStyle = this.GridStyleInfo.ApplyHeaderFontStyle;
                gridSettings.GridStyles.ApplyRowHeaderStyle = this.GridStyleInfo.ApplyRowHeaderStyle;
                gridSettings.GridStyles.ApplySummaryColumnStyle = this.GridStyleInfo.ApplySummaryColumnStyle;
                gridSettings.GridStyles.ApplySummaryHeaderFontStyle = this.GridStyleInfo.ApplySummaryHeaderFontStyle;
                gridSettings.GridStyles.ApplySummaryRowStyle = this.GridStyleInfo.ApplySummaryRowStyle;
                gridSettings.GridStyles.ApplyValueCellStyle = this.GridStyleInfo.ApplyValueCellStyle;
#endif
                gridSettings.GridStyles.ValueCellForeground = this.GridStyleInfo.CellFontColor.ToString();
            }
            
            return gridSettings;
        }

        /// <summary>
        /// Sets the grid appearance settings.
        /// </summary>
        /// <param name="gridSettings">The grid settings.</param>
        private void SetGridAppearanceSettings(GridAppearanceSettings gridSettings)
        {

            if (!string.IsNullOrEmpty(gridSettings.Background))
            {
#if !SILVERLIGHT                
               this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(gridSettings.Background));
#else
               this.Background = new SolidColorBrush(HexStringToColor(gridSettings.Background));
#endif
            } 


            this.AllowResizeColumns = gridSettings.AllowResizeColumns;
            this.AllowResizeRows = gridSettings.AllowResizeRows;
            this.AllowSelection = gridSettings.AllowSelection;
            this.FreezeHeaders = gridSettings.FreezeHeader;
            if (!string.IsNullOrEmpty(gridSettings.GridLayout))
                this.Layout = (GridLayout)Enum.Parse(typeof(GridLayout), gridSettings.GridLayout, true);
            this.ResizeRowsToFit = gridSettings.ResizeRowsToFit;
            this.ResizeColumnsToFit = gridSettings.ResizeColumnsToFit;
            this.ShowValueCellToolTip = gridSettings.ShowValueCellTooltip;
#if !SILVERLIGHT
            this.ShowHeaderCellsToolTip = gridSettings.ShowHeaderCellTooltip;
            this.ShowMemberPropertiesToolTip = gridSettings.ShowMemberPropertiesToolTip; 
#endif
            if (!string.IsNullOrEmpty(gridSettings.ValueCellHorizontalAlignment))
                this.ValueCellTextAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), gridSettings.ValueCellHorizontalAlignment, true);
            if (gridSettings.GridStyles != null)
            {
                this.GridStyleInfo = this.GridStyleInfo ?? new ExportingGridStyleInfo();
#if SILVERLIGHT
                this.GridStyleInfo.CellFontColor = HexStringToColor(gridSettings.GridStyles.ValueCellForeground);
                this.GridStyleInfo.CellFontName = gridSettings.GridStyles.ValueCellFontFamily;
                this.GridStyleInfo.CellFontSize = gridSettings.GridStyles.ValueCellFontSize;
                this.GridStyleInfo.CellFontStyle = gridSettings.GridStyles.ValueCellFontStyle;
                this.GridStyleInfo.GridBackColor = HexStringToColor(gridSettings.GridStyles.ValueCellBackground);
                this.GridStyleInfo.GridlineColor = HexStringToColor(gridSettings.GridStyles.GridLineStroke);
                this.GridStyleInfo.GridlineThickness = gridSettings.GridStyles.GridLineThickness;

                this.GridStyleInfo.HeaderBackgroundColor = HexStringToColor(gridSettings.GridStyles.ColumnHeaderBackground);
                this.GridStyleInfo.HeaderFontName = gridSettings.GridStyles.HeaderCellFontFamily;
                this.GridStyleInfo.HeaderFontSize = gridSettings.GridStyles.HeaderCellFontSize;
                this.GridStyleInfo.HeaderForeGroundColor = HexStringToColor(gridSettings.GridStyles.ColumnHeaderForeground);
                this.GridStyleInfo.HeaderRowBackgroundColor = HexStringToColor(gridSettings.GridStyles.RowHeaderBackground);
                this.GridStyleInfo.HeaderRowForegroundColor = HexStringToColor(gridSettings.GridStyles.RowHeaderForeground);

                this.GridStyleInfo.SummaryColumnBackgroundColor = HexStringToColor(gridSettings.GridStyles.ColumnSummaryBackground);
                this.GridStyleInfo.SummaryColumnForegroundColor = HexStringToColor(gridSettings.GridStyles.ColumnSummaryForeground);
                this.GridStyleInfo.SummaryFontName = gridSettings.GridStyles.SummaryCellFontFamily;
                this.GridStyleInfo.SummaryFontSize = gridSettings.GridStyles.SummaryCellFontSize;
                this.GridStyleInfo.SummaryRowBackgroundColor = HexStringToColor(gridSettings.GridStyles.RowSummaryBackground);
                this.GridStyleInfo.SummaryRowForegroundColor = HexStringToColor(gridSettings.GridStyles.RowSummaryForeground);
#else
                this.GridStyleInfo.HeaderBackgroundColor = gridSettings.GridStyles.ColumnHeaderBackground;
                this.GridStyleInfo.HeaderForeGroundColor = gridSettings.GridStyles.ColumnHeaderForeground;
                this.GridStyleInfo.HeaderRowBackgroundColor = gridSettings.GridStyles.RowHeaderBackground;
                this.GridStyleInfo.HeaderRowForegroundColor = gridSettings.GridStyles.RowHeaderForeground;
                this.GridStyleInfo.HeaderFontSize = gridSettings.GridStyles.HeaderCellFontSize;
                this.GridStyleInfo.HeaderFontName = gridSettings.GridStyles.HeaderCellFontFamily;

                this.GridStyleInfo.SummaryColumnBackgroundColor = gridSettings.GridStyles.ColumnSummaryBackground;
                this.GridStyleInfo.SummaryColumnForegroundColor = gridSettings.GridStyles.ColumnSummaryForeground;
                this.GridStyleInfo.SummaryRowBackgroundColor = gridSettings.GridStyles.RowSummaryBackground;
                this.GridStyleInfo.SummaryRowForegroundColor = gridSettings.GridStyles.RowSummaryForeground;
                this.GridStyleInfo.SummaryFontSize = (float)gridSettings.GridStyles.SummaryCellFontSize;
                this.GridStyleInfo.SummaryFontName = gridSettings.GridStyles.SummaryCellFontFamily;
                
                this.GridStyleInfo.CellFontColor = gridSettings.GridStyles.ValueCellForeground;
                this.GridStyleInfo.CellFontName = gridSettings.GridStyles.ValueCellFontFamily;
                this.GridStyleInfo.CellFontSize = (float)gridSettings.GridStyles.ValueCellFontSize;
                this.GridStyleInfo.CellFontStyle = gridSettings.GridStyles.ValueCellFontStyle;

                this.GridStyleInfo.ApplyColumnHeaderStyle = gridSettings.GridStyles.ApplyColumnHeaderStyle;
                this.GridStyleInfo.ApplyHeaderFontStyle = gridSettings.GridStyles.ApplyHeaderFontStyle;
                this.GridStyleInfo.ApplyRowHeaderStyle = gridSettings.GridStyles.ApplyRowHeaderStyle;
                this.GridStyleInfo.ApplySummaryColumnStyle = gridSettings.GridStyles.ApplySummaryColumnStyle;
                this.GridStyleInfo.ApplySummaryHeaderFontStyle = gridSettings.GridStyles.ApplySummaryHeaderFontStyle;
                this.GridStyleInfo.ApplySummaryRowStyle = gridSettings.GridStyles.ApplySummaryRowStyle;
                this.GridStyleInfo.ApplyValueCellStyle = gridSettings.GridStyles.ApplyValueCellStyle;
#endif 
                
                this.ApplyStylesToGrid();
            }
        }

        /// <summary>
        /// Applies the styles to grid.
        /// </summary>
        private void ApplyStylesToGrid()
        {
            this.ColumnHeaderStyle = this.ColumnHeaderStyle ?? new OlapGridCellStyle();
            this.RowHeaderStyle = this.RowHeaderStyle ?? new OlapGridCellStyle();
            this.SummaryColumnStyle = this.SummaryColumnStyle ?? new OlapGridCellStyle();
            this.SummaryRowStyle = this.SummaryRowStyle ?? new OlapGridCellStyle();
            this.ValueCellStyle = this.ValueCellStyle ?? new OlapGridCellStyle();
#if SILVERLIGHT
            this.ColumnHeaderStyle.Background = new SolidColorBrush(this.GridStyleInfo.HeaderBackgroundColor);
            this.ColumnHeaderStyle.Foreground = new SolidColorBrush(this.GridStyleInfo.HeaderForeGroundColor);
            this.RowHeaderStyle.Background = new SolidColorBrush(this.GridStyleInfo.HeaderRowBackgroundColor);
            this.RowHeaderStyle.Foreground = new SolidColorBrush(this.GridStyleInfo.HeaderRowForegroundColor);
            this.RowHeaderStyle.FontSize = this.ColumnHeaderStyle.FontSize = (int)this.GridStyleInfo.HeaderFontSize;
            this.RowHeaderStyle.FontFamily = this.ColumnHeaderStyle.FontFamily = new FontFamily(this.GridStyleInfo.HeaderFontName);

            this.SummaryColumnStyle.Background = new SolidColorBrush(this.GridStyleInfo.SummaryColumnBackgroundColor);
            this.SummaryColumnStyle.Foreground = new SolidColorBrush(this.GridStyleInfo.SummaryColumnForegroundColor);
            this.SummaryRowStyle.Background = new SolidColorBrush(this.GridStyleInfo.SummaryRowBackgroundColor);
            this.SummaryRowStyle.Foreground = new SolidColorBrush(this.GridStyleInfo.SummaryRowForegroundColor);
            this.SummaryRowStyle.FontFamily = this.SummaryColumnStyle.FontFamily = new FontFamily(this.GridStyleInfo.SummaryFontName);
            this.SummaryColumnStyle.FontSize = this.SummaryRowStyle.FontSize = (int)this.GridStyleInfo.SummaryFontSize;

            this.ValueCellStyle.Foreground = new SolidColorBrush(this.GridStyleInfo.CellFontColor);
            this.ValueCellStyle.FontFamily = new FontFamily(this.GridStyleInfo.CellFontName);
            this.ValueCellStyle.FontSize = (int)this.GridStyleInfo.CellFontSize;
            this.ValueCellStyle.Background = new SolidColorBrush(this.GridStyleInfo.GridBackColor);
            if (this.GridStyleInfo.CellFontStyle.Equals("Black"))
            {
                this.ValueCellStyle.FontWeight = FontWeights.Black; 
            }
            else if (this.GridStyleInfo.CellFontStyle.Equals("Bold"))
            {
                this.ValueCellStyle.FontWeight = FontWeights.Bold;
            }

            this.GridLineStroke = new SolidColorBrush(this.GridStyleInfo.GridlineColor);
            this.GridLineThickness = this.GridStyleInfo.GridlineThickness;
#else
            if (GridStyleInfo.ApplyColumnHeaderStyle)
            {
                Brush HeaderColumnBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderBackgroundColor));
                Brush HeaderColumnForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderForeGroundColor));
                this.ColumnHeaderStyle.Background = HeaderColumnBackColor;
                this.ColumnHeaderStyle.Foreground = HeaderColumnForeColor;
            }
            if (GridStyleInfo.ApplyRowHeaderStyle)
            {
                Brush HeaderRowBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderRowBackgroundColor));
                Brush HeaderRowForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderRowForegroundColor));
                this.RowHeaderStyle.Background = HeaderRowBackColor;
                this.RowHeaderStyle.Foreground = HeaderRowForeColor;
            }
            if (GridStyleInfo.ApplySummaryColumnStyle)
            {
                Brush SummaryColumnBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryColumnBackgroundColor));
                Brush SummaryColumnForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryColumnForegroundColor));
                this.SummaryColumnStyle.Background = SummaryColumnBackColor;
                this.SummaryColumnStyle.Foreground = SummaryColumnForeColor;
            }
            if (GridStyleInfo.ApplySummaryRowStyle)
            {
                Brush SummaryRowBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryRowBackgroundColor));
                Brush SummaryRowForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryRowForegroundColor));
                this.SummaryRowStyle.Background = SummaryRowBackColor;
                this.SummaryRowStyle.Foreground = SummaryRowForeColor;
            }
            if (GridStyleInfo.ApplyHeaderFontStyle)
            {
                this.RowHeaderStyle.FontSize = this.ColumnHeaderStyle.FontSize = (int)GridStyleInfo.HeaderFontSize;
                this.RowHeaderStyle.FontFamily = this.ColumnHeaderStyle.FontFamily = new FontFamily(GridStyleInfo.HeaderFontName);
            }
            if (GridStyleInfo.ApplySummaryHeaderFontStyle)
            {
                this.SummaryColumnStyle.FontFamily = this.SummaryRowStyle.FontFamily = new FontFamily(GridStyleInfo.SummaryFontName);
                this.SummaryColumnStyle.FontSize = this.SummaryRowStyle.FontSize = (int)GridStyleInfo.SummaryFontSize;
            }
            if (GridStyleInfo.ApplyValueCellStyle)
            {
                Brush CellFont = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.CellFontColor));
                this.ValueCellStyle.Foreground = CellFont;
                this.ValueCellStyle.FontFamily = new FontFamily(GridStyleInfo.CellFontName);
                this.ValueCellStyle.FontSize = (int)GridStyleInfo.CellFontSize;
                if (GridStyleInfo.CellFontStyle.Equals("Black"))
                {
                    this.ValueCellStyle.FontWeight = FontWeights.Black;
                }
                else if (GridStyleInfo.CellFontStyle.Equals("Bold"))
                {
                    this.ValueCellStyle.FontWeight = FontWeights.Bold;
                }
            }

            this.GridLineStroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.GridBorderColor));
            this.GridLineThickness = GridStyleInfo.GridThickness;
#endif
        }

        /// <summary>
        /// Converts the Hexadecimal string to Color object .
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns></returns>
        private Color HexStringToColor(string hexString)
        {
            try
            {
                hexString = hexString.Replace("#", string.Empty);
                byte pos = 0;
                //// Variable for alpha channel with default code.
                byte alpha = System.Convert.ToByte("ff", 16);

                if (hexString.Length == 8)
                {
                    //// Get the alpha value
                    alpha = System.Convert.ToByte(hexString.Substring(pos, 2), 16);
                    pos = 2;
                }

                //// Get the red value
                byte red = System.Convert.ToByte(hexString.Substring(pos, 2), 16);
                pos += 2;

                //// Get the green value
                byte green = System.Convert.ToByte(hexString.Substring(pos, 2), 16);
                pos += 2;

                //// Get the blue value
                byte blue = System.Convert.ToByte(hexString.Substring(pos, 2), 16);

                // create the Color object with A,R,G and B values.
                Color color = Color.FromArgb(alpha, red, green, blue);

                return color;
            }
            catch
            {
                throw new ArgumentException("Invalid Hexadecimal string");
            }
        }

        /// <summary>
        /// Raises the after drill down.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void RaiseAfterDrillDown(OlapGridDrillDownEventArgs args)
        {
            if (this.AfterRefresh != null)
            {
                this.AfterRefresh(this, args);
            }
        }
    

        /// <summary>
        /// Raises the before drill down.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void RaiseBeforeDrillDown(OlapGridDrillDownEventArgs args)
        {
            if (this.BeforeRefresh != null)
            {
                args.GridArea = this.GridArea;
                this.BeforeRefresh(this, args);
            }
        }

        internal void RaiseLinkLabelClick(object sender, LinkLabelEventArgs args)
        {
            if (this.LinkClick != null)
            {
                this.LinkClick(this, args);
            }

#if !SILVERLIGHT
            if (Command != null)
            {
                if (CommandParameter == null)
                {
                    Command.Execute(args.CellDescriptor);
                }
                else
                {
                    object[] commandParameters = new object[] { args.CellDescriptor, CommandParameter };
                    Command.Execute(commandParameters);
                }
            }
#endif
        }

        /// <summary>
        /// Raises the Grid Selection event.
        /// </summary>
        /// <param name="SelectionChangingEventArgs">The <see cref="Syncfusion.Windows.Controls.OlapGrid.SelectionChangedEventArgs"/> instance containing the event data.</param>
        internal void RaiseSelectionEvent(OlapGridSelectionChangedEventArgs SelectionChangingEventArgs)
        {
            if (SelectionChanged != null)
            {
                this.SelectedItems = SelectionChangingEventArgs.SelectedItems;
                SelectionChanged(this, SelectionChangingEventArgs);
            }
        }

        #endregion        
    
    }

    /// <summary>
    /// OlapGrid Exception class
    /// </summary>
    public class OlapGridException : Exception
    {
        private const string __message = @"This operation is invalid in current context.";

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridExceptoin"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OlapGridException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridExceptoin"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public OlapGridException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif    

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridExceptoin"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OlapGridException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridExceptoin"/> class.
        /// </summary>
        public OlapGridException()
            : this(__message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridExceptoin"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public OlapGridException(Exception innerException)
            : this(__message, innerException)
        {
        }

    }

#if !SILVERLIGHT
    /// <summary>
    /// OlapGrid Commands class
    /// </summary>
    public static class OlapGridCommands
    {
        /// <summary>
        /// Routed command for context menu.
        /// </summary>
        public static RoutedUICommand ContextMenu = new RoutedUICommand("ContextMenu", "ContextMenu", typeof(OlapGridCommands));

        /// <summary>
        /// Intializes the c_dataBind RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_dataBind = new RoutedUICommand("DataBind", "DataBind", typeof(OlapGridCommands));

        /// <summary>
        /// Intializes the c_refresh RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_refresh = new RoutedUICommand("Refresh", "Refresh", typeof(OlapGridCommands));

        /// <summary>
        /// Intializes the c_showStyleDialog RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_showStyleDialog = new RoutedUICommand("ShowStyleDialog", "ShowStyleDialog", typeof(OlapGridCommands));

        /// <summary>
        /// Opens the ShowStyleDialog window
        /// </summary>
        public static RoutedUICommand ShowStyleDialog
        {
            get { return c_showStyleDialog; }
        }

        /// <summary>
        /// Calls the DataBind method of OlapGrid
        /// </summary>
        public static RoutedUICommand DataBind
        {
            get { return c_dataBind; }
        }

        /// <summary>
        /// Calls the Refresh method of OlapGrid
        /// </summary>
        public static RoutedUICommand Refresh
        {
            get { return c_refresh; }
        }

    }
#endif


    /// <summary>
    /// Specifies the Visual Style for OlapGrid
    /// </summary>
    public enum OlapGridVisualStyle
    {
        /// <summary>
        /// Provide Blend Style for OlapGrid
        /// </summary>
        Blend,
        /// <summary>
        /// Provide Metro Style for OlapGrid
        /// </summary>
        Metro,
        /// <summary>
        /// Provide Default Style for OlapGrid
        /// </summary>
        Default,
        /// <summary>
        /// Provide Office 2007 Blue Style for OlapGrid
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Provide Office 2007 Black Style for OlapGrid
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Provide Office 2007 Silver Style for OlapGrid
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Provide Office 2010 Black Style for OlapGrid
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Provide office 2010 Blue Style for OlapGrid
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Provides office 2010 Silver Style for OlapGrid
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Provides transparent style for olapgrid
        /// </summary>
        Transparent,
#if !SILVERLIGHT
        /// <summary>
        /// Provide Office 2003 Style for OlapGrid
        /// </summary>
        Office2003
#endif
    }
}