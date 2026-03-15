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
using System.Text.RegularExpressions;
using Syncfusion.Reports.Server;
using Syncfusion.RDL.Data;

#if WINRT
using System.Threading.Tasks;
#endif

namespace Syncfusion.RDL.ServerProcessor
{
    internal class ServerReportProcessor
    {
        ReportServiceClient reportingServer = null;
        bool isValidConnection = false;
        byte[] report = null;
        SharedDatasetinfo dataSet = null;
        ServiceDataSourceDefinition dataSource = null;
        RecordInfo dataSetRecord = null;

        private ReportModel ReportModel { get; set; }

        internal event GetReportEventHandler GetReportCompleted;

        internal event GetDataEventHandler GetDataCompleted;

        internal event GetSharedDataSourceEventHandler GetSharedDataSourceCompleted;

        internal event GetSharedDataSetEventHandler GetSharedDataSetCompleted;

        internal event IsValidConnectionEventHandler IsValidConnectionCompleted;

        internal event ExportedHandler ExportCompleted;

        internal ReportServiceClient ReportingServer
        {
            get
            {
                if (reportingServer == null)
                {
                    this.UpdateServer();
                }

                return reportingServer;
            }
        }

        ReportServiceClient UpdateServer()
        {
            reportingServer = new ReportServiceClient(new System.ServiceModel.BasicHttpBinding()
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue
            }, new System.ServiceModel.EndpointAddress(this.ReportModel.ReportServiceURL));

#if !WINRT
            reportingServer.GetReportCompleted += new EventHandler<GetReportCompletedEventArgs>(reportingServer_GetReportCompleted);
            reportingServer.GetSharedDataSourceDefinitionCompleted += new EventHandler<GetSharedDataSourceDefinitionCompletedEventArgs>(reportingServer_GetSharedDataSourceDefinitionCompleted);
            reportingServer.IsValidConnectionCompleted += new EventHandler<IsValidConnectionCompletedEventArgs>(reportingServer_IsValidConnectionCompleted);
            reportingServer.GetDataCompleted += new EventHandler<GetDataCompletedEventArgs>(reportingServer_GetDataCompleted);
            reportingServer.GetSharedDataSetCompleted += new EventHandler<GetSharedDataSetCompletedEventArgs>(reportingServer_GetSharedDataSetCompleted);
            reportingServer.ExportCompleted += new EventHandler<ExportCompletedEventArgs>(reportingServer_ExportCompleted);
#endif
            return reportingServer;
        }


        public ServerReportProcessor(ReportModel reportModel)
        {
            this.ReportModel = reportModel;
        }

        public void GetReport(ReportSetting setting)
        {
            report = null;
#if WINRT
            this.GetReportAsync(setting);
#else
            this.ReportingServer.GetReportAsync(setting);
#endif
        }

        public void GetData(ReportSetting settings,ReportDataInfo info)
        {
#if WINRT
            this.GetDataAsync(settings, info);
#else
            this.ReportingServer.GetDataAsync(settings,info);
#endif
        }

        public void IsValidConnection(string connection,string dataProvider)
        {
#if WINRT
            this.IsValidConnectionAsync(connection, dataProvider);
#else
            this.ReportingServer.IsValidConnectionAsync(connection, dataProvider);
#endif
        }

        public void GetSharedDataSourceDefinition(ReportSetting setting,string dataSourceName)
        {
#if WINRT
            this.GetSharedDataSourceDefinitionAsync(setting, dataSourceName);
#else
            this.ReportingServer.GetSharedDataSourceDefinitionAsync(setting, dataSourceName);
#endif
        }

        public void GetSharedDataSet(ReportSetting setting, string dataSetName)
        {
#if WINRT
            this.GetSharedDataSetDefinitionAsync(setting, dataSetName);
#else
            this.ReportingServer.GetSharedDataSetAsync(setting, dataSetName);
#endif
        }

        public void Export(ReportSetting setting,string exportType)
        {
#if WINRT
            this.ExportAsync(setting, exportType);
#else
            this.ReportingServer.ExportAsync(setting, exportType);
#endif            
        }

#if WINRT        
        public async Task GetReportAsync(ReportSetting setting)
        {
            report = await this.ReportingServer.GetReportAsync(setting);
            this.RaiseGetReportCompletedEvent(report);
        }

        public async Task GetSharedDataSourceDefinitionAsync(ReportSetting setting,string dataSource)
        {
            this.dataSource = await this.ReportingServer.GetSharedDataSourceDefinitionAsync(setting, dataSource);
            this.RaiseGetSharedDataSourceCompletedEvent(this.dataSource);
        }

