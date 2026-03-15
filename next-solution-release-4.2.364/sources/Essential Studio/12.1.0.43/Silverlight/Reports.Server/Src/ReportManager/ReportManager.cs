#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlServerCe;
using Syncfusion.ReportWriter;
using System.Threading;
using System.Data.OracleClient;
using Syncfusion.Reports.Server.Data;
using Syncfusion.ReportWriter;
using Syncfusion.Windows.Reports.DataProcessor;
using System.Data.Odbc;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Syncfusion.Reports.Server
{
    [DataContract]
    public class ReportManager
    {
        private ServerReportProcessor GetServerProcessor(ReportSetting setting)
        {
            ServerReportProcessor reportService = new ServerReportProcessor();
            reportService.ReportPath = setting.ReportPath;
            reportService.ReportServerUrl = setting.ReportServerURL;

            if (setting.ReportServerCredential != null)
            {
                reportService.ReportServerCredential = this.GetNetworkCredential(setting.ReportServerCredential);
            }
            else
            {
                reportService.ReportServerFormsCredential = new Windows.Reports.ReportServerFormsCredential(setting.ReportServerFormCredential.UserName, setting.ReportServerFormCredential.Password);
            }

            return reportService;
        }

        public List<ServiceDataSourceDefinition> Datasources
        {
            get;
            set;
        }

        public SharedDatasetinfo GetSharedDataSet(ReportSetting setting, string dataset)
        {
            SharedDatasetinfo info = null;
            try
            {
                ServerReportProcessor reportService = this.GetServerProcessor(setting);
                info = reportService.GetSharedDatadefintion(dataset);
                ServiceDataSourceDefinition data = new ServiceDataSourceDefinition();
                data.Name = info.DataSource.Name;
                data.ConnectionString = info.DataSource.ConnectString;
                data.AutenticationInfo = info.DataSource.CredentialRetrieval.ToString();
                data.Provider = info.DataSource.Extension;
                info.DataSourceDefinition = data;
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public byte[] GetReport(ReportSetting setting)
        {
            if (!string.IsNullOrEmpty(setting.ReportPath) && !string.IsNullOrEmpty(setting.ReportServerURL))
            {
                ServerReportProcessor reportService = this.GetServerProcessor(setting);

                try
                {
                    byte[] report = reportService.GetReportDefinition();
                    return report;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    if (File.Exists(setting.ReportPath))
                    {
                        using (FileStream stream = new FileStream(setting.ReportPath, FileMode.Open))
                        {
                            BinaryReader bin = new BinaryReader(stream);
                            return bin.ReadBytes(Convert.ToInt32(stream.Length));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return null;
        }

        public RecordInfo GetDataSource(ReportSetting setting , ReportDataInfo ReportInfo)
        {
            DataSource dataSource = ReportInfo.DataSource;
            DataTable dataTable = null;

            if (ReportInfo.DataSource.DataSetRefernce != null)
            {
                 this.UpdateSharedDataSet(this.GetSharedDataSet(setting, ReportInfo.DataSource.DataSetRefernce), ReportInfo);
            }

            if (ReportInfo.DataSource.ConnectionProperties.ConnectionString == null)
            {
                if (this.Datasources == null)
                {
                    this.Datasources = new List<ServiceDataSourceDefinition>();
                }

                foreach (string datasource in ReportInfo.DataSource.DataSourceReference)
                {
                    this.Datasources.Add(this.GetDataSourceDefinition(setting, datasource));
                }

                var datasourcedefintion = (from datasourcedef in this.Datasources where datasourcedef.Name == ReportInfo.DataSource.Name select datasourcedef).FirstOrDefault();

                ReportInfo.DataSource.ConnectionProperties.ConnectionString = datasourcedefintion.ConnectionString;
                ReportInfo.DataSource.ConnectionProperties.DataProvider = datasourcedefintion.Provider;
                switch (datasourcedefintion.AutenticationInfo)
                {
                    case "Integrated":
                        ReportInfo.DataSource.ConnectionProperties.IntegratedSecurity = true;
                        break;
                    case "Prompt":
                        ReportInfo.DataSource.ConnectionProperties.Prompt = "Prompt";
                        break;
                }

            }


            try
            {
                if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLServer, StringComparison.InvariantCultureIgnoreCase)
                    || (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLAzure, StringComparison.InvariantCultureIgnoreCase)))
                {
                    dataTable=this.GetSQLData(dataSource);
                }

                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.OLEDB, StringComparison.InvariantCultureIgnoreCase))
                {
                    dataTable = this.GetOLEDBData(dataSource);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLServerCe, StringComparison.InvariantCultureIgnoreCase))
                {
                    dataTable = this.GetSQLCeData(dataSource);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.ORACLE, StringComparison.InvariantCultureIgnoreCase))
                {
                    dataTable = this.GetOracleData(dataSource);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.ODBC, StringComparison.InvariantCultureIgnoreCase))
                {
                    dataTable = this.GetODBCData(dataSource);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.XML, StringComparison.InvariantCultureIgnoreCase))
                {
                    dataTable = this.GetXMLData(dataSource);
                }

                foreach (var filed in dataSource.ExpressionFields)
                {
                    System.Data.DataColumn expressionColumn = new System.Data.DataColumn();
                    expressionColumn.ColumnName = filed.ColumnName;
                    expressionColumn.Expression = filed.Expression;
                    dataTable.Columns.Add(expressionColumn);
                    if (dataTable.Rows.Count > 0)
                    {
                        double value = 0;
                        if (double.TryParse(dataTable.Rows[0][filed.ColumnName].ToString(), out value))
                        {
                            dataTable.Columns.Remove(expressionColumn);
                            expressionColumn.DataType = typeof(double);
                            dataTable.Columns.Add(expressionColumn);
                        }
                    }
                }
            }
            catch
            {
            }

            if (dataTable != null)
            {
                return GetWrapperData(dataTable);
            }
            else
            {
                return null;
            }
        }

        void UpdateSharedDataSet(SharedDatasetinfo info,ReportDataInfo datasetinfo)
        {
            Stream stream = new MemoryStream(info.DataSetStream);
            using (XmlReader reader = XmlReader.Create(stream))
            {
                reader.MoveToContent();
                string Namespace = reader.NamespaceURI;
                string Version = (Regex.IsMatch(Namespace, @"\d{4}") ? Regex.Match(Namespace, @"\d{4}").Value : string.Empty);
                if (!string.IsNullOrEmpty(Version))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Syncfusion.RDL.Data.SharedDataSet), Namespace);
                    Syncfusion.RDL.Data.SharedDataSet dataset = (Syncfusion.RDL.Data.SharedDataSet)xs.Deserialize(reader);
                    datasetinfo.DataSource.CommandText = dataset.DataSet.Query.CommandText;
                    datasetinfo.DataSource.ConnectionProperties = new ConnectionProperties();
                    datasetinfo.DataSource.ConnectionProperties.ConnectionString = info.DataSourceDefinition.ConnectionString;
                    datasetinfo.DataSource.ConnectionProperties.DataProvider = info.DataSourceDefinition.Provider;
                    switch (info.DataSourceDefinition.AutenticationInfo)
                    {
                        case "Integrated":
                            datasetinfo.DataSource.ConnectionProperties.IntegratedSecurity = true;
                            break;
                        case "Prompt":
                            datasetinfo.DataSource.ConnectionProperties.Prompt = "Prompt";
                            break;
                    }

                }
            }
        }


        DataTable GetSQLData(DataSource dataSource)
        {
            try
            {
                string connectionString = dataSource.ConnectionProperties.ConnectionString;
                if (dataSource.ConnectionProperties.IntegratedSecurity == true)
                {
                    connectionString = dataSource.ConnectionProperties.ConnectionString + ";Trusted_Connection=true;";
                }
                SqlCommand command = new SqlCommand(dataSource.CommandText);

                foreach (var QP in dataSource.QueryParameters)
                {
                    command.Parameters.AddWithValue(QP.Name, QP.Value);
                }
                return new Syncfusion.Reports.Server.Data.SqlDataProvider().GetTable(connectionString, command, dataSource.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         DataTable GetSQLCeData(DataSource dataSource)
         {
             try
             {
                 string connectionString = dataSource.ConnectionProperties.ConnectionString;
                 connectionString = dataSource.ConnectionProperties.ConnectionString;
                 SqlCeCommand command = new SqlCeCommand(dataSource.CommandText);

                 foreach (var QP in dataSource.QueryParameters)
                 {
                     command.Parameters.AddWithValue(QP.Name, QP.Value);
                 }

                 return new Syncfusion.Reports.Server.Data.SqlServerCeDataProvider().GetTable(connectionString, command,dataSource.Name);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }

         DataTable GetOracleData(DataSource dataSource)
        {
             try
             {
                 string connectionString = dataSource.ConnectionProperties.ConnectionString;
                 connectionString = dataSource.ConnectionProperties.ConnectionString;
                 OracleCommand command = new OracleCommand(dataSource.CommandText);
                 foreach (var QP in dataSource.QueryParameters)
                 {
                     command.Parameters.AddWithValue(QP.Name, QP.Value);
                 }
                 return new Syncfusion.Reports.Server.Data.OracleDataProvider().GetTable(connectionString, command,dataSource.Name);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
        }

        DataTable GetOLEDBData(DataSource dataSource)
        {
            try
            {
                string connectionString = dataSource.ConnectionProperties.ConnectionString;
                connectionString = dataSource.ConnectionProperties.ConnectionString;
                OleDbCommand command = new OleDbCommand(dataSource.CommandText);

                foreach (var QP in dataSource.QueryParameters)
                {
                    command.Parameters.AddWithValue(QP.Name, QP.Value);
                }
                return new Syncfusion.Reports.Server.Data.OleDbDataProvider().GetTable(connectionString, command,dataSource.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        DataTable GetODBCData(DataSource dataSource)
        {
            try
            {
                string connectionString = dataSource.ConnectionProperties.ConnectionString;
                connectionString = dataSource.ConnectionProperties.ConnectionString;
                OdbcCommand command = new OdbcCommand(dataSource.CommandText);

                foreach (var QP in dataSource.QueryParameters)
                {
                    command.Parameters.AddWithValue(QP.Name, QP.Value);
                }
                return new Syncfusion.Reports.Server.Data.OdbcDataProvider().GetTable(connectionString, command,dataSource.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        DataTable GetXMLData(DataSource dataSource)
        {
            try
            {
                string connectionString = dataSource.ConnectionProperties.ConnectionString;
                connectionString = dataSource.ConnectionProperties.ConnectionString;
                return new Syncfusion.Reports.Server.Data.XMLDataProvider().GetTable(dataSource.CommandText,dataSource.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ServiceDataSourceDefinition GetDataSourceDefinition(ReportSetting setting, string dataSource)
        {
            ServerReportProcessor reportService = this.GetServerProcessor(setting);
            ServiceDataSourceDefinition data = new ServiceDataSourceDefinition();

            try
            {
                var dataSourceDefinition = reportService.GetDataSourceDefinition(dataSource);
                data.Name = dataSourceDefinition.Name;
                data.ConnectionString = dataSourceDefinition.ConnectString;
                data.AutenticationInfo = dataSourceDefinition.CredentialRetrieval.ToString();
                data.Provider = dataSourceDefinition.Extension;
                return data;
            }
            catch (Exception ex)
            {
                data.Exception = ex.Message;
                return data;
            }
        }


        internal ExportData Export(ReportSetting reportSetting, string exportType)
        {
            ExportData data = new ExportData();

            try
            {
                WriterFormat writerFormat = (WriterFormat)Enum.Parse(typeof(WriterFormat), exportType, true);

                if (!string.IsNullOrEmpty(exportType))
                {
                    Syncfusion.ReportWriter.ReportWriter reportWriter = new Syncfusion.ReportWriter.ReportWriter();
                    MemoryStream stream = null;

                    Thread thread = new Thread(delegate()
                    {
                        reportWriter = new ReportWriter.ReportWriter();
                        reportWriter.ReportPath = reportSetting.ReportPath;
                        if (reportSetting.PageSettingsInfo != null)
                        {
                            reportWriter.PageSettings = new Syncfusion.ReportWriter.PageSettings
                            {
                                PageHeight = reportSetting.PageSettingsInfo.PageHeight,
                                PageWidth = reportSetting.PageSettingsInfo.PageWidth,
                                BottomMargin = reportSetting.PageSettingsInfo.BottomMargin,
                                LeftMargin = reportSetting.PageSettingsInfo.LeftMargin,
                                RightMargin = reportSetting.PageSettingsInfo.RightMargin,
                                TopMargin = reportSetting.PageSettingsInfo.TopMargin
                            };
                        }
                        
                        if (reportSetting.DataSources == null)
                        {
                            reportWriter.ReportProcessingMode = ProcessingMode.Remote;
                            reportWriter.ReportServerUrl = reportSetting.ReportServerURL;

                            if (reportSetting.ReportServerCredential != null)
                            {
                                reportWriter.ReportServerCredential = this.GetNetworkCredential(reportSetting.ReportServerCredential);
                            }
                            else if (reportSetting.ReportServerFormCredential != null)
                            {
                                string userName = reportSetting.ReportServerFormCredential.UserName;
                                string passWord = reportSetting.ReportServerFormCredential.Password;
                                reportWriter.ReportServerFormsCredential = new Windows.Reports.ReportServerFormsCredential(userName, passWord);
                            }
                        }
                        else
                        {
                            reportWriter.ReportProcessingMode = ProcessingMode.Local;
                        }

                        if (string.IsNullOrEmpty(reportSetting.ReportPath))
                        {
                            MemoryStream fileStream = new MemoryStream(reportSetting.Report);
                            reportWriter.LoadReport(fileStream);
                        }

                        List<Syncfusion.Windows.Reports.ReportParameter> parameters = new List<Syncfusion.Windows.Reports.ReportParameter>();

                        if (reportSetting.Parameters != null)
                        {
                            foreach (ReportParameterInfo parameter in reportSetting.Parameters)
                            {
                                Syncfusion.Windows.Reports.ReportParameter param = new Syncfusion.Windows.Reports.ReportParameter();
                                param.Name = parameter.Name;

                                foreach (string label in parameter.Labels)
                                {
                                    param.Labels.Add(label);
                                }

                                foreach (string value in parameter.Values)
                                {
                                    param.Values.Add(value);
                                }

                                parameters.Add(param);
                            }

                            reportWriter.SetParameters(parameters);
                        }

                        if (reportWriter.ReportProcessingMode == ReportWriter.ProcessingMode.Remote)
                        {
                            List<Syncfusion.Windows.Reports.DataSourceCredentials> credentialCollection = new List<Syncfusion.Windows.Reports.DataSourceCredentials>();

                            foreach (DataSourceCredentialsInfo parameter in reportSetting.DataSourceCredentials)
                            {
                                Syncfusion.Windows.Reports.DataSourceCredentials dataSourceCred = new Windows.Reports.DataSourceCredentials();
                                dataSourceCred.Name = parameter.Name;
                                dataSourceCred.UserId = parameter.UserId;
                                dataSourceCred.Password = parameter.Password;
                                dataSourceCred.IntegratedSecurity = parameter.IntegratedSecurity;
                                credentialCollection.Add(dataSourceCred);
                            }

                            reportWriter.SetDataSourceCredentials(credentialCollection);
                        }
                        else
                        {

                            reportWriter.DataSources = new Windows.Reports.ReportDataSourceCollection();
                            foreach (ReportDataSource dataSource in reportSetting.DataSources)
                            {
                                Syncfusion.Windows.Reports.ReportDataSource dataSour = new Windows.Reports.ReportDataSource();
                                dataSour.Name = dataSource.Name;
                                dataSour.Value = this.GetWrapperData(dataSource.Value);
                                reportWriter.DataSources.Add(dataSour);
                            }
                        }

                        stream = new MemoryStream();
                        reportWriter.Save(stream, writerFormat);
                    });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();

                    data.Export = stream.ToArray();
                }
            }

            catch(Exception e)
            {
                data.Exception = e.Message;
            }

            return data;
        }

        System.Collections.IEnumerable GetWrapperData(List<ReportData> dataList)
        {
            List<Syncfusion.RDL.Data.ReportData> datas = new List<RDL.Data.ReportData>();

            foreach (var data in dataList)
            {
                Syncfusion.RDL.Data.ReportData reportData = new RDL.Data.ReportData();
                reportData.Data = new Dictionary<string, object>();

                foreach (string key in data.Data.Keys)
                {
                    reportData.Data.Add(key, data.Data[key]);
                }
                datas.Add(reportData);
            }

            return datas;
        }


        RecordInfo GetWrapperData(DataTable table)
        {
            RecordInfo field = new RecordInfo();
            field.Fieldname = new List<string>();
            field.FieldType = new List<string>();
            field.ReportItems = new List<ReportDatas>();

            DataTable dt = table;

            for (int col = 0; col < dt.Columns.Count; col++)
            {
                string temp = dt.Columns[col].DataType.FullName.ToString();
                if (temp == "System.Int16" || temp == "System.Int32" || temp == "System.Int64")
                {
                    temp = "System.Int";
                }
                field.FieldType.Add(temp);
                field.Fieldname.Add(dt.Columns[col].ColumnName.ToString());
            }

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                ReportDatas data = new ReportDatas();
                data.Data = new List<DataField>();
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    if (dt.Rows[row][col] != System.DBNull.Value && dt.Rows[row][col]!=null)
                    {
                        data.Data.Add(new DataField() { FieldName = dt.Columns[col].ColumnName, Value = dt.Rows[row][col].ToString() });
                    }
                    else
                    {
                        data.Data.Add(new DataField() { FieldName = dt.Columns[col].ColumnName, Value = null });
                    }
                }
                field.ReportItems.Add(data);
            }
            return field;
        }

        NetworkCredential GetNetworkCredential(ReportServerCredential serverCredential)
        {
            NetworkCredential credential = null;

            if (serverCredential != null && serverCredential.Domain == null)
            {
                credential = new NetworkCredential(serverCredential.UserName, serverCredential.Password);
            }
            else if (serverCredential != null)
            {
                credential = new NetworkCredential(serverCredential.UserName, serverCredential.Password, serverCredential.Domain);
            }

            return credential;
        }

        internal bool IsValidConnection(string connectionString, string dataProvider)
        {
            bool isValidConnection = false;

            if (dataProvider.Equals(DataProviders.SQLServer, StringComparison.InvariantCultureIgnoreCase)
                || dataProvider.Equals(DataProviders.SQLAzure, StringComparison.InvariantCultureIgnoreCase))
            {
                isValidConnection = new SqlDataProvider().ChecksWhetherValidConnection(connectionString);
            }
            else if (dataProvider.Equals(DataProviders.ORACLE, StringComparison.InvariantCultureIgnoreCase))
            {
                isValidConnection = new OracleDataProvider().ChecksWhetherValidConnection(connectionString);
            }

            return isValidConnection;
        }
    }


    /// <summary>
    /// A Class Provides the DataProviders List
    /// </summary>
    class DataProviders
    {
        /// <summary>
        /// Constants stands for Providers List
        /// </summary>
        #region Constants

        /// <summary>
        /// Constant string determines Microsoft SQL Server Database
        /// </summary>
        public const string SQLServer = "SQL";

        /// <summary>
        /// Constant string determines Microsoft SQL Server Database
        /// </summary>
        public const string SQLAzure = "SQLAZURE";

        /// <summary>
        /// Constant string determines Oracle Database
        /// </summary>
        public const string ORACLE = "ORACLE";

        /// <summary>
        /// Constant string determines MySql Database
        /// </summary>
        public const string MySQL = "MySQL";

        /// <summary>
        /// Constant string determines OLEDB Connection
        /// </summary>
        public const string OLEDB = "OLEDB";

        /// <summary>
        /// Constant string determines SQLServer Service Analysis
        /// </summary>
        public const string SQLServerAnalysisService = "OLEDB-MD";

        /// <summary>
        /// Constant string determines ODBC Connection
        /// </summary>
        public const string ODBC = "ODBC";

        /// <summary>
        /// Constant string determines XML files
        /// </summary>
        public const string XML = "XML";

        /// <summary>
        /// Constant string determines SAP NetWeaverBI
        /// </summary>
        public const string SAPNetWeaverBI = "SAP NetWeaverBI";

        /// <summary>
        /// Constant string determines Hypersion Essbase Database
        /// </summary>
        public const string HyperionEssbase = "Hyperion Essbase";

        /// <summary>
        /// Constant string determines TERADATA
        /// </summary>
        public const string TERADATA = "TERADATA";

        /// <summary>
        /// Constant string determines SQLServerCe Compact Edition Database.
        /// </summary>
        public const string SQLServerCe = "SQLCe";

        #endregion
    }

    enum ExportType
    {
        PDF
    }

}