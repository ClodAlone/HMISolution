#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public class DocumentPositionHandler
    {
        private CaretAdv caret;
        private TextPosition textPosition;
        private DocumentAdv documentAdv;
        internal LineInfo Line;
        internal Inline DeletedInline;

        public DocumentPositionHandler(DocumentAdv document)
        {
            documentAdv = document;
            TextPosition = new TextPosition(Document);
        }

        internal TextPosition TextPosition
        {
            get
            {
                return textPosition;
            }
            set
            {
                textPosition = value;
            }
        }

        internal PageAdv CurrentPage
        {
            get
            {
                return OwnerControl.Viewer.CurrentPage;
            }
            set
            {
                OwnerControl.Viewer.CurrentPage = value;
            }
        }

        public DocumentAdv Document
        {
            get
            {
                return documentAdv;
            }
        }

        private bool isStyleChnaged = false;

        internal bool IsStyleChanged
        {
            get
            {
                return isStyleChnaged;
            }
            set
            {
                isStyleChnaged = value;
            }
        }

        internal RichTextBoxAdv OwnerControl
        {
            get;
            set;
        }

        internal ParagraphAdv Paragraph
        {
            get
            {
                if (TextPosition != null)
                    return TextPosition.Paragraph;
                return null;
            }
        }

        internal CaretAdv Caret
        {
            get
            {
                return caret;
            }
            set
            {
                if (caret != value)
                {
                    if (caret != null) caret.Hide();
                    caret = value;
                    if(!OwnerControl.IsReadOnly || OwnerControl.EnableCursorOnReadOnly)
                        caret.Show();
                }
                caret = value;
            }
        }

        /// <summary>
        /// Gets the starting position of the document
        /// </summary>
        public TextPosition StartingPosOfDocument
        {
            get
            {
                return new TextPosition(Document);
            }
        }

        /// <summary>
        /// Gets the ending position of the document
        /// </summary>
        public TextPosition EndingPosOfDocument
        {
            get
            {
                TextPosition pos = null;
                if (Document.Sections.Count > 0)
                {
                    SectionAdv section = Document.Sections.Last();
                    if (section.Blocks.Count > 0)
                    {
                        pos = new TextPosition(Document);
                        BlockAdv block = null;
                        if (section.Blocks.Last() is TableAdv)
                        {
                            block = (section.Blocks.Last() as TableAdv).GetLastBlockInLastCell();
                            while (block is TableAdv)
                            {
                                block = (section.Blocks.Last() as TableAdv).GetLastBlockInLastCell();
                            }
                        }
                        else if (section.Blocks.Last() is ParagraphAdv)
                        {
                            block = section.Blocks.Last();
                        }
                        pos.Paragraph = block as ParagraphAdv;
                        pos.SetIndex(pos.Paragraph.Length());
                    }
                }
                return pos;
            }
        }

        private InlineStyle InlineStyle
        {
            get
            {
                return OwnerControl.CurrentInlineStyle;
            }
        }

        private ParagraphStyle ParagraphStyle
        {
            get
            {
                return OwnerControl.CurrentParagraphStyle;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementBox"></param>
        public void UpdateCurrentInlineStyle(ElementBox elementBox)
        {
            if (elementBox is ImageElementBox || elementBox is UIElementBox)
                elementBox = GetPreviousSpanBox(elementBox);
            if (elementBox is TextElementBox)
            {
                SpanAdv span = elementBox.Inline as SpanAdv;
                if (InlineStyle.Baseline != span.Baseline) InlineStyle.Baseline = span.Baseline;
                if (InlineStyle.FontFamily != span.FontFamily) InlineStyle.FontFamily = span.FontFamily;
                if (InlineStyle.FontSize != span.FontSize) InlineStyle.FontSize = span.FontSize;
                if (InlineStyle.FontStyle != span.FontStyle) InlineStyle.FontStyle = span.FontStyle;
                if (InlineStyle.FontWeight != span.FontWeight) InlineStyle.FontWeight = span.FontWeight;
                if (InlineStyle.Foreground != span.Foreground) InlineStyle.Foreground = span.Foreground;
                if (InlineStyle.HighlightColor != span.HighlightColor) InlineStyle.HighlightColor = span.HighlightColor;
                InlineStyle.InlineType = typeof(SpanAdv);
                InlineStyle.StrikeThrough = span.StrikeThrough;
                InlineStyle.IsUnderline = span.Underline;
            }
            else if (elementBox is HyperlinkElementBox)
            {
                HyperlinkAdv span = elementBox.Inline as HyperlinkAdv;
                if (InlineStyle.Baseline != span.Baseline) InlineStyle.Baseline = span.Baseline;
                if (InlineStyle.FontFamily != span.FontFamily) InlineStyle.FontFamily = span.FontFamily;
                if (InlineStyle.FontSize != span.FontSize) InlineStyle.FontSize = span.FontSize;
                if (InlineStyle.FontStyle != span.FontStyle) InlineStyle.FontStyle = span.FontStyle;
                if (InlineStyle.FontWeight != span.FontWeight) InlineStyle.FontWeight = span.FontWeight;
                if (InlineStyle.Foreground != span.Foreground) InlineStyle.Foreground = span.Foreground;
                if (InlineStyle.HighlightColor != span.HighlightColor) InlineStyle.HighlightColor = span.HighlightColor;
                InlineStyle.InlineType = typeof(HyperlinkAdv);
                InlineStyle.StrikeThrough = span.StrikeThrough;
                InlineStyle.IsUnderline = span.Underline;
            }

        }

        public void SetDefaultInlineStyle()
        {
            InlineStyle.SetDefaultStyle();
        }
        public void SetCurrentInlineStyle()
        {
            InlineStyle.SetCurrentStyle();
        }
        public ElementBox GetPreviousSpanBox(ElementBox elementBox)
        {
            if (elementBox.PreviousElementBox is TextElementBox || elementBox.PreviousElementBox is HyperlinkElementBox)
            {
                return elementBox.PreviousElementBox;
            }
            else if (elementBox.PreviousElementBox == null)
            {
                return null;
            }
            else
            {
                return GetPreviousSpanBox(elementBox.PreviousElementBox);
            }
        }

        /// <summary>
        /// Inserts the text using the TextPosition
        /// </summary>
        /// <param name="str"></param>
        public void InsertText(string str)
        {
            int indexInInline = 0;
            int indexInElementBox = 0;
            bool canArrange = true;
            Inline inline = GetInlineFromTextPosition(ref indexInInline);
            ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);

            if (Paragraph != null && Paragraph.Inlines.Count == 0)
            {
                SpanAdv span = SpanAdv.CreateNewSpan(InlineStyle);
                span.Text = str;
                span.MeasureElements();
                Paragraph.Inlines.Add(span);
                Paragraph.LinkElementBoxes();
                Paragraph.ArrangeElements();
                TextPosition.Index = TextPosition.StepUp(1);
                IsStyleChanged = false;
            }
            else if (elementBox == null && inline != null && inline.ElementBoxes.Count == 0 &&
                inline.InternalText == string.Empty && Paragraph.Inlines.Count == 1)
            {
                if (inline is SpanAdv)
                {
                    SpanAdv span = inline as SpanAdv;
                    span.Text = str;
                    span.MeasureElements();
                }
                else if (inline is HyperlinkAdv)
                {
                    HyperlinkAdv hyperlink = inline as HyperlinkAdv;
                    hyperlink.Text = str;
                    hyperlink.MeasureElements();
                }
                Paragraph.LinkElementBoxes();
                Paragraph.ArrangeElements();
                TextPosition.Index = TextPosition.StepUp(1);
                IsStyleChanged = true;
            }
            else
            {
                if (IsStyleChanged || (inline != null && (inline.IsUIContainer || inline.IsImageContainer)))
                {
                    SpanAdv span = SpanAdv.CreateNewSpan(InlineStyle);
                    span.Text = str;
                    span.MeasureElements();
                    InsertInline(span);
                    canArrange = false;
                    IsStyleChanged = false;
                }
                else
                {
                    if (str == " " || str == "\t")
                    {
                        if (inline != null)
                        {
                            inline.InternalText = inline.InternalText.Substring(0, Convert.ToInt32(indexInInline)) + str + inline.InternalText.Substring(Convert.ToInt32(indexInInline));
                            if (elementBox != null)
                            {
                                bool needToHandle = inline is SpanAdv ? HandleURLText(inline,indexInInline) :true;
                                if(needToHandle)
                                {
                                    int boxIndex = inline.ElementBoxes.IndexOf(elementBox);
                                    string remainingText = string.Empty;
                                    ElementBox element = elementBox.CreateElementBox();
                                    element.Inline = inline;
                                    element.InternalText = str;
                                    boxIndex++;
                                    inline.ElementBoxes.Insert(boxIndex, element);
                                    remainingText = elementBox.InternalText.Substring(Convert.ToInt32(indexInElementBox));
                                    if (!string.IsNullOrEmpty(remainingText))
                                    {
                                        element = elementBox.CreateElementBox();
                                        element.InternalText = elementBox.InternalText.Substring(Convert.ToInt32(indexInElementBox));
                                        element.Inline = inline;
                                        boxIndex++;
                                        inline.ElementBoxes.Insert(boxIndex, element);
                                        elementBox.InternalText = elementBox.InternalText.Substring(0, Convert.ToInt32(indexInElementBox));
                                    }
                                }
                                int index = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
                                Paragraph.LinkElementBoxes();
                                Paragraph.ArrangeElements(index);
                            }
                        }
                    }
                    else
                    {
                        if (inline != null)
                        {
                            inline.InternalText = inline.InternalText.Substring(0, Convert.ToInt32(indexInInline)) + str + inline.InternalText.Substring(Convert.ToInt32(indexInInline));
                            if (elementBox != null)
                            {
                                if (elementBox.InternalText != " " && elementBox.InternalText != "\t")
                                {
                                    elementBox.InternalText = elementBox.InternalText.Substring(0, Convert.ToInt32(indexInElementBox)) + str + elementBox.InternalText.Substring(Convert.ToInt32(indexInElementBox));
                                }
                                else
                                {
                                    int boxIndex = inline.ElementBoxes.IndexOf(elementBox);
                                    ElementBox element = elementBox.CreateElementBox();
                                    element.Inline = inline;
                                    element.InternalText = str;
                                    boxIndex++;
                                    inline.ElementBoxes.Insert(boxIndex, element);
                                }

                                int index = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
                                Paragraph.LinkElementBoxes();
                                Paragraph.ArrangeElements(index);
                            }
                        }
                    }

                    TextPosition.Index = TextPosition.StepUp(1);
                }
            }

            if (canArrange)
                InvalidateVisibleRegion();
        }

        internal LineInfo GetLineFromIndex(string index, ParagraphAdv paragraph)
        {
            int indexInInline = 0;
            int indexInElementBox = 0;
            Inline inline = GetInlineFromIndex(paragraph, index, ref indexInInline);
            ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
            if (elementBox != null)
                return elementBox.LineInfo;
            return null;
        }


        /// <summary>
        /// Inserts the inline into current text position
        /// </summary>
        /// <param name="newInline"></param>
        public void InsertInline(Inline newInline)
        {
            if (newInline == null)
                return;
            int indexInInline = 0;
            int indexInElementBox = 0;
            Inline inline = GetInlineFromTextPosition(ref indexInInline);
            ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);

            if (Paragraph != null && Paragraph.Inlines.Count == 0)
            {
                AttachParagraphToBlock();
                newInline.MeasureElements();
                Paragraph.Inlines.Add(newInline);
                BlockAdv nextblock = Paragraph.NextBlock;
                if (nextblock != null)
                {
                    nextblock.PreviousBlock = Paragraph;
                }
                Paragraph.ArrangeElements();
                TextPosition.Index = TextPosition.StepUp(Convert.ToInt16(newInline.GetLength()));
                Paragraph.LinkElementBoxes();
                IsStyleChanged = false;
            }
            else
            {
                if (inline != null && (inline.IsUIContainer || inline.IsImageContainer))
                {
                    newInline.MeasureElements();
                    if (Paragraph.Inlines.Count > 0)
                    {
                        int inlineIndex = TextPosition.IsPositionAtParagraphStart ? Paragraph.Inlines.IndexOf(inline) : Paragraph.Inlines.IndexOf(inline) + 1;
                        Paragraph.Inlines.Insert(inlineIndex, newInline);
                        Paragraph.LinkElementBoxes();
                        int index = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
                        Paragraph.ArrangeElements(index);
                    }
                    else
                    {
                        Paragraph.Inlines.Add(newInline);
                        Paragraph.LinkElementBoxes();
                        Paragraph.ArrangeElements();
                    }

                    TextPosition.Index = TextPosition.StepUp(Convert.ToInt16(newInline.GetLength()));
                }
                else
                {
                    if (inline != null)
                    {
                        Inline lastInline = null;
                        newInline.MeasureElements();
                        if (indexInInline != 0 && indexInInline != inline.GetLength())
                        {
                            lastInline = inline.CreatInline();
                            lastInline.InternalText = inline.InternalText.Substring(indexInInline);
                            lastInline.MeasureElements();
                            inline.InternalText = inline.InternalText.Substring(0, indexInInline);
                            if (elementBox != null)
                            {
                                elementBox.InternalText = elementBox.InternalText.Substring(0, indexInElementBox);
                                int index = inline.ElementBoxes.IndexOf(elementBox) + 1;

                                for (int i = index; i < inline.ElementBoxes.Count; i = index)
                                {
                                    inline.ElementBoxes.Remove(inline.ElementBoxes[i]);
                                }
                            }
                        }

                        int inlineIndex = TextPosition.IsPositionAtParagraphStart ? Paragraph.Inlines.IndexOf(inline) : Paragraph.Inlines.IndexOf(inline) + 1;
                        Paragraph.Inlines.Insert(inlineIndex, newInline);

                        if (lastInline != null && !string.IsNullOrEmpty(lastInline.InternalText))
                        {
                            inlineIndex++;
                            Paragraph.Inlines.Insert(inlineIndex, lastInline);
                        }
                        Paragraph.LinkElementBoxes();
                        if (elementBox != null)
                        {
                            int index = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
                            Paragraph.ArrangeElements(index);
                        }
                        else
                        {
                            Paragraph.ArrangeElements();
                        }
                    }

                    TextPosition.Index = TextPosition.StepUp(int.Parse(newInline.GetLength().ToString()));
                }
            }
            IsStyleChanged = false;
            InvalidateVisibleRegion();
        }
        /// <summary>
        /// Attach the paragraph to its owner block while inserting inline by selecting and replacing the entire inlines of a paragraph
        /// </summary>
        /// <![CDATA[//This AttachParagraphToBlock method checks for availability of current Paragraph (textPosition.Paragraph) in its parent's blocks. If not present, then it will add the paragraph to its owner blocks.]]>
        private void AttachParagraphToBlock()
        {
            //Get the section index into a integer variable
            int sectionIndex = Convert.ToInt32(textPosition.SectionIndex);
            //Initialize a temp varaible to hold the virtual paragraph index
            int paragraphIndex = -1;
            //If the paragraphIndex is not proper, then find the current paragraph index and insert the paragraph into proper Blocks.
            if (textPosition.ParagraphIndex < 0)
            {
                //Checking the previous block element for null
                if (Paragraph.PreviousBlock != null)
                {
                    //check the presence of paragraph within a table or not
                    if (!Paragraph.IsInsideTable)
                    {
                        //Insert the paragraph in its proper index while the paragraph is present outside a table
                        paragraphIndex = Document.Sections[sectionIndex].Blocks.IndexOf(Paragraph.PreviousBlock);
                        if (paragraphIndex >= 0)
                            paragraphIndex++;
                    }
                    else
                    {
                        //Insert the paragraph in its proper index while the paragraph is present inside a table
                        if (Paragraph.AssociatedCell != null)
                        {
                            paragraphIndex = Paragraph.AssociatedCell.Blocks.IndexOf(Paragraph.PreviousBlock);
                            if (paragraphIndex > 0)
                                paragraphIndex++;
                        }
                    }
                }
                else if (Paragraph.NextBlock != null)
                {
                    //check the presence of paragraph within a table or not
                    if (!Paragraph.IsInsideTable)
                    {
                        //Insert the paragraph in its proper index while the paragraph is present outside a table
                        paragraphIndex = Document.Sections[sectionIndex].Blocks.IndexOf(Paragraph.NextBlock);
                    }
                    else
                    {
                        //Insert the paragraph in its proper index while the paragraph is present inside a table
                        if (Paragraph.AssociatedCell != null)
                        {
                            paragraphIndex = Paragraph.AssociatedCell.Blocks.IndexOf(Paragraph.NextBlock);
                        }
                    }
                }
                else
                {
                    paragraphIndex = 0;
                }
            }
            //Inserts the Paragraph in the calculated paragraph index.
            if (paragraphIndex >= 0)
            {
                if (!Paragraph.IsInsideTable)
                {
                    if (!Document.Sections[sectionIndex].Blocks.Contains(Paragraph))
                        Document.Sections[sectionIndex].Blocks.Insert(paragraphIndex, Paragraph);
                }
                else
                {
                    if (!Paragraph.AssociatedCell.Blocks.Contains(Paragraph))
                    {
                        Paragraph.AssociatedCell.Blocks.Insert(paragraphIndex, Paragraph);
                    }
                }
            }
        }

        internal void InsertTable(int Rows, int Columns)
        {
            TableAdv table = new TableAdv(Rows, Columns);
            table.RightIndent = Paragraph.RightIndent;
            table.LeftIndent = Paragraph.LeftIndent;
            table.IsInsideTable = Paragraph.IsInsideTable;
            table.LayoutViewer = Paragraph.GetLayoutViewer();
            table.Section = Paragraph.Section;
            table.Margin = Paragraph.Margin;
            table.AssociatedCell = Paragraph.AssociatedCell;
            table.MeasureElements();

            InsertTable(table);
        }

        internal void InsertTable(TableAdv table)
        {
            InsertedTableHistory history = new InsertedTableHistory();
            history.Action = Actions.InsertTable;
            history.StartPosition = TextPosition.CopyForHistory();
            history.RowsCount = table.Rows.Count;
            history.ColumnsCount = table.TableHolder.Columns.Count;

            int indextoinsert = 0;
            bool arrange = false;

            if (Paragraph != null)
            {
                Enter();

                if (Paragraph.IsInsideTable)
                {
                    indextoinsert = Paragraph.AssociatedCell.Blocks.IndexOf(Paragraph);
                    Paragraph.AssociatedCell.Blocks.AddBlockAtIndex(indextoinsert, table);
                    Paragraph.PreviousBlock = table;
                }
                else
                {
                    indextoinsert = Document.Sections[0].Blocks.IndexOf(Paragraph);
                    Document.Sections[0].Blocks.AddBlockAtIndex(indextoinsert, table);
                    Paragraph.PreviousBlock = table;
                }

                OwnerControl.Viewer.SetIsArrangedToFalse();
                OwnerControl.Viewer.SetPreviousBlocks();
                table.ArrangeElements();

                history.TableIndex = indextoinsert;

                arrange = true;
            }
            OwnerControl.History.RecordUndo(history);

            if (arrange)
            {
                TextPosition.Paragraph = table.GetFirstBlockInFirstCell() as ParagraphAdv;
                TextPosition.SetIndex("0");
                InvalidateVisibleRegion();
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        public void SelectCell()
        {
            if (Paragraph.IsInsideTable)
            {
                SelectCell(Paragraph.AssociatedCell);
            }
        }

        public void SelectCell(TableCellAdv tablecell)
        {
            TableCellElementBox cbox = tablecell.CellElementBox;
            PageAdv page = Paragraph.GetLayoutViewer().GetPageFromLine(cbox.LineInfo);
            PathFigure fig = cbox.SelectBox();
            PathGeometry pathgeo = new PathGeometry();
            pathgeo.Figures.Add(fig);

            if (cbox.BottomCellBox != null)
            {
                TableCellElementBox bottom = cbox.BottomCellBox;
                while (bottom != null)
                {
                    pathgeo.Figures.Add(bottom.SelectBox());
                    bottom = bottom.BottomCellBox;
                }
            }
            if (pathgeo.Figures.Count > 0)
            {
                OwnerControl.Selection.IsCellSelected = true;
            }
            TextPosition textposition = new TextPosition(Document);
            if (tablecell.Blocks[0].IsParagraph)
            {
                textposition.Paragraph = tablecell.Blocks[0] as ParagraphAdv;
                textposition.SetIndex("0");
            }
            else if (tablecell.Blocks[0].IsTable)
            {
                BlockAdv parent = tablecell.Blocks[0];
                while (parent.IsTable)
                {
                    parent = (parent as TableAdv).GetFirstBlockInFirstCell();
                }
                textposition.Paragraph = parent as ParagraphAdv;
                textposition.SetIndex("0");
            }
            textposition.VirtualPosition = textposition.Index;
            OwnerControl.Selection.Start = textposition;

            TextPosition textPos = new Controls.TextPosition(Document);
            if (tablecell.Blocks.Last().IsParagraph)
            {
                textPos.Paragraph = tablecell.Blocks.Last() as ParagraphAdv;
                textPos.SetIndex("0");
            }
            else if (tablecell.Blocks.Last().IsTable)
            {
                BlockAdv parent = tablecell.Blocks.Last();
                while (parent.IsTable)
                {
                    parent = (parent as TableAdv).GetLastBlockInLastCell();
                }
                textPos.Paragraph = parent as ParagraphAdv;
                textPos.SetIndex("0");
            }
            textPos.VirtualPosition = textPos.Index;
            OwnerControl.Selection.End = textPos;

            if (page != null)
            {
                page.SelectedLines.Add(cbox.LineInfo);

                if (page.SelectionPath == null)
                {
                    page.SelectionPath = new Path();
                    page.SelectionPath.Fill = OwnerControl.Selection.SelectionFillColor;
                    page.SelectionPath.Stroke = OwnerControl.Selection.SelectionStrokeColor;
                    page.SelectionPath.Opacity = 0.5;
                }
                if (page.SelectionPath != null)
                {
                    if (page.SelectionPath != null)
                        page.SelectionPath.Data = pathgeo;
                    Paragraph.GetLayoutViewer().IsSelected = true;
                }
            }
        }

        public void SelectRow(TableRowAdv rowtoselect)
        {
            if (Paragraph.IsInsideTable)
            {
                List<ElementBox> cboxes = new List<ElementBox>();
                PathGeometry geo = new PathGeometry();

                TextPosition startTextPos = new TextPosition(Document);

                BlockAdv first = rowtoselect.Cells[0].Blocks[0];
                if (first.IsParagraph)
                {
                    startTextPos.Paragraph = first as ParagraphAdv;
                }
                else
                {
                    while (first.IsTable)
                    {
                        first = (first as TableAdv).GetFirstBlockInFirstCell();
                    }
                    startTextPos.Paragraph = first as ParagraphAdv;
                }
                startTextPos.SetIndex("0");

                OwnerControl.Selection.Start = startTextPos;


                TextPosition endTextPos = new TextPosition(Document);

                BlockAdv last = rowtoselect.Cells[rowtoselect.Cells.Count - 1].Blocks[rowtoselect.Cells[rowtoselect.Cells.Count - 1].Blocks.Count - 1];

                if (last.IsParagraph)
                {
                    endTextPos.Paragraph = last as ParagraphAdv;
                }
                else
                {
                    while (last.IsTable)
                    {
                        last = (last as TableAdv).GetLastBlockInLastCell();
                    }
                    endTextPos.Paragraph = last as ParagraphAdv;
                }
                endTextPos.SetIndex(last.Length());

                OwnerControl.Selection.End = endTextPos;

                OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(startTextPos, endTextPos);

                foreach (TableCellAdv c in rowtoselect.Cells)
                {
                    cboxes.Add(c.CellElementBox);
                }
                cboxes.ForEach(c => geo.Figures.Add((c as TableCellElementBox).SelectBox()));

                PageAdv page = Paragraph.GetLayoutViewer().GetPageFromLine(Paragraph.AssociatedCell.CellElementBox.LineInfo);
                page.SelectedLines.Add(rowtoselect.Cells[0].CellElementBox.LineInfo);

                if (page.SelectionPath == null)
                {
                    page.SelectionPath = new Path();
                    page.SelectionPath.Fill = OwnerControl.Selection.SelectionFillColor;
                    page.SelectionPath.Stroke = OwnerControl.Selection.SelectionStrokeColor;
                    page.SelectionPath.Opacity = 0.5;
                }
                if (page.SelectionPath != null)
                {
                    if (page.SelectionPath != null)
                        page.SelectionPath.Data = geo;
                    Paragraph.GetLayoutViewer().IsSelected = true;
                }
            }
        }

        public void SelectRow()
        {
            if (!Paragraph.IsInsideTable)
                return;

            TableRowAdv currentrow = Paragraph.AssociatedCell.OwnerRow;
            SelectRow(currentrow);
        }

        public void SelectColumn(double x1, double x2)
        {
            if (Paragraph.IsInsideTable)
            {
                PathGeometry pathgeo = new PathGeometry();
                PageAdv page = Paragraph.GetLayoutViewer().GetPageFromLine(Paragraph.AssociatedCell.CellElementBox.LineInfo);

                List<ElementBox> cboxes = new List<ElementBox>();

                foreach (LineInfo line in Paragraph.AssociatedCell.OwnerTable.LineInfo)
                {
                    foreach (ElementBox b in line.ElementBoxes)
                    {
                        if (b.BoundingRectangle.X >= x1 && b.BoundingRectangle.Right <= x2)
                        {
                            pathgeo.Figures.Add((b as TableCellElementBox).SelectBox());
                        }
                    }
                    if (line.ElementBoxes.Where(e => (e as TableCellElementBox).IsFullCellSelected).Count<ElementBox>() > 0)
                    {
                        page.SelectedLines.Add(line);
                    }
                }

                if (page.SelectionPath == null)
                {
                    page.SelectionPath = new Path();
                    page.SelectionPath.Fill = OwnerControl.Selection.SelectionFillColor;
                    page.SelectionPath.Stroke = OwnerControl.Selection.SelectionStrokeColor;
                    page.SelectionPath.Opacity = 0.5;
                }
                if (page.SelectionPath != null)
                {
                    if (page.SelectionPath != null)
                        page.SelectionPath.Data = pathgeo;
                    Paragraph.GetLayoutViewer().IsSelected = true;
                }
            }
        }

        public void SelectColumn(int index)
        {
            if (!Paragraph.IsInsideTable)
                return;

            PathGeometry geo = new PathGeometry();

            TableAdv table = Paragraph.AssociatedCell.OwnerTable;
            PageAdv page = Paragraph.GetLayoutViewer().GetPageFromLine(Paragraph.AssociatedCell.CellElementBox.LineInfo);

            if (page != null)
            {
                List<LineInfo> selectedlineinfo = new List<LineInfo>();
                List<TableCellAdv> cellsToSelect = new List<TableCellAdv>();

                foreach (TableRowAdv row in table.Rows)
                {
                    TableCellAdv cellAdv = null;
                    foreach (TableCellAdv cell in row.Cells)
                    {
                        if ((index < (cell.ColumnIndex + cell.ColumnSpan)) && (index >= cell.ColumnIndex))
                        {
                            cellAdv = cell;
                            break;
                        }
                    }
                    if (cellAdv != null)
                    {
                        cellsToSelect.Add(cellAdv);
                        selectedlineinfo.Add(cellAdv.CellElementBox.LineInfo);
                    }
                }

                TextPosition strtpos = new TextPosition(Document);
                TextPosition endpos = new TextPosition(Document);

                foreach (LineInfo l in selectedlineinfo)
                {
                    page.SelectedLines.Add(l);
                }

                foreach (TableCellAdv cell3 in cellsToSelect)
                {
                    if (cellsToSelect.First() == cell3)
                    {
                        BlockAdv first = cell3.Blocks.First();
                        if (first.IsParagraph)
                        {
                            strtpos.Paragraph = first as ParagraphAdv;
                        }
                        else
                        {
                            while (first.IsTable)
                            {
                                first = (first as TableAdv).GetFirstBlockInFirstCell();
                            }
                            strtpos.Paragraph = first as ParagraphAdv;
                        }
                        strtpos.SetIndex("0");
                    }
                    else if (cellsToSelect.Last() == cell3)
                    {
                        BlockAdv last = cellsToSelect.Last().Blocks.Last();
                        if (last.IsTable)
                        {
                            while (last.IsTable)
                            {
                                last = (last as TableAdv).GetLastBlockInLastCell();
                            }
                        }
                        endpos.Paragraph = last as ParagraphAdv;
                        endpos.SetIndex(last.Length());
                    }
                }
                OwnerControl.Selection.Start = strtpos;
                OwnerControl.Selection.End = endpos;
                OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(strtpos, endpos);
                //geo.Figures.Add(cell3.CellElementBox.SelectBox());
                OwnerControl.Selection.Select(strtpos, endpos);
                
                //if (page.SelectionPath == null)
                //{
                //    page.SelectionPath = new Path();
                //    page.SelectionPath.Fill = OwnerControl.Selection.SelectionFillColor;
                //    page.SelectionPath.Stroke = OwnerControl.Selection.SelectionStrokeColor;
                //    page.SelectionPath.Opacity = 0.5;
                //}
                //if (page.SelectionPath != null)
                //{
                //    if (page.SelectionPath != null)
                //        page.SelectionPath.Data = geo;
                //    Paragraph.GetLayoutViewer().IsSelected = true;
                //}
            }
        }

        public void SelectColumn(TableAdv table, int index)
        {
            if (!Paragraph.IsInsideTable)
                return;

            if (table.TableElementBoxes[0].Count <= index)
                index = index - 1;

            double x1 = table.TableElementBoxes[0][index].BoundingRectangle.X;
            double x2 = table.TableElementBoxes[0][index].BoundingRectangle.Right;

            SelectColumn(x1, x2);
        }

        public void SelectColumn()
        {
            if (!Paragraph.IsInsideTable)
                return;

            int columnindex = Paragraph.AssociatedCell.ColumnIndex;

            SelectColumn(columnindex);

        }

        public void SelectTable()
        {
            if (Paragraph.IsInsideTable)
            {
                List<ElementBox> cboxes = new List<ElementBox>();
                PathGeometry geo = new PathGeometry();

                PageAdv page = Paragraph.GetLayoutViewer().GetPageFromLine(Paragraph.AssociatedCell.CellElementBox.LineInfo);
                TableAdv table=Paragraph.AssociatedCell.OwnerTable;

                OwnerControl.Selection.Start = new TextPosition(Document);

                BlockAdv first = table.Rows.First().Cells.First().Blocks.First();

                if (first.IsTable)
                {
                    while (first.IsTable)
                    {
                        first = (first as TableAdv).GetFirstBlockInFirstCell();
                    }
                }
                OwnerControl.Selection.Start.Paragraph = first as ParagraphAdv;
                OwnerControl.Selection.Start.SetIndex("0");

                OwnerControl.Selection.End = new TextPosition(Document);

                BlockAdv last = table.Rows.Last().Cells.Last().Blocks.Last();

                if (last.IsTable)
                {
                    while (last.IsTable)
                    {
                        last = (last as TableAdv).GetLastBlockInLastCell();
                    }
                }
                OwnerControl.Selection.End.Paragraph = last as ParagraphAdv;
                OwnerControl.Selection.End.SetIndex(last.Length());

                OwnerControl.Selection.Select();
                //foreach (TableRowAdv row in Paragraph.AssociatedCell.OwnerTable.Rows)
                //{
                //    foreach (TableCellAdv c in row.Cells)
                //    {
                //        cboxes.Add(c.CellElementBox);
                //    }
                //}

                //foreach (LineInfo l in Paragraph.AssociatedCell.OwnerTable.LineInfo)
                //{
                //    page.SelectedLines.Add(l);
                //}

                //cboxes.ForEach(c => geo.Figures.Add((c as TableCellElementBox).SelectBox()));

                //if (page.SelectionPath == null)
                //{
                //    page.SelectionPath = new Path();
                //    page.SelectionPath.Fill = OwnerControl.Selection.SelectionFillColor;
                //    page.SelectionPath.Stroke = OwnerControl.Selection.SelectionStrokeColor;
                //    page.SelectionPath.Opacity = 0.5;
                //}
                //if (page.SelectionPath != null)
                //{
                //    if (page.SelectionPath != null)
                //        page.SelectionPath.Data = geo;
                //    Paragraph.GetLayoutViewer().IsSelected = true;
                //}
            }
        }

        public void DeleteTable()
        {
            if (!Paragraph.IsInsideTable)
                return;

            bool canarrange = false;

            TableAdv tableparent = Paragraph.AssociatedCell.OwnerTable;
            BlockAdv nextblock = Paragraph.AssociatedCell.OwnerTable.NextBlock;
            BlockAdv currentblock = null;

            if (tableparent != null)
            {
                if (tableparent.NextBlock != null && tableparent.PreviousBlock != null)
                {
                    tableparent.PreviousBlock.NextBlock = tableparent.NextBlock;
                    currentblock = tableparent.PreviousBlock.NextBlock;
                }
                tableparent.ClearLines();

                if (tableparent.IsInsideTable)
                {
                    tableparent.AssociatedCell.Blocks.Remove(tableparent);
                }
                else
                {
                    tableparent.Section.Blocks.Remove(tableparent);
                }

                OwnerControl.Viewer.SetPreviousBlocks();

                OwnerControl.Viewer.SetIsArrangedToFalse();

                tableparent.PreviousBlock.ArrangeElements();

                OwnerControl.Viewer.SetIsArrangedToFalse();

                if (currentblock != null)
                {
                    BlockAdv tempblk = currentblock;
                    while (currentblock.IsTable)
                    {
                        tempblk = (tempblk as TableAdv).GetFirstBlockInFirstCell();
                    }
                    TextPosition.Paragraph = tempblk as ParagraphAdv;
                }

                TextPosition.SetIndex("0");

                canarrange = true;
            }
            if (canarrange)
            {
                InvalidateVisibleRegion();
            }
        }

        internal void DeleteRow(SelectedCellsInfo selected)
        {
            if (!Paragraph.IsInsideTable)
                return;

            TableRowAdv currentrow = Paragraph.AssociatedCell.OwnerRow;

            
        }

        internal void DeleteRow(TableRowAdv rowtodelete)
        {
            TableAdv tableparent=null;
            bool canarrange = false;
            BlockAdv currentblock = null;
            
            if (rowtodelete.Owner.Rows.Count == 1)
            {
                tableparent = rowtodelete.Owner;
                if (tableparent.NextBlock != null && tableparent.PreviousBlock != null)
                {
                    tableparent.PreviousBlock.NextBlock = tableparent.NextBlock;
                    currentblock = tableparent.PreviousBlock.NextBlock;
                }
                tableparent.ClearLines();

                if (tableparent.IsInsideTable)
                {
                    tableparent.AssociatedCell.Blocks.Remove(tableparent);
                }
                else
                {
                    tableparent.Section.Blocks.Remove(tableparent);
                }

                OwnerControl.Viewer.SetPreviousBlocks();

                OwnerControl.Viewer.SetIsArrangedToFalse();

                tableparent.PreviousBlock.ArrangeElements();

                OwnerControl.Viewer.SetIsArrangedToFalse();

                if (currentblock != null)
                {
                    BlockAdv tempblk = currentblock;
                    while (currentblock.IsTable)
                    {
                        tempblk = (tempblk as TableAdv).GetFirstBlockInFirstCell();
                    }
                    TextPosition.Paragraph = tempblk as ParagraphAdv;
                }

                TextPosition.SetIndex("0");

                canarrange = true;
            }
            else
            {
                bool lastrow = false;
                if (rowtodelete != null)
                {
                    tableparent = rowtodelete.Owner;
                    List<TableCellAdv> list1 = new List<TableCellAdv>();
                        
                    tableparent.GetRowSpannedCellsIntersectingWithGivenRow(rowtodelete,ref list1);

                    foreach (TableCellAdv cell2 in list1)
                    {
                        if (cell2.HasRowSpan())
                        {
                            cell2.RowSpan--;
                        }
                    }

                    TableRowAdv nextrow = null;
                    if (tableparent.Rows.Last() == rowtodelete)
                    {
                        lastrow = true;
                    }
                    else
                    {
                        nextrow=rowtodelete.Owner.Rows[rowtodelete.Owner.Rows.IndexOf(rowtodelete) + 1];
                    }

                    if (tableparent.Rows.Contains(rowtodelete))
                    {
                        tableparent.Rows.Remove(rowtodelete);
                    }

                    tableparent.MeasureElements();

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    tableparent.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    if (lastrow)
                    {
                        BlockAdv nextblk = rowtodelete.Owner.NextBlock;
                        if (nextblk.IsParagraph)
                        {
                            TextPosition.Paragraph = nextblk as ParagraphAdv;
                            TextPosition.SetIndex("0");
                        }
                        else if (nextblk.IsTable)
                        {
                            while (nextblk.IsTable)
                            {
                                nextblk = (nextblk as TableAdv).GetFirstBlockInFirstCell();
                            }
                            TextPosition.Paragraph = nextblk as ParagraphAdv;
                            TextPosition.SetIndex("0");
                        }
                    }
                    else
                    {
                        if (nextrow.Cells.Count > 0)
                        {
                            BlockAdv firstblk = nextrow.Cells.First().Blocks.First();
                            if (firstblk.IsParagraph)
                            {
                                TextPosition.Paragraph = (ParagraphAdv)firstblk;
                                TextPosition.SetIndex("0");
                            }
                            else if (firstblk.IsTable)
                            {
                                while (firstblk.IsTable)
                                {
                                    firstblk = (firstblk as TableAdv).GetFirstBlockInFirstCell();
                                }
                                TextPosition.Paragraph = (ParagraphAdv)firstblk;
                                TextPosition.SetIndex("0");
                            }
                        }
                    }
                    canarrange = true;
                }
            }

            if (canarrange)
            {
                InvalidateVisibleRegion();
            }
        }

        public void DeleteColumn()
        {
            if (!Paragraph.IsInsideTable)
                return;

            TableAdv parent = Paragraph.AssociatedCell.OwnerTable;
            int mainIndex = Paragraph.AssociatedCell.ColumnIndex;

            DeleteColumn(parent, mainIndex);
        }

        public void DeleteColumn(TableAdv table, int indextodelete)
        {
            bool canarrange = false;
            BlockAdv currentblock = null;
            if (table != null)
            {
                int count = table.TableHolder.Columns.Count;
                if (count == 1)
                {
                    if (table.NextBlock != null)
                    {
                        table.PreviousBlock.NextBlock = table.NextBlock;
                    }
                    else if (table.PreviousBlock != null)
                    {
                        currentblock = table.PreviousBlock.NextBlock;
                    }
                    table.ClearLines();

                    if (table.IsInsideTable)
                    {
                        table.AssociatedCell.Blocks.Remove(table);
                    }
                    else
                    {
                        table.Section.Blocks.Remove(table);
                    }

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    table.PreviousBlock.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    if (currentblock != null)
                    {
                        BlockAdv tempblk = currentblock;
                        while (currentblock.IsTable)
                        {
                            tempblk = (tempblk as TableAdv).GetFirstBlockInFirstCell();
                        }
                        TextPosition.Paragraph = tempblk as ParagraphAdv;
                    }

                    TextPosition.SetIndex("0");

                    canarrange = true;
                }
                else
                {
                    if (count != 0)
                    {
                        List<TableCellAdv> templist = new List<TableCellAdv>();

                        for (int r = 0; r < table.Rows.Count; r++)
                        {
                            TableRowAdv row = table.Rows[r];

                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                TableCellAdv cell = row.Cells[i];

                                if (row.Cells.Count == 1 && cell.ColumnSpan <=1)
                                {
                                    DeleteRow(row);
                                    continue;
                                }
                                if (cell.ColumnIndex > indextodelete || (cell.ColumnIndex + cell.ColumnSpan) <= indextodelete)
                                    continue;

                                templist.Add(cell);
                            }
                        }

                        foreach (TableCellAdv c in templist)
                        {
                            if (c.ColumnSpan > 1)
                            {
                                c.ColumnSpan--;
                            }
                            else
                            {
                                c.OwnerRow.Cells.Remove(c);
                            }
                        }
                    }

                    table.MeasureElements();

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    table.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    canarrange = true;
                }
                if (canarrange)
                    InvalidateVisibleRegion();
            }
        }

        internal void MergeSelectedCells()
        {
            TableRowAdv startrow=null;
            TableRowAdv endrow=null;
            TableCellAdv startcell=null;
            TableCellAdv endcell=null;
            bool canarrange = false;

            MergedCellsHistory history = new MergedCellsHistory();

            if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
            {
                history.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                history.EndPosition = OwnerControl.Selection.End.CopyForHistory();
            }
            history.Action = Actions.Merging;

            if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
            {
                BlockAdv startblk = Document.GetBlockFromVirtualPosition(OwnerControl.Selection.Start.VirtualPosition, ref startrow, ref startcell);

                BlockAdv endblk = Document.GetBlockFromVirtualPosition(OwnerControl.Selection.End.VirtualPosition, ref endrow, ref endcell);
                
                if (startblk != null && endblk != null)
                {
                    if (startblk != endblk)
                        return;
                    else if (startblk.IsTable && endblk.IsTable)
                    {
                        if (startcell != null)
                        {
                            MergeSelectedCells(startblk, startcell,history);
                        }
                    }
                    canarrange = true;
                }
                if (canarrange)
                {
                    OwnerControl.Viewer.SetIsArrangedToFalse();
                    OwnerControl.Viewer.SetVisibleLinesToPage();
                    OwnerControl.Viewer.SetIsArrangedToFalse();
                    OwnerControl.Viewer.CheckForCursorVisibility(false);
                    SelectCell(startcell);
                }
                OwnerControl.History.RecordUndo(history);
            }
        }

        public void MergeSelectedCells(BlockAdv startblock,TableCellAdv startcell,MergedCellsHistory history)
        {
            history.AffectedColumnSpan = startcell.ColumnSpan;
            history.AffectedRowSpan = startcell.RowSpan;

            TableAdv table = startblock as TableAdv;

            int minrowindex = 0x7f;
            int maxrowspanwithindex = 0;
            int mincolumnindex = 0x7f;
            int maxcolumnspanwithindex = 0;
            int rowSpan = 0;
            int columnSpan = 0;

            List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                       
            foreach (TableCellAdv cell in selectedcells)
            {
                mincolumnindex = Math.Min(mincolumnindex, cell.ColumnIndex);
                maxcolumnspanwithindex = Math.Max(maxcolumnspanwithindex, (cell.ColumnIndex + cell.ColumnSpan) - 1);
                minrowindex = Math.Min(minrowindex, cell.RowIndex);
                maxrowspanwithindex = Math.Max(maxrowspanwithindex, (cell.RowIndex + cell.RowSpan) - 1);
            }

            rowSpan = (maxrowspanwithindex - minrowindex) + 1;
            columnSpan = (maxcolumnspanwithindex - mincolumnindex) + 1;

            startcell.ColumnSpan = columnSpan;
            startcell.RowSpan = rowSpan;

            foreach (TableCellAdv cell2 in selectedcells)
            {
                PreservedCellsInfo preservedcell = new PreservedCellsInfo();
                preservedcell.RowIndex = cell2.RowIndex;
                preservedcell.ColumnIndex = cell2.ColumnIndex;
                history.MergedCellsInfo.Add(preservedcell);
                
                foreach (BlockAdv b in cell2.Blocks)
                {
                    preservedcell.Blocks.Add(b.CopyBlock());
                }

                if (selectedcells.First() == cell2)
                    continue;
                else
                {
                    preservedcell.TableCell = cell2.CreateNewCell(false, false);
                    foreach (BlockAdv b in cell2.Blocks)
                    {
                        if (b.IsParagraph)
                        {
                            if (b.Inlines != null && b.Inlines.Count != 0)
                            {
                                startcell.Blocks.Add(b.CopyBlock());
                            }
                        }
                        else if (b.IsTable)
                        {
                            startcell.Blocks.Add(b.CopyBlock());
                        }
                    }
                }
            }

            int k = 0; int ct = 1;
            foreach (TableRowAdv row in table.Rows)
            {
                k = 0;
                while (k < row.Cells.Count)
                {
                    TableCellAdv c = row.Cells[k];
                    if (ct < selectedcells.Count)
                    {
                        if (c == selectedcells[ct])
                        {
                            row.Cells.RemoveAt(k);
                            ct++;
                            continue;
                        }
                    }
                    k++;
                }
            }

            for (int r = 0; r < table.Rows.Count; r++)
            {
                TableRowAdv row3 = table.Rows[r];

                if (row3.Cells.Count == 0)
                {
                    List<TableCellAdv> intersectedcells = new List<TableCellAdv>();
                    table.GetRowSpannedCellsIntersectingWithGivenRow(row3,ref intersectedcells);
                    List<TableRowAdv> rows = new List<TableRowAdv>();
                    rows.Add(row3.CopyFromGivenRow(false, false));
                    DeletedRowHistory rowhistory = new DeletedRowHistory(r,rows);
                    rowhistory.RowSpanAffectedCells = intersectedcells;
                    history.DeletedRowsHistory.Add(rowhistory);
                }
            }


            List<TableRowAdv> rowToDelete = new List<TableRowAdv>();

            foreach (TableRowAdv row2 in table.Rows)
            {
                if (row2.Cells.Count == 0)
                {
                    rowToDelete.Add(row2);
                }
            }

            foreach (TableRowAdv row3 in rowToDelete)
            {
                if (row3.Owner.Rows.Count == 1)
                {
                    TableAdv tableparent = row3.Owner;
                    if (tableparent.NextBlock != null && tableparent.PreviousBlock != null)
                    {
                        tableparent.PreviousBlock.NextBlock = tableparent.NextBlock;
                    }
                    tableparent.ClearLines();

                    if (tableparent.IsInsideTable)
                    {
                        tableparent.AssociatedCell.Blocks.Remove(tableparent);
                    }
                    else
                    {
                        tableparent.Section.Blocks.Remove(tableparent);
                    }
                }
                else
                {
                    if (row3 != null)
                    {
                        TableAdv currentTable = row3.Owner;
                        List<TableCellAdv> list1 = new List<TableCellAdv>();
                        currentTable.GetRowSpannedCellsIntersectingWithGivenRow(row3,ref list1);

                        foreach (TableCellAdv cell2 in list1)
                        {
                            if (cell2.HasRowSpan())
                            {
                                cell2.RowSpan--;
                            }
                        }

                        if (currentTable.Rows.Contains(row3))
                        {
                            currentTable.Rows.Remove(row3);
                        }
                    }
                }
            }

            startblock.MeasureElements();
            OwnerControl.Viewer.SetPreviousBlocks();
            OwnerControl.Viewer.SetIsArrangedToFalse();
            startblock.ArrangeElements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraph"></param>
        public void UpdateParagraphStyle(ParagraphAdv paragraph)
        {
            if (paragraph != null && ParagraphStyle != null)
            {
                ParagraphStyle.AfterSpacing = paragraph.AfterSpacing;
                ParagraphStyle.BeforeSpacing = paragraph.BeforeSpacing;
                ParagraphStyle.LeftIndent = paragraph.LeftIndent;
                ParagraphStyle.LineSpacing = paragraph.LineSpacing;
                ParagraphStyle.RightIndent = paragraph.RightIndent;
                ParagraphStyle.TextAlignment = paragraph.TextAlignment;
                ParagraphStyle.ListType = paragraph.ListType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SelectWord()
        {
            if (TextPosition.Paragraph != null)
            {
                int indexInInline = 0;
                int indexInElementBox = 0;
                Inline inline = GetInlineFromTextPosition(ref indexInInline);
                ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
                double start = 0;
                double end = 0;
                TextPosition.Paragraph.GetIndexOfWord(elementBox, ref start, ref end);
                OwnerControl.Selection.Start = new TextPosition(Document) { Paragraph = TextPosition.Paragraph };
                OwnerControl.Selection.Start.SetIndex(start.ToString());
                OwnerControl.Selection.End = new TextPosition(Document) { Paragraph = TextPosition.Paragraph };
                OwnerControl.Selection.End.SetIndex(end.ToString());
                OwnerControl.Selection.Select();
            }
        }

        internal void Delete()
        {
            DeletedInline = null;
            int indexInInline = 0;
            int indexInElementBox = 0;
            int removeAt = 0;
            Inline removeFromInline = null;
            Inline inline = GetInlineFromTextPosition(ref indexInInline);
            removeAt = indexInInline;
            removeFromInline = inline;
            ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
            bool arrange = false;
            if (elementBox != null)
            {
                bool deleteFromNextInline = elementBox.IsUIBox || elementBox.IsImageBox ? true : inline.InternalText.Length == indexInInline;

                if (deleteFromNextInline && !TextPosition.IsPositionAtParagraphStart)
                {
                    if (elementBox.NextElementBox != null && elementBox.NextElementBox.Inline != null)
                    {
                        removeFromInline = elementBox.NextElementBox.Inline;
                        removeAt = 1;
                        arrange = true;
                        RemoveAt(removeAt, removeFromInline);
                    }
                    else if (Paragraph.NextBlock != null)
                    {
                        if (Paragraph.NextBlock.IsParagraph)
                        {
                            if (Paragraph.NextBlock.Inlines.Count == 0)
                            {
                                removeFromInline = null;
                                goto MergeParagraph;
                            }

                            removeFromInline = Paragraph.NextBlock.Inlines[0];
                            removeAt = 1;
                        MergeParagraph:
                            arrange = true;
                            MergeTwoParagraphs(Paragraph, Paragraph.NextBlock);
                        }
                        else if (Paragraph.NextBlock.IsTable)
                        {
                            BlockAdv firstblock = (Paragraph.NextBlock as TableAdv).GetFirstBlockInFirstCell();
                            while (firstblock.IsTable)
                            {
                                firstblock = (firstblock as TableAdv).GetFirstBlockInFirstCell();
                            }
                            MergeCurrentParagraphToCellBlocks(Paragraph, firstblock);
                            TextPosition.Paragraph = firstblock as ParagraphAdv;
                            TextPosition.SetIndex(firstblock.Length());
                            arrange = true;
                        }
                        //RemoveAt(removeAt, removeFromInline);
                    }
                }
                else
                {
                    removeAt += 1;
                    arrange = true;
                    RemoveAt(removeAt, removeFromInline);
                }
            }
            else if (Paragraph != null && Paragraph.Inlines != null && Paragraph.Inlines.Count == 0)
            {
                if (Paragraph.NextBlock != null)
                {
                    Paragraph.ClearLines();
                    if (Paragraph.IsInsideTable)
                    {
                        Paragraph.AssociatedCell.Blocks.Remove(Paragraph);
                    }
                    else
                    {
                        Paragraph.Section.Blocks.Remove(Paragraph);
                    }
                }
                if (Paragraph.PreviousBlock != null && Paragraph.NextBlock != null)
                {
                    Paragraph.PreviousBlock.NextBlock = Paragraph.NextBlock;
                    Paragraph.NextBlock.PreviousBlock = Paragraph.PreviousBlock;
                    if (Paragraph.NextBlock.IsTable)
                    {
                        BlockAdv blk = (Paragraph.NextBlock as TableAdv).GetFirstBlockInFirstCell();
                        if (blk is TableAdv)
                        {
                            blk = (blk as TableAdv).GetFirstBlockInFirstCell();
                        }
                        TextPosition.Paragraph = blk as ParagraphAdv;
                    }
                    else if (Paragraph.NextBlock.IsParagraph)
                    {
                        TextPosition.Paragraph = Paragraph.NextBlock as ParagraphAdv;
                    }
                    TextPosition.SetIndex("0");
                    if (Paragraph != null)
                    {
                        Paragraph.ArrangeElements(false);
                        InvalidateVisibleRegion();
                    }
                }
                else if (Paragraph.PreviousBlock == null && Paragraph.NextBlock != null)
                {
                    Paragraph.NextBlock.PreviousBlock = null;
                    if (Paragraph.NextBlock.IsTable)
                    {
                        BlockAdv blk = (Paragraph.NextBlock as TableAdv).GetFirstBlockInFirstCell();
                        if (blk.IsTable)
                        {
                            blk = (blk as TableAdv).GetFirstBlockInFirstCell();
                        }
                        TextPosition.Paragraph = blk as ParagraphAdv;
                    }
                    else if (Paragraph.NextBlock.IsParagraph)
                    {
                        TextPosition.Paragraph = Paragraph.NextBlock as ParagraphAdv;
                    }
                    TextPosition.SetIndex("0");
                    if (Paragraph != null)
                    {
                        Paragraph.ArrangeElements(false);
                        InvalidateVisibleRegion();
                    }
                }
            }
            if (arrange)
            {
                int index = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
                Paragraph.ArrangeElements(index);
                if (Paragraph.NextBlock != null && !Paragraph.NextBlock.IsArranged)
                {
                    Paragraph.NextBlock.ArrangeElements();
                }
                InvalidateVisibleRegion();
            }
        }

        internal void InsertBlocks(BlockCollection<BlockAdv> blocks)
        {
            List<BlockAdv> affectedblks = new List<BlockAdv>();
            ParagraphAdv paraToPos = null;
            if (TextPosition.Paragraph != null)
            {
                if (!TextPosition.Paragraph.IsInsideTable)
                {
                    SectionAdv section = TextPosition.Paragraph.Section;
                    int index = section.Blocks.IndexOf(TextPosition.Paragraph);
                    foreach (BlockAdv blk in blocks)
                    {
                        if (blk is ParagraphAdv)
                        {
                            ParagraphAdv tempPara = (blk as ParagraphAdv).CreateNewParagraph();
                            tempPara.IsInsideTable = TextPosition.Paragraph.IsInsideTable;
                            tempPara.AssociatedCell = TextPosition.Paragraph.AssociatedCell;
                            tempPara.Section = TextPosition.Paragraph.Section;
                            tempPara.LayoutViewer = TextPosition.Paragraph.LayoutViewer;
                            section.Blocks.AddBlockAtIndex(index++, tempPara);
                            affectedblks.Add(tempPara);
                            foreach (Inline inline in blk.Inlines)
                            {
                                Inline tempInline = inline.CreatInline();
                                tempInline.InternalText = inline.InternalText;
                                tempInline.Paragraph = tempPara;
                                tempInline.MeasureElements();
                                tempPara.Inlines.Add(tempInline);
                            }
                            tempPara.LinkElementBoxes();
                            if (blocks.Last() == blk)
                            {
                                paraToPos = tempPara;
                            }
                        }
                        else
                        {
                            TableAdv table = blk.CreateBlock() as TableAdv;
                            table.IsInsideTable = TextPosition.Paragraph.IsInsideTable;
                            table.AssociatedCell = TextPosition.Paragraph.AssociatedCell;
                            table.Section = TextPosition.Paragraph.Section;
                            table.LayoutViewer = TextPosition.Paragraph.LayoutViewer;
                            foreach (TableRowAdv row in (blk as TableAdv).Rows)
                            {
                                TableRowAdv newrow = row.CreateNewRow();
                                foreach (TableCellAdv cell2 in row.Cells)
                                {
                                    TableCellAdv newcell = cell2.CreateNewCell(false,false);
                                    foreach (BlockAdv b in cell2.Blocks)
                                    {
                                        if (cell2.Blocks.Last() == b)
                                        {
                                            if (cell2.Blocks.Last().IsParagraph)
                                            {
                                                paraToPos = cell2.Blocks.Last() as ParagraphAdv;
                                            }
                                            else
                                            {
                                                BlockAdv lasblk = (cell2.Blocks.Last() as TableAdv).GetLastBlockInLastCell();
                                                while (lasblk.IsTable)
                                                {
                                                    lasblk = (lasblk as TableAdv).GetLastBlockInLastCell();
                                                }
                                                paraToPos = lasblk as ParagraphAdv;
                                            }
                                        }
                                        newcell.Blocks.Add(b.CopyBlock());
                                    }
                                    newrow.Cells.Add(newcell);
                                }
                                table.Rows.Add(newrow);
                            }
                            table.MeasureElements();
                            section.Blocks.AddBlockAtIndex(index++, table);
                            affectedblks.Add(table);
                        }
                    }
                }
                else
                {
                    TableCellAdv parentcell = TextPosition.Paragraph.AssociatedCell;
                    int index = parentcell.Blocks.IndexOf(TextPosition.Paragraph);

                    foreach (BlockAdv b in blocks)
                    {
                        if (b is ParagraphAdv)
                        {
                            ParagraphAdv tempPara = (b as ParagraphAdv).CreateNewParagraph();
                            tempPara.IsInsideTable = TextPosition.Paragraph.IsInsideTable;
                            tempPara.AssociatedCell = TextPosition.Paragraph.AssociatedCell;
                            tempPara.LayoutViewer = TextPosition.Paragraph.LayoutViewer;
                            parentcell.Blocks.AddBlockAtIndex(index++, tempPara);
                            affectedblks.Add(tempPara);
                            foreach (Inline inline in b.Inlines)
                            {
                                Inline tempInline = inline.CreatInline();
                                tempInline.InternalText = inline.InternalText;
                                tempInline.Paragraph = tempPara;
                                tempInline.MeasureElements();
                                tempPara.Inlines.Add(tempInline);
                            }
                            tempPara.LinkElementBoxes();
                            if (blocks.Last() == b)
                            {
                                paraToPos = tempPara;
                            }
                        }
                        else
                        {
                            TableAdv table = b.CreateBlock() as TableAdv;
                            table.IsInsideTable = TextPosition.Paragraph.IsInsideTable;
                            table.AssociatedCell = TextPosition.Paragraph.AssociatedCell;
                            table.Section = TextPosition.Paragraph.Section;
                            table.LayoutViewer = TextPosition.Paragraph.LayoutViewer;
                            foreach (TableRowAdv row in (b as TableAdv).Rows)
                            {
                                TableRowAdv newrow = row.CreateNewRow();
                                foreach (TableCellAdv cell2 in row.Cells)
                                {
                                    TableCellAdv newcell = cell2.CreateNewCell(false,false);
                                    foreach (BlockAdv block in cell2.Blocks)
                                    {
                                        if (cell2.Blocks.Last() == block)
                                        {
                                            if (cell2.Blocks.Last().IsParagraph)
                                            {
                                                paraToPos = cell2.Blocks.Last() as ParagraphAdv;
                                            }
                                            else
                                            {
                                                BlockAdv lasblk = (cell2.Blocks.Last() as TableAdv).GetLastBlockInLastCell();
                                                while (lasblk.IsTable)
                                                {
                                                    lasblk = (lasblk as TableAdv).GetLastBlockInLastCell();
                                                }
                                                paraToPos = lasblk as ParagraphAdv;
                                            }
                                        }
                                        newcell.Blocks.Add(block.CopyBlock());
                                    }
                                    newrow.Cells.Add(newcell);
                                }
                                table.Rows.Add(newrow);
                            }
                            table.MeasureElements();
                            parentcell.Blocks.AddBlockAtIndex(index++, table);
                            affectedblks.Add(table);
                        }
                    }
                }
                
                OwnerControl.Viewer.SetPreviousBlocks();

                foreach (BlockAdv blk in affectedblks)
                {
                    blk.MeasureElements();
                }

                foreach (BlockAdv blk in affectedblks)
                {
                    if (!blk.IsArranged)
                    {
                        blk.ArrangeElements();
                    }

                    blk.IsArranged = false;
                }

                if (paraToPos != null)
                {
                    TextPosition pos = new TextPosition(OwnerControl.Document);
                    pos.Paragraph = paraToPos;
                    pos.SetIndex(paraToPos.Length());
                    TextPosition = pos;
                }

                InvalidateVisibleRegion();
            }
        }

        internal bool BackSpace()
        {
            DeletedInline = null;
            int indexInInline = 0;
            int indexInElementBox = 0;
            int removeAt = 0;
            Inline removeFromInline = null;
            Inline inline = GetInlineFromTextPosition(ref indexInInline);
            removeAt = indexInInline;
            removeFromInline = inline;
            ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
            bool arrange = false;

            if (elementBox != null && !TextPosition.IsZeroIndex())
            {
                RemoveAt(removeAt, inline);
                TextPosition.Index = TextPosition.StepDown(1);
                arrange = true;
                int index = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
                Paragraph.ArrangeElements(index);
                if (Paragraph.NextBlock != null && !Paragraph.NextBlock.IsArranged)
                {
                    Paragraph.NextBlock.ArrangeElements();
                }
            }
            else if (Paragraph != null && TextPosition.IsZeroIndex() && Paragraph.ListType != ListType.None)
            {
                Paragraph.ListType = ListType.None;
                Paragraph.ArrangeElements();
                arrange = true;
            }
            else if (Paragraph != null && TextPosition.IsZeroIndex() && Paragraph.PreviousBlock != null)
            {
                if (!Paragraph.PreviousBlock.IsTable)
                {
                    if (Paragraph.PreviousBlock.Inlines.Count == 0)
                    {
                        TextPosition.SetIndex("0");
                    }
                    else
                    {
                        int index = Paragraph.PreviousBlock.Inlines.Count - 1;
                        Inline lastInline = Paragraph.PreviousBlock.Inlines[index];
                        int spanIndex = lastInline.IsImageContainer || lastInline.IsUIContainer ? 1 : lastInline.InternalText.Length;
                        TextPosition.SetIndex(Paragraph.PreviousBlock.GetIndexFromInlineAndSpanIndex(lastInline, spanIndex).ToString());
                    }
                    MergeTwoParagraphs(Paragraph.PreviousBlock, Paragraph);
                    TextPosition.Paragraph = Paragraph.PreviousBlock as ParagraphAdv;
                    if (Paragraph.LineInfo.Count > 0)
                    {
                        int lineIndex = Paragraph.LineInfo.Count - 1;
                        LineInfo line = Paragraph.LineInfo[lineIndex];
                        lineIndex = Paragraph.LineInfo.IndexOf(line) == 0 ? 0 : Paragraph.LineInfo.IndexOf(line) - 1;
                        Paragraph.ArrangeElements(lineIndex);
                    }
                    arrange = true;
                }
                else
                    return arrange;
            }
            if (arrange)
            {
                InvalidateVisibleRegion();
            }

            return arrange;
        }

        internal void Enter()
        {
            int indexInInline = 0;
            int indexInElementBox = 0;
            Inline inline = GetInlineFromTextPosition(ref indexInInline);
            ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
            ParagraphAdv newPara = null;
            bool arrange = false;
            int lineIndex = 0;
            if (elementBox != null)
            {
                lineIndex = Paragraph.LineInfo.IndexOf(elementBox.LineInfo) == 0 ? 0 : Paragraph.LineInfo.IndexOf(elementBox.LineInfo) - 1;
            }

            if (inline is SpanAdv)
            {
                HandleURLText(inline, indexInInline);
            }

            if (!SplitAt(TextPosition.Index, Paragraph, ref newPara))
            {
                if (TextPosition.IsZeroIndex())
                {
                    if (Paragraph.ListType != ListType.None && Paragraph.Inlines.Count == 0)
                    {
                        TextPosition.SetIndex("0");
                        Paragraph.ListType = ListType.None;
                        Paragraph.ArrangeElements();
                    }
                    else
                    {
                        newPara = Paragraph.CreateNewParagraph();
                        newPara.Margin = Paragraph.Margin;
                        if (Paragraph.IsInsideTable)
                        {
                            newPara.IsInsideTable = true;
                            newPara.AssociatedCell = Paragraph.AssociatedCell;
                            int parIndex = Paragraph.AssociatedCell.Blocks.IndexOf(Paragraph);
                            Paragraph.AssociatedCell.Blocks.AddBlockAtIndex(parIndex, newPara);
                        }
                        else
                        {
                            newPara.LayoutViewer = Paragraph.LayoutViewer;
                            newPara.Section = Paragraph.Section;
                            int paraIndex = Paragraph.Section.Blocks.IndexOf(Paragraph);
                            Paragraph.Section.Blocks.AddBlockAtIndex(paraIndex, newPara);
                        }
                        newPara.PreviousBlock = Paragraph.PreviousBlock;
                        newPara.NextBlock = Paragraph;
                        if (Paragraph.PreviousBlock != null)
                        {
                            Paragraph.PreviousBlock.NextBlock = newPara;
                        }
                        Paragraph.PreviousBlock = newPara;
                        TextPosition.SetIndex("0");
                        newPara.ArrangeElements();
                    }
                    arrange = true;
                }
                else if (inline != null)
                {
                    bool isEnd = Paragraph.Inlines.Last() == inline;
                    isEnd = isEnd & inline.IsUIContainer || inline.IsImageContainer ? indexInInline == 1 : indexInInline == inline.InternalText.Length;
                    if (isEnd)
                    {
                        newPara = Paragraph.CreateNewParagraph();
                        newPara.Margin = Paragraph.Margin;
                        if (Paragraph.IsInsideTable)
                        {
                            newPara.IsInsideTable = true;
                            newPara.AssociatedCell = Paragraph.AssociatedCell;
                            int index = Paragraph.AssociatedCell.Blocks.IndexOf(Paragraph) + 1;
                            Paragraph.AssociatedCell.Blocks.AddBlockAtIndex(index, newPara);
                        }
                        else
                        {
                            newPara.Section = Paragraph.Section;
                            newPara.LayoutViewer = Paragraph.GetLayoutViewer();
                            int paraIndex = Paragraph.Section.Blocks.IndexOf(Paragraph) + 1;
                            Paragraph.Section.Blocks.AddBlockAtIndex(paraIndex, newPara);
                        }
                        if (Paragraph.NextBlock != null)
                        {
                            Paragraph.NextBlock.PreviousBlock = newPara;
                        }
                        newPara.PreviousBlock = Paragraph;
                        newPara.NextBlock = Paragraph.NextBlock;
                        Paragraph.ArrangeElements(lineIndex);
                        if (Paragraph.NextBlock != null && !Paragraph.NextBlock.IsArranged)
                        {
                            Paragraph.NextBlock.ArrangeElements();
                        }
                        TextPosition.Paragraph = newPara;
                        TextPosition.SetIndex("0");
                        arrange = true;
                    }
                }
            }
            else
            {
                newPara.LayoutViewer = Paragraph.GetLayoutViewer();
                if (Paragraph.IsInsideTable)
                {
                    newPara.IsInsideTable = true;
                    newPara.AssociatedCell = Paragraph.AssociatedCell;
                }
                Paragraph.ArrangeElements(lineIndex);
                if (Paragraph.NextBlock != null && !Paragraph.NextBlock.IsArranged)
                {
                    Paragraph.NextBlock.ArrangeElements();
                }
                TextPosition.Paragraph = newPara;
                TextPosition.SetIndex("0");
                arrange = true;
            }

            if (arrange)
            {
                InvalidateVisibleRegion();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void InvalidateVisibleRegion()
        {
            OwnerControl.Viewer.SetIsArrangedToFalse();
            OwnerControl.Viewer.SetVisibleLinesToPage();
            OwnerControl.Viewer.SetIsArrangedToFalse();
            PositionCursor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="inline"></param>
        internal void RemoveAt(int index, Inline inline)
        {
            if (inline != null)
            {
                DeletedInline = inline.CreatInline();
                bool isImageOrUI = inline.IsImageContainer || inline.IsUIContainer;
                if (isImageOrUI && index == 1)
                {
                    inline.ElementBoxes.Clear();
                    if (inline.Paragraph != null)
                    {
                        inline.Paragraph.Inlines.Remove(inline);
                        inline.Paragraph.LinkElementBoxes();
                    }
                }
                else
                {
                    int indexInElementBox = 0;
                    ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, index, ref indexInElementBox);
                    if (elementBox != null)
                    {
                        DeletedInline.InternalText = inline.InternalText.Substring(Convert.ToInt32(index - 1), 1);
                        inline.InternalText = inline.InternalText.Substring(0, Convert.ToInt32(index - 1)) + inline.InternalText.Substring(Convert.ToInt32(index));
                        elementBox.InternalText = elementBox.InternalText.Substring(0, Convert.ToInt32(indexInElementBox - 1)) + elementBox.InternalText.Substring(Convert.ToInt32(indexInElementBox));
                        if (string.IsNullOrEmpty(elementBox.InternalText) && !inline.IsImageContainer && !inline.IsUIContainer)
                        {
                            inline.ElementBoxes.Remove(elementBox);
                            if (elementBox.PreviousElementBox != null)
                                elementBox.PreviousElementBox.NextElementBox = elementBox.NextElementBox;
                            if (elementBox.NextElementBox != null)
                                elementBox.NextElementBox.PreviousElementBox = elementBox.PreviousElementBox;
                        }
                        if (string.IsNullOrEmpty(inline.InternalText) && !inline.IsImageContainer && !inline.IsUIContainer && inline.Paragraph != null)
                        {
                            inline.Paragraph.Inlines.Remove(inline);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Merge first paragraph with second paragraph
        /// </summary>
        /// <param name="firstParagraph"></param>
        /// <param name="secondParagraph"></param>
        internal void MergeTwoParagraphs(BlockAdv firstParagraph, BlockAdv secondParagraph)
        {
            if (firstParagraph != null && secondParagraph != null)
            {
                foreach (Inline inline in secondParagraph.Inlines)
                {
                    inline.Paragraph = firstParagraph as ParagraphAdv;
                    firstParagraph.Inlines.Add(inline);
                }

                firstParagraph.LinkElementBoxes();
                secondParagraph.Inlines.Clear();
                secondParagraph.ClearLines();
                firstParagraph.NextBlock = secondParagraph.NextBlock;
                if (firstParagraph.NextBlock != null)
                {
                    firstParagraph.NextBlock.PreviousBlock = firstParagraph;
                }
                if (!secondParagraph.IsInsideTable)
                {
                    secondParagraph.Section.Blocks.Remove(secondParagraph);
                }
                else
                {
                    secondParagraph.AssociatedCell.Blocks.Remove(secondParagraph);
                }
            }
        }

        internal void MergeCurrentParagraphToCellBlocks(BlockAdv first, BlockAdv second)
        {
            if (first != null && second != null)
            {
                for (int i = first.Inlines.Count - 1; i >= 0; i--)
                {
                    Inline firstinline = first.Inlines[i];
                    firstinline.Paragraph = second as ParagraphAdv;
                    second.Inlines.Insert(0, firstinline);
                }
                second.LinkElementBoxes();
                first.Inlines.Clear();
                first.ClearLines();
                BlockAdv ownertable = second.AssociatedCell.OwnerTable;
                while (ownertable.AssociatedCell != null)
                {
                    ownertable = ownertable.AssociatedCell.OwnerTable;
                }
                if (!first.IsInsideTable)
                {
                    ownertable.PreviousBlock = first.PreviousBlock;
                    first.Section.Blocks.Remove(first);
                }
                else
                {
                    first.AssociatedCell.Blocks.Remove(first);
                }

            }
        }

        internal ElementBox GetElementBoxFromTextPosition()
        {
            LineInfo line = this.Caret.GetLineInfoFromPoint(this.TextPosition.Point);
            foreach (ElementBox elementbox in line.ElementBoxes)
            {
                return elementbox;
            }
            return null;
        }

        /// <summary>
        /// Returns the inline using the Text Position
        /// </summary>
        /// <param name="indexInInline"></param>
        /// <returns></returns>
        internal Inline GetInlineFromTextPosition(ref int indexInInline)
        {
            int sum = 0;
            if (Paragraph != null && Paragraph.Inlines != null)
            {
                foreach (Inline inline in Paragraph.Inlines)
                {
                    int inlineLength = inline.IsImageContainer || inline.IsUIContainer ? 1 : inline.InternalText.Length;
                    if (sum + inlineLength >= TextPosition.ParseIndex())
                    {
                        indexInInline = TextPosition.ParseIndex() - sum;
                        return inline;
                    }

                    sum = sum + inlineLength;
                }
            }
            return null;
        }


        internal Inline GetInlineFromTextPosition(TextPosition textPosition)
        {
            int sum = 0;
            if (Paragraph != null && Paragraph.Inlines != null)
            {
                foreach (Inline inline in Paragraph.Inlines)
                {
                    int inlineLength = inline.IsImageContainer || inline.IsUIContainer ? 1 : inline.InternalText.Length;
                    if (sum + inlineLength >= TextPosition.ParseIndex())
                    {
                        return inline;
                    }

                    sum = sum + inlineLength;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="index"></param>
        /// <param name="indexInInline"></param>
        /// <returns></returns>
        internal Inline GetInlineFromIndex(ParagraphAdv block, string index, ref int indexInInline)
        {
            int sum = 0;
            if (block != null)
            {
                if (block is ParagraphAdv)
                {
                    foreach (Inline inline in block.Inlines)
                    {
                        int inlineLength = inline.IsImageContainer || inline.IsUIContainer ? 1 : inline.InternalText.Length;
                        if (sum + inlineLength >= TextPosition.ParseIndex(index))
                        {
                            indexInInline = TextPosition.ParseIndex(index) - sum;
                            return inline;
                        }

                        sum = sum + inlineLength;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the ElementBox using index in inline 
        /// </summary>
        /// <param name="inline"></param>
        /// <param name="indexInInline"></param>
        /// <param name="indexInElementBox"></param>
        /// <returns></returns>
        internal ElementBox GetElemenBoxFromIndexInInline(Inline inline, int indexInInline, ref int indexInElementBox)
        {
            int sum = 0;
            if (inline != null)
            {
                foreach (ElementBox elementBox in inline.ElementBoxes)
                {
                    int boxLength = elementBox.IsImageBox || elementBox.IsUIBox ? 1 : elementBox.InternalText.Length;
                    if (sum + boxLength >= indexInInline)
                    {
                        indexInElementBox = indexInInline - sum;
                        return elementBox;
                    }
                    sum = sum + boxLength;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="paragraph"></param>
        /// <param name="newPara"></param>
        /// <returns></returns>
        private bool SplitAt(string index, ParagraphAdv paragraph, ref ParagraphAdv newPara)
        {
            if (TextPosition.ParseIndex() != 0 && paragraph.Inlines.Count > 0)
            {
                newPara = paragraph.CreateNewParagraph();
                newPara.Margin = paragraph.Margin;
                int indexInInline = 0;
                Inline inline = GetInlineFromIndex(paragraph, index, ref indexInInline);
                if (inline != null)
                {
                    bool isEnd = inline.IsImageContainer || inline.IsUIContainer ? indexInInline == 1 : indexInInline == inline.InternalText.Length;
                    int inlineIndex = paragraph.Inlines.IndexOf(inline) + 1;
                    if (!isEnd)
                    {
                        int indexInElementBox = 0;
                        ElementBox elementBox = GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
                        Inline newInline = inline.CreatInline();
                        newInline.InternalText = inline.InternalText.Substring(indexInInline);
                        inline.InternalText = inline.InternalText.Substring(0, indexInInline);
                        if (elementBox != null)
                        {
                            int indexOfBox = inline.ElementBoxes.IndexOf(elementBox) + 1;
                            if (elementBox.InternalText.Length != indexInElementBox)
                            {
                                ElementBox newBox = elementBox.CreateElementBox();
                                newBox.InternalText = elementBox.InternalText.Substring(indexInElementBox);
                                elementBox.InternalText = elementBox.InternalText.Substring(0, indexInElementBox);
                                if (!string.IsNullOrEmpty(newBox.InternalText))
                                {
                                    newBox.Inline = newInline;
                                    newInline.ElementBoxes.Add(newBox);
                                }
                            }

                            for (int i = indexOfBox; i < inline.ElementBoxes.Count; i = indexOfBox)
                            {
                                inline.ElementBoxes[i].Inline = newInline;
                                newInline.ElementBoxes.Add(inline.ElementBoxes[i]);
                                inline.ElementBoxes.RemoveAt(i);
                            }
                        }
                        if (!string.IsNullOrEmpty(newInline.InternalText))
                        {
                            newInline.Paragraph = newPara;
                            newPara.Inlines.Add(newInline);
                        }
                    }

                    for (int i = inlineIndex; i < paragraph.Inlines.Count; i = inlineIndex)
                    {
                        paragraph.Inlines[i].Paragraph = newPara;
                        newPara.Inlines.Add(paragraph.Inlines[i]);
                        paragraph.Inlines.RemoveAt(i);
                    }

                    //if (newPara.Inlines.Count > 0 && paragraph.Section != null)
                    if (paragraph.Section != null && !paragraph.IsInsideTable)
                    {
                        int paraIndex = paragraph.Section.Blocks.IndexOf(paragraph) + 1;
                        newPara.Section = paragraph.Section;
                        newPara.LayoutViewer = paragraph.LayoutViewer;
                        newPara.PreviousBlock = paragraph;
                        newPara.NextBlock = paragraph.NextBlock;
                        if (newPara.NextBlock != null)
                        {
                            newPara.NextBlock.PreviousBlock = newPara;
                        }
                        paragraph.NextBlock = newPara;
                        paragraph.LinkElementBoxes();
                        newPara.LinkElementBoxes();
                        paragraph.Section.Blocks.Insert(paraIndex, newPara);
                        return true;
                    }

                    if (Paragraph.IsInsideTable)
                    {
                        int paraIndex = paragraph.AssociatedCell.Blocks.IndexOf(paragraph) + 1;
                        newPara.PreviousBlock = paragraph;
                        newPara.NextBlock = paragraph.NextBlock;
                        if (newPara.NextBlock != null)
                        {
                            newPara.NextBlock.PreviousBlock = newPara;
                        }
                        paragraph.NextBlock = newPara;
                        paragraph.LinkElementBoxes();
                        newPara.LinkElementBoxes();
                        //newPara.ArrangeElements();
                        Paragraph.AssociatedCell.Blocks.Insert(paraIndex, newPara);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void PositionCursor()
        {
            string offsetString = string.Empty;
            int indexInInline = 0;
            int indexInElementBox = 0;
            Inline inline = GetInlineFromTextPosition(ref indexInInline);
            ElementBox elementBox = inline != null ? GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox) : null;
            Point point = new Point();
            if (inline != null && elementBox != null)
            {
                if (elementBox.IsUIBox || elementBox.IsImageBox)
                {
                    point.X = elementBox.ElementLocation.X + elementBox.Size.Width;
                    point.Y = elementBox.Location.Y + elementBox.Size.Height / 2;
                    if (TextPosition.IsPositionAtParagraphStart)
                        point.X = elementBox.ElementLocation.X;
                }
                else
                {
                    double width = TextHelper.MeasureText(elementBox.InternalText.Substring(0, indexInElementBox), elementBox).Width;
                    point.X = elementBox.ElementLocation.X + width;
                    point.Y = elementBox.Location.Y + elementBox.Size.Height / 2;
                }
                PageAdv page = GetPageUsingLineInfo(elementBox.LineInfo);
                if (page != null)
                {
                    Caret = page.Caret;
                    Caret.MoveCaretToPosition(point);
                    CurrentPage = page;
                }
            }
            else if (Paragraph != null && Paragraph.Inlines != null && Paragraph.Inlines.Count == 0 && Paragraph.LineInfo.Count > 0)
            {
                LineInfo line = Paragraph.LineInfo[0];
                PageAdv page = GetPageUsingLineInfo(line);
                Caret = page.Caret;
                //Canvas.SetLeft(Caret, line.Location.X);
                //Canvas.SetTop(Caret, line.Location.Y);
                Caret.MoveCaretToPosition(new Point(line.Location.X, line.Location.Y + line.Height / 2));
                if (page != null)
                    CurrentPage = page;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inline"></param>
        /// <param name="elementBox"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal double GetSpanPositionToInsertText(Inline inline, ElementBox elementBox, string text)
        {
            double index = 0;
            double wordSum = 0;
            List<string> words = SpanAdv.SplitInToWord(inline.InternalText);

            if (!inline.IsImageContainer && !inline.IsUIContainer)
            {
                double pos = inline.ElementBoxes.IndexOf(elementBox);
                wordSum = SpanAdv.GetSumOfTheWords(inline.ElementBoxes, pos);
            }

            index = wordSum + text.Length;

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal PageAdv GetPageUsingLineInfo(LineInfo line)
        {
            PageAdv page = null;
            if (line != null)
            {
                if (OwnerControl.Viewer.Pages.Count > line.PageIndex)
                {
                    page = OwnerControl.Viewer.Pages[line.PageIndex];
                }
                else
                {
                    foreach (PageAdv pge in OwnerControl.Viewer.Pages)
                    {
                        if (pge.LineInfos.Contains(line))
                        {
                            line.PageIndex = OwnerControl.Viewer.Pages.IndexOf(pge);
                            page = pge;
                            break;
                        }
                    }
                }
            }

            return page;
        }
        /// <summary>
        /// Handles the URL text.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <param name="indexInInline">The index in inline.</param>
        /// <returns></returns>
        private bool HandleURLText(Inline inline, int indexInInline)
        {
            int endindex = Paragraph.Inlines.IndexOf(inline);
            int startindex = endindex;
            Stack<SpanAdv> spans = new Stack<SpanAdv>();
            bool needToHandle = GetSpans(indexInInline, ref startindex, ref spans);
            if (needToHandle)
            {
                string url = string.Empty;
                SpanAdv firstSpan = spans.Pop();
                if (startindex == endindex && indexInInline!= firstSpan.Text.Length)
                {
                    SpanAdv newSpan = new SpanAdv();
                    firstSpan.Clone(newSpan);
                    newSpan.Text = firstSpan.Text.Substring(indexInInline);
                    firstSpan.Text = firstSpan.Text.Remove(indexInInline);
                    Paragraph.Inlines.Insert(startindex + 1, newSpan);
                }
                int lastindex = firstSpan.Text.LastIndexOf(' ');
                if (lastindex != firstSpan.Text.Length - 1)
                {
                    url = firstSpan.Text.Substring(lastindex + 1);
                    firstSpan.Text = firstSpan.Text.Remove(lastindex + 1);
                }
                for (int i = startindex; i < endindex; i++)
                {
                    SpanAdv span = spans.Pop();
                    url += span.Text;
                    Paragraph.Inlines.Remove(span);
                }
                if (url.EndsWith(" "))
                {
                    SpanAdv newSpan = new SpanAdv();
                    newSpan.Clone(Paragraph.Inlines[startindex] as SpanAdv);
                    newSpan.Text = " ";
                    url = url.TrimEnd();
                    Paragraph.Inlines.Insert(startindex + 1, newSpan);
                }
                HyperlinkAdv hyperlink = new HyperlinkAdv();
                hyperlink.Text = url;
                hyperlink.NavigationUrl = url;
                Paragraph.Inlines.Insert(startindex + 1, hyperlink);
                Paragraph.MeasureElements();
                return false;
            }
            return true;
        }
        /// <summary>
        /// Gets the spans.
        /// </summary>
        /// <param name="indexInInline">The index in inline.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="spans">The spans.</param>
        /// <returns></returns>
        private bool GetSpans(int indexInInline,ref int startIndex,ref Stack<SpanAdv> spans)
        {
            string data = (Paragraph.Inlines[startIndex] as SpanAdv).Text.Substring(0, indexInInline).ToLower();
            while (startIndex >0 && Paragraph.Inlines[startIndex-1] is SpanAdv)
            {
                spans.Push(Paragraph.Inlines[startIndex] as SpanAdv);
                if ((data.Contains(" http://") || data.Contains(" www.") || data.Contains(" mailto:")) && !(data.EndsWith(" http://") && data.EndsWith(" www.") && data.EndsWith(" mailto:")))
                    return true;
                data = (Paragraph.Inlines[startIndex - 1] as SpanAdv).Text + data;
                data = data.ToLower();
                startIndex--;
            }
            if (((data.Contains(" http://") || data.Contains(" www.") || data.Contains(" mailto:")) && !(data.EndsWith(" http://") || data.EndsWith(" www.") || data.EndsWith(" mailto:")))
                || (!(data.Equals("http://") || data.Equals("www.") || data.Equals("mailto:")) && (data.StartsWith("http://") || data.StartsWith("www.") || data.StartsWith("mailto:"))))
            {
                spans.Push(Paragraph.Inlines[startIndex] as SpanAdv);
                return true;
            }
            return false;
        }
    }
}
