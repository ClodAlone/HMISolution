//-------------------------------------------------------------------------------------------------
// <copyright file="OlapDataManager.cs" company="syncfusion">
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.DataProvider;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.MDXQueryBuilder;
using Syncfusion.Olap.Common;
using System.Globalization;

namespace Syncfusion.Olap.Manager
{
    /// <summary>
    /// Represents the management of data.
    /// </summary>
    [Serializable]
    public class OlapDataManager : IOlapDataManager, IDisposable
    {
        #region Private Variables
        private string _connectionString;

        private string _currentCubeName;

        [NonSerialized]
        private CubeSchema _currentCubeSchema;

        private OlapReport _currentReport;

        [NonSerialized]
        private IDataProvider _dataProvider;

        private bool _isCurrentReportModified = false;

        private bool _isUpdatedCellSet;
        private CultureInfo culture;

        private bool _isWhereClauseForSlicing = true;

        private bool _allowMdxToOlapReportParse = true;

        [NonSerialized]
        private MDXQuerySpecification _querySpecification;

        private string _reportPath;

        ////[NonSerialized]
        private OlapReportCollection _reports;

        [NonSerialized]
        private CellSet m_cellSet = null;

        [NonSerialized]
        private PivotEngine m_pivotEngine = null;

        [NonSerialized]
        private Member m_tempParentMember = null;

