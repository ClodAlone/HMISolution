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
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    public class FlowLayoutViewer : LayoutViewer
    {
        private PageAdv page;
        internal bool UseSelectionWidth = false;
        private DispatcherTimer scrollTimer;
        private TranslateTransform transform;
        private bool m_Scrollingenabled = false;

        /// <summary>
        /// Gets or Sets the visible line infos
        /// </summary>
        internal List<LineInfo> VisibleLineInfos
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the page
        /// </summary>
        internal PageAdv Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
                Content = value;
                if (page != null)
                    page.DecreaseThickness();
            }
        }

        /// <summary>
        /// Initialize control on load
        /// </summary>
        public FlowLayoutViewer(RichTextBoxAdv richTextBox)
            : base(richTextBox)
        {
            VisibleLineInfos = new List<LineInfo>();
            SizeChanged += new SizeChangedEventHandler(FlowLayoutViewer_SizeChanged);
            MouseWheel += new MouseWheelEventHandler(FlowLayoutViewer_MouseWheel);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(FlowLayoutViewer_ManipulationDelta);
            this.IsManipulationEnabled = richTextBox.IsManipulationEnabled;
#endif
            Page = new PageAdv();
            CurrentPage = Page;
            Pages.Add(Page);
            Page.Viewer = this;
            Page.OwnerControl = richTextBox;
            Caret = Page.Caret;
            transform = new TranslateTransform();
            LineInfos = new ObservableCollection<LineInfo>();
            LineInfos.CollectionChanged += new NotifyCollectionChangedEventHandler(LineInfosCollectionChanged);
            scrollTimer = new DispatcherTimer();
            scrollTimer.Interval = new TimeSpan(0, 0, 0, 0, 3);
            scrollTimer.Tick += new EventHandler(scrollTimer_Tick);
        }

        public FlowLayoutViewer()
        {

        }

