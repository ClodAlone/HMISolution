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
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
#if WPF
using Syncfusion.Licensing;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    public class CaretAdv : Control
    {
        /// <summary>
        /// cursor storyboard
        /// </summary>
        private Storyboard storyBoard;

        /// <summary>
        /// inner rectangle
        /// </summary>
        private Rectangle rectangle;

        /// <summary>
        /// Gets or Sets the double animation
        /// </summary>
        //private DoubleAnimation animation;

        DoubleAnimationUsingKeyFrames animation;


        private Point location;

        /// <summary>
        /// Gets or Sets the cursors owner page
        /// </summary>
        internal PageAdv Page
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the location of the cursor
        /// </summary>
        public Point Location
        {
            get
            {
                return location;
                // return new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            }
            set
            {
                location = value;
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }

        /// <summary>
        /// Gets or Sets the current element
        /// </summary>
        internal ElementBox CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the text after which cursor appears
        /// </summary>
        internal string AfterText
        {
            get;
            set;
        }

        private double caretspanindex;
        /// <summary>
        /// Gets or Sets the caret's index in the span
        /// </summary>
        internal double CaretSpanIndex
        {
            get
            {
                return caretspanindex;
            }
            set
            {
                caretspanindex = value;
            }
        }

        /// <summary>
        /// Gets or Sets the current inline
        /// </summary>
        internal Inline CurrentInline;

        /// <summary>
        /// Returns whether the currently cursor position is at the last position
        /// </summary>
        internal bool IsPositionAtMiddle
        {
            get
            {
                if (Page.CurrentLine != null && Page.LineInfos.Count > 0)
                {
                    if (Page.LineInfos.Last<LineInfo>() == Page.CurrentLine)
                    {
                        if (Page.CurrentLine.ElementBoxes.Count != 0)
                        {
                            return Math.Floor(Location.X) < Math.Floor(Page.CurrentLine.BoundingRectangle.Right - 1) ? true : false;
                        }
                        return false;
                    }
                    else
                    {
                        if (Page.CurrentParagraph.Inlines.Count == 0 && Page.CurrentLine.ElementBoxes.Count == 0)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Initializes the new instance of RichEditCaret class
        /// </summary>
        public CaretAdv()
        {
            this.DefaultStyleKey = typeof(CaretAdv);

            storyBoard = new Storyboard();
            //animation = new DoubleAnimation();
            //animation.Duration = new Duration(new TimeSpan(0, 0, 0,0,500));
            //animation.To = 0;
            //animation.AutoReverse = true;
            //animation.RepeatBehavior = RepeatBehavior.Forever;

            animation = new DoubleAnimationUsingKeyFrames();
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.AutoReverse = true;
            //animation.BeginTime = new TimeSpan(0);
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 600));

            DiscreteDoubleKeyFrame keyFrame = new DiscreteDoubleKeyFrame();
            keyFrame.KeyTime = new TimeSpan(0, 0, 0, 0, 300);
            keyFrame.Value = 0;

            animation.KeyFrames.Add(keyFrame);

            storyBoard.Children.Add(animation);

            this.Width = 1.5;
            this.Height = 15;
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(CaretAdv));
            }
#endif

        }

        #region Methods

        /// <summary>
        /// Applies the template
        /// </summary>
        public override void OnApplyTemplate()
        {
            rectangle = (Rectangle)this.GetTemplateChild("rectangle");

            if (animation != null && rectangle != null)
            {
                storyBoard.Stop();
                Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard.SetTarget(animation, rectangle);
                storyBoard.Begin();
            }

#if WPF
            if (!(Page != null && Page.OwnerControl != null && Page.OwnerControl.IsFocused))
#else
            if (FocusManager.GetFocusedElement() != Page.OwnerControl)
#endif
                Hide();

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Shows the caret
        /// </summary>
        internal void Show()
        {
#if WPF
            if (Page != null && Page.OwnerControl != null && Page.OwnerControl.IsFocused)
            {
                if (!Page.OwnerControl.IsKeyboardFocused
                    && (Page.OwnerControl.ContextMenu == null || !Page.OwnerControl.ContextMenu.IsOpen))
                    Keyboard.Focus(Page.OwnerControl);
                this.Visibility = Visibility.Visible;
            }
#else
            if (Page != null && FocusManager.GetFocusedElement() == Page.OwnerControl)
                this.Visibility = Visibility.Visible;
#endif
            //if (rectangle != null)
            //{               
            //    this.rectangle.Visibility = Visibility.Visible;
            //}
        }

        /// <summary>
        /// Hides the caret
        /// </summary>
        internal void Hide()
        {
            this.Visibility = Visibility.Collapsed;
            //if (rectangle != null)
            //{
            //    this.rectangle.Visibility = Visibility.Collapsed;
            //}
        }

        /// <summary>
        /// Moves the caret to the specified position
        /// </summary>
        /// <param name="p"></param>
        internal void MoveCaretToPosition(Point p)
        {
            Canvas.SetLeft(this, p.X);
            Canvas.SetTop(this, p.Y);
            UpdateCursorPosition(p);
        }

        /// <summary>
        /// Gets After text from index
        /// </summary>
        /// <returns></returns>
        internal string GetAfterTextFromIndex()
        {
            string afterText = string.Empty;
            SpanAdv span = CurrentInline as SpanAdv;
            string text = span != null ? span.Text : (CurrentInline as HyperlinkAdv).Text;
            List<string> words = SpanAdv.SplitInToWord(text);
            double sum = 0;
            foreach (string word in words)
            {
                if (sum + word.Length >= CaretSpanIndex)
                {
                    int length = Convert.ToInt32(CaretSpanIndex - sum);
                    afterText = word.Substring(0, length);
                    break;
                }

                sum = sum + word.Length;
            }

            return afterText;
        }

        /// <summary>
        /// Updates the current paragraph and line information
        /// </summary>
        internal void UpdateParagraphAndLineInfo()
        {
            if (CurrentElement != null)
            {
                if (!(CurrentElement is TableCellElementBox) && !(CurrentElement is ChildTableCellElementBox))
                {
                    //SpanAdv span = CurrentElement.Inline as SpanAdv;
                    Page.OwnerControl.PositionHandler.TextPosition.Paragraph = CurrentElement.Inline.Paragraph;
                    Page.CurrentParagraph = CurrentElement.Inline.Paragraph;
                    Page.OwnerControl.PositionHandler.Line = Page.CurrentLine;
                }
            }
            TextPosition position = Page.OwnerControl.PositionHandler.TextPosition;
            if (position.Paragraph.IsInsideTable)
            {
                if (position.Paragraph.AssociatedCell != null)
                {
                    Page.CurrentLine = GetLineInfoFromPoint(position.Paragraph.AssociatedCell.Blocks, new Point(Location.X, Location.Y + 2));
                }
            }
            else
            {
                Page.CurrentLine = GetLineInfoFromPoint(new Point(Location.X, Location.Y + 2));
            }
            if (Page.CurrentLine != null)
            {
                if (Page.CurrentLine.Block is TableAdv)
                {
                    Page.CurrentParagraph = Page.OwnerControl.PositionHandler.TextPosition.Paragraph;
                }
                else
                {
                    Page.CurrentParagraph = Page.CurrentLine.Block as ParagraphAdv;
                }
            }
        }

        /// <summary>
        /// Returns the line information from point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal LineInfo GetLineInfoFromPoint(Point point)
        {
            LineInfo lineInfo = null;
            point = new Point(Math.Floor(point.X), point.Y);

            if (Page != null)
            {
                foreach (LineInfo line in Page.LineInfos)
                {
                    if (line.BoundingRectangle.Contains(point) || (point.Y > line.BoundingRectangle.Top && point.Y < line.BoundingRectangle.Bottom))
                    {
                        lineInfo = line;
                        break;
                    }
                }

                if (lineInfo == null && Page.LineInfos.Count > 0)
                {
                    if (point.Y >= Page.LineInfos.Last().BoundingRectangle.Top)
                    {
                        lineInfo = Page.LineInfos.Last();
                    }
                    else if (point.Y <= Page.LineInfos.First().BoundingRectangle.Top)
                    {
                        lineInfo = Page.LineInfos.First();
                    }
                }
            }

            return lineInfo;

            //return GetLine(0, Page.LineInfos.Count - 1, point);
        }

        internal LineInfo GetLineInfoFromPoint(BlockCollection<BlockAdv> blocks, Point point)
        {
            LineInfo lineInfo = null;
            point = new Point(Math.Floor(point.X), point.Y);

            foreach (LineInfo line in GetLineInfosFromBlocks(blocks))
            {
                if (line.BoundingRectangle.Contains(point) || (point.Y > line.BoundingRectangle.Top && point.Y < line.BoundingRectangle.Bottom))
                {
                    lineInfo = line;
                    break;
                }
            }

            if (lineInfo == null && GetLineInfosFromBlocks(blocks).Count > 0)
            {
                if (point.Y >= GetLineInfosFromBlocks(blocks).Last().BoundingRectangle.Top)
                {
                    lineInfo = GetLineInfosFromBlocks(blocks).Last();
                }
                else if (point.Y <= GetLineInfosFromBlocks(blocks).First().BoundingRectangle.Top)
                {
                    lineInfo = GetLineInfosFromBlocks(blocks).First();
                }
            }

            return lineInfo;
        }

        internal List<LineInfo> GetLineInfosFromBlocks(BlockCollection<BlockAdv> l_blocks)
        {
            List<LineInfo> collection = new List<LineInfo>();
            foreach (BlockAdv b in l_blocks)
            {
                collection.AddRange(b.LineInfo);
            }
            return collection;
        }

        internal LineInfo GetLineInfoFromPointInsideCell(ElementBox box, Point p)
        {
            LineInfo lineInfo = null;
            p = new Point(Math.Floor(p.X), p.Y);

            TableCellElementBox tablecellbox = box as TableCellElementBox;

            //foreach (LineInfo l in tablecellbox.LineInfos)
            //{
            //    if (l.BoundingRectangle.Contains(p) || (p.Y > l.BoundingRectangle.Top && p.Y < l.BoundingRectangle.Bottom))
            //    {
            //        lineInfo = l;
            //        break;
            //    }
            //}

            GetInternalBlocks(tablecellbox.CellBlocks, ref lineInfo, p);

            if (lineInfo == null && tablecellbox.LineInfos.Count > 0)
            {
                if (p.Y >= tablecellbox.LineInfos.Last().BoundingRectangle.Top)
                {
                    lineInfo = tablecellbox.LineInfos.Last();
                }
                else if (p.Y <= tablecellbox.LineInfos.First().BoundingRectangle.Top)
                {
                    lineInfo = tablecellbox.LineInfos.First();
                }
            }

            return lineInfo;
        }

        internal void GetInternalBlocks(BlockCollection<BlockAdv> blks, ref LineInfo lineInfo, Point point)
        {
            foreach (BlockAdv b in blks)
            {
                if (b is ParagraphAdv)
                {
                    foreach (LineInfo line in b.LineInfo)
                    {
                        if (line.BoundingRectangle.Contains(point) || (point.Y > line.BoundingRectangle.Top && point.Y < line.BoundingRectangle.Bottom))
                        {
                            lineInfo = line;
                            break;
                        }
                    }
                }
                else if (b is TableAdv)
                {
                    foreach (LineInfo line in b.LineInfo)
                    {
                        if (line.BoundingRectangle.Contains(point))
                        {
                            foreach (ElementBox box in line.ElementBoxes)
                            {
                                if (box is ChildTableCellElementBox)
                                {
                                    GetInternalBlocks(((box as ChildTableCellElementBox).GetParentCellBox() as TableCellElementBox).CellBlocks, ref lineInfo, point);
                                }
                                else if (box is TableCellElementBox)
                                {
                                    GetInternalBlocks((box as TableCellElementBox).CellBlocks, ref lineInfo, point);
                                }
                            }
                        }
                    }
                }
            }
        }



        internal LineInfo GetLine(int start, int end, Point point)
        {
            if (Page.LineInfos.Count != 0)
            {
                if (end != -1 && start != end)
                {
                    if (Page.LineInfos[start].BoundingRectangle.Top <= point.Y && Page.LineInfos[end].BoundingRectangle.Bottom >= point.Y)
                    {
                        int noOfLines = end - start;
                        int half = (int)Math.Round((double)(noOfLines / 2));
                        LineInfo line = GetLine(start, start + half, point);
                        if (line == null)
                        {
                            line = GetLine(start + half + 1, end, point);
                        }

                        return line;
                    }
                }
                else if (start != -1)
                {
                    if (Page.LineInfos[start].BoundingRectangle.Contains(point) || (point.Y > Page.LineInfos[start].BoundingRectangle.Top && point.Y < Page.LineInfos[start].BoundingRectangle.Bottom))
                    {
                        return Page.LineInfos[start];
                    }
                }
            }

            return null;
        }

        internal void UpdateCursorPosition(Point cursorPoint)
        {
            UpdateCursorPosition(GetLineInfoFromPoint(cursorPoint), cursorPoint);
        }

        /// <summary>
        /// Updates the current position of the cursor to the specified point
        /// </summary>
        internal void UpdateCursorPosition(LineInfo lineInfo, Point cursorPoint)
        {
            //LineInfo lineInfo = GetLineInfoFromPoint(cursorPoint);
            if (lineInfo != null)
            {
                string text = string.Empty;
                ElementBox elementBox = lineInfo.GetElementBoxFromPoint(cursorPoint);
                if (elementBox != null)
                {
                    TextElementBox textElement = elementBox as TextElementBox;
                    HyperlinkElementBox hyperlink = elementBox as HyperlinkElementBox;
                    TableCellElementBox cellElement = elementBox as TableCellElementBox;
                    ChildTableCellElementBox childelement = elementBox as ChildTableCellElementBox;
                    if (textElement != null)
                    {
                        CurrentElement = textElement;
                        double width = cursorPoint.X - textElement.ElementLocation.X;
                        text = GetText(textElement.Text, width);
                        Location = new Point(textElement.ElementLocation.X + TextHelper.MeasureText(text, textElement).Width, textElement.ElementLocation.Y);
                        AfterText = text;
                        CaretSpanIndex = GetSpanPositionToInsertText(elementBox.Inline as SpanAdv, elementBox, AfterText);
                        CurrentInline = elementBox.Inline;
                        if (CaretSpanIndex == 0)
                        {
                            //AdjustZeroIndex();
                        }
                    }
                    else if (hyperlink != null)
                    {
                        CurrentElement = hyperlink;
                        double width = cursorPoint.X - hyperlink.ElementLocation.X;
                        text = GetText(hyperlink.Text, width);
                        Location = new Point(hyperlink.ElementLocation.X + TextHelper.MeasureText(text, hyperlink).Width, hyperlink.ElementLocation.Y);
                        AfterText = text;
                        CaretSpanIndex = GetSpanPositionToInsertText(elementBox.Inline as HyperlinkAdv, elementBox, AfterText);
                        CurrentInline = elementBox.Inline;
                        if (CaretSpanIndex == 0)
                        {
                            //AdjustZeroIndex();
                        }
                    }
                    else if (childelement != null)
                    {
                        LineInfo ln = (childelement as ChildTableCellElementBox).GetLineInfoFromPoint(cursorPoint);
                        UpdateCursorPosition(ln, cursorPoint);
                        return;
                    }
                    else if (cellElement != null)
                    {
                        LineInfo line = cellElement.GetLineFromPoint(cursorPoint);
                        if (line == null && cellElement.BaseCell.HasRowSpan())
                        {
                            UpdateCursorPosition(cellElement.BaseCell.LineInfos.Last(), cursorPoint);
                        }
                        else
                        {
                            UpdateCursorPosition(line, cursorPoint);
                        }
                        return;
                    }
                    else
                    {
                        double x = 0;
                        if (Page.OwnerControl.Selection.StartPoint.X > cursorPoint.X)
                        {
                            x = elementBox.ElementLocation.X;
                        }
                        else
                        {
                            x = elementBox.ElementLocation.X + elementBox.ElementSize.Width;
                        }
                        if (elementBox.IsImageBox || elementBox.IsUIBox)
                        {
                            CaretSpanIndex = 1;
                            if (cursorPoint.X == elementBox.ElementLocation.X)
                            {
                                x = elementBox.ElementLocation.X;
                                CaretSpanIndex = 0;
                            }
                        }

                        CurrentElement = elementBox;
                        Canvas.SetLeft(this, x);
                        Location = new Point(x, Location.Y);
                        AfterText = string.Empty;

                        CurrentInline = elementBox.Inline;
                    }
                    UpdateParagraphAndLineInfo();
                    if (Page.OwnerControl.CurrentParagraph != null)
                    {
                        Page.OwnerControl.PositionHandler.TextPosition.SetIndex(Page.OwnerControl.CurrentParagraph.GetIndexFromInlineAndSpanIndex(CurrentInline, CaretSpanIndex).ToString());
                        if (Page.OwnerControl.CurrentParagraph != null && Page.OwnerControl.CurrentParagraph.Inlines.Count > 0
                            && Page.OwnerControl.CurrentParagraph.Inlines.Last() == CurrentInline)
                        {
                            Page.OwnerControl.PositionHandler.TextPosition.IsPositionAtParagraphEnd = CurrentInline.GetLength() == CaretSpanIndex;
                        }
                    }

                    Page.OwnerControl.PositionHandler.TextPosition.Point = cursorPoint;
                    Page.OwnerControl.PositionHandler.UpdateCurrentInlineStyle(CurrentElement);
                    Page.OwnerControl.PositionHandler.Caret = this;
                }
                else if (lineInfo.Block !=null && lineInfo.Block.Inlines != null && lineInfo.Block.Inlines.Count == 0)
                {
                    CurrentElement = null;
                    CurrentInline = null;
                    Location = new Point(lineInfo.Location.X, lineInfo.Location.Y);
                    Page.OwnerControl.PositionHandler.TextPosition.Paragraph = lineInfo.Block as ParagraphAdv;
                    Page.OwnerControl.PositionHandler.TextPosition.SetIndex("0");
                    Page.OwnerControl.PositionHandler.TextPosition.Point = cursorPoint;
                    Page.OwnerControl.PositionHandler.Caret = this;
                    Page.OwnerControl.PositionHandler.SetCurrentInlineStyle();
                    UpdateParagraphAndLineInfo();
                }

                Page.OwnerControl.PositionHandler.TextPosition.LineInfo = lineInfo;
                if (lineInfo != null)
                {
                    Page.OwnerControl.Viewer.BringLineToView(lineInfo);
                    Page.OwnerControl.PositionHandler.UpdateParagraphStyle(lineInfo.Block as ParagraphAdv);
                }
                if (!Page.Viewer.UseUpDownSelection)
                {
                    Page.Viewer.UseUpDownSelection = true;
                    Page.Viewer.UpDownSelectionWidth = Location.X;
                }
            }

            if (Page != null && Page.OwnerControl != null && Page.OwnerControl.Viewer != null && !Page.OwnerControl.Viewer.IsSelected)
            {
                Page.OwnerControl.Selection.Text = string.Empty;
            }
#if !WPF
            Page.OwnerControl.CanExecuteInsertDeleteTableCommands();
            Page.OwnerControl.CanExecuteSelectCommands();
#endif
            UpdateSize();
            Page.OwnerControl.Viewer.UpdateCurrentPageNumber();
        }

        internal void AdjustZeroIndex()
        {
            int index = CurrentInline.Paragraph.Inlines.IndexOf(CurrentInline);
            if (index != 0)
            {
                CurrentInline = CurrentInline.Paragraph.Inlines[index - 1];
                if (CurrentInline is HyperlinkAdv)
                {
                    HyperlinkAdv hyperlink = CurrentInline as HyperlinkAdv;
                    CaretSpanIndex = hyperlink.Text.Length;
                    if (hyperlink.ElementBoxes.Count > 0)
                    {
                        CurrentElement = hyperlink.ElementBoxes.Last();
                    }
                    AfterText = CurrentElement.InternalText;
                }
                else if (CurrentInline is SpanAdv)
                {
                    SpanAdv span = CurrentInline as SpanAdv;
                    CaretSpanIndex = span.Text.Length;
                    if (span.ElementBoxes.Count > 0)
                    {
                        CurrentElement = span.ElementBoxes.Last();
                    }
                    AfterText = CurrentElement.InternalText;
                }
                else
                {
                    CurrentElement = CurrentInline.ElementBoxes[0];
                    AfterText = string.Empty;
                    CaretSpanIndex = 1;
                }
            }
            else
            {
                CaretSpanIndex = 0;
            }
        }

        /// <summary>
        /// Returns the string using width
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public string GetText(string Text, double width)
        {
            int i = 0;
            string text = string.Empty;
            while (i < Text.Length)
            {
                text = Text.Substring(0, i + 1);
                if (Math.Floor(TextHelper.MeasureText(text, CurrentElement).Width) >= Math.Floor(width))
                {
                    if (Math.Floor(TextHelper.MeasureText(text, CurrentElement).Width) > Math.Floor(width))
                    {
                        text = Text.Substring(0, i);
                    }
                    break;
                }
                i++;
            }
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="span"></param>
        /// <param name="textBlock"></param>
        /// <param name="text"></param>
        internal double GetSpanPositionToInsertText(SpanAdv span, ElementBox elementBox, string text)
        {
            double index = 0;
            double wordSum = 0;
            List<string> words = SpanAdv.SplitInToWord(span.Text);

            //if (words.Count == span.ElementBoxes.Count)
            //{
            double pos = span.ElementBoxes.IndexOf(elementBox);
            wordSum = SpanAdv.GetSumOfTheWords(span.ElementBoxes, pos);
            //}

            index = wordSum + text.Length;

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="span"></param>
        /// <param name="textBlock"></param>
        /// <param name="text"></param>
        internal double GetSpanPositionToInsertText(HyperlinkAdv hyperlink, ElementBox elementBox, string text)
        {
            double index = 0;
            double wordSum = 0;
            List<string> words = SpanAdv.SplitInToWord(hyperlink.Text);

            //if (words.Count == hyperlink.ElementBoxes.Count)
            //{
            double pos = hyperlink.ElementBoxes.IndexOf(elementBox);
            wordSum = SpanAdv.GetSumOfTheWords(hyperlink.ElementBoxes, pos);
            //}

            index = wordSum + text.Length;

            return index;
        }

        /// <summary>
        /// Returns the text block using the index
        /// </summary>
        /// <param name="span"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal ElementBox GetTextElementUsingSpanIndex(Inline inline, double index)
        {
            string text = string.Empty;
            text = (inline is SpanAdv) ? (inline as SpanAdv).Text : (inline is HyperlinkAdv) ? (inline as HyperlinkAdv).Text : string.Empty;
            List<string> Words = SpanAdv.SplitInToWord(text);
            double sum = 0;
            int i = 0;
            foreach (string word in Words)
            {
                sum = sum + word.Length;
                if (sum >= index)
                {
                    return inline.ElementBoxes[i];
                }

                i++;
            }

            return null;
        }

        /// <summary>
        /// Releases the resources
        /// </summary>
        internal void ReleaseResources()
        {
            storyBoard.Stop();
            animation.KeyFrames.Clear();
            animation = null;
            storyBoard.Children.Clear();
            storyBoard = null;
            Page = null;
        }

        /// <summary>
        /// Updates the size of the caret
        /// </summary>
        internal void UpdateSize()
        {
            if (CurrentElement != null)
            {
                Height = CurrentElement.ElementSize.Height;
                if (CurrentElement != null && (CurrentElement is TextElementBox || CurrentElement.IsImageBox || CurrentElement.IsUIBox))
                {
                    double top = CurrentElement.ElementLocation.Y;
                    if (CurrentElement.IsImageBox || CurrentElement.IsUIBox)
                    {
                        Size size = TextHelper.MeasureText(" ", Page.OwnerControl.CurrentInlineStyle);
                        top = (Height - size.Height) > 0 ? top + (Height - size.Height) : top;
                        Height = size.Height;
                    }
                    //Canvas.SetTop(this, Canvas.GetTop((CurrentElement as TextElementBox).textBlock));
                    Canvas.SetTop(this, top);
                }

                if (CurrentInline is SpanAdv || CurrentInline is HyperlinkAdv)
                {
                    bool flag = (CurrentInline as SpanAdv) != null ? (CurrentInline as SpanAdv).FontStyle == FontStyles.Italic : false;
                    if (flag)
                    {
                        RotateTransform transform = new RotateTransform();
                        transform.Angle = 12;
                        transform.CenterX = 0;
                        transform.CenterY = ActualHeight / 2;
                        RenderTransform = transform;
                    }
                    else
                    {
                        RotateTransform transform = new RotateTransform();
                        transform.Angle = 0;
                        RenderTransform = transform;
                    }
                }

                if (Page.Viewer.isDragging)
                {
                    Width = 2;
                    if (rectangle != null)
                        rectangle.Opacity = 1;
                    storyBoard.Pause();
                }
                else
                {
                    Width = 1.5;
                    storyBoard.Resume();
                }

                if(!Page.OwnerControl.IsReadOnly || Page.OwnerControl.EnableCursorOnReadOnly)  
                    Show();
            }
            else
            {
                Size size = TextHelper.MeasureText(" ", Page.OwnerControl.CurrentInlineStyle);
                Height = size.Height;
                if (Page.CurrentLine != null && Page.CurrentParagraph !=null &&
                    Page.CurrentParagraph.IsEmpty && Page.CurrentParagraph.ListType!=ListType.None)
                {
                    Canvas.SetTop(this, Page.CurrentLine.BoundingRectangle.Top + 4);
                }
                else if(Page.CurrentLine !=null)
                {
                    Canvas.SetTop(this, Page.CurrentLine.BoundingRectangle.Top);
                }
            }
        }

        internal string GetAfterTextFromIndex(int p)
        {
            //throw new NotImplementedException();
            return string.Empty;
        }

        internal void ResumeAnimation()
        {
            storyBoard.Resume();
        }

        #endregion
    }
}
