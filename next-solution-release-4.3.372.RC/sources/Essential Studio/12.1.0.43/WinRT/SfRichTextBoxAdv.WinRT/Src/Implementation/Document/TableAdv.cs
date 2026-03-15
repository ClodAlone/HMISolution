#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
#if WPF
using System.Windows.Media;
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Markup;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
#if WPF
    [ContentProperty("Rows")]
#else
    [ContentProperty(Name = "Rows")]
#endif
    public class TableAdv : BlockAdv
    {
        #region Private Fields
        TableHolder tableholder = null;
        List<Widget> tableWidgets;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>
        /// The border brush.
        /// </value>
        internal Color BorderBrush
        {
            get 
            { 
                return (Color)GetValue(BorderBrushProperty); 
            }
            set 
            { 
                SetValue(BorderBrushProperty, value); 
            }
        }
        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        /// <value>
        /// The border thickness.
        /// </value>
        internal double BorderThickness
        {
            get 
            { 
                return (double)GetValue(BorderThicknessProperty); 
            }
            set 
            { 
                SetValue(BorderThicknessProperty, value); 
            }
        }
        /// <summary>
        /// Gets the rows collection.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public TableRowAdvCollection Rows
        {
            get
            {
                return ChildNodes as TableRowAdvCollection;
            }
        }
        /// <summary>
        /// Gets the nested level.
        /// </summary>
        /// <value>
        /// The nested level.
        /// </value>
        internal int NestedLevel
        {
            get
            {
                if (IsInsideTable)
                    return AssociatedCell.OwnerTable.NestedLevel + 1;
                return 1;
            }
        }
        /// <summary>
        /// Table Holder to hold columns
        /// </summary>
        internal TableHolder TableHolder
        {
            get
            {
                return tableholder;
            }
            set
            {
                tableholder = value;
            }
        }
        /// <summary>
        /// Gets the width of the table.
        /// </summary>
        /// <value>
        /// The width of the table.
        /// </value>
        internal double TableWidth
        {
            get
            {
                return GetTableWidth();
            }
        }
        /// <summary>
        /// Gets the table widgets.
        /// </summary>
        /// <value>
        /// The table widgets.
        /// </value>
        internal List<Widget> TableWidgets
        {
            get
            {
                return tableWidgets;
            }
        }
        /// <summary>
        /// Gets or sets the table format.
        /// </summary>
        /// <value>
        /// The table format.
        /// </value>
        public TableFormat TableFormat
        {
            get
            {
                return (TableFormat)GetValue(TableFormatProperty);
            }
            set
            {
                SetValue(TableFormatProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the BorderBrush dependency property.
        /// </summary>
        /// <returns>The identifier of the BorderBrush dependency property.</returns>
        internal static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Color), typeof(TableAdv), new PropertyMetadata(Colors.Black));
        /// <summary>
        /// Identifies the BorderThickness dependency property.
        /// </summary>
        /// <returns>The identifier of the BorderThickness dependency property.</returns>
        internal static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(TableAdv), new PropertyMetadata(1d));
        /// <summary>
        /// Identifies the TableFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the TableFormat dependency property.</returns>
        internal static readonly DependencyProperty TableFormatProperty = DependencyProperty.Register("TableFormat", typeof(TableFormat), typeof(TableAdv), new PropertyMetadata(null, OnTableFormatChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when table format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnTableFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as TableFormat).SetOwner(d as TableAdv);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAdv" /> class.
        /// </summary>
        public TableAdv()
            : this(null)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAdv" /> class.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        internal TableAdv(int rowCount, int columnCount)
            : this()
        {
            TableRowAdv row = null;
            TableCellAdv cell = null;
            for (int i = 0; i < rowCount; i++)
            {
                row = new TableRowAdv();
                for (int j = 0; j < columnCount; j++)
                {
                    cell = new TableCellAdv();
                    cell.CellFormat.CellWidth = 100;
                    cell.CellFormat.CellMargin = new Thickness(5, 3, 5, 3);
                    cell.Blocks.Add(new ParagraphAdv());
                    row.Cells.Add(cell);
                }
                Rows.Add(row);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAdv" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal TableAdv(Node owner)
            : base(owner)
        {
            ChildNodes = new TableRowAdvCollection(this);
            TableFormat = new TableFormat(this);
            tableWidgets = new List<Widget>();
            TableHolder = new TableHolder();
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override BlockAdv Clone()
        {
            TableAdv table = new TableAdv();
            table.TableFormat.CopyFormat(TableFormat);
            foreach (TableRowAdv row in Rows)
            {
                table.Rows.Add(row.Clone());
            }
            return table;
        }
        /// <summary>
        /// Updates the list items.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        internal override bool UpdateListItems(BlockAdv block)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                bool isListUpdated = Rows[i].UpdateListItems(block);
                if (isListUpdated)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Updates the rendered list items.
        /// </summary>
        internal override void UpdateRenderedListItems()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].UpdateRenderedListItems();
            }
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal override void LayoutItems(LayoutViewer viewer)
        {
            if (tableWidgets != null
                && tableWidgets.Count > 0)
            {
                //Removes the table widget.
                tableWidgets.Clear();
            }
            AddColumns();
            if (viewer is FlowLayoutViewer && !IsInsideTable)
            {
                double tableWidth = 0;
#if WPF
                tableWidth = GetTableWidth();
#else
                UIDispatcher.Execute(() => tableWidth = GetTableWidth());
#endif
                //Handled specifically for flow layout inorder to get the exceeding width of table.
                if (Math.Round(viewer.ClientArea.Width) < Math.Round(tableWidth)
                    && viewer.HorizontalWidth < viewer.ClientActiveArea.X + tableWidth + viewer.ClientArea.X)
                    viewer.HorizontalWidth = viewer.ClientActiveArea.X + tableWidth + viewer.ClientArea.X;
            }
            //TableLayoutCalculator.MeasureTableOnAutoMode(this, viewer.ClientArea.Width);
            AddTableWidget(viewer.ClientActiveArea);
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].LayoutItems(viewer);
            }
            UpdateWidgetToPage(viewer);
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal override void ClearWidgets()
        {
            if (tableWidgets != null
                && tableWidgets.Count > 0)
            {
                //Removes the table widget.
                for (int i = 0; i < tableWidgets.Count; i++)
                {
                    TableWidget widget = tableWidgets[i] as TableWidget;
                    widget.Dispose();
                    tableWidgets.Remove(widget);
                    i--;
                }
            }
        }
        /// <summary>
        /// Combines the table widgets.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        internal void CombineTableWidgets(TableCellWidget cellWidget)
        {
            if (tableWidgets.Count == 0)
                return;
            TableWidget tableWidget = tableWidgets[0] as TableWidget;
            if (cellWidget != null && !cellWidget.ChildWidgets.Contains(tableWidget))
                tableWidget.UpdateContainerWidget(cellWidget);
            if (tableWidgets.Count == 1)
            {
                for (int i = 0; i < tableWidgets[0].ChildWidgets.Count; i++)
                {
                    TableRowWidget rowWidget = tableWidgets[0].ChildWidgets[i] as TableRowWidget;
                    for (int j = 0; j < rowWidget.ChildWidgets.Count; j++)
                    {
                        (rowWidget.ChildWidgets[j] as TableCellWidget).UpdateWidgetHeight();
                    }
                }
                return;
            }
            tableWidget.Height = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i] as TableRowAdv;
                TableRowWidget rowWidget = row.TableRowWidgets[0] as TableRowWidget;
                bool addRowHeight = tableWidget.ChildWidgets.Contains(rowWidget);
                row.CombineTableRowWidgets(tableWidget);
                row.UpdateRowHeightBySpannedCell(tableWidget, rowWidget);
                if (addRowHeight)
                    tableWidget.Height += rowWidget.Height;
            }
            tableWidget.UpdateChildLocation(tableWidget.Location.Y);
            for (int i = 1; i < tableWidgets.Count; i++)
            {
                TableWidget curWidget = tableWidgets[i] as TableWidget;
                curWidget.Dispose();
                i--;
            }
        }
        /// <summary>
        /// Shifts the widgets.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal override void ShiftWidgets(LayoutViewer viewer)
        {
            int index = 0;
            BodyWidget prevBodyWidget = GetBodyWidgetOfPreviousBlock(ref index);
            if (viewer is PageLayoutViewer)
                CombineTableWidgets(null);
            for (int i = 0; i < tableWidgets.Count; i++)
            {
                TableWidget widget = tableWidgets[i] as TableWidget;
                TableWidget splittedWidget = null;
                if (viewer.ClientActiveArea.Height < widget.Height && widget.IsFirstLineFit(widget.Location.Y + viewer.ClientActiveArea.Height))
                {
                    splittedWidget = widget.GetSplittedWidget(widget.Location.Y + viewer.ClientActiveArea.Height);
                    splittedWidget.UpdateChildLocation(widget.Location.Y);
                }
                if (viewer is FlowLayoutViewer || viewer.ClientActiveArea.Height >= widget.Height)
                {
                    //Updates table widget location.
                    widget.Location = new Point(widget.Location.X, viewer.ClientActiveArea.Y);
                    widget.UpdateChildLocation(widget.Location.Y);
                    if (prevBodyWidget != widget.ContainerWidget)
                    {
                        index++;
                        widget.UpdateContainerWidget(prevBodyWidget, index);
                    }
                    if (viewer is PageLayoutViewer)
                        widget.UpdateHeight(viewer);
                    viewer.CutFromTop(widget.Location.Y + widget.Height);
                }
                else if (splittedWidget == null)
                {
                    splittedWidget = widget;
                    i--;
                }
                if (splittedWidget != null)
                {
                    if (!tableWidgets.Contains(splittedWidget))
                        tableWidgets.Add(splittedWidget);
                    BodyWidget nextBodyWidget = prevBodyWidget.CreateOrGetNextBodyWiget(viewer);
                    if (nextBodyWidget.ChildWidgets.Count > 0 || splittedWidget.ContainerWidget == nextBodyWidget)
                        //Updates client area based on next body widget.
                        viewer.UpdateClientArea(nextBodyWidget.Section.SectionFormat);
                    if (splittedWidget.ContainerWidget != nextBodyWidget)
                        splittedWidget.UpdateContainerWidget(nextBodyWidget, 0);
                    prevBodyWidget = nextBodyWidget;
                }
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            SetOwner(null);
            TableFormat.Dispose();
            ClearValue(TableFormatProperty);
            ClearWidgets();
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                row.Dispose();
                Rows.Remove(row);
                i--;
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the width of the table.
        /// </summary>
        /// <returns></returns>
        internal double GetTableWidth()
        {
            double width = 0;
            foreach (TableRowAdv row in Rows)
            {
                double rowWidth = 0;
                foreach (TableCellAdv cell in row.Cells)
                {
                    rowWidth += cell.CellFormat.CellWidth;
                }
                if (width < rowWidth)
                    width = rowWidth;
            }
            return width;
        }
        /// <summary>
        /// Fits the cells to client area.
        /// </summary>
        /// <param name="clientWidth">Width of the client.</param>
        internal void FitCellsToClientArea(double clientWidth)
        {
            double tableWidth = GetTableWidth();
            double factor = clientWidth / tableWidth;
            foreach (TableRowAdv row in Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    cell.CellFormat.CellWidth *= factor;
                }
            }
        }
        /// <summary>
        /// Adds the table widget.
        /// </summary>
        /// <param name="area">The area.</param>
        internal void AddTableWidget(Rect area)
        {
            TableWidget tableWidget = new TableWidget(this);
            tableWidget.Width = area.Width;
            tableWidget.Location = new Point(area.X, area.Y);
            tableWidgets.Add(tableWidget);
        }
        /// Updates the widget to page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateWidgetToPage(LayoutViewer viewer)
        {
            TableWidget tableWidget = tableWidgets[tableWidgets.Count - 1] as TableWidget;
            if (IsInsideTable)
            {
                //Adds the table widget to owner cell widget.
                AssociatedCell.TableCellWidgets[AssociatedCell.TableCellWidgets.Count - 1].ChildWidgets.Add(tableWidget);
                tableWidget.ContainerWidget = AssociatedCell.TableCellWidgets[AssociatedCell.TableCellWidgets.Count - 1];
            }
            else
            {
                //Adds the table widget to the Header Footer/ Body widget.
                UpdateWidgetsToBody(viewer, tableWidget);
                tableWidget.UpdateHeight(viewer);
            }
            //Renders Table outline rectangle - Border and background color.
            //viewer.OwnerControl.RenderingManager.RenderTableOutline(tableWidget, viewer.CurrentPage);
        }
        /// <summary>
        /// Adds the columns.
        /// </summary>
        internal void AddColumns()
        {
            TableHolder.Columns.Clear();
            List<double> tableGrid = new List<double>();
            int toremovelast = 0;
            List<TableCellAdv> rowSpannedCells = new List<TableCellAdv>();
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                int columnSpan = 0;
                double currOffset = 0;
                if (!tableGrid.Contains(currOffset))
                    tableGrid.Add(currOffset);
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    TableCellAdv cell = row.Cells[j];
                    if (rowSpannedCells.Count == 0)
                        cell.ColumnIndex = columnSpan;
                    for (int k = 0; k < rowSpannedCells.Count; k++)
                    {
                        if (rowSpannedCells[k].ColumnIndex < columnSpan)
                        {
                            cell.ColumnIndex = columnSpan;
                            continue;
                        }
                        int rowSpan = 1;
#if !WPF
                        UIDispatcher.Execute(() =>
                        {
#endif
                            rowSpan = rowSpannedCells[k].CellFormat.RowSpan;
                            if (rowSpannedCells[k].ColumnIndex > columnSpan)
                                cell.ColumnIndex = columnSpan;
                            else
                            {
                                cell.ColumnIndex = columnSpan = TableHolder.Columns.IndexOf(rowSpannedCells[k].OwnerColumn) + rowSpannedCells[k].CellFormat.ColumnSpan;
                                //Converts the cell width from pixel to twips point by 15 factor.
                                currOffset = Math.Round(currOffset + (rowSpannedCells[k].CellFormat.CellWidth * 15), 2);
                            }
#if !WPF
                        });
#endif
                        if (i - rowSpannedCells[k].OwnerRow.RowIndex == rowSpan - 1)
                        {
                            rowSpannedCells.RemoveAt(k);
                            k--;
                        }
                    }
#if !WPF
                    UIDispatcher.Execute(() =>
                    {
#endif
                        if (cell.CellFormat.RowSpan > 1)
                            rowSpannedCells.Add(cell);
                        columnSpan += cell.CellFormat.ColumnSpan;
                        //Converts the cell width from pixel to twips point by 15 factor.
                        currOffset = Math.Round(currOffset + (cell.CellFormat.CellWidth * 15), 2);
#if !WPF
                    });
#endif
                    if (!tableGrid.Contains(currOffset))
                        tableGrid.Add(currOffset);
                    toremovelast = Math.Max(toremovelast, columnSpan - 1);
                    if (TableHolder.Columns.Count < columnSpan)
                    {
#if !WPF
                        UIDispatcher.Execute(() =>
                        {
#endif
                            TableColumnAdv column = new TableColumnAdv();
                            TableHolder.Columns.Add(column);
                            while (TableHolder.Columns.Count < columnSpan)
                            {
                                column = new TableColumnAdv();
                                TableHolder.Columns.Add(column);
                            }
#if !WPF
                        });
#endif
                    }
                }
            }
            tableGrid.Sort();
            for (int i = 0; i < tableGrid.Count - 1; i++)
            {
                TableColumnAdv column = TableHolder.Columns[i];
                //Converts the twips point to cell width in pixel by 15 factor.
                column.PreferredWidth = (tableGrid[i + 1] - tableGrid[i]) / 15;
            }
            tableGrid.Clear();
            while ((toremovelast + 1) < TableHolder.Columns.Count)
            {
                TableHolder.Columns.RemoveAt(TableHolder.Columns.Count - 1);
            }
        }
        /// <summary>
        /// Sets the desired width to cells.
        /// </summary>
        internal void SetDesiredWidthToCells()
        {
            foreach (TableRowAdv rw in Rows)
            {
                foreach (TableCellAdv cell in rw.Cells)
                {
                    if (cell.CellFormat.ColumnSpan > 1)
                    {
                        double tempwidth = 0.0;
                        for (int i = cell.ColumnIndex; i < (cell.ColumnIndex + cell.CellFormat.ColumnSpan); i++)
                        {
                            tempwidth += TableHolder.Columns[i].PreferredWidth;
                        }
                        cell.CellFormat.CellWidth = tempwidth;
                    }
                    else
                    {
                        cell.CellFormat.CellWidth = TableHolder.Columns[cell.ColumnIndex].PreferredWidth;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the last block in last cell.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetLastBlockInLastCell()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv lastrow = Rows[Rows.Count - 1];
                TableCellAdv lastcell = lastrow.Cells[lastrow.Cells.Count - 1];
                return lastcell.Blocks[lastcell.Blocks.Count - 1];
            }
            return null;
        }
        /// <summary>
        /// Gets the last paragraph in first row.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetLastParagraphInFirstRow()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv row = Rows[0];
                TableCellAdv lastcell = row.Cells[row.Cells.Count - 1];
                BlockAdv lastBlock = lastcell.Blocks[lastcell.Blocks.Count - 1];
                if (lastBlock is ParagraphAdv)
                    return lastBlock as ParagraphAdv;
                else
                    return (lastBlock as TableAdv).GetLastParagraphInLastCell();
            }
            return null;
        }
        /// <summary>
        /// Gets the last paragraph in last cell.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetLastParagraphInLastCell()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv lastrow = Rows[Rows.Count - 1];
                TableCellAdv lastcell = lastrow.Cells[lastrow.Cells.Count - 1];
                BlockAdv lastBlock = lastcell.Blocks[lastcell.Blocks.Count - 1];
                if (lastBlock is ParagraphAdv)
                    return lastBlock as ParagraphAdv;
                else
                    return (lastBlock as TableAdv).GetLastParagraphInLastCell();
            }
            return null;
        }
        /// <summary>
        /// Gets the first paragraph in last row.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetFirstParagraphInLastRow()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv lastrow = Rows[Rows.Count - 1];
                TableCellAdv lastcell = lastrow.Cells[0];
                BlockAdv lastBlock = lastcell.Blocks[0];
                if (lastBlock is ParagraphAdv)
                    return lastBlock as ParagraphAdv;
                else
                    return (lastBlock as TableAdv).GetFirstParagraphInFirstCell();
            }
            return null;
        }
        /// <summary>
        /// Gets the first paragraph in first cell.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetFirstParagraphInFirstCell()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv row = Rows[0];
                TableCellAdv cell = row.Cells[0];
                BlockAdv block = cell.Blocks[0];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetFirstParagraphInFirstCell();
            }
            return null;
        }
        /// <summary>
        /// Gets the first block in first cell.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetFirstBlockInFirstCell()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv firstrow = Rows[0];
                if (firstrow.Cells.Count > 0)
                {
                    TableCellAdv firstcell = firstrow.Cells[0];
                    if (firstcell.Blocks.Count == 0)
                        return null;
                    return firstcell.Blocks[0];
                }
            }
            return null;
        }
        /// <summary>
        /// Determines whether table contains the specified table cell.
        /// </summary>
        /// <param name="tableCell">The table cell.</param>
        /// <returns>
        ///   <c>true</c> if table contains the specified table cell; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(TableCellAdv tableCell)
        {
            if (this == tableCell.OwnerTable)
                return true;
            while (tableCell.OwnerTable.IsInsideTable)
            {
                if (this == tableCell.OwnerTable)
                    return true;
                tableCell = tableCell.OwnerTable.AssociatedCell;
            }
            return this == tableCell.OwnerTable;
        }
        /// <summary>
        /// Gets the cell in table.
        /// </summary>
        /// <param name="tableCell">The table cell.</param>
        /// <returns></returns>
        internal TableCellAdv GetCellInTable(TableCellAdv tableCell)
        {
            while (tableCell.OwnerTable.IsInsideTable)
            {
                if (this == tableCell.OwnerTable)
                    return tableCell;
                tableCell = tableCell.OwnerTable.AssociatedCell;
            }
            return tableCell;
        }
        /// <summary>
        /// Inserts the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="moveRows">if set to <c>true</c> move rows to inserted table.</param>
        internal void InsertTable(TableAdv table, bool moveRows)
        {
            //Gets the index of current table.
            int insertIndex = GetIndexInOwnerCollection();
            CompositeNode owner = Owner as CompositeNode;
            RemoveBlock();
            if (moveRows)
            {
                //Moves the rows to table.
                for (int i = 0, index = 0; i < Rows.Count; i++, index++)
                {
                    TableRowAdv row = Rows[i];
                    table.Rows.Insert(index, row);
                    i--;
                }
            }
            //Inserts table in the current table position.
            (owner.ChildNodes as BlockAdvCollection).Insert(insertIndex, table);
        }
        #endregion

        #region Highlight Selected Contents
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal override void Highlight(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            Rows[0].Highlight(selection, start, end);
            if (!end.Paragraph.IsInsideTable //Selection end is outside the table cell.
                || !Contains(end.Paragraph.AssociatedCell)) //Selection end is not inside the current table.
            {
                BlockAdv block = GetNextRenderedBlock();
                //Goto the next block.
                block.Highlight(selection, start, end);
            }
        }
        /// <summary>
        /// Highlights the cells.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        internal void HighlightCells(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            for (int i = Rows.IndexOf(startCell.OwnerRow); i <= count; i++)
            {
                Rows[i].Highlight(selection, start, end);
            }
        }
        #endregion

        #region Delete Selected Contents
        /// <summary>
        /// Deletes the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="editAction">The edit action.</param>
        internal override void Delete(SelectionAdv selection, TextPosition start, TextPosition end, byte editAction)
        {
            if (end.Paragraph.IsInsideTable && Contains(end.Paragraph.AssociatedCell))
            {
                if (Rows[Rows.Count - 1].Contains(end.Paragraph.AssociatedCell))
                    Delete(selection, editAction);
                else
                {
                    DocIO.DLS.WTable table = null;
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        TableRowAdv row = Rows[i];
                        if (editAction > 2)
                        {
                            if (table == null)
                            {
                                table = new DocIO.DLS.WTable(selection.WordDocument);
                                selection.WordDocument.LastSection.Body.ChildEntities.Add(table);
                            }
                            DocxExporting.SerializeTableRow(row, table);
                        }
                        if (editAction < 4)
                        {
                            Rows.Remove(row);
                            selection.CurrentHistoryInfo.RemovedNodes.Add(row);
                            i--;
                        }
                        if (row.Contains(end.Paragraph.AssociatedCell))
                            return;
                    }
                }
            }
            else
            {
                BlockAdv block = GetNextRenderedBlock();
                SectionAdv section = Section;
                Delete(selection, editAction);
                if (block != null)
                {
                    SectionAdv nextSection = block.Section;
                    if (section != nextSection)
                    {
                        if (editAction < 4)
                            section.CombineSection(selection, nextSection);
                        if (editAction > 2)
                            selection.CopySectionFormat(section);
                    }
                    //Goto the next block.
                    block.Delete(selection, start, end, editAction);
                }
            }
        }
        /// <summary>
        /// Deletes the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="editAction">The edit action.</param>
        internal void Delete(SelectionAdv selection, byte editAction)
        {
            if (editAction > 2)
                //Copy as DocIO instance.
                DocxExporting.SerializeTable(this, selection.WordDocument.LastSection.Body);
            if (editAction < 4)
            {
                RemoveBlock();
                selection.CurrentHistoryInfo.RemovedNodes.Add(this);
            }
        }
        /// <summary>
        /// Splits the table.
        /// </summary>
        /// <param name="splitEndRow">The split end row.</param>
        /// <returns></returns>
        internal TableAdv SplitTable(TableRowAdv splitEndRow)
        {
            TableAdv newTable = new TableAdv();
            newTable.TableFormat.CopyFormat(TableFormat);
            //Moves the rows to new table.
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                if (row == splitEndRow)
                    break;
                newTable.Rows.Add(row);
                i--;
            }
            //Inserts new table in the current text position.
            int insertIndex = GetIndexInOwnerCollection();
            ((Owner as CompositeNode).ChildNodes as BlockAdvCollection).Insert(insertIndex, newTable);
            return newTable;
        }
        /// <summary>
        /// Deletes the cells.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        /// <param name="editAction">The edit action.</param>
        internal void DeleteCells(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell, byte editAction)
        {
            TableAdv clonedTable = null;
            Actions action = Actions.Delete;
            bool isDeleteCells = false;
            if (selection.CurrentHistoryInfo != null)
            {
                action = selection.CurrentHistoryInfo.Action;
                isDeleteCells = selection.CurrentHistoryInfo.Action == Actions.BackSpace || selection.CurrentHistoryInfo.Action == Actions.DeleteCells
                    || selection.CurrentHistoryInfo.Action == Actions.InsertTable || (startCell.OwnerRow.PreviousNode == null
                    && endCell.OwnerRow.NextNode == null && selection.CurrentHistoryInfo.Action == Actions.Cut);
                selection.CurrentHistoryInfo.Action = isDeleteCells ? Actions.DeleteCells : Actions.ClearCells;
                clonedTable = CloneTableToHistoryInfo(selection.CurrentHistoryInfo);
                selection.OwnerControl.IsLayoutEnabled = false;
            }
            //Deletes the selected cells.
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            bool isStarted = false;
            DocIO.DLS.WTable wTable = null;
            DocIO.DLS.WTableRow wRow = null;
            bool isCellCleared = false;
            for (int i = Rows.IndexOf(startCell.OwnerRow); i <= count && i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                if (wTable != null)
                    wRow = new DocIO.DLS.WTableRow(selection.WordDocument);
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double left = row.GetCellLeft(row.Cells[j]);
                    if (start <= left && left < end)
                    {
                        if (!isStarted)
                        {
                            if (editAction > 2)
                            {
                                //Copy as DocIO instance.
                                wTable = new DocIO.DLS.WTable(selection.WordDocument);
                                wRow = new DocIO.DLS.WTableRow(selection.WordDocument);
                                //Adds the copied contents to DocIO Word document instance.
                                selection.WordDocument.LastSection.Body.ChildEntities.Add(wTable);
                            }
                            row.Cells[j].UpdateEditPosition(selection);
                            isStarted = true;
                        }
                        if (wRow != null)
                            DocxExporting.SerializeTableCell(row.Cells[j], wRow);
                        if (isDeleteCells)
                        {
                            //Specific for Backspace and Cut if selection includes all rows.
                            row.Cells.RemoveAt(j);
                            j--;
                        }
                        else if (editAction < 4)
                            isCellCleared |= row.Cells[j].Delete(selection, editAction,false);
                    }
                }
                if (wRow != null && wRow.Cells.Count > 0)
                    wTable.Rows.Add(wRow);
                if (row.Cells.Count == 0)
                {
                    Rows.RemoveAt(i);
                    i--;
                }
            }
            if (selection.CurrentHistoryInfo != null)
            {
                //Layouts the table after delete cells.
                selection.OwnerControl.IsLayoutEnabled = true;
                if (Rows.Count == 0)
                {
                    selection.EditPosition = GetHierarchicalIndex("0");
                    selection.CurrentHistoryInfo.Action = action;
                    RemoveBlock();
                }
                else
                {
                    if (!isDeleteCells && !isCellCleared)
                        selection.CurrentHistoryInfo = null;
                    else
                        Layout();
                }
            }
        }
        /// <summary>
        /// Clones the table to history info.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        /// <returns></returns>
        internal TableAdv CloneTableToHistoryInfo(HistoryInfo historyInfo)
        {
            //Clones the entire table to preserve in history.
            TableAdv clonedTable = Clone() as TableAdv;
            //Preserves the cloned table in history info, for future undo operation.
            historyInfo.RemovedNodes.Add(clonedTable);
            //Sets the insert position in history info as current table.
            historyInfo.InsertPosition = GetHierarchicalIndex("0");
            return clonedTable;
        }
        #endregion

        #region Merge Selected Cells
        /// <summary>
        /// Determines whether the selected cells in this table can be merged.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        /// <returns>
        ///   <c>true</c> if the selected cells in this table can be merged; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanMergeSelectedCells(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell)
        {
            int count = Rows.IndexOf(endCell.OwnerRow);
            int rowStartIndex = Rows.IndexOf(startCell.OwnerRow);
            if (rowStartIndex == count)
                return true;
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            double selectionLeft = 0, selectionRight = 0;
            for (int i = rowStartIndex; i <= count; i++)
            {
                TableRowAdv row = Rows[i];
                double rowLeft = 0, rowRight = 0;
                bool isStarted = false;
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double cellStart = row.GetCellLeft(row.Cells[j]);
                    if (start <= cellStart && cellStart < end)
                    {
                        if (!isStarted)
                        {
                            rowLeft = cellStart;
                            rowRight = cellStart;
                            isStarted = true;
                        }
                        rowRight += row.Cells[j].CellFormat.CellWidth;
                    }
                }
                if (i == rowStartIndex)
                {
                    selectionLeft = rowLeft;
                    selectionRight = rowRight;
                }
                else
                {
                    if (!(Math.Round(selectionLeft) == Math.Round(rowLeft) && Math.Round(selectionRight) == Math.Round(rowRight)))
                        return false;
                    if (i == count)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Merges the selected cells.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        /// <returns></returns>
        internal TableCellAdv MergeSelectedCells(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell)
        {
            //Clones the entire table to preserve in history.
            TableAdv clonedTable = CloneTableToHistoryInfo(selection.CurrentHistoryInfo);
            selection.OwnerControl.IsLayoutEnabled = false;
            //Merges the selected cells.
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            int rowStartIndex = Rows.IndexOf(startCell.OwnerRow);
            TableCellAdv mergedCell = null;
            for (int i = rowStartIndex; i <= count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double cellStart = row.GetCellLeft(row.Cells[j]);
                    if (start <= cellStart && cellStart < end)
                    {
                        if (mergedCell == null)
                            mergedCell = row.Cells[j];
                        else
                        {
                            if (i == rowStartIndex)
                                mergedCell.CellFormat.CellWidth += row.Cells[j].CellFormat.CellWidth;
                            foreach (BlockAdv block in row.Cells[j].Blocks)
                            {
                                mergedCell.Blocks.Add(block.Clone());
                            }
                            row.Cells.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            if (mergedCell != null && rowStartIndex < count)
                mergedCell.CellFormat.RowSpan = count - rowStartIndex + 1;
            //Layouts the table after merging cells.
            selection.OwnerControl.IsLayoutEnabled = true;
            Layout();
            return mergedCell;
        }
        #endregion

        #region Apply Character Format for Selected Contents
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].ApplyCharacterFormat(selection, property, value);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = GetNextRenderedBlock();
            //Goto the next block.
            block.ApplyCharacterFormat(selection, start, end, property, value);
        }
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyCharacterFormat(SelectionAdv selection, DependencyProperty property, object value)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].ApplyCharacterFormat(selection, property, value);
                }
            }
        }
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyCharacterFormat(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell, DependencyProperty property, object value)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            bool isStarted = false;
            for (int i = Rows.IndexOf(startCell.OwnerRow); i <= count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double left = row.GetCellLeft(row.Cells[j]);
                    if (start <= left && left < end)
                    {
                        if (!isStarted)
                        {
                            value = row.Cells[j].GetCharacterFormatValue(property, value);
                            isStarted = true;
                        }
                        row.Cells[j].ApplyCharacterFormat(selection, property, value);
                    }
                }
            }
        }
        #endregion

        #region Apply Paragraph Format for Selected Contents
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].ApplyParagraphFormat(selection, property, value);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = GetNextRenderedBlock();
            //Goto the next block.
            block.ApplyParagraphFormat(selection, start, end, property, value);
        }
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyParagraphFormat(SelectionAdv selection, DependencyProperty property, object value)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].ApplyParagraphFormat(selection, property, value);
                }
            }
        }
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyParagraphFormat(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell, DependencyProperty property, object value)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            bool isStarted = false;
            for (int i = Rows.IndexOf(startCell.OwnerRow); i <= count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double left = row.GetCellLeft(row.Cells[j]);
                    if (start <= left && left < end)
                    {
                        if (!isStarted)
                        {
                            value = row.Cells[j].GetParagraphFormatValue(property, value);
                            isStarted = true;
                        }
                        row.Cells[j].ApplyParagraphFormat(selection, property, value);
                    }
                }
            }
        }
        #endregion

        #region Get Character Format for Selected Contents
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal override void GetCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].GetCharacterFormat(selection);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = GetNextRenderedBlock();
            //Goto the next block.
            block.GetCharacterFormat(selection, start, end);
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal override void GetCharacterFormat(SelectionAdv selection)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].GetCharacterFormat(selection);
                }
            }
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        internal void GetCharacterFormat(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            for (int i = Rows.IndexOf(startCell.OwnerRow); i <= count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double left = row.GetCellLeft(row.Cells[j]);
                    if (start <= left && left < end)
                    {
                        row.Cells[j].GetCharacterFormat(selection);
                    }
                }
            }
        }
        #endregion

        #region Get Paragraph Format for Selected Contents
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal override void GetParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].GetParagraphFormat(selection);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = GetNextRenderedBlock();
            //Goto the next block.
            block.GetParagraphFormat(selection, start, end);
        }
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="selection">The selection.</param>
        internal override void GetParagraphFormat(SelectionAdv selection)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].GetParagraphFormat(selection);
                }
            }
        }
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startCell">The start cell.</param>
        /// <param name="endCell">The end cell.</param>
        internal void GetParagraphFormat(SelectionAdv selection, TableCellAdv startCell, TableCellAdv endCell)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            double endCellLeft = endCell.OwnerRow.GetCellLeft(endCell);
            double endCellRight = endCellLeft + endCell.CellFormat.CellWidth;
            if (start > endCellLeft)
                start = endCellLeft;
            if (end < endCellRight)
                end = endCellRight;
            if (start > selection.UpDownSelectionLength)
                start = selection.UpDownSelectionLength;
            if (end < selection.UpDownSelectionLength)
                end = selection.UpDownSelectionLength;
            int count = Rows.IndexOf(endCell.OwnerRow);
            for (int i = Rows.IndexOf(startCell.OwnerRow); i <= count; i++)
            {
                TableRowAdv row = Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    double left = row.GetCellLeft(row.Cells[j]);
                    if (start <= left && left < end)
                    {
                        row.Cells[j].GetParagraphFormat(selection);
                    }
                }
            }
        }
        #endregion

        #region Gets Cell Format for selection
        /// <summary>
        /// Gets the cell format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void GetCellFormat(SelectionAdv selection,TextPosition start, TextPosition end)
        {
            bool isStarted = false;
            for (int i = 0; i < Rows.Count; i++)
            {
                TableRowAdv row = Rows[i];
                if (row == start.Paragraph.AssociatedCell.OwnerRow)
                    isStarted = true;
                if (isStarted)
                {
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        TableCellAdv cell = row.Cells[j];
                        if (cell.IsCellSelected(start, end))
                            selection.CellFormat.CombineFormat(cell.CellFormat);
                        if (cell == end.Paragraph.AssociatedCell)
                        {
                            selection.CellFormat.CombineFormat(cell.CellFormat);
                            return;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
