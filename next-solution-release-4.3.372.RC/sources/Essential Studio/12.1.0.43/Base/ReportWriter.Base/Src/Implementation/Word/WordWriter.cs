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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;

#if !SILVERLIGHT
using System.Data.Sql;
#endif
using System.Globalization;
using System.Collections;
using System.Threading;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.ItemModel;
#if WINRT 
using Windows.UI.Xaml.Media.Imaging;
using Syncfusion.UI.Xaml.Reports;
using Windows.UI.Xaml;
#else
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Reports;
using System.Windows;
#endif

namespace Syncfusion.ReportWriter
{
    /// <summary>
    /// Exports the report in Word mode.
    /// </summary>
    /// <remarks></remarks>
    public class WordWriter : WriterBase
    {
        WordDocument document = null;
        IWSection section = null;
        IWParagraph paragraph = null;
        IWTable table = null;
        IWTable wordTable = null;
        TablixCellInfo m_currentListCellInfo;
        List<WordReportItemCellModel> reportItemCellModels;
        WordLayoutHelper layoutHelper = new WordLayoutHelper();
        List<WordReportItemCellValue> cellModels = new List<WordReportItemCellValue>();
        List<WordReportItemCellValue> listCellModels = new List<WordReportItemCellValue>();
        List<WordReportItemCellValue> tableCellModels = new List<WordReportItemCellValue>();
        //List<WordReportItemCellValue> rectCellModels = new List<WordReportItemCellValue>();
        //List<WordReportItemCellModel> listReportItemCellModels;
        List<WordReportItemCellModel> tableReportItemCellModels;
        //List<WordReportItemCellModel> rectReportItemCellModels;
        List<double> nestedRowHeights = new List<double>();
        List<double> nestedColumnWidths = new List<double>();
        List<double> rectRowHeights = new List<double>();
        List<double> rectColumnWidths = new List<double>();
        List<double> rowHeights = new List<double>();
        List<double> columnWidths = new List<double>();
        int TotalPages { get; set; }
        int CurrentPage { get; set; }
        List<IReportItemModeler> listValues = new List<IReportItemModeler>();
        bool isListProcess = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.ReportWriter.WordWriter">WordWriter</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public WordWriter()
        {
            DataSources = new ReportDataSourceCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.ReportWriter.WordWriter">WordWriter</see> class. 
        /// </summary>
        /// <param name="rdlFilename">The full path of the file</param>
        /// <remarks></remarks>
        public WordWriter(string rdlFilename)
            : this()
        {
            if (rdlFilename == null || rdlFilename.Length == 0 || rdlFilename == string.Empty)
            {
                throw new ArgumentException("Filename should be null or empty");
            }
            ReportPath = rdlFilename;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.ReportWriter.WordWriter">WordWriter</see> class. 
        /// </summary>
        /// <param name="rdlStream">Stream of the file</param>
        /// <remarks></remarks>
        public WordWriter(Stream rdlStream)
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
        /// Initializes a new instance of the <see cref="T:Syncfusion.ReportWriter.WordWriter">WordWriter</see> class. 
        /// </summary>
        /// <param name="rdlFilename">The full path of the file</param>
        /// <param name="reportDataSources">ReportDatasource</param>
        /// <remarks></remarks>
        public WordWriter(string rdlFilename, ReportDataSourceCollection reportDataSources)
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
        /// Initializes a new instance of the <see cref="T:Syncfusion.ReportWriter.WordWriter">WordWriter</see> class. 
        /// </summary>
        /// <param name="rdlStream">stream of the RDL file</param>
        /// <param name="reportDataSources">Report Datasources </param>
        /// <remarks></remarks>
        public WordWriter(Stream rdlStream, ReportDataSourceCollection reportDataSources)
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

#if !SILVERLIGHT
        /// <summary>
        /// Exports the Report
        /// </summary>
        /// <param name="fileName">File name to save the Report</param>
        /// <param name="response">Http Response</param>
        /// <remarks></remarks>
        public void Save(string fileName, System.Web.HttpResponse response)
        {
            if (this.ReportModel.HasReport)
            {
                Thread thread = new Thread(delegate()
                {
                   PageModelFactory reportModelFactory = this.UpdatePageLayoutForWord(this.ReportModel);
                   document = this.ConvertToDoc(reportModelFactory, ReportModel);
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                thread.Join();
                document.Save(fileName, FormatType.Doc, response, HttpContentDisposition.Attachment);
            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }
#endif

#if !WINRT 
        /// <summary>
        /// Exports the Report as word file
        /// </summary>
        /// <param name="DocFilename">File name to save the Report</param>
        /// <remarks></remarks>
        public void Save(string DocFilename)
        {
            using (FileStream docStream = new FileStream(DocFilename, FileMode.Create))
            {
                Save(docStream);
            }
        }
#endif
        /// <summary>
        /// Exports the Report as Word file
        /// </summary>
        /// <param name="DocStream">Stream to save the Report</param>
        /// <remarks></remarks>
        public void Save(Stream DocStream)
        {
            if (this.ReportModel.HasReport)
            {
                PageModelFactory reportModelFactory = this.UpdatePageLayoutForWord(this.ReportModel);
                document = this.ConvertToDoc(reportModelFactory, ReportModel);
                document.Save(DocStream, (FormatType)Enum.Parse(typeof(FormatType),this.WordFormatType.ToString(),true));
            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }

        private PageModelFactory UpdatePageLayoutForWord(ReportModel reportModel)
        {
            if (!this.ReportModel.IsEvaluatedReport)
            {
                if (this.DataSources.Count == 0)
                {
                    reportModel.InitilizeReport();
                }
                else
                {
                    reportModel.IsRDLC = true;
                    reportModel.DataSources = this.DataSources;
                    reportModel.InitilizeReport();
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

            pageModelFactory.UpdateFlowPageLayout();
            return pageModelFactory;
        }

        internal WordDocument ConvertToDoc(PageModelFactory factory, ReportModel reportModel)
        {
            document = new WordDocument();
            layoutHelper.WordPageModelFactoty = factory;
            this.TotalPages = layoutHelper.WordPageModelFactoty.FlowLayoutDictionary.Count();

            foreach (var page in layoutHelper.WordPageModelFactoty.FlowLayoutDictionary)
            {
                section = document.AddSection();
                section.PageSetup.FooterDistance = this.PixelToPoint(layoutHelper.WordPageModelFactoty.HeaderHeight);
                section.PageSetup.HeaderDistance = this.PixelToPoint(layoutHelper.WordPageModelFactoty.HeaderHeight);
                this.CurrentPage = page.Key+1;

                if (layoutHelper.WordPageModelFactoty.HeaderHeight > 0)
                {
                    if (reportModel.Report.Page != null && reportModel.Report.Page.PageHeader != null && reportModel.Report.Page.PageHeader.ReportItems.Count != 0)
                    {
                        HeaderFooter header = document.Sections[page.Key].HeadersFooters.OddHeader;
                        table = header.AddTable();
                        this.UpdateTable(UpdateSection.Header, reportModel, page.Key);
                    }
                    else if (reportModel.Page != null && reportModel.Page.PageHeader != null && reportModel.Page.PageHeader.ReportItems.Count != 0)
                    {
                        HeaderFooter header = document.Sections[page.Key].HeadersFooters.OddHeader;
                        table = header.AddTable();
                        this.UpdateTable(UpdateSection.Header, reportModel,page.Key);
                    }
                }

                if (layoutHelper.WordPageModelFactoty.FooterHeight > 0)
                {
                    if (reportModel.Page != null && reportModel.Page.PageFooter != null && reportModel.Page.PageFooter.ReportItems.Count != 0)
                    {
                        HeaderFooter footer = document.Sections[page.Key].HeadersFooters.OddFooter;
                        table = footer.AddTable();
                        this.UpdateTable(UpdateSection.Footer, reportModel, page.Key);
                    }
                }

                table = section.AddTable();
                this.UpdateTable(UpdateSection.Body, reportModel, page.Key);

                double totalWidth = 0;

                for (int column = 0; column < this.columnWidths.Count(); column++)
                {
                    totalWidth += this.PixelToPoint(this.columnWidths[column]);
                }

                if (reportModel != null)
                {
                    float top = PixelToPoint(factory.Margin.Top);
                    float Bottom = PixelToPoint(factory.Margin.Bottom);
                    float left= PixelToPoint(factory.Margin.Left);
                    float right = PixelToPoint(factory.Margin.Right);

                    float height = (float)(reportModel.Page.PageHeight.PixelValue * 0.75);
                    float width = (float)(left + right + totalWidth);

#if WINRT 
                    section.PageSetup.PageSize = new Syncfusion.DocIO.DLS.SizeF(width, height);
#else 
                    section.PageSetup.PageSize = new SizeF(width, height);
#endif
                }
                if (this.cellModels != null)
                {
                    this.DisposeCellModels(this.cellModels);
                    this.cellModels.Clear();
                }

                if (this.reportItemCellModels != null)
                {
                    this.DisposeCellModels(this.reportItemCellModels);
                    this.reportItemCellModels.Clear();
                }

                this.cellModels = null;
                this.reportItemCellModels = null;
            }
            return this.document;
        }

        private void DisposeCellModels(List<WordReportItemCellModel> disposeModels)
        {
            foreach (var model in disposeModels)
            {
                model.DataSource = null;
                model.FieldValues = null;
                model.WordReportItemModel = null;
                model.WordCellInfo = null;
            }
        }

        private void DisposeCellModels(List<WordReportItemCellValue> disposeValues)
        {
            foreach (var cellValue in disposeValues)
            {
                cellValue.WordReportItemCellModel = null;
                cellValue.WordRight = null;
            }
        }

        private void UpdateTable(UpdateSection updateSection, ReportModel reportModel, int section)
        {
            this.cellModels = new List<WordReportItemCellValue>();

            this.reportItemCellModels = layoutHelper.GetReportItemCellModels(updateSection, section);
            this.rowHeights = layoutHelper.UpdateRowHeightValues(this.reportItemCellModels, this.cellModels, updateSection);
            this.columnWidths = layoutHelper.UpdateColumnWidthValues(this.reportItemCellModels, this.cellModels, updateSection);
            table.Rows.Clear();
            table.ResetCells(this.rowHeights.Count, this.columnWidths.Count + 1);
            table.TableFormat.Borders.BorderType = BorderStyle.None;
            table.TableFormat.IsAutoResized = true;
            table.TableFormat.Paddings.All = 0;
            for (int i = 0; i < this.rowHeights.Count; i++)
            {
                WTableRow row = table.Rows[i];
                row.Height = this.PixelToPoint(this.rowHeights[i]);

                for (int j = 0; j < this.columnWidths.Count; j++)
                {
                    row.Cells[j].Width = this.PixelToPoint(this.columnWidths[j]);
                }
            }
            foreach (var cellModel in this.cellModels)
            {
                if (cellModel.WordReportItemCellModel.WordCellInfo != null && cellModel.WordReportItemCellModel.WordCellInfo.ItemModel.ModelType == ModelType.TablixModel)
                {
                    ProcessTablixModel(cellModel, reportModel, null, this.tableCellModels);
                }
                else
                {
                    this.ProcessReportModels(cellModel, reportModel, null);
                }
            }
        }

        private float PixelToPoint(double paramValue)
        {
            float value = Convert.ToSingle(paramValue);
            return PixelToPoint(value);
        }

        private float PixelToPoint(float paramValue)
        {
            return (paramValue * 0.75f);
        }

        private void ProcessReportModels(WordReportItemCellValue cellModel, ReportModel reportModel,IWTable rectTable)
        {
            //if (cellModel.WordReportItemCellModel.WordReportItemModel != null)
            //{
                this.ProcessCellModel(cellModel, reportModel, rectTable);
            //}
        }

        void ProcessCellModel(WordReportItemCellValue cellModel, ReportModel reportModel, IWTable rectTable)
        {
            if (cellModel.WordReportItemCellModel.WordCellInfo!=null && cellModel.WordReportItemCellModel.WordCellInfo.ItemModel != null)
            {
                switch (cellModel.WordReportItemCellModel.WordCellInfo.ItemModel.ModelType)
                {
                    case ModelType.TextBoxModel:
                        ProcessTextBox(cellModel, reportModel, rectTable);
                        break;
                    case ModelType.ImageModel:
                        ProcessImageModel(cellModel, rectTable);
                        break;
                    case ModelType.LineModel:
                        ProcessLineModel(cellModel, rectTable);
                        break;
                    case ModelType.RectangleModel:
                        ProcessRectangleModel(cellModel, reportModel, rectTable);
                        break;
                    case ModelType.GaugeModel:
                        ProcessGaugeModel(cellModel, rectTable);
                        break;
                    case ModelType.ChartModel:
                        ProcessChartModel(cellModel, rectTable);
                        break;
                    case ModelType.TablixModel:
                        ProcessTablixModel(cellModel, reportModel, rectTable, this.tableCellModels);
                        break;
                        
#if !SyncfusionFramework3_5
                    case ModelType.MapModel:
                        ProcessMapModel(cellModel, rectTable);
                        break;
#endif
                    default:
                        return;
                }
            }
            else
            {
                switch (cellModel.WordReportItemCellModel.WordReportItemModel.ModelType)
                {
                    case ModelType.TextBoxModel:
                        ProcessTextBox(cellModel, reportModel, rectTable);
                        break;
                    case ModelType.ImageModel:
                        ProcessImageModel(cellModel, rectTable);
                        break;
                    case ModelType.LineModel:
                        ProcessLineModel(cellModel, rectTable);
                        break;
                    case ModelType.RectangleModel:
                        ProcessRectangleModel(cellModel, reportModel, rectTable);
                        break;
                    case ModelType.GaugeModel:
                        ProcessGaugeModel(cellModel, rectTable);
                        break;
                    case ModelType.ChartModel:
                        ProcessChartModel(cellModel, rectTable);
                        break;
                    case ModelType.TablixModel:
                        ProcessTablixModel(cellModel, reportModel, rectTable, this.tableCellModels);
                        break;
                        
#if !SyncfusionFramework3_5
                    case ModelType.MapModel:
                        ProcessMapModel(cellModel, rectTable);
                        break;
#endif
                    default:
                        return;
                }
            }
        }

        private void UpdateMergeBorder(WordReportItemCellValue cellValue,IWTable rectTable)
        {
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            int lastRow = 0;
            if (cellValue.WordRowSpan < 0)
            {
                lastRow = row;
            }
            else
            {
                lastRow = row + cellValue.WordRowSpan;
            }
            int lastColumn = column + cellValue.WordColumnSpan;

            if (rectTable != null)
            {
                wordTable = rectTable;
            }
            else
            {
                wordTable = table;
            }

            if (row != lastRow && column != lastColumn)
            {
                wordTable.Rows[row].Cells[column].CellFormat.VerticalMerge = CellMerge.Start;

                for (int vRowCount = row +1 ; vRowCount <= lastRow; vRowCount++)
                {
                    wordTable.Rows[vRowCount].Cells[column].CellFormat.VerticalMerge = CellMerge.Continue;
                }

                for (int hRowCount = row; hRowCount <= lastRow; hRowCount++)
                {
                    wordTable.Rows[hRowCount].Cells[column].CellFormat.HorizontalMerge = CellMerge.Start;

                    int col = column + 1;
                    while (col <= lastColumn)
                    {
                        wordTable.Rows[hRowCount].Cells[col].CellFormat.HorizontalMerge = CellMerge.Continue;
                        ++col;
                    }
                }
            }
            else if (column != lastColumn)
            {
                wordTable.Rows[row].Cells[column].CellFormat.HorizontalMerge = CellMerge.Start;

                for (int hColumnCount = column + 1; hColumnCount <= lastColumn; hColumnCount++)
                {
                    wordTable.Rows[row].Cells[hColumnCount].CellFormat.HorizontalMerge = CellMerge.Continue;
                }
            }
            else if ( row != lastRow)
            {
                wordTable.Rows[row].Cells[column].CellFormat.VerticalMerge = CellMerge.Start;

                for (int hRowCount = row +1 ; hRowCount <= lastRow; hRowCount++)
                {
                    wordTable.Rows[hRowCount].Cells[lastColumn].CellFormat.VerticalMerge = CellMerge.Continue;
                }
            }

           Thickness border = new Thickness(0);

            if (!cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (cellValue.WordReportItemCellModel.WordReportItemModel.ModelType == ModelType.TextBoxModel)
                {
                    var textbox = (TextboxModel)cellValue.WordReportItemCellModel.WordReportItemModel;
                    if (textbox.TextBoxProperties != null)
                    {
                        if (textbox.TextBoxProperties.Border.Default != null && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.None)
                        {
                            border = new Thickness(textbox.TextBoxProperties.Border.Default.Thickness);//((TextboxModel)cellValue.WordReportItemCellModel.WordReportItemModel)
                        }
                    }
                }
            }

            if (border.Top != 0)
            {
                if (row != lastRow && column != lastColumn)
                {
                    for (int rowVal = row; rowVal <= lastRow; rowVal++)
                    {
                        for (int colVal = column; colVal <= lastColumn; colVal++)
                        {
                            wordTable.Rows[rowVal].Cells[colVal].CellFormat.Borders.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        }
                    }
                }
                else if (row != lastRow)
                {
                    for (int rowVal = row; rowVal <= lastRow; rowVal++)
                    {
                        wordTable.Rows[rowVal].Cells[column].CellFormat.Borders.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
                else if (column != lastColumn)
                {
                    for (int colVal = column; colVal <= lastColumn; colVal++)
                    {
                        wordTable.Rows[row].Cells[colVal].CellFormat.Borders.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
                else
                {
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                }
            }
        }

        private void ProcessImageModel(WordReportItemCellValue cellValue, IWTable nestTable)
        {
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            int lastRow = row + cellValue.WordRowSpan;
            int lastColumn = column + cellValue.WordColumnSpan;
            this.UpdateMergeBorder(cellValue, nestTable);
            if (nestTable != null)
            {
                wordTable = nestTable;
            }
            else
            {
                wordTable = table;
            }

            if (cellValue.WordReportItemCellModel.WordCellInfo != null )
            {
                cellValue.WordReportItemCellModel.WordCellInfo.Evaluate();
            }

            IReportItemModeler reportModel = cellValue.WordReportItemCellModel.WordReportItemModel;

            if (cellValue.WordReportItemCellModel.WordReportItemModel != null)
            {
                reportModel = cellValue.WordReportItemCellModel.WordReportItemModel;
            }
            else if (cellValue.WordReportItemCellModel.WordCellInfo != null)
            {
                cellValue.WordReportItemCellModel.WordCellInfo.Evaluate();
                reportModel = cellValue.WordReportItemCellModel.WordCellInfo.ItemModel;
            }

            if (reportModel != null && cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DataSource = cellValue.WordReportItemCellModel.DataSource;
                    reportModel.FieldValues = cellValue.WordReportItemCellModel.FieldValues;
                    reportModel.RowNumbers = cellValue.WordReportItemCellModel.RowNumbers;
                }
                reportModel.Evaluate();
            }

            ImageModel imageModel = reportModel as ImageModel;
            object imageData = imageModel.ImageData;            

            Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
            if (imageData == null)
                return;


#if ! SILVERLIGHT
#if !WINRT
            Stream imageStream = null;
#endif
            try
            {
                imageStream = ((BitmapImage)base64ImageConverter.ConvertToImage(imageModel.ImageData)).StreamSource;
            }
            catch
            {
                imageStream = ((BitmapImage)BufferImage(imageModel.ImageData)).StreamSource;
            }
#endif
            paragraph = wordTable.Rows[row].Cells[column].AddParagraph();

#if SILVERLIGHT
                        IWPicture picture1 = paragraph.AppendPicture(imageData as byte[]);

#else
                        IWPicture picture1 = paragraph.AppendPicture(new Bitmap(imageStream));

#endif
            if (reportModel.FlowLayoutInfo != null)
            {
                picture1.Height = (this.PixelToPoint((reportModel.FlowLayoutInfo.ActualHeight)) - 1);
                picture1.Width = (this.PixelToPoint((reportModel.FlowLayoutInfo.ActualWidth)) - 1);
            }
            else
            {
                picture1.Height = this.PixelToPoint(reportModel.Height);
                picture1.Width = this.PixelToPoint(reportModel.Width);
            }

            if (imageModel.ImageProperties.Border.Default != null)
            {

#if !SILVERLIGHT
                Color borderColor = System.Drawing.ColorTranslator.FromHtml(imageModel.ImageProperties.Border.Default.BorderBrush);
#elif !WINRT
                System.Windows.Media.Color color = GetColorFromHexa(imageModel.ImageProperties.Border.Default.BorderBrush);
                Color borderColor = Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                Syncfusion.DocIO.DLS.Color borderColor = GetColorFromString(imageModel.ImageProperties.Border.Default.BorderBrush);
#endif

                if (imageModel.ImageProperties.Border.Default.Thickness > 0 && imageModel.ImageProperties.Border.Default.BorderStyle != Syncfusion.RDL.DOM.BorderStyles.None 
                    && imageModel.ImageProperties.Border.Default.BorderStyle != Syncfusion.RDL.DOM.BorderStyles.Default)
                {
                    borderColor = this.ConvertStringToColor(imageModel.ImageProperties.Border.Default.BorderBrush.ToString());
#if WINRT
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = borderColor;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = borderColor;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = borderColor;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = borderColor;
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
#else
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)imageModel.ImageProperties.Border.Default.Thickness;
#endif
                }
                else
                {
#if WINRT 
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;

#else
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Color.Transparent;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Color.Transparent;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Color.Transparent;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Color.Transparent;

#endif
                }
            }
            else
            {
#if WINRT 
                wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.Transparent;

#else
                wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Color.Transparent;
                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Color.Transparent;
                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Color.Transparent;
                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Color.Transparent;

#endif
            }

            if (reportModel != null && cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DisposeEvalObjects();
                }
                else
                {
                    if (cellValue.WordReportItemCellModel.WordCellInfo != null)
                    {
                        cellValue.WordReportItemCellModel.WordCellInfo.DisposeEvalObjects();                        
                    }
                }
            }
        }

        private void ProcessGaugeModel(WordReportItemCellValue cellValue, IWTable nestTable)
        {
            int row = cellValue.WordRowIndex-1;
            int column = cellValue.WordColumnIndex-1;
            this.UpdateMergeBorder(cellValue, nestTable);
            if (nestTable!=null)
            {
                wordTable = nestTable;
            }
            else
            {
                wordTable = table;
            }

            bool isTablixChild = cellValue.WordReportItemCellModel.IsTablixCell;

            IReportItemModeler reportModel = isTablixChild ? cellValue.WordReportItemCellModel.WordCellInfo.ItemModel : cellValue.WordReportItemCellModel.WordReportItemModel;  
            GaugeModel gaugeModel = reportModel as GaugeModel;
            Stream gaugeStream = gaugeModel.GetImageStream();
            if (gaugeStream != null)
            {
                paragraph = wordTable.Rows[row].Cells[column].AddParagraph();
#if SILVERLIGHT
                IWPicture picture1 = paragraph.AppendPicture(gaugeStream);
#else
                IWPicture picture1 = paragraph.AppendPicture(new Bitmap(gaugeStream));

#endif
                if (reportModel.FlowLayoutInfo != null)
                {
                    picture1.Height = (this.PixelToPoint((reportModel.FlowLayoutInfo.ActualHeight)) - 1);
                    picture1.Width = (this.PixelToPoint((reportModel.FlowLayoutInfo.ActualWidth)) - 1);
                }
                else
                {
                    picture1.Height = this.PixelToPoint(reportModel.Height);
                    picture1.Width = this.PixelToPoint(reportModel.Width);
                }
            }

            if(gaugeModel.GaugePanelProperties != null && gaugeModel.GaugePanelProperties.Border != null)
            {
                 this.ApplyBorder(gaugeModel.GaugePanelProperties.Border,cellValue);
            }
        }

        private void ProcessChartModel(WordReportItemCellValue cellValue, IWTable nestTable)
        {
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            this.UpdateMergeBorder(cellValue, nestTable);
            if (nestTable!=null)
            {
                wordTable = nestTable;
            }
            else
            {
                wordTable = table;
            }

            bool isTablixChild = cellValue.WordReportItemCellModel.IsTablixCell;
            IReportItemModeler reportModel = isTablixChild ? cellValue.WordReportItemCellModel.WordCellInfo.ItemModel : cellValue.WordReportItemCellModel.WordReportItemModel;
            ChartModel chartModel = reportModel as ChartModel;
            Stream chartStream = chartModel.GetImageStream();

            
            if (chartStream != null)
            {
                paragraph = wordTable.Rows[row].Cells[column].AddParagraph();
                IWPicture picture1 = null;
#if SILVERLIGHT
                try
                {
                    picture1 = paragraph.AppendPicture(chartStream);
                }
                catch
                {

                }
#else
                picture1 = paragraph.AppendPicture(new Bitmap(chartStream));
#endif
                picture1.Width = this.PixelToPoint(reportModel.Width);
                picture1.Height = this.PixelToPoint(reportModel.Height);
            }
            if (chartModel.ChartProperties != null && chartModel.ChartProperties.Border != null)
            {
                this.ApplyBorder(chartModel.ChartProperties.Border, cellValue);
            }
        }
        
#if !SyncfusionFramework3_5
        private void ProcessMapModel(WordReportItemCellValue cellValue, IWTable nestTable)
        {
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            this.UpdateMergeBorder(cellValue, nestTable);
            if (nestTable != null)
            {
                wordTable = nestTable;
            }
            else
            {
                wordTable = table;
            }

            bool isTablixChild = cellValue.WordReportItemCellModel.IsTablixCell;
            IReportItemModeler reportModel = isTablixChild ? cellValue.WordReportItemCellModel.WordCellInfo.ItemModel : cellValue.WordReportItemCellModel.WordReportItemModel;  
            MapModel mapModel = reportModel as MapModel;
            Stream mapStream = mapModel.GetImageStream();
            if (mapStream != null)
            {
                paragraph = wordTable.Rows[row].Cells[column].AddParagraph();
#if !SILVERLIGHT
                IWPicture picture1 = paragraph.AppendPicture(new Bitmap(mapStream));
#else
                IWPicture picture1 = paragraph.AppendPicture(mapStream);
#endif

                picture1.Width = this.PixelToPoint(reportModel.Width);
                picture1.Height = this.PixelToPoint(reportModel.Height);
            }

            if(mapModel.MapProperties != null && mapModel.MapProperties.Border != null)
            {
                 this.ApplyBorder(mapModel.MapProperties.Border,cellValue);                
            }
        }
#endif

        private void ApplyBorder(BorderExpval border, WordReportItemCellValue cellValue)
        {
            if (border != null)
            {
                int row = cellValue.WordRowIndex - 1;
                int column = 0;
                if (cellValue.WordColumnIndex == 0)
                {
                    column = cellValue.WordColumnIndex;
                }
                else
                {
                    column = cellValue.WordColumnIndex - 1;
                }
                int lastRow = row + cellValue.WordRowSpan;
                int lastColumn = column + cellValue.WordColumnSpan;

                if (border.Default != null && border.Default.BorderBrush != null && border.Default.BorderStyle != RDL.DOM.BorderStyles.None && border.Default.Thickness != 0.0)
                {
#if WINRT 
                    Syncfusion.DocIO.DLS.Color borderColor = new DocIO.DLS.Color();
                    borderColor = this.ConvertStringToColor(border.Default.BorderBrush.ToString());
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);

#else
                    Color borderColor = new Color();
                    borderColor = this.ConvertStringToColor(border.Default.BorderBrush.ToString());
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);

#endif
                    //if()  Color.FromArgb(borderColor.B, borderColor.G, borderColor.R);// this.ConvertStringToColor(borderColor.Name);

                }

                if (border.LeftBorder != null && border.LeftBorder.Thickness >= 1 && border.LeftBorder.BorderStyle != RDL.DOM.BorderStyles.None)
                {
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Left.LineWidth = (float)border.LeftBorder.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Left.LineWidth = (float)border.LeftBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Left.LineWidth = (float)border.LeftBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Left.LineWidth = (float)border.LeftBorder.Thickness;
                }
                else if (border.RightBorder != null && border.RightBorder.Thickness >= 1 && border.RightBorder.BorderStyle != RDL.DOM.BorderStyles.None)
                {
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Right.LineWidth = (float)border.RightBorder.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)border.RightBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Right.LineWidth = (float)border.RightBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)border.RightBorder.Thickness;
                }
                else if (border.BottomBorder != null && border.BottomBorder.Thickness >= 1 && border.BottomBorder.BorderStyle != RDL.DOM.BorderStyles.None)
                {
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Bottom.LineWidth = (float)border.BottomBorder.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Bottom.LineWidth = (float)border.BottomBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Bottom.LineWidth = (float)border.BottomBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Bottom.LineWidth = (float)border.BottomBorder.Thickness;
                }
                else if (border.TopBorder != null && border.TopBorder.Thickness >= 1 && border.TopBorder.BorderStyle != RDL.DOM.BorderStyles.None)
                {
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Top.LineWidth = (float)border.TopBorder.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Top.LineWidth = (float)border.TopBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Top.LineWidth = (float)border.TopBorder.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Top.LineWidth = (float)border.TopBorder.Thickness;
                }
                else if (border.Default != null && border.Default.Thickness >= 1 && border.Default.BorderStyle != RDL.DOM.BorderStyles.None)
                {
                    wordTable.Rows[row].Cells[column].CellFormat.Borders.LineWidth = (float)border.Default.Thickness;
                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)border.Default.Thickness;
                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.LineWidth = (float)border.Default.Thickness;
                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)border.Default.Thickness;
                }
            }
        }

        private void ProcessTextBox(WordReportItemCellValue cellValue, ReportModel reportModel, IWTable rectTable)
        {

            IReportItemModeler reportModel1 = cellValue.WordReportItemCellModel.WordReportItemModel;
            if (reportModel1 == null)
            {
                reportModel1 = cellValue.WordReportItemCellModel.WordCellInfo.ItemModel;
            }

            if (cellValue.WordReportItemCellModel != null && cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel1.DataSource = (List<object>)cellValue.WordReportItemCellModel.DataSource;
                    reportModel1.FieldValues = cellValue.WordReportItemCellModel.FieldValues;
                    reportModel1.RowNumbers = cellValue.WordReportItemCellModel.RowNumbers;
                    reportModel1.Evaluate();
                }
            }

            int row = cellValue.WordRowIndex - 1;
            int column = 0;
            if (cellValue.WordColumnIndex == 0)
            {
                 column = cellValue.WordColumnIndex;
            }
            else
            {
                column = cellValue.WordColumnIndex-1;
            }
            int lastRow = row + cellValue.WordRowSpan;
            int lastColumn = column + cellValue.WordColumnSpan;
            this.UpdateMergeBorder(cellValue, rectTable);
            if (cellValue.WordReportItemCellModel.WordReportItemModel != null && this.ReportModel.EnableVirtualEvaluation)
            {
                cellValue.WordReportItemCellModel.WordReportItemModel.Evaluate();
            }
            if (cellValue.WordReportItemCellModel.WordCellInfo != null && this.ReportModel.EnableVirtualEvaluation)
            {
                cellValue.WordReportItemCellModel.WordCellInfo.Evaluate();
            }

            TextboxModel textbox = reportModel1 as TextboxModel;
#if WINRT 
            Syncfusion.DocIO.DLS.Color textColor = new Syncfusion.DocIO.DLS.Color();

#else
                        Color textColor = new Color();
#endif

            if (rectTable != null)
            {
                wordTable = rectTable;
            }
            else
            {
                wordTable = table;
            }

            paragraph = wordTable.Rows[row].Cells[column].AddParagraph();

            if (textbox.ParaExpval != null)
            {
                foreach (var block in textbox.ParaExpval)
                {
                    foreach (var run in block.Runs)
                    {
                        IWTextRange range = null;
                        IWTextRange pagerange = null;
                        string textValue = textbox.Model.ExpressionEngine.GetFormattedText(GetTextRun(textbox, run), textbox.ReportItem.Style.Format, textbox.ReportItem.Style.Language);
                        if (textValue.Contains("Globals.PageNumber"))
                        {
                            section.PageSetup.RestartPageNumbering = true;
                            section.PageSetup.PageStartingNumber = 1;
                            section.PageSetup.PageNumberStyle = PageNumberStyle.Arabic;
                            int a = textValue.IndexOf("Globals.PageNumber");
                            textValue.Remove(a);
                            range = paragraph.AppendText("Page ");
                            pagerange = paragraph.AppendField(" ", FieldType.FieldPage);
                        }
                        else
                        {
                            range = paragraph.AppendText(textValue);
                        }
                        range.CharacterFormat.FontName = run.Style.Font.FontFamily;
                        range.CharacterFormat.FontSize = this.PixelToPoint(run.Style.Font.FontSize);
                        if (pagerange != null)
                        {
                            pagerange.CharacterFormat.FontName = run.Style.Font.FontFamily;
                            pagerange.CharacterFormat.FontSize = this.PixelToPoint(run.Style.Font.FontSize);
                        }
                        if (run.Style.Font.FontWeight == Syncfusion.RDL.DOM.FontWeight.Bold)
                        {
                            range.CharacterFormat.Bold = true;
                            if (pagerange != null)
                            {
                                pagerange.CharacterFormat.Bold = true;
                            }
                        }
                        if (run.Style.Font.FontStyle == Syncfusion.RDL.DOM.FontStyle.Italic)
                        {
                            range.CharacterFormat.Italic = true;
                            if (pagerange != null)
                            {
                                pagerange.CharacterFormat.Italic = true;
                            }
                        }

                        if (textbox.TextBoxProperties.Border != null)
                        {
                            if (textbox.TextBoxProperties.Border.Default != null && textbox.TextBoxProperties.Border.Default.BorderBrush != null && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.None
                                && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.Default && textbox.TextBoxProperties.Border.Default.Thickness != 0.0)
                                //&& textbox.TextBoxProperties.Border.LeftBorder == null && textbox.TextBoxProperties.Border.TopBorder == null &&
                                //textbox.TextBoxProperties.Border.RightBorder == null && textbox.TextBoxProperties.Border.BottomBorder == null)
                            {
#if WINRT 
                                Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.Default.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);

#else 
                                Color borderColor = new Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.Default.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
#endif

                                //if()  Color.FromArgb(borderColor.B, borderColor.G, borderColor.R);// this.ConvertStringToColor(borderColor.Name);

                                wordTable.Rows[row].Cells[column].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;

                                if (textbox.TextBoxProperties.Border.Default.BorderStyle == RDL.DOM.BorderStyles.Solid)
                                {
                                    wordTable.Rows[row].Cells[column].CellFormat.Borders.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.BorderType = BorderStyle.Thick;
                                }
                            }

                            if (textbox.TextBoxProperties.Border.LeftBorder != null && textbox.TextBoxProperties.Border.LeftBorder.Thickness >= 1 && textbox.TextBoxProperties.Border.LeftBorder.BorderStyle != RDL.DOM.BorderStyles.None
                                && textbox.TextBoxProperties.Border.LeftBorder.BorderStyle != RDL.DOM.BorderStyles.Default )
                            {
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.LeftBorder.Thickness;
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.LeftBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.LeftBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.LeftBorder.Thickness;

#if WINRT
                                Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.LeftBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
#else 
                                Color borderColor = new Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.LeftBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Left.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Left.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Left.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Left.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);

#endif
                                if (textbox.TextBoxProperties.Border.LeftBorder.BorderStyle == RDL.DOM.BorderStyles.Solid)
                                {
                                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Left.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Left.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Left.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Left.BorderType = BorderStyle.Thick;
                                }
                            }
                            else if (textbox.TextBoxProperties.Border.RightBorder != null && textbox.TextBoxProperties.Border.RightBorder.Thickness >= 1
                                && textbox.TextBoxProperties.Border.RightBorder.BorderStyle != RDL.DOM.BorderStyles.None && textbox.TextBoxProperties.Border.RightBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
                            {
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.RightBorder.Thickness;
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.RightBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.RightBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.RightBorder.Thickness;
#if WINRT
                                Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.RightBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
#else 
                                Color borderColor = new Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.RightBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Right.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Right.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Right.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Right.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);

#endif

                                if (textbox.TextBoxProperties.Border.RightBorder.BorderStyle == RDL.DOM.BorderStyles.Solid)
                                {
                                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Right.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Right.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Right.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Right.BorderType = BorderStyle.Thick;
                                }
                            }
                            else if (textbox.TextBoxProperties.Border.BottomBorder != null && textbox.TextBoxProperties.Border.BottomBorder.Thickness >= 1
                                && textbox.TextBoxProperties.Border.BottomBorder.BorderStyle != RDL.DOM.BorderStyles.None && textbox.TextBoxProperties.Border.BottomBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
                            {
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.BottomBorder.Thickness;
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.BottomBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.BottomBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.BottomBorder.Thickness;
#if WINRT
                                Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.BottomBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);


#else 
                                Color borderColor = new Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.BottomBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Bottom.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Bottom.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Bottom.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Bottom.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
#endif

                                if (textbox.TextBoxProperties.Border.BottomBorder.BorderStyle == RDL.DOM.BorderStyles.Solid)
                                {
                                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
                                }
                            }
                            else if (textbox.TextBoxProperties.Border.TopBorder != null && textbox.TextBoxProperties.Border.TopBorder.Thickness >= 1
                                && textbox.TextBoxProperties.Border.TopBorder.BorderStyle != RDL.DOM.BorderStyles.None && textbox.TextBoxProperties.Border.TopBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
                            {
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Top.LineWidth = (float)textbox.TextBoxProperties.Border.TopBorder.Thickness;
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Top.LineWidth = (float)textbox.TextBoxProperties.Border.TopBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Top.LineWidth = (float)textbox.TextBoxProperties.Border.TopBorder.Thickness;
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Top.LineWidth = (float)textbox.TextBoxProperties.Border.TopBorder.Thickness;
#if WINRT
                                Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.TopBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
#else 
                                Color borderColor = new Color();
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.TopBorder.BorderBrush.ToString());
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.Top.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Top.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Top.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Top.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
#endif

                                if (textbox.TextBoxProperties.Border.TopBorder.BorderStyle == RDL.DOM.BorderStyles.Solid)
                                {
                                    wordTable.Rows[row].Cells[column].CellFormat.Borders.Top.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.Top.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.Top.BorderType = BorderStyle.Thick;
                                    wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.Top.BorderType = BorderStyle.Thick;
                                }
                            }
                            else if (textbox.TextBoxProperties.Border.Default != null && textbox.TextBoxProperties.Border.Default.Thickness >= 1
                                && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.None && textbox.TextBoxProperties.Border.Default.BorderStyle != RDL.DOM.BorderStyles.Default)
                                // && textbox.TextBoxProperties.Border.LeftBorder == null && textbox.TextBoxProperties.Border.TopBorder == null &&
                                //textbox.TextBoxProperties.Border.RightBorder == null && textbox.TextBoxProperties.Border.BottomBorder == null)
                            {
                                wordTable.Rows[row].Cells[column].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                                wordTable.Rows[row].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                                wordTable.Rows[lastRow].Cells[column].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                                wordTable.Rows[lastRow].Cells[lastColumn].CellFormat.Borders.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;
                            }
                        }

                        if (textbox.TextBoxProperties.BackGroundColor != null)
                        {
                            string backGroundColor = null;
#if WINRT 
                            Syncfusion.DocIO.DLS.Color backColor = new Syncfusion.DocIO.DLS.Color();
#else
                            Color backColor = new Color();

#endif
                            if (textbox.ContainerModel != null)
                            {
                                if (textbox.ContainerModel.ModelType == ModelType.RectangleModel && textbox.TextBoxProperties.BackGroundColor.ToString().ToLower() == "#00ffffff")
                                {
                                    backGroundColor = textbox.ContainerModel.ReportItem.Style.BackgroundColor;
                                }
                                else
                                {
                                    backGroundColor = textbox.TextBoxProperties.BackGroundColor.ToString();
                                }
                            }
                            else
                            {
                                backGroundColor = textbox.TextBoxProperties.BackGroundColor.ToString();
                            }

                            backColor = this.ConvertStringToColor(backGroundColor);
                            if (backGroundColor == "Transparent")
                            {
//#if WINRT
//                                wordTable.Rows[row].Cells[column].CellFormat.BackColor = Syncfusion.DocIO.DLS.Color.FromArgb(255, 255, 255);
//#else
//                                wordTable.Rows[row].Cells[column].CellFormat.BackColor = Color.FromArgb(255, 255, 255);
//#endif
                            }
                            else
                            {
#if WINRT 
                                wordTable.Rows[row].Cells[column].CellFormat.BackColor = Syncfusion.DocIO.DLS.Color.FromArgb(backColor.R, backColor.G, backColor.B);
#else
                                wordTable.Rows[row].Cells[column].CellFormat.BackColor = Color.FromArgb(backColor.R, backColor.G, backColor.B);
#endif
                            }
                        }

                        if (textbox.TextBoxProperties.Padding != null)
                        {
                            wordTable.Rows[row].Cells[column].CellFormat.Paddings.Bottom =(float)textbox.TextBoxProperties.Padding.Bottom;
                            wordTable.Rows[row].Cells[column].CellFormat.Paddings.Left = (float)textbox.TextBoxProperties.Padding.Left;
                            wordTable.Rows[row].Cells[column].CellFormat.Paddings.Right = (float)textbox.TextBoxProperties.Padding.Right;
                            wordTable.Rows[row].Cells[column].CellFormat.Paddings.Top = (float)textbox.TextBoxProperties.Padding.Top;
                        }

                        switch (block.TextAlignment)
                        {
                            case "Left":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Left;
                                break;
                            case "Right":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
                                break;
                            case "Center":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;
                                break;
                            case "Justify":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Justify;
                                break;
                        }
                        switch (textbox.TextBoxProperties.VerticalAlignment)
                        {
                            case Syncfusion.RDL.DOM.VerticalAlign.Top:
                                wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Top;
                                break;
                            case Syncfusion.RDL.DOM.VerticalAlign.Bottom:
                                wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Bottom;
                                break;
                            case Syncfusion.RDL.DOM.VerticalAlign.Middle:
                                wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                                break;
                            default:
                                wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Top;
                                break;
                        }
                        if (run.Style.TextColor != null)
                        {
                            textColor = this.ConvertStringToColor(run.Style.TextColor.ToString());
                        }
#if WINRT 
                        range.CharacterFormat.TextColor = Syncfusion.DocIO.DLS.Color.FromArgb(textColor.R, textColor.G, textColor.B);
                        if (pagerange != null)
                        {
                            pagerange.CharacterFormat.TextColor = Syncfusion.DocIO.DLS.Color.FromArgb(textColor.R, textColor.G, textColor.B);
                        }
#else
                        range.CharacterFormat.TextColor = Color.FromArgb(textColor.R, textColor.G, textColor.B);
                        if (pagerange != null)
                        {
                            pagerange.CharacterFormat.TextColor = Color.FromArgb(textColor.R, textColor.G, textColor.B);
                        }
#endif
                    }
                }
            }
            if (reportModel1 != null && cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel1.DisposeEvalObjects();
                }
                else
                {
                    if (cellValue.WordReportItemCellModel.WordCellInfo != null)
                    {
                        cellValue.WordReportItemCellModel.WordCellInfo.DisposeEvalObjects();   
                    }
                }
            }
        }

