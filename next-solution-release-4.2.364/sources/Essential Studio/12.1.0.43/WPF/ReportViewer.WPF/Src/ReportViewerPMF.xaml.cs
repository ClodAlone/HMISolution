//-------------------------------------------------------------------------------------------------
// <copyright file="ReportViewerPMF.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports.Viewer.Utils;
using Syncfusion.Windows.Reports.Viewer.Dialogs;
using System.Net;
using System.Text.RegularExpressions;
using DOM = Syncfusion.RDL.DOM;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Controls;
using Syncfusion.RDL.ItemModel;
using System.Windows.Controls.Primitives;

#if SILVERLIGHT
using RESX = Syncfusion.Windows.Report.Viewer.Properties.Resources;
using System.Windows.Printing;
using Syncfusion.Reports.Server;
using Syncfusion.Windows.Tools.Controls;
using System.Reflection;
using System.Windows.Resources;
using Syncfusion.Windows.Controls;
using Syncfusion.ReportWriter;
#else
using RESX = Syncfusion.ReportViewer.WPF.Properties.Resources;
using System.IO.Packaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Syncfusion.ReportWriter;
using System.Windows.Markup;
using System.Printing;
#endif

namespace Syncfusion.Windows.Reports.Viewer
{
    /// <summary>
    /// Represents the Report Viewer control.
    /// </summary>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif

