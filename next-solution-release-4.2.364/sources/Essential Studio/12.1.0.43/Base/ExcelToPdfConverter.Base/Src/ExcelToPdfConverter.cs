#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Native;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.Drawing.Imaging;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.FormatParser;
using XlsIOTokenType = Syncfusion.XlsIO.FormatParser.FormatTokens;
using System.Windows.Forms;
using Syncfusion.XlsIO.Implementation.Charts;

namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// This class provides functions for the conversion of Excel documents to the Pdf documents.
    /// </summary>
    public class ExcelToPdfConverter : IDisposable
    {
        #region Constants
        private const float LeftandRightMargin = 20;
        private const float TopandBottomMargin = 10;
        private const float MaxFooterWidth = 60;
        private const float MaxHeaderHeight = 60;
        private const string NewLineKey = "_x000A_";
        private const string CarriageReturnKey = "_x000D_";
        private const string TabKey = "_x0009_";
        private const string ExponentialFormatString = "{0:0.##E+00}";
        #endregion

        #region Fields

        /// <summary>
        /// Represents the object of Excel sheet conditional formatting .
        /// </summary>
        private CFApplier conditionalFormatApplier;

        /// <summary>
        /// Indicates the object of the pdf document.
        /// </summary>
        private PdfDocument pdfDocument;

        /// <summary>
        /// Indicates the current rendering pdf page in the document.
        /// </summary>
        private PdfPage currentPage;

        /// <summary>
        /// Indicates the active workbook of the input document.
        /// </summary>
        private IWorkbook workBook;

        /// <summary>
        /// Indicates the active worksheet of the workbook.
        /// </summary>
        private IWorksheet workSheet;

        /// <summary>
        /// Indicates the object of the workbook Implementation class.
        /// </summary>
        private WorkbookImpl workBookImpl;

        /// <summary>
        /// This object is used for the conversion between the units.
        /// </summary>
        private PdfUnitConvertor m_pdfUnitConverter;

        /// <summary>
        /// Indicates the object of the Excel to pdf converter settings object.
        /// </summary>
        private ExcelToPdfConverterSettings excelToPdfSettings;

        /// <summary>
        /// Indicates the object of the pdf template in which the document is drawn first.
        /// </summary>
        private PdfTemplate pdfPageTemplate = null;

        /// <summary>
        /// Indicates the object of the pdf graphics of the current pdf page.
        /// </summary>
        private PdfGraphics pdfGraphics;

        /// <summary>
        /// Indicates the object of the excel engine.
        /// </summary>
        private ExcelEngine excelEngine;

        /// <summary>
        /// Indicates the object of the current Pdf section.
        /// </summary>
        private PdfSection pdfSection;

        /// <summary>
        /// Represents the collection of Pdf templates.
        /// </summary>
        private List<PdfTemplate> pdfTemplateCollection;

        /// <summary>
        /// Represents the collection of Header and Footer representations and their respecive values.
        /// </summary>
        private Dictionary<string, string> predefinedHeaderFooter = new Dictionary<string, string>();

        /// <summary>
        /// Represents the collection of table style border color list.
        /// </summary>
        private Dictionary<IRange, Dictionary<ExcelBordersIndex, Color>> tableBorderColorList;

        /// <summary>
        /// Represents the collection of the table style font list.
        /// </summary>
        private Dictionary<IRange, Color> fontTableColors;

        /// <summary>
        /// Represents the collection of the header and footer objects.
        /// </summary>
        private List<HeaderFooter> headerFooter;

        /// <summary>
        /// Indicates the Excelborder index width.
        /// </summary>
        private float borderWidth = 0;

        /// <summary>
        /// Indicates the no of page counts in the Pdf/
        /// </summary>
        private int pageCount = 1;

        /// <summary>
        /// Indicates the Bookmark for the Pdf page.
        /// </summary>
        private bool bookmark = false;

        /// <summary>
        /// Indicates the scaled page width to which the template should be drawn.
        /// </summary>
        private float scaledPageWidth;

        /// <summary>
        /// Indicates the scaled page height to which the template should be drawn.
        /// </summary>
        private float scaledPageHeight;
        /// <summary>
        /// Indicates the scaled page width to which the template should be drawn.
        /// </summary>
        private float scaledPageHFWidth;

        /// <summary>
        /// Indicates the scaled page height to which the template should be drawn.
        /// </summary>
        private float scaledPageHFHeight;

        /// <summary>
        /// Indicates the list of the fonts collections to which the fonts are need to substituted.
        /// </summary>
        private List<string> fontList = new List<string>();

        /// <summary>
        /// Indicates the object of the Table style.
        /// </summary>
        private TableStyleRenderer tableStyle;

        /// <summary>
        /// Indicates the wrapped cell display text for a rotated text.
        /// </summary>
        private string cellDisplayText;
        private Dictionary<Font, PdfFont> fontCollection = new Dictionary<Font, PdfFont>();
        internal float sheetHeight;
        internal float sheetWidth;
        private List<SplitText> splitTextCollection = new List<SplitText>();
        private bool isNewPage;
        private float adjacentRectWidth;
        private int[] unicodeChar = new int[] { 34, 183 };
        private char[] numberFormatChar = new char[] { '€' };
        private string[] numberFormats = new string[] { "[$R -1C09]* #,##0.00;[$R -1C09]* \\-#,##0.00" };
        float headerMargin;
        float footerMargin;
        float topMargin;
        float bottomMargin;
        float leftMargin;
        float rightMargin;
        /// <summary>
        /// It's define the Pdf Page-setup.
        /// </summary>
        private PageSetupOption pageSetupOption;
        /// <summary>
        /// It's define the page layout settings.
        /// </summary>
        private ExcelToPdfLayoutSetting ExcelToPdfPageLayout;
        private bool m_bIsPrintTitleRowPage;
        private bool m_bIsPrintTitleColumnPage;
        /// <summary>
        /// It's define the used range coloum width
        /// </summary>
        private ItemSizeHelper m_columnWidthGetter;
        /// <summary>
        /// It's define the used range row height
        /// </summary>
        private ItemSizeHelper m_rowHeightGetter;
        /// <summary>
        /// It's define the current draw sheet pagesetup information's
        /// </summary>
        private IPageSetup m_pageSetup;
        private bool m_bHasPrintTitleColumn;
        private bool m_bHasPrintTitleRow;
        private IRange m_sheetUsedRange;
        private enum Alignment
        {
            Left=1,
            Center=2,
            Right=4,
            None,
        }
        private bool m_fitText;
		private const string SmallFontBoldTagName = "&b";        
        private const string SmallFontUnderlineTagName = "&u";
        private const string SmallFontItalicTagName = "&i";
        private const string BigFontBoldTagName = "&B";
        private const string BigFontUnderlineTagName = "&U";        
        private const string BigFontItalicTagName = "&I";

        private List<char> removableCharaters = new List<char>();
        private bool hasContent = true;
        private PivotTableImpl pivotImpl = null;
        private IRange pivotTableRange;
        private Dictionary<int, IRange> pivotTableList = new Dictionary<int, IRange>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<ImageCodecInfo, Guid> imageCodec = new Dictionary<ImageCodecInfo, Guid>();
        /// <summary>
        /// Represents the list of sorted border values.
        /// </summary>
        private Dictionary<ExcelBordersIndex, double> m_sortedBorders = new Dictionary<ExcelBordersIndex, double>();
        /// <summary>
        /// Represents the list of border line styles.
        /// </summary>
        internal Dictionary<ExcelLineStyle, List<ExcelBordersIndex>> m_dicBorderLineStyle = new Dictionary<ExcelLineStyle, List<ExcelBordersIndex>>();
        /// <summary>
        /// Helper Methods.
        /// </summary>
        private HelperMethods Helper;
        /// <summary>
        /// Find Header Footer having the Rich Text.
        /// </summary>
        internal bool AllowHeaderFooterOnce;
        internal bool AllowHeader;
        internal bool AllowFooter;

        internal int chartIndex = 0;
        internal int sheetIndex = 0;
        private float startX = 0;
        private float headerHeight = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToPdfConverter"/> class.
        /// </summary>
        public ExcelToPdfConverter()
        {
            this.conditionalFormatApplier = new CFApplier();
            this.pdfDocument = new PdfDocument();
            this.excelToPdfSettings = new ExcelToPdfConverterSettings();
            m_pdfUnitConverter = new PdfUnitConvertor();
            pdfTemplateCollection = new List<PdfTemplate>();
            this.IntializeFonts();
            this.IntializeRemovableCharacters();
            this.InitializeImageEncoder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToPdfConverter"/> class.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        public ExcelToPdfConverter(IWorkbook workbook)
            : this()
        {
            this.workBook = workbook;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToPdfConverter"/> class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        public ExcelToPdfConverter(IWorksheet worksheet)
            : this()
        {
            this.workSheet = worksheet;
            this.workBookImpl = (WorkbookImpl)worksheet.Application.ActiveWorkbook;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToPdfConverter"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public ExcelToPdfConverter(string filePath)
            : this()
        {
            IFormatProvider formatProvider = CultureInfo.CurrentCulture;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("filePath");
            }

            this.excelEngine = new ExcelEngine();
            IApplication application = this.excelEngine.Excel;
            if (filePath.ToString().EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                application.DefaultVersion = ExcelVersion.Excel2007;
                this.workBook = application.Workbooks.Open(filePath, ExcelOpenType.Automatic);
            }
            else if (filePath.ToString().EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                application.DefaultVersion = ExcelVersion.Excel97to2003;
                this.workBook = application.Workbooks.Open(filePath, ExcelOpenType.Automatic);
            }
            else
            {
                throw new NotSupportedException(string.Format(formatProvider, "The current file format is not supported :{0}", Path.GetExtension(filePath)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToPdfConverter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public ExcelToPdfConverter(Stream stream)
            : this()
        {
            if (!(stream.CanSeek && stream.CanRead))
            {
                throw new ArgumentException("The stream cannot be read", "stream");
            }

            this.excelEngine = new ExcelEngine();
            IApplication application = this.excelEngine.Excel;
            this.workBook = application.Workbooks.Open(stream, ExcelOpenType.Automatic);
        }

        /// Gets the LayoutOptions based on PageSetup.
        /// <summary>
        /// <param name="pageSetup">Current Page setup.</param>
        /// <returns>LayoutOptions.</returns>
        /// </summary>
        public static  LayoutOptions GetLayoutOptions(PageSetupBaseImpl pageSetup)
        {
            if (!pageSetup.IsFitToPage) //fitToPage==true
            {
                return (pageSetup.Zoom == 100) ? LayoutOptions.NoScaling : LayoutOptions.CustomScaling;
            }
            else
            {
                if (pageSetup.FitToPagesWide == 1 && pageSetup.FitToPagesTall == 1)
                {
                    return LayoutOptions.FitSheetOnOnePage; ;
                }
                else if (pageSetup.FitToPagesWide == 1 && pageSetup.FitToPagesTall == 0)
                    return LayoutOptions.FitAllColumnsOnOnePage;
                else if (pageSetup.FitToPagesWide == 0 && pageSetup.FitToPagesTall == 1)
                    return LayoutOptions.FitAllRowsOnOnePage;
                else
                    return LayoutOptions.CustomScaling;
            }

        }
        #endregion

        #region Events and Delegates

        /// <summary>
        /// Delegate for Merged cell rendering.
        /// </summary>
        private delegate void MergeMethod(WorksheetImpl sheet, MergeCellsRecord.MergedRegion mergedRegion, int firstRow, int firstColumn, int lastRow, int lastColumn, PdfGraphics graphics,float originalWidth,float startX,float startY);

        /// <summary>
        /// Delegate for background cell rendering.
        /// </summary>
        private delegate void CellMethod(IRange cell, RectangleF rect, PdfGraphics graphics);

        /// <summary>
        /// Occurs when [current progress changed].
        /// </summary>
        public event CurrentProgressChangedEventHandler CurrentProgressChanged;

        /// <summary>
        /// Occurs when [sheet before drawn].
        /// </summary>
        public event SheetBeforeDrawnEventHandler SheetBeforeDrawn;

        /// <summary>
        /// Occurs when [sheet after drawn].
        /// </summary>
        public event SheetAfterDrawnEventHandler SheetAfterDrawn;

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns>pdfDocument Object</returns>
        public PdfDocument Convert()
        {
            LayoutOptions layoutOptions = this.excelToPdfSettings.LayoutOptions;
            if (this.workBook != null)
            {
                this.pdfDocument.EnableMemoryOptimization = true;
                this.workBookImpl = (WorkbookImpl)this.workBook;
                int sheetsCount = this.workBook.Worksheets.Count;
                int totalCount;
                #if SyncfusionFramework4_0 || SyncfusionFramework4_5
                totalCount = workBook.Worksheets.Count + workBook.Charts.Count;
                #else
                totalCount = workBook.Worksheets.Count;
                #endif
                for (int i = 0; i < totalCount; i++)
                {
                    if ( sheetIndex < workBook.Worksheets.Count && i == workBook.Worksheets[sheetIndex].TabIndex )
                    {
                        workSheet = this.workBook.Worksheets[sheetIndex];
                        if (workBook.BuiltInDocumentProperties[ExcelBuiltInProperty.ApplicationName].Text
                            == "Essential XlsIO" || (workBook as WorkbookImpl).IsCellModified)
                        {
                            workSheet.EnableSheetCalculations();

                            workSheet.CalcEngine.UseDatesInCalculations = true;
                            workSheet.CalcEngine.UseNoAmpersandQuotes = true;
                        }
                        DrawWorkSheet(workSheet, sheetsCount, layoutOptions);
                        sheetIndex++;
                    }
                        #if SyncfusionFramework4_0 || SyncfusionFramework4_5
                    else
                    {
                        IChart chart = workBook.Charts[chartIndex];
                        if ((workBook.Application as ApplicationImpl).ChartToImageConverter != null)
                        {
                            DrawChartSheet(chart, i, LayoutOptions.Automatic);
                        }
                        chartIndex++;
                    }
                    #endif
                }
            }
            
            else
            {
                this.workBook = workSheet.Workbook;
                if (workBook.BuiltInDocumentProperties[ExcelBuiltInProperty.ApplicationName].Text
                        == "Essential XlsIO" || (workBook as WorkbookImpl).IsCellModified)
                {
                    workSheet.EnableSheetCalculations();
                    workSheet.CalcEngine.UseDatesInCalculations = true;
                    workSheet.CalcEngine.UseNoAmpersandQuotes = true;
                }
                DrawWorkSheet(workSheet, 1, layoutOptions);
            }
            if (this.excelToPdfSettings.ExportDocumentProperties)
            {
                this.SetDocumentProperties();
            }
            return this.pdfDocument;
        }

        private void InitializaSheetSettings()
        {
            this.pageCount = 1;
            this.isNewPage = false;
            this.SplitTexts.Clear();
            this.adjacentRectWidth = 0;
            this.m_columnWidthGetter = null;
            this.m_rowHeightGetter = null;
        }
        internal void DrawWorkSheet(IWorksheet worksheet, int sheetsCount, LayoutOptions layoutOptions)
        {
            //Assign the used range
            if (workSheet.PageSetup.PrintArea != null)
                m_sheetUsedRange = workSheet.UsedRange;
            else
                m_sheetUsedRange = GetActualUsedRange(workSheet);    //.UsedRange;

            pageSetupOption = new PageSetupOption((WorksheetImpl)workSheet, m_sheetUsedRange);
            Helper = new HelperMethods(pageSetupOption);
            InitializaSheetSettings();
            bool skipSheet = this.RaiseSheetBeforeDrawn(workSheet.Index);
            if (!skipSheet)
            {
                if (this.excelToPdfSettings.ExportBookmarks || this.bookmark)
                {
                    this.excelToPdfSettings.ExportBookmarks = true;
                }

                WorksheetImpl workSheetImpl = (WorksheetImpl)workSheet;
                this.OnProgressChanged(sheetsCount, workSheet.Index);

                //Condition to check whether sheet has only images.
                if ((!workSheetImpl.IsEmpty || workSheetImpl.Shapes.Count > 0)
                          && workSheetImpl.Visibility == WorksheetVisibility.Visible )
                {
                    hasContent = false;
                    pdfPageTemplate = null;
                    pdfTemplateCollection = new List<PdfTemplate>();
                    this.headerFooter = new List<HeaderFooter>();
                    if (workSheet.IsRightToLeft)
                    {
                        this.excelToPdfSettings.EnableRTL = true;
                    }

                    if (workSheet.ListObjects.Count != 0)
                    {
                        this.tableStyle = new TableStyleRenderer(workSheet.ListObjects);
                        this.tableBorderColorList = this.tableStyle.ApplyStyles(workSheet,
                                                                                out this.fontTableColors);
                    }
                    if (workSheet.PivotTables.Count > 0)
                    {
                        for (int index = 0; index < workSheet.PivotTables.Count; index++)
                        {
                            IPivotTable pivotTable = workSheet.PivotTables[index];
#if !SyncfusionFramework2_0
                            pivotTable.Layout();
#endif
                            pivotImpl = pivotTable as PivotTableImpl;
                            pivotTableRange = pivotTable.Location;


                        }
                    }
                    if (layoutOptions == LayoutOptions.Automatic)
                        excelToPdfSettings.LayoutOptions = GetLayoutOptions(workSheet.PageSetup as PageSetupBaseImpl);
                    IRange[] rangesCount = pageSetupOption.GetBreakRanges(excelToPdfSettings.LayoutOptions, pageSetupOption.HasPrintArea ? pageSetupOption.PrintAreas : new IRange[] { m_sheetUsedRange });
                    this.pdfDocument = DrawSheet(workSheet, rangesCount);
                }
                else if (this.excelToPdfSettings.ThrowWhenExcelFileIsEmpty)
                {
                    if (workSheetImpl.Visibility != WorksheetVisibility.Hidden && hasContent)
                    {
                        throw new ExcelToPDFConverterException("The Empty Excel Document cannot be converted to an PDF document. You can disable this exception by using Boolean ThrowWhenExcelFileIsEmpty property to True or False.");
                    }
                }

                this.OnSheetAfterDrawn(workSheet.Index);
            }
            workSheet.DisableSheetCalculations();
        }
#if SyncfusionFramework4_0 || SyncfusionFramework4_5
        /// <summary>
        /// To draw the chart Sheet in to pdf
        /// </summary>
        /// <param name="chart">workbook chart</param>
        /// <param name="sheetsCount">sheets count</param>
        /// <param name="layoutOptions">layout options for the chart sheet</param>
        /// <returns></returns>
        internal PdfDocument DrawChartSheet(IChart chart, int sheetsCount, LayoutOptions layoutOptions)
        {
            pageSetupOption = new PageSetupOption();
            Helper = new HelperMethods(pageSetupOption);
            InitializaSheetSettings();

            hasContent = false;
            pdfPageTemplate = null;
            pdfTemplateCollection = new List<PdfTemplate>();
            this.headerFooter = new List<HeaderFooter>();

            ChartPageSetupImpl chartPageSetup = chart.PageSetup as ChartPageSetupImpl;
            this.pageCount = sheetIndex + chartIndex + 1;

            pdfTemplateCollection.Clear();
            

            leftMargin = Pdf_UnitConverter.ConvertUnits((float)chartPageSetup.LeftMargin,
                                                               PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            rightMargin = Pdf_UnitConverter.ConvertUnits((float)chartPageSetup.RightMargin,
                                                                PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            headerMargin = Pdf_UnitConverter.ConvertUnits((float)(chartPageSetup.HeaderMargin),
                                                              PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            topMargin = Pdf_UnitConverter.ConvertUnits((float)(chartPageSetup.TopMargin),
                                                              PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            bottomMargin = Pdf_UnitConverter.ConvertUnits((float)(chartPageSetup.BottomMargin),
                                                                 PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            footerMargin = Pdf_UnitConverter.ConvertUnits((float)(chartPageSetup.FooterMargin),
                                                                 PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            pdfSection = this.pdfDocument.Sections.Add();

            // Pdf Margin Setup.
            pdfSection.PageSettings.Margins.Left = leftMargin > 0 ? leftMargin : LeftandRightMargin;
            pdfSection.PageSettings.Margins.Right = rightMargin > 0 ? rightMargin : LeftandRightMargin;
            pdfSection.PageSettings.Margins.Top = TopMargin > 0 ? TopMargin : TopandBottomMargin;
            pdfSection.PageSettings.Margins.Bottom = BottomMargin > 0 ? BottomMargin : TopandBottomMargin;

            pdfPageTemplate = new PdfTemplate( Pdf_UnitConverter.ConvertFromPixels((float)chart.Width,PdfGraphicsUnit.Point)
                                                             , Pdf_UnitConverter.ConvertFromPixels((float)chart.Height,PdfGraphicsUnit.Point));
            

            pdfTemplateCollection.Add(pdfPageTemplate);

            if ((!String.IsNullOrEmpty(chartPageSetup.FullHeaderString)
                                       || !String.IsNullOrEmpty(chartPageSetup.FullFooterString)))
            {
                this.headerFooter = this.DrawHeadersAndFooters(pdfSection, null,chart as ChartImpl);
            }
            
            this.InitializePdfPage();

            DrawChart(chart, pdfPageTemplate);

            float headerHeight=0;
            float footerHeight=0;
            if (this.headerFooter.Count != 0)
            {
                DrawPdfPageHeaderFooter(pdfSection, this.headerFooter,(chart as ChartImpl).PageSetupBase, topMargin, bottomMargin,
                                             pdfSection.Pages[0], out headerHeight, out footerHeight);
            }
            if ((chart.PageSetup as PageSetupBaseImpl).FullHeaderString!=string.Empty)
            {
                if (headerHeight == 0)
                {
                    headerHeight = topMargin;
                }
                else if (headerHeight < topMargin)
                {
                    headerHeight = topMargin + headerMargin;
                }
                pdfSection.PageSettings.Margins.Top = 0;
            }
            if ((chart.PageSetup as PageSetupBaseImpl).FullHeaderString != string.Empty)
            {
                pdfSection.PageSettings.Margins.Bottom = 0;
            }

            if (chartPageSetup.Orientation == ExcelPageOrientation.Landscape)
                pdfSection.Pages[0].Section.PageSettings.Orientation = PdfPageOrientation.Landscape;
                        

            pdfSection.Pages[0].Graphics.DrawPdfTemplate(pdfPageTemplate, new PointF(0.0f,headerHeight),
                                                                      new SizeF(pdfPageTemplate.Width, pdfPageTemplate.Height));
            headerHeight = 0;

            //Use the Pdf HeaderFooter Feature.
            if (AllowHeader && !string.IsNullOrEmpty((chart as ChartImpl).PageSetupBase.FullHeaderString) && headerFooter.Count > 0)
            {
                if (this.excelToPdfSettings.HeaderFooterOption.ShowHeader)
                    this.pdfSection.Template.Top = AddPDFHeaderFooter((chart as ChartImpl).PageSetupBase, pdfSection, true, true);
                else
                    headerFooter.RemoveAt(0);
            }
            if (AllowFooter && !string.IsNullOrEmpty((chart as ChartImpl).PageSetupBase.FullFooterString) && headerFooter.Count > 0)
            {
                if (this.excelToPdfSettings.HeaderFooterOption.ShowFooter)
                    this.pdfSection.Template.Bottom = AddPDFHeaderFooter((chart as ChartImpl).PageSetupBase, pdfSection, false, true);
                else
                    headerFooter.RemoveAt(0);
            }

            AllowHeaderFooterOnce = false;
           
            return this.pdfDocument;
        }            
            
        /// <summary>
        /// To draw the workbook chart in pdf
        /// </summary>
        /// <param name="chart">It's represent the chart object</param>
        /// <param name="chartTemplate">pdf template for the workbook chart</param>
        internal void DrawChart(IChart chart, PdfTemplate chartTemplate)
        {   
            
            RectangleF pictureRect = new RectangleF(0,0,Pdf_UnitConverter.ConvertFromPixels((float)chart.Width, PdfGraphicsUnit.Point),
                                                             Pdf_UnitConverter.ConvertFromPixels((float)chart.Height, PdfGraphicsUnit.Point));

            MemoryStream memoryStream = new MemoryStream();
            chart.SaveAsImage(memoryStream);
            if (memoryStream.Length > 0)
            {
                System.Drawing.Image cropped = System.Drawing.Image.FromStream(memoryStream);
                ImageFormat imageFormat = null;
                imageFormat = cropped.RawFormat;
                if (imageCodec.ContainsValue(imageFormat.Guid))
                {
                    memoryStream.Position = 0;
                    cropped.Save(memoryStream, ImageFormat.Png);
                }

                PdfBitmap bitMap = new PdfBitmap(memoryStream);
                chartTemplate.Graphics.DrawImage(bitMap, new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
            }
        }

#endif
        /// <summary>
        /// Converts the document with the specified converter settings.
        /// </summary>
        /// <param name="converterSettings">The converter settings.</param>
        /// <returns>pdfdcoument object</returns>
        public PdfDocument Convert(ExcelToPdfConverterSettings converterSettings)
        {
            this.pdfDocument = converterSettings.TemplateDocument;
            this.excelToPdfSettings = converterSettings;
            return this.Convert();
        }

        /// <summary>
        /// Called when [progress changed].
        /// </summary>
        /// <param name="noOfSheets">The no of sheets.</param>
        /// <param name="activeSheetIndex">Index of the active sheet.</param>
        internal void OnProgressChanged(int noOfSheets, int activeSheetIndex)
        {
            if (this.CurrentProgressChanged != null)
            {
                CurrentProgressChangedEventArgs args = new CurrentProgressChangedEventArgs(noOfSheets, activeSheetIndex, this);
                this.CurrentProgressChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SheetBeforeDrawn"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.ExcelToPdfConverter.SheetBeforeDrawnEventArgs"/> instance containing the event data.</param>
        internal void OnSheetBeforeDrawn(SheetBeforeDrawnEventArgs args)
        {
            if (this.SheetBeforeDrawn != null)
            {
                this.SheetBeforeDrawn(this, args);
            }
        }

        /// <summary>
        /// Called when [sheet after drawn].
        /// </summary>
        /// <param name="activeSheetIndex">Index of the active sheet.</param>
        internal void OnSheetAfterDrawn(int activeSheetIndex)
        {
            if (this.SheetAfterDrawn != null)
            {
                SheetAfterDrawnEventArgs args = new SheetAfterDrawnEventArgs(activeSheetIndex, this);
                this.SheetAfterDrawn(this, args);
            }
        }

        /// <summary>
        /// Raises the sheet before drawn.
        /// </summary>
        /// <param name="activeSheetIndex">Index of the active sheet.</param>
        /// <returns>the boolean values</returns>
        private bool RaiseSheetBeforeDrawn(int activeSheetIndex)
        {
            if (this.SheetBeforeDrawn != null)
            {
                SheetBeforeDrawnEventArgs args = new SheetBeforeDrawnEventArgs(activeSheetIndex, this);
                this.OnSheetBeforeDrawn(args);
                return args.Skip;
            }

            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (this.excelEngine != null)
                {
                    this.excelEngine.Dispose();
                    this.excelEngine = null;
                }
                fontCollection.Clear();
            }
        }

        #endregion

        #region Implementation Properties
        internal float SheetWidth
        {
            get
            {
                return sheetWidth;
            }
            set
            {
                sheetWidth = value;
            }
        }
        internal float SheetHeight
        {
            get
            {
                return sheetHeight;
            }
            set
            {
                sheetHeight = value;
            }
        }
        internal List<SplitText> SplitTexts
        {
            get
            {
                return splitTextCollection;
            }
        }
        internal bool IsNewPage
        {
            get
            {
                return isNewPage;
            }
            set
            {
                isNewPage = value;
            }
        }
        internal float AdjacentRectWidth
        {
            get
            {
                return adjacentRectWidth;
            }
        }
        internal float TopMargin
        {
            get
            {
                return topMargin;
            }
        }
        internal float BottomMargin
        {
            get
            {
                return bottomMargin;
            }
        }
        internal float HeaderMargin
        {
            get
            {
                return headerMargin;
            }
        }
        internal float FooterMargin
        {
            get
            {
                return footerMargin;
            }
        }
        internal bool HasHeader
        {
            get
            {
                return ((WorksheetImpl)this.workSheet).PageSetupBase.FullHeaderString != string.Empty;
            }
        }
        internal bool HasFooter
        {
            get
            {
                return ((WorksheetImpl)this.workSheet).PageSetupBase.FullFooterString != string.Empty;
            }
        }
        internal float LeftMargin
        {
            get
            {
                return leftMargin;
            }
        }
        internal float RightMargin
        {
            get
            {
                return rightMargin;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is print title row page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is print title row page; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPrintTitleRowPage
        {
            get
            {
                return m_bIsPrintTitleRowPage;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is print title column page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is print title column page; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPrintTitleColumnPage
        {
            get
            {
                return m_bIsPrintTitleColumnPage;
            }
        }

        /// <summary>
        /// Gets the row height getter.
        /// </summary>
        internal ItemSizeHelper RowHeightGetter
        {
            get
            {
                if (m_rowHeightGetter == null)
                    m_rowHeightGetter = new ItemSizeHelper(this.workSheet.GetRowHeightInPixels);

                return m_rowHeightGetter;
            }
        }

        /// <summary>
        /// Gets the column width getter.
        /// </summary>
        internal ItemSizeHelper ColumnWidthGetter
        {
            get
            {
                if (m_columnWidthGetter == null)
                    m_columnWidthGetter = new ItemSizeHelper(this.workSheet.GetColumnWidthInPixels);

                return m_columnWidthGetter;
            }
        }
        /// <summary>
        /// PDF Unit Converter.
        /// </summary>
        internal PdfUnitConvertor Pdf_UnitConverter
        {
            get
            {
                return m_pdfUnitConverter; 
            }            
        }
        internal IPageSetup SheetPageSetup
        {
            get
            {
                return m_pageSetup;
            }
            set
            {
                m_pageSetup = value;
            }
        }
        internal PageSetupOption ExcelToPdfPagesetup
        {
            get
            {
                return pageSetupOption;
            }
        }
        #endregion
        #region Draw Routines

        /// <summary>
        /// Draws the sheet.
        /// </summary>
        /// <param name="wkSheet">The work sheet.</param>
        /// <returns>pdfdocument object</returns>
        private PdfDocument DrawSheet(IWorksheet wkSheet, IRange[] printAreas)
        {

            WorksheetImpl sheetImpl = (WorksheetImpl)wkSheet;
            SheetPageSetup = wkSheet.PageSetup;
            this.pageCount = SheetPageSetup.FirstPageNumber;
            float widthValue = 0;
            float heightValue = 0;
            pdfTemplateCollection.Clear();
            PageSetupBaseImpl pageSetup = workSheet.PageSetup as PageSetupBaseImpl;

            leftMargin = Pdf_UnitConverter.ConvertUnits((float)SheetPageSetup.LeftMargin,
                                                               PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            rightMargin = Pdf_UnitConverter.ConvertUnits((float)SheetPageSetup.RightMargin,
                                                                PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            headerMargin = Pdf_UnitConverter.ConvertUnits((float)(SheetPageSetup.HeaderMargin),
                                                              PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            topMargin = Pdf_UnitConverter.ConvertUnits((float)(SheetPageSetup.TopMargin),
                                                              PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            bottomMargin = Pdf_UnitConverter.ConvertUnits((float)(SheetPageSetup.BottomMargin),
                                                                 PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            footerMargin = Pdf_UnitConverter.ConvertUnits((float)(SheetPageSetup.FooterMargin),
                                                                 PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);

            pdfSection = this.pdfDocument.Sections.Add();
            foreach (IRange range in printAreas)
            {
                int sheetFirstRow = 1;
                int sheetFirstColumn = 1;
                int sheetLastColumn;
                int sheetLastRow;

                IRange originalRange = range;
                // Find the sheet used range with the list object Range-Need to include in Xlsio.
                if (sheetImpl.ListObjects.Count != 0 && !(pageSetupOption.HasPrintArea || pageSetupOption.HasVerticalBreak || pageSetupOption.HasHorizontalBreak))
                {
                    originalRange=FindListObjectRange(sheetImpl, originalRange);
                }
                if (sheetImpl.Pictures.Count != 0 && !(pageSetupOption.HasPrintArea || pageSetupOption.HasVerticalBreak || pageSetupOption.HasHorizontalBreak))
                {
                    originalRange = UpdatePictureRange(sheetImpl, originalRange);
                }

                if (sheetImpl.Charts.Count != 0 && !(pageSetupOption.HasPrintArea || pageSetupOption.HasVerticalBreak || pageSetupOption.HasHorizontalBreak))
                {
                    originalRange = UpdateChartsRange(sheetImpl, originalRange);
                }

                sheetFirstRow = originalRange.Row;
                if (pageSetupOption.HasPrintTitleRows && printAreas[0].Row > pageSetupOption.PrintTitleFirstRow)
                {
                    sheetFirstRow = pageSetupOption.PrintTitleFirstRow;
                }
                sheetFirstColumn = originalRange.Column;
                if (pageSetupOption.HasPrintTitleColumns && printAreas[0].Column > pageSetupOption.PrintTitleFirstColumn)
                {
                    sheetFirstRow = pageSetupOption.PrintTitleFirstColumn;
                }
                sheetLastRow = originalRange.LastRow;
                sheetLastColumn = originalRange.LastColumn;

                if ((!pageSetupOption.HasVerticalBreak && !pageSetupOption.HasHorizontalBreak && !pageSetupOption.HasPrintArea))
                {
                    sheetFirstColumn = 1;
                    sheetFirstRow = 1;
                }
                 
                //get the Sheet width and height value
                widthValue = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetTotal(sheetFirstColumn, sheetLastColumn),
                                                                        PdfGraphicsUnit.Point);

                heightValue = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetTotal(sheetFirstRow, sheetLastRow),
                                                                        PdfGraphicsUnit.Point);

                int columnIndex = 0;
                int columnCount = 0;
                int rowCount = 0;
                int[] columns = new int[16384];
                int[] rows = new int[1048576];                
                int columnStartIndex = 1;
                int rowStartIndex = 1;
                sheetHeight = 0;
                sheetWidth = 0;
                float usedRangeHeight = heightValue;
                float usedRangeWidth = widthValue;

                // Pdf Margin Setup.
                pdfSection.PageSettings.Margins.Left = leftMargin > 0 ? leftMargin : LeftandRightMargin;
                pdfSection.PageSettings.Margins.Right = rightMargin > 0 ? rightMargin : LeftandRightMargin;
                pdfSection.PageSettings.Margins.Top = TopMargin > 0 ? TopMargin : TopandBottomMargin;
                pdfSection.PageSettings.Margins.Bottom = BottomMargin > 0 ? BottomMargin : TopandBottomMargin;

                if (!String.IsNullOrEmpty(sheetImpl.PageSetupBase.FullHeaderString)
                                        || !String.IsNullOrEmpty(sheetImpl.PageSetupBase.FullFooterString))
                {
                    this.headerFooter = this.DrawHeadersAndFooters(pdfSection, sheetImpl,null);
                }

                if (excelToPdfSettings.LayoutOptions == LayoutOptions.FitSheetOnOnePage)
                {
                    if (leftMargin == 0)
                        leftMargin = LeftandRightMargin;
                    if (rightMargin == 0)
                        rightMargin = LeftandRightMargin;
                    if (bottomMargin == 0)
                        bottomMargin = TopandBottomMargin;
                    if (topMargin == 0)
                        topMargin = TopandBottomMargin;
                }

                if (HasFooter)
                {
                    pdfSection.PageSettings.Margins.Bottom = 0;
                }
                if (HasHeader)
                {
                    pdfSection.PageSettings.Margins.Top = 0;
                }

                ExcelToPdfPageLayout = new ExcelToPdfLayoutSetting(this);
                #region switch statement
                switch (excelToPdfSettings.LayoutOptions)
                {

                    case LayoutOptions.FitSheetOnOnePage:                        
                        ExcelToPdfPageLayout.FitSheetOnPage(pdfSection, m_pageSetup,usedRangeWidth,usedRangeHeight);
                        rows[0] = sheetFirstRow;
                        rows[1] = sheetLastRow;
                        columns[0] = sheetFirstColumn;
                        columns[1] = sheetLastColumn;
                        rowCount = 1;
                        columnCount = 1;
                        break;
                    case LayoutOptions.FitAllColumnsOnOnePage:                        
                        ExcelToPdfPageLayout.FitAllColumnOnOnePage(pdfSection, SheetPageSetup, usedRangeWidth, usedRangeHeight);                        
                        columns[0] = sheetFirstColumn;
                        columns[1] = sheetLastColumn;
                        columnCount = 1;
                        //Find the Row breaks
                        rows = Helper.RowBreaker(sheetFirstRow, sheetLastRow, sheetHeight, RowHeightGetter, out rowStartIndex, excelToPdfSettings);
                        rows[rowStartIndex] = sheetLastRow;
                        rowCount = rowStartIndex;
                        break;

                    case LayoutOptions.FitAllRowsOnOnePage:                        
                            ExcelToPdfPageLayout.FitAllRowsOnOnePage(pdfSection, SheetPageSetup, usedRangeWidth, usedRangeHeight);
                            pdfPageTemplate = new PdfTemplate(SheetWidth, SheetHeight);
                            rows[0] = sheetFirstRow;
                            rows[1] = sheetLastRow;
                            rowCount = 1;

                            //Find the Column Breaks                
                            columns = Helper.ColumnBreaker(sheetFirstColumn, sheetLastColumn, sheetWidth, ColumnWidthGetter, out columnStartIndex, excelToPdfSettings);
                            columns[columnStartIndex] = sheetLastColumn;
                            columnCount = columnStartIndex;
                        break;
                    case LayoutOptions.NoScaling:
                        ExcelToPdfPageLayout.NoScaling(pdfSection, SheetPageSetup, usedRangeWidth, usedRangeHeight);
                        //Find the Row breaks
                        rows = Helper.RowBreaker(sheetFirstRow, sheetLastRow, sheetHeight, RowHeightGetter, out rowStartIndex, excelToPdfSettings);
                        rows[rowStartIndex] = sheetLastRow;
                        rowCount = rowStartIndex;

                        //Find the Column Breaks                
                        columns = Helper.ColumnBreaker(sheetFirstColumn, sheetLastColumn, sheetWidth, ColumnWidthGetter, out columnStartIndex, excelToPdfSettings);
                        columns[columnStartIndex] = sheetLastColumn;
                        columnCount = columnStartIndex;
                        break;
                    case LayoutOptions.CustomScaling:                        
                        ExcelToPdfPageLayout.CustomScaling(pdfSection, SheetPageSetup, usedRangeWidth, usedRangeHeight, printAreas);
                        //Find the Row breaks
                        rows = Helper.RowBreaker(sheetFirstRow, sheetLastRow, sheetHeight, RowHeightGetter, out rowStartIndex, excelToPdfSettings);
                        rows[rowStartIndex] = sheetLastRow;
                        rowCount = rowStartIndex;

                        //Find the Column Breaks                
                        columns = Helper.ColumnBreaker(sheetFirstColumn, sheetLastColumn, sheetWidth, ColumnWidthGetter, out columnStartIndex, excelToPdfSettings);
                        columns[columnStartIndex] = sheetLastColumn;
                        columnCount = columnStartIndex;
                        break;
                }
                #endregion

                //create the pdf template 
                pdfPageTemplate = new PdfTemplate(SheetWidth, SheetHeight);

                if (excelToPdfSettings.LayoutOptions != LayoutOptions.NoScaling && excelToPdfSettings.LayoutOptions != LayoutOptions.CustomScaling)
                {
                    if (pdfPageTemplate.Width < pdfSection.PageSettings.Width && pdfPageTemplate.Height < pdfSection.PageSettings.Height)
                    {
                        pdfPageTemplate = new PdfTemplate(pdfSection.PageSettings.Width, pdfSection.PageSettings.Height);
                    }
                }

                int firstColumn = columns[columnIndex] == 0 ? 1 : columns[columnIndex];                
                columnIndex++;
                int lastColumn = columns[columnIndex];
                float positionX = 0;
                int pageIndex = 1;
                #region Down Then Over
                if (SheetPageSetup.Order == ExcelOrder.DownThenOver)
                {
                    for (int column = 0, count = 0; column < columnCount; column++)
                    {
                        int rowIndex = 0;
                        int firstRow = rows[rowIndex];
                        rowIndex++;
                        int lastRow = rows[rowIndex];
                        float positionY = 0;
                        for (int row = 0; row < rowCount; row++)
                        {
                            if (Array.IndexOf(pageSetupOption.RowIndexes.ToArray(), rowIndex) >= 0)
                                m_bIsPrintTitleRowPage = m_bHasPrintTitleRow = true;

                            if (Array.IndexOf(pageSetupOption.ColumnIndexes.ToArray(), columnIndex) >= 0)
                                m_bIsPrintTitleColumnPage = m_bHasPrintTitleColumn = true;
                            float totalRowHeight=RowHeightGetter.GetTotal(firstRow, lastRow);
                            float totalColumnWidth = ColumnWidthGetter.GetTotal(firstColumn, lastColumn);
                            bool isBlank = true;
                            if (firstColumn <= lastColumn)
                            {
                                RangeImpl rangeImpl=sheetImpl.Range[firstRow, firstColumn, lastRow, lastColumn] as RangeImpl;
                                if (!rangeImpl.IsBlankorHasStyle || CheckMergedRegion(sheetImpl.Range[firstRow, firstColumn, lastRow, lastColumn]))
                                {
                                    isBlank = false;
                                }
                                if (isBlank)
                                {
                                    isBlank = !HasPicture(rangeImpl, workSheet);
                                }
                                if (!isBlank)
                                {
                                    if (this.excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling)
                                     {
                                        if (!pageSetup.IsFitToPage)
                                        {
                                            float zoomValue = workSheet.PageSetup.Zoom / 100.0f;
                                            this.pdfPageTemplate = new PdfTemplate((this.pdfSection.PageSettings.Width - leftMargin) / zoomValue, (this.pdfSection.PageSettings.Height + (topMargin)) / zoomValue);
                                        }
                                        else
                                            this.pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                                    }
                                    else if (excelToPdfSettings.LayoutOptions != LayoutOptions.FitSheetOnOnePage)
                                    {
                                       pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                                    }
                                    if (excelToPdfSettings.LayoutOptions != LayoutOptions.NoScaling && excelToPdfSettings.LayoutOptions != LayoutOptions.FitSheetOnOnePage && excelToPdfSettings.LayoutOptions != LayoutOptions.CustomScaling)
                                    {
                                        if (IsPrintTitleRowPage)
                                        {
                                            if (sheetHeight > (totalRowHeight +(RowHeightGetter.GetTotal(pageSetupOption.PrintTitleFirstRow, pageSetupOption.
                                                PrintTitleLastRow))))
                                            {
                                                sheetHeight = totalRowHeight;
                                                sheetHeight += pageSetupOption.TitleRowHeight;
                                            }
                                        }
                                        else if (sheetHeight > totalRowHeight)
                                        {
                                            sheetHeight = totalRowHeight;
                                        }
                                        if (IsPrintTitleColumnPage)
                                        {
                                            if (sheetWidth > (totalColumnWidth + (ColumnWidthGetter.GetTotal(pageSetupOption.PrintTitleFirstColumn, pageSetupOption.
                                                                         PrintTitleLastColumn))) && excelToPdfSettings.LayoutOptions != LayoutOptions.FitAllColumnsOnOnePage)
                                            {
                                                sheetWidth = totalColumnWidth;
                                                sheetWidth += pageSetupOption.TitleColumnWidth;
                                            }
                                        }
                                        else if (sheetWidth > totalColumnWidth)
                                        {
                                            sheetWidth = totalColumnWidth;
                                        }
                                        pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                                    }

                                    pdfTemplateCollection.Add(pdfPageTemplate);

                                    this.InitializePdfPage();

                                    FindPageSettings(pdfPageTemplate, this.currentPage, wkSheet, ref startX, ref headerHeight, widthValue);

                                    this.pdfDocument = this.DrawRow(sheetImpl, firstColumn, lastColumn, firstRow, lastRow,
                                                                    positionX, positionY, sheetWidth);
                                    if (excelToPdfSettings.ExportBookmarks)
                                    {
                                        this.AddBookMark();
                                        excelToPdfSettings.ExportBookmarks = false;
                                        this.bookmark = true;
                                    }
                                    count++;
                                }
                            }

                            rowIndex++;
                            firstRow = lastRow + 1;
                            lastRow = rows[rowIndex];
                            if (!isBlank)
                                positionY += sheetHeight;
                            pageIndex++;
                            startX = 0;
                            headerHeight = 0;
                        }
                        positionX += sheetWidth;
                        columnIndex++;
                        firstColumn = lastColumn + 1;
                        lastColumn = columns[columnIndex];
                    }
                }
                #endregion

                #region Over Then Down
                else if (SheetPageSetup.Order == ExcelOrder.OverThenDown)
                {
                    int rowIndex = 0;
                    int firstRow = rows[rowIndex];
                    rowIndex++;
                    int lastRow = rows[rowIndex];
                    for (int row = 0, count = 0; row < rowCount; row++)
                    {
                        columnIndex = 0;
                        firstColumn = columns[columnIndex];
                        columnIndex++;
                        lastColumn = columns[columnIndex];
                        float positionY = 0;
                        bool isBlank = true;
                        for (int column = 0; column < columnCount; column++)
                        {

                            if (Array.IndexOf(pageSetupOption.RowIndexes.ToArray(), rowIndex) >= 0)
                                m_bIsPrintTitleRowPage = m_bHasPrintTitleRow = true;

                            if (Array.IndexOf(pageSetupOption.ColumnIndexes.ToArray(), columnIndex) >= 0)
                                m_bIsPrintTitleColumnPage = m_bHasPrintTitleColumn = true;


                            if (firstColumn <= lastColumn)
                            {
                                RangeImpl rangeImpl = sheetImpl.Range[firstRow, firstColumn, lastRow, lastColumn] as RangeImpl;
                                if (!rangeImpl.IsBlankorHasStyle || CheckMergedRegion(sheetImpl.Range[firstRow, firstColumn, lastRow, lastColumn]) || IsPrintTitleRowPage || IsPrintTitleColumnPage)
                                {
                                    isBlank = false;
                                }
                                if (isBlank)
                                {
                                    isBlank = !HasPicture(rangeImpl, workSheet);
                                }
                                if (!isBlank)
                                {
                                    if (this.excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling)
                                    {
                                        if (!pageSetup.IsFitToPage)
                                        {
                                            float zoomValue = workSheet.PageSetup.Zoom / 100.0f;
                                            this.pdfPageTemplate = new PdfTemplate((this.pdfSection.PageSettings.Width - leftMargin) / zoomValue, (this.pdfSection.PageSettings.Height + (topMargin)) / zoomValue);
                                        }
                                        else
                                            this.pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                                    }
                                    else if (excelToPdfSettings.LayoutOptions != LayoutOptions.FitSheetOnOnePage)
                                    {
                                        pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                                    }
                                    if (excelToPdfSettings.LayoutOptions != LayoutOptions.NoScaling && excelToPdfSettings.LayoutOptions != LayoutOptions.FitSheetOnOnePage && excelToPdfSettings.LayoutOptions != LayoutOptions.CustomScaling)
                                    {
                                        if (IsPrintTitleRowPage)
                                        {
                                            if (sheetHeight > (RowHeightGetter.GetTotal(firstRow, lastRow) + (RowHeightGetter.GetTotal(pageSetupOption.PrintTitleFirstRow, pageSetupOption.PrintTitleLastRow))))
                                            {
                                                sheetHeight = RowHeightGetter.GetTotal(firstRow, lastRow);
                                                sheetHeight += pageSetupOption.TitleRowHeight;
                                            }
                                        }
                                        else if (sheetHeight > RowHeightGetter.GetTotal(firstRow, lastRow))
                                        {
                                            sheetHeight = RowHeightGetter.GetTotal(firstRow, lastRow);
                                        }
                                        if (IsPrintTitleColumnPage)
                                        {
                                            if (sheetWidth > (ColumnWidthGetter.GetTotal(firstColumn, lastColumn) + (ColumnWidthGetter.GetTotal(pageSetupOption.PrintTitleFirstColumn, pageSetupOption.PrintTitleLastColumn))))
                                            {
                                                sheetWidth = ColumnWidthGetter.GetTotal(firstColumn, lastColumn);
                                                sheetWidth += pageSetupOption.TitleColumnWidth;
                                            }
                                        }
                                        else if (sheetWidth > ColumnWidthGetter.GetTotal(firstColumn, lastColumn))
                                        {
                                            sheetWidth = ColumnWidthGetter.GetTotal(firstColumn, lastColumn);
                                        }
                                        pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                                    }

                                    pdfTemplateCollection.Add(pdfPageTemplate);
                                    

                                    this.InitializePdfPage();

                                    FindPageSettings(pdfPageTemplate, this.currentPage, wkSheet, ref startX, ref headerHeight, widthValue);

                                    this.pdfDocument = this.DrawRow(sheetImpl, firstColumn, lastColumn, firstRow, lastRow,
                                                                    positionX, positionY, sheetWidth);
                                    if (excelToPdfSettings.ExportBookmarks)
                                    {
                                        this.AddBookMark();
                                        excelToPdfSettings.ExportBookmarks = false;
                                        this.bookmark = true;
                                    }

                                    count++;
                                }
                            }

                            positionX += sheetWidth;
                            columnIndex++;
                            firstColumn = lastColumn + 1;
                            lastColumn = columns[columnIndex];
                            isBlank = true;
                        }

                        rowIndex++;
                        firstRow = lastRow + 1;
                        lastRow = rows[rowIndex];
                        if (!isBlank)
                            positionY += sheetHeight;
                        pageIndex++;
                    }
                }
                #endregion
                if (this.SplitTexts.Count != 0 && excelToPdfSettings.LayoutOptions == LayoutOptions.NoScaling && m_fitText && !workSheet.IsRightToLeft)
                {
                    pdfPageTemplate = new PdfTemplate(sheetWidth, sheetHeight);
                    pdfTemplateCollection.Add(pdfPageTemplate);
                    if (!String.IsNullOrEmpty(sheetImpl.PageSetupBase.FullHeaderString)
                                    || !String.IsNullOrEmpty(sheetImpl.PageSetupBase.FullFooterString))
                    {
                        this.headerFooter = this.DrawHeadersAndFooters(pdfSection, sheetImpl,null);
                    }
                    this.InitializePdfPage();
                    this.DrawSplitText();
                }
            }
            
            if ((pdfSection.Pages.Count != 0) && (pdfTemplateCollection.Count != 0))
            {
                float headerHeight = 0;
               
                for (int i = 0; i < pdfSection.Pages.Count; i++)
                {
                    float startX = 0;
                    PdfTemplate pdfTemplate = pdfTemplateCollection[i];
                    FindPageSettings(pdfTemplate, pdfSection.Pages[i], wkSheet, ref startX, ref headerHeight, widthValue);                    
                    pdfSection.Pages[i].Graphics.DrawPdfTemplate(pdfTemplate, new PointF(startX, headerHeight),
                                                                      new SizeF(scaledPageWidth, this.scaledPageHeight));
                    headerHeight = 0;
                }
            }

            //Use the Pdf HeaderFooter Feature.
            if (AllowHeader && !string.IsNullOrEmpty(sheetImpl.PageSetupBase.FullHeaderString) && headerFooter.Count > 0)
            {
                if (this.excelToPdfSettings.HeaderFooterOption.ShowHeader)
                    this.pdfSection.Template.Top = AddPDFHeaderFooter(sheetImpl.PageSetupBase, pdfSection, true, false);
                else
                    headerFooter.RemoveAt(0);
            }
            if (AllowFooter && !string.IsNullOrEmpty(sheetImpl.PageSetupBase.FullFooterString) && headerFooter.Count > 0)
            {
                if (this.excelToPdfSettings.HeaderFooterOption.ShowFooter)
                    this.pdfSection.Template.Bottom = AddPDFHeaderFooter(sheetImpl.PageSetupBase, pdfSection, false, false);
                else
                    headerFooter.RemoveAt(0);
            }

            AllowHeaderFooterOnce = false;
            m_bIsPrintTitleRowPage = false;
            m_bIsPrintTitleColumnPage = false;
            return this.pdfDocument;
        }
        /// <summary>
        /// It's finding the scaled page width and height
        /// </summary>
        /// <param name="pdfTemplate"></param>
        /// <param name="page"></param>
        /// <param name="wkSheet"></param>
        /// <param name="startX"></param>
        /// <param name="headerHeight"></param>
        /// <param name="widthValue"></param>
        private void FindPageSettings(PdfTemplate pdfTemplate, PdfPage page, IWorksheet wkSheet, ref float startX, ref float headerHeight,float widthValue)
        {
            float footerHeight = 0;
            PageSetupBaseImpl pageSetup = workSheet.PageSetup as PageSetupBaseImpl;
            float[] centerMargin = GetMarginForCenterAlignment(pageSetup.IsFitToPage);
            if (this.headerFooter.Count != 0)
            {
#if SyncfusionFramework4_0 || SyncfusionFramework4_5
                DrawPdfPageHeaderFooter(pdfSection, this.headerFooter, (wkSheet as WorksheetImpl).PageSetupBase, topMargin, bottomMargin,
                                             page, out headerHeight, out footerHeight);
#else
                        DrawPdfPageHeaderFooter(pdfSection, this.headerFooter, wkSheet, topMargin, bottomMargin,
                                                     page, out headerHeight, out footerHeight);
#endif
            }

            if (excelToPdfSettings.LayoutOptions != LayoutOptions.NoScaling && excelToPdfSettings.LayoutOptions != LayoutOptions.CustomScaling)
            {
                if (SheetPageSetup.RightMargin == 0)
                {
                    GetScaledPage(pdfTemplate.Width, pdfTemplate.Height,
                                       page.Size.Width - pdfSection.PageSettings.Margins.Left,
                                       page.Size.Height - (headerHeight + footerHeight), true);
                }
                else
                {
                    double originalWidth = page.Size.Width;

                    if (pdfTemplate.Width > originalWidth)
                        originalWidth -= (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right);

                    if (HasFooter && HasHeader)
                        GetScaledPage(pdfTemplate.Width, pdfTemplate.Height,
                                  (float)Math.Ceiling(originalWidth),
                                   page.Size.Height - (footerHeight + headerHeight), true);
                    else if (HasFooter)
                        GetScaledPage(pdfTemplate.Width, pdfTemplate.Height,
                                          (float)Math.Ceiling(originalWidth),
                                           page.Size.Height - (footerHeight + headerHeight + topMargin), true);
                    else if (HasHeader)
                        GetScaledPage(pdfTemplate.Width, pdfTemplate.Height,
                                      (float)Math.Ceiling(originalWidth),
                                       page.Size.Height - (footerHeight + headerHeight + bottomMargin), true);
                    else
                        GetScaledPage(pdfTemplate.Width, pdfTemplate.Height, (float)Math.Ceiling(originalWidth),
                                           page.Size.Height - (topMargin + bottomMargin), true);

                }
            }
            else if (excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling)
            {
                if (!pageSetup.IsFitToPage)
                {
                    if (this.workSheet.PageSetup.CenterHorizontally)
                    {
                        this.scaledPageWidth = page.Size.Width - (centerMargin[0] + centerMargin[1]);
                        this.scaledPageHeight = page.Size.Height;
                    }
                    else
                    {
                        this.scaledPageWidth = page.Size.Width;
                        this.scaledPageHeight = page.Size.Height;
                    }
                }
                else
                {
                    if (SheetPageSetup.Zoom <= 50)
                    {
                        GetScaledPage(pdfTemplate.Width, pdfTemplate.Height, (page.Size.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)), (page.Size.Height - (pdfSection.PageSettings.Margins.Top + pdfSection.PageSettings.Margins.Bottom)), true);
                    }
                    else
                    {
                        GetScaledPage(pdfTemplate.Width, pdfTemplate.Height, (page.Size.Width + leftMargin + rightMargin), (page.Size.Height - (bottomMargin + topMargin)), true);
                    }
                }
            }
            else
            {
                GetScaledPage(pdfTemplate.Width, pdfTemplate.Height, sheetWidth,
                                   sheetHeight - (footerHeight + headerHeight), false);
            }

            if (SheetPageSetup.CenterHorizontally)
            {
                float width = pdfSection.PageSettings.Width;
                width -= (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right);
                if (scaledPageWidth < width)
                {
                    startX += (width - scaledPageWidth) / 2;
                    if (scaledPageWidth > widthValue)
                    {
                        startX += (scaledPageWidth - widthValue) / 2;
                    }
                }
                else if (widthValue < scaledPageWidth)
                {
                    float interScaledWidth = scaledPageWidth;
                    if (interScaledWidth == pdfSection.PageSettings.Width)
                        interScaledWidth = width;
                    startX += (interScaledWidth - widthValue) / 2;
                }

                if (this.excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling && (!pageSetup.IsFitToPage))
                    startX = centerMargin[0];
            }

            if (SheetPageSetup.CenterVertically)
            {
                if (HasFooter)
                {
                    headerHeight += ((pdfSection.PageSettings.Height - this.scaledPageHeight) - (footerHeight + headerHeight)) / 2;
                }
                else
                {
                    headerHeight += (pdfSection.PageSettings.Height - this.scaledPageHeight) / 2;
                }

                if (this.excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling && (!pageSetup.IsFitToPage))
                    headerHeight = topMargin;
            }
            if (HasHeader)
            {
                if (this.excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling && (!pageSetup.IsFitToPage))
                {
                    headerHeight = topMargin;
                }
                else
                {
                    if (headerHeight == 0)
                    {
                        headerHeight = topMargin;
                    }
                    else if (headerHeight < topMargin)
                    {
                        headerHeight = topMargin + headerMargin;
                    }
                }
            }
        }
        /// <summary>
        /// Get margins for center alignment.
        /// </summary>
        private float[] GetMarginForCenterAlignment(bool isFitToPage)
        {
            //left,right,top and bottom margins respectively.
            float[] margins = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
            float leftMar = 0.0f;
            float rightMar = 0.0f;
            float topMar = 0.0f;
            float botMar = 0.0f;

            double[] arrayValue = Helper.CalculateFontValue(excelToPdfSettings);

            if (excelToPdfSettings.LayoutOptions == LayoutOptions.CustomScaling && (!isFitToPage))
            {
                float pageWidthInInches = this.pdfSection.PageSettings.Width / 72;
                pageWidthInInches *= 2.54f;
                float colTitleWidth = pageSetupOption.TitleColumnWidth;
                colTitleWidth = ((colTitleWidth * (float)arrayValue[0]) / 72.0f) * 2.54f;

                if (this.workSheet.PageSetup.CenterHorizontally)
                {
                    leftMar = (pageWidthInInches - colTitleWidth) / 2.0f;
                    leftMar = (float)Math.Round(leftMar, 2);

                    rightMar = (pageWidthInInches - colTitleWidth) / 2.0f;
                    rightMar = (float)Math.Round(rightMar, 2);
                }
                else
                {
                    leftMar = leftMargin;
                    rightMar = rightMargin;
                }

                if (leftMar < 0.0)
                    leftMar = 0.0f;
                if (rightMar < 0.0)
                    rightMar = 0.0f;

                float pageHeightInInches = this.pdfSection.PageSettings.Height / 72;
                pageHeightInInches *= 2.54f;
                float rowTitleHeight = pageSetupOption.TitleRowHeight;
                rowTitleHeight = ((rowTitleHeight * (float)arrayValue[1]) / 72.0f) * 2.54f;

                if (this.workSheet.PageSetup.CenterVertically)
                {
                    topMar = (pageWidthInInches - rowTitleHeight) / 2.0f;
                    if (topMar > 0.0)
                        topMar = (float)Math.Round(topMar, 2);

                    botMar = (pageWidthInInches - rowTitleHeight) / 2.0f;
                    if (botMar > 0.0)
                        botMar = (float)Math.Round(botMar, 2);
                }
                else
                {
                    topMar = topMargin;
                    botMar = bottomMargin;
                }

                if (topMar < 0.0)
                    topMar = 0.0f;
                if (botMar < 0.0)
                    botMar = 0.0f;

                margins[0] = leftMar;
                margins[1] = rightMar;
                margins[2] = topMar;
                margins[3] = botMar;
            }

            return margins;
        }

        /// <summary>
        /// Draws the split text.
        /// </summary>
        private void DrawSplitText()
        {
            for (int i = 0; i < this.SplitTexts.Count; i++)
            {
                SplitText text = SplitTexts[i];
                RectangleF rect = text.OriginRect;
                if (text.OriginRect.X != 0)
                {
                    rect.X = 0;
                }
                float textWidth = text.TextFont.MeasureString(text.Text).Width;
                if (rect.Width < textWidth)
                {
                    rect.Width = textWidth;
                }
                pdfPageTemplate.Graphics.DrawString(text.Text, text.TextFont, text.Brush, rect);
            }
        }
        /// <summary>
        /// Draws the split text.
        /// </summary>
        /// <param name="splitText">The split text.</param>
        private void DrawSplitText(SplitText splitText)
        {
            SplitText text = splitText;
            RectangleF adjacentRect = text.OriginRect;
            PdfStringFormat pdfTextformat = splitText.Format;
            PdfFont pdfFont = splitText.TextFont;
            PdfBrush pdfBrush = text.Brush;
            float cellTextWidth = pdfFont.MeasureString(splitText.Text, pdfTextformat).Width;
            string cellText = splitText.Text;
            if (pdfTextformat.RightToLeft)
            {
                bool enableCalculation = true;

                if (pdfTextformat.Alignment == PdfTextAlignment.Right)
                {
                    pdfTextformat.Alignment = PdfTextAlignment.Left;
                    enableCalculation = true;
                }
                if (enableCalculation)
                {
                    if (cellTextWidth < adjacentRect.Width)
                    {
                        adjacentRect.X += (adjacentRect.Width - cellTextWidth);
                    }
                    else if (adjacentRect.X > 0)
                        adjacentRect.X += (cellTextWidth - adjacentRect.Width);

                    pdfPageTemplate.Graphics.DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat);
                }
                else
                {
                    pdfPageTemplate.Graphics.DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat);
                }
            }
        }

        /// <summary>
        /// Draws the back ground image of Excel sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="templateSize">Size of the template.</param>
        /// <param name="page">The pdf page.</param>*/
        /*private void DrawBackgroundImage(WorksheetImpl sheet, SizeF templateSize, PdfPage page)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException("sheet");
            }

            Dictionary<PointF, SizeF> imageCoordinates = new Dictionary<PointF, SizeF>();
            imageCoordinates = this.GetBackgroundWidthCoordinates(0, 0,
                                                                 (float)sheet.PageSetupBase.BackgoundImage.Width,
                                                                 (float)sheet.PageSetupBase.BackgoundImage.Height,
                                                                  imageCoordinates, page);
            if (imageCoordinates.Count != 0)
            {
                foreach (KeyValuePair<PointF, SizeF> coordinate in imageCoordinates)
                {
                    page.Graphics.DrawImage(new PdfBitmap(sheet.PageSetupBase.BackgoundImage), coordinate.Key,
                                            coordinate.Value);
                }
            }
            else
            {
                page.Graphics.DrawImage(new PdfBitmap(sheet.PageSetupBase.BackgoundImage), 0, 0,
                                        templateSize.Width, templateSize.Height);
            }
        }*/

#if SyncfusionFramework4_0 || SyncfusionFramework4_5

        /// <summary>
        /// Draws the Header and Footer to the PDF page .
        /// </summary>
        /// <param name="section">The Pdf section.</param>
        /// <param name="headerFooterCollection">The header footer collection.</param>
        /// <param name="pageSetupBase">The Worksheet.</param>
        /// <param name="page">The Pdf page.</param>
        /// <param name="headerHeight">Height of the header.</param>
        /// <param name="footerHeight">Height of the footer.</param>
        private void DrawPdfPageHeaderFooter(PdfSection section, List<HeaderFooter> headerFooterCollection, IPageSetupBase pageSetupBase, float topmargin, float bottomMargin,
                                   PdfPage page, out float headerHeight, out float footerHeight)
        {
            headerHeight = 0;
            footerHeight = 0;
            float excelHeaderHeight = Pdf_UnitConverter.ConvertUnits((float)pageSetupBase.TopMargin, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            int pageCount = this.pageCount++;
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            bool isHeader = this.excelToPdfSettings.HeaderFooterOption.ShowHeader;
            bool isFooter = this.excelToPdfSettings.HeaderFooterOption.ShowFooter;
            foreach (HeaderFooter headerFooter in headerFooterCollection)
            {
                if (headerFooter.HeaderFooterName == "Header" && isHeader && !AllowHeader)
                {

                    float height = this.GetMaxHeight(headerFooter.HeaderFooterSections) > excelHeaderHeight
                                                               ? this.GetMaxHeight(headerFooter.HeaderFooterSections)
                                                               : excelHeaderHeight;

                    if (height < topmargin)
                        height = topmargin;

                    PdfTemplate template = new PdfTemplate(headerFooter.TemplateSize.Width, height);
                    foreach (HeaderFooterSection headerFooterSection in headerFooter.HeaderFooterSections)
                    {
                        this.GetHeaderFooterInformation(headerFooterCollection, section, pageSetupBase,
                                                  headerFooterSection.HeaderFooterCollections, template,
                                                  headerFooterSection.Width, 0, headerFooterSection.SectionName,
                                                  headerFooter.HeaderFooterName, System.Convert.ToString(pageCount, formatProvider));
                    }
                    page.Graphics.DrawPdfTemplate(template, new PointF(0, HeaderMargin / 2));
                    headerHeight = template.Height;
                }
                else if (headerFooter.HeaderFooterName == "Footer" && isFooter && !AllowFooter)
                {
                    float height = this.GetMaxHeight(headerFooter.HeaderFooterSections) > 50
                                                               ? 50
                                                               : this.GetMaxHeight(headerFooter.HeaderFooterSections);

                    if (height < bottomMargin)
                        height = bottomMargin;

                    PdfTemplate template = new PdfTemplate(headerFooter.TemplateSize.Width, height);
                    foreach (HeaderFooterSection headerFooterSection in headerFooter.HeaderFooterSections)
                    {
                        this.GetHeaderFooterInformation(headerFooterCollection, section, pageSetupBase,
                                                  headerFooterSection.HeaderFooterCollections, template,
                                                  headerFooterSection.Width, height, headerFooterSection.SectionName,
                                                  headerFooter.HeaderFooterName, System.Convert.ToString(pageCount, formatProvider));
                    }
                    page.Graphics.DrawPdfTemplate(template, new PointF(0, (page.Size.Height) - (bottomMargin + footerMargin)));
                    footerHeight = template.Height;
                }
            }
        }
#else

        /// <summary>
        /// Draws the Header and Footer to the PDF page .
        /// </summary>
        /// <param name="section">The Pdf section.</param>
        /// <param name="headerFooterCollection">The header footer collection.</param>
        /// <param name="sheet">The Worksheet.</param>
        /// <param name="page">The Pdf page.</param>
        /// <param name="headerHeight">Height of the header.</param>
        /// <param name="footerHeight">Height of the footer.</param>
        private void DrawPdfPageHeaderFooter(PdfSection section, List<HeaderFooter> headerFooterCollection, IWorksheet sheet, float topmargin, float bottomMargin,
                                   PdfPage page, out float headerHeight, out float footerHeight)
        {
            headerHeight = 0;
            footerHeight = 0;
            float excelHeaderHeight = Pdf_UnitConverter.ConvertUnits((float)this.workSheet.PageSetup.TopMargin, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            int pageCount = this.pageCount++;
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            bool isHeader = this.excelToPdfSettings.HeaderFooterOption.ShowHeader;
            bool isFooter = this.excelToPdfSettings.HeaderFooterOption.ShowFooter;
            foreach (HeaderFooter headerFooter in headerFooterCollection)
            {
                float resizeWidth = (headerFooter.TemplateSize.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)) / 3;
                if (headerFooter.HeaderFooterName == "Header" && isHeader && !AllowHeader)
                {

                    float height = this.GetMaxHeight(headerFooter.HeaderFooterSections) > excelHeaderHeight
                                                               ? this.GetMaxHeight(headerFooter.HeaderFooterSections)
                                                               : excelHeaderHeight;
                    float actualHeight = height;
                    if (height < topmargin)
                        height = topmargin;

                    PdfTemplate template = new PdfTemplate(headerFooter.TemplateSize.Width, height);
                    
                    Dictionary<string, PdfTemplate> templates = new Dictionary<string, PdfTemplate>();
                    List<float> scaledHeights = new List<float>();
                    foreach (HeaderFooterSection headerFooterSection in headerFooter.HeaderFooterSections)
                    {
                     this.GetHeaderFooterInformation(headerFooterCollection, section, sheet,
                                                  headerFooterSection.HeaderFooterCollections, template,
                                                  headerFooterSection.Width, 0, headerFooterSection.SectionName,
                                                  headerFooter.HeaderFooterName, System.Convert.ToString(pageCount, formatProvider),headerFooterSection,templates);
                    }
                    foreach (KeyValuePair<string, PdfTemplate> templ in templates)
                    {

                        if (templ.Value.Width > this.pdfSection.PageSettings.Width)
                        {
                         //   this.GetScaledHFPage(templ.Value.Width, templ.Value.Height, GetSortedWidth(templ.Key, templates, resizeWidth), topmargin, true);
                            this.scaledPageHFHeight = templ.Value.Height/3;
                            this.scaledPageHFWidth = templ.Value.Width/3;
                        }
                        else
                        {
                            this.scaledPageHFHeight = templ.Value.Height;
                            this.scaledPageHFWidth = templ.Value.Width;
                        }                        
                        
                        float x = 0;
                        if (templ.Key == "Center")
                            x = ((template.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)) - scaledPageHFWidth)/2;
                        else if (templ.Key == "Right")
                        {
                            float maginWidth=pdfSection.PageSettings.Margins.Left+pdfSection.PageSettings.Margins.Right;
                            x = pdfSection.PageSettings.Width - maginWidth - scaledPageHFWidth;
                        }
                        template.Graphics.DrawPdfTemplate(templ.Value, new PointF(x , HeaderMargin), new SizeF(this.scaledPageHFWidth, this.scaledPageHFHeight));                       
                    }
                    page.Graphics.DrawPdfTemplate(template, new PointF(0, 0));
                    headerHeight = (topmargin !=0) ? topmargin : 5;
                }
                else if (headerFooter.HeaderFooterName == "Footer" && isFooter && !AllowFooter)
                {
                     this.scaledPageHFHeight = 0;
                    this.scaledPageHFWidth = 0;
                    float height = this.GetMaxHeight(headerFooter.HeaderFooterSections) > 50
                                                               ? 50
                                                               : this.GetMaxHeight(headerFooter.HeaderFooterSections);

                    if (height < bottomMargin && footerMargin != bottomMargin)
                        height = bottomMargin-footerMargin;

                    PdfTemplate template = new PdfTemplate(headerFooter.TemplateSize.Width, height);
                    List<float> scaledHeights = new List<float>();
                    Dictionary<string, PdfTemplate> templates = new Dictionary<string, PdfTemplate>();
                    foreach (HeaderFooterSection headerFooterSection in headerFooter.HeaderFooterSections)
                    {
                        this.GetHeaderFooterInformation(headerFooterCollection, section, sheet,
                                                  headerFooterSection.HeaderFooterCollections, template,
                                                  headerFooterSection.Width, 0, headerFooterSection.SectionName,
                                                  headerFooter.HeaderFooterName, System.Convert.ToString(pageCount, formatProvider),headerFooterSection,templates);
                    }

                    foreach (KeyValuePair<string, PdfTemplate> templ in templates)
                    {
                        if (templ.Value.Width > this.pdfSection.PageSettings.Width)
                        {
                            //   this.GetScaledHFPage(templ.Value.Width, templ.Value.Height, GetSortedWidth(templ.Key, templates, resizeWidth), topmargin, true);                            
                            this.scaledPageHFHeight = templ.Value.Height/3 ;
                            this.scaledPageHFWidth = templ.Value.Width / 3;
                        }
                        else
                        {
                            this.scaledPageHFHeight = templ.Value.Height;
                            this.scaledPageHFWidth = templ.Value.Width;
                        }
                        
                        float x = 0;
                        float y = 0;
                        float marginHeight = pdfSection.PageSettings.Margins.Top + pdfSection.PageSettings.Margins.Bottom;
                        float maxHeight = scaledPageHFHeight > MaxFooterWidth ? MaxFooterWidth : scaledPageHFHeight;
                        if (templ.Key == "Center")
                        {
                            x = ((template.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)) - scaledPageHFWidth) / 2;
                            y = pdfSection.PageSettings.Height - marginHeight - maxHeight;
                        }
                        else if (templ.Key == "Right")
                        {
                            float maginWidth = pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right;
                            x = pdfSection.PageSettings.Width - maginWidth - scaledPageHFWidth;
                            y = pdfSection.PageSettings.Height - marginHeight - maxHeight;
                        }
                        else
                            y = pdfSection.PageSettings.Height - marginHeight - maxHeight;

                        scaledHeights.Add(scaledPageHFHeight);
                        page.Graphics.DrawPdfTemplate(templ.Value, new PointF(x,y),new SizeF(scaledPageHFWidth,scaledPageHFHeight));
                        footerHeight = (float)sheet.PageSetup.FooterMargin;
                    }
                }
            }
        }


#endif
        /// <summary>
        /// Draws the row.
        /// </summary>
        /// <param name="wkSheet">The wk sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        /// <param name="originalWidth">Width of the original.</param>
        /// <returns>The Object of Pdf Document.</returns>
        private PdfDocument DrawRow(IWorksheet wkSheet, int startColumn, int endColumn, int startRow, int endRow,
                                    float xValue, float yValue,float originalWidth)
        {
            WorksheetImpl sheetImpl = (WorksheetImpl)wkSheet;

            int startColIndex = startColumn;
            int endColIndex = endColumn;
            int startRowIndex = startRow;
            int endRowIndex = endRow;
            float startX =  startRow <= endRow ? GetBorderXandY(wkSheet[startRow, startColumn], true) : 0;
            float startY =  startColumn <= endColumn ? GetBorderXandY(wkSheet[startRow, startColumn], false) : 0;
            float originalX = xValue;
            float originalY = yValue;
            int rowIndex=1;
            bool drawprinttitle = false;
            if (IsPrintTitleRowPage || IsPrintTitleColumnPage)
                rowIndex = 0;

                for (; rowIndex <= 1;rowIndex++)
                {
                    float rangeHeight = IsPrintTitleRowPage && drawprinttitle ?
                        pageSetupOption.TitleRowHeight
                        : Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetTotal(startRow, endRow),
                                                                             PdfGraphicsUnit.Point);

                    float rangeWidth = IsPrintTitleColumnPage && drawprinttitle ?
                        pageSetupOption.TitleColumnWidth
                        :Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetTotal(startColumn, endColumn), PdfGraphicsUnit.Point);

                    if (IsPrintTitleRowPage && drawprinttitle)
                    {
                        startRowIndex = pageSetupOption.PrintTitleFirstRow;
                        endRowIndex = pageSetupOption.PrintTitleLastRow;
                    }
                    else
                    {
                        //Need to find the Y position if sheet has print title row
                        if (IsPrintTitleRowPage)
                        {
                            startY = pageSetupOption.TitleRowHeight + startY + this.borderWidth;
                            if (pageSetupOption.PrintTitleFirstRow == startRow)
                            startRow = startRow + pageSetupOption.PrintTitleLastRow;
                        }
                        
                        startRowIndex = startRow;
                        endRowIndex = endRow;
                    }
                    if (IsPrintTitleColumnPage && drawprinttitle)
                    {
                        startColIndex = pageSetupOption.PrintTitleFirstColumn;
                        endColIndex = pageSetupOption.PrintTitleLastColumn;                        
                    }
                    else
                    {
                        //Need to find the Y position if sheet has print title row
                        if (IsPrintTitleColumnPage)
                        {
                            startX = pageSetupOption.TitleColumnWidth + startX + this.borderWidth;
                            if (pageSetupOption.PrintTitleFirstColumn == startColumn)
                                startColumn = startColumn + pageSetupOption.PrintTitleLastColumn;
                        }
                        
                        startColIndex = startColumn;
                        endColIndex = endColumn;
                    }
                    if (drawprinttitle)
                    {
                        float y =  startY;
                        float height = rangeHeight;

                        float x = startX;
                        float width = rangeWidth;
                        RectangleF rect=new RectangleF(startX,startY,width,height);
                        PdfBrush pdfBrush = new PdfSolidBrush(new PdfColor(Color.White));

                        this.pdfGraphics.DrawRectangle(pdfBrush, rect);
                    }

                    DrawRow(sheetImpl, startColIndex, endColIndex, startRowIndex, endRowIndex, xValue,
                        yValue, originalWidth, startX, startY, rangeWidth, rangeHeight);

                    if ((IsPrintTitleRowPage && IsPrintTitleColumnPage && drawprinttitle))
                    {
                        float m_startX = startX;
                        float m_startY = startY;

                    startX += Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetTotal(startColIndex, endColIndex), PdfGraphicsUnit.Point);
                    startX += GetBorderXandY(wkSheet[startRowIndex, startColIndex], true);// this.borderWidth;

                    int m_startColIndex = startColIndex;
                    int m_endColIndex = endColIndex;
                    int m_startRowIndex = startRowIndex;
                    int m_endRowIndex = endRowIndex;
                    startColIndex = startColumn;
                    endColIndex = endColumn;

                    this.DrawRow(sheetImpl, startColIndex, endColIndex, startRowIndex, endRowIndex, xValue,
                        yValue, originalWidth, startX, startY, rangeWidth, rangeHeight);

                    startX = m_startX;
                            

                    startY += Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetTotal(startRowIndex, endRowIndex), PdfGraphicsUnit.Point);

                    startY += GetBorderXandY(wkSheet[startRowIndex, startColIndex], false);// this.borderWidth;

                    startRowIndex = startRow;
                    endRowIndex = endRow;
                    startColIndex = m_startColIndex;
                    endColIndex = m_endColIndex;

                    this.DrawRow(sheetImpl, startColIndex, endColIndex, startRowIndex, endRowIndex, xValue,
                        yValue, originalWidth, startX, startY, rangeWidth, rangeHeight);

                    startRowIndex = m_startColIndex;
                    endRowIndex = m_endRowIndex;
                    startY = m_startY;
                    }
                    if (drawprinttitle)
                    {
                        m_bIsPrintTitleColumnPage = false;
                        m_bIsPrintTitleRowPage = false;
                    }
                    drawprinttitle = true;
                    startY = startColumn <= endColumn ? GetBorderXandY(wkSheet[startRow, startColumn], false) : 0;
                    startX = startRow <= endRow ? GetBorderXandY(wkSheet[startRow, startColumn], true) : 0;
                    xValue = originalX;
                    yValue = originalY;
                }
            return this.pdfDocument;
        }

        /// <summary>
        /// Draws the row.
        /// </summary>
        /// <param name="wkSheet">The wk sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>    
        /// <param name="originalWidth">Width of the original.</param>
        /// <returns>The Object of Pdf Document.</returns>
        private PdfDocument DrawRow(IWorksheet wkSheet, int startColIndex, int endColIndex, int startRowIndex, int endRowIndex,
                                   float xValue, float yValue, float originalWidth, float startX, float startY, float rangeWidth, float rangeHeight)
        {
            WorksheetImpl sheetImpl = (WorksheetImpl)wkSheet;

            if (sheetImpl.IsRightToLeft && !IsPrintTitleRowPage && this.excelToPdfSettings.LayoutOptions == LayoutOptions.NoScaling)
            {
                float rtlX = 0;
                if (sheetImpl.IsRightToLeft)
                    rtlX = pdfPageTemplate.Width - rangeWidth;

                pdfPageTemplate.Graphics.SetClip(new RectangleF(rtlX, 0, rangeWidth, rangeHeight));
            }

            if ((this.excelToPdfSettings.DisplayGridLines == GridLinesDisplayStyle.Auto && sheetImpl.PageSetup.PrintGridlines) ||
                this.excelToPdfSettings.DisplayGridLines == GridLinesDisplayStyle.Visible)
            {

                this.DrawLines(sheetImpl, startRowIndex, startColIndex, endRowIndex, endColIndex, this.pdfGraphics,
                    rangeWidth, rangeHeight, startX, startY);
            }

            if (sheetImpl.PageSetupBase.BackgoundImage == null)
            {

                this.DrawBackGround(sheetImpl, startRowIndex, startColIndex, endRowIndex, endColIndex, this.pdfGraphics,
                    this.DrawBackground, originalWidth, startX, startY);
            }

            this.DrawCells(sheetImpl, startRowIndex, startColIndex, endRowIndex, endColIndex, this.pdfGraphics, originalWidth, startX, startY);

            this.IterateMerges(sheetImpl, startRowIndex, startColIndex, endRowIndex, endColIndex, this.pdfGraphics, this.DrawMerge, originalWidth, startX, startY);

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
            if ((sheetImpl.Application as ApplicationImpl).ChartToImageConverter != null)
            {
                this.DrawCharts(sheetImpl, this.pdfGraphics, startRowIndex, startColIndex, endRowIndex, endColIndex, startX, startY);
            }
#endif

            this.DrawImages(sheetImpl, this.pdfGraphics, startRowIndex, startColIndex, endRowIndex, endColIndex, startX, startY);            

            this.DrawShape(sheetImpl, this.pdfGraphics, xValue + startX, yValue + startY);

            return this.pdfDocument;
        }
        /// <summary>
        /// Get Range Border X and Y
        /// </summary>
        /// <param name="Range">Cell</param>
        /// <param name="isXaxis">Identify the X or Y axis</param>
        /// <returns></returns>
        private float GetBorderXandY(IRange Range,bool isXaxis)
        {
            (Range.CellStyle as CellStyle).AskAdjacent = false;
            IBorder border;
            if(isXaxis)
                border = Range.Borders[ExcelBordersIndex.EdgeLeft];
            else
                border = Range.Borders[ExcelBordersIndex.EdgeTop];
                    
            return GetBorderWidth(border);
        }
        /// <summary>
        /// Draws the shape.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="positionX">The position X.</param>
        /// <param name="positionY">The position Y.</param>
        private void DrawShape(WorksheetImpl sheet, PdfGraphics graphics, float positionX, float positionY)
        {
            ITextBoxShape shape;
            if (sheet.TextBoxes != null && sheet.TextBoxes.Count>0)
            {
                IFont font;
                ITextBoxes textbox = sheet.TextBoxes;
                float sheetHeight = this.pdfDocument.PageSettings.Height -
                                   (this.pdfDocument.PageSettings.Margins.Top +
                                    this.pdfDocument.PageSettings.Margins.Bottom);
                float sheetWidth = this.pdfDocument.PageSettings.Width -
                                  (this.pdfDocument.PageSettings.Margins.Left +
                                   this.pdfDocument.PageSettings.Margins.Right);
                float valueX = positionX + sheetWidth;
                float valueY = positionY + sheetHeight;
                for (int i = 0; i < textbox.Count; i++)
                {
                    shape = textbox[i];
                    if (shape.IsShapeVisible)
                    {
                        double leftValue = Pdf_UnitConverter.ConvertFromPixels((float)shape.Left, PdfGraphicsUnit.Point);
                        float left = (float)leftValue - (((int)leftValue / (int)sheetWidth) * sheetWidth);
                        double topValue = Pdf_UnitConverter.ConvertFromPixels((float)shape.Top, PdfGraphicsUnit.Point);
                        float top = (float)topValue - (((int)topValue / (int)sheetHeight) * sheetHeight);
                        float height = Pdf_UnitConverter.ConvertFromPixels((float)shape.Height, PdfGraphicsUnit.Point);
                        float width = Pdf_UnitConverter.ConvertFromPixels((float)shape.Width, PdfGraphicsUnit.Point); 
                        if (height != 0 && width != 0)
                        {
                            System.Drawing.RectangleF rect = new RectangleF(left, top, width, height);
                            if ((shape.Top <= valueY && shape.Top >= positionY) && (shape.Left <= valueX
                                && shape.Left >= positionX || shape.Left==0))
                            {
                                PdfColor color=new PdfColor(shape.Fill.ForeColor);
                                if(shape.Fill.ForeColor.A== 0x00)
                                  color  =new PdfColor(Color.White);

                                Color lineColor = (shape.Line.Visible) ? shape.Line.ForeColor : Color.White;
                                PdfPen pdfPen = this.CreatePen(shape.Line, lineColor);
                                                            
                                if (!String.IsNullOrEmpty(shape.RichText.Text))
                                {
                                   
                                    font = shape.RichText.GetFont(0);
                                    PdfStringFormat horizontalFormat = new PdfStringFormat();
                                    horizontalFormat.Alignment = this.GetTextAlignmentFromShape(shape);
                                    PdfFont pdfFont = new PdfTrueTypeFont(this.GetFont(font, font.FontName, (int)font.Size),
                                                                          this.excelToPdfSettings.EmbedFonts);
                                    horizontalFormat.LineAlignment = this.GetVerticalAlignmentFromShape(shape);
                                    if (shape.ShapeRotation !=0)
                                    {
                                        PdfGraphicsState state = graphics.Save();
                                        PointF transformPoint = new PointF(0, 0);
                                        transformPoint.X = rect.X + rect.Width / 2;
                                        transformPoint.Y = rect.Y + rect.Height / 2;
                                        graphics.TranslateTransform(transformPoint.X, transformPoint.Y);
                                        graphics.RotateTransform((float)shape.ShapeRotation);
                                        RectangleF reverseAngle = new RectangleF(0, 0, rect.Height, rect.Width);
                                        graphics.DrawRectangle(pdfPen,
                                                       new PdfSolidBrush(color),
                                                                       reverseAngle);
                                        graphics.DrawString(shape.Text, pdfFont,
                                                           new PdfSolidBrush(this.NormalizeColor(font.RGBColor)), reverseAngle,
                                                           horizontalFormat);
                                        graphics.Restore(state);
                                       
                                    }
                                    else 
                                    {
                                        graphics.DrawRectangle(pdfPen, new PdfSolidBrush(color), rect);
                                        graphics.DrawString(shape.Text, pdfFont,
                                                            new PdfSolidBrush(this.NormalizeColor(font.RGBColor)), rect,
                                                            horizontalFormat);                                     
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the pen.
        /// </summary>
        /// <param name="lineFormat">The format of the line.</param>
        /// <param name="lineColor">Color of the line.</param>
        /// <returns>The PdfPen object.</returns>
        private PdfPen CreatePen(IShapeLineFormat lineFormat, Color lineColor)
        {
            if (lineColor.IsEmpty)
            {
                lineColor = this.NormalizeColor(lineColor);
            }

            PdfColor penColor = new PdfColor(lineColor);
            PdfPen pdfPen = new PdfPen(penColor, this.GetShapeLineWidth(lineFormat));
            pdfPen.DashStyle = this.GetDashStyle(lineFormat);
            return pdfPen;
        }

        /// <summary>
        /// Gets the dash style.
        /// </summary>
        /// <param name="lineFormat">Theformat of the line.</param>
        /// <returns>The PdfDashStyle value.</returns>
        private PdfDashStyle GetDashStyle(IShapeLineFormat lineFormat)
        {
            PdfDashStyle dashStyle = PdfDashStyle.Solid;

            switch (lineFormat.DashStyle)
            {
                case ExcelShapeDashLineStyle.Dash_Dot:
                case ExcelShapeDashLineStyle.Medium_Dash_Dot:
                    dashStyle = PdfDashStyle.DashDot;
                    break;

                case ExcelShapeDashLineStyle.Dotted:
                case ExcelShapeDashLineStyle.Dotted_Round:
                    dashStyle = PdfDashStyle.Dot;
                    break;

                case ExcelShapeDashLineStyle.Dash_Dot_Dot:
                    dashStyle = PdfDashStyle.DashDotDot;                               
                    break;
               
                case ExcelShapeDashLineStyle.Dashed:
                case ExcelShapeDashLineStyle.Medium_Dashed:
                    dashStyle = PdfDashStyle.Dash;
                    break;

                case ExcelShapeDashLineStyle.Solid:
                    dashStyle = PdfDashStyle.Solid;
                    break;            
            }

            return dashStyle;
        }

        /// <summary>
        /// Gets the width of the line
        /// </summary>
        /// <param name="lineFormat">The format of the line.</param>
        /// <returns>The borderWidth of the line index.</returns>
        private float GetShapeLineWidth(IShapeLineFormat lineFormat)
        {
            switch (lineFormat.DashStyle)
            {
                case ExcelShapeDashLineStyle.Dash_Dot:
                case ExcelShapeDashLineStyle.Dashed:
                case ExcelShapeDashLineStyle.Dotted:
                case ExcelShapeDashLineStyle.Dash_Dot_Dot:
                case ExcelShapeDashLineStyle.Dotted_Round:             
                    this.borderWidth = 1F;
                    break;
                case ExcelShapeDashLineStyle.Medium_Dash_Dot:
                case ExcelShapeDashLineStyle.Medium_Dashed:     
                    this.borderWidth = 2F;
                    break;              
                case ExcelShapeDashLineStyle.Solid:
                    this.borderWidth = 0.5F;
                    break;
            }
            return this.borderWidth;
        }
        /// <summary>
        /// Draws the headers and footers.
        /// </summary>
        /// <param name="pdfSection">The PDF section.</param>
        /// <param name="sheetImpl">The sheet impl.</param>
        /// <returns>The collection of the header and footer objects.</returns>
        private List<HeaderFooter> DrawHeadersAndFooters(PdfSection pdfSection, WorksheetImpl sheetImpl,ChartImpl chart)
        {
            AllowHeaderFooterOnce = true;
            string sheetOrChartName;
            RectangleF rect = new RectangleF();
            Dictionary<string, string> pageSetup;
            List<HeaderFooter> headerFooterCollections = new List<HeaderFooter>();
            HeaderFooter headerFooter = null;
            PageSetupBaseImpl pageSetupBase;
            if (sheetImpl != null)
            {
                pageSetupBase = sheetImpl.PageSetupBase;
                sheetOrChartName = sheetImpl.Name;
            }
            else
            {
                pageSetupBase = chart.PageSetupBase;
                sheetOrChartName = chart.Name;
            }
            //To assign the workbook Impl.
            if (this.workBookImpl == null && sheetImpl != null)
            {
                this.workBookImpl = (WorkbookImpl)sheetImpl.Workbook;
            }
            else if (this.workBookImpl == null)
            {
                this.workBookImpl = (WorkbookImpl)chart.Workbook;
            }
            if (this.predefinedHeaderFooter.Count == 0 && sheetImpl!=null)
            {
                this.IntializeHeaderFooter(sheetImpl as IWorksheet);
            }
            else if (this.predefinedHeaderFooter.Count == 0 && chart != null)
            {
                this.IntializeHeaderFooter(chart as IChart);
            }
            if (!String.IsNullOrEmpty(pageSetupBase.FullHeaderString))
            {
                headerFooter = new HeaderFooter();
                //pdfSection.PageSettings.Margins.Top = 10;
                rect = new RectangleF(0, 0, pdfSection.PageSettings.Width, 0);
                headerFooter.TemplateSize = rect;
                headerFooter.HeaderFooterName = "Header";
            }

            pageSetup = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(pageSetupBase.LeftHeader))
            {
                pageSetup.Add("L", pageSetupBase.LeftHeader);
            }

            if (!String.IsNullOrEmpty(pageSetupBase.CenterHeader))
            {
                pageSetup.Add("C", pageSetupBase.CenterHeader);
            }

            if (!String.IsNullOrEmpty(pageSetupBase.RightHeader))
            {
                pageSetup.Add("R", pageSetupBase.RightHeader);
            }

            if (!String.IsNullOrEmpty(pageSetupBase.FullHeaderString))
            {

                headerFooter.HeaderFooterSections = this.GetHeaderFooterOptions(pageSetup, pdfSection, pageSetupBase,
                                                                pdfSection.PageSettings.Width -
                                                               (pdfSection.PageSettings.Margins.Left +
                                                                pdfSection.PageSettings.Margins.Right), "Header", sheetOrChartName);
                headerFooterCollections.Add(headerFooter);
                AllowHeader = AllowHeaderFooterOnce;
                AllowHeaderFooterOnce = true;
            }

            if (!String.IsNullOrEmpty(pageSetupBase.FullFooterString))
            {
                headerFooter = new HeaderFooter();
                //pdfSection.PageSettings.Margins.Bottom = 0;

                rect = new RectangleF(0, 0, pdfSection.PageSettings.Width, 0);
                headerFooter.TemplateSize = rect;
                headerFooter.HeaderFooterName = "Footer";
                pageSetup = new Dictionary<string, string>();
                if (!String.IsNullOrEmpty(pageSetupBase.LeftFooter))
                {
                    pageSetup.Add("L", pageSetupBase.LeftFooter);
                }

                if (!String.IsNullOrEmpty(pageSetupBase.CenterFooter))
                {
                    pageSetup.Add("C", pageSetupBase.CenterFooter);
                }

                if (!String.IsNullOrEmpty(pageSetupBase.RightFooter))
                {
                    pageSetup.Add("R", pageSetupBase.RightFooter);
                }

                if (!String.IsNullOrEmpty(pageSetupBase.FullFooterString))
                {                    
                    headerFooter.HeaderFooterSections = this.GetHeaderFooterOptions(pageSetup, pdfSection, pageSetupBase,
                                                                    pdfSection.PageSettings.Width -
                                                                   (pdfSection.PageSettings.Margins.Left +
                                                                    pdfSection.PageSettings.Margins.Right),
                                                                    "Footer",sheetOrChartName);
                    headerFooterCollections.Add(headerFooter);
                    AllowFooter = AllowHeaderFooterOnce;
                }
            }
            AllowHeaderFooterOnce = true;
            return headerFooterCollections;
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="lastRow">The last row.</param>
        /// <param name="lastColumn">The last column.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void DrawLines(WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
                               PdfGraphics graphics, float width, float height, float startX, float startY)
        {            
            float x = startX;
            float y = startY;

            PdfPen pen = sheet.DefaultGridlineColor ?
            PdfPens.Gray :
             new PdfPen(sheet.Workbook.GetPaletteColor(sheet.GridLineColor));

            // Draw Rows Border
            graphics.DrawLine(pen, x, y, width + x, y);
            for (int i = firstRow; i <= lastRow; i++)
            {
                y += Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetSize(i),
                                                           PdfGraphicsUnit.Point);
                graphics.DrawLine(pen, startX, y, width + x, y);
            }
            startY -= 0.5f;
            y += 0.5f;

            //// Draw Columns Border
            graphics.DrawLine(pen, x, startY, x, y);
            for (int i = firstColumn; i <= lastColumn; i++)
            {
                x += Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetSize(i),
                                                           PdfGraphicsUnit.Point);
                graphics.DrawLine(pen, x, startY, x, y);
            }
        }
        /// <summary>
        /// Iterates the merges.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="lastRow">The last row.</param>
        /// <param name="lastColumn">The last column.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="method">The method.</param>
        /// <param name="originalWidth">Width of the i original.</param>
        private void IterateMerges(WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow,
                                    int lastColumn, PdfGraphics graphics, MergeMethod method, float originalWidth,float startX,float startY)
        {
            if (sheet.HasMergedCells)
            {
                List<MergeCellsRecord.MergedRegion> lstRegions = new List<MergeCellsRecord.MergedRegion>();
                MergeCellsImpl mergedCells = sheet.MergeCells;
                mergedCells.CacheMerges(sheet[firstRow, firstColumn, lastRow, lastColumn], lstRegions);

                for (int i = 0, len = lstRegions.Count; i < len; i++)
                {
                    method(sheet, lstRegions[i], firstRow, firstColumn, lastRow, lastColumn, graphics, originalWidth,startX,startY);
                }
            }
        }

        /// <summary>
        /// Draws the merge.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="mergedRegion">The merged region.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="originalWidth">Width of the i original.</param>
        private void DrawMerge(WorksheetImpl sheet, MergeCellsRecord.MergedRegion mergedRegion, int firstRow,
                               int firstColumn, int lastRow, int lastColumn, PdfGraphics graphics, float originalWidth,float startX,float startY)
        {
            MergeCellsImpl mergedCells = sheet.MergeCells;
            ExtendedFormatImpl format = mergedCells.GetFormat(mergedRegion);
            RectangleF rect = this.GetMergedRectangle(sheet, mergedRegion, firstRow, firstColumn,
                                                      lastRow, lastColumn,startX,startY);
            if (this.excelToPdfSettings.EnableRTL)
            {
                rect.X = originalWidth - rect.X;
                rect.X = rect.X - rect.Width;
            }

            IRange cell = sheet[mergedRegion.RowFrom + 1, mergedRegion.ColumnFrom + 1];

            if (rect.Right > sheetWidth)
            {
                float actualWidth = ColumnWidthGetter.GetTotal(mergedRegion.ColumnFrom + 1, lastColumn);
                actualWidth = Pdf_UnitConverter.ConvertFromPixels(actualWidth, PdfGraphicsUnit.Point);
                rect.Width = actualWidth;
            }
            if (cell.CellStyle.FillPattern != ExcelPattern.None)
            {
                this.DrawBackground(cell, rect, graphics);
            }
            if (cell.ConditionalFormats.Count != 0)
            {
                this.ApplyMergeCellCondiFormat(cell, rect, graphics);
            }
            if (rect.Height > 0 && rect.Width > 0)
            {                
                this.DrawCell(format, cell, rect, rect, graphics);               
            }
        }
        private void ApplyMergeCellCondiFormat(IRange cell, RectangleF rectangle, PdfGraphics graphics)
        {
            ExtendedFormatImpl xf = (cell.CellStyle as CellStyle).Wrapped;
            xf = this.conditionalFormatApplier.ApplyCF(cell, xf);
            this.DrawBackground(xf, rectangle, graphics, cell);
        }
        /// <summary>
        /// Draws the back ground.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="lastRow">The last row.</param>
        /// <param name="lastColumn">The last column.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="method">The method.</param>
        /// <param name="originalWidth">Width of the i original.</param>
        private void DrawBackGround(WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow,
                                    int lastColumn, PdfGraphics graphics, CellMethod method, float originalWidth, float startX, float startY)
        {
            MigrantRangeImpl cell = new MigrantRangeImpl(sheet.Application, sheet);
            float x = 0;
            float y = 0;
            x += startX;
            y += startY;
            for (int rowIndex = firstRow; rowIndex <= lastRow; rowIndex++)
            {
                y = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetTotal(firstRow, rowIndex - 1),
                                                         PdfGraphicsUnit.Point) + startY;
                float height = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetSize(rowIndex),
                                                                    PdfGraphicsUnit.Point);
                for (int columnIndex = firstColumn; height > 0 && columnIndex <= lastColumn; columnIndex++)
                {
                    cell.ResetRowColumn(rowIndex, columnIndex);
                    if (!cell.IsMerged && ((cell.CellStyle as CellStyle).Wrapped.FillPattern != ExcelPattern.None || cell.ConditionalFormats.Count > 0))
                    {
                        x = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetTotal(firstColumn, columnIndex - 1),
                             PdfGraphicsUnit.Point) + startX;
                        float width = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(columnIndex),
                            PdfGraphicsUnit.Point);
                        if (this.excelToPdfSettings.EnableRTL)
                        {
                            x = originalWidth - x;
                            x = x - width;
                        }
                        RectangleF rect = new RectangleF(x, y, width, height);
                        method(cell, rect, graphics);
                    }
                }
            }
        }
        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="cell">The cell range.</param>
        /// <param name="rectangle">The cell rectangle.</param>
        /// <param name="graphics">The pdf page graphics object.</param>
        private void DrawBackground(IRange cell, RectangleF rectangle, PdfGraphics graphics)
        {
            ExtendedFormatImpl xf = (cell.CellStyle as CellStyle).Wrapped;
#if !SyncfusionFramework2_0
            int startRow = 1;
            int startCol = 1;
            for (int pivotCount = 0; pivotCount < cell.Worksheet.PivotTables.Count; pivotCount++)
            {
                if (cell.Row == cell.Worksheet.PivotTables[pivotCount].Location.Row && cell.Column == cell.Worksheet.PivotTables[pivotCount].Location.Column)
                {
                    pivotTableRange = cell.Worksheet.PivotTables[pivotCount].Location;
                    pivotImpl = cell.Worksheet.PivotTables[pivotCount] as PivotTableImpl;
                }
            }

            if (pivotTableRange != null)
            {
                startRow = pivotTableRange.Row;
                startCol = pivotTableRange.Column;
            }

            PivotTableStyleRenderer renderer = new PivotTableStyleRenderer(cell.Worksheet);
            
            if (cell.Worksheet.PivotTables.Count != 0 && pivotImpl.PageFields.Count != 0)
            {
                if (startRow - 2 == cell.Row && startCol == cell.Column)
                {
                    xf = renderer.GetPageFilterLabel(pivotImpl.BuiltInStyle);
                }
                if (startRow - 2 == cell.Row && startCol + 1 == cell.Column)
                {
                    xf = renderer.GetPageFilterValue(pivotImpl.BuiltInStyle);
                }
            }

            if (pivotTableRange != null && pivotTableRange.Row <= cell.Row && pivotTableRange.LastRow >= cell.LastRow
              && pivotTableRange.Column <= cell.Column && pivotTableRange.LastColumn >= cell.LastColumn)
            {
                xf = pivotImpl.PivotLayout[cell.Row - startRow, cell.Column - startCol].XF;
            }
            else
#endif
                xf = this.conditionalFormatApplier.ApplyCF(cell, xf);
            this.DrawBackground(xf, rectangle, graphics, cell);
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="internalExtendedFormat">The internal extended format.</param>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="cell">The cell range.</param>
        private void DrawBackground(IInternalExtendedFormat internalExtendedFormat, RectangleF rect,
                                     PdfGraphics graphics, IRange cell)
        {
            if (rect.Height > 0 && rect.Width > 0)
            {
                PdfBrush pdfBrush;
                IConditionalFormats formats = cell.ConditionalFormats;

                if (internalExtendedFormat.FillPattern == ExcelPattern.None)
                {
                    rect.Offset(1, 1);
                    rect.Width -= 3;
                    rect.Height -= 3;

                    IBorders borders = internalExtendedFormat.Borders;
                    rect = this.UpdateRectangleCoordinates(rect, borders);

                    pdfBrush = PdfBrushes.Transparent;
                    graphics.DrawRectangle(pdfBrush, rect);
                    pdfBrush = null;
                }
                else if (formats.Count > 0 && cell.CellStyle.FillPattern == ExcelPattern.None && (internalExtendedFormat.Color != Color.White || cell.CellStyle.Color == Color.Empty))
                {
                    pdfBrush = new PdfSolidBrush(new PdfColor(internalExtendedFormat.Color));
                    graphics.DrawRectangle(pdfBrush, rect);
                    pdfBrush = null;
                }
                else
                {
                    pdfBrush = this.GetBrush(internalExtendedFormat);
                    graphics.DrawRectangle(pdfBrush, rect);
                    pdfBrush = null;
                }
            }
        }

        /// <summary>
        /// Draws the merged background.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="mergedRegion">The merged region.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="originalWidth">Width of the i original.</param>
        private void DrawMergedBackground(WorksheetImpl sheet, MergeCellsRecord.MergedRegion mergedRegion,
                                         int firstRow, int firstColumn, int lastRow, int lastColumn, PdfGraphics graphics,float originalWidth,float startX,float startY)
        {
            MergeCellsImpl mergedCells = sheet.MergeCells;

            ExtendedFormatImpl format = mergedCells.GetFormat(mergedRegion);

            float y = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetSize(mergedRegion.RowFrom + 1),
                                                           PdfGraphicsUnit.Point);
            float x = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(mergedRegion.ColumnFrom + 1),
                                                           PdfGraphicsUnit.Point);
            int firstMergedRow = firstRow > mergedRegion.RowFrom + 1 ? firstRow : mergedRegion.RowFrom + 1;
            int firstMergedColumn = firstColumn > mergedRegion.ColumnFrom + 1 ? firstColumn : mergedRegion.ColumnFrom + 1;
            y = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetTotal(firstRow, firstMergedRow),
                                                      PdfGraphicsUnit.Point) - y;
            x = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetTotal(firstColumn, firstMergedColumn),
                                                      PdfGraphicsUnit.Point) - x;
            y += startY;
            x += startX;
            if (this.excelToPdfSettings.EnableRTL)
            {
                x = originalWidth - x;
            }

            float height = RowHeightGetter.GetTotal(firstMergedRow,mergedRegion.RowTo+1);
            height = Pdf_UnitConverter.ConvertFromPixels(height, PdfGraphicsUnit.Point);
            float width = GetMergedWidth(firstColumn, lastColumn, mergedRegion.ColumnFrom + 1, mergedRegion.ColumnTo + 1);
            float actualWidth = ColumnWidthGetter.GetTotal(firstColumn, lastColumn);
            actualWidth = Pdf_UnitConverter.ConvertFromPixels(actualWidth, PdfGraphicsUnit.Point);
            width = Pdf_UnitConverter.ConvertFromPixels(width, PdfGraphicsUnit.Point);
            if (width > actualWidth)
            {
                width = actualWidth - x;             
            }
            //columnWidthGetter.GetTotal(mergedRegion.ColumnFrom + 1, mergedRegion.ColumnTo + 1);
        
            if (this.excelToPdfSettings.EnableRTL)
            {
                x = x - width;
            }

            IRange cell = sheet[firstMergedRow, mergedRegion.ColumnFrom + 1];
            RectangleF rect = new RectangleF(x, y, width, height);
            if (height > 0 && width > 0)
            {
                rect.Width += 1.0f;
                rect.Height += 1.0f;
                this.DrawBackground(format, rect, graphics, cell);
            }
        }
        /// <summary>
        /// Gets the width of the merged cells.
        /// </summary>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="lastColumn">The last column.</param>
        /// <param name="mergedFirstColumn">The merged first column.</param>
        /// <param name="mergedLastColumn">The merged last column.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <returns></returns>
        private float GetMergedWidth(int firstColumn, int lastColumn, int mergedFirstColumn, int mergedLastColumn)
        {
            int columnFirst = firstColumn;
            int columnLast = lastColumn;

            if (mergedFirstColumn <= lastColumn)
                columnFirst = mergedFirstColumn;

            if (mergedLastColumn <= lastColumn)
                columnLast = mergedLastColumn;
            else
                columnLast = lastColumn;


            return ColumnWidthGetter.GetTotal(columnFirst, columnLast);
        }
        /// <summary>
        /// Draws the cells.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="lastRow">The last row.</param>
        /// <param name="lastColumn">The last column.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="originalWidth">Width of the i original.</param>
        private void DrawCells(WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
                               PdfGraphics graphics, float originalWidth,float startX,float startY)
        {
            MigrantRangeImpl cell = new MigrantRangeImpl(sheet.Application, sheet);

            MigrantRangeImpl cell2 = new MigrantRangeImpl(sheet.Application, sheet);
            int lastPossibleColumn = cell.Workbook.MaxColumnCount;
            float x=0;
            float y=0;
            x += startX;
            y += startY;
            for (int rowIndex = firstRow; rowIndex <= lastRow; rowIndex++)
            {
                this.adjacentRectWidth = 0;
                y = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetTotal(firstRow, rowIndex - 1),
                                                         PdfGraphicsUnit.Point)+startY;
                float height = Pdf_UnitConverter.ConvertFromPixels(RowHeightGetter.GetSize(rowIndex),
                                                                    PdfGraphicsUnit.Point);

                if (height > 0)
                {
                    for (int columnIndex = firstColumn; columnIndex <= lastColumn; columnIndex++)
                    {
                        x = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetTotal(firstColumn, columnIndex - 1),
                                                                 PdfGraphicsUnit.Point)+startX;
                        if (this.excelToPdfSettings.EnableRTL)
                        {
                            x = originalWidth - x;
                        }

                        cell.ResetRowColumn(rowIndex, columnIndex);

                        if (!cell.IsMerged)
                        {
                            float width = Pdf_UnitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(columnIndex),
                                                                               PdfGraphicsUnit.Point);
                            if (this.excelToPdfSettings.EnableRTL)
                            {
                                x = x - width;
                            }

                            if (width > 0)
                            {
                                RectangleF rect = new RectangleF(x, y, width, height);
                                RectangleF adjacentRect = this.GetAdjacentCells(cell, rect,firstColumn, cell2, originalWidth);
                                if (m_bHasPrintTitleColumn)
                                    adjacentRect.X = x;

                                this.DrawCell(cell, rect, adjacentRect, graphics);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the cell.
        /// </summary>
        /// <param name="cell">The migrant cell range.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="adjacentRect">The adjacent cell rectangle.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        private void DrawCell(MigrantRangeImpl cell, RectangleF cellRect, RectangleF adjacentRect,
                              PdfGraphics graphics)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell");
            }

            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            int extendedFormatIndex = cell.ExtendedFormatIndex;
            ExtendedFormatImpl extendedFormatImpl = cell.Workbook.InnerExtFormats[extendedFormatIndex];
            if(cell.HasStyle || !String.IsNullOrEmpty(cell.DisplayText))
            this.DrawCell(extendedFormatImpl, cell, cellRect, adjacentRect, graphics);
        }

        /// <summary>
        /// Draws the cell.
        /// </summary>
        /// <param name="extendedFormatImpl">The extended format impl.</param>
        /// <param name="cell">The cell range.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="adjacentRect">The adjacent cell rect.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        private void DrawCell(ExtendedFormatImpl extendedFormatImpl, IRange cell, RectangleF cellRect,
                              RectangleF adjacentRect, PdfGraphics graphics)
        {
            bool m_nativeFont=false;
            m_fitText = false;
            adjacentRectWidth += adjacentRect.Width;
            int intentLevel = extendedFormatImpl.IndentLevel;
            #region Pivot Table
#if !SyncfusionFramework2_0
            int startRow = 1;
            int startCol = 1;
            for (int pivotCount = 0; pivotCount < cell.Worksheet.PivotTables.Count; pivotCount++)
            {
                if (cell.Row == cell.Worksheet.PivotTables[pivotCount].Location.Row && cell.Column == cell.Worksheet.PivotTables[pivotCount].Location.Column)
                {
                    pivotTableRange = cell.Worksheet.PivotTables[pivotCount].Location;
                    pivotImpl = cell.Worksheet.PivotTables[pivotCount] as PivotTableImpl;
                }
            }
            if(pivotTableRange != null )
            {          
                startRow = pivotTableRange.Row;
                startCol = pivotTableRange.Column;
            }
            PivotTableStyleRenderer renderer = new PivotTableStyleRenderer(cell.Worksheet);
            if (cell.Worksheet.PivotTables.Count != 0 && pivotImpl.PageFields.Count != 0)
            {
                if (startRow - 2 == cell.Row && startCol == cell.Column)
                {
                    extendedFormatImpl = renderer.GetPageFilterLabel(pivotImpl.BuiltInStyle);
                    extendedFormatImpl.VerticalAlignment = ExcelVAlign.VAlignTop;
                }
                if (startRow - 2 == cell.Row && startCol + 1 == cell.Column)
                {
                    extendedFormatImpl = renderer.GetPageFilterValue(pivotImpl.BuiltInStyle);
                    extendedFormatImpl.VerticalAlignment = ExcelVAlign.VAlignTop;
                }
            }

            if (cell.Worksheet.PivotTables.Count!=0&&pivotTableRange != null && pivotTableRange.Row <= cell.Row && pivotTableRange.LastRow >= cell.LastRow
              && pivotTableRange.Column <= cell.Column && pivotTableRange.LastColumn >= cell.LastColumn)
            {
                extendedFormatImpl = pivotImpl.PivotLayout[cell.Row - startRow, cell.Column - startCol].XF;                        
                extendedFormatImpl.VerticalAlignment = ExcelVAlign.VAlignTop;
               if (extendedFormatImpl.HorizontalAlignment == ExcelHAlign.HAlignGeneral && extendedFormatImpl.HorizontalAlignment != cell.CellStyle.HorizontalAlignment)
               {
                   extendedFormatImpl.HorizontalAlignment = cell.CellStyle.HorizontalAlignment;
                   if (intentLevel == 0)
                   {
                       intentLevel = 10;
                   }
                   else
                   {
                       intentLevel = intentLevel * 23;
                   }
                   extendedFormatImpl.IndentLevel = intentLevel;
               }
            }
            else
#endif
            #endregion
                extendedFormatImpl = this.conditionalFormatApplier.ApplyCF(cell, extendedFormatImpl);
            PdfBrush pdfBrush;
            bool isRTL = false;

            Color fontColor = GetCustomNumberFormatColor(extendedFormatImpl, cell);

            IFont excelFont = extendedFormatImpl.Font;
            string excelFontName = excelFont.FontName;
            bool isUnicode = false;
            if (cell.Text != null)
                isUnicode = CheckUnicode(cell.Text);
            if (isUnicode)
                switch (excelFontName)
                {
                    case "Arial":
                    case "Arial Black":
                    case "Microsoft Sans Serif":
                    case "Times New Roman":
                    case "Trebuchet MS":
                        excelFont.FontName = "Arial Unicode MS";
                        break;
                    default:
                        break;
                }                        
            Color cellTextColor = this.NormalizeColor(excelFont.RGBColor);

            if (fontColor != Color.Empty)
                cellTextColor = fontColor;

            if (this.fontTableColors != null)
            {
                if (this.fontTableColors.Count != 0)
                {
                    foreach (KeyValuePair<IRange, Color> fontTableColor in this.fontTableColors)
                    {
                        if (this.CheckRange(fontTableColor.Key, cell))
                        {
                            cellTextColor = fontTableColor.Value;
                        }
                    }
                }
            }

            pdfBrush = new PdfSolidBrush(cellTextColor);
            PdfStringFormat pdfTextformat = new PdfStringFormat();
            pdfTextformat.WordWrap = extendedFormatImpl.WrapText ? PdfWordWrapType.Word : PdfWordWrapType.None;
            pdfTextformat.Alignment = this.GetHorizontalAlignmentFromExtendedFormat(extendedFormatImpl, cell);
            pdfTextformat.LineAlignment = this.GetVerticalAlignmentFromExtendedFormat(extendedFormatImpl);
            pdfTextformat.FirstLineIndent = extendedFormatImpl.IndentLevel * 5;

            string cellText = cell.DisplayText;
            if (cell.IsMerged && !cell.WrapText)
            {
                char[] cellTextArray = cellText.ToCharArray();
                for (int i = 0; i < cellTextArray.Length; i++)
                {
                    if(removableCharaters.Contains (cellTextArray[i]))
                    {
                       cellText=cellText.Replace(cellTextArray[i], '\0');
                    }
                }                          
            }
            if(Array.IndexOf(numberFormats,cell.NumberFormat)>=0)
            {
                StringBuilder build=new StringBuilder();
                build.Append("R");
                build.Append("\t");
                build.Append(System.Convert.ToDecimal(cell.Number.ToString("N2")));
                cellText= build.ToString();
            }

            
            cellText = this.GetBottomText(cellText, extendedFormatImpl.Rotation);
            Font nativeFont = ((FontImpl)excelFont).GenerateNativeFont((float)excelFont.Size);

            ushort[] characterCodes = new ushort[cellText.Length];
            KernelApi.GetStringTypeExW(0x800, StringInfoType.CT_TYPE2, cellText, cellText.Length, characterCodes);
            pdfTextformat.RightToLeft =isRTL= this.CheckIfRTL(characterCodes);

            if(cellText.StartsWith(" "))
            {
                pdfTextformat.MeasureTrailingSpaces = true;
            }

            if (!this.excelToPdfSettings.EmbedFonts)
            {
                //Checking the condition for the unicode text(eg:chinese,japanese etc)
                if (Encoding.UTF8.GetByteCount(cellText) != cellText.Length)
                {
                    this.excelToPdfSettings.EmbedFonts = true;
                }
            }

            if (this.CheckUnicode(cellText))
            {
                if (!this.fontList.Contains(nativeFont.Name))
                {
                    nativeFont = extendedFormatImpl.Font.GenerateNativeFont();
                    m_nativeFont=true;
                }
            }
           
            PdfFont pdfFont;

            if (PdfDocument.EnableCache)
            {
                pdfFont = new PdfTrueTypeFont(nativeFont, this.excelToPdfSettings.EmbedFonts);
            }
            else
            {
                if (!fontCollection.ContainsKey(nativeFont))
                {
                    pdfFont = new PdfTrueTypeFont(nativeFont, this.excelToPdfSettings.EmbedFonts);
                    fontCollection.Add(nativeFont, pdfFont);
                }
                else
                    pdfFont = fontCollection[nativeFont];
            }
          
            if (SplitTexts.Count != 0)
            {
                for (int index = 0; index < SplitTexts.Count; index++)
                {
                    SplitText text = SplitTexts[index];

                    if (isNewPage && cell.Row == text.Row && cell.Column == text.AdjacentColumn)
                    {
                        this.DrawSplitText(text);
                        SplitTexts.RemoveAt(index);
                    }
                }
            }
            float cellTextWidth = pdfFont.MeasureString(cellText, pdfTextformat).Width;
            float cellTextHeight = pdfFont.MeasureString(cellText, pdfTextformat).Height;
            if (cell.CellStyleName == "Hyperlink")
            {
                cellText = this.GetDisplayText(pdfFont, pdfTextformat, cellText, adjacentRect, cell, pdfBrush, isRTL);
                if (cell.Hyperlinks.Count > 0)
                {
                    this.SetHyperLink(adjacentRect,cell);
                }
                if (adjacentRect.Width < cellTextWidth && (pdfTextformat.WordWrap == PdfWordWrapType.None))
                {
                    adjacentRect.Width = cellTextWidth;
                }
                else if (AdjacentRectWidth > SheetWidth && this.excelToPdfSettings.LayoutOptions == LayoutOptions.NoScaling && !pdfTextformat.RightToLeft && !pdfTextformat.RightToLeft)
                {
                    cellText = this.GetDisplayText(pdfFont, pdfTextformat, cellText, adjacentRect, cell, pdfBrush, isRTL);
                }
                graphics.DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat);
            }
            else if (extendedFormatImpl.Rotation != ExtendedFormatImpl.TopToBottomRotation
                     && extendedFormatImpl.Rotation != 0 && !String.IsNullOrEmpty(cellText))
            {
                char[] cellTextArray = cellText.ToCharArray();

                SizeF firstCharWidth = new SizeF(pdfFont.MeasureString(cellTextArray[0].ToString(), pdfTextformat).Width,
                                                 pdfFont.MeasureString(cellTextArray[0].ToString(), pdfTextformat).Height);
                float totalWidth = pdfFont.MeasureString(cellText, pdfTextformat).Width;
                SizeF lastCharSize = new SizeF(pdfFont.MeasureString(cellTextArray[cellTextArray.Length - 1].ToString(), pdfTextformat).Width,
                                               pdfFont.MeasureString(cellTextArray[cellTextArray.Length - 1].ToString(), pdfTextformat).Height);
                if (pdfTextformat.Alignment == PdfTextAlignment.Right)
                {
                    adjacentRect.Width -= this.borderWidth;
                    adjacentRect.Height -= this.borderWidth;
                }
                else
                {
                    adjacentRect.X += this.borderWidth;
                    adjacentRect.Height -= this.borderWidth;
                }

                this.DrawRotatedText(adjacentRect, extendedFormatImpl, cell, cellText, graphics, pdfFont,
                                     pdfBrush, pdfTextformat, firstCharWidth, totalWidth, lastCharSize);
            }
            else
            {
                if ((cellTextWidth > adjacentRect.Width)
                     && (pdfTextformat.WordWrap == PdfWordWrapType.None))
                {
                    if (!cell.HasDateTime && !cell.HasNumber)
                    {
                        cellText = this.GetDisplayText(pdfFont, pdfTextformat, cellText, adjacentRect, cell, pdfBrush, isRTL);
                    }
                    if (cell.HasNumber && !cell.HasDateTime)
                    {
                        cellText = String.Format(CultureInfo.InvariantCulture, ExponentialFormatString, System.Convert.ToDouble(cellText));
                    }
                    else
                        adjacentRect.Width = cellTextWidth;
                    cellTextWidth = pdfFont.MeasureString(cellText, pdfTextformat).Width;
                    cellTextHeight = pdfFont.MeasureString(cellText, pdfTextformat).Height;
                }
                else if (adjacentRect.Width >= SheetWidth && this.excelToPdfSettings.LayoutOptions == LayoutOptions.NoScaling && !workSheet.IsRightToLeft)
                {
                    cellText = this.GetDisplayText(pdfFont, pdfTextformat, cellText, new RectangleF(adjacentRect.X, adjacentRect.Y, sheetWidth, adjacentRect.Height), cell, pdfBrush, isRTL);
                    cellTextWidth = pdfFont.MeasureString(cellText, pdfTextformat).Width;
                    cellTextHeight = pdfFont.MeasureString(cellText, pdfTextformat).Height;
                }

                if (cellTextHeight > adjacentRect.Height
                    && (pdfTextformat.WordWrap == PdfWordWrapType.Word))
                {
                    cellText = this.GetDisplayText(pdfFont, pdfTextformat, cellText, adjacentRect);                    
                    adjacentRect.Height = pdfFont.MeasureString(cellText, pdfTextformat).Height;
                }
                else if(cellTextHeight > adjacentRect.Height)
                {
                    adjacentRect.Height = pdfFont.MeasureString(cellText, pdfTextformat).Height;                 
                }

                if (pdfTextformat.Alignment == PdfTextAlignment.Left)
                {
                    if (cellRect.X > adjacentRect.X)
                        adjacentRect.X = cellRect.X;
                    adjacentRect.X = adjacentRect.X + (this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeLeft])*2);                    
                }
                else if (pdfTextformat.Alignment == PdfTextAlignment.Right)
                {
                    if (!cell.HasRichText || !m_nativeFont)
                    {
                        if ((cellRect.X + cellRect.Width) < (adjacentRect.X + adjacentRect.Width))
                        {
                            adjacentRect.Width = cellRect.Width;
                        }
                        adjacentRect.X = adjacentRect.X - this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeRight]);
                    }
                    else
                    {
                        adjacentRect.X = (adjacentRect.X+(adjacentRect.Width-cellTextWidth)) - this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeRight]);
                    }
                }
                else if (pdfTextformat.Alignment == PdfTextAlignment.Center && cell.HorizontalAlignment == ExcelHAlign.HAlignCenterAcrossSelection)
                {
                    adjacentRect.X = (this.pdfPageTemplate.Size.Width / 2) - (adjacentRect.Width / 2 + leftMargin+rightMargin);
                }

                if (pdfTextformat.LineAlignment == PdfVerticalAlignment.Top)
                {
                    adjacentRect.Y += this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeTop]);
                }
                else if (pdfTextformat.LineAlignment == PdfVerticalAlignment.Bottom)
                {
                    adjacentRect.Y -= this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeBottom]);
                    IRange aboveRange= GetAdjacentRange(cell);
                    if (this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeBottom])
                        > this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeTop]))
                    {
                        adjacentRect.Y += this.GetBorderWidth(aboveRange.Borders[ExcelBordersIndex.EdgeBottom]);
                    }
                    else
                    {
                        adjacentRect.Y += this.GetBorderWidth(extendedFormatImpl.Borders[ExcelBordersIndex.EdgeTop]);
                    }
                }
#if !SyncfusionFramework4_0 && !SyncfusionFramework4_5
                if (!cell.HasRichText)
                {
                    if (pdfTextformat.RightToLeft)
                    {
                        bool enableCalculation = false;
                        
                        if (pdfTextformat.Alignment == PdfTextAlignment.Right)
                        {
                            pdfTextformat.Alignment = PdfTextAlignment.Left;
                            enableCalculation = true;
                        }
                        if (enableCalculation)
                        {
                            if (cellTextWidth < adjacentRect.Width)
                            {
                                adjacentRect.X += (adjacentRect.Width - cellTextWidth);
                            }
                            else if (adjacentRect.X > 0 && pdfTextformat.WordWrap == PdfWordWrapType.None)
                                adjacentRect.X += (cellTextWidth - adjacentRect.Width);

                            DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat,graphics);
                        }
                        else
                        {
                            DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat,graphics);
                        }
                    }
                    else
                    {
                        DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat,graphics);
                        
                        if (adjacentRect.Height > cellRect.Height && !cell.IsMerged && cell.Worksheet.TextBoxes.Count!=0)
                        {
                            graphics.DrawRectangle(new PdfSolidBrush(Color.White), new RectangleF(adjacentRect.X, cellRect.Y + cellRect.Height, adjacentRect.Width, (adjacentRect.Height - cellRect.Height)));
                        }                        
                    }
                }
                else
                {
                   
                    PdfMetafile meta = (PdfMetafile)PdfImage.FromRtf(cell.RichText.RtfText, adjacentRect.Width, adjacentRect.Height, PdfImageType.Metafile);
                    PdfMetafileLayoutFormat format = new PdfMetafileLayoutFormat();
                    if (pdfTextformat.WordWrap != PdfWordWrapType.None)
                        format.SplitTextLines = true;
                    meta.Draw(pdfGraphics,new PointF(adjacentRect.X,adjacentRect.Y));
                }
#else
                if (!cell.HasRichText)
                {
                    if (pdfTextformat.RightToLeft)
                    {
                        bool enableCalculation = false;

                        if (pdfTextformat.Alignment == PdfTextAlignment.Right)
                        {
                            pdfTextformat.Alignment = PdfTextAlignment.Left;
                            enableCalculation = true;
                        }
                        if (enableCalculation)
                        {
                            if (cellTextWidth < adjacentRect.Width)
                            {
                                adjacentRect.X += (adjacentRect.Width - cellTextWidth);
                            }
                            else if (adjacentRect.X > 0 && pdfTextformat.WordWrap == PdfWordWrapType.None)
                                adjacentRect.X += (cellTextWidth - adjacentRect.Width);

                            DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat, graphics);
                        }
                        else
                        {
                            DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat, graphics);
                        }
                    }
                    else
                    {
                        if (cellText.Length > 0 && cell.WrapText)
                        {
                            char newLineChar = cellText[cellText.Length - 1];
                            if (newLineChar == '\n')
                            {
                                cellText = cellText + " ";
                            }
                        }
                    }
                    try
                    {
                        //TODO: Added support for Specific number, Need to complete it properly.
                        //This is hard coded implement as in need of requirement in Incident 116651.
                        string numberFormat = cell.NumberFormat;
                        string currencySymbol = "$";
                        bool leftAlignCurrency = numberFormat[0] == '_';
                        if (numberFormat != null && numberFormat.Contains(currencySymbol) && cell.HasNumber)
                        {
                            string[] formats = numberFormat.Split(';');
                            int bracesIndex = numberFormat.IndexOf('[');
                            bool hasNegativePrefix = false; ;
                            string symbols = "";
                            if (bracesIndex != -1)
                            {
                                currencySymbol = "" + numberFormat[numberFormat.IndexOf('-') - 1];
                            }

                            if (cell.Number < 0)
                            {
                                if (formats.Length > 1)
                                {
                                    bracesIndex = formats[1].IndexOf('[');
                                    hasNegativePrefix = (bracesIndex == 0) ? false : (formats[1][bracesIndex - 1] == '-');
                                }
                                else
                                    hasNegativePrefix = true;
                            }
                            {

                                int currencyIndex = cellText.IndexOf(currencySymbol);
                                PdfTextAlignment alignment = pdfTextformat.Alignment;
                                if (leftAlignCurrency)
                                {
                                    cellText = cellText.Remove(currencyIndex, 1);
                                    symbols = currencySymbol;
                                }
                                if (hasNegativePrefix)
                                {
                                    cellText = cellText.Remove(cellText.IndexOf('-'), 1);
                                    symbols = '-' + symbols;
                                }
                                if (leftAlignCurrency || hasNegativePrefix)
                                {
                                    if (leftAlignCurrency)
                                    {
                                        pdfTextformat.Alignment = PdfTextAlignment.Left;
                                        Rectangle currencyRect = new Rectangle((int)adjacentRect.X + 3,
                                       (int)adjacentRect.Y,
                                      (int)adjacentRect.Width,
                                      (int)adjacentRect.Height);
                                        DrawString(symbols, pdfFont, pdfBrush, currencyRect, pdfTextformat, graphics);
                                        pdfTextformat.Alignment = alignment;
                                    }
                                    else
                                    {
                                        cellText = symbols + cellText;
                                    }

                                }
                                DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat, graphics);
                            }
                        }
                        else
                            DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat, graphics);
                    }
                    catch (Exception)
                    {
                        DrawString(cellText, pdfFont, pdfBrush, adjacentRect, pdfTextformat, graphics);
                    }

                }
                else
                {
                    cellText = PdfGraphics.NormalizeText(pdfFont, cellText);
                    PdfStringLayouter layouter = new PdfStringLayouter();
                    pdfTextformat = new PdfStringFormat();
                    PdfStringLayoutResult result = layouter.Layout(cellText, pdfFont, pdfTextformat, adjacentRect.Size);
                    result.LineCount.ToString();
                    RangeRichTextString RTF = cell.RichText as RangeRichTextString;
                    int length = RTF.TextObject.FormattingRunsCount;
                    List<IFont> richTextFont = new List<IFont>();
                    List<string> drawString = GetDrawString(cellText, RTF, out richTextFont, excelFont);
                    List<IFont> updatedRTFfont = new List<IFont>();
                    drawString = UpdateRTFValues(drawString, richTextFont, out updatedRTFfont, result);
                    richTextFont = updatedRTFfont;                    
                    float maxwidth = adjacentRect.Width;
                    float actualX = adjacentRect.X;
                    int maxLength = 0;
                    int lineCount=0;
                    int lineLength = result.Lines[lineCount].Text.Length;                    
                    adjacentRect.Height = result.LineHeight;
                    float spaceValue = pdfFont.GetLineWidth(" ", pdfTextformat);
                    for (int i = 0; i < drawString.Count; i++)
                    {
                        char newLineChar = cellText[cellText.Length - 1];
                        bool toOmit = false;
                        maxLength = maxLength + drawString[i].Length;
                        string currentString=drawString[i];
                        if (maxLength > lineLength)
                         {
                            if(lineCount + 1 <result.LineCount)
                            lineCount++;
                            adjacentRect.X = actualX;
                            adjacentRect.Y = adjacentRect.Y + cellTextHeight;
                            maxLength = 0;
                            if (String.IsNullOrWhiteSpace(currentString))
                                toOmit = true;
                            lineLength = result.Lines[lineCount].Text.Length;
                            maxLength = currentString.Length;
                         }
                        if (!toOmit)
                        {
                            string text = drawString[i];
                            cellTextColor = this.NormalizeColor(richTextFont[i].RGBColor);
                            pdfBrush = new PdfSolidBrush(cellTextColor);
                            nativeFont = ((FontImpl)richTextFont[i]).GenerateNativeFont((float)richTextFont[i].Size);
                            pdfFont = new PdfTrueTypeFont(nativeFont, true);
                            cellTextWidth = pdfFont.MeasureString(text, pdfTextformat).Width;
                            float spacewidth = 0;
                            while (currentString.EndsWith(" "))
                            {
                                spacewidth = spacewidth + spaceValue;
                                currentString = currentString.Remove(currentString.Length - 1);
                            }
                            cellTextWidth = cellTextWidth + spacewidth;
                            cellTextHeight = pdfFont.MeasureString(text, pdfTextformat).Height;
                            adjacentRect.Width = cellTextWidth;
                            adjacentRect.Height = cellTextHeight;
                            DrawString(drawString[i], pdfFont, pdfBrush, adjacentRect, pdfTextformat, graphics);
                            adjacentRect.X = adjacentRect.X + cellTextWidth;
                        }
                    }
                }
#endif
            }
            
            pdfBrush = null;
            if (cell.HasStyle && CheckCellBorderStyle(extendedFormatImpl.Borders))
                this.DrawBordersAsMSExcel(extendedFormatImpl.Borders, cellRect, graphics, cell);
            excelFont.FontName = excelFontName;
        }
        private bool CheckCellBorderStyle(IBorders borders)
        {
            if(borders[ ExcelBordersIndex.DiagonalDown].LineStyle!=ExcelLineStyle.None)
               return true;
            if (borders[ExcelBordersIndex.DiagonalUp].LineStyle != ExcelLineStyle.None)
                return true;
            if (borders[ExcelBordersIndex.EdgeBottom].LineStyle != ExcelLineStyle.None)
                return true;
            if (borders[ExcelBordersIndex.EdgeLeft].LineStyle != ExcelLineStyle.None)
                return true;
            if (borders[ExcelBordersIndex.EdgeRight].LineStyle != ExcelLineStyle.None)
                return true;
            if (borders[ExcelBordersIndex.EdgeTop].LineStyle != ExcelLineStyle.None)
                return true;
            return false;
        }

        /// <summary>
        /// Get Custom Number format Color value.
        /// </summary>
        private Color GetCustomNumberFormatColor(ExtendedFormatImpl extendedFormatImpl,IRange cell)
        {
            Color fontColor = Color.Empty;
            if (extendedFormatImpl.IncludeNumberFormat)
            {
                FormatParserImpl numFormat = extendedFormatImpl.Workbook.InnerFormats.Parser;
                FormatSectionCollection formatSec = numFormat.Parse(extendedFormatImpl.NumberFormat);
                List<FormatSection> lst = formatSec.InnerList;

                for (int i = 0; i < lst.Count; i++)
                {
                    for (int j = 0; j < lst[i].Count; j++)
                    {
                        XlsIOTokenType.TokenType tokType = lst[i][j].TokenType;
                        if (tokType == XlsIOTokenType.TokenType.Color)
                        {
                            if (lst[i].FormatType != ExcelFormatType.Number || cell.Number < 0)
                            {
                                fontColor = Color.FromName(lst[i][j].Format);
                            }
                        }
                    }
                }
            }

            return fontColor;
        }

        private void DrawString(string text, PdfFont pdfFont, PdfBrush pdfBrush, RectangleF rect, PdfStringFormat format,PdfGraphics graphics)
        {
            if (m_fitText)
            {
                float textWidth = pdfFont.MeasureString(text, format).Width;
                float ratio = rect.Width / textWidth;
                graphics.Save();
                graphics.TranslateTransform(rect.X, rect.Y);
                graphics.ScaleTransform(ratio, 1);
                graphics.DrawString(text, pdfFont, pdfBrush, new RectangleF(0, 0, textWidth, rect.Height), format);
                graphics.Restore();
            }
            else
            {
                graphics.DrawString(text, pdfFont, pdfBrush, rect, format);
            }
        }
        /// <summary>
        /// Draws the rotated text.
        /// </summary>
        /// <param name="adjacentRect">The adjacent cell rect.</param>
        /// <param name="extendedFormatImpl">The extended format impl of the cell.</param>
        /// <param name="cell">The cell range.</param>
        /// <param name="value">The cell display text.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="nativeFont">The native font.</param>
        /// <param name="brush">The pdf brush.</param>
        /// <param name="format">The pdf string format.</param>
        /// <param name="firstCharWidth">First char width of the display text .</param>
        /// <param name="totalWidth">The total width of the display text.</param>
        /// <param name="lastCharSize">Last char size of the display text .</param>
        private void DrawRotatedText(RectangleF adjacentRect, ExtendedFormatImpl extendedFormatImpl, IRange cell,
                                     string value, PdfGraphics graphics, PdfFont nativeFont, PdfBrush brush,
                                     PdfStringFormat format, SizeF firstCharWidth, float totalWidth,
                                     SizeF lastCharSize)
        {
            this.cellDisplayText = value;
            WorksheetImpl sheet = cell.Worksheet as WorksheetImpl;
            int rotationAngle = this.GetCounterClockwiseRotation(extendedFormatImpl.Rotation);
            PdfGraphicsState state = graphics.Save();
            PointF coordinates = this.GetVector(rotationAngle, totalWidth, adjacentRect, extendedFormatImpl,
                                                lastCharSize, firstCharWidth, nativeFont, format);
            graphics.TranslateTransform(coordinates.X, coordinates.Y);

            graphics.RotateTransform((float)rotationAngle);
            graphics.DrawString(this.cellDisplayText, nativeFont, brush, PointF.Empty);
            graphics.Restore(state);
        }

        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="borders">The border collections of the cell.</param>
        /// <param name="rect">The rectangle of the cell.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="cell">The cell range.</param>
        private void DrawBorders(IBorders borders, RectangleF rect, PdfGraphics graphics, IRange cell)
        {
            IBorder border;
            Color leftColor = Color.Empty;
            Color rightColor = Color.Empty;
            Color topColor = Color.Empty;
            Color bottomColor = Color.Empty;
            if (this.workSheet.ListObjects.Count !=0 && this.tableBorderColorList != null)
            {
                if (this.tableBorderColorList.Count != 0)
                {
                    foreach (KeyValuePair<IRange, Dictionary<ExcelBordersIndex, Color>> tableBorderColors in this.tableBorderColorList)
                    {
                        if (this.CheckRange(tableBorderColors.Key, cell))
                        {
                            if (leftColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeLeft, out leftColor);
                            }

                            if (rightColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeRight, out rightColor);
                            }

                            if (topColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeTop, out topColor);
                            }

                            if (bottomColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeBottom, out bottomColor);
                            }
                        }
                    }
                }
            }
            ExcelBordersIndex index;
            border = borders[ExcelBordersIndex.EdgeLeft];
            index= ExcelBordersIndex.EdgeLeft;
            if (this.workSheet.IsRightToLeft)
            {
                border = borders[ExcelBordersIndex.EdgeRight];
                index= ExcelBordersIndex.EdgeRight;
            }

            this.DrawBorder(borders, border, rect.Left, rect.Top, rect.Left, rect.Bottom, graphics, cell,
                            leftColor,index);

            border = borders[ExcelBordersIndex.EdgeTop];
            index = ExcelBordersIndex.EdgeTop;
            this.DrawBorder(borders, border, rect.Left, rect.Top, rect.Right, rect.Top, graphics, cell, topColor,index);

            border = borders[ExcelBordersIndex.EdgeRight];
            index = ExcelBordersIndex.EdgeRight;
            if (this.workSheet.IsRightToLeft)
            {
                border = borders[ExcelBordersIndex.EdgeLeft];
                index = ExcelBordersIndex.EdgeLeft;
            }

            this.DrawBorder(borders, border, rect.Right, rect.Top, rect.Right, rect.Bottom, graphics, cell,
                            rightColor,index);         

            border = borders[ExcelBordersIndex.EdgeBottom];
            index = ExcelBordersIndex.EdgeBottom;
            this.DrawBorder(borders, border, rect.Left, rect.Bottom, rect.Right, rect.Bottom, graphics, cell,
                            bottomColor,index);

            border = borders[ExcelBordersIndex.DiagonalDown];
            index = ExcelBordersIndex.DiagonalDown;
            if (border.ShowDiagonalLine)
            {
                this.DrawBorder(borders, border, rect.Left, rect.Top, rect.Right, rect.Bottom, graphics, cell,
                                Color.Empty,index);
            }

            border = borders[ExcelBordersIndex.DiagonalUp];
            index = ExcelBordersIndex.DiagonalUp;
            if (border.ShowDiagonalLine)
            {
                this.DrawBorder(borders, border, rect.Left, rect.Bottom, rect.Right, rect.Top, graphics, cell,
                                Color.Empty,index);
            }
        }

        /// <summary>
        /// Draws the borders as MS Excel.
        /// </summary>
        /// <param name="borders">The border collections of the cell.</param>
        /// <param name="rect">The rectangle of the cell.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="cell">The cell range.</param>
        private void DrawBordersAsMSExcel(IBorders borders, RectangleF rect, PdfGraphics graphics, IRange cell)
        {
            IBorder border;
            Color leftColor = Color.Empty;
            Color rightColor = Color.Empty;
            Color topColor = Color.Empty;
            Color bottomColor = Color.Empty;
            if (this.workSheet.ListObjects.Count != 0 && this.tableBorderColorList != null)
            {
                if (this.tableBorderColorList.Count != 0)
                {
                    foreach (KeyValuePair<IRange, Dictionary<ExcelBordersIndex, Color>> tableBorderColors in this.tableBorderColorList)
                    {
                        if (this.CheckRange(tableBorderColors.Key, cell))
                        {
                            if (leftColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeLeft, out leftColor);
                            }

                            if (rightColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeRight, out rightColor);
                            }

                            if (topColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeTop, out topColor);
                            }

                            if (bottomColor.IsEmpty)
                            {
                                tableBorderColors.Value.TryGetValue(ExcelBordersIndex.EdgeBottom, out bottomColor);
                            }
                        }
                    }
                }
            }

            SortBorderLineStyle();
            FillBorderLineStyle(borders);

            List<ExcelBordersIndex> sourceIndexLists = new List<ExcelBordersIndex>();
            List<ExcelBordersIndex> destIndexLists = new List<ExcelBordersIndex>();

            ExcelLineStyle[] lineStyles = new ExcelLineStyle[m_dicBorderLineStyle.Count];
            m_dicBorderLineStyle.Keys.CopyTo(lineStyles, 0);
            foreach(ExcelLineStyle lineStyle in lineStyles)
            {                
                    if (m_dicBorderLineStyle[lineStyle].Count > 1)
                    {
                        m_sortedBorders.Clear();
                        sourceIndexLists = m_dicBorderLineStyle[lineStyle];

                        foreach (ExcelBordersIndex bIndex in sourceIndexLists)
                        {
                            BorderColorContrast(borders, bIndex);
                        }

                        if (m_sortedBorders != null && m_sortedBorders.Count > 0)
                        {
                            SortDictionaryByValue();

                            foreach (ExcelBordersIndex borderIndex in m_sortedBorders.Keys)
                            {
                                destIndexLists.Add(borderIndex);
                            }
                        }

                        m_dicBorderLineStyle[lineStyle].Clear();
                        m_dicBorderLineStyle[lineStyle] = destIndexLists;
                    }                
            }

            int i = 1;
            bool isFirstBorder = false;
            ExcelBordersIndex fillEdge = 0;

            foreach (ExcelLineStyle lineStyle in m_dicBorderLineStyle.Keys)
            {
                if (m_dicBorderLineStyle[lineStyle].Count > 0)
                {
                    if (m_dicBorderLineStyle[lineStyle].Count == 1)
                    {
                        if (i == 1)
                            isFirstBorder = true;
                        else
                            isFirstBorder = false;

                        ExcelBordersIndex borderIndex = m_dicBorderLineStyle[lineStyle][0];
                        border = borders[borderIndex];
                        DrawAllBorders(borders, border, rect, graphics, cell, Color.Empty, borderIndex, leftColor, rightColor, topColor, bottomColor, isFirstBorder, fillEdge);

                        i++;
                        fillEdge |=borderIndex;
                    }
                    else
                    {
                        foreach (ExcelBordersIndex borderIndex in m_dicBorderLineStyle[lineStyle])
                        {
                            if (i == 1)
                                isFirstBorder = true;
                            else
                                isFirstBorder = false;

                            border = borders[borderIndex];
                            DrawAllBorders(borders, border, rect, graphics, cell, Color.Empty, borderIndex, leftColor, rightColor, topColor, bottomColor, isFirstBorder, fillEdge);
                            fillEdge |= borderIndex;
                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sort border line style.
        /// </summary>
        private void SortBorderLineStyle()
        {
            m_dicBorderLineStyle.Clear();

            m_dicBorderLineStyle.Add(ExcelLineStyle.Double, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Hair, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Dotted, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Dash_dot_dot, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Dash_dot, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Dashed, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Thin, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Medium_dash_dot_dot, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Slanted_dash_dot, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Medium_dash_dot, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Medium_dashed, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Medium, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.Thick, new List<ExcelBordersIndex>());
            m_dicBorderLineStyle.Add(ExcelLineStyle.None, new List<ExcelBordersIndex>());
        }

        /// <summary>
        /// Fill border line style.
        /// </summary>
        private void FillBorderLineStyle(IBorders borders)
        {
            List<ExcelBordersIndex> indexList = new List<ExcelBordersIndex>();
            ExcelLineStyle lineStyle = ExcelLineStyle.None;
            List<ExcelLineStyle> line_Style = new List<ExcelLineStyle>();
            
            int count = 0;
            lineStyle=borders[ExcelBordersIndex.EdgeLeft].LineStyle;
            if (lineStyle != ExcelLineStyle.None && m_dicBorderLineStyle.ContainsKey(lineStyle))
            {
                indexList = new List<ExcelBordersIndex>();
                m_dicBorderLineStyle.TryGetValue(lineStyle, out indexList);
                indexList.Add(ExcelBordersIndex.EdgeLeft);
                m_dicBorderLineStyle[lineStyle] = indexList;
                count++;
                line_Style.Add(lineStyle);
                
            }

            //top border
            lineStyle = borders[ExcelBordersIndex.EdgeTop].LineStyle;
            if (lineStyle != ExcelLineStyle.None && m_dicBorderLineStyle.ContainsKey(lineStyle))
            {
                indexList = new List<ExcelBordersIndex>();
                m_dicBorderLineStyle.TryGetValue(lineStyle, out indexList);
                indexList.Add(ExcelBordersIndex.EdgeTop);
                m_dicBorderLineStyle[lineStyle] = indexList;
                count++; 
                if(!line_Style.Contains(lineStyle))
                line_Style.Add(lineStyle);
            }

            //Right border
            lineStyle = borders[ExcelBordersIndex.EdgeRight].LineStyle;
            if (borders[ExcelBordersIndex.EdgeRight].LineStyle != ExcelLineStyle.None && m_dicBorderLineStyle.ContainsKey(borders[ExcelBordersIndex.EdgeRight].LineStyle))
            {
                indexList = new List<ExcelBordersIndex>();
                m_dicBorderLineStyle.TryGetValue(lineStyle, out indexList);
                indexList.Add(ExcelBordersIndex.EdgeRight);
                m_dicBorderLineStyle[lineStyle] = indexList;
                count++;
                if (!line_Style.Contains(lineStyle))
                    line_Style.Add(lineStyle);
            }

            //Bottom border
            lineStyle = borders[ExcelBordersIndex.EdgeBottom].LineStyle;
            if (lineStyle != ExcelLineStyle.None && m_dicBorderLineStyle.ContainsKey(lineStyle))
            {
                indexList = new List<ExcelBordersIndex>();
                m_dicBorderLineStyle.TryGetValue(lineStyle, out indexList);
                indexList.Add(ExcelBordersIndex.EdgeBottom);
                m_dicBorderLineStyle[lineStyle] = indexList;
                count++;
                if (!line_Style.Contains(lineStyle))
                    line_Style.Add(lineStyle);
            }

            //Diagonal down border
            lineStyle = borders[ExcelBordersIndex.DiagonalDown].LineStyle;
            if (lineStyle != ExcelLineStyle.None && m_dicBorderLineStyle.ContainsKey(lineStyle))
            {
                indexList = new List<ExcelBordersIndex>();
                m_dicBorderLineStyle.TryGetValue(lineStyle, out indexList);
                indexList.Add(ExcelBordersIndex.DiagonalDown);
                m_dicBorderLineStyle[lineStyle] = indexList;
                count++;
                if (!line_Style.Contains(lineStyle))
                    line_Style.Add(lineStyle);
            }

            //Diagonal up border
            lineStyle = borders[ExcelBordersIndex.DiagonalUp].LineStyle;
            if (lineStyle != ExcelLineStyle.None && m_dicBorderLineStyle.ContainsKey(lineStyle))
            {
                indexList = new List<ExcelBordersIndex>();
                m_dicBorderLineStyle.TryGetValue(lineStyle, out indexList);
                indexList.Add(ExcelBordersIndex.DiagonalUp);
                m_dicBorderLineStyle[lineStyle] = indexList;
                count++;
                if (!line_Style.Contains(lineStyle))
                    line_Style.Add(lineStyle);
            }
            if (line_Style.Count == 1)
            {
                Dictionary<ExcelLineStyle, List<ExcelBordersIndex>> result=new Dictionary<ExcelLineStyle,List<ExcelBordersIndex>>();
                result.Add(line_Style[0],m_dicBorderLineStyle[line_Style[0]]);
                m_dicBorderLineStyle.Clear();
                m_dicBorderLineStyle = result;
            }
            else
            {
                m_dicBorderLineStyle = RemoveUnwantedBorderStyle(m_dicBorderLineStyle, count);
            }
                
        }
        private Dictionary<ExcelLineStyle, List<ExcelBordersIndex>> RemoveUnwantedBorderStyle(Dictionary<ExcelLineStyle, List<ExcelBordersIndex>> m_dicBorderLineStyle, int count)
        {
            ExcelLineStyle[] lineStyles = new ExcelLineStyle[m_dicBorderLineStyle.Count];
            Dictionary<ExcelLineStyle, List<ExcelBordersIndex>> result=new Dictionary<ExcelLineStyle,List<ExcelBordersIndex>>();
            m_dicBorderLineStyle.Keys.CopyTo(lineStyles, 0);
            foreach(ExcelLineStyle style in lineStyles)
            {
                if (count > 0)
                {
                    if ((m_dicBorderLineStyle[style].Count > 0))
                    {
                        result.Add(style, m_dicBorderLineStyle[style]);
                        count -= m_dicBorderLineStyle[style].Count;
                    }
                }
                else
                {
                   return result;                   
                }
            }
            return result;
        }
        /// <summary>
        /// Calculate the color contrast of the cell borders.
        /// </summary>
        /// <param name="borders">The border collections of the cell.</param>
        /// <param name="index">Border index.</param>
        private void BorderColorContrast(IBorders borders,ExcelBordersIndex index)
        {
            double contrast = 0;
            contrast = 1 - (0.3 * borders[index].ColorRGB.R + 0.59 * borders[index].ColorRGB.G + 0.11 * borders[index].ColorRGB.B)/255;

            if (!m_sortedBorders.ContainsKey(index))
                m_sortedBorders.Add(index, contrast);
        }

        /// <summary>
        /// Sort the dictionary by its values.
        /// </summary>
        private void SortDictionaryByValue()
        {
            List<KeyValuePair<ExcelBordersIndex, double>> tempList = new List<KeyValuePair<ExcelBordersIndex, double>>(m_sortedBorders);            

            tempList.Sort(delegate(KeyValuePair<ExcelBordersIndex, double> firstPair, KeyValuePair<ExcelBordersIndex, double> secondPair)
            {
                return firstPair.Value.CompareTo(secondPair.Value);
            });

            m_sortedBorders.Clear();
            foreach (KeyValuePair<ExcelBordersIndex, double> pair in tempList)
            {
                m_sortedBorders.Add(pair.Key, pair.Value);
            }
        }
        /// <summary>
        /// Draw all border lines.
        /// </summary>
        private void DrawAllBorders(IBorders borders, IBorder border, RectangleF rect, PdfGraphics graphics, IRange cell, Color borderColor, 
            ExcelBordersIndex borderIndex,Color leftColor,Color rightColor,Color topColor,Color bottomColor, bool isFirstBorder, ExcelBordersIndex fillEdge)
        {
            switch (borderIndex)
            {
                case ExcelBordersIndex.EdgeLeft:

                    if (this.workSheet.IsRightToLeft)
                        border = borders[ExcelBordersIndex.EdgeRight];

                    this.DrawSingleBorder(borders, border, rect.Left, rect.Top, rect.Left, rect.Bottom, graphics, cell,
                        leftColor, borderIndex, isFirstBorder, fillEdge);

                    break;

                case ExcelBordersIndex.EdgeTop:
                    this.DrawSingleBorder(borders, border, rect.Left, rect.Top, rect.Right, rect.Top, graphics, cell, topColor, borderIndex, isFirstBorder, fillEdge);
                    break;

                case ExcelBordersIndex.EdgeRight:

                    if (this.workSheet.IsRightToLeft)
                        border = borders[ExcelBordersIndex.EdgeLeft];

                    this.DrawSingleBorder(borders, border, rect.Right, rect.Top, rect.Right, rect.Bottom, graphics, cell,
                        rightColor, borderIndex, isFirstBorder, fillEdge);
                    break;

                case ExcelBordersIndex.EdgeBottom:
                    this.DrawSingleBorder(borders, border, rect.Left, rect.Bottom, rect.Right, rect.Bottom, graphics, cell,
                        bottomColor, borderIndex, isFirstBorder, fillEdge);
                    break;

                case ExcelBordersIndex.DiagonalDown:
                    if (border.ShowDiagonalLine)
                        this.DrawSingleBorder(borders, border, rect.Left, rect.Top, rect.Right, rect.Bottom, graphics, cell,
                            Color.Empty, borderIndex, isFirstBorder, fillEdge);
                    break;

                case ExcelBordersIndex.DiagonalUp:
                    if (border.ShowDiagonalLine)
                        this.DrawSingleBorder(borders, border, rect.Left, rect.Bottom, rect.Right, rect.Top, graphics, cell,
                            Color.Empty, borderIndex, isFirstBorder, fillEdge);
                    break;
            }            
        }

        /// <summary>
        /// Draw single border.
        /// </summary>
        /// <param name="borders">The borders collection of the cell.</param>
        /// <param name="border">The cell Excel border indez.</param>
        /// <param name="x1">The x1 point of the border line.</param>
        /// <param name="y1">The y1 point of the border line.</param>
        /// <param name="x2">The x2 point of the border line.</param>
        /// <param name="y2">The y2 point of the border line.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="cell">The cell range.</param>
        /// <param name="borderColor">Color of the cell border.</param>
        /// <param name="index">border index.</param>
        /// <param name="isFirstBorder">check is this is first border.</param>
        /// <param name="fillEdge">Border index to fill edge.</param>
        private void DrawSingleBorder(IBorders borders, IBorder border, float x1, float y1, float x2, float y2,
                                PdfGraphics graphics, IRange cell, Color borderColor, ExcelBordersIndex index, bool isFirstBorder, ExcelBordersIndex fillEdge)
        {
            bool canDrawBorder = false;
            bool isTop = (index == ExcelBordersIndex.EdgeTop) ? true : false;
            if (border.LineStyle != ExcelLineStyle.None)
            {
                canDrawBorder = CanDrawDorder(borders, border, cell, index, isTop);
            }

            if (index == ExcelBordersIndex.EdgeLeft || index == ExcelBordersIndex.EdgeRight || index == ExcelBordersIndex.DiagonalUp || index == ExcelBordersIndex.DiagonalDown)
            {
                canDrawBorder = true;
            }
            if (border.LineStyle == ExcelLineStyle.Double && canDrawBorder)
            {
                this.DrawDoubleBorder(borders, border, x1, y1, x2, y2, graphics, cell, borderColor);
            }
            else if (canDrawBorder)
            {
                PdfPen pen = this.CreatePen(border, borderColor);
                float incr=0;
                if (pen.Width != 0.5)
                    incr = pen.Width / 2;

                if (isFirstBorder)
                {
                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }
                else
                {
                    switch (index)
                    {
                        case ExcelBordersIndex.EdgeLeft:
                        case ExcelBordersIndex.EdgeRight:
                            
                            bool isTopDrawn = (fillEdge & ExcelBordersIndex.EdgeTop) == ExcelBordersIndex.EdgeTop;
                            bool isBottomDrawn = (fillEdge & ExcelBordersIndex.EdgeBottom) == ExcelBordersIndex.EdgeBottom;

                            if (isTopDrawn)
                                y1 -= incr;
                            if (isBottomDrawn)
                                y2 += incr;

                            graphics.DrawLine(pen, x1, y1, x2, y2);                           
                            break;

                        case ExcelBordersIndex.EdgeTop:
                        case ExcelBordersIndex.EdgeBottom:

                            bool isLeftDrawn = (fillEdge & ExcelBordersIndex.EdgeLeft) == ExcelBordersIndex.EdgeLeft;
                            bool isRightDrawn = (fillEdge & ExcelBordersIndex.EdgeRight) == ExcelBordersIndex.EdgeRight;

                            if (isLeftDrawn)
                                x1 -= incr;
                            if (isRightDrawn)
                                x2 += incr;

                            graphics.DrawLine(pen, x1, y1, x2, y2);                        
                            break;

                        case ExcelBordersIndex.DiagonalDown:
                        case ExcelBordersIndex.DiagonalUp:
                            graphics.DrawLine(pen, x1, y1, x2, y2);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the border.
        /// </summary>
        /// <param name="borders">The borders collection of the cell.</param>
        /// <param name="border">The cell Excel border indez.</param>
        /// <param name="x1">The x1 point of the border line.</param>
        /// <param name="y1">The y1 point of the border line.</param>
        /// <param name="x2">The x2 point of the border line.</param>
        /// <param name="y2">The y2 point of the border line.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="cell">The cell range.</param>
        /// <param name="borderColor">Color of the cell border.</param>
        private void DrawBorder(IBorders borders, IBorder border, float x1, float y1, float x2, float y2,
                                PdfGraphics graphics, IRange cell, Color borderColor,ExcelBordersIndex index)
        {
            bool canDrawBorder=false;
            bool isTop = (index == ExcelBordersIndex.EdgeTop) ? true : false;
            if (border.LineStyle != ExcelLineStyle.None)
            {
                canDrawBorder = CanDrawDorder(borders, border, cell, index,isTop);
            }

            if (index == ExcelBordersIndex.EdgeLeft || index == ExcelBordersIndex.EdgeRight || index == ExcelBordersIndex.DiagonalUp || index == ExcelBordersIndex.DiagonalDown)
            {
                canDrawBorder = true;
            }
            if (border.LineStyle == ExcelLineStyle.Double && canDrawBorder )
            {
                this.DrawDoubleBorder(borders, border, x1, y1, x2, y2, graphics, cell, borderColor);
            }
            else if(canDrawBorder)
           {                 
                this.DrawOrdinaryBorder(border, x1, y1, x2, y2, graphics, borderColor, index);             
            }
        }
        private bool CanDrawDorder(IBorders borders, IBorder border, IRange cell, ExcelBordersIndex index,bool isTop)
        {
            IWorksheet sheet = cell.Worksheet;
            int row = cell.Row;
            int column = cell.Column;
            int maxrow = sheet.Workbook.MaxRowCount;
            int maxcol = sheet.Workbook.MaxColumnCount;
            IBorder adjacentBorder = borders[index];
            IRange cell2 = null;

            (cell.CellStyle as CellStyle).AskAdjacent = false;

            if (isTop && row - 1 > 0)
            {
                cell2 = sheet[row - 1, column];
                (cell2.CellStyle as CellStyle).AskAdjacent = false;
                if (cell.Borders[ExcelBordersIndex.EdgeTop].LineStyle == ExcelLineStyle.Double)
                    return true;
                IBorders adjacentBorders = cell2.Borders;
                adjacentBorder = adjacentBorders[ExcelBordersIndex.EdgeBottom];
            }
           else if (row + 1 < maxrow)
            {
                cell2 = sheet[row + 1, column];
                (cell2.CellStyle as CellStyle).AskAdjacent = false;
                if (cell.Borders[ExcelBordersIndex.EdgeBottom].LineStyle == ExcelLineStyle.Double)
                    return true;
                IBorders adjacentBorders = cell2.Borders;
                adjacentBorder = adjacentBorders[ExcelBordersIndex.EdgeTop];
            }
            else
                return false;

            bool isValidLineStyle = (adjacentBorder.LineStyle == ExcelLineStyle.None ||cell.Borders[ExcelBordersIndex.EdgeBottom].LineStyle != ExcelLineStyle.Double );
            (cell.CellStyle as CellStyle).AskAdjacent = true;
            if (cell2 != null) (cell2.CellStyle as CellStyle).AskAdjacent = true;

            return isValidLineStyle;
         }

        /// <summary>
        /// Draws the double border.
        /// </summary>
        /// <param name="borders">The borders collection of the cell.</param>
        /// <param name="border">The Excel cell border Index.</param>
        /// <param name="x1">The x1 point of the border line.</param>
        /// <param name="y1">The y1 point of the border line.</param>
        /// <param name="x2">The x2 point of the border line.</param>
        /// <param name="y2">The y2 point of the border line.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="cell">The cell range.</param>
        /// <param name="borderColor">Color of the cell border.</param>
        private void DrawDoubleBorder(IBorders borders, IBorder border, float x1, float y1, float x2, float y2,
                                      PdfGraphics graphics, IRange cell, Color borderColor)
        {
            PdfPen pen = this.CreatePen(border, borderColor);
            int deltaX;
            int deltaY;

            BorderImpl borderImpl = border as BorderImpl;
            ExcelBordersIndex borderIndex = borderImpl.BorderIndex;

            switch (borderIndex)
            {
                case ExcelBordersIndex.EdgeBottom:
                    deltaX = 0;
                    deltaY = 1;
                    break;

                case ExcelBordersIndex.EdgeLeft:
                    deltaX = -1;
                    deltaY = 0;
                    break;

                case ExcelBordersIndex.EdgeRight:
                    deltaX = 1;
                    deltaY = 0;
                    break;

                case ExcelBordersIndex.EdgeTop:
                    deltaX = 0;
                    deltaY = -1;
                    break;

                default:
                    // maybe different diagonal borders should differ.
                    deltaX = 1;
                    deltaY = 1;
                    break;
            }

            this.DrawInnerBorderLine(graphics, pen, borders, borderIndex, x1, y1, x2, y2, deltaX, deltaY, cell);
            this.DrawOuterBorderLine(graphics, pen, borderIndex, x1, y1, x2, y2, deltaX, deltaY, cell);
        }

        /// <summary>
        /// Draws the outer line.
        /// </summary>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="pen">The pdf pen to draw the border.</param>
        /// <param name="borderIndex">Index of the border .</param>
        /// <param name="x1">The x1 point of the border line.</param>
        /// <param name="y1">The y1 point of the border line.</param>
        /// <param name="x2">The x2 point of the border line.</param>
        /// <param name="y2">The y2 point of the border line.</param>
        /// <param name="deltaX">The delta X.</param>
        /// <param name="deltaY">The delta Y.</param>
        /// <param name="cell">The cell range.</param>
        private void DrawOuterBorderLine(PdfGraphics graphics, PdfPen pen, ExcelBordersIndex borderIndex, float x1,
                                    float y1, float x2, float y2, int deltaX, int deltaY, IRange cell)
        {
            ExcelBordersIndex start;
            ExcelBordersIndex end;
            this.GetStartEndBorderIndex(borderIndex, out start, out end);

            int deltaX1 = deltaX;
            int deltaX2 = deltaX;
            int deltaY1 = deltaY;
            int deltaY2 = deltaY;
            int rowIndex = cell.Row + deltaY;
            int columnIndex = cell.Column + deltaX;
            if (columnIndex == 0)
            {
                columnIndex = 1;
            }

            if (rowIndex == 0)
            {
                rowIndex = 1;
            }

            IBorders adjacentBorders = cell.Worksheet[rowIndex, columnIndex].Borders;

            this.UpdateBorderDelta(cell.Worksheet, rowIndex, columnIndex, deltaX, deltaY, ref deltaX1, ref deltaY1,
                              true, adjacentBorders, end, start, true);

            this.UpdateBorderDelta(cell.Worksheet, rowIndex, columnIndex, deltaY, deltaY, ref deltaX2, ref deltaY2,
                              true, adjacentBorders, start, end, false);

            graphics.DrawLine(pen, x1 + deltaX1, y1 + deltaY1, x2 + deltaX2, y2 + deltaY2);
        }

        /// <summary>
        /// Draws the inner line.
        /// </summary>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="pen">The pdf pen to draw the border.</param>
        /// <param name="borders">The borders collection of the cell.</param>
        /// <param name="borderIndex">Index of the border .</param>
        /// <param name="x1">The x1 point of the border line.</param>
        /// <param name="y1">The y1 point of the border line.</param>
        /// <param name="x2">The x2 point of the border line.</param>
        /// <param name="y2">The y2 point of the border line.</param>
        /// <param name="deltaX">The delta X.</param>
        /// <param name="deltaY">The delta Y.</param>
        /// <param name="cell">The cell range.</param>
        private void DrawInnerBorderLine(PdfGraphics graphics, PdfPen pen, IBorders borders,
                                    ExcelBordersIndex borderIndex, float x1, float y1, float x2, float y2,
                                    int deltaX, int deltaY, IRange cell)
        {
            ExcelBordersIndex start;
            ExcelBordersIndex end;
            this.GetStartEndBorderIndex(borderIndex, out start, out end);

            int deltaX1 = deltaX;
            int deltaX2 = deltaX;
            int deltaY1 = deltaY;
            int deltaY2 = deltaY;
            int firstRow = cell.Row;
            int firstColumn = cell.Column;
            IWorksheet sheet = cell.Worksheet;

            this.UpdateBorderDelta(sheet, firstRow, firstColumn, -deltaX, -deltaY, ref deltaX1, ref deltaY1, false,
                              borders, end, start, true);

            this.UpdateBorderDelta(sheet, firstRow, firstColumn, -deltaX, -deltaY, ref deltaX2, ref deltaY2, false,
                              borders, start, end, false);

            graphics.DrawLine(pen, x1 - deltaX1, y1 - deltaY1, x2 - deltaX2, y2 - deltaY2);
        }

        /// <summary>
        /// Draws the ordinary border.
        /// </summary>
        /// <param name="border">The Excel border index.</param>
        /// <param name="x1">The x1 point of the border line.</param>
        /// <param name="y1">The y1 point of the border line.</param>
        /// <param name="x2">The x2 point of the border line.</param>
        /// <param name="y2">The y2 point of the border line.</param>
        /// <param name="graphics">The pdf page graphics.</param>
        /// <param name="borderColor">Color of the border.</param>
        private void DrawOrdinaryBorder(IBorder border, float x1, float y1, float x2, float y2,
                                        PdfGraphics graphics, Color borderColor,ExcelBordersIndex index)
        {
            if (border.LineStyle != ExcelLineStyle.None)
            {
                PdfPen pen = this.CreatePen(border, borderColor);
                if (pen.Width != 0.5)
                {
                    float incr = pen.Width / 2;

                    switch (index)
                    {                        
                        case ExcelBordersIndex.EdgeTop:
                            x1 -= incr;
                            x2 -= incr;
                            break;
                        case ExcelBordersIndex.EdgeRight:
                            y1 -= incr;
                            break;
                        case ExcelBordersIndex.EdgeBottom:
                            x1 -= incr;
                            x2 += incr;
                            break;
                    }
                }
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        /// <summary>
        /// Draws the images.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <returns>Return the Pdf document with the image.</returns>
        private PdfDocument DrawImages(WorksheetImpl sheet)
        {
            IPictures pictures = sheet.Pictures;

            List<int> maxHeightValue = new List<int>();
            List<int> maxWidthValue = new List<int>();

            float sheetHeight = this.pdfDocument.PageSettings.Height -
                               (this.pdfDocument.PageSettings.Margins.Top +
                                this.pdfDocument.PageSettings.Margins.Bottom);

            float sheetWidth = this.pdfDocument.PageSettings.Width -
                              (this.pdfDocument.PageSettings.Margins.Left +
                               this.pdfDocument.PageSettings.Margins.Right);

            int maxHeight, maxWidth = 0;

            for (int i = 0, length = pictures.Count; i < length; i++)
            {
                IPictureShape picture = pictures[i];
                maxHeightValue.Add(picture.Top + picture.Height);
                maxWidthValue.Add(picture.Left + picture.Width);
            }

            maxHeight = this.GetMaxValue(maxHeightValue);
            maxWidth = this.GetMaxValue(maxWidthValue);

            Image image = new Bitmap(maxWidth, maxHeight);
            //Drawing all images to a single bitmap image
            using (Graphics systemGraphics = Graphics.FromImage(image))
            {
                systemGraphics.FillRectangle(Brushes.White, new Rectangle(0, 0, maxWidth, maxHeight));
                foreach (IPictureShape picture in pictures)
                {
                    systemGraphics.DrawImage(picture.Picture, new Rectangle(picture.Left, picture.Top,
                                             picture.Width, picture.Height));
                }
            }

            //Pdf document rendering.
            PdfPage imagePage = this.pdfDocument.Pages.Add();
            PdfGraphics pdfGraphics = imagePage.Graphics;
            Image scaledImage = this.GetScaledPicture(image, (int)sheetWidth, (int)sheetHeight);
            PdfImage pdfImage = new PdfBitmap(scaledImage);
            pdfGraphics.DrawImage(pdfImage, new RectangleF(0, 0, (float)scaledImage.Width,
                                 (float)scaledImage.Height));
            return this.pdfDocument;
        }

        /// <summary>
        /// Draws all necessary images.
        /// </summary>
        /// <param name="sheet">Worksheet that is being converted into image.</param>
        /// <param name="graphics">PdfGraphics to draw cells at.</param>
        /// <param name="firstRow">One-based index of the first row to convert.</param>
        /// <param name="firstColumn">One-based index of the first column to convert.</param>
        /// <param name="lastRow">One-based index of the last row to convert.</param>
        /// <param name="lastColumn">One-based index of the last column to convert.</param>
        /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
        /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
        private void DrawImages(WorksheetImpl sheet, PdfGraphics graphics, int firstRow, int firstColumn,
                                int lastRow, int lastColumn,float startX,float startY)
        {
            if (sheet.HasPictures)
            {
                IPictures pictures = sheet.Pictures;
                float x = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(firstColumn - 1),
                                                                PdfGraphicsUnit.Point);
                float y = Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(firstRow - 1),
                                                                PdfGraphicsUnit.Point);
                float x2 = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(lastColumn),
                                                                 PdfGraphicsUnit.Point);
                float y2 = Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(lastRow),
                                                                 PdfGraphicsUnit.Point);
                x += startX;
                y += startY;
                float newx = x;
                float newy = y;
                for (int i = 0, len = pictures.Count; i < len; i++)
                {
                    IPictureShape picture = pictures[i];
                    ShapeImpl pictureShape = picture as ShapeImpl;
                    bool isBetween;
                    bool toDraw=Drawpicture(firstRow, lastRow, firstColumn, lastColumn, pictureShape, out isBetween);
                    if (picture.Picture != null && toDraw && ((float)picture.Left < pdfPageTemplate.Width || (firstColumn <= pictureShape.LeftColumn || lastColumn >= pictureShape.RightColumn) || isBetween))
                    {
                         // Is image visible?
                         if ((Pdf_UnitConverter.ConvertFromPixels(picture.Top, PdfGraphicsUnit.Point) <= y2 &&
                          Pdf_UnitConverter.ConvertFromPixels((float)picture.Top + picture.Height,
                                                                PdfGraphicsUnit.Point) >= y &&
                           Pdf_UnitConverter.ConvertFromPixels((float)picture.Left, PdfGraphicsUnit.Point) <= x2 &&
                           Pdf_UnitConverter.ConvertFromPixels((float)picture.Left + picture.Width,
                                                                 PdfGraphicsUnit.Point) >= x) ||
                          (Pdf_UnitConverter.ConvertFromPixels(picture.Top, PdfGraphicsUnit.Point) <= this.currentPage.Size.Height &&
                            Pdf_UnitConverter.ConvertFromPixels((float)picture.Left, PdfGraphicsUnit.Point) <= this.currentPage.Size.Width))
                         {

                             RectangleF pictureRect = new RectangleF(Pdf_UnitConverter.ConvertFromPixels((float)picture.Left, PdfGraphicsUnit.Point),
                                                                     Pdf_UnitConverter.ConvertFromPixels((float)picture.Top, PdfGraphicsUnit.Point),
                                                                     Pdf_UnitConverter.ConvertFromPixels((float)picture.Width, PdfGraphicsUnit.Point),
                                                                     Pdf_UnitConverter.ConvertFromPixels((float)picture.Height, PdfGraphicsUnit.Point));                             
                             if (this.workSheet.IsRightToLeft && ( firstRow <=pictureShape.TopRow && lastRow >=pictureShape.BottomRow) )
                             {
                                 newx = pdfPageTemplate.Width - (pictureRect.Width + pictureRect.Left + startX);
                                 if (pictureRect.Y < newx || picture.Top>picture.Left)
                                 {
                                     newx -= pictureRect.Left;
                                     newy = startY;
                                 }
                                 if (pictureRect.Y > y && this.pdfDocument.Pages.Count>1)
                                 {                                     
                                     pictureRect.Y = pictureRect.Y - (newx+ newx/2-pictureRect.Height);
                                     newy = startY;
                                 }                                
                                 pictureRect.Offset(newx, newy);                      
                             }
                             else
                             {
                                 if (y > 0)
                                 {
                                     y -= startY;
                                 }  
                                 if (x > 0 && isNewPage)
                                 {
                                     y += startY;
                                 }
                                 pictureRect.Offset(-x, -y);
                             }
                             
                             MemoryStream memoryStream = new MemoryStream();

                             System.Drawing.Image cropped = picture.Picture;
                             BitmapShapeImpl shapeImpl = picture as BitmapShapeImpl;
                             //As mentioned in XlsIO,  crop offset properties are divided by 1000.
                             double cropLeft = (double)shapeImpl.CropLeftOffset / 1000;
                             double cropTop = shapeImpl.CropTopOffset / 1000;
                             double cropRight = shapeImpl.CropRightOffset / 1000;
                             double cropBottom = shapeImpl.CropBottomOffset / 1000;
                             if ((pictureRect.Height < picture.Picture.Size.Height && pictureRect.Width < picture.Picture.Width) &&
                                 (shapeImpl.CropLeftOffset > 0 && shapeImpl.CropTopOffset > 0 && shapeImpl.CropRightOffset > 0 && shapeImpl.CropLeftOffset > 0))
                             {
                                 cropped = CropImage(picture.Picture, cropLeft, cropTop, cropRight, cropBottom,shapeImpl.HasTransparency);
                             }

                             if (shapeImpl.HasTransparency)
                             {
                                 cropped.Save(memoryStream, ImageFormat.Png);
                             }
                             else if (this.excelToPdfSettings.ExportQualityImage)
                             {
                                 cropped.Save(memoryStream, ImageFormat.Tiff);
                             }
                             else
                             {
                                 ImageFormat imageFormat = null;
                                 imageFormat = picture.Picture.RawFormat;
                                 if (imageCodec.ContainsValue(imageFormat.Guid))
                                 {
                                     memoryStream.Position = 0;
                                     cropped.Save(memoryStream, ImageFormat.Png);
                                 }
                             }
                             if (lastColumn < pictureShape.RightColumn)
                                 pictureRect.Width = pictureRect.Width + pdfSection.PageSettings.Margins.Left;
                             if (lastRow < pictureShape.BottomRow)
                                 pictureRect.Height = pictureRect.Height + pdfSection.PageSettings.Margins.Top;
                             if (memoryStream.Length!=0)
                             {
                                 if (shapeImpl.HasTransparency)
                                 {
                                     PdfBitmap bitMap = new PdfBitmap(memoryStream);
                                     bitMap.Matte = new int[] { 0, 0, 0 };
                                     graphics.DrawImage(bitMap, new RectangleF(pictureRect.X, pictureRect.Y, pictureRect.Width, pictureRect.Height));
                                 }
                                 else if (!(pageSetupOption.PrintTitleFirstRow <= firstRow) && !(pageSetupOption.PrintTitleFirstColumn <= firstColumn) || (firstColumn <= pictureShape.LeftColumn && lastColumn >= pictureShape.RightColumn))
                                 { 
                                     graphics.DrawImage(PdfImage.FromStream(memoryStream), new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
                                 }
                                 else if (((float)picture.Left < pdfPageTemplate.Width || IsPrintTitleColumnPage || IsPrintTitleRowPage && (float)picture.Left + pictureRect.Width <= pdfPageTemplate.Width) || excelToPdfSettings.LayoutOptions == LayoutOptions.NoScaling)
                                 {
                                     graphics.DrawImage(PdfImage.FromStream(memoryStream), new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
                                 }
                             }
                             else
                             {
                                 graphics.DrawImage(new PdfBitmap(cropped), new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
                             }
                             memoryStream.Close();
                         }
                    }
              }
            }
        }

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
        /// <summary>
        /// It's draw the worksheet chart's to the pdf
        /// </summary>
        /// <param name="sheet">It's represent the worksheet</param>
        /// <param name="graphics">Pdf Graphics</param>
        /// <param name="firstRow">First row</param>
        /// <param name="firstColumn">First Column</param>
        /// <param name="lastRow">Last row of chart shape</param>
        /// <param name="lastColumn">Last column of chart shape</param>
        /// <param name="startX">Starting point of chartshape-X Point</param>
        /// <param name="startY">starting point of chartshape - Y point</param>
        private void DrawCharts(WorksheetImpl sheet, PdfGraphics graphics, int firstRow, int firstColumn,
                                int lastRow, int lastColumn, float startX, float startY)
        {
            if (sheet.Charts.Count>0)
            {
                IPictures pictures = sheet.Pictures;
                float x = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(firstColumn - 1),
                                                                PdfGraphicsUnit.Point);
                float y = Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(firstRow - 1),
                                                                PdfGraphicsUnit.Point);
                float x2 = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(lastColumn),
                                                                 PdfGraphicsUnit.Point);
                float y2 = Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(lastRow),
                                                                 PdfGraphicsUnit.Point);
                x += startX;
                y += startY;
                float newx = x;
                float newy = y;
                for (int i = 0, len = sheet.Charts.Count; i < len; i++)
                {
                    
                    ShapeImpl pictureShape = sheet.Charts[i] as ShapeImpl;
                    bool isBetween;
                    bool toDraw = Drawpicture(firstRow, lastRow, firstColumn, lastColumn, pictureShape, out isBetween);
                    if (toDraw && ((float)pictureShape.Left < pdfPageTemplate.Width || (firstColumn <= pictureShape.LeftColumn || lastColumn >= pictureShape.RightColumn) || isBetween))
                    {
                        // Is image visible?
                        if ((Pdf_UnitConverter.ConvertFromPixels(pictureShape.Top, PdfGraphicsUnit.Point) <= y2 &&
                         Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Top + pictureShape.Height,
                                                               PdfGraphicsUnit.Point) >= y &&
                          Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Left, PdfGraphicsUnit.Point) <= x2 &&
                          Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Left + pictureShape.Width,
                                                                PdfGraphicsUnit.Point) >= x) ||
                         (Pdf_UnitConverter.ConvertFromPixels(pictureShape.Top, PdfGraphicsUnit.Point) <= this.currentPage.Size.Height &&
                           Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Left, PdfGraphicsUnit.Point) <= this.currentPage.Size.Width))
                        {

                            RectangleF pictureRect = new RectangleF(Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Left, PdfGraphicsUnit.Point),
                                                                    Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Top, PdfGraphicsUnit.Point),
                                                                    Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Width, PdfGraphicsUnit.Point),
                                                                    Pdf_UnitConverter.ConvertFromPixels((float)pictureShape.Height, PdfGraphicsUnit.Point));
                            if (this.workSheet.IsRightToLeft && (firstRow <= pictureShape.TopRow && lastRow >= pictureShape.BottomRow))
                            {
                                newx = pdfPageTemplate.Width - (pictureRect.Width + pictureRect.Left + startX);
                                if (pictureRect.Y < newx || pictureShape.Top > pictureShape.Left)
                                {
                                    newx -= pictureRect.Left;
                                    newy = startY;
                                }
                                if (pictureRect.Y > y && this.pdfDocument.Pages.Count > 1)
                                {
                                    pictureRect.Y = pictureRect.Y - (newx + newx / 2 - pictureRect.Height);
                                    newy = startY;
                                }
                                pictureRect.Offset(newx, newy);
                            }
                            else
                            {
                                if (y > 0)
                                {
                                    y -= startY;
                                }
                                if (x > 0 && isNewPage)
                                {
                                    y += startY;
                                }
                                pictureRect.Offset(-x, -y);
                            }

                            MemoryStream memoryStream = new MemoryStream();
                            sheet.Charts[i].SaveAsImage(memoryStream);
                            if (memoryStream.Length > 0)
                            {
                                System.Drawing.Image cropped = System.Drawing.Image.FromStream(memoryStream);

                                //As mentioned in XlsIO,  crop offset properties are divided by 1000.

                                {
                                    ImageFormat imageFormat = null;
                                    imageFormat = cropped.RawFormat;
                                    if (imageCodec.ContainsValue(imageFormat.Guid))
                                    {
                                        memoryStream.Position = 0;
                                        cropped.Save(memoryStream, ImageFormat.Png);
                                    }
                                }
                                if (lastColumn < pictureShape.RightColumn)
                                    pictureRect.Width = pictureRect.Width + pdfSection.PageSettings.Margins.Left;
                                if (lastRow < pictureShape.BottomRow)
                                    pictureRect.Height = pictureRect.Height + pdfSection.PageSettings.Margins.Top;
                                PdfBitmap bitMap = new PdfBitmap(memoryStream);
                                bitMap.Matte = new int[] { 0, 0, 0 };
                                if (memoryStream.Length != 0)
                                {
                                    if (!(pageSetupOption.PrintTitleFirstRow <= firstRow) && !(pageSetupOption.PrintTitleFirstColumn <= firstColumn) || (firstColumn <= pictureShape.LeftColumn && lastColumn >= pictureShape.RightColumn))
                                    {
                                        graphics.DrawImage(bitMap, new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
                                    }
                                    else if (((float)pictureShape.Left < pdfPageTemplate.Width || IsPrintTitleColumnPage || IsPrintTitleRowPage && (float)pictureShape.Left + pictureRect.Width <= pdfPageTemplate.Width) || excelToPdfSettings.LayoutOptions == LayoutOptions.NoScaling)
                                    {
                                        graphics.DrawImage(bitMap, new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
                                    }
                                }
                                else
                                {
                                    graphics.DrawImage(new PdfBitmap(cropped), new RectangleF((int)pictureRect.X, (int)pictureRect.Y, (int)pictureRect.Width, (int)pictureRect.Height));
                                }
                                memoryStream.Close();
                            }
                        }
                    }
                }
            }
        }

#endif
        /// <summary>
        /// Crops the image with the specified offset.
        /// </summary>
        /// <param name="cropableImage">Source Image to crop.</param>
        /// <param name="leftOffset">left offset to crop from.</param>
        /// <param name="topOffset">top offset to crop from.</param>
        /// <param name="rightOffset">right offset to crop.</param>
        /// <param name="bottomOffset">bottom offset to crop.</param>
        /// <param name="isTransparent">Indicates the destination image is transparent.</param>
        /// <returns>Returns the cropped image with the specified offsets.</returns>
        public static System.Drawing.Image CropImage(System.Drawing.Image cropableImage, double leftOffset,
           double topOffset, double rightOffset, double bottomOffset, bool isTransparent)
        {
            double actualImageWidth = cropableImage.Width;
            double actualImageHeight = cropableImage.Height;


            leftOffset = (actualImageWidth * (leftOffset / 100));
            topOffset = (actualImageHeight * (topOffset / 100));
            rightOffset = (actualImageWidth * (rightOffset / 100));
            bottomOffset = (actualImageHeight * (bottomOffset / 100));

            int width = (int)(actualImageWidth - (leftOffset + rightOffset));
            int height = (int)(actualImageHeight - (topOffset + bottomOffset));
            Bitmap bitmapDestination = new Bitmap(width, height, cropableImage.PixelFormat);
            bitmapDestination.SetResolution(cropableImage.VerticalResolution, cropableImage.HorizontalResolution);

            Graphics bitmapDestinationGraphics = Graphics.FromImage(bitmapDestination);

            if (isTransparent)
            {
                bitmapDestinationGraphics.CompositingMode = CompositingMode.SourceCopy;
                bitmapDestinationGraphics.CompositingQuality = CompositingQuality.HighQuality;
                bitmapDestinationGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                bitmapDestinationGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                bitmapDestinationGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                bitmapDestinationGraphics.Clear(Color.Transparent);
            }

            Rectangle rect = new Rectangle((int)leftOffset, (int)topOffset, width, height);

            bitmapDestinationGraphics.DrawImage(cropableImage, 0, 0, rect, GraphicsUnit.Pixel);

            if (isTransparent)
            {
                bitmapDestination.MakeTransparent();
            }
            return bitmapDestination;
        }

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
        /// <summary>
        /// Draws the header and footer images.
        /// </summary>
        /// <param name="pageSetupBase">The worksheet object.</param>
        /// <param name="sheetWidth">Width of the sheet.</param>
        /// <param name="pageTemplate">The pdf page template.</param>
        /// <param name="align">The alignment of the header and footer text.</param>
        /// <param name="name">The name represents the header part or footer part.</param>
        /// <param name="pdfSection">The PDF section.</param>
        private void DrawHeaderFooterImages(IPageSetupBase pageSetupBase, float sheetWidth, PdfTemplate pageTemplate, string align,
                                   string name, PdfSection pdfSection)
        {
            float width = sheetWidth;
            float height = pageTemplate.Height;
            if (align == "Left" && pageSetupBase.LeftHeaderImage != null && name == "Header")
            {
                Image image = this.GetScaledPicture(pageSetupBase.LeftHeaderImage, 0, (int)height);
                PdfImage leftImage = new PdfBitmap(image);
                pageTemplate.Graphics.DrawImage(leftImage, new PointF(0, 0));
            }

            if (align == "Center" && pageSetupBase.CenterHeaderImage != null && name == "Header")
            {
                Image image = this.GetScaledPicture(pageSetupBase.CenterHeaderImage, 0, (int)height);
                PdfImage centerImage = new PdfBitmap(image);
                pageTemplate.Graphics.DrawImage(centerImage, new PointF(width, 0));
            }

            if (align == "Right" && pageSetupBase.RightHeaderImage != null && name == "Header")
            {
                Image image = this.GetScaledPicture(pageSetupBase.RightHeaderImage, 0, (int)height);
                PdfImage rightImage = new PdfBitmap(image);
                width = sheetWidth - rightImage.Width;
                pageTemplate.Graphics.DrawImage(rightImage, new PointF(width, 0));
            }

            if (align == "Left" && pageSetupBase.LeftFooterImage != null && name == "Footer")
            {
                Image image = this.GetScaledPicture(pageSetupBase.LeftFooterImage, 0, (int)height);
                pageTemplate.Graphics.DrawImage(new PdfBitmap(image), new PointF(0, 0));
            }

            if (align == "Center" && pageSetupBase.CenterFooterImage != null && name == "Footer")
            {
                Image image = this.GetScaledPicture(pageSetupBase.CenterFooterImage, 0, (int)height);
                PdfImage centerImage = new PdfBitmap(image);
                pageTemplate.Graphics.DrawImage(centerImage, new PointF(width, 0));
            }

            if (align == "Right" && pageSetupBase.RightFooterImage != null && name == "Footer")
            {
                Image image = this.GetScaledPicture(pageSetupBase.RightFooterImage, 0, (int)height);
                PdfImage rightImage = new PdfBitmap(image);
                width = sheetWidth - rightImage.Width;
                pageTemplate.Graphics.DrawImage(rightImage, new PointF(width, 0));
            }
        }
#else

        /// <summary>
        /// Draws the header and footer images.
        /// </summary>
        /// <param name="sheet">The worksheet object.</param>
        /// <param name="sheetWidth">Width of the sheet.</param>
        /// <param name="pageTemplate">The pdf page template.</param>
        /// <param name="align">The alignment of the header and footer text.</param>
        /// <param name="name">The name represents the header part or footer part.</param>
        /// <param name="pdfSection">The PDF section.</param>
        private void DrawHeaderFooterImages(IWorksheet sheet, float sheetWidth, PdfTemplate pageTemplate, string align,
                                   string name, PdfSection pdfSection,RichTextString rxtString,out int imageHeight,int maxWidth,out int imagewidth)
        {           
            float height = pageTemplate.Height;
            Image image=null;
            imageHeight = 0;
            if (name == "Header")
            {
                if (align == "Left" && sheet.PageSetup.LeftHeaderImage != null)
                {
                    //if ((sheet.PageSetup.RightHeaderImage != null && sheet.PageSetup.CenterHeaderImage!=null ) || (sheet.PageSetup.CenterHeader.Length !=0 || sheet.PageSetup.RightHeader.Length !=0))
                    //{
                    //    image = this.GetScaledPicture(sheet.PageSetup.LeftHeaderImage, maxWidth, (int)height);
                    //}
                    //else
                    {
                        image = sheet.PageSetup.LeftHeaderImage;                                     
                    }
                }
               
                if (align == "Center" && sheet.PageSetup.CenterHeaderImage != null)
                    image = sheet.PageSetup.CenterHeaderImage; 

                if (align == "Right" && sheet.PageSetup.RightHeaderImage != null)
                    image = sheet.PageSetup.RightHeaderImage; 
            }
            else if (name == "Footer")
            {
                if (align == "Left" && sheet.PageSetup.LeftFooterImage != null )
                    image = sheet.PageSetup.LeftFooterImage; 

                if (align == "Center" && sheet.PageSetup.CenterFooterImage != null )
                    image = sheet.PageSetup.CenterFooterImage; 
                
                if (align == "Right" && sheet.PageSetup.RightFooterImage != null )
                    image = sheet.PageSetup.RightFooterImage; 
            }
            imageHeight = image.Height;
            imagewidth = image.Width;

            RichTextBoxExt ext = new RichTextBoxExt();
            Clipboard.SetDataObject(image);
            DataFormats.Format df = DataFormats.GetFormat(DataFormats.Bitmap);
            if (ext.CanPaste(df))
                ext.Paste(df);
            rxtString.ImageRTF = ext.Rtf;
        }

#endif
        #endregion
        

        #region Helper Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfSection"></param>
        /// <param name="zoomValue"></param>
        /// <returns></returns>
       internal SizeF CalculateZoomValue(PdfSection pdfSection,int zoomValue)
        {
            if (zoomValue <= 100)
            {
                sheetHeight = ((pdfSection.PageSettings.Height * (100 - zoomValue)) / 100) + (pdfSection.PageSettings.Height + headerMargin + footerMargin);
                sheetWidth = ((pdfSection.PageSettings.Width * (100 - zoomValue)) / 100) + (pdfSection.PageSettings.Width);
                if (zoomValue > 50)
                {
                    sheetHeight = sheetHeight - ((sheetHeight * (100 - zoomValue)) / 100);
                    sheetWidth = sheetWidth - ((sheetWidth * (100 - zoomValue)) / 100);
                    if (zoomValue == 100)
                    {
                        sheetHeight = sheetHeight +  (footerMargin + headerMargin + 2*topMargin +2* bottomMargin);
                        sheetWidth = sheetWidth + (sheetWidth ) + (2* rightMargin +  2*leftMargin);
                    }
                    else if (zoomValue >= 90)
                    {
                        sheetHeight = sheetHeight + (pdfSection.PageSettings.Margins.Bottom + pdfSection.PageSettings.Margins.Top);
                        sheetWidth = sheetWidth - (rightMargin + leftMargin);
                    }
                    else if (zoomValue >= 80)
                    {
                        sheetHeight = sheetHeight + (footerMargin + headerMargin);
                        sheetWidth = sheetWidth + (2*rightMargin + 2*leftMargin);
                    }
                    else if (zoomValue >= 70)
                    {
                        sheetHeight = sheetHeight + (pdfSection.PageSettings.Margins.Bottom + pdfSection.PageSettings.Margins.Top);
                        sheetWidth = sheetWidth + (pdfSection.PageSettings.Margins.Right + pdfSection.PageSettings.Margins.Left);
                    }
                    else if (zoomValue >= 60)
                    {
                        sheetHeight = sheetHeight + (sheetHeight/4)+(footerMargin + headerMargin + topMargin + 2*bottomMargin);
                        sheetWidth = sheetWidth + (sheetWidth /2) + (3 * rightMargin + 3 * leftMargin);
                    }
                    else
                    {
                        sheetHeight = sheetHeight + (sheetHeight / 2) + (footerMargin + headerMargin + topMargin + bottomMargin);
                        sheetWidth = sheetWidth + (sheetWidth / 2)+ (3 * rightMargin + 3 * leftMargin);
                    }

                }
                else
                {
                    if (zoomValue >= 40)
                    {
                        sheetHeight = sheetHeight + (footerMargin + headerMargin);
                    }
                    else if (zoomValue >= 30)
                    {
                        sheetHeight = sheetHeight + ((sheetHeight * (100 - zoomValue)) / 100);
                        sheetHeight = sheetHeight - (footerMargin + headerMargin + topMargin + bottomMargin);
                    }
                    else if (zoomValue >= 20)
                    {
                        sheetHeight = sheetHeight + ((sheetHeight * (100 - zoomValue)) / 100);
                        sheetHeight = sheetHeight + (footerMargin + headerMargin);
                    }
                    else if (zoomValue >= 10)
                    {
                        sheetHeight = sheetHeight + ((sheetHeight * (100 - zoomValue)) / 100);
                        sheetHeight = sheetHeight + (footerMargin + headerMargin + topMargin + bottomMargin);
                    }
                    sheetWidth = sheetWidth + (rightMargin + leftMargin);
                }
            }
            return new SizeF(sheetWidth, sheetHeight);
        }
        /// <summary>
        /// Initializes the PDF page.
        /// </summary>
        private void InitializePdfPage()
        {
            this.currentPage = pdfSection.Pages.Add();
            if (pdfSection.Pages.Count > 1)
                this.isNewPage = true;
            if (pdfPageTemplate != null)
            {
                this.pdfGraphics = pdfPageTemplate.Graphics;
            }
            else
            {
                this.pdfGraphics = this.currentPage.Graphics;
            }
        }

        /// <summary>
        /// Intializes the header and footer.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        private void IntializeHeaderFooter(IWorksheet sheet)
        {
            this.predefinedHeaderFooter.Add("&A", sheet.Name);
            this.predefinedHeaderFooter.Add("&D", DateTime.Now.ToShortDateString());
            this.predefinedHeaderFooter.Add("&T", DateTime.Now.ToShortTimeString());
            if (this.workBookImpl != null)
            {
                this.predefinedHeaderFooter.Add("&Z", Path.GetDirectoryName(this.workBookImpl.FullFileName));
                this.predefinedHeaderFooter.Add("&F", Path.GetFileNameWithoutExtension(this.workBookImpl.FullFileName));
            }
        }
        /// <summary>
        /// Initialize the default header and footer for chartsheet
        /// </summary>
        /// <param name="chart"></param>
        private void IntializeHeaderFooter(IChart chart)
        {
            this.predefinedHeaderFooter.Add("&A", chart.Name);
            this.predefinedHeaderFooter.Add("&D", DateTime.Now.ToShortDateString());
            this.predefinedHeaderFooter.Add("&T", DateTime.Now.ToShortTimeString());
            if (this.workBookImpl != null)
            {
                this.predefinedHeaderFooter.Add("&Z", Path.GetDirectoryName(this.workBookImpl.FullFileName));
                this.predefinedHeaderFooter.Add("&F", Path.GetFileNameWithoutExtension(this.workBookImpl.FullFileName));
            }
        }

        /// <summary>
        /// Intializes the fonts.
        /// </summary>
        private void IntializeFonts()
        {
            this.fontList.Add("Verdana");
            this.fontList.Add("Times New Roman");
            this.fontList.Add("Microsoft Sans Serif");
            this.fontList.Add("Tahoma");
            this.fontList.Add("Arial");
            this.fontList.Add("SimSun");
            this.fontList.Add("MingLiU");
            this.fontList.Add("Calibri");
        }
        /// <summary>
        /// Intializes the Remoable Characters
        /// </summary>
        private void IntializeRemovableCharacters()
        {
            this.removableCharaters.Add('\n');
            this.removableCharaters.Add('\r');            
        }

        /// <summary>
        /// Intializes the Image Encoders
        /// </summary>
        private void InitializeImageEncoder()
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                imageCodec.Add(codec, codec.FormatID);
            }
        }
        /// <summary>
        /// Gets the adjacent cell size.
        /// </summary>
        /// <param name="cell">The cell rnage.</param>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <param name="firstColumn">The first column of the range.</param>
        /// <param name="cell2">The adjacent cell.</param>
        /// <param name="originalWidth">Width of the sheet.</param>
        /// <returns>The Adjacent cell RectangleF</returns>
        private RectangleF GetAdjacentCells(IRange cell, RectangleF rect,int firstColumn, MigrantRangeImpl cell2, float originalWidth)
        {
            RectangleF rectangle = rect;
            RangeImpl rangeImpl = cell as RangeImpl;
            rangeImpl.updateCellValue = false;
            if (!cell.IsBlank && !cell.WrapText && (cell.HasString || cell.DisplayText!=string.Empty || cell.FormulaStringValue != null))
            {
                float newX;
                float newWidth;
                float newHeight = rect.Height;
                int rowIndex = cell.Row;
                int columnIndex = cell.Column;
                if (columnIndex == 0)
                {
                    columnIndex = 1;
                }

                int lastColumnIndex = columnIndex;
                WorksheetImpl sheetImpl = cell.Worksheet as WorksheetImpl;
                int lastPossibleColumn = sheetImpl.ParentWorkbook.MaxColumnCount;
                SizeF cellSize = sheetImpl.MeasureCell(cell, false, false);
                float currentWidth = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetSize(columnIndex),
                                                                           PdfGraphicsUnit.Point);
                float requiredWidth = cellSize.Width;

                if (requiredWidth > currentWidth)
                {
                    IStyle style = cell.CellStyle;
                    ExcelHAlign horizontalAlign = style.HorizontalAlignment;
                    int deltaIndex = 1;

                    if (horizontalAlign == ExcelHAlign.HAlignRight)
                    {
                        deltaIndex = -1;
                    }

                    //Cell Left and Right align calculation
                    if (horizontalAlign != ExcelHAlign.HAlignCenter)
                    {
                        cell2.ResetRowColumn(rowIndex, lastColumnIndex + deltaIndex);

                        while (cell2.IsBlank &&
                         lastColumnIndex < lastPossibleColumn &&
                         lastColumnIndex > 0 && requiredWidth > currentWidth)
                        {
                            int cell2Column = cell2.Column;
                            if (cell2Column == 0)
                            {
                                cell2Column = 1;
                            }

                            currentWidth += Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetSize(cell2Column),
                                                                                  PdfGraphicsUnit.Point);
                            lastColumnIndex += deltaIndex;
                            cell2.ResetRowColumn(rowIndex, lastColumnIndex + deltaIndex);
                        }

                        int firstColumnIndex = Math.Min(lastColumnIndex, cell.Column);
                        if (firstColumnIndex == 0)
                        {
                            firstColumnIndex = 1;
                        }

                        int newLastColumnIndex = Math.Max(lastColumnIndex, cell.Column);
                        newX = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(firstColumn, firstColumnIndex - 1),
                                                                     PdfGraphicsUnit.Point);
                        newWidth = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(firstColumnIndex, newLastColumnIndex),
                                                                         PdfGraphicsUnit.Point);
                    }
                    //Cell text center align calculation
                    else
                    {
                       float frontCellWidth = (requiredWidth / 2) - (currentWidth / 2);
                       float backCellWidth = (requiredWidth/ 2) + (currentWidth / 2);
                        newX = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(firstColumn, columnIndex-1), PdfGraphicsUnit.Point) -
                                                                     Pdf_UnitConverter.ConvertFromPixels(frontCellWidth, PdfGraphicsUnit.Point);
                        newWidth = requiredWidth;
                    }

                    if (this.excelToPdfSettings.EnableRTL)
                    {
                        newX = originalWidth - newX;
                        newX = newX - newWidth;
                    }

                    rectangle = new RectangleF(newX, rect.Y, newWidth, newHeight);
                }
            }

            return rectangle;
        }

        /// <summary>
        /// Gets the background height coordinates.
        /// </summary>
        /// <param name="startX">The start X.</param>
        /// <param name="startY">The start Y.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="imageCoordinates">The image coordinates.</param>
        /// <param name="pdfPage">The PDF page.</param>
        /// <returns>
        /// The collection coordinates and the sizes of Background height.
        /// </returns>
        protected Dictionary<PointF, SizeF> GetBackgroundHeightCoordinates(float startX, float startY,
                                                                           float imageWidth, float imageHeight,
                                                                           Dictionary<PointF, SizeF> imageCoordinates,
                                                                           PdfPage pdfPage)
        {
            if (pdfPage.Size.Height >= startY)
            {
                if (!imageCoordinates.ContainsKey(new PointF(startX, startY)))
                {
                    imageCoordinates.Add(new PointF(startX, startY), new SizeF(imageWidth, imageHeight));
                }

                this.GetBackgroundHeightCoordinates(startX, imageHeight + startY, imageWidth, imageHeight,
                                                    imageCoordinates, pdfPage);
            }

            return imageCoordinates;
        }

        /// <summary>
        /// Gets the background width coordinates.
        /// </summary>
        /// <param name="startX">The start X.</param>
        /// <param name="startY">The start Y.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="imageCoordinates">The image coordinates.</param>
        /// <param name="pdfPage">The PDF page.</param>
        /// <returns>
        /// The collection coordinates and the sizes of Background image width .
        /// </returns>
        protected Dictionary<PointF, SizeF> GetBackgroundWidthCoordinates(float startX, float startY,
                                                                          float imageWidth, float imageHeight,
                                                                          Dictionary<PointF, SizeF> imageCoordinates,
                                                                          PdfPage pdfPage)
        {
            if (pdfPage.Size.Width >= startX)
            {
                if (!imageCoordinates.ContainsKey(new PointF(startX, startY)))
                {
                    imageCoordinates.Add(new PointF(startX, startY), new SizeF(imageWidth, imageHeight));
                }

                imageCoordinates = this.GetBackgroundHeightCoordinates(startX, startY, imageWidth, imageHeight,
                                                                       imageCoordinates, pdfPage);
                this.GetBackgroundWidthCoordinates(startX + imageWidth, startY, imageWidth, imageHeight,
                                                   imageCoordinates, pdfPage);
            }

            return imageCoordinates;
        }

        /// <summary>
        /// Gets the width of the border.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <returns>The borderWidth of the Excel border Index.</returns>
        private float GetBorderWidth(IBorder border)
        {
            switch (border.LineStyle)
            {
                case ExcelLineStyle.Double:
                case ExcelLineStyle.Thin:
                case ExcelLineStyle.Dashed:
                case ExcelLineStyle.Dotted:
                case ExcelLineStyle.Dash_dot:
                case ExcelLineStyle.Slanted_dash_dot:
                case ExcelLineStyle.Dash_dot_dot:
                    this.borderWidth = 1F;
                    break;
                case ExcelLineStyle.Medium:
                case ExcelLineStyle.Medium_dashed:
                case ExcelLineStyle.Medium_dash_dot:
                case ExcelLineStyle.Medium_dash_dot_dot:
                    this.borderWidth = 2F;
                    break;
                case ExcelLineStyle.Thick:
                    this.borderWidth = 3F;
                    break;
                case ExcelLineStyle.Hair:
                case ExcelLineStyle.None:
                    this.borderWidth = 0.5F;
                    break;
            }

            return this.borderWidth;
        }

        /// <summary>
        /// Gets the bottom text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The text to be displayed in the rotated Text.</returns>
        private string GetBottomText(string value, int rotationAngle)
        {
            if (rotationAngle == ExtendedFormatImpl.TopToBottomRotation)
            {
                StringBuilder builder = new StringBuilder(value);

                for (int i = 0, j = 1, len = value.Length; i < len; i++, j += 2)
                {
                    builder.Insert(j, '\n');
                }

                value = builder.ToString();
            }

            return value;
        }

        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <param name="internalExtendedFormat">The internal extended format of the cell.</param>
        /// <returns>The PdfBrush object.</returns>
        private PdfBrush GetBrush(IInternalExtendedFormat internalExtendedFormat)
        {
            PdfBrush pdfBrush = null;
            if (internalExtendedFormat.FillPattern == ExcelPattern.Solid)
            {
                PdfColor pdfColor = new PdfColor(NormalizeColor(internalExtendedFormat.Color));
                pdfBrush = new PdfSolidBrush(pdfColor);
            }
            else
            {
                pdfBrush = new PdfSolidBrush(new PdfColor(NormalizeColor(internalExtendedFormat.Color)));
            }

            return pdfBrush;
        }

        /// <summary>
        /// Gets the counter clockwise rotation.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The rotation angle as an MS Excel representation.</returns>
        private int GetCounterClockwiseRotation(int rotationAngle)
        {
            if (rotationAngle > 90)
            {
                rotationAngle -= 90;
            }
            else
            {
                rotationAngle = -rotationAngle;
            }

            return rotationAngle;
        }

        /// <summary>
        /// Gets the dash style.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <returns>The PdfDashStyle value.</returns>
        private PdfDashStyle GetDashStyle(IBorder border)
        {
            PdfDashStyle dashStyle = PdfDashStyle.Solid;

            switch (border.LineStyle)
            {
                case ExcelLineStyle.Thin:
                case ExcelLineStyle.Medium:
                case ExcelLineStyle.Thick:
                case ExcelLineStyle.Double:
                case ExcelLineStyle.Hair:
                    dashStyle = PdfDashStyle.Solid;
                    break;
                case ExcelLineStyle.Dashed:
                case ExcelLineStyle.Medium_dashed:
                    dashStyle = PdfDashStyle.Dash;
                    break;
                case ExcelLineStyle.Dotted:
                    dashStyle = PdfDashStyle.Dot;
                    break;
                case ExcelLineStyle.Dash_dot:
                case ExcelLineStyle.Medium_dash_dot:
                case ExcelLineStyle.Slanted_dash_dot:
                    dashStyle = PdfDashStyle.DashDot;
                    break;
                case ExcelLineStyle.Dash_dot_dot:
                case ExcelLineStyle.Medium_dash_dot_dot:
                    dashStyle = PdfDashStyle.DashDotDot;
                    break;
            }

            return dashStyle;
        }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <param name="font">The pdf font object.</param>
        /// <param name="format">The pdf string format for the Excel cell display text.</param>
        /// <param name="value">The cell display text.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <returns>
        /// The actual text that need to be displayed in the Pdf page.
        /// </returns>
        private string GetDisplayText(PdfFont font, PdfStringFormat format, string value, RectangleF cellRect, IRange cell, PdfBrush brush, bool isRTL)
        {
            if (Math.Round(font.MeasureString(value, format).Width) == cellRect.Width)
            {
                m_fitText = true;
                return value;
            }
            else if (font.MeasureString(value, format).Width < cellRect.Width)
            {
                return value;
            }

            string splitValue = "#";
            Dictionary<int, string> keywords=null;            
            ArrayList arrayValues = new ArrayList();
            //Add the char array to the array list.
            arrayValues.AddRange(value.ToCharArray());
            if (isRTL)
            {
             keywords  = GetKeyowrds(value, arrayValues, isRTL);
            }
            for (int i = arrayValues.Count - 1; i > 0; i--)
            {
                //remove from the reverse 
                arrayValues.RemoveAt(i);
                //Convert back to char array
                char[] charArray = (char[])arrayValues.ToArray(typeof(char));
                splitValue = new string(charArray);
                //check the condition.
                if (font.MeasureString(splitValue, format).Width <= cellRect.Width)
                {
                    break;
                }
            }
            if (value.Length != splitValue.Length && value.Length>0)
            {
                SplitText split = new SplitText();
                string text = value.Substring(splitValue.Length);
                split.OriginRect = cellRect;
                split.Sheet = workSheet;
                split.Row = cell.Row;
                split.AdjacentColumn = cell.Column + 1;
                split.TextFont = font;
                split.Format = format;
                split.Brush = brush;
                if(isRTL)
                UpdateSplitText(keywords, ref splitValue, ref text, value);
                split.Text = text;
                splitTextCollection.Add(split);
            }

            return splitValue;
        }

        /// <summary>
        /// Gets the keyowrds.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="arrayValues">The array values.</param>
        /// <param name="isRTL">if set to <c>true</c> [is RTL].</param>
        /// <returns></returns>
        private Dictionary<int, string> GetKeyowrds(string value, ArrayList arrayValues, bool isRTL)
        {
            if (!isRTL)
                return null;

            Dictionary<int, string> keywords = new Dictionary<int, string>();   
            for(int i=0;i<arrayValues.Count-1;i++)
            {
                if (Array.IndexOf(unicodeChar,(int)((char) arrayValues[i])) == 1 || Char.IsPunctuation((char)arrayValues[i]))
                {
                    keywords.Add(i, arrayValues[i].ToString());                    
                }                
                else if (Char.IsLetterOrDigit((char)arrayValues[i]))
                {
                    break;
                }                
                
            }
            for (int i = arrayValues.Count - 1; i >= 0; i--)
            {
                if (Array.IndexOf(unicodeChar, (int)((char)arrayValues[i])) == 0 || Char.IsPunctuation((char)arrayValues[i]))
                {
                    keywords.Add(i, arrayValues[i].ToString());
                    
                }
                else if (Char.IsLetterOrDigit((char)arrayValues[i]))
                {
                    break;
                }
               
            }       
                
            
            return keywords;
        }

        /// <summary>
        /// Updates the split text.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="splitValue">The split value.</param>
        /// <param name="text">The text.</param>
        /// <param name="sourceString">The source string.</param>
        private void UpdateSplitText(Dictionary<int, string> keywords, ref  string splitValue, ref string text, string sourceString)
        {
            if (keywords.Count == 0)
                return;

            if (sourceString.Length != (splitValue.Length + text.Length))
                throw new ArgumentException("Length of the strings does not match");

            //ClearKeywords(ref splitValue, ref text, keywords, sourceString);
            
            foreach (KeyValuePair<int, string> split in keywords)
            {
                if (split.Key >= text.Length )
                {
                    if (!CheckIndex(split.Key, text, splitValue, split.Value))
                    {
                        int index = split.Key - text.Length;
                        splitValue = splitValue.Insert(index+1, split.Value);
                    }
                }
                else if (split.Key <= splitValue.Length)
                {
                    if (splitValue.Contains(split.Value))
                        splitValue = splitValue.Remove(splitValue.IndexOf(split.Value), split.Value.Length);

                    text = text.Insert(split.Key, split.Value);
                }
            }
        }

        /// <summary>
        /// Checks the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="text">The text.</param>
        /// <param name="splitValue">The split value.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool CheckIndex(int index, string text, string splitValue,string value)
        {
            bool check = false;

            if (index >= text.Length)
            {
                if (splitValue.IndexOf(value) + text.Length == index)
                {
                    check = true;                    
                }
            }
            else if (index <= splitValue.Length)
            {
                if (text.IndexOf(value) == index)
                {
                    check = true;
                }
            }

            return check;
        }

        private void ClearKeywords(ref string splitValue, ref string text, Dictionary<int, string> keywords, string sourceString)
        {
            foreach (KeyValuePair<int, string> keyword in keywords)
            {
                if(splitValue.Contains(keyword.Value) && keyword.Key >= text.Length )
                {
                   splitValue= splitValue.Remove(splitValue.IndexOf(keyword.Value));
                }
                else if(text.Contains(keyword.Value)&& keyword.Key<= splitValue.Length)
                {
                  text=  text.Remove(text.IndexOf(keyword.Value));
                }
            }
        }
        private string GetDisplayText(PdfFont font, PdfStringFormat format, string value, RectangleF cellRect, bool isRTL)
        {
            if (font.MeasureString(value, format).Width <= cellRect.Width)
            {
                return value;
            }

            string splitValue = "#";
            float actualWidth = cellRect.Width;
            ArrayList arrayValues = new ArrayList();
            //Add the char array to the array list.
            arrayValues.AddRange(value.ToCharArray());
            if (!isRTL)
            {
                for (int i = arrayValues.Count - 1; i > 0; i--)
                {
                    //remove from the reverse 
                    arrayValues.RemoveAt(i);
                    //Convert back to char array
                    char[] charArray = (char[])arrayValues.ToArray(typeof(char));
                    splitValue = new string(charArray);
                    //check the condition.
                    if (font.MeasureString(splitValue, format).Width <= actualWidth)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i =0 ; i < arrayValues.Count - 1; i++)
                {
                    //remove from the reverse 
                    arrayValues.RemoveAt(i);
                    //Convert back to char array
                    char[] charArray = (char[])arrayValues.ToArray(typeof(char));
                    splitValue = new string(charArray);
                    //check the condition.
                    if (font.MeasureString(splitValue, format).Width <= actualWidth)
                    {
                        break;
                    }
                }
            }

            return splitValue;
        }

        private string GetDisplayText(PdfFont font, PdfStringFormat format, string value, RectangleF cellRect)
        {
            if (font.MeasureString(value, format).Height <= cellRect.Height)
            {
                return value;
            }            
            string splitValue = string.Empty;
            string str=value;
            for (int i = 0, len= str.Length; i < len; i++)
            {
                if (str[i].Equals('\n'))
                {
                    string subStr = str.Substring(0, i);
                    if (font.MeasureString(subStr, format).Height <= cellRect.Height)
                    {
                        splitValue += subStr;
                        str = str.Substring(i);
                        len = str.Length;
                        i = 0;
                    }
                    else
                    {
                        splitValue += subStr;
                        break;
                    }
                }
                else if(i==len-1)
                {
                    if (font.MeasureString(str, format).Height <= cellRect.Height)
                    {
                        splitValue += str;
                    }
                    else
                    {
                        splitValue = str;
                    }
                }

            }          

            return splitValue;
        }

        /// <summary>
        /// Gets the display wrap text list.
        /// </summary>
        /// <param name="pdfFont">The PDF font.</param>
        /// <param name="pdfFormat">The PDF string format.</param>
        /// <param name="cellTextValue">The cell text value.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <returns>The list of wrapped text.</returns>
        private List<string> GetDisplayWrapTextList(PdfFont pdfFont, PdfStringFormat pdfFormat,
                                                    string cellTextValue, RectangleF cellRect)
        {
            List<string> splitTextList = new List<string>();
            if (pdfFont.MeasureString(cellTextValue, pdfFormat).Width <= cellRect.Width)
            {
                splitTextList.Add(cellTextValue);
                return splitTextList;
            }

            if (pdfFormat.WordWrap != PdfWordWrapType.None)
            {
                pdfFormat.WordWrap = PdfWordWrapType.Character;
            }

            splitTextList = this.GetDisplayWrapTextList(pdfFont, pdfFormat, cellTextValue, cellRect,
                                                        splitTextList);
            return splitTextList;
        }

        /// <summary>
        /// Gets the display wrap text list.
        /// </summary>
        /// <param name="pdfFont">The PDF font.</param>
        /// <param name="pdfFormat">The PDF string format.</param>
        /// <param name="cellTextValue">The cell text value.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="splitTextList">The split text list.</param>
        /// <returns>The List of wrapped text.</returns>
        private List<string> GetDisplayWrapTextList(PdfFont pdfFont, PdfStringFormat pdfFormat,
                                                    string cellTextValue, RectangleF cellRect,
                                                    List<string> splitTextList)
        {
            string splitValue = "#";
            List<string> splitValues = new List<string>();
            ArrayList arrayValues = new ArrayList();
            //Add the char array to the array list.
            arrayValues.AddRange(cellTextValue.ToCharArray());
            if (pdfFont.MeasureString(cellTextValue, pdfFormat).Width <= cellRect.Width)
            {
                if (splitTextList.Count != 0)
                {
                    splitTextList.Add("\r\n");
                }

                splitTextList.Add(cellTextValue);
            }
            else
            {
                for (int i = arrayValues.Count - 1; i >= 0; i--)
                {
                    if (arrayValues.Count > 1)
                    {
                        splitValues.Add(arrayValues[i].ToString());
                        //remove from the reverse 
                        arrayValues.RemoveAt(i);
                    }

                    //Convert back to char array
                    char[] charArray = (char[])arrayValues.ToArray(typeof(char));
                    splitValue = new string(charArray);
                    //check the condition.
                    if (pdfFont.MeasureString(splitValue, pdfFormat).Width <= cellRect.Height)
                    {
                        if (splitTextList.Count != 0)
                        {
                            splitTextList.Add("\r\n");
                        }

                        splitTextList.Add(splitValue);
                        break;
                    }
                }
            }

            if (splitValues.Count != 0)
            {
                splitValues.Reverse();
                StringBuilder builder = new StringBuilder();
                foreach (string strValue in splitValues)
                {
                    builder.Append(strValue);
                }

                //this.GetDisplayWrapTextList(pdfFont, pdfFormat, builder.ToString(), cellRect, splitTextList);
            }

            return splitTextList;
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="font">The font object of the cell.</param>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="size">The size of the font.</param>
        /// <returns>Font object</returns>
        private Font GetFont(IFont font, string fontName, int size)
        {
            Font m_font = new Font(fontName, size);
            FontStyle bold = new FontStyle();
            if (font.Bold == true)
            {
                bold = FontStyle.Bold;
            }
            else
            {
                bold = FontStyle.Regular;
            }

            FontStyle italic = new FontStyle();
            if (font.Italic == true)
            {
                italic = FontStyle.Italic;
            }

            FontStyle underline = new FontStyle();
            ExcelUnderline line = font.Underline;
            if (line.ToString() == "Single")
            {
                underline = FontStyle.Underline;
            }

            m_font = new Font(fontName, size, bold | italic | underline);
            return m_font;
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <param name="size">The size of the font.</param>
        /// <returns>Font object</returns>
        private Font GetFont(string name, int size,bool hasUnderline,Font fontSettings)
        {
            FontStyle fontBold = (fontSettings.Style & FontStyle.Bold) != 0 ? FontStyle.Bold : FontStyle.Regular;
            FontStyle fontItalic = (fontSettings.Style & FontStyle.Italic) != 0 ? FontStyle.Italic : FontStyle.Regular; ;
            FontStyle fontUnderline = hasUnderline ? FontStyle.Underline : FontStyle.Regular;
            Font font = new Font("Calibri", 12);
            string fontName = "Calibri";
            if (name[0] == '\"')
            {
                string[] split = name.Split('\"');
                split = split[1].Split(',');
                if (split.Length > 1)
                {
                    if (split[0] != "-")
                    {
                        fontName = split[0];
                    }
                    else if (split[0] == "-")
                    {
                        fontBold = FontStyle.Regular;
                        fontItalic = FontStyle.Regular;
                        fontUnderline = FontStyle.Regular;
                    }

                    string[] stringsplit = split[1].Split(' ');
                    for (int i = 0; i < stringsplit.Length; i++)
                    {
                        if (stringsplit[i] == "Bold")
                        {
                            fontBold = FontStyle.Bold;
                        }

                        if (stringsplit[i] == "Italic")
                        {
                            fontItalic = FontStyle.Italic;
                        }
                        if (stringsplit[i] == "Regular")
                        {
                            fontBold = FontStyle.Regular;                            
                        }
                    }
                }
            }
            else
            {
                fontName = name;
            }

            font = new Font(fontName, size, fontBold | fontItalic| fontUnderline);
            return font;
        }

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <param name="size">The size of the font.</param>
        /// <returns>Font object</returns>
        private Font GetFont(string name, int size)
        {
            FontStyle fontBold = FontStyle.Regular;
            FontStyle fontItalic = FontStyle.Regular;
            Font font = new Font("Calibri", 12);
            string fontName = "Calibri";
            if (name[0] == '\"')
            {
                string[] split = name.Split('\"');
                split = split[1].Split(',');
                if (split.Length > 1)
                {
                    if (split[0] != "-")
                    {
                        fontName = split[0];
                    }

                    string[] stringsplit = split[1].Split(' ');
                    for (int i = 0; i < stringsplit.Length; i++)
                    {
                        if (stringsplit[i] == "Bold")
                        {
                            fontBold = FontStyle.Bold;
                        }

                        if (stringsplit[i] == "Italic")
                        {
                            fontItalic = FontStyle.Italic;
                        }
                    }
                }
            }

            font = new Font(fontName, size, fontBold | fontItalic);
            return font;
        }

        /// <summary>
        /// Gets the header information.
        /// </summary>
        /// <param name="sheet">The IWorksheet object.</param>
        /// <param name="fontColorSettings">The font color settings.</param>
        /// <param name="align">The alignment of the header and footer Text.</param>
        /// <param name="name">The name represets either the header part or the footer part.</param>
        /// <param name="height">The height of the Header or Footer.</param>
        /// <param name="width">The width of the Header or Footer.</param>
        /// <param name="pageWidth">Width of the page.</param>
        private void GetHeaderInformation(IPageSetupBase pageSetupBase, Dictionary<string, HeaderFooterFontColorSettings> fontColorSettings,
                                          string align, string name, out float height, out float width,
                                          float pageWidth)
        {
            int imageHeight = 0;
            int imageWidth = 0;
            width = 0;
            Font font = new Font("Times New Roman", 12);

            PdfStringFormat format = new PdfStringFormat();
            StringBuilder builder = new StringBuilder();
            if (align == "Left")
            {
                format.Alignment = PdfTextAlignment.Left;
            }
            else if (align == "Center")
            {
                format.Alignment = PdfTextAlignment.Center;
            }
            else if (align == "Right")
            {
                format.Alignment = PdfTextAlignment.Right;
            }
            else
            {
                format.Alignment = PdfTextAlignment.Justify;
            }

            foreach (KeyValuePair<string, HeaderFooterFontColorSettings> headerFooterCollections in fontColorSettings)
            {
                string value = headerFooterCollections.Key.Remove(headerFooterCollections.Key.IndexOf('|'),
                                                        headerFooterCollections.Key.Length - headerFooterCollections.Key.IndexOf('|'));
                if (value != "&G")
                {
                    builder.Append(value);
                }
                else
                {
                    this.GetTemplateHeight(pageSetupBase, name, align, out imageHeight, out imageWidth);
                }
                if (headerFooterCollections.Value != null)
                    font = headerFooterCollections.Value.Font;
                else
                    font = new Font(this.workBook.StandardFont, (float)this.workBook.StandardFontSize);
            }
            //Checking the condition for the unicode text(eg:chinese,japanese etc)
            if (Encoding.UTF8.GetByteCount(builder.ToString()) != (builder.ToString().Length))
            {
                this.excelToPdfSettings.EmbedFonts = true;
            }
            PdfFont fontPdf = new PdfTrueTypeFont(font, this.excelToPdfSettings.EmbedFonts);
            SizeF size = fontPdf.MeasureString(builder.ToString(), format);
            if (imageHeight != 0)
            {
                size.Height = size.Height + imageHeight;
            }

            if (align == "Right")
            {
                if (this.workSheet.PageSetup.RightMargin == 0)
                    width = pageWidth + (pdfSection.PageSettings.Margins.Left - (size.Width + imageWidth));
                else
                    width = pageWidth - (size.Width + imageWidth);
            }
            else
            {
                width = pageWidth;
            }

            height = size.Height;
        }

        /// <summary>
        /// Gets the header footer information.
        /// </summary>
        /// <param name="headerFooterCollection">The header footer collection.</param>
        /// <param name="pdfSection">The PDF section.</param>
        /// <param name="sheet">The IWorksheet object.</param>
        /// <param name="fontColorSettings">The font color settings.</param>
        /// <param name="pdfTemplate">The PDF template.</param>
        /// <param name="width">The width of the header or footer.</param>
        /// <param name="height">The height of the header or footer.</param>
        /// <param name="align">The alignment of the header and footer text.</param>
        /// <param name="name">The name represents either the header or footer.</param>
        /// <param name="pageNumber">The page number of the pdf document.</param>
        private void GetHeaderFooterInformation(List<HeaderFooter> headerFooterCollection, PdfSection pdfSection, IPageSetupBase pageSetupBase,
                                          Dictionary<string, HeaderFooterFontColorSettings> fontColorSettings,
                                          PdfTemplate pdfTemplate, float width, float height, string align,
                                          string name, string pageNumber)
        {
            string value = string.Empty;
            Font font = new Font("Times New Roman", 12);
            Color color = new Color();
            PdfStringFormat pdfFormat = new PdfStringFormat();
            StringBuilder builder = new StringBuilder();
            float resizeWidth = (pdfTemplate.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)) / 3;
            if (align == "Left")
            {
                pdfFormat.Alignment = PdfTextAlignment.Left;
            }
            else if (align == "Center")
            {
                pdfFormat.Alignment = PdfTextAlignment.Center;
            }
            else if (align == "Right")
            {
                pdfFormat.Alignment = PdfTextAlignment.Right;
            }
            else
            {
                pdfFormat.Alignment = PdfTextAlignment.Justify;
            }

            foreach (KeyValuePair<string, HeaderFooterFontColorSettings> headerFooterCollections in fontColorSettings)
            {
                value = headerFooterCollections.Key.Remove(headerFooterCollections.Key.IndexOf('|'),
                                                 headerFooterCollections.Key.Length - headerFooterCollections.Key.IndexOf('|'));
                if (value == "&G")
                {
                    this.DrawHeaderFooterImages(pageSetupBase, width, pdfTemplate, align, name, pdfSection);
                    AllowHeaderFooterOnce = false;
                }
                else if (value == "&P")
                {
                    builder.Append(pageNumber);
                }
                else if (value == "&N")
                {
                    builder.Append(pdfSection.Pages.Count);
                }
                else
                {
                    builder.Append(value);
                }
                if (headerFooterCollections.Value != null)
                {
                    color = headerFooterCollections.Value.FontColor;
                    font = headerFooterCollections.Value.Font;
                }
                else
                {
                    color = Color.Black;
                    font = new Font(this.workBook.StandardFont, (float)this.workBook.StandardFontSize);
                }


            }

            HeaderFooterSection topHeaderSection = new HeaderFooterSection();
            foreach (HeaderFooter headerFooter in headerFooterCollection)
            {
                if (headerFooter.HeaderFooterName == "Header")
                {
                    foreach (HeaderFooterSection section in headerFooter.HeaderFooterSections)
                    {
                        if (section.SectionName == "Right")
                        {
                            topHeaderSection = section;
                        }
                    }
                }
            }

            if (align == "Right" && name == "Footer" && headerFooterCollection[0].HeaderFooterName == "Header")
            {
                if (headerFooterCollection[0].HeaderFooterSections.Contains(topHeaderSection))
                {
                    width = headerFooterCollection[0].HeaderFooterSections[headerFooterCollection[0].HeaderFooterSections.IndexOf(topHeaderSection)].Width;
                }
            }
            //Checking the condition for the unicode text(eg:chinese,japanese etc)
            if (Encoding.UTF8.GetByteCount(builder.ToString()) != (builder.ToString().Length))
            {
                this.excelToPdfSettings.EmbedFonts = true;
            }
            PdfFont fontPdf = new PdfTrueTypeFont(font, this.excelToPdfSettings.EmbedFonts);
            SizeF size = fontPdf.MeasureString(builder.ToString(), pdfFormat);
            if (name == "Footer" && footerMargin != bottomMargin && height > pdfTemplate.Height)
                height = BottomMargin - (footerMargin + size.Height);
            else
                height = size.Height;

            float x = 0;
            float y = 0;

            if (align == "Center")
            {
                x = ((pdfTemplate.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)) - scaledPageHFWidth) / 2;
                x -= size.Width / 2 + (leftMargin + rightMargin);
            }
            else if (align == "Right")
            {
                x = pdfTemplate.Width - (size.Width + (leftMargin + rightMargin));
            }
            if (pdfTemplate.Width < size.Width)
            {
                x = x - width;
                width = pdfTemplate.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right);
                height = pdfTemplate.Height;
            }
            if (size.Width > width)
            {
                size.Width = width;
            }

            pdfTemplate.Graphics.DrawString(builder.ToString(),
                                            new PdfTrueTypeFont(font, this.excelToPdfSettings.EmbedFonts),
                                            new PdfSolidBrush(color), new RectangleF(x, y, size.Width, height), pdfFormat);
        }
#else
        /// <summary>
        /// Gets the header information.
        /// </summary>
        /// <param name="sheet">The IWorksheet object.</param>
        /// <param name="fontColorSettings">The font color settings.</param>
        /// <param name="align">The alignment of the header and footer Text.</param>
        /// <param name="name">The name represets either the header part or the footer part.</param>
        /// <param name="height">The height of the Header or Footer.</param>
        /// <param name="width">The width of the Header or Footer.</param>
        /// <param name="pageWidth">Width of the page.</param>
        private List<RichTextString> GetHeaderInformation(IPageSetupBase pageSetupBase, Dictionary<string, HeaderFooterFontColorSettings> fontColorSettings,
                                          string align, string name, out float height, out float width,
                                          float pageWidth)
        {
            int imageHeight = 0;
            int imageWidth = 0;            
            Font font = new Font("Times New Roman", 12);                      
            RichTextString richTextHF = new RichTextString(this.workBook.Application, workSheet, false, true);
            List<SizeF> richTextSizeF = new List<SizeF>();
            List<RichTextString> richTextCollections = new List<RichTextString>();
                  
            foreach (KeyValuePair<string, HeaderFooterFontColorSettings> headerFooterCollections in fontColorSettings)
            {
                RichTextString richTextSection = new RichTextString(workBook.Application, workSheet, false, true);                
                
                string value = headerFooterCollections.Key.Remove(headerFooterCollections.Key.IndexOf('|'),
                                                        headerFooterCollections.Key.Length - headerFooterCollections.Key.IndexOf('|'));

                if (value == "&G")
                {
                    this.GetTemplateHeight(pageSetupBase, name, align, out imageHeight, out imageWidth);
                    AllowHeaderFooterOnce = false;
                }
                if (headerFooterCollections.Value != null)
                    font = headerFooterCollections.Value.Font;
                else
                    font = new Font(this.workBook.StandardFont, (float)this.workBook.StandardFontSize);

                IFont fontImpl = new FontImpl(workBook.Application, workSheet, font);
                if(headerFooterCollections.Value !=null)
                fontImpl.RGBColor = headerFooterCollections.Value.FontColor;

                richTextHF.AddText(value, fontImpl);
                richTextSection.AddText(value, fontImpl);
                richTextSizeF.Add(richTextSection.StringSize);
                richTextCollections.Add(richTextSection);
            }
            SizeF size = GetMaxWidth(richTextSizeF);
            if (imageHeight != 0)
            {
                size.Height = size.Height + imageHeight;
            }

            if (imageWidth != 0)
            {
                size.Width += imageWidth;
            }
            width = size.Width;      

            height = size.Height;

            return richTextCollections;
        }     

        /// <summary>
        /// Gets the header footer information.
        /// </summary>
        /// <param name="headerFooterCollection">The header footer collection.</param>
        /// <param name="pdfSection">The PDF section.</param>
        /// <param name="sheet">The IWorksheet object.</param>
        /// <param name="fontColorSettings">The font color settings.</param>
        /// <param name="pdfTemplate">The PDF template.</param>
        /// <param name="width">The width of the header or footer.</param>
        /// <param name="height">The height of the header or footer.</param>
        /// <param name="align">The alignment of the header and footer text.</p_waram>
        /// <param name="name">The name represents either the header or footer.</param>
        /// <param name="pageNumber">The page number of the pdf document.</param>
        private void GetHeaderFooterInformation(List<HeaderFooter> headerFooterCollection, PdfSection pdfSection, IWorksheet sheet,
                                          Dictionary<string, HeaderFooterFontColorSettings> fontColorSettings,
                                          PdfTemplate pdfTemplate, float width, float height, string align,
                                          string name, string pageNumber,HeaderFooterSection hfsection,Dictionary<string,PdfTemplate> templates)
        {
            string stringContent = string.Empty;
            Font font = new Font("Times New Roman", 12);              
            bool hasPageContent = false;
            int imageHeight=0;
            int imagewidth = 0;
            RichTextString richTextHFString = new RichTextString(workBook.Application, workSheet,false,true);
            int maxWidth =(int)(pdfTemplate.Width - (pdfSection.PageSettings.Margins.Left + pdfSection.PageSettings.Margins.Right)) / 3;

            foreach (RichTextString rtfString in hfsection.RTF)
            {
                stringContent = rtfString.Text;

                if (stringContent.Contains("&G"))
                {                    
                    this.DrawHeaderFooterImages(sheet, width, pdfTemplate, align, name, pdfSection,richTextHFString,out imageHeight,maxWidth,out imagewidth);
                }
                else if (stringContent.Contains("&P"))
                {
                    hasPageContent = true;
                    stringContent = pageNumber;
                }
                else if (stringContent.Contains("&N"))
                {
                    hasPageContent = true;
                    stringContent = pdfSection.Pages.Count.ToString();
                }

                richTextHFString.AddText(stringContent, rtfString.GetFont(0));
            }

            HeaderFooterSection topHeaderSection = new HeaderFooterSection();
            foreach (HeaderFooter headerFooter in headerFooterCollection)
            {
                if (headerFooter.HeaderFooterName == "Header")
                {
                    foreach (HeaderFooterSection section in headerFooter.HeaderFooterSections)
                    {
                        if (section.SectionName == "Right")
                        {
                            topHeaderSection = section;
                        }
                    }
                }
            }

            if (align == "Right" && name == "Footer" && headerFooterCollection[0].HeaderFooterName == "Header")
            {
                if (headerFooterCollection[0].HeaderFooterSections.Contains(topHeaderSection))
                {
                    width = headerFooterCollection[0].HeaderFooterSections[headerFooterCollection[0].HeaderFooterSections.IndexOf(topHeaderSection)].Width;
                }
            }
            if (name == "Footer" && imageHeight==0)
            {
                if (pdfTemplate.Height > richTextHFString.StringSize.Height)
                {
                    height = pdfTemplate.Height - richTextHFString.StringSize.Height;
                }
                hfsection.Height = pdfTemplate.Height;
            }
            string rtf = richTextHFString.GenerateRtfText(align);
            if (hfsection.Width < richTextHFString.StringSize.Width)
                hfsection.Width = richTextHFString.StringSize.Width;
            PdfTemplate finalTemplate = new PdfTemplate(hfsection.Width, hfsection.Height);           
            PdfMetafile meta = (PdfMetafile)PdfImage.FromRtf(rtf, finalTemplate.Size.Width, finalTemplate.Size.Height, PdfImageType.Metafile);
            
            if (height > richTextHFString.StringSize.Height && hfsection.Width < richTextHFString.StringSize.Width)
            {
                height = richTextHFString.StringSize.Height;
            } 
            meta.Draw(finalTemplate.Graphics, new PointF(0, height));           
            templates.Add(align,finalTemplate);
        }
#endif

        /// <summary>
        /// Gets the header footer options.
        /// </summary>
        /// <param name="pageSetups">The dictionart collection of the header and footer page setups.</param>
        /// <param name="pdfSection">The PDF section.</param>
        /// <param name="pageSetupBase">The worksheet object.</param>
        /// <param name="templateWidth">Width of the template.</param>
        /// <param name="name">The name represents either the header part or the footer part.</param>
        /// <param name="sheetOrchartname">It's Represent the sheet or chart name</param>
        /// <returns></returns>
        private List<HeaderFooterSection> GetHeaderFooterOptions(Dictionary<string, string> pageSetups, PdfSection pdfSection,
                                                 IPageSetupBase pageSetupBase, float templateWidth, string name, string sheetOrchartname)
        {
            if (((pdfSection.Template.Top != null) && (name == "Header"))
                || ((pdfSection.Template.Bottom != null) && (name == "Footer")))
            {
                return null;
            }

            List<HeaderFooterSection> headerFooterSections = new List<HeaderFooterSection>();
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            HeaderFooterSection headerFooterSection = null;
            string[] values;
            string[] headerCount;
            float width = 0;
            bool isRich;

            foreach (KeyValuePair<string, string> pageSetup in pageSetups)
            {
                if (pageSetup.Key == "L")
                {
                    headerFooterSection = new HeaderFooterSection();
                    width = 0;
                    headerFooterSection.Width = width;
                    headerFooterSection.SectionName = "Left";
                    headerFooterSection.TextAlignment = PdfTextAlignment.Left;
                }
                else if (pageSetup.Key == "C")
                {
                    headerFooterSection = new HeaderFooterSection();
                    width = templateWidth / 2;
                    headerFooterSection.Width = width;
                    headerFooterSection.SectionName = "Center";
                    headerFooterSection.TextAlignment = PdfTextAlignment.Center;
                }
                else if (pageSetup.Key == "R")
                {
                    headerFooterSection = new HeaderFooterSection();
                    width = templateWidth;
                    headerFooterSection.Width = width;
                    headerFooterSection.SectionName = "Right";
                    headerFooterSection.TextAlignment = PdfTextAlignment.Right;
                }
                HeaderFooterFontColorSettings fontColorSettings = null;
                string newValue = pageSetup.Value.Replace(CarriageReturnKey, "\r");
                newValue = newValue.Replace(TabKey, "\t");
                newValue = newValue.Replace(NewLineKey, "\n");
                headerCount = this.GetSplitted(newValue);
                List<string> headerValues = new List<string>();
                headerValues.AddRange(headerCount);
                Dictionary<string, HeaderFooterFontColorSettings> headerFooterCollections = new Dictionary<string, HeaderFooterFontColorSettings>();
                float height = 0;
                if (AllowHeaderFooterOnce)
                    AllowHeaderFooterOnce = CheckIsRich(headerCount);
                while (headerValues.Count != 0)
                {

                    if (headerValues[0].StartsWith("&G", StringComparison.Ordinal))
                    {
                        headerFooterCollections.Add(string.Format(formatProvider, "&G|{0}", Guid.NewGuid().ToString()),
                                          fontColorSettings);
                        headerValues.Remove(headerValues[headerValues.IndexOf("&G")]);
                        AllowHeaderFooterOnce = false;
                    }
                    else if (headerValues[0].StartsWith("&A", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider, sheetOrchartname + "|{0}",
                                          Guid.NewGuid().ToString()), fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&P", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider, "&P" + "|{0}",
                                          Guid.NewGuid().ToString()), fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&N", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider, "&N" + "|{0}",
                                          Guid.NewGuid().ToString()), fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&D", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider, DateTime.Now.ToShortDateString() + "|{0}",
                                          Guid.NewGuid().ToString()), fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&T", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider, DateTime.Now.ToShortTimeString() + "|{0}",
                                          Guid.NewGuid().ToString()), fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&U", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                        fontColorSettings.HasUnderline = true;
                    }
                    else if (headerValues[0].StartsWith("&Z", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider, Path.GetDirectoryName(this.workBookImpl.FullFileName) + "|{0}",
                                          Guid.NewGuid().ToString()), fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&F", StringComparison.Ordinal))
                    {
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        headerFooterCollections.Add(string.Format(formatProvider,
                                          Path.GetFileName(this.workBookImpl.FullFileName) + "|{0}", Guid.NewGuid().ToString()),
                                          fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&K", StringComparison.Ordinal))
                    {
                        //&K- Font Color &B- font Bold &I- Font Italic
                        headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                        values = new string[1];
                        headerValues.CopyTo(0, values, 0, 1);
                        fontColorSettings = this.GetHeaderFooterValues(values, values.Length, fontColorSettings);

                        headerValues.Remove(headerValues[0]);
                    }
                    else if (CheckFontValues(headerValues[0]))
                    {
                        //&K- Font Color &B- font Bold &I- Font Italic                        
                        values = new string[1];
                        headerValues.CopyTo(0, values, 0, 1);
                        fontColorSettings = this.GetHeaderFooterValues(values, values.Length, fontColorSettings);
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&S", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&X", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&Y", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&U", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&E", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&O", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].StartsWith("&H", StringComparison.Ordinal))
                    {
                        headerValues.Remove(headerValues[0]);
                    }
                    else if (headerValues[0].Length != 0 && headerValues[0] != string.Empty)
                    {
                        if (headerValues[0].StartsWith("&\"", StringComparison.Ordinal))
                        {
                            string outputValue = string.Empty;
                            if (headerValues[0].StartsWith("&", StringComparison.Ordinal))
                            {
                                headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                            }
                            values = new string[1];
                            headerValues.CopyTo(0, values, 0, 1);
                            fontColorSettings = this.GetHeaderFooterValues(values, values.Length, fontColorSettings);

                            headerValues.RemoveRange(0, 1);
                        }
                        else if (headerValues[0].StartsWith("&", StringComparison.Ordinal) && headerValues[0].Length >= 2 && Char.IsDigit(headerValues[0][1]))
                        {
                            string outputValue = string.Empty;
                            if (!String.IsNullOrEmpty(headerValues[0]))
                            {
                                values = new string[1];
                                if (headerValues[0].StartsWith("&", StringComparison.Ordinal) && !CheckFontValues(headerValues[0]))
                                {
                                    headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                                }
                                headerValues.CopyTo(0, values, 0, 1);
                                fontColorSettings = this.GetHeaderFooterValues(values, values.Length, fontColorSettings);

                                headerValues.RemoveRange(0, 1);
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(headerValues[0]))
                            {
                                if (headerValues[0].StartsWith("&", StringComparison.Ordinal))
                                {
                                    headerValues[0] = headerValues[0].Remove(headerValues[0].IndexOf('&'), 1);
                                }
                                if (headerValues[0].Length > 0)
                                {
                                    values = new string[1];
                                    headerValues.CopyTo(0, values, 0, 1);
                                    fontColorSettings = this.GetHeaderFooterValues(values, values.Length, fontColorSettings);

                                    headerFooterCollections.Add(string.Format(formatProvider, headerValues[0] + "|{0}",
                                                      Guid.NewGuid().ToString()), fontColorSettings);
                                }
                                headerValues.Remove(headerValues[0]);
                            }
                        }
                    }

                }

                if (headerFooterCollections.Count != 0)
                {
                    headerFooterSection.HeaderFooterCollections = headerFooterCollections;
#if SyncfusionFramework4_0 || SyncfusionFramework4_5
                    this.GetHeaderInformation(pageSetupBase, headerFooterCollections, headerFooterSection.SectionName, name, out height,
                                              out width, width);
#else
                    headerFooterSection.RTF = this.GetHeaderInformation(pageSetupBase, headerFooterCollections, headerFooterSection.SectionName, name, out height,
                                              out width, width);
#endif
                    headerFooterSection.Height = height;
                    headerFooterSection.Width = width;
                }

                headerFooterSections.Add(headerFooterSection);
            }

            return headerFooterSections;
        }
        /// <summary>
        /// To get the headerfooter font color settings
        /// </summary>
        /// <param name="headerFooterValues"></param>
        /// <param name="count"></param>
        /// <param name="hfFontColorSettings"></param>
        /// <param name="isRich"></param>
        /// <returns></returns>
        private HeaderFooterFontColorSettings GetHeaderFooterValues(string[] headerFooterValues, int count, HeaderFooterFontColorSettings hfFontColorSettings)
        {
            HeaderFooterFontColorSettings fontColorSettings = hfFontColorSettings != null
                ? hfFontColorSettings.Clone() as HeaderFooterFontColorSettings : null;

            bool isNull = fontColorSettings == null;
            if (fontColorSettings == null)
                fontColorSettings = new HeaderFooterFontColorSettings();

            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            double standardFontSize = this.workBook != null ?
                this.workBook.StandardFontSize :
                this.workSheet.Workbook.StandardFontSize;
            string standardFont = this.workBook != null ?
                this.workBook.StandardFont :
                this.workSheet.Workbook.StandardFont;
            int size = isNull ? System.Convert.ToInt32(standardFontSize) : System.Convert.ToInt32(fontColorSettings.Font.Size);
            Font font = isNull ? new Font(standardFont, size) : new Font(fontColorSettings.Font.Name, size, fontColorSettings.Font.Style);
            string value = null;
            string fontValue = isNull ? standardFont : fontColorSettings.Font.Name;
            Color color = isNull ? Color.FromArgb(255, 0, 0, 0) : fontColorSettings.FontColor;
            for (int i = 0; i < count; i++)
            {
                if ((headerFooterValues[i].Length >= 1 && headerFooterValues[i].Length <= 2) && !CheckDigit(headerFooterValues[i]))
                {
                    for (int x = 0; x < headerFooterValues[i].Length; x++)
                    {
                        if (Char.IsDigit(headerFooterValues[i][x]))
                        {
                            if ((headerFooterValues[i].Length != 1)
                                && (!headerFooterValues[i].StartsWith("&", StringComparison.Ordinal)))
                            {
                                if (Char.IsDigit(headerFooterValues[i][x + 1]))
                                {
                                    size = int.Parse(new String(new char[] { headerFooterValues[i][x], headerFooterValues[i][x + 1] }),
                                                     System.Globalization.NumberStyles.Any, formatProvider);
                                    break;
                                }
                            }
                            else
                            {
                                size = int.Parse(headerFooterValues[i][x].ToString(), System.Globalization.NumberStyles.Any,
                                                 formatProvider);
                                break;
                            }
                        }
                    }
                }

                if (headerFooterValues[i].Substring(0, 1) == "K")
                {
                    color = this.GetHeaderFooterColor(headerFooterValues[i]);
                }
                else if (headerFooterValues[i].IndexOf('\"') == 0)
                {
                    fontValue = headerFooterValues[i];
                }
                else if (headerFooterValues[i].StartsWith("&", StringComparison.Ordinal))
                {
                    if (headerFooterValues[i].Equals(SmallFontUnderlineTagName) || headerFooterValues[i].Equals(BigFontUnderlineTagName))
                    {
                        fontColorSettings.HasUnderline = true;
                    }
                    if (headerFooterValues[i].Equals(SmallFontBoldTagName) || headerFooterValues[i].Equals(BigFontBoldTagName))
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Insert(0, '\"');
                        builder.Append("Bold,");
                        fontValue = builder.ToString();
                    }
                    if (headerFooterValues[i].Equals(SmallFontItalicTagName) || headerFooterValues[i].Equals(BigFontItalicTagName))
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Insert(0, '\"');
                        builder.Append("Italic,");
                        fontValue = builder.ToString();
                    }
                }
                else
                {
                    value = headerFooterValues[i];
                }
            }

            if (fontValue != null)
            {
                bool hasUnderline = hfFontColorSettings != null ? hfFontColorSettings.HasUnderline : false;
                font = this.GetFont(fontValue, size, hasUnderline, font);
            }

            fontColorSettings.Font = font;
            fontColorSettings.FontColor = color;
            return fontColorSettings;
        }

        /// <summary>
        /// Gets the color of the header footer text color.
        /// </summary>
        /// <param name="colorValue">The color value.</param>
        /// <returns>The HeaderFooter Text color.</returns>
        private Color GetHeaderFooterColor(string colorValue)
        {
            Color color = new Color();
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            if (colorValue.Substring(1, 6).Length > 1 && colorValue.Length < 8)
            {
                char[] valueCheck = colorValue.Substring(1, 6).ToCharArray();
                for (int i = 0; i < valueCheck.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(valueCheck[i]))
                    {
                        colorValue = colorValue.Replace(valueCheck[i].ToString(), "0");
                    }
                }

                int redValue = int.Parse(colorValue.Substring(1, 2),
                                         System.Globalization.NumberStyles.HexNumber, formatProvider);
                int greenValue = int.Parse(colorValue.Substring(3, 2),
                                           System.Globalization.NumberStyles.HexNumber, formatProvider);

                int blueValue = int.Parse(colorValue.Substring(5, 2), System.Globalization.NumberStyles.HexNumber,
                                          formatProvider);
                color = Color.FromArgb(redValue, greenValue, blueValue);
            }

            return color;
        }
		/// <summary>
        /// Checks whether the string contains the digit or not.
        /// </summary>
        /// <param name="values">string values</param>
        private bool CheckDigit(string values)
        {
            foreach (char value in values )
            {
                if (char.IsLetter(value))
                {
                    return true;                    
                }                
            }
            return false;
        }
        /// <summary>
        /// Checks whether the string contains the digit or not.
        /// </summary>   
        /// <param name="fontValue">font value</param>
        private bool CheckFontValues(string fontValue)
        {
            string[] fontValues = new string[] { "&b", "&B", "&i", "&I", "&u", "&U","&A" };
            foreach (string value in fontValues)
            {
                if (fontValue.Equals(value))
                    return true;
            }
            return false;
        }

        private bool CheckIsRich(string[] headervalues)
        {
            bool result = true;
            bool stringStarted = false;
            for (int i = 0; i < headervalues.Length; i++)
            {
                bool fontSetting = headervalues[i].StartsWith("&\"") || (headervalues[i].Length > 2 && Char.IsDigit(headervalues[i][1]) && headervalues[i][0] != '&');
                if (headervalues[i] != string.Empty && !fontSetting)
                {
                    stringStarted = true;
                }
                else if (stringStarted && fontSetting)
                {
                    result = false;
                }

            }
            return result;
        }
        /// <summary>
        /// Gets the horizontal alignment from extended format.
        /// </summary>
        /// <param name="extendedFormatStyle">The extended format style.</param>
        /// <param name="cell">The cell range.</param>
        /// <returns>The PdfTextAlignment value.</returns>
        private PdfTextAlignment GetHorizontalAlignmentFromExtendedFormat(IExtendedFormat extendedFormatStyle,
                                                                           IRange cell)
        {
            PdfTextAlignment textAlignment;
            switch (extendedFormatStyle.HorizontalAlignment)
            {
                case ExcelHAlign.HAlignRight:
                    textAlignment = PdfTextAlignment.Right;
                    break;
                case ExcelHAlign.HAlignCenter:
                case ExcelHAlign.HAlignCenterAcrossSelection:
                    textAlignment = PdfTextAlignment.Center;
                    break;
                case ExcelHAlign.HAlignGeneral:
                    if (extendedFormatStyle.Rotation == ExtendedFormatImpl.TopToBottomRotation)
                    {
                        textAlignment = PdfTextAlignment.Center;
                    }
                    else if (cell.HasNumber || cell.HasDateTime ||
                     cell.HasFormula &&
                     cell.FormulaStringValue == null &&
                     !cell.HasFormulaErrorValue &&
                     !cell.HasFormulaBoolValue)
                    {
                        WorkbookImpl book = cell.Worksheet.Workbook as WorkbookImpl;
                        FormatImpl numberFormat = book.InnerFormats[extendedFormatStyle.NumberFormatIndex];
                        ExcelFormatType formatType = numberFormat.GetFormatType(cell.Number);

                        textAlignment = (formatType == ExcelFormatType.Text) ?
                        PdfTextAlignment.Left :
                        PdfTextAlignment.Right;
                    }
                    else
                    {
                        textAlignment = PdfTextAlignment.Left;
                    }

                    break;
                case ExcelHAlign.HAlignLeft:
                default:
                    textAlignment = PdfTextAlignment.Left;
                    break;
            }

            return textAlignment;
        }

        /// <summary>
        /// Gets the horizontal left.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle of the cell display text.</param>
        /// <param name="stringLength">Length of the string.</param>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="verticalAlign">The vertical alignment of the cell text.</param>
        /// <param name="firstCharSize">First char size of the display text .</param>
        /// <param name="nativeFont">The native font of the cell text.</param>
        /// <param name="format">The pdf string format.</param>
        /// <returns>The vector for the text rotation.</returns>
        private PointF GetHorizontalLeft(int rotationAngle, float stringLength, RectangleF rect,
                                         ExcelVAlign verticalAlign, SizeF firstCharSize, PdfFont nativeFont,
                                         PdfStringFormat format)
        {
            int degrees = rotationAngle;
            float hypoLength = stringLength;
            PointF startPoint = PointF.Empty;
            PointF endPoint = PointF.Empty;
            float endX;
            float endY;
            if (!(degrees > 0))
            {
                degrees = -rotationAngle;
            }
            //Vertical-TopAlign
            if (verticalAlign == ExcelVAlign.VAlignTop)
            {
                //Initial Point
                startPoint = rotationAngle < 0
                                           ? new PointF(rect.Left + (firstCharSize.Width / 2),
                                                       rect.Top + (firstCharSize.Width / 2))
                                           : rotationAngle >= 45 && rotationAngle < 90
                                                                                  ? new PointF(rect.Left + (firstCharSize.Height / (float)(90 / rotationAngle)),
                                                                                               rect.Top + (firstCharSize.Width / (float)(90 / rotationAngle)))
                                                                                  : new PointF(rect.Left + firstCharSize.Width, rect.Top);

                if (rotationAngle == 90)
                {
                    startPoint = new PointF(rect.Left + firstCharSize.Height, rect.Top + firstCharSize.Width);
                }

                endX = startPoint.X;
                endY = startPoint.Y;
                //Calculate opposite line length.
                double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                //Calculate adjacent line length.
                double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));

                if (rotationAngle < 0)
                {
                    if (oppLine > rect.Height)
                    {
                        endY += rect.Height;
                        endY -= firstCharSize.Height;
                    }
                    else
                    {
                        endY += (float)oppLine;
                    }
                }
                else
                {
                    endY = startPoint.Y;
                }

                //To find whether the string reached the end of the cell height.
                if (endY > (rect.Bottom + rect.Top))
                {
                    endY = rect.Top + rect.Bottom;
                    endX -= firstCharSize.Width;
                    endY -= firstCharSize.Width;
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Width;

                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != -90)
                        {
                            endX += widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }
                else
                {
                    PointF intersectPoint = PointF.Empty;
                    float xD1, yD1;
                    float length;
                    intersectPoint.X = rect.Right;
                    intersectPoint.Y = rect.Top + (float)(stringLength * Math.Sin(this.DegreeToRadian(degrees)));
                    xD1 = intersectPoint.X - endX;
                    yD1 = intersectPoint.Y - endY;
                    length = (float)Math.Sqrt((xD1 * xD1) + (yD1 * yD1));
                    if (length > rect.Width)
                    {
                        rect.Width = length;
                        this.cellDisplayText = this.GetDisplayText(nativeFont, format, this.cellDisplayText, rect,false);
                    }
                }

                endPoint = new PointF((float)endX, (float)endY);
            }
            //Vertical-CenterAlign
            else if (verticalAlign == ExcelVAlign.VAlignCenter)
            {
                PointF rectTopCoordinate = new PointF(rect.Left, rect.Top);
                PointF rectBottomCoordinate = new PointF(rect.Left, rect.Bottom);
                PointF centerCoordinate = new PointF((rectTopCoordinate.X + rectBottomCoordinate.X) / 2,
                                                     (rectTopCoordinate.Y + rectBottomCoordinate.Y) / 2);
                float splitPoint = (stringLength / 180) * degrees;
                if (rotationAngle > 0)
                {
                    endPoint = new PointF(centerCoordinate.X, centerCoordinate.Y - splitPoint);
                    if (rotationAngle < 45)
                    {
                        endPoint.X += firstCharSize.Width;
                        endPoint.Y -= firstCharSize.Width / 2;
                    }
                    else if (rotationAngle >= 45 && rotationAngle < 90)
                    {
                        endPoint.X += firstCharSize.Height;
                        endPoint.Y -= firstCharSize.Width / 2;
                    }
                    else
                    {
                        endPoint.X += firstCharSize.Height;
                    }
                }
                else
                {
                    endPoint = new PointF(centerCoordinate.X, centerCoordinate.Y + splitPoint);
                    endPoint.X += firstCharSize.Width / 2;
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Width;

                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float heigthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                heigthValue += format.LineSpacing;
                                heigthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != -90)
                        {
                            endPoint.X += heigthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }
            }
            //Vertical-BottomAlign
            else if (verticalAlign == ExcelVAlign.VAlignBottom)
            {
                //Initial Point
                startPoint = rotationAngle < 0
                                           ? new PointF(rect.Left + (firstCharSize.Width / 2),
                                                       rect.Bottom - firstCharSize.Height)
                                           : rotationAngle > 45
                                                          ? new PointF(rect.Left + firstCharSize.Height,
                                                                       rect.Bottom - firstCharSize.Width)
                                                          : new PointF(rect.Left + firstCharSize.Width,
                                                                       rect.Bottom - firstCharSize.Height);

                if (rotationAngle == 90)
                {
                    startPoint = new PointF(rect.Left + firstCharSize.Height, rect.Bottom - firstCharSize.Width);
                }
                else if (rotationAngle == -90)
                {
                    startPoint = new PointF(rect.Left + (firstCharSize.Height / 4),
                                            rect.Bottom - firstCharSize.Width);
                }

                endX = startPoint.X;
                endY = startPoint.Y;
                //Calculate opposite line length.
                double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                //Calculate adjacent line length.
                double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));

                if (rotationAngle > 0)
                {
                    if (rotationAngle != 90)
                    {
                        endY -= (float)oppLine;
                    }
                    else
                    {
                        if (oppLine > rect.Height)
                        {
                            endY -= (float)rect.Height;
                            endY += firstCharSize.Height;
                        }
                        else
                        {
                            endY -= (float)oppLine;
                        }
                    }
                }

                //To find whether the string reached the end of the cell height.
                if (endY > (rect.Bottom + rect.Top))
                {
                    endY = rect.Top + rect.Bottom;
                    endX -= firstCharSize.Width / 2;
                    endY -= firstCharSize.Width / 2;
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Height;
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != -90)
                        {
                            endX += widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }
                else
                {
                    PointF intersectPoint = PointF.Empty;
                    float xD1, yD1;
                    float length;
                    intersectPoint.X = rect.Right;
                    intersectPoint.Y = rect.Top + (float)(stringLength * Math.Sin(this.DegreeToRadian(degrees)));
                    xD1 = intersectPoint.X - endX;
                    yD1 = intersectPoint.Y - endY;
                    length = (float)Math.Sqrt((xD1 * xD1) + (yD1 * yD1));
                    if (length > rect.Width)
                    {
                        rect.Width = length;
                        this.cellDisplayText = this.GetDisplayText(nativeFont, format, this.cellDisplayText, rect,false);
                    }
                }

                endPoint = new PointF((float)endX, (float)endY);
            }

            return endPoint;
        }

        /// <summary>
        /// Gets the horizontal right.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle of the cell display text.</param>
        /// <param name="stringLength">Length of the string.</param>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="verticalAlign">The vertical alignment of the cell text.</param>
        /// <param name="lastCharSize">Last char size of the display text.</param>
        /// <param name="firstCharSize">First char size of the display text .</param>
        /// <param name="nativeFont">The native font of the cell text.</param>
        /// <param name="format">The pdf string format.</param>
        /// <returns>The vector for the text rotation.</returns>
        private PointF GetHorizontalRight(int rotationAngle, float stringLength, RectangleF rect,
                                          ExcelVAlign verticalAlign, SizeF lastCharSize, SizeF firstCharSize,
                                          PdfFont nativeFont, PdfStringFormat format)
        {
            int degrees = rotationAngle;
            float hypoLength = stringLength;
            PointF startPoint = PointF.Empty;
            PointF endPoint = PointF.Empty;
            float endX;
            float endY;
            if (!(degrees > 0))
            {
                degrees = -rotationAngle;
            }
            //Vertical-TopAlign
            if (verticalAlign == ExcelVAlign.VAlignTop)
            {
                //Initial Point
                startPoint = rotationAngle < 0
                                           ? rotationAngle > -45
                                                           ? new PointF(rect.Right - lastCharSize.Width, rect.Top)
                                                           : new PointF(rect.Right - lastCharSize.Height, rect.Top + lastCharSize.Width)
                                                           : rotationAngle < 45
                                                                           ? new PointF(rect.Right - lastCharSize.Width, rect.Top)
                                                                           : new PointF(rect.Right, rect.Top);

                endX = startPoint.X;
                endY = startPoint.Y;
                //Calculate opposite line length.
                double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                //Calculate adjacent line length.
                double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));
                endX -= (float)adjLine;
                if (rotationAngle < 0)
                {
                    if (oppLine > rect.Height)
                    {
                        oppLine = rect.Height;
                        oppLine -= firstCharSize.Width;
                    }

                    endY += (float)oppLine;
                }
                else
                {
                    endY = startPoint.Y;
                    if (rotationAngle >= 45)
                    {
                        endX -= firstCharSize.Width;
                        endY += firstCharSize.Width;
                    }
                    else
                    {
                        endX -= firstCharSize.Height / (float)(90 / degrees);
                        endY += firstCharSize.Width;
                    }
                }

                //To find whether the string reached the end of the cell height.
                if (endY > (rect.Bottom + rect.Top))
                {
                    endY = rect.Top + rect.Bottom;
                    endX -= firstCharSize.Width;
                    endY -= firstCharSize.Width;
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Width;
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != 90)
                        {
                            endX -= widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }
                else
                {
                    PointF intersectPoint = PointF.Empty;
                    float xD1, yD1;
                    float length;
                    intersectPoint.X = rect.Width;
                    intersectPoint.Y = rect.Y + (float)(stringLength * Math.Sin(this.DegreeToRadian(degrees)));
                    xD1 = intersectPoint.X - endX;
                    yD1 = intersectPoint.Y - endY;
                    length = (float)Math.Sqrt((xD1 * xD1) + (yD1 * yD1));
                    if (length > rect.Width)
                    {
                        rect.Width = length;
                        this.cellDisplayText = this.GetDisplayText(nativeFont, format, this.cellDisplayText, rect,false);
                    }
                }

                endPoint = new PointF((float)endX, (float)endY);
            }
            //Vertical-CenterAlign
            else if (verticalAlign == ExcelVAlign.VAlignCenter)
            {
                PointF rectTopCoordinate = new PointF(rect.Right, rect.Top);
                PointF rectBottomCoordinate = new PointF(rect.Right, rect.Bottom);
                PointF centerCoordinate = new PointF((rectTopCoordinate.X + rectBottomCoordinate.X) / 2,
                                                     (rectTopCoordinate.Y + rectBottomCoordinate.Y) / 2);
                float splitPoint = (stringLength / 180) * degrees;
                if (rotationAngle > 0)
                {
                    startPoint = new PointF(centerCoordinate.X, centerCoordinate.Y + splitPoint);
                    if (rotationAngle > 0 && rotationAngle <= 45)
                    {
                        startPoint.X -= lastCharSize.Height / (90 / degrees);
                        startPoint.Y += lastCharSize.Width;
                    }
                    else
                    {
                        startPoint.X -= lastCharSize.Width;
                        startPoint.Y += lastCharSize.Width;
                    }
                }
                else
                {
                    startPoint = new PointF(centerCoordinate.X, centerCoordinate.Y - splitPoint);
                    if (rotationAngle <= -45 && rotationAngle >= -90)
                    {
                        startPoint.X -= lastCharSize.Height;
                        startPoint.Y -= lastCharSize.Width / (90 / degrees);
                    }
                    else
                    {
                        startPoint.X -= lastCharSize.Width;
                        startPoint.Y -= lastCharSize.Width / (90 / degrees);
                    }
                }

                double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                //Calculate adjacent line length.
                double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));
                endX = startPoint.X;
                endY = startPoint.Y;
                endX -= (float)adjLine;
                if (rotationAngle > 0)
                {
                    endY -= (float)oppLine;
                    if (endY < rect.Top)
                    {
                        endY = rect.Top;
                    }
                }
                else
                {
                    endY += (float)oppLine;
                    if (endY > rect.Bottom)
                    {
                        endY = rect.Bottom;
                    }
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Width;
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != 90)
                        {
                            endX -= widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }

                endPoint = new PointF(endX, endY);
            }
            //Vertical-BottomAlign
            else if (verticalAlign == ExcelVAlign.VAlignBottom)
            {
                //Initial Point
                startPoint = rotationAngle < 0
                                           ? rotationAngle < -45
                                                           ? new PointF(rect.Right - (lastCharSize.Width / (90 / degrees)),
                                                                        rect.Bottom - lastCharSize.Width)
                                                           : rotationAngle > -23
                                                                           ? new PointF(rect.Right - lastCharSize.Width, rect.Bottom - lastCharSize.Height)
                                                                           : new PointF(rect.Right - lastCharSize.Width, rect.Bottom - lastCharSize.Width)
                                                                           : rotationAngle > 45
                                                                                           ? new PointF(rect.Right - (lastCharSize.Width / (90 / degrees)), rect.Bottom - (lastCharSize.Width / (90 / degrees)))
                                                                                           : new PointF(rect.Right - lastCharSize.Width, rect.Bottom - lastCharSize.Height);

                endX = startPoint.X;
                endY = startPoint.Y;
                //Calculate opposite line length.
                double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                //Calculate adjacent line length.
                double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));
                endX -= (float)adjLine;
                if (rotationAngle < 0)
                {
                    if (rotationAngle > -45)
                    {
                        endY -= firstCharSize.Height / (90 / degrees);
                        endX -= firstCharSize.Width / (90 / degrees);
                    }
                    else if (rotationAngle > -90)
                    {
                        endY -= firstCharSize.Width / (90 / degrees);
                        endX -= firstCharSize.Width / (90 / degrees);
                    }
                    else
                    {
                        endX -= firstCharSize.Width;
                    }
                }
                else
                {
                    if (rotationAngle != 90)
                    {
                        endY -= (float)oppLine;
                    }
                    else
                    {
                        if (oppLine > rect.Height)
                        {
                            endY -= rect.Height;
                            endY += firstCharSize.Height;
                        }
                        else
                        {
                            endY -= (float)oppLine;
                        }
                    }
                }

                //To find whether the string reached the end of the cell height.
                if (endY > (rect.Bottom + rect.Top))
                {
                    endY = rect.Top + rect.Bottom;
                    endX -= firstCharSize.Width;
                    endY -= firstCharSize.Width;
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Height;
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    if (splittedTexts.Count > 0)
                    {
                        splitValue.Append(splittedTexts[0]);
                        splittedTexts.RemoveAt(0);
                    }
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != 90)
                        {
                            endX -= widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }
                else
                {
                    PointF intersectPoint = PointF.Empty;
                    float xD1, yD1;
                    float length;
                    intersectPoint.X = rect.Width;
                    intersectPoint.Y = rect.Y + (float)(stringLength * Math.Sin(this.DegreeToRadian(degrees)));
                    xD1 = intersectPoint.X - endX;
                    yD1 = intersectPoint.Y - endY;
                    length = (float)Math.Sqrt((xD1 * xD1) + (yD1 * yD1));
                    if (length > rect.Width)
                    {
                        rect.Width = length;
                        this.cellDisplayText = this.GetDisplayText(nativeFont, format, this.cellDisplayText, rect,false);
                    }
                }

                endPoint = new PointF((float)endX, (float)endY);
            }

            return endPoint;
        }

        /// <summary>
        /// Gets the horizontal center.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle of the cell display text.</param>
        /// <param name="stringLength">Length of the string.</param>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="verticalAlign">The vertical alignment of the cell text.</param>
        /// <param name="lastCharSize">Last char size of the display text.</param>
        /// <param name="firstCharSize">First char size of the display text .</param>
        /// <param name="nativeFont">The native font of the cell text.</param>
        /// <param name="format">The pdf string format.</param>
        /// <returns>The vector for the text rotation.</returns>
        private PointF GetHorizontalCenter(int rotationAngle, float stringLength, RectangleF rect,
                                           ExcelVAlign verticalAlign, SizeF lastCharSize, SizeF firstCharSize,
                                           PdfFont nativeFont, PdfStringFormat format)
        {
            int degrees = rotationAngle;
            float hypoLength = stringLength;
            PointF startPoint = PointF.Empty;
            PointF endPoint = PointF.Empty;
            float endX;
            float endY;

            if (!(degrees > 0))
            {
                degrees = -rotationAngle;
            }
            //Vertical-TopAlign
            if (verticalAlign == ExcelVAlign.VAlignTop)
            {
                PointF rectLeftCoordinate = new PointF(rect.Left, rect.Top);
                PointF rectRightCoordinate = new PointF(rect.Right, rect.Top);
                PointF centerCoordinate = new PointF((rectLeftCoordinate.X + rectRightCoordinate.X) / 2,
                                                     (rectLeftCoordinate.Y + rectRightCoordinate.Y) / 2);
                float splitPoint = (stringLength / 180) * (90 - degrees);
                endX = centerCoordinate.X;
                endY = centerCoordinate.Y;
                if (rotationAngle < 0)
                {
                    startPoint = new PointF(centerCoordinate.X + splitPoint, centerCoordinate.Y);
                    startPoint.Y += lastCharSize.Width;
                    double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                    //Calculate adjacent line length.
                    double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));
                    endX = (float)(startPoint.X - adjLine);
                    endY = (float)(startPoint.Y + oppLine);
                    if (endY > rect.Bottom)
                    {
                        endY = rect.Bottom;
                        endY -= firstCharSize.Width;
                    }
                }
                else
                {
                    endX -= splitPoint;
                    endY += firstCharSize.Width;
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Width;
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != 90)
                        {
                            endX -= widthValue;
                        }
                        else
                        {
                            endX += widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }

                endPoint = new PointF(endX, endY);
            }
            //Vertical-CenterAligns
            else if (verticalAlign == ExcelVAlign.VAlignCenter)
            {
                PointF centerPoint = new PointF(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
                hypoLength = stringLength / 2;
                double oppLength = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                double adjLength = hypoLength * Math.Cos(this.DegreeToRadian(degrees));
                if (rotationAngle < 0 && rotationAngle >= -90)
                {
                    centerPoint.Y += (float)oppLength;
                    centerPoint.X -= (float)adjLength + (firstCharSize.Height / 2);
                    endPoint = new PointF(centerPoint.X, centerPoint.Y);
                    if (endPoint.Y > rect.Bottom)
                    {
                        endPoint.Y = rect.Bottom;
                        endPoint.Y -= firstCharSize.Width;
                    }
                }
                else if (rotationAngle > 0 && rotationAngle <= 90)
                {
                    centerPoint.Y -= (float)oppLength;
                    centerPoint.X -= (float)adjLength - (firstCharSize.Height / 2);
                    endPoint = new PointF(centerPoint.X, centerPoint.Y);
                    if (endPoint.Y < rect.Top)
                    {
                        endPoint.Y = rect.Top;
                        endPoint.Y += firstCharSize.Width;
                    }
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    if (firstCharSize.Height > rect.Height)
                    {
                        rect.Width = rect.Width - firstCharSize.Width;
                        endPoint.Y += rect.Height;
                    }
                    else
                    {
                        rect.Width = rect.Height - firstCharSize.Width;
                    }
                    
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    if (splittedTexts.Count > 0)
                    {
                        splitValue.Append(splittedTexts[0]);
                        splittedTexts.RemoveAt(0);
                    }
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }

                        if (rotationAngle != 90)
                        {
                            endPoint.X -= widthValue;
                        }
                        else
                        {
                            endPoint.X += widthValue;
                        }
                    }

                    this.cellDisplayText = splitValue.ToString();
                }
            }
            //Vertical-BottomAlign
            else if (verticalAlign == ExcelVAlign.VAlignBottom)
            {
                PointF rectLeftCoordinate = new PointF(rect.Left, rect.Bottom);
                PointF rectRightCoordinate = new PointF(rect.Right, rect.Bottom);
                PointF centerCoordinate = new PointF((rectLeftCoordinate.X + rectRightCoordinate.X) / 2,
                                                     (rectLeftCoordinate.Y + rectRightCoordinate.Y) / 2);
                float splitPoint = (stringLength / 180) * (90 - degrees);
                endX = centerCoordinate.X;
                endY = centerCoordinate.Y;
                if (rotationAngle < 0)
                {
                    endX -= splitPoint;
                    if (rotationAngle != -90)
                    {
                        endY -= firstCharSize.Height;
                    }
                    else
                    {
                        endY -= firstCharSize.Height / 4;
                    }
                }
                else
                {
                    startPoint = new PointF(centerCoordinate.X + splitPoint, centerCoordinate.Y);
                    if (rotationAngle != 90)
                    {
                        startPoint.Y -= lastCharSize.Height;
                    }

                    double oppLine = hypoLength * Math.Sin(this.DegreeToRadian(degrees));
                    //Calculate adjacent line length.
                    double adjLine = hypoLength * Math.Cos(this.DegreeToRadian(degrees));
                    endX = (float)(startPoint.X - adjLine);
                    endY = (float)(startPoint.Y - oppLine);
                    if (endY < rect.Top)
                    {
                        endY = rect.Top;
                        endY += firstCharSize.Width;
                    }
                }

                if (rotationAngle == 90 || rotationAngle == -90)
                {
                    rect.Width = rect.Height - firstCharSize.Height;
                    List<string> splittedTexts = this.GetDisplayWrapTextList(nativeFont, format,
                                                                             this.cellDisplayText, rect);

                    float widthValue = 0;
                    StringBuilder splitValue = new StringBuilder();
                    splitValue.Append(splittedTexts[0]);
                    splittedTexts.RemoveAt(0);
                    if (splittedTexts.Count != 0)
                    {
                        foreach (string splitText in splittedTexts)
                        {
                            if (splitText != "\r\n")
                            {
                                widthValue += format.LineSpacing;
                                widthValue += nativeFont.MeasureString(splitText[0].ToString(), format).Height;
                            }

                            splitValue.Append(splitText);
                        }
                    }
                    else
                    {
                        widthValue += (nativeFont.MeasureString(this.cellDisplayText, format).Height)/2;
                    }

                    if (rotationAngle != 90)
                    {
                        endX -= widthValue;
                    }
                    else
                    {
                        endX += widthValue;
                    }

                    this.cellDisplayText = splitValue.ToString();
                }

                endPoint = new PointF(endX, endY);
            }

            return endPoint;
        }


        /// <summary>
        /// Gets the Max height of the HeaderFooterSection.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <returns>The Maximum value.</returns>
        private float GetMaxHeight(List<HeaderFooterSection> sections)
        {
            List<float> maxList = new List<float>();
            foreach (HeaderFooterSection sec in sections)
            {
                maxList.Add(sec.Height);
            }

            return this.GetMaxValue(maxList);
        }
        /// <summary>
        /// Gets the width of the max.
        /// </summary>
        /// <param name="sizes">The sizes.</param>
        /// <returns></returns>
        private SizeF GetMaxWidth(List<SizeF> sizes)
        {
            SizeF returnSize = new SizeF();
            float height = 0;
            foreach (SizeF size in sizes)
            {
                if (returnSize.Width < size.Width)
                    returnSize.Width = size.Width;

                height += size.Height;
            }
            returnSize.Height = height;
            return returnSize;
        }

        /// <summary>
        /// Gets the max value.
        /// </summary>
        /// <param name="maxList">The max list.</param>
        /// <returns>The maximum value</returns>
        private float GetMaxValue(List<float> maxList)
        {
            float max = Int32.MinValue;
            for (int i = 0; i < maxList.Count; i++)
            {
                float val = maxList[i];
                if (val > max)
                {
                    max = val;
                }
            }

            return max;
        }

        /// <summary>
        /// Gets the max value.
        /// </summary>
        /// <param name="maxList">The max list.</param>
        /// <returns>The maximum value.</returns>
        private int GetMaxValue(List<int> maxList)
        {
            int max = Int32.MinValue;
            for (int i = 0; i < maxList.Count; i++)
            {
                int val = (int)maxList[i];
                if (val > max)
                {
                    max = val;
                }
            }

            return max;
        }

        /// <summary>
        /// Gets the merged rectangle.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="mergedRegion">The merged region.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <returns>The Merge Rectangle Coordinates.</returns>
        private RectangleF GetMergedRectangle(WorksheetImpl sheet, MergeCellsRecord.MergedRegion mergedRegion,
                                             int firstRow, int firstColumn, int lastRow, int lastColumn,float startX,float startY)
        {
            float y = Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(mergedRegion.RowFrom),
                                                            PdfGraphicsUnit.Point);
            float x = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(mergedRegion.ColumnFrom),
                                                            PdfGraphicsUnit.Point);

            y -= Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(firstRow - 1),
                                                       PdfGraphicsUnit.Point);
            x -= Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(firstColumn - 1),
                                                       PdfGraphicsUnit.Point);
            y += startY;
            x += startX;

            float height = Pdf_UnitConverter.ConvertFromPixels((float)RowHeightGetter.GetTotal(mergedRegion.RowFrom + 1, mergedRegion.RowTo + 1),
                                                                 PdfGraphicsUnit.Point);

            float width = Pdf_UnitConverter.ConvertFromPixels((float)ColumnWidthGetter.GetTotal(mergedRegion.ColumnFrom + 1, mergedRegion.ColumnTo + 1), PdfGraphicsUnit.Point);
            //GetMergedWidth(firstColumn,lastColumn,mergedRegion.ColumnFrom+1,mergedRegion.ColumnTo+1,columnWidthGetter),


            IRange cell = sheet[mergedRegion.RowFrom + 1, mergedRegion.ColumnFrom + 1];
            return new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Gets the resized image.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The resized Image.</returns>
        private Image GetResizedImage(Image originalImage, int width, int height)
        {
            Image newImage = new Bitmap(width, height);
            Graphics gr = Graphics.FromImage(newImage);

            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.CompositingQuality = CompositingQuality.HighQuality;

            gr.DrawImage(originalImage, 0, 0, width, height);
            return newImage;
        }
        /// <summary>
        /// Gets the adjacent range.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        private IRange GetAdjacentRange(IRange cell)
        {
            return workSheet[cell.Row == 1 ?
                cell.Row :
                cell.Row - 1,
                cell.Column,
                cell.LastRow == 1 ?
                cell.LastRow
                : cell.LastRow - 1,
                cell.LastColumn];
        }
        /// <summary>
        /// Gets the scaled page.
        /// </summary>
        /// <param name="originalWidth">Width of the original.</param>
        /// <param name="originalHeight">Height of the original.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <param name="fitPage">if set to <c>true</c> [fit page].</param>
        private void GetScaledPage(float originalWidth, float originalHeight, float maxWidth, float maxHeight,
                                   bool fitPage)
        {
            if (!fitPage)
            {
                if (originalWidth <= maxWidth)
                {
                    maxWidth = originalWidth;
                }

                float newHeight = originalHeight * maxWidth / originalWidth;
                if (newHeight > maxHeight)
                {
                    maxWidth = originalWidth * maxHeight / originalHeight;
                    newHeight = maxHeight;
                }

                this.scaledPageWidth = maxWidth;
                this.scaledPageHeight = maxHeight;
            }
            else
            {
                float width = 0, height = 0;
                float aspectRatio = (float)originalWidth / (float)originalHeight;
                if ((maxHeight > 0) && (maxWidth > 0))
                {
                    if ((originalWidth < maxWidth) && (originalHeight < maxHeight))
                    {
                        width = originalWidth;
                        height = originalHeight;
                    }
                    else if (aspectRatio > 1)
                    {
                        width = maxWidth;
                        height = width / aspectRatio;
                        
                        if (height > maxHeight)
                        {
                            height = maxHeight;
                            width = height * aspectRatio;
                        }
                    }
                    else
                    {
                        height = maxHeight;
                        width = height * aspectRatio;

                        if (width > maxWidth)
                        {
                            width = maxWidth;
                            height = width / aspectRatio;
                        }
                    }
                }
                else if (maxHeight == 0 && originalWidth > maxWidth)
                {
                    width = maxWidth;
                    height = width / aspectRatio;

                    width = (int)(height * aspectRatio);
                }
                else if (maxWidth == 0 && originalHeight >= maxHeight)
                {
                    height = maxHeight;
                    width = height * aspectRatio;
                }
                else
                {
                    width = originalWidth;
                    height = originalHeight;
                }

                this.scaledPageWidth = width;
                this.scaledPageHeight = height;
            }
        }
        /// <summary>
        /// Gets the scaled page.
        /// </summary>
        /// <param name="originalWidth">Width of the original.</param>
        /// <param name="originalHeight">Height of the original.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <param name="fitPage">if set to <c>true</c> [fit page].</param>
        private void GetScaledHFPage(float originalWidth, float originalHeight, float maxWidth, float maxHeight,
                                   bool fitPage)
        {
            if (!fitPage)
            {
                if (originalWidth <= maxWidth)
                {
                    maxWidth = originalWidth;
                }

                float newHeight = originalHeight * maxWidth / originalWidth;
                if (newHeight > maxHeight)
                {
                    maxWidth = originalWidth * maxHeight / originalHeight;
                    newHeight = maxHeight;
                }

                this.scaledPageHFWidth = maxWidth;
                this.scaledPageHFHeight = maxHeight;
            }
            else
            {
                float width = 0, height = 0;
                float aspectRatio = (float)originalWidth / (float)originalHeight;
                if ((maxHeight > 0) && (maxWidth > 0))
                {
                    if ((originalWidth < maxWidth) && (originalHeight < maxHeight))
                    {
                        width = originalWidth;
                        height = originalHeight;
                    }
                    else if (aspectRatio > 1)
                    {
                        width = maxWidth;
                        height = width / aspectRatio;
                        if (height > maxHeight)
                        {
                            height = maxHeight;
                            width = height * aspectRatio;
                        }
                    }
                    else
                    {
                        height = maxHeight;
                        width = height * aspectRatio;

                        if (width > maxWidth)
                        {
                            width = maxWidth;
                            height = width / aspectRatio;
                        }
                    }
                }
                else if (maxHeight == 0 && originalWidth > maxWidth)
                {
                    width = maxWidth;
                    height = width / aspectRatio;

                    width = (int)(height * aspectRatio);
                }
                else if (maxWidth == 0 && originalHeight >= maxHeight)
                {
                    height = maxHeight;
                    width = height * aspectRatio;
                }
                else
                {
                    width = originalWidth;
                    height = originalHeight;
                }

                this.scaledPageHFWidth = width;
                this.scaledPageHFHeight = height;
            }
        }
        /// <summary>
        /// Gets the scaled picture.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <returns>The scaled Image</returns>
        private Image GetScaledPicture(Image originalImage, int maxWidth, int maxHeight)
        {
            int width, height;
            Image newImage = null;
            float imageWidth = Pdf_UnitConverter.ConvertFromPixels((float)originalImage.Width, PdfGraphicsUnit.Point);
            float imageHeight = Pdf_UnitConverter.ConvertFromPixels((float)originalImage.Height, PdfGraphicsUnit.Point);
            float aspectRatio = (float)originalImage.Width / (float)originalImage.Height;

            if ((maxHeight > 0) && (maxWidth > 0))
            {
                if ((originalImage.Width < maxWidth) && (originalImage.Height < maxHeight))
                {
                    return originalImage;
                }
                else if (aspectRatio > 1)
                {
                    width = maxWidth;
                    height = (int)(width / aspectRatio);
                    if (height > maxHeight)
                    {
                        height = maxHeight;
                        width = (int)(height * aspectRatio);
                    }
                }
                else
                {
                    height = maxHeight;
                    width = (int)(height * aspectRatio);
                    if (width > maxWidth)
                    {
                        width = maxWidth;
                        height = (int)(width / aspectRatio);
                    }
                }
            }
            else if ((maxHeight == 0) && (originalImage.Width > maxWidth))
            {
                width = maxWidth;
                height = (int)(width / aspectRatio);
            }
            else if ((maxWidth == 0) && (originalImage.Height > maxHeight))
            {
                height = maxHeight;
                width = (int)(height * aspectRatio);
            }
            else
            {
                newImage = this.GetResizedImage(originalImage, (int)imageWidth,(int) imageHeight);
                return newImage;             
            }

            newImage = this.GetResizedImage(originalImage, width, height);
            return newImage;
        }
        private int GetNextIndex(char ch, int currentIndex, List<char> charList)
        {
            for (int i = currentIndex + 1, len = charList.Count; i <= len; i++)
            {
                if ((int)charList[i] == 34)
                {
                    return i + 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// Gets the width of the sorted.
        /// </summary>
        /// <param name="align">The align.</param>
        /// <param name="temps">The temps.</param>
        /// <param name="dividedWidth">Width of the divided.</param>
        /// <returns></returns>
        private float GetSortedWidth(string align, Dictionary<string, PdfTemplate> temps, float dividedWidth)
        {            
            float width = 0;
            Alignment preserveAlignment= Alignment.None;
            float actualWidth = 0;
            float leftWidth = 0;
            float centerWidth = 0;
            float rightWidth = 0;
            foreach (KeyValuePair<string, PdfTemplate> temp in temps)
            {
                Alignment al = (Alignment)Enum.Parse(typeof(Alignment), temp.Key);
                
                    if (temp.Key == align)
                    {
                        width += dividedWidth;
                        preserveAlignment = al;
                        actualWidth = temp.Value.Width;
                    }
                    else
                    {
                        PdfTemplate outTemplate=null;
                        if (preserveAlignment == Alignment.Left)
                        {
                            leftWidth = width;                            
                            if (!temps.TryGetValue("Center", out outTemplate))
                            {
                                leftWidth += dividedWidth;
                                if (width < actualWidth)
                                {
                                    if (!temps.TryGetValue("Right", out outTemplate))
                                    {
                                        leftWidth += dividedWidth;
                                    }
                                }
                            }
                        }
                        else if (preserveAlignment == Alignment.Center)
                        {                            
                            centerWidth = width;
                            if (!temps.TryGetValue("Left", out outTemplate))
                            {
                                centerWidth += dividedWidth;
                            }
                            else if(leftWidth < dividedWidth)
                            {
                                centerWidth += (dividedWidth - leftWidth);
                            }

                            if (!temps.TryGetValue("Right", out outTemplate))
                            {
                                centerWidth += dividedWidth;
                            }
                            else
                            {
                                centerWidth += outTemplate.Width;
                            }
                        }
                        else if (preserveAlignment == Alignment.Right)
                        {
                            rightWidth = width;                            
                            if (!temps.TryGetValue("Center", out outTemplate))
                            {
                                rightWidth += dividedWidth;
                                if (width < actualWidth)
                                {
                                    if (!temps.TryGetValue("Right", out outTemplate))
                                    {
                                        rightWidth += dividedWidth;
                                    }
                                }
                            }
                            else if (centerWidth < dividedWidth)
                            {
                                rightWidth += (dividedWidth - centerWidth);
                            }
                        }
                    }                
            }

            if (preserveAlignment == Alignment.Left)
                return leftWidth;
            if (preserveAlignment == Alignment.Right)
                return rightWidth;
            if (preserveAlignment == Alignment.Center)
                return centerWidth;


            return width;
        }
        /// <summary>
        /// Gets the splitted header and footer text.
        /// </summary>
        /// <param name="pageSetup">The page setup.</param>
        /// <returns>The Array of splitted strings</returns>
        private string[] GetSplitted(string pageSetup)
        {

            bool success = false;
            string splittedString = string.Empty;
            while (!success)
            {
                char[] ch = pageSetup.ToCharArray();
                List<char> list = new List<char>();
                list.AddRange(ch);
                for (int i = 0; i < list.Count; )
                {
                    if (list[i] == '&')
                    {
                        list.Insert(i, '$');
                        if (list[i + 2] == 'K')
                        {
                            list.Insert(i + 9, '$');
                            i = i + 9;
                        }
                        else if (list[i + 2] != '\"')
                        {
                            if (!Char.IsDigit(list[i + 2]))
                            {
                                list.Insert(i + 3, '$');
                                i = i + 4;
                            }
                            else
                            {
                                if (Char.IsDigit(list[i + 3]))
                                {
                                    list.Insert(i + 4, '$');
                                    i = i + 5;
                                }
                                else
                                {
                                    list.Insert(i + 3, '$');
                                    i = i + 4;
                                }
                            }
                        }
                        else
                        {
                            if ((int)list[i + 2] == 34)
                            {
                                int nextIndex = GetNextIndex('"', i + 2, list);
                                list.Insert(nextIndex, '$');
                                i = i + 1;
                            }

                            i = i + 2;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }

                ch = new char[list.Count];
                ch = list.ToArray();
                splittedString = new string(ch);
                success = true;
            }

            string[] arr = splittedString.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
            return arr;
        }

        /// <summary>
        /// Gets the start index of the end border.
        /// </summary>
        /// <param name="borderIndex">Index of the Excel border.</param>
        /// <param name="start">The start Excelborder index.</param>
        /// <param name="end">The end Excel border index.</param>
        private void GetStartEndBorderIndex(ExcelBordersIndex borderIndex, out ExcelBordersIndex start,
                                             out ExcelBordersIndex end)
        {
            start = (ExcelBordersIndex)(-1);
            end = (ExcelBordersIndex)(-1);
            switch (borderIndex)
            {
                case ExcelBordersIndex.EdgeTop:
                case ExcelBordersIndex.EdgeBottom:
                    start = ExcelBordersIndex.EdgeLeft;
                    end = ExcelBordersIndex.EdgeRight;
                    break;
                case ExcelBordersIndex.EdgeRight:
                case ExcelBordersIndex.EdgeLeft:
                    start = ExcelBordersIndex.EdgeTop;
                    end = ExcelBordersIndex.EdgeBottom;
                    break;
            }
        }

        /// <summary>
        /// Gets the height of the template.
        /// </summary>
        /// <param name="pageSetup">The page setup object of the sheet.</param>
        /// <param name="name">The name represents either the header part or the footer part.</param>
        /// <param name="align">The alignent of the header and footer text.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="imageWidth">Width of the image.</param>
        private void GetTemplateHeight(IPageSetupBase pageSetup, string name, string align, out int imageHeight,
                                        out int imageWidth)
        {
            imageHeight = 0;
            imageWidth = 0;

            if (name == "Header")
            {
                if (pageSetup.LeftHeaderImage != null && pageSetup.LeftHeader.Contains("&G")
                    && align == "Left")
                {
                    imageHeight = pageSetup.LeftHeaderImage.Height;
                    imageWidth = pageSetup.LeftHeaderImage.Width;
                }

                if (pageSetup.CenterHeaderImage != null && pageSetup.CenterHeader.Contains("&G")
                    && align == "Center")
                {
                    imageHeight = pageSetup.CenterHeaderImage.Height;
                    imageWidth = pageSetup.CenterHeaderImage.Width;
                }

                if (pageSetup.RightHeaderImage != null && pageSetup.RightHeader.Contains("&G")
                    && align == "Right")
                {
                    imageHeight = pageSetup.RightHeaderImage.Height;
                    imageWidth = pageSetup.RightHeaderImage.Width;
                }
            }
            else if (name == "Footer")
            {
                if (pageSetup.LeftFooterImage != null && pageSetup.LeftFooter.Contains("&G") && align == "Left")
                {
                    imageHeight = pageSetup.LeftFooterImage.Height;
                    imageWidth = pageSetup.LeftFooterImage.Width;
                }

                if (pageSetup.CenterFooterImage != null && pageSetup.CenterFooter.Contains("&G")
                    && align == "Center")
                {
                    imageHeight = pageSetup.CenterFooterImage.Height;
                    imageWidth = pageSetup.CenterFooterImage.Width;
                }

                if (pageSetup.RightFooterImage != null && pageSetup.RightFooter.Contains("&G")
                    && align == "Right")
                {
                    imageHeight = pageSetup.RightFooterImage.Height;
                    imageWidth = pageSetup.RightFooterImage.Width;
                }
            }
        }

        /// <summary>
        /// Gets the text alignment from shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>The PdfTextAlignment value.</returns>
        private PdfTextAlignment GetTextAlignmentFromShape(ITextBoxShape shape)
        {
            PdfTextAlignment textAlignment;
            switch (shape.HAlignment)
            {
                case ExcelCommentHAlign.Center:
                    textAlignment = PdfTextAlignment.Center;
                    break;
                case ExcelCommentHAlign.Left:
                    textAlignment = PdfTextAlignment.Left;
                    break;
                case ExcelCommentHAlign.Right:
                    textAlignment = PdfTextAlignment.Right;
                    break;
                default:
                    textAlignment = PdfTextAlignment.Justify;
                    break;
            }

            return textAlignment;
        }

        /// <summary>
        /// Gets the vector.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle of the cell display text.</param>
        /// <param name="stringLength">Length of the string.</param>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="horizontalAlign">The horizontal alignment of the cell text.</param>
        /// <param name="verticalAlign">The vertical alignment of the cell text.</param>
        /// <param name="lastCharSize">Last char size of the display text.</param>
        /// <param name="firstCharSize">First char size of the display text .</param>
        /// <param name="nativeFont">The native font of the cell text.</param>
        /// <param name="format">The pdf string format.</param>
        /// <returns>The vector for the text rotation.</returns>
        private PointF GetVector(int rotationAngle, float stringLength, RectangleF rect,
                                 ExcelHAlign horizontalAlign, ExcelVAlign verticalAlign, SizeF lastCharSize,
                                 SizeF firstCharSize, PdfFont nativeFont, PdfStringFormat format)
        {
            //    B (x1,y1)
            //      /|
            //     / |
            //    /  |
            //   /   |
            //A /____|C(x2,y2)
            //(x3,y3)

            PointF endPoint = PointF.Empty;
            switch (horizontalAlign)
            {
                //Horizontal-LeftAlign
                case ExcelHAlign.HAlignLeft:
                    endPoint = this.GetHorizontalLeft(rotationAngle, stringLength, rect, verticalAlign, firstCharSize, nativeFont, format);
                    break;
                //Horizontal-RightAlign
                case ExcelHAlign.HAlignRight:
                    endPoint = this.GetHorizontalRight(rotationAngle, stringLength, rect, verticalAlign, lastCharSize, firstCharSize, nativeFont, format);
                    break;
                //Horizontal-CenterAlign
                case ExcelHAlign.HAlignCenter:
                case ExcelHAlign.HAlignCenterAcrossSelection:
                    endPoint = this.GetHorizontalCenter(rotationAngle, stringLength, rect, verticalAlign, lastCharSize, firstCharSize, nativeFont, format);
                    break;
                case ExcelHAlign.HAlignGeneral:
                    if(rotationAngle>0)
                        endPoint = this.GetHorizontalLeft(rotationAngle, stringLength, rect, verticalAlign, firstCharSize, nativeFont, format);
                    else
                        endPoint = this.GetHorizontalRight(rotationAngle, stringLength, rect, verticalAlign, lastCharSize, firstCharSize, nativeFont, format);
                    break;
            }

            return endPoint;
        }

        /// <summary>
        /// Gets the vector.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle of the text within the cell.</param>
        /// <param name="width">The total width of the display text.</param>
        /// <param name="adjacentRect">The adjacent cell rect.</param>
        /// <param name="extendedFormatImpl">The extended format impl of the cell.</param>
        /// <param name="lastCharSize">Last char size of the display text. </param>
        /// <param name="firstCharSize">First char size of the display text .</param>
        /// <param name="nativeFont">The native font of the cell.</param>
        /// <param name="format">The pdf string format.</param>
        /// <returns>The vector for the text rotation</returns>
        private PointF GetVector(int rotationAngle, float width, RectangleF adjacentRect,
                                 ExtendedFormatImpl extendedFormatImpl, SizeF lastCharSize, SizeF firstCharSize,
                                 PdfFont nativeFont, PdfStringFormat format)
        {
            if (rotationAngle > 90 || rotationAngle < -90)
            {
                throw new ArgumentException("Unexpected rotation angle.");
            }

            return this.GetVector(rotationAngle, width, adjacentRect, extendedFormatImpl.HorizontalAlignment,
                                      extendedFormatImpl.VerticalAlignment, lastCharSize, firstCharSize, nativeFont,
                                      format);
       }

        /// <summary>
        /// Gets the vertical alignment from extended format.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>The PdfVerticalAlignment value</returns>
        private PdfVerticalAlignment GetVerticalAlignmentFromExtendedFormat(IExtendedFormat style)
        {
            PdfVerticalAlignment verticalAlignment;
            switch (style.VerticalAlignment)
            {
                case ExcelVAlign.VAlignCenter:
                    verticalAlignment = PdfVerticalAlignment.Middle;
                    break;
                case ExcelVAlign.VAlignTop:
                    verticalAlignment = PdfVerticalAlignment.Top;
                    break;
                case ExcelVAlign.VAlignBottom:
                default:
                    verticalAlignment = PdfVerticalAlignment.Bottom;
                    break;
            }

            return verticalAlignment;
        }

        /// <summary>
        /// Gets the vertical alignment from shape.
        /// </summary>
        /// <param name="textBoxShape">The text box shape.</param>
        /// <returns>The pdfVerticalAlignment value</returns>
        private PdfVerticalAlignment GetVerticalAlignmentFromShape(ITextBoxShape textBoxShape)
        {
            PdfVerticalAlignment verticalAlignment;
            switch (textBoxShape.VAlignment)
            {
                case ExcelCommentVAlign.Center:
                    verticalAlignment = PdfVerticalAlignment.Middle;
                    break;

                case ExcelCommentVAlign.Bottom:
                    verticalAlignment = PdfVerticalAlignment.Bottom;
                    break;

                default:
                    verticalAlignment = PdfVerticalAlignment.Top;
                    break;
            }

            return verticalAlignment;
        }

        /// <summary>
        /// Sets the document properties.
        /// </summary>
        private void SetDocumentProperties()
        {
            this.workBook = this.workSheet.Workbook;
            this.pdfDocument.DocumentInformation.Author = this.workBook.BuiltInDocumentProperties.Author;
            this.pdfDocument.DocumentInformation.ModificationDate = DateTime.Now;
            this.pdfDocument.DocumentInformation.Creator = this.workBook.BuiltInDocumentProperties.ApplicationName;
            this.pdfDocument.DocumentInformation.Producer = this.workBook.BuiltInDocumentProperties.Company;
            this.pdfDocument.DocumentInformation.Title = this.workBook.BuiltInDocumentProperties.Title;
            this.pdfDocument.DocumentInformation.Subject = this.workBook.BuiltInDocumentProperties.Subject;
            this.pdfDocument.DocumentInformation.Keywords = this.workBook.BuiltInDocumentProperties.Keywords;
            this.pdfDocument.DocumentInformation.CreationDate = this.workBook.BuiltInDocumentProperties.CreationDate;
        }

        /// <summary>
        /// Sets the hyper link.
        /// </summary>
        /// <param name="cellRect">The cell rectangle.</param>
        private void SetHyperLink(RectangleF cellRect,IRange range)
        {
            IHyperLinks links = this.workSheet.HyperLinks;
            if (links.Count == 0)
            {
                return;
            }
            //Need to change the hyperlink position for pdfpage
            cellRect = resize(cellRect);
            Syncfusion.XlsIO.Implementation.Collections.CollectionBase<Syncfusion.XlsIO.Implementation.HyperLinkImpl> addresss = links as Syncfusion.XlsIO.Implementation.Collections.HyperLinksCollection;
            if (range.Hyperlinks[0].Type == ExcelHyperLinkType.Url)
            {
                PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(cellRect, range.Hyperlinks[0].Address);
                uriAnnotation.Border.Width = 0;
                this.currentPage.Annotations.Add(uriAnnotation);
            }
            else if (range.Hyperlinks[0].Type == ExcelHyperLinkType.File)
            {
                PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(cellRect,
                                                                                     range.Hyperlinks[0].Address);
                fileLinkAnnotation.Border.Width = 0;
                this.currentPage.Annotations.Add(fileLinkAnnotation);
            }
        }
        /// <summary>
        /// To Resize the hyperlink position for fit in the pdf page.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private RectangleF resize(RectangleF rect)
        {
            float x = (this.scaledPageWidth / this.pdfPageTemplate.Width) * rect.X;
            float y = (this.scaledPageHeight / this.pdfPageTemplate.Height) * rect.Y;
            float width = (this.scaledPageWidth / this.pdfPageTemplate.Width) * rect.Width;
            float height = (this.scaledPageWidth / this.pdfPageTemplate.Width) * rect.Height;
            float newHeight = (this.workSheet.PageSetup.CenterVertically ? headerHeight : 0);
            float newstartX = (this.scaledPageWidth / this.pdfPageTemplate.Width) * startX;
            return rect = new RectangleF(x + newstartX, y + newHeight , width, height);
        }

        /// <summary>
        /// Gets the actual used range.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <returns>The IRange object</returns>
        private IRange GetActualUsedRange(IWorksheet sheet)
        {
            if (sheet.UsedRange.Row == sheet.UsedRange.LastRow
                && sheet.UsedRange.Column == sheet.UsedRange.LastColumn)
            {
                return sheet.UsedRange;
            }

            List<int> usedRows = new List<int>();
            List<int> usedColumns = new List<int>();
            List<int> missedRows = new List<int>();
            List<int> missedColumns = new List<int>();
          
            for (int row = sheet.UsedRange.LastRow; row >= sheet.UsedRange.Row; row--)
            {
                IRange rowRange = sheet.Range[row, sheet.UsedRange.Column].EntireRow;

                if (!rowRange.IsBlank)
                {
                    usedRows.Add(row);
                    break;
                }
                else
                {
                    missedRows.Add(row);
                }
            }

            for (int column = sheet.UsedRange.LastColumn; column >= sheet.UsedRange.Column; column--)
            {
                IRange columnRange = sheet.Range[sheet.UsedRange.Row, column].EntireColumn;
                if (!columnRange.IsBlank)
                {
                    usedColumns.Add(column);
                    break;
                }
                else
                {
                    missedColumns.Add(column);
                }
            }

            if (sheet.MergedCells != null)
            {
                foreach (IRange mergeRange in sheet.MergedCells)
                {
                    IRange mergeCheckRange = sheet.Range[mergeRange.Row, mergeRange.Column, mergeRange.LastRow,
                                                         mergeRange.LastColumn];
                    if (!mergeCheckRange.IsBlank || mergeCheckRange.CellStyle.Color.Name != "ffffffff")
                    {
                        if (!usedRows.Contains(mergeRange.Row))
                        {
                            usedRows.Add(mergeRange.Row);
                        }

                        if (!usedRows.Contains(mergeRange.LastRow))
                        {
                            usedRows.Add(mergeRange.LastRow);
                        }

                        if (!usedColumns.Contains(mergeRange.Column))
                        {
                            usedColumns.Add(mergeRange.Column);
                        }

                        if (!usedColumns.Contains(mergeRange.LastColumn))
                        {
                            usedColumns.Add(mergeRange.LastColumn);
                        }
                    }
                }
            }

            missedRows.Sort();
            missedColumns.Sort();
            bool gotlastRowColumn= false;
            int maxRow = usedRows.Count > 0 ? usedRows[usedRows.Count - 1] : missedRows[0];
            if (missedRows.Count != 0 && missedRows[missedRows.Count - 1] > maxRow)
            {
                for (int row = missedRows[missedRows.Count - 1] + 1; row >= maxRow && !gotlastRowColumn; row--)
                {
                    for (int column = sheet.UsedRange.Column; column <= sheet.UsedRange.LastColumn && !gotlastRowColumn ; column++)
                    {
                        if (sheet.Range[row, column].CellStyle.Color.ToArgb() != -1)
                        {
                            if (sheet.Range[row, column].CellStyle.Color.Name != "ffffffff")
                            {
                                usedRows.Add(row);
                                gotlastRowColumn = true;
                            }
                        }
                        else
                        {
                            IRange range = sheet.Range[row, column];
                            (range.CellStyle as CellStyle).AskAdjacent = false;
                            if ((range.CellStyle as ExtendedFormatWrapper).HasBorder)
                            {
                                usedRows.Add(row);
                                gotlastRowColumn = true;
                            }
                            (range.CellStyle as CellStyle).AskAdjacent = true;
                        }
                    }
                }
            }
            gotlastRowColumn = false;
            int maxColumn = usedColumns.Count > 0? usedColumns[usedColumns.Count-1] : missedColumns[0];
            if (missedColumns.Count != 0 && missedColumns[missedColumns.Count - 1] > maxColumn)
            {
                for (int column = missedColumns[missedColumns.Count - 1]; column >= maxColumn && !gotlastRowColumn; column--)
                {
                    for (int row = sheet.UsedRange.Row; row <= sheet.UsedRange.LastRow &&  !gotlastRowColumn; row++)
                    {
                        if (sheet.Range[row, column].CellStyle.Color.ToArgb() != -1)
                        {
                            if (sheet.Range[row, column].CellStyle.Color.Name != "ffffff")
                            {
                                usedColumns.Add(column);
                                gotlastRowColumn = true;
                            }
                        }
                        else
                        {
                            IRange range = sheet.Range[row, column];
                            (range.CellStyle as CellStyle).AskAdjacent = false;
                            IBorders borders = range.Borders;
                            if ((range.CellStyle as ExtendedFormatWrapper).HasBorder)
                            {
                                for (int i = 5; i <= 10; i++)
                                {
                                    if (borders[(ExcelBordersIndex)i].LineStyle != ExcelLineStyle.None)
                                    {
                                        if (((ExcelBordersIndex)i) != ExcelBordersIndex.EdgeLeft)
                                        {   
                                           usedColumns.Add(column);
                                           gotlastRowColumn = true;
                                        }
                                    }
                                }
                            }
                            (range.CellStyle as CellStyle).AskAdjacent = true;
                        }
                    }
                }
            }

            usedColumns.Sort();
            usedRows.Sort();

            if (usedColumns.Count != 0 || usedRows.Count != 0)
            {
                return sheet.Range[sheet.UsedRange.Row, sheet.UsedRange.Column, usedRows[usedRows.Count - 1],
                                   usedColumns[usedColumns.Count - 1]];
            }
            else
                return sheet.UsedRange;
        }

        /// <summary>
        /// Adds the book mark.
        /// </summary>
        private void AddBookMark()
        {
            PdfBookmark bookMark = this.pdfDocument.Bookmarks.Add(this.workSheet.Name);
            bookMark.Destination = new PdfDestination(this.currentPage, new PointF(0, 0));
        }

        /// <summary>
        /// Checks the range.
        /// </summary>
        /// <param name="tableRange">The table range.</param>
        /// <param name="sheetRange">The sheet range.</param>
        /// <returns>True if the sheet range is within the table range else false will be returned.</returns>
        private bool CheckRange(IRange tableRange, IRange sheetRange)
        {
            return (sheetRange.Row >= tableRange.Row && sheetRange.Column >= tableRange.Column
                    && sheetRange.LastRow <= tableRange.LastRow && sheetRange.LastColumn <= tableRange.LastColumn);
        }

        /// <summary>
        /// Checks the unicode.
        /// </summary>
        /// <param name="unicodeText">The unicode text.</param>
        /// <returns>
        /// True if the text is an unicode else false will returned.
        /// </returns>
        private bool CheckUnicode(string unicodeText)
        {
            char[] unicodeArray = unicodeText.ToCharArray();
            for (int i = 0; i < unicodeArray.Length; i++)
            {
                if ((int)unicodeArray[i] > 255 && Array.IndexOf(numberFormatChar,unicodeArray[i])< 0)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Determines whether [has merged region] [the specified i range].
        /// </summary>
        /// <param name="iRange">The i range.</param>
        /// <returns>
        /// 	<c>true</c> if [has merged region] [the specified i range]; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckMergedRegion(IRange iRange)
        {
            List<MergeCellsRecord.MergedRegion> lstRegions = new List<MergeCellsRecord.MergedRegion>();
            MergeCellsImpl mergedCells = (workSheet as WorksheetImpl).MergeCells;
            mergedCells.CacheMerges(iRange, lstRegions);
            for (int i = 0, len = lstRegions.Count; i < len; i++)
            {
                MergeCellsRecord.MergedRegion region = lstRegions[i];
                IRange mergeRange = workSheet[region.RowFrom+1, region.ColumnFrom+1, region.RowTo+1, region.ColumnTo+1];
                if ((iRange.Row >= mergeRange.Row || iRange.Row <= mergeRange.LastRow)
                    && (iRange.LastRow >= mergeRange.Row || iRange.LastRow <= mergeRange.LastRow)
                    && (iRange.Column >= mergeRange.Column || iRange.Column <= mergeRange.LastColumn)
                    && (iRange.LastColumn >= mergeRange.Column || mergeRange.LastColumn <= mergeRange.LastColumn))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if array of flags contains at least on RTL symbol.
        /// </summary>
        /// <param name="characterCodes">Array of flags.</param>
        /// <returns>True if array of flags contains at least on RTL symbol, False otherwise.</returns>
        private bool CheckIfRTL(ushort[] characterCodes)
        {
            if (characterCodes == null)
            {
                throw new ArgumentNullException("characterCodes");
            }

            bool isRTL = false;

            for (int i = 0, len = characterCodes.Length; i < len; ++i)
            {
                if (characterCodes[i] == (ushort)StringInfoCtype2.C2_RIGHTTOLEFT
                   || characterCodes[i] == (ushort)StringInfoCtype2.C2_ARABICNUMBER)
                {
                    isRTL = true;
                    break;
                }
            }

            return isRTL;
        }

        /// <summary>
        /// Creates the pen.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <returns>The PdfPen object.</returns>
        private PdfPen CreatePen(IBorder border, Color borderColor)
        {
            if (borderColor.IsEmpty)
            {
                borderColor = this.NormalizeColor(border.ColorRGB);
            }

            PdfColor penColor = new PdfColor(borderColor);
            PdfPen pdfPen = new PdfPen(penColor, this.GetBorderWidth(border));
            pdfPen.DashStyle = this.GetDashStyle(border);
            return pdfPen;
        }

        /// <summary>
        /// Degrees to radian.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>Radian value</returns>
        private double DegreeToRadian(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        /// <summary>
        /// Normalizes the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The Normailzed Color</returns>
        private Color NormalizeColor(Color color)
        {
            if (color.A == 0x00)
                return Color.FromArgb(0xFF, color.R, color.G, color.B);

            return color;
        }

        /// <summary>
        /// The function used to identify the required width or height.
        /// </summary>
        /// <param name="excelSheetWidth">Width of the excel sheet.</param>
        /// <param name="sheetHeight">Height of the sheet.</param>
        /// <param name="excelSheetHeight">Height of the excel sheet.</param>
        /// <returns>Returns the required height or width</returns>
        internal float RequiredWidth(float excelSheetWidth, float shWidth, float excelSheetHeight)
        {
            return (excelSheetWidth * shWidth) / excelSheetHeight;
        }
        internal float RequiredHeight(float excelSheetHeight, float shHeight, float excelSheetWidth)
        {
            return (excelSheetWidth * shHeight) / excelSheetHeight;
        }

        /// <summary>
        /// Updates the border delta.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="row">The row of the excel sheet.</param>
        /// <param name="column">The column of the excel sheet.</param>
        /// <param name="deltaX">The delta X.</param>
        /// <param name="deltaY">The delta Y.</param>
        /// <param name="deltaX1">The delta x1.</param>
        /// <param name="deltaY1">The delta y1.</param>
        /// <param name="isInvertCondition">if set to <c>true</c> [is invert condition].</param>
        /// <param name="borders">The borders collection of the cell.</param>
        /// <param name="start">The start Excel border index.</param>
        /// <param name="end">The end Excel border index.</param>
        /// <param name="isLineStart">if set to <c>true</c> [is line start].</param>
        private void UpdateBorderDelta(IWorksheet sheet, int row, int column, int deltaX, int deltaY,
                                        ref int deltaX1, ref int deltaY1, bool isInvertCondition,
                                        IBorders borders, ExcelBordersIndex start, ExcelBordersIndex end,
                                        bool isLineStart)
        {
            int checkRowIndex = row + deltaX;
            int checkColumnIndex = column + deltaY;
            IWorkbook book = sheet.Workbook;
            int maxRowIndex = book.MaxRowCount;
            int maxColumnIndex = book.MaxColumnCount;
            if (checkRowIndex <= 0 || checkRowIndex > maxRowIndex
                || checkColumnIndex <= 0 || checkColumnIndex > maxColumnIndex)
            {
                return;
            }

            IBorders adjacentBorders = sheet[row + deltaX, column + deltaY].Borders;
            bool borderCondition = borders[end].LineStyle == ExcelLineStyle.Double ||
            adjacentBorders[start].LineStyle == ExcelLineStyle.Double;

            if (isInvertCondition)
            {
                borderCondition = !borderCondition;
            }

            int valueToAssign;

            if (end != (ExcelBordersIndex)(-1) && borderCondition)
            {
                valueToAssign = isLineStart ? -1 : 1;
            }
            else
            {
                valueToAssign = isLineStart ? 1 : -1;
            }

            if (deltaY != 0)
            {
                deltaX1 = valueToAssign;
            }
            else if (deltaX != 0)
            {
                deltaY1 = valueToAssign;
            }
        }

        /// <summary>
        /// Updates the rectangle coordinates.
        /// </summary>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="borders">The borders collection of the cell.</param>
        /// <returns>The updated Rectangle coordinates</returns>
        private RectangleF UpdateRectangleCoordinates(RectangleF cellRect, IBorders borders)
        {
            if (borders[ExcelBordersIndex.EdgeLeft].LineStyle != ExcelLineStyle.None)
            {
                cellRect.Offset(-1, 0);
                cellRect.Width++;
            }

            if (borders[ExcelBordersIndex.EdgeTop].LineStyle != ExcelLineStyle.None)
            {
                cellRect.Offset(0, -1);
                cellRect.Height++;
            }

            if (borders[ExcelBordersIndex.EdgeBottom].LineStyle != ExcelLineStyle.None)
            {
                cellRect.Height++;
            }

            if (borders[ExcelBordersIndex.EdgeRight].LineStyle != ExcelLineStyle.None)
            {
                cellRect.Width++;
            }

            return cellRect;
        }
        private IRange FindListObjectRange(IWorksheet sheetImpl, IRange originalRange)
        {
            foreach (IListObject list in sheetImpl.ListObjects)
            {
                if (originalRange.LastRow < list.Location.LastRow)
                {
                    originalRange = sheetImpl.Range[originalRange.Row, originalRange.Column,
                                                    list.Location.LastRow, originalRange.LastColumn];
                }

                if (originalRange.LastColumn < list.Location.LastColumn)
                {
                    originalRange = sheetImpl.Range[originalRange.Row, originalRange.Column,
                                                    originalRange.LastRow, list.Location.LastColumn];
                }

                if ((originalRange.LastColumn < list.Location.LastColumn)
                    && (originalRange.LastRow < list.Location.LastRow))
                {
                    originalRange = sheetImpl.Range[originalRange.Row, originalRange.Column,
                                                    list.Location.LastRow, list.Location.LastColumn];
                }
            }
            return originalRange;
        }
        private IRange UpdatePictureRange(IWorksheet sheetImpl, IRange originalRange)
        {
            int startRow = originalRange.Row > 0 ? originalRange.Row : 1;
            int startColumn = originalRange.Column > 0 ? originalRange.Column : 1;
            int lastRow = originalRange.LastRow > 0 ? originalRange.LastRow : 1;
            int lastColumn = originalRange.LastColumn > 0 ? originalRange.LastColumn : 1;
            foreach (IPictureShape picture in sheetImpl.Pictures)
            {                
                ShapeImpl pictureShape = picture as ShapeImpl;
                if (originalRange.LastRow < pictureShape.BottomRow)
                {
                    originalRange = sheetImpl.Range[startRow, startColumn,
                                                    pictureShape.BottomRow, lastColumn];
                    lastRow = pictureShape.BottomRow;
                }

                if (originalRange.LastColumn < pictureShape.RightColumn)
                {
                    originalRange = sheetImpl.Range[startRow, startColumn,
                                                    lastRow, pictureShape.RightColumn];
                    lastColumn = pictureShape.RightColumn;
                }
            }
            return originalRange;
        }
        /// <summary>
        /// To update the used range for the chart shape
        /// </summary>
        /// <param name="sheetImpl"></param>
        /// <param name="originalRange"></param>
        /// <returns></returns>
        private IRange UpdateChartsRange(IWorksheet sheetImpl, IRange originalRange)
        {
            int startRow = originalRange.Row > 0 ? originalRange.Row : 1;
            int startColumn = originalRange.Column > 0 ? originalRange.Column : 1;
            int lastRow = originalRange.LastRow > 0 ? originalRange.LastRow : 1;
            int lastColumn = originalRange.LastColumn > 0 ? originalRange.LastColumn : 1;
            foreach (IChart chart in sheetImpl.Charts)
            {
                ShapeImpl pictureShape = chart as ShapeImpl;
                if (originalRange.LastRow < pictureShape.BottomRow)
                {
                    originalRange = sheetImpl.Range[startRow, startColumn,
                                                    pictureShape.BottomRow, lastColumn];
                    lastRow = pictureShape.BottomRow;
                }

                if (originalRange.LastColumn < pictureShape.RightColumn)
                {
                    originalRange = sheetImpl.Range[startRow, startColumn,
                                                    lastRow, pictureShape.RightColumn];
                    lastColumn = pictureShape.RightColumn;
                }
            }
            return originalRange;
        }
        /// <summary>
        /// Draw the Pdf HeaderFooter
        /// </summary>
        /// <param name="Sheet">Reoresents the worksheet</param>
        /// <param name="PdfSection">Represent the pdf Section for drawing the HeaderOrFooter</param>
        /// <param name="IsHeader">Used to identify the Header or Footer</param>
        /// <returns>Return the pdfpagetemplete element</returns>
        internal PdfPageTemplateElement AddPDFHeaderFooter(IPageSetupBase PageSetup, PdfSection PdfSection, bool IsHeader, bool isChart)
        {
            HeaderFooter tempHeaderFooter;
            PdfPageTemplateElement pageTemplate;
            float excelHeadFootHeight = Pdf_UnitConverter.ConvertUnits((float)PageSetup.TopMargin, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            
            if (IsHeader)
            {
                tempHeaderFooter = headerFooter[0];
                float maxHight = GetMaxHeight(tempHeaderFooter.HeaderFooterSections);
                excelHeadFootHeight = maxHight > excelHeadFootHeight ? maxHight : excelHeadFootHeight;
                if (excelHeadFootHeight < TopMargin)
                    excelHeadFootHeight = TopMargin;
                if (isChart)
                    pageTemplate = new PdfPageTemplateElement(new RectangleF(0, 0, pdfSection.PageSettings.GetActualSize().Width, excelHeadFootHeight));                    
                else
                    pageTemplate = new PdfPageTemplateElement(new RectangleF(0, 0, pdfDocument.Pages[0].GetClientSize().Width, excelHeadFootHeight));
            }
            else
            {
                if (headerFooter.Count > 1)
                    tempHeaderFooter = headerFooter[1];
                else
                    tempHeaderFooter = headerFooter[0];

                float maxHight = GetMaxHeight(tempHeaderFooter.HeaderFooterSections);
                excelHeadFootHeight = maxHight > 50 ? 50 : maxHight;

                if (excelHeadFootHeight < bottomMargin)
                    excelHeadFootHeight = bottomMargin;
                if(isChart)
                    pageTemplate = new PdfPageTemplateElement(new RectangleF(0, 0, pdfSection.PageSettings.GetActualSize().Width, excelHeadFootHeight));
                else
                    pageTemplate = new PdfPageTemplateElement(new RectangleF(0, 0, pdfDocument.Pages[0].GetClientSize().Width, excelHeadFootHeight));
            }
                
            foreach(HeaderFooterSection section in tempHeaderFooter.HeaderFooterSections)
            {
                if(section.HeaderFooterCollections!=null)
                AddHeaderFooterSection(pageTemplate, section);
            }
            headerFooter.RemoveAt(0);
            return pageTemplate;
        }
        /// <summary>
        /// Draw the Pdf Header Footer string
        /// </summary>
        /// <param name="pageTemplate">PdfPagetemplate-HeaderOrFooter</param>
        /// <param name="Section">HeaderOrFooter Section</param>
        internal void AddHeaderFooterSection(PdfPageTemplateElement pageTemplate,HeaderFooterSection Section)
        {
            string sectionValue=string.Empty;
            string sectionStr = string.Empty;
            
            List<PdfAutomaticField> list = new List<PdfAutomaticField>();
            PdfAutomaticField field;

            Font font = new Font("Times New Roman", 11);
            Color color = new Color();
            
            PdfStringFormat format = new PdfStringFormat();
            PdfPageCountField tolalCount = new PdfPageCountField();
            PdfPageNumberField pageNo = new PdfPageNumberField();
            format.Alignment = Section.TextAlignment;
            int automaticCount = 0;
            foreach (KeyValuePair<string, HeaderFooterFontColorSettings> headerFooterCollections in Section.HeaderFooterCollections)
            {
                sectionValue = headerFooterCollections.Key.Remove(headerFooterCollections.Key.IndexOf('|'),
                                 headerFooterCollections.Key.Length - headerFooterCollections.Key.IndexOf('|'));
                if (headerFooterCollections.Value != null && string.IsNullOrEmpty(sectionStr))
                {
                    color = headerFooterCollections.Value.FontColor;
                    font = headerFooterCollections.Value.Font;
                }
                if (sectionValue == "&P")
                {
                    field = pageNo;
                    list.Add(field);
                    sectionStr += "{" + automaticCount.ToString() + "}";
                    automaticCount++;
                    //autoMaticFiled.SetValue(pageNo, automaticCount);
                }
                else if (sectionValue == "&N")
                {
                    field = tolalCount;
                    list.Add(field);
                    sectionStr += "{" + automaticCount.ToString() + "}";
                    automaticCount++;
                    //autoMaticFiled.SetValue(tolalCount, automaticCount);
                }
                else 
                    sectionStr += sectionValue;                
            }
            if (Encoding.UTF8.GetByteCount(sectionStr.ToString()) != (sectionStr.ToString().Length))
            {
                this.excelToPdfSettings.EmbedFonts = true;
            }
            PdfSolidBrush brush = new PdfSolidBrush(color);
            PdfCompositeField compositeField;
            PdfAutomaticField[] autoMaticFiled = new PdfAutomaticField[list.Count];
            list.CopyTo(autoMaticFiled);
            PdfTrueTypeFont pdfFont = new PdfTrueTypeFont(font,font.Size);
            
            if (list.Count>0)
                compositeField = new PdfCompositeField(pdfFont, brush, sectionStr, autoMaticFiled);
            else
                compositeField = new PdfCompositeField(pdfFont, brush, sectionStr);
            
            compositeField.Bounds = pageTemplate.Bounds;
            compositeField.StringFormat = format;
            compositeField.StringFormat.LineAlignment = PdfVerticalAlignment.Middle;            
            compositeField.Draw(pageTemplate.Graphics);            
        }
        private bool Drawpicture(int firstrow,int lastRow,int firstColumn,int lastColumn,ShapeImpl shape,out bool inBetween)
        {            
            bool isInRow = (firstrow <= shape.TopRow && lastRow >= shape.TopRow) || (firstrow <= shape.BottomRow && lastRow >= shape.BottomRow);
            bool inBetweenRow = (firstrow > shape.TopRow && shape.BottomRow > lastRow);
            bool isInColumn = (firstColumn <= shape.LeftColumn && lastColumn >= shape.LeftColumn) || (firstColumn <= shape.RightColumn && lastColumn >= shape.RightColumn); ;
            bool inBetweenColumn = (firstColumn > shape.LeftColumn && shape.RightColumn > lastColumn);
            inBetween = (inBetweenColumn && isInRow ) || (inBetweenRow && isInColumn) || (inBetweenColumn && inBetweenRow);
            if (shape.TopRow == 0 || shape.LeftColumn == 0 || (inBetweenRow && isInColumn) || (inBetweenColumn && isInRow) || (inBetweenColumn&& inBetweenRow))
                return true;
            else if(shape.TopRow != 0 && shape.LeftColumn != 0 && (isInColumn && isInRow))
               return true;
            else
            return false;
        }
        internal bool HasPicture(RangeImpl range,IWorksheet sheetImpl)
        {
            int firstRow=range.Row;
            int lastRow=range.LastRow;
            int firstColumn=range.Column;
            int lastColumn = range.LastColumn;
            bool hasPic=false;
            bool isBetween;
            for (int count=0;!hasPic && count < sheetImpl.Pictures.Count;count++)
            {
                ShapeImpl pictureShape = sheetImpl.Pictures[count] as ShapeImpl;
                hasPic = Drawpicture(firstRow, lastRow, firstColumn, lastColumn, pictureShape,out isBetween);
            }
            if(!hasPic)
                for (int count = 0; !hasPic && count < sheetImpl.Charts.Count; count++)
                {
                    ShapeImpl pictureShape = sheetImpl.Charts[count] as ShapeImpl;
                    hasPic = Drawpicture(firstRow, lastRow, firstColumn, lastColumn, pictureShape, out isBetween);
                }
            return hasPic;
        }
#if SyncfusionFramework4_0 || SyncfusionFramework4_5
       internal List<string> GetDrawString(string cellText, RangeRichTextString RTF,out List<IFont> richTextFonts,IFont excelFont)
        {
            List<IFont> pdfFonts = new List<IFont>();
            IList<int> fontIndex = RTF.TextObject.FormattingRuns.Values;
            int length = RTF.TextObject.FormattingRunsCount;            
            IList<int> key = RTF.TextObject.FormattingRuns.Keys;
            List<string> drawString = new List<string>();
            IFont font;
            int start = 0;
            if (key[0] != 0)
            {
                start = 1;
                drawString.Add(cellText.Substring(0, key[0]));
                pdfFonts.Add(excelFont);
            }
            for (int i = 0; i < length - 1; i++)
            {
                string rtText = cellText.Substring(key[i], key[i + 1] - key[i]);
                drawString.Add(rtText);
                start++;
                font = (workBook as WorkbookImpl).InnerFonts[fontIndex[i]];
                pdfFonts.Add(font);
            }
            drawString.Add(cellText.Substring(key[length - 1], cellText.Length - key[length -1]));
            font = (workBook as WorkbookImpl).InnerFonts[fontIndex[length-1]];
            pdfFonts.Add(font);
            richTextFonts = pdfFonts;
            return drawString;
        }
        internal List<string> UpdateRTFValues(List<string> Drawstring, List<IFont> RTFFont,out List<IFont> updatedRTFFont, PdfStringLayoutResult result)
        {
            List<string> drawString = Drawstring;
            updatedRTFFont = RTFFont;
            int maxLenght = 0;
            int count=0;
            int lineLength = result.Lines[count].Text.Length;
            for (int i = 0; i < Drawstring.Count; i++)
            {
                drawString[i] = drawString[i].Replace("\n", string.Empty);
                maxLenght = maxLenght + drawString[i].Length;
                if (maxLenght >= lineLength)
                {
                    int startPoint = 0;
                    int len = maxLenght - drawString[i].Length;
                    //for (int lent = len; len < lineLength; len++)

                    string original = drawString[i];
                    string sub = original.Substring(startPoint, lineLength - len);
                    if (sub != string.Empty)
                    {
                        string nextStr = original.Substring(startPoint + lineLength - len, original.Length - (startPoint + lineLength - len));
                        if (!String.IsNullOrWhiteSpace(nextStr))
                        {
                            while (nextStr[0] == ' ' && !String.IsNullOrWhiteSpace(nextStr))
                                nextStr = nextStr.Substring(1);                            
                            drawString.Insert(i + 1, nextStr);
                            RTFFont.Insert(i + 1, RTFFont[i]);
                        }
                        drawString[i] = sub;
                    }
                    if(result.LineCount > count + 1)
                    count++;
                    lineLength = result.Lines[count].Text.Length;
                    maxLenght = 0;
                }
            }
            updatedRTFFont = RTFFont;
            return drawString;
        }
#endif
        #endregion
    }
}

