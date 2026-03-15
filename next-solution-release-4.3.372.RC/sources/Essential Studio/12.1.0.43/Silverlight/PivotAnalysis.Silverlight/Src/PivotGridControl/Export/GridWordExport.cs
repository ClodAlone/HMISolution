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

namespace Syncfusion.Silverlight.Controls.PivotGrid
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
                                    for (int i = 0; i <= pivotCellInfo.CellRange.Right + 1; i++)
                                    {
                                        docTable.Rows[i].Cells[0].CellFormat.HorizontalMerge = CellMerge.Start;
                                        for (int j = 1; j < pivotCellInfo.CellRange.Bottom; j++)
                                        {
                                            docTable.Rows[i].Cells[j].CellFormat.HorizontalMerge = CellMerge.Continue;
                                        }
                                    }
                                    for (int i = 0; i <= pivotCellInfo.CellRange.Right; i++)
                                    {
                                        docTable.Rows[0].Cells[i].CellFormat.VerticalMerge = CellMerge.Start;
                                        for (int j = 1; j <= pivotCellInfo.CellRange.Bottom; j++)
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
                                        case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                            docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                            break;
                                        case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                            docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                            break;
                                    }
                                }
                            }

                            docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                            cellIndex++;
                        }
                    }
                    _count += 6;
                    _temp = _count - 5;
                }
            }

            else
            {
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

                if (this.GridControl.PivotEngine.ColumnCount - 6 > 0)
                {
                    docTable.ResetCells(this.GridControl.PivotEngine.RowCount, this.GridControl.PivotEngine.ColumnCount - _temp, _format, 140);
                }
                else
                {
                    docTable.ResetCells(this.GridControl.PivotEngine.RowCount, this.GridControl.PivotEngine.ColumnCount, _format, 140);
                }

                for (int row = 0; row <= this.GridControl.PivotEngine.RowCount - 1; row++)
                {
                    int cellIndex = 0;
                    for (int c =0 ; c <= this.GridControl.PivotEngine.ColumnCount - 1; c++)
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
                                    case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                        break;
                                    case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor =
                                            GetColorFromHex("#FF86BCF2");
                                        break;
                                }
                            }
                        }
                        docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                        cellIndex++;
                    }
                }
            }
            document.Save(stream, FormatType.Doc);
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
       

