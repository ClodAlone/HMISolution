using System;
using System.Collections.Generic;
using System.Linq;
using DataReader;
using DataReader.Extensions;
using ReportSettings.Documents;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.XtraReports.Parameters;
using System.Data;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.Data.Browsing.Design;
using System.Diagnostics;
using UFUAEditor.ComponentService;
using DataReader.Helpers;
using StringManager.ComponentService;
using OPCUAViewModel;
using Utilities;
using DevExpress.XtraCharts;

namespace ReportManager.ReportService
{
    public sealed class ReportServiceHelper
    {
        #region Declarations

        readonly ReportDocument reportDoc;
        readonly IUFUAEditorManager ufuaEditorManager;
        readonly DefaultSourceType defaultSourceType;
        readonly bool throwOnError;

        #endregion

        #region Constants
        /// <summary>
        /// header for Xpo initial catalog connection string.
        /// </summary>
        const String CatalogSourceHeader = "initial catalog";
        /// <summary>
        /// Default data source name used in the report
        /// </summary>
        const String DefaultDataSourceName = "DataSource";
        /// <summary>
        /// Data source name key to use for Event.
        /// </summary>
        const String EventDataSourceKey = "Event";
        /// <summary>
        /// Data source name key to use for Historian.
        /// </summary>
        const String HistorianDataSourceKey = "Historian";
        /// <summary>
        /// Table name for historical database
        /// </summary>
        const String HistorianTableName = "UFUAAuditDataItem";
        /// <summary>
        /// Table name for event log database
        /// </summary>
        const String EventLogTableName = "UFUAAuditLogItem";
        /// <summary>
        /// Special parameter searched inside the report for filling with current user name
        /// </summary>
        const String UserNameParameter = "_UserName_";
        /// <summary>
        /// Special parameter searched inside the report for filling with current project's default connection
        /// </summary>
        const String defaultConnectionParameter = "_ConnectionString_";
        /// <summary>
        /// Special parameter searched inside the report for filling with current project's culture strings
        /// </summary>
        const String cultureStringsParameter = "_CurrentCultureStrings_";
        /// <summary>
        /// Argument char separator for key map
        /// </summary>
        const Char argumentKeySeparator = '@';
        #endregion

        #region Constructors
        public ReportServiceHelper(ReportDocument doc, IUFUAEditorManager ufuaeditormanager) :
            this(doc, ufuaeditormanager, DefaultSourceType.Undefined, throwOnException: false)
        { }
        
        public ReportServiceHelper(ReportDocument doc, IUFUAEditorManager ufuaeditormanager, DefaultSourceType sourceType) :
            this(doc, ufuaeditormanager, sourceType, throwOnException: false)
        { }

        public ReportServiceHelper(ReportDocument doc, IUFUAEditorManager ufuaeditormanager, bool throwOnException) :
            this(doc, ufuaeditormanager, DefaultSourceType.Undefined, throwOnException)
        { }

        public ReportServiceHelper(ReportDocument doc, IUFUAEditorManager ufuaeditormanager, DefaultSourceType sourceType, bool throwOnException)
        {
            reportDoc = doc;
            ufuaEditorManager = ufuaeditormanager;            
            var model = new DataReader.DataReaderModel(reportDoc.ReaderItemSources);
            model.Connection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(reportDoc.ReaderItemSources.Connection, reportDoc.SessionString);
            model.NormalizeConnectionString(doc?.rootBase);
            ReportDocModel = model;
            defaultSourceType = sourceType;
            throwOnError = throwOnException;
        }
        #endregion

        #region Properties

        public ReportDocument ReportDoc
        {
            get
            {
                return reportDoc;
            }
        }

        DataReaderModel reportDocModel;
        public DataReaderModel ReportDocModel 
        {
            get 
            {
                return reportDocModel;
            }
            set
            {
                if (reportDocModel == value)
                    return;

                reportDocModel = value;
            }
        }