#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Handles the ManipulationDelta event of the FlowLayoutViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationDeltaEventArgs" /> instance containing the event data.</param>
        void FlowLayoutViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
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
        /// <summary>
        /// Handles mouse wheel event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FlowLayoutViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
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

                Zoom();
            }
            else
            {
                if (m_Scrollingenabled)
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

            e.Handled = true;
        }

        internal override void Zoom()
        {
            double calculatedWidth = 0;
            double calculatedHeight = 0;
            scaleTransform.ScaleX = transX;
            scaleTransform.ScaleY = transY;

            TransformGroup renderTransform = new TransformGroup();
            renderTransform.Children.Add(transform);
            renderTransform.Children.Add(scaleTransform);

            Pages.ForEach(page =>
            {
                page.ForegroundContainer.RenderTransform = renderTransform;
                page.DecorationContainer.RenderTransform = renderTransform;
                page.SelectionContainer.RenderTransform = renderTransform;
            });

            calculatedWidth = OriginalSize.Width / transX;
            calculatedHeight = OriginalSize.Height / transY;

            Visiblebounds = new Rect(Visiblebounds.X, Visiblebounds.Y, calculatedWidth, calculatedHeight);

            AvailableSize = new Size(calculatedWidth, calculatedHeight);

            ArrangeElements();

            SetVisibleLinesToPage();

            OwnerControl.PositionHandler.PositionCursor();
        }

        /// <summary>
        /// Handles the size changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FlowLayoutViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVerticalScrollBar();
        }

        /// <summary>
        /// Updates the vertical scroll bar
        /// </summary>
        public override void UpdateVerticalScrollBar()
        {
            if (LineInfos.Count > 0)
            {
                if (GetZoomedValue(LineInfos.Last().BoundingRectangle.Bottom, false) > AvailableSize.Height)
                {
                    VerticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(verticalScrollBar_ValueChanged);
                    double height = LineInfos.Last().BoundingRectangle.Bottom - LineInfos.First().BoundingRectangle.Top;
                    if (OwnerControl.VerticalScrollBarVisibility)
                        VerticalScrollBar.Visibility = Visibility.Visible;
                    VerticalScrollBar.ViewportSize = AvailableSize.Height;
                    VerticalScrollBar.Maximum = LineInfos.Last().BoundingRectangle.Bottom - AvailableSize.Height;
                    VerticalScrollBar.Minimum = 0;
                    VerticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(verticalScrollBar_ValueChanged);
                    VerticalScrollBar.SmallChange = height / LineInfos.Count;
                    VerticalScrollBar.LargeChange = VerticalScrollBar.SmallChange * 2;
                    m_Scrollingenabled = true;
                }
                //else if (Visiblebounds.Top > 0)
                //{
                //    VerticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(verticalScrollBar_ValueChanged);
                //    VerticalScrollBar.Visibility = Visibility.Visible;
                //    VerticalScrollBar.ViewportSize = AvailableSize.Height;
                //    VerticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(verticalScrollBar_ValueChanged);
                //    VerticalScrollBar.Minimum = 0;
                //    VerticalScrollBar.Maximum = LineInfos.Last().BoundingRectangle.Bottom;
                //}
                else
                {
                    VerticalScrollBar.Visibility = Visibility.Collapsed;
                    m_Scrollingenabled = false;
                    if (transform.Y != 0 && page != null)
                    {
                        transform.Y = 0;
                        TransformGroup renderTransform = new TransformGroup();
                        renderTransform.Children.Add(transform);
                        renderTransform.Children.Add(scaleTransform);

                        page.ForegroundContainer.RenderTransform = renderTransform;
                        page.SelectionContainer.RenderTransform = renderTransform;
                        page.DecorationContainer.RenderTransform = renderTransform;
                    }
                }
            }
            else
            {
                VerticalScrollBar.Visibility = Visibility.Collapsed;
                m_Scrollingenabled = false;
                if (transform.Y != 0 && page != null)
                {
                    transform.Y = 0;
                    TransformGroup renderTransform = new TransformGroup();
                    renderTransform.Children.Add(transform);
                    renderTransform.Children.Add(scaleTransform);

                    page.ForegroundContainer.RenderTransform = renderTransform;
                    page.SelectionContainer.RenderTransform = renderTransform;
                    page.DecorationContainer.RenderTransform = renderTransform;
                }
            }
        }

        /// <summary>
        /// Releases the resources
        /// </summary>
        internal override void RemoveViewer()
        {
            if (Page != null)
            {
                Page.ReleaseResources();
                Page.Viewer = null;
                Page.OwnerControl = null;
                Page = null;
            }

            VisibleLineInfos.Clear();
            VisibleLineInfos = null;
            SizeChanged -= new SizeChangedEventHandler(FlowLayoutViewer_SizeChanged);
            MouseWheel -= new MouseWheelEventHandler(FlowLayoutViewer_MouseWheel);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            this.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(FlowLayoutViewer_ManipulationDelta);
#endif
            CurrentPage = null;
            LineInfos.CollectionChanged -= new NotifyCollectionChangedEventHandler(LineInfosCollectionChanged);
            if (VerticalScrollBar != null) VerticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(verticalScrollBar_ValueChanged);
            LineInfos.Clear();
            LineInfos = null;
            scrollTimer.Tick -= new EventHandler(scrollTimer_Tick);
            scrollTimer = null;
            base.RemoveViewer();
        }

        /// <summary>
        /// Brings the specified line to view
        /// </summary>
        /// <param name="page"></param>
        /// <param name="line"></param>
        public override void BringLineToView(LineInfo line)
        {
            if (line != null)
            {
                double bottom = line.BoundingRectangle.Bottom; //GetZoomedValue(line.BoundingRectangle.Bottom, false);
                double height = line.BoundingRectangle.Height; //GetZoomedValue(line.BoundingRectangle.Height, false);
                double top = line.BoundingRectangle.Top; //GetZoomedValue(line.BoundingRectangle.Top, false);

                if (bottom > Visiblebounds.Bottom)
                {
                    VerticalScrollBar.Value = Visiblebounds.Top + (bottom + height) - Visiblebounds.Bottom;
                }

                if (top < Visiblebounds.Top && bottom < Visiblebounds.Bottom)
                {
                    VerticalScrollBar.Value = top;
                }
            }
        }

        void LineInfosCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList list = e.NewItems;
            int index = e.NewStartingIndex;
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                list = e.OldItems;
                index = e.OldStartingIndex;
            }
            if (list != null)
            {
                foreach (object obj in list)
                {
                    if (obj is LineInfo)
                    {
                        PageAdv page = null;
                        LineInfo line = obj as LineInfo;
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            if (Pages.Count == 0)
                            {
                                page = new PageAdv();
                                page.Viewer = this;
                                Pages.Add(page);
                            }

                            page = Pages[0];
                            page.Section = line.Block.Section;
                            page.Background = line.Block.Section.PageBackground;
                            page.ShadowBackground = line.Block.Section.PageShadowBackground;

                            if (index == page.LineInfos.Count)
                            {
                                page.LineInfos.Add(line);
                            }
                            else
                            {
                                page.LineInfos.Insert(index, line);
                            }
                        }
                        else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                        {
                            page = Pages[0];
                            page.LineInfos.Remove(line);
                        }
                    }
                }
            }
        }

        RoutedPropertyChangedEventArgs<double> eventArgs = null;

        void verticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OwnerControl.DefferedScrolling)
            {
                scrollTimer.Stop();
                scrollTimer.Start();
                eventArgs = e;
            }
            //Dispatcher.BeginInvoke(new Action<RoutedPropertyChangedEventArgs<double>>(DoVerticalScroll), e);
            DoVerticalScroll(e);
        }

        void scrollTimer_Tick(object sender, EventArgs e)
        {
            //Dispatcher.BeginInvoke(new Action<RoutedPropertyChangedEventArgs<double>>(DoVerticalScroll), eventArgs);
            DoVerticalScroll(eventArgs);
        }

        private void DoVerticalScroll(RoutedPropertyChangedEventArgs<double> e)
        {
            //TranslateTransform transform = new TranslateTransform();
            transform.X = 0;
            transform.Y = -VerticalScrollBar.Value;

            bool top = Visiblebounds.Y > e.NewValue ? false : true;

            if (Visiblebounds.Y != e.NewValue)
            {
                Visiblebounds = new Rect(0, e.NewValue, AvailableSize.Width, AvailableSize.Height);

                //Dispatcher.BeginInvoke(new Action<bool>(CalcVisibleItems), top);
                CalcVisibleItems(top);

                if (VisibleLineInfos.Count == 0)
                {
                    bool contains = false;
                    if (Page.ForegroundContainer.Children.Contains(ImageResizer))
                    {
                        contains = true;
                    }
                    Page.ForegroundContainer.Children.Clear();
                    Page.ForegroundContainer.Children.Add(Page.Caret);
                    Page.DecorationContainer.Children.Clear();
                    if (contains)
                        Page.ForegroundContainer.Children.Add(ImageResizer);
                    foreach (LineInfo lineInfo in LineInfos)
                    {
                        if (Visiblebounds.IsIntersecting(lineInfo.BoundingRectangle))
                        {
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Add(lineInfo);
                            }
                            AddLineToPage(lineInfo, Page);
                        }
                        else
                        {
                            RemoveLineFromPage(lineInfo, Page);
                        }
                    }
                }

                TransformGroup renderTransform = new TransformGroup();
                renderTransform.Children.Add(transform);
                renderTransform.Children.Add(scaleTransform);

                page.ForegroundContainer.RenderTransform = renderTransform;
                page.SelectionContainer.RenderTransform = renderTransform;
                page.DecorationContainer.RenderTransform = renderTransform;
            }
        }

        /// <summary>
        /// Updates the horizontal scroll bar
        /// </summary>
        public override void UpdateHorizontalScrollBar()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override bool CheckForScrollBarVisibility()
        {
            bool flag = false;
            if (LineInfos.Count > 0)
            {
                if (LineInfos.Last().BoundingRectangle.Bottom > AvailableSize.Height)
                {
                    if (VerticalScrollBar.Visibility == Visibility.Visible)
                    {
                        flag = false;
                    }
                    else
                    {
                        VerticalScrollBar.Visibility = Visibility.Collapsed;
                        flag = true;
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// Sets the visible lines
        /// </summary>
        public override void SetVisibleLinesToPage()
        {
            Page.Measure(AvailableSize);
            Rect rect = new Rect(0, 0, OriginalSize.Width, OriginalSize.Height);
            UpdateClip(rect);

            UpdateVerticalScrollBar();

            Visiblebounds = new Rect(0, VerticalScrollBar.Value, AvailableSize.Width, AvailableSize.Height);

            VisibleLineInfos.Clear();

            foreach (LineInfo lineInfo in LineInfos)
            {
                if (Visiblebounds.IsIntersecting(lineInfo.BoundingRectangle))
                {
                    if(!VisibleLineInfos.Contains(lineInfo))
                        VisibleLineInfos.Add(lineInfo);
                }
            }
            bool contains = false;
            if (Page.ForegroundContainer.Children.Contains(ImageResizer))
            {
                contains = true;
            }

            //Page.ForegroundContainer.Children.Clear();
            if (!Page.ForegroundContainer.Children.Contains(Page.Caret))
            {
                Page.ForegroundContainer.Children.Add(Page.Caret);
            }
            //Page.DecorationContainer.Children.Clear();

            if (contains)
                if (!Page.ForegroundContainer.Children.Contains(ImageResizer))
                {
                    Page.ForegroundContainer.Children.Add(ImageResizer);
                }

            foreach (LineInfo lineInfo in VisibleLineInfos)
            {

                /******Uncomment the below line to render the text in image******/

                //lineInfo.CreateDrawingContext();

                /******Uncomment the below line to render the text in TextBlock******/

                RenderingManager.Render(lineInfo, Page);

                //foreach (UIElement element in lineInfo.Elements)
                //{
                //    if (!Page.ForegroundContainer.Children.Contains(element))
                //    {
                //        Page.ForegroundContainer.Children.Add(element);
                //    }
                //}
            }
        }

        /// <summary>
        /// Does the virtualization
        /// </summary>
        internal void CalcVisibleItems(bool top)
        {
            List<LineInfo> removedLines = new List<LineInfo>();

            if (top)
            {
                int j = 0;
                for (int i = 0; i < VisibleLineInfos.Count; i++)
                {
                    //if (!(Visiblebounds.Contains(VisibleLineInfos[i].Location) || Visiblebounds.Contains(new Point(VisibleLineInfos[i].BoundingRectangle.Left, VisibleLineInfos[i].BoundingRectangle.Bottom))))
                    if (!(Visiblebounds.IsIntersecting(VisibleLineInfos[i].BoundingRectangle)))
                    {
                        j++;
                        removedLines.Add(VisibleLineInfos[i]);
                        RemoveLineFromPage(VisibleLineInfos[i], Page);
                    }
                }

                foreach (LineInfo line in removedLines)
                {
                    VisibleLineInfos.Remove(line);
                }

                int index = 0;

                if (VisibleLineInfos.Count != 0)
                {
                    index = LineInfos.IndexOf(VisibleLineInfos.Last());
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
                        if (index < LineInfos.Count)
                        {
                            LineInfo lineInfo = LineInfos[index];
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Add(lineInfo);
                            }
                            AddLineToPage(lineInfo, Page);
                        }
                    }

                    while (index < LineInfos.Count && Visiblebounds.IsIntersecting(LineInfos[index].BoundingRectangle))
                    {
                        if (!VisibleLineInfos.Contains(LineInfos[index]))
                        {
                            LineInfo lineInfo = LineInfos[index];
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Add(lineInfo);
                            }
                            AddLineToPage(lineInfo, Page);
                        }

                        ++index;
                    }
                }
                else if (VisibleLineInfos.Count != 0)
                {
                    for (int i = index + 1; i < LineInfos.Count; i++)
                    {
                        LineInfo lineInfo = LineInfos[i];
                        //if (Visiblebounds.Contains(lineInfo.Location) || Visiblebounds.Contains(new Point(lineInfo.BoundingRectangle.Left, lineInfo.BoundingRectangle.Bottom)))
                        if (Visiblebounds.IsIntersecting(lineInfo.BoundingRectangle))
                        {
                            AddLineToPage(lineInfo, Page);
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Add(lineInfo);
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
                for (int i = VisibleLineInfos.Count - 1; i >= 0; i--)
                {
                    //if (!(Visiblebounds.Contains(VisibleLineInfos[i].Location) || Visiblebounds.Contains(new Point(VisibleLineInfos[i].BoundingRectangle.Left, VisibleLineInfos[i].BoundingRectangle.Bottom))))
                    if (!(Visiblebounds.IsIntersecting(VisibleLineInfos[i].BoundingRectangle)))
                    {
                        j++;
                        removedLines.Add(VisibleLineInfos[i]);
                        RemoveLineFromPage(VisibleLineInfos[i], Page);
                    }
                }

                foreach (LineInfo line in removedLines)
                {
                    VisibleLineInfos.Remove(line);
                }

                int index = 0;

                if (VisibleLineInfos.Count != 0)
                {
                    index = LineInfos.IndexOf(VisibleLineInfos.First());
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
                            LineInfo lineInfo = LineInfos[index];
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Insert(0, lineInfo);
                            }
                            AddLineToPage(lineInfo, Page);
                        }
                    }

                    while (index >= 0 && Visiblebounds.IsIntersecting(LineInfos[index].BoundingRectangle))
                    {
                        if (!VisibleLineInfos.Contains(LineInfos[index]))
                        {
                            LineInfo lineInfo = LineInfos[index];
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Insert(0, lineInfo);
                            }
                            AddLineToPage(lineInfo, Page);
                        }

                        --index;
                    }
                }
                else if (VisibleLineInfos.Count != 0)
                {
                    for (int i = index - 1; i >= 0; i--)
                    {
                        LineInfo lineInfo = LineInfos[i];

                        //if (Visiblebounds.Contains(lineInfo.Location) || Visiblebounds.Contains(new Point(lineInfo.BoundingRectangle.Left, lineInfo.BoundingRectangle.Bottom)))
                        if (Visiblebounds.IsIntersecting(lineInfo.BoundingRectangle))
                        {
                            if (!VisibleLineInfos.Contains(lineInfo))
                            {
                                VisibleLineInfos.Add(lineInfo);
                            }
                            AddLineToPage(lineInfo, Page);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        internal void Select(Point startPt, Point endPt)
        {
            OwnerControl.Selection.StartPoint = startPt;
            OwnerControl.Selection.EndPoint = endPt;
            List<LineInfo> lineInfos = Page.GenerateLineInfoForSelection();
            OwnerControl.Selection.SelectedLines = lineInfos;
            Point startPoint = OwnerControl.Selection.StartPoint;
            Point endPoint = OwnerControl.Selection.EndPoint;
            if ((OwnerControl.Selection.StartPoint.Y > OwnerControl.Selection.EndPoint.Y && lineInfos.Count != 1)
                || ((OwnerControl.Selection.StartPoint.Y == OwnerControl.Selection.EndPoint.Y || lineInfos.Count == 1) && OwnerControl.Selection.StartPoint.X > OwnerControl.Selection.EndPoint.X))
            {
                startPoint = OwnerControl.Selection.EndPoint;
                endPoint = OwnerControl.Selection.StartPoint;
            }
            OwnerControl.Selection.StartingInline = Page.GetInlineFromPoint(startPoint);
            OwnerControl.Selection.EndingInline = Page.GetInlineFromPoint(endPoint);
            OwnerControl.Selection.StartingIndex = Page.GetIndexFromPoint(startPoint);
            OwnerControl.Selection.EndingIndex = Page.GetIndexFromPoint(endPoint);
            OwnerControl.Selection.ExtractSelectedText();
            Path path = null;
            if (!(lineInfos.Count == 1 && (lineInfos[0].BoundingRectangle.Width == 0 || string.IsNullOrEmpty(OwnerControl.Selection.Text))))
            {
                path = OwnerControl.Selection.GetSelectionPathForLineInfo(lineInfos);
            }
            else
            {
                if (Page.SelectionContainer.Children.Contains(Page.SelectionPath))
                {
                    Page.SelectionContainer.Children.Remove(Page.SelectionPath);
                }
            }
            if (path != null)
            {
                if (Page.SelectionContainer.Children.Contains(Page.SelectionPath))
                {
                    Page.SelectionContainer.Children.Remove(Page.SelectionPath);
                }
                Page.SelectionPath = path;
                Page.SelectionContainer.Children.Add(Page.SelectionPath);
                Canvas.SetLeft(Page.SelectionPath, 0);
                Canvas.SetTop(Page.SelectionPath, 0);
            }
        }

        /// <summary>
        /// Updates the clip
        /// </summary>
        /// <param name="rect"></param>
        public void UpdateClip(Rect rect)
        {
            RectangleGeometry rectGeo = new RectangleGeometry();
            rectGeo.Rect = rect;
            Clip = rectGeo;
        }

        /// <summary>
        /// Arranges elements
        /// </summary>
        public override void ArrangeElements()
        {
            LineInfos.Clear();
            Page.LineInfos.Clear();
            Page.ForegroundContainer.Children.Clear();
            Page.DecorationContainer.Children.Clear();
            Page.SelectionContainer.Children.Clear();
            VisibleLineInfos.Clear();
            bool isBreak = false;

            foreach (SectionAdv section in Document.Sections)
            {
                if (isBreak)
                    break;
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
                    block.ArrangeElements();
                    block.IsArranged = false;
                    if (OnLoading)
                    {
                        if (OwnerControl.VerticalScrollBarVisibility)
                            isBreak = CheckForScrollBarVisibility();
                        else
                            isBreak = false;
                        if (isBreak)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Arranges the element
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Sets previous and next paragraph
        /// </summary>
        //public override void SetPreviousBlocks()
        //{
        //    foreach (SectionAdv section in Document.Sections)
        //    {
        //        if (section.Blocks.Count > 0)// && Document.Sections.First() == section)
        //        {
        //            section.Blocks[0].PreviousBlock = null;
        //        }
        //        //else if(section.Blocks.Count > 0)
        //        //{
        //        //    section.Blocks[0].PreviousBlock = previousPara;
        //        //    previousPara.NextBlock = section.Blocks[0].PreviousBlock;
        //        //}
        //        for (int i = 1; i < section.Blocks.Count; i++)
        //        {
        //            section.Blocks[i].PreviousBlock = section.Blocks[i - 1];
        //        }

        //        for (int i = 0; i < section.Blocks.Count; i++)
        //        {
        //            section.Blocks[i].NextBlock = null;
        //            if (i + 1 != section.Blocks.Count)
        //            {
        //                section.Blocks[i].NextBlock = section.Blocks[i + 1];
        //            }
        //        }
        //    }
        //}

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal override void FindFocusedPage(MouseEventArgs e)
        {
            CurrentPage = Pages[0];
        }
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(TouchEventArgs e)
        {
            CurrentPage = Pages[0];
        }
#endif
    }
}
