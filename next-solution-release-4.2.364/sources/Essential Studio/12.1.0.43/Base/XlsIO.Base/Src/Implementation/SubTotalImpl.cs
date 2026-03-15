#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Text;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation
{
    internal class SubTotalImpl
    {
        #region ClassMembers
        /// <summary>
        /// Incidates to replace existing subtotal
        /// </summary>
        private bool m_replace;
        /// <summary>
        /// Indicates to insert pageBreaks
        /// </summary>
        private bool m_pageBreaks;
        /// <summary>
        /// Indicates to insert GrandTotal to below data
        /// </summary>
        private bool m_summaryBelowData;
        /// <summary>
        /// Indicates to group rows
        /// </summary>
        private bool m_groupRows;
        /// <summary>
        /// ConsolidationFunction enumeration
        /// </summary>
        private ConsolidationFunction consolidationFunction;
        /// <summary>
        /// TotalList of columns to be added
        /// </summary>
        private int[] m_totalList;
        /// <summary>
        /// Current row
        /// </summary>
        private int irow;
        /// <summary>
        /// Zero based index of column
        /// </summary>
        private int m_groupBy;
        /// <summary>
        /// Total colulmn Count
        /// </summary>
        private int columnCount;
        /// <summary>
        /// Record Table
        /// </summary>
        private RecordTable m_recordTable;
        /// <summary>
        /// Column Names
        /// </summary>
        private string[] columnName;
        /// <summary>
        /// TotalString
        /// </summary>
        private string total = "Total";
        /// <summary>
        /// GrandTotalString
        /// </summary>
        private string grandTotal = "Grand Total";
        /// <summary>
        /// Height of the row
        /// </summary>
        private int m_height;
        /// <summary>
        /// ExcelVersion
        /// </summary>
        private ExcelVersion m_version;
        /// <summary>
        /// Bloack size of the rows
        /// </summary>
        private int blockSize;
        /// <summary>
        /// worksheet
        /// </summary>
        private WorksheetImpl m_worksheet;
        /// <summary>
        /// Horizontal PageBreaks collection
        /// </summary>
        private HPageBreaksCollection m_hPageBreaks;
        /// <summary>
        /// First row index
        /// </summary>
        private int m_firstRow;
        /// <summary>
        /// First column index
        /// </summary>
        private int m_firstColumn;
        /// <summary>
        /// Last row index
        /// </summary>
        private int m_lastRow;
        /// <summary>
        /// Las column index
        /// </summary>
        private int m_lastColumn;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a SubTotal Object
        /// </summary>
        /// <param name="worksheet">WorkSheet</param>
        internal SubTotalImpl(WorksheetImpl worksheet)
        {
            this.m_groupRows = true;

            m_worksheet = worksheet;
            m_recordTable = worksheet.CellRecords.Table;

            m_height = worksheet.AppImplementation.StandardHeightInRowUnits;
            blockSize = worksheet.AppImplementation.RowStorageAllocationBlockSize;
            m_version = worksheet.Version;
            this.m_hPageBreaks = worksheet.HPageBreaks as HPageBreaksCollection;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Creates SubTotal for the Specified Ranges
        /// </summary>
        /// <param name="firstRow">FirstRow</param>
        /// <param name="firstColumn">FirstColumn</param>
        /// <param name="lastRow">LastRow</param>
        /// <param name="lastColumn">LastColumn</param>
        /// <param name="groupBy">GroupBy</param>
        /// <param name="function">ConsolidationFunction</param>
        /// <param name="totalList">TotalList</param>
        /// <param name="replace">Replace</param>
        /// <param name="pageBreaks">InsertPageBreaks</param>
        /// <param name="summaryBelowData">SummaryBelowData</param>
        internal void CalculateSubTotal(int firstRow, int firstColumn, int lastRow, int lastColumn, int groupBy, ConsolidationFunction function, int[] totalList, bool replace, bool pageBreaks, bool summaryBelowData)
        {
            this.m_firstRow = firstRow;
            this.m_firstColumn = firstColumn;
            this.m_lastRow = lastRow;
            this.m_lastColumn = lastColumn;

            this.m_groupBy = groupBy;
            this.consolidationFunction = function;
            this.m_totalList = totalList;
            this.m_replace = replace;
            this.m_pageBreaks = pageBreaks;
            this.m_summaryBelowData = summaryBelowData;
            this.total = GetEnumerationString(function);
            this.grandTotal = "Grand " + this.total;
            this.columnCount = firstColumn + groupBy;
            bool containsGroup = false;
            for (int i = totalList.Length - 1; i >= 0; i--)
            {
                if (totalList[i] == groupBy)
                {
                    containsGroup = true;
                }
            }
            if (containsGroup)
            {
                containsGroup = false;
                for (int k = groupBy - 1; k >= 0; k--)
                {
                    containsGroup = true;
                    for (int m = totalList.Length - 1; m >= 0; m--)
                    {
                        if (totalList[m] == k)
                        {
                            containsGroup = false;
                            break;
                        }
                    }
                    if (containsGroup)
                    {
                        this.columnCount = firstColumn + k;
                        break;
                    }
                }
                if (!containsGroup)
                {
                    this.columnCount = firstColumn;
                    this.m_worksheet.InsertColumn(firstColumn + 1);
                    firstColumn++;
                    lastColumn++;
                }
            }
            this.columnName = new string[totalList.Length];
            for (int j = 0; j < totalList.Length; j++)
            {
                this.columnName[j] = RangeImpl.GetColumnName(totalList[j] + firstColumn + 1);
                totalList[j] += firstColumn;
            }
            this.irow = firstRow;
            this.irow = firstRow;

            while (this.irow <= lastRow)
            {
                int index = this.m_hPageBreaks.GetPageBreakIndex(this.m_worksheet[this.irow + 1, 1]);
                if (index != -1)
                {
                    this.m_hPageBreaks.RemoveAt(index);
                }
                this.irow++;
            }

            this.m_groupBy = firstColumn + groupBy;
            if (!replace)
            {
                summaryBelowData = !this.HasSubTotal(firstRow);
            }
            else
            {
                bool flag2 = false;
                this.irow = firstRow;
                while (this.irow <= lastRow)
                {
                    RowStorage row = this.m_recordTable.GetOrCreateRow(this.irow, m_height, false, m_version);
                    if (row != null)
                    {
                        for (int n = 0; n < totalList.Length; n++)
                        {
                            ICellPositionFormat cellOrNull = row.GetRecord(totalList[n], blockSize);
                            if (((cellOrNull != null) && cellOrNull.TypeCode == TBIFFRecord.Formula) && m_worksheet.GetValue(cellOrNull, false).StartsWith("=SUBTOTAL("))
                            {
                                this.m_worksheet.DeleteRow(this.irow + 1);
                                lastRow--;
                                break;
                            }
                        }
                    }
                    this.irow++;

                }
            }
            if (summaryBelowData)
            {
                this.CreateTotalBelowData();
            }
            else
            {
                this.CreateTotalAboveData();
            }
        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// Retruns corresponding column index above the subtotal formula
        /// </summary>
        /// <param name="rowStorage"></param>
        /// <returns></returns>
        private int SubTotalColumnIndex(RowStorage rowStorage)
        {
            int num = -1;
            for (int i = this.m_firstColumn; i <= this.m_lastColumn; i++)
            {
                ICellPositionFormat cellOrNull = rowStorage.GetRecord(i, blockSize);
                if ((cellOrNull != null) && cellOrNull.TypeCode != TBIFFRecord.Blank)
                {
                    if (cellOrNull.TypeCode != TBIFFRecord.Formula && (num == -1))
                    {
                        num = i;
                    }
                    if (cellOrNull.TypeCode == TBIFFRecord.Formula && m_worksheet.GetValue(cellOrNull, false).StartsWith("=SUBTOTAL("))
                    {
                        return num;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Check whether the current row contains SubTotal Formula
        /// </summary>
        /// <param name="irow">RowIndex</param>
        /// <returns>Returns true if row contains SubTotal Formula</returns>
        private bool HasSubTotal(int irow)
        {
            RowStorage row = this.m_recordTable.GetOrCreateRow(irow, m_height, false, m_version);
            if (row != null)
            {
                for (int i = this.m_firstColumn; i <= this.m_lastColumn; i++)
                {
                    ICellPositionFormat cellOrNull = row.GetRecord(i, blockSize);
                    if (((cellOrNull != null) && cellOrNull.TypeCode == TBIFFRecord.Formula) && m_worksheet.GetValue(cellOrNull, false).StartsWith("=SUBTOTAL("))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Check whether the current row contains SubTotal Formula
        /// </summary>
        /// <param name="rowStorage">RowStorage</param>
        /// <returns></returns>
        private bool HasSubTotal(RowStorage rowStorage)
        {
            for (int i = this.m_firstColumn; i <= this.m_lastColumn; i++)
            {
                ICellPositionFormat cellOrNull = rowStorage.GetRecord(i, blockSize);
                if (((cellOrNull != null) && cellOrNull.TypeCode == TBIFFRecord.Formula) && m_worksheet.GetValue(cellOrNull, false).StartsWith("=SUBTOTAL("))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Creates the Total row below the data
        /// </summary>
        private void CreateTotalBelowData()
        {
            bool flag = false;
            int num = -1;
            string strB = "";
            int firstIndex = -1;
            int num3 = (int)this.consolidationFunction;
            string str2 = "=SUBTOTAL(" + num3 + ",";
            IRange cell = null;

            this.irow = this.m_firstRow;
            while (this.irow <= this.m_lastRow)
            {
                bool flag2 = false;
                RowStorage row = this.m_recordTable.GetOrCreateRow(this.irow, m_height, false, m_version);
                if (row != null)
                {

                    ICellPositionFormat cellOrNull = row.GetRecord(this.m_groupBy, blockSize);
                    if ((cellOrNull == null) || (cellOrNull.TypeCode == TBIFFRecord.Blank))
                    {
                        if (this.m_replace)
                        {
                            goto Label_03C0;
                        }
                        if (this.m_lastRow == this.irow)
                        {
                            flag = true;
                        }
                        else
                        {
                            if (this.SubTotalColumnIndex(row) != -1)
                            {
                                num = this.irow;
                                flag2 = true;
                            }
                            if (!flag2)
                            {
                                goto Label_03C0;
                            }
                        }
                    }
                    if (!flag2)
                    {
                        string stringValue = m_worksheet.GetValue(cellOrNull, true);
                        if (firstIndex == -1)
                        {
                            strB = stringValue;
                            firstIndex = (this.irow > this.m_firstRow) ? this.m_firstRow : this.irow;
                        }
                        else
                        {
                            if ((string.Compare(stringValue, strB,System.StringComparison.OrdinalIgnoreCase) != 0) && ((firstIndex + 1) <= this.irow))
                            {
                                if (!this.m_replace && this.HasSubTotal(this.irow))
                                {
                                    if (num == -1)
                                    {
                                        num = this.irow + 1;
                                    }
                                    if (this.irow != this.m_lastRow)
                                    {
                                        goto Label_03C0;
                                    }
                                    flag = true;
                                }
                                goto Label_01A4;
                            }
                            strB = stringValue;
                        }
                        goto Label_03C0;
                    }
                Label_01A4:
                    this.m_worksheet.InsertRow(this.irow + 1);
                    this.m_lastRow++;
                    for (int i = 0; i < this.m_totalList.Length; i++)
                    {
                        StringBuilder builder = new StringBuilder(str2);
                        builder.Append(this.columnName[i]);
                        builder.Append((int)(firstIndex + 1));
                        builder.Append(':');
                        builder.Append(this.columnName[i]);
                        builder.Append(this.irow);
                        builder.Append(')');
                        this.m_worksheet[this.irow + 1, this.m_totalList[i] + 1].Formula = builder.ToString();
                    }
                    cell = this.m_worksheet[this.irow + 1, this.columnCount + 1];
                    cell.Value = strB + " " + this.total;
                    cell.CellStyle.Font.Bold = true;

                    if (this.m_groupRows)
                    {
                        if (num != -1)
                        {
                            this.m_worksheet[firstIndex + 1, this.m_firstColumn + 1, num, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
                        }
                        else
                        {
                            this.m_worksheet[firstIndex + 1, this.m_firstColumn + 1, this.irow, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
                        }
                    }
                    if (this.m_pageBreaks && !flag)
                    {
                        this.m_hPageBreaks.Add(this.m_worksheet[this.irow + 2, 1]);
                    }
                    strB = m_worksheet.GetValue(cellOrNull, true);
                    if (num == -1)
                    {
                        goto Label_03A6;
                    }
                    this.irow++;
                    firstIndex = this.irow + 1;
                    this.irow++;
                    while (this.irow < this.m_lastRow)
                    {
                        row = this.m_recordTable.GetOrCreateRow(this.irow, m_height, false, m_version);
                        if ((row == null) || !this.HasSubTotal(row))
                        {
                            goto Label_0378;
                        }
                        this.irow++;
                    }
                    goto Label_037F;
                Label_0378:
                    firstIndex = this.irow;
                Label_037F:
                    if (this.irow == this.m_lastRow)
                    {
                        flag = true;
                    }
                    else
                    {
                        this.irow--;
                    }
                    goto Label_03BB;
                Label_03A6:
                    this.irow++;
                    firstIndex = this.irow;
                Label_03BB:
                    num = -1;
                    if (flag)
                    {
                        break;
                    }
                }
            Label_03C0:
                this.irow++;
            }
            if (!flag && (firstIndex != -1))
            {
                this.m_worksheet.InsertRow(this.irow + 1, 1);
                cell = this.m_worksheet[this.irow + 1, this.columnCount + 1];
                cell.Value = strB + " " + this.total;
                cell.CellStyle.Font.Bold = true;
                if (this.m_groupRows)
                {
                    this.m_worksheet[firstIndex + 1, this.m_firstColumn + 1, (int)(this.irow), this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
                }
                for (int j = 0; j < this.m_totalList.Length; j++)
                {
                    StringBuilder builder2 = new StringBuilder(str2);
                    builder2.Append(this.columnName[j]);
                    builder2.Append((int)(firstIndex + 1));
                    builder2.Append(':');
                    builder2.Append(this.columnName[j]);
                    builder2.Append(this.irow);
                    builder2.Append(')');
                    this.m_worksheet[this.irow + 1, this.m_totalList[j] + 1].Formula = builder2.ToString();
                }
                this.irow++;
                if (this.m_replace)
                {
                    this.m_worksheet.InsertRow(this.irow + 1, 1);
                    cell = this.m_worksheet[this.irow + 1, this.columnCount + 1];
                    cell.Value = this.grandTotal;
                    cell.CellStyle.Font.Bold = true;
                    for (int k = 0; k < this.m_totalList.Length; k++)
                    {
                        StringBuilder builder3 = new StringBuilder(str2);
                        builder3.Append(this.columnName[k]);
                        builder3.Append((int)(this.m_firstRow + 1));
                        builder3.Append(':');
                        builder3.Append(this.columnName[k]);
                        builder3.Append((int)(this.m_lastRow + 1));
                        builder3.Append(')');
                        this.m_worksheet[this.irow + 1, this.m_totalList[k] + 1].Formula = builder3.ToString();
                    }
                }
            }
            if (this.m_groupRows && !flag)
            {
                this.m_worksheet[this.m_firstRow + 1, this.m_firstColumn + 1, this.irow, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
            }
            this.m_hPageBreaks.Add(this.m_worksheet[this.irow + 2, 1]);
        }
        /// <summary>
        /// Creates the Total row above the data
        /// </summary>
        private void CreateTotalAboveData()
        {
            bool flag = false;
            int num = -1;
            string str = "";
            int lastIndex = -1;
            int num3 = (int)this.consolidationFunction;
            string str2 = "=SUBTOTAL(" + num3 + ",";
            IRange cell = null;

            this.irow = this.m_lastRow;
            while (this.irow >= this.m_firstRow)
            {
                int num6;
                ICellPositionFormat cellOrNull = this.m_recordTable.GetOrCreateRow(this.irow, m_height, true, m_version).GetRecord(this.m_groupBy, blockSize);
                if ((cellOrNull == null) || (cellOrNull.TypeCode == TBIFFRecord.Blank))
                {
                    goto Label_0301;
                }
                string stringValue = m_worksheet.GetValue(cellOrNull, true);
                if (lastIndex == -1)
                {
                    str = stringValue;
                    lastIndex = (this.irow < this.m_lastRow) ? this.m_lastRow : this.irow;
                    goto Label_0301;
                }
                if (stringValue == str)
                {
                    goto Label_0301;
                }
                if (this.m_replace)
                {
                    goto Label_01AB;
                }
                RowStorage row = this.m_recordTable.GetOrCreateRow(this.irow, m_height, false, m_version);
                bool flag2 = false;
                for (int i = 0; i < this.m_totalList.Length; i++)
                {
                    cellOrNull = row.GetRecord(this.m_totalList[i], blockSize);
                    if (((cellOrNull != null) && cellOrNull.TypeCode == TBIFFRecord.Formula) && m_worksheet.GetValue(cellOrNull, false).StartsWith("=SUBTOTAL("))
                    {
                        goto Label_0176;
                    }
                }
                goto Label_0179;
            Label_0176:
                flag2 = true;
            Label_0179:
                if (flag2)
                {
                    if (num == -1)
                    {
                        num = this.irow;
                    }
                    if (this.m_summaryBelowData || (this.irow != this.m_firstRow))
                    {
                        goto Label_0301;
                    }
                    flag = true;
                }
            Label_01AB:
                num6 = this.irow + 1;
                lastIndex++;
                this.m_worksheet.InsertRow(num6 + 1);
                this.m_lastRow++;
                for (int j = 0; j < this.m_totalList.Length; j++)
                {
                    StringBuilder builder = new StringBuilder(str2);
                    builder.Append(this.columnName[j]);
                    builder.Append((int)(num6 + 2));
                    builder.Append(':');
                    builder.Append(this.columnName[j]);
                    builder.Append((int)(lastIndex + 1));
                    builder.Append(')');
                    this.m_worksheet[num6 + 1, this.m_totalList[j] + 1].Formula = builder.ToString();
                }
                cell = this.m_worksheet[num6 + 1, this.columnCount + 1];
                cell.Value = str + " " + this.total;
                cell.CellStyle.Font.Bold = true;
                if (this.m_groupRows)
                {
                    if (num != -1)
                    {
                        this.m_worksheet[(int)(num + 2) + 1, this.m_firstColumn + 1, lastIndex + 1, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
                    }
                    else
                    {
                        this.m_worksheet[(int)(num6 + 1) + 1, this.m_firstColumn + 1, lastIndex + 1, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
                    }
                }
                num = -1;

                if (this.m_pageBreaks && !flag)
                {
                    this.m_hPageBreaks.Add(this.m_worksheet[lastIndex + 2, 1]);
                }

                str = stringValue;
                lastIndex = this.irow;
                if (flag)
                {
                    break;
                }
            Label_0301:
                this.irow--;
            }
            this.irow++;
            if (!flag)
            {
                lastIndex++;
                this.m_worksheet.InsertRow(this.irow + 1, 1);
                this.m_lastRow++;
                cell = this.m_worksheet[this.irow + 1, this.columnCount + 1];
                cell.Value = str + " " + this.total;
                cell.CellStyle.Font.Bold = true;
                if (this.m_groupRows)
                {
                    this.m_worksheet[(int)(this.irow + 1) + 1, this.m_firstColumn + 1, lastIndex + 1, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
                }
                for (int k = 0; k < this.m_totalList.Length; k++)
                {
                    StringBuilder builder2 = new StringBuilder(str2);
                    builder2.Append(this.columnName[k]);
                    builder2.Append((int)(this.irow + 2));
                    builder2.Append(':');
                    builder2.Append(this.columnName[k]);
                    builder2.Append((int)(lastIndex + 1));
                    builder2.Append(')');
                    this.m_worksheet[this.irow + 1, this.m_totalList[k] + 1].Formula = builder2.ToString();
                }
                if (this.m_replace)
                {
                    this.m_worksheet.InsertRow(this.irow + 1, 1);
                    this.m_lastRow++;
                    cell = this.m_worksheet[this.irow + 1, this.columnCount + 1];
                    cell.Value = this.grandTotal;
                    cell.CellStyle.Font.Bold = true;
                    for (int m = 0; m < this.m_totalList.Length; m++)
                    {
                        StringBuilder builder3 = new StringBuilder(str2);
                        builder3.Append(this.columnName[m]);
                        builder3.Append((int)(this.m_firstRow + 2));
                        builder3.Append(':');
                        builder3.Append(this.columnName[m]);
                        builder3.Append((int)(this.m_lastRow + 1));
                        builder3.Append(')');
                        this.m_worksheet[this.irow + 1, this.m_totalList[m] + 1].Formula = builder3.ToString();
                    }
                }
            }
            if (this.m_groupRows)
            {
                this.m_worksheet[(this.m_firstRow + 1) + 1, this.m_firstColumn + 1, this.m_lastRow + 1, this.m_lastColumn + 1].Group(ExcelGroupBy.ByRows, false);
            }
            this.m_hPageBreaks.Add(this.m_worksheet[lastIndex + 2, 1]);
        }
        /// <summary>
        /// Get Corresponding String Value
        /// </summary>
        /// <param name="consolidationFunction">ConsolidationFunction</param>
        /// <returns>Retruns the consolidationfunction string</returns>
        private static string GetEnumerationString(ConsolidationFunction consolidationFunction)
        {
            switch (consolidationFunction)
            {
                case ConsolidationFunction.Count:
                case ConsolidationFunction.CountNums:
                    return ConsolidationFunction.Count.ToString();

                case ConsolidationFunction.Average:
                    return ConsolidationFunction.Average.ToString();

                case ConsolidationFunction.Max:
                    return ConsolidationFunction.Max.ToString();

                case ConsolidationFunction.Min:
                    return ConsolidationFunction.Min.ToString();

                case ConsolidationFunction.Product:
                    return ConsolidationFunction.Product.ToString();

                case ConsolidationFunction.StdDev:
                    return ConsolidationFunction.StdDev.ToString();

                case ConsolidationFunction.StdDevp:
                    return ConsolidationFunction.StdDevp.ToString();

                case ConsolidationFunction.Var:
                    return ConsolidationFunction.Var.ToString();

                case ConsolidationFunction.Varp:
                    return ConsolidationFunction.Varp.ToString();
            }
            return "Total";
        }
        #endregion

        
    }

}