        String lastUfuaHistorianConnection;
        DataReaderModel ufuaHistorianModel;
        public DataReaderModel UFUAHistorianModel
        {
            get
            {
                if (reportDoc != null)
                {
                    if (ufuaEditorManager != null/* && !ReportDoc.ReaderItemSources.IsValidDataSource()*/)
                    {
                        string historianconn = ufuaEditorManager.GetHistorianDefaultConnection(reportDoc, createTable: true);
                        if (lastUfuaHistorianConnection != historianconn)
                        {
                            lastUfuaHistorianConnection = historianconn;
                            if (String.IsNullOrEmpty(historianconn))
                                ufuaHistorianModel = null;
                            else
                            {
                                var helper = new ConnectionStringParser(historianconn);
                                ufuaHistorianModel = new DataReaderModel()
                                {
                                    DataSourceName = String.Format("{0} {1}", HistorianDataSourceKey, "(Default)"),
                                    DataSourceDisplayName = String.Format(Properties.Resources.HistorianDataSourceDisplayName, helper.GetPartByName(CatalogSourceHeader)),
                                    DataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(historianconn),
                                    Connection = XpoConversionHelper.GetConnectionStringFromXpoConnection(historianconn)
                                };

                                ufuaHistorianModel.Connection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ufuaHistorianModel.Connection, ReportDoc.SessionString);

                                try
                                {
                                    using (var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ufuaHistorianModel.DataProvider, ufuaHistorianModel.Connection))
                                    {
                                        ufuaHistorianModel.Select = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(HistorianTableName));
                                    }
                                }
                                catch
                                {
                                    ufuaHistorianModel.Select = String.Format("SELECT * FROM {0}", HistorianTableName);
                                }
                            }
                        }

                        if (ufuaHistorianModel != null)
                            return ufuaHistorianModel;
                    }
                }

