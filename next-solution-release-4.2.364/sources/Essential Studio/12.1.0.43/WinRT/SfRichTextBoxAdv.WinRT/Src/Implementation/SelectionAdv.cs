#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
#if WPF
using System.Windows.Media;
using System.Windows.Shapes;
using FontStyleEnum = System.Windows.FontStyles;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using DPParagraphFormat = Syncfusion.UI.Xaml.RichTextBoxAdv.ParagraphFormat;
using DPCharacterFormat = Syncfusion.UI.Xaml.RichTextBoxAdv.CharacterFormat;
using DPSectionFormat = Syncfusion.UI.Xaml.RichTextBoxAdv.SectionFormat;
using DPTableFormat = Syncfusion.UI.Xaml.RichTextBoxAdv.TableFormat;
using DPCellFormat = Syncfusion.UI.Xaml.RichTextBoxAdv.CellFormat;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public class SelectionAdv : DependencyObject
    {
        #region Fields
        private TextPosition start = null;
        private TextPosition end = null;
        private SfRichTextBoxAdv ownerControl;
        private List<LineWidget> SelectedLines = new List<LineWidget>();
        private List<TableCellWidget> SelectedCells = new List<TableCellWidget>();
        internal double UpDownSelectionLength = 0;
        internal bool IsRetrieveFormatting = false;
        private SelectionCharacterFormat characterFormat;
        private SelectionParagraphFormat paragraphFormat;
        private SelectionSectionFormat sectionFormat;
        private SelectionTableFormat tableFormat;
        private SelectionCellFormat cellFormat;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the new instance of SelectionAdv
        /// </summary>
        public SelectionAdv()
        {
            characterFormat = new SelectionCharacterFormat(this);
            paragraphFormat = new SelectionParagraphFormat(this);
            sectionFormat = new SelectionSectionFormat(this);
            tableFormat = new SelectionTableFormat(this);
            cellFormat = new SelectionCellFormat(this);
        }
        /// <summary>
        /// Initializes the new instance of SelectionAdv
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        internal SelectionAdv(SfRichTextBoxAdv richTextBoxAdv)
            : this()
        {
            ownerControl = richTextBoxAdv;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start position of the selection
        /// </summary>
        public TextPosition Start
        {
            get
            {
                if (start == null && ownerControl != null && ownerControl.Document != null)
                    start = ownerControl.Document.DocumentStart;
                return start;
            }
            internal set
            {
                start = value;
            }
        }
        /// <summary>
        /// Gets the end position of the selection
        /// </summary>
        public TextPosition End
        {
            get
            {
                if (end == null && Start != null)
                    end = Start.Clone();
                return end;
            }
            internal set
            {
                end = value;
            }
        }
        /// <summary>
        /// Gets or Sets the selected Text
        /// </summary>
        internal string Text
        {
            get
            {
                return "";
            }
            set
            {
                if (IsEmpty)
                    return;
                InsertText(value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the selection is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the selection is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                if (Start == null)
                    return true;
                return Start.IsAtSamePosition(End);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this selection is forward.
        /// </summary>
        /// <value>
        /// <c>true</c> if this selection is forward; otherwise, <c>false</c>.
        /// </value>
        internal bool IsForward
        {
            get
            {
                return Start.IsExistBefore(End);
            }
        }
        /// <summary>
        /// Gets the owner SfRichTextBoxAdv control
        /// </summary>
        internal SfRichTextBoxAdv OwnerControl
        {
            get
            {
                return ownerControl;
            }
        }
        /// <summary>
        /// Gets the text edit position.
        /// </summary>
        /// <value>
        /// The text position.
        /// </value>
        internal string EditPosition
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the current history info.
        /// </summary>
        /// <value>
        /// The current history info.
        /// </value>
        internal HistoryInfo CurrentHistoryInfo
        {
            get;
            set;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is cleared.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is cleared; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCleared
        {
            get
            {
                return end == null;
            }
        }
        /// <summary>
        /// Gets or sets the word document.
        /// </summary>
        /// <value>
        /// The word document.
        /// </value>
        internal DocIO.DLS.WordDocument WordDocument
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the selection character format.
        /// </summary>
        /// <value>
        /// The selection character format.
        /// </value>
        public SelectionCharacterFormat CharacterFormat
        {
            get
            {
                return characterFormat;
            }
        }
        /// <summary>
        /// Gets the selection paragraph format.
        /// </summary>
        /// <value>
        /// The selection paragraph format.
        /// </value>
        public SelectionParagraphFormat ParagraphFormat
        {
            get
            {
                return paragraphFormat;
            }
        }
        /// <summary>
        /// Gets the selection section format.
        /// </summary>
        /// <value>
        /// The selection section format.
        /// </value>
        public SelectionSectionFormat SectionFormat
        {
            get
            {
                return sectionFormat;
            }
        }
        /// <summary>
        /// Gets the selection table format.
        /// </summary>
        /// <value>
        /// The selection table format.
        /// </value>
        internal SelectionTableFormat TableFormat
        {
            get
            {
                return tableFormat;
            }
        }
        /// <summary>
        /// Gets the selection cell format.
        /// </summary>
        /// <value>
        /// The selection cell format.
        /// </value>
        internal SelectionCellFormat CellFormat
        {
            get
            {
                return cellFormat;
            }
        }
        #endregion

        #region Selection Implementation
        /// <summary>
        /// Selects all the content.
        /// </summary>
        internal void SelectAll()
        {
            //Selects the entire document.
            //If the selection is in document text body, then entire document (include HF) should be copied to clipboard.
            TextPosition documentStart = null;
            if (OwnerControl.Document != null)
                documentStart = OwnerControl.Document.DocumentStart;
            if (documentStart != null)
            {
                Start.SetPosition(documentStart);
                TextPosition documentEnd = OwnerControl.Document.DocumentEnd;
                End.SetPosition(documentEnd.Paragraph, documentEnd.Offset + 1);
                UpDownSelectionLength = End.Location.X;
                FireSelectionChanged(true);
            }
        }
        /// <summary>
        /// Selects the content within the specified text position.
        /// </summary>
        /// <param name="textPosition">The text position.</param>
        internal void Select(TextPosition textPosition)
        {
            if (textPosition == null)
                throw new ArgumentNullException("textPosition is null.");

            Start.SetPosition(textPosition);
            End.SetPosition(textPosition);
            UpDownSelectionLength = End.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Selects the content within the specified start and end position.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        internal void Select(TextPosition startPosition, TextPosition endPosition)
        {
            if (startPosition == null)
                throw new ArgumentNullException("startPosition is null.");
            if (endPosition == null)
                throw new ArgumentNullException("endPosition is null.");
            if (!startPosition.IsInSameDocument(endPosition))
                throw new ArgumentException("startPosition and endPosition are not in the same document.");

            Start.SetPosition(startPosition);
            End.SetPosition(endPosition);
            UpDownSelectionLength = End.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Selects the specified paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="inline">The inline.</param>
        /// <param name="index">The index.</param>
        /// <param name="physicalLocation">The physical location.</param>
        internal void Select(ParagraphAdv paragraph, Inline inline, int index, Point physicalLocation)
        {
            Start.SetPosition(paragraph, inline, index, physicalLocation);
            End.SetPosition(Start);
            UpDownSelectionLength = physicalLocation.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Selects the specified paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="offset">The offset.</param>
        internal void Select(ParagraphAdv paragraph, double offset)
        {
            Start.SetPosition(paragraph, offset);
            End.SetPosition(Start);
            UpDownSelectionLength = Start.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Selects the specified paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="positionAtStart">if set to <c>true</c> [position at start].</param>
        internal void Select(ParagraphAdv paragraph, bool positionAtStart)
        {
            if (positionAtStart)
                Start.SetPosition(paragraph, positionAtStart);
            else
            {
                double endOffset = paragraph.GetEndOffset();
                Start.SetPosition(paragraph, endOffset);
            }
            End.SetPosition(Start);
            UpDownSelectionLength = Start.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Selects the table cell.
        /// </summary>
        /// <param name="tableCell">The table cell.</param>
        internal void SelectTableCell(TableCellAdv tableCell)
        {
            ParagraphAdv firstParagraph = tableCell.GetFirstParagraph();
            ParagraphAdv lastParagraph = tableCell.GetLastParagraph();
            if (firstParagraph == lastParagraph && lastParagraph.IsEmpty())
                Select(lastParagraph, true);
            else
            {
                Start.SetPosition(firstParagraph, true);
                End.SetPosition(lastParagraph, lastParagraph.GetEndOffset());
                HighlightSelection(false);
            }
        }
        /// <summary>
        /// Clears the selection.
        /// </summary>
        internal void ClearSelection()
        {
            start = null;
            end = null;
            UpDownSelectionLength = 0;
            ClearSelectionHighlight();
        }
        /// <summary>
        /// Moves the text position to line start.
        /// </summary>
        internal void MoveToLineStart()
        {
            if (Start == null)
                return;
            if (!IsEmpty)
            {
                if (IsForward)
                    End.SetPosition(Start);
                else
                    Start.SetPosition(End);
            }
            Start.MoveToLineStart();
            End.SetPosition(Start);
            UpDownSelectionLength = Start.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Moves the text position to line end.
        /// </summary>
        internal void MoveToLineEnd()
        {
            if (Start == null)
                return;
            if (!IsEmpty)
            {
                if (IsForward)
                    Start.SetPosition(End);
                else
                    End.SetPosition(Start);
            }
            Start.MoveToLineEnd();
            End.SetPosition(Start);
            UpDownSelectionLength = Start.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Moves the next position.
        /// </summary>
        internal void MoveNextPosition()
        {
            if (Start == null)
                return;
            if (IsEmpty)
            {
                Start.MoveNextPosition();
                End.SetPosition(Start);
            }
            else
            {
                if (IsForward)
                    Start.SetPosition(End);
                else
                    End.SetPosition(Start);
            }
            UpDownSelectionLength = Start.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Moves the previous position.
        /// </summary>
        internal void MovePreviousPosition()
        {
            if (Start == null)
                return;
            if (IsEmpty)
            {
                Start.MovePreviousPosition();
                End.SetPosition(Start);
            }
            else
            {
                if (IsForward)
                    End.SetPosition(Start);
                else
                    Start.SetPosition(End);
            }
            UpDownSelectionLength = Start.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Moves the caret position up.
        /// </summary>
        internal void MoveUp()
        {
            if (Start == null)
                return;
            if (!IsEmpty)
            {
                if (IsForward)
                    End.SetPosition(Start);
                else
                    Start.SetPosition(End);
                UpDownSelectionLength = Start.Location.X;
            }
            Start.MoveUp(UpDownSelectionLength);
            End.SetPosition(Start);
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Moves the caret position down.
        /// </summary>
        internal void MoveDown()
        {
            if (Start == null)
                return;
            if (!IsEmpty)
            {
                if (IsForward)
                    Start.SetPosition(End);
                else
                    End.SetPosition(Start);
                UpDownSelectionLength = Start.Location.X;
            }
            Start.MoveDown(UpDownSelectionLength);
            End.SetPosition(Start);
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Extends to line end.
        /// </summary>
        internal void ExtendToLineEnd()
        {
            if (Start == null)
                return;
            End.MoveToLineEnd();
            UpDownSelectionLength = End.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Extends to line start.
        /// </summary>
        internal void ExtendToLineStart()
        {
            if (Start == null)
                return;
            End.MoveToLineStart();
            UpDownSelectionLength = End.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Extends the selection forward.
        /// </summary>
        internal void ExtendForward()
        {
            if (Start == null)
                return;
            End.MoveForward();
            UpDownSelectionLength = End.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Extends the selection backward.
        /// </summary>
        internal void ExtendBackward()
        {
            if (Start == null)
                return;
            End.MoveBackward();
            UpDownSelectionLength = End.Location.X;
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Extends the selection to next line.
        /// </summary>
        internal void ExtendToNextLine()
        {
            if (Start == null)
                return;
            End.MoveToNextLine(UpDownSelectionLength);
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Extends the selection to previous line.
        /// </summary>
        internal void ExtendToPreviousLine()
        {
            if (Start == null)
                return;
            End.MoveToPreviousLine(UpDownSelectionLength);
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Moves the text position.
        /// </summary>
        /// <param name="cursorPoint">The cursor point.</param>
        internal void MoveTextPosition(Point cursorPoint)
        {
            if (Start == null)
                return;
            //Updates the text position based on the cursor position.
            LineWidget widget = OwnerControl.Viewer.GetLineWidget(cursorPoint);
            if (widget != null)
                widget.UpdateTextPosition(OwnerControl, cursorPoint, End, true);
            UpDownSelectionLength = End.Location.X;
            double length = End.Paragraph.GetLength();
            if (!IsEmpty && !IsForward && End.Offset > length)
            {
                int indexInInline = 0;
                Inline inline = End.Paragraph.GetInline(length, ref indexInInline);
                ParagraphAdv paragraph = End.Paragraph;
                double endOffset = 0;
                if (inline != null)
                {
                    inline = inline.GetPreviousValidInline();
                    indexInInline = inline is FieldCharacterAdv ? 0 : inline.Length;
                    if (inline is FieldEndAdv)
                        indexInInline++;
                    endOffset = inline.OwnerParagraph.GetOffset(inline, indexInInline);
                    paragraph = inline.OwnerParagraph;
                }
                End.SetPosition(paragraph, endOffset);
            }
            string selectionStartIndex = Start.GetHierarchicalIndex();
            string selectionEndIndex = End.GetHierarchicalIndex();
            if (selectionStartIndex != selectionEndIndex)
            {
                //Extends selection end to field begin or field end.
                if (TextPosition.IsForwardSelection(selectionStartIndex, selectionEndIndex))
                    End.ValidateForwardFieldSelection(selectionStartIndex, selectionEndIndex);
                else
                    End.ValidateBackwardFieldSelection(selectionStartIndex, selectionEndIndex);
            }
            FireSelectionChanged(true);
        }
        /// <summary>
        /// Fires the selection changed.
        /// </summary>
        /// <param name="isSelectionChanged">if set to <c>true</c> is selection changed.</param>
        internal void FireSelectionChanged(bool isSelectionChanged)
        {
            // To retrieve Formats of selected contents
            RetrieveCurrentFormatProperties();
            if (isSelectionChanged)
                ownerControl.FireSelectionChanged(new SelectionChangedEventArgs());
            ClearSelectionHighlight();
            if (ownerControl.IsLayoutEnabled && !ownerControl.IsShiftingEnabled)
                HighlightSelection(isSelectionChanged);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Inits the history.
        /// </summary>
        /// <param name="action">The action.</param>
        private void InitHistory(Actions action)
        {
            CurrentHistoryInfo = new HistoryInfo(OwnerControl);
            CurrentHistoryInfo.Action = action;
            CurrentHistoryInfo.UpdateSelection(this);
        }
        /// <summary>
        /// Fits the image to page.
        /// </summary>
        /// <param name="image">The image.</param>
        private void FitImageToPage(ImageContainerAdv image)
        {
            SectionAdv section = Start.Paragraph.Section;
            double pageWidth = section.SectionFormat.PageSize.Width - section.SectionFormat.PageMargin.Left - section.SectionFormat.PageMargin.Right;
            double pageHeight = section.SectionFormat.PageSize.Height - section.SectionFormat.PageMargin.Top - section.SectionFormat.PageMargin.Bottom;
            //Resizes image to page size.
            if (image.Width > pageWidth)
            {
                image.Height = image.Height * pageWidth / image.Width;
                image.Width = pageWidth;
            }
            if (image.Height > pageHeight)
            {
                image.Width = image.Width * pageHeight / image.Height;
                image.Height = pageHeight;
            }
        }
        /// <summary>
        /// Determines whether this instance can merge selected cells.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can merge selected cells; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanMergeSelectedCells()
        {
            if (IsEmpty || !Start.Paragraph.IsInsideTable || !End.Paragraph.IsInsideTable)
                return false;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            TableCellAdv containerCell = startPosition.Paragraph.AssociatedCell.GetContainerCell(endPosition.Paragraph.AssociatedCell);

            if (containerCell.OwnerTable.Contains(endPosition.Paragraph.AssociatedCell))
            {
                if (!containerCell.Contains(endPosition.Paragraph.AssociatedCell))
                {
                    //Start and End are in different cells.
                    TableCellAdv startCell = startPosition.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    TableCellAdv endCell = endPosition.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    //Returns if Start and End are in same row.
                    if (startCell.OwnerRow == endCell.OwnerRow)
                        return true;
                    return startCell.OwnerTable.CanMergeSelectedCells(this, startCell, endCell);
                }
            }
            return false;
        }
        /// <summary>
        /// Removes the content of the selected.
        /// </summary>
        /// <returns></returns>
        internal bool RemoveSelectedContent()
        {
            OwnerControl.IsShiftingEnabled = true;
            bool isRemoved;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            if (startPosition.Paragraph == endPosition.Paragraph && startPosition.Offset == startPosition.Paragraph.GetLength()
                && startPosition.Offset + 1 == endPosition.Offset)
            {
                Select(startPosition);
                return true;
            }
            EditPosition = startPosition.GetHierarchicalIndex();
            isRemoved = startPosition.Paragraph.RemoveSelectedContent(this, startPosition, endPosition);
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.SetPosition(EditPosition);
            Select(textPosition);
            return isRemoved;
        }
        /// <summary>
        /// Deletes the content of the selected.
        /// </summary>
        /// <param name="isBackSpace">if set to <c>true</c> [is back space].</param>
        internal bool DeleteSelectedContent(bool isBackSpace)
        {
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            EditPosition = startPosition.GetHierarchicalIndex();
            bool skipBackSpace = false;
            if (isBackSpace && startPosition.IsInSameParagraph(endPosition))
            {
                //Handled specifically to skip removal of contents, if selection is only paragraph mark and next rendered block is table.
                if (startPosition.Offset < endPosition.Offset && startPosition.Offset == endPosition.Paragraph.GetLength())
                {
                    BlockAdv nextBlock = startPosition.Paragraph.GetNextRenderedBlock();
                    skipBackSpace = nextBlock is TableAdv;
                }
                //Handled specifically to remove paragraph completely (Delete behavior), if the selected paragraph is empty.
                if (endPosition.Offset == 1 && endPosition.Offset > endPosition.Paragraph.GetLength()
                    && !(endPosition.Paragraph.IsInsideTable && endPosition.Paragraph.NextBlock == null))
                    isBackSpace = false;
            }
            if (!skipBackSpace)
            {
                OwnerControl.IsShiftingEnabled = true;
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = EditPosition;
                byte editAction = (byte)(isBackSpace ? 1 : 0);
                startPosition.Paragraph.DeleteSelectedContent(this, startPosition, endPosition, editAction);
            }
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.SetPosition(EditPosition);
            Select(textPosition);
            return skipBackSpace;
        }
        /// <summary>
        /// Updates the character format.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void UpdateCharacterFormat(DependencyProperty property, object value)
        {
            OwnerControl.IsShiftingEnabled = true;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = startPosition.GetHierarchicalIndex();
            startPosition.Paragraph.ApplyCharacterFormatForSelection(this, startPosition, endPosition, property, value);
        }
        /// <summary>
        /// Updates the paragraph format.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void UpdateParagraphFormat(DependencyProperty property, object value)
        {
            OwnerControl.IsShiftingEnabled = true;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = startPosition.GetHierarchicalIndex();
            startPosition.Paragraph.ApplyParagraphFormatForSelection(this, startPosition, endPosition, property, value);
        }
        /// <summary>
        /// Updates the section format.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void UpdateSectionFormat(DependencyProperty property, object value)
        {
            OwnerControl.IsShiftingEnabled = true;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = startPosition.GetHierarchicalIndex();
            int startSectionIndex = ownerControl.Document.Sections.IndexOf(start.Paragraph.Section);
            int endSectionIndex = ownerControl.Document.Sections.IndexOf(end.Paragraph.Section);
            for (int i = startSectionIndex; i <= endSectionIndex; i++)
                ownerControl.Document.Sections[i].SectionFormat.ApplyPropertyValue(CurrentHistoryInfo, property, value);
            UpdateHistory();
        }
        /// <summary>
        /// Updates the image size.
        /// </summary>
        /// <param name="imageFormat">The image format.</param>
        internal void UpdateImageSize(ImageFormat imageFormat)
        {
            OwnerControl.IsShiftingEnabled = true;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            } Inline inline = null;
            int index = 0;
            ParagraphAdv paragraph = startPosition.Paragraph;
            if (!OwnerControl.IsReadOnlyMode && paragraph == endPosition.Paragraph
                && startPosition.Offset + 1 == endPosition.Offset)
                inline = paragraph.GetInline(endPosition.Offset, ref index);
            if (inline is ImageContainerAdv)
            {
                double width = (inline as ImageContainerAdv).Width;
                double height = (inline as ImageContainerAdv).Height;
                (inline as ImageContainerAdv).Width = imageFormat.Width;
                (inline as ImageContainerAdv).Height = imageFormat.Height;
                imageFormat.Width = width;
                imageFormat.Height = height;
                if (paragraph != null && paragraph.BaseParent != null)
                {
                    paragraph.Relayout(paragraph.Inlines.IndexOf(inline));
                    HighlightSelection(false);
                }
            }
        }
        /// <summary>
        /// Relayouts the specified document.
        /// </summary>
        /// <param name="isEmptySelection">if set to <c>true</c> is empty selection.</param>
        internal void Relayout(bool isEmptySelection)
        {
            if (OwnerControl.IsShiftingEnabled)
            {
                OwnerControl.IsShiftingEnabled = false;
                OwnerControl.Viewer.ShiftLayoutedItems();
                Start.UpdatePhysicalPosition();
                if (isEmptySelection)
                    End.SetPosition(Start);
                else
                    End.UpdatePhysicalPosition();
                UpDownSelectionLength = End.Location.X;
                FireSelectionChanged(isEmptySelection);
#if WPF
                OwnerControl.Focus();
#else
                OwnerControl.Focus(FocusState.Pointer);
#endif
            }
            UpdateHistory();
            FireContentChanged();
        }
        /// <summary>
        /// Fires the content changed.
        /// </summary>
        private void FireContentChanged()
        {
            if (OwnerControl.IsLayoutEnabled && !OwnerControl.IsShiftingEnabled)
            {
                ContentChangedEventArgs args = new ContentChangedEventArgs();
                OwnerControl.FireContentChanged(args);
            }
        }
        /// <summary>
        /// Updates the history.
        /// </summary>
        private void UpdateHistory()
        {
            if (CurrentHistoryInfo != null)
            {
                //Updates the current end position
                if (CurrentHistoryInfo.EndPosition == null)
                    CurrentHistoryInfo.EndPosition = CurrentHistoryInfo.InsertPosition;
                OwnerControl.History.RecordChanges(CurrentHistoryInfo);
                CurrentHistoryInfo = null;
            }
        }
        /// <summary>
        /// Gets the hyperlink field in current selection.
        /// </summary>
        /// <returns></returns>
        internal FieldBeginAdv GetHyperlinkField()
        {
            if (End == null)
                return null;
            int index = 0;
            Inline inline = End.Paragraph.GetInline(end.Offset, ref index);
            //Check if inline is within a field result.
            List<FieldBeginAdv> CheckedFields = new List<FieldBeginAdv>();
            FieldBeginAdv field = null;
            if (inline == null)
                field = End.Paragraph.GetHyperlinkField(End.Paragraph, CheckedFields);
            else
                field = inline.OwnerParagraph.GetHyperlinkField(inline, CheckedFields);
            CheckedFields.Clear();
            CheckedFields = null;
            return field;
        }
        #endregion

        #region Edit Implementation
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="text">The text.</param>
        internal void InsertText(string text)
        {
            if (Start == null)
                return;
            InitHistory(Actions.Insert);
            bool isRemoved = true;
            if (!IsEmpty)
                isRemoved = RemoveSelectedContent();
            if (isRemoved)
            {
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                TextPosition insertPosition = Start;
                if (insertPosition.Paragraph.IsEmpty())
                {
                    SpanAdv span = new SpanAdv();
                    span.CharacterFormat.CopyFormat(insertPosition.Paragraph.CharacterFormat);
                    span.Text = text;
                    insertPosition.Paragraph.Inlines.Add(span);
                }
                else
                {
                    int indexInInline = 0;
                    Inline inline = insertPosition.Paragraph.GetInline(insertPosition.Offset, ref indexInInline);
                    inline.InsertText(this, text, indexInInline);
                }
                Select(insertPosition.Paragraph, insertPosition.Offset + text.Length);
                if (CurrentHistoryInfo.EndPosition == null)
                    CurrentHistoryInfo.EndPosition = Start.GetHierarchicalIndex();
                Relayout(true);
            }
            else
                Select(Start);
        }
        /// <summary>
        /// Inserts the image.
        /// </summary>
        /// <param name="image">The image.</param>
        internal void InsertImage(ImageContainerAdv image)
        {
            InitHistory(Actions.InsertInline);
            FitImageToPage(image);
            InsertInline(image);
            Relayout(true);
        }
        /// <summary>
        /// Edits the hyperlink.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="displayText">The display text.</param>
        /// <returns></returns>
        internal bool EditHyperlink(string url, string displayText)
        {
            FieldBeginAdv fieldBegin = GetHyperlinkField();
            if (fieldBegin == null)
                return false;
            InitHistory(Actions.InsertHyperlink);
            string fieldResult = "";
            bool isNestedField = false;
            CharacterFormat format = GetCharacterFormat();
            FieldSeparatorAdv fieldSeparator = null;
            if (fieldBegin.FieldSeparator != null)
            {
                fieldSeparator = fieldBegin.FieldSeparator;
                fieldResult = fieldBegin.FieldSeparator.OwnerParagraph.GetHyperlinkDisplayText(fieldBegin.FieldSeparator, fieldBegin.FieldEnd, ref isNestedField, ref format);
            }
            double offset = fieldBegin.OwnerParagraph.GetOffset(fieldBegin, 0);
            Start.SetPosition(fieldBegin.OwnerParagraph, offset);
            offset = fieldBegin.FieldEnd.OwnerParagraph.GetOffset(fieldBegin.FieldEnd, 1);
            End.SetPosition(fieldBegin.FieldEnd.OwnerParagraph, offset);
            DeleteSelectedContent(false);
            if (!isNestedField && fieldResult != displayText || fieldSeparator == null)
                InsertHyperlinkInternal(url, displayText, format);
            else
            {
                //Modify the new hyperlink url. Inserts field begin, url and field separator.
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                FieldBeginAdv newfieldBegin = new FieldBeginAdv();
                newfieldBegin.CharacterFormat.CopyFormat(fieldBegin.CharacterFormat);
                InsertInlineInternal(newfieldBegin);
                SpanAdv span = new SpanAdv();
                span.CharacterFormat.CopyFormat(fieldBegin.CharacterFormat);
                span.Text = " HYPERLINK \"" + url + "\" ";
                InsertInlineInternal(span);
                CurrentHistoryInfo.InsertClonedFieldResult(fieldSeparator);
                Select(newfieldBegin.FieldEnd.OwnerParagraph, offset);
                CurrentHistoryInfo.EndPosition = Start.GetHierarchicalIndex();
                Relayout(true);
            }
            return true;
        }
        /// <summary>
        /// Inserts the hyperlink.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="displayText">The display text.</param>
        internal void InsertHyperlink(string url, string displayText)
        {
            if (Start == null)
                return;
            if (EditHyperlink(url, displayText))
                return;
            InitHistory(Actions.InsertHyperlink);
            bool isRemoved = true;
            if (!IsEmpty)
                isRemoved = RemoveSelectedContent();
            if (isRemoved)
            {
                CharacterFormat format = GetCharacterFormat();
                InsertHyperlinkInternal(url, displayText, format);
            }
            else
                Select(Start);
        }
        /// <summary>
        /// Inserts the hyperlink internal.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="format">The format.</param>
        private void InsertHyperlinkInternal(string url, string displayText, CharacterFormat format)
        {
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            OwnerControl.IsShiftingEnabled = true;
            FieldBeginAdv fieldBegin = new FieldBeginAdv();
            fieldBegin.CharacterFormat.CopyFormat(format);
            InsertInlineInternal(fieldBegin);
            SpanAdv span = new SpanAdv();
            span.CharacterFormat.CopyFormat(format);
            span.Text = " HYPERLINK \"" + url + "\" ";
            InsertInlineInternal(span);
            FieldSeparatorAdv fieldSeparator = new FieldSeparatorAdv();
            fieldSeparator.CharacterFormat.CopyFormat(format);
            InsertInlineInternal(fieldSeparator);
            if (!string.IsNullOrEmpty(displayText))
            {
                span = new SpanAdv();
                span.CharacterFormat.CopyFormat(format);
                span.CharacterFormat.Underline = Underline.Single;
                span.CharacterFormat.FontColor = Color.FromArgb(255, 5, 99, 193);
                span.Text = displayText;
                InsertInlineInternal(span);
            }
            FieldEndAdv fieldEnd = new FieldEndAdv();
            fieldEnd.CharacterFormat.CopyFormat(format);
            InsertInlineInternal(fieldEnd);
            double offset = fieldEnd.OwnerParagraph.GetOffset(fieldEnd, 1);
            Select(fieldEnd.OwnerParagraph, offset);
            CurrentHistoryInfo.EndPosition = Start.GetHierarchicalIndex();
            Relayout(true);
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <returns></returns>
        private CharacterFormat GetCharacterFormat()
        {
            if (Start.Paragraph.IsEmpty())
                return Start.Paragraph.CharacterFormat;
            else
            {
                int indexInInline = 0;
                Inline curInline = Start.Paragraph.GetInline(Start.Offset, ref indexInInline);
                return curInline.CharacterFormat;
            }
        }
        /// <summary>
        /// Inserts the inline internal.
        /// </summary>
        /// <param name="inline">The inline.</param>
        internal void InsertInlineInternal(Inline inline)
        {
            if (Start.Paragraph.IsEmpty())
                Start.Paragraph.Inlines.Add(inline);
            else
            {
                int indexInInline = 0;
                Inline curInline = Start.Paragraph.GetInline(Start.Offset, ref indexInInline);
                curInline.InsertInline(inline, indexInInline);
            }
            Select(inline.OwnerParagraph, Start.Offset + inline.Length);
        }
        /// <summary>
        /// Inserts the inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        internal void InsertInline(Inline inline)
        {
            if (!IsEmpty)
                RemoveSelectedContent();
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            InsertInlineInternal(inline);
            if (CurrentHistoryInfo.EndPosition == null)
                CurrentHistoryInfo.EndPosition = Start.GetHierarchicalIndex();
            FireContentChanged();
        }
        /// <summary>
        /// Inserts the table.
        /// </summary>
        /// <param name="table">The table.</param>
        internal void InsertTable(TableAdv table)
        {
            OwnerControl.IsShiftingEnabled = true;
            InitHistory(Actions.InsertTable);
            InsertBlock(table);
            double clientWidth = table.GetContainerWidth();
            table.FitCellsToClientArea(clientWidth);
            //Layouts the table.
            table.Layout();
            Start.SetPosition(table.GetFirstParagraphInFirstCell(), true);
            End.SetPosition(Start);
            ParagraphAdv lastPargaraph = table.GetLastParagraphInLastCell();
            double endOffset = lastPargaraph.GetLength() + 1;
            CurrentHistoryInfo.EndPosition = lastPargaraph.GetHierarchicalIndex(endOffset.ToString());
            Relayout(true);
        }
        /// <summary>
        /// Inserts the row.
        /// </summary>
        /// <param name="rowPlacement">The row placement.</param>
        internal void InsertRow(RowPlacement rowPlacement)
        {
            if (Start.Paragraph.IsInsideTable)
            {
                if (!OwnerControl.History.IsRedoing)
                    InitHistory(rowPlacement == RowPlacement.Above ? Actions.InsertRowAbove : Actions.InsertRowBelow);
                OwnerControl.IsShiftingEnabled = true;
                TableRowAdv row = Start.Paragraph.AssociatedCell.OwnerRow;
                //Clones the entire table to preserve in history.
                TableAdv clonedTable = row.OwnerTable.CloneTableToHistoryInfo(CurrentHistoryInfo);
                //Inserts new row to the table.
                TableRowAdv insertRow = row.GetBlankRow();
                int index = row.RowIndex;
                if (rowPlacement == RowPlacement.Below)
                    index++;
                row.OwnerTable.Rows.Insert(index, insertRow);
                Start.SetPosition(insertRow.Cells[0].GetFirstParagraph(), true);
                End.SetPosition(insertRow.Cells[insertRow.Cells.Count - 1].GetLastParagraph(), false);
                if (!OwnerControl.History.IsRedoing)
                    Relayout(true);
            }
        }
        /// <summary>
        /// Inserts the column.
        /// </summary>
        /// <param name="columnPlacement">The column placement.</param>
        internal void InsertColumn(ColumnPlacement columnPlacement)
        {
            if (Start.Paragraph.IsInsideTable)
            {
                if (!OwnerControl.History.IsRedoing)
                    InitHistory(columnPlacement == ColumnPlacement.Left ? Actions.InsertColumnLeft : Actions.InsertColumnRight);
                OwnerControl.IsShiftingEnabled = true;
                TableCellAdv cell = Start.Paragraph.AssociatedCell;
                TableAdv table = cell.OwnerRow.OwnerTable;
                //Clones the entire table to preserve in history.
                TableAdv clonedTable = table.CloneTableToHistoryInfo(CurrentHistoryInfo);
                OwnerControl.IsLayoutEnabled = false;
                int cellIndex = cell.CellIndex;
                if (columnPlacement == ColumnPlacement.Right)
                    cellIndex++;
                ParagraphAdv startParagraph = null;
                TableCellAdv newCell = null;
                foreach (TableRowAdv row in table.Rows)
                {
                    newCell = cell.GetBlankCell();
                    if (startParagraph == null)
                        startParagraph = newCell.GetFirstParagraph();
                    if (cellIndex > row.Cells.Count)
                        row.Cells.Add(newCell);
                    else
                        row.Cells.Insert(cellIndex, newCell);
                }
                double clientWidth = table.GetContainerWidth();
                if (Math.Round(clientWidth) < Math.Round(table.TableWidth))
                    table.FitCellsToClientArea(clientWidth);
                OwnerControl.IsLayoutEnabled = true;
                //Layouts the table.
                table.Layout();
                Start.SetPosition(startParagraph, true);
                End.SetPosition(newCell.GetLastParagraph(), false);
                if (!OwnerControl.History.IsRedoing)
                    Relayout(true);
            }
        }
        /// <summary>
        /// Deletes the table.
        /// </summary>
        internal void DeleteTable()
        {
            if (Start.Paragraph.IsInsideTable)
            {
                TableAdv table = Start.Paragraph.AssociatedCell.OwnerTable;
                OwnerControl.IsShiftingEnabled = true;
                if (!OwnerControl.History.IsRedoing)
                    InitHistory(Actions.DeleteTable);
                //Sets the insert position in history info as current table.
                CurrentHistoryInfo.InsertPosition = table.GetHierarchicalIndex("0");
                ParagraphAdv paragraph = null;
                if (table.NextBlock != null)
                    paragraph = table.NextBlock is ParagraphAdv ? table.NextBlock as ParagraphAdv : (table.NextBlock as TableAdv).GetFirstParagraphInFirstCell();
                else if (table.PreviousBlock != null)
                    paragraph = table.PreviousBlock is ParagraphAdv ? table.PreviousBlock as ParagraphAdv : (table.NextBlock as TableAdv).GetLastParagraphInLastCell();
                table.RemoveBlock();
                CurrentHistoryInfo.RemovedNodes.Add(table);
                Select(paragraph, true);
                if (!OwnerControl.History.IsRedoing)
                    Relayout(true);
            }
        }
        /// <summary>
        /// Deletes the row.
        /// </summary>
        internal void DeleteRow()
        {
            if (Start.Paragraph.IsInsideTable)
            {
                TableRowAdv row = Start.Paragraph.AssociatedCell.OwnerRow;
                if (!OwnerControl.History.IsRedoing)
                    InitHistory(Actions.DeleteRow);
                OwnerControl.IsShiftingEnabled = true;
                //Clones the entire table to preserve in history.
                TableAdv clonedTable = row.OwnerTable.CloneTableToHistoryInfo(CurrentHistoryInfo);
                ParagraphAdv paragraph = null;
                if (row.NextNode != null)
                {
                    TableCellAdv nextCell = (row.NextNode as TableRowAdv).Cells[0];
                    paragraph = nextCell.GetFirstParagraph();
                }
                if (paragraph == null)
                {
                    if (row.OwnerTable.NextBlock != null)
                        paragraph = row.OwnerTable.NextBlock is ParagraphAdv ? row.OwnerTable.NextBlock as ParagraphAdv : (row.OwnerTable.NextBlock as TableAdv).GetFirstParagraphInFirstCell();
                    else if (row.OwnerTable.PreviousBlock != null)
                        paragraph = row.OwnerTable.PreviousBlock is ParagraphAdv ? row.OwnerTable.PreviousBlock as ParagraphAdv : (row.OwnerTable.NextBlock as TableAdv).GetLastParagraphInLastCell();
                }
                if (row.OwnerTable.Rows.Count == 1)
                {
                    row.OwnerTable.RemoveBlock();
                    CurrentHistoryInfo.Action = Actions.Delete;
                }
                else
                    row.OwnerTable.Rows.Remove(row);
                Select(paragraph, true);
                if (!OwnerControl.History.IsRedoing)
                    Relayout(true);
            }
        }
        /// <summary>
        /// Deletes the column.
        /// </summary>
        internal void DeleteColumn()
        {
            if (Start.Paragraph.IsInsideTable)
            {
                if (!OwnerControl.History.IsRedoing)
                    InitHistory(Actions.DeleteColumn);
                OwnerControl.IsShiftingEnabled = true;
                TableCellAdv cell = Start.Paragraph.AssociatedCell;
                TableAdv table = cell.OwnerTable;
                //Clones the entire table to preserve in history.
                TableAdv clonedTable = table.CloneTableToHistoryInfo(CurrentHistoryInfo);
                OwnerControl.IsLayoutEnabled = false;
                ParagraphAdv paragraph = null;
                if (cell.NextNode != null)
                {
                    TableCellAdv nextCell = cell.NextNode as TableCellAdv;
                    paragraph = nextCell.GetFirstParagraph();
                }
                else if (cell.PreviousNode != null)
                {
                    TableCellAdv previousCell = cell.PreviousNode as TableCellAdv;
                    paragraph = previousCell.GetFirstParagraph();
                }
                if (paragraph == null)
                {
                    if (table.NextBlock != null)
                        paragraph = table.NextBlock is ParagraphAdv ? table.NextBlock as ParagraphAdv : (table.NextBlock as TableAdv).GetFirstParagraphInFirstCell();
                    else if (table.PreviousBlock != null)
                        paragraph = table.PreviousBlock is ParagraphAdv ? table.PreviousBlock as ParagraphAdv : (table.NextBlock as TableAdv).GetLastParagraphInLastCell();
                }
                int cellIndex = cell.CellIndex;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    TableRowAdv row = table.Rows[i];
                    if (row.Cells.Count == 1)
                    {
                        table.Rows.Remove(row);
                        i--;
                    }
                    else
                    {
                        if (cellIndex < row.Cells.Count)
                            row.Cells.RemoveAt(cellIndex);
                        else
                            row.Cells.RemoveAt(row.Cells.Count - 1);
                    }
                }
                OwnerControl.IsLayoutEnabled = true;
                if (table.Rows.Count == 0)
                {
                    table.RemoveBlock();
                    CurrentHistoryInfo.Action = Actions.Delete;
                }
                else
                    table.Layout();
                Select(paragraph, true);
                if (!OwnerControl.History.IsRedoing)
                    Relayout(true);
            }
        }
        /// <summary>
        /// Merges the selected cells.
        /// </summary>
        internal void MergeSelectedCells()
        {
            if (!CanMergeSelectedCells())
                return;
            if (!OwnerControl.History.IsRedoing)
                InitHistory(Actions.MergeCells);
            OwnerControl.IsShiftingEnabled = true;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            TableCellAdv containerCell = startPosition.Paragraph.AssociatedCell.GetContainerCell(endPosition.Paragraph.AssociatedCell);
            if (containerCell.OwnerTable.Contains(endPosition.Paragraph.AssociatedCell))
            {
                if (!containerCell.Contains(endPosition.Paragraph.AssociatedCell))
                {
                    //Start and End are in different cells.
                    TableCellAdv startCell = startPosition.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    TableCellAdv endCell = endPosition.Paragraph.AssociatedCell.GetSelectedCell(containerCell);
                    //Merges the selected cells.
                    TableCellAdv mergedCell = startCell.OwnerTable.MergeSelectedCells(this, startCell, endCell);
                    ParagraphAdv lastParagraph = mergedCell.GetLastParagraph();
                    endPosition.SetPosition(lastParagraph, false);
                }
            }
            if (!OwnerControl.History.IsRedoing)
                Relayout(false);
        }
        /// <summary>
        /// Inserts the block internal.
        /// </summary>
        /// <param name="block">The block.</param>
        internal void InsertBlockInternal(BlockAdv block)
        {
            if (!Start.IsAtParagraphStart)
            {
                if (block is ParagraphAdv)
                {
                    InsertParagraph(block as ParagraphAdv, false);
                    return;
                }
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                Start.Paragraph.SplitParagraph(0, Start.Offset);
                Select(Start.Paragraph as ParagraphAdv, true);
            }
            int index = Start.Paragraph.GetIndexInOwnerCollection();
            ((Start.Paragraph.Owner as CompositeNode).ChildNodes as BlockAdvCollection).Insert(index, block);
        }
        /// <summary>
        /// Inserts the block.
        /// </summary>
        /// <param name="block">The block.</param>
        internal void InsertBlock(BlockAdv block)
        {
            bool isRemoved = true;
            if (!IsEmpty)
                isRemoved = RemoveSelectedContent();
            if (!isRemoved)
                Select(Start);
            InsertBlockInternal(block);
            if (CurrentHistoryInfo.InsertPosition == null)
            {
                ParagraphAdv paragraph = block as ParagraphAdv;
                if (block is TableAdv)
                    paragraph = (block as TableAdv).GetFirstParagraphInFirstCell();
                CurrentHistoryInfo.InsertPosition = paragraph.GetHierarchicalIndex("0");
            }
            FireContentChanged();
        }
        /// <summary>
        /// Inserts the block before the table.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="table">The table.</param>
        internal void InsertBlock(BlockAdv block, TableAdv table)
        {
            double offset = Start.Offset;
            if (block is ParagraphAdv && offset > 0)
            {
                //Moves the inline items before selection start to the inserted paragraph.
                Start.Paragraph.MoveInlines(block as ParagraphAdv, 0, 0, offset);
                Select(Start.Paragraph, true);
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = (block as ParagraphAdv).GetHierarchicalIndex(offset.ToString());
            }
            if (offset > 0 && CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            int index = table.GetIndexInOwnerCollection();
            ((table.Owner as CompositeNode).ChildNodes as BlockAdvCollection).Insert(index, block);
            if (CurrentHistoryInfo.InsertPosition == null)
            {
                ParagraphAdv paragraph = block as ParagraphAdv;
                if (block is TableAdv)
                    paragraph = (block as TableAdv).GetFirstParagraphInFirstCell();
                CurrentHistoryInfo.InsertPosition = paragraph.GetHierarchicalIndex("0");
            }
        }
        /// <summary>
        /// Inserts the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="insertAfter">if set to <c>true</c> insert after.</param>
        internal void InsertParagraph(ParagraphAdv paragraph, bool insertAfter)
        {
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            Start.Paragraph.InsertParagraph(paragraph, Start.Offset, insertAfter);
            if (!insertAfter)
            {
                ParagraphAdv nextParagraph = paragraph.GetNextParagraph();
                if (nextParagraph != null)
                    Select(nextParagraph, true);
                else
                    Select(paragraph, true);
            }
            FireContentChanged();
        }
        /// <summary>
        /// Inserts the section.
        /// </summary>
        /// <param name="section">The section.</param>
        internal void InsertSection(SectionAdv section)
        {
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            SectionAdv ownerSection = Start.Paragraph.Section;
            ownerSection.SplitSection(section, Start.Paragraph);
        }
        /// <summary>
        /// Inserts the section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="table">The table.</param>
        internal void InsertSection(SectionAdv section, TableAdv table)
        {
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            SectionAdv ownerSection = table.Section;
            ownerSection.SplitSection(section, table);
        }
        /// <summary>
        /// Called when enter key pressed.
        /// </summary>
        internal void OnEnter()
        {
            if (Start == null)
                return;
            if (IsEmpty)
            {
                ParagraphAdv paragraph = Start.Paragraph;
                if (paragraph.IsEmpty() && !string.IsNullOrEmpty(paragraph.ParagraphFormat.ListFormat.ListId))
                {
                    OnApplyList(null, null);
                    return;
                }
            }
            InitHistory(Actions.Enter);
            bool isRemoved = true;
            if (!IsEmpty)
                isRemoved = RemoveSelectedContent();
            if (isRemoved)
            {
                OwnerControl.IsShiftingEnabled = true;
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                Start.Paragraph.SplitParagraph(Start.Offset);
                Start.SetPosition(CurrentHistoryInfo.InsertPosition);
                Select(Start.Paragraph.NextBlock as ParagraphAdv, true);
                if (CurrentHistoryInfo.EndPosition == null)
                    CurrentHistoryInfo.EndPosition = Start.GetHierarchicalIndex();
                Relayout(true);
            }
            else
            {
                CurrentHistoryInfo = null;
                Select(Start);
            }
        }
        /// <summary>
        /// Singles the backspace.
        /// </summary>
        /// <param name="isRedoing">if set to <c>true</c> is redoing.</param>
        internal void SingleBackspace(bool isRedoing)
        {
            ParagraphAdv paragraph = Start.Paragraph;
            double offset = Start.Offset;
            int indexInInline = 0;
            Inline inline = paragraph.GetInline(offset, ref indexInInline);
            if (inline is FieldEndAdv)
            {
                Inline prevInline = inline.GetPreviousValidInline();
                if (prevInline is FieldEndAdv)
                {
                    inline = (prevInline as FieldEndAdv).FieldBegin;
                    paragraph = inline.OwnerParagraph;
                    offset = paragraph.GetOffset(inline, 0);
                    //Selects the entire field.
                    End.SetPosition(paragraph, offset);
                    FireSelectionChanged(true);
                    return;
                }
                else if (prevInline != inline)
                {
                    //Updates the offset to delete next content.
                    inline = prevInline;
                    paragraph = inline.OwnerParagraph;
                    offset = paragraph.GetOffset(inline, inline.Length);
                }
            }
            if (!isRedoing)
                InitHistory(Actions.BackSpace);
            if (offset == paragraph.GetStartOffset())
            {
                if (!string.IsNullOrEmpty(paragraph.ParagraphFormat.ListFormat.ListId))
                {
                    OnApplyList(null, null);
                    return;
                }
                if (paragraph.PreviousBlock is ParagraphAdv)
                {
                    OwnerControl.IsShiftingEnabled = true;
                    ParagraphAdv previousParagraph = paragraph.PreviousBlock as ParagraphAdv;
                    if (previousParagraph.IsEmpty())
                    {
                        previousParagraph.RemoveBlock();
                        CurrentHistoryInfo.RemovedNodes.Add(previousParagraph);
                    }
                    else
                    {
                        double endOffset = previousParagraph.GetEndOffset();
                        for (int i = 0; i < paragraph.Inlines.Count; i++)
                        {
                            inline = paragraph.Inlines[i];
                            i--;
                            previousParagraph.Inlines.Add(inline);
                        }
                        paragraph.RemoveBlock();
                        Select(previousParagraph, endOffset);
                        CurrentHistoryInfo.RemovedNodes.Add(paragraph);
                    }
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                    CurrentHistoryInfo.EndPosition = CurrentHistoryInfo.InsertPosition;
                    if (!isRedoing)
                        Relayout(true);
                }
                else
                    CurrentHistoryInfo = null;
            }
            else
            {
                paragraph.RemoveAtOffset(CurrentHistoryInfo, offset - 1);
                Select(Start.Paragraph, offset - 1);
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                CurrentHistoryInfo.EndPosition = CurrentHistoryInfo.InsertPosition;
                if (!isRedoing)
                    UpdateHistory();
                FireContentChanged();
            }
        }
        /// <summary>
        /// Called when backspace key pressed.
        /// </summary>
        internal void OnBackspace()
        {
            if (Start == null)
                return;
            if (IsEmpty)
                SingleBackspace(false);
            else
            {
                InitHistory(Actions.BackSpace);
                bool skipBackSpace = DeleteSelectedContent(true);
                if (skipBackSpace)
                    CurrentHistoryInfo = null;
                else
                    Relayout(true);
            }
        }
        /// <summary>
        /// Deletes the single character.
        /// </summary>
        /// <param name="isRedoing">if set to <c>true</c> is redoing.</param>
        internal void SingleDelete(bool isRedoing)
        {
            ParagraphAdv paragraph = Start.Paragraph;
            double offset = Start.Offset;
            int indexInInline = 0;
            Inline inline = paragraph.GetInline(Start.Offset, ref indexInInline);
            if (inline != null)
            {
                Inline nextRenderedInline = inline.GetNextRenderedInline(indexInInline);
                if (nextRenderedInline is FieldBeginAdv)
                {
                    //Selects the entire field.
                    inline = (nextRenderedInline as FieldBeginAdv).FieldEnd;
                    paragraph = inline.OwnerParagraph;
                    offset = paragraph.GetOffset(inline, 1);
                    End.SetPosition(paragraph, offset);
                    FireSelectionChanged(true);
                    return;
                }
                else if (inline != nextRenderedInline)
                {
                    //Updates the offset to delete next content.
                    inline = nextRenderedInline;
                    paragraph= inline.OwnerParagraph;
                    offset = paragraph.GetOffset(inline, 0);
                    if (inline is FieldEndAdv)
                        offset++;
                }
            }
            if (offset == paragraph.GetLength())
            {
                if (paragraph.IsInsideTable && paragraph.NextBlock == null)
                    return;
                ParagraphAdv previousParagraph = null, newParagraph = null;
                ParagraphAdv nextParagraph = paragraph.GetNextParagraph();
                if (nextParagraph == null)
                {
                    if (offset > 0)
                        return;
                    else
                    {
                        previousParagraph = paragraph.PreviousBlock as ParagraphAdv;
                        if (paragraph.PreviousBlock is TableAdv)
                            return;
                        if (previousParagraph == null)
                        {
                            //Adds an empty paragraph, to ensure minimal content.
                            newParagraph = new ParagraphAdv();
                            (paragraph.Owner as CompositeNode).ChildNodes.Add(newParagraph);
                        }
                    }
                }
                //Delete executed at paragraph end.
                if (!isRedoing)
                    InitHistory(Actions.Delete);
                EditPosition = Start.GetHierarchicalIndex();
                if (CurrentHistoryInfo.InsertPosition == null)
                {
                    CurrentHistoryInfo.InsertPosition = EditPosition;
                    CurrentHistoryInfo.EndPosition = EditPosition;
                }
                OwnerControl.IsShiftingEnabled = true;
                if (paragraph.IsEmpty())
                {
                    paragraph.RemoveBlock();
                    CurrentHistoryInfo.RemovedNodes.Add(paragraph);
                    if (nextParagraph == null)
                    {
                        if (previousParagraph == null)
                        {
                            Select(newParagraph, true);
                            double paraEndOffset = newParagraph.GetLength() + 1;
                            CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                            CurrentHistoryInfo.EndPosition = newParagraph.GetHierarchicalIndex(paraEndOffset.ToString());
                        }
                        else
                        {
                            Select(previousParagraph, false);
                            CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                            CurrentHistoryInfo.EndPosition = CurrentHistoryInfo.InsertPosition;
                        }
                    }
                    else
                        Select(nextParagraph, true);
                }
                else
                {
                    ParagraphAdv currentParagraph = paragraph.SplitParagraph(0, Start.Offset);
                    //Removes the current paragraph.
                    paragraph.RemoveBlock();
                    CurrentHistoryInfo.RemovedNodes.Add(paragraph);
                    currentParagraph.DeleteParagraphMark(this, 0);
                    Start.SetPosition(EditPosition);
                    Select(Start);
                }
                if (!isRedoing)
                    Relayout(true);
            }
            else
            {
                if (!isRedoing)
                    InitHistory(Actions.Delete);
                if (CurrentHistoryInfo.InsertPosition == null)
                {
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                    CurrentHistoryInfo.EndPosition = CurrentHistoryInfo.InsertPosition;
                }
                paragraph.RemoveAtOffset(CurrentHistoryInfo, Start.Offset);
                Select(Start.Paragraph, Start.Offset);
                if (!isRedoing)
                    UpdateHistory();
                FireContentChanged();
            }
        }
        /// <summary>
        /// Called when delete key pressed.
        /// </summary>
        internal void OnDelete()
        {
            if (Start == null)
                return;
            if (IsEmpty)
                SingleDelete(false);
            else
            {
                InitHistory(Actions.Delete);
                DeleteSelectedContent(false);
                Relayout(true);
            }
        }
        /// <summary>
        /// Applies bold style.
        /// </summary>
        internal void OnBold()
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.Bold);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.BoldProperty, !Start.Paragraph.CharacterFormat.Bold);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.Bold);
            UpdateCharacterFormat(DPCharacterFormat.BoldProperty, DependencyProperty.UnsetValue);
            Relayout(false);
        }
        /// <summary>
        /// Applies italic style.
        /// </summary>
        internal void OnItalic()
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.Italic);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.ItalicProperty, !Start.Paragraph.CharacterFormat.Italic);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.Italic);
            UpdateCharacterFormat(DPCharacterFormat.ItalicProperty, DependencyProperty.UnsetValue);
            Relayout(false);
        }
        /// <summary>
        /// Called when font color changed.
        /// </summary>
        /// <param name="fontColor">Color of the font.</param>
        internal void OnFontColor(Color fontColor)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.FontColor);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.FontColorProperty, fontColor);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.FontColor);
            UpdateCharacterFormat(DPCharacterFormat.FontColorProperty, fontColor);
            Relayout(false);
        }
        /// <summary>
        /// Called when font family changed.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        internal void OnFontFamily(FontFamily fontFamily)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.FontFamily);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.FontFamilyProperty, fontFamily);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.FontFamily);
            UpdateCharacterFormat(DPCharacterFormat.FontFamilyProperty, fontFamily);
            Relayout(false);
        }
        /// <summary>
        /// Called when font size changed.
        /// </summary>
        /// <param name="fontSize">Size of the font.</param>
        internal void OnFontSize(double fontSize)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.FontSize);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.FontSizeProperty, fontSize);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.FontSize);
            UpdateCharacterFormat(DPCharacterFormat.FontSizeProperty, fontSize);
            Relayout(false);
        }
        /// <summary>
        /// Called when highlightColor changed.
        /// </summary>
        /// <param name="highlightColor">The highlightColor.</param>
        internal void OnHighlightColor(HighlightColor highlightColor)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.HighlightColor);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.HighlightColorProperty, highlightColor);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.HighlightColor);
            UpdateCharacterFormat(DPCharacterFormat.HighlightColorProperty, highlightColor);
            Relayout(false);
        }
        /// <summary>
        /// Called when baselineAlignment changed.
        /// </summary>
        /// <param name="baselineAlignment">The baselineAlignment.</param>
        internal void OnBaselineAlignment(BaselineAlignment baselineAlignment)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.BaselineAlignment);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.BaselineAlignmentProperty, baselineAlignment);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.BaselineAlignment);
            UpdateCharacterFormat(DPCharacterFormat.BaselineAlignmentProperty, baselineAlignment);
            Relayout(false);
        }
        /// <summary>
        /// Called when strikeThrough changed.
        /// </summary>
        /// <param name="strikeThrough">The strikeThrough.</param>
        internal void OnStrikeThrough(StrikeThrough strikeThrough)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.StrikeThrough);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.StrikeThroughProperty, strikeThrough);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.StrikeThrough);
            UpdateCharacterFormat(DPCharacterFormat.StrikeThroughProperty, strikeThrough);
            Relayout(false);
        }
        /// <summary>
        /// Called when underline changed.
        /// </summary>
        /// <param name="underline">The underline.</param>
        internal void OnUnderline(Underline underline)
        {
            if (IsEmpty)
            {
                if (Start.Offset == Start.Paragraph.GetLength())
                {
                    InitHistory(Actions.Underline);
                    Start.Paragraph.CharacterFormat.ApplyPropertyValue(CurrentHistoryInfo, DPCharacterFormat.UnderlineProperty, underline);
                    UpdateHistory();
                    FireContentChanged();
                }
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.Underline);
            UpdateCharacterFormat(DPCharacterFormat.UnderlineProperty, underline);
            Relayout(false);
        }
        /// <summary>
        /// Called when after spacing changed.
        /// </summary>
        /// <param name="afterSpacing">The after spacing.</param>
        internal void OnAfterSpacing(double afterSpacing)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.AfterSpacing);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.AfterSpacingProperty, afterSpacing);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.AfterSpacing);
            UpdateParagraphFormat(RichTextBoxAdv.ParagraphFormat.AfterSpacingProperty, afterSpacing);
            Relayout(false);
        }
        /// <summary>
        /// Called when before spacing changed.
        /// </summary>
        /// <param name="beforeSpacing">The before spacing.</param>
        internal void OnBeforeSpacing(double beforeSpacing)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.BeforeSpacing);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.BeforeSpacingProperty, beforeSpacing);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.BeforeSpacing);
            UpdateParagraphFormat(DPParagraphFormat.BeforeSpacingProperty, beforeSpacing);
            Relayout(false);
        }
        /// <summary>
        /// Called when left indent changed.
        /// </summary>
        /// <param name="leftIndent">The left indent.</param>
        internal void OnLeftIndent(double leftIndent)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.LeftIndent);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.LeftIndentProperty, leftIndent);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.LeftIndent);
            UpdateParagraphFormat(DPParagraphFormat.LeftIndentProperty, leftIndent);
            Relayout(false);
        }
        /// <summary>
        /// Called when right indent changed.
        /// </summary>
        /// <param name="rightIndent">The right indent.</param>
        internal void OnRightIndent(double rightIndent)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.RightIndent);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.RightIndentProperty, rightIndent);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.RightIndent);
            UpdateParagraphFormat(DPParagraphFormat.RightIndentProperty, rightIndent);
            Relayout(false);
        }
        /// <summary>
        /// Called when first line indent changed.
        /// </summary>
        /// <param name="firstLineIndent">The first line indent.</param>
        internal void OnFirstLineIndent(double firstLineIndent)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.FirstLineIndent);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.FirstLineIndentProperty, firstLineIndent);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.FirstLineIndent);
            UpdateParagraphFormat(DPParagraphFormat.FirstLineIndentProperty, firstLineIndent);
            Relayout(false);
        }
        /// <summary>
        /// Called when line spacing type changed.
        /// </summary>
        /// <param name="lineSpacingType">Type of the line spacing.</param>
        internal void OnLineSpacingType(LineSpacingType lineSpacingType)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.LineSpacingType);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.LineSpacingTypeProperty, lineSpacingType);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.LineSpacingType);
            UpdateParagraphFormat(DPParagraphFormat.LineSpacingTypeProperty, lineSpacingType);
            Relayout(false);
        }
        /// <summary>
        /// Called when line spacing changed.
        /// </summary>
        /// <param name="lineSpacing">The line spacing.</param>
        internal void OnLineSpacing(double lineSpacing)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.LineSpacing);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.LineSpacingProperty, lineSpacing);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.LineSpacing);
            UpdateParagraphFormat(DPParagraphFormat.LineSpacingProperty, lineSpacing);
            Relayout(false);
        }
        /// <summary>
        /// Called when text alignment changed.
        /// </summary>
        /// <param name="textAlignment">The text alignment.</param>
        internal void OnTextAlignment(TextAlignment textAlignment)
        {
            if (IsEmpty)
            {
                InitHistory(Actions.TextAlignment);
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.TextAlignmentProperty, textAlignment);
                UpdateHistory();
                FireContentChanged();
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.TextAlignment);
            UpdateParagraphFormat(DPParagraphFormat.TextAlignmentProperty, textAlignment);
            Relayout(false);
        }
        /// <summary>
        /// Called when apply list.
        /// </summary>
        internal void OnApplyList(ListAdv list, ListLevelAdv listLevel)
        {
            ListFormat listFormat = new ListFormat();
            if (list != null)
            {
                listFormat.ListId = list.Name;
                listFormat.ListLevelNumber = listLevel.OwnerAbstractList.Levels.IndexOf(listLevel);
            }
            if (IsEmpty)
            {
                InitHistory(Actions.ListFormat);
                OwnerControl.IsShiftingEnabled = true;
                Start.Paragraph.ParagraphFormat.ApplyPropertyValue(CurrentHistoryInfo, DPParagraphFormat.ListFormatProperty, listFormat);
                Relayout(true);
                return;
            }
            //Iterate and update formattings.
            InitHistory(Actions.ListFormat);
            UpdateParagraphFormat(DPParagraphFormat.ListFormatProperty, listFormat);
            Relayout(false);
        }
        /// <summary>
        /// Called when [page margin changed].
        /// </summary>
        /// <param name="pageMargin">The page margin.</param>
        internal void OnPageMarginChanged(Thickness pageMargin)
        {
            InitHistory(Actions.PageMargin);
            UpdateSectionFormat(DPSectionFormat.PageMarginProperty, pageMargin);
            OwnerControl.Document.LayoutItems();
        }
        /// <summary>
        /// Called when [page size changed].
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        internal void OnPageSizeChanged(Size pageSize)
        {
            InitHistory(Actions.PageSize);
            UpdateSectionFormat(DPSectionFormat.PageSizeProperty, pageSize);
            OwnerControl.Document.LayoutItems();
        }
        #endregion

        #region Clipboard
        /// <summary>
        /// Copies this instance.
        /// </summary>
        internal void Copy()
        {
            if (IsEmpty)
                return;
            CopySelectedContent(false);
        }
        /// <summary>
        /// Cuts this instance.
        /// </summary>
        internal void Cut()
        {
            if (IsEmpty)
                return;
            InitHistory(Actions.Cut);
            if (CurrentHistoryInfo.InsertPosition == null)
                CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
            CopySelectedContent(true);
            Relayout(true);
        }
        /// <summary>
        /// Pastes the clipboard contents.
        /// </summary>
