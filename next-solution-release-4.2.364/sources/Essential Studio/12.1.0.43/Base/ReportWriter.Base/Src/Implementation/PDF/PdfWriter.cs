#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System.Threading;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.DOM;

#if SILVERLIGHT &&!WINRT
using System.Windows.Media;
#endif

#if WINRT 
using Syncfusion.UI.Xaml.Reports;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#else
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Reports;
using System.Windows;

#endif


namespace Syncfusion.ReportWriter
{

    /// <summary>
    /// Exports the Report in Pdf mode .
    /// </summary>
    /// <remarks></remarks>
    public class PdfWriter : WriterBase
    {
        #region fields
        PdfDocument document = null;
        PdfPage currentPage = null;
        PdfSection currentSection = null;
        string backColorname = string.Empty;
        string borderColorname = string.Empty;
        float parentX = 0f, parentY = 0f;
        double parentLeft = 0f, parentTop = 0f;
        private Stack<object> parentModel = new Stack<object>();
        int m_currentPageNumber;
        PageModelFactory m_pageModelFactory;
        private int m_position = 0;
        private PdfTextLayoutResult result;
        //private PdfPageTemplateElement Header = null;
        //private PdfPageTemplateElement Footer = null;
        private bool IsHeaderFooterTemplate;
        private bool m_bIsInList;
        //private double incTop = 0;
        //private double incLeft = 0;

#if SILVERLIGHT
        Dictionary<string, string> colorcollections = ReportingBrushConverter.colors;
     
#endif
#if WINRT 
        System.Drawing.Color backcolor;
        System.Drawing.Color borderColor;
#elif SILVERLIGHT
        System.Windows.Media.Color backcolor;
        System.Windows.Media.Color borderColor;

#else
        System.Drawing.Color backcolor;
        System.Drawing.Color borderColor;
#endif

        string colorName; 
#if WINRT 
        Dictionary<string, Color> colorValues;
        Dictionary<string, PdfBrush> brushValues;
#elif SILVERLIGHT
        Dictionary<string, System.Windows.Media.Color> colorValues=FetchColors();
        Dictionary<string, PdfBrush> brushValues = FetchColorsBrushs();
#else
        Dictionary<string, System.Drawing.Color> colorValues=new Dictionary<string,Color>();
        Dictionary<string, PdfBrush> brushValues=new Dictionary<string,PdfBrush>();
#endif

        Dictionary<string, PdfTrueTypeFont> fontValues;

        private TablixCellInfo m_currentListCellInfo = null;
        private Dictionary<Guid, int> ListRowIndexCollection = new Dictionary<Guid, int>();
        private bool isWhite=false;

        #endregion fields

        #region Initializer/Finalizer
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public PdfWriter()
        {
            DataSources = new ReportDataSourceCollection();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <param name="rdlFilename">The filename of the report with its full path</param>
        /// <remarks></remarks>
        public PdfWriter(string rdlFilename)
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
        public PdfWriter(Stream rdlStream)
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
        /// <param name="reportDataSources">DataSource of the Report</param>
        /// <remarks></remarks>
        public PdfWriter(string rdlFilename, ReportDataSourceCollection reportDataSources)
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
        public PdfWriter(Stream rdlStream, ReportDataSourceCollection reportDataSources)
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
#if !SILVERLIGHT
        /// <summary>
        /// Exports the Report.
        /// </summary>
        /// <param name="fileName">File name to save the Report.</param>
        /// <param name="response">Http Response</param>
        /// <remarks></remarks>
        public void Save(string fileName, System.Web.HttpResponse response)
        {
            if (this.ReportModel.HasReport)
            {
                PdfDocument pdfDoc = null;

                Thread thread = new Thread(delegate()
                {
                    PageModelFactory pageModelFactory = UpdatePageLayoutForPDF(ReportDefinition, ReportModel);
                    PdfWriter converter = new PdfWriter();
                    converter.EnablePDFSplitMerge = this.EnablePDFSplitMerge;
                    pdfDoc = converter.ConvertToPDF(pageModelFactory, ReportModel);
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                thread.Join();
                pdfDoc.Save(fileName, response, HttpReadType.Save);
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
        /// <param name="PdfFilename">The name of the Pdf file to be saved</param>
        public void Save(string PdfFilename)
        {
            using (FileStream pdfStream = new FileStream(PdfFilename, FileMode.Create))
            {
                Save(pdfStream);
            }
        }
#endif

        /// <summary>
        /// Exports the report as a PDF document
        /// </summary>
        /// <param name="PdfStream">The stream where the Pdf document to be saved</param>
        /// <remarks></remarks>
        public void Save(Stream PdfStream)
        {
            if (this.ReportModel.HasReport)
            {
                PageModelFactory pageModelFactory = UpdatePageLayoutForPDF(ReportDefinition, ReportModel);
                PdfStream.Position = 0;
                PdfDocument pdfDoc = null;
                PdfWriter converter = new PdfWriter();
                converter.PDFSplitPageCount = this.PDFSplitPageCount;
                if (!this.EnablePDFSplitMerge)
                {
                    this.EnablePDFSplitMerge = this.ReportModel.EnableVirtualEvaluation;
                }
                converter.EnablePDFSplitMerge = this.EnablePDFSplitMerge;
                converter.PDFFonts = this.PDFFonts;
                pdfDoc = converter.ConvertToPDF(pageModelFactory, ReportModel);
                if(pdfDoc != null)
                {
                    pdfDoc.Save(PdfStream);
                }
            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }
        #endregion Public Methods

        /// <summary>
        /// Converts the PageModelFactory into its PDF document
        /// </summary>
        /// <param name="pageModelFactory">The pageModelFactory of the report</param>
        /// <param name="report">ReportModel of the Report</param>
        /// <returns>Returns its equivalent PDF document</returns>
        /// <remarks></remarks>
        internal PdfDocument ConvertToPDF(PageModelFactory pageModelFactory, ReportModel report)
        {
#if  WINRT
            this.colorValues = new Dictionary<string, Color>();
            if (colorValues == null)
            {
                colorValues = new Dictionary<string, Color>();
            }
            foreach (var value in this.colorcollections)
            {
                colorValues.Add(value.Key, GetColorFromHexa(value.Value));
            }


#elif SILVERLIGHT
            this.colorValues = FetchColors();
#endif
            if (this.brushValues == null)
            {

                this.brushValues = new Dictionary<string, PdfBrush>();
            }
            this.fontValues = new Dictionary<string, PdfTrueTypeFont>();
            this.GetBrushColor("Transparent");
            int totalPages = pageModelFactory.PrintLayoutPageDictionary.Count;
            List<string> documents = new List<string>();
#if SILVERLIGHT
            string tempPath = this.PDFTempPath;
#else
            string tempPath = string.IsNullOrEmpty(this.PDFTempPath) ? Path.GetTempPath() : this.PDFTempPath;
#endif

            int tempindex = 0;

            double height = report.Page.PageHeight.PixelValue;
            double width = report.Page.PageWidth.PixelValue;
            double headerOffset = 0;
            double bodyOffset = height;

            if (report.Page.PageHeader != null)
            {
                //height += pageModelFactory.HeaderHeight;
                headerOffset += pageModelFactory.HeaderHeight;
            }

            //if (report.Page.PageFooter != null)
            //{
            //    height += pageModelFactory.FooterHeight;
            //}

            m_pageModelFactory = pageModelFactory;

            for (int curPage = 0; curPage < totalPages; )
            { 
                if (this.document == null)
                {
                    document = new PdfDocument();
                    document.PageSettings.Margins.Top = PixelToPoint(pageModelFactory.Margin.Top);
                    document.PageSettings.Margins.Bottom = PixelToPoint(pageModelFactory.Margin.Bottom);
                    document.PageSettings.Margins.Left = PixelToPoint(pageModelFactory.Margin.Left);
                    document.PageSettings.Margins.Right = PixelToPoint(pageModelFactory.Margin.Right);

                    PdfSection section = document.Sections.Add();
                    section.PageSettings.Height = PixelToPoint(height);
                    section.PageSettings.Width = PixelToPoint(width);

                    this.currentSection = section;
                }

                var pageModel = pageModelFactory.PrintLayoutPageDictionary[curPage];         
                currentPage = this.currentSection.Pages.Add() as PdfPage;

                m_currentPageNumber++;

                if (report.Page.PageHeader != null)
                {
                    this.AddHeader(pageModelFactory, report);
                }

                this.parentModel.Push(new PointF(0, (float)headerOffset));
                this.UpdateParentOffset();

                foreach (var reportModel in pageModel.ReportModelCollection)
                {
                    ProcessReportModel(reportModel, curPage);
                }

                this.ClearParentOffset();

                if (report.Page.PageFooter != null)
                {
                    float bottom = Convert.ToSingle((currentPage.Graphics.ClientSize.Height / 0.75) - pageModelFactory.FooterHeight);
                    this.parentModel.Push(new PointF(0f, bottom));
                    this.UpdateParentOffset();
                    AddFooter(pageModelFactory, report);
                    this.ClearParentOffset();
                }

                curPage++;

                if (this.EnablePDFSplitMerge && this.PDFSplitPageCount>0 && curPage % this.PDFSplitPageCount == 0 && curPage != totalPages)
                {
                    string name = Path.Combine(tempPath, "SyncTempPDF" + tempindex++ + ".pdf");
                    documents.Add(name);
#if !SILVERLIGHT
                    this.document.Save(name);
#endif

                    this.document.Close();
                    this.document = null;
                    this.currentSection = null;
                }
            }

 #if !SILVERLIGHT

            if (this.EnablePDFSplitMerge)
            {

                if (this.document != null)
                {
                    string name = Path.Combine(tempPath, "SyncTempPDF" + tempindex++ + "Last.pdf");
                    documents.Add(name);
                    this.document.Save(name);

                    this.document.Close();
                    this.document = null;
                }

                PdfDocument doc = PdfDocument.Merge(documents.ToArray());
                foreach (var file in documents)
                {
                    File.Delete(file);
                }

                documents.Clear();
                return doc;
            }
#endif

            this.brushValues.Clear();
            this.brushValues = null;

            if (colorValues != null)
            {
                this.colorValues.Clear();
            }
            this.colorValues = null;

            this.fontValues.Clear();
            this.fontValues = null;

            this.currentSection = null;

            return document;
        }
        /// <summary>
        /// Process the footer element
        /// </summary>
        /// <param name="pageModelFactory">The PageModelFactory</param>
        /// <param name="report">The ReportModel</param>
        /// <remarks></remarks>
        private void AddFooter(PageModelFactory pageModelFactory, ReportModel report)
        {
            if (report.Page.PageFooter == null && pageModelFactory.FooterHeight <=0)
                return;
            
            if (pageModelFactory.Model.FooterBehaviour != null && pageModelFactory.Model.FooterBehaviour.Border != null)
            {
                this.ApplyFooterBorder(pageModelFactory, report);
            }

            IsHeaderFooterTemplate = true;

            PdfGraphics graphics = this.currentPage.Graphics;

            if (!string.IsNullOrEmpty(report.FooterBehaviour.ImageValue) && report.FooterBehaviour.ImageSource == Source.Embedded)
            {
#if !WINRT 
                System.Windows.Media.ImageBrush imageBrush = GetImageBrush(report, report.FooterBehaviour.ImageValue);
#else
                Windows.UI.Xaml.Media.ImageBrush imageBrush = GetImageBrush(report, report.FooterBehaviour.ImageValue); 
#endif
                if (imageBrush != null)
                {
#if !SILVERLIGHT
                    System.Drawing.Image image = System.Drawing.Image.FromStream((imageBrush.ImageSource as BitmapImage).StreamSource as Stream);
                    image = ResizeImage(image, (float)(pageModelFactory.PageWidth - (pageModelFactory.Margin.Left + pageModelFactory.Margin.Right)), (float)pageModelFactory.FooterHeight);

                    PdfBitmap bitmap = new PdfBitmap(image);
                    bitmap.Draw(graphics, parentX, parentY);
#endif

                }
            }

            ProcessHeaderFooteritems(pageModelFactory.Model.FooterReportItemModels, graphics);
            IsHeaderFooterTemplate = false;
        }

        /// <summary>
        /// Process the header element
        /// </summary>
        /// <param name="pageModelFactory">The PageModelFactory</param>
        /// <param name="report">The ReportModel</param>
        /// <remarks></remarks>
        private void AddHeader(PageModelFactory pageModelFactory, ReportModel report)
        {
            if (report.Page.PageHeader == null)
                return;

            if (pageModelFactory.HeaderHeight > 0.0)
            {
                PdfGraphics graphics = this.currentPage.Graphics;

                if (pageModelFactory.Model.HeaderBehaviour!=null && pageModelFactory.Model.HeaderBehaviour.Border != null)
                {
                    this.ApplyHeaderBorder(pageModelFactory, report);
                }

                IsHeaderFooterTemplate = true;

                if (!string.IsNullOrEmpty(report.HeaderBehaviour.ImageValue) && report.HeaderBehaviour.ImageSource == Source.Embedded)
                {
#if SILVERLIGHT
                    ImageBrush imageBrush = GetImageBrush(report, report.HeaderBehaviour.ImageValue);
#else
                    System.Windows.Media.ImageBrush imageBrush = GetImageBrush(report, report.HeaderBehaviour.ImageValue);
#endif
                    if (imageBrush != null)
                    {
#if !SILVERLIGHT
                        System.Drawing.Image image = System.Drawing.Image.FromStream((imageBrush.ImageSource as BitmapImage).StreamSource as Stream);
                        image = ResizeImage(image, (float)(pageModelFactory.PageWidth - (pageModelFactory.Margin.Left + pageModelFactory.Margin.Right)), (float)(pageModelFactory.HeaderHeight));

                        PdfBitmap bitmap = new PdfBitmap(image);
                        bitmap.Draw(graphics, 0, 0);
#endif

                    }
                }                

                ProcessHeaderFooteritems(pageModelFactory.Model.HeaderReportItemModels, graphics);
                IsHeaderFooterTemplate = false;
            }
        }

        private void ApplyHeaderBorder(PageModelFactory pageModelFactory, ReportModel report)
        {
            PdfGraphics g = currentPage.Graphics;
            if (pageModelFactory.Model.HeaderBehaviour.Border != null && pageModelFactory.Model.HeaderBehaviour.Border.Default!=null && pageModelFactory.Model.HeaderBehaviour.Border.Default.BorderStyle != BorderStyles.None
                && pageModelFactory.Model.HeaderBehaviour.Border.Default.BorderStyle != BorderStyles.Default)
            {
                PdfPen pen = null;
                if (pageModelFactory.Model.HeaderBehaviour.Border.Default != null )
                {
                    pageModelFactory.Model.HeaderBehaviour.Border.Default.Thickness = pageModelFactory.Model.HeaderBehaviour.Border.Default.Thickness == 0 ? 0.75 : pageModelFactory.Model.HeaderBehaviour.Border.Default.Thickness / 1.333;
                    if (pageModelFactory.Model.HeaderBehaviour.Border.Default.BorderStyle != BorderStyles.None && pageModelFactory.Model.HeaderBehaviour.Border.Default.BorderStyle != BorderStyles.Default)
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.HeaderBehaviour.Border.Default.BorderBrush), (float)pageModelFactory.Model.HeaderBehaviour.Border.Default.Thickness);
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)pageModelFactory.Model.HeaderBehaviour.Border.Default.Thickness);
                    
                    pen.DashStyle =GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.HeaderBehaviour.Border.Default.BorderStyle.ToString(),true)); 
                    
                    //top
                    if (pageModelFactory.Model.HeaderBehaviour.Border.TopBorder == null )
                    {
                        g.DrawLine(pen, 0, 0, (float)g.ClientSize.Width, 0);
                    }
                     
                    //bottom
                    if (pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder == null )
                    {
                        g.DrawLine(pen, 0, (float)(pageModelFactory.HeaderHeight / 1.33), (float)g.ClientSize.Width, (float)(pageModelFactory.HeaderHeight / 1.33));
                    }
                   
                    //left
                    if (pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder == null)
                    {
                        g.DrawLine(pen, 0, (float)(pageModelFactory.HeaderHeight / 1.33), 0, 0);
                    }
                    
                    //right
                    if (pageModelFactory.Model.HeaderBehaviour.Border.RightBorder == null)
                    {
                        g.DrawLine(pen, (float)g.ClientSize.Width, 0, (float)g.ClientSize.Width, (float)(pageModelFactory.HeaderHeight / 1.33));
                    }
                }

                if (pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder != null )
                {

                    pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.Thickness = pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderBrush))
                            pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderBrush), (float)(pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.HeaderBehaviour.Border.BottomBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, 0, (float)(pageModelFactory.HeaderHeight / 1.33), (float)g.ClientSize.Width, (float)(pageModelFactory.HeaderHeight / 1.33));
                   
                } 
                if (pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder != null)
                {
                    pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.Thickness = pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderBrush))
                            pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderBrush), (float)(pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.HeaderBehaviour.Border.LeftBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, 0, (float)(pageModelFactory.HeaderHeight / 1.33), 0, 0);
                }

                if (pageModelFactory.Model.HeaderBehaviour.Border.TopBorder != null)
                {
                    pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.Thickness = pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderStyle != BorderStyles.None)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderBrush))
                            pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderBrush), (float)(pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.HeaderBehaviour.Border.TopBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, 0, 0, (float)g.ClientSize.Width, 0);
                }

                if (pageModelFactory.Model.HeaderBehaviour.Border.RightBorder != null)
                {
                    pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.Thickness = pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderBrush))
                            pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderBrush), (float)(pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.HeaderBehaviour.Border.RightBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, (float)g.ClientSize.Width, 0, (float)g.ClientSize.Width, (float)(pageModelFactory.HeaderHeight / 1.33));
                }
            }
        }


        private void ApplyFooterBorder(PageModelFactory pageModelFactory, ReportModel report)
        {
            PdfGraphics g = currentPage.Graphics;
            if (pageModelFactory.Model.FooterBehaviour.Border != null) 
            {
                PdfPen pen = null;
                if (pageModelFactory.Model.FooterBehaviour.Border.Default != null)
                {
                    pageModelFactory.Model.FooterBehaviour.Border.Default.Thickness = pageModelFactory.Model.FooterBehaviour.Border.Default.Thickness == 0 ? 0.75 : pageModelFactory.Model.FooterBehaviour.Border.Default.Thickness / 1.333;
                    if (pageModelFactory.Model.FooterBehaviour.Border.Default.BorderStyle != BorderStyles.None && pageModelFactory.Model.FooterBehaviour.Border.Default.BorderStyle != BorderStyles.Default)
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.FooterBehaviour.Border.Default.BorderBrush), (float)pageModelFactory.Model.FooterBehaviour.Border.Default.Thickness);
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)pageModelFactory.Model.FooterBehaviour.Border.Default.Thickness);

                    if (pageModelFactory.Model.FooterBehaviour.Border.Default.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.FooterBehaviour.Border.Default.BorderStyle.ToString(), true));

                    //top
                    if (pageModelFactory.Model.FooterBehaviour.Border.TopBorder == null)
                    {
                        g.DrawLine(pen, 0, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33), (float)g.ClientSize.Width, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33));
                    }

                    //bottom
                    if (pageModelFactory.Model.FooterBehaviour.Border.BottomBorder == null)
                    {
                        g.DrawLine(pen, 0, (float)(g.ClientSize.Height), (float)g.ClientSize.Width, (float)(g.ClientSize.Height));
                    }

                    //left
                    if (pageModelFactory.Model.FooterBehaviour.Border.LeftBorder == null)
                    {
                        g.DrawLine(pen, 0, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33), 0, (float)(g.ClientSize.Height));
                    }

                    //right
                    if (pageModelFactory.Model.FooterBehaviour.Border.RightBorder == null)
                    {
                        g.DrawLine(pen, (float)g.ClientSize.Width, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33), (float)g.ClientSize.Width, (float)(g.ClientSize.Height));
                    }
                }

                if (pageModelFactory.Model.FooterBehaviour.Border.BottomBorder != null)
                {

                    pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.Thickness = pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderBrush))
                            pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderBrush), (float)(pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.FooterBehaviour.Border.BottomBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, 0, (float)(g.ClientSize.Height), (float)g.ClientSize.Width, (float)(g.ClientSize.Height));
                    
                }
                if (pageModelFactory.Model.FooterBehaviour.Border.LeftBorder != null)
                {
                    pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.Thickness = pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderBrush))
                            pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderBrush), (float)(pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.FooterBehaviour.Border.LeftBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, 0, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33), 0, (float)(g.ClientSize.Height));
                }

                if (pageModelFactory.Model.FooterBehaviour.Border.TopBorder != null)
                {
                    pageModelFactory.Model.FooterBehaviour.Border.TopBorder.Thickness = pageModelFactory.Model.FooterBehaviour.Border.TopBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.FooterBehaviour.Border.TopBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderBrush))
                            pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderBrush), (float)(pageModelFactory.Model.FooterBehaviour.Border.TopBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.FooterBehaviour.Border.TopBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.FooterBehaviour.Border.TopBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, 0, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33), (float)g.ClientSize.Width, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33));
                }

                if (pageModelFactory.Model.FooterBehaviour.Border.RightBorder != null)
                {
                    pageModelFactory.Model.FooterBehaviour.Border.RightBorder.Thickness = pageModelFactory.Model.FooterBehaviour.Border.RightBorder.Thickness == 0 ? 0.75 : pageModelFactory.Model.FooterBehaviour.Border.RightBorder.Thickness / 1.333;
                    if (pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderStyle != BorderStyles.None && pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderStyle != BorderStyles.Default)
                    {
                        if (string.IsNullOrEmpty(pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderBrush))
                            pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderBrush = "black";
                        pen = new PdfPen(GetBrushColor(pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderBrush), (float)(pageModelFactory.Model.FooterBehaviour.Border.RightBorder.Thickness / 1.33));
                    }
                    else
                        pen = new PdfPen(GetBrushColor("white"), (float)(pageModelFactory.Model.FooterBehaviour.Border.RightBorder.Thickness / 1.33));
                    if (pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderStyle != BorderStyles.Default)
                        pen.DashStyle = GetLineStyle((LineStyle)Enum.Parse(typeof(LineStyle), pageModelFactory.Model.FooterBehaviour.Border.RightBorder.BorderStyle.ToString(), true));
                    g.DrawLine(pen, (float)g.ClientSize.Width, (float)(g.ClientSize.Height - pageModelFactory.FooterHeight / 1.33), (float)g.ClientSize.Width, (float)(g.ClientSize.Height));
                }
            }
        }

