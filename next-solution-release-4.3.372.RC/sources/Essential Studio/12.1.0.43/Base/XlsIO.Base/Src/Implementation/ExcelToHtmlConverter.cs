#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation
{
    /// <summary>
    ///  Helper class to perform excel to html conversion.
    /// </summary>
    class ExcelToHtmlConverter : IDisposable
    {

        #region Constants
        private const string Pt = "pt";
        private const string Px = "px";
        private const string Em = "em";
        private const string TabPart = "tabpart";
        private const string HyperlinkStyleName = "Hyperlink";
        #endregion

        #region Enums
        /// <summary>
        /// Represents the conversion mode.
        /// </summary>
        private enum ConversionMode
        {
            Workbook,
            Worksheet
        }
        #endregion

        #region Fields
        private XmlTextWriter m_writer;
        private Dictionary<string, LinkedList<string>> m_styles;
        private StringBuilder builder = new StringBuilder();
        private List<MergeCellsRecord.MergedRegion> lstRegions;
        private string m_keyNoPosition;
        private ConversionMode m_conversionMode;
        private Dictionary<string, Dictionary<string, LinkedList<string>>> m_worksheetStyleCollections;
        int increment = 0;
        private ItemSizeHelper m_columnWidthGetter = null;
        private IWorksheet workSheet;

        /// <summary>
        /// Represents the object of Excel sheet conditional formatting .
        /// </summary>
        private CFApplier conditionalFormatApplier;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        private XmlTextWriter Writer
        {
            get
            {
                return m_writer;
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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToHtmlConverter"/> class.
        /// </summary>
        public ExcelToHtmlConverter()
        {

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="book">The book.</param>
        /// <param name="outputDirectoryPath">The output directory path.</param> 
        /// <param name="saveOption">The saveoption.</param> 
        public void ConvertToHtml(Stream stream, WorkbookImpl book, string outputDirectoryPath, HtmlSaveOptions saveOption)
        {
            m_conversionMode = ConversionMode.Workbook;
            m_writer = new XmlTextWriter(stream, Encoding.UTF8);
            m_writer.Formatting = Formatting.Indented;
            this.conditionalFormatApplier = new CFApplier();
            m_styles = new Dictionary<string, LinkedList<string>>();
            lstRegions = new List<MergeCellsRecord.MergedRegion>();
            WriteDocumentStart();
            BuildMainPage(book, outputDirectoryPath, saveOption);
            BuildTabPage(book, outputDirectoryPath);
            BuildHtmlFiles(book, outputDirectoryPath, saveOption);
        }
        /// <summary>
        /// Converts to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="book">The book.</param>
        /// <param name="saveOption">The saveoption.</param>
        internal void ConvertToHtml(Stream stream, WorkbookImpl book, HtmlSaveOptions saveOption)
        {
            m_conversionMode = ConversionMode.Workbook;
            m_writer = new XmlTextWriter(stream, Encoding.UTF8);
            this.conditionalFormatApplier = new CFApplier();
            m_styles = new Dictionary<string, LinkedList<string>>();
            lstRegions = new List<MergeCellsRecord.MergedRegion>();
            WriteDocumentStart();
            BuildStyles(book, saveOption, m_writer);
            Writer.WriteStartElement(HtmlTags.Body);
            Writer.WriteAttributeString(HtmlAttributes.Style, "overflow: hidden");
            Writer.WriteStartElement(HtmlTags.Div);
          
            foreach (WorksheetImpl sheet in book.Worksheets)
            {
                this.workSheet = sheet;
                Writer.WriteStartElement(HtmlTags.Div);
                Writer.WriteAttributeString(HtmlAttributes.Id, string.Format("link{0}_content", sheet.Index + 1));
                if (sheet == book.ActiveSheet)
                {
                    Writer.WriteAttributeString(HtmlAttributes.Style, "display:block ; height:500px; width=100%; overflow:scroll;");
                }
                else
                {
                    Writer.WriteAttributeString(HtmlAttributes.Style, "display:none; height:500px; width=100%; overflow:scroll;");
                }
                WriteSheetContent(sheet, saveOption, Writer, m_worksheetStyleCollections[sheet.Name]);
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
            BuildTabPage(book);
            BuildScripts(book);
            Writer.WriteEndElement();
            WriteDocumentEnd();
        }
        /// <summary>
        /// Converts the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="outputDirectoryPath">The output directory path.</param> 
        /// <param name="saveOption">The saveoption.</param> 
        public void ConvertToHtml(Stream stream, WorksheetImpl sheet, string outputDirectoryPath, HtmlSaveOptions saveOption)
        {
            this.workSheet = sheet;
            m_conversionMode = ConversionMode.Worksheet;
            m_writer = new XmlTextWriter(stream, Encoding.UTF8);
            m_writer.Formatting = Formatting.Indented;
            this.conditionalFormatApplier = new CFApplier();
            m_styles = new Dictionary<string, LinkedList<string>>();
            lstRegions = new List<MergeCellsRecord.MergedRegion>();
            WriteDocumentStart();
            BuildStyles(sheet, saveOption, m_writer);
            Writer.WriteStartElement(HtmlTags.Body);
            WriteSheetContent(sheet, outputDirectoryPath, saveOption, m_writer);
            Writer.WriteEndElement();
            WriteDocumentEnd();
        }

        /// <summary>
        /// Builds the main page.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="saveOption">The saveoption.</param>
        private void BuildMainPage(WorkbookImpl book, string outputDirectoryPath, HtmlSaveOptions saveOption)
        {
            string directory = new DirectoryInfo(outputDirectoryPath).Name;
            Writer.WriteStartElement(HtmlTags.FrameSet);
            Writer.WriteAttributeString(HtmlAttributes.Rows, "93%,7%");
            Writer.WriteAttributeString(HtmlAttributes.FrameBorder, "0");
            Writer.WriteStartElement(HtmlTags.Frame);
            string fileName = string.Format("{0}.html", book.Worksheets[0].Name);
            Writer.WriteAttributeString(HtmlAttributes.Source, Path.Combine(directory, fileName));
            Writer.WriteAttributeString(HtmlAttributes.Name, "top");
            Writer.WriteAttributeString(HtmlAttributes.Scrolling, "auto");
            Writer.WriteEndElement();
            Writer.WriteStartElement(HtmlTags.Frame);

            Writer.WriteAttributeString(HtmlAttributes.Source, Path.Combine(directory, "tabs.html"));
            Writer.WriteAttributeString(HtmlAttributes.Name, "bottom");
            Writer.WriteAttributeString(HtmlAttributes.Scrolling, "auto");
            Writer.WriteEndElement();
            Writer.WriteStartElement(HtmlTags.NoFrames);
            Writer.WriteStartElement(HtmlTags.Body);
            Writer.WriteString("This text will appear only if the browser does not support frames.");
            WriteDocumentEnd();
            WriteDocumentEnd();
            WriteDocumentEnd();
            WriteDocumentEnd();
        }
        /// <summary>
        /// Builds the scripts.
        /// </summary>
        /// <param name="book">The book.</param>
        private void BuildScripts(WorkbookImpl book)
        {

            Writer.WriteStartElement(HtmlTags.Script);
            Writer.WriteAttributeString(HtmlAttributes.Type, "text/javascript");

            Writer.WriteRaw(GetScript(book));
            Writer.WriteEndElement();
        }
        /// <summary>
        /// Builds the tab page.
        /// </summary>
        /// <param name="book">The book.</param>
        private void BuildTabPage(WorkbookImpl book)
        {
            Writer.WriteStartElement(HtmlTags.Div);
            Writer.WriteAttributeString(HtmlAttributes.Id, "footer");
            Writer.WriteAttributeString(HtmlAttributes.Style, "position: absolute; bottom: 0px; width:100%; background-color:#808080; background-repeat:repeat-x ; height:35px ");
            Writer.WriteStartElement(HtmlTags.Table);
            Writer.WriteAttributeString(HtmlAttributes.Border, "0");
            Writer.WriteAttributeString(HtmlAttributes.BgColor, "#808080");
            Writer.WriteStartElement(HtmlTags.Tr);
            foreach (WorksheetImpl sheet in book.Worksheets)
            {
                Writer.WriteStartElement(HtmlTags.Td);
                Writer.WriteAttributeString(HtmlAttributes.BgColor, "#FFFFFF");
                Writer.WriteAttributeString(HtmlAttributes.NoWarp, string.Empty);
                Writer.WriteStartElement(HtmlTags.Bold);
                Writer.WriteStartElement(HtmlTags.Small);
                Writer.WriteStartElement(HtmlTags.Small);
                Writer.WriteRaw("&nbsp;");
                Writer.WriteStartElement(HtmlTags.A);
                Writer.WriteAttributeString(HtmlAttributes.Id, string.Format("link{0}", sheet.Index + 1));
                Writer.WriteAttributeString(HtmlAttributes.OnClick, "hyperclick(this)");
                Writer.WriteAttributeString(HtmlAttributes.Href, "#");
                Writer.WriteAttributeString(HtmlAttributes.Style, "text-decoration:none");

                Writer.WriteStartElement(HtmlTags.Font);
                Writer.WriteAttributeString(HtmlAttributes.Face, "Verdana");
                Writer.WriteAttributeString(HtmlAttributes.Color, "#000000");
                Writer.WriteAttributeString(HtmlAttributes.FontSize, "13Px");

                Writer.WriteString(sheet.Name);

                Writer.WriteEndElement();
                Writer.WriteEndElement();
                Writer.WriteRaw("&nbsp;");
                Writer.WriteEndElement();
                Writer.WriteEndElement();
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
            Writer.WriteEndElement();
            Writer.WriteEndElement();
        }

        /// <summary>
        /// Builds the tab page.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="outputDirectoryPath">The output directory path.</param>
        private void BuildTabPage(WorkbookImpl book, string outputDirectoryPath)
        {
            string directory = new DirectoryInfo(outputDirectoryPath).Name;
            int count = book.Worksheets.Count;
            string tabFile = Path.Combine(outputDirectoryPath, "tabs.html");
            FileStream stream = new FileStream(tabFile, FileMode.OpenOrCreate);
            m_writer = new XmlTextWriter(stream, Encoding.UTF8);
            m_writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement(HtmlTags.Html);
            Writer.WriteStartElement(HtmlTags.Style);
            Writer.WriteAttributeString(HtmlAttributes.Type, "text/css");
            Writer.WriteString(HtmlTags.Table);
            Writer.WriteString(Operators.OpenCurlyBrace);
            Writer.WriteString(HtmlAttributes.BorderCollapse);
            Writer.WriteString(Operators.Colon);
            Writer.WriteString("collapse");
            Writer.WriteString(Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.BorderSpacing);
            Writer.WriteString(Operators.Colon);
            Writer.WriteString("0");
            Writer.WriteString(Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.EmptyCells);
            Writer.WriteString(Operators.Colon);
            Writer.WriteString("show");
            Writer.WriteString(Operators.SemiColon);
            Writer.WriteString(Operators.CloseCurlyBrace);
            Writer.WriteString(HtmlTags.A);
            Writer.WriteString(Operators.OpenCurlyBrace);
            Writer.WriteString(HtmlAttributes.TextDecoration + Operators.Colon + HtmlAttributes.None + Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.FontFamily + Operators.Colon + "Verdana" + Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.FontSize + Operators.Colon + "13" + Px + Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.FontWeight + Operators.Colon + HtmlAttributes.Bold + Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.BackgroundColor + Operators.Colon + "rgb(255,255,255)" + Operators.SemiColon);
            Writer.WriteString(Operators.CloseCurlyBrace);
            Writer.WriteString(HtmlAttributes.Ahover);
            Writer.WriteString(Operators.OpenCurlyBrace);
            Writer.WriteString(HtmlAttributes.TextDecoration + Operators.Colon + HtmlAttributes.None + Operators.SemiColon);
            Writer.WriteString(Operators.CloseCurlyBrace);
            Writer.WriteString(".X1");
            Writer.WriteString(Operators.OpenCurlyBrace);
            Writer.WriteString(HtmlAttributes.Position);
            Writer.WriteString(Operators.Colon);
            Writer.WriteString(HtmlAttributes.Absolute);
            Writer.WriteString(Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.Left);
            Writer.WriteString(Operators.Colon);
            Writer.WriteString("0" + Em);
            Writer.WriteString(Operators.SemiColon);
            Writer.WriteString(HtmlAttributes.Top);
            Writer.WriteString(Operators.Colon);
            Writer.WriteString("0" + Px);
            Writer.WriteString(Operators.SemiColon);
            Writer.WriteString(Operators.CloseCurlyBrace);
            WriteDocumentEnd();
            Writer.WriteStartElement(HtmlTags.Body);
            Writer.WriteAttributeString(HtmlAttributes.Alink, "rgb(0,0,255)");
            Writer.WriteAttributeString(HtmlAttributes.Vlink, "rgb(0,0,0)");
            Writer.WriteAttributeString(HtmlAttributes.Link, "rgb(0,0,0)");
            Writer.WriteAttributeString(HtmlAttributes.BgColor, "#808080");
            Writer.WriteStartElement(HtmlTags.Table);
            Writer.WriteAttributeString(HtmlAttributes.Border, "0");
            Writer.WriteStartElement(HtmlTags.Tr);
            Writer.WriteAttributeString(HtmlAttributes.Class, "X1");

            for (int i = 0; i < count; i++)
            {
                Writer.WriteStartElement(HtmlTags.Td);
                string fileName = string.Format("{0}.html", book.Worksheets[i].Name);
                Writer.WriteStartElement(HtmlTags.A);
                Writer.WriteAttributeString(HtmlAttributes.Href, fileName);
                Writer.WriteAttributeString(HtmlAttributes.Target, HtmlAttributes.Top);
                Writer.WriteString(book.Worksheets[i].Name);
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }

            WriteDocumentEnd();
            WriteDocumentEnd();
            WriteDocumentEnd();
            WriteDocumentEnd();
            Writer.Close();
        }

        /// <summary>
        /// Builds the styles.
        /// </summary>
        /// <param name="sheet">The sheet.</param>  
        private void BuildStyles(WorksheetImpl sheet, HtmlSaveOptions saveOption, XmlWriter writer)
        {
            Writer.WriteStartElement(HtmlTags.Style);
            Writer.WriteAttributeString(HtmlAttributes.Type, "text/css");
            string style = GetStyles(sheet, saveOption);
            Writer.WriteString(style);
            WriteDocumentEnd();
        }
        /// <summary>
        /// Builds the styles.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="saveOption">The save option.</param>
        /// <param name="m_writer">The m_writer.</param>
        private void BuildStyles(WorkbookImpl book, HtmlSaveOptions saveOption, XmlTextWriter m_writer)
        {
            m_worksheetStyleCollections = new Dictionary<string, Dictionary<string, LinkedList<string>>>();
            StringBuilder styleBuilder = new StringBuilder();
            Writer.WriteStartElement(HtmlTags.Style);
            Writer.WriteAttributeString(HtmlAttributes.Id, TabPart);
            Writer.WriteAttributeString(HtmlAttributes.Type, "text/css");            
            foreach (WorksheetImpl sheet in book.Worksheets)
            {
                styleBuilder.Append(GetStreamStyles(sheet, saveOption));
            }
            Writer.WriteString(styleBuilder.ToString());
            Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the document start.
        /// </summary>
        private void WriteDocumentStart()
        {
            Writer.WriteStartElement(HtmlTags.Html);
            Writer.WriteStartElement(HtmlTags.Head);
            WriteDocumentEnd();
        }

        /// <summary>
        /// Writes the document end.
        /// </summary>
        private void WriteDocumentEnd()
        {
            Writer.WriteEndElement();
            Writer.Flush();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="saveOption">The save option.</param> 
        /// <returns></returns>
        private string GetStreamStyles(WorksheetImpl sheet, HtmlSaveOptions saveOption)
        {
            Dictionary<string, LinkedList<string>> styles = new Dictionary<string, LinkedList<string>>();

            ItemSizeHelper rowHeightGetter = new ItemSizeHelper(sheet.GetRowHeightInPixels);
            ItemSizeHelper columnWidthGetter = new ItemSizeHelper(sheet.GetColumnWidthInPixels);
            StringBuilder builder = new StringBuilder();
            IRange range = sheet.UsedRange;
            int rowStart = range.Row;
            int columnStart = range.Column;
            int rowEnd = range.LastRow;
            int columnEnd = range.LastColumn;
            MigrantRangeImpl result = new MigrantRangeImpl(sheet.Application, sheet);
            int count = 0;
            int stylesCount = 0;
            string key = null;
            int rowFrom = 0;
            int columnFrom = 0;
            int rowTo = 0;
            int columnTo = 0;
            int k = 0;
            if (sheet.HasMergedCells)
            {
                MergeCellsImpl mergedCells = sheet.MergeCells;
                mergedCells.CacheMerges(sheet[rowStart, columnStart, rowEnd, columnEnd], lstRegions);
            }

            for (int i = 1; i <= rowEnd; i++)
            {
                for (int j = 1; j <= columnEnd; j++)
                {
                    count++;
                    int colWidth = sheet.GetColumnWidthInPixels(j);
                  
                    
                        
                        string row = i.ToString();
                        String cellPos = row + GetColumnName(j);
                        result.ResetRowColumn(i, j);                  
                        string value = null;
                        if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                            value = result.DisplayText;
                        else
                            value = result.Value;

                        ExtendedFormatImpl xf = (result.CellStyle as CellStyle).Wrapped;
                        xf = this.conditionalFormatApplier.ApplyCF(result, xf);

                        int width = sheet.GetColumnWidthInPixels(j);
                        int height = sheet.GetRowHeightInPixels(i);
                        int rotaionAngle = result.CellStyle.Rotation;
                        string fontName = result.CellStyle.Font.FontName.ToString();
                        Color color = NormalizeColor(result.CellStyle.Font.RGBColor);
                        int red = color.R;
                        int green = color.G;
                        int blue = color.B;
                        string fontColor = HtmlAttributes.Rgb + Operators.OpenBrace + red + Operators.Comma + green + Operators.Comma + blue + Operators.CloseBrace;
                        double fontSize = result.CellStyle.Font.Size;
                        string cellColor = xf.Color.Name;
                        string backColor = xf.ColorIndex.ToString();
                        int backgroundRed = xf.Color.R;
                        int backgroundGreen = xf.Color.G;
                        int backgroundBlue = xf.Color.B;
                        string bgColor = HtmlAttributes.Rgb + Operators.OpenBrace + backgroundRed + Operators.Comma + backgroundGreen + Operators.Comma + backgroundBlue + Operators.CloseBrace;

                        string underline = result.CellStyle.Font.Underline.ToString(CultureInfo.InstalledUICulture.NumberFormat);
                        m_keyNoPosition = Operators.OpenCurlyBrace + HtmlAttributes.FontColor + Operators.Colon + fontColor + Operators.SemiColon + HtmlAttributes.FontFamily + Operators.Colon + fontName + Operators.SemiColon + HtmlAttributes.FontSize + Operators.Colon + fontSize + Pt + Operators.SemiColon + HtmlAttributes.BackgroundColor + Operators.Colon + bgColor + Operators.SemiColon + HtmlAttributes.Height + Operators.Colon + height + Operators.SemiColon;

                        Dictionary<string, string> borderDictionary = new Dictionary<string, string>();
                        Dictionary<string, string> currentBorderDictionary = new Dictionary<string, string>();


                        if (k < lstRegions.Count)
                        {
                            rowFrom = lstRegions[k].RowFrom;
                            columnFrom = lstRegions[k].ColumnFrom;
                            rowTo = lstRegions[k].RowTo;
                            columnTo = lstRegions[k].ColumnTo;
                            String cellPosition = (rowTo + 1).ToString() + GetColumnName(columnTo + 1);

                            IRange newResult = sheet.Range[cellPosition];
                            if (i == rowFrom + 1 && j == columnFrom + 1)
                            {
                                borderDictionary = BuildBorders(result);
                                currentBorderDictionary = BuildBorders(newResult);

                                foreach (string borderKey in currentBorderDictionary.Keys)
                                {
                                    if (!borderDictionary.ContainsKey(borderKey))
                                    {
                                        m_keyNoPosition += borderKey + Operators.Colon + currentBorderDictionary[borderKey] + Operators.SemiColon;
                                    }
                                }

                                k++;
                            }

                        }
                    

                        m_keyNoPosition = GetBorderStyles(result, m_keyNoPosition);
                        key = Operators.Dot + cellPos + m_keyNoPosition;

                        if (result.CellStyle.Font.Bold)
                    {
                            key += HtmlAttributes.FontWeight + Operators.Colon + HtmlAttributes.Bold + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.FontWeight + Operators.Colon + HtmlAttributes.Bold + Operators.SemiColon;
                    }

                        if (result.CellStyle.Font.Italic)
                    {
                            key += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.Italic + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.Italic + Operators.SemiColon;
                    }
                        if (result.CellStyle.Font.Strikethrough)
                        {
                            key += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.StrikeThrough + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.StrikeThrough + Operators.SemiColon;
                        }

                        if (!underline.Equals("None"))
                    {
                            key += HtmlAttributes.TextDecoration + Operators.Colon + HtmlAttributes.Underline + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.TextDecoration + Operators.Colon + HtmlAttributes.Underline + Operators.SemiColon;
                    }

                        if (result.VerticalAlignment == ExcelVAlign.VAlignBottom)
                    {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Bottom + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Bottom + Operators.SemiColon;
                        }
                        if (result.VerticalAlignment == ExcelVAlign.VAlignTop)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Top + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Top + Operators.SemiColon;
                        }
                        if (result.VerticalAlignment == ExcelVAlign.VAlignCenter)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Center + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Center + Operators.SemiColon;
                        }
                        if (result.VerticalAlignment == ExcelVAlign.VAlignDistributed)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Distriburted + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Distriburted + Operators.SemiColon;
                        }
                        if (result.VerticalAlignment == ExcelVAlign.VAlignJustify)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Justify + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Justify + Operators.SemiColon;
                        }

                        if (result.HorizontalAlignment == ExcelHAlign.HAlignCenter)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Center;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Center;

                    }
                        if (result.HorizontalAlignment == ExcelHAlign.HAlignLeft)
                    {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Left;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Left;

                        }
                        int extendedFormatIndex = result.ExtendedFormatIndex;
                        ExtendedFormatImpl extendedFormatImpl = result.Workbook.InnerExtFormats[extendedFormatIndex];
                        if (result.HasNumber || result.HasDateTime ||
                            result.HasFormula &&
                            result.FormulaStringValue == null &&
                            !result.HasFormulaErrorValue &&
                            !result.HasFormulaBoolValue)
                        {
                            WorkbookImpl book = result.Worksheet.Workbook as WorkbookImpl;
                            FormatImpl numberFormat = book.InnerFormats[extendedFormatImpl.NumberFormatIndex];
                            ExcelFormatType formatType = numberFormat.GetFormatType(result.Number);

                            result.HorizontalAlignment = (formatType == ExcelFormatType.Text) ?
                                ExcelHAlign.HAlignLeft : ExcelHAlign.HAlignRight;
                        }
                        if (result.HorizontalAlignment == ExcelHAlign.HAlignRight)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Right;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Right;
                        }
                        if (result.HorizontalAlignment == ExcelHAlign.HAlignJustify)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Justify;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Justify;
                        }
                        if (result.HorizontalAlignment == ExcelHAlign.HAlignDistributed)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Distriburted;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Distriburted;
                        }
                        if (result.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
                        {
                            string general = HtmlAttributes.General;
                            if (result.HasNumber)
                                general = HtmlAttributes.Right;
                            key += HtmlAttributes.TextAlignment + Operators.Colon + general;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + general;
                        }

                        key += Operators.CloseCurlyBrace;
                        m_keyNoPosition += Operators.CloseCurlyBrace;
                        string className = String.Concat(".X", increment);

                        if (!styles.ContainsKey(m_keyNoPosition))
                        {
                            LinkedList<string> cellPositions = new LinkedList<string>();
                            cellPositions.AddFirst(className);
                            increment++;
                            cellPositions.AddLast(cellPos);
                            styles.Add(m_keyNoPosition, cellPositions);
                            stylesCount++;
                        }
                        else
                        {
                            LinkedList<string> cellPositions = (LinkedList<string>)styles[m_keyNoPosition];
                            cellPositions.AddLast(cellPos);
                        }

                    }
                }

            string appendText = "." + HtmlTags.Table + sheet.Index.ToString() + Operators.OpenCurlyBrace + HtmlAttributes.BorderCollapse + Operators.Colon + "collapse" + Operators.SemiColon + HtmlAttributes.BorderSpacing + Operators.Colon + "0" + Operators.SemiColon + HtmlAttributes.EmptyCells + Operators.Colon + "show" + Operators.CloseCurlyBrace;
            builder.Append(appendText);
            foreach (string dictionaryKey in styles.Keys)
            {
                LinkedList<string> cellPositions = styles[dictionaryKey];
                builder.AppendLine();
                builder.Append(cellPositions.First.Value);
                builder.Append(dictionaryKey);
            }

            if (sheet.HasPictures && saveOption.ImagePath != null)
                GetImageStyles(sheet, builder);

            m_worksheetStyleCollections.Add(sheet.Name, styles);
            return builder.ToString();
        }
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        private string GetScript(WorkbookImpl book)
        {
            string script = " function hyperclick(test) {"
             + "var content = test.id;"
            + " content = content + \"_content\";"
             + "var count =" + book.Worksheets.Count + ";"
             + "for (var i = 1; i <= count; i++) {"

                 + "var formatName = \"link\" + i + \"_content\";"

                 + "if (content == formatName) {"
                     + "document.getElementById(formatName).style.display = \"block\";"
                     + "document.getElementById(formatName).style.height = window.innerHeight-40+\"px\";"
                 + "}"
                 + "else {"
                     + "document.getElementById(formatName).style.display = \"none\";"
                     + "document.getElementById(formatName).style.height = window.innerHeight-40+\"px\";"
                 + "}"
             + "}"
             + "}";
            return script;
        }
     
        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="saveOption">The save option.</param> 
        /// <returns></returns>
        private string GetStyles(WorksheetImpl sheet, HtmlSaveOptions saveOption)
        {
            StringBuilder builder = new StringBuilder();
            IRange range = sheet.UsedRange;
            int rowStart = range.Row;
            int columnStart = range.Column;
            int rowEnd = range.LastRow;
            int columnEnd = range.LastColumn;
            int count = 0;
            int stylesCount = 0;
            string key = null;
            int rowFrom = 0;
            int columnFrom = 0;
            int rowTo = 0;
            int columnTo = 0;
            int k = 0;
            if (sheet.HasMergedCells)
            {
                MergeCellsImpl mergedCells = sheet.MergeCells;
                mergedCells.CacheMerges(sheet[rowStart, columnStart, rowEnd, columnEnd], lstRegions);
            }

            MigrantRangeImpl cell = new MigrantRangeImpl(sheet.Application, sheet);
            string cellColor = string.Empty;
            string backColor = string.Empty;

            int backgroundRed = 0;
            int backgroundGreen = 0;
            int backgroundBlue = 0;
            ExtendedFormatImpl xf=null;
          
            for (int i = 1; i <= rowEnd; i++)
            {
                int rowHeight = sheet.GetRowHeightInPixels(i);
                if (rowHeight > 0)
                {
                    for (int j = 1; j <= columnEnd; j++)
                    {
                        int colWidth = sheet.GetColumnWidthInPixels(j);
                        string row = i.ToString();
                        String cellPos = row + GetColumnName(j);
                        cell.ResetRowColumn(i, j);
                        if (colWidth > 0)
                        {
                           
                            string value = null;
                            if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                                value = cell.DisplayText;
                            else
                                value = cell.Value;


                            IConditionalFormats formats = cell.ConditionalFormats;
                            if (formats.Count > 0)
                            {
                                xf = (cell.CellStyle as CellStyle).Wrapped;
                                xf = this.conditionalFormatApplier.ApplyCF(cell, xf);
                                foreach (IConditionalFormat format in formats)
                                {
                                    if (format.BackColorRGB != Color.White)
                                    {
                                        cellColor = xf.Color.ToString();
                                        backColor = xf.ColorIndex.ToString();
                                        backgroundRed =  format.BackColorRGB.R;
                                        backgroundBlue = format.BackColorRGB.B;
                                        backgroundGreen = format.BackColorRGB.G;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                cellColor = cell.CellStyle.Color.Name;
                                backColor = cell.CellStyle.ColorIndex.ToString();

                                backgroundRed = cell.CellStyle.Color.R;
                                backgroundGreen = cell.CellStyle.Color.G;
                                backgroundBlue = cell.CellStyle.Color.B;
                            }                     
                            int height = sheet.GetRowHeightInPixels(i);
                            int rotaionAngle = cell.CellStyle.Rotation;
                            string fontName = cell.CellStyle.Font.FontName.ToString();
                            Color color = NormalizeColor(cell.CellStyle.Font.RGBColor);
                            int red = color.R;
                            int green = color.G;
                            int blue = color.B;
                            string fontColor = HtmlAttributes.Rgb + Operators.OpenBrace + red + Operators.Comma + green + Operators.Comma + blue + Operators.CloseBrace;
                            double fontSize = cell.CellStyle.Font.Size;
                            string bgColor = HtmlAttributes.Rgb + Operators.OpenBrace + backgroundRed + Operators.Comma + backgroundGreen + Operators.Comma + backgroundBlue + Operators.CloseBrace;

                            string underline = cell.CellStyle.Font.Underline.ToString(CultureInfo.InstalledUICulture.NumberFormat);
                            m_keyNoPosition = Operators.OpenCurlyBrace + HtmlAttributes.FontColor + Operators.Colon + fontColor + Operators.SemiColon + HtmlAttributes.FontFamily + Operators.Colon + fontName + Operators.SemiColon + HtmlAttributes.FontSize + Operators.Colon + fontSize + Pt + Operators.SemiColon + HtmlAttributes.BackgroundColor + Operators.Colon + bgColor + Operators.SemiColon + HtmlAttributes.Height + Operators.Colon + height + Operators.SemiColon;

                            Dictionary<string, string> borderDictionary = new Dictionary<string, string>();
                            Dictionary<string, string> currentBorderDictionary = new Dictionary<string, string>();



                            if (k < lstRegions.Count)
                            {
                                rowFrom = lstRegions[k].RowFrom;
                                columnFrom = lstRegions[k].ColumnFrom;
                                rowTo = lstRegions[k].RowTo;
                                columnTo = lstRegions[k].ColumnTo;
                                String cellPosition = (rowTo + 1).ToString() + GetColumnName(columnTo + 1);

                                IRange newResult = sheet.Range[cellPosition];
                                if (i == rowFrom + 1 && j == columnFrom + 1)
                                {
                                    borderDictionary = BuildBorders(cell);
                                    currentBorderDictionary = BuildBorders(newResult);

                                    foreach (string borderKey in currentBorderDictionary.Keys)
                                    {
                                        if (!borderDictionary.ContainsKey(borderKey))
                                        {
                                            m_keyNoPosition += borderKey + Operators.Colon + currentBorderDictionary[borderKey] + Operators.SemiColon;
                                        }
                                    }

                                    k++;
                                }

                            }

                  

                        m_keyNoPosition = GetBorderStyles(cell, m_keyNoPosition);
                        key = Operators.Dot + cellPos + m_keyNoPosition;

                        if (cell.CellStyle.Font.Bold)
                    {
                            key += HtmlAttributes.FontWeight + Operators.Colon + HtmlAttributes.Bold + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.FontWeight + Operators.Colon + HtmlAttributes.Bold + Operators.SemiColon;
                    }

                        if (cell.CellStyle.Font.Italic)
                    {
                            key += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.Italic + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.Italic + Operators.SemiColon;
                    }
                        if (cell.CellStyle.Font.Strikethrough)
                        {
                            key += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.StrikeThrough + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.FontStyle + Operators.Colon + HtmlAttributes.StrikeThrough + Operators.SemiColon;
                        }

                        if (!underline.Equals("None"))
                    {
                            key += HtmlAttributes.TextDecoration + Operators.Colon + HtmlAttributes.Underline + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.TextDecoration + Operators.Colon + HtmlAttributes.Underline + Operators.SemiColon;
                    }

                        if (cell.VerticalAlignment == ExcelVAlign.VAlignBottom) 
                    {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Bottom + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Bottom + Operators.SemiColon;
                        }
                        if (cell.VerticalAlignment == ExcelVAlign.VAlignTop)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Top + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Top + Operators.SemiColon;
                        }
                        if (cell.VerticalAlignment == ExcelVAlign.VAlignCenter)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Center + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Center + Operators.SemiColon;
                        }
                        if (cell.VerticalAlignment == ExcelVAlign.VAlignDistributed)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Distriburted + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Distriburted + Operators.SemiColon;
                        }
                        if (cell.VerticalAlignment == ExcelVAlign.VAlignJustify)
                        {
                            key += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Justify + Operators.SemiColon;
                            m_keyNoPosition += HtmlAttributes.VerticalAlign + Operators.Colon + HtmlAttributes.Justify + Operators.SemiColon;
                        }

                        if (cell.HorizontalAlignment == ExcelHAlign.HAlignCenter)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Center;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Center;

                    }
                        if (cell.HorizontalAlignment == ExcelHAlign.HAlignLeft)
                    {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Left;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Left;

                        }
                        int extendedFormatIndex = cell.ExtendedFormatIndex;
                        ExtendedFormatImpl extendedFormatImpl = cell.Workbook.InnerExtFormats[extendedFormatIndex];
                        if (cell.HasNumber || cell.HasDateTime ||
                            cell.HasFormula &&
                            cell.FormulaStringValue == null &&
                            !cell.HasFormulaErrorValue &&
                            !cell.HasFormulaBoolValue)
                        {
                            WorkbookImpl book = cell.Worksheet.Workbook as WorkbookImpl;
                            FormatImpl numberFormat = book.InnerFormats[extendedFormatImpl.NumberFormatIndex];
                            ExcelFormatType formatType = numberFormat.GetFormatType(cell.Number);

                            cell.HorizontalAlignment = (formatType == ExcelFormatType.Text) ?
                                ExcelHAlign.HAlignLeft:ExcelHAlign.HAlignRight;
                        }
                        if (cell.HorizontalAlignment == ExcelHAlign.HAlignRight)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Right;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Right;
                        }
                        if (cell.HorizontalAlignment == ExcelHAlign.HAlignJustify)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Justify;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Justify;
                        }
                        if (cell.HorizontalAlignment == ExcelHAlign.HAlignDistributed)
                        {
                            key += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Distriburted;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + HtmlAttributes.Distriburted;
                        }
                        if (cell.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
                        {
                            string general = HtmlAttributes.General;
                            if (cell.HasNumber)
                                general = HtmlAttributes.Right;
                            key += HtmlAttributes.TextAlignment + Operators.Colon + general;
                            m_keyNoPosition += HtmlAttributes.TextAlignment + Operators.Colon + general;
                        }

                        key += Operators.CloseCurlyBrace;
                        m_keyNoPosition += Operators.CloseCurlyBrace;
                        string className = String.Concat(".X", increment);

                        if (!m_styles.ContainsKey(m_keyNoPosition))
                        {
                            LinkedList<string> cellPositions = new LinkedList<string>();
                            cellPositions.AddFirst(className);
                            increment++;
                            cellPositions.AddLast(cellPos);
                            m_styles.Add(m_keyNoPosition, cellPositions);
                            stylesCount++;
                            }
                            else
                            {
                                LinkedList<string> cellPositions = (LinkedList<string>)m_styles[m_keyNoPosition];
                                cellPositions.AddLast(cellPos);
                            }

                        }
                    }
                }

            }

            string appendText = "." + HtmlTags.Table + sheet.Index.ToString() + Operators.OpenCurlyBrace + HtmlAttributes.BorderCollapse + Operators.Colon + "collapse" + Operators.SemiColon + HtmlAttributes.BorderSpacing + Operators.Colon + "0" + Operators.SemiColon + HtmlAttributes.EmptyCells + Operators.Colon + "show" + Operators.CloseCurlyBrace;
            builder.Append(appendText);
            foreach (string dictionaryKey in m_styles.Keys)
            {
                LinkedList<string> cellPositions = m_styles[dictionaryKey];
                builder.AppendLine();
                builder.Append(cellPositions.First.Value);
                builder.Append(dictionaryKey);
            }

            if (sheet.HasPictures && saveOption.ImagePath != null)
                GetImageStyles(sheet, builder);


            return builder.ToString();
        }

        /// <summary>
        /// Normalizes the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color NormalizeColor(Color color)
        {
            return Color.FromArgb(0xFF, color.R, color.G, color.B);
        }

        /// <summary>
        /// Gets the border styles.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="keyNoPosition">The key no position.</param>
        /// <returns></returns>
        private string GetBorderStyles(IRange result, string keyNoPosition)
        {
            IBorder border;
            ExcelBordersIndex index;
            IWorksheet sheet = result.Worksheet;
            string None = "None";
            border = result.Borders[ExcelBordersIndex.EdgeTop];
            index = ExcelBordersIndex.EdgeTop;
            string topBorder = CanDrawBorder(border, index, result);

            IRange range = sheet.Range[result.Row + 1, result.Column];
            border = result.Borders[ExcelBordersIndex.EdgeBottom];
            index = ExcelBordersIndex.EdgeBottom;
            string bottomBorder = CanDrawBorder(border, index, result);
            if (!range.IsMerged || result.IsMerged)
            {                
                bottomBorder = result.Borders[ExcelBordersIndex.EdgeBottom].LineStyle.ToString();
            }

            border = result.Borders[ExcelBordersIndex.EdgeLeft];
            index = ExcelBordersIndex.EdgeLeft;
            string leftBorder = CanDrawBorder(border, index, result);

            string rightBorder = result.Borders[ExcelBordersIndex.EdgeRight].LineStyle.ToString();
            string topBorderColorRed = result.CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB.R.ToString();
            string topBorderColorGreen = result.CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB.G.ToString();
            string topBorderColorBlue = result.CellStyle.Borders[ExcelBordersIndex.EdgeTop].ColorRGB.B.ToString();
            string topBorderColor = HtmlAttributes.Rgb + Operators.OpenBrace + topBorderColorRed + Operators.Comma + topBorderColorGreen + Operators.Comma + topBorderColorBlue + Operators.CloseBrace;

            string bottomBorderColorRed = result.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB.R.ToString();
            string bottomBorderColorGreen = result.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB.G.ToString();
            string bottomBorderColorBlue = result.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB.B.ToString();
            string bottomBorderColor = HtmlAttributes.Rgb + Operators.OpenBrace + bottomBorderColorRed + Operators.Comma + bottomBorderColorGreen + Operators.Comma + bottomBorderColorBlue + Operators.CloseBrace;

            string leftBorderColorRed = result.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB.R.ToString();
            string leftBorderColorGreen = result.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB.G.ToString();
            string leftBorderColorBlue = result.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB.B.ToString();
            string leftBorderColor = HtmlAttributes.Rgb + Operators.OpenBrace + leftBorderColorRed + Operators.Comma + leftBorderColorGreen + Operators.Comma + leftBorderColorBlue + Operators.CloseBrace;

            string rightBorderColorRed = result.CellStyle.Borders[ExcelBordersIndex.EdgeRight].ColorRGB.R.ToString();
            string rightBorderColorGreen = result.CellStyle.Borders[ExcelBordersIndex.EdgeRight].ColorRGB.G.ToString();
            string rightBorderColorBlue = result.CellStyle.Borders[ExcelBordersIndex.EdgeRight].ColorRGB.B.ToString();
            string rightBorderColor = HtmlAttributes.Rgb + Operators.OpenBrace + rightBorderColorRed + Operators.Comma + rightBorderColorGreen + Operators.Comma + rightBorderColorBlue + Operators.CloseBrace;

            if (topBorder != None)
                keyNoPosition += HtmlAttributes.TopBorder + Operators.Colon + GetDashStyle(topBorder) + Operators.SemiColon + HtmlAttributes.TopBorderWidth + Operators.Colon + GetBorderWidth(topBorder) + Operators.SemiColon;

            if (bottomBorder != None)
                keyNoPosition += HtmlAttributes.BottomBorder + Operators.Colon + GetDashStyle(bottomBorder) + Operators.SemiColon + HtmlAttributes.BottomBorderWidth + Operators.Colon + GetBorderWidth(bottomBorder) + Operators.SemiColon;

            if (leftBorder != None)
                keyNoPosition += HtmlAttributes.LeftBorder + Operators.Colon + GetDashStyle(leftBorder) + Operators.SemiColon + HtmlAttributes.LeftBorderWidth + Operators.Colon + GetBorderWidth(leftBorder) + Operators.SemiColon;

            if (rightBorder != None)
                keyNoPosition += HtmlAttributes.RightBorder + Operators.Colon + GetDashStyle(rightBorder) + Operators.SemiColon + HtmlAttributes.RightBorderWidth + Operators.Colon + GetBorderWidth(rightBorder) + Operators.SemiColon;

            keyNoPosition += HtmlAttributes.TopBorderColor + Operators.Colon + topBorderColor + Operators.SemiColon + HtmlAttributes.BottomBorderColor + Operators.Colon + bottomBorderColor + Operators.SemiColon + HtmlAttributes.LeftBorderColor + Operators.Colon + leftBorderColor + Operators.SemiColon + HtmlAttributes.RightBorderColor + Operators.Colon + rightBorderColor + Operators.SemiColon;
            return keyNoPosition;

        }

        private string CanDrawBorder(IBorder border,ExcelBordersIndex index,IRange result)
        {
            IWorksheet sheet = result.Worksheet;
            int row = result.Row;
            int column = result.Column;
            int maxrow = sheet.Workbook.MaxRowCount;
            int maxcol = sheet.Workbook.MaxColumnCount;
            string lineStyle = "None";

            int rowHeight = 0;
            int colWidth = 0;
            bool isTop = (index == ExcelBordersIndex.EdgeTop)? true : false;

            bool isLeft = (index == ExcelBordersIndex.EdgeLeft) ? true : false;

            if (result.Borders[index].LineStyle != ExcelLineStyle.None)
            {
                if (row - 1 > 0 && row + 1 < maxrow && isTop)
                {
                    IRange cell2 = sheet.Range[row - 1, column];                   
                    rowHeight = (int)cell2.RowHeight;
                    IBorders adjacentBorder = cell2.Borders;
                    if (adjacentBorder[ExcelBordersIndex.EdgeBottom].LineStyle == ExcelLineStyle.None || (rowHeight >0 && cell2.IsBlank) )
                    {
                        lineStyle = result.Borders[index].LineStyle.ToString();                        
                    }
                    else
                        lineStyle = ExcelLineStyle.None.ToString();

                }

                else if (column - 1 > 0 && column + 1 < maxcol && isLeft)
                {
                    IRange cell2 = sheet.Range[row, column - 1];
                    IBorders adjacentBorder = cell2.Borders;
                    if (adjacentBorder[ExcelBordersIndex.EdgeRight].LineStyle == ExcelLineStyle.None)
                    {
                        lineStyle = result.Borders[index].LineStyle.ToString();
                    }
                    else
                        lineStyle = ExcelLineStyle.None.ToString();
                }
                else
                {
                    lineStyle = result.Borders[index].LineStyle.ToString();
                }
               
            }           
            return lineStyle;
        }

        /// <summary>
        /// Gets the width of the border.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <returns></returns>
        private float GetBorderWidth(string border)
        {
            float result = 0;

            switch (border)
            {
                case "Hair":
                    result = 0.5F;
                    break;

                case "Thin":
                case "Dashed":
                case "Dotted":
                case "Dash_dot":
                case "Slanted_dash_dot":
                case "Dash_dot_dot":
                    result = 1F;
                    break;

                case "Medium":
                case "Medium_dashed":
                case "Double":
                case "Medium_dash_dot":
                case "Medium_dash_dot_dot":
                    result = 2F;
                    break;

                case "Thick":
                    result = 3F;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the dash style.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <returns></returns>
        private string GetDashStyle(string border)
        {
            string result = HtmlAttributes.Solid;
            switch (border)
            {
                case "Thin":
                case "Medium":
                case "Thick":
                case "Hair":
                    result = HtmlAttributes.Solid;
                    break;

                case "Double":
                    result = HtmlAttributes.Double;
                    break;

                case "Dashed":
                case "Medium_dashed":
                    result = HtmlAttributes.Dashed;
                    break;

                case "Dotted":
                case "Dash_dot":
                case "Medium_dash_dot":
                case "Slanted_dash_dot":
                case "Dash_dot_dot":
                case "Medium_dash_dot_dot":
                    result = HtmlAttributes.Dotted;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Builds the borders.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private Dictionary<string, string> BuildBorders(IRange result)
        {
            Dictionary<string, string> borderDictionary = new Dictionary<string, string>();
            string None = "None";
            string topBorder = result.Borders[ExcelBordersIndex.EdgeTop].LineStyle.ToString();
            string bottomBorder = result.Borders[ExcelBordersIndex.EdgeBottom].LineStyle.ToString();
            string leftBorder = result.Borders[ExcelBordersIndex.EdgeLeft].LineStyle.ToString();
            string rightBorder = result.Borders[ExcelBordersIndex.EdgeRight].LineStyle.ToString();

            if (topBorder != None)
            {
                borderDictionary.Add(HtmlAttributes.TopBorder, GetDashStyle(topBorder));
                borderDictionary.Add(HtmlAttributes.TopBorderWidth, GetBorderWidth(topBorder).ToString());
            }
            if (bottomBorder != None)
            {
                borderDictionary.Add(HtmlAttributes.BottomBorder, GetDashStyle(bottomBorder));
                borderDictionary.Add(HtmlAttributes.BottomBorderWidth, GetBorderWidth(bottomBorder).ToString());
            }
            if (leftBorder != None)
            {
                borderDictionary.Add(HtmlAttributes.LeftBorder, GetDashStyle(leftBorder));
                borderDictionary.Add(HtmlAttributes.LeftBorderWidth, GetBorderWidth(leftBorder).ToString());
            }
            if (rightBorder != None)
            {
                borderDictionary.Add(HtmlAttributes.RightBorder, GetDashStyle(rightBorder));
                borderDictionary.Add(HtmlAttributes.RightBorderWidth, GetBorderWidth(rightBorder).ToString());
            }

            return borderDictionary;
        }

        /// <summary>
        /// Gets the image styles.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="builder">The builder.</param>
        private void GetImageStyles(WorksheetImpl sheet, StringBuilder builder)
        {

            IPictures pictures = sheet.Pictures;
            for (int i = 0; i < pictures.Count; i++)
            {
                string uniqueId = "";
                IPictureShape pictureShape = pictures[i];
                Image img = pictureShape.Picture;
                int left = pictureShape.Left;
                int top = pictureShape.Top;
                int height = pictureShape.Height;
                int width = pictureShape.Width;
                uniqueId = string.Concat(".ix", i);
               
                string toWrite = String.Concat(uniqueId, Operators.OpenCurlyBrace);
                toWrite = String.Concat(toWrite, HtmlAttributes.MarginLeft, Operators.Colon, left,Px, Operators.SemiColon);
                toWrite = String.Concat(toWrite, HtmlAttributes.MarginTop, Operators.Colon, top,Px,Operators.SemiColon);
                toWrite = String.Concat(toWrite, HtmlAttributes.Height, Operators.Colon, height.ToString(CultureInfo.InvariantCulture),Px, Operators.SemiColon);
                toWrite = String.Concat(toWrite, HtmlAttributes.Width, Operators.Colon, width,Px, Operators.SemiColon);
                toWrite = String.Concat(toWrite, HtmlAttributes.Position, Operators.Colon, HtmlAttributes.Absolute, Operators.CloseCurlyBrace);

                builder.Append(toWrite);
                builder.AppendLine();
            }

        }

        /// <summary>
        /// Builds the merged regions.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <returns></returns>
        private List<string> BuildMergedRegions(WorksheetImpl sheet)
        {
            MergeCellsImpl mergedCells = sheet.MergeCells;
            IRange range = sheet.UsedRange;
            int rowStart = range.Row;
            int columnStart = range.Column;
            int rowEnd = range.LastRow;
            int columnEnd = range.LastColumn;
            mergedCells.CacheMerges(sheet[rowStart, columnStart, rowEnd, columnEnd], lstRegions);
            List<string> cellPosiotions = new List<string>();

            for (int k = 0; k < lstRegions.Count; k++)
            {
                int rowFrom = lstRegions[k].RowFrom + 1;
                int rowTo = lstRegions[k].RowTo + 1;
                int colFrom = lstRegions[k].ColumnFrom + 1;
                int colTo = lstRegions[k].ColumnTo + 1;

                for (int i = rowFrom; i <= rowTo; i++)
                {
                    for (int j = colFrom; j <= colTo; j++)
                    {
                        if (!(i == rowFrom && j == colFrom))
                        {
                            string cellPosition = Operators.OpenBrace + i + Operators.Comma + j + Operators.CloseBrace;
                            cellPosiotions.Add(cellPosition);
                        }
                    }
                }

            }
            return cellPosiotions;
        }

        /// <summary>
        /// Gets the column width for an excel worksheet
        /// </summary>
        /// <param name="sheetImpl"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int GetColumnWidth(WorksheetImpl sheetImpl, int columnIndex)
        {
            List<int> columnWidths = new List<int>();
            MigrantRangeImpl cell = new MigrantRangeImpl(sheetImpl.Application, sheetImpl);

            int row=sheetImpl.UsedRange.Row;
            int endRow=sheetImpl.UsedRange.LastRow;
            int lastPossibleColumn = sheetImpl.ParentWorkbook.MaxColumnCount;
            int lastColumnIndex=0;
            for (; row < endRow; row++)
            {
                cell.ResetRowColumn(row, columnIndex);
                lastColumnIndex =cell.Column;
                if (!cell.IsBlank && !cell.WrapText && (cell.HasString || cell.FormulaStringValue != null))
                {
                    SizeF cellSize = sheetImpl.MeasureCell(cell,false,false);
                    int requiredWidth = (int)cellSize.Width;
                    int columnWidth = sheetImpl.GetColumnWidthInPixels(columnIndex);
                    if (requiredWidth > columnWidth)
                    {
                        int deltaIndex=1;
                        IRange cell2 = sheetImpl.Range[row, lastColumnIndex + deltaIndex];
                        while(cell2.IsBlank && requiredWidth > columnWidth && lastColumnIndex < lastPossibleColumn)
                        {
                            columnWidth += sheetImpl.GetColumnWidthInPixels(lastColumnIndex + deltaIndex);  
                            lastColumnIndex += deltaIndex;
                            cell2 = sheetImpl.Range[row, lastColumnIndex + deltaIndex];
                        }
                        int firstColumn = Math.Min(lastColumnIndex, cell.Column);
                        int lastColumn = Math.Max(lastColumnIndex, cell.Column);

                        int newWidth = ColumnWidthGetter.GetTotal(firstColumn, lastColumn);
                        columnWidths.Add(newWidth);
                    }
                    else 
                    {
                        columnWidths.Add(columnWidth);
                    }
                }
            }

            return GetMaxWidth(columnWidths);
        }

        /// <summary>
        /// Gets the maximum width value
        /// </summary>
        /// <param name="maxList"></param>
        /// <returns></returns>
        private int GetMaxWidth(List<int> maxList)
        {
            int max = 0;
            for (int i = 0; i < maxList.Count; i++)
            {
                int val = maxList[i];
                if (val > max)
                {
                    max = val;
                }
            }
            return max;
        }

        /// <summary>
        /// Writes the content of the sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="outputDirectoryPath">The output directory path.</param> 
        /// <param name="saveOption">The saveoption.</param> 
        private void WriteSheetContent(WorksheetImpl sheet, string outputDirectoryPath, HtmlSaveOptions saveOption, XmlTextWriter Writer)
        {
            StringBuilder build = new StringBuilder();
            IRange range = sheet.UsedRange;
            int rowStart = range.Row;
            int columnStart = range.Column;
            int rowEnd = range.LastRow;
            int columnEnd = range.LastColumn;
            List<string> mergedRegions = new List<string>();
            List<string> longWidthCells = new List<string>();
            IPictures picture = sheet.Pictures;
            int k = 0;
            int columnGap = 0;
            int rowGap = 0;
            MigrantRangeImpl result = new MigrantRangeImpl(sheet.Application, sheet);
            string toWrite = "";
            int l = 0;
            int sumRowHeight = 0;
            int sumColWidth = 0;
            int rowHeight = 0;
            int colWidth = 0;
            int g = 0;

            if (sheet.HasPictures && saveOption.ImagePath != null)
                WriteImage(sheet, Writer, outputDirectoryPath, saveOption);


            if (sheet.HasMergedCells)
                mergedRegions = BuildMergedRegions(sheet);

            Writer.WriteStartElement(HtmlTags.Table);
            Writer.WriteAttributeString(HtmlAttributes.CellSpacing, "0");
            List<int> columnsWidth = new List<int>();
            for (int i = 1; i <= columnEnd; i++)
            {
                int columnWidth = sheet.GetColumnWidthInPixels(i);
                if (columnWidth > 0)
                {
                    Writer.WriteStartElement(HtmlTags.Col);
                    //int newColumnWidth = GetColumnWidth(sheet, i);
                    //if (newColumnWidth != 0)
                    //{
                    //    columnsWidth.Add(newColumnWidth);
                    //    Writer.WriteAttributeString(HtmlAttributes.Width, newColumnWidth.ToString());
                    //}
                    //else
                    //{
                        columnsWidth.Add(columnWidth);
                        Writer.WriteAttributeString(HtmlAttributes.Width, columnWidth.ToString());
                  //  }
                    Writer.WriteEndElement();
                }
            }

            for (int i = 1; i <= rowEnd; i++)
            {
                result.ResetRowColumn(i, 1);
                rowHeight = sheet.GetRowHeightInPixels(i);
                int colCount = 0;
                if (rowHeight>0)
                {
                    Writer.WriteStartElement(HtmlTags.Tr);
                    sumRowHeight += rowHeight;
                    sumColWidth = 0;
                    

                    for (int j = 1; j <= columnEnd; j++)
                    {

                            colWidth = sheet.GetColumnWidthInPixels(j);
                            sumColWidth += colWidth;
                            string cellPos;
                            string row = i.ToString();
                            cellPos = row + GetColumnName(j);
                            result.ResetRowColumn(i, j);
                            if (colWidth>0)
                            {
                                ++colCount;
                            string width = String.Concat(result.ColumnWidth, ",");
                            ExtendedFormatImpl xf = (result.CellStyle as CellStyle).Wrapped;
                            xf = this.conditionalFormatApplier.ApplyCF(result, xf);
                            if (!sheet.HasMergedCells)
                            {
                                string cellPosition = Operators.OpenBrace + i + Operators.Comma + j + Operators.CloseBrace;
                                if (longWidthCells.Contains(cellPosition))
                                    continue;
                                Writer.WriteStartElement(HtmlTags.Td);
                            }
                            #region MergedCells
                            if (sheet.HasMergedCells)
                            {
                                string cellPosition = Operators.OpenBrace + i + Operators.Comma + j + Operators.CloseBrace;

                                if (mergedRegions.Contains(cellPosition))                               
                                    continue;
                              
                                Writer.WriteStartElement(HtmlTags.Td);
                                string flag = "false";

                                if (k < lstRegions.Count)
                                {
                                    int rowFrom = lstRegions[k].RowFrom;
                                    int columnFrom = lstRegions[k].ColumnFrom;
                                    int rowTo = lstRegions[k].RowTo;
                                    int columnTo = lstRegions[k].ColumnTo;
                                    if (i == rowFrom + 1 && j == columnFrom + 1)
                                    {
                                        columnGap = columnTo - columnFrom + 1;
                                        rowGap = rowTo - rowFrom + 1;

                                        if (columnGap > 1)
                                            Writer.WriteAttributeString(HtmlAttributes.ColumnSpan, columnGap.ToString());
                                        if (rowGap > 1)
                                            Writer.WriteAttributeString(HtmlAttributes.RowSpan, rowGap.ToString());

                                        k++;
                                        //flag = "true";
                                    }

                                }

                                string className = "";

                                foreach (string key in m_styles.Keys)
                                {
                                    LinkedList<string> cellPositions = m_styles[key];

                                    if (cellPositions.Contains(cellPos))
                                        className = cellPositions.First.Value.Substring(1);
                                }

                                Writer.WriteAttributeString(HtmlAttributes.Class, className);
                                string value = null;

                                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                                    value = result.DisplayText;
                                else
                                    value = result.Value;


                                if (flag.Equals("true"))
                                    Writer.WriteString(value);

                                if (value.Equals(""))
                                    Writer.WriteRaw("&nbsp");

                                if (flag.Equals("false") && !value.Equals(""))
                                {
                                    int count = WriteCellWidth(result, Writer, columnsWidth, colCount, row, sheet, saveOption);
                                    int currentColumnNo = j;

                                    for (int c = 0; c < count; c++)
                                    {
                                        cellPosition = Operators.OpenBrace + i + Operators.Comma + currentColumnNo + Operators.CloseBrace;
                                        mergedRegions.Add(cellPosition);
                                        currentColumnNo++;
                                    }

                                    NormalizeString(value, result, count, columnsWidth, colCount, sheet, saveOption);
                                }

                                Writer.WriteEndElement();
                            }
                            #endregion


                            #region mergedcells
                            if (!sheet.HasMergedCells)
                            {
                                int count = WriteCellWidth(result, Writer, columnsWidth, colCount, row, sheet, saveOption);
                                int currentColumnNo = j;

                                for (int c = 0; c < count; c++)
                                {
                                    string group = Operators.OpenBrace + i + Operators.Comma + currentColumnNo + Operators.CloseBrace;
                                    longWidthCells.Add(group);
                                    currentColumnNo++;
                                }

                                string className = "";

                                foreach (string key in m_styles.Keys)
                                {
                                    LinkedList<string> cellPositions = m_styles[key];
                                    if (cellPositions.Contains(cellPos))
                                        className = cellPositions.First.Value.Substring(1);
                                }

                                Writer.WriteAttributeString(HtmlAttributes.Class, className);
                                string value = null;
                                IConditionalFormats formats = result.ConditionalFormats;

                                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                                    value = result.DisplayText;
                                else
                                    value = result.Value;

                                if (value.Equals(""))
                                    Writer.WriteRaw("&nbsp");
                                else
                                    NormalizeString(value, result, count, columnsWidth, colCount, sheet, saveOption);

                                Writer.WriteEndElement();
                            }
                            #endregion
                            }
                    }
                Writer.WriteEndElement();
                }
            }
            Writer.WriteEndElement();
        }
        /// <summary>
        /// Writes the content of the sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="saveOption">The save option.</param>
        /// <param name="Writer">The writer.</param>
        /// <param name="styles">The styles.</param>
        private void WriteSheetContent(WorksheetImpl sheet, HtmlSaveOptions saveOption, XmlTextWriter Writer, Dictionary<string, LinkedList<string>> styles)
        {
            StringBuilder build = new StringBuilder();
            IRange range = sheet.UsedRange;
            int rowStart = range.Row;
            int columnStart = range.Column;
            int rowEnd = range.LastRow;
            int columnEnd = range.LastColumn;
            List<string> mergedRegions = new List<string>();
            List<string> longWidthCells = new List<string>();
            int k = 0;
            int columnGap = 0;
            int rowGap = 0;
            MigrantRangeImpl result = new MigrantRangeImpl(sheet.Application, sheet);
            string toWrite = "";
            int l = 0;
            int sumRowHeight = 0;
            int sumColWidth = 0;
            int rowHeight = 0;
            int colWidth = 0;
            int g = 0;
            bool checkTableContent = false;

            if (sheet.HasPictures)
                WriteImageStream(sheet, Writer, saveOption);


            if (sheet.HasMergedCells)
                mergedRegions = BuildMergedRegions(sheet);

            Writer.WriteStartElement(HtmlTags.Table);
            Writer.WriteAttributeString(HtmlAttributes.Class, HtmlTags.Table + sheet.Index);
            Writer.WriteAttributeString(HtmlAttributes.CellSpacing, "0");
            List<int> columnsWidth = new List<int>();
            for (int i = 1; i <= columnEnd; i++)
            {
                int columnWidth = sheet.GetColumnWidthInPixels(i);
                if (columnWidth > 0)
                {
                    Writer.WriteStartElement(HtmlTags.Col);                                
                    columnsWidth.Add(columnWidth);
                    Writer.WriteAttributeString(HtmlAttributes.Width, sheet.GetColumnWidthInPixels(i).ToString());
                    Writer.WriteEndElement();
                    checkTableContent = true;
                }
            }

            for (int i = 1; i <= rowEnd; i++)
            {
                result.ResetRowColumn(i, 1);
                rowHeight = sheet.GetRowHeightInPixels(i);
                int colCount = 0;
                if(rowHeight>0)
                {
                    Writer.WriteStartElement(HtmlTags.Tr);
                    for (int j = 1; j <= columnEnd; j++)
                    {
                        Writer.WriteStartElement(HtmlTags.Td);
                        colWidth = sheet.GetColumnWidthInPixels(j);
                        sumColWidth += colWidth;
                        string cellPos;
                        string row = i.ToString();
                        cellPos = row + GetColumnName(j);
                        result.ResetRowColumn(i, j);
                        if (colWidth > 0)
                        {
                            ++colCount;
                            string width = String.Concat(result.ColumnWidth, ",");
                            ExtendedFormatImpl xf = (result.CellStyle as CellStyle).Wrapped;
                            xf = this.conditionalFormatApplier.ApplyCF(result, xf);

                          
                            sumRowHeight += rowHeight;
                            sumColWidth = 0;

                            if (!sheet.HasMergedCells)
                            {
                                string cellPosition = Operators.OpenBrace + i + Operators.Comma + j + Operators.CloseBrace;
                                if (longWidthCells.Contains(cellPosition))
                                    continue;
                               
                            }

                            if (sheet.HasMergedCells)
                            {
                                string cellPosition = Operators.OpenBrace + i + Operators.Comma + j + Operators.CloseBrace;

                                if (mergedRegions.Contains(cellPosition))
                                    continue;

                                string flag = "false";

                                if (k < lstRegions.Count)
                                {
                                    int rowFrom = lstRegions[k].RowFrom;
                                    int columnFrom = lstRegions[k].ColumnFrom;
                                    int rowTo = lstRegions[k].RowTo;
                                    int columnTo = lstRegions[k].ColumnTo;
                                    if (i == rowFrom + 1 && j == columnFrom + 1)
                                    {
                                        columnGap = columnTo - columnFrom + 1;
                                        rowGap = rowTo - rowFrom + 1;

                                        if (columnGap > 1)
                                            Writer.WriteAttributeString(HtmlAttributes.ColumnSpan, columnGap.ToString());
                                        if (rowGap > 1)
                                            Writer.WriteAttributeString(HtmlAttributes.RowSpan, rowGap.ToString());

                                        k++;
                                        flag = "true";
                                    }

                                }
                                string className = "";

                                foreach (string key in styles.Keys)
                                {
                                    LinkedList<string> cellPositions = styles[key];

                                    if (cellPositions.Contains(cellPos))
                                        className = cellPositions.First.Value.Substring(1);
                                }

                                Writer.WriteAttributeString(HtmlAttributes.Class, className);
                                string value = null;

                                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                                    value = result.DisplayText;
                                else
                                    value = result.Value;

                                if (flag.Equals("true"))
                                    Writer.WriteString(value);
                               
                                if (flag.Equals("false") && !value.Equals(""))
                                {
                                    int count = WriteCellWidth(result, Writer, columnsWidth, j, row, sheet, saveOption);
                                    int currentColumnNo = j;

                                    for (int c = 0; c < count; c++)
                                    {
                                        cellPosition = Operators.OpenBrace + i + Operators.Comma + currentColumnNo + Operators.CloseBrace;
                                        mergedRegions.Add(cellPosition);
                                        currentColumnNo++;
                                    }
                                    NormalizeString(value, result, count, columnsWidth, j, sheet, saveOption);
                                }
                            }
                            if (!sheet.HasMergedCells)
                            {
                                int count = WriteCellWidth(result, Writer, columnsWidth, j, row, sheet, saveOption);
                                int currentColumnNo = j;

                                for (int c = 0; c < count; c++)
                                {
                                    string group = Operators.OpenBrace + i + Operators.Comma + currentColumnNo + Operators.CloseBrace;
                                    longWidthCells.Add(group);
                                    currentColumnNo++;
                                }
                                string className = "";

                                foreach (string key in styles.Keys)
                                {
                                    LinkedList<string> cellPositions = styles[key];
                                    if (cellPositions.Contains(cellPos))
                                        className = cellPositions.First.Value.Substring(1);
                                }

                                Writer.WriteAttributeString(HtmlAttributes.Class, className);
                                string value = null;

                                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                                    value = result.DisplayText;
                                else
                                    value = result.Value;

                                if (!value.Equals(""))
                                    NormalizeString(value, result, count, columnsWidth, j, sheet, saveOption);

                            }
                        }
                        Writer.WriteEndElement();
                    }
                checkTableContent = true;
                Writer.WriteEndElement();
            }
                }
            if (checkTableContent)
                Writer.WriteEndElement();
            else
            {
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the image stream.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="Writer">The writer.</param>
        /// <param name="saveOption">The save option.</param>
        private void WriteImageStream(WorksheetImpl sheet, XmlTextWriter Writer, HtmlSaveOptions saveOption)
        {
            IPictures pictures = sheet.Pictures;
            string embeddedImage = string.Empty;
            for (int i = 0; i < pictures.Count; i++)
            {
                IPictureShape pictureShape = pictures[i];
                Image img = pictureShape.Picture;
                ImageFormat imageFormat = img.RawFormat;
                embeddedImage = ImageToBase64(img, imageFormat);
                int left = pictureShape.Left;
                int top = pictureShape.Top;
                int height = pictureShape.Height;
                int width = pictureShape.Width;
                string uniqueName = string.Concat(".ix", i);
                Writer.WriteStartElement(HtmlTags.Img);
                Writer.WriteAttributeString(HtmlAttributes.Class, uniqueName.Substring(1));
                Writer.WriteAttributeString(HtmlAttributes.Source,string.Format("data:image/png;base64,{0}", embeddedImage));

                string alternativeText = string.IsNullOrEmpty(pictureShape.AlternativeText) ? "Image" : pictureShape.AlternativeText;

                Writer.WriteAttributeString(HtmlAttributes.Alt, alternativeText);
                Writer.WriteEndElement();

            }
        }

        /// <summary>
        /// Sets the Hyperlink for the cell.
        /// </summary>
        /// <param name="cell">The cell value</param>        
        private void SerializeHyperLink(IRange cell)
        {
            IHyperLinks hyperlinks = this.workSheet.HyperLinks;
            //
            //Todo: The formula hyperlink style name is correct. But it should be added to the cell styles and Hyper link collection.
            //This collection must be udpated with CalcEngine team while evaluating this formula.
            //
            if (cell.HasFormula && cell.Formula.StartsWith("=HYPERLINK") && cell.DisplayText.Length > 0)
            {
                Writer.WriteStartElement(HtmlTags.A);
                string formula = cell.Formula;
                string fAddress = formula.Substring(formula.IndexOf('(')+1);
                fAddress = GetAddress(fAddress);                
                Writer.WriteAttributeString(HtmlAttributes.Href, fAddress);
                Writer.WriteAttributeString(HtmlAttributes.Target, HtmlAttributes.Parent);
                Writer.WriteEndElement();
            }
            else if (hyperlinks.Count == 0)
            {
                return;
            }
            
            Syncfusion.XlsIO.Implementation.Collections.CollectionBase<Syncfusion.XlsIO.Implementation.HyperLinkImpl> address = cell.Hyperlinks as Syncfusion.XlsIO.Implementation.Collections.HyperLinksCollection;

            if ( address.InnerList.Count > 0 && (address.InnerList[0].Type == ExcelHyperLinkType.Url ||
                address.InnerList[0].Type == ExcelHyperLinkType.File))
            {
                Writer.WriteStartElement(HtmlTags.A);
                Writer.WriteAttributeString(HtmlAttributes.Href, address.InnerList[0].Address);
                Writer.WriteAttributeString(HtmlAttributes.Target,HtmlAttributes.Parent );
                Writer.WriteEndElement();
            }

        }
        /// <summary>
        /// Returns the hyperlink.
        /// </summary>
        /// <param name="address">It's represent cell formula</param>
        /// <returns></returns>
        private string GetAddress(string address)
        {
            string result=string.Empty;
            if (address.StartsWith("\""))
            {
                string[] split = address.Split(new string[] { "\"" }, StringSplitOptions.None);
                result = split[1];
            }
            else
            {
                if (address.Contains(","))
                    address = address.Substring(0, address.IndexOf(","));
                else
                    address = address.Remove(address.IndexOf(")"));
                result = this.workSheet[address].DisplayText;
            }
            return result;
        }
        /// <summary>
        /// Normalizes the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <param name="count">The count.</param>
        /// <param name="columnsWidth">Width of the columns.</param>
        /// <param name="columnNo">The column number</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="saveOption">The saveoption.</param>
        private void NormalizeString(string value, IRange result, int count, List<int> columnsWidth, int columnNo, WorksheetImpl sheet, HtmlSaveOptions saveOption)
        {

            int totalWidth = 0;

            for (int y = result.Column; y <= result.Column + count; y++)
            {
                int width = sheet.GetColumnWidthInPixels(y);
                totalWidth += Int32.Parse(width.ToString());
            }

            IFont font = result.CellStyle.Font;
            Font nativeFont = font.GenerateNativeFont();
            StringMeasurer measurer = new StringMeasurer();
            SizeF size;

            if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                size = measurer.Measure(result.DisplayText, nativeFont);
            else
                size = measurer.Measure(result.Value, nativeFont);

            string contentWidth = size.Width.ToString();
            double cellContentWidth = Double.Parse(contentWidth);
            int contentWidthInt = System.Convert.ToInt32(cellContentWidth);

            if (contentWidthInt <= totalWidth)
            {
                if (result.CellStyleName == HyperlinkStyleName ||
                (result.HasFormula && result.Formula.StartsWith("=HYPERLINK") 
                && result.DisplayText.Length > 0))
                {
                    SerializeHyperLink(result);
                }               
                Writer.WriteString(value);
            }

            else
            {
                char[] valueChar = value.ToCharArray();
                int totalContentWidth = 0;
                string modifiedValue = "";
                string subString = "";
                double totalColumnWidth = Double.Parse(totalWidth.ToString());
                string originalText = null;
                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                    originalText = result.DisplayText;
                else
                    originalText = result.Value;

                string originalSubString = "";
                int r;
                for (r = 0; r < originalText.Length; r++)
                {

                    if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                        subString = result.DisplayText.Substring(0, result.DisplayText.Length - r);
                    else
                        subString = result.Value.Substring(0, result.Value.Length - r);

                    SizeF stringSize = measurer.Measure(subString, nativeFont);
                    cellContentWidth = Double.Parse(stringSize.Width.ToString());

                    if (cellContentWidth <= totalColumnWidth)
                    {
                        originalSubString = subString;
                        break;
                    }

                }

                int startIndex = originalText.Length - r;
                string hiddenString = originalText.Substring(startIndex, r);
                //
            //Todo: The formula hyperlink style name is correct. But it should be added to the cell styles and Hyper link collection.
            //This collection must be udpated with CalcEngine team while evaluating this formula.
            //
                if (result.CellStyleName == HyperlinkStyleName ||
                (result.HasFormula && result.Formula.StartsWith("=HYPERLINK") 
                && result.DisplayText.Length > 0))
                {
                    SerializeHyperLink(result);
                }
                Writer.WriteString(originalSubString);
                Writer.WriteStartElement(HtmlTags.span);
                Writer.WriteAttributeString(HtmlTags.Style, "display:none");
                Writer.WriteString(hiddenString);
                Writer.WriteEndElement();

            }
        }

        /// <summary>
        /// Writes the width of the cell.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="Writer">The writer.</param>
        /// <param name="columnsWidth">Width of the columns.</param>
        /// <param name="columnNo">The column Number.</param>
        /// <param name="row">The row.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="saveOption">The saveoption.</param>
        /// <returns></returns>
        private int WriteCellWidth(IRange result, XmlTextWriter Writer, List<int> columnsWidth, int columnNo, string row, WorksheetImpl sheet, HtmlSaveOptions saveOption)
        {
            IFont font = result.CellStyle.Font;
            Font nativeFont = font.GenerateNativeFont();
            SizeF size;
            string contentWidth = null;
            int count = 0;
            string cellText = null;
            ExtendedFormatImpl xf = (result.CellStyle as CellStyle).Wrapped;
            xf = this.conditionalFormatApplier.ApplyCF(result, xf);
            IConditionalFormats conditinalFormat = result.ConditionalFormats;
            if (conditinalFormat.Count <= 0)
            {
                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                    cellText = result.DisplayText;
                else
                    cellText = result.Value;
            }

            if (cellText != "" ||result.HasStyle)
            {
                StringMeasurer measurer = new StringMeasurer();
                if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                    size = measurer.Measure(result.DisplayText, nativeFont);
                else
                    size = measurer.Measure(result.Value, nativeFont);

                int cellContentWidth = System.Convert.ToInt32(Double.Parse(size.Width.ToString()));
                int prevColumnNo = columnNo - 1;
                int postColumnNo = result.Column + 1;
                int sumColumnWidth = Int32.Parse(columnsWidth[prevColumnNo].ToString());
                int cellColumnWidth = sheet.GetColumnWidthInPixels(result.Column);
                if (cellColumnWidth < sumColumnWidth)
                    count = 1;
                else
                    count = 0;
                for (int h = prevColumnNo + 1; h < columnsWidth.Count; h++)
                {
                    if (cellColumnWidth < sumColumnWidth)
                    {
                        string realRow = GetColumnName(result.Column);
                        postColumnNo++;
                        String cellPos = row + realRow;
                        result = sheet.Range[cellPos];
                        string value = null;
                        if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                            value = result.DisplayText;
                        else
                            value = result.Value;

                        if (value != "")
                            break;
                        if (value == "")
                        {
                            cellColumnWidth += Int32.Parse(columnsWidth[h].ToString());
                            count++;
                        }
                    }

                    if (cellContentWidth < sumColumnWidth)
                        break;
                    else
                    {
                        string realRow = GetColumnName(postColumnNo);
                        postColumnNo++;
                        String cellPos = row + realRow;
                        result = sheet.Range[cellPos];
                        string value = null;

                        if (saveOption.TextMode == HtmlSaveOptions.GetText.DisplayText)
                            value = result.DisplayText;
                        else
                            value = result.Value;

                        if (value != "" || result.ConditionalFormats.Count>0)
                            break;
                        if (value == "")
                        {
                            sumColumnWidth += Int32.Parse(columnsWidth[h].ToString());
                            count++;
                        }
                    }
                }
            }
            if (count > 0)
                Writer.WriteAttributeString(HtmlAttributes.ColumnSpan, count.ToString());

            return count;
        }

        /// <summary>
        /// Writes the image.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="Writer">The writer.</param>
        /// <param name="outputDirectoryPath">The output directory path.</param> 
        /// <param name="saveOption">The saveoption.</param>
        private void WriteImage(WorksheetBaseImpl sheet, XmlTextWriter Writer, string outputDirectoryPath, HtmlSaveOptions saveOption)
        {

            IPictures pictures = sheet.Pictures;
            string directory = new DirectoryInfo(outputDirectoryPath).Name;
            for (int i = 0; i < pictures.Count; i++)
            {
                IPictureShape pictureShape = pictures[i];
                Image img = pictureShape.Picture;
                ImageFormat imageFormat = img.RawFormat;
                ImageToBase64(img, imageFormat);
                int left = pictureShape.Left;
                int top = pictureShape.Top;
                int height = pictureShape.Height;
                int width = pictureShape.Width;
                string extension = GetExtension(imageFormat);
                string sheetName = sheet.Name;
                string fileName = string.Format("{0}." + extension, sheetName + i);
                string sourceFile = Path.Combine(outputDirectoryPath, fileName);
                string sourceFilePath = string.Format("{0}." + extension, sheetName + i);
                string uniqueName = string.Concat(".ix", i);
                Writer.WriteStartElement(HtmlTags.Img);
                Writer.WriteAttributeString(HtmlAttributes.Class, uniqueName.Substring(1));

                if (m_conversionMode == ConversionMode.Worksheet)
                {
                    sourceFilePath = Path.Combine(saveOption.ImagePath, string.Format("{0}." + extension, sheetName + i));
                    Writer.WriteAttributeString(HtmlAttributes.Source, sourceFilePath);
                }

                else
                    Writer.WriteAttributeString(HtmlAttributes.Source, sourceFilePath);

                string alternativeText = string.IsNullOrEmpty(pictureShape.AlternativeText) ? "Image" : pictureShape.AlternativeText;

                Writer.WriteAttributeString(HtmlAttributes.Alt, alternativeText);
                Writer.WriteEndElement();
                img.Save(sourceFile);

            }

        }

        private string ImageToBase64(Image img, ImageFormat imageFormat)
        {
            string base64String = string.Empty;
            using (MemoryStream mstream = new MemoryStream())
            {
                img.Save(mstream, imageFormat);
                byte[] imageBytes = mstream.ToArray();
                base64String = Convert.ToBase64String(imageBytes);
            }
            return base64String;
        }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private string GetExtension(ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            string strResult;
            if (format.Equals(ImageFormat.Bmp))
                strResult = "bmp";
            else if (format.Equals(ImageFormat.Jpeg))
                strResult = "jpeg";
            else if (format.Equals(ImageFormat.Png))
                strResult = "png";
            else if (format.Equals(ImageFormat.Emf))
                strResult = "emf";
            else if (format.Equals(ImageFormat.Gif))
                strResult = "gif";
            else
                strResult = "png";

            return strResult;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="iColumn">The i column.</param>
        /// <returns></returns>
        private string GetColumnName(int iColumn)
        {
            iColumn--;
            string strColumnName = string.Empty;
            do
            {
                int iCurrentDigit = iColumn % 26;
                iColumn = iColumn / 26 - 1;
                strColumnName = (char)('A' + iCurrentDigit) + strColumnName;
            }
            while (iColumn >= 0);
            return strColumnName;
        }

        /// <summary>
        /// Builds the HTML files.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="outputDirectoryPath">The output directory path.</param> 
        /// <param name="saveOption">The saveoption.</param> 
        private void BuildHtmlFiles(WorkbookImpl book, string outputDirectoryPath, HtmlSaveOptions saveOption)
        {
            string directory = new DirectoryInfo(outputDirectoryPath).Name;
            int count = book.Worksheets.Count;
            for (int i = 0; i < count; i++)
            {
                string fileName = string.Format("{0}.html", book.Worksheets[i].Name);
                fileName = Path.Combine(outputDirectoryPath, fileName);

                if (File.Exists(fileName))
                    File.Delete(fileName);

                using (FileStream fileStream = new FileStream(fileName, FileMode.CreateNew))
                {
                    WorksheetImpl sheet = (WorksheetImpl)book.Worksheets[i];
                    this.workSheet = sheet;
                    m_writer = new XmlTextWriter(fileStream, Encoding.UTF8);
                    m_writer.Formatting = Formatting.Indented;
                    m_styles = new Dictionary<string, LinkedList<string>>();
                    lstRegions = new List<MergeCellsRecord.MergedRegion>();
                    WriteDocumentStart();
                    BuildStyles(sheet, saveOption, m_writer);
                    Writer.WriteStartElement(HtmlTags.Body);
                    WriteSheetContent(sheet, outputDirectoryPath, saveOption, m_writer);
                    Writer.WriteEndElement();
                    WriteDocumentEnd();
                    fileStream.Close();
                }
            }
        }
        #endregion

        #region Internals
        /// <summary>
        /// Utility class for measuring the size of the string.
        /// </summary>
        private class StringMeasurer
        {
            #region Fields
            private Graphics m_graphics;
            #endregion

            #region Properties
            internal Graphics Graphics
            {
                get
                {
                    return m_graphics;
                }
            }
            #endregion

            #region Constructor
            public StringMeasurer()
            {
                InitializeGraphics();
            }
            #endregion

            #region Implementation
            public void InitializeGraphics()
            {
                Bitmap bmp = new Bitmap(1, 1);
                m_graphics = Graphics.FromImage(bmp);
            }

            public SizeF Measure(string text, Font font)
            {
                return Graphics.MeasureString(text, font, new SizeF(float.MaxValue, float.MaxValue), StringFormat.GenericTypographic);
            }
            #endregion
        }

        private class HtmlTags
        {
            public static string Html = "html";
            public static string Head = "head";
            public static string Title = "title";
            public static string Style = "style";
            public static string Body = "body";
            public static string Div = "div";
            public static string A = "a";
            public static string P = "P";
            public static string Font = "font";
            public static string Table = "table";
            public static string Td = "td";
            public static string Tr = "tr";
            public static string Th = "th";
            public static string Img = "img";
            public static string Col = "Col";
            public static string span = "span";
            public static string Script = "script";
            public static string Iframe = "iframe";
            public static string Input = "input";
            public static string FrameSet = "frameset";
            public static string Frame = "frame";
            public static string NoFrames = "noframes";
            public static string Bold = "b";
            public static string Small = "small";

        }

        private class HtmlAttributes
        {
            public static string Align = "align";
            public static string Italic = "italic";
            public static string Bold = "bold";
            public static string Underline = "underline";
            public static string BackgroundColor = "background-color";
            public static string BgColor = "bgColor";
            public static string BorderColor = "border-color";
            public static string Class = "class";
            public static string Border = "border";
            public static string FontColor = "color";
            public static string FontName = "font-name";
            public static string FontStyle = "font-style";
            public static string FontSize = "font-size";
            public static string StrikeThrough = "strike-through";
            public static string TextAlignment = "text-align";
            public static string Left = "left";
            public static string Right = "right";
            public static string Center = "center";
            public static string MarginLeft = "margin-left";
            public static string MarginTop = "margin-top";
            public static string Position = "position";
            public static string Absolute = "absolute";
            public static string Source = "src";
            public static string Alt = "alt";
            public static string TopBorder = "border-top";
            public static string BottomBorder = "border-bottom";
            public static string LeftBorder = "border-left";
            public static string RightBorder = "border-right";
            public static string TopBorderColor = "border-top-color";
            public static string BottomBorderColor = "border-bottom-color";
            public static string LeftBorderColor = "border-left-color";
            public static string RightBorderColor = "border-right-color";
            public static string Rgb = "rgb";
            public static string Solid = "solid";
            public static string Double = "double";
            public static string RowSpan = "ROWSPAN";
            public static string ColumnSpan = "COLSPAN";
            public static string FontWeight = "font-weight";
            public static string TopBorderWidth = "border-top-width";
            public static string BottomBorderWidth = "border-bottom-width";
            public static string LeftBorderWidth = "border-left-width";
            public static string RightBorderWidth = "border-right-width";
            public static string Dashed = "dashed";
            public static string Dotted = "dotted";
            public static string FontFamily = "font-family";
            public static string UnderLine = "underline";
            public static string TextDecoration = "text-decoration";
            public static string VerticalAlign = "vertical-align";
            public static string Top = "top";
            public static string Bottom = "bottom";
            public static string Distriburted = "distributed";
            public static string Justify = "justify";
            public static string General = "general";
            public static string Id = "id";
            public static string Type = "type";
            public static string Value = "value";
            public static string Height = "height";
            public static string Width = "width";
            public static string Scrollbar = "scrollbar";
            public static string Rows = "rows";
            public static string FrameBorder = "frameborder";
            public static string Name = "name";
            public static string Scrolling = "scrolling";
            public static string None = "none";
            public static string Alink = "alink";
            public static string Vlink = "vlink";
            public static string Link = "link";
            public static string Target = "target";
            public static string Href = "href";
            public static string Ahover = "a:hover";
            public static string BorderCollapse = "border-collapse";
            public static string BorderSpacing = "border-spacing";
            public static string EmptyCells = "empty-cells";
            public static string CellSpacing = "cellspacing";
            public static string Style = "style";
            public static string OnClick = "onclick";
            public static string Face = "face";
            public static string Color = "color";
            public static string NoWarp = "nowrap";
            public static string Parent = "_parent";
        }

        private class Operators
        {
            public static string OpenCurlyBrace = "{";
            public static string CloseCurlyBrace = "}";
            public static string Comma = ",";
            public static string Dot = ".";
            public static string Colon = ":";
            public static string SemiColon = ";";
            public static string OpenBrace = "(";
            public static string CloseBrace = ")";
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_styles.Clear();
            m_writer.Close();
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class HtmlSaveOptions
    {
        #region Static Fields
        public static HtmlSaveOptions Default = new HtmlSaveOptions();
        #endregion

        #region Enums
        public enum GetText
        {
            DisplayText,
            Value
        }
        #endregion

        #region Fields
        private string m_imagePath;
        private GetText m_getText = GetText.Value;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSaveOption"/> class.
        /// </summary>
        static HtmlSaveOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSaveOption"/> class.
        /// </summary>
        public HtmlSaveOptions()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        /// <value>The image path.</value>
        public string ImagePath
        {
            get
            {
                return m_imagePath;
            }
            set
            {
                m_imagePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the input text mode
        /// </summary>
        /// <remarks>Useful to convert sheet data based on either DisplayText or Value</remarks>
        public GetText TextMode
        {
            get { return m_getText; }
            set { m_getText = value; }
        }


        #endregion
    }
}
