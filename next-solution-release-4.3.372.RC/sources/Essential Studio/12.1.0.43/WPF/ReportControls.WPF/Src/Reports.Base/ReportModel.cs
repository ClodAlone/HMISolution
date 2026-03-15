//-------------------------------------------------------------------------------------------------
// <copyright file="PageModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------


namespace Syncfusion.RDL.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Net;
    using Syncfusion.RDL.Internal;
    using Syncfusion.RDL.Data;
    using Syncfusion.RDL.DOM;
    using Syncfusion.RDL.ItemModel;
    using Syncfusion.RDL.ServerProcessor;
    using Syncfusion.RDL.Layout;

#if WINRT
    using Syncfusion.UI.Xaml.Reports; 
    using Syncfusion.Reports.Server;
    using Windows.UI.Xaml;    
#elif SILVERLIGHT
    using Syncfusion.Windows.Reports;
    using Syncfusion.Reports.Server;
    using System.Threading;
    using System.Windows;
#elif MVC
    using Syncfusion.Reports.Mvc;
#else
    using Syncfusion.Windows.Reports;
    using System.Windows;
#endif

    /// <summary>
    /// A page model. It helps to parse the rdl if required. Fills the Report objects. Gets the processed data for each and every data driven report items.
    /// </summary>
    internal class ReportModel
    {
        #region Members

        private string parseTime = string.Empty;
        private DateTime execTime = DateTime.Now;
        private ReportDefinition report = null;

        List<Syncfusion.RDL.DOM.DataSource> sharedDataSource = null;
        int sharedDataSourceIndex = -1;
        List<string> dataSources = new List<string>();

        List<Syncfusion.RDL.DOM.DataSet> sharedDataSet = null;
        int sharedDataSetIndex = -1;
        List<string> dataSets = new List<string>();
        internal static IEnumerable<DataSourceCredentials> dataSourceCredential;
        ServerReportProcessor reportingServer = null;
        internal bool isRender = false;
        internal bool isContainsToggle = false;
        internal bool isContainsPageBreak = false;
#if SILVERLIGHT
        internal static Dictionary<string, UIElement> UICollection = new Dictionary<string, UIElement>();
#endif
        ReportExp HeaderBehaviourExp;

        ReportExp FooterBehaviourExp;

        ReportExp BodyBehaviourExp;

        ReportExp ReportBehaviourExp;

        #endregion

        #region Events

        internal event ReportLoadedEventHandler ReportLoaded;
        internal event ReportItemEvaluatedHanlder ReportItemsEvaluated;
        internal event ReportExceptionHandler ReportException;
        internal event DataSourceUpdatedEventHandler DataSourceUpdated;
        internal event CredentialCheckCompletedEventHandler CrendentialCheckCompleted;
        internal event ConnectionValidatedEventHandler ConnectionValidated;
        internal event SubreportProcessingEventHandler SubreportProcessing;
        internal event DataSourceCredentialsUpdatedEventHandler DataSourceCredentialsUpdated;
        internal event DrillThroughEventHandler DrillThroughInnerReport;
        internal event ToggleChangedEventHandler ToggleChanged;
        internal event MouseWheelScrollHandler MouseWheelScrolling;


        #endregion

        #region Public Properties

#if SILVERLIGHT || WINRT
        internal string ReportServiceURL
        {
            get;
            set;
        }

        internal bool LoadInformationFromServer
        {
            get;
            set;
        }
#endif

        internal bool EnableVirtualEvaluation
        {
            get;
            set;
        }

        internal bool IsServerReport
        {
            get;
            set;
        }

        internal Dictionary<string, Stream> SubReportStream
        {
            get;
            set;
        }

        internal List<String> ExceptionDetails
        {
            get;
            set;
        }

        internal ServerReportProcessor ReportingServer
        {
            get
            {
                if (reportingServer == null)
                {
                    reportingServer = new ServerReportProcessor(this);
                }

                return reportingServer;
            }
        }

        internal string ParseTime
        {
            get
            {
                return this.parseTime;
            }
        }

        internal DateTime ExecutionTime
        {
            get
            {
                return this.execTime;
            }
            set
            {
                this.execTime = value;
            }
        }

        internal bool IsRDLC
        {
            get;
            set;
        }

        internal object DataSources
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the processed data.
        /// </summary>
        /// <value>The processed data.</value>
        internal ProcessedData ProcessedData
        {
            get;
            set;
        }

        internal byte[] ReportStreamByte
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the report.
        /// </summary>
        /// <value>The report.</value>
        internal Syncfusion.RDL.DOM.ReportDefinition Report
        {
            get
            {
                return this.report;
            }
            set
            {
                this.report = value;
            }
        }

        internal bool IsEvaluatedReport
        {
            get;
            set;
        }

        public bool HasReport
        {
            get
            {
                return this.Report != null;
            }
        }

        /// <summary>
        /// Gets or sets RDL(C) report path. This initializes the report viewer.
        /// </summary>
        /// <value>The report path.</value>
        public string ReportPath
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set the Report Server Url.
        /// </summary>
        /// <value>The Report server url.</value>
        public string ReportServerUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set the Report Server Credential.
        /// </summary>
        /// <value>The Report server Credential.</value>
        public ICredentials ReportServerCredential
        {
            get;
            set;
        }

        public ReportServerFormsCredential ReportServerFormsCredential
        {
            get;
            set;
        }

        public bool PreAuthenticate
        {
            get; 
            set;
        }

        internal ReportModelContentCollection BodyReportItemModels
        {
            get;
            set;
        }

        internal ReportModelContentCollection HeaderReportItemModels
        {
            get;
            set;
        }

        internal ReportModelContentCollection FooterReportItemModels
        {
            get;
            set;
        }

        internal ReportItems BodyReportItems
        {
            get;
            set;
        }

        internal ReportItems HeaderReportItems
        {
            get;
            set;
        }

        internal ReportItems FooterReportItems
        {
            get;
            set;
        }

        internal ReportParameters ReportParameters
        {
            get;
            set;
        }

        internal DataSources ReportDataSources
        {
            get;
            set;
        }

        internal DataSets ReportDataSets
        {
            get;
            set;
        }

        internal Page Page
        {
            get;
            set;
        }

        internal Body Body
        {
            get;
            set;
        }

        internal ExpressionEngine ExpressionEngine
        {
            get;
            set;
        }

        internal List<DataSetInfo> DataSetDetails
        {
            get;
            set;
        }

        internal List<ParameterInformation> ParameterDetails
        {
            get;
            set;
        }

        public ReportExpval HeaderBehaviour
        {
            get;
            set;
        }

        public ReportExpval FooterBehaviour
        {
            get;
            set;
        }

        public ReportExpval BodyBehaviour
        {
            get;
            set;
        }

        public ReportExpval ReportBehaviour
        {
            get;
            set;
        }

        internal DrillThroughModel Model
        {
            get;
            set;
        }

        internal PageModelFactory PageModelFactory
        {
            get;
            set;
        }

        internal DocumentMapModel MapModel
        {
            get;
            set;
        }

        internal object InnerReportParameter
        {
            get;
            set;
        }

        internal int LastPageIndex
        {
            get;
            set;
        }

        internal bool IsToggleState
        {
            get; 
            set;
        }

        #endregion

        #region Constructor

        internal ReportModel()
        {
            this.SubReportStream = new Dictionary<string, Stream>();
            this.Report = null;
            this.ReportDataSources = new DataSources();
            this.ReportParameters = new ReportParameters();
            this.ReportDataSets = new DataSets();
            this.ReportStreamByte = null;
            this.ExceptionDetails = new List<string>();
        }

        #endregion

        #region Intialization

        internal void UpdateReport()
        {
            if (this.Report != null)
            {
                if (this.Report.RDLType == RDLType.RDL2008)
                {
                    this.BodyReportItems = this.Report.Body.ReportItems;
                    this.Page = this.Report.Page;
                    this.Body = this.Report.Body;
                }
                else
                {
                    this.BodyReportItems = this.Report.ReportSections.First().Body.ReportItems;
                    this.Page = this.Report.ReportSections.First().Page;
                    this.Body = this.Report.ReportSections.First().Body;
                    this.Report.Width = this.Report.ReportSections.First().Width;
                }

                if (this.Page != null)
                {
                    if (this.Page.PageHeader != null)
                    {
                        this.HeaderReportItems = this.Page.PageHeader.ReportItems;
                    }

                    if (this.Page.PageFooter != null)
                    {
                        this.FooterReportItems = this.Page.PageFooter.ReportItems;
                    }
                }

                this.UpdateReportModel(false);
                this.ParseExpression();
                this.IntializeReportParametersDefaultValue();

                if (this.HeaderReportItemModels != null)
                {
                    this.HeaderReportItemModels.ReportItemLoaded += HeaderReportItemModels_ReportItemLoaded;
                    this.HeaderReportItemModels.Load();
                }
                else if (this.FooterReportItems != null)
                {
                    this.FooterReportItemModels.ReportItemLoaded += FooterReportItemModels_ReportItemLoaded;
                    this.FooterReportItemModels.Load();
                }
                else
                {
                    this.BodyReportItemModels.ReportItemLoaded += BodyReportItemModels_ReportItemLoaded;
                    this.BodyReportItemModels.Load();
                }
            }
            else
            {
                this.UpdateReportModel(true);
            }
        }

        void HeaderReportItemModels_ReportItemLoaded(object sender, ReportItemLoadedEventArgs e)
        {
            this.HeaderReportItemModels.ReportItemLoaded -= HeaderReportItemModels_ReportItemLoaded;

            if (this.FooterReportItems != null)
            {
                this.FooterReportItemModels.ReportItemLoaded += FooterReportItemModels_ReportItemLoaded;
                this.FooterReportItemModels.Load();
            }
            else
            {
                this.BodyReportItemModels.ReportItemLoaded += BodyReportItemModels_ReportItemLoaded;
                this.BodyReportItemModels.Load();
            }
        }

        void FooterReportItemModels_ReportItemLoaded(object sender, ReportItemLoadedEventArgs e)
        {
            this.FooterReportItemModels.ReportItemLoaded -= FooterReportItemModels_ReportItemLoaded;

            this.BodyReportItemModels.ReportItemLoaded += BodyReportItemModels_ReportItemLoaded;
            this.BodyReportItemModels.Load();
        }

        void BodyReportItemModels_ReportItemLoaded(object sender, ReportItemLoadedEventArgs e)
        {
            this.BodyReportItemModels.ReportItemLoaded -= BodyReportItemModels_ReportItemLoaded;
            this.RaiseReportLoadedEvent(new EventArgs());
        }

        void BodyReportItemModels_ReportItemEvaluated(object sender, ReportItemEvaluatedEventArgs e)
        {
            this.BodyReportItemModels.ReportItemEvaluated -= BodyReportItemModels_ReportItemEvaluated;
            this.RaiseReportItemsEvaluated(e);
        }

        void FooterReportItemModels_ReportItemEvaluated(object sender, ReportItemEvaluatedEventArgs e)
        {
            this.FooterReportItemModels.ReportItemEvaluated -= FooterReportItemModels_ReportItemEvaluated;

            if (this.BodyReportItemModels != null)
            {
                this.BodyReportItemModels.ReportItemEvaluated += BodyReportItemModels_ReportItemEvaluated;
                this.BodyReportItemModels.Evaluate();
            }
        }

        void HeaderReportItemModels_ReportItemEvaluated(object sender, ReportItemEvaluatedEventArgs e)
        {
            this.HeaderReportItemModels.ReportItemEvaluated -= HeaderReportItemModels_ReportItemEvaluated;

            if (this.FooterReportItems != null)
            {
                this.FooterReportItemModels.ReportItemEvaluated += FooterReportItemModels_ReportItemEvaluated;
                this.FooterReportItemModels.Evaluate();
            }
            else
            {
                this.BodyReportItemModels.ReportItemEvaluated += BodyReportItemModels_ReportItemEvaluated;
                this.BodyReportItemModels.Evaluate();
            }
        }

        internal void Evaluate()
        {
            try
            {
                this.HeaderBehaviour = new ReportExpval();
                if (this.HeaderBehaviourExp.Border != null)
                {
                    this.HeaderBehaviour.Border = this.GetBorderValue(this.HeaderBehaviourExp.Border);
                    this.HeaderBehaviour.BackgroudColor = this.ExpressionEngine.GetEvalExpressionString(this.HeaderBehaviourExp.BackgroudColor);
                    this.HeaderBehaviour.PrintOnFirstPage = this.HeaderBehaviourExp.PrintOnFirstPage;
                    this.HeaderBehaviour.PrintOnLastPage = this.HeaderBehaviourExp.PrintOnLastPage;
                    if (this.HeaderBehaviourExp.ImageValue != null)
                    {
                        this.HeaderBehaviour.ImageSource = (Source)Enum.Parse(typeof(Source), this.ExpressionEngine.GetEvalExpressionString(this.HeaderBehaviourExp.ImageSource), true);
                        this.HeaderBehaviour.ImageValue = this.ExpressionEngine.GetEvalExpressionString(this.HeaderBehaviourExp.ImageValue);
                    }
                }

                this.FooterBehaviour = new ReportExpval();
                if (this.FooterBehaviourExp.Border != null)
                {
                    this.FooterBehaviour.Border = this.GetBorderValue(this.FooterBehaviourExp.Border);
                    this.FooterBehaviour.BackgroudColor = this.ExpressionEngine.GetEvalExpressionString(this.FooterBehaviourExp.BackgroudColor);
                    this.FooterBehaviour.PrintOnFirstPage = this.FooterBehaviourExp.PrintOnFirstPage;
                    this.FooterBehaviour.PrintOnLastPage = this.FooterBehaviourExp.PrintOnLastPage;
                    if (this.FooterBehaviourExp.ImageValue != null)
                    {
                        this.FooterBehaviour.ImageSource = (Source)Enum.Parse(typeof(Source), this.ExpressionEngine.GetEvalExpressionString(this.FooterBehaviourExp.ImageSource), true);
                        this.FooterBehaviour.ImageValue = this.ExpressionEngine.GetEvalExpressionString(this.FooterBehaviourExp.ImageValue);
                    }
                }

                this.BodyBehaviour = new ReportExpval();
                if (this.BodyBehaviourExp.Border != null)
                {
                    this.BodyBehaviour.Border = this.GetBorderValue(this.BodyBehaviourExp.Border);
                    this.BodyBehaviour.BackgroudColor = this.ExpressionEngine.GetEvalExpressionString(this.BodyBehaviourExp.BackgroudColor);
                    if (this.BodyBehaviourExp.ImageValue != null)
                    {
                        this.BodyBehaviour.ImageSource = (Source)Enum.Parse(typeof(Source), this.ExpressionEngine.GetEvalExpressionString(this.BodyBehaviourExp.ImageSource), true);
                        this.BodyBehaviour.ImageValue = this.ExpressionEngine.GetEvalExpressionString(this.BodyBehaviourExp.ImageValue);
                    }
                }

                this.ReportBehaviour = new ReportExpval();
                if (this.ReportBehaviourExp.Border != null)
                {
                    this.ReportBehaviour.Border = this.GetBorderValue(this.ReportBehaviourExp.Border);
                    this.ReportBehaviour.BackgroudColor = this.ExpressionEngine.GetEvalExpressionString(this.ReportBehaviourExp.BackgroudColor);
                    if (this.ReportBehaviourExp.ImageValue != null)
                    {
                        this.ReportBehaviour.ImageSource = (Source)Enum.Parse(typeof(Source), this.ExpressionEngine.GetEvalExpressionString(this.ReportBehaviourExp.ImageSource), true);
                        this.ReportBehaviour.ImageValue = this.ExpressionEngine.GetEvalExpressionString(this.ReportBehaviourExp.ImageValue);
                    }
                }

                if (this.HeaderReportItemModels != null)
                {
                    this.HeaderReportItemModels.ReportItemEvaluated += HeaderReportItemModels_ReportItemEvaluated;
                    this.HeaderReportItemModels.Evaluate();
                }
                else if (this.FooterReportItems != null)
                {
                    this.FooterReportItemModels.ReportItemEvaluated += FooterReportItemModels_ReportItemEvaluated;
                    this.FooterReportItemModels.Evaluate();
                }
                else
                {
                    this.BodyReportItemModels.ReportItemEvaluated += BodyReportItemModels_ReportItemEvaluated;
                    this.BodyReportItemModels.Evaluate();
                }
            }
            catch (Exception e)
            {
                if (this.ExceptionDetails == null)
                {
                    this.ExceptionDetails = new List<string>();
                }
                this.ExceptionDetails.Add("Getting follwoing exception while evaluate the expression " + e.Message);
            }
        }

        private BorderExpval GetBorderValue(BorderExp borderExp)
        {
            BorderExpval border = new BorderExpval();
            if (borderExp.Default != null)
            {
                border.Default = new BorderExpvalProperties();
                if (borderExp.Default.BorderBrush != null)
                    border.Default.BorderBrush = this.ExpressionEngine.GetEvalExpressionString(borderExp.Default.BorderBrush);
                if (borderExp.Default.BorderStyle != null)
                    border.Default.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.ExpressionEngine.GetEvalExpressionString(borderExp.Default.BorderStyle), true);
                if (borderExp.Default.Thickness != null)
                    border.Default.Thickness = new DOM.Size(this.ExpressionEngine.GetEvalExpressionString(borderExp.Default.Thickness)).FloatValue;
            }
            if (borderExp.LeftBorder != null)
            {
                border.LeftBorder = new BorderExpvalProperties();
                if (borderExp.LeftBorder.BorderBrush != null)
                    border.LeftBorder.BorderBrush = this.ExpressionEngine.GetEvalExpressionString(borderExp.LeftBorder.BorderBrush);
                if (borderExp.LeftBorder.BorderStyle != null)
                    border.LeftBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.ExpressionEngine.GetEvalExpressionString(borderExp.LeftBorder.BorderStyle), true);
                if (borderExp.LeftBorder.Thickness != null)
                    border.LeftBorder.Thickness = new DOM.Size(this.ExpressionEngine.GetEvalExpressionString(borderExp.LeftBorder.Thickness)).FloatValue;
            }
            if (borderExp.TopBorder != null)
            {
                border.TopBorder = new BorderExpvalProperties();
                if (borderExp.TopBorder.BorderBrush != null)
                    border.TopBorder.BorderBrush = this.ExpressionEngine.GetEvalExpressionString(borderExp.TopBorder.BorderBrush);
                if (borderExp.TopBorder.BorderStyle != null)
                    border.TopBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.ExpressionEngine.GetEvalExpressionString(borderExp.TopBorder.BorderStyle), true);
                if (borderExp.TopBorder.Thickness != null)
                    border.TopBorder.Thickness = new DOM.Size(this.ExpressionEngine.GetEvalExpressionString(borderExp.TopBorder.Thickness)).FloatValue;
            }
            if (borderExp.RightBorder != null)
            {
                border.RightBorder = new BorderExpvalProperties();
                if (borderExp.RightBorder.BorderBrush != null)
                    border.RightBorder.BorderBrush = this.ExpressionEngine.GetEvalExpressionString(borderExp.RightBorder.BorderBrush);
                if (borderExp.RightBorder.BorderStyle != null)
                    border.RightBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.ExpressionEngine.GetEvalExpressionString(borderExp.RightBorder.BorderStyle), true);
                if (borderExp.RightBorder.Thickness != null)
                    border.RightBorder.Thickness = new DOM.Size(this.ExpressionEngine.GetEvalExpressionString(borderExp.RightBorder.Thickness)).FloatValue;
            }
            if (borderExp.BottomBorder != null)
            {
                border.BottomBorder = new BorderExpvalProperties();
                if (borderExp.BottomBorder.BorderBrush != null)
                    border.BottomBorder.BorderBrush = this.ExpressionEngine.GetEvalExpressionString(borderExp.BottomBorder.BorderBrush);
                if (borderExp.BottomBorder.BorderStyle != null)
                    border.BottomBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.ExpressionEngine.GetEvalExpressionString(borderExp.BottomBorder.BorderStyle), true);
                if (borderExp.BottomBorder.Thickness != null)
                    border.BottomBorder.Thickness = new DOM.Size(this.ExpressionEngine.GetEvalExpressionString(borderExp.BottomBorder.Thickness)).FloatValue;
            }

            return border;
        }

        internal void UpdateSize()
        {
            if (this.HeaderReportItemModels != null)
            {
                this.HeaderReportItemModels.UpdateSize();
            }

            if (this.FooterReportItemModels != null)
            {
                this.FooterReportItemModels.UpdateSize();
            }

            if (this.BodyReportItemModels != null)
            {
                this.BodyReportItemModels.UpdateSize();
            }
        }

        internal void UpdateReportModel(bool reset)
        {
            if (reset)
            {
                if (this.ExpressionEngine != null)
                {
                    this.ExpressionEngine.ResetExpressionCache();
                }

                this.ExpressionEngine = null;
                this.DataSetDetails = null;
                this.ParameterDetails = null;
                this.ProcessedData = null;
            }
            else
            {
                if (this.ExpressionEngine != null)
                {
                    this.ExpressionEngine.ResetExpressionCache();
                }

                this.ExpressionEngine = new ExpressionEngine(this);
                this.DataSetDetails = new List<DataSetInfo>();
                this.ParameterDetails = new List<ParameterInformation>();
                this.IntiateReportInformations();
                this.ProcessedData = new ProcessedData(this);
            }
        }

        void ParseExpression()
        {
            try
            {
                if (this.ReportDataSources != null)
                {
                    foreach (var dataSource in this.ReportDataSources.Where(data => data.ConnectionProperties != null))
                    {
                        dataSource.ConnectionProperties.ConnectString = this.ExpressionEngine.GetExpressionKey(dataSource.ConnectionProperties.ConnectString);
                    }
                }

                if (this.ReportDataSets != null)
                {
                    foreach (var dataSet in this.ReportDataSets)
                    {
                        if (dataSet.Filters != null)
                        {
                            if (this.ProcessedData.ExpFilters == null)
                            {
                                this.ProcessedData.ExpFilters = new Dictionary<string, List<ExpFilter>>();
                            }
                            this.ProcessedData.ExpFilters.Add(dataSet.Name, this.ParseFilters(dataSet.Filters, dataSet.Name, true));
                        }

                        if (!string.IsNullOrEmpty(dataSet.Query.CommandText) && dataSet.Query.CommandText.StartsWith("="))
                        {
                            dataSet.Query.CommandText = this.ExpressionEngine.GetExpressionKey(dataSet.Query.CommandText);
                        }
                        if (dataSet.Query.QueryParameters != null)
                        {
                            foreach (var queryParameter in dataSet.Query.QueryParameters)
                            {
                                queryParameter.Value = this.ExpressionEngine.GetExpressionKey(queryParameter.Value);
                            }
                        }

                        if (dataSet.Fields != null)
                        {
                            foreach (var field in dataSet.Fields.Where(f => f.DataField == null))
                            {
                                field.Value = this.ExpressionEngine.GetExpressionKey(field.Value, dataSet.Name, true);
                            }
                        }
                    }
                }

                if (this.ReportParameters != null)
                {
                    foreach (var reportParameter in this.ReportParameters)
                    {
                        if (reportParameter.DefaultValue != null && reportParameter.DefaultValue.Values != null && reportParameter.DefaultValue.DataSetReference == null)
                        {
                            Values expression = new Values();

                            foreach (var defaultValue in reportParameter.DefaultValue.Values)
                            {
                                expression.Add(this.ExpressionEngine.GetExpressionKey(defaultValue));
                            }

                            reportParameter.DefaultValue.Values = expression;
                        }
                    }
                }

                string dataSetName = null;

                if (this.ReportDataSets != null && this.ReportDataSets.Count == 1)
                {
                    dataSetName = this.ReportDataSets.First().Name;
                }

                this.ReportBehaviourExp = new ReportExp();
                if (this.Page.Style != null)
                {
                    this.ReportBehaviourExp.Border = this.GetBorderFromExp(this.Page.Style, dataSetName);
                    this.ReportBehaviourExp.BackgroudColor = this.ExpressionEngine.GetExpressionKey(this.Page.Style.BackgroundColor, dataSetName);
                    if (this.Page.Style.BackgroundImage != null)
                    {
                        this.ReportBehaviourExp.ImageValue = this.ExpressionEngine.GetExpressionKey(this.Page.Style.BackgroundImage.Value, dataSetName);
                        this.ReportBehaviourExp.ImageSource = this.Page.Style.BackgroundImage.Source.ToString();
                    }
                }
                if (this.HeaderReportItems != null)
                {
                    this.HeaderReportItemModels = new ReportModelContentCollection();

                    foreach (var reportItem in this.HeaderReportItems)
                    {
                        IReportItemModeler model = this.GetModel(reportItem, false, dataSetName);
                        this.HeaderReportItemModels.Add(model);

                        if (model.ModelType == ModelType.RectangleModel)
                        {
                            foreach (IReportItemModeler itemModel in model.ReportItemModelers)
                            {
                                this.HeaderReportItemModels.Add(itemModel);
                            }
                        }
                    }
                }

                this.HeaderBehaviourExp = new ReportExp();
                if (this.Page.PageHeader != null && this.Page.PageHeader.Style != null)
                {
                    this.HeaderBehaviourExp.Border = this.GetBorderFromExp(this.Page.PageHeader.Style, dataSetName);
                    this.HeaderBehaviourExp.BackgroudColor = this.ExpressionEngine.GetExpressionKey(this.Page.PageHeader.Style.BackgroundColor, dataSetName);
                    this.HeaderBehaviourExp.PrintOnFirstPage = this.Page.PageHeader.PrintOnFirstPage;
                    this.HeaderBehaviourExp.PrintOnLastPage = this.Page.PageHeader.PrintOnLastPage;
                    if (this.Page.PageHeader.Style.BackgroundImage != null)
                    {
                        this.HeaderBehaviourExp.ImageValue = this.ExpressionEngine.GetExpressionKey(this.Page.PageHeader.Style.BackgroundImage.Value, dataSetName);
                        this.HeaderBehaviourExp.ImageSource = this.Page.PageHeader.Style.BackgroundImage.Source.ToString();
                    }
                }
                if (this.FooterReportItems != null)
                {
                    this.FooterReportItemModels = new ReportModelContentCollection();

                    foreach (var reportItem in this.FooterReportItems)
                    {
                        IReportItemModeler model = this.GetModel(reportItem, false, dataSetName);
                        this.FooterReportItemModels.Add(model);

                        if (model.ModelType == ModelType.RectangleModel)
                        {
                            foreach (IReportItemModeler itemModel in model.ReportItemModelers)
                            {
                                this.FooterReportItemModels.Add(itemModel);
                            }
                        }
                    }
                }
                this.FooterBehaviourExp = new ReportExp();
                if (this.Page.PageFooter != null && this.Page.PageFooter.Style != null)
                {
                    this.FooterBehaviourExp.Border = this.GetBorderFromExp(this.Page.PageFooter.Style, dataSetName);
                    this.FooterBehaviourExp.BackgroudColor = this.ExpressionEngine.GetExpressionKey(this.Page.PageFooter.Style.BackgroundColor, dataSetName);
                    this.FooterBehaviourExp.PrintOnFirstPage = this.Page.PageFooter.PrintOnFirstPage;
                    this.FooterBehaviourExp.PrintOnLastPage = this.Page.PageFooter.PrintOnLastPage;
                    if (this.Page.PageFooter.Style.BackgroundImage != null)
                    {
                        this.FooterBehaviourExp.ImageValue = this.ExpressionEngine.GetExpressionKey(this.Page.PageFooter.Style.BackgroundImage.Value, dataSetName);
                        this.FooterBehaviourExp.ImageSource = this.Page.PageFooter.Style.BackgroundImage.Source.ToString();
                    }
                }
                if (this.BodyReportItems != null)
                {
                    this.BodyReportItemModels = new ReportModelContentCollection();

                    foreach (var reportItem in this.BodyReportItems)
                    {
                        IReportItemModeler model = this.GetModel(reportItem, false, dataSetName);
                        this.BodyReportItemModels.Add(model);

                        if (model.ModelType == ModelType.RectangleModel)
                        {
                            foreach (IReportItemModeler itemModel in model.ReportItemModelers)
                            {
                                this.BodyReportItemModels.Add(itemModel);
                            }
                        }
                    }
                }

                this.BodyBehaviourExp = new ReportExp();
                if (this.Body.Style != null)
                {
                    this.BodyBehaviourExp.Border = this.GetBorderFromExp(this.Body.Style, dataSetName);
                    this.BodyBehaviourExp.BackgroudColor = this.ExpressionEngine.GetExpressionKey(this.Body.Style.BackgroundColor, dataSetName);
                    if (this.Body.Style.BackgroundImage != null)
                    {
                        this.BodyBehaviourExp.ImageValue = this.ExpressionEngine.GetExpressionKey(this.Body.Style.BackgroundImage.Value, dataSetName);
                        this.BodyBehaviourExp.ImageSource = this.Body.Style.BackgroundImage.Source.ToString();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private BorderExp GetBorderFromExp(Syncfusion.RDL.DOM.Style style, string dataSetName)
        {
            BorderExp border = new BorderExp();
            if (style.Border != null)
            {
                border.Default = new BorderExpProperties();
                border.Default.BorderBrush = this.ExpressionEngine.GetExpressionKey(style.Border.Color, dataSetName);
                border.Default.BorderStyle = this.ExpressionEngine.GetExpressionKey(style.Border.Style, dataSetName);
                if (style.Border.Width != null)
                {
                    border.Default.Thickness = this.ExpressionEngine.GetExpressionKey(style.Border.Width.size, dataSetName);
                }
            }
            if (style.LeftBorder != null)
            {
                border.LeftBorder = new BorderExpProperties();
                border.LeftBorder.BorderBrush = this.ExpressionEngine.GetExpressionKey(style.LeftBorder.Color, dataSetName);
                border.LeftBorder.BorderStyle = this.ExpressionEngine.GetExpressionKey(style.LeftBorder.Style, dataSetName);
                if (style.LeftBorder.Width != null)
                {
                    border.LeftBorder.Thickness = this.ExpressionEngine.GetExpressionKey(style.LeftBorder.Width.size, dataSetName);
                }
            }
            if (style.RightBorder != null)
            {
                border.RightBorder = new BorderExpProperties();
                border.RightBorder.BorderBrush = this.ExpressionEngine.GetExpressionKey(style.RightBorder.Color, dataSetName);
                border.RightBorder.BorderStyle = this.ExpressionEngine.GetExpressionKey(style.RightBorder.Style, dataSetName);
                if (style.RightBorder.Width != null)
                {
                    border.RightBorder.Thickness = this.ExpressionEngine.GetExpressionKey(style.RightBorder.Width.size, dataSetName);
                }
            }
            if (style.TopBorder != null)
            {
                border.TopBorder = new BorderExpProperties();
                border.TopBorder.BorderBrush = this.ExpressionEngine.GetExpressionKey(style.TopBorder.Color, dataSetName);
                border.TopBorder.BorderStyle = this.ExpressionEngine.GetExpressionKey(style.TopBorder.Style, dataSetName);
                if (style.TopBorder.Width != null)
                {
                    border.TopBorder.Thickness = this.ExpressionEngine.GetExpressionKey(style.TopBorder.Width.size, dataSetName);
                }
            }
            if (style.BottomBorder != null)
            {
                border.BottomBorder = new BorderExpProperties();
                border.BottomBorder.BorderBrush = this.ExpressionEngine.GetExpressionKey(style.BottomBorder.Color, dataSetName);
                border.BottomBorder.BorderStyle = this.ExpressionEngine.GetExpressionKey(style.BottomBorder.Style, dataSetName);
                if (style.BottomBorder.Width != null)
                {
                    border.BottomBorder.Thickness = this.ExpressionEngine.GetExpressionKey(style.BottomBorder.Width.size, dataSetName);
                }
            }

            return border;
        }

        public IReportItemModeler GetModel(ReportItem reportItem, bool isTablixChild, string dataSetName)
        {
            try
            {
                IReportItemModeler model = null;

                if (reportItem is TextBox)
                {
                    TextboxModel itemModel = new TextboxModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is Image)
                {
                    ImageModel itemModel = new ImageModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is Line)
                {
                    LineModel itemModel = new LineModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is SubReport)
                {
                    SubReportModel itemModel = new SubReportModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is Rectangle)
                {
                    RectangleModel itemModel = new RectangleModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is GaugePanel)
                {
                    GaugeModel itemModel = new GaugeModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is Chart)
                {
                    ChartModel itemModel = new ChartModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
                else if (reportItem is Tablix)
                {
                    TablixModel itemModel = new TablixModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
#if !SyncfusionFramework3_5
                else if (reportItem is Map)
                {
                    MapModel itemModel = new MapModel(reportItem, this, isTablixChild, dataSetName);
                    model = itemModel;
                }
#endif

                return model;
            }
            catch
            {
                if (this.ExceptionDetails == null)
                {
                    this.ExceptionDetails = new List<string>();
                }

                this.ExceptionDetails.Add("Getting exception while rendering following Report Item : " + reportItem.Name);
                return null;
            }
        }

        void IntializeReportParametersDefaultValue()
        {
            try
            {
                if (this.ReportParameters != null && this.ParameterDetails != null)
                {
                    this.ProcessedData.InitilizeDatasetDefaultParameterValues(this.ParameterDetails.Select(p => p.Name));

                    foreach (Syncfusion.RDL.DOM.ReportParameter reportParam in this.ReportParameters)
                    {
                        ParameterInformation paramInfo = this.ParameterDetails.Where(p => p.Name == reportParam.Name ).First();

                        paramInfo.Value = paramInfo.Value.Where(val => val != null && !String.IsNullOrEmpty(val.ToString())).ToList();
                      
                        if (paramInfo.Hidden)
                        {
                            paramInfo.Value.Clear();
                        }

                        if (reportParam.DefaultValue != null && reportParam.DefaultValue.Values != null && reportParam.DefaultValue.Values.Count > 0 && paramInfo.Value.Count == 0)
                        {
                            foreach (var obj in reportParam.DefaultValue.Values)
                            {
                                object value = this.ExpressionEngine.GetEvalExpression(obj);

                                if (reportParam.DataType == DataTypes.DateTime && value != null)
                                {
                                    DateTime date = value is DateTime ?  (DateTime)value : DateTime.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                    value = (date.Hour == 0 && date.Minute == 0 && date.Second == 0) ? date.ToString("d") : value;
                                    paramInfo.Value.Add(value);
                                }
                                else
                                {
                                    if (!paramInfo.Value.Contains(value) && value!=null && !string.IsNullOrEmpty(value.ToString()))
                                    {
                                        paramInfo.Value.Add(value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.ExceptionDetails.Add("Getting following exception while initialize  the reportparameter " + e.Message);
            }
        }

        void IntiateReportInformations()
        {
            try
            {
                if (this.ReportParameters != null)
                {
                    foreach (Syncfusion.RDL.DOM.ReportParameter reportParam in this.ReportParameters)
                    {
                        ParameterInformation paramInfo = new ParameterInformation();
                        paramInfo.Name = reportParam.Name;
                        paramInfo.Prompt = reportParam.Prompt;
                        paramInfo.Value = new List<object>();
                        paramInfo.Label = new List<object>();
                        paramInfo.IsMultiValue = reportParam.MultiValue;
                        paramInfo.Hidden = reportParam.Hidden;
                        paramInfo.Nullable = reportParam.Nullable;
                        paramInfo.DataType = (ParamType)Enum.Parse(typeof(ParamType), reportParam.DataType.ToString(), true);

                        List<string> dataSets = new List<string>();

                        if (reportParam.ValidValues != null && reportParam.ValidValues.DataSetReference != null)
                        {
                            dataSets.Add(reportParam.ValidValues.DataSetReference.DataSetName);
                        }

                        if (reportParam.DefaultValue != null && reportParam.DefaultValue.DataSetReference != null)
                        {
                            dataSets.Add(reportParam.DefaultValue.DataSetReference.DataSetName);
                        }

                        paramInfo.DependentDataSets = dataSets;
                        paramInfo.DependentParameters = new List<string>();
                        this.ParameterDetails.Add(paramInfo);
                    }
                }

                if (this.ReportDataSets != null)
                {
                    List<DataSetInfo> priorityDataSetDetails = new List<DataSetInfo>();
                    List<string> prioritizedDataSetNames = new List<string>();

                    foreach (DataSet dataSet in this.ReportDataSets)
                    {
                        List<string> parameters = new List<string>();
                        List<string> dataSets = new List<string>();

                        if (dataSet.Query.QueryParameters != null)
                        {
                            foreach (QueryParameter QP in dataSet.Query.QueryParameters)
                            {
                                string value = QP.Value;

                                if (value.Contains("Parameters!") && (value.Contains(".Value") || value.Contains(".Label")))
                                {
                                    int indexOfExclamatory = value.IndexOf("!");
                                    int indexOfDot = value.IndexOf(".");
                                    int stringLength = (indexOfDot - (indexOfExclamatory)) - 1;
                                    int startingIndex = indexOfExclamatory + 1;
                                    string fieldName = value.Substring(startingIndex, stringLength);
                                    parameters.Add(fieldName);

                                    var parameterInfo = (from para in ParameterDetails
                                                         where (para.Name.Equals(fieldName))
                                                         select para).FirstOrDefault();

                                    foreach (var set in parameterInfo.DependentDataSets)
                                    {
                                        if (!dataSets.Contains(set))
                                        {
                                            dataSets.Add(set);
                                        }
                                    }
                                }
                            }
                        }

                        if (dataSet.Filters != null)
                        {
                            foreach (var filter in dataSet.Filters)
                            {
                               var value= filter.FilterValues.FirstOrDefault().Value;
                                if (value.Contains("Parameters!") && (value.Contains(".Value") || value.Contains(".Label")))
                                {
                                    int indexOfExclamatory = value.IndexOf("!");
                                    int indexOfDot = value.IndexOf(".");
                                    int stringLength = (indexOfDot - (indexOfExclamatory)) - 1;
                                    int startingIndex = indexOfExclamatory + 1;
                                    string fieldName = value.Substring(startingIndex, stringLength);
                                    parameters.Add(fieldName);

                                    var parameterInfo = (from para in ParameterDetails
                                                         where (para.Name.Equals(fieldName))
                                                         select para).FirstOrDefault();

                                    foreach (var set in parameterInfo.DependentDataSets)
                                    {
                                        if (!dataSets.Contains(set))
                                        {
                                            dataSets.Add(set);
                                        }
                                    }
                                }

                               
                            }
                        }

                        DataSetInfo information = new DataSetInfo();
                        information.Name = dataSet.Name;
                        information.DependentParameters = parameters;
                        information.DependentDataSets = dataSets;

                        if (parameters.Count > 0)
                        {
                            var paraInfo = from para in ParameterDetails
                                           where (para.DependentDataSets.Contains(dataSet.Name))
                                           select para;

                            foreach (var para in paraInfo)
                            {
                                foreach (string paramName in parameters)
                                {
                                    if (!para.DependentParameters.Contains(paramName))
                                    {
                                        para.DependentParameters.Add(paramName);
                                    }
                                }
                            }
                            priorityDataSetDetails.Add(information);
                        }
                        else
                        {
                            prioritizedDataSetNames.Add(information.Name);
                            this.DataSetDetails.Add(information);
                        }
                    }

                    while (DataSetDetails.Count != this.ReportDataSets.Count)
                    {
                        List<DataSetInfo> compltedInfo = new List<DataSetInfo>();
                        foreach (var info in priorityDataSetDetails)
                        {
                            int dependedfailure = (from dataSet in info.DependentDataSets
                                                   where (!prioritizedDataSetNames.Contains(dataSet))
                                                   select info).Count();

                            if (dependedfailure == 0)
                            {
                                compltedInfo.Add(info);
                            }
                        }

                        foreach (var info in compltedInfo)
                        {
                            priorityDataSetDetails.Remove(info);
                            this.DataSetDetails.Add(info);
                            prioritizedDataSetNames.Add(info.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.ExceptionDetails.Add("Getting following exception while initialize the report " + e.Message);
            }

        }

        #endregion

        #region Filters

        internal List<ExpFilter> ParseFilters(DOM.Filters filters, string dataset, bool isChild)
        {
            List<ExpFilter> expFilters = new List<ExpFilter>();
            const string startTag = "=IIf(";
            const string endTag = ",true,false)";

            if (filters == null)
            {
                return expFilters;
            }
            foreach (var filter in filters)
            {
                ExpFilter expFilter = new ExpFilter();
                string vbExp = startTag;
                bool isValueFlag = false;
                bool isExpValue = !filter.FilterValues.First().Value.StartsWith("=");
                vbExp += GetFilderExpression(filter.FilterExpression);
                switch (filter.Operator)
                {
                    case FilterOperators.LessThan:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += ("<\"" + GetFilderExpression(filter.FilterValues.First().Value) + "\"");
                        }
                        else
                        {
                            vbExp += ("<" + GetFilderExpression(filter.FilterValues.First().Value));
                        }
                        break;
                    case FilterOperators.LessThanOrEqual:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += ("<=\"" + GetFilderExpression(filter.FilterValues.First().Value) + "\"");
                        }
                        else
                        {
                            vbExp += ("<=" + GetFilderExpression(filter.FilterValues.First().Value));
                        }
                        break;
                    case FilterOperators.GreaterThan:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += (">\"" + GetFilderExpression(filter.FilterValues.First().Value) + "\"");
                        }
                        else
                        {
                            vbExp += (">" + GetFilderExpression(filter.FilterValues.First().Value));
                        }
                        break;
                    case FilterOperators.GreaterThanOrEqual:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += (">=\"" + GetFilderExpression(filter.FilterValues.First().Value) + "\"");
                        }
                        else
                        {
                            vbExp += (">=" + GetFilderExpression(filter.FilterValues.First().Value));
                        }
                        break;
                    case FilterOperators.Equal:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += ("=\"" + GetFilderExpression(filter.FilterValues.First().Value) + "\"");
                        }
                        else
                        {
                            vbExp += ("=" + GetFilderExpression(filter.FilterValues.First().Value));
                        }
                        break;
                    case FilterOperators.NotEqual:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += ("<>\"" + GetFilderExpression(filter.FilterValues.First().Value) + "\"");
                        }
                        else
                        {
                            vbExp += ("<>" + GetFilderExpression(filter.FilterValues.First().Value));
                        }
                        break;
                    case FilterOperators.Between:
                        if (filter.FilterValues.First().DataType == DataTypes.String && isExpValue)
                        {
                            vbExp += (">\"" + GetFilderExpression(filter.FilterValues[0].Value) + "\" and " + GetFilderExpression(filter.FilterExpression) + "<\"" + GetFilderExpression(GetFilderExpression(filter.FilterValues[1].Value)) + "\"");
                        }
                        else
                        {
                            vbExp += (">" + GetFilderExpression(filter.FilterValues[0].Value) + " and " + GetFilderExpression(filter.FilterExpression) + "<" + GetFilderExpression(GetFilderExpression(filter.FilterValues[1].Value)));
                        }
                        break;
                    case FilterOperators.In:
                        isValueFlag = true;
                        break;
                    case FilterOperators.TopN:
                        isValueFlag = true;
                        break;
                    case FilterOperators.TopPercent:
                        isValueFlag = true;
                        break;
                    case FilterOperators.BottomN:
                        isValueFlag = true;
                        break;
                    case FilterOperators.BottomPercent:
                        isValueFlag = true;
                        break;
                }
                vbExp += endTag;
                if (isValueFlag)
                {
                    expFilter.FilterExp = this.ExpressionEngine.GetExpressionKey(filter.FilterExpression, dataset, isChild);
                    expFilter.FieldValue = this.ExpressionEngine.GetExpressionKey(filter.FilterValues.First().Value, dataset, isChild);
                    expFilter.OperatorType = filter.Operator;
                    if (expFilter.OperatorType == FilterOperators.In && filter.FilterValues.Count > 1)
                    {
                        string[] param = (from ftr in filter.FilterValues where ftr.Value != null select ftr.Value).ToArray();
                        expFilter.FieldValue = string.Join(",", param);
                    }
                }
                else
                {
                    expFilter.FilterExp = this.ExpressionEngine.GetExpressionKey(vbExp, dataset, isChild);
                    expFilter.OperatorType = filter.Operator;
                }
                expFilter.IsSelectType = isValueFlag;
                expFilters.Add(expFilter);
            }
            return expFilters;
        }

        string GetFilderExpression(string expression)
        {
            if (expression.StartsWith("="))
            {
                expression = expression.Substring(1, expression.Length - 1);
            }
            return expression;
        }

        #endregion

        #region ReportItem evaluation

        public void EvaluateReport()
        {
        }

        #endregion

        #region Helper Methods

        internal void InitilizeReport()
        {
            this.DataSourceUpdate();
            this.IntializeReportParametersDefaultValue();
        }

        void DataSourceUpdate()
        {
            if (this.ReportDataSets.Count > 0 && this.ReportDataSources.Count > 0)
            {
                if (this.IsRDLC)
                {
                    this.ProcessedData.UpdateData(this.DataSources);
                    this.RaiseDataSourceUpdatedEvent(new EventArgs());
                }
                else
                {
                    this.ProcessedData.DataSourceUpdated += new DataSourceUpdatedEventHandler(ProcessedData_DataSourceUpdated);
                    this.ProcessedData.UpdateData();
                }
            }
            else
            {
                this.RaiseDataSourceUpdatedEvent(new EventArgs());
            }
        }

        void ProcessedData_DataSourceUpdated(object sender, EventArgs e)
        {
            this.ProcessedData.DataSourceUpdated -= new DataSourceUpdatedEventHandler(ProcessedData_DataSourceUpdated);
            this.RaiseDataSourceUpdatedEvent(new EventArgs());
        }

        internal void UpdateReportParameters()
        {
            if (this.IsRDLC)
            {
                this.ProcessedData.UpdateParameterDatasets(this.DataSources);
                this.RaiseDataSourceUpdatedEvent(new EventArgs());
            }
            else
            {
                this.ProcessedData.DataSourceUpdated += new DataSourceUpdatedEventHandler(ProcessedData_DataSourceUpdated);
                this.ProcessedData.UpdateParameterDatasets();
            }
        }

        internal List<RDL.DOM.DataSource> GetCredentialDataSources()
        {
            var dataSources = from dataSource in this.ReportDataSources
                              where !dataSource.ConnectionProperties.IntegratedSecurity && dataSource.ConnectionProperties.Prompt != null
                              && dataSource.ConnectionProperties.UserName == null && !this.dataSources.Contains(dataSource.Name)
                              select dataSource;

            if (dataSources.Count() > 0)
            {
                return dataSources.ToList();
            }

            return null;
        }

#if SILVERLIGHT
        internal void ValidateConnection(ReportingConnectionEventArgs connectionArgs)
        {
            var validationDataSource = (from datasource in this.ReportDataSources 
                                        where connectionArgs.DataSourceName.Equals(datasource.Name) 
                                        select datasource).FirstOrDefault();

            string connectionString = validationDataSource.ConnectionProperties.ConnectString + "; User ID=" + connectionArgs.UserName + "; Password=" + connectionArgs.Password;

            bool isValidConnection = false;

            this.ReportingServer.IsValidConnection(connectionString, validationDataSource.ConnectionProperties.DataProvider);

            IsValidConnectionEventHandler handler = null;

            System.Threading.ManualResetEvent resetEvent = new System.Threading.ManualResetEvent(false);

            handler = (sender, e) =>
                {
                    this.ReportingServer.IsValidConnectionCompleted -= handler;
                    isValidConnection = e.Result;

                    if (isValidConnection)
                    {
                        validationDataSource.ConnectionProperties.UserName = connectionArgs.UserName;
                        validationDataSource.ConnectionProperties.PassWord = connectionArgs.Password;
                    }
                    else
                    {
                        validationDataSource.ConnectionProperties.UserName = null;
                        validationDataSource.ConnectionProperties.PassWord = null;
                    }

                    ReportingConnectionValidationEventArgs args = new ReportingConnectionValidationEventArgs();
                    args.Success = isValidConnection;
                    args.DataSourceName = connectionArgs.DataSourceName;

                    this.RaiseConnectionValidatedEvent(args);
                    resetEvent.Set();
                };

            this.ReportingServer.IsValidConnectionCompleted += handler;
        }

#else
        internal bool ValidateConnection(ReportingConnectionEventArgs connectionArgs)
        {
            var validationDataSource = (from datasource in this.ReportDataSources
                                        where connectionArgs.DataSourceName.Equals(datasource.Name)
                                        select datasource).FirstOrDefault();

            string connectionString = validationDataSource.ConnectionProperties.ConnectString + "; User ID=" + connectionArgs.UserName + "; Password=" + connectionArgs.Password;

            bool isValidConnection = false;

            if (validationDataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLServer, StringComparison.InvariantCultureIgnoreCase)
                    || validationDataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLAzure, StringComparison.InvariantCultureIgnoreCase))
            {
                isValidConnection = new SqlDataProvider().ChecksWhetherValidConnection(connectionString);
            }
            else if (validationDataSource.ConnectionProperties.DataProvider.Equals(DataProviders.ORACLE, StringComparison.InvariantCultureIgnoreCase))
            {
                isValidConnection = new OracleDataProvider().ChecksWhetherValidConnection(connectionString);
            }

            if (isValidConnection)
            {
                validationDataSource.ConnectionProperties.UserName = connectionArgs.UserName;
                validationDataSource.ConnectionProperties.PassWord = connectionArgs.Password;
            }
            else
            {
                validationDataSource.ConnectionProperties.UserName = null;
                validationDataSource.ConnectionProperties.PassWord = null;
            }

            ReportingConnectionValidationEventArgs args = new ReportingConnectionValidationEventArgs();
            args.Success = isValidConnection;
            args.DataSourceName = validationDataSource.Name;

            this.RaiseConnectionValidatedEvent(args);

            return isValidConnection;
        }
#endif

        #endregion

        #region LoadReports

        public void LoadReport(Stream fileStream)
        {
            this.Report = null;
            this.ReportStreamByte = null;

            using (XmlReader reader = XmlReader.Create(fileStream))
            {
                reader.MoveToContent();
                string Namespace = reader.NamespaceURI;
                string Version = (Regex.IsMatch(Namespace, @"\d{4}") ? Regex.Match(Namespace, @"\d{4}").Value : string.Empty);

                if (!string.IsNullOrEmpty(Version))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ReportDefinition), Namespace);
                    ReportDefinition o = (ReportDefinition)xs.Deserialize(reader);
                    if (o != null)
                    {
                        switch (Version)
                        {
                            case "2008":
                                o.RDLType = Syncfusion.RDL.DOM.RDLType.RDL2008;
                                break;
                            case "2010":
                                o.RDLType = Syncfusion.RDL.DOM.RDLType.RDL2010;
                                break;
                            default:
                                o.RDLType = Syncfusion.RDL.DOM.RDLType.None;
                                break;
                        }

                        byte[] buffer = new byte[fileStream.Length];
                        fileStream.Position = 0;
                        fileStream.Read(buffer, 0, buffer.Length);
#if !WINRT
                        fileStream.Close();
#endif

                        this.LoadReport(o);
                        this.ReportStreamByte = buffer;
                    }
                }
            }
        }

        internal void LoadReport(ReportDefinition report)
        {
            this.ReportDataSources = report.DataSources;
            this.ReportParameters = report.ReportParameters;
            this.ReportDataSets = report.DataSets;
            this.Report = report;

            if (this.IsRDLC)
            {
                this.UpdateReport();
            }
            else
            {
                this.UpdateReportSharedDataSet();
            }
        }

        internal void UpdateReportSharedDataSource()
        {
            var dataSources = from dataSouceInfo in this.ReportDataSources
                              where dataSouceInfo.ConnectionProperties == null && !string.IsNullOrEmpty(dataSouceInfo.DataSourceReference)
                              select dataSouceInfo;

            this.sharedDataSource = dataSources.ToList();

            if (sharedDataSource.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.ReportServerUrl) &&
                (this.ReportServerCredential != null || this.ReportServerFormsCredential != null))
                {
                    this.sharedDataSourceIndex = -1;
                    this.UpdateSharedDataSource();
                }
                else
                {
                    Exception ex = new Exception("Provide ReportServer information to get shared Datasource information");
                    this.RaiseReportExceptionEvent(ex);
                }
            }
            else
            {
                this.UpdateReport();
            }
        }

        internal void UpdateReportSharedDataSet()
        {
            var dataSets = from dataSetInfo in this.ReportDataSets
                           where dataSetInfo.SharedDataSet != null
                           select dataSetInfo;

            this.sharedDataSet = dataSets.ToList();

            if (this.sharedDataSet.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.ReportServerUrl) &&
               (this.ReportServerCredential != null || this.ReportServerFormsCredential != null))
                {
                    this.sharedDataSetIndex = -1;
                    this.UpdateSharedDataSet();
                }
                else
                {
                    Exception ex = new Exception("Provide ReportServer information to get shared Dataset information");
                    this.RaiseReportExceptionEvent(ex);
                }
            }
            else
            {
                this.UpdateReportSharedDataSource();
            }
        }

#if WINRT
        public void ProcessReport()
        {
            this.IsServerReport = false;

            if (!string.IsNullOrEmpty(this.ReportServiceURL))// this.ReportServerUrl !string.IsNullOrEmpty(this.ReportPath) && (this.ReportServerCredential != null || this.ReportServerFormsCredential != null) && !string.IsNullOrEmpty(this.ReportServerUrl))
            {
                this.ProcessServerReport();
            }
            else
            {
                Exception ex = new Exception("Provide ReportService information to retrive the Report from server");
                this.RaiseReportExceptionEvent(ex);
            }
        }

#elif SILVERLIGHT
        public void ProcessReport()
        {
            if (!string.IsNullOrEmpty(this.ReportServiceURL) && !string.IsNullOrEmpty(this.ReportPath))
            {
                this.ProcessServerReport();
            }
        }
#endif

#if SILVERLIGHT

        internal ReportServerCredential GetReportServerCredential()
        {
            ReportServerCredential credential = new ReportServerCredential();
            NetworkCredential clientCredential = this.ReportServerCredential as NetworkCredential;
            credential.UserName = clientCredential.UserName;
            credential.Password = clientCredential.Password;
            credential.Domain = clientCredential.Domain;
            return credential;
        }

        internal ReportServerFormCredential GetReportServerFormCredential()
        {
            ReportServerFormCredential credential = new ReportServerFormCredential();
            credential.UserName = this.ReportServerFormsCredential.UserName;
            credential.Password = this.ReportServerFormsCredential.Password;

            if (!string.IsNullOrEmpty(this.ReportServerUrl))
            {
                Regex reg = new Regex(@"((?<servername>\S*)(?i:reportserver))", RegexOptions.IgnoreCase);
                Match match = reg.Match(this.ReportServerUrl);
                string authority = match.Groups["servername"].Value;
                credential.Authority = authority;
            }

            return credential;
        }

        void ProcessServerReport()
        {
            Syncfusion.Reports.Server.ReportSetting setting = new Syncfusion.Reports.Server.ReportSetting();
            setting.ReportPath = this.ReportPath;
            setting.ReportServerURL = this.ReportServerUrl;
            setting.LoadInformationfromServer = this.LoadInformationFromServer;

            if (this.ReportServerCredential != null)
            {
                setting.ReportServerCredential = this.GetReportServerCredential();
            }

            if (this.ReportServerFormsCredential != null)
            {
                setting.ReportServerFormCredential = this.GetReportServerFormCredential();
            }

            this.ReportingServer.GetReportCompleted += new GetReportEventHandler(ReportingServer_GetReportCompleted);
            this.ReportingServer.GetReport(setting);
        }

        void ReportingServer_GetReportCompleted(object sender, GetReportEventArgs e)
        {
            this.ReportingServer.GetReportCompleted -= new GetReportEventHandler(ReportingServer_GetReportCompleted);

            if (e.Result == null)
            {
                this.RaiseReportExceptionEvent(new Exception("Provided Reportpath was invalid"));
            }
            else
            {
                this.IsServerReport = true;
                MemoryStream mem = new MemoryStream(e.Result);
                this.LoadReport(mem);
            }
        }

        void UpdateSharedDataSet()
        {
            this.sharedDataSetIndex++;
            DOM.DataSet dataSet = this.sharedDataSet[this.sharedDataSetIndex];
            Syncfusion.Reports.Server.ReportSetting setting = new Syncfusion.Reports.Server.ReportSetting();
            setting.ReportPath = this.ReportPath;
            setting.ReportServerURL = this.ReportServerUrl;
            setting.LoadInformationfromServer = this.LoadInformationFromServer;

            if (this.ReportServerCredential != null)
            {
                setting.ReportServerCredential = this.GetReportServerCredential();
            }

            if (this.ReportServerFormsCredential != null)
            {
                setting.ReportServerFormCredential = this.GetReportServerFormCredential();
            }

            this.ReportingServer.GetSharedDataSetCompleted += new GetSharedDataSetEventHandler(ReportingServer_GetSharedDataSetCompleted);
            this.ReportingServer.GetSharedDataSet(setting, dataSet.Name);
        }

        void ReportingServer_GetSharedDataSetCompleted(object sender, GetSharedDataSetEventArgs e)
        {
            this.ReportingServer.GetSharedDataSetCompleted -= new GetSharedDataSetEventHandler(ReportingServer_GetSharedDataSetCompleted);

            if (e.Result.Exception != null || e.Result.DataSetStream == null)
            {
                this.RaiseReportExceptionEvent(new Exception("Getting exception while processs shared dataset " + e.Result.Exception));
            }

            else
            {
                this.UpdateSharedDataSet(e.Result);
            }
        }

        void UpdateSharedDataSource()
        {
            sharedDataSourceIndex++;
            Syncfusion.RDL.DOM.DataSource dataSource = sharedDataSource[sharedDataSourceIndex];

            Syncfusion.Reports.Server.ReportSetting setting = new Syncfusion.Reports.Server.ReportSetting();
            setting.ReportPath = this.ReportPath;
            setting.ReportServerURL = this.ReportServerUrl;

            if (this.ReportServerCredential != null)
            {
                setting.ReportServerCredential = this.GetReportServerCredential();
            }

            if (this.ReportServerFormsCredential != null)
            {
                setting.ReportServerFormCredential = this.GetReportServerFormCredential();
            }

            if ((this.ReportServerCredential != null || this.ReportServerFormsCredential != null) && !string.IsNullOrEmpty(this.ReportServerUrl))
            {
                this.ReportingServer.GetSharedDataSourceCompleted += new GetSharedDataSourceEventHandler(ReportingServer_GetSharedDataSourceCompleted);
                this.ReportingServer.GetSharedDataSourceDefinition(setting, dataSource.Name);
            }
        }

        void ReportingServer_GetSharedDataSourceCompleted(object sender, GetSharedDataSourceEventArgs e)
        {
            if (e.Result.Exception != null)
            {
                this.RaiseReportExceptionEvent(new Exception("Getting following excetion while process shared datasource " + e.Result.Exception));
            }

            else
            {
                DOM.DataSource dataSource = sharedDataSource[sharedDataSourceIndex];
                UpdateDataSourceDefintion(e.Result, dataSource);

                if (sharedDataSourceIndex == sharedDataSource.Count - 1)
                {
                    this.UpdateReport();
                }
                else
                {
                    UpdateSharedDataSource();
                }
            }
        }

        void UpdateDataSourceDefintion(ServiceDataSourceDefinition dsDefinition, Syncfusion.RDL.DOM.DataSource dataSource)
        {

            dataSource.ConnectionProperties = new Syncfusion.RDL.DOM.ConnectionProperties();

            if (!LoadInformationFromServer)
            {
                dataSource.ConnectionProperties.ConnectString = dsDefinition.ConnectionString;
                dataSource.ConnectionProperties.DataProvider = dsDefinition.Provider;

                switch (dsDefinition.AutenticationInfo)
                {
                    case "Integrated":
                        dataSource.ConnectionProperties.IntegratedSecurity = true;
                        break;
                    case "Prompt":
                        dataSource.ConnectionProperties.Prompt = "Prompt";
                        break;
                }
            }
        }
#else
        public void ProcessReport()
        {
            this.IsServerReport = false;

            try
            {
                if (!string.IsNullOrEmpty(this.ReportPath) && (this.ReportServerCredential != null || this.ReportServerFormsCredential != null) && !string.IsNullOrEmpty(this.ReportServerUrl))
                {
                    if (!File.Exists(this.ReportPath))
                    {
                        this.ProcessServerReport();
                    }
                }
                if (!string.IsNullOrEmpty(this.ReportPath) && !this.IsServerReport &&
                    (this.ReportPath.StartsWith("http:") || this.ReportPath.StartsWith("https:")))
                {
                    try
                    {
                        WebClient client = new WebClient();
                        Stream stream = new MemoryStream(client.DownloadData(this.ReportPath));
                        this.LoadReport(stream);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (!string.IsNullOrEmpty(this.ReportPath) && File.Exists(this.ReportPath)
                    && !this.IsServerReport)
                {
                    this.Load(this.ReportPath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void ProcessServerReport()
        {
            if (string.IsNullOrEmpty(this.ReportServerUrl) || (this.ReportServerCredential == null && this.ReportServerFormsCredential == null))
                throw new WebException("The ReportServerUrl/ReportServerCredential is missing");

            string exception = null;
            Stream reportStream = this.ReportingServer.GetReportDefinition(out exception);

            if (!string.IsNullOrEmpty(exception))
            {
                throw new Exception(exception);
            }
            
            if (reportStream != null)
            {
                this.IsServerReport = true;
                this.LoadReport(reportStream);
            }
        }

        void UpdateSharedDataSet()
        {
            this.sharedDataSetIndex++;
            DOM.DataSet currentdataset = this.sharedDataSet[this.sharedDataSetIndex];
            UpdateSharedDataSet(this.ReportingServer.GetSharedDatadefintion(currentdataset.Name));
        }

        void UpdateSharedDataSource()
        {
            this.sharedDataSourceIndex++;
            DataSource dataSource = this.sharedDataSource[sharedDataSourceIndex];

            DataSourceDefinition dsDefinition = null;

            if (!this.IsServerReport)
            {
                dsDefinition = this.ReportingServer.GetDataSourceDefinition(dataSource.DataSourceReference);
            }
            else
            {
                dsDefinition = this.ReportingServer.GetDataSourceDefinition(dataSource.Name);
            }

            this.UpdateDataSourceDefintion(dsDefinition, dataSource);

            if (this.sharedDataSourceIndex == this.sharedDataSource.Count - 1)
            {
                this.UpdateReport();
            }
            else
            {
                this.UpdateSharedDataSource();
            }
        }

        void UpdateDataSourceDefintion(DataSourceDefinition dsDefinition, DataSource dataSource)
        {
            if (dsDefinition != null)
            {
                dataSource.ConnectionProperties = new ConnectionProperties();
                dataSource.ConnectionProperties.ConnectString = dsDefinition.ConnectString;
                dataSource.ConnectionProperties.DataProvider = dsDefinition.Extension;

                switch (dsDefinition.CredentialRetrieval)
                {
                    case CredentialRetrievalEnum.Integrated:
                        dataSource.ConnectionProperties.IntegratedSecurity = true;
                        break;
                    case CredentialRetrievalEnum.Prompt:
                        dataSource.ConnectionProperties.Prompt = dsDefinition.Prompt;
                        break;
                }
            }
        }

        void Load(string reportPath)
        {
            if (File.Exists(reportPath))
            {
                using (FileStream stream = new FileStream(reportPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    this.LoadReport(stream);
                }
            }
        }
#endif

        void UpdateSharedDataSet(SharedDatasetinfo info)
        {
#if SILVERLIGHT
            Stream stream = new MemoryStream(info.DataSetStream);
#else
            Stream stream = info.DataSetStream;
#endif
            using (XmlReader reader = XmlReader.Create(stream))
            {
                if (this.ReportDataSources == null)
                {
                    this.ReportDataSources = new DataSources();
                }

                DOM.DataSet dataSet = this.sharedDataSet[this.sharedDataSetIndex];
                reader.MoveToContent();
                string Namespace = reader.NamespaceURI;
                string Version = (Regex.IsMatch(Namespace, @"\d{4}") ? Regex.Match(Namespace, @"\d{4}").Value : string.Empty);
                if (!string.IsNullOrEmpty(Version))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SharedDataSet), Namespace);
                    SharedDataSet dataset = (SharedDataSet)xs.Deserialize(reader);
                    bool isDataSourceContains = false;
                    string dataSourceName = null;


                    isDataSourceContains = (from dataSource in this.ReportDataSources
                                            where dataSource.Name == info.DataSourceDefinition.Name
                                            select dataSource).Count() == 0;

                    dataSourceName = info.DataSourceDefinition.Name;

                    if (isDataSourceContains)
                    {
                        DOM.DataSource datasource = new DOM.DataSource();
                        datasource.DataSourceReference = dataset.DataSet.Query.DataSourceReference;
                        datasource.Name = dataSourceName;
                        this.Report.DataSources.Add(datasource);
                        this.UpdateDataSourceDefintion(info.DataSourceDefinition, datasource);
                    }



                    //else
                    //{
                    //    dataSourceName = "sharedData";

                    //    int count = 1;
                    //    do
                    //    {
                    //        dataSourceName += count;

                    //        isDataSourceContains = (from dataSource in this.ReportDataSources
                    //                                where dataSource.Name == dataSourceName
                    //                                select dataSource).Count() == 0;
                    //    } while (!isDataSourceContains);
                    //}

                    if (this.sharedDataSet == null)
                    {
                        this.sharedDataSet = new List<DataSet>();
                    }


                    dataSet.Query = new Query();
                    dataSet.Query.QueryParameters = dataSet.SharedDataSet.QueryParameters;
                    dataSet.Query.DataSourceName = dataSourceName;
                    dataSet.Query.CommandText = dataset.DataSet.Query.CommandText;
                }
            }

            if (this.sharedDataSetIndex < this.sharedDataSet.Count - 1)
            {
                UpdateSharedDataSet();
            }
            else
            {
                UpdateReportSharedDataSource();
            }
        }

        #endregion

        internal string GetReportName()
        {
            if (!string.IsNullOrEmpty(this.ReportPath))
            {
#if !SILVERLIGHT || !WINRT
                if (File.Exists(this.ReportPath))
                {
                    string fileName = System.IO.Path.GetFileName(this.ReportPath);
                    return fileName.Substring(0, fileName.LastIndexOf('.'));
                }
                else
#endif
                {
                    var index = this.ReportPath.LastIndexOf('/');
                    if (index != -1)
                    {
                        string filename = this.ReportPath;
                        return filename.Substring(index + 1, (filename.Length - (index + 1)));
                    }
                    return this.ReportPath;
                }
            }

            return string.Empty;
        }

        #region DrillThroughSupport

        internal void DrillThroughReport(ReportModel model, DOM.Drillthrough drillthrough)
        {
            DrillThroughModel reportModel = new DrillThroughModel();
            reportModel.ParentModel = model;
            reportModel.ChildModel = new ReportModel();

            reportModel.ChildModel.IsRDLC = model.IsRDLC;
            reportModel.ChildModel.ReportServerUrl = model.ReportServerUrl;
            reportModel.ChildModel.ReportServerCredential = model.ReportServerCredential;
            reportModel.ChildModel.ReportServerFormsCredential = model.ReportServerFormsCredential;
            string reportPath = drillthrough.ReportName;

            if (!string.IsNullOrEmpty(model.ReportServerUrl))
            {
                if (!(reportPath.Trim().StartsWith(@"/")))
                {
                    string parentpath = model.ReportPath;
                    int lastIndex = parentpath.LastIndexOf("/");
                    reportPath = parentpath.Substring(0, lastIndex + 1) + reportPath;
                }
                reportModel.ChildModel.ReportPath = reportPath;
            }
            else
            {
                if (!string.IsNullOrEmpty(drillthrough.ReportName) && !string.IsNullOrEmpty(model.ReportPath))
                {
                    if ((reportPath.Trim().StartsWith(@"/")))
                    {
                        reportPath = reportPath.Substring(1);
                    }
                    if (model.IsRDLC)
                    {
                        reportPath = System.IO.Path.GetDirectoryName(model.ReportPath) + "\\" + reportPath + ".rdlc";
                    }
                    else
                    {
                        reportPath = System.IO.Path.GetDirectoryName(model.ReportPath) + "\\" + reportPath + ".rdl";
                    }

                    reportModel.ChildModel.ReportPath = reportPath;
                }
            }
#if SILVERLIGHT || WINRT
            reportModel.ChildModel.ReportServiceURL = model.ReportServiceURL;
#endif
#if WINRT
            List<Syncfusion.UI.Xaml.Reports.ReportParameter> reportParameters = new List<Syncfusion.UI.Xaml.Reports.ReportParameter>();
            if (drillthrough.Parameters != null)
            {
                foreach (var parameter in drillthrough.Parameters)
                {
                    Syncfusion.UI.Xaml.Reports.ReportParameter meter = new Syncfusion.UI.Xaml.Reports.ReportParameter();
                    meter.Name = parameter.Name;
                    meter.Values.Add(parameter.Value);
                    meter.Labels.Add(parameter.Value);
                    reportParameters.Add(meter);
                }
            }
#elif !MVC
            List<Syncfusion.Windows.Reports.ReportParameter> reportParameters = new List<Syncfusion.Windows.Reports.ReportParameter>();
            if (drillthrough.Parameters != null)
            {
                foreach (var parameter in drillthrough.Parameters)
                {
                    Syncfusion.Windows.Reports.ReportParameter meter = new Syncfusion.Windows.Reports.ReportParameter();
                    meter.Name = parameter.Name;
                    meter.Values.Add(parameter.Value);
                    meter.Labels.Add(parameter.Value);
                    reportParameters.Add(meter);
                }
            }
#endif

#if !MVC
            reportModel.ChildModel.InnerReportParameter = reportParameters;
            this.RaiseDrillThroughReport(reportModel);
#endif
        }

        #endregion

        #region DataSource supports

        public IList<string> GetDataSetNames()
        {
            List<string> DataSourceCollection = new List<string>();

            foreach (DataSet dataSource in this.ReportDataSets)
            {
                DataSourceCollection.Add(dataSource.Name);
            }

            return DataSourceCollection;
        }

        internal ReportDataSourceInfoCollection GetDataSources()
        {
            if (this.ReportDataSources != null)
            {
                ReportDataSourceInfoCollection dataSourceInfos = new ReportDataSourceInfoCollection();

                foreach (var dataSource in this.ReportDataSources)
                {
                    ReportDataSourceInfo dataSourceInfo = new ReportDataSourceInfo();
                    dataSourceInfo.Name = dataSource.Name;
                    if (dataSource.ConnectionProperties != null)
                    {
                        dataSourceInfo.Prompt = dataSource.ConnectionProperties.Prompt;
                    }

                    dataSourceInfos.Add(dataSourceInfo);
                }

                return dataSourceInfos;
            }

            return null;
        }

        internal void SetDataSourceCredentials(IEnumerable<DataSourceCredentials> dataSourceCredentials)
        {
            if (dataSourceCredentials != null)
            {
#if SILVERLIGHT || WINRT 
                ProcessedData.dataSourceCredentials = dataSourceCredentials;
#endif
                SetDataSourceCredentials(dataSourceCredentials.ToArray());
                dataSourceCredential = dataSourceCredentials;
            }
        }

        internal void SetDataSourceCredentials(DataSourceCredentials[] dataSourceCredentials)
        {
            if (dataSourceCredentials != null)
            {
                foreach (DataSourceCredentials dataSourceCreden in dataSourceCredentials)
                {
                    var dataSources = from dataSource in this.ReportDataSources
                                      where dataSource.Name.Equals(dataSourceCreden.Name)
                                      select dataSource;

                    if (dataSources.Count() > 0)
                    {
                        var dataSource = dataSources.First();
                        dataSource.ConnectionProperties.UserName = dataSourceCreden.UserId;
                        dataSource.ConnectionProperties.PassWord = dataSourceCreden.Password;
                        dataSource.ConnectionProperties.IntegratedSecurity = dataSourceCreden.IntegratedSecurity;
                    }
                }
            }
        }

        #endregion

        #region Parameter supports

        public ReportParameterInfoCollection GetParameters()
        {
            ReportParameterInfoCollection paramInfoCollection = new ReportParameterInfoCollection();

            foreach (Syncfusion.RDL.DOM.ReportParameter parameter in this.ReportParameters)
            {
#if WINRT
                var paramInfo = new Syncfusion.UI.Xaml.Reports.ReportParameterInfo();
#elif MVC
                var paramInfo = new Syncfusion.Reports.Mvc.ReportParameterInfo();
#else
                var paramInfo = new Syncfusion.Windows.Reports.ReportParameterInfo();
#endif
                paramInfo.AllowBlank = parameter.AllowBlank;
                paramInfo.DataType = (ParamType)Enum.Parse(typeof(ParamType), parameter.DataType.ToString(), true);

                var param = (from modelParamter in this.ParameterDetails
                             where (modelParamter.Name.Equals(parameter.Name))
                             select modelParamter).FirstOrDefault();

                foreach (var value in param.Label)
                {
                    paramInfo.Labels.Add(value.ToString());
                }

                paramInfo.MultiValue = parameter.MultiValue;
                paramInfo.Name = parameter.Name;
                paramInfo.Nullable = parameter.Nullable;
                paramInfo.Prompt = parameter.Prompt;

                paramInfo.Hidden = parameter.Hidden;

                if (parameter.ValidValues != null)
                {
                    foreach (ParameterValue paramValue in parameter.ValidValues.ParameterValues)
                    {
                        paramInfo.ValidValues.Add(new ValidValue { Label = paramValue.Label, Value = paramValue.Value });
                    }
                }

                foreach (var value in param.Value)
                {
                    paramInfo.Values.Add(value.ToString());
                }

                paramInfoCollection.Add(paramInfo);
            }
            return paramInfoCollection;
        }
#if WINRT
        public void SetParameters(IEnumerable<Syncfusion.UI.Xaml.Reports.ReportParameter> reportParameters)
#elif MVC
        public void SetParameters(IEnumerable<Syncfusion.Reports.Mvc.ReportParameter> reportParameters)
#else
        public void SetParameters(IEnumerable<Syncfusion.Windows.Reports.ReportParameter> reportParameters)
#endif
        {
            isRender = false;
            foreach (var reportParameter in reportParameters)
            {
                if (this.ParameterDetails != null)
                {
                    var parameter = (from modelParamter in this.ParameterDetails
                                     where (modelParamter.Name.Equals(reportParameter.Name))
                                     select modelParamter).FirstOrDefault();

                    if (parameter != null)
                    {
                        parameter.Name = reportParameter.Name;
                        parameter.Value.Clear();

                        if (!string.IsNullOrEmpty(reportParameter.Prompt))
                        {
                            parameter.Prompt = reportParameter.Prompt;
                        }
                        if (reportParameter.Values != null)
                        {
                            foreach (var data in reportParameter.Values)
                            {
                                if (parameter.DataType == ParamType.DateTime)
                                {
                                    try
                                    {
                                        DateTime dt = DateTime.Parse(data,System.Globalization.CultureInfo.InvariantCulture);
                                        parameter.Value.Add(dt);
                                    }
                                    catch
                                    {
                                        parameter.Value.Add(data);
                                    }
                                }
                                else
                                {
                                    parameter.Value.Add(data);
                                }
                            }
                        }
                        
                        parameter.Label.Clear();
                        if (reportParameter.Labels != null)
                        {
                            foreach (var data in reportParameter.Labels)
                            {
                                parameter.Label.Add(data);
                            }
                        }                       

#if MVC
                        if (!isRender)
                        {
                            bool isNullable = parameter.Nullable && reportParameter.Nullable;                        
                            isRender = IsValidParameterValues(parameter, reportParameter.Values,isNullable);
                        }
#endif
                    }
                }
            }
        }

        private bool IsValidParameterValues(ParameterInformation parameter, List<string> values, bool isNull)
        {
            if (parameter.DataType == ParamType.Boolean)
            {
                if (!isNull)
                {
                    if (!values.Any())
                    {
                        return true;
                    }
                    else
                    {
                        var text = values.First();
                        if (text == "null" || string.IsNullOrEmpty(text))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (parameter.DataType == ParamType.DateTime)
            {
                if (!isNull)
                {
                    var text = values.First();
                    if (string.IsNullOrEmpty(text))
                    {
                        return true;
                    }                    
                }
            }
            else if (parameter.DataType == ParamType.Float || parameter.DataType == ParamType.Integer ||
                     parameter.DataType == ParamType.String)
            {
                if (!isNull)
                {
                    var text = values.Count() > 0 ? values.First() : string.Empty;
                    if (!parameter.AllowBlank && (text == "null" || string.IsNullOrEmpty(text)))
                    {
                        return true;
                    }                    
                }
            }
            return false;
        }


        #endregion

        #region Events

        internal void RaiseDrillThroughReport(DrillThroughModel model)
        {
            if (DrillThroughInnerReport != null)
            {
                DrillThroughInnerReport(this, new DrillThroughEventArgs() { Model = model });
            }
        }

        internal void RaiseToggleChanged(object sender, object e, int row, int col)
        {
            if (sender is TextboxModel)
            {
                var txtmodel = (sender as TextboxModel);
                this.UpdateDrillDown(txtmodel, row, col);
            }
            if (ToggleChanged != null)
            {
                ToggleChanged(sender, e);
            }
        }

        internal void RaiseMouseScrolling(object sender, object eventArgs)
        {
            if (MouseWheelScrolling != null)
            {
                MouseWheelScrolling(sender,eventArgs);
            }
        }

        internal void UpdateDrillDown(TextboxModel model, int row, int col)
        {
            if (model.ToggleInfos != null && model.ToggleInfos.Count > 0)
            {
                foreach (var reportItem in from name in model.ToggleInfos from reportItem in this.BodyReportItemModels where reportItem.Name == name select reportItem)
                {
                    reportItem.Hidden = !reportItem.Hidden;
                }
            }
            else if (model.ToggleGroups != null && model.ToggleGroups.Count > 0 && model.IsTablixChild)
            {
                var toggleGroup = model.ToggleGroups.Where(t => t.RowNo == row && t.ColNo == col);

                if (toggleGroup.Any())
                {
                    DrillDownModel drillModel = toggleGroup.First().DrillDownInfos;
                    if (drillModel != null)
                    {
                        foreach (var drilldown in drillModel.InnerDrillInfo)
                        {
                            drilldown.IsHidden = !drilldown.IsHidden;
                        }
                    }
                }
            }
        }

        internal void RaiseSubReportProcessingEvent(SubreportProcessingEventArgs e)
        {
            if (SubreportProcessing != null)
            {
                SubreportProcessing(this, e);
            }
        }

        protected virtual void RaiseReportLoadedEvent(EventArgs e)
        {
            if (this.ReportLoaded != null)
            {
                this.ReportLoaded(this, e);
            }
        }

        protected virtual void RaiseReportItemsEvaluated(ReportItemEvaluatedEventArgs e)
        {
            if (this.ReportItemsEvaluated != null)
            {
                this.ReportItemsEvaluated(this, e);
            }
        }

        protected virtual void RaiseReportExceptionEvent(Exception exception)
        {
            if (this.ReportException != null)
            {
                this.ReportException(this, new ReportExceptionEventArgs { Exception = exception });
            }
        }

        protected virtual void RaiseDataSourceUpdatedEvent(EventArgs e)
        {
            if (this.DataSourceUpdated != null)
            {
                this.DataSourceUpdated(this, e);
            }
        }

        protected virtual void RaiseCrendentialCheckCompletedEvent(EventArgs e)
        {
            if (this.CrendentialCheckCompleted != null)
            {
                this.CrendentialCheckCompleted(this, e);
            }
        }

        protected virtual void RaiseConnectionValidatedEvent(ReportingConnectionValidationEventArgs e)
        {
            if (this.ConnectionValidated != null)
            {
                this.ConnectionValidated(this, e);
            }
        }

        protected virtual void RaiseDataSourceCredentialsUpdatedEvent()
        {
            if (this.DataSourceCredentialsUpdated != null)
            {
                this.DataSourceCredentialsUpdated(this, new EventArgs());
            }
        }

        #endregion
    }

    #region HelperClasses

    class DataSetInfo
    {
        public string Name { get; set; }
        public List<string> DependentParameters { get; set; }
        public List<string> DependentDataSets { get; set; }
    }

    class DataSourceInfo
    {
        public string Name { get; set; }
    }

    class ParameterInformation
    {
        public string Name { get; set; }
        public bool IsMultiValue { get; set; }
        public bool AllowBlank { get; set; }
        public bool Nullable { get; set; }
        public bool Hidden { get; set; }
        public List<string> DependentDataSets { get; set; }
        public List<string> DependentParameters { get; set; }
        public List<object> Value { get; set; }
        public List<object> Label { get; set; }
        public ParamType DataType { get; set; }
        public IList<ValidValue> ValidValues { get; set; }
        public string Prompt { get; set; }
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    [XmlRoot("SharedDataSet")]
    public class SharedDataSet
    {
        [XmlElement("DataSet")]
        public DataSetShared DataSet { get; set; }
    }

    public class DataSetShared
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Query")]
        public SharedQuery Query { get; set; }

        public Fields Fields { get; set; }
    }

    public class SharedQuery
    {
        public string DataSourceReference { get; set; }
        public string CommandText { get; set; }
    }

    #endregion

    internal class DrillThroughModel
    {
        internal ReportModel ParentModel { get; set; }
        internal ReportModel ChildModel { get; set; }
    }

    internal class DrillthroughInfo
    {
        public string ReportName { get; set; }
        public List<DrillReportParameter> ReportParameters { get; set; }
    }

    internal class DrillReportParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Omit { get; set; }
    }

    internal class DocumentMapModel
    {
        public List<DocumentData> NodeData { get; set; }
        public DocumentMapModel()
        {
            this.NodeData = new List<DocumentData>();
        }
    }

    internal class DocumentData
    {
        public string DocumentLable { get; set; }
        public string ReportItemName { get; set; }
        public Dictionary<int, KeysCalculationValues> ReferRowInfo { get; set; }
        public bool IsTablixChild { get; set; }
        public double LeftPos { get; set; }
        public double TopPos { get; set; }
        public int PageNo { get; set; }
        public DocumentMapModel Node { get; set; }
        public ModelType ModelType { get; set; }
        public CellPosInfo PosInfos { get; set; }
    }

    internal class CellPosInfo
    {
        public int RowNo { get; set; }
        public int ColNo { get; set; }
    }
}
