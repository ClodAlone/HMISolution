using System;
using System.Text;
using System.Collections.Generic;
using DocumentManager.ComponentService;
using ReportSettings.Documents;
using System.Reflection;

namespace ReportAlarms
{
    public class ReportAlarms
    {
        #region Declarations
        readonly string basename;
        readonly IDocument parent;

        bool renameFilePath;
        string filePath;

        const string reportPrefix = "AlarmReport.";
        #endregion 

        #region Constructors
        public ReportAlarms(AlarmReportType reportType, IDocument parent, bool renameFilePath = false)
        {
            basename = GetBaseName(reportType);
            this.parent = parent;
            this.renameFilePath = renameFilePath;
        }
        #endregion

        #region Public Methods
        public ReportDocument GetReportDocument()
        {
            try
            {
                ExtractFileFromResources();
                if (!String.IsNullOrEmpty(filePath))
                    return ReportDocument.FromFile(filePath, parent, true);

            }
            finally
            {
                if (!String.IsNullOrEmpty(filePath))
                {
                    try 
                    { 
                        System.IO.File.Delete(filePath); 
                    }
                    catch { }
                }
            }

            return null;
        }

        public static List<Uri> GetWholeAlarmReportDocumentList(IDocument parent, string tmpPath = null)
        {
            var reports = new List<Uri>();
            foreach (AlarmReportType reportType in Enum.GetValues(typeof(AlarmReportType)))
            {
                var report = new ReportAlarms(reportType, parent) { renameFilePath = true, filePath = tmpPath };
                report.ExtractFileFromResources();
                if (!String.IsNullOrEmpty(report.filePath))
                    reports.Add(new Uri(report.filePath, UriKind.RelativeOrAbsolute));
            }

            return reports;
        }

        public static bool IsAlarmReportFile(string filePath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(filePath).StartsWith(reportPrefix);
        }
        #endregion

        #region Private Methods
        string GetBaseName(AlarmReportType reportType)
        {
            switch (reportType)
            {
                case AlarmReportType.OrderByDateTime:
                    return "OrderByDateTime.rpt";
                case AlarmReportType.OrderByDuration:
                    return "OrderByDuration.rpt";
                case AlarmReportType.OrderByOccurrence:
                    return "OrderByOccurrence.rpt";
                default:
                    return null;
            }
        }

        void ExtractFileFromResources()
        {
            if (basename != null)
            {
                try
                {
                    if (filePath == null)
                        filePath = System.IO.Path.GetTempFileName();
                    if (renameFilePath)
                        filePath = String.Format("{0}\\{1}{2}", System.IO.Path.GetDirectoryName(filePath), reportPrefix, basename);
                    else if (String.IsNullOrEmpty(System.IO.Path.GetFileName(filePath)))
                        filePath = String.Format("{0}{1}", filePath, System.IO.Path.GetRandomFileName());
                    if (!System.IO.File.Exists(filePath) || new System.IO.FileInfo(filePath).Length == 0)
                        Utilities.ExtractFileFromResource.Extract(Assembly.GetExecutingAssembly(), filePath, String.Format("{0}.{1}", typeof(ReportAlarms).Namespace, basename));
                }
                catch
                {
                    filePath = null;
                }
            }
        }

        #endregion
    }
}
