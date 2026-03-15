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
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    public class PageLayoutViewer : LayoutViewer
    {
        private Canvas Container;

        internal double PageGap = 20;
        internal double StartMargin = 30;
        private double VerticalHeight;
        private double HorizontalWidth;
        private TranslateTransform transform;
        private List<PageAdv> VisiblePages;

        public PageLayoutViewer(RichTextBoxAdv richTextBox)
            : base(richTextBox)
        {
            Container = new Canvas();
            Container.Background = new SolidColorBrush(Colors.Transparent);
            DefaultStyleKey = typeof(PageLayoutViewer);
            this.Content = Container;
            LineInfos = new ObservableCollection<LineInfo>();
            LineInfos.CollectionChanged += new NotifyCollectionChangedEventHandler(LineInfosCollectionChanged);
            transform = new TranslateTransform();
            VisiblePages = new List<PageAdv>();
            this.MouseWheel += new MouseWheelEventHandler(PageLayoutViewer_MouseWheel);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(PageLayoutViewer_ManipulationDelta);
            this.IsManipulationEnabled = richTextBox.IsManipulationEnabled;
#endif
        }

        public PageLayoutViewer()
        {

        }
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Handles the ManipulationDelta event of the PageLayoutViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationDeltaEventArgs" /> instance containing the event data.</param>
        void PageLayoutViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (isTouchDownOnSelectionMark)
            {
                base.OnManipulationDelta(e);
                return;
            }
            if (e.DeltaManipulation.Scale.Length == 1.4142135623730951
                || (e.DeltaManipulation.Scale.X == 1d && e.DeltaManipulation.Scale.Y == 1d))
            {
                base.OnManipulationDelta(e);
            }
            else
            {
                //Handled for zooming the page view size.
                e.Handled = true;
                if (e.DeltaManipulation.Scale.Length < 1.4142135623730951)
                {
                    if (transX >= 0.25 && transY >= 0.25)
                    {
                        transX -= 0.05;
                        transY -= 0.05;
                    }
                }
                else if (e.DeltaManipulation.Scale.Length > 1.4142135623730951)
                {
                    if (transX <= 4 && transY <= 4)
                    {
                        transX += 0.05;
                        transY += 0.05;
                    }
                }
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = transX / 4;
                Zoom();
            }
            //Handled for panning (Move) the page to view particular region.
            if (e.DeltaManipulation.Translation.X != 0)
                this.HorizontalScrollBar.Value = HorizontalScrollBar.Value - e.DeltaManipulation.Translation.X;
            if (e.DeltaManipulation.Translation.Y != 0)
                this.VerticalScrollBar.Value = VerticalScrollBar.Value - e.DeltaManipulation.Translation.Y;
        }
