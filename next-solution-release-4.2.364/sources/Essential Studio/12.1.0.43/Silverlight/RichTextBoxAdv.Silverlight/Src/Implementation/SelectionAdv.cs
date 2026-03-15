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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    public class SelectionAdv : DependencyObject
    {
        #region

        internal Point StartPoint;

        internal Point EndPoint;

        internal double StartingIndex;

        internal double EndingIndex;

        internal Inline StartingInline;

        internal Inline EndingInline;

        internal int StartingPage = 0;

        internal int EndingPage = 0;

        private RichTextBoxAdv ownerControl;

        internal BlockCollection<BlockAdv> Blocks;

        internal DocumentAdv Document;

        internal BlockCollection<BlockAdv> AffectedBlocks;

        internal LayoutViewer LayoutViewer;

        List<Inline> inlinesToFormat;

        internal StringBuilder stringbuilder = new StringBuilder();

        private TextPosition start = null;
        private TextPosition end = null;
        internal bool m_mergetable = false;
        internal bool m_mergeparagraph = false;
        
        internal bool IsReadOnly
        {
            get
            {
                return OwnerControl.IsReadOnly;
            }
        }

        public bool IsCellSelected { get; set; }

        #endregion

        /// <summary>
        /// Initializes the new instance of SelectionAdv
        /// </summary>
        public SelectionAdv()
        {
            Blocks = new BlockCollection<BlockAdv>();
            AffectedBlocks = new BlockCollection<BlockAdv>();
#if !WPF
            Blocks.CollectionChanged += (sender, e) =>
                {
                    OwnerControl.PasteCommand.ExecuteChanged();
                };
#endif
        }

        /// <summary>
        /// Initializes the new instance of SelectionAdv
        /// </summary>
        /// <param name="ownerControl"></param>
        public SelectionAdv(RichTextBoxAdv ownerControl)
            : this()
        {
            OwnerControl = ownerControl;
        }

        internal List<LineInfo> SelectedLines;

        /// <summary>
        /// Gets the start position of the selection
        /// </summary>
        public TextPosition Start
        {
            get
            {
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
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            internal set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the owner control
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
        /// Gets or Sets the TextProperty dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SelectionAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or Sets the fill color
        /// </summary>
        public Brush SelectionFillColor
        {
            get
            {
                return (Brush)GetValue(SelectionFillColorProperty);
            }
            set
            {
                SetValue(SelectionFillColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the FillColorProperty dependency property
        /// </summary>
        public static readonly DependencyProperty SelectionFillColorProperty = DependencyProperty.Register("SelectionFillColor", typeof(Brush), typeof(SelectionAdv), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 142, 239))));

        /// <summary>
        /// Gets or Sets the stroke value
        /// </summary>
        public Brush SelectionStrokeColor
        {
            get
            {
                return (Brush)GetValue(SelectionStrokeColorProperty);
            }
            set
            {
                SetValue(SelectionStrokeColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the FillColorProperty dependency property
        /// </summary>
        public static readonly DependencyProperty SelectionStrokeColorProperty = DependencyProperty.Register("SelectionStrokeColor", typeof(Brush), typeof(SelectionAdv), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 142, 239))));

        /// <summary>
        /// Returns the selection path for an paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal Path GetSelectionPathForParagraph(ParagraphAdv paragraph)
        {
            bool flag = false;
            ObservableCollection<LineInfo> lineInfo = paragraph.LineInfo;
            Path path = new Path();
            PathGeometry pathGeo = new PathGeometry();
            PathFigure pathFig = null;
            int startIndex = 0;
            int endIndex = 0;
            if (lineInfo.Count != 0)
            {
                bool newStart = false;
                for (int i = 0; i < lineInfo.Count; i++)
                {
                    endIndex = i;
                    LineInfo line = lineInfo[i];
                    if (lineInfo.First().Equals(line) || newStart)
                    {
                        newStart = false;
                        startIndex = i;
                        pathFig = new PathFigure();
                        pathGeo.Figures.Add(pathFig);
                        pathFig.StartPoint = new Point(lineInfo[i].Location.X, lineInfo[i].BoundingRectangle.Bottom);
                        LineSegment lineSeg = new LineSegment();
                        lineSeg.Point = new Point(line.Location.X, line.BoundingRectangle.Bottom);
                        pathFig.Segments.Add(lineSeg);
                        LineSegment lineSeg1 = new LineSegment();
                        lineSeg1.Point = line.Location;
                        pathFig.Segments.Add(lineSeg1);
                    }
                    if (lineInfo.Last().Equals(line))
                    {
                        LineSegment lineSeg = new LineSegment();
                        lineSeg.Point = new Point(line.BoundingRectangle.Right, line.Location.Y);
                        pathFig.Segments.Add(lineSeg);
                        LineSegment lineSeg1 = new LineSegment();
                        lineSeg1.Point = new Point(line.BoundingRectangle.Right, line.BoundingRectangle.Bottom);
                        pathFig.Segments.Add(lineSeg1);
                        flag = true;
                    }
                    else
                    {
                        LineSegment lineSeg = new LineSegment();
                        lineSeg.Point = new Point(line.BoundingRectangle.Right, line.Location.Y);
                        pathFig.Segments.Add(lineSeg);
                        LineSegment lineSeg1 = new LineSegment();
                        lineSeg1.Point = new Point(line.BoundingRectangle.Right, line.BoundingRectangle.Bottom);
                        pathFig.Segments.Add(lineSeg1);
                        if (lineInfo[i].Location.X > lineInfo[i + 1].BoundingRectangle.Right || lineInfo[i].BoundingRectangle.Right < lineInfo[i + 1].Location.X)
                        {
                            flag = true;
                            newStart = true;
                        }
                    }

                    if (flag)
                    {
                        for (int j = endIndex; j >= startIndex; j--)
                        {
                            LineSegment lineSeg = new LineSegment();
                            lineSeg.Point = new Point(lineInfo[j].Location.X, lineInfo[j].BoundingRectangle.Bottom);
                            pathFig.Segments.Add(lineSeg);
                            LineSegment lineSeg1 = new LineSegment();
                            lineSeg1.Point = new Point(lineInfo[j].Location.X, lineInfo[j].Location.Y);
                            pathFig.Segments.Add(lineSeg1);
                        }

                        flag = false;
                    }
                }
            }
            else
            {
                return null;
            }
            path.Data = pathGeo;
            path.Stroke = SelectionStrokeColor;
            path.Fill = SelectionFillColor;
            path.Opacity = 0.5;
            return path;
        }

        /// <summary>
        /// Returns selection path using lineinfo
        /// </summary>
        /// <param name="list"></param>
        internal Path GetSelectionPathForLineInfo(List<LineInfo> lineInfo)
        {
            Path path = new Path();
            PathGeometry pathGeo = new PathGeometry();
            PathFigure pathFig = null;
            bool flag = false;
            int startIndex = 0;
            int endIndex = 0;
            if (lineInfo.Count != 0)
            {
                bool newStart = false;
                for (int i = 0; i < lineInfo.Count; i++)
                {
                    endIndex = i;
                    LineInfo line = lineInfo[i];
                    if (!line.IsTableLine)
                    {
                        if (lineInfo.First().Equals(line) || newStart)
                        {
                            newStart = false;
                            startIndex = i;
                            pathFig = new PathFigure();
                            pathGeo.Figures.Add(pathFig);
                            pathFig.StartPoint = new Point(lineInfo[i].Location.X, lineInfo[i].BoundingRectangle.Bottom);
                            LineSegment lineSeg = new LineSegment();
                            lineSeg.Point = new Point(line.Location.X, line.BoundingRectangle.Bottom);
                            pathFig.Segments.Add(lineSeg);
                            LineSegment lineSeg1 = new LineSegment();
                            lineSeg1.Point = line.Location;
                            pathFig.Segments.Add(lineSeg1);
                        }
                        if (lineInfo.Last().Equals(line) && pathFig !=null)
                        {
                            LineSegment lineSeg = new LineSegment();
                            lineSeg.Point = new Point(line.BoundingRectangle.Right, line.Location.Y);
                            pathFig.Segments.Add(lineSeg);
                            LineSegment lineSeg1 = new LineSegment();
                            lineSeg1.Point = new Point(line.BoundingRectangle.Right, line.BoundingRectangle.Bottom);
                            pathFig.Segments.Add(lineSeg1);
                            flag = true;
                        }
                        else if(pathFig !=null)
                        {
                            LineSegment lineSeg = new LineSegment();
                            lineSeg.Point = new Point(line.BoundingRectangle.Right, line.Location.Y);
                            pathFig.Segments.Add(lineSeg);
                            LineSegment lineSeg1 = new LineSegment();
                            lineSeg1.Point = new Point(line.BoundingRectangle.Right, line.BoundingRectangle.Bottom);
                            pathFig.Segments.Add(lineSeg1);
                            if (lineInfo[i].Location.X > lineInfo[i + 1].BoundingRectangle.Right || lineInfo[i].BoundingRectangle.Right < lineInfo[i + 1].Location.X)
                            {
                                flag = true;
                                newStart = true;
                            }
                        }

                        if (flag && pathFig !=null)
                        {
                            for (int j = endIndex; j >= startIndex; j--)
                            {
                                LineSegment lineSeg = new LineSegment();
                                lineSeg.Point = new Point(lineInfo[j].Location.X, lineInfo[j].BoundingRectangle.Bottom);
                                pathFig.Segments.Add(lineSeg);
                                LineSegment lineSeg1 = new LineSegment();
                                lineSeg1.Point = new Point(lineInfo[j].Location.X, lineInfo[j].Location.Y);
                                pathFig.Segments.Add(lineSeg1);
                            }

                            flag = false;
                        }
                    }
                    else
                    {
                        var list = from e in line.ElementBoxes
                                   where (e as TableCellElementBox).IsFullCellSelected == true
                                   select e;
                        foreach (ElementBox box in list)
                        {
                            TableCellElementBox cellbox = box as TableCellElementBox;
                            if (!cellbox.IsTopHide)
                            {
                                pathGeo.Figures.Add(cellbox.SelectBox());
                                if (cellbox.BottomCellBox != null)
                                {
                                    TableCellElementBox bottom = cellbox.BottomCellBox;
                                    while (bottom != null)
                                    {
                                        pathGeo.Figures.Add(bottom.SelectBox());
                                        bottom = bottom.BottomCellBox;
                                    }
                                }
                                cellbox.BaseCell.IsSelected = true;
                            }
                        }
                    }
                }
            }
            else
            {
                return null;
            }
            path.Data = pathGeo;
            path.Stroke = SelectionStrokeColor;
            path.Fill = SelectionFillColor;
            path.Opacity = 0.5;
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateSelection()
        {
            LayoutViewer.UseUpDownSelection = false;
            LayoutViewer.UpDownSelectionWidth = double.NaN;
            if (Start != null && End != null)
            {
                if (OwnerControl.Viewer.SelectedImage != null)
                {
                    OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(Start, End);
                    OwnerControl.Viewer.IsImageResizerSelected = true;
                    OwnerControl.Viewer.ImageResizer.Visibility = Visibility.Visible;
                    OwnerControl.Viewer.ImageResizer.ImageContainer = OwnerControl.Viewer.SelectedImage;
                    OwnerControl.Viewer.ImageResizer.ImageSource = OwnerControl.Viewer.SelectedImage.ImageSource;
                    OwnerControl.Viewer.ImageResizer.ImageWidth = OwnerControl.Viewer.SelectedImage.Width;
                    OwnerControl.Viewer.ImageResizer.ImageHeight = OwnerControl.Viewer.SelectedImage.Height;
                    OwnerControl.Viewer.ImageResizer.UpdateResizerLocationAndParent();
                }
                else
                {
                    Select(Start, End);
                }
                SelectionChangedEventArgs args = new SelectionChangedEventArgs();
                args.StartPosition = Start;
                args.EndPosition = End;
                args.SelectedText = Text;
                OwnerControl.FireSelectionChanged(args);
            }
        }

        /// <summary>
        /// Select text between two position
        /// </summary>
        /// <param name="beginPos"></param>
        /// <param name="endPos"></param>
        internal void Select(TextPosition beginPos, TextPosition endPos)
        {
            if (beginPos != null && endPos != null && beginPos.Paragraph != null && endPos.Paragraph != null &&
                !beginPos.IsEqual(endPos))
            {
                if (beginPos.IsGreaterThan(endPos))
                {
                    TextPosition temp = start;
                    beginPos = endPos;
                    endPos = temp;
                }

                Start = beginPos;
                End = endPos;

                int indexInBeginInline = 0;
                int indexInEndInline = 0;

                Inline beginInline = OwnerControl.PositionHandler.GetInlineFromIndex(beginPos.Paragraph, beginPos.Index, ref indexInBeginInline);

                Inline endInline = OwnerControl.PositionHandler.GetInlineFromIndex(endPos.Paragraph, endPos.Index, ref indexInEndInline);

                int indeInBeginBox = 0;
                int indexInEndBox = 0;

                ElementBox beginBox = OwnerControl.PositionHandler.GetElemenBoxFromIndexInInline(beginInline, indexInBeginInline, ref indeInBeginBox);

                ElementBox endBox = OwnerControl.PositionHandler.GetElemenBoxFromIndexInInline(endInline, indexInEndInline, ref indexInEndBox);
                bool canselect = false;

                if (beginBox != null && endBox != null && beginBox.LineInfo !=null && endBox.LineInfo !=null)
                {
                    StartingPage = beginBox.LineInfo.PageIndex;
                    EndingPage = endBox.LineInfo.PageIndex;

                    StartPoint = beginBox.GetApproxRight(indeInBeginBox);
                    EndPoint = endBox.GetApproxRight(indexInEndBox);
                    canselect = true;
                }
                else if (end.Paragraph !=null && end.IsGreaterThan(start) && end.Paragraph.Inlines.Count == 0 && end.Paragraph.LineInfo.Count > 0
                    && beginBox != null && endBox == null && beginBox.LineInfo !=null)
                {
                    StartingPage = beginBox.LineInfo.PageIndex;
                    EndingPage = end.Paragraph.LineInfo[0].PageIndex;

                    StartPoint = beginBox.GetApproxRight(indeInBeginBox);
                    EndPoint = new Point(end.Paragraph.LineInfo[0].BoundingRectangle.Right, end.Paragraph.LineInfo[0].BoundingRectangle.Top + end.Paragraph.LineInfo[0].BoundingRectangle.Height / 2);
                    canselect = true;
                }
                else if (start.Paragraph !=null && end.IsGreaterThan(start) && start.Paragraph.Inlines.Count == 0 && start.Paragraph.LineInfo.Count > 0
                    && beginBox == null && endBox != null)
                {
                    StartingPage = start.Paragraph.LineInfo[0].PageIndex;
                    EndingPage = endBox.LineInfo.PageIndex;

                    StartPoint = new Point(start.Paragraph.LineInfo[0].BoundingRectangle.Left, start.Paragraph.LineInfo[0].BoundingRectangle.Top + start.Paragraph.LineInfo[0].BoundingRectangle.Height / 2);
                    EndPoint = endBox.GetApproxRight(indexInEndBox);
                    canselect = true;
                }
                else if (start.Paragraph !=null && end.Paragraph !=null && end.IsGreaterThan(start) && end.Paragraph.Inlines.Count == 0 && start.Paragraph.Inlines.Count == 0
                    && beginBox == null && endBox == null && start.Paragraph.LineInfo.Count > 0 && end.Paragraph.LineInfo.Count > 0)
                {
                    StartingPage = start.Paragraph.LineInfo[0].PageIndex;
                    EndingPage = end.Paragraph.LineInfo[0].PageIndex;

                    StartPoint = new Point(start.Paragraph.LineInfo[0].BoundingRectangle.Left, start.Paragraph.LineInfo[0].BoundingRectangle.Top + start.Paragraph.LineInfo[0].BoundingRectangle.Height / 2);
                    EndPoint = new Point(end.Paragraph.LineInfo[0].BoundingRectangle.Right, end.Paragraph.LineInfo[0].BoundingRectangle.Top + end.Paragraph.LineInfo[0].BoundingRectangle.Height / 2);
                    canselect = true;
                }
                if (canselect)
                {
                    ExtractSelectedText();
                    LayoutViewer.SelectPages(StartingPage, EndingPage);
                }
            }
            else if (beginPos.IsEqual(endPos))
            {
                LayoutViewer.IsSelected = false;
            }

        }

        /// <summary>
        /// Selects from starting to ending position
        /// </summary>
        internal void Select()
        {
            if (OwnerControl.Viewer.SelectedImage != null)
            {
                OwnerControl.Viewer.SelectedImage.SelectElement();
            }
            else if (Start != null && End != null)
            {
                Select(Start, End);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal void ExtractSelectedText()
        {
            bool started = false;
            if (start != null && end != null && !start.IsEqual(end))
            {
                TextPosition startPos = start;
                TextPosition endPos = end;
                if (start.IsGreaterThan(end))
                {
                    startPos = end;
                    endPos = start;
                }
                Blocks.Clear();

                stringbuilder = new StringBuilder();

                int firstInlineIndex = 0;
                int secondInlineIndex = 0;
                Inline newInline = null;

                Inline firstInline = OwnerControl.PositionHandler.GetInlineFromIndex(startPos.Paragraph, startPos.Index, ref firstInlineIndex);
                Inline secondInline = OwnerControl.PositionHandler.GetInlineFromIndex(endPos.Paragraph, endPos.Index, ref secondInlineIndex);

                if (OwnerControl.PositionHandler.Caret !=null && OwnerControl.PositionHandler.Caret.Page != null)
                {
                    ElementBox firstbox = OwnerControl.PositionHandler.Caret.Page.GetElementBox(OwnerControl.PositionHandler.Caret.GetLineInfoFromPoint(startPos.Point), startPos.Point);
                    ElementBox lastbox = OwnerControl.PositionHandler.Caret.Page.GetElementBox(OwnerControl.PositionHandler.Caret.GetLineInfoFromPoint(endPos.Point), endPos.Point);

                    TableCellAdv startcell = null;
                    TableCellAdv endcell = null;
                    BlockCollection<BlockAdv> blocks = new BlockCollection<BlockAdv>();

                    SelectionAdv.SetStartAndEndCells(start, end, ref startcell, ref endcell);

                    if (startcell == null || endcell == null)
                    {
                        if ((startcell == null && startPos.Paragraph.AssociatedCell != null) || (endcell == null && endPos.Paragraph.AssociatedCell != null))
                        {
                            if (startcell == null && endcell == null)
                            {
                                if (startPos.Paragraph.AssociatedCell == endPos.Paragraph.AssociatedCell)
                                {
                                    blocks = startPos.Paragraph.AssociatedCell.Blocks;
                                }
                                else
                                {
                                    TableAdv owner = startPos.Paragraph.AssociatedCell.OwnerTable;
                                    if (owner.AssociatedCell != null)
                                    {
                                        blocks = owner.AssociatedCell.Blocks;
                                    }
                                    else
                                    {
                                        blocks = OwnerControl.Document.Sections[0].Blocks;
                                    }
                                }
                            }
                            else if (startcell == null)
                            {
                                blocks = startPos.Paragraph.AssociatedCell.Blocks;
                            }
                            else if (endcell == null)
                            {
                                blocks = endPos.Paragraph.AssociatedCell.Blocks;
                            }
                        }
                        else
                        {
                            blocks = OwnerControl.Document.Sections[0].Blocks;
                        }
                    }
                    else if (startcell != null && endcell != null)
                    {
                        TableAdv startTable = startcell.OwnerTable;
                        TableAdv endTable = endcell.OwnerTable;

                        if (startTable.AssociatedCell == null && endTable.AssociatedCell == null)
                        {
                            blocks = OwnerControl.Document.Sections[0].Blocks;
                        }
                        else
                        {
                            blocks = startTable.AssociatedCell.Blocks;
                        }
                    }

                    if (startcell != null && endcell != null)
                    {
                        if (startcell.OwnerTable == endcell.OwnerTable)
                        {
                            List<TableCellAdv> selected = SelectedCellsInTable();
                            int i = 0;

                            bool canjump = false;

                            if (startPos.IsPositionAtTableStart && endPos.IsPositionAtTableEnd)
                            {
                                Blocks.Add(startcell.OwnerTable.CopyBlock());
                                canjump = true;
                            }
                            else
                            {
                                foreach (TableRowAdv row in startcell.OwnerTable.Rows)
                                {
                                    foreach (TableCellAdv c in row.Cells)
                                    {
                                        if (i < selected.Count && c == selected[i])
                                        {
                                            foreach (BlockAdv blk in c.Blocks)
                                            {
                                                Blocks.Add(blk.CopyBlock());
                                                canjump = true;
                                            }
                                            i++;
                                        }
                                    }
                                }
                            }
                            if (canjump)
                                goto Here;
                        }
                    }


                    if (startcell != null)
                        startcell = startcell.OwnerRow.Cells[0];

                    if (endcell != null)
                        endcell = endcell.OwnerRow.Cells[endcell.OwnerRow.Cells.Count - 1];

                    if (startcell == null && endcell == null && firstInline == secondInline && secondInlineIndex > firstInlineIndex && !firstInline.IsUIContainer)
                    {
                        newInline = firstInline.CreatInline();
                        if (!firstInline.IsImageContainer)
                        {
                            int length = secondInlineIndex - firstInlineIndex;
                            newInline.InternalText = firstInline.InternalText.Substring(firstInlineIndex, length);
                            stringbuilder.Append(newInline.InternalText);
                        }
                        ParagraphAdv para = firstInline.Paragraph.CreateNewParagraph();
                        para.Inlines.Add(newInline);
                        Blocks.Add(para);
                    }
                    else
                    {
                        foreach (BlockAdv b in blocks)
                        {
                            int value = b.Search(startPos, endPos, startcell, endcell, ref Blocks, ref started, ref stringbuilder);
                            if (value == 1)
                            {
                                started = true;
                            }
                            else if (value == 0)
                            {
                                started = false;
                                break;
                            }
                            else
                            {
                                if (started)
                                {
                                    Blocks.Add(b.CopyBlock());
                                }
                            }
                        }
                    }
                Here:
                    Text = Convert.ToString(stringbuilder);
                }
            }
        }

        internal void SetVirtualPositionsForStartAndEnd(TextPosition Start,TextPosition End)
        {
            TextPosition startPos = null;
            TextPosition endPos = null;

            TextPosition sPos = Start;
            TextPosition ePos = End;

            if (sPos != null && ePos != null)
            {
                startPos = sPos;
                endPos = ePos;

                if (sPos.IsGreaterThan(ePos))
                {
                    startPos = ePos;
                    endPos = sPos;
                }

                string tempstart = string.Empty;
                string tempend = string.Empty;
                TableRowAdv crow = null;
                TableCellAdv cell = null;

                int cCount = 0;

                List<string> list1 = startPos.Index.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();

                List<string> list2 = endPos.Index.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();
                List<string> final = new List<string>();

                int mincount = Math.Min(list1.Count, list2.Count);
                if (list1 != list2)
                {
                    for (int i = 0; i < mincount; i++)
                    {
                        if (list1[i] == list2[i])
                        {
                            final.Add(list1[i]);
                            continue;
                        }
                        else
                            break;
                    }
                }
                if (final.Count > 2 && final != list1 && final != list2)
                {
                    foreach (string s in final)
                    {
                        if (s.Contains("r"))
                        {
                            if (final.Last() != s)
                            {
                                string next = final[final.IndexOf(s) + 1];
                                if (next.Contains("c"))
                                {
                                    cCount++;
                                }
                            }
                        }
                    }

                    if (cCount > 0)
                    {
                        GetExtractedString(startPos.Index, cCount, ref tempstart);
                        GetExtractedString(endPos.Index, cCount, ref tempend);

                        BlockAdv endblk = OwnerControl.Document.GetBlockFromVirtualPosition(tempend, ref crow, ref cell);
                        BlockAdv startblk = OwnerControl.Document.GetBlockFromVirtualPosition(tempstart, ref crow, ref cell);

                        if (startblk != null && endblk != null)
                        {
                            if (startblk.IsTable && endblk.IsTable && startblk == endblk)
                            {
                                goto Here;
                            }
                            if (tempstart != startPos.Index)
                            {
                                tempstart = tempstart.Substring(0, tempstart.LastIndexOf('c'));
                                tempstart += "c0";
                            }
                            if (endblk.IsTable)
                            {
                                int rCount = 0;
                                if (tempend.Contains('r'))
                                {
                                    List<string> positions = tempend.Split(new string[] { "=>" }, StringSplitOptions.None).ToList<string>();
                                    string rValue = string.Empty;
                                    foreach (string str in positions)
                                    {
                                        if (str.Contains('r'))
                                        {
                                            rValue = str.Substring(1);
                                        }
                                    }
                                    rCount = int.Parse(rValue);
                                }
                                TableRowAdv row = (endblk as TableAdv).Rows[rCount];
                                int celcount = row.Cells.Count - 1;
                                tempend = tempend.Substring(0, tempend.LastIndexOf('c'));
                                tempend += "c" + celcount;
                            }
                        }

                    }
                }
                else
                {
                    int addindex = 0;

                    bool started = false;
                    string finalstr = string.Empty;

                    if (startPos.Index.Contains('r') && startPos.Index.Contains('c'))
                    {
                        foreach (char cha in startPos.Index)
                        {
                            if (cha == 'c')
                            {
                                addindex = startPos.Index.IndexOf(cha);
                                break;
                            }
                        }
                        for (int k = 0; k < startPos.Index.Length; k++)
                        {
                            char value = startPos.Index[k];
                            if (started)
                            {
                                if (value == '=')
                                {
                                    started = false;
                                    break;
                                }
                                finalstr += value.ToString();
                            }
                            if (k == addindex)
                            {
                                started = true;
                            }
                        }

                        tempstart = startPos.Index.Substring(0, addindex + 1) + finalstr;
                    }
                    else
                    {
                        tempstart += startPos.Index;
                    }
                    started = false;
                    finalstr = string.Empty;
                    if (endPos.Index.Contains('r') && endPos.Index.Contains('c'))
                    {
                        foreach (char cha in endPos.Index)
                        {
                            if (cha == 'c')
                            {
                                addindex = endPos.Index.IndexOf(cha);
                                break;
                            }
                        }
                        for (int k = 0; k < endPos.Index.Length; k++)
                        {
                            char value = endPos.Index[k];
                            if (started)
                            {
                                if (value == '=')
                                {
                                    started = false;
                                    break;
                                }
                                finalstr += value.ToString();
                            }
                            if (k == addindex)
                            {
                                started = true;
                            }
                        }

                        tempend = endPos.Index.Substring(0, addindex + 1) + finalstr;
                    }
                    else
                    {
                        tempend += endPos.Index;
                    }
                    BlockAdv endblk = OwnerControl.Document.GetBlockFromVirtualPosition(tempend, ref crow, ref cell);
                    BlockAdv startblk = OwnerControl.Document.GetBlockFromVirtualPosition(tempstart, ref crow, ref cell);

                    if (startblk != null && endblk != null)
                    {
                        if (startblk.IsTable && endblk.IsTable && startblk == endblk)
                        {
                            goto Here;
                        }
                        if (tempstart != startPos.Index && tempstart.Contains('c'))
                        {
                            tempstart = tempstart.Substring(0, tempstart.LastIndexOf('c'));
                            tempstart += "c0";
                        }
                        if (endblk.IsTable)
                        {
                            int rCount = 0;
                            if (tempend.Contains('r'))
                            {
                                List<string> positions = tempend.Split(new string[] { "=>" }, StringSplitOptions.None).ToList<string>();
                                string rValue = string.Empty;
                                foreach (string str in positions)
                                {
                                    if (str.Contains('r'))
                                    {
                                        rValue = str.Substring(1);
                                    }
                                }
                                rCount = int.Parse(rValue);
                            }
                            TableRowAdv row = (endblk as TableAdv).Rows[rCount];
                            int celcount = row.Cells.Count - 1;
                            tempend = tempend.Substring(0, tempend.LastIndexOf('c'));
                            tempend += "c" + celcount;
                        }
                    }
                }
            Here: startPos.VirtualPosition = tempstart;
                endPos.VirtualPosition = tempend;
            }
        }

        internal void GetExtractedString(string stringvalue, int cCount, ref string tempstring)
        {
            for (int i = 0; i < stringvalue.Length; i++)
            {
                char ch = stringvalue[i];
                if (ch == 'c' && cCount != 0)
                {
                    cCount--;
                }
                if (cCount == 0)
                {
                    if (ch == '>')
                    {
                        int addindex = 0; string final = string.Empty;
                        bool started = false;
                        string remaining = stringvalue.Substring(i + 1);
                        if (remaining.Contains('r') && remaining.Contains('c'))
                        {
                            foreach (char cha in remaining)
                            {
                                if (cha == 'c')
                                {
                                    addindex = remaining.IndexOf(cha);
                                    break;
                                }
                            }
                            for (int k = 0; k < remaining.Length; k++)
                            {
                                char value = remaining[k];
                                if (started)
                                {
                                    if (value == '=')
                                    {
                                        started = false;
                                        break;
                                    }
                                    final += value.ToString();
                                }
                                if (k == addindex)
                                {
                                    started = true;
                                }
                            }
                            tempstring = stringvalue.Substring(0, i + addindex + 2) + final;
                            break;
                        }
                        else
                        {
                            tempstring = stringvalue;
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Sets the text in clipboard
        /// </summary>
        public void Copy()
        {
            ClipboardAdv.SetText(Text);
            if (Blocks.Count > 0)
            {
                BlockCollection<BlockAdv> tempBlocks = new BlockCollection<BlockAdv>();
                foreach (BlockAdv block in Blocks)
                {
                    tempBlocks.Add(block.CopyBlock());
                }
                ClipboardAdv.SetBlock(tempBlocks);
            }
        }
        
        internal static void SetStartAndEndCells(TextPosition start, TextPosition end, ref TableCellAdv startcell, ref TableCellAdv endcell)
        {
            if (start != null && end != null)
            {
                ParagraphAdv startParagraph = start.Paragraph;
                ParagraphAdv endParagraph = end.Paragraph;

                if (!startParagraph.IsInsideTable || !end.IsInsideTable)
                {
                    if (startParagraph.IsInsideTable)
                    {
                        startcell = startParagraph.AssociatedCell;
                    }
                    if (endParagraph.IsInsideTable)
                    {
                        endcell = endParagraph.AssociatedCell;
                    }
                }
                else
                {
                    string startindex = start.Index;
                    string endindex = end.Index;
                    if (startindex != endindex)
                    {
                        int countToMinus = CheckIsSameCell(startindex, endindex);

                        List<char> start_r = startindex.Where(s => s == 'r').ToList();
                        int rCount = start_r.Count;

                        BlockAdv tempblock = startParagraph;
                        for (int i = 0; i < rCount - countToMinus; i++)
                        {
                            startcell = tempblock.AssociatedCell;
                            tempblock = tempblock.AssociatedCell.OwnerTable;
                        }

                        tempblock = endParagraph;

                        List<char> end_r = endindex.Where(s => s == 'r').ToList();

                        for (int i = 0; i < end_r.Count - countToMinus; i++)
                        {
                            endcell = tempblock.AssociatedCell;
                            if (tempblock.AssociatedCell != null)
                            {
                                tempblock = tempblock.AssociatedCell.OwnerTable;
                            }
                        }
                    }

                }
            }
        }

        internal static int CheckIsSameCell(string startindex, string endindex)
        {
            int countToMinus = 0;

            List<string> list1 = startindex.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();

            List<string> list2 = endindex.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();
            List<string> final = new List<string>();

            int mincount = Math.Min(list1.Count, list2.Count);

            for (int i = 0; i < mincount; i++)
            {
                if (list1[i] == list2[i])
                {
                    final.Add(list1[i]);
                    continue;
                }
                else
                    break;
            }
            foreach (string s in final)
            {
                if (s.Contains("r"))
                {
                    if (final.Last() != s)
                    {
                        string next = final[final.IndexOf(s) + 1];
                        if (next.Contains("c"))
                        {
                            countToMinus++;
                        }
                    }
                }
            }
            return countToMinus;

        }

        /// <summary>
        /// Toggles bold for the selection
        /// </summary>
        public void Bold()
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                inlinesToFormat = GetInlinesToFormat();

                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                if (inlinesToFormat.Count > 0)
                {
                    FontWeight fontWeight = FontWeights.Normal;
                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                fontWeight = (inlinesToFormat[i] as SpanAdv).FontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold;
                            (inlinesToFormat[i] as SpanAdv).FontWeight = fontWeight;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                fontWeight = (inlinesToFormat[i] as HyperlinkAdv).FontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold;
                            (inlinesToFormat[i] as HyperlinkAdv).FontWeight = fontWeight;
                        }
                    }

                    SpanAdv span = new SpanAdv();
                    span.FontWeight = fontWeight;

                    GetHistoryInfo(history);
                    //GetHistoryInfoFromExtractedBlocks(history);
                    history.Action = Actions.FontWeight;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.FontWeight = OwnerControl.CurrentInlineStyle.FontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="history"></param>
        internal void GetHistoryInfo(HistoryInfo history)
        {
            if (Blocks.Count > 0 && Start != null && End != null)
            {
                history.StartPosition = Start.CopyForHistory();
                history.EndPosition = End.CopyForHistory();
                history.Blocks = CopyBlocks(Blocks);
                history.UndoType = UndoType.SelectionBased;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ArrangeLayout()
        {
            LayoutViewer.SetIsArrangedToFalse();

            foreach (BlockAdv paragraph in AffectedBlocks)
            {
                if (!paragraph.IsArranged)
                {
                    paragraph.LinkElementBoxes();
                    paragraph.ArrangeElements();
                    //paragraph.ArrangeElements(paragraph.LayoutViewer.AvailableSize);
                }

                paragraph.IsArranged = false;
            }

            OwnerControl.PositionHandler.PositionCursor();

            LayoutViewer.SetIsArrangedToFalse();

            LayoutViewer.SetVisibleLinesToPage();

            AffectedBlocks.Clear();

            StartingInline = inlinesToFormat.First();
            EndingInline = inlinesToFormat.Last();

            UpdateSelection();
        }

        internal List<BlockAdv> GetSelectedBlocks()
        {
            BlockCollection<BlockAdv> blocks = new BlockCollection<BlockAdv>();
            BlockCollection<BlockAdv> tempblocks = new BlockCollection<BlockAdv>();
            bool started = false;
            if (Start != null && End != null)
            {
                TableRowAdv startrow=null;
                TableRowAdv endrow=null;
                TableCellAdv startcell = null;
                TableCellAdv endcell = null;


                BlockAdv startblk = Document.GetBlockFromVirtualPosition(Start.VirtualPosition, ref startrow, ref startcell);
                BlockAdv endblk = Document.GetBlockFromVirtualPosition(End.VirtualPosition, ref endrow, ref endcell);

                if (startblk !=null && endblk !=null)
                {
                    TableCellAdv associatecell=startblk.AssociatedCell;

                    if (associatecell != null)
                    {
                        blocks = associatecell.Blocks;
                    }
                    else
                    {
                        blocks = Document.Sections[0].Blocks;
                    }
                }

                foreach (BlockAdv b in blocks)
                {
                    int value = b.SearchToSelect(Start, End, ref started, ref tempblocks);
                    if (value == 0)
                    {
                        started = false;
                        break;
                    }
                    else if (value == 1)
                    {
                        started = true;
                    }
                }

            }

            return tempblocks.ToList<BlockAdv>();
        }

        public bool CanMerge()
        {
            TableAdv table = null;
            int minrowindex = 0x7f;
            int maxrowspanwithindex = 0;
            int mincolumnindex = 0x7f;
            int maxcolumnspanwithindex = 0;

            List<TableCellAdv> cellsToMerge = SelectedCellsInTable();

            if (cellsToMerge.Count <= 1)
            {
                return false;
            }
            foreach (TableCellAdv cell in cellsToMerge)
            {
                if (cell == null)
                {
                    return false;
                }
                if (table == null)
                {
                    table = cell.OwnerTable;
                }
                else if (cell.OwnerTable != table)
                {
                    return false;
                }
                minrowindex = Math.Min(minrowindex, cell.RowIndex);
                maxrowspanwithindex = Math.Max(maxrowspanwithindex, (cell.RowIndex + cell.RowSpan) - 1);
                mincolumnindex = Math.Min(mincolumnindex, cell.ColumnIndex);
                maxcolumnspanwithindex = Math.Max(maxcolumnspanwithindex, (cell.ColumnIndex + cell.ColumnSpan) - 1);
            }

            bool[,] cellmatriz = new bool[(maxrowspanwithindex - minrowindex) + 1, (maxcolumnspanwithindex - mincolumnindex) + 1];

            foreach (TableCellAdv cell2 in cellsToMerge)
            {
                for (int j = cell2.RowIndex; j < (cell2.RowIndex + cell2.RowSpan); j++)
                {
                    for (int k = cell2.ColumnIndex; k < (cell2.ColumnIndex + cell2.ColumnSpan); k++)
                    {
                        cellmatriz[j - minrowindex, k - mincolumnindex] = true;
                    }
                }
            }
            bool[,] cellmatriz2 = cellmatriz;
            int upperBound = cellmatriz2.GetUpperBound(0);
            int no = cellmatriz2.GetUpperBound(1);
            for (int i = cellmatriz2.GetLowerBound(0); i <= upperBound; i++)
            {
                for (int f = cellmatriz2.GetLowerBound(1); f <= no; f++)
                {
                    if (!cellmatriz2[i, f])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal List<TableCellAdv> SelectedCellsInTable()
        {
            TableRowAdv startrow = null;
            TableRowAdv endrow = null;
            TableCellAdv startcell = null;
            TableCellAdv endcell = null;
            bool started = false;
            bool canexe = false;

            List<TableCellAdv> selectedcells = new List<TableCellAdv>();

            if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
            {
                BlockAdv startblk = Document.GetBlockFromVirtualPosition(OwnerControl.Selection.Start.VirtualPosition, ref startrow, ref startcell);

                BlockAdv endblk = Document.GetBlockFromVirtualPosition(OwnerControl.Selection.End.VirtualPosition, ref endrow, ref endcell);

                if (startblk !=null && startblk.IsInsideTable && endblk !=null && endblk.IsInsideTable)
                {
                    if (startblk.AssociatedCell == endblk.AssociatedCell && IsCellSelected)
                    {
                        selectedcells.Add(startblk.AssociatedCell);
                    }
                    else
                        canexe = true;
                }
                else
                    canexe = true;

                if (canexe)
                {
                    if (startblk != null && endblk != null && startcell != null && endcell != null)
                    {
                        if (startblk.IsTable && endblk.IsTable && startblk == endblk)
                        {
                            int endindex = endcell.ColumnIndex;
                            int startindex = startcell.ColumnIndex;

                            TableAdv table = startblk as TableAdv;
                            foreach (TableRowAdv row in table.Rows)
                            {
                                foreach (TableCellAdv cell in row.Cells)
                                {
                                    if (cell == startcell)
                                    {
                                        selectedcells.Add(cell);
                                        started = true;
                                    }
                                    else if (cell == endcell)
                                    {
                                        selectedcells.Add(cell);
                                        started = false;
                                        break;
                                    }
                                    else if (started && cell.ColumnIndex <= endindex && cell.ColumnIndex >= startindex)
                                    {
                                        selectedcells.Add(cell);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return selectedcells;
                
        }

        internal bool CheckCanSelectCell(ref TableCellAdv tablecell)
        {
            if (Start != null && End != null)
            {
                TableRowAdv startrow = null;
                TableRowAdv endrow = null;
                TableCellAdv startcell = null;
                TableCellAdv endcell = null;

                BlockAdv startblk = Document.GetBlockFromVirtualPosition(Start.VirtualPosition, ref startrow, ref startcell);

                BlockAdv endblk = Document.GetBlockFromVirtualPosition(End.VirtualPosition, ref endrow, ref endcell);

                if (startblk != null && startblk.IsInsideTable && endblk != null && endblk.IsInsideTable)
                {
                    if (startblk.AssociatedCell == endblk.AssociatedCell)
                    {
                        tablecell = startblk.AssociatedCell;
                        return true;                        
                    }
                }
            }
            return false;
        }

        public void ChangeListType(ListType listType)
        {
            if (IsReadOnly)
                return;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.ListType;
            bool canRecord = false;
            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeListTypeForSelection(listType);
                OwnerControl.PositionHandler.PositionCursor();
                UpdateSelection();
                canRecord = true;
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            if (OwnerControl.PositionHandler != null && OwnerControl.PositionHandler.TextPosition != null
                && OwnerControl.PositionHandler.TextPosition.Paragraph != null && OwnerControl.PositionHandler.TextPosition.Paragraph.ListType != listType)
            {
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                OwnerControl.PositionHandler.TextPosition.Paragraph.ListType = listType;
                OwnerControl.PositionHandler.TextPosition.Paragraph.ArrangeElements();
                LayoutViewer.SetIsArrangedToFalse();
                LayoutViewer.SetVisibleLinesToPage();
                LayoutViewer.SetIsArrangedToFalse();
                OwnerControl.PositionHandler.PositionCursor();

                history.StartPosition = OwnerControl.PositionHandler.TextPosition.CopyForHistory();
                history.Blocks.Add(OwnerControl.PositionHandler.TextPosition.Paragraph.CreateNewParagraph());
                canRecord = true;
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <param name="listType"></param>
        internal void ChangeListTypeForSelection(ListType listType)
        {
            if (IsReadOnly)
                return;
            List<BlockAdv> blocks = GetSelectedBlocks();
            ParagraphAdv paragraph = null;

            foreach (BlockAdv b in blocks)
            {
                paragraph = b as ParagraphAdv;
                if (paragraph != null)
                {
                    paragraph.ListType = listType;
                    paragraph.CreateNewLines = true;
                }
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (BlockAdv b in blocks)
            {
                if (!b.IsArranged)
                {
                    b.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        internal void ChangeLineSpacingForSelection(double lineSpacing)
        {
            List<BlockAdv> blocks = GetSelectedBlocks();
            ParagraphAdv paragraph = null;

            foreach (BlockAdv para in blocks)
            {
                paragraph = para as ParagraphAdv;
                if (paragraph != null)
                {
                    paragraph.LineSpacing = lineSpacing;
                }
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (BlockAdv para in blocks)
            {
                para.CreateNewLines = true;
            }

            foreach (BlockAdv para in blocks)
            {
                if (!para.IsArranged)
                {
                    para.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        internal void ChangeAfterSpacingForSelection(double afterSpacing)
        {
            List<BlockAdv> blocks = GetSelectedBlocks();
            ParagraphAdv paragraph = null;

            foreach (BlockAdv para in blocks)
            {
                paragraph = para as ParagraphAdv;
                if (paragraph != null)
                {
                    paragraph.AfterSpacing = afterSpacing;
                    paragraph.CreateNewLines = true;
                }
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (BlockAdv para in blocks)
            {
                if (!para.IsArranged)
                {
                    para.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        /// <summary>
        /// Changes font size
        /// </summary>
        public void ChangeFontSize(double fontSize)
        {
            if (IsReadOnly)
                return;
            if (fontSize > 0)
            {
                if (LayoutViewer.IsSelected)
                {
                    HistoryInfo history = new HistoryInfo();
                    List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                    List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                    if (selectedcells.Count > 0)
                    {
                        foreach (TableCellAdv cell in selectedcells)
                        {
                            PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                            cleanedcell.RowIndex = cell.RowIndex;
                            cleanedcell.ColumnIndex = cell.ColumnIndex;
                            foreach (BlockAdv b in cell.Blocks)
                            {
                                cleanedcell.Blocks.Add(b.CopyBlock());
                            }
                            copiedcells.Add(cleanedcell);
                        }
                        history.PreservedCells = copiedcells;
                    }

                    inlinesToFormat = GetInlinesToFormat();
                    if (inlinesToFormat.Count > 0)
                    {
                        foreach (Inline inline in inlinesToFormat)
                        {
                            if (inline is SpanAdv)
                                (inline as SpanAdv).FontSize = fontSize;
                            else if (inline is HyperlinkAdv)
                                (inline as HyperlinkAdv).FontSize = fontSize;
                        }

                        SpanAdv span = new SpanAdv();
                        span.FontSize = fontSize;
                        GetHistoryInfo(history);
                        history.Action = Actions.FontSize;
                        history.InlineStyle = span;
                        OwnerControl.History.RecordUndo(history);

                        ArrangeLayout();
                    }
                }
                else
                {
                    OwnerControl.CurrentInlineStyle.FontSize = fontSize;
                    OwnerControl.PositionHandler.IsStyleChanged = true;
                }

                OwnerControl.History.CheckForClearingRedo();
            }
        }



        public void ChangeFontFamily(FontFamily fontFamily)
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    foreach (Inline inline in inlinesToFormat)
                    {
                        if (inline is SpanAdv)
                            (inline as SpanAdv).FontFamily = fontFamily;
                        else if (inline is HyperlinkAdv)
                            (inline as HyperlinkAdv).FontFamily = fontFamily;
                    }

                    SpanAdv span = new SpanAdv();
                    span.FontFamily = fontFamily;

                    GetHistoryInfo(history);
                    history.Action = Actions.FontFamily;
                    //history.NeedSelection = true;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.FontFamily = fontFamily;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Changes the foreground
        /// </summary>
        /// <param name="brush"></param>
        public void ChangeForeground(Color brush)
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    foreach (Inline inline in inlinesToFormat)
                    {
                        if (inline is SpanAdv)
                            (inline as SpanAdv).Foreground = brush;
                        else if (inline is HyperlinkAdv)
                            (inline as HyperlinkAdv).Foreground = brush;
                    }

                    SpanAdv span = new SpanAdv();
                    span.Foreground = brush;

                    GetHistoryInfo(history);
                    history.Action = Actions.Foreground;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.Foreground = brush;
                OwnerControl.PositionHandler.IsStyleChanged = true;
                OwnerControl.Focus();
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Changes the foreground
        /// </summary>
        /// <param name="brush"></param>
        public void ChangeHighlightColor(Color brush)
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    foreach (Inline inline in inlinesToFormat)
                    {
                        if (inline is SpanAdv)
                            (inline as SpanAdv).HighlightColor = brush;
                        else if (inline is HyperlinkAdv)
                            (inline as HyperlinkAdv).HighlightColor = brush;
                    }

                    SpanAdv span = new SpanAdv();
                    span.HighlightColor = brush;

                    GetHistoryInfo(history);
                    history.Action = Actions.HighlightColor;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.HighlightColor = brush;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Toggles the single strike through
        /// </summary>
        public void ChangeSingleStrikeThrough()
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    StrikeThrough strikeThrough = StrikeThrough.None;

                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                strikeThrough = (inlinesToFormat[i] as SpanAdv).StrikeThrough == StrikeThrough.SingleStrike ? StrikeThrough.None : StrikeThrough.SingleStrike;
                            (inlinesToFormat[i] as SpanAdv).StrikeThrough = strikeThrough;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                strikeThrough = (inlinesToFormat[i] as HyperlinkAdv).StrikeThrough == StrikeThrough.SingleStrike ? StrikeThrough.None : StrikeThrough.SingleStrike;
                            (inlinesToFormat[i] as HyperlinkAdv).StrikeThrough = strikeThrough;
                        }
                        inlinesToFormat[i].MeasureElements();
                    }

                    SpanAdv span = new SpanAdv();
                    span.StrikeThrough = strikeThrough;

                    GetHistoryInfo(history);
                    history.Action = Actions.SingleStrike;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.StrikeThrough = OwnerControl.CurrentInlineStyle.StrikeThrough == StrikeThrough.SingleStrike ? StrikeThrough.None : StrikeThrough.SingleStrike;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Toggles the double strike through
        /// </summary>
        public void ChangeDoubleStrikeThrough()
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    StrikeThrough strikeThrough = StrikeThrough.None;

                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                strikeThrough = (inlinesToFormat[i] as SpanAdv).StrikeThrough == StrikeThrough.DoubleStrike ? StrikeThrough.None : StrikeThrough.DoubleStrike;
                            (inlinesToFormat[i] as SpanAdv).StrikeThrough = strikeThrough;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                strikeThrough = (inlinesToFormat[i] as HyperlinkAdv).StrikeThrough == StrikeThrough.DoubleStrike ? StrikeThrough.None : StrikeThrough.DoubleStrike;
                            (inlinesToFormat[i] as HyperlinkAdv).StrikeThrough = strikeThrough;
                        }
                        inlinesToFormat[i].MeasureElements();
                    }

                    SpanAdv span = new SpanAdv();
                    span.StrikeThrough = strikeThrough;

                    GetHistoryInfo(history);
                    history.Action = Actions.DoubleStrike;
                    // history.NeedSelection = true;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.StrikeThrough = OwnerControl.CurrentInlineStyle.StrikeThrough == StrikeThrough.DoubleStrike ? StrikeThrough.None : StrikeThrough.DoubleStrike;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Toggles the superscript
        /// </summary>
        public void ChangeSuperscript()
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    Baseline superScript = Baseline.Normal;
                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                superScript = (inlinesToFormat[i] as SpanAdv).Baseline == Baseline.Superscript ? Baseline.Normal : Baseline.Superscript;
                            (inlinesToFormat[i] as SpanAdv).Baseline = superScript;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                superScript = (inlinesToFormat[i] as HyperlinkAdv).Baseline == Baseline.Superscript ? Baseline.Normal : Baseline.Superscript;
                            (inlinesToFormat[i] as HyperlinkAdv).Baseline = superScript;
                        }
                        inlinesToFormat[i].MeasureElements();
                    }
                    SpanAdv span = new SpanAdv();
                    span.Baseline = superScript;

                    GetHistoryInfo(history);
                    history.Action = Actions.SuperScript;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.Baseline = OwnerControl.CurrentInlineStyle.Baseline == Baseline.Superscript ? Baseline.Normal : Baseline.Superscript;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Toggles the subscript
        /// </summary>
        public void ChangeSubscript()
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    Baseline subScript = Baseline.Normal;
                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                subScript = (inlinesToFormat[i] as SpanAdv).Baseline == Baseline.Subscript ? Baseline.Normal : Baseline.Subscript;
                            (inlinesToFormat[i] as SpanAdv).Baseline = subScript;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                subScript = (inlinesToFormat[i] as HyperlinkAdv).Baseline == Baseline.Subscript ? Baseline.Normal : Baseline.Subscript;
                            (inlinesToFormat[i] as HyperlinkAdv).Baseline = subScript;
                        }
                        inlinesToFormat[i].MeasureElements();
                    }

                    SpanAdv span = new SpanAdv();
                    span.Baseline = subScript;

                    GetHistoryInfo(history);
                    history.Action = Actions.SubScript;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.Baseline = OwnerControl.CurrentInlineStyle.Baseline == Baseline.Subscript ? Baseline.Normal : Baseline.Subscript;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Changes the text alignment.
        /// </summary>
        /// <param name="textAlignment">The text alignment.</param>
        public void ChangeTextAlignment(TextAlignment textAlignment)
        {
            if (IsReadOnly)
                return;
            bool canRecord = false;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.TextAlignment;

            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeTextAlignmentForSelection(textAlignment);
                OwnerControl.PositionHandler.PositionCursor();
                canRecord = true;
                UpdateSelection();
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            else if (LayoutViewer.TextPosition != null && LayoutViewer.TextPosition.Paragraph != null)
            {
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                LayoutViewer.TextPosition.Paragraph.TextAlignment = textAlignment;

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.TextPosition.Paragraph.ArrangeElements();

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetVisibleLinesToPage();

                OwnerControl.PositionHandler.PositionCursor();

                history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
                history.Blocks.Add(LayoutViewer.TextPosition.Paragraph.CreateNewParagraph());
                canRecord = true;
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        internal void ChangeTextAlignmentForSelection(TextAlignment textAlignment)
        {
            List<BlockAdv> blocks = GetSelectedBlocks();

            foreach (BlockAdv blk in blocks)
            {
                if(blk.IsParagraph)
                    (blk as ParagraphAdv).TextAlignment = textAlignment;
                blk.CreateNewLines = true;
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (BlockAdv block in blocks)
            {
                if (!block.IsArranged)
                {
                    block.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        /// <summary>
        /// before Spacing to current Paragraph.
        /// </summary>
        /// <param name="spacingValue">The spacing value.</param>
        public void ChangeBeforeSpacing(double spacingValue)
        {
            if (IsReadOnly)
                return;

            bool canRecord = false;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.BeforeSpacing;
            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeBeforeSpacingForSelection(spacingValue);
                OwnerControl.PositionHandler.PositionCursor();
                UpdateSelection();
                canRecord = true;
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            else if (LayoutViewer.TextPosition != null && LayoutViewer.TextPosition.Paragraph != null)
            {
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                LayoutViewer.TextPosition.Paragraph.BeforeSpacing = spacingValue;

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.TextPosition.Paragraph.ArrangeElements();

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetVisibleLinesToPage();

                OwnerControl.PositionHandler.PositionCursor();
                history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
                history.Blocks.Add(LayoutViewer.TextPosition.Paragraph.CreateNewParagraph());
                canRecord = true;
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        internal void ChangeBeforeSpacingForSelection(double beforeSpacing)
        {
            List<BlockAdv> blocks = GetSelectedBlocks();

            foreach (ParagraphAdv para in blocks)
            {
                para.BeforeSpacing = beforeSpacing;
                para.CreateNewLines = true;
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (ParagraphAdv para in blocks)
            {
                if (!para.IsArranged)
                {
                    para.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        /// <summary>
        /// After Spacing to current Paragraph.
        /// </summary>
        /// <param name="spacingValue">The spacing value.</param>
        public void ChangeAfterSpacing(double spacingValue)
        {
            if (IsReadOnly)
                return;
            bool canRecord = false;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.AfterSpacing;

            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeAfterSpacingForSelection(spacingValue);
                OwnerControl.PositionHandler.PositionCursor();
                UpdateSelection();
                canRecord = true;
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            else if (LayoutViewer.TextPosition != null && LayoutViewer.TextPosition.Paragraph != null)
            {
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                LayoutViewer.TextPosition.Paragraph.AfterSpacing = spacingValue;

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.TextPosition.Paragraph.ArrangeElements();

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetVisibleLinesToPage();

                OwnerControl.PositionHandler.PositionCursor();
                history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
                history.Blocks.Add(LayoutViewer.TextPosition.Paragraph.CreateNewParagraph());
                canRecord = true;
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Changes the left indent.
        /// </summary>
        /// <param name="indentValue">The indent value.</param>
        public void ChangeLeftIndent(double indentValue)
        {
            if (IsReadOnly)
                return;
            bool canRecord = false;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.LeftIndent;
            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeLeftIndentForSelection(indentValue);
                OwnerControl.PositionHandler.PositionCursor();
                canRecord = true;
                UpdateSelection();
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            else if (LayoutViewer.TextPosition != null && LayoutViewer.TextPosition.Paragraph != null)
            {
                if (LayoutViewer.TextPosition.Paragraph.CheckForRightIndent(indentValue, LayoutViewer.TextPosition.Paragraph.RightIndent))
                {
                    history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                    LayoutViewer.TextPosition.Paragraph.LeftIndent = indentValue;

                    LayoutViewer.SetIsArrangedToFalse();

                    LayoutViewer.TextPosition.Paragraph.ArrangeElements();

                    LayoutViewer.SetIsArrangedToFalse();

                    LayoutViewer.SetVisibleLinesToPage();

                    OwnerControl.PositionHandler.PositionCursor();

                    history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
                    history.Blocks.Add(LayoutViewer.TextPosition.Paragraph.CreateNewParagraph());
                    canRecord = true;
                }
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Changes the right indent.
        /// </summary>
        /// <param name="indentValue">The indent value.</param>
        public void ChangeRightIndent(double indentValue)
        {
            if (IsReadOnly)
                return;
            bool canRecord = false;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.LeftIndent;
            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeRightIndentForSelection(indentValue);
                OwnerControl.PositionHandler.PositionCursor();
                canRecord = true;
                UpdateSelection();
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            else if (LayoutViewer.TextPosition != null && LayoutViewer.TextPosition.Paragraph != null)
            {
                if (LayoutViewer.TextPosition.Paragraph.CheckForRightIndent(indentValue, LayoutViewer.TextPosition.Paragraph.LeftIndent))
                {
                    history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                    LayoutViewer.TextPosition.Paragraph.RightIndent = indentValue;

                    LayoutViewer.SetIsArrangedToFalse();

                    LayoutViewer.TextPosition.Paragraph.ArrangeElements();

                    LayoutViewer.SetIsArrangedToFalse();

                    LayoutViewer.SetVisibleLinesToPage();

                    OwnerControl.PositionHandler.PositionCursor();

                    history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
                    history.Blocks.Add(LayoutViewer.TextPosition.Paragraph.CreateNewParagraph());
                    canRecord = true;
                }
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        internal void ChangeRightIndentForSelection(double rightIndent)
        {
            List<BlockAdv> blocks = GetSelectedBlocks();

            foreach (ParagraphAdv para in blocks)
            {
                if (para.CheckForRightIndent(rightIndent, para.LeftIndent))
                    para.RightIndent = rightIndent;

                para.CreateNewLines = true;
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (ParagraphAdv para in blocks)
            {
                if (!para.IsArranged)
                {
                    para.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        internal void ChangeLeftIndentForSelection(double leftIndent)
        {
            List<BlockAdv> blocks = GetSelectedBlocks();

            //foreach (ParagraphAdv para in blocks)
            //{
            //    if (para.CheckForLeftIndent(leftIndent, para.RightIndent))
            //        para.LeftIndent = leftIndent;

            //    para.CreateNewLines = true;
            //}
            foreach (BlockAdv b in blocks)
            {
                if (b.CheckForRightIndent(leftIndent, b.RightIndent))
                {
                    b.LeftIndent = leftIndent;
                }
                b.CreateNewLines = true;
            }

            LayoutViewer.SetIsArrangedToFalse();

            foreach (BlockAdv b in blocks)
            {
                if (!b.IsArranged)
                {
                    b.ArrangeElements();
                }
            }

            LayoutViewer.SetIsArrangedToFalse();
            LayoutViewer.SetVisibleLinesToPage();
        }

        /// <summary>
        /// Changes the right indent.
        /// </summary>
        /// <param name="indentValue">The indent value.</param>
        public void DecreaseIndent(double indentValue)
        {
            if (LayoutViewer.CurrentParagraph != null)
            {
                LayoutViewer.CurrentParagraph.RightIndent = indentValue;

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.CurrentParagraph.ArrangeElements();

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetVisibleLinesToPage();

                UpdateSelection();
            }
        }

        /// <summary>
        /// Changes the line spacing.
        /// </summary>
        /// <param name="indentValue">The indent value.</param>
        public void ChangeLineSpacing(double lineSpaceValue)
        {
            if (IsReadOnly)
                return;
            bool canRecord = false;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.LineSpacing;
            if (LayoutViewer.IsSelected)
            {
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                GetHistoryInfo(history);
                ChangeLineSpacingForSelection(lineSpaceValue);
                OwnerControl.PositionHandler.PositionCursor();
                UpdateSelection();
                canRecord = true;
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
            }
            else if (LayoutViewer.TextPosition != null && LayoutViewer.TextPosition.Paragraph != null)
            {
                history.ParagraphStyle = LayoutViewer.TextPosition.Paragraph.CreateNewParagraph();
                LayoutViewer.TextPosition.Paragraph.LineSpacing = lineSpaceValue;

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.TextPosition.Paragraph.ArrangeElements();

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetVisibleLinesToPage();

                OwnerControl.PositionHandler.PositionCursor();
                history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
                history.Blocks.Add(LayoutViewer.TextPosition.Paragraph.CreateNewParagraph());
                canRecord = true;
            }

            if (canRecord)
                OwnerControl.History.RecordUndo(history);

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Toggles the strike through
        /// </summary>
        public void ChangeUnderline()
        {
            if (IsReadOnly)
                return;
            if (LayoutViewer.IsSelected)
            {
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    HistoryInfo history = new HistoryInfo();
                    List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                    List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                    if (selectedcells.Count > 0)
                    {
                        foreach (TableCellAdv cell in selectedcells)
                        {
                            PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                            cleanedcell.RowIndex = cell.RowIndex;
                            cleanedcell.ColumnIndex = cell.ColumnIndex;
                            foreach (BlockAdv b in cell.Blocks)
                            {
                                cleanedcell.Blocks.Add(b.CopyBlock());
                            }
                            copiedcells.Add(cleanedcell);
                        }
                        history.PreservedCells = copiedcells;
                    }
                    bool underline = true;
                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                underline = (inlinesToFormat[i] as SpanAdv).Underline ? false : true;
                            (inlinesToFormat[i] as SpanAdv).Underline = underline;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                underline = (inlinesToFormat[i] as HyperlinkAdv).Underline ? false : true;
                            (inlinesToFormat[i] as HyperlinkAdv).Underline = underline;
                        }
                        inlinesToFormat[i].MeasureElements();
                    }

                    SpanAdv span = new SpanAdv();
                    span.Underline = underline;

                    GetHistoryInfo(history);
                    history.Action = Actions.Underline;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);

                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.IsUnderline = !OwnerControl.CurrentInlineStyle.IsUnderline;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// Returns the inlines to be formatted
        /// </summary>
        /// <returns></returns>
        internal List<Inline> GetInlinesToFormat()
        {
            List<Inline> inlinesToFormat = new List<Inline>();
            TableRowAdv firstrow=null;
            TableCellAdv firstcell=null;
            TableRowAdv secondrow=null;
            TableCellAdv secondcell=null;
            int firstInlineIndex = 0;
            int secondInlineIndex = 0;

            Inline newInline = null;
            Inline lastInline = null;

            if (start != null && end != null)
            {
                TextPosition startPos = start;
                TextPosition endPos = end;

                if (start.IsGreaterThan(end))
                {
                    startPos = end;
                    endPos = start;
                }

                BlockAdv startblk = Document.GetBlockFromVirtualPosition(startPos.VirtualPosition, ref firstrow, ref firstcell);

                BlockAdv endblk = Document.GetBlockFromVirtualPosition(endPos.VirtualPosition, ref secondrow, ref secondcell);

                BlockCollection<BlockAdv> blocks = new BlockCollection<BlockAdv>();
                blocks = Document.Sections[0].Blocks;

                if (startblk != null && endblk != null)
                {
                    if (startblk.AssociatedCell != null)
                    {
                        blocks = startblk.AssociatedCell.Blocks;
                    }
                }

                Inline firstInline = OwnerControl.PositionHandler.GetInlineFromIndex(startPos.Paragraph, startPos.Index, ref firstInlineIndex);
                Inline secondInline = OwnerControl.PositionHandler.GetInlineFromIndex(endPos.Paragraph, endPos.Index, ref secondInlineIndex);

                if (firstInline == secondInline && secondInlineIndex > firstInlineIndex)
                {
                    if (!firstInline.IsUIContainer && !firstInline.IsImageContainer)
                    {
                        int length = secondInlineIndex - firstInlineIndex;
                        int paraIndex = firstInline.Paragraph.Inlines.IndexOf(firstInline);

                        newInline = firstInline.CreatInline();

                        if (firstInlineIndex == 0)
                        {
                            newInline.InternalText = firstInline.InternalText.Substring(secondInlineIndex);
                            firstInline.InternalText = firstInline.InternalText.Substring(0, secondInlineIndex);
                            inlinesToFormat.Add(firstInline);
                        }
                        else
                        {
                            newInline.InternalText = firstInline.InternalText.Substring(firstInlineIndex, length);
                            lastInline = firstInline.CreatInline();
                            lastInline.InternalText = firstInline.InternalText.Substring(secondInlineIndex);
                            firstInline.InternalText = firstInline.InternalText.Substring(0, firstInlineIndex);
                        }

                        firstInline.MeasureElements();


                        if (newInline != null && !string.IsNullOrEmpty(newInline.InternalText))
                        {
                            newInline.Paragraph = firstInline.Paragraph;
                            firstInline.Paragraph.Inlines.Insert(++paraIndex, newInline);
                            newInline.MeasureElements();
                            if (firstInlineIndex != 0)
                                inlinesToFormat.Add(newInline);
                        }

                        if (lastInline != null && !string.IsNullOrEmpty(lastInline.InternalText))
                        {
                            lastInline.Paragraph = firstInline.Paragraph;
                            firstInline.Paragraph.Inlines.Insert(++paraIndex, lastInline);
                            lastInline.MeasureElements();
                        }

                        if (!AffectedBlocks.Contains(firstInline.Paragraph))
                        {
                            AffectedBlocks.Add(firstInline.Paragraph);
                        }
                    }
                }
                else
                {
                    bool isStarted = false;
                    foreach (BlockAdv block in blocks)
                    {
                        int value = block.ExtractInlines(startPos, endPos, ref isStarted, ref AffectedBlocks, ref inlinesToFormat);
                        if (value == 0)
                        {
                            isStarted = false;
                            break;
                        }
                        else if (value == 1)
                        {
                            isStarted = true;
                        }
                        else if (value == -1)
                        {
                            continue;
                        }
                    }
                }
            }

            return inlinesToFormat;
        }

        /// <summary>
        /// Toggles italic for the selection
        /// </summary>
        public void Italic()
        {
            if (IsReadOnly)
                return;
            if (OwnerControl.CurrentPage.IsSelected)
            {
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                inlinesToFormat = GetInlinesToFormat();
                if (inlinesToFormat.Count > 0)
                {
                    FontStyle fontStyle = FontStyles.Normal;
                    for (int i = 0; i < inlinesToFormat.Count; i++)
                    {
                        if (inlinesToFormat[i] is SpanAdv)
                        {
                            if (i == 0)
                                fontStyle = (inlinesToFormat[0] as SpanAdv).FontStyle == FontStyles.Italic ? FontStyles.Normal : FontStyles.Italic;
                            (inlinesToFormat[i] as SpanAdv).FontStyle = fontStyle;
                        }
                        else if (inlinesToFormat[i] is HyperlinkAdv)
                        {
                            if (i == 0)
                                fontStyle = (inlinesToFormat[0] as HyperlinkAdv).FontStyle == FontStyles.Italic ? FontStyles.Normal : FontStyles.Italic;
                            (inlinesToFormat[i] as HyperlinkAdv).FontStyle = fontStyle;
                        }
                    }

                    SpanAdv span = new SpanAdv();
                    span.FontStyle = fontStyle;

                    GetHistoryInfo(history);
                    history.Action = Actions.FontStyle;
                    history.InlineStyle = span;
                    OwnerControl.History.RecordUndo(history);


                    ArrangeLayout();
                }
            }
            else
            {
                OwnerControl.CurrentInlineStyle.FontStyle = OwnerControl.CurrentInlineStyle.FontStyle == FontStyles.Italic ? FontStyles.Normal : FontStyles.Italic;
                OwnerControl.PositionHandler.IsStyleChanged = true;
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        internal BlockCollection<BlockAdv> CopyBlocks(BlockCollection<BlockAdv> blocks)
        {
            BlockCollection<BlockAdv> blks = new BlockCollection<BlockAdv>();
            foreach (BlockAdv block in blocks)
            {
                blks.Add(block.CopyBlock());
            }

            return blks;
        }

        /// <summary>
        /// Pastes the selected text
        /// </summary>
        public void Paste()
        {
            if (IsReadOnly)
                return;

            BlockCollection<BlockAdv> blocks = ClipboardAdv.GetBlock();
            Paste(blocks);
            TextChangedEventArgs args = new TextChangedEventArgs();
            args.Text = ClipboardAdv.GetText();
            OwnerControl.FireTextChanged(args);
            OwnerControl.History.CheckForClearingRedo();
        }

        internal void Paste(BlockCollection<BlockAdv> blocks)
        {
            if (IsReadOnly)
                return;

            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.Paste;
            history.StartPosition = LayoutViewer.TextPosition.CopyForHistory();
            if (blocks != null && blocks.Count > 0)
            {
                if (LayoutViewer.IsSelected)
                {
                    history.StartPosition = Start.CopyForHistory();
                    history.EndPosition = End.CopyForHistory();
                    GetHistoryInfo(history);
                    OwnerControl.Selection.RemoveSelection(true);
                }

                history.TempPositionStart = history.StartPosition;
                history.CopiedBlocks = CopyBlocks(blocks);
                OwnerControl.History.RecordUndo(history);
            }

            PasteSelection(blocks);
            history.TempPositionEnd = LayoutViewer.TextPosition.CopyForHistory();
            OwnerControl.History.CheckForClearingRedo();
        }


        internal void PasteByMergingLastParagraph(BlockCollection<BlockAdv> bks, HistoryInfo history)
        {
            m_mergetable = true;
            TextPosition startPos = history.StartPosition.IsGreaterThan(history.EndPosition) ? history.EndPosition : history.StartPosition;
            TextPosition endPos = history.StartPosition.IsGreaterThan(history.EndPosition) ? history.StartPosition : history.EndPosition;

            if (startPos.IsInSameTable(endPos))
            {
                PasteSelectedCellsBlocks(bks, history);
            }
            else
            {
                PasteSelection(bks);
            }
            m_mergetable = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blks"></param>
        /// <param name="history"></param>
        internal void PasteSelectedCellsBlocks(BlockCollection<BlockAdv> blks, HistoryInfo history)
        {
            List<BlockAdv> blocks = new List<BlockAdv>();
            int index = 0;
            TextPosition position = OwnerControl.PositionHandler.TextPosition;

            TableAdv table = position.Paragraph.AssociatedCell.OwnerTable;

            List<PreservedCellsInfo> cleanedcells = history.PreservedCells;

            if (cleanedcells.Count > 0)
            {
                //foreach (TableRowAdv row in table.Rows)
                //{
                //    foreach (TableCellAdv cell in row.Cells)
                //    {
                //        if (index < cleanedcells.Count)
                //        {
                //            PreservedCellsInfo cleaned = cleanedcells[index];
                //            if (cell.ColumnIndex == cleaned.ColumnIndex && cell.RowIndex == cleaned.RowIndex)
                //            {
                //                index++;
                //                if (cell.Blocks.Count > 0)
                //                    cell.Blocks.Clear();
                //                foreach (BlockAdv b in cleaned.Blocks)
                //                {
                //                    cell.Blocks.Add(b);
                //                }
                //            }
                //        }
                //    }
                //}le

                foreach (PreservedCellsInfo preserved in history.PreservedCells)
                {
                    foreach (TableRowAdv row2 in table.Rows)
                    {
                        foreach (TableCellAdv cell2 in row2.Cells)
                        {
                            if (cell2.ColumnIndex == preserved.ColumnIndex && cell2.RowIndex == preserved.RowIndex)
                            {
                                if (cell2.Blocks.Count > 0)
                                {
                                    foreach (BlockAdv b in cell2.Blocks)
                                    {
                                        b.ClearLines();
                                    }
                                    cell2.Blocks.Clear();
                                }
                                foreach (BlockAdv b in preserved.Blocks)
                                {
                                    cell2.Blocks.Add(b);
                                }
                            }
                        }
                    }
                }
            }

            table.MeasureElements();

            LayoutViewer.SetPreviousBlocks();

            LayoutViewer.SetIsArrangedToFalse();

            table.ArrangeElements();

            LayoutViewer.SetIsArrangedToFalse();

            LayoutViewer.UpdateVerticalScrollBar();

            LayoutViewer.SetVisibleLinesToPage();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bks"></param>
        internal void PasteSelection(BlockCollection<BlockAdv> bks)
        {
            Inline inlineToPositionCaret = null;
            double inlineindex = 0;
            BlockCollection<BlockAdv> blocks = bks;
            AffectedBlocks.Clear(); ParagraphAdv paragraph = null;
            bool deleteparagraph = false;
            SectionAdv sectionAdv = Document.Sections[0];
            TextPosition tempPos = new TextPosition(Document);

            if (blocks != null && blocks.Count != 0)
            {
                Inline newInl = null;
                Inline existingInline = null;
                int indexInInline = 0;
                TextPosition pos = OwnerControl.PositionHandler.TextPosition;
                existingInline = OwnerControl.PositionHandler.GetInlineFromIndex(pos.Paragraph, pos.Index, ref indexInInline);
                if (existingInline != null && !existingInline.IsUIContainer && !existingInline.IsImageContainer)
                {
                    if (existingInline.InternalText.Length != indexInInline && indexInInline > 0)
                    {
                        newInl = existingInline.CreatInline();
                        newInl.InternalText = existingInline.InternalText.Substring(indexInInline);
                        newInl.MeasureElements();
                        existingInline.InternalText = existingInline.InternalText.Substring(0, indexInInline);
                        existingInline.MeasureElements();
                    }
                }

                paragraph = pos.Paragraph;
                //SectionAdv sectionAdv = !pos.Paragraph.IsInsideTable ? pos.Paragraph.Section : pos.Paragraph.GetLayoutViewer().Document.Sections[0];

                if (paragraph != null && (paragraph.Inlines.Count == 0 || paragraph.Inlines.Contains(existingInline)))
                {
                    int insertAt = existingInline != null ? indexInInline == 0 ? paragraph.Inlines.IndexOf(existingInline) : paragraph.Inlines.IndexOf(existingInline) + 1 : 0;
                    int insertParaAt = !paragraph.IsInsideTable ? sectionAdv.Blocks.IndexOf(paragraph) + 1 : paragraph.AssociatedCell.Blocks.IndexOf(paragraph) + 1;
                    AffectedBlocks.Add(paragraph);
                    ParagraphAdv para = null;
                    TableAdv newtable = null;
                    TableRowAdv newrow = null;
                    TableCellAdv newcell = null;

                    foreach (BlockAdv tempblock in blocks)
                    {
                        if (blocks.First() == tempblock)
                        {
                            if (tempblock.IsParagraph)
                            {
                                //(tempblock as ParagraphAdv).Clone(paragraph);
                                paragraph.Clone(tempblock as ParagraphAdv);
                                foreach (Inline tempInline in tempblock.Inlines)
                                {
                                    Inline newInline = tempInline.CreatInline();
                                    newInline.InternalText = tempInline.InternalText;
                                    newInline.Paragraph = paragraph;
                                    newInline.MeasureElements();
                                    paragraph.Inlines.Insert(insertAt, newInline);
                                    insertAt++;
                                    //if (blocks.Last() == tempblock && tempblock.Inlines.Last() == tempInline)
                                    //{
                                    //    inlineToPositionCaret = newInline;
                                    //    inlineindex = inlineToPositionCaret.GetLength();
                                    //}
                                    if (tempblock.Inlines.Last() == tempInline)
                                    {
                                        inlineToPositionCaret = newInline;
                                        inlineindex = inlineToPositionCaret.GetLength();
                                        if (inlineToPositionCaret != null && inlineToPositionCaret.Paragraph != null)
                                        {
                                            tempPos.Paragraph = inlineToPositionCaret.Paragraph;
                                            tempPos.SetIndex(inlineToPositionCaret.Paragraph.GetIndexFromInlineAndSpanIndex(inlineToPositionCaret, inlineindex).ToString());
                                            //OwnerControl.PositionHandler.PositionCursor();
                                        }
                                    }
                                }
                            }
                            else if (tempblock.IsTable)
                            {
                                newtable = tempblock.CreateBlock() as TableAdv;
                                newtable.LayoutViewer = LayoutViewer;
                                newtable.Section = LayoutViewer.OwnerControl.Document.Sections[0];
                                newtable.Margin = LayoutViewer.OwnerControl.Document.Sections[0].PageContentMargin;
                                foreach (TableRowAdv row in (tempblock as TableAdv).Rows)
                                {
                                    newrow = row.CreateNewRow();
                                    foreach (TableCellAdv c in row.Cells)
                                    {
                                        newcell = c.CreateNewCell(false, false);
                                        foreach (BlockAdv b in c.Blocks)
                                        {
                                            newcell.Blocks.Add(b.CopyBlock());
                                        }
                                        newrow.Cells.Add(newcell);

                                    }
                                    newtable.Rows.Add(newrow);
                                }
                                newtable.MeasureElements();

                                foreach (TableRowAdv row4 in newtable.Rows)
                                {
                                    if (newtable.Rows.Last() == row4)
                                    {
                                        foreach (TableCellAdv cell4 in row4.Cells)
                                        {
                                            if (row4.Cells.Last() == cell4)
                                            {
                                                BlockAdv b = cell4.Blocks.Last();
                                                if (b.Inlines.Count > 0)
                                                {
                                                    inlineToPositionCaret = b.Inlines.Last();
                                                    inlineindex = inlineToPositionCaret.GetLength();
                                                    if (inlineToPositionCaret != null && inlineToPositionCaret.Paragraph != null)
                                                    {
                                                        tempPos.Paragraph = inlineToPositionCaret.Paragraph;
                                                        tempPos.SetIndex(inlineToPositionCaret.Paragraph.GetIndexFromInlineAndSpanIndex(inlineToPositionCaret, inlineindex).ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    if (b.IsParagraph)
                                                    {
                                                        tempPos.Paragraph = b as ParagraphAdv;
                                                        tempPos.SetIndex(b.Length());
                                                    }
                                                    else if (b.IsTable)
                                                    {
                                                        BlockAdv lasblk = (b as TableAdv).GetLastBlockInLastCell();
                                                        while (lasblk.IsTable)
                                                        {
                                                            lasblk = (lasblk as TableAdv).GetLastBlockInLastCell();
                                                        }
                                                        tempPos.Paragraph = lasblk as ParagraphAdv;
                                                        tempPos.SetIndex(lasblk.Length());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                newtable.AssociatedCell = paragraph.AssociatedCell;
                                newtable.IsInsideTable = paragraph.IsInsideTable;

                                if (paragraph.PreviousBlock != null && m_mergetable && paragraph.PreviousBlock.IsTable)
                                {
                                    for (int i = (paragraph.PreviousBlock as TableAdv).Rows.Count - 1; i >= 0; i--)
                                    {
                                        TableRowAdv row = (paragraph.PreviousBlock as TableAdv).Rows[i];

                                        newrow = row.CreateNewRow();
                                        foreach (TableCellAdv c in row.Cells)
                                        {
                                            newcell = c.CreateNewCell(false, false);
                                            foreach (BlockAdv bl in c.Blocks)
                                            {
                                                newcell.Blocks.Add(bl.CopyBlock());
                                            }
                                            newrow.Cells.Add(newcell);
                                        }
                                        newtable.Rows.Insert(0, newrow);
                                    }
                                    int index = paragraph.IsInsideTable ? paragraph.AssociatedCell.Blocks.IndexOf(paragraph.PreviousBlock) : sectionAdv.Blocks.IndexOf(paragraph.PreviousBlock);

                                    if (paragraph.IsInsideTable)
                                    {
                                        //paragraph.AssociatedCell.Blocks.RemoveAt(removeat);
                                        paragraph.AssociatedCell.Blocks.Remove(paragraph.PreviousBlock);
                                    }
                                    else
                                    {
                                        //sectionAdv.Blocks.RemoveAt(removeat);
                                        sectionAdv.Blocks.Remove(paragraph.PreviousBlock);
                                    }
                                    paragraph.PreviousBlock.ClearLines();
                                    paragraph.PreviousBlock = newtable;
                                    newtable.NextBlock = paragraph;
                                    newtable.MeasureElements();

                                    if (paragraph.IsInsideTable)
                                    {
                                        paragraph.AssociatedCell.Blocks.Insert(index, newtable);
                                    }
                                    else
                                    {
                                        sectionAdv.Blocks.Insert(index, newtable);
                                    }
                                    insertParaAt--;
                                    deleteparagraph = true;
                                }
                                else
                                {
                                    if (pos.IsPositionAtParagraphStart)
                                    {
                                        insertParaAt--;
                                        deleteparagraph = true;
                                    }

                                    if (paragraph.IsInsideTable)
                                    {
                                        paragraph.AssociatedCell.Blocks.Insert(insertParaAt, newtable);
                                    }
                                    else
                                    {
                                        sectionAdv.Blocks.Insert(insertParaAt, newtable);
                                    }

                                    insertParaAt++;
                                }

                                AffectedBlocks.Add(newtable);
                            }
                        }
                        else
                        {
                            if (tempblock.IsParagraph)
                            {
                                para = (tempblock as ParagraphAdv).CreateNewParagraph();
                                para.Section = LayoutViewer.OwnerControl.Document.Sections[0];
                                para.Margin = LayoutViewer.OwnerControl.Document.Sections[0].PageContentMargin;
                                para.LayoutViewer = LayoutViewer;
                                foreach (Inline tempInline in tempblock.Inlines)
                                {
                                    Inline newInline = tempInline.CreatInline();
                                    newInline.InternalText = tempInline.InternalText;
                                    newInline.Paragraph = para;
                                    newInline.MeasureElements();
                                    para.Inlines.Add(newInline);

                                    if (tempblock.Inlines.Last() == tempInline)
                                    {
                                        inlineToPositionCaret = newInline;
                                        inlineindex = inlineToPositionCaret.GetLength();
                                        if (inlineToPositionCaret != null && inlineToPositionCaret.Paragraph != null)
                                        {
                                            tempPos.Paragraph = inlineToPositionCaret.Paragraph;
                                            tempPos.SetIndex(inlineToPositionCaret.Paragraph.GetIndexFromInlineAndSpanIndex(inlineToPositionCaret, inlineindex).ToString());
                                            //OwnerControl.PositionHandler.PositionCursor();
                                        }
                                    }
                                }
                                AffectedBlocks.Add(para);
                                para.AssociatedCell = paragraph.AssociatedCell;
                                para.IsInsideTable = paragraph.IsInsideTable;

                                if (paragraph.IsInsideTable)
                                {
                                    paragraph.AssociatedCell.Blocks.Insert(insertParaAt, para);
                                }
                                else
                                {
                                    sectionAdv.Blocks.Insert(insertParaAt, para);
                                }
                            }
                            else if (tempblock.IsTable)
                            {
                                newtable = tempblock.CreateBlock() as TableAdv;
                                newtable.LayoutViewer = LayoutViewer;
                                newtable.Section = LayoutViewer.OwnerControl.Document.Sections[0];
                                newtable.Margin = LayoutViewer.OwnerControl.Document.Sections[0].PageContentMargin;
                                foreach (TableRowAdv row in (tempblock as TableAdv).Rows)
                                {
                                    newrow = row.CreateNewRow();
                                    foreach (TableCellAdv c in row.Cells)
                                    {
                                        newcell = c.CreateNewCell(false, false);
                                        foreach (BlockAdv b in c.Blocks)
                                        {
                                            newcell.Blocks.Add(b.CopyBlock());
                                        }
                                        newrow.Cells.Add(newcell);
                                    }
                                    newtable.Rows.Add(newrow);
                                }
                                newtable.MeasureElements();
                                foreach (TableRowAdv row4 in newtable.Rows)
                                {
                                    if (newtable.Rows.Last() == row4)
                                    {
                                        foreach (TableCellAdv cell4 in row4.Cells)
                                        {
                                            if (row4.Cells.Last() == cell4)
                                            {
                                                BlockAdv b = cell4.Blocks.Last();
                                                if (b.Inlines.Count > 0)
                                                {
                                                    inlineToPositionCaret = b.Inlines.Last();
                                                    inlineindex = inlineToPositionCaret.GetLength();
                                                    if (inlineToPositionCaret != null && inlineToPositionCaret.Paragraph != null)
                                                    {
                                                        tempPos.Paragraph = inlineToPositionCaret.Paragraph;
                                                        tempPos.SetIndex(inlineToPositionCaret.Paragraph.GetIndexFromInlineAndSpanIndex(inlineToPositionCaret, inlineindex).ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    if (b.IsParagraph)
                                                    {
                                                        tempPos.Paragraph = b as ParagraphAdv;
                                                        tempPos.SetIndex(b.Length());
                                                    }
                                                    else if (b.IsTable)
                                                    {
                                                        BlockAdv lasblk = (b as TableAdv).GetLastBlockInLastCell();
                                                        while (lasblk.IsTable)
                                                        {
                                                            lasblk = (lasblk as TableAdv).GetLastBlockInLastCell();
                                                        }
                                                        tempPos.Paragraph = lasblk as ParagraphAdv;
                                                        tempPos.SetIndex(lasblk.Length());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                newtable.AssociatedCell = paragraph.AssociatedCell;
                                newtable.IsInsideTable = paragraph.IsInsideTable;

                                if (paragraph.IsInsideTable)
                                    paragraph.AssociatedCell.Blocks.Insert(insertParaAt, newtable);
                                else
                                    sectionAdv.Blocks.Insert(insertParaAt, newtable);

                                AffectedBlocks.Add(newtable);
                            }
                            insertParaAt++;
                        }

                            if (blocks.Last() == tempblock && blocks.First() != tempblock)
                            {
                                int i = insertAt;
                                List<Inline> list = new List<Inline>();

                                if (tempblock.IsParagraph)
                                {
                                    if (newInl != null && para != null)
                                    {
                                        para.Inlines.Add(newInl);
                                    }

                                    if (paragraph.Inlines.Count > i)
                                    {
                                        for (int j = i; j < paragraph.Inlines.Count; j++)
                                        {
                                            Inline inl = paragraph.Inlines[j];
                                            if (inl != null)
                                            {
                                                paragraph.Inlines.Remove(inl);
                                                inl.Paragraph = para;
                                                list.Add(inl);
                                                j--;
                                            }
                                        }
                                        for (int k = 0; k < list.Count; k++)
                                        {
                                            Inline inl = list[k];
                                            if (inl != null)
                                            {
                                                para.Inlines.Add(inl);
                                            }
                                        }
                                    }
                                }
                                else if (tempblock.IsTable)
                                {
                                    if (newInl != null || paragraph.Inlines.Count > i)
                                    {
                                        para = new ParagraphAdv();
                                        paragraph.Clone(para);
                                        para.Section = LayoutViewer.OwnerControl.Document.Sections[0];
                                        para.Margin = LayoutViewer.OwnerControl.Document.Sections[0].PageContentMargin;
                                        para.LayoutViewer = LayoutViewer;
                                        para.IsInsideTable = paragraph.IsInsideTable;
                                        para.AssociatedCell = paragraph.AssociatedCell;

                                        if (newInl != null)
                                        {
                                            para.Inlines.Add(newInl);
                                        }

                                        for (int j = i; j < paragraph.Inlines.Count; j++)
                                        {
                                            Inline inl = paragraph.Inlines[j];
                                            if (inl != null)
                                            {
                                                paragraph.Inlines.Remove(inl);
                                                inl.Paragraph = para;
                                                list.Add(inl);
                                                j--;
                                            }
                                        }

                                        for (int m = 0; m < list.Count; m++)
                                        {
                                            Inline inl = list[m];
                                            if (inl != null)
                                            {
                                                para.Inlines.Add(inl);
                                            }
                                        }
                                        //if (para.Inlines.Count > 0)
                                        //{
                                        //    inlineToPositionCaret = para.Inlines[0];
                                        //    inlineindex = 0;
                                        //}
                                        if (paragraph.IsInsideTable)
                                        {
                                            paragraph.AssociatedCell.Blocks.Insert(insertParaAt, para);
                                        }
                                        else
                                        {
                                            sectionAdv.Blocks.Insert(insertParaAt, para);
                                        }
                                    }
                                }
                                if (paragraph.NextBlock != null && paragraph.NextBlock.IsParagraph && m_mergeparagraph && tempblock.IsParagraph)
                                {
                                    foreach (Inline inline in paragraph.NextBlock.Inlines)
                                    {
                                        inline.Paragraph = para;
                                        para.Inlines.Add(inline);
                                    }
                                    para.LinkElementBoxes();
                                    if (sectionAdv.Blocks.Contains(paragraph.NextBlock))
                                    {
                                        paragraph.NextBlock.ClearLines();
                                        sectionAdv.Blocks.Remove(paragraph.NextBlock);
                                    }
                                }
                                if (paragraph.NextBlock != null && paragraph.NextBlock.IsTable && m_mergetable && tempblock.IsTable)
                                {
                                    AffectedBlocks.Remove(newtable);
                                    for (int j = 0; j < (paragraph.NextBlock as TableAdv).Rows.Count; j++)
                                    {
                                        TableRowAdv row = (paragraph.NextBlock as TableAdv).Rows[j];

                                        newrow = row.CreateNewRow();
                                        foreach (TableCellAdv c in row.Cells)
                                        {
                                            newcell = c.CreateNewCell(false, false);
                                            foreach (BlockAdv bl in c.Blocks)
                                            {
                                                newcell.Blocks.Add(bl.CopyBlock());
                                            }
                                            newrow.Cells.Add(newcell);
                                        }
                                        newtable.Rows.Add(newrow);
                                    }
                                    int index = paragraph.NextBlock.IsInsideTable ? paragraph.NextBlock.AssociatedCell.Blocks.IndexOf(paragraph.NextBlock) : sectionAdv.Blocks.IndexOf(paragraph.NextBlock);

                                    if (paragraph.NextBlock.IsInsideTable)
                                    {
                                        //paragraph.NextBlock.AssociatedCell.Blocks.RemoveAt(removeat);
                                        paragraph.NextBlock.AssociatedCell.Blocks.Remove(paragraph.NextBlock);
                                    }
                                    else
                                    {
                                        //sectionAdv.Blocks.RemoveAt(removeat);
                                        sectionAdv.Blocks.Remove(paragraph.NextBlock);
                                    }
                                    paragraph.NextBlock.ClearLines();

                                    //paragraph.NextBlock = newtable;

                                    newtable.MeasureElements();

                                    AffectedBlocks.Add(newtable);
                                    //insertParaAt++;
                                }
                            }
                            else if (blocks.Last() == tempblock && blocks.First() == tempblock)
                            {
                                if (newInl != null)
                                {
                                    if (tempblock.IsTable)
                                    {
                                        ParagraphAdv newPara = paragraph.CreateNewParagraph();
                                        newInl.Paragraph = newPara;
                                        newPara.LayoutViewer = paragraph.LayoutViewer;
                                        newPara.Section = paragraph.Section;
                                        newPara.Margin = paragraph.Margin;

                                        int i = insertAt;
                                        List<Inline> list = new List<Inline>();
                                        bool canCreateNewPara = false;
                                        if (newInl != null && newPara != null)
                                        {
                                            newPara.Inlines.Add(newInl);
                                            canCreateNewPara = true;
                                        }

                                        if (paragraph.Inlines.Count > i)
                                        {
                                            for (int j = i; j < paragraph.Inlines.Count; j++)
                                            {
                                                Inline inl = paragraph.Inlines[j];
                                                if (inl != null)
                                                {
                                                    paragraph.Inlines.Remove(inl);
                                                    inl.Paragraph = para;
                                                    list.Add(inl);
                                                    j--;
                                                    canCreateNewPara = true;
                                                }
                                            }
                                            for (int k = 0; k < list.Count; k++)
                                            {
                                                Inline inl = list[k];
                                                if (inl != null)
                                                {
                                                    newPara.Inlines.Add(inl);
                                                }
                                            }
                                        }

                                        if (canCreateNewPara)
                                        {
                                            if (paragraph.IsInsideTable)
                                            {
                                                paragraph.AssociatedCell.Blocks.Insert(insertParaAt, newPara);
                                            }
                                            else
                                            {
                                                sectionAdv.Blocks.Insert(insertParaAt, newPara);
                                            }

                                            AffectedBlocks.Add(newPara);
                                            list.Clear();
                                        }
                                    }
                                    else
                                    {
                                        newInl.Paragraph = paragraph;
                                        paragraph.Inlines.Insert(insertAt, newInl);
                                    }
                                }
                            }
                        }
                    }
                }

                if (deleteparagraph && paragraph != null)
                {
                    if (paragraph.Inlines.Count == 0)
                    {
                        if (paragraph.IsInsideTable)
                        {
                            paragraph.AssociatedCell.Blocks.Remove(paragraph);
                        }
                        else
                        {
                            sectionAdv.Blocks.Remove(paragraph);
                        }
                        paragraph.ClearLines();

                        if (AffectedBlocks.Contains(paragraph))
                            AffectedBlocks.Remove(paragraph);
                    }
                }

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetPreviousBlocks();

                LayoutViewer.SetIsArrangedToFalse();

                foreach (BlockAdv block in AffectedBlocks)
                {
                    block.LinkElementBoxes();
                    if (!block.IsArranged)
                    {
                        if (LayoutViewer.CurrentParagraph != null)
                        {
                            block.Section = LayoutViewer.CurrentParagraph.Section;
                            block.ArrangeElements();
                        }
                    }

                    block.IsArranged = false;
                }

                LayoutViewer.UpdateVerticalScrollBar();

                LayoutViewer.SetVisibleLinesToPage();

                OwnerControl.PositionHandler.TextPosition = tempPos;
                OwnerControl.PositionHandler.PositionCursor();

                AffectedBlocks.Clear();
        }

        /// <summary>
        /// Cuts the selected texts
        /// </summary>
        public void Cut()
        {
            if (IsReadOnly)
                return;
            if (OwnerControl.Viewer.IsSelected)
            {
                Copy();
                HistoryInfo history = new HistoryInfo();
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }

                history.Action = Actions.Cut;
                GetHistoryInfo(history);
                history.IsImageResizerSelected = LayoutViewer.IsImageResizerSelected;
                RemoveSelection(true);
                history.NextIsTable = LayoutViewer.IsTableAtNext();
                OwnerControl.History.RecordUndo(history);
                AffectedBlocks.Clear();
                OwnerControl.History.CheckForClearingRedo();
            }
        }

        bool mergePara = false;

        internal void RemoveSelectionMergeStartAndEnd(bool flag)
        {
            mergePara = true;
            RemoveSelection(flag);
            mergePara = false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RemoveSelection(bool flag)
        {
            if (LayoutViewer.IsSelected)
            {
                if (LayoutViewer.SelectedImage != null)
                {
                    LayoutViewer.ImageResizer.Visibility = Visibility.Collapsed;
                }

                if (Start != null && End != null)
                {
                    TextPosition startPos = start;
                    TextPosition endPos = end;
                    if (start.IsGreaterThan(end))
                    {
                        startPos = end;
                        endPos = start;
                    }
                    SectionAdv tempsection = null;
                    BlockAdv tempblock = null;
                    List<Inline> Inlines = new List<Inline>();
                    BlockCollection<BlockAdv> removedblocks = new BlockCollection<BlockAdv>();

                    bool started = false;

                    SectionAdv section = OwnerControl.Document.Sections[0];
                    TableCellAdv startcell = null;
                    TableCellAdv endcell = null;
                    TableRowAdv startrow = null;
                    TableRowAdv endrow = null;
                    BlockCollection<BlockAdv> blocks = new BlockCollection<BlockAdv>();

                    BlockAdv startblk = Document.GetBlockFromVirtualPosition(Start.VirtualPosition, ref startrow, ref startcell);
                    BlockAdv endblk = Document.GetBlockFromVirtualPosition(End.VirtualPosition, ref endrow, ref endcell);

                    if (startblk != null && endblk != null)
                    {
                        if (startblk.AssociatedCell != null)
                        {
                            blocks = startblk.AssociatedCell.Blocks;
                        }
                        else
                        {
                            blocks = Document.Sections[0].Blocks;
                        }

                        bool canjump = false;
                        if (startcell != null && endcell != null)
                        {
                            if (startPos.IsInSameTable(endPos))
                            {
                                List<TableCellAdv> selected = SelectedCellsInTable();
                                int i = 0;

                                if (startPos.IsPositionAtTableStart && endPos.IsPositionAtTableEnd)
                                {
                                    if (!removedblocks.Contains(startblk))
                                    {
                                        removedblocks.Add(startblk);
                                        if (AffectedBlocks.Contains(startblk))
                                        {
                                            AffectedBlocks.Remove(startblk);
                                        }
                                        canjump = true;
                                    }
                                }
                                else
                                {
                                    foreach (TableRowAdv row in (startblk as TableAdv).Rows)
                                    {
                                        foreach (TableCellAdv c in row.Cells)
                                        {
                                            if (i < selected.Count && c == selected[i])
                                            {
                                                c.Blocks.Clear();
                                                c.Blocks.Add(c.CreateEmptyBlock());
                                                if (!AffectedBlocks.Contains(startblk))
                                                {
                                                    AffectedBlocks.Add(startblk);
                                                }
                                                i++;
                                                canjump = true;
                                            }
                                        }
                                    }
                                }
                                if (canjump)
                                    goto Here;
                            }
                        }

                        foreach (BlockAdv b in blocks)
                        {
                            int value = b.SearchToCut(startPos, endPos, startcell, endcell, ref started, ref removedblocks, ref AffectedBlocks);
                            if (value == 0)
                            {
                                started = false;
                                break;
                            }
                            else if (value == 1)
                                started = true;
                            else if (value == -1)
                                continue;
                        }

                    Here: for (int i = removedblocks.Count - 1; i >= 0; i--)
                        {
                            removedblocks[i].ClearLines();
                            if (i == 0)
                            {
                                tempblock = removedblocks[i];
                                tempsection = section;
                            }
                            else
                            {
                                //removedPara[i].ClearLines();
                                removedblocks[i].ClearLines();
                                if (removedblocks[i].IsInsideTable)
                                {
                                    removedblocks[i].AssociatedCell.Blocks.Remove(removedblocks[i]);
                                }
                                else
                                {
                                    section.Blocks.Remove(removedblocks[i]);
                                }
                            }
                        }

                        if (mergePara && startPos != null && endPos != null && startPos.Paragraph != null
                            && endPos.Paragraph != null && startPos.Paragraph != endPos.Paragraph && endPos.Paragraph.Inlines.Count > 0)
                        {
                            while (endPos.Paragraph.Inlines.Count != 0)
                            {
                                endPos.Paragraph.Inlines[0].Paragraph = startPos.Paragraph as ParagraphAdv;
                                startPos.Paragraph.Inlines.Add(endPos.Paragraph.Inlines[0]);
                                endPos.Paragraph.Inlines.RemoveAt(0);
                            }

                            if (endPos.Paragraph.Inlines.Count == 0)
                            {
                                if (AffectedBlocks.Contains(endPos.Paragraph))
                                    AffectedBlocks.Remove(endPos.Paragraph);
                                endPos.Paragraph.ClearLines();
                            }

                            startPos.Paragraph.LinkElementBoxes();

                            if (endPos.Paragraph.Section != null && endPos.Paragraph.Section.Blocks.Contains(endPos.Paragraph))
                                endPos.Paragraph.Section.Blocks.Remove(endPos.Paragraph);

                            if (!AffectedBlocks.Contains(startPos.Paragraph))
                            {
                                AffectedBlocks.Add(startPos.Paragraph);
                            }
                        }

                        BlockAdv fisrtBlock = null;

                        LayoutViewer.SetPreviousBlocks();

                        if (removedblocks.Count != 0 && AffectedBlocks.Count == 0)
                        {
                            if (removedblocks.Last().NextBlock != null)
                            {
                                if (tempblock != null)
                                {
                                    if (tempblock.NextBlock.IsParagraph)
                                    {
                                        tempblock.ClearLines();
                                        if (tempblock.IsInsideTable)
                                        {
                                            tempblock.AssociatedCell.Blocks.Remove(tempblock);
                                        }
                                        else
                                        {
                                            section.Blocks.Remove(tempblock);
                                        }
                                        LayoutViewer.SetPreviousBlocks();
                                        removedblocks.Last().NextBlock.ArrangeElements();
                                        fisrtBlock = removedblocks.Last().NextBlock;
                                    }
                                    else if(flag)
                                    {
                                        tempblock.ClearLines();
                                        int index = tempblock.IsInsideTable ? tempblock.AssociatedCell.Blocks.IndexOf(tempblock) : Document.Sections[0].Blocks.IndexOf(tempblock);
                                        ParagraphAdv newParagraph = new ParagraphAdv();
                                        OwnerControl.CurrentParagraphStyle.SetDefaultStyle();
                                        OwnerControl.CurrentInlineStyle.SetDefaultStyle();
                                        newParagraph.Section = Document.Sections[0];
                                        newParagraph.LayoutViewer = LayoutViewer;
                                        newParagraph.IsInsideTable = tempblock.IsInsideTable;
                                        newParagraph.AssociatedCell = tempblock.AssociatedCell;
                                        tempblock.NextBlock.PreviousBlock = newParagraph;
                                        if (tempblock.IsInsideTable)
                                        {
                                            tempblock.AssociatedCell.Blocks.AddBlockAtIndex(index, newParagraph);
                                            tempblock.AssociatedCell.Blocks.Remove(tempblock);
                                        }
                                        else
                                        {
                                            Document.Sections[0].Blocks.AddBlockAtIndex(index, newParagraph);
                                            Document.Sections[0].Blocks.Remove(tempblock);
                                        }
                                        LayoutViewer.SetPreviousBlocks();
                                        newParagraph.ArrangeElements();
                                        fisrtBlock=newParagraph;
                                    }
                                    else
                                    {
                                        tempblock.ClearLines();
                                        if (tempblock.IsInsideTable)
                                        {
                                            tempblock.AssociatedCell.Blocks.Remove(tempblock);
                                        }
                                        else
                                        {
                                            section.Blocks.Remove(tempblock);
                                        }
                                        LayoutViewer.SetPreviousBlocks();
                                        removedblocks.Last().NextBlock.ArrangeElements();
                                        fisrtBlock = removedblocks.Last().NextBlock;
                                    }
                                }
                            }
                            else if (Document.IsEmpty)
                            {
                                if (tempblock != null)
                                {
                                    tempblock.ClearLines();
                                    if (tempblock.IsInsideTable)
                                    {
                                        tempblock.AssociatedCell.Blocks.Remove(tempblock);
                                    }
                                    else
                                    {
                                        section.Blocks.Remove(tempblock);
                                    }
                                    LayoutViewer.SetPreviousBlocks();
                                }
                                LayoutViewer.CreateBlockOnEmpty();
                                fisrtBlock = LayoutViewer.GetFirstBlock();
                                fisrtBlock.ArrangeElements();
                            }
                            else if (tempblock != null)
                            {
                                fisrtBlock = tempblock;
                                tempblock.ClearLines();
                                if (tempblock.IsTable)
                                {
                                    TableAdv table = tempblock as TableAdv;
                                    if (table.Rows.Count == 0)
                                    {
                                        if (tempblock.IsInsideTable)
                                        {
                                            tempblock.AssociatedCell.Blocks.Remove(tempblock);
                                        }
                                        else
                                        {
                                            section.Blocks.Remove(tempblock);
                                        }
                                        LayoutViewer.SetPreviousBlocks();
                                        LayoutViewer.CreateBlockOnEmpty();
                                        fisrtBlock = LayoutViewer.GetFirstBlock();
                                    }
                                }
                                else if (fisrtBlock.IsParagraph)
                                {
                                    if (fisrtBlock.Inlines.Count == 0)
                                        (fisrtBlock as ParagraphAdv).ListType = ListType.None;

                                }
                                AffectedBlocks.Add(tempblock);
                            }
                        }
                        else if (tempblock != null && section != null)
                        {
                            //tempParagraph.ClearLines();
                            if (tempblock.IsInsideTable)
                            {
                                tempblock.AssociatedCell.Blocks.Remove(tempblock);
                            }
                            else
                            {
                                section.Blocks.Remove(tempblock);
                            }
                            tempblock.ClearLines();
                            LayoutViewer.SetPreviousBlocks();
                        }

                        foreach (BlockAdv b in AffectedBlocks)
                        {
                            if (b is TableAdv)
                            {
                                b.MeasureElements();
                            }
                        }

                        LayoutViewer.SetIsArrangedToFalse();

                        foreach (BlockAdv block in AffectedBlocks)
                        {
                            block.LinkElementBoxes();
                            if (AffectedBlocks.First() == block && block.IsTable)
                            {
                                if ((startblk.IsTable && endblk.IsTable) || startblk.IsTable)
                                {
                                    if (block.NextBlock == null || block.NextBlock.IsTable)
                                    {
                                        ParagraphAdv newParagraph = new ParagraphAdv();
                                        OwnerControl.CurrentParagraphStyle.SetDefaultStyle();
                                        OwnerControl.CurrentInlineStyle.SetDefaultStyle();
                                        newParagraph.Section = Document.Sections[0];
                                        newParagraph.LayoutViewer = LayoutViewer;
                                        newParagraph.IsInsideTable = AffectedBlocks.First().IsInsideTable;
                                        newParagraph.AssociatedCell = AffectedBlocks.First().AssociatedCell;
                                        block.NextBlock = newParagraph;
                                        if (AffectedBlocks.First().IsInsideTable)
                                        {
                                            BlockAdv blk = AffectedBlocks.First();
                                            int index = blk.AssociatedCell.Blocks.IndexOf(blk);
                                            blk.AssociatedCell.Blocks.Insert(index + 1, newParagraph);
                                        }
                                        else
                                        {
                                            BlockAdv blk = AffectedBlocks.First();
                                            int index = section.Blocks.IndexOf(blk);
                                            section.Blocks.Insert(index + 1, newParagraph);
                                        }
                                        LayoutViewer.SetPreviousBlocks();
                                        fisrtBlock = newParagraph;
                                    }
                                    else
                                        fisrtBlock = block.NextBlock;
                                }
                                else if (startblk.IsParagraph)
                                {
                                    ParagraphAdv newParagraph = new ParagraphAdv();
                                    OwnerControl.CurrentParagraphStyle.SetDefaultStyle();
                                    OwnerControl.CurrentInlineStyle.SetDefaultStyle();
                                    newParagraph.Section = Document.Sections[0];
                                    newParagraph.LayoutViewer = LayoutViewer;
                                    newParagraph.IsInsideTable = AffectedBlocks.First().IsInsideTable;
                                    newParagraph.AssociatedCell = AffectedBlocks.First().AssociatedCell;
                                    //block.PreviousBlock.NextBlock = newParagraph;
                                    block.PreviousBlock = newParagraph;
                                    if (AffectedBlocks.First().IsInsideTable)
                                    {
                                        BlockAdv blk = AffectedBlocks.First();
                                        int index = blk.AssociatedCell.Blocks.IndexOf(blk);
                                        blk.AssociatedCell.Blocks.Insert(index, newParagraph);
                                    }
                                    else
                                    {
                                        BlockAdv blk = AffectedBlocks.First();
                                        int index = section.Blocks.IndexOf(blk);
                                        section.Blocks.Insert(index, newParagraph);
                                    }
                                    LayoutViewer.SetPreviousBlocks();
                                    newParagraph.ArrangeElements();
                                    fisrtBlock = newParagraph;
                                }
                                //fisrtBlock = block;
                            }
                            else if (AffectedBlocks.First() == block && block.IsParagraph)
                            {
                                fisrtBlock = block;
                            }

                            if (!block.IsArranged)
                            {
                                block.ArrangeElements();
                            }
                            block.IsArranged = false;
                        }

                        LayoutViewer.SetIsArrangedToFalse();

                        if (flag)
                        {
                            LayoutViewer.SetVisibleLinesToPage();

                            AffectedBlocks.Clear();

                            if (!Start.IsPositionAtParagraphStart && startblk.IsParagraph || canjump)
                            {
                                OwnerControl.PositionHandler.TextPosition = Start.IsGreaterThan(end) ? End : Start;
                                OwnerControl.PositionHandler.PositionCursor();
                            }
                            else if (fisrtBlock != null)
                            {
                                if ((fisrtBlock is ParagraphAdv) && fisrtBlock.Inlines.Count == 0)
                                {
                                    OwnerControl.CurrentInlineStyle.SetDefaultStyle();
                                }
                                if (fisrtBlock is ParagraphAdv)
                                {
                                    OwnerControl.PositionHandler.TextPosition.Paragraph = fisrtBlock as ParagraphAdv;
                                }

                                OwnerControl.PositionHandler.TextPosition.SetIndex("0");
                                OwnerControl.PositionHandler.PositionCursor();
                            }

                            LayoutViewer.IsSelected = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects the content of the whole page
        /// </summary>
        public void SelectAll()
        {
            if (OwnerControl != null && OwnerControl.PositionHandler.StartingPosOfDocument != null &&
                OwnerControl.PositionHandler.StartingPosOfDocument.Paragraph != null && OwnerControl.PositionHandler.EndingPosOfDocument != null
                && OwnerControl.PositionHandler.EndingPosOfDocument.Paragraph != null)
            {
                Start = OwnerControl.PositionHandler.StartingPosOfDocument;
                End = OwnerControl.PositionHandler.EndingPosOfDocument;
                Select();
                LayoutViewer.MoveToPosition(End);
            }
        }
    }
}