#if WPF
        internal void Paste()
        {
            string text = "";
            try
            {
                text = Clipboard.GetText();
            }
            catch (Exception ex)
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open.
            }
            if (text != null && text != "")
            {
                WordDocument = new DocIO.DLS.WordDocument();
                WordDocument.OpenText(text);
                text = "";
                if (WordDocument != null)
                    PasteContents();
            }
        }
#else
        internal void Paste()
        {
            DataPackageView dataView = null;
            try
            {
                dataView = Clipboard.GetContent();
            }
            catch
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open.
            }
            if (dataView != null)
            {
                if (dataView.Contains(StandardDataFormats.Bitmap))
                {
                    //Gets image from Clipboard.
                    Task<RandomAccessStreamReference> streamReferenceTask = dataView.GetBitmapAsync().AsTask();
                    streamReferenceTask.Wait();
                    Task<IRandomAccessStreamWithContentType> imageTask = streamReferenceTask.Result.OpenReadAsync().AsTask();
                    imageTask.Wait();
                    PasteImage(imageTask.Result);
                }
                else if (dataView.Contains(StandardDataFormats.Rtf))
                {
                    WordDocument = new DocIO.DLS.WordDocument();
                    Task<string> rtfTask = dataView.GetRtfAsync().AsTask();
                    rtfTask.Wait();
                    WordDocument.OpenRtf(rtfTask.Result);
                }
                else if (dataView.Contains(StandardDataFormats.Text))
                {
                    WordDocument = new DocIO.DLS.WordDocument();
                    Task<string> textTask = dataView.GetTextAsync().AsTask();
                    textTask.Wait();
                    WordDocument.OpenText(textTask.Result);
                }
                if (WordDocument != null)
                    PasteContents();
            }
        }
        /// <summary>
        /// Pastes the image.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        private void PasteImage(IRandomAccessStream imageStream)
        {
            bool isRemoved = true;
            InitHistory(Actions.Paste);
            if (!IsEmpty)
                isRemoved = RemoveSelectedContent();
            if (isRemoved)
            {
                ImageContainerAdv imageContainerAdv = new ImageContainerAdv();
                Stream stream = imageStream.AsStreamForRead();
                imageContainerAdv.ImageBytes = stream.GetBytes();
                imageStream.Seek(0);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(imageStream.CloneStream());
                imageContainerAdv.ImageSource = bitmapImage;
                FitImageToPage(imageContainerAdv);
                InsertInline(imageContainerAdv);
                Relayout(OwnerControl.Selection.IsEmpty);
            }
            else
                CurrentHistoryInfo = null;
        }
