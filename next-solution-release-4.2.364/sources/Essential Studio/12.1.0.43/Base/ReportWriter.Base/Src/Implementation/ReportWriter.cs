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
using Syncfusion.RDL.Data;
using System.Drawing;
using System.ComponentModel;
#if WINRT
using Syncfusion.UI.Xaml.Reports;
using Syncfusion.RDL.Internal;
using System.Collections;
using System.Reflection;
using System.Collections.ObjectModel;
#else
using Syncfusion.Windows.Reports;
using Syncfusion.RDL.Internal;
using System.Collections;
using System.Reflection;

#endif

namespace Syncfusion.ReportWriter
{
    /// <summary>
    /// To set the PageSettings of Reportviewer
    /// </summary>
    /// <remarks></remarks>
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class PageSettings
    {
        /// <summary>
        /// Gets or sets TopMargin value.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double TopMargin { get; set; }
        /// <summary>
        /// Gets or sets LeftMargin value.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double LeftMargin { get; set; }
        /// <summary>
        /// Gets or sets RightMargin value.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double RightMargin { get; set; }
        /// <summary>
        /// Gets or sets BottomMargin value.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double BottomMargin { get; set; }
        /// <summary>
        /// Gets or sets PageHeight value.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PageHeight { get; set; }
        /// <summary>
        /// Gets or sets PageWidth value.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PageWidth { get; set; }
    }

    internal class ExportReportParameter
    {
        public string Name { get; set; }
        public List<object> Label { get; set; }
        public List<object> Value { get; set; }
    }

    internal class ExportReportDataSource
    {        
        public string Name { get; set; }
#if !WINRT 
        public object Value { get; set; }
#else 
        public System.Collections.IEnumerable Value { get; set; }
#endif
    }

