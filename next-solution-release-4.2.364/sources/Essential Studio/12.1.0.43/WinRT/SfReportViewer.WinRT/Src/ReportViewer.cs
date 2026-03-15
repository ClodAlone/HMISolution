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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Controls.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Shape = Windows.UI.Xaml.Shapes;
using Windows.UI.Core;
using Windows.System;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Net;
using Syncfusion.RDL.Data;
using Syncfusion.UI.Xaml.Reports;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Syncfusion.RDL.ItemModel;
using System.Collections;
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Printing;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Syncfusion.ReportWriter;
namespace Syncfusion.UI.Xaml.Reports
{
    public class SfReportViewer : Control
    {
        private ReportDataSourceCollection _DataSources;
        private Grid gridLoadingIndicator = null;
        private RowDefinition loadingIndicatorRow = null;
        private ScrollViewer scrollViewer = null;
        private StackPanel PageView;
        private RowDefinition viewerContentRow = null;
        private PageModelFactory pageModelFactory = null;
        private Button ExportButton = null;
        private Button PrintButton = null;
        private Button DocIO = null;
        private Button Html = null;
        private Button PDF = null;
        private Button Excel = null;
        private Border PageViewBody = null;

        private Canvas canvasContentPage = null;
        private Canvas CanvasHeader = null;
        private Canvas CanvasFooter = null;
        private Border PageHeaderBorder = null;
        private Border PageBodyBorder = null;
        private Border PageFooterBorder = null;

        private Button NextButton = null;
        private Button PreviousButton = null;
        private Button FirstButton = null;
        private Button LastButton = null;
        private Button BackButton = null;
        private Button RefreshButton = null;
        private Button DocumentMapButton = null;
        private Grid grid_ReportParameterBlock = null;
        private Border ParameterblockBorder = null;
        private FileSavePicker fileSavePicker = null;
        private StorageFile savedItem = null;
        private Button ViewReport = null;
        private Button ParameterCancel = null;
        private TextBlock PageNumber = null;
        private TextBlock TotalPages = null;
        private Button NextPageView = null;
        private Button PreviousPageView = null;
        private Grid gridException = null;
        private RowDefinition gridExceptionRow = null;
        private TextBlock textBlockException = null;
        private ScrollViewer groupBoxExpandedExceptionScroll = null;
        private Grid groupBoxExpandedException = null;
        private TextBlock textBlockStackTrace = null;
        private ToggleButton toggleShowDetails = null;
        private UserControl ShowError = null;
        private Grid NavigationPanelGrid = null;
        private Grid NavigationBar = null;
        private Grid crederror = null;
        private Grid credaccept = null;
        private Button CredViewreport = null;

        private Button Parameter = null;
        DispatcherTimer timer = new DispatcherTimer();
        private Popup NextPopup = null;
        private Popup PreviousPopup = null;
        internal Popup ExportBlock = null;
        internal Popup ParameterBlock = null;
        private Popup PageInfoBlock = null;
        internal Popup DocumentMapBlock = null;
        private ContentControl Dialog = null;
        private RowDefinition Credblock = null;
        private Button CredNext = null;
        private TextBox Username = null;
        private PasswordBox Password = null;
        private TextBlock credDatasourceName = null;

        private SfTreeNavigator TreeNavigator = null;

        internal int m_current = 0;
        internal int m_totalPages = 0;
        private int credentialDSCount = 0;

        private bool textBoxValueChanged = false;
        private bool internalValueChange = false;
        private bool hasPageFooter;
        private bool hasPageHeader;
        private string stackTrace = null;
        private string errorMessage = null;
        private bool? rendered = null;
        private IPrintDocumentSource printDocumentSource = null;
        private PrintDocument printDocument = null;
        //private bool isRegisterforPrint = false;

        private bool isDrillReport = false;

        private bool isPageInfo = false;

        private Stream loadStream = null;

        private List<Syncfusion.RDL.DOM.DataSource> CredentailDataSource = null;


        /// <summary>
        /// Occurs when report loaded for ReportViewer.
        /// </summary>
        public event ReportLoadedEventHandler ReportLoaded;

        /// <summary>
        /// Occurs when subreport loaded in Local processing mode
        /// </summary>
        public event SubreportProcessingEventHandler SubreportProcessing;

        /// <summary>
        /// Occurs when reporting rendering started with ReportViewer.
        /// </summary>
        public event RenderingBeginEventHandler RenderingBegin;

        /// <summary>
        /// Occurs when reporting rendering completed with ReportViewer.
        /// </summary>
        public event RenderingCompletedEventHandler RenderingCompleted;

        /// <summary>
        /// Occurs when occur error with Reportloading.
        /// </summary>
        public event ReportErrorEventHandler ReportError;


        /// <summary>
        /// When ReportParameter Cata
        /// </summary>
        public event ViewButtonClickHandler ViewButtonClick;

        public event RefreshEventHandler ReportRefresh;


        #region Properties

        /// <summary>
        /// Identifies the <see cref="Syncfusion.UI.Xaml.Reports.SfReportViewer.ReportPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportPathProperty =
        DependencyProperty.Register("ReportPath", typeof(string), typeof(SfReportViewer), new PropertyMetadata(string.Empty));//ReportPathChanged

