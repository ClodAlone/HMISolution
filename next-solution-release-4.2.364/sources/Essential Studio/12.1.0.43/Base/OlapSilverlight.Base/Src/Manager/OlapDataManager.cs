#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Engine;
using System.Collections;
using Syncfusion.OlapSilverlight.Data;
using System.Linq;
using Syncfusion.OlapSilverlight.MDXQueryBuilder;
using Syncfusion.OlapSilverlight.Common;
using System.IO;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Manager
{

    /// <summary>
    /// Manages all data related operations and communicate the Client (UI controls) and Server (WCF Service).
    /// </summary>
    public class OlapDataManager : DependencyObject
    {
        #region Events

        /// <summary>
        /// Occurs when [cell set changed].
        /// </summary>
        public event CellSetChangedEventHandler CellSetChanged;
        /// <summary>
        /// Occurs when [cube info collection changed].
        /// </summary>
        public event CubeInfoCollectionChangedEventHandler CubeInfoCollectionChanged;
        /// <summary>
        /// Occurs when [cube schema changed].
        /// </summary>
        public event CubeSchemaChangedEventHandler CubeSchemaChanged;
        /// <summary>
        /// Occurs when [cube changed].
        /// </summary>
        public event CubeChangedEventHandler CubeChanged;
        /// <summary>
        /// Occurs when [report changed].
        /// </summary>
        public event ReportChangedEventHandler ReportChanged;
        /// <summary>
        /// Occurs when [axis element changed].
        /// </summary>
        public event AxisElementChangedEventHandler AxisElementChanged;
        /// <summary>
        /// Occurs when [level members obtained].
        /// </summary>
        public event LevelMembersObtainedHandler LevelMembersObtained;
        /// <summary>
        /// Occurs when [child members obtained].
        /// </summary>
        public event ChildMembersObtainedHandler ChildMembersObtained;
        /// <summary>
        /// Occurs when [cell set changing].
        /// </summary>
        public event CellSetChangingEventHandler CellSetChanging;
        /// <summary>
        /// Occurs when [MDX query obtained].
        /// </summary>
        public event MdxObtainedEventHandler MdxQueryObtained;

        #endregion
        
        #region Private Members

        /// <summary>
        /// Current OlapReport
        /// </summary>
        private OlapReport m_currentReport;

        /// <summary>
        /// Report Collection
        /// </summary>
        private OlapReportCollection m_reportList;

        /// <summary>
        /// PivotEngine
        /// </summary>
        private PivotEngine m_pivotEngine = null;

        /// <summary>
        /// ItemSource
        /// </summary>
        private object m_ItemSource;

        /// <summary>
        /// CellSet
        /// </summary>
        private CellSet m_cellSet = null;

        /// <summary>
        /// CubeSchema
        /// </summary>
        private CubeSchema m_cubeScheam = null;

        /// <summary>
        /// CubeName
        /// </summary>
        private string m_cubeName;

        /// <summary>
        /// PropertyCollection
        /// </summary>
        private PropertyCollection m_properties;

        /// <summary>
        /// AllowMDXToOlapReport
        /// </summary>
        private bool m_allowMdxToOlapReportParsing = true;

        /// <summary>
        /// Drillthrough result set
        /// </summary>
        private object m_resultSet;

        private ConnectionStringOptions _connectionStringOptions;

        private string _encryptedConnectionString;

        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapDataManager"/> class.
        /// </summary>
        public OlapDataManager()
        {
            this.ReportList = new OlapReportCollection();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// IOlapDataProvider
        /// </summary>
        public IOlapDataProvider DataProvider { get; set; }
        private Items _virtualKpiElements = new Items();

        /// <summary>
        /// Gets or sets the virtual KPI collection.
        /// </summary>
        [DataMember]
        public Items VirtualKpiElements
        {
            get { return _virtualKpiElements; }
            set { _virtualKpiElements = value; }
        }
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string options.
        /// </summary>
        /// <value>The connection string options.</value>
        public ConnectionStringOptions ConnectionStringOptions 
        {
            get
            {
                return _connectionStringOptions = _connectionStringOptions ?? new ConnectionStringOptions();
            }        
        }

        /// <summary>
        /// Gets the encrypted connection string.
        /// </summary>
        /// <value>The encrypted connection string.</value>
        public string EncryptedConnectionString
        {
            get
            {
                if (this.ConnectionStringOptions.EncryptConnectionString && !string.IsNullOrEmpty(this.ConnectionString))
                {
                    _encryptedConnectionString = Common.Common.Encrypt(this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName, this.ConnectionStringOptions.EncryptionKey);
                }
                return _encryptedConnectionString;
            }
            set
            {
                _encryptedConnectionString = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is non SSAS data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is non SSAS data; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This property is no longer exist.")]
        public bool IsNonSSASData { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public Providers ProviderName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show level type "All".
        /// </summary>
        public bool ShowLevelTypeAll { get; set; }


        /// <summary>
        /// Gets or sets the MDX query.
        /// </summary>
        /// <value>The MDX query.</value>
        public string MdxQuery { get; set; }

        /// <summary>
        /// Gets or sets ItemSource of OlapDataManager
        /// </summary>
        public object ItemSource
        {
            get
            {
                return m_ItemSource;
            }
            set
            {
                m_ItemSource = value;
            }
        }

        /// <summary>
        /// Gets or sets PivotEngine
        /// </summary>
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
        /// Gets or sets current OlapReport
        /// </summary>
        public OlapReport CurrentReport
        {
            get
            {
                if (m_currentReport == null)
                {
                    this.m_currentReport = new OlapReport();
                }

                return m_currentReport ;
            }
            set
            {
                m_currentReport = value ;
                if (m_currentReport == null)
                {
                    m_currentReport = new OlapReport();
                }
            }
        }

        /// <summary>
        /// Gets or sets the report collection.
        /// </summary>
        /// <value>The report list.</value>
        public OlapReportCollection ReportList
        {
            get
            {
                return m_reportList;
            }
            set
            {
                m_reportList = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is processing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is current report modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current report modified; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentReportModified { get; set; }

        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        public string CurrentCubeName 
        {
            get
            {
                return m_cubeName;
            }
            set
            {
                m_cubeName = value;
                NotifyCubeChanged();
            }


        }

        /// <summary>
        /// Gets or sets Current CellSet
        /// </summary>
        public CellSet CurrentCellSet
        {
            get
            {
                return m_cellSet;
            }

            set
            {
                if (m_cellSet != value)
                {
                    m_cellSet = value;
                }
            }
        }

        public SerializableDictionary<string,int> Counts { get; set; }

        /// <summary>
        /// Gets or sets the current cube schema.
        /// </summary>
        /// <value>The current cube schema.</value>
        public CubeSchema CurrentCubeSchema
        {
            get
            {
                return m_cubeScheam;
            }
            set
            {
                m_cubeScheam = value ;
                this.SetParentElements(m_cubeScheam);
            }
        }

        /// <summary>
        /// Gets or sets the drag drop manager.
        /// </summary>
        /// <value>The drag drop manager.</value>
        public DragDropManager DragDropManager { get; set; }

        /// <summary>
        /// Gets or sets the cubes.
        /// </summary>
        /// <value>The cubes.</value>
        public CubeInfoCollection Cubes { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public PropertyCollection Properties 
        {
            get
            {
                if (this.m_properties == null)
                {
                    this.m_properties = new PropertyCollection();
                }
                return this.m_properties;
            }
            set
            {
                this.m_properties = value;
            }
        }

        /// <summary>
        /// Gets or sets the result set.
        /// </summary>
        /// <value>The result set.</value>
        public object ResultSet
        {
            get;
            set;
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


        /// <summary>
        /// Gets or Sets the value indicating whether the MDX query to be parsed as OlapReport or not.
        /// </summary>
        /// <value>true</value>
        public bool AllowMdxToOlapReportParsing
        {
            get { return m_allowMdxToOlapReportParsing; }
            set { m_allowMdxToOlapReportParsing = value; }
        }

        

        #endregion

        #region Report operations
        /// <summary>
        /// Sets Current OlapReport with the given Report Object
        /// </summary>
        /// <param name="report"></param>
        public void SetCurrentReport(OlapReport report)
        {
            bool reportNameExist = false;
            foreach (OlapReport item in this.ReportList)
            {
                if (item.Name.Equals(report.Name))
                {
                    reportNameExist = true;
                    break;
                }
            }
            if (!reportNameExist)
            {
                this.ReportList.Add(report);
            }
            this.CurrentReport = report;
        }

        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="olapReport">The OlapReport.</param>
        public void AddReport(OlapReport olapReport)
        {
            this.ReportList.Add(olapReport);
            this.SetCurrentReport(olapReport);
        }

        /// <summary>
        /// Saves the current report list in the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void SaveReport(Stream filePath)
        {
            if (filePath != null && filePath != Stream.Null)
            {
                OlapReportCollection tempCollection = new OlapReportCollection();
                foreach (OlapReport report in this.ReportList)
                {
                    tempCollection.Add(report);
                }

                XmlParser.ToXml(tempCollection, filePath, false);
                this.IsCurrentReportModified = false;
            }
        }

        /// <summary>
        /// Load the reports from stream.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void LoadReportFromStream(Stream filePath)
        {
            try
            {
                this.ReportList.Clear();
                OlapReportCollection tempCollection = new OlapReportCollection();
                tempCollection = XmlParser.XmlToFromFile(filePath, typeof(OlapReportCollection)) as OlapReportCollection;
                this.ReportList = tempCollection;
                if (this.ReportList.Count > 0)
                    this.CurrentReport = this.ReportList[0];
                this.IsCurrentReportModified = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CellSet Creation

        /// <summary>
        /// Executes with the current report or MDX Query.
        /// </summary>
        public void ExecuteCellSet()
        {
            try
            {
                if (ItemSource == null)
                {
                    if (this.MdxQuery != null && this.MdxQuery != string.Empty)
                    {
                        if (AllowMdxToOlapReportParsing)
                        {
                            try
                            {
                                OlapReport olapReport = MDXQueryParser.MDXToOlapParser.GenerateOlapReport(this.MdxQuery);
                                this.SetCurrentReport(olapReport);
                                this.ExecuteCellSet(this.CurrentReport);
                            }
                            catch
                            {
                                this.ExecuteCellSet(this.MdxQuery);
                            }
                            this.MdxQuery = string.Empty;
                        }
                        else
                            this.ExecuteCellSet(this.MdxQuery);
                    }
                    else if (this.CurrentReport != null)
                    {
                        this.ExecuteCellSet(this.CurrentReport);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ExecuteCellSet(string MdxQuery)
        {
            this.IsProcessing = true;
            AsyncCallback callback = new AsyncCallback(this.EndExecuteMdxQuery);
            //this.DataProvider.BeginExecuteMdxQuery(MdxQuery, callback, this.DataProvider);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                this.DataProvider.BeginExecuteMdxQuery(MdxQuery, callback, this.DataProvider);
            }
            else
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                    this.DataProvider.BeginExecuteMdxQuery(MdxQuery, callback, this.DataProvider);
                }
            }
        }

        private void ExecuteCellSet(OlapReport olapReport)
        {
            NotifyCellSetChanging();
            foreach (Item item in olapReport.CategoricalElements)
            {
                item.Axis = AxisPosition.Categorical;
            }
            foreach (Item item in olapReport.SeriesElements)
            {
                item.Axis = AxisPosition.Series;
            }
            foreach (Item item in olapReport.SlicerElements)
            {
                item.Axis = AxisPosition.Slicer;
            }
            this.IsProcessing = true;
            if (olapReport.CategoricalElements.Count > 0)
            {
                if (olapReport.EnablePaging)
                {
                    AsyncCallback callback = new AsyncCallback(this.EndExecuteOlapReportWithTotalCount);
                    //this.DataProvider.BeginExecuteOlapReportWithTotalCount(this.CurrentReport, callback, this.DataProvider);
                    if (string.IsNullOrEmpty(ConnectionString))
                    {
                        this.DataProvider.BeginExecuteOlapReportWithTotalCount(this.CurrentReport, callback, this.DataProvider);
                    }
                    else
                    {
                        using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                        {                            
                            OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                            this.DataProvider.BeginExecuteOlapReportWithTotalCount(this.CurrentReport, callback, this.DataProvider);
                        }
                    }
                }
                else if (this.ValidateCurrentReport(this.CurrentReport))
                {
                    AsyncCallback callback = new AsyncCallback(this.EndExecuteOlapReportOnDrillState);
                    //this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                    if (string.IsNullOrEmpty(ConnectionString))
                    {
                        this.DataProvider.BeginExecuteOlapReportOnDrillState(this.CurrentReport, this.ShowLevelTypeAll, callback, this.DataProvider);
                    }
                    else
                    {
                        using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                        {
                            OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                            this.DataProvider.BeginExecuteOlapReportOnDrillState(this.CurrentReport, this.ShowLevelTypeAll, callback, this.DataProvider);
                        }
                    }
                }
                else if (this.ShowLevelTypeAll)
                {
                    AsyncCallback callback = new AsyncCallback(this.EndExecuteOlapReportWithLevelTypeAll);
                    if (string.IsNullOrEmpty(ConnectionString))
                    {
                        this.DataProvider.BeginExecuteOlapReportWithLevelTypeAll(this.CurrentReport, this.ShowLevelTypeAll, callback, this.DataProvider);
                    }
                    else
                    {
                        using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                        {
                            OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                            this.DataProvider.BeginExecuteOlapReportWithLevelTypeAll(this.CurrentReport, this.ShowLevelTypeAll, callback, this.DataProvider);
                        }
                    }
                }
                else
                {
                    AsyncCallback callback = new AsyncCallback(this.EndExecuteOlapReport);
                    //this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                    if (string.IsNullOrEmpty(ConnectionString))
                    {
                        this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                    }
                    else
                    {
                        using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                        {
                            OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                            this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(this.CurrentCubeName)&& string.IsNullOrEmpty(this.MdxQuery))
            {
                string query = "SELECT {} on 0 FROM " + Common.Utils.QuoteIdentifier(this.CurrentCubeName);
                this.ExecuteCellSet(query);
            }
            else
            {
                AsyncCallback callback = new AsyncCallback(this.EndExecuteOlapReport);
                //this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                }
                else
                {
                    using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                        this.DataProvider.BeginExecuteOlapReport(this.CurrentReport, callback, this.DataProvider);
                    }
                }
            }
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        public void Execute(string query)
        {
            this.IsProcessing = true;
            AsyncCallback callback = new AsyncCallback(this.EndExecute);
            //this.DataProvider.BeginExecute(query, callback, this.DataProvider);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                this.DataProvider.BeginExecute(query, callback, this.DataProvider);
            }
            else
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                    this.DataProvider.BeginExecute(query, callback, this.DataProvider);
                }
            }
        }

        private void EndExecuteOlapReport(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                this.CurrentCellSet = olapManager.EndExecuteOlapReport(asyncResult);

                this.Dispatcher.BeginInvoke(delegate
                    {
                        this.NotifyCellSetChanged();
                        this.IsProcessing = false;
                    });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { ExceptionObject = ex, Message = ex.Message });
            }
        }

        private void EndExecuteOlapReportWithLevelTypeAll(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                this.CurrentCellSet = olapManager.EndExecuteOlapReportWithLevelTypeAll(asyncResult);

                this.Dispatcher.BeginInvoke(delegate
                {
                    this.NotifyCellSetChanged();
                    this.IsProcessing = false;
                });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { ExceptionObject = ex, Message = ex.Message });
            }
        }

        private void EndExecuteOlapReportOnDrillState(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                var tempDictionary = olapManager.EndExecuteOlapReportOnDrillState(asyncResult);
                this.CurrentCellSet = (CellSet)tempDictionary["CellSet"];
                this.CurrentReport = (OlapReport)tempDictionary["OlapReport"];

                this.Dispatcher.BeginInvoke(delegate
                {
                    this.NotifyCellSetChanged();
                    this.IsProcessing = false;
                });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { ExceptionObject = ex, Message = ex.Message });
            }
        }

        private void EndExecuteOlapReportWithTotalCount(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                var dictionary = olapManager.EndExecuteOlapReportWithTotalCount(asyncResult);

                var isCellSetAvailable = dictionary.Keys.FirstOrDefault(k => k == "CellSet") != null;

                this.Counts = dictionary["Count"] as SerializableDictionary<string, int>;

                if (isCellSetAvailable)
                {
                    this.CurrentCellSet = dictionary["CellSet"] as CellSet;

                    if ((this.Counts["Column"] < (this.CurrentReport.PagerOptions.CategorialCurrentPage - 1) * this.CurrentReport.PagerOptions.CategorialPageSize || this.Counts["Row"] < (this.CurrentReport.PagerOptions.SeriesCurrentPage - 1) * this.CurrentReport.PagerOptions.SeriesPageSize))
                    {
                        this.CurrentReport.PagerOptions.CategorialCurrentPage = Math.Max(1, (int)Math.Ceiling((double)this.Counts["Column"] / this.CurrentReport.PagerOptions.CategorialPageSize));
                        this.CurrentReport.PagerOptions.SeriesCurrentPage = Math.Max(1, (int)Math.Ceiling((double)this.Counts["Row"] / this.CurrentReport.PagerOptions.SeriesPageSize));
                    }
                }
                else
                {
                    this.CurrentCellSet = null;
                }

                this.Dispatcher.BeginInvoke(delegate
                    {
                        this.NotifyCellSetChanged();
                        this.IsProcessing = false;
                    });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { ExceptionObject = ex, Message = ex.Message });
            }
        }

        private void EndExecuteMdxQuery(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                this.CurrentCellSet = olapManager.EndExecuteMdxQuery(asyncResult);
                this.Dispatcher.BeginInvoke(delegate
                {
                    this.NotifyCellSetChanged();
                    this.IsProcessing = false;
                });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { Message = ex.Message, ExceptionObject = ex });
            }
        }

        private void EndExecute(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                m_resultSet = olapManager.EndExecute(asyncResult);
                this.ResultSet = m_resultSet;
                this.Dispatcher.BeginInvoke(delegate
                {
                    this.NotifyResultSetChanged();
                    this.IsProcessing = false;
                });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { ExceptionObject = ex, Message = ex.Message });                
            }
        }

        /// <summary>
        /// Notifies the cell set changed.
        /// </summary>
        public void NotifyCellSetChanged()
        {
            //if (this.CurrentCellSet != null)
            {
                this.NotifyCellSetChanged(this.CurrentCellSet);
            }
        }

        private void NotifyCellSetChanging()
        {
            if (this.CellSetChanging != null)
            {
                this.CellSetChanging(this, new CellSetChangingEventArgs());
            }
        }

        private void NotifyCellSetChanged(CellSet cellSet)
        {
            if (this.CellSetChanged != null)
            {
                this.CellSetChanged(this, new CellSetChangedEventArgs() { NewCellSet = cellSet });
            }
        }

        private void NotifyResultSetChanged()
        {
            if (this.CellSetChanged != null)
            {
                this.CellSetChanged(this, new CellSetChangedEventArgs() { ResultSet = m_resultSet, NewCellSet = this.CurrentCellSet});
            }
        }

        #endregion

        #region Engine Creation
        /// <summary>
        /// Executes the olap table.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns></returns>
        public virtual PivotEngine ExecuteOlapTable(GridLayout layout)
        {
            if (this.ItemSource == null)
            {
                this.m_pivotEngine = this.ExecuteOlapTable(m_cellSet, layout);
            }
            else
            {
                PivotElements pivotDataElements = new PivotElements(this.CurrentReport);
                if (this.ItemSource is IEnumerable)
                {
                    var source = (IEnumerable)this.ItemSource;
                    System.Linq.IQueryable IQueryableSource = (IQueryable)source.AsQueryable();

                    if (IQueryableSource != null)
                    {
                        this.m_pivotEngine = TableBuilder.BuildEngineFromIQueryable(source.AsQueryable(), pivotDataElements.Report, this.RelationalDataSortOrder,
                            pivotDataElements.ColumnItems.ToArray(),
                            pivotDataElements.SeriesItems.ToArray(),
                            pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, layout, pivotDataElements.ExpandAll, null, false);
                    }
                }
            }
            return m_pivotEngine;
        }

        /// <summary>
        /// Engine generation for SASS Data by passing CellSet and Layout
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>A <see cref="PivotEngine"/> class.</returns>
        public virtual PivotEngine ExecuteOlapTable(CellSet cellSet, GridLayout layout)
        {
            if (cellSet != null)
            {
                if (layout == GridLayout.Normal || cellSet.Axes.Count < 2)
                {
                    if (layout == GridLayout.NormalTopSummary)
                    {
                        m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Top, layout, this.ShowLevelTypeAll, false);
                    }
                    else if (layout == GridLayout.NoSummaries)
                    {
                        m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.None, layout, this.ShowLevelTypeAll, false);
                    }
                    else
                    {
                        m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Bottom, layout, this.ShowLevelTypeAll, false);
                    }
                }
                else if (layout == GridLayout.NormalTopSummary)
                {
                    m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.Top, layout, this.ShowLevelTypeAll, false);
                }
                else if (layout == GridLayout.ExcelLikeLayout || layout== GridLayout.ExcelLikeLayoutWithMemberProperties)
                {
                    m_pivotEngine = TableBuilder.BuildEngineFromCellSetforExcelLayout(cellSet, null, false, layout, this.ShowLevelTypeAll, this);
                }
                else if (layout == GridLayout.NoSummaries)
                {
                    m_pivotEngine = TableBuilder.BuildEngineFromCellSet(cellSet, null, SummaryLayout.None, layout, this.ShowLevelTypeAll, false);
                }
            }

            return m_pivotEngine;
        }

        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <returns>A <see cref="PivotEngine"/> class.</returns>
        public PivotEngine ToggleExpandableState(PivotCellDescriptor cellDescriptor, GridLayout gridLayout)
        {
            if (this.ItemSource != null)
            {
                CustomSourceReportHelper.ProcessReport(cellDescriptor, gridLayout, this.CurrentReport);
                PivotElements pivotDataElements = new PivotElements(this.CurrentReport);

                if (this.ItemSource is IEnumerable)
                {
                    IEnumerable source = this.ItemSource as IEnumerable;
                    if (source != null)
                    {
                        this.m_pivotEngine = TableBuilder.BuildEngineFromIQueryable(source.AsQueryable(), pivotDataElements.Report, this.RelationalDataSortOrder,
                            pivotDataElements.ColumnItems.ToArray(),
                            pivotDataElements.SeriesItems.ToArray(),
                            pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, gridLayout, pivotDataElements.ExpandAll, cellDescriptor , false);
                    }
                }

                return this.m_pivotEngine;
            }
            return null;
        }

        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        /// <returns>Expanded state as a Boolean.</returns>
        public bool ToggleExpandableState(PivotCellDescriptorType cellType, Member member)
        {
            return this.ToggleExpandableState(cellType, member, true);
        }

        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        /// <returns>Expanded state as a Boolean.</returns>
        public bool ToggleExpandableState(PivotCellDescriptorType cellType, Member member, bool triggerEvents)
        {
            bool isDrilldown = false;
            if (member != null)
            {
                Items items = null;
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
                    isDrilldown = QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
                }
                else
                {
                    QueryBuilderEngineHelper.UpdateDrillDownItems(items, member);
                }
            }
            return isDrilldown;
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <returns>Expanded state as a Boolean.</returns>
        public bool ToggleExpandableStateOnDrillPosition(PivotCellDescriptor cellDescriptor)
        {
            if (this.CurrentReport == null || cellDescriptor.Tag == null) return false;
            return ToggleExpandableStateOnDrillPosition(cellDescriptor, cellDescriptor.Tag as Member);
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="member">The member.</param>
        /// <returns>Expanded state as a Boolean.</returns>
        public bool ToggleExpandableStateOnDrillPosition(PivotCellDescriptor cellDescriptor, Member member)
        {
            return this.ToggleExpandableStateOnDrillPosition(member, cellDescriptor.CellType, GetPositionsInfo(cellDescriptor), cellDescriptor.ExpandableState, true);
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="type">The type.</param>
        /// <param name="positionInfo">The position info.</param>
        /// <param name="state">The state.</param>
        /// <returns>Expanded state as a Boolean.</returns>
        public bool ToggleExpandableStateOnDrillPosition(Member member, PivotCellDescriptorType type, List<PositionInfo> positionInfo, ExpandableState state)
        {
            return this.ToggleExpandableStateOnDrillPosition(member, type, positionInfo, state, true);
        }

        /// <summary>
        /// Toggles the expandable state on drill position.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="type">The type.</param>
        /// <param name="positionInfo">The position info.</param>
        /// <param name="state">The state.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        /// <returns>Expanded state as a Boolean.</returns>
        public bool ToggleExpandableStateOnDrillPosition(Member member, PivotCellDescriptorType type, List<PositionInfo> positionInfo, ExpandableState state, bool triggerEvents)
        {
            if (member == null || this.CurrentReport == null
                || !(type == PivotCellDescriptorType.ColumnHeader || type == PivotCellDescriptorType.RowHeader))
                return false;

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
            return triggerEvents;
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
        /// <returns>A collection of <see cref="PositionInfo"/>.</returns>
        public List<PositionInfo> GetPositionsInfo(PivotCellDescriptor cellDescriptor)
        {
            List<PositionInfo> drillInfo = null;
            var locationInfo = PivotEngine.GetCellLocation(cellDescriptor);
            int row = locationInfo.Top, column = locationInfo.Left;

            if (row >= 0 && column >= 0 &&
                this.PivotEngine.TableColumns.Count > column && this.PivotEngine.RowsCount > row)
            {
                drillInfo = new List<PositionInfo>();

                if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader || cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
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
                member = cells[i].Tag as Member ?? cells[i].SpanCell.Tag as Member;

                if ((cells[i].CellType == cells[position].CellType || cells[i].CellType == PivotCellDescriptorType.SummaryRow || cells[i].CellType ==  PivotCellDescriptorType.SummaryColumn) && member != null)
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
        #endregion

        #region Getting cube informations

        /// <summary>
        /// Notifies the cube changed.
        /// </summary>
        private void NotifyCubeChanged()
        {
            try
            {
                if (this.CurrentCubeName != null && this.CurrentReport != null && this.CurrentReport.CurrentCubeName.ToUpper() != this.CurrentCubeName.ToUpper())
                {
                    this.CurrentReport.CurrentCubeName = this.CurrentCubeName;
                    this.CurrentReport.CategoricalElements.Clear();
                    this.CurrentReport.SeriesElements.Clear();
                    this.CurrentReport.SlicerElements.Clear();
                    if (this.CubeChanged != null)
                        this.CubeChanged(this, new CubeChangedEventArgs() { NewCubeName = this.CurrentReport.CurrentCubeName });
                    this.ExecuteCellSet(this.CurrentReport);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Raise the Async Call to get the cubes info from server.
        /// </summary>
        public void GetCubes()
        {
            try
            {
                AsyncCallback cubesCallback = new AsyncCallback(this.EndGetCubes);
                //this.DataProvider.BeginGetCubes(cubesCallback, this.DataProvider);
                if (string.IsNullOrEmpty(this.ConnectionString))
                {
                    this.DataProvider.BeginGetCubes(cubesCallback, this.DataProvider);
                }
                else
                {
                    using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));                        
                        this.DataProvider.BeginGetCubes(cubesCallback, this.DataProvider);
                    }
                }
                //this.DataProvider.BeginGetCubes(this.ConnectionString, cubesCallback, this.DataProvider);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


#region Exception

        #region Event Handlers

        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event EventHandler OnError;

        #endregion

        #region Event Delegates

        /// <summary>
        /// Event handler declaration for Error event.
        /// </summary>
        public delegate void EventHandler(object sender, ErrorEventArgs e);

        #endregion

        #region Raise OnError

        /// <summary>
        /// Raises the on error.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.OlapDataManager.ErrorEventArgs"/> instance containing the event data.</param>
        public virtual void RaiseOnError(ErrorEventArgs e)
        {
            if (this.OnError != null)
            {
                this.OnError(this, e);
            }
        }

        #endregion

        #region OnError Events Args

        /// <summary>
        /// Event arguments defines for Error related events.
        /// </summary>
        public class ErrorEventArgs
            :EventArgs
        {
            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            /// <value>The message.</value>
            public string Message { get; set; }
            /// <summary>
            /// Gets or sets the exception object.
            /// </summary>
            /// <value>The exception object.</value>
            public Exception ExceptionObject { get; set; }
        }

        #endregion

#endregion

        /// <summary>
        ///  Receive the response from server for the get cubes call raise the cube info collection changed event
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void EndGetCubes(IAsyncResult asyncResult)
        {

            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                CubeInfoCollection cubes = olapManager.EndGetCubes(asyncResult);
                if (cubes != null && cubes.Count > 0)
                {
                    this.Dispatcher.BeginInvoke(delegate
                    {
                        this.Cubes = cubes;
                        this.NotifyCubeInfoCollectionChanged();
                    });
                }
            }
            catch(Exception ex)
            {
                RaiseOnError(new ErrorEventArgs() { Message = ex.Message, ExceptionObject= ex });
            }
        }

        /// <summary>
        /// Gets the MDX query for current report of data manager.
        /// </summary>
        /// <param name="report">The report.</param>
        public void GetMdxQuery(OlapReport report)
        {
            try
            {
                AsyncCallback cubesCallback = new AsyncCallback(this.EndGetMdxQuery);
                //this.DataProvider.GetMdxQuery(report, cubesCallback, this.DataProvider);
                if (string.IsNullOrEmpty(this.ConnectionString))
                {
                    this.DataProvider.BeginGetMdxQuery(report, cubesCallback, this.DataProvider);
                }
                else
                {
                    using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                        this.DataProvider.BeginGetMdxQuery(report, cubesCallback, this.DataProvider);
                    }
                }
                //this.DataProvider.BeginGetMdxQuery(report, this.ConnectionString, cubesCallback, this.DataProvider);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EndGetMdxQuery(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider olapManager = asyncResult.AsyncState as IOlapDataProvider;
                string mdxQuery = olapManager.EndGetMdxQuery(asyncResult);
                this.Dispatcher.BeginInvoke(delegate
                {                    
                    this.CurrentReport.CurrentMdxQuery = mdxQuery;
                    if (this.MdxQueryObtained != null)
                    {
                        this.MdxQueryObtained();
                    }

                });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs() { Message = ex.Message, ExceptionObject = ex });
            }
        }

        /// <summary>
        /// Notifies the cube info collection changed.
        /// </summary>
        private void NotifyCubeInfoCollectionChanged()
        {
            if (this.CubeInfoCollectionChanged != null)
            {
                this.CubeInfoCollectionChanged(this, new CubeInfoCollectionChangedEventArgs() { NewCubes = this.Cubes });
            }
        }

        /// <summary>
        /// Raise the Async Call to get the cube schema from server.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        public void GetCubeSchema(string cubeName)
        {
            AsyncCallback cubesCallback = new AsyncCallback(this.EndGetCubeSchema);
            //this.DataProvider.BeginGetCubeSchema(cubeName,cubesCallback, this.DataProvider);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                this.DataProvider.BeginGetCubeSchema(cubeName, cubesCallback, this.DataProvider);
            }
            else
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                    this.DataProvider.BeginGetCubeSchema(cubeName, cubesCallback, this.DataProvider);
                }
            }
        }

        /// <summary>
        /// Receive the response from server for the get cube schema call and raise the cube schema changed event.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void EndGetCubeSchema(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider dataProvider = asyncResult.AsyncState as IOlapDataProvider;
                CubeSchema cubeSchema = dataProvider.EndGetCubeSchema(asyncResult);
                this.CurrentCubeSchema = cubeSchema;
                this.CurrentCubeSchema.DataProvider = this;
                this.Dispatcher.BeginInvoke(delegate
                {
                    if (this.CubeSchemaChanged != null)
                    {
                        this.CubeSchemaChanged(this, new CubeSchemaChangedEventArgs() { NewCubeSchema = cubeSchema });
                    }
                });
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { Message = ex.Message, ExceptionObject = ex });
            }
        }

        #endregion

        #region Refreshing events

        /// <summary>
        /// Notifies the current report of OlapDataManager has been changed. 
        /// </summary>
        public void NotifyReportChanged()
        {
            try
            {
                bool IsCubeSchemaChanged = false;
                if (this.ReportChanged != null)
                {
                    this.ReportChanged(this, new ReportChangedEventArgs { NewReport = this.CurrentReport });
                }

                if (this.CurrentReport.CurrentCubeName != this.CurrentCubeName)
                {
                    this.CurrentCubeName = this.CurrentReport.CurrentCubeName;

                    if (this.CubeChanged != null)
                    {
                        IsCubeSchemaChanged = true;
                        this.CubeChanged(this, new CubeChangedEventArgs() { NewCubeName = this.CurrentReport.CurrentCubeName });
                    }
                }

                if (this.CurrentCubeSchema != null && this.AxisElementChanged != null && !IsCubeSchemaChanged)
                {
                    this.AxisElementChanged(this, null);
                }

                this.ExecuteCellSet(this.CurrentReport);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Notifies the element changed in particular axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void NotifyElementChanged(AxisPosition axis)
        {
            this.IsCurrentReportModified = true;
            this.AxisElementChanged(this, new AxisElementChangedEventArgs { NewPosition = axis });
            this.ExecuteCellSet(this.CurrentReport);
        }

        /// <summary>
        /// Notifies the element changed.
        /// </summary>
        public void NotifyElementChanged()
        {
            this.IsCurrentReportModified = true;
            this.AxisElementChanged(this, null);
            this.ExecuteCellSet(this.CurrentReport);
        }


        /// <summary>
        /// Refreshes the axis element builder.
        /// </summary>
        public void RefreshAxisElementBuilder()
        {
            this.IsCurrentReportModified = true;
            this.AxisElementChanged(this, null);
        }

        #endregion

        #region Getting Level Members
        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="level">The parent level.</param>
        public void GetLevelMembers(string levelUniqueName, string cubeName)
        {
            AsyncCallback callback = new AsyncCallback(this.EndGetLevelMembers);
            //this.DataProvider.BeginGetLevelMembers(levelUniqueName, cubeName, callback, this.DataProvider);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                this.DataProvider.BeginGetLevelMembers(levelUniqueName, cubeName, callback, this.DataProvider);
            }
            else
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                    this.DataProvider.BeginGetLevelMembers(levelUniqueName, cubeName, callback, this.DataProvider);
                }
            }
        }

        /// <summary>
        ///  Receive the response from server and updated the level members
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void EndGetLevelMembers(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider dataProvider = asyncResult.AsyncState as IOlapDataProvider;
                MemberCollection membercollection = dataProvider.EndGetLevelMembers(asyncResult);
                if (membercollection != null)
                {
                    this.Dispatcher.BeginInvoke(delegate
                    {
                        if (this.LevelMembersObtained != null)
                        {
                            this.LevelMembersObtained(this, new LevelMembersObtainedEventArgs { LevelMembers = membercollection });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { ExceptionObject = ex, Message = ex.Message });
            }
        }
        #endregion

        #region Getting Child Member

        public void GetChildrenByMDX(string commandText)
        {
            AsyncCallback callback = new AsyncCallback(this.EndGetChildrenByMDX);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                this.DataProvider.BeginGetChildrenByMDX(commandText,callback,this.DataProvider);
            }
            else
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                    this.DataProvider.BeginGetChildrenByMDX(commandText,callback,this.DataProvider);
                }
            }
        }

        private void EndGetChildrenByMDX(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider dataProvider = asyncResult.AsyncState as IOlapDataProvider;
                MemberCollection membercollection = dataProvider.EndGetChildrenByMDX(asyncResult);
                if (membercollection != null)
                {
                    this.Dispatcher.BeginInvoke(delegate
                    {
                        if (this.ChildMembersObtained != null)
                        {
                            this.ChildMembersObtained(this, new ChildMembersObtaindedEventArgs { ChildMembers = membercollection });
                        }
                    });
                }
            }
           catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { Message = ex.Message, ExceptionObject = ex });
            }
        }



        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="member">Parent Member</param>
        public void GetChildMembers(string memberUniqueName, string cubeName)
        {
            AsyncCallback callback = new AsyncCallback(this.EndGetChildMembers);
            //this.DataProvider.BeginGetChildMembers(memberUniqueName, cubeName, callback, this.DataProvider);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                this.DataProvider.BeginGetChildMembers(memberUniqueName, cubeName, callback, this.DataProvider);
            }
            else
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)this.DataProvider))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(this.ConnectionStringOptions.MessageHeaderName, this.ConnectionStringOptions.Namespace, this.ConnectionStringOptions.EncryptConnectionString ? this.EncryptedConnectionString : this.ConnectionString + this.ConnectionStringOptions.StringSplitter + this.ProviderName));
                    this.DataProvider.BeginGetChildMembers(memberUniqueName, cubeName, callback, this.DataProvider);
                }
            }
        }

        /// <summary>
        /// Receive the response from server and updated the child members
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void EndGetChildMembers(IAsyncResult asyncResult)
        {
            try
            {
                IOlapDataProvider dataProvider = asyncResult.AsyncState as IOlapDataProvider;
                MemberCollection membercollection = dataProvider.EndGetChildMembers(asyncResult);
                if (membercollection != null)
                {
                    this.Dispatcher.BeginInvoke(delegate
                    {
                        if (this.ChildMembersObtained != null)
                        {
                            this.ChildMembersObtained(this, new ChildMembersObtaindedEventArgs { ChildMembers = membercollection });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                RaiseOnError(new ErrorEventArgs { Message = ex.Message, ExceptionObject = ex });                
            }
        }
        #endregion

        #region Updation of parent
        /// <summary>
        /// Sets the parent elements of various cube elements.
        /// </summary>
        /// <param name="m_cubeScheam">The cube scheam.</param>
        private void SetParentElements(CubeSchema m_cubeScheam)
        {
            foreach (Dimension dimension in m_cubeScheam.Dimensions)
            {
                dimension.ParentCubeSchema = m_cubeScheam;
                foreach (Hierarchy hierarchy in dimension.Hierarchies)
                {
                    hierarchy.ParentDimension = dimension;
                    foreach (Level level in hierarchy.Levels)
                    {
                        level.ParentHierarchy = hierarchy;
                        level.CubeSchema = m_cubeScheam;
                    }
                }
            }
        }
        #endregion

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
    }

    /// <summary>
    /// Holds information about connection string related information for WCF Service.
    /// </summary>
    public class ConnectionStringOptions
    {
        private string _msgHeaderName, _nsName, _encryptionKey, _stringSplitter;

        #region Properties

        /// <summary>
        /// Gets or sets the name of the message header.
        /// </summary>
        /// <value>The name of the message header.</value>
        public string MessageHeaderName
        {
            get
            {
                if (string.IsNullOrEmpty(_msgHeaderName))
                {
                    _msgHeaderName = "ConnectionString";
                }
                return _msgHeaderName;
            }
            set { _msgHeaderName = value; }
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace
        {
            get
            {
                if (string.IsNullOrEmpty(_nsName))
                {
                    _nsName = "http://schemas.syncfusion.com/silverlight/olapclient";
                }
                return _nsName;
            }
            set { _nsName = value; }
        }

        /// <summary>
        /// Gets or sets the encryption key.
        /// </summary>
        /// <value>The encryption key.</value>
        public string EncryptionKey
        {
            get
            {
                if (this.EncryptConnectionString && string.IsNullOrEmpty(_encryptionKey))
                {
                    _encryptionKey = "syncfusion!10";
                }
                return _encryptionKey;
            }
            set { _encryptionKey = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [encrypt connection string].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [encrypt connection string]; otherwise, <c>false</c>.
        /// </value>
        public bool EncryptConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the splitter string, which is used to append an extra information like provider name in to the connection string message header.
        /// Default splitter is pipeline symbol ("|").
        /// </summary>
        public string StringSplitter
        {
            get
            {
                if (string.IsNullOrEmpty(_stringSplitter))
                {
                    _stringSplitter = "|";
                }

                return _stringSplitter;
            }
            set
            {
                _stringSplitter = value;
            }
        }

        #endregion

    }

    /// <summary>
    /// Represents the data provider names.
    /// </summary>
    public enum Providers
    {
        /// <summary>
        /// Represents the SSAS (SQL Server Analysis Services). This is default value.
        /// </summary>
        SSAS,
        /// <summary>
        /// Represents the Mondrian Service.
        /// </summary>
        Mondrian,
        /// <summary>
        /// Represents the ActivePivot Service.
        /// </summary>
        ActivePivot
    }

}