        List<WordReportItemCellModel> m_tableCellModels = new List<WordReportItemCellModel>();
        private void ProcessTablixModel(WordReportItemCellValue cellValue, ReportModel reportModelCollection,IWTable rectTable,List<WordReportItemCellValue> cellcollection)
        {
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            int lastRow = row + cellValue.WordRowSpan;
            int lastColumn = column + cellValue.WordColumnSpan;
            this.UpdateMergeBorder(cellValue, rectTable);

            if (rectTable != null)
            {
                wordTable = rectTable;
            }
            else
            {
                wordTable = table;
            }

            IReportItemModeler reportModel = cellValue.WordReportItemCellModel.WordReportItemModel;
            if (reportModel == null)
            {
                reportModel = cellValue.WordReportItemCellModel.WordCellInfo.ItemModel;
            }

            TablixModel tablix = reportModel as TablixModel;

            IWTable tablixTable = wordTable[row, column].AddTable();
            WTableRow row5 = wordTable.Rows[row];
            row5.Height = 0;
            RowFormat format = new RowFormat();
            format.Paddings.All = 0;
            format.IsAutoResized = true;
            tablixTable.ResetCells(tablix.RowCount, tablix.ColumnCount,format,0);

            tablixTable.TableFormat.Paddings.Left = 1;
            tablixTable.TableFormat.Paddings.Right = 0;

            if (tablix.ReportItem.Style.Border != null)
            {
                if (tablix.ReportItem.Style.Border.Style == "None")
                {
                    tablixTable.TableFormat.Borders.BorderType = BorderStyle.None;
                }
                else
                {
                    tablixTable.TableFormat.Borders.BorderType = BorderStyle.Hairline;
                }
            }

            if (this.tableReportItemCellModels != null)
            {
                this.tableReportItemCellModels.Clear();
            }

            //if (this.tableCellModels.Count != 0 )
            //{
            //    this.tableCellModels.Clear();
            //}

            this.tableReportItemCellModels = GetTablixReportItemCellModels(tablix);
            this.UpdateTableRowColumnValues(this.tableReportItemCellModels, cellcollection, UpdateSection.Body,tablix);
            int index = cellcollection.Count - this.tableReportItemCellModels.Count;
            var temp = cellcollection.GetRange(index, this.tableReportItemCellModels.Count);
            cellcollection.RemoveRange(index, this.tableReportItemCellModels.Count);
            for (int i = 0; i < tablix.RowCount; i++)
            {
                WTableRow row1 = tablixTable.Rows[i];
                row1.Height = this.PixelToPoint(tablix.RowHeights[i]);

                for (int j = 0; j < tablix.ColumnCount; j++)
                {
                    row1.Cells[j].Width = this.PixelToPoint(tablix.ColumnWights[j]);
                }
            }

            foreach (var cellModel in temp)
            {
                if (cellModel.WordReportItemCellModel.WordCellInfo != null && cellModel.WordReportItemCellModel.WordCellInfo.ItemModel.ModelType == ModelType.TablixModel)
                {
                    ProcessTablixModel(cellModel, reportModelCollection, null,new List<WordReportItemCellValue>());
                    break;
                }
                else
                {
                    this.ProcessTable(cellModel, reportModelCollection, tablixTable);
                }
            }
        }