                return null;
            }
        }

        String lastUfuaEventConnection;
        DataReaderModel ufuaEventLogModel;
        public DataReaderModel UFUAEventLogModel
        {
            get
            {
                if (reportDoc != null)
                {
                    if (ufuaEditorManager != null/* && !ReportDoc.ReaderItemSources.IsValidDataSource()*/)
                    {
                        string eventconn = ufuaEditorManager.GetEventDefaultConnection(reportDoc, createTable: true);
                        if (lastUfuaEventConnection != eventconn)
                        {
                            lastUfuaEventConnection = eventconn;
                            if (String.IsNullOrEmpty(eventconn))
                                ufuaEventLogModel = null;
                            else
                            {
                                var helper = new ConnectionStringParser(eventconn);
                                ufuaEventLogModel = new DataReaderModel()
                                {
                                    DataSourceName = String.Format("{0} {1}", EventDataSourceKey, "(Deafult)"),
                                    DataSourceDisplayName = String.Format(Properties.Resources.EventDataSourceDisplayName, helper.GetPartByName(CatalogSourceHeader)),
                                    DataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(eventconn),
                                    Connection = XpoConversionHelper.GetConnectionStringFromXpoConnection(eventconn)
                                };

                                ufuaEventLogModel.Connection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ufuaEventLogModel.Connection, ReportDoc.SessionString);

                                try
                                {
                                    using (var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ufuaEventLogModel.DataProvider, ufuaEventLogModel.Connection))
                                    {
                                        ufuaEventLogModel.Select = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(EventLogTableName));
                                    }
                                }
                                catch
                                {
                                    ufuaEventLogModel.Select = String.Format("SELECT * FROM {0}", EventLogTableName);
                                }
                            }
                        }

                        if (ufuaEventLogModel != null)
                            return ufuaEventLogModel;
                    }
                }

                return null;
            }
        }

        public string DataSourceName
        {
            get
            {
                if (ReportDocModel != null && !String.IsNullOrEmpty(ReportDocModel.DataSourceName))
                    return ReportDocModel.DataSourceName;
                else if ((defaultSourceType == DefaultSourceType.Historian || defaultSourceType == DefaultSourceType.Undefined) && 
                    UFUAHistorianModel != null && !String.IsNullOrEmpty(UFUAHistorianModel.DataSourceName))
                        return UFUAHistorianModel.DataSourceName;
                else if ((defaultSourceType == DefaultSourceType.EventLog || defaultSourceType == DefaultSourceType.Undefined) && 
                    UFUAEventLogModel != null && !String.IsNullOrEmpty(UFUAEventLogModel.DataSourceName))
                        return UFUAEventLogModel.DataSourceName;

                return DefaultDataSourceName;
            }
        }

        public string DataSourceDisplayName
        {
            get
            {
                if (ReportDocModel != null && !String.IsNullOrEmpty(ReportDocModel.DataSourceDisplayName))
                    return ReportDocModel.DataSourceDisplayName;
                else if ((defaultSourceType == DefaultSourceType.Historian || defaultSourceType == DefaultSourceType.Undefined) && 
                    UFUAHistorianModel != null && !String.IsNullOrEmpty(UFUAHistorianModel.DataSourceDisplayName))
                    return UFUAHistorianModel.DataSourceDisplayName;
                else if ((defaultSourceType == DefaultSourceType.EventLog || defaultSourceType == DefaultSourceType.Undefined) && 
                    UFUAEventLogModel != null && !String.IsNullOrEmpty(UFUAEventLogModel.DataSourceDisplayName))
                    return UFUAEventLogModel.DataSourceDisplayName;

                return Properties.Resources.InvalidDataSourceDisplayName;
            }
        }

        public string DataProviderName
        {
            get
            {
                if (ReportDocModel != null && !String.IsNullOrEmpty(ReportDocModel.DataProvider))
                    return ReportDocModel.DataProvider;
                else if ((defaultSourceType == DefaultSourceType.Historian || defaultSourceType == DefaultSourceType.Undefined) &&
                    UFUAHistorianModel != null && !String.IsNullOrEmpty(UFUAHistorianModel.DataProvider))
                    return UFUAHistorianModel.DataProvider;
                else if ((defaultSourceType == DefaultSourceType.EventLog || defaultSourceType == DefaultSourceType.Undefined) &&
                    UFUAEventLogModel != null && !String.IsNullOrEmpty(UFUAEventLogModel.DataProvider))
                    return UFUAEventLogModel.DataProvider;

                return null;
            }
        }

        public string DataConnection
        {
            get
            {
                if (ReportDocModel != null && !String.IsNullOrEmpty(ReportDocModel.Connection))
                    return ReportDocModel.Connection;
                else if ((defaultSourceType == DefaultSourceType.Historian || defaultSourceType == DefaultSourceType.Undefined) &&
                    UFUAHistorianModel != null && !String.IsNullOrEmpty(UFUAHistorianModel.Connection))
                    return UFUAHistorianModel.Connection;
                else if ((defaultSourceType == DefaultSourceType.EventLog || defaultSourceType == DefaultSourceType.Undefined) &&
                    UFUAEventLogModel != null && !String.IsNullOrEmpty(UFUAEventLogModel.Connection))
                    return UFUAEventLogModel.Connection;

                return null;
            }
        }

        public string ReportUserNameParameter
        {
            get
            {
                return UserNameParameter;
            }
        }

        public string DefaultConnectionParameter
        {
            get
            {
                return defaultConnectionParameter;
            }
        }

        public string CultureStringsParameter
        {
            get
            {
                return cultureStringsParameter;
            }
        }

        public byte[] ReportData
        {
            get
            {
                if (reportDoc != null)
                    return reportDoc.ReportData;

                return null;
            }
        }

        public byte[] ReportDataXML
        {
            get
            {
                if (reportDoc != null)
                    return reportDoc.ReportDataXML;

                return null;
            }
        }

        Dictionary<String, byte[]> SubReportDataXML
        {
            get
            {
                if (reportDoc != null)
                    return reportDoc.SubReportDataXML;

                return null;
            }
        }

        string lastErrorInfo;
        public string LastErrorInfo
        {
            get 
            { 
                return lastErrorInfo ?? String.Empty;
            }
        }

        bool invalidDataSource;
        public bool InvalidDataSource
        {
            get
            {
                return invalidDataSource;
            }
            set
            {
                if (invalidDataSource == value)
                    return;
                invalidDataSource = value;
            }
        }

        int maxTake;
        public int MaxTake
        {
            get
            {
                return maxTake;
            }
            set
            {
                if (maxTake == value)
                    return;
                maxTake = value;
            }
        }

        int commandTimeout;
        public int CommandTimeout
        {
            get
            {
                return commandTimeout;
            }
            set
            {
                if (commandTimeout == value)
                    return;
                commandTimeout = value;
            }
        }

        #endregion

        #region Members
        static IDictionary<String, ReportParameters.Parameter> emptyParameters = null;
        byte[] emptyLayoutData = null;
        public XtraReport PrepareXtraReportDocument(bool onlyschema = false)
        {
            return PrepareXtraReportDocument(emptyParameters, emptyLayoutData, onlyschema);
        }

#if !NET_STANDARD
        public XtraReport PrepareXtraReportDocument(DevExpress.Xpf.Reports.UserDesigner.IReportSerializer serializer, XtraReport report, bool onlyschema = false)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                serializer.Save(stream, report);
                return PrepareXtraReportDocument(emptyParameters, stream.ToArray(), onlyschema);
            }
        }
