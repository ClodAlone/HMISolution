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
using Syncfusion.XlsIO;
#if !SILVERLIGHT
using System.Data.Sql;
#endif
using System.Globalization;
using System.Threading;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.ItemModel;

#if WINRT 
using Syncfusion.UI.Xaml.Reports;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Reports;
#endif

namespace Syncfusion.ReportWriter
{
    /// <summary>
    /// Exports the Report in Excel mode
    /// </summary>
    /// <remarks></remarks>
    public class ExcelWriter : WriterBase
    {
        #region fields

        int totalHeaderRows = 0;
        int currentSheet = 0;

        bool isHeader = false;
#if SILVERLIGHT && !WINRT
        Dictionary<string, System.Windows.Media.Color> colorValues = FetchColors();
#else
        Dictionary<string, Color> colorValues=new Dictionary<string,Color>();

#endif

        Dictionary<string, IFont> fontValues=new Dictionary<string,IFont>();

        IWorkbook workbook;
        IWorksheet sheet;
        ExcelLayoutHelper layoutHelper;
        //TablixCellInfo m_currentListCellInfo;

        List<ExcelReportItemCellModel> reportItemCellModels;
        List<ExcelReportItemCellModel> headerItemCellModels;
        List<ExcelReportItemCellValue> cellModels;

        List<double> rowHeights = null;
        List<double> columnWidths = null;

        #endregion fields