        private void ProcessTable(WordReportItemCellValue cellValue, ReportModel reportModel, IWTable tablixTable)
        {
            
            if (tablixTable != null)
            {
                wordTable = tablixTable;
            }
            else
            {
                wordTable = table;
            }
            m_currentListCellInfo = cellValue.WordReportItemCellModel.WordCellInfo;

            if (cellValue.WordReportItemCellModel != null && cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    cellValue.WordReportItemCellModel.WordCellInfo.DataSource = (List<object>)cellValue.WordReportItemCellModel.DataSource;
                    cellValue.WordReportItemCellModel.WordCellInfo.FieldValues = cellValue.WordReportItemCellModel.FieldValues;
                    reportModel.Evaluate();
                }        
            }
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            int lastRow = row + cellValue.WordRowSpan;
            int lastColumn = column + cellValue.WordColumnSpan;
            this.UpdateMergeBorder(cellValue, tablixTable);
            string backgroundcolor=string.Empty;

            if (m_currentListCellInfo.ItemModel.ModelType == ModelType.TextBoxModel)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    m_currentListCellInfo.Evaluate();
                }
                var textbox = (TextboxModel)m_currentListCellInfo.ItemModel;
                if (textbox.TextBoxProperties != null)
                {
                    backgroundcolor = textbox.TextBoxProperties.BackGroundColor;

                    if (backgroundcolor != null)
                    {
#if WINRT 
                        Syncfusion.DocIO.DLS.Color cellColor = new Syncfusion.DocIO.DLS.Color();
                        cellColor = this.ConvertStringToColor(backgroundcolor.ToString());
                        wordTable.Rows[row].Cells[column].CellFormat.BackColor = Syncfusion.DocIO.DLS.Color.FromArgb(cellColor.R, cellColor.G, cellColor.B);

#else
                          Color cellColor = new Color();
                        cellColor = this.ConvertStringToColor(backgroundcolor.ToString());
                        wordTable.Rows[row].Cells[column].CellFormat.BackColor = Color.FromArgb(cellColor.R, cellColor.G, cellColor.B);
#endif
                    }

                    if (textbox.TextBoxProperties.Border != null)
                    {
                        if (textbox.TextBoxProperties.Border.Default != null && textbox.TextBoxProperties.Border.Default.BorderBrush != null
                            && textbox.TextBoxProperties.Border.BottomBorder == null && textbox.TextBoxProperties.Border.LeftBorder == null
                             && textbox.TextBoxProperties.Border.TopBorder == null && textbox.TextBoxProperties.Border.RightBorder == null)
                        {
#if WINRT 
                            Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                            if (textbox.TextBoxProperties.Border.Default.BorderBrush == "LightGrey")
                            {
                                borderColor = this.ConvertStringToColor("LightGray");
                            }
                            else
                            {
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.Default.BorderBrush.ToString());
                            }
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }
#else 
                            Color borderColor = new Color();
                            if (textbox.TextBoxProperties.Border.Default.BorderBrush == "LightGrey")
                            {
                                borderColor = this.ConvertStringToColor("LightGray");
                            }
                            else
                            {
                                borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.Default.BorderBrush.ToString());
                            }
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }
#endif


                        }
                        else if (textbox.TextBoxProperties.Border.TopBorder != null && textbox.TextBoxProperties.Border.TopBorder.BorderBrush != null)
                        {
#if WINRT
                            Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();

                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.TopBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }



