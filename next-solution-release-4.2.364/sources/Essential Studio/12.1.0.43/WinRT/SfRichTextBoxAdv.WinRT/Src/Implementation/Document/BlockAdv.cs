#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
#if WPF
#else
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public abstract class BlockAdv : CompositeNode
    {
        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (Section != null)
                    return Section.Document;
                return null;
            }
        }
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
                if (Owner is TableCellAdv)
                    return (Owner as TableCellAdv).Section;
                if (Owner is HeaderFooter)
                    return (Owner as HeaderFooter).Section;
                return Owner as SectionAdv;
            }
        }
        /// <summary>
        /// Gets the base parent.
        /// </summary>
        /// <value>
        /// The base parent.
        /// </value>
        internal SfRichTextBoxAdv BaseParent
        {
            get
            {
                if (Section != null)
                    return Section.BaseParent;
                return  null;
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
                if (Owner is TableCellAdv)
                    return (Owner as TableCellAdv).IsInHeaderFooter;
                return Owner is HeaderFooter;
            }
        }
        /// <summary>
        /// Gets the associated cell.
        /// </summary>
        /// <value>
        /// The associated cell.
        /// </value>
        internal TableCellAdv AssociatedCell
        {
            get
            {
                return Owner as TableCellAdv;
            }
        }
        /// <summary>
        /// Gets or Sets the previous Block
        /// </summary>
        /// <value>
        /// The previous block.
        /// </value>
        internal BlockAdv PreviousBlock
        {
            get
            {
                return PreviousNode as BlockAdv;
            }
        }
        /// <summary>
        /// Gets or Sets the next block.
        /// </summary>
        /// <value>
        /// The next block.
        /// </value>
        internal BlockAdv NextBlock
        {
            get
            {
                return NextNode as BlockAdv;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is inside table.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is inside table; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInsideTable
        {
            get
            {
                return Owner is TableCellAdv;
            }
        }
        /// <summary>
        /// Gets the left indent.
        /// </summary>
        /// <value>
        /// The left indent.
        /// </value>
        internal double LeftIndent
        {
            get
            {
                if (this is ParagraphAdv)
                    return (this as ParagraphAdv).ParagraphFormat.LeftIndent;
                if (this is TableAdv)
                    return (this as TableAdv).TableFormat.LeftIndent;
                return 0;
            }
        }
        /// <summary>
        /// Gets the right indent.
        /// </summary>
        /// <value>
        /// The right indent.
        /// </value>
        internal double RightIndent
        {
            get
            {
                if (this is ParagraphAdv)
                    return (this as ParagraphAdv).ParagraphFormat.RightIndent;
                return 0;
            }
        }
        #endregion

        #region Constructor
        public BlockAdv()
            : base(null)
        {
        }
        internal BlockAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Abstract Methods
        internal abstract void ApplyCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value);
        internal abstract void ApplyCharacterFormat(SelectionAdv selection, DependencyProperty property, object value);
        internal abstract void ApplyParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value);
        internal abstract void ApplyParagraphFormat(SelectionAdv selection, DependencyProperty property, object value);
        internal abstract void GetCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end);
        internal abstract void GetParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end);
        internal abstract void GetCharacterFormat(SelectionAdv selection);
        internal abstract void GetParagraphFormat(SelectionAdv selection);
        internal abstract void ClearWidgets();
        internal abstract BlockAdv Clone();
        internal abstract void Delete(SelectionAdv selection, TextPosition start, TextPosition end, byte editAction);
        internal abstract void Dispose();
        internal abstract void Highlight(SelectionAdv selection, TextPosition start, TextPosition end);
        internal abstract bool UpdateListItems(BlockAdv block);
        internal abstract void UpdateRenderedListItems();
        internal abstract void LayoutItems(LayoutViewer viewer);
        internal abstract void ShiftWidgets(LayoutViewer viewer);
        #endregion

        #region Implementations
        /// <summary>
        /// Relayouts the or shift widgets.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void RelayoutOrShiftWidgets(LayoutViewer viewer)
        {
            bool relayoutItems = false;
            if (this is ParagraphAdv)
                relayoutItems = (this as ParagraphAdv).ParagraphWidgets.Count == 0;
            else
                relayoutItems = (this as TableAdv).TableWidgets.Count == 0;
            if (relayoutItems)
            {
                //Handle layouting the block.
                viewer.UpdateClientArea(this, true);
                LayoutItems(viewer);
                viewer.UpdateClientArea(this, false);
            }
            else
            {
                //Handled to check client area and shift layouted widget.
                ShiftWidgets(viewer);
                //Updates the list value of the rendered paragraph.
                UpdateRenderedListItems();
            }
        }
        /// <summary>
        /// Layouts the next items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void LayoutNextItems(LayoutViewer viewer)
        {
            BlockAdv block = this;
            if (block.NextNode == null || viewer.BlockToShift == block)
                viewer.BlockToShift = null;
            bool updateNextBlockList = true;
            while (block.NextNode is BlockAdv)
            {
                Widget currentWidget = null;
                if (block is ParagraphAdv)
                    currentWidget = (block as ParagraphAdv).ParagraphWidgets[(block as ParagraphAdv).ParagraphWidgets.Count - 1];
                else
                    currentWidget = (block as TableAdv).TableWidgets[(block as TableAdv).TableWidgets.Count - 1];
                if (viewer.FieldEndParagraph == block)
                    //Sets field end paragraph to null, inorder to hold relayouting with this paragraph.
                    viewer.FieldEndParagraph = null;
                block = block.NextNode as BlockAdv;
                if (viewer.BlockToShift == block)
                    viewer.BlockToShift = null;
                updateNextBlockList = false;
                Widget nextWidget = null;
                if (block is ParagraphAdv && (block as ParagraphAdv).ParagraphWidgets.Count > 0)
                    nextWidget = (block as ParagraphAdv).ParagraphWidgets[0];
                else if (block is TableAdv && (block as TableAdv).TableWidgets.Count > 0)
                    nextWidget = (block as TableAdv).TableWidgets[0];
                if (viewer.FieldEndParagraph == null && viewer.FieldStack.Count == 0 && nextWidget != null && currentWidget.ContainerWidget == nextWidget.ContainerWidget
                    && (Math.Round(nextWidget.Location.Y, 2) == Math.Round(viewer.ClientActiveArea.Y, 2)))
                {
                    if (viewer.BlockToShift != null)
                        viewer.BlockToShift = block;
                    break;
                }
                updateNextBlockList = true;
                if (viewer.OwnerControl.IsShiftingEnabled && viewer.FieldEndParagraph == null && viewer.FieldStack.Count == 0)
                {
                    viewer.BlockToShift = block;
                    break;
                }
                else
                {
                    block.ClearWidgets();
                    viewer.UpdateClientArea(block, true);
                    block.LayoutItems(viewer);
                    viewer.UpdateClientArea(block, false);
                }
            }
            if (!viewer.OwnerControl.IsShiftingEnabled || viewer.BlockToShift != block)
                block.UpdateListItemsTillEnd(updateNextBlockList);
            if (viewer is FlowLayoutViewer)
                viewer.UpdateScrollBars();
        }
        /// <summary>
        /// Updates the list items till document end.
        /// </summary>
        /// <param name="updateNextBlockList">if set to <c>true</c> [update next block list].</param>
        internal void UpdateListItemsTillEnd(bool updateNextBlockList)
        {
            BlockAdv block = updateNextBlockList ? GetNextRenderedBlock() : this;
            while (block != null)
            {
                //Updates the list value of the rendered paragraph.
                block.UpdateRenderedListItems();
                block = block.GetNextRenderedBlock();
            }
        }
        /// <summary>
        /// Gets the body widget of previous block.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal BodyWidget GetBodyWidgetOfPreviousBlock(ref int index)
        {
            index = 0;
            BodyWidget prevBodyWidget = null;
            if (PreviousBlock is ParagraphAdv)
            {
                prevBodyWidget = (PreviousBlock as ParagraphAdv).ParagraphWidgets[(PreviousBlock as ParagraphAdv).ParagraphWidgets.Count - 1].ContainerWidget as BodyWidget;
                index = prevBodyWidget.ChildWidgets.IndexOf((PreviousBlock as ParagraphAdv).ParagraphWidgets[(PreviousBlock as ParagraphAdv).ParagraphWidgets.Count - 1]);
            }
            else if (PreviousBlock is TableAdv)
            {
                prevBodyWidget = (PreviousBlock as TableAdv).TableWidgets[(PreviousBlock as TableAdv).TableWidgets.Count - 1].ContainerWidget as BodyWidget;
                index = prevBodyWidget.ChildWidgets.IndexOf((PreviousBlock as TableAdv).TableWidgets[(PreviousBlock as TableAdv).TableWidgets.Count - 1]);
            }
            return prevBodyWidget;
        }
        /// <summary>
        /// Updates the widgets to body.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="curWidget">The cur widget.</param>
        internal void UpdateWidgetsToBody(LayoutViewer viewer, Widget curWidget)
        {
            if (viewer.CurrentHeaderFooter == null)
            {
                if (curWidget.ContainerWidget != null
                    && curWidget.ContainerWidget.ChildWidgets.Contains(curWidget))
                {
                    curWidget.ContainerWidget.ChildWidgets.Remove(curWidget);
                    curWidget.ContainerWidget.Height -= curWidget.Height;
                }
                List<BodyWidget> bodyWidgets = Section.BodyWidgets;
                BodyWidget bodyWidget;
                List<Widget> widgets;
                if (this is ParagraphAdv)
                    widgets = (this as ParagraphAdv).ParagraphWidgets;
                else
                    widgets = (this as TableAdv).TableWidgets;
                if (widgets.Count > 1)
                {
                    BodyWidget prevBodyWidget = widgets[widgets.Count - 2].ContainerWidget as BodyWidget;
                    int prevIndex = bodyWidgets.IndexOf(prevBodyWidget);
                    if (prevIndex + 1 < bodyWidgets.Count)
                        bodyWidget = bodyWidgets[prevIndex + 1];
                    else
                        //Need to analyse further here.
                        bodyWidget = bodyWidgets[bodyWidgets.Count - 1];
                    bodyWidget.ChildWidgets.Insert(0, curWidget);
                    curWidget.ContainerWidget = bodyWidget;
                    bodyWidget.Height += curWidget.Height;
                }
                else
                {
                    BodyWidget prevBodyWidget = null;
                    int index = 0;
                    if (PreviousBlock is ParagraphAdv)
                    {
                        prevBodyWidget = (PreviousBlock as ParagraphAdv).ParagraphWidgets[(PreviousBlock as ParagraphAdv).ParagraphWidgets.Count - 1].ContainerWidget as BodyWidget;
                        index = prevBodyWidget.ChildWidgets.IndexOf((PreviousBlock as ParagraphAdv).ParagraphWidgets[(PreviousBlock as ParagraphAdv).ParagraphWidgets.Count - 1]);
                    }
                    else if (PreviousBlock is TableAdv)
                    {
                        prevBodyWidget = (PreviousBlock as TableAdv).TableWidgets[(PreviousBlock as TableAdv).TableWidgets.Count - 1].ContainerWidget as BodyWidget;
                        index = prevBodyWidget.ChildWidgets.IndexOf((PreviousBlock as TableAdv).TableWidgets[(PreviousBlock as TableAdv).TableWidgets.Count - 1]);
                    }
                    if (prevBodyWidget != null && Math.Round(curWidget.Location.Y, 2) ==
                        Math.Round((prevBodyWidget.ChildWidgets[index] as Widget).Location.Y + (prevBodyWidget.ChildWidgets[index] as Widget).Height, 2))
                    {
                        prevBodyWidget.ChildWidgets.Insert(index + 1, curWidget);
                        curWidget.ContainerWidget = prevBodyWidget;
                        prevBodyWidget.Height += curWidget.Height;
                    }
                    else
                    {
                        int prevIndex = 0;
                        //Specific for the document first paragraph.
                        TextPosition documentStart = Document.DocumentStart;
                        if ((documentStart == null || this == documentStart.Paragraph)
                            || (documentStart.Paragraph.IsInsideTable && this is TableAdv
                            && (this as TableAdv).Contains(documentStart.Paragraph.AssociatedCell)))
                            prevIndex = -1;
                        if (prevBodyWidget != null)
                            prevIndex = bodyWidgets.IndexOf(prevBodyWidget);
                        if (prevIndex + 1 < bodyWidgets.Count)
                            bodyWidget = bodyWidgets[prevIndex + 1];
                        else
                            bodyWidget = bodyWidgets[bodyWidgets.Count - 1];
                        bodyWidget.ChildWidgets.Insert(0, curWidget);
                        curWidget.ContainerWidget = bodyWidget;
                        bodyWidget.Height += curWidget.Height;
                    }
                }
            }
            else
            {
                //Need analyse further on adding widgets to the corresponding HFWidget.
                HeaderFooterWidget hfWidget = viewer.CurrentHeaderFooter.LayoutedWidgets[viewer.CurrentHeaderFooter.LayoutedWidgets.Count - 1];
                hfWidget.ChildWidgets.Add(curWidget);
                curWidget.ContainerWidget = hfWidget;
            }
        }
        /// <summary>
        /// Gets the width of the container.
        /// </summary>
        /// <returns></returns>
        internal double GetContainerWidth()
        {
            if (IsInsideTable)
                return AssociatedCell.CellFormat.CellWidth - (AssociatedCell.CellFormat.CellMargin.Left + AssociatedCell.CellFormat.CellMargin.Right);
            else
                return Section.SectionFormat.PageSize.Width - (Section.SectionFormat.PageMargin.Left + Section.SectionFormat.PageMargin.Right);
        }
        /// <summary>
        /// Layouts this instance.
        /// </summary>
        internal void Layout()
        {
            SectionAdv section = Owner as SectionAdv;
            if (Owner is SectionAdv)
            {
                int index = section.Blocks.IndexOf(this);
                if (section.BaseParent != null
                    && section.BaseParent.IsLayoutEnabled)
                    section.Layout(index);
            }
            else if (Owner is TableCellAdv)
            {
                TableCellAdv cell = Owner as TableCellAdv;
                cell = cell.GetContainerCell();
                section = cell.Section;
                if (section != null && section.BaseParent != null)
                {
                    DocumentAdv ownerDocument = section.Document;
                    LayoutViewer viewer = ownerDocument.OwnerControl.Viewer;
                    if (ownerDocument.OwnerControl.IsLayoutEnabled)
                        cell.OwnerRow.Layout(viewer);
                }
            }
        }
        /// <summary>
        /// Gets the next rendered block.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetNextRenderedBlock()
        {
            if (NextBlock == null)
            {
                if (Owner is SectionAdv
                    && Owner.NextNode != null)
                    return (Owner.NextNode as SectionAdv).Blocks[0];
            }
            return NextBlock;
        }
        /// <summary>
        /// Removes the block.
        /// </summary>
        internal void RemoveBlock()
        {
            if (IsInsideTable)
                AssociatedCell.Blocks.Remove(this);
            else if (Owner is HeaderFooter)
                (Owner as HeaderFooter).Blocks.Remove(this);
            else
                (Owner as SectionAdv).Blocks.Remove(this);
            ClearWidgets();
        }
        /// <summary>
        /// Gets the next selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetNextSelection(SelectionAdv selection)
        {
            if (NextBlock is ParagraphAdv)
                return NextBlock as ParagraphAdv;
            else if (NextBlock is TableAdv)
            {
                if (selection.IsEmpty || selection.IsForward)
                    return (NextBlock as TableAdv).GetLastParagraphInFirstRow();
                else
                    return (NextBlock as TableAdv).Rows[0].GetNextParagraph(selection);
            }

            if (Owner is TableCellAdv)
                return (Owner as TableCellAdv).GetNextSelection(selection);
            else if (Owner is SectionAdv)
                return (Owner as SectionAdv).GetNextSelection(selection);
            return null;
        }
        /// <summary>
        /// Gets the previous selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousSelection(SelectionAdv selection)
        {
            if (PreviousBlock is ParagraphAdv)
                return PreviousBlock as ParagraphAdv;
            else if (PreviousBlock is TableAdv)
            {
                if (!selection.IsForward)
                    return (PreviousBlock as TableAdv).GetFirstParagraphInLastRow();
                else
                    return (PreviousBlock as TableAdv).Rows[(PreviousBlock as TableAdv).Rows.Count - 1].GetPreviousParagraph(selection);
            }

            if (Owner is TableCellAdv)
                return (Owner as TableCellAdv).GetPreviousSelection(selection);
            else if (Owner is SectionAdv)
                return (Owner as SectionAdv).GetPreviousSelection(selection);
            return null;
        }
        /// <summary>
        /// Gets the next paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetNextParagraph()
        {
            if (NextBlock is ParagraphAdv)
                return NextBlock as ParagraphAdv;
            else if (NextBlock is TableAdv)
                return (NextBlock as TableAdv).GetFirstParagraphInFirstCell();

            if (Owner is TableCellAdv)
                return (Owner as TableCellAdv).GetNextParagraph();
            else if (Owner is SectionAdv)
                return (Owner as SectionAdv).GetNextParagraph();
            return null;
        }
        /// <summary>
        /// Gets the previous paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousParagraph()
        {
            if (PreviousBlock is ParagraphAdv)
                return PreviousBlock as ParagraphAdv;
            else if (PreviousBlock is TableAdv)
                return (PreviousBlock as TableAdv).GetLastParagraphInLastCell();

            if (Owner is TableCellAdv)
                return (Owner as TableCellAdv).GetPreviousParagraph();
            else if (Owner is SectionAdv)
                return (Owner as SectionAdv).GetPreviousParagraph();
            return null;
        }
        /// <summary>
        /// Gets the previous block.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetPreviousBlock()
        {
            if (PreviousBlock == null)
            {
                if (Owner is TableCellAdv)
                    return (Owner as TableCellAdv).OwnerTable.GetPreviousBlock();
                else if (Owner is SectionAdv)
                    return (Owner as SectionAdv).GetPreviousBlock();
            }
            return PreviousBlock;
        }
        /// <summary>
        /// Determines whether this block exists before the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns>
        ///   <c>true</c> if this block exists exist before the specified block; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsExistBefore(BlockAdv block)
        {
            if (IsInsideTable)
            {
                TableCellAdv cell1 = AssociatedCell;
                //Current paragraph in cell, paragraph in cell
                if (block.IsInsideTable)
                {
                    TableCellAdv cell2 = block.AssociatedCell;
                    if (cell1 == cell2)
                        return cell1.Blocks.IndexOf(this) < cell1.Blocks.IndexOf(block);
                    if (cell1.OwnerRow == cell2.OwnerRow)
                        return cell1.CellIndex < cell2.CellIndex;
                    if (cell1.OwnerTable == cell2.OwnerTable)
                        return cell1.OwnerRow.RowIndex < cell2.OwnerRow.RowIndex;
                    //Checks if current block exists before the block.
                    TableCellAdv containerCell = cell1.GetContainerCell(cell2);
                    if (containerCell.OwnerTable.Contains(cell2))
                    {
                        cell1 = cell1.GetSelectedCell(containerCell);
                        cell2 = cell2.GetSelectedCell(containerCell);
                        if (cell1 == containerCell)
                            return IsExistBefore(cell2.OwnerTable);
                        if (cell2 == containerCell)
                            return cell1.OwnerTable.IsExistBefore(block);
                        if (containerCell.OwnerRow == cell2.OwnerRow)
                            return containerCell.CellIndex < cell2.CellIndex;
                        if (containerCell.OwnerTable == cell2.OwnerTable)
                            return containerCell.OwnerRow.RowIndex < cell2.OwnerRow.RowIndex;
                        return cell1.OwnerTable.IsExistBefore(cell2.OwnerTable);
                    }
                    return containerCell.OwnerTable.IsExistBefore(cell2.OwnerTable.GetContainerTable());
                }
                //Current paragraph in cell, paragraph outside cell
                else
                {
                    TableAdv ownerTable = GetContainerTable();
                    return ownerTable.IsExistBefore(block);
                }
            }
            //Current paragraph outside cell, paragraph in cell
            else if (block.IsInsideTable)
            {
                TableAdv ownerTable = block.GetContainerTable();
                return IsExistBefore(ownerTable);
            }
            else
            {
                if (Owner is HeaderFooter)
                     return (Owner as HeaderFooter).Blocks.IndexOf(this) < (block.Owner as HeaderFooter).Blocks.IndexOf(block);
                else if (Section == block.Section)
                    return Section.Blocks.IndexOf(this) < Section.Blocks.IndexOf(block);
                else
                    return Document.Sections.IndexOf(Section) < Document.Sections.IndexOf(block.Section);
            }
        }
        /// <summary>
        /// Determines whether this block exists after the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns>
        ///   <c>true</c> if this block exists exist after the specified block; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsExistAfter(BlockAdv block)
        {
            if (IsInsideTable)
            {
                TableCellAdv cell1 = AssociatedCell;
                //Current paragraph in cell, paragraph in cell
                if (block.IsInsideTable)
                {
                    TableCellAdv cell2 = block.AssociatedCell;
                    if (cell1 == cell2)
                        return cell1.Blocks.IndexOf(this) > cell1.Blocks.IndexOf(block);
                    if (cell1.OwnerRow == cell2.OwnerRow)
                        return cell1.CellIndex > cell2.CellIndex;
                    if (cell1.OwnerTable == cell2.OwnerTable)
                        return cell1.OwnerRow.RowIndex > cell2.OwnerRow.RowIndex;
                    //Checks if this block exists before block.
                    TableCellAdv containerCell = cell1.GetContainerCell(cell2);
                    if (containerCell.OwnerTable.Contains(cell2))
                    {
                        cell1 = cell1.GetSelectedCell(containerCell);
                        cell2 = cell2.GetSelectedCell(containerCell);
                        if (cell1 == containerCell)
                            return IsExistAfter(cell2.OwnerTable);
                        if (cell2 == containerCell)
                            return cell1.OwnerTable.IsExistAfter(block);
                        if (containerCell.OwnerRow == cell2.OwnerRow)
                            return containerCell.CellIndex > cell2.CellIndex;
                        if (containerCell.OwnerTable == cell2.OwnerTable)
                            return containerCell.OwnerRow.RowIndex > cell2.OwnerRow.RowIndex;
                        return cell1.OwnerTable.IsExistAfter(cell2.OwnerTable);
                    }
                    return containerCell.OwnerTable.IsExistAfter(cell2.OwnerTable.GetContainerTable());
                }
                //Current paragraph in cell, paragraph outside cell
                else
                {
                    TableAdv ownerTable = GetContainerTable();
                    return ownerTable.IsExistAfter(block);
                }
            }
            //Current paragraph outside cell, paragraph in cell
            else if (block.IsInsideTable)
            {
                TableAdv ownerTable = block.GetContainerTable();
                return IsExistAfter(ownerTable);
            }
            else
            {
                if (Owner is HeaderFooter)
                    return (Owner as HeaderFooter).Blocks.IndexOf(this) > (block.Owner as HeaderFooter).Blocks.IndexOf(block);
                else if (Section == block.Section)
                    return Section.Blocks.IndexOf(this) > Section.Blocks.IndexOf(block);
                else
                    return Document.Sections.IndexOf(Section) > Document.Sections.IndexOf(block.Section);
            }
        }
        /// <summary>
        /// Gets the container table.
        /// </summary>
        /// <returns></returns>
        private TableAdv GetContainerTable()
        {
            TableAdv container = this as TableAdv;
            if (IsInsideTable)
            {
                if (AssociatedCell.OwnerTable.IsInsideTable)
                    container = AssociatedCell.OwnerTable.GetContainerTable();
                else
                    container = AssociatedCell.OwnerTable;
            }
            return container;
        }
        #endregion
    }
}
