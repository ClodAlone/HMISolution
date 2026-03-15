#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Shared.Olap;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools.Olap;
using Syncfusion.Olap.Engine;
using System.Windows.Media.Animation;
using Syncfusion.Windows.Grid.Olap.Converter;
using Syncfusion.Windows.Chart.Olap.Converter;
using Syncfusion.Licensing;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.Olap.DataProvider;
using Syncfusion.Windows.Chart.Olap;

namespace Syncfusion.Windows.Client.Olap
{
    #if !SILVERLIGHT
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(OlapClient), XamlResource = "/Syncfusion.OlapClient.WPF;component/Themes/TransparentStyle.xaml")]
    #endif
    [StyleTypedProperty(Property = "TabControlExtStyle", StyleTargetType = typeof(TabControlExt))]
    [StyleTypedProperty(Property = "CubeDimensionBrowserStyle", StyleTargetType = typeof(CubeDimensionBrowser))]
    [StyleTypedProperty(Property = "ListBoxStyle", StyleTargetType = typeof(ListBox))]

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class OlapClient : UserControl
    {
        #region Variables

        Style tabstyle = null;
        Style treeviewstyle = null;
        Style listboxstyle = null;
        private bool _loadDefaultReport = true;
        private static DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapClient), new UIPropertyMetadata(null, OnOlapDataManagerPropertyChanged));

        private static void OnOlapDataManagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapClient client = d as OlapClient;
            if (client != null && !DesignerProperties.GetIsInDesignMode(client))
            {
                client.DataBind();
            }
        }
        private static DependencyProperty ShowToolBarProperty =
            DependencyProperty.Register("ShowToolBar", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnToolBarVisibilityChanged)));

        private static DependencyProperty ShowCubeSelectorProperty =
           DependencyProperty.Register("ShowCubeSelector", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnCubeSelectorVisibilityChanged)));

        // Using a DependencyProperty as the backing store for ChartHeaderText.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ChartHeaderTextProperty =
            DependencyProperty.Register("ChartHeaderText", typeof(string), typeof(OlapClient), new UIPropertyMetadata(string.Empty));

        private static readonly DependencyProperty ChartTabHeaderTextProperty =
            DependencyProperty.Register("ChartTabHeaderText", typeof(string), typeof(OlapClient), new UIPropertyMetadata("Chart"));

        private static readonly DependencyProperty ConnectionStringProperty =
            DependencyProperty.Register("ConnectionString", typeof(string), typeof(OlapClient), new UIPropertyMetadata(string.Empty));

        private static readonly DependencyProperty ProviderNameProperty =
            DependencyProperty.Register("ProviderName", typeof(Providers), typeof(OlapClient), new UIPropertyMetadata(Providers.SSAS));


        private static readonly DependencyProperty CurrentThemeProperty =
            DependencyProperty.Register("CurrentTheme", typeof(SolidColorBrush), typeof(OlapClient), new UIPropertyMetadata(Brushes.Blue));

        private static readonly DependencyProperty GridTabHeaderTextProperty =
            DependencyProperty.Register("GridTabHeaderText", typeof(string), typeof(OlapClient), new UIPropertyMetadata("Grid"));

        private static readonly DependencyPropertyKey AxisElementBuilderColumnKey =
                 DependencyProperty.RegisterReadOnly("AxisElementBuilderColumn", typeof(AxisElementBuilder), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty AxisElementBuilderColumnProperty = AxisElementBuilderColumnKey.DependencyProperty;

        private static readonly DependencyPropertyKey AxisElementBuilderRowKey =
                DependencyProperty.RegisterReadOnly("AxisElementBuilderRow", typeof(AxisElementBuilder), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty AxisElementBuilderRowProperty = AxisElementBuilderRowKey.DependencyProperty;

        private static readonly DependencyPropertyKey AxisElementBuilderSlicerKey =
               DependencyProperty.RegisterReadOnly("AxisElementBuilderSlicer", typeof(AxisElementBuilder), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty AxisElementBuilderSlicerProperty = AxisElementBuilderSlicerKey.DependencyProperty;

        private static readonly DependencyPropertyKey OlapChartTabKey =
                DependencyProperty.RegisterReadOnly("OlapChartTab", typeof(TabItemExt), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty OlapChartTabProperty = OlapChartTabKey.DependencyProperty;

        private static readonly DependencyPropertyKey OlapGridTabKey =
                DependencyProperty.RegisterReadOnly("OlapGridTab", typeof(TabItemExt), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty OlapGridTabProperty = OlapGridTabKey.DependencyProperty;

        private static DependencyPropertyKey OlapChartToolBarKey =
                DependencyProperty.RegisterReadOnly("OlapChartToolBar", typeof(ToolBar), typeof(OlapClient), new PropertyMetadata(null));
        private static DependencyProperty OlapChartToolBarProperty = OlapChartToolBarKey.DependencyProperty;

        private static DependencyPropertyKey OlapClientToolBarKey =
                DependencyProperty.RegisterReadOnly("OlapClientToolBar", typeof(ToolBar), typeof(OlapClient), new PropertyMetadata(null));
        private static DependencyProperty OlapClientToolBarProperty = OlapClientToolBarKey.DependencyProperty;

        private static readonly DependencyPropertyKey olapTabControlKey =
                DependencyProperty.RegisterReadOnly("OlapTabControl", typeof(TabControlExt), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty olapTabControlProperty = olapTabControlKey.DependencyProperty;

        private static readonly DependencyPropertyKey CubeDimensionBrowserKey =
                DependencyProperty.RegisterReadOnly("CubeDimensionBrowser", typeof(CubeDimensionBrowser), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty CubeDimensionBrowserProperty = CubeDimensionBrowserKey.DependencyProperty;

        private static readonly DependencyPropertyKey CubeSelectorKey =
                DependencyProperty.RegisterReadOnly("CubeSelector", typeof(CubeSelector), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty CubeSelectorProperty = CubeSelectorKey.DependencyProperty;

        private static DependencyPropertyKey OlapGridToolBarKey =
                DependencyProperty.RegisterReadOnly("OlapGridToolBar", typeof(ToolBar), typeof(OlapClient), new PropertyMetadata(null));
        private static DependencyProperty OlapGridToolBarProperty = OlapGridToolBarKey.DependencyProperty;

        private static readonly DependencyPropertyKey OlapChartKey =
                DependencyProperty.RegisterReadOnly("OlapChart", typeof(Syncfusion.Windows.Chart.Olap.OlapChart), typeof(OlapClient), new PropertyMetadata(null));
        private static readonly DependencyProperty OlapChartProperty = OlapChartKey.DependencyProperty;

        private static readonly DependencyPropertyKey OlapGridPropertyKey =
                DependencyProperty.RegisterReadOnly("OlapGrid", typeof(Syncfusion.Windows.Grid.Olap.OlapGrid), typeof(OlapClient), new PropertyMetadata(null));

        private static readonly DependencyProperty OlapGridProperty = OlapGridPropertyKey.DependencyProperty;

        private static DependencyProperty ShowConnectOptionButtonProperty =
                DependencyProperty.Register("ShowConnectOptionButton", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnConnectOptionButtonVisibilityChanged)));

        private static DependencyProperty ShowReportButtonsProperty =
                DependencyProperty.Register("ShowReportButtons", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnReportButtonsVisibilityChanged)));

        private static DependencyProperty ShowFilterSortButtonsProperty =
                DependencyProperty.Register("ShowFilterSortButtons", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnFilterSortButtonsVisibilityChanged)));

        public static readonly DependencyProperty SubsetFiltersVisibilityProperty =
            DependencyProperty.Register("ShowSubsetFilters", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnSubsetFiltersVisibilityChanged)));

        private static DependencyProperty ShowExecuteButtonProperty =
         DependencyProperty.Register("ShowExecuteButton", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(false, new PropertyChangedCallback(OnExecuteButtonVisibilityChanged)));

        private static DependencyProperty AutoExecuteProperty =
            DependencyProperty.Register("AutoExecute", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAutoExecuteChanged)));

        private static readonly DependencyProperty IsCalculatedMembersEnabledProperty =
            DependencyProperty.Register("IsCalculatedMembersEnabled", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(false, new PropertyChangedCallback(OnCalculatedMembersEnabled)));
        
        public static readonly DependencyProperty IsVirtualKpiEnabledProperty =
            DependencyProperty.Register("IsVirtualKpiEnabled", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(false,new PropertyChangedCallback(OnVirtualKpiEnabled)));

        public static readonly DependencyProperty HeaderBackgroundProperty =
           DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(OlapClient), new UIPropertyMetadata());

        public static readonly DependencyProperty HeaderBorderBrushProperty =
          DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(OlapClient), new UIPropertyMetadata());

        public static readonly DependencyProperty HeaderMouseOverBackgroundProperty =
        DependencyProperty.Register("HeaderMouseOverBackground", typeof(Brush), typeof(OlapClient), new UIPropertyMetadata());

        public static readonly DependencyProperty HeaderMouseOverBorderBrushProperty =
          DependencyProperty.Register("HeaderMouseOverBorderBrush", typeof(Brush), typeof(OlapClient), new UIPropertyMetadata());

        public static readonly DependencyProperty IsDragOverProperty =
          DependencyProperty.Register("IsDragOver", typeof(bool), typeof(OlapClient), new UIPropertyMetadata(false));

        /// <summary>
        /// VisualStyle dependency property
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(OlapClientVisualStyle), typeof(OlapClient), new UIPropertyMetadata(OlapClientVisualStyle.Default,OnVisualStyleChanged));
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapClient"/> class.
        /// </summary>
        public OlapClient()
        {
            EnvironmentTestTools.ValidateLicense(typeof(OlapClient));
            InitializeComponent();
            this.SetValue(AxisElementBuilderColumnKey, this.axisElementBuiderCategorical);
            this.SetValue(AxisElementBuilderRowKey, this.axisElementBuiderSeries);
            this.SetValue(AxisElementBuilderSlicerKey, this.axisElementBuiderSlicer);
            this.SetValue(OlapChartTabKey, this.olapChartTab);
            this.SetValue(OlapGridTabKey, this.olapGridTab);
            this.SetValue(OlapChartToolBarKey, this.olapChartToolbar);
            this.SetValue(OlapClientToolBarKey, this.olapClientToolbar);
            this.SetValue(olapTabControlKey, this.olapTabControl);
            this.SetValue(CubeDimensionBrowserKey, this.cubeDimensionBrowser);
            this.SetValue(CubeSelectorKey, this.cubeSelector);
            this.SetValue(OlapGridToolBarKey, this.olapGridToolbar);
            this.SetValue(OlapChartKey, this.olapChart);
            this.SetValue(OlapGridPropertyKey, this.olapGrid);
            this.subsetFilterCategorical.ValueChanged += new ValueChangedEventHandler(subsetFilterCategorical_ValueChanged);
            this.subsetFilterRows.ValueChanged += new ValueChangedEventHandler(subsetFilterRows_ValueChanged);
            this.olapTabControl.ContextMenuOpening += new ContextMenuEventHandler(olapTabControl_ContextMenuOpening);
            treeviewstyle = this.Resources["DefaultCubeDimensionBrowser"] as Style;
            //Commands
            CommandBinding newReportBinding = new CommandBinding(OlapClientCommands.NewReport, new ExecutedRoutedEventHandler(OnNewReportCommand));
            CommandBinding addReportBinding = new CommandBinding(OlapClientCommands.AddReport, new ExecutedRoutedEventHandler(OnAddReportCommand));
            CommandBinding saveReportBinding = new CommandBinding(OlapClientCommands.SaveReport, new ExecutedRoutedEventHandler(OnSaveReportCommand));
            CommandBinding loadReportBinding = new CommandBinding(OlapClientCommands.LoadReport, new ExecutedRoutedEventHandler(OnLoadReportCommand));
            CommandBinding loadReportStreamBinding = new CommandBinding(OlapClientCommands.LoadReportStream, new ExecutedRoutedEventHandler(OnLoadReportStreamCommand));
            CommandBinding removeReportBinding = new CommandBinding(OlapClientCommands.RemoveReport, new ExecutedRoutedEventHandler(OnRemoveReportCommand));
            CommandBinding renameReportBinding = new CommandBinding(OlapClientCommands.RenameReport, new ExecutedRoutedEventHandler(OnRenameReportCommand));
            CommandBinding showMdxBinding = new CommandBinding(OlapClientCommands.ShowMdxDialog, new ExecutedRoutedEventHandler(OnShowMdxDialogCommand));
            CommandBinding showColumnFilterDlgBinding = new CommandBinding(OlapClientCommands.ShowColumnFilterDialog, new ExecutedRoutedEventHandler(OnShowColumnFilterDialogCommand));
            CommandBinding showRowFilterDlgBinding = new CommandBinding(OlapClientCommands.ShowRowFilterDialog, new ExecutedRoutedEventHandler(OnShowRowFilterDialogCommand));
            CommandBinding showColumnSortingDlgBinding = new CommandBinding(OlapClientCommands.ShowColumnSortingDialog, new ExecutedRoutedEventHandler(OnShowColumnSortingDialogCommand));
            CommandBinding showRowSortingDlgBinding = new CommandBinding(OlapClientCommands.ShowRowSortingDialog, new ExecutedRoutedEventHandler(OnShowRowSortingDialogCommand));
            CommandBinding showConnOptionBinding = new CommandBinding(OlapClientCommands.ShowConnectOption, new ExecutedRoutedEventHandler(OnShowConnectOptionCommand));

            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), newReportBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), addReportBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), saveReportBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), loadReportBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), loadReportStreamBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), removeReportBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), renameReportBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), showMdxBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), showColumnFilterDlgBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), showRowFilterDlgBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), showColumnSortingDlgBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), showRowSortingDlgBinding);
            CommandManager.RegisterClassCommandBinding(typeof(OlapClient), showConnOptionBinding);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.FlowDirection == System.Windows.FlowDirection.RightToLeft)
            {
                btnConnectOption.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnNewReport.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnLoadReport.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnSaveAsReport.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnAddReport.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnRemoveReport.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnRenameReport.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnTogglePivot.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnShowExpanders.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnFilterColumn.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnFilterRow.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnSortingColumn.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnSortingRow.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnShowExpanders.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnShowMdx.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                ShowAppearance.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnShowLegend.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnExportChart.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnPrintChart.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnPrintModeChart.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnExportChartToWord.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnExportChartToPdf.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                ShowGridStyleDialog.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnShowHeaderCellsToolTip.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnShowValueCellsToolTip.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnfrozenHeaders.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnExportExcel.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnExportWord.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                btnExportPdf.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                olapGridTab.FlowDirection = this.FlowDirection;
                olapChartTab.FlowDirection = this.FlowDirection;
                olapChart.FlowDirection = this.FlowDirection;
                olapGrid.FlowDirection = this.FlowDirection;
                axisElementBuiderCategorical.FlowDirection = this.FlowDirection;
                axisElementBuiderSeries.FlowDirection = this.FlowDirection;
                axisElementBuiderSlicer.FlowDirection = this.FlowDirection;
            }

        }

        #endregion

        #region Static Methods for Commanding Support
        /// <summary>
        /// Opens the NewReport window
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnNewReportCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapclient = target as OlapClient;
            if (olapclient != null)
            {
                olapclient.CreateNewReport();
            }
        }

        /// <summary>
        /// Opens the AddReport window
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnAddReportCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapclient = target as OlapClient;
            if (olapclient != null)
            {
                if (args.Parameter is string)
                    olapclient.AddReport((string)args.Parameter);
                else
                    olapclient.AddReport();
            }
        }

        /// <summary>
        /// Opens the SaveReport window
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnSaveReportCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                if (args.Parameter is string)
                    olapClient.SaveReports((string)args.Parameter);
                else
                    olapClient.SaveReport();
            }
        }

        /// <summary>
        /// Opens the LoadReport window
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnLoadReportCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.LoadReport();
            }
        }

        /// <summary>
        /// Opens the RemoveReport window
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnRemoveReportCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.RemoveReport();
            }
        }

        /// <summary>
        /// Opens the RenameReport window
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnRenameReportCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.RenameReport();
            }
        }

        /// <summary>
        /// Opens the MDX dialog
        /// </summary>
        /// <param name="target">OlapClient</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnShowMdxDialogCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                string mdxQuery = olapClient.OlapDataManager.GetMDXQuery();
                MdxDialog mdxDialog = new MdxDialog(mdxQuery);
                mdxDialog.FlowDirection = olapClient.FlowDirection;
                mdxDialog.ShowDialog();
            }
        }
        static void OnShowColumnFilterDialogCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.ShowColumnFilterDialog();
            }
        }
        static void OnShowRowFilterDialogCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.ShowRowFilterDialog();
            }
        }
        static void OnShowColumnSortingDialogCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.ShowColumnSortingDialog();
            }
        }
        static void OnShowRowSortingDialogCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.ShowRowSortingDialog();
            }
        }
        static void OnShowConnectOptionCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null)
            {
                olapClient.ShowConnectOption();
            }
        }
        static void OnLoadReportStreamCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapClient olapClient = target as OlapClient;
            if (olapClient != null && args.Parameter is System.IO.Stream)
            {
                olapClient.LoadReportStream(args.Parameter as System.IO.Stream);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to show all the measures or only show the current report measures.
        /// </summary>
        public bool ShowAllMeasuresInFilterSortDlg { get; set; }

        /// <summary>
        /// Gets or sets the show subset filters.
        /// </summary>
        /// <value>The show subset filters.</value>
        public bool ShowSubsetFilters
        {
            get { return (bool)GetValue(SubsetFiltersVisibilityProperty); }
            set { SetValue(SubsetFiltersVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the AxisElementBuilderColumn.
        /// </summary>
        /// <value>The axis element builder column.</value>
        [Browsable(false)]
        public AxisElementBuilder AxisElementBuilderColumn
        {
            get
            {
                return (AxisElementBuilder)GetValue(AxisElementBuilderColumnProperty);
            }

            set
            {
                SetValue(AxisElementBuilderColumnProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the AxisElementBuilderRow.
        /// </summary>
        /// <value>The axis element builder row.</value>
        [Browsable(false)]
        public AxisElementBuilder AxisElementBuilderRow
        {
            get
            {
                return (AxisElementBuilder)GetValue(AxisElementBuilderRowProperty);
            }

            set
            {
                SetValue(AxisElementBuilderRowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the AxisElementBuilderSlicer.
        /// </summary>
        /// <value>The axis element builder slicer.</value>
        [Browsable(false)]
        public AxisElementBuilder AxisElementBuilderSlicer
        {
            get
            {
                return (AxisElementBuilder)GetValue(AxisElementBuilderSlicerProperty);
            }

            set
            {
                SetValue(AxisElementBuilderSlicerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chart header text.
        /// </summary>
        /// <value>The chart header text.</value>
        public string ChartHeaderText
        {
            get
            {
                return (string)GetValue(ChartHeaderTextProperty);
            }

            set
            {
                SetValue(ChartHeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the OlapChartTab.
        /// </summary>
        /// <value>The olap chart tab.</value>
        [Browsable(false)]
        public TabItemExt OlapChartTab
        {
            get
            {
                return (TabItemExt)GetValue(OlapChartTabProperty);
            }

            set
            {
                SetValue(OlapChartTabProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chart tab header text.
        /// </summary>
        /// <value>The chart tab header text.</value>
        public string ChartTabHeaderText
        {
            get
            {
                return (string)GetValue(ChartTabHeaderTextProperty);
            }

            set
            {
                SetValue(ChartTabHeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Olap Chart ToolBar.
        /// </summary>
        /// <value>The olap chart tool bar.</value>
        [Browsable(false)]
        public ToolBar OlapChartToolBar
        {
            get
            {
                return (ToolBar)GetValue(OlapChartToolBarProperty);
            }

            set
            {
                SetValue(OlapChartToolBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Olap Client ToolBar.
        /// </summary>
        /// <value>The olap client tool bar.</value>
        [Browsable(false)]
        public ToolBar OlapClientToolBar
        {
            get
            {
                return (ToolBar)GetValue(OlapClientToolBarProperty);
            }

            set
            {
                SetValue(OlapClientToolBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cube selector header.
        /// </summary>
        /// <value>The cube selector header.</value>
        [Browsable(false)]
        public TextBlock CubeSelectorHeader
        {
            get
            {
                return cubeSelectorHeader;
            }
            set
            {
                cubeSelectorHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the service provider.
        /// </summary>
        public Providers ProviderName
        {
            get { return (Providers)GetValue(ProviderNameProperty); }
            set { SetValue(ProviderNameProperty, value); }
        }        

        /// <summary>
        /// Gets or sets the Olap TabControl.
        /// </summary>
        /// <value>The olap tab control.</value>
        [Browsable(false)]
        public TabControlExt OlapTabControl
        {
            get
            {
                return (TabControlExt)GetValue(olapTabControlProperty);
            }

            set
            {
                SetValue(olapTabControlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CubeDimensionBrowser.
        /// </summary>
        /// <value>The CubeDimensionBrowser.</value>
        [Browsable(false)]
        public CubeDimensionBrowser CubeDimensionBrowser
        {
            get
            {
                return (CubeDimensionBrowser)GetValue(CubeDimensionBrowserProperty);
            }
            set
            {
                SetValue(CubeDimensionBrowserProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CubeSelector.
        /// </summary>
        /// <value>The CubeSelector.</value>
        [Browsable(false)]
        public CubeSelector CubeSelector
        {
            get
            {
                return (CubeSelector)GetValue(CubeSelectorProperty);
            }

            set
            {
                SetValue(CubeSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show tool bar].
        /// </summary>
        /// <value><c>true</c> if [show tool bar]; otherwise, <c>false</c>.</value>
        public bool ShowToolBar
        {
            get
            {
                return (bool)GetValue(ShowToolBarProperty);
            }

            set
            {
                SetValue(ShowToolBarProperty, value);

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show cube selector].
        /// </summary>
        /// <value><c>true</c> if [show cube selector]; otherwise, <c>false</c>.</value>
        public bool ShowCubeSelector
        {
            get
            {
                return (bool)GetValue(ShowCubeSelectorProperty);
            }

            set
            {
                SetValue(ShowCubeSelectorProperty, value);

            }
        }

        /// <summary>
        /// Gets or sets the current theme.
        /// </summary>
        /// <value>The current theme.</value>
        public SolidColorBrush CurrentTheme
        {
            get
            {
                return (SolidColorBrush)GetValue(CurrentThemeProperty);
            }

            set
            {
                SetValue(CurrentThemeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the OlapGrid tab.
        /// </summary>
        /// <value>The OlapGrid tab.</value>
        [Browsable(false)]
        public TabItemExt OlapGridTab
        {
            get
            {
                return (TabItemExt)GetValue(OlapGridTabProperty);
            }

            set
            {
                SetValue(OlapGridTabProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the grid tab header text.
        /// </summary>
        /// <value>The grid tab header text.</value>
        public string GridTabHeaderText
        {
            get
            {
                return (string)GetValue(GridTabHeaderTextProperty);
            }

            set
            {
                SetValue(GridTabHeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Olap Grid tool bar.
        /// </summary>
        /// <value>The Olap Grid tool bar.</value>
        [Browsable(false)]
        public ToolBar OlapGridToolBar
        {
            get
            {
                return (ToolBar)GetValue(OlapGridToolBarProperty);
            }

            set
            {
                SetValue(OlapGridToolBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the OlapChart.
        /// </summary>
        /// <value>The OlapChart.</value>
        [Browsable(false)]
        public Syncfusion.Windows.Chart.Olap.OlapChart OlapChart
        {
            get
            {
                return (Syncfusion.Windows.Chart.Olap.OlapChart)GetValue(OlapChartProperty);
            }

            set
            {
                SetValue(OlapChartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show connect option button].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show connect option button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowConnectOptionButton
        {
            get
            {
                return (bool)GetValue(ShowConnectOptionButtonProperty);
            }

            set
            {
                SetValue(ShowConnectOptionButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show report buttons].
        /// </summary>
        /// <value><c>true</c> if [show report buttons]; otherwise, <c>false</c>.</value>
        public bool ShowReportButtons
        {
            get
            {
                return (bool)GetValue(ShowReportButtonsProperty);
            }

            set
            {
                SetValue(ShowReportButtonsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show filter sort buttons].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show filter sort buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFilterSortButtons
        {
            get
            {
                return (bool)GetValue(ShowFilterSortButtonsProperty);
            }

            set
            {
                SetValue(ShowFilterSortButtonsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show execute button].
        /// </summary>
        /// <value><c>true</c> if [show execute button]; otherwise, <c>false</c>.</value>
        public bool ShowExecuteButton
        {
            get
            {
                return (bool)GetValue(ShowExecuteButtonProperty);
            }

            set
            {
                SetValue(ShowExecuteButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the elements will automatically execute the query.
        /// </summary>
        /// <value><c>true</c> if [auto execute]; otherwise, <c>false</c>.</value>
        public bool AutoExecute
        {
            get
            {
                return (bool)GetValue(AutoExecuteProperty);
            }

            set
            {
                SetValue(AutoExecuteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the calculated members are to be enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if calculated members are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCalculatedMembersEnabled
        {
            get { return (bool)GetValue(IsCalculatedMembersEnabledProperty); }
            set { SetValue(IsCalculatedMembersEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Virtual Kpi support are to be enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if virtual kpi support are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtualKpiEnabled
        {
            get { return (bool)GetValue(IsVirtualKpiEnabledProperty); }
            set { SetValue(IsVirtualKpiEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for enabling sorting in Cube Browser measure
        /// </summary>
        public SortCubeMeasureOrder MeasureSortOrderInCubeBrowser
        {
            get { return (SortCubeMeasureOrder)GetValue(MeasureSortOrderInCubeBrowserProperty); }
            set { SetValue(MeasureSortOrderInCubeBrowserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowMeasureSorting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasureSortOrderInCubeBrowserProperty =
            DependencyProperty.Register("MeasureSortOrderInCubeBrowser", typeof(SortCubeMeasureOrder), typeof(OlapClient), new UIPropertyMetadata(SortCubeMeasureOrder.ASC, (obj, args) => 
            {
                OlapClient client = obj as OlapClient;
                if (client != null && !DesignerProperties.GetIsInDesignMode(client))
                {
                    client.DataBind();
                }
            }));
       
        /// <summary>
        /// Gets or sets the measure group caption
        /// </summary>
        public System.Collections.Generic.Dictionary<string,string> MeasureGroupNameCaptions
        {
            get { return (System.Collections.Generic.Dictionary<string,string>)GetValue(MeasureGroupNameCaptionsProperty); }
            set { SetValue(MeasureGroupNameCaptionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Declaring dependency property ot get measure group caption
        /// </summary>
        public static readonly DependencyProperty MeasureGroupNameCaptionsProperty =
            DependencyProperty.Register("MeasureGroupNameCaptions", typeof(System.Collections.Generic.Dictionary<string, string>), typeof(OlapClient), new UIPropertyMetadata(null,
                (obj, arg) =>
                {
                    OlapClient client = obj as OlapClient;
                    if (client != null && !DesignerProperties.GetIsInDesignMode(client))
                    {
                        client.DataBind();
                    }
                }));


        /// <summary>
        /// Gets or sets the OlapDataManager.
        /// </summary>
        /// <value>The OlapDataManager.</value>
        public OlapDataManager OlapDataManager
        {
            get
            {
                return (OlapDataManager)GetValue(OlapDataManagerProperty);
            }
            set
            {
                SetValue(OlapDataManagerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the OlapGrid.
        /// </summary>
        /// <value>The OlapGrid.</value>
        [Browsable(false)]
        public Syncfusion.Windows.Grid.Olap.OlapGrid OlapGrid
        {
            get
            {
                return (Syncfusion.Windows.Grid.Olap.OlapGrid)GetValue(OlapGridProperty);
            }

            set
            {
                SetValue(OlapGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        public DisplayModes DisplayMode
        {
            get { return (DisplayModes)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        //Properties added for Skin support

        /// <summary>
        /// Gets or sets the Header Background.
        /// </summary>
        /// <value>The show subset filters.</value>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header BorderBrush.
        /// </summary>
        /// <value>The show subset filters.</value>
        public Brush HeaderBorderBrush
        {
            get { return (Brush)GetValue(HeaderBorderBrushProperty); }
            set { SetValue(HeaderBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header Hover Background.
        /// </summary>
        /// <value>The show subset filters.</value>
        public Brush HeaderMouseOverBackground
        {
            get { return (Brush)GetValue(HeaderMouseOverBackgroundProperty); }
            set { SetValue(HeaderMouseOverBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header Hover BorderBrush.
        /// </summary>
        /// <value>The show subset filters.</value>
        public Brush HeaderMouseOverBorderBrush
        {
            get { return (Brush)GetValue(HeaderMouseOverBorderBrushProperty); }
            set { SetValue(HeaderMouseOverBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header Hover BorderBrush.
        /// </summary>
        /// <value>The show subset filters.</value>
        public bool IsDragOver
        {
            get { return (bool)GetValue(IsDragOverProperty); }
            set { SetValue(IsDragOverProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the VisualStyle for OlapClient
        /// </summary>
        public OlapClientVisualStyle VisualStyle
        {
            get { return (OlapClientVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        /// <summary>
        /// Gets or Sets Value for ColorEachSeries while ChartType is Pie
        /// </summary>
        public bool ColorEachPieSeries { get; set; }

       

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayModes), typeof(OlapClient),
            new UIPropertyMetadata(DisplayModes.Both, OnDisplayModePropertyChanged));

        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.OlapDataManager != null)
            {
                client.SetDisplayMode(client);
            }
        }

        private void SetDisplayMode(OlapClient olapClient)
        {
            switch (olapClient.DisplayMode)
            {
                case DisplayModes.Both:
                    olapClient.olapChart.Visibility = Visibility.Visible;
                    olapClient.olapChartTab.Visibility = Visibility.Visible;
                    olapClient.olapGrid.Visibility = Visibility.Visible;
                    olapClient.olapGridTab.Visibility = Visibility.Visible;
                    olapClient.olapChartTab.IsSelected = true;
                    olapClient.olapGrid.OlapDataManager = olapClient.OlapDataManager;
                    olapClient.olapGrid.FlowDirection = this.FlowDirection;
                    olapClient.olapGrid.ApplyTemplate();
                    olapClient.olapChart.OlapDataManager = olapClient.OlapDataManager;
                    break;
                case DisplayModes.ChartOnly:
                    olapClient.olapChart.Visibility = Visibility.Visible;
                    olapClient.olapChartTab.Visibility = Visibility.Collapsed;
                    olapClient.olapGrid.Visibility = Visibility.Collapsed;
                    olapClient.olapGridTab.Visibility = Visibility.Collapsed;
                    olapClient.olapChartTab.IsSelected = true;
                    olapClient.olapChart.OlapDataManager = olapClient.OlapDataManager;
                    olapClient.olapGrid.OlapDataManager = null;
                    break;
                case DisplayModes.GridOnly:
                    olapClient.olapChart.Visibility = Visibility.Collapsed;
                    olapClient.olapChartTab.Visibility = Visibility.Collapsed;
                    olapClient.olapGrid.Visibility = Visibility.Visible;
                    olapClient.olapGridTab.Visibility = Visibility.Collapsed;
                    olapClient.olapGridTab.IsSelected = true;
                    olapClient.olapChart.OlapDataManager = null;
                    olapClient.olapGrid.FlowDirection = this.FlowDirection;
                    olapClient.olapGrid.OlapDataManager = olapClient.OlapDataManager;
                    olapClient.olapGrid.ApplyTemplate();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [load default report].
        /// </summary>
        /// <value><c>true</c> if [load default report]; otherwise, <c>false</c>.</value>
        public bool LoadWithDefaultReport
        {
            get { return _loadDefaultReport; }
            set { _loadDefaultReport = value; }
        }

        /// <summary>
        /// Gets or sets the TabControlExtStyle.
        /// </summary>
        /// <value>TabControlExtStyle.</value>
        public Style TabControlExtStyle
        {
            get { return tabstyle; }
            set
            {
                tabstyle = value;
                olapTabControl.Style = tabstyle;
            }
        }

        /// <summary>
        /// Gets or sets the CubeDimensionBrowserStyle.
        /// </summary>
        /// <value>CubeDimensionBrowserStyle.</value>
        public Style CubeDimensionBrowserStyle
        {
            get { return treeviewstyle; }
            set
            {
                treeviewstyle = value;
                cubeDimensionBrowser.Style = treeviewstyle;
            }
        }

        /// <summary>
        /// Gets or sets the ListBoxStyle.
        /// </summary>
        /// <value>ListBoxStyle.</value>
        public Style ListBoxStyle
        {
            get { return listboxstyle; }
            set
            {
                listboxstyle = value;
                axisElementBuiderCategorical.AxisElementBuilderStyle = listboxstyle;
                axisElementBuiderSeries.AxisElementBuilderStyle = listboxstyle;
                axisElementBuiderSlicer.AxisElementBuilderStyle = listboxstyle;
            }
        }

        #endregion

        #region Binding Controls with OlapDataManager
        /// <summary>
        /// Bind the data to OlapClient.
        /// </summary>
        public void DataBind()
        {
            WaitingDialog waitingDialog = new WaitingDialog();
            waitingDialog.FlowDirection = this.FlowDirection;
            Window parentWindow = Common.GetParentWindow<Window>(this);
            if (parentWindow != null)
            {
                waitingDialog.Owner = parentWindow;
            }
            waitingDialog.Show();
            try
            {
                this.ConnectionString = this.OlapDataManager.ConnectionString;
                this.ProviderName = this.OlapDataManager.DataProvider.ProviderName;
                this.cubeDimensionBrowser.MeasureSortOrderInCubeBrowser = this.MeasureSortOrderInCubeBrowser;
                this.cubeDimensionBrowser.MeasureGroupNameCaption = this.MeasureGroupNameCaptions;
                this.cubeDimensionBrowser.OlapDataManager = this.OlapDataManager;
                this.cubeSelector.OlapDataManager = this.OlapDataManager;
                this.axisElementBuiderCategorical.OlapDataManager = this.OlapDataManager;
                this.axisElementBuiderSeries.OlapDataManager = this.OlapDataManager;
                this.axisElementBuiderSlicer.OlapDataManager = this.OlapDataManager;
                this.olapGrid.ShowHeaderCellsToolTip = (bool)this.btnShowHeaderCellsToolTip.IsChecked;
                this.olapGrid.ShowValueCellToolTip = (bool)this.btnShowValueCellsToolTip.IsChecked;
                if (this.reportList.Items.Count <= 0)
                {
                    if (this.OlapDataManager.Reports.Count == 0)
                    {
                        this.AddReport("Report1");
                        if (this.LoadWithDefaultReport)
                        {
                            this.LoadDefaultData();
                        }
                    }
                    else
                    {
                        this.reportList.Reports = this.OlapDataManager.Reports;
                        if (this.OlapDataManager.CurrentReport != null)
                        {
                            this.reportList.SelectedItem = this.OlapDataManager.CurrentReport.Name;
                            if (this.OlapDataManager.CurrentReport.CalculatedMembers.Count > 0)
                            {
                                this.IsCalculatedMembersEnabled = true;
                            }
                            if (this.OlapDataManager.CurrentReport.VirtualKpiElements.Count > 0)
                            {
                                this.IsVirtualKpiEnabled = true;
                            }
                        }
                    }
                }
                
                SetDisplayMode(this);
                try
                {
                    this.OlapDataManager.CurrentReport.ChartSettings.LegendVisibility = (this.olapChart.ChartAppearance.LegendVisibility) ? true : false;
                    if (this.olapChart.ChartAppearance.XAxisForeGround != null)
                    {
                        this.OlapDataManager.CurrentReport.ChartSettings.XAxisForeGround = this.olapChart.ChartAppearance.XAxisForeGround;
                    }
                    if (this.olapChart.ChartAppearance.YAxisForeGround != null)
                    {
                        this.OlapDataManager.CurrentReport.ChartSettings.YAxisForeGround = this.olapChart.ChartAppearance.YAxisForeGround;
                    }
                }
                catch
                {

                }
                this.OlapDataManager.NotifyReportChanged();                
                waitingDialog.Close();
            }
            catch (Exception ex)
            {
                waitingDialog.Close();
                MessageBox.Show(ex.Message, "Error in Binding Model");
            }

        }

        /// <summary>
        /// Binds the model to the controls in OLAPClient
        /// </summary>
        private void BindModelToControls()
        {
            try
            {
                if (string.IsNullOrEmpty(this.ConnectionString))
                {
                    this.ShowConnectOptionWindow();
                }
                else
                {
                    WaitingDialog waitingDialog = new WaitingDialog();
                    waitingDialog.FlowDirection = this.FlowDirection;
                    Window parentWindow = Common.GetParentWindow<Window>(this);
                    if (parentWindow != null)
                        waitingDialog.Owner = parentWindow;
                    try
                    {
                        waitingDialog.Show();
                        OlapDataManager newDataManager = new OlapDataManager(this.ConnectionString);
                        newDataManager.DataProvider.ProviderName = this.ProviderName;
                        this.OlapDataManager = newDataManager;
                        bool legendvisibility = this.OlapDataManager.CurrentReport.ChartSettings.LegendVisibility;
                        var xforeground = this.OlapDataManager.CurrentReport.ChartSettings.XAxisForeGround;
                        var yforeground = this.OlapDataManager.CurrentReport.ChartSettings.YAxisForeGround;
                        this.cubeDimensionBrowser.MeasureSortOrderInCubeBrowser = this.MeasureSortOrderInCubeBrowser;
                        this.cubeDimensionBrowser.MeasureGroupNameCaption = this.MeasureGroupNameCaptions;
                        this.cubeDimensionBrowser.OlapDataManager = this.OlapDataManager;
                        this.cubeSelector.OlapDataManager = this.OlapDataManager;
                        this.axisElementBuiderCategorical.OlapDataManager = this.OlapDataManager;
                        this.axisElementBuiderSeries.OlapDataManager = this.OlapDataManager;
                        this.axisElementBuiderSlicer.OlapDataManager = this.OlapDataManager;
                        this.reportList.Items.Clear();
                        this.AddReport("Report1");
                        if (this.LoadWithDefaultReport)
                        {
                            this.LoadDefaultData();
                        }

                        this.olapGrid.ShowHeaderCellsToolTip = (bool)this.btnShowHeaderCellsToolTip.IsChecked;
                        this.olapGrid.ShowValueCellToolTip = (bool)this.btnShowValueCellsToolTip.IsChecked;

                        SetDisplayMode(this);
                        try
                        {
                            this.OlapDataManager.CurrentReport.ChartSettings.LegendVisibility = (legendvisibility) ? true : false;
                            if (this.olapChart.ChartAppearance.XAxisForeGround != null)
                            {
                                this.OlapDataManager.CurrentReport.ChartSettings.XAxisForeGround = xforeground;
                            }
                            if (this.olapChart.ChartAppearance.YAxisForeGround != null)
                            {
                                this.OlapDataManager.CurrentReport.ChartSettings.YAxisForeGround = yforeground;
                            }
                        }
                        catch
                        {

                        }
                        this.OlapDataManager.NotifyReportChanged();
                        this.OlapDataManager.NotifyElementModified();

                        waitingDialog.Close();
                    }
                    catch (Exception ex)
                    {
                        if (waitingDialog.IsActive)
                        {
                            waitingDialog.Close();
                        }

                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in binding the model");
            }
        }

        /// <summary>
        /// Loads the default data.
        /// </summary>
        private void LoadDefaultData()
        {
            ///// Loading the Default data
            CubeSchema schema = this.OlapDataManager.DataProvider.GetCubeSchema(this.OlapDataManager.CurrentCubeName);
            //// Extracting cube meta data
            Syncfusion.Olap.Data.Dimension timeDimension = schema.GetTimeDimension();
            if (timeDimension != null)
            {
                Hierarchy defaultHierarchy = schema.GetHierarchyByUniqueName(timeDimension.DefaultHierarchyName);
                Level defaultLevel = schema.GetLevelByUniqueName(defaultHierarchy.DefaultLevelUniqueName);
                //// Creating Elements
                DimensionElement dimensionElement = new DimensionElement();
                dimensionElement.Name = timeDimension.Name;
                dimensionElement.AddLevel(defaultHierarchy.Name, defaultLevel.Name);

                this.OlapDataManager.CurrentReport.SeriesElements.Add(new Item { ElementValue = dimensionElement, Axis = AxisPosition.Series });
            }
            Member defaultMeasure = schema.GetDefaultMeasure();
            Item seriesItem = this.GetDefaultMeasureElements(defaultMeasure);
            this.OlapDataManager.CurrentReport.CategoricalElements.Add(seriesItem);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Shows the column filter dialog.
        /// </summary>
        public void ShowColumnFilterDialog()
        { 
            try
            {
                if (this.OlapDataManager.CurrentReport.CategoricalElements.Count > 0)
                {
                    FilterSortDialog filterSortDialog = new FilterSortDialog(this.OlapDataManager, AxisPosition.Categorical, true, this.ShowAllMeasuresInFilterSortDlg,this.VisualStyle.ToString());
                    filterSortDialog.FlowDirection = this.FlowDirection;
                    SkinStorage.SetVisualStyle(filterSortDialog, SkinStorage.GetVisualStyle(this));
                    filterSortDialog.ShowDialog();
                    this.UpdateShowExpanderState();
                }
                else
                {
                    MessageBox.Show("There is no element in categorical axis to apply filter, \n Please add elements and then try again.", "Olap Client", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Shows the row filter dialog.
        /// </summary>
        public void ShowRowFilterDialog()
        {
            try
            {
                if (this.OlapDataManager.CurrentReport.SeriesElements.Count > 0)
                {
                    FilterSortDialog filterSortDialog = new FilterSortDialog(this.OlapDataManager, AxisPosition.Series, true,this.ShowAllMeasuresInFilterSortDlg,this.VisualStyle.ToString());
                    filterSortDialog.FlowDirection = this.FlowDirection;
                    SkinStorage.SetVisualStyle(filterSortDialog, SkinStorage.GetVisualStyle(this));
                    filterSortDialog.ShowDialog();
                    this.UpdateShowExpanderState();
                }
                else
                {
                    MessageBox.Show("There is no element in series axis to apply filter, \n Please add elements and then try again.", "Olap Client", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Exports the OlapGrid to excel.
        /// </summary>
        public void ExportToExcel()
        {
            try
            {
                if (this.OlapDataManager != null && this.olapGrid != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "OlapGrid Report";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.DefaultExt = "xls";
                    saveFileDialog.Filter = "Excel (.xls)|*.xls";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        GridExcelExport excelExport = new GridExcelExport(this.olapGrid.OlapDataManager.PivotEngine, this.olapGrid.GridStyleInfo, this.olapGrid.Layout, this.olapGrid.OlapDataManager.ItemSource == null ? false : true);
                        excelExport.Export(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Shows the column sorting dialog.
        /// </summary>
        [Obsolete("Kindly use ShowColumnSortingDialog() method.")]
        public void ShowColumSortingDialog()
        {
            this.ShowColumnSortingDialog();
        }

        /// <summary>
        /// Shows the column sorting dialog.
        /// </summary>
        public void ShowColumnSortingDialog()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.OlapDataManager.CurrentReport.CategoricalElements.Count > 0)
                    {
                        FilterSortDialog filterSortDialog = new FilterSortDialog(this.OlapDataManager, AxisPosition.Categorical, false, this.ShowAllMeasuresInFilterSortDlg,this.VisualStyle.ToString());
                        filterSortDialog.FlowDirection = this.FlowDirection;
                        SkinStorage.SetVisualStyle(filterSortDialog, SkinStorage.GetVisualStyle(this));
                        filterSortDialog.ShowDialog();
                        this.UpdateShowExpanderState();
                    }
                    else
                    {
                        MessageBox.Show("There is no element in categorical axis to apply sorting, \n Please add elements and then try again.", "Olap Client", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Shows the row sorting dialog.
        /// </summary>
        public void ShowRowSortingDialog()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.OlapDataManager.CurrentReport.SeriesElements.Count > 0)
                    {
                        FilterSortDialog filterSortDialog = new FilterSortDialog(this.OlapDataManager, AxisPosition.Series, false,this.ShowAllMeasuresInFilterSortDlg,this.VisualStyle.ToString());
                        filterSortDialog.FlowDirection = this.FlowDirection;
                        SkinStorage.SetVisualStyle(filterSortDialog, SkinStorage.GetVisualStyle(this));
                        filterSortDialog.ShowDialog();
                        this.UpdateShowExpanderState();
                    }
                    else
                    {
                        MessageBox.Show("There is no element in series axis to apply sorting, \n Please add elements and then try again.", "Olap Client", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Updates the state of the show expander.
        /// </summary>
        private void UpdateShowExpanderState()
        {
            if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
            {
                if (!this.OlapDataManager.CurrentReport.SeriesElements.IsFilterOrSortOn && !this.OlapDataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn)
                {
                    this.btnShowExpanders.IsChecked = this.OlapDataManager.CurrentReport.ShowExpanders;
                    this.btnShowExpanders.IsEnabled = true;
                }
                else
                {
                    this.btnShowExpanders.IsChecked = false;
                    this.btnShowExpanders.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Shows the connection option.
        /// </summary>
        public void ShowConnectOption()
        {
            try
            {
                bool? dialogResult = this.ShowConnectOptionWindow();
                if (dialogResult == true)
                {
                    this.BindModelToControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occurred while connecting to server");
            }
        }

        /// <summary>
        /// Shows the connect option window.
        /// </summary>
        /// <returns></returns>
        private bool? ShowConnectOptionWindow()
        {
            try
            {
                ConnectOptions connectOptions = new ConnectOptions(this.ConnectionString, this.ProviderName);
                connectOptions.FlowDirection = this.FlowDirection;
                SkinStorage.SetVisualStyle(connectOptions, SkinStorage.GetVisualStyle(this));
                bool? dialogStatus = connectOptions.ShowDialog();
                if (dialogStatus == true)
                {
                    this.ConnectionString = connectOptions.ConnectionString;
                    this.ProviderName = connectOptions.ProviderName;
                }

                return dialogStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occurred while displaying the connect to server dialog");
            }

            return false;
        }

        /// <summary>
        /// Shows the connect option window and bind model to controls.
        /// </summary>
        [Obsolete("Use ShowConnectionOption() method instead", false)]
        public void ShowConnectOptionWindowAndBindModelToControls()
        {
            try
            {
                bool? dialogResult = this.ShowConnectOptionWindow();
                if (dialogResult == true)
                {
                    this.BindModelToControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occurred while connecting to server");
            }
        }

        /// <summary>
        /// Exports the OlapGrid to word.
        /// </summary>
        public void ExportToWord()
        {
            try
            {
                if (this.OlapDataManager != null && this.olapGrid != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "OlapGrid Report";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.DefaultExt = "doc";
                    saveFileDialog.Filter = "Word (.doc)|*.doc";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        GridWordExport gridWordExport = new GridWordExport(this.olapGrid.OlapDataManager.PivotEngine, this.olapGrid.Layout);
                        gridWordExport.Export(fileName, this.olapGrid.GridStyleInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Exports the OlapGrid to PDF.
        /// </summary>
        public void ExportToPdf()
        {
            try
            {
                if (this.OlapDataManager != null && this.olapGrid != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "OlapGrid Report";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.DefaultExt = "pdf";
                    saveFileDialog.Filter = "Pdf (.pdf)|*.pdf";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        GridPdfExport gridPdfExport = new GridPdfExport(this.olapGrid.OlapDataManager.PivotEngine, this.olapGrid.GridStyleInfo);
                        gridPdfExport.Export(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Execute the MDX query.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SetExecute(object sender, EventArgs e)
        {
            try
            {
                this.axisElementBuiderCategorical.RefreshOlapDataManagerElementItems(axisElementBuiderCategorical, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occurred while executing the query");
            }
        }

        /// <summary>
        /// Gets the sub set element.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="items">The items.</param>
        private void GetSubSetElement(string newValue, Syncfusion.Olap.Reports.Items items)
        {
            int newSubSetValue;
            if (Int32.TryParse(newValue, out newSubSetValue) && !String.IsNullOrEmpty(newValue))
            {
                SubsetElement subsetElement = new SubsetElement();
                subsetElement.EndIndex = newSubSetValue;
                items.SubSetElement = subsetElement;
            }
            else
            {
                items.SubSetElement = null;
            }

            //Updates the olap controls
            this.OlapDataManager.NotifyElementModified();
        }
        #endregion

        #region Report Manipulations
        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        private void AddReport(string reportName)
        {
            if (this.OlapDataManager != null)
            {
                OlapReport olapReport = new OlapReport(reportName);
                olapReport.EngineVersion = QueryBuilderEngineVersions.Version3;
                //if (this.ShowOlapChart)
                //    olapReport.ChartSettings = this.olapChart.ChartAppearance;
                this.OlapDataManager.AddReport(olapReport.Name);
                OlapDataManager.SetCurrentReport(olapReport);
                this.btnRemoveReport.IsEnabled = this.OlapDataManager.Reports.Count > 1;

                this.reportList.Reports = this.OlapDataManager.Reports;
                this.reportList.SelectedItem = reportName;
            }
            else
            {
                MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                this.ShowConnectOption();
            }
        }

        /// <summary>
        /// Creates the new report.
        /// </summary>
        public void CreateNewReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.OlapDataManager.IsCurrentReportModified)
                    {
                        MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to save changes to the report?", "OlapClient", MessageBoxButton.YesNoCancel);
                        if (msgBoxResult == MessageBoxResult.Yes)
                        {
                            this.SaveReport();
                            this.OlapDataManager.Reports = new OlapReportCollection();
                            this.olapChart.ChartAppearance = new ChartAppearanceSettings();
                            this.btnRemoveReport.IsEnabled = false;
                            this.AddReport();
                        }
                        else if (msgBoxResult == MessageBoxResult.No)
                        {
                            this.OlapDataManager.Reports = new OlapReportCollection();
                            this.btnRemoveReport.IsEnabled = false;
                            this.AddReport();
                        }
                    }
                    else
                    {
                        this.OlapDataManager.Reports = new OlapReportCollection();
                        this.olapChart.ChartAppearance = new ChartAppearanceSettings();
                        this.btnRemoveReport.IsEnabled = false;
                        this.AddReport();
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Loads the report.
        /// </summary>
        public void LoadReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.OlapDataManager.IsCurrentReportModified)
                    {
                        MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to save changes to the report?", "OlapClient", MessageBoxButton.YesNoCancel);
                        if (msgBoxResult == MessageBoxResult.Yes)
                        {
                            this.SaveReport();
                            this.LoadReports();
                        }
                        else if (msgBoxResult == MessageBoxResult.No)
                        {
                            this.LoadReports();
                        }
                    }
                    else
                    {
                        this.LoadReports();
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }


        /// <summary>
        /// Loads the report stream.
        /// </summary>
        /// <param name="reportStream">The report stream.</param>
        public void LoadReportStream(System.IO.Stream reportStream)
        {
            try
            {
                if (this.OlapDataManager != null && reportStream != null)
                {
                    this.OlapDataManager.LoadReportDefinitionFromStream(reportStream);
                    if (this.OlapDataManager.Reports.Count > 0)
                    {
                        this.reportList.Reports = this.OlapDataManager.Reports;
                        this.reportList.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error:" + ex.Message, "OlapClient");
            }

        }

        /// <summary>
        /// Gets the report stream.
        /// </summary>
        /// <returns></returns>
        public System.IO.Stream GetReportStream()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.OlapDataManager.Reports.Count > 0)
                    {
                        return (System.IO.MemoryStream)this.OlapDataManager.GetReportAsStream();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error:" + ex.Message, "OlapClient");
            }
            return null;
        }

        /// <summary>
        /// Saves the report.
        /// </summary>
        public void SaveReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {

                    string currentReport = string.Empty;
                    if (this.reportList.SelectedItem != null)
                        currentReport = this.reportList.SelectedItem.ToString();
                    ////Updating chart & grid appearance settings
                    this.OlapDataManager.CurrentReport.ChartSettings = this.OlapChart.ChartAppearance;
                    this.OlapDataManager.CurrentReport.GridSettings = this.OlapGrid.GridSettings;
                    if (string.IsNullOrEmpty(this.OlapDataManager.ReportPath))
                    {
                        if (this.SaveReports(currentReport))
                        {
                            this.reportList.Reports = this.OlapDataManager.Reports;
                            if (currentReport != string.Empty)
                            {
                                this.reportList.SelectedItem = currentReport;
                                this.btnRemoveReport.IsEnabled = OlapDataManager.Reports.Count > 1;
                                if (this.OlapDataManager.Reports.Count > 0)
                                {
                                    this.btnRenameReport.IsEnabled = true;
                                    this.reportList.SelectedItem = this.OlapDataManager.Reports[0].Name;
                                }

                            }
                        }
                    }
                    else
                    {
                        this.OlapDataManager.SaveReport(this.OlapDataManager.ReportPath);
                        this.reportList.Reports = this.OlapDataManager.Reports;
                        if (currentReport != string.Empty)
                        {
                            this.reportList.SelectedItem = currentReport;
                            if (OlapDataManager.Reports.Count > 1)
                            {
                                this.btnRemoveReport.IsEnabled = true;
                            }
                            else
                            {
                                this.btnRemoveReport.IsEnabled = false;
                            }
                            if (this.OlapDataManager.Reports.Count > 0)
                            {
                                this.btnRenameReport.IsEnabled = true;
                                this.reportList.SelectedItem = this.OlapDataManager.Reports[0].Name;
                            }

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    //// Calling the connect option method
                    ShowConnectOptionWindowAndBindModelToControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Report Builder");
            }
        }

        /// <summary>
        /// Saves as report.
        /// </summary>
        public void SaveAsReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    string currentReport = this.reportList.SelectedItem.ToString();
                    if (this.SaveReports(currentReport))
                    {
                        currentReport = string.Empty;
                        if (this.reportList.SelectedItem != null)
                        {
                            currentReport = this.reportList.SelectedItem.ToString();
                        }

                        if (currentReport == string.Empty && this.OlapDataManager.CurrentReport != null)
                        {
                            currentReport = this.OlapDataManager.CurrentReport.Name;
                        }

                        this.OlapDataManager.CurrentReport.ChartSettings = this.olapChart.ChartAppearance;
                        this.reportList.Reports = this.OlapDataManager.Reports;
                        if (currentReport != string.Empty)
                        {
                            this.reportList.SelectedItem = currentReport;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Adds the report.
        /// </summary>
        public void AddReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    OlapDataManager.PivotEngine = null;

                    List<string> reportNames = (from r in this.OlapDataManager.Reports.Cast<OlapReport>() select r.Name).ToList();
                    ReportNameWindow renameReport = new ReportNameWindow(reportNames);
                    renameReport.FlowDirection = this.FlowDirection;
                    SkinStorage.SetVisualStyle(renameReport, SkinStorage.GetVisualStyle(this));
                    renameReport.ReportName = "Report" + (this.OlapDataManager.Reports.Count + 1).ToString();
                    if (renameReport.ShowDialog() == true)
                    {
                        this.AddReport(renameReport.ReportName);
                        this.olapChart.ChartAppearance = new ChartAppearanceSettings();
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Removes the report.
        /// </summary>
        public void RemoveReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.reportList.SelectedItem != null && this.OlapDataManager.Reports.Count > 1)
                    {
                        this.OlapDataManager.RemoveReport(this.reportList.SelectedItem.ToString());
                        if (this.OlapDataManager.Reports.Count > 1)
                        {
                            this.btnRemoveReport.IsEnabled = true;
                        }
                        else
                        {
                            this.btnRemoveReport.IsEnabled = false;
                        }

                        this.reportList.Reports = this.OlapDataManager.Reports;
                        if (this.OlapDataManager.CurrentReport != null)
                        {
                            this.reportList.SelectedItem = this.OlapDataManager.CurrentReport.Name;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a report to remove or OlapClient view needs atleast one report.", "Olap Client");
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Renames the report.
        /// </summary>
        public void RenameReport()
        {
            try
            {
                if (this.OlapDataManager != null)
                {
                    if (this.reportList.SelectedItem != null)
                    {
                        List<string> reportNames = (from r in this.OlapDataManager.Reports.Cast<OlapReport>() select r.Name).ToList();
                        ReportNameWindow renameReport = new ReportNameWindow(reportNames);
                        renameReport.FlowDirection = this.FlowDirection;
                        renameReport.ReportName = this.reportList.SelectedItem.ToString();
                        SkinStorage.SetVisualStyle(renameReport, SkinStorage.GetVisualStyle(this));
                        if (renameReport.ShowDialog() == true)
                        {
                            this.OlapDataManager.RenameReport(this.reportList.SelectedItem.ToString(), renameReport.ReportName);
                            this.reportList.Reports = this.OlapDataManager.Reports;
                            this.reportList.SelectedItem = renameReport.ReportName;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Gets the default measure elements.
        /// </summary>
        /// <param name="memberMeasureObj">The member measure obj.</param>
        /// <returns></returns>
        private Item GetDefaultMeasureElements(Member memberMeasureObj)
        {
            try
            {
                MeasureElements measureElements = new MeasureElements();
                measureElements.Add(new MeasureElement { UniqueName = memberMeasureObj.UniqueName });
                return new Item { ElementValue = measureElements };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OLAPClient");
            }

            return null;
        }

        /// <summary>
        /// Loads the reports form report file.
        /// </summary>
        private void LoadReports()
        {
            try
            {
                if (this.ConnectionString != string.Empty || this.OlapDataManager == null)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.AddExtension = true;
                    openFileDialog.DefaultExt = "xml";
                    openFileDialog.Filter = "Report (.xml)|*.xml";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        string fileName = openFileDialog.FileName;
                        this.OlapDataManager.LoadReportDefinitionFile(fileName);
                        this.reportList.Reports = this.OlapDataManager.Reports;
                        if (this.OlapDataManager.Reports.Count > 1)
                        {
                            this.btnRemoveReport.IsEnabled = true;
                        }
                        else
                        {
                            this.btnRemoveReport.IsEnabled = false;
                        }

                        if (this.OlapDataManager.Reports.Count > 0)
                        {
                            this.btnRenameReport.IsEnabled = true;
                            this.reportList.SelectedItem = this.OlapDataManager.Reports[0].Name;
                            if (this.ShowSubsetFilters)
                            {
                                if (this.OlapDataManager.Reports[0].CategoricalElements.SubSetElement != null && this.OlapDataManager.Reports[0].SeriesElements.SubSetElement != null)
                                {
                                    this.subsetFilterCategorical.ValueBox.Text = this.OlapDataManager.Reports[0].CategoricalElements.SubSetElement.EndIndex.ToString();
                                    this.subsetFilterRows.ValueBox.Text = this.OlapDataManager.Reports[0].SeriesElements.SubSetElement.EndIndex.ToString();
                                }
                            }
                            if (this.OlapDataManager.Reports[0].TogglePivot)
                            {
                                this.btnTogglePivot.IsChecked = true;
                            }
                            else
                            {
                                this.btnTogglePivot.IsChecked = false;
                            }
                            if (OlapDataManager.Reports[0].ShowExpanders)
                            {
                                this.btnShowExpanders.IsChecked = true;
                            }
                            else
                            {
                                this.btnShowExpanders.IsChecked = false;
                            }

                            for (int i = 0; i < cmb_ChartType.Items.Count; i++)
                            {
                                if (((ImageData)cmb_ChartType.Items[i]).Text == this.OlapDataManager.CurrentReport.ChartSettings.ChartType)
                                {
                                    this.cmb_ChartType.SelectedIndex = i;
                                    break;
                                }
                            }
                            for (int i = 0; i < this.cmb_ChartPalette.Items.Count; i++)
                            {
                                if (((ImageData)cmb_ChartPalette.Items[i]).Text == this.OlapDataManager.CurrentReport.ChartSettings.ChartColorPalette)
                                {
                                    this.cmb_ChartPalette.SelectedIndex = i;
                                    break;
                                }
                            }
                            for (int i = 0; i < this.cmb_GridLayoutType.Items.Count; i++)
                            {
                                if (((ImageData)cmb_GridLayoutType.Items[i]).Text == this.OlapDataManager.CurrentReport.GridSettings.GridLayout)
                                {
                                    this.cmb_GridLayoutType.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to the server first then try loading the report", "Server connection not found");
                    this.ShowConnectOption();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OLAP Client");
            }
        }

        /// <summary>
        /// Saves the report.
        /// </summary>
        /// <returns></returns>
        private bool SaveReports(String currentReport)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = currentReport;
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = "xml";
                saveFileDialog.Filter = "Report (.xml)|*.xml";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    this.OlapDataManager.SaveReport(fileName);
                    this.reportList.Reports = this.OlapDataManager.Reports;
                    if (this.reportList.IsEnabled)
                    {
                        this.btnAddReport.IsEnabled = true;
                        this.btnRemoveReport.IsEnabled = true;
                        this.btnRenameReport.IsEnabled = true;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured while saving the report");
            }
            return false;
        }

        #endregion

        #region Report Events
        /// <summary>
        /// Handles the Click event of the btnRemoveReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnRemoveReport_Click(object sender, RoutedEventArgs e)
        {
            this.RemoveReport();
        }

        /// <summary>
        /// Handles the Click event of the btnRenameReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnRenameReport_Click(object sender, RoutedEventArgs e)
        {
            this.RenameReport();
        }

        /// <summary>
        /// Handles the Click event of the btnSaveAsReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSaveAsReport_Click(object sender, RoutedEventArgs e)
        {
            this.SaveAsReport();
        }

        /// <summary>
        /// Handles the Click event of the btnSaveReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSaveReport_Click(object sender, RoutedEventArgs e)
        {
            this.SaveReport();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the reportList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void reportList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (this.reportList.SelectedItem != null)
                {
                    OlapDataManager.SetCurrentReport(this.OlapDataManager.Reports[this.reportList.SelectedIndex]);
                    OlapDataManager.PivotEngine = null;

                    if (this.OlapDataManager.CurrentReport.TogglePivot)
                    {
                        this.btnTogglePivot.IsChecked = true;
                    }
                    else
                    {
                        this.btnTogglePivot.IsChecked = false;
                    }

                    if (OlapDataManager.CurrentReport.ShowExpanders)
                    {
                        this.btnShowExpanders.IsChecked = true;
                    }
                    else
                    {
                        this.btnShowExpanders.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnLoadReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnLoadReport_Click(object sender, RoutedEventArgs e)
        {
            this.LoadReport();
        }

        /// <summary>
        /// Handles the Click event of the btnNewReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnNewReport_Click(object sender, RoutedEventArgs e)
        {
            this.CreateNewReport();
        }

        /// <summary>
        /// Handles the Click event of the btnAddReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAddReport_Click(object sender, RoutedEventArgs e)
        {
            this.AddReport();
        }
        #endregion

        #region Chart ToolBar Events
        /// <summary>
        /// Handles the SelectionChanged event of the cmb_ChartPalette control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmb_ChartPalette_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (olapChart != null && olapChart.ColorModel != null)
                {
                    if (this.cmb_ChartPalette.SelectedIndex == 0)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Default;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 1)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.DefaultAlpha;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 2)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Analog;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 3)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Colorful;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 4)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.EarthTone;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 5)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Grayscale;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 6)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Nature;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 7)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Pastel;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 8)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Triad;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 9)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.WarmCold;
                    }
                    if (this.cmb_ChartPalette.SelectedIndex == 10)
                    {
                        this.olapChart.ColorModel.Palette = ChartColorPalette.Custom;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmb_ChartType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmb_ChartType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (olapChart != null)
                {
                    if (this.cmb_ChartType.SelectedIndex == 0)
                    {
                        this.olapChart.ChartType = ChartTypes.Column;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 1)
                    {
                        this.olapChart.ChartType = ChartTypes.StackingColumn;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 2)
                    {
                        this.olapChart.ChartType = ChartTypes.StackingColumn100;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 3)
                    {
                        this.olapChart.ChartType = ChartTypes.Bar;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 4)
                    {
                        this.olapChart.ChartType = ChartTypes.StackingBar;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 5)
                    {
                        this.olapChart.ChartType = ChartTypes.Area;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 6)
                    {
                        this.olapChart.ChartType = ChartTypes.StackingArea;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 7)
                    {
                        this.olapChart.ChartType = ChartTypes.SplineArea;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 8)
                    {
                        this.olapChart.ChartType = ChartTypes.StepArea;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 9)
                    {
                        this.olapChart.ChartType = ChartTypes.Line;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 10)
                    {
                        this.olapChart.ChartType = ChartTypes.Spline;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 11)
                    {
                        this.olapChart.ChartType = ChartTypes.RotatedSpline;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 12)
                    {
                        this.olapChart.ChartType = ChartTypes.StepLine;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 13)
                    {
                        this.olapChart.ChartType = ChartTypes.Scatter;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 14)
                    {
                        this.olapChart.ChartType = ChartTypes.Pie;
                        this.olapChart.ColorEachSeries = this.ColorEachPieSeries;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 15)
                    {
                        this.olapChart.ChartType = ChartTypes.Radar;
                    }
                    if (this.cmb_ChartType.SelectedIndex == 16)
                    {
                        this.olapChart.ChartType = ChartTypes.Funnel;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnExportChartToWord control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExportChartToWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.OlapDataManager != null && this.olapChart != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "OlapChart Report";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.DefaultExt = "doc";
                    saveFileDialog.Filter = "Word (.doc)|*.doc";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        OlapChartWordExport olapChartWordExport = new OlapChartWordExport(this.olapChart);
                        olapChartWordExport.ExportintoNewDoc(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnExportChartToPdf control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExportChartToPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.OlapDataManager != null && this.olapChart != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "OlapChart Report";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.DefaultExt = "pdf";
                    saveFileDialog.Filter = "Pdf (.pdf)|*.pdf";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        OlapChartPdfExport chartPdfExport = new OlapChartPdfExport(this.olapChart);
                        chartPdfExport.ExportIntoNewPdf(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }

        /// <summary>
        /// Handles the Click event of the ShowAppearanceDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowAppearanceDialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                this.olapChart.ShowAppearanceDialog();
                if (this.olapChart.ChartAppearance.LegendVisibility == true)
                {
                    btnShowLegend.IsChecked = true;
                    if (this.olapChart.Legend != null)
                    {
                        this.olapChart.Legend.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    btnShowLegend.IsChecked = false;
                    if (this.olapChart.Legend != null)
                    {
                        this.olapChart.Legend.Visibility = Visibility.Collapsed;
                    }
                }

                for (int i = 0; i < cmb_ChartType.Items.Count; i++)
                {
                    if (((ImageData)cmb_ChartType.Items[i]).Text == this.olapChart.ChartAppearance.ChartType.ToString())
                    {
                        this.cmb_ChartType.SelectedIndex = i;
                        this.OlapDataManager.CurrentReport.ChartSettings.ChartType = this.olapChart.ChartAppearance.ChartType.ToString();
                        break;
                    }
                }

                for (int i = 0; i < cmb_ChartPalette.Items.Count; i++)
                {
                    if (((ImageData)cmb_ChartPalette.Items[i]).Text == this.olapChart.ChartAppearance.ChartColorPalette.ToString())
                    {
                        this.cmb_ChartPalette.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OlapClient");
            }
        }
        #endregion

        #region Grid ToolBar Events
        /// <summary>
        /// Handles the Click event of the btnShowHeaderCellsToolTip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnShowHeaderCellsToolTip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (this.ShowOlapGrid)
                //{
                this.olapGrid.ShowHeaderCellsToolTip = (bool)this.btnShowHeaderCellsToolTip.IsChecked;
                this.olapGrid.DataBind();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnShowValueCellsToolTip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnShowValueCellsToolTip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (this.ShowOlapGrid)
                //{
                this.olapGrid.ShowValueCellToolTip = (bool)this.btnShowValueCellsToolTip.IsChecked;
                this.olapGrid.DataBind();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnfrozenHeaders control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnfrozenHeaders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.olapGrid.FreezeHeaders = btnfrozenHeaders.IsChecked == true ? true : false;
                this.olapGrid.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnGridExport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExportExcelGrid_Click(object sender, RoutedEventArgs e)
        {
            this.ExportToExcel();
        }

        /// <summary>
        /// Handles the Click event of the btnExportWord control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExportWord_Click(object sender, RoutedEventArgs e)
        {
            this.ExportToWord();
        }

        /// <summary>
        /// Handles the Click event of the btnExportPdf control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            this.ExportToPdf();
        }

        /// <summary>
        /// Handles the Click event of the ShowGridStyleDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowGridStyleDialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (this.ShowOlapGrid)
                this.olapGrid.ShowStyleDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmb_GridLayoutType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmb_GridLayoutType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.olapGrid != null)
            {
                if (e.AddedItems.Count > 0)
                {
                    ImageData imageData = (ImageData)e.AddedItems[0];
                    if (this.cmb_GridLayoutType.SelectedIndex == 0)
                    {
                        this.olapGrid.Layout = Syncfusion.Olap.Engine.GridLayout.Normal;
                        this.olapGrid.OlapDataManager.NotifyElementModified();
                    }
                    if (this.cmb_GridLayoutType.SelectedIndex == 1)
                    {
                        this.olapGrid.Layout = Syncfusion.Olap.Engine.GridLayout.ExcelLikeLayout;
                        this.olapGrid.OlapDataManager.NotifyElementModified();
                    }
                    if (this.cmb_GridLayoutType.SelectedIndex == 2)
                    {
                        this.olapGrid.Layout = Syncfusion.Olap.Engine.GridLayout.NormalTopSummary;
                        this.olapGrid.OlapDataManager.NotifyElementModified();
                    }
                    if (this.cmb_GridLayoutType.SelectedIndex == 3)
                    {
                        this.olapGrid.Layout = Syncfusion.Olap.Engine.GridLayout.NoSummaries;
                        this.olapGrid.OlapDataManager.NotifyElementModified();
                    }
                }
            }
        }
        #endregion

        #region Common ToolBar Events
        /// <summary>
        /// Handles the Click event of the btnSortingColumn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSortingColumn_Click(object sender, RoutedEventArgs e)
        {
            this.ShowColumSortingDialog();
        }

        /// <summary>
        /// Handles the Click event of the btnSortingRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSortingRow_Click(object sender, RoutedEventArgs e)
        {
            this.ShowRowSortingDialog();
        }

        /// <summary>
        /// Handles the Click event of the btnTogglePivot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnTogglePivot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OlapDataManager.CurrentReport.TogglePivot = !this.OlapDataManager.CurrentReport.TogglePivot;
                this.OlapDataManager.NotifyElementModified();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnShowExpanders control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnShowExpanders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OlapDataManager.CurrentReport.ShowExpanders = !this.OlapDataManager.CurrentReport.ShowExpanders;
                //if (this.ShowOlapChart != false)
                this.olapChart.DataBind();
                //if (this.ShowOlapGrid != false)
                this.olapGrid.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Olap Client");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnConnectOption control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnConnectOption_Click(object sender, RoutedEventArgs e)
        {
            this.ShowConnectOption();
        }

        /// <summary>
        /// Handles the Click event of the btnFilterColumn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnFilterColumn_Click(object sender, RoutedEventArgs e)
        {
            this.ShowColumnFilterDialog();
        }

        /// <summary>
        /// Handles the Click event of the btnFilterRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnFilterRow_Click(object sender, RoutedEventArgs e)
        {
            this.ShowRowFilterDialog();
        }


        /// <summary>
        /// Handles the ValueChanged event of the subsetFilterRows control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Olap.ValueChangedEventArgs"/> instance containing the event data.</param>
        private void subsetFilterRows_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            GetSubSetElement(e.NewValue, this.OlapDataManager.CurrentReport.SeriesElements);
        }

        /// <summary>
        /// Handles the ValueChanged event of the subsetFilterCategorical control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Olap.ValueChangedEventArgs"/> instance containing the event data.</param>
        private void subsetFilterCategorical_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            GetSubSetElement(e.NewValue, this.OlapDataManager.CurrentReport.CategoricalElements);
        }
        #endregion

        #region Control Visibility Events
        /// <summary>
        /// Called when [tool bar visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnToolBarVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            if (olapClient.ShowToolBar)
            {
                olapClient.toolBarRow.Height = new GridLength(40);
                olapClient.olapClientToolbar.Visibility = Visibility.Visible;
            }
            else
            {
                olapClient.toolBarRow.Height = new GridLength(0);
                olapClient.olapClientToolbar.Visibility = Visibility.Collapsed;
            }
        }

        private double columnWider;
        /// <summary>
        /// Gets or sets the width between CubeDimensionBrowser and ChartArea
        /// </summary>
        public double ColumnWider
        {
            get { return columnWider; }
            set
            {
                columnWider = value;
                this.Wider.Width = new GridLength(value);
            }
        }
        /// <summary>
        /// Called when [cube selector visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnCubeSelectorVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;

            if (olapClient.ShowCubeSelector == true)
            {
                olapClient.cubeSelectorRow.Height = new GridLength(40);
            }
            else
            {
                olapClient.cubeSelectorRow.Height = new GridLength(0);
            }
        }


        /// <summary>
        /// Called when [connect option button visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnConnectOptionButtonVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            if (olapClient.ShowConnectOptionButton)
            {
                olapClient.btnConnectOption.Visibility = Visibility.Visible;
                olapClient.sptrConnectReport.Visibility = Visibility.Visible;
            }
            else
            {
                olapClient.btnConnectOption.Visibility = Visibility.Collapsed;
                olapClient.sptrConnectReport.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when [report buttons visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnReportButtonsVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            if (olapClient.ShowReportButtons)
            {
                olapClient.btnNewReport.Visibility = Visibility.Visible;
                olapClient.btnLoadReport.Visibility = Visibility.Visible;
                olapClient.btnSaveReport.Visibility = Visibility.Visible;
                olapClient.btnSaveAsReport.Visibility = Visibility.Visible;
                olapClient.btnAddReport.Visibility = Visibility.Visible;
                olapClient.btnRemoveReport.Visibility = Visibility.Visible;
                olapClient.btnRenameReport.Visibility = Visibility.Visible;
                olapClient.reportList.Visibility = Visibility.Visible;
                olapClient.sptrReportCommon.Visibility = Visibility.Visible;
                olapClient.btnShowMdx.Visibility = Visibility.Visible;
            }
            else
            {
                olapClient.btnNewReport.Visibility = Visibility.Collapsed;
                olapClient.btnLoadReport.Visibility = Visibility.Collapsed;
                olapClient.btnSaveReport.Visibility = Visibility.Collapsed;
                olapClient.btnSaveAsReport.Visibility = Visibility.Collapsed;
                olapClient.btnAddReport.Visibility = Visibility.Collapsed;
                olapClient.btnRemoveReport.Visibility = Visibility.Collapsed;
                olapClient.btnRenameReport.Visibility = Visibility.Collapsed;
                olapClient.reportList.Visibility = Visibility.Collapsed;
                olapClient.sptrReportCommon.Visibility = Visibility.Collapsed;
                olapClient.btnShowMdx.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when [filter sort buttons visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFilterSortButtonsVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            if (olapClient.ShowFilterSortButtons)
            {
                olapClient.btnTogglePivot.Visibility = Visibility.Visible;
                olapClient.btnFilterColumn.Visibility = Visibility.Visible;
                olapClient.btnFilterRow.Visibility = Visibility.Visible;
                olapClient.btnSortingColumn.Visibility = Visibility.Visible;
                olapClient.btnSortingRow.Visibility = Visibility.Visible;
            }
            else
            {
                olapClient.btnTogglePivot.Visibility = Visibility.Collapsed;
                olapClient.btnFilterColumn.Visibility = Visibility.Collapsed;
                olapClient.btnFilterRow.Visibility = Visibility.Collapsed;
                olapClient.btnSortingColumn.Visibility = Visibility.Collapsed;
                olapClient.btnSortingRow.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when [subset filters visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnSubsetFiltersVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            if (olapClient.ShowSubsetFilters)
            {
                olapClient.subsetFilterCategorical.Visibility = Visibility.Visible;
                olapClient.subsetFilterRows.Visibility = Visibility.Visible;
            }
            else
            {
                olapClient.subsetFilterCategorical.Visibility = Visibility.Collapsed;
                olapClient.subsetFilterRows.Visibility = Visibility.Collapsed;
            }
        }

        void olapTabControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.OlapGrid.EnableColumnHeaderContextMenu || this.OlapGrid.EnableRowHeaderContextMenu)
            {
                FieldInfo fi = typeof (ContextMenuEventArgs).GetField("_targetElement",BindingFlags.NonPublic | BindingFlags.Instance);
                if (e.Source is Grid.Olap.OlapGrid && fi != null && fi.GetValue(e) != null &&
                    fi.GetValue(e).ToString().Equals("System.Windows.Controls.Grid"))
                {
                    return;
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Called when [execute button visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnExecuteButtonVisibilityChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            if (olapClient.ShowExecuteButton)
            {
                olapClient.sptrExecute.Visibility = Visibility.Visible;
                olapClient.btnExecute.Visibility = Visibility.Visible;
                olapClient.AutoExecute = false;
            }
            else
            {
                olapClient.sptrExecute.Visibility = Visibility.Collapsed;
                olapClient.btnExecute.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when [execute button visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnAutoExecuteChanged(DependencyObject dependencyobj, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyobj as OlapClient;
            olapClient.axisElementBuiderCategorical.AutoExecute = olapClient.AutoExecute;
            olapClient.axisElementBuiderSeries.AutoExecute = olapClient.AutoExecute;
            olapClient.axisElementBuiderSlicer.AutoExecute = olapClient.AutoExecute;
            if (olapClient.AutoExecute)
            {
                olapClient.sptrExecute.Visibility = Visibility.Collapsed;
                olapClient.btnExecute.Visibility = Visibility.Collapsed;
            }
            else
            {
                olapClient.sptrExecute.Visibility = Visibility.Visible;
                olapClient.btnExecute.Visibility = Visibility.Visible;
            }
        }
        private static void OnVirtualKpiEnabled(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && !DesignerProperties.GetIsInDesignMode(client))
            {
                if ((bool)args.NewValue)
                    client.btn_VirtualKpi.Visibility = Visibility.Visible;
                else
                {
                    client.btn_VirtualKpi.Visibility = Visibility.Collapsed;
                    if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                    {
                        client.OlapDataManager.CurrentReport.VirtualKpiElements.Clear();
                        client.cubeDimensionBrowser.OlapDataManager = null;
                        client.cubeDimensionBrowser.OlapDataManager = client.OlapDataManager;

                        //// Refresh the Axis Element build items if any.
                        var virtualKpis = client.axisElementBuiderCategorical.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember).ToList();
                        if (virtualKpis.Count > 0)
                        {
                            foreach (var item in virtualKpis)
                            {
                                client.axisElementBuiderCategorical.MetaTreeNodes.Remove(item);
                            }
                            client.axisElementBuiderCategorical.RefreshOlapDataManagerElementItems(client.axisElementBuiderCategorical, client.AutoExecute);
                        }

                        virtualKpis = client.axisElementBuiderSeries.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember).ToList();
                        if (virtualKpis.Count > 0)
                        {
                            foreach (var item in virtualKpis)
                            {
                                client.axisElementBuiderSeries.MetaTreeNodes.Remove(item);
                            }
                            client.axisElementBuiderSeries.RefreshOlapDataManagerElementItems(client.axisElementBuiderSeries, client.AutoExecute);
                        }

                        virtualKpis = client.axisElementBuiderSlicer.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember).ToList();
                        if (virtualKpis.Count > 0)
                        {
                            foreach (var item in virtualKpis)
                            {
                                client.axisElementBuiderSlicer.MetaTreeNodes.Remove(item);
                            }
                            client.axisElementBuiderSlicer.RefreshOlapDataManagerElementItems(client.axisElementBuiderSlicer, client.AutoExecute);
                        }

                    }
                }
            }
        }

        private static void OnCalculatedMembersEnabled(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && !DesignerProperties.GetIsInDesignMode(client))
            {
                if ((bool)args.NewValue)
                {
                    client.btnCreateCalcMemebr.Visibility = Visibility.Visible;
                }
                else
                {
                    client.btnCreateCalcMemebr.Visibility = Visibility.Collapsed;
                    if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                    {
                        client.OlapDataManager.CurrentReport.CalculatedMembers.Clear();
                        client.cubeDimensionBrowser.OlapDataManager = null;
                        client.cubeDimensionBrowser.OlapDataManager = client.OlapDataManager;

                        //// Refresh the Axis Element build items if any.
                        var calcMembers = client.axisElementBuiderCategorical.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember).ToList();
                        if (calcMembers.Count > 0)
                        {
                            foreach (var item in calcMembers)
                            {
                                client.axisElementBuiderCategorical.MetaTreeNodes.Remove(item);
                            }
                            client.axisElementBuiderCategorical.RefreshOlapDataManagerElementItems(client.axisElementBuiderCategorical, client.AutoExecute);
                        }

                        calcMembers = client.axisElementBuiderSeries.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember).ToList();
                        if (calcMembers.Count > 0)
                        {
                            foreach (var item in calcMembers)
                            {
                                client.axisElementBuiderSeries.MetaTreeNodes.Remove(item);
                            }
                            client.axisElementBuiderSeries.RefreshOlapDataManagerElementItems(client.axisElementBuiderSeries, client.AutoExecute);
                        }

                        calcMembers = client.axisElementBuiderSlicer.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember).ToList();
                        if (calcMembers.Count > 0)
                        {
                            foreach (var item in calcMembers)
                            {
                                client.axisElementBuiderSlicer.MetaTreeNodes.Remove(item);
                            }
                            client.axisElementBuiderSlicer.RefreshOlapDataManagerElementItems(client.axisElementBuiderSlicer, client.AutoExecute);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Called when [VisualStyle changed]
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        public static void OnVisualStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = dependencyObject as OlapClient;
            ResourceDictionary resource = new ResourceDictionary();
            if (olapClient != null)
            {
                SkinStorage.SetVisualStyle(olapClient, e.NewValue.ToString());
                if (olapClient.OlapChart != null)
                    olapClient.OlapChart.VisualStyle = (OlapChartVisualStyle)Enum.Parse(typeof(OlapChartVisualStyle), e.NewValue.ToString());
            }
        }
        #endregion

        #region PopupButton Event
        /// <summary>
        /// Handles the Click event of the popupButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void popupButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 30;
            da.To = 100;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.AutoReverse = true;

            //popupButton.BeginAnimation(Button.HeightProperty, da);
        }
        #endregion

        #region Overridden Methods

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name.Equals("VisualStyle"))
            {
                ResourceDictionary rs = new ResourceDictionary();
                rs.Source = new Uri("/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                switch (e.NewValue.ToString())
                {
                    case "Default":
                        //if (this.CubeDimensionBrowserStyle == null)
                        //{
                        //    this.cubeDimensionBrowser.Style = rs["DefaultCubeDimensionBrowser"] as Style;
                        //}
                        //else
                        //{
                        //    this.cubeDimensionBrowser.Style = this.CubeDimensionBrowserStyle;
                        //}

                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["DefaultReportList"] as Style;
                        this.cubeSelector.Style = rs["DefaultCubeSelector"] as Style;
                        break;
                    case "Office2003":
                        //this.cubeDimensionBrowser.Style = rs["Office2003CubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2003ReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2003CubeSelector"] as Style;
                        break;
                    case "Blend":
                        //this.cubeDimensionBrowser.Style = rs["BlendCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["BlendReportList"] as Style;
                        this.cubeSelector.Style = rs["BlendCubeSelector"] as Style;
                        break;
                    case "Office2007Blue":
                        //this.cubeDimensionBrowser.Style = rs["Office2007BlueCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2007BlueReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2007BlueCubeSelector"] as Style;
                        break;
                    case "Office2007Black":
                        //this.cubeDimensionBrowser.Style = rs["Office2007BlackCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2007BlackReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2007BlackCubeSelector"] as Style;
                        break;
                    case "Office2007Silver":
                        //this.cubeDimensionBrowser.Style = rs["Office2007SilverCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2007SilverReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2007SilverCubeSelector"] as Style;
                        break;

                    case "Office2010Blue":
                        //this.cubeDimensionBrowser.Style = rs["Office2010BlueCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2010BlueReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2010BlueCubeSelector"] as Style;
                        break;
                    case "Office2010Black":
                        //this.cubeDimensionBrowser.Style = rs["Office2010BlackCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2010BlackReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2010BlackCubeSelector"] as Style;
                        break;
                    case "Office2010Silver":
                        //this.cubeDimensionBrowser.Style = rs["Office2010SilverCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["Office2010SilverReportList"] as Style;
                        this.cubeSelector.Style = rs["Office2010SilverCubeSelector"] as Style;
                        break;

                    case "Metro":
                        //this.cubeDimensionBrowser.Style = rs["MetroCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["MetroReportList"] as Style;
                        this.cubeSelector.Style = rs["MetroCubeSelector"] as Style;
                        break;

                    case "Transparent":
                        //this.cubeDimensionBrowser.Style = rs["TransparentCubeDimensionBrowser"] as Style;
                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["TransparentReportList"] as Style;
                        this.cubeSelector.Style = rs["TransparentCubeSelector"] as Style;
                        break;

                    default:
                        //if (this.CubeDimensionBrowserStyle == null)
                        //{
                        //    this.cubeDimensionBrowser.Style = rs["DefaultCubeDimensionBrowser"] as Style;
                        //}
                        //else
                        //{
                        //    this.cubeDimensionBrowser.Style = this.CubeDimensionBrowserStyle;
                        //}

                        treeviewstyle = this.cubeDimensionBrowser.Style;
                        this.reportList.Style = rs["DefaultReportList"] as Style;
                        this.cubeSelector.Style = rs["DefaultCubeSelector"] as Style;
                        break;
                }
                SetBackground(this.HeaderBackground, this.HeaderBorderBrush);
            }
        }

        #endregion

        #region AxisElementBuilder Events

        private void axisElementBuiderSeries_DragOver(object sender, DragEventArgs e)
        {
            IsDragOver = true;
            string visualstyle = SkinStorage.GetVisualStyle(this).ToString();
            AxisElementBuilder builder = (AxisElementBuilder)sender;
            ChangeVisibilty(Visibility.Visible);
            //SolidColorBrush brush = new SolidColorBrush(new Color() { A = 100, R = 127, G = 127, B = 127 });          
            switch (builder.Axis)
            {
                case AxisPosition.Categorical:
                    CategoricalBorder.Background = this.HeaderMouseOverBackground;
                    CategoricalBorder.BorderBrush = this.HeaderMouseOverBorderBrush;
                    //CategoricalOuterBorder.BorderThickness = new Thickness(1);
                    //axisElementBuiderCategorical.BorderThickness = new Thickness(2);
                    break;
                case AxisPosition.Series:
                    SeriesBorder.Background = this.HeaderMouseOverBackground;
                    SeriesBorder.BorderBrush = this.HeaderMouseOverBorderBrush;
                    //SeriesOuterBorder.BorderThickness = new Thickness(1);
                    //axisElementBuiderSeries.BorderThickness = new Thickness(2);
                    break;
                case AxisPosition.Slicer:
                    SlicerBorder.Background = this.HeaderMouseOverBackground;
                    SlicerBorder.BorderBrush = this.HeaderMouseOverBorderBrush;
                    //SlicerOuterBorder.BorderThickness = new Thickness(1);
                    //axisElementBuiderSlicer.BorderThickness = new Thickness(2);
                    break;
            }
        }

        private void axisElementBuiderSeries_Drop(object sender, DragEventArgs e)
        {
            IsDragOver = false;
            ChangeVisibilty(Visibility.Collapsed);
            SetBackground(this.HeaderBackground, this.HeaderBorderBrush);
        }

        void ChangeVisibilty(Visibility value)
        {
            tabborder.Visibility = value;
            toolbarborder.Visibility = value;
            leftpanelborder.Visibility = value;
            splitterborder.Visibility = value;
            hzsplitterborder.Visibility = value;
            //filterborder.Visibility = value;
            //filterborder1.Visibility = value;
            //border.Visibility = value;
        }

        #endregion

        private void cubeDimensionBrowser_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TreeView treeview = (TreeView)e.Source;
                //Point position = e.GetPosition(null);
                //if (Math.Abs(position.X - startingposition.X) > SystemParameters.MinimumHorizontalDragDistance && Math.Abs(position.Y - startingposition.Y) > SystemParameters.MinimumHorizontalDragDistance)
                //{
                if (!(e.OriginalSource is System.Windows.Controls.Primitives.Thumb))
                {
                    if (treeview.SelectedItem is MetaTreeNode)
                        ChangeVisibilty(Visibility.Visible);
                }
                //}
            }
        }

        private void leftpanelborder_Drop(object sender, DragEventArgs e)
        {
            ChangeVisibilty(Visibility.Collapsed);
        }

        private void leftpanelborder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeVisibilty(Visibility.Collapsed);
        }

        private void axisElementBuiderCategorical_DragLeave(object sender, DragEventArgs e)
        {
            IsDragOver = false;
            SetBackground(this.HeaderBackground, this.HeaderBorderBrush);
        }

        void SetBackground(Brush background, Brush borderbrush)
        {
            CategoricalBorder.Background = background;
            CategoricalBorder.BorderBrush = borderbrush;
            SeriesBorder.Background = background;
            SeriesBorder.BorderBrush = borderbrush;
            SlicerBorder.Background = background;
            SlicerBorder.BorderBrush = borderbrush;
        }

        private void btnShowMdx_Click(object sender, RoutedEventArgs e)
        {
            MdxDialog mdxDlg = new MdxDialog(this.OlapDataManager.GetMDXQuery());
            SkinStorage.SetVisualStyle(mdxDlg, SkinStorage.GetVisualStyle(this));
            mdxDlg.FlowDirection = this.FlowDirection;
            mdxDlg.ShowDialog();
        }

        private void btnCreateCalcMemebr_Click(object sender, RoutedEventArgs e)
        {
            if (this.OlapDataManager != null && !String.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName))
            {
                CalcMemberEditor calcMemberEditor = new CalcMemberEditor(this.OlapDataManager);
                calcMemberEditor.FlowDirection = this.FlowDirection;
                SkinStorage.SetVisualStyle(calcMemberEditor, SkinStorage.GetVisualStyle(this));
                calcMemberEditor.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                calcMemberEditor.AutoExecute = this.AutoExecute;
                calcMemberEditor.ShowDialog();
            }
        }

        private void btn_VirtualKpi_Click(object sender, RoutedEventArgs e)
        {
            if(this.OlapDataManager !=null && !String.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName))
            {
                KPIEditor virtualKpiEditor = new KPIEditor(this.OlapDataManager);
                virtualKpiEditor.FlowDirection = this.FlowDirection;
                SkinStorage.SetVisualStyle(virtualKpiEditor, SkinStorage.GetVisualStyle(this));
                virtualKpiEditor.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                virtualKpiEditor.ShowDialog();
            }
        }

    }

    /// <summary>
    /// Represents commands that can be invoked on <see cref="OlapClient"/>.
    /// </summary>
    /// <remarks>
    /// Commanding is an input mechanism in Windows Presentation Foundation 
    /// which provides input handling at a more semantic level than device input.
    /// </remarks>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public static class OlapClientCommands
    {
        /// <summary>
        /// Initializes c_newReport RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_newReport = new RoutedUICommand("NewReport", "NewReport", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_loadReport RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_loadReport = new RoutedUICommand("LoadReport", "LoadReport", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_loadReportStream RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_loadReportStream = new RoutedUICommand("LoadReportStream", "LoadReportStream", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_saveReport RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_saveReport = new RoutedUICommand("SaveReport", "SaveReport", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_addReport RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_addReport = new RoutedUICommand("AddReport", "AddReport", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_removeReport RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_removeReport = new RoutedUICommand("RemoveReport", "RemoveReport", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_renameReport RoutedUICommand
        /// </summary>
        private readonly static RoutedUICommand c_renameReport = new RoutedUICommand("RenameReport", "RenameReport", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_showMdxDialog RoutedUICommadn
        /// </summary>
        private readonly static RoutedUICommand c_showConnectOption = new RoutedUICommand("ShowConnectOption", "ShowConnectOption", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_showMdxDialog RoutedUICommadn
        /// </summary>
        private readonly static RoutedUICommand c_showMdxDialog = new RoutedUICommand("ShowMdxDialog", "ShowMdxDialog", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_showColumnFilterDialog RoutedUICommadn
        /// </summary>
        private readonly static RoutedUICommand c_showColumnFilterDialog = new RoutedUICommand("ShowColumnFilterDialog", "ShowColumnFilterDialog", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_showRowFilterDialog RoutedUICommadn
        /// </summary>
        private readonly static RoutedUICommand c_showRowFilterDialog = new RoutedUICommand("ShowRowFilterDialog", "ShowRowFilterDialog", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_showColumSortingDialog RoutedUICommadn
        /// </summary>
        private readonly static RoutedUICommand c_showColumnSortingDialog = new RoutedUICommand("ShowColumnSortingDialog", "ShowColumnSortingDialog", typeof(OlapClientCommands));

        /// <summary>
        /// Initializes c_showRowSortingDialog RoutedUICommadn
        /// </summary>
        private readonly static RoutedUICommand c_showRowSortingDialog = new RoutedUICommand("ShowRowSortingDialog", "ShowRowSortingDialog", typeof(OlapClientCommands));

        #region Properties

        /// <summary>
        /// Gets the NewReport
        /// </summary>
        public static RoutedUICommand NewReport
        {
            get { return c_newReport; }
        }

        /// <summary>
        /// Gets the LoadReport
        /// </summary>
        public static RoutedUICommand LoadReport
        {
            get { return c_loadReport; }
        }

        /// <summary>
        /// Gets the load report stream.
        /// </summary>
        /// <value>The load report stream.</value>
        public static RoutedUICommand LoadReportStream
        {
            get { return c_loadReportStream; }
        }

        /// <summary>
        /// Gets the SaveReport
        /// </summary>
        public static RoutedUICommand SaveReport
        {
            get { return c_saveReport; }
        }

        /// <summary>
        /// Gets the AddReport
        /// </summary>
        public static RoutedUICommand AddReport
        {
            get { return c_addReport; }
        }

        /// <summary>
        /// Gets the RemoveReport
        /// </summary>
        public static RoutedUICommand RemoveReport
        {
            get { return c_removeReport; }
        }

        /// <summary>
        /// Gets the RenameReport
        /// </summary>
        public static RoutedUICommand RenameReport
        {
            get { return c_renameReport; }
        }

        /// <summary>
        /// Gets the show MDX dialog.
        /// </summary>
        /// <value>The show MDX dialog.</value>
        public static RoutedUICommand ShowMdxDialog
        {
            get { return c_showMdxDialog; }
        }

        /// <summary>
        /// Gets the show column filter dialog.
        /// </summary>
        /// <value>The show column filter dialog.</value>
        public static RoutedUICommand ShowColumnFilterDialog
        {
            get { return c_showColumnFilterDialog; }
        }

        /// <summary>
        /// Gets the show row filter dialog.
        /// </summary>
        /// <value>The show row filter dialog.</value>
        public static RoutedUICommand ShowRowFilterDialog
        {
            get { return c_showRowFilterDialog; }
        }

        /// <summary>
        /// Gets the show colum sorting dialog.
        /// </summary>
        /// <value>The show colum sorting dialog.</value>
        public static RoutedUICommand ShowColumnSortingDialog
        {
            get { return c_showColumnSortingDialog; }
        }

        /// <summary>
        /// Gets the show row sorting dialog.
        /// </summary>
        /// <value>The show row sorting dialog.</value>
        public static RoutedUICommand ShowRowSortingDialog
        {
            get { return c_showRowSortingDialog; }
        }

        /// <summary>
        /// Gets the show connect option.
        /// </summary>
        /// <value>The show connect option.</value>
        public static RoutedUICommand ShowConnectOption
        {
            get { return c_showConnectOption; }
        }
        #endregion
    }
}
