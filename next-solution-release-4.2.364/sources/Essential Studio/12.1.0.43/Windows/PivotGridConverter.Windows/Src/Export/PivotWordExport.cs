#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Drawing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Forms.PivotAnalysis;
using Syncfusion.Windows.Forms.Grid;
using System;

namespace Syncfusion.PivotGridConverter
{
    /// <summary>
    /// GridWordExport exports the Pivot data to Word with the applied style
    /// </summary>    
    public class PivotWordExport
    {
        #region Private Members
        private WTextBody _textBody;
        #endregion

        /// <summary>
        /// Gets or sets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl gridControl1 { get; internal set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PivotWordExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        public PivotWordExport(PivotGridControl gridControl)
        {
            this.gridControl1 = gridControl;
        }

        /// <summary>
        /// Exports the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void pivotGridToWord(string filename)
        {
            int l1, r1;
            WordDocument document = new WordDocument();
            //// Define Section to render data in declared document
            //// To define word paragraphs
            IWParagraph paragraph;
            //// Add sections in document
            IWSection section = document.AddSection();
            section.PageSetup.PageSize = new SizeF(1000, 1275);
            System.Collections.ArrayList widthCollection = new System.Collections.ArrayList();
            
            int columnCount = this.gridControl1.TableModel.Model.ColCount;
            double sum = 0;
            for (int i = 0; i <= columnCount; i++)
            {
                sum += this.gridControl1.TableModel.Model.ColWidths[i];
                if (sum > section.PageSetup.ClientWidth - 100)
                {
                    widthCollection.Add(i);
                    sum = 0;
                }
            }
            if (widthCollection.Count == 0 || ((int)widthCollection[widthCollection.Count - 1] < columnCount))
            {
                widthCollection.Add(columnCount);
            }
            if (widthCollection.Count == 0)
                return;

            for (int _col = 0; _col < widthCollection.Count; _col++)
            {
                int renderColCount = (int)widthCollection[_col];                
                if (_col > 0)
                    renderColCount = ((int)widthCollection[_col] - (int)widthCollection[_col - 1]);

                //// Add sections in document
                paragraph = section.AddParagraph();
                //// format paragraph
                paragraph.ParagraphFormat.BeforeSpacing = 18f;
                //// Define a section content
                _textBody = section.Body;
                //// Create instance for table in word
                IWTable docTable = _textBody.AddTable();
                
                docTable.ResetCells(this.gridControl1.PivotEngine.RowCount - 1, renderColCount, null, (int)((section.PageSetup.ClientWidth - 100) / renderColCount));

                for (int row = 0; row < this.gridControl1.PivotEngine.RowCount - 1; row++)
                {
                    int cellIndex = 0;
                    for (int c = (int)widthCollection[_col] - renderColCount; c < (int)widthCollection[_col]; c++)
                    {
                        PivotCellInfo pivotCellInfo = this.gridControl1.PivotEngine[row, c];
                        if (pivotCellInfo != null)
                        {
                            if (pivotCellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell))
                            {
                                int cell3 = cellIndex;
                                int row3 = row;
                                int cell4 = cellIndex;
                                int row4 = row;

                                //Merge the header cells
                                for (int i = pivotCellInfo.CellRange.Top; i <= pivotCellInfo.CellRange.Bottom; i++)
                                {
                                    for (int j = pivotCellInfo.CellRange.Left; j < pivotCellInfo.CellRange.Right; j++)
                                    {
                                        docTable.Rows[row4].Cells[cell4].CellFormat.HorizontalMerge = CellMerge.Start;
                                        cell4 += 1;
                                        docTable.Rows[row4].Cells[cell4].CellFormat.HorizontalMerge = CellMerge.Continue;
                                    }
                                    row4 += 1;
                                    cell4 = cellIndex;
                                }

                                for (int i = pivotCellInfo.CellRange.Left; i <= pivotCellInfo.CellRange.Right; i++)
                                {
                                    docTable.Rows[row3].Cells[cell3].CellFormat.VerticalMerge = CellMerge.Start;
                                    for (int j = pivotCellInfo.CellRange.Top; j <= pivotCellInfo.CellRange.Bottom; j++)
                                    {
                                        docTable.Rows[row3].Cells[cell3].CellFormat.VerticalMerge = CellMerge.Continue;
                                        row3 += 1;
                                    }
                                    cell3 += 1;
                                    row3 = row;
                                }
                            }

                            //Merge total cells
                            int cell1 = cellIndex;
                            int row1 = row;
                            int cell2 = cellIndex;
                            int row2 = row;

                            if (pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell))
                            {
                                if (cell2 < 10)
                                    docTable.Rows[row2].Cells[cell2].CellFormat.VerticalMerge = CellMerge.Start;

                                for (int i = pivotCellInfo.CellRange.Left; i <= pivotCellInfo.CellRange.Right; i++)
                                {
                                    int c1 = 0;
                                    for (int j = pivotCellInfo.CellRange.Top; j <= pivotCellInfo.CellRange.Bottom; j++)
                                    {
                                        if (cell2 < 10)
                                        {
                                            docTable.Rows[row2].Cells[cell2].CellFormat.VerticalMerge = CellMerge.Continue;
                                            row2 += 1;
                                        }
                                        c1++;
                                    }
                                    cell2 = cell2 + 1;
                                    row2 -= c1;
                                }

                                for (int i = pivotCellInfo.CellRange.Top; i <= pivotCellInfo.CellRange.Bottom; i++)
                                {
                                    docTable.Rows[row1].Cells[cell1].CellFormat.HorizontalMerge = CellMerge.Start;

                                    for (int j = pivotCellInfo.CellRange.Left; j <= pivotCellInfo.CellRange.Right; j++)
                                    {
                                        if (cell1 < 10)
                                        {
                                            docTable.Rows[row1 + 1].Cells[cell1].CellFormat.HorizontalMerge = CellMerge.Continue;
                                            cell1 += 1;
                                        }
                                    }
                                    cell1 = cellIndex;
                                    row1 += 1;
                                }
                            }

                            //Merge left three cells
                            if (pivotCellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell))
                            {
                                docTable.Rows[pivotCellInfo.CellRange.Top].Cells[0].CellFormat.VerticalMerge = CellMerge.Start;

                                for (int i = pivotCellInfo.CellRange.Top; i <= pivotCellInfo.CellRange.Bottom; i++)
                                {
                                    docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.VerticalMerge = CellMerge.Continue;

                                    if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black)
                                        docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");

                                    else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Metro)
                                        docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");

                                    else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2003)
                                        docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");

                                    else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black ||
                                    gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver ||
                                    gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Silver)
                                        docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");