#else 
                            Color borderColor = new Color();
                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.TopBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }


#endif
                        }

                        else if (textbox.TextBoxProperties.Border.BottomBorder != null && textbox.TextBoxProperties.Border.BottomBorder.BorderBrush != null)
                        {
#if WINRT
                            Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.BottomBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.Transparent;

                                    if (textbox.TextBoxProperties.Border.BottomBorder.Thickness > 0.0)
                                    {
                                        wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.BottomBorder.Thickness;
                                    }
                                }
                            }

#else 
                            Color borderColor = new Color();
                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.BottomBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Color.Transparent;
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Color.Transparent;
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Color.Transparent;

                                    if (textbox.TextBoxProperties.Border.BottomBorder.Thickness > 0.0)
                                    {
                                        wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.BottomBorder.Thickness;
                                    }
                                }
                            }

#endif

                        }

                        else if (textbox.TextBoxProperties.Border.LeftBorder != null && textbox.TextBoxProperties.Border.LeftBorder.BorderBrush != null)
                        {
#if WINRT
                            Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.LeftBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }

#else 
                           Color borderColor = new Color();
                           borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.LeftBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }

#endif

                        }

                        else if (textbox.TextBoxProperties.Border.RightBorder != null && textbox.TextBoxProperties.Border.RightBorder.BorderBrush != null)
                        {
#if WINRT
                            Syncfusion.DocIO.DLS.Color borderColor = new Syncfusion.DocIO.DLS.Color();
                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.RightBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }
  