#endif

        public XtraReport PrepareXtraReportDocument(IList<ReportParameters.Parameter> parameters)
        {
            if (parameters != null)
            {
                var mapParameters = new Dictionary<String, ReportParameters.Parameter>();
                foreach (var par in parameters)
                {
                    if (!mapParameters.ContainsKey(par.Name))
                        mapParameters.Add(par.Name, par);
                }

                return PrepareXtraReportDocument(mapParameters, emptyLayoutData, false);
            }
            
            return PrepareXtraReportDocument(emptyParameters, emptyLayoutData, false);
        }

        public XtraReport PrepareXtraReportDocument(IDictionary<String, ReportParameters.Parameter> parameters, byte[] layoutData, bool onlyschema)
        {
            lastErrorInfo = String.Empty;
            if (layoutData == null)
#if !NET_STANDARD
                if (ReportDataXML != null)
                    layoutData = ReportDataXML;
                else
                    layoutData = ReportData;
#else
                layoutData = ReportDataXML;
#endif            

            XtraReport report = null;
            if (layoutData != null)
            {
                using (var stream = new MemoryStream(layoutData))
                {
                    try
                    {
                        report = XtraReport.FromStream(stream, true);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.Fail("Invalid report layout ('{0}')", ex.Message);
#endif
                        if (ex is NotSupportedException)
                            lastErrorInfo = Properties.Resources.NotSupportedReportLayout;
                        else
                            lastErrorInfo = String.Format(Properties.Resources.UnknowReportLayout, ex.Message);
                    }
                }
            }

            if (report == null)
            {
                if (onlyschema)
                    report = new XtraReport();
                else
                    return new XtraReport();
            }
            else if (!onlyschema && SubReportDataXML != null && SubReportDataXML.Count > 0)
            {
                LoadSubReports(report, parameters, onlyschema);
            }

            // Add default parameters for any reports
            if (ufuaEditorManager != null)
            {
                if (parameters == null)
                    parameters = new Dictionary<String, ReportParameters.Parameter>();

                if (!parameters.ContainsKey(DefaultConnectionParameter))
                {
                    parameters.Add(DefaultConnectionParameter,
                        new ReportParameters.Parameter()
                        {
                            Name = DefaultConnectionParameter,
                            Type = ReportParameters.ParameterType.String,
                            Value = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(
                                ufuaEditorManager.GetHistorianDefaultConnection(reportDoc), reportDoc.SessionString)
                        });
                }
            }

            if (!onlyschema)
                LocalizeReport(report);

            if (report != null && parameters != null)
            {
                if (report.Parameters != null)
                {
                    foreach (var par in report.Parameters)
                    {
                        if (parameters.ContainsKey(par.Name))
                        {
                            try
                            {
                                var value = Convert.ChangeType(parameters[par.Name].Value, par.Type);
                                par.Value = value;
                                par.Visible = false;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(String.Format("Error setting the report parameter '{0}' : {1}", par.Name, ex.Message));
                            }
                        }
                    }
                }
            }

            try
            {
                NormalizeConnectionString(report);
                if (!(report.DataSource is DevExpress.DataAccess.Sql.SqlDataSource))
                {
                    var dataModel = CreateDataReaderModel();
                    if (dataModel != null && !dataModel.IsValid() &&
                        ufuaEditorManager != null)
                    {
                        string connectionString;
                        if (defaultSourceType == DefaultSourceType.EventLog)
                            connectionString = ufuaEditorManager.GetEventDefaultConnection(reportDoc);
                        else
                            connectionString = ufuaEditorManager.GetHistorianDefaultConnection(reportDoc);

                        connectionString = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(connectionString, ReportDoc.SessionString);
                        var sqlDataSource = SqlDataSourceFactory.CreateSqlDataSource(connectionString);
                        if (CommandTimeout > 0)
                            sqlDataSource.ConnectionOptions.DbCommandTimeout = CommandTimeout;

                        string select;
                        if (!onlyschema && !string.IsNullOrEmpty(dataModel.Where))
                        {
                            if (MaxTake > 0)
                                dataModel.AddTopClause(MaxTake);
                            select = String.Format("{0} where {1}", dataModel.Select, dataModel.WhereClause());
                        }
                        else
                            select = dataModel.Select;
                        if (!string.IsNullOrEmpty(dataModel.GroupBy))
                            select = String.Format("{0} group by {1}", select, dataModel.GroupBy);
                        if (!onlyschema && !string.IsNullOrEmpty(dataModel.Sort))
                            select = String.Format("{0} order by {1}", select, dataModel.Sort);
                        sqlDataSource.Queries.Add(new DevExpress.DataAccess.Sql.CustomSqlQuery("Table", select));
                        sqlDataSource.RebuildResultSchema();
                        report.DataSource = sqlDataSource;
                        report.DataMember = "Table";
                    }
                }                

                if (report.DataSource is DevExpress.DataAccess.Sql.SqlDataSource)
                {
                    var sqldata = report.DataSource as DevExpress.DataAccess.Sql.SqlDataSource;
                    if (!onlyschema)
                    {
                        bool fill = false;
                        if (parameters != null)
                        {
                            foreach (var query in sqldata.Queries)
                            {
                                foreach (var par in query.Parameters)
                                {
                                    if (parameters.ContainsKey(par.Name))
                                    {
                                        try
                                        {
                                            var value = Convert.ChangeType(parameters[par.Name].Value, par.Type);
                                            par.Value = value;
                                            fill = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine(String.Format("Error setting the report query parameter '{0}' : {1}", par.Name, ex.Message));
                                        }
                                    }
                                }
                            }
                        }

                        if (fill)
                            sqldata.Fill();
                    }
                    else
                        sqldata.RebuildResultSchema();
                }
                else if (report.ComponentStorage.Count == 0)
                {
                    var dataset = new DataSet(DataSourceName);
                    FillDataSource(ref dataset, onlyschema, parameters.Values.ToList());
                    //if (!InvalidDataSource)
                    {
                        report.DataSource = dataset;
                        if (dataset.Tables.Count > 0)
                            report.DataMember = dataset.Tables[0].TableName;
                        //report.RegisterDataSourceName(DisplayDataSourceName, report.DataSource);
                    }
                }
            }
            catch (Exception ex)
            {
                lastErrorInfo = String.Format(Properties.Resources.ErrorInfoOnPreparingReport, ex.Message);

                if (throwOnError)
                    throw ex;
            }

            return report;
        }

        public void LoadSubReports(XtraReport report, bool onlyschema)
        {
            LoadSubReports(report, null, onlyschema);
        }

        void LoadSubReports(XtraReport report, IDictionary<String, ReportParameters.Parameter> parameters, bool onlyschema)
        {
            foreach (Band band in report.Bands)
                LoadSubReports(band, parameters, onlyschema);
        }

        void LoadSubReports(Band band, IDictionary<String, ReportParameters.Parameter> parameters, bool onlyschema)
        {
            foreach (XRControl control in band.Controls)
            {
                LoadSubReports(control, parameters, onlyschema);
            }
        }

        void LoadSubReports(XRPanel band, IDictionary<String, ReportParameters.Parameter> parameters, bool onlyschema)
        {
            foreach (XRControl control in band.Controls)
            {
                LoadSubReports(control, parameters, onlyschema);
            }
        }

        void LoadSubReports(XRControl control, IDictionary<String, ReportParameters.Parameter> parameters, bool onlyschema)
        {
            if (control is XRSubreport)
            {
                var subReport = control as XRSubreport;
                var key = GetUniqueControlKey(subReport, subReport.Name);
                if (SubReportDataXML != null && SubReportDataXML.ContainsKey(key))
                    subReport.ReportSource = PrepareXtraReportDocument(parameters, SubReportDataXML[key], onlyschema);
                else
                    subReport.ReportSource = new XtraReport() { Name = subReport.Name };
            }
            else if (control is XRPanel)
            {
                var p = control as XRPanel;
                LoadSubReports(p, parameters, onlyschema);
            }
            else if (control is Band)
            {
                var b = control as Band;
                LoadSubReports(b, parameters, onlyschema);
            }
        }

        public String GetUniqueControlKey(XRControl ownerControl, String keyName)
        {
            return String.Format("{0}{1}{2}", ownerControl.RootReport.Name, argumentKeySeparator, keyName);
        }

        void LocalizeReport(XtraReport report)
        {
            var stringEditor = reportDoc.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            if (stringEditor == null)
                return;
            var map = stringEditor.GetListStringForCulture(reportDoc, stringEditor.GetActiveCulture(reportDoc));
            if (map == null || map.Count == 0)
                return;

            if (report != null)
            {
                foreach (Band band in report.Bands)
                    TranslateControls(map, band);

                if (report.Parameters != null)
                {
                    var parameter = report.Parameters[CultureStringsParameter] as Parameter;
                    if (parameter != null)
                    {
                        try
                        {
                            parameter.Value = map.ToXml();
                            parameter.Visible = false;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(String.Format("Error setting the report parameter '{0}' : {1}", parameter.Name, ex.Message));
                        }
                    }
                }
            }
        }

        private static void TranslateControls(IDictionary<string, string> map, Band band)
        {
            foreach (XRControl control in band.Controls)
            {
                TranslateControl(map, control);
            }
        }

        private static void TranslateControls(IDictionary<string, string> map, XRPanel band)
        {
            foreach (XRControl control in band.Controls)
            {
                TranslateControl(map, control);
            }
        }

        private static void TranslateControls(IDictionary<string, string> map, XRTable table)
        {
            foreach (XRTableRow control in table.Rows)
            {
                TranslateControl(map, control);
            }
        }

        private static void TranslateControls(IDictionary<string, string> map, XRTableRow tablerow)
        {
            foreach (XRTableCell control in tablerow.Cells)
            {
                TranslateControl(map, control);
            }
        }

        private static void TranslateControls(IDictionary<string, string> map, XRChart chart)
        {
            foreach (XRControl control in chart.Controls)
            {
                TranslateControl(map, control);
            }
        }

        private static void TranslateControl(IDictionary<string, string> map, XRControl control)
        {
            if (control is XRLabel)
            {
                var label = control as XRLabel;
                if (map.ContainsKey(label.Text))
                    label.Text = map[label.Text];
            }
            else if (control is XRPanel)
            {
                var p = control as XRPanel;
                if (map.ContainsKey(p.Text))
                    p.Text = map[p.Text];
                TranslateControls(map, p);
            }
            else if (control is XRTableRow)
            {
                var p = control as XRTableRow;
                if (map.ContainsKey(p.Text))
                    p.Text = map[p.Text];
                TranslateControls(map, p);
            }
            else if (control is XRTable)
            {
                var p = control as XRTable;
                if (map.ContainsKey(p.Text))
                    p.Text = map[p.Text];
                TranslateControls(map, p);
            }
            else if (control is Band)
            {
                var b = control as Band;
                TranslateControls(map, b);
            }
            else if (control is XRChart)
            {
                var c = control as XRChart;
                if (c.Diagram is XYDiagram)
                {
                    var diagram = c.Diagram as XYDiagram;
                    if (diagram.AxisX != null && diagram.AxisX.Title != null &&
                        !String.IsNullOrEmpty(diagram.AxisX.Title.Text))
                    {
                        var title = diagram.AxisX.Title.Text;
                        if (map.ContainsKey(title))
                            diagram.AxisX.Title.Text = map[title];
                    }
                    if (diagram.AxisY != null && diagram.AxisY.Title != null &&
                        !String.IsNullOrEmpty(diagram.AxisY.Title.Text))
                    {
                        var title = diagram.AxisY.Title.Text;
                        if (map.ContainsKey(title))
                            diagram.AxisY.Title.Text = map[title];
                    }
                }

                TranslateControls(map, c);
            }
        }

        public void NormalizeConnectionString(XtraReport report)
        {
            for (int ii = 0; ii < report.ComponentStorage.Count; ii++)
            {
                if (report.ComponentStorage[ii] is DevExpress.DataAccess.Sql.SqlDataSource)
                {
                    var sqldata = report.ComponentStorage[ii] as DevExpress.DataAccess.Sql.SqlDataSource;
                    if (sqldata != null && sqldata.ConnectionParameters is DevExpress.DataAccess.ConnectionParameters.FileConnectionParametersBase)
                    {
                        var fileConnectionParameters = sqldata.ConnectionParameters as DevExpress.DataAccess.ConnectionParameters.FileConnectionParametersBase;
                        fileConnectionParameters.FileName = XpoHelpers.XpoHelper.NormalizeConnectionString(fileConnectionParameters.FileName?.Replace('\\', Path.DirectorySeparatorChar), reportDoc?.rootBase);
                        sqldata.ConnectionParameters = null;
                        sqldata.ConnectionParameters = fileConnectionParameters;
                    }
                }
            }
        }