                                    if ((gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue) ||
                                    (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue))
                                        docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");

                                    if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Custom)
                                        docTable.Rows[i].Cells[pivotCellInfo.CellRange.Left].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");
                                }
                            }

                            //Merge topleft cell
                            if (pivotCellInfo.CellType == PivotCellType.TopLeftCell)
                            {
                                for (int i = 0; i <= pivotCellInfo.CellRange.Right + 2; i++)
                                {
                                    docTable.Rows[i].Cells[0].CellFormat.HorizontalMerge = CellMerge.Start;
                                    for (int j = 1; j < pivotCellInfo.CellRange.Bottom; j++)
                                    {
                                        docTable.Rows[i].Cells[j].CellFormat.HorizontalMerge = CellMerge.Continue;

                                        if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Metro)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2003)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black ||
                                        gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver ||
                                        gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Silver)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");

                                        else if ((gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue) ||
                                        (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue))
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor =                                                    ColorTranslator.FromHtml("#e3ecf7");

                                        else  if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Custom)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor =
                                                    ColorTranslator.FromHtml("#e3ecf7");
                                    }
                                }

                                for (int i = 0; i <= pivotCellInfo.CellRange.Right; i++)
                                {
                                    docTable.Rows[0].Cells[i].CellFormat.VerticalMerge = CellMerge.Start;
                                    for (int j = 1; j <= pivotCellInfo.CellRange.Bottom; j++)
                                    {
                                        docTable.Rows[j].Cells[i].CellFormat.VerticalMerge = CellMerge.Continue;

                                        if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Metro)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2003)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black ||
                                        gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver ||
                                        gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Silver)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");

                                        else if ((gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue) ||
                                        (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue))
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor =
                                                    ColorTranslator.FromHtml("#e3ecf7");

                                        else if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Custom)
                                            docTable.Rows[i].Cells[j].CellFormat.BackColor =
                                                    ColorTranslator.FromHtml("#e3ecf7");

                                    }
                                }
                            }

                                if (pivotCellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell) || pivotCellInfo.CellType ==
                                    (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) || pivotCellInfo.CellType == (PivotCellType.ColumnHeaderCell |
                                    PivotCellType.GrandTotalCell))
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Continue;
                                }

                            if (pivotCellInfo.FormattedText != null)
                            {
                                if (cellIndex < docTable.Rows[row].Cells.Count)
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.ClearFormatting();
                                    docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(
                                        pivotCellInfo.FormattedText);
                                }

                                if (pivotCellInfo.CellRange != null)
                                {
                                    if (pivotCellInfo.CellRange.Left < pivotCellInfo.CellRange.Right)
                                    {
                                        int r = row + 1;
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                        int top = pivotCellInfo.CellRange.Top;
                                        while (top != pivotCellInfo.CellRange.Bottom && r < docTable.Rows.Count && cellIndex < docTable.Rows[r++].Cells.Count)
                                        {
                                            docTable.Rows[r++].Cells[cellIndex].CellFormat.VerticalMerge =
                                                CellMerge.Continue;
                                            top++;
                                        }
                                    }

                                    if (pivotCellInfo.CellRange.Bottom == pivotCellInfo.CellRange.Top)
                                    {
                                        int r = row;
                                        int cell = cellIndex + 1;
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                        int left = pivotCellInfo.CellRange.Left;
                                        l1 = pivotCellInfo.CellRange.Left;
                                        r1 = pivotCellInfo.CellRange.Right;

                                        if ((pivotCellInfo.CellType == (PivotCellType.ExpanderCell)) || (pivotCellInfo.CellType == PivotCellType.ColumnHeaderCell))
                                        {
                                            docTable.Rows[r].Cells[0].CellFormat.ClearFormatting();
                                            docTable.Rows[r].Cells[0].AddParagraph().AppendText(
                                                pivotCellInfo.FormattedText);
                                        }

                                        if (_col > 0)
                                            renderColCount = ((int)widthCollection[_col] - (int)widthCollection[_col - 1]);

                                        while (left != pivotCellInfo.CellRange.Right && cell < renderColCount)
                                        {
                                            docTable.Rows[r].Cells[cell++].CellFormat.HorizontalMerge =
                                                CellMerge.Continue;
                                            left++;
                                            l1++;
                                        }
                                    }


                                    if ((pivotCellInfo.CellRange.Bottom - pivotCellInfo.CellRange.Top) == 1 &&
                                        (pivotCellInfo.CellRange.Right - pivotCellInfo.CellRange.Left) == 1)
                                    {
                                        if (pivotCellInfo.CellType ==
                                            (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                            pivotCellInfo.CellType ==
                                            (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell))
                                        {
                                            int cell = cellIndex + 1;
                                            docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge =
                                                CellMerge.Start;
                                            int left = pivotCellInfo.CellRange.Left;
                                            while (left != pivotCellInfo.CellRange.Right && cell < docTable.Rows[row].Cells.Count)
                                            {
                                                docTable.Rows[row].Cells[cell++].CellFormat.HorizontalMerge =
                                                    CellMerge.Continue;
                                                left++;
                                            }
                                            int r = row + 1;
                                            docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                            int top = pivotCellInfo.CellRange.Top;
                                            while (pivotCellInfo.CellRange.Bottom != top && cellIndex < docTable.Rows[r++].Cells.Count)
                                            {
                                                docTable.Rows[r++].Cells[cellIndex].CellFormat.VerticalMerge =
                                                    CellMerge.Continue;
                                                top++;
                                            }
                                        }

                                        if (pivotCellInfo.CellType == PivotCellType.HeaderCell && pivotCellInfo.CellType == PivotCellType.ColumnHeaderCell)
                                        {
                                            int r = row;
                                            int cell = cellIndex + 1;
                                            docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                            int left = pivotCellInfo.CellRange.Left;

                                            while (cell < renderColCount)
                                            {
                                                docTable.Rows[r].Cells[cell++].CellFormat.HorizontalMerge =
                                                    CellMerge.Continue;
                                                left++;
                                            }
                                        }
                                    }
                                }
                            }

                            if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Metro)
                            {
                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;

                                    case PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");
                                        break;

                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#2abff1");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#d0d0d0");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;

                                    case PivotCellType.CalculationHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#d0d0d0");
                                        break;
                                }
                            }

                            if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2003)
                            {
                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#85a9e4");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;

                                    case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;

                                    case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;

                                    case PivotCellType.CalculationHeaderCell | PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;

                                    case PivotCellType.CalculationHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                }
                            }

                            if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black ||
                                gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver ||
                                gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Silver)
                            {

                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;

                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;

                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e9e9e9");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                }
                            }

                            if ((gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue) ||
                                (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue))
                            {
                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#e3ecf7");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                      break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                }
                            }

                            if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black)
                            {
                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#626262");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                }
                            }

                            if (gridControl1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Custom)
                            {
                                if (pivotCellInfo.FormattedText.Equals("Grand Total"))
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#fbe292");
                                }

                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#f7f8fa");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#f7f8fa");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#f7f8fa");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#f7f8fa");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#f7f8fa");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#f7f8fa");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                    case PivotCellType.CalculationHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml("#fbe292");
                                        break;
                                }
                            }
                        }
                        if (cellIndex < docTable.Rows[row].Cells.Count)
                        {
                            docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                            int gridColIndex = (_col > 0) ? ((int)widthCollection[_col - 1] + cellIndex) : cellIndex;
                            docTable.Rows[row].Cells[cellIndex].Width = (float)this.gridControl1.TableModel.Model.ColWidths[gridColIndex];
                        }
                        cellIndex++;
                    }
                }
            }
            document.Save(filename, FormatType.Doc);
        }
    }
}