#else 
                            Color borderColor = new Color();
                            borderColor = this.ConvertStringToColor(textbox.TextBoxProperties.Border.RightBorder.BorderBrush.ToString());
                            for (int i = row; i <= lastRow; i++)
                            {
                                for (int j = column; j <= lastColumn; j++)
                                {
                                    wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Color.FromArgb(borderColor.R, borderColor.G, borderColor.B);
                                }
                            }
  
#endif
                        }
                    }

                    else
                    {
                        for (int i = row; i <= lastRow; i++)
                        {
                            for (int j = column; j <= lastColumn; j++)
                            {
#if WINRT
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.Transparent;
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.Transparent;
#else
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Top.Color = Color.Transparent;
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Right.Color = Color.Transparent;
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Left.Color = Color.Transparent;
                                wordTable.Rows[i].Cells[j].CellFormat.Borders.Bottom.Color = Color.Transparent;

#endif

                            }
                        }
                    }
                }

            }
            if (m_currentListCellInfo.ItemModel.ModelType == ModelType.ImageModel || m_currentListCellInfo.ItemModel.ModelType == ModelType.ChartModel || m_currentListCellInfo.ItemModel.ModelType == ModelType.GaugeModel)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    m_currentListCellInfo.Evaluate();
                }
                var imageModel = (ImageModel)m_currentListCellInfo.ItemModel;
                object imgData = imageModel.ImageData;
                MemoryStream imageStream = new MemoryStream(imgData as byte[]);
                WTableRow imageRow = wordTable.Rows[row];
                paragraph = (IWParagraph)imageRow.Cells[column].AddParagraph();
#if !SILVERLIGHT
                IWPicture picture1 = paragraph.AppendPicture(new Bitmap(imageStream));
#else
                IWPicture picture1 = paragraph.AppendPicture(imageStream);
