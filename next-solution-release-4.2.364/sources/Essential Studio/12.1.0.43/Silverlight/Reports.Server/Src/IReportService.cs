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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;
using System.Data.SqlServerCe;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using System.Collections;

namespace Syncfusion.Reports.Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IReportService
    {
        [OperationContract]
        byte[] GetReport(ReportSetting setting);

        [OperationContract]
        SharedDatasetinfo GetSharedDataSet(ReportSetting setting,string path);

        [OperationContract]
        RecordInfo GetData(ReportSetting setting, ReportDataInfo ReportInfo);

        [OperationContract]
        ServiceDataSourceDefinition GetSharedDataSourceDefinition(ReportSetting setting, string dataSource);

        [OperationContract]
        [STAThread]
        ExportData Export(ReportSetting setting, string exportType);

        [OperationContract]
        bool IsValidConnection(string connectionString, string dataProvider);
    }

    [DataContract]
    public class ServiceDataSource
    {
        [DataMember]
        public string DataSourceName { get; set; }

        [DataMember]
        public IList DataSource { get; set; }
    }

    [DataContract]
    public class ExportData
    {
        [DataMember]
        public byte[] Export { get; set; }

        [DataMember]
        public string Exception { get; set; }
    }


    [DataContract]
    public class RecordInfo
    {
        [DataMember]
        public List<string> Fieldname { get; set; }

        [DataMember]
        public List<string> FieldType { get; set; }

        [DataMember]
        public List<ReportDatas> ReportItems { get; set; }
    }

    [DataContract]
    public class ReportDatas
    {
        [DataMember]
        public List<DataField> Data { get; set; }
    }

    [DataContract]
    public class DataField
    {
        [DataMember]
        public string FieldName { get; set; }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract]
    public class ReportData
    {
        [DataMember]
        public Dictionary<string, object> Data { get; set; }
    }

    [DataContract]
    public class ServiceDataSourceDefinition
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public string Provider { get; set; }


        [DataMember]
        public string AutenticationInfo
        {
            get;
            set;
        }

        [DataMember]
        public string Exception
        {
            get;
            set;
        }

    }

    [DataContract]
    public class ReportParameterInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string[] Labels { get; set; }

        [DataMember]
        public string[] Values { get; set; }
    }

    [DataContract]
    public sealed class DataSourceCredentialsInfo
    {
        [DataMember]
        public bool IntegratedSecurity { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }

    [DataContract]
    public class ReportDataSource
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public List<ReportData> Value
        {
            get;
            set;
        }
    }

    [DataContract]
    public class SharedDatasetinfo
    {
        [DataMember]
        public byte[] DataSetStream
        {
            get;
            set;
        }

        [DataMember]
        public Syncfusion.Windows.Reports.DataProcessor.DataSourceDefinition DataSource
        {
            get;
            set;
        }

        [DataMember]
        public ServiceDataSourceDefinition DataSourceDefinition
        {
            get;
            set;
        }

        [DataMember]
        public string Exception
        {
            get;
            set;
        }
  
    }

    [DataContract]
    public class ReportSetting
    {
        string _ReportServerURL;
        string _ReportPath;
        bool _LoadInformationfromServer;
        byte[] _Report;

        [DataMember]
        public bool LoadInformationfromServer
        {
            get { return _LoadInformationfromServer; }
            set { _LoadInformationfromServer = value; }
        }

        [DataMember]
        public string ReportServerURL
        {
            get { return _ReportServerURL; }
            set { _ReportServerURL = value; }
        }

        [DataMember]
        public string ReportPath
        {
            get { return _ReportPath; }
            set { _ReportPath = value; }
        }

        [DataMember]
        public byte[] Report
        {
            get { return _Report; }
            set { _Report = value; }
        }

        [DataMember]
        public ReportDataSource[] DataSources
        {
            get;
            set;
        }

        [DataMember]
        public ReportServerCredential ReportServerCredential { get; set; }

        [DataMember]
        public ReportServerFormCredential ReportServerFormCredential { get; set; }

        [DataMember]
        public ReportParameterInfo[] Parameters { get; set; }

        [DataMember]
        public DataSourceCredentialsInfo[] DataSourceCredentials { get; set; }

        [DataMember]
        public PageSettings PageSettingsInfo { get; set; }
    }

    [DataContract]
    public class ReportServerCredential
    {
        string _username;
        string _password;
        string _domain;

        [DataMember]
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        [DataMember]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        [DataMember]
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }
    }

    [DataContract]
    public class ReportServerFormCredential
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Authority { get; set; }

        public ReportServerFormCredential(string user, string Password, string authority)
        {
            this.UserName = user;
            this.Password = Password;
            this.Authority = authority;
        }
    }

    [DataContract]
    public class ReportDataInfo
    {
        [DataMember]
        public DataSource DataSource { get; set; }
    }

    [DataContract]
    public class DataSource
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ConnectionProperties ConnectionProperties { get; set; }

        [DataMember]
        public IList<string> DataSourceReference { get; set; }

        [DataMember]
        public string DataSetRefernce { get; set; }

        [DataMember]
        public string CommandText { get; set; }

        [DataMember]
        public IList<QueryReportParameter> QueryParameters { get; set; }

        [DataMember]
        public IList<ExpressionField> ExpressionFields { get; set; }
    }

    [DataContract]
    public class ConnectionProperties
    {
        [DataMember]
        public string DataProvider { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public bool IntegratedSecurity { get; set; }

        [DataMember]
        public string Prompt { get; set; }
    }

    [DataContract]
    public class QueryReportParameter
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object Value { get; set; }
    }

    [DataContract]
    public class ExpressionField
    {
        [DataMember]
        public string ColumnName { get; set; }
        [DataMember]
        public string Expression { get; set; }
    }

    [DataContract]
    public class PageSettings
    {
        [DataMember]
        public double PageWidth { get; set; }
        [DataMember]
        public double PageHeight { get; set; }
        [DataMember]
        public double TopMargin { get; set; }
        [DataMember]
        public double LeftMargin { get; set; }
        [DataMember]
        public double RightMargin { get; set; }
        [DataMember]
        public double BottomMargin { get; set; }


    }

}


