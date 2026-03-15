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
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
#if WPF
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Markup;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.Foundation;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
#if WPF
    [ContentProperty("Cells")]
#else
    [ContentProperty(Name = "Cells")]
#endif
    public class TableRowAdv : CompositeNode
    {
        #region Fields
        List<TableRowWidget> tableRowWidgets;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public TableCellAdvCollection Cells
        {
            get
            {
                return ChildNodes as TableCellAdvCollection;
            }
        }
        /// <summary>
        /// Gets the owner table.
        /// </summary>
        /// <value>
        /// The owner table.
        /// </value>
        public TableAdv OwnerTable
        {
            get
            {
                return Owner as TableAdv;
            }
        }
        /// <summary>
        /// Gets the table row widgets.
        /// </summary>
        /// <value>
        /// The table row widgets.
        /// </value>
        internal List<TableRowWidget> TableRowWidgets
        {
            get
            {
                return tableRowWidgets;
            }
        }
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>
        /// The index of the row.
        /// </value>
        internal int RowIndex
        {
            get
            {
                if (OwnerTable != null)
                    return OwnerTable.Rows.IndexOf(this);
                return -1;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowAdv"/> class.
        /// </summary>
        public TableRowAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal TableRowAdv(Node owner)
            : base(owner)
        {
            ChildNodes = new TableCellAdvCollection(this);
            tableRowWidgets = new List<TableRowWidget>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Combines the table row widgets.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        internal void CombineTableRowWidgets(TableWidget tableWidget)
        {
            if (tableRowWidgets.Count == 0)
                return;
            TableRowWidget rowWidget = tableRowWidgets[0] as TableRowWidget;
            if (!tableWidget.ChildWidgets.Contains(rowWidget))
                rowWidget.UpdateTableWidget(tableWidget, tableWidget.ChildWidgets.Count);
            rowWidget.Height = 0;
            for (int i = 0; i < Cells.Count; i++)
            {
                TableCellAdv cell = Cells[i] as TableCellAdv;
                cell.CombineTableCellWidgets(rowWidget);
                TableCellWidget cellWidget = cell.TableCellWidgets[0] as TableCellWidget;
                if (cell.CellFormat.RowSpan == 1)
                {
                    double cellHeight = cellWidget.Height + cellWidget.Margin.Top + cellWidget.Margin.Bottom;
                    if (rowWidget.Height < cellHeight)
                        rowWidget.Height = cellHeight;
                }
            }
            for (int i = 1; i < tableRowWidgets.Count; i++)
            {
                TableRowWidget curWidget = tableRowWidgets[i] as TableRowWidget;
                curWidget.Dispose();
                i--;
            }
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal TableRowAdv Clone()
        {
            TableRowAdv row = new TableRowAdv();
            foreach (TableCellAdv cell in Cells)
            {
                row.Cells.Add(cell.Clone());
            }
            return row;
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal void ClearWidgets()
        {
            if (tableRowWidgets != null
                && tableRowWidgets.Count > 0)
            {
                //Removes the table row widget.
                for (int i = 0; i < tableRowWidgets.Count; i++)
                {
                    TableRowWidget widget = tableRowWidgets[i] as TableRowWidget;
                    widget.Dispose();
                    tableRowWidgets.Remove(widget);
                    i--;
                }
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearWidgets();
            for (int i = 0; i < Cells.Count; i++)
            {
                TableCellAdv cell = Cells[i];
                cell.Dispose();
                Cells.Remove(cell);
                i--;
            }
        }
        /// <summary>
        /// Updates the list items.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        internal bool UpdateListItems(BlockAdv block)
        {
            if (block.IsInsideTable && Cells.Contains(block.AssociatedCell.GetContainerCell()))
            {
                //Returns as list updated, inorder to start list numbering from first list paragraph of this row.
                return true;
            }
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].UpdateListItems(block);
            }
            return false;
        }
        /// <summary>
        /// Updates the rendered list items.
        /// </summary>
        internal void UpdateRenderedListItems()
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].UpdateRenderedListItems();
            }
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void LayoutItems(LayoutViewer viewer)
        {
            if (tableRowWidgets != null
                && tableRowWidgets.Count > 0)
            {
                //Removes the table row widget.
                tableRowWidgets.Clear();
            }
            AddTableRowWidget(viewer.ClientActiveArea);
            viewer.UpdateClientArea(this, true);
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].LayoutItems(viewer);
            }
            viewer.UpdateClientArea(this, false);
            UpdateWidgetToTable(viewer);
        }
        /// <summary>
        /// Layouts the specified viewer.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void Layout(LayoutViewer viewer)
        {
            //If the content is in text body, updates the client area based on section formattings.
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                viewer.UpdateClientArea(OwnerTable.Section.SectionFormat);
#if !WPF
            });