        //[DefaultValue(SummaryLayout.Bottom)]
        //private SummaryLayout m_summaryLayout;
        //private GridLayout m_gridLayout;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapDataManager"/> class.
        /// </summary>
        public OlapDataManager()
            : this(String.Empty)
        {
            this.MdxQuery = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapDataManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OlapDataManager(string connectionString)
        {
            this.InitializeOlapDataManager(connectionString);
            this.IsCommonConnection = false;
            this.MdxQuery = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapDataManager"/> class.
        /// </summary>
        /// <param name="adomdDataProvider">The ADOMD data provider.</param>
        public OlapDataManager(IDataProvider adomdDataProvider)
        {
            this.InitializeOlapDataManager(adomdDataProvider);
            this.IsCommonConnection = true;
            this.MdxQuery = string.Empty;
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Occurs when [axis element changed].
        /// </summary>
        public event AxisElementChangedEventHandler AxisElementChanged;
        /// <summary>
        /// Occurs when [axis element modified].
        /// </summary>
        public event AxisElementModifiedEventHandler AxisElementModified;
        /// <summary>
        /// Occurs when [cube changed].
        /// </summary>
        public event CubeChangedEventHandler CubeChanged;
        /// <summary>
        /// Occurs when [before MDX query execute].
        /// </summary>
        public event QueryExecuteEventHandler BeforeMdxQueryExecute;
        /// <summary>
        /// Occurs when [report changed].
        /// </summary>
        public event ReportChangedEventHandler ReportChanged;

        /// <summary>
        /// Occurs when [Active report changed].
        /// </summary>
        public event ActiveReportChangedEventHandler ActiveReportChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value to display the Localized member property in the output
        /// </summary>
        /// <value>
        /// <c>true</c> display the Localized member property; otherwise, <c>false</c>
        /// </value>
        public bool ShowLocalizedMemberProperties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance holds a common connection.
        /// </summary>
        /// <value>
        ///       <c>true</c> if this instance holds a common connection; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        internal bool IsCommonConnection { get; set; }

        /// <summary>
        /// Property that gets or sets whether to slice data using "Where" clause or "Select" clause.
        /// </summary>
        public bool UseWhereClauseForSlicing
        {
            get
            {
                return _isWhereClauseForSlicing;
            }

            set
            {
                _isWhereClauseForSlicing = value;
                if (!value)
                {
                    CurrentReport.UseWhereClauseForSlicing = value;
                }
            }
        }


        

        /// <summary>
        /// Gets or Sets a value indicating to enable MDX to OlapReport parsing.
        /// </summary>
        public bool AllowMdxToOlapReportParse
        {
            get { return _allowMdxToOlapReportParse; }
            set { _allowMdxToOlapReportParse = value; }
        }


        /// <summary>
        /// Gets or sets the MDX query.
        /// </summary>
        /// <value>The MDX query.</value>
        public string MdxQuery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show level type all].
        /// </summary>
        /// <value><c>true</c> if [show level type all]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowLevelTypeAll { get; set; }

        [NonSerialized]
        private object m_itemSource;
        /// <summary>
        /// Gets or sets the item source.
        /// </summary>
        /// <value>The item source.</value>
        [XmlIgnore()]
        public object ItemSource
        {
            get
            {
                return m_itemSource;
            }

            set
            {
                m_itemSource = value;
            }
        }

        /// <summary>
        /// Gets the current cell set.
        /// </summary>
        /// <value>The current cell set.</value>
        [XmlIgnore()]
        public CellSet CurrentCellSet
        {
            get
            {
                return m_cellSet;
            }

            private set
            {
                if (m_cellSet != value)
                {
                    m_cellSet = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        /// <value>The name of the current cube.</value>
        public string CurrentCubeName
        {
            get
            {
                return _currentCubeName;
            }

            set
            {
                NotifyCubeNameChanged(value, true);
            }
        }

        /// <summary>
        /// Gets the current cube schema.
        /// </summary>
        /// <value>The current cube schema.</value>
        [XmlIgnore]
        public CubeSchema CurrentCubeSchema
        {
            get
            {
                if (_currentCubeSchema == null)
                {
                    _currentCubeSchema = this.DataProvider.GetCubeSchema(this.CurrentCubeName);
                }

                return _currentCubeSchema;
            }

            set
            {
                _currentCubeSchema = value;
            }
        }

        /// <summary>
        /// Gets or sets the current report.
        /// </summary>
        /// <value>The current report.</value>
        public OlapReport CurrentReport
        {
            get
            {
                if (_currentReport == null)
                {
                    this._currentReport = new OlapReport();
                    ////this._CurrentReport.Model = this;
                }

                return _currentReport;
            }

            set
            {
                _currentReport = value;
                ///// when the latest report has a valid report load the same, trigger the cube change to load new report
                ///// update controls to to refresh data
                ///// update OLAP Client and Report Builder to clear/refresh the data
                if (_currentReport != null)
                {
                    if (!string.IsNullOrEmpty(this.ConnectionString) && !string.IsNullOrEmpty(this._currentReport.CurrentCubeName))
                    {
                        _currentCubeName = this._currentReport.CurrentCubeName;
                        (this.DataProvider as AdomdDataProvider).CurrentCubeName = _currentCubeName;
                        if (!this.UseWhereClauseForSlicing)
                            this._currentReport.UseWhereClauseForSlicing = false;
                        // Triggering the Report changed event 
                        // TODO update full comments here
                        this.NotifyReportChanged(this._currentReport);
                        // Triggering element modified event, this will trigger the WPF controls
                        // to refresh the data
                        this.NotifyElementModified();
                    }
                }
                else
                {
                    // when the value is null, it means clear the report
                    // Create a new instance of OLAP Report, update the controls to clear the data
                    // Update OLAP Client to clear the data
                    _currentReport = new OlapReport();
                    this.m_cellSet = null;
                    if (!string.IsNullOrEmpty(this.ConnectionString) && !string.IsNullOrEmpty(this.CurrentCubeName))
                    {
                        _currentReport.CurrentCubeName = this.CurrentCubeName;
                        // TODO update full comments here
                        this.NotifyReportChanged(this._currentReport);
                        // Triggering element modified event, this will trigger the WPF controls
                        // to refresh the data
                        this.NotifyElementModified();
                    }
                }
            }
        }

        private OlapReport activeReport;

        /// <summary>
        /// Gets or sets the OlapReport when multiple OlapControls shares same OlapDataManager.
        /// </summary>
        public OlapReport ActiveReport
        {
            get { return activeReport; }
            set
            {
                activeReport = value;
                if (activeReport != null)
                {
                    if (!string.IsNullOrEmpty(this.ConnectionString) &&
                        !string.IsNullOrEmpty(this.activeReport.CurrentCubeName))
                    {
                        _currentCubeName = this.activeReport.CurrentCubeName;
                        if (!this.UseWhereClauseForSlicing)
                            this.activeReport.UseWhereClauseForSlicing = false;
                    }
                }
            }
        }

        private bool _useSharedDataManager = false;

        /// <summary>
        /// Gets or sets whether this instance can be shared among different OLAP controls. Default value is false. 
        /// </summary>
        public bool UseSharedDataManager
        {
            get { return _useSharedDataManager; }
            set { _useSharedDataManager = value; }
        }

        private Items _virtualKpiElements = new Items();

        /// <summary>
        /// Gets or sets the virtual KPI collection.
        /// </summary>
        public Items VirtualKpiElements
        {
            get { return _virtualKpiElements; }
            set { _virtualKpiElements = value; }
        }


        private Items _calculatedMembers = new Items();

        /// <summary>
        /// Gets or sets the calculated members/measures collection.
        /// </summary>
        public Items CalculatedMembers
        {
            get { return _calculatedMembers; }
            set
            {
                _calculatedMembers = value;
            }
        }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        [XmlIgnore()]
        public IDataProvider DataProvider
        {
            get
            {
                if (this.ConnectionString != null && this.ConnectionString != string.Empty)
                {
                    if (this._dataProvider == null || this._dataProvider.ConnectionString == string.Empty)
                    {
                        this._dataProvider = new AdomdDataProvider(this.ConnectionString);
                    }
                }
                return _dataProvider;
            }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Value cannot be null");
                }

                _dataProvider = value;
                //// When directly Data provider supplied update the current
                //// connection string with the supplied provider connection string
                this.ConnectionString = _dataProvider.ConnectionString;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is current report modified.
        /// </summary>
        /// <value>
        ///       <c>true</c> if this instance is current report modified; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentReportModified
        {
            get
            {
                return _isCurrentReportModified;
            }

            set
            {
                _isCurrentReportModified = value;
            }
        }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture
        {
            get
            {
                if (culture == null)
                    culture = CultureInfo.CurrentCulture;
                return culture;
            }
            set
            {
                culture = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether  to override default OlapCube's FormatStrings of Value cells.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [override default format strings]; otherwise, <c>false</c>.
        /// </value>
        public bool OverrideDefaultFormatStrings { get; set; }

        /// <summary>
        /// Gets or sets the pivot engine.
        /// </summary>
        /// <value>
        /// The pivot engine.
        /// </value>
        [XmlIgnore()]
        public PivotEngine PivotEngine
        {
            get
            {
                return m_pivotEngine;
            }

            set
            {
                if (m_pivotEngine != value)
                {
                    m_pivotEngine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [XmlIgnore()]
        public PropertyCollection Properties { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// Gets the query specification.
        /// </summary>
        /// <value>The query specification.</value>
        [XmlIgnore()]
        [Obsolete("Use GetMDXQuerySpecification() method instead")]
        public MDXQuerySpecification QuerySpecification
        {
            get
            {
                if (this.UseSharedDataManager && this.ActiveReport != null)
                    _querySpecification = GetMDXQuerySpecForActiveReport();
                else
                    _querySpecification = GetMDXQuerySpecification();
                return _querySpecification;
            }
        }

        /// <summary>
        /// Gets the report path.
        /// </summary>
        public string ReportPath
        {
            get
            {
                return _reportPath;
            }

            private set
            {
                _reportPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        ////    [XmlIgnore()]
        public OlapReportCollection Reports
        {
            get
            {
                return _reports;
            }

            set
            {
                //// When the report path is empty ReportBuilder/OLAP Client will 
                //// promt the user to save the report before creating report
                this.ReportPath = string.Empty;
                _reports = value;
            }
        }

        /// <summary>
        /// Gets or sets the relational data's sort order.
        /// </summary>
        /// <value>The relational data's sort order.</value>
        public SortType RelationalDataSortOrder
        {
            get;
            set;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        public void AddReport(string reportName)
        {
            OlapReport report = this.Reports[reportName];
            if (report == null)
            {
                report = new OlapReport(reportName);
                report.CurrentCubeName = this.CurrentCubeName;
                report.EngineVersion = QueryBuilderEngineVersions.Version3;

                this.Reports.Add(report);
                this.CurrentReport = report;
                this.IsCurrentReportModified = true;
            }
            else
            {
                throw new OlapDataManagerException("A Report already exist with the same name");
            }
        }

        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="report">Name of the report.</param>
        public void AddReport(OlapReport report)
        {
            if (report != null)
            {
                if (report.CurrentCubeName == string.Empty)
                {
                    report.CurrentCubeName = this.CurrentCubeName;
                }

                this.Reports.Add(report);
                this.CurrentReport = report;
                this.IsCurrentReportModified = true;
            }
            else
            {
                throw new OlapDataManagerException("Supplied Report should not be null");
            }
        }

        /// <summary>
        /// Clones the <see cref="OlapDataManager"/> elements.
        /// </summary>
        /// <returns>A <see cref="OlapDataManager"/> object.</returns>
        public OlapDataManager CloneOlapDataManagerElements()
        {
            OlapDataManager olapDataManager = new OlapDataManager();
            olapDataManager.ConnectionString = this.ConnectionString;
            /////Maintaining same connection object so that it should not create a new connection on 
            /////every tooltip
            olapDataManager.DataProvider = this.DataProvider;
            /////Copying the current cube name
            olapDataManager.NotifyCubeNameChanged(this.CurrentCubeName, false);
            olapDataManager.SetCurrentReport(this.CurrentReport.Clone());
            return olapDataManager;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.CurrentReport = null;
            this.PivotEngine = null;
            this.CurrentCellSet = null;
            
            if (!this.IsCommonConnection && this.DataProvider != null)
            {
                this.DataProvider.CloseConnection();
            }
        }

        /// <summary>
        /// Executes the cell set.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>CellSet</returns>
        public CellSet ExecuteCellSet(string commandText)
        {
            MdxQuery = commandText;

            var args = new QueryExecutingEventArgs(commandText, "ExecuteCellSet");
            RaiseQueryExecuting(args);
            if (args.Cancel) return null;
            (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
            m_cellSet = this.DataProvider.ExecuteCellSet(args.MdxQuery, true, true, this.CurrentReport);
            return m_cellSet;
        }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Object contains the cell set information.</returns>
        public object Execute(string commandText)
        {
            var args = new QueryExecutingEventArgs(commandText, "Execute");
            RaiseQueryExecuting(args);
            if (args.Cancel) return null;

            return this.DataProvider.Execute(args.MdxQuery, this.CurrentReport);
        }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="returnResult">if set to <c>true</c> [return result].</param>
        /// <returns>Object contains the cell set information.</returns>
        public object Execute(string commandText, bool returnResult)
        {
            return (this.DataProvider as AdomdDataProvider).Execute(commandText, returnResult);
        }

        /// <summary>
        /// Gets the MDX query.
        /// </summary>
        /// <returns>String contains the MDX Query.</returns>
        public string GetMDXQuery()
        {
            if (this.MdxQuery != string.Empty)
            {
                return this.MdxQuery;
            }

            MDXQuerySpecification querySpecification = (this.UseSharedDataManager && this.ActiveReport != null) ? this.GetMDXQuerySpecForActiveReport() :this.GetMDXQuerySpecification();
            string mdxQuery = "";
            if (querySpecification != null)
            {
                mdxQuery = QueryBuilderEngine.GenerateQueryEx(querySpecification, false, this.DataProvider.ProviderName, this.CurrentReport.SlicerRangeFilters, this.CurrentReport.DrilledCells, this.ShowLevelTypeAll, this.CurrentReport.DrillType,this.CurrentReport.VisualTotalVisibility,this.CurrentReport.UseDefaultMember);
                if (this.UseWhereClauseForSlicing && this.DataProvider.ProviderName == Providers.SSAS && (querySpecification.Slicer.Items.List.Where(i => i.ElementValue is NamedSetElement).Count() > 0))
                    this.UseWhereClauseForSlicing = false;
            }
            if (!((this.UseSharedDataManager && this.ActiveReport != null) ?this.ActiveReport.UseWhereClauseForSlicing:this.CurrentReport.UseWhereClauseForSlicing) && this.DataProvider.ProviderName == Providers.SSAS)
            {
                string[] commandTextSplit = mdxQuery.Split(new string[] { " WHERE " }, StringSplitOptions.None);
                if (commandTextSplit.Count() >= 2)
                {
                    mdxQuery = (commandTextSplit[0].Substring(0, commandTextSplit[0].LastIndexOf("[" + querySpecification.CubeName)) + " (SELECT {" + commandTextSplit[commandTextSplit.Count() - 1] + "} ON 0 FROM [" + querySpecification.CubeName + "])").Replace("CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE", "") + " CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE";
                }
            }

            return mdxQuery;

        }

        internal string GetMDXQuery(MDXQuerySpecification querySpecification)
        {
            if (querySpecification == null)
                return null;
            return QueryBuilderEngine.GenerateQueryEx(querySpecification, false, Providers.SSAS, this.CurrentReport.SlicerRangeFilters, this.CurrentReport.DrilledCells, false, this.CurrentReport.DrillType,this.CurrentReport.VisualTotalVisibility,this.CurrentReport.UseDefaultMember);
        }

        internal void RaiseQueryExecuting(QueryExecutingEventArgs args)
        {
            if (BeforeMdxQueryExecute != null)
                BeforeMdxQueryExecute(this, args);
        }

        /// <summary>
        /// Gets the row and column count.
        /// </summary>
        /// <param name="querySpecification">The query specification.</param>
        /// <returns>An integer array contains row and column count.</returns>
        public int[] GetRowAndColumnCount(MDXQuerySpecification querySpecification)
        {
            return null; // QueryBuilderEngine.GenerateQueryEx(querySpecification);
        }

        /// <summary>
        /// Get the drill down/up query for the cell descriptor passed
        /// </summary>
        /// <param name="cellDescriptor">Drill down member</param>
        /// <returns>The MDX Query after performing a drilldown operation.</returns>
        public string GetDrillDownMDXQuery(PivotCellDescriptor cellDescriptor)
        {
            Member memberObj = cellDescriptor.Tag as Member;
            if (memberObj != null)
            {
                return GetDrillDownMDXQuery(cellDescriptor.CellType, memberObj);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the drill down MDX query.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="memberObj">The member obj.</param>
        /// <returns>The MDX Query after performing a drilldown operation.</returns>
        public string GetDrillDownMDXQuery(PivotCellDescriptorType cellType, Member memberObj)
        {
            //// Parameter false indicate the events associated with in the methods should
            //// not be triggered
            this.ToggleExpandableState(cellType, memberObj, false);

            //// returns the MDX query
            return this.GetMDXQuery();
        }

        /// <summary>
        /// Executes the count.
        /// </summary>
        /// <returns></returns>
        public SerializableDictionary<string, int> ExecuteCount()
        {
            var count = new SerializableDictionary<string, int>();
            int[] counts = null;
            var querySpecification = this.GetMDXQuerySpecification();
            if (querySpecification != null)
            {
                this.CurrentReport.ConnectionString = this.DataProvider.ConnectionString;
                string commandText = QueryBuilderEngine.GenerateQueryEx(querySpecification, true, this.DataProvider.ProviderName, this.CurrentReport.SlicerRangeFilters, this.CurrentReport.DrilledCells, this.ShowLevelTypeAll, this.CurrentReport.DrillType,this.CurrentReport.VisualTotalVisibility,this.CurrentReport.UseDefaultMember);
                string[] commandTextSplit = commandText.Split(new string[] { " WHERE " }, StringSplitOptions.None);
                if (commandTextSplit.Count() >= 2 && !(this.DataProvider.ProviderName == Providers.Mondrian))
                {
                    String commandQuery = (commandTextSplit[0].Substring(0, commandTextSplit[0].LastIndexOf("[" + querySpecification.CubeName)) + " (SELECT {" + commandTextSplit[commandTextSplit.Count() - 1] + "} ON 0 FROM [" + querySpecification.CubeName + "])").Replace("CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE", "") + " CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE";

                    var args = new QueryExecutingEventArgs(commandQuery, "ExecuteCount", this.CurrentReport);
                    RaiseQueryExecuting(args);
                    if (args.Cancel) return null;
                    (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                    counts = this.DataProvider.ExecuteCount(args.MdxQuery);
                }
                else
                {
                    if ((this.DataProvider.ProviderName == Providers.Mondrian))
                        commandText = commandText.Replace(" NON EMPTY", "").Replace(" dimension properties MEMBER_TYPE, PARENT_UNIQUE_NAME", "").Replace(" CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE", "").Replace("VISUALTOTALS", "");

                    var args = new QueryExecutingEventArgs(commandText, "ExecuteCount", this.CurrentReport);
                    RaiseQueryExecuting(args);
                    if (args.Cancel) return null;
                    (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                    counts = this.DataProvider.ExecuteCount(args.MdxQuery);
                }

                if (counts != null)
                {
                    count["Column"] = counts[0];
                    count["Row"] = counts[1];
                }
            }
            return count;
        }

        /// <summary>
        /// Executes the cell set.
        /// </summary>
        /// <param name="querySpecification">The query specification.</param>
        /// <returns></returns>
        [Obsolete("Use ExecuteCellSet instead")]
        public CellSet ExecuteCellSet(MDXQuerySpecification querySpecification)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (querySpecification != null)
            {
                if (this.ActiveReport == null && !UseSharedDataManager)
                {
                    this.CurrentReport.ConnectionString = this.DataProvider.ConnectionString;
                    if (ValidateCurrentReport(this.CurrentReport))
                        this.ProcessCurrentReport(this.CurrentReport);

                    //Condition used to check the namedsets in slicer axis in order to avoid the circular reference.
                    if (this.CurrentReport.UseWhereClauseForSlicing && this.DataProvider.ProviderName == Providers.SSAS && (querySpecification.Slicer.Items.List.Where(i => i.ElementValue is NamedSetElement).Count() > 0))
                        this.CurrentReport.UseWhereClauseForSlicing = false;

                    string commandText = QueryBuilderEngine.GenerateQueryEx(querySpecification, false, this.DataProvider.ProviderName, this.CurrentReport.SlicerRangeFilters, this.CurrentReport.DrilledCells, this.ShowLevelTypeAll, this.CurrentReport.DrillType,this.CurrentReport.VisualTotalVisibility,this.CurrentReport.UseDefaultMember);
                    if (CurrentReport.UseWhereClauseForSlicing || this.DataProvider.ProviderName == Providers.ActivePivot || this.DataProvider.ProviderName == Providers.Mondrian)
                    {
                        var args = new QueryExecutingEventArgs(commandText, "ExecuteCellSet", this.CurrentReport);
                        RaiseQueryExecuting(args);
                        if (args.Cancel) return null;

                        this.IsCurrentReportModified = true;
                        if (this.DataProvider is AdomdDataProvider)
                            (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                        m_cellSet = this.DataProvider.ExecuteCellSet(args.MdxQuery, !this.CurrentReport.ShowExpanders, false, this.CurrentReport);
                    }
                    else
                    {
                        string[] commandTextSplit = commandText.Split(new string[] { " WHERE " }, StringSplitOptions.None);
                        if (commandTextSplit.Count() >= 2)
                        {
                            commandText = (commandTextSplit[0].Substring(0, commandTextSplit[0].LastIndexOf("[" + querySpecification.CubeName)) + " (SELECT {" + commandTextSplit[commandTextSplit.Count() - 1] + "} ON 0 FROM [" + querySpecification.CubeName + "])").Replace("CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE", "") + " CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE";
                        }
                        var args = new QueryExecutingEventArgs(commandText, "ExecuteCellSet", this.CurrentReport);
                        RaiseQueryExecuting(args);
                        if (args.Cancel) return null;

                        this.IsCurrentReportModified = true;
                        (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                        m_cellSet = this.DataProvider.ExecuteCellSet(args.MdxQuery, !this.CurrentReport.ShowExpanders, false, this.CurrentReport);
                    }
                }
                else
                {
                    this.ActiveReport.ConnectionString = this.DataProvider.ConnectionString;
                    if (ValidateCurrentReport(this.ActiveReport))
                        this.ProcessCurrentReport(this.ActiveReport);

                    //Condition used to check the namedsets in slicer axis in order to avoid the circular reference.
                    if (this.ActiveReport.UseWhereClauseForSlicing && this.DataProvider.ProviderName == Providers.SSAS && (querySpecification.Slicer.Items.List.Where(i => i.ElementValue is NamedSetElement).Count() > 0))
                        this.ActiveReport.UseWhereClauseForSlicing = false;

                    string commandText = QueryBuilderEngine.GenerateQueryEx(querySpecification, false, this.DataProvider.ProviderName, this.ActiveReport.SlicerRangeFilters, this.ActiveReport.DrilledCells, this.ShowLevelTypeAll, this.ActiveReport.DrillType,this.ActiveReport.VisualTotalVisibility,this.CurrentReport.UseDefaultMember);
                    if (ActiveReport.UseWhereClauseForSlicing || this.DataProvider.ProviderName == Providers.ActivePivot || this.DataProvider.ProviderName == Providers.Mondrian)
                    {
                        var args = new QueryExecutingEventArgs(commandText, "ExecuteCellSet", this.ActiveReport);
                        RaiseQueryExecuting(args);
                        if (args.Cancel) return null;

                        this.IsCurrentReportModified = true;
                        (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                        m_cellSet = this.DataProvider.ExecuteCellSet(args.MdxQuery, !this.ActiveReport.ShowExpanders, false, this.ActiveReport);
                    }
                    else
                    {
                        string[] commandTextSplit = commandText.Split(new string[] { " WHERE " }, StringSplitOptions.None);
                        if (commandTextSplit.Count() >= 2)
                        {
                            commandText = (commandTextSplit[0].Substring(0, commandTextSplit[0].LastIndexOf("[" + querySpecification.CubeName)) + " (SELECT {" + commandTextSplit[commandTextSplit.Count() - 1] + "} ON 0 FROM [" + querySpecification.CubeName + "])").Replace("CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE", "") + " CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE";
                        }
                        var args = new QueryExecutingEventArgs(commandText, "ExecuteCellSet", this.ActiveReport);
                        RaiseQueryExecuting(args);
                        if (args.Cancel) return null;

                        this.IsCurrentReportModified = true;
                        (this.DataProvider as AdomdDataProvider).ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                        m_cellSet = this.DataProvider.ExecuteCellSet(args.MdxQuery, !this.ActiveReport.ShowExpanders, false, this.ActiveReport);
                    }
                }
                this.MdxQuery = string.Empty;
#if DEBUG
                var watch = new Stopwatch();
                watch.Start();
#endif
                if (m_cellSet.Axes.Count > 0)
                {
                    if (m_cellSet.Axes[0].TupleSet.Count > 0)
                    {
                        if (querySpecification.IsKPI)
                        {
                            UpdateKPIStatus(querySpecification.KPIAxis);
                        }
                        if (querySpecification.Select.Items.List.Any(i=>i.ElementValue is VirtualKpiElement))
                        {
                            UpdateVirtualKPIStatus(querySpecification.Select.Items.List.Where(i => i.ElementValue is VirtualKpiElement).FirstOrDefault().Axis);
                        }
                    }
                    else
                    {
                        m_cellSet = null;
                    }
                }
                else
                {
                    m_cellSet = null;
                }
#if DEBUG
                watch.Stop();
                Debug.WriteLine("KPI Time : {0}", watch.ElapsedMilliseconds.ToString());

                sw.Stop();
                Console.WriteLine("Time taken form OlapDataManager.ExecuteCellSet -- CellSet Generation {0}", sw.Elapsed);
                sw.Reset();
                sw.Start();
#endif
                ////this.ExecuteOlapTable(m_cellSet);
            }
            else
            {
                m_cellSet = null;
            }
#if DEBUG
            sw.Stop();
            //Console.WriteLine("Time taken form OlapDataManager.ExecuteCellSet -- PivotEngine Generation {0}", sw.Elapsed);
#endif
            this.UseWhereClauseForSlicing = _useWhereBackup;
            this.CurrentReport.UseWhereClauseForSlicing = _useWhereBackup;
            return m_cellSet;
        }

        /// <summary>
        /// Based on the current report defined this method process it, and returns you the
        /// MDX cell set
        /// </summary>
        /// <returns>The Cell set after running the MDXQuery or QuerySpecification</returns>
        public virtual CellSet ExecuteCellSet()
        {
            CellSet cellSet = null;
            if (this.MdxQuery != string.Empty)
            {
                if (AllowMdxToOlapReportParse)
                {
                    try
                    {
                        this._currentReport = MDXQueryParser.MDXToOlapParser.GenerateOlapReport(this.MdxQuery);
                        this.Reports.Add(this._currentReport);
                        _currentCubeName = this._currentReport.CurrentCubeName;
                        cellSet = this.ExecuteCellSet(this.GetMDXQuerySpecification());
                    }
                    catch
                    {
                        cellSet = this.ExecuteCellSet(this.MdxQuery);
                    }
                }
                else
                {
                    cellSet = this.ExecuteCellSet(this.MdxQuery);
                }
               
            }
            else if (this.ActiveReport != null && this.UseSharedDataManager)
            {
                cellSet = this.ExecuteCellSet(this.GetMDXQuerySpecForActiveReport());
            }
            else
            {
                cellSet = this.ExecuteCellSet(this.GetMDXQuerySpecification());
            }

            return cellSet;
        }

        /// <summary>
        /// Based on the current report defined this method process it and generates the cellset. From the 
        /// cellset a 2 dimension representation of pivot engine is generated and it returns the same
        /// </summary>
        /// <returns>Returns the engine (2 dimensional representation of cellset)</returns>
        public PivotEngine ExecuteOlapTable()
        {
            if (this.ItemSource == null)
            {
                CellSet cellSet = this.ExecuteCellSet();
                this.m_pivotEngine = this.ExecuteOlapTable(cellSet);
            }
            return m_pivotEngine;
        }

        /// <summary>
        /// Executes the OlapTable based on the layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>Returns the engine (2 dimensional representation of CellSet)</returns>
        public PivotEngine ExecuteOlapTable(GridLayout layout)
        {
            if (this.ItemSource == null)
            {
                CellSet cellSet = this.ExecuteCellSet();
                this.m_pivotEngine = this.ExecuteOlapTable(cellSet, layout);
            }
            else
            {
                PivotElements pivotDataElements = new PivotElements(this.CurrentReport);
                if (this.ItemSource is IEnumerable)
                {
                    IEnumerable source = this.ItemSource as IEnumerable;
                    if (source != null)
                    {
                        this.m_pivotEngine = TableBuilder.BuildEngineFromIQueryable(source.AsQueryable(), pivotDataElements.Report, this.RelationalDataSortOrder,
                            pivotDataElements.ColumnItems.ToArray(),
                            pivotDataElements.SeriesItems.ToArray(),
                            pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, layout, pivotDataElements.ExpandAll, null, false);
                    }
                }
                else if (this.ItemSource is IListSource)
                {
                    IListSource source = this.ItemSource as IListSource;
                    this.m_pivotEngine = TableBuilder.BuildEngineFromIListSource(source, pivotDataElements.Report, this.RelationalDataSortOrder,
                        pivotDataElements.ColumnItems.ToArray(),
                        pivotDataElements.SeriesItems.ToArray(),
                        pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, layout, pivotDataElements.ExpandAll, pivotDataElements.SummaryStringCount, null, false);
                }
            }

            return m_pivotEngine;
        }

        /// <summary>
        /// Executes the OlapTable by using command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Returns the engine (2 dimensional representation of CellSet)</returns>
        public PivotEngine ExecuteOlapTable(string commandText)
        {
            this.ExecuteCellSet(commandText);
            return ExecuteOlapTable(m_cellSet);
        }

        /// <summary>
        /// Executes the OlapTable based on <see cref="CellSet"/>
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <returns>Returns the engine (2 dimensional representation of CellSet)</returns>
        public PivotEngine ExecuteOlapTable(CellSet cellSet)
        {
            m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Bottom, GridLayout.Normal, false, false, this);
            return m_pivotEngine;
        }

        /// <summary>
        /// Executes the OlapTable based on <see cref="CellSet"/> and <see cref="GridLayout"/>.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>Returns the engine (2 dimensional representation of CellSet)</returns>
        public PivotEngine ExecuteOlapTable(CellSet cellSet, GridLayout layout)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (cellSet != null && cellSet.ColumnMaxLevel > 0)
            {
                bool isMdx = this.MdxQuery != string.Empty;

                if (layout == GridLayout.Normal || cellSet.Axes.Count < 2)
                {
                    if (layout == GridLayout.NormalTopSummary)
                    {
                        m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Top, layout, this.ShowLevelTypeAll, isMdx, this);
                    }
                    else if (layout == GridLayout.NoSummaries)
                    {
                        m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.None, layout, this.ShowLevelTypeAll, isMdx, this);
                    }
                    else
                    {
                        m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Bottom, layout, this.ShowLevelTypeAll, isMdx, this);
                    }
                }
                else if (layout == GridLayout.NormalTopSummary)
                {
                    m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Top, layout, this.ShowLevelTypeAll, isMdx, this);
                }
                else if (layout == GridLayout.ExcelLikeLayout || layout == GridLayout.ExcelLikeLayoutWithMemberProperties)
                {
                    m_pivotEngine = TableBuilder.BuildEngineFromCellSetforExcelLayout(cellSet, null, isMdx, layout, this.ShowLevelTypeAll, this);
                }
                else if (layout == GridLayout.NoSummaries)
                {
                    m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.None, layout, this.ShowLevelTypeAll, isMdx, this);
                }
            }
            else
            {
                this.m_pivotEngine = null;
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Time taken form OlapDataManager.ExecuteOlapTable -- Engine Generation {0}" + sw.Elapsed);

#endif

            return m_pivotEngine;
        }

        /// <summary>
        /// Returns OLAP table with expanded rows only.
        /// </summary>
        /// <param name="cellDescriptor">Descriptor to get expand rows for.</param>
        /// <returns>the pivot engine with expanded rows.</returns>
        public PivotEngine GetExpandedRows(PivotCellDescriptor cellDescriptor)
        {
            CubeSchema cubeSchema = this.CurrentCubeSchema;

            var cellData = this.PivotEngine.GetCellData(cellDescriptor);
            Member member = cellDescriptor.Tag as Member ?? cubeSchema.GetMemberByUniqueName(cellDescriptor.UniqueName, false);

            PivotEngine expandTable = null;
            PivotCellDescriptorType cellType = cellDescriptor.CellType;
            if (this.CurrentReport != null)
            {
                if (this.CurrentReport.TogglePivot)
                {
                    switch (cellType)
                    {
                        case PivotCellDescriptorType.ColumnHeader:
                            cellType = PivotCellDescriptorType.RowHeader;
                            break;
                        case PivotCellDescriptorType.RowHeader:
                            cellType = PivotCellDescriptorType.ColumnHeader;
                            break;
                    }
                }
            }

            OlapDataManager cloneOfMainOlapDataManager = this.CloneOlapDataManagerElements();

            if (member != null)
            {
                if (this.CurrentReport.CategoricalElements.Count > 0)
                {
                    #region New Cube Model
                    ///// Creating a temporary model and adding the required elements to the
                    ///// appripriate element properties
                    var model = new OlapDataManager
                        {
                            ConnectionString = this.ConnectionString,
                            CurrentReport =
                                {
                                    CurrentCubeName = cubeSchema.CubeInfo.Name,
                                    TogglePivot = cloneOfMainOlapDataManager.CurrentReport.TogglePivot,
                                    EngineVersion = cloneOfMainOlapDataManager.CurrentReport.EngineVersion
                                },
                            DataProvider = this.DataProvider
                        };
                    ///// Using the same dataprovider object to Maintain the same conneciton
                    model.NotifyCubeNameChanged(cubeSchema.CubeInfo.Name, false);

                    ///// Slicer will not be changed at drill down so adding all elements
                    foreach (Item item in cloneOfMainOlapDataManager.CurrentReport.SlicerElements)
                    {
                        model.CurrentReport.SlicerElements.Add((Item)item.Clone());
                    }

                    Items cloneUnchangeItems = null, cloneChangeItems = null, unchangeItems = null, changeItems = null;
                    HeaderInfoCollection infos = null;

                    switch (cellType)
                    {
                        case PivotCellDescriptorType.ColumnHeader:
                            cloneUnchangeItems = cloneOfMainOlapDataManager.CurrentReport.SeriesElements;
                            unchangeItems = model.CurrentReport.SeriesElements;
                            cloneChangeItems = cloneOfMainOlapDataManager.CurrentReport.CategoricalElements;
                            changeItems = model.CurrentReport.CategoricalElements;
                            if (cellData != null) infos = cellData.ColumnInfo;
                            break;
                        case PivotCellDescriptorType.RowHeader:
                            cloneUnchangeItems = cloneOfMainOlapDataManager.CurrentReport.CategoricalElements;
                            unchangeItems = model.CurrentReport.CategoricalElements;
                            cloneChangeItems = cloneOfMainOlapDataManager.CurrentReport.SeriesElements;
                            changeItems = model.CurrentReport.SeriesElements;
                            if (cellData != null) infos = cellData.RowInfo;
                            break;
                    }

                    if (cloneChangeItems != null)
                    {
                        foreach (Item item in cloneUnchangeItems)
                        {
                            unchangeItems.Add((Item)item.Clone());
                        }

                        var measureElements = cloneChangeItems.List.Where(i => i.ElementValue is MeasureElements).Select(i => i);
                        var measureElementsList = measureElements as IList<Item> ?? measureElements.ToList();
                        if (measureElementsList.Any())
                        {
                            changeItems.Add(measureElementsList.First() as Item);
                        }
                        var memberIndex = cloneChangeItems.List.FindIndex(e => e.ElementValue is DimensionElement && (e.ElementValue as DimensionElement).HierarchyName == member.ParentHierarchy && (e.ElementValue as DimensionElement).Name == member.ParentDimension);
                        MemberElement memberElement = QueryBuilderEngineHelper.GetMemberElement(member, true);

                        for (int i = 0; i < cloneChangeItems.Count; i++)
                        {
                            var dimensionElement = cloneChangeItems[i].ElementValue as DimensionElement;

                            if (dimensionElement != null && infos != null)
                            {
                                var expandedCells = infos.Where(info => info.Member.ParentHierarchy == dimensionElement.HierarchyName && info.Member.ParentDimension == dimensionElement.Name);
                                string uniqueName = expandedCells.Aggregate("", (current, cell) => current + (cell.UniqueName + ","));
                                if (i == memberIndex)
                                {
                                    uniqueName += string.Format("{0},{0}.Children", memberElement.UniqueName);
                                }
                                uniqueName = uniqueName.TrimEnd(',');
                                if (uniqueName.Length > 0)
                                {
                                    var nameSet = new NamedSetElement() { UniqueName = uniqueName };
                                    changeItems.Add(new Item { Axis = cloneChangeItems[i].Axis, ElementValue = nameSet });
                                    continue;
                                }
                            }
                            if (i == memberIndex)
                            {
                                changeItems.Add(new Item { Axis = cloneChangeItems[i].Axis, ElementValue = memberElement });
                                continue;
                            }
                            if (measureElementsList.Any() && cloneChangeItems[i] != measureElementsList.First())
                                changeItems.Add(cloneChangeItems[i].Clone());
                        }
                    }

                    ////if (isDrillDownAvailable)
                    {
                        expandTable = model.ExecuteOlapTable(GridLayout.NoSummaries);
                    }

                    #endregion
                }
            }

            return expandTable;
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A Report collection<see cref="OlapReportCollection"/>.</returns>
        public OlapReportCollection GetReport(string fileName)
        {
            return Syncfusion.Olap.Common.Common.XmlToFromFile(fileName, typeof(OlapReportCollection)) as OlapReportCollection;
        }

        /// <summary>
        /// Loads the <see cref="OlapDataManager"/> based on <see cref="OlapReport"/>.
        /// </summary>
        /// <param name="report">The report.</param>
        public void LoadOlapDataManager(OlapReport report)
        {
            this.CurrentReport.ShowEmptyRowData = report.ShowEmptyRowData;
            this.CurrentReport.ShowEmptyColumnData = report.ShowEmptyColumnData;
            ////this.CurrentReport.ShowGrandTotal = report.ShowGrandTotal;
            this.CurrentReport.TogglePivot = report.TogglePivot;
            this.CurrentCubeName = report.CurrentCubeName;
            this.CurrentReport.CategoricalElements.Clear();
            if (report.CategoricalElements.Count > 0)
            {
                foreach (Item item in report.CategoricalElements)
                {
                    this.CurrentReport.CategoricalElements.Add(item);
                }

                this.CurrentReport.CategoricalElements.IsFilterOrSortOn = report.CategoricalElements.IsFilterOrSortOn;
            }

            this.CurrentReport.SeriesElements.Clear();
            if (report.SeriesElements.Count > 0)
            {
                foreach (Item item in report.SeriesElements)
                {
                    this.CurrentReport.SeriesElements.Add(item);
                }

                this.CurrentReport.SeriesElements.IsFilterOrSortOn = report.SeriesElements.IsFilterOrSortOn;
            }

            this.CurrentReport.SlicerElements.Clear();
            if (report.SlicerElements.Count > 0)
            {
                foreach (Item item in report.SlicerElements)
                {
                    this.CurrentReport.SlicerElements.Add(item);
                }

                this.CurrentReport.SlicerElements.IsFilterOrSortOn = report.SlicerElements.IsFilterOrSortOn;
            }

            this.CurrentReport.FilterElements.Clear();
            if (report.FilterElements.Count > 0)
            {
                foreach (Item item in report.FilterElements)
                {
                    this.CurrentReport.FilterElements.Add(item);
                }

                this.CurrentReport.FilterElements.IsFilterOrSortOn = report.FilterElements.IsFilterOrSortOn;
            }

            this.NotifyElementModified();
            //this.NotifyDrillDown();
        }

        /// <summary>
        /// Loads the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        public void LoadReport(string reportName)
        {
            if (this.Reports.Count > 0)
            {
                if (this.Reports[reportName] != null)
                {
                    this.CurrentReport = this.Reports[reportName];
                    this.IsCurrentReportModified = false;
                }
                else
                {
                    throw new OlapDataManagerException("The specified report is not exist in the Reports");
                }
            }
            else
            {
                throw new OlapDataManagerException("There is no report in the Reports");
            }
        }

        /// <summary>
        /// Loads the report definition file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadReportDefinitionFile(string fileName)
        {
            try
            {
                // Loads the reports form the file
                this.Reports = this.GetReport(fileName);
                foreach (OlapReport item in this.Reports)
                {
                    if (item.EngineVersion == QueryBuilderEngineVersions.None)
                    {
                        item.EngineVersion = QueryBuilderEngineVersions.Version3;
                    }
                }

                this.ReportPath = fileName;
            }
            catch (Exception ex)
            {
                throw new OlapDataManagerException(ex);
            }
        }

        /// <summary>
        /// Loads the report definition from stream.
        /// </summary>
        /// <param name="reportStream">The report stream.</param>
        public void LoadReportDefinitionFromStream(Stream reportStream)
        {
            TextReader text = new StreamReader(reportStream);

            this.Reports = new XmlSerializer(typeof(OlapReportCollection)).Deserialize(text) as OlapReportCollection;

            foreach (OlapReport item in this.Reports)
            {
                if (item.EngineVersion == QueryBuilderEngineVersions.None)
                {
                    item.EngineVersion = QueryBuilderEngineVersions.Version3;
                }
            }
        }

        /// <summary>
        /// Gets the report as stream.
        /// </summary>
        /// <returns>A stream object.</returns>
        public Stream GetReportAsStream()
        {
            MemoryStream reportStream = new MemoryStream();
            if (this.Reports != null && this.Reports.Count > 0)
            {
                OlapReportCollection clonedReports = new OlapReportCollection();
                foreach (OlapReport report in this.Reports)
                {
                    clonedReports.Add(report.Clone());
                }
                string reportString = Syncfusion.Olap.Common.Common.ToXml(clonedReports, false);

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(reportString);
                reportStream.Write(buffer, 0, buffer.Length);
            }
            return reportStream;
        }

        //public void NotifyDrillDown(AxisPosition axisPosition)
        //{
        //    this.OnDrillDown(this, new DrillDownEventArgs(axisPosition));
        //}

        //public void NotifyDrillDown()
        //{
        //    this.OnDrillDown(this, new DrillDownEventArgs());
        //}

        /// <summary>
        /// Notifies the element modified.
        /// </summary>
        public void NotifyElementModified()
        {
            if (this.AxisElementChanged != null)
            {
                this.OnAxisElementChanged(this, new AxisElementChangedEventArgs());
            }
        }
        public void ToggleAxis(OlapReport olapReport)
        {
            int rowCount = olapReport.SeriesElements.Count; 
            int colsCount= olapReport.CategoricalElements.Count;
            for (int i = 0; i < colsCount; i++)
            {
                olapReport.SeriesElements.Add(olapReport.CategoricalElements[0]);
                olapReport.CategoricalElements.RemoveAt(0);
            }
            olapReport.CategoricalElements.RemoveAll(AxisPosition.Categorical);
            for (int j = 0; j < rowCount; j++)
            {
                olapReport.CategoricalElements.Add(olapReport.SeriesElements[0]);
                olapReport.SeriesElements.RemoveAt(0);
            }
        }

        /// <summary>
        /// Raises the axis element modified.
        /// </summary>
        public void RaiseAxisElementModified()
        {
            this.OnAxisElementModified(this, new AxisElementModifiedEventArgs());
        }

        /// <summary>
        /// Notifies the report changed.
        /// </summary>
        public void NotifyReportChanged()
        {
            if (this.ReportChanged != null)
            {
                this.OnReportChanged(this, new ReportChangedEventArgs(this.CurrentReport));
            }
            //this.NotifyElementModified();
        }

        /// <summary>
        /// Notifies the report changed.
        /// </summary>
        /// <param name="currentReport">The current report.</param>
        public void NotifyReportChanged(OlapReport currentReport)
        {
            if (this.ReportChanged != null)
            {
                this.OnReportChanged(this, new ReportChangedEventArgs(currentReport));
            }
            //this.NotifyElementModified();
        }

        /// <summary>
        /// Notifies the active report changed
        /// </summary>
        public void NotifyActiveReportChanged()
        {
            if (this.ActiveReportChanged != null)
            {
                this.OnActiveReportChanged(this, new ActiveReportChangedEventArgs(this.ActiveReport,true));
            }
        }

        /// <summary>
        /// Notifies the active report changed
        /// </summary>
        public void NotifyActiveReportChanged(OlapReport activeReport)
        {
            if (this.ActiveReportChanged != null)
            {
                this.OnActiveReportChanged(this, new ActiveReportChangedEventArgs(activeReport, true));
            }
        }

        /// <summary>
        /// Notifies the active report changed
        /// </summary>
        public void NotifyActiveReportChanged(OlapReport activeReport,bool isReportChanged)
        {
            if (this.ActiveReportChanged != null)
            {
                this.OnActiveReportChanged(this, new ActiveReportChangedEventArgs(activeReport, isReportChanged));
            }
        }


        /// <summary>
        /// Notifies the element modified.
        /// </summary>
        /// <param name="axisPosition">The axis position.</param>
        public void NotifyElementModified(AxisPosition axisPosition)
        {
            if (this.AxisElementChanged != null)
            {
                this.OnAxisElementChanged(this, new AxisElementChangedEventArgs(axisPosition));
            }
        }

        /// <summary>
        /// Removes the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        public void RemoveReport(string reportName)
        {
            OlapReport report = this.Reports[reportName];
            if (report != null)
            {
                this.Reports.Remove(report);
                if (this.Reports.Count > 0)
                {
                    this.CurrentReport = this.Reports[0];
                }
                else
                {
                    this.CurrentReport = new OlapReport();
                    //this.CurrentReport.Model = this;
                }

                this.IsCurrentReportModified = true;
            }
            else
            {
                throw new OlapDataManagerException(string.Format("{0} not exist in the reports", reportName));
            }
        }

        /// <summary>
        /// Renames the report.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="newReportName">New name of the report.</param>
        public void RenameReport(int index, string newReportName)
        {
            this.Reports[index].Name = newReportName;
            this.IsCurrentReportModified = true;
        }

        /// <summary>
        /// Renames the report.
        /// </summary>
        /// <param name="oldReportName">Old name of the report.</param>
        /// <param name="newReportName">New name of the report.</param>
        public void RenameReport(string oldReportName, string newReportName)
        {
            OlapReport report = this.Reports[newReportName];
            if (report == null)
            {
                this.Reports[oldReportName].Name = newReportName;
                this.IsCurrentReportModified = true;
            }
            else
            {
                throw new OlapDataManagerException("already a report exist with same name");
            }
        }

        /// <summary>
        /// Saves the report.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveReport(string fileName)
        {
            ////this.UpdateReportWithCurrentSetting();
            ////Checking for is report available else add a new one.
            OlapReportCollection clonedReports = new OlapReportCollection();
            this.ReportPath = fileName;
            foreach (OlapReport report in this.Reports)
            {
                clonedReports.Add(report.Clone());
            }

            /*            OlapReport report = this.Reports[this.CurrentReport.Name];
                        OlapReport clonedReport = FilteredMembersReport(this.CurrentReport.Clone());
                        if (report == null)
                        {
                            this.Reports.Add(clonedReport);
                        }
                        else
                        {
                            this.Reports.Add(clonedReport);
                        }*/

            Syncfusion.Olap.Common.Common.ToXml(clonedReports, fileName, false);
            ////LoadReports(fileName);
            this.IsCurrentReportModified = false;
        }

        //private OlapReport FilteredMembersReport(OlapReport clonedReport)
        //{
        //    clonedReport.CategoricalElements = MembersRemove(clonedReport.CategoricalElements);
        //    clonedReport.SeriesElements = MembersRemove(clonedReport.SeriesElements);
        //    clonedReport.SlicerElements = MembersRemove(clonedReport.SlicerElements);
        //    return clonedReport;
        //}

        //private Items MembersRemove(Items items)
        //{
        //    foreach (Item item in items)
        //    {
        //        if (item.ElementValue is DimensionElement)
        //        {
        //            DimensionElement dimensionElement = (DimensionElement)item.ElementValue;
        //            foreach (LevelElement levelElement in dimensionElement.Hierarchy.LevelElements)
        //            {
        //                if (levelElement.MemberElements.Count > 0 && !levelElement.IncludeAvailableMembers)
        //                {
        //                    levelElement.MemberElements.Clear();
        //                }
        //            }
        //        }

        //        if (item.ElementValue is HierarchyElement)
        //        {
        //            HierarchyElement hierarchyElement = (HierarchyElement)item.ElementValue;
        //            foreach (LevelElement levelElement in hierarchyElement.LevelElements)
        //            {
        //                if (levelElement.MemberElements.Count > 0 && !levelElement.IncludeAvailableMembers)
        //                {
        //                    levelElement.MemberElements.Clear();
        //                }
        //            }
        //        }

        //        if (item.ElementValue is LevelElement)
        //        {
        //            LevelElement levelElement = (LevelElement)item.ElementValue;
        //            if (levelElement.MemberElements.Count > 0 && !levelElement.IncludeAvailableMembers)
        //            {
        //                levelElement.MemberElements.Clear();
        //            }
        //        }
        //    }

        //    return items;
        //}

        /// <summary>
        /// Sets the current report.
        /// </summary>
        /// <param name="report">The report.</param>
        public void SetCurrentReport(OlapReport report)
        {
            bool reportExist = false;
            this.CurrentReport = report;
            if (this.Reports.Count > 0)
            {
                foreach (OlapReport item in this.Reports)
                {
                    if (item.Name.Equals(report.Name))
                    {
                        reportExist = true;
                    }
                }
            }

            if (!reportExist)
            {
                this.Reports.Add(report);
            }
            //if (this.CurrentReport != null)
            //{
            //    this.CurrentReport.Model = this;
            //}

            this.IsCurrentReportModified = false;
            //this.ReportPath = string.Empty;
        }

        /// <summary>
        /// Validates the current report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns></returns>
        private bool ValidateCurrentReport(OlapReport report)
        {
            foreach (Item _item in report.CategoricalElements)
            {
                if (_item.ElementValue is DimensionElement && (_item.ElementValue as DimensionElement).DrillState != DrillState.Default)
                    return true;

            }
            foreach (Item _item in report.SeriesElements)
            {
                if (_item.ElementValue is DimensionElement && (_item.ElementValue as DimensionElement).DrillState != DrillState.Default)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Validates the current report with expand all/collapse all/expand to specific level/collapse to specific level conditions.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns></returns>
        private OlapReport ProcessCurrentReport(OlapReport report)
        {
            List<Item> m_items = new List<Item>();
            List<string> m_excludeElementsUniqueName = new List<string>();
            List<string> m_includeElementsUniqueName = new List<string>();

            foreach (Item _item in report.CategoricalElements)
                GetExcludeAndIncludeElementValue(_item, m_items, m_excludeElementsUniqueName, m_includeElementsUniqueName);
            foreach (Item _item in report.SeriesElements)
                GetExcludeAndIncludeElementValue(_item, m_items, m_excludeElementsUniqueName, m_includeElementsUniqueName);

            foreach (Item _item in m_items)
            {
                DimensionElement _dimensionElement = _item.ElementValue as DimensionElement;
                if (_dimensionElement.DrillState == DrillState.ExpandAll)
                {
                    PopulateMemberElements(_item, report, m_excludeElementsUniqueName, m_includeElementsUniqueName);
                }
                else if (_dimensionElement.DrillState == DrillState.CollapseAll)
                {
                    ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].MemberElements.Clear();
                }
                else if (_dimensionElement.DrillState == DrillState.ExpandToLevel && String.IsNullOrEmpty(_dimensionElement.DrillUpDownMember))
                {
                    PopulateMemberElements(_item, report, m_excludeElementsUniqueName, m_includeElementsUniqueName);
                }
                else if (_dimensionElement.DrillState == DrillState.CollapseToLevel && String.IsNullOrEmpty(_dimensionElement.DrillUpDownMember))
                {
                    LevelCollection _levelCollection = this.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name == _item.ElementValue.Name).FirstOrDefault().Hierarchies.Select(j => j).Where(j => j.Name == (_item.ElementValue as DimensionElement).HierarchyName).FirstOrDefault().Levels;
                    int _levelIndex = 0;
                    if (!string.IsNullOrEmpty(_dimensionElement.DrillUpDownLevel) && this.ShowLevelTypeAll)
                        _levelIndex = _levelCollection.ToList().FindIndex(k => k.Name == _dimensionElement.DrillUpDownLevel) + 1;
                    else if (!string.IsNullOrEmpty(_dimensionElement.DrillUpDownLevel))
                        _levelIndex = _levelCollection.ToList().FindIndex(k => k.Name == _dimensionElement.DrillUpDownLevel);

                    if (_levelIndex <= 1)
                        ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].MemberElements.Clear();
                    else
                    {
                        foreach (MemberElement _memberElement in ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].MemberElements)
                            ClearChildMembers(_memberElement.ChildMemberElements, _levelIndex - 1);
                    }
                }
                else if (_dimensionElement.DrillState == DrillState.ExpandToLevel && !String.IsNullOrEmpty(_dimensionElement.DrillUpDownMember))
                {
                    var _member = this.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name.ToUpper() == _item.ElementValue.Name.ToUpper()).FirstOrDefault()
                        .Hierarchies.Select(j => j).Where(j => j.Name.ToUpper() == (_item.ElementValue as DimensionElement).HierarchyName.ToUpper()).FirstOrDefault()
                        .Levels.Select(k => k.Members).Where(k => k.FindByName(_dimensionElement.DrillUpDownMember) != null).FirstOrDefault().Select(m => m).Where(m => (m.Name.ToUpper() == _dimensionElement.DrillUpDownMember.ToUpper() || m.UniqueName.ToUpper() == _dimensionElement.DrillUpDownMember.ToUpper())).FirstOrDefault();
                    PopulateSpecificMemberElements(_item, _member, report, m_excludeElementsUniqueName, m_includeElementsUniqueName);
                }
                else if (_dimensionElement.DrillState == DrillState.CollapseToLevel && !String.IsNullOrEmpty(_dimensionElement.DrillUpDownMember))
                {
                    var _member = this.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name == _item.ElementValue.Name).FirstOrDefault()
                        .Hierarchies.Select(j => j).Where(j => j.Name == (_item.ElementValue as DimensionElement).HierarchyName).FirstOrDefault()
                        .Levels.Select(k => k.Members).Where(k => k.FindByName(_dimensionElement.DrillUpDownMember) != null).FirstOrDefault().Select(m => m).Where(m => m.Name == _dimensionElement.DrillUpDownMember).FirstOrDefault();

                    GetParentMember(_member, _dimensionElement.DrillUpDownLevel);
                    LevelElementCollection _levelElementCollection = ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements;
                    foreach (LevelElement _levelElement in _levelElementCollection)
                    {
                        if (m_tempParentMember.ParentMember == null)
                        {
                            if (m_tempParentMember.ChildMembers[0].LevelUniqueName == _levelElement.UniqueName)
                            {
                                _levelElement.MemberElements.Clear();
                                break;
                            }
                        }
                        else if (!this.ShowLevelTypeAll && m_tempParentMember.ParentMember.LevelDepth == 0)
                        {
                            if (m_tempParentMember.LevelUniqueName == _levelElement.UniqueName)
                            {
                                for (int i = _levelElement.MemberElements.Count - 1; i >= 0; i--)
                                {
                                    if (_levelElement.MemberElements[i].Name == m_tempParentMember.Name)
                                        _levelElement.MemberElements.RemoveAt(i);
                                }
                                break;
                            }
                        }
                        else
                            ClearSpecificMemberElements(_levelElement.MemberElements, this.m_tempParentMember);
                    }
                }
                _dimensionElement.DrillState = DrillState.Default;
                _dimensionElement.DrillUpDownLevel = _dimensionElement.DrillUpDownMember = string.Empty;
            }
            return report;
        }

        /// <summary>
        /// Gets the parent member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="drillUpDownLevel">The drill up down level.</param>
        private void GetParentMember(Member member, string drillUpDownLevel)
        {
            if (member.LevelUniqueName.Contains(drillUpDownLevel))
                m_tempParentMember = member;
            else if (member.ParentMember == null)
                m_tempParentMember = member;
            else
                GetParentMember(member.ParentMember, drillUpDownLevel);
        }

        /// <summary>
        /// Fills the child member element.
        /// </summary>
        /// <param name="memberElementCollection">The member element collection.</param>
        /// <param name="memberElement">The member element.</param>
        private void FillChildMemberElement(MemberElementCollection memberElementCollection, MemberElement memberElement)
        {
            foreach (MemberElement _memberElement in memberElementCollection)
            {
                if (_memberElement.Name == memberElement.ParentMemberElement.Name || _memberElement.UniqueName == memberElement.ParentMemberElement.UniqueName)
                {
                    memberElement.ParentMemberElement = _memberElement;
                    for (int i = _memberElement.ChildMemberElements.Count - 1; i >= 0; i--)
                    {
                        if (_memberElement.ChildMemberElements[i].Name == memberElement.Name)
                            _memberElement.ChildMemberElements.RemoveAt(i);
                    }
                    _memberElement.ChildMemberElements.Add(memberElement);
                    break;
                }
                else
                    FillChildMemberElement(_memberElement.ChildMemberElements, memberElement);
            }
        }

        /// <summary>
        /// Adds the child members.
        /// </summary>
        /// <param name="_memberElement">The _member element.</param>
        /// <param name="memberCollection">The member collection.</param>
        /// <param name="_levelIndex">Index of the _level.</param>
        /// <param name="_excludeElementsUniqueName">Name of the _exclude elements unique.</param>
        /// <param name="_dimensionName">Name of the _dimension.</param>
        private void AddChildMembers(MemberElement _memberElement, MemberCollection memberCollection, int _levelIndex, List<string> _excludeElementsUniqueName, string _dimensionName)
        {
            if (_levelIndex > 1 || _levelIndex == -1)
            {
                foreach (Member _member in memberCollection)
                {
                    if (!_excludeElementsUniqueName.Select(i => i).Where(i => i == _member.Name + "-" + _dimensionName).Any())
                    {
                        MemberElement _subMemberElement = new MemberElement()
                        {
                            IsParentLevel = false,
                            ParentLevelElement = null,
                            ParentMemberElement = _memberElement,
                            ShowChildMembers = true,
                            Level = _member.LevelDepth,
                            UniqueName = _member.UniqueName,
                            Name = _member.Name,
                            Visible = false
                        };
                        if (_levelIndex == -1)
                            AddChildMembers(_subMemberElement, _member.ChildMembers, _levelIndex, _excludeElementsUniqueName, _dimensionName);
                        else
                            AddChildMembers(_subMemberElement, _member.ChildMembers, _levelIndex - 1, _excludeElementsUniqueName, _dimensionName);
                        _memberElement.Add(_subMemberElement);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the exclude and include element value in current report.
        /// </summary>
        /// <param name="_item">The _item.</param>
        /// <param name="m_items">The m_items.</param>
        /// <param name="m_excludeElementsUniqueName">Name of the m_exclude elements unique.</param>
        /// <param name="m_includeElementsUniqueName">Name of the m_include elements unique.</param>
        private void GetExcludeAndIncludeElementValue(Item _item, List<Item> m_items, List<string> m_excludeElementsUniqueName, List<string> m_includeElementsUniqueName)
        {
            if (_item.ElementValue is DimensionElement)
            {
                if (_item.ExcludedElementValue != null)
                    GetExcludeElementsUniqueName(_item, m_excludeElementsUniqueName);
                if (_item.ElementValue != null)
                    GetIncludeElementsUniqueName(_item, m_includeElementsUniqueName);
                m_items.Add(_item);
            }
        }

        /// <summary>
        /// Gets the unique name of the excluded elements.
        /// </summary>
        /// <param name="_item">The _item.</param>
        /// <param name="m_excludeElementsUniqueName">Name of the m_exclude elements unique.</param>
        private void GetExcludeElementsUniqueName(Item _item, List<string> m_excludeElementsUniqueName)
        {
            var m_levelElements = (_item.ExcludedElementValue as DimensionElement).Hierarchy.LevelElements;
            foreach (LevelElement _levelElement in m_levelElements)
            {
                GetExcludeChildElementsUniqueName(_levelElement.MemberElements, m_excludeElementsUniqueName, _item.ExcludedElementValue.Name);
            }
        }

        /// <summary>
        /// Gets the unique name of the excluded child elements.
        /// </summary>
        /// <param name="memberElementCollection">The member element collection.</param>
        /// <param name="m_excludeElementsUniqueName">Name of the m_exclude elements unique.</param>
        /// <param name="_dimensionName">Name of the _dimension.</param>
        private void GetExcludeChildElementsUniqueName(MemberElementCollection memberElementCollection, List<string> m_excludeElementsUniqueName, string _dimensionName)
        {
            foreach (MemberElement _memberElement in memberElementCollection)
            {
                if (_memberElement.ChildMemberElements.Count == 0)
                    m_excludeElementsUniqueName.Add(_memberElement.Name + "-" + _dimensionName);
                else
                    GetExcludeChildElementsUniqueName(_memberElement.ChildMemberElements, m_excludeElementsUniqueName, _dimensionName);
            }
        }

        /// <summary>
        /// Gets the unique name of the included elements.
        /// </summary>
        /// <param name="_item">The _item.</param>
        /// <param name="m_includeElementsUniqueName">Name of the m_include elements unique.</param>
        private void GetIncludeElementsUniqueName(Item _item, List<string> m_includeElementsUniqueName)
        {
            if (((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements.Count > 0)
            {
                foreach (LevelElement _levelElement in ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements)
                {
                    foreach (MemberElement _memberElement in _levelElement.MemberElements)
                    {
                        if (_levelElement.IncludeAvailableMembers)
                            m_includeElementsUniqueName.Add(_memberElement.Name + "-" + _item.ElementValue.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Populates the member elements.
        /// </summary>
        /// <param name="_item">The _item.</param>
        /// <param name="report">The report.</param>
        /// <param name="m_excludeElementsUniqueName">Name of the m_exclude elements unique.</param>
        /// <param name="m_includeElementsUniqueName">Name of the m_include elements unique.</param>
        private void PopulateMemberElements(Item _item, OlapReport report, List<string> m_excludeElementsUniqueName, List<string> m_includeElementsUniqueName)
        {
            LevelCollection _levelCollection = this.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name.ToUpper() == _item.ElementValue.Name.ToUpper()).FirstOrDefault().Hierarchies.Select(j => j).Where(j => j.Name.ToUpper() == (_item.ElementValue as DimensionElement).HierarchyName.ToUpper()).FirstOrDefault().Levels;
            Level _level = _levelCollection.Select(k => k).Where(k => k.Name == ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].Name).FirstOrDefault();
            DimensionElement _dimensionElement = _item.ElementValue as DimensionElement;

            int _levelIndex = 0;
            if (!string.IsNullOrEmpty(_dimensionElement.DrillUpDownLevel) && this.ShowLevelTypeAll)
                _levelIndex = _levelCollection.ToList().FindIndex(k => k.Name == _dimensionElement.DrillUpDownLevel) + 1;
            else if (!string.IsNullOrEmpty(_dimensionElement.DrillUpDownLevel))
                _levelIndex = _levelCollection.ToList().FindIndex(k => k.Name == _dimensionElement.DrillUpDownLevel);

            bool _isExcludeOrInclude = m_excludeElementsUniqueName.Select(i => i).Where(i => i.Contains("-" + _item.ElementValue.Name)).Any() ||
                m_includeElementsUniqueName.Select(i => i).Where(i => i.Contains("-" + _item.ElementValue.Name)).Any();
            {
                MemberElement _parentMemberElement = null;
                if (this.ShowLevelTypeAll && _level.Members[0].ParentMember != null && (_levelIndex == 0 || _levelIndex > 1))
                {
                    _parentMemberElement = new MemberElement()
                    {
                        ParentLevelElement = ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0],
                        ParentMemberElement = null,
                        ShowChildMembers = true,
                        Level = _level.Members[0].ParentMember.LevelDepth,
                        UniqueName = _level.Members[0].ParentMember.UniqueName,
                        Name = _level.Members[0].ParentMember.Name,
                        Visible = false,
                        IsParentLevel = true
                    };
                    ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].MemberElements.Add(_parentMemberElement);
                }
                if (_dimensionElement.DrillState == DrillState.ExpandAll || (_parentMemberElement != null && _levelIndex > 2) || (_parentMemberElement == null && _levelIndex > 1))
                {
                    foreach (Member _member in _level.Members)
                    {
                        bool isValid = true;
                        if (_isExcludeOrInclude)
                            isValid = (!m_excludeElementsUniqueName.Select(i => i).Where(i => i == _member.Name + "-" + _item.ElementValue.Name).Any())
                                && (m_includeElementsUniqueName.Select(i => i).Where(i => i == _member.Name + "-" + _item.ElementValue.Name).Any() || m_includeElementsUniqueName.Count == 0);
                        if (isValid)
                        {
                            MemberElement _memberElement = new MemberElement()
                            {
                                ParentLevelElement = _parentMemberElement == null ? ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0] : null,
                                ParentMemberElement = _parentMemberElement == null ? null : _parentMemberElement,
                                ShowChildMembers = true,
                                Level = _member.LevelDepth,
                                UniqueName = _member.UniqueName,
                                Name = _member.Name,
                                Visible = false,
                                IsParentLevel = _parentMemberElement == null ? true : false
                            };
                            if (_dimensionElement.DrillState == DrillState.ExpandAll)
                                AddChildMembers(_memberElement, _member.ChildMembers, -1, m_excludeElementsUniqueName, _item.ElementValue.Name);
                            else if (this.ShowLevelTypeAll && _parentMemberElement != null)
                                AddChildMembers(_memberElement, _member.ChildMembers, _levelIndex - 2, m_excludeElementsUniqueName, _item.ElementValue.Name);
                            else
                                AddChildMembers(_memberElement, _member.ChildMembers, _levelIndex - 1, m_excludeElementsUniqueName, _item.ElementValue.Name);

                            if (this.ShowLevelTypeAll && _level.Members[0].ParentMember != null)
                                ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].MemberElements[0].ChildMemberElements.Add(_memberElement);
                            else
                                ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0].MemberElements.Add(_memberElement);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populates the specific member elements.
        /// </summary>
        /// <param name="_item">The _item.</param>
        /// <param name="_member">The _member.</param>
        /// <param name="report">The report.</param>
        /// <param name="m_excludeElementsUniqueName">Name of the m_exclude elements unique.</param>
        /// <param name="m_includeElementsUniqueName">Name of the m_include elements unique.</param>
        private void PopulateSpecificMemberElements(Item _item, Member _member, OlapReport report, List<string> m_excludeElementsUniqueName, List<string> m_includeElementsUniqueName)
        {
            DimensionElement _dimensionElement = _item.ElementValue as DimensionElement;
            int _levelIndex = this.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name.ToUpper() == _item.ElementValue.Name.ToUpper()).FirstOrDefault().Hierarchies.Select(j => j).Where(j => j.Name.ToUpper() == (_item.ElementValue as DimensionElement).HierarchyName.ToUpper()).FirstOrDefault().Levels.Select(k => k).Where(k => k.Name.ToUpper() == _dimensionElement.DrillUpDownLevel.ToUpper()).FirstOrDefault().LevelDepth;

            MemberElement _parentMemberElement = null;
            Member _parentMember = _member.ParentMember;
            if (!(_parentMember == null || (!this.ShowLevelTypeAll && _parentMember.LevelDepth == 0)))
            {
                _parentMemberElement = new MemberElement()
                {
                    ParentLevelElement = ((this.ShowLevelTypeAll && _parentMember.LevelDepth == 0) || (!this.ShowLevelTypeAll && _parentMember.LevelDepth == 1)) ? ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0] : null,
                    ParentMemberElement = null,
                    ShowChildMembers = true,
                    Level = _parentMember.LevelDepth,
                    UniqueName = _parentMember.UniqueName,
                    Name = _parentMember.Name,
                    Visible = false,
                    IsParentLevel = ((this.ShowLevelTypeAll && _parentMember.LevelDepth == 0) || (!this.ShowLevelTypeAll && _parentMember.LevelDepth == 1)) ? true : false
                };
            }

            MemberElement _memberElement = new MemberElement
            {
                ParentLevelElement = _parentMemberElement == null ? ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements[0] : null,
                ParentMemberElement = _parentMemberElement == null ? null : _parentMemberElement,
                ShowChildMembers = true,
                Level = _member.LevelDepth,
                UniqueName = _member.UniqueName,
                Name = _member.Name,
                Visible = false,
                IsParentLevel = _parentMemberElement == null ? true : false
            };
            if (this.ShowLevelTypeAll)
                AddChildMembers(_memberElement, _member.ChildMembers, _levelIndex - _memberElement.Level, m_excludeElementsUniqueName, _item.ElementValue.Name);
            else
                AddChildMembers(_memberElement, _member.ChildMembers, _levelIndex - _memberElement.Level, m_excludeElementsUniqueName, _item.ElementValue.Name);

            if (_memberElement.IsParentLevel)
            {
                LevelElementCollection _levelElementCollection = ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements;
                foreach (LevelElement _levelElement in _levelElementCollection)
                {
                    for (int i = _levelElement.MemberElements.Count - 1; i >= 0; i--)
                    {
                        if (_levelElement.MemberElements[i].Name == _memberElement.Name)
                            _levelElement.MemberElements.RemoveAt(i);
                    }
                    _levelElement.MemberElements.Add(_memberElement);
                }
            }
            else
            {
                LevelElementCollection _levelElementCollection = ((_item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).LevelElements;
                foreach (LevelElement _levelElement in _levelElementCollection)
                {
                    FillChildMemberElement(_levelElement.MemberElements, _memberElement);
                }
            }
        }

        /// <summary>
        /// Clears the child members.
        /// </summary>
        /// <param name="_memberElementCollection">The _member element collection.</param>
        /// <param name="_levelIndex">Index of the _level.</param>
        private void ClearChildMembers(MemberElementCollection _memberElementCollection, int _levelIndex)
        {
            foreach (MemberElement _memberElement in _memberElementCollection)
            {
                if (_levelIndex == 1)
                {
                    _memberElement.ParentMemberElement.ChildMemberElements.Clear();
                    break;
                }
                ClearChildMembers(_memberElement.ChildMemberElements, _levelIndex - 1);
            }
        }

        /// <summary>
        /// Clears the specific member elements.
        /// </summary>
        /// <param name="_memberElementCollection">The _member element collection.</param>
        /// <param name="member">The member.</param>
        private void ClearSpecificMemberElements(MemberElementCollection _memberElementCollection, Member member)
        {
            foreach (MemberElement _memberElement in _memberElementCollection)
            {
                if (_memberElement.UniqueName == member.ParentMember.UniqueName)
                {
                    for (int i = _memberElement.ChildMemberElements.Count - 1; i >= 0; i--)
                    {
                        if (_memberElement.ChildMemberElements[i].Name == member.Name)
                            _memberElement.ChildMemberElements.RemoveAt(i);
                    }
                    break;
                }
                ClearSpecificMemberElements(_memberElement.ChildMemberElements, member);
            }
        }

        /// <summary>
        /// Toggles expandable state of specified cell.
        /// </summary>
        /// <param name="cellDescriptor">Cell to toggle expandable state for.</param>
        public void ToggleExpandableState(PivotCellDescriptor cellDescriptor)
        {
            Member member = cellDescriptor.Tag as Member;
            this.ToggleExpandableState(cellDescriptor.CellType, member);
        }

        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        public void ToggleExpandableState(PivotCellDescriptorType cellType, Member member)
        {
            if (this.ActiveReport != null && this.UseSharedDataManager)
            {
                this.ToggleExpandableStateForActiveReport(cellType, member, false);
            }
            else
                this.ToggleExpandableState(cellType, member, true);
        }

        public void ToggleExpandableStateForActiveReport(PivotCellDescriptorType cellType, Member member, bool triggerEvents)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (member != null)
            {
                Items items = null;
                _isUpdatedCellSet = false;
                if (this.ActiveReport != null)
                {
                    if (this.ActiveReport.TogglePivot)
                    {
                        if (cellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            cellType = PivotCellDescriptorType.RowHeader;
                        }
                        else if (cellType == PivotCellDescriptorType.RowHeader)
                        {
                            cellType = PivotCellDescriptorType.ColumnHeader;
                        }
                    }
                }

                if (cellType == PivotCellDescriptorType.ColumnHeader)
                {
                    items = this.ActiveReport.CategoricalElements;
                }
                else if (cellType == PivotCellDescriptorType.RowHeader)
                {
                    items = this.ActiveReport.SeriesElements;
                }

                if (triggerEvents)
                {
                    if (this.ActiveReport.EngineVersion == QueryBuilderEngineVersions.None ||
                        this.ActiveReport.EngineVersion == QueryBuilderEngineVersions.Version3)
                    {
                        bool isDrillDown = QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
#if !SILVERLIGHT
#if DEBUG
                        sw.Start();
#endif
#endif
                        if (!_isUpdatedCellSet || isDrillDown)
                        {
                            if (items.Count > 0)
                            {
                                this.NotifyElementModified(items[0].Axis);
                                //this.NotifyDrillDown(items[0].Axis);
                            }
                            else
                            {
                                this.NotifyElementModified();
                                //this.NotifyDrillDown();
                            }
                            ////this.ExecuteOlapTable();
#if !SILVERLIGHT
#if DEBUG
                            sw.Stop();
                            Console.WriteLine("Time taken form QueryBuilderEngineHelper.UpdateDrillDownItems -- Data Rendering {0}", sw.Elapsed);
#endif
#endif
                        }
                    }
                    else
                    {
                        QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);

                        if (!_isUpdatedCellSet)
                        {
                            this.ExecuteOlapTable();
                        }
                    }
                }
                else
                {
                    QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
                }
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine("Time taken form OlapDataManager.ToggleExpandableState {0}", sw.Elapsed);
#endif
        }

        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        public void ToggleExpandableState(PivotCellDescriptorType cellType, Member member, bool triggerEvents)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (member != null)
            {
                Items items = null;
                _isUpdatedCellSet = false;
                if (this.CurrentReport != null)
                {
                    if (this.CurrentReport.TogglePivot)
                    {
                        if (cellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            cellType = PivotCellDescriptorType.RowHeader;
                        }
                        else if (cellType == PivotCellDescriptorType.RowHeader)
                        {
                            cellType = PivotCellDescriptorType.ColumnHeader;
                        }
                    }
                }

                if (cellType == PivotCellDescriptorType.ColumnHeader)
                {
                    items = this.CurrentReport.CategoricalElements;
                }
                else if (cellType == PivotCellDescriptorType.RowHeader)
                {
                    items = this.CurrentReport.SeriesElements;
                }

                if (triggerEvents)
                {
                    if (this.CurrentReport.EngineVersion == QueryBuilderEngineVersions.None ||
                        this.CurrentReport.EngineVersion == QueryBuilderEngineVersions.Version3)
                    {
                        bool isDrillDown = QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
#if !SILVERLIGHT
#if DEBUG
                        sw.Start();
#endif
#endif
                        if (!_isUpdatedCellSet || isDrillDown)
                        {
                            if (items.Count > 0)
                            {
                                this.NotifyElementModified(items[0].Axis);
                                //this.NotifyDrillDown(items[0].Axis);
                            }
                            else
                            {
                                this.NotifyElementModified();
                                //this.NotifyDrillDown();
                            }
                            ////this.ExecuteOlapTable();
#if !SILVERLIGHT
#if DEBUG
                            sw.Stop();
                            Console.WriteLine("Time taken form QueryBuilderEngineHelper.UpdateDrillDownItems -- Data Rendering {0}", sw.Elapsed);
#endif
#endif
                        }
                    }
                    else
                    {
                        QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);

                        if (!_isUpdatedCellSet)
                        {
                            this.ExecuteOlapTable();
                        }
                    }
                }
                else
                {
                    QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
                }
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine("Time taken form OlapDataManager.ToggleExpandableState {0}", sw.Elapsed);
#endif
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Called when [axis element changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Olap.Manager.AxisElementChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            if (this.ItemSource == null)
            {
                this.ExecuteCellSet();
                _isUpdatedCellSet = true;
            }
            if (this.AxisElementChanged != null)
            {
                this.AxisElementChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [axis element modified].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Olap.Manager.AxisElementModifiedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAxisElementModified(object sender, AxisElementModifiedEventArgs e)
        {
            if (this.AxisElementModified != null)
            {
                this.AxisElementModified(sender, e);
            }
        }

        /// <summary>
        /// Called when [cube changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Olap.Manager.CubeChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCubeChanged(object sender, CubeChangedEventArgs e)
        {
            //this._currentReport = new OlapReport();
            //this._currentReport.Model = this;
            this._currentCubeName = e.CubeName;

            if (this.CurrentReport.CurrentCubeName.ToUpper() != e.CubeName.ToUpper())
            {
                this.CurrentReport.CategoricalElements.Clear();
                this.CurrentReport.SeriesElements.Clear();
                this.CurrentReport.SlicerElements.Clear();
            }

            this.CurrentReport.CurrentCubeName = e.CubeName;

            if (this.CubeChanged != null)
            {
                this.CubeChanged(sender, e);
            }

            this.NotifyElementModified();
        }

        //protected virtual void OnDrillDown(object sender, DrillDownEventArgs e)
        //{
        //    if (this.DrillDown != null)
        //    {
        //        this.DrillDown(sender, e);
        //    }
        //}

        /// <summary>
        /// Called when [report changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Olap.Manager.ReportChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnReportChanged(object sender, ReportChangedEventArgs e)
        {
            if (this.ReportChanged != null)
            {
                this.ReportChanged(sender, e);
            }
        }

        protected virtual void OnActiveReportChanged(object sender, ActiveReportChangedEventArgs e)
        {
            if (this.ActiveReportChanged != null)
                this.ActiveReportChanged(sender, e);
        }
        #endregion

        #region Private Methods
        bool _useWhereBackup =  true;
        private MDXQuerySpecification GetMDXQuerySpecification()
        {
            if (this._currentReport == null) return null;
            List<string> uniqueNames = new List<string>();
            bool useWhere = true;
            _useWhereBackup = this.UseWhereClauseForSlicing;//Getting the user set value

            MDXQuerySpecification query = new MDXQuerySpecification();
            query.CubeName = this._currentReport.CurrentCubeName;
            ////query.ShowGrandTotal = !this.CurrentReport.ShowGrandTotal;
            query.ShowEmptyRowData = this.CurrentReport.ShowEmptyRowData;
            query.ShowEmptyColumnData = this.CurrentReport.ShowEmptyColumnData;
            query.EngineVersion = this.CurrentReport.EngineVersion;
            if (this.CurrentReport.CategoricalElements.Count > 0)
            {
                foreach (Item item in this.CurrentReport.CategoricalElements)
                {
                    //// Reversing the Axis Posting
                    if (this.CurrentReport.TogglePivot)
                    {
                        item.Axis = AxisPosition.Series;
                    }
                    else
                    {
                        item.Axis = AxisPosition.Categorical;
                    }

                    query.Select.Items.Add(item);
                    if (item.ElementValue is DimensionElement)
                        uniqueNames.Add((item.ElementValue as DimensionElement).UniqueName);
                }

                foreach (Item item in this.CurrentReport.SeriesElements)
                {
                    if (this.CurrentReport.TogglePivot)
                    {
                        item.Axis = AxisPosition.Categorical;
                    }
                    else
                    {
                        item.Axis = AxisPosition.Series;
                    }

                    query.Select.Items.Add(item);
                    if (item.ElementValue is DimensionElement)
                        uniqueNames.Add((item.ElementValue as DimensionElement).UniqueName);
                }

                foreach (Item item in this.CurrentReport.SlicerElements)
                {
                    item.Axis = AxisPosition.Slicer;
                    query.Slicer.Items.Add(item);
                    if (item.ElementValue is DimensionElement)
                    {
                        if (uniqueNames.Any(i => i.ToUpper() == (item.ElementValue as DimensionElement).UniqueName.ToUpper()))
                        {
                            useWhere = false;
                        }
                    }
                }

                foreach (SlicerRangeFiltersInfo item in this.CurrentReport.SlicerRangeFilters)
                {
                    if (uniqueNames.Any(i => i.ToUpper() == item.StartValue.Split('.')[0].ToUpper()))
                    {
                        useWhere = false;
                    }
                }

                if (this.CurrentReport.FilterElements.Count > 0)
                {
                    if (this.CurrentReport.TogglePivot)
                    {
                        Items clonedFilterElements = this.CurrentReport.FilterElements.Clone();
                        ToggleElementAxis(clonedFilterElements);
                        foreach (Item item in clonedFilterElements)
                        {
                            query.Filter.Items.Add(item);
                        }
                    }
                    else
                    {
                        foreach (Item item in this.CurrentReport.FilterElements)
                        {
                            query.Filter.Items.Add(item);
                        }
                    }
                }

                foreach (Item item in this.CurrentReport.CalculatedMembers)
                {
                    query.With.Items.Add(item);
                }

                foreach (var item in this.CurrentReport.VirtualKpiElements)
                {
                    query.With.Items.Add(item);
                }

                ////handled for adding subsetElement into Items
                if (this.CurrentReport.CategoricalElements.SubSetElement != null)
                {
                    query.Select.Items.Add(new Item { ElementValue = this.CurrentReport.CategoricalElements.SubSetElement, Axis = AxisPosition.Categorical });
                }

                if (this.CurrentReport.SeriesElements.SubSetElement != null)
                {
                    query.Select.Items.Add(new Item { ElementValue = this.CurrentReport.SeriesElements.SubSetElement, Axis = AxisPosition.Series });
                }
                query.IsPagingEnabled = this.CurrentReport.EnablePaging;

                if (query.IsPagingEnabled)
                {
                    query.Page.CategorialCurrentPage = this.CurrentReport.PagerOptions.CategorialCurrentPage;
                    query.Page.CategorialPageSize = this.CurrentReport.PagerOptions.CategorialPageSize;
                    query.Page.SeriesCurrentPage = this.CurrentReport.PagerOptions.SeriesCurrentPage;
                    query.Page.SeriesPageSize = this.CurrentReport.PagerOptions.SeriesPageSize;
                }
                this.UseWhereClauseForSlicing = useWhere;
                this.CurrentReport.UseWhereClauseForSlicing = useWhere;
                return query;
            }

            return null;
        }

        private MDXQuerySpecification GetMDXQuerySpecForActiveReport()
        {
            if (this.ActiveReport == null) return null;

            MDXQuerySpecification query = new MDXQuerySpecification();
            query.CubeName = this.ActiveReport.CurrentCubeName;
            ////query.ShowGrandTotal = !this.ActiveReport.ShowGrandTotal;
            query.ShowEmptyRowData = this.ActiveReport.ShowEmptyRowData;
            query.ShowEmptyColumnData = this.ActiveReport.ShowEmptyColumnData;
            query.EngineVersion = this.ActiveReport.EngineVersion;
            if (this.ActiveReport.CategoricalElements.Count > 0)
            {
                foreach (Item item in this.ActiveReport.CategoricalElements)
                {
                    //// Reversing the Axis Posting
                    if (this.ActiveReport.TogglePivot)
                    {
                        item.Axis = AxisPosition.Series;
                    }
                    else
                    {
                        item.Axis = AxisPosition.Categorical;
                    }

                    query.Select.Items.Add(item);
                }

                foreach (Item item in this.ActiveReport.SeriesElements)
                {
                    if (this.ActiveReport.TogglePivot)
                    {
                        item.Axis = AxisPosition.Categorical;
                    }
                    else
                    {
                        item.Axis = AxisPosition.Series;
                    }

                    query.Select.Items.Add(item);
                }

                foreach (Item item in this.ActiveReport.SlicerElements)
                {
                    item.Axis = AxisPosition.Slicer;
                    query.Slicer.Items.Add(item);
                }

                if (this.ActiveReport.FilterElements.Count > 0)
                {
                    if (this.ActiveReport.TogglePivot)
                    {
                        Items clonedFilterElements = this.ActiveReport.FilterElements.Clone();
                        ToggleElementAxis(clonedFilterElements);
                        foreach (Item item in clonedFilterElements)
                        {
                            query.Filter.Items.Add(item);
                        }
                    }
                    else
                    {
                        foreach (Item item in this.ActiveReport.FilterElements)
                        {
                            query.Filter.Items.Add(item);
                        }
                    }
                }

                foreach (Item item in this.ActiveReport.CalculatedMembers)
                {
                    query.With.Items.Add(item);
                }

                foreach (var item in this.ActiveReport.VirtualKpiElements)
                {
                    query.With.Items.Add(item);
                }

                ////handled for adding subsetElement into Items
                if (this.ActiveReport.CategoricalElements.SubSetElement != null)
                {
                    query.Select.Items.Add(new Item { ElementValue = this.ActiveReport.CategoricalElements.SubSetElement, Axis = AxisPosition.Categorical });
                }

                if (this.ActiveReport.SeriesElements.SubSetElement != null)
                {
                    query.Select.Items.Add(new Item { ElementValue = this.ActiveReport.SeriesElements.SubSetElement, Axis = AxisPosition.Series });
                }
                query.IsPagingEnabled = this.ActiveReport.EnablePaging;

                if (query.IsPagingEnabled)
                {
                    query.Page.CategorialCurrentPage = this.ActiveReport.PagerOptions.CategorialCurrentPage;
                    query.Page.CategorialPageSize = this.ActiveReport.PagerOptions.CategorialPageSize;
                    query.Page.SeriesCurrentPage = this.ActiveReport.PagerOptions.SeriesCurrentPage;
                    query.Page.SeriesPageSize = this.ActiveReport.PagerOptions.SeriesPageSize;
                }
                return query;
            }

            return null;
        }

        private void ToggleElementAxis(Items items)
        {
            foreach (var item in items)
            {
                if (item.Axis == AxisPosition.Categorical)
                {
                    item.Axis = AxisPosition.Series;
                }
                else if (item.Axis == AxisPosition.Series)
                {
                    item.Axis = AxisPosition.Categorical;
                }
            }
        }

        private void InitializeOlapDataManager(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.IsCurrentReportModified = false;
            this.Reports = new OlapReportCollection();
            this.Properties = new PropertyCollection();
            if (this.ConnectionString != null && this.ConnectionString != string.Empty)
            {
                this.DataProvider = new AdomdDataProvider(this.ConnectionString);
            }
        }

        private void InitializeOlapDataManager(IDataProvider adomdDataProvider)
        {
            this.IsCurrentReportModified = false;
            this.Reports = new OlapReportCollection();
            this.Properties = new PropertyCollection();
            this.DataProvider = adomdDataProvider;
        }

        private void NotifyCubeNameChanged(string cubeName, bool triggerCubeChangeEvent)
        {
            if (_currentCubeName != cubeName)
            {
                _currentCubeName = cubeName;
                this.CurrentCubeSchema = null;
                //// If this passed the condition then it will trigger the CubeDimension browser to update the 
                //// cube meta structure
                if (triggerCubeChangeEvent)
                {
                    this.OnCubeChanged(this, new CubeChangedEventArgs(cubeName));
                }
            }
        }

        private void UpdateVirtualKPIStatus(AxisPosition axisPostion)
        {
            Axis axis = null;
            if (axisPostion == AxisPosition.Categorical)
            {
                axis = m_cellSet.Axes[0];
            }
            else if (axisPostion == AxisPosition.Series)
            {
                axis = m_cellSet.Axes[1];
            }

            for (int i = 0; i < axis.TupleSet.Count; i++)
            {
                Syncfusion.Olap.Data.Tuple tupleSet = axis.TupleSet[i];
                for (int j = 0; j < tupleSet.Members.Count; j++)
                {
                    Member kpiMember = tupleSet.Members[j];
                    if (kpiMember.Type == MemberTypeEnum.Formula && kpiMember.KPIType == KpiTypeEnum.Kpi_None)
                    {
                        MDXQuerySpecification mdxQuerySpec = (this.UseSharedDataManager && this.ActiveReport != null) ? GetMDXQuerySpecForActiveReport() : GetMDXQuerySpecification();
                        if (kpiMember.Caption.Trim().EndsWith("Value"))
                        {
                            kpiMember.KPIType = KpiTypeEnum.Kpi_Value;
                            VirtualKpiElement virtualKpiElement = mdxQuerySpec.Select.Items.List.Where(mem => mem.ElementValue is VirtualKpiElement && (mem.ElementValue as VirtualKpiElement).ValueUniqueName == kpiMember.UniqueName).FirstOrDefault().ElementValue as VirtualKpiElement;
                            if (virtualKpiElement != null)
                            {
                                kpiMember.Properties.Add(new Property(PropertyConstants.VirtualKpiHeaderName, virtualKpiElement.Name));
                            }
                        }
                        else if (kpiMember.Caption.Trim().EndsWith("Goal"))
                        {
                            kpiMember.KPIType = KpiTypeEnum.Kpi_Goal;
                            VirtualKpiElement virtualKpiElement = mdxQuerySpec.Select.Items.List.Where(mem => mem.ElementValue is VirtualKpiElement && (mem.ElementValue as VirtualKpiElement).GoalUniqueName == kpiMember.UniqueName).FirstOrDefault().ElementValue as VirtualKpiElement;
                            if (virtualKpiElement != null)
                            {
                                kpiMember.Properties.Add(new Property(PropertyConstants.VirtualKpiHeaderName, virtualKpiElement.Name));
                            }
                        }
                        else if (kpiMember.Caption.Trim().EndsWith("Trend"))
                        {
                            kpiMember.KPIType = KpiTypeEnum.Kpi_Trend;
                            VirtualKpiElement virtualKpiElement = mdxQuerySpec.Select.Items.List.Where(mem => mem.ElementValue is VirtualKpiElement && (mem.ElementValue as VirtualKpiElement).TrendUniqueName == kpiMember.UniqueName).FirstOrDefault().ElementValue as VirtualKpiElement;
                            if (virtualKpiElement != null)
                            {
                                kpiMember.KPITrendGraphic = virtualKpiElement.TrendGraphic;
                                kpiMember.Properties.Add(new Property(PropertyConstants.VirtualKpiHeaderName, virtualKpiElement.Name));
                            }
                        }
                        else if (kpiMember.Caption.Trim().EndsWith("Status"))
                        {
                            kpiMember.KPIType = KpiTypeEnum.Kpi_Status;
                            VirtualKpiElement virtualKpiElement = mdxQuerySpec.Select.Items.List.Where(mem => mem.ElementValue is VirtualKpiElement && (mem.ElementValue as VirtualKpiElement).StatusUniqueName == kpiMember.UniqueName).FirstOrDefault().ElementValue as VirtualKpiElement;
                            if (virtualKpiElement != null)
                            {
                                kpiMember.KPIStatusGraphic = virtualKpiElement.StatusGraphic;
                                kpiMember.Properties.Add(new Property(PropertyConstants.VirtualKpiHeaderName, virtualKpiElement.Name));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Member with KPI Properties if the Member is a type of KPI
        /// </summary>
        /// <param name="axisPosition">Axis Position of KPI</param>
        private void UpdateKPIStatus(AxisPosition axisPosition)
        {
            Axis axis;
            if (axisPosition == AxisPosition.Categorical)
            {
                axis = m_cellSet.Axes[0];
            }
            else if (axisPosition == AxisPosition.Series)
            {
                axis = m_cellSet.Axes[1];
            }
            else
            {
                axis = null;
            }

            KpiCollection kpis = this.CurrentCubeSchema.Kpis;
            MeasureCollection measures = this.CurrentCubeSchema.Measures;
            for (int i = 0; i < axis.TupleSet.Count; i++)
            {
                Syncfusion.Olap.Data.Tuple tupleSet = axis.TupleSet[i];
                for (int j = 0; j < tupleSet.Members.Count; j++)
                {
                    Member kpiMember = tupleSet.Members[j];
                    if (kpiMember.Type == MemberTypeEnum.Measure || kpiMember.Type == MemberTypeEnum.Formula)
                    {
                        foreach (Kpi kpiObj in kpis)
                        {
                            if (kpiObj.Properties.Count > 0)
                            {
                                Property propertyKPIObj = kpiObj.Properties.FindByName(PropertyConstants.KPI);
                                if (propertyKPIObj != null)
                                {
                                    Microsoft.AnalysisServices.AdomdClient.Kpi admomdKPIObj = propertyKPIObj.Value as Microsoft.AnalysisServices.AdomdClient.Kpi;
                                    if (admomdKPIObj.Properties[PropertyConstants.KPI_VALUE].Value.ToString() == kpiMember.UniqueName)
                                    {
                                        kpiMember.KPIType = KpiTypeEnum.Kpi_Value;
                                        string kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_GOAL].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_GOAL, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_STATUS].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_STATUS, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_TREND].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_TREND, kpiObjectValue));
                                        }

                                        kpiMember.Properties.Add(new Property(PropertyConstants.KPI, propertyKPIObj));
                                    }
                                    else if (admomdKPIObj.Properties[PropertyConstants.KPI_GOAL].Value.ToString() == kpiMember.UniqueName)
                                    {
                                        kpiMember.KPIType = KpiTypeEnum.Kpi_Goal;
                                        string kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_VALUE].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_VALUE, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_STATUS].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_STATUS, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_TREND].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_TREND, kpiObjectValue));
                                        }

                                        kpiMember.Properties.Add(new Property(PropertyConstants.KPI, propertyKPIObj));
                                    }
                                    else if (admomdKPIObj.Properties[PropertyConstants.KPI_STATUS].Value.ToString() == kpiMember.UniqueName)
                                    {
                                        kpiMember.KPIType = KpiTypeEnum.Kpi_Status;
                                        kpiMember.KPIStatusGraphic = kpiObj.StatusGraphic;
                                        string kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_GOAL].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_GOAL, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_VALUE].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_VALUE, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_TREND].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_TREND, kpiObjectValue));
                                        }

                                        kpiMember.Properties.Add(new Property(PropertyConstants.KPI, propertyKPIObj));
                                    }
                                    else if (admomdKPIObj.Properties[PropertyConstants.KPI_TREND].Value.ToString() == kpiMember.UniqueName)
                                    {
                                        kpiMember.KPIType = KpiTypeEnum.Kpi_Trend;
                                        kpiMember.KPITrendGraphic = kpiObj.TrendGraphic;
                                        string kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_GOAL].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_GOAL, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_STATUS].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_STATUS, kpiObjectValue));
                                        }

                                        kpiObjectValue = admomdKPIObj.Properties[PropertyConstants.KPI_VALUE].Value as string;
                                        if (kpiObjectValue != null)
                                        {
                                            kpiMember.Properties.Add(new Property(PropertyConstants.KPI_VALUE, kpiObjectValue));
                                        }

                                        kpiMember.Properties.Add(new Property(PropertyConstants.KPI, propertyKPIObj));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="gridLayout">The grid layout.</param>
        public void ToggleExpandableState(PivotCellDescriptor cellDescriptor, GridLayout gridLayout)
        {
            if (this.ItemSource == null)
            {
                Member member = cellDescriptor.Tag as Member;
                this.ToggleExpandableState(cellDescriptor.CellType, member, gridLayout);
            }
            else
            {
                CustomSourceReportHelper.ProcessReport(cellDescriptor, gridLayout, this.CurrentReport);
                PivotElements pivotDataElements = new PivotElements(this.CurrentReport);

                if (this.ItemSource is IEnumerable)
                {
                    IEnumerable source = this.ItemSource as IEnumerable;
                    //if (source != null)
                    {
                        this.m_pivotEngine = TableBuilder.BuildEngineFromIQueryable(source.AsQueryable(), pivotDataElements.Report, this.RelationalDataSortOrder,
                            pivotDataElements.ColumnItems.ToArray(),
                            pivotDataElements.SeriesItems.ToArray(),
                            pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, gridLayout, pivotDataElements.ExpandAll, cellDescriptor, true);
                    }
                }
                else if (this.ItemSource is IListSource)
                {
                    IListSource source = this.ItemSource as IListSource;
                    this.m_pivotEngine = TableBuilder.BuildEngineFromIListSource(source, pivotDataElements.Report, this.RelationalDataSortOrder,
                        pivotDataElements.ColumnItems.ToArray(),
                        pivotDataElements.SeriesItems.ToArray(),
                        pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, gridLayout, pivotDataElements.ExpandAll, pivotDataElements.SummaryStringCount, cellDescriptor, true);
                }

                this.NotifyElementModified();
            }
        }

        /// <summary>
        /// Toggles the state of the member.
        /// </summary>
        /// <param name="pivotCellDescriptorType">Type of the pivot cell descriptor.</param>
        /// <param name="member">The member.</param>
        /// <param name="gridLayout">The grid layout.</param>
        public void ToggleExpandableState(PivotCellDescriptorType pivotCellDescriptorType, Member member, GridLayout gridLayout)
        {
            if (this.ActiveReport != null && this.UseSharedDataManager)
            {
                this.ToggleExpandableStateForActiveReport(pivotCellDescriptorType, member, gridLayout, true);
            }
            else
                this.ToggleExpandableState(pivotCellDescriptorType, member, gridLayout, true);
        }

        public void ToggleExpandableStateForActiveReport(PivotCellDescriptorType cellType, Member member, GridLayout gridLayout, bool triggerEvents)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (member != null)
            {
                Items items = null;
                _isUpdatedCellSet = false;
                if (this.ActiveReport != null)
                {
                    if (this.ActiveReport.TogglePivot)
                    {
                        if (cellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            cellType = PivotCellDescriptorType.RowHeader;
                        }
                        else if (cellType == PivotCellDescriptorType.RowHeader)
                        {
                            cellType = PivotCellDescriptorType.ColumnHeader;
                        }
                    }
                }

                if (cellType == PivotCellDescriptorType.ColumnHeader)
                {
                    items = this.ActiveReport.CategoricalElements;
                }
                else if (cellType == PivotCellDescriptorType.RowHeader)
                {
                    items = this.ActiveReport.SeriesElements;
                }

                if (triggerEvents)
                {
                    if (this.ActiveReport.EngineVersion == QueryBuilderEngineVersions.None ||
                        this.ActiveReport.EngineVersion == QueryBuilderEngineVersions.Version3)
                    {
                        bool isDrillDown = QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);

                        if (!_isUpdatedCellSet || isDrillDown)
                        {
                            if (this.ActiveReport == null)
                            {
                                if (items.Count > 0)
                                {
                                    this.NotifyElementModified(items[0].Axis);
                                    //this.NotifyDrillDown(items[0].Axis);
                                }
                                else
                                {
                                    this.NotifyElementModified();
                                    //this.NotifyDrillDown();
                                }
                            }
                            ////this.ExecuteOlapTable();
                        }
                    }
                    else
                    {
                        QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);

                        if (!_isUpdatedCellSet)
                        {
                            this.ExecuteOlapTable();
                        }
                    }
                }
                else
                {
                    QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
                }
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine("Time taken form OlapDataManager.ToggleExpandableState {0}", sw.Elapsed);
#endif
        }

        /// <summary>
        /// Toggles the state of the Member.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        public void ToggleExpandableState(PivotCellDescriptorType cellType, Member member, GridLayout gridLayout, bool triggerEvents)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (member != null)
            {
                Items items = null;
                _isUpdatedCellSet = false;
                if (this.CurrentReport != null)
                {
                    if (this.CurrentReport.TogglePivot)
                    {
                        if (cellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            cellType = PivotCellDescriptorType.RowHeader;
                        }
                        else if (cellType == PivotCellDescriptorType.RowHeader)
                        {
                            cellType = PivotCellDescriptorType.ColumnHeader;
                        }
                    }
                }

                if (cellType == PivotCellDescriptorType.ColumnHeader)
                {
                    items = this.CurrentReport.CategoricalElements;
                }
                else if (cellType == PivotCellDescriptorType.RowHeader)
                {
                    items = this.CurrentReport.SeriesElements;
                }

                if (triggerEvents)
                {
                    if (this.CurrentReport.EngineVersion == QueryBuilderEngineVersions.None ||
                        this.CurrentReport.EngineVersion == QueryBuilderEngineVersions.Version3)
                    {
                        bool isDrillDown = QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);

                        if (!_isUpdatedCellSet || isDrillDown)
                        {
                            if (items.Count > 0)
                            {
                                this.NotifyElementModified(items[0].Axis);
                                //this.NotifyDrillDown(items[0].Axis);
                            }
                            else
                            {
                                this.NotifyElementModified();
                                //this.NotifyDrillDown();
                            }
                            ////this.ExecuteOlapTable();
                        }
                    }
                    else
                    {
                        QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);

                        if (!_isUpdatedCellSet)
                        {
                            this.ExecuteOlapTable();
                        }
                    }
                }
                else
                {
                    QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
                }
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine("Time taken form OlapDataManager.ToggleExpandableState {0}", sw.Elapsed);
#endif
        }

        /// <summary>
        /// Gets the member properties for HeaderToolTip's.
        /// </summary>
        /// <param name="expandCell">The expanded cell.</param>
        /// <returns></returns>
        public PivotEngine GetMemberProperties(PivotCellDescriptor expandCell)
        {
            PivotEngine expandTable = TableBuilder.BuildEngineWithMemberProperties(expandCell);
            return expandTable;
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        public void ToggleExpandableStateOnDrillPosition(PivotCellDescriptor cellDescriptor)
        {
            if (this.CurrentReport == null || cellDescriptor.Tag == null) return;
            ToggleExpandableStateOnDrillPosition(cellDescriptor, cellDescriptor.Tag as Member);
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="member">The member.</param>
        public void ToggleExpandableStateOnDrillPosition(PivotCellDescriptor cellDescriptor, Member member)
        {
            this.ToggleExpandableStateOnDrillPosition(member, cellDescriptor.CellType, GetPositionsInfo(cellDescriptor), cellDescriptor.ExpandableState, true);
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="type">The type.</param>
        /// <param name="positionInfo">The position info.</param>
        /// <param name="state">The state.</param>
        public void ToggleExpandableStateOnDrillPosition(Member member, PivotCellDescriptorType type, List<PositionInfo> positionInfo, ExpandableState state)
        {
            this.ToggleExpandableStateOnDrillPosition(member, type, positionInfo, state, true);
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="type">The type.</param>
        /// <param name="positionInfo">The position info.</param>
        /// <param name="state">The state.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        public void ToggleExpandableStateOnDrillPosition(Member member, PivotCellDescriptorType type, List<PositionInfo> positionInfo, ExpandableState state, bool triggerEvents)
        {
            if (member == null || this.CurrentReport == null
                || !type.In(PivotCellDescriptorType.ColumnHeader, PivotCellDescriptorType.RowHeader))
                return;

            if (this.CurrentReport.TogglePivot)
                type = type == PivotCellDescriptorType.ColumnHeader ? PivotCellDescriptorType.RowHeader : PivotCellDescriptorType.ColumnHeader;

            var drillInfo = HeaderPositionsInfo.GetHierarchyBasedPositions(positionInfo);
            var drilledCells = this.CurrentReport.DrilledCells[type.ToString()];

            if (state == ExpandableState.Collapsed)
            {
                if (drilledCells == null)
                    drilledCells = this.CurrentReport.DrilledCells[type.ToString()] = new HeaderPositionsInfo();
                drilledCells.Add(drillInfo);
            }
            else if (drilledCells != null)
            {
                var hierarchy = member.ParentHierarchy;
                for (int i = 0; i < drilledCells.Count; i++)
                {
                    if (TestEquality(drilledCells[i], drillInfo, hierarchy))
                    {
                        drilledCells.RemoveAt(i--);
                    }
                }
            }

            if (triggerEvents)
            {
                var items = type == PivotCellDescriptorType.ColumnHeader ? this.CurrentReport.CategoricalElements : this.CurrentReport.SeriesElements;
                if (items.Count > 0)
                    this.NotifyElementModified(items[0].Axis);
                else
                    this.NotifyElementModified();
            }
        }

        bool TestEquality(SerializableDictionary<string, List<PositionInfo>> dCell, SerializableDictionary<string, List<PositionInfo>> currentCell, string hierarchy)
        {
            return currentCell.All(cc => dCell.Keys.Contains(cc.Key)
                && cc.Value.All(cv => dCell.Values.Any(dc =>
                    dc.Any(dv => dv.HierarchyUniqueName == cv.HierarchyUniqueName && dv.UniqueName == cv.UniqueName))));
        }

        /// <summary>
        /// Gets the positions info.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <returns></returns>
        public List<PositionInfo> GetPositionsInfo(PivotCellDescriptor cellDescriptor)
        {
            List<PositionInfo> drillInfo = null;
            var locationInfo = PivotEngine.GetCellLocation(cellDescriptor);
            int row = locationInfo.Top, column = locationInfo.Left;

            if (row >= 0 && column >= 0 &&
                this.PivotEngine.TableColumns.Count > column && this.PivotEngine.RowsCount > row)
            {
                drillInfo = new List<PositionInfo>();

                if (cellDescriptor.CellType.In(PivotCellDescriptorType.RowHeader, PivotCellDescriptorType.SummaryRow))
                    UpdateDrillInfo(drillInfo, column, PivotEngine.GetRowAt(row).Cells);
                else
                    UpdateDrillInfo(drillInfo, row, PivotEngine.TableColumns[column].Cells);
            }

            return drillInfo;
        }
        void UpdateDrillInfo(List<PositionInfo> drillInfo, int position, PivotCellCollection cells)
        {
            Member member = null;

            for (int i = 0; i <= position; i++)
            {
                member = cells[i].Tag as Member ?? (cells[i].SpanCell != null ? cells[i].SpanCell.Tag as Member : null);

                if (cells[i].CellType.In(cells[position].CellType, PivotCellDescriptorType.SummaryRow, PivotCellDescriptorType.SummaryColumn) && member != null)
                {
                    drillInfo.Add(new PositionInfo
                    {
                        UniqueName = string.IsNullOrEmpty(cells[i].UniqueName) ? member.UniqueName : cells[i].UniqueName,
                        HierarchyUniqueName = string.IsNullOrEmpty(member.ParentHierarchy) ? string.Format("Empty{0}", i) : member.ParentHierarchy
                    });
                }
                else if (cells[i].CellType == PivotCellDescriptorType.Value) break;
            }
        }

    }

    #region Event Arguments

    /// <summary>
    /// Represents the arguments for AxisElementChangedEvent.
    /// </summary>
    public class AxisElementChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisElementChangedEventArgs"/> class.
        /// </summary>
        /// <param name="axisPosition">The axis position.</param>
        public AxisElementChangedEventArgs(AxisPosition axisPosition)
        {
            this.AxisPosition = axisPosition;
            this.IsValidAxisPosition = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisElementChangedEventArgs"/> class.
        /// </summary>
        public AxisElementChangedEventArgs()
        {
            this.IsValidAxisPosition = false;
        }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>The axis position.</value>
        public AxisPosition AxisPosition { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid axis position.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid axis position; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidAxisPosition { get; private set; }
    }

    /// <summary>
    /// Represents the arguments for AxisElementModifiedEvent.
    /// </summary>
    public class AxisElementModifiedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisElementModifiedEventArgs"/> class.
        /// </summary>
        public AxisElementModifiedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisElementModifiedEventArgs"/> class.
        /// </summary>
        /// <param name="axisPosition">The axis position.</param>
        public AxisElementModifiedEventArgs(AxisPosition axisPosition)
        {
            this.AxisPosition = axisPosition;
        }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>The axis position.</value>
        public AxisPosition AxisPosition { get; set; }
    }

    /// <summary>
    /// Represents the arguments for DrillDownEvent.
    /// </summary>
    public class DrillDownEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownEventArgs"/> class.
        /// </summary>
        /// <param name="axisPosition">The axis position.</param>
        public DrillDownEventArgs(AxisPosition axisPosition)
        {
            this.AxisPosition = axisPosition;
            this.IsValidAxisPosition = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownEventArgs"/> class.
        /// </summary>
        public DrillDownEventArgs()
        {
            this.IsValidAxisPosition = false;
        }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>The axis position.</value>
        public AxisPosition AxisPosition { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid axis position.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid axis position; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidAxisPosition { get; set; }
    }

    /// <summary>
    /// Represents the arguments for ReportChangedEvent.
    /// </summary>
    public class ReportChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportChangedEventArgs"/> class.
        /// </summary>
        public ReportChangedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newReport">The new report.</param>
        public ReportChangedEventArgs(OlapReport newReport)
        {
            this.NewReport = newReport;
        }

        /// <summary>
        /// Gets or sets the new report.
        /// </summary>
        /// <value>The new report.</value>
        public OlapReport NewReport { get; set; }
    }

    public class ActiveReportChangedEventArgs : System.EventArgs
    {
        public ActiveReportChangedEventArgs()
        {

        }
        public ActiveReportChangedEventArgs(OlapReport newActiveReport,bool isReportChanged)
        {
            this.NewActiveReport = newActiveReport;
            this.IsReportChanged = isReportChanged;
        }

        public OlapReport NewActiveReport { get; set; }

        public bool IsReportChanged { get; set; }
    }

    public class CalculatedMemberAddedEventArgs : System.EventArgs
    {
        public CalculatedMemberAddedEventArgs()
        {

        }
    }

    /// <summary>
    /// Represents the arguments for QueryExecutingEvent.
    /// </summary>
    public class QueryExecutingEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExecutingEventArgs"/> class.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        public QueryExecutingEventArgs(string mdxQuery) : this(mdxQuery, "") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExecutingEventArgs"/> class.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <param name="executingFunction">The executing function.</param>
        public QueryExecutingEventArgs(string mdxQuery, string executingFunction) : this(mdxQuery, executingFunction, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExecutingEventArgs"/> class.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <param name="executingFunction">The executing function.</param>
        /// <param name="report">The report.</param>
        public QueryExecutingEventArgs(string mdxQuery, string executingFunction, OlapReport report)
        {
            this.MdxQuery = mdxQuery;
            ExecutingFunction = executingFunction;
            Report = report;
        }

        /// <summary>
        /// Gets or sets the MDX query.
        /// </summary>
        /// <value>The MDX query.</value>
        public string MdxQuery { get; set; }

        /// <summary>
        /// Gets or sets the executing function.
        /// </summary>
        /// <value>The executing function.</value>
        public string ExecutingFunction { get; set; }

        /// <summary>
        /// Gets or sets the report.
        /// </summary>
        /// <value>The report.</value>
        public OlapReport Report { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="QueryExecutingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Represents the arguments for CubeChangedEvent.
    /// </summary>
    public class CubeChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        public CubeChangedEventArgs(string cubeName)
        {
            this.CubeName = cubeName;
        }

        /// <summary>
        /// Gets or sets the name of the cube.
        /// </summary>
        /// <value>The name of the cube.</value>
        public string CubeName { get; set; }
    }

    #endregion

    #region Delegates

    /// <summary>
    /// CubeChangedEvent will be triggered.
    /// </summary>
    /// <param name="sender">Source object.</param>
    /// <param name="e">Argument for CubeChangedEventHandler</param>
    public delegate void CubeChangedEventHandler(object sender, CubeChangedEventArgs e);
    /// <summary>
    /// AxisElementChangedEvent will be triggered only if AutoExecute set to false and this will
    /// refresh only axis element not the controls
    /// </summary>
    /// <param name="sender">Source object.</param>
    /// <param name="e">Argument for AxisElementChangedEventHandler</param>
    public delegate void AxisElementChangedEventHandler(object sender, AxisElementChangedEventArgs e);
    /// <summary>
    /// AxisElememtModifiedEent will be triggered only if AutoExecute set to false and this will
    /// refresh only axis element not the controls
    /// </summary>
    /// <param name="sender">Source object.</param>
    /// <param name="e">AxisElementModifiedEventHandler</param>
    public delegate void AxisElementModifiedEventHandler(object sender, AxisElementModifiedEventArgs e);
    /// <summary>
    /// DrillDownEvent will be triggered.
    /// </summary>
    /// <param name="sender">Source object.</param>
    /// <param name="e">An argument for DrillDownEventHandler</param>
    public delegate void DrillDownEventHandler(object sender, DrillDownEventArgs e);
    /// <summary>
    /// ReportChangedEvent will be triggered.
    /// </summary>
    /// <param name="sender">Source object.</param>
    /// <param name="e">An argument for ReportChangedEventHandler</param>
    public delegate void ReportChangedEventHandler(object sender, ReportChangedEventArgs e);

    public delegate void ActiveReportChangedEventHandler(object sender, ActiveReportChangedEventArgs e);


    public delegate void CalculatedMemberAddedEventHandler(object sender,CalculatedMemberAddedEventArgs e);
    /// <summary>
    /// QueryExecuteEvent will be triggered.
    /// </summary>
    /// <param name="sender">Source object.</param>
    /// <param name="e">An argument for QueryExecuteEventHandler</param>
    public delegate void QueryExecuteEventHandler(object sender, QueryExecutingEventArgs e);
    #endregion
}