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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;

#if !WPF
using System.Windows.Browser;
#endif

using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Inlines")]
    public class ParagraphAdv : BlockAdv
    {
        #region fields

        private double hanging = 25;
        private ListItem associatedListItem;
        internal bool isStarted = false;

        #endregion

        #region public fields

        /// <summary>
        /// Gets or Sets Text Alignment
        /// </summary>
        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Registers TextAlignment dependency property
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(ParagraphAdv), new PropertyMetadata(TextAlignment.Left));

        /// <summary>
        /// Gets or Sets After Spacing
        /// </summary>
        public double AfterSpacing
        {
            get
            {
                return (double)GetValue(AfterSpacingProperty);
            }
            set
            {
                SetValue(AfterSpacingProperty, value);
            }
        }

        /// <summary>
        /// Registers AfterSpacing dependency property
        /// </summary>
        public static readonly DependencyProperty AfterSpacingProperty = DependencyProperty.Register("AfterSpacing", typeof(double), typeof(ParagraphAdv), new PropertyMetadata(13d));

        /// <summary>
        /// Gets or Sets Before Spacing
        /// </summary>
        public double BeforeSpacing
        {
            get
            {
                return (double)GetValue(BeforeSpacingProperty);
            }
            set
            {
                SetValue(BeforeSpacingProperty, value);
            }
        }

        /// <summary>
        /// Registers BeforeSpacing dependency property
        /// </summary>
        public static readonly DependencyProperty BeforeSpacingProperty = DependencyProperty.Register("BeforeSpacing", typeof(double), typeof(ParagraphAdv), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets Line spacing
        /// </summary>
        public double LineSpacing
        {
            get
            {
                return (double)GetValue(LineSpacingProperty);
            }
            set
            {
                SetValue(LineSpacingProperty, value);
            }
        }

        /// <summary>
        /// Registers LineSpacing dependency property
        /// </summary>
        public static readonly DependencyProperty LineSpacingProperty = DependencyProperty.Register("LineSpacing", typeof(double), typeof(ParagraphAdv), new PropertyMetadata(1d));


        /// <summary>
        /// Gets or Sets the list type of the paragraph
        /// </summary>
        public ListType ListType
        {
            get
            {
                return (ListType)GetValue(ListTypeProperty);
            }
            set
            {
                SetValue(ListTypeProperty, value);
            }
        }

        /// <summary>
        /// Registers the ListType dependency property
        /// </summary>
        public static readonly DependencyProperty ListTypeProperty = DependencyProperty.Register("ListType", typeof(ListType), typeof(ParagraphAdv), new PropertyMetadata(ListType.None, new PropertyChangedCallback(OnListTypeChanged)));

        /// <summary>
        /// It gets the ListNumber
        /// </summary>
        internal double ListNumber
        {
            get
            {
                if (PreviousBlock != null && PreviousBlock is ParagraphAdv && (PreviousBlock as ParagraphAdv).ListType == ListType.Numbered)
                {
                    return (PreviousBlock as ParagraphAdv).ListNumber + 1;
                }

                return 1;
            }
        }
        
        /// <summary>
        /// It gets/sets the Hanging value
        /// </summary>
        public double Hanging
        {
            get
            {
                return hanging;
            }
            internal set
            {
                hanging = value;
            }
        }

        /// <summary>
        /// It gets/sets the AssociatedItem
        /// </summary>
        internal ListItem AssociatedListItem
        {
            get
            {
                return associatedListItem;
            }
            set
            {
                associatedListItem = value;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return Inlines.Count == 0;
            }
        }

        #endregion

        /// <summary>
        /// Initializes new instance of Paragraph class
        /// </summary>
        public ParagraphAdv()
        {
            Inlines = new InlineCollection();
            LineInfo = new ObservableCollection<LineInfo>();
            EndPoint = 0;
            IsParagraph = true;
        }

        #region methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnListTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ParagraphAdv paragraph = (ParagraphAdv)dependencyObject;
            paragraph.OnListTypeChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        internal void OnListTypeChanged(DependencyPropertyChangedEventArgs args)
        {
            if ((ListType)args.OldValue != ListType.None)
            {
                LeftIndent -= 25;
            }

            if ((ListType)args.NewValue != ListType.None)
            {
                LeftIndent += 25;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraph"></param>
        internal void Clone(ParagraphAdv paragraph)
        {
            paragraph.AfterSpacing = AfterSpacing;
            paragraph.BeforeSpacing = BeforeSpacing;
            paragraph.LineSpacing = LineSpacing;
            paragraph.TextAlignment = TextAlignment;
            paragraph.RightIndent = RightIndent;
            paragraph.LeftIndent = LeftIndent;
            paragraph.ListType = ListType;
        }

        /// <summary>
        /// It creates the new Paragraph instance.
        /// </summary>
        /// <returns></returns>
        public ParagraphAdv CreateNewParagraph()
        {
            ParagraphAdv paragraph = new ParagraphAdv();
            paragraph.IsInsideTable = IsInsideTable;
            paragraph.AssociatedCell = AssociatedCell;
            paragraph.AfterSpacing = AfterSpacing;
            paragraph.BeforeSpacing = BeforeSpacing;
            paragraph.LineSpacing = LineSpacing;
            paragraph.TextAlignment = TextAlignment;
            paragraph.RightIndent = RightIndent;
            paragraph.LeftIndent = LeftIndent;
            paragraph.ListType = ListType;
            if (ListType != ListType.None)
            {
                paragraph.LeftIndent -= 25;
            }
            return paragraph;
        }

        /// <summary>
        /// Check whether two paragraphs in equal style.
        /// </summary>
        /// <param name="secondparagrah"></param>
        /// <returns></returns>
        public bool IsEqualInStyle(ParagraphAdv secondparagrah)
        {
            if (this == null && secondparagrah == null)
                return false;
            return (this.AfterSpacing == secondparagrah.AfterSpacing) && (this.BeforeSpacing == secondparagrah.BeforeSpacing) && (this.LeftIndent == secondparagrah.LeftIndent)
                   && (this.LineSpacing == secondparagrah.LineSpacing) && (this.RightIndent == this.RightIndent);
        }

        /// <summary>
        /// Measures elements and creates UIElement
        /// </summary>
        /// <param name="size"></param>
        internal override void MeasureElements()
        {
            foreach (LineInfo line in LineInfo)
            {
                line.ElementBoxes.Clear();
            }

            LineInfo.Clear();

            foreach (Inline inline in this.Inlines)
            {
                
                if (inline.Paragraph == null)
                {
                    inline.Paragraph = this;
                }
                inline.MeasureElements();
            }
        }

        /// <summary>
        /// It arranges the Paragraph
        /// </summary>
        internal override void ArrangeElements()
        {
            this.CreateNewLines = true;
            if (this.IsInsideTable)
            {
                TableAdv owner = AssociatedCell.OwnerTable;
                //double preWidth = AssociatedCell.DesiredWidth;
                //double height=AssociatedCell.CellHeight;

                //double subracted = owner.TableWidth - GetLayoutViewer().Document.Sections[0].PageContentMargin.Left - 
                //    GetLayoutViewer().Document.Sections[0].PageContentMargin.Right;

                //TableLayoutCalculator.MeasureTableLayout(owner, subracted);
                //bool isSplitted = AssociatedCell.CellElementBox.LineInfo.IsSplitted;

                //if (preWidth == AssociatedCell.DesiredWidth)
                //{
                //    CreateNewLines = true;
                //    ArrangeParagraph();
                //    AssociatedCell.CellElementBox.SetElementPosition();
                //    //if(Math.Floor(AssociatedCell.MeasureCellHeight()) !=Math.Floor(height) || isSplitted)
                //    //{
                //    //    int index = owner.LineInfo.IndexOf(AssociatedCell.CellElementBox.LineInfo);
                //    //    index= index == 0 ? 0 : index - 1;
                //    //    LineInfo lineInfo = owner.LineInfo[index];
                //    //    while (lineInfo.HasChildBoxes)
                //    //    {
                //    //        index--;
                //    //        lineInfo = owner.LineInfo[index];
                //    //    }
                //    //    owner.ArrangeElements(index);                        
                //    //}
                //}
                //else
                //{
                    while (owner !=null)
                    {
                        owner.ArrangeElements();
                        if (owner.AssociatedCell == null)
                            break;
                        owner = owner.AssociatedCell.OwnerTable;
                    }
                //}
            }
            else
            {
                ArrangeParagraph();
            }
        }

        internal void ArrangeParagraph()
        {
            if (Section == null)
                Section=base.GetLayoutViewer().Document.Sections[0];

            if (!IsInsideTable)
            {
                if (LayoutViewer is PageLayoutViewer)
                {
                    ArrangeElements(Section.PageSize);
                }
                else
                {
                    ArrangeElements(LayoutViewer.AvailableSize);
                }
            }
            else
            {
                ArrangeElements(new Size(AssociatedCell.DesiredWidth, 0));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createNewLines"></param>
        internal override void ArrangeElements(bool createNewLines)
        {
            this.CreateNewLines = createNewLines;

            if (this.IsInsideTable)
            {
                TableAdv owner = AssociatedCell.OwnerTable;
                //double preWidth = AssociatedCell.DesiredWidth;
                //double height = AssociatedCell.CellHeight;

                //double subracted = owner.TableWidth - owner.Section.PageContentMargin.Left - owner.Section.PageContentMargin.Right;

                //TableLayoutCalculator.MeasureTableLayout(owner, subracted);

                //bool isSplitted = AssociatedCell.CellElementBox.LineInfo.IsSplitted;

                //if (preWidth == AssociatedCell.DesiredWidth)
                //{
                //    CreateNewLines = true;
                //    ArrangeParagraph();
                //    AssociatedCell.CellElementBox.SetElementPosition();
                //    if (Math.Floor(AssociatedCell.MeasureCellHeight()) != Math.Floor(height) || isSplitted)
                //    {
                //        int index = owner.LineInfo.IndexOf(AssociatedCell.CellElementBox.LineInfo);
                //        index = index == 0 ? 0 : index - 1;
                //        LineInfo lineInfo = owner.LineInfo[index];
                //        while (lineInfo.HasChildBoxes)
                //        {
                //            index--;
                //            lineInfo = owner.LineInfo[index];
                //        }
                //        owner.ArrangeElements(index);
                //    }
                //}
                //else
                //{
                while (owner != null)
                {
                    owner.ArrangeElements();
                    if (owner.AssociatedCell == null)
                        break;
                    owner = owner.AssociatedCell.OwnerTable;
                }
                //}
            }
            else
            {
                if (LayoutViewer is PageLayoutViewer)
                {
                    ArrangeElements(Section.PageSize);
                }
                else
                {
                    ArrangeElements(LayoutViewer.AvailableSize);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        internal override void ArrangeElements(Size size)
        {
            BlockAdv block = Arrange(size);
            while (block != null)
            {
                block.LayoutViewer = LayoutViewer;
                block = block.Arrange(size);
            }
        }

        /// <summary>
        /// It returns the new Block instance
        /// </summary>
        /// <returns></returns>
        internal override BlockAdv CreateBlock()
        {
            ParagraphAdv paragraph = new ParagraphAdv();

            paragraph.AfterSpacing = AfterSpacing;
            paragraph.BeforeSpacing = BeforeSpacing;
            paragraph.LineSpacing = LineSpacing;
            paragraph.TextAlignment = TextAlignment;
            paragraph.RightIndent = RightIndent;
            paragraph.LeftIndent = LeftIndent;
            paragraph.ListType = ListType;
            paragraph.IsInsideTable = IsInsideTable;
            paragraph.AssociatedCell = AssociatedCell;

            if (ListType != ListType.None)
            {
                paragraph.LeftIndent -= 25;
            }
            return paragraph;
        }

        /// <summary>
        /// Arranges the elements.
        /// </summary>
        /// <param name="width">The size.</param>
        internal override BlockAdv Arrange(Size size)
        {
            int LineIndex = 0;
            double ComputedWidth = 0.0;

            bool ContainsLine = false;
            double startpointofblock = 0.0;
            double ComX = 0.0;
            LineIndex = 0;
            double previousendpoint = EndPoint;
            bool recreate = false;
            Width = size.Width;
             if (LayoutViewer != null)
            {
            if (CreateNewLines)
            {
                foreach (LineInfo line in LineInfo)
                {
                    if (IsInsideTable)
                    {
                        if (AssociatedCell.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                ContainsLine = true;
                                LineIndex = AssociatedCell.LineInfos.IndexOf(line);
                            }
                            AssociatedCell.LineInfos.Remove(line);
                        }
                    }
                    else
                    {
                        if (LayoutViewer.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                ContainsLine = true;
                                LineIndex = LayoutViewer.LineInfos.IndexOf(line);
                            }
                            LayoutViewer.LineInfos.Remove(line);
                        }
                    }
                    line.ClearElementBoxes();
                }

                LineInfo.Clear();

                ComputedWidth = Width - (Margin.Left + Margin.Right + LeftIndent + RightIndent);
                ComX = Margin.Left + LeftIndent;
                startpointofblock = PreviousBlock != null ? PreviousBlock.EndPoint : Margin.Top;

                if (IsInsideTable)
                {
                    startpointofblock = PreviousBlock != null ? PreviousBlock.EndPoint : 0;
                    ComX = AssociatedCell.CellMargin.Left;
                    ComputedWidth = Width;
                }
                if (LayoutViewer is FlowLayoutViewer)
                {
                    ComputedWidth = Width - (LeftIndent + RightIndent);
                    ComX = LeftIndent;
                    startpointofblock = PreviousBlock != null ? PreviousBlock.EndPoint : 0;
                }
                if (ListType != ListType.None)
                {
                    ComX += Hanging;
                    ComputedWidth -= Hanging;
                }

                LineInfo lineInfo = new LineInfo();
                lineInfo.Block = this;

                if (inlines.Count == 0)
                {
                    lineInfo.LineSpacing = LineSpacing;
                    if (Section != null && Section.Document != null
                        && Section.Document.OwnerControl != null)
                    {
                        lineInfo.AfterSpacing = AfterSpacing;
                        lineInfo.BeforeSpacing = BeforeSpacing;
                        lineInfo.CalEmptyLineHeight(Section.Document.OwnerControl.CurrentInlineStyle);
                    }
                    else if (IsInsideTable)
                    {
                        lineInfo.CalEmptyLineHeight(GetLayoutViewer().OwnerControl.CurrentInlineStyle);
                    }
                    lineInfo.BoundingRectangle = new Rect(ComX, startpointofblock, lineInfo.Width, lineInfo.Height);
                    lineInfo.Width = lineInfo.BoundingRectangle.Width;
                    if (PreviousBlock != null && !ContainsLine && PreviousBlock.LineInfo.Count > 0)
                    {
                        ContainsLine = true;
                        LineIndex = IsInsideTable ? AssociatedCell.LineInfos.IndexOf(PreviousBlock.LineInfo.Last()) + 1 : LayoutViewer.LineInfos.IndexOf(PreviousBlock.LineInfo.Last()) + 1;
                    }
                    else if (!ContainsLine && NextBlock != null && NextBlock.LineInfo.Count > 0)
                    {
                        ContainsLine = true;
                        LineIndex = IsInsideTable ? AssociatedCell.LineInfos.IndexOf(NextBlock.LineInfo.First()) : LayoutViewer.LineInfos.IndexOf(NextBlock.LineInfo.First());
                    }
                }
                if (!ContainsLine && IsPreviousBlockArranged)
                {
                    ContainsLine = true;
                    LineIndex = IsInsideTable ? AssociatedCell.LineInfos.IndexOf(PreviousBlock.LineInfo.Last()) + 1 : LayoutViewer.LineInfos.IndexOf(PreviousToPrevious.LineInfo.Last()) + 1;
                }
                if (!ContainsLine && IsNextBlockArranged)
                {
                    ContainsLine = true;
                    LineIndex = IsInsideTable ? AssociatedCell.LineInfos.IndexOf(NextBlock.LineInfo.First()) : LayoutViewer.LineInfos.IndexOf(NextToNext.LineInfo.First());
                }

                LineInfo.Add(lineInfo);

                double xPos = ComX;

                foreach (Inline inline in Inlines)
                {
                    inline.Paragraph = this;

                    for (int i = 0; i < inline.ElementBoxes.Count; i++)
                    {
                        ElementBox elementbox = inline.ElementBoxes[i];
                        CheckForFitTocontent(elementbox, ComputedWidth);
                        elementbox.IsAddedToLine = true;
                        bool canAdd = CanAddToLine(elementbox);
                        if (elementbox.Element != null && elementbox.Element.Parent != null)
                        {
                            if (elementbox.Element.Parent is Canvas)
                            {
                                (elementbox.Element.Parent as Canvas).Children.Remove(elementbox.Element);
                            }
                        }
                        if (xPos + elementbox.ElementSize.Width > ComputedWidth + ComX && !canAdd)
                        {
                            if (elementbox.ElementSize.Width > ComputedWidth)
                            {
                                elementbox.SplitLongText(ComputedWidth);
                            }
                            CheckCanAddToPreviousBox(elementbox);
                            if (!(Inlines[0] == inline && inline.ElementBoxes[0] == elementbox))
                            {
                                lineInfo = new LineInfo();
                                lineInfo.Block = this;
                                LineInfo.Add(lineInfo);
                                lineInfo.Width = 0;
                            }
                            xPos = ComX;
                        }
                        elementbox.LineInfo = lineInfo;
                        if (xPos + elementbox.ElementSize.Width > ComputedWidth + ComX && canAdd)
                        {
                            elementbox.IsAddedToLine = false;
                            CheckCanAddToPreviousBox(elementbox);
                        }
                        else
                        {
                            lineInfo.Width = lineInfo.Width + elementbox.ElementSize.Width;
                        }
                        lineInfo.Add(elementbox);
                        xPos = xPos + elementbox.ElementSize.Width;
                    }
                }
                foreach (LineInfo line in LineInfo)
                {
                    line.IsFirstLine = false;
                    line.LineSpacing = LineSpacing;
                    line.CalculateMaxHeight();
                    if (!IsInsideTable)
                    {
                        if (ContainsLine && LineIndex > -1)
                        {
                            LayoutViewer.LineInfos.Insert(LineIndex, line);
                        }
                        else
                        {
                            LayoutViewer.LineInfos.Add(line);
                        }
                    }
                    else
                    {
                        if (AssociatedCell != null)
                        {
                            if (ContainsLine && LineIndex > -1)
                            {
                                AssociatedCell.LineInfos.Insert(LineIndex, line);
                            }
                            else
                            {
                                AssociatedCell.LineInfos.Add(line);
                            }
                        }
                    }
                    LineIndex++;

                    if (line.IsFirstLine)
                    {
                        startpointofblock = line.BoundingRectangle.Top;
                    }
                    line.BoundingRectangle = new Rect(ComX, startpointofblock, line.Width, line.Height);
                    line.ArrangeElementBoxes(TextAlignment, ComputedWidth);
                    if (ListType != ListType.None && LineInfo[0] == line)
                    {
                        ListItem item = new ListItem(this);
                        item.Line = line;
                        item.Size = line.MaxHeightForCalOffset;
                        if (item.GetBulletSize(item.Size) > Hanging - 5)
                        {
                            Hanging += 25;
                            recreate = true;
                            break;
                        }
                        else if (Hanging > 25 && item.GetBulletSize(item.Size) < 25 - 5)
                        {
                            Hanging = 25;
                            recreate = true;
                            break;
                        }
                        //double x = comX - Hanging;
                        double x = line.BoundingRectangle.Left - Hanging;
                        double y = line.BoundingRectangle.Top;
                        UIElement element = item.Render(new Point(x, y));
                        line.Elements.Add(element);
                    }
                    startpointofblock = line.BoundingRectangle.Bottom;
                }
            }
            else
            {
                UpdateBounds(size);
            }
            if (LayoutViewer != null)
            {
                LayoutViewer.ImageResizer.UpdateResizerLocation();
            }
            bool flag = false;
            if (recreate)
            {
                ArrangeElements();
            }
            else
            {
                IsArranged = true;
                WrapToNextParagraphs(previousendpoint, size, ref flag);
                CreateNewLines = false;
            }

            if (flag)
                return NextBlock;
            }
            return null;
        }

        /// <summary>
        /// It Extracts the selected text.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startcell"></param>
        /// <param name="endcell"></param>
        /// <param name="blocks"></param>
        /// <param name="IsStarted"></param>
        /// <param name="stringbuilder"></param>
        /// <returns></returns>
        internal override int Search(TextPosition start, TextPosition end, TableCellAdv startcell, TableCellAdv endcell, ref BlockCollection<BlockAdv> blocks, ref bool IsStarted, ref System.Text.StringBuilder stringbuilder)
        {
            int firstInlineIndex = 0;
            int secondInlineIndex = 0;
            Inline newInline = null;
            BlockAdv tempParagraph = null;
            Inline firstInline = BaseParent.PositionHandler.GetInlineFromIndex(start.Paragraph, start.Index, ref firstInlineIndex);
            Inline secondInline = BaseParent.PositionHandler.GetInlineFromIndex(end.Paragraph, end.Index, ref secondInlineIndex);
            isStarted = IsStarted;

            if (Inlines.Count > 0)
            {
                if (isStarted)
                    tempParagraph = CreateBlock();
                foreach (Inline inline in Inlines)
                {
                    if (firstInline != null && firstInline == inline)
                    {
                        if (!firstInline.IsUIContainer)
                        {
                            tempParagraph = CreateBlock();
                            newInline = inline.CreatInline();
                            if (!inline.IsImageContainer)
                            {
                                if (inline.InternalText.Length != firstInlineIndex)
                                    newInline.InternalText = inline.InternalText.Substring(firstInlineIndex);
                                if (!string.IsNullOrEmpty(newInline.InternalText))
                                {
                                    stringbuilder.Append(newInline.InternalText);
                                    tempParagraph.Inlines.Add(newInline);
                                }
                            }
                            else if (inline.IsImageContainer && start.IsZeroIndex())
                            {
                                tempParagraph.Inlines.Add(newInline);
                            }

                            if (!blocks.Contains(tempParagraph) && tempParagraph.Inlines.Count > 0)
                            {
                                blocks.Add(tempParagraph);
                            }
                        }
                        isStarted = true;
                    }
                    else if (secondInline != null && secondInline == inline)
                    {
                        if (!secondInline.IsUIContainer)
                        {
                            if (tempParagraph == null)
                            {
                                tempParagraph = CreateBlock();
                            }
                            newInline = inline.CreatInline();
                            if (!inline.IsImageContainer)
                            {
                                if (newInline.InternalText.Length != secondInlineIndex)
                                    newInline.InternalText = inline.InternalText.Substring(0, secondInlineIndex);
                                if (!string.IsNullOrEmpty(newInline.InternalText))
                                {
                                    stringbuilder.Append(newInline.InternalText);
                                    tempParagraph.Inlines.Add(newInline);
                                }
                            }
                            else
                            {
                                tempParagraph.Inlines.Add(newInline);
                            }

                            if (!blocks.Contains(tempParagraph) && tempParagraph.Inlines.Count > 0)
                            {
                                blocks.Add(tempParagraph);
                            }
                        }
                        isStarted = false;
                        return 0;
                    }
                    else if (isStarted)
                    {
                        if (!inline.IsUIContainer)
                        {
                            if (tempParagraph == null)
                            {
                                tempParagraph = CreateBlock();
                            }
                            newInline = inline.CreatInline();
                            newInline.InternalText = inline.InternalText;
                            tempParagraph.Inlines.Add(newInline);
                            if (!string.IsNullOrEmpty(newInline.InternalText))
                                stringbuilder.Append(newInline.InternalText);
                            if (!blocks.Contains(tempParagraph))
                            {
                                blocks.Add(tempParagraph);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!isStarted && start.Paragraph == this)
                {
                    tempParagraph = CreateBlock();
                    blocks.Add(tempParagraph);
                    isStarted = true;
                    return 1;
                }
                else if (isStarted && end.Paragraph == this)
                {
                    tempParagraph = CreateBlock();
                    blocks.Add(tempParagraph);
                    isStarted = false;
                    return 0;
                }
                else if (isStarted)
                {
                    tempParagraph = CreateBlock();
                    blocks.Add(tempParagraph);
                    return -1;
                }
            }
            if (isStarted)
            {
                stringbuilder.Append("\r\n");
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// It cuts the selected text.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startcell"></param>
        /// <param name="endcell"></param>
        /// <param name="IsStarted"></param>
        /// <param name="removedblocks"></param>
        /// <param name="affectedblocks"></param>
        /// <returns></returns>
        internal override int SearchToCut(TextPosition start, TextPosition end, TableCellAdv startcell, TableCellAdv endcell, ref bool IsStarted, ref BlockCollection<BlockAdv> removedblocks, ref BlockCollection<BlockAdv> affectedblocks)
        {
            int firstInlineIndex = 0;
            int secondInlineIndex = 0;
            Inline firstInline = BaseParent.PositionHandler.GetInlineFromIndex(start.Paragraph, start.Index, ref firstInlineIndex);
            Inline secondInline = BaseParent.PositionHandler.GetInlineFromIndex(end.Paragraph, end.Index, ref secondInlineIndex);

            bool started = IsStarted;
            InlineCollection tempInlines = new InlineCollection();

            foreach (Inline inline in Inlines)
            {
                if (firstInline == inline)
                {
                    if (firstInline == secondInline)
                    {
                        if (firstInline.IsImageContainer || firstInline.IsUIContainer)
                        {
                            tempInlines.Add(firstInline);
                        }
                        else
                        {
                            firstInline.InternalText = firstInline.InternalText.Substring(0, firstInlineIndex) + firstInline.InternalText.Substring(secondInlineIndex);
                            firstInline.MeasureElements();
                            CreateNewLines = true;
                            affectedblocks.Add(this);
                            if (string.IsNullOrEmpty(firstInline.InternalText))
                            {
                                tempInlines.Add(firstInline);
                            }
                        }

                        foreach (Inline inln in tempInlines)
                        {
                            Inlines.Remove(inln);
                            if (Inlines.Count == 0)
                            {
                                if (!removedblocks.Contains(this))
                                {
                                    removedblocks.Add(this);
                                    if (affectedblocks.Contains(this))
                                    {
                                        affectedblocks.Remove(this);
                                    }
                                }
                            }
                            else
                            {
                                if (!affectedblocks.Contains(this))
                                {
                                    CreateNewLines = true;
                                    affectedblocks.Add(this);
                                }
                            }
                        }
                        return 0;
                    }

                    if (!firstInline.IsImageContainer && !firstInline.IsUIContainer && firstInlineIndex != 1)
                    {
                        firstInline.InternalText = firstInline.InternalText.Substring(0, firstInlineIndex);

                        firstInline.MeasureElements();
                        if (string.IsNullOrEmpty(firstInline.InternalText))
                        {
                            tempInlines.Add(firstInline);
                        }
                    }
                    else if ((firstInline.IsImageContainer || firstInline.IsUIContainer) && firstInlineIndex == 0)
                    {
                        tempInlines.Add(firstInline);
                    }
                    firstInline.Paragraph.CreateNewLines = true;
                    affectedblocks.Add(firstInline.Paragraph);
                    started = true;
                }
                else if (secondInline == inline)
                {
                    if (secondInline.IsImageContainer || secondInline.IsUIContainer)
                    {
                        tempInlines.Add(secondInline);
                    }
                    else
                    {
                        secondInline.InternalText = secondInline.InternalText.Substring(secondInlineIndex);
                        secondInline.MeasureElements();
                    }
                    if (!affectedblocks.Contains(secondInline.Paragraph))
                    {
                        secondInline.Paragraph.CreateNewLines = true;
                        affectedblocks.Add(secondInline.Paragraph);
                    }
                    if (string.IsNullOrEmpty(secondInline.InternalText))
                    {
                        tempInlines.Add(secondInline);
                    }
                    started = false;
                    break;
                }
                else if (started)
                {
                    tempInlines.Add(inline);
                }
            }

            if (started && Inlines.Count == 0)
            {
                if (!removedblocks.Contains(this))
                {
                    removedblocks.Add(this);
                }
                if (end.Paragraph == this)
                    started = false;
            }
            else if (start.Paragraph == this && Inlines.Count == 0)
            {
                if (!removedblocks.Contains(this))
                {
                    removedblocks.Add(this);
                }
                started = true;
                if (start.IsEqual(end))
                {
                    started = false;
                }
            }
            else if (end.Paragraph == this && Inlines.Count == 0)
            {
                if (!removedblocks.Contains(this))
                {
                    removedblocks.Add(this);
                }

                started = false;
            }

            foreach (Inline inline in tempInlines)
            {
                Inlines.Remove(inline);
                if (Inlines.Count == 0)
                {
                    if (!removedblocks.Contains(this))
                    {
                        removedblocks.Add(this);
                        if (affectedblocks.Contains(this))
                        {
                            affectedblocks.Remove(this);
                        }
                    }
                }
                else
                {
                    if (!affectedblocks.Contains(this))
                    {
                        CreateNewLines = true;
                        affectedblocks.Add(this);
                    }
                }
            }

            tempInlines.Clear();

            if (started)
                return 1;

            if (IsStarted && !started)
                return 0;

            return -1;
        }

        /// <summary>
        /// It selects the text.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="IsStarted"></param>
        /// <param name="blocks"></param>
        /// <returns></returns>
        internal override int SearchToSelect(TextPosition start, TextPosition end, ref bool IsStarted, ref BlockCollection<BlockAdv> blocks)
        {
            bool started = IsStarted;
            if (start.Paragraph == this)
            {
                started = true;
                if (!blocks.Contains(this))
                    blocks.Add(this);
                if (end.Paragraph == this)
                {
                    started = false;
                    return 0;
                }
            }
            else if (end.Paragraph == this)
            {
                started = false;
                if (!blocks.Contains(this))
                    blocks.Add(this);
            }
            else if (started)
            {
                if (!blocks.Contains(this))
                    blocks.Add(this);
            }
            if (started)
                return 1;

            if (IsStarted && !started)
                return 0;

            return -1;
        }

        /// <summary>
        /// It extracts the Inlines.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="started"></param>
        /// <param name="affectedblocks"></param>
        /// <param name="inlinesToFormat"></param>
        /// <returns></returns>
        internal override int ExtractInlines(TextPosition start, TextPosition end, ref bool started, ref BlockCollection<BlockAdv> affectedblocks, ref List<Inline> inlinesToFormat)
        {
            Inline newInline = null;
            int firstInlineIndex = 0;
            int secondInlineIndex = 0;
            TableRowAdv row1 = null;
            TableCellAdv cell1 = null;
            TableRowAdv row2 = null;
            TableCellAdv cell2 = null;
            BlockAdv veryfirstblk = start.Paragraph;
            BlockAdv lastblk = end.Paragraph;

            Inline firstInline = BaseParent.PositionHandler.GetInlineFromIndex(start.Paragraph, start.Index, ref firstInlineIndex);
            Inline secondInline = BaseParent.PositionHandler.GetInlineFromIndex(end.Paragraph, end.Index, ref secondInlineIndex);

            BlockAdv firstblk = BaseParent.Document.GetBlockFromVirtualPosition(start.VirtualPosition, ref row1, ref cell1);

            BlockAdv secondblk = BaseParent.Document.GetBlockFromVirtualPosition(end.VirtualPosition, ref row2, ref cell2);

            if (firstblk !=null && firstblk.IsTable && cell1 != null)
            {
                veryfirstblk = cell1.Blocks[0];
                while (veryfirstblk.IsTable)
                {
                    veryfirstblk = (veryfirstblk as TableAdv).GetFirstBlockInFirstCell();
                }
                firstInline = BaseParent.PositionHandler.GetInlineFromIndex(veryfirstblk as ParagraphAdv, "0", ref firstInlineIndex);
            }
            if (secondblk != null && secondblk.IsTable && cell2 != null)
            {
                lastblk = cell2.Blocks[cell2.Blocks.Count - 1];
                while (lastblk.IsTable)
                {
                    lastblk = (lastblk as TableAdv).GetLastBlockInLastCell();
                }
                secondInline = BaseParent.PositionHandler.GetInlineFromIndex(lastblk as ParagraphAdv, lastblk.Length(), ref secondInlineIndex);
            }

            if (firstInline == null)
            {
                if (veryfirstblk !=null && veryfirstblk == this)
                {
                    started = true;
                    return 1;
                }
            }
            if (secondInline == null)
            {
                if (lastblk !=null && lastblk == this)
                {
                    started = false;
                    return 0;
                }
            }

            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (firstInline != null && firstInline == inline)
                {
                    if (!firstInline.IsImageContainer && !firstInline.IsUIContainer)
                    {
                        if (firstInlineIndex != 0)
                        {
                            newInline = firstInline.CreatInline();
                            newInline.Paragraph = firstInline.Paragraph;
                            newInline.InternalText = firstInline.InternalText.Substring(firstInlineIndex);
                            newInline.MeasureElements();
                            firstInline.InternalText = firstInline.InternalText.Substring(0, firstInlineIndex);
                            firstInline.MeasureElements();
                            if (!string.IsNullOrEmpty(newInline.InternalText))
                            {
                                int index = firstInline.Paragraph.Inlines.IndexOf(firstInline);
                                firstInline.Paragraph.Inlines.Insert(++index, newInline);
                                inlinesToFormat.Add(newInline);
                            }
                        }
                        else
                        {
                            inlinesToFormat.Add(firstInline);
                        }

                        if (!affectedblocks.Contains(this))
                        {
                            CreateNewLines = true;
                            affectedblocks.Add(firstInline.Paragraph);
                        }
                    }
                    started = true;
                }
                else if (secondInline != null && secondInline == inline)
                {
                    if (!secondInline.IsImageContainer && !secondInline.IsUIContainer)
                    {
                        if (secondInline.InternalText.Length != secondInlineIndex)
                        {
                            newInline = secondInline.CreatInline();
                            newInline.InternalText = secondInline.InternalText.Substring(secondInlineIndex);
                            secondInline.InternalText = secondInline.InternalText.Substring(0, secondInlineIndex);
                            if (!string.IsNullOrEmpty(newInline.InternalText))
                            {
                                secondInline.MeasureElements();
                                newInline.MeasureElements();
                                int index = secondInline.Paragraph.Inlines.IndexOf(secondInline);
                                secondInline.Paragraph.Inlines.Insert(++index, newInline);
                            }
                        }
                        inlinesToFormat.Add(secondInline);

                        if (!affectedblocks.Contains(this))
                        {
                            CreateNewLines = true;
                            affectedblocks.Add(this);
                        }
                    }
                    started = false;
                    return 0;
                }
                else if (started)
                {
                    if (!inlinesToFormat.Contains(inline))
                    {
                        inlinesToFormat.Add(inline);
                    }
                }
            }

            if (started)
            {
                if (!affectedblocks.Contains(this))
                {
                    CreateNewLines = true;
                    affectedblocks.Add(this);
                }
                return 1;
            }
            return -1;
        }


        /// <summary>
        /// It copies the Paragraph instance.
        /// </summary>
        /// <returns></returns>
        public override BlockAdv CopyBlock()
        {
            ParagraphAdv para = this.CreateNewParagraph();
            foreach (Inline inline in this.Inlines)
            {
                if (inline is SpanAdv)
                {
                    SpanAdv span = inline as SpanAdv;
                    SpanAdv newSpan = span.CreateNewSpan();
                    newSpan.Text = span.Text;
                    para.Inlines.Add(newSpan);
                }
                else if (inline is HyperlinkAdv)
                {
                    HyperlinkAdv hyperlink = inline as HyperlinkAdv;
                    HyperlinkAdv newHyperlink = hyperlink.CreateNewHyperlink();
                    newHyperlink.Text = hyperlink.Text;
                    para.Inlines.Add(newHyperlink);
                }
                else if (inline is ImageContainerAdv)
                {
                    ImageContainerAdv src = inline as ImageContainerAdv;
                    ImageContainerAdv image = new ImageContainerAdv();
                    image.ImageSource = src.ImageSource;
                    image.ImageBytes = src.ImageBytes;
                    image.Width = src.Width;
                    image.Height = src.Height;
                    para.Inlines.Add(image);
                }
            }
            return para;
        }

        /// <summary>
        /// It updates the Bounds.
        /// </summary>
        /// <param name="size"></param>
        internal void UpdateBounds(Size size)
        {
            double end = EndPoint;
            Width = size.Width;
            int lineIndex = 0;
            bool containsLine = false;

            double startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : Margin.Top;
            double computedWidth = Width - (Margin.Left + Margin.Right + LeftIndent + RightIndent);
            double comX = Margin.Left + LeftIndent;

            if (IsInsideTable)
            {
                startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : AssociatedCell.CellElementBox.BoundingRectangle.Y;
                comX = AssociatedCell.CellElementBox.BoundingRectangle.X + AssociatedCell.CellMargin.Left;
                computedWidth = Width - AssociatedCell.CellMargin.Left - AssociatedCell.CellMargin.Right;
            }

            foreach (LineInfo line in LineInfo)
            {
                if (line.RenderingOption == RenderingOptions.None)
                {
                    line.RenderingOption = RenderingOptions.RemoveAndAdd;
                }
            }

            if (LayoutViewer is FlowLayoutViewer)
            {
                computedWidth = Width - (LeftIndent + RightIndent);
                comX = LeftIndent;
                startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : 0;
            }
            if (ListType != ListType.None)
            {
                comX += Hanging;
                computedWidth -= Hanging;
            }

            if (LayoutViewer is PageLayoutViewer)
            {
                foreach (LineInfo line in LineInfo)
                {
                    if (IsInsideTable)
                    {
                        if (AssociatedCell.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                containsLine = true;
                                lineIndex = AssociatedCell.LineInfos.IndexOf(line);
                            }
                            AssociatedCell.LineInfos.Remove(line);
                        }
                    }
                    else
                    {
                        if (LayoutViewer.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                containsLine = true;
                                lineIndex = LayoutViewer.LineInfos.IndexOf(line);
                            }
                            LayoutViewer.LineInfos.Remove(line);
                        }
                    }
                }

                foreach (LineInfo line in LineInfo)
                {
                    line.IsFirstLine = false;
                    line.LineSpacing = LineSpacing;
                    line.CalculateMaxHeight();
                    if (!IsInsideTable)
                    {
                        if (containsLine && lineIndex > -1)
                        {
                            LayoutViewer.LineInfos.Insert(lineIndex, line);
                        }
                        else
                        {
                            LayoutViewer.LineInfos.Add(line);
                        }
                    }
                    else
                    {
                        if (AssociatedCell != null)
                        {
                            if (containsLine && lineIndex > -1)
                            {
                                AssociatedCell.LineInfos.Insert(lineIndex, line);
                            }
                            else
                            {
                                AssociatedCell.LineInfos.Add(line);
                            }
                        }
                    }
                    lineIndex++;
                    if (line.IsFirstLine)
                    {
                        startPoint = line.BoundingRectangle.Top;
                    }
                    line.BoundingRectangle = new Rect(comX, startPoint, line.Width, line.Height);
                    //double comWidth = ListType != ListType.None ? computedWidth - Hanging : computedWidth;
                    line.ArrangeElementBoxes(TextAlignment, computedWidth);
                    if (ListType != ListType.None && LineInfo[0] == line && AssociatedListItem != null)
                    {
                        if (line.Elements.Contains(AssociatedListItem.Item))
                            line.Elements.Remove(AssociatedListItem.Item);
                        AssociatedListItem.Line = line;
                        //double x = comX - Hanging;
                        double x = line.BoundingRectangle.Left - Hanging;
                        double y = line.BoundingRectangle.Top;
                        AssociatedListItem.ValidateListNumber(new Point(x, y));
                        line.Elements.Add(AssociatedListItem.Item);
                    }
                    startPoint = line.BoundingRectangle.Bottom;
                }
            }
            else
            {
                foreach (LineInfo line in LineInfo)
                {
                    if (!IsInsideTable)
                    {
                        if (!LayoutViewer.LineInfos.Contains(line))
                        {
                            LayoutViewer.LineInfos.Add(line);
                        }
                    }
                    else
                    {
                        if (!AssociatedCell.LineInfos.Contains(line))
                        {
                            AssociatedCell.LineInfos.Add(line);
                        }
                    }
                    line.BoundingRectangle = new Rect(comX, startPoint, line.Width, line.Height);
                    line.ArrangeElementBoxes(GetTextAlignment(), computedWidth);
                    if (ListType != ListType.None && LineInfo[0] == line && AssociatedListItem != null)
                    {
                        if (line.Elements.Contains(AssociatedListItem.Item))
                            line.Elements.Remove(AssociatedListItem.Item);
                        AssociatedListItem.Line = line;
                        //double x = comX - Hanging;
                        double x = line.BoundingRectangle.Left - Hanging;
                        double y = line.BoundingRectangle.Top;
                        AssociatedListItem.ValidateListNumber(new Point(x, y));
                        line.Elements.Add(AssociatedListItem.Item);
                    }
                    startPoint = line.BoundingRectangle.Bottom;
                }
            }
        }

        private TextAlignment GetTextAlignment()
        {
            if (this.IsInsideTable)
            {
                if (TextAlignment == TextAlignment.Left)
                {
                    if (AssociatedCell != null && AssociatedCell.TextAlignment == TextAlignment.Left)
                    {
                        if (AssociatedCell.OwnerRow.TextAlignment == TextAlignment.Left)
                        {
                            if (AssociatedCell.ColumnAlignment == TextAlignment.Left)
                            {
                                return AssociatedCell.ColumnAlignment;
                            }
                            else
                            {
                                return this.TextAlignment;
                            }
                        }
                        else
                        {
                            return AssociatedCell.OwnerRow.TextAlignment;
                        }
                    }
                    else
                    {
                        return AssociatedCell.TextAlignment;
                    }
                }
                else
                {
                    return TextAlignment;
                }
            }
            else
            {
                return this.TextAlignment;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        internal override void ArrangeElements(int index)
        {
            if (index <= 0)
                ArrangeElements();
            else
            {
                double end = EndPoint;
                int lineIndex = 0;
                bool containsLine = false;
                Size size = new Size(0.0, 0.0);
                LayoutViewer layoutviewer = null;

                if (IsInsideTable)
                    layoutviewer = GetLayoutViewer();
                else
                    layoutviewer = LayoutViewer;

                if (IsInsideTable)
                {
                    TableAdv owner = AssociatedCell.OwnerTable;
                    double preWidth = AssociatedCell.DesiredWidth;
                    SectionAdv section = GetSection();

                    double subracted = owner.TableWidth - section.PageContentMargin.Left - section.PageContentMargin.Right;

                    TableLayoutCalculator.MeasureTableLayout(owner, subracted);

                    if (preWidth != AssociatedCell.DesiredWidth)
                    {
                        while (owner.AssociatedCell != null)
                        {
                            owner = owner.AssociatedCell.OwnerTable;
                        }
                        owner.ArrangeElements();
                        return;
                    }
                }

                if (layoutviewer != null)
                {
                    if (!IsInsideTable)
                    {
                        if (layoutviewer is PageLayoutViewer)
                        {
                            Width = Section.PageSize.Width;
                            size = Section.PageSize;
                        }
                        else
                        {
                            Width = layoutviewer.AvailableSize.Width;
                            size = layoutviewer.AvailableSize;
                        }
                    }
                    else
                    {
                        Width = AssociatedCell.DesiredWidth;
                        size = new Size(AssociatedCell.DesiredWidth, 0);
                    }
                }
                Inline startInline = null;
                ElementBox startElementBox = null;

                for (int i = index; i < LineInfo.Count; i = index)
                {
                    if (!IsInsideTable)
                    {
                        if (LayoutViewer.LineInfos.Contains(LineInfo[i]))
                        {
                            if (startInline == null)
                                startInline = LineInfo[i].ElementBoxes[0].Inline;
                            if (startElementBox == null)
                                startElementBox = LineInfo[i].ElementBoxes[0];
                            containsLine = true;
                            if (i == index)
                                lineIndex = LayoutViewer.LineInfos.IndexOf(LineInfo[i]);
                            LayoutViewer.LineInfos.Remove(LineInfo[i]);
                            LineInfo[i].ClearElementBoxes();
                            LineInfo.Remove(LineInfo[i]);
                        }
                    }
                    else
                    {
                        if (AssociatedCell.LineInfos.Contains(LineInfo[i]))
                        {
                            if (startInline == null)
                                startInline = LineInfo[i].ElementBoxes[0].Inline;
                            if (startElementBox == null)
                                startElementBox = LineInfo[i].ElementBoxes[0];
                            containsLine = true;
                            if (i == index)
                                lineIndex = AssociatedCell.LineInfos.IndexOf(LineInfo[i]);
                            AssociatedCell.LineInfos.Remove(LineInfo[i]);
                            LineInfo[i].ClearElementBoxes();
                            LineInfo.Remove(LineInfo[i]);
                        }
                    }
                }

                double computedWidth = Width - (Margin.Left + Margin.Right + LeftIndent + RightIndent);
                double comX = Margin.Left + LeftIndent;
                double startPoint = LineInfo.Count > 0 && LineInfo[LineInfo.Count - 1] != null ? LineInfo[LineInfo.Count - 1].BoundingRectangle.Bottom : PreviousBlock != null ? PreviousBlock.EndPoint : Margin.Top;

                if (IsInsideTable)
                {
                    startPoint = LineInfo.Count > 0 && LineInfo[LineInfo.Count - 1] != null ? LineInfo[LineInfo.Count - 1].BoundingRectangle.Bottom : PreviousBlock != null ? PreviousBlock.EndPoint : AssociatedCell.CellElementBox.BoundingRectangle.Y;
                    comX = AssociatedCell.CellElementBox.BoundingRectangle.X + AssociatedCell.CellMargin.Left;
                    computedWidth = Width - AssociatedCell.CellMargin.Left - AssociatedCell.CellMargin.Right;
                }
                if (LayoutViewer is FlowLayoutViewer)
                {
                    computedWidth = Width - (LeftIndent + RightIndent);
                    comX = LeftIndent;
                    startPoint = LineInfo.Count > 0 && LineInfo[LineInfo.Count - 1] != null ? LineInfo[LineInfo.Count - 1].BoundingRectangle.Bottom : PreviousBlock != null ? PreviousBlock.EndPoint : 0;
                }
                double xPos = comX;

                if (ListType != ListType.None)
                {
                    comX += Hanging;
                    computedWidth -= Hanging;
                }

                LineInfo lineInfo = null;

                int inlineIndex = Inlines.IndexOf(startInline);
                int elementIndex = startInline.ElementBoxes.IndexOf(startElementBox);

                for (int j = inlineIndex; j < Inlines.Count; j++)
                {
                    Inlines[j].Paragraph = this;

                    for (int i = elementIndex; i < Inlines[j].ElementBoxes.Count; i++)
                    {
                        ElementBox elementbox = Inlines[j].ElementBoxes[i];
                        CheckForFitTocontent(elementbox, computedWidth);
                        elementbox.IsAddedToLine = true;
                        bool canAdd = CanAddToLine(elementbox);
                        elementIndex = 0;

                        if (elementbox.Element != null && elementbox.Element.Parent != null)
                        {
                            if (elementbox.Element.Parent is Canvas)
                            {
                                (elementbox.Element.Parent as Canvas).Children.Remove(elementbox.Element);
                            }
                        }
                        if (xPos + elementbox.ElementSize.Width > computedWidth + comX && !canAdd)
                        {
                            //double comWidth = ListType != ListType.None ? computedWidth - Hanging : computedWidth;
                            if (elementbox.ElementSize.Width > computedWidth)
                            {
                                elementbox.SplitLongText(computedWidth);
                            }

                            CheckCanAddToPreviousBox(elementbox);

                            lineInfo = new LineInfo();
                            lineInfo.Block = this;
                            LineInfo.Add(lineInfo);
                            lineInfo.Width = 0;
                            xPos = comX;
                        }

                        if (lineInfo == null)
                        {
                            lineInfo = new LineInfo();
                            lineInfo.Block = this;
                            LineInfo.Add(lineInfo);
                        }

                        elementbox.LineInfo = lineInfo;

                        if (xPos + elementbox.ElementSize.Width > computedWidth + comX && canAdd)
                        {
                            elementbox.IsAddedToLine = false;
                            CheckCanAddToPreviousBox(elementbox);
                        }
                        else
                            lineInfo.Width = lineInfo.Width + elementbox.ElementSize.Width;


                        lineInfo.Add(elementbox);

                        xPos = xPos + elementbox.ElementSize.Width;
                    }
                }

                for (int i = index; i < LineInfo.Count; i++)
                {
                    LineInfo[i].IsFirstLine = false;
                    LineInfo[i].LineSpacing = LineSpacing;
                    LineInfo[i].CalculateMaxHeight();
                    if (!IsInsideTable)
                    {
                        if (containsLine && lineIndex > -1)
                        {
                            LayoutViewer.LineInfos.Insert(lineIndex, LineInfo[i]);
                        }
                        else
                        {
                            LayoutViewer.LineInfos.Add(LineInfo[i]);
                        }
                    }
                    else
                    {
                        if (AssociatedCell != null)
                        {
                            if (containsLine && lineIndex > -1)
                            {
                                AssociatedCell.LineInfos.Insert(lineIndex, LineInfo[i]);
                            }
                            else
                            {
                                AssociatedCell.LineInfos.Add(LineInfo[i]);
                            }
                        }
                    }
                    lineIndex++;
                    if (LineInfo[i].IsFirstLine)
                    {
                        startPoint = LineInfo[i].BoundingRectangle.Top;
                    }
                    LineInfo[i].BoundingRectangle = new Rect(comX, startPoint, LineInfo[i].Width, LineInfo[i].Height);
                    //double comWidth = ListType != ListType.None ? computedWidth - Hanging : computedWidth;
                    LineInfo[i].ArrangeElementBoxes(TextAlignment, computedWidth);
                    startPoint = LineInfo[i].BoundingRectangle.Bottom;
                }
                if (LayoutViewer != null)
                {
                    LayoutViewer.ImageResizer.UpdateResizerLocation();
                }
                IsArranged = true;

                if (LineInfo.Count != 0)
                {
                    EndPoint = LineInfo.Last().BoundingRectangle.Bottom;

                    if (Math.Round(end) != Math.Round(EndPoint) && !IsArrangingParagraph)
                    {
                        if (NextBlock != null)
                        {
                            NextBlock.CreateNewLines = false;
                            if (NextBlock.LineInfo.Count == 0)
                                NextBlock.CreateNewLines = true;
                            NextBlock.ArrangeElements(size);
                        }
                    }
                    else if ((Math.Round(end) == Math.Round(EndPoint) && !IsArrangingParagraph))
                    {
                        bool flag = false;
                        if (NextBlock != null)
                        {
                            if (NextBlock.LineInfo.Count == 0)
                            {
                                NextBlock.CreateNewLines = true;
                                flag = true;
                            }
                            else
                            {
                                PageAdv page1 = LayoutViewer.GetPageFromLine(LineInfo.Last());
                                PageAdv page2 = LayoutViewer.GetPageFromLine(NextBlock.LineInfo.Last());
                                flag = page1 != page2;
                            }

                            if (flag)
                                NextBlock.ArrangeElements(size);
                        }
                    }

                    IsArrangingParagraph = false;
                }
                else
                {
                    EndPoint = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementbox"></param>
        /// <returns></returns>
        bool IsSpace(ElementBox elementbox)
        {
            if (elementbox.InternalText == " ")
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal double GetBulletSize(double size)
        {
            return Math.Round((24.07 * size) / 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool IsWhiteColor(Color c)
        {
            return c.A == 0 && c.B == 0 && c.G == 0 && c.R == 0;
        }

        /// <summary>
        /// Draws the inline
        /// </summary>
        /// <param name="inline"></param>
        internal void DrawHighlight(Inline inline)
        {
            foreach (ElementBox element in inline.ElementBoxes)
            {
                Path path = new Path();
                PathGeometry pathGeo = new PathGeometry();
                PathFigure pathFig = new PathFigure();
                Rect rect = element.BoundingRectangle;
                LineSegment lineSeg = new LineSegment();
                lineSeg.Point = new Point(rect.X - 1, rect.Bottom);
                pathFig.StartPoint = new Point(rect.X, rect.Bottom);
                pathFig.Segments.Add(lineSeg);
                LineSegment lineSeg1 = new LineSegment();
                lineSeg1.Point = new Point(rect.X - 1, rect.Top);
                pathFig.Segments.Add(lineSeg1);
                LineSegment lineSeg2 = new LineSegment();
                lineSeg2.Point = new Point(rect.Right, rect.Y);
                pathFig.Segments.Add(lineSeg2);
                LineSegment lineSeg3 = new LineSegment();
                lineSeg3.Point = new Point(rect.Right, rect.Bottom);
                pathFig.Segments.Add(lineSeg3);
                pathFig.IsClosed = true;
                pathGeo.Figures.Add(pathFig);
                path.Data = pathGeo;
                if (inline is SpanAdv)
                {
                    path.Fill = new SolidColorBrush((inline as SpanAdv).HighlightColor);
                    if ((element as TextElementBox).Highlightpath != null)
                    {
                        Path pth = (element as TextElementBox).Highlightpath;
                        if ((pth.Parent as Canvas) != null && (pth.Parent as Canvas).Children.Contains(pth))
                        {
                            (pth.Parent as Canvas).Children.Remove(pth);
                        }
                    }
                    (element as TextElementBox).Highlightpath = path;
                }
                else if (inline is HyperlinkAdv)
                {
                    path.Fill = new SolidColorBrush((inline as HyperlinkAdv).HighlightColor);
                    if ((element as HyperlinkElementBox).Highlightpath != null)
                    {
                        Path pth = (element as HyperlinkElementBox).Highlightpath;
                        if ((pth.Parent as Canvas) != null && (pth.Parent as Canvas).Children.Contains(pth))
                        {
                            (pth.Parent as Canvas).Children.Remove(pth);
                        }
                    }
                    (element as HyperlinkElementBox).Highlightpath = path;
                }

                if (LayoutViewer is PageLayoutViewer && path != null)
                {
                    element.AddDecorationToPage(path);
                }
            }
        }

        /// <summary>
        /// Draws the underline
        /// </summary>
        /// <param name="inline"></param>
        internal void DrawUnderline(Inline inline)
        {
            if (inline is SpanAdv || inline is HyperlinkAdv)
            {
                foreach (ElementBox element in inline.ElementBoxes)
                {
                    FrameworkElement textElement = element.Element;
                    double y = Canvas.GetTop(element.Element);
                    double x1 = element.Location.X;
                    double y1 = y + element.ElementSize.Height;
                    double x2 = element.Location.X + element.ElementSize.Width;
                    double y2 = y + element.ElementSize.Height;
                    Line line = new Line();
                    line.X1 = x1;
                    line.X2 = x2;
                    line.Y1 = y1;
                    line.Y2 = y2;

                    if (inline is SpanAdv)
                    {
                        line.Stroke = new SolidColorBrush((inline as SpanAdv).Foreground);
                        if ((element as TextElementBox).Underline != null)
                        {
                            Line lne = (element as TextElementBox).Underline;
                            if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                            {
                                (lne.Parent as Canvas).Children.Remove(lne);
                            }
                        }
                        (element as TextElementBox).Underline = line;
                    }
                    else if (inline is HyperlinkAdv)
                    {
                        line.Stroke = new SolidColorBrush((inline as HyperlinkAdv).Foreground);
                        if ((element as HyperlinkElementBox).Underline != null)
                        {
                            Line lne = (element as HyperlinkElementBox).Underline;
                            if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                            {
                                (lne.Parent as Canvas).Children.Remove(lne);
                            }
                        }
                        (element as HyperlinkElementBox).Underline = line;
                    }

                    if (LayoutViewer is PageLayoutViewer && line != null)
                    {
                        element.AddDecorationToPage(line);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the single strike through
        /// </summary>
        /// <param name="inline"></param>
        internal void DrawSingleStrikeThrough(Inline inline)
        {
            if (inline is SpanAdv || inline is HyperlinkAdv)
            {
                foreach (ElementBox element in inline.ElementBoxes)
                {
                    FrameworkElement textElement = element.Element;
                    double y = Canvas.GetTop(element.Element);
                    double x1 = element.Location.X;
                    double y1 = y + element.ElementSize.Height / 2;
                    double x2 = element.Location.X + element.ElementSize.Width;
                    double y2 = y + element.ElementSize.Height / 2;
                    Line line = new Line();
                    line.X1 = x1;
                    line.X2 = x2;
                    line.Y1 = y1;
                    line.Y2 = y2;

                    if (inline is SpanAdv)
                    {
                        line.Stroke = new SolidColorBrush((inline as SpanAdv).Foreground);
                        if ((element as TextElementBox).SingleStrike != null)
                        {
                            Line lne = (element as TextElementBox).SingleStrike;
                            if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                            {
                                (lne.Parent as Canvas).Children.Remove(lne);
                            }
                        }
                        (element as TextElementBox).SingleStrike = line;
                    }
                    else if (inline is HyperlinkAdv)
                    {
                        line.Stroke = new SolidColorBrush((inline as HyperlinkAdv).Foreground);
                        if ((element as HyperlinkElementBox).SingleStrike != null)
                        {
                            Line lne = (element as HyperlinkElementBox).SingleStrike;
                            if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                            {
                                (lne.Parent as Canvas).Children.Remove(lne);
                            }
                        }
                        (element as HyperlinkElementBox).SingleStrike = line;
                    }

                    if (LayoutViewer is PageLayoutViewer && line != null)
                    {
                        element.AddDecorationToPage(line);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the double strike through
        /// </summary>
        /// <param name="inline"></param>
        internal void DrawDoubleStrikeThrough(Inline inline)
        {
            if (inline is SpanAdv || inline is HyperlinkAdv)
            {
                foreach (ElementBox element in inline.ElementBoxes)
                {
                    FrameworkElement textElement = element.Element;
                    double y = Canvas.GetTop(element.Element);
                    double x1 = element.Location.X;
                    double y1 = y + element.ElementSize.Height / 2;
                    double x2 = element.Location.X + element.ElementSize.Width;
                    double y2 = y + element.ElementSize.Height / 2;
                    Line line = new Line();
                    line.X1 = x1;
                    line.X2 = x2;
                    line.Y1 = y1;
                    line.Y2 = y2;
                    Line line1 = new Line();
                    line1.X1 = element.Location.X;
                    line1.X2 = element.Location.X + element.ElementSize.Width;
                    line1.Y1 = y + element.ElementSize.Height / 2 + (line.StrokeThickness * 2);
                    line1.Y2 = y + element.ElementSize.Height / 2 + (line.StrokeThickness * 2);

                    if (inline is SpanAdv)
                    {
                        if ((element as TextElementBox).DoubleStrikes.Count == 2)
                        {
                            if ((element as TextElementBox).DoubleStrikes[0] != null)
                            {
                                Line lne = (element as TextElementBox).DoubleStrikes[0];
                                if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                                {
                                    (lne.Parent as Canvas).Children.Remove(lne);
                                }
                            }

                            if ((element as TextElementBox).DoubleStrikes[1] != null)
                            {
                                Line lne = (element as TextElementBox).DoubleStrikes[1];
                                if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                                {
                                    (lne.Parent as Canvas).Children.Remove(lne);
                                }
                            }
                        }
                        line.Stroke = new SolidColorBrush((inline as SpanAdv).Foreground);
                        line1.Stroke = new SolidColorBrush((inline as SpanAdv).Foreground);
                        (element as TextElementBox).DoubleStrikes.Clear();
                        (element as TextElementBox).DoubleStrikes.AddRange(new Line[] { line, line1 });
                    }
                    else if (inline is HyperlinkAdv)
                    {
                        if ((element as HyperlinkElementBox).DoubleStrikes.Count == 2)
                        {
                            if ((element as HyperlinkElementBox).DoubleStrikes[0] != null)
                            {
                                Line lne = (element as HyperlinkElementBox).DoubleStrikes[0];
                                if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                                {
                                    (lne.Parent as Canvas).Children.Remove(lne);
                                }
                            }

                            if ((element as HyperlinkElementBox).DoubleStrikes[1] != null)
                            {
                                Line lne = (element as HyperlinkElementBox).DoubleStrikes[1];
                                if ((lne.Parent as Canvas) != null && (lne.Parent as Canvas).Children.Contains(lne))
                                {
                                    (lne.Parent as Canvas).Children.Remove(lne);
                                }
                            }
                        }
                        line.Stroke = new SolidColorBrush((inline as HyperlinkAdv).Foreground);
                        line1.Stroke = new SolidColorBrush((inline as HyperlinkAdv).Foreground);
                        (element as HyperlinkElementBox).DoubleStrikes.Clear();
                        (element as HyperlinkElementBox).DoubleStrikes.AddRange(new Line[] { line, line1 });
                    }

                    if (LayoutViewer is PageLayoutViewer && line != null && line1 != null)
                    {
                        element.AddDecorationToPage(line);
                        element.AddDecorationToPage(line1);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the current paragraph
        /// </summary>
        /// <returns></returns>
        internal static ParagraphAdv GetCurrentParagraph()
        {
            return new ParagraphAdv();
        }

        /// <summary>
        /// Returns the ending position of the paragraph
        /// </summary>
        /// <returns></returns>
        internal TextPosition EndPosition()
        {
            TextPosition pos = null;
            if (Section != null && Section.Document != null)
            {
                pos = new TextPosition(Section.Document);
                pos.Index = Length();
                pos.Paragraph = this;
            }
            return pos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = string.Empty;
            foreach (Inline inline in Inlines)
            {
                str += inline.InternalText;
            }
            return str;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ListType
    {
        None,
        Bulleted,
        Numbered,
    }
}