        public async Task GetSharedDataSetDefinitionAsync(ReportSetting setting, string dataSet)
        {
            this.dataSet = await this.ReportingServer.GetSharedDataSetAsync(setting, dataSet);
            this.RaiseGetSharedDataSetCompletedEvent(this.dataSet);
        }

        public async Task IsValidConnectionAsync(string connectionString,string dataProvider)
        {
            this.isValidConnection = await this.ReportingServer.IsValidConnectionAsync(connectionString, dataProvider);
            this.RaiseIsValidConnectionCompletedEvent(this.isValidConnection);
        }

        public async Task GetDataAsync(ReportSetting setting , ReportDataInfo info)
        {
            this.dataSetRecord = await this.ReportingServer.GetDataAsync(setting,info);
            this.RaiseGetDataCompletedEvent(this.dataSetRecord);
        }

        public async Task ExportAsync(ReportSetting setting,string exportType)
        {
            var export = await this.ReportingServer.ExportAsync(setting, exportType);
            this.RaiseExportCompletedEvent(export.Export);
        }
#else
        void reportingServer_ExportCompleted(object sender, ExportCompletedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(e.Result.Exception)))
            {
                this.ReportModel.ExceptionDetails.Clear();
                this.ReportModel.ExceptionDetails.Add("Report was not exported getting following exception :" + e.Result.Exception);
            }

            this.RaiseExportCompletedEvent(e.Result.Export);
        }

        void reportingServer_GetReportCompleted(object sender, GetReportCompletedEventArgs e)
        {
            this.report = e.Result;
            this.RaiseGetReportCompletedEvent(report);
        }

        void reportingServer_GetSharedDataSourceDefinitionCompleted(object sender, GetSharedDataSourceDefinitionCompletedEventArgs e)
        {            
            this.dataSource = e.Result;
            this.RaiseGetSharedDataSourceCompletedEvent(this.dataSource);
        }

        void reportingServer_GetSharedDataSetCompleted(object sender, GetSharedDataSetCompletedEventArgs e)
        {
            this.dataSet = e.Result;
            this.RaiseGetSharedDataSetCompletedEvent(this.dataSet);
        }

        void reportingServer_IsValidConnectionCompleted(object sender, IsValidConnectionCompletedEventArgs e)
        {
            this.isValidConnection = e.Result;
            this.RaiseIsValidConnectionCompletedEvent(this.isValidConnection);
        }

        void reportingServer_GetDataCompleted(object sender, GetDataCompletedEventArgs e)
        {
            this.dataSetRecord = e.Result;
            this.RaiseGetDataCompletedEvent(this.dataSetRecord);
        }
#endif

        void RaiseExportCompletedEvent(byte[] document)
        {
            ExportedEventArgs args = new ExportedEventArgs();
            args.Result = document;

            if (this.ExportCompleted != null)
            {
                this.ExportCompleted(this, args);
            }
        }

        void RaiseGetDataCompletedEvent(RecordInfo data)
        {
            GetDataEventArgs args = new GetDataEventArgs();
            args.Result = data;
            if (this.GetDataCompleted != null)
            {
                this.GetDataCompleted(this, args);
            }
        }

        void RaiseIsValidConnectionCompletedEvent(bool isValid)
        {
            IsValidConnectionEventArgs args = new IsValidConnectionEventArgs();
            args.Result = isValid;
            if (this.IsValidConnectionCompleted != null)
            {
                this.IsValidConnectionCompleted(this, args);
            }
        }

        void RaiseGetReportCompletedEvent(byte[] report)
        {
            GetReportEventArgs args = new GetReportEventArgs();
            args.Result = report;
            if (this.GetReportCompleted != null)
            {
                this.GetReportCompleted(this, args);
            }
        }

        void RaiseGetSharedDataSourceCompletedEvent(ServiceDataSourceDefinition dataSource)
        {
            GetSharedDataSourceEventArgs args = new GetSharedDataSourceEventArgs();
            args.Result = dataSource;
            if (this.GetSharedDataSourceCompleted != null)
            {
                this.GetSharedDataSourceCompleted(this, args);
            }
        }

        void RaiseGetSharedDataSetCompletedEvent(SharedDatasetinfo dataSet)
        {
            GetSharedDataSetEventArgs args = new GetSharedDataSetEventArgs();
            args.Result = dataSet;

            if (this.GetSharedDataSetCompleted != null)
            {
                this.GetSharedDataSetCompleted(this, args);
            }
        }
    }
}