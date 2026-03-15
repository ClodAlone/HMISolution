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
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.IO;
using System;
using Syncfusion.Silverlight.Controls.PivotGrid;
using System.Collections.Generic;

namespace Syncfusion.Silverlight.Controls.PivotGrid.Converter
{
    /// <summary>
    /// GridWordExport exports the Pivot data to Word with the applied style
    /// </summary>
    public class GridWordExport
    {
        #region Private Members

        private RowFormat _format;
        private WTextBody _textBody;
        private int _col, _temp, _count = 5;

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
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            WordDocument document = new WordDocument();
            //// Define Section to render data in declared document
            //// To define word paragraphs
            IWParagraph paragraph;
            //// Add sections in document
            IWSection section = document.AddSection();
            List<int> subTotalRowIndices=new List<int>();
            if (this.GridControl.PivotEngine.ColumnCount > 6)
            {
                for (_col = 6; _col <= this.GridControl.PivotEngine.ColumnCount; _col += 6)
                {
                    //// Add sections in document
                    paragraph = section.AddParagraph();
                    //// sets the page size of the word document

                    section.PageSetup.PageSize = new SizeF(1000, 1275);
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

                    _format.Borders.Color = System.Drawing.Color.Blue;
                    docTable.ResetCells(this.GridControl.PivotEngine.RowCount - 1, 6, _format, 140);

                    List<int> subTotalColumnIndices = new List<int>();
                    for (int row = 0; row < this.GridControl.PivotEngine.RowCount - 1; row++)
                    {
                        int cellIndex = 0;
                        for (int c = _temp; c <= _count; c++)
                        {
                            PivotCellInfo pivotCellInfo = this.GridControl.PivotEngine[row, c];
                            if (pivotCellInfo != null)
                            {
                                if (pivotCellInfo.CellType == PivotCellType.TopLeftCell)
                                {
                                    if (pivotCellInfo.CellRange != null)
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
                                }

                                if (pivotCellInfo.FormattedText != null)
                                {
                                    docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(
                                        pivotCellInfo.FormattedText);

                                    if (pivotCellInfo.CellRange != null)
                                    {
                                        if (pivotCellInfo.CellRange.Left == pivotCellInfo.CellRange.Right)
                                        {
                                            int r = row + 1;
                                            docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                            int top = pivotCellInfo.CellRange.Top;
                                            while (top != pivotCellInfo.CellRange.Bottom)
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
                                            while (left != pivotCellInfo.CellRange.Right && cell < 6)
                                            {
                                                docTable.Rows[r].Cells[cell++].CellFormat.HorizontalMerge =
                                                    CellMerge.Continue;
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
                                                docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge =
                                                    CellMerge.Start;
                                                int left = pivotCellInfo.CellRange.Left;
                                                while (left != pivotCellInfo.CellRange.Right)
                                                {
                                                    docTable.Rows[row].Cells[cell++].CellFormat.HorizontalMerge =
                                                        CellMerge.Continue;
                                                    left++;
                                                }
                                                int r = row + 1;
                                                docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                                int top = pivotCellInfo.CellRange.Top;
                                                while (pivotCellInfo.CellRange.Bottom != top)
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
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                    }
                                }
                                switch (pivotCellInfo.CellType)
                                {
                                    case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF7B7BEE");
                                        break;
                                    case PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF7B7BEE");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF7B7BEE");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF7B7BEE7");
                                        break;
                                    case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF7B7BEE");
                                        break;
                                    case PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                              GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF86BCF2");
                                        break;
                                }
                                
                            }

                            docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                            cellIndex++;
                        }
                    }

                    if (!this.GridControl.ShowSubTotals)
                    {
                        for (int row = 0; row < this.GridControl.PivotEngine.RowCount-1; row++)
                        {
                            int cellIndex = 0;
                            for (int c = _temp; c < _count; c++)
                            {
                                PivotCellInfo pivotCellInfo = this.GridControl.PivotEngine[row, c];
                                int c1 = cellIndex;
                                if (pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && !this.GridControl.ShowSubTotals)
                                {
                                    if ((this.GridControl.PivotCalculations.Count == 1))
                                        subTotalColumnIndices.Add(cellIndex);
                                    else if ((this.GridControl.PivotCalculations.Count > 1) && (this.GridControl.PivotCalculations.Count > 1 && !this.GridControl.PivotEngine[row, c + this.GridControl.PivotCalculations.Count].FormattedText.Contains(this.GridControl.PivotEngine.GrandString)))
                                    {
                                        foreach (PivotComputationInfo pc in this.GridControl.PivotCalculations)
                                        {
                                            subTotalColumnIndices.Add(c1);
                                            c1++;
                                        }
                                    }
                                    else if ((this.GridControl.PivotCalculations.Count > 1 && this.GridControl.PivotEngine[row, c + this.GridControl.PivotCalculations.Count].FormattedText.Contains(this.GridControl.PivotEngine.GrandString)))
                                    {
                                        foreach (PivotComputationInfo pc in this.GridControl.PivotCalculations)
                                        {
                                            subTotalColumnIndices.Add(c1);
                                        }
                                    }
                                }
                                if (pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) && !this.GridControl.ShowSubTotals)
                                {
                                    subTotalRowIndices.Add(row);
                                }
                                cellIndex++;
                            }
                        }
                        for (int row = 0; row < this.GridControl.PivotEngine.RowCount - 1; row++)
                        {
                            int cellIndex = 0;
                            List<int> cr1 = new List<int>();
                            cr1.AddRange(subTotalRowIndices);
                            for (int i = 0; i < subTotalRowIndices.Count; i++)
                            {
                                if (subTotalRowIndices.Contains(row) && subTotalRowIndices.Count == 1 && subTotalRowIndices != null && row < docTable.Rows.Count && subTotalRowIndices[i] < docTable.Rows.Count)
                                {
                                    docTable.Rows.RemoveAt(subTotalRowIndices[i]);
                                }
                                else if (cr1.Contains(row) && cr1.Count > 1 && cr1 != null && row < docTable.Rows.Count && cr1[i] < docTable.Rows.Count)
                                {
                                    docTable.Rows.RemoveAt(cr1[i]);
                                    for (int i1 = 0; i1 < cr1.Count; i1++)
                                    {
                                        if (cr1[i1] != subTotalRowIndices[i] && i1 >= i)
                                        {
                                            cr1[i1]--;
                                        }
                                    }
                                }
                            }
                            for (int c = _temp; c < _count; c++)
                            {
                                List<int> ce1 = new List<int>();
                                ce1.AddRange(subTotalColumnIndices);
                                for (int i = 0; i < subTotalColumnIndices.Count; i++)
                                {
                                    int c1 = cellIndex;
                                    if (subTotalColumnIndices.Contains(c1) && subTotalColumnIndices.Count == 1 && subTotalColumnIndices != null && row < docTable.Rows.Count && subTotalColumnIndices[i] < docTable.Rows[row].Cells.Count)
                                    {
                                        docTable.Rows[row].Cells.RemoveAt(subTotalColumnIndices[i]);
                                    }
                                    else if (ce1.Contains(c1) && ce1.Count > 1 && ce1 != null && row < docTable.Rows.Count && ce1[i] < docTable.Rows[row].Cells.Count)
                                    {
                                        docTable.Rows[row].Cells.RemoveAt(ce1[i]);
                                        for (int i1 = 0; i1 < ce1.Count; i1++)
                                        {
                                            if (ce1[i1] != subTotalColumnIndices[i] && i1 >= i)
                                            {
                                                ce1[i1]--;
                                            }
                                        }
                                    }
                                }
                                cellIndex++;
                            }
                        }
                    }
                    _count += 6;
                    _temp = _count - 5;
                }
                if (this.GridControl.PivotEngine.ColumnCount % 6 != 0)
                {
                    paragraph = DoforRemainingColumns(section, (this.GridControl.PivotEngine.ColumnCount % 6), subTotalRowIndices);
                }
            }