    public partial class ReportViewer
        : Control
    {
        #region Members


        Button buttonPrint;
        Button buttonFirst;
        Button buttonLast;
        Button buttonPrevious;
        Button buttonNext;
        Button buttonParameters;
        Button buttonPageSetup;
        Button buttonRefresh;
        Button btnViewReport;
        Button buttonFind;
        Button btnViewReport1;
        Button btnback;
        Button buttonShowOrHideDocumentMap;

        ToggleButton buttonPrintLayout;
        ToggleButton toggleShowDetails;

        ComboBox comboBoxPageZoom;
        ComboBox exportControl;

        System.Windows.Controls.Grid MainGrid;
        System.Windows.Controls.Grid gridException;
        System.Windows.Controls.Grid renderArea;
        System.Windows.Controls.Grid treeItemArea;
        System.Windows.Controls.Grid ExceptionGrid;
        System.Windows.Controls.Grid gridRenderingRegion;
        System.Windows.Controls.Grid gridLoadingIndicator;
        System.Windows.Controls.Grid grid_ReportParameterBlock;
        GridSplitter viewerSpliter;

        ScaleTransform Zoom;
        WrapPanel ShowError;
        StackPanel PageView;
        StackPanel sPanel_Head;
        StackPanel toolBar;
        StackPanel PageViewContainer;

        Border PageViewBody;
        Border PageFooterBorder;
        Border PageBodyBorder;
        Border PageHeaderBorder;

        Canvas CanvasFooter;
        Canvas CanvasHeader;
        Canvas canvasContentPage;

        TextBox textBoxCurrentPage;
        TextBox textBlockStackTrace;
        TextBox textBoxFind;

        TextBlock labelOf;
        TextBlock textBlockException;
        internal TextBlock textBoxTotalPages;
        TreeView DocumentMap;

        ScrollViewer scrollViewer;
        ScrollViewer scrollViewerParamBlock;
        ScrollViewer scorllDSCredentialBlock;
        ScrollViewer groupBoxExpandedExceptionScroll;

        RowDefinition gridExceptionRow;
        RowDefinition loadingIndicatorRow;
        RowDefinition toolBarGridRow;
        RowDefinition dsCredentialRow;
        RowDefinition parameterGridRow;
        RowDefinition viewerContentRow;
#if SILVERLIGHT
        System.Windows.Controls.Grid groupBoxExpandedException;
        Canvas zoomCanvas;
        HyperlinkButton hyperlink;
#else
        Hyperlink hyperlink;
        GroupBox groupBoxExpandedException;
#endif

        private int m_current = 0;
        private int m_totalPages = 0;

        private double zoomFactor = 1;

        private int credentialDSCount;

        private string stackTrace;
        private string errorMessage;

        private bool? rendered = null;
        private bool hasPageFooter;
        private bool hasPageHeader;
        private bool isExporting;
        private bool textBoxValueChanged;
        private bool internalValueChange;
        private bool isLoaded = false;

        private double printDpiX = 96d;

        private double printDpiY = 96d;

#if !SILVERLIGHT
        private bool IsScroll;
#endif

        private PageSetupDialog pageSetup = null;

#if SILVERLIGHT
        private Stream exportFileStream;
        private bool isDrillReport = false;
#endif

        private ResourceFinder resourceFinder = new ResourceFinder();
        private ReportDataSourceCollection _DataSources;
        private PageModelFactory pageModelFactory = null;
        private List<ExportOption> exportOptions;
        private List<CallBackActions> actionStore = new List<CallBackActions>();

        private ExportOption pdfExport;
        private ExportOption excelExport;
        private ExportOption wordExport;
        private ExportOption htmlExport;
        private BackgroundWorker currentWorker;

#if !SILVERLIGHT
        private ExportOption xpsExport;
#endif
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.HasReportParameters"/> dependency property. 
        /// </summary>
        static readonly DependencyProperty HasReportParametersProperty =
            DependencyProperty.Register("HasReportParameters", typeof(bool), typeof(ReportViewer), new PropertyMetadata(false, OnHasReportParametersPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowToolBarProperty"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowToolBarProperty =
            DependencyProperty.Register("ShowToolBar", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, OnShowToolBarChanged));
        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowPageLayoutControl"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowPageLayoutControlProperty =
            DependencyProperty.Register("ShowPageLayoutControl", typeof(bool?), typeof(ReportViewer), new PropertyMetadata(null, OnShowPageLayoutControlPropertyChanged));
        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowPageNavigationControlsProperty"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowPageNavigationControlsProperty =
            DependencyProperty.Register("ShowPageNaviagationControls", typeof(bool?), typeof(ReportViewer), new PropertyMetadata(true, OnShowPageNavigationControlsPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowRefreshButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowRefreshButtonProperty =
            DependencyProperty.Register("ShowRefreshButton", typeof(bool?), typeof(ReportViewer), new PropertyMetadata(null, OnShowRefreshButtonPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowPrintButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowPrintButtonProperty =
            DependencyProperty.Register("ShowPrintButton", typeof(bool?), typeof(ReportViewer), new PropertyMetadata(null, OnShowPrintButtonPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowParameterButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowParametereButtonProperty =
            DependencyProperty.Register("ShowParameterButton", typeof(bool?), typeof(ReportViewer), new PropertyMetadata(true, OnShowParameterButtonPropertyChanged));


        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowZoomControl"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowZoomControlProperty =
            DependencyProperty.Register("ShowZoomControl", typeof(bool?), typeof(ReportViewer), new PropertyMetadata(null, OnShowZoomControlPropertyChanged));
        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowParametersBlock"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowParametersBlockProperty =
            DependencyProperty.Register("ShowParametersBlock", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, OnShowParametersBlockChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.CurrentPage"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(ReportViewer), new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ViewMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModeProperty =
        DependencyProperty.Register("ViewMode", typeof(ViewMode), typeof(ReportViewer), new PropertyMetadata(ViewMode.Normal, ViewModePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ViewMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
        DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ReportViewer), new PropertyMetadata(100.0, ZoomFactorPropertyChanged));


        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ProcessingMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessingModeProperty =
        DependencyProperty.Register("ProcessingMode", typeof(ProcessingMode), typeof(ReportViewer), new PropertyMetadata(ProcessingMode.Remote));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.PaperOrientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaperOrientationProperty =
        DependencyProperty.Register("PaperOrientation", typeof(PaperOrientation), typeof(ReportViewer), new PropertyMetadata(PaperOrientation.Portrait, PaperOrientationPropertyChanged));


        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowPdfExportButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowPdfExportButtonProperty =
            DependencyProperty.Register("ShowPdfExportButton", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, ShowPdfExportButtonPropertyChanged));

        public static readonly DependencyProperty ShowExcelExportButtonProperty =
            DependencyProperty.Register("ShowExcelExportButton", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, ShowExcelExportButtonPropertyChanged));

        public static readonly DependencyProperty ShowWordExportButtonProperty =
            DependencyProperty.Register("ShowWordExportButton", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, ShowWordExportButtonPropertyChanged));

        public static readonly DependencyProperty ShowHtmlExportButtonProperty =
            DependencyProperty.Register("ShowHtmlExportButton", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, ShowHtmlExportButtonPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowXpsExportButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowExportControlsProperty =
            DependencyProperty.Register("ShowExportControls", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, ShowExportControlsPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowXpsExportButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EnableVirtualEvaluationProperty =
            DependencyProperty.Register("EnableVirtualEvaluation", typeof(bool), typeof(ReportViewer), new PropertyMetadata(false, null));

#if SILVERLIGHT
        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportPathProperty =
        DependencyProperty.Register("ReportPath", typeof(string), typeof(ReportViewer), new PropertyMetadata(string.Empty));//ReportPathChanged

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerUrl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerUrlProperty =
        DependencyProperty.Register("ReportServerUrl", typeof(string), typeof(ReportViewer), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerCredentialProperty =
        DependencyProperty.Register("ReportServerCredential", typeof(ICredentials), typeof(ReportViewer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerFormsCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerFormsCredentialProperty =
        DependencyProperty.Register("ReportServerFormsCredential", typeof(ReportServerFormsCredential), typeof(ReportViewer), new PropertyMetadata(null));
#else
        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowContextMenu"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowContextMenuProperty =
            DependencyProperty.Register("ShowContextMenu", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ShowXpsExportButton"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowXpsExportButtonProperty =
            DependencyProperty.Register("ShowXpsExportButton", typeof(bool), typeof(ReportViewer), new PropertyMetadata(true, ShowXpsExportButtonPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportPathProperty =
        DependencyProperty.Register("ReportPath", typeof(string), typeof(ReportViewer), new PropertyMetadata(string.Empty, ProcessReport));//ReportPathChanged

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerUrl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerUrlProperty =
        DependencyProperty.Register("ReportServerUrl", typeof(string), typeof(ReportViewer), new PropertyMetadata(string.Empty, ProcessReport));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerCredentialProperty =
        DependencyProperty.Register("ReportServerCredential", typeof(ICredentials), typeof(ReportViewer), new PropertyMetadata(CredentialCache.DefaultCredentials, ProcessReport));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerFormsCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerFormsCredentialProperty =
        DependencyProperty.Register("ReportServerFormsCredential", typeof(ReportServerFormsCredential), typeof(ReportViewer), new PropertyMetadata(null, ProcessReport));
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewer"/> class.
        /// </summary>
        public ReportViewer()
        {
            this.DefaultStyleKey = typeof(ReportViewer);
            ResourceDictionary styledict = new ResourceDictionary();
            ResourceDictionary customCombo = new ResourceDictionary();
#if SILVERLIGHT
            Uri uri = new Uri("/Syncfusion.ReportViewer.Silverlight;component/Themes/Styles.xaml", UriKind.Relative);
            Uri combouri = new Uri("/Syncfusion.ReportViewer.Silverlight;component/Utils/MultiValueComboBox/MultiValueComboBox.xaml", UriKind.Relative);
#else
            Uri uri = new Uri("/Syncfusion.ReportViewer.WPF;component/Themes/Styles.xaml", UriKind.Relative);
            Uri combouri = new Uri("/Syncfusion.ReportViewer.WPF;component/Utils/MultiValueComboBox/MultiValueComboBox.xaml", UriKind.Relative);
#endif
            styledict.Source = uri;
            customCombo.Source = combouri;
            this.Resources.MergedDictionaries.Add(styledict);
            this.Resources.MergedDictionaries.Add(customCombo);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.exportControl = GetTemplateChild("PART_exportControl") as ComboBox;
            this.comboBoxPageZoom = GetTemplateChild("PART_comboBoxPageZoom") as ComboBox;
            this.Zoom = GetTemplateChild("PART_Zoom") as ScaleTransform;
            this.ShowError = GetTemplateChild("PART_ShowError") as WrapPanel;
            this.PageViewBody = GetTemplateChild("PART_PageViewBody") as Border;
            this.PageFooterBorder = GetTemplateChild("PART_PageFooterBorder") as Border;
            this.PageBodyBorder = GetTemplateChild("PART_PageBodyBorder") as Border;
            this.PageHeaderBorder = GetTemplateChild("PART_PageHeaderBorder") as Border;
            this.scrollViewer = GetTemplateChild("PART_scrollViewer") as ScrollViewer;
            this.scorllDSCredentialBlock = GetTemplateChild("PART_scorllDSCredentialBlock") as ScrollViewer;
            this.groupBoxExpandedExceptionScroll = GetTemplateChild("PART_groupBoxExpandedExceptionScroll") as ScrollViewer;
            this.scrollViewerParamBlock = GetTemplateChild("PART_scrollViewerParamBlock") as ScrollViewer;
            this.canvasContentPage = GetTemplateChild("PART_canvasContentPage") as Canvas;
            this.CanvasFooter = GetTemplateChild("PART_CanvasFooter") as Canvas;
            this.CanvasHeader = GetTemplateChild("PART_CanvasHeader") as Canvas;
            this.treeItemArea = GetTemplateChild("PART_treeItemArea") as System.Windows.Controls.Grid;
            this.renderArea = GetTemplateChild("PART_renderArea") as System.Windows.Controls.Grid;
            this.grid_ReportParameterBlock = GetTemplateChild("PART_grid_ReportParameterBlock") as System.Windows.Controls.Grid;
            this.gridRenderingRegion = GetTemplateChild("PART_gridRenderingRegion") as System.Windows.Controls.Grid;
            this.ExceptionGrid = GetTemplateChild("PART_ExceptionGrid") as System.Windows.Controls.Grid;
            this.gridException = GetTemplateChild("PART_gridException") as System.Windows.Controls.Grid;
            this.gridLoadingIndicator = GetTemplateChild("PART_gridLoadingIndicator") as System.Windows.Controls.Grid;
            this.MainGrid = GetTemplateChild("PART_MainGrid") as System.Windows.Controls.Grid;
            this.viewerSpliter = GetTemplateChild("PART_viewerSpliter") as GridSplitter;
            this.PageView = GetTemplateChild("PART_PageView") as StackPanel;
            this.sPanel_Head = GetTemplateChild("PART_sPanel_Head") as StackPanel;
            this.PageViewContainer = GetTemplateChild("PART_PageViewContainer") as StackPanel;
            this.toolBar = GetTemplateChild("PART_toolBar") as StackPanel;
            this.DocumentMap = GetTemplateChild("PART_DocumentMap") as TreeView;

            this.btnViewReport1 = GetTemplateChild("PART_btnViewReport1") as Button;
            this.buttonNext = GetTemplateChild("PART_buttonNext") as Button;
            this.buttonParameters = GetTemplateChild("PART_buttonParameters") as Button;
            this.buttonPrint = GetTemplateChild("PART_buttonPrint") as Button;
            this.buttonFirst = GetTemplateChild("PART_buttonFirst") as Button;
            this.buttonLast = GetTemplateChild("PART_buttonLast") as Button;
            this.buttonPrevious = GetTemplateChild("PART_buttonPrevious") as Button;
            this.buttonShowOrHideDocumentMap = GetTemplateChild("PART_buttonShowOrHideDocumentMap") as Button;
            this.buttonPrintLayout = GetTemplateChild("PART_buttonPrintLayout") as System.Windows.Controls.Primitives.ToggleButton;
            this.buttonPageSetup = GetTemplateChild("PART_buttonPageSetup") as Button;
            this.buttonRefresh = GetTemplateChild("PART_buttonRefresh") as Button;
            this.btnViewReport = GetTemplateChild("PART_btnViewReport") as Button;
            this.buttonFind = GetTemplateChild("PART_buttonFind") as Button;
            this.btnback = GetTemplateChild("PART_btnback") as Button;
            this.toggleShowDetails = GetTemplateChild("PART_toggleShowDetails") as System.Windows.Controls.Primitives.ToggleButton;
            this.textBoxTotalPages = GetTemplateChild("PART_textBoxTotalPages") as TextBlock;
            this.labelOf = GetTemplateChild("PART_labelOf") as TextBlock;
            this.textBlockException = GetTemplateChild("PART_textBlockException") as TextBlock;
            this.textBoxFind = GetTemplateChild("PART_textBoxFind") as TextBox;
            this.textBoxCurrentPage = GetTemplateChild("PART_textBoxCurrentPage") as TextBox;
            this.textBlockStackTrace = GetTemplateChild("PART_textBlockStackTrace") as TextBox;
            this.gridExceptionRow = GetTemplateChild("PART_gridExceptionRow") as RowDefinition;
            this.loadingIndicatorRow = GetTemplateChild("PART_loadingIndicatorRow") as RowDefinition;
            this.toolBarGridRow = GetTemplateChild("PART_toolBarGridRow") as RowDefinition;
            this.dsCredentialRow = GetTemplateChild("PART_dsCredentialRow") as RowDefinition;
            this.parameterGridRow = GetTemplateChild("PART_parameterGridRow") as RowDefinition;
            this.viewerContentRow = GetTemplateChild("PART_viewerContentRow") as RowDefinition;

#if SILVERLIGHT
            this.groupBoxExpandedException = GetTemplateChild("PART_groupBoxExpandedException") as System.Windows.Controls.Grid;
            this.zoomCanvas = GetTemplateChild("PART_zoomCanvas") as Canvas;
            this.hyperlink = GetTemplateChild("PART_hyperlink") as HyperlinkButton;
#else
            this.hyperlink = GetTemplateChild("PART_hyperlink") as Hyperlink;
            this.groupBoxExpandedException = GetTemplateChild("PART_groupBoxExpandedException") as GroupBox;
#endif
            this.isLoaded = true;

            WireEvents();
            this.buttonParameters.DataContext = this;
            UpdateToolbarCulture();
            IntializeReportViewerSettings();
            NormalView();

            foreach (var action in this.actionStore)
            {
                action.Execute();
            }

            this.actionStore.Clear();
        }

        private void WireEvents()
        {
            buttonPrint.Click+=buttonPrint_Click;
            buttonFirst.Click += buttonFirst_Click;
            buttonPrevious.Click += buttonPrevious_Click;
            buttonNext.Click += buttonNext_Click;
            buttonLast.Click += buttonLast_Click;
            btnback.Click += Btn_back_Click;
            buttonPageSetup.Click += buttonPageSetup_Click;
            buttonRefresh.Click += buttonRefresh_Click;
            buttonShowOrHideDocumentMap.Click += buttonDocumentMap_Click;
            buttonParameters.Click += ButtonParameters_OnClick;
            btnViewReport1.Click += btnViewReport_Click;
            hyperlink.Click += Hyperlink_Click;
            btnViewReport.Click += Button_View_Report_Click;
            toggleShowDetails.Click += toggleShowDetails_Click;
            textBlockStackTrace.GotFocus += textBlockStackTrace_GotFocus;
        }

        void UpdateToolbarCulture()
        {
#if !SILVERLIGHT
            this.buttonPrint.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Print");
            this.exportControl.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Export");
            this.buttonFirst.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "First");
            this.buttonLast.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Last");
            this.buttonPrevious.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Previous");
            this.buttonNext.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Next");
            this.comboBoxPageZoom.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Zoom");
            this.buttonPrintLayout.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "PrintLayout");
            this.buttonPageSetup.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "PageSetup");
            this.buttonRefresh.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Refresh");
            this.btnViewReport.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "ViewReport");
            this.buttonParameters.ToolTip = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Parameters");

#else
            ToolTipService.SetToolTip(this.buttonPrint , Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Print"));
            ToolTipService.SetToolTip(this.exportControl , Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Export"));
            ToolTipService.SetToolTip(this.buttonFirst, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "First"));
            ToolTipService.SetToolTip(this.buttonLast, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Last"));
            ToolTipService.SetToolTip(this.buttonPrevious, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Previous"));
            ToolTipService.SetToolTip(this.buttonNext, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Next"));
            ToolTipService.SetToolTip(this.comboBoxPageZoom, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Zoom"));
            ToolTipService.SetToolTip(this.buttonPrintLayout, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "PrintLayout"));
            ToolTipService.SetToolTip(this.buttonPageSetup ,Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "PageSetup"));
            ToolTipService.SetToolTip(this.buttonRefresh,  Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Refresh"));
            ToolTipService.SetToolTip(this.btnViewReport, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "ViewReport"));
            ToolTipService.SetToolTip(this.buttonParameters, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Parameters"));
#endif

        }


        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when navigation buttons state changed.
        /// </summary>
        internal event NavigationButtonVisibilityChangedEventHandler NavigationButtonsStateChanged;

        /// <summary>
        /// Occurs when toggled to normal view.
        /// </summary>
        public event ViewModeChangedEventHandler ViewModeChanged;

        /// <summary>
        /// Occurs when report loaded for ReportViewer.
        /// </summary>
        public event ReportLoadedEventHandler ReportLoaded;

        /// <summary>
        /// Occurs when view report button clicked.
        /// </summary>
        [CLSCompliant(false)]
        public event CancelEventHandler ViewButtonClick;

        public event RefreshEventHandler ReportRefresh;

        public event RefreshEventHandler ReportRefreshCompleted;

        public event ReportErrorHandler ReportError;

        public event RenderingBeginEventHandler RenderingBegin;

        public event ExportByteCompletedEventHandler ExportByteCompleted;

        public event RenderingCompletedEventHandler RenderingCompleted;

        public event SubreportProcessingEventHandler SubreportProcessing;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the zoom button in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowParametersBlock
        {
            get { return (bool)GetValue(ShowParametersBlockProperty); }
            set { SetValue(ShowParametersBlockProperty, value); }
        }

        /// <summary>
        /// Gets or sets RDL(C) report path. This initializes the report viewer.
        /// </summary>
        /// <value>The report path.</value>
        [Category("Report Viewer")]
        public string ReportPath
        {
            get
            {
                return (string)this.GetValue(ReportPathProperty);
            }
            set
            {
                this.SetValue(ReportPathProperty, value);
            }
        }

        /// <summary>
        /// Get or Set the Report Server Url.
        /// </summary>
        /// <value>The Report server url.</value>
        [Browsable(false)]
        public string ReportServerUrl
        {
            get
            {
                return (string)this.GetValue(ReportServerUrlProperty);
            }
            set
            {
                this.SetValue(ReportServerUrlProperty, value);
            }
        }

        /// <summary>
        /// Get or Set the Report Server Credential.
        /// </summary>
        /// <value>The Report server Credential.</value>
        [Browsable(false)]
        public ICredentials ReportServerCredential
        {
            get
            {
                return (ICredentials)this.GetValue(ReportServerCredentialProperty);
            }

            set
            {
                this.SetValue(ReportServerCredentialProperty, value);
            }
        }

        /// <summary>
        /// Get or Set the Report Server Forms Credential.
        /// </summary>
        /// <value>The Report server forms Credential.</value>
        [Browsable(false)]
        public ReportServerFormsCredential ReportServerFormsCredential
        {
            get
            {
                return (ReportServerFormsCredential)this.GetValue(ReportServerFormsCredentialProperty);
            }

            set
            {
                this.SetValue(ReportServerFormsCredentialProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Whether RDLC support or not.
        /// </summary>
        /// <value><c>Local</c> if this instance is RDLC; otherwise, <c>Remote</c>.</value>
        [Category("Report Viewer")]
        public ProcessingMode ProcessingMode
        {
            get
            {
                return (ProcessingMode)this.GetValue(ProcessingModeProperty);
            }
            set
            {
                if (this.ProcessingMode != value)
                {
                    this.SetValue(ProcessingModeProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value for Whether RDLC support or not.
        /// </summary>
        [Category("Report Viewer")]
        public PaperOrientation PaperOrientation
        {
            get
            {
                return (PaperOrientation)this.GetValue(PaperOrientationProperty);
            }
            set
            {
                if (this.PaperOrientation != value)
                {
                    this.SetValue(PaperOrientationProperty, value);
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the Export button and related buttons in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowExportControls
        {
            get { return (bool)GetValue(ShowExportControlsProperty); }
            set { SetValue(ShowExportControlsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is export to PDF button collapsed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is export to PDF button collapsed; otherwise, <c>false</c>.
        /// </value>
        [Category("Report Viewer")]
        public bool ShowPdfExportButton
        {
            get { return (bool)GetValue(ShowPdfExportButtonProperty); }
            set { SetValue(ShowPdfExportButtonProperty, value); }
        }

        [Category("Report Viewer")]
        public bool ShowExcelExportButton
        {
            get { return (bool)GetValue(ShowExcelExportButtonProperty); }
            set { SetValue(ShowExcelExportButtonProperty, value); }
        }

        [Category("Report Viewer")]
        public bool ShowWordExportButton
        {
            get { return (bool)GetValue(ShowWordExportButtonProperty); }
            set { SetValue(ShowWordExportButtonProperty, value); }
        }

        [Category("Report Viewer")]
        public bool ShowHtmlExportButton
        {
            get { return (bool)GetValue(ShowHtmlExportButtonProperty); }
            set { SetValue(ShowHtmlExportButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the zoom button in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowZoomControl
        {
            get { return (bool)GetValue(ShowZoomControlProperty); }
            set { SetValue(ShowZoomControlProperty, value); }
        }

        /// <summary>
        /// Gets the current page number of the report.
        /// </summary>
        [Category("Report Viewer")]
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowToolBar
        {
            get { return (bool)GetValue(ShowToolBarProperty); }
            set { SetValue(ShowToolBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page layout button should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowPageLayoutControl
        {
            get { return (bool)GetValue(ShowPageLayoutControlProperty); }
            set { SetValue(ShowPageLayoutControlProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the page Navigation button should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowPageNaviagationControls
        {
            get { return (bool)GetValue(ShowPageNavigationControlsProperty); }
            set { SetValue(ShowPageNavigationControlsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the refresh button in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowRefreshButton
        {
            get { return (bool)GetValue(ShowRefreshButtonProperty); }
            set { SetValue(ShowRefreshButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the print button in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowPrintButton
        {
            get { return (bool)GetValue(ShowPrintButtonProperty); }
            set { SetValue(ShowPrintButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the print button in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowParameterButton
        {
            get { return (bool)GetValue(ShowParametereButtonProperty); }
            set { SetValue(ShowParametereButtonProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating the Zoom Factor.
        /// </summary>
        [Category("Report Viewer")]
        public double ZoomFactor
        {
            get { return (double)this.GetValue(ZoomFactorProperty); }
            set { this.SetValue(ZoomFactorProperty, value); }
        }


        /// <summary>
        /// Gets the data sources.
        /// </summary>
        /// <value>The data sources.</value>
        [Browsable(false)]
        public ReportDataSourceCollection DataSources
        {
            get
            {
                if (this._DataSources == null)
                    this._DataSources = new ReportDataSourceCollection();
                return this._DataSources;
            }
        }

        /// <summary>
        /// Get or Set the Report Render Mode.
        /// </summary>
        /// <value>The Render Mode.</value>
        [Category("Report Viewer")]
        public ViewMode ViewMode
        {
            get
            {
                return (ViewMode)this.GetValue(ViewModeProperty);
            }
            set
            {
                if (this.ViewMode != value)
                {
                    this.SetValue(ViewModeProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the zoom button in the toolbar should be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool EnableVirtualEvaluation
        {
            get { return (bool)GetValue(EnableVirtualEvaluationProperty); }
            set { SetValue(EnableVirtualEvaluationProperty, value); }
        }

#if SILVERLIGHT
        public byte[] ExportByte 
        {
            get;
            set; 
        }

        /// <summary>
        /// Get or Set the ReportServiceURL.
        /// </summary>
        /// <value>The URL of the RemoteServer.</value>
        public string ReportServiceURL
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set the ExportMode.
        /// </summary>
        /// <value>The Mode of the Export.</value>
        [DefaultValue(ExportMode.Local)]
        public ExportMode ExportMode
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether the Load Server information in server side .
        /// </summary>
        public bool LoadCredentialsInformationinServer
        {
            get;
            set;
        }

#else
        /// <summary>
        /// Gets or sets a value indicating whether this instance is export to XPS button collapsed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is export to XPS button collapsed; otherwise, <c>false</c>.
        /// </value>
        [Category("Report Viewer")]
        public bool ShowXpsExportButton
        {
            get { return (bool)GetValue(ShowXpsExportButtonProperty); }
            set { SetValue(ShowXpsExportButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the context menu can be visible or not.
        /// </summary>
        [Category("Report Viewer")]
        public bool ShowContextMenu
        {
            get { return (bool)GetValue(ShowContextMenuProperty); }
            set { SetValue(ShowContextMenuProperty, value); }
        }

        /// <summary>
        /// Gets or sets Excel version for ReportViewer
        /// </summary>
        [Category("Report Viewer")]
        public ExcelVersion ExcelVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Doc Format type  for ReportViewer
        /// </summary>
        [Category("Report Viewer")]
        public WordFormatType WordFormatType
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is first visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsFirstVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsLastVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is next visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is next visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsNextVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is previous visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPreviousVisible
        {
            get;
            set;
        }
#endif
        internal ReportModel ReportModel
        {
            get;
            set;
        }

        int Current
        {
            get
            {
                return this.m_current;
            }

            set
            {
                if (this.m_totalPages > 0 && value <= this.m_totalPages - 1 && value >= 0)
                {
                    this.m_current = value;
#if !SILVERLIGHT
                    this.IsScroll = false;
#endif
                    this.UpdateCurrentPage();
                }
            }
        }

        internal bool HasReportParameters
        {
            get { return (bool)GetValue(HasReportParametersProperty); }
            set { SetValue(HasReportParametersProperty, value); }
        }

        internal DOM.ReportDefinition Report
        {
            get
            {
                return this.ReportModel.Report;
            }
        }

        bool IsExportable
        {
            get
            {
                return this.ReportModel.HasReport && this.pageModelFactory != null;
            }
        }

        public double MarginLeft
        {
            get;
            set;
        }

        public double MarginRight
        {
            get;
            set;
        }

        public double MarginTop
        {
            get;
            set;
        }

        public double MarginBottom
        {
            get;
            set;
        }

        public double PaperWidth
        {
            get;
            set;
        }

        public double PaperHeight
        {
            get;
            set;
        }

        public double PrintdpiX
        {
            get { return this.printDpiX; }
            set { this.printDpiX = value; }
        }

        public double PrintdpiY
        {
            get { return this.printDpiY; }
            set { this.printDpiY = value; }
        }


        internal string PaperType
        {
            get;
            set;
        }

        ReportModelContentCollection ReportModelCollection
        {
            get;
            set;
        }

        ReportModelContentCollection HeaderModelCollection
        {
            get;
            set;
        }

        ReportModelContentCollection FooterModelCollection
        {
            get;
            set;
        }

        List<DOM.DataSource> CredentailDataSource
        {
            get;
            set;
        }
        internal bool CurrentReportModel
        {
            get;
            set;
        }

        internal string ExportWriterFormat
        {
            get;
            set;
        }

        internal double DocumentMapLength
        {
            get;
            set;
        }
        #endregion

        #region Helper Events

        /// <summary>
        /// Navigate to First page
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates routed event args.</param>
        private void buttonFirst_Click(object sender, RoutedEventArgs e)
        {
            this.MoveFirst();
        }

        /// <summary>
        /// Navigate to Last page
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates routed event args.</param>
        private void buttonLast_Click(object sender, RoutedEventArgs e)
        {
            this.MoveLast();
        }

        /// <summary>
        /// Next page navigation
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates routed event args.</param>
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this.MoveNext();
        }

        /// <summary>
        /// Previous page navigation
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates routed event args.</param>
        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            this.MovePrevious();
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.ReportModel.ExceptionDetails.Clear();
            this.Refresh();
        }

        private void buttonDocumentMap_Click(object sender, RoutedEventArgs e)
        {
            var mapLength = this.renderArea.ColumnDefinitions.First().Width.Value;
            if (mapLength == 0.0)
            {
                this.renderArea.ColumnDefinitions.First().Width = new GridLength(DocumentMapLength);
            }
            else
            {
                DocumentMapLength = mapLength;
                this.renderArea.ColumnDefinitions.First().Width = new GridLength(0.0);
            }
        }

        private void textBoxCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text != string.Empty && e.Key == Key.Enter)
            {
                this.GoTo(int.Parse((sender as TextBox).Text, CultureInfo.InvariantCulture));
            }
        }

        private void textBoxCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            int pagenumber;
            if (int.TryParse(this.textBoxCurrentPage.Text, out pagenumber))
            {
                this.CurrentPage = int.Parse(this.textBoxCurrentPage.Text);
            }
        }

        /// <summary>
        /// Zooming operation
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates selction changed event args.</param>
        private void comboBoxPageZoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Regex reg = new Regex("[0-9]*");
            var value = reg.Match((this.comboBoxPageZoom.SelectedItem as ComboBoxItem).Content.ToString()).Value;
            this.zoomFactor = double.Parse(value) / 100;
            this.Zoom.ScaleX = this.zoomFactor;
            this.Zoom.ScaleY = this.zoomFactor;
#if SILVERLIGHT
            UpdateZoomCanvas();
#endif
        }

#if SILVERLIGHT
        private void UpdateZoomCanvas()
        {
            if (pageModelFactory.IsPrintMode || isExporting)
            {
                double height = this.PaperHeight;
                double width = this.PaperWidth;
                zoomCanvas.Height = height * this.zoomFactor;
                zoomCanvas.Width = width * this.zoomFactor;
            }
            else
            {
                double height = pageModelFactory.PageDictionary[this.Current].Height + pageModelFactory.HeaderHeight + pageModelFactory.FooterHeight;
                double width = pageModelFactory.PageDictionary[this.Current].Width;
                zoomCanvas.Height = height * this.zoomFactor;
                zoomCanvas.Width = width * this.zoomFactor;
            }
        }
#endif

        private void buttonPrintLayout_Click(object sender, RoutedEventArgs e)
        {
            if (this.buttonPrintLayout.IsChecked == true)
            {
                this.ViewMode = Viewer.ViewMode.Print;

                if (this.renderExceptionDetails.Count > 0)
                {
                    this.UpdateExceptionBarDetails();
                }
                this.viewerSpliter.Visibility = Visibility.Collapsed;
                this.treeItemArea.Visibility = Visibility.Collapsed;
                this.buttonShowOrHideDocumentMap.Visibility = Visibility.Collapsed;
                this.renderArea.ColumnDefinitions.First().Width = new GridLength(0);
            }
            else
            {
                if (this.ReportModel.MapModel != null && this.ReportModel.MapModel.NodeData.Count > 0)
                {
                    this.viewerSpliter.Visibility = Visibility.Visible;
                    this.treeItemArea.Visibility = Visibility.Visible;
                    this.buttonShowOrHideDocumentMap.Visibility = Visibility.Visible;
                    this.buttonShowOrHideDocumentMap.IsEnabled = true;
                    this.renderArea.ColumnDefinitions.First().Width = new GridLength(this.DocumentMapLength);
                }
                this.ViewMode = Viewer.ViewMode.Normal;
            }
        }

        /// <summary>
        /// Printing file
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates routed event args.</param>
        private void buttonPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VisualStateManager.GoToState(buttonPrint, "Unselected", true);
                this.Print();

                if (this.renderExceptionDetails.Count > 0)
                {
                    this.errorMessage = string.Empty;
                    foreach (String exce in this.renderExceptionDetails)
                    {
                        this.errorMessage += exce + "\n";
                    }
                    MessageBox.Show(this.errorMessage);
                }
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        private void textBlockStackTrace_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlockStackTrace.SelectAll();
#if !SILVERLIGHT
            this.textBlockStackTrace.Copy();
#endif
        }

        private void toggleShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (this.toggleShowDetails.IsChecked == true)
            {
                this.ShowError.Visibility = Visibility.Visible;
                this.textBlockStackTrace.Margin = new Thickness(5, 0, -5, 0);
                if (this.ReportModel.ExceptionDetails != null)
                {
                    foreach (string exception in this.ReportModel.ExceptionDetails)
                    {
                        this.textBlockStackTrace.Text += exception;
                    }
                }
                this.textBlockException.Padding = new Thickness(5);
                this.toggleShowDetails.Content = "Hide Details";
            }
            else
            {
                this.ShowError.Visibility = Visibility.Collapsed;
                this.toggleShowDetails.Content = "Show Details";
                this.textBlockStackTrace.Text = null;
            }
        }

        /// <summary>
        /// Handles the Click event of the Button_View_Report control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_View_Report_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "Please fill all the input boxes.";
            if (this.IsValidParameterValues(ref errorMessage))
            {
                CancelEventArgs arg = new CancelEventArgs();
                if (ViewButtonClick != null)
                {
                    ViewButtonClick(this, arg);
                }

                if (!arg.Cancel)
                {
                    this.ClearViewer();
                    this.UpdateReport();
                }
            }
            else
            {
#if SILVERLIGHT
                MessageBox.Show(errorMessage, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleParamError"), MessageBoxButton.OK);
#else
                MessageBox.Show(errorMessage, Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleParamError"), MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
        }

        /// <summary>
        /// Exporting Operations
        /// </summary>
        /// <param name="sender">Indicates sender.</param>
        /// <param name="e">Indicates routed event args.</param>
        private void PdfExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ExportToPdf();
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        void ReportModel_ReportLoaded(object sender, EventArgs e)
        {
            this.RaiseReportLoadedEvent();

#if SILVERLIGHT
            if (!string.IsNullOrEmpty(this.ReportModel.ReportPath))
            {
                BackgroundWorker processWorker = new BackgroundWorker();
                processWorker.DoWork += (sen, arg) =>
                {
                    this.ProcessReport();
                };
                processWorker.RunWorkerAsync();
            }
#endif
        }

#if SILVERLIGHT
        void ReportLoaded_Completed(object sender, EventArgs e)
        {
            this.Export(this.ExportWriterFormat);
        }


        void ReportingServer_ExportCompleted(object sender, RDL.ServerProcessor.ExportedEventArgs e)
        {
            this.ReportModel.ReportingServer.ExportCompleted -= new RDL.ServerProcessor.ExportedHandler(ReportingServer_ExportCompleted);
            exportFileStream.Write(e.Result, 0, e.Result.Length);
            exportFileStream.Close();
        }

        void RenderReportExport_Completed(object sender, RDL.ServerProcessor.ExportedEventArgs e)
        {
            RaiseExportByte(e.Result);
        }
       
#endif

        public void ShowPageDialog()
        {
            this.buttonPageSetup_Click(null, null);
        }

        internal void buttonPageSetup_Click(object sender, RoutedEventArgs e)
        {
            string paperName;
            Thickness margins;
            PaperSize paperSize;

            if (this.pageSetup != null)
            {
                margins = new Thickness(pageSetup.LeftMargin, pageSetup.TopMargin, pageSetup.RightMargin, pageSetup.BottomMargin);
                paperSize = new PaperSize(pageSetup.PageWidth, pageSetup.PageHeight);
                paperName = pageSetup.PaperName;
            }
            else
            {
                margins = new Thickness(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);
                paperSize = new PaperSize(this.PaperWidth, this.PaperHeight);
                paperName = "Letter";
            }

            pageSetup = new PageSetupDialog(margins, paperSize, paperName, this.PaperType);
#if ! SILVERLIGHT
            pageSetup.Owner = Window.GetWindow(this);
#endif
            this.UpdatePageSetupCulture();
            pageSetup.ShowDialog();

#if SILVERLIGHT
            pageSetup.Closed += (s, args) => 
            {
                this.ApplyPageSetupValues(pageSetup);
            };
#else
            this.ApplyPageSetupValues(pageSetup);
#endif
        }

        private void UpdatePageSetupCulture()
        {
            this.pageSetup.btn_Ok.Content = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "OK");
            this.pageSetup.btn_Cancel.Content = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Cancel");
            this.pageSetup.btn_setDefault.Content = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "SetDefault");
            this.pageSetup.textblock_bottom.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Bottom");
            this.pageSetup.textblock_top.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Top");
            this.pageSetup.textblock_left.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Left");
            this.pageSetup.textblock_right.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Right");
            this.pageSetup.textblock_size.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Size");
            this.pageSetup.textblock_paper.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Paper");
            this.pageSetup.textblock_orientation.Text = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Orientation");
            this.pageSetup.rbtn_landscape.Content = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Landscape");
            this.pageSetup.rbtn_portrait.Content = Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "Portrait");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Intialize Report Viewer with new settings
        /// </summary>
        private void IntializeReportViewerSettings()
        {
            RESX.Culture = Thread.CurrentThread.CurrentUICulture;

            this.comboBoxPageZoom.SelectedIndex = 2;
            this.buttonParameters.DataContext = this;

#if SILVERLIGHT
            //SetResourceValues();
#endif
            SetExportButtons();
#if !SyncfusionFramework3_5  && !SILVERLIGHT
            this.scrollViewer.PanningMode = PanningMode.Both;
#endif
            this.comboBoxPageZoom.SelectionChanged += new SelectionChangedEventHandler(this.comboBoxPageZoom_SelectionChanged);
            this.buttonPrintLayout.Click += new RoutedEventHandler(this.buttonPrintLayout_Click);
            this.textBoxCurrentPage.TextChanged += new TextChangedEventHandler(textBoxCurrentPage_TextChanged);
            this.textBoxCurrentPage.KeyDown += new KeyEventHandler(textBoxCurrentPage_KeyDown);
        }

        /// <summary>
        /// Move to the specified page number in the report
        /// </summary>
        /// <param name="pageNo">Page Number Want to Go</param>
        public void GoTo(int pageNo)
        {
            this.Current = pageNo - 1;
        }

        /// <summary>
        /// Finds the starting page for the control
        /// </summary>
        /// <param name="y">Indicates the control top.</param>
        /// <param name="pageHeight">Indicates the page height.</param>
        /// <returns>Returns the Starting page.</returns>
        private int FindPageStart(double y, double pageHeight)
        {
            return (int)Math.Floor(y / pageHeight);
        }

        void SetExportButtons()
        {
            this.exportOptions = new List<ExportOption>();

            pdfExport = new ExportOption() { Export = ExportType.PDF, ExportImage = this.Resources["PdfExport"] as ImageSource, Visibility = System.Windows.Visibility.Visible };
            this.exportOptions.Add(pdfExport);

            excelExport = new ExportOption() { Export = ExportType.Excel, ExportImage = this.Resources["ExcelExport"] as ImageSource, Visibility = System.Windows.Visibility.Visible };
            this.exportOptions.Add(excelExport);

            wordExport = new ExportOption() { Export = ExportType.Word, ExportImage = this.Resources["WordExport"] as ImageSource, Visibility = System.Windows.Visibility.Visible };
            this.exportOptions.Add(wordExport);

            htmlExport = new ExportOption() { Export = ExportType.Html, ExportImage = this.Resources["HtmlExport"] as ImageSource, Visibility = System.Windows.Visibility.Visible };
            this.exportOptions.Add(htmlExport);


#if !SILVERLIGHT
            xpsExport = new ExportOption() { Export = ExportType.XPS, ExportImage = this.Resources["XpsExport"] as ImageSource, Visibility = System.Windows.Visibility.Visible };
            this.exportOptions.Add(xpsExport);
#endif

            this.exportControl.ItemsSource = this.exportOptions;

            this.exportControl.DropDownOpened += (s, e) =>
            {
                this.exportControl.SelectedIndex = -1;
            };

            this.exportControl.SelectionChanged += (s, e) =>
            {
                if (this.exportControl.SelectedIndex != -1)
                {
                    ExportOption exportOption = (ExportOption)this.exportControl.SelectedValue;
                    ExportType optionType = exportOption.Export;
                    if (optionType == ExportType.PDF)
                    {
                        this.exportControl.IsDropDownOpen = false;
                        this.ExportToPdf();
                    }
                    else if (optionType == ExportType.Excel)
                    {
                        this.exportControl.IsDropDownOpen = false;
                        this.ExportToExcel();
                    }
                    else if (optionType == ExportType.Word)
                    {
                        this.exportControl.IsDropDownOpen = false;
                        this.ExportToWord();
                    }

                    else if (optionType == ExportType.Html)
                    {
                        this.exportControl.IsDropDownOpen = false;
                        this.ExportingToHtml();
                    }

#if !SILVERLIGHT
                    else if (optionType == ExportType.XPS)
                    {
                        this.ExportToXps();
                    }
#endif
                }
            };
        }

#if SILVERLIGHT
        void SetResourceValues()
        {
            //ResourceFinder finder = new ResourceFinder();
            //finder.SetResourceImage(this, "Next_NavDisabled", this.buttonNext);
            //finder.SetResourceImage(this, "Last_NavDisabled", this.buttonLast);
            //finder.SetResourceImage(this, "Previous_NavDisabled", this.buttonPrevious);
            //finder.SetResourceImage(this, "First_NavDisabled", this.buttonFirst);
            //finder.SetResourceImage(this, "PrintDisabled", this.buttonPrint);
            //finder.SetResourceImage(this, "RefreshDisabled", this.buttonRefresh);
            //finder.SetResourceImage(this, "Find", this.buttonFind);
            //finder.SetResourceImage(this, "PrintLayoutXDisabled", this.buttonPrintLayout);
            //finder.SetResourceImage(this, "PageSetup", this.buttonPageSetup);
        }
#endif

        void UpdateExportControlVisibility()
        {
            var controlVisiblity = from exportOption in this.exportOptions where exportOption.Visibility == Visibility.Visible select exportOption;
            if (controlVisiblity.Count() == 0)
            {
                this.exportControl.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyPageSetupValues(PageSetupDialog pageSetup)
        {
            if (pageSetup.IsChangesValid)
            {
                this.MarginLeft = pageSetup.LeftMargin;
                this.MarginRight = pageSetup.RightMargin;
                this.MarginTop = pageSetup.TopMargin;
                this.MarginBottom = pageSetup.BottomMargin;

                this.PaperWidth = pageSetup.PageWidth;
                this.PaperHeight = pageSetup.PageHeight;
                this.PaperType = pageSetup.PaperType;

                pageModelFactory.PageHeight = this.PaperHeight;
                pageModelFactory.PageWidth = this.PaperWidth;
                pageModelFactory.Margin = new LayoutThicknessInfo(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);
                pageModelFactory.UpdatePrintPageLayout();

                if (buttonPrintLayout.IsChecked == true)
                {
                    this.RefreshReportViewer();
                }
            }
        }

        #region Style Application Helper

        /// <summary>
        /// Sets the built in functions.
        /// </summary>
        /// <param name="pageLayoutPagesCount">The page layout pages count.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <param name="canvasPageHeader">The canvas page header.</param>
        private void SetBuiltInFunctions(int pageLayoutPagesCount, int totalPages, Canvas canvasPageHeader)
        {
            try
            {
                if (this.isExporting)
                {
                    totalPages = this.pageModelFactory.PrintLayoutPageDictionary.Count;
                }
                //// Currently we are supporting text box alone. In future, we will add rectangel, etc.,
                foreach (FrameworkElement frameworkElement in canvasPageHeader.Children)
                {
                    if (frameworkElement is ReportingTextBox)
                    {
                        ReportingTextBox rich = frameworkElement as ReportingTextBox;
                        SetRichTextBoxValue(pageLayoutPagesCount, totalPages, rich);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sets the rich text box value.
        /// </summary>
        /// <param name="pageLayoutPagesCount">The page layout pages count.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <param name="rich">The rich.</param>
        private void SetRichTextBoxValue(int pageLayoutPagesCount, int totalPages, ReportingTextBox rich)
        {
#if SILVERLIGHT
            foreach (Block prg in rich.Blocks.ToList())
#else
            foreach (Block prg in rich.Document.Blocks.ToList())
#endif
            {
                foreach (Run textRun in (prg as Paragraph).Inlines.ToList())
                {
                    if (textRun.Text.Contains("Globals.PageNumber"))
                    {
                        textRun.Text = textRun.Text.Replace("Globals.PageNumber", pageLayoutPagesCount.ToString());
                        textRun.FlowDirection = System.Windows.FlowDirection.RightToLeft;
                    }
                    if (textRun.Text.Contains("Globals.TotalPages"))
                    {
                        textRun.Text = textRun.Text.Replace("Globals.TotalPages", totalPages.ToString());
                    }
                    if (textRun.Text.Contains("Globals.ExecutionTime"))
                    {
                        textRun.Text = textRun.Text.Replace("Globals.ExecutionTime", System.DateTime.Now.ToLocalTime().ToString(CultureInfo.CurrentCulture));
                    }
                    if (textRun.Text.Contains("User.Language"))
                    {
                        textRun.Text = textRun.Text.Replace("User.Language", CultureInfo.CurrentCulture.Name.ToString());
                    }
                }
            }
        }

        #endregion

        #region Exception Window Helper

        /// <summary>
        /// Creates the exception window.
        /// </summary>
        /// <param name="message">The message.</param>
        private void CreateExceptionWindow(string message)
        {
            this.ShowExcpetionContent();

            //// Reset the toolbar to normal state.
            this.ResetToolbar();

            //// Adding message to the exception window
            this.textBlockException.Text = message;
            this.textBlockException.Width = 340;
            this.textBlockException.HorizontalAlignment = HorizontalAlignment.Left;
            this.textBlockException.TextAlignment = System.Windows.TextAlignment.Center;
            this.textBlockException.Margin = new Thickness(10, 48, 10, 24);
            this.textBlockException.Foreground = new SolidColorBrush(Colors.Gray);

            if (string.IsNullOrEmpty(this.stackTrace))
            {
                this.groupBoxExpandedException.Visibility = Visibility.Collapsed;
                this.groupBoxExpandedExceptionScroll.Visibility = System.Windows.Visibility.Collapsed;
                this.toggleShowDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.groupBoxExpandedException.Visibility = Visibility.Visible;
                this.groupBoxExpandedExceptionScroll.Visibility = System.Windows.Visibility.Visible;
                this.toggleShowDetails.Visibility = Visibility.Visible;
            }

#if SILVERLIGHT
            this.textBlockException.TextWrapping = TextWrapping.Wrap;
#else
            this.textBlockException.ContextMenu = null;
            this.textBlockException.TextWrapping = TextWrapping.WrapWithOverflow;
#endif
        }

        void NormalView()
        {
            this.buttonPrintLayout.IsChecked = false;
            this.PageView.Margin = new Thickness();
            this.PageView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.PageView.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.PageViewBody.Margin = new Thickness(0);

            //this.gridRenderingRegion.Background = new SolidColorBrush(Colors.White);

#if SILVERLIGHT
            VisualStateManager.GoToState(this, "LayoutNormal", true);
            this.zoomCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.zoomCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Top;
#endif

            this.canvasContentPage.Margin = new Thickness(0);
            this.DocumentMapLength = 130;
        }

        void PrintView()
        {
            this.buttonPrintLayout.IsChecked = true;
            this.PageView.Margin = new Thickness(0, 20, 0, 20);
            this.PageView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.PageView.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            this.PageViewBody.Margin = new Thickness(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);

#if SILVERLIGHT
            VisualStateManager.GoToState(this, "LayoutPrint", true);
            this.zoomCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.zoomCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Center;
#endif
            //this.gridRenderingRegion.Background = new SolidColorBrush(Colors.LightGray);
        }

        List<string> renderExceptionDetails = new List<string>();

        internal void UpdateExceptionBarDetails()
        {
            this.ExceptionGrid.Visibility = System.Windows.Visibility.Collapsed;

            if (this.ReportModel.ExceptionDetails.Count > 0 || this.renderExceptionDetails.Count > 0)
            {
                this.ExceptionGrid.Visibility = System.Windows.Visibility.Visible;

                this.errorMessage = string.Empty;

                foreach (var error in this.ReportModel.ExceptionDetails)
                {
                    this.errorMessage += error + "\n";
                }

                foreach (var error in this.renderExceptionDetails)
                {
                    this.errorMessage += error + "\n";
                }

                this.RaiseReportErrorEvent(this.errorMessage);
            }
        }

        /// <summary>
        /// Shows the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        internal void ShowException(Exception ex)
        {
            this.Visibility = Visibility.Visible;
            this.errorMessage = ex.Message;
            this.stackTrace = ex.StackTrace;
            this.ClearViewer();

            if (this.errorMessage.ToLowerInvariant().Contains("object reference"))
            {
                this.errorMessage = "Unexpected Error Occurred, please check the Stack Trace below";
            }

            this.CreateExceptionWindow(this.errorMessage);

            this.RaiseReportErrorEvent(this.errorMessage);
        }

        #endregion

        #region Page Creation Helper

        internal void ApplyStyle(ReportExpval style, Canvas canvas)
        {
            if (!string.IsNullOrEmpty(style.ImageValue))
            {
                foreach (Syncfusion.RDL.DOM.EmbeddedImage embeddedImage in this.ReportModel.Report.EmbeddedImages)
                {
                    if (embeddedImage.Name == style.ImageValue)
                    {
                        string imageData = embeddedImage.ImageData;
                        BitmapImage bitMapImage = new BitmapImage();
                        Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
                        bitMapImage = (BitmapImage)base64ImageConverter.ConvertToImage(imageData);
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = bitMapImage;
                        imageBrush.Stretch = Stretch.Fill;
                        canvas.Background = imageBrush;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(style.BackgroudColor))
            {
                canvas.Background = new ReportingBrushConverter().ConvertFromInvariantString(style.BackgroudColor);
            }
        }


        /// <summary>
        /// Creates the page view footer.
        /// </summary>
        private void CreatePageViewFooter(Canvas footer)
        {
            if (this.hasPageFooter)
            {
                this.PageFooterBorder.Visibility = System.Windows.Visibility.Visible;
                footer.Visibility = Visibility.Visible;
                footer.Background = new SolidColorBrush(Colors.White);

                //// Height of the Page footer            
                footer.Height = this.pageModelFactory.FooterHeight;

                this.DrawHeaderControls(this.pageModelFactory.ReportFooterModelerLists, footer);

                //// Applying the built in functions
                this.SetBuiltInFunctions((m_current + 1), int.Parse(textBoxTotalPages.Text), footer);
            }
            else
            {
                this.PageFooterBorder.Visibility = System.Windows.Visibility.Collapsed;
                footer.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Creates the page view header.
        /// </summary>
        private void CreatePageViewHeader(Canvas header)
        {
            if (this.hasPageHeader)
            {
                this.PageHeaderBorder.Visibility = System.Windows.Visibility.Visible;
                header.Visibility = Visibility.Visible;

                //// Height of the Page header
                header.Height = this.pageModelFactory.HeaderHeight;

                this.DrawHeaderControls(this.pageModelFactory.ReportHeaderModelerLists, header);

                //// Applying the built in functions
                this.SetBuiltInFunctions((m_current + 1), int.Parse(textBoxTotalPages.Text), header);
            }
            else
            {
                this.PageHeaderBorder.Visibility = System.Windows.Visibility.Collapsed;
                header.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void SetBorder(DOM.Style Style, Border border)
        {
            try
            {
                ReportingBrushConverter con = new ReportingBrushConverter();
                double leftValue = 0, topValue = 0, rightValue = 0, bottomvalue = 0;
                bool hasSetBorder = false;

                if (Style.Border != null)
                {
                    border.BorderBrush = con.ConvertFromString(Style.Border.Color);

                    if (Style.Border.Style != null && Style.Border.Style.ToLower() == "solid")
                    {
                        hasSetBorder = true;
                        if (Style.Border.Width == null)
                        {
                            border.BorderThickness = new Thickness(1);
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(Style.Border.Width.PixelValue);
                        }
                    }
                }

                if (!hasSetBorder)
                {
                    hasSetBorder = false;
                    if (Style.LeftBorder != null)
                    {
                        border.BorderBrush = !String.IsNullOrEmpty(Style.LeftBorder.Color) ? con.ConvertFromString(Style.LeftBorder.Color) : con.ConvertFromString(Style.Border.Color);

                        if (Style.LeftBorder.Style.ToLower() == "solid")
                        {
                            if (Style.LeftBorder.Width == null)
                            {
                                border.BorderThickness = new Thickness(1, 0, 0, 0);
                                leftValue = 1;
                            }
                            else
                            {
                                border.BorderThickness = new Thickness(Style.LeftBorder.Width.PixelValue, 0, 0, 0);
                                leftValue = Style.LeftBorder.Width.PixelValue;
                            }
                        }

                    }

                    if (Style.TopBorder != null)
                    {
                        border.BorderBrush = !String.IsNullOrEmpty(Style.TopBorder.Color) ? con.ConvertFromString(Style.TopBorder.Color) : con.ConvertFromString(Style.Border.Color);

                        if (Style.TopBorder.Style.ToLower() == "solid")
                        {
                            if (Style.TopBorder.Width == null)
                            {
                                border.BorderThickness = new Thickness(leftValue, 1, 0, 0);
                                topValue = 1;
                            }
                            else
                            {
                                border.BorderThickness = new Thickness(leftValue, Style.TopBorder.Width.PixelValue, 0, 0);
                                topValue = Style.TopBorder.Width.PixelValue;
                            }
                        }

                    }


                    if (Style.RightBorder != null)
                    {
                        border.BorderBrush = !String.IsNullOrEmpty(Style.RightBorder.Color) ? con.ConvertFromString(Style.RightBorder.Color) : con.ConvertFromString(Style.Border.Color);

                        if (Style.RightBorder.Style.ToLower() == "solid")
                        {
                            if (Style.RightBorder.Width == null)
                            {
                                border.BorderThickness = new Thickness(leftValue, topValue, 1, 0);
                                rightValue = 1;
                            }
                            else
                            {
                                border.BorderThickness = new Thickness(leftValue, topValue, Style.RightBorder.Width.PixelValue, 0);
                                rightValue = Style.RightBorder.Width.PixelValue;
                            }
                        }

                    }


                    if (Style.BottomBorder != null)
                    {
                        border.BorderBrush = !String.IsNullOrEmpty(Style.BottomBorder.Color) ? con.ConvertFromString(Style.BottomBorder.Color) : con.ConvertFromString(Style.Border.Color);

                        if (Style.BottomBorder.Style.ToLower() == "solid")
                        {
                            if (Style.BottomBorder.Width == null)
                            {
                                border.BorderThickness = new Thickness(leftValue, topValue, rightValue, 1);
                                bottomvalue = 1;
                            }
                            else
                            {
                                border.BorderThickness = new Thickness(leftValue, topValue, rightValue, Style.BottomBorder.Width.PixelValue);
                                bottomvalue = Style.BottomBorder.Width.PixelValue;
                            }
                        }

                    }
                }
            }
            catch
            {

            }

        }


        /// <summary>
        /// Draws controls in the given Header or Footer
        /// </summary>
        /// <param name="pageModelCollection">Page Model content collection of either Page header or Page footer</param>
        /// <param name="canvasHeaderFooter">Header or Footer canvas</param>
        private void DrawHeaderControls(ReportModelContentCollection pageModelCollection, Canvas canvasHeaderFooter)
        {
            canvasHeaderFooter.Children.Clear();

            if (pageModelCollection != null)
            {
                foreach (IReportItemModeler pageContent in pageModelCollection)
                {
                    try
                    {
                        if (pageContent != null)
                        {
                            switch (pageContent.ModelType)
                            {
                                case ModelType.TextBoxModel:
                                    canvasHeaderFooter.Children.Add(new ReportingTextBox(pageContent));
                                    break;

                                case ModelType.LineModel:
                                    canvasHeaderFooter.Children.Add(new ReportingLine(pageContent));
                                    break;

                                case ModelType.ImageModel:
                                    canvasHeaderFooter.Children.Add(new ReportingImage(pageContent));
                                    break;
                                case ModelType.RectangleModel:
                                    ReportingRectangle rect = new ReportingRectangle(pageContent);
                                    rect.InnerReportItems();
                                    canvasHeaderFooter.Children.Add(rect);
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.renderExceptionDetails.Add(" Unable to render the Reportitem " + pageContent.Name);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Draws controls in the content page
        /// </summary>
        /// <param name="reportModelCollection">Indicates the page model content collection.</param>
        /// <param name="isPrintLayout">Indicates the page model content layout.</param>
        private void DrawBodyControls(ReportModelContentCollection reportModelCollection, bool isPrintLayout)
        {
            this.canvasContentPage.Children.Clear();

            ReportingBrushConverter brush = new ReportingBrushConverter();

            foreach (IReportItemModeler reportItemContent in reportModelCollection)
            {
                try
                {
                    switch (reportItemContent.ModelType)
                    {
                        case ModelType.TablixModel:
                            ReportingTablixControl dataGrid = new ReportingTablixControl(reportItemContent);
                            this.canvasContentPage.Children.Add(dataGrid);
                            dataGrid.IsPrintMode = isPrintLayout;
                            dataGrid.CurrentPage = dataGrid.GetPage(Current);
                            dataGrid.UpdatePage();
                            break;
                        case ModelType.GaugeModel:
                            ReportingGauge dataGauge = new ReportingGauge(reportItemContent);
                            dataGauge.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(dataGauge);
                            break;
                        case ModelType.RectangleModel:
                            ReportingRectangle dataRectangle = new ReportingRectangle(reportItemContent);
                            dataRectangle.UpdateRectangleHeight(Current, isPrintLayout);
                            dataRectangle.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(dataRectangle);
                            dataRectangle.CurrentPage = dataRectangle.GetPage(Current);
                            dataRectangle.InnerReportItems();
                            break;
                        case ModelType.ChartModel:
                            ReportingChartControl dataChart = new ReportingChartControl(reportItemContent);
                            dataChart.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(dataChart);
                            break;
                        case ModelType.TextBoxModel:
                            ReportingPageControl richTextBox = new ReportingPageControl(reportItemContent, false, isPrintLayout);
                            richTextBox.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(richTextBox);
                            richTextBox.CurrentPage = richTextBox.GetPage(Current);
                            break;
                        case ModelType.LineModel:
                            ReportingLine pageLine = new ReportingLine(reportItemContent);
                            pageLine.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(pageLine);
                            break;
                        case ModelType.ImageModel:
                            ReportingImage imageControl = new ReportingImage(reportItemContent);
                            imageControl.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(imageControl);
                            break;
                        case ModelType.SubReportModel:
                            ReportingSubReport dataSubReport = new ReportingSubReport(reportItemContent);
                            dataSubReport.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(dataSubReport);
                            dataSubReport.CurrentPage = dataSubReport.GetPage(Current);
                            break;
#if !SyncfusionFramework3_5
                        case ModelType.MapModel:
                            ReportingMap mapModel = new ReportingMap(reportItemContent);
                            mapModel.IsPrintMode = isPrintLayout;
                            this.canvasContentPage.Children.Add(mapModel);
                            break;
#endif
                    }
                }
                catch (Exception e)
                {
                    this.renderExceptionDetails.Add("Unable to load Reportitem " + reportItemContent.Name + " Getting following error : " + e.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Gets the header footer.
        /// </summary>
        private void GetHeaderFooter(Canvas header, Canvas footer)
        {
            try
            {
                this.CreatePageViewHeader(header);
                this.CreatePageViewFooter(footer);

                if (ReportModel.Body.Style != null)
                {
                    this.SetBorder(ReportModel.Body.Style, this.PageBodyBorder);
                }

                if (ReportModel.Page != null && ReportModel.Page.PageHeader != null && ReportModel.Page.PageHeader.Style!=null)
                {
                    this.SetBorder(ReportModel.Page.PageHeader.Style, this.PageHeaderBorder);
                }

                if (ReportModel.Page != null && ReportModel.Page.PageFooter != null && ReportModel.Page.PageFooter.Style != null)
                {
                    this.SetBorder(ReportModel.Page.PageFooter.Style, this.PageFooterBorder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Parameters Helper

        internal void ParametersBlock(bool hide)
        {
            if (hide)
            {
                this.scrollViewerParamBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (ShowParametersBlock)
                {
                    this.scrollViewerParamBlock.Visibility = Visibility.Visible;
                }
            }
        }

        public ReportParameterInfoCollection GetParameters()
        {
            return this.ReportModel.GetParameters();
        }

        public void SetParameters(IEnumerable<ReportParameter> reportParameters)
        {
            this.ReportModel.SetParameters(reportParameters);
            ModifyReportParameters();
        }

        void ModifyReportParameters()
        {
            if (this.Report.ReportParameters != null && this.ReportModel.ParameterDetails != null)
            {
                var modifiedColl = from rparam in this.Report.ReportParameters
                                   join rp in this.ReportModel.ParameterDetails
                                   on rparam.Name equals rp.Name
                                   where !string.Equals(rp.Prompt, rparam.Prompt)
                                   select rp;
                
                if (modifiedColl != null && modifiedColl.Count() > 0)
                {
                    var parameterControlCollection = this.grid_ReportParameterBlock.Children.OfType<System.Windows.Controls.Grid>();
                    var paramBlocks = this.grid_ReportParameterBlock.Children.OfType<System.Windows.Controls.TextBlock>();
                    
                    if (paramBlocks != null && paramBlocks.Count() > 0)
                    {
                        foreach (var parameter in modifiedColl)
                        {
                            var innergrid = parameterControlCollection.Where(p => p.Name.Equals("ReportParam_" + parameter.Name)).First();
                            int row = System.Windows.Controls.Grid.GetRow(innergrid as System.Windows.Controls.Grid);
                            int col = System.Windows.Controls.Grid.GetColumn(innergrid as System.Windows.Controls.Grid);
                            TextBlock tblock = (from param in paramBlocks
                                                where row == System.Windows.Controls.Grid.GetRow(param as System.Windows.Controls.TextBlock) && (col - 1) == System.Windows.Controls.Grid.GetColumn(param as System.Windows.Controls.TextBlock)
                                                select param).First();
                            tblock.Text = parameter.Prompt;
                        }
                    }
                }
            }
        }

        void ReportParametersDataSourceUpdated(object sender, EventArgs e)
        {
            this.ReportModel.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ReportParametersDataSourceUpdated);

            internalValueChange = true;
            Syncfusion.RDL.DOM.ReportParameters reportParameters = this.Report.ReportParameters;
            var dataSetParameters = this.Report.ReportParameters.Where(p => p.ValidValues != null && p.ValidValues.DataSetReference != null);
            var parameterControlCollection = this.grid_ReportParameterBlock.Children.OfType<System.Windows.Controls.Grid>();
            foreach (var reportParam in dataSetParameters)
            {
                var comboBox = parameterControlCollection.Where(p => p.Name.Equals("ReportParam_" + reportParam.Name)).First().Children[0] as ComboBox;
                var param = (from modelParamter in this.ReportModel.ParameterDetails
                             where (modelParamter.Name.Equals(reportParam.Name))
                             select modelParamter).FirstOrDefault();
                var datas = from data in this.ReportModel.ProcessedData.DataSourceObjects
                            where (data.Key == reportParam.ValidValues.DataSetReference.DataSetName)
                            select data;

                if (datas.Count() > 0)
                {
                    string selectedValuePath = reportParam.ValidValues.DataSetReference.ValueField;
                    string displayMemberPath = reportParam.ValidValues.DataSetReference.ValueField;

                    if (reportParam.ValidValues.DataSetReference.LabelField != null)
                    {
                        displayMemberPath = reportParam.ValidValues.DataSetReference.LabelField;
                    }
                    else
                    {
                        displayMemberPath = reportParam.ValidValues.DataSetReference.ValueField;
                    }

                    IEnumerable m_itemSource = datas.First().Value;
                    List<bool> isSelected = new List<bool>();

                    if (this.ProcessingMode == Viewer.ProcessingMode.Remote)
                    {
                        List<ParameterReportData> datavalues = new List<ParameterReportData>();

                        foreach (Syncfusion.RDL.Data.ReportData data in m_itemSource)
                        {
                            datavalues.Add(new ParameterReportData() { ValueField = data.Data[selectedValuePath].ToString(), DisplayField = data.Data[displayMemberPath].ToString() });
                        }

                        selectedValuePath = "ValueField";
                        displayMemberPath = "DisplayField";
                        m_itemSource = datavalues;
                    }
                    if (reportParam.MultiValue)
                    {
                        List<ParameterReportData> datavalues = new List<ParameterReportData>();
                        ComboBox helperComboBox = new ComboBox();
                        helperComboBox.ItemsSource = m_itemSource;
                        helperComboBox.DisplayMemberPath = displayMemberPath;

                        datavalues.Add(new ParameterReportData() { ValueField = "(SelectAll)", DisplayField = "(SelectAll)", IsSelected = false });

                        foreach (object data in m_itemSource)
                        {
                            helperComboBox.SelectedItem = null;
                            helperComboBox.SelectedValuePath = selectedValuePath;
                            helperComboBox.SelectedItem = data;
                            string valueFiledValue = helperComboBox.SelectedValue.ToString();
                            helperComboBox.SelectedValuePath = displayMemberPath;
                            helperComboBox.SelectedItem = data;
                            string displayValue = helperComboBox.SelectedValue.ToString();
                            datavalues.Add(new ParameterReportData() { ValueField = valueFiledValue, DisplayField = displayValue });
                        }

                        foreach (var data in datavalues)
                        {
                            data.IsSelected = param.Value != null && param.Value.Contains(data.ValueField);
                            if (data.IsSelected==false && reportParam.DefaultValue != null && reportParam.DefaultValue.DataSetReference != null && reportParam.DefaultValue.DataSetReference.DataSetName != null)
                            {
                                  data.IsSelected = true;
                            }
                            isSelected.Add(data.IsSelected);

                        }
                        comboBox.ItemsSource = datavalues;

                        MulitValueComboBox combo = comboBox as MulitValueComboBox;
                        string selctedValues = string.Empty;

                        foreach (ParameterReportData data in datavalues)
                        {
                            if (data.IsSelected && data.DisplayField != "(SelectAll)")
                            {
                                selctedValues += data.DisplayField + ",";
                                param.Value.Add(data.ValueField);
                                param.Label.Add(data.DisplayField);
                            }
                        }

                        combo.DisplayText = selctedValues.TrimEnd(',');

                        combo.IsSelected = isSelected;


                    }
                    else
                    {
                        comboBox.ItemsSource = m_itemSource;

                        if (param.Value.Count > 0)
                        {
                            comboBox.SelectedValue = param.Value.First();
                        }
                        else
                        {
                            comboBox.SelectedIndex = -1;
                        }
                    }
                }
                else
                {
                    comboBox.ItemsSource = null;
                    comboBox.SelectedIndex = -1;
                }
            }

            internalValueChange = false;
        }

        private void UpdateDataSetParameterValues()
        {
            if (this.ProcessingMode == Viewer.ProcessingMode.Local)
            {
                this.ReportModel.DataSources = this.DataSources;
            }

            this.ReportModel.DataSourceUpdated += new DataSourceUpdatedEventHandler(ReportParametersDataSourceUpdated);
            this.ReportModel.UpdateReportParameters();
        }

        internal System.Windows.Controls.Grid GetReportItemGrid(DOM.ReportParameter reportParameter, ParameterInformation param)
        {
            System.Windows.Controls.Grid parameterGrid = new System.Windows.Controls.Grid();
            int columnIndex = 0;
            parameterGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            parameterGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            parameterGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });

            string value = (param.Value != null && param.Value.Count > 0) ? (param.Value.First() != null ? param.Value.First().ToString() : null) : null;

            if (reportParameter.MultiValue)
            {
                MulitValueComboBox comboData = new MulitValueComboBox();
                //comboData.MinWidth = 100;
#if SILVERLIGHT
                if (this.MainGrid != null && this.MainGrid.Resources["PART_comboboxStyle"] != null)
                {
                    comboData.Style = (Style)this.MainGrid.Resources["PART_comboboxStyle"];
                }
#else
                if (Template.Resources["PART_comboboxStyle"] != null)
                {
                    comboData.Style = (Style)Template.Resources["PART_comboboxStyle"];
                }
#endif
                else
                {
                    comboData.Style = (Style)this.Resources["PART_comboboxStyle"];
                }

                if (reportParameter.ValidValues != null)
                {
                    if (reportParameter.ValidValues.ParameterValues.Count() > 0)
                    {
                        List<ParameterReportData> datas = new List<ParameterReportData>();
                        datas.Add(new ParameterReportData() { ValueField = "(SelectAll)", DisplayField = "(SelectAll)", IsSelected = false });
                        List<bool> isSelected = new List<bool>();
                        isSelected.Add(false);

                        foreach (var data in reportParameter.ValidValues.ParameterValues)
                        {
                            datas.Add(new ParameterReportData() { ValueField = data.Value, DisplayField = data.Label, IsSelected = false });
                            isSelected.Add(param.Label != null && param.Label.Contains(data.Label));
                        }

                        comboData.ItemsSource = datas;
                        comboData.IsSelected = isSelected;
                    }
                }

                if (param.Label != null)
                {
                    string[] labels = new string[param.Label.Count];

                    for (int labelIndex = 0; labelIndex < param.Label.Count; labelIndex++)
                    {
                        labels[labelIndex] = (param.Label[labelIndex] != null) ? param.Label[labelIndex].ToString() : string.Empty;
                    }

                    comboData.DisplayText = string.Join(",", labels);
                }
                System.Windows.Controls.Grid.SetColumn(comboData, columnIndex++);
                parameterGrid.Children.Add(comboData);
                //comboData.Margin = new Thickness(5);
                comboData.DropDownClosed += new EventHandler(comboData_DropDownClosed);
            }
            else if (reportParameter.ValidValues != null)
            {
                ComboBox comboData = new ComboBox();
#if SILVERLIGHT
                if (this.MainGrid != null && this.MainGrid.Resources["PART_ParamComboBox"] != null)
                {
                    comboData.Style = (Style)this.MainGrid.Resources["PART_ParamComboBox"];
                }
#else
                if (Template.Resources["PART_ParamComboBox"] != null)
                {
                    comboData.Style = (Style)Template.Resources["PART_ParamComboBox"];
                }
#endif
                else
                {
                    comboData.Style = (Style)this.Resources["PART_ParamComboBox"];
                }
                //comboData.MinWidth = 100;
                //comboData.MaxWidth = 100;
                if (reportParameter.ValidValues.ParameterValues != null && reportParameter.ValidValues.ParameterValues.Count() > 0)
                {
                    comboData.ItemsSource = reportParameter.ValidValues.ParameterValues.ToList();
                    comboData.SelectedValuePath = "Value";
                    comboData.DisplayMemberPath = "Label";
#if SILVERLIGHT
                    comboData.SelectedValue = value;
#else
                    if (param.Label.Count > 0)
                    {
                        comboData.Text = param.Label.First().ToString();
                    }
#endif
                }
                else
                {
                    comboData.SelectedValuePath = reportParameter.ValidValues.DataSetReference.ValueField;

                    if (reportParameter.ValidValues.DataSetReference.LabelField != null)
                    {
                        comboData.DisplayMemberPath = reportParameter.ValidValues.DataSetReference.LabelField;
                    }
                    else
                    {
                        comboData.DisplayMemberPath = reportParameter.ValidValues.DataSetReference.ValueField;
                    }

                    if (this.ProcessingMode == Viewer.ProcessingMode.Remote)
                    {
                        comboData.SelectedValuePath = "ValueField";
                        comboData.DisplayMemberPath = "DisplayField";
                    }
                }

                System.Windows.Controls.Grid.SetColumn(comboData, columnIndex++);
                parameterGrid.Children.Add(comboData);
                //comboData.Margin = new Thickness(5);
                comboData.SelectionChanged += new SelectionChangedEventHandler(comboData_SelectionChanged);
            }
            else if (reportParameter.DataType == DOM.DataTypes.Boolean)
            {
                RadioButton rbtn_true = new RadioButton();
                RadioButton rbtn_false = new RadioButton();
                rbtn_true.Content = "True";
                rbtn_false.Content = "False";
                rbtn_true.VerticalAlignment = VerticalAlignment.Center;
                rbtn_false.VerticalAlignment = VerticalAlignment.Center;

                if (value != null)
                {
                    if (value.ToString().ToUpper().Equals("true"))
                    {
                        rbtn_true.IsChecked = true;
                    }
                    else
                    {
                        rbtn_false.IsChecked = true;
                    }
                }

                rbtn_true.SetValue(System.Windows.Controls.Grid.ColumnProperty, columnIndex++);
                parameterGrid.Children.Add(rbtn_true);
                rbtn_true.Margin = new Thickness(5);
                rbtn_false.SetValue(System.Windows.Controls.Grid.ColumnProperty, columnIndex++);
                parameterGrid.Children.Add(rbtn_false);
                rbtn_false.Margin = new Thickness(5);
                rbtn_true.Checked += new RoutedEventHandler(rbtn_true_Checked);
                rbtn_false.Checked += new RoutedEventHandler(rbtn_false_Checked);
            }
            else if (reportParameter.DataType == DOM.DataTypes.DateTime)
            {
                DateTimeEdit dateEdit = new DateTimeEdit();
                DateTime date = DateTime.Now;
                dateEdit.MinWidth = 100;
                dateEdit.MaxWidth = 100;

                if (value != null && DateTime.TryParse(value, out date))
                {
                    dateEdit.DateTime = date;
                }
                else
                {
                    dateEdit.NoneDateText = "";
                }

                System.Windows.Controls.Grid.SetColumn(dateEdit, columnIndex++);
                parameterGrid.Children.Add(dateEdit);
                dateEdit.Margin = new Thickness(5);
                dateEdit.DateTimeChanged += new PropertyChangedCallback(dateEdit_DateTimeChanged);
            }
            else if (reportParameter.DataType == DOM.DataTypes.Float ||
                reportParameter.DataType == DOM.DataTypes.Integer ||
                reportParameter.DataType == DOM.DataTypes.String)
            {
                TextBox txtBox = new TextBox();
#if SILVERLIGHT
                if (this.MainGrid != null && this.MainGrid.Resources["PART_ParamTextBox"] != null)
                {
                    txtBox.Style = (Style)this.MainGrid.Resources["PART_ParamTextBox"];
                }
#else
                if (Template.Resources["PART_ParamTextBox"] != null)
                {
                    txtBox.Style = (Style)Template.Resources["PART_ParamTextBox"];
                }
#endif
                else
                {
                    txtBox.Style = (Style)this.Resources["PART_ParamTextBox"];
                }
                //txtBox.MinWidth = 100;
                //txtBox.MaxWidth = 100;
                //txtBox.MaxHeight = 25;

                if (value != null)
                {
                    txtBox.Text = value;
                }

                txtBox.TextChanged += new TextChangedEventHandler(txtBox_TextChanged);
                txtBox.LostFocus += new RoutedEventHandler(txtBox_LostFocus);
                System.Windows.Controls.Grid.SetColumn(txtBox, columnIndex++);
                //txtBox.Margin = new Thickness(5);
                parameterGrid.Children.Add(txtBox);
            }

            if (reportParameter.Nullable == true && reportParameter.ValidValues == null)
            {
                CheckBox chkbox = new CheckBox();
                chkbox.Content = "Null";

                if (value == null)
                {
                    chkbox.IsChecked = true;
                }

                chkbox.Checked += new RoutedEventHandler(chkbox_Checked);
                chkbox.Unchecked += new RoutedEventHandler(chkbox_Unchecked);
                chkbox.VerticalAlignment = VerticalAlignment.Center;
                chkbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                chkbox.Margin = new Thickness(5);
                System.Windows.Controls.Grid.SetColumn(chkbox, columnIndex++);
                parameterGrid.Children.Add(chkbox);
            }

            return parameterGrid;
        }

        /// <summary>
        /// Check the report parameter, if any, render in report parameter block
        /// </summary>
        internal void AddReportParameters()
        {
            if (this.Report.ReportParameters != null)
            {
                int columnPosition = 0;
                int rowPosition = 0;
                char[] trimCharacters = { ' ', '@', ':' };
                this.grid_ReportParameterBlock.Children.Clear();
                var reportParameters = from reportParameter in this.Report.ReportParameters
                                       where reportParameter.Hidden == false
                                       select reportParameter;

                foreach (Syncfusion.RDL.DOM.ReportParameter reportParameter in reportParameters)
                {
                    RowDefinition rowDefn = new RowDefinition();
                    this.grid_ReportParameterBlock.RowDefinitions.Add(rowDefn);
                    TextBlock txtBlock = new TextBlock();
#if SILVERLIGHT
                    if (this.MainGrid != null && this.MainGrid.Resources["PART_ParamTextBlock"] != null)
                    {
                        txtBlock.Style = (Style)this.MainGrid.Resources["PART_ParamTextBlock"];
                    }
#else
                    if (Template.Resources["PART_ParamTextBlock"] != null)
                    {
                        txtBlock.Style = (Style)Template.Resources["PART_ParamTextBlock"];
                    }
#endif
                    else
                    {
                        txtBlock.Style = (Style)this.Resources["PART_ParamTextBlock"];
                    }

                    //txtBlock.Foreground = new SolidColorBrush(Colors.Black);
                    if (reportParameter.Prompt != null)
                    {
                        txtBlock.Text = reportParameter.Prompt.ToString(CultureInfo.InvariantCulture).Trim(trimCharacters);
                    }
                    else
                    {
                        txtBlock.Text = reportParameter.Name.ToString(CultureInfo.InvariantCulture).Trim(trimCharacters);
                    }
                    //txtBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    //txtBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    txtBlock.SetValue(System.Windows.Controls.Grid.ColumnProperty, columnPosition++);
                    txtBlock.SetValue(System.Windows.Controls.Grid.RowProperty, rowPosition);
                    //txtBlock.Margin = new Thickness(5, 5, 5, 5);
                    //txtBlock.MinWidth = 100;
                    this.grid_ReportParameterBlock.Children.Add(txtBlock);

                    var param = (from modelParamter in this.ReportModel.ParameterDetails
                                 where (modelParamter.Name.Equals(reportParameter.Name))
                                 select modelParamter).FirstOrDefault();

                    FrameworkElement parameterControl = this.GetReportItemGrid(reportParameter, param);

                    parameterControl.Name = "ReportParam_" + reportParameter.Name;
                    parameterControl.HorizontalAlignment = HorizontalAlignment.Left;
                    parameterControl.SetValue(System.Windows.Controls.Grid.ColumnProperty, columnPosition++);
                    parameterControl.SetValue(System.Windows.Controls.Grid.RowProperty, rowPosition);
                    parameterControl.Margin = new Thickness(5);
                    this.grid_ReportParameterBlock.Children.Add(parameterControl);

                    if (columnPosition == 4)
                    {
                        columnPosition = 0;
                        rowPosition++;
                    }
                }
                ModifyReportParameters();
            }
        }

        private ParameterInformation GetReportParameter(System.Windows.Controls.Grid parameter)
        {
            string paramName = parameter.Name.Remove(0, ("ReportParam_").Length);

            var param = (from modelParamter in this.ReportModel.ParameterDetails
                         where (modelParamter.Name.Equals(paramName))
                         select modelParamter).FirstOrDefault();

            return param;
        }

        private void rbtn_true_Checked(object sender, RoutedEventArgs e)
        {
            if (!internalValueChange)
            {
                RadioButton rbtn = sender as RadioButton;
                System.Windows.Controls.Grid parentElement = rbtn.Parent as System.Windows.Controls.Grid;
                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(rbtn.Content);
                param.Label.Add(rbtn.Content);
            }
        }

        private void rbtn_false_Checked(object sender, RoutedEventArgs e)
        {
            if (!internalValueChange)
            {
                RadioButton rbtn = sender as RadioButton;
                System.Windows.Controls.Grid parentElement = rbtn.Parent as System.Windows.Controls.Grid;
                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(rbtn.Content);
                param.Label.Add(rbtn.Content);
            }
        }

        void chkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!internalValueChange)
            {
                CheckBox checkBox = sender as CheckBox;
                System.Windows.Controls.Grid parentElement = checkBox.Parent as System.Windows.Controls.Grid;
                var param = GetReportParameter(parentElement);

                param.Value = null;

                foreach (var childEle in parentElement.Children.OfType<Control>())
                {
                    if (childEle != checkBox)
                    {
                        childEle.IsEnabled = false;
                    }
                }
            }
        }

        void chkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!internalValueChange)
            {
                CheckBox checkBox = sender as CheckBox;
                System.Windows.Controls.Grid parentElement = checkBox.Parent as System.Windows.Controls.Grid;

                var param = GetReportParameter(parentElement);

                param.Value = new List<object>();
                param.Value.Add(string.Empty);

                foreach (var childEle in parentElement.Children.OfType<Control>())
                {
                    if (childEle != checkBox)
                    {
                        childEle.IsEnabled = true;
                    }
                }
            }
        }

        void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (textBoxValueChanged)
            {
                System.Windows.Controls.Grid parentElement = txtBox.Parent as System.Windows.Controls.Grid;
                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(txtBox.Text);
                param.Label.Add(txtBox.Text);
                var dependentParam = from modelParameter in this.ReportModel.ParameterDetails
                                     where (!modelParameter.Name.Equals(txtBox.Name) && modelParameter.DependentParameters.Contains(param.Name))
                                     select modelParameter;

                if (dependentParam.Count() > 0)
                {
                    this.UpdateDataSetParameterValues();
                }
            }
            textBoxValueChanged = false;
        }

        void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!internalValueChange)
            {
                textBoxValueChanged = true;
            }
        }

        void comboData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (!internalValueChange)
            {
                System.Windows.Controls.Grid parentElement = combo.Parent as System.Windows.Controls.Grid;

                var param = GetReportParameter(parentElement);

                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(combo.SelectedValue);
                string label = param.Value.ToString();

                if (this.ProcessingMode == Viewer.ProcessingMode.Remote)
                {
                    ParameterReportData seletedItem = combo.SelectedItem as ParameterReportData;

                    if (seletedItem != null && seletedItem.DisplayField != null)
                    {
                        param.Label.Add(seletedItem.DisplayField.ToString());
                    }

                    DOM.ParameterValue parameter = combo.SelectedItem as DOM.ParameterValue;

                    if (parameter != null && parameter.Label != null)
                    {
                        param.Label.Add(parameter.Label);
                    }
                }
                else
                {
                    Type propertyType = combo.SelectedItem.GetType();
                    System.Reflection.PropertyInfo[] propertyInfo = propertyType.GetProperties();

                    if (propertyInfo.Where(pi => pi.Name == combo.DisplayMemberPath).FirstOrDefault() != null)
                    {
                        label = combo.SelectedItem.GetType().GetProperty(combo.DisplayMemberPath).GetValue(combo.SelectedItem, null) as string;
                    }

                    param.Label.Add(label);
                }

                var dependentParam = from modelParameter in this.ReportModel.ParameterDetails
                                     where (!modelParameter.Name.Equals(param.Name) && modelParameter.DependentParameters.Contains(param.Name))
                                     select modelParameter;

                if (dependentParam.Count() > 0)
                {
                    this.UpdateDataSetParameterValues();
                }
            }
        }

        void comboData_DropDownClosed(object sender, EventArgs e)
        {
            MulitValueComboBox combo = sender as MulitValueComboBox;
            string selctedValues = string.Empty;

            if (combo.ItemsSource != null)
            {
                System.Windows.Controls.Grid parentElement = combo.Parent as System.Windows.Controls.Grid;
                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();

                foreach (ParameterReportData data in combo.ItemsSource)
                {
                    if (data.IsSelected && data.DisplayField != "(SelectAll)")
                    {
                        selctedValues += data.DisplayField + ",";
                        param.Value.Add(data.ValueField);
                        param.Label.Add(data.DisplayField);
                    }
                }

                combo.DisplayText = selctedValues.TrimEnd(',');
            }
        }

        void dateEdit_DateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit dateTimeBox = d as DateTimeEdit;

            if (!internalValueChange)
            {
                System.Windows.Controls.Grid parentElement = dateTimeBox.Parent as System.Windows.Controls.Grid;

                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(dateTimeBox.DateTime);
                param.Label.Add(dateTimeBox.DateTime);
                var dependentParam = from modelParamter in this.ReportModel.ParameterDetails
                                     where (!modelParamter.Name.Equals(param.Name) && modelParamter.DependentParameters.Contains(param.Name))
                                     select modelParamter;

                if (dependentParam.Count() > 0)
                {
                    this.UpdateDataSetParameterValues();
                }
            }
        }

        private bool IsValidParameterValues(ref string errorMessage)
        {
            var reportParameters = from reportParameter in this.Report.ReportParameters
                                   where reportParameter.Hidden == false
                                   select reportParameter;

            foreach (Syncfusion.RDL.DOM.ReportParameter reportParameter in reportParameters)
            {
                System.Windows.Controls.Grid parentGrid = (from grid in this.grid_ReportParameterBlock.Children.OfType<System.Windows.Controls.Grid>()
                                                           where grid.Name == "ReportParam_" + reportParameter.Name
                                                           select grid).First();
                if (reportParameter.MultiValue)
                {
                    MulitValueComboBox comboBox = parentGrid.Children.OfType<MulitValueComboBox>().First();

                    if (string.IsNullOrEmpty(comboBox.DisplayText))
                    {
                        errorMessage = "Please select a value for the parameter '" + reportParameter.Prompt + "'.";
                        return false;
                    }
                }
                else if (reportParameter.ValidValues != null)
                {
                    ComboBox comboBox = parentGrid.Children.OfType<ComboBox>().First();

                    if (comboBox.SelectedIndex < 0)
                    {
                        errorMessage = "Please select a value for the parameter '" + reportParameter.Prompt + "'.";
                        return false;
                    }
                }
                else if (reportParameter.DataType == DOM.DataTypes.Boolean)
                {
                    var radioButtons = from radio in parentGrid.Children.OfType<RadioButton>()
                                       where radio.IsChecked == true
                                       select radio;

                    if (radioButtons.Count() == 0 && !reportParameter.Nullable)
                    {
                        errorMessage = "Please enter a value for the parameter '" + reportParameter.Prompt + "'. The parameter cannot be blank";
                        return false;
                    }
                }
                else if (reportParameter.DataType == DOM.DataTypes.DateTime)
                {
                    var dateTime = from radio in parentGrid.Children.OfType<DateTimeEdit>()
                                   where radio.IsEnabled == true
                                   select radio;

                    if (dateTime.Count() > 0 && dateTime.First().DateTime == null)
                    {
                        errorMessage = "Please enter a value for the parameter'" + reportParameter.Prompt + "'. The parameter cannot be blank";
                        return false;
                    }
                }
                else if (reportParameter.DataType == DOM.DataTypes.Float ||
                reportParameter.DataType == DOM.DataTypes.Integer ||
                reportParameter.DataType == DOM.DataTypes.String)
                {
                    var textBox = from radio in parentGrid.Children.OfType<TextBox>()
                                  where radio.IsEnabled == true
                                  select radio;

                    if (textBox.Count() > 0)
                    {
                        if (String.IsNullOrEmpty(textBox.First().Text) && !reportParameter.AllowBlank)
                        {
                            errorMessage = "Please enter a value for the parameter'" + reportParameter.Prompt + "'. The parameter cannot be blank";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void AddDSCredentialBlock(List<DOM.DataSource> datasource)
        {
            this.CredentailDataSource = datasource;
            sPanel_Head.Children.Clear();
            scorllDSCredentialBlock.Visibility = Visibility.Visible;
            foreach (DOM.DataSource connectionArg in datasource)
            {
                WrapPanel wPanel = new WrapPanel();
                wPanel.Name = connectionArg.Name;
                wPanel.Orientation = Orientation.Vertical;

                System.Windows.Controls.Grid gHeader = new System.Windows.Controls.Grid();

                TextBlock txtBlockHead = new TextBlock();
                txtBlockHead.Text = connectionArg.ConnectionProperties.Prompt;
                txtBlockHead.HorizontalAlignment = HorizontalAlignment.Left;
                txtBlockHead.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtBlockHead.Margin = new Thickness(5, 5, 5, 5);
                txtBlockHead.MinWidth = 100;

                gHeader.Children.Add(txtBlockHead);

                wPanel.Children.Add(gHeader);

                System.Windows.Controls.Grid gContent = new System.Windows.Controls.Grid();
#if SILVERLIGHT
                gContent.Name = connectionArg.Name+Guid.NewGuid().ToString();
#else
                gContent.Name = connectionArg.Name;
#endif
                gContent.RowDefinitions.Add(new RowDefinition());
                ColumnDefinition c1 = new ColumnDefinition();
                c1.Width = GridLength.Auto;
                gContent.ColumnDefinitions.Add(c1);

                ColumnDefinition c2 = new ColumnDefinition();
                c2.Width = GridLength.Auto;
                gContent.ColumnDefinitions.Add(c2);

                ColumnDefinition c3 = new ColumnDefinition();
                c3.Width = GridLength.Auto;
                gContent.ColumnDefinitions.Add(c3);

                ColumnDefinition c4 = new ColumnDefinition();
                c4.Width = GridLength.Auto;
                gContent.ColumnDefinitions.Add(c4);

                TextBlock txtBlockUsername = new TextBlock();
                txtBlockUsername.Text = "Username";
                txtBlockUsername.HorizontalAlignment = HorizontalAlignment.Left;
                txtBlockUsername.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtBlockUsername.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                txtBlockUsername.SetValue(System.Windows.Controls.Grid.RowProperty, 0);
                txtBlockUsername.Margin = new Thickness(5, 5, 5, 5);
                txtBlockUsername.MinWidth = 70;

                gContent.Children.Add(txtBlockUsername);

                TextBox txtUsername = new TextBox();
                txtUsername.HorizontalAlignment = HorizontalAlignment.Left;
                txtUsername.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtUsername.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                txtUsername.SetValue(System.Windows.Controls.Grid.RowProperty, 0);
                txtUsername.MinHeight = 20;
                txtUsername.MinWidth = 150;

                gContent.Children.Add(txtUsername);

                TextBlock txtBlockPassword = new TextBlock();
                txtBlockPassword.Text = "Password";
                txtBlockPassword.HorizontalAlignment = HorizontalAlignment.Left;
                txtBlockPassword.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtBlockPassword.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                txtBlockPassword.SetValue(System.Windows.Controls.Grid.RowProperty, 0);
                txtBlockPassword.Margin = new Thickness(5, 5, 5, 5);
                txtBlockPassword.MinWidth = 70;

                gContent.Children.Add(txtBlockPassword);

                PasswordBox txtPassword = new PasswordBox();
                txtPassword.HorizontalAlignment = HorizontalAlignment.Left;
                txtPassword.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtPassword.SetValue(System.Windows.Controls.Grid.ColumnProperty, 3);
                txtPassword.SetValue(System.Windows.Controls.Grid.RowProperty, 0);
                txtPassword.PasswordChar = '*';
                txtPassword.MinHeight = 20;
                txtPassword.MinWidth = 150;

                gContent.Children.Add(txtPassword);

                wPanel.Children.Add(gContent);

                System.Windows.Controls.Grid gFooter = new System.Windows.Controls.Grid();
#if SILVERLIGHT
                gFooter.Name = "Invalid" + Guid.NewGuid().ToString();
#else
                gFooter.Name = "Invalid";
#endif
                gFooter.Visibility = Visibility.Collapsed;
                TextBlock txtBlockFoot = new TextBlock();
                txtBlockFoot.Foreground = new SolidColorBrush(Colors.Red);
                txtBlockFoot.HorizontalAlignment = HorizontalAlignment.Center;
                txtBlockFoot.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtBlockFoot.Margin = new Thickness(5, 5, 5, 5);

                gFooter.Children.Add(txtBlockFoot);

                wPanel.Children.Add(gFooter);

                sPanel_Head.Children.Add(wPanel);
            }
        }

        private void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DOM.DataSource connectionArg in this.CredentailDataSource)
                {
                    System.Windows.Controls.Grid grids = (from grid in ((from wrappanels in this.sPanel_Head.Children.OfType<WrapPanel>() where wrappanels.Name == connectionArg.Name select wrappanels)).First().Children.OfType<System.Windows.Controls.Grid>() where IsStringCheck(grid.Name, connectionArg.Name) select grid).First();
                    foreach (UIElement uiElement in grids.Children)
                    {
                        if (uiElement is TextBox)
                        {
                            connectionArg.ConnectionProperties.UserName = (uiElement as TextBox).Text;
                        }
                        else if (uiElement is PasswordBox)
                        {
                            connectionArg.ConnectionProperties.PassWord = (uiElement as PasswordBox).Password;
                        }
                    }
                }
                credentialDSCount = 0;
                IsValidConnection();
            }
            catch
            {
            }
        }

        void IsValidConnection()
        {
            if (CredentailDataSource.Count != credentialDSCount)
            {
                ReportingConnectionEventArgs arg = new ReportingConnectionEventArgs();

                this.ReportModel.ConnectionValidated += new ConnectionValidatedEventHandler(ReportModel_ConnectionValidated);
                this.ReportModel.ValidateConnection(this.GetConnectionArgs(CredentailDataSource[credentialDSCount++]));
            }
            else
            {
                this.ReportModel.ConnectionValidated -= new ConnectionValidatedEventHandler(ReportModel_ConnectionValidated);
                var isCount = (from data in this.Report.DataSources where !data.ConnectionProperties.IntegratedSecurity && data.ConnectionProperties.Prompt != null && data.ConnectionProperties.UserName != null && data.ConnectionProperties.UserName != null select data).Count();
                if (isCount == CredentailDataSource.Count)
                {
                    scorllDSCredentialBlock.Visibility = Visibility.Collapsed;
                    this.UpdateReport();
                }
            }
        }

        ReportingConnectionEventArgs GetConnectionArgs(DOM.DataSource datasource)
        {
            ReportingConnectionEventArgs arg = new ReportingConnectionEventArgs();
            arg.DataProvider = datasource.ConnectionProperties.DataProvider;
            arg.DataSourceName = datasource.Name;
            arg.Promt = datasource.ConnectionProperties.Prompt;
            arg.UserName = datasource.ConnectionProperties.UserName;
            arg.Password = datasource.ConnectionProperties.PassWord;
            return arg;
        }

        void ReportModel_ConnectionValidated(object sender, ReportingConnectionValidationEventArgs e)
        {
            try
            {
                this.ReportModel.ConnectionValidated -= new ConnectionValidatedEventHandler(ReportModel_ConnectionValidated);
                System.Windows.Controls.Grid gridfopter = (from grid in ((from wrappanels in this.sPanel_Head.Children.OfType<WrapPanel>() where wrappanels.Name == e.DataSourceName select wrappanels)).First().Children.OfType<System.Windows.Controls.Grid>() where IsStringCheck(grid.Name, "Invalid") select grid).First();
                if (!e.Success)
                {
                    gridfopter.Visibility = Visibility.Visible;
                    (gridfopter.Children[0] as TextBlock).Text = "Invalid Username and Password";
                }
                else
                {
                    gridfopter.Visibility = Visibility.Collapsed;
                    (gridfopter.Children[0] as TextBlock).Text = "";
                }
                IsValidConnection();
            }
            catch { }
        }

        bool IsStringCheck(string str, string strvali)
        {
            if (str.Length != 0)
            {
                if (str.Substring(0, strvali.Length) == strvali)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Initializers

        public void Reset()
        {
            if (!this.isLoaded)
            {
                this.actionStore.Add(new CallBackActions(Reset));
                return;
            }
            if (ReportModel != null)
            {
#if SILVERLIGHT
                this.ReportModel.ReportingServer.ExportCompleted -= ReportingServer_ExportCompleted;
#endif
                this.ReportModel.ReportLoaded -= ReportModel_ReportLoaded;
                this.ReportModel.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ReportModel_DataSourceUpdated);
                this.ReportModel.ReportItemsEvaluated -= ReportModel_ReportItemsEvaluated;
                this.ReportModel.SubreportProcessing -= new SubreportProcessingEventHandler(ReportModel_SubreportProcessing);
            }

            if (currentWorker != null && currentWorker.CancellationPending)
            {
                try
                {
                    currentWorker.CancelAsync();
                }
                catch { }
            }

            this.ReportModel = null;
            this.rendered = null;
            this.ResetToolbar();
            this.renderExceptionDetails.Clear();
            this.CanvasFooter.Children.Clear();
            this.CanvasHeader.Children.Clear();
            this.canvasContentPage.Children.Clear();
            this.pageModelFactory = null;
            this.scrollViewerParamBlock.Visibility = Visibility.Collapsed;
            this.grid_ReportParameterBlock.Children.Clear();
            this.UpdateLayout();
            this.ExceptionGrid.Visibility = System.Windows.Visibility.Collapsed;
            if (this.DataSources != null)
            {
              this.DataSources.Clear();                
            }
            this.ReportPath = string.Empty;
            this.ReportServerUrl = string.Empty;
            this.ReportServerCredential = null;
            this.ReportServerFormsCredential = null;
            //if (this.ViewMode == ViewMode.Normal)
            //{
            //    this.gridRenderingRegion.Background = new SolidColorBrush(Colors.White);
            //}
            //else
            //{
            //    this.gridRenderingRegion.Background = new SolidColorBrush(Colors.LightGray);
            //}
            //this.PageView.Background = new SolidColorBrush(Colors.White);
            this.PageViewBody.Background = new SolidColorBrush(Colors.Transparent);
            this.PageViewContainer.Background = new SolidColorBrush(Colors.Transparent);
            this.PageHeaderBorder.Background = new SolidColorBrush(Colors.Transparent);
            this.PageBodyBorder.Background = new SolidColorBrush(Colors.Transparent);
            this.PageFooterBorder.Background = new SolidColorBrush(Colors.Transparent);
            this.canvasContentPage.Background = new SolidColorBrush(Colors.Transparent);
            this.CanvasFooter.Background = new SolidColorBrush(Colors.Transparent);
            this.CanvasHeader.Background = new SolidColorBrush(Colors.Transparent);
        }

        public void RefreshReport()
        {
            if (!this.isLoaded)
            {
                this.actionStore.Add(new CallBackActions(RefreshReport));
                return;
            }
            if (this.ReportModel == null)
            {
                this.ReportModel = new ReportModel();
                this.ReportModel.ReportLoaded += new ReportLoadedEventHandler(ReportModel_ReportLoaded);
            }

            this.ReportModel.EnableVirtualEvaluation = this.EnableVirtualEvaluation;

            this.ReportModel.SubreportProcessing += new SubreportProcessingEventHandler(ReportModel_SubreportProcessing);
            this.ReportModel.DrillThroughInnerReport += new DrillThroughEventHandler(ReportModel_DrillThroughInnerReport);
            this.ReportModel.ToggleChanged += new ToggleChangedEventHandler(ReportModel_ToggleChanged);
            this.ReportModel.MouseWheelScrolling += ReportModel_MouseWheelScrolling;
            this.ClearViewer();
            this.ShowLoadingIndicator();
            this.ReportModel.IsRDLC = this.ProcessingMode == Viewer.ProcessingMode.Local;
            this.HasReportParameters = false;
            BackgroundWorker processWorker = new BackgroundWorker();
            this.ReportModel.ReportException += new ReportExceptionHandler(ReportModel_ReportException);
#if SILVERLIGHT
            if (!this.isDrillReport)
            {
                this.ReportModel.ReportServerCredential = this.ReportServerCredential;
                this.ReportModel.ReportServerFormsCredential = this.ReportServerFormsCredential;
                this.ReportModel.LoadInformationFromServer = this.LoadCredentialsInformationinServer;
                this.ReportModel.ReportServerUrl = this.ReportServerUrl;
                this.ReportModel.ReportPath = this.ReportPath;
                this.ReportModel.ReportServiceURL = this.ReportServiceURL;
            }
                            
            this.isDrillReport = false;

            if (!string.IsNullOrEmpty(this.ReportPath))
            {
                this.ReportModel.ProcessReport();
            }
            else
            {
                processWorker.DoWork += (sen, arg) =>
                {
                    this.ProcessReport();
                };

                currentWorker = processWorker;
                processWorker.RunWorkerAsync();
            }
#else
            processWorker.DoWork += (sen, arg) =>
            {
                this.ProcessReport();
            };

            currentWorker = processWorker;
            processWorker.RunWorkerAsync();
#endif
        }

        void ReportModel_MouseWheelScrolling(object sender, object e)
        {
            try
            {
                if (e is MouseWheelEventArgs)
                {
                    var args = e as MouseWheelEventArgs;
                    if (args.Delta > 0)
                    {
                        this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset - 60);
                    }
                    else if (args.Delta < 0)
                    {
                        this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset + 60);
                    }
                }
            }
            catch 
            {
            }
        }

        void ReportModel_ToggleChanged(object sender, object e)
        {
            this.ReportModel.UpdateSize();
            this.UpdatePageLayout();
        }

        void ReportModel_DrillThroughInnerReport(object sender, DrillThroughEventArgs e)
        {
            DrillThroughModel drillModel = new DrillThroughModel();
            drillModel.ParentModel = this.ReportModel;
            drillModel.ChildModel = null;
            this.ReportModel = e.Model.ChildModel;
            this.ReportModel.LastPageIndex = this.Current;
            this.ReportModel.Model = drillModel;
#if SILVERLIGHT
            this.isDrillReport = true;
#endif
            this.ReportModel.ReportLoaded += new ReportLoadedEventHandler(ReportModel_ReportLoaded);
#if !SILVERLIGHT
            this.ReportModel.ProcessReport();
#endif
            if (this.ReportModel.InnerReportParameter != null)
            {
                this.ReportModel.SetParameters(this.ReportModel.InnerReportParameter as IEnumerable<Syncfusion.Windows.Reports.ReportParameter>);
            }
            this.RefreshReport();
            this.btnback.Visibility = Visibility.Visible;
        }

        void ReportModel_ReportException(object sender, ReportExceptionEventArgs e)
        {
            this.ShowException(e.Exception);
        }

        void ProcessReport()
        {
            if (this.ReportModel.HasReport)
            {
                this.MarginLeft = (this.ReportModel.Page.LeftMargin != null && this.ReportModel.Page.LeftMargin.size != null) ? this.ReportModel.Page.LeftMargin.PixelValue : 0;
                this.MarginRight = (this.ReportModel.Page.RightMargin != null && this.ReportModel.Page.RightMargin.size != null) ? this.ReportModel.Page.RightMargin.PixelValue : 0;
                this.MarginTop = (this.ReportModel.Page.TopMargin != null && this.ReportModel.Page.TopMargin.size != null) ? this.ReportModel.Page.TopMargin.PixelValue : 0;
                this.MarginBottom = (this.ReportModel.Page.BottomMargin != null && this.ReportModel.Page.BottomMargin.size != null) ? this.ReportModel.Page.BottomMargin.PixelValue : 0;

                this.PaperHeight = this.ReportModel.Page.PageHeight != null ? this.ReportModel.Page.PageHeight.PixelValue : 1122.64;
                this.PaperWidth = this.ReportModel.Page.PageHeight != null ? this.ReportModel.Page.PageWidth.PixelValue : 793.92;
                this.PaperType = (this.PaperHeight > this.PaperWidth) ? "portrait" : "landscape";

                pageModelFactory = null;
                m_totalPages = 0;

                ////// Loading indicator initialization.
                this.m_current = 0;

                if (!this.ReportModel.IsRDLC)
                {
                    List<DOM.DataSource> dataSources = null;
                    dataSources = this.ReportModel.GetCredentialDataSources();

#if SILVERLIGHT
                    if (dataSources != null && !this.LoadCredentialsInformationinServer)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.AddDSCredentialBlock(dataSources);
                        }));
                    }
#else
                    if (dataSources != null)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.AddDSCredentialBlock(dataSources);
                        }));
                    }
#endif
                    else
                    {
                        this.UpdateReport();
                    }
                }
                else
                {
                    this.UpdateReport();
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Exception ex = new Exception("The source of the report has not been specified");
                        this.ShowException(ex);
                    }));
            }
        }

        void ReportModel_GetCredentialDetailsCompleted(object sender, EventArgs e)
        {
            this.ReportModel.DataSourceCredentialsUpdated -= new DataSourceCredentialsUpdatedEventHandler(ReportModel_GetCredentialDetailsCompleted);
            this.UpdateReport();
        }

        void UpdateReport()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                     {
                         this.ShowLoadingIndicator();
                     }));

            this.ReportModel.ProcessedData.ResetDocumentModel();
            this.ReportModel.ProcessedData.ResetProceesedData();
            this.ReportModel.DataSources = this.DataSources;
            this.ReportModel.DataSourceUpdated += new DataSourceUpdatedEventHandler(ReportModel_DataSourceUpdated);

            BackgroundWorker dataSourceWorker = new BackgroundWorker();

            dataSourceWorker.DoWork += (sen, arg) =>
            {
                this.ReportModel.InitilizeReport();
            };

            currentWorker = dataSourceWorker;
            dataSourceWorker.RunWorkerAsync();
        }

        void ReportModel_DataSourceUpdated(object sender, EventArgs e)
        {
            this.ReportModel.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ReportModel_DataSourceUpdated);

            if (this.ReportModel.ProcessedData != null && this.ReportModel.ProcessedData.HasException)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        string exceptionMessage = this.ReportModel.ProcessedData.Exception.Message;
                        var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                               where reportParameter.Hidden == false
                                               select reportParameter;
                        this.HasReportParameters = reportParameters.Count() > 0;

                        Exception ex = new Exception(exceptionMessage);
                        this.ShowException(ex);
                    }));
            }
            else
            {
                this.UpdateReportModel();
            }
        }

        /// <summary>
        /// Updating the report model content collection
        /// </summary>
        private void UpdateReportModel()
        {
            BackgroundWorker evaluateWorker = new BackgroundWorker();

            evaluateWorker.DoWork += (sen, arg) =>
                {
                    this.ReportModel.ReportItemsEvaluated += new ReportItemEvaluatedHanlder(ReportModel_ReportItemsEvaluated);
                    this.ReportModel.Evaluate();
                };

            currentWorker = evaluateWorker;
            evaluateWorker.RunWorkerAsync();
        }

        void ReportModel_ReportItemsEvaluated(object sender, ReportItemEvaluatedEventArgs e)
        {
            this.ReportModel.ReportItemsEvaluated -= new ReportItemEvaluatedHanlder(ReportModel_ReportItemsEvaluated);
#if !SILVERLIGHT
            this.ReportModel.UpdateSize();
            this.UpdatePageModelFactory();
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ReportModel.UpdateSize();
                this.UpdatePageModelFactory();
            }));
#endif
        }

        private void CreateInstaceofpageModel(Exception execption)
        {
            try
            {
                this.pageModelFactory = new PageModelFactory(this.ReportModel);
                this.hasPageHeader = this.ReportModel.Page.PageHeader != null ? true : false;
                this.hasPageFooter = this.ReportModel.Page.PageFooter != null ? true : false;

                this.pageModelFactory.PageHeight = this.PaperHeight;
                this.pageModelFactory.PageWidth = this.PaperWidth;
                this.pageModelFactory.Margin = new LayoutThicknessInfo(this.MarginLeft, this.MarginTop,
                                                                       this.MarginRight, this.MarginBottom);
                this.pageModelFactory.UpdatePageLayout();
                this.ReportModel.PageModelFactory = this.pageModelFactory;
                this.ReportModel.IsToggleState = false;
            }
            catch (Exception ex)
            {
                execption = ex;
            }
        }

        private void UpdatePageModelFactory()
        {
            Exception execption = null;
            BackgroundWorker layoutWorker = new BackgroundWorker();

            layoutWorker.DoWork += (sen, arg) =>
            {
                this.CreateInstaceofpageModel(execption);
            };

            layoutWorker.RunWorkerCompleted += (sen, arg) =>
            {
                currentWorker = null;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.canvasContentPage.Children.Clear();
                    this.CanvasHeader.Children.Clear();
                    this.CanvasFooter.Children.Clear();

                    if (execption != null)
                    {
                        var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                               where reportParameter.Hidden == false
                                               select reportParameter;

                        this.HasReportParameters = reportParameters.Count() > 0;

                        this.ShowException(execption);
                    }
                    else
                    {
                        this.RefreshViewer();
                    }
                }));
            };

            currentWorker = layoutWorker;
            layoutWorker.RunWorkerAsync();
        }

        private void UpdatePageLayout()
        {
            Exception execption = null;
            BackgroundWorker layoutWorker = new BackgroundWorker();

            layoutWorker.DoWork += (sen, arg) =>
            {
                this.CreateInstaceofpageModel(execption);
            };

            layoutWorker.RunWorkerCompleted += (sen, arg) =>
            {
                currentWorker = null;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.canvasContentPage.Children.Clear();
                    this.CanvasHeader.Children.Clear();
                    this.CanvasFooter.Children.Clear();
                    if (execption != null)
                    {
                        this.ShowException(execption);
                    }
                    else
                    {
                        if (this.pageModelFactory != null)
                        {
                            this.UpdatePages();
                        }
#if !SILVERLIGHT
                        IsScroll = true;
#endif
                        this.UpdateCurrentPage();
                    }
                }));
            };

            currentWorker = layoutWorker;
            layoutWorker.RunWorkerAsync();

        }

        /// <summary>
        /// Refreshing the Viewer with Page Model
        /// </summary>
        private void RefreshViewer()
        {
            try
            {
                this.CanvasFooter.Children.Clear();
                this.CanvasHeader.Children.Clear();
                this.canvasContentPage.Children.Clear();
                this.UpdateLayout();

#if !SILVERLIGHT
                this.scrollViewer.ScrollToHome();
#endif

                var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                       where reportParameter.Hidden == false
                                       select reportParameter;

                this.HasReportParameters = reportParameters.Count() > 0;

                //// Updates toolbar icons
                this.UpdateToolBarIcons();

                this.RefreshReportViewer();
                this.ShowReportContent();
                this.DocumentMapTree();
                this.RaiseReportRefreshCompletedEvent();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Navigation Helper Methods

        void UpdateCurrentPage()
        {
            try
            {
                if (this.pageModelFactory.PageDictionary.Keys.Contains(this.Current))
                {
                    if (this.rendered == null)
                    {
                        this.rendered = false;
                        this.RaiseRenderingBeginEvent();
                    }

                    this.DrawBodyControls(pageModelFactory.PageDictionary[this.Current].ReportModelCollection, this.ViewMode == Viewer.ViewMode.Print);

                    this.UpdatePageDetails(this.Current);

                    if (pageModelFactory.IsPrintMode || isExporting)
                    {
                        this.PageView.Height = this.PaperHeight;
                        this.PageView.Width = this.PaperWidth;
                        double pageHeight = this.PaperHeight - this.MarginTop - this.MarginBottom - pageModelFactory.FooterHeight - pageModelFactory.HeaderHeight;
                        double pageWidth = this.PaperWidth - this.MarginLeft - this.MarginRight;
                        this.CanvasHeader.Width = pageWidth;
                        this.CanvasFooter.Width = pageWidth;
                        this.canvasContentPage.Height = pageHeight;
                        this.canvasContentPage.Width = pageWidth;
                    }
                    else
                    {
                        this.PageView.Height = pageModelFactory.PageDictionary[this.Current].Height + pageModelFactory.HeaderHeight + pageModelFactory.FooterHeight;
                        this.PageView.Width = pageModelFactory.PageDictionary[this.Current].Width;
                        this.CanvasHeader.Width = this.pageModelFactory.PageDictionary[this.Current].Width;
                        this.CanvasFooter.Width = this.pageModelFactory.PageDictionary[this.Current].Width;
                        this.canvasContentPage.Height = this. pageModelFactory.PageDictionary[this.Current].Height;
                        this.canvasContentPage.Width = this.pageModelFactory.PageDictionary[this.Current].Width;
                    }

                    this.GetHeaderFooter(this.CanvasHeader, this.CanvasFooter);
                    this.ApplyStyle(this.ReportModel.FooterBehaviour, this.CanvasFooter);
                    this.ApplyStyle(this.ReportModel.HeaderBehaviour, this.CanvasHeader);
                    this.ApplyStyle(this.ReportModel.BodyBehaviour, this.canvasContentPage);

#if SILVERLIGHT
                    UpdateZoomCanvas();
#else
                    this.UpdateNavigationButtonVisibility();
                    this.RaiseNavigationChanged();

                    if (!IsScroll)
                    {
                        this.scrollViewer.ScrollToHome();
                        IsScroll = !IsScroll;
                    }
#endif
                    this.UpdateExceptionBarDetails();

                    if (this.rendered != null && this.rendered == false)
                    {
                        this.rendered = true;
                        this.RaiseRenderingCompletedEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                this.renderExceptionDetails.Add("Getting following exception while rendering report : " + ex.Message);
                this.ShowException(ex);
            }
        }

        void DocumentMapTree()
        {
            this.DocumentMap.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(DocumentMap_SelectedItemChanged);
            this.DocumentMap.Items.Clear();
            TreeViewItem treeItem = new TreeViewItem();
            treeItem.Header = this.GetFileName();
            this.DocumentMap.Items.Add(treeItem);
            this.DocumentMap.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(DocumentMap_SelectedItemChanged);

            if (this.ReportModel.MapModel != null && this.ReportModel.MapModel.NodeData.Count > 0)
            {
                this.NodeIteration(this.ReportModel.MapModel.NodeData, treeItem);
                this.treeItemArea.Visibility = Visibility.Visible;
                this.viewerSpliter.Visibility = Visibility.Visible;
                this.buttonShowOrHideDocumentMap.Visibility = Visibility.Visible;
                this.renderArea.ColumnDefinitions.First().Width = new GridLength(this.DocumentMapLength);
                this.buttonShowOrHideDocumentMap.IsEnabled = true;
            }
            else
            {
                this.treeItemArea.Visibility = Visibility.Collapsed;
                this.viewerSpliter.Visibility = Visibility.Collapsed;
                this.DocumentMapLength = this.renderArea.ColumnDefinitions.First().Width.Value;
                this.renderArea.ColumnDefinitions.First().Width = new GridLength(0.0);
                this.buttonShowOrHideDocumentMap.Visibility = Visibility.Collapsed;
            }
        }

        void DocumentMap_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OldValue != e.NewValue)
            {
                TreeViewItem view = e.NewValue as TreeViewItem;
                if (this.DocumentMap.Items[0] != view)
                {
                    if (!string.IsNullOrEmpty(view.Header.ToString()))
                    {
                        var node = (view.Tag as DocumentData);
                        {
                            this.GoTo(node.PageNo);
                            this.scrollViewer.ScrollToHorizontalOffset(node.LeftPos);
                            this.scrollViewer.ScrollToVerticalOffset(node.TopPos);
                        }
                    }
                }
                else
                {
                    this.GoTo(1);
                    this.scrollViewer.ScrollToHorizontalOffset(0);
                    this.scrollViewer.ScrollToVerticalOffset(0);
                }
            }
        }

        void NodeIteration(List<DocumentData> Nodes, TreeViewItem treeItem)
        {
            foreach (var node in Nodes)
            {
                TreeViewItem tree = new TreeViewItem();
                //tree.Name = node.ReportItemName;
                tree.Header = node.DocumentLable;
                tree.Tag = node;
                treeItem.Items.Add(tree);
                if (node.Node != null)
                {
                    this.NodeIteration(node.Node.NodeData, tree);
                }
            }
        }


        /// <summary>
        /// Navigates to the First possible page of the current report.
        /// </summary>
        public void MoveFirst()
        {
            this.Current = 0;
        }

        /// <summary>
        /// Navigates to the Last possible page of the current report.
        /// </summary>
        public void MoveLast()
        {
            this.Current = m_totalPages - 1;
        }

        /// <summary>
        /// Navigates to the Next page of the current report.
        /// </summary>
        public void MoveNext()
        {
            this.Current = this.Current + 1;
        }

        /// <summary>
        /// Navigates to the Previous page of the current report.
        /// </summary>
        public void MovePrevious()
        {
            this.Current = this.Current - 1;
        }

        internal void RaiseViewModeChanged()
        {
            if (this.ViewModeChanged != null)
            {
                this.OnViewModeChanged(this, new ViewModeChangedEventArgs());
            }
        }

        internal void RaiseExportByte(byte[] e)
        {
            if (ExportByteCompleted != null)
            {
#if SILVERLIGHT
                if (e is byte[])
                {
                    this.ExportByte = e as byte[];                    
                }
#endif
                ExportByteCompleted(this, e);
            }
        }

        void ReportModel_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            RaiseSubReportProcessingEvent(e);
        }

        protected virtual void RaiseSubReportProcessingEvent(SubreportProcessingEventArgs e)
        {
            if (this.SubreportProcessing != null)
            {
                this.SubreportProcessing(this, e);
            }
        }

        protected virtual void RaiseReportLoadedEvent()
        {
            if (this.ReportLoaded != null)
            {
                this.ReportLoaded(this, new EventArgs());
            }
        }

        protected virtual void RaiseReportRefreshEvent()
        {
            if (this.ReportRefresh != null)
            {
                this.ReportRefresh(this, new EventArgs());
            }
        }

        protected virtual void RaiseReportRefreshCompletedEvent()
        {
            if (this.ReportRefreshCompleted != null)
            {
                this.ReportRefreshCompleted(this, new EventArgs());
            }
        }

        protected virtual void RaiseReportErrorEvent(string messsage)
        {
            if (this.ReportError != null)
            {
                this.ReportError(this, new ReportErrorEventArgs { Message = messsage });
            }
        }

        protected virtual void RaiseRenderingBeginEvent()
        {
            if (this.RenderingBegin != null)
            {
                this.RenderingBegin(this, new EventArgs());
            }
        }

        protected virtual void RaiseRenderingCompletedEvent()
        {
            if (this.RenderingCompleted != null)
            {
                this.RenderingCompleted(this, new EventArgs());
            }
        }

#if !SILVERLIGHT

        public void Export(Stream stream, ExportFormat type)
        {
            if (this.IsExportable)
            {
                byte[] exportStreamArray = null;
                if (type == ExportFormat.Excel)
                {
                    exportStreamArray = this.ExportByte("Excel");
                }
                else if (type == ExportFormat.Word)
                {
                    exportStreamArray = this.ExportByte("Word");
                }
                else if (type == ExportFormat.PDF)
                {
                    exportStreamArray = this.ExportByte("Pdf");
                }


                else if (type == ExportFormat.Html)
                {
                    exportStreamArray = this.ExportByte("Html");
                }
                if (exportStreamArray != null)
                {
                    stream.Write(exportStreamArray, 0, exportStreamArray.Length);
                }
            }
        }

        /// <summary>
        /// Raises the navigation changed.
        /// </summary>
        internal void RaiseNavigationChanged()
        {
            if (this.NavigationButtonsStateChanged != null)
            {
                this.OnNavigationButtonsStateChanged(this, new NavigationButtonVisibilityChangedEventArgs(this.IsFirstVisible, this.IsPreviousVisible, this.IsNextVisible, this.IsLastVisible));
            }
        }

        /// <summary>
        /// Updates the navigation button visibility.
        /// </summary>
        private void UpdateNavigationButtonVisibility()
        {
            this.IsFirstVisible = this.buttonFirst.IsEnabled;
            this.IsPreviousVisible = this.buttonPrevious.IsEnabled;
            this.IsNextVisible = this.buttonNext.IsEnabled;
            this.IsLastVisible = this.buttonLast.IsEnabled;
        }
#endif
        #endregion

        #region EventsHelper Methods

        /// <summary>
        /// Called when [navigation buttons state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Reports.Viewer.NavigationButtonVisibilityChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNavigationButtonsStateChanged(object sender, NavigationButtonVisibilityChangedEventArgs e)
        {
            if (this.NavigationButtonsStateChanged != null)
            {
                this.NavigationButtonsStateChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [toggled to normal view].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Reports.Viewer.ViewModeChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModeChanged(object sender, ViewModeChangedEventArgs e)
        {
            //// This will be fired if the view is toggled to normal view.
            if (this.ViewModeChanged != null)
            {
                this.ViewModeChanged(this, e);
            }
        }

        #endregion

        #region Property Change Call Back

        /// <summary>
        /// Called when [has report parameters property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnHasReportParametersPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonParameters != null)
            {
                reportViewer.buttonParameters.Visibility = Visibility.Collapsed;
                reportViewer.ParametersBlock(!(bool)e.NewValue);

                if (!(bool)e.OldValue && (bool)e.NewValue)
                {
                    if (reportViewer.ShowParameterButton)
                    {
                        reportViewer.buttonParameters.Visibility = Visibility.Visible;
                    }
                    reportViewer.AddReportParameters();
                    reportViewer.UpdateDataSetParameterValues();
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnHasReportParametersPropertyChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when [toolbar visibility property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowToolBarChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.toolBar != null && reportViewer.MainGrid != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.MainGrid.RowDefinitions[0].Height = new GridLength(50);
                    reportViewer.toolBar.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.toolBar.Visibility = Visibility.Collapsed;
                    reportViewer.MainGrid.RowDefinitions[0].Height = new GridLength(0);
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowToolBarChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when [is page layout button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowPageLayoutControlPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonPrintLayout != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.buttonPrintLayout.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.buttonPrintLayout.Visibility = Visibility.Collapsed;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowPageLayoutControlPropertyChanged, dependencyObject, e));
            }
        }
        /// <summary>
        /// Called when [is page layout button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowPageNavigationControlsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonFirst != null && reportViewer.buttonLast != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.buttonFirst.Visibility = Visibility.Visible;
                    reportViewer.buttonPrevious.Visibility = Visibility.Visible;
                    reportViewer.textBoxCurrentPage.Visibility = Visibility.Visible;
                    reportViewer.labelOf.Visibility = Visibility.Visible;
                    reportViewer.buttonNext.Visibility = Visibility.Visible;
                    reportViewer.buttonLast.Visibility = Visibility.Visible;
                    reportViewer.textBoxTotalPages.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.buttonFirst.Visibility = Visibility.Collapsed;
                    reportViewer.buttonPrevious.Visibility = Visibility.Collapsed;
                    reportViewer.textBoxCurrentPage.Visibility = Visibility.Collapsed;
                    reportViewer.labelOf.Visibility = Visibility.Collapsed;
                    reportViewer.buttonNext.Visibility = Visibility.Collapsed;
                    reportViewer.buttonLast.Visibility = Visibility.Collapsed;
                    reportViewer.textBoxTotalPages.Visibility = Visibility.Collapsed;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowPageNavigationControlsPropertyChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when [is refresh button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowRefreshButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonRefresh != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.buttonRefresh.Visibility = Visibility.Collapsed;
                }
                else
                {
                    reportViewer.buttonRefresh.Visibility = Visibility.Visible;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowRefreshButtonPropertyChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when [is print button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowPrintButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonPrint != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.buttonPrint.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.buttonPrint.Visibility = Visibility.Collapsed;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowPrintButtonPropertyChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when [is parameter button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowParameterButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonParameters != null)
            {
                if ((bool)e.NewValue == true && (bool)e.OldValue == false)
                {
                    //reportViewer.resourceFinder.SetResourceImage(reportViewer, "Parameters", reportViewer.buttonParameters);
                    reportViewer.buttonParameters.IsEnabled = true;
                    reportViewer.buttonParameters.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.buttonParameters.Visibility = Visibility.Collapsed;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowParameterButtonPropertyChanged, dependencyObject, e));
            }
        }


        /// <summary>
        /// Called when [is zoom button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowZoomControlPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.comboBoxPageZoom != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.comboBoxPageZoom.Visibility = Visibility.Collapsed;
                }
                else
                {
                    reportViewer.comboBoxPageZoom.Visibility = Visibility.Visible;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowZoomControlPropertyChanged, dependencyObject, e));
            }
        }
        /// <summary>
        /// Called when [is zoom button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnShowParametersBlockChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.scrollViewerParamBlock != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.ParametersBlock(false);
                }
                else
                {
                    reportViewer.ParametersBlock(true);
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnShowParametersBlockChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when [is last button collapsed property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnIsLastButtonCollapsedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.buttonLast != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportViewer.buttonLast.Visibility = Visibility.Collapsed;
                }
                else
                {
                    reportViewer.buttonLast.Visibility = Visibility.Visible;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(OnIsLastButtonCollapsedPropertyChanged, dependencyObject, e));
            }
        }

        static void ZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ReportViewer reportViewer = d as ReportViewer;
            if (reportViewer != null && reportViewer.Zoom != null)
            {
                if ((double)args.NewValue != (double)args.OldValue)
                {
                    reportViewer.zoomFactor = ((double)args.NewValue) / 100;
                    reportViewer.Zoom.ScaleX = reportViewer.zoomFactor;
                    reportViewer.Zoom.ScaleY = reportViewer.zoomFactor;
                    try
                    {
                        var zoomValue = Math.Floor((double)args.NewValue) + "%";
                        for (int i = 0; i < reportViewer.comboBoxPageZoom.Items.Count; i++)
                        {
                            ComboBoxItem comboitem = (ComboBoxItem)reportViewer.comboBoxPageZoom.Items[i];
                            if (comboitem.Content.Equals(zoomValue))
                            {
                                reportViewer.comboBoxPageZoom.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ZoomFactorPropertyChanged, d, args));
            }
        }

        /// <summary>
        /// PaperOrientation Mode changed.
        /// </summary>
        private static void PaperOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ReportViewer reportViewer = d as ReportViewer;
            if (reportViewer != null && reportViewer.PaperType != null)
            {
                if (args.NewValue != null)
                {
                    if (((PaperOrientation)args.NewValue) == PaperOrientation.Portrait)
                    {
                        reportViewer.PaperType = "portrait";
                    }
                    else
                    {
                        reportViewer.PaperType = "landscape";
                    }
                    if (reportViewer.pageModelFactory != null)
                    {
                        reportViewer.pageModelFactory.PageHeight = reportViewer.PaperHeight;
                        reportViewer.pageModelFactory.PageWidth = reportViewer.PaperWidth;
                        reportViewer.pageModelFactory.Margin = new LayoutThicknessInfo(reportViewer.MarginLeft, reportViewer.MarginTop, reportViewer.MarginRight, reportViewer.MarginBottom);
                        reportViewer.pageModelFactory.UpdatePrintPageLayout();
                        reportViewer.RefreshReportViewer();
                    }
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(PaperOrientationPropertyChanged, d, args));
            }
        }

        /// <summary>
        /// View Mode changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void ViewModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ReportViewer reportViewer = d as ReportViewer;
            if (reportViewer != null && reportViewer.buttonPrintLayout != null)
            {
                try
                {
                    if (args.NewValue != null)
                    {
                        if (((ViewMode)args.NewValue) == ViewMode.Normal)
                        {
                            reportViewer.RaiseViewModeChanged();

                            reportViewer.buttonPrintLayout.IsChecked = true;

                            reportViewer.NormalView();
                        }
                        else
                        {
                            reportViewer.RaiseViewModeChanged();

                            if (reportViewer.pageModelFactory != null)
                            {
                                reportViewer.PrintView();
                            }
                            else
                            {
                                reportViewer.buttonPrintLayout.IsChecked = true;

                                reportViewer.PageView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                reportViewer.PageView.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                //reportViewer.gridRenderingRegion.Background = new SolidColorBrush(Colors.LightGray);

                                reportViewer.PageView.Margin = new Thickness(0, 20, 0, 20);
                            }
                        }

                        reportViewer.RefreshReportViewer();
                    }
                }
                catch (Exception ex)
                {
                    reportViewer.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        reportViewer.ShowException(ex);
                    }));
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ViewModePropertyChanged, d, args));
            }
        }

        /// <summary>
        /// Called when export control button collapsed property changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void ShowExportControlsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.exportControl != null)
            {
                if ((bool)e.NewValue)
                {
                    reportViewer.exportControl.Visibility = Visibility.Visible;
                    reportViewer.UpdateExportControlVisibility();
                }
                else
                {
                    reportViewer.exportControl.Visibility = Visibility.Collapsed;
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ShowExportControlsPropertyChanged, dependencyObject, e));
            }
        }

        /// <summary>
        /// Called when export to PDF button collapsed property changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void ShowPdfExportButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.pdfExport != null)
            {
                if ((bool)e.NewValue)
                {
                    reportViewer.pdfExport.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.pdfExport.Visibility = Visibility.Collapsed;
                    reportViewer.UpdateExportControlVisibility();
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ShowPdfExportButtonPropertyChanged, dependencyObject, e));
            }
        }

        static void ShowExcelExportButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.excelExport != null)
            {
                if ((bool)e.NewValue)
                {
                    reportViewer.excelExport.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.excelExport.Visibility = Visibility.Collapsed;
                    reportViewer.UpdateExportControlVisibility();
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ShowExcelExportButtonPropertyChanged, dependencyObject, e));
            }
        }

        static void ShowWordExportButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.wordExport != null)
            {

                if ((bool)e.NewValue)
                {
                    reportViewer.wordExport.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.wordExport.Visibility = Visibility.Collapsed;
                    reportViewer.UpdateExportControlVisibility();
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ShowWordExportButtonPropertyChanged, dependencyObject, e));
            }
        }

        static void ShowHtmlExportButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.htmlExport != null)
            {

                if ((bool)e.NewValue)
                {
                    reportViewer.htmlExport.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.htmlExport.Visibility = Visibility.Collapsed;
                    reportViewer.UpdateExportControlVisibility();
                }
            }
            else if (reportViewer != null)
            {
                reportViewer.actionStore.Add(new CallBackActions(ShowHtmlExportButtonPropertyChanged, dependencyObject, e));
            }
        }

#if !SILVERLIGHT

        static void ProcessReport(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ReportViewer reportViewer = d as ReportViewer;

            if (reportViewer != null)
            {
                try
                {
                    if (reportViewer.ReportModel == null)
                    {
                        reportViewer.ReportModel = new ReportModel();
                        reportViewer.EnableVirtualEvaluation = reportViewer.EnableVirtualEvaluation;
                        reportViewer.ReportModel.ReportLoaded += new ReportLoadedEventHandler(reportViewer.ReportModel_ReportLoaded);
                    }

                    reportViewer.ReportModel.ReportPath = reportViewer.ReportPath;
                    reportViewer.ReportModel.ReportServerUrl = reportViewer.ReportServerUrl;
                    reportViewer.ReportModel.ReportServerCredential = reportViewer.ReportServerCredential;
                    reportViewer.ReportModel.ReportServerFormsCredential = reportViewer.ReportServerFormsCredential;
                    reportViewer.ReportModel.IsRDLC = reportViewer.ProcessingMode == ProcessingMode.Local;
                    reportViewer.ReportModel.ProcessReport();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Called when export to XPS button collapsed property changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void ShowXpsExportButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = dependencyObject as ReportViewer;

            if (reportViewer != null && reportViewer.xpsExport != null)
            {
                if ((bool)e.NewValue)
                {
                    reportViewer.xpsExport.Visibility = Visibility.Visible;
                }
                else
                {
                    reportViewer.xpsExport.Visibility = Visibility.Collapsed;
                    reportViewer.UpdateExportControlVisibility();
                }
            }
        }
#endif

        #endregion

        #region Layout Updation Helper

        /// <summary>
        /// Refresh the report and Navigates to the first possible page and refresh data of the report.
        /// </summary>
        public void Refresh()
        {
            if (!this.isLoaded)
            {
                this.actionStore.Add(new CallBackActions(Refresh));
                return;
            }
            this.ClearViewer();
            this.RaiseReportRefreshEvent();
            this.UpdateReport();
        }

        internal void RefreshReportViewer()
        {
            if (this.pageModelFactory != null)
            {
                this.UpdatePages();
                this.Current = 0;
            }
        }

        void UpdatePages()
        {
            if (this.pageModelFactory != null)
            {
                this.pageModelFactory.IsPrintMode = this.ViewMode == Viewer.ViewMode.Print;
                this.m_totalPages = pageModelFactory.PageDictionary.Count;
                this.textBoxTotalPages.Text = this.m_totalPages.ToString();

                if (this.pageModelFactory.IsPrintMode)
                {
                    this.PageViewBody.Margin = new Thickness(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);
                }

                this.canvasContentPage.InvalidateArrange();
                this.CanvasHeader.InvalidateArrange();
            }
        }

        /// <summary>
        /// Resets the toolbar.
        /// </summary>
        private void ResetToolbar()
        {
            //// Disable ShowHide - Logic comes here
            //resourceFinder.SetResourceImage(this, "ShowHideDisabled", this.buttonShowOrHideDocumentMap);
            this.buttonShowOrHideDocumentMap.IsEnabled = false;

            //if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
            //{
            //    //// Disable First - Logic for "First" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "First_NavDisabled", this.buttonFirst);

            //    //// Disable Previous - Logic for "Previous" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "Previous_NavDisabled", this.buttonPrevious);

            //    //// Disable Next - Logic comes "Next" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "Next_NavDisabled", this.buttonNext);

            //    //// Disable Last - Logic comes "Last" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "Last_NavDisabled", this.buttonLast);
            //}

            //else
            //{
            //    //// Disable First - Logic for "First" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "Last_NavDisabled", this.buttonFirst);

            //    //// Disable Previous - Logic for "Previous" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "Next_NavDisabled", this.buttonPrevious);

            //    //// Disable Next - Logic comes "Next" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "Previous_NavDisabled", this.buttonNext);

            //    //// Disable Last - Logic comes "Last" implemented in UpdatePageDetails
            //    resourceFinder.SetResourceImage(this, "First_NavDisabled", this.buttonLast);
            //}

            this.buttonFirst.IsEnabled = false;
            this.buttonPrevious.IsEnabled = false;
            this.buttonNext.IsEnabled = false;
            this.buttonLast.IsEnabled = false;

            //// Disable Refresh - Logic comes here
            //resourceFinder.SetResourceImage(this, "RefreshDisabled", this.buttonRefresh);
            this.buttonRefresh.IsEnabled = false;

            //// Disable Print - Logic comes here
            //resourceFinder.SetResourceImage(this, "PrintDisabled", this.buttonPrint);
            this.buttonPrint.IsEnabled = false;

            //// Disable Print setup - Logic comes here
            //resourceFinder.SetResourceImage(this, "PageSetup", this.buttonPageSetup);
            this.buttonPageSetup.IsEnabled = false;

            //// Disable Page layout - Logic comes here
            //resourceFinder.SetResourceImage(this, "PrintLayoutXDisabled", this.buttonPrintLayout);
            this.buttonPrintLayout.IsEnabled = false;

            //// Disable Zoom
            this.comboBoxPageZoom.IsEnabled = false;

            //// Disable Find
            //resourceFinder.SetResourceImage(this, "FindDisabled", this.buttonFind);
            this.buttonFind.IsEnabled = false;

            //// Disable Parameters - Logic comes here
            //resourceFinder.SetResourceImage(this, "ParametersDisabled", this.buttonParameters);
            this.buttonParameters.IsEnabled = false;

            this.textBoxCurrentPage.Text = "0";

            this.textBoxTotalPages.Text = "0";

            this.exportControl.IsEnabled = false;
        }

        /// <summary>
        /// Updates the Current and Total page text boxes
        /// </summary>
        /// <param name="q">Indicates the current page number.</param>
        private void UpdatePageDetails(int q)
        {
            q++;

            if (q == 1)
            {
                this.buttonFirst.IsEnabled = false;
                this.buttonPrevious.IsEnabled = false;
                //if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                //{
                //    resourceFinder.SetResourceImage(this, "First_NavDisabled", this.buttonFirst);
                //    resourceFinder.SetResourceImage(this, "Previous_NavDisabled", this.buttonPrevious);
                //}

                //else
                //{
                //    resourceFinder.SetResourceImage(this, "Last_NavDisabled", this.buttonFirst);
                //    resourceFinder.SetResourceImage(this, "Next_NavDisabled", this.buttonPrevious);
                //}
            }
            else
            {
                this.buttonFirst.IsEnabled = true;
                this.buttonPrevious.IsEnabled = true;
                //if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                //{
                //    resourceFinder.SetResourceImage(this, "First_Nav", this.buttonFirst);
                //    resourceFinder.SetResourceImage(this, "Previous_Nav", this.buttonPrevious);
                //}
                //else
                //{
                //    resourceFinder.SetResourceImage(this, "Last_Nav", this.buttonFirst);
                //    resourceFinder.SetResourceImage(this, "Next_Nav", this.buttonPrevious);
                //}
            }

            textBoxCurrentPage.Text = q.ToString();

            if (q == m_totalPages)
            {
                this.buttonNext.IsEnabled = false;
                this.buttonLast.IsEnabled = false;
                //if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                //{
                //    resourceFinder.SetResourceImage(this, "Next_NavDisabled", this.buttonNext);
                //    resourceFinder.SetResourceImage(this, "Last_NavDisabled", this.buttonLast);
                //}

                //else
                //{
                //    resourceFinder.SetResourceImage(this, "Previous_NavDisabled", this.buttonNext);
                //    resourceFinder.SetResourceImage(this, "First_NavDisabled", this.buttonLast);
                //}

            }
            else
            {
                this.buttonNext.IsEnabled = true;
                this.buttonLast.IsEnabled = true;
                //if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                //{
                //    resourceFinder.SetResourceImage(this, "Next_Nav", this.buttonNext);
                //    resourceFinder.SetResourceImage(this, "Last_Nav", this.buttonLast);
                //}

                //else
                //{
                //    resourceFinder.SetResourceImage(this, "Previous_Nav", this.buttonNext);
                //    resourceFinder.SetResourceImage(this, "First_Nav", this.buttonLast);
                //}
            }

            if (this.m_totalPages > 1)
            {
                this.textBoxCurrentPage.IsEnabled = true;
            }
            else
            {
                this.textBoxCurrentPage.IsEnabled = false;
            }
        }

        /// <summary>
        /// Updates the tool bar icons.
        /// </summary>
        private void UpdateToolBarIcons()
        {
            /// Enable ShowHide - Logic comes here
            //resourceFinder.SetResourceImage(this, "ShowHideDisabled", this.buttonShowOrHideDocumentMap);
            this.buttonShowOrHideDocumentMap.IsEnabled = false;

            //// Enable First - Logic for "First" implemented in UpdatePageDetails
            //resourceFinder.SetResourceImage(this, "First_Nav", this.buttonFirst);
            this.buttonFirst.IsEnabled = true;

            //// Enable Previous - Logic for "Previous" implemented in UpdatePageDetails
            //resourceFinder.SetResourceImage(this, "Previous_Nav", this.buttonPrevious);
            this.buttonPrevious.IsEnabled = true;

            //// Enable Next - Logic comes "Next" implemented in UpdatePageDetails
           // resourceFinder.SetResourceImage(this, "Next_Nav", this.buttonNext);
            this.buttonNext.IsEnabled = true;

            //// Enable Last - Logic comes "Last" implemented in UpdatePageDetails
            //resourceFinder.SetResourceImage(this, "Last_Nav", this.buttonLast);
            this.buttonLast.IsEnabled = true;

            //// Enable Refresh - Logic comes here
            //resourceFinder.SetResourceImage(this, "Refresh", this.buttonRefresh);
            this.buttonRefresh.IsEnabled = true;

            //// Enable Print - Logic comes here
            //resourceFinder.SetResourceImage(this, "Print", this.buttonPrint);
            this.buttonPrint.IsEnabled = true;

            //// Print setup - Logic comes here
            //resourceFinder.SetResourceImage(this, "PageSetup", this.buttonPageSetup);
            this.buttonPageSetup.IsEnabled = true;

            //// Page layout - Logic comes here
            //resourceFinder.SetResourceImage(this, "PrintLayoutX", this.buttonPrintLayout);
            this.buttonPrintLayout.IsEnabled = true;

            //// Enable Zoom
            this.comboBoxPageZoom.IsEnabled = true;

            //// Enable Find
            this.textBoxFind.IsEnabled = true;

           // resourceFinder.SetResourceImage(this, "Find", this.buttonFind);
            this.buttonFind.IsEnabled = true;

            //// Enable Parameters - Logic comes here
            if (this.HasReportParameters && this.ShowParameterButton)
            {
                //resourceFinder.SetResourceImage(this, "Parameters", this.buttonParameters);
                this.buttonParameters.IsEnabled = true;
            }
            else
            {
                //resourceFinder.SetResourceImage(this, "ParametersDisabled", this.buttonParameters);
                this.buttonParameters.IsEnabled = false;
            }

            this.exportControl.IsEnabled = true;
        }

        #endregion

        #region Print Report

#if SILVERLIGHT

        public void Print()
        {
            PrintDocument printDocument = new PrintDocument();

            if (pageModelFactory != null)
            {
                int printPageCount = pageModelFactory.PrintLayoutPageDictionary.Count;
                int startPage = 0;
                int currentPage = this.Current;
                int selectedZoom = this.comboBoxPageZoom.SelectedIndex;
                this.comboBoxPageZoom.SelectedIndex = 2;

                if (this.ViewMode == Viewer.ViewMode.Normal)
                {
                    SetPrintLayout();
                }

                this.comboBoxPageZoom.SelectedIndex = 2;
                this.isExporting = true;
                printDocument.PrintPage += (s, args) =>
                {
                    try
                    {
                        this.m_current = startPage;
                        var layout = GetVisual(startPage);
                        args.PageVisual = layout;

                        if (this.PaperType.ToLower() == "landscape")
                        {
                            var transform = new TransformGroup();
                            transform.Children.Add(new RotateTransform() { Angle = 90 });
                            transform.Children.Add(new TranslateTransform() { X = args.PrintableArea.Width });
                            layout.RenderTransform = transform;
                        }

                        startPage++;
                        args.HasMorePages = startPage < printPageCount;
                    }
                    catch (Exception e)
                    {
                        this.renderExceptionDetails.Add(e.Message);
                    }

                };

                printDocument.EndPrint += (s, args) =>
                {
                    this.isExporting = false;

                    if (this.PaperType.ToLower() == "landscape")
                    {
                        for (int i = 0; i < printPageCount; i++)
                        {
                            var layout = GetVisual(i);
                            var transform = new TransformGroup();
                            transform.Children.Add(new RotateTransform() { Angle = 0 });
                            transform.Children.Add(new TranslateTransform() { X = 0 });
                            layout.RenderTransform = transform;
                        }
                    }
                    if (this.ViewMode == Viewer.ViewMode.Normal)
                    {
                        ReSetPrintLayout();
                    }

                    this.Current = currentPage;
                    this.comboBoxPageZoom.SelectedIndex = selectedZoom;
                };
                printDocument.Print("Silverlight print document");
            }
        }

#else
        public void Print()
        {
            System.Windows.Forms.PrintDialog dialog = new System.Windows.Forms.PrintDialog();
            
            dialog.Document = this.GetPrintDocument();
            dialog.AllowSomePages = true;
            dialog.AllowCurrentPage = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (this.ViewMode == Viewer.ViewMode.Normal)
                {
                    SetPrintLayout();
                }

                this.comboBoxPageZoom.SelectedIndex = 2;
                this.isExporting = true;
                dialog.Document.Print();
            }         
        }

        public System.Drawing.Printing.PrintDocument GetPrintDocument()
        {
            System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
            printDocument.DefaultPageSettings.Landscape = (this.PaperType.ToLower() == "landscape") ? true : false;
            if (pageModelFactory != null)
            {
                int printPageCount = pageModelFactory.PrintLayoutPageDictionary.Count;
                int startPage = 0;
                int currentPage = this.Current;
                int selectedZoom = this.comboBoxPageZoom.SelectedIndex;
                this.comboBoxPageZoom.SelectedIndex = 2;
                printDocument.PrinterSettings.MaximumPage = printPageCount;
                bool isCalculatedrange = false;
                int printedpagescount = 0;

                printDocument.PrintPage += (s, args) =>
                {
                    try
                    {
                        if (printDocument.PrinterSettings.ToPage > 0 && !isCalculatedrange)
                        {
                            startPage = printDocument.PrinterSettings.FromPage - 1;
                            printPageCount = (printDocument.PrinterSettings.ToPage - printDocument.PrinterSettings.FromPage) + 1 > printPageCount ? printPageCount : (printDocument.PrinterSettings.ToPage - printDocument.PrinterSettings.FromPage) + 1;
                            isCalculatedrange = true;
                        }

                        this.m_current = startPage;
                        args.Graphics.DrawImage(GetImageFromUIElement(GetVisual(startPage), this.PrintdpiX, this.PrintdpiY), new System.Drawing.Rectangle(0, 0, (int)this.PageView.Width, (int)this.PaperHeight));
                        startPage++;
                        printedpagescount++;
                        args.HasMorePages = startPage < printPageCount;

                        if (isCalculatedrange)
                        {
                            args.HasMorePages = (printPageCount != printedpagescount);
                        }

                    }
                    catch (Exception e)
                    {
                        this.renderExceptionDetails.Add(e.Message);
                    }
                };

                printDocument.EndPrint += (s, args) =>
                {
                    this.isExporting = false;
                    if (this.ViewMode == Viewer.ViewMode.Normal)
                    {
                        ReSetPrintLayout();
                    }

                    else
                    {
                        this.PageView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        this.PageView.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    }

                    this.Current = currentPage;
                    this.comboBoxPageZoom.SelectedIndex = selectedZoom;

                };
            }
            return printDocument;
        }

        public List<System.Drawing.Image> ExportAsImage()
        {
            if (pageModelFactory != null)
            {
                return ExportAsImage(1, pageModelFactory.PrintLayoutPageDictionary.Count);
            }
            return null;
        }

        internal List<System.Drawing.Image> ExportAsImage(int startPage, int endPage)
        {
            int currentPage = this.Current;
            List<System.Drawing.Image> images = new List<System.Drawing.Image>();
            if (this.ViewMode == Viewer.ViewMode.Normal)
            {
                SetPrintLayout();
            }
            try
            {
                int spage = startPage - 1;
                int epage = endPage - 1;
                if (spage >= 0 && pageModelFactory.PrintLayoutPageDictionary.Count >= endPage)
                {
                    if (spage <= endPage)
                    {
                        for (int i = spage; i <= epage; i++)
                        {
                            this.m_current = i;
                            var image = GetImageFromUIElement(GetVisual(i), this.PrintdpiX, this.PrintdpiY);
                            images.Add(image);
                        }
                    }
                }
            }
            catch {
                images.Clear();
            }
            if (this.ViewMode == Viewer.ViewMode.Normal)
            {
                ReSetPrintLayout();
            }
            else
            {
                this.PageView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                this.PageView.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
            this.Current = currentPage;
            return images.Count > 0 ? images : null ;
        }

        public static System.Drawing.Image GetImageFromUIElement(StackPanel source, double dpix, double dpiy)
        {
            //System.Drawing.Image image = null;

            int width = (int)source.ActualWidth;
            int height = (int)source.ActualHeight;

            if (width == 0)
                width = (int)source.Width;
            if (height == 0)
                height = (int)source.Height;

            int dpiwidth = (int)((width * dpix) / 96);
            int dpiheight = (int)((height * dpiy) / 96);

            RenderTargetBitmap bitmap = new RenderTargetBitmap(dpiwidth, dpiheight, dpix, dpiy, PixelFormats.Pbgra32);
            bitmap.Render(source);

            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                source.Clip = null;
                bitmap.Freeze();
                bitmap.Clear();
                return System.Drawing.Image.FromStream(stream);
            }

            //return image;
        }
#endif

        StackPanel GetVisual(int pageNumber)
        {
            this.GetHeaderFooter(this.CanvasHeader, this.CanvasFooter);

            if (this.ViewMode == Viewer.ViewMode.Print)
            {
                this.PageView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                this.PageView.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            }

            this.DrawBodyControls(pageModelFactory.PrintLayoutPageDictionary[pageNumber].ReportModelCollection, true);
            this.canvasContentPage.UpdateLayout();
            this.PageView.UpdateLayout();
            return PageView;
        }

        void SetPrintLayout()
        {
            double footerHeight = this.hasPageHeader ? this.ReportModel.Page.PageHeader.Height.PixelValue : 0;
            double headerHeight = this.hasPageFooter ? this.ReportModel.Page.PageFooter.Height.PixelValue : 0;

            double pageAreaHeight = this.PaperHeight - this.MarginTop - this.MarginBottom;
            double pageAreaWidth = this.PaperWidth - this.MarginLeft - this.MarginRight;

            double pageHeight = pageAreaHeight - footerHeight - headerHeight;

            if (this.hasPageFooter)
            {
                this.CanvasFooter.Width = pageAreaWidth;
            }

            if (this.hasPageHeader)
            {
                this.CanvasHeader.Width = pageAreaWidth;
            }

            this.PageView.Height = this.PaperHeight;
            this.PageView.Width = this.PaperWidth;

            this.PageViewBody.Height = pageAreaHeight;
            this.PageViewBody.Width = pageAreaWidth;

            this.canvasContentPage.Height = pageHeight;
            this.canvasContentPage.Width = pageAreaWidth;

            this.PageViewBody.Margin = new Thickness(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);
        }

        void ReSetPrintLayout()
        {
            this.PageView.Margin = new Thickness(0);
            this.canvasContentPage.Margin = new Thickness(0);
            this.PageViewBody.Margin = new Thickness(0, 0, 0, 0);
            this.PageViewBody.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.PageViewBody.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            if (this.ViewMode == ViewMode.Normal)
            {
                this.PageViewBody.Width = double.NaN;
                this.PageViewBody.Height = double.NaN;
            }
            this.DocumentMapLength = 130;
        }

        #endregion

        #region Helper methods

        string GetExportFileName()
        {
            if (!string.IsNullOrEmpty(this.ReportPath))
            {
                if (File.Exists(this.ReportPath))
                {
                    string fileName = System.IO.Path.GetFileName(this.ReportPath);
                    return fileName.Substring(0, fileName.LastIndexOf('.'));
                }
                else
                {
                    return this.ReportPath.TrimStart('/');
                }
            }

            return "Report";
        }

        string GetFileName()
        {
            return !string.IsNullOrEmpty(this.ReportPath) ? System.IO.Path.GetFileNameWithoutExtension(this.ReportPath) : "Report";
        }

        void ClearViewer()
        {
            this.canvasContentPage.Children.Clear();
            this.CanvasHeader.Children.Clear();
            this.CanvasFooter.Children.Clear();
        }

        internal void ShowReportContent()
        {
            ShowLoadingGrid(false);
            ShowExceptionGrid(false);
            ShowContentGrid(true);
        }

        internal void ShowExcpetionContent()
        {
            ShowLoadingGrid(false);
            ShowContentGrid(false);
            ShowExceptionGrid(true);
        }

        internal void ShowLoadingIndicator()
        {
            ShowContentGrid(false);
            ShowExceptionGrid(false);
            ShowLoadingGrid(true);
        }

        void ShowExceptionGrid(bool show)
        {
            if (show)
            {
                this.gridException.Visibility = Visibility.Visible;
                this.gridExceptionRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                this.gridException.Visibility = Visibility.Collapsed;
                this.gridExceptionRow.Height = new GridLength(0);
            }
        }

        void ShowContentGrid(bool show)
        {
            if (show)
            {
                this.scrollViewer.Visibility = Visibility.Visible;
                this.viewerContentRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                this.scrollViewer.Visibility = Visibility.Collapsed;
                this.viewerContentRow.Height = new GridLength(0);
            }
        }

        void ShowLoadingGrid(bool show)
        {
            if (show)
            {
                this.gridLoadingIndicator.Visibility = Visibility.Visible;
                this.loadingIndicatorRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                this.gridLoadingIndicator.Visibility = Visibility.Collapsed;
                this.loadingIndicatorRow.Height = new GridLength(0);
            }
        }

        private void ResetPageNumbers()
        {
            this.textBoxTotalPages.Text = "0";
            this.textBoxCurrentPage.Text = "0";
        }

        public IList<string> GetDataSetNames()
        {
            if (this.ReportModel.IsRDLC)
            {
                return this.ReportModel.GetDataSetNames();
            }

            return null;
        }

        public ReportDataSourceInfoCollection GetDataSources()
        {
            if (!this.ReportModel.IsRDLC && this.ReportModel.HasReport)
            {
                return this.ReportModel.GetDataSources();
            }

            return null;
        }

        public void SetDataSourceCredentials(Syncfusion.Windows.Reports.DataSourceCredentials[] dataSourceCredentials)
        {
            if (!this.ReportModel.IsRDLC && this.ReportModel.HasReport)
            {
                this.ReportModel.SetDataSourceCredentials(dataSourceCredentials);
            }
        }

        public void SetDataSourceCredentials(IEnumerable<Syncfusion.Windows.Reports.DataSourceCredentials> dataSourceCredentials)
        {
            if (!this.ReportModel.IsRDLC && this.ReportModel.HasReport)
            {
                this.ReportModel.SetDataSourceCredentials(dataSourceCredentials);
            }
        }

        public int GetTotalPage()
        {
            if (pageModelFactory != null)
            {
                return pageModelFactory.PageCount;
            }

            return 0;
        }

        public void LoadReport(Stream fileStream)
        {
            if (this.ReportModel == null)
            {
                this.ReportModel = new ReportModel();
                this.ReportModel.EnableVirtualEvaluation = this.EnableVirtualEvaluation;
                this.ReportModel.ReportLoaded += new ReportLoadedEventHandler(ReportModel_ReportLoaded);
            }

            this.ReportModel.IsRDLC = this.ProcessingMode == Viewer.ProcessingMode.Local;
            this.ReportModel.LoadReport(fileStream);
        }

        internal void LoadReport(DOM.ReportDefinition designerReport)
        {
            if (this.ReportModel == null)
            {
                this.ReportModel = new ReportModel();
                this.ReportModel.EnableVirtualEvaluation = this.EnableVirtualEvaluation;
                this.ReportModel.ReportLoaded += new ReportLoadedEventHandler(ReportModel_ReportLoaded);
            }

            this.ReportModel.IsRDLC = this.ProcessingMode == Viewer.ProcessingMode.Local;
            this.ReportModel.LoadReport(designerReport);
        }

        public void LoadSubreport(string reportName, Stream report)
        {
            if (this.ReportModel == null)
            {
                this.ReportModel = new ReportModel();
                this.ReportModel.EnableVirtualEvaluation = this.EnableVirtualEvaluation;
                this.ReportModel.ReportLoaded += new ReportLoadedEventHandler(ReportModel_ReportLoaded);
            }

            if (this.ReportModel.SubReportStream.Keys.Contains(reportName))
            {
                this.ReportModel.SubReportStream[reportName] = report;
            }
            else
            {
                this.ReportModel.SubReportStream.Add(reportName, report);
            }
            try
            {
                var subreports = from reportitem in this.ReportModel.BodyReportItemModels where reportitem.ReportItem != null && (reportitem.ReportItem is RDL.DOM.SubReport) && (reportitem.ReportItem as RDL.DOM.SubReport).ReportName.Equals(reportName) select reportitem;
                if (subreports != null)
                {
                    foreach (var subreport in subreports.ToList())
                    {
                        subreport.Load();
                    }
                }
            }
            catch { }
        }

        public void LoadSubreport(string reportName, string reportPath)
        {
            if (File.Exists(reportPath))
            {
                using (FileStream stream = new FileStream(reportPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    this.LoadSubreport(reportName, stream);
                }
            }
        }

        public void LoadSubreport(string reportName, TextReader report)
        {
            byte[] reportBytes = System.Text.Encoding.UTF8.GetBytes(report.ReadToEnd());
            Stream stream = new MemoryStream(reportBytes.ToArray());
            this.LoadSubreport(reportName, stream);
        }

        #endregion

        #region Export to PDF

#if SILVERLIGHT
        
        /////// <summary>
        /////// Exporting To Pdf of Reporting Object
        /////// </summary>
        void ExportToPdf()
        {
            if (this.IsExportable == true)
            {
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Pdf(*.pdf)|*.pdf",
                };
                this.ExportReport(saveDialog, "Pdf");
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        /////// <summary>
        /////// Exporting To Pdf of Reporting Object
        /////// </summary>
        void ExportToExcel()
        {
            if (this.IsExportable == true)
            {
                // Creating save file dialog box.
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Excel(*.xls)|*.xls",
                };

                this.ExportReport(saveDialog, "excel");
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        /////// <summary>
        /////// Exporting To Pdf of Reporting Object
        /////// </summary>
        void ExportToWord()
        {
            if (this.IsExportable == true)
            {
                // Creating save file dialog box.
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Word(*.doc)|*.doc",
                };

                this.ExportReport(saveDialog, "word");
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        /////// <summary>
        /////// Exporting to Pdf of Reporting Object
        /////// </summary>
        void ExportingToHtml()
        {
            if (this.IsExportable == true)
            {
                // Creating save file dialog box.
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Html(*.html)|*.html",
                };

                this.ExportReport(saveDialog, "html");
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// the user object convert into reportdata.
        /// </summary>
        /// <param name="reportSettingsVar"></param>
        List<Syncfusion.Reports.Server.ReportData> GetWrapperDataSource(IEnumerable Datasoure)
        {
            List<Syncfusion.Reports.Server.ReportData> dataSource1 = new List<Syncfusion.Reports.Server.ReportData>();
            IEnumerable list = Datasoure;
            foreach (object o in list)
            {
                Syncfusion.Reports.Server.ReportData data = new Syncfusion.Reports.Server.ReportData();
                data.Data = new Dictionary<string, object>();
                System.Type objectType = o.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(objectType.GetProperties());
                foreach (PropertyInfo prop in props)
                {
                    string propValue = prop.Name;
                    object getObjectValue = prop.GetValue(o, null);
                    data.Data.Add(propValue, getObjectValue);
                }
                dataSource1.Add(data);
            }
            return dataSource1;
        }

        public void RenderReport(string writerformat)
        {

            this.ExportWriterFormat = writerformat;
            if (this.CurrentReportModel==false)
            {
                this.ReportModel = new ReportModel();
                this.ReportModel.ReportLoaded += new ReportLoadedEventHandler(ReportLoaded_Completed);
                this.CurrentReportModel = true;
            }
            this.ReportModel.ReportPath = this.ReportPath;
            this.ReportModel.ReportServiceURL = this.ReportServiceURL;
            this.ReportModel.LoadInformationFromServer = this.LoadCredentialsInformationinServer;

            if (!string.IsNullOrEmpty(this.ReportPath))
            {
                this.ReportModel.ProcessReport();
            }
            else
            {
                this.ProcessReport();
            }
        }

        public void Export(string writerformat)
        {
            this.ReportModel.ReportingServer.ExportCompleted += new RDL.ServerProcessor.ExportedHandler(RenderReportExport_Completed);
            ReportSetting reportSetting = new ReportSetting() { ReportPath = this.ReportModel.ReportPath, ReportServerURL = this.ReportModel.ReportServerUrl };

            if (this.ReportModel.ReportServerCredential != null)
            {
                reportSetting.ReportServerCredential = this.ReportModel.GetReportServerCredential();
            }

            if (this.ReportModel.ReportServerFormsCredential != null)
            {
                reportSetting.ReportServerFormCredential = this.ReportModel.GetReportServerFormCredential();
            }

            if (string.IsNullOrEmpty(this.ReportPath) && this.ReportModel.ReportStreamByte != null)
            {
                reportSetting.Report = this.ReportModel.ReportStreamByte;
            }

            List<Syncfusion.Reports.Server.ReportParameterInfo> parametersInfo = new List<Syncfusion.Reports.Server.ReportParameterInfo>();
            ReportParameterInfoCollection parameterInfo = this.GetParameters();

            foreach (var parameter in parameterInfo)
            {
                Syncfusion.Reports.Server.ReportParameterInfo paramInfo = new Syncfusion.Reports.Server.ReportParameterInfo();
                paramInfo.Name = parameter.Name;

                paramInfo.Labels = new string[parameter.Labels.Count];
                paramInfo.Values = new string[parameter.Values.Count];

                int index = 0;
                foreach (var value in parameter.Values)
                {
                    paramInfo.Values[index++] = value;
                }

                index = 0;

                foreach (var label in parameter.Labels)
                {
                    paramInfo.Labels[index++] = label;
                }

                parametersInfo.Add(paramInfo);
            }

            reportSetting.Parameters = parametersInfo.ToArray();
            if (this.ProcessingMode == Viewer.ProcessingMode.Remote)
            {

                List<Syncfusion.Reports.Server.DataSourceCredentialsInfo> credentialsInfo = new List<Syncfusion.Reports.Server.DataSourceCredentialsInfo>();

                if (this.ReportModel.Report.DataSources != null)
                {
                    foreach (var dataSource in this.ReportModel.Report.DataSources)
                    {
                        DataSourceCredentialsInfo dataSourceInfo = new DataSourceCredentialsInfo();
                        dataSourceInfo.Name = dataSource.Name;
                        dataSourceInfo.UserId = dataSource.ConnectionProperties.UserName;
                        dataSourceInfo.Password = dataSource.ConnectionProperties.PassWord;
                        dataSourceInfo.IntegratedSecurity = dataSource.ConnectionProperties.IntegratedSecurity;
                        credentialsInfo.Add(dataSourceInfo);
                    }
                }

                reportSetting.DataSourceCredentials = credentialsInfo.ToArray();
            }
            else
            {
                List<Syncfusion.Reports.Server.ReportDataSource> listDatasourceinfo = new List<Syncfusion.Reports.Server.ReportDataSource>();
                Dictionary<string, IEnumerable> datasourceCollection = this.ReportModel.ProcessedData.DataSourceObjects;

                foreach (KeyValuePair<string, IEnumerable> DataSources in datasourceCollection)
                {
                    Syncfusion.Reports.Server.ReportDataSource datasourceinfo = new Syncfusion.Reports.Server.ReportDataSource();
                    datasourceinfo.Name = DataSources.Key;
                    datasourceinfo.Value = this.GetWrapperDataSource(DataSources.Value).ToArray();
                    listDatasourceinfo.Add(datasourceinfo);
                }
                reportSetting.DataSources = listDatasourceinfo.ToArray();
            }
            this.ReportModel.ReportingServer.Export(reportSetting, writerformat);
        }

      public void ExportReport(SaveFileDialog saveDialog, string writerFormat)
        {
            if (saveDialog.ShowDialog() == true)
            {
                exportFileStream = saveDialog.OpenFile();

                if (this.ExportMode == Viewer.ExportMode.Local)
                {
                    ReportWriter.WriterFormat format = (Syncfusion.ReportWriter.WriterFormat)Enum.Parse(typeof(Syncfusion.ReportWriter.WriterFormat), writerFormat, true);
                    ReportWriter.ReportWriter reportWriter = new ReportWriter.ReportWriter();
                    this.ReportModel.IsEvaluatedReport = true;
                    reportWriter.ReportModel = this.ReportModel;
                    reportWriter.Save(exportFileStream, format);
                    exportFileStream.Close();
                }

                else if(!String.IsNullOrEmpty(this.ReportServiceURL))
                {
                    this.ReportModel.ReportingServer.ExportCompleted += new RDL.ServerProcessor.ExportedHandler(ReportingServer_ExportCompleted);
                    ReportSetting reportSetting = new ReportSetting() { ReportPath = this.ReportModel.ReportPath, ReportServerURL = this.ReportModel.ReportServerUrl };

                    if (this.ReportModel.ReportServerCredential != null)
                    {
                        reportSetting.ReportServerCredential = this.ReportModel.GetReportServerCredential();
                    }

                    if (this.ReportModel.ReportServerFormsCredential != null)
                    {
                        reportSetting.ReportServerFormCredential = this.ReportModel.GetReportServerFormCredential();
                    }

                    if (string.IsNullOrEmpty(this.ReportPath) && this.ReportModel.ReportStreamByte != null)
                    {
                        reportSetting.Report = this.ReportModel.ReportStreamByte;
                    }

                    List<Syncfusion.Reports.Server.ReportParameterInfo> parametersInfo = new List<Syncfusion.Reports.Server.ReportParameterInfo>();
                    ReportParameterInfoCollection parameterInfo = this.GetParameters();

                    foreach (var parameter in parameterInfo)
                    {
                        Syncfusion.Reports.Server.ReportParameterInfo paramInfo = new Syncfusion.Reports.Server.ReportParameterInfo();
                        paramInfo.Name = parameter.Name;

                        paramInfo.Labels = new string[parameter.Labels.Count];
                        paramInfo.Values = new string[parameter.Values.Count];

                        int index = 0;
                        foreach (var value in parameter.Values)
                        {
                            paramInfo.Values[index++] = value;
                        }

                        index = 0;

                        foreach (var label in parameter.Labels)
                        {
                            paramInfo.Labels[index++] = label;
                        }

                        parametersInfo.Add(paramInfo);
                    }

                    reportSetting.Parameters = parametersInfo.ToArray();
                    if (this.ProcessingMode == Viewer.ProcessingMode.Remote)
                    {

                        List<Syncfusion.Reports.Server.DataSourceCredentialsInfo> credentialsInfo = new List<Syncfusion.Reports.Server.DataSourceCredentialsInfo>();

                        if (this.ReportModel.Report.DataSources != null)
                        {
                            foreach (var dataSource in this.ReportModel.Report.DataSources)
                            {
                                DataSourceCredentialsInfo dataSourceInfo = new DataSourceCredentialsInfo();
                                dataSourceInfo.Name = dataSource.Name;
                                dataSourceInfo.UserId = dataSource.ConnectionProperties.UserName;
                                dataSourceInfo.Password = dataSource.ConnectionProperties.PassWord;
                                dataSourceInfo.IntegratedSecurity = dataSource.ConnectionProperties.IntegratedSecurity;
                                credentialsInfo.Add(dataSourceInfo);
                            }
                        }

                        reportSetting.DataSourceCredentials = credentialsInfo.ToArray();
                    }
                    else
                    {
                        List<Syncfusion.Reports.Server.ReportDataSource> listDatasourceinfo = new List<Syncfusion.Reports.Server.ReportDataSource>();
                        Dictionary<string, IEnumerable> datasourceCollection = this.ReportModel.ProcessedData.DataSourceObjects;

                        foreach (KeyValuePair<string, IEnumerable> DataSources in datasourceCollection)
                        {
                            Syncfusion.Reports.Server.ReportDataSource datasourceinfo = new Syncfusion.Reports.Server.ReportDataSource();
                            datasourceinfo.Name = DataSources.Key;
                            datasourceinfo.Value = this.GetWrapperDataSource(DataSources.Value).ToArray();
                            listDatasourceinfo.Add(datasourceinfo);
                        }
                        reportSetting.DataSources = listDatasourceinfo.ToArray();
                    }
                    this.ReportModel.ReportingServer.Export(reportSetting, writerFormat);
                }
            }
        }

#else
        void ExportToPdf()
        {
            if (this.IsExportable == true)
            {
                this.ExportingToPdf();
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        void ExportToExcel()
        {
            if (this.IsExportable == true)
            {
                this.ExportingToExcel();
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        void ExportToWord()
        {
            if (this.IsExportable == true)
            {
                this.ExportingToWord();
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        internal void ExportingToExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = this.GetExportFileName(),
                Filter = this.ExcelVersion == ExcelVersion.Excel97to2003 ? "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx" : "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls",
                FilterIndex = 0
            };

            this.Export(saveDialog, "Excel");
        }

        internal void ExportingToWord()
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = this.GetExportFileName(),
                Filter = "Word(*.doc)|*.doc|Word(*.docx)|*.docx",
                FilterIndex = 0
            };

            this.Export(saveDialog, "Word");
        }

        internal void ExportingToHtml()
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = this.GetExportFileName(),
                Filter = "Html(*.html)|*.html",
                FilterIndex = 0
            };

            this.Export(saveDialog, "Html");
        }


        internal void ExportingToPdf()
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = this.GetExportFileName(),
                Filter = "Pdf(*.pdf)|*.pdf",
            };

            this.Export(saveDialog, "PDF");
        }

        void Export(SaveFileDialog dialog, string writerFormat)
        {
#if SyncfusionFramework3_5
            byte[] streamBytes = ExportByte(writerFormat);
#endif
            if (dialog.ShowDialog() == true)
            {
                string url = dialog.FileName;

                if (url != string.Empty)
                {
#if !SyncfusionFramework3_5
                    byte[] streamBytes = ExportByte(writerFormat);
#endif
                    Stream stream = dialog.OpenFile();
                    stream.Write(streamBytes, 0, streamBytes.Length);
                    stream.Close();
                }
            }
        }

        byte[] ExportByte(string writerFormat)
        {
            MemoryStream exportStream = new MemoryStream();
            ReportWriter.WriterFormat format = (WriterFormat)Enum.Parse(typeof(WriterFormat), writerFormat, true);
            ReportWriter.ReportWriter reportWriter = new ReportWriter.ReportWriter();
            reportWriter.ExcelVersion = (ReportWriter.ExcelVersion)Enum.Parse(typeof(ReportWriter.ExcelVersion), this.ExcelVersion.ToString());
            reportWriter.WordFormatType=(ReportWriter.WordFormatType)Enum.Parse(typeof(ReportWriter.WordFormatType),this.WordFormatType.ToString());
            this.ReportModel.IsEvaluatedReport = true;
            reportWriter.ReportModel = this.ReportModel;
            reportWriter.Save(exportStream, format);
            return exportStream.ToArray();
        }
#endif

        #endregion

#if !SILVERLIGHT

        #region Export ReportDefinition to XPS

        /// <summary>
        /// Exports the currently rendered report.
        /// </summary>
        /// <remarks>Currently this will supports exporting the report as XML Paper Specification format (XPS)</remarks>
        void ExportToXps()
        {
            if (this.IsExportable == true)
            {
                this.SaveToXps();
            }
            else
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveReportError"),
                    Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(CultureInfo.CurrentUICulture, "titleReportViewer"), MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Saves to XPS.
        /// </summary>
        void SaveToXps()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = this.GetExportFileName();
            dlg.DefaultExt = ".xps"; // Default file extension 
            dlg.Filter = "XPS Document (.xps)|*.xps"; // Filter files by extension // Show save file dialog box 

            if (dlg.ShowDialog() == true)
            {

                var package = Package.Open(dlg.FileName, FileMode.Create);
                var xpsDoc = new XpsDocument(package, CompressionOption.Normal);
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                // xps documents are built using fixed document sequences
                var fixedDocSeq = new FixedDocumentSequence();
                var docRef = new DocumentReference();
                docRef.BeginInit();
                docRef.SetDocument(this.GetFixedDocument());
                docRef.EndInit();
                ((IAddChild)fixedDocSeq).AddChild(docRef);

                // write out our fixed document to xps
                xpsWriter.Write(fixedDocSeq.DocumentPaginator);

                xpsDoc.Close();
                package.Close();

            }
        }

        /// <summary>
        /// Return FixedDocument
        /// </summary>
        public FixedDocument GetFixedDocument()
        {
            this.isExporting = true;
            FixedDocument document = new FixedDocument();
            StackPanel viewerControl = this.PageView;
            PrintDialog printDialog = new System.Windows.Controls.PrintDialog();

            if (pageModelFactory != null)
            {
                int printPageCount = pageModelFactory.PrintLayoutPageDictionary.Count;
                int currentPage = this.Current;
                int selectedZoom = this.comboBoxPageZoom.SelectedIndex;
                this.comboBoxPageZoom.SelectedIndex = 3;

                if (this.ViewMode == Viewer.ViewMode.Normal)
                {
                    SetPrintLayout();
                }

                for (int i = 0; i < pageModelFactory.PrintLayoutPageDictionary.Count; i++)
                {
                    this.Current = i;
                    viewerControl = GetVisual(i);
                    PrintCapabilities m_PrintCapabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                    Size controlSize = new Size(this.PaperWidth, this.PaperHeight);
                    viewerControl.Measure(controlSize);
                    viewerControl.Arrange(new Rect(new Point(0, 0), controlSize));
                    //Capture the image of the visual in the same size as Printing page.  

                    int dpiwidth = (int)((viewerControl.ActualWidth * this.PrintdpiX) / 96);
                    int dpiheight = (int)((viewerControl.ActualHeight * this.PrintdpiY) / 96);

                    RenderTargetBitmap bmp = new RenderTargetBitmap(dpiwidth, dpiheight, this.printDpiX, this.printDpiY, PixelFormats.Pbgra32);
                    bmp.Render(viewerControl);
                    DrawingVisual pageVisual = new DrawingVisual();
                    DrawingContext drawingContext = pageVisual.RenderOpen();
                    drawingContext.PushTransform(new TranslateTransform(0, 0));
                    drawingContext.DrawImage(bmp, new System.Windows.Rect(new Size(this.PaperWidth, this.PaperHeight)));
                    drawingContext.Close();
                    PageContent m_PageContent = new PageContent();
                    FixedPage page = new FixedPage();
                    page.Width = this.PaperWidth;
                    page.Height = this.PaperHeight;
                    VisualContainer myContainer = new VisualContainer();
                    myContainer.AddVisual(pageVisual);
                    page.Children.Add(myContainer);
                    ((IAddChild)m_PageContent).AddChild(page);
                    document.Pages.Add(m_PageContent);
                }

                if (this.ViewMode == Viewer.ViewMode.Normal)
                {
                    ReSetPrintLayout();
                }

                this.isExporting = false;
                this.Current = currentPage;
                this.comboBoxPageZoom.SelectedIndex = selectedZoom;
            }
            return document;
        }

        /// <summary>
        /// Saves the visuals.
        /// </summary>
        /// <param name="xpsdw">The XPSDW.</param>
        /// <param name="v">The v.</param>
        /// <param name="visualToXpsDocument">The visual to XPS document.</param>
        private void SaveVisuals(XpsDocumentWriter xpsdw, Visual v, VisualsToXpsDocument visualToXpsDocument)
        {
            visualToXpsDocument.Write(v);
        }

        #endregion

#endif
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ExceptionDetail exceptionWindow = new ExceptionDetail(this.errorMessage);

#if ! SILVERLIGHT
            exceptionWindow.Owner = Window.GetWindow(this);
#endif
            exceptionWindow.ShowDialog();
        }

        #endregion

        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.m_current = this.ReportModel.LastPageIndex;
            if (this.ReportModel.Model != null)
            {
                if (this.ReportModel.Model.ParentModel != null)
                {
                    this.ReportModel = this.ReportModel.Model.ParentModel;
                    if (this.ReportModel.Model == null)
                    {
                        this.btnback.Visibility = Visibility.Collapsed;
                    }
                    this.pageModelFactory = this.ReportModel.PageModelFactory;

                    var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                           where reportParameter.Hidden == false
                                           select reportParameter;

                    this.HasReportParameters = reportParameters.Count() > 0;
                    this.buttonParameters.Visibility = this.HasReportParameters ? Visibility.Visible : Visibility.Collapsed;
                    this.AddReportParameters();
                    this.UpdateDataSetParameterValues();
                    if (this.pageModelFactory != null)
                    {
                        if (this.ViewMode == ViewMode.Print)
                        {
                            this.PrintView();
                            this.UpdatePages();
                        }
                        this.m_totalPages = pageModelFactory.PageDictionary.Count;
                        this.textBoxTotalPages.Text = this.m_totalPages.ToString();
                        this.UpdateCurrentPage();
                    }
                }
            }
            else
            {
                this.btnback.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonParameters_OnClick(object sender, RoutedEventArgs e)
        {
            this.ShowParametersBlock = !this.ShowParametersBlock;
        }
    }

#if !SILVERLIGHT
    class VisualContainer : FrameworkElement
    {
        private readonly VisualCollection children;
        public VisualContainer()
        {
            children = new VisualCollection(this);
        }

        public void AddVisual(Visual v)
        {
            children.Add(v);
        }
        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }
        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }
    }
#endif

    #region Resource helper
    class ResourceFinder
    {

        public ResourceFinder()
        {
        }
        internal ImageSource FindResource(UserControl control, string key)
        {
            ImageSource sour = control.Resources[key] as ImageSource;
#if SILVERLIGHT
            BitmapImage imag = sour as BitmapImage;
            if (imag != null)
            {
                string loc = imag.UriSource.ToString();
                string assembly = "Syncfusion.ReportViewer.Silverlight;component";
                Uri uri = new Uri(assembly + loc, UriKind.Relative);
                StreamResourceInfo sri = Application.GetResourceStream(uri);
                BitmapImage bi = new BitmapImage();
                bi.SetSource(sri.Stream);
                return bi;
            }
#endif
            return sour;
        }


        internal void SetResourceImage(UserControl control, string key, Button button)
        {
            try
            {
                Image image = new Image();
                image.Source = this.FindResource(control, key);
                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Center;
                image.Stretch = Stretch.Fill;
                image.Height = 16;
                image.Width = 16;
                button.Content = image;
            }
            catch
            {
            }
        }

        internal void SetResourceImage(UserControl control, string key, System.Windows.Controls.Primitives.ToggleButton button)
        {
            Image image = new Image();
            image.Source = this.FindResource(control, key);
            button.Content = image;
        }
    }
    #endregion

    #region Event Delegates

    public delegate void NavigationButtonVisibilityChangedEventHandler(object sender, NavigationButtonVisibilityChangedEventArgs e);

    public delegate void ViewModeChangedEventHandler(object sender, ViewModeChangedEventArgs e);

    public delegate void RenderingBeginEventHandler(object sender, EventArgs e);

    public delegate void ExportByteCompletedEventHandler(object sender, byte[] e);

    public delegate void RefreshEventHandler(object sender, EventArgs e);

    public delegate void ReportErrorHandler(object sender, ReportErrorEventArgs e);

    public delegate void RenderingCompletedEventHandler(object sender, EventArgs e);

    #endregion

    #region Event Class Declaration

    /// <summary>
    /// Event for Updating the navigational button status.
    /// </summary>
    public class NavigationButtonVisibilityChangedEventArgs
        : System.EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationButtonVisibilityChangedEventArgs"/> class.
        /// </summary>
        /// <param name="first">if set to <c>true</c> [first].</param>
        /// <param name="previous">if set to <c>true</c> [previous].</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <param name="last">if set to <c>true</c> [last].</param>
        public NavigationButtonVisibilityChangedEventArgs(bool first, bool previous, bool next, bool last)
        {
            this.IsFirstVisible = first;
            this.IsPreviousVisible = previous;
            this.IsNextVisible = next;
            this.IsLastVisible = last;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is next visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is next visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsNextVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is previous visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is previous visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsPreviousVisible { get; set; }

        #endregion
    }

    /// <summary>
    /// Event for notifying report view toggled back to normal view from print view.
    /// </summary>
    public class ViewModeChangedEventArgs
        : System.EventArgs
    {
        private string _previousmode;
        private string _currentmode;
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModeChangedEventArgs"/> class.
        /// </summary>
        public ViewModeChangedEventArgs()
        {
        }
        public string PreviousMode
        {
            get { return _previousmode; }
            set { _previousmode = value; }
        }
        public string CurrentMode
        {
            get { return _currentmode; }
            set { _currentmode = value; }
        }
        #endregion
    }

    public class ReportErrorEventArgs
        : System.EventArgs
    {
        public string Message
        {
            get;
            set;
        }
    }

    #endregion

    #region ExportOptions

#if SILVERLIGHT

    public class ExportOption
    {
        public ImageSource ExportImage { get; set; }
        public ExportType Export { get; set; }
        public Visibility Visibility { get; set; }
    }

    public enum ExportType
    {
        PDF,
        Excel,
        Word,
        Html
    }

#else

    internal class ExportOption
    {
        public ImageSource ExportImage { get; set; }
        public ExportType Export { get; set; }
        public Visibility Visibility { get; set; }
    }

    internal enum ExportType
    {
        PDF,
        XPS,
        Excel,
        Word,
        Html
    }

    public enum ExportFormat
    {
        PDF,
        Word,
        Excel,
        Html
    }

    public enum ExcelVersion
    {
        // Summary:
        //     Represents excel version 97-2003.
        Excel97to2003 = 0,
        Excel2007 = 1,
        Excel2010 = 2,
        Excel2013 = 3,
    }

    public enum WordFormatType
    {
        // Summary:
        //     Microsoft Word file format.
        Doc = 0,
        //
        // Summary:
        //     Microsoft Word document template
        Dot = 1,
        //
        // Summary:
        //     Microsoft Word 2007 file format.
        Docx = 2,
        //
        // Summary:
        //     Microsoft Word 2007 file format.
        Word2007 = 3,
        //
        // Summary:
        //     Microsoft Word 2010 file format.
        Word2010 = 4,
        //
        // Summary:
        //     Microsoft Word 2013 file format.
        Word2013 = 5,
        //
        // Summary:
        //     Microsoft Word 2007 Template format.
        Word2007Dotx = 6,
        //
        // Summary:
        //     Microsoft Word 2010 Template format.
        Word2010Dotx = 7,
        //
        // Summary:
        //     Microsoft Word 2013 Template format.
        Word2013Dotx = 8,
        //
        // Summary:
        //     Microsoft Word 2007 macro enabled file format.
        Word2007Docm = 9,
        //
        // Summary:
        //     Microsoft Word 2010 macro enabled file format.
        Word2010Docm = 10,
        //
        // Summary:
        //     Microsoft Word 2013 macro enabled file format.
        Word2013Docm = 11,
        //
        // Summary:
        //     Microsoft Word 2007 macro enabled template format.
        Word2007Dotm = 12,
        //
        // Summary:
        //     Microsoft Word 2010 macro enabled template format.
        Word2010Dotm = 13,
        //
        // Summary:
        //     Microsoft Word 2013 macro enabled template format.
        Word2013Dotm = 14,
        //
        // Summary:
        //     Rtf format
        Rtf = 15,
        //
        // Summary:
        //     Text file format.
        Txt = 16,
        //
        // Summary:
        //     E-book format.
        EPub = 17,
        //
        // Summary:
        //     Html format.
        Html = 18,
        //
        // Summary:
        //     Xml file format.
        Xml = 19,
        //
        // Summary:
        //     Support all Format Types.
        Automatic = 20,
    }



#endif

    #endregion

    #region Toolbar Modes

    /// <summary>
    /// Represents different modes of toolbar.
    /// </summary>
    public enum ToolBarMode
    {
        /// <summary>
        /// Default. It displays all the buttons in the toolbar.
        /// </summary>
        Default,

        /// <summary>
        /// Mini. Mini toolbar mode allows to view toolbar with four navigation arrows namely First, Previous, Next and Last along with current page number and total pages.
        /// </summary>
        Navigation,

        /// <summary>
        /// Micro. Micro toolbar mode allows to view toolbar with two navigation arrows namely previous and next.
        /// </summary>
        FullNavigation,
    }

    #endregion

    #region Report Rendering Modes
    /// <summary>
    /// Represents different view mode of Report Viewer.
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// Default. It will display the reports in Normal Mode.
        /// </summary>
        Normal,
        /// <summary>
        /// Default. It will display the reports in Print Mode.
        /// </summary>
        Print
    }

    #endregion

    #region ProcessingMode for ReportViewer

    public enum ProcessingMode
    {
        Remote,
        Local
    }

#if SILVERLIGHT
    public enum ExportMode
    {
        Server,
        Local
    }
#endif

    #endregion

    #region PaperOrientation for ReportViewer

    public enum PaperOrientation
    {
        Portrait,
        Landscape
    }

    #endregion

    #region RecordCallBackActions

    class CallBackActions
    {
        private Action<DependencyObject, DependencyPropertyChangedEventArgs> action;
        private Action memberCall;
        DependencyObject dependencyObject;
        DependencyPropertyChangedEventArgs argument;

        public CallBackActions(Action<DependencyObject, DependencyPropertyChangedEventArgs> action, DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.action = action;
            this.dependencyObject = d;
            this.argument = e;
        }

        public CallBackActions(Action action)
        {
            this.memberCall = action;
        }

        private Action<DependencyObject, DependencyPropertyChangedEventArgs> Action
        {
            get { return this.action; }
        }

        private Action MemberFunction
        {
            get { return this.memberCall; }
        }
        public void Execute()
        {
            if (this.MemberFunction != null)
            {
                this.MemberFunction.Invoke();
            }
            else
            {
                this.Action.Invoke(this.dependencyObject, this.argument);
            }
        }
    }

#endregion
}