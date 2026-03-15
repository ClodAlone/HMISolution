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
using System.Windows.Input;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
#if WPF
using System.Windows.Controls;
using System.Windows.Media;
using RangeBaseValueChangedEventHandlerInternal = System.Windows.RoutedPropertyChangedEventHandler<double>;
using RangeBaseValueChangedEventArgsInternal = System.Windows.RoutedPropertyChangedEventArgs<double>;
using System.Windows.Media.Imaging;
using System.IO;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.System;
using Windows.UI.Xaml.Input;
using RangeBaseValueChangedEventHandlerInternal = Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventHandler;
using RangeBaseValueChangedEventArgsInternal = Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs;
using Windows.UI.Core;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class PageLayoutViewer : LayoutViewer
    {
        #region Fields
        internal double PageGap = 20;
        internal List<PageAdv> VisiblePages;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PageLayoutViewer" /> class.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        internal PageLayoutViewer(SfRichTextBoxAdv richTextBox)
            : base(richTextBox)
        {
            //Adds the grid to display current loading page in Asychnronous open.
            Children.Add(richTextBox.CurrentLoadingPagePopup);
            VisiblePages = new List<PageAdv>();
#if WPF
            this.MouseWheel += new MouseWheelEventHandler(PageLayoutViewer_MouseWheel);
#else
#endif
#if WPF && SyncfusionFramework4_0
            this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(PageLayoutViewer_ManipulationDelta);
            this.IsManipulationEnabled = richTextBox.IsManipulationEnabled;
#endif
        }
        #endregion

        #region Scrollbar events
        /// <summary>
        /// Handles the horizontal scroll bar value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal override void HorizontalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgsInternal e)
        {
            if (IsLoadingPreloadPages() || OwnerControl.LayoutType == LayoutType.Block)
            {
                HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                HorizontalScrollBar.Value = e.OldValue;
                HorizontalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                return;
            }
            transform.X = -HorizontalScrollBar.Value;
            Container.RenderTransform = transform;
            Visiblebounds = new Rect(e.NewValue, Visiblebounds.Y, Visiblebounds.Width, Visiblebounds.Height);
        }
        /// <summary>
        /// Handles the vertical scroll bar value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal override void VerticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgsInternal e)
        {
            if (IsLoadingPreloadPages() || OwnerControl.LayoutType == LayoutType.Block)
            {
                VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                VerticalScrollBar.Value = e.OldValue;
                VerticalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                return;
            }
            DoVerticalScroll(e);
        }

        /// <summary>
        /// Does the vertical scroll.
        /// </summary>
        /// <param name="e">The e.</param>
        private void DoVerticalScroll(RangeBaseValueChangedEventArgsInternal e)
        {
            transform.Y = -VerticalScrollBar.Value;

            bool top = Visiblebounds.Y > e.NewValue ? false : true;
#if DEBUG
            int startIndex = 0, endIndex = 0;
            DateTime startTime = DateTime.Now;
            if (VisiblePages.Count > 0)
            {
                if (top)
                    startIndex = Pages.IndexOf(VisiblePages[0]);
                else
                    startIndex = Pages.IndexOf(VisiblePages[VisiblePages.Count - 1]);
            }
#endif
            if (Visiblebounds.Y != e.NewValue)
            {
                Visiblebounds = new Rect(Visiblebounds.X, e.NewValue, Visiblebounds.Width, Visiblebounds.Height);
                UpdateVisiblePages(top);
                Container.RenderTransform = transform;
                UpdateCaretToPage();
            }
#if DEBUG
            if (VisiblePages.Count > 0)
            {
                if (top)
                    endIndex = Pages.IndexOf(VisiblePages[0]);
                else
                    endIndex = Pages.IndexOf(VisiblePages[VisiblePages.Count - 1]);
            }
            this.OwnerControl.PerformanceInfo.PanningPagesCount = endIndex - startIndex;
            this.OwnerControl.PerformanceInfo.PageLoadTimeOnPanning = DateTime.Now - startTime;
#endif
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Creates the new page.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        internal override PageAdv CreateNewPage(SectionAdv section)
        {
            PageAdv page = null;
#if !WPF
            UIDispatcher.Execute(() =>
                {
#endif
                    double yPos = PageGap;
                    page = new PageAdv();
                    if (CurrentRenderingPage != null)
                        yPos = CurrentRenderingPage.BoundingRectangle.Bottom + PageGap;
                    page.Background = new SolidColorBrush(section.Document.Background);
                    page.Viewer = this;
                    page.Section = section;
                    SectionFormat sectionFormat = section.SectionFormat;
                    page.Width = sectionFormat.PageSize.Width;
                    page.Height = sectionFormat.PageSize.Height;
                    double xPos = (Visiblebounds.Width - page.Width) / 2;
                    if (xPos < 30)
                        xPos = 30;
                    page.BoundingRectangle = new Rect(xPos, yPos, page.Width, page.Height);
                    if (Pages.Count == 0)
                    {
                        CurrentPage = page;
                        HorizontalWidth = page.BoundingRectangle.Width;
                    }
                    if (OwnerControl.CurrentLoadingPagePopup.Visibility == Visibility.Visible
                        && Pages.Count >= OwnerControl.PreloadPageCount)
                    {
                        if (Pages.Count == OwnerControl.PreloadPageCount)
                            LoadPreloadPages();
                        if (AsyncLoadedPages.Count == OwnerControl.AsyncPageLoadCount)
                            LoadAsyncPages();
                        Pages.Add(page);
                        AsyncLoadedPages.Add(page);
                    }
                    else
                    {
                        Pages.Add(page);
                        if (OwnerControl.CurrentLoadingPagePopup.Visibility == Visibility.Visible)
                        {
                            if (Pages.Count == 1)
                            {
                                OwnerControl.CurrentLoadingPageBlock.Text = Pages.Count.ToString();
                                PageAdv emptyPage = new PageAdv();
                                emptyPage.SetBackground(OwnerControl);
                                emptyPage.Width = page.Width;
                                emptyPage.Height = page.Height;
                                emptyPage.BoundingRectangle = page.BoundingRectangle;
                                //Adds an empty page in viewer, to preserve empty page till rendered pages exceed PreloadPageCount.
                                UpdatePageToViewer(emptyPage);
                            }
                        }
                        else
                            UpdatePageToViewer(page);
                    }
                    //Renders the header footer.
                    LayoutHeaderFooter(section);
                    //Updates the client area.
                    UpdateClientArea(sectionFormat);
                    //Adds new body widget for the content layouted in new page.
                    page.BodyWidgets.Add(section.AddBodyWidget(ClientActiveArea));
#if !WPF
                });
#endif
            return page;
        }
        /// <summary>
        /// Adds the empty page.
        /// </summary>
        internal override void AddEmptyPage()
        {
            PageAdv emptyPage = new PageAdv();
            emptyPage.SetBackground(OwnerControl);
            emptyPage.Width = 816;
            emptyPage.Height = 1056;
            double xPos = (Visiblebounds.Width - 816) / 2;
            if (xPos < 30)
                xPos = 30;
            emptyPage.BoundingRectangle = new Rect(xPos, PageGap, 816, 1056);
            //Adds an empty page in viewer, to preserve empty page till rendered pages exceed PreloadPageCount.
            UpdatePageToViewer(emptyPage);
        }
        /// <summary>
        /// Zooms this instance.
        /// </summary>
        internal override void Zoom()
        {
            if (ScaleFactor == scaleTransform.ScaleY)
                return;
            double prevScaleFactor = scaleTransform.ScaleY;
            scaleTransform.ScaleX = scaleTransform.ScaleY = ScaleFactor;
            if (TouchStart != null)
            {
                TouchStart.Width = 18.0 / ScaleFactor;
                TouchStart.Height = 18.0 / ScaleFactor;
                TouchStart.StrokeThickness = 1.5 / ScaleFactor;
            }
            if (TouchEnd != null)
            {
                TouchEnd.Width = 18.0 / ScaleFactor;
                TouchEnd.Height = 18.0 / ScaleFactor;
                TouchEnd.StrokeThickness = 1.5 / ScaleFactor;
            }
            if (VerticalScrollBar != null && HorizontalScrollBar != null)
            {
                PageAdv page = null;
                double height = (VerticalHeight * ScaleFactor + (Pages.Count + 1) * PageGap * (1 - ScaleFactor)) - Visiblebounds.Height;
                if (height > 0)
                {
                    double value = VerticalScrollBar.Value;
                    if (VisiblePages.Count > 0)
                    {
                        page = VisiblePages[0];
                        double prevPageTop = (page.BoundingRectangle.Top - (Pages.IndexOf(page) + 1) * PageGap) * prevScaleFactor + (Pages.IndexOf(page) + 1) * PageGap;
                        if (double.IsNaN(zoomY))
                            zoomY = Visiblebounds.Height / 2;
                        double prevY = value + zoomY;
                        while (prevY > prevPageTop + page.Height)
                        {
                            int pageIndex = Pages.IndexOf(page) + 1;
                            if (pageIndex == Pages.Count)
                                break;
                            page = Pages[pageIndex];
                            prevPageTop = (page.BoundingRectangle.Top - (Pages.IndexOf(page) + 1) * PageGap) * prevScaleFactor + (Pages.IndexOf(page) + 1) * PageGap;
                        }
                        double currentY = (page.BoundingRectangle.Top - (Pages.IndexOf(page) + 1) * PageGap) * ScaleFactor + (Pages.IndexOf(page) + 1) * PageGap
                            + ((prevY - prevPageTop) < 0 ? prevY - prevPageTop : (prevY - prevPageTop) * (ScaleFactor / prevScaleFactor));
                        value = currentY - zoomY;
                        zoomY = Visiblebounds.Height / 2;
                    }
                    VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                    VerticalScrollBar.Maximum = height;
                    VerticalScrollBar.Minimum = 0;
                    VerticalScrollBar.ViewportSize = Visiblebounds.Height;
                    VerticalScrollBar.SmallChange = 20;
                    VerticalScrollBar.LargeChange = (Visiblebounds.Height - PageGap);
                    VerticalScrollBar.Value = value;
                    Visiblebounds = new Rect(Visiblebounds.X, VerticalScrollBar.Value, Visiblebounds.Width, Visiblebounds.Height);
                    transform.Y = -VerticalScrollBar.Value;
                    if (OwnerControl.VerticalScrollBarVisibility)
                        VerticalScrollBar.Visibility = Visibility.Visible;
                    VerticalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                }
                else
                {
                    VerticalScrollBar.Visibility = Visibility.Collapsed;
                    VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                    VerticalScrollBar.Value = 0;
                    transform.Y = 0;
                }
                double horWidth = HorizontalWidth * ScaleFactor - Visiblebounds.Width;
                if (Visiblebounds.Width - HorizontalWidth * ScaleFactor < 60)
                    horWidth += 60;
                if (horWidth > 0)
                {
                    double value = HorizontalScrollBar.Value;
                    if (VisiblePages.Count > 0)
                    {
                        if (page == null)
                            page = VisiblePages[0];
                        if (double.IsNaN(zoomX))
                            zoomX = Visiblebounds.Width / 2;
                        double prevValue = page.Width / page.BoundingRectangle.Width;
                        double prevX = value + zoomX;
                        double currentX = page.BoundingRectangle.Left
                            + ((prevX - page.BoundingRectangle.Left) < 0 ? prevX - page.BoundingRectangle.Left : (prevX - page.BoundingRectangle.Left) * (ScaleFactor / prevValue));
                        value = currentX - zoomX;
                        zoomX = Visiblebounds.Width / 2;
                    }
                    HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                    HorizontalScrollBar.Maximum = horWidth;
                    HorizontalScrollBar.Minimum = 0;
                    HorizontalScrollBar.ViewportSize = Visiblebounds.Width;
                    HorizontalScrollBar.SmallChange = (HorizontalWidth * ScaleFactor) / Visiblebounds.Width;
                    HorizontalScrollBar.LargeChange = HorizontalScrollBar.SmallChange * 2;
                    HorizontalScrollBar.Value = value;
                    Visiblebounds = new Rect(HorizontalScrollBar.Value, Visiblebounds.Y, Visiblebounds.Width, Visiblebounds.Height);
                    transform.X = -HorizontalScrollBar.Value;
                    HorizontalScrollBar.Visibility = Visibility.Visible;
                    HorizontalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                }
                else
                {
                    HorizontalScrollBar.Visibility = Visibility.Collapsed;
                    HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                    HorizontalScrollBar.Value = 0;
                    transform.X = 0;
                }
                Container.RenderTransform = transform;
            }
            UpdateVisiblePagesAfterZoom();
        }
        /// <summary>
        /// Determines whether loading preload pages.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if loading preload pages; otherwise, <c>false</c>.
        /// </returns>
        internal override bool IsLoadingPreloadPages()
        {
            //Returns true; if currently disposing previous document,
            //Or pages layouted in new document is less than preload pages count,
            //Or currently DocIO is parsing file. 
            return OwnerControl.Document == null || !OwnerControl.IsDocumentLoaded
                && Pages.Count < OwnerControl.PreloadPageCount
                && (VisiblePages.Count == 0 || !Pages.Contains(VisiblePages[0]));
        }
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(MouseEventArgs e)
        {
#if WPF
            Point point = e.GetPosition(this);
#else
            Point point = new Point(e.MouseDelta.X, e.MouseDelta.Y);
#endif
            if (Pages.Count > 0)
                CurrentPage = GetPage(point);
        }
#if !WPF
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(PointerRoutedEventArgs e)
        {
            Point point = e.GetCurrentPoint(this.Container).Position;
            if (Pages.Count > 0)
                CurrentPage = GetPage(point);
        }
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TappedRoutedEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(TappedRoutedEventArgs e)
        {
            Point point = e.GetPosition(this.Container);
            if (Pages.Count > 0)
                CurrentPage = GetPage(point);
        }
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="DoubleTappedRoutedEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(DoubleTappedRoutedEventArgs e)
        {
            Point point = e.GetPosition(this.Container);
            if (Pages.Count > 0)
                CurrentPage = GetPage(point);
        }
        /// <summary>
        /// Updates the core cursor.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        internal override void UpdateCoreCursor(PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(this.Container);
            if (pointerPoint.PointerDevice.PointerDeviceType != PointerDeviceType.Mouse)
                return;
            Point point = pointerPoint.Position;
            for (int i = 0; i < Pages.Count; i++)
            {
                PageAdv page = Pages[i];
                double pageBottom = page.BoundingRectangle.Top * ScaleFactor + (i + 1) * PageGap * (1 - ScaleFactor) + page.BoundingRectangle.Height * ScaleFactor;
                if (pageBottom >= point.Y || i == Pages.Count - 1)
                {
                    pointerPoint = e.GetCurrentPoint(page.ForegroundContainer);
                    LineWidget currentLineWidget = GetLineWidget(pointerPoint.Position);
                    FieldBeginAdv hyperlinkField = null;
                    double lineLeft = 0;
                    if (currentLineWidget != null)
                    {
                        lineLeft = currentLineWidget.GetLineStartLeft();
                        hyperlinkField = currentLineWidget.GetHyperlinkField(OwnerControl, pointerPoint.Position);
                    }
#if WPF
#else
                    if (hyperlinkField != null && pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse
                        && (OwnerControl.ModifierKey & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control)
                        OwnerControl.Cursor = new CoreCursor(CoreCursorType.Hand, 0);
                    else if ((OwnerControl.Selection.IsEmpty || ImageResizer.Visibility == Visibility.Visible)
                        && pointerPoint.Position.X >= lineLeft)
                        OwnerControl.Cursor = new CoreCursor(CoreCursorType.IBeam, 0);
                    else
                        OwnerControl.Cursor = new CoreCursor(CoreCursorType.Arrow, 0);
#endif
                    return;
                }
            }
#if WPF
#else
            OwnerControl.Cursor = new CoreCursor(CoreCursorType.Arrow, 0);
#endif
        }
#endif
#if WPF && SyncfusionFramework4_0
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(TouchEventArgs e)
        {
            Point point = e.GetTouchPoint(Container).Position;
            //if (Pages.Count > 0)
            //{
            //    CurrentPage = GetPage(0, Pages.Count - 1, point);
            //}
        }
#endif
        /// <summary>
        /// Renders the visible pages.
        /// </summary>
        internal override void RenderVisiblePages()
        {
            foreach (PageAdv page in VisiblePages)
            {
                page.RemoveWidgets();
                page.RenderWidgets();
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            ClearVisiblePages();
            VisiblePages = null;
#if WPF
            this.MouseWheel -= new MouseWheelEventHandler(PageLayoutViewer_MouseWheel);
#else
#endif
#if WPF && SyncfusionFramework4_0
            this.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(PageLayoutViewer_ManipulationDelta);
#endif
            base.Dispose();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Loads the preload pages.
        /// </summary>
        internal void LoadPreloadPages()
        {
            VisiblePages.Clear();
            Container.Children.Clear();
            for (int i = 0; i < Pages.Count; i++)
            {
                PageAdv page = Pages[i];
                UpdatePageToViewer(page);
            }
            OwnerControl.CurrentLoadingPageBlock.Text = Pages.Count.ToString();
        }
        /// <summary>
        /// Loads the async pages.
        /// </summary>
        internal void LoadAsyncPages()
        {
            if (Pages.Count <= OwnerControl.PreloadPageCount)
                LoadPreloadPages();
            for (int i = 0; i < AsyncLoadedPages.Count; i++)
            {
                PageAdv page = AsyncLoadedPages[i];
                AsyncLoadedPages.RemoveAt(i);
                i--;
                UpdatePageToViewer(page);
            }
            OwnerControl.CurrentLoadingPageBlock.Text = Pages.Count.ToString();
        }
        /// <summary>
        /// Layouts the header footer.
        /// </summary>
        /// <param name="section">The section.</param>
        private void LayoutHeaderFooter(SectionAdv section)
        {
            //Renders the header footer.
            CurrentHeaderFooter = GetCurrentPageHeaderFooter(section, true);
            if (CurrentHeaderFooter.Blocks.Count > 0)
            {
                UpdateHFClientArea(section, true);
                CurrentRenderingPage.HeaderWidget = CurrentHeaderFooter.LayoutItems(this);
            }
            CurrentHeaderFooter = GetCurrentPageHeaderFooter(section, false);
            if (CurrentHeaderFooter.Blocks.Count > 0)
            {
                UpdateHFClientArea(section, false);
                CurrentRenderingPage.FooterWidget = CurrentHeaderFooter.LayoutItems(this);
            }
            CurrentHeaderFooter = null;
        }
        /// <summary>
        /// Gets the current page header footer.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="isHeader">if set to <c>true</c> [is header].</param>
        /// <returns></returns>
        private HeaderFooter GetCurrentPageHeaderFooter(SectionAdv section, bool isHeader)
        {
            //Deafult/Odd header footer.
            HeaderFooterType type = isHeader ? HeaderFooterType.OddHeader : HeaderFooterType.OddFooter;
            if (section.SectionFormat.DifferentFirstPage
                && (Pages.Count == 1 || Pages[Pages.Count - 2].Section != section))
                //First page header footer.
                type = isHeader ? HeaderFooterType.FirstPageHeader : HeaderFooterType.FirstPageFooter;
            else if (section.SectionFormat.DifferentOddAndEvenPages && Pages.Count % 2 == 0)
                //Even header footer.
                type = isHeader ? HeaderFooterType.EvenHeader : HeaderFooterType.EvenFooter;
            return section.GetCurrentHeaderFooter(type);
        }
        /// <summary>
        /// Updates the page to viewer.
        /// </summary>
        /// <param name="page">The page.</param>
        private void UpdatePageToViewer(PageAdv page)
        {
            double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
            if (x < 30)
                x = 30;
            if (page.BoundingRectangle.Width < HorizontalWidth)
                x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
            double y = page.BoundingRectangle.Top * ScaleFactor + Pages.Count * PageGap * (1 - ScaleFactor);
            if (Visiblebounds.IsIntersecting(new Rect(x, y, page.BoundingRectangle.Width * ScaleFactor, page.BoundingRectangle.Height * ScaleFactor)))
                AddVisiblePage(page, x, y, false);
            if (HorizontalWidth < page.BoundingRectangle.Width)
            {
                HorizontalWidth = page.BoundingRectangle.Width;
                UpdatePageLeft();
            }
            VerticalHeight = page.BoundingRectangle.Bottom + PageGap;
            UpdateScrollBars();
        }
        /// <summary>
        /// Updates the page left.
        /// </summary>
        private void UpdatePageLeft()
        {
            for (int i = 0; i < VisiblePages.Count; i++)
            {
                PageAdv page = VisiblePages[i];
                double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                if (x < 30)
                    x = 30;
                if (page.BoundingRectangle.Width < HorizontalWidth)
                    x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
                if (!double.IsInfinity(x))
                    Canvas.SetLeft(page, x);
            }
        }
        /// <summary>
        /// Clears the visible pages.
        /// </summary>
        internal void ClearVisiblePages()
        {
            for (int i = 0; i < VisiblePages.Count; i++)
            {
                VisiblePages[i].RemoveWidgets();
                VisiblePages.RemoveAt(i);
                i--;
            }
        }
#if WPF
        /// <summary>
        /// Saves the pages as image.
        /// </summary>
        /// <param name="outputFilePath">The output file path.</param>
        internal void SavePagesAsImage(string outputFilePath)
        {
            if (string.IsNullOrEmpty(outputFilePath.Trim()))
                outputFilePath = "";
            else if (!outputFilePath.EndsWith("\\"))
                outputFilePath = outputFilePath + "\\";
            string fileName = "RTEDocument";
            if (!string.IsNullOrEmpty(OwnerControl.DocumentTitle))
                fileName = OwnerControl.DocumentTitle;
            for (int i = 0; i < Pages.Count; i++)
            {
                if (VisiblePages.Contains(Pages[i]))
                {
                    if (scaleTransform.ScaleX != 1)
                    {
                        //Sets the page to normal (100%) scaling factor.
                        Pages[i].Width = Pages[i].BoundingRectangle.Width;
                        Pages[i].Height = Pages[i].BoundingRectangle.Height;
                        ScaleTransform scale = new ScaleTransform();
                        scale.ScaleX = scale.ScaleY = 1;
                        Pages[i].ForegroundContainer.RenderTransform = scale;
                        Pages[i].DecorationContainer.RenderTransform = scale;
                    }
                    Pages[i].HidePageNumber();
                }
                else
                    Pages[i].RenderWidgets();
                // Render the UIElement into a writeable bitmap
                BitmapSource bitmapSource = CreateBitmap(Pages[i], false);
                if (VisiblePages.Contains(Pages[i]))
                {
                    if (Container.Children.Contains(Pages[i]))
                        Container.Children.Remove(Pages[i]);
                    Container.Children.Add(Pages[i]);
                    if (scaleTransform.ScaleX != 1)
                    {
                        //Resets the page to current scaling factor.
                        Pages[i].Width = Pages[i].BoundingRectangle.Width * scaleTransform.ScaleX;
                        Pages[i].Height = Pages[i].BoundingRectangle.Height * scaleTransform.ScaleY;
                        Pages[i].ForegroundContainer.RenderTransform = scaleTransform;
                        Pages[i].DecorationContainer.RenderTransform = scaleTransform;
                    }
                }
                else
                    Pages[i].RemoveWidgets();
                string imageName = Path.GetFullPath(outputFilePath + fileName + "_Page_" + (i + 1).ToString() + ".bmp");
                using (FileStream fileStream = new FileStream(imageName, FileMode.Create))
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(fileStream);
                    fileStream.Dispose();
                }
            }
        }
        /// <summary>
        /// Creates the bitmap.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isInUiTree">if set to <c>true</c> [is in UI tree].</param>
        /// <returns></returns>
        private BitmapSource CreateBitmap(FrameworkElement element, bool isInUiTree)
        {
            if (!isInUiTree)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));
            }

            int width = (int)Math.Ceiling(element.DesiredSize.Width);
            int height = (int)Math.Ceiling(element.DesiredSize.Height);

            width = width == 0 ? 1 : width;
            height = height == 0 ? 1 : height;

            RenderTargetBitmap rtbmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            rtbmp.Render(element);
            return rtbmp;
        }
        /// <summary>
        /// Handles the MouseWheel event of the PageLayoutViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        void PageLayoutViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (Keyboard.Modifiers == ModifierKeys.Control && OwnerControl.IsZoomEnabled)
            {
                if (e.Delta < 0)
                {
                    if (ScaleFactor >= 0.25)
                        ScaleFactor -= 0.05;
                }
                else
                {
                    if (ScaleFactor <= 4)
                        ScaleFactor += 0.05;
                }
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = ScaleFactor / 4;
                Zoom();
            }
            else
            {
                if (e.Delta < 0)
                    VerticalScrollBar.Value = VerticalScrollBar.Value + VerticalScrollBar.SmallChange;
                else
                    VerticalScrollBar.Value = VerticalScrollBar.Value - VerticalScrollBar.SmallChange;
            }
        }
