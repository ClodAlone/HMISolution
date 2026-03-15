#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.SqlServer.ReportingServices2005;
using Microsoft.SqlServer.ReportingServices2006;
using Microsoft.SqlServer.ReportingServices2010;
using System.Web.Services.Protocols;
using System.Text.RegularExpressions;
using Syncfusion.RDL.Data;

#if MVC
using Syncfusion.Reports.Mvc;
#else
using Syncfusion.Windows.Reports;
#endif

namespace Syncfusion.RDL.ServerProcessor
{
    internal interface IReportService
    {
        bool SetReportDefinition(byte[] reportData);

        DataSourceDefinition GetDataSourceDefinition(string dataSource);

        byte[] GetReportDefinition();

        SharedDatasetinfo GetSharedDataSet(string path);

        bool IsExecuted { get; set; }
    }

    internal abstract class ReportingServer
    {
        internal bool IsServerReport { get; set; }

        internal string ReportServerUrl { get; set; }

        internal ICredentials ReportServerCredential { get; set; }

        internal ReportServerFormsCredential ReportServerFormsCredential { get; set; }

        internal string ReportPath { get; set; }

        internal bool PreAuthenticate { get; set; }
    }

    internal class ServerReportProcessor : ReportingServer
    {
        private ReportModel ReportModel { get; set; }

        private ReportService2005 Service2005 { get; set; }

        private ReportService2006 Service2006 { get; set; }

        private ReportService2010 Service2010 { get; set; }

        public ServerReportProcessor()
        {
            this.Service2005 = new ReportService2005(this);
            this.Service2006 = new ReportService2006(this);
            this.Service2010 = new ReportService2010(this);
        }

        public ServerReportProcessor(ReportModel reportModel)
        {
            this.Service2005 = new ReportService2005(this);
            this.Service2006 = new ReportService2006(this);
            this.Service2010 = new ReportService2010(this);
            this.ReportModel = reportModel;
        }

        bool IsSharePointReport()
        {
            return !string.IsNullOrEmpty(this.ReportPath) && this.ReportPath.Trim().StartsWith("http");
        }

        bool IsSharePointFolder(string folderName)
        {
            return folderName.Trim().StartsWith("http");
        }

        private void SetReportingServer()
        {
            if (this.ReportModel != null)
            {
                this.ReportPath = this.ReportModel.ReportPath;
                this.ReportServerCredential = this.ReportModel.ReportServerCredential;
                this.ReportServerFormsCredential = this.ReportModel.ReportServerFormsCredential;
                this.ReportServerUrl = this.ReportModel.ReportServerUrl;
                this.IsServerReport = this.ReportModel.IsServerReport;
                this.PreAuthenticate = this.ReportModel.PreAuthenticate;
            }
        }

        public SharedDatasetinfo GetSharedDatadefintion(string path)
        {
            return this.Service2010.GetSharedDataSet(path);
        }

        public Stream GetReportDefinition(out string exception)
        {
            this.SetReportingServer();
            Byte[] reportBinaryContents = null;
            exception = null;

            if (this.IsSharePointReport())
            {
                try
                {
                    reportBinaryContents = this.Service2006.GetReportDefinition();
                    exception = null;
                }
                catch (Exception ex)
                {
                    reportBinaryContents = null;
                    exception = ex.ToString();
                }

                if (!this.Service2006.IsExecuted)
                {
                    try
                    {
                        reportBinaryContents = this.Service2010.GetReportDefinition();
                        exception = null;
                    }
                    catch (Exception ex)
                    {
                        reportBinaryContents = null;
                        exception = ex.ToString();
                    }
                }
            }
            else
            {
                try
                {
                    reportBinaryContents = this.Service2005.GetReportDefinition();
                    exception = null;
                }
                catch (Exception ex)
                {
                    reportBinaryContents = null;
                    exception = ex.ToString();
                }

                if (!this.Service2005.IsExecuted)
                {
                    try
                    {
                        reportBinaryContents = this.Service2010.GetReportDefinition();
                        exception = null;
                    }
                    catch (Exception ex)
                    {
                        reportBinaryContents = null;
                        exception = ex.ToString();
                    }
                }
            }

            if (reportBinaryContents != null)
            {
                MemoryStream reportStream = new MemoryStream(reportBinaryContents);
                return reportStream;
            }

            return null;
        }

