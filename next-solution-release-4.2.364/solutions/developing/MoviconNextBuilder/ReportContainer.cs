using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;
using ReportSettings.Documents;
using ReportManager.ComponentService;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.XtraReports.Parameters;
using DevExpress.Xpf.Reports.UserDesigner;

namespace MoviconNextBuilder
{
    public enum ReportReturnCode : int
    {
        Ok = 0,
        ReportInvalid = -1,
        DataConversionFail = -2,
        ParamSetFail = -3,
        NoReport = -4,
        NoData = -5,
        NoParameter = -6
    }

    public class ReportContainer : IDisposable
    {
        #region Ctor
        public ReportContainer(IDocument parent, UFProjectManagerComponent manager)
        {
            projectdoc = (UFProjectManager.UFProjectDocument)parent;
            projectman = manager;
        }
        #endregion Ctor

        #region data
        UFProjectManager.UFProjectDocument projectdoc;
        UFProjectManagerComponent projectman;
        Dictionary<Uri, ReportDocument> mapDocuments = new Dictionary<Uri, ReportDocument>();
        #endregion data

        #region Properties
        static ReportManagerComponent reportManagerComponent = new ReportManagerComponent();
        static ReportManagerComponent ReportManagerComponent
        {
            get { return reportManagerComponent; }
        }
        #endregion Properties

        #region Method
        public ReportDocument GetReport(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ReportManager", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, ReportManagerComponent, projectdoc, bCheckExists: true);
            if (uri == null)
                return null;

            if (mapDocuments.ContainsKey(uri))
                return mapDocuments[uri];
            else
            {
                var doc = ReportManagerComponent.GetDocument(projectdoc, uri, bCreate:false);
                if (doc != null)
                {
                    mapDocuments[uri] = doc;
                    return doc;
                }
            }
            return null;
        }
        
        public object GetParameterValue(string reportName, string paramName, string subfolder = null)
        {
            var doc = GetReport(reportName, subfolder);
            if (doc != null)
            {
                XtraReport report = null;
                byte[] layoutData;
                if (doc.ReportDataXML != null)
                    layoutData = doc.ReportDataXML;
                else
                    layoutData = doc.ReportData;
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
                            return null;
                        }
                    }
                    if (report != null && report.Parameters != null)
                    {
                        var list = (from Parameter p in report.Parameters where p.Name == paramName select p).ToList();
                        if (list.Count > 0)
                        {
                            return list[0].Value;
                        }
                    }
                }
            }
            return null;
        }
        public ReportReturnCode SetParameterValue(string reportName, string paramName, object value, string subfolder = null)
        {
            var doc = GetReport(reportName, subfolder);
            if(doc != null)
            {
                XtraReport report = null;
                byte[] layoutData;
                if (doc.ReportDataXML != null)
                    layoutData = doc.ReportDataXML;
                else
                    layoutData = doc.ReportData;
                if(layoutData != null)
                {
                    using (var stream = new MemoryStream(layoutData))
                    {
                        try
                        {
                            report = XtraReport.FromStream(stream, true);
                        }
                        catch (Exception ex)
                        {
                            return ReportReturnCode.ReportInvalid;
                        }
                    }
                    if(report != null && report.Parameters != null)
                    {
                        var list = (from Parameter p in report.Parameters where p.Name == paramName select p).ToList();
                        if(list.Count > 0)
                        {
                            try
                            {
                                var newVal = Convert.ChangeType(value, list[0].Type);
                                list[0].Value = value;
                            }
                            catch(Exception ex)
                            {
                                return ReportReturnCode.DataConversionFail;
                            }

                            bool noData = false;
                            bool noXMLData = false;
                            //get new report data to store in document and save
                            using (var stream = new MemoryStream())
                            {
                                try
                                {
                                    ReportDesigner rp = new ReportDesigner();
                                    rp.ReportSerializer.Save(stream, report);
                                    doc.ReportData = stream.ToArray();
                                }
                                catch (Exception ex)
                                {
                                    noData = true;
                                }
                            }
                            using (var memoryStream = new MemoryStream())
                            {
                                try
                                {
                                    report.SaveLayoutToXml(memoryStream);
                                    doc.ReportDataXML = memoryStream.ToArray();
                                }
                                catch (Exception ex)
                                {
                                    noXMLData = true;
                                }
                            }
                            if (noData && noXMLData)
                                return ReportReturnCode.ParamSetFail;
                            doc.NeedsSave = true;
                            return ReportReturnCode.Ok;
                        }
                    }
                    return ReportReturnCode.NoParameter;
                }
                return ReportReturnCode.NoData;
            }
            return ReportReturnCode.NoReport;
        }
        public void Save()
        {
            if (mapDocuments.Count > 0)
            {
                foreach (var doc in mapDocuments.Values)
                {
                    if (doc.NeedsSave)
                    {
                        doc.SaveToFile();
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var doc in mapDocuments.Values)
                doc.Dispose();
            mapDocuments.Clear();
        }
        #endregion Method
    }
}