        #region Initializer/Finalizer
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ExcelWriter()
        {
            DataSources = new ReportDataSourceCollection();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.RdlIO.RdlIOExportEngine">RdlIOExportEngine</see> class. 
        /// </summary>
        /// <param name="rdlFilename">The filename of the report with its full path</param>
        /// <remarks></remarks>
        public ExcelWriter(string rdlFilename)
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
        public ExcelWriter(Stream rdlStream)
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
        public ExcelWriter(string rdlFilename, ReportDataSourceCollection reportDataSources)
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
        public ExcelWriter(Stream rdlStream, ReportDataSourceCollection reportDataSources)
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
        /// Exports the report.
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="response">Http Response</param>
        /// <remarks></remarks>
        public void Save(string fileName, System.Web.HttpResponse response)
        {
            if (this.ReportModel.HasReport)
            {
                IWorkbook workbook = null;
                Thread thread = new Thread(delegate()
                {
                    if (!this.ReportModel.IsEvaluatedReport)
                    {
                        if (this.DataSources.Count == 0)
                        {
                            this.ReportModel.InitilizeReport();
                        }
                        else
                        {
                            this.ReportModel.IsRDLC = true;
                            this.ReportModel.DataSources = this.DataSources;
                            this.ReportModel.InitilizeReport();
                        }

                        this.ReportModel.Evaluate();
                        this.ReportModel.UpdateSize();
                    }

                    workbook = this.ConvertToExcel(ReportModel);
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                thread.Join();
                workbook.SaveAs(fileName, ExcelSaveType.SaveAsXLS, response);
            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }
#endif
#if !WINRT 
        /// <summary>
        /// Exports the report as a Excel document
        /// </summary>
        /// <param name="ExcelFilename">The name of the Excel file to be saved</param>
        public void Save(string ExcelFilename)
        {
            using (FileStream excelStream = new FileStream(ExcelFilename, FileMode.Create))
            {
                Save(excelStream);
            }
        }
#endif

        /// <summary>
        /// Exports the report as a Excel document
        /// </summary>
        /// <param name="ExcelStream">The stream to save the Excel file</param>
        /// <remarks></remarks>
#if WINRT 
        async public System.Threading.Tasks.Task<bool> Save(Stream ExcelStream)
#else
        public void Save(Stream ExcelStream)
#endif
        {
            if (this.ReportModel.HasReport)
            {
                if (!this.ReportModel.IsEvaluatedReport)
                {
                    if (this.DataSources.Count == 0)
                    {
                        this.ReportModel.InitilizeReport();
                    }
                    else
                    {
                        this.ReportModel.IsRDLC = true;
                        this.ReportModel.DataSources = this.DataSources;
                        this.ReportModel.InitilizeReport();
                    }

                    this.ReportModel.Evaluate();
                    this.ReportModel.UpdateSize();
                }

                IWorkbook workbook = this.ConvertToExcel(ReportModel);
#if WINRT
                await workbook.SaveAsAsync(ExcelStream, ExcelSaveType.SaveAsXLS);
                workbook.Close();
                return true;
#else 
                workbook.SaveAs(ExcelStream, ExcelSaveType.SaveAsXLS);
#endif
            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }

        #endregion Public Methods

        /// <summary>
        /// Converts the PageModelFactory into its Excel document
        /// </summary>
        /// <param name="reportModel">The ReportModel of the report</param>
        /// <returns>Returns its equivalent PDF document</returns>
        /// <remarks></remarks>
        internal IWorkbook ConvertToExcel(ReportModel reportModel)
        {
            this.layoutHelper = new ExcelLayoutHelper();
            this.rowHeights = new List<double>();
            this.columnWidths = new List<double>();
#if !SILVERLIGHT
            colorValues = new Dictionary<string, Color>();
#endif

            fontValues = new Dictionary<string, IFont>();

            PageModelFactory factory = new PageModelFactory(reportModel);

            double leftMargin, rightMargin, topMargin, bottomMargin;

            if (this.PageSettings != null)
            {
                factory.PageWidth = PageSettings.PageWidth;
                factory.PageHeight = PageSettings.PageHeight;

                leftMargin = PageSettings.LeftMargin;
                rightMargin = PageSettings.RightMargin;
                topMargin = PageSettings.TopMargin;
                bottomMargin = PageSettings.BottomMargin;
            }
            else
            {
                factory.PageWidth = ReportModel.Page.PageWidth.PixelValue;
                factory.PageHeight = ReportModel.Page.PageHeight.PixelValue;

                leftMargin = ReportModel.Page.LeftMargin != null && ReportModel.Page.LeftMargin.size != null ? ReportModel.Page.LeftMargin.PixelValue : 0;
                rightMargin = ReportModel.Page.RightMargin != null && ReportModel.Page.RightMargin.size != null ? ReportModel.Page.RightMargin.PixelValue : 0;
                topMargin = ReportModel.Page.TopMargin != null && ReportModel.Page.TopMargin.size != null ? ReportModel.Page.TopMargin.PixelValue : 0;
                bottomMargin = ReportModel.Page.BottomMargin != null && ReportModel.Page.BottomMargin.size != null ? ReportModel.Page.BottomMargin.PixelValue : 0;
            }

            factory.Margin = new LayoutThicknessInfo(leftMargin, topMargin, rightMargin, bottomMargin);

            factory.UpdateFlowPageLayout();

            ExcelEngine engine = new ExcelEngine();
            IWorkbook excelBook = engine.Excel.Workbooks.Create(1);
            this.workbook = excelBook;
            workbook.Version = (Syncfusion.XlsIO.ExcelVersion)Enum.Parse(typeof(Syncfusion.XlsIO.ExcelVersion),this.ExcelVersion.ToString(),true);
            this.currentSheet = 0;

            this.layoutHelper.PageModelFactoty = factory;
            this.layoutHelper.IsExcelWriter = true;

            foreach (var page in layoutHelper.PageModelFactoty.FlowLayoutDictionary)
            {
                this.rowHeights.Clear();
                this.columnWidths.Clear();

                this.cellModels = new List<ExcelReportItemCellValue>();
                this.reportItemCellModels = new List<ExcelReportItemCellModel>();
                this.headerItemCellModels = new List<ExcelReportItemCellModel>();

                if (this.currentSheet != 0)
                {
                    workbook.Worksheets.Create();
                }

                this.sheet = workbook.Worksheets[this.currentSheet];
                this.sheet.IsGridLinesVisible = false;

                if (layoutHelper.PageModelFactoty.HeaderHeight > 0)
                {
                    if (reportModel.Report.Page != null && reportModel.Report.Page.PageHeader != null && reportModel.Report.Page.PageHeader.ReportItems.Count != 0)
                    {
                        this.reportItemCellModels = layoutHelper.GetReportItemCellModels(UpdateSection.Header, null, this.currentSheet);
                        this.headerItemCellModels = this.reportItemCellModels;
                        this.rowHeights = (layoutHelper.UpdateRowHeightValues(this.reportItemCellModels, this.cellModels, UpdateSection.Header, 0));
                        this.columnWidths = (layoutHelper.UpdateColumnWidthValues(this.reportItemCellModels, this.cellModels, UpdateSection.Header));
                        this.sheet.Range[this.rowHeights.Count + 1, 1].FreezePanes();
                        totalHeaderRows = this.rowHeights.Count - 1;
                        isHeader = true;
                    }
                    else if (reportModel.Page != null && reportModel.Page.PageHeader != null && reportModel.Page.PageHeader.ReportItems.Count != 0)
                    {
                        this.reportItemCellModels = layoutHelper.GetReportItemCellModels(UpdateSection.Header, null, this.currentSheet);
                        this.headerItemCellModels = this.reportItemCellModels;
                        this.rowHeights = (layoutHelper.UpdateRowHeightValues(this.reportItemCellModels, this.cellModels, UpdateSection.Header, 0));
                        this.columnWidths = (layoutHelper.UpdateColumnWidthValues(this.reportItemCellModels, this.cellModels, UpdateSection.Header));
                        this.sheet.Range[this.rowHeights.Count + 1, 1].FreezePanes();
                        totalHeaderRows = this.rowHeights.Count - 1;
                        isHeader = true;
                    }
                }
                if (this.reportItemCellModels.Count != 0)
                {
                    //this.cellModels.Clear();
                    //this.rowHeights.Clear();
                    this.columnWidths.Clear();
                }

                this.reportItemCellModels = layoutHelper.GetReportItemCellModels(UpdateSection.Body, null, this.currentSheet);

                if (isHeader)
                {
                    List<ExcelReportItemCellValue> tempModels = new List<ExcelReportItemCellValue>();
                    var tempheight = layoutHelper.UpdateRowHeightValues(this.reportItemCellModels, tempModels, UpdateSection.Body, totalHeaderRows);
                    this.cellModels.AddRange(tempModels);
                    this.rowHeights.AddRange(tempheight);
                    this.headerItemCellModels.AddRange(this.reportItemCellModels);
                    this.columnWidths = layoutHelper.UpdateColumnWidthValues(this.headerItemCellModels, this.cellModels, UpdateSection.Body);
                }
                else
                {
                    this.rowHeights = layoutHelper.UpdateRowHeightValues(this.reportItemCellModels, this.cellModels, UpdateSection.Body, 0);
                    this.columnWidths = layoutHelper.UpdateColumnWidthValues(this.reportItemCellModels, this.cellModels, UpdateSection.Body);
                }

                foreach (var cellModel in this.cellModels)
                {
                    if (cellModel.ReportItemCellModel.CellInfo != null && cellModel.ReportItemCellModel.CellInfo.ItemModel.ModelType == ModelType.TablixModel)
                    {
                        List<ExcelReportItemCellModel> reportcellModels = new List<ExcelReportItemCellModel>();
                        reportcellModels = layoutHelper.GetReportItemCellModels(UpdateSection.Body, cellModel.ReportItemCellModel, this.currentSheet);
                        this.UpdateTablix(reportcellModels, reportModel);
                    }
                    else
                    {
                        this.ProcessReportModels(cellModel, reportModel);
                    }
                }

                int rowIndex = 1;
                int columnIndex = 1;

                foreach (var heightValue in this.rowHeights)
                {
                    if (heightValue < 0 || heightValue > 409.5)
                    {
                        sheet.SetRowHeightInPixels(rowIndex++, 0);
                    }
                    else
                    {
                        sheet.SetRowHeightInPixels(rowIndex++, heightValue);
                    }
                }

                foreach (var widthValue in this.columnWidths)
                {
                    sheet.SetColumnWidthInPixels(columnIndex++, (int)Math.Floor(widthValue));
                }

                this.currentSheet++;

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

                if (this.headerItemCellModels != null)
                {
                    this.DisposeCellModels(this.headerItemCellModels);
                    this.headerItemCellModels.Clear();
                }

                this.cellModels = null;
                this.reportItemCellModels = null;
                this.headerItemCellModels = null;
            }

            this.colorValues.Clear();
            this.fontValues.Clear();
            this.colorValues = null;
            this.fontValues = null;

            this.rowHeights = null;
            this.columnWidths = null;
            this.sheet = null;
            this.layoutHelper.PageModelFactoty = null;
            this.workbook = null;
            this.layoutHelper = null;
            //this.m_currentListCellInfo = null;

            return excelBook;
        }

        private void UpdateTablix(List<ExcelReportItemCellModel> reportcellModels, ReportModel reportModel)
        {
            List<ExcelReportItemCellValue> cellModel = new List<ExcelReportItemCellValue>();
            List<double> rowHeight = new List<double>();
            List<double> columnWidth = new List<double>();
            rowHeight = layoutHelper.UpdateRowHeightValues(reportcellModels, cellModel, UpdateSection.Body, 0);
            columnWidth = layoutHelper.UpdateColumnWidthValues(reportcellModels, cellModel, UpdateSection.Body);

            foreach (var cellModels in cellModel)
            {
                if (cellModels.ReportItemCellModel.CellInfo.ItemModel.ModelType == ModelType.TablixModel)
                {
                    List<ExcelReportItemCellModel> reportModels = new List<ExcelReportItemCellModel>();
                    reportModels = layoutHelper.GetReportItemCellModels(UpdateSection.Body, cellModels.ReportItemCellModel, this.currentSheet);
                    this.UpdateTablix(reportModels, reportModel);
                }
                else
                {
                    this.ProcessReportModels(cellModels, reportModel);
                }
            }
            int rowIndex = 1;
            int colIndex = 1;

            foreach (var heightValue in rowHeight)
            {
                if (heightValue < 0)
                {
                    this.sheet.SetRowHeightInPixels(rowIndex++, 0);
                }
                else
                {
                    this.sheet.SetRowHeightInPixels(rowIndex++, heightValue);
                }
            }

            foreach (var widthValue in columnWidth)
            {
                this.sheet.SetColumnWidthInPixels(colIndex++, (int)Math.Floor(widthValue));
            }

        }

        private void DisposeCellModels(List<ExcelReportItemCellModel> disposeModels)
        {
            foreach (var model in disposeModels)
            {
                model.DataSource = null;
                model.FieldValues = null;
                model.ReportItemModel = null;
                model.CellInfo = null;
            }
        }

        private void DisposeCellModels(List<ExcelReportItemCellValue> disposeValues)
        {
            foreach (var cellValue in disposeValues)
            {
                cellValue.ReportItemCellModel = null;
                cellValue.Right = null;
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

        private void ProcessReportModels(ExcelReportItemCellValue cellModel, ReportModel reportModel)
        {
            //if (cellModel.ReportItemCellModel.ReportItemModel != null)
            //{
            this.ProcessCellModel(cellModel, reportModel);
            //}
        }

        void ProcessCellModel(ExcelReportItemCellValue cellModel, ReportModel reportModel)
        {
            if (cellModel.ReportItemCellModel.ReportItemModel != null)
            {
                switch (cellModel.ReportItemCellModel.ReportItemModel.ModelType)
                {
                    case ModelType.TextBoxModel:
                        ProcessTextBox(cellModel, reportModel);
                        break;
                    case ModelType.ImageModel:
                        ProcessImageModel(cellModel);
                        break;
                    case ModelType.LineModel:
                        ProcessLineModel(cellModel);
                        break;
                    case ModelType.RectangleModel:
                        ProcessRectangleModel(cellModel);
                        break;
                    case ModelType.GaugeModel:
                        ProcessGaugeModel(cellModel);
                        break;
                    case ModelType.ChartModel:
                        ProcessChartModel(cellModel);
                        break;

#if !SyncfusionFramework3_5
                    case ModelType.MapModel:
                        ProcessMapModel(cellModel);
                        break;
#endif

                    default:
                        return;
                }
            }
            else
            {
                switch (cellModel.ReportItemCellModel.CellInfo.ItemModel.ModelType)
                {
                    case ModelType.TextBoxModel:
                        ProcessTextBox(cellModel, reportModel);
                        break;
                    case ModelType.ImageModel:
                        ProcessImageModel(cellModel);
                        break;
                    case ModelType.LineModel:
                        ProcessLineModel(cellModel);
                        break;
                    case ModelType.RectangleModel:
                        ProcessRectangleModel(cellModel);
                        break;
                    case ModelType.GaugeModel:
                        ProcessGaugeModel(cellModel);
                        break;
                    case ModelType.ChartModel:
                        ProcessChartModel(cellModel);
                        break;

#if !SyncfusionFramework3_5
                    case ModelType.MapModel:
                        ProcessMapModel(cellModel);
                        break;
#endif

                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Process the rectangle report model
        /// </summary>
        /// <param name="cellValue">value of the rectangle model</param>
        /// <remarks></remarks>
        private void ProcessRectangleModel(ExcelReportItemCellValue cellValue)
        {
            int row = 0;
            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;
            if (reportModel == null)
            {
                reportModel = cellValue.ReportItemCellModel.CellInfo.ItemModel;
            }

            if (cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DataSource = cellValue.ReportItemCellModel.DataSource;
                    reportModel.FieldValues = cellValue.ReportItemCellModel.FieldValues;
                    reportModel.RowNumbers = cellValue.ReportItemCellModel.RowNumbers;
                    reportModel.Evaluate();
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        reportModel.RowNumbers = cellValue.ReportItemCellModel.CellInfo.RowNumbers;
                    }
                }
            }

            RectangleModel rectangeModel = reportModel as RectangleModel;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
            }
            else
            {
                row = cellValue.RowIndex;
            }

            int lastRow = 0;
            if (cellValue.RowSpan >= 0)
            {
                lastRow = row + cellValue.RowSpan;
            }
            else
            {
                lastRow = row;
            }
            int column = cellValue.ColumnIndex;
            int lastColumn = column + cellValue.ColumnSpan;
            IRange range = sheet.Range[row, column, lastRow, lastColumn];

            //if (!isTablixCell)
            //{               
            if (rectangeModel.ReportItem.Style.Border != null)
            {
                if (rectangeModel.ReportItem.Style.Border.Style == "Solid")
                {
                    sheet.Range[row, column, row, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    sheet.Range[lastRow, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    sheet.Range[row, column, lastRow, column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    sheet.Range[row, lastColumn, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                }
                else
                {
                    sheet.Range[row, column, row, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                    sheet.Range[lastRow, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    sheet.Range[row, column, lastRow, column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                    sheet.Range[row, lastColumn, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                }
            }
            else
            {
                sheet.Range[row, column, row, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                sheet.Range[lastRow, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                sheet.Range[row, column, lastRow, column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                sheet.Range[row, lastColumn, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
            }
            //}

            if (rectangeModel.ReportItem.Style.Border != null)
            {
#if WINRT
                Windows.UI.Color borderColor;
#elif SILVERLIGHT
                System.Windows.Media.Color borderColor;
#else 
                Color borderColor;
#endif

                if (rectangeModel.ReportItem.Style.Border.Color != null)
                {
                    if (colorValues != null && !colorValues.ContainsKey(rectangeModel.ReportItem.Style.Border.Color))
                    {
#if WINRT
                        borderColor = GetUIColorFromHexa(rectangeModel.ReportItem.Style.Border.Color);
                        colorValues.Add(rectangeModel.ReportItem.Style.Border.Color, Color.FromArgb(borderColor.A,borderColor.R,borderColor.G,borderColor.B));
#elif SILVERLIGHT
                         borderColor = GetColorFromHexa(rectangeModel.ReportItem.Style.Border.Color);
                        colorValues.Add(rectangeModel.ReportItem.Style.Border.Color, borderColor);

#else
                        borderColor = GetColorFromHexa(rectangeModel.ReportItem.Style.Border.Color);
                        colorValues.Add(rectangeModel.ReportItem.Style.Border.Color, Color.FromArgb(borderColor.R, borderColor.G, borderColor.B));

#endif

                    }

#if !WINRT 
                    borderColor = colorValues[rectangeModel.ReportItem.Style.Border.Color];
#else
                    borderColor = GetUIColorFromHexa(rectangeModel.ReportItem.Style.Border.Color);
#endif
                    sheet.Range[row, column, row, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = borderColor;
                    sheet.Range[lastRow, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = borderColor;
                    sheet.Range[row, column, lastRow, column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = borderColor;
                    sheet.Range[row, lastColumn, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = borderColor;
                }
            }

            if (rectangeModel.ReportItem.Style.BackgroundColor != null)
            {
                string backGroundColor = null;
                //Color backColor = new Color();
                if (rectangeModel.ContainerModel != null)
                {
                    if (rectangeModel.ContainerModel.ModelType == ModelType.RectangleModel && rectangeModel.ReportItem.Style.BackgroundColor == "Transparent")
                    {
                        backGroundColor = rectangeModel.ContainerModel.ReportItem.Style.BackgroundColor;
                    }
                    else
                    {
                        backGroundColor = rectangeModel.ReportItem.Style.BackgroundColor;
                    }
                }
                else
                {
                    backGroundColor = rectangeModel.ReportItem.Style.BackgroundColor;
                }
                if (backGroundColor != "Transparent")
                {
                    this.SetBackColor(range, backGroundColor);
                }
            }

            if (cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DisposeEvalObjects();
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        cellValue.ReportItemCellModel.CellInfo.DisposeEvalObjects();
                    }
                }
            }
        }

        /// <summary>
        /// Process the line model
        /// </summary>
        /// <param name="cellValue">Value of the Line Model</param>
        /// <remarks></remarks>

        private void ProcessLineModel(ExcelReportItemCellValue cellValue)
        {
            int row = 0;
            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;
            LineModel lineModel = reportModel as LineModel;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
            }
            else
            {
                row = cellValue.RowIndex;
            }

            int column = cellValue.ColumnIndex;
            int lastRow = 0;
            if (cellValue.RowSpan >= 0)
            {
                lastRow = row + cellValue.RowSpan;
            }
            else
            {
                lastRow = row;
            }
            int lastColumn = column + cellValue.ColumnSpan;

            if (!isTablixCell)
            {
#if WINRT
                Windows.UI.Color lineColor = Windows.UI.Colors.Transparent;
#elif SILVERLIGHT
                System.Windows.Media.Color lineColor = System.Windows.Media.Colors.Transparent;
#else 
                Color lineColor = Color.Empty;

#endif
                if (lineModel.LineProperties.LineColor != null)
                {

#if WINRT
                   lineColor = GetUIColorFromHexa(lineModel.LineProperties.LineColor);
#elif SILVERLIGHT
                   lineColor = GetColorFromHexa(lineModel.LineProperties.LineColor);
#else 
                    lineColor = GetColorFromHexa(lineModel.LineProperties.LineColor);

#endif

                }

                System.Drawing.PointF point1 = new System.Drawing.PointF(PixelToPoint(lineModel.Left), PixelToPoint(lineModel.Top));
                System.Drawing.PointF point2 = new System.Drawing.PointF(PixelToPoint(lineModel.Left + lineModel.Width), PixelToPoint(lineModel.Top + lineModel.Height));

                sheet.Range[row, column, lastRow, lastColumn].Merge();

                if (point1.Y < point2.Y)
                {
                    if (point1.X == point2.X)
                    {
                        if (lineModel.LineProperties.LineWidth >= 2)
                        {
                            sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        }
                        else
                        {
                            sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        }
#if !SILVERLIGHT
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#else
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = lineColor;
#endif


                    }
                    else
                    {
                        if (lineModel.LineProperties.LineWidth >= 2)
                        {
                            sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.Thick;
                        }
                        else
                        {
                            sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.Thin;
                        }
#if !SILVERLIGHT
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalDown].ColorRGB = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#else
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalDown].ColorRGB = lineColor;
#endif


                    }
                }
                if (point1.Y > point2.Y)
                {
                    if (lineModel.LineProperties.LineWidth >= 2)
                    {
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.Thick;
                    }
                    else
                    {
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.Thin;
                    }
#if !SILVERLIGHT
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalUp].ColorRGB = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#else
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.DiagonalUp].ColorRGB = lineColor;
#endif

                }
                if (point1.Y == point2.Y)
                {
                    if (lineModel.LineProperties.LineWidth >= 2)
                    {
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    else
                    {
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
#if !SILVERLIGHT
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(lineColor.R, lineColor.G, lineColor.B);
#else
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = lineColor;
#endif

                }
            }
        }

        /// <summary>
        /// Process the image model
        /// </summary>
        /// <param name="cellValue">Value of the Image Model</param>
        /// <remarks></remarks>

        private void ProcessImageModel(ExcelReportItemCellValue cellValue)
        {
            int row = 0;
            IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;

            if (reportModel == null)
            {
                reportModel = cellValue.ReportItemCellModel.CellInfo.ItemModel;
            }

            if (reportModel != null && cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DataSource = cellValue.ReportItemCellModel.DataSource;
                    reportModel.FieldValues = cellValue.ReportItemCellModel.FieldValues;
                    reportModel.RowNumbers = cellValue.ReportItemCellModel.RowNumbers;
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        reportModel.RowNumbers = cellValue.ReportItemCellModel.CellInfo.RowNumbers;
                    }
                }
                reportModel.Evaluate();
            }

            ImageModel imageModel = reportModel as ImageModel;

            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
            }
            else
            {
                row = cellValue.RowIndex;
            }

            int lastRow = 0;
            if (cellValue.RowSpan >= 0)
            {
                lastRow = row + cellValue.RowSpan;
            }
            else
            {
                lastRow = row;
            }
            int column = cellValue.ColumnIndex;
            int lastColumn = column + cellValue.ColumnSpan;

            IRange range = sheet.Range[row, column, lastRow, lastColumn];
            if (row != lastRow || column != lastColumn)
            {
                range.Merge();
            }
            if (imageModel != null)
            {
                object imageData = imageModel.ImageData;

                //if (!isTablixCell)
                try
                {
                    MemoryStream imageStream = new MemoryStream(imageData as byte[]);
                    IShape shape = sheet.Pictures.AddPicture(row, column, imageStream);
                    shape.Height = Convert.ToInt32(cellValue.ReportItemCellModel.Height);
                    shape.Width = Convert.ToInt32(cellValue.ReportItemCellModel.Width);
                    Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl image = shape as Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl;
                    if (imageModel.ReportItem.Style == null || imageModel.ReportItem.Style.Border == null ||
                        imageModel.ReportItem.Style.Border.Color == null || imageModel.ReportItem.Style.Border.Style.ToLower() == "none"
                        || imageModel.ReportItem.Style.Border.Width == null)
                    {
                        image.Line.Visible = false;
                    }
                }
                //else
                catch
                {
                    Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
                    if (imageData == null)
                        return;


                    Stream imageStream = null;

#if !SILVERLIGHT
                    try
                    {
                        imageStream = ((BitmapImage)base64ImageConverter.ConvertToImage(imageModel.ImageData)).StreamSource;
                    }
                    catch
                    {
                        imageStream = ((BitmapImage)BufferImage(imageModel.ImageData)).StreamSource;
                    }
#endif

                    range.AutofitRows();
                    range.AutofitColumns();
                    IShape shape = sheet.Pictures.AddPicture(row, column, imageStream);
                    if (reportModel.FlowLayoutInfo != null)
                    {
                        shape.Height = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualHeight) - 1);
                        shape.Width = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualWidth) - 1);
                    }
                    else
                    {
                        shape.Height = (Convert.ToInt32(reportModel.Height));
                        shape.Width = (Convert.ToInt32(reportModel.Width));
                    }
                    Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl image = shape as Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl;
                    if (imageModel.ReportItem.Style == null || imageModel.ReportItem.Style.Border == null ||
                        imageModel.ReportItem.Style.Border.Color == null || imageModel.ReportItem.Style.Border.Style.ToLower() == "none"
                        || imageModel.ReportItem.Style.Border.Width == null)
                    {
                        image.Line.Visible = false;
                    }
                }
                int r, g, b;

                if (imageModel.ImageProperties.Border.Default != null)
                {
#if WINRT
                    Windows.UI.Color borderColor = GetUIColorFromHexa(imageModel.ImageProperties.Border.Default.BorderBrush);
#elif SILVERLIGHT
                    System.Windows.Media.Color borderColor = GetColorFromHexa(imageModel.ImageProperties.Border.Default.BorderBrush);
#else 
                    Color borderColor = System.Drawing.ColorTranslator.FromHtml(imageModel.ImageProperties.Border.Default.BorderBrush);

#endif
                    r = borderColor.R;
                    g = borderColor.G;
                    b = borderColor.B;
                    if (imageModel.ImageProperties.Border.Default.Thickness > 0 && imageModel.ImageProperties.Border.Default.BorderStyle != Syncfusion.RDL.DOM.BorderStyles.None
                        && imageModel.ImageProperties.Border.Default.BorderStyle != Syncfusion.RDL.DOM.BorderStyles.Default)
                    {
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
#if SILVERLIGHT
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = borderColor;
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = borderColor;
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = borderColor;
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = borderColor;

#else
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(r, g, b);
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(r, g, b);
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(r, g, b);
                        sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(r, g, b);
#endif
                    }
                }

                else
                {
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                    sheet.Range[row, column, lastRow, lastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                }
            }

            if (reportModel != null && cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DisposeEvalObjects();
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        cellValue.ReportItemCellModel.CellInfo.DisposeEvalObjects();
                    }
                }
            }
        }

        /// <summary>
        /// Process the Gauge model
        /// </summary>
        /// <param name="cellValue">Value of the Gauge Model</param>
        /// <remarks></remarks>
        private void ProcessGaugeModel(ExcelReportItemCellValue cellValue)
        {
            int row = 0;
            IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;

            if (reportModel == null)
            {
                reportModel = cellValue.ReportItemCellModel.CellInfo.ItemModel;
            }

            if (reportModel != null && cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DataSource = cellValue.ReportItemCellModel.DataSource;
                    reportModel.FieldValues = cellValue.ReportItemCellModel.FieldValues;
                    reportModel.RowNumbers = cellValue.ReportItemCellModel.RowNumbers;
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        reportModel.RowNumbers = cellValue.ReportItemCellModel.CellInfo.RowNumbers;
                    }
                }
                reportModel.Evaluate();
            }

            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
            }
            else
            {
                row = cellValue.RowIndex;
            }

            int lastRow = 0;
            if (cellValue.RowSpan >= 0)
            {
                lastRow = row + cellValue.RowSpan;
            }
            else
            {
                lastRow = row;
            }
            int column = cellValue.ColumnIndex;
            int lastColumn = column + cellValue.ColumnSpan;

            IRange range = sheet.Range[row, column, lastRow, lastColumn];
            if (row != lastRow || column != lastColumn)
            {
                range.Merge();
            }
            if (isTablixCell)
            {
                if (cellValue.ReportItemCellModel.CellInfo != null)
                {
                    GaugeModel gaugeModel = reportModel as GaugeModel;
                    Stream gaugeStream = gaugeModel.GetImageStream();
                    if (gaugeStream != null)
                    {
                        IPictureShape shape = sheet.Pictures.AddPicture(row, column, gaugeStream);
                        shape.Height = (Convert.ToInt32(gaugeModel.Height) - 1);
                        shape.Width = (Convert.ToInt32(gaugeModel.Width) - 1);
                    }
                }
                else
                {
                    GaugeModel gaugeModel = reportModel as GaugeModel;
                    Stream gaugeStream = gaugeModel.GetImageStream();
                    if (gaugeStream != null)
                    {
                        IPictureShape shape = sheet.Pictures.AddPicture(row, column, gaugeStream);
                        shape.Height = (Convert.ToInt32(gaugeModel.Height) - 1);
                        shape.Width = (Convert.ToInt32(gaugeModel.Width) - 1);
                    }
                }
            }
            else
            {
                GaugeModel gaugeModel = reportModel as GaugeModel;
                Stream gaugeStream = gaugeModel.GetImageStream();
                if (gaugeStream != null)
                {
                    IPictureShape shape = sheet.Pictures.AddPicture(row, column, gaugeStream);
                    Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl image = shape as Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl;
                    if (gaugeModel.ReportItem.Style == null || gaugeModel.ReportItem.Style.Border == null ||
                        gaugeModel.ReportItem.Style.Border.Color == null || gaugeModel.ReportItem.Style.Border.Style.ToLower() == "none"
                        || gaugeModel.ReportItem.Style.Border.Width == null)
                    {
                        image.Line.Visible = false;
                    }
                    shape.Height = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualHeight) - 1);
                    shape.Width = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualWidth) - 1);
                }
            }
        }

        /// <summary>
        /// Process the Chart model
        /// </summary>
        /// <param name="cellValue">Value of the Chart Model</param>
        /// <remarks></remarks>
        private void ProcessChartModel(ExcelReportItemCellValue cellValue)
        {
            int row = 0;
            IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;

            if (reportModel == null)
            {
                reportModel = cellValue.ReportItemCellModel.CellInfo.ItemModel;
            }

            if (reportModel != null && cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DataSource = cellValue.ReportItemCellModel.DataSource;
                    reportModel.FieldValues = cellValue.ReportItemCellModel.FieldValues;
                    reportModel.RowNumbers = cellValue.ReportItemCellModel.RowNumbers;
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        reportModel.RowNumbers = cellValue.ReportItemCellModel.CellInfo.RowNumbers;
                    }
                }
                reportModel.Evaluate();
            }

            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
            }
            else
            {
                row = cellValue.RowIndex;
            }

            int lastRow = 0;
            if (cellValue.RowSpan >= 0)
            {
                lastRow = row + cellValue.RowSpan;
            }
            else
            {
                lastRow = row;
            }
            int column = cellValue.ColumnIndex;
            int lastColumn = column + cellValue.ColumnSpan;

            IRange range = sheet.Range[row, column, lastRow, lastColumn];
            if (row != lastRow || column != lastColumn)
            {
                range.Merge();
            }

            if (isTablixCell)
            {
                ChartModel chartModel = reportModel as ChartModel;
                Stream chartStream = chartModel.GetImageStream();
                if (chartStream != null)
                {
                    IPictureShape shape = sheet.Pictures.AddPicture(row, column, chartStream);
                    shape.Height = (Convert.ToInt32(chartModel.Height) - 1);
                    shape.Width = (Convert.ToInt32(chartModel.Width) - 1);
                }
            }
            else
            {
                ChartModel chartModel = reportModel as ChartModel;
                Stream chartStream = chartModel.GetImageStream();
    
                if (chartStream != null)
                {
                    try
                    {
                        IPictureShape shape = sheet.Pictures.AddPicture(row, column, chartStream);
                        Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl image = shape as Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl;
                        if (chartModel.ReportItem.Style == null || chartModel.ReportItem.Style.Border == null ||
                            chartModel.ReportItem.Style.Border.Color == null || chartModel.ReportItem.Style.Border.Style.ToLower() == "none"
                            || chartModel.ReportItem.Style.Border.Width == null)
                        {
                            image.Line.Visible = false;
                        }
                        shape.Height = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualHeight) - 1);
                        shape.Width = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualWidth) - 1);
                    }
                    catch
                    {

                    }
                }
            }
        }


