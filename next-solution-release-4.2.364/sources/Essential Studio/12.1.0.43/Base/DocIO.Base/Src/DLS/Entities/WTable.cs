#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.XML;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a table in a document. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WTable
      : TextBodyItem
      , IWTable
#if !SILVERLIGHT && !WP
      , ITableWidget
#endif
    {
        #region Constants
        private const string DEF_NORMAL_STYLE = "Normal Table";
        private const int DEF_USER_STYLE_ID = 4094;
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private WRowCollection m_rows = null;
        private RowFormat m_initTableFormat;
        private float m_tableWidth = float.MinValue;
        private List<float> m_tableGrid;

        private XmlTableFormat m_xmlTblFormat;
        internal XmlTableFormat m_trackTblFormat;
#if !SILVERLIGHT && !WP
        private ITableLayoutInfo m_tableInfo;
#endif
#if (!SILVERLIGHT && !WP) || WINRT
        internal bool m_isTextBox = false;
        internal bool m_isTextBoxInTable = false;
        internal WTextBoxFormat m_textBoxFormat = null;
#endif
        internal List<float> m_trackTableGrid;
        /// <summary>
        /// The table style
        /// </summary>
        private IWTableStyle m_style;
        private bool m_applyStyleForHeaderRow = true;
        private bool m_applyStyleForLastRow;
        private bool m_applyStyleForFirstColumn = true;
        private bool m_applyStyleForLastColumn;
        private bool m_applyStyleForBandedRows = true;
        private bool m_applyStyleForBandedColumns;
        // Word-2010 specific properties
        private string m_title;
        private string m_description;
        internal bool m_bIsTableGridUpdated;
        internal bool m_isTableGridCorrupted = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Table;
            }
        }
        /// <summary>
        /// Get the table rows
        /// </summary>
        /// <value></value>
        public WRowCollection Rows
        {
            get
            {
                return m_rows;
            }
        }
        /// <summary>
        /// Gets the table formatting after ResetCells call.
        /// </summary>
        /// <value>The table format.</value>
        public RowFormat TableFormat
        {
            get
            {
                if (m_initTableFormat == null)
                {
                    m_initTableFormat = new RowFormat();
                    m_initTableFormat.SetOwner(this);
                }

                if (m_initTableFormat.IsDefault && Rows.Count > 0)
                {
                    m_initTableFormat.ImportContainer(FirstRow.RowFormat);
                    m_initTableFormat.RemoveRowSprms();
                }

                return m_initTableFormat;
            }
        }
        /// <summary>
        /// Gets the preferred width of the table.
        /// </summary>
        /// <value>The width of the preferred table.</value>
        internal PreferredWidthInfo PreferredTableWidth
        {
            get
            {
                return TableFormat.PreferredWidth;
            }
        }
        /// <summary>
        /// Gets table style name.
        /// </summary>
        /// <value></value>
        public string StyleName
        {
            get
            {
                if (m_style == null)
                {
                    return null;
                }
                return m_style.Name;
            }
        }
        /// <summary>
        /// Get last cell of the table
        /// </summary>
        /// <value></value>
        public WTableCell LastCell
        {
            get
            {
                WTableRow row = LastRow;

                if (row != null)
                {
                    int cellsCount = row.Cells.Count;
                    return (cellsCount > 0) ? row.Cells[cellsCount - 1] : null;
                }

                return null;
            }
        }
        /// <summary>
        /// Get first row of the table.
        /// </summary>
        /// <value></value>
        public WTableRow FirstRow
        {
            get
            {
                return Rows.FirstItem as WTableRow;
            }
        }
        /// <summary>
        /// Get last row of the table.
        /// </summary>
        /// <value></value>
        public WTableRow LastRow
        {
            get
            {
                return Rows.LastItem as WTableRow;
            }
        }
        /// <summary>
        /// Get table cell by row and column indexes.
        /// </summary>
        /// <value></value>
        public WTableCell this[int row, int column]
        {
            get
            {
                return Rows[row].Cells[column];
            }
        }
        /// <summary>
        /// Gets the table width
        /// </summary>
        /// <value></value>
        public float Width
        {
            get
            {
                if( m_tableWidth == float.MinValue )
                {
                    m_tableWidth = UpdateWidth();
                }

                return m_tableWidth;
            }
        }
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_rows;
            }
        }
        /// <summary>
        /// Gets the table grid.
        /// </summary>
        internal List<float> TableGrid
        {
            get
            {
                if (!m_bIsTableGridUpdated)
                    m_tableGrid = null;
                else if (m_rows != null && m_rows.Count > 0
                    && !Document.IsOpening && !Document.IsCloning
                    && (Document.ActualFormatType.ToString().Contains("Docx")
                    || Document.ActualFormatType.ToString().Contains("Word")))
                    CheckTableGrid();
                if (m_tableGrid == null)
                {
                    if (m_rows != null && m_rows.Count > 0)
                        UpdateTableGrid();
                    else
                        m_tableGrid = new List<float>();
                    m_bIsTableGridUpdated = true;
                }
                return m_tableGrid;
            }
        }
        /// <summary>
        /// Gets/sets indent from left for the table.
        /// </summary>
        public float IndentFromLeft
        {
            get
            {
                return TableFormat.LeftIndent;
            }
            set
            {
                if (value != 0)
                {
                    if (value > float.MaxValue || value < float.MinValue)
                    {
                        throw new ArgumentOutOfRangeException("IndentFromLeft",
                          "Value must be lower than " + float.MaxValue + " and larger than " + float.MinValue);
                    }
                    TableFormat.LeftIndent = value;
                    foreach (WTableRow row in Rows)
                    {
                        row.RowFormat.LeftIndent = value;
                    }
                }
            }
        }
        //#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the docx table format.
        /// </summary>
        /// <value>The docx table format.</value>
        internal XmlTableFormat DocxTableFormat
        {
            get
            {
                if (m_xmlTblFormat == null)
                {
                    m_xmlTblFormat = new XmlTableFormat(this);
                }
                return m_xmlTblFormat;
            }
            set
            {
                m_xmlTblFormat = value;
            }
        }
        /// <summary>
        /// Gets the old row format.
        /// </summary>
        /// <value>The old row format.</value>
        internal XmlTableFormat TrackTblFormat
        {
            get
            {
                if (m_trackTblFormat == null)
                {
                    m_trackTblFormat = new XmlTableFormat(this);
                }
                return m_trackTblFormat;
            }
        }
        //#endif
        /// <summary>
        /// Gets the track table grid.
        /// </summary>
        /// <value>The track table grid.</value>
        internal List<float> TrackTableGrid
        {
            get
            {
                if (m_trackTableGrid == null)
                {
                    m_trackTableGrid = new List<float>();
                }
                return m_trackTableGrid;
            }
        }

        /// <summary>
        /// Gets / sets the boolean value indicating whether to apply style for header row
        /// </summary>
        internal bool ApplyStyleForHeaderRow
        {
            get
            {
                return m_applyStyleForHeaderRow;
            }
            set
            {
                m_applyStyleForHeaderRow = value;
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating whether to apply style for last row
        /// </summary>
        internal bool ApplyStyleForLastRow
        {
            get
            {
                return m_applyStyleForLastRow;
            }
            set
            {
                m_applyStyleForLastRow = value;
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating whether to apply style for first column
        /// </summary>
        internal bool ApplyStyleForFirstColumn
        {
            get
            {
                return m_applyStyleForFirstColumn;
            }
            set
            {
                m_applyStyleForFirstColumn = value;
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating whether to apply style for last column
        /// </summary>
        internal bool ApplyStyleForLastColumn
        {
            get
            {
                return m_applyStyleForLastColumn;
            }
            set
            {
                m_applyStyleForLastColumn = value;
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating whether to apply style for banded rows
        /// </summary>
        internal bool ApplyStyleForBandedRows
        {
            get
            {
                return m_applyStyleForBandedRows;
            }
            set
            {
                m_applyStyleForBandedRows = value;
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating whether to apply style for banded columns
        /// </summary>
        internal bool ApplyStyleForBandedColumns
        {
            get
            {
                return m_applyStyleForBandedColumns;
            }
            set
            {
                m_applyStyleForBandedColumns = value;
            }
        }
        /// <summary>
        /// Gets or sets the table title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }
        /// <summary>
        /// Gets or sets the table description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is frame.
        /// </summary>
        /// <value><c>true</c> if this instance is frame; otherwise, <c>false</c>.</value>
        internal bool IsFrame
        {
            get
            {
                if (Rows.Count > 0
                    && Rows[0].Cells.Count > 0
                    && Rows[0].Cells[0].Paragraphs.Count > 0)
                {
                    return Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.IsFrame;
                }
                return false;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTable"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WTable(IWordDocument doc)
            : this(doc, false)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WTable"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="showBorder">if it specifies to show the table border, set to <c>true</c>.</param>
        public WTable(IWordDocument doc, bool showBorder)
            : base((WordDocument)doc)
        {
            m_rows = new WRowCollection(this);

            if (showBorder)
            {
                TableFormat.Borders.BorderType = BorderStyle.Single;
                TableFormat.Borders.Color = Color.Black;
                TableFormat.Borders.LineWidth = 1f;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        new public WTable Clone()
        {
            return (WTable)CloneImpl();
        }
        /// <summary>
        /// Resets rows / columns numbers.
        /// </summary>
        /// <param name="rowsNum">The rows number.</param>
        /// <param name="columnsNum">The columns number.</param>
        public void ResetCells(int rowsNum, int columnsNum)
        {
            if (rowsNum <= 0 || columnsNum <= 0)
                throw new ArgumentException("Table should have atleast 1 row and 1 column");

            float cellWidth = 0;
            WSection ownerSection = this.GetOwnerSection();
            if (ownerSection != null)
                cellWidth = ownerSection.PageSetup.ClientWidth / columnsNum;
            else if (Document.LastSection != null)
                cellWidth = Document.LastSection.PageSetup.ClientWidth / columnsNum;
            ResetCells(rowsNum, columnsNum, null, cellWidth);
        }
        /// <summary>
        /// Resets rows / columns numbers.
        /// </summary>
        /// <param name="rowsNum">The rows num.</param>
        /// <param name="columnsNum">The columns num.</param>
        /// <param name="format"></param>
        /// <param name="cellWidth">Width of the cell.</param>
        public void ResetCells(int rowsNum, int columnsNum, RowFormat format, float cellWidth)
        {
            if (rowsNum <= 0 || columnsNum <= 0)
                throw new ArgumentException("Table should have atleast 1 row and 1 column");

            if (columnsNum > 63)
                throw new ArgumentException("Not supported more than 63 cells.");


            if (format != null)
            {
                TableFormat.ClearFormatting();
                TableFormat.ImportContainer(format);
            }

            m_rows.Clear();

            if (rowsNum > 0)
            {
                WTableRow row0 = AddRow();
                --rowsNum;

                while (columnsNum > 0)
                {
                    WTableCell cell = new WTableCell(Document);
                    cell.Width = cellWidth;
                    cell.PreferredWidth.Width = cellWidth;
                    cell.PreferredWidth.WidthType = FtsWidth.Point;
                    row0.Cells.Add(cell);
                    --columnsNum;
                }

                while (rowsNum > 0)
                {
                    AddRow();
                    --rowsNum;
                }
            }
        }
        /// <summary>
        /// Applies the built-in table style.
        /// </summary>
        /// <param name="builtinStyle">The built-in table style.</param>
        public void ApplyStyle(BuiltinTableStyle builtinTableStyle)
        {
            CheckNormalStyle();
            string styleName = Style.BuiltInToName(builtinTableStyle);
            IStyle tStyle = Document.Styles.FindByName(styleName, StyleType.TableStyle) as IWTableStyle;
            if (tStyle == null)
            {
                tStyle = (IWTableStyle)Style.CreateBuiltinStyle(builtinTableStyle, Document);
                if ((tStyle as WTableStyle).StyleId > 10)
                    (tStyle as WTableStyle).StyleId = DEF_USER_STYLE_ID;
                Document.Styles.Add(tStyle);
                string styleId = tStyle.Name.Replace("Accent", "-Accent");
                Document.StyleNameIds.Add(styleId.Replace(" ", ""), tStyle.Name);
                (tStyle as WTableStyle).ApplyBaseStyle(DEF_NORMAL_STYLE);
            }
            ApplyStyle(tStyle as IWTableStyle);
        }
        /// <summary>
        /// Adds a row to table
        /// </summary>
        /// <returns></returns>
        public WTableRow AddRow()
        {
            return AddRow(true, true);
        }
        /// <summary>
        /// Adds new row to table.
        /// </summary>
        /// <param name="isCopyFormat"></param>
        /// <returns></returns>
        public WTableRow AddRow(bool isCopyFormat)
        {
            return AddRow(isCopyFormat, true);
        }
        /// <summary>
        /// Adds a row to table with copy format option
        /// </summary>
        /// <param name="isCopyFormat">Indicates whether copy format from previous row or not</param>
        /// <param name="autoPopulateCells">if specifies to populate cells automatically,s et to <c>true</c>.</param>
        /// <returns></returns>
        public WTableRow AddRow(bool isCopyFormat, bool autoPopulateCells)
        {
            WTableRow row = new WTableRow(Document);

            if (autoPopulateCells)
            {
                WTableRow lastRow = LastRow;

                if (lastRow != null)
                {
                    WTableCell cell = null;
                    for (int i = 0, cnt = lastRow.Cells.Count; i < cnt; i++)
                    {
                        cell = lastRow.Cells[i];
                        WTableCell newCell = new WTableCell(Document);
                        row.Cells.Add(newCell);
                        newCell.Width = cell.Width;

                        if (isCopyFormat)
                        {
                            newCell.CellFormat.ImportContainer(cell.CellFormat);
                        }
                    }

                    row.Height = lastRow.Height;
                }
            }

            if (isCopyFormat)
            {
                //if (FirstRow != null && FirstRow.Cells.Count == LastRow.Cells.Count)
                //    row.RowFormat.ImportContainer(TableFormat);
                //else 
                if (LastRow != null)
                    row.RowFormat.ImportContainer(LastRow.RowFormat);
                else
                    row.RowFormat.ImportContainer(TableFormat);
            }

            Rows.Add(row);
            return row;
        }
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern">Pattern</param>
        /// <param name="replace">Replace text</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, string replace)
        {
            int changesMade = 0;

            foreach (WTableRow row in Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    foreach (TextBodyItem bodyItem in cell.ChildEntities)
                    {
                        changesMade += bodyItem.Replace(pattern, replace);

                        if (Document.ReplaceFirst && changesMade > 0)
                        {
                            return changesMade;
                        }
                    }
                }
            }

            return changesMade;
        }
        /// <summary>
        /// Replaces by specified given string.
        /// </summary>
        /// <param name="given">The given text.</param>
        /// <param name="replace">The replace text.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Replace(pattern, replace);
        }
        /// <summary>
        /// Replaces by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection)
        {
            return Replace(pattern, textSelection, false);
        }
        /// <summary>
        /// Replaces by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting)
        {
            textSelection.CacheRanges();

            int count = 0;

            foreach (WTableRow row in m_rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    count += cell.Replace(pattern, textSelection, saveFormatting);

                    if (Document.ReplaceFirst && count > 0)
                    {
                        return count;
                    }
                }
            }

            return count;
        }
        /// <summary>
        /// Finds text by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public override TextSelection Find(Regex pattern)
        {
            foreach (WTableRow row in m_rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    TextSelection textSel = cell.Find(pattern);

                    if (textSel != null && textSel.Count > 0)
                    {
                        return textSel;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Applies the vertical merge for table cells.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="endRowIndex">End index of the row.</param>
        public void ApplyVerticalMerge(int columnIndex, int startRowIndex, int endRowIndex)
        {
            if (m_rows == null || m_rows.Count == 0)
            {
                throw new Exception("Table rows are not initialized.");
            }
            if (startRowIndex < 0 || startRowIndex >= m_rows.Count)
            {
                throw new ArgumentOutOfRangeException("startRowIndex", "Row with specified row index doesn't exist");
            }
            if (endRowIndex < 0 || endRowIndex >= m_rows.Count)
            {
                throw new ArgumentOutOfRangeException("endRowIndex", "Row with specified row index doesn't exist");
            }
            if (startRowIndex > endRowIndex)
            {
                throw new Exception("Start row index is greater than end row index.");
            }
            if (columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex", "Column with specified column index doesn't exist");
            }

            for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++)
            {
                if (columnIndex >= m_rows[rowIndex].Cells.Count)
                {
                    throw new ArgumentOutOfRangeException("columnIndex", "Column with specified column index doesn't exist");
                }
            }

            m_rows[startRowIndex].Cells[columnIndex].CellFormat.VerticalMerge = CellMerge.Start;
            for (int rowIndex = startRowIndex + 1; rowIndex <= endRowIndex; rowIndex++)
            {
                m_rows[rowIndex].Cells[columnIndex].CellFormat.VerticalMerge = CellMerge.Continue;
            }
        }
        /// <summary>
        /// Applies horizontal merging for cells of table row.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startCellIndex">Start index of the cell.</param>
        /// <param name="endCellIndex">End index of the cell.</param>
        public void ApplyHorizontalMerge(int rowIndex, int startCellIndex, int endCellIndex)
        {
            if (m_rows == null || m_rows.Count == 0)
            {
                throw new Exception("Table rows are not initialized.");
            }
            if (rowIndex < 0 || rowIndex >= m_rows.Count)
            {
                throw new ArgumentOutOfRangeException("rowIndex", "Row with specified row index doesn't exist");
            }
            WCellCollection cells = (m_rows[rowIndex] as WTableRow).Cells;
            if (cells == null || cells.Count == 0)
            {
                throw new Exception("Table row cells are not initialized.");
            }
            if (startCellIndex < 0 || startCellIndex > cells.Count - 1)
            {
                throw new ArgumentOutOfRangeException("startCellIndex", "Cell with specified start cell index doesn't exist");
            }
            if (endCellIndex < 0 || endCellIndex > cells.Count - 1)
            {
                throw new ArgumentOutOfRangeException("endCellIndex", "Cell with specified end cell index doesn't exist");
            }
            if (startCellIndex > endCellIndex)
            {
                throw new Exception("Start cell index is greater than end cell index.");
            }

            cells[startCellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
            for (int cellIndex = startCellIndex + 1; cellIndex <= endCellIndex; cellIndex++)
            {
                cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Continue;
            }
        }
        /// <summary>
        /// Removes the absolute position data. If table has absolute position in the document,
        /// all position data will be erased.  
        /// </summary>
        public void RemoveAbsPosition()
        {
            foreach (WTableRow row in m_rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    foreach (TextBodyItem item in cell.Items)
                    {
                        if (item is WParagraph)
                        {
                            (item as WParagraph).RemoveAbsPosition();
                        }
                        else if (item is WTable)
                        {
                            (item as WTable).RemoveAbsPosition();
                        }
                    }
                }
                if (row.RowFormat != null)
                {
                    row.RowFormat.RemovePositioning();
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the table text.
        /// </summary>
        /// <returns></returns>
        internal string GetTableText()
        {
            string text = string.Empty;
            foreach (WTableRow row in Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    for (int i = 0; i < cell.Items.Count; i++)
                    {
                        if (cell.Items[i] is WParagraph)
                            text += (cell.Items[i] as WParagraph).GetParagraphText();
                        else if (cell.Items[i] is WTable)
                            text += (cell.Items[i] as WTable).GetTableText();
                        if (Document.m_prevClonedEntity != null
                            && Document.m_prevClonedEntity.OwnerTextBody == cell)
                        {
                            i = Document.m_prevClonedEntity.GetIndexInOwnerCollection();
                            Document.m_prevClonedEntity = null;
                        }
                        if (i == cell.Items.Count - 1
                            && cell.CellFormat.CurCellIndex < row.Cells.Count - 1)
                        {
                            text = text.Substring(0, text.Length - 1);
                            text += ControlChar.Tab;
                        }
                    }
                }
            }
            return text;
        }
        /// <summary>
        /// Checks the normal style.
        /// </summary>
        private void CheckNormalStyle()
        {
            WTableStyle tStyle = Document.Styles.FindByName("Normal Table", StyleType.TableStyle) as WTableStyle;
            if (tStyle == null)
            {
                tStyle = (WTableStyle)Style.CreateBuiltinStyle(BuiltinTableStyle.TableNormal, Document);
                Document.Styles.Add(tStyle);
                Document.StyleNameIds.Add("TableNormal", tStyle.Name);
            }
        }
        /// <summary>
        /// Gets related style.
        /// </summary>
        internal IWTableStyle GetStyle()
        {
            return m_style;
        }
        /// <summary>
        /// Applies the specified style.
        /// </summary>
        /// <param name="style">Style name</param>
        /// <remarks>Specified style must exist in Document.Styles collection</remarks>
        private void ApplyStyle(IWTableStyle style)
        {
            if (style == null)
                throw new ArgumentNullException("newStyle");

            m_style = style;
        }
        /// <summary>
        /// Applies the specified style.
        /// </summary>
        /// <param name="style">Style name</param>
        /// <remarks>Specified style must exist in Document.Styles collection</remarks>
        internal void ApplyStyle(string styleName)
        {
            IWTableStyle style = Document.Styles.FindByName(styleName, StyleType.TableStyle) as IWTableStyle;
            if (style == null)
                throw new ArgumentNullException("newStyle");

            ApplyStyle(style);
        }
        /// <summary>
        /// Applies the base style formats.
        /// </summary>
        internal void ApplyBaseStyleFormats()
        {
            if (m_style != null)
            {
                TableFormat.ApplyBase(m_style.TableProperties.GetAsTableFormat());
                for (int i = 0; i < Rows.Count; i++)
                {
                    int rowIndex = i + 1;
                    ConditionalFormattingStyle cnfStyle = null;
                    bool isOddRow = ((ApplyStyleForHeaderRow
                        && (m_style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.FirstRow))
                        ? ((i != 0) ? (((i - 1) / m_style.TableProperties.RowStripe + 1) % 2 == 1) : false)
                        : ((rowIndex / m_style.TableProperties.RowStripe) % 2 == 1));

                    foreach (KeyValuePair<ConditionalFormattingCode, ConditionalFormattingStyle> keyPair in (m_style as WTableStyle).ConditionalFormattingStyles)
                    {
                        switch (keyPair.Key)
                        {
                            case ConditionalFormattingCode.FirstRow:
                                if (i == 0 && ApplyStyleForHeaderRow)
                                    cnfStyle = keyPair.Value;
                                break;
                            case ConditionalFormattingCode.LastRow:
                                if (i != 0 && i == Rows.Count - 1 && ApplyStyleForLastRow)
                                    cnfStyle = keyPair.Value;
                                break;
                            case ConditionalFormattingCode.OddRowBanding:
                                if ((i != Rows.Count - 1 || !ApplyStyleForLastRow) && isOddRow && ApplyStyleForBandedRows)
                                    cnfStyle = keyPair.Value;
                                break;
                            case ConditionalFormattingCode.EvenRowBanding:
                                if (i != 0 && i != Rows.Count - 1 && !isOddRow && ApplyStyleForBandedRows)
                                    cnfStyle = keyPair.Value;
                                break;
                        }
                    }
                    if (cnfStyle != null)
                    {
                        Rows[i].RowFormat.ApplyBase(cnfStyle.RowProperties.GetAsRowFormat());
                    }
                    else
                        Rows[i].RowFormat.ApplyBase(m_style.RowProperties.GetAsRowFormat());

                    for (int j = 0; j < Rows[i].Cells.Count; j++)
                    {
                        int cellIndex = j + 1;
                        bool isOddColumn = ((ApplyStyleForFirstColumn
                            && (m_style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.FirstColumn))
                            ? ((j != 0) ? (((j - 1) / m_style.TableProperties.ColumnStripe + 1) % 2 == 1) : false)
                            : ((cellIndex / m_style.TableProperties.ColumnStripe) % 2 == 1));

                        WParagraphFormat pFormat = new WParagraphFormat(Document);
                        WCharacterFormat cFormat = new WCharacterFormat(Document);
                        CellFormat cellformat = new CellFormat();
                        pFormat.CopyFormat(m_style.ParagraphFormat);
                        cFormat.CopyFormat(m_style.CharacterFormat);
                        cellformat.UpdateCellFormat(m_style.CellProperties);
                        if (cnfStyle != null)
                        {
                            cellformat.UpdateCellFormat(cnfStyle.CellProperties);
                            UpdateRowBorders(cellformat.Borders, cnfStyle.CellProperties.Borders, TableFormat.Borders, j, Rows[i].Cells.Count);
                            pFormat.CopyFormat(cnfStyle.ParagraphFormat);
                            cFormat.CopyFormat(cnfStyle.CharacterFormat);
                        }
                        ConditionalFormattingStyle cnfStyle1 = null;
                        foreach (KeyValuePair<ConditionalFormattingCode, ConditionalFormattingStyle> keyPair in (m_style as WTableStyle).ConditionalFormattingStyles)
                        {
                            switch (keyPair.Key)
                            {
                                case ConditionalFormattingCode.FirstColumn:
                                    if (j == 0 && ApplyStyleForFirstColumn)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.LastColumn:
                                    if (j != 0 && j == Rows[i].Cells.Count - 1 && ApplyStyleForLastColumn)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.OddColumnBanding:
                                    if (j != Rows[i].Cells.Count - 1 && isOddColumn && ApplyStyleForBandedColumns)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.EvenColumnBanding:
                                    if (j != 0 && j != Rows[i].Cells.Count - 1 && !isOddColumn && ApplyStyleForBandedColumns)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.FirstRowLastCell:
                                    if (i == 0 && j != 0 && j == Rows[i].Cells.Count - 1 && ApplyStyleForHeaderRow && ApplyStyleForLastColumn)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.FirstRowFirstCell:
                                    if (i == 0 && j == 0 && ApplyStyleForHeaderRow && ApplyStyleForFirstColumn)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.LastRowLastCell:
                                    if (i != 0 && i == Rows.Count - 1 && j != 0 && j == Rows[i].Cells.Count - 1 && ApplyStyleForLastRow && ApplyStyleForLastColumn)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                                case ConditionalFormattingCode.LastRowFirstCell:
                                    if (i != 0 && i == Rows.Count - 1 && j == 0 && ApplyStyleForLastRow && ApplyStyleForFirstColumn)
                                        cnfStyle1 = keyPair.Value;
                                    break;
                            }
                            if (cnfStyle1 != null)
                            {
                                cellformat.UpdateCellFormat(cnfStyle1.CellProperties);
                                UpdateColumnBorders(cellformat.Borders, cnfStyle1.CellProperties.Borders, TableFormat.Borders, i, Rows.Count);
                                pFormat.CopyFormat(cnfStyle1.ParagraphFormat);
                                cFormat.CopyFormat(cnfStyle1.CharacterFormat);
                                if (cnfStyle != null)
                                {
                                    if (cnfStyle.ConditionalFormattingType == ConditionalFormattingCode.FirstRow)
                                    {
                                        pFormat.CopyFormat(cnfStyle.ParagraphFormat);
                                        cFormat.CopyFormat(cnfStyle.CharacterFormat);
                                    }
                                    cellformat.UpdateCellFormat(cnfStyle.CellProperties);
                                    UpdateRowBorders(cellformat.Borders, cnfStyle.CellProperties.Borders, TableFormat.Borders, j, Rows[i].Cells.Count);
                                }
                                switch (cnfStyle1.ConditionalFormattingType)
                                {
                                    case ConditionalFormattingCode.FirstColumn:
                                        if (cnfStyle == null || cnfStyle.ConditionalFormattingType != ConditionalFormattingCode.FirstRow)
                                        {
                                            cellformat.UpdateCellFormat(cnfStyle1.CellProperties);
                                            UpdateColumnBorders(cellformat.Borders, cnfStyle1.CellProperties.Borders, TableFormat.Borders, i, Rows.Count);
                                        }
                                        break;
                                    case ConditionalFormattingCode.FirstRowFirstCell:
                                    case ConditionalFormattingCode.FirstRowLastCell:
                                    case ConditionalFormattingCode.LastRowFirstCell:
                                    case ConditionalFormattingCode.LastRowLastCell:
                                        cellformat.CopyFormat(cnfStyle1.CellProperties);
                                        pFormat.CopyFormat(cnfStyle1.ParagraphFormat);
                                        cFormat.CopyFormat(cnfStyle1.CharacterFormat);
                                        break;
                                }
                            }
                        }
                        Rows[i].Cells[j].ApplyTableStyleBaseFormats(cellformat, pFormat, cFormat);
                    }
                }
            }
        }
        /// <summary>
        /// Updates the row borders.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="src">The src.</param>
        /// <param name="tableBorders">The table borders.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        private void UpdateRowBorders(Borders dest, Borders src, Borders tableBorders, int index, int count)
        {
            if (!src.NoBorder)
            {
                dest.Top.CopyBorderFormatting(src.Top);
                dest.Bottom.CopyBorderFormatting(src.Bottom);
                if (index == 0)
                    dest.Left.CopyBorderFormatting(src.Left);
                if (index == count - 1)
                    dest.Right.CopyBorderFormatting(src.Right);
                if (src.Vertical.HasValue(Border.BorderTypeKey))
                {
                    if (index < count - 1)
                    {
                        dest.Right.CopyBorderFormatting(src.Vertical);
                        if (src.Vertical.BorderType == BorderStyle.Cleared && tableBorders.Vertical.BorderType != BorderStyle.None)
                            dest.Right.CopyBorderFormatting(tableBorders.Vertical);
                    }
                    if (index > 0 && index < count)
                    {
                        dest.Left.CopyBorderFormatting(src.Vertical);
                        if (src.Vertical.BorderType == BorderStyle.Cleared && tableBorders.Vertical.BorderType != BorderStyle.None)
                            dest.Left.CopyBorderFormatting(tableBorders.Vertical);
                    }
                }
            }
        }
        /// <summary>
        /// Updates the column borders.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="src">The src.</param>
        /// <param name="tableBorders">The table borders.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        private void UpdateColumnBorders(Borders dest, Borders src, Borders tableBorders, int index, int count)
        {
            if (!src.NoBorder)
            {
                dest.Left.CopyBorderFormatting(src.Left);
                dest.Right.CopyBorderFormatting(src.Right);
                if (index == 0)
                    dest.Top.CopyBorderFormatting(src.Top);
                if (index == count - 1)
                    dest.Bottom.CopyBorderFormatting(src.Bottom);
                if (src.Horizontal.HasValue(Border.BorderTypeKey))
                {
                    if (index < count - 1)
                    {
                        dest.Bottom.CopyBorderFormatting(src.Horizontal);
                        if (src.Horizontal.BorderType == BorderStyle.Cleared && tableBorders.Horizontal.BorderType != BorderStyle.None)
                            dest.Bottom.CopyBorderFormatting(tableBorders.Vertical);
                    }
                    if (index > 0 && index < count)
                    {
                        dest.Top.CopyBorderFormatting(src.Horizontal);
                        if (src.Horizontal.BorderType == BorderStyle.Cleared && tableBorders.Horizontal.BorderType != BorderStyle.None)
                            dest.Top.CopyBorderFormatting(tableBorders.Vertical);
                    }
                }
            }
        }
        /// <summary>
        /// Returns all entries of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal override TextSelectionList FindAll(Regex pattern)
        {
            TextSelectionList allSelections = null;

            foreach (WTableRow row in m_rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    TextSelectionList selections = cell.FindAll(pattern);

                    if (selections != null && selections.Count > 0)
                    {
                        if (allSelections == null)
                        {
                            allSelections = selections;
                        }
                        else
                        {
                            allSelections.AddRange(selections);
                        }
                    }
                }
            }

            return allSelections;
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            foreach (WTableRow row in Rows)
            {
                row.AddSelf();
            }
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            WTable table = (WTable)base.CloneImpl();
            table.m_rows = new WRowCollection(table);

            table.m_initTableFormat = null;
            table.m_tableGrid = new List<float>(TableGrid);
            table.TableFormat.ImportContainer(TableFormat);
            table.TableFormat.SetOwner(table);
            if (m_xmlTblFormat != null)
                table.m_xmlTblFormat = m_xmlTblFormat.Clone(table);

            Rows.CloneTo(table.m_rows);

            IWTableStyle style = GetStyle();

            if (style != null)
            {
                WTableStyle clonedStyle = style.Clone() as WTableStyle;
                table.ApplyStyle(clonedStyle);
            }
            return table;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            if (doc.ImportStyles)
                CloneStyleTo(doc);
            Entity ent = null;
            for (int i = 0, cnt = ChildEntities.Count; i < cnt; i++)
            {
                ent = ChildEntities[i];
                ent.CloneRelationsTo(doc, nextOwner);
            }
        }
        /// <summary>
        /// Clones the style.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void CloneStyleTo(WordDocument doc)
        {
            if (m_style != null)
            {
                IStyle foundStyle = doc.Styles.FindByName(m_style.Name, StyleType.TableStyle);

                // Export style object
                if (foundStyle == null)
                {
                    (m_style as Style).ImportStyleTo(doc);
                }
                else
                {
                    if (doc.CurClonedSection != null)
                    {
                        m_style = (WTableStyle)(m_style as Style).ApplyOrImportStyleTo(doc, foundStyle);
                        ApplyStyle(m_style);
                    }
                }
            }
        }
        /// <summary>
        /// Checks the table grid, after parsing Docx format documents.
        /// </summary>
        private void CheckTableGrid()
        {
            if (m_tableGrid != null
                && (int)PreferredTableWidth.WidthType >= 2)
            {
                float expectedWidth = GetTableClientWidth();
                expectedWidth = (float)Math.Round(expectedWidth * DLSConstants.TwipsInOnePoint);
                float currentWidth = m_tableGrid[m_tableGrid.Count - 1] - m_tableGrid[0];
                //Updates the table grid, if the width(from grid) and preferred width are not same.
                if (expectedWidth != currentWidth)
                    UpdateTableGrid();
            }
            else if (m_isTableGridCorrupted)// update the table grid if it is corrupted.
            {
                UpdateTableGrid();
                m_isTableGridCorrupted = false;
            }
        }
        /// <summary>
        /// Updates the table grid.
        /// </summary>
        private void UpdateTableGrid()
        {
            m_tableGrid = new List<float>();
            float clientWidth = GetTableClientWidth();
            float maxRowWidth = GetMaxRowWidth();
            // Build grid columns with offsets ---
            foreach (WTableRow row in Rows)
            {
                float currOffset = 0;
                UpdateTableGrid(currOffset);
                if (row.RowFormat.GridBeforeWidth.Width > 0)
                {
                    currOffset += GetGridBeforeAfter(row.RowFormat.GridBeforeWidth, clientWidth);
                    UpdateTableGrid(currOffset);
                }
                if ((int)PreferredTableWidth.WidthType >= 2
                    && PreferredTableWidth.Width > 0)
                    UpdateCellWidth(row, clientWidth, maxRowWidth);
                foreach (WTableCell cell in row.Cells)
                {
                    currOffset += GetCellWidth(cell, clientWidth, currOffset, maxRowWidth);
                    UpdateTableGrid(currOffset);
                }
                if (row.RowFormat.GridAfterWidth.Width > 0)
                {
                    currOffset += GetGridBeforeAfter(row.RowFormat.GridAfterWidth, clientWidth);
                    UpdateTableGrid(currOffset);
                }
            }
        }
        /// <summary>
        /// Gets the width of the max row.
        /// </summary>
        /// <returns></returns>
        private float GetMaxRowWidth()
        {
            float clientWidth = GetOwnerWidth();
            float maxRowWidth = clientWidth;
            foreach (WTableRow row in Rows)
            {
                float rowWidth = row.GetWidthToResizeCells(row, clientWidth);
                if (rowWidth > maxRowWidth)
                    maxRowWidth = rowWidth;
            }
            return maxRowWidth;
        }
        /// <summary>
        /// Updates the width of the cell.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="tableWidth">Width of the table.</param>
        /// <param name="maxRowWidth">Width of the max row.</param>
        private void UpdateCellWidth(WTableRow row, float tableWidth, float maxRowWidth)
        {
            float currOffset = 0;
            if (row.RowFormat.GridBeforeWidth.Width > 0)
                currOffset += GetGridBeforeAfter(row.RowFormat.GridBeforeWidth, tableWidth);
            if (row.RowFormat.GridAfterWidth.Width > 0)
                currOffset += GetGridBeforeAfter(row.RowFormat.GridAfterWidth, tableWidth);
            //Gets the total width of cells calculated from table grid.
            float width = row.GetRowWidth();
            width = (float)Math.Round(width * DLSConstants.TwipsInOnePoint);
            currOffset += width;

            if (!TableFormat.IsAutoResized)
                //Difference: Preferred width - width from table grid.
                currOffset += row.GetRowPreferredWidth(tableWidth) - width;
            if (currOffset != (float)Math.Round(tableWidth * DLSConstants.TwipsInOnePoint))
            {
                float factor = (float)Math.Round(tableWidth * DLSConstants.TwipsInOnePoint) / currOffset;
                if (TableFormat.IsAutoResized)
                {
                    //Need to do: Handle resizing of width automatically based on the contents.
                    float clientWidth = GetOwnerWidth();
                    //Checks if the total width exceeds the page/parent cell client width.
                    if (currOffset > (float)Math.Round(clientWidth * DLSConstants.TwipsInOnePoint)
                        && (float)Math.Round(maxRowWidth * DLSConstants.TwipsInOnePoint) != currOffset)
                    {
                        factor = (float)Math.Round((tableWidth > maxRowWidth ? tableWidth : maxRowWidth) * DLSConstants.TwipsInOnePoint) / currOffset;
                        //Updates the cell width (table grid) based on the factor, if the total width exceeds the page/parent cell client width.
                        foreach (WTableCell cell in row.Cells)
                        {
                            cell.CellFormat.CellWidth = (float)Math.Round(cell.CellFormat.CellWidth * factor, 2);
                        }
                    }
                }
                else
                {
                    //Updates the cell preferred width based on the factor, if the total width exceeds the preferred width.
                    foreach (WTableCell cell in row.Cells)
                    {
                        if (cell.PreferredWidth.WidthType == FtsWidth.Point)
                            cell.CellFormat.CellWidth = (float)Math.Round(cell.PreferredWidth.Width * factor, 2);
                        else if (cell.PreferredWidth.WidthType == FtsWidth.Percentage)
                            cell.CellFormat.CellWidth = (float)Math.Round((tableWidth * cell.PreferredWidth.Width / DLSConstants.HundredthsUnit) * factor, 2);
                    }
                }
            }
            else if (!TableFormat.IsAutoResized)
            {
                //Updates the preferred width for Table grid calculation.
                foreach (WTableCell cell in row.Cells)
                {
                    if (cell.PreferredWidth.WidthType == FtsWidth.Point)
                        cell.CellFormat.CellWidth = cell.PreferredWidth.Width;
                    else if (cell.PreferredWidth.WidthType == FtsWidth.Percentage)
                        cell.CellFormat.CellWidth = (tableWidth * cell.PreferredWidth.Width / DLSConstants.HundredthsUnit);
                }
            }
        }
        /// <summary>
        /// Gets the grid before after.
        /// </summary>
        /// <param name="widthInfo">The width info.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <returns></returns>
        private float GetGridBeforeAfter(PreferredWidthInfo widthInfo, float clientWidth)
        {
            float gridValue = 0;
            if (widthInfo.WidthType == FtsWidth.Point)
                gridValue = (float)Math.Round(widthInfo.Width * DLSConstants.TwipsInOnePoint);
            else if (widthInfo.WidthType == FtsWidth.Percentage)
                gridValue = (float)Math.Round(clientWidth * widthInfo.Width / 5);
            return gridValue;
        }
        /// <summary>
        /// Gets the width of the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="tableWidth">Width of the table.</param>
        /// <param name="currOffset">The curr offset.</param>
        /// <param name="maxRowWidth">Width of the max row.</param>
        /// <returns></returns>
        private float GetCellWidth(WTableCell cell, float tableWidth, float currOffset, float maxRowWidth)
        {
            float width = (float)Math.Round(cell.Width * DLSConstants.TwipsInOnePoint);
            float preferredWidth = 0;
            if (!TableFormat.IsAutoResized && ((int)PreferredTableWidth.WidthType < 2 || PreferredTableWidth.Width == 0))
            {
                if (cell.PreferredWidth.WidthType == FtsWidth.Point)
                    preferredWidth = (float)Math.Round(cell.PreferredWidth.Width * DLSConstants.TwipsInOnePoint);
                else if (cell.PreferredWidth.WidthType == FtsWidth.Percentage)
                    preferredWidth = (float)Math.Round(tableWidth * cell.PreferredWidth.Width / 5);
            }
            //Updates the Cell width in grid, if preferredWidth is maximum.
            if (width < preferredWidth)
            {
                cell.CellFormat.CellWidth = preferredWidth / DLSConstants.TwipsInOnePoint;
                width = preferredWidth;
            }
            if (((int)PreferredTableWidth.WidthType >= 2
                && PreferredTableWidth.Width > 0)
                && (float)Math.Round(tableWidth * DLSConstants.TwipsInOnePoint) < currOffset + width)
            {
                //Adjusts the width of the cell, based on remaining width.
                if (TableFormat.IsAutoResized)
                {
                    //Updates the cell width (table grid) based on remaining width, if the total width exceeds the page/parent cell client width.
                    float clientWidth = GetOwnerWidth();
                    if (currOffset + width > (float)Math.Round(clientWidth * DLSConstants.TwipsInOnePoint)
                        && (float)Math.Round(maxRowWidth * DLSConstants.TwipsInOnePoint) != currOffset + width)
                    {
                        width = (float)Math.Round(clientWidth * DLSConstants.TwipsInOnePoint) - currOffset;
                        cell.CellFormat.CellWidth = width / DLSConstants.TwipsInOnePoint;
                    }
                }
                else
                {
                    //Updates the cell width based on remaining width, if the total width exceeds the preferred width.
                    width = (float)Math.Round(tableWidth * DLSConstants.TwipsInOnePoint) - currOffset;
                    cell.CellFormat.CellWidth = width / DLSConstants.TwipsInOnePoint;
                }
            }

            return width;
        }
        /// <summary>
        /// Gets the width of the table owner.
        /// </summary>
        /// <returns></returns>
        internal float GetOwnerWidth()
        {
            float width = 0;
            if (Owner is WTableCell)
            {
                WTableCell ownerCell = Owner as WTableCell;
                width = ownerCell.Width;
                WTable ownerTable = ownerCell.OwnerRow.OwnerTable;
                float leftPad = ownerCell.CellFormat.Paddings.Left;
                if (ownerCell.CellFormat.SamePaddingsAsTable)
                {
                    if (ownerTable.TableFormat.Paddings.HasKey(Paddings.LeftKey))
                        leftPad = ownerTable.TableFormat.Paddings.Left;
                    //Doc format document can have default cell margin value as zero.
                    else if (Document.ActualFormatType != FormatType.Doc)
                        leftPad = 5.4f;
                }
                float rightPad = ownerCell.CellFormat.Paddings.Right;
                if (ownerCell.CellFormat.SamePaddingsAsTable)
                {
                    if (ownerTable.TableFormat.Paddings.HasKey(Paddings.RightKey))
                        rightPad = ownerTable.TableFormat.Paddings.Right;
                    //Doc format document can have default cell margin value as zero.
                    else if (Document.ActualFormatType != FormatType.Doc)
                        rightPad = 5.4f;
                }
                if (ownerTable.TableFormat.CellSpacing > 0)
                {
                    leftPad += ownerTable.TableFormat.CellSpacing * 2 + TableFormat.Borders.Left.LineWidth;
                    rightPad += ownerTable.TableFormat.CellSpacing * 2 + TableFormat.Borders.Right.LineWidth;
                }
                //Calculates the cell client width reducing padding and cell spacing.
                width -= leftPad + rightPad;
            }
            else
            {
                WSection ownerSection = this.GetOwnerSection();
                if (ownerSection != null)
                {
                    if (ownerSection.Columns.Count > 1)
                        width = ownerSection.Columns[0].Width;
                    else
                        width = ownerSection.PageSetup.ClientWidth;
                }
                else if (Document.LastSection != null)
                    width = Document.LastSection.PageSetup.ClientWidth;
            }
            return width;
        }
        /// <summary>
        /// Gets the width of the table client.
        /// </summary>
        /// <returns></returns>
        internal float GetTableClientWidth()
        {
            float clientWidth = 0;
            if (PreferredTableWidth.WidthType == FtsWidth.Point
                && PreferredTableWidth.Width > 0)
                clientWidth = PreferredTableWidth.Width;
            else
            {
                clientWidth = GetOwnerWidth();
                if (PreferredTableWidth.WidthType == FtsWidth.Percentage
                    && PreferredTableWidth.Width > 0)
                {
                    clientWidth = clientWidth * PreferredTableWidth.Width / DLSConstants.HundredthsUnit;
                    if (OwnerTextBody is WTableCell)
                    {
                        float leftLineWidth = Rows[0].Cells[0].CellFormat.Borders.Left.LineWidth;
                        if (leftLineWidth < TableFormat.Borders.Left.LineWidth)
                            leftLineWidth = TableFormat.Borders.Left.LineWidth;
                        float rightLineWidth = Rows[0].Cells[Rows[0].Cells.Count - 1].CellFormat.Borders.Right.LineWidth;
                        if (rightLineWidth < TableFormat.Borders.Right.LineWidth)
                            rightLineWidth = TableFormat.Borders.Right.LineWidth;
                        clientWidth -= leftLineWidth / 2 + rightLineWidth / 2;
                    }
                    else
                    {
                        float leftPad = Rows[0].Cells[0].CellFormat.Paddings.Left;
                        if (Rows[0].Cells[0].CellFormat.SamePaddingsAsTable)
                        {
                            if (TableFormat.Paddings.HasKey(Paddings.LeftKey))
                                leftPad = TableFormat.Paddings.Left;
                            //Doc format document can have default cell margin value as zero.
                            else if (Document.ActualFormatType != FormatType.Doc)
                                leftPad = 5.4f;
                        }
                        float rightPad = Rows[0].Cells[Rows[0].Cells.Count - 1].CellFormat.Paddings.Right;
                        if (Rows[0].Cells[Rows[0].Cells.Count - 1].CellFormat.SamePaddingsAsTable)
                        {
                            if (TableFormat.Paddings.HasKey(Paddings.RightKey))
                                rightPad = TableFormat.Paddings.Right;
                            //Doc format document can have default cell margin value as zero.
                            else if (Document.ActualFormatType != FormatType.Doc)
                                rightPad = 5.4f;
                        }
                        clientWidth += leftPad + rightPad;
                        if (TableFormat.CellSpacing > 0)
                        {
                            clientWidth += TableFormat.CellSpacing * 2 + TableFormat.Borders.Left.LineWidth;
                            clientWidth += TableFormat.CellSpacing * 2 + TableFormat.Borders.Right.LineWidth;
                        }
                    }
                }
            }
            return clientWidth;
        }
        /// <summary>
        /// Updates the table grid.
        /// </summary>
        /// <param name="currOffset">The current offset.</param>
        private void UpdateTableGrid(float currOffset)
        {
            if (m_tableGrid.IndexOf(currOffset) < 0)
            {
                if (m_tableGrid.Count > 0)
                {
                    for (int i = 0, len = m_tableGrid.Count; i < len; i++)
                    {
                        float offset = m_tableGrid[i];

                        if (offset > currOffset)
                        {
                            m_tableGrid.Insert(i, currOffset);
                            break;
                        }
                        else if (len == i + 1)
                        {
                            m_tableGrid.Add(currOffset);
                        }
                    }
                }
                else
                {
                    m_tableGrid.Add(currOffset);
                }
            }
        }
        /// <summary>
        /// Gets the next TextBodyItem in the document.
        /// </summary>
        /// <returns></returns>
        internal override TextBodyItem GetNextTextBodyItem()
        {
            if (this.NextSibling == null)
            {
                if (this.OwnerTextBody is WTableCell)
                {
                    (this.OwnerTextBody as WTableCell).GetNextTextBodyItem();
                }
                else if (this.OwnerTextBody != null)
                {
                    GetNextInSection(this.OwnerTextBody.Owner as WSection);
                }
                return null;
            }
            else
            {
                return this.NextSibling as TextBodyItem;
            }
        }
        /// <summary>
        /// Updates the table format.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        internal void UpdateFormat(FormatBase format, int propKey)
        {
            if (format is RowFormat)
            {
                foreach (WTableRow row in Rows)
                {
                    row.RowFormat.SetPropertyValue(propKey, TableFormat.GetPropertyValue(propKey));
                }
            }
            else if (format is Border || format is Borders)
            {
                foreach (WTableRow row in Rows)
                {
                    row.RowFormat.Borders.ImportContainer(TableFormat.Borders);
                }
            }
            else if (format is Paddings)
            {
                foreach (WTableRow row in Rows)
                {
                    row.RowFormat.Paddings.ImportContainer(TableFormat.Paddings);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override void Close()
        {
            if (m_rows != null && m_rows.Count > 0)
            {
                int cnt = m_rows.Count;
                WTableRow row = null;
                for (int i = 0; i < cnt; i++)
                {
                    row = m_rows[i];
                    row.Close();
                    row = null;
                }

                m_rows.Clear();
                m_rows = null;
            }

            if (m_initTableFormat != null)
                m_initTableFormat.Close();

            if (m_tableGrid != null)
            {
                m_tableGrid.Clear();
                m_tableGrid = null;
            }
            //#if !SILVERLIGHT
            if (m_xmlTblFormat != null)
            {
                m_xmlTblFormat.Close();
                m_xmlTblFormat = null;
            }
            //#endif

        }
        /// <summary>
        /// Updates the width.
        /// </summary>
        /// <returns></returns>
        internal float UpdateWidth()
        {
            float width = 0f;
            if (Rows.Count > 0
                && TableGrid != null)
            {
                float curWidth = 0f;

                WTableCell cell = null;

                for (int index = 0, counter = Rows.Count; index < counter; index++)
                {
                    curWidth = 0f;
                    int gridCount = 0;
                    for (int i = 0, cnt = Rows[index].Cells.Count; i < cnt; i++)
                    {
                        cell = Rows[index].Cells[i];
                        //If cell width is explicitly defined beyound measurement units (0 - 1584 pt).
                        if (cell.Width > 1584)
                        {
                            if (gridCount + cell.GridSpan < m_tableGrid.Count)
                                curWidth += (m_tableGrid[gridCount + cell.GridSpan] - m_tableGrid[gridCount]) / DLSConstants.TwipsInOnePoint;
                            else
                                curWidth += 1584;
                        }
                        else
                            curWidth += cell.Width;
                        gridCount += cell.GridSpan;
                    }
                    if (this.Document != null && this.Document.GrammarSpellingData == null &&
                      (Rows[index] as WTableRow).RowFormat.HorizontalAlignment == RowAlignment.Left)
                    {
                        curWidth += Math.Abs((Rows[index] as WTableRow).RowFormat.LeftIndent);
                    }
                    if (curWidth > width)
                    {
                        width = curWidth;
                    }
                }
            }
            return width;
        }
        /// <summary>
        /// Get the section in which table is present
        /// </summary>
        /// <returns></returns>
        private WSection GetOwnerSection()
        {
            IEntity entity = this as IEntity;
            while (entity != null)
            {
                if (entity.EntityType == DLS.EntityType.Section)
                    return entity as WSection;
                entity = entity.Owner;
            }
            return null;
        }
        #endregion

        #region Implementation / track changes
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        /// <param name="acceptChanges">if it accepts the changes, set to <c>true</c>.</param>
        internal override void MakeChanges(bool acceptChanges)
        {
            for (int i = 0; i < m_rows.Count; i++)
            {
                WTableRow row = m_rows[i] as WTableRow;
                foreach (WTableCell cell in row.Cells)
                {
                    (cell as WTextBody).MakeChanges(acceptChanges);
                    if (acceptChanges)
                    {
                        cell.m_trackCellFormat = null;
                    }
                    else if (cell.m_trackCellFormat != null)
                    {
                        cell.CellFormat.ClearFormatting();
                        cell.CellFormat.ImportContainer(cell.TrackCellFormat);
                        cell.m_trackCellFormat = null;
                    }
                }

                if (acceptChanges)
                {
                    row.m_trackRowFormat = null;
                }
                else if (row.m_trackRowFormat != null)
                {
                    row.RowFormat.ClearFormatting();
                    row.RowFormat.ImportContainer(row.TrackRowFormat);
                    row.m_trackRowFormat = null;
                }

                if ((row.IsDeleteRevision && acceptChanges) || (row.IsInsertRevision && !acceptChanges))
                {
                    m_rows.RemoveAt(i);
                    i -= 1;
                }
            }
        }
        /// <summary>
        /// Removes the character format changes.
        /// </summary>
        internal override void RemoveCFormatChanges()
        {
            foreach (WTableRow row in m_rows)
            {
                row.CharacterFormat.RemoveChanges();

                foreach (WTableCell cell in row.Cells)
                {
                    cell.CharacterFormat.RemoveChanges();
                }
            }
        }
        /// <summary>
        /// Removes the table format changes.
        /// </summary>
        internal override void RemovePFormatChanges()
        {
            foreach (WTableRow row in m_rows)
            {
                row.RowFormat.RemoveChanges();
            }
        }
        /// <summary>
        /// Accepts the changes for character format.
        /// </summary>
        internal override void AcceptCChanges()
        {
            foreach (WTableRow row in m_rows)
            {
                row.CharacterFormat.AcceptChanges();
                foreach (WTableCell cell in row.Cells)
                {
                    cell.CharacterFormat.AcceptChanges();
                }
            }
        }
        /// <summary>
        /// Accept changes in table format.
        /// </summary>
        internal override void AcceptPChanges()
        {
            foreach (WTableRow row in m_rows)
            {
                row.RowFormat.AcceptChanges();
            }
        }
        /// <summary>
        /// Defines whether table format is changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedPFormat()
        {
            foreach (WTableRow row in m_rows)
            {
                if (row.RowFormat.IsChangedFormat)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Checks a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        internal override bool CheckDeleteRev()
        {
            foreach (WTableRow row in m_rows)
            {
                if (row.CharacterFormat.IsDeleteRevision)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        internal override bool CheckInsertRev()
        {
            foreach (WTableRow row in m_rows)
            {
                if (row.CharacterFormat.IsInsertRevision)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Defines whether table format was changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedCFormat()
        {
            foreach (WTableRow row in m_rows)
            {
                if (row.CharacterFormat.IsChangedFormat)
                {
                    return true;
                }
                foreach (WTableCell cell in row.Cells)
                {
                    if (cell.CharacterFormat.IsChangedFormat)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether item has tracked changes.
        /// </summary>
        /// <returns>
        /// 	if has tracked changes, set to <c>true</c>.
        /// </returns>
        internal override bool HasTrackedChanges()
        {
            if (IsDeleteRevision || IsInsertRevision ||
              IsChangedCFormat || IsChangedPFormat)
            {
                return true;
            }

            foreach (WTableRow row in m_rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    foreach (TextBodyItem item in cell.Items)
                    {
                        if (item.HasTrackedChanges())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Remove added/deleted table rows, while number of rows > 1.
        /// </summary>
        /// <returns>Value indicating whether to completely remove table</returns>
        internal bool RemoveChangedTable()
        {
            WTableRow row = null;
            for (int i = 0; i < m_rows.Count; i++)
            {
                row = m_rows[i];
                if (row.CharacterFormat.IsDeleteRevision || row.CharacterFormat.IsInsertRevision)
                {
                    if (m_rows.Count > 1)
                    {
                        m_rows.Remove(row);
                        i -= 1;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Sets the delete revision for table.
        /// </summary>
        /// <param name="check">if it specifies the revision to delete, set to <c>true</c>.</param>
        internal override void SetDeleteRev(bool check)
        {
            foreach (WTableRow row in m_rows)
            {
                row.CharacterFormat.IsDeleteRevision = check;
            }
        }
        /// <summary>
        /// Sets the insert revision for table.
        /// </summary>
        /// <param name="check">if it specifies the revision for insertion, set to <c>true</c>.</param>
        internal override void SetInsertRev(bool check)
        {
            foreach (WTableRow row in m_rows)
            {
                row.CharacterFormat.IsInsertRevision = check;
            }
        }
        /// <summary>
        /// Sets the changed Character format for table.
        /// </summary>
        /// <param name="check">if it specifies format to be change, set to <c>true</c>.</param>
        internal override void SetChangedCFormat(bool check)
        {
            foreach (WTableRow row in m_rows)
            {
                row.CharacterFormat.IsChangedFormat = check;
                foreach (WTableCell cell in row.Cells)
                {
                    cell.CharacterFormat.IsChangedFormat = check;
                }
            }
        }
        /// <summary>
        /// Sets the changed Paragraph format for table.
        /// </summary>
        /// <param name="check">if it specifies the format to be changed, set to <c>true</c>.</param>
        internal override void SetChangedPFormat(bool check)
        {
            foreach (WTableRow row in m_rows)
            {
                row.RowFormat.IsChangedFormat = check;
            }
        }
        #endregion

        #region Implementation / xml
        //#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.RowsItemTag, Rows);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, XDLSConstants.ItemTypeTableValue);
        }
        //#endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);

            if (this.m_isTextBox && this.m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                m_layoutInfo.IsSkipBottomAlign = true;
            if (this.NextSibling is WParagraph)
                m_layoutInfo.IsPageBreakItem = (this.NextSibling as WParagraph).ParagraphFormat.PageBreakBefore;
            else if (this.NextSibling is WTable)
                m_layoutInfo.IsPageBreakItem = (this.NextSibling as WTable).Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.PageBreakBefore;
            if (TableFormat.CellSpacing > 0)
            {
                m_layoutInfo.Margins.Left = TableFormat.Borders.Left.LineWidth / 2;
                m_layoutInfo.Margins.Right = TableFormat.Borders.Right.LineWidth / 2;
                m_layoutInfo.Margins.Top = TableFormat.Borders.Top.LineWidth / 2;
                m_layoutInfo.Margins.Bottom = TableFormat.Borders.Bottom.LineWidth / 2;
            }
            if (IsHiddenTable(this))
                m_layoutInfo.IsSkip = true;
        }

        /// <summary>
        /// Determine whether the current row is need to be hidden
        /// </summary>
        /// <returns></returns>
        internal bool IsHiddenRow(int rowIndex, WTable table)
        {
            bool isHidden = false;
            if (table.Rows[rowIndex].RowFormat.Hidden)
            {
                for (int i = 0; i < table.Rows[rowIndex].Cells.Count; i++)
                {
                    for (int j = 0; j < table.Rows[rowIndex].Cells[i].ChildEntities.Count; j++)
                    {
                        if (table.Rows[rowIndex].Cells[i].ChildEntities[j] is WParagraph)
                        {
                            if (IsHiddenParagraph(table.Rows[rowIndex].Cells[i].ChildEntities[j] as WParagraph))
                                isHidden = true;
                            else
                                isHidden = false;
                        }
                        else if (table.Rows[rowIndex].Cells[i].ChildEntities[j] is WTable)
                        {
                            if (IsHiddenTable(table.Rows[rowIndex].Cells[i].ChildEntities[j] as WTable))
                                isHidden = true;
                            else
                                isHidden = false;
                        }
                    }
                }
            }
            return isHidden;
        }
        /// <summary>
        /// Determine whether the paragraph is Hidden 
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal bool IsHiddenParagraph(WParagraph paragraph)
        {
            bool isHidden = false;
            for (int i = 0; i < paragraph.ChildEntities.Count; i++)
            {
                if ((paragraph.ChildEntities[i] as ParagraphItem).ParaItemCharFormat.Hidden)
                    isHidden = true;
                else
                {
                    isHidden = false;
                    break;
                }
            }
            if (paragraph.ChildEntities.Count == 0 && paragraph.BreakCharacterFormat.Hidden)
                return true;
            else
                return isHidden;
        }
        /// <summary>
        /// Determine whether the table is hidden 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal bool IsHiddenTable(WTable table)
        {
            bool isHidden = false;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (IsHiddenRow(i, table))
                    isHidden = true;
                else
                    isHidden = false;
            }
            return isHidden;
        }
        /// <summary>
        /// Specifies Table layout information.
        /// </summary>
        ITableLayoutInfo ITableWidget.TableLayoutInfo
        {
            get
            {
                if (m_tableInfo == null)
                {
                    m_tableInfo = new TableLayoutInfo(this);
                }

                return m_tableInfo;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        int ITableWidget.MaxRowIndex
        {
            get
            {
                int count = 0, index = 0;
                for (int i = 0; i < m_rows.Count; i++)
                {
                    if (count < m_rows[i].Cells.Count)
                    {
                        count = m_rows[i].Cells.Count;
                        index = i;
                    }
                }
                return index;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        int ITableWidget.ColumnsCount
        {
            get
            {
                int count = 0;
                int colscount;
                for (int i = 0; i < m_rows.Count; i++)
                {
                    colscount = 0;
                    for (int j = 0; j < m_rows[i].Cells.Count; j++)
                    {
                        colscount += m_rows[i].Cells[j].Colspan;
                    }
                    if (count < colscount)
                    {
                        count = colscount;
                    }

                }
                return count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        int ITableWidget.RowsCount
        {
            get
            {
                return m_rows.Count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        IWidgetContainer ITableWidget.GetCellWidget(int row, int column)
        {
            return m_rows[row].Cells[column];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IWidget ITableWidget.GetRowWidget(int row)
        {
            return m_rows[row];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawTable(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
#endif
        #endregion

        #region Class internal declarations
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected class TableLayoutInfo
          : ITableLayoutInfo
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private WTable m_table;
            private float[] m_cellsWidth;
            private int m_headersRowCount = 0;
            private bool[] m_isDefaultCells;
            private float m_width;
            /// <summary>
            /// 
            /// </summary>
            private bool m_bIsSplittedTable = false;
            #endregion

            #region Class properties
            /// <summary>
            /// Gets / sets table width
            /// </summary>
            public float Width
            {
                get
                {
                    return m_width;
                }
                set
                {
                    m_width = value;
                }
            }
            /// <summary>
            /// Gets table height
            /// </summary>
            public float Height
            {
                get
                {
                    return 0f;
                }
            }
            /// <summary>
            /// Gets / sets cells width array
            /// </summary>
            public float[] CellsWidth
            {
                get
                {
                    return m_cellsWidth;
                }
                set
                {
                    m_cellsWidth = value;
                }
            }
            /// <summary>
            /// Gets / sets count of headers rows
            /// </summary>
            public int HeadersRowCount
            {
                get
                {
                    return m_headersRowCount;
                }
            }
            /// <summary>
            /// Gets the bool values array determining if cells are default
            /// </summary>
            public bool[] IsDefaultCells
            {
                get
                {
                    return m_isDefaultCells;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether this table is splitted table
            /// </summary>
            /// <value>
            /// <c>true</c> If this table is splitted into next page; otherwise, <c>false</c>>
            /// </value>
            public bool IsSplittedTable
            {
                get
                {
                    return m_bIsSplittedTable;
                }
                set
                {
                    m_bIsSplittedTable = value;
                }
            }
            #endregion

            #region Class initialize/finalize methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="table"></param>
            public TableLayoutInfo(WTable table)
            {
                m_table = table;

                int colCount = 0, maxRowIndex = 0;
                for (int i = 0; i < m_table.Rows.Count; i++)
                {
                    if (colCount < m_table.Rows[i].Cells.Count)
                    {
                        colCount = m_table.Rows[i].Cells.Count;
                        maxRowIndex = i;
                    }
                }

                m_width = m_table.Width;
                m_cellsWidth = new float[colCount];
                m_isDefaultCells = new bool[colCount];

                for (int i = 0, cnt = colCount; i < cnt; i++)
                {
                    WTableCell cell = m_table.Rows[maxRowIndex].Cells[i];
                    m_cellsWidth[i] = cell.Width;
                    m_isDefaultCells[i] = cell.IsFixedWidth;
                }
                m_headersRowCount = GetHeadersRowCount();
            }
            #endregion

            #region Class helper methods
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private int GetHeadersRowCount()
            {
                int hrCount = 0;
                for (int i = 0; i < m_table.Rows.Count; i++)
                {
                    WTableRow row = m_table.Rows[i];

                    if (!row.IsHeader)
                        break;

                    hrCount++;
                }

                return hrCount;
            }
            #endregion

            #region ITableLayoutInfo Members

            /// <summary>
            /// Gets owner table cell spacings.
            /// </summary>
            public double CellSpacings
            {
                get
                {
                    if ((m_table.Owner.Owner as WTableCell) != null)
                    {
                        double spasings = (m_table.Owner.Owner as WTableCell).OwnerRow.RowFormat.CellSpacing * 2;
                        return Math.Max(0, spasings);
                    }

                    return 0;
                }
            }
            /// <summary>
            /// Gets owner cell paddings.
            /// </summary>
            public double CellPaddings
            {
                get
                {
                    if ((m_table.Owner.Owner as WTableCell) != null)
                    {
                        return (m_table.Owner.Owner as WTableCell).CellFormat.Paddings.Left + (m_table.Owner.Owner as WTableCell).CellFormat.Paddings.Right;
                    }

                    return 0;
                }
            }

            #endregion
        }
#endif

        #endregion
    }
}