        /// <summary>
        /// Identifies the <see cref="Syncfusion.UI.Xaml.Reports.SfReportViewer.ReportServerUrl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerUrlProperty =
        DependencyProperty.Register("ReportServerUrl", typeof(string), typeof(SfReportViewer), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.UI.Xaml.Reports.SfReportViewer.ReportServerCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerCredentialProperty =
        DependencyProperty.Register("ReportServerCredential", typeof(ICredentials), typeof(SfReportViewer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.UI.Xaml.Reports.SfReportViewer.ReportServerFormsCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerFormsCredentialProperty =
        DependencyProperty.Register("ReportServerFormsCredential", typeof(ReportServerFormsCredential), typeof(SfReportViewer), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="Syncfusion.UI.Xaml.Reports.SfReportViewer.ViewMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessingModeProperty =
        DependencyProperty.Register("ProcessingMode", typeof(ProcessingMode), typeof(SfReportViewer), new PropertyMetadata(ProcessingMode.Remote));

        public static readonly DependencyProperty EnableVirtualEvaluationProperty =
            DependencyProperty.Register("EnableVirtualEvaluation", typeof(bool), typeof(SfReportViewer), new PropertyMetadata(false, null));


        /// <summary>
        /// Identifies the <see cref="Syncfusion.UI.Xaml.Reports.SfReportViewer.HasReportParameters"/> dependency property. 
        /// </summary>
        static readonly DependencyProperty HasReportParametersProperty =
            DependencyProperty.Register("HasReportParameters", typeof(bool), typeof(SfReportViewer), new PropertyMetadata(false, OnHasReportParametersPropertyChanged));

        public static readonly DependencyProperty ShowNavigationBarProperty =
           DependencyProperty.Register("ShowNavigationBar", typeof(bool), typeof(SfReportViewer), new PropertyMetadata(true, OnShowNavigationBarPropertyChanged));


        static void OnShowNavigationBarPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SfReportViewer reportViewer = dependencyObject as SfReportViewer;

            if (reportViewer != null)
            {
                if (reportViewer != null && reportViewer.NavigationBar != null)
                {
                    bool value = (bool)e.NewValue;
                    if (value == false)
                    {
                        reportViewer.NavigationPanelGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        reportViewer.NavigationPanelGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        //<summary>
        //Called when [has report parameters property changed].
        //</summary>
        //<param name="dependencyObject">The dependency object.</param>
        //<param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnHasReportParametersPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SfReportViewer reportViewer = dependencyObject as SfReportViewer;
            //  reportViewer.buttonParameters.Visibility = Visibility.Collapsed;

            if (reportViewer != null)
            {
                //  reportViewer.ParametersBlock(!(bool)e.NewValue);

                if (!(bool)e.OldValue && (bool)e.NewValue)
                {
                    //  reportViewer.buttonParameters.Visibility = Visibility.Visible;
                    reportViewer.AddReportParameters();
                    reportViewer.UpdateDataSetParameterValues();
                }
            }
        }


        /// <summary>
        /// Gets or sets RDL(C) report path. This initializes the report viewer.
        /// </summary>
        /// <value>The report path.</value>
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
        /// Set the toolbar visibility
        /// </summary>
        /// <value>The toolbar.</value>
        public bool ShowNavigationBar
        {
            get
            {
                return (bool)this.GetValue(ShowNavigationBarProperty);
            }
            set
            {
                if (this.ShowNavigationBar != value)
                {
                    this.SetValue(ShowNavigationBarProperty, value);
                }
            }
        }


        /// <summary>
        /// Gets the data sources.
        /// </summary>
        /// <value>The data sources.</value>
        public ReportDataSourceCollection DataSources
        {
            get
            {
                if (this._DataSources == null)
                {
                    this._DataSources = new ReportDataSourceCollection();
                }

                return this._DataSources;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the reportItem Evaluation Mode.
        /// </summary>
        public bool EnableVirtualEvaluation
        {
            get { return (bool)GetValue(EnableVirtualEvaluationProperty); }
            set { SetValue(EnableVirtualEvaluationProperty, value); }
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
        /// <value>The Mode for Export.</value>
        [DefaultValue(ExportMode.Local)]
        public ExportMode ExportMode
        {
            get;
            set;
        }


        public bool LoadCredentialsInformationinServer
        {
            get;
            set;
        }

        #endregion

        #region Internal helper properties

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
                    UpdateCurrentPage();
                }
            }
        }

        internal bool HasReportParameters
        {
            get { return (bool)GetValue(HasReportParametersProperty); }
            set { SetValue(HasReportParametersProperty, value); }
        }

        internal double MarginLeft
        {
            get;
            set;
        }

        internal double MarginRight
        {
            get;
            set;
        }

        internal double MarginTop
        {
            get;
            set;
        }

        internal double MarginBottom
        {
            get;
            set;
        }

        internal double PaperWidth
        {
            get;
            set;
        }

        internal double PaperHeight
        {
            get;
            set;
        }


        #endregion

        #region Navigation Helper Methods

        internal void UpdateCurrentPage()
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

                    this.DrawBodyControls(pageModelFactory.PageDictionary[this.Current].ReportModelCollection);

                    this.PageViewBody.Margin = new Thickness(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);
                    this.PageView.Height = this.PaperHeight;
                    this.PageView.Width = this.PaperWidth;
                    double pageHeight = this.PaperHeight - this.MarginTop - this.MarginBottom - pageModelFactory.FooterHeight - pageModelFactory.HeaderHeight;
                    double pageWidth = this.PaperWidth - this.MarginLeft - this.MarginRight;
                    this.CanvasHeader.Width = pageWidth;
                    this.CanvasFooter.Width = pageWidth;
                    this.canvasContentPage.Height = pageHeight;
                    this.canvasContentPage.Width = pageWidth;

                    this.UpdatePageDetails(this.Current);

                    this.GetHeaderFooter(this.CanvasHeader, this.CanvasFooter);

                    this.ApplyStyle(this.ReportModel.FooterBehaviour, this.CanvasFooter);
                    this.ApplyStyle(this.ReportModel.FooterBehaviour, this.CanvasHeader);
                    this.ApplyStyle(this.ReportModel.BodyBehaviour, this.canvasContentPage);

                    this.scrollViewer.ScrollToHorizontalOffset(0);
                    this.scrollViewer.ScrollToVerticalOffset(0);

                    this.UpdateExceptionDetails();
                    if (this.rendered != null && this.rendered == false)
                    {
                        this.rendered = true;
                        this.RefreshButton.IsEnabled = true;
                        this.RaiseRenderingCompletedEvent();
                    }
                }
            }
            catch
            {

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates the page view header.
        /// </summary>
        private void CreatePageViewHeader(Canvas header)
        {
            if (this.hasPageHeader)
            {
                this.PageHeaderBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
                header.Visibility = Visibility.Visible;

                //// Height of the Page header
                header.Height = this.pageModelFactory.HeaderHeight;

                this.DrawHeaderControls(this.ReportModel.HeaderReportItemModels, header);
                this.SetBuiltInFunctions(this.Current + 1, this.pageModelFactory.PageCount, header);
            }
            else
            {
                this.PageHeaderBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                header.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

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
                        imageBrush.Stretch = Stretch.UniformToFill;
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
                this.PageFooterBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
                footer.Visibility = Visibility.Visible;
                //// Height of the Page footer            
                footer.Height = this.pageModelFactory.FooterHeight;

                this.DrawHeaderControls(this.pageModelFactory.ReportFooterModelerLists, footer);

                this.SetBuiltInFunctions(this.Current + 1, this.pageModelFactory.PageCount, footer);
            }
            else
            {
                this.PageFooterBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                footer.Visibility = Visibility.Collapsed;
            }
        }

        private void SetBuiltInFunctions(int pageLayoutPagesCount, int totalPages, Canvas canvasPageHeader)
        {
            try
            {
                //// Currently we are supporting text box alone. In future, we will add rectangel, etc.,
                foreach (FrameworkElement frameworkElement in canvasPageHeader.Children)
                {
                    if (frameworkElement is ReportingTextbox)
                    {
                        ReportingTextbox rich = frameworkElement as ReportingTextbox;
                        SetRichTextBoxValue(pageLayoutPagesCount, totalPages, rich);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetRichTextBoxValue(int pageLayoutPagesCount, int totalPages, ReportingTextbox rich)
        {
            foreach (Windows.UI.Xaml.Documents.Block prg in rich.InternalTextBox.Blocks)
            {
                foreach (Run textRun in (prg as Paragraph).Inlines.ToList())
                {
                    if (textRun.Text.Contains("Globals.PageNumber"))
                    {
                        textRun.Text = textRun.Text.Replace("Globals.PageNumber", pageLayoutPagesCount.ToString());
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

        /// <summary>
        /// Move to the specified page number in the report
        /// </summary>
        /// <param name="pageNo">Page Number Want to Go</param>
        public void GoTo(int pageNo)
        {
            this.Current = pageNo - 1;
        }


        #endregion
        public SfReportViewer()
        {
            this.DefaultStyleKey = typeof(SfReportViewer);
        }

        protected override void OnApplyTemplate()
        {
            this.scrollViewer = GetTemplateChild("scrollViewer") as ScrollViewer;
            this.gridLoadingIndicator = GetTemplateChild("gridLoadingIndicator") as Grid;
            this.gridException = GetTemplateChild("gridException") as Grid;
            this.gridExceptionRow = GetTemplateChild("gridExceptionRow") as RowDefinition;
            this.viewerContentRow = GetTemplateChild("viewerContentRow") as RowDefinition;
            this.loadingIndicatorRow = GetTemplateChild("loadingIndicatorRow") as RowDefinition;

            this.PageViewBody = GetTemplateChild("PageViewBody") as Border;
            this.CanvasHeader = GetTemplateChild("CanvasHeader") as Canvas;
            this.CanvasFooter = GetTemplateChild("CanvasFooter") as Canvas;
            this.canvasContentPage = GetTemplateChild("canvasContentPage") as Canvas;
            this.PageHeaderBorder = GetTemplateChild("PageHeaderBorder") as Border; ;
            this.PageBodyBorder = GetTemplateChild("PageBodyBorder") as Border; ;
            this.PageFooterBorder = GetTemplateChild("PageFooterBorder") as Border;

            this.PageView = GetTemplateChild("PageView") as StackPanel;
            this.grid_ReportParameterBlock = GetTemplateChild("grid_ReportParameterBlock") as Grid;
            this.ParameterblockBorder = GetTemplateChild("ParameterblockBorder") as Border;
            this.ParameterCancel = GetTemplateChild("ParameterCancel") as Button;
            this.ViewReport = GetTemplateChild("ViewReport") as Button;
            this.PageNumber = GetTemplateChild("PageNumber") as TextBlock;
            this.TotalPages = GetTemplateChild("TotalPages") as TextBlock;
            this.NextPageView = GetTemplateChild("NextPageView") as Button;
            this.PreviousPageView = GetTemplateChild("PreviousPageView") as Button;
            this.PrintButton = GetTemplateChild("Print") as Button;
            this.textBlockException = GetTemplateChild("textBlockException") as TextBlock;
            this.groupBoxExpandedExceptionScroll = GetTemplateChild("groupBoxExpandedExceptionScroll") as ScrollViewer;
            this.groupBoxExpandedException = GetTemplateChild("groupBoxExpandedException") as Grid;
            this.textBlockStackTrace = GetTemplateChild("textBlockStackTrace") as TextBlock;
            this.toggleShowDetails = GetTemplateChild("toggleShowDetails") as ToggleButton;
            this.ShowError = GetTemplateChild("ShowError") as UserControl;
            this.NavigationPanelGrid = GetTemplateChild("NavigationPanelGrid") as Grid;
            this.NavigationBar = GetTemplateChild("NavigationBar") as Grid;
            this.Dialog = GetTemplateChild("Dialog") as ContentControl;
            this.Credblock = GetTemplateChild("Credblock") as RowDefinition;
            this.CredNext = GetTemplateChild("CredNext") as Button;
            this.Username = GetTemplateChild("Username") as TextBox;
            this.Password = GetTemplateChild("Password") as PasswordBox;
            this.crederror = GetTemplateChild("crederror") as Grid;
            this.credaccept = GetTemplateChild("credaccept") as Grid;
            this.CredViewreport = GetTemplateChild("CredViewreport") as Button;
            this.credDatasourceName = GetTemplateChild("credDatasourceName") as TextBlock;

            this.TreeNavigator = GetTemplateChild("DocumentMapTree") as SfTreeNavigator;

            this.ExportBlock = GetTemplateChild("ExportBlock") as Popup;
            this.ParameterBlock = GetTemplateChild("ParameterBlock") as Popup;
            this.PageInfoBlock = GetTemplateChild("PageInfoBlock") as Popup;
            this.PreviousPopup = GetTemplateChild("PreviousPopup") as Popup;
            this.NextPopup = GetTemplateChild("NextPopup") as Popup;
            this.DocumentMapBlock = GetTemplateChild("DocumentMapBlock") as Popup;
            this.UpdateReportingToolBarControls();
            this.DocIO = GetTemplateChild("DocIO") as Button;
            this.PDF = GetTemplateChild("PDF") as Button;
            this.Excel = GetTemplateChild("Excel") as Button;
            this.Html = GetTemplateChild("Html") as Button;
            this.ParameterblockBorder.SizeChanged += ParameterblockBorder_SizeChanged;
            this.ViewReport.Click += ViewReport_Click;
            this.ParameterCancel.Click += ParameterCancel_Click;

            this.ParameterBlock.Opened += ParameterBlock_Opened;
            this.NextPageView.Click += NextPageView_Click;
            this.PreviousPageView.Click += PreviousPageView_Click;
            this.toggleShowDetails.Click += toggleShowDetails_Click;
            this.scrollViewer.ViewChanged += scrollViewer_ViewChanged;
            this.DocIO.Click += ExportButton_Click;
            this.PDF.Click += ExportButton_Click;
            this.Html.Click += ExportButton_Click;
            this.Excel.Click += ExportButton_Click;
            this.CredNext.Click += CredNext_Click;
            this.CredViewreport.Click += ViewReport_Click;
            this.PrintButton.Click += PrintButton_Click;


            if (!this.ShowNavigationBar)
            {
                this.NavigationPanelGrid.Visibility = Visibility.Collapsed;
            }

            base.OnApplyTemplate();
        }

        void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            this.PrintReport();
        }


        void CredNext_Click(object sender, RoutedEventArgs e)
        {
            if (CredentailDataSource.Count != credentialDSCount)
            {
                CredentailDataSource[credentialDSCount].ConnectionProperties.UserName = Username.Text;
                CredentailDataSource[credentialDSCount].ConnectionProperties.PassWord = Password.Password;
                credaccept.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                crederror.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ValidateConnection();
            }

        }

        void ValidateConnection()
        {
            if (CredentailDataSource.Count != credentialDSCount)
            {
                ReportingConnectionEventArgs arg = new ReportingConnectionEventArgs();
                this.ReportModel.ConnectionValidated += new ConnectionValidatedEventHandler(ReportModel_ConnectionValidated);
                this.ReportModel.ValidateConnection(this.GetConnectionArgs(CredentailDataSource[credentialDSCount]));
            }
        }

        ReportingConnectionEventArgs GetConnectionArgs(Syncfusion.RDL.DOM.DataSource datasource)
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
                if (!e.Success)
                {
                    crederror.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    credaccept.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                else
                {
                    credentialDSCount++;
                    crederror.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    if (CredentailDataSource.Count != credentialDSCount)
                    {
                        Username.Text = "";
                        Password.Password = "";
                        this.credDatasourceName.Text =  this.CredentailDataSource[credentialDSCount].ConnectionProperties.Prompt;
                    }

                    if (CredentailDataSource.Count == credentialDSCount)
                    {
                        credaccept.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        this.CredNext.IsEnabled = false;
                        this.CredViewreport.IsEnabled = true;
                    }
                }
            }
            catch { }
        }

        void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            this.PageInfoBlock.IsOpen = false;
        }

        void toggleShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (this.toggleShowDetails.IsChecked == true)
            {
                this.ShowError.Visibility = Visibility.Visible;
                this.textBlockStackTrace.Margin = new Thickness(5, 0, -5, 0);
                foreach (string exception in this.ReportModel.ExceptionDetails)
                {
                    this.textBlockStackTrace.Text += exception;
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

        void PreviousPageView_Click(object sender, RoutedEventArgs e)
        {
            this.MovePrevious();
        }

        void NextPageView_Click(object sender, RoutedEventArgs e)
        {
            this.MoveNext();
        }

        void ParameterCancel_Click(object sender, RoutedEventArgs e)
        {
            this.ParameterBlock.IsOpen = false;
        }

        void ViewReport_Click(object sender, RoutedEventArgs e)
        {
            ViewButtonClickEventArgs args = new ViewButtonClickEventArgs();

            if (ViewButtonClick != null)
            {
                ViewButtonClick(this, args);
            }

            if (!args.Cancel)
            {
                this.DisableButton();
                this.UpdateReport();
                this.ShowCredentialBlock(false);
                this.ParameterBlock.IsOpen = false;
            }
        }

        void ParameterBlock_Opened(object sender, object e)
        {

        }

        void ParameterblockBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ParameterblockBorder.Height = this.ActualHeight;
        }

        void Export_Click(object sender, RoutedEventArgs e)
        {
            this.ExportBlock.IsOpen = true;
        }

        void DocumentMapTree()
        {
            if (this.ReportModel.MapModel != null && this.ReportModel.MapModel.NodeData.Count > 0)
            {
                this.TreeNavigator.SelectionChanged -= TreeNavigator_SelectionChanged;
                this.TreeNavigator.Items.Clear();
                SfTreeNavigatorItem treeItem = new SfTreeNavigatorItem();
                treeItem.Header = this.GetFileName();
                this.TreeNavigator.Items.Add(treeItem);
                this.TreeNavigator.SelectionChanged += TreeNavigator_SelectionChanged;
                this.DocumentMapButton.Visibility = Visibility.Visible;
                this.TreeNavigator.Height = (Windows.UI.Xaml.Window.Current.Bounds.Height - 100);
                this.NodeIteration(this.ReportModel.MapModel.NodeData, treeItem);
            }
            else
            {
                this.DocumentMapButton.Visibility = Visibility.Collapsed;
            }
        }

        string GetFileName()
        {
            if (!string.IsNullOrEmpty(this.ReportPath))
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.ReportPath);
            }
            return "Report";
        }

        void TreeNavigator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != e.RemovedItems)
            {
                if (e.AddedItems[0] is SfTreeNavigatorItem)
                {
                    SfTreeNavigatorItem view = e.AddedItems[0] as SfTreeNavigatorItem;
                    if (this.TreeNavigator.Items[0] != view)
                    {
                        if (!string.IsNullOrEmpty(view.Header.ToString()))
                        {
                            var node = (view.Tag as DocumentData);
                            {
                                if (this.Current + 1 != node.PageNo)
                                {
                                    this.GoTo(node.PageNo);
                                }
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
                else
                {
                    this.GoTo(1);
                    this.scrollViewer.ScrollToHorizontalOffset(0);
                    this.scrollViewer.ScrollToVerticalOffset(0);
                }
            }        
        }
       
        void NodeIteration(List<DocumentData> Nodes, SfTreeNavigatorItem treeItem)
        {
            foreach (var node in Nodes)
            {
                SfTreeNavigatorItem tree = new SfTreeNavigatorItem();
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

        public void SetDataSourceCredentials(DataSourceCredentials[] dataSourceCredentials)
        {
            if (!this.ReportModel.IsRDLC && this.ReportModel.HasReport)
            {
                this.ReportModel.SetDataSourceCredentials(dataSourceCredentials);
            }
        }

        public void SetDataSourceCredentials(IEnumerable<DataSourceCredentials> dataSourceCredentials)
        {
            if (!this.ReportModel.IsRDLC && this.ReportModel.HasReport)
            {
                this.ReportModel.SetDataSourceCredentials(dataSourceCredentials);
            }
        }

        /// <summary>
        /// Check the report parameter, if any, render in report parameter block
        /// </summary>
        internal void AddReportParameters()
        {
            if (this.ReportModel.Report.ReportParameters != null)
            {
                int columnPosition = 0;
                int rowPosition = 0;
                char[] trimCharacters = { ' ', '@', ':' };
                this.grid_ReportParameterBlock.Children.Clear();
                var reportParameters = from reportParameter in this.ReportModel.Report.ReportParameters
                                       where reportParameter.Hidden == false
                                       select reportParameter;

                foreach (Syncfusion.RDL.DOM.ReportParameter reportParameter in reportParameters)
                {
                    RowDefinition rowDefn = new RowDefinition();
                    rowDefn.Height = GridLength.Auto;
                    this.grid_ReportParameterBlock.RowDefinitions.Add(rowDefn);
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Text = reportParameter.Prompt.ToString().Trim(trimCharacters);
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    txtBlock.FontSize = 15;
                    txtBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    txtBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                    txtBlock.SetValue(Grid.ColumnProperty, columnPosition++);
                    txtBlock.SetValue(Grid.RowProperty, rowPosition);
                    txtBlock.Margin = new Thickness(5, 5, 5, 5);
                    txtBlock.MinWidth = 100;

                    this.grid_ReportParameterBlock.Children.Add(txtBlock);

                    var param = (from modelParamter in this.ReportModel.ParameterDetails
                                 where (modelParamter.Name.Equals(reportParameter.Name))
                                 select modelParamter).FirstOrDefault();

                    FrameworkElement parameterControl = this.GetReportItemGrid(reportParameter, param);

                    parameterControl.Name = "ReportParam_" + reportParameter.Name;
                    parameterControl.HorizontalAlignment = HorizontalAlignment.Left;
                    parameterControl.SetValue(Grid.ColumnProperty, columnPosition++);
                    parameterControl.SetValue(Grid.RowProperty, rowPosition);
                    parameterControl.Margin = new Thickness(5);
                    this.grid_ReportParameterBlock.Children.Add(parameterControl);

                    if (columnPosition == 2)
                    {
                        columnPosition = 0;
                        rowPosition++;
                    }
                }
            }
        }

        internal Grid GetReportItemGrid(Syncfusion.RDL.DOM.ReportParameter reportParameter, ParameterInformation param)
        {
            Grid parameterGrid = new Grid();
            int columnIndex = 0;
            parameterGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            parameterGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            parameterGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });

            string value = (param.Value != null && param.Value.Count > 0) ? param.Value.First().ToString() : null;

            if (reportParameter.MultiValue)
            {
                MultiValueComboBox comboData = new MultiValueComboBox();
                comboData.MinWidth = 100;

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
                Grid.SetColumn(comboData, columnIndex++);
                parameterGrid.Children.Add(comboData);
                comboData.Margin = new Thickness(5);
                comboData.DropDownClosed += comboData_DropDownClosed;
            }
            else if (reportParameter.ValidValues != null)
            {
                ComboBox comboData = new ComboBox();
                comboData.MinWidth = 100;
                comboData.MaxWidth = 100;
                if (reportParameter.ValidValues.ParameterValues != null && reportParameter.ValidValues.ParameterValues.Count() > 0)
                {
                    comboData.ItemsSource = reportParameter.ValidValues.ParameterValues.ToList();
                    comboData.SelectedValuePath = "Value";
                    comboData.DisplayMemberPath = "Label";
                    comboData.SelectedValue = value;
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
                    if (this.ProcessingMode == ProcessingMode.Remote)
                    {
                        comboData.SelectedValuePath = "ValueField";
                        comboData.DisplayMemberPath = "DisplayField";
                    }
                }

                Grid.SetColumn(comboData, columnIndex++);
                parameterGrid.Children.Add(comboData);
                comboData.Margin = new Thickness(5);
                comboData.SelectionChanged += new SelectionChangedEventHandler(comboData_SelectionChanged);
            }
            else if (reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.Boolean)
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

                rbtn_true.SetValue(Grid.ColumnProperty, columnIndex++);
                parameterGrid.Children.Add(rbtn_true);
                rbtn_true.Margin = new Thickness(5);
                rbtn_false.SetValue(Grid.ColumnProperty, columnIndex++);
                parameterGrid.Children.Add(rbtn_false);
                rbtn_false.Margin = new Thickness(5);
                rbtn_true.Checked += new RoutedEventHandler(rbtn_true_Checked);
                rbtn_false.Checked += new RoutedEventHandler(rbtn_false_Checked);
            }
            else if (reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.DateTime)
            {
                SfDatePicker dateEdit = new SfDatePicker();
                DateTime date = DateTime.Now;
                dateEdit.MinWidth = 100;
                dateEdit.MaxWidth = 100;

                if (value != null && DateTime.TryParse(value, out date))
                {
                    dateEdit.Value = date;
                }
                else
                {
                    dateEdit.Value = "";
                }

                Grid.SetColumn(dateEdit, columnIndex++);
                parameterGrid.Children.Add(dateEdit);
                dateEdit.Margin = new Thickness(5);
                dateEdit.ValueChanged += dateEdit_ValueChanged;
            }
            else if (reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.Float ||
                reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.Integer ||
                reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.String)
            {
                TextBox txtBox = new TextBox();
                txtBox.MinWidth = 100;
                txtBox.MaxWidth = 100;

                if (value != null)
                {
                    txtBox.Text = value;
                }

                txtBox.TextChanged += new TextChangedEventHandler(txtBox_TextChanged);
                txtBox.LostFocus += new RoutedEventHandler(txtBox_LostFocus);
                Grid.SetColumn(txtBox, columnIndex++);
                txtBox.Margin = new Thickness(5);
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
                chkbox.HorizontalAlignment = HorizontalAlignment.Center;
                chkbox.Margin = new Thickness(5);
                Grid.SetColumn(chkbox, columnIndex++);
                parameterGrid.Children.Add(chkbox);
            }

            return parameterGrid;
        }

        void comboData_DropDownClosed(object sender, object e)
        {
            MultiValueComboBox combo = sender as MultiValueComboBox;
            string selctedValues = string.Empty;

            if (combo.ItemsSource != null && combo.Parent != null)
            {
                Grid parentElement = combo.Parent as Grid;
                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();

                foreach (ParameterReportData data in combo.ItemsSource as IEnumerable)
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

        void dateEdit_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDatePicker dateTimeBox = d as SfDatePicker;

            if (!internalValueChange)
            {
                Grid parentElement = dateTimeBox.Parent as Grid;

                var param = GetReportParameter(parentElement);
                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(dateTimeBox.Value);
                param.Label.Add(dateTimeBox.Value);
                var dependentParam = from modelParamter in this.ReportModel.ParameterDetails
                                     where (!modelParamter.Name.Equals(param.Name) && modelParamter.DependentParameters.Contains(param.Name))
                                     select modelParamter;

                if (dependentParam.Count() > 0)
                {
                    this.UpdateDataSetParameterValues();
                }
            }
        }

        private void rbtn_true_Checked(object sender, RoutedEventArgs e)
        {
            if (!internalValueChange)
            {
                RadioButton rbtn = sender as RadioButton;
                Grid parentElement = rbtn.Parent as Grid;
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
                Grid parentElement = rbtn.Parent as Grid;
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
                Grid parentElement = checkBox.Parent as Grid;
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
                Grid parentElement = checkBox.Parent as Grid;

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
                Grid parentElement = txtBox.Parent as Grid;
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
        }

        private void UpdateDataSetParameterValues()
        {
            if (this.ProcessingMode == ProcessingMode.Local)
            {
                this.ReportModel.DataSources = this.DataSources;
            }

            this.ReportModel.DataSourceUpdated += new DataSourceUpdatedEventHandler(ReportParametersDataSourceUpdated);
            this.ReportModel.UpdateReportParameters();
        }

        void ReportParametersDataSourceUpdated(object sender, EventArgs e)
        {
            this.ReportModel.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ReportParametersDataSourceUpdated);

            internalValueChange = true;
            Syncfusion.RDL.DOM.ReportParameters reportParameters = this.ReportModel.Report.ReportParameters;
            var dataSetParameters = this.ReportModel.Report.ReportParameters.Where(p => p.ValidValues != null && p.ValidValues.DataSetReference != null);
            var parameterControlCollection = this.grid_ReportParameterBlock.Children.OfType<Grid>();
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

                    if (this.ProcessingMode == ProcessingMode.Remote)
                    {
                        List<ParameterReportData> datavalues = new List<ParameterReportData>();

                        foreach (Syncfusion.RDL.Data.ReportData data in m_itemSource)
                        {
                            if (data.Data[displayMemberPath] != null)
                            {
                                datavalues.Add(new ParameterReportData() { ValueField = data.Data[selectedValuePath].ToString(), DisplayField = data.Data[displayMemberPath].ToString() });
                            }
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
                        isSelected.Add(false);

                        foreach (object data in m_itemSource)
                        {
                            helperComboBox.SelectedItem = null;
                            helperComboBox.SelectedValuePath = selectedValuePath;
                            helperComboBox.SelectedItem = data;
                            string valueFiledValue = helperComboBox.SelectedValue.ToString();
                            helperComboBox.SelectedValuePath = displayMemberPath;
                            helperComboBox.SelectedItem = data;
                            string displayValue = helperComboBox.SelectedValue.ToString();
                            isSelected.Add(param.Label != null && param.Label.Contains(displayValue));
                            datavalues.Add(new ParameterReportData() { ValueField = valueFiledValue, DisplayField = displayValue });
                        }

                        (comboBox as MultiValueComboBox).IsSelected = isSelected;
                        comboBox.ItemsSource = datavalues;
                    }
                    else
                    {
                        comboBox.ItemsSource = m_itemSource;

                        if (param.Value.Count > 0)
                        {
                            object selectedval = param.Value.First();
                            comboBox.SelectedValue = selectedval.ToString();
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



        void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        void comboData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (!internalValueChange)
            {
                Grid parentElement = combo.Parent as Grid;

                var param = GetReportParameter(parentElement);

                param.Value = new List<object>();
                param.Label = new List<object>();
                param.Value.Add(combo.SelectedValue);
                string label = combo.SelectedValue.ToString();

                if (this.ProcessingMode == ProcessingMode.Remote)
                {
                    ParameterReportData seletedItem = combo.SelectedItem as ParameterReportData;

                    if (seletedItem.DisplayField != null)
                    {
                        param.Label.Add(seletedItem.DisplayField.ToString());
                    }

                    RDL.DOM.ParameterValue selectedValue = combo.SelectedItem as RDL.DOM.ParameterValue;

                    if (selectedValue != null)
                    {
                        param.Label.Add(selectedValue.Label);
                    }
                }
                else
                {
                    Type propertyType = combo.SelectedItem.GetType();
                    IEnumerable<PropertyInfo> propertyInfo = propertyType.GetRuntimeProperties();

                    if (propertyInfo.Where(pi => pi.Name == combo.DisplayMemberPath).FirstOrDefault() != null)
                    {
                        label = combo.SelectedItem.GetType().GetRuntimeProperty(combo.DisplayMemberPath).GetValue(combo.SelectedItem, null) as string;
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


        private ParameterInformation GetReportParameter(Grid parameter)
        {
            string paramName = parameter.Name.Remove(0, ("ReportParam_").Length);

            var param = (from modelParamter in this.ReportModel.ParameterDetails
                         where (modelParamter.Name.Equals(paramName))
                         select modelParamter).FirstOrDefault();

            return param;
        }


        public ReportParameterInfoCollection GetParameters()
        {
            return this.ReportModel.GetParameters();
        }

        public void SetParameters(IEnumerable<ReportParameter> reportParameters)
        {
            this.ReportModel.SetParameters(reportParameters);
        }

        public void LoadReport(Stream fileStream)
        {
            this.loadStream = fileStream;
        }

        internal void ShowExcpetionContent()
        {
            ShowLoadingGrid(false);
            ShowContentGrid(false);
            ShowExceptionGrid(true);
        }

        private void CreateExceptionWindow(string message)
        {
            this.ShowExcpetionContent();

            //// Adding message to the exception window
            this.textBlockException.Text = message;
            this.textBlockException.Width = 340;
            this.textBlockException.HorizontalAlignment = HorizontalAlignment.Left;
            this.textBlockException.TextAlignment = TextAlignment.Center;
            this.textBlockException.Margin = new Thickness(10, 48, 10, 24);
            this.textBlockException.Foreground = new SolidColorBrush(Colors.Gray);
            this.NextPopup.IsOpen = false;
            this.PreviousPopup.IsOpen = false;

            if (string.IsNullOrEmpty(this.stackTrace))
            {
                this.groupBoxExpandedException.Visibility = Visibility.Collapsed;
                this.groupBoxExpandedExceptionScroll.Visibility = Visibility.Collapsed;
                this.toggleShowDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.groupBoxExpandedException.Visibility = Visibility.Visible;
                this.groupBoxExpandedExceptionScroll.Visibility = Visibility.Visible;
                this.toggleShowDetails.Visibility = Visibility.Visible;
            }

            this.textBlockException.TextWrapping = TextWrapping.Wrap;

        }

        void ClearViewer()
        {
            this.canvasContentPage.Children.Clear();
            this.CanvasHeader.Children.Clear();
            this.CanvasFooter.Children.Clear();
        }

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

        internal void ShowCredntialBlockContent()
        {
            ShowLoadingGrid(false);
            ShowContentGrid(false);
            ShowExceptionGrid(false);
            ShowCredentialBlock(true);
        }

        void ShowCredentialBlock(bool show)
        {
            if (show)
            {
                this.Dialog.Visibility = Visibility.Visible;
                this.Credblock.Height = new GridLength(1, GridUnitType.Star);
                this.NextPopup.IsOpen = false;
                this.PreviousPopup.IsOpen = false;
            }
            else
            {
                this.Dialog.Visibility = Visibility.Collapsed;
                this.Credblock.Height = new GridLength(0);
            }
        }

        void ShowLoadingGrid(bool show)
        {
            if (show)
            {
                this.loadingIndicatorRow.Height = new GridLength(1, GridUnitType.Star);
                this.gridLoadingIndicator.Visibility = Visibility.Visible;
                this.NextPopup.IsOpen = false;
                this.PreviousPopup.IsOpen = false;

            }
            else
            {
                this.gridLoadingIndicator.Visibility = Visibility.Collapsed;
                this.loadingIndicatorRow.Height = new GridLength(0);
                this.UpdatePageDetails(this.Current);
            }
        }

        CancellationTokenSource currentTask = null;

        public void Reset()
        {
            if (ReportModel != null)
            {
                this.ReportModel.ReportLoaded -= ReportModel_ReportLoaded;
                this.ReportModel.ReportingServer.ExportCompleted -= ReportingServer_ExportCompleted;
                this.ReportModel.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ReportModel_DataSourceUpdated);
                this.ReportModel.ReportItemsEvaluated -= ReportModel_ReportItemsEvaluated;
                this.ReportModel.SubreportProcessing -= ReportModel_SubreportProcessing;
            }

            this.loadStream = null;
            this.ReportModel = null;
            this.rendered = null;

            if (currentTask != null)
            {
                currentTask.Cancel();
            }

            this.CanvasFooter.Children.Clear();
            this.CanvasHeader.Children.Clear();
            this.canvasContentPage.Children.Clear();
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                this.ShowLoadingGrid(false);
                this.ShowContentGrid(true);
            });

            GC.Collect();
        }

        /// <summary>
        /// Refresh the report and Navigates to the first possible page and refresh data of the report.
        /// </summary>
        public void Refresh()
        {
            this.DisableButton();
            if (this.ReportRefresh != null)
            {
                EventArgs args = new EventArgs();
                ReportRefresh(this, args);
            }
            this.ClearViewer();
            this.UpdateReport();
        }

        void DisableButton()
        {
            this.NextButton.IsEnabled = false;
            this.LastButton.IsEnabled = false;
            this.FirstButton.IsEnabled = false;
            this.PreviousButton.IsEnabled = false;
            this.Parameter.IsEnabled = false;
            this.PrintButton.IsEnabled = false;
            this.ExportButton.IsEnabled = true;
            this.DocumentMapButton.Visibility = Visibility.Collapsed;
        }

        public void RefreshReport()
        {
            if (this.ReportRefresh != null)
            {
                EventArgs args=new EventArgs();
                ReportRefresh(this, args);
            }

            if (this.ReportModel == null)
            {
                this.ReportModel = new ReportModel();
            }

            this.ReportModel.EnableVirtualEvaluation = this.EnableVirtualEvaluation;

            this.HasReportParameters = false;

            if (!this.isDrillReport)
            {
                this.ReportModel.ReportServerCredential = this.ReportServerCredential;
                this.ReportModel.ReportServerFormsCredential = this.ReportServerFormsCredential;
                this.ReportModel.ReportServerUrl = this.ReportServerUrl;
                this.ReportModel.ReportPath = this.ReportPath;
                this.ReportModel.ReportServiceURL = this.ReportServiceURL;
                this.ReportModel.LoadInformationFromServer = this.LoadCredentialsInformationinServer;
                this.ReportModel.IsRDLC = this.ProcessingMode == Syncfusion.UI.Xaml.Reports.ProcessingMode.Local;
            }
            this.isDrillReport = false;
            this.ReportModel.ReportLoaded += ReportModel_ReportLoaded;
            this.ReportModel.SubreportProcessing += ReportModel_SubreportProcessing;
            this.ReportModel.DrillThroughInnerReport += ReportModel_DrillThroughInnerReport;
            this.ReportModel.ToggleChanged += ReportModel_ToggleChanged;
            this.ShowContentGrid(false);
            this.ShowLoadingGrid(true);

            CancellationTokenSource ts = new CancellationTokenSource();
            CancellationToken token = ts.Token;
            this.currentTask = ts;

            Task.Run(() =>
            {
                this.LoadReport();
            }, token);
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
            this.ReportModel.Model = drillModel;
            this.isDrillReport = true;
            this.RefreshReport();
            this.BackButton.Visibility = Visibility.Visible;
        }

        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            int currentPageNo = this.Current;
            if(this.ReportModel.Model!=null)
            {
                if (this.ReportModel.Model.ParentModel != null)
                {
                    this.ReportModel = this.ReportModel.Model.ParentModel;
                    if (this.ReportModel.Model == null)
                    {
                        this.BackButton.Visibility = Visibility.Collapsed;
                    }
                    this.pageModelFactory = this.ReportModel.PageModelFactory;

                    var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                           where reportParameter.Hidden == false
                                           select reportParameter;

                    this.HasReportParameters = reportParameters.Count() > 0;
                    this.AddReportParameters();
                    this.UpdateDataSetParameterValues();
                    if (this.pageModelFactory != null)
                    {
                        this.UpdatePages();
                        this.UpdateCurrentPage();
                    }
                }
            }
            else
            {
                this.BackButton.Visibility = Visibility.Collapsed;
            }
        }


        void UpdateReportingToolBarControls()
        {
            try
            {
                this.NextButton = GetTemplateChild("Next") as Button;
                this.PreviousButton = GetTemplateChild("Previous") as Button;
                this.FirstButton = GetTemplateChild("First") as Button;
                this.LastButton = GetTemplateChild("Last") as Button;
                this.BackButton = GetTemplateChild("Back") as Button;
                this.DocumentMapButton = GetTemplateChild("DocumentMap") as Button;
                this.ExportButton = GetTemplateChild("ExportButton") as Button;
                this.RefreshButton = GetTemplateChild("Refresh") as Button;
                this.Parameter = GetTemplateChild("Parameter") as Button;
                this.NavigationBar = GetTemplateChild("NavigationBar") as Grid;

                this.NextButton.Click += NextButton_Click;
                this.PreviousButton.Click += PreviousButton_Click;
                this.FirstButton.Click += FirstButton_Click;
                this.LastButton.Click += LastButton_Click;
                this.BackButton.Click += Btn_back_Click;
                this.RefreshButton.Click += RefreshButton_Click;
                this.DocumentMapButton.Click += DocumentMapButton_Click;
                this.ExportButton.Click += Export_Click;
                this.Parameter.Click += Parameter_Click;

            }
            catch
            {

            }

        }

        void DocumentMapButton_Click(object sender, RoutedEventArgs e)
        {
            this.DocumentMapBlock.IsOpen = true;
        }

        void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.ReportModel.ExceptionDetails.Clear();
            this.Refresh();
        }

        void Parameter_Click(object sender, RoutedEventArgs e)
        {
            this.ParameterBlock.IsOpen = true;
        }

        void LastButton_Click(object sender, RoutedEventArgs e)
        {
            this.MoveLast();
        }

        void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            this.MoveFirst();
        }

        void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            this.MovePrevious();
        }

        void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.MoveNext();
        }

        async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            fileSavePicker = new FileSavePicker();
            string format = (sender as Button).Content.ToString().ToLower();
            fileSavePicker = new FileSavePicker();
            switch (format)
            {
                case "word":
                    {
                        fileSavePicker.FileTypeChoices.Add("Word", new List<string> { ".doc" });
                        fileSavePicker.DefaultFileExtension = ".doc";
                        break;
                    }
                case "pdf":
                    {
                        fileSavePicker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });
                        fileSavePicker.DefaultFileExtension = ".pdf";
                        break;
                    }
                case "excel":
                    {
                        fileSavePicker.FileTypeChoices.Add("Excel", new List<string> { ".xls" });
                        fileSavePicker.DefaultFileExtension = ".xls";
                        break;
                    }
                case "html":
                    {
                        fileSavePicker.FileTypeChoices.Add("Html", new List<string> { ".html" });
                        fileSavePicker.DefaultFileExtension = ".html";
                        break;
                    }


            }
            fileSavePicker.SuggestedFileName = "ExportReport";
            savedItem = await fileSavePicker.PickSaveFileAsync();
            if (savedItem != null)
            {
                ExportReport(fileSavePicker, format);
            }
        }

        async void ExportReport(FileSavePicker fileSavePicker, string writerFormat)
        {
            this.ReportModel.ReportingServer.ExportCompleted += ReportingServer_ExportCompleted;
            MemoryStream exportFileStream = new MemoryStream();
            if (this.ExportMode == ExportMode.Local)
            {
                ReportWriter.WriterFormat format = (Syncfusion.ReportWriter.WriterFormat)Enum.Parse(typeof(Syncfusion.ReportWriter.WriterFormat), writerFormat, true);
                ReportWriter.ReportWriter reportWriter = new ReportWriter.ReportWriter();
                this.ReportModel.IsEvaluatedReport = true;
                reportWriter.ExportMode = ReportWriter.ExportMode.Local;
                reportWriter.ReportModel = this.ReportModel;
                await reportWriter.SaveASync(exportFileStream, format);
                ExportFile(fileSavePicker, exportFileStream);
                Windows.UI.Popups.MessageDialog msgDialog = new Windows.UI.Popups.MessageDialog("Report has been exported successfully.");
                msgDialog.ShowAsync();
            }

            else if (!string.IsNullOrEmpty(this.ReportServiceURL))
            {
                Syncfusion.Reports.Server.ReportSetting reportSetting = new Syncfusion.Reports.Server.ReportSetting() { ReportPath = this.ReportModel.ReportPath, ReportServerURL = this.ReportModel.ReportServerUrl };

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

                    if (paramInfo.Labels == null)
                    {
                        paramInfo.Labels = new ObservableCollection<string>();
                    }
                    if (paramInfo.Values == null)
                    {
                        paramInfo.Values = new ObservableCollection<string>();
                    }
                    paramInfo.Labels.Add(parameter.Labels.Count.ToString());
                    paramInfo.Values.Add(parameter.Values.Count.ToString());

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

                reportSetting.Parameters = new ObservableCollection<Syncfusion.Reports.Server.ReportParameterInfo>();

                foreach (Syncfusion.Reports.Server.ReportParameterInfo info in parametersInfo)
                {
                    reportSetting.Parameters.Add(info);
                }

                if (this.ProcessingMode == ProcessingMode.Remote)
                {

                    List<Syncfusion.Reports.Server.DataSourceCredentialsInfo> credentialsInfo = new List<Syncfusion.Reports.Server.DataSourceCredentialsInfo>();

                    if (this.ReportModel.Report.DataSources != null)
                    {
                        foreach (var dataSource in this.ReportModel.Report.DataSources)
                        {
                            Syncfusion.Reports.Server.DataSourceCredentialsInfo dataSourceInfo = new Syncfusion.Reports.Server.DataSourceCredentialsInfo();
                            dataSourceInfo.Name = dataSource.Name;
                            dataSourceInfo.UserId = dataSource.ConnectionProperties.UserName;
                            dataSourceInfo.Password = dataSource.ConnectionProperties.PassWord;
                            dataSourceInfo.IntegratedSecurity = dataSource.ConnectionProperties.IntegratedSecurity;
                            credentialsInfo.Add(dataSourceInfo);
                        }
                    }
                    reportSetting.DataSourceCredentials = new ObservableCollection<Syncfusion.Reports.Server.DataSourceCredentialsInfo>();
                    foreach (Syncfusion.Reports.Server.DataSourceCredentialsInfo info in credentialsInfo)
                    {
                        reportSetting.DataSourceCredentials.Add(info);

                    }
                }
                else
                {
                    List<Syncfusion.Reports.Server.ReportDataSource> listDatasourceinfo = new List<Syncfusion.Reports.Server.ReportDataSource>();
                    var datasourceCollection = this.ReportModel.DataSources as ReportDataSourceCollection;

                    foreach (var reportDataSource in datasourceCollection)
                    {
                        Syncfusion.Reports.Server.ReportDataSource datasourceinfo = new Syncfusion.Reports.Server.ReportDataSource();
                        datasourceinfo.Name = reportDataSource.Name;
                        if (datasourceinfo.Value == null)
                        {
                            datasourceinfo.Value = new ObservableCollection<Syncfusion.Reports.Server.ReportData>();
                        }

                        foreach (Syncfusion.Reports.Server.ReportData data in this.GetWrapperDataSource(reportDataSource.Value))
                        {
                            datasourceinfo.Value.Add(data);
                        }
                        listDatasourceinfo.Add(datasourceinfo);
                    }
                    reportSetting.DataSources = new ObservableCollection<Syncfusion.Reports.Server.ReportDataSource>();
                    foreach (Syncfusion.Reports.Server.ReportDataSource data in listDatasourceinfo)
                    {
                        reportSetting.DataSources.Add(data);
                    }
                }
                this.ReportModel.ReportingServer.ExportAsync(reportSetting, writerFormat);
            }
        }

        void ReportingServer_ExportCompleted(object sender, RDL.ServerProcessor.ExportedEventArgs e)
        {
            ExportFile(fileSavePicker, new MemoryStream(e.Result));
            Windows.UI.Popups.MessageDialog msgDialog = new Windows.UI.Popups.MessageDialog("Report has been exported successfully.");
            msgDialog.ShowAsync();
        }

        async void ExportFile(FileSavePicker fileSavePicker, MemoryStream ms)
        {
            if (savedItem != null)
            {
                try
                {
                    using (IRandomAccessStream stream = await savedItem.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        // Write compressed data from memory to file
                        using (Stream outstream = stream.AsStreamForWrite())
                        {
                            byte[] buffer = ms.ToArray();
                            outstream.Write(buffer, 0, buffer.Length);
                            outstream.Flush();
                        }
                    }
                    ms.Dispose();
                }
                catch
                {

                }
            }
        }

        List<Syncfusion.Reports.Server.ReportData> GetWrapperDataSource(IEnumerable Datasoure)
        {
            List<Syncfusion.Reports.Server.ReportData> dataSource1 = new List<Syncfusion.Reports.Server.ReportData>();
            IEnumerable list = Datasoure;
            foreach (object o in list)
            {
                Syncfusion.Reports.Server.ReportData data = new Syncfusion.Reports.Server.ReportData();
                data.Data = new Dictionary<string, object>();
                System.Type objectType = o.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(objectType.GetRuntimeProperties());
                foreach (PropertyInfo prop in props)
                {

                    bool isField = false;
                    Syncfusion.RDL.DOM.DataSets info = this.ReportModel.Report.DataSets;

                    foreach (Syncfusion.RDL.DOM.DataSet dataset in info)
                    {
                        foreach (Syncfusion.RDL.DOM.Field fields in dataset.Fields)
                        {
                            if ( (fields.DataField == prop.Name) || fields.Name == prop.Name)
                            {
                                isField = true;
                            }
                        }
                    }

                    if (isField)
                    {
                        string propValue = prop.Name;
                        object getObjectValue = prop.GetValue(o, null);
                        data.Data.Add(propValue, getObjectValue);
                    }
                }
                dataSource1.Add(data);
            }
            return dataSource1;
        }



        private void UpdatePageDetails(int currentpage)
        {
            currentpage++;
            timer.Stop();
            if (!isPageInfo)
            {
                this.PageInfoBlock.IsOpen = true;
            }
            isPageInfo = false;

            timer.Start();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 3);
            this.PageNumber.Text = currentpage.ToString();
            if (currentpage == 1)
            {
                this.FirstButton.IsEnabled = false;
                this.PreviousButton.IsEnabled = false;
                this.PreviousPopup.IsOpen = false;
            }
            else
            {
                this.FirstButton.IsEnabled = true;
                this.PreviousButton.IsEnabled = true;
                this.PreviousPopup.IsOpen = true;
            }


            if (currentpage == m_totalPages)
            {
                this.NextButton.IsEnabled = false;
                this.LastButton.IsEnabled = false;
                this.NextPopup.IsOpen = false;
            }
            else
            {
                this.NextButton.IsEnabled = true;
                this.LastButton.IsEnabled = true;
                this.NextPopup.IsOpen = true;
            }
        }

        void timer_Tick(object sender, object e)
        {
            this.PageInfoBlock.IsOpen = false;
            timer.Stop();
            timer.Tick -= timer_Tick;
        }

        void ReportModel_ReportLoaded(object sender, EventArgs e)
        {
            this.ReportLoaded -= ReportModel_ReportLoaded;
            this.RaiseReportLoadedEvent();
            this.ProcessReport();
        }

        void UpdateReport()
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                this.ShowContentGrid(false);
                this.ShowLoadingGrid(true);
            });

            this.ReportModel.ProcessedData.ResetDocumentModel();
            this.ReportModel.ProcessedData.ResetProceesedData();
            this.ReportModel.DataSources = this.DataSources;
            this.ReportModel.DataSourceUpdated += new DataSourceUpdatedEventHandler(ReportModel_DataSourceUpdated);
            this.ReportModel.InitilizeReport();
        }

        void ReportModel_DataSourceUpdated(object sender, EventArgs e)
        {
            this.ReportModel.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ReportModel_DataSourceUpdated);

            if (this.ReportModel.ProcessedData != null && this.ReportModel.ProcessedData.HasException)
            {
                string exceptionMessage = this.ReportModel.ProcessedData.Exception.Message;
                var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                       where reportParameter.Hidden == false
                                       select reportParameter;

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.HasReportParameters = reportParameters.Count() > 0;
                    Exception ex = new Exception(exceptionMessage);
                    this.ShowException(ex);
                });

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
            if (this.ReportModel != null)
            {
                this.ReportModel.ExceptionDetails = new List<string>();
                this.ReportModel.ReportItemsEvaluated += ReportModel_ReportItemsEvaluated;
                this.ReportModel.Evaluate();
            }
        }

        void ReportModel_ReportItemsEvaluated(object sender, ReportItemEvaluatedEventArgs e)
        {
            this.ReportModel.ReportItemsEvaluated -= ReportModel_ReportItemsEvaluated;
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                this.ReportModel.UpdateSize();

                CancellationTokenSource ts = new CancellationTokenSource();
                CancellationToken token = ts.Token;
                this.currentTask = ts;

                Task.Run(() =>
                {
                    this.UpdatePageModelFactory();
                }, token);
            }));
        }

        private void CreateInstaceofpageModel()
        {
            this.pageModelFactory = new PageModelFactory(this.ReportModel);
            this.hasPageHeader = this.ReportModel.Page.PageHeader != null ? true : false;
            this.hasPageFooter = this.ReportModel.Page.PageFooter != null ? true : false;

            this.pageModelFactory.PageHeight = this.PaperHeight;
            this.pageModelFactory.PageWidth = this.PaperWidth;
            this.pageModelFactory.Margin = new LayoutThicknessInfo(this.MarginLeft, this.MarginTop,
                                                                   this.MarginRight, this.MarginBottom);
            this.pageModelFactory.IsPrintMode = true;
            this.pageModelFactory.UpdatePageLayout();
            this.pageModelFactory.UpdatePrintPageLayout();
            this.ReportModel.PageModelFactory = this.pageModelFactory;
            this.pageModelFactory = this.ReportModel.PageModelFactory;
            this.ReportModel.IsToggleState = false;
        }

        private void UpdatePageModelFactory()
        {

            try
            {
                this.CreateInstaceofpageModel();
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    this.canvasContentPage.Children.Clear();
                    this.CanvasHeader.Children.Clear();
                    this.CanvasFooter.Children.Clear();

                    this.RefreshViewer();
                }));
            }
            catch (Exception e)
            {
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                           where reportParameter.Hidden == false
                                           select reportParameter;

                    this.HasReportParameters = reportParameters.Count() > 0;

                    this.ShowException(e);
                }));

            }
        }

        private void UpdatePageLayout()
        {
            try
            {
                this.CreateInstaceofpageModel();
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    this.canvasContentPage.Children.Clear();
                    this.CanvasHeader.Children.Clear();
                    this.CanvasFooter.Children.Clear();

                    if (this.pageModelFactory != null)
                    {
                        this.UpdatePages();
                    }
                    isPageInfo = true;
                    this.UpdateCurrentPage();
                }));
            }
            catch (Exception e)
            {
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    this.ShowException(e);
                }));

            }
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

                var reportParameters = from reportParameter in this.ReportModel.ParameterDetails
                                       where reportParameter.Hidden == false
                                       select reportParameter;

                this.HasReportParameters = reportParameters.Count() > 0;

                if (this.HasReportParameters)
                {
                    this.Parameter.IsEnabled = true;
                }

                //// Updates toolbar icons
                //this.UpdateToolBarIcons();

                this.RefreshReportViewer();
                this.ShowLoadingGrid(false);
                this.ShowContentGrid(true);
            }
            catch 
            {
            }
        }

        internal void RefreshReportViewer()
        {
            if (this.pageModelFactory != null)
            {
                this.UpdatePages();
                this.Current = 0;
                this.Parameter.IsEnabled = this.HasReportParameters;
                this.PrintButton.IsEnabled = true;
                this.ExportButton.IsEnabled = true;
                this.DocumentMapTree();
            }
        }

        void UpdatePages()
        {
            if (this.pageModelFactory != null)
            {
                this.pageModelFactory.IsPrintMode = true;
                this.m_totalPages = pageModelFactory.PageDictionary.Count;
                this.TotalPages.Text = this.m_totalPages.ToString();
                this.canvasContentPage.InvalidateArrange();
                this.CanvasHeader.InvalidateArrange();
            }
        }

        internal void UpdateExceptionDetails()
        {
            if (this.ReportModel.ExceptionDetails!=null && this.ReportModel.ExceptionDetails.Count > 0)
            {

                this.errorMessage = string.Empty;

                foreach (var error in this.ReportModel.ExceptionDetails)
                {
                    this.errorMessage += error + "\n";
                }

                if (this.ReportError != null)
                {
                    this.ReportError(this, new ReportErrorEventArgs { Message = this.errorMessage });
                }
            }
        }


        /// <summary>
        /// Hides the visibility.
        /// </summary>
        /// <param name="canvasObj">The canvas obj.</param>
        private void HideVisibility(Canvas canvasObj)
        {
            if (canvasObj != null)
            {
                foreach (var item in canvasObj.Children)
                {
                    if (item is UIElement)
                    {
                        (item as UIElement).Visibility = Visibility.Collapsed;
                    }
                    else if (item is FrameworkElement)
                    {
                        (item as FrameworkElement).Visibility = Visibility.Collapsed;
                    }
                }
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

            foreach (IReportItemModeler reportItemContent in pageModelCollection)
            {
                switch (reportItemContent.ModelType)
                {
                    case ModelType.TextBoxModel:
                        ReportingTextbox richTextBox = new ReportingTextbox(reportItemContent, true);
                        canvasHeaderFooter.Children.Add(richTextBox);
                        break;

                    case ModelType.LineModel:
                        ReportingLine line = new ReportingLine(reportItemContent);
                        canvasHeaderFooter.Children.Add(line);
                        break;
                    case ModelType.ImageModel:
                        ReportingImage image = new ReportingImage(reportItemContent);
                        canvasHeaderFooter.Children.Add(image);
                        break;
                    case ModelType.RectangleModel:
                        canvasHeaderFooter.Children.Add(new ReportingRectangle(reportItemContent));
                        break;
                }
            }
        }

        private void DrawBodyControls(ReportModelContentCollection reportModelCollection)
        {
            this.canvasContentPage.Children.Clear();
            try
            {
                foreach (IReportItemModeler reportItemContent in reportModelCollection)
                {
                    switch (reportItemContent.ModelType)
                    {
                        case ModelType.TextBoxModel:
                            ReportingTextbox richTextBox = new ReportingTextbox(reportItemContent);
                            this.canvasContentPage.Children.Add(richTextBox);
                            break;
                        case ModelType.TablixModel:
                            ReportingTablixControl dataGrid = new ReportingTablixControl(reportItemContent);
                            this.canvasContentPage.Children.Add(dataGrid);
                            dataGrid.CurrentPage = dataGrid.GetPage(this.Current);
                            dataGrid.UpdatePage();
                            break;
                        case ModelType.LineModel:
                            ReportingLine line = new ReportingLine(reportItemContent);
                            this.canvasContentPage.Children.Add(line);
                            break;
                        case ModelType.ChartModel:
                            ReportingChartControl chart = new ReportingChartControl(reportItemContent);
                            this.canvasContentPage.Children.Add(chart);
                            break;
                        case ModelType.GaugeModel:
                            ReportingGauge gauge = new ReportingGauge(reportItemContent);
                            this.canvasContentPage.Children.Add(gauge);
                            break;
                        case ModelType.ImageModel:
                            ReportingImage image = new ReportingImage(reportItemContent);
                            this.canvasContentPage.Children.Add(image);
                            break;
                        case ModelType.RectangleModel:
                            ReportingRectangle rect = new ReportingRectangle(reportItemContent);
                            this.canvasContentPage.Children.Add(rect);
                            rect.CurrentPage = rect.GetPage(this.Current);
                            break;
                        case ModelType.SubReportModel:
                            ReportingSubReport subReport = new ReportingSubReport(reportItemContent);
                            this.canvasContentPage.Children.Add(subReport);
                            //       subReport.CurrentPage = subReport.GetPage(this.Current);
                            break;
                        case ModelType.MapModel:
                            Reports.Controls.ReportingMap mapModel = new Reports.Controls.ReportingMap(reportItemContent);
                            this.canvasContentPage.Children.Add(mapModel);
                            break;
                    }
                }

            }
            catch
            {
            }
        }

        internal void LoadReport()
        {
            if (this.loadStream != null)
            {
                this.ReportModel.LoadReport(this.loadStream);
            }
            else if (!string.IsNullOrEmpty(this.ReportModel.ReportPath))
            {
                this.ReportModel.ProcessReport();
            }
        }

        internal void ProcessReport()
        {
            if (this.ReportModel.HasReport)
            {
                this.MarginLeft = (this.ReportModel.Page.LeftMargin != null && this.ReportModel.Page.LeftMargin.size != null) ? this.ReportModel.Page.LeftMargin.PixelValue : 0;
                this.MarginRight = (this.ReportModel.Page.RightMargin != null && this.ReportModel.Page.RightMargin.size != null) ? this.ReportModel.Page.RightMargin.PixelValue : 0;
                this.MarginTop = (this.ReportModel.Page.TopMargin != null && this.ReportModel.Page.TopMargin.size != null) ? this.ReportModel.Page.TopMargin.PixelValue : 0;
                this.MarginBottom = (this.ReportModel.Page.BottomMargin != null && this.ReportModel.Page.BottomMargin.size != null) ? this.ReportModel.Page.BottomMargin.PixelValue : 0;

                this.PaperHeight = this.ReportModel.Page.PageHeight != null ? this.ReportModel.Page.PageHeight.PixelValue : 1122.64;
                this.PaperWidth = this.ReportModel.Page.PageHeight != null ? this.ReportModel.Page.PageWidth.PixelValue : 793.92;

                pageModelFactory = null;
                m_totalPages = 0;

                ////// Loading indicator initialization.
                this.m_current = 0;

                if (!this.ReportModel.IsRDLC)
                {
                    List<Syncfusion.RDL.DOM.DataSource> dataSources = null;

                    dataSources = this.ReportModel.GetCredentialDataSources();

                    if (dataSources != null && !this.LoadCredentialsInformationinServer)
                    {
                        this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                        {
                            this.CredentailDataSource = dataSources;
                            this.credDatasourceName.Text =  this.CredentailDataSource[0].ConnectionProperties.Prompt;
                            this.ShowCredntialBlockContent();
                        });

                    }
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
                Exception ex = new Exception("The source of the report has not been specified");
                this.ShowException(ex);
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

        /// <summary>
        /// Raises to nofity loaded information.
        /// </summary>
        protected virtual void RaiseReportLoadedEvent()
        {
            if (this.ReportLoaded != null)
            {
                this.ReportLoaded(this, new EventArgs());
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

        StackPanel GetVisual(int pageNumber)
        {
            this.GetHeaderFooter(this.CanvasHeader, this.CanvasFooter);
            this.DrawBodyControls(pageModelFactory.PrintLayoutPageDictionary[pageNumber].ReportModelCollection);
            this.canvasContentPage.UpdateLayout();
            this.PageView.UpdateLayout();
            return PageView;
        }


        public async void PrintReport()
        {
             RegisterForPrinting();
            await PrintManager.ShowPrintUIAsync();
        }

        protected void RegisterForPrinting()
        {
            try
            {
                // Create the PrintDocument.
                printDocument = new PrintDocument();

                // Save the DocumentSource.
                printDocumentSource = printDocument.DocumentSource;

                // Add an event handler which sets up print preview.
                printDocument.Paginate += Paginate;


                // Add an event handler which provides a specified preview page.
                printDocument.GetPreviewPage += GetPrintPreviewPage;

                // Add an event handler which provides all final print pages.
                printDocument.AddPages += AddPrintPages;

                // Create a PrintManager and add a handler for printing initialization.
                PrintManager printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested += PrintTaskRequested;
                //isRegisterforPrint = true;
            }
            catch
            {

            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. 
        /// It fires when the PrintManager requests print preview
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        protected void Paginate(object sender, PaginateEventArgs e)
        {
            // Report the number of preview pages
            printDocument.SetPreviewPageCount(pageModelFactory.PageCount, PreviewPageCountType.Intermediate);
            this.PageInfoBlock.IsOpen = false;
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        protected void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDocument.SetPreviewPage(e.PageNumber, GetVisual(this.Current));
            this.Current = e.PageNumber;
            this.PageInfoBlock.IsOpen = false;
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        protected void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the pages and add each one to be printed
            int current = this.Current;
            this.MoveFirst();
            for (int i = 0; i < pageModelFactory.PageCount;i++ )
            {
                this.PageInfoBlock.IsOpen = false;
                printDocument.AddPage(GetVisual(i));
                this.Current = i + 1;
            }

            // Indicate that all of the print pages have been provided
            printDocument.AddPagesComplete();
            this.Current = current;
        }


        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("Syncfusion ReportViewer", sourceRequested =>
            {
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    // Notify the user when the print operation fails.
                    if (args.Completion == PrintTaskCompletion.Failed)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            MessageDialog dialog = new MessageDialog("Failed to print.");
                            dialog.ShowAsync();
                        });
                    }
                };

                sourceRequested.SetSource(printDocumentSource);
            });


            sender.PrintTaskRequested -= PrintTaskRequested;
        }

    }

    #region ProcessingMode for ReportViewer
    public enum ProcessingMode
    {
        Remote,
        Local
    }

    public enum ExportMode
    {
        /// <summary>
        /// If export from Server 
        /// </summary>
        /// <remarks></remarks>
        Server,
        /// <summary>
        /// If export from Local
        /// </summary>
        /// <remarks></remarks>
        Local
    }

    #endregion
}