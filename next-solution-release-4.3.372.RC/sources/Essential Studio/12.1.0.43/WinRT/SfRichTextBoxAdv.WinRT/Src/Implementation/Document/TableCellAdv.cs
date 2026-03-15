#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
#if WPF
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.Foundation;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
#if WPF
    [ContentProperty("Blocks")]
#else
    [ContentProperty(Name = "Blocks")]
#endif
    public class TableCellAdv : CompositeNode
    {
        #region Fields
        int colIndex;
        List<TableCellWidget> tableCellWidgets;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        internal SectionAdv Section
        {
            get
            {
                if (OwnerTable != null)
                    return OwnerTable.Section;
                return null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is in header footer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in header footer; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInHeaderFooter
        {
            get
            {
                if (OwnerTable != null)
                    return OwnerTable.IsInHeaderFooter;
                return false;
            }
        }
        /// <summary>
        /// Gets the owner row.
        /// </summary>
        /// <value>
        /// The owner row.
        /// </value>
        internal TableRowAdv OwnerRow
        {
            get
            {
                return Owner as TableRowAdv;
            }
        }
        /// <summary>
        /// Gets the owner table.
        /// </summary>
        /// <value>
        /// The owner table.
        /// </value>
        internal TableAdv OwnerTable
        {
            get
            {
                if (OwnerRow != null)
                    return OwnerRow.OwnerTable;
                else
                    return null;
            }
        }
        /// <summary>
        /// Gets the blocks.
        /// </summary>
        /// <value>
        /// The blocks.
        /// </value>
        public BlockAdvCollection Blocks
        {
            get
            {
                return ChildNodes as BlockAdvCollection;
            }
        }
        /// <summary>
        /// Gets the index of the cell.
        /// </summary>
        /// <value>
        /// The index of the cell.
        /// </value>
        internal int CellIndex
        {
            get
            {
                if (OwnerRow != null)
                    return OwnerRow.Cells.IndexOf(this);
                return -1;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is last cell.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is last cell; otherwise, <c>false</c>.
        /// </value>
        internal bool IsLastCell
        {
            get
            {
                TableRowAdv row = null;
                TableCellAdv cell = null;
                if (OwnerTable.Rows.Count > 0)
                {
                    row = OwnerTable.Rows[OwnerTable.Rows.Count - 1];
                    if (row == OwnerRow)
                    {
                        if (row.Cells.Count > 0)
                        {
                            cell = row.Cells[row.Cells.Count - 1];
                            if (cell != null)
                                return true;
                        }
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Gets or sets the index of the column.
        /// </summary>
        /// <value>
        /// The index of the column.
        /// </value>
        internal int ColumnIndex
        {
            get
            {
                return colIndex;
            }
            set
            {
                colIndex = value;
            }
        }
        /// <summary>
        /// Gets or sets the cell format.
        /// </summary>
        /// <value>
        /// The cell format.
        /// </value>
        public CellFormat CellFormat
        {
            get
            {
                return (CellFormat)GetValue(CellFormatProperty);
            }
            set
            {
                SetValue(CellFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets the owner column.
        /// </summary>
        /// <value>
        /// The owner column.
        /// </value>
        internal TableColumnAdv OwnerColumn
        {
            get
            {
                return OwnerTable.TableHolder.Columns[ColumnIndex];
            }
        }
        /// <summary>
        /// Gets the table cell widgets.
        /// </summary>
        /// <value>
        /// The table cell widgets.
        /// </value>
        internal List<TableCellWidget> TableCellWidgets
        {
            get
            {
                return tableCellWidgets;
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the CellFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the CellFormat dependency property.</returns>
        internal static readonly DependencyProperty CellFormatProperty = DependencyProperty.Register("CellFormat", typeof(CellFormat), typeof(TableCellAdv), new PropertyMetadata(null, OnCellFormatChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when cell format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCellFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as CellFormat).SetOwner(d as TableCellAdv);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCellAdv"/> class.
        /// </summary>
        public TableCellAdv()
            : this(null)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCellAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal TableCellAdv(Node owner)
            : base(owner)
        {
            ChildNodes = new BlockAdvCollection(this);
            CellFormat = new CellFormat(this);
            tableCellWidgets = new List<TableCellWidget>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Combines the table cell widgets.
        /// </summary>
        /// <param name="rowWidget">The row widget.</param>
        internal void CombineTableCellWidgets(TableRowWidget rowWidget)
        {
            if (tableCellWidgets.Count == 0)
                return;
            TableCellWidget cellWidget = tableCellWidgets[0] as TableCellWidget;
            if (!rowWidget.ChildWidgets.Contains(cellWidget))
                cellWidget.UpdateTableRowWidget(rowWidget, rowWidget.ChildWidgets.Count);
            cellWidget.UpdateWidgetHeight();
            for (int i = 0; i < Blocks.Count; i++)
            {
                BlockAdv block = Blocks[i];
                if (block is ParagraphAdv)
                    (block as ParagraphAdv).CombineParagraphWidgets(cellWidget);
                else
                    (block as TableAdv).CombineTableWidgets(cellWidget);
            }
            for (int i = 1; i < tableCellWidgets.Count; i++)
            {
                TableCellWidget curWidget = tableCellWidgets[i] as TableCellWidget;
                curWidget.Dispose();
                i--;
            }
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal TableCellAdv Clone()
        {
            TableCellAdv cell = new TableCellAdv();
            cell.CellFormat.CopyFormat(CellFormat);
            foreach (BlockAdv block in Blocks)
            {
                cell.Blocks.Add(block.Clone());
            }
            return cell;
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal void ClearWidgets()
        {
            if (tableCellWidgets != null
                && tableCellWidgets.Count > 0)
            {
                //Removes the table cell widget.
                for (int i = 0; i < tableCellWidgets.Count; i++)
                {
                    TableCellWidget widget = tableCellWidgets[i] as TableCellWidget;
                    widget.Dispose();
                    tableCellWidgets.Remove(widget);
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
            CellFormat.Dispose();
            ClearValue(CellFormatProperty);
            ClearWidgets();
            for (int i = 0; i < Blocks.Count; i++)
            {
                BlockAdv block = Blocks[i];
                block.Dispose();
                Blocks.Remove(block);
                i--;
            }
        }
        /// <summary>
        /// Updates the list items.
        /// </summary>
        /// <param name="paragraph">The block.</param>
        internal void UpdateListItems(BlockAdv block)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].UpdateListItems(block);
            }
        }
        /// <summary>
        /// Updates the rendered list items.
        /// </summary>
        internal void UpdateRenderedListItems()
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].UpdateRenderedListItems();
            }
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void LayoutItems(LayoutViewer viewer)
        {
            if (tableCellWidgets != null
                && tableCellWidgets.Count > 0)
            {
                //Removes the table cell widget.
                tableCellWidgets.Clear();
            }
            AddTableCellWidget(viewer.ClientActiveArea);
            viewer.UpdateClientArea(this, true);
            if (this.Blocks.Count == 0)
            {
                //Creates an empty paragraph in cell.
                ParagraphAdv paragraph = null;
#if WPF
                paragraph = new ParagraphAdv(this);
#else
                UIDispatcher.Execute(() => paragraph = new ParagraphAdv(this));
#endif
                this.Blocks.Add(paragraph);
            }
            for (int i = 0; i < Blocks.Count; i++)
            {
                viewer.UpdateClientArea(Blocks[i], true);
                //Update client area based on the nested block.
                Blocks[i].LayoutItems(viewer);
                viewer.UpdateClientArea(Blocks[i], false);
            }
            UpdateWidgetToRow(viewer);
            viewer.UpdateClientArea(this, false);
        }
        /// <summary>
        /// Adds the table cell widget.
        /// </summary>
        /// <param name="area">The area.</param>
        internal void AddTableCellWidget(Rect area)
        {
            TableCellWidget tableCellWidget = new TableCellWidget(this);
            int prevColumnIndex = 0;
#if !WPF
            UIDispatcher.Execute(() =>
                {
#endif
                    double left = CellFormat.CellMargin.Left;
                    double top = CellFormat.CellMargin.Top;
                    double right = CellFormat.CellMargin.Right;
                    double bottom = CellFormat.CellMargin.Bottom;
                    left += left < 0.5 ? 0.5 - left : 0;
                    top += top < 0.5 ? 0.5 - top : 0;
                    right += right < 0.5 ? 0.5 - right : 0;
                    bottom += bottom < 0.5 ? 0.5 - bottom : 0;
                    tableCellWidget.Margin = new Thickness(left, top, right, bottom);
                    tableCellWidget.Width = CellFormat.CellWidth;
                    if (PreviousNode != null)
                        prevColumnIndex = (PreviousNode as TableCellAdv).ColumnIndex + (PreviousNode as TableCellAdv).CellFormat.ColumnSpan;
#if !WPF
                });
#endif
            tableCellWidgets.Add(tableCellWidget);
            double prevSpannedCellWidth = 0;
            if (prevColumnIndex < ColumnIndex)
                prevSpannedCellWidth = OwnerTable.TableHolder.GetPreviousSpannedCellWidth(prevColumnIndex, ColumnIndex);
            tableCellWidget.Location = new Point(area.X + prevSpannedCellWidth + tableCellWidget.Margin.Left, area.Y + tableCellWidget.Margin.Top);
            tableCellWidget.Width = tableCellWidget.Width - tableCellWidget.Margin.Left - tableCellWidget.Margin.Right;
        }
        /// <summary>
        /// Updates the widget to row.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateWidgetToRow(LayoutViewer viewer)
        {
            TableCellWidget tableCellWidget = tableCellWidgets[tableCellWidgets.Count - 1];
            //Adds table cell widget to owner row widget.
            TableRowWidget rowWidget = OwnerRow.TableRowWidgets[OwnerRow.TableRowWidgets.Count - 1];
            double cellLeft = rowWidget.Location.X;
            if (rowWidget.ChildWidgets.Count > 0)
            {
                TableCellWidget lastWidget = rowWidget.ChildWidgets[rowWidget.ChildWidgets.Count - 1] as TableCellWidget;
                cellLeft = lastWidget.Location.X + lastWidget.Width + lastWidget.Margin.Right;
            }
            rowWidget.ChildWidgets.Add(tableCellWidget);
            tableCellWidget.ContainerWidget = rowWidget;
            tableCellWidget.Height = viewer.ClientActiveArea.Y - tableCellWidget.Location.Y;
            //Add condition not cell merged vertically.
#if !WPF
            UIDispatcher.Execute(() =>
                {
#endif
                    if (CellFormat.RowSpan == 1)
                    {
                        double cellHeight = tableCellWidget.Height + tableCellWidget.Margin.Top + tableCellWidget.Margin.Bottom;
                        if (rowWidget.Height < cellHeight)
                            rowWidget.Height = cellHeight;
                    }
#if !WPF
                });
#endif
        }
        /// <summary>
        /// Gets the blank cell.
        /// </summary>
        /// <returns></returns>
        internal TableCellAdv GetBlankCell()
        {
            TableCellAdv cell = new TableCellAdv();
            cell.CellFormat.CopyFormat(CellFormat);
            ParagraphAdv lastParagraph = GetLastParagraph();
            ParagraphAdv newParagraph = new ParagraphAdv();
            newParagraph.ParagraphFormat.CopyFormat(lastParagraph.ParagraphFormat);
            newParagraph.CharacterFormat.CopyFormat(lastParagraph.CharacterFormat);
            cell.Blocks.Add(newParagraph);
            return cell;
        }
        /// <summary>
        /// Determines whether this cell contains the specified nested cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns>
        ///   <c>true</c> if this cell contains the specified nested cell; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(TableCellAdv cell)
        {
            if (this == cell)
                return true;
            while (cell.OwnerTable.IsInsideTable)
            {
                if (this == cell.OwnerTable.AssociatedCell)
                    return true;
                cell = cell.OwnerTable.AssociatedCell;
            }
            return false;
        }
        /// <summary>
        /// Gets the container cell.
        /// </summary>
        /// <returns></returns>
        internal TableCellAdv GetContainerCell()
        {
            TableCellAdv cell = this;
            while (cell.OwnerTable != null && cell.OwnerTable.IsInsideTable)
            {
                cell = cell.OwnerTable.AssociatedCell;
            }
            return cell;
        }
        /// <summary>
        /// Gets the container cell.
        /// </summary>
        /// <param name="tableCell">The table cell.</param>
        /// <returns></returns>
        internal TableCellAdv GetContainerCell(TableCellAdv tableCell)
        {
            TableCellAdv cell = this;
            while (cell.OwnerTable.IsInsideTable)
            {
                if (cell.OwnerTable.Contains(tableCell))
                    return cell;
                cell = cell.OwnerTable.AssociatedCell;
            }
            return cell;
        }
        /// <summary>
        /// Gets the selected cell.
        /// </summary>
        /// <param name="containerCell">The container cell.</param>
        /// <returns></returns>
        internal TableCellAdv GetSelectedCell(TableCellAdv containerCell)
        {
            TableCellAdv cell = this;
            if (cell.OwnerTable == containerCell.OwnerTable)
                return cell;
            while (cell.OwnerTable.IsInsideTable)
            {
                if (cell.OwnerTable.AssociatedCell == containerCell)
                    return cell;
                cell = cell.OwnerTable.AssociatedCell;
            }
            return cell;
        }
        /// <summary>
        /// Determines whether this cell is selected.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        /// <returns>
        ///   <c>true</c> if this cell is selected; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsCellSelected(TextPosition startPosition, TextPosition endPosition)
        {
            ParagraphAdv lastParagraph = GetLastParagraph();

            bool isAtCellEnd = lastParagraph == endPosition.Paragraph && endPosition.Offset == lastParagraph.GetLength() + 1;

            return isAtCellEnd || (!Contains(startPosition.Paragraph.AssociatedCell) || !Contains(endPosition.Paragraph.AssociatedCell));
        }
        /// <summary>
        /// Updates the edit position.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void UpdateEditPosition(SelectionAdv selection)
        {
            ParagraphAdv firstParagraph = GetFirstParagraph();
            selection.EditPosition = firstParagraph.GetHierarchicalIndex("0");
        }
        #endregion

        #region Navigation methods
        /// <summary>
        /// Gets the next selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetNextSelection(SelectionAdv selection)
        {
            if (NextNode != null)
            {
                if (selection.IsEmpty || selection.IsForward)
                {
                    BlockAdv block = (NextNode as TableCellAdv).Blocks[(NextNode as TableCellAdv).Blocks.Count - 1];
                    if (block is ParagraphAdv)
                        return block as ParagraphAdv;
                    else
                        return (block as TableAdv).GetLastParagraphInLastCell();
                }
                else
                {
                    //Return first paragraph in cell. 
                    BlockAdv block = (NextNode as TableCellAdv).Blocks[0];
                    if (block is ParagraphAdv)
                        return block as ParagraphAdv;
                    else
                        return (block as TableAdv).Rows[0].GetNextParagraph(selection);
                }
            }
            return OwnerRow.GetNextSelection(selection);
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
                    BlockAdv block = (PreviousNode as TableCellAdv).Blocks[0];
                    if (block is ParagraphAdv)
                        return block as ParagraphAdv;
                    else
                        return (block as TableAdv).GetFirstParagraphInLastRow();
                }
                else
                {
                    BlockAdv block = (PreviousNode as TableCellAdv).Blocks[(PreviousNode as TableCellAdv).Blocks.Count - 1];
                    if (block is ParagraphAdv)
                        return block as ParagraphAdv;
                    else
                        return (block as TableAdv).Rows[(block as TableAdv).Rows.Count - 1].GetPreviousParagraph(selection);
                }
            }
            return OwnerRow.GetPreviousSelection(selection);
        }
        /// <summary>
        /// Gets the next paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetNextParagraph()
        {
            if (NextNode != null)
            {
                //Return first paragraph in cell. 
                BlockAdv block = (NextNode as TableCellAdv).Blocks[0];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetFirstParagraphInFirstCell();
            }
            return OwnerRow.GetNextParagraph();
        }
        /// <summary>
        /// Gets the previous paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousParagraph()
        {
            if (PreviousNode != null)
            {
                BlockAdv block = (PreviousNode as TableCellAdv).Blocks[(PreviousNode as TableCellAdv).Blocks.Count - 1];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetLastParagraphInLastCell();
            }
            return OwnerRow.GetPreviousParagraph();
        }
        /// <summary>
        /// Gets the first paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetFirstParagraph()
        {
            BlockAdv firstBlock = Blocks[0];
            if (firstBlock is ParagraphAdv)
                return firstBlock as ParagraphAdv;
            else
                return (firstBlock as TableAdv).GetFirstParagraphInFirstCell();
        }
        /// <summary>
        /// Gets the first block.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetFirstBlock()
        {
            if (Blocks.Count > 0)
                return Blocks[0];
            return null;
        }
        /// <summary>
        /// Gets the last paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetLastParagraph()
        {
            BlockAdv lastBlock = Blocks[Blocks.Count - 1];
            if (lastBlock is ParagraphAdv)
                return lastBlock as ParagraphAdv;
            else
                return (lastBlock as TableAdv).GetLastParagraphInLastCell();
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
            if (end.Paragraph.IsInsideTable)
            {
                TableCellAdv containerCell = GetContainerCell(end.Paragraph.AssociatedCell);
                if (containerCell.OwnerTable.Contains(end.Paragraph.AssociatedCell))
                {
                    TableCellAdv startCell = GetSelectedCell(containerCell);
                    TableCellAdv endCell = end.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    if (containerCell.Contains(end.Paragraph.AssociatedCell))
                    {
                        //Selection end is in container cell.
                        if (containerCell.IsCellSelected(start, end))
                            containerCell.Highlight(selection);
                        else
                        {
                            if (startCell == containerCell)
                                start.Paragraph.Highlight(selection, start, end);
                            else
                                startCell.HighlightContainer(selection, start, end);
                        }
                    }
                    else
                    {
                        //Selection end is not in container cell.
                        containerCell.Highlight(selection);
                        if (containerCell.OwnerRow == endCell.OwnerRow)
                        {
                            //Highlight other selected cells in current row.
                            startCell = containerCell as TableCellAdv;
                            while (startCell.NextNode != null)
                            {
                                startCell = startCell.NextNode as TableCellAdv;
                                startCell.Highlight(selection);
                                if (startCell == endCell)
                                    break;
                            }
                        }
                        else
                            //Highlight other selected cells in current table.
                            containerCell.OwnerTable.HighlightCells(selection, containerCell, endCell);
                    }
                }
                else
                    containerCell.HighlightContainer(selection, start, end);
            }
            else
            {
                TableCellAdv cell = GetContainerCell();
                cell.HighlightContainer(selection, start, end);
            }
        }
        /// <summary>
        /// Highlights the container.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void HighlightContainer(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            OwnerRow.Highlight(selection, start, end);
            BlockAdv block = OwnerTable.GetNextRenderedBlock();
            //Goto the next block.
            block.Highlight(selection, start, end);
        }
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void Highlight(SelectionAdv selection)
        {
            for (int i = 0; i < tableCellWidgets.Count; i++)
            {
                tableCellWidgets[i].Highlight(selection);
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
        internal void Delete(SelectionAdv selection, TextPosition start, TextPosition end, byte editAction)
        {
            if (end.Paragraph.IsInsideTable)
            {
                TableCellAdv containerCell = GetContainerCell(end.Paragraph.AssociatedCell);
                if (containerCell.OwnerTable.Contains(end.Paragraph.AssociatedCell))
                {
                    TableCellAdv startCell = GetSelectedCell(containerCell);
                    TableCellAdv endCell = end.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    if (containerCell.Contains(end.Paragraph.AssociatedCell))
                    {
                        //Selection end is in container cell.
                        if (containerCell.IsCellSelected(start, end))
                        {
                            //Container cell is completely selected.
                            containerCell.UpdateEditPosition(selection);
                            if (editAction == 1)
                            {
                                //Specifically handled for backspace. Delete selected cell in current table.
                                OwnerRow.OwnerTable.DeleteCells(selection, containerCell, containerCell, editAction);
                            }
                            else
                            {
                                //Delete contents within table cell or Copy contents within table cell to clipboard.
                                bool isCellCleared = containerCell.Delete(selection, editAction,true);
                                if (!isCellCleared && editAction != 2)
                                    selection.CurrentHistoryInfo = null;
                            }
                        }
                        else
                        {
                            if (startCell == containerCell)
                                start.Paragraph.Delete(selection, start, end, editAction);
                            else
                                startCell.DeleteContainer(selection, start, end, editAction);
                        }
                    }
                    else
                    {
                        if (editAction == 2)
                            //Delete contents within table cell.
                            Delete(selection, 2,false);
                        else
                            //Delete other selected cells in current table.
                            containerCell.OwnerTable.DeleteCells(selection, containerCell, endCell, editAction);
                    }
                }
                else
                    //Selection end is different table.
                    containerCell.DeleteContainer(selection, start, end, editAction);
            }
            else
            {
                //Selection end is outside table.
                TableCellAdv cell = GetContainerCell();
                cell.DeleteContainer(selection, start, end, editAction);
            }
        }
        /// <summary>
        /// Deletes the container.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="editAction">The edit action.</param>
        private void DeleteContainer(SelectionAdv selection, TextPosition start, TextPosition end, byte editAction)
        {
            OwnerRow.Cells[0].UpdateEditPosition(selection);
            bool deleteNextBlock = !(end.Paragraph.IsInsideTable && OwnerTable.Contains(end.Paragraph.AssociatedCell));
            BlockAdv block = OwnerTable.GetNextRenderedBlock();
            SectionAdv section = Section;
            if (OwnerRow.RowIndex == 0 && (!end.Paragraph.IsInsideTable
                || !OwnerTable.Contains(end.Paragraph.AssociatedCell)
                || OwnerTable.Rows[OwnerTable.Rows.Count - 1].Contains(end.Paragraph.AssociatedCell)))
            {
                OwnerTable.Delete(selection, editAction);
            }
            else
            {
                DocIO.DLS.WTable table = null;
                TableAdv ownerTable = OwnerTable;
                if (deleteNextBlock || end.Paragraph.IsInsideTable && ownerTable.Rows[ownerTable.Rows.Count - 1].Contains(end.Paragraph.AssociatedCell))
                {
                    TableAdv newTable = ownerTable.SplitTable(OwnerRow);
                    ownerTable.Delete(selection, editAction);
                }
                else
                {
                    for (int i = OwnerRow.RowIndex; i < ownerTable.Rows.Count; i++)
                    {
                        TableRowAdv row = ownerTable.Rows[i];
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
                            ownerTable.Rows.Remove(row);
                            selection.CurrentHistoryInfo.RemovedNodes.Add(row);
                            i--;
                        }
                        if (end.Paragraph.IsInsideTable && row.Contains(end.Paragraph.AssociatedCell))
                            return;
                    }
                }
            }
            if (deleteNextBlock && block != null)
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
        /// <summary>
        /// Deletes the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="editAction">The edit action.</param>
        /// <returns></returns>
        internal bool Delete(SelectionAdv selection, byte editAction, bool copyChildsToClipboard)
        {
            //Checks whether this is last paragraph of owner textbody.
            BlockAdv block = Blocks[0];
            if (Blocks.Count == 1 && block is ParagraphAdv && (block as ParagraphAdv).IsEmpty())
            {
                if (editAction >= 3 && copyChildsToClipboard)
                    DocxExporting.SerializeParagraph(block as ParagraphAdv, selection.WordDocument.LastSection.Body);
                return false;
            }
            for (int i = 0; i < Blocks.Count; i++)
            {
                block = Blocks[i];
                if (editAction > 2 && copyChildsToClipboard)
                {
                    //Copy as DocIO instance.
                    if (block is ParagraphAdv)
                        DocxExporting.SerializeParagraph(block as ParagraphAdv, selection.WordDocument.LastSection.Body);
                    else
                        DocxExporting.SerializeTable(block as TableAdv, selection.WordDocument.LastSection.Body);
                }
                if (editAction < 4)
                {
                    //Checks whether this is last paragraph of owner textbody.
                    if (block is ParagraphAdv && Blocks.Count == 1)
                    {
                        //Preserves empty paragraph, to ensure minimal content.
                        ParagraphAdv paragraph = block as ParagraphAdv;
                        //Removes all the inlines in the paragraph.
                        for (int j = 0; i < paragraph.Inlines.Count; i++)
                        {
                            Inline inline = paragraph.Inlines[j];
                            paragraph.Inlines.Remove(inline);
                            j--;
                            if (selection.CurrentHistoryInfo.Action != Actions.ClearCells)
                                selection.CurrentHistoryInfo.RemovedNodes.Add(inline);
                        }
                        if (selection.CurrentHistoryInfo.Action != Actions.ClearCells)
                        {
                            selection.EditPosition = paragraph.GetHierarchicalIndex("0");
                            selection.CurrentHistoryInfo.InsertPosition = selection.EditPosition;
                        }
                        break;
                    }
                    block.RemoveBlock();
                    i--;
                    if (selection.CurrentHistoryInfo.Action != Actions.ClearCells)
                        selection.CurrentHistoryInfo.RemovedNodes.Add(block);
                }
            }
            return true;
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
            if (end.Paragraph.IsInsideTable)
            {
                TableCellAdv containerCell = GetContainerCell(end.Paragraph.AssociatedCell);
                if (containerCell.OwnerTable.Contains(end.Paragraph.AssociatedCell))
                {
                    TableCellAdv startCell = GetSelectedCell(containerCell);
                    TableCellAdv endCell = end.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    if (containerCell.Contains(end.Paragraph.AssociatedCell))
                    {
                        //Selection end is in container cell.
                        if (containerCell.IsCellSelected(start, end))
                        {
                            value = containerCell.GetCharacterFormatValue(property, value);
                            containerCell.ApplyCharacterFormat(selection, property, value);
                        }
                        else
                        {
                            if (startCell == containerCell)
                                start.Paragraph.ApplyCharacterFormat(selection, start, end, property, value);
                            else
                                startCell.OwnerRow.ApplyCharacterFormat(selection, start, end, property, value);
                        }
                    }
                    else
                        //Format other selected cells in current table.
                        containerCell.OwnerTable.ApplyCharacterFormat(selection, containerCell, endCell, property, value);
                }
                else
                    containerCell.OwnerRow.ApplyCharacterFormat(selection, start, end, property, value);
            }
            else
            {
                TableCellAdv cell = GetContainerCell();
                cell.OwnerRow.ApplyCharacterFormat(selection, start, end, property, value);
            }
        }
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyCharacterFormat(SelectionAdv selection, DependencyProperty property, object value)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].ApplyCharacterFormat(selection, property, value);
            }
        }
        /// <summary>
        /// Gets the character format value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal object GetCharacterFormatValue(DependencyProperty property, object value)
        {
            if (value is bool)
            {
                ParagraphAdv firstParagraph = GetFirstParagraph();
                CharacterFormat format = firstParagraph.CharacterFormat;
                if (firstParagraph.Inlines.Count > 0)
                    format = firstParagraph.Inlines[0].CharacterFormat;
                value = !(bool)format.GetPropertyValue(property);
            }
            return value;
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
            if (end.Paragraph.IsInsideTable)
            {
                TableCellAdv containerCell = GetContainerCell(end.Paragraph.AssociatedCell);
                if (containerCell.OwnerTable.Contains(end.Paragraph.AssociatedCell))
                {
                    TableCellAdv startCell = GetSelectedCell(containerCell);
                    TableCellAdv endCell = end.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    if (containerCell.Contains(end.Paragraph.AssociatedCell))
                    {
                        //Selection end is in container cell.
                        if (containerCell.IsCellSelected(start, end))
                        {
                            value = containerCell.GetParagraphFormatValue(property, value);
                            containerCell.ApplyParagraphFormat(selection, property, value);
                        }
                        else
                        {
                            if (startCell == containerCell)
                                start.Paragraph.ApplyParagraphFormat(selection, start, end, property, value);
                            else
                                startCell.OwnerRow.ApplyParagraphFormat(selection, start, end, property, value);
                        }
                    }
                    else
                        //Format other selected cells in current table.
                        containerCell.OwnerTable.ApplyParagraphFormat(selection, containerCell, endCell, property, value);
                }
                else
                    containerCell.OwnerRow.ApplyParagraphFormat(selection, start, end, property, value);
            }
            else
            {
                TableCellAdv cell = GetContainerCell();
                cell.OwnerRow.ApplyParagraphFormat(selection, start, end, property, value);
            }
        }
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyParagraphFormat(SelectionAdv selection, DependencyProperty property, object value)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].ApplyParagraphFormat(selection, property, value);
            }
        }
        /// <summary>
        /// Gets the paragraph format value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal object GetParagraphFormatValue(DependencyProperty property, object value)
        {
            if (value is bool)
            {
                ParagraphAdv firstParagraph = GetFirstParagraph();
                value = !(bool)firstParagraph.ParagraphFormat.GetPropertyValue(property);
            }
            return value;
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
            if (end.Paragraph.IsInsideTable)
            {
                TableCellAdv containerCell = GetContainerCell(end.Paragraph.AssociatedCell);
                if (containerCell.OwnerTable.Contains(end.Paragraph.AssociatedCell))
                {
                    TableCellAdv startCell = GetSelectedCell(containerCell);
                    TableCellAdv endCell = end.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    if (containerCell.Contains(end.Paragraph.AssociatedCell))
                    {
                        //Selection end is in container cell.
                        if (containerCell.IsCellSelected(start, end))
                        {
                            containerCell.GetCharacterFormat(selection);
                        }
                        else
                        {
                            if (startCell == containerCell)
                                start.Paragraph.GetCharacterFormat(selection, start, end);
                            else
                                startCell.OwnerRow.GetCharacterFormat(selection, start, end);
                        }
                    }
                    else
                        //Format other selected cells in current table.
                        containerCell.OwnerTable.GetCharacterFormat(selection, containerCell, endCell);
                }
                else
                    containerCell.OwnerRow.GetCharacterFormat(selection, start, end);
            }
            else
            {
                TableCellAdv cell = GetContainerCell();
                cell.OwnerRow.GetCharacterFormat(selection, start, end);
            }
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void GetCharacterFormat(SelectionAdv selection)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].GetCharacterFormat(selection);
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
        internal void GetParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            if (end.Paragraph.IsInsideTable)
            {
                TableCellAdv containerCell = GetContainerCell(end.Paragraph.AssociatedCell);
                if (containerCell.OwnerTable.Contains(end.Paragraph.AssociatedCell))
                {
                    TableCellAdv startCell = GetSelectedCell(containerCell);
                    TableCellAdv endCell = end.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    if (containerCell.Contains(end.Paragraph.AssociatedCell))
                    {
                        //Selection end is in container cell.
                        if (containerCell.IsCellSelected(start, end))
                        {
                            
                            containerCell.GetParagraphFormat(selection);
                        }
                        else
                        {
                            if (startCell == containerCell)
                                start.Paragraph.GetParagraphFormat(selection, start, end);
                            else
                                startCell.OwnerRow.GetParagraphFormat(selection, start, end);
                        }
                    }
                    else
                        //Format other selected cells in current table.
                        containerCell.OwnerTable.GetParagraphFormat(selection, containerCell, endCell);
                }
                else
                    containerCell.OwnerRow.GetParagraphFormat(selection, start, end);
            }
            else
            {
                TableCellAdv cell = GetContainerCell();
                cell.OwnerRow.GetParagraphFormat(selection, start, end);
            }
        }
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void GetParagraphFormat(SelectionAdv selection)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].GetParagraphFormat(selection);
            }
        }
        #endregion
    }
}