#endif

                picture1.Height = this.PixelToPoint(cellValue.WordReportItemCellModel.WordHeight);
                picture1.Width = this.PixelToPoint(cellValue.WordReportItemCellModel.WordWidth);
                updateTableBorder(row, column, lastRow, lastColumn);
            }
            else if (m_currentListCellInfo.ItemModel.ModelType == ModelType.RectangleModel)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    m_currentListCellInfo.Evaluate();
                }
                isListProcess = true;
                ProcessRectangleModel(cellValue, reportModel,tablixTable);
            }
            else if (m_currentListCellInfo.ItemModel.ModelType == ModelType.TablixModel)
            {
                ProcessTablixModel(cellValue, reportModel, tablixTable,new List<WordReportItemCellValue>());
            }
            else
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    m_currentListCellInfo.Evaluate();
                }
                var textbox = (TextboxModel)m_currentListCellInfo.ItemModel;
                foreach (var textblock in textbox.ParaExpval)
                {
                    foreach (var runs in textblock.Runs)
                    {
#if WINRT
                        Syncfusion.DocIO.DLS.Color textColor = new Syncfusion.DocIO.DLS.Color();

#else 
                                Color textColor = new Color();
#endif

                        paragraph = wordTable.Rows[row].Cells[column].AddParagraph();
                        string textValue = m_currentListCellInfo.ItemModel.Model.ExpressionEngine.GetFormattedText(runs.Text, runs.Style.Format, runs.Style.Language);
                        IWTextRange theadertext = paragraph.AppendText(textValue);
                        theadertext.CharacterFormat.FontName = runs.Style.Font.FontFamily;
                        theadertext.CharacterFormat.FontSize = this.PixelToPoint((runs.Style.Font.FontSize));

                        if (runs.Style.TextColor != null)
                        {
                            textColor = this.ConvertStringToColor(runs.Style.TextColor.ToString());
#if WINRT 
                            theadertext.CharacterFormat.TextColor = Syncfusion.DocIO.DLS.Color.FromArgb(textColor.R, textColor.G, textColor.B);                            
#else
                            theadertext.CharacterFormat.TextColor = Color.FromArgb(textColor.R, textColor.G, textColor.B);                            
#endif
                        }

                        if (runs.Style.Font.FontStyle == Syncfusion.RDL.DOM.FontStyle.Italic)
                        {
                            theadertext.CharacterFormat.Italic = true;
                        }
                        if (runs.Style.Font.FontWeight == Syncfusion.RDL.DOM.FontWeight.Bold)
                        {
                            theadertext.CharacterFormat.Bold = true;
                        }

                        switch (textblock.TextAlignment)
                        {
                            case "Right":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
                                break;
                            case "Left":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Left;
                                break;
                            case "Center":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;
                                break;
                            case "Stretch":
                                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Justify;
                                break;
                        }

                    }
                }
                switch (textbox.TextBoxProperties.VerticalAlignment)
                {
                    case Syncfusion.RDL.DOM.VerticalAlign.Bottom:
                        wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Bottom;
                        break;
                    case Syncfusion.RDL.DOM.VerticalAlign.Middle:
                        wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                        break;
                    case Syncfusion.RDL.DOM.VerticalAlign.Top:
                        wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Top;
                        break;
                    default:
                        wordTable.Rows[row].Cells[column].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Top;
                        break;
                }
            }
            updateTableBorder(row, column, lastRow, lastColumn);
        }

        private void updateTableBorder(int row,int column,int lastRow,int lastColumn)
        {
            if (m_currentListCellInfo.ItemModel.ModelType == ModelType.TextBoxModel)
            {
                var textbox = (TextboxModel)m_currentListCellInfo.ItemModel;

                if (textbox.TextBoxProperties.Border.LeftBorder != null && textbox.TextBoxProperties.Border.RightBorder != null && textbox.TextBoxProperties.Border.TopBorder != null && textbox.TextBoxProperties.Border.BottomBorder != null)
                {
                    if (textbox.TextBoxProperties.Border.LeftBorder.Thickness > 0 && textbox.TextBoxProperties.Border.RightBorder.Thickness > 0
                           && textbox.TextBoxProperties.Border.TopBorder.Thickness > 0 && textbox.TextBoxProperties.Border.BottomBorder.Thickness > 0)
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
                else if (textbox.TextBoxProperties.Border.Default != null)
                {
                    if (textbox.TextBoxProperties.Border.Default.Thickness > 0)
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness; //Syncfusion.DocIO.DLS.BorderStyle.Thick;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness; ;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                        row1.Cells[column].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness; //Syncfusion.DocIO.DLS.BorderStyle.Thick;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness; //Syncfusion.DocIO.DLS.BorderStyle.Thick;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                        row2.Cells[column].CellFormat.Borders.Left.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.LineWidth = (float)textbox.TextBoxProperties.Border.Default.Thickness;// Syncfusion.DocIO.DLS.BorderStyle.Thick;
                    }
                    else
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
                else
                {
                    WTableRow row1 = wordTable.Rows[row];
                    row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;

                    if (row != lastRow)
                    {
                        for (int i = row + 1; i < lastRow; i++)
                        {
                            WTableRow midRow = wordTable.Rows[i];
                            midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        }
                    }

                    WTableRow row2 = wordTable.Rows[lastRow];
                    row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
#if WINRT 
                    Syncfusion.DocIO.DLS.Color cellColor = new Syncfusion.DocIO.DLS.Color();
#else
                    Color cellColor = new Color();
#endif
                    cellColor = this.ConvertStringToColor("LightGray");
                    WTableRow row3 = wordTable.Rows[row];
                    row3.Cells[column].CellFormat.Borders.Top.Color = cellColor;
                    row3.Cells[lastColumn].CellFormat.Borders.Top.Color = cellColor;
                    row3.Cells[column].CellFormat.Borders.Left.Color = cellColor;
                    row3.Cells[lastColumn].CellFormat.Borders.Right.Color = cellColor;

                    if (row != lastRow)
                    {
                        for (int i = row + 1; i < lastRow; i++)
                        {
                            WTableRow midRow = wordTable.Rows[i];
                            midRow.Cells[column].CellFormat.Borders.Left.Color = cellColor;
                            midRow.Cells[lastColumn].CellFormat.Borders.Right.Color = cellColor;
                        }
                    }

                    WTableRow row4 = wordTable.Rows[lastRow];
                    row4.Cells[column].CellFormat.Borders.Bottom.Color = cellColor;
                    row4.Cells[lastColumn].CellFormat.Borders.Bottom.Color = cellColor;
                    row4.Cells[column].CellFormat.Borders.Left.Color = cellColor;
                    row4.Cells[lastColumn].CellFormat.Borders.Right.Color = cellColor;
                }
            }
            else if (m_currentListCellInfo.ItemModel.ModelType == ModelType.ImageModel)
            {
                var image = (ImageModel)m_currentListCellInfo.ItemModel;
                if (image.ImageProperties.Border.LeftBorder != null && image.ImageProperties.Border.RightBorder != null && image.ImageProperties.Border.TopBorder != null && image.ImageProperties.Border.BottomBorder != null)
                {
                    if (image.ImageProperties.Border.LeftBorder.Thickness > 0 && image.ImageProperties.Border.RightBorder.Thickness > 0
                           && image.ImageProperties.Border.TopBorder.Thickness > 0 && image.ImageProperties.Border.BottomBorder.Thickness > 0)
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                    }
                    else
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    }
                }
                else if (image.ImageProperties.Border.Default != null)
                {
                    if (image.ImageProperties.Border.Default.BorderStyle != Syncfusion.RDL.DOM.BorderStyles.None)
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (image.ImageProperties.Border.Default.BorderBrush != null)
                        {
#if WINRT
                            Syncfusion.DocIO.DLS.Color cellColor = new Syncfusion.DocIO.DLS.Color();
#else
                           Color cellColor = new Color();
#endif
                            cellColor = this.ConvertStringToColor(image.ImageProperties.Border.Default.BorderBrush);
                            WTableRow row3 = wordTable.Rows[row];
                            row3.Cells[column].CellFormat.Borders.Top.Color = cellColor;
                            row3.Cells[lastColumn].CellFormat.Borders.Top.Color = cellColor;
                            row3.Cells[column].CellFormat.Borders.Left.Color = cellColor;
                            row3.Cells[lastColumn].CellFormat.Borders.Right.Color = cellColor;

                            if (row != lastRow)
                            {
                                for (int i = row + 1; i < lastRow; i++)
                                {
                                    WTableRow midRow = wordTable.Rows[i];
                                    midRow.Cells[column].CellFormat.Borders.Left.Color = cellColor;
                                    midRow.Cells[lastColumn].CellFormat.Borders.Right.Color = cellColor;
                                }
                            }

                            WTableRow row4 = wordTable.Rows[lastRow];
                            row4.Cells[column].CellFormat.Borders.Bottom.Color = cellColor;
                            row4.Cells[lastColumn].CellFormat.Borders.Bottom.Color = cellColor;
                            row4.Cells[column].CellFormat.Borders.Left.Color = cellColor;
                            row4.Cells[lastColumn].CellFormat.Borders.Right.Color = cellColor;
                        }
                    }
                    else
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    }
                }
            }
            else if (m_currentListCellInfo.ItemModel.ModelType == ModelType.RectangleModel)
            {
                var rectangle = (RectangleModel)m_currentListCellInfo.ItemModel;
                if (rectangle.RectItemExpPro.Border!=null && rectangle.RectItemExpPro.Border.LeftBorder != null && rectangle.RectItemExpPro.Border.RightBorder != null && rectangle.RectItemExpPro.Border.TopBorder != null && rectangle.RectItemExpPro.Border.BottomBorder != null)
                {
                    if (rectangle.RectItemExpPro.Border.LeftBorder.Thickness > 0 && rectangle.RectItemExpPro.Border.RightBorder.Thickness > 0
                           && rectangle.RectItemExpPro.Border.TopBorder.Thickness > 0 && rectangle.RectItemExpPro.Border.BottomBorder.Thickness > 0)
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
                else if (rectangle.RectItemExpPro.Border!=null && rectangle.RectItemExpPro.Border.Default != null)
                {
                    if (rectangle.RectItemExpPro.Border.Default.Thickness > 0 && rectangle.RectItemExpPro.Border.Default.BorderStyle!=RDL.DOM.BorderStyles.None)
                    {
                    WTableRow row1 = wordTable.Rows[row];
                    row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                    if (row != lastRow)
                    {
                        for (int i = row + 1; i < lastRow; i++)
                        {
                            WTableRow midRow = wordTable.Rows[i];
                            midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        }
                    }

                    WTableRow row2 = wordTable.Rows[lastRow];
                    row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
                else
                {
                    WTableRow row1 = wordTable.Rows[row];
                    row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;

                    if (row != lastRow)
                    {
                        for (int i = row + 1; i < lastRow; i++)
                        {
                            WTableRow midRow = wordTable.Rows[i];
                            midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                        }
                    }

                    WTableRow row2 = wordTable.Rows[lastRow];
                    row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                    row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                }
            }
        }

        private List<WordReportItemCellModel> GetTablixReportItemCellModels(TablixModel tablixModel)
        {
            TablixModel tablix = tablixModel as TablixModel;
            double tabLeft = 0;
            double tabTop = 0;

            if (tablixModel.FlowLayoutInfo != null)
            {
                tabLeft = tablixModel.FlowLayoutInfo.ActualLeft;
                tabTop = tablixModel.FlowLayoutInfo.ActualTop;
            }

            for (int i = 0; i < tablix.RowCount; i++)
            {
                for (int j = 0; j < tablix.ColumnCount; j++)
                {
                    TablixCellInfo cellInfo = tablix.Data[i][j];

                    if (cellInfo != null)
                    {
                        cellInfo.Evaluate();
                        WordReportItemCellModel rptModel = new WordReportItemCellModel();
                        rptModel.WordCellInfo = cellInfo;
                        rptModel.WordCellInfo.DataSource =(List<object>)rptModel.DataSource;
                        rptModel.WordCellInfo.FieldValues = rptModel.FieldValues;
                        rptModel.WordReportItemModel = tablix.Data[i][j].ItemModel;
                        rptModel.IsTablixCell = true;

                        double celltop = tabTop;
                        for (int row = 0; row < i; row++)
                        {
                            celltop += tablix.RowHeights[row];
                        }

                        double cellLeft = tabLeft;
                        for (int column = 0; column < j; column++)
                        {
                            cellLeft += tablix.ColumnWights[column];
                        }

                        var coverCells = from coveredCell in tablix.CoveredRanges
                                         where coveredCell.Left == j && coveredCell.Top == i
                                         select coveredCell;

                        if (coverCells.Count() > 0)
                        {
                            int leftCell = coverCells.First().Left;
                            int topCell = coverCells.First().Top;
                            double width = 0;
                            double height = 0;

                            for (int row = topCell; row <= coverCells.First().Bottom; row++)
                            {
                                height += tablix.RowHeights[row];
                            }

                            for (int column = leftCell; column <= coverCells.First().Right; column++)
                            {
                                width += tablix.ColumnWights[column];
                            }

                            rptModel.WordHeight = height;
                            rptModel.WordWidth = width;
                        }
                        else
                        {
                            rptModel.WordHeight = tablix.RowHeights[i];
                            rptModel.WordWidth = tablix.ColumnWights[j];
                        }

                        rptModel.WordLeft = cellLeft;
                        rptModel.WordTop = celltop;

                        m_tableCellModels.Add(rptModel);

                    }
                }
            }
            return m_tableCellModels;
        }

        private void UpdateTableRowColumnValues(List<WordReportItemCellModel> reportItemCellModels, List<WordReportItemCellValue> cellModels, UpdateSection updateSection,TablixModel tablix)
        {
            int count = 0;
            WordReportItemCellModel model;
            TablixModel tablixmodel = tablix as TablixModel;

            if (reportItemCellModels.Count > 0)
            {
                model = reportItemCellModels[count];

                for (int row = 0; row < tablixmodel.RowCount; row++)
                {
                    for (int column = 0; column < tablixmodel.ColumnCount; column++)
                    {
                        if (tablixmodel.Data[row][column] != null)
                        {
                            WordReportItemCellValue indexer = new WordReportItemCellValue();
                            indexer.WordReportItemCellModel = reportItemCellModels[count];
                            indexer.WordRowIndex = row + 1;
                            indexer.WordColumnIndex = column + 1;
                            cellModels.Add(indexer);
                            if (reportItemCellModels[count].WordCellInfo.CellRange != null)
                            {
                                if (reportItemCellModels[count].WordCellInfo.CellRange.Right > 0)
                                {
                                    indexer.WordColumnSpan = reportItemCellModels[count].WordCellInfo.CellRange.Right - reportItemCellModels[count].WordCellInfo.CellRange.Left;
                                }
                                if (reportItemCellModels[count].WordCellInfo.CellRange.Bottom > 0)
                                {
                                    indexer.WordRowSpan = reportItemCellModels[count].WordCellInfo.CellRange.Bottom - reportItemCellModels[count].WordCellInfo.CellRange.Top;
                                }
                            }
                            count++;
                        }
                    }
                }
            }
        }
        List<IReportItemModeler> tempReportItems = new List<IReportItemModeler>();
        private List<WordReportItemCellModel> GetListReportItemCellModels(IEnumerable<IReportItemModeler> models,bool isList)
        {
            List<WordReportItemCellModel> m_listCellModels = new List<WordReportItemCellModel>();

            foreach (var item in models)
            {
                if (item.ContainerModel != null)
                {
                    foreach (IReportItemModeler report in item.ContainerModel.ReportItemModelers)
                    {
                        tempReportItems.Add(report);
                    }
                }
            }
            foreach (var reportItem in models)
            {
                if (tempReportItems != null)
                {
                    if (!tempReportItems.Contains(reportItem))
                    {
                        WordReportItemCellModel listrptModel = new WordReportItemCellModel();
                        if (isList)
                        {
                            listrptModel.WordHeight = reportItem.FlowLayoutInfo.ActualHeight;
                            listrptModel.WordWidth = reportItem.FlowLayoutInfo.ActualWidth;
                            listrptModel.WordLeft = reportItem.FlowLayoutInfo.ActualLeft;
                            listrptModel.WordTop = reportItem.FlowLayoutInfo.ActualTop;
                            listrptModel.WordReportItemModel = reportItem;
                            m_listCellModels.Add(listrptModel);
                        }
                        else
                        {
                            if (reportItem.ModelType == ModelType.TablixModel)
                            {
                                listrptModel.WordHeight = reportItem.FlowLayoutInfo.ActualHeight;
                            }
                            else
                            {
                                listrptModel.WordHeight = reportItem.Height;
                            }
                            listrptModel.WordWidth = reportItem.Width;
                            listrptModel.WordLeft = reportItem.Left;
                            listrptModel.WordTop = reportItem.Top;
                            listrptModel.WordReportItemModel = reportItem;
                            m_listCellModels.Add(listrptModel);
                        }
                    }
                }
            }
            if (tempReportItems != null)
            {
                tempReportItems.Clear();
            }
            return m_listCellModels;
        }

        private void ProcessRectangleModel(WordReportItemCellValue cellValue, ReportModel reportModelCollection, IWTable nestTable)
        {
            SubReportModel subReportModel = null;
            RectangleModel rectangeModel = null;
            List<WordReportItemCellValue> rectCellModels = new List<WordReportItemCellValue>();
            List<WordReportItemCellModel> rectReportItemCellModels=new List<WordReportItemCellModel>();
            
            IReportItemModeler reportModel = cellValue.WordReportItemCellModel.WordReportItemModel;
            if (reportModel == null)
            {
                cellValue.WordReportItemCellModel.WordCellInfo.Evaluate();
                reportModel = cellValue.WordReportItemCellModel.WordCellInfo.ItemModel;
            }

            if (cellValue.WordReportItemCellModel != null)
            {
                if (cellValue.WordReportItemCellModel.IsTablixCell)
                {
                    if (this.ReportModel.EnableVirtualEvaluation)
                    {
                        reportModel.DataSource = cellValue.WordReportItemCellModel.DataSource;
                        reportModel.FieldValues = cellValue.WordReportItemCellModel.FieldValues;
                        reportModel.RowNumbers = cellValue.WordReportItemCellModel.RowNumbers;
                        reportModel.Evaluate();
                    }
                }
            }

            if (reportModel.ModelType == ModelType.SubReportModel)
            {
                subReportModel = reportModel as SubReportModel;
            }
            else
            {
                rectangeModel = reportModel as RectangleModel;
            }

            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            int lastRow = row + cellValue.WordRowSpan;
            int lastColumn = column + cellValue.WordColumnSpan;

            if (cellValue.WordReportItemCellModel.WordReportItemModel.IsTablixChild)
            {
                this.UpdateMergeBorder(cellValue, nestTable);
            }

            if (cellValue.WordReportItemCellModel.WordReportItemModel != null && cellValue.WordReportItemCellModel.WordReportItemModel.FlowLayoutInfo != null)
            {
                reportModel.Top = cellValue.WordReportItemCellModel.WordReportItemModel.FlowLayoutInfo.ActualTop;
                reportModel.Left = cellValue.WordReportItemCellModel.WordReportItemModel.FlowLayoutInfo.ActualLeft;
                reportModel.Height = cellValue.WordReportItemCellModel.WordReportItemModel.FlowLayoutInfo.ActualHeight;
                reportModel.Width = cellValue.WordReportItemCellModel.WordReportItemModel.FlowLayoutInfo.ActualWidth;
            }
            else
            {
                reportModel.Top = cellValue.WordReportItemCellModel.WordTop;
                reportModel.Left = cellValue.WordReportItemCellModel.WordLeft;
                reportModel.Height = cellValue.WordReportItemCellModel.WordHeight;
                reportModel.Width = cellValue.WordReportItemCellModel.WordWidth;
            }

            if (nestTable != null)
            {
                wordTable = nestTable;
            }
            else
            {
                wordTable = table;
            }
            if (rectangeModel == null)
            {
#if WINRT
                Syncfusion.DocIO.DLS.Color backColor = new Syncfusion.DocIO.DLS.Color();
#else
                    Color backColor = new Color();
#endif
                backColor = this.ConvertStringToColor(subReportModel.ReportItem.Style.BackgroundColor);

                for (int rowVal = row; rowVal <= lastRow; rowVal++)
                {
                    for (int colVal = column; colVal <= lastColumn; colVal++)
                    {
#if WINRT 
                        wordTable.Rows[rowVal].Cells[colVal].CellFormat.BackColor = Syncfusion.DocIO.DLS.Color.FromArgb(backColor.R, backColor.G, backColor.B);
#else
                        wordTable.Rows[rowVal].Cells[colVal].CellFormat.BackColor = Color.FromArgb(backColor.R, backColor.G, backColor.B);
#endif
                    }
                }
                if (subReportModel.ReportItem.Style.TopBorder != null && subReportModel.ReportItem.Style.BottomBorder != null && subReportModel.ReportItem.Style.RightBorder != null && subReportModel.ReportItem.Style.LeftBorder != null)
                {
                    if (subReportModel.ReportItem.Style.TopBorder.Width.PixelValue > 0 && subReportModel.ReportItem.Style.BottomBorder.Width.PixelValue > 0 && subReportModel.ReportItem.Style.RightBorder.Width.PixelValue > 0 && subReportModel.ReportItem.Style.LeftBorder.Width.PixelValue > 0)
                    {
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            }
                        }

                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                        row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    }
                }
            }
            else
            {
#if WINRT 
                Syncfusion.DocIO.DLS.Color backColor = new Syncfusion.DocIO.DLS.Color();
#else 
                Color backColor = new Color();
#endif
                backColor = this.ConvertStringToColor(rectangeModel.ReportItem.Style.BackgroundColor);

                for (int rowVal = row; rowVal <= lastRow; rowVal++)
                {
                    for (int colVal = column; colVal <= lastColumn; colVal++)
                    {
#if WINRT 
                        wordTable.Rows[rowVal].Cells[colVal].CellFormat.BackColor = Syncfusion.DocIO.DLS.Color.FromArgb(backColor.R, backColor.G, backColor.B);
#else
                        wordTable.Rows[rowVal].Cells[colVal].CellFormat.BackColor = Color.FromArgb(backColor.R, backColor.G, backColor.B);

#endif
                    }
                }

                if (rectangeModel.RectItemExpPro.Border != null && rectangeModel.RectItemExpPro.Border.Default != null)
                {
                    switch (rectangeModel.RectItemExpPro.Border.Default.BorderStyle)
                    {
                        case Syncfusion.RDL.DOM.BorderStyles.Default:
                        case Syncfusion.RDL.DOM.BorderStyles.None:
                            WTableRow row1 = wordTable.Rows[row];
                            row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;

                            if (row != lastRow)
                            {
                                for (int i = row + 1; i < lastRow; i++)
                                {
                                    WTableRow midRow = wordTable.Rows[i];
                                    midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                                    midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                                }
                            }
                            if (column != lastColumn)
                            {
                                for (int i = column + 1; i < lastColumn; i++)
                                {
                                    wordTable.Rows[row].Cells[i].CellFormat.Borders.Top.BorderType = BorderStyle.None;
                                    wordTable.Rows[lastRow].Cells[i].CellFormat.Borders.Bottom.BorderType = BorderStyle.None;
                                }
                            }

                            WTableRow row2 = wordTable.Rows[lastRow];
                            row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.None;
                            break;

                        default:
                            WTableRow row3 = wordTable.Rows[row];
                            row3.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            row3.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            row3.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            row3.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;

                            if (row != lastRow)
                            {
                                for (int i = row + 1; i < lastRow; i++)
                                {
                                    WTableRow midRow = wordTable.Rows[i];
                                    midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                    midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                                }
                            }

                            if (column != lastColumn)
                            {
                                for (int i = column + 1; i < lastColumn; i++)
                                {
                                    wordTable.Rows[row].Cells[i].CellFormat.Borders.Top.BorderType = BorderStyle.Hairline;
                                    wordTable.Rows[lastRow].Cells[i].CellFormat.Borders.Bottom.BorderType = BorderStyle.Hairline;                              
                                }
                            }

                            WTableRow row4 = wordTable.Rows[lastRow];
                            row4.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            row4.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            row4.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            row4.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                            break;
                    }

                    if (rectangeModel.RectItemExpPro.Border.Default.BorderBrush != null)
                    {
#if WINRT 
                        Syncfusion.DocIO.DLS.Color bordercolor = new Syncfusion.DocIO.DLS.Color();
                        bordercolor = this.ConvertStringToColor(rectangeModel.RectItemExpPro.Border.Default.BorderBrush);
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row1.Cells[lastColumn].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row1.Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row1.Cells[lastColumn].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                            }
                        }
                        if (column != lastColumn)
                        {
                            for (int i = column + 1; i < lastColumn; i++)
                            {
                                wordTable.Rows[row].Cells[i].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                                wordTable.Rows[lastRow].Cells[i].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                            }
                        }
                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row2.Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row2.Cells[lastColumn].CellFormat.Borders.Right.Color = Syncfusion.DocIO.DLS.Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                    }
#else
                         Color bordercolor = new Color();
                        bordercolor = this.ConvertStringToColor(rectangeModel.RectItemExpPro.Border.Default.BorderBrush);
                        WTableRow row1 = wordTable.Rows[row];
                        row1.Cells[column].CellFormat.Borders.Top.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row1.Cells[lastColumn].CellFormat.Borders.Top.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row1.Cells[column].CellFormat.Borders.Left.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row1.Cells[lastColumn].CellFormat.Borders.Right.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);

                        if (row != lastRow)
                        {
                            for (int i = row + 1; i < lastRow; i++)
                            {
                                WTableRow midRow = wordTable.Rows[i];
                                midRow.Cells[column].CellFormat.Borders.Left.Color =  Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                                midRow.Cells[lastColumn].CellFormat.Borders.Right.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                            }
                        }
                        if (column != lastColumn)
                        {
                            for (int i = column + 1; i < lastColumn; i++)
                            {
                                wordTable.Rows[row].Cells[i].CellFormat.Borders.Top.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                                wordTable.Rows[lastRow].Cells[i].CellFormat.Borders.Bottom.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                            }
                        }
                        WTableRow row2 = wordTable.Rows[lastRow];
                        row2.Cells[column].CellFormat.Borders.Bottom.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row2.Cells[lastColumn].CellFormat.Borders.Bottom.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row2.Cells[column].CellFormat.Borders.Left.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                        row2.Cells[lastColumn].CellFormat.Borders.Right.Color = Color.FromArgb(bordercolor.R, bordercolor.G, bordercolor.B);
                    }