        public bool SetReportDefinition(byte[] reportData)
        {
            this.SetReportingServer();
            bool isStored = false;

            if (this.IsSharePointReport())
            {
                isStored = this.Service2006.SetReportDefinition(reportData);

                if (!this.Service2006.IsExecuted)
                {
                    isStored = this.Service2010.SetReportDefinition(reportData);
                }
            }
            else
            {
                isStored = this.Service2005.SetReportDefinition(reportData);

                if (!this.Service2005.IsExecuted)
                {
                    isStored = this.Service2010.SetReportDefinition(reportData);
                }
            }

            return isStored;
        }

        public DataSourceDefinition GetDataSourceDefinition(string dataSource)
        {
            this.SetReportingServer();

            DataSourceDefinition dataSourceDefinition = null;

            if (this.IsSharePointReport() || dataSource.StartsWith("http"))
            {
                dataSourceDefinition = this.Service2006.GetDataSourceDefinition(dataSource);

                if (!this.Service2006.IsExecuted)
                {
                    dataSourceDefinition = this.Service2010.GetDataSourceDefinition(dataSource);
                }
            }
            else
            {
                dataSourceDefinition = this.Service2005.GetDataSourceDefinition(dataSource);

                if (!this.Service2005.IsExecuted)
                {
                    dataSourceDefinition = this.Service2010.GetDataSourceDefinition(dataSource);
                }
            }

            return dataSourceDefinition;
        }

        public bool CreateReport(string reportname, string folderName, byte[] reportdata)
        {
            this.SetReportingServer();

            bool isStored = false;

            if (this.IsSharePointFolder(folderName))
            {
                isStored = this.Service2005.CreateReportDefinition(reportname, folderName, reportdata);

                if (!this.Service2005.IsExecuted)
                {
                    isStored = this.Service2010.CreateReportDefinition(reportname, folderName, reportdata);
                }
            }
            else
            {
                isStored = this.Service2005.CreateReportDefinition(reportname, folderName, reportdata);

                if (!this.Service2005.IsExecuted)
                {
                    isStored = this.Service2010.CreateReportDefinition(reportname, folderName, reportdata);
                }
            }

            return isStored;
        }

        public bool GetCatalogItem(string folderName,out List<CatalogItem> items)
        {
            List<CatalogItem> outputItem = null;

            this.SetReportingServer();

            bool isExecuted = false;

            if (this.IsSharePointFolder(folderName))
            {
                outputItem = this.Service2006.GetCatalogItem(folderName);
                isExecuted = this.Service2006.IsExecuted;
                if (!this.Service2006.IsExecuted)
                {
                    outputItem = this.Service2010.GetCatalogItem(folderName);
                    isExecuted = this.Service2010.IsExecuted;
                }
            }
            else
            {
                outputItem = this.Service2005.GetCatalogItem(folderName);
                isExecuted = this.Service2005.IsExecuted;

                if (!this.Service2005.IsExecuted)
                {
                    outputItem = this.Service2010.GetCatalogItem(folderName);
                    isExecuted = this.Service2010.IsExecuted;
                }
            }

            items = outputItem;
            return isExecuted;
        }
    }

    internal class ReportService2005 : ReportingServer, IReportService
    {
        private ReportingServer reportingServer;

        private ReportingService2005 ServiceProxy { get; set; }

        public bool IsExecuted { get; set; }

        public ReportService2005(ReportingServer reportingServer)
        {
            ServiceProxy = new ReportingService2005();
            this.reportingServer = reportingServer;
        }

