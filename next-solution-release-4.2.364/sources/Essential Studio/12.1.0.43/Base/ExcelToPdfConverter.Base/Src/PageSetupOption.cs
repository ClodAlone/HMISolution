#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.Pdf.Graphics;
using System.Collections.Generic;
using Syncfusion.XlsIO;

namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// 
    /// </summary>
    internal class PageSetupOption
    {
        #region Fields
        private string[] m_rowCollections;
        private string[] m_colCollections;
        private WorksheetImpl m_sheet;
        private float m_titleRowHeight=0;
        private float m_titleColWidth=0;
        private List<int> m_rowIndexes;
        private List<int> m_colIndexes;
        private List<IRange> m_printAreas;
        private List<int> m_verticalIds;
        private List<int> m_horizontalIds;
        private IRange m_usedRange;
        private string[] m_skippedAreas = new string[] { "#REF" };
        internal const char sheetToken = '!';
        internal const char separatorToken = ',';

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PageSetupOption"/> class.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        public PageSetupOption(WorksheetImpl sheet, IRange actualUsedRange)
        {
            m_usedRange = actualUsedRange;
            m_sheet = sheet;
            m_verticalIds = new List<int>();
            m_horizontalIds = new List<int>();
            if (HasPrintTitleRows)
                ParseTitleRows();

            if (HasPrintTitleColumns)
                ParseTitleColumns();

            if (HasPrintArea)
                ParsePrintArea();

            if (HasVerticalBreak)            
                ParseVerticalBreaks();
            

            if(HasHorizontalBreak)
                ParseHorizontalBreaks();
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public PageSetupOption()
        {
        }

        private void ParseHorizontalBreaks()
        {
            if (m_sheet.HPageBreaks.Count == 0)
                return;
            
            for (int hIndex = 0; hIndex < m_sheet.HPageBreaks.Count; hIndex++)
            {
                m_horizontalIds.Add(m_sheet.HPageBreaks[hIndex].Location.Row - 1);
            }            
        }

        private void ParseVerticalBreaks()
        {
            if (m_sheet.VPageBreaks.Count == 0)
                return;
            
            for (int hIndex = 0; hIndex < m_sheet.VPageBreaks.Count; hIndex++)
            {
                int iColumn = m_sheet.VPageBreaks[hIndex].Location.Column - 1;
                if (!m_verticalIds.Contains(iColumn))
                    m_verticalIds.Add(iColumn);
               
            }   
        }
        #endregion

        #region Implementation properties
        /// <summary>
        /// Gets the print title first row.
        /// </summary>
        /// <value>The print title first row.</value>
        internal int PrintTitleFirstRow
        {
            get
            {
                return m_rowCollections!= null?
                    (int)Convert.ToInt16(m_rowCollections[0].Remove(0, 1))
                    : 0;
            }
        }
        /// <summary>
        /// Gets the print title first column.
        /// </summary>
        /// <value>The print title first column.</value>
        internal int PrintTitleFirstColumn
        {
            get
            {
                return m_colCollections != null?
                (int)Convert.ToInt16((Convert.ToInt16(m_colCollections[0].Remove(0, 1).ToCharArray()[0])-65)+1 )
                :0;
            }
        }
        /// <summary>
        /// Gets the print title last row.
        /// </summary>
        /// <value>The print title last row.</value>
        internal int PrintTitleLastRow
        {
            get
            {
                return m_rowCollections !=null ?
                    (int)Convert.ToInt16(m_rowCollections[1].Remove(0, 1))
                    :0;
            }
        }
        /// <summary>
        /// Gets the print title last column.
        /// </summary>
        /// <value>The print title last column.</value>
        internal int PrintTitleLastColumn
        {
            get
            {
                return m_colCollections != null ?
                    (int)Convert.ToInt16((Convert.ToInt16(m_colCollections[1].Remove(0, 1).ToCharArray()[0])-65)+1)
                    :0;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has print title rows.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has print title rows; otherwise, <c>false</c>.
        /// </value>
        internal bool HasPrintTitleRows
        {
            get
            {
                return m_sheet.PageSetup.PrintTitleRows != null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has print title columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has print title columns; otherwise, <c>false</c>.
        /// </value>
        internal bool HasPrintTitleColumns
        {
            get
            {
                return m_sheet.PageSetup.PrintTitleColumns != null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has print area.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has print area; otherwise, <c>false</c>.
        /// </value>
        internal bool HasPrintArea
        {
            get
            {
                return m_sheet.PageSetup.PrintArea != null;
            }
        }
        internal bool HasVerticalBreak
        {
            get
            {
                return m_sheet.VPageBreaks.Count != 0;
            }
        }
        internal bool HasHorizontalBreak
        {
            get
            {
                return m_sheet.HPageBreaks.Count != 0;
            }
        }

        /// <summary>
        /// Gets the height of the title row.
        /// </summary>
        /// <value>The height of the title row.</value>
        internal float TitleRowHeight
        {
            get
            {
                return m_titleRowHeight==0 ?
                    GetRowHeight(new PdfUnitConvertor(),new ItemSizeHelper(this.m_sheet.GetRowHeightInPixels))
                    : m_titleRowHeight;
            }
        }

        /// <summary>
        /// Gets the width of the title column.
        /// </summary>
        /// <value>The width of the title column.</value>
        internal float TitleColumnWidth
        {
            get
            {
                return m_titleColWidth==0 ?
                    GetColumnWidth(new PdfUnitConvertor(),new ItemSizeHelper(this.m_sheet.GetColumnWidthInPixels))
                    :m_titleColWidth;
            }
        }
        /// <summary>
        /// Gets the row indexes.
        /// </summary>
        internal List<int> RowIndexes
        {
            get
            {
                if (m_rowIndexes == null)
                    m_rowIndexes = new List<int>();

                return m_rowIndexes;
            }
        }
        /// <summary>
        /// Gets the column indexes.
        /// </summary>
        internal List<int> ColumnIndexes
        {
            get
            {
                if (m_colIndexes == null)
                    m_colIndexes = new List<int>();

                return m_colIndexes;
            }
        }
        /// <summary>
        /// Gets the print areas.
        /// </summary>
        /// <value>The print areas.</value>
        internal IRange[] PrintAreas
        {
            get
            {
                if (m_printAreas == null)
                    m_printAreas = new List<IRange>();

                return m_printAreas.ToArray();
            }
        }

        /// <summary>
        /// Gets the worksheet.
        /// </summary>
        internal WorksheetImpl Worksheet
        {
            get
            {
                return m_sheet;
            }
        }

        /// <summary>
        /// Gets the page setup of the worksheet.
        /// </summary>
        internal IPageSetup PageSetup
        {
            get
            {
                return m_sheet.PageSetup;
            }
        }

        #endregion

        #region ParseMethods
        /// <summary>
        /// Parses the title rows.
        /// </summary>
        private void ParseTitleRows()
        {
            string rowTitle = m_sheet.PageSetup.PrintTitleRows;
            if (rowTitle == string.Empty)
                return;

            m_rowCollections = GetSplittedTitle(rowTitle);
        }
        /// <summary>
        /// Parses the title columns.
        /// </summary>
        private void ParseTitleColumns()
        {
            string columnTitle = m_sheet.PageSetup.PrintTitleColumns;
            if (columnTitle == string.Empty)
                return;

            m_colCollections = GetSplittedTitle(columnTitle);
        }
        /// <summary>
        /// Parses the print area.
        /// </summary>
        private void ParsePrintArea()
        {
            List<string> splitValues = new List<string>();
            splitValues = GetSplittedPrintArea(m_sheet.PageSetup.PrintArea);
            m_printAreas = new List<IRange>();
            foreach (string split in splitValues)
            {
                string areaRange = split.Split('!')[1];
                m_printAreas.Add(UpdateRange(areaRange));
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Gets the splitted title.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string[] GetSplittedTitle(string value)
        {
            return value.Split('!')[1].Split(':');
        }
        /// <summary>
        /// Gets the splitted print area.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private List<string> GetSplittedPrintArea(string value)
        {
            int startIndex = 0;
            int index = 0;
            int commaIndex = 0;
            int count;
            List<string> printArea = new List<string>();
            printArea.Add(value);
            while (value.Length > startIndex)
            {
                count = printArea.Count - 1;
                index = printArea[count].IndexOf(sheetToken, startIndex);
                commaIndex = printArea[count].IndexOf(separatorToken, index);
                if (commaIndex == -1)
                {
                    break;
                }
                else
                {
                    string temp = printArea[count];
                    printArea[count] = string.Empty;
                    printArea[count] = temp.Substring(startIndex, (commaIndex) - startIndex);
                    printArea.Add(temp.Substring(commaIndex + 1, (temp.Length - 1) - commaIndex));                    
                }
            }
            return printArea;
            
        }
        /// <summary>
        /// Gets the height of the row.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="rowHeightGetter">The row height getter.</param>
        /// <returns></returns>
        private float GetRowHeight(PdfUnitConvertor converter, ItemSizeHelper rowHeightGetter)
        {
           m_titleRowHeight = 0;

            if (!HasPrintTitleRows)
                return 0;

            for (int i = PrintTitleFirstRow; i <= PrintTitleLastRow; i++)
            {
                m_titleRowHeight += converter.ConvertFromPixels(rowHeightGetter.GetSize(i), PdfGraphicsUnit.Point);
            }
            return m_titleRowHeight;
        }
        /// <summary>
        /// Gets the width of the column.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="columnWidthGetter">The column width getter.</param>
        /// <returns></returns>
        private float GetColumnWidth(PdfUnitConvertor converter, ItemSizeHelper columnWidthGetter)
        {
            m_titleColWidth = 0;

            if (!HasPrintTitleColumns)
                return m_titleColWidth;

            for (int i = PrintTitleFirstColumn; i <= PrintTitleLastColumn; i++)
            {
                m_titleColWidth += converter.ConvertFromPixels(columnWidthGetter.GetSize(i), PdfGraphicsUnit.Point);
            }
            return m_titleColWidth;
        }
        /// <summary>
        /// Updates the range.
        /// </summary>
        /// <param name="split">The split.</param>
        /// <returns></returns>
        private IRange UpdateRange(string split)
        {
            if(Array.IndexOf(m_skippedAreas,split) >=0)
                return m_usedRange;

            IRange range = m_sheet[split];

            if (range.Row == 1 && range.LastRow == m_sheet.Workbook.MaxRowCount)
                range = m_sheet[m_usedRange.Row, range.Column, m_usedRange.LastRow, range.LastColumn];
            else if(range.Column==1 && range.LastColumn== m_sheet.Workbook.MaxColumnCount)
               range = m_sheet[range.Row, m_usedRange.Column, range.LastRow, m_usedRange.LastColumn];

            return range;
        }
        /// <summary>
        /// Checks the row bounds.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal bool CheckRowBounds(int startIndex,int endIndex)
        {
            return ((PrintTitleFirstRow >= startIndex && PrintTitleFirstRow <= endIndex) || (PrintTitleLastRow <= endIndex && PrintTitleLastRow >= startIndex));
        }
        /// <summary>
        /// Checks the column bounds.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal bool CheckColumnBounds(int startIndex,int endIndex)
        {
            return ((PrintTitleFirstColumn >= startIndex && PrintTitleFirstColumn <= endIndex) || (PrintTitleLastColumn <= endIndex && PrintTitleLastColumn >= startIndex));
        }
        /// <summary>
        /// Gets the break ranges.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="actualUsedRange">The actual used range.</param>
        /// <returns></returns>
        internal IRange[] GetBreakRanges(LayoutOptions option,IRange[] actualUsedRange)
        {                        
            List<IRange> finalranges = new List<IRange>();

            if (option == LayoutOptions.FitSheetOnOnePage)
                return actualUsedRange;

            foreach (IRange usedRange in actualUsedRange)
            {
                List<IRange> ranges = new List<IRange>();               
                ranges.Add(usedRange);

                if (!HasVerticalBreak && !HasHorizontalBreak)
                    return actualUsedRange;

                int firstRow = usedRange.Row;
                int firstColumn = usedRange.Column;
                int lastRow = usedRange.LastRow;
                int lastColumn = usedRange.LastColumn;
                switch (option)
                {
                    case LayoutOptions.NoScaling:
                        if (m_sheet.PageSetup.Order == ExcelOrder.DownThenOver)
                        {
                            GetVerticalBreaks(ranges, lastColumn);
                            GetHorizontalBreaks(ranges, lastRow);
                        }
                        else
                        {
                            GetHorizontalBreaks(ranges, lastRow);
                            GetVerticalBreaks(ranges, lastColumn);                            
                        }
                        break;
                    case LayoutOptions.FitAllRowsOnOnePage:
                        GetVerticalBreaks(ranges, lastColumn);
                        break;
                    case LayoutOptions.FitAllColumnsOnOnePage:
                        GetHorizontalBreaks(ranges, lastRow);
                        break;
                    case LayoutOptions.CustomScaling:
                        if (m_sheet.PageSetup.Order == ExcelOrder.DownThenOver)
                        {
                            GetVerticalBreaks(ranges, lastColumn);
                            GetHorizontalBreaks(ranges, lastRow);
                        }
                        else
                        {
                            GetHorizontalBreaks(ranges, lastRow);
                            GetVerticalBreaks(ranges, lastColumn); 
                        }
                        break;
                }
                finalranges.AddRange(ranges.ToArray());
            }
            return finalranges.ToArray();
        }
        /// <summary>
        /// Gets the horizontal breaks.
        /// </summary>
        /// <param name="ranges">The ranges.</param>
        /// <param name="finalLastRow">The final last row.</param>
        private void GetHorizontalBreaks(List<IRange> ranges,int finalLastRow)
        {
            IRange[] copyRange = new IRange[ranges.Count];
            ranges.CopyTo(copyRange);
            ranges.Clear();
            for (int index = 0; index < copyRange.Length; index++)
            {
                bool check = false;
                int firstRow = copyRange[index].Row;
                int lastRow = copyRange[index].LastRow;
                int firstColumn = copyRange[index].Column;
                int lastColumn = copyRange[index].LastColumn;
                foreach (int hID in m_horizontalIds)
                {
                    IRange veriRange;
                    if (hID >= firstRow && hID <= lastRow)
                    {
                        if (check)
                        {
                            firstRow = ranges[ranges.Count - 1].LastRow + 1;
                        }

                        veriRange = m_sheet[firstRow, firstColumn, hID, lastColumn];
                        ranges.Add(veriRange);
                        check = true;
                    }
                }
                if (ranges.Count != 0 && ranges[ranges.Count - 1].LastRow < lastRow)
                {
                    IRange lastRange = ranges[ranges.Count - 1];
                    ranges.Add(m_sheet[lastRange.LastRow + 1, lastRange.Column, lastRow, lastRange.LastColumn]);
                }
                else if (!CheckRange(ranges, copyRange[index])) 
                    ranges.Add(copyRange[index]);
            }
            
        }
        /// <summary>
        /// Gets the vertical breaks.
        /// </summary>
        /// <param name="veriRange">The veri range.</param>
        /// <param name="finalColumn">The final column.</param>
        private void GetVerticalBreaks(List<IRange> veriRange,int finalColumn)
        {
           IRange[] copyRange=new IRange[veriRange.Count];
           veriRange.CopyTo(copyRange);
           veriRange.Clear();
           for (int i = 0; i < copyRange.Length; i++)
           {
               bool check = false;
               int firstColumn= copyRange[i].Column;
               int lastColumn= copyRange[i].LastColumn;
               foreach(int vId in m_verticalIds)
               {
                   if (vId >= firstColumn && vId <= lastColumn)
                   {
                       if (check)
                           firstColumn = veriRange[veriRange.Count - 1].LastColumn + 1;

                       IRange range = m_sheet.Range[copyRange[i].Row, firstColumn, copyRange[i].LastRow, vId];
                       veriRange.Add(range);
                       check = true;
                   }
               }

               if (veriRange.Count != 0 && veriRange[veriRange.Count - 1].LastColumn < finalColumn
                 && m_sheet.Range[copyRange[i].Row, veriRange[veriRange.Count - 1].LastColumn + 1, copyRange[i].LastRow, finalColumn].IsBlank)
               {
                   IRange leftRange = m_sheet[copyRange[i].Row, veriRange[veriRange.Count - 1].LastColumn + 1, copyRange[i].LastRow, finalColumn];
                   veriRange.Add(leftRange);
               }
               else if ((!CheckRange(veriRange, copyRange[i]) && m_verticalIds.Count==0)||veriRange.Count ==0)
                   veriRange.Add(copyRange[i]);
           }            
        }
        private bool CheckRange(List<IRange> veriRange, IRange range)
        {
            foreach (IRange veri in veriRange)
            {
                if (veri.AddressLocal == range.AddressLocal)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