#endif
        /// <summary>
        /// Pastes the contents.
        /// </summary>
        private void PasteContents()
        {
            bool isRemoved = true;
            InitHistory(Actions.Paste);
            if (!IsEmpty)
                isRemoved = RemoveSelectedContent();
            if (isRemoved)
            {
                OwnerControl.IsShiftingEnabled = true;
                if (CurrentHistoryInfo.InsertPosition == null)
                    CurrentHistoryInfo.InsertPosition = Start.GetHierarchicalIndex();
                DocxImporting.ParseList(WordDocument, OwnerControl.Document);
                for (int i = 0; i < WordDocument.Sections.Count; i++)
                {
                    DocIO.DLS.WSection wSection = WordDocument.Sections[i];
                    PasteSectionChilds(wSection);
                    if (wSection != WordDocument.LastSection)
                        SplitSection(wSection);
                }
                CurrentHistoryInfo.EndPosition = Start.GetHierarchicalIndex();
                ownerControl.IsPastingContent = false;
                Relayout(OwnerControl.Selection.IsEmpty);
            }
            else
                CurrentHistoryInfo = null;
            WordDocument.Close();
            WordDocument = null;
        }
        /// <summary>
        /// Splits the section.
        /// </summary>
        /// <param name="wSection">The w section.</param>
        private void SplitSection(DocIO.DLS.WSection wSection)
        {
            SectionAdv curSection = Start.Paragraph.Section;
            int secIndex = curSection.Document.Sections.IndexOf(curSection);
            SectionAdv newSection = new SectionAdv();
            DocxImporting.ParseSectionFormat(wSection, newSection);
            DocxImporting.ParseHeaderFooters(wSection.HeadersFooters, newSection.HeaderFooters);
            curSection.Document.Sections.Insert(secIndex, newSection);
            for (int i = 0; i < curSection.Blocks.Count; i++)
            {
                BlockAdv block = curSection.Blocks[i];
                if (block == Start.Paragraph)
                    break;
                newSection.Blocks.Add(block);
                i--;
            }
        }
        /// <summary>
        /// Pastes the section childs.
        /// </summary>
        /// <param name="wSection">The wsection.</param>
        private void PasteSectionChilds(DocIO.DLS.WSection wSection)
        {
            for (int i = 0; i < wSection.Body.ChildEntities.Count; i++)
            {
                DocIO.DLS.TextBodyItem bodyItem = wSection.Body.ChildEntities[i] as DocIO.DLS.TextBodyItem;
                BlockAdv block = null;
                if (bodyItem is DocIO.DLS.WParagraph)
                    block = DocxImporting.ParseParagraph(bodyItem as DocIO.DLS.WParagraph);
                else if (bodyItem is DocIO.DLS.WTable)
                    block = DocxImporting.ParseTable(bodyItem as DocIO.DLS.WTable);
                if (wSection.NextSibling == null && bodyItem.NextSibling == null && block is ParagraphAdv)
                {
                    for (int j = 0; j < (block as ParagraphAdv).Inlines.Count; j++)
                    {
                        InsertInlineInternal((block as ParagraphAdv).Inlines[0]);
                        j--;
                    }
                }
                else if (block is BlockAdv)
                {
                    if (block is TableAdv && Start.Paragraph.IsInsideTable)
                    {
                        //Handled to resize table based on parent cell width.
                        double clientWidth = Start.Paragraph.GetContainerWidth();
                        (block as TableAdv).FitCellsToClientArea(clientWidth);
                    }
                    InsertBlock(block);
                }
                if (i == 0 && !ownerControl.IsPastingContent)
                    ownerControl.IsPastingContent = true;
            }
        }
        /// <summary>
        /// Inits the word document.
        /// </summary>
        private void InitWordDocument()
        {
            WordDocument = new Syncfusion.DocIO.DLS.WordDocument();
            WordDocument.DefCharFormat = new Syncfusion.DocIO.DLS.WCharacterFormat(WordDocument);
            DocxExporting.SerializeCharacterFormat(OwnerControl.Document.CharacterFormat, WordDocument.DefCharFormat);
            WordDocument.DefParaFormat = new Syncfusion.DocIO.DLS.WParagraphFormat(WordDocument);
            DocxExporting.SerializeParagraphFormat(OwnerControl.Document.ParagraphFormat, WordDocument.DefParaFormat);
            DocIO.DLS.IWSection wSection = WordDocument.AddSection();
        }
        /// <summary>
        /// Copies the section format.
        /// </summary>
        /// <param name="sectionAdv">The section adv.</param>
        internal void CopySectionFormat(SectionAdv sectionAdv)
        {
            DocxExporting.SerializeSectionFormat(sectionAdv, WordDocument.LastSection);
            if (sectionAdv.HeaderFooters != null)
                DocxExporting.SerializeHeaderFooters(sectionAdv.HeaderFooters, WordDocument.LastSection.HeadersFooters);
            WordDocument.AddSection();
        }
        /// <summary>
        /// Copies to clipboard.
        /// </summary>
        /// <param name="isCut">if set to <c>true</c> is cut.</param>
        /// <param name="imageContainerAdv">The image container adv.</param>
        private void CopyToClipboard(bool isCut, ImageContainerAdv imageContainerAdv)
        {
            string text = WordDocument.GetText();
#if WPF
            Clipboard.SetText(text);
#else
            DataPackage data = new DataPackage();
            data.RequestedOperation = isCut ? DataPackageOperation.Move : DataPackageOperation.Copy;
            //Sets the text content to clipboard.
            data.SetText(text);
            if (imageContainerAdv != null && imageContainerAdv.ImageBytes != null)
            {
                //Copy image to Clipboard, if selected item is image.
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                Stream stream = randomAccessStream.AsStreamForWrite();
                stream.Write(imageContainerAdv.ImageBytes, 0, imageContainerAdv.ImageBytes.Length);
                stream.Flush();
                stream.Position = 0;
                data.SetBitmap(RandomAccessStreamReference.CreateFromStream(randomAccessStream));
            }
            else
            {
                text = WordDocument.GetRtfText();
                //Sets the rich text format content to clipboard.
                data.SetRtf(text);
            }
            try
            {
                Clipboard.SetContent(data);
                Clipboard.Flush();
            }
            catch
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open.
            }