        private void SetReportingServer()
        {
            this.ReportPath = this.reportingServer.ReportPath;
            this.ReportServerCredential = this.reportingServer.ReportServerCredential;
            this.ReportServerFormsCredential = this.reportingServer.ReportServerFormsCredential;
            this.ReportServerUrl = this.reportingServer.ReportServerUrl;
            this.IsServerReport = this.reportingServer.IsServerReport;
            this.PreAuthenticate = this.reportingServer.PreAuthenticate;


            ServiceProxy.Url = this.ReportServerUrl.GetServiceUrl("2005");
            this.ServiceProxy.PreAuthenticate = this.PreAuthenticate;
            if(this.ReportServerFormsCredential != null)
            {
                this.ServiceProxy.CookieContainer = new CookieContainer();
                string userName = this.ReportServerFormsCredential.UserName;
                string passWord = this.ReportServerFormsCredential.Password;
                this.ServiceProxy.LogonUser(userName, passWord, this.ReportServerUrl.GetAuthortiy());
            }
            else
            {
                ServiceProxy.Credentials = this.ReportServerCredential;
            }
        }

        private void CloseReportingServer()
        {
            if (this.ReportServerFormsCredential != null)
            {
                this.ServiceProxy.Logoff();
                this.ServiceProxy.CookieContainer = null;
            }
        }

        public bool SetReportDefinition(byte[] reportData)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                ServiceProxy.SetReportDefinition(this.ReportPath, reportData);
                this.IsExecuted = true;
                this.CloseReportingServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateReportDefinition(string reportname,string folderName, byte[] reportData)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                // Set report properties
                Microsoft.SqlServer.ReportingServices2005.Property reportProperty = new Microsoft.SqlServer.ReportingServices2005.Property();
                reportProperty.Name = "Description";
                reportProperty.Value = reportname; // May want to prompt for this
                Microsoft.SqlServer.ReportingServices2005.Property[] reportProperties = new Microsoft.SqlServer.ReportingServices2005.Property[1];
                reportProperties[0] = reportProperty;
                ServiceProxy.CreateReport(reportname, folderName, true, reportData, reportProperties);
                this.IsExecuted = true;
                this.CloseReportingServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<CatalogItem> GetCatalogItem(string folderName)
        {
            this.IsExecuted = false;
            this.SetReportingServer();
            try
            {
                Microsoft.SqlServer.ReportingServices2005.CatalogItem[] items = ServiceProxy.ListChildren(folderName, false);
                
                List<CatalogItem> reportItems = new List<CatalogItem>();

                foreach (var item in items)
                {
                    CatalogItem reportItem = new CatalogItem();
                    reportItem.Name = item.Name;
                    reportItem.Type = item.Type.ToString().GetItemTypeEnum();
                    reportItems.Add(reportItem);
                }

                this.IsExecuted = true;
                this.CloseReportingServer();
                return reportItems;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataSourceDefinition GetDataSourceDefinition(string dataSource)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                Microsoft.SqlServer.ReportingServices2005.DataSourceDefinition dataSourceDefinition = null;

                if (this.IsServerReport)
                {
                    Microsoft.SqlServer.ReportingServices2005.DataSource[] dataSources = ServiceProxy.GetItemDataSources(this.ReportPath);
                    Dictionary<string, string> dataSourceReferences = new Dictionary<string, string>();
                    dataSourceReferences = (from ds in dataSources where ds.Item is Microsoft.SqlServer.ReportingServices2005.DataSourceReference select new { Key = ds.Name, Value = (ds.Item as Microsoft.SqlServer.ReportingServices2005.DataSourceReference).Reference }).ToDictionary(k => k.Key, v => v.Value);
                    dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSourceReferences[dataSource]);
                }
                else
                {
                    dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSource);
                }

                this.CloseReportingServer();
                this.IsExecuted = true;
                return this.GetDataSourceDefinition(dataSourceDefinition);
            }
            catch (SoapException)
            {
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }

        public SharedDatasetinfo GetSharedDataSet(string path)
        {
            return null;
        }