#if SILVERLIGHT
        private ImageBrush GetImageBrush(RDL.Data.ReportModel report, string imageName)
        {
            Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
            foreach (EmbeddedImage embeddedImage in report.Report.EmbeddedImages)
            {
                if (embeddedImage.Name == imageName)
                {
                    string imageData = embeddedImage.ImageData;
                    BitmapImage bitMapImage = new BitmapImage();
                    bitMapImage = (BitmapImage)base64ImageConverter.ConvertToImage(imageData);
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = bitMapImage;
                    imageBrush.Stretch = Stretch.Uniform;
                    return imageBrush;
                }
            }
            return null;
        }
#else
        private System.Windows.Media.ImageBrush GetImageBrush(RDL.Data.ReportModel report, string imageName)
        {
            Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
            foreach (EmbeddedImage embeddedImage in report.Report.EmbeddedImages)
            {
                if (embeddedImage.Name == imageName)
                {
                    string imageData = embeddedImage.ImageData;
                    BitmapImage bitMapImage = new BitmapImage();
                    bitMapImage = (BitmapImage)base64ImageConverter.ConvertToImage(imageData);
                    System.Windows.Media.ImageBrush imageBrush = new System.Windows.Media.ImageBrush();
                    imageBrush.ImageSource = bitMapImage;
                    imageBrush.Stretch = System.Windows.Media.Stretch.Uniform;
                    return imageBrush;
                }
            }
            return null;
        }
#endif


#if !SILVERLIGHT


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        /// <returns>Returns the resized image</returns>
        /// <remarks></remarks>
        private System.Drawing.Image ResizeImage(System.Drawing.Image image, float newWidth, float newHeight)
        {
            Stream stream = new MemoryStream();
            System.Drawing.Image thumbnail = new Bitmap((int)newWidth, (int)newHeight);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnail);
            graphic.DrawImage(image, 0, 0, (float)newWidth, (float)newHeight);
            thumbnail.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            return System.Drawing.Image.FromStream(stream);
        }