            else
            {
                paragraph = DoforRemainingColumns(section, this.GridControl.PivotEngine.ColumnCount,subTotalRowIndices);
            }
            document.Save(stream, FormatType.Doc);
        }

        private IWParagraph DoforRemainingColumns(IWSection section, int columnCount, List<int> subTotalRowIndices)
        {
            IWParagraph paragraph;
            //// Add sections in document
            paragraph = section.AddParagraph();
            //// sets the page size of the word document
            section.PageSetup.PageSize = new SizeF(1200, 1275);
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

            _format.Borders.Color = System.Drawing.Color.Blue;
            if (this.GridControl.PivotEngine.RowCount - 1 > 0 && columnCount-1 > 0)
                docTable.ResetCells(this.GridControl.PivotEngine.RowCount - 1, columnCount - 1, _format, 140);
            List<int> subTotalColumnIndices = new List<int>();
            for (int row = 0; row <= this.GridControl.PivotEngine.RowCount - 2; row++)
            {
                int cellIndex = 0;
                for (int c = _temp; c <= (_temp + columnCount - 2); c++)
                {
                    PivotCellInfo pivotCellInfo = this.GridControl.PivotEngine[row, c];
                    if (pivotCellInfo != null)
                    {
                        if (pivotCellInfo.CellType == PivotCellType.TopLeftCell)
                        {
                            for (int i = 0; i <= pivotCellInfo.CellRange.Right; i++)
                            {
                                docTable.Rows[i].Cells[0].CellFormat.HorizontalMerge = CellMerge.Start;
                                for (int j = 1; j <= pivotCellInfo.CellRange.Bottom; j++)
                                {
                                    docTable.Rows[i].Cells[j].CellFormat.HorizontalMerge = CellMerge.Continue;
                                }
                            }
                            for (int i = 0; i <= pivotCellInfo.CellRange.Right; i++)
                            {
                                docTable.Rows[0].Cells[i].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int j = 0; j <= pivotCellInfo.CellRange.Bottom; j++)
                                {
                                    docTable.Rows[j].Cells[i].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                        }

                        if (pivotCellInfo.FormattedText != null)
                        {
                            docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(
                                pivotCellInfo.FormattedText);

                            if (pivotCellInfo.CellRange != null)
                            {
                                if (pivotCellInfo.CellRange.Left == pivotCellInfo.CellRange.Right)
                                {
                                    int r = row + 1;
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                    int top = pivotCellInfo.CellRange.Top;
                                    while (pivotCellInfo.CellRange.Bottom != top)
                                    {
                                        docTable.Rows[r++].Cells[cellIndex].CellFormat.VerticalMerge =
                                            CellMerge.Continue;
                                        top++;
                                    }

                                }
                                if (pivotCellInfo.CellRange.Bottom == pivotCellInfo.CellRange.Top)
                                {
                                    int cell = cellIndex + 1;
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                    int left = pivotCellInfo.CellRange.Left;
                                    while (left != pivotCellInfo.CellRange.Right)
                                    {
                                        docTable.Rows[row].Cells[cell++].CellFormat.HorizontalMerge =
                                            CellMerge.Continue;
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
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge =
                                            CellMerge.Start;
                                        int left = pivotCellInfo.CellRange.Left;
                                        while (left != pivotCellInfo.CellRange.Right)
                                        {
                                            docTable.Rows[row].Cells[cell++].CellFormat.HorizontalMerge =
                                                CellMerge.Continue;
                                            left++;
                                        }
                                        int r = row + 1;
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                        int top = pivotCellInfo.CellRange.Top;
                                        while (pivotCellInfo.CellRange.Bottom != top)
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
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                        GetColorFromHex("#FF86BCF2");
                            }
                        }
                        switch (pivotCellInfo.CellType)
                        {
                            case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF86BCF2");
                                break;
                            case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF86BCF2");
                                break;
                            case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF7B7BEE");
                                break;
                            case PivotCellType.RowHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF7B7BEE");
                                break;
                            case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF7B7BEE");
                                break;
                            case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF7B7BEE7");
                                break;
                            case PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                      GetColorFromHex("#FF86BCF2");
                                break;
                            case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF86BCF2");
                                break;
                            case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF7B7BEE");
                                break;
                            case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF86BCF2");
                                break;
                            case PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF86BCF2");
                                break;
                            case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                    GetColorFromHex("#FF86BCF2");
                                break;
                        }
                        
                    }
                    docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                    cellIndex++;
                }
            }
            if (!this.GridControl.ShowSubTotals)
            {
                for (int row = 0; row < this.GridControl.PivotEngine.RowCount - 2; row++)
                {
                    int cellIndex = 0;
                    for (int c = _temp; c <= (_temp + columnCount - 2); c++)
                    {
                        PivotCellInfo pivotCellInfo = this.GridControl.PivotEngine[row, c];
                        int c1 = cellIndex;
                        if (pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && !this.GridControl.ShowSubTotals)
                        {
                            if ((this.GridControl.PivotCalculations.Count == 1))
                                subTotalColumnIndices.Add(cellIndex);
                            else if ((this.GridControl.PivotCalculations.Count > 1) && (this.GridControl.PivotCalculations.Count > 1 && !this.GridControl.PivotEngine[row, c + this.GridControl.PivotCalculations.Count].FormattedText.Contains(this.GridControl.PivotEngine.GrandString)))
                            {
                                foreach (PivotComputationInfo pc in this.GridControl.PivotCalculations)
                                {
                                    subTotalColumnIndices.Add(c1);
                                    c1++;
                                }
                            }
                            else if ((this.GridControl.PivotCalculations.Count > 1 && this.GridControl.PivotEngine[row, c + this.GridControl.PivotCalculations.Count].FormattedText.Contains(this.GridControl.PivotEngine.GrandString)))
                            {
                                foreach (PivotComputationInfo pc in this.GridControl.PivotCalculations)
                                {
                                    subTotalColumnIndices.Add(c1);
                                }
                            }
                        }
                        if (pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) && !this.GridControl.ShowSubTotals)
                        {
                            subTotalRowIndices.Add(row);
                        }
                        cellIndex++;
                    }
                }
                for (int row = 0; row < this.GridControl.PivotEngine.RowCount - 2; row++)
                {
                    int cellIndex = 0;
                    List<int> cr1 = new List<int>();
                    cr1.AddRange(subTotalRowIndices);
                    for (int i = 0; i < subTotalRowIndices.Count; i++)
                    {
                        if (subTotalRowIndices.Contains(row) && subTotalRowIndices.Count == 1 && subTotalRowIndices != null && row < docTable.Rows.Count && subTotalRowIndices[i] < docTable.Rows.Count)
                        {
                            docTable.Rows.RemoveAt(subTotalRowIndices[i]);
                        }
                        else if (cr1.Contains(row) && cr1.Count > 1 && cr1 != null && row < docTable.Rows.Count && cr1[i] < docTable.Rows.Count)
                        {
                            docTable.Rows.RemoveAt(cr1[i]);
                            for (int i1 = 0; i1 < cr1.Count; i1++)
                            {
                                if (cr1[i1] != subTotalRowIndices[i] && i1 >= i)
                                {
                                    cr1[i1]--;
                                }
                            }
                        }
                    }
                    for (int c = _temp; c <= (_temp + columnCount - 2); c++)
                    {
                        List<int> ce1 = new List<int>();
                        ce1.AddRange(subTotalColumnIndices);
                        for (int i = 0; i < subTotalColumnIndices.Count; i++)
                        {
                            int c1 = cellIndex;
                            if (subTotalColumnIndices.Contains(c1) && subTotalColumnIndices.Count == 1 && subTotalColumnIndices != null && row < docTable.Rows.Count && subTotalColumnIndices[i] < docTable.Rows[row].Cells.Count)
                            {
                                docTable.Rows[row].Cells.RemoveAt(subTotalColumnIndices[i]);
                            }
                            else if (ce1.Contains(c1) && ce1.Count > 1 && ce1 != null && row < docTable.Rows.Count && ce1[i] < docTable.Rows[row].Cells.Count)
                            {
                                docTable.Rows[row].Cells.RemoveAt(ce1[i]);
                                for (int i1 = 0; i1 < ce1.Count; i1++)
                                {
                                    if (ce1[i1] != subTotalColumnIndices[i] && i1 >= i)
                                    {
                                        ce1[i1]--;
                                    }
                                }
                            }
                        }
                        cellIndex++;
                    }
                }
            }
            return paragraph;
        }

        private Color GetColorFromHex(string myColor)
        {
            return Color.FromArgb(
                Convert.ToByte(myColor.Substring(1, 2), 16),
                Convert.ToByte(myColor.Substring(3, 2), 16),
                Convert.ToByte(myColor.Substring(5, 2), 16),
                Convert.ToByte(myColor.Substring(7, 2), 16));
        }
    }
}
