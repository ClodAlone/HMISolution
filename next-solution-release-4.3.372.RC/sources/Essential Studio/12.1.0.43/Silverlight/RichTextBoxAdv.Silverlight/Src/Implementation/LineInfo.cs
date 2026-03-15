#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    public class LineInfo : DependencyObject
    {
        private double afterSpacing = 0;
        private double beforeSpacing = 0;
        private int pageIndex = 0;
        private bool isFirstLine = false;
        private bool isLastLine = false;
        //internal Canvas Container;
        internal Image Container;
        internal WriteableBitmap bmp;
        internal TranslateTransform transform;
        internal List<UIElement> Elements;
        internal bool IsDrawingContextCreated = true;
        public double Offset = 0;
        internal double MaxHeightForCalOffset = 17.78;
        internal List<TextBlock> TextRenderers;
        internal List<UIElement> DecoratingElements;
        internal const double PageLimit = 940.0;
        private bool haschildboxes = false;
        private RenderingOptions rendering = RenderingOptions.Render;

        /// <summary>
        /// Initializes the new instance of LineInfo class
        /// </summary>
        /// <param name="lineInfo"></param>
        public LineInfo()
        {
            ElementBoxes = new List<ElementBox>();
            //Container = new Image();
            //transform = new TranslateTransform();
            Elements = new List<UIElement>();
            //Elements.Add(Container);
            // Container = new Canvas();
            TextRenderers = new List<TextBlock>();
            DecoratingElements = new List<UIElement>();
        }

        /// <summary>
        /// Initializes the new instance of LineInfo class
        /// </summary>
        /// <param name="lineInfo"></param>
        public LineInfo(LineInfo lineInfo)
            : this()
        {
            if (lineInfo != null)
            {
                this.BoundingRectangle = new Rect(lineInfo.BoundingRectangle.X, lineInfo.BoundingRectangle.Y, lineInfo.BoundingRectangle.Width, lineInfo.BoundingRectangle.Height);
                this.isFirstLine = lineInfo.IsFirstLine;
                this.IsSplitted = lineInfo.IsSplitted;
                this.HasChildBoxes = lineInfo.HasChildBoxes;
                this.pageIndex = lineInfo.PageIndex;
            }
        }

        /// <summary>
        /// Gets or Sets the index of the page
        /// </summary>
        internal int PageIndex
        {
            get
            {
                return pageIndex;
            }
            set
            {
                if (PageIndex != value)
                {
                    RenderingOption = RenderingOptions.Render;
                }
                pageIndex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsFirstLine
        {
            get
            {
                return isFirstLine;
            }
            set
            {
                isFirstLine = value;
            }
        }

        internal bool IsLastLine
        {
            get
            {
                return isLastLine;
            }
            set
            {
                isLastLine = true;
            }
        }

        internal BlockAdv Block
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the bounding rectangle property
        /// </summary>
        public Rect BoundingRectangle
        {
            get
            {
                return (Rect)GetValue(BoundingRectangleProperty);
            }
            set
            {
                SetValue(BoundingRectangleProperty, value);
                if (Container != null)
                {
                    Canvas.SetLeft(Container, Location.X);
                    Canvas.SetTop(Container, Location.Y);
                }

                if (TextRenderers.Count > 0)
                {
                    foreach (TextBlock block in TextRenderers)
                    {
                        Canvas.SetTop(block, Location.Y);
                    }
                }
            }
        }

        private List<ElementBox> elementboxes = new List<ElementBox>();

        /// <summary>
        /// Gets or Sets the element boxes
        /// </summary>
        public List<ElementBox> ElementBoxes
        {
            get
            {
                return elementboxes;
            }
            set
            {
                elementboxes = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing
        /// </summary>
        internal double LineSpacing
        {
            get;
            set;
        }

        internal bool IsSplitted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the After Spacing
        /// </summary>
        internal double AfterSpacing
        {
            get
            {
                return afterSpacing;
            }
            set
            {
                afterSpacing = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Before Spacing
        /// </summary>
        internal double BeforeSpacing
        {
            get
            {
                return beforeSpacing;
            }
            set
            {
                beforeSpacing = value;
            }
        }

        /// <summary>
        /// Gets the location of the current line
        /// </summary>
        public Point Location
        {
            get
            {
                return new Point(BoundingRectangle.X, BoundingRectangle.Y);
            }
        }

        internal RenderingOptions RenderingOption
        {
            get
            {
                return rendering;
            }
            set
            {
                if (rendering != value)
                {
                    LineInfo line = GetParentLine();
                    if (line != null)
                    {
                        if (value == RenderingOptions.None)
                        {
                            if (line.RenderingOption != RenderingOptions.Render)
                            {
                                rendering = value;
                            }
                        }
                        else
                            rendering = value;
                    }
                    else
                    {
                        rendering = value;
                    }
                    SetRenderingOptions(rendering);
                }
            }
        }

        private LineInfo GetParentLine()
        {
            if (Block != null)
            {
                TableCellAdv cell = Block.AssociatedCell;
                if (cell != null)
                {
                    if (cell.CellElementBox != null)
                    {
                        return cell.CellElementBox.LineInfo;
                    }
                }
            }
            return null;
        }

        internal bool IsTableLine
        {
            get;
            set;
        }

        double width = 0.0;

        /// <summary>
        /// Gets or Sets the width of the line
        /// </summary>
        public double Width
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

        double height = 0.0;

        /// <summary>
        /// Gets or Sets the height of the line
        /// </summary>
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        internal bool HasChildBoxes
        {
            get
            {
                if (ElementBoxes.Count == 0)
                    return haschildboxes;
                return ElementBoxes.All(e => e is ChildTableCellElementBox);
            }
            set
            {
                haschildboxes = value;
            }
        }

        /// <summary>
        /// Registers the bounding rectangle property
        /// </summary>
        public static readonly DependencyProperty BoundingRectangleProperty = DependencyProperty.Register("BoundingRectangle", typeof(Rect), typeof(LineInfo), null);

        /// <summary>
        /// Adds the specified rectangle to the bounding rectangle
        /// </summary>
        /// <param name="rect"></param>
        public void Add(Rect rect)
        {
            BoundingRectangle.Union(rect);
        }

        /// <summary>
        /// 
        /// </summary>
        internal double CalculatedLineSpace = 0;

        /// <summary>
        /// Calculate the height of the line
        /// </summary>
        internal void CalculateMaxHeight()
        {
            double max = 0;
            double maxHeight = 0;
            for (int i = 0; i < ElementBoxes.Count; i++)
            {
                if (ElementBoxes[i].Element != null && !Elements.Contains(ElementBoxes[i].Element))
                {
                    Elements.Add(ElementBoxes[i].Element);
                }
                //if (!(ElementBoxes[i] is TableCellElementBox && ElementBoxes[i] is ChildTableCellElementBox && (ElementBoxes[i] as TableCellElementBox).BaseCell.HasRowSpan()))
                max = Math.Max(ElementBoxes[i].ElementSize.Height, max);
                if (!ElementBoxes[i].IsImageBox && !ElementBoxes[i].IsUIBox)
                    MaxHeightForCalOffset = Math.Max(ElementBoxes[i].ElementSize.Height, MaxHeightForCalOffset);
                if (LineSpacing >= 1)
                {
                    CalculatedLineSpace = (MaxHeightForCalOffset * LineSpacing) - MaxHeightForCalOffset;
                }
                else
                {
                    CalculatedLineSpace = 0;
                }
                if (ElementBoxes[i] is ImageElementBox && GetMaximumAscent() == ElementBoxes[i].BaselineOffset)
                    Height += GetMaximumDescent();
                Height = max + CalculatedLineSpace;
                maxHeight = MaxHeightForCalOffset + CalculatedLineSpace;
            }

            if (ElementBoxes.Count == 0)
                maxHeight = MaxHeightForCalOffset;

            if (Block != null)
            {
                if (Block.LineInfo.Last() == this && !(Block is TableAdv) && !Block.IsInsideTable)
                {
                    AfterSpacing = (Block as ParagraphAdv).AfterSpacing;
                    Height = Height + AfterSpacing;
                }

                if (Block.LineInfo.First() == this && !(Block is TableAdv) && !Block.IsInsideTable)
                {
                    BeforeSpacing = (Block as ParagraphAdv).BeforeSpacing;
                    Height = Height + BeforeSpacing;
                }
            }

            if (ElementBoxes.Count < 1)
                Height = BoundingRectangle.Height;

            BoundingRectangle = new Rect(Location, new Size(Width, Height));

            //bmp = new WriteableBitmap((int)Width, (int)Height);
            //Container.Source = bmp;
            //Container.Width = Width;
            //Container.Height = Height;
        }

        internal void SetRenderingOptions(RenderingOptions renderoption)
        {
            if (IsTableLine)
            {
                foreach (ElementBox box in ElementBoxes)
                {
                    ChildTableCellElementBox childbox = box as ChildTableCellElementBox;
                    TableCellElementBox tablebox = box as TableCellElementBox;
                    if (childbox != null)
                    {
                        foreach (LineInfo line in childbox.LineInfos)
                        {
                            if (PageIndex == line.PageIndex)
                                line.RenderingOption = renderoption;
                        }
                    }
                    else
                    {
                        foreach (LineInfo line2 in tablebox.LineInfos)
                        {
                            if (PageIndex == line2.PageIndex)
                                line2.RenderingOption = renderoption;
                        }
                    }
                }
            }
        }

#if !WPF

        public void CreateDrawingContext()
        {
            if (Container == null)
            {
                bmp = new WriteableBitmap((int)(Math.Ceiling(Width)), (int)(Math.Ceiling(Height)));
                Container = new Image() { IsHitTestVisible = false };
                Elements.Add(Container);
                Container.Source = bmp;
                Container.Width = Math.Ceiling(Width);
                Container.Height = Math.Ceiling(Height);
                foreach (ElementBox element in ElementBoxes)
                {
                    element.Render();
                }
                Canvas.SetLeft(Container, Location.X);
                Canvas.SetTop(Container, Location.Y);
            }
        }

        internal void RefreshImage()
        {
            //bmp = new WriteableBitmap((int)(Math.Ceiling(Width)), (int)(Math.Ceiling(Height)));
            bmp = new WriteableBitmap((int)Width, (int)Height);
            Container.Source = bmp;
        }

#endif

        /// <summary>
        /// Arranges the element boxes
        /// </summary>
        /// <param name="startPoint"></param>
        internal void ArrangeElementBoxes(TextAlignment alignment, double width)
        {
            double xPos = Location.X;

            if (alignment == TextAlignment.Left || Math.Ceiling(xPos + width) == Math.Ceiling(BoundingRectangle.Right))
            {
                foreach (ElementBox element in ElementBoxes)
                {
                    element.CalculatedLineSpacing = CalculatedLineSpace;
                    element.AfterSpacing = AfterSpacing;
                    element.BeforeSpacing = BeforeSpacing;
                    element.LineInfo = this;
                    element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                    xPos = xPos + element.ElementSize.Width;
                }
            }
            else if (alignment == TextAlignment.Center)
            {
                if (width > BoundingRectangle.Width)
                {
                    double offset = (width - BoundingRectangle.Width) / 2;
                    xPos = xPos + offset;

                    BoundingRectangle = new Rect(xPos, Location.Y, BoundingRectangle.Width, BoundingRectangle.Height);

                    Width = BoundingRectangle.Width;
                    Height = BoundingRectangle.Height;

                    foreach (ElementBox element in ElementBoxes)
                    {
                        element.CalculatedLineSpacing = CalculatedLineSpace;
                        element.AfterSpacing = AfterSpacing;
                        element.BeforeSpacing = BeforeSpacing;
                        element.LineInfo = this;
                        element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                        xPos = xPos + element.ElementSize.Width;
                    }
                }
                else
                {
                    foreach (ElementBox element in ElementBoxes)
                    {
                        element.CalculatedLineSpacing = CalculatedLineSpace;
                        element.AfterSpacing = AfterSpacing;
                        element.BeforeSpacing = BeforeSpacing;
                        element.LineInfo = this;
                        element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                    }
                }
            }
            else if (alignment == TextAlignment.Justify)
            {
                if (width > BoundingRectangle.Width)
                {
                    double offset = 0;

                    if (Block.LineInfo.Last() != this)
                    {
                        offset = (width - BoundingRectangle.Width) / GetElementBoxCount();
                    }

                    BoundingRectangle = new Rect(xPos, Location.Y, width, BoundingRectangle.Height);

                    Width = width;
                    Height = BoundingRectangle.Height;

                    foreach (ElementBox element in ElementBoxes)
                    {
                        element.CalculatedLineSpacing = CalculatedLineSpace;
                        element.AfterSpacing = AfterSpacing;
                        element.BeforeSpacing = BeforeSpacing;
                        element.LineInfo = this;
                        if (ElementBoxes.First() != element)
                        {
                            element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width + offset, Height);
                        }
                        else
                        {
                            element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width + (2 * offset), Height);
                        }
                        xPos = xPos + element.BoundingRectangle.Width;
                    }
                }
                else
                {
                    foreach (ElementBox element in ElementBoxes)
                    {
                        element.CalculatedLineSpacing = CalculatedLineSpace;
                        element.AfterSpacing = AfterSpacing;
                        element.BeforeSpacing = BeforeSpacing;
                        element.LineInfo = this;
                        element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                    }
                }
            }
            else if (alignment == TextAlignment.Right)
            {
                if (Block.IsInsideTable)
                    xPos -= 8.0;

                if (width > BoundingRectangle.Width)
                {
                    double offset = (width - BoundingRectangle.Width);

                    xPos = xPos + offset;

                    BoundingRectangle = new Rect(xPos, Location.Y, BoundingRectangle.Width, BoundingRectangle.Height);

                    Width = BoundingRectangle.Width;
                    Height = BoundingRectangle.Height;

                    foreach (ElementBox element in ElementBoxes)
                    {
                        element.CalculatedLineSpacing = CalculatedLineSpace;
                        element.AfterSpacing = AfterSpacing;
                        element.BeforeSpacing = BeforeSpacing;
                        element.LineInfo = this;
                        element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                        xPos = xPos + element.ElementSize.Width;
                    }
                }
                else
                {
                    foreach (ElementBox element in ElementBoxes)
                    {
                        element.CalculatedLineSpacing = CalculatedLineSpace;
                        element.AfterSpacing = AfterSpacing;
                        element.BeforeSpacing = BeforeSpacing;
                        element.LineInfo = this;
                        element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                    }
                }
            }
        }

        internal void ArrangeTableElementBoxes()
        {
            double xPos = Location.X;
            foreach (ElementBox element in ElementBoxes)
            {
                element.CalculatedLineSpacing = CalculatedLineSpace;
                element.AfterSpacing = AfterSpacing;
                element.BeforeSpacing = BeforeSpacing;
                element.LineInfo = this;
                element.BoundingRectangle = new Rect(xPos, Location.Y, element.ElementSize.Width, Height);
                xPos = xPos + element.ElementSize.Width;
            }
        }

        private double GetElementBoxCount()
        {
            double count = 0;
            foreach (ElementBox element in ElementBoxes)
            {
                if (element.IsAddedToLine)
                    ++count;
            }

            return count;
        }

        internal void Add(ElementBox box)
        {
            ElementBoxes.Add(box);
            //CalculateMaxHeight(box);
            //if (!Container.Children.Contains(box.Element))
            //{
            //    Container.Children.Add(box.Element);
            //}
        }

        internal void CalEmptyLineHeight(InlineStyle style)
        {
            if (style != null)
            {
                Size size = TextHelper.MeasureText(" ", style);
                Height = size.Height;
                Width = size.Width;
                if (LineSpacing >= 1)
                {
                    CalculatedLineSpace = (Height * LineSpacing) - Height;
                }
                else
                {
                    CalculatedLineSpace = 0;
                }

                Offset = (Height * 20) / 100;
                Height = Height + CalculatedLineSpace + AfterSpacing + BeforeSpacing + Offset;
            }
        }

        internal double CalculateEmptyLineHeight(InlineStyle style)
        {
            double height = 0.0;
            if (style != null)
            {
                Size size = TextHelper.MeasureText(" ", style);
                height = size.Height;
                if (LineSpacing >= 1)
                {
                    CalculatedLineSpace = (height * LineSpacing) - height;
                }
                else
                {
                    CalculatedLineSpace = 0;
                }

                Offset = (height * 20) / 100;
                height = height + CalculatedLineSpace + AfterSpacing + BeforeSpacing + Offset;
            }
            return height;
        }

        internal double GetMaxElementBoxHeight()
        {
            double maxheight = 0.0;
            if (IsTableLine)
            {
                foreach (ElementBox e in ElementBoxes)
                {
                    TableCellElementBox cellbox = e as TableCellElementBox;
                    if (cellbox != null)
                    {
                        if (!cellbox.HasRowSpan())
                        {
                            maxheight = Math.Max(maxheight, cellbox.LineInfos.Sum<LineInfo>(l => l.Height));
                        }
                    }
                }
            }
            return maxheight;
        }

        internal void SplitTableLine(double newlineheight)
        {
            ChildTableCellElementBox childbox = null;
            TableCellElementBox tablecellbox = null;
            List<LineInfo> linestoremove = new List<LineInfo>();
            double currentlineheight = Height - newlineheight;
            LineInfo newline = null;
            double previouspoint = 0.0;
            double sum = 0;
            double minheight = CalculateEmptyLineHeight(Block.GetLayoutViewer().OwnerControl.CurrentInlineStyle);

            if (currentlineheight < minheight)
            {
                return;
            }

            if (ElementBoxes.Count <= 0)
                return;

            Height = currentlineheight;

            if (IsFirstLine)
            {
                previouspoint = Block.GetLayoutViewer().Document.PageContentMargin.Top;
            }
            else
            {
                if (this != Block.LineInfo.First())
                {
                    previouspoint = Block.LineInfo[Block.LineInfo.IndexOf(this) - 1].BoundingRectangle.Bottom;
                }
                else
                {
                    if (Block.PreviousBlock != null)
                        previouspoint = Block.PreviousBlock.EndPoint;
                    else
                        previouspoint = Block.StartPoint;
                }
            }

            newline = new LineInfo();

            newline.Block = Block;
            newline.PageIndex = PageIndex + 1;
            newline.IsTableLine = IsTableLine;

            foreach (ElementBox e in ElementBoxes)
            {
                tablecellbox = e as TableCellElementBox;
                childbox = new ChildTableCellElementBox(e);
                childbox.BaseCell = tablecellbox.BaseCell;
                if (tablecellbox.BottomCellBox != null)
                {
                    childbox.BottomCellBox = tablecellbox.BottomCellBox;
                    tablecellbox.BottomCellBox = null;
                }
                childbox.ColumnIndex = tablecellbox.ColumnIndex;
                childbox.ColumnSpan = tablecellbox.ColumnSpan;
                childbox.Background = tablecellbox.Background;
                tablecellbox.ChildBox = childbox;
                childbox.RowSpan = tablecellbox.RowSpan;
                childbox.IsBottomHide = tablecellbox.IsBottomHide;
                int j = 0;
                double limit = previouspoint + Height;
                sum = 0.0;
                while (tablecellbox.LineInfos.Count > 0)
                {
                    LineInfo line = tablecellbox.LineInfos[j];
                    line.IsSplitted = false;
                    line.CalculateMaxHeight();
                    if ((sum + line.Height) > Height)
                    {
                        linestoremove.Add(line);
                    }
                    if (line.IsTableLine && linestoremove.Count > 0 && linestoremove.First() == line && (sum + line.Height) > Height)
                    {
                        line.SplitTableLine((sum + line.Height) - Height);

                        if (!line.IsSplitted)
                        {
                            Rect bounding = line.BoundingRectangle;
                            bounding.Height = line.Height;
                            line.BoundingRectangle = bounding;
                            line.ArrangeTableElementBoxes();
                        }
                    }
                    j++;
                    sum = sum + line.Height;
                    if (j == tablecellbox.LineInfos.Count)
                        break;
                }
                for (int i = 0; i < linestoremove.Count; i++)
                {
                    LineInfo l = linestoremove[i];
                    if (l.IsTableLine && linestoremove.First() == l && l.IsSplitted)
                        continue;
                    childbox.LineInfos.Add(l);
                }
                linestoremove.Clear();
                newline.ElementBoxes.Add(childbox);
                double height = childbox.LineInfos.Sum<LineInfo>(l => l.Height);
                childbox.ElementSize = new Size(e.ElementSize.Width, height);
            }

            Rect newrect = BoundingRectangle;
            newline.Width = Width;
            newrect.Y = Block.LayoutViewer.Document.PageContentMargin.Top;
            newline.BoundingRectangle = newrect;
            newline.IsFirstLine = true;
            IsSplitted = true;

            if (newlineheight < minheight)
            {
                newline.Height = minheight;
            }
            IsLastLine = true;
            if (Block.IsInsideTable)
            {
                newline.CalculateMaxHeight();
                Block.AssociatedCell.LineInfos.Insert(Block.AssociatedCell.LineInfos.IndexOf(this) + 1, newline);
            }
            Block.LineInfo.Insert(Block.LineInfo.IndexOf(this) + 1, newline);
        }

        /// <summary>
        /// Returns the element from box
        /// </summary>
        /// <param name="cursorPoint"></param>
        internal ElementBox GetElementBoxFromPoint(Point cursorPoint)
        {
            cursorPoint = new Point(Math.Floor(cursorPoint.X), cursorPoint.Y);
            ElementBox elementBox = null;
            foreach (ElementBox element in ElementBoxes)
            {
                Rect rect = new Rect(element.Location, new Size(Math.Ceiling(element.BoundingRectangle.Width), element.BoundingRectangle.Height));
                if (rect.Contains(cursorPoint))
                {
                    elementBox = element;
                }
                else if (element.BoundingRectangle.Left <= cursorPoint.X && element.BoundingRectangle.Right >= cursorPoint.X)
                {
                    elementBox = element;
                }
            }

            if (elementBox == null && ElementBoxes.Count > 0)
            {
                if (cursorPoint.X >= BoundingRectangle.Right)
                {
                    elementBox = ElementBoxes.Last();
                }
                else if (cursorPoint.X <= BoundingRectangle.Left)
                {
                    elementBox = ElementBoxes.First();
                }
            }
            return elementBox;
        }

        internal void ClearElementBoxes()
        {
            if (Container != null && Container.Parent != null)
            {
                Canvas canvas = Container.Parent as Canvas;
                canvas.Children.Remove(Container);
                Container = null;
            }

            if (Block !=null && Block.IsInsideTable)
            {
                //LineInfo line = GetRootLine();
                //if (line != null)
                //{
                //    PageAdv page = Block.GetLayoutViewer().GetPageFromLine(line);
                //    Block.GetLayoutViewer().RenderingManager.Remove(this, page);
                //}
                LayoutViewer viewer = Block.GetLayoutViewer();
                if (PageIndex < viewer.Pages.Count)
                {
                    PageAdv page = viewer.Pages[PageIndex];
                    viewer.RenderingManager.Remove(this, page);
                }
            }

            ElementBoxes.Clear();
            TextRenderers.Clear();
            DecoratingElements.Clear();
            Elements.Clear();
            //Container.Children.Clear();
        }

        public void DrawText(Point point)
        {
            if (bmp != null)
            {
                if (transform == null)
                {
                    transform = new TranslateTransform();
                }
                transform.X = point.X;
                transform.Y = point.Y;

#if !WPF
                bmp.Invalidate();
                bmp.Render(TextHelper.TextMeasurer, transform);
                bmp.Invalidate();
#endif

            }
        }

        internal LineInfo GetRootLine()
        {
            if (!Block.IsInsideTable)
                return Block.LineInfo[0];

            else
            {
                BlockAdv block = Block.AssociatedCell.OwnerTable;
                while (block.IsInsideTable)
                {
                    block = block.AssociatedCell.OwnerTable;
                }

                if (block.LineInfo.Count > 0)
                    return block.LineInfo[0];
                else
                    return null;
            }
        }

        public void DrawRectangle(Rect rect, Color color)
        {
            if (bmp != null)
            {
                int x1 = (int)rect.X;
                int y1 = (int)rect.Y;
                int x2 = (int)rect.Right;
                int y2 = (int)Math.Ceiling(rect.Bottom);

#if !WPF
                bmp.Invalidate();
                bmp.FillRectangle(x1, y1, x2, y2, color);
                bmp.Invalidate();
#endif

            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            if (bmp != null)
            {
#if !WPF
                bmp.Invalidate();
                bmp.DrawLine(x1, y1, x2, y2, color);
                bmp.Invalidate();
#endif
            }
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (ElementBox element in ElementBoxes)
            {
                str += element.InternalText;
            }
            return str;
        }

        internal double GetMaximumAscent()
        {
            double maxAscent = 0;
            if (ElementBoxes != null)
            {
                foreach (ElementBox box in ElementBoxes)
                    if (box.BaselineOffset > maxAscent)
                        maxAscent = box.BaselineOffset;
            }
            return maxAscent;
        }

        internal double GetMaximumDescent()
        {
            double maxDescent = 0;
            if (ElementBoxes != null)
            {
                foreach (ElementBox box in ElementBoxes)
                    if ((box.ElementSize.Height - box.BaselineOffset) > maxDescent)
                        maxDescent = box.ElementSize.Height - box.BaselineOffset;
            }
            return maxDescent;
        }
    }
}


public enum RenderingOptions
{
    Render,
    RemoveAndAdd,
    None
}