    internal class ExportReportServerFormsCredential
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }

    internal class ExportDataSourceCredentials
    {
        public bool IntegratedSecurity { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string UserId { get; set; }
    }


    /// <summary>
    /// Types of writer format in Reportviewer
    /// </summary>
    /// <remarks></remarks>
    public enum WriterFormat
    {
        /// <summary>
        /// Export option for PDF format.
        /// </summary>
        /// <remarks></remarks>
        PDF,
        /// <summary>
        /// Export option for Excel format.
        /// </summary>
        /// <remarks></remarks>
        Excel,
        /// <summary>
        /// Export option for Word format.
        /// </summary>
        /// <remarks></remarks>
        Word,
        /// <summary>
        /// Export option for HTML format.
        /// </summary>
        /// <remarks></remarks>
        HTML
    }


    /// <summary>
    /// Exports the Reports in different format.
    /// </summary>
    /// <remarks></remarks>
    public abstract class WriterBase
    {
        #region fields
        private string m_reportName;
        private ReportDataSourceCollection m_dataSources;
        private ReportModel m_reportModel;
        private ICredentials m_reportServerCredentials;
        private ReportServerFormsCredential m_reportServerFormCredentials = null;
        private string m_reportServerUrl;
#if SILVERLIGHT
        private string m_reportServiceURL;
#endif
        private int m_PDFSplitPageCount = 10;
        ReportingBrushConverter m_Convertor = new ReportingBrushConverter();
        #endregion fields

        #region Properties

        /// <summary>
        /// Gets or sets the report path (file name of the report with its full path).
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ReportPath
        {
            get
            {
                return m_reportName;
            }
            set
            {
                m_reportName = value;
                ProcessReport();
            }
        }
        /// <summary>
        /// Gets or sets a value for Whether RDLC support or not.
        /// </summary>
        /// <value><c>Local</c> if this instance is RDLC; otherwise, <c>Remote</c>.</value>
        public ProcessingMode ReportProcessingMode
        {
            get;
            set;
        }

#if SILVERLIGHT
        /// <summary>
        ///  Sets a value for ExportMode.
        /// </summary>
        /// <value><c>Server</c> if this RDL; otherwise, <c>Local</c>.</value>
        [DefaultValue(ExportMode.Local)]
        public ExportMode ExportMode
        {
            get;
            set;
        }
#endif


#if SILVERLIGHT
        /// <summary>
        /// Get or Set the ReportServiceURL.
        /// </summary>
        /// <value>The URL of the RemoteServer.</value>
        public string ReportServiceURL
        {
            get
            {
                return m_reportServiceURL;
            }
            set
            {
                m_reportServiceURL = value;
            }
        }
#endif
        /// <summary>
        /// Gets or sets the report server URL.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ReportServerUrl
        {
            get
            {
                return m_reportServerUrl;
            }
            set
            {
                m_reportServerUrl = value;
                ProcessReport();
            }
        }
        /// <summary>
        /// Gets or sets the data sources.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ReportDataSourceCollection DataSources
        {
            get
            {
                return m_dataSources;
            }
            set
            {
                m_dataSources = value;
            }
        }
        /// <summary>
        /// Gets or sets the report server credential.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ICredentials ReportServerCredential
        {
            get
            {
                return m_reportServerCredentials;
            }
            set
            {
                m_reportServerCredentials = value;
                ProcessReport();
            }
        }


        /// <summary>
        /// Gets or sets credentials for ReportServer.
        /// </summary>
        /// <value>UserName and Password</value>
        /// <remarks></remarks>
        public ReportServerFormsCredential ReportServerFormsCredential
        {
            get
            {
                return m_reportServerFormCredentials;
            }
            set
            {
                m_reportServerFormCredentials = value;
                ProcessReport();
            }
        }

        /// <summary>
        /// Specify the temp path to perform split and merge process in PDF export.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string PDFTempPath
        {
            get;
            set;
        }

        /// <summary>
        /// Specify the pagecount to split the PDF document for merge.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [DefaultValue(10)]
        public int PDFSplitPageCount
        {
            get
            {
                return this.m_PDFSplitPageCount;
            }
            set
            {
                if (this.m_PDFSplitPageCount != value && value > 0)
                {
                    this.m_PDFSplitPageCount = value;
                }
            }
        }

        /// <summary>
        /// Enable Split and Merge in PDF export to avoid out of memory exceptions.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool EnablePDFSplitMerge
        {
            get;
            set;
        }

        /// <summary>
        /// Load Font stream from external file
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Dictionary<string,Stream> PDFFonts
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ExcelVersion.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ExcelVersion ExcelVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets WordFormatType.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public WordFormatType WordFormatType
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the report definition.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal Syncfusion.RDL.DOM.ReportDefinition ReportDefinition
        {
            get
            {
                return this.ReportModel.Report;
            }
        }


        /// <summary>
        /// Gets or sets the report model.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal ReportModel ReportModel
        {
            get
            {
                if (m_reportModel == null)
                {
                    m_reportModel = new ReportModel();
                    m_reportModel.EnableVirtualEvaluation = true;
                }

                return m_reportModel;
            }
            set
            {
                m_reportModel = value;
            }
        }

        /// <summary>
        /// Gets or sets PageSettings of Reportviewer.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public PageSettings PageSettings
        {
            get;
            set;
        }

        internal IEnumerable<DataSourceCredentials> DataSourceCredentials
        {
            get;
            set;
        }


        #endregion Properties

        #region PUBLIC Methods
        /// <summary>
        /// Loads the report
        /// </summary>
        /// <param name="fileStream">The stream which contains the rdl contents</param>
        /// <remarks></remarks>
        public void LoadReport(Stream fileStream)
        {
            this.ReportModel.LoadReport(fileStream);
        }

        /// <summary>
        /// Loads the sub report
        /// </summary>
        /// <param name="report">The stream which contains the rdl contents</param>
        /// <param name="reportName">Name of the report</param>
        /// <remarks></remarks>
        public void LoadSubreport(string reportName, Stream report)
        {
            if (this.ReportModel != null)
            {
                if (this.ReportModel.SubReportStream.Keys.Contains(reportName))
                {
                    this.ReportModel.SubReportStream[reportName] = report;
                }
                else
                {
                    this.ReportModel.SubReportStream.Add(reportName, report);
                }
                try
                {
                    var subreports = from reportitem in this.ReportModel.BodyReportItemModels where reportitem.ReportItem != null && (reportitem.ReportItem is RDL.DOM.SubReport) && (reportitem.ReportItem as RDL.DOM.SubReport).ReportName.Equals(reportName) select reportitem;
                    if (subreports != null)
                    {
                        foreach (var subreport in subreports.ToList())
                        {
                            subreport.Load();
                        }
                    }
                }
                catch { }
            }
        }

#if !WINRT
        /// <summary>
        /// Loads the sub report
        /// </summary>
        /// <param name="reportName">Name of the report</param>
        /// <param name="reportPath">Path to the report file</param>
        /// <remarks></remarks>
        public void LoadSubreport(string reportName, string reportPath)
        {
            if (File.Exists(reportPath))
            {
                using (FileStream stream = new FileStream(reportPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    this.LoadSubreport(reportName, stream);
                }
            }
        }
#endif
        /// <summary>
        /// Loads the sub report
        /// </summary>
        /// <param name="report">The  text reader which contains the rdl contents</param>
        /// <param name="reportName">Name of the report</param>
        /// <remarks></remarks>
        public void LoadSubreport(string reportName, TextReader report)
        {
            byte[] reportBytes = System.Text.Encoding.UTF8.GetBytes(report.ReadToEnd());
            Stream stream = new MemoryStream(reportBytes.ToArray());
            this.LoadSubreport(reportName, stream);
        }

        /// <summary>
        /// Returns the data set names
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<string> GetDataSetNames()
        {
            if (this.ReportProcessingMode == ProcessingMode.Local)
            {
                return this.ReportModel.GetDataSetNames();
            }

            return null;
        }

        /// <summary>
        ///Gets the Datasource information of the report
        /// </summary>
        /// <returns>Report Datasources</returns>
        /// <remarks></remarks>
        public ReportDataSourceInfoCollection GetDataSources()
        {
            if (this.ReportProcessingMode == ProcessingMode.Remote && this.ReportModel.HasReport)
            {
                this.ReportModel.GetDataSources();
            }

            return null;
        }

        /// <summary>
        /// Sets Datasource credentials
        /// </summary>
        /// <param name="dataSourceCredentials">Array of Credential Informations</param>
        /// <remarks></remarks>
        public void SetDataSourceCredentials(DataSourceCredentials[] dataSourceCredentials)
        {
            if (this.ReportProcessingMode == ProcessingMode.Remote && this.ReportModel.HasReport)
            {
                this.ReportModel.SetDataSourceCredentials(dataSourceCredentials);
            }
        }

        /// <summary>
        /// Sets Datasource credentials
        /// </summary>
        /// <param name="dataSourceCredentials">collection of Credential Informations</param>
        /// <remarks></remarks>
        public void SetDataSourceCredentials(IEnumerable<DataSourceCredentials> dataSourceCredentials)
        {
            this.DataSourceCredentials = dataSourceCredentials;
            if (this.ReportProcessingMode == ProcessingMode.Remote && this.ReportModel.HasReport)
            {
                this.ReportModel.SetDataSourceCredentials(dataSourceCredentials);
            }
        }

        internal void SetExportDataSourceCredentials(IEnumerable<ExportDataSourceCredentials> dataSourceCredentials)
        {
            if (this.ReportProcessingMode == ProcessingMode.Remote)
            {
                List<DataSourceCredentials> credentials = new List<DataSourceCredentials>();

                foreach (var dataSource in dataSourceCredentials)
                {
                    DataSourceCredentials crden = new DataSourceCredentials();
                    crden.Name = dataSource.Name;
                    crden.UserId = dataSource.UserId;
                    crden.Password = dataSource.Password;
                    crden.IntegratedSecurity = dataSource.IntegratedSecurity;
                    credentials.Add(crden);
                }

                this.ReportModel.SetDataSourceCredentials(credentials);
            }
        }

        /// <summary>
        /// Get the parameters
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public ReportParameterInfoCollection GetParameters()
        {
            return this.ReportModel.GetParameters();
        }
        /// <summary>
        /// Set the parameter for the report
        /// </summary>
        /// <param name="reportParameters"></param>
        /// <remarks></remarks>
        public void SetParameters(IEnumerable<ReportParameter> reportParameters)
        {
            this.ReportModel.SetParameters(reportParameters);
        }

        internal void SetExportParameters(IEnumerable<ExportReportParameter> reportParameters)
        {
            List<ReportParameter> parameters = new List<ReportParameter>();

            foreach (var reportParam in reportParameters)
            {
                ReportParameter param = new ReportParameter();
                param.Name = reportParam.Name;
                param.Labels = new List<string>();
                param.Values = new List<string>();
                foreach(var lab in reportParam.Label)
                {
                    param.Labels.Add(lab.ToString());
                }

                foreach(var val in reportParam.Value)
                {
                    param.Values.Add(val.ToString());
                }
                parameters.Add(param);
            }

            this.ReportModel.SetParameters(parameters);
        }


        internal void SetDataSource(List<ExportReportDataSource> reportDataSource)
        {
            this.DataSources = new ReportDataSourceCollection();

            foreach (var data in reportDataSource)
            {
                ReportDataSource dataSource = new ReportDataSource();
                dataSource.Name = data.Name;
                dataSource.Value = data.Value;
                this.DataSources.Add(dataSource);
            }
        }

        internal void SetReportServerFormsCrdential(ExportReportServerFormsCredential creden)
        {
            this.ReportServerFormsCredential = new ReportServerFormsCredential(creden.UserName, creden.PassWord);
        }

        #endregion PUBLIC Methods

        #region Internal Methods

        void ProcessReport()
        {
            this.ReportModel.ReportPath = this.ReportPath;
            this.ReportModel.ReportServerUrl = this.ReportServerUrl;
            this.ReportModel.ReportServerCredential = this.ReportServerCredential;
            this.ReportModel.ReportServerFormsCredential = this.ReportServerFormsCredential;
            this.ReportModel.IsRDLC = this.ReportProcessingMode == ProcessingMode.Local;
            this.ReportModel.ProcessReport();
        }

#if WINRT
        internal static Color GetColorFromHexa(string colorValue)
        {
            if (colorValue.ToLower() == "#00ffffff" || colorValue.ToLower()=="transparent")
            {
                return Color.FromArgb(255, 255, 255, 255);
            }

            if (colorValue.StartsWith("#"))
            {
                colorValue = colorValue.Replace("#", string.Empty);
                byte r = (byte)(Convert.ToUInt32(colorValue.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(colorValue.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(colorValue.Substring(4, 2), 16));
                return Color.FromArgb(255, r, g, b);

            }
            else
            {
                return GetColorFromHexa(Syncfusion.RDL.Internal.ReportingBrushConverter.colors[colorValue]);
            }
        }

        internal static Windows.UI.Color GetUIColorFromHexa(string colorValue)
        {
            try
            {
                if (colorValue.ToLower() == "#00ffffff" || colorValue.ToLower() == "transparent")
                {
                    return Windows.UI.Colors.Transparent;
                }

                if (!colorValue.StartsWith("#"))
                {
                     colorValue = Syncfusion.RDL.Internal.ReportingBrushConverter.colors[colorValue];
                }
                colorValue = colorValue.Replace("#", string.Empty);
                byte r = (byte)(Convert.ToUInt32(colorValue.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(colorValue.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(colorValue.Substring(4, 2), 16));

                return Windows.UI.Color.FromArgb(255, r, g, b);
            }
            catch
            {
               return Windows.UI.Colors.Transparent;
            }
        }

        internal static Syncfusion.DocIO.DLS.Color GetColorFromString(string colorValue)
        {
            if (colorValue.StartsWith("#"))
            {
                colorValue = colorValue.Replace("#", string.Empty);
                byte r = (byte)(Convert.ToUInt32(colorValue.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(colorValue.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(colorValue.Substring(4, 2), 16));
                return Syncfusion.DocIO.DLS.Color.FromArgb(255, r, g, b);
            }
            else
            {
                return GetColorFromString(Syncfusion.RDL.Internal.ReportingBrushConverter.colors[colorValue]);
            }
        }

#elif SILVERLIGHT
        internal static Dictionary<string, System.Windows.Media.Color> FetchColors()
        {
            // This could be simplified with LINQ.
            Dictionary<string, System.Windows.Media.Color> ret = new Dictionary<string, System.Windows.Media.Color>();
            foreach (var property in ReportingBrushConverter.colors)
            {
                ret[property.Key] = GetColorFromHexa(property.Value);
            }
            return ret;
        }

        internal static Dictionary<string, Syncfusion.Pdf.Graphics.PdfBrush> FetchColorsBrushs()
        {
            // This could be simplified with LINQ.
            Dictionary<string, Syncfusion.Pdf.Graphics.PdfBrush> ret = new Dictionary<string, Syncfusion.Pdf.Graphics.PdfBrush>();
            foreach (var property in ReportingBrushConverter.colors)
            {
                ret[property.Key] = new Syncfusion.Pdf.Graphics.PdfSolidBrush(new Pdf.Graphics.PdfColor(GetColorFromHexa(property.Value)));
            }

            return ret;
        }

        internal static System.Windows.Media.Color GetColorFromHexa(string colorValue)
        {
            try
            {
                if (colorValue.ToLower() == "#00ffffff" || colorValue.ToLower() == "transparent")
                {
                    return System.Windows.Media.Colors.Transparent;
                }


                if (colorValue.StartsWith("#"))
                {
                    colorValue = colorValue.Replace("#", string.Empty);
                    byte r = (byte)(Convert.ToUInt32(colorValue.Substring(0, 2), 16));
                    byte g = (byte)(Convert.ToUInt32(colorValue.Substring(2, 2), 16));
                    byte b = (byte)(Convert.ToUInt32(colorValue.Substring(4, 2), 16));
                    return System.Windows.Media.Color.FromArgb(255, r, g, b);

                }
                else
                {
                    return GetColorFromHexa(ReportingBrushConverter.colors[colorValue]);
                }
            }
            catch
            {
                return System.Windows.Media.Colors.Transparent;
            }
        }
#else
        internal static Color GetColorFromHexa(string colorValue)
        {
            if (colorValue!=null && colorValue.StartsWith("#"))
            {
                return System.Drawing.ColorTranslator.FromHtml(colorValue); 
            }
            else if (colorValue != null)
            {
                if (colorValue.ToLower() == "lightgrey")
                    return Color.LightGray;
                return Color.FromName(colorValue);
            }
            else
            {
                return Color.Transparent;
            }
        }
#endif



#if !SILVERLIGHT
        ColorConverter colorConverter = new ColorConverter();

        internal System.Drawing.Color ConvertStringToColor(string colorValue)
        {
            Color color = new Color();

            if (colorValue == "LightGrey")
            {
                colorValue = "LightGray";
            }
            if (colorValue == null)
            {
                return Color.Empty;
            }

            if (colorValue.StartsWith("#"))
            {
                try
                {
                    color = System.Drawing.ColorTranslator.FromHtml(colorValue);
                    return color;
                }
                catch
                {
                    colorValue=colorValue.Remove(0,1);
                    color = System.Drawing.ColorTranslator.FromHtml(colorValue);
                    return color;
                }
            }
            else
            {
                colorValue = "#" + colorValue;
                try
                {
                    return (Color)colorConverter.ConvertFromInvariantString(colorValue);
                }
                catch
                {
                    try
                    {
                        color = System.Drawing.ColorTranslator.FromHtml(colorValue);
                        return color;
                    }
                    catch
                    {
                        colorValue = colorValue.Remove(0, 1);
                        color = System.Drawing.ColorTranslator.FromHtml(colorValue);
                        return color;
                    }
                }
            }
        }
#endif

        #endregion Internal Methods

    }

    /// <summary>
    /// ReportWriter class to Export the Report.
    /// </summary>
    /// <remarks></remarks>
    public class ReportWriter : WriterBase
    {
        #region Initializer/Finalizer
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ReportWriter()
        {
            DataSources = new ReportDataSourceCollection();
            this.ReportModel.SubreportProcessing += new SubreportProcessingEventHandler(ReportModel_SubreportProcessing);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <param name="rdlFilename">The filename of the report with its full path</param>
        /// <remarks></remarks>
        public ReportWriter(string rdlFilename)
            : this()
        {
            if (rdlFilename == null || rdlFilename.Length == 0 || rdlFilename == string.Empty)
            {
                throw new ArgumentException("Filename should be null or empty");
            }

            ReportPath = rdlFilename;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <param name="rdlStream"></param>
        /// <remarks></remarks>
        public ReportWriter(Stream rdlStream)
            : this()
        {
            if (rdlStream == null || rdlStream.Length == 0)
            {
                throw new ArgumentException("Stream is null or empty");
            }
            rdlStream.Position = 0;
            LoadReport(rdlStream);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <param name="rdlFilename">The filename of the report with its full path</param>
        /// <param name="reportDataSources"></param>
        /// <remarks></remarks>
        public ReportWriter(string rdlFilename, ReportDataSourceCollection reportDataSources)
            : this()
        {
            if (rdlFilename == null || rdlFilename.Length == 0 || rdlFilename == string.Empty)
            {
                throw new ArgumentException("Filename should be null or empty");
            }

            if (reportDataSources == null)
            {
                throw new ArgumentException("Datasource should be null");
            }

            ReportPath = rdlFilename;
            DataSources = reportDataSources;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <param name="rdlStream"></param>
        /// <param name="reportDataSources"></param>
        /// <remarks></remarks>
        public ReportWriter(Stream rdlStream, ReportDataSourceCollection reportDataSources)
            : this()
        {
            if (rdlStream == null || rdlStream.Length == 0)
            {
                throw new ArgumentException("Stream is null or empty");
            }

            if (reportDataSources == null)
            {
                throw new ArgumentException("Datasource should be null");
            }

            rdlStream.Position = 0;
            LoadReport(rdlStream);
            DataSources = reportDataSources;
        }

        #endregion Initializer/Finalizer

        #region Public Methods

        /// <summary>
        /// Saves the document into a HTTP response stream.
        /// </summary>
        /// <param name="fileName">File name to save the Report</param>
        /// <param name="format">Format to save the Report</param>
        /// <param name="response">The HTTP response stream object.</param>
        /// <remarks></remarks> 
#if !SILVERLIGHT
        public void Save(string fileName, WriterFormat format, System.Web.HttpResponse response)
        {
            if (this.ReportModel.HasReport)
            {

                if (format == WriterFormat.Word)
                {
                    WordWriter wordWriter = new WordWriter();
                    wordWriter.DataSources = this.DataSources;
                    wordWriter.ReportModel = this.ReportModel;
                    wordWriter.Save(fileName, response);
                }
                else if (format == WriterFormat.Excel)
                {
                    ExcelWriter excelWriter = new ExcelWriter();
                    excelWriter.DataSources = this.DataSources;
                    excelWriter.ReportModel = this.ReportModel;
                    excelWriter.Save(fileName, response);
                }
                 else if (format == WriterFormat.HTML)
                {
                    HtmlWriter htmlWriter = new HtmlWriter();
                    htmlWriter.DataSources = this.DataSources;
                    htmlWriter.ReportModel = this.ReportModel;
                    htmlWriter.Save(fileName, response);
                    response.End();
                }
        
                else if (format == WriterFormat.PDF)
                {
                    PdfWriter pdfWriter = new PdfWriter();
                    pdfWriter.DataSources = this.DataSources;
                    pdfWriter.ReportModel = this.ReportModel;
                    if (this.ReportModel != null)
                    {
                        pdfWriter.EnablePDFSplitMerge = this.ReportModel.EnableVirtualEvaluation;
                    }
                    pdfWriter.EnablePDFSplitMerge = this.EnablePDFSplitMerge;
                    pdfWriter.Save(fileName, response);
                }

            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }
#endif

#if !WINRT 
        /// <summary>
        /// Exports the report as a PDF document
        /// </summary>
        /// <param name="filename">The name of the Pdf file to be saved</param>
        /// <param name="format">The writer format to be saved</param>
        public void Save(string filename,WriterFormat format)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            {
                Save(fileStream,format);
            }
        }
#endif

        /// <summary>
        /// Exports the report as a PDF document
        /// </summary>
        /// <param name="stream">The stream where the Pdf document to be saved</param>
        /// <param name="format">The writer format to be saved</param>
        /// <remarks></remarks>
        public void Save(Stream stream,WriterFormat format)
        {
#if SILVERLIGHT 
            if (this.ExportMode == Syncfusion.ReportWriter.ExportMode.Server && this.ReportServiceURL != null)
            {
                this.ReportModel.ReportingServer.ExportCompleted += (sender, arg) =>
                {
                    try
                    {
                        stream.Write(arg.Result, 0, arg.Result.Length);
#if WINRT 
                        stream.Dispose();
#else
                        stream.Close();
#endif
                    }
                    catch
                    {
#if WINRT
                        stream.Dispose();
#else
                        stream.Close();
#endif
                    }
                };
                this.ExportFromService(format, stream);
            }
#endif
#if SILVERLIGHT
            else if (this.ReportModel.HasReport && this.ExportMode == Syncfusion.ReportWriter.ExportMode.Local)
#else
            if (this.ReportModel.HasReport)
#endif
            {
                try
                {
                    if (format == WriterFormat.Word)
                    {
                        WordWriter wordWriter = new WordWriter();
                        wordWriter.DataSources = this.DataSources;
                        wordWriter.ReportModel = this.ReportModel;
                        wordWriter.WordFormatType = this.WordFormatType;
                        wordWriter.Save(stream);
                        wordWriter.ReportModel = null;
                        wordWriter.DataSources = null;
                    }

                    else if (format == WriterFormat.Excel)
                    {
                        ExcelWriter excelWriter = new ExcelWriter();
                        excelWriter.DataSources = this.DataSources;
                        excelWriter.ReportModel = this.ReportModel;
                        excelWriter.ExcelVersion = this.ExcelVersion;
                        excelWriter.Save(stream);
                        excelWriter.ReportModel = null;
                        excelWriter.DataSources = null;
                    }
                    else if (format == WriterFormat.PDF)
                    {
                        PdfWriter pdfWriter = new PdfWriter();
                        pdfWriter.DataSources = this.DataSources;
                        pdfWriter.ReportModel = this.ReportModel;
                        pdfWriter.PDFSplitPageCount = this.PDFSplitPageCount;
                        pdfWriter.EnablePDFSplitMerge = this.EnablePDFSplitMerge;
                        pdfWriter.PDFTempPath = this.PDFTempPath;
                        pdfWriter.PageSettings = this.PageSettings;
                        pdfWriter.PDFFonts = this.PDFFonts;
                        pdfWriter.Save(stream);
                        pdfWriter.DataSources = null;
                        pdfWriter.ReportModel = null;
                        pdfWriter.PDFTempPath = null;
                        pdfWriter.PageSettings = null;
                    }
                    else if (format == WriterFormat.HTML)
                    {
                        HtmlWriter htmlWriter = new HtmlWriter();
                        htmlWriter.DataSources = this.DataSources;
                        htmlWriter.ReportModel = this.ReportModel;
                        htmlWriter.Save(stream);
                        htmlWriter.DataSources = null;
                        htmlWriter.ReportModel = null;
                    }

                    if (this.ExportCompleted != null)
                    {
                        this.ExportCompleted(this, ReadFully(stream));
                    }

                    if (this.ReportModel.ExceptionDetails != null && this.ReportModel.ExceptionDetails.Count > 0 && this.ReportError != null)
                    {
                        this.ReportError(this, new ReportErrorEventArgs { Error = this.ReportModel.ExceptionDetails });
                    }
                    if (this.SubreportProcessing != null)
                    {
                        this.ReportModel.SubreportProcessing -= new SubreportProcessingEventHandler(ReportModel_SubreportProcessing);
                    }
                }
                catch
                {

                }
            }
        }

#if WINRT 
        async public System.Threading.Tasks.Task<bool> SaveASync(Stream stream, WriterFormat format)
        {
            try
            {
                if (format == WriterFormat.Word)
                {
                    WordWriter wordWriter = new WordWriter();
                    wordWriter.DataSources = this.DataSources;
                    wordWriter.ReportModel = this.ReportModel;
                    wordWriter.Save(stream);
                    wordWriter.ReportModel = null;
                    wordWriter.DataSources = null;
                }

                else if (format == WriterFormat.Excel)
                {
                    ExcelWriter excelWriter = new ExcelWriter();
                    excelWriter.DataSources = this.DataSources;
                    excelWriter.ReportModel = this.ReportModel;
                    await excelWriter.Save(stream);
                    excelWriter.ReportModel = null;
                    excelWriter.DataSources = null;
                }
                else if (format == WriterFormat.PDF)
                {
                    PdfWriter pdfWriter = new PdfWriter();
                    pdfWriter.DataSources = this.DataSources;
                    pdfWriter.ReportModel = this.ReportModel;
                    pdfWriter.PDFSplitPageCount = this.PDFSplitPageCount;
                    pdfWriter.EnablePDFSplitMerge = this.EnablePDFSplitMerge;
                    pdfWriter.PDFTempPath = this.PDFTempPath;
                    pdfWriter.PageSettings = this.PageSettings;
                    pdfWriter.PDFFonts = this.PDFFonts;
                    pdfWriter.Save(stream);
                    pdfWriter.DataSources = null;
                    pdfWriter.ReportModel = null;
                    pdfWriter.PDFTempPath = null;
                    pdfWriter.PageSettings = null;
                }
                else if (format == WriterFormat.HTML)
                {
                    HtmlWriter htmlWriter = new HtmlWriter();
                    htmlWriter.DataSources = this.DataSources;
                    htmlWriter.ReportModel = this.ReportModel;
                    htmlWriter.Save(stream);
                    htmlWriter.DataSources = null;
                    htmlWriter.ReportModel = null;
                }

                if (this.ExportCompleted != null)
                {
                    this.ExportCompleted(this, ReadFully(stream));
                }

                if (this.ReportModel.ExceptionDetails != null && this.ReportModel.ExceptionDetails.Count > 0 && this.ReportError != null)
                {
                    this.ReportError(this, new ReportErrorEventArgs { Error = this.ReportModel.ExceptionDetails });
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

#endif

        internal static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            input.Position = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

#if SILVERLIGHT
         void ExportFromService(WriterFormat format,Stream stream)
         {
            this.ReportModel.ReportServiceURL = this.ReportServiceURL;

            Syncfusion.Reports.Server.ReportSetting reportSetting = new Syncfusion.Reports.Server.ReportSetting() { ReportPath = this.ReportModel.ReportPath, ReportServerURL = this.ReportModel.ReportServerUrl };

            if (this.ReportModel.ReportServerCredential != null)
            {
                reportSetting.ReportServerCredential = this.ReportModel.GetReportServerCredential();
            }

            if (this.ReportModel.ReportServerFormsCredential != null)
            {
                reportSetting.ReportServerFormCredential = this.ReportModel.GetReportServerFormCredential();
            }

            if (string.IsNullOrEmpty(this.ReportPath) && this.ReportModel.ReportStreamByte != null)
            {
                reportSetting.Report = this.ReportModel.ReportStreamByte;
            }

            List<Syncfusion.Reports.Server.ReportParameterInfo> parametersInfo = new List<Syncfusion.Reports.Server.ReportParameterInfo>();
            ReportParameterInfoCollection parameterInfo = this.GetParameters();

            foreach (var parameter in parameterInfo)
            {
                Syncfusion.Reports.Server.ReportParameterInfo paramInfo = new Syncfusion.Reports.Server.ReportParameterInfo();
                paramInfo.Name = parameter.Name;

#if WINRT 
                if (paramInfo.Labels == null)
                {
                    paramInfo.Labels = new ObservableCollection<string>();
                }
                if (paramInfo.Values == null)
                {
                    paramInfo.Values = new ObservableCollection<string>();
                }
                paramInfo.Labels.Add(parameter.Labels.Count.ToString());
                paramInfo.Values.Add(parameter.Values.Count.ToString());

#else

                paramInfo.Labels = new string[parameter.Labels.Count];
                paramInfo.Values = new string[parameter.Values.Count];
#endif
                int index = 0;
                foreach (var value in parameter.Values)
                {
                    paramInfo.Values[index++] = value;
                }

                index = 0;

                foreach (var label in parameter.Labels)
                {
                    paramInfo.Labels[index++] = label;
                }

                parametersInfo.Add(paramInfo);
            }
#if WINRT 
            reportSetting.Parameters = new ObservableCollection<Syncfusion.Reports.Server.ReportParameterInfo>();

            foreach (Syncfusion.Reports.Server.ReportParameterInfo info in parametersInfo)
            {
                reportSetting.Parameters.Add(info);
            }
#else
            reportSetting.Parameters = parametersInfo.ToArray();
#endif
            if (this.ReportProcessingMode == ProcessingMode.Remote && this.DataSources.Count==0)
            {

                List<Syncfusion.Reports.Server.DataSourceCredentialsInfo> credentialsInfo = new List<Syncfusion.Reports.Server.DataSourceCredentialsInfo>();

                if ( this.ReportModel.Report!=null && this.ReportModel.Report.DataSources != null)
                {
                    foreach (var dataSource in this.ReportModel.Report.DataSources)
                    {
                        Syncfusion.Reports.Server.DataSourceCredentialsInfo dataSourceInfo = new Syncfusion.Reports.Server.DataSourceCredentialsInfo();
                        dataSourceInfo.Name = dataSource.Name;
                        dataSourceInfo.UserId = dataSource.ConnectionProperties.UserName;
                        dataSourceInfo.Password = dataSource.ConnectionProperties.PassWord;
                        dataSourceInfo.IntegratedSecurity = dataSource.ConnectionProperties.IntegratedSecurity;
                        credentialsInfo.Add(dataSourceInfo);
                    }
                }
#if WINRT 
                reportSetting.DataSourceCredentials = new ObservableCollection<Syncfusion.Reports.Server.DataSourceCredentialsInfo>();
                foreach (var data in this.DataSourceCredentials)
                {
                    reportSetting.DataSourceCredentials.Add(new Reports.Server.DataSourceCredentialsInfo { Name = data.Name, UserId = data.UserId, Password = data.Password });
                }

#else
                if (this.DataSourceCredentials != null)
                {
                    foreach (var data in this.DataSourceCredentials)
                    {
                        credentialsInfo.Add(new Reports.Server.DataSourceCredentialsInfo { Name = data.Name, UserId = data.UserId, Password = data.Password });
                    }
                }
                   reportSetting.DataSourceCredentials = credentialsInfo.ToArray();

#endif
            }
            else
            {
                List<Syncfusion.Reports.Server.ReportDataSource> listDatasourceinfo = new List<Syncfusion.Reports.Server.ReportDataSource>();
                Dictionary<string, IEnumerable> datasourceCollection = new Dictionary<string, IEnumerable>();
                foreach (var Data in DataSources)
                {
                    Syncfusion.Reports.Server.ReportDataSource datasourceinfo = new Syncfusion.Reports.Server.ReportDataSource();
                    datasourceinfo.Name = Data.Name;
#if WINRT 
                    if (datasourceinfo.Value == null)
                    {
                        datasourceinfo.Value = new ObservableCollection<Syncfusion.Reports.Server.ReportData>();
                    }

                    foreach (Syncfusion.Reports.Server.ReportData data in this.GetWrapperDataSource(Data.Value))
                    {
                        datasourceinfo.Value.Add(data);
                    }
#else
                    datasourceinfo.Value = this.GetWrapperDataSource(Data.Value as IEnumerable).ToArray();

#endif
                    listDatasourceinfo.Add(datasourceinfo);
                }
#if WINRT 
                reportSetting.DataSources = new ObservableCollection<Syncfusion.Reports.Server.ReportDataSource>();
                foreach (Syncfusion.Reports.Server.ReportDataSource data in listDatasourceinfo)
                {
                    reportSetting.DataSources.Add(data);
                }

#else
                reportSetting.DataSources = listDatasourceinfo.ToArray();
#endif
            }
#if WINRT 
            this.ReportModel.ReportingServer.ExportAsync(reportSetting, format.ToString());

#else
            this.ReportModel.ReportingServer.Export(reportSetting, format.ToString());
#endif

         }
#endif


#if WINRT 
         List<Syncfusion.Reports.Server.ReportData> GetWrapperDataSource(IEnumerable Datasoure)
         {
             List<Syncfusion.Reports.Server.ReportData> dataSource1 = new List<Syncfusion.Reports.Server.ReportData>();
             IEnumerable list = Datasoure;
             foreach (object o in list)
             {
                 Syncfusion.Reports.Server.ReportData data = new Syncfusion.Reports.Server.ReportData();
                 data.Data = new Dictionary<string, object>();
                 System.Type objectType = o.GetType();
                 IList<PropertyInfo> props = new List<PropertyInfo>(objectType.GetRuntimeProperties());
                 foreach (PropertyInfo prop in props)
                 {

                     bool isField = false;
                     Syncfusion.RDL.DOM.DataSets info = this.ReportModel.Report.DataSets;

                     foreach (Syncfusion.RDL.DOM.DataSet dataset in info)
                     {
                         foreach (Syncfusion.RDL.DOM.Field fields in dataset.Fields)
                         {
                             if ((fields.DataField == prop.Name) || fields.Name == prop.Name)
                             {
                                 isField = true;
                             }
                         }
                     }

                     if (isField)
                     {
                         string propValue = prop.Name;
                         object getObjectValue = prop.GetValue(o, null);
                         data.Data.Add(propValue, getObjectValue);
                     }
                 }
                 dataSource1.Add(data);
             }
             return dataSource1;
         }
#elif SILVERLIGHT

         List<Syncfusion.Reports.Server.ReportData> GetWrapperDataSource(IEnumerable Datasoure)
        {
            List<Syncfusion.Reports.Server.ReportData> dataSource1 = new List<Syncfusion.Reports.Server.ReportData>();
            IEnumerable list = Datasoure;
            foreach (object o in list)
            {
                Syncfusion.Reports.Server.ReportData data = new Syncfusion.Reports.Server.ReportData();
                data.Data = new Dictionary<string, object>();
                System.Type objectType = o.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(objectType.GetProperties());
                foreach (PropertyInfo prop in props)
                {
                    bool isField = false;
                    Syncfusion.RDL.DOM.DataSets info = this.ReportModel.Report.DataSets;

                    foreach (Syncfusion.RDL.DOM.DataSet dataset in info)
                    {
                        foreach (Syncfusion.RDL.DOM.Field fields in dataset.Fields)
                        {
                            if (fields.Name == prop.Name || fields.DataField==prop.Name)
                            {
                                isField = true;
                            }
                        }
                    }

                    if (isField)
                    {
                        string propValue = prop.Name;
                        object getObjectValue = prop.GetValue(o, null);
                        data.Data.Add(propValue, getObjectValue);
                    }
                }
                dataSource1.Add(data);
            }

            return dataSource1;
        }
#endif

        #endregion Public Methods

        void ReportModel_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            RaiseSubReportProcessingEvent(e);
        }

        /// <summary>
        /// RaiseSubReportProcessingEvent
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.Windows.Reports.SubreportProcessingEventArgs">SubreportProcessingEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        protected virtual void RaiseSubReportProcessingEvent(SubreportProcessingEventArgs e)
        {
            if (this.SubreportProcessing != null)
            {
                this.SubreportProcessing(this, e);
            }
        }

        #region Public Events

        /// <summary>
        /// ReportErrorEvents argument
        /// </summary>
        /// <remarks></remarks>
        public class ReportErrorEventArgs : System.EventArgs
        {
            /// <summary>
            /// Gets or sets errors.
            /// </summary>
            /// <value>List of errors</value>
            /// <remarks></remarks>
            public List<string> Error
            {
                get;
                set;
            }
        }

        /// <summary>
        /// delegate of ReportErrorHandler
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Syncfusion.ReportWriter.ReportWriter.ReportErrorEventArgs">ReportErrorEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        public delegate void ReportErrorHandler(object sender, ReportErrorEventArgs e);

        /// <summary>
        /// Occurs when error in report. 
        /// </summary>
        /// <remarks></remarks>
        public event ReportErrorHandler ReportError;

        /// <summary>
        /// delegate of ExportByteCompletedEventHandler
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Syncfusion.ReportWriter.ReportWriter.ReportErrorEventArgs">ReportExportEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        public delegate void ExportByteCompletedEventHandler(object sender, byte[] e);

        /// <summary>
        /// Occurs when export report completed. 
        /// </summary>
        /// <remarks></remarks>
        public event ExportByteCompletedEventHandler ExportCompleted;

        /// <summary>
        /// Occurs when SubreportProcessing. 
        /// </summary>
        /// <remarks></remarks>
        public event SubreportProcessingEventHandler SubreportProcessing;

        #endregion

    }
}
