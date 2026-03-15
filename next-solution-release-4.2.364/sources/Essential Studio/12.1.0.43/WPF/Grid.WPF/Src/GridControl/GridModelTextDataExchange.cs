#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
#if !WinRT
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Collections;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Diagnostics;
using System.Security.Permissions;
using System.Windows.Documents;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Grid
{
#else

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Text;
using Syncfusion.WinRT.Controls.Cells;

namespace Syncfusion.WinRT.Controls.Grid
{
#endif

    /// <summary>
    /// Manages text data exchange for the grid. Lets you copy cell text to a stream or clipboard and recreate the
    /// cell text at a later time.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridModelTextDataExchange : IDisposable
    {
        /// <summary>
        /// Returns grid model.
        /// </summary>
        public GridModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelTextDataExchange"/>.
        /// </summary>
        /// <param name="model">Grid model.</param>
        public GridModelTextDataExchange(GridModel model)
        {
            this.Model = model;
            this.TabDelimiter = "\t";
        }

        /// <summary>
        /// Gets or sets the character that is used for separating columns when importing text file.
        /// </summary>
        public string TabDelimiter
        {
            get;
            set;
        }

        /// <summary>
        /// Copy cellvalue to the buffer
        /// </summary>
        /// <param name="buffer">contain the formatted text for clipboard</param>
        /// <param name="rangeList">currently selected rangeList</param>
        /// <param name="nrowsdone">Number of rows affected</param>
        /// <param name="ncolsdone">Number of columns affected</param>
        /// <param name="gridControl">Reference for gridControl</param>
        /// <returns>returns true after successful copy the selected cell text value to the buffer</returns>
        public virtual bool CopyTextToBuffer(out string buffer, GridRangeInfoList rangeList, out int nrowsdone, out int ncolsdone, bool clear)
        {
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
            int nrows = rowRanges.Cast<GridRangeInfo>().Sum(range => range.Height);
            int ncols = colRanges.Cast<GridRangeInfo>().Sum(range => range.Width);

            var sb = new StringBuilder();
            nrowsdone = 0;
            ncolsdone = 0;

            string tabDelim = "\t";
            if (this.TabDelimiter != string.Empty)
            {
                tabDelim = this.TabDelimiter;
            }
            for (int rowindex = 0; rowindex < rowRanges.Count; rowindex++)
            {
                for (int nrow = rowRanges[rowindex].Top; nrow <= rowRanges[rowindex].Bottom; nrow++)
                {
                    if (nrowsdone > 0)
                    {
                        sb.Append(Environment.NewLine);
                    }

                    ncolsdone = 0;
                    bool firstCol;
                    firstCol = true;

                    for (int colindex = 0; colindex < colRanges.Count; colindex++)
                    {
                        for (int ncol = colRanges[colindex].Left; ncol <= colRanges[colindex].Right; ncol++)
                        {
                            if (!firstCol)
                            {
                                sb.Append(tabDelim);
                            }
                            //  Gets the Cell Value(Not Formated Value)
                            string text = this.GetCopyTextRowCol(nrow, ncol);

                            if (!rangeList.AnyRangeContains(GridRangeInfo.Cell(nrow, ncol)))
                            {
                                ncolsdone++;
                                continue;
                            }

                            text = new StringBuilder(text)
                                .ToString()
                                .Trim();
                            // Append the Cell balue to buffer text
                            sb.Append(text);
                            firstCol = false;
                            CutValueFromCell(nrow, ncol, clear);
                            ncolsdone++;
                        }
                    }
                    nrowsdone++;
                }
            }
            foreach (GridRangeInfo range in rangeList)
            {
                this.Model.InvalidateCell(range);
            }
            this.Model.InvalidateVisual(true);
            buffer = sb.ToString();
            return true;
        }
        /// <summary>
        /// Apply the default value to cell when Cut operation executed
        /// </summary>
        /// <param name="rowindex">Row value for the cell</param>
        /// <param name="colindex">Column value for the cell</param>
        /// <param name="clear">Clear the selected ranges.</param>
        public virtual void CutValueFromCell(int rowindex, int colindex, bool clear)
        {
            if (clear && (this.Model.Options.CopyPasteOption & CopyPaste.CutCell) != CopyPaste.CutCell)
            {
                GridStyleInfo style = this.Model[rowindex, colindex];
                if (!style.ApplyFormattedText(string.Empty))
                {
                    style.ResetCellValue();
                }
            }
        }

#if !SILVERLIGHT 
        /// <summary>
        /// Copy the selected cells in XmlDocument format
        /// </summary>
        /// <param name="buffer">Returns the Xml content of copied ranges</param>
        /// <param name="rangeList">currently selected rangeList</param>
        /// <param name="nrowsdone">Number of rows affected</param>
        /// <param name="ncolsdone">Number of columns affected</param>
        /// <param name="clear">Clear the selected ranges.</param>
        /// <returns>returns true after successful copy the selected cell text value to the buffer</returns>
        public virtual bool CopyXmlToBuffer(out string buffer, GridRangeInfoList rangeList, out int nrowsdone, out int ncolsdone, bool clear)
            {
            buffer = null;
            nrowsdone = 0;
            ncolsdone = 0;
            //GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            //GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols); Unused local variable
            int top = rangeList[0].Top;
            int left = rangeList[0].Left;
            int right = rangeList[0].Right;
            int bottom = rangeList[0].Bottom;
            int rowcount = (bottom - top) + 1;
            int colcount = (right - left) + 1;
            StringBuilder sb = new StringBuilder();
            GridStyleInfo style;

            // Header section " 
            string header = GetXmlHeaderContent();
            sb.Append(header);

            // Styles Section: 
            sb.Append("<Styles>");

            // Append Default Style
            string defaultstyle = GetDefaultXmlStyle();
            sb.Append(defaultstyle);

            // Append Cell Styles
            for (int row = 0; row < rowcount; row++)
                {
                for (int col = 0; col < colcount; col++)
                    {
                       style = Model[row + top, col + left];
                        string cellstyles = CreateXmlCellStyles(style);
                        sb.Append(cellstyles);
                    }
                }
            sb.Append("  </Styles><Worksheet ss:Name=\"Sheet1\">");

            //Append Table
            sb.Append("<Table ss:ExpandedColumnCount=\"" + colcount +
                      "\" ss:ExpandedRowCount=\"" + rowcount + "\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"13.2\">");

            // Append Rows
            for (int row = 0; row < rowcount; row++)
            {
                sb.Append(" <Row>");
                for (int col = 0; col < colcount; col++)
                {
                    //Append Cells
                    style = this.Model[row +top , col +left];
                    string cellcontent = CreateXmlCellContent(style);
                    sb.Append(cellcontent);
                }
                sb.Append(" </Row>");
            }
            sb.Append("</Table>");
            sb.Append("</Worksheet>");
            sb.Append("</Workbook>");
            buffer = sb.ToString();
            // Set the xml formated data in Clipboard.

            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(buffer);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);

            Stream xmlStream = new MemoryStream();
            xmlStream.Write(isoBytes, 0, buffer.Length);
#if !WinRT
            Clipboard.SetData("XML Spreadsheet", xmlStream);
#endif
            return true;

            }



        /// <summary>
        /// Returns the Header content of the Xml Document. 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string GetXmlHeaderContent()
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sb.Append(" <?mso-application progid=\"Excel.Sheet\"?>");
            sb.Append(" <Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
            sb.Append(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            sb.Append(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            sb.Append(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.Append(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the default cell style of the table. 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string GetDefaultXmlStyle()
        {
        var sb = new StringBuilder();
     
        sb.Append("<Style ss:ID=\"Default\" ss:Name=\"Normal\">");
        sb.Append("<Alignment ss:Vertical=\"Bottom\"/>");
        sb.Append("<Borders>");
        sb.Append("<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
        sb.Append("<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
        sb.Append("<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
        sb.Append("<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
        sb.Append("</Borders>");
        sb.Append("<Font/>");
        sb.Append("<Interior/>");
        sb.Append("<NumberFormat/>");
        sb.Append("<Protection/>");
        sb.Append("</Style>");
        return sb.ToString();
        }

        /// <summary>
        /// Returns the Cell style  in Xml format. 
        /// </summary>
        /// <param name="style">Cell Style</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string CreateXmlCellStyles(GridStyleInfo style)
        {
            var sb = new StringBuilder();
            string styleid = "R" + style.RowIndex + "C" + style.ColumnIndex.ToString();
            string backcolor = style.Background.ToString().Substring(3);
            string forecolor = style.Foreground.ToString().Substring(3);
            string fontfamily = style.Font.FontFamily.ToString();
            double size = (style.Font.FontSize/1.2);
            int bold = 0;
            if (style.Font.FontWeight.ToString() =="Bold")
                {
                bold = 1;
                }
            int italic = 0;
            if (style.Font.FontStyle.ToString() == "Italic")
                {
                italic = 1;
                }
            sb.Append("<Style ss:ID=\"s" + styleid + "\">");
            sb.Append("<Interior ss:Color=\"#" + backcolor + "\" ss:Pattern=\"Solid\"/>");
            sb.Append("<Font ss:Color=\"#" + forecolor + "\" ss:Size=\"" + size + "\"  ss:FontName=\"" + fontfamily + "\" ss:Bold=\"" + bold + "\"  ss:Italic=\"" + italic + "\"/>");
            sb.Append("</Style>");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the Cell content of the Table. 
        /// </summary>
        /// <param name="style"> Cell Styles</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string CreateXmlCellContent(GridStyleInfo style )
        {
            var sb = new StringBuilder();
            string type = style.CellValue.GetType().Name;
            string Type = "String";
            if (type == "Int32" || type == "Decimal")
                {
                Type = "Number";
                }
            if (style.CellType == "FormaulCell")
                {
                sb.Append("<Cell  ss:StyleID=\"sR" + style.RowIndex + "C" + style.ColumnIndex + "\" ss:Formula=\"" + style.Text + "\">");
                }
            else
                {
              
                sb.Append("<Cell  ss:StyleID=\"sR" + style.RowIndex + "C" + style.ColumnIndex + "\">");
                }
            sb.Append("<Data ss:Type=\"" + Type + "\">");
            if (style.CellType == "RichText" || style.CellType == "DropDownList" || style.CellType == "ComboBox")
            {
            sb.Append(style.GetFormattedText(style.CellValue));
            }
            else
            {
               sb.Append(style.CellValue.ToString());
            }

            sb.Append("</Data>");
            sb.Append("</Cell>");
            return sb.ToString();
        }

        /// <summary>
        /// Paste the  XML formatted text in Grid Control. 
        /// </summary>
        /// <param name="xDocument"> Xml document contains the copied data. </param>
        /// <param name="rangeList">Current cell range to paste the copied data</param>
        /// <returns></returns>
        /// <remarks></remarks>
#if !WinRT
        public virtual bool PasteXmlFromBuffer(XmlDocument xDocument, GridRangeInfoList rangeList)
        {
            int rowcount = 0;
            int columncount = 0;
            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) == CopyPaste.PasteCell)
            {
                return false;
            }
            if (rangeList == null)
            {
                rangeList = new GridRangeInfoList();
                RowColumnIndex rci = this.Model.CurrentCellState.CellRowColumnIndex;
                rangeList.Add(GridRangeInfo.Cell(rci.RowIndex, rci.ColumnIndex));
            }
            if (!this.Model.CutPaste.PasteBoundCheck(rangeList))
            {
                return false;
            }
            if (Clipboard.ContainsData("XML Spreadsheet"))
            {
                // Get  the Table node from the XmlDocument. 
                var tablenodes = xDocument.GetElementsByTagName("Table");
                // Get All the Style Nodes from the XML Document
                var styles = xDocument.GetElementsByTagName("Style");
                // Get all the Row Nodes From the XML Document.
                var rownodes = xDocument.GetElementsByTagName("Row");
                if (tablenodes.Count > 0)
                {
                    // Get he Row Count ans Column Count.
                    rowcount = Convert.ToInt32(tablenodes[0].Attributes["ss:ExpandedRowCount"].Value);
                    columncount = Convert.ToInt32(tablenodes[0].Attributes["ss:ExpandedColumnCount"].Value);
                }
                for (int rowindex = 0; rowindex < rowcount; rowindex++)
                {
                    var rownode = rownodes.Item(rowindex);
                    if (rownode != null)
                        for (int colindex = 0; colindex < rownode.ChildNodes.Count; colindex++)
                        {
                            var cell = rownode.ChildNodes[colindex];
                            string data = rownode.ChildNodes[colindex].InnerText;
                            GridStyleInfo style = this.Model[rowindex + rangeList[0].Top, colindex + rangeList[0].Left];
                            style.ApplyFormattedText(data, GridCellBaseTextInfo.PasteText);
                            if (cell.Attributes != null)
                                for (int index = 0; index<cell.Attributes.Count; index++)
                                {
                                    string attribute = cell.Attributes[index].Name;
                                    // If The Cell is Formula Cells
                                    if (attribute == "ss:Formula")
                                    {
                                        style.Text = cell.Attributes["ss:Formula"].Value;
                                        style.CellType = "FormulaCell";
                                    }
                                    // Gets the Style Attribute of the Cells
                                    if (attribute == "ss:StyleID")
                                    {
                                        for (int styleindex = 0; styleindex < styles.Count; styleindex++)
                                        {
                                        if (cell.Attributes[index].Value == styles[styleindex].Attributes["ss:ID"].Value)
                                            {
                                                var cellstyles = styles[styleindex];
                                            // Gets All the basic styles like Fint style, sizem foreground ,background from the Xml Document.
                                                for (int attributeindex = 0;
                                                     attributeindex < cellstyles.ChildNodes.Count;
                                                     attributeindex++)
                                                {
                                                    var innerstyle = cellstyles.ChildNodes[attributeindex];
                                                    if (innerstyle.LocalName == "Font")
                                                    {
                                                        for (int fontstyle = 0;
                                                             fontstyle < innerstyle.Attributes.Count;
                                                             fontstyle++)
                                                        {
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:FontName")
                                                            {
                                                                style.Font.FontFamily =
                                                                    new FontFamily(
                                                                        innerstyle.Attributes[fontstyle].Value);
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Size")
                                                            {
                                                                style.Font.FontSize =
                                                                    (Convert.ToDouble(
                                                                        innerstyle.Attributes[fontstyle].Value)*1.2);
                                                            }

                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Color")
                                                            {
                                                                string forecolor = innerstyle.Attributes[fontstyle].Value;
                                                                var conv = new BrushConverter();
                                                                var brush =conv.ConvertFromString("#FF" + forecolor.Substring(1)) as SolidColorBrush;
                                                                if (brush != null)
                                                                {
                                                                    style.Foreground = brush;
                                                                }
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Bold")
                                                            {
                                                                if (innerstyle.Attributes[fontstyle].Value == "1")
                                                                {
                                                                    style.Font.FontWeight = FontWeights.Bold;
                                                                }
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Italic")
                                                            {
                                                                if (innerstyle.Attributes[fontstyle].Value == "1")
                                                                {
                                                                    style.Font.FontStyle = FontStyles.Italic;
                                                                }
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Underline")
                                                            {
                                                                style.Font.TextDecorations = TextDecorations.Underline;
                                                            }
                                                        }
                                                    }
                                                    if (innerstyle.LocalName == "Interior")
                                                    {
                                                        for (int fontstyle = 0;
                                                             fontstyle < innerstyle.Attributes.Count;
                                                             fontstyle++)
                                                        {
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Color")
                                                            {
                                                                string color = innerstyle.Attributes[fontstyle].Value;
                                                                var conv = new BrushConverter();
                                                                var brush =
                                                                    conv.ConvertFromString("#FF" + color.Substring(1)) as SolidColorBrush;
                                                                if (brush != null)
                                                                {
                                                                    style.Background = brush;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                        }
                }
                //Consolidates the pasted ranges to invalidate and add in selected ranges
                GridRangeInfo pastedrange = GridRangeInfo.Cells(rangeList[0].Top, rangeList[0].Left,
                                                                rangeList[0].Top + (rowcount - 1),
                                                                rangeList[0].Left + (columncount - 1));
                this.Model.InvalidateCell(pastedrange);
                this.Model.SelectedRanges.Clear();
                this.Model.SelectedRanges.Add(pastedrange);
            }
            this.Model.InvalidateVisual(true);

            return true;
        }
#endif 
#endif


        /// <summary>
        /// Paste Text From The Buffer
        /// </summary>
        /// <param name="buffer">For Holding final clipboard value</param>
        /// <param name="rangeList">conatin the selected range</param>
        /// <param name="gridControl">reference for gridControl</param>
        /// <returns>return true after fill the cell value from the buffer</returns>
        public virtual bool PasteTextFromBuffer(string buffer, GridRangeInfoList rangeList)
        {
            var baseString = buffer;
            if (buffer.Equals(string.Empty))
            {
                return false;
            }

            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) == CopyPaste.PasteCell)
            {
                return false;
            }
            if (rangeList == null)
                {
                rangeList = new GridRangeInfoList();
                RowColumnIndex rci = this.Model.CurrentCellState.CellRowColumnIndex;
                rangeList.Add(GridRangeInfo.Cell(rci.RowIndex, rci.ColumnIndex));
                }
            if (!this.Model.CutPaste.PasteBoundCheck(rangeList))
            {
                return false;
            }

            string s = string.Empty;
            int rowIndex, colIndex;

            string tabDelim = "\t";
            if (this.TabDelimiter != string.Empty)
            {
                tabDelim = this.TabDelimiter;
            }

            bool canceled = true;
            rowIndex = rangeList[0].Top;
            colIndex = rangeList[0].Left;

            rowIndex = rowIndex == 0 ? 1 : rowIndex;

            int lastCol = colIndex;
            int size = buffer.Length;

            StringReader sr = new StringReader(buffer);
            // int rangelColIndex = 0; The variable is assigned but it is never used.
            string bufferCopy = buffer;
            buffer = sr.ReadLine();

            int rowCount = 0;
       
            string temp = bufferCopy;
            StringReader bf = new StringReader(bufferCopy);
            while (temp != null)
            {
                temp = bf.ReadLine();
                if (temp != null)
                    rowCount++;
            }
            var colCollect = baseString.Split(new string[] { tabDelim }, StringSplitOptions.None);
            foreach (var item in colCollect)
            {
                if (item.EndsWith("\r\n\""))
                    s += item.Remove(item.Length - 3) + "\"\t";
                else
                    s += item + "\t";
            }
            s = s.Remove(s.Length - 1);
            var rowCollect = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (var item in rowCollect)
            {
                buffer = item;
                size = buffer.Length;
                for (int index = 0, last = 0; index <= size; index++)
                {
                    //// Check for a delimiter.
                    if (size == 0)
                    {
                        if (this.Model[rowIndex, colIndex].CellType == "Static" || this.Model[rowIndex, colIndex].ReadOnly || this.Model[rowIndex, colIndex].CellType == "ReadOnly" || this.Model[rowIndex, colIndex].CellType == "static")
                        {
                            continue;

                        }
                        this.PasteTextRowCol(rowIndex, colIndex, string.Empty);
                    }
                    else
                    {
                        if (this.Model[rowIndex, colIndex].CellType == "Static" || this.Model[rowIndex, colIndex].ReadOnly || this.Model[rowIndex, colIndex].CellType == "ReadOnly" || this.Model[rowIndex, colIndex].CellType == "static")
                        {
                        continue;
                            
                        }
                        bool isDelimiter = (("\r\n".IndexOf(buffer[index]) != -1 || tabDelim[0] == buffer[index]) && (buffer[index] != '\n'));
                        if (index == size - 1 || isDelimiter)
                        {
                            //// End of a string found, copy value to cell.
                            if ("\r\n".IndexOf(buffer[index]) != -1)
                            {
                                if (buffer[index] == '\r' && buffer[index + 1] == '\n')
                                {
                                    index++;
                                    colIndex = rangeList[0].Left;
                                    last = index + 1;
                                    continue;
                                }
                            }

                            if (rowIndex <= this.Model.RowCount && colIndex <= this.Model.ColumnCount)
                            {
                                s = isDelimiter
                                        ? (index != last ? buffer.Substring(last, index - last) : String.Empty)
                                        : buffer.Substring(last);

                                //Remove double quotes start and end when new line is used in cell
                                if (s.StartsWith("\"") && s.EndsWith("\"") && s.Length > 1)
                                    s = s.Substring(1, s.Length - 2);
                                else if (s.StartsWith("\""))
                                {
                                    if (s.Length > 1)
                                        s = s.Substring(1, s.Length - 1);
                                    else
                                    {
                                        rowIndex--;
                                        break;
                                    }
                                }
                                s = s.Replace("\"\"", "\"");

                                //// Give the control the chance to validate
                                //// and change the pasted text.
                                canceled = !this.PasteTextRowCol(rowIndex, colIndex, s);
                            }

                            if (canceled || index == size - 1)
                            {
                                break;
                            }
                            else if ("\r\n".IndexOf(buffer[index]) != -1)
                            {
                                rowIndex++;
                                colIndex = rangeList[0].Left;
                                if (buffer[index] == '\r' && buffer[index + 1] == '\n')
                                {
                                    index++;
                                }

                                //// Abort parsing the string if next char
                                //// is an end-of-string.
                                if (index == size - 1)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                colIndex++;
                            }

                            last = index + 1;
                            lastCol = Math.Max(colIndex, lastCol);
                        }
                    }
                }
                rowIndex++;
               colIndex = rangeList[0].Left;
            }

            var list = rangeList.Clone() as GridRangeInfoList;
            Model.Selections.Clear();
            if (list.Count > 0)
            {
            Model.InvalidateCell(GridRangeInfo.Cells(list[0].Top, list[0].Left, rowIndex - 1, lastCol));
            Model.Selections.Add(GridRangeInfo.Cells(list[0].Top, list[0].Left, rowIndex - 1, lastCol));
            }
            this.Model.InvalidateVisual(true);
            if (Model.CutPaste.CutFlag)
            {
                try
                {
#if (!SILVERLIGHT &&  !WinRT)
                    System.Windows.Clipboard.SetDataObject(string.Empty);
#endif
#if SILVERLIGHT
                   System.Windows.Clipboard.SetText(string.Empty);
#endif

#if WinRT
                    Clipboard.SetContent(null);
#endif
                }
                finally { }
                Model.CutPaste.CutFlag = false;
            }

            return !canceled;
        }

        /// <summary>
        /// Calculates the Buffer dimentison as how many rows and cols does it required to paste.
        /// </summary>
        /// <param name="psz"></param>
        /// <param name="nRows"></param>
        /// <param name="nCols"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool CalcBufferDimension(string psz, out int nRows, out int nCols)
        {
            int nRowsDone = 0, nColsDone = 0;

            int colIndex = 0;
            bool canceled = false;

            //// (New line will start a new row, tab delimiter will
            //// move to the next column).

            string sTabDelim = "\t";
            if (this.TabDelimiter != null && this.TabDelimiter.Length > 0)
            {
                sTabDelim = this.TabDelimiter;
            }

            int size = psz.Length;

            for (int nIndex = 0, nLast = 0; !canceled && nIndex < size; nIndex++)
            {
                //// Check for a delimiter
                if (nIndex == size - 1 || "\r\n".IndexOf(psz[nIndex]) != -1 || sTabDelim[0] == psz[nIndex])
                {
                    //// Abort parsing the string if next char
                    //// is an end-of-string.
                    if (nIndex == size - 1)
                    {
                        if (nIndex > nLast)
                        {
                            colIndex++;
                        }

                        break;
                    }
                    else if ("\r\n".IndexOf(psz[nIndex]) != -1)
                    {
                        //// Now that the value has been copied to the cell,
                        //// let's check if we should jump to a new row.

                        //// Yes, I found an end of line.
                        //// Let's increase rowIndex and reset colIndex to first column.
                        nRowsDone++;
                        colIndex = 0;

                        if (psz[nIndex] == '\r' && psz[nIndex + 1] == '\n')
                        {
                            nIndex++;
                        }

                        // Abort parsing the string if next char
                        // is an end-of-string.
                        if (nIndex == size - 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //// Move to next column.
                        colIndex++;
                    }

                    //// Save index where the next cell's value starts.
                    nLast = nIndex + 1;

                    nColsDone = Math.Max(nColsDone, colIndex + 1);
                }
            }

            if (colIndex > 0)
            {
                nRowsDone++;
            }

            nRows = nRowsDone;
            nCols = Math.Max(nColsDone, 1);

            return true;
        }


        /// <summary>
        /// Pastes the text value to the cell
        /// </summary>
        /// <param name="rowIndex">Row value for the cell</param>
        /// <param name="colIndex">Column value for the cell</param>
        /// <param name="text">New String value for the cell</param>
        /// <returns>successful paste returns true</returns>
        public  virtual bool PasteTextRowCol(int rowIndex, int colIndex, string text)
        {
            bool state = false;

            // Newly added code to affect the underlying business objects on pasting text from Clipboard
            foreach (GridControlBase grid in Model.Views)
            {
                if (grid != null)
                {
                    GridStyleInfo style = this.Model[rowIndex, colIndex];
                    state = style.CellType == "DropDownList" || style.CellType == "ComboBox"
                                ? style.ApplyFormattedText(text)
                                : style.ApplyText(text);
                    
                    this.Model.InvalidateCell(new RowColumnIndex(rowIndex,colIndex)); // grid.CurrentCell.RowIndex,grid.CurrentCell.ColumnIndex));
                }
            }
            return state;
        }

        /// <summary>
        /// Get the value of the cell
        /// </summary>
        /// <param name="rowIndex">contain the row value</param>
        /// <param name="colIndex">contain the column value</param>
        /// <param name="gridControl">Reference for gridControl</param>
        /// <returns>the value of the cell</returns>
        public virtual string GetCopyTextRowCol(int rowIndex, int colIndex)
        {
            GridStyleInfo style = this.Model[rowIndex, colIndex];
            if (style.CellType == "RichText")
            {
                return style.GetFormattedText(style.CellValue);
            }
            return style != null
                       ? (style.CellType == "DropDownList" || style.CellType == "ComboBox"
                              ? style.GetFormattedText(style.CellValue, GridCellBaseTextInfo.CopyText)
                              : style.CellValue!=null? style.CellValue.ToString(): string.Empty)
                       : string.Empty;
        }

        public void Dispose()
        {
            
        }
    }
}