#endif

        private void ProcessHeaderFooteritems(ReportModelContentCollection reportModelContentCollection, PdfGraphics graphics)
        {
            if (reportModelContentCollection != null)
            {
                foreach (var reportModel in reportModelContentCollection)
                {
                    switch (reportModel.ModelType)
                    {
                        case ModelType.TextBoxModel:
                            ProcessTextBox(reportModel);
                            break;
                        case ModelType.LineModel:
                            ProcessLineModel(reportModel);
                            break;
                        case ModelType.ImageModel:
                            ProcessImageModel(reportModel);
                            break;
                        case ModelType.RectangleModel:
                            ProcessRectangleModel(reportModel,0);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts the pixel value to point value
        /// </summary>
        /// <param name="paramValue">Pixel value in double</param>
        /// <returns>Returns the equivalent point value as float</returns>
        /// <remarks></remarks>
        private float PixelToPoint(double paramValue)
        {
            float value = Convert.ToSingle(paramValue);
            return PixelToPoint(value);
        }
        /// <summary>
        /// Converts the pixel value to point value
        /// </summary>
        /// <param name="paramValue">Pixel value in float</param>
        /// <returns>Returns the equivalent point value</returns>
        /// <remarks></remarks>
        private float PixelToPoint(float paramValue)
        {
            return (paramValue * 0.75f);
        }
        /// <summary>
        /// Process the report models
        /// </summary>
        /// <param name="reportModel">The ReportModeler</param>
        /// <param name="page">Pagenumber</param>
        /// <remarks></remarks>
        private void ProcessReportModel(IReportItemModeler reportModel, int page)
        {
            switch (reportModel.ModelType)
            {
                case ModelType.TextBoxModel:
                    ProcessTextBox(reportModel);
                    break;

                case ModelType.ImageModel:
                    ProcessImageModel(reportModel);
                    break;
                case ModelType.LineModel:
                    ProcessLineModel(reportModel);
                    break;
                case ModelType.RectangleModel:
                    ProcessRectangleModel(reportModel, page);
                    break;
                case ModelType.GaugeModel:
                    ProcessGaugeModel(reportModel);
                    break;
                case ModelType.ChartModel:
                    ProcessChartModel(reportModel);
                    break;
                case ModelType.TablixModel:
                    ProcessTablixModel(reportModel, page);
                    break;
                case ModelType.MapModel:
                    ProcessMapModel(reportModel);
                    break;
                default:
                    return;
            }
        }

        private void ProcessTablixModel(IReportItemModeler reportModel, int page)
        {
            TablixModel tablix = reportModel as TablixModel;
            ProcessTable(reportModel, page,false);
        }

        private void UpdateListRowIndex(IReportItemModeler reportModel)
        {
            if (ListRowIndexCollection.ContainsKey(reportModel.GUID))
                ListRowIndexCollection[reportModel.GUID]++;
            else
                ListRowIndexCollection.Add(reportModel.GUID, 0);
        }

        private PdfGrid ProcessTable(IReportItemModeler reportModel, int page, bool isRecursive)
        {
            TablixModel tablix = reportModel as TablixModel;
            TablixEngine engine = tablix.Engine;
            PdfGrid grid = new PdfGrid();
            int pageNo = reportModel.PrintPageInfo.BelongsTo[page];
            var pageInfo = tablix.PrintPageSizes[pageNo];
            if (pageInfo.RowIndices.Length <= 0 || pageInfo.ColumnIndices.Length <= 0)
                return null;

            int rowStart = pageInfo.RowIndices[0];
            int rowEnd = pageInfo.RowIndices[pageInfo.RowIndices.Length - 1];
            int columnStart = pageInfo.ColumnIndices[0];
            int columnEnd = pageInfo.ColumnIndices[pageInfo.ColumnIndices.Length - 1];

            int columnCount = pageInfo.ColumnIndices.Count();
            int rowCount = pageInfo.RowIndices.Count();

            float x, y, width, height;

            var tempx = pageNo % tablix.PrintPageColumnCount == 0 ? tablix.PrintPageInfo.ActualLeft : 0;
            var tempy = pageNo < tablix.PrintPageColumnCount ? tablix.PrintPageInfo.ActualTop : 0;

            x = PixelToPoint(tempx);
            y = PixelToPoint(tempy);

            if (tablix.IsTablixChild && !isRecursive)
            {
                x += PixelToPoint(parentLeft);
                y += PixelToPoint(parentTop);
            }

            grid.Columns.Add(columnCount);
            for (int i = 0; i < columnCount; i++)
            {
                grid.Columns[i].Width = PixelToPoint(tablix.ColumnWights[i + columnStart - 1]);
            }

            for (int i = 0; i < rowCount; i++)
            {
                PdfGridRow row = grid.Rows.Add();
                grid.Rows[i].Height = PixelToPoint(tablix.RowHeights[pageInfo.RowIndices[i] - 1]);

                for (int j = 0; j < columnCount; j++)
                {
                    var cellInfo = tablix.Data[pageInfo.RowIndices[i] - 1][j + columnStart - 1];

                    if (cellInfo != null)
                    {
                        if (tablix.Model.EnableVirtualEvaluation)
                        {
                            cellInfo.CurrentKey = new List<int>();
                            cellInfo.CurrentKey.Add(pageInfo.RowIndices[i] - 1);
                            cellInfo.CurrentKey.Add(j + columnStart - 1);
                        }

                        PdfGridCell cell = row.Cells[j];

                        if (cellInfo.ItemModel.ModelType == ModelType.ImageModel)
                        {
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            ImageModel imageModel = (ImageModel)cellInfo.ItemModel;
                            if (imageModel != null && imageModel.ImageProperties.Border != null)
                            {
                                cellInfo.Border = imageModel.ImageProperties.Border;
                                ApplyPdfGridCellStyle(cell, cellInfo);
                            }
                            else
                            {
                                ApplyEmptyBorder(cell);
                            }
                            MemoryStream stream = new MemoryStream(imageModel.ImageData as byte[]);
                            PdfBitmap image = new PdfBitmap(stream);
                            cell.Style.BackgroundImage = image;
                            cell.ImagePosition = PdfGridImagePosition.Stretch;
                        }

                        else if (cellInfo.ItemModel.ModelType == ModelType.RectangleModel)
                        {
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            float leftOffset = (float)tempx;
                            float topOffset = (float)tempy;
                            m_bIsInList = true;

                            for (int rows = 0; rows < i; rows++)
                            {
                                topOffset += (float)(tablix.RowHeights[pageInfo.RowIndices[rows] - 1]);
                            }

                            for (int cols = 0; cols < j; cols++)
                            {
                                leftOffset += (float)(tablix.ColumnWights[pageInfo.ColumnIndices[cols] - 1]);
                            }

                            var top = cellInfo.ItemModel.Top = topOffset;
                            var left = cellInfo.ItemModel.Left = leftOffset;

                            ProcessRectangleModel(cellInfo.ItemModel as RectangleModel, pageNo);
                            m_bIsInList = false;
                            cellInfo.ItemModel.Top = top;
                            cellInfo.ItemModel.Left = left;
                            var rectangle = cellInfo.ItemModel as RectangleModel;
                            if (rectangle != null && rectangle.RectItemExpPro.Border != null)
                            {
                                cellInfo.Border = rectangle.RectItemExpPro.Border;
                                ApplyPdfGridCellStyle(cell, cellInfo);
                            }
                            else
                            {
                                ApplyEmptyBorder(cell);
                            }
                        }

                        else if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel)
                        {
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            TextRunExpval run = (cellInfo.ItemModel as TextboxModel).ParaExpval.First().Runs.First();
                            cell.Value = run.Text;
                            float leftOffset = x;
                            float topOffset = y;
                            TextboxModel textbox = cellInfo.ItemModel as TextboxModel;

                            //colorName = textbox.TextBoxProperties.BackGroundColor;
                            //grid.Rows[i].Style.BackgroundBrush = GetBrushColor(colorName);

                            if (textbox != null && textbox.ParaExpval.First().TextAlignment != null)
                            {
                                switch (textbox.ParaExpval.First().TextAlignment)
                                {
                                    case "Right":
                                        cell.StringFormat.Alignment = PdfTextAlignment.Right;
                                        break;
                                    case "Left":
                                        cell.StringFormat.Alignment = PdfTextAlignment.Left;
                                        break;
                                    case "Middle":
                                        cell.StringFormat.Alignment = PdfTextAlignment.Center;
                                        break;
                                    default:
                                        cell.StringFormat.Alignment = PdfTextAlignment.Left;
                                        break;

                                }
                            }

                            if (textbox != null && textbox.TextBoxProperties.Border != null)
                            {
                                cellInfo.Border = textbox.TextBoxProperties.Border;
                                ApplyPdfGridCellStyle(cell, cellInfo);
                            }
                            else
                            {
                                ApplyEmptyBorder(cell);
                            }
                            cell.Style.Font = GetPdfFont(run);
                        }
                        else if (cellInfo.ItemModel.ModelType == ModelType.TablixModel)
                        {
                            grid.Style.CellPadding.Right = (-1.0f);
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                var model = (cellInfo.ItemModel as TablixModel);
                                TablixEvaluationItems items = cellInfo.Rows[pageInfo.RowIndices[i] - 1].TablixValues[j + (columnStart - 1)][model.Name];
                                model.UpdateTablixValue(model, items);
                            }
                            cell.Value = this.ProcessTable(cellInfo.ItemModel, pageNo, true);
                            ApplyPdfGridCellStyle(cell, cellInfo);
                        }
                        else if (cellInfo.ItemModel.ModelType == ModelType.GaugeModel)
                        {
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            GaugeModel gaugeModel = cellInfo.ItemModel as GaugeModel;
                            if (gaugeModel != null && gaugeModel.GaugePanelProperties.Border != null)
                            {
                                cellInfo.Border = gaugeModel.GaugePanelProperties.Border;
                                ApplyPdfGridCellStyle(cell, cellInfo);
                            }
                            else
                            {
                                ApplyEmptyBorder(cell);
                            }
                            Stream stream = gaugeModel.GetImageStream();
                            PdfBitmap image = new PdfBitmap(stream);
                            cell.Style.BackgroundImage = image;
                            cell.ImagePosition = PdfGridImagePosition.Stretch;
                        }
                        else if (cellInfo.ItemModel.ModelType == ModelType.ChartModel)
                        {
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            ChartModel chartModel = cellInfo.ItemModel as ChartModel;
                            if (chartModel != null && chartModel.ChartProperties.Border != null)
                            {
                                cellInfo.Border = chartModel.ChartProperties.Border;
                                ApplyPdfGridCellStyle(cell, cellInfo);
                            }
                            else
                            {
                                ApplyEmptyBorder(cell);
                            }
                            Stream stream = chartModel.GetImageStream();
                            PdfBitmap image = new PdfBitmap(stream);
                            cell.Style.BackgroundImage = image;
                            cell.ImagePosition = PdfGridImagePosition.Stretch;
                        }
                        else if (cellInfo.ItemModel.ModelType == ModelType.MapModel)
                        {
                            if (tablix.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            MapModel mapModel = cellInfo.ItemModel as MapModel;
                            if (mapModel != null && mapModel.MapProperties.Border != null)
                            {
                                cellInfo.Border = mapModel.MapProperties.Border;
                                ApplyPdfGridCellStyle(cell, cellInfo);
                            }
                            else
                            {
                                ApplyEmptyBorder(cell);
                            }
                            Stream stream = mapModel.GetImageStream();
                            PdfBitmap image = new PdfBitmap(stream);
                            cell.Style.BackgroundImage = image;
                            cell.ImagePosition = PdfGridImagePosition.Stretch;
                        }

                        if (tablix.Model.EnableVirtualEvaluation)
                        {
                            cellInfo.DisposeEvalObjects();
                        }
                    }
                }
            }

            SetCoveredRange(tablix, pageInfo, grid);
            height = PixelToPoint(tablix.PrintPageSizes[pageNo].Height);
            width = PixelToPoint(tablix.PrintPageSizes[pageNo].Width);

            if (!tablix.IsTablixChild)
            {
                grid.Draw(currentPage.Graphics, parentX + x, parentY + y, width);
            }
            else if (tablix.IsTablixChild && !isRecursive)
            {
                grid.Draw(currentPage.Graphics, x, y, width);
            }

            //for (int i = 0; i < rowCount; i++)
            //{
            //    for (int j = 0; j < columnCount; j++)
            //    {
            //        var cellInfo = tablix.Data[i + rowStart - 1, j + columnStart - 1];
            //        cellInfo.Evaluate();
            //        if (cellInfo != null && cellInfo.ItemModel.ModelType == ModelType.RectangleModel)
            //        {
            //            m_bIsInList = true;
            //            this.m_currentListCellInfo = cellInfo;

            //            float leftOffset = x;
            //            float topOffset = y;

            //            for (int rows = rowStart; rows < i + rowStart; rows++)
            //            {
            //                topOffset += (float)(tablix.RowHeights[rows - 1]);
            //            }

            //            for (int cols = columnStart; cols < j + columnStart; cols++)
            //            {
            //                leftOffset += (float)(tablix.ColumnWights[cols - 1]);
            //            }

            //            PointF listOffset = new PointF((float)leftOffset, (float)topOffset);
            //            parentModel.Push(listOffset);
            //            UpdateParentOffset();

            //            var models = from rptModel in cellInfo.ItemModel.ReportItemModelers
            //                         where rptModel.ContainerModel == cellInfo.ItemModel
            //                         select rptModel;

            //            foreach (var reportitem in models)
            //            {
            //                ProcessReportModel(reportitem, page);
            //            }

            //            m_currentListCellInfo = null;
            //            ClearParentOffset();

            //            m_bIsInList = false;
            //            cellInfo.DisposeEvalObjects();
            //        }
            //    }
            //}
            return grid;
        }
        int GetIndexPos(int[] indicis, int element,TablixModel tablixModel)
        {
            int pos = Array.IndexOf(indicis, element);
            if (pos == -1)
            {
                return IsheaderSequential(indicis,tablixModel);
            }
            return pos;
        }

        int IsheaderSequential(int[] indicis, TablixModel tablixModel)
        {
            if (tablixModel.RepeatHeaderIndexes.ContainsKey(indicis[0] - 1))
            {
                int sqno = indicis[0];
                for (int i = 1; i < indicis.Length - 2; i++)
                {
                    sqno++;
                    if (!tablixModel.RepeatHeaderIndexes.ContainsKey(indicis[i] - 1))
                    {
                        if (sqno == indicis[i])
                        {
                            return 0;
                        }
                        return i;
                    }
                }
            }
            return 0;
        }

        void SetCoveredRange(TablixModel tablixModel, TablixPageInfo pageInfo,PdfGrid grid)
        {
            ReportingBrushConverter brushConverter = new ReportingBrushConverter();
            var startLeft = pageInfo.ColumnIndices.First();
            var endLeft = pageInfo.ColumnIndices.Last();
            var originalTop = pageInfo.RowIndices.First();
            var originalbottom = pageInfo.RowIndices.Last();
            var startTop = pageInfo.RowIndices[this.IsheaderSequential(pageInfo.RowIndices,tablixModel)];
            var endTop = originalbottom; var endCol = grid.Columns.Count;
            var endRow = grid.Rows.Count ;

            var coveredRange = tablixModel.CoveredRanges.Where(range => ((range.Left + 1 >= startLeft && range.Left + 1 <= endLeft && pageInfo.ColumnIndices.Contains(range.Left + 1) || (startLeft > range.Left + 1 && endLeft < range.Right + 1)) || ((range.Right + 1 <= endLeft && range.Right + 1 >= startLeft && pageInfo.ColumnIndices.Contains(range.Right + 1)) && range.Left + 1 < startLeft)) && ((range.Top + 1 >= startTop && range.Top + 1 <= endTop && pageInfo.RowIndices.Contains(range.Top + 1)) || ((range.Bottom + 1 <= endTop && range.Bottom + 1 >= startTop && (pageInfo.RowIndices.Contains(range.Bottom + 1) || (endTop >= range.Bottom + 1))) && range.Top + 1 < startTop)) || (startTop > range.Top + 1 && endTop < range.Bottom + 1) || (pageInfo.RowIndices.Contains(range.Top + 1) && pageInfo.ColumnIndices.Contains(range.Left + 1)) || (pageInfo.RowIndices.Contains(range.Bottom + 1) && pageInfo.ColumnIndices.Contains(range.Right + 1)));

            foreach (CoveredCellRange range in coveredRange)
            {
                int top = (range.Top + 1 < originalTop) ? 1 : (this.GetIndexPos(pageInfo.RowIndices, (range.Top + 1),tablixModel)) + 1;
                int right = ((range.Right + 1) <= endLeft) ? (Array.IndexOf(pageInfo.ColumnIndices, (range.Right + 1))) + 1 : endCol;
                int left = (range.Left + 1 < startLeft) ? 1 : (Array.IndexOf(pageInfo.ColumnIndices, (range.Left + 1))) + 1;
                int bottom = ((range.Bottom + 1) <= endTop) ? (Array.IndexOf(pageInfo.RowIndices, (range.Bottom + 1))) + 1 : endRow;

                if (!pageInfo.RowIndices.Contains(range.Bottom + 1) && bottom == 0)
                {
                    var prev = pageInfo.RowIndices.Where(index => (index > range.Bottom + 1) && (range.Bottom + 1 <= endTop)).FirstOrDefault();
                    bottom = Array.LastIndexOf(pageInfo.RowIndices, prev);
                }
                if (top == bottom && left == right)
                {
                    this.ApplyMergeStyle(grid, top - 1, left - 1, tablixModel.Data[range.Top][range.Left]);
                }
                else if (top <= bottom && left <= right)
                {
                    bool isLeft = pageInfo.ColumnIndices.Contains(range.Left + 1);
                    bool isTop = pageInfo.RowIndices.Contains(range.Top + 1);
                    bool isColumnCover = !(isLeft && pageInfo.ColumnIndices.Contains(range.Right + 1));
                    bool isRowCover = !(isTop && pageInfo.RowIndices.Contains(range.Bottom + 1));
                    if ((isColumnCover && !isLeft) || (isRowCover && !isTop))
                    {
                        this.ApplyMergeStyle(grid, top - 1, left - 1, tablixModel.Data[range.Top][range.Left]);
                    }
                    if(right-left>0)
                       grid.Rows[top-1].Cells[left-1].ColumnSpan = (right-left)+1;
                    if(bottom-top>0)
                       grid.Rows[top-1].Cells[left-1].RowSpan = (bottom-top)+1;
                }
            }
        }

        private void ApplyMergeStyle(PdfGrid grid, int rowIndex, int columnIndex, TablixCellInfo cellInfo)
        {
            ApplyCellBorder(cellInfo.Border, grid.Rows[rowIndex].Cells[columnIndex].Style.Borders.Left);
            ApplyCellBorder(cellInfo.Border, grid.Rows[rowIndex].Cells[columnIndex].Style.Borders.Right);
            ApplyCellBorder(cellInfo.Border, grid.Rows[rowIndex].Cells[columnIndex].Style.Borders.Top);
            ApplyCellBorder(cellInfo.Border, grid.Rows[rowIndex].Cells[columnIndex].Style.Borders.Bottom);

            var textboxModel = (cellInfo.ItemModel as TextboxModel);

            textboxModel.Evaluate();

            colorName=(cellInfo.ItemModel as TextboxModel).TextBoxProperties.BackGroundColor;
            grid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundBrush = GetBrushColor(colorName);

            grid.Rows[rowIndex].Cells[columnIndex].Value = textboxModel.ParaExpval.First().Runs.First().Text;

            grid.Rows[rowIndex].Cells[columnIndex].Style.TextBrush = ConvertToPdfColor(textboxModel.ParaExpval.First().Runs.First().Style.TextColor);

            if (textboxModel != null && textboxModel.ParaExpval.First().TextAlignment != null)
            {
                switch (textboxModel.ParaExpval.First().TextAlignment)
                {
                    case "Right":
                        grid.Rows[rowIndex].Cells[columnIndex].StringFormat.Alignment = PdfTextAlignment.Right;
                        break;
                    case "Left":
                        grid.Rows[rowIndex].Cells[columnIndex].StringFormat.Alignment = PdfTextAlignment.Left;
                        break;
                    case "Middle":
                        grid.Rows[rowIndex].Cells[columnIndex].StringFormat.Alignment = PdfTextAlignment.Center;
                        break;
                    default:
                        grid.Rows[rowIndex].Cells[columnIndex].StringFormat.Alignment = PdfTextAlignment.Left;
                        break;

                }
            }

            if (textboxModel != null && textboxModel.TextBoxProperties.Border != null)
            {
                cellInfo.Border = textboxModel.TextBoxProperties.Border;
                ApplyPdfGridCellStyle(grid.Rows[rowIndex].Cells[columnIndex], cellInfo);
            }
            else
            {
                ApplyEmptyBorder(grid.Rows[rowIndex].Cells[columnIndex]);
            }

            grid.Rows[rowIndex].Cells[columnIndex].Style.Font = this.GetPdfFont(textboxModel.ParaExpval.First().Runs.First());

            //textboxModel.DisposeEvalObjects();
        }

        private void ApplyEmptyBorder(PdfGridCell cell)
        {
#if WINRT 
            cell.Style.Borders.Left.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));
            cell.Style.Borders.Right.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));
            cell.Style.Borders.Top.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));
            cell.Style.Borders.Bottom.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));

