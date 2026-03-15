//-------------------------------------------------------------------------------------------------
// <copyright file="GridWordConverter.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Text;  
    using System.ComponentModel;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.DocIO.DLS;

    /// <summary>
    /// A helper class to export Grid to Word.
    /// </summary>
    /// <remarks> It has support for header and footer.</remarks>
    [ToolboxItem(false)]
    public class GridWordConverter : GridWordConverterBase
    {
        private const int portraitWidth = 570; //Default page width in DocIO = 595.3
        private const int landScapeWidth = 820; //Default page width in DocIO = 841.9
        private const int limitWidth = 1550;//Page maximum width
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridWordConverter()
            : base()
        {
        }

        /// <summary>
        /// Initializes word converter control
        /// </summary>
        /// <param name="showHeader">if True, converts header.</param>
        /// <param name="showFooter">if True, converts footer.</param>
        public GridWordConverter(bool showHeader, bool showFooter)
            : base(showHeader, showFooter)
        {
        }

        /// <summary>
        /// Exports grid contents to a word document.
        /// </summary>
        /// <param name="filename">Name of the word file.</param>
        /// <param name="grid">The grid control</param>
        public void GridToWord(string filename, GridControlBase grid)
        {
            if (filename.Length == 0)
            {
                throw new Exception("Specify a valid file name to export");
            }

            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            // Create a new Word document
            WordDocument wordDoc = new WordDocument();
            wordDoc.AddSection();

            // DrawHeaderFooter
            DrawHeaderFooter(wordDoc, ShowHeader, ShowFooter);
            
            // Export
            int totWidth = grid.Model.ColWidths.GetTotal(0, grid.Model.ColCount);
            float width = wordDoc.LastSection.PageSetup.PageSize.Width;
            float height = wordDoc.LastSection.PageSetup.PageSize.Height;
            bool limitExceeded = false;
            int calcWidth = 0;
            int col = 0;
            int remainCol = 0;
            if (totWidth > portraitWidth)
            {
                wordDoc.LastSection.PageSetup.Orientation = PageOrientation.Landscape;
                if (totWidth > landScapeWidth && limitWidth >= totWidth)
                    wordDoc.LastSection.PageSetup.PageSize = new System.Drawing.SizeF(totWidth + wordDoc.LastSection.PageSetup.Margins.Left + wordDoc.LastSection.PageSetup.Margins.Right, height);
                else
                {
                    limitExceeded = true;
                    wordDoc.LastSection.PageSetup.PageSize = new System.Drawing.SizeF(limitWidth + wordDoc.LastSection.PageSetup.Margins.Left + wordDoc.LastSection.PageSetup.Margins.Right, height);
                    WTable table = new WTable(wordDoc);
                    wordDoc.LastSection.Tables.Add(table);                   
                    for (int k = 0; k <= grid.Model.ColCount; k++)
                    {
                        calcWidth = grid.Model.ColWidths.GetTotal(col, k);
                        if (calcWidth > limitWidth)
                        {
                            for (int i = 0; i <= grid.Model.RowCount; i++)
                            {
                                if (grid.Model.RowHeights[i] != 0)
                                {
                                    table.AddRow(true, false);
                                    for (int j = col; j < k; j++)
                                    {
                                        WTableCell cell = new WTableCell(wordDoc);
                                        cell.AddParagraph().AppendText(grid.Model[i, j].Text);
                                        table.LastRow.Cells.Add(cell);
                                        cell.Width = grid.Model.ColWidths[j];
                                    }
                                }
                            }
                            table.AddRow(false , false);
                            WTableCell emptyCell = new WTableCell(wordDoc);
                            emptyCell.CellFormat.Borders.BorderType = Syncfusion.DocIO.DLS.BorderStyle.Cleared;                           
                            table.LastRow.Cells.Add(emptyCell);
                            col = k;
                            calcWidth = 0;
                        }
                        remainCol = k;
                    }
                    int calWidth = grid.Model.ColWidths.GetTotal(col, grid.Model.ColCount);
                    if (calWidth < limitWidth)
                    {
                        for (int i = 0; i <= grid.Model.RowCount; i++)
                        {
                            if (grid.Model.RowHeights[i] != 0)
                            {
                                table.AddRow(true, false);
                                for (int j = col; j <= remainCol; j++)
                                {
                                    WTableCell cell = new WTableCell(wordDoc);
                                    cell.AddParagraph().AppendText(grid.Model[i, j].Text);
                                    table.LastRow.Cells.Add(cell);
                                    cell.Width = grid.Model.ColWidths[j];
                                }
                            }
                        }
                    }
                }
            }
            else
                wordDoc.LastSection.PageSetup.Orientation = PageOrientation.Portrait;
            if (!limitExceeded)
            {
                WTable table = new WTable(wordDoc);
                wordDoc.LastSection.Tables.Add(table);
                for (int i = 0; i <= grid.Model.RowCount; i++)
                {
                    if (grid.Model.RowHeights[i] != 0)
                    {
                        table.AddRow(true, false);
                        for (int j = 0; j <= grid.Model.ColCount; j++)
                        {
                            WTableCell cell = new WTableCell(wordDoc);
                            cell.AddParagraph().AppendText(grid.Model[i, j].Text);
                            table.LastRow.Cells.Add(cell);
                            cell.Width = grid.Model.ColWidths[j];
                        }
                    }
                }
            }

            // Save
            wordDoc.Save(filename);

            // Dispose Word document
            wordDoc.Close();
        }
    }
}