#if !NET_STANDARD
        public XtraReport LoadReportDocument(DevExpress.Xpf.Reports.UserDesigner.IReportSerializer serializer)
        {
            if (ReportData != null)
            {
                using (var stream = new MemoryStream(ReportData))
                {
                    try
                    {
                        return serializer.Load(stream);
                    }
                    catch
                    { }
                }
            }

            return new ReportEmptyLayout();
        }

        public void SaveReportDocument(DevExpress.Xpf.Reports.UserDesigner.IReportSerializer serializer, XtraReport report)
        {
            if (reportDoc != null)
            {
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        serializer.Save(stream, report);
                        reportDoc.ReportData = stream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail("Invalid report layout ('{0}')", ex.Message);
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        report.SaveLayoutToXml(memoryStream);
                        reportDoc.ReportDataXML = memoryStream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail("Invalid report layout XML ('{0}')", ex.Message);
                    }
                }
            }
        }

        public void SaveReportDocument(byte[] layoutData, byte[] layoutDataXML, bool bSaveDocument = false)
        {
            if (reportDoc != null)
            {
                reportDoc.ReportData = layoutData;
                reportDoc.ReportDataXML = layoutDataXML;

                if (bSaveDocument)
                    reportDoc.SaveCurrentDocument();
            }
        }

        public void AddPlaceholderToConnectionString(XtraReport report)
        {
            for (int ii = 0; ii < report.ComponentStorage.Count; ii++)
            {
                if (report.ComponentStorage[ii] is DevExpress.DataAccess.Sql.SqlDataSource)
                {
                    var sqldata = report.ComponentStorage[ii] as DevExpress.DataAccess.Sql.SqlDataSource;
                    if (sqldata != null && sqldata.ConnectionParameters is DevExpress.DataAccess.ConnectionParameters.FileConnectionParametersBase)
                    {
                        var fileConnectionParameters = sqldata.ConnectionParameters as DevExpress.DataAccess.ConnectionParameters.FileConnectionParametersBase;
                        fileConnectionParameters.FileName = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(fileConnectionParameters.FileName, reportDoc?.rootBase);
                        sqldata.ConnectionParameters = null;
                        sqldata.ConnectionParameters = fileConnectionParameters;
                    }
                }
            }
        }

        public void SaveSubReportDocument(byte[] layoutDataXML, string subReportName)
        {
            if (reportDoc != null)
            {
                if (reportDoc.SubReportDataXML == null)
                    reportDoc.SubReportDataXML = new Dictionary<string, byte[]>();
                reportDoc.SubReportDataXML[subReportName] = layoutDataXML;
                reportDoc.NeedsSave = true;
                reportDoc.SaveCurrentDocument();
            }
        }

        public void CleanSubReportsMap(XtraReport report, bool isSubreport)
        {
            if (SubReportDataXML == null || SubReportDataXML.Keys.Count == 0)
                return;

            var subReportKeys = GetSubReportKeys(report);
            foreach (var key in SubReportDataXML.Keys.ToList())
            {
                if (isSubreport && !key.StartsWith(report.Name))
                    continue;

                if (!subReportKeys.Contains(key))
                    SubReportDataXML.Remove(key);
            }
        }

        List<String> GetSubReportKeys(XtraReport report)
        {
            var ret = new List<String>();
            foreach (Band band in report.Bands)
            {
                ret.AddRange(GetSubReportKeys(band));
            }
            return ret;
        }

        List<String> GetSubReportKeys(Band band)
        {
            var ret = new List<String>();
            foreach (XRControl control in band.Controls)
            {
                ret.AddRange(GetSubReportKeys(control));
            }

            return ret;
        }

        List<String> GetSubReportKeys(XRPanel band)
        {
            var ret = new List<String>();
            foreach (XRControl control in band.Controls)
            {
                ret.AddRange(GetSubReportKeys(control));
            }
            return ret;
        }

        List<String> GetSubReportKeys(XRControl control)
        {
            var ret = new List<String>();
            if (control is XRSubreport)
            {
                var subReport = control as XRSubreport;
                var key = GetUniqueControlKey(subReport, subReport.Name);
                ret.Add(key);
                if (subReport.ReportSource != null)
                    ret.AddRange(GetSubReportKeys(subReport.ReportSource));
            }
            else if (control is XRPanel)
            {
                var p = control as XRPanel;
                ret.AddRange(GetSubReportKeys(p));
            }
            else if (control is Band)
            {
                var b = control as Band;
                ret.AddRange(GetSubReportKeys(b));
            }
            return ret;
        }
