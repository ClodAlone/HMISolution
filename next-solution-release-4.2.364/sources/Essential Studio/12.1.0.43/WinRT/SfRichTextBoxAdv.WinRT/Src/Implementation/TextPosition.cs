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
#if WPF
#else
using Windows.Foundation;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public class TextPosition
    {
        #region Fields
        internal SfRichTextBoxAdv OwnerControl;
        private ParagraphAdv currentParagraph = null;
        private double offset = 0;
        private Point location;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Document
        /// </summary>
        internal DocumentAdv Document
        {
            get
            {
                return OwnerControl.Document;
            }
        }
        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        /// <value>
        /// The paragraph.
        /// </value>
        public ParagraphAdv Paragraph
        {
            get
            {
                return currentParagraph;
            }
        }
        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        internal double Offset
        {
            get
            {
                return offset;
            }
        }
        /// <summary>
        /// Gets the physical location of the TextPosition in page.
        /// </summary>
        internal Point Location
        {
            get
            {
                return location;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is at paragraph start.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is at paragraph start; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAtParagraphStart
        {
            get
            {
                return offset == Paragraph.GetStartOffset();
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextPosition"/> class.
        /// </summary>
        public TextPosition()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextPosition"/> class.
        /// </summary>
        /// <param name="rte">The rte.</param>
        internal TextPosition(SfRichTextBoxAdv rte)
        {
            OwnerControl = rte;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Indicates whether the specified position is same as the current position.
        /// </summary>
        /// <param name="textPosition">A System.Windows.Documents.TextPointer that specifies a position to compare to the current position.</param>
        /// <returns>
        ///   <c>true</c> if textPosition indicates a position that is same as the current position; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAtSamePosition(TextPosition textPosition)
        {
            if (textPosition == null)
                throw new ArgumentNullException("textPosition is null.");
            return currentParagraph == textPosition.currentParagraph
                && offset == textPosition.offset;
        }
        /// <summary>
        /// Indicates whether the specified position is in the same text container as the current position.
        /// </summary>
        /// <param name="textPosition">A System.Windows.Documents.TextPointer that specifies a position to compare to the current position.</param>
        /// <returns>
        ///   <c>true</c>if textPosition indicates a position that is in the same text container as the current position; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInSameDocument(TextPosition textPosition)
        {
            if (textPosition == null)
                throw new ArgumentNullException("textPosition is null.");
            return Document == textPosition.Document;
        }
        /// <summary>
        /// Returns a value indicating whether both TextPosition are in same paragraph
        /// </summary>
        /// <param name="textPosition"></param>
        /// <returns></returns>
        internal bool IsInSameParagraph(TextPosition textPosition)
        {
            if (textPosition == null)
                throw new ArgumentNullException("textPosition is null.");
            return Paragraph == textPosition.Paragraph;
        }
        /// <summary>
        /// Determines whether this instance exist before the specified text position.
        /// </summary>
        /// <param name="textPosition">The text position.</param>
        /// <returns>
        ///   <c>true</c> if this instance exist before the specified text position; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">textPosition is null.</exception>
        /// <exception cref="System.ArgumentException">textPosition is not in the same document.</exception>
        internal bool IsExistBefore(TextPosition textPosition)
        {
            if (textPosition == null)
                throw new ArgumentNullException("textPosition is null.");
            if (!IsInSameDocument(textPosition))
                throw new ArgumentException("textPosition is not in the same document.");

            if (currentParagraph == textPosition.currentParagraph)
                return offset < textPosition.offset;
            if (currentParagraph.Owner == textPosition.currentParagraph.Owner)
            {
                if (currentParagraph.IsInsideTable)
                    return currentParagraph.AssociatedCell.Blocks.IndexOf(currentParagraph) < textPosition.currentParagraph.AssociatedCell.Blocks.IndexOf(textPosition.currentParagraph);
                else if (currentParagraph.Owner is HeaderFooter)
                    return (currentParagraph.Owner as HeaderFooter).Blocks.IndexOf(currentParagraph) < (textPosition.currentParagraph.Owner as HeaderFooter).Blocks.IndexOf(textPosition.currentParagraph);
                else
                    return currentParagraph.Section.Blocks.IndexOf(currentParagraph) < textPosition.currentParagraph.Section.Blocks.IndexOf(textPosition.currentParagraph);
            }

            return currentParagraph.IsExistBefore(textPosition.currentParagraph);
        }
        /// <summary>
        /// Determines whether this instance exist after the specified text position.
        /// </summary>
        /// <param name="textPosition">The text position.</param>
        /// <returns>
        ///   <c>true</c> if this instance exist after the specified text position; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">textPosition is null.</exception>
        /// <exception cref="System.ArgumentException">textPosition is not in the same document.</exception>
        internal bool IsExistAfter(TextPosition textPosition)
        {
            if (textPosition == null)
                throw new ArgumentNullException("textPosition is null.");
            if (!IsInSameDocument(textPosition))
                throw new ArgumentException("textPosition is not in the same document.");

            if (currentParagraph == textPosition.currentParagraph)
                return offset > textPosition.offset;
            if (currentParagraph.Owner == textPosition.currentParagraph.Owner)
            {
                if (currentParagraph.IsInsideTable)
                    return currentParagraph.AssociatedCell.Blocks.IndexOf(currentParagraph) > textPosition.currentParagraph.AssociatedCell.Blocks.IndexOf(textPosition.currentParagraph);
                else if (currentParagraph.Owner is HeaderFooter)
                    return (currentParagraph.Owner as HeaderFooter).Blocks.IndexOf(currentParagraph) > (textPosition.currentParagraph.Owner as HeaderFooter).Blocks.IndexOf(textPosition.currentParagraph);
                else
                    return currentParagraph.Section.Blocks.IndexOf(currentParagraph) > textPosition.currentParagraph.Section.Blocks.IndexOf(textPosition.currentParagraph);
            }

            return currentParagraph.IsExistAfter(textPosition.currentParagraph);
        }
        /// <summary>
        /// Returns the copy of the current position
        /// </summary>
        /// <returns></returns>
        internal TextPosition Clone()
        {
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.currentParagraph = currentParagraph;
            textPosition.offset = offset;
            textPosition.location = location;
            return textPosition;
        }
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="offsetInParagraph">The offset in paragraph.</param>
        internal void SetPosition(ParagraphAdv paragraph, double offsetInParagraph)
        {
            currentParagraph = paragraph;
            offset = offsetInParagraph;
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="inline">The inline.</param>
        /// <param name="index">The index.</param>
        /// <param name="physicalLocation">The physical location.</param>
        internal void SetPosition(ParagraphAdv paragraph, Inline inline, int index, Point physicalLocation)
        {
            bool isParagraphEnd = false;
            if (inline == null)
                currentParagraph = paragraph;
            else
            {
                currentParagraph = inline.OwnerParagraph;
                if (inline.NextNode is FieldCharacterAdv && index > inline.Length)
                    isParagraphEnd = inline.IsLastRenderedInline(inline.Length);
            }
            location = physicalLocation;
            if (isParagraphEnd)
                offset = currentParagraph.GetLength() + 1;
            else
                offset = currentParagraph.GetOffset(inline, index);
        }
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="textPosition">The text position.</param>
        internal void SetPosition(TextPosition textPosition)
        {
            currentParagraph = textPosition.currentParagraph;
            offset = textPosition.offset;
            location = textPosition.location;
        }
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="positionAtStart">if set to <c>true</c> [position at start].</param>
        internal void SetPosition(ParagraphAdv paragraph, bool positionAtStart)
        {
            currentParagraph = paragraph;
            offset = positionAtStart ? paragraph.GetStartOffset() : paragraph.GetLength() + 1;
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Sets the text position.
        /// </summary>
        /// <param name="hierarchicalIndex">The hierarchicalIndex.</param>
        internal void SetPosition(string hierarchicalIndex)
        {
            ParagraphAdv paragraph = OwnerControl.Document.GetParagraph(ref hierarchicalIndex);
            double offset = double.Parse(hierarchicalIndex);
            SetPosition(paragraph, offset);
        }
        /// <summary>
        /// Gets the hierarchical index of current text position.
        /// </summary>
        /// <returns></returns>
        internal string GetHierarchicalIndex()
        {
            return Paragraph.GetHierarchicalIndex(Offset.ToString());
        }
        /// <summary>
        /// Moves the next position.
        /// </summary>
        internal void MoveNextPosition()
        {
            Inline inline = currentParagraph.GetNextStartInline(offset);
            if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
            {
                MoveNextPosition(inline as FieldBeginAdv);
                MoveNextPosition();
                return;
            }
            double nextOffset = currentParagraph.GetNextValidOffset(offset);
            int indexInInline = 0;
            if (nextOffset > offset)
            {
                offset = nextOffset;
                inline = currentParagraph.GetInline(offset, ref indexInInline);
                if (inline != null && indexInInline == inline.Length && inline.NextNode is FieldCharacterAdv)
                {
                    Inline nextValidInline = (inline.NextNode as Inline).GetNextValidInline();
                    //Moves to field end mark.
                    if (nextValidInline is FieldEndAdv)
                    {
                        inline = nextValidInline as Inline;
                        offset = currentParagraph.GetOffset(inline, 1);
                    }
                }
            }
            else
            {
                //Moves to owner and get next paragraph.
                ParagraphAdv nextParagraph = currentParagraph.GetNextParagraph();
                if (nextParagraph != null)
                {
                    currentParagraph = nextParagraph;
                    offset = currentParagraph.GetStartOffset();
                }
                inline = currentParagraph.GetInline(offset, ref indexInInline);
                if (inline is FieldEndAdv)
                    offset++;
            }
            //Gets physical position in current page.
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Moves the next position.
        /// </summary>
        /// <param name="fieldBegin">The field begin.</param>
        private void MoveNextPosition(FieldBeginAdv fieldBegin)
        {
            Inline inline;
            if (fieldBegin.FieldSeparator == null)
                inline = fieldBegin.FieldEnd;
            else
            {
                inline = fieldBegin.FieldSeparator;
                currentParagraph = inline.OwnerParagraph;
                if (currentParagraph == fieldBegin.FieldEnd.OwnerParagraph && !currentParagraph.HasValidInline(inline, fieldBegin.FieldEnd))
                    inline = fieldBegin.FieldEnd;
            }
            currentParagraph = inline.OwnerParagraph;
            offset = currentParagraph.GetOffset(inline, 1);
        }
        /// <summary>
        /// Moves to the previous position.
        /// </summary>
        internal void MovePreviousPosition()
        {
            int indexInInline = 0;
            Inline inline = currentParagraph.GetInline(offset, ref indexInInline);
            if (inline is FieldEndAdv && (inline as FieldEndAdv).FieldBegin != null)
                MovePreviousPosition(inline as FieldEndAdv);
            double prevOffset = currentParagraph.GetPreviousValidOffset(offset);
            if (offset > prevOffset)
                offset = prevOffset;
            else
            {
                //Moves to owner and get previous paragraph.
                ParagraphAdv previousParagraph = currentParagraph.GetPreviousSelection(OwnerControl.Selection);
                if (previousParagraph != null)
                {
                    currentParagraph = previousParagraph;
                    offset = previousParagraph.GetEndOffset();
                }
            }
            indexInInline = 0;
            inline = currentParagraph.GetInline(offset, ref indexInInline);
            if (inline is FieldCharacterAdv)
            {
                //Checks if field character is part of rendered field, otherwise moves to previous rendered content.
                Inline prevInline = inline.GetPreviousValidInline();
                if (prevInline != null)
                {
                    inline = prevInline;
                    currentParagraph = inline.OwnerParagraph;
                    offset = currentParagraph.GetOffset(inline, inline.Length);
                    if (inline is FieldBeginAdv)
                        offset--;
                }
            }
            //Gets physical position in current page.
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Moves the previous position.
        /// </summary>
        /// <param name="fieldEnd">The field end.</param>
        private void MovePreviousPosition(FieldEndAdv fieldEnd)
        {
            Inline inline;
            if (fieldEnd.FieldSeparator == null)
                inline = fieldEnd.FieldBegin.GetPreviousValidInline();
            else
                inline = fieldEnd.GetPreviousValidInline();
            currentParagraph = inline.OwnerParagraph;
            offset = currentParagraph.GetOffset(inline, inline is FieldCharacterAdv ? 0 : inline.Length);
        }
        /// <summary>
        /// Moves down.
        /// </summary>
        /// <param name="left">The left.</param>
        internal void MoveDown(double left)
        {
            //Moves text position to end of line.
            ParagraphAdv prevParagraph = currentParagraph;
            double prevOffset = offset;
            MoveToLineEnd();
            if (prevParagraph == currentParagraph && prevOffset == offset)
            {
                MoveNextPosition();
                MoveToLineEnd();
            }
            double length = currentParagraph.GetLength();
            if (offset > length)
                offset = length;
            //Moves next line starting.
            MoveNextPosition();
            LineWidget nextLine = currentParagraph.GetLineWidget(offset);
            //Moves till the Up/Down selection width.
            double top = nextLine.GetTop();
            nextLine.UpdateTextPosition(OwnerControl, new Point(left, top), this, false);
        }
        /// <summary>
        /// Moves up.
        /// </summary>
        /// <param name="left">The left.</param>
        internal void MoveUp(double left)
        {
            //Moves text position to start of line.
            MoveToLineStart();
            //Moves previous line starting.
            MovePreviousPosition();
            LineWidget previousLine = currentParagraph.GetLineWidget(offset);
            //Moves till the Up/Down selection width.
            double top = previousLine.GetTop();
            previousLine.UpdateTextPosition(OwnerControl, new Point(left, top), this, false);
        }
        /// <summary>
        /// Moves the forward.
        /// </summary>
        internal void MoveForward()
        {
            int indexInInline = 0;
            Inline inline = currentParagraph.GetInline(offset, ref indexInInline);
            if (inline != null)
            {
                if (!OwnerControl.Selection.IsEmpty && indexInInline == inline.Length && inline is FieldEndAdv)
                {
                    string hierarchicalIndex = OwnerControl.Selection.Start.GetHierarchicalIndex();
                    double fieldBeginOffset = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetOffset((inline as FieldEndAdv).FieldBegin, 0);
                    string fieldBeginIndex = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetHierarchicalIndex(fieldBeginOffset.ToString());
                    if (!TextPosition.IsForwardSelection(hierarchicalIndex, fieldBeginIndex))
                    {
                        //If field begin is before selection start, move selection start to field begin.
                        OwnerControl.Selection.Start.SetPosition((inline as FieldEndAdv).FieldBegin.OwnerParagraph, fieldBeginOffset);
                        return;
                    }
                }
                inline = inline.GetNextRenderedInline(indexInInline);
            }
            if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
            {
                ParagraphAdv selectionStartParagraph = OwnerControl.Selection.Start.Paragraph;
                int selectionStartIndex = 0;
                Inline selectionStartInline = selectionStartParagraph.GetInline(OwnerControl.Selection.Start.Offset, ref selectionStartIndex);
                Inline nextRenderInline = selectionStartInline.GetNextRenderedInline(selectionStartIndex);
                if (nextRenderInline == inline)
                    MoveNextPosition(inline as FieldBeginAdv);
                else
                {
                    //If selection start is before field begin, extend selection end to field end.
                    inline = (inline as FieldBeginAdv).FieldEnd;
                    currentParagraph = inline.OwnerParagraph;
                    offset = currentParagraph.GetOffset(inline, 1);
                    //Updates physical position in current page.
                    UpdatePhysicalPosition();
                    return;
                }
            }
            else if (inline is FieldBeginAdv || inline is FieldEndAdv)
            {
                currentParagraph = inline.OwnerParagraph;
                offset = currentParagraph.GetOffset(inline, 1);
            }
            indexInInline = 0;
            double nextOffset = currentParagraph.GetNextValidOffset(offset);
            double length = currentParagraph.GetLength();
            if (offset <= nextOffset && offset < length + 1)
            {
                if (offset == nextOffset)
                    offset = length + 1;
                else
                {
                    offset = nextOffset;
                    inline = currentParagraph.GetInline(offset, ref indexInInline);
                    if (inline != null && indexInInline == inline.Length && inline.NextNode is FieldCharacterAdv)
                    {
                        Inline nextValidInline = (inline.NextNode as Inline).GetNextValidInline();
                        //Moves to field end mark.
                        if (nextValidInline is FieldEndAdv)
                        {
                            inline = nextValidInline as Inline;
                            offset = currentParagraph.GetOffset(inline, 1);
                        }
                    }
                }
            }
            else
            {
                //Moves to owner and get next paragraph.
                ParagraphAdv lastParagraph = currentParagraph.GetNextSelection(OwnerControl.Selection);
                if (lastParagraph != null)
                {
                    bool positionAtEnd = false;
                    if (lastParagraph.Owner is TableCellAdv)
                    {
                        if (OwnerControl.Selection.Start.Paragraph.IsInsideTable)
                        {
                            TableCellAdv containerCell = OwnerControl.Selection.Start.Paragraph.AssociatedCell.GetContainerCell(lastParagraph.AssociatedCell);
                            positionAtEnd = !containerCell.Contains(lastParagraph.AssociatedCell);
                        }
                        else
                            positionAtEnd = true;
                    }
                    currentParagraph = lastParagraph;
                    offset = positionAtEnd ? currentParagraph.GetLength() + 1 : 1;
                }
                inline = currentParagraph.GetInline(offset, ref indexInInline);
                if (inline is FieldEndAdv)
                    offset++;
            }
            //Gets physical position in current page.
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Moves the backward.
        /// </summary>
        internal void MoveBackward()
        {
            int indexInInline = 0;
            Inline inline = currentParagraph.GetInline(offset, ref indexInInline);
            if (!OwnerControl.Selection.IsEmpty && inline != null)
            {
                Inline nextInline = inline.GetNextRenderedInline(indexInInline);
                if (nextInline is FieldBeginAdv)
                {
                    string hierarchicalIndex = OwnerControl.Selection.Start.GetHierarchicalIndex();
                    double fieldEndOffset = (nextInline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetOffset((nextInline as FieldBeginAdv).FieldEnd, 1);
                    string fieldEndIndex = (nextInline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetHierarchicalIndex(fieldEndOffset.ToString());
                    if (!TextPosition.IsForwardSelection(fieldEndIndex, hierarchicalIndex))
                    {
                        //If field end is after selection start, move selection start to field end.
                        OwnerControl.Selection.Start.SetPosition((nextInline as FieldBeginAdv).FieldEnd.OwnerParagraph, fieldEndOffset);
                        return;
                    }
                }
            }
            if (inline is FieldEndAdv && (inline as FieldEndAdv).FieldBegin != null)
            {
                string hierarchicalIndex = OwnerControl.Selection.Start.GetHierarchicalIndex();
                double fieldEndOffset = inline.OwnerParagraph.GetOffset(inline, 1);
                string fieldEndIndex = inline.OwnerParagraph.GetHierarchicalIndex(fieldEndOffset.ToString());
                if (!TextPosition.IsForwardSelection(hierarchicalIndex, fieldEndIndex))
                {
                    //If field end is after selection start, extend selection end to field begin.
                    double fieldBeginOffset = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetOffset((inline as FieldEndAdv).FieldBegin, 0);
                    currentParagraph = (inline as FieldEndAdv).FieldBegin.OwnerParagraph;
                    offset = fieldBeginOffset;
                    //Updates physical position in current page.
                    UpdatePhysicalPosition();
                    return;
                }
                MovePreviousPosition(inline as FieldEndAdv);
            }
            double prevOffset = currentParagraph.GetPreviousValidOffset(offset);
            if (offset > prevOffset)
                offset = prevOffset;
            else
            {
                //Moves to owner and get previous paragraph.
                ParagraphAdv lastParagraph = currentParagraph.GetPreviousSelection(OwnerControl.Selection);
                if (lastParagraph != null)
                {
                    bool positionAtStart = false;
                    if (lastParagraph.Owner is TableCellAdv)
                    {
                        if (OwnerControl.Selection.Start.Paragraph.IsInsideTable)
                        {
                            TableCellAdv containerCell = OwnerControl.Selection.Start.Paragraph.AssociatedCell.GetContainerCell(lastParagraph.AssociatedCell);
                            positionAtStart = !containerCell.Contains(lastParagraph.AssociatedCell);
                        }
                        else
                            positionAtStart = true;
                    }
                    currentParagraph = lastParagraph;
                    offset = positionAtStart ? currentParagraph.GetStartOffset() : currentParagraph.GetEndOffset();
                }
            }
            //Updates the offset to previous valid inline.
            indexInInline = 0;
            inline = currentParagraph.GetInline(offset, ref indexInInline);
            if (inline is FieldCharacterAdv)
            {
                //Checks if field character is part of rendered field, otherwise moves to previous rendered content.
                Inline prevInline = inline.GetPreviousValidInline();
                if (prevInline != null)
                {
                    inline = prevInline;
                    currentParagraph = inline.OwnerParagraph;
                    offset = currentParagraph.GetOffset(inline, inline is FieldBeginAdv ? 0 : inline.Length);
                }
            }
            //Gets physical position in current page.
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Updates the physical position.
        /// </summary>
        internal void UpdatePhysicalPosition()
        {
            if (currentParagraph.ParagraphWidgets.Count > 0 && OwnerControl.IsLayoutEnabled)
                location = currentParagraph.GetPhysicalPosition(offset);
        }
        /// <summary>
        /// Moves to previous line.
        /// </summary>
        /// <param name="left">The left.</param>
        internal void MoveToPreviousLine(double left)
        {
            string currentIndex = GetHierarchicalIndex();
            LineWidget currentLine = currentParagraph.GetLineWidget(offset);
            //Moves text position to start of line.
            MoveToLineStart();
            if (currentParagraph.IsInsideTable)
                MoveUpInTable();
            else
                MoveBackward();
            LineWidget prevLine = currentParagraph.GetLineWidget(offset);
            double lineStart = prevLine.GetLeft();
            double lineWidth = prevLine.GetWidth(true);
            //Moves till the Up/Down selection width.
            if (lineWidth + lineStart >= left && currentLine != prevLine)
            {
                double top = prevLine.GetTop();
                prevLine.UpdateTextPosition(OwnerControl, new Point(left, top), this, true);
            }
            //Checks if the current position is between field result, then move to field begin.
            string selectionEndIndex = GetHierarchicalIndex();
            ValidateBackwardFieldSelection(currentIndex, selectionEndIndex);
        }
        /// <summary>
        /// Moves up in table.
        /// </summary>
        private void MoveUpInTable()
        {
            bool isPositionUpdated = false;
            TextPosition end = OwnerControl.Selection.Start;
            bool isBackwardSelection = !OwnerControl.Selection.IsForward;
            if (isPositionUpdated = end.Paragraph.IsInsideTable)
            {
                TableCellAdv startCell = currentParagraph.AssociatedCell;
                TableCellAdv endCell = end.Paragraph.AssociatedCell;
                TableCellAdv containerCell = endCell.GetContainerCell(startCell);
                if (isPositionUpdated = containerCell.OwnerTable.Contains(startCell))
                {
                    endCell = endCell.GetSelectedCell(containerCell);
                    startCell = startCell.GetSelectedCell(containerCell);
                    bool isInContainerCell = containerCell.Contains(currentParagraph.AssociatedCell);
                    bool isContainerCellSelected = containerCell.IsCellSelected(this, end);
                    if (!isContainerCellSelected)
                        isContainerCellSelected = currentParagraph == containerCell.GetFirstParagraph() && IsAtParagraphStart;
                    if ((isInContainerCell && isContainerCellSelected
                        || !isInContainerCell) && startCell.OwnerRow.PreviousNode != null)
                    {
                        //Moves to cell in previous row.
                        TableRowAdv row = startCell.OwnerRow.PreviousNode as TableRowAdv;
                        TableCellAdv cell = row.GetFirstCellInRegion(containerCell, OwnerControl.Selection.UpDownSelectionLength);
                        SetPosition(cell.GetFirstParagraph(), true);
                        return;
                    }
                    else if (isInContainerCell && isContainerCellSelected && startCell.OwnerRow.PreviousNode == null || !isInContainerCell)
                    {
                        if (isBackwardSelection)
                        {
                            //Moves to first cell of row.
                            startCell = startCell.OwnerRow.Cells[0];
                            SetPosition(startCell.GetFirstParagraph(), true);
                        }
                        else
                        {
                            //Moves to last cell of row.
                            startCell = startCell.OwnerRow.Cells[startCell.OwnerRow.Cells.Count - 1];
                            SetPosition(startCell.GetLastParagraph(), false);
                        }
                    }
                }
            }
            if (!isPositionUpdated)
            {
                //Moves to previous row / previous block.
                TableCellAdv cell = currentParagraph.AssociatedCell.GetContainerCell();
                if (isBackwardSelection)
                {
                    //Moves to first cell of row.
                    cell = cell.OwnerRow.Cells[0];
                    SetPosition(cell.GetFirstParagraph(), true);
                }
                else
                {
                    //Moves to end of row.
                    cell = cell.OwnerRow.Cells[cell.OwnerRow.Cells.Count - 1];
                    SetPosition(cell.GetLastParagraph(), false);
                }
            }
            //Moves to previous row / previous block.
            MoveBackward();
        }
        /// <summary>
        /// Moves to line start.
        /// </summary>
        internal void MoveToLineStart()
        {
            LineWidget currentLine = currentParagraph.GetLineWidget(offset);
            ElementBox firstElement = currentLine.GetFirstElement();
            double startOffset = currentParagraph.GetStartOffset();
            if (firstElement == null && offset > startOffset)
                offset = startOffset;
            else if (firstElement != null)
            {
                int indexInInline = firstElement.GetIndexInInline();
                currentParagraph = firstElement.Inline.OwnerParagraph;
                offset = currentParagraph.GetOffset(firstElement.Inline, indexInInline);
                indexInInline = 0;
                Inline inline = currentParagraph.GetInline(offset, ref indexInInline);
                if (inline is FieldCharacterAdv)
                {
                    //Checks if field character is part of rendered field, otherwise moves to previous rendered content.
                    Inline prevInline = inline.GetPreviousValidInline();
                    if (prevInline != null)
                    {
                        inline = prevInline;
                        currentParagraph = inline.OwnerParagraph;
                        offset = currentParagraph.GetOffset(inline, inline.Length);
                        if (inline is FieldBeginAdv)
                            offset--;
                    }
                }
            }
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Moves to next line.
        /// </summary>
        /// <param name="left">The left.</param>
        internal void MoveToNextLine(double left)
        {
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.SetPosition(this);
            string currentIndex = GetHierarchicalIndex();
            LineWidget currentLine = currentParagraph.GetLineWidget(offset);
            //Moves text position to end of line.
            MoveToLineEnd();
            bool isMoveToLineEnd = !textPosition.IsAtSamePosition(this);
            textPosition.SetPosition(this);
            if (currentParagraph.IsInsideTable)
                MoveDownInTable();
            else
                MoveForward();
            LineWidget nextLine = currentParagraph.GetLineWidget(offset);
            double lineStart = nextLine.GetLeft();
            ElementBox firstElement = nextLine.GetFirstElement();
            double firstItemWidth = firstElement == null ? nextLine.GetWidth(true) : nextLine.GetLeft(firstElement, 1) - lineStart;
            //Moves till the Up/Down selection width.
            if (lineStart < left && (firstItemWidth / 2 < left - lineStart))
            {
                double top = nextLine.GetTop();
                nextLine.UpdateTextPosition(OwnerControl, new Point(left, top), this, true);
                double width = nextLine.GetWidth(true);
                if (width < left - lineStart)
                    MoveToLineEnd();
            }
            else if (isMoveToLineEnd && currentParagraph.IsInsideTable
                && currentParagraph.Owner == OwnerControl.Selection.Start.currentParagraph.Owner)                
                SetPosition(textPosition);
            else if (!isMoveToLineEnd)
                MoveToLineEnd();
            //Checks if the current position is between field result, then move to field end.
            string selectionEndIndex = GetHierarchicalIndex();
            ValidateForwardFieldSelection(currentIndex, selectionEndIndex);
        }
        internal void MoveToInline(Inline inline, int index)
        {
            currentParagraph = inline.OwnerParagraph;
            offset = currentParagraph.GetOffset(inline, index);
            //Updates physical position in current page.
            UpdatePhysicalPosition();
        }
        /// <summary>
        /// Moves down in table.
        /// </summary>
        private void MoveDownInTable()
        {
            bool isPositionUpdated = false;
            bool isForwardSelection = OwnerControl.Selection.IsEmpty || OwnerControl.Selection.IsForward;
            if (isPositionUpdated = OwnerControl.Selection.Start.Paragraph.IsInsideTable)
            {
                TableCellAdv startCell = OwnerControl.Selection.Start.Paragraph.AssociatedCell;
                TableCellAdv endCell = currentParagraph.AssociatedCell;
                TableCellAdv containerCell = startCell.GetContainerCell(endCell);
                if (isPositionUpdated = containerCell.OwnerTable.Contains(endCell))
                {
                    startCell = startCell.GetSelectedCell(containerCell);
                    endCell = endCell.GetSelectedCell(containerCell);
                    bool isInContainerCell = containerCell.Contains(currentParagraph.AssociatedCell);
                    bool isContainerCellSelected = containerCell.IsCellSelected(OwnerControl.Selection.Start, this);
                    if ((isInContainerCell && isContainerCellSelected
                        || !isInContainerCell) && endCell.OwnerRow.NextNode != null)
                    {
                        //Moves to cell in next row.
                        TableRowAdv row = endCell.OwnerRow.NextNode as TableRowAdv;
                        TableCellAdv cell = row.GetLastCellInRegion(containerCell, OwnerControl.Selection.UpDownSelectionLength);
                        SetPosition(cell.GetLastParagraph(), false);
                        return;
                    }
                    else if (isInContainerCell && isContainerCellSelected && endCell.OwnerRow.NextNode == null || !isInContainerCell)
                    {
                        if (isForwardSelection)
                        {
                            //Moves to last cell of row.
                            endCell = endCell.OwnerRow.Cells[endCell.OwnerRow.Cells.Count - 1];
                            SetPosition(endCell.GetLastParagraph(), false);
                        }
                        else
                        {
                            //Moves to first cell of row.
                            endCell = endCell.OwnerRow.Cells[0];
                            SetPosition(endCell.GetFirstParagraph(), true);
                        }
                    }
                }
            }
            if (!isPositionUpdated)
            {
                //Moves to next row / next block.
                TableCellAdv cell = currentParagraph.AssociatedCell.GetContainerCell();
                if (isForwardSelection)
                {
                    //Moves to end of row.
                    cell = cell.OwnerRow.Cells[cell.OwnerRow.Cells.Count - 1];
                    SetPosition(cell.GetLastParagraph(), false);
                }
                else
                {
                    //Moves to first cell of row.
                    cell = cell.OwnerRow.Cells[0];
                    SetPosition(cell.GetFirstParagraph(), true);
                }
            }
            //Moves to next row / next block.
            MoveForward();
        }
        /// <summary>
        /// Moves to line end.
        /// </summary>
        internal void MoveToLineEnd()
        {
            LineWidget currentLine = currentParagraph.GetLineWidget(offset);
            ElementBox firstElement = currentLine.GetFirstElement();
            if (firstElement == null && offset == Paragraph.GetStartOffset())
            {
                offset = Paragraph.GetLength() + 1;
                UpdatePhysicalPosition();
            }
            else if (firstElement != null)
            {
                ElementBox lastElement = currentLine.Children[currentLine.Children.Count - 1];
                int index = lastElement.GetIndexInInline();
                index += lastElement is TextElementBox ? (lastElement as TextElementBox).Length : 1;
                currentParagraph = lastElement.Inline.OwnerParagraph;
                if (index == lastElement.Inline.Length
                    && lastElement.Inline.NextNode == null)
                    offset = currentParagraph.GetLength() + 1;
                else
                {
                    Inline inline = lastElement.Inline;
                    while (inline != null && inline.Length == index && inline.NextNode is FieldCharacterAdv)
                    {
                        Inline nextInline = (inline.NextNode as FieldCharacterAdv).GetNextValidInline();
                        if (inline != nextInline)
                        {
                            inline = nextInline;
                            index = 0;
                        }
                        if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
                        {
                            FieldBeginAdv fieldBegin = inline as FieldBeginAdv;
                            if (fieldBegin.FieldSeparator == null)
                                inline = fieldBegin.FieldEnd;
                            else
                            {
                                inline = fieldBegin.FieldSeparator;
                                currentParagraph = inline.OwnerParagraph;
                                if (currentParagraph == fieldBegin.FieldEnd.OwnerParagraph && !currentParagraph.HasValidInline(inline, fieldBegin.FieldEnd))
                                    inline = fieldBegin.FieldEnd;
                            }
                            currentParagraph = inline.OwnerParagraph;
                        }
                        if (inline is FieldCharacterAdv)
                            index = 1;
                    }
                    if (index == inline.Length && inline.NextNode == null)
                        index++;
                    offset = currentParagraph.GetOffset(inline, index);
                }
                UpdatePhysicalPosition();
            }
        }
        /// <summary>
        /// Validates the backward field selection.
        /// </summary>
        /// <param name="currentIndex">Index of the current.</param>
        /// <param name="selectionEndIndex">End index of the selection.</param>
        internal void ValidateBackwardFieldSelection(string currentIndex, string selectionEndIndex)
        {
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.SetPosition(currentIndex);
            string selectionStartIndex = OwnerControl.Selection.Start.GetHierarchicalIndex();
            while (currentIndex != selectionEndIndex && IsForwardSelection(selectionEndIndex, currentIndex))
            {
                int indexInInline = 0;
                Inline inline = textPosition.Paragraph.GetInline(textPosition.Offset, ref indexInInline);
                if (inline != null)
                {
                    Inline nextInline = inline.GetNextRenderedInline(indexInInline);
                    if (nextInline is FieldBeginAdv)
                    {
                        double fieldEndOffset = (nextInline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetOffset((nextInline as FieldBeginAdv).FieldEnd, 1);
                        string fieldEndIndex = (nextInline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetHierarchicalIndex(fieldEndOffset.ToString());
                        if (!TextPosition.IsForwardSelection(fieldEndIndex, selectionStartIndex))
                        {
                            //If field end is after selection start, move selection start to field end.
                            OwnerControl.Selection.Start.SetPosition((nextInline as FieldBeginAdv).FieldEnd.OwnerParagraph, fieldEndOffset);
                            selectionStartIndex = fieldEndIndex;
                        }
                    }
                }
                if (inline is FieldEndAdv && (inline as FieldEndAdv).FieldBegin != null)
                {
                    double fieldBeginOffset = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetOffset((inline as FieldEndAdv).FieldBegin, 0);
                    string fieldBeginIndex = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetHierarchicalIndex(fieldBeginOffset.ToString());
                    if (!TextPosition.IsForwardSelection(selectionEndIndex, fieldBeginIndex))
                    {
                        //If field begin is before selection end, extend selection end to field begin.
                        MoveToInline((inline as FieldEndAdv).FieldBegin, 0);
                        return;
                    }
                    textPosition.MoveToInline((inline as FieldEndAdv).FieldBegin, 0);
                }
                else
                    textPosition.MovePreviousPosition();
                currentIndex = textPosition.GetHierarchicalIndex();
            }
        }
        /// <summary>
        /// Validates the forward field selection.
        /// </summary>
        /// <param name="currentIndex">Index of the current.</param>
        /// <param name="selectionEndIndex">End index of the selection.</param>
        internal void ValidateForwardFieldSelection(string currentIndex, string selectionEndIndex)
        {
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.SetPosition(currentIndex);
            bool isTextPositionMoved = false;
            while (currentIndex != selectionEndIndex && IsForwardSelection(currentIndex, selectionEndIndex))
            {
                if (!isTextPositionMoved)
                {
                    textPosition.MoveNextPosition();
                    string nextIndex = textPosition.GetHierarchicalIndex();
                    //Handled specifically to break infinite looping, if selection ends at last paragraph mark.
                    if (currentIndex == nextIndex)
                        break;
                }
                int indexInInline = 0;
                Inline inline = textPosition.Paragraph.GetInline(textPosition.Offset, ref indexInInline);
                if (inline != null)
                {
                    string selectionStartIndex = OwnerControl.Selection.Start.GetHierarchicalIndex();
                    if (indexInInline == inline.Length && inline is FieldEndAdv)
                    {
                        double fieldBeginOffset = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetOffset((inline as FieldEndAdv).FieldBegin, 0);
                        string fieldBeginIndex = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetHierarchicalIndex(fieldBeginOffset.ToString());
                        if (!TextPosition.IsForwardSelection(selectionStartIndex, fieldBeginIndex))
                            //If field begin is before selection start, move selection start to field begin.
                            OwnerControl.Selection.Start.SetPosition((inline as FieldEndAdv).FieldBegin.OwnerParagraph, fieldBeginOffset);
                    }
                    inline = inline.GetNextRenderedInline(indexInInline);
                }
                if (isTextPositionMoved = (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null))
                {
                    double fieldEndOffset = (inline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetOffset((inline as FieldBeginAdv).FieldEnd, 1);
                    string fieldEndIndex = (inline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetHierarchicalIndex(fieldEndOffset.ToString());
                    if (!TextPosition.IsForwardSelection(fieldEndIndex, selectionEndIndex))
                    {
                        //If selection end is after field begin, extend selection end to field end.
                        MoveToInline((inline as FieldBeginAdv).FieldEnd, 1);
                        return;
                    }
                    textPosition.MoveToInline((inline as FieldBeginAdv).FieldEnd, 1);
                }
                currentIndex = textPosition.GetHierarchicalIndex();
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Determines whether the specified start and end is forward selection.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        ///   <c>true</c> if the specified start and end is forward selection; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsForwardSelection(string start, string end)
        {
            if (start == end)
                return true;
            string[] selectionStart = start.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] selectionEnd = end.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int length = selectionStart.Length;
            if (length > selectionEnd.Length)
                length = selectionEnd.Length - 1;
            for (int i = 0; i < length; i++)
            {
                double startOffset = double.Parse(selectionStart[i]);
                double endOffset = double.Parse(selectionEnd[i]);
                if (startOffset != endOffset)
                    return startOffset < endOffset;
            }
            return false;
        }
        #endregion
    }
}