        public byte[] GetReportDefinition()
        {
            this.IsExecuted = false;
            this.SetReportingServer();
            Byte[] reportBinaryContents = null;

            try
            {
                reportBinaryContents = ServiceProxy.GetReportDefinition(this.ReportPath);
                this.CloseReportingServer();
                this.IsExecuted = true;
                 return reportBinaryContents;
            }
            catch (SoapException ex)
            {
                throw ex;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
     
        DataSourceDefinition GetDataSourceDefinition(Microsoft.SqlServer.ReportingServices2005.DataSourceDefinition dataSourceDefinition)
        {
            DataSourceDefinition defintion = new DataSourceDefinition();
            defintion.ConnectString = dataSourceDefinition.ConnectString;
            defintion.CredentialRetrieval = dataSourceDefinition.CredentialRetrieval.ToString().GetCredential();
            defintion.Enabled = dataSourceDefinition.Enabled;
            defintion.EnabledSpecified = dataSourceDefinition.EnabledSpecified;
            defintion.Extension = dataSourceDefinition.Extension;
            defintion.ImpersonateUser = dataSourceDefinition.ImpersonateUser;
            defintion.ImpersonateUserSpecified = dataSourceDefinition.ImpersonateUserSpecified;
            defintion.OriginalConnectStringExpressionBased = dataSourceDefinition.OriginalConnectStringExpressionBased;
            defintion.Password = dataSourceDefinition.Password;
            defintion.Prompt = dataSourceDefinition.Prompt;
            defintion.UseOriginalConnectString = dataSourceDefinition.UseOriginalConnectString;
            defintion.UserName = dataSourceDefinition.UserName;
            defintion.WindowsCredentials = dataSourceDefinition.WindowsCredentials;
            return defintion;
        }
    }

    internal class ReportService2006 : ReportingServer, IReportService
    {
        private ReportingServer reportingServer;

        private ReportingService2006 ServiceProxy { get; set; }        

        public bool IsExecuted { get; set; }

        public ReportService2006(ReportingServer reportingServer)
        {
            ServiceProxy = new ReportingService2006();
            this.reportingServer = reportingServer;
        }

        private void SetReportingServer()
        {
            this.ReportPath = this.reportingServer.ReportPath;
            this.ReportServerCredential = this.reportingServer.ReportServerCredential;
            this.ReportServerFormsCredential = this.reportingServer.ReportServerFormsCredential;
            this.ReportServerUrl = this.reportingServer.ReportServerUrl;
            this.IsServerReport = this.reportingServer.IsServerReport;
            this.PreAuthenticate = this.reportingServer.PreAuthenticate;

            ServiceProxy.PreAuthenticate = this.PreAuthenticate;
            ServiceProxy.Url = this.ReportServerUrl.GetServiceUrl("2006");
            ServiceProxy.Credentials = this.ReportServerCredential;
        }


        public bool SetReportDefinition(byte[] reportData)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                ServiceProxy.SetReportDefinition(this.ReportPath, reportData);
                this.IsExecuted = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateReportDefinition(string reportname, string folderName, byte[] reportData)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                Microsoft.SqlServer.ReportingServices2006.Warning[] warnings;
                // Set report properties
                Microsoft.SqlServer.ReportingServices2006.Property reportProperty = new Microsoft.SqlServer.ReportingServices2006.Property();
                reportProperty.Name = "Description";
                reportProperty.Value = reportname; // May want to prompt for this
                Microsoft.SqlServer.ReportingServices2006.Property[] reportProperties = new Microsoft.SqlServer.ReportingServices2006.Property[1];
                reportProperties[0] = reportProperty;
                ServiceProxy.CreateReport(reportname, folderName, true, reportData, reportProperties,out warnings);
                this.IsExecuted = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<CatalogItem> GetCatalogItem(string folderName)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                Microsoft.SqlServer.ReportingServices2006.CatalogItem[] items = ServiceProxy.ListChildren(folderName);

                List<CatalogItem> reportItems = new List<CatalogItem>();

                foreach (var item in items)
                {
                    CatalogItem reportItem = new CatalogItem();
                    reportItem.Name = item.Name;
                    reportItem.Type = item.Type.ToString().GetItemTypeEnum();
                    reportItems.Add(reportItem);
                }

                this.IsExecuted = true;
                return reportItems;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public DataSourceDefinition GetDataSourceDefinition(string dataSource)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                Microsoft.SqlServer.ReportingServices2006.DataSourceDefinition dataSourceDefinition = null;

                if (this.IsServerReport)
                {
                    Microsoft.SqlServer.ReportingServices2006.DataSource[] dataSources = ServiceProxy.GetItemDataSources(this.ReportPath);
                    Dictionary<string, string> dataSourceReferences = new Dictionary<string, string>();
                    dataSourceReferences = (from ds in dataSources where ds.Item is Microsoft.SqlServer.ReportingServices2006.DataSourceReference select new { Key = ds.Name, Value = (ds.Item as Microsoft.SqlServer.ReportingServices2006.DataSourceReference).Reference }).ToDictionary(k => k.Key, v => v.Value);
                    dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSourceReferences[dataSource]);
                }
                else
                {
                    dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSource);
                }

