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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.PdfViewer.Base;
using Syncfusion.Pdf.Parsing;
using System.IO;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Documents.Serialization;
using System.Printing;
using Syncfusion.Pdf.Graphics;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.PdfViewer
{
    /// <summary>
    /// Interaction logic for PdfDocumentViewer.xaml
    /// </summary>
    internal partial class DocumentView : UserControl, IPdfDocumentView
    {
        internal PdfDocumentView parent;
        DispatcherTimer timer = new DispatcherTimer();
        System.Windows.Controls.Image im = new System.Windows.Controls.Image();
        private VirtualizationList<IEnumerable<System.Windows.Controls.Image>> virtualizingPdfPages;
        private ImageProvider imageProvider;
        private CustomVPanel virtualPanel;
        private ScrollViewer scrollViewer;
        private double m_zoomFactor = 1.0;
        public event CurrentPageChangedEventHandler CurrentPageChanged;
        public event NavigationButtonStatesChangedEventHandler NavigationButtonStatesChanged;
        private int m_currentPageIndex;
        private ToolTip m_previewToolTip;
        private PageNumberToolTip tool = new PageNumberToolTip();
        private delegate void NoParamCallback();
        private PdfLoadedDocument m_loadedDocument;
        internal string FileName;
        List<Page> pages = new List<Page>();
        PdfUnitConvertor m_unitConvertor;
        int pageIndex;
        private int nextMatch = -1;
        PdfLoadedDocument ldoc = null;
        List<BitmapSource> bmaplist = new List<BitmapSource>();
        private double m_touchZoom;
        bool isTouchZoom;
        private double m_touchFitPageTransition;
        private double m_touchTransition;

        public static DependencyProperty PageCountProperty;
        public static DependencyProperty PrintDocumentProperty;
        public static DependencyProperty CurrentPageIndexProperty;
        public static DependencyProperty ZoomModeProperty;

        private bool m_canGoToFirstPage;
        private bool m_canGoToPreviousPage;
        private bool m_canGoToNextPage;
        private bool m_canGoToLastPage;
        private MemoryStream m_blankImageStream = new MemoryStream();
        private bool m_showPageNumber;
        private Size[] m_actualBounds;

        internal MemoryStream BlankImageStream
        {
            get
            {
                return m_blankImageStream;
            }
            set
            {
                m_blankImageStream = value;
            }
        }

        public bool ShowPageNumber
        {
            get
            {
                return m_showPageNumber;
            }
            set
            {
                m_showPageNumber = value;
            }
        }
        public double ZoomFactor
        {
            get
            {
                return m_zoomFactor;
            }
            set
            {
                m_zoomFactor = value;

            }
        }

        public void SetZoom()
        {
            if (this.parent.ZoomMode == ZoomMode.FitPage)
            {
                var scrollBarHeight = this.scrollViewer.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible ? SystemParameters.HorizontalScrollBarHeight : 0;
                ZoomInternal((this.ActualHeight) / m_actualBounds[0].Height);
            }
            else if (this.parent.ZoomMode == ZoomMode.FitWidth)
            {
                var scrollBarWidth = this.scrollViewer.ComputedVerticalScrollBarVisibility == System.Windows.Visibility.Visible ? SystemParameters.VerticalScrollBarWidth : 0;
                ZoomInternal((this.ActualWidth - scrollBarWidth) / m_actualBounds[0].Width);
            }
            else
            {
                ZoomInternal(1);
            }
        }

        public DocumentView(PdfDocumentView docViewerParent)
        {
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            InitializeComponent();
            this.parent = docViewerParent;
            this.Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        }

        void timer_Tick(object sender, EventArgs e)
        {
            scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(this);
            if (scrollViewer != null)
            {
                timer.Stop();
                PdfDocumentViewer_Loaded(null, null);
            }
        }
        /// <summary>
        /// Gets the PrintDocument
        /// </summary>
        public FixedDocument PrintDocument
        {
            get
            {
                CultureInfo current = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                return GetFixedDocument();
                Thread.CurrentThread.CurrentCulture = current;
            }
        }
        private FixedDocument GetFixedDocument()
        {
            FixedDocument document = new FixedDocument();
            Visual viewerControl;

            int printPageCount = this.Pages.Length; ;

            for (int i = 0; i < this.Pages.Length; i++)
            {
                Page currentPage = this.Pages[i];
                currentPage.DrawForPrinting();

                DrawingVisual dv = this.Pages[i].Graphics.Visual;
                viewerControl = dv;

                Size controlSize = new Size(currentPage.Width, currentPage.Height);

                DrawingVisual pageVisual = new DrawingVisual();
                DrawingContext dc = pageVisual.RenderOpen();
                dc.PushTransform(new TranslateTransform(0, 0));
                if (currentPage.Width > currentPage.Height)
                {
                    dc.PushTransform(new TranslateTransform(0, controlSize.Width));
                    dc.PushTransform(new RotateTransform(-90));
                }

                DrawingGroup dg = dv.Drawing;
                dg.Transform = new TranslateTransform(0, 0);
                dc.DrawDrawing(dg);
                dc.Close();

                PageContent m_PageContent = new PageContent();
                FixedPage page = new FixedPage();
                if (currentPage.Width > currentPage.Height)
                {
                    page.Width = controlSize.Height;
                    page.Height = controlSize.Width;
                }
                else
                {
                    page.Width = controlSize.Width;
                    page.Height = controlSize.Height;
                }
                VisualContainer myContainer = new VisualContainer();
                myContainer.AddVisual(pageVisual);
                page.Children.Add(myContainer);
                ((IAddChild)m_PageContent).AddChild(page);
                document.Pages.Add(m_PageContent);
            }

            return document;
        }
        
        public void ZoomIn()
        {
            ZoomInternal(ZoomFactor + this.parent.ZoomStep);
        }

        public int CurrentPageIndex
        {
            get
            {
                return GetCurrentPageIndex();
            }
            set
            {
                m_currentPageIndex = GetCurrentPageIndex();
            }
        }
        public void ZoomOut()
        {
            ZoomInternal(ZoomFactor - this.parent.ZoomStep);
        }
        public void GotoPreviousPage()
        {
            if (this.scrollViewer == null)
                return;

            var currentPageIndex = GetCurrentPageIndex();

            if (currentPageIndex == 0)
                return;

            var verticalOffset = this.virtualPanel.GetVerticalOffset(currentPageIndex - 1);
            this.scrollViewer.ScrollToVerticalOffset(verticalOffset);
        }
        public void GotoNextPage()
        {
            var nextIndex = GetCurrentPageIndex();

            if (nextIndex == -1)
                return;

            GotoPage(nextIndex + 1);
        }
        public void GoToPageAtIndex(int pageIndex)
        {
            GotoPage(pageIndex);
        }
        public void GotoPage(int pageNumber)
        {
            if (this.scrollViewer == null)
                return;

            var verticalOffset = this.virtualPanel.GetVerticalOffset(pageNumber);
            this.scrollViewer.ScrollToVerticalOffset(verticalOffset);
        }
        public int GetCurrentPageIndex()
        {
            if (this.scrollViewer == null || virtualPanel == null)
                return 0;

            var pageIndex = this.virtualPanel.GetItemIndex(this.scrollViewer.VerticalOffset);

            return pageIndex;
        }

        public bool CanGoToFirstPage
        {
            get
            {
                return m_canGoToFirstPage;
            }
            set
            {
                m_canGoToFirstPage = value;
            }
        }

        public bool CanGoToPreviousPage
        {
            get
            {
                return m_canGoToPreviousPage;
            }
            set
            {
                m_canGoToPreviousPage = value;
            }
        }

        public bool CanGoToNextPage
        {
            get
            {
                return m_canGoToNextPage;
            }
            set
            {
                m_canGoToNextPage = value;
            }
        }

        public bool CanGoToLastPage
        {
            get
            {
                return m_canGoToLastPage;
            }
            set
            {
                m_canGoToLastPage = value;
            }
        }

        internal void ResetNavigationButtonStates()
        {
            if (CurrentPageIndex == 0 && PageCount == 1)
            {
                CanGoToFirstPage = false;
                CanGoToPreviousPage = false;
                CanGoToNextPage = false;
                CanGoToLastPage = false;
            }
            else if (CurrentPageIndex == 0 && PageCount > 1)
            {
                CanGoToFirstPage = false;
                CanGoToPreviousPage = false;
                CanGoToNextPage = true;
                CanGoToLastPage = true;
            }
            else if (CurrentPageIndex >= 0 && CurrentPageIndex < PageCount-1)
            {
                CanGoToFirstPage = true;
                CanGoToPreviousPage = true;
                CanGoToNextPage = true;
                CanGoToLastPage = true;
            }
            else if (CurrentPageIndex == PageCount-1)
            {
                CanGoToFirstPage = true;
                CanGoToPreviousPage = true;
                CanGoToNextPage = false;
                CanGoToLastPage = false;
            }
            else if (PageCount < CurrentPageIndex)
            {
                CanGoToFirstPage = false;
                CanGoToPreviousPage = false;
                CanGoToNextPage = false;
                CanGoToLastPage = false;
            }

            if (NavigationButtonStatesChanged != null)
                NavigationButtonStatesChanged(this, null);

            if (CurrentPageChanged != null)
                CurrentPageChanged(this, null);
        }
        public void ZoomTo(double zoomFactor)
        {
            zoomFactor = (float)zoomFactor / 100;
            this.ZoomInternal(zoomFactor);
        }
        private void ZoomInternal(double zoomFactor)
        {
            if (scrollViewer != null)
            {
                var yOffset = this.scrollViewer.VerticalOffset;
                var xOffset = this.scrollViewer.HorizontalOffset;
                var zoom = ZoomFactor;

                ZoomFactor = (float)zoomFactor;
                this.CreateNewItemsSource();

                this.scrollViewer.ScrollToHorizontalOffset((xOffset / zoom) * zoomFactor);
                this.scrollViewer.ScrollToVerticalOffset((yOffset / zoom) * zoomFactor);
            }
        }
        public int PageCount
        {
            get
            {
                if (LoadedDocument == null)
                    return 0;
                return LoadedDocument.PageCount;
            }
        }
        void PdfDocumentViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_loadedDocument != null)
            {
                CreateNewItemsSource();
                SetZoom();
                m_actualBounds = this.virtualPanel.PageRowBounds;
            }
        }

        private void CreateNewItemsSource()
        {

            scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(this);
            this.virtualPanel = VisualTreeHelperEx.FindChild<CustomVPanel>(this);
            if (this.virtualizingPdfPages == null)
            {
                this.virtualizingPdfPages = new AsyncVList<IEnumerable<System.Windows.Controls.Image>>(this.imageProvider, scrollViewer, this.virtualPanel);
                itemsControl.ItemsSource = virtualizingPdfPages;
            }
            else
                this.virtualizingPdfPages.CleanAllPages();
            var finalBounds = new List<System.Windows.Size>();
            
            for (int i = 0; i < imageProvider.Count(); i++)
            {
                float width = UnitConvertor.ConvertToPixels(this.LoadedDocument.Pages[i].Size.Width, PdfGraphicsUnit.Point);
                float height = UnitConvertor.ConvertToPixels(this.LoadedDocument.Pages[i].Size.Height, PdfGraphicsUnit.Point);
                finalBounds.Add(new System.Windows.Size(width * ZoomFactor, (height * ZoomFactor) + 4));
            }
            this.virtualPanel.PageRowBounds = finalBounds.ToArray();
            scrollViewer.ScrollChanged += new ScrollChangedEventHandler(scrollViewer_ScrollChanged);
            virtualPanel.Pages = pages.ToArray();
            Dictionary<object, int> pageKidsCollection = new Dictionary<object,int>();
            PdfCrossTable basePage = (m_loadedDocument.Pages[0] as PdfLoadedPage).CrossTable;
            PdfDictionary dict = basePage.DocumentCatalog;
            if (dict.ContainsKey("Pages"))
            {
                PdfDictionary basePge = (dict["Pages"] as PdfReferenceHolder).Object as PdfDictionary;
                PdfArray kids = basePge["Kids"] as PdfArray;
                for (int i = 0; i < kids.Count; i++)
                {
                    PdfReferenceHolder tempRef = kids[i] as PdfReferenceHolder;
                    if (!pageKidsCollection.ContainsKey(tempRef.Reference))
                    {
                        pageKidsCollection.Add(tempRef.Reference, i);
                    }
                }
            }
            virtualPanel.PageKidsCollection = pageKidsCollection;
            virtualPanel.ParentView = this;
        }

        void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (CurrentPageChanged != null)
                CurrentPageChanged(this, null);
            ResetNavigationButtonStates();
            ScrollViewer scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(this as FrameworkElement);
            if (scrollViewer != null && ShowPageNumber)
            {
                string scrollBarPartName = "PART_VerticalScrollBar";
                ScrollBar scrollBar = VisualTreeHelperEx.FindName<ScrollBar>(scrollBarPartName, scrollViewer);
                if (scrollBar != null)
                {
                    Track track = VisualTreeHelperEx.FindName<Track>("PART_Track", scrollBar);
                    if (track != null)
                    {
                        Thumb thumb = track.Thumb;
                        if (thumb != null)
                        {
                            thumb.DragStarted += delegate(object senders, DragStartedEventArgs eNew)
                            {
                                if (m_previewToolTip == null)
                                {
                                    m_previewToolTip = new ToolTip();
                                    tool.PageDisplay.Content = m_currentPageIndex.ToString();
                                    tool.VerticalAlignment = VerticalAlignment.Center;
                                    tool.HorizontalAlignment = HorizontalAlignment.Center;
                                    tool.Margin = new Thickness(0, -10, 0, 0);
                                    m_previewToolTip.Content = tool;
                                    m_previewToolTip.FontSize = 12;
                                    m_previewToolTip.Margin = new Thickness(5);
                                    m_previewToolTip.Background = Brushes.Transparent;
                                    m_previewToolTip.BorderBrush = Brushes.Transparent;
                                }
                                m_previewToolTip.PlacementTarget = thumb;
                                m_previewToolTip.VerticalContentAlignment = VerticalAlignment.Center;
                                m_previewToolTip.HorizontalContentAlignment = HorizontalAlignment.Center;
                                m_previewToolTip.Placement = PlacementMode.Right;
                                m_previewToolTip.HorizontalAlignment = HorizontalAlignment.Left;
                                m_previewToolTip.VerticalOffset = 0.0;
                                m_previewToolTip.HorizontalOffset = 0.0;
                                m_previewToolTip.IsOpen = true;
                            };
                            thumb.DragDelta += delegate(object senders, DragDeltaEventArgs eNew2)
                            {
                                if ((m_previewToolTip != null) && (scrollBar.Value > scrollBar.Minimum) && (scrollBar.Value < scrollBar.Maximum))
                                {
                                    m_previewToolTip.VerticalOffset = m_previewToolTip.VerticalOffset == 0.0 ? 0.001 : 0.0;
                                }
                            };
                            thumb.DragCompleted += delegate(object senders, DragCompletedEventArgs eNew3)
                            {
                                if (m_previewToolTip != null)
                                {
                                    m_previewToolTip.IsOpen = false;
                                }
                            };
                            scrollBar.Scroll += delegate(object senders, ScrollEventArgs eNew4)
                            {
                                if (m_previewToolTip != null && m_previewToolTip.IsOpen == true)
                                {
                                    scrollBar.Dispatcher.BeginInvoke((NoParamCallback)delegate()
                                    {
                                        tool.PageDisplay.Content = (CurrentPageIndex + 1).ToString();
                                    }, System.Windows.Threading.DispatcherPriority.Input);
                                }
                            };
                        }
                    }
                }
            }
        }

        public PdfLoadedDocument LoadedDocument
        {
            get
            {
                return m_loadedDocument;
            }
            set
            {
                m_loadedDocument = value;
            }
        }

        internal Page[] Pages
        {
            get
            {
                return pages.ToArray();
            }
        }
        public void Print()
        {
            SilentPrint();
        }

        internal PdfUnitConvertor UnitConvertor
        {
            get
            {
                if (m_unitConvertor == null)
                    m_unitConvertor = new PdfUnitConvertor();

                return m_unitConvertor;
            }
        }
        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <returns>Returns the specified page as BitmapSource</returns>
        public BitmapSource ExportAsImage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= this.Pages.Length)
                throw new IndexOutOfRangeException("Page index is not inside the range of the pages");

            DrawingVisual dv = this.Pages[pageIndex].Graphics.Visual;

            int actualWidth = (int)UnitConvertor.ConvertToPixels((float)(this.Pages[pageIndex].ActualWidth), PdfGraphicsUnit.Point);
            int actualHeight = (int)UnitConvertor.ConvertToPixels((float)(this.Pages[pageIndex].ActualHeight), PdfGraphicsUnit.Point);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(actualWidth, actualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(dv);

            return renderBitmap;
        }

        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Returns the specified pages as Images</returns>
        public BitmapSource[] ExportAsImage(int startIndex, int endIndex)
        {
            if (endIndex < startIndex)
                throw new ArgumentException("Invalid arguments-Start index cannot be greater than end index");

            if (startIndex < 0 && endIndex >= this.Pages.Length)
                throw new IndexOutOfRangeException("The specified index is not inside the bounds of the page range");

            BitmapSource[] bitmapCollection = new BitmapSource[endIndex - startIndex + 1];

            for (int i = startIndex; i <= endIndex; i++)
            {
                DrawingVisual dv = this.Pages[i].Graphics.Visual;

                PdfUnitConvertor m_unitConvertor = new PdfUnitConvertor();
                int actualWidth = (int)UnitConvertor.ConvertToPixels((float)(this.Pages[i].ActualWidth), PdfGraphicsUnit.Point);
                int actualHeight = (int)UnitConvertor.ConvertToPixels((float)(this.Pages[i].ActualHeight), PdfGraphicsUnit.Point);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(actualWidth, actualHeight, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(dv);
                bitmapCollection[i] = renderBitmap;
            }

            return bitmapCollection;
        }

        public int m_nextMatch
        {
            get
            {
                return nextMatch;
            }
            set
            {
                nextMatch = value;
            }
        }
        public void ClearSearch()
        {
            foreach (Page p in Pages)
                p.textMatchRectList.Clear();
        }
        /// <summary>
        /// Search the text in the Pdfdocument
        /// </summary>
        /// <param name="txt">The text to be searched</param>
        /// <param name="IsNextClicked">Determined whether to search next or previous</param>        
        public void TextSearch(String txt, bool IsNextClicked)
        {
            if (IsNextClicked)
            {
                SearchNext(txt);
            }
            else
            {
                pageIndex = CurrentPageIndex + 1;
                Pages[pageIndex - 1].textMatchRectList.Clear();
                bool foundMatch = false;
                List<TextSearch> txtMatchs = new List<TextSearch>();
                if (m_nextMatch == 0)
                {
                    if (CurrentPageIndex + 1 > 1)
                    {
                        pageIndex = CurrentPageIndex ;
                        GoToPageAtIndex(pageIndex);

                        foundMatch = Pages[pageIndex - 1].SearchText(txt, out txtMatchs);
                        m_nextMatch = txtMatchs.Count;
                    }
                    else
                    {
                        return;
                    }
                }

                if (foundMatch == false && m_nextMatch == 0)
                {
                    while (foundMatch == false)
                    {
                        pageIndex--;
                        foundMatch = Pages[pageIndex - 1].SearchText(txt, out txtMatchs);
                        m_nextMatch = txtMatchs.Count;
                    }
                    GoToPageAtIndex(pageIndex);
                }

                foundMatch = Pages[pageIndex - 1].SearchText(txt, out txtMatchs);
                if (foundMatch)
                {
                    if (m_nextMatch > 0)
                    {
                        m_nextMatch--;
                        Pages[pageIndex - 1].textMatchRectList.Clear();
                        Pages[pageIndex - 1].textMatchRectList.Add(txtMatchs[m_nextMatch]);
                        Pages[pageIndex - 1].targetTextHighlight = txt;
                        InvalidateVisual();
                    }
                }
                DependencyObject parent = this.Parent as UIElement;
                while (parent != null)
                {
                    Type parentType = parent.GetType();
                    if (parentType.Name == "PdfViewerControl")
                    {
                        m_unitConvertor = new PdfUnitConvertor();
                        PdfViewerControl pdfViewer = parent as PdfViewerControl;
                        double bottom = pdfViewer.ActualHeight;
                        double offSet = 8;
                        offSet *= ZoomFactor;
                        if (m_nextMatch <= txtMatchs.Count && txtMatchs.Count > 0 && m_nextMatch >= 0)
                        {
                            offSet += txtMatchs[m_nextMatch].CurrentLocation.Y;
                            bottom = m_unitConvertor.ConvertFromPixels((float)pdfViewer.ActualHeight, PdfGraphicsUnit.Point);
                            if (offSet > bottom)
                            {
                                GoToPageAtIndexAndOffset(pageIndex , (float)offSet / 2);
                            }
                            else
                            {
                                GoToPageAtIndex(pageIndex-1);
                            }
                        }
                    }
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
        }
        /// <summary>
        /// Moves the Vscroll bar to specified page and offset location
        /// </summary>
        /// <param name="pageIndex">The destination page</param>
        /// <param name="destOffset">The destination offset in the page</param>        
        public void GoToPageAtIndexAndOffset(int pageIndex, float destOffset)
        {
            string pageName = string.Format("page{0}", pageIndex);
            double offSet = Pages[pageIndex - 1].Bounds.Top;
            offSet *= ZoomFactor;
            offSet += destOffset;
            this.scrollViewer.ScrollToVerticalOffset(offSet);
            ResetNavigationButtonStates();
        }
        /// <summary>
        /// Returns the page number and rectangle postions of the text matchs found in the page
        /// </summary>
        /// <param name="text">The text to be searched</param>
        /// <param name="matchRect">Holds the page number and rectangle positions of the text matches</param> 
        internal bool FindTextMatches(string text, out Dictionary<int, List<System.Drawing.RectangleF>> matchRect)
        {
            bool IsMatchFound = false;
            matchRect = new Dictionary<int, List<System.Drawing.RectangleF>>();

            for (int i = 0; i <= Pages.Length - 1; i++)
            {
                List<System.Drawing.RectangleF> matchRects = new List<System.Drawing.RectangleF>();
                List<TextSearch> txtMatchs = new List<TextSearch>();
                bool foundMatch = Pages[i].SearchText(text, out txtMatchs);
                float scaleX = -1, scaleY = -1;
                float locationX = 0, locationY = 0, width = 0, height = 0;

                foreach (TextSearch url in txtMatchs)
                {
                    System.Drawing.PointF transformLocation = url.CurrentLocation;

                    if (url.ScalingFactor.X != 0)
                    {
                        scaleX = url.ScalingFactor.X;
                    }
                    if (url.ScalingFactor.Y != 0)
                    {
                        scaleY = url.ScalingFactor.Y;
                    }
                    locationX = transformLocation.X;
                    locationY = transformLocation.Y;
                    if (scaleX > 0)
                    {
                        width = url.TextElementWidth * scaleX;
                    }
                    else
                    {
                        width = url.TextElementWidth;
                    }
                    if (scaleY > 0)
                    {
                        height = url.FontSize * scaleX;
                    }
                    else
                    {
                        height = url.FontSize;
                    }

                    System.Drawing.RectangleF temprect = new System.Drawing.RectangleF(locationX, locationY, width, height);
                    int startCharLoc = url.Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase);
                    String targetString = url.Text.Substring(startCharLoc, text.Length);
                    String previousString = url.Text.Substring(0, startCharLoc);
                    FormattedText ftTargetString = new FormattedText(targetString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, url.TFace, url.TextFont.Size, Brushes.Black);
                    FormattedText ftPrevString = new FormattedText(previousString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, url.TFace, url.TextFont.Size, Brushes.Black);
                    if (scaleX > 0)
                    {
                        temprect = new System.Drawing.RectangleF((float)(locationX + (ftPrevString.WidthIncludingTrailingWhitespace * scaleX)), locationY, (float)ftTargetString.Width * scaleX, height);
                    }
                    else
                    {
                        temprect = new System.Drawing.RectangleF((float)(locationX + ftPrevString.WidthIncludingTrailingWhitespace), locationY, (float)ftTargetString.Width, height);
                    }
                    IsMatchFound = true;
                    matchRects.Add(temprect);
                }
                matchRect.Add(i, matchRects);
            }
            return IsMatchFound;
        }
        public void DrawTextSearch(int i)
        {
            if (virtualPanel.Pages[i].textMatchRectList.Count > 0)
            {
                DisplaybyAddingControl1(i);
            }
        }
        /// <summary>
        /// Search the next matching text in the pdf page
        /// </summary>
        /// <param name="txt">Text to be serched</param>        
        private void SearchNext(String txt)
        {
            bool IsTextFound = false;
            int pageIndex = CurrentPageIndex + 1;
            Pages[pageIndex - 1].textMatchRectList.Clear();
            bool finishedHighlightingCurrentPage = false;
            m_nextMatch++;

            List<TextSearch> txtMatchs = new List<TextSearch>();
            bool foundMatch = Pages[pageIndex - 1].SearchText(txt, out txtMatchs);
            if (CurrentPageIndex + 1 == Pages.Length && txtMatchs.Count == m_nextMatch)
            {
                MessageBoxResult IsOk = MessageBox.Show(
                       string.Format("Reader has finished searching the document. No matches were found"), "Essential Pdf Viewer", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (m_nextMatch >= txtMatchs.Count && m_nextMatch != 0)
            {
                finishedHighlightingCurrentPage = true;
            }

            if (m_nextMatch >= txtMatchs.Count && pageIndex == Pages.Length)
            {
                Pages[pageIndex - 1].textMatchRectList.Clear();
                m_nextMatch = 0;
                for (int i = 0; i <= Pages.Length - 1; i++)
                {
                    foundMatch = Pages[i].SearchText(txt, out txtMatchs);
                    if (foundMatch)
                    {
                        GoToPageAtIndex(i + 1);
                        Pages[i].textMatchRectList.Add(txtMatchs[m_nextMatch]);
                        Pages[i].targetTextHighlight = txt;
                        return;
                    }
                }
            }

            if (foundMatch && finishedHighlightingCurrentPage == false)
            {
                bool offsetChanged = false;
                DependencyObject parent = this.Parent as UIElement;
                while (parent != null)
                {
                    Type parentType = parent.GetType();
                    if (parentType.Name == "PdfViewerControl")
                    {
                        m_unitConvertor = new PdfUnitConvertor();
                        PdfViewerControl pdfViewer = parent as PdfViewerControl;
                        double bottom = pdfViewer.ActualHeight;
                        double offSet = 8;
                        offSet *= ZoomFactor;
                        offSet += txtMatchs[m_nextMatch].CurrentLocation.Y;
                        bottom = m_unitConvertor.ConvertFromPixels((float)pdfViewer.ActualHeight, PdfGraphicsUnit.Point);
                        if (offSet > bottom)
                        {
                            GoToPageAtIndexAndOffset(pageIndex, (float)offSet / 2);
                            offsetChanged = true;
                        }
                    }
                    parent = VisualTreeHelper.GetParent(parent);
                }
                if (offsetChanged == false)
                {
                    GoToPageAtIndex(pageIndex - 1);
                }
                Pages[pageIndex - 1].textMatchRectList.Clear();
                Pages[pageIndex - 1].textMatchRectList.Add(txtMatchs[m_nextMatch]);
                Pages[pageIndex - 1].targetTextHighlight = txt;
                InvalidateVisual();
            }
            else if (foundMatch == false)
            {
                for (int i = 0; i <= Pages.Length - 1; i++)
                {
                    foundMatch = Pages[i].SearchText(txt, out txtMatchs);
                    if (foundMatch)
                    {
                        IsTextFound = true;
                        GoToPageAtIndex(i);
                        Pages[i].textMatchRectList.Add(txtMatchs[m_nextMatch]);
                        Pages[i].targetTextHighlight = txt;
                        DependencyObject parent = this.Parent as UIElement;
                        while (parent != null)
                        {
                            Type parentType = parent.GetType();
                            if (parentType.Name == "PdfViewerControl")
                            {
                                DrawTextSearch(i);
                                m_unitConvertor = new PdfUnitConvertor();
                                PdfViewerControl pdfViewer = parent as PdfViewerControl;
                                double bottom = pdfViewer.ActualHeight;
                                double offSet = 8;
                                offSet *= ZoomFactor;
                                offSet += txtMatchs[m_nextMatch].CurrentLocation.Y;
                                bottom = m_unitConvertor.ConvertFromPixels((float)pdfViewer.ActualHeight, PdfGraphicsUnit.Point);
                                if (offSet > bottom)
                                {
                                    GoToPageAtIndexAndOffset(i, (float)offSet / 2);
                                }
                                else
                                {
                                    GoToPageAtIndex(i);
                                }
                            }
                            parent = VisualTreeHelper.GetParent(parent);
                        }
                    }
                }
                if (IsTextFound == false)
                {
                    MessageBox.Show("Reader reached the end of the document.No matches found", "Essential PDF Viewer", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (finishedHighlightingCurrentPage)
            {
                m_nextMatch = 0;
                for (int i = CurrentPageIndex + 1; i <= Pages.Length - 1; i++)
                {
                    foundMatch = Pages[i].SearchText(txt, out txtMatchs);
                    if (foundMatch)
                    {
                        GoToPageAtIndex(i);
                        Pages[i].textMatchRectList.Add(txtMatchs[m_nextMatch]);
                        Pages[i].targetTextHighlight = txt;
                        return;
                    }
                }
            }
        }

        internal void SilentPrint()
        {
            bmaplist.Clear();
            if (ldoc == null)
                ldoc = this.LoadedDocument;
            PdfDocument doc = new PdfDocument();
            for (int i = 0; i < ldoc.PageCount; i++)
            {
                doc = new PdfDocument();
                doc.ImportPage(ldoc, i);

                Stream s = new MemoryStream();
                doc.Save(s);
                this.Load(s);

                BitmapSource im = this.ExportAsImage(0);
                bmaplist.Add(im);
                doc.Close(true);
            }

            PrintDialog d = new PrintDialog();
            if (FileName != null)
            {
                FileInfo fi = new FileInfo(FileName);
                d.PrintQueue.CurrentJobSettings.Description = fi.Name;
            }
            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(d.PrintQueue);
            SerializerWriterCollator collator = writer.CreateVisualsCollator();
            collator.BeginBatchWrite();


            DrawingVisual drawvis;
            DrawingContext context;
            BitmapImage img;

            foreach (BitmapSource bmap in bmaplist)
            {
                drawvis = new DrawingVisual();
                context = drawvis.RenderOpen();
                img = new BitmapImage();
                img.BeginInit();

                BitmapEncoder encoder = null;
                encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmap));
                Stream stream = new MemoryStream();
                encoder.Save(stream);
                img.StreamSource = stream;

                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                if (img.Width > img.Height)
                {
                    context.PushTransform(new TranslateTransform(0, img.Width));
                    context.PushTransform(new RotateTransform(-90));
                }
                context.DrawImage(img, new Rect(0, 0, img.Width, img.Height));
                context.Close();

                PageContent m_PageContent = new PageContent();
                FixedPage page = new FixedPage();

                VisualContainer myContainer = new VisualContainer();
                myContainer.AddVisual(drawvis);
                page.Children.Add(myContainer);

                ContainerVisual newPage = new ContainerVisual();
                newPage.Children.Add(page);
                PrintTicket ticket = new PrintTicket();
                ticket.PageMediaSize = new PageMediaSize(img.Width, img.Height);

                collator.Write(newPage, ticket);

                stream.Close();
            }
            collator.EndBatchWrite();
            d.PrintQueue = d.PrintQueue;
            bmaplist.Clear();
        }
        internal void LoadPages()
        {
            Page page;
            for (int i = 0; i < m_loadedDocument.Pages.Count; i++)
            {
                page = new Page(m_loadedDocument.Pages[i]);
                pages.Add(page);
            }
            ResetNavigationButtonStates();
        }
        /// <summary>
        /// Loads a Pdf document  in the Pdf viewer from the specified stream.
        /// </summary>
        /// <param name="stream">A stream that contains the data for the Pdf document</param>
        public void Load(Stream stream)
        {
            if (stream == null || stream.CanRead == false || stream.Length == 0)
                throw new Exception("Stream cannot be read");
            stream.Position = 0;

            m_loadedDocument = DocumentLoader.Instance.Load(stream);
            LoadPages();
            this.imageProvider = new ImageProvider(m_loadedDocument, this);
            if (this.scrollViewer != null)
            {
                CreateNewItemsSource();
                this.scrollViewer.ScrollToTop();
                m_actualBounds = this.virtualPanel.PageRowBounds;
            }
        }
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        /// <param name="password">The password for opening the document.</param>
        public void Load(String filePath, string password)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Cannot able to locate the specified file");

            m_loadedDocument = DocumentLoader.Instance.Load(filePath, password);
            FileName = filePath;
            LoadPages();
            this.imageProvider = new ImageProvider(m_loadedDocument, this);
            if (this.scrollViewer != null)
            {
                CreateNewItemsSource();
                this.scrollViewer.ScrollToTop();
                m_actualBounds = this.virtualPanel.PageRowBounds;
            }
        }
        /// <summary>
        /// Loads a pdf document in the Pdf viewer from the specified PdfLoadedDocuemnt.
        /// </summary>
        /// <param name="loadedDocument">The PdfLoadedDocument to be viewed in the PdfViewer</param>
        public void Load(PdfLoadedDocument loadedDocument)
        {
            if (loadedDocument == null)
                throw new ArgumentNullException("Loaded document should not be null");

            m_loadedDocument = loadedDocument;
            LoadPages();
            this.imageProvider = new ImageProvider(m_loadedDocument, this);
            if (this.scrollViewer != null)
            {
                CreateNewItemsSource();
                this.scrollViewer.ScrollToTop();
                m_actualBounds = this.virtualPanel.PageRowBounds;
            }
        }
        public void Load(string filePath)
        {

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Cannot able to locate the specified file");
            m_loadedDocument = DocumentLoader.Instance.Load(filePath);
            FileName = filePath;
            int count = m_loadedDocument.PageCount;
            LoadPages();
            this.imageProvider = new ImageProvider(m_loadedDocument, this);
            if (this.scrollViewer != null)
            {
                CreateNewItemsSource();
                this.scrollViewer.ScrollToTop();
                m_actualBounds = this.virtualPanel.PageRowBounds;
            }
        }
        public void UpdateOffset(double offSet)
        {
            double verticalOffset = scrollViewer.VerticalOffset;
            verticalOffset += offSet;
            scrollViewer.ScrollToVerticalOffset(verticalOffset);
            scrollViewer.ScrollToVerticalOffset(verticalOffset);
        }
        private void DisplaybyAddingControl1(int pageindex)
        {
            Image imagecntrl = new Image();

            Page currentPage = this.virtualPanel.Pages[pageindex];
            Size controlSize = new Size(currentPage.Width, currentPage.Height);

            DrawingVisual pageVisual = new DrawingVisual();
            DrawingContext dc = pageVisual.RenderOpen();
            dc.PushTransform(new TranslateTransform(0, 0));
            dc.PushTransform(new ScaleTransform(1, 1));
            currentPage.Draw(dc, new List<Page>(virtualPanel.Pages), pageindex);

            DrawingVisual dv = virtualPanel.Pages[pageindex].Graphics.Visual;
            DrawingGroup dg = dv.Drawing;
            dc.DrawDrawing(dg);
            dc.Close();

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)(controlSize.Width * 1), (int)(controlSize.Height * 1), 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(pageVisual);
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = bitmap;
            var list = new List<IEnumerable<System.Windows.Controls.Image>>();
            var rowList = new List<System.Windows.Controls.Image>(1);
            rowList.Add(img);
            list.Add(rowList);
            virtualizingPdfPages.RePopulatePage(pageindex, (IList<IEnumerable<Image>>)list);
        }
        public BitmapSource DisplaybyAddingControl(int pageindex)
        {
            RenderTargetBitmap bitmap = null;

            Page currentPage = this.Pages[pageindex];
            Size controlSize = new Size(currentPage.Width, currentPage.Height);

            DrawingVisual pageVisual = new DrawingVisual();
            DrawingContext dc = pageVisual.RenderOpen();
            dc.PushTransform(new TranslateTransform(0, 0));
            dc.PushTransform(new ScaleTransform(m_zoomFactor, m_zoomFactor));

            currentPage.Draw(dc, pages, pageindex);

            DrawingVisual dv = this.Pages[pageindex].Graphics.Visual;
            DrawingGroup dg = dv.Drawing;
            dc.DrawDrawing(dg);
            dc.Close();

            bitmap = new RenderTargetBitmap((int)(controlSize.Width * m_zoomFactor), (int)(controlSize.Height * m_zoomFactor), 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(pageVisual);

            return bitmap;
        }
        public void Unload()
        {
            if (m_loadedDocument != null)
            {
                m_loadedDocument.EnableMemoryOptimization = true;
                m_loadedDocument.Dispose();
                m_loadedDocument = null;
            }
            if (virtualizingPdfPages != null)
            {
                this.virtualizingPdfPages.Unload();
                this.virtualizingPdfPages.CleanAllPages();
                this.virtualizingPdfPages = null;
            }
            if (virtualPanel != null)
            {
                this.virtualPanel.Pages = null;
                this.virtualPanel.PageKidsCollection = null;
                this.virtualPanel.ParentView = null;
            }
            BlankImageStream.Dispose();
            BlankImageStream = new MemoryStream();
            if(imageProvider != null)
            this.imageProvider.Unload();
            this.imageProvider = null;
            ZoomFactor = 1;
            foreach (Page p in this.Pages)
            {

                foreach (KeyValuePair<string, object> item in p.Resources)
                {
                    if (item.Value is Syncfusion.Pdf.FontStructure)
                    {
                        (item.Value as Syncfusion.Pdf.FontStructure).fontStream.Dispose();

                    }
                }
            }
            pages = new List<Page>();

            //itemsControl.ItemsSource = null;
            GC.Collect();
        }
        ScrollViewer IPdfDocumentView.ScrollViewer
        {
            get { return this.scrollViewer; }
        }

        UserControl IPdfDocumentView.Instance
        {
            get { return this; }
        }

        //private void itemsControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        //{
        //    isTouchZoom = false;
        //    m_touchZoom = 0;
        //    var element = e.Source as FrameworkElement;
        //    if (element != null)
        //    {
        //        var deltaManipulation = e.DeltaManipulation;
        //        int currentZoom = 0;
        //        if (deltaManipulation.Expansion.X > 0 && deltaManipulation.Expansion.Y > 0)
        //        {
        //            m_touchZoom += (int)deltaManipulation.Expansion.X;
        //            isTouchZoom = true;
        //        }
        //        else if (deltaManipulation.Expansion.X < 0 && deltaManipulation.Expansion.Y < 0)
        //        {
        //            m_touchZoom += (int)deltaManipulation.Expansion.X;
        //            isTouchZoom = true;
        //        }

        //        if (e.DeltaManipulation.Translation.Y != 0)
        //        {
        //            if (parent.ZoomMode == ZoomMode.FitPage)
        //            {
        //                if (e.DeltaManipulation.Translation.Y > 0)
        //                {
        //                    m_touchFitPageTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //                else
        //                {
        //                    m_touchFitPageTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //            }
        //            else
        //            {
        //                if (e.DeltaManipulation.Translation.Y > 0)
        //                {
        //                    m_touchTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //                else
        //                {
        //                    m_touchTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //            }
        //        }
        //    }
        //}

        //private void ItemsControl_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    if (this.parent.ZoomMode == ZoomMode.FitPage)
        //    {
        //        m_touchFitPageTransition = 0;
        //    }
        //    else
        //    {
        //        if (m_touchTransition > 0)
        //        {
        //            UpdateOffset(-m_touchTransition);
        //        }
        //        else if (m_touchTransition < 0)
        //        {
        //            UpdateOffset(-m_touchTransition);
        //        }
        //        m_touchTransition = 0;
        //    }

        //    if (m_touchZoom != 0 && isTouchZoom)
        //    {
        //        int percentage = (int)((ZoomFactor * 100) + (m_touchZoom * 2));
        //        float m_zoomFactor = (float)percentage / 100;
        //        ZoomTo(m_zoomFactor);
        //    }
        //}

    }
     /// <summary>
    /// 
    /// </summary>
    internal class VisualContainer : FrameworkElement
    {
        private readonly VisualCollection children;
        public VisualContainer()
        {
            children = new VisualCollection(this);
        }

        public void AddVisual(Visual v)
        {
            children.Add(v);
        }
        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }
        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }
    }
    public class ZoomEventArgs : EventArgs
    {
        int m_currentZoomPercentage;

        public ZoomEventArgs(int zoomPercentage)
        {
            m_currentZoomPercentage = zoomPercentage;
        }

        public int ZoomPercentage
        {
            get
            {
                return m_currentZoomPercentage;
            }
        }
    }
}