#if !SyncfusionFramework3_5
        private void ProcessMapModel(ExcelReportItemCellValue cellValue)
        {
            int row = 0;
            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
            }
            else
            {
                row = cellValue.RowIndex;
            }

            int column = cellValue.ColumnIndex;
            int lastRow = row + cellValue.RowSpan;
            int lastColumn = column + cellValue.ColumnSpan;

            if (row != lastRow || column != lastColumn)
            {
                sheet.Range[row, column, lastRow, lastColumn].Merge();
            }

            if (isTablixCell)
            {
                if (cellValue.ReportItemCellModel.CellInfo != null)
                {
                    IReportItemModeler reportModel = cellValue.ReportItemCellModel.CellInfo.ItemModel;
                    MapModel mapModel = reportModel as MapModel;
                    Stream mapStream = mapModel.GetImageStream();
                    if (mapStream != null)
                    {
                        IPictureShape shape = sheet.Pictures.AddPicture(row, column, mapStream);
                        shape.Height = (Convert.ToInt32(mapModel.Height) - 1);
                        shape.Width = (Convert.ToInt32(mapModel.Width) - 1);
                    }
                }
            }
            else
            {
                IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;
                MapModel mapModel = reportModel as MapModel;
                Stream mapStream = mapModel.GetImageStream();
                if (mapStream != null)
                {
                    IPictureShape shape = sheet.Pictures.AddPicture(row, column, mapStream);
                    Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl image = shape as Syncfusion.XlsIO.Implementation.Shapes.ShapeImpl;
                    if (mapModel.ReportItem.Style == null || mapModel.ReportItem.Style.Border == null ||
                        mapModel.ReportItem.Style.Border.Color == null || mapModel.ReportItem.Style.Border.Style.ToLower() == "none"
                        || mapModel.ReportItem.Style.Border.Width == null)
                    {
                        image.Line.Visible = false;
                    }
                    shape.Height = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualHeight) - 1);
                    shape.Width = (Convert.ToInt32(reportModel.FlowLayoutInfo.ActualWidth) - 1);
                }
            }
        }