#endif
#if WPF && SyncfusionFramework4_0
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
                    if (ScaleFactor >= 0.25)
                        ScaleFactor -= 0.05;
                }
                else if (e.DeltaManipulation.Scale.Length > 1.4142135623730951)
                {
                    if (ScaleFactor <= 4)
                        ScaleFactor += 0.05;
                }
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = ScaleFactor / 4;
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
        /// Updates the visible pages after zoom.
        /// </summary>
        private void UpdateVisiblePagesAfterZoom()
        {
            int startIndex = 0;
            if (VisiblePages.Count > 0)
                startIndex = GetVisiblePageStartIndex();
            ClearVisiblePages();
            Container.Children.Clear();
            if (ImageResizer.Visibility == Visibility.Visible)
            {
                if (OwnerControl.Selection.IsForward)
                    PositionImageResizer(ImageResizer.CurrentImageElementBox, OwnerControl.Selection.Start, OwnerControl.Selection.End);
                else
                    PositionImageResizer(ImageResizer.CurrentImageElementBox, OwnerControl.Selection.End, OwnerControl.Selection.Start);
            }
            bool started = false;
            for (int i = startIndex; i < Pages.Count; i++)
            {
                PageAdv page = Pages[i];
                double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                if (x < 30)
                    x = 30;
                if (page.BoundingRectangle.Width < HorizontalWidth)
                    x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
                double y = page.BoundingRectangle.Top * ScaleFactor + (i + 1) * PageGap * (1 - ScaleFactor);
                if (Visiblebounds.IsIntersecting(new Rect(x, y, page.BoundingRectangle.Width * ScaleFactor, page.BoundingRectangle.Height * ScaleFactor)))
                {
                    started = true;
                    AddVisiblePage(page, x, y, false);
                }
                else if (started)
                    break;
            }
        }
        /// <summary>
        /// Updates the scroll bars.
        /// </summary>
        internal override void UpdateScrollBars()
        {
            if (VerticalScrollBar != null && HorizontalScrollBar != null)
            {
                double height = (VerticalHeight * ScaleFactor + (Pages.Count + 1) * PageGap * (1 - ScaleFactor)) - Visiblebounds.Height;
                if (height > 0)
                {
                    VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                    if (OwnerControl.VerticalScrollBarVisibility)
                        VerticalScrollBar.Visibility = Visibility.Visible;
                    VerticalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                    VerticalScrollBar.Maximum = height;
                    VerticalScrollBar.Minimum = 0;
                    VerticalScrollBar.ViewportSize = Visiblebounds.Height;
                    VerticalScrollBar.SmallChange = 20;
                    VerticalScrollBar.LargeChange = (Visiblebounds.Height - PageGap);
                }
                else
                {
                    VerticalScrollBar.Maximum = 0;
                    VerticalScrollBar.Visibility = Visibility.Collapsed;
                    VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                    transform.Y = 0;
                    Container.RenderTransform = transform;
                }
                double horWidth = HorizontalWidth * ScaleFactor - Visiblebounds.Width;
                if (Visiblebounds.Width - HorizontalWidth * ScaleFactor < 60)
                    horWidth += 60;
                if (horWidth > 0)
                {
                    HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                    HorizontalScrollBar.Visibility = Visibility.Visible;
                    HorizontalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                    HorizontalScrollBar.Maximum = horWidth;
                    HorizontalScrollBar.Minimum = 0;
                    HorizontalScrollBar.ViewportSize = Visiblebounds.Width;
                    HorizontalScrollBar.SmallChange = HorizontalWidth * ScaleFactor / Visiblebounds.Width;
                    HorizontalScrollBar.LargeChange = HorizontalScrollBar.SmallChange * 2;
                }
                else
                {
                    HorizontalScrollBar.Maximum = 0;
                    HorizontalScrollBar.Visibility = Visibility.Collapsed;
                    HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                    transform.X = 0;
                    Container.RenderTransform = transform;
                }
            }
        }
        /// <summary>
        /// Gets the start index of the visible page.
        /// </summary>
        /// <returns></returns>
        private int GetVisiblePageStartIndex()
        {
            PageAdv page = VisiblePages[0];
            int startIndex = Pages.IndexOf(page);
            double y = page.BoundingRectangle.Top * ScaleFactor + (startIndex + 1) * PageGap * (1 - ScaleFactor);
            while (Visiblebounds.Top < y)
            {
                if (startIndex == 0)
                    break;
                startIndex--;
                page = Pages[startIndex];
                y = page.BoundingRectangle.Top * ScaleFactor + (startIndex + 1) * PageGap * (1 - ScaleFactor);
            }
            return startIndex;
        }
        /// <summary>
        /// Gets the last index of the visible page.
        /// </summary>
        /// <returns></returns>
        private int GetVisiblePageLastIndex()
        {
            PageAdv page = VisiblePages[VisiblePages.Count - 1];
            int lastIndex = Pages.IndexOf(page);
            double pageBottom = page.BoundingRectangle.Top * ScaleFactor + (lastIndex + 1) * PageGap * (1 - ScaleFactor)
                + page.BoundingRectangle.Height * ScaleFactor + PageGap * ScaleFactor;
            while (Visiblebounds.Bottom > pageBottom)
            {
                if (lastIndex == Pages.Count - 1)
                    break;
                lastIndex++;
                page = Pages[lastIndex];
                pageBottom = page.BoundingRectangle.Top * ScaleFactor + (lastIndex + 1) * PageGap * (1 - ScaleFactor)
                    + page.BoundingRectangle.Height * ScaleFactor + PageGap * ScaleFactor;
            }
            return lastIndex;
        }
        /// <summary>
        /// Updates the visible pages.
        /// </summary>
        /// <param name="top">if set to <c>true</c> [top].</param>
        internal void UpdateVisiblePages(bool top)
        {
            if (top)
            {
                int startIndex = 0;
                if (VisiblePages.Count > 0 && Pages.Contains(VisiblePages[0]))
                    startIndex = GetVisiblePageStartIndex();
                ClearVisiblePages();
                Container.Children.Clear();
                bool started = false;
                for (int i = startIndex; i < Pages.Count; i++)
                {
                    PageAdv page = Pages[i];
                    double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                    if (x < 30)
                        x = 30;
                    if (page.BoundingRectangle.Width < HorizontalWidth)
                        x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
                    double y = page.BoundingRectangle.Top * ScaleFactor + (i + 1) * PageGap * (1 - ScaleFactor);
                    if (Visiblebounds.IsIntersecting(new Rect(x, y, page.BoundingRectangle.Width * ScaleFactor, page.BoundingRectangle.Height * ScaleFactor)))
                    {
                        started = true;
                        AddVisiblePage(page, x, y, false);
                    }
                    else if (started)
                        break;
                }
            }
            else
            {
                int lastIndex = Pages.Count - 1;
                if (VisiblePages.Count > 0 && Pages.Contains(VisiblePages[VisiblePages.Count - 1]))
                    lastIndex = GetVisiblePageLastIndex();
                ClearVisiblePages();
                Container.Children.Clear();
                bool started = false;
                for (int i = lastIndex; i >= 0; i--)
                {
                    PageAdv page = Pages[i];
                    double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                    if (x < 30)
                        x = 30;
                    if (page.BoundingRectangle.Width < HorizontalWidth)
                        x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
                    double y = page.BoundingRectangle.Top * ScaleFactor - (i + 1) * PageGap * (ScaleFactor - 1);
                    if (Visiblebounds.IsIntersecting(new Rect(x, y, page.BoundingRectangle.Width * ScaleFactor, page.BoundingRectangle.Height * ScaleFactor)))
                    {
                        started = true;
                        AddVisiblePage(page, x, y, true);
                    }
                    else if (started)
                        break;
                }
            }
        }
        /// <summary>
        /// Adds the visible page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="addAsInitial">if set to <c>true</c> [add as initial].</param>
        private void AddVisiblePage(PageAdv page, double x, double y, bool addAsInitial)
        {
            Canvas.SetLeft(page, x);
            Canvas.SetTop(page, y);
            page.Width = page.BoundingRectangle.Width * ScaleFactor;
            page.Height = page.BoundingRectangle.Height * ScaleFactor;
            //Clips the content extending beyond page bounds.
            page.Clip = new RectangleGeometry();
#if WPF
            (page.Clip as RectangleGeometry).Rect = new Rect(0, 0, page.Width, page.Height);
#else
            page.Clip.Rect = new Rect(0, 0, page.Width, page.Height);
#endif
            page.ForegroundContainer.RenderTransform = scaleTransform;
            page.DecorationContainer.RenderTransform = scaleTransform;
            if (!VisiblePages.Contains(page))
            {
                if (addAsInitial)
                    VisiblePages.Insert(0, page);
                else
                    VisiblePages.Add(page);
                page.RenderWidgets();
            }
            //Adds image resizer to container canvas, if it does not exists.
            if (!Container.Children.Contains(ImageResizer))
                Container.Children.Add(ImageResizer);
            if (!Container.Children.Contains(page))
            {
                if (addAsInitial)
                    Container.Children.Insert(0, page);
                else
                    Container.Children.Add(page);
            }
            page.ShowPageNumber(Pages.Contains(page) ? (Pages.IndexOf(page) + 1).ToString() : "1");
        }
        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal PageAdv GetPage(Point point)
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                PageAdv page = Pages[i];
                double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                if (x < 30)
                    x = 30;
                if (page.BoundingRectangle.Width < HorizontalWidth)
                    x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
                double y = page.BoundingRectangle.Top * ScaleFactor + (i + 1) * PageGap * (1 - ScaleFactor);
                Rect pageRect = new Rect(x, y, page.BoundingRectangle.Width * ScaleFactor, page.BoundingRectangle.Height * ScaleFactor);
                if (pageRect.Bottom >= point.Y || i == Pages.Count - 1)
                {
                    double leftMargin = x;
                    if (page.Section is SectionAdv && page.Section.SectionFormat is SectionFormat)
                        leftMargin += page.Section.SectionFormat.PageMargin.Left * ScaleFactor;
#if WPF
#else
                    if ((OwnerControl.Selection.IsEmpty || ImageResizer.Visibility == Visibility.Visible)
                        && point.X >= leftMargin)
                        OwnerControl.Cursor = new CoreCursor(CoreCursorType.IBeam, 0);
                    else
                        OwnerControl.Cursor = new CoreCursor(CoreCursorType.Arrow, 0);
#endif
                    return page;
                }
            }
#if WPF
#else
            OwnerControl.Cursor = new CoreCursor(CoreCursorType.Arrow, 0);
#endif
            return null;
        }
        #endregion
    }
}
