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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    public class PageAdv : ContentControl
    {
        #region fields

        private ParagraphAdv currentparagraph;
        private LineInfo currentLine;
        internal List<LineInfo> SelectedLines;
        private Border pageBorder = null;
        private Border effectBorder = null;
        private bool pageselect = false;
        private bool parentboxselect = false;
        private SectionAdv section = null;
        private ObservableCollection<LineInfo> lines;
        private RichTextBoxAdv richtext;

        /// <summary>
        /// Gets or Sets the foreground container
        /// </summary>
        internal Canvas ForegroundContainer;

        /// <summary>
        /// Gets or Sets the selection container
        /// </summary>
        internal Canvas SelectionContainer;

        /// <summary>
        /// Gets or Sets the decoration container
        /// </summary>
        internal Canvas DecorationContainer;

        /// <summary>
        /// Gets or Sets the viewer
        /// </summary>
        internal LayoutViewer Viewer;

        /// <summary>
        /// 
        /// </summary>
        internal Canvas Container;

        private CaretAdv caret;
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        private Ellipse touchStart;
        private Ellipse touchEnd;
        internal Point touchStartPoint = new Point();
        internal Point touchEndPoint = new Point();

        /// <summary>
        /// Gets or sets the touch start mark ellipse.
        /// </summary>
        /// <value>The touch start.</value>
        internal Ellipse TouchStart
        {
            get
            {
                return touchStart;
            }
            set
            {
                touchStart = value;
            }
        }
        /// <summary>
        /// Gets or sets the touch end mark ellipse.
        /// </summary>
        /// <value>The touch end.</value>
        internal Ellipse TouchEnd
        {
            get
            {
                return touchEnd;
            }
            set
            {
                touchEnd = value;
            }
        }
#endif
        /// <summary>
        /// Gets the Caret instance.
        /// </summary>
        public CaretAdv Caret
        {
            get
            {
                return caret;
            }
            internal set
            {
                caret = value;
            }
        }

        /// <summary>
        /// Gets or sets the word is selected
        /// </summary>
        internal bool IsSelected
        {
            get
            {
                if (SelectionContainer.Children.Contains(SelectionPath) || HasSelectedImage())
                    return true;
                if (SelectionContainer == null || SelectionPath == null || HasSelectedImage())
                    return false;
                return false;
            }
            set
            {
                if (!value && IsSelected)
                {
                    SelectionContainer.Children.Remove(SelectionPath);
                    SelectionPath = null;
                    foreach (LineInfo line in SelectedLines)
                    {
                        if (line.IsTableLine)
                        {
                            line.ElementBoxes.ForEach(e => (e as TableCellElementBox).IsFullCellSelected = false);
                        }
                    }
                    SelectedLines.Clear();
                }
                if (value && !IsSelected && SelectionContainer != null && SelectionPath != null)
                {
                    Canvas.SetLeft(SelectionPath, 0);
                    Canvas.SetTop(SelectionPath, 0);
                    SelectionContainer.Children.Add(SelectionPath);
                }
#if !WPF
                OwnerControl.CanExecuteCommands();
#endif
            }
        }

        private bool HasSelectedImage()
        {
            if (CurrentParagraph != null)
            {
                if (CurrentParagraph.LayoutViewer != null)
                {
                    if (CurrentParagraph.LayoutViewer.SelectedImage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
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
            }

        }

        /// <summary>
        /// Gets or Sets the current paragraph
        /// </summary>
        internal ParagraphAdv CurrentParagraph
        {
            get
            {
                return currentparagraph;
            }
            set
            {
                currentparagraph = value;
                OwnerControl.CurrentParagraph = value;
                Viewer.CurrentParagraph = value;
            }
        }

        /// <summary>
        /// Gets or Sets the current line
        /// </summary>
        internal LineInfo CurrentLine
        {
            get
            {
                return currentLine;
            }
            set
            {
                currentLine = value;
                Viewer.CurrentLine = value;
            }
        }

        /// <summary>
        /// Gets or Sets the selection path
        /// </summary>
        internal Path SelectionPath;

        private Rect boundingRectangle;

        #endregion

        #region public fields

        /// <summary>
        /// Gets or Sets the owner control
        /// </summary>
        public RichTextBoxAdv OwnerControl
        {
            get
            {
                return richtext;
            }
            set
            {
                richtext = value;
            }
        }

        internal Rect BoundingRectangle
        {
            get
            {
                return boundingRectangle;
            }
            set
            {
                boundingRectangle = value;
                if (pageBorder != null)
                    pageBorder.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, value.Width, value.Height) };
            }
        }

        private double containerWidth = 0;

        internal double ContainerWidth
        {
            get
            {
                return containerWidth;
            }
            set
            {
                containerWidth = value;
            }
        }

        private double containerHeight = 0;

        internal double ContainerHeight
        {
            get
            {
                return containerHeight;
            }
            set
            {
                containerHeight = value;
            }
        }

        /// <summary>
        /// Gets or Sets the padding
        /// </summary>
        public Thickness PageContentMargin
        {
            get;
            set;
        }



        internal Brush ShadowBackground
        {
            get { return (Brush)GetValue(ShadowBackgroundProperty); }
            set { SetValue(ShadowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageShadowBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShadowBackgroundProperty =
            DependencyProperty.Register("ShadowBackground", typeof(Brush), typeof(PageAdv), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));



        /// <summary>
        /// Gets or Sets the Line informations
        /// </summary>
        // internal List<LineInfo> LineInfo;

        public ObservableCollection<LineInfo> LineInfos
        {
            get
            {
                return lines;
            }
            internal set
            {
                lines = value;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// Initializes the new instance of RichEditPage class
        /// </summary>
        public PageAdv()
        {
            this.DefaultStyleKey = typeof(PageAdv);
#if WPF
            //Handled specifically to avoid focus, on Tab navigation. Sets PageAdv as not focusable control.
            this.Focusable = false;
#endif
            this.Container = new Canvas();
            this.ForegroundContainer = new Canvas();
            this.SelectionContainer = new Canvas();
            this.DecorationContainer = new Canvas();
            this.Content = Container;
            this.Caret = new CaretAdv();
            this.Caret.Page = this;
            this.Caret.Background = new SolidColorBrush(Colors.Black);
            this.Caret.Visibility = Visibility.Collapsed;
            this.ForegroundContainer.Children.Add(this.Caret);
            Canvas.SetZIndex(this.Caret, 4);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            //Initializes the touch selection start ellipse.
            this.TouchStart = new Ellipse();
            this.TouchStart.Height = 15;
            this.TouchStart.Width = 15;
            this.TouchStart.Visibility = Visibility.Collapsed;
            this.TouchStart.Stroke = Brushes.Black;
            this.TouchStart.StrokeThickness = 1;
            Canvas.SetZIndex(this.TouchStart, 5);
            this.ForegroundContainer.Children.Add(this.TouchStart);
            //Initializes the touch selection end ellipse.
            this.TouchEnd = new Ellipse();
            this.TouchEnd.Height = 15;
            this.TouchEnd.Width = 15;
            this.TouchEnd.Visibility = Visibility.Collapsed;
            this.TouchEnd.Stroke = Brushes.Black;
            this.TouchEnd.StrokeThickness = 1;
            Canvas.SetZIndex(this.TouchEnd, 6);
            this.ForegroundContainer.Children.Add(this.TouchEnd);
#endif
            Container.Children.Add(ForegroundContainer);
            Container.Children.Add(SelectionContainer);
            Container.Children.Add(DecorationContainer);
            Canvas.SetZIndex(ForegroundContainer, 2);
            Canvas.SetZIndex(DecorationContainer, 1);
            Canvas.SetZIndex(SelectionContainer, 3);
            SelectedLines = new List<LineInfo>();
            LineInfos = new ObservableCollection<LineInfo>();
            LineInfos.CollectionChanged += new NotifyCollectionChangedEventHandler(LineInfos_CollectionChanged);
            HideCaret(false);
        }

        #endregion

        #region methods

        /// <summary>
        /// Applies the template
        /// </summary>
        public override void OnApplyTemplate()
        {
            pageBorder = (Border)GetTemplateChild("PART_border");
            effectBorder = (Border)GetTemplateChild("PART_shadowEfffect");
            if (decreaseBorder)
            {
                DecreaseThickness();
            }
            else
            {
                IncreaseThickness();
            }
            base.OnApplyTemplate();
        }

        private bool decreaseBorder = false;

        internal void DecreaseThickness()
        {
            decreaseBorder = true;
            if (pageBorder != null)
            {
                pageBorder.BorderThickness = new Thickness(0);
            }
        }

        internal void IncreaseThickness()
        {
            decreaseBorder = false;
            if (pageBorder != null)
            {
                pageBorder.BorderThickness = new Thickness(1);
            }
        }

        /// <summary>
        /// Draws the selection to the page
        /// </summary>
        internal void AddSelectionPath()
        {
            if (SelectedLines.Count > 0)
            {
                if (SelectionPath != null && SelectionContainer.Children.Contains(SelectionPath))
                {
                    SelectionContainer.Children.Remove(SelectionPath);
                }
                if (SelectedLines != null && SelectedLines.Count > 0)
                {
                    SelectionPath = OwnerControl.Selection.GetSelectionPathForLineInfo(SelectedLines);
                }
                IsSelected = true;
            }
        }

        /// <summary>
        /// Returns index from point
        /// </summary>
        /// <param name="point"></param>
        internal double GetIndexFromPoint(Point point)
        {
            LineInfo lineInfo = Caret.GetLineInfoFromPoint(point);
            double index = 0;
            if (lineInfo != null)
            {
                ElementBox elementBox = lineInfo.GetElementBoxFromPoint(point);
                if (elementBox is TextElementBox)
                {
                    TextElementBox textElement = elementBox as TextElementBox;
                    //TextBlock textBlock = textElement.textBlock;
                    //double width = point.X - Canvas.GetLeft(textBlock);
                    double width = point.X - elementBox.ElementLocation.X;
                    int i = 0;
                    string text = string.Empty;

                    while (i < textElement.Text.Length)
                    {
                        text = textElement.Text.Substring(0, i + 1);
                        if (Math.Floor(TextHelper.MeasureText(text, textElement).Width) >= Math.Floor(width))
                        {
                            if (Math.Floor(TextHelper.MeasureText(text, textElement).Width) > Math.Floor(width))
                            {
                                text = textElement.Text.Substring(0, i);
                            }
                            break;
                        }
                        i++;
                    }
                    index = Caret.GetSpanPositionToInsertText(textElement.Inline as SpanAdv, textElement, text);
                }
            }

            return index;
        }

        /// <summary>
        /// Returns inline from point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal Inline GetInlineFromPoint(Point point)
        {
            LineInfo line = Caret.GetLineInfoFromPoint(point);
            ElementBox elementBox = null;
            Inline inline = null;
            if (line != null)
            {
                elementBox = line.GetElementBoxFromPoint(point);
                if (elementBox != null)
                {
                    inline = elementBox.Inline;
                }
            }

            return inline;
        }

        internal void ResumeAnimation()
        {
            Caret.ResumeAnimation();
        }

        internal ElementBox GetElementBox(LineInfo line, Point point)
        {
            if(line==null)
                return null;
            return line.GetElementBoxFromPoint(point);
        }

        internal ElementBox GetElementBox(Point cursorPoint)
        {
            LineInfo line = Caret.GetLineInfoFromPoint(cursorPoint);
            if (line != null)
                return GetElementBox(line, cursorPoint);

            return null;
        }

        internal void UpdateCaretPosition(MouseEventArgs e)
        {
            Point cursorPoint = e.GetPosition(this.ForegroundContainer);
            Caret.UpdateCursorPosition(cursorPoint);
        }

        internal void UpdateCaretPosition(Point point)
        {
            OwnerControl.Viewer.CurrentPage = this;
            Caret.UpdateCursorPosition(point);
        }

        /// <summary>
        /// Handles got focus event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            //ShowCaret();
            if (Viewer != null)
            {
                Viewer.CheckForCursorVisibility(false);
            }
        }

        /// <summary>
        /// Handles the lost focus event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            HideCaret(false);
        }

        /// <summary>
        /// Generates lines for selection`
        /// </summary>
        /// <returns></returns>
        internal List<LineInfo> GenerateLineInfoForSelection()
        {
            List<LineInfo> lineInfo = new List<LineInfo>();
            bool islinestarted = false;
            PathGeometry pathgeo = new PathGeometry();
            int index = Viewer.Pages.IndexOf(this);
            List<ElementBox> SelectedBoxes = new List<ElementBox>();

            Point startPoint = OwnerControl.Selection.StartPoint;
            Point endPoint = OwnerControl.Selection.EndPoint;

            LineInfo line1 = null;
            LineInfo line2 = null;
            TextPosition startPos = OwnerControl.Selection.Start;
            TextPosition endPos = OwnerControl.Selection.End;
            OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(startPos,endPos);

            List<LineInfo> LocalLineInfos = new List<LineInfo>();
            TableCellAdv startcell = null;
            TableCellAdv endcell = null;
            TableCellAdv AssociatedCell = null;
            SelectionAdv.SetStartAndEndCells(startPos, endPos, ref startcell, ref endcell);

            if (startcell == null || endcell == null)
            {
                if ((startcell == null && startPos !=null && startPos.Paragraph.AssociatedCell != null) || (endcell == null && endPos !=null && endPos.Paragraph.AssociatedCell != null))
                {
                    if (startcell == null && endcell == null)
                    {
                        if (startPos != null && endPos != null)
                        {
                            if (startPos.Paragraph.AssociatedCell == endPos.Paragraph.AssociatedCell)
                            {
                                AssociatedCell = startPos.Paragraph.AssociatedCell;
                            }
                            else
                            {
                                TableAdv owner = startPos.Paragraph.AssociatedCell.OwnerTable;
                                if (owner.AssociatedCell != null)
                                {
                                    AssociatedCell = owner.AssociatedCell;
                                }
                            }
                        }
                    }
                    else if (startcell == null)
                    {
                        AssociatedCell = startPos.Paragraph.AssociatedCell;
                    }
                    else if (endcell == null)
                    {
                        AssociatedCell = endPos.Paragraph.AssociatedCell;
                    }
                }
            }
            else if (startcell != null && endcell != null)
            {
                TableAdv startTable = startcell.OwnerTable;
                TableAdv endTable = endcell.OwnerTable;

                if (startTable.AssociatedCell != null && endTable.AssociatedCell != null)
                {
                    AssociatedCell = startTable.AssociatedCell;
                }
            }

            if (AssociatedCell != null)
            {
                LocalLineInfos = AssociatedCell.LineInfos.ToList();
            }
            else
            {
                LocalLineInfos = LineInfos.ToList();
            }

            List<LineInfo> templineinfos = GetBasedOngivenPageIndex(LocalLineInfos, index);

            TableCellAdv endc = null;
            TableRowAdv endr=null;

            BlockAdv endblk= OwnerControl.Document.GetBlockFromVirtualPosition(endPos.VirtualPosition, ref endr, ref endc);
            int endindex=0;
            if(endc !=null)
             endindex= endr.Cells.IndexOf(endc);

            TableCellAdv startc = null;
            TableRowAdv startr = null;
            BlockAdv startblk = null;

            if (startPos != null)
            {
                startblk = OwnerControl.Document.GetBlockFromVirtualPosition(startPos.VirtualPosition, ref startr, ref startc);
            }
            int startindex = 0;
            if(startc !=null)    
                startindex=startr.Cells.IndexOf(startc);
            
            if (OwnerControl.Selection.StartingPage == index && OwnerControl.Selection.EndingPage < index)
            {
                if (templineinfos.Count > 0)
                {
                    if (!templineinfos.First().IsTableLine)
                    {
                        endPoint = new Point(templineinfos.First().BoundingRectangle.Left, templineinfos.First().BoundingRectangle.Top + templineinfos.First().BoundingRectangle.Height / 2);
                    }
                    else
                    {
                        endPoint = new Point(templineinfos.First().ElementBoxes[endindex].BoundingRectangle.Right -1, templineinfos.First().BoundingRectangle.Top + templineinfos.First().BoundingRectangle.Height / 2);
                    }
                }
            }
            else if (OwnerControl.Selection.StartingPage == index && OwnerControl.Selection.EndingPage > index)
            {
                if (templineinfos.Count > 0)
                {
                    if (!templineinfos.Last().IsTableLine)
                    {
                        endPoint = new Point(templineinfos.Last().BoundingRectangle.Right, templineinfos.Last().BoundingRectangle.Top + templineinfos.Last().BoundingRectangle.Height / 2);
                    }
                    else
                    {
                        endPoint = new Point(templineinfos.Last().ElementBoxes[endindex].BoundingRectangle.Right - 1, templineinfos.Last().BoundingRectangle.Top + templineinfos.Last().BoundingRectangle.Height / 2);
                    }
                }
            }
            else if (OwnerControl.Selection.StartingPage > index && OwnerControl.Selection.EndingPage == index)
            {
                if (templineinfos.Count > 0)
                {
                    if (!templineinfos.Last().IsTableLine)
                    {
                        startPoint = new Point(templineinfos.Last().BoundingRectangle.Right, templineinfos.Last().BoundingRectangle.Top + templineinfos.Last().BoundingRectangle.Height / 2);
                    }
                    else
                    {
                        startPoint = new Point(templineinfos.Last().ElementBoxes[startindex].BoundingRectangle.Right -1, templineinfos.Last().BoundingRectangle.Top + templineinfos.Last().BoundingRectangle.Height / 2);
                    }
                }
            }
            else if (OwnerControl.Selection.StartingPage < index && OwnerControl.Selection.EndingPage == index)
            {
                if (templineinfos.Count > 0)
                {
                    if (!templineinfos.First().IsTableLine)
                    {
                        startPoint = new Point(templineinfos.First().BoundingRectangle.Left, templineinfos.First().BoundingRectangle.Top + templineinfos.First().BoundingRectangle.Height / 2);
                    }
                    else
                    {
                        startPoint = new Point(templineinfos.First().ElementBoxes[startindex].BoundingRectangle.Left + 1, templineinfos.First().BoundingRectangle.Top + templineinfos.First().BoundingRectangle.Height / 2);
                    }
                }
            }
            else if ((OwnerControl.Selection.StartingPage < index && OwnerControl.Selection.EndingPage > index) || (OwnerControl.Selection.StartingPage > index && OwnerControl.Selection.EndingPage < index))
            {
                if (templineinfos.Count > 0)
                {
                    if (!templineinfos.First().IsTableLine)
                    {
                        startPoint = new Point(templineinfos.First().BoundingRectangle.Left, templineinfos.First().BoundingRectangle.Top + templineinfos.First().BoundingRectangle.Height / 2);
                    }
                    else
                    {
                        startPoint = new Point(templineinfos.First().ElementBoxes[startindex].BoundingRectangle.Left + 1, templineinfos.First().BoundingRectangle.Top + templineinfos.First().BoundingRectangle.Height / 2);
                    }
                    if (!templineinfos.Last().IsTableLine)
                    {
                        endPoint = new Point(templineinfos.Last().BoundingRectangle.Right, templineinfos.Last().BoundingRectangle.Top + templineinfos.Last().BoundingRectangle.Height / 2);
                    }
                    else
                    {
                        endPoint = new Point(templineinfos.Last().ElementBoxes[endindex].BoundingRectangle.Right - 1, templineinfos.Last().BoundingRectangle.Top + templineinfos.Last().BoundingRectangle.Height / 2);
                    }
                }
            }
            LocalLineInfos = templineinfos;

            if (AssociatedCell != null)
            {
                line1 = Caret.GetLineInfoFromPoint(AssociatedCell.Blocks, startPoint);
                line2 = Caret.GetLineInfoFromPoint(AssociatedCell.Blocks, endPoint);
            }
            else
            {
                line1 = Caret.GetLineInfoFromPoint(startPoint);
                line2 = Caret.GetLineInfoFromPoint(endPoint);
                LocalLineInfos = LineInfos.ToList();
            }

            LineInfo startingLine = new LineInfo(line1);
            LineInfo endingLine = new LineInfo(line2);

            if (line1 != null && line2 != null)
            {
                if (line1.IsTableLine && line2.IsTableLine && line1.Block == line2.Block)
                {
                    ElementBox startbox = GetElementBox(line1, startPoint);
                    ElementBox endbox = GetElementBox(line2, endPoint);

                    if (startbox != null && endbox != null)
                    {
                        if (startbox == endbox)
                        {
                            line1 = Caret.GetLineInfoFromPointInsideCell(startbox, startPoint);
                            line2 = Caret.GetLineInfoFromPointInsideCell(endbox, endPoint);

                            startingLine = new LineInfo(line1);
                            endingLine = new LineInfo(line2);

                            (startbox as TableCellElementBox).IsFullCellSelected = true;
                            line1 = startbox.LineInfo;

                            lineInfo.Add(startbox.LineInfo);
                        }
                        else if (startbox != endbox)
                        {
                            foreach (LineInfo line in SelectedLines)
                            {
                                line.ElementBoxes.ForEach(e => (e as TableCellElementBox).IsFullCellSelected = false);
                            }
                            if (startbox.BoundingRectangle.Y > endbox.BoundingRectangle.Y || (startbox.BoundingRectangle.Y == endbox.BoundingRectangle.Y && startbox.BoundingRectangle.X > endbox.BoundingRectangle.X))
                            {
                                ElementBox box = endbox;
                                endbox = startbox;
                                startbox = box;
                            }
                            double x1 = startbox.BoundingRectangle.X;
                            double x2 = endbox.BoundingRectangle.Right;

                            if (startbox.BoundingRectangle.X > endbox.BoundingRectangle.X)
                            {
                                x1 = endbox.BoundingRectangle.X;
                                x2 = startbox.BoundingRectangle.Right;
                            }

                            foreach (LineInfo line in LocalLineInfos)
                            {
                                if (line.ElementBoxes.Contains(startbox))
                                {
                                    islinestarted = true;
                                }
                                if (islinestarted)
                                {
                                    foreach (ElementBox b in line.ElementBoxes)
                                    {
                                        if (b.BoundingRectangle.X >= x1 && b.BoundingRectangle.Right <= x2)
                                        {
                                            if (b is TableCellElementBox)
                                            {
                                                (b as TableCellElementBox).IsFullCellSelected = true;
                                            }
                                        }
                                        else if ((b.BoundingRectangle.X == x1 && b.BoundingRectangle.Right >= x2 && (b as TableCellElementBox).BaseCell.HasColumnSpan()) || (b.BoundingRectangle.X <= x1 && b.BoundingRectangle.Right >= x2 && (b as TableCellElementBox).BaseCell.HasColumnSpan()) ||
                                            (b.BoundingRectangle.X <= x1 && b.BoundingRectangle.Right == x2 && (b as TableCellElementBox).BaseCell.HasColumnSpan()))
                                        {
                                            if (b is TableCellElementBox)
                                            {
                                                (b as TableCellElementBox).IsFullCellSelected = true;
                                            }
                                        }
                                    }

                                    if (line.ElementBoxes.All(e=>e as TableCellElementBox != null) && line.ElementBoxes.Where(e => (e as TableCellElementBox).IsFullCellSelected).Count<ElementBox>() > 0)
                                    {
                                        if (startblk != null && endblk != null)
                                        {
                                            if (startblk.IsParagraph || endblk.IsParagraph)
                                            {
                                                line.ElementBoxes.ForEach(e => (e as TableCellElementBox).IsFullCellSelected = true);
                                            }
                                        }
                                        lineInfo.Add(line);
                                    }
                                }
                                if (islinestarted && line.ElementBoxes.Contains(endbox))
                                {
                                    islinestarted = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (line1 == line2)
                    {
                        Point point = new Point();
                        Size size = Size.Empty;
                        if (startPoint.X > endPoint.X)
                        {
                            point = new Point(endPoint.X, startingLine.Location.Y);
                            size = new Size(startPoint.X - endPoint.X, startingLine.BoundingRectangle.Height);
                        }
                        else
                        {
                            point = new Point(startPoint.X, startingLine.Location.Y);
                            size = new Size(endPoint.X - startPoint.X, startingLine.BoundingRectangle.Height);
                        }
                        startingLine.BoundingRectangle = new Rect(point, size);
                        if (size.Width > 0)
                        {
                            lineInfo.Add(startingLine);
                        }
                    }
                    else if (LocalLineInfos.IndexOf(line1) < LocalLineInfos.IndexOf(line2))
                    {
                        Point point;
                        Size size = Size.Empty;
                        if (!line1.IsTableLine)
                        {
                            point = new Point(startPoint.X, startingLine.Location.Y);
                            if (startingLine.BoundingRectangle.Right < startPoint.X)
                            {
                                size = new Size(startPoint.X - startingLine.BoundingRectangle.Right, startingLine.BoundingRectangle.Height);
                            }
                            else
                            {
                                size = new Size(startingLine.BoundingRectangle.Right - startPoint.X, startingLine.BoundingRectangle.Height);
                            }
                            startingLine.BoundingRectangle = new Rect(point, size);
                        }
                        else
                        {
                            foreach (ElementBox elementbox in line1.ElementBoxes)
                            {
                                (elementbox as TableCellElementBox).IsFullCellSelected = true;
                            }
                        }
                        if (!line2.IsTableLine)
                        {
                            point = endingLine.Location;
                            if (endPoint.X > endingLine.Location.X)
                            {
                                size = new Size(endPoint.X - endingLine.Location.X, endingLine.BoundingRectangle.Height);
                            }
                            else
                            {
                                size = new Size(0, endingLine.BoundingRectangle.Height);
                            }
                            endingLine.BoundingRectangle = new Rect(point, size);
                        }
                        else
                        {
                            foreach (ElementBox box in line2.ElementBoxes)
                            {
                                (box as TableCellElementBox).IsFullCellSelected = true;
                            }
                        }
                        if (startingLine.BoundingRectangle.Width > 0)
                        {
                            lineInfo.Add(startingLine);
                        }
                        for (int i = LocalLineInfos.IndexOf(line1) + 1; i < LocalLineInfos.IndexOf(line2); i++)
                        {
                            if (LocalLineInfos[i].IsTableLine)
                                LocalLineInfos[i].ElementBoxes.ForEach(e => (e as TableCellElementBox).IsFullCellSelected = true);
                            LineInfo line = new LineInfo(LocalLineInfos[i]);
                            lineInfo.Add(line);
                        }

                        if (endingLine.BoundingRectangle.Width > 0)
                        {
                            lineInfo.Add(endingLine);
                        }
                    }
                    else if (LocalLineInfos.IndexOf(line1) > LocalLineInfos.IndexOf(line2))
                    {
                        Point point;
                        Size size = Size.Empty;
                        if (!line2.IsTableLine)
                        {
                            point = new Point(endPoint.X, endingLine.Location.Y);
                            if (endingLine.BoundingRectangle.Right > endPoint.X)
                            {
                                size = new Size(endingLine.BoundingRectangle.Right - endPoint.X, endingLine.BoundingRectangle.Height);
                            }
                            else
                            {
                                size = new Size(endingLine.BoundingRectangle.Right, endingLine.BoundingRectangle.Height);
                            }
                            endingLine.BoundingRectangle = new Rect(point, size);
                        }
                        else
                        {
                            foreach (ElementBox box in line2.ElementBoxes)
                            {
                                (box as TableCellElementBox).IsFullCellSelected = true;
                            }
                        }
                        if (!line1.IsTableLine)
                        {
                            point = startingLine.Location;
                            if (startPoint.X > startingLine.Location.X)
                            {
                                size = new Size(startPoint.X - startingLine.Location.X, startingLine.BoundingRectangle.Height);
                            }
                            else
                            {
                                size = new Size(0, startingLine.BoundingRectangle.Height);
                            }
                            startingLine.BoundingRectangle = new Rect(point, size);
                        }
                        else
                        {
                            foreach (ElementBox elementbox in line1.ElementBoxes)
                            {
                                (elementbox as TableCellElementBox).IsFullCellSelected = true;
                            }
                        }
                        if (endingLine.BoundingRectangle.Width > 0 && endingLine.PageIndex==index)
                        {
                            lineInfo.Add(endingLine);
                        }
                        for (int i = LocalLineInfos.IndexOf(line2) + 1; i < LocalLineInfos.IndexOf(line1); i++)
                        {
                            if (LocalLineInfos[i].IsTableLine)
                                LocalLineInfos[i].ElementBoxes.ForEach(e => (e as TableCellElementBox).IsFullCellSelected = true);
                            LineInfo line = new LineInfo(LocalLineInfos[i]);
                            lineInfo.Add(line);
                        }
                        if (startingLine.BoundingRectangle.Width > 0 && startingLine.PageIndex==index)
                        {
                            lineInfo.Add(startingLine);
                        }
                    }
                }
            }
            if (lineInfo.Count >0)
            {
                bool check = false;
                LineInfo given = null;
                check = lineInfo.Where(l => l.HasChildBoxes || l.IsSplitted).Count<LineInfo>() ==1;
                if (line1 !=null && (line1.HasChildBoxes || line1.IsSplitted))
                {
                    given = line1;
                }
                else if (line2 !=null && (line2.HasChildBoxes || line2.IsSplitted))
                {
                    given = line2;
                }
                if (check)
                {
                    if (given != null)
                    {
                        parentboxselect = given.HasChildBoxes && !given.IsSplitted;
                        pageselect = given.IsSplitted && !given.HasChildBoxes;
                        SelectSplittedLines(given);
                    }
                }
                else
                {
                    //foreach (PageAdv page in Viewer.Pages)
                    //{
                    //    if (Viewer.Pages.IndexOf(page) == index)
                    //        continue;
                    //    else
                    //        page.IsSelected = false;
                    //}
                }
            }
            
            return lineInfo;
        }

        internal void SelectSplittedLines(LineInfo line)
        {
            PageAdv page = null;
            bool isselected=false;

            foreach (ElementBox elementbox in line.ElementBoxes)
            {
                if (elementbox is ChildTableCellElementBox)
                {
                    ChildTableCellElementBox child = elementbox as ChildTableCellElementBox;
                    if (pageselect)
                    {
                        if (child.ChildBox != null)
                        {
                            page = Viewer.Pages[child.ChildBox.LineInfo.PageIndex];
                            //isselected=(child.ParentBox as TableCellElementBox).IsFullCellSelected;
                            isselected = page.IsSelected;
                            child.ChildBox.IsFullCellSelected = child.IsFullCellSelected;
                            if (line.ElementBoxes.IndexOf(elementbox) == line.ElementBoxes.Count - 1)
                            {
                                if (!page.SelectedLines.Contains(child.ChildBox.LineInfo) && !isselected)
                                {
                                    page.SelectedLines.Add(child.ChildBox.LineInfo);
                                    if (child.ChildBox.ChildBox != null)
                                    {
                                        SelectSplittedLines(child.ChildBox.LineInfo);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (child.ParentBox != null)
                        {
                            page = Viewer.Pages[child.ParentBox.LineInfo.PageIndex];
                            //isselected=(child.ParentBox as TableCellElementBox).IsFullCellSelected;
                            isselected = page.IsSelected;
                            (child.ParentBox as TableCellElementBox).IsFullCellSelected = child.IsFullCellSelected;
                            if (line.ElementBoxes.IndexOf(elementbox) == line.ElementBoxes.Count - 1)
                            {
                                if (!page.SelectedLines.Contains(child.ParentBox.LineInfo) && !isselected)
                                {
                                    page.SelectedLines.Add(child.ParentBox.LineInfo);
                                    if (child.ParentBox is ChildTableCellElementBox)
                                    {
                                        if ((child.ParentBox as ChildTableCellElementBox).ParentBox != null)
                                        {
                                            SelectSplittedLines((child.ParentBox as ChildTableCellElementBox).ParentBox.LineInfo);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                else if (elementbox is TableCellElementBox)
                {
                    TableCellElementBox tablebox = elementbox as TableCellElementBox;
                    if (parentboxselect)
                    {
                        if (tablebox != null)
                        {
                            page = Viewer.Pages[tablebox.LineInfo.PageIndex];
                            //isselected =tablebox.ChildBox.IsFullCellSelected;
                            isselected = page.IsSelected;
                            tablebox.IsFullCellSelected = tablebox.ChildBox.IsFullCellSelected;
                            if (line.ElementBoxes.IndexOf(elementbox) == line.ElementBoxes.Count - 1)
                            {
                                if (!page.SelectedLines.Contains(tablebox.LineInfo) && !isselected)
                                {
                                    page.SelectedLines.Add(tablebox.LineInfo);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tablebox.ChildBox != null)
                        {
                            page = Viewer.Pages[tablebox.ChildBox.LineInfo.PageIndex];
                            //isselected =tablebox.ChildBox.IsFullCellSelected;
                            isselected = page.IsSelected;
                            tablebox.ChildBox.IsFullCellSelected = tablebox.IsFullCellSelected;
                            if (line.ElementBoxes.IndexOf(elementbox) == line.ElementBoxes.Count - 1)
                            {
                                if (!page.SelectedLines.Contains(tablebox.ChildBox.LineInfo) && !isselected)
                                {
                                    page.SelectedLines.Add(tablebox.ChildBox.LineInfo);
                                    if (tablebox.ChildBox.ChildBox != null)
                                    {
                                        SelectSplittedLines(tablebox.ChildBox.LineInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal List<LineInfo> GetBasedOngivenPageIndex(List<LineInfo> given, int index)
        {
            List<LineInfo> output = given.Where(l => l.PageIndex == index).ToList();
            return output;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.ForegroundContainer != null && SelectionContainer != null && Container != null && OwnerControl !=null)
            {
                if (!double.IsInfinity(availableSize.Height) && !double.IsInfinity(availableSize.Width))
                {
                    this.ForegroundContainer.Width = availableSize.Width;
                    this.ForegroundContainer.Height = availableSize.Height;
                    SelectionContainer.Width = availableSize.Width;
                    SelectionContainer.Height = availableSize.Height;
                    Container.Width = availableSize.Width;
                    Container.Height = availableSize.Height;
                }
                else
                {
                    this.ForegroundContainer.Width = OwnerControl.contentPresenter.ActualWidth;
                    this.ForegroundContainer.Height = OwnerControl.contentPresenter.ActualHeight;
                    SelectionContainer.Width = OwnerControl.contentPresenter.Width;
                    SelectionContainer.Height = OwnerControl.contentPresenter.Height;
                    Container.Width = OwnerControl.contentPresenter.Width;
                    Container.Height = OwnerControl.contentPresenter.Height;
                }
            }

            //Caret.PositionCursor();

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Shows the caret
        /// </summary>
        public void ShowCaret(bool isTouch)
        {
            if (Caret != null && (!OwnerControl.IsReadOnly || OwnerControl.EnableCursorOnReadOnly))
            {
                this.Caret.Show();
            }
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            if ((this.Viewer != null && !this.Viewer.isTouchInsideSelection) || !isTouch)
            {
                //Collapses the visibility of Touch start and end marks.
                TouchStart.Visibility = Visibility.Collapsed;
                TouchEnd.Visibility = Visibility.Collapsed;
            }
#endif
        }
        
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Shows the touch mark.
        /// </summary>
        internal void ShowTouchMark()
        {
            if (Caret != null && TouchStart != null && (!OwnerControl.IsReadOnly || OwnerControl.EnableCursorOnReadOnly))
            {
                SetTouchStart(new Point(Caret.Location.X + Caret.Width, Caret.Location.Y + Caret.Height), true);
                touchEndPoint.X = touchStartPoint.X;
                touchEndPoint.Y = touchStartPoint.Y;
            }
        }
        /// <summary>
        /// Sets the touch start.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        internal void SetTouchStart(Point startPoint, bool visible)
        {
            if (TouchStart != null)
            {
                if (visible)
                    TouchStart.Visibility = Visibility.Visible;
                touchStartPoint = new Point(startPoint.X - TouchStart.Width / 2, startPoint.Y);
                Canvas.SetLeft(TouchStart, touchStartPoint.X);
                Canvas.SetTop(TouchStart, touchStartPoint.Y);
            }
        }
        /// <summary>
        /// Sets the touch end.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        internal void SetTouchEnd(Point endPoint, bool visible)
        {
            if (TouchEnd != null)
            {
                if (visible)
                    TouchEnd.Visibility = Visibility.Visible;
                touchEndPoint = new Point(endPoint.X - TouchStart.Width / 2, endPoint.Y);
                Canvas.SetLeft(TouchEnd, touchEndPoint.X);
                Canvas.SetTop(TouchEnd, touchEndPoint.Y);
            }
        }
        /// <summary>
        /// Sets the touch mark at selection start.
        /// </summary>
        internal void SetTouchMarkAtSelectionStart()
        {
            ElementBox elementBox = OwnerControl.Selection.Start.LineInfo.GetElementBoxFromPoint(OwnerControl.Selection.StartPoint);
            TableCellAdv cell = OwnerControl.Selection.Start.Paragraph.AssociatedCell;
            Point startPoint = new Point();
            if (cell != null && cell.CellElementBox != null && cell.CellElementBox.IsFullCellSelected)
            {
                elementBox = cell.CellElementBox;
                startPoint = new Point(elementBox.ElementLocation.X, elementBox.ElementLocation.Y + TouchStart.Width / 2);
                SetTouchEnd(startPoint, true);
            }
            else if (elementBox != null)
            {
                startPoint = new Point(OwnerControl.Selection.StartPoint.X, elementBox.ElementLocation.Y + elementBox.ElementSize.Height);
                SetTouchStart(startPoint, true);
            }
        }
        /// <summary>
        /// Sets the touch mark at selection end.
        /// </summary>
        internal void SetTouchMarkAtSelectionEnd()
        {
            ElementBox elementBox = OwnerControl.Selection.End.LineInfo.GetElementBoxFromPoint(OwnerControl.Selection.EndPoint);
            TableCellAdv cell = OwnerControl.Selection.End.Paragraph.AssociatedCell;
            Point endPoint = new Point();
            if (cell != null && cell.CellElementBox != null && cell.CellElementBox.IsFullCellSelected)
            {
                elementBox = cell.CellElementBox;
                endPoint = new Point(elementBox.ElementLocation.X + elementBox.ElementSize.Width, elementBox.ElementLocation.Y + TouchEnd.Width / 2);
                SetTouchEnd(endPoint, true);
            }
            else if (elementBox != null)
            {
                endPoint = new Point(OwnerControl.Selection.EndPoint.X, elementBox.ElementLocation.Y + elementBox.ElementSize.Height);
                SetTouchEnd(endPoint, true);
            }
        }
#endif
        /// <summary>
        /// Hides the caret
        /// </summary>
        public void HideCaret(bool isTouch)
        {
            if (Caret != null)
            {
                this.Caret.Hide();
            }
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            if ((this.Viewer != null && !this.Viewer.isTouchInsideSelection) || !isTouch)
            {
                //Collapses the visibility of Touch start and end marks.
                TouchStart.Visibility = Visibility.Collapsed;
                TouchEnd.Visibility = Visibility.Collapsed;
            }
#endif
        }

        /// <summary>
        /// Updates page layout
        /// </summary>
        internal void UpdatePageLayout()
        {
            InvalidateMeasure();
            InvalidateArrange();
            UpdateLayout();
        }

        /// <summary>
        /// Releases the resources
        /// </summary>
        public void ReleaseResources()
        {
            OwnerControl = null;
            Viewer = null;
            this.Container = null;
            this.ForegroundContainer = null;
            this.SelectionContainer = null;
            this.DecorationContainer = null;
            this.Content = null;
            Caret.ReleaseResources();
            this.Caret = null;
            SelectedLines.Clear();
            SelectedLines = null;
            LineInfos.CollectionChanged -= new NotifyCollectionChangedEventHandler(LineInfos_CollectionChanged);
            LineInfos.Clear();
            LineInfos = null;
        }

        internal void CollapseBorder(bool collapse)
        {
            if (effectBorder != null)
            {
                if (collapse)
                    effectBorder.Visibility = Visibility.Collapsed;
                else
                    effectBorder.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LineInfos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList list = e.NewItems;
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                list = e.OldItems;
            }
            if (list != null)
            {
                foreach (object obj in list)
                {
                    LineInfo line = obj as LineInfo;
                    if (e.Action == NotifyCollectionChangedAction.Add && Viewer is PageLayoutViewer)
                    {
                        //foreach (ElementBox element in line.ElementBoxes)
                        //{
                        //    if (!ForegroundContainer.Children.Contains(element.Element))
                        //    {
                        //        if (element.Element.Parent != null)
                        //        {
                        //            (element.Element.Parent as Canvas).Children.Remove(element.Element);
                        //        }
                        //        ForegroundContainer.Children.Add(element.Element);
                        //    }
                        //}

                        foreach (UIElement element in line.Elements)
                        {
                            if (!ForegroundContainer.Children.Contains(element))
                            {
                                if ((element as FrameworkElement).Parent != null)
                                {
                                    ((element as FrameworkElement).Parent as Canvas).Children.Remove(element);
                                }
                                ForegroundContainer.Children.Add(element);
                            }
                        }

                        //if (line.Container != null && !ForegroundContainer.Children.Contains(line.Container))
                        //{
                        //    ForegroundContainer.Children.Add(line.Container);
                        //}
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        //foreach (ElementBox element in line.ElementBoxes)
                        //{
                        //    if (ForegroundContainer.Children.Contains(element.Element))
                        //    {
                        //        ForegroundContainer.Children.Remove(element.Element);
                        //    }
                        //}

                        OwnerControl.RenderingManager.Remove(line, this);

                        foreach (UIElement element in line.Elements)
                        {
                            if (ForegroundContainer.Children.Contains(element))
                            {
                                ForegroundContainer.Children.Remove(element);
                            }
                        }

                        //if (line.Container != null && ForegroundContainer.Children.Contains(line.Container))
                        //{
                        //    ForegroundContainer.Children.Remove(line.Container);
                        //    line.Container = null;
                        //}
                    }
                }
            }
        }

        #endregion
    }
}