#elif SILVERLIGHT
            cell.Style.Borders.Left.Color = System.Windows.Media.Colors.White;
            cell.Style.Borders.Right.Color = System.Windows.Media.Colors.White;
            cell.Style.Borders.Top.Color = System.Windows.Media.Colors.White;
            cell.Style.Borders.Bottom.Color = System.Windows.Media.Colors.White;

#else
            cell.Style.Borders.Left.Color =  Color.Empty;
            cell.Style.Borders.Right.Color = Color.Empty;
            cell.Style.Borders.Top.Color =  Color.Empty;
            cell.Style.Borders.Bottom.Color = Color.Empty;


#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellInfo"></param>
        /// <remarks></remarks>
        private void ApplyPdfGridCellStyle(PdfGridCell cell, TablixCellInfo cellInfo)
        {
            cell.Style = new PdfGridCellStyle();

            if (cellInfo.ItemModel is TextboxModel)
            {
                var textBoxModel = (cellInfo.ItemModel as TextboxModel);

                //Background color
                if (textBoxModel.TextBoxProperties.BackGroundColor != null)
                {
                    cell.Style.BackgroundBrush = ConvertToPdfColor(textBoxModel.TextBoxProperties.BackGroundColor);
                }

                cell.StringFormat.LineAlignment = GetVerticalAlignment(textBoxModel.TextBoxProperties.VerticalAlignment.ToString());

                var para = textBoxModel.ParaExpval.FirstOrDefault();

                if (para != null)
                {
                    var run = para.Runs.FirstOrDefault();

                    if (run != null)
                    {
                        cell.Style.Font = this.GetPdfFont(run);// new PdfTrueTypeFont(font);                     
                        //Foreground color
                        if (!string.IsNullOrEmpty(run.Style.TextColor))
                        {
                            cell.Style.TextBrush = ConvertToPdfColor(run.Style.TextColor);                                         
                        }
                    }

                    //Alignment 
                    cell.StringFormat.Alignment = GetAlignment(para.TextAlignment);
                }
            }
          
            //Cell borders
            if (cellInfo.Border != null)
            {
                ApplyPdfGridCellBorder(cellInfo.Border.LeftBorder, cell.Style.Borders.Left);
                ApplyPdfGridCellBorder(cellInfo.Border.RightBorder, cell.Style.Borders.Right);
                ApplyPdfGridCellBorder(cellInfo.Border.TopBorder, cell.Style.Borders.Top);
                ApplyPdfGridCellBorder(cellInfo.Border.BottomBorder, cell.Style.Borders.Bottom);
                if (cellInfo.Border.LeftBorder == null)
                {
                    ApplyCellBorder(cellInfo.Border, cell.Style.Borders.Left);
                }
                if (cellInfo.Border.RightBorder == null)
                {
                    ApplyCellBorder(cellInfo.Border, cell.Style.Borders.Right);
                }
                if (cellInfo.Border.TopBorder == null)
                {
                    ApplyCellBorder(cellInfo.Border, cell.Style.Borders.Top);
                }
                if (cellInfo.Border.BottomBorder == null)
                {
                    ApplyCellBorder(cellInfo.Border, cell.Style.Borders.Bottom);
                }
            }
            //cell padding
        }

        private void ApplyPdfGridCellBorder(BorderExpvalProperties border, PdfPen borderPen)
        {
            if (border != null && border.BorderStyle != RDL.DOM.BorderStyles.None && border.BorderStyle != RDL.DOM.BorderStyles.Default)
            {
                colorName = border.BorderBrush;
                if (string.IsNullOrEmpty(colorName))
                {
                    colorName = "Transparent";
                }
                if (!colorValues.ContainsKey(colorName))
                {
                    colorValues.Add(colorName, GetColorFromHexa(colorName));
                }

                borderPen.Color = colorValues[colorName];
                borderPen.Width = PixelToPoint(border.Thickness);
                if (border.Thickness == 0)
                {
                    borderPen.Color = GetColorFromHexa("Transparent");
                }
            }
            else
            {
#if WINRT 
                borderPen.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));

#elif SILVERLIGHT
                borderPen.Color = System.Windows.Media.Colors.White;

#else
                borderPen.Color = Color.Empty;

