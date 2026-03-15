using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System.Configuration;
using System.Net;
using DevExpress.Xpo.DB.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using DevExpress.Data.XtraReports.DataProviders;
using System.Text;
using DevExpress.Data.XtraReports.Wizard;
using System.Text.RegularExpressions;
using System.IO;
using ReportSettings.Documents;
using DevExpress.XtraReports.Data;
using ReportManager.ReportService;
using OPCUAViewModelService.ComponentService;

namespace UFUAWebReportEditor.Web
{
    /// <summary>
    /// Summary description for XpoHelper
    /// </summary>
    public static class XpoHelper
    {
        public static ReportServiceHelper Helper;
        
        public static void InitizalizeStaticMembers()
        {
            _ReportDoc = null;
            Helper = null;

            OPCUAViewModelComponent.QueryInterfaces();
            Helper = new ReportServiceHelper(ReportDoc, OPCUAViewModelComponent.ufuaEditorService);
        }

        static ReportDocument _ReportDoc;
        static ReportDocument ReportDoc
        {
            get
            {
                if (_ReportDoc == null)
                {
                    Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                    KeyValueConfigurationCollection settings = rootWebConfig.AppSettings.Settings;

                    var uri = settings["Uri"];
                    if (uri == null || String.IsNullOrEmpty(uri.Value))
                        throw new SettingsPropertyNotFoundException("'Uri' key element inside <appSettings> tag of the Web.Config file cannot be null or empty");
                    
                    var defaultUri = new Uri(uri.Value, UriKind.RelativeOrAbsolute);
                    _ReportDoc = ReportDocument.FromFile(defaultUri.AbsolutePath);
                }

                return _ReportDoc;
            }
        }
    }
}