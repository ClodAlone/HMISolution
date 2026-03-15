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
using System.Windows.Input;
using System.Collections.Generic;
#if !WPF
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class HistoryInfo
    {
        #region Fields
        private SfRichTextBoxAdv ownerControl;
        private Actions action;
        private List<Node> removedNodes;
        private List<DependencyObject> modifiedProperties;
        private List<int> modifiedNodeLength;
        private string selectionStart;
        private string selectionEnd;
        private string insertPosition;
        private string endPosition;
        private int currentPropertyIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the owner control
        /// </summary>
        /// <value>
        /// The owner control.
        /// </value>
        internal SfRichTextBoxAdv OwnerControl
        {
            get
            {
                return ownerControl;
            }
        }
        /// <summary>
        /// Gets or Sets the action
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        internal Actions Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }
        /// <summary>
        /// Gets or sets the modified nodes
        /// </summary>
        /// <value>
        /// The modified nodes.
        /// </value>
        internal List<Node> RemovedNodes
        {
            get
            {
                return removedNodes;
            }
        }
        /// <summary>
        /// Gets or sets the modified properties.
        /// </summary>
        /// <value>
        /// The modified properties.
        /// </value>
        internal List<DependencyObject> ModifiedProperties
        {
            get
            {
                return modifiedProperties;
            }
        }
        /// <summary>
        /// Gets or sets the selection start.
        /// </summary>
        /// <value>
        /// The selection start.
        /// </value>
        internal string SelectionStart
        {
            get
            {
                return selectionStart;
            }
            set
            {
                selectionStart = value;
            }
        }
        /// <summary>
        /// Gets or sets the selection end.
        /// </summary>
        /// <value>
        /// The selection end.
        /// </value>
        internal string SelectionEnd
        {
            get
            {
                return selectionEnd;
            }
            set
            {
                selectionEnd = value;
            }
        }
        /// <summary>
        /// Gets or sets the insert position.
        /// </summary>
        /// <value>
        /// The insert position.
        /// </value>
        internal string InsertPosition
        {
            get
            {
                return insertPosition;
            }
            set
            {
                insertPosition = value;
            }
        }
        /// <summary>
        /// Gets or sets the end position.
        /// </summary>
        /// <value>
        /// The end position.
        /// </value>
        internal string EndPosition
        {
            get
            {
                return endPosition;
            }
            set
            {
                endPosition = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryInfo"/> class.
        /// </summary>
        internal HistoryInfo()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryInfo" /> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        internal HistoryInfo(SfRichTextBoxAdv richTextBoxAdv)
        {
            ownerControl = richTextBoxAdv;
            removedNodes = new List<Node>();
            modifiedProperties = new List<DependencyObject>();
            modifiedNodeLength = new List<int>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Updates the selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void UpdateSelection(SelectionAdv selection)
        {
            selectionStart = selection.Start.GetHierarchicalIndex();
            selectionEnd = selection.End.GetHierarchicalIndex();
        }
        /// <summary>
        /// Adds the modified properties.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal object AddModifiedProperties(ParagraphFormat format, DependencyProperty property, object value)
        {
            if (OwnerControl.History.IsUndoing || OwnerControl.History.IsRedoing)
            {
                ParagraphFormat previousFormat = (currentPropertyIndex < modifiedProperties.Count ? modifiedProperties[currentPropertyIndex] : modifiedProperties[modifiedProperties.Count - 1]) as ParagraphFormat;
                value = previousFormat.ReadLocalValue(property);
                previousFormat.ClearValue(property);
                if (property == ParagraphFormat.ListFormatProperty)
                    previousFormat.ListFormat = new ListFormat();
                previousFormat.CopyFormat(format);
                currentPropertyIndex++;
            }
            else
            {
                ParagraphFormat currentFormat = new ParagraphFormat();
                currentFormat.CopyFormat(format);
                ModifiedProperties.Add(currentFormat);
            }
            return value;
        }
        /// <summary>
        /// Adds the modified properties.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal object AddModifiedProperties(CharacterFormat format, DependencyProperty property, object value)
        {
            if (OwnerControl.History.IsUndoing || OwnerControl.History.IsRedoing)
            {
                CharacterFormat previousFormat = (currentPropertyIndex < modifiedProperties.Count ? modifiedProperties[currentPropertyIndex] : modifiedProperties[modifiedProperties.Count - 1]) as CharacterFormat;
                if (format.OwnerBase is Inline)
                {
                    int prevLength = modifiedNodeLength[currentPropertyIndex];
                    if ((format.OwnerBase as Inline).Length < prevLength)
                    {
                        modifiedNodeLength[currentPropertyIndex] = (format.OwnerBase as Inline).Length;
                        modifiedNodeLength.Insert(currentPropertyIndex + 1, prevLength - (format.OwnerBase as Inline).Length);
                        //Adds a copy of character format at next position for splitted inline.
                        CharacterFormat nextFormat = new CharacterFormat();
                        nextFormat.CopyFormat(previousFormat);
                        modifiedProperties.Insert(currentPropertyIndex + 1, nextFormat);
                    }
                }
                value = previousFormat.ReadLocalValue(property);
                previousFormat.ClearValue(property);
                previousFormat.CopyFormat(format);
                currentPropertyIndex++;
            }
            else
            {
                CharacterFormat currentFormat = new CharacterFormat();
                currentFormat.CopyFormat(format);
                ModifiedProperties.Add(currentFormat);
                if (format.OwnerBase is Inline)
                    modifiedNodeLength.Add((format.OwnerBase as Inline).Length);
            }
            return value;
        }
        /// <summary>
        /// Adds the modified properties.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal object AddModifiedProperties(SectionFormat format, DependencyProperty property, object value)
        {
            if (OwnerControl.History.IsUndoing || OwnerControl.History.IsRedoing)
            {
                SectionFormat previousFormat = (currentPropertyIndex < modifiedProperties.Count ? modifiedProperties[currentPropertyIndex] : modifiedProperties[modifiedProperties.Count - 1]) as SectionFormat;
                value = previousFormat.ReadLocalValue(property);
                previousFormat.ClearValue(property);
                previousFormat.CopyFormat(format);
                currentPropertyIndex++;
            }
            else
            {
                SectionFormat currentFormat = new SectionFormat();
                currentFormat.CopyFormat(format);
                ModifiedProperties.Add(currentFormat);
            }
            return value;
        }
        /// <summary>
        /// Gets the text position.
        /// </summary>
        /// <param name="hierarchicalIndex">Index of the hierarchical.</param>
        /// <returns></returns>
        private TextPosition GetTextPosition(string hierarchicalIndex)
        {
            TextPosition textPosition = new TextPosition(OwnerControl);
            textPosition.SetPosition(hierarchicalIndex);
            return textPosition;
        }
        /// <summary>
        /// Reverts this instance.
        /// </summary>
        internal void Revert()
        {
            OwnerControl.IsShiftingEnabled = true;
            TextPosition selectionStartTextPosition = null, selectionEndTextPosition = null;
            string start = selectionStart, end = selectionEnd;
            bool isForwardSelection = TextPosition.IsForwardSelection(start, end);
            if (ModifiedProperties.Count > 0)
            {
                selectionStartTextPosition = GetTextPosition(start);
                selectionEndTextPosition = GetTextPosition(end);
                OwnerControl.Selection.Select(selectionStartTextPosition, selectionEndTextPosition);
                RevertProperties();
            }
            else
            {
                List<Node> deletedNodes = RemovedNodes;
                removedNodes = new List<Node>();
                bool isForward = TextPosition.IsForwardSelection(InsertPosition, EndPosition);
                TextPosition insertTextPosition = GetTextPosition(isForward ? InsertPosition : EndPosition);
                TextPosition endTextPosition = GetTextPosition(isForward ? EndPosition : InsertPosition);
                OwnerControl.Selection.Select(insertTextPosition);
                if (Action == Actions.InsertHyperlink && OwnerControl.History.IsRedoing)
                {
                    FieldBeginAdv fieldBegin = OwnerControl.Selection.GetHyperlinkField();
                    if (fieldBegin != null)
                    {
                        double offset = fieldBegin.OwnerParagraph.GetOffset(fieldBegin, 0);
                        insertTextPosition.SetPosition(fieldBegin.OwnerParagraph, offset);
                        OwnerControl.Selection.Start.SetPosition(insertTextPosition);
                        offset = fieldBegin.FieldEnd.OwnerParagraph.GetOffset(fieldBegin.FieldEnd, 1);
                        endTextPosition.SetPosition(fieldBegin.FieldEnd.OwnerParagraph, offset);
                    }
                }
                selectionStart = InsertPosition;
                InsertPosition = null;
                selectionEnd = EndPosition;
                EndPosition = null;
                OwnerControl.Selection.CurrentHistoryInfo = this;
                bool isRemoveContent = false;
                if (!insertTextPosition.IsAtSamePosition(endTextPosition))
                {
                    if (!(isRemoveContent = (Action == Actions.BackSpace || Action == Actions.Delete || Action == Actions.ClearCells
                        || Action == Actions.DeleteCells)))
                    {
                        OwnerControl.Selection.End.SetPosition(endTextPosition);
                        if (!OwnerControl.Selection.IsEmpty)
                        {
                            if (OwnerControl.History.IsRedoing)
                                OwnerControl.Selection.RemoveSelectedContent();
                            else
                                OwnerControl.Selection.DeleteSelectedContent(true);
                        }
                    }
                }
                bool isRedoAction = OwnerControl.History.IsRedoing && !isRemoveContent;
                RevertModifiedNodes(deletedNodes, isRedoAction, isForwardSelection ? start : end, start == end);
                if (isRemoveContent)
                {
                    //Removes if any empty paragraph is added while delete.
                    OwnerControl.Selection.Select(insertTextPosition, endTextPosition);
                    bool isDelete = (Action == Actions.DeleteCells || Action == Actions.BackSpace) ? true : false;
                    OwnerControl.Selection.DeleteSelectedContent(isDelete);
                }
            }
            if (OwnerControl.History.IsUndoing)
            {
                selectionStartTextPosition = GetTextPosition(start);
                selectionEndTextPosition = GetTextPosition(end);
                OwnerControl.Selection.Select(selectionStartTextPosition, selectionEndTextPosition);
            }
            //Updates insert position of history info instance.
            InsertPosition = start;
            EndPosition = end;
            OwnerControl.Selection.Relayout(OwnerControl.Selection.IsEmpty);
        }
        /// <summary>
        /// Reverts the modified nodes.
        /// </summary>
        /// <param name="deletedNodes">The deleted nodes.</param>
        /// <param name="isRedoAction">if set to <c>true</c> is redo action.</param>
        /// <param name="start">The start.</param>
        /// <param name="isEmptySelection">if set to <c>true</c> [is empty selection].</param>
        private void RevertModifiedNodes(List<Node> deletedNodes, bool isRedoAction, string start, bool isEmptySelection)
        {
            if (isRedoAction && (Action == Actions.Delete || Action == Actions.BackSpace || Action == Actions.MergeCells
                || Action == Actions.DeleteColumn || Action == Actions.DeleteRow || Action == Actions.InsertRowAbove
                || Action == Actions.InsertRowBelow || Action == Actions.InsertColumnLeft || Action == Actions.InsertColumnRight
                || Action == Actions.DeleteTable))
                RedoAction();
            else if (deletedNodes.Count > 0)
            {
                if (OwnerControl.History.IsUndoing && (Action == Actions.MergeCells || Action == Actions.ClearCells
                    || Action == Actions.DeleteCells || Action == Actions.DeleteColumn || Action == Actions.DeleteRow
                    || Action == Actions.InsertRowAbove || Action == Actions.InsertRowBelow || Action == Actions.InsertColumnLeft
                    || Action == Actions.InsertColumnRight))
                {
                    string insertIndex = selectionStart;
                    BlockAdv block = OwnerControl.Document.GetBlock(ref insertIndex);
                    Node firstNode = deletedNodes[0];
                    if (block is TableAdv && firstNode is TableAdv)
                    {
                        (block as TableAdv).InsertTable(firstNode as TableAdv, false);
                        deletedNodes.Remove(firstNode);
                    }
                }
                else
                {
                    string initialStart = start;
                    BlockAdv block = OwnerControl.Document.GetBlock(ref initialStart);
                    if (deletedNodes.Count > 0 && (Action == Actions.BackSpace && isEmptySelection || !(block is TableAdv)))
                    {
                        Node lastNode = deletedNodes[deletedNodes.Count - 1];
                        //If there is no next rendered block, then insert last paragraph first. 
                        if (lastNode is ParagraphAdv && OwnerControl.Selection.Start.Offset > 0)
                        {
                            OwnerControl.Selection.InsertParagraph(lastNode as ParagraphAdv, true);
                            deletedNodes.Remove(lastNode);
                            if (block == null)
                            {
                                BlockAdv nextBlock = (lastNode as ParagraphAdv).GetNextRenderedBlock();
                                if (nextBlock == null)
                                    //Sets the selection as starting of last paragraph.
                                    OwnerControl.Selection.Select(lastNode as ParagraphAdv, true);
                            }
                        }
                    }
                    if (deletedNodes.Count > 0)
                    {
                        Node firstNode = deletedNodes[0];
                        if (block is TableAdv)
                        {
                            if (firstNode is TableAdv)
                            {
                                (block as TableAdv).InsertTable(firstNode as TableAdv, true);
                                deletedNodes.Remove(firstNode);
                                InsertPosition = start;
                                if (firstNode.NextNode is TableAdv)
                                    block = firstNode.NextNode as TableAdv;
                                else
                                {
                                    initialStart = start;
                                    block = OwnerControl.Document.GetBlock(ref initialStart);
                                }
                            }
                        }
                        //Checks if first node is paragraph and current insert position is paragraph end.
                        else if (firstNode is ParagraphAdv && OwnerControl.Selection.Start.Offset > 0
                            && OwnerControl.Selection.Start.Offset == OwnerControl.Selection.Start.Paragraph.GetLength())
                        {
                            OwnerControl.Selection.InsertParagraph(firstNode as ParagraphAdv, false);
                            deletedNodes.Remove(firstNode);
                            //Removes the intermediate empty paragraph instance.
                            if (Action != Actions.Paste)
                                OwnerControl.Selection.Start.Paragraph.RemoveBlock();
                            ParagraphAdv paragraph = (firstNode as ParagraphAdv).GetNextParagraph();
                            OwnerControl.Selection.Select(paragraph, true);
                        }
                        if (deletedNodes.Count > 0)
                            InsertRemovedNodes(deletedNodes, block);
                    }
                }
            }
        }
        /// <summary>
        /// Inserts the removed nodes.
        /// </summary>
        /// <param name="deletedNodes">The deleted nodes.</param>
        /// <param name="block">The block.</param>
        private void InsertRemovedNodes(List<Node> deletedNodes, BlockAdv block)
        {
            for (int i = 0, j = 0; i < deletedNodes.Count; i++)
            {
                Node node = deletedNodes[i];
                deletedNodes.Remove(node);
                i--;

                if (node is Inline)
                    OwnerControl.Selection.InsertInline(node as Inline);
                else if (node is BlockAdv)
                {
                    if (block is TableAdv)
                        OwnerControl.Selection.InsertBlock(node as BlockAdv, block as TableAdv);
                    else
                    {
                        OwnerControl.Selection.InsertBlock(node as BlockAdv);
                        if (Action == Actions.InsertTable && node is TableAdv && deletedNodes.Count == 0)
                        {
                            ParagraphAdv paragraph = (node as TableAdv).GetFirstParagraphInFirstCell();
                            OwnerControl.Selection.Select(paragraph, true);
                        }
                    }
                }
                else if (node is TableRowAdv)
                {
                    if (block is TableAdv)
                    {
                        (block as TableAdv).Rows.Insert(j, node as TableRowAdv);
                        j++;
                    }
                    else
                    {
                    }
                }
                else if (node is SectionAdv)
                {
                    if (block is TableAdv)
                        OwnerControl.Selection.InsertSection(node as SectionAdv, block as TableAdv);
                    else
                        OwnerControl.Selection.InsertSection(node as SectionAdv);
                }
            }
        }
        /// <summary>
        /// Inserts the cloned field result.
        /// </summary>
        /// <param name="fieldSeparator">The field separator.</param>
        internal void InsertClonedFieldResult(FieldSeparatorAdv fieldSeparator)
        {
            bool isStarted = false;
            for (int i = 0; i < RemovedNodes.Count; i++)
            {
                Node node = RemovedNodes[i];
                if (!isStarted)
                {
                    if (fieldSeparator == node)
                        isStarted = true;
                    else
                    {
                        if (node is ParagraphAdv && node == fieldSeparator.OwnerParagraph)
                        {
                            isStarted = true;
                            ParagraphAdv paragraph = null;
                            if (i == 0)
                            {
                                paragraph = OwnerControl.Selection.Start.Paragraph;
                                fieldSeparator.OwnerParagraph.GetClonedFieldResult(OwnerControl.Selection, fieldSeparator);
                            }
                            else
                            {
                                paragraph = fieldSeparator.OwnerParagraph.GetClonedFieldResult(fieldSeparator);
                                OwnerControl.Selection.Start.Paragraph.InsertParagraph(paragraph, OwnerControl.Selection.Start.Offset, true);
                            }
                            OwnerControl.Selection.Select(paragraph.GetNextParagraph(), true);
                        }
                        continue;
                    }
                }
                if (node is Inline)
                    OwnerControl.Selection.InsertInlineInternal((node as Inline).Clone());
                else if (node is BlockAdv)
                    OwnerControl.Selection.InsertBlockInternal((node as BlockAdv).Clone());
                else if (node is SectionAdv)
                    OwnerControl.Selection.InsertSection((node as SectionAdv).Clone());
            }
        }
        /// <summary>
        /// Redoes the action.
        /// </summary>
        private void RedoAction()
        {
            switch (Action)
            {
                case Actions.BackSpace:
                    OwnerControl.Selection.SingleBackspace(true);
                    break;
                case Actions.Delete:
                    OwnerControl.Selection.SingleDelete(true);
                    break;
                case Actions.MergeCells:
                    OwnerControl.Selection.MergeSelectedCells();
                    break;
                case Actions.InsertRowAbove:
                    OwnerControl.Selection.InsertRow(RowPlacement.Above);
                    break;
                case Actions.InsertRowBelow:
                    OwnerControl.Selection.InsertRow(RowPlacement.Below);
                    break;
                case Actions.InsertColumnLeft:
                    OwnerControl.Selection.InsertColumn(ColumnPlacement.Left);
                    break;
                case Actions.InsertColumnRight:
                    OwnerControl.Selection.InsertColumn(ColumnPlacement.Right);
                    break;
                case Actions.DeleteColumn:
                    OwnerControl.Selection.DeleteColumn();
                    break;
                case Actions.DeleteRow:
                    OwnerControl.Selection.DeleteRow();
                    break;
                case Actions.DeleteTable:
                    OwnerControl.Selection.DeleteTable();
                    break;
            }
        }
        /// <summary>
        /// Reverts the properties.
        /// </summary>
        private void RevertProperties()
        {
            OwnerControl.Selection.CurrentHistoryInfo = this;
            currentPropertyIndex = 0;
            DependencyProperty property = GetDependencyProperty();
            if (modifiedProperties[0] is CharacterFormat)
                OwnerControl.Selection.UpdateCharacterFormat(property, DependencyProperty.UnsetValue);
            else if (modifiedProperties[0] is ParagraphFormat)
                OwnerControl.Selection.UpdateParagraphFormat(property, DependencyProperty.UnsetValue);
            else if (modifiedProperties[0] is ImageFormat)
                OwnerControl.Selection.UpdateImageSize(modifiedProperties[0] as ImageFormat);
            else if (modifiedProperties[0] is SectionFormat)
                OwnerControl.Selection.UpdateSectionFormat(property, DependencyProperty.UnsetValue);
            currentPropertyIndex = 0;
        }
        /// <summary>
        /// Gets the dependency property.
        /// </summary>
        /// <returns></returns>
        private DependencyProperty GetDependencyProperty()
        {
            switch(Action)
            {
                case Actions.Bold:
                    return CharacterFormat.BoldProperty;
                case Actions.Italic:
                    return CharacterFormat.ItalicProperty;
                case Actions.FontColor:
                    return CharacterFormat.FontColorProperty;
                case Actions.FontFamily:
                    return CharacterFormat.FontFamilyProperty;
                case Actions.FontSize:
                    return CharacterFormat.FontSizeProperty;
                case Actions.HighlightColor:
                    return CharacterFormat.HighlightColorProperty;
                case Actions.BaselineAlignment:
                    return CharacterFormat.BaselineAlignmentProperty;
                case Actions.StrikeThrough:
                    return CharacterFormat.StrikeThroughProperty;
                case Actions.Underline:
                    return CharacterFormat.UnderlineProperty;
                case Actions.AfterSpacing:
                    return ParagraphFormat.AfterSpacingProperty;
                case Actions.BeforeSpacing:
                    return ParagraphFormat.BeforeSpacingProperty;
                case Actions.LeftIndent:
                    return ParagraphFormat.LeftIndentProperty;
                case Actions.RightIndent:
                    return ParagraphFormat.RightIndentProperty;
                case Actions.FirstLineIndent:
                    return ParagraphFormat.FirstLineIndentProperty;
                case Actions.LineSpacingType:
                    return ParagraphFormat.LineSpacingTypeProperty;
                case Actions.LineSpacing:
                    return ParagraphFormat.LineSpacingProperty;
                case Actions.TextAlignment:
                    return ParagraphFormat.TextAlignmentProperty;
                case Actions.ListFormat:
                    return ParagraphFormat.ListFormatProperty;
                case Actions.PageMargin:
                    return SectionFormat.PageMarginProperty;
                case Actions.PageSize:
                    return SectionFormat.PageSizeProperty;
            }
            return null;
        }
        #endregion
    }
    internal class ImageFormat : DependencyObject
    {
        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        internal double Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        internal double Height
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFormat"/> class.
        /// </summary>
        /// <param name="imageContainer">The image container.</param>
        internal ImageFormat(ImageContainerAdv imageContainer)
        {
            Width = imageContainer.Width;
            Height = imageContainer.Height;
        }
        #endregion
    }
}