#endif
            }
        }

        private void ApplyCellBorder(BorderExpval bor, PdfPen border)
        {
            if (bor != null && bor.Default != null)
            {
                if (bor.Default.BorderStyle != RDL.DOM.BorderStyles.None && bor.Default.BorderStyle != RDL.DOM.BorderStyles.Default)
                {
                    colorName = bor.Default.BorderBrush;

                    if (string.IsNullOrEmpty(colorName))
                    {
                        colorName = "Transparent";
                    }

                    if (colorValues!=null && !colorValues.ContainsKey(colorName))
                    {
                        colorValues.Add(colorName, GetColorFromHexa(colorName));
                    }

                    else
                    {
                        border.Color = colorValues[colorName];
                    }
                    border.Width = PixelToPoint(bor.Default.Thickness);
                }
                if (bor.Default.Thickness == 0.0 || bor.Default.BorderStyle == RDL.DOM.BorderStyles.None || bor.Default.BorderStyle == RDL.DOM.BorderStyles.Default)
                {
#if WINRT 
                    border.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));
#elif SILVERLIGHT
                    border.Color = System.Windows.Media.Colors.White;

#else
                    border.Color = Color.Empty;
#endif
                }
            }
            else
            {
#if WINRT 
                border.Color = new PdfColor(Color.FromArgb(255, 255, 255, 255));
#elif SILVERLIGHT
                border.Color = System.Windows.Media.Colors.White;

#else
                    border.Color = Color.Empty;
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verticalAlignment"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private PdfVerticalAlignment GetVerticalAlignment(string verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case "Bottom":
                    return PdfVerticalAlignment.Bottom;
                case "Middle":
                    return PdfVerticalAlignment.Middle;
                case "Top":
                    return PdfVerticalAlignment.Top;
            }
            return PdfVerticalAlignment.Top;
        }

        /// <summary>
        /// Get the equivalent PdfTextAlignement
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private PdfTextAlignment GetAlignment(HorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    return PdfTextAlignment.Center;
                case HorizontalAlignment.Left:
                    return PdfTextAlignment.Left;
                case HorizontalAlignment.Right:
                    return PdfTextAlignment.Right;
                case HorizontalAlignment.Stretch:
                    return PdfTextAlignment.Justify;
            }
            return PdfTextAlignment.Left;
        }

        private PdfBrush ConvertToPdfColor(string color)
        {
            return GetBrushColor(color);
        }
        /// <summary>
        /// Process the rectangle report model
        /// </summary>
        /// <param name="reportModel"></param>
        /// <remarks></remarks>
        /// <summary>
        /// Page number of the item to be rendered.
        /// </summary>
        /// <param name="pageNo"></param>
        /// <remarks></remarks>
        private void ProcessRectangleModel(IReportItemModeler reportModel,int pageNo)
        {
            RectangleModel rectangeModel = reportModel as RectangleModel;
            double topValue = 0;
            double leftVal = 0;
            double heightVal = 0;
            double widthVal = 0;
            var page = 0;

            if (m_currentListCellInfo != null || IsHeaderFooterTemplate)
            {
                topValue = rectangeModel.FlowLayoutInfo.ActualTop;
                leftVal = rectangeModel.FlowLayoutInfo.ActualLeft;
                heightVal = rectangeModel.FlowLayoutInfo.ActualHeight;
                widthVal = rectangeModel.FlowLayoutInfo.ActualWidth;
            }
            else if (rectangeModel.PrintPageInfo != null)
            {
                if (rectangeModel.IsTablixChild)
                {
                    topValue =parentTop= rectangeModel.Top;
                    leftVal = parentLeft= rectangeModel.Left;
                }
                else
                {
                    topValue = rectangeModel.PrintPageInfo.ActualTop;
                    leftVal = rectangeModel.PrintPageInfo.ActualLeft;
                }
                if (rectangeModel.PrintPageSizes != null && rectangeModel.PrintPageSizes.Count > 0)
                {
                    var pageSize = rectangeModel.PrintPageSizes[rectangeModel.PrintPageInfo.BelongsTo[pageNo]];
                    heightVal = pageSize.Height;
                    widthVal = pageSize.Width;
                }
                else
                {
                    heightVal = rectangeModel.PrintPageInfo.ActualHeight;
                    widthVal = rectangeModel.PrintPageInfo.ActualWidth;
                }
            }
            else
            {
                topValue = rectangeModel.Top;
                leftVal = rectangeModel.Left;
                heightVal = rectangeModel.Height;
                widthVal = rectangeModel.Width;
            }
            PdfGraphics g = currentPage.Graphics;

            colorName = rectangeModel.RectItemExpPro.BackgroundColor;

            if (string.IsNullOrEmpty(colorName))
            {
                colorName = "Transparent";
            }
            if (colorValues!=null && !colorValues.ContainsKey(colorName))
            {
                colorValues.Add(colorName, GetColorFromHexa(colorName));
            }

            backcolor = colorValues[colorName];
#if WINRT
            borderColor = new PdfColor(Color.FromArgb(255, 255, 255, 255));
#elif SILVERLIGHT
                borderColor = System.Windows.Media.Colors.White;
#else
            borderColor = Color.Empty;
#endif

            if (rectangeModel.RectItemExpPro.Border != null && rectangeModel.RectItemExpPro.Border.Default != null)
            {
                colorName = rectangeModel.RectItemExpPro.Border.Default.BorderBrush;
                if (string.IsNullOrEmpty(colorName))
                {
                    colorName = "Transparent";
                }
                if (colorValues!=null && !colorValues.ContainsKey(colorName))
                {
                    colorValues.Add(colorName, GetColorFromHexa(colorName));
                }

                borderColor = colorValues[colorName];
            }



#if WINRT 
            backColorname = backcolor.ToString();
            borderColorname = borderColor.ToString();
#elif SILVERLIGHT
            backColorname = backcolor.ToString(); //GetColorName(backcolor);
            borderColorname = borderColor.ToString(); //GetColorName(borderColor);
#else

            if (backcolor == Color.Empty)
            {
                backColorname = null;
            }
            else
            {
                if (backcolor.IsNamedColor)
                {
                    backColorname = backcolor.Name;
                }
                else
                {
                    backColorname = "#" + backcolor.Name.Remove(0,2);
                }
            }
            if (borderColor == Color.Empty)
            {
                borderColorname = null;
            }
            else
            {
                if (borderColor.IsNamedColor)
                {
                    borderColorname = borderColor.Name;
                }
                else
                {
                    borderColorname = "#" + borderColor.Name.Remove(0, 2);
                }
            }

#endif
            isWhite = false;
            if (backcolor == GetColorFromHexa("White") || backcolor == GetColorFromHexa("Transparent"))
            {
                isWhite = true;
            }
            bool isBorderwhite = false;
            if (borderColor == GetColorFromHexa("White") || borderColor == GetColorFromHexa("Transparent"))
            {
                isBorderwhite = true;
            }


            if (!(backColorname == "ffffffff" || isWhite) && borderColorname != null &&
                !(borderColorname == "ffffffff" || isBorderwhite))
            {
                PdfPen pen = new PdfPen(GetColorFromHexa(colorName));
                if(rectangeModel.RectItemExpPro.Border!=null)
                pen.Width = PixelToPoint(rectangeModel.RectItemExpPro.Border.Default.Thickness);
                g.DrawRectangle(pen, GetBrushColor(backColorname), parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topValue), PixelToPoint(widthVal), PixelToPoint(heightVal));
            }
            else if (!(backColorname == "ffffffff" || isWhite))
            {
                g.DrawRectangle(GetBrushColor(colorName), parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topValue), PixelToPoint(widthVal), PixelToPoint(heightVal));
            }
            else if (borderColor != null && borderColorname != null && !(borderColorname == "ffffffff" || isBorderwhite))
            {
                if (rectangeModel.RectItemExpPro.Border.Default.BorderStyle != RDL.DOM.BorderStyles.None && rectangeModel.RectItemExpPro.Border.Default.BorderStyle != RDL.DOM.BorderStyles.Default)
                {
                    PdfPen pen = new PdfPen(GetColorFromHexa(colorName));
                    pen.Width = PixelToPoint(rectangeModel.RectItemExpPro.Border.Default.Thickness);
                    g.DrawRectangle(pen, parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topValue), PixelToPoint(widthVal), PixelToPoint(heightVal));
                }
            }
            if (rectangeModel.PrintPageInfo != null)
            {
                page = rectangeModel.PrintPageInfo.BelongsTo[pageNo];
            }
            if (rectangeModel.ReportItemModelers.Count > 0 && m_bIsInList == true)
            {
                parentModel.Push(reportModel);
                UpdateParentOffset();

                foreach (var item in rectangeModel.ReportItemModelers)
                {
                    ProcessReportModel(item as IReportItemModeler, page);
                }

                ClearParentOffset();
            }
        }

#if !WINRT 
        internal static string GetColorName(System.Windows.Media.Color color)
        {
            System.Type colors = typeof(System.Windows.Media.Colors);
            foreach (var prop in colors.GetProperties())
            {
                if (((System.Windows.Media.Color)prop.GetValue(null, null)) == color)
                    return prop.Name;
            }

            throw new Exception("The provided Color is not named.");
        }