#endif

                }

                else
                {
                    WTableRow row1 = wordTable.Rows[row];
                    row1.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                    row1.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                    row1.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                    row1.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;

                    if (row != lastRow)
                    {
                        for (int i = row + 1; i < lastRow; i++)
                        {
                            WTableRow midRow = wordTable.Rows[i];
                            midRow.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                            midRow.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                        }
                    }

                    if (column != lastColumn)
                    {
                        for (int i = column + 1; i < lastColumn; i++)
                        {
                            wordTable.Rows[row].Cells[i].CellFormat.Borders.Top.BorderType = BorderStyle.Cleared;
                            wordTable.Rows[lastRow].Cells[i].CellFormat.Borders.Bottom.BorderType = BorderStyle.Cleared;
                        }
                    }

                    WTableRow row2 = wordTable.Rows[lastRow];
                    row2.Cells[column].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                    row2.Cells[lastColumn].CellFormat.Borders.Bottom.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                    row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                    row2.Cells[lastColumn].CellFormat.Borders.Right.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;
                }
            }

            if (reportModel.ReportItemModelers.Count>0)
            {
                if (rectReportItemCellModels.Count != 0)
                {
                    rectReportItemCellModels.Clear();
                }
                if (rectCellModels.Count != 0)
                {
                    rectCellModels.Clear();
                }
                if (this.rectRowHeights.Count != 0)
                {
                    this.rectRowHeights.Clear();
                    this.rectColumnWidths.Clear();
                }

               
                if (reportModel.ModelType == ModelType.RectangleModel)
                {
                    if (rectangeModel.DataSetName != null || listValues.Contains(rectangeModel))
                    {
                        rectReportItemCellModels = GetListReportItemCellModels(reportModel.ReportItemModelers, true);
                        if (isListProcess)
                        {
                            listValues = reportModel.ReportItemModelers.ToList();
                            isListProcess = false;
                        }
                    }
                    else
                    {
                        rectReportItemCellModels = GetListReportItemCellModels(reportModel.ReportItemModelers, false);
                    }
                }
                else
                {
                    rectReportItemCellModels = GetListReportItemCellModels(reportModel.ReportItemModelers, false);
                }

                List<WordReportItemCellModel> m_listCellModels = new List<WordReportItemCellModel>();
                foreach (var model in reportModel.ReportItemModelers)
                {
                    if (model.Model.FooterReportItemModels != null && model.Model.FooterReportItemModels.Count!=0)
                    {
                   
                            if (model.ModelType == ModelType.TextBoxModel)
                            {
                                WordReportItemCellModel listrptModel = new WordReportItemCellModel();
                                listrptModel.WordHeight = model.Height;
                                listrptModel.WordWidth =model.Width;
                                listrptModel.WordLeft = model.Left;
                                listrptModel.WordTop = model.Top;
                                listrptModel.WordReportItemModel = model;
                                //m_listCellModels.Add(listrptModel);
                            }
                            rectReportItemCellModels = m_listCellModels;
                    }
                }
                
                this.rectRowHeights = layoutHelper.UpdateRowHeightValues(rectReportItemCellModels, rectCellModels, UpdateSection.Body);
                this.rectColumnWidths = layoutHelper.UpdateColumnWidthValues(rectReportItemCellModels, rectCellModels, UpdateSection.Body);
                IWTable rectTable = wordTable[row, column].AddTable();
                rectTable.TableFormat.IsAutoResized = true;
                WTableRow rowPos = wordTable.Rows[row];
                rowPos.Height = 0;
                RowFormat format = new RowFormat();
                format.Paddings.All = 0;
                format.Borders.BorderType = BorderStyle.None;
                if (this.rectRowHeights.Count > 0 && this.rectColumnWidths.Count > 0)
                {
                    rectTable.ResetCells(this.rectRowHeights.Count, this.rectColumnWidths.Count, format, 0);
                }
                rectTable.TableFormat.Borders.BorderType = BorderStyle.None;
                bool isRectangle = false;
                for (int i = 0; i < rectReportItemCellModels.Count; i++)
                {
                    if (rectReportItemCellModels[i].WordReportItemModel.ModelType == ModelType.TablixModel && rectReportItemCellModels[i].WordReportItemModel.ContainerModel!=null && rectReportItemCellModels[i].WordReportItemModel.ContainerModel.ModelType == ModelType.RectangleModel)
                    {
                        isRectangle = true;
                    }
                }
                    
                for (int i = 0; i < this.rectRowHeights.Count; i++)
                {
                    if (rectTable.Rows.Count > 0)
                    {
                        WTableRow row3 = rectTable.Rows[i];
                        row3.Height = this.PixelToPoint(this.rectRowHeights[i]);
                        if (!isRectangle)
                        {
                            row3.HeightType = TableRowHeightType.Exactly;
                        }

                        for (int j = 0; j < this.rectColumnWidths.Count; j++)
                        {
                            row3.Cells[j].Width = this.PixelToPoint(this.rectColumnWidths[j]);
                        }
                    }
                }
                
                for (int i = 0; i < rectCellModels.Count; i++)
                {
                    if (rectCellModels[i].WordReportItemCellModel.WordReportItemModel.ModelType == ModelType.RectangleModel)
                    {
                        int a = rectCellModels.IndexOf(rectCellModels[i]);
                        WordReportItemCellValue it = rectCellModels[a];
                        rectCellModels.RemoveAt(a);
                        rectCellModels.Add(it);
                    }
                }
                for (int i = 0; i < rectCellModels.Count; i++)
                {
                    this.ProcessReportModels(rectCellModels[i], reportModelCollection, rectTable);
                }
            }
            if (cellValue.WordReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DisposeEvalObjects();
                }
                else
                {
                    if (cellValue.WordReportItemCellModel.WordCellInfo != null)
                    {
                        cellValue.WordReportItemCellModel.WordCellInfo.DisposeEvalObjects();
                    }
                }
            }
        }

        private void ProcessLineModel(WordReportItemCellValue cellValue, IWTable nestTable)
        {
            IReportItemModeler reportModel = cellValue.WordReportItemCellModel.WordReportItemModel;
            LineModel lineModel = reportModel as LineModel;
#if WINRT 
            Syncfusion.DocIO.DLS.Color lineColor = new Syncfusion.DocIO.DLS.Color();
#else
            Color lineColor = new Color();
#endif
            int lastRow = 0;
            int row = cellValue.WordRowIndex - 1;
            int column = cellValue.WordColumnIndex - 1;
            if (cellValue.WordRowSpan < 0)
            {
                lastRow = row;
            }
            else
            {
               lastRow = row + cellValue.WordRowSpan;
            }
            int lastColumn = column + cellValue.WordColumnSpan;

            if (nestTable != null)
            {
                wordTable = nestTable;
            }
            else
            {
                wordTable = table;
            }

            lineColor = this.ConvertStringToColor(lineModel.LineProperties.LineColor);

            System.Drawing.PointF point1 = new System.Drawing.PointF(PixelToPoint(cellValue.WordReportItemCellModel.WordLeft), PixelToPoint(cellValue.WordReportItemCellModel.WordTop));
            System.Drawing.PointF point2 = new System.Drawing.PointF(PixelToPoint(cellValue.WordReportItemCellModel.WordLeft + cellValue.WordReportItemCellModel.WordWidth), PixelToPoint(cellValue.WordReportItemCellModel.WordTop + cellValue.WordReportItemCellModel.WordHeight));

            this.UpdateMergeBorder(cellValue, nestTable);

            if (point1.Y < point2.Y)
            {
                if (point1.X == point2.X)
                {
                    WTableRow row2 = wordTable.Rows[row];
                    WTableRow row3 = wordTable.Rows[lastRow];
                    row2.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    row3.Cells[column].CellFormat.Borders.Left.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                    if (lineModel.LineProperties.LineWidth == 1)
                    {
                        row2.Cells[column].CellFormat.Borders.Left.LineWidth = 1;
                        row3.Cells[column].CellFormat.Borders.Left.LineWidth = 1;
                    }
                    else
                    {
                        float lineWidth = (float)(lineModel.LineProperties.LineWidth * 0.75);
                        row2.Cells[column].CellFormat.Borders.Left.LineWidth = lineWidth;
                        row3.Cells[column].CellFormat.Borders.Left.LineWidth = lineWidth;
                    }
#if WINRT 
                    row2.Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
                    row3.Cells[column].CellFormat.Borders.Left.Color = Syncfusion.DocIO.DLS.Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#else 
                    row2.Cells[column].CellFormat.Borders.Left.Color = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
                    row3.Cells[column].CellFormat.Borders.Left.Color = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#endif
                }
            }
            if (point1.Y == point2.Y)
            {
                WTableRow row2 = wordTable.Rows[row];
                row2.Cells[column].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                row2.Cells[lastColumn].CellFormat.Borders.Top.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Hairline;
                if (lineModel.LineProperties.LineWidth == 1 || (lineModel.LineProperties.LineStyle != RDL.DOM.LineStyle.None && lineModel.LineProperties.LineWidth < 1))
                {
                    row2.Cells[column].CellFormat.Borders.Top.LineWidth = 1;
                    row2.Cells[lastColumn].CellFormat.Borders.Top.LineWidth = 1;
                }
                else
                {
                    float lineWidth = (float)(lineModel.LineProperties.LineWidth * 0.75);
                    row2.Cells[column].CellFormat.Borders.Top.LineWidth = lineWidth;
                    row2.Cells[lastColumn].CellFormat.Borders.Top.LineWidth = lineWidth;
                }
#if WINRT 
                row2.Cells[column].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
                row2.Cells[lastColumn].CellFormat.Borders.Top.Color = Syncfusion.DocIO.DLS.Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#else 
                 row2.Cells[column].CellFormat.Borders.Top.Color = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
                row2.Cells[lastColumn].CellFormat.Borders.Top.Color = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#endif
            }
        }

        private string GetTextRun(TextboxModel txtModel, TextRunExpval run)
        {
            if (run.Text != null)
            {
                if (run.Text.Contains("Globals.TotalPages"))
                {
                    run.Text = run.Text.Replace("Globals.TotalPages", this.TotalPages.ToString());
                }
                if (run.Text.Contains("Globals.ExecutionTime"))
                {
                    run.Text = run.Text.Replace("Globals.ExecutionTime", System.DateTime.Now.ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
                if (run.Text.Contains("User.Language"))
                {
                    run.Text = run.Text.Replace("User.Language", System.Globalization.CultureInfo.CurrentCulture.Name.ToString());
                }
            }
            return run.Text;
        }

#if !WINRT 
#if SILVERLIGHT 
        private Color ConvertStringToColor(string colorName)
#else
        new private Color ConvertStringToColor(string colorName)
#endif
        {
            Color colorValue = new Color();

            if (colorName.StartsWith("#"))
            {
#if SILVERLIGHT 
                System.Windows.Media.Color color = GetColorFromHexa(colorName);
                colorValue = Color.FromArgb(color.A, color.R, color.G, color.B);

#else
                colorValue = System.Drawing.ColorTranslator.FromHtml(colorName);

#endif
            }
            else
            {
                if (colorName.ToLower() == "lightgrey")
                    colorName = "LightGray";
#if SILVERLIGHT
                System.Windows.Media.Color color = GetColorFromHexa(colorName);
                colorValue = Color.FromArgb(color.A, color.R, color.G, color.B);

#else
                colorValue = System.Drawing.Color.FromName(colorName);
#endif

            }
            return colorValue;
        }
#else
        private Syncfusion.DocIO.DLS.Color ConvertStringToColor(string colorName)
        {
            Syncfusion.DocIO.DLS.Color colorValue = new Syncfusion.DocIO.DLS.Color();
            colorValue = GetColorFromString(colorName);
            return colorValue;
        }
#endif


        private BitmapImage BufferImage(object imageData)
        {
            return (BitmapImage)this.Buffer(imageData, typeof(BitmapImage), null, System.Globalization.CultureInfo.InvariantCulture);
        }

        private BitmapImage Buffer(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] imageData = null;

            string s = value as string;

            if (s != null)
            {
                imageData = System.Convert.FromBase64String(s);
            }
            if (value is byte[])
            {
                imageData = value as byte[];
            }

            if (imageData != null)
            {
                BitmapImage bi = new BitmapImage();
                MemoryStream stream = new MemoryStream();
#if !SILVERLIGHT
                int offset = 78;
                stream.Write(imageData, offset, imageData.Length - offset);
                stream.Seek(0, SeekOrigin.Begin);
                bi.BeginInit();
                bi.StreamSource = stream;
                bi.EndInit();
#elif WINRT 
                bi.SetSource(new MemoryStream(imageData) as Windows.Storage.Streams.IRandomAccessStream);
#else 
                bi.SetSource(new MemoryStream(imageData));
#endif
                return bi;
            }
            return null;
        }
    }
}