#endif

        private void ProcessTextBox(ExcelReportItemCellValue cellValue, ReportModel reportModelCollection)
        {
            int row = 0;
            int lastRow = 0;

            bool isTablixCell = cellValue.ReportItemCellModel.IsTablixCell;

            if (isHeader && !cellValue.ReportItemCellModel.IsHeaderItem)
            {
                row = cellValue.RowIndex + 1;
                if (cellValue.ReportItemCellModel.CellInfo != null)
                {
                    if (cellValue.ReportItemCellModel.CellInfo.ItemModel.IsTablixChild
                        && !cellValue.ReportItemCellModel.CellInfo.ItemModel.IsTablixInnerChild)
                    {
                        cellValue.RowSpan = cellValue.RowSpan - 1;
                    }
                }
            }
            else
            {
                row = cellValue.RowIndex;
            }

            if (cellValue.RowSpan < 0)
            {
                lastRow = row;
            }
            else
            {
                lastRow = row + cellValue.RowSpan;
            }

            int column = cellValue.ColumnIndex;
            int lastColumn = column + cellValue.ColumnSpan;

            IRange range = sheet.Range[row, column, lastRow, lastColumn];

            if (row != lastRow || column != lastColumn)
            {
                range.Merge();
            }

            IReportItemModeler reportModel = cellValue.ReportItemCellModel.ReportItemModel;
            if (reportModel == null)
            {
                reportModel = cellValue.ReportItemCellModel.CellInfo.ItemModel;
            }
            TextboxModel textbox = reportModel as TextboxModel;

            if (reportModel != null && cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DataSource = cellValue.ReportItemCellModel.DataSource;
                    reportModel.FieldValues = cellValue.ReportItemCellModel.FieldValues;
                    reportModel.RowNumbers = cellValue.ReportItemCellModel.RowNumbers;
                    reportModel.Evaluate();
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        reportModel.RowNumbers = cellValue.ReportItemCellModel.CellInfo.RowNumbers;
                    }
                }
            }

            bool initial = true;
            int startPos;
            int initialRun = 0;

            List<int> startpoint = new List<int>();
            List<int> endpoint = new List<int>();
            List<IFont> fonttype = new List<IFont>();

            range.AutofitColumns();
            range.AutofitRows();
            bool hasFormat = false;
            foreach (ParagraphExpval block in textbox.ParaExpval)
            {
                startpoint.Clear();
                endpoint.Clear();
                fonttype.Clear();

                foreach (TextRunExpval run in block.Runs)
                {
                    string textValue = run.Text;
                    if (isHeader && !string.IsNullOrEmpty(textValue))
                        textValue = this.ProcessGlobals(textValue);

                    if (textValue != null)
                    {
                        if (initial)
                        {
                            startPos = 0;
                        }
                        else
                        {
                            startPos = initialRun;
                        }
                        if (!string.IsNullOrEmpty(run.Style.Format))
                        {
                            hasFormat = true;
                        }

                        range.WrapText = true;
                        initialRun += textValue.Length;
                        range.Text += textValue;
                        startpoint.Add(startPos);
                        if (initialRun == 0)
                        {
                            initialRun = 1;
                        }
                        endpoint.Add(initialRun - 1);
                        fonttype.Add(this.GetFont(run));
                        initial = false;
                    }

                    this.SetParagraphAlign(range, block);
                }

                IRichTextString rt = range.RichText;

                for (int i = 0; i < startpoint.Count; i++)
                {
                    if (startpoint[i] > endpoint[i])
                    {
                        int temp = startpoint[i];
                        startpoint[i] = endpoint[i];
                        endpoint[i] = temp;
                    }
                    try
                    {
                        rt.SetFont(startpoint[i], endpoint[i], fonttype[i]);
                    }
                    catch { }
                }
            }

            if (hasFormat && textbox.ParaExpval.Count == 1 && textbox.ParaExpval.First().Runs.Count == 1)
            {
                var run = textbox.ParaExpval.First().Runs.First();
                if (run.RunText != null)
                {
                    string runText = run.RunText.ToString();
                    range.NumberFormat = run.Style.Format;
                    if (run.Style.Format.Contains("'"))
                    {
                        range.NumberFormat = run.Style.Format.Replace("'", "\"");
                    }
                    if (run.Style.Format.ToLower().StartsWith("c") || run.Style.Format.ToLower().StartsWith("p"))
                    {
                        string appendtext = "00";
                        if (run.Style.Format.Length > 1)
                        {
                            try
                            {
                                int decimalPlace = Convert.ToInt32(run.Style.Format.Substring(1));
                                appendtext = "";
                                for (int i = 0; i < decimalPlace; i++)
                                {
                                    appendtext += "0";
                                }
                            }
                            catch { }
                        }
                        if (run.Style.Format.ToLower().StartsWith("c"))
                        {
                            range.NumberFormat = string.Format("\"$\"#,##0.{0}", appendtext);
                        }
                        else
                        {
                            range.NumberFormat = string.Format("#,##0.{0}%", appendtext);
                        }
                    }
                    double numberData = 0;

                    if (double.TryParse(runText, out numberData))
                    {
                        range.Number = numberData;
                    }
                    else
                    {
                        DateTime dateTime = DateTime.Now;

                        if (DateTime.TryParse(runText, out dateTime))
                        {
                            range.DateTime = dateTime;
                        }
                    }                    
                }
            }

            string backGroundColor = null;

            if (textbox.ReportItem.Style.BackgroundColor == null)
            {
                if (textbox.ContainerModel.ModelType == ModelType.RectangleModel && textbox.ReportItem.Style.BackgroundColor == "Transparent")
                {
                    backGroundColor = textbox.ContainerModel.ReportItem.Style.BackgroundColor;
                }
            }
            else
            {
                backGroundColor = textbox.TextBoxProperties.BackGroundColor;
            }

            this.SetBackColor(range, backGroundColor);
            this.SetTextBoxProperties(range, textbox.TextBoxProperties);

            if (reportModel != null && cellValue.ReportItemCellModel.IsTablixCell)
            {
                if (this.ReportModel.EnableVirtualEvaluation)
                {
                    reportModel.DisposeEvalObjects();
                }
                else
                {
                    if (cellValue.ReportItemCellModel.CellInfo != null)
                    {
                        cellValue.ReportItemCellModel.CellInfo.DisposeEvalObjects();
                    }
                }
            }
        }

        private string ProcessGlobals(string textRun)
        {
            if (textRun.Contains("Globals.TotalPages"))
            {
                textRun = textRun.Replace("Globals.TotalPages", (this.currentSheet + 1).ToString());
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
                textRun = textRun.Replace("Globals.PageNumber", (this.currentSheet + 1).ToString());
            }

            return textRun;
        }

        private void SetBackColor(IRange range, string backGroundColor)
        {
#if !SILVERLIGHT
            Color tempColor;
#elif WINRT 
            Windows.UI.Color tempColor;

#else
            System.Windows.Media.Color tempColor;
#endif

            if (!string.IsNullOrEmpty(backGroundColor))
            {
                if (backGroundColor == "Transparent")
                {
                    backGroundColor = "white";
                }

                if (colorValues!=null && !colorValues.ContainsKey(backGroundColor))
                {
                    if (backGroundColor != "Transparent")
                    {

#if WINRT 
                        tempColor = GetUIColorFromHexa(backGroundColor);

#elif SILVERLIGHT
                        tempColor = GetColorFromHexa(backGroundColor);
                        colorValues.Add(backGroundColor, tempColor);
#else 
                        tempColor = GetColorFromHexa(backGroundColor);
                        colorValues.Add(backGroundColor, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
#endif

                    }
                    else
                    {
#if !SILVERLIGHT
                        colorValues.Add(backGroundColor, Color.Transparent);
#endif
                    }
                }
                if (backGroundColor.ToLower() != "white")
                {
#if WINRT 
                    tempColor = GetUIColorFromHexa(backGroundColor);
#else
                    tempColor = colorValues[backGroundColor];
#endif
                    range.CellStyle.Color = tempColor;
                }
            }
        }

        int fontCount = 0;
        private IFont GetFont(TextRunExpval run)
        {
#if !SILVERLIGHT
            Color tempColor;
#elif WINRT 
            Windows.UI.Color tempColor;
#else
            System.Windows.Media.Color tempColor;
#endif
            string fontKey = "" + run.Style.Font.FontWeight + "," + run.Style.Font.FontStyle + ",";
            fontKey += string.IsNullOrEmpty(run.Style.TextColor) ? "," : run.Style.TextColor;
            fontKey = fontKey + run.Style.Font.FontSize + "," + run.Style.Font.FontFamily;

            IFont textFont = null;

            if (!fontValues.ContainsKey(fontKey))
            {
                textFont = this.workbook.CreateFont();

                if (run.Style.Font.FontWeight == RDL.DOM.FontWeight.Bold)
                {
                    textFont.Bold = true;
                }
                if (run.Style.Font.FontStyle == RDL.DOM.FontStyle.Italic)
                {
                    textFont.Italic = true;
                }
                if (run.Style.TextColor != null)
                {
                    if (colorValues!=null && !colorValues.ContainsKey(run.Style.TextColor))
                    {
#if !SILVERLIGHT
                        tempColor = GetColorFromHexa(run.Style.TextColor);
                        colorValues.Add(run.Style.TextColor, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
#elif WINRT 
                        tempColor = GetUIColorFromHexa(run.Style.TextColor);
#else
                        tempColor = GetColorFromHexa(run.Style.TextColor);
                        colorValues.Add(run.Style.TextColor,tempColor);
#endif
                    }

#if WINRT 
                    tempColor = GetUIColorFromHexa(run.Style.TextColor);

#else
                    tempColor = colorValues[run.Style.TextColor];

#endif
                    textFont.RGBColor = tempColor;
                }

                textFont.Size = this.PixelToPoint(run.Style.Font.FontSize);
                textFont.FontName = run.Style.Font.FontFamily;
                fontValues.Add(fontKey, textFont);
                fontCount++;
            }
            else
            {
                textFont = fontValues[fontKey];
            }

            return textFont;
        }

        private void SetParagraphAlign(IRange range, ParagraphExpval paragraph)
        {
            switch (paragraph.TextAlignment)
            {
                case "Right":
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                    break;
                case "Left":
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    break;
                case "Center":
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    break;
                case "Stretch":
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    break;
            }
        }

        private void SetTextBoxProperties(IRange range, ReportItemExpval expVal)
        {
#if !SILVERLIGHT
            Color tempColor;
#elif WINRT 
            Windows.UI.Color tempColor;
#else
            System.Windows.Media.Color tempColor;
#endif

            switch (expVal.VerticalAlignment)
            {
                case Syncfusion.RDL.DOM.VerticalAlign.Bottom:
                    range.CellStyle.VerticalAlignment = ExcelVAlign.VAlignBottom;
                    break;
                case Syncfusion.RDL.DOM.VerticalAlign.Middle:
                    range.CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    break;
                case Syncfusion.RDL.DOM.VerticalAlign.Default:
                    range.CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    break;
                case Syncfusion.RDL.DOM.VerticalAlign.Top:
                    range.CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    break;
            }

            if (expVal.Border.Default != null && expVal.Border.Default.BorderBrush != null && expVal.Border.Default.BorderStyle != RDL.DOM.BorderStyles.None && expVal.Border.Default.BorderStyle != RDL.DOM.BorderStyles.Default)
            {
                if (colorValues != null && !colorValues.ContainsKey(expVal.Border.Default.BorderBrush))
                {
#if !SILVERLIGHT
                    tempColor = GetColorFromHexa(expVal.Border.Default.BorderBrush);
                    colorValues.Add(expVal.Border.Default.BorderBrush, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));

#elif WINRT 
                    tempColor = GetUIColorFromHexa(expVal.Border.Default.BorderBrush);
#else
                    tempColor = GetColorFromHexa(expVal.Border.Default.BorderBrush);
                    colorValues.Add(expVal.Border.Default.BorderBrush,tempColor);
#endif
                }

#if WINRT 
                tempColor = tempColor = GetUIColorFromHexa(expVal.Border.Default.BorderBrush);

#else
                tempColor = colorValues[expVal.Border.Default.BorderBrush];
#endif
                ExcelLineStyle style = ExcelLineStyle.None;

                if (expVal.Border.Default.Thickness <= 0)
                {
                    style = ExcelLineStyle.Hair;
                }
                else if (expVal.Border.Default.Thickness == 1)
                {
                    style = ExcelLineStyle.Thin;
                }
                else
                {
                    style = ExcelLineStyle.Thick;
                }

                if (expVal.Border.RightBorder == null)
                {
                    range.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = tempColor;
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = style;
                }
                if (expVal.Border.LeftBorder == null)
                {
                    range.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = tempColor;
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = style;
                }
                if (expVal.Border.TopBorder == null)
                {
                    range.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = tempColor;
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = style;
                }
                if (expVal.Border.BottomBorder == null)
                {
                    range.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = tempColor;
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = style;
                }
            }

            if (expVal.Border.TopBorder != null && expVal.Border.TopBorder.BorderBrush != null
                && expVal.Border.TopBorder.BorderStyle != RDL.DOM.BorderStyles.None && expVal.Border.TopBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
            {
                if (colorValues != null && !colorValues.ContainsKey(expVal.Border.TopBorder.BorderBrush))
                {
#if !SILVERLIGHT
                    tempColor = GetColorFromHexa(expVal.Border.TopBorder.BorderBrush);
                    colorValues.Add(expVal.Border.TopBorder.BorderBrush, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));

#elif  WINRT 
                    tempColor = GetUIColorFromHexa(expVal.Border.TopBorder.BorderBrush);
#else
                    tempColor = GetColorFromHexa (expVal.Border.TopBorder.BorderBrush);
                    colorValues.Add(expVal.Border.TopBorder.BorderBrush,tempColor);

#endif
                }
#if WINRT 
                tempColor = GetUIColorFromHexa(expVal.Border.TopBorder.BorderBrush);
#else
                tempColor = colorValues[expVal.Border.TopBorder.BorderBrush];
#endif

                range.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = tempColor;

                if (expVal.Border.TopBorder.Thickness <= 0)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Hair;
                }
                else if (expVal.Border.TopBorder.Thickness == 1)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                }
                else
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                }
            }

            if (expVal.Border.BottomBorder != null && expVal.Border.BottomBorder.BorderBrush != null
                && expVal.Border.BottomBorder.BorderStyle != RDL.DOM.BorderStyles.None && expVal.Border.BottomBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
            {
                if (colorValues != null && !colorValues.ContainsKey(expVal.Border.BottomBorder.BorderBrush))
                {
#if !SILVERLIGHT
                    tempColor = GetColorFromHexa(expVal.Border.BottomBorder.BorderBrush);
                    colorValues.Add(expVal.Border.BottomBorder.BorderBrush, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
#elif WINRT 
                    tempColor = GetUIColorFromHexa(expVal.Border.BottomBorder.BorderBrush);
#else
                    tempColor = GetColorFromHexa(expVal.Border.BottomBorder.BorderBrush);
                    colorValues.Add(expVal.Border.BottomBorder.BorderBrush,tempColor);

#endif
                }

#if WINRT 
                tempColor = GetUIColorFromHexa(expVal.Border.BottomBorder.BorderBrush);

#else 
                tempColor = colorValues[expVal.Border.BottomBorder.BorderBrush];

#endif

                range.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = tempColor;

                if (expVal.Border.BottomBorder.Thickness <= 0)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                }
                else if (expVal.Border.BottomBorder.Thickness == 1)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                }
                else
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                }
            }

            if (expVal.Border.LeftBorder != null && expVal.Border.LeftBorder.BorderBrush != null
                && expVal.Border.LeftBorder.BorderStyle != RDL.DOM.BorderStyles.None && expVal.Border.LeftBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
            {
                if (colorValues != null && !colorValues.ContainsKey(expVal.Border.LeftBorder.BorderBrush))
                {
#if !SILVERLIGHT
                    tempColor = GetColorFromHexa(expVal.Border.LeftBorder.BorderBrush);
                    colorValues.Add(expVal.Border.LeftBorder.BorderBrush, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));

#elif WINRT 
                    tempColor = GetUIColorFromHexa(expVal.Border.LeftBorder.BorderBrush);
#else 

#endif
                }
#if WINRT 
                tempColor = GetUIColorFromHexa(expVal.Border.LeftBorder.BorderBrush);
#else 
                tempColor = colorValues[expVal.Border.LeftBorder.BorderBrush];

#endif

                range.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = tempColor;

                if (expVal.Border.LeftBorder.Thickness <= 0)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Hair;
                }
                else if (expVal.Border.LeftBorder.Thickness == 1)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                }
                else
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                }
            }

            if (expVal.Border.RightBorder != null && expVal.Border.RightBorder.BorderBrush != null
                && expVal.Border.RightBorder.BorderStyle != RDL.DOM.BorderStyles.None && expVal.Border.RightBorder.BorderStyle != RDL.DOM.BorderStyles.Default)
            {
                if (colorValues != null && !colorValues.ContainsKey(expVal.Border.RightBorder.BorderBrush))
                {
#if !SILVERLIGHT
                    tempColor = GetColorFromHexa(expVal.Border.RightBorder.BorderBrush);
                    colorValues.Add(expVal.Border.RightBorder.BorderBrush, Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));

#elif WINRT 
                    tempColor = GetUIColorFromHexa(expVal.Border.RightBorder.BorderBrush);
#else 
                    tempColor = GetColorFromHexa(expVal.Border.RightBorder.BorderBrush);
                    colorValues.Add(expVal.Border.RightBorder.BorderBrush, tempColor);

#endif
                }

#if WINRT 
                tempColor = GetUIColorFromHexa(expVal.Border.RightBorder.BorderBrush);
#else
                tempColor = colorValues[expVal.Border.RightBorder.BorderBrush];

#endif
                range.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = tempColor;

                if (expVal.Border.RightBorder.Thickness <= 0)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Hair;
                }
                else if (expVal.Border.RightBorder.Thickness == 1)
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                }
                else
                {
                    range.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                }
            }
        }

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
#if SILVERLIGHT
                Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
                BitmapImage bitMapImage = new BitmapImage();
                bitMapImage = (BitmapImage)base64ImageConverter.ConvertToImage(imageData);
                return bitMapImage;
#else
                BitmapImage bi = new BitmapImage();
                MemoryStream stream = new MemoryStream();
                int offset = 78;
                stream.Write(imageData, offset, imageData.Length - offset);
                stream.Seek(0, SeekOrigin.Begin);
                bi.BeginInit();
                bi.StreamSource = stream;
                bi.EndInit();
                return bi;
#endif
            }
            return null;
        }
    }
}