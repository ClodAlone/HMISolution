//-------------------------------------------------------------------------------------------------
// <copyright file="GroupingGridWordConverter.cs" company="Syncfusion">
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
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using Syncfusion.DocIO.DLS;
    using Syncfusion.Grouping;

    /// <summary>
    /// A helper class to export GridGroupingControl to Word.
    /// </summary>
    /// <remarks> It has support for header and footer.</remarks>
    [ToolboxItem(false)]
    public class GroupingGridWordConverter : GridWordConverterBase
    {
         /// <summary>
        /// Default Constructor.
        /// </summary>
        public GroupingGridWordConverter()
            : base()
        {
        }

        /// <summary>
        /// Initializes word converter control
        /// </summary>
        /// <param name="showHeader">if True, converts header.</param>
        /// <param name="showFooter">if True, converts footer.</param>
        public GroupingGridWordConverter(bool showHeader, bool showFooter)
            : base(showHeader, showFooter)
        {
        }

        #region Word Conversion
        /// <summary>
        /// Exports grouping grid contents to a word document.
        /// </summary>
        /// <param name="filename">The name of the file..</param>
        /// <param name="grid">The grouping grid control.</param>
        public void GroupingGridToWord(string filename, GridGroupingControl grid)
        {
            // Create a word document.
            WordDocument wordDoc = new WordDocument();
            wordDoc.AddSection();

            // DrawHeaderFooter
            DrawHeaderFooter(wordDoc, ShowHeader, ShowFooter);

            // Export parent and child tables to word.
            this.GroupingGridToWord(grid, wordDoc);

            // Save the word document.
            wordDoc.Save(filename);

            // Dispose the word document.
            wordDoc.Close();
        }

        GridTable oldTable = null;

        private bool useColumnHeaderText = false;

        /// <summary>
        /// Gets or sets a value indicating whether the HeaderText of columns should be exported. True if HeaderText should be shown; Default is false.
        /// </summary>
        public bool UseColumnHeaderText
        {
            get
            {
                return useColumnHeaderText;
            }
            set
            {
                useColumnHeaderText = value;
            }
        }

        /// <summary>
        /// Converts grouping grid to a word document.
        /// </summary>
        /// <param name="grid">The gridgrouping control.</param>
        /// <param name="doc">The word document.</param>
        private void GroupingGridToWord(GridGroupingControl grid, WordDocument doc)
        {
            this.oldTable = grid.Table;
            this.ExportElements(grid.Table.DisplayElements, doc);
        }

        /// <summary>
        /// Converts the display elements of grouping grid to word document.
        /// </summary>
        /// <param name="dispElements">The display elements in table collection.</param>
        /// <param name="doc">The word document.</param>
        private void ExportElements(DisplayElementsInTableCollection dispElements, WordDocument doc)
        {
            for (int i = 0; i < dispElements.Count; i++)
            {
                Element el = dispElements[i];
                if (this.oldTable != (GridTable)el.ParentTable)
                {
                    this.InsertEmptyRow(doc);
                    this.oldTable = el.ParentTable as GridTable;
                }

                switch (el.Kind)
                {
                    case DisplayElementKind.ColumnHeader:
                        GridColumnHeaderRow headerRow;
                        if (el is GridColumnHeaderSection)
                        {
                            GridColumnHeaderSection headerSec = el as GridColumnHeaderSection;
                            headerRow = new GridColumnHeaderRow(headerSec);
                        }
                        else
                            headerRow = (GridColumnHeaderRow)el;
                        this.ExportColumnHeader(headerRow, doc);
                        break;

                    case DisplayElementKind.Record:
                        GridRecordRow recRow;
                        if (el is GridRecord)
                        {
                            GridRecord rec = el as GridRecord;
                            recRow = new GridRecordRow(new RecordRowsPart(rec));
                        }
                        else
                        {
                            recRow = el as GridRecordRow;
                        }

                        this.ExportRecord(recRow, doc);
                        break;

                    case DisplayElementKind.NestedTable:
                        GridNestedTable nTable = (GridNestedTable)el;
                        this.ExportElements(nTable.ChildTable.DisplayElements, doc);
                        i += nTable.ChildTable.DisplayElements.Count - 1;
                        break;

                    case DisplayElementKind.Summary:
                        GridSummaryRow summary = (GridSummaryRow)el;
                        this.ExportSummaryRow(summary, doc);
                        break;
                }
            }
        }

        private void InsertEmptyRow(WordDocument doc)
        {
            WTable wTable = new WTable(doc);
            doc.LastSection.Tables.Add(wTable);
            wTable.AddRow();
            wTable.LastRow.AddCell().AddParagraph();  
            wTable.LastRow.Height = 10f;
        }

        /// <summary>
        /// Converts Column Header.
        /// </summary>
        /// <param name="hr">The grid column header.</param>
        /// <param name="doc">The word document.</param>
        private void ExportColumnHeader(GridColumnHeaderRow hr, WordDocument doc)
        {
            GridTableDescriptor table = hr.ParentTableDescriptor;
            GridVisibleColumnDescriptorCollection visColumns = table.VisibleColumns;
            GridColumnDescriptorCollection columns = table.Columns;

            WTable wTable = new WTable(doc);
            doc.LastSection.Tables.Add(wTable);
            wTable.AddRow();
            for (int i = 0; i < visColumns.Count; i++)
            {
                WTableCell cell = new WTableCell(doc);
                if (useColumnHeaderText)
                    cell.AddParagraph().AppendText(columns[visColumns[i].Name].HeaderText);
                else
                    cell.AddParagraph().AppendText(visColumns[i].Name);
                cell.Width = 100;
                wTable.LastRow.Cells.Add(cell);
            }
        }
        private void ExportSummaryRow(GridSummaryRow summary, WordDocument doc)
        {
            GridTableDescriptor table = summary.ParentTableDescriptor;
            GridSummaryRowDescriptor descriptor = summary.SummaryRowDescriptor;
            GridSummaryColumnDescriptorCollection arrColumns = descriptor.SummaryColumns;
            GridVisibleColumnDescriptorCollection visColumns = table.VisibleColumns;
            WTable wTable = new WTable(doc);
            doc.LastSection.Tables.Add(wTable);
            wTable.AddRow();

            for (int col = 0; col < visColumns.Count; col++)
            {
                WTableCell cell = new WTableCell(doc);
                for (int row = 0, len = arrColumns.Count; row < len; row++)
                {
                    GridSummaryColumnDescriptor column = arrColumns[row];
                    if (visColumns[col].Name == column.DisplayColumn.ToString())
                    {
                        cell.AddParagraph().AppendText(column.GetDisplayText(summary.ParentGroup));
                    }
                }

                if (col == 0)
                {
                    cell.AddParagraph().AppendText(descriptor.Title.ToString());
                }

                cell.Width = 100;
                wTable.LastRow.Cells.Add(cell);
            }
        }
        /// <summary>
        /// Converts grid records.
        /// </summary>
        /// <param name="rr">The grid record row.</param>
        /// <param name="doc">The word document.</param>
        private void ExportRecord(GridRecordRow rr, WordDocument doc)
        {
            GridTableDescriptor table = rr.ParentTableDescriptor;
            GridVisibleColumnDescriptorCollection visColumns = table.VisibleColumns;
            GridRecord rec = (GridRecord)rr.GetRecord();

            WTable wTable = (WTable)doc.LastSection.Tables[doc.LastSection.Tables.Count - 1];
            wTable.AddRow();
            int no_of_cells = wTable.LastRow.Cells.Count;
            for (int c = 0; c < visColumns.Count - no_of_cells; c++)
            {
                wTable.LastRow.Cells.Add(new WTableCell(doc));
            }

            for (int i = 0; i < visColumns.Count; i++)
            {
                WTableCell cell = wTable.LastRow.Cells[i];
                cell.AddParagraph().AppendText(rec.GetValue(visColumns[i].Name).ToString());
                cell.Width = 100;
            }
        }

        #endregion
    }
}