#endif
            WordDocument.Close();
            WordDocument = null;
        }
        /// <summary>
        /// Copies the content of the selected.
        /// </summary>
        /// <param name="isCut">if set to <c>true</c>is cut.</param>
        internal void CopySelectedContent(bool isCut)
        {
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            if (isCut)
            {
                OwnerControl.IsShiftingEnabled = true;
                EditPosition = startPosition.GetHierarchicalIndex();
            }
            InitWordDocument();
            ImageContainerAdv imageContainerAdv = null;
            if (startPosition.Paragraph == endPosition.Paragraph && startPosition.Offset + 1 == endPosition.Offset)
            {
                //Gets selected image and copy image to clipboard.
                int index = 0;
                Inline inline = startPosition.Paragraph.GetInline(endPosition.Offset, ref index);
                imageContainerAdv = inline as ImageContainerAdv;
            }
            startPosition.Paragraph.DeleteSelectedContent(this, startPosition, endPosition, (byte)(isCut ? 3 : 4));
            CopyToClipboard(isCut, imageContainerAdv);
            if (isCut)
            {
                TextPosition textPosition = new TextPosition(OwnerControl);
                textPosition.SetPosition(EditPosition);
                Select(textPosition);
            }
        }
        #endregion

        #region Highlight Implementation
        /// <summary>
        /// Updates the physical position.
        /// </summary>
        internal void UpdatePhysicalPosition()
        {
            if (Start != null)
                Start.UpdatePhysicalPosition();
            if (End != null)
                End.UpdatePhysicalPosition();
        }
        /// <summary>
        /// Highlights the selection.
        /// </summary>
        /// <param name="isSelectionChanged">if set to <c>true</c> [is selection changed].</param>
        internal void HighlightSelection(bool isSelectionChanged)
        {
            OwnerControl.Viewer.HideImageResizer();
            if (IsEmpty)
            {
#if !WPF
                if (OwnerControl.LayoutType == LayoutType.Block)
                {
                    OwnerControl.IsPopItemVisible = false;
                    OwnerControl.CopyIcon.Visibility = Visibility.Collapsed;
                }
#endif
                OwnerControl.Viewer.UpdateCaretPosition();
#if !WPF
                if (Start.Paragraph.IsInsideTable)
                    OwnerControl.LoadTableRadialMenuIcon();
                else
                    OwnerControl.LoadDefaultRadialMenu();
#endif
            }
            else
            {
                if (IsForward)
                    Start.Paragraph.HighlightSelectedContent(this, Start, End);
                else
                    End.Paragraph.HighlightSelectedContent(this, End, Start);
                OwnerControl.Viewer.UpdateTouchMarkPosition();
#if !WPF
                if (isSelectionChanged)
                    OwnerControl.LoadTextRadialMenu();
#endif
            }
            if (isSelectionChanged)
                OwnerControl.Viewer.ScrollToPosition(Start, End);
        }
        /// <summary>
        /// Creates the highlight border.
        /// </summary>
        /// <param name="lineWidget">The line widget.</param>
        /// <param name="width">The width.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal void CreateHighlightBorder(LineWidget lineWidget, double width, double left, double top)
        {
            if (width < 0)
                width = 0;
            if (lineWidget.SelectionHighlight == null)
                lineWidget.SelectionHighlight = new Border();
            lineWidget.SelectionHighlight.Opacity = OwnerControl.SelectionOpacity;
            lineWidget.SelectionHighlight.Background = OwnerControl.SelectionBrush;
            lineWidget.SelectionHighlight.Width = width;
            lineWidget.SelectionHighlight.Height = lineWidget.Height;
            SelectedLines.Add(lineWidget);
            PageAdv page = lineWidget.ParagraphWidget.GetPage();
            if (OwnerControl.Viewer is FlowLayoutViewer || (OwnerControl.Viewer as PageLayoutViewer).VisiblePages.Contains(page))
            {
                if (lineWidget.SelectionHighlight.Parent is Panel)
                    (lineWidget.SelectionHighlight.Parent as Panel).Children.Remove(lineWidget.SelectionHighlight);
                page.ForegroundContainer.Children.Add(lineWidget.SelectionHighlight);
            }
            Canvas.SetLeft(lineWidget.SelectionHighlight, left);
            Canvas.SetTop(lineWidget.SelectionHighlight, top);
        }
        /// <summary>
        /// Creates the highlight border.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        internal void CreateHighlightBorder(TableCellWidget cellWidget)
        {
            if (cellWidget.SelectionHighlight == null)
                cellWidget.SelectionHighlight = new Border();
            cellWidget.SelectionHighlight.Opacity = OwnerControl.SelectionOpacity;
            cellWidget.SelectionHighlight.Background = OwnerControl.SelectionBrush;
            cellWidget.SelectionHighlight.Width = cellWidget.Width + cellWidget.Margin.Left + cellWidget.Margin.Right;
            cellWidget.SelectionHighlight.Height = cellWidget.Height + cellWidget.Margin.Top + cellWidget.Margin.Bottom;
            SelectedCells.Add(cellWidget);
            PageAdv page = cellWidget.GetPage();
            if (OwnerControl.Viewer is FlowLayoutViewer || (OwnerControl.Viewer as PageLayoutViewer).VisiblePages.Contains(page))
            {
                if (cellWidget.SelectionHighlight.Parent is Panel)
                    (cellWidget.SelectionHighlight.Parent as Panel).Children.Remove(cellWidget.SelectionHighlight);
                page.ForegroundContainer.Children.Add(cellWidget.SelectionHighlight);
            }
            Canvas.SetLeft(cellWidget.SelectionHighlight, cellWidget.Location.X - cellWidget.Margin.Left);
            Canvas.SetTop(cellWidget.SelectionHighlight, cellWidget.Location.Y - cellWidget.Margin.Top);
        }
        /// <summary>
        /// Clears the selection highlight.
        /// </summary>
        private void ClearSelectionHighlight()
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                for (int i = 0; i < SelectedLines.Count; i++)
                {
                    LineWidget lineWidget = SelectedLines[i];
                    lineWidget.ClearSelectionHighlight();
                    SelectedLines.Remove(lineWidget);
                    i--;
                }
                for (int i = 0; i < SelectedCells.Count; i++)
                {
                    TableCellWidget cellWidget = SelectedCells[i];
                    cellWidget.ClearSelectionHighlight();
                    SelectedCells.Remove(cellWidget);
                    i--;
                }