#endif
        void PageLayoutViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (Keyboard.Modifiers == ModifierKeys.Control && IsZoomEnabled)
            {
                if (e.Delta < 0)
                {
                    if (transX >= 0.25 && transY >= 0.25)
                    {
                        transX -= 0.05;
                        transY -= 0.05;
                    }
                }
                else
                {
                    if (transX <= 4 && transY <= 4)
                    {
                        transX += 0.05;
                        transY += 0.05;
                    }
                }
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = transX / 4;
                Zoom();
            }
            else
            {
                if (e.Delta < 0)
                {
                    this.VerticalScrollBar.Value = VerticalScrollBar.Value + VerticalScrollBar.SmallChange;
                }
                else
                {
                    this.VerticalScrollBar.Value = VerticalScrollBar.Value - VerticalScrollBar.SmallChange;
                }
            }
        }

        internal override void Zoom()
        {
            scaleTransform.ScaleX = transX;
            scaleTransform.ScaleY = transY;
            Pages.ForEach(page =>
            {
                page.ForegroundContainer.RenderTransform = scaleTransform;
                page.DecorationContainer.RenderTransform = scaleTransform;
                page.SelectionContainer.RenderTransform = scaleTransform;
                page.Width = page.ContainerWidth * transX;
                page.Height = page.ContainerHeight * transY;

            });

            //calculatedWidth = OriginalSize.Width / transX;
            //calculatedHeight = OriginalSize.Height / transY;

            //AvailableSize = new Size(calculatedWidth, calculatedHeight);

            //Visiblebounds = new Rect(Visiblebounds.X, Visiblebounds.Y, calculatedWidth, calculatedHeight);

            ArrangePages(AvailableSize);

            OwnerControl.PositionHandler.PositionCursor();
        }

        internal void ApplyZoomingFactor(PageAdv page)
        {
            scaleTransform.ScaleX = transX;
            scaleTransform.ScaleY = transY;
            page.ForegroundContainer.RenderTransform = scaleTransform;
            page.DecorationContainer.RenderTransform = scaleTransform;
            page.SelectionContainer.RenderTransform = scaleTransform;
            page.Width = page.ContainerWidth * transX;
            page.Height = page.ContainerHeight * transY;
        }

        internal override bool CheckForScrollBarVisibility()
        {
            return false;
        }

        /// <summary>
        /// Arranges the elements
        /// </summary>
        public override void ArrangeElements()
        {
            if (OnLoading)
            {
                RemovePages();
                LineInfos.Clear();
                foreach (SectionAdv section in Document.Sections)
                {
                    foreach (BlockAdv block in section.Blocks)
                    {
                        if (block.LineInfo.Count > 0)
                        {
                            block.ClearLines();
                        }
                        if (block.NextBlock != null && block.NextBlock.LineInfo.Count > 0)
                        {
                            block.NextBlock.ClearLines();
                        }
                        block.IsArrangingParagraph = true;
                        block.LayoutViewer = this;
                        block.LinkElementBoxes();
                        block.ArrangeElements();
                        block.IsArranged = false;
                    }
                }
            }

            ArrangePages(AvailableSize);
        }

        /// <summary>
        /// Removes all the pages from the container
        /// </summary>
        internal void RemovePages()
        {
            foreach (PageAdv page in Pages)
            {
                if (Container.Children.Contains(page))
                {
                    Container.Children.Remove(page);
                }
                page.ReleaseResources();
            }
            VisiblePages.Clear();
            Pages.Clear();
        }

        public override void SetPreviousBlocks()
        {
            foreach (SectionAdv section in Document.Sections)
            {
                if (section.Blocks.Count > 0)
                {
                    section.Blocks[0].PreviousBlock = null;
                    section.Blocks[0].Section = section;
                }
                IterateBlocks(section.Blocks);
            }
        }

        internal void IterateBlocks(BlockCollection<BlockAdv> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] is TableAdv)
                {
                    foreach (TableRowAdv row in (blocks[i] as TableAdv).Rows)
                    {
                        foreach (TableCellAdv cell in row.Cells)
                        {
                            IterateBlocks(cell.Blocks);
                        }
                    }
                }
                if (i > 0)
                {
                    blocks[i].PreviousBlock = blocks[i - 1];
                }
                if (i + 1 != blocks.Count)
                    blocks[i].NextBlock = blocks[i + 1];
                else
                    blocks[i].NextBlock = null;
            }
        }

        public override void SetVisibleLinesToPage()
        {
            ArrangePages(AvailableSize);
        }

        /// <summary>
        /// Brings the line to the visible bounds
        /// </summary>
        /// <param name="line"></param>
        public override void BringLineToView(LineInfo line)
        {
            if (line != null)
            {
                PageAdv page = Pages[line.PageIndex];

                double bottom = page.BoundingRectangle.Top + GetZoomedValue(line.BoundingRectangle.Bottom, false);
                double right = page.BoundingRectangle.Left + GetZoomedValue(line.BoundingRectangle.Right, true);
                double top = page.BoundingRectangle.Top + GetZoomedValue(line.BoundingRectangle.Top, false);
                double left = page.BoundingRectangle.Left + GetZoomedValue(line.BoundingRectangle.Left, true);
                //double bottom = page.BoundingRectangle.Top + line.BoundingRectangle.Bottom;
                //double right = page.BoundingRectangle.Left + line.BoundingRectangle.Right;
                //double top = page.BoundingRectangle.Top + line.BoundingRectangle.Top;
                //double left = page.BoundingRectangle.Left + line.BoundingRectangle.Left;
                if (Visiblebounds.Bottom < bottom)
                {
                    VerticalScrollBar.Value = VerticalScrollBar.Value + (bottom - Visiblebounds.Bottom);
                }

                if (Visiblebounds.Top > top)
                {
                    VerticalScrollBar.Value = VerticalScrollBar.Value - (Visiblebounds.Top - top);
                }

                if (Visiblebounds.Right < right)
                {
                    HorizontalScrollBar.Value = HorizontalScrollBar.Value + (right - Visiblebounds.Right);
                }

                if (Visiblebounds.Left > left)
                {
                    HorizontalScrollBar.Value = HorizontalScrollBar.Value - (Visiblebounds.Left - left);
                }
            }
        }

        public override void UpdateVerticalScrollBar()
        {

        }

        public override void UpdateHorizontalScrollBar()
        {
            if (HorizontalScrollBar.Visibility == Visibility.Visible)
            {
                HorizontalScrollBar.Maximum = HorizontalWidth - AvailableSize.Width;
            }
        }

        /// <summary>
        /// Releases the resources
        /// </summary>
        internal override void RemoveViewer()
        {
            RemovePages();
            Container = null;
            this.Content = null;
            LineInfos.CollectionChanged -= new NotifyCollectionChangedEventHandler(LineInfosCollectionChanged);
            LineInfos.Clear();
            LineInfos = null;
            transform = null;
            VisiblePages.Clear();
            VisiblePages = null;
            this.MouseWheel -= new MouseWheelEventHandler(PageLayoutViewer_MouseWheel);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            this.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(PageLayoutViewer_ManipulationDelta);
#endif
            VerticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(VerticalScrollBar_ValueChanged);
            HorizontalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(HorizontalScrollBar_ValueChanged);
            HorizontalScrollBar.Visibility = Visibility.Collapsed;
            base.RemoveViewer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LineInfosCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList lineInfos = e.NewItems;
            int index = e.NewStartingIndex;
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                lineInfos = e.OldItems;
                index = e.OldStartingIndex;
            }
            if (lineInfos != null)
            {
                foreach (object obj in lineInfos)
                {
                    if (obj is LineInfo)
                    {
                        LineInfo line = obj as LineInfo;
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            PageAdv page = null; ;
                            if (index == 0 && LineInfos.Count == 0)
                            {
                                page = new PageAdv();
                                page.Width = line.Block.Section.PageSize.Width;
                                page.Height = line.Block.Section.PageSize.Height;
                                page.Section = line.Block.Section;
                                page.Background = line.Block.Section.PageBackground;
                                page.ShadowBackground = line.Block.Section.PageShadowBackground;
                                page.ContainerWidth = page.Width;
                                page.ContainerHeight = page.Height;
                                page.OwnerControl = OwnerControl;
                                page.Viewer = this;
                                page.LineInfos.Add(line);
                                Pages.Add(page);
                                line.PageIndex = Pages.IndexOf(page);
                                ApplyZoomingFactor(page);
                            }
                            else if (index == 0)
                            {
                                if (Pages.Count > 0)
                                {
                                    page = Pages[0];
                                    page.LineInfos.Insert(0, line);
                                    line.PageIndex = 0;
                                }
                                else
                                {
                                    page = new PageAdv();
                                    page.Width = line.Block.Section.PageSize.Width;
                                    page.Height = line.Block.Section.PageSize.Height;
                                    page.Section = line.Block.Section;
                                    page.Background = line.Block.Section.PageBackground;
                                    page.ShadowBackground = line.Block.Section.PageShadowBackground;
                                    page.ContainerWidth = page.Width;
                                    page.ContainerHeight = page.Height;
                                    page.OwnerControl = OwnerControl;
                                    page.Viewer = this;
                                    page.LineInfos.Add(line);
                                    Pages.Add(page);
                                    line.PageIndex = Pages.IndexOf(page);
                                    ApplyZoomingFactor(page);
                                }
                            }
                            else if (index < LineInfos.Count && LineInfos.Count > 1)
                            {
                                LineInfo lineInfo = LineInfos[e.NewStartingIndex - 1];
                                //if (lineInfo.PageIndex >= Pages.Count)
                                //{
                                page = GetPageFromLine(lineInfo);
                                //}
                                //else
                                //{
                                //    page = Pages[lineInfo.PageIndex];
                                //}
                                double limit = 0.0;
                                if (page != null)
                                {
                                    limit = page.Height - GetZoomedValue(lineInfo.Block.Margin.Bottom, false);
                                    double total = GetZoomedValue(lineInfo.BoundingRectangle.Bottom, false) + GetZoomedValue(line.Height, false);

                                    if (line.IsTableLine && total > limit)
                                    {
                                        double previous = Math.Round(Math.Ceiling(GetZoomedValue(lineInfo.BoundingRectangle.Bottom, false)));
                                        if (previous == limit)
                                        {
                                            if (GetZoomedValue(line.Height, false) + GetZoomedValue(lineInfo.Block.Margin.Top, false) > limit)
                                            {
                                                line.SplitTableLine(GetZoomedValue(line.Height, false) + GetZoomedValue(lineInfo.Block.Margin.Bottom, false) - limit);
                                            }
                                            else
                                            {
                                                line.SplitTableLine(total - limit);
                                            }
                                        }
                                        else
                                        {
                                            line.SplitTableLine(total - limit);
                                        }

                                        line.Height = Math.Floor(GetZoomedValue(line.Height, false));
                                    }
                                    if (GetZoomedValue(lineInfo.BoundingRectangle.Bottom, false) + GetZoomedValue(line.Height, false) > limit)
                                    {
                                        PageAdv pgAdv = GetPageFromLine(lineInfo);
                                        lineInfo.IsLastLine = true;
                                        if (pgAdv != null)
                                            lineInfo.PageIndex = Pages.IndexOf(pgAdv);
                                        if (lineInfo.PageIndex + 1 == Pages.Count)
                                        {
                                            page = new PageAdv();
                                            page.Width = line.Block.Section.PageSize.Width;
                                            page.Height = line.Block.Section.PageSize.Height;
                                            page.Section = line.Block.Section;
                                            page.Background = line.Block.Section.PageBackground;
                                            page.ShadowBackground = line.Block.Section.PageShadowBackground;
                                            page.ContainerWidth = page.Width;
                                            page.ContainerHeight = page.Height;
                                            page.OwnerControl = OwnerControl;
                                            page.Viewer = this;
                                            Pages.Add(page);
                                            page.LineInfos.Add(line);
                                            line.PageIndex = Pages.IndexOf(page);
                                            ApplyZoomingFactor(page);

                                        }
                                        else
                                        {
                                            page = Pages[lineInfo.PageIndex + 1];
                                            page.LineInfos.Insert(0, line);
                                            line.PageIndex = Pages.IndexOf(page);
                                        }
                                        Rect rect = line.BoundingRectangle;
                                        rect.Y = line.Block.Margin.Top;
                                        line.BoundingRectangle = rect;
                                        line.IsFirstLine = true;
                                    }
                                    else
                                    {
                                        line.PageIndex = Pages.IndexOf(page);
                                        if (lineInfo.PageIndex == line.PageIndex)
                                        {
                                            int i = page.LineInfos.IndexOf(lineInfo);
                                            if (page.LineInfos.Count == i - 1)
                                            {
                                                page.LineInfos.Add(line);
                                            }
                                            else
                                            {
                                                page.LineInfos.Insert(i + 1, line);
                                            }
                                        }
                                        else if (page.LineInfos.Count == 0)
                                        {
                                            page.LineInfos.Add(line);
                                        }
                                        else
                                        {
                                            page.LineInfos.Insert(0, line);
                                        }
                                    }
                                }
                            }
                        }
                        else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                        {
                            if (Pages.Count > 0)
                            {
                                PageAdv page = null;
                                //if (line.PageIndex >= Pages.Count)
                                //{
                                page = GetPageFromLine(line);
                                //}
                                //else
                                //{
                                //    page = Pages[line.PageIndex];
                                //}
                                if (page != null)
                                {
                                    if (page.LineInfos.Contains(line))
                                    {
                                        page.LineInfos.Remove(line);
                                    }

                                    if (page.LineInfos.Count == 0)
                                    {
                                        Pages.Remove(page);
                                        page.ForegroundContainer.Children.Clear();
                                        page.SelectionContainer.Children.Clear();
                                        if (page.Parent != null && page.Parent is Canvas)
                                        {
                                            (page.Parent as Canvas).Children.Remove(page);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Width))
                Container.Width = availableSize.Width;
            if (!double.IsInfinity(availableSize.Height))
                Container.Height = availableSize.Height;

            Visiblebounds = new Rect(0, 0, availableSize.Width, availableSize.Height);
            Rect rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
            RectangleGeometry geo = new RectangleGeometry();
            geo.Rect = rect;

            this.Clip = geo;

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Arranges the pages in order
        /// </summary>
        /// <param name="availableSize"></param>
        internal void ArrangePages(Size availableSize)
        {
            double height = PageGap;
            int startIndex = 0;
            int endIndex = 0;
            bool canArrange = false;
            double width = 0;
            HorizontalWidth = 0;
            for (int i = 0; i <= Pages.Count; i++)
            {
                endIndex = i;
                if (i != Pages.Count)
                {
                    width = width + Pages[i].Width;
                }
                //if (Pages.Count != 1 && i != Pages.Count - 1 && i != 0)
                if (Pages.Count != 1 && i != 0)
                {
                    width += PageGap;
                }
                bool isTrue = Pages.Count == 1 ? true : Pages.Count == i ? true : false;
                canArrange = (availableSize.Width - width) / 2 <= StartMargin ? true : isTrue;

                if (Pages.Count != 1)
                {
                    endIndex = i - 1;
                }
                if (Pages.Count == 1 && endIndex == Pages.Count)
                {
                    canArrange = false;
                }

                if (canArrange)
                {
                    PositionPages(startIndex, endIndex, availableSize, ref height);
                    startIndex = i;
                    canArrange = false;
                    if (i != Pages.Count)
                    {
                        width = Pages[i].Width;
                    }
                }
            }

            VerticalHeight = height;

            height -= availableSize.Height;


            double horWidth = HorizontalWidth - availableSize.Width;

            if (!IsPrinting)
            {
                if (VerticalScrollBar != null && HorizontalScrollBar != null)
                {
                    if (height > 0)
                    {
                        VerticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(VerticalScrollBar_ValueChanged);
                        if (OwnerControl.VerticalScrollBarVisibility)
                            VerticalScrollBar.Visibility = Visibility.Visible;
                        VerticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(VerticalScrollBar_ValueChanged);
                        VerticalScrollBar.Maximum = height;
                        VerticalScrollBar.Minimum = 0;
                        VerticalScrollBar.ViewportSize = AvailableSize.Height;
                        VerticalScrollBar.SmallChange = height / LineInfos.Count;
                        VerticalScrollBar.LargeChange = VerticalScrollBar.SmallChange * 2;
                    }
                    else
                    {
                        VerticalScrollBar.Visibility = Visibility.Collapsed;
                        VerticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(VerticalScrollBar_ValueChanged);
                        transform.Y = 0;
                        Container.RenderTransform = transform;

                    }
                    if (horWidth > 0)
                    {
                        HorizontalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(HorizontalScrollBar_ValueChanged);
                        HorizontalScrollBar.Visibility = Visibility.Visible;
                        HorizontalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(HorizontalScrollBar_ValueChanged);
                        HorizontalScrollBar.Maximum = horWidth;
                        HorizontalScrollBar.Minimum = 0;
                        HorizontalScrollBar.ViewportSize = availableSize.Width;
                        HorizontalScrollBar.SmallChange = horWidth / 50;
                        HorizontalScrollBar.LargeChange = HorizontalScrollBar.SmallChange * 2;
                    }
                    else
                    {
                        HorizontalScrollBar.Visibility = Visibility.Collapsed;
                        HorizontalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(HorizontalScrollBar_ValueChanged);
                        transform.X = 0;
                        Container.RenderTransform = transform;
                    }
                }
            }
        }

        void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            transform.X = -HorizontalScrollBar.Value;
            Container.RenderTransform = transform;

            Visiblebounds = new Rect(e.NewValue, VerticalScrollBar.Value, AvailableSize.Width, AvailableSize.Height);
        }

        /// <summary>
        /// Position the pages depending on the size
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="size"></param>
        /// <param name="top"></param>
        internal void PositionPages(int startIndex, int endIndex, Size size, ref double top)
        {
            double maxHeight = 0;
            double sumOfWidth = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                maxHeight = Math.Max(Pages[i].Height, maxHeight);
                sumOfWidth = sumOfWidth + Pages[i].Width;
            }

            if (startIndex < endIndex)
            {
                sumOfWidth = sumOfWidth + ((endIndex - startIndex) * PageGap);
            }

            double xPos = (size.Width - sumOfWidth) / 2 >= StartMargin ? (size.Width - sumOfWidth) / 2 : StartMargin;
            double yPos = 0;
            sumOfWidth = sumOfWidth + (xPos * 2);

            for (int i = startIndex; i <= endIndex; i++)
            {
                yPos = top + (maxHeight - Pages[i].Height) / 2;

                if (!double.IsInfinity(xPos))
                    Canvas.SetLeft(Pages[i], xPos);

                if (!double.IsInfinity(yPos))
                    Canvas.SetTop(Pages[i], yPos);

                Pages[i].BoundingRectangle = new Rect(xPos, yPos, Pages[i].Width, Pages[i].Height);
                xPos = xPos + Pages[i].Width + PageGap;

                if (!Container.Children.Contains(Pages[i]) && Visiblebounds.IsIntersecting(Pages[i].BoundingRectangle))
                {
                    Container.Children.Add(Pages[i]);
                    if(!VisiblePages.Contains(Pages[i]))
                        VisiblePages.Add(Pages[i]);

                    AddLinesAtTop(Pages[i]);
                }
                else if (Visiblebounds.IsIntersecting(Pages[i].BoundingRectangle))
                {
                    //Pages[i].DecorationContainer.Children.Clear();
                    AddLinesAtTop(Pages[i]);
                }
            }

            top = top + maxHeight + PageGap;

            HorizontalWidth = Math.Max(HorizontalWidth, sumOfWidth);
        }

        internal PageAdv GetPageDownTo(PageAdv page)
        {
            if (page != null)
            {
                double top = page.BoundingRectangle.Top + page.Height + PageGap + 2;
                double left = page.BoundingRectangle.Left;
                Point point = new Point(left, top);
                foreach (PageAdv pageAdv in Pages)
                {
                    if (pageAdv.BoundingRectangle.Contains(point))
                        return pageAdv;
                }
            }
            return null;
        }

        internal PageAdv GetPageTopOf(PageAdv page)
        {
            if (page != null)
            {
                double top = page.BoundingRectangle.Top - PageGap - 2;
                double left = page.BoundingRectangle.Left;
                Point point = new Point(left, top);
                if (top < 0)
                    return null;
                foreach (PageAdv pageAdv in Pages)
                {
                    if (pageAdv.BoundingRectangle.Contains(point))
                        return pageAdv;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Dispatcher.BeginInvoke(new Action<RoutedPropertyChangedEventArgs<double>>(DoVerticalScroll), e);
            DoVerticalScroll(e);
            //transform.Y = -VerticalScrollBar.Value;
            //Container.RenderTransform = transform;

            //Visiblebounds = new Rect(HorizontalScrollBar.Value, e.NewValue, AvailableSize.Width, AvailableSize.Height);

            //Pages.ForEach(page =>
            //{
            //    if (Visiblebounds.IsIntersecting(page.BoundingRectangle))
            //    {
            //        if (!Container.Children.Contains(page))
            //        {
            //            Container.Children.Add(page);
            //        }
            //    }
            //    else
            //    {
            //        if (Container.Children.Contains(page))
            //        {
            //            Container.Children.Remove(page);
            //        }
            //    }
            //});

            //UpdateHorizontalScrollBar();
        }

        void DoVerticalScroll(RoutedPropertyChangedEventArgs<double> e)
        {
            transform.Y = -VerticalScrollBar.Value;

            bool top = Visiblebounds.Y > e.NewValue ? false : true;

            if (Visiblebounds.Y != e.NewValue)
            {
                Visiblebounds = new Rect(HorizontalScrollBar.Value, e.NewValue, AvailableSize.Width, AvailableSize.Height);

                CalcVisibleItems(top);

                if (VisiblePages.Count == 0)
                {
                    Container.Children.Clear();
                    Pages.ForEach(page =>
                    {
                        if (Visiblebounds.IsIntersecting(page.BoundingRectangle))
                        {
                            if (!Container.Children.Contains(page))
                            {
                                Container.Children.Add(page);
                                page.ForegroundContainer.RenderTransform = scaleTransform;
                                page.DecorationContainer.RenderTransform = scaleTransform;
                                page.SelectionContainer.RenderTransform = scaleTransform;
                                AddLinesAtTop(page);
                            }
                        }
                        else
                        {
                            if (Container.Children.Contains(page))
                            {
                                Container.Children.Remove(page);
                                //if (OwnerControl.DefferedScrolling)
                                //{
                                RemoveLinesAtTop(page);
                                //}
                            }
                        }
                    });
                }

                Container.RenderTransform = transform;

                UpdateHorizontalScrollBar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="top"></param>
        private void CalcVisibleItems(bool top)
        {
            List<PageAdv> removedPages = new List<PageAdv>();

            if (top)
            {
                int j = 0;
                for (int i = 0; i < VisiblePages.Count; i++)
                {
                    if (!(Visiblebounds.IsIntersecting(VisiblePages[i].BoundingRectangle)))
                    {
                        j++;
                        removedPages.Add(VisiblePages[i]);
                        if (Container.Children.Contains(VisiblePages[i]))
                        {
                            Container.Children.Remove(VisiblePages[i]);
                            //if (OwnerControl.DefferedScrolling)
                            //{
                            RemoveLinesAtTop(VisiblePages[i]);
                            //}
                        }
                    }
                }

                foreach (PageAdv page in removedPages)
                {
                    VisiblePages.Remove(page);
                }

                int index = 0;

                if (VisiblePages.Count != 0)
                {
                    index = Pages.IndexOf(VisiblePages.Last());
                }
                else
                {
                    j = 0;
                }
                if (j != 0)
                {
                    for (int i = 0; i < j; i++)
                    {
                        index++;
                        if (index < Pages.Count)
                        {
                            PageAdv page = Pages[index];
                            if (Visiblebounds.IsIntersecting(page.BoundingRectangle))
                            {
                                if(!VisiblePages.Contains(page))
                                    VisiblePages.Add(page);
                                if (!Container.Children.Contains(page))
                                {
                                    Container.Children.Add(page);
                                }
                            }
                        }
                    }

                    while (index < Pages.Count && Visiblebounds.IsIntersecting(Pages[index].BoundingRectangle))
                    {
                        if (!VisiblePages.Contains(Pages[index]))
                        {
                            PageAdv page = Pages[index];
                            if (!VisiblePages.Contains(page))
                                VisiblePages.Add(page);
                            if (!Container.Children.Contains(page))
                            {
                                Container.Children.Add(page);
                            }
                        }

                        ++index;
                    }
                }
                else if (VisiblePages.Count != 0)
                {
                    for (int i = index + 1; i < Pages.Count; i++)
                    {
                        PageAdv page = Pages[i];
                        if (Visiblebounds.IsIntersecting(page.BoundingRectangle))
                        {
                            if (!VisiblePages.Contains(page))
                                VisiblePages.Add(page);
                            if (!Container.Children.Contains(page))
                            {
                                Container.Children.Add(page);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                int j = 0;
                for (int i = VisiblePages.Count - 1; i >= 0; i--)
                {
                    if (!(Visiblebounds.IsIntersecting(VisiblePages[i].BoundingRectangle)))
                    {
                        j++;
                        removedPages.Add(VisiblePages[i]);
                        if (Container.Children.Contains(VisiblePages[i]))
                        {
                            Container.Children.Remove(VisiblePages[i]);
                            //if (OwnerControl.DefferedScrolling)
                            //{
                            RemoveLinesAtTop(VisiblePages[i]);
                            //}
                        }
                    }
                }

                foreach (PageAdv page in removedPages)
                {
                    VisiblePages.Remove(page);
                }

                int index = 0;

                if (VisiblePages.Count != 0)
                {
                    index = Pages.IndexOf(VisiblePages.First());
                }
                else
                {
                    j = 0;
                }

                if (j != 0)
                {
                    for (int i = 0; i < j; i++)
                    {
                        index--;
                        if (index >= 0)
                        {
                            PageAdv page = Pages[index];
                            if (Visiblebounds.IsIntersecting(page.BoundingRectangle))
                            {
                                if (!VisiblePages.Contains(page))
                                    VisiblePages.Insert(0, page);
                                if (!Container.Children.Contains(page))
                                {
                                    Container.Children.Add(page);
                                }
                            }
                        }
                    }

                    while (index >= 0 && Visiblebounds.IsIntersecting(Pages[index].BoundingRectangle))
                    {
                        if (!VisiblePages.Contains(Pages[index]))
                        {
                            PageAdv page = Pages[index];
                            if (!VisiblePages.Contains(page))
                                VisiblePages.Insert(0, page);
                            if (!Container.Children.Contains(page))
                            {
                                Container.Children.Add(page);
                            }
                        }

                        --index;
                    }
                }
                else if (VisiblePages.Count != 0)
                {
                    for (int i = index - 1; i >= 0; i--)
                    {
                        PageAdv page = Pages[i];
                        if (Visiblebounds.IsIntersecting(page.BoundingRectangle))
                        {
                            if (!VisiblePages.Contains(page))
                                VisiblePages.Add(page);
                            if (!Container.Children.Contains(page))
                            {
                                Container.Children.Add(page);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            foreach (PageAdv page in VisiblePages)
            {
                AddLinesAtTop(page);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="top"></param>
        internal void AddLinesAtTop(PageAdv page)
        {
            double offset = page.BoundingRectangle.Top;

            for (int i = 0; i < page.LineInfos.Count; i++)
            {
                double lineTop = GetZoomedValue(page.LineInfos[i].BoundingRectangle.Top, false);
                double lineBottom = GetZoomedValue(page.LineInfos[i].BoundingRectangle.Bottom, false);

                if (Visiblebounds.Top <= lineBottom + offset && Visiblebounds.Bottom >= lineTop + offset)
                {
                    /******Uncomment the below lines to render the text in image******/

                    //page.LineInfos[i].CreateDrawingContext();
                    //if (!page.ForegroundContainer.Children.Contains(page.LineInfos[i].Container))
                    //{
                    //    if (page.LineInfos[i].Container.Parent != null && page.LineInfos[i].Container.Parent is Canvas)
                    //    {
                    //        (page.LineInfos[i].Container.Parent as Canvas).Children.Remove(page.LineInfos[i].Container);
                    //    }
                    //    page.ForegroundContainer.Children.Add(page.LineInfos[i].Container);
                    //}

                    /******Uncomment the below line to render the text in TextBlock******/

                    // if (page.LineInfos[i].TextRenderers.Count == 0 || page.LineInfos[i].DecoratingElements.Count == 0)

                    RenderingManager.Render(page.LineInfos[i], page);

                    //foreach (UIElement element in page.LineInfos[i].Elements)
                    //{
                    //    if (!page.ForegroundContainer.Children.Contains(element))
                    //    {
                    //        if ((element as FrameworkElement).Parent != null && (element as FrameworkElement).Parent is Canvas)
                    //            ((element as FrameworkElement).Parent as Canvas).Children.Remove(element);
                    //        page.ForegroundContainer.Children.Add(element);
                    //    }
                    //}
                }
                else
                {
                    RemoveLineFromPage(page.LineInfos[i], page);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="top"></param>
        internal void RemoveLinesAtTop(PageAdv page)
        {
            for (int i = 0; i < page.LineInfos.Count; i++)
            {
                if (page.LineInfos[i].Container != null)
                {
                    if (page.ForegroundContainer.Children.Contains(page.LineInfos[i].Container))
                    {
                        page.ForegroundContainer.Children.Remove(page.LineInfos[i].Container);
                        page.LineInfos[i].Elements.Remove(page.LineInfos[i].Container);
                        page.LineInfos[i].Container = null;
                    }
                }
                
                RenderingManager.Remove(page.LineInfos[i], page);

                page.LineInfos[i].RenderingOption = RenderingOptions.Render;

                //foreach (UIElement element in page.LineInfos[i].Elements)
                //{
                //    if (page.ForegroundContainer.Children.Contains(element))
                //    {
                //        page.ForegroundContainer.Children.Remove(element);
                //    }
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal override void FindFocusedPage(MouseEventArgs e)
        {
            Point point = e.GetPosition(Container);
            if (Pages.Count > 0)
            {
                //foreach (PageAdv page in Pages)
                //{
                //    if (page.BoundingRectangle.Contains(point))
                //    {
                //        CurrentPage = page;
                //        break;
                //    }
                //    else
                //    {
                //        CurrentPage = null;
                //    }
                //}

                CurrentPage = GetPage(0, Pages.Count - 1, point);
            }
        }
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(TouchEventArgs e)
        {
            Point point = e.GetTouchPoint(Container).Position;
            if (Pages.Count > 0)
            {
                CurrentPage = GetPage(0, Pages.Count - 1, point);
            }
        }
#endif

        internal PageAdv GetPage(int start, int end, Point point)
        {
            if (end != -1 && start != end)
            {
                if (Pages[start].BoundingRectangle.Top <= point.Y && Pages[end].BoundingRectangle.Bottom >= point.Y)
                {
                    int noOfLines = end - start;
                    int half = (int)Math.Round((double)(noOfLines / 2));
                    PageAdv page = GetPage(start, start + half, point);
                    if (page == null)
                    {
                        page = GetPage(start + half + 1, end, point);
                    }

                    return page;
                }
            }
            else if (start != -1)
            {
                if (Pages[start].BoundingRectangle.Contains(point))
                {
                    return Pages[start];
                }
            }

            return null;
        }
    }
}
