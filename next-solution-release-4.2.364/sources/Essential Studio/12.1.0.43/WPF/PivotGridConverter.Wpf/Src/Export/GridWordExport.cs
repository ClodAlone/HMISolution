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
using Syncfusion.Windows.Controls.PivotGrid;
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.PivotGrid.Converter
{
    /// <summary>
    /// GridWordExport exports the Pivot data to Word with the applied style
    /// </summary>
    public class GridWordExport
    {
        #region Private Members

        private RowFormat _format;
        private WTextBody _textBody;

        #endregion

        /// <summary>
        /// Gets or sets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl GridControl { get; internal set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="GridWordExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        public GridWordExport(PivotGridControl gridControl)
        {
            this.GridControl = gridControl;
        }

        /// <summary>
        /// Exports the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Export(string filename)
        {
            WordDocument document = new WordDocument();
            //// Define Section to render data in declared document
            //// To define word paragraphs
            IWParagraph paragraph;
            //// Add sections in document
            IWSection section = document.AddSection();

            section.PageSetup.PageSize = new SizeF(1000, 1275);

            System.Collections.ArrayList widthCollection = new System.Collections.ArrayList();
            System.Collections.ArrayList hiddenColCollection = new System.Collections.ArrayList();
            int columnCount = this.GridControl.InternalGrid.ColumnWidths.LineCount;
            double sum = 0;
            int dummy = 0;
            int hiddenColCount = 0;      
            for (int i = 0,col=0; col <= columnCount; col++)
            {
                if (this.GridControl.InternalGrid.Model.ColumnWidths.GetHidden(col, out dummy))
                {
                    hiddenColCount++;
                    continue;
                }
                sum += this.GridControl.InternalGrid.ColumnWidths[col];
                if (sum > section.PageSetup.ClientWidth - 100)
                {
                    widthCollection.Add(i);
                    hiddenColCollection.Add(hiddenColCount);
                    sum = 0;
                }
                i++;
            }
            if (widthCollection.Count == 0 || ((int)widthCollection[widthCollection.Count - 1] < columnCount-hiddenColCount))
            {
                widthCollection.Add(columnCount-hiddenColCount);
            }

            if (widthCollection.Count == 0)
                return;

            int renderColCount = (int)widthCollection[0];
            for (int _col = 0; _col < widthCollection.Count; _col++)
            {
                if (_col > 0)
                    renderColCount = ((int)widthCollection[_col] - (int)widthCollection[_col - 1]);

                //// Add sections in document
                paragraph = section.AddParagraph();
                //// sets the page size of the word document

                //// format paragraph
                paragraph.ParagraphFormat.BeforeSpacing = 18f;
                //// Define a section content
                _textBody = section.Body;
                //// Create instance for table in word
                IWTable docTable = _textBody.AddTable();
                //// set the margin of the setion
                //// Formats the table values 
                _format = new RowFormat();

                _format.Borders.BorderType = BorderStyle.Single;
                _format.Borders.LineWidth = 1.0F;
                _format.CellSpacing = 0;
                int rowCount = 0;
                _format.Borders.Color = System.Drawing.Color.Blue;
                if (this.GridControl.PivotEngine.RowCount - 1 > 0 && renderColCount > 0)
                    docTable.ResetCells(this.GridControl.PivotEngine.RowCount - 1, renderColCount, _format, (int)((section.PageSetup.ClientWidth - 100) / renderColCount));
                for (int row = 0, row1 = 0; row < this.GridControl.PivotEngine.RowCount - 1; )
                {
                    int cellIndex = 0 , c=0, column =0;
                    if (!this.GridControl.InternalGrid.Model.RowHeights.GetHidden(row, out dummy))
                    {
                        c = (int)widthCollection[_col] + (_col > 0 ? (int)hiddenColCollection[_col - 1] : 0) - renderColCount;
                        for (column= (int)widthCollection[_col] - renderColCount;c< columnCount && column < (int)widthCollection[_col]; c++)
                        {
                            if (this.GridControl.InternalGrid.Model.ColumnWidths.GetHidden(c, out dummy))
                            {
                                continue;
                            }
                            column++;
                            PivotCellInfo pivotCellInfo = this.GridControl.PivotEngine[row, c];
                            if (pivotCellInfo != null)
                            {
                                if (pivotCellInfo.CellType == PivotCellType.TopLeftCell)
                                {
                                    for (int i = 0; i <= pivotCellInfo.CellRange.Right + 1; i++)
                                    {
                                        if (i < docTable.Rows.Count)
                                            docTable.Rows[i].Cells[0].CellFormat.HorizontalMerge = CellMerge.Start;
                                        for (int j = 1; j < pivotCellInfo.CellRange.Bottom; j++)
                                        {
                                            docTable.Rows[i].Cells[j].CellFormat.HorizontalMerge = CellMerge.Continue;
                                        }
                                    }
                                    for (int i = 0; i <= pivotCellInfo.CellRange.Right; i++)
                                    {
                                        if (i < docTable.Rows[0].Cells.Count)
                                            docTable.Rows[0].Cells[i].CellFormat.VerticalMerge = CellMerge.Start;
                                        for (int j = 1; j <= pivotCellInfo.CellRange.Bottom; j++)
                                        {
                                            docTable.Rows[j].Cells[i].CellFormat.VerticalMerge = CellMerge.Continue;
                                        }
                                    }
                                }

                                if (pivotCellInfo.FormattedText != null)
                                {
                                    if (cellIndex < docTable.Rows[row1].Cells.Count)
                                    {
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.ClearFormatting();
                                        docTable.Rows[row1].Cells[cellIndex].AddParagraph().AppendText(
                                            pivotCellInfo.FormattedText);
                                    }
                                    if (pivotCellInfo.CellRange != null)
                                    {
                                        if (pivotCellInfo.CellRange.Left == pivotCellInfo.CellRange.Right)
                                        {
                                            int r = row1 + 1;
                                            docTable.Rows[row1].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                            int top = pivotCellInfo.CellRange.Top;
                                            while (top != pivotCellInfo.CellRange.Bottom && r + 1 < docTable.Rows.Count && cellIndex < docTable.Rows[r + 1].Cells.Count)
                                            {
                                                docTable.Rows[r++].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                                top++;
                                            }
                                        }
                                        if (pivotCellInfo.CellRange.Bottom == pivotCellInfo.CellRange.Top)
                                        {
                                            int r = row1;
                                            int cell = cellIndex + 1;
                                            docTable.Rows[row1].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                            int left = pivotCellInfo.CellRange.Left;
                                            while (left != pivotCellInfo.CellRange.Right && cell < renderColCount)
                                            {
                                                docTable.Rows[r].Cells[cell++].CellFormat.HorizontalMerge = CellMerge.Continue;
                                                left++;
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
                                                docTable.Rows[row1].Cells[cellIndex].CellFormat.HorizontalMerge =
                                                    CellMerge.Start;
                                                int left = pivotCellInfo.CellRange.Left;
                                                while (left != pivotCellInfo.CellRange.Right && cell < docTable.Rows[row1].Cells.Count)
                                                {
                                                    docTable.Rows[row1].Cells[cell++].CellFormat.HorizontalMerge =
                                                        CellMerge.Continue;
                                                    left++;
                                                }
                                                int r = row1 + 1;
                                                docTable.Rows[row1].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                                int top = pivotCellInfo.CellRange.Top;
                                                while (pivotCellInfo.CellRange.Bottom != top && cellIndex < docTable.Rows[r++].Cells.Count)
                                                {
                                                    docTable.Rows[r++].Cells[cellIndex].CellFormat.VerticalMerge =
                                                        CellMerge.Continue;
                                                    top++;
                                                }
                                            }
                                        }
                                    }

                                    if (pivotCellInfo.FormattedText.Equals("Grand Total"))
                                    {
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                                ColorTranslator.FromHtml("#86BCF2");
                                    }
                                }
                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#86BCF2");
                                        break;
                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#86BCF2");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#7B7BEE");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#7B7BEE");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#7B7BEE");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#7B7BEE7");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#7B7BEE");
                                        break;
                                    case PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#86BCF2");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#86BCF2");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#86BCF2");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row1].Cells[cellIndex].CellFormat.BackColor =
                                            ColorTranslator.FromHtml("#86BCF2");
                                        break;
                                }

                            }
                            if (cellIndex < docTable.Rows[row1].Cells.Count)
                            {
                                docTable.Rows[row1].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                                docTable.Rows[row1].Cells[cellIndex].Width = (float)this.GridControl.InternalGrid.ColumnWidths[c];
                            }
                            cellIndex++;
                        }
                        row1++;
                        rowCount++;
                    }
                    row++;
                }
                while (docTable.Rows.Count > rowCount)
                    docTable.Rows.Remove(docTable.LastRow);
            }
            document.Save(filename, FormatType.Doc);
        }
    }
}
