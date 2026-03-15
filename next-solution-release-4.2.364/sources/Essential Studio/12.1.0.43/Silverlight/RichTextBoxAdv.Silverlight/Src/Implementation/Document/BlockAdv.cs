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
using System.Windows.Documents;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    public abstract class BlockAdv :DependencyObject
    {

        #region Members

        SectionAdv section;
        internal ObservableCollection<LineInfo> lines = new ObservableCollection<LineInfo>();
        private double startPoint;
        private double endPoint;
        internal LayoutViewer layoutViewer;
        private BlockAdv nextblock;
        internal protected InlineCollection inlines;
        internal bool CreateNewLines = true;
        internal bool IsArrangingParagraph = false;
        internal bool IsArranged = false;
        internal double MaxHeightForCalOffset = 17.78;
        internal bool IsInvalidated = false;
        private TableCellAdv tablecell = null;
        private bool istable = false;
        private bool isparagraph = false;
        internal double width;
        BlockAdv previous = null;

        #endregion

        #region Properties

        /// <summary>
        /// LineInfos of the Block
        /// </summary>
        internal ObservableCollection<LineInfo> LineInfo
        {
            get
            {
                return lines;
            }
            set
            {
                lines = value;
            }
        }

        /// <summary>
        /// It checks whether this is table.
        /// </summary>
        internal bool IsTable
        {
            get
            {
                return istable;
            }
            set
            {
                istable = value;
            }
        }

        /// <summary>
        /// It checks whether this is a Paragraph
        /// </summary>
        internal bool IsParagraph
        {
            get
            {
                return isparagraph;
            }
            set
            {
                isparagraph = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Thickness Margin
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        internal RichTextBoxAdv BaseParent
        {
            get
            {
                return  GetLayoutViewer().OwnerControl;
            }
        }

        /// <summary>
        /// Gets or Sets Layoutviewer
        /// </summary>
        internal LayoutViewer LayoutViewer
        {
            get
            {
                if (IsInsideTable)
                    return GetLayoutViewer();
                return layoutViewer;
            }
            set
            {
                layoutViewer = value;
            }
        }

        /// <summary>
        /// Gets or sets the end point of the Block
        /// </summary>
        internal double EndPoint
        {
            get
            {
                if (LineInfo!=null && LineInfo.Count > 0)
                {
                    return LineInfo.Last().BoundingRectangle.Bottom;
                }

                return endPoint;
            }
            set
            {
                endPoint = value;
            }
        }

        /// <summary>
        /// Gets or Sets the start point of the Block
        /// </summary>
        internal double StartPoint
        {
            get
            {
                if (LineInfo.Count > 0)
                {
                    return LineInfo.First().BoundingRectangle.Top;
                }

                return startPoint;
            }
            set
            {
                startPoint = value;
            }
        }

        internal SectionAdv Section
        {
            get
            {
                return section;
            }
            set
            {
                section = value;
                if (section != null)
                {
                    Margin = section.PageContentMargin;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the inlines
        /// </summary>
        public InlineCollection Inlines
        {
            get
            {
                return inlines;
            }
            internal protected set
            {
                inlines = value;
            }
        }

        /// <summary>
        /// It sets the Parent Cell.
        /// </summary>
        internal TableCellAdv AssociatedCell
        {
            get
            {
                return tablecell;
            }
            set
            {
                tablecell = value;
            }
        }

        /// <summary>
        /// Gets or Sets the previous Block
        /// </summary>
        internal BlockAdv PreviousBlock
        {
            get
            {
                return previous;
            }
            set
            {
                previous = value;
            }
        }
        
        /// <summary>
        /// Gets or Sets the next block.
        /// </summary>
        internal BlockAdv NextBlock
        {
            get
            {
                return nextblock;
            }
            set
            {
                nextblock = value;
            }
        }

        /// <summary>
        /// It gets or sets the Width of the Block
        /// </summary>
        internal double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        
        /// <summary>
        /// It gets the height of the Block.
        /// </summary>
        internal double Height
        {
            get
            {
                if (LineInfo.Count > 0)
                {
                    return LineInfo.Sum<LineInfo>(l => l.Height);
                }
                else
                    return 0.0;
            }
        }

        /// <summary>
        /// It checks whether the Block is inside of the table.
        /// </summary>
        internal bool IsInsideTable
        {
            get;
            set;
        }

        internal bool IsNextBlockArranged
        {
            get
            {
                if (NextBlock !=null && this.NextBlock.LineInfo.Count > 0)
                    return true;

                return NextBlock != null && NextBlock.LineInfo.Count > 0 ? NextBlock.IsNextBlockArranged : false;
            }
        }

        internal bool IsPreviousBlockArranged
        {
            get
            {
                if (PreviousBlock !=null && this.PreviousBlock.LineInfo.Count > 0)
                    return true;

                return PreviousBlock != null && PreviousBlock.LineInfo.Count > 0 ? PreviousBlock.IsPreviousBlockArranged : false;
            }
        }

        internal BlockAdv NextToNext
        {
            get
            {
                if (NextBlock != null && NextBlock.LineInfo.Count > 0)
                    return NextBlock;

                return NextBlock != null ? NextBlock.NextToNext : null;
            }
        }

        internal BlockAdv PreviousToPrevious
        {
            get
            {
                if (PreviousBlock != null && PreviousBlock.LineInfo.Count > 0)
                    return PreviousBlock;

                return PreviousBlock != null ? PreviousBlock.PreviousToPrevious : null;
            }
        }

        public abstract bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Gets or Sets the left indent
        /// </summary>
        public double LeftIndent
        {
            get
            {
                return (double)GetValue(LeftIndentProperty);
            }
            set
            {
                SetValue(LeftIndentProperty, value);
            }
        }

        /// <summary>
        /// Registers left indent dependency property
        /// </summary>
        public static readonly DependencyProperty LeftIndentProperty = DependencyProperty.Register("LeftIndent", typeof(double), typeof(BlockAdv), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets the right indent
        /// </summary>
        public double RightIndent
        {
            get
            {
                return (double)GetValue(RightIndentProperty);
            }
            set
            {
                SetValue(RightIndentProperty, value);
            }
        }

        /// <summary>
        /// Registers right indent dependency property
        /// </summary>
        public static readonly DependencyProperty RightIndentProperty = DependencyProperty.Register("RightIndent", typeof(double), typeof(BlockAdv), new PropertyMetadata(0d));

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public BlockAdv()
        {
            
        }

        /// <summary>
        /// Clears all the lines
        /// </summary>
        internal void ClearLines()
        {
            foreach (LineInfo line in LineInfo)
            {
                if (!IsInsideTable)
                {
                    if (LayoutViewer.LineInfos.Contains(line))
                    {
                        LayoutViewer.LineInfos.Remove(line);
                        line.ClearElementBoxes();
                    }
                }
                else
                {
                    if (AssociatedCell.LineInfos.Contains(line))
                    {
                        AssociatedCell.LineInfos.Remove(line);
                        line.ClearElementBoxes();
                    }
                }
            }

            LineInfo.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementbox"></param>
        /// <returns></returns>
        internal bool CanAddToLine(ElementBox elementbox)
        {
            return elementbox != null &&
                            !elementbox.IsImageBox && !elementbox.IsUIBox && (string.IsNullOrEmpty(elementbox.InternalText) ||
                            elementbox.InternalText == " " || elementbox.InternalText == "\t");
        }

        /// <summary>
        /// It checks whether can add to Previous box.
        /// </summary>
        /// <param name="elementbox"></param>
        internal void CheckCanAddToPreviousBox(ElementBox elementbox)
        {
            if (elementbox.PreviousElementBox != null && elementbox.PreviousElementBox.IsAddedToLine &&
                                CanAddToLine(elementbox.PreviousElementBox))
            {
                elementbox.PreviousElementBox.IsAddedToLine = false;
                elementbox.PreviousElementBox.LineInfo.Width = elementbox.PreviousElementBox.LineInfo.Width - elementbox.PreviousElementBox.ElementSize.Width;
                if (elementbox.PreviousElementBox.LineInfo.Width < 0)
                {
                    elementbox.PreviousElementBox.LineInfo.Width = 0;
                }
                else
                {
                    CheckCanAddToPreviousBox(elementbox.PreviousElementBox);
                }
            }
            else
                return;
        }

        /// <summary>
        /// It checks to Right indent of Paragraph
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftindent"></param>
        /// <returns></returns>
        internal bool CheckForRightIndent(double value, double leftindent)
        {
            double width = GetLayoutViewer() is PageLayoutViewer ? GetLayoutViewer().OwnerControl.Document.Sections[0].PageSize.Width : GetLayoutViewer().AvailableSize.Width;
            double left = GetLayoutViewer() is PageLayoutViewer ? Margin.Left : 0;
            double right = GetLayoutViewer() is PageLayoutViewer ? Margin.Right : 0;
            if ((left + value) > (width - 50 - left - leftindent) || value < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// It checks to left indent of the Paragraph
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightIndent"></param>
        /// <returns></returns>
        internal bool CheckForLeftIndent(double value, double rightIndent)
        {
            double width = GetLayoutViewer() is PageLayoutViewer ? GetLayoutViewer().OwnerControl.Document.Sections[0].PageSize.Width : GetLayoutViewer().AvailableSize.Width;
            double left = GetLayoutViewer() is PageLayoutViewer ? Margin.Left : 0;
            double right = GetLayoutViewer() is PageLayoutViewer ? Margin.Right : 0;
            if ((left + value) > (width - 50 - right - rightIndent) || value < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// It returns the index from Span
        /// </summary>
        /// <param name="inline"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal double GetIndexFromInlineAndSpanIndex(Inline inline, double index)
        {
            int inlineIndex = Inlines.IndexOf(inline);
            double sum = 0;

            for (int i = 0; i < inlineIndex; i++)
            {
                if (Inlines[i].IsUIContainer || Inlines[i].IsImageContainer)
                    sum = sum + 1;
                sum = sum + Inlines[i].InternalText.Length;
            }

            sum = sum + index;

            return sum;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="spanIndex"></param>
        /// <returns></returns>
        internal Inline GetInlineAndSpanIndexFromIndex(string index, out double spanIndex)
        {
            double sum = 0;
            Inline neededInline = null;
            spanIndex = double.NaN;
            double amt = 0;

            if (this is ParagraphAdv)
            {
                for (int i = 0; i < Inlines.Count; i++)
                {
                    //Inline inline = Inlines[i];

                    if (Inlines[i].IsUIContainer || Inlines[i].IsImageContainer)
                        amt = 1;
                    else
                        amt = Inlines[i].InternalText.Length;

                    if (LayoutViewer.TextPosition.ParseIndex(index) <= amt + sum)
                    {
                        spanIndex = LayoutViewer.TextPosition.ParseIndex(index) - sum;
                        neededInline = Inlines[i];
                        break;
                    }
                    else
                    {
                        sum = sum + amt;
                    }
                }
            }
            return neededInline;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void GetIndexOfWord(ElementBox box, ref double start, ref double end)
        {
            if (box != null)
            {
                ElementBox startBox = null;
                ElementBox endBox = null;
                if (!box.IsImageBox && !box.IsUIBox)
                {
                    startBox = GetPreviousTextBox(box);
                    endBox = GetNextTextBox(box);
                    if (startBox != null && endBox != null)
                    {
                        double startIndex = GetSpanPositionToInsertText(startBox.Inline, startBox, string.Empty);
                        double endIndex = GetSpanPositionToInsertText(endBox.Inline, endBox, endBox.InternalText);
                        start = GetIndexFromInlineAndSpanIndex(startBox.Inline, startIndex);
                        end = GetIndexFromInlineAndSpanIndex(endBox.Inline, endIndex);
                    }
                }
            }
        }

        /// <summary>
        /// It returns the Position to insert.
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

            double pos = inline.ElementBoxes.IndexOf(elementBox);
            wordSum = SpanAdv.GetSumOfTheWords(inline.ElementBoxes, pos);
            index = wordSum + text.Length;

            return index;
        }

        /// <summary>
        /// It gets the Previous texbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private ElementBox GetPreviousTextBox(ElementBox box)
        {
            if (box.PreviousElementBox == null || box.IsUIBox || box.IsImageBox ||
                string.IsNullOrEmpty(box.PreviousElementBox.InternalText) || box.PreviousElementBox.InternalText == " " ||
                box.PreviousElementBox.InternalText == "    " || box.PreviousElementBox.InternalText.Contains(" "))
            {
                return box;
            }

            return GetPreviousTextBox(box.PreviousElementBox);
        }

        /// <summary>
        /// It gets the Previous word to start.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal TextPosition GetPreviousWordStart(ElementBox box)
        {
            ElementBox startBox = GetPreviousTextBox(box);

            //ElementBox previousBox = startBox.PreviousElementBox;
            ElementBox previousBox = startBox;

            if (!previousBox.IsImageBox && !previousBox.IsUIBox)
            {
                while (previousBox != null && (string.IsNullOrEmpty(previousBox.InternalText) || previousBox.InternalText == " " ||
                    previousBox.InternalText == "    "))
                {
                    previousBox = previousBox.PreviousElementBox;
                }
            }

            if (previousBox == null)
            {
                TextPosition pos = new TextPosition(Section.Document);
                pos.SetIndex("0");
                pos.Paragraph = this as ParagraphAdv;
                return pos;
            }
            else if (previousBox != null && !string.IsNullOrEmpty(previousBox.InternalText) && previousBox.InternalText != " " ||
                previousBox.InternalText != "    ")
            {
                previousBox = GetPreviousTextBox(previousBox);
                double startIndex = GetSpanPositionToInsertText(previousBox.Inline, previousBox, string.Empty);
                double start = GetIndexFromInlineAndSpanIndex(previousBox.Inline, startIndex);
                TextPosition pos = new TextPosition(Section.Document);
                pos.Paragraph = this as ParagraphAdv;
                pos.SetIndex(start.ToString());
                return pos;
            }

            return null;
        }

        /// <summary>
        /// It returns the next word to start.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal TextPosition GetNextWordStart(ElementBox box)
        {
            ElementBox startBox = GetNextTextBox(box);

            ElementBox nextBox = startBox.NextElementBox;

            if (nextBox != null && !nextBox.IsImageBox && !nextBox.IsUIBox)
            {
                while (nextBox != null && (string.IsNullOrEmpty(nextBox.InternalText) || nextBox.InternalText == " " ||
                    nextBox.InternalText == "    "))
                {
                    nextBox = nextBox.NextElementBox;
                }
            }

            if (nextBox != null && (nextBox.IsImageBox || nextBox.IsUIBox) && box.NextElementBox == nextBox)
            {
                string index = LayoutViewer.TextPosition.Index;
                double spanIndex = 0;
                GetInlineAndSpanIndexFromIndex(index, out spanIndex);
                if (spanIndex == box.Inline.GetLength())
                {
                    nextBox = nextBox.NextElementBox;
                }
            }

            if ((box.IsUIBox || box.IsImageBox) && box.NextElementBox == nextBox && nextBox != null)
            {
                nextBox = nextBox.NextElementBox;
            }

            if (nextBox == null)
            {
                TextPosition pos = new TextPosition(Section.Document);
                pos.Index = Length();
                pos.Paragraph = this as ParagraphAdv;
                return pos;
            }
            else if (nextBox != null && !string.IsNullOrEmpty(nextBox.InternalText) || nextBox.InternalText != " " ||
                nextBox.InternalText != "    ")
            {
                double startIndex = GetSpanPositionToInsertText(nextBox.Inline, nextBox, string.Empty);
                double start = GetIndexFromInlineAndSpanIndex(nextBox.Inline, startIndex);
                TextPosition pos = new TextPosition(Section.Document);
                pos.Paragraph = this as ParagraphAdv;
                pos.SetIndex(start.ToString());
                return pos;
            }

            return null;
        }

        /// <summary>
        /// It gets the next box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private ElementBox GetNextTextBox(ElementBox box)
        {
            if (box.NextElementBox == null || box.IsUIBox || box.IsImageBox ||
                string.IsNullOrEmpty(box.NextElementBox.InternalText) || box.NextElementBox.InternalText == " " ||
                box.NextElementBox.InternalText == "    " || box.NextElementBox.InternalText.Contains(" "))
            {
                return box;
            }

            return GetNextTextBox(box.NextElementBox);
        }

        /// <summary>
        /// It gets the LayoutViewer.
        /// </summary>
        /// <returns></returns>
        internal LayoutViewer GetLayoutViewer()
        {
            if (IsInsideTable)
            {
                BlockAdv block = AssociatedCell.OwnerTable;
                while (block.AssociatedCell != null)
                {
                    block = block.AssociatedCell.OwnerTable;
                }
                return block.LayoutViewer;
            }
            else
                return LayoutViewer;
        }

        /// <summary>
        /// It gets the section.
        /// </summary>
        /// <returns></returns>
        internal SectionAdv GetSection()
        {
            if (IsInsideTable)
            {
                BlockAdv block = AssociatedCell.OwnerTable;
                while (block.AssociatedCell != null)
                {
                    block = block.AssociatedCell.OwnerTable;
                }
                return block.Section;
            }
            else
                return Section;
        }

        /// <summary>
        /// It returns the length of the Block.
        /// </summary>
        /// <returns></returns>
        public string Length()
        {
            double length = 0;
            if (this is ParagraphAdv)
            {
                length = GetLengthofInlines(this);
            }
            else if (this is TableAdv)
            {
                BlockAdv lastblock = (this as TableAdv).GetLastBlockInLastCell();
                length = double.Parse(lastblock.Length());
            }
            return length.ToString();
        }

        /// <summary>
        /// It returns the length of the Lines.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal double GetLengthofInlines(BlockAdv b)
        {
            return b.Inlines.Sum<Inline>(inline => inline.GetLength());
        }

        /// <summary>
        /// Links the element boxes in this paragraph
        /// </summary>
        internal void LinkElementBoxes()
        {
            int j = 0;

            if (Inlines != null)
            {

                foreach (Inline inline in Inlines)
                {
                    int i = 0;
                    foreach (ElementBox elementBox in inline.ElementBoxes)
                    {
                        if (Inlines.First().Equals(inline) && inline.ElementBoxes.Count > 0 && inline.ElementBoxes.First().Equals(elementBox))
                        {
                            elementBox.PreviousElementBox = null;
                        }
                        if (inline.ElementBoxes.Count > 0 && !inline.ElementBoxes.Last().Equals(elementBox))
                        {
                            elementBox.NextElementBox = inline.ElementBoxes[i + 1];
                            inline.ElementBoxes[i + 1].PreviousElementBox = elementBox;
                        }
                        else
                        {
                            if (!Inlines.Last().Equals(inline) && Inlines[j + 1].ElementBoxes.Count > 0)
                            {
                                elementBox.NextElementBox = Inlines[j + 1].ElementBoxes[0];
                                Inlines[j + 1].ElementBoxes[0].PreviousElementBox = elementBox;
                            }
                            else if (Inlines.Last().Equals(inline) && inline.ElementBoxes.Count > 0 && inline.ElementBoxes.Last().Equals(elementBox))
                            {
                                elementBox.NextElementBox = null;
                            }
                        }
                        i++;
                    }
                    j++;
                }
            }
        }

        /// <summary>
        /// It checks to fit the content.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="width"></param>
        internal void CheckForFitTocontent(ElementBox box, double width)
        {
            if (box.Inline != null)
            {
                if (box.IsImageBox)
                {
                    if ((box.Inline as ImageContainerAdv).FitToContent && (box.Inline as ImageContainerAdv).Width > width)
                        (box.Inline as ImageContainerAdv).Width = width;

                    if ((box.Inline as ImageContainerAdv).Width > width)
                    {
                        Image image = (box as ImageElementBox).Image;
                        image.Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, (box as ImageElementBox).Height) };

                        if (LayoutViewer != null && LayoutViewer.ImageResizer != null)
                        {
                            LayoutViewer.ImageResizer.PreviewImage.Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, (box as ImageElementBox).Height) };
                        }
                    }

                    if (LayoutViewer != null && LayoutViewer.SelectedImage != null && LayoutViewer.SelectedImage == box.Inline && LayoutViewer.ImageResizer != null
                        && LayoutViewer.ImageResizer.ImageContainer == box.Inline)
                    {
                        LayoutViewer.ImageResizer.Width = (box.Inline as ImageContainerAdv).Width;
                        LayoutViewer.ImageResizer.Height = (box.Inline as ImageContainerAdv).Height;
                    }
                }
                if (box.IsUIBox)
                {
                    if ((box.Inline as UIContainerAdv).FitToContent && (box.Inline as UIContainerAdv).Width > width)
                        (box.Inline as UIContainerAdv).Width = width;
                }
            }
        }

        /// <summary>
        /// It wraps to next paragraphs.
        /// </summary>
        /// <param name="previousendpoint"></param>
        /// <param name="size"></param>
        /// <param name="flag"></param>
        internal void WrapToNextParagraphs(double previousendpoint,Size size,ref bool flag)
        {
            if (LineInfo.Count != 0)
            {
                EndPoint = LineInfo.Last().BoundingRectangle.Bottom;

                if ((Math.Round(previousendpoint) != Math.Round(EndPoint) || (NextBlock != null && NextBlock.StartPoint != EndPoint)) && !IsArrangingParagraph)
                {
                    if (NextBlock != null)
                    {
                        if (NextBlock.LineInfo.Count == 0)
                            NextBlock.CreateNewLines = true;
                        //NextBlock.ArrangeElements(size);
                        flag = true;
                    }
                }
                else if ((Math.Round(previousendpoint) == Math.Round(EndPoint) && !IsArrangingParagraph))
                {
                    if (NextBlock != null)
                    {
                        if (NextBlock.LineInfo.Count == 0)
                        {
                            NextBlock.CreateNewLines = true;
                            flag = true;
                        }
                        else
                        {
                            PageAdv page1 = GetLayoutViewer().GetPageFromLine(LineInfo.Last());
                            PageAdv page2 = GetLayoutViewer().GetPageFromLine(NextBlock.LineInfo.Last());
                            //PageAdv page1 = LayoutViewer.Pages[LineInfo.Last().PageIndex];
                            //PageAdv page2 = LayoutViewer.Pages[NextBlock.LineInfo.Last().PageIndex];
                            flag = page1 != page2;
                        }
                        //if (flag)
                        //    NextBlock.ArrangeElements(size);
                    }
                }

                IsArrangingParagraph = false;
            }
            else
            {
                EndPoint = 0;
            }
        }

        internal abstract int Search(TextPosition start, TextPosition end, TableCellAdv startcell, TableCellAdv endcell,ref BlockCollection<BlockAdv> blocks ,ref bool IsStarted,ref StringBuilder stringbuilder);

        internal abstract BlockAdv CreateBlock();

        public abstract BlockAdv CopyBlock();
        
        internal abstract void MeasureElements();

        internal abstract void ArrangeElements();

        internal abstract int ExtractInlines(TextPosition start, TextPosition end, ref bool started,ref BlockCollection<BlockAdv> affectedblocks,ref List<Inline> inlinesToFormat);

        internal abstract int SearchToCut(TextPosition start, TextPosition end, TableCellAdv startcell, TableCellAdv endcell, ref bool IsStarted, ref BlockCollection<BlockAdv> removedblocks, ref BlockCollection<BlockAdv> affectedblocks);

        internal abstract int SearchToSelect(TextPosition start, TextPosition end,ref bool IsStarted, ref BlockCollection<BlockAdv> blocks);

        internal virtual void ArrangeElements(int index)
        {
        }

        internal virtual void ArrangeElements(bool CreateNewLines)
        {
        }

        internal virtual void ArrangeElements(Size size)
        {
        }

        internal virtual BlockAdv Arrange(Size size)
        {
            return null;
        }
    }
}
