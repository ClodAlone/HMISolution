#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TableCellElementBox : ElementBox
    {
        internal BlockCollection<BlockAdv> cellblocks = new BlockCollection<BlockAdv>();
        internal ObservableCollection<LineInfo> lineinfos = new ObservableCollection<LineInfo>();
        bool isbottomhide = false;
        bool istophide = false;
        private Color color = Color.FromArgb(0, 0, 0, 0);

        public TableCellElementBox()
        {
            LineInfos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LineInfos_CollectionChanged);
        }

        void LineInfos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IList collection = e.NewItems;
            int newindex = e.NewStartingIndex;

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                collection = e.OldItems;
                newindex = e.OldStartingIndex;
            }
            if (collection != null)
            {
                foreach (LineInfo l in collection)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        if (BottomCellBox != null)
                        {
                            if (BottomCellBox.LineInfos.Contains(l))
                            {
                                BottomCellBox.LineInfos.Remove(l);
                            }
                        }
                    }
                }
            }
        }
        
        internal int RowSpan
        {
            get;
            set;
        }

        internal int ColumnSpan
        {
            get;
            set;
        }

        internal int RowIndex
        {
            get;
            set;
        }

        internal int ColumnIndex
        {
            get;
            set;
        }

        public Color Background
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        internal double CellHeight
        {
            get
            {
                return LineInfos.Sum<LineInfo>(line => line.Height);
            }
        }

        private bool isfullcellselected = false;

        internal bool IsFullCellSelected
        {
            get
            {
                return isfullcellselected;
            }
            set
            {
                isfullcellselected = value;
            }
        }

        internal TableCellAdv BaseCell
        {
            get;
            set;
        }
        
        internal TableCellElementBox BottomCellBox
        {
            get;
            set;
        }

        internal BlockCollection<BlockAdv> CellBlocks
        {
            get
            {
                return cellblocks;
            }
            set
            {
                cellblocks = value;
            }
        }

        internal ObservableCollection<LineInfo> LineInfos
        {
            get
            {
                return lineinfos;
            }
            set
            {
                lineinfos = value;
            }
        }

        internal bool IsBottomHide
        {
            get
            {
                return isbottomhide;
            }
            set
            {
                isbottomhide = value;
            }
        }

        internal bool IsTopHide
        {
            get
            {
                return istophide;
            }
            set
            {
                istophide = value;
            }
        }

        internal ElementBox ParentBox
        {
            get;
            set;
        }

        internal ChildTableCellElementBox ChildBox
        {
            get;
            set;
        }

        internal Path AssociatedPath
        {
            get;
            set;
        }

        internal override void SetElementPosition()
        {
            Point point = new Point(Location.X, Location.Y);

            ElementLocation = point;

            double y = point.Y;

            foreach (LineInfo line in LineInfos)
            {
                double xPos = line.BoundingRectangle.X;
                line.BoundingRectangle = new Rect(ElementLocation.X + 5d, y, line.Width, line.Height);
                ParagraphAdv paragraph = line.Block as ParagraphAdv;
                if (paragraph != null)
                {
                    ListItem item = paragraph.AssociatedListItem;
                    if (item != null && paragraph.ListType !=ListType.None)
                    {
                        line.BoundingRectangle = new Rect(line.BoundingRectangle.X + xPos, y, line.Width, line.Height);
                        item.SetPosition(new Point(ElementLocation.X + 10d, paragraph.LineInfo.First().BoundingRectangle.Top));
                    }
                }
                if (LineInfo != null)
                {
                    line.PageIndex = LineInfo.PageIndex;
                }

                if (line.Block.IsTable)
                    line.ArrangeTableElementBoxes();
                else
                    line.ArrangeElementBoxes((line.Block as ParagraphAdv).TextAlignment, BoundingRectangle.Width);

                y += line.BoundingRectangle.Height;
            }
        }

        internal LineInfo GetLineFromPoint(Point cursorPoint)
        {
            Point newpoint = new Point(Math.Floor(cursorPoint.X), cursorPoint.Y);
            LineInfo lineInfo = null;
            foreach (LineInfo line in LineInfos)
            {
                Rect newrect = new Rect(line.BoundingRectangle.X, line.BoundingRectangle.Y, Math.Ceiling(line.BoundingRectangle.Width), line.BoundingRectangle.Height);
                if (newrect.Contains(newpoint))
                {
                    lineInfo = line;
                    break;
                }
                else if ((newpoint.X <= line.BoundingRectangle.Left || newpoint.X >= line.BoundingRectangle.Right) &&
                    (newpoint.Y >= line.BoundingRectangle.Top && newpoint.Y <= line.BoundingRectangle.Bottom) && BoundingRectangle.Contains(newpoint))
                {
                    lineInfo = line;
                    break;
                }
            }
            if (LineInfos.Count == 1)
            {
                if (lineInfo == null)
                {
                    if (newpoint.Y >= LineInfos.Last().BoundingRectangle.Top)
                    {
                        lineInfo = LineInfos.Last();
                    }
                    else if (newpoint.Y <= LineInfos.First().BoundingRectangle.Top)
                    {
                        lineInfo = LineInfos.First();
                    }
                }
            }

            if (lineInfo != null)
                return lineInfo;
            return null;
        }

        internal override Point GetApproxRight(int index)
        {
            double y = BoundingRectangle.Top + BoundingRectangle.Height / 2;
            double x = 0;
            if (index == 0)
            {
                x = ElementLocation.X;
            }
            else
            {
                if (this.Inline.Paragraph.LayoutViewer is PageLayoutViewer)
                {
                    x = ElementLocation.X + this.Inline.Paragraph.Section.PageSize.Width;
                }
                else if (this.Inline.Paragraph.LayoutViewer is FlowLayoutViewer)
                {
                    x = ElementLocation.X + this.Inline.Paragraph.LayoutViewer.AvailableSize.Width;
                }
            }
            return new Point(x, y);
        }

        public override void ArrangeLinesInSpnnedBoxes()
        {
            double maxrowheight = LineInfo.GetMaxElementBoxHeight();
            double num = 0.0;

            int i = 0;
            if (HasRowSpan() && BottomCellBox !=null)
            {
                while (i < LineInfos.Count)
                {
                    LineInfo line = LineInfos[i];
                    num += line.Height;
                    if (num > maxrowheight)
                    {
                        BottomCellBox.LineInfos.Add(line);
                        LineInfos.Remove(line);
                        continue;
                    }
                    i++;
                }
            }
        }

        internal bool HasRowSpan()
        {
            if (RowSpan > 1)
                return true;
            return false;
        }


        internal PathFigure SelectBox()
        {
            PathFigure figure = new PathFigure();
            LineSegment lineseg = null;
            figure.IsClosed = true;
            figure.StartPoint=new Point(BoundingRectangle.X,BoundingRectangle.Bottom);
            lineseg = new LineSegment();
            lineseg.Point = new Point(BoundingRectangle.X, BoundingRectangle.Top);
            figure.Segments.Add(lineseg);
            lineseg = new LineSegment();
            lineseg.Point = new Point(BoundingRectangle.Right, BoundingRectangle.Y);
            figure.Segments.Add(lineseg);
            lineseg = new LineSegment();
            lineseg.Point = new Point(BoundingRectangle.Right, BoundingRectangle.Bottom);
            figure.Segments.Add(lineseg);
            lineseg = new LineSegment();
            lineseg.Point = new Point(BoundingRectangle.X, BoundingRectangle.Bottom);
            figure.Segments.Add(lineseg);
            IsFullCellSelected = true;
            return figure;
        }
                        
    }
}