#if !WPF
            });
#endif
        }
        #endregion

        internal void ToggleUnderline()
        {
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }

            int indexInInline = 0;
            Inline inline = startPosition.Paragraph.GetInline(Start.Offset, ref indexInInline);
            if (inline != null && inline.Length == indexInInline)
                inline = inline.NextNode as Inline;

            ParagraphAdv paragraph = startPosition.Paragraph;
            Underline underline = Underline.None;
            if (inline != null)
                underline = inline.CharacterFormat.Underline;
            else if(paragraph != null)
                underline = paragraph.CharacterFormat.Underline;
            
            if (underline == Underline.None)
                OnUnderline(Underline.Single);
            else
                OnUnderline(Underline.None);
        }

        #region Formats Retrieval
        /// <summary>
        /// Retrieves the current format properties.
        /// </summary>
        internal void RetrieveCurrentFormatProperties()
        {
            IsRetrieveFormatting = true;
            TextPosition startPosition = Start;
            TextPosition endPosition = End;
            if (!IsForward)
            {
                startPosition = End;
                endPosition = Start;
            }
            RetrieveCharacterFormat(startPosition, endPosition);
            RetrieveParagraphFormat(startPosition, endPosition);
            RetrieveSectionFormat(startPosition, endPosition);
            RetrieveTableFormat(startPosition, endPosition);
            RetrieveCellFormat(startPosition, endPosition);
            IsRetrieveFormatting = false;
        }
        /// <summary>
        /// Retrieves the character format.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void RetrieveCharacterFormat(TextPosition start, TextPosition end)
        {
            if (!start.Paragraph.IsEmpty())
                start.Paragraph.GetCharacterFormatForSelection(this, start, end);
            else
                CharacterFormat.CopyFormat(start.Paragraph.CharacterFormat);
        }
        /// <summary>
        /// Retrieves the paragraph format.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void RetrieveParagraphFormat(TextPosition start, TextPosition end)
        {
            if (!start.Paragraph.IsEmpty())
                start.Paragraph.GetParagraphFormatForSelection(this, start, end);
            else
                ParagraphFormat.CopyFormat(start.Paragraph.ParagraphFormat);
        }
        /// <summary>
        /// Retrieves the section format.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void RetrieveSectionFormat(TextPosition start, TextPosition end)
        {
            SectionFormat.CopyFormat(start.Paragraph.Section.SectionFormat);
            int startSectionIndex = ownerControl.Document.Sections.IndexOf(start.Paragraph.Section);
            int endSectionIndex = ownerControl.Document.Sections.IndexOf(end.Paragraph.Section);
            for (int i = startSectionIndex + 1; i <= endSectionIndex; i++)
                SectionFormat.CombineFormat(ownerControl.Document.Sections[i].SectionFormat);
        }
        /// <summary>
        /// Retrieves the table format.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void RetrieveTableFormat(TextPosition start, TextPosition end)
        {
            if (start.Paragraph.IsInsideTable && end.Paragraph.IsInsideTable && (start.Paragraph.Owner as TableCellAdv).OwnerTable == (end.Paragraph.Owner as TableCellAdv).OwnerTable)
                TableFormat.CopyFormat(start.Paragraph.AssociatedCell.OwnerTable.TableFormat);
        }
        /// <summary>
        /// Retrieves the cell format.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void RetrieveCellFormat(TextPosition start, TextPosition end)
        {
            if (start.Paragraph.IsInsideTable && end.Paragraph.IsInsideTable && (start.Paragraph.Owner as TableCellAdv).OwnerTable == (end.Paragraph.Owner as TableCellAdv).OwnerTable)
            {
                CellFormat.CopyFormat(start.Paragraph.AssociatedCell.CellFormat);
                start.Paragraph.AssociatedCell.OwnerTable.GetCellFormat(this, start, end);
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            ClearSelection();
            ownerControl = null;
            if (characterFormat != null)
            {
                characterFormat.Dispose();
                characterFormat = null;
            }
            if (paragraphFormat != null)
            {
                paragraphFormat.Dispose();
                paragraphFormat = null;
            }
            if (sectionFormat != null)
            {
                sectionFormat.Dispose();
                sectionFormat = null;
            }
            if (tableFormat != null)
            {
                tableFormat.Dispose();
                tableFormat = null;
            }
            if (cellFormat != null)
            {
                cellFormat.Dispose();
                cellFormat = null;
            }
        }
        #endregion
    }
    public class SelectionCharacterFormat : INotifyPropertyChanged
    {
        #region fields
        // Declaring owner selection
        private SelectionAdv selection;
        //Declaring character formats
        private bool? bold;
        private bool? italic;
        private Underline? underline;
        private StrikeThrough? strikethrough;
        private BaselineAlignment? baselineAlignment;
        private HighlightColor? highlightColor;
        private double fontSize;
        private FontFamily fontFamily;
        private Color? fontColor;       
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the font size.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                NotifyPropertyChanged("FontSize");
            }
        }
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value;
                NotifyPropertyChanged("FontFamily");
            }
        }
        /// <summary>
        /// Gets or sets the font color of text.
        /// </summary>
        /// <value>
        /// The color of the font.
        /// </value>
        public Color? FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
               fontColor = value;
               NotifyPropertyChanged("FontColor");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CharacterFormat"/> is bold.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bold; otherwise, <c>false</c>.
        /// </value>
        public bool? Bold
        {
            get
            {
                return bold;
            }
            set
            {
                bold = value;
                NotifyPropertyChanged("Bold");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CharacterFormat"/> is italic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if italic; otherwise, <c>false</c>.
        /// </value>
        public bool? Italic
        {
            get
            {
                return italic;
            }
            set
            {
                italic = value;
                NotifyPropertyChanged("Italic");
            }
        }
        /// <summary>
        /// Gets or Sets the strike through.
        /// </summary>
        /// <value>
        /// The strike through.
        /// </value>
        public StrikeThrough? StrikeThrough
        {
            get
            {
                return strikethrough;
            }
            set
            {
                strikethrough = value;
                NotifyPropertyChanged("StrikeThrough");
            }
        }
        /// <summary>
        /// Gets or Sets the baseline.
        /// </summary>
        /// <value>
        /// The baseline alignment.
        /// </value>
        public BaselineAlignment? BaselineAlignment
        {
            get
            {
                return baselineAlignment;
            }
            set
            {
                baselineAlignment = value;
                NotifyPropertyChanged("BaselineAlignment");
            }
        }
        /// <summary>
        /// Gets or Sets the underline style.
        /// </summary>
        /// <value>
        /// The underline.
        /// </value>
        public Underline? Underline
        {
            get
            {
                return underline;
            }
            set
            {
                underline = value;
                NotifyPropertyChanged("Underline");
            }
        }
        /// <summary>
        /// Gets or Sets the highlight color.
        /// </summary>
        /// <value>
        /// The color of the highlight.
        /// </value>
        public HighlightColor? HighlightColor
        {
            get
            {
                return highlightColor;
            }
            set
            {
                highlightColor = value;
                NotifyPropertyChanged("HighlightColor");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionCharacterFormat"/> class.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal SelectionCharacterFormat(SelectionAdv selection)
        {
            this.selection = selection;
        }
        #endregion

        #region Event
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementations
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!selection.IsRetrieveFormatting)
            {
                switch (propertyName)
                {
                    case "Bold": selection.OnBold();
                        break;
                    case "Italic": selection.OnItalic();
                        break;
                    case "FontSize": selection.OnFontSize(fontSize);
                        break;
                    case "FontFamily": selection.OnFontFamily(fontFamily);
                        break;
                    case "StrikeThrough": selection.OnStrikeThrough(strikethrough.Value);
                        break;
                    case "BaselineAlignment": selection.OnBaselineAlignment(baselineAlignment.Value);
                        break;
                    case "HighlightColor": selection.OnHighlightColor(highlightColor.Value);
                        break;
                    case "Underline": selection.ToggleUnderline();
                        break;
                    case "FontColor": selection.OnFontColor(fontColor.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(CharacterFormat format)
        {
            FontSize = format.FontSize;
            FontFamily = format.FontFamily;
            Bold = format.Bold;
            Italic = format.Italic;
            BaselineAlignment = format.BaselineAlignment;
            Underline = format.Underline;
            FontColor = format.FontColor;
            HighlightColor = format.HighlightColor;
            StrikeThrough = format.StrikeThrough;
        }
        /// <summary>
        /// Combines the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CombineFormat(CharacterFormat format)
        {
            if (Bold.HasValue && !format.Bold)
                Bold = null;
            if (Italic.HasValue & !format.Italic)
                Italic = null;
            if (FontSize != format.FontSize)
                FontSize = 0;
            if (FontFamily != null && FontFamily.Source != format.FontFamily.Source)
                FontFamily = null;
            if (HighlightColor != format.HighlightColor)
                HighlightColor = null;
            if (BaselineAlignment != format.BaselineAlignment)
                BaselineAlignment = null;
            if (FontColor != format.FontColor)
                FontColor = null;
            if (Underline != format.Underline)
                Underline = null;
            if (StrikeThrough != format.StrikeThrough)
                StrikeThrough = null;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            selection = null;
            bold = null;
            italic = null;
            fontFamily = null;
            fontColor = null;
        }
        #endregion
    }
    public class SelectionParagraphFormat : INotifyPropertyChanged
    {
        #region fields
        // Declaring owner selection
        private SelectionAdv selection;
        //Declaring character formats
        private double leftIndent;
        private double rightIndent;
        private double beforeSpacing;
        private double afterSpacing;
        private TextAlignment? textAlignment;
        private double firstLineIndent;
        private double lineSpacing;
        private LineSpacingType? lineSpacingType;      
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the left indent.
        /// </summary>
        /// <value>
        /// The left indent.
        /// </value>
        public double LeftIndent
        {
            get
            {
                return leftIndent;
            }
            set
            {
                leftIndent = value;
                NotifyPropertyChanged("LeftIndent");
            }
        }
        /// <summary>
        /// Gets or Sets the right indent.
        /// </summary>
        /// <value>
        /// The right indent.
        /// </value>
        public double RightIndent
        {
            get
            {
                return rightIndent;
            }
            set
            {
                rightIndent = value;
                NotifyPropertyChanged("RightIndent");
            }
        }
        /// <summary>
        /// Gets or Sets the first line indent.
        /// </summary>
        /// <value>
        /// The first line indent.
        /// </value>
        public double FirstLineIndent
        {
            get
            {
                return firstLineIndent;
            }
            set
            {
                firstLineIndent = value;
                NotifyPropertyChanged("FirstLineIndent");
            }
        }
        /// <summary>
        /// Gets or Sets the text alignment.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        public TextAlignment? TextAlignment
        {
            get
            {
                return textAlignment;
            }
            set
            {
                textAlignment = value;
                NotifyPropertyChanged("TextAlignment");
            }
        }
        /// <summary>
        /// Gets or Sets the after spacing.
        /// </summary>
        /// <value>
        /// The after spacing.
        /// </value>
        public double AfterSpacing
        {
            get
            {
                return afterSpacing;
            }
            set
            {
                afterSpacing = value;
                NotifyPropertyChanged("AfterSpacing");
            }
        }
        /// <summary>
        /// Gets or Sets the before spacing.
        /// </summary>
        /// <value>
        /// The before spacing.
        /// </value>
        public double BeforeSpacing
        {
            get
            {
                return beforeSpacing;
            }
            set
            {
                beforeSpacing = value;
                NotifyPropertyChanged("BeforeSpacing");
            }
        }
        /// <summary>
        /// Gets or Sets the line spacing.
        /// </summary>
        /// <value>
        /// The line spacing.
        /// </value>
        public double LineSpacing
        {
            get
            {
                return lineSpacing;
            }
            set
            {
                lineSpacing = value;
                NotifyPropertyChanged("LineSpacing");
            }
        }
        /// <summary>
        /// Gets or Sets the line spacing type.
        /// </summary>
        /// <value>
        /// The type of the line spacing.
        /// </value>
        public LineSpacingType? LineSpacingType
        {
            get
            {
                return lineSpacingType;
            }
            set
            {
               lineSpacingType = value;
               NotifyPropertyChanged("LineSpacingType");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionParagraphFormat"/> class.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal SelectionParagraphFormat(SelectionAdv selection)
        {
            this.selection = selection;
        }
        #endregion

        #region Event
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementations
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(selection.OwnerControl, new PropertyChangedEventArgs(propertyName));
            if (!selection.IsRetrieveFormatting)
            {
                switch (propertyName)
                {
                    case "LeftIndent": selection.OnLeftIndent(leftIndent);
                        break;
                    case "RightIndent": selection.OnRightIndent(rightIndent);
                        break;
                    case "FirstLineIndent": selection.OnFirstLineIndent(firstLineIndent);
                        break;
                    case "BeforeSpacing": selection.OnBeforeSpacing(beforeSpacing);
                        break;
                    case "AfterSpacing": selection.OnAfterSpacing(afterSpacing);
                        break;
                    case "TextAlignment": selection.OnTextAlignment(textAlignment.Value);
                        break;
                    case "LineSpacing": selection.OnLineSpacing(lineSpacing);
                        break;
                    case "LineSpacingType": selection.OnLineSpacingType(lineSpacingType.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(ParagraphFormat format)
        {
            LeftIndent = format.LeftIndent;
            RightIndent = format.RightIndent;
            FirstLineIndent = format.FirstLineIndent;
            AfterSpacing = format.AfterSpacing;
            BeforeSpacing = format.BeforeSpacing;
            LineSpacing = format.LineSpacing;
            LineSpacingType = format.LineSpacingType;
            TextAlignment = format.TextAlignment;
        }
        /// <summary>
        /// Combines the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal bool CombineFormat(ParagraphFormat format)
        {
            if (LeftIndent != format.LeftIndent)
                LeftIndent = 0;
            if (RightIndent != format.RightIndent)
                RightIndent = 0;
            if (FirstLineIndent != format.FirstLineIndent)
                FirstLineIndent = 0;
            if (LineSpacing != format.LineSpacing)
                LineSpacing = 0;
            if (BeforeSpacing != format.BeforeSpacing)
                BeforeSpacing = 0;
            if (AfterSpacing != format.AfterSpacing)
                AfterSpacing = 0;
            if (LineSpacingType != format.LineSpacingType)
                LineSpacingType = null;
            if (TextAlignment != format.TextAlignment)
                TextAlignment = null;
            return false;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            selection = null;
            textAlignment = null;
            lineSpacingType = null;
        }
        #endregion
    }
    public class SelectionSectionFormat : INotifyPropertyChanged
    {
        #region fields
        // Declaring owner selection
        private SelectionAdv selection;
        //Declaring character formats
        private bool? differentFirstPage;
        private bool? differentOddAndEvenPages;
        private double headerDistance;
        private double footerDistance;
        private Size pageSize;
        private Thickness pageMargin;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the page size.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public Size PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
                NotifyPropertyChanged("PageSize");
            }
        }
        /// <summary>
        /// Gets or Sets the page margins.
        /// </summary>
        /// <value>
        /// The page margin.
        /// </value>
        public Thickness PageMargin
        {
            get
            {
                return pageMargin;
            }
            set
            {
                pageMargin = value;
                NotifyPropertyChanged("PageMargin");
            }
        }
        /// <summary>
        /// Gets or Sets the header distance from page top.
        /// </summary>
        /// <value>
        /// The header distance.
        /// </value>
        internal double HeaderDistance
        {
            get
            {
                return headerDistance;
            }
            set
            {
               headerDistance = value;
               NotifyPropertyChanged("HeaderDistance");
            }
        }
        /// <summary>
        /// Gets or Sets the footer distance from page bottom.
        /// </summary>
        /// <value>
        /// The footer distance.
        /// </value>
        internal double FooterDistance
        {
            get
            {
                return footerDistance;
            }
            set
            {
                footerDistance = value;
                NotifyPropertyChanged("FooterDistance");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the section has different first page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the section has different first page; otherwise, <c>false</c>.
        /// </value>
        internal bool? DifferentFirstPage
        {
            get
            {
                return differentFirstPage;
            }
            set
            {
                differentFirstPage = value;
                NotifyPropertyChanged("DifferentFirstPage");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the section has different odd and even pages.
        /// </summary>
        /// <value>
        /// <c>true</c> if the section has different odd and even pages; otherwise, <c>false</c>.
        /// </value>
        internal bool? DifferentOddAndEvenPages
        {
            get
            {
                return differentOddAndEvenPages;
            }
            set
            {
                differentOddAndEvenPages = value;
                NotifyPropertyChanged("DifferentOddAndEvenPages");
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionSectionFormat"/> class.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal SelectionSectionFormat(SelectionAdv selection)
        {
            this.selection = selection;
        }
        #endregion

        #region Event
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementations
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!selection.IsRetrieveFormatting)
            {
                switch (propertyName)
                {
                    case "PageSize": selection.OnPageSizeChanged(pageSize);
                        break;
                    case "PageMargin": selection.OnPageMarginChanged(pageMargin);
                        break;
                }
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(SectionFormat format)
        {
            PageSize = format.PageSize;
            PageMargin = format.PageMargin;
            HeaderDistance = format.HeaderDistance;
            FooterDistance = format.FooterDistance;
            DifferentFirstPage = format.DifferentFirstPage;
            DifferentOddAndEvenPages = format.DifferentOddAndEvenPages;
        }
        /// <summary>
        /// Combines the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CombineFormat(SectionFormat format)
        {
            if (PageSize != format.PageSize)
                pageSize = new Size(0, 0);
            if (PageMargin != format.PageMargin)
                PageMargin = new Thickness(0);
            if (HeaderDistance != format.HeaderDistance)
                HeaderDistance = 0;
            if (FooterDistance != format.FooterDistance)
                FooterDistance = 0;
            if (DifferentFirstPage.HasValue && DifferentFirstPage != format.DifferentFirstPage)
                DifferentFirstPage = null;
            if (DifferentOddAndEvenPages.HasValue && DifferentOddAndEvenPages != format.DifferentOddAndEvenPages)
                DifferentOddAndEvenPages = null;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            selection = null;
            differentFirstPage = null;
            differentOddAndEvenPages = null;
        }
        #endregion
    }
    internal sealed class SelectionTableFormat : INotifyPropertyChanged
    {
        #region fields
        // Declaring owner selection
        private SelectionAdv selection;
        //Declaring character formats
        private double leftIndent;
        private Color? background;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the table left indent.
        /// </summary>
        /// <value>
        /// The left indent.
        /// </value>
        public double LeftIndent
        {
            get
            {
                return leftIndent;
            }
            set
            {
                leftIndent = value;
                NotifyPropertyChanged("LeftIndent");
            }
        }
        /// <summary>
        /// Gets or Sets the table background color.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color? Background
        {
            get 
            {
                return background; 
            }
            set
            { 
                background = value;
                NotifyPropertyChanged("Background");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionTableFormat"/> class.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal SelectionTableFormat(SelectionAdv selection)
        {
            this.selection = selection;
        }
        #endregion

        #region Event
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementations
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!selection.IsRetrieveFormatting)
            {
                // To do implementations
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(TableFormat format)
        {
            LeftIndent = format.LeftIndent;
            Background = format.Background;
        }
        /// <summary>
        /// Combines the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CombineFormat(TableFormat format)
        {
            if (LeftIndent != format.LeftIndent)
                LeftIndent = 0d;
            if (Background != format.Background)
                Background = null;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            selection = null;
            background = null;
        }
        #endregion
    }
    internal sealed class SelectionCellFormat : INotifyPropertyChanged
    {
        #region fields
        // Declaring owner selection
        private SelectionAdv selection;
        //Declaring character formats
        private Thickness cellMargin;
        private Color? background;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the cell margin.
        /// </summary>
        /// <value>
        /// The cell margin.
        /// </value>
        public Thickness CellMargin
        {
            get
            {
                return cellMargin;
            }
            set
            {
                cellMargin = value;
                NotifyPropertyChanged("Margin");
            }
        }
        /// <summary>
        /// Gets or Sets the cell background color.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color? Background
        {
            get
            {
                return background;
            }
            set
            {
                background = value;
                NotifyPropertyChanged("Background");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionCellFormat"/> class.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal SelectionCellFormat(SelectionAdv selection)
        {
            this.selection = selection;
        }
        #endregion

        #region Event
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementations
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!selection.IsRetrieveFormatting)
            {
                // To do implementations
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(CellFormat format)
        {
            CellMargin = format.CellMargin;
            Background = format.Background;
        }
        /// <summary>
        /// Combines the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CombineFormat(CellFormat format)
        {
            if (CellMargin != format.CellMargin)
                CellMargin = new Thickness(0);
            if (Background != format.Background)
                Background = null;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            selection = null;
            background = null;
        }
        #endregion
    }
}