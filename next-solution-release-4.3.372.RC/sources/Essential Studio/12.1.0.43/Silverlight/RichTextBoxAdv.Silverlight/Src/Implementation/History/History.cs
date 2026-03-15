#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    public class History
    {
        private Stack<HistoryInfo> undoStack;
        private Stack<HistoryInfo> redoStack;
        private RichTextBoxAdv ownerControl;
        internal DocumentAdv Document;
        private bool isRedo = false;
        private bool isUndo = false;

        private BlockCollection<ParagraphAdv> AffectedParagraphs;

        public History(RichTextBoxAdv richText)
        {
            undoStack = new Stack<HistoryInfo>(500);
            redoStack = new Stack<HistoryInfo>(500);
            ownerControl = richText;
            AffectedParagraphs = new BlockCollection<ParagraphAdv>();
        }

        public bool DisableHistory
        {
            get
            {
                return OwnerControl.DisableHistory;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal DocumentPositionHandler PositionHandler
        {
            get
            {
                return OwnerControl.PositionHandler;
            }
        }

        internal SelectionAdv Selection
        {
            get
            {
                return OwnerControl.Selection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TextPosition TextPosition
        {
            get
            {
                return PositionHandler.TextPosition;
            }
        }

        /// <summary>
        /// Gets or sets the owner control
        /// </summary>
        public RichTextBoxAdv OwnerControl
        {
            get
            {
                return ownerControl;
            }
            set
            {
                ownerControl = value;
            }
        }

        /// <summary>
        /// Gets the undo stack
        /// </summary>
        internal Stack<HistoryInfo> UndoStack
        {
            get
            {
                return undoStack;
            }
        }

        /// <summary>
        /// Gets the Redo stack
        /// </summary>
        internal Stack<HistoryInfo> RedoStack
        {
            get
            {
                return redoStack;
            }
        }

        /// <summary>
        /// Clears the history
        /// </summary>
        public void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
#if !WPF
            OwnerControl.UndoCommand.ExecuteChanged();
            OwnerControl.RedoCommand.ExecuteChanged();
#endif
        }

        public void ClearRedo()
        {
            redoStack.Clear();
#if !WPF
            OwnerControl.RedoCommand.ExecuteChanged();
#endif
        }

        /// <summary>
        /// Records the undo history
        /// </summary>
        /// <param name="historyInfo"></param>
        internal void RecordUndo(HistoryInfo historyInfo)
        {
            if (!DisableHistory)
            {
                undoStack.Push(historyInfo);
#if !WPF
                OwnerControl.UndoCommand.ExecuteChanged();
#endif
            }
        }

        /// <summary>
        /// Records the redo history
        /// </summary>
        /// <param name="historyInfo"></param>
        internal void RecordRedo(HistoryInfo historyInfo)
        {
            if (!DisableHistory)
            {
                redoStack.Push(historyInfo);
#if !WPF
                OwnerControl.RedoCommand.ExecuteChanged();
#endif
            }
        }

        private bool UndoInsert(HistoryInfo history)
        {
            if (PositionCursor(history.StartPosition))
            {
                if (history.Action == Actions.InsertInline)
                {
                    OwnerControl.Selection.Start = OwnerControl.PositionHandler.TextPosition;
                    OwnerControl.Selection.End = GetPosition(history.EndPosition);
                    Selection.Select();
                    Selection.RemoveSelection(true);
                }
                else
                    PositionHandler.Delete();

                OwnerControl.Viewer.IsSelected = false;
                if (history.UndoType == UndoType.SelectionBased)
                {
                    InsertBlocks(history.Blocks, history);
                    UpdateSelection(history);
                }
                return true;
            }
            return false;
        }

        private bool UndoDelete(HistoryInfo history)
        {
            if (PositionCursor(history.StartPosition))
            {
                if (history.StartPosition.IsPositionAtParagraphEnd)
                {
                    PositionHandler.Enter();
                    PositionCursor(history.StartPosition);
                }
                if (history.UndoType == UndoType.SelectionBased)
                {
                    InsertBlocks(history.Blocks, history);
                    UpdateSelection(history);
                }
                if (history.InlineStyle != null)
                {
                    if (history.InlineStyle.IsImageContainer || history.InlineStyle.IsUIContainer)
                    {
                        PositionHandler.InsertInline(history.InlineStyle);
                    }
                    else
                    {
                        if (!OwnerControl.CurrentInlineStyle.IsEqualInStyle(history.InlineStyle))
                        {
                            PositionHandler.IsStyleChanged = true;
                            OwnerControl.CurrentInlineStyle.SetInlineStyle(history.InlineStyle);
                        }
                        PositionHandler.InsertText(history.InlineStyle.InternalText);
                    }
                    PositionCursor(history.StartPosition);
                    return true;
                }
            }
            return false;
        }

        private bool UndoBackSpace(HistoryInfo history)
        {
            bool canUndo = PositionCursor(history.StartPosition) || history.UndoType == UndoType.SelectionBased;
            if (!canUndo && PositionCursor(history.TempPositionStart))
            {
                PositionHandler.Enter();
            }
            if (canUndo)
            {
                if (history.UndoType == UndoType.SelectionBased)
                {
                    InsertBlocks(history.Blocks, history);
                    UpdateSelection(history);
                }
                if (history.InlineStyle != null)
                {
                    if (history.InlineStyle.IsImageContainer || history.InlineStyle.IsUIContainer)
                    {
                        PositionHandler.InsertInline(history.InlineStyle);
                    }
                    else
                    {
                        if (!OwnerControl.CurrentInlineStyle.IsEqualInStyle(history.InlineStyle))
                        {
                            PositionHandler.IsStyleChanged = true;
                            OwnerControl.CurrentInlineStyle.SetInlineStyle(history.InlineStyle);
                        }
                        PositionHandler.InsertText(history.InlineStyle.InternalText);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool PositionCursor(TextPosition pos)
        {
            if (pos != null)
            {
                ParagraphAdv paragraph = Document.GetBlock(pos) as ParagraphAdv;
                if (paragraph != null)
                {
                    TextPosition.Index = pos.Index;
                    TextPosition.Paragraph = paragraph;
                    PositionHandler.PositionCursor();
                    return true;
                }
            }
            return false;
        }

        private bool PositionCursor(TextPosition pos, int minus)
        {
            if (pos != null)
            {
                ParagraphAdv paragraph = Document.GetBlock(pos) as ParagraphAdv;
                if (paragraph != null && pos.ParseIndex() > 0)
                {
                    TextPosition.Index = (pos.ParseIndex() - minus).ToString();
                    TextPosition.Paragraph = paragraph;
                    PositionHandler.PositionCursor();
                    return true;
                }
            }
            return false;
        }

        private bool PositionCursor(TextPosition pos, TableRowAdv deleted)
        {
            if (pos != null && deleted !=null)
            {
                List<string> extracted = pos.Index.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();

                string cellindex = string.Empty;
                string blockindex = string.Empty;

                foreach (string str in extracted)
                {
                    if (str.Contains('c'))
                    {
                        cellindex = str;
                    }
                    if (str.Contains('b'))
                    {
                        blockindex = str;
                    }
                }
                int cellind = int.Parse(cellindex.Substring(1));
                int blockind = int.Parse(blockindex.Substring(1));

                TableCellAdv tablecell = deleted.Cells[cellind];

                ParagraphAdv paragraph = tablecell.Blocks[blockind] as ParagraphAdv;
                if (paragraph != null)
                {
                    TextPosition.Index = pos.Index;
                    TextPosition.Paragraph = paragraph;
                    PositionHandler.PositionCursor();
                    return true;
                }
            }
            return false;
        }

        private bool PositionCursor(TextPosition pos, List<PreservedCellsInfo> preserved)
        {
            if (pos != null)
            {
                List<string> extracted = pos.Index.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();

                string cellindex = string.Empty;
                string blockindex = string.Empty;
                string rowindex = string.Empty;

                foreach (string str in extracted)
                {
                    if (str.Contains('c'))
                    {
                        cellindex = str;
                    }
                    if (str.Contains('b'))
                    {
                        blockindex = str;
                    }
                    if (str.Contains('r'))
                    {
                        rowindex = str;
                    }
                }
                int cellind = int.Parse(cellindex.Substring(1));
                int blockind = int.Parse(blockindex.Substring(1));
                int rowind = int.Parse(rowindex.Substring(1));

                foreach (PreservedCellsInfo cell in preserved)
                {
                    if (rowind == cell.RowIndex && cellind == cell.ColumnIndex)
                    {
                        ParagraphAdv paragraph = cell.Blocks[blockind] as ParagraphAdv;

                        if (paragraph != null)
                        {
                            TextPosition.Index = pos.Index;
                            TextPosition.Paragraph = paragraph;
                            PositionHandler.PositionCursor();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private TextPosition GetPosition(TextPosition pos)
        {
            if (pos != null)
            {
                ParagraphAdv block = Document.GetBlock(pos) as ParagraphAdv;
                if (block != null)
                {
                    TextPosition position = new TextPosition(pos.Document);
                    position.Index = pos.Index;
                    position.Paragraph = block;
                    position.VirtualPosition = pos.VirtualPosition;
                    return position;
                }
            }
            return null;
        }

        /// <summary>
        /// Undo the action
        /// </summary>
        public void Undo()
        {
            if (OwnerControl.IsReadOnly)
                return;
            isUndo = true;
            if (OwnerControl != null && OwnerControl.Viewer != null)
            {
                OwnerControl.Viewer.IsSelected = false;
            }
            if (undoStack.Count != 0)
            {
                HistoryInfo historyInfo = undoStack.Pop();
                Actions action = historyInfo.Action;
                OwnerControl.Viewer.IsSelected = false;
                if (action == Actions.Insert || action == Actions.Enter || action == Actions.InsertInline)
                {
                    UndoInsert(historyInfo);
                }
                else if (action == Actions.Delete)
                {
                    UndoDelete(historyInfo);
                }
                else if (action == Actions.BackSpace)
                {
                    UndoBackSpace(historyInfo);
                }
                else if (action == Actions.FontWeight || action == Actions.DoubleStrike || action == Actions.FontFamily || action == Actions.FontSize
                    || action == Actions.Foreground || action == Actions.HighlightColor || action == Actions.SingleStrike ||
                    action == Actions.Underline || action == Actions.FontStyle || action == Actions.SubScript || action == Actions.SuperScript)
                {
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        Select(historyInfo);
                        Selection.RemoveSelection(true);
                        PositionCursor(historyInfo.StartPosition);
                        InsertBlocks(historyInfo.Blocks, historyInfo);
                        UpdateSelection(historyInfo);
                    }
                }
                else if (action == Actions.AfterSpacing || action == Actions.BeforeSpacing || action == Actions.LeftIndent
                || action == Actions.LineSpacing || action == Actions.ListType || action == Actions.RightIndent ||
                action == Actions.TextAlignment)
                {
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                        List<BlockAdv> block = Selection.GetSelectedBlocks();
                        if (block != null && block.Count > 0)
                        {
                            SetParagraphStyle(block, historyInfo);
                            UpdateSelection(historyInfo);
                        }
                    }
                    else
                    {
                        ParagraphAdv paragraph = historyInfo.ParagraphStyle;
                        SetParagraphStyle(historyInfo, paragraph);
                    }
                }
                else if (action == Actions.Cut)
                {
                    if (PositionCursor(historyInfo.StartPosition))
                    {
                        InsertBlocks(historyInfo.Blocks, historyInfo);
                        UpdateSelection(historyInfo);
                    }
                }
                else if (action == Actions.Paste)
                {
                    OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(historyInfo.StartPosition, historyInfo.TempPositionEnd);
                    if (PositionCursor(historyInfo.StartPosition))
                    {
                        OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(historyInfo.TempPositionStart, historyInfo.TempPositionEnd);
                        UpdateSelectionUsingTempPos(historyInfo);
                        if (!historyInfo.TempPositionStart.IsPositionAtParagraphEnd)
                        {
                            Selection.RemoveSelectionMergeStartAndEnd(true);
                        }
                        else
                        {
                            Selection.RemoveSelection(true);
                        }

                        InsertBlocks(historyInfo.Blocks, historyInfo);
                        if (historyInfo.UndoType == UndoType.SelectionBased)
                        {
                            UpdateSelection(historyInfo);
                        }
                    }
                }
                else if (action == Actions.ImageReszing)
                {
                    ImageContainerAdv imageContainer = (ImageContainerAdv)GetInline(historyInfo);
                    imageContainer.Width = ((historyInfo.Blocks[0] as ParagraphAdv).Inlines[0] as ImageContainerAdv).Width;
                    imageContainer.Height = ((historyInfo.Blocks[0] as ParagraphAdv).Inlines[0] as ImageContainerAdv).Height;
                    imageContainer.Paragraph.ArrangeElements();
                    imageContainer.SelectElement();
                    PositionHandler.InvalidateVisibleRegion();
                }
                else if (action == Actions.DragDrop)
                {
                    UpdateSelectionUsingTempPos(historyInfo);
                    if (!historyInfo.TempPositionStart.IsPositionAtParagraphEnd)
                    {
                        Selection.RemoveSelectionMergeStartAndEnd(true);
                    }
                    else
                    {
                        Selection.RemoveSelection(true);
                    }
                    PositionCursor(historyInfo.StartPosition);
                    InsertBlocks(historyInfo.Blocks, historyInfo);
                    UpdateSelection(historyInfo);
                }
                else if (action == Actions.InsertTable)
                {
                    UndoInsertTable(historyInfo);
                }
                else if (action == Actions.InsertRow)
                {
                    if (historyInfo.IsInsertedRowHistory)
                    {
                        InsertedRowHistory rowhistory = historyInfo as InsertedRowHistory;
                        if (PositionCursor(rowhistory.StartPosition))
                        {
                            TableAdv table = TextPosition.Paragraph.AssociatedCell.OwnerTable;

                            for (int i = 0; i < rowhistory.RowsCount; i++)
                            {
                                TableRowAdv row = table.Rows[rowhistory.RowIndex];
                                if (table != null)
                                {
                                    PositionHandler.DeleteRow(row);
                                }
                            }

                            if (rowhistory.UndoType == UndoType.SelectionBased)
                            {
                                UpdateSelection(rowhistory);
                            }
                        }
                    }
                }
                else if (action == Actions.InsertColumn)
                {
                    UndoInsertColumn(historyInfo);
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }
                }
                else if (action == Actions.DeleteRow)
                {
                    UndoDeleteRow(historyInfo);

                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }
                    else
                    {
                        PositionCursor(historyInfo.StartPosition);
                    }
                }
                else if (action == Actions.DeleteColumn)
                {
                    UndoDeleteColumn(historyInfo);

                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }
                    else
                    {
                        PositionCursor(historyInfo.StartPosition);
                    }
                }
                else if (action == Actions.DeleteTable)
                {
                    UndoDeleteTable(historyInfo);

                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }
                    else
                    {
                        PositionCursor(historyInfo.StartPosition);
                    }
#if !WPF
                    OwnerControl.CanExecuteInsertDeleteTableCommands();
#endif
                }
                else if (action == Actions.Merging)
                {
                    UndoMerging(historyInfo);
                    UpdateSelection(historyInfo);
                }
                else if (action == Actions.TableCellBackground)
                {
                    UndoChangeTableCellBackground(historyInfo);
                }
                else if (action == Actions.TableBorderColor)
                {
                    UndoChangeTableBorderColor(historyInfo);
                }
                else if (action == Actions.TableBorderThickness)
                {
                    UndoChangeTableBorderThickness(historyInfo);
                }

                RecordRedo(historyInfo);
            }
            OwnerControl.Viewer.CheckForCursorVisibility(false);
#if !WPF
            OwnerControl.UndoCommand.ExecuteChanged();
#endif
            isUndo = false;
        }

        private void SetParagraphStyle(HistoryInfo history, ParagraphAdv para)
        {
            BlockAdv block = Document.GetBlock(history.StartPosition);
            if (block != null)
            {
                var style = para.GetType().GetProperty(history.Action.ToString()).GetValue(para, null);
                Actions action = history.Action;

                if (action == Actions.AfterSpacing || action == Actions.BeforeSpacing || action == Actions.LeftIndent
                || action == Actions.LineSpacing || action == Actions.ListType || action == Actions.RightIndent ||
                action == Actions.TextAlignment)
                {
                    block.GetType().GetProperty(history.Action.ToString()).SetValue(block, style, null);
                }

                block.ArrangeElements();
                PositionHandler.InvalidateVisibleRegion();
                OwnerControl.Viewer.SetIsArrangedToFalse();
            }
        }

        private void SetParagraphStyle(List<BlockAdv> blocks, HistoryInfo history)
        {
            int i = 0;
            BlockCollection<BlockAdv> tempblock = GetExtractedBlocks(history.Blocks);
            foreach (BlockAdv block in blocks)
            {
                if (i < tempblock.Count)
                {
                    if (tempblock[i].IsParagraph)
                    {
                        var style = tempblock[i].GetType().GetProperty(history.Action.ToString()).GetValue(tempblock[i], null);
                        block.GetType().GetProperty(history.Action.ToString()).SetValue(block, style, null);
                    }
                    i++;
                }
            }

            foreach (BlockAdv block in blocks)
            {
                block.CreateNewLines = true;
            }

            foreach (BlockAdv block in blocks)
            {
                if (!block.IsArranged)
                    block.ArrangeElements();
            }

            PositionHandler.InvalidateVisibleRegion();
            OwnerControl.Viewer.SetIsArrangedToFalse();
        }

        internal BlockCollection<BlockAdv> GetExtractedBlocks(BlockCollection<BlockAdv> list)
        {
            BlockCollection<BlockAdv> tempblocks = new BlockCollection<BlockAdv>();

            if (list.Count > 0)
            {
                foreach (BlockAdv b in list)
                {
                    if (b.IsParagraph)
                    {
                        tempblocks.Add(b.CopyBlock());
                    }
                    else if (b.IsTable)
                    {
                        foreach (BlockAdv b2 in (b as TableAdv).GetBlocksFromTable())
                        {
                            tempblocks.Add(b2.CopyBlock());
                        }
                    }
                }
            }
            return tempblocks;
        }

        private void Select(HistoryInfo history)
        {
            Selection.Start = GetPosition(history.StartPosition);
            Selection.End = GetPosition(history.EndPosition);
            Selection.Select();
        }

        private void UpdateSelection(HistoryInfo history)
        {
            Selection.Start = GetPosition(history.StartPosition);
            Selection.End = GetPosition(history.EndPosition);
            if (history.IsImageResizerSelected)
            {
                Inline imageinline = PositionHandler.GetInlineFromTextPosition(Selection.Start);
                if (imageinline != null && imageinline is ImageContainerAdv)
                {
                    OwnerControl.Viewer.SelectedImage = (ImageContainerAdv)imageinline;
                }
            }
            if (history.IsCellSelected)
            {
                TableCellAdv tablecell = null;
                if (Selection.CheckCanSelectCell(ref tablecell))
                {
                    PositionHandler.SelectCell(tablecell);
                }
            }
            else
            {
                Selection.UpdateSelection();
            }
        }

        private void UpdateSelectionUsingTempPos(HistoryInfo history)
        {
            Selection.Start = GetPosition(history.TempPositionStart);
            Selection.End = GetPosition(history.TempPositionEnd);
            if (history.IsImageResizerSelected)
            {
                Inline imageinline = PositionHandler.GetInlineFromTextPosition(Selection.Start);
                if (imageinline != null)
                {
                    if(imageinline is ImageContainerAdv)
                    {
                        OwnerControl.Viewer.SelectedImage = (ImageContainerAdv)imageinline;
                    }
                }
            }
            Selection.UpdateSelection();
        }

        private void InsertBlocks(BlockCollection<BlockAdv> blocks, HistoryInfo history)
        {
            if (blocks.Count > 0)
            {
                TableRowAdv startrow=null;
                TableCellAdv startCell=null;
                BlockCollection<BlockAdv> cellBlocks = new BlockCollection<BlockAdv>();
                TextPosition startPos = history.StartPosition.IsGreaterThan(history.EndPosition) ? history.EndPosition : history.StartPosition;
                TextPosition endPos = history.StartPosition.IsGreaterThan(history.EndPosition) ? history.StartPosition : history.EndPosition;
                
                if (IsWholeParagraphRemoved(history) && !history.NextIsTable && !history.StartPosition.IsPositionAtTableStart 
                    && !history.EndPosition.IsPositionAtTableEnd)
                {
                    SectionAdv section = Document.GetSection(startPos);
                    BlockAdv startBlk = Document.GetBlockFromVirtualPosition(startPos.VirtualPosition, ref startrow, ref startCell);
                    if (startBlk.IsInsideTable)
                    {
                        cellBlocks = startBlk.AssociatedCell.Blocks;
                    }
                    else
                        cellBlocks = section.Blocks;

                    if (section != null && startPos != null)
                    {
                        ParagraphAdv paragraph = (blocks[0] as ParagraphAdv).CreateNewParagraph();
                        cellBlocks.Insert((int)cellBlocks.IndexOf(startBlk), paragraph);
                        paragraph.LayoutViewer = OwnerControl.Viewer;
                        paragraph.Section = section;
                        OwnerControl.Viewer.SetPreviousBlocks();
                        PositionCursor(startPos);
                    }
                }
                if (startPos != null && endPos != null)
                {
                    if (IsBlockIsParagraph(startPos, endPos))
                    {
                        if (!startPos.IsPositionAtParagraphStart && !endPos.IsPositionAtParagraphEnd && !(history.Action == Actions.DragDrop && isRedo))
                        {
                            Selection.m_mergeparagraph = true;
                            Selection.PasteSelection(blocks);
                        }
                        else
                            Selection.PasteSelection(blocks);
                    }
                    else
                    {
                        if (startPos.IsInSameTable(endPos))
                        {
                            if (startPos.IsPositionAtTableStart && endPos.IsPositionAtTableEnd)
                            {
                                Selection.PasteSelection(blocks);
                            }
                            else
                            {
                                Selection.PasteSelectedCellsBlocks(blocks, history);
                            }
                        }
                        else
                        {
                            if (CanMergeTable(startPos, endPos))
                            {
                                Selection.m_mergetable = true;
                                Selection.PasteSelection(blocks);
                            }
                            else
                            {
                                Selection.PasteSelection(blocks);
                            }
                        }
                    }
                }
            }
        }

        private bool CanMergeTable(TextPosition startPos,TextPosition endPos)
        {
            if (!IsTable(startPos))
            {
                return !endPos.TempTableEndPosition;
            }
            else if (!IsTable(endPos))
            {
                return !startPos.IsPositionAtTableStart;
            }
            else if (IsTable(startPos) && IsTable(endPos))
            {
                return !startPos.IsPositionAtTableStart && !endPos.TempTableEndPosition;
            }
            return false;
        }
        

        private bool IsBlockIsParagraph(TextPosition startPos, TextPosition endPos)
        {
            return !IsTable(startPos) && !IsTable(endPos);
        }

        private bool IsTable(TextPosition Pos)
        {
            List<string> pos = Pos.VirtualPosition.Split(new string[] { "=>" }, StringSplitOptions.None).ToList<string>();

            int startlength = pos.Count - 1;

            return (pos[startlength].Contains('r') || pos[startlength].Contains('c') || pos[startlength].Contains('b'));
        }

        private bool IsWholeParagraphRemoved(HistoryInfo history)
        {
            if (history.StartPosition != null && history.EndPosition != null)
            {
                TextPosition startPos = history.StartPosition.IsGreaterThan(history.EndPosition) ? history.EndPosition : history.StartPosition;
                TextPosition endPos = history.StartPosition.IsGreaterThan(history.EndPosition) ? history.StartPosition : history.EndPosition;
                return startPos.IsPositionAtParagraphStart && endPos.IsPositionAtParagraphEnd;
            }
            return false;
        }

        private void ChangeStyle(HistoryInfo history)
        {
            string str = history.Action.ToString();
            if (history.Action == Actions.SingleStrike || history.Action == Actions.DoubleStrike)
                str = "StrikeThrough";
            if (history.Action == Actions.SubScript || history.Action == Actions.SuperScript)
                str = "Baseline";
            var style = history.InlineStyle.GetType().GetProperty(str).GetValue(history.InlineStyle, null);
            Actions action = history.Action;
            if (action == Actions.FontFamily || action == Actions.FontSize ||
                action == Actions.Foreground || action == Actions.HighlightColor)
            {
                string methodName = "Change" + action.ToString();
                Selection.GetType().GetMethod(methodName).Invoke(Selection, new object[] { style });
            }
            else if (action == Actions.FontWeight)
            {
                Selection.Bold();
            }
            else if (action == Actions.FontStyle)
            {
                Selection.Italic();
            }
            else if (action == Actions.DoubleStrike)
            {
                Selection.ChangeDoubleStrikeThrough();
            }
            else if (action == Actions.SingleStrike)
            {
                Selection.ChangeSingleStrikeThrough();
            }
            else if (action == Actions.SubScript)
            {
                Selection.ChangeSubscript();
            }
            else if (action == Actions.SuperScript)
            {
                Selection.ChangeSuperscript();
            }
            else if (action == Actions.Underline)
            {
                Selection.ChangeUnderline();
            }
        }

        private void ChangeParagraphStyle(HistoryInfo history)
        {
            if (history.Blocks.Count > 0)
            {
                ParagraphAdv para = history.UndoType == UndoType.SelectionBased ? history.ParagraphStyle : history.Blocks[0] as ParagraphAdv;
                var style = para.GetType().GetProperty(history.Action.ToString()).GetValue(para, null);
                Actions action = history.Action;

                if (action == Actions.AfterSpacing || action == Actions.BeforeSpacing || action == Actions.LeftIndent
                    || action == Actions.LineSpacing || action == Actions.ListType || action == Actions.RightIndent ||
                    action == Actions.TextAlignment)
                {
                    string methodName = "Change" + action.ToString();
                    Selection.GetType().GetMethod(methodName).Invoke(Selection, new object[] { style });
                }
            }
        }

        public bool UndoDeleteRow(HistoryInfo history)
        {
            if (history.IsDeletedRowHistory)
            {
                DeletedRowHistory rowhistory = history as DeletedRowHistory;

                TableAdv table = null;

                if (history.StartPosition != null && history.StartPosition.IsInsideTable)
                {
                    ParagraphAdv paragraph = (ParagraphAdv)Document.GetBlock(history.StartPosition);
                    if (paragraph != null)
                    {
                        TableCellAdv tablecell = paragraph.AssociatedCell;
                        table = tablecell.OwnerTable;
                    }
                }
                
                //if (rowhistory.DeletedRows.Count > 0)
                //{
                //    table = rowhistory.DeletedRows[0].Owner;
                //}

                UndoDeleteRow(rowhistory, table);

                OwnerControl.PositionHandler.InvalidateVisibleRegion();
                return true;
            }
            return false;
        }

        internal void UndoDeleteRow(DeletedRowHistory deletedrowhistory, TableAdv table)
        {
            List<TableCellAdv> rowspanaffectedcells = deletedrowhistory.RowSpanAffectedCells;

            if (deletedrowhistory.CanDeleteTable)
            {
                if (deletedrowhistory.DeletedTable != null)
                {
                    if (table.IsInsideTable)
                    {
                        table.AssociatedCell.Blocks.Insert(deletedrowhistory.DeletedTableIndex, deletedrowhistory.DeletedTable);
                    }
                    else
                    {
                        Document.Sections[0].Blocks.Insert(deletedrowhistory.DeletedTableIndex, deletedrowhistory.DeletedTable);
                    }

                    deletedrowhistory.DeletedTable.MeasureElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    deletedrowhistory.DeletedTable.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                }
            }
            else
            {
                if (table != null)
                {
                    if (rowspanaffectedcells.Count > 0)
                    {
                        foreach (TableCellAdv cell in rowspanaffectedcells)
                        {
                            cell.RowSpan++;
                        }
                    }

                    if (table != null)
                    {
                        for (int i = deletedrowhistory.DeletedRows.Count - 1; i >= 0; i--)
                        {
                            TableRowAdv row2 = deletedrowhistory.DeletedRows[i];
                            table.InsertRowAtIndex(row2, deletedrowhistory.RowIndex);
                        }
                    }
                }
            }
        }

        public bool UndoInsertColumn(HistoryInfo history)
        {
            if (history.IsInsertedColumnHistory)
            {
                InsertedColumnHistory columnhistory = history as InsertedColumnHistory;
                if (PositionCursor(columnhistory.StartPosition))
                {
                    if (!TextPosition.Paragraph.IsInsideTable)
                        return false;

                    TableAdv table = TextPosition.Paragraph.AssociatedCell.OwnerTable;
                    List<TableCellAdv> cellsToRemove = new List<TableCellAdv>();
                    bool canarrange = false;
                    if (table != null)
                    {
                        if (columnhistory.InsertedCellsInfo.Count > 0)
                        {
                            for (int i = 0; i < columnhistory.InsertedCellsInfo.Count; i++)
                            {
                                PreservedCellsInfo preservedcell = columnhistory.InsertedCellsInfo[i];

                                foreach (TableRowAdv row in table.Rows)
                                {
                                    foreach (TableCellAdv cell2 in row.Cells)
                                    {
                                        if (preservedcell.ColumnIndex == cell2.ColumnIndex && preservedcell.RowIndex == cell2.RowIndex)
                                        {
                                            cellsToRemove.Add(cell2);
                                        }
                                    }
                                }
                            }

                            foreach (TableCellAdv cell2 in cellsToRemove)
                            {
                                if (cell2 != null)
                                {
                                    cell2.OwnerRow.Cells.Remove(cell2);
                                }
                            }

                            table.MeasureElements();

                            OwnerControl.Viewer.SetIsArrangedToFalse();

                            OwnerControl.Viewer.SetPreviousBlocks();

                            OwnerControl.Viewer.SetIsArrangedToFalse();

                            table.ArrangeElements();
                            canarrange = true;
                        }
                        if (canarrange)
                        {
                            PositionHandler.InvalidateVisibleRegion();
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool UndoDeleteColumn(HistoryInfo history)
        {
            bool canarrange = false;
            if (history.IsDeletedColumnHistory)
            {
                DeletedColumnHistory columnhistory = history as DeletedColumnHistory;

                if (columnhistory.CanDeleteTable)
                {
                    if (columnhistory.DeletedTable != null)
                    {
                        if (columnhistory.DeletedTable.IsInsideTable)
                        {
                            columnhistory.DeletedTable.AssociatedCell.Blocks.Insert(columnhistory.DeletedTableIndex, columnhistory.DeletedTable);                            
                        }
                        else
                        {
                            Document.Sections[0].Blocks.Insert(columnhistory.DeletedTableIndex, columnhistory.DeletedTable);
                        }

                        columnhistory.DeletedTable.MeasureElements();

                        OwnerControl.Viewer.SetIsArrangedToFalse();

                        OwnerControl.Viewer.SetPreviousBlocks();

                        OwnerControl.Viewer.SetIsArrangedToFalse();

                        columnhistory.DeletedTable.ArrangeElements();

                        OwnerControl.Viewer.SetIsArrangedToFalse();

                        canarrange = true;
                    }
                }
                else
                {
                    TableAdv table = null;

                    if (TextPosition.Paragraph.IsInsideTable)
                    {
                        table = TextPosition.Paragraph.AssociatedCell.OwnerTable;
                    }
                    else
                        table = null;

                    if (table != null)
                    {
                        if (columnhistory.DeletedCellsInfo.Count > 0)
                        {
                            foreach (DeletedRowHistory row3 in columnhistory.DeletedRows)
                            {
                                UndoDeleteRow(row3, table);
                            }

                            List<TableRowAdv> list = new List<TableRowAdv>();

                            foreach (PreservedCellsInfo preservedcell in columnhistory.DeletedCellsInfo)
                            {
                                if (preservedcell.IsColumnSpanChanged)
                                {
                                    continue;
                                }

                                foreach (TableRowAdv row2 in table.Rows)
                                {
                                    if (table.Rows.IndexOf(row2) == preservedcell.RowIndex)
                                    {
                                        foreach (BlockAdv b in preservedcell.Blocks)
                                        {
                                            preservedcell.TableCell.Blocks.Add(b);
                                        }
                                        int index = preservedcell.ColumnIndex;

                                        if (index >= 0)
                                        {
                                            if (index >= row2.Cells.Count)
                                            {
                                                row2.Cells.Add(preservedcell.TableCell);
                                            }
                                            else
                                            {
                                                row2.Cells.Insert(index, preservedcell.TableCell);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            int i = 0;
                            List<PreservedCellsInfo> cellswithColumnspan = columnhistory.DeletedCellsInfo.Where(c => c.IsColumnSpanChanged == true).ToList();
                            foreach (TableRowAdv row4 in table.Rows)
                            {
                                foreach (TableCellAdv cell in row4.Cells)
                                {
                                    if (i < cellswithColumnspan.Count)
                                    {
                                        PreservedCellsInfo cellinfo = cellswithColumnspan[i];
                                        if (cellinfo.RowIndex == cell.RowIndex && cellinfo.ColumnIndex == cell.ColumnIndex && cellinfo.IsColumnSpanChanged)
                                        {
                                            cell.ColumnSpan++;
                                            i++;
                                        }
                                    }
                                }
                            }

                            table.MeasureElements();

                            OwnerControl.Viewer.SetIsArrangedToFalse();

                            table.ArrangeElements();

                            canarrange = true;

                        }

                    }
                }
                if (canarrange)
                {
                    OwnerControl.PositionHandler.InvalidateVisibleRegion();
                }

                return true;
            }
            return false;
        }

        public bool UndoMerging(HistoryInfo history)
        {
            if (history.IsMergedCellsHistory)
            {
                MergedCellsHistory mergedcells = history as MergedCellsHistory;

                if (PositionCursor(mergedcells.StartPosition))
                {
                    if (!TextPosition.Paragraph.IsInsideTable)
                        return false;

                    TableRowAdv tablerow = null;
                    TableCellAdv tablecell = null;
                    TableAdv table = Document.GetBlockFromVirtualPosition(mergedcells.StartPosition.VirtualPosition, ref tablerow, ref tablecell) as TableAdv;

                    if (table != null)
                    {
                        if (mergedcells.MergedCellsInfo.Count > 0)
                        {
                            foreach (DeletedRowHistory row2 in mergedcells.DeletedRowsHistory)
                            {
                                UndoDeleteRow(row2, table);
                            }

                            foreach (PreservedCellsInfo preservedcell in mergedcells.MergedCellsInfo)
                            {
                                foreach (TableRowAdv row in table.Rows)
                                {
                                    if (table.Rows.IndexOf(row) == preservedcell.RowIndex)
                                    {
                                        if (mergedcells.MergedCellsInfo.First() == preservedcell)
                                        {
                                            TableCellAdv cell3 = row.Cells.Where(c => c.ColumnIndex == preservedcell.ColumnIndex).ToList()[0];
                                            cell3.ColumnSpan = mergedcells.AffectedColumnSpan;
                                            cell3.RowSpan = mergedcells.AffectedRowSpan;
                                            cell3.Blocks.Clear();
                                            foreach (BlockAdv b in preservedcell.Blocks)
                                            {
                                                cell3.Blocks.Add(b);
                                            }
                                            continue;
                                        }
                                        int index = preservedcell.ColumnIndex;

                                        foreach (BlockAdv b in preservedcell.Blocks)
                                        {
                                            preservedcell.TableCell.Blocks.Add(b);
                                        }

                                        if (index >= 0 && index <= row.Cells.Count)
                                        {
                                            if (index == row.Cells.Count)
                                            {
                                                row.Cells.Add(preservedcell.TableCell);
                                            }
                                            else
                                            {
                                                row.Cells.Insert(index, preservedcell.TableCell);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            table.MeasureElements();
                            OwnerControl.Viewer.SetIsArrangedToFalse();
                            OwnerControl.Viewer.SetPreviousBlocks();
                            OwnerControl.Viewer.SetIsArrangedToFalse();
                            table.ArrangeElements();

                            OwnerControl.Viewer.SetIsArrangedToFalse();
                            OwnerControl.Viewer.SetVisibleLinesToPage();
                            OwnerControl.Viewer.SetIsArrangedToFalse();

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool UndoChangeTableCellBackground(HistoryInfo historyInfo)
        {
            if (historyInfo != null)
            {
                TableCellStyleHistoryInfo cellstyleinfo = historyInfo as TableCellStyleHistoryInfo;
                if (cellstyleinfo.UndoType == UndoType.SelectionBased)
                {
                    TableAdv table = cellstyleinfo.Table;

                    foreach (PreservedCellsInfo preserved in cellstyleinfo.PreservedCells)
                    {
                        foreach (TableRowAdv row in table.Rows)
                        {
                            foreach (TableCellAdv cell in row.Cells)
                            {
                                if (cell.RowIndex == preserved.RowIndex && cell.ColumnIndex == preserved.ColumnIndex)
                                {
                                    cell.CellElementBox.AssociatedPath.Fill = new SolidColorBrush(preserved.Background);
                                    cell.Background = preserved.Background;
                                }
                            }
                        }
                    }

                    UpdateSelection(historyInfo);
                    return true;
                }
                else
                {
                    TableAdv table = cellstyleinfo.Table;
                    PositionCursor(historyInfo.StartPosition);
                    TableCellAdv cell = TextPosition.Paragraph.AssociatedCell;
                    if (cell != null)
                    {
                        cell.CellElementBox.AssociatedPath.Fill = new SolidColorBrush(cellstyleinfo.TableCellBackground);
                        cell.Background = cellstyleinfo.TableCellBackground;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool UndoChangeTableBorderColor(HistoryInfo historyInfo)
        {
            if (historyInfo != null)
            {
                TableCellStyleHistoryInfo cellstyleinfo = historyInfo as TableCellStyleHistoryInfo;
                TableAdv table = cellstyleinfo.Table;
                table.BorderBrush = cellstyleinfo.BorderColor;
                foreach (TableRowAdv row in table.Rows)
                {
                    foreach (TableCellAdv cell in row.Cells)
                    {
                        Path path = cell.CellElementBox.AssociatedPath;
                        if (path != null)
                        {
                            path.Stroke = new SolidColorBrush(cellstyleinfo.BorderColor);
                        }
                    }
                }

                if (cellstyleinfo.UndoType == UndoType.SelectionBased)
                {
                    UpdateSelection(historyInfo);
                    return true;
                }
            }
            return false;
        }

        public bool UndoChangeTableBorderThickness(HistoryInfo historyInfo)
        {
            if (historyInfo != null)
            {
                TableCellStyleHistoryInfo cellstyleinfo = historyInfo as TableCellStyleHistoryInfo;
                TableAdv table = cellstyleinfo.Table;
                table.BorderThickness = cellstyleinfo.BorderThickness;
                foreach (TableRowAdv row in table.Rows)
                {
                    foreach (TableCellAdv cell in row.Cells)
                    {
                        Path path = cell.CellElementBox.AssociatedPath;
                        if (path != null)
                        {
                            path.StrokeThickness = cellstyleinfo.BorderThickness;
                        }
                    }
                }

                if (cellstyleinfo.UndoType == UndoType.SelectionBased)
                {
                    UpdateSelection(historyInfo);
                    return true;
                }
            }
            return false;
        }

        public bool UndoInsertTable(HistoryInfo historyinfo)
        {
            if (historyinfo.IsInsertedTableHistory)
            {
                InsertedTableHistory tablehistory = historyinfo as InsertedTableHistory;
                if (PositionCursor(tablehistory.StartPosition))
                {
                    int indextodelete = tablehistory.TableIndex;

                    if (TextPosition.IsPositionAtParagraphStart || tablehistory.StartPosition.IsPositionAtParagraphEnd)
                    {
                        if (TextPosition.IsPositionAtParagraphStart)
                            indextodelete--;

                        if (TextPosition.Paragraph.IsInsideTable)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                BlockAdv block = TextPosition.Paragraph.AssociatedCell.Blocks[indextodelete];
                                TextPosition.Paragraph.AssociatedCell.Blocks.Remove(block);
                                block.ClearLines();
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                BlockAdv blk = Document.Sections[0].Blocks[indextodelete];
                                Document.Sections[0].Blocks.Remove(blk);
                                blk.ClearLines();
                            }
                        }
                    }
                    else
                    {
                        if (TextPosition.Paragraph.IsInsideTable)
                        {
                            BlockAdv blk = TextPosition.Paragraph.AssociatedCell.Blocks[indextodelete];
                            TextPosition.Paragraph.AssociatedCell.Blocks.Remove(blk);
                            blk.ClearLines();
                        }
                        else
                        {
                            BlockAdv block = Document.Sections[0].Blocks[indextodelete];
                            Document.Sections[0].Blocks.Remove(block);
                            block.ClearLines();
                        }
                    }
                    OwnerControl.Viewer.SetIsArrangedToFalse();
                    OwnerControl.Viewer.SetPreviousBlocks();

                    ParagraphAdv paragraph = Document.GetBlock(TextPosition) as ParagraphAdv;

                    if (paragraph != null)
                    {
                        paragraph.ArrangeElements();
                    }

                    OwnerControl.Viewer.SetIsArrangedToFalse();
                    OwnerControl.Viewer.SetVisibleLinesToPage();

                    if (!TextPosition.IsPositionAtParagraphStart && !tablehistory.StartPosition.IsPositionAtParagraphEnd)
                        PositionHandler.Delete();

                    PositionCursor(TextPosition);
                    return true;
                }
            }
            return false;
        }

        public bool UndoDeleteTable(HistoryInfo historyInfo)
        {
            if (historyInfo.IsDeletedTableHistory)
            {
                DeletedTableHistory tablehistory = historyInfo as DeletedTableHistory;
                if (tablehistory.PreservedBlock != null)
                {
                    BlockAdv blk = tablehistory.PreservedBlock;
                    TableAdv table =null;

                    if (blk != null)
                    {
                        if (blk.IsInsideTable)
                        {
                            blk.AssociatedCell.Blocks.Insert(tablehistory.DeletedTableIndex, tablehistory.DeletedTable);
                            blk.PreviousBlock = table;
                        }
                        else
                        {
                            Document.Sections[0].Blocks.Insert(tablehistory.DeletedTableIndex, tablehistory.DeletedTable);
                            blk.PreviousBlock = table;
                        }
                    }

                    tablehistory.DeletedTable.MeasureElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    tablehistory.DeletedTable.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    OwnerControl.Viewer.SetVisibleLinesToPage();

                    return true;
                }
            }
            return false;
        }

        public bool CanUndo()
        {
            return undoStack.Count > 0;
        }

        public bool CanRedo()
        {
            return redoStack.Count > 0;
        }

        /// <summary>
        /// Redo the action
        /// </summary>
        public void Redo()
        {
            if (OwnerControl.IsReadOnly)
                return;
            isRedo = true;
            if (OwnerControl != null && OwnerControl.Viewer != null)
            {
                OwnerControl.Viewer.IsSelected = false;
            }
            if (redoStack.Count != 0)
            {
                OwnerControl.Viewer.IsSelected = false;
                HistoryInfo historyInfo = redoStack.Pop();
                Actions action = historyInfo.Action;
                if (action == Actions.Insert)
                {
                    RedoInsert(historyInfo);
                    //RecordUndo(historyInfo);
                }
                else if (action == Actions.Enter)
                {
                    RedoEnter(historyInfo);
                    //RecordUndo(historyInfo);
                }
                else if (action == Actions.FontWeight || action == Actions.DoubleStrike || action == Actions.FontFamily || action == Actions.FontSize
                    || action == Actions.Foreground || action == Actions.HighlightColor || action == Actions.SingleStrike ||
                    action == Actions.Underline || action == Actions.FontStyle || action == Actions.SubScript || action == Actions.SuperScript)
                {
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                        ChangeStyle(historyInfo);
                        //Selection.UpdateSelection();
                    }
                }
                else if (action == Actions.AfterSpacing || action == Actions.BeforeSpacing || action == Actions.LeftIndent
                || action == Actions.LineSpacing || action == Actions.ListType || action == Actions.RightIndent ||
                action == Actions.TextAlignment)
                {
                    PositionCursor(historyInfo.StartPosition);
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }
                    ChangeParagraphStyle(historyInfo);
                }
                else if (action == Actions.InsertInline)
                {
                    PositionCursor(historyInfo.StartPosition);
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }

                    OwnerControl.Viewer.InsertInline(historyInfo.InlineStyle);
                }
                else if (action == Actions.ImageReszing)
                {
                    ImageContainerAdv imageContainer = (ImageContainerAdv)GetInline(historyInfo);
                    imageContainer.Width = (historyInfo.InlineStyle as ImageContainerAdv).Width;
                    imageContainer.Height = (historyInfo.InlineStyle as ImageContainerAdv).Height;
                    imageContainer.Paragraph.ArrangeElements();
                    imageContainer.SelectElement();
                    PositionHandler.InvalidateVisibleRegion();
                    RecordUndo(historyInfo);
                }
                else if (action == Actions.DragDrop)
                {
                    UpdateSelection(historyInfo);
                    Selection.RemoveSelection(false);
                    PositionCursor(historyInfo.TempPositionStart);
                    InsertBlocks(historyInfo.Blocks, historyInfo);
                    RecordUndo(historyInfo);
                    UpdateSelectionUsingTempPos(historyInfo);
                }
                else if (action == Actions.BackSpace)
                {
                    PositionCursor(historyInfo.StartPosition);
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }

                    OwnerControl.Viewer.HandleBackKey();
                }
                else if (action == Actions.Delete)
                {
                    PositionCursor(historyInfo.StartPosition);
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                    }
                    OwnerControl.Viewer.HandleDeleteKey();
                }
                else if (action == Actions.Cut)
                {
                    if (historyInfo.UndoType == UndoType.SelectionBased)
                    {
                        UpdateSelection(historyInfo);
                        Selection.Cut();
                    }
                }
                else if (action == Actions.Paste)
                {
                    if (PositionCursor(historyInfo.StartPosition))
                    {
                        if (historyInfo.UndoType == UndoType.SelectionBased)
                        {
                            UpdateSelection(historyInfo);
                        }

                        ClipboardAdv.SetBlock(historyInfo.CopiedBlocks);

                        Selection.Paste();
                    }
                }
                else if (action == Actions.InsertTable)
                {
                    if (historyInfo.IsInsertedTableHistory)
                    {
                        InsertedTableHistory tablehistory = historyInfo as InsertedTableHistory;
                        if (PositionCursor(tablehistory.StartPosition))
                        {
                            OwnerControl.Viewer.InsertTable(tablehistory.RowsCount, tablehistory.ColumnsCount);
                        }
                    }
                }
                else if (action == Actions.InsertRow)
                {
                    if (historyInfo.IsInsertedRowHistory)
                    {
                        InsertedRowHistory rowhistory = historyInfo as InsertedRowHistory;
                        if (PositionCursor(rowhistory.StartPosition))
                        {
                            if (historyInfo.UndoType == UndoType.SelectionBased)
                            {
                                UpdateSelection(historyInfo);
                            }
                            OwnerControl.Viewer.InsertRow(rowhistory.RowPlacement);
                        }
                    }
                }
                else if (action == Actions.InsertColumn)
                {
                    if (historyInfo.IsInsertedColumnHistory)
                    {
                        InsertedColumnHistory columnhistory = historyInfo as InsertedColumnHistory;
                        if (PositionCursor(columnhistory.StartPosition))
                        {
                            if (historyInfo.UndoType == UndoType.SelectionBased)
                            {
                                UpdateSelection(historyInfo);
                            }
                            OwnerControl.Viewer.InsertColumn(columnhistory.ColumnPlace);
                        }
                    }
                }
                else if (action == Actions.DeleteRow)
                {
                    if (historyInfo.IsDeletedRowHistory)
                    {
                        DeletedRowHistory rowhistory = historyInfo as DeletedRowHistory;
                        if (PositionCursor(rowhistory.StartPosition))
                        {
                            if (rowhistory.UndoType == UndoType.SelectionBased)
                            {
                                UpdateSelection(rowhistory);
                            }
                            OwnerControl.Viewer.DeleteRow();
                        }
                    }
                }
                else if (action == Actions.DeleteColumn)
                {
                    if (historyInfo.IsDeletedColumnHistory)
                    {
                        DeletedColumnHistory columnhistory = historyInfo as DeletedColumnHistory;
                        if (PositionCursor(columnhistory.StartPosition))
                        {
                            if (columnhistory.UndoType == UndoType.SelectionBased)
                            {
                                UpdateSelection(columnhistory);
                            }
                            OwnerControl.Viewer.DeleteColumn();
                        }
                    }
                }
                else if (action == Actions.DeleteTable)
                {
                    if (historyInfo.IsDeletedTableHistory)
                    {
                        DeletedTableHistory tablehistory=historyInfo as DeletedTableHistory;
                        if (PositionCursor(tablehistory.StartPosition))
                        {
                            if (tablehistory.UndoType == UndoType.SelectionBased)
                            {
                                UpdateSelection(tablehistory);
                            }
                            OwnerControl.Viewer.DeleteTable();
                        }
                    }
                }
                else if (action == Actions.Merging)
                {
                    if (historyInfo.IsMergedCellsHistory)
                    {
                        MergedCellsHistory mergecellhistory = historyInfo as MergedCellsHistory;
                        UpdateSelection(mergecellhistory);
                        OwnerControl.Viewer.MergeSelectedCells();
                    }
                }
                else if (action == Actions.TableCellBackground)
                {
                    PositionCursor(historyInfo.StartPosition);
                    TableCellStyleHistoryInfo cellstyle = historyInfo as TableCellStyleHistoryInfo;
                    if (cellstyle != null)
                    {
                        OwnerControl.Viewer.ChangeTableCellBackground(cellstyle.TableCellBackground);
                    }
                }
                else if (action == Actions.TableBorderColor)
                {
                    PositionCursor(historyInfo.StartPosition);
                    TableCellStyleHistoryInfo cellstyle = historyInfo as TableCellStyleHistoryInfo;
                    if (cellstyle != null)
                    {
                        OwnerControl.Viewer.ChangeTableBorderColor(cellstyle.BorderColor);
                    }
                }
                else if (action == Actions.TableBorderThickness)
                {
                    PositionCursor(historyInfo.StartPosition);
                    TableCellStyleHistoryInfo cellstyle = historyInfo as TableCellStyleHistoryInfo;
                    if (cellstyle != null)
                    {
                        OwnerControl.Viewer.ChangeBorderThickness(cellstyle.BorderThickness);
                    }
                }
            }
#if !WPF
            OwnerControl.RedoCommand.ExecuteChanged();
#endif
            isRedo = false;
        }

        internal void CheckForClearingRedo()
        {
            if (!isUndo && !isRedo)
                ClearRedo();
        }

        private Inline GetInline(HistoryInfo history)
        {
            BlockAdv block = Document.GetBlock(history.StartPosition);
            if (block != null)
            {
                double spanIndex = 0;
                return block.GetInlineAndSpanIndexFromIndex(history.StartPosition.Index, out spanIndex);
            }

            return null;
        }

        private bool RedoInsert(HistoryInfo history)
        {
            if (PositionCursor(history.StartPosition))
            {
                if (history.UndoType == UndoType.SelectionBased)
                {
                    UpdateSelection(history);
                }
                PositionHandler.IsStyleChanged = history.IsStyleChanged;
                if (history.IsStyleChanged)
                    OwnerControl.CurrentInlineStyle.SetInlineStyle(history.InlineStyle);
                OwnerControl.Viewer.HandleTextInput(history.Text);
                return true;
            }

            return false;
        }

        private bool RedoEnter(HistoryInfo history)
        {
            if (PositionCursor(history.StartPosition))
            {
                if (history.UndoType == UndoType.SelectionBased)
                {
                    UpdateSelection(history);
                }
                OwnerControl.Viewer.HandleEnterKey();
                return true;
            }

            return false;
        }
    }
}
