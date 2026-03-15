#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Syncfusion.Windows.PdfViewer
{
    public sealed partial class ThumbnailView : UserControl
    {
        GridViewItem m_currentHightLighetedThumnailItem;
        int m_heightOfEachRow;
        private ItemCollection m_collection = new ItemCollection();
        List<GridViewItem> m_itemCollection = new List<GridViewItem>();
        bool IsViewPortHeightSet = false;
        PdfDocumentView m_documentViewer;
        double m_backHorizontalScrollPos = 0;
        double m_widthOfEachPage;
        int m_numberOfColumnsInViewport = 0;
        int m_numberOfRowsInViewerport = 1;
        int PageIndexToHighlight;
        bool scrolledToPage = false;
        int m_pageToBeFocused = 0;
        int DestinationPageIndex;

        internal bool Iinitial = true;
        internal int ColumnIndex = 0;
        internal int pageIndex = 0;
        internal int NumberOfImagesInViewport = 0;
        internal int PageCount = 0;
        internal bool IsToggled = false;
        internal double RequiredViewportHeight;
        internal float AdjustmentHeight;
        internal DispatcherTimer Timer = new DispatcherTimer();
        internal ScrollViewer Scrollviewer;
        internal SemanticZoom SemanticZoom;

        internal ItemCollection Collection
        {
            get
            {
                return this.m_collection;
            }
            set
            {
                this.m_collection = value;
            }
        }

        internal void scrollToPage(int pageIdx)
        {
            GridViewItem item = ItemGridView.Items[pageIdx] as GridViewItem;
            pageIndex = pageIdx;
            m_pageToBeFocused = pageIdx;
            item.Focus(global::Windows.UI.Xaml.FocusState.Programmatic);
            scrolledToPage = true;

            ItemGridView.SelectedItem = item;
            int column = pageIdx / m_numberOfRowsInViewerport;

            double newScrollPositon = column * m_widthOfEachPage;
            Scrollviewer.ChangeView(newScrollPositon, null, null);
            PageIndexToHighlight = pageIdx;
        }


        void ThumbnailView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case global::Windows.System.VirtualKey.Right:
                    if (PageIndexToHighlight + m_numberOfRowsInViewerport < PageCount)
                    {
                        highlightThumbnailBorder(m_currentHightLighetedThumnailItem);
                        IsToggled = true;
                    }
                    break;
                case global::Windows.System.VirtualKey.Left:
                    if (PageIndexToHighlight - m_numberOfRowsInViewerport >= 0)
                    {
                        highlightThumbnailBorder(m_currentHightLighetedThumnailItem);
                        IsToggled = true;
                    }
                    break;
            }
        }

        internal void getData(int pageCount, int width, int height)
        {
            Canvas thumbnail;
            Border outerBorder;
            Border pageNumberDisplay;
            int k = 0;
            for (int j = 0; j < pageCount; j++)
            {
                outerBorder = new Border();

                outerBorder.Background = new SolidColorBrush(global::Windows.UI.Colors.Transparent);

                thumbnail = new Canvas();
                if (k == 0)
                {
                    thumbnail.Background = new SolidColorBrush(global::Windows.UI.Colors.White);
                    k++;
                }
                else
                    thumbnail.Background = new SolidColorBrush(global::Windows.UI.Colors.White);

                pageNumberDisplay = new Border();

                pageNumberDisplay.Background = new SolidColorBrush(global::Windows.UI.Colors.Black);

                Viewbox pagenumberbox = new Viewbox();
                pagenumberbox.Width = 30;
                pagenumberbox.Height = 30;
                pagenumberbox.MaxWidth = 30;
                pagenumberbox.MaxHeight = 30;
                pagenumberbox.HorizontalAlignment = HorizontalAlignment.Stretch;
                TextBlock pageDisplay = new TextBlock();
                global::Windows.UI.Xaml.Documents.Run runPageIndex = new global::Windows.UI.Xaml.Documents.Run();
                runPageIndex.Text = (j + 1).ToString();
                runPageIndex.FontSize = 17;
                pageDisplay.Inlines.Add(runPageIndex);
                pageDisplay.Foreground = new SolidColorBrush(global::Windows.UI.Colors.White);
                pagenumberbox.Child = pageDisplay;

                pageNumberDisplay.Padding = new Thickness(5, 5, 5, 5);
                pageNumberDisplay.Child = pagenumberbox;


                Canvas.SetLeft(pageNumberDisplay, -50);
                Canvas.SetTop(pageNumberDisplay, 0);
                Canvas.SetZIndex(pageNumberDisplay, 1);

                thumbnail.Children.Add(pageNumberDisplay);

                thumbnail.Width = width;
                thumbnail.Height = height;

                outerBorder.Width = width + 15;
                outerBorder.Height = height + 15;
                m_heightOfEachRow = (int)outerBorder.Height;
                outerBorder.Child = thumbnail;
                outerBorder.Margin = new Thickness(20, 0, 20, 0);

                Collection.Add(outerBorder);

                GridViewItem item = new GridViewItem();
                item.LostFocus += new RoutedEventHandler(ItemLostFocus);
                item.GotFocus += new RoutedEventHandler(ItemGotFocus);
                item.Content = outerBorder;
                m_itemCollection.Add(item);
            }
        }

        internal class ItemCollection : IEnumerable<Object>
        {
            private System.Collections.ObjectModel.ObservableCollection<object> itemCollection = new System.Collections.ObjectModel.ObservableCollection<object>();

            public IEnumerator<Object> GetEnumerator()
            {
                return itemCollection.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(object item)
            {
                itemCollection.Add(item);
            }
        }

        public ThumbnailView()
        {
            this.InitializeComponent();
            ItemGridView.ItemClick += ItemGridView_ItemClick;
            this.KeyDown += ThumbnailView_KeyDown;
            Timer.Tick += timer_Tick;
        }

        GridViewItem currentlyFocusedThumbnail = new GridViewItem();
        private void ItemGotFocus(object obj, RoutedEventArgs arg)
        {
            currentlyFocusedThumbnail = obj as GridViewItem;
            highlightThumbnailBorder(obj as GridViewItem);
        }

        private void ItemLostFocus(object obj, RoutedEventArgs arg)
        {
            RemoveHighlightThumbnail(obj as GridViewItem);
        }


        void highlightThumbnailBorder(GridViewItem item)
        {
            m_currentHightLighetedThumnailItem = item;
            Border border = (item).Content as Border;
            if (border != null)
            {
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 19, 190, 240));//4C13BEF0 
                border.Opacity = 30;
            }
        }

        void RemoveHighlightThumbnail(GridViewItem item)
        {
            Border border = (item).Content as Border;
            if (border != null)
                border.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));
        }

        internal void initialize(PdfDocumentView docView)
        {
            m_documentViewer = docView;
        }

        async void timer_Tick(object sender, object e)
        {
            if(scrolledToPage)
            {
                GridViewItem item = ItemGridView.Items[m_pageToBeFocused] as GridViewItem;
                bool IsFocusSet = item.Focus(global::Windows.UI.Xaml.FocusState.Programmatic);
                if (IsFocusSet)
                    scrolledToPage = false;
            }

            if (ItemGridView.Height != RequiredViewportHeight)
            {
                ItemGridView.Height = RequiredViewportHeight - 2 * AdjustmentHeight - 9;
            }
            if (SemanticZoom.ZoomedOutView.IsActiveView && IsToggled)
            {
                this.Focus(FocusState.Programmatic);
                scrollToPage(m_documentViewer.PageIndex);
                IsToggled = false;
            }
            if (Iinitial && Scrollviewer.ScrollableWidth != 0)
            {
                double width = Scrollviewer.ScrollableWidth + this.Scrollviewer.ViewportWidth;

                m_numberOfRowsInViewerport = (int)(this.Scrollviewer.ViewportHeight / m_heightOfEachRow);
                if (PageCount % m_numberOfRowsInViewerport != 0)
                    m_widthOfEachPage = width / ((PageCount + m_numberOfRowsInViewerport) / m_numberOfRowsInViewerport);
                else
                    m_widthOfEachPage = width / (PageCount / m_numberOfRowsInViewerport);
                m_numberOfColumnsInViewport = (int)(this.Scrollviewer.ViewportWidth / m_widthOfEachPage) + 1;
                Iinitial = false;
                scrollToPage(DestinationPageIndex);
            }

            double horizontalScrollPos = Scrollviewer.HorizontalOffset;

            if ((horizontalScrollPos - m_backHorizontalScrollPos) < 2000)
            {
                ColumnIndex = GetPageByOffset(horizontalScrollPos);
                pageIndex = (ColumnIndex * m_numberOfRowsInViewerport);
                NumberOfImagesInViewport = m_numberOfColumnsInViewport * m_numberOfRowsInViewerport;
                if (NumberOfImagesInViewport == 0)
                {
                    NumberOfImagesInViewport = PageCount;
                }
                if (m_documentViewer.thumbnailZoomFactor != 0)
                    await m_documentViewer.OnDemandThumbnail(pageIndex);
            }

            m_backHorizontalScrollPos = horizontalScrollPos;
        }

        int GetPageByOffset(double offset)
        {
            int pageIdx = 0;
            bool IsProcessed = false;
            double temp = 0;
            for (int i = 0; i < PageCount; i++)
            {
                temp += m_widthOfEachPage;
                if (temp < offset)
                {
                    pageIdx++;
                    IsProcessed = true;
                }
                else if (IsProcessed)
                    return pageIdx;
            }

            return pageIdx;
        }

        void ItemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem is GridViewItem))
            {
                if ((e.ClickedItem as GridViewItem).Content is Border)
                {
                    Border clickedItem = (e.ClickedItem as GridViewItem).Content as Border;
                    Canvas thumbnailCanvas = clickedItem.Child as Canvas;
                    int pageId = 0;
                    if (thumbnailCanvas.Children.Count == 2)
                    {
                        int.TryParse((thumbnailCanvas.Children[1] as Image).Name, out pageId);
                    }
                    m_documentViewer.isThumbnail = false;
                    m_documentViewer.SetScrollHeight(pageId);
                    SemanticZoom.ToggleActiveView();
                }
            }
            else if (e.ClickedItem is Border)
            {
                Border clickedItem = e.ClickedItem as Border;
                Canvas thumbnailCanvas = clickedItem.Child as Canvas;
                int pageId = 0;
                if (thumbnailCanvas.Children.Count == 2)
                {
                    int.TryParse((thumbnailCanvas.Children[1] as Image).Name, out pageId);
                }
                m_documentViewer.isThumbnail = false;
                m_documentViewer.SetScrollHeight(pageId);
                SemanticZoom.ToggleActiveView();
            }
        }
        internal List<object> IncludeCanvas(int width, int height)
        {
            adjustmentTop.Height = AdjustmentHeight * 2;
            m_itemCollection = new List<GridViewItem>();
            Collection = new ItemCollection();
            getData(PageCount, width, height);
            ItemGridView.ItemsSource = m_itemCollection;
            List<object> panels = Collection.ToList();
            return panels;
        }

        internal void Clear()
        {
            foreach (object obj in ItemGridView.Items)
            {
                if (obj is GridViewItem)
                {
                    Border thumb = (obj as GridViewItem).Content as Border;
                    if (thumb.Child is Canvas)
                    {
                        Canvas page = thumb.Child as Canvas;
                        if (page.Children.Count > 0)
                        {
                            for (int i = page.Children.Count - 1; i >= 0; i--)
                            {
                                if (page.Children[i] is Image)
                                    (page.Children[i] as Image).Source = null;
                                page.Children.RemoveAt(i);
                            }
                        }
                        page.Children.Clear();
                    }
                }
            }
            ItemGridView.ItemsSource = null;
            m_backHorizontalScrollPos = 0;

            ColumnIndex = 0;
            pageIndex = 0;
            m_widthOfEachPage = 0;
            m_numberOfColumnsInViewport = 0;
            m_numberOfRowsInViewerport = 1;
            NumberOfImagesInViewport = 0;
            PageCount = 0;
            PageIndexToHighlight = 0;
            DestinationPageIndex = 0;
        }

        private void ItemGridView_Loaded(object sender, RoutedEventArgs e)
        {
            Scrollviewer = FindingVisual.FindVisualChild<ScrollViewer>(this.ItemGridView);
        }
    }

    public static class FindingVisual
    {
        public static T FindVisualChild<T>(this DependencyObject obj)
                          where T : FrameworkElement
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                    if (child != null && child is T)
                        return (T)child;
                    else
                    {
                        T childOfChild = FindVisualChild<T>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}