                this.IsExecuted = true;
                return this.GetDataSourceDefinition(dataSourceDefinition);
            }
            catch (SoapException)
            {
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }

        public SharedDatasetinfo GetSharedDataSet(string path)
        {
            return null;
        }

        public byte[] GetReportDefinition()
        {
            this.IsExecuted = false;
            this.SetReportingServer();
            Byte[] reportBinaryContents = null;

            try
            {
                reportBinaryContents = ServiceProxy.GetReportDefinition(this.ReportPath);
                this.IsExecuted = true;
                return reportBinaryContents;
            }
            catch (SoapException ex)
            {
                throw ex;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        DataSourceDefinition GetDataSourceDefinition(Microsoft.SqlServer.ReportingServices2006.DataSourceDefinition dataSourceDefinition)
        {
            DataSourceDefinition defintion = new DataSourceDefinition();
            defintion.ConnectString = dataSourceDefinition.ConnectString;
            defintion.CredentialRetrieval = dataSourceDefinition.CredentialRetrieval.ToString().GetCredential();
            defintion.Enabled = dataSourceDefinition.Enabled;
            defintion.EnabledSpecified = dataSourceDefinition.EnabledSpecified;
            defintion.Extension = dataSourceDefinition.Extension;
            defintion.ImpersonateUser = dataSourceDefinition.ImpersonateUser;
            defintion.ImpersonateUserSpecified = dataSourceDefinition.ImpersonateUserSpecified;
            defintion.OriginalConnectStringExpressionBased = dataSourceDefinition.OriginalConnectStringExpressionBased;
            defintion.Password = dataSourceDefinition.Password;
            defintion.Prompt = dataSourceDefinition.Prompt;
            defintion.UseOriginalConnectString = dataSourceDefinition.UseOriginalConnectString;
            defintion.UserName = dataSourceDefinition.UserName;
            defintion.WindowsCredentials = dataSourceDefinition.WindowsCredentials;
            return defintion;
        }
    }

    internal class ReportService2010 : ReportingServer, IReportService
    {
        private ReportingServer reportingServer;

        private ReportingService2010 ServiceProxy { get; set; }

        public bool IsExecuted { get; set; }

        public ReportService2010(ReportingServer reportingServer)
        {
            ServiceProxy = new ReportingService2010();
            this.reportingServer = reportingServer;
        }

        private void SetReportingServer()
        {
            this.ReportPath = this.reportingServer.ReportPath;
            this.ReportServerCredential = this.reportingServer.ReportServerCredential;
            this.ReportServerFormsCredential = this.reportingServer.ReportServerFormsCredential;
            this.ReportServerUrl = this.reportingServer.ReportServerUrl;
            this.IsServerReport = this.reportingServer.IsServerReport;
            this.PreAuthenticate = this.reportingServer.PreAuthenticate;

            ServiceProxy.PreAuthenticate = this.PreAuthenticate;
            ServiceProxy.Url = this.ReportServerUrl.GetServiceUrl("2010");

            if (this.ReportServerFormsCredential != null)
            {
                this.ServiceProxy.CookieContainer = new CookieContainer();
                string userName = this.ReportServerFormsCredential.UserName;
                string passWord = this.ReportServerFormsCredential.Password;
                this.ServiceProxy.LogonUser(userName, passWord, this.ReportServerUrl.GetAuthortiy());
            }
            else
            {
                ServiceProxy.Credentials = this.ReportServerCredential;
            }
        }

        private void CloseReportingServer()
        {
            if (this.ReportServerFormsCredential != null)
            {
                this.ServiceProxy.Logoff();
                this.ServiceProxy.CookieContainer = null;
            }
        }

        public bool SetReportDefinition(byte[] reportData)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                ServiceProxy.SetItemDefinition(this.ReportPath, reportData,null);
                this.IsExecuted = true;
                this.CloseReportingServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateReportDefinition(string reportname, string folderName, byte[] reportData)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                Microsoft.SqlServer.ReportingServices2010.Warning[] warnings;
                // Set report properties
                Microsoft.SqlServer.ReportingServices2010.Property reportProperty = new Microsoft.SqlServer.ReportingServices2010.Property();
                reportProperty.Name = "Description";
                reportProperty.Value = reportname; // May want to prompt for this
                Microsoft.SqlServer.ReportingServices2010.Property[] reportProperties = new Microsoft.SqlServer.ReportingServices2010.Property[1];
                reportProperties[0] = reportProperty;
                ServiceProxy.CreateCatalogItem("Report",reportname, folderName, true, reportData, reportProperties, out warnings);
                this.IsExecuted = true;
                this.CloseReportingServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<CatalogItem> GetCatalogItem(string folderName)
        {
            this.IsExecuted = false;
            this.SetReportingServer();
            try
            {
                Microsoft.SqlServer.ReportingServices2010.CatalogItem[] items = ServiceProxy.ListChildren(folderName, false);

                List<CatalogItem> reportItems = new List<CatalogItem>();

                foreach (var item in items)
                {
                    CatalogItem reportItem = new CatalogItem();
                    reportItem.Name = item.Name;
                    reportItem.Type = item.TypeName.GetItemTypeEnum();
                    reportItems.Add(reportItem);
                }

                this.IsExecuted = true;
                this.CloseReportingServer();
                return reportItems;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SharedDatasetinfo GetSharedDataSet(string path)
        {
            this.IsExecuted = false;
            this.SetReportingServer();
            SharedDatasetinfo info = new SharedDatasetinfo();
            //Microsoft.SqlServer.ReportingServices2010.DataSourceDefinition DataSourceDefintion = null;
            try
            {
                Byte[] reportBinaryContents = null;
                ItemReferenceData[] refre = ServiceProxy.GetItemReferences(this.ReportPath, "DataSet");
                Dictionary<string, string> dataSetReferences = new Dictionary<string, string>();
                dataSetReferences = (from ds in refre select new { Key = ds.Name, Value = (ds as ItemReferenceData).Reference }).ToDictionary(k => k.Key, v => v.Value);
                reportBinaryContents = ServiceProxy.GetItemDefinition(dataSetReferences[path]);
                info.DataSetStream = new MemoryStream(reportBinaryContents);
                Microsoft.SqlServer.ReportingServices2010.DataSource[] _dataSource = ServiceProxy.GetItemDataSources(dataSetReferences[path]);
                Dictionary<string, string> dataSourceReferences = new Dictionary<string, string>();
                dataSourceReferences = (from ds in _dataSource where ds.Item is Microsoft.SqlServer.ReportingServices2010.DataSourceReference select new { Key = ds.Name, Value = (ds.Item as Microsoft.SqlServer.ReportingServices2010.DataSourceReference).Reference }).ToDictionary(k => k.Key, v => v.Value);
                info.DataSourceDefinition = this.GetDataSourceDefinition(ServiceProxy.GetDataSourceContents(dataSourceReferences[_dataSource[0].Name]), dataSourceReferences[_dataSource[0].Name].Substring(dataSourceReferences[_dataSource[0].Name].LastIndexOf('/') + 1));
                this.CloseReportingServer();
                this.IsExecuted = true;
                return info;

            }
            catch (SoapException)
            {
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }

        public DataSourceDefinition GetDataSourceDefinition(string dataSource)
        {
            this.IsExecuted = false;
            this.SetReportingServer();

            try
            {
                Microsoft.SqlServer.ReportingServices2010.DataSourceDefinition dataSourceDefinition = null;

                if (this.IsServerReport)
                {
                    Microsoft.SqlServer.ReportingServices2010.DataSource[] dataSources = ServiceProxy.GetItemDataSources(this.ReportPath);
                    Dictionary<string, string> dataSourceReferences = new Dictionary<string, string>();
                    dataSourceReferences = (from ds in dataSources where ds.Item is Microsoft.SqlServer.ReportingServices2010.DataSourceReference select new { Key = ds.Name, Value = (ds.Item as Microsoft.SqlServer.ReportingServices2010.DataSourceReference).Reference }).ToDictionary(k => k.Key, v => v.Value);

                    if (dataSourceReferences.Count == 0)
                    {
                        if (dataSource.StartsWith(@"/"))
                        {
                            dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSource);
                        }
                        else
                        {
                            ItemReferenceData[] refre = ServiceProxy.GetItemReferences(this.ReportPath, "DataSet");
                            dataSourceReferences = (from ds in refre select new { Key = ds.Name, Value = (ds as ItemReferenceData).Reference }).ToDictionary(k => k.Key, v => v.Value);
                            string sourcepath = dataSourceReferences[dataSource];
                            Microsoft.SqlServer.ReportingServices2010.DataSource[] _dataSource = ServiceProxy.GetItemDataSources(sourcepath);
                            Dictionary<string, string> _dataSourceReference = new Dictionary<string, string>();
                            _dataSourceReference = (from ds in _dataSource where ds.Item is Microsoft.SqlServer.ReportingServices2010.DataSourceReference select new { Key = ds.Name, Value = (ds.Item as Microsoft.SqlServer.ReportingServices2010.DataSourceReference).Reference }).ToDictionary(k => k.Key, v => v.Value);
                            dataSourceDefinition = ServiceProxy.GetDataSourceContents(_dataSourceReference.FirstOrDefault().Value);
                        }
                    }
                    else
                    {
                        dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSourceReferences[dataSource]);
                    }

                }
                else
                {
                    dataSourceDefinition = ServiceProxy.GetDataSourceContents(dataSource);
                }

                this.CloseReportingServer();
                this.IsExecuted = true;
                return this.GetDataSourceDefinition(dataSourceDefinition,dataSource);
            }
            catch (SoapException)
            {
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }

        public byte[] GetReportDefinition()
        {
            this.IsExecuted = false;
            this.SetReportingServer();
            Byte[] reportBinaryContents = null;

            try
            {
                reportBinaryContents = ServiceProxy.GetItemDefinition(this.ReportPath);
                this.CloseReportingServer();
                this.IsExecuted = true;
                return reportBinaryContents;
            }
            catch (SoapException ex)
            {
                throw ex;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        DataSourceDefinition GetDataSourceDefinition(Microsoft.SqlServer.ReportingServices2010.DataSourceDefinition dataSourceDefinition,String datasourcename)
        {
            DataSourceDefinition defintion = new DataSourceDefinition();
            defintion.Name = datasourcename;
            defintion.ConnectString = dataSourceDefinition.ConnectString;
            defintion.CredentialRetrieval = dataSourceDefinition.CredentialRetrieval.ToString().GetCredential();
            defintion.Enabled = dataSourceDefinition.Enabled;
            defintion.EnabledSpecified = dataSourceDefinition.EnabledSpecified;
            defintion.Extension = dataSourceDefinition.Extension;
            defintion.ImpersonateUser = dataSourceDefinition.ImpersonateUser;
            defintion.ImpersonateUserSpecified = dataSourceDefinition.ImpersonateUserSpecified;
            defintion.OriginalConnectStringExpressionBased = dataSourceDefinition.OriginalConnectStringExpressionBased;
            defintion.Password = dataSourceDefinition.Password;
            defintion.Prompt = dataSourceDefinition.Prompt;
            defintion.UseOriginalConnectString = dataSourceDefinition.UseOriginalConnectString;
            defintion.UserName = dataSourceDefinition.UserName;
            defintion.WindowsCredentials = dataSourceDefinition.WindowsCredentials;
            return defintion;
        }
    }

    public enum CredentialRetrievalEnum
    {
        Prompt,
        Store,
        Integrated,
        None,
    }

    internal class SharedDatasetinfo
    {
        public Stream DataSetStream
        {
            get;
            set;
        }
        public DataSourceDefinition DataSourceDefinition
        {
            get;
            set;
        }
    }

    internal class DataSourceDefinition
    {

        private string name; 

        private string extensionField;

        private string connectStringField;

        private bool useOriginalConnectStringField;

        private bool originalConnectStringExpressionBasedField;

        private CredentialRetrievalEnum credentialRetrievalField;

        private bool windowsCredentialsField;

        private bool impersonateUserField;

        private bool impersonateUserFieldSpecified;

        private string promptField;

        private string userNameField;

        private string passwordField;

        private bool enabledField;

        private bool enabledFieldSpecified;

        public string Extension
        {
            get
            {
                return this.extensionField;
            }
            set
            {
                this.extensionField = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }


        public string ConnectString
        {
            get
            {
                return this.connectStringField;
            }
            set
            {
                this.connectStringField = value;
            }
        }

        public bool UseOriginalConnectString
        {
            get
            {
                return this.useOriginalConnectStringField;
            }
            set
            {
                this.useOriginalConnectStringField = value;
            }
        }

        public bool OriginalConnectStringExpressionBased
        {
            get
            {
                return this.originalConnectStringExpressionBasedField;
            }
            set
            {
                this.originalConnectStringExpressionBasedField = value;
            }
        }

        public CredentialRetrievalEnum CredentialRetrieval
        {
            get
            {
                return this.credentialRetrievalField;
            }
            set
            {
                this.credentialRetrievalField = value;
            }
        }

        public bool WindowsCredentials
        {
            get
            {
                return this.windowsCredentialsField;
            }
            set
            {
                this.windowsCredentialsField = value;
            }
        }

        public bool ImpersonateUser
        {
            get
            {
                return this.impersonateUserField;
            }
            set
            {
                this.impersonateUserField = value;
            }
        }

        public bool ImpersonateUserSpecified
        {
            get
            {
                return this.impersonateUserFieldSpecified;
            }
            set
            {
                this.impersonateUserFieldSpecified = value;
            }
        }

        public string Prompt
        {
            get
            {
                return this.promptField;
            }
            set
            {
                this.promptField = value;
            }
        }

        public string UserName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }

        public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        public bool EnabledSpecified
        {
            get
            {
                return this.enabledFieldSpecified;
            }
            set
            {
                this.enabledFieldSpecified = value;
            }
        }
    }

    internal class CatalogItem
    {
        /// <remarks/>
        public string Name
        {
            get;
            set;            
        }       

        /// <remarks/>
        public ItemTypeEnum Type
        {
            get;
            set;
        }
    }

    internal enum ItemTypeEnum
    {
        /// <remarks/>
        Unknown,

        /// <remarks/>
        Folder,

        /// <remarks/>
        Report,

        /// <remarks/>
        Resource,

        /// <remarks/>
        DataSource,

        /// <remarks/>
        Model,

        /// <remarks/>
        Site,
    }

    internal static class ServerUtility
    {
        public static string GetServiceUrl(this string reportServerUrl,string version)
        {
            reportServerUrl = reportServerUrl.TrimEnd('/') + "/reportservice" + version + ".asmx";
            return reportServerUrl;
        }

        public static string GetAuthortiy(this string reportServerUrl)
        {
            Regex reg = new Regex(@"((?<servername>\S*)(?i:reportserver))", RegexOptions.IgnoreCase);
            Match match = reg.Match(reportServerUrl);
            return match.Groups["servername"].Value;
        }

        public static ItemTypeEnum GetItemTypeEnum(this string itemTypeEnum)
        {
            return (ItemTypeEnum)Enum.Parse(typeof(ItemTypeEnum), itemTypeEnum);
        }

        public static CredentialRetrievalEnum GetCredential(this string credentialRetrival)
        {
            return (CredentialRetrievalEnum)Enum.Parse(typeof(CredentialRetrievalEnum), credentialRetrival);
        }
    }
}