#endif



        /// <summary>
        /// Process the line model
        /// </summary>
        /// <param name="reportModel"></param>
        /// <remarks></remarks>
        private void ProcessLineModel(IReportItemModeler reportModel)
        {
            LineModel lineModel = reportModel as LineModel;

            double topValue = 0;
            double leftVal = 0;
            double heightVal = 0;
            double widthVal = 0;

            if (m_currentListCellInfo != null || IsHeaderFooterTemplate)
            {
                topValue = lineModel.FlowLayoutInfo.ActualTop;
                leftVal = lineModel.FlowLayoutInfo.ActualLeft;
                heightVal = lineModel.FlowLayoutInfo.ActualHeight;
                widthVal = lineModel.FlowLayoutInfo.ActualWidth;
            }
            else
            {
                topValue = lineModel.PrintPageInfo.ActualTop;
                leftVal = lineModel.PrintPageInfo.ActualLeft;
                heightVal = lineModel.PrintPageInfo.ActualHeight;
                widthVal = lineModel.PrintPageInfo.ActualWidth;
            }

            PdfGraphics g = currentPage.Graphics;
            GetBrushColor(lineModel.LineProperties.LineColor);
            lineModel.LineProperties.LineWidth = lineModel.LineProperties.LineWidth == 0 ? 0.75 : lineModel.LineProperties.LineWidth / 1.333;
            PdfPen pen = new PdfPen(GetColorFromHexa(lineModel.LineProperties.LineColor), (float)lineModel.LineProperties.LineWidth);
            pen.DashStyle = GetLineStyle(lineModel.LineProperties.LineStyle);
            System.Drawing.PointF point1 = new System.Drawing.PointF(parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topValue));
            System.Drawing.PointF point2 = new System.Drawing.PointF(parentX + PixelToPoint(leftVal + widthVal), parentY + PixelToPoint(topValue + heightVal));
            g.DrawLine(pen, point1, point2);
        }

        private PdfDashStyle GetLineStyle(LineStyle style)
        {
            switch(style)
            {
                case LineStyle.Dashed:
                    return PdfDashStyle.Dash;
                case LineStyle.DashDot:
                    return PdfDashStyle.DashDot;
                case LineStyle.Dotted:
                    return PdfDashStyle.Dot;
                default:
                    return PdfDashStyle.Solid;
            }
        }


        /// <summary>
        /// Process the image model
        /// </summary>
        /// <param name="reportModel"></param>
        /// <remarks></remarks>
        private void ProcessImageModel(IReportItemModeler reportModel)
        {
            ImageModel imageModel = reportModel as ImageModel;
            double topValue = 0;
            double leftVal = 0;
            double heightVal = 0;
            double widthVal = 0;

            if (m_currentListCellInfo != null || IsHeaderFooterTemplate)
            {
                topValue = imageModel.FlowLayoutInfo.ActualTop;
                leftVal = imageModel.FlowLayoutInfo.ActualLeft;
                heightVal = imageModel.FlowLayoutInfo.ActualHeight;
                widthVal = imageModel.FlowLayoutInfo.ActualWidth;
            }
            else if(imageModel.PrintPageInfo!=null)
            {
                topValue = imageModel.PrintPageInfo.ActualTop;
                leftVal = imageModel.PrintPageInfo.ActualLeft;
                heightVal = imageModel.PrintPageInfo.ActualHeight;
                widthVal = imageModel.PrintPageInfo.ActualWidth;
            }

            else
            {
                topValue = imageModel.Top;
                leftVal = imageModel.Left;
                heightVal = imageModel.Height;
                widthVal = imageModel.Width;
            }
            object imageData = imageModel.ImageData;

            if (imageModel.ImageProperties.Border.Default!=null && imageModel.ImageProperties.Border.Default.BorderBrush != null)
            {
                colorName = imageModel.ImageProperties.Border.Default.BorderBrush;

                if (string.IsNullOrEmpty(colorName))
                {
                    colorName = "Transparent";
                }
                if ( colorValues!=null && !colorValues.ContainsKey(colorName))
                {
                    colorValues.Add(colorName, GetColorFromHexa(colorName));
                }

                borderColor = colorValues[colorName];
            }
            else
            {
                borderColor = GetColorFromHexa("Transparent");
            }

            imageData = imageModel.ImageData;       

            Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
            if (imageData == null)
                return;
            Stream imageStream;

            PdfGraphics g = currentPage.Graphics;
            isWhite = false;
            bool hasStyle = false;

            if (imageModel.ImageProperties.Border.Default != null && imageModel.ImageProperties.Border.Default.BorderStyle != BorderStyles.Default
                && imageModel.ImageProperties.Border.Default.BorderStyle != BorderStyles.None)
            {
                hasStyle = true;
            }
            if (borderColor == GetColorFromHexa("White") || borderColor == GetColorFromHexa("Transparent"))
            {
                isWhite = true;
            }

            ThicknessExpval padding = imageModel.ImageProperties.Padding;

            if (!(borderColorname == "ffffffff" || isWhite) && hasStyle)
            {
                PdfPen pen = new PdfPen(borderColor);
                pen.Width = (float)(imageModel.ImageProperties.Border.Default.Thickness);
                g.DrawRectangle(pen, parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topValue), PixelToPoint(widthVal), PixelToPoint(heightVal));
            }

            if (imageModel.ImageFormat != ImageFormats.Emf)
            {
#if SILVERLIGHT

                byte[] data1 = imageData as byte[];
                imageStream = new MemoryStream(data1,0,data1.Length);
#if !WINRT
                //BitmapImage im = null;
#endif
                try
                {
                    //using (MemoryStream ms = new MemoryStream(data1, 0,
                    //  data1.Length))
                    //{
                    //     im = new BitmapImage();
                    //    im.SetSource(ms);
                    //    imageStream=ms;
                    //}
#if !WINRT 
                    System.Drawing.Image image1 = new System.Drawing.Image(imageStream);

#endif
                }

                catch
                {

                }
                MemoryStream memoryStream = new MemoryStream();
                int offset = 78;
                memoryStream.Write(data1, offset, data1.Length - offset);
                memoryStream.Seek(0, SeekOrigin.Begin);
#else
                try
                {
                    imageStream = ((BitmapImage)base64ImageConverter.ConvertToImage(imageModel.ImageData)).StreamSource;
                }
                catch
                {
                    imageStream = ((BitmapImage)BufferImage(imageModel.ImageData)).StreamSource;
                }
#endif

#if WINRT 

                PdfBitmap image = new PdfBitmap(imageStream);
#else 
                PdfBitmap image = new PdfBitmap(imageStream);

#endif
                g.DrawImage(image, parentX + PixelToPoint(leftVal + padding.Left), parentY + PixelToPoint(topValue + padding.Top), PixelToPoint(widthVal - (padding.Left + padding.Right)), PixelToPoint(heightVal - (padding.Top + padding.Bottom)));
            }
            else
            {
                byte[] data = imageData as byte[];
                imageStream = new MemoryStream(data);
            }
        }



   
        /// <summary>
        /// Process the Gauge model
        /// </summary>
        /// <param name="reportModel"></param>
        /// <remarks></remarks>
        private void ProcessGaugeModel(IReportItemModeler reportModel)
        {
            GaugeModel gaugeModel = reportModel as GaugeModel;

            //Syncfusion.Reports.Controls.Utils.ImageConversion gaugeConverter = new Syncfusion.Reports.Controls.Utils.ImageConversion();
            Stream Sources = null;  //gaugeConverter.CovertToImage(new ReportingGauge(gaugeModel));
#if SILVERLIGHT
           // Sources = gaugeModel.
#else
            Sources = gaugeModel.GetImageStream();
#endif

            if (Sources != null)
            {
                PdfBitmap image = new PdfBitmap(Sources);
                PdfGraphics g = currentPage.Graphics;
                if (gaugeModel.PrintPageInfo != null)
                {
                    g.DrawImage(image, parentX + PixelToPoint(gaugeModel.PrintPageInfo.ActualLeft), parentY + PixelToPoint(gaugeModel.PrintPageInfo.ActualTop), PixelToPoint(gaugeModel.PrintPageInfo.ActualWidth), PixelToPoint(gaugeModel.PrintPageInfo.ActualHeight));                    
                }
                else
                {
                    g.DrawImage(image, parentX + PixelToPoint(gaugeModel.Left), parentY + PixelToPoint(gaugeModel.Top), PixelToPoint(gaugeModel.Width), PixelToPoint(gaugeModel.Height));                                        
                }
            }
        }
        /// <summary>
        /// Process the Gauge model
        /// </summary>
        /// <param name="reportModel"></param>
        /// <remarks></remarks>
        private void ProcessChartModel(IReportItemModeler reportModel)
        {
            ChartModel chartModel = reportModel as ChartModel;

            //Syncfusion.Reports.Controls.Utils.ImageConversion gaugeConverter = new Syncfusion.Reports.Controls.Utils.ImageConversion();
            Stream Sources = chartModel.GetImageStream(); //gaugeConverter.CovertToImage(new ReportingChartControl(chartModel));
            if (Sources != null)
            {

                try
                {
                    PdfBitmap image = new PdfBitmap(Sources);
                    PdfGraphics g = currentPage.Graphics;
                    g.DrawImage(image, parentX + PixelToPoint(chartModel.PrintPageInfo.ActualLeft), parentY + PixelToPoint(chartModel.PrintPageInfo.ActualTop), PixelToPoint(chartModel.PrintPageInfo.ActualWidth), PixelToPoint(chartModel.PrintPageInfo.ActualHeight));
                }
                catch
                {

                }
            }
        }


        private void ProcessMapModel(IReportItemModeler reportModel)
        {
#if !SyncfusionFramework3_5
            MapModel mapModel = reportModel as MapModel;

            Stream Sources = mapModel.GetImageStream();
            if (Sources != null)
            {
                PdfBitmap image = new PdfBitmap(Sources);
                PdfGraphics g = currentPage.Graphics;
                g.DrawImage(image, parentX + PixelToPoint(mapModel.PrintPageInfo.ActualLeft), parentY + PixelToPoint(mapModel.PrintPageInfo.ActualTop), PixelToPoint(mapModel.PrintPageInfo.ActualWidth), PixelToPoint(mapModel.PrintPageInfo.ActualHeight));
            }
#endif
        }

        /// <summary>
        /// Process the textbox ReportItem
        /// </summary>
        /// <param name="reportModel"></param>
        /// <remarks></remarks>
        private void ProcessTextBox(IReportItemModeler reportModel)
        {
            PdfGraphics graphics = this.currentPage.Graphics;
            TextboxModel textbox = reportModel as TextboxModel;
            double topVal = 0;
            double leftVal = 0;
            double heightVal = 0;
            double widthVal = 0;

            if(textbox.PrintPageInfo!=null)
            {
                topVal = textbox.PrintPageInfo.ActualTop;
                leftVal = textbox.PrintPageInfo.ActualLeft;
                heightVal = textbox.PrintPageInfo.ActualHeight;
                widthVal = textbox.PrintPageInfo.ActualWidth;
            }
            else
            {
                topVal = textbox.Top;
                leftVal = textbox.Left;
                heightVal = textbox.Height;
                widthVal = textbox.Width;
            }


#if WINRT 
            borderColor = new PdfColor(Color.FromArgb(255, 255, 255, 255));

#elif SILVERLIGHT
            borderColor = System.Windows.Media.Colors.White;

#else
            borderColor = Color.Empty;
#endif
            colorName = textbox.TextBoxProperties.BackGroundColor;
            if (colorValues!=null && !colorValues.ContainsKey(colorName))
            {
                colorValues.Add(colorName, GetColorFromHexa(colorName));
            }

            backcolor = colorValues[colorName];

            if (textbox.TextBoxProperties.Border.Default != null && textbox.TextBoxProperties.Border.Default.BorderBrush != null)
            {
                if (textbox.TextBoxProperties.Border.Default.Thickness != 0 && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.None && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.Default)
                {
                    colorName = textbox.TextBoxProperties.Border.Default.BorderBrush;
                    if (colorValues!=null && !colorValues.ContainsKey(colorName))
                    {
                        colorValues.Add(colorName, GetColorFromHexa(colorName));
                    }

                    borderColor = colorValues[colorName];
                }
                else
                {
                    borderColor = GetColorFromHexa("Transparent");
                }
            }
            else
            {
                borderColor = GetColorFromHexa("Transparent");
            }

#if !SILVERLIGHT
            if (backcolor.IsNamedColor)
            {
                backColorname = backcolor.Name;
            }
            else
            {
                backColorname = "#" + backcolor.Name.Remove(0, 2);
            }

            if (borderColor.IsNamedColor)
            {
                borderColorname = borderColor.Name;
            }
            else
            {
                borderColorname = "#" + borderColor.Name.Remove(0, 2);
            }
#endif
            isWhite = false;

            if (backcolor == GetColorFromHexa("White") || backcolor == GetColorFromHexa("Transparent"))
            {
                isWhite = true;
            }

            if (!(backColorname == "ffffffff" || isWhite))
            {
                graphics.DrawRectangle(GetBrushColor(backColorname), parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topVal), PixelToPoint(widthVal), PixelToPoint(heightVal));
            }

            isWhite = false;

            if (borderColor == GetColorFromHexa("White") || borderColor == GetColorFromHexa("Transparent"))
            {
                isWhite = true;
            }

            if (!(borderColorname == "ffffffff" || isWhite))
            {
                PdfPen pen = new PdfPen(GetBrushColor(colorName));
                pen.Width = PixelToPoint(textbox.TextBoxProperties.Border.Default.Thickness);
                graphics.DrawRectangle(pen, parentX + PixelToPoint(leftVal), parentY + PixelToPoint(topVal), PixelToPoint(widthVal), PixelToPoint(heightVal));
            }

            ThicknessExpval padding = textbox.TextBoxProperties.Padding;
            string text = string.Empty;
            PdfStringFormat currentStringFormat = new PdfStringFormat();
            PdfFont font = null;
            result = null;

            // Create a text element with large amount of text.
            PdfTextElement element;

            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            layoutFormat.Layout = PdfLayoutType.Paginate;

            System.Drawing.RectangleF bounds;

            float tempWidth = (float)(widthVal - (padding.Left + padding.Right));
            float tempHeight = (float)(heightVal - (padding.Top + padding.Bottom));
            float tempLeft = (float)(leftVal + padding.Left);
            float tempTop = (float)(topVal + padding.Top);
            float currentWidth = 0f;
            float currentHeight = 0f;
            float currentTop = PixelToPoint(tempTop);
            float currentLeft = PixelToPoint(tempLeft);
            int position=0;
            //float spaceLeft = 0f;
            float left = 0f;

            float totalWidth = (float)tempWidth;

            foreach (ParagraphExpval block in textbox.ParaExpval)
            {
                if (result != null)
                {
                    currentTop = result.LastLineBounds.Bottom + font.Height;
                    currentLeft = PixelToPoint(tempLeft);
                    result = null;
                }
                currentWidth = 0;
                m_position = 0;
                currentStringFormat.Alignment = GetAlignment(block.TextAlignment);
                currentStringFormat.WordWrap = PdfWordWrapType.Word;

                switch (currentStringFormat.Alignment)
                {
                    case PdfTextAlignment.Left:
                        foreach (TextRunExpval run in block.Runs)
                        {
                            text = GetTextRun(textbox, run);
                            font = GetPdfFont(run);

                            position++;
                            if (text != null)
                            {
                                element = new PdfTextElement(text, font);
                                element.Brush = GetPdfBrush(run);
                                element.StringFormat = currentStringFormat;
                                //element.
                                if (IsHeaderFooterTemplate)
                                    element.Text = ProcessGlobals(text, font);

                                text = element.Text;
                                #region LeftAligned
                                // Set the bounds.
                                if (result == null)
                                {
                                    SizeF size = font.MeasureString(text);
                                    var width = PixelToPoint(tempWidth);
                                    var height = PixelToPoint(tempHeight);
                                    if (height < size.Height)
                                    {
                                        height =(float)(size.Height + .5);
                                    }
                                    bounds = new System.Drawing.RectangleF(new System.Drawing.PointF(parentX + currentLeft, parentY + currentTop), new System.Drawing.SizeF(width,height));
                                    if (size.Width >= bounds.Width)
                                    {
                                        if (currentPage != null)
                                        {
                                            string textDrawn = CompleteCurrentTextLine(text, result, bounds, font, tempWidth, run);
                                            string toBeDrawn = text.Substring(textDrawn.Length);
                                            element.Text =this.ProcessGlobals(toBeDrawn,null);
                                        }

                                        // Draw the text element.
                                        if (m_currentListCellInfo == null)
                                            result = element.Draw(currentPage, new RectangleF(PixelToPoint(tempLeft), bounds.Y + font.Height, PixelToPoint(tempWidth), 0), layoutFormat);
                                        else
                                            result = element.Draw(currentPage, new RectangleF(PixelToPoint(tempLeft), bounds.Y + font.Height, PixelToPoint(tempWidth), 0), layoutFormat);
                                    }
                                    else
                                    {
                                        result = element.Draw(currentPage, bounds, layoutFormat);
                                    }
                                }
                                else
                                {
                                    currentWidth += result.LastLineBounds.Width;
                                    currentHeight = Math.Max(currentHeight, result.LastLineBounds.Height);
                                    float availableWidth = (totalWidth - currentWidth);
                                    float currentTextWidth = font.MeasureString(text).Width;

                                    if ((currentTextWidth + currentWidth) > availableWidth || currentTextWidth > PixelToPoint(availableWidth) || currentWidth > PixelToPoint(availableWidth))
                                    {
                                        bounds = new RectangleF(new PointF(0, result.LastLineBounds.Y + currentHeight), new SizeF(PixelToPoint(tempWidth) - result.Bounds.X, PixelToPoint(tempHeight) - 10));
                                        string textDrawn = CompleteCurrentTextLine(text, result, bounds, font, tempWidth, run);
                                        string toBeDrawn = text.Substring(textDrawn.Length);
                                        element.Text = toBeDrawn;
                                        // Draw the text element.                                      
                                        result = element.Draw(currentPage, new RectangleF(parentX + currentLeft, result.LastLineBounds.Y + font.Height, PixelToPoint(tempWidth), 0), layoutFormat);

                                        currentWidth = 0;
                                        currentTop += font.Height;
                                    }
                                    else
                                    {
                                        int lineCount = (int)(font.MeasureString(text).Height / font.Height);
                                        string[] texts = text.Split('\n');

                                        int lineValue = 0;
                                        foreach (var textValue in texts.ToList())
                                        {
                                            if (lineValue > 0)
                                            {
                                                currentWidth = 0;
                                                currentTop += font.Height;
                                                element.Text = textValue;
                                                if (IsHeaderFooterTemplate)
                                                    element.Text = this.ProcessGlobals(textValue, font);
                                                bounds = new System.Drawing.RectangleF(new System.Drawing.PointF(parentX + currentLeft,
                                                                                                                     parentY + currentTop), new System.Drawing.SizeF(PixelToPoint(tempWidth), PixelToPoint(tempHeight)));
                                            }
                                            else
                                            {
                                                float widthVals = font.MeasureString(textValue as string).Width;
                                                element.Text = textValue;
                                                if (IsHeaderFooterTemplate)
                                                    element.Text = this.ProcessGlobals(textValue, font);
                                                bounds = new System.Drawing.RectangleF(new System.Drawing.PointF(parentX + currentLeft + currentWidth + left,
                                                                                                                              parentY + currentTop), new System.Drawing.SizeF(PixelToPoint(tempWidth), PixelToPoint(tempHeight)));

                                            }
                                       
                                            result = element.Draw(currentPage, bounds, layoutFormat);
                                            lineValue++;
                                        }
                                    }
                                }
                                #endregion LeftAligned
                            }
                        }
                        break;
                    case PdfTextAlignment.Center:
                        foreach (TextRunExpval run in block.Runs)
                        {
                            text = GetTextRun(textbox, run);
                            font = GetPdfFont(run);

                            element = new PdfTextElement(text, font);
                            element.Brush = GetPdfBrush(run);
                            element.StringFormat = currentStringFormat;
                            position++;

                            if (IsHeaderFooterTemplate)
                            {
                               text = ProcessGlobals(text, font);
                            }

                            element.Text = text;

                            #region CenterAligned
                            // Set the bounds.
                            if (result == null)
                            {
                                bounds = new System.Drawing.RectangleF(currentLeft - font.MeasureString(text).Width / text.Length, parentY + currentTop, PixelToPoint(tempWidth), PixelToPoint(tempHeight));
                                SizeF size = font.MeasureString(text);
                                if (size.Width >= bounds.Width)
                                {
                                    element.Text = text;
                                    // Draw the text element.

                                    if (m_currentListCellInfo == null)
                                    {
                                        result = element.Draw(currentPage, new RectangleF(PixelToPoint(tempLeft), bounds.Y, PixelToPoint(tempWidth), 0), layoutFormat);
                                    }
                                    else
                                    {
                                        result = element.Draw(currentPage, new RectangleF(PixelToPoint(tempLeft), bounds.Y + font.Height, PixelToPoint(tempWidth), 0), layoutFormat);
                                    }
                                }
                                else
                                {
                                    result = element.Draw(currentPage, bounds, layoutFormat);
                                }
                            }
                            else
                            {
                                currentWidth += result.LastLineBounds.Width;
                                currentHeight = Math.Max(currentHeight, result.LastLineBounds.Height);
                                float availableWidth = (totalWidth - currentWidth);
                                float currentTextWidth = font.MeasureString(text).Width;

                                if ((currentTextWidth + currentWidth) > availableWidth || currentTextWidth > PixelToPoint(availableWidth) || currentWidth > PixelToPoint(availableWidth))
                                {
                                    bounds = new RectangleF(new PointF(0, result.LastLineBounds.Y + currentHeight), new SizeF(PixelToPoint(tempWidth) - result.Bounds.X, PixelToPoint(tempHeight)));
                                    string textDrawn = CompleteCurrentTextLine(text, result, bounds, font, tempWidth, run);
                                    string toBeDrawn = text.Substring(textDrawn.Length);
                                    element.Text = toBeDrawn;
                                    // Draw the text element.
                                    result = element.Draw(currentPage, new RectangleF(parentX + currentLeft, result.LastLineBounds.Y + font.Height, PixelToPoint(tempWidth), 0), layoutFormat);

                                    currentWidth = 0;
                                    currentTop += font.Height;
                                }
                                else
                                {
                                    float value = 0f;
                                    if (result.LastLineBounds.X  - (parentX  + currentWidth) > (currentLeft + currentWidth + left))
                                    {
                                        value = left;
                                    }
                                    bounds = new System.Drawing.RectangleF(new System.Drawing.PointF(currentLeft + currentWidth + left + value,
                                                                                                                          parentY + currentTop),
                                                                                                                          new System.Drawing.SizeF(PixelToPoint(tempWidth), PixelToPoint(tempHeight)));
                                   
                                    // Draw the text element
                                    result = element.Draw(currentPage, bounds, layoutFormat);
                                }
                            }
                            #endregion CenterAligned
                        }
                        break;
                    case PdfTextAlignment.Right:
                        int runCount = 0;
                        bounds = new RectangleF(parentX + PixelToPoint(tempLeft), parentY + PixelToPoint(tempTop),
                                                PixelToPoint(tempWidth), PixelToPoint(tempHeight));
                        float availWidth = PixelToPoint(tempWidth);
                        float occupiedWidth = 0;
                        totalWidth = PixelToPoint(tempWidth);
                        for (int i = 0; i < block.Runs.Count; i++)
                        {
                            TextRunExpval run = block.Runs[i];
                            text = GetTextRun(textbox,run);
                            font = GetPdfFont(run);
                            runCount++;

                            if (IsHeaderFooterTemplate)
                                text = ProcessGlobals(text, font);

                            element = new PdfTextElement(text, font);                            

                            element.Brush = GetPdfBrush(run);
                            element.StringFormat = currentStringFormat;

                            SizeF size = font.MeasureString(text);
                            occupiedWidth += size.Width;
                        }
                       
                        availWidth = totalWidth - occupiedWidth;
                            DrawRightAlignedTextRuns(textbox, block, bounds, availWidth, graphics);
                        break;
                }//end of switch
                
            }
        }

        /// <summary>
        /// Right Aligns the Text
        /// </summary>
        /// <param name="textBox">TextBox Model</param>
        /// <param name="block">Evaluated value of Block</param>
        /// <param name="bounds">Bounds to the Text</param>
        /// <param name="availWidth">Width of the Text block</param>
        /// <param name="graphics"></param>
        /// <remarks></remarks>
        private void DrawRightAlignedTextRuns(TextboxModel textBox,ParagraphExpval block, RectangleF bounds, float availWidth, PdfGraphics graphics)
        {
            PdfLayoutResult layoutResult = null;
            float height = 0f;
            if (availWidth < 0)
            {
                availWidth = 0;
            }
            RectangleF newBounds = new RectangleF(bounds.Left + availWidth, bounds.Top, bounds.Width - availWidth, bounds.Height);
            for (int i = 0; i < block.Runs.Count; i++)
            {
                TextRunExpval run = block.Runs[i];
                string text = GetTextRun(textBox, run);
                
                if (IsHeaderFooterTemplate)
                    text = ProcessGlobals(text, null);

                PdfFont font = GetPdfFont(run);
                PdfStringFormat stringformat = new PdfStringFormat();
                PdfTextElement element = new PdfTextElement(text, font);
                element.Brush = GetPdfBrush(run);
                stringformat.Alignment = GetAlignment(block.TextAlignment);

                if (availWidth == 0)
                {
                    element.StringFormat = stringformat;
                    element.Text = text;
                    layoutResult = element.Draw(currentPage, new RectangleF(parentX + newBounds.Left, parentY + newBounds.Y + height, newBounds.Width, 0));
                    height = font.Height;
                }
                else if (layoutResult != null)
                {
                    layoutResult = element.Draw(currentPage, new PointF(layoutResult.Bounds.Left + layoutResult.Bounds.Width, newBounds.Top));
                }
                else
                {
                    layoutResult = element.Draw(currentPage, new PointF(newBounds.Left, newBounds.Top));
                }
            }
        }

        private string ProcessGlobals(string textRun, PdfFont font)
        {
            if (textRun.Contains("Globals.TotalPages"))
            {
                textRun = textRun.Replace("Globals.TotalPages", this.m_pageModelFactory.PrintLayoutPageDictionary.Count.ToString());
            }
            if (textRun.Contains("Globals.ExecutionTime"))
            {
                textRun = textRun.Replace("Globals.ExecutionTime", System.DateTime.Now.ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture));
            }
            if (textRun.Contains("User.Language"))
            {
                textRun = textRun.Replace("User.Language", System.Globalization.CultureInfo.CurrentCulture.Name.ToString());
            }
            if (textRun.Contains("Globals.PageNumber"))
            {
                textRun = textRun.Replace("Globals.PageNumber", (this.m_currentPageNumber).ToString());
            }

            return textRun;
        }

        private string GetTextRun(TextboxModel txtModel,TextRunExpval run)
        {
            return run.Text;
        }

        private PdfBrush GetPdfBrush(TextRunExpval run)
        {
            colorName = run.Style.TextColor;
            return GetBrushColor(colorName);
        }

        private PdfBrush GetBrushColor(string color)
        {
            PdfBrush brush = PdfBrushes.Transparent;
            if (!string.IsNullOrEmpty(color))
            {
#if !SILVERLIGHT
                if (brushValues!=null && !brushValues.ContainsKey(color))
                {
                    brush = new PdfSolidBrush(GetColorFromHexa(color));
                    brushValues.Add(color, brush);
                }
                return brushValues[color];
#else
                if (brushValues!=null && brushValues.Count>0 && !brushValues.ContainsKey(color) && colorValues!=null && !colorValues.ContainsKey(color))
                {
#if SILVERLIGHT && !WINRT 
                    brush = new PdfSolidBrush(new PdfColor(GetColorFromHexa(color)));
                    System.Windows.Media.Color colorValue = GetColorFromHexa(color);
                    return brush;
#elif !WINRT
                    Color colorValue = ConvertStringToColor(color);
                    brush = new PdfSolidBrush(colorValue);
                    colorValues.Add(color, colorValue);
                    brushValues.Add(color, brush);
#endif


                }
#if WINRT
                Windows.UI.Color UIcolor = GetUIColorFromHexa(color);
                brush = new PdfSolidBrush(new PdfColor(UIcolor.R, UIcolor.G, UIcolor.B));
                return brush;
#endif
#if !WINRT
                return new PdfSolidBrush(GetColorFromHexa(color));
#endif
#endif

            }
#if !WINRT 
            return new PdfSolidBrush(GetColorFromHexa(color));
#else 
            Windows.UI.Color tempUIcolor = GetUIColorFromHexa("Transparent");
            PdfBrush tempbrush = new PdfSolidBrush(new PdfColor(tempUIcolor.R, tempUIcolor.G, tempUIcolor.B));
            return tempbrush;
#endif
        }

        /// <summary>
        /// Returns string values for current line in the Report
        /// </summary>
        /// <param name="text">Text value</param>
        /// <param name="result">Layout information</param>
        /// <param name="bounds">text Rectangle bounds</param>
        /// <param name="font">Font Information</param>
        /// <param name="widthVal">Font width</param>
        /// <param name="run">Text Style value</param>
        /// <returns>string value</returns>
        /// <remarks></remarks>
        private string CompleteCurrentTextLine(string text, PdfTextLayoutResult result, RectangleF bounds, PdfFont font, double widthVal, TextRunExpval run)
        {
            //available width in the current  line

            float availableWidth = 0f;
            if (result != null)
                availableWidth = PixelToPoint(widthVal) - result.LastLineBounds.Right;
            else
                availableWidth = PixelToPoint(widthVal);
            float currentWidth = 0;
            string currentWord = ReadWord(text);

            if (currentWord == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            currentWidth += font.MeasureString(currentWord).Width;

            //Compare the text size with the page clientsize and add words.
            while (currentWidth < availableWidth)
            {
                builder.Append(currentWord);
                currentWord = ReadWord(text);
                if (currentWord == null)
                    break;
                currentWidth += font.MeasureString(currentWord).Width;
            }

            PdfTextElement textElement = new PdfTextElement();
            textElement.Text = builder.ToString();
            textElement.Font = font;
            textElement.Brush = GetPdfBrush(run);
            if (result != null)
                result = textElement.Draw(currentPage, new System.Drawing.PointF(result.LastLineBounds.Right, result.LastLineBounds.Y)) as PdfTextLayoutResult;
            else
                result = textElement.Draw(currentPage, new System.Drawing.PointF(bounds.X, bounds.Y)) as PdfTextLayoutResult;

            return builder.ToString();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string ReadWord(string text)
        {
            int pos = m_position;

            while (pos < text.Length)
            {
                char ch = text[pos];

                switch (ch)
                {

                    case ' ':
                    case '\t':
                        {
                            if (pos == m_position)
                            {
                                pos++;
                            }

                            string text1 = text.Substring(m_position, pos - m_position);
                            m_position = pos;
                            return text1;
                        }

                }
                pos++;
            }

            // The remaining text.
            if (pos > m_position)
            {
                string text2 = text.Substring(m_position, pos - m_position);

                m_position = pos;

                return text2;
            }

            return null;
        }
        /// <summary>
        /// Get the PdfFont
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private PdfFont GetPdfFont(TextRunExpval run)
        {
            string fontname = null;

            if (run.Text!=null &&!IsUnicode(run.Text))
            {
                fontname = run.Style.Font.FontFamily;
            }
            else
            {
                fontname = "Arial Unicode MS";
            }

#if SILVERLIGHT
            PdfFont font=null;
            float fontsize=(float)run.Style.Font.FontSize;
#if !WINRT
            //PdfFontFamily fpntfamily ;
#endif
            PdfFontStyle fontstyle=GetFontStyle(run);

            if (this.PDFFonts != null && this.PDFFonts.ContainsKey(fontname))
            {
                font = new PdfTrueTypeFont(this.PDFFonts[fontname], (float)(fontsize/1.3333333), fontstyle);
            }

            else
            {
                font = new PdfStandardFont(PdfFontFamily.TimesRoman, (float)(fontsize / 1.3333333), fontstyle);
            }

            return font;
#else

            string fontKey = "" + run.Style.Font.FontWeight + "," + run.Style.Font.FontStyle + ",";
            fontKey += string.IsNullOrEmpty(run.Style.TextColor) ? "," : run.Style.TextColor;
            fontKey = fontKey + run.Style.Font.FontSize + "," + fontname;

            if (!fontValues.ContainsKey(fontKey))
            {


                float fontSize = PixelToPoint(run.Style.Font.FontSize);
                System.Drawing.FontStyle fontStyle = GetFontStyle(run);
                System.Drawing.Font font = new System.Drawing.Font(fontname, fontSize, fontStyle);
                this.fontValues.Add(fontKey, new PdfTrueTypeFont(font , true));
            }

            return this.fontValues[fontKey];
#endif

        }

        internal bool IsUnicode(string text)
        {
            try
            {
                const char lowestHanCodepoint = '\u4E00';
                const char highestHanCodepoint = '\u9FFF';
                if (text != null)
                {
                    foreach (char c in text)
                    {
                        if (lowestHanCodepoint <= c && c <= highestHanCodepoint)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

#if SILVERLIGHT
        private PdfFontStyle GetFontStyle(TextRunExpval run)
        {
            PdfFontStyle style = PdfFontStyle.Regular;
            bool hasStyle = false;

            string runFontStyle = run.Style.Font.FontStyle.ToString().ToLower();

            if (runFontStyle == "italic")
            {
                if (!hasStyle)
                    style = PdfFontStyle.Italic;
                else
                    style |= PdfFontStyle.Italic;
            }
            else if (runFontStyle == "normal")
            {
                if (!hasStyle)
                    style = PdfFontStyle.Regular;
                else
                    style |= PdfFontStyle.Regular;
            }

            string fontWeight = run.Style.Font.FontWeight.ToString().ToLower();

            switch (fontWeight)
            {
                case "bold":
                case "extrabold":
                case "ultrabold":
                case "black":
                case "heavy":
                case "extrablack":
                case "ultrablack":
                    if (!hasStyle)
                        style = PdfFontStyle.Bold;
                    else
                        style |= PdfFontStyle.Bold;
                    break;

            }

            return style;
        }

#else

        /// <summary>
        /// Gets the Font style for PdfFont
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private System.Drawing.FontStyle GetFontStyle(TextRunExpval run)
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            bool hasStyle = false;

            string runFontStyle = run.Style.Font.FontStyle.ToString().ToLower();

            if (runFontStyle == "italic")
            {
                if (!hasStyle)
                    style = System.Drawing.FontStyle.Italic;
                else
                    style |= System.Drawing.FontStyle.Italic;
            }
            else if (runFontStyle == "normal")
            {
                if (!hasStyle)
                    style = System.Drawing.FontStyle.Regular;
                else
                    style |= System.Drawing.FontStyle.Regular;
            }

            string fontWeight = run.Style.Font.FontWeight.ToString().ToLower();

            switch (fontWeight)
            {
                case "bold":
                case "extrabold":
                case "ultrabold":
                case "black":
                case "heavy":
                case "extrablack":
                case "ultrablack":
                    if (!hasStyle)
                        style = System.Drawing.FontStyle.Bold;
                    else
                        style |= System.Drawing.FontStyle.Bold;
                    break;

            }

            return style;
        }
#endif

        /// <summary>
        /// Gets the PDF alignment
        /// </summary>
        /// <param name="textAlignment"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private PdfTextAlignment GetAlignment(string textAlignment)
        {
            switch (textAlignment)
            {
                case "Center":
                    return PdfTextAlignment.Center;
                case "Justify":
                    return PdfTextAlignment.Justify;
                case "Right":
                    return PdfTextAlignment.Right;
                default:
                    return PdfTextAlignment.Left;
            }
        }

        /// <summary>
        /// Update the parent bounds
        /// </summary>
        /// <remarks></remarks>
        private void UpdateParentOffset()
        {
            object parent = parentModel.Peek();
            double topValue = 0;
            double leftVal = 0;
            double heightVal = 0;
            double widthVal = 0;

            if (parent is IReportItemModeler)
            {
                IReportItemModeler modeler = (IReportItemModeler)parent;

                if ((m_bIsInList || IsHeaderFooterTemplate) && modeler.FlowLayoutInfo != null)
                {
                    if (modeler.IsTablixChild)
                    {
                        topValue = modeler.Top;
                        leftVal = modeler.Left;
                        heightVal = modeler.Height;
                        widthVal = modeler.Width;
                    }
                    else
                    {
                        topValue = modeler.FlowLayoutInfo.ActualTop;
                        leftVal = modeler.FlowLayoutInfo.ActualLeft;
                        heightVal = modeler.FlowLayoutInfo.ActualHeight;
                        widthVal = modeler.FlowLayoutInfo.ActualWidth;
                    }
                }
                else if (modeler.PrintPageInfo != null)
                {
                    topValue = modeler.PrintPageInfo.ActualTop;
                    leftVal = modeler.PrintPageInfo.ActualLeft;
                    heightVal = modeler.PrintPageInfo.ActualHeight;
                    widthVal = modeler.PrintPageInfo.ActualWidth;
                }
                else
                {
                    topValue = modeler.Top;
                    leftVal = modeler.Left;
                    heightVal = modeler.Height;
                    widthVal = modeler.Width;
                }
            }
            else
            {
                PointF offsetValue = (PointF)parent;
                leftVal = offsetValue.X;
                topValue = offsetValue.Y;
            }
            parentX += PixelToPoint(leftVal);
            parentY += PixelToPoint(topValue);
        }

        /// <summary>
        /// Clears the parent offset
        /// </summary>
        /// <remarks></remarks>
        private void ClearParentOffset()
        {
            if (parentModel.Count > 0)
            {
                object parent = parentModel.Peek();
                double topValue = 0;
                double leftVal = 0;
                double heightVal = 0;
                double widthVal = 0;

                if (parent is IReportItemModeler)
                {
                    IReportItemModeler modeler = (IReportItemModeler)parent;

                    if ((m_bIsInList || IsHeaderFooterTemplate) && modeler.FlowLayoutInfo != null)
                    {
                        if (modeler.IsTablixChild)
                        {
                            topValue = modeler.Top;
                            leftVal = modeler.Left;
                            heightVal = modeler.Height;
                            widthVal = modeler.Width;
                        }
                        else
                        {
                            topValue = modeler.FlowLayoutInfo.ActualTop;
                            leftVal = modeler.FlowLayoutInfo.ActualLeft;
                            heightVal = modeler.FlowLayoutInfo.ActualHeight;
                            widthVal = modeler.FlowLayoutInfo.ActualWidth;
                        }
                    }
                    else if (modeler.PrintPageInfo != null)
                    {
                        topValue = modeler.PrintPageInfo.ActualTop;
                        leftVal = modeler.PrintPageInfo.ActualLeft;
                        heightVal = modeler.PrintPageInfo.ActualHeight;
                        widthVal = modeler.PrintPageInfo.ActualWidth;
                    }
                    else
                    {
                        topValue = modeler.Top;
                        leftVal = modeler.Left;
                        heightVal = modeler.Height;
                        widthVal = modeler.Width;
                    }
                }
                else
                {
                    PointF offsetValue = (PointF)parent;
                    leftVal = offsetValue.X;
                    topValue = offsetValue.Y;
                }

                parentX -= PixelToPoint(leftVal);
                parentY -= PixelToPoint(topValue);

                parentModel.Pop();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Report"></param>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private PageModelFactory UpdatePageLayoutForPDF(Syncfusion.RDL.DOM.ReportDefinition Report, ReportModel ReportModel)
        {
            if (!this.ReportModel.IsEvaluatedReport)
            {
                if (this.DataSources.Count == 0)
                {
                    ReportModel.InitilizeReport();
                }
                else
                {
                    ReportModel.IsRDLC = true;
                    ReportModel.DataSources = this.DataSources;
                    ReportModel.InitilizeReport();
                }

                this.ReportModel.Evaluate();
                this.ReportModel.UpdateSize();
            }

            PageModelFactory pageModelFactory = new PageModelFactory(this.ReportModel);

            double leftMargin, rightMargin, topMargin, bottomMargin;

            if (this.PageSettings != null)
            {
                pageModelFactory.PageWidth = PageSettings.PageWidth;
                pageModelFactory.PageHeight = PageSettings.PageHeight;

                leftMargin = PageSettings.LeftMargin;
                rightMargin = PageSettings.RightMargin;
                topMargin = PageSettings.TopMargin;
                bottomMargin = PageSettings.BottomMargin;
            }
            else
            {
                pageModelFactory.PageWidth = ReportModel.Page.PageWidth.PixelValue;
                pageModelFactory.PageHeight = ReportModel.Page.PageHeight.PixelValue;

                leftMargin = ReportModel.Page.LeftMargin != null && ReportModel.Page.LeftMargin.size != null ? ReportModel.Page.LeftMargin.PixelValue : 0;
                rightMargin = ReportModel.Page.RightMargin != null && ReportModel.Page.RightMargin.size != null ? ReportModel.Page.RightMargin.PixelValue : 0;
                topMargin = ReportModel.Page.TopMargin != null && ReportModel.Page.TopMargin.size != null ? ReportModel.Page.TopMargin.PixelValue : 0;
                bottomMargin = ReportModel.Page.BottomMargin != null && ReportModel.Page.BottomMargin.size != null ? ReportModel.Page.BottomMargin.PixelValue : 0;
            }

            pageModelFactory.Margin = new LayoutThicknessInfo(leftMargin, topMargin, rightMargin, bottomMargin);

            pageModelFactory.UpdatePageLayout();
            //pageModelFactory.IsPrintMode = true;

            return pageModelFactory;
        }

#if !SILVERLIGHT
        private BitmapImage BufferImage(object imageData)
        {
            byte[] imageDataValue = imageData as byte[];

            if (imageData != null)
            {
                BitmapImage bi = new BitmapImage();
                MemoryStream stream = new MemoryStream();
                int offset = 78;
                stream.Write(imageDataValue, offset, imageDataValue.Length - offset);
                bi.BeginInit();
                bi.StreamSource = stream;
                bi.EndInit();
                return bi;
            }
            return null;
        }
#endif
    }

}