#endif
            Widget prevWidget = null;
            if (RowIndex > 0)
                prevWidget = OwnerTable.Rows[RowIndex - 1].tableRowWidgets[OwnerTable.Rows[RowIndex - 1].tableRowWidgets.Count - 1];
            else
            {
                BlockAdv prevBlock = OwnerTable.PreviousBlock;
                if (prevBlock is ParagraphAdv)
                    prevWidget = (prevBlock as ParagraphAdv).ParagraphWidgets[(prevBlock as ParagraphAdv).ParagraphWidgets.Count - 1];
                else if (prevBlock is TableAdv)
                    prevWidget = (prevBlock as TableAdv).TableWidgets[(prevBlock as TableAdv).TableWidgets.Count - 1];
            }
            if (prevWidget != null)
                viewer.CutFromTop(prevWidget.Location.Y + prevWidget.Height);
            TableWidget tableWidget = tableRowWidgets[0].ContainerWidget as TableWidget;
            ClearWidgets();
            if (OwnerTable.TableWidgets.Count == 0)
            {
                OwnerTable.AddTableWidget(viewer.ClientActiveArea);
                tableWidget = OwnerTable.TableWidgets[0] as TableWidget;
            }
            viewer.UpdateClientArea(tableWidget);
            BlockAdv block = Cells.Count > 0 ? Cells[0].GetFirstBlock() : null;
            if (block != null && viewer.OwnerControl.IsDocumentLoaded && !viewer.OwnerControl.IsPastingContent)
                //Updates list values of previous rendered paragraphs.
                viewer.OwnerControl.Document.UpdateListItems(block);
            LayoutItems(viewer);
            LayoutNextItems(viewer);
        }
        /// <summary>
        /// Layouts the next items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void LayoutNextItems(LayoutViewer viewer)
        {
            TableRowAdv tableRow = this;
            TableRowWidget prevRowWidget = tableRowWidgets[tableRowWidgets.Count - 1];
            while (tableRow.NextNode is TableRowAdv)
            {
                tableRow = tableRow.NextNode as TableRowAdv;
                if (tableRow is TableRowAdv && tableRow.tableRowWidgets.Count > 0)
                {
                    if (tableRow.tableRowWidgets[0].ContainerWidget == prevRowWidget.ContainerWidget
                        && Math.Round(tableRow.tableRowWidgets[0].Location.Y, 2) == Math.Round(viewer.ClientActiveArea.Y, 2))
                        return;
                    prevRowWidget = tableRow.tableRowWidgets[0];
                }
                tableRow.ClearWidgets();
                tableRow.LayoutItems(viewer);
            }
            if (OwnerTable.TableWidgets[OwnerTable.TableWidgets.Count - 1].ContainerWidget is Widget)
            {
                TableWidget tableWidget = OwnerTable.TableWidgets[OwnerTable.TableWidgets.Count - 1] as TableWidget;
                tableWidget.ContainerWidget.ChildWidgets.Remove(tableWidget);
                if (!(tableWidget.ContainerWidget is TableCellWidget))
                    tableWidget.ContainerWidget.Height -= tableWidget.Height;
            }
            OwnerTable.UpdateWidgetToPage(viewer);
            viewer.UpdateClientArea(OwnerTable, false);
            OwnerTable.LayoutNextItems(viewer);
        }
        /// <summary>
        /// Adds the table row widget.
        /// </summary>
        /// <param name="area">The area.</param>
        internal void AddTableRowWidget(Rect area)
        {
            TableRowWidget tableRowWidget = new TableRowWidget(this);
            tableRowWidget.Location = new Point(area.X, area.Y);
            tableRowWidget.Width = area.Width;
            tableRowWidgets.Add(tableRowWidget);
        }
        /// <summary>
        /// Adds the widget to table.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="tableRowWidget">The table row widget.</param>
        private void AddWidgetToTable(LayoutViewer viewer, TableRowWidget tableRowWidget)
        {
            //Adds table row widget to owner table widget.
            TableWidget tableWidget = OwnerTable.TableWidgets[0] as TableWidget;
            int index = tableWidget.ChildWidgets.Count;
            TableRowWidget prevWidget = null;
            int rowWidgetIndex = tableRowWidgets.IndexOf(tableRowWidget);
            if (rowWidgetIndex > 0)
                prevWidget = tableRowWidgets[rowWidgetIndex - 1];
            else if (PreviousNode is TableRowAdv
                && (PreviousNode as TableRowAdv).tableRowWidgets.Count > 0)
                prevWidget = (PreviousNode as TableRowAdv).tableRowWidgets[(PreviousNode as TableRowAdv).tableRowWidgets.Count - 1];
            if (prevWidget == null)
            {
                if (index > 0)
                {
                    for (int i = tableWidget.ChildWidgets.Count - 1; i >= 0; i--)
                    {
                        TableRowWidget widget = tableWidget.ChildWidgets[i] as TableRowWidget;
                        if (widget.TableRow.RowIndex < tableRowWidget.TableRow.RowIndex)
                        {
                            index = i + 1;
                            break;
                        }
                        else
                            index = i;
                    }
                    if (index == 0)
                        tableWidget.UpdateWidgetLocation(tableRowWidget);
                }
            }
            else
            {
                tableWidget = prevWidget.ContainerWidget as TableWidget;
                index = tableWidget.ChildWidgets.Count;
                if (tableWidget.ChildWidgets.Contains(prevWidget))
                    index = tableWidget.ChildWidgets.IndexOf(prevWidget) + 1;
                if (Math.Round(tableRowWidget.Location.Y, 2) != Math.Round(prevWidget.Location.Y + prevWidget.Height, 2))
                {
                    int prevIndex = OwnerTable.TableWidgets.IndexOf(tableWidget);
                    tableWidget = OwnerTable.TableWidgets[prevIndex + 1] as TableWidget;
                    index = tableWidget.ChildWidgets.Count;
                }
                if (rowWidgetIndex > 0)
                    index = 0;
            }
            UpdateRowHeightBySpannedCell(tableWidget, tableRowWidget);
            tableWidget.ChildWidgets.Insert(index, tableRowWidget);
            tableRowWidget.ContainerWidget = tableWidget;
            tableWidget.Height = tableWidget.Height + tableRowWidget.Height;
            if (tableWidget.ContainerWidget != null
                && tableWidget.ContainerWidget.ChildWidgets.Contains(tableWidget))
                tableWidget.ContainerWidget.Height += tableRowWidget.Height;
            tableRowWidget.UpdateHeight(viewer);
            viewer.CutFromTop(tableRowWidget.Location.Y + tableRowWidget.Height);
        }
        /// <summary>
        /// Updates the row height by spanned cell.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        /// <param name="rowWidget">The row widget.</param>
        internal void UpdateRowHeightBySpannedCell(TableWidget tableWidget, TableRowWidget rowWidget)
        {
            //Back track to previous table row widgets and update it height if vertical merge ends with this row.
            for (int i = 0; i < tableWidget.ChildWidgets.Count; i++)
            {
                TableRowWidget prevRowWidget = tableWidget.ChildWidgets[i] as TableRowWidget;
                for (int j = 0; j < prevRowWidget.ChildWidgets.Count; j++)
                {
                    TableCellWidget cellWidget = prevRowWidget.ChildWidgets[j] as TableCellWidget;
                    int rowSpan = 1;
#if WPF
                    rowSpan = cellWidget.TableCell.CellFormat.RowSpan;
#else
                    UIDispatcher.Execute(() => rowSpan = cellWidget.TableCell.CellFormat.RowSpan);
#endif
                    if (RowIndex - cellWidget.TableCell.OwnerRow.RowIndex == rowSpan - 1)
                    {
                        double mergedCellHeight = cellWidget.Location.Y + cellWidget.Height + cellWidget.Margin.Bottom - rowWidget.Location.Y;
                        if (rowWidget.Height < mergedCellHeight)
                            rowWidget.Height = mergedCellHeight;
                    }
                }
            }
        }
        /// <summary>
        /// Updates the widget to table.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateWidgetToTable(LayoutViewer viewer)
        {
            //viewer.ClientArea.Bottom - Cell spacing
            int count = tableRowWidgets.Count - 1;
            TableRowWidget tableRowWidget = tableRowWidgets[count];
            while (count < tableRowWidgets.Count)
            {
                count = tableRowWidgets.Count;
                if (viewer is FlowLayoutViewer
                    || OwnerTable.IsInsideTable
                    || tableRowWidget.Location.Y + tableRowWidget.Height <= viewer.ClientArea.Bottom)
                {
                    AddWidgetToTable(viewer, tableRowWidget);
                }
                else
                {
                    TableRowWidget splittedWidget = tableRowWidget;
                    //ToDo: Check whether row included in vertical merge or AllowRowSplitbyPage is true, if so split row.
                    //Checks if atleast first line fits in the client area.
                    if (tableRowWidget.IsFirstLineFit(viewer.ClientArea.Bottom))
                        splittedWidget = tableRowWidget.GetSplittedWidget(viewer.ClientArea.Bottom);
                    if (splittedWidget != null)
                    {
                        if (splittedWidget != tableRowWidget)
                        {
                            AddWidgetToTable(viewer, tableRowWidget);
                            //Updates the fitted table rows to current page.
                            OwnerTable.UpdateWidgetToPage(viewer);
                            int index = OwnerTable.TableWidgets.IndexOf(tableRowWidget.ContainerWidget);
                            if (index + 1 >= OwnerTable.TableWidgets.Count)
                                //Creates new table widget for splitted rows.
                                OwnerTable.AddTableWidget(viewer.ClientActiveArea);
                            tableRowWidget = splittedWidget;
                        }
                        else
                        {
                            if (RowIndex > 0)
                            {
                                //Updates the fitted table rows to current page.
                                OwnerTable.UpdateWidgetToPage(viewer);
                                if (PreviousNode is TableRowAdv
                                    && (PreviousNode as TableRowAdv).tableRowWidgets.Count > 0)
                                {
                                    TableRowWidget prevWidget = (PreviousNode as TableRowAdv).tableRowWidgets[(PreviousNode as TableRowAdv).tableRowWidgets.Count - 1];
                                    if (Math.Round(tableRowWidget.Location.Y, 2) == Math.Round(prevWidget.Location.Y + prevWidget.Height, 2))
                                    {
                                        int prevIndex = OwnerTable.TableWidgets.IndexOf(prevWidget.ContainerWidget);
                                        if (prevIndex + 1 >= OwnerTable.TableWidgets.Count)
                                            //Creates new table widget for splitted rows.
                                            OwnerTable.AddTableWidget(viewer.ClientActiveArea);
                                    }
                                }
                                else
                                    //Creates new table widget for splitted rows.
                                    OwnerTable.AddTableWidget(viewer.ClientActiveArea);
                            }
                            count--;
                        }
                        TableWidget tableWidget = OwnerTable.TableWidgets[OwnerTable.TableWidgets.Count - 1] as TableWidget;
                        BodyWidget prevBodyWidget = null;
                        if (OwnerTable.TableWidgets.Count > 1)
                            prevBodyWidget = OwnerTable.TableWidgets[OwnerTable.TableWidgets.Count - 2].ContainerWidget as BodyWidget;
                        else
                        {
                            BlockAdv previousBlock = OwnerTable.PreviousBlock;
                            if (previousBlock is ParagraphAdv)
                                prevBodyWidget = (previousBlock as ParagraphAdv).ParagraphWidgets[(previousBlock as ParagraphAdv).ParagraphWidgets.Count - 1].ContainerWidget as BodyWidget;
                            else if (previousBlock is TableAdv)
                                prevBodyWidget = (previousBlock as TableAdv).TableWidgets[(previousBlock as TableAdv).TableWidgets.Count - 1].ContainerWidget as BodyWidget;
                        }
                        int pageIndex = 0;
                        if (prevBodyWidget != null)
                            pageIndex = viewer.Pages.IndexOf(prevBodyWidget.Page);
                        if (pageIndex == viewer.Pages.Count - 1
                            || viewer.Pages[pageIndex + 1].Section != OwnerTable.Section)
                        {
                            //Creates new page
                            PageAdv page = viewer.CreateNewPage(OwnerTable.Section);
                            if (viewer.Pages[pageIndex + 1].Section != OwnerTable.Section)
                                viewer.InsertPage(pageIndex + 1, page);
                        }
                        else
                        {
                            viewer.Pages[pageIndex + 1].BoundingRectangle = new Rect(viewer.Pages[pageIndex + 1].BoundingRectangle.X, viewer.Pages[pageIndex].BoundingRectangle.Bottom + 20, viewer.Pages[pageIndex + 1].BoundingRectangle.Width, viewer.Pages[pageIndex + 1].BoundingRectangle.Height);
                            //Updates the client area.
#if !WPF
                            UIDispatcher.Execute(() =>
                            {
#endif
                                viewer.UpdateClientArea(OwnerTable.Section.SectionFormat);
#if !WPF
                            });
#endif
                        }
                        //Updates table widgets location.
                        viewer.UpdateClientArea(OwnerTable, true);
                        //Update splitted row widget location.
                        splittedWidget.Location = new Point(splittedWidget.Location.X, tableWidget.Location.Y);
                        splittedWidget.UpdateChildLocation(tableWidget.Location.Y);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the blank row.
        /// </summary>
        /// <returns></returns>
        internal TableRowAdv GetBlankRow()
        {
            TableRowAdv row = new TableRowAdv();
            foreach (TableCellAdv cell in Cells)
            {
                TableCellAdv newCell = cell.GetBlankCell();
                row.Cells.Add(newCell);
            }
            return row;
        }
        /// <summary>
        /// Gets the next selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetNextSelection(SelectionAdv selection)
        {
            if (NextNode != null)
            {
                bool isForwardSelection = selection.IsEmpty || selection.IsForward;
                if (isForwardSelection)
                {
                    TableCellAdv cell = (NextNode as TableRowAdv).Cells[(NextNode as TableRowAdv).Cells.Count - 1];
                    BlockAdv block = cell.Blocks[cell.Blocks.Count - 1];
                    if (block is ParagraphAdv)
                        return block as ParagraphAdv;
                    else
                        return (block as TableAdv).GetLastParagraphInLastCell();
                }
                else
                    return (NextNode as TableRowAdv).GetNextParagraph(selection);
            }
            return OwnerTable.GetNextSelection(selection);
        }
        /// <summary>
        /// Gets the next paragraph.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetNextParagraph(SelectionAdv selection)
        {
            //Iterate the exact cell based on UP/Down selection length.
            TableCellAdv cell = Cells[0];
            if (selection.Start.Paragraph.IsInsideTable
                && OwnerTable.Contains(selection.Start.Paragraph.AssociatedCell))
            {
                TableCellAdv startCell = OwnerTable.GetCellInTable(selection.Start.Paragraph.AssociatedCell);
                cell = GetFirstCellInRegion(startCell, selection.UpDownSelectionLength);
            }
            BlockAdv block = cell.Blocks[0];
            if (block is ParagraphAdv)
                return block as ParagraphAdv;
            else
                return (block as TableAdv).GetFirstParagraphInFirstCell();
        }
        /// <summary>
        /// Gets the previous selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousSelection(SelectionAdv selection)
        {
            if (PreviousNode != null)
            {
                if (!selection.IsForward)
                {
                    TableCellAdv cell = (PreviousNode as TableRowAdv).Cells[0];
                    BlockAdv block = cell.Blocks[0];
                    if (block is ParagraphAdv)
                        return block as ParagraphAdv;
                    else
                        return (block as TableAdv).GetFirstParagraphInFirstCell();
                }
                else
                    return (PreviousNode as TableRowAdv).GetPreviousParagraph(selection);
            }
            return OwnerTable.GetPreviousSelection(selection);
        }
        /// <summary>
        /// Gets the previous paragraph.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousParagraph(SelectionAdv selection)
        {
            //Iterate the exact cell based on UP/Down selection length.
            TableCellAdv cell = Cells[Cells.Count - 1];
            if (selection.Start.Paragraph.IsInsideTable
                && OwnerTable.Contains(selection.Start.Paragraph.AssociatedCell))
            {
                TableCellAdv startCell = OwnerTable.GetCellInTable(selection.Start.Paragraph.AssociatedCell);
                cell = GetLastCellInRegion(startCell, selection.UpDownSelectionLength);
            }
            BlockAdv block = cell.Blocks[cell.Blocks.Count - 1];
            if (block is ParagraphAdv)
                return block as ParagraphAdv;
            else
                return (block as TableAdv).GetLastParagraphInLastCell();
        }
        /// <summary>
        /// Gets the next paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetNextParagraph()
        {
            if (NextNode != null)
            {
                TableCellAdv cell = (NextNode as TableRowAdv).Cells[0];
                BlockAdv block = cell.Blocks[0];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetFirstParagraphInFirstCell();
            }
            return OwnerTable.GetNextParagraph();
        }
        /// <summary>
        /// Gets the previous paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousParagraph()
        {
            if (PreviousNode != null)
            {
                TableCellAdv cell = (PreviousNode as TableRowAdv).Cells[(PreviousNode as TableRowAdv).Cells.Count - 1];
                BlockAdv block = cell.Blocks[cell.Blocks.Count - 1];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetLastParagraphInLastCell();
            }
            return OwnerTable.GetPreviousParagraph();
        }
        /// <summary>
        /// Determines whether this row contains the specified table cell.
        /// </summary>
        /// <param name="tableCell">The table cell.</param>
        /// <returns>
        ///   <c>true</c> if this row contains the specified table cell; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(TableCellAdv tableCell)
        {
            if (Cells.Contains(tableCell))
                return true;
            while (tableCell.OwnerTable.IsInsideTable)
            {
                if (Cells.Contains(tableCell))
                    return true;
                tableCell = tableCell.OwnerTable.AssociatedCell;
            }
            return Cells.Contains(tableCell);
        }
        /// <summary>
        /// Gets the first cell in region.
        /// </summary>
        /// <param name="startCell">The start cell.</param>
        /// <param name="selectionLength">Length of the selection.</param>
        /// <returns></returns>
        internal TableCellAdv GetFirstCellInRegion(TableCellAdv startCell, double selectionLength)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            if (start <= selectionLength && selectionLength < end)
            {
                for (int i = 0; i < Cells.Count; i++)
                {
                    double left = GetCellLeft(Cells[i]);
                    if (start <= left && left < end)
                        return Cells[i];
                }
            }
            else
            {
                for (int i = 0; i < Cells.Count; i++)
                {
                    double left = GetCellLeft(Cells[i]);
                    if (left <= selectionLength && left + Cells[i].CellFormat.CellWidth > selectionLength)
                        return Cells[i];
                }
            }
            return Cells[0];
        }
        /// <summary>
        /// Gets the last cell in region.
        /// </summary>
        /// <param name="startCell">The start cell.</param>
        /// <param name="selectionLength">Length of the selection.</param>
        /// <returns></returns>
        internal TableCellAdv GetLastCellInRegion(TableCellAdv startCell, double selectionLength)
        {
            double start = startCell.OwnerRow.GetCellLeft(startCell);
            double end = start + startCell.CellFormat.CellWidth;
            if (start <= selectionLength && selectionLength < end)
            {
                for (int i = Cells.Count - 1; i >= 0; i--)
                {
                    double left = GetCellLeft(Cells[i]);
                    if (start <= left && left < end)
                        return Cells[i];
                }
            }
            else
            {
                for (int i = Cells.Count - 1; i >= 0; i--)
                {
                    double left = GetCellLeft(Cells[i]);
                    if (left <= selectionLength && left + Cells[i].CellFormat.CellWidth > selectionLength)
                        return Cells[i];
                }
            }
            return Cells[Cells.Count - 1];
        }
        /// <summary>
        /// Gets the cell left.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        internal double GetCellLeft(TableCellAdv cell)
        {
            double left = 0;
            if (cell.TableCellWidgets.Count > 0)
                left += cell.TableCellWidgets[0].Location.X - cell.TableCellWidgets[0].Margin.Left;
            //if (TableRowWidgets.Count > 0)
            //    left += TableRowWidgets[0].Location.X;
            //int count = Cells.IndexOf(cell);
            //if (count == 0)
            //{
            //    int prevColumnIndex = 0;
            //    double prevSpannedCellWidth = 0;
            //    if (prevColumnIndex < Cells[0].ColumnIndex)
            //        prevSpannedCellWidth = OwnerTable.TableHolder.GetPreviousSpannedCellWidth(prevColumnIndex, Cells[0].ColumnIndex);
            //    left += prevSpannedCellWidth;
            //}
            //for (int i = 0; i < count; i++)
            //{
            //    int prevColumnIndex = 0;
            //    if (Cells[i].PreviousNode != null)
            //        prevColumnIndex = (Cells[i].PreviousNode as TableCellAdv).ColumnIndex + (Cells[i].PreviousNode as TableCellAdv).CellFormat.ColumnSpan;
            //    double prevSpannedCellWidth = 0;
            //    if (prevColumnIndex < Cells[i].ColumnIndex)
            //        prevSpannedCellWidth = OwnerTable.TableHolder.GetPreviousSpannedCellWidth(prevColumnIndex, Cells[i].ColumnIndex);
            //    left += Cells[i].CellFormat.CellWidth + prevSpannedCellWidth;
            //}
            return left;
        }
        #endregion

        #region Highlight Selected Contents
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void Highlight(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].Highlight(selection);
            }
            if (end.Paragraph.IsInsideTable
                && Contains(end.Paragraph.AssociatedCell))
                return;
            else if (NextNode is TableRowAdv)
                (NextNode as TableRowAdv).Highlight(selection, start, end);
        }
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void Highlight(SelectionAdv selection, double start, double end)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                double left = GetCellLeft(Cells[i]);
                if (start <= left && left < end)
                    Cells[i].Highlight(selection);
            }
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
        internal void ApplyCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            value = Cells[0].GetCharacterFormatValue(property, value);
            for (int i = RowIndex; i < OwnerTable.Rows.Count; i++)
            {
                TableRowAdv row = OwnerTable.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].ApplyCharacterFormat(selection, property, value);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = OwnerTable.GetNextRenderedBlock();
            //Goto the next block.
            block.ApplyCharacterFormat(selection, start, end, property, value);
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
        internal void ApplyParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            value = Cells[0].GetParagraphFormatValue(property, value);
            for (int i = RowIndex; i < OwnerTable.Rows.Count; i++)
            {
                TableRowAdv row = OwnerTable.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].ApplyParagraphFormat(selection, property, value);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = OwnerTable.GetNextRenderedBlock();
            //Goto the next block.
            block.ApplyParagraphFormat(selection, start, end, property, value);
        }
        #endregion

        #region Get Character Format for Selected Contents
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void GetCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            for (int i = RowIndex; i < OwnerTable.Rows.Count; i++)
            {
                TableRowAdv row = OwnerTable.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].GetCharacterFormat(selection);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = OwnerTable.GetNextRenderedBlock();
            //Goto the next block.
            block.GetCharacterFormat(selection, start, end);
        }
        #endregion

        #region Get Paragraph Format for Selected Contents
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void GetParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            for (int i = RowIndex; i < OwnerTable.Rows.Count; i++)
            {
                TableRowAdv row = OwnerTable.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    row.Cells[j].GetParagraphFormat(selection);
                }
                if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                    return;
            }
            BlockAdv block = OwnerTable.GetNextRenderedBlock();
            //Goto the next block.
            block.GetParagraphFormat(selection, start, end);
        }
        #endregion
    }
}