#endif

        DataReaderModel CreateDataReaderModel()
        {
            DataReaderModel dataModel = null;
            if (ReportDocModel != null && !ReportDocModel.IsEmpty())
                dataModel = new DataReaderModel(ReportDocModel);
            else if ((defaultSourceType == DefaultSourceType.Historian || defaultSourceType == DefaultSourceType.Undefined) && 
                UFUAHistorianModel != null && !UFUAHistorianModel.IsEmpty())
                dataModel = new DataReaderModel(UFUAHistorianModel);
            else if ((defaultSourceType == DefaultSourceType.EventLog || defaultSourceType == DefaultSourceType.Undefined) && 
                UFUAEventLogModel != null && !UFUAEventLogModel.IsEmpty())
                dataModel = new DataReaderModel(UFUAEventLogModel);

            if (dataModel != null)
            {
                if (String.IsNullOrEmpty(dataModel.DataSourceName))
                {
                    dataModel.DataSourceName = DataSourceName;
                }
                if (String.IsNullOrEmpty(dataModel.DataProvider))
                {
                    dataModel.DataProvider = DataProviderName;
                }
                if (String.IsNullOrEmpty(dataModel.Connection))
                {
                    dataModel.Connection = DataConnection;
                }
            }

            return dataModel;
        }

        public void FillDataSource(ref DataSet dataset, bool onlyschema = false, IList<ReportParameters.Parameter> parameters = null)
        {
            var select = String.Empty;
            lastErrorInfo = String.Empty;
            invalidDataSource = false;
            var dataModel = CreateDataReaderModel();
            if (dataModel != null && dataModel.IsValid() &&
                !String.IsNullOrEmpty(dataModel.Select))
            {
                try
                {
                    using (var connection = DataReader.DataReader.CreateDbConnection(dataModel.DataProvider, dataModel.Connection))
                    {
                        connection.Open();
                        var dbdapater = DataReader.DataReader.CreateDbDataAdapter(dataModel.DataProvider);
                        dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(dataModel.DataProvider);
                        dbdapater.SelectCommand.Connection = connection;
                        if (CommandTimeout > 0)
                            dbdapater.SelectCommand.CommandTimeout = CommandTimeout;
                        if (!onlyschema && !string.IsNullOrEmpty(dataModel.Where))
                        {
                            if (MaxTake > 0)
                                dataModel.AddTopClause(MaxTake);
                            select = String.Format("{0} where {1}", dataModel.Select, dataModel.WhereClause());
                        }
                        else
                            select = dataModel.Select;
                        if (!string.IsNullOrEmpty(dataModel.GroupBy))
                            select = String.Format("{0} group by {1}", select, dataModel.GroupBy);
                        if (!onlyschema && !string.IsNullOrEmpty(dataModel.Sort))
                            select = String.Format("{0} order by {1}", select, dataModel.Sort);
                        dbdapater.SelectCommand.CommandText = select;

                        if (onlyschema)
                            dbdapater.FillSchema(dataset, SchemaType.Mapped);
                        else
                        {
                            if (parameters != null)
                            {
                                var customParameters = GetCustomParameters(parameters);
                                if (customParameters.Count > 0)
                                {
                                    using (var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(dataModel.DataProvider, dataModel.Connection))
                                    {
                                        foreach (var par in customParameters)
                                        {
                                            var selectParameterName = dbSchemaInfo.FormatParameterName(par.Name);
                                            if (!select.Contains(selectParameterName))
                                                continue;

                                            try
                                            {
                                                var dbparameter = DataReader.DataReader.CreateDbParameter(dataModel.DataProvider);
                                                dbparameter.ParameterName = par.Name;
                                                dbparameter.DbType = par.ToSystemType().ToDbType();
                                                dbparameter.Value = Convert.ChangeType(par.Value, par.ToSystemType());
                                                dbdapater.SelectCommand.Parameters.Add(dbparameter);
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(String.Format("Error setting the data adapter parameter '{0}' : {1}", par.Name, ex.Message));
                                            }
                                        }
                                    }
                                }
                            }

                            dbdapater.Fill(dataset);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!String.IsNullOrEmpty(select))
                        lastErrorInfo = String.Format(Properties.Resources.ErrorInfoOnSelect, select, ex.Message);
                    else
                        lastErrorInfo = String.Format(Properties.Resources.ErrorInfoOnConnection, dataModel.DataProvider, dataModel.Connection, ex.Message);

                    invalidDataSource = true;

                    if (throwOnError)
                        throw ex;
                }
            }
        }

        IList<ReportParameters.Parameter> GetCustomParameters(IList<ReportParameters.Parameter> parameters)
        {
            return (from p in parameters
                    where p.Name != DefaultConnectionParameter &&
                    p.Name != UserNameParameter select p).ToList();
        }
#endregion
    }
}
