using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.Service;
using DevExpress.XtraReports.Service;
using DevExpress.XtraReports.UI;
using System.Web;
using DevExpress.Xpo;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Collections.Generic;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.ServiceModel.DataContracts;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Xpo.DB;
using System.Text;
using DevExpress.XtraPrinting;
using ReportManager.ReportService;
using DataReader.Extensions;

namespace UFUAWebReportEditor.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ReportService" in code, svc and config file together.
    [SilverlightFaultBehavior]
    public class ReportService : DevExpress.XtraReports.Service.ReportService
    {
        protected override void RegisterDataSources(XtraReport report, string reportName)
        {
            report.DataSource = new DataSet(XpoHelper.Helper.DataSourceName);
            report.RegisterDataSourceName(XpoHelper.Helper.DisplayDataSourceName, report.DataSource);
        }
        
        protected override void FillDataSources(XtraReport report, string reportName, bool isDesignActive)
        {
            //if (isDesignActive)
            {
                var dataSet = (DataSet)report.GetDataSourceByName(XpoHelper.Helper.DisplayDataSourceName);
                XpoHelper.Helper.FillDataSource(ref dataSet, isDesignActive);
                report.DataSource = dataSet;
            }
        }

        protected override void SaveReportLayout(string reportName, byte[] layoutData)
        {
            XpoHelper.Helper.SaveReportDocument(layoutData, true);
        }

        protected override byte[] LoadReportLayout(string reportName)
        {
            if (XpoHelper.Helper.ReportData != null)
                return XpoHelper.Helper.ReportData;

            return base.LoadReportLayout(reportName);
        }

        public override string AddNewReport(ReportModel model)
        {
            XpoHelper.Helper.WizardGeneratedReport(model);

            return base.AddNewReport(model);
        }

        public override IEnumerable<DataSourceInfo> GetDataSources()
        {
            var ret = new List<DataSourceInfo>();

            // add reportDoc data source name if exists
            if (XpoHelper.Helper.ReportDocModel != null && XpoHelper.Helper.ReportDocModel.IsValidDataSource())
            {
                ret.Add(new DataSourceInfo()
                {
                    Name = XpoHelper.Helper.ReportDocModel.DataSourceName,
                    DisplayName = XpoHelper.Helper.ReportDocModel.DataSourceDisplayName,
                    TablesOrViewsSupported = true
                });
            }

            // add historian data source name if exists
            if (XpoHelper.Helper.UFUAHistorianModel != null && XpoHelper.Helper.UFUAHistorianModel.IsValidDataSource())
            {
                ret.Add(new DataSourceInfo()
                {
                    Name = XpoHelper.Helper.UFUAHistorianModel.DataSourceName,
                    DisplayName = XpoHelper.Helper.UFUAHistorianModel.DataSourceDisplayName,
                    TablesOrViewsSupported = true
                });
            }

            // add event data source name if exists
            if (XpoHelper.Helper.UFUAEventLogModel != null && XpoHelper.Helper.UFUAEventLogModel.IsValidDataSource())
            {
                ret.Add(new DataSourceInfo()
                {
                    Name = XpoHelper.Helper.UFUAEventLogModel.DataSourceName,
                    DisplayName = XpoHelper.Helper.UFUAEventLogModel.DataSourceDisplayName,
                    TablesOrViewsSupported = true
                });
            }

            return ret;

            /*
            DevExpress.XtraReports.Data.DataProviderRepository.Current.Clear();
            DevExpress.XtraReports.Data.DataProviderRepository.Current.Register(Helper.DataSourceName, Helper.DisplayDataSourceName, Helper.SchemaInfo);
            return DevExpress.XtraReports.Data.DataProviderRepository.Current.EnumerateDataSourceInfo();
            */
        }

        public override IEnumerable<TableInfo> GetTables(string dataSourceName)
        {
            return XpoHelper.Helper.GetTables(dataSourceName);
        }

        public override IEnumerable<TableInfo> GetViews(string dataSourceName)
        {
            return XpoHelper.Helper.GetViews(dataSourceName);
        }

        public override IEnumerable<ColumnInfo> GetColumns(string dataSourceName, TableInfo dataMemberName)
        {
            return XpoHelper.Helper.GetColumns(dataSourceName, dataMemberName);
        }
    }
}
