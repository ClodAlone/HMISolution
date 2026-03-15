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

namespace Syncfusion.Reports.Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    
    public class ReportService : IReportService
    {
        ReportManager manager = new ReportManager();
        
        public byte[] GetReport(ReportSetting setting)
        {
            this.OnGetServerCredentials(setting);
            return manager.GetReport(setting);
        }

        public SharedDatasetinfo GetSharedDataSet(ReportSetting setting, string path)
        {
            this.OnGetServerCredentials(setting);
            return manager.GetSharedDataSet(setting,path);
        }

        public RecordInfo GetData(ReportSetting Setting , ReportDataInfo ReportInfo)
        {
            this.OnGetServerCredentials(Setting);
            this.OnGetDataSourceInformation(ReportInfo);
            return manager.GetDataSource (Setting,ReportInfo);
        }

        public ServiceDataSourceDefinition GetSharedDataSourceDefinition(ReportSetting setting, string dataSource)
        {
            this.OnGetServerCredentials(setting);
            return manager.GetDataSourceDefinition(setting, dataSource);
        }

        public ExportData Export(ReportSetting setting, string exportType)
        {
            this.OnGetServerCredentials(setting);
            return manager.Export(setting, exportType);
        }

        public bool IsValidConnection(string connectionString,string dataProvider)
        {
            return manager.IsValidConnection(connectionString,dataProvider);
        }

        public virtual void OnGetDataSourceInformation( ReportDataInfo info)
        {
            
        }

        public virtual void OnGetServerCredentials(ReportSetting setting)
        {

        }
    }
}