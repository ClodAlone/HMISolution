#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;
using Windows.Storage;
using Windows.Foundation;
using System.IO;
#if SyncfusionFramework4_5
using Syncfusion.DirectXWrapper.WinRT;
#endif
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Globalization;
using Windows.Globalization;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Input;
using System.Threading;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Input;

namespace Syncfusion.Windows.PdfViewer
{
    [TemplatePart(Name = "documentScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PdfDocumentPanel", Type = typeof(Canvas))]
    [TemplatePart(Name = "PrintPanel", Type = typeof(Canvas))]
#if !SyncfusionFramework4_5
    [TemplatePart(Name = "ThumbnailViewer", Type = typeof(ThumbnailView))]
    [TemplatePart(Name = "progressIndicator", Type = typeof(ProgressRing))]
    [TemplatePart(Name = "zoomInGridView", Type = typeof(GridView))]
    [TemplatePart(Name = "semanticZoom", Type = typeof(SemanticZoom))]
#endif
    internal class PdfDocumentView : Control
    {
        private Dictionary<object, int> m_pageKidsCollection = new Dictionary<object, int>();
        PdfUnitConvertor m_convertor = new PdfUnitConvertor();
        Dictionary<int, double> m_pageLocation = new Dictionary<int, double>();
        PdfLoadedDocument m_loadedDocument;
        /// <summary>
        /// Internal variable to hold the initialized pdf document page.
        /// </summary>
#if !SyncfusionFramework4_5
        internal Dictionary<int, PdfDocumentPage> m_initializedDocumentPages = new Dictionary<int, PdfDocumentPage>();
        private bool IsSearchRotatePages = false;
        private int txtSearchInitiationPageIndex = 0;
        private TargetTextProperties previousFoundTextInstance;
        private Line PreviousTextHighLightLine = new Line();
        private int prevPageTextSearch = -1;
        private List<int> highlightedPagesList = new List<int>();
#endif

#if SyncfusionFramework4_5
        WinRTRenderer renderer;
        PdfDocumentPage documentPage = null;
        private Dictionary<int, PdfDocumentPage> m_initializedDocumentPages = new Dictionary<int, PdfDocumentPage>();
#else
        internal ThumbnailView m_thumbnailView;
        private global::Windows.Data.Pdf.PdfDocument _pdfDocument;
        internal List<PdfTextCoordinates> textCoordinatesList = new List<PdfTextCoordinates>();
        internal bool IsTextSelectionEnabled = true;
#endif
        internal int m_pageCount;
        int currentPageHeight = 0;
        double zoomFactor = 1;
        double verticalScrollPosition = 0;
        double backVerticalOffset = 0;
        double initialScrollableHeight = 0;
        bool IsZoomChanged = false;
        private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        internal Dictionary<int, double> m_pagesRenderedWithZoomFactor = new Dictionary<int, double>();
        private double m_originalWidth;
        private double m_originalHeight;
        int i = 0;
        int backPageIndex = 10;
        int maxi, mini;
        int count = 0;
        int pageNumberDisplayCount = 0;
        double gotoHeight = 0;
        int pageGapHeight = 10;
        private Canvas PdfDocumentPanel;
        private Canvas PrintPanel;
        internal ScrollViewer documentScrollViewer;
        PrintDocument printDocument;
        IPrintDocumentSource printDocumentSource;
#if SyncfusionFramework4_5
        internal List<UIElement> printPreviewPages = new List<UIElement>();
#endif
        double marginWidth;
        double marginHeight;
        private List<string> m_imagePaths = new List<string>();
        private SfPdfViewerControl m_pdfViewer;
        private bool m_showPageNumber = true;
#if !SyncfusionFramework4_5
        ProgressRing progressIndicator;
        GridView zoomInGridView;
        internal SemanticZoom semanticZoom;
        ManualResetEvent m_mreThumbnail = new ManualResetEvent(false);
        ManualResetEvent m_mre = new ManualResetEvent(true);
        Dictionary<int, InMemoryRandomAccessStream> m_thumbCacheImages = new Dictionary<int, InMemoryRandomAccessStream>();
        private object s_lock = new object();
        private List<int> thumbnailRenderedPages = new List<int>();
        List<object> thumbnailCanvas = new List<object>();
        internal Dictionary<int, UIElement> printPreviewPages = new Dictionary<int, UIElement>();
#endif
        internal bool ShowPageNumber
        {
            get
            {
                return m_showPageNumber;
            }
            set
            {
                if (m_showPageNumber != value)
                {
                    m_showPageNumber = value;
                }
            }
        }
        internal double VerticalScrollPosition
        {
            get
            {
                return verticalScrollPosition;
            }
            set
            {
                verticalScrollPosition = value;
            }
        }

        internal double ZoomFactor
        {
            get
            {
                return zoomFactor;
            }
            set
            {
                zoomFactor = value;
            }
        }

        ///// <summary>
        ///// Gets or sets the PdfLoadedDocument
        ///// </summary>
        internal PdfLoadedDocument LoadedDocument
        {
            get
            {
                return m_loadedDocument;
            }
            set
            {
                m_loadedDocument = value;
                m_pageCount = m_loadedDocument.PageCount;
            }
        }

        internal double OriginalWidth
        {
            get
            {
                return m_originalWidth;
            }
        }

        internal double OriginalHeight
        {
            get
            {
                return m_originalHeight;
            }
        }

        internal int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(PdfDocumentView), new PropertyMetadata(0));

        public PdfDocumentView()
        {
            DefaultStyleKey = typeof(PdfDocumentView);
            this.IsTabStop = true;
        }
        public PdfDocumentView(SfPdfViewerControl ctrl)
        {
            DefaultStyleKey = typeof(PdfDocumentView);
            this.IsTabStop = true;
            m_pdfViewer = ctrl;

        }

        void zoomInGridView_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        void documentScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
#if !SyncfusionFramework4_5
            documentScrollViewer.ChangeView(null, gotoHeight, null);
#endif
        }
        internal bool isThumbnail = false;
        void documentScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate)
            {
                m_dispatcherTimer.Start();
                if (zoomFactor != documentScrollViewer.ZoomFactor)
                {
                    zoomFactor = documentScrollViewer.ZoomFactor;
                    IsZoomChanged = true;
                    m_pdfViewer.InternalZoom = documentScrollViewer.ZoomFactor * 100;
                    return;
                }
            }
            else
            {

                if (zoomFactor != documentScrollViewer.ZoomFactor)
                {
                    documentScrollViewer.UpdateLayout();
                    m_dispatcherTimer.Stop();
                }
#if !SyncfusionFramework4_5
                if (m_onInitialLoad)
                {
                    m_dispatcherTimer.Start();
                    m_onInitialLoad = false;
                    documentScrollViewer.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                    progressIndicator.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
                }
#endif
            }
            gotoHeight = (sender as ScrollViewer).VerticalOffset;
        }

#if !SyncfusionFramework4_5

        internal async Task OnDemandThumbnail(int j)
        {
            for (int pageNumber = j; pageNumber < j + m_thumbnailView.NumberOfImagesInViewport + 4; pageNumber++)
            {
                if (m_pageCount > pageNumber && !thumbnailRenderedPages.Contains(pageNumber))
                {
                    thumbnailRenderedPages.Add(pageNumber);
                    await RenderThumbnail(pageNumber);
                }
            }

            List<int> renderedThumbnails = new List<int>(thumbnailRenderedPages);
            foreach (int num in renderedThumbnails)
            {
                if ((num > m_thumbnailView.pageIndex + m_thumbnailView.NumberOfImagesInViewport + 4 || num < m_thumbnailView.pageIndex - m_thumbnailView.NumberOfImagesInViewport))
                {
                    object UIElementBorder;
                    UIElementBorder = thumbnailCanvas[num];
                    if (UIElementBorder is Border)
                    {
                        Border border = UIElementBorder as Border;
                        Canvas page = border.Child as Canvas;
                        if (page.Children.Count >= 2)
                        {
                            Image img = page.Children[page.Children.Count - 1] as Image;
                            img.Source = null;
                            page.Children.RemoveAt(page.Children.Count - 1);
                            thumbnailRenderedPages.Remove(num);
                        }
                    }
                }
            }
        }


#endif
        protected override void OnApplyTemplate()
        {
            PdfDocumentPanel = GetTemplateChild("PdfDocumentPanel") as Canvas;
            PrintPanel = GetTemplateChild("PrintPanel") as Canvas;
            documentScrollViewer = GetTemplateChild("documentScrollViewer") as ScrollViewer;
#if !SyncfusionFramework4_5
            m_thumbnailView = GetTemplateChild("ThumbnailViewer") as ThumbnailView;
            m_thumbnailView.initialize(this);
            progressIndicator = GetTemplateChild("progressIndicator") as ProgressRing;
            zoomInGridView = GetTemplateChild("zoomInGridView") as GridView;
            semanticZoom = GetTemplateChild("semanticZoom") as SemanticZoom;
            m_thumbnailView.SemanticZoom = semanticZoom;
#endif
            documentScrollViewer.ViewChanged += documentScrollViewer_ViewChanged;
            if (this.LoadedDocument != null)
            {
                LoadPages();
                DocumentLoadedEventArgs args = new DocumentLoadedEventArgs();
                m_pdfViewer.OnDocumentLoaded(args);
            }
        }

        private void HidePageNumber()
        {
            if (ShowPageNumber)
            {
                for (int j = 0; j < m_pageCount; j++)
                {
                    if (j >= 0 && j < m_pageCount)
                        if (PdfDocumentPanel.Children[j] as Canvas != null)
                            ((PdfDocumentPanel.Children[j] as Canvas).Children[1] as Border).Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                }
            }
            else
            {
                for (int j = 0; j < m_pageCount; j++)
                {
                    if (j >= 0 && j < m_pageCount)
                        if (PdfDocumentPanel.Children[j] as Canvas != null)
                            ((PdfDocumentPanel.Children[j] as Canvas).Children[1] as Border).Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        internal Image[] GetPages(int startIndex, int endIndex, float exportZoomFactor)
        {
            if (endIndex < startIndex)
                return null;
            if (endIndex > m_pageCount)
                return null;

            int imageCount = endIndex - startIndex;
            List<Image> images = new List<Image>();
            Image image;

#if SyncfusionFramework4_5
            WinRTRenderer renderer;
            PdfDocumentPage page;
#endif
            for (int i = startIndex; i <= endIndex; i++)
            {
#if !SyncfusionFramework4_5
                {
                    if (_pdfDocument != null && _pdfDocument.PageCount > 0)
                    {
                        //Get Pdf page
                        var pdfPage = _pdfDocument.GetPage(uint.Parse(i.ToString()));

                        if (pdfPage != null)
                        {
                            InMemoryRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                            global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
                            Size pdfPageSize = pdfPage.Size;
                            pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * (exportZoomFactor / 100));
                            pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * (exportZoomFactor / 100));

                            IAsyncAction actionResult = pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
                            actionResult.AsTask().Wait();

                            pdfPage.Dispose();
                            image = new Image();
                            image.Name = Guid.NewGuid().ToString();
                            BitmapImage src = new BitmapImage();
                            src.SetSource(randomStream);
                            image.Source = src;
                            randomStream.Dispose();
                            images.Add(image);
                        }
                    }

                }
#else
                {
                    if (!m_initializedDocumentPages.ContainsKey(i))
                    {
                        PdfPageBase pageToBeRendered = m_loadedDocument.Pages[i];
                        page = new PdfDocumentPage(pageToBeRendered);
                        page.Initialize(pageToBeRendered, true);
                        m_initializedDocumentPages.Add(i, page);
                    }
                    else
                        page = m_initializedDocumentPages[i];

                    if (WinRTRenderer.Cancel)
                        WinRTRenderer.Cancel = false;
                    renderer = new WinRTRenderer(exportZoomFactor / 100, true, this);
                    //if (WinRTRenderer.m_initializedDocumentPages.Count == 0)
                    renderer.m_lDoc = m_loadedDocument;
                    renderer.docPage = PdfDocumentPanel;
                    image = new Image();
                    image.Width = m_convertor.ConvertFromPixels(page.Width, Pdf.Graphics.PdfGraphicsUnit.Point) * exportZoomFactor / 100;
                    image.Height = m_convertor.ConvertFromPixels(page.Height, Pdf.Graphics.PdfGraphicsUnit.Point) * exportZoomFactor / 100;
                    renderer.Render(i);
                    if (PdfDocumentPanel.Children[i] is Canvas)
                    {
                        foreach (UIElement e in (PdfDocumentPanel.Children[i] as Canvas).Children)
                        {
                            if (e is Image)
                            {
                                images.Add(e as Image);
                                break;
                            }
                        }
                    }
                    if (m_pagesRenderedWithZoomFactor.ContainsKey(i))
                        m_pagesRenderedWithZoomFactor.Remove(i);
                    m_pagesRenderedWithZoomFactor.Add(i, exportZoomFactor / 100);
                }
#endif
            }
            return images.ToArray();
        }

        internal void Unload()
        {
            m_dispatcherTimer.Stop();
#if SyncfusionFramework4_5
            WinRTRenderer.Cancel = true;
#endif
            this.documentScrollViewer.ScrollToVerticalOffset(0);
            this.documentScrollViewer.ZoomToFactor(1);
            foreach (UIElement element in PdfDocumentPanel.Children)
            {
                if (element is Canvas)
                {
                    Canvas page = element as Canvas;
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
#if SyncfusionFramework4_5
            if (renderer != null)
            {
                if (renderer.m_lDoc != null)
                    renderer.m_lDoc.Dispose();
                renderer.m_lDoc = null;
                renderer.m_initializedDocumentPages.Clear();
            }
            FontStructure.fontReference.Clear();
#else
            progressIndicator.IsActive = true;
            m_onInitialLoad = true;
            documentScrollViewer.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            progressIndicator.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
            m_thumbCacheImages.Clear();
            thumbnailCanvas = new List<object>();
            thumbnailRenderedPages = new List<int>();
            renderedImages = new Dictionary<int, double>();
            m_thumbnailView.Clear();
            m_thumbCacheImages = new Dictionary<int, InMemoryRandomAccessStream>();
            m_thumbnailView.Timer.Stop();
#endif
            UnregisterForPrint();
            PrintPanel.Children.Clear();
#if SyncfusionFramework4_5
            FontStructure.m_fontCache.Clear();
            Syncfusion.PdfViewer.Base.TextElement.m_graphicsBrushes.Clear();
#endif
            PdfDocumentPanel.Children.Clear();
            PdfDocumentPanel.ClearValue(Canvas.HeightProperty);
            PageIndex = 0;
            m_pageCount = 0;
            zoomFactor = 1;
            m_pageLocation.Clear();
#if !SyncfusionFramework4_5
            documentScrollViewer.ChangeView(null, null, 1);
#endif
            IsZoomChanged = true;
#if !SyncfusionFramework4_5
            documentScrollViewer.ChangeView(null, 0, null);
#endif
            m_pagesRenderedWithZoomFactor.Clear();
#if SyncfusionFramework4_5
            documentPage = null;
            renderer = null;
#endif
            currentPageHeight = 0;
            if (m_loadedDocument != null)
            {
                m_loadedDocument.Close(true);
                m_loadedDocument = null;
            }
#if SyncfusionFramework4_5
            foreach (KeyValuePair<string, IRandomAccessStream> imageStream in WinRTRenderer.RenderedImages)
            {
                imageStream.Value.Dispose();
            }
            WinRTRenderer.RenderedImages.Clear();
            IAsyncOperation<StorageFolder> folder = ApplicationData.Current.LocalFolder.CreateFolderAsync("Fonts", CreationCollisionOption.OpenIfExists);
            folder.AsTask().Wait();
            StorageFolder _folder = folder.GetResults();
            _folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
#endif
        }

        /// <summary>
        /// Print the pages in viewer.
        /// </summary>
        internal async void Print()
        {
            UIDispatcher.Execute(async () =>
            {
                RegisterForPrint();
                await PrintManager.ShowPrintUIAsync();
            });
        }
#if DEBUG
        public List<System.IO.Stream> ExportAsImage(int startIndex, int endIndex, float exportZoomFactor)
        {
            if (endIndex < startIndex)
                return null;
            if (endIndex > m_pageCount)
                return null;

            int imageCount = endIndex - startIndex;
            List<System.IO.Stream> images = new List<System.IO.Stream>();
#if SyncfusionFramework4_5
            WinRTRenderer renderer;
            System.IO.Stream image;
            PdfDocumentPage page;
            for (int i = startIndex; i <= endIndex; i++)
            {
                //page = new PdfDocumentPage(this.m_loadedDocument.Pages[i]);
                //page.Initialize(this.m_loadedDocument.Pages[i], true);
                renderer = new WinRTRenderer(exportZoomFactor / 100, true, this);
                renderer.m_lDoc = this.m_loadedDocument;
                image = renderer.ExportAsImage(i);
                images.Add(image);
            }
#endif
            return images;
        }
#endif
        internal void Dispose()
        {
#if SyncfusionFramework4_5
            FontStructure.m_fontCache.Clear();
            Syncfusion.PdfViewer.Base.TextElement.m_graphicsBrushes.Clear();
            m_initializedDocumentPages.Clear();
#endif
            PageIndex = 0;
            m_pageCount = 0;
            zoomFactor = 1;
            m_pageLocation.Clear();
            IsZoomChanged = true;
            m_pagesRenderedWithZoomFactor.Clear();
#if SyncfusionFramework4_5
            documentPage = null;
            renderer = null;
#endif
            currentPageHeight = 0;
            if (m_loadedDocument != null)
            {
                m_loadedDocument.Close(true);
                m_loadedDocument = null;
            }
        }

        /// <summary>
        /// Prepare images for print.
        /// </summary>
#if SyncfusionFramework4_5
        private void PrepareForPrint(int startIndex, int count)
        {
            PrintPanel.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
            StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
            PrepareForPrint(startIndex, count, tempFolder);
            
            PrintPanel.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            tempFolder = null;
        }
#else
        private async Task<int> PrepareForPrint(int startIndex, int count)
        {
            PrintPanel.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
            StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
            int result = await PrepareForPrint(startIndex, count, tempFolder);

            PrintPanel.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            tempFolder = null;
            return result;
        }
#endif
#if SyncfusionFramework4_5
        private void PrepareForPrint(int p, int count, StorageFolder tempfolder)
        {
            for (int i = p; i < count; i++)
            {
                if (PrintPanel.Children.Count >= i + 1)
                    continue;

                Canvas canvas = new Canvas();
                PdfDocumentPage docPage;
                if (!m_initializedDocumentPages.ContainsKey(i))
                {
                    PdfPageBase pageToBeRendered = m_loadedDocument.Pages[i];
                    docPage = new PdfDocumentPage(pageToBeRendered);
                    docPage.Initialize(pageToBeRendered, true);
                    m_initializedDocumentPages.Add(i, docPage);
                }
                else
                    docPage = m_initializedDocumentPages[i];

                if (WinRTRenderer.Cancel)
                    WinRTRenderer.Cancel = false;

                    renderer = new WinRTRenderer(1f, true, this);
                    //if (WinRTRenderer.m_initializedDocumentPages.Count == 0)
                    renderer.m_lDoc = m_loadedDocument;
                    //renderer = new WinRTRenderer(docPage, 1f, true);
                    renderer.docPage = PdfDocumentPanel;
                    renderer.RenderAsImage(i, true);
                    ImageBrush brush = new ImageBrush();
                    m_imagePaths.Add(renderer.ImageFilePath);
                    brush.ImageSource = new BitmapImage(new Uri(renderer.ImageFilePath));
                    canvas.Background = brush;
                    canvas.Width = (PdfDocumentPanel.Children[i] as Canvas).Width;
                    canvas.Height = (PdfDocumentPanel.Children[i] as Canvas).Height;

                    PrintPanel.Children.Add(canvas);

                ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                PrintPanel.InvalidateMeasure();
                PrintPanel.UpdateLayout();
            }
            
        }
#else
        /// <summary>
        /// Prepare images for print.
        /// </summary>

        private async Task<int> PrepareForPrint(int p, int count, StorageFolder tempfolder)
        {
            for (int i = p; i < count; i++)
            {


                ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
                var pdfPage = _pdfDocument.GetPage(uint.Parse(i.ToString()));
                IRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
                Size pdfPageSize = pdfPage.Size;
                PdfUnitConvertor convertor = new PdfUnitConvertor();

                pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * zoomFactor);
                pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * zoomFactor);
                await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);

                Canvas canvas = PrintPanel.Children[i] as Canvas;
                canvas.Width = (PdfDocumentPanel.Children[i] as Canvas).Width;
                canvas.Height = (PdfDocumentPanel.Children[i] as Canvas).Height;
                imageCtrl = new Image();
                BitmapImage src = new BitmapImage();
                randomStream.Seek(0);
                src.SetSource(randomStream);
                imageCtrl.Source = src;
                canvas.Children.Add(imageCtrl);
                randomStream.Dispose();
                pdfPage.Dispose();
                printDocument.AddPage(printPreviewPages[i]);
                if ((printPreviewPages[i] as Canvas).Children.Count > 0)
                    (printPreviewPages[i] as Canvas).Children.RemoveAt(0);
                ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                PrintPanel.InvalidateMeasure();
                PrintPanel.UpdateLayout();
            }
            return 0;
        }
#endif
        /// <summary>
        /// Registers and triggers print events.
        /// </summary>
        private void RegisterForPrint()
        {
            printPreviewPages.Clear();
            m_imagePaths = new List<string>();
            PrintPanel.Children.Clear();
#if SyncfusionFramework4_5
            PrepareForPrint(0, 1);
#else
            for (int j = 0; j < PdfDocumentPanel.Children.Count; j++)
            {
                Canvas canvas = new Canvas();
                canvas.Width = (PdfDocumentPanel.Children[j] as Canvas).Width;
                canvas.Height = (PdfDocumentPanel.Children[j] as Canvas).Height;

                PrintPanel.Children.Add(canvas);
            }
#endif
            // Create the PrintDocument.
            printDocument = new PrintDocument();

            // Save the DocumentSource.
            printDocumentSource = printDocument.DocumentSource;

            // Add an event handler which creates preview pages.
            printDocument.Paginate += CreatePrintPreviewPages;

            // Add an event handler which provides a specified preview page.
            printDocument.GetPreviewPage += GetPrintPreviewPage;

            // Add an event handler which provides all final print pages.
            printDocument.AddPages += AddPrintPages;

            // Create a PrintManager and add a handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// Pages are prepared for preview.
        /// </summary>
        private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            PrintPageDescription pageDescription = printingOptions.GetPageDescription((uint)e.CurrentPreviewPageNumber);

            marginWidth = pageDescription.PageSize.Width;
            marginHeight = pageDescription.PageSize.Height;
#if SyncfusionFramework4_5
            PrepareForPrint(1, PdfDocumentPanel.Children.Count);
#endif
            AddOnePrintPreviewPage();

            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPageCount(m_pageCount, PreviewPageCountType.Final);
        }

        /// <summary>
        /// Addes the pages for preview.
        /// </summary>
        private void AddOnePrintPreviewPage()
        {
            for (int i = 0; i < PdfDocumentPanel.Children.Count; i++)
            {
                Canvas print = PrintPanel.Children[i] as Canvas;
                if (print != null)
                {
                    print.Width = marginWidth;
                    print.Height = marginHeight;
#if !SyncfusionFramework4_5
                    if (printPreviewPages.Count < i + 1)
                        printPreviewPages.Add(i, print);
#else
                    if (printPreviewPages.Count < i + 1)
                        printPreviewPages.Add(print);
#endif
                }
            }
        }

        /// <summary>
        /// Sets the specific page as preview.
        /// </summary>
        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
#if !SyncfusionFramework4_5
            Canvas print = PrintPanel.Children[e.PageNumber - 1] as Canvas;

            if (print != null)
            {
                int pageNum = e.PageNumber - 1;

                ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
                var pdfPage = _pdfDocument.GetPage(uint.Parse(pageNum.ToString()));
                IRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
                Size pdfPageSize = pdfPage.Size;
                PdfUnitConvertor convertor = new PdfUnitConvertor();

                pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * zoomFactor);
                pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * zoomFactor);
                IAsyncAction actionResult = pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
                actionResult.AsTask().Wait();
                Canvas canvas = PrintPanel.Children[pageNum] as Canvas;
                canvas.Width = (PdfDocumentPanel.Children[pageNum] as Canvas).Width;
                canvas.Height = (PdfDocumentPanel.Children[pageNum] as Canvas).Height;
                imageCtrl = new Image();
                BitmapImage src = new BitmapImage();
                randomStream.Seek(0);
                src.SetSource(randomStream);
                imageCtrl.Source = src;
                if (canvas.Children.Count < 1)
                    canvas.Children.Add(imageCtrl);
                else
                {
                    canvas.Children.RemoveAt(0);
                    canvas.Children.Add(imageCtrl);
                }

                pdfPage.Dispose();
                randomStream.Dispose();
                ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                PrintPanel.InvalidateMeasure();
                PrintPanel.UpdateLayout();

            }
#endif
            PrintDocument printDoc = (PrintDocument)sender;

            printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// Adds the pages to the print document and notifies the print list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if SyncfusionFramework4_5
        private void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            for (int i = 0; i < printPreviewPages.Count; i++)
                printDocument.AddPage(printPreviewPages[i]);

            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.AddPagesComplete();
        }
#else
        private async void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            try
            {
                await PrepareForPrint(0, PdfDocumentPanel.Children.Count);
                //for (int i = 0; i < printPreviewPages.Count; i++)
                //    printDocument.AddPage(printPreviewPages[i]);

                PrintDocument printDoc = (PrintDocument)sender;
                printDoc.AddPagesComplete();
            }
            catch
            {
                PrintDocument printDoc = (PrintDocument)sender;
                printDoc.InvalidatePreview();
            }
        }
#endif
        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = e.Request.CreatePrintTask("Syncfusion PdfViewer", sourceRequested => sourceRequested.SetSource(printDocumentSource));
            printTask.Completed += printTask_Completed;
        }

        private void printTask_Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            UIDispatcher.Execute(() =>
            {
                UnregisterForPrint();
            });
        }

        /// <summary>
        /// Unregisters print events.
        /// </summary>
        private void UnregisterForPrint()
        {
            if (printDocument == null)
                return;

            printDocument.Paginate -= CreatePrintPreviewPages;
            printDocument.GetPreviewPage -= GetPrintPreviewPage;
            printDocument.AddPages -= AddPrintPages;

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
            for (int c = 0; c < PrintPanel.Children.Count; c++)
            {
                UIElement printCanvas = PrintPanel.Children[c];
                if (printCanvas is Canvas)
                {
                    Canvas canvas = printCanvas as Canvas;
                    if (canvas.Children[0] is Image)
                    {
                        Image printImage = canvas.Children[0] as Image;
                        printImage.Source = null;
                        printImage.ReleasePointerCaptures();
                        canvas.Children.Remove(printImage);
                    }
                }
            }
            PrintPanel.Children.Clear();
            printPreviewPages.Clear();

            StorageFolder roamingFolder = ApplicationData.Current.TemporaryFolder;
            IAsyncOperation<IReadOnlyList<StorageFile>> files = roamingFolder.GetFilesAsync();
            files.AsTask().Wait();
            foreach (StorageFile file in files.GetResults())
            {
                if (m_imagePaths.Contains(file.Path))
                    file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            files.Close();

            m_imagePaths.Clear();
        }
        internal static int curPageNo = 0;
        internal Guid controlName = Guid.Empty;
        bool IsScrollviewerInitialized = false;
        internal float thumbnailZoomFactor;
        async void m_dispatcherTimer_Tick(object sender, object e)
        {
#if !SyncfusionFramework4_5
            if (m_pdfViewer.IsThumbnailViewEnabled)
            {
                if (this.documentScrollViewer.ViewportHeight > 0 && IsLoadPagesInvoked)
                {
                    var pdfPage = _pdfDocument.GetPage(uint.Parse("0"));
                    double viewportHeight = this.documentScrollViewer.ViewportHeight;
                    double thumbnailHeight;

                    double pageHeight = pdfPage.Size.Height;
                    double pageWidth = pdfPage.Size.Width;
                    float sx = 0;
                    float sy = 0;
                    sx = (float)this.documentScrollViewer.ViewportWidth / (float)pageWidth;
                    sy = (float)this.documentScrollViewer.ViewportHeight / (float)pageHeight;

                    sx = m_convertor.ConvertToPixels(sx, PdfGraphicsUnit.Point);
                    sy = m_convertor.ConvertToPixels(sy, PdfGraphicsUnit.Point);
                    if (sx < 0)
                        sx = 1;

                    if (sy < 0)
                        sy = 1;

                    float fitWidthZoomFactor = Math.Min(sx, sy);

                    if (fitWidthZoomFactor < 1)
                        this.documentScrollViewer.MinZoomFactor = fitWidthZoomFactor;

                    if (pageHeight >= pageWidth)
                    {
                        thumbnailZoomFactor = (float)(260 / pageHeight);
                    }
                    else
                    {
                        thumbnailZoomFactor = (float)(400 / pageWidth);
                    }

                    thumbnailHeight = pageHeight * thumbnailZoomFactor;

                    float estimatedNumberOfRows = (float)(viewportHeight / thumbnailHeight);

                    double remainingGap = viewportHeight % thumbnailHeight;

                    float adjustHeight = (float)(remainingGap / 6);
                    m_thumbnailView.AdjustmentHeight = adjustHeight;

                    thumbnailCanvas = m_thumbnailView.IncludeCanvas((int)(pageWidth * thumbnailZoomFactor), (int)(pageHeight * thumbnailZoomFactor));
                    m_thumbnailView.Iinitial = true;
                    m_thumbnailView.Width = zoomInGridView.ActualWidth;

                    if (m_thumbnailView.Scrollviewer != null)
                        m_thumbnailView.Timer.Start();
                    IsLoadPagesInvoked = false;
                    this.documentScrollViewer.ScrollToVerticalOffset(0);
                    if (this.m_pdfViewer.ViewMode == PageViewMode.FitWidth)
                        this.documentScrollViewer.ZoomToFactor((float)(this.m_pdfViewer.ActualWidth / this.OriginalWidth));
                    else if (this.m_pdfViewer.ViewMode == PageViewMode.OnePage)
                        this.documentScrollViewer.ZoomToFactor((float)(this.OriginalWidth / this.m_pdfViewer.ActualWidth));
                    else
                        this.documentScrollViewer.ZoomToFactor(1);
                }
                if (IsScrollviewerInitialized && this.documentScrollViewer.ViewportHeight != 0)
                {
                    m_thumbnailView.RequiredViewportHeight = this.documentScrollViewer.ViewportHeight;
                }
                if (semanticZoom.ZoomedInView.IsActiveView && !m_thumbnailView.IsToggled)
                {
                    m_thumbnailView.IsToggled = true;
                }
            }
            else
            {
                if (semanticZoom.ZoomedOutView.IsActiveView)
                    semanticZoom.ToggleActiveView();
            }
#endif
            if (this.m_pageCount != PdfDocumentPanel.Children.Count)
                return;

            documentScrollViewer.ScrollToVerticalOffset(gotoHeight);
            documentScrollViewer.UpdateLayout();
            this.UpdateLayout();
            verticalScrollPosition = documentScrollViewer.VerticalOffset;
            gotoHeight = documentScrollViewer.VerticalOffset;
            if (initialScrollableHeight == 0)
                initialScrollableHeight = documentScrollViewer.ScrollableHeight;
            double pageStart1;
            int pageIndex1;
            GetPageByOffset(verticalScrollPosition / zoomFactor, out pageIndex1, out pageStart1);
            PageIndex = pageIndex1;
            curPageNo = PageIndex;
            if (backPageIndex != PageIndex || IsZoomChanged)
            {
                count = 0;
                pageNumberDisplayCount = 0;
                i = PageIndex;
            }
            if (count == 1)
                i = maxi;
            else if (count == 2)
                i = mini;
            else if (pageIndex1 == 0)
                i = 0;

            if (PageIndex == m_pageCount - 1 && i == mini && m_pagesRenderedWithZoomFactor.ContainsKey(PageIndex - 2))
            {
                m_pagesRenderedWithZoomFactor.Remove(PageIndex - 2);
            }
            //MSRenderingCodeChange
            if (i >= 0)
            {
                UIElement element = PdfDocumentPanel.Children[i];
                if (element is Canvas)
                {
                    if (m_pagesRenderedWithZoomFactor.ContainsKey(i))
                    {
                        if (m_pagesRenderedWithZoomFactor[i] != zoomFactor)
                            IsZoomChanged = true;
                    }
                    double diff = Math.Abs(verticalScrollPosition - backVerticalOffset);
                    //MSRenderingCodeChange
#if !SyncfusionFramework4_5
                    if (renderedImages.ContainsKey(i))
                    {
                        if (renderedImages[i] != zoomFactor)
                            IsZoomChanged = true;
                        else
                            IsZoomChanged = false;
                    }
#else
                    Canvas page = element as Canvas;

                    if (diff > (page.Height * 2))
                        WinRTRenderer.Cancel = true;

                    m_originalWidth = page.Width;
                    m_originalHeight = page.Height;

                    if (WinRTRenderer.RenderedImages.ContainsKey(i.ToString() + "-" + zoomFactor.ToString() + "-" + this.controlName) && !m_pagesRenderedWithZoomFactor.ContainsKey(i))
                    {
                        if (element is Canvas)
                        {
                            Image image = new Image();
                            WriteableBitmap writeBitmap = new WriteableBitmap((int)page.Width, (int)page.Height);
                            IRandomAccessStream randomStream = WinRTRenderer.RenderedImages[i.ToString() + "-" + zoomFactor.ToString() + "-" + this.controlName];
                            randomStream.Seek(0);
                            writeBitmap.SetSource(randomStream);
                            image.Source = writeBitmap;
                            image.Width = page.Width;
                            image.Height = page.Height;
                            global::Windows.UI.Xaml.Shapes.Rectangle PageBorder = new global::Windows.UI.Xaml.Shapes.Rectangle();
                            PageBorder.Height = (int)page.Height + 5;
                            PageBorder.Width = (int)page.Width + 5;
                            PageBorder.Stroke = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 0, 0, 0));
                            double dble = -2.5;

                            Canvas.SetLeft(PageBorder, dble);
                            Canvas.SetTop(PageBorder, dble);
                            Canvas.SetZIndex(PageBorder, 0);

                            page.Children.Add(PageBorder);
                            page.Children.Add(image);
                        }
                        if (m_pagesRenderedWithZoomFactor.ContainsKey(i))
                            m_pagesRenderedWithZoomFactor.Remove(i);
                        m_pagesRenderedWithZoomFactor.Add(i, zoomFactor);
                        Dictionary<int, double> tempPages = new Dictionary<int, double>();
                        foreach (KeyValuePair<int, double> val in m_pagesRenderedWithZoomFactor)
                        {
                            tempPages.Add(val.Key, val.Value);
                        }
                        foreach (KeyValuePair<int, double> val in tempPages)
                        {
                            if (val.Key != i - 1 && val.Key != i + 1 && val.Key != i && val.Key != i + 2 && val.Key != i - 2)
                            {
                                if (m_pagesRenderedWithZoomFactor.ContainsKey(val.Key))
                                {
                                    m_pagesRenderedWithZoomFactor.Remove(val.Key);
                                    UIElement element1 = PdfDocumentPanel.Children[val.Key];
                                    if (element1 is Canvas)
                                    {
                                        Canvas pageCanvas = element1 as Canvas;
                                        for (int k = 0; k < pageCanvas.Children.Count; k++)
                                        {
                                            if (pageCanvas.Children[k] is Image)
                                                (pageCanvas.Children[k] as Image).Source = null;
                                        }
                                    }
                                }
                            }
                        }
                        //count++;
                        //backVerticalOffset = verticalScrollPosition;
                        //backPageIndex = PageIndex;
                    }
#endif
                    if (diff < 1000)
                    {

#if !SyncfusionFramework4_5
                        if (i < m_pageCount && i >= 0)
                            await RenderPDFPage(i);
#else
                        if (i < m_pageCount && i >= 0)
                            RenderPage(i);
                        count++;
                        documentPage = null;
#endif

                        backPageIndex = PageIndex;
                        IsZoomChanged = false;
                    }

                    if ((IsZoomChanged || backVerticalOffset == verticalScrollPosition) && documentScrollViewer.ZoomFactor == ZoomFactor)
                    {
#if !SyncfusionFramework4_5
                        Dictionary<int, double> tempPages = new Dictionary<int, double>();
                        foreach (KeyValuePair<int, double> val in renderedImages)
                        {
                            tempPages.Add(val.Key, val.Value);
                        }
                        foreach (KeyValuePair<int, double> val in tempPages)
                        {
                            if ((val.Key > PageIndex + 5 || val.Key < PageIndex - 5) && val.Key != PageIndex)
                            {
                                if (renderedImages.ContainsKey(val.Key))
                                {

                                    UIElement element1 = PdfDocumentPanel.Children[val.Key];
                                    if (element1 is Canvas)
                                    {
                                        Canvas pageCanvas = element1 as Canvas;
                                        for (int k = 0; k < pageCanvas.Children.Count; k++)
                                        {
                                            if (pageCanvas.Children[k] is Image)
                                            {
                                                (pageCanvas.Children[k] as Image).Source = null;
                                                pageCanvas.Children.RemoveAt(k);
                                                renderedImages.Remove(val.Key);
                                            }
                                        }
                                    }
                                }
                            }
                        }
#endif
                        pageNumberDisplayCount++;
                        if (ShowPageNumber)
                        {
                            if (pageNumberDisplayCount < 100)
                            {
                                for (int j = PageIndex - 1; j < PageIndex + 3; j++)
                                {
                                    if (j >= 0 && j < m_pageCount)
                                        if (PdfDocumentPanel.Children[j] as Canvas != null)
                                            ((PdfDocumentPanel.Children[j] as Canvas).Children[0] as Border).Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                                }
                            }
                            else
                            {
                                for (int j = PageIndex - 1; j < PageIndex + 3; j++)
                                {
                                    if (j >= 0 && j < m_pageCount)
                                        if (PdfDocumentPanel.Children[j] as Canvas != null)
                                            ((PdfDocumentPanel.Children[j] as Canvas).Children[0] as Border).Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
                                }
                            }
                        }
#if SyncfusionFramework4_5
                        foreach (KeyValuePair<string, IRandomAccessStream> renderCanvas in WinRTRenderer.RenderedImages)
                        {
                            string[] canvasNumber = renderCanvas.Key.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                            int z = int.Parse(canvasNumber[0]);
                            if ((z < PageIndex - 1) || (z > PageIndex + 3))
                            {
                                if ((PdfDocumentPanel.Children[z] as Canvas != null))
                                {
                                    if ((PdfDocumentPanel.Children[z] as Canvas).Children.Count >= 2 && ((PdfDocumentPanel.Children[z] as Canvas).Children[1] is Rectangle))
                                    {
                                        ((PdfDocumentPanel.Children[z] as Canvas).Children[1] as Rectangle).ReleasePointerCaptures();
                                        (PdfDocumentPanel.Children[z] as Canvas).Children.RemoveAt(1);
                                        if ((PdfDocumentPanel.Children[z] as Canvas != null) && ((PdfDocumentPanel.Children[z] as Canvas).Children[1] is Image))
                                        {
                                            ((PdfDocumentPanel.Children[z] as Canvas).Children[1] as Image).ReleasePointerCaptures();
                                            (PdfDocumentPanel.Children[z] as Canvas).Children.RemoveAt(1);
                                        }
                                    }
                                }

                            }
                        }
#endif
                    }
                }
            }
            count++;
            if ((IsZoomChanged || backVerticalOffset != verticalScrollPosition) || documentScrollViewer.ZoomFactor != ZoomFactor)
                pageNumberDisplayCount = 0;

            backVerticalOffset = verticalScrollPosition;

            if (IsManualPageNavigation)
            {
#if !SyncfusionFramework4_5
                this.documentScrollViewer.ChangeView(null, this.documentScrollViewer.VerticalOffset + 3, null);
#endif
                IsManualPageNavigation = false;
            }
            if (count > 5)
                count = 0;
        }
        Image imageCtrl = new Image();
#if !SyncfusionFramework4_5
        public virtual void RenderAll(int i)
        {
            if (OnRender != null)
                OnRender(i);
        }
        public event Action<int> OnRender;
        async void WinRTRenderer_OnRender(int e)
        {
            Func<int, Task<int>> invokeRenderContent;
            invokeRenderContent = RenderCacheImages;

            try
            {
                await Task.Factory.StartNew(() => invokeRenderContent(e).ContinueWith(x => { m_mreThumbnail.WaitOne(); }));
            }
            catch (OperationCanceledException)
            {
            }
        }


        internal async Task<int> RenderThumbnail(int pageNumber)
        {
            try
            {
                if (_pdfDocument != null && _pdfDocument.PageCount > 0 && _pdfDocument.PageCount > pageNumber)
                {
                    if (!m_thumbCacheImages.ContainsKey(pageNumber))
                    {
                        //Get Pdf page
                        var pdfPage = _pdfDocument.GetPage(uint.Parse(pageNumber.ToString()));
                        if (pdfPage != null)
                        {
                            InMemoryRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                            global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
                            Size pdfPageSize = pdfPage.Size;
                            pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * thumbnailZoomFactor);
                            pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * thumbnailZoomFactor);
                            await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
                            pdfPage.Dispose();
                            lock (s_lock)
                            {
                                if (!m_thumbCacheImages.ContainsKey(pageNumber))
                                {
                                    randomStream.Seek(0);
                                    m_thumbCacheImages.Add(pageNumber, randomStream);
                                }
                            }
                            Image imageCtrl;
                            object UIElementBorder;
                            if (thumbnailCanvas != null)
                            {
                                UIElementBorder = thumbnailCanvas[pageNumber];
                                if (UIElementBorder is Border)
                                {
                                    Border border = UIElementBorder as Border;
                                    Canvas page = border.Child as Canvas;
                                    imageCtrl = new Image();
                                    imageCtrl.Name = pageNumber.ToString();
                                    BitmapImage src = new BitmapImage();
                                    randomStream.Seek(0);
                                    src.SetSource(randomStream);
                                    imageCtrl.Source = src;
                                    page.Children.Add(imageCtrl);
                                }
                            }
                        }
                    }
                    else
                    {
                        InMemoryRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                        randomStream = m_thumbCacheImages[pageNumber];
                        Image imageCtrl;
                        object UIElementBorder;
                        UIElementBorder = thumbnailCanvas[pageNumber];
                        if (UIElementBorder is Border)
                        {
                            Border border = UIElementBorder as Border;
                            Canvas page = border.Child as Canvas;
                            imageCtrl = new Image();
                            imageCtrl.Name = pageNumber.ToString();
                            BitmapImage src = new BitmapImage();
                            randomStream.Seek(0);
                            src.SetSource(randomStream);
                            imageCtrl.Source = src;
                            page.Children.Add(imageCtrl);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);

            }
            return 0;
        }

        private async Task<int> RenderCacheImages(int pageToBeRender)
        {
            try
            {
                if (_pdfDocument != null && _pdfDocument.PageCount > 0 && _pdfDocument.PageCount > pageToBeRender && !m_thumbCacheImages.ContainsKey(pageToBeRender))
                {
                    var pdfPage = _pdfDocument.GetPage(uint.Parse(pageToBeRender.ToString()));

                    if (pdfPage != null)
                    {
                        InMemoryRandomAccessStream thumbrandomStream = new InMemoryRandomAccessStream();
                        global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
                        Size pdfPageSize = pdfPage.Size;

                        pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * thumbnailZoomFactor);
                        pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * thumbnailZoomFactor);

                        IAsyncAction asc = pdfPage.RenderToStreamAsync(thumbrandomStream, pdfPageRenderOptions);
                        asc.AsTask().Wait();
                        lock (s_lock)
                        {
                            if (!m_thumbCacheImages.ContainsKey(pageToBeRender))
                            {
                                thumbrandomStream.Seek(0);
                                m_thumbCacheImages.Add(pageToBeRender, thumbrandomStream);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);

            }
            return 0;
        }

        Dictionary<int, double> renderedImages = new Dictionary<int, double>();
        private async Task<int> RenderPDFPage(int pageToBeRender)
        {
            try
            {
                if ((!renderedImages.ContainsKey(pageToBeRender)) || IsZoomChanged)
                {

                    if (_pdfDocument != null && _pdfDocument.PageCount > 0)
                    {
                        //if (!m_cachedImages.ContainsKey(pageToBeRender.ToString() + "-" + zoomFactor.ToString()))
                        {

                            for (int pageNumber = pageToBeRender - 5; pageNumber < pageToBeRender + 5; pageNumber++)
                            {
                                if (m_pageCount > pageNumber && pageNumber >= 0)
                                {
                                    if (m_thumbCacheImages.ContainsKey(pageNumber) && !renderedImages.ContainsKey(pageNumber))
                                    {
                                        UIElement element = PdfDocumentPanel.Children[pageNumber];
                                        if (element is Canvas)
                                        {
                                            Canvas page = element as Canvas;
                                            if (page.Children.Count < 2)
                                            {
                                                InMemoryRandomAccessStream randomStream1 = m_thumbCacheImages[pageNumber];
                                                imageCtrl = new Image();
                                                imageCtrl.Name = Guid.NewGuid().ToString();
                                                BitmapImage src = new BitmapImage();
                                                randomStream1.Seek(0);
                                                src.SetSource(randomStream1);
                                                imageCtrl.Source = src;

                                                imageCtrl.Width = m_originalWidth = page.Width;
                                                imageCtrl.Height = m_originalHeight = page.Height;
                                                if (page.Children.Count < 2)
                                                {
                                                    page.Children.Add(imageCtrl);
                                                }
                                                else
                                                {
                                                    page.Children.RemoveAt(1);
                                                    //page.Children.Add(imageCtrl);
                                                    page.Children.Insert(1, imageCtrl);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (renderedImages.ContainsKey(pageToBeRender))
                                renderedImages.Remove(pageToBeRender);
                            renderedImages.Add(pageToBeRender, zoomFactor);

                            m_mre.Reset();
                            m_mreThumbnail = new ManualResetEvent(false);
                            var pdfPage = _pdfDocument.GetPage(uint.Parse(pageToBeRender.ToString()));

                            if (pdfPage != null)
                            {
                                InMemoryRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                                global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
                                Size pdfPageSize = pdfPage.Size;
                                pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * zoomFactor);
                                pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * zoomFactor);

                                await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);

                                pdfPage.Dispose();
                                imageCtrl = new Image();
                                imageCtrl.Name = Guid.NewGuid().ToString();
                                BitmapImage src = new BitmapImage();
                                randomStream.Seek(0);
                                src.SetSource(randomStream);
                                imageCtrl.Source = src;
                                UIElement element = PdfDocumentPanel.Children[pageToBeRender];
                                if (element is Canvas)
                                {
                                    Canvas page = element as Canvas;
                                    imageCtrl.Width = m_originalWidth = page.Width = pdfPage.Size.Width;
                                    imageCtrl.Height = m_originalHeight = page.Height = pdfPage.Size.Height;
                                    if (page.Children.Count < 2)
                                    {
                                        page.Children.Add(imageCtrl);
                                    }
                                    else
                                    {
                                        page.Children.RemoveAt(1);
                                        //page.Children.Add(imageCtrl);
                                        page.Children.Insert(1, imageCtrl);
                                    }
                                }
                                progressIndicator.IsActive = false;
                                if (m_onInitialLoad)
                                {
                                    m_onInitialLoad = false;
                                    documentScrollViewer.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                                    progressIndicator.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
                                }
                            }
                            m_mreThumbnail = new ManualResetEvent(true);
                            m_mre.Set();
                        }

                    }
                }
            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);
            }
            finally
            {
                m_mre.Set();
                if (targetedTxtSearchPageIndex != -1 && foundTextInstances != null)
                {
                    SearchTextWhileScroll(pageToBeRender);
                    HighLightPdfPageFromTimer(targetedTxtSearchPageIndex, searchText);
                }
            }
            return 0;
        }
#else
        private void RenderPage(int i)
        {
            m_pdfViewer.DocumentViewPageIndex = PageIndex;
            UIElement element = PdfDocumentPanel.Children[i];
            if (element is Canvas)
            {
                if (m_pagesRenderedWithZoomFactor.ContainsKey(i))
                {
                    if (m_pagesRenderedWithZoomFactor[i] != zoomFactor)
                        IsZoomChanged = true;
                }
                Canvas page = element as Canvas;

                if (!m_pagesRenderedWithZoomFactor.ContainsKey(i) || IsZoomChanged)
                {
                    ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
                    if (WinRTRenderer.Cancel)
                        WinRTRenderer.Cancel = false;
                    renderer = new WinRTRenderer(zoomFactor, IsZoomChanged, this);
                    //if (WinRTRenderer.m_initializedDocumentPages.Count == 0)
                    renderer.m_lDoc = m_loadedDocument;
                    renderer.docPage = PdfDocumentPanel;
                    renderer.RenderAsImage(i, false);
                    ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                    if (m_pagesRenderedWithZoomFactor.ContainsKey(i))
                        m_pagesRenderedWithZoomFactor.Remove(i);
                    m_pagesRenderedWithZoomFactor.Add(i, zoomFactor);
                    Dictionary<int, double> tempPages = new Dictionary<int, double>();
                    foreach (KeyValuePair<int, double> val in m_pagesRenderedWithZoomFactor)
                    {
                        tempPages.Add(val.Key, val.Value);
                    }
                    foreach (KeyValuePair<int, double> val in tempPages)
                    {
                        if (val.Key != PageIndex - 1 && val.Key != PageIndex + 1 && val.Key != PageIndex)
                        {
                            if (m_pagesRenderedWithZoomFactor.ContainsKey(val.Key))
                            {
                                m_pagesRenderedWithZoomFactor.Remove(val.Key);
                                UIElement element1 = PdfDocumentPanel.Children[val.Key];
                                if (element1 is Canvas)
                                {
                                    Canvas pageCanvas = element1 as Canvas;
                                    for (int k = 0; k < pageCanvas.Children.Count; k++)
                                    {
                                        if (pageCanvas.Children[k] is Image)
                                            (pageCanvas.Children[k] as Image).Source = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (m_pdfViewer.IsIntialViewModeChanged)
            {
                m_pdfViewer.IsIntialViewModeChanged = false;
                SetViewMode();
                IsZoomChanged = true;
                RenderPage(i);
            }
        }
#endif

        private void SetViewMode()
        {
            SfPdfViewerControl viewer = m_pdfViewer;
            double zfactor = 100;
            switch (viewer.ViewMode)
            {
                case PageViewMode.FitWidth:
                    zfactor = (viewer.ActualWidth / this.OriginalWidth) * 100;
                    break;
                case PageViewMode.OnePage:
                    zfactor = (this.OriginalWidth / viewer.ActualWidth) * 100;
                    break;
                case PageViewMode.Normal:
                default:
                    zfactor = 100;
                    break;
            }
            if (!double.IsInfinity(zfactor))
            {
                viewer.InternalZoom = (float)zfactor;
            }
        }

        internal void SetZoom(double zFactor)
        {
            if (zFactor < 1 && zFactor < this.documentScrollViewer.MinZoomFactor)
                zFactor = this.documentScrollViewer.MinZoomFactor;
            double verticalOff = this.documentScrollViewer.VerticalOffset;
            verticalOff = (verticalOff / this.ZoomFactor * zFactor);

            gotoHeight = verticalOff;
            //#if !SyncfusionFramework4_5
            //this.documentScrollViewer.ChangeView(null, verticalOff, null);
            //this.documentScrollViewer.ChangeView(null, null, (float)zFactor);
            this.documentScrollViewer.ScrollToVerticalOffset(verticalOff);
            documentScrollViewer.ZoomToFactor((float)zFactor);
            //#endif
        }

        bool IsManualPageNavigation = false;

        internal void SetScrollHeight(int pageNumber)
        {
            gotoHeight = 0;
            for (int i = 0; i < pageNumber; i++)
            {
                gotoHeight += ((PdfDocumentPanel.Children[i] as Canvas).Height + pageGapHeight) * zoomFactor;
            }
            IsManualPageNavigation = true;
        }
#if !SyncfusionFramework4_5
        internal void SetTextSearchScrollHeight(double value)
        {
            double d = this.documentScrollViewer.VerticalOffset;
            gotoHeight = 0;
            for (int i = 0; i < targetedTxtSearchPageIndex; i++)
            {
                gotoHeight += ((PdfDocumentPanel.Children[i] as Canvas).Height + pageGapHeight) * zoomFactor;
            }
            gotoHeight += ((value) - 100) * zoomFactor;
            IsManualPageNavigation = true;
        }
#endif

        internal void GetPageByOffset(double offset, out int pageIndex, out double pageStart)
        {
            offset += this.documentScrollViewer.ViewportHeight / 2;
            pageIndex = 0;
            pageStart = 0;
            foreach (KeyValuePair<int, double> element in m_pageLocation)
            {
                if (element.Value < offset - 5)
                {
                    pageIndex = element.Key;
                    pageStart = element.Value;
                    while (pageIndex >= m_pageCount)
                    {
                        pageIndex--;
                    }
                }
                else
                {
                    double difference = element.Value - offset;
                    if (difference > this.documentScrollViewer.ViewportHeight / 2)
                    {
                        maxi = pageIndex - 1;
                        mini = pageIndex + 1;
                    }
                    else
                    {
                        maxi = pageIndex + 1;
                        mini = pageIndex - 1;
                    }
                    return;
                }
            }
        }

        void outerBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border)
            {
                Border OuterBorder = sender as Border;
                if (OuterBorder.Child is Canvas)
                {
                    Canvas canvas = OuterBorder.Child as Canvas;
                    if ((canvas.Background as SolidColorBrush).Color.R != 255)
                        (OuterBorder.Child as Canvas).Background = new SolidColorBrush(global::Windows.UI.Colors.Transparent);
                }
            }
        }

        void outerBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border)
            {
                Border OuterBorder = sender as Border;
                if (OuterBorder.Child is Canvas)
                {
                    Canvas canvas = OuterBorder.Child as Canvas;
                    if (canvas.Background == null)
                    {
                        (OuterBorder.Child as Canvas).Background = new SolidColorBrush(global::Windows.UI.Colors.Blue);
                        return;
                    }
                    else
                    {
                        global::Windows.UI.Color backColor = (canvas.Background as SolidColorBrush).Color;
                        if (backColor != global::Windows.UI.Color.FromArgb(255, 255, 0, 0))
                            (OuterBorder.Child as Canvas).Background = new SolidColorBrush(global::Windows.UI.Colors.Blue);
                    }
                }
            }
        }
        private bool m_onInitialLoad = true;
        internal bool IsLoadPagesInvoked;
#if !SyncfusionFramework4_5
        private async void startbackgroundthread()
        {
            Task.Run(() => ParseContentForTextSearch());
        }
#endif
        internal async void LoadPages()
        {
#if !SyncfusionFramework4_5
            textSelectionEnviCreatedList = new List<int>();
            textSelectIndexSequence = new Dictionary<float, Line>();
            textSelectLineSequence = new Dictionary<Line, float>();
            textSelectionDifferentLineEnd = new Dictionary<double, Line>();
            highlightedIndex.Clear();
            TextSelectionPageCollection.Clear();
#endif
#if !SyncfusionFramework4_5
            #region TextSearch
            textSearchCache.Clear();
            m_initializedDocumentPages.Clear();
            searchInstancesInPage = 0;
            currentHighlightedIndex = -1;
            searchText = string.Empty;
            targetedTxtSearchPageIndex = 0;
            IsSearchParsingCompleted = false;
            renderer = new WinRTRenderer(zoomFactor, false, this);
            //WinRTRenderer.textSearchCache.Clear();
            textSearchCache.Clear();
            #endregion
            if (!semanticZoom.ZoomedInView.IsActiveView)
                semanticZoom.ToggleActiveView();
#endif
            if (m_dispatcherTimer.IsEnabled)
                m_dispatcherTimer.Stop();
            this.m_dispatcherTimer.Tick -= m_dispatcherTimer_Tick;
            documentScrollViewer.ViewChanged -= documentScrollViewer_ViewChanged;
            int width = 0, height = 0;
            IsZoomChanged = true;

#if !SyncfusionFramework4_5
            StorageFile pdfFile;
            //PdfDocument document = new PdfDocument();
            //document.ImportPageRange(LoadedDocument, 0, LoadedDocument.PageCount - 1);
            //Stream str = new MemoryStream();

            //document.Save(str);
            //document.Close(true);
            //LoadedDocument = new PdfLoadedDocument(str);

            using (MemoryStream ms = new MemoryStream())
            {
                LoadedDocument.Save(ms);
                //str.Dispose();
                ms.Position = 0;

                byte[] buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);

                StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

                pdfFile = await tempFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".pdf", CreationCollisionOption.ReplaceExisting);
                if (pdfFile != null)
                {
                    IRandomAccessStream randomStream = await pdfFile.OpenAsync(FileAccessMode.ReadWrite);
                    Stream mstr = randomStream.AsStream();

                    mstr.Write(buffer, 0, buffer.Length);

                    mstr.Position = 0;
                    IAsyncOperation<bool> operationResult = randomStream.FlushAsync();
                    operationResult.AsTask().Wait();
                    randomStream.Dispose();
                }
            }
            IAsyncOperation<global::Windows.Data.Pdf.PdfDocument> result = global::Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(pdfFile);

            result.AsTask().Wait();
            _pdfDocument = result.GetResults();

            m_thumbnailView.PageCount = this.m_pageCount;
            IsLoadPagesInvoked = true;
            IsScrollviewerInitialized = true;
            this.OnRender += WinRTRenderer_OnRender;
            startbackgroundthread();
            double tempWidth = 0, tempHeight = 0; ;
            for (int i = 0; i < m_pageCount; i++)
            {
                var pdfPage = _pdfDocument.GetPage(uint.Parse(i.ToString()));
                double firstPageWidth = pdfPage.Size.Width;
                double firstPageHeight = pdfPage.Size.Height;
                if (firstPageWidth > tempWidth)
                {
                    tempWidth = firstPageWidth;
                }
                if (firstPageHeight > tempHeight)
                {
                    tempHeight = firstPageHeight;
                }
            }

            PdfDocumentPanel.Width = tempWidth;

            for (int i = 0; i < m_pageCount; i++)
            {
                var pdfPage = _pdfDocument.GetPage(uint.Parse(i.ToString()));
                double firstPageWidth = pdfPage.Size.Width;
                double firstPageHeight = pdfPage.Size.Height;

                if (firstPageHeight >= firstPageWidth)
                {
                    thumbnailZoomFactor = (float)(260 / firstPageHeight);
                }
                else
                {
                    thumbnailZoomFactor = (float)(400 / firstPageWidth);
                }

                RenderAll(i);
                //if (i == 0)
                {
                    width = (int)firstPageWidth;
                    height = (int)firstPageHeight;
                    IncludeCanvas(i, width, height);
                }
                //else
                //{
                //    width = (int)firstPageWidth;
                //    height = (int)firstPageHeight;
                //    IncludeCanvas(i, width, height);
                //}
            }
#else
            WinRTRenderer.Cancel = false;

            for (int i = 0; i < m_pageCount; i++)
            {
                if (i == 0)
                {
                    documentPage = new PdfDocumentPage(m_loadedDocument.Pages[i]);
                    documentPage.Initialize(m_loadedDocument.Pages[i], true);
                    width = documentPage.Width;
                    height = documentPage.Height;
                    IncludeCanvas(i, documentPage.Width, documentPage.Height);
                }
                else
                {
                    IncludeCanvas(i, width, height);
                }
            }
#endif
#if SyncfusionFramework4_5
            PdfDocumentPanel.Height = currentPageHeight;
#endif
#if !SyncfusionFramework4_5
            documentScrollViewer.Width = zoomInGridView.ActualWidth;
#endif
            documentScrollViewer.ViewChanged += documentScrollViewer_ViewChanged;
            if (m_pdfViewer.ViewMode == PageViewMode.Normal)
            {
                this.zoomFactor = 1;
#if !SyncfusionFramework4_5
                documentScrollViewer.ChangeView(null, null, 1);
#endif
            }
            this.m_dispatcherTimer.Tick += m_dispatcherTimer_Tick;
            m_dispatcherTimer.Start();

#if !SyncfusionFramework4_5
            {
                //UIDispatcher.Execute(async () =>
                //{
                //    ParseContentForTextSearch();
                //});
            }
#endif
        }
#if SyncfusionFramework4_5
        private void IncludeCanvas(int pageIndex, int width, int height)
        {
            Canvas page = new Canvas();
            page.Width = (double)m_convertor.ConvertFromPixels((float)width, PdfGraphicsUnit.Point) + 5;
            page.Height = (double)m_convertor.ConvertFromPixels((float)height, PdfGraphicsUnit.Point) + 5;

            page.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Top;
            page.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            page.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));
            page.Margin = new Thickness(0, 5, 0, 5);

            Border pageNumberDisplay = new Border();

            pageNumberDisplay.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(178, 0, 0, 0));


            TextBlock pageDisplay = new TextBlock();

            global::Windows.UI.Xaml.Documents.Run runPageIndex = new global::Windows.UI.Xaml.Documents.Run();
            runPageIndex.Text = (pageIndex + 1).ToString();
            runPageIndex.FontSize = 21;
            pageDisplay.Inlines.Add(runPageIndex);

            global::Windows.UI.Xaml.Documents.Run runSpace = new global::Windows.UI.Xaml.Documents.Run();
            runSpace.Text = " ";
            runSpace.FontSize = 5;
            pageDisplay.Inlines.Add(runSpace);

            global::Windows.UI.Xaml.Documents.Run runSeperator = new global::Windows.UI.Xaml.Documents.Run();
            runSeperator.Text = "|";
            runSeperator.FontSize = 17;
            pageDisplay.Inlines.Add(runSeperator);

            runSpace = new global::Windows.UI.Xaml.Documents.Run();
            runSpace.Text = " ";
            runSpace.FontSize = 5;
            pageDisplay.Inlines.Add(runSpace);

            global::Windows.UI.Xaml.Documents.Run runPageCount = new global::Windows.UI.Xaml.Documents.Run();
            runPageCount.Text = m_pageCount.ToString();
            runPageCount.FontSize = 12;
            pageDisplay.Inlines.Add(runPageCount);

            pageDisplay.Foreground = new SolidColorBrush(global::Windows.UI.Colors.White);

            pageNumberDisplay.Padding = new Thickness(10, 15, 10, 10);
            pageNumberDisplay.Child = pageDisplay;

            if (!ShowPageNumber)
            {
                pageNumberDisplay.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            }

            Canvas.SetLeft(pageNumberDisplay, 0);
            Canvas.SetTop(pageNumberDisplay, 0);
            Canvas.SetZIndex(pageNumberDisplay, 1);

            page.Children.Add(pageNumberDisplay);

            Canvas.SetLeft(page, 0);
            Canvas.SetTop(page, currentPageHeight);

            currentPageHeight += (int)page.Height + pageGapHeight;
            m_pageLocation.Add(pageIndex + 1, currentPageHeight);
            PdfDocumentPanel.Children.Add(page);
            PdfDocumentPanel.Width = page.Width;
            PdfDocumentPanel.Height = currentPageHeight;
        }

#else
        private void IncludeCanvas(int pageIndex, int width, int height)
        {
            Canvas page = new Canvas();
            page.Width = width;
            page.Height = height;
            page.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Top;
            page.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            page.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));
            page.Margin = new Thickness(0, 5, 0, 5);
            Border pageNumberDisplay = new Border();
            pageNumberDisplay.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(178, 0, 0, 0));
            TextBlock pageDisplay = new TextBlock();
            global::Windows.UI.Xaml.Documents.Run runPageIndex = new global::Windows.UI.Xaml.Documents.Run();
            runPageIndex.Text = (pageIndex + 1).ToString();
            runPageIndex.FontSize = 21;
            pageDisplay.Inlines.Add(runPageIndex);
            global::Windows.UI.Xaml.Documents.Run runSpace = new global::Windows.UI.Xaml.Documents.Run();
            runSpace.Text = " ";
            runSpace.FontSize = 5;
            pageDisplay.Inlines.Add(runSpace);
            global::Windows.UI.Xaml.Documents.Run runSeperator = new global::Windows.UI.Xaml.Documents.Run();
            runSeperator.Text = "|";
            runSeperator.FontSize = 17;
            pageDisplay.Inlines.Add(runSeperator);
            runSpace = new global::Windows.UI.Xaml.Documents.Run();
            runSpace.Text = " ";
            runSpace.FontSize = 5;
            pageDisplay.Inlines.Add(runSpace);
            global::Windows.UI.Xaml.Documents.Run runPageCount = new global::Windows.UI.Xaml.Documents.Run();
            runPageCount.Text = m_pageCount.ToString();
            runPageCount.FontSize = 12;
            pageDisplay.Inlines.Add(runPageCount);
            pageDisplay.Foreground = new SolidColorBrush(global::Windows.UI.Colors.White);
            pageNumberDisplay.Padding = new Thickness(10, 15, 10, 10);
            pageNumberDisplay.Child = pageDisplay;
            if (!ShowPageNumber)
            {
                pageNumberDisplay.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            }
            Canvas.SetLeft(pageNumberDisplay, 0);
            Canvas.SetTop(pageNumberDisplay, 0);
            Canvas.SetZIndex(pageNumberDisplay, 1);
            page.Children.Add(pageNumberDisplay);
            double sx = (PdfDocumentPanel.Width - page.Width) / 2;
            Canvas.SetLeft(page, (float)sx);
            Canvas.SetTop(page, currentPageHeight);
            currentPageHeight += (int)page.Height + pageGapHeight;
            m_pageLocation.Add(pageIndex + 1, currentPageHeight);
            page.Name = pageIndex.ToString();
            page.PointerPressed += new PointerEventHandler(documentview_PointerPressed);
            page.PointerReleased += new PointerEventHandler(documentview_PointerReleased);
            page.PointerMoved += new PointerEventHandler(documentview_PointerMoved);
            page.PointerExited += new PointerEventHandler(page_PointerExited);
            page.DoubleTapped += documentview_DoubleTapped;
            PdfDocumentPanel.Children.Add(page);
            PdfDocumentPanel.Height = currentPageHeight;
        }

        #region TextSearch
        internal Dictionary<int, List<TextSearchElements>> textSearchCache = new Dictionary<int, List<TextSearchElements>>();
        WinRTRenderer renderer;
        List<TargetTextProperties> foundTextInstances;
        int searchInstancesInPage = 0;
        int currentHighlightedIndex = -1;
        string searchText = string.Empty;
        int targetedTxtSearchPageIndex = 0;
        bool IsSearchParsingCompleted = false;
        #region NextSearch
        private async Task ParseContentForTextSearch()
        {
            textSearchCache.Clear();
            if (!IsSearchParsingCompleted)
            {
                for (int i = 0; i < m_loadedDocument.PageCount; i++)
                {
                    renderer = new WinRTRenderer(zoomFactor, IsZoomChanged, this);
                    //renderer.zoomFactor = zoomFactor;
                    //renderer.IsZoomChanged = IsZoomChanged;
                    //renderer.m_documentView = this;
                    renderer.PageIndex = i;
                    renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                    renderer.docPage = PdfDocumentPanel;
                    renderer.RenderTextSearchCache(i);
                    if (i == m_loadedDocument.PageCount - 1)
                    {
                        IsSearchParsingCompleted = true;
                    }
                }
            }
        }

        internal void SearchTextCoordinates(string targetText, int pageIndex, out List<PdfTextCoordinates> resultCoordsList)
        {
            targetedTxtSearchPageIndex = pageIndex;
            if (searchText != targetText && targetedTxtSearchPageIndex < m_pageCount)
            {
                currentHighlightedIndex = -1;
                searchInstancesInPage = 0;
                renderer.zoomFactor = zoomFactor;
                renderer.IsZoomChanged = IsZoomChanged;
                renderer.m_documentView = this;
                renderer.PageIndex = pageIndex;
                renderer.targetText = targetText;
                renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                renderer.docPage = PdfDocumentPanel;
                renderer.Render(targetedTxtSearchPageIndex);
                if (searchInstancesInPage == 0)
                {
                    searchText = targetText;
                    foundTextInstances = renderer.targetTextInstancesList;
                    if (foundTextInstances != null)
                    {
                        searchInstancesInPage = foundTextInstances.Count;
                        GetTextCoordinates(targetedTxtSearchPageIndex, targetText);
                    }
                    else
                    {
                        RemoveTextSearchHighlightings(targetedTxtSearchPageIndex);
                        targetedTxtSearchPageIndex++;
                        SearchNextPage(targetText);
                    }
                }
            }
            else
            {
                if (currentHighlightedIndex < searchInstancesInPage - 1)
                {
                    GetTextCoordinates(targetedTxtSearchPageIndex, targetText);
                }
                else
                {
                    RemoveTextSearchHighlightings(targetedTxtSearchPageIndex);
                    targetedTxtSearchPageIndex++;
                    SearchNextPage(targetText);
                }
            }
            resultCoordsList = textCoordinatesList;
            searchInstancesInPage = 0;
            currentHighlightedIndex = -1;
            searchText = string.Empty;
            targetedTxtSearchPageIndex = 0;
        }

        private void GetTextCoordinates(int pageToBeRender, string targetText)
        {
            try
            {
                UIElement element = PdfDocumentPanel.Children[pageToBeRender];
                if (element is Canvas)
                {
                    currentHighlightedIndex++;
                    if (searchInstancesInPage > currentHighlightedIndex && currentHighlightedIndex > -1)
                    {
                        foreach (TargetTextProperties foundTextInstance in foundTextInstances)
                        {
                            if (foundTextInstance != null)
                            {
                                double matrixOffsetY = foundTextInstance.matrix.M22 * foundTextInstance.Y;
                                double currentOffsetY = matrixOffsetY + foundTextInstance.matrix.OffsetY;

                                double matrixOffsetX = foundTextInstance.matrix.M11 * foundTextInstance.X;
                                double currentOffsetX = matrixOffsetX + foundTextInstance.matrix.OffsetX;

                                double height = foundTextInstance.height * foundTextInstance.matrix.M22;
                                double width = foundTextInstance.width * foundTextInstance.matrix.M11;

                                PdfTextCoordinates Coords = new PdfTextCoordinates(targetText, pageToBeRender, currentOffsetX, currentOffsetY, width, height);
                                textCoordinatesList.Add(Coords);
                            }
                        }
                    }
                    else
                    {
                        searchInstancesInPage = 0;//repeat search with same word
                        currentHighlightedIndex = -1;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }


        float textSelectionStartIndex;
        float textSelectionEndIndex;
        List<float> highlightedIndex = new List<float>();
        Dictionary<Line, float> textSelectLineSequence = new Dictionary<Line, float>();
        Dictionary<float, Line> textSelectIndexSequence = new Dictionary<float, Line>();
        List<int> textSelectionEnviCreatedList = new List<int>();
        int lineCounter = 1;
        Brush highlightBrush = new SolidColorBrush(global::Windows.UI.Color.FromArgb(77, 0, 0, 0));
        Dictionary<int, TextSelectionPage> TextSelectionPageCollection = new Dictionary<int, TextSelectionPage>();
        TextSelectionPage textSelectionPage = new TextSelectionPage();
        Canvas touchEndCanvas, touchStartCanvas;
        int textStartPageIndex, textEndPageIndex;
        Dictionary<double, Line> textSelectionDifferentLineEnd = new Dictionary<double, Line>();
        double backY;
        Line lastWordInALine;

        private bool CreateTextSelectionEnvironment(int pageNumber)
        {
            if (m_initializedDocumentPages.Count == 0)
                return false;
            //textSelectLineSequence = new Dictionary<Line, int>();
            //textSelectIndexSequence = new Dictionary<int, Line>();
            lineCounter = 1;
            textSelectionDifferentLineEnd = new Dictionary<double, Line>();
            renderer = new WinRTRenderer(zoomFactor, IsZoomChanged, this);
            renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
            renderer.docPage = PdfDocumentPanel;
            List<TextSearchElements> textSearchElements = renderer.GetAllTextElements(pageNumber);
            List<TextSearchElements> tempTextSearchElements = textSearchElements;// renderer.GetAllTextElements(pageNumber);
            Canvas currentPage = PdfDocumentPanel.Children[pageNumber] as Canvas;
            double YAxis = 0;
            var pageResource = m_initializedDocumentPages[pageNumber].Resources;
            foreach (TextSearchElements tempTextSearchElement in textSearchElements)
            {
                TextSearchElements txtSearchElement = new TextSearchElements(tempTextSearchElement.TextElements, tempTextSearchElement.TranformPoints, tempTextSearchElement.CurrentLocation, tempTextSearchElement.PageIndex, tempTextSearchElement.SearchableText, tempTextSearchElement.TextRenderingOperator, tempTextSearchElement.StringWithTextSpace);
                string textContent = txtSearchElement.SearchableText;
                char[] splitter = { ' ' };
                string[] words = textContent.Split(splitter);
                bool IsStandardFont = false;
                float spaceWidth = 0;
                //Calculate space width
                Syncfusion.PdfViewer.Base.TextElement txtSpaceElement = txtSearchElement.TextElements;
                txtSpaceElement.Text = " ";

                if (txtSearchElement.TextRenderingOperator == "Tj")
                {
                    spaceWidth = txtSpaceElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                }
                else if (txtSearchElement.TextRenderingOperator == "TJ")
                {
                    spaceWidth = txtSpaceElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                }
                for (int x = 0; x < words.Length; x++)
                {
                    Syncfusion.PdfViewer.Base.TextElement txtSelectionTextElement = txtSearchElement.TextElements;
                    float X, Y, wordWidth = 0, characterHeight;
                    txtSelectionTextElement.Text = words[x];

                    X = (txtSearchElement.CurrentLocation.X);
                    Y = txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize;
                    if (txtSelectionTextElement.Text.Length > 0)
                    {
                        Syncfusion.PdfViewer.Base.TextElement txtElement = txtSearchElement.TextElements;
                        txtElement.Text = txtSearchElement.SearchableText;
                        string tempText = txtElement.Text;
                        string previousText = string.Empty;
                        string targetText = words[x];
                        int startIndex = txtElement.Text.IndexOf(targetText, StringComparison.OrdinalIgnoreCase);
                        if (startIndex >= 0)
                            previousText = txtElement.Text.Substring(0, startIndex);
                        string matchText = txtElement.Text.Substring(startIndex, targetText.Length);
                        float Width = 0;
                        if (previousText.Length == 0)
                        {
                            txtElement.Text = matchText;
                            X = (txtSearchElement.CurrentLocation.X);
                            Y = txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize;
                            if (txtSearchElement.TextRenderingOperator == "Tj")
                            {
                                Width = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                            }
                            else if (txtSearchElement.TextRenderingOperator == "TJ")
                            {
                                Width = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, true, out IsStandardFont, pageResource);
                            }
                        }
                        else
                        {
                            txtElement.Text = previousText;

                            txtElement.Text = matchText;
                            Width = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                        }
                        txtElement.Text = tempText;
                        if (IsStandardFont)
                        {
                            Y = (txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize) + (txtElement.textLeading / 4);
                        }
                        wordWidth = Width;

                        characterHeight = txtSelectionTextElement.FontSize;
                        global::Windows.UI.Xaml.Media.Matrix transformMatrix = new global::Windows.UI.Xaml.Media.Matrix((float)txtSearchElement.TranformPoints.M11, (float)txtSearchElement.TranformPoints.M12, (float)txtSearchElement.TranformPoints.M21, (float)txtSearchElement.TranformPoints.M22, (float)txtSearchElement.TranformPoints.OffsetX, (float)txtSearchElement.TranformPoints.OffsetY);
                        TargetTextProperties foundTextInstance = new TargetTextProperties(transformMatrix, true, X, Y, wordWidth, characterHeight);
                        double scaleAndYvalue = foundTextInstance.matrix.M22 * foundTextInstance.Y;
                        double currentOffsetY = scaleAndYvalue + foundTextInstance.matrix.OffsetY;
                        YAxis = currentOffsetY;

                        Line line = new Line();
                        line.Name = pageNumber.ToString() + "_" + lineCounter.ToString() + "_" + words[x];
                        MatrixTransform matrixTransform = new MatrixTransform();
                        matrixTransform.Matrix = foundTextInstance.matrix;
                        line.RenderTransform = matrixTransform;
                        line.X1 = foundTextInstance.X;
                        line.X2 = foundTextInstance.X + foundTextInstance.width;
                        if (foundTextInstance.matrix.M11 == 1.33f && foundTextInstance.matrix.M22 == 1.33f)
                        {
                            line.Y1 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                            line.Y2 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                        }
                        else
                        {
                            line.Y1 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                            line.Y2 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                        }
                        line.StrokeThickness = foundTextInstance.height;
                        line.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 255, 255, 255));

                        if (line.Name.Contains("0_1_"))
                        {
                            Line tempLine = new Line();
                            currentPage.Children.Add(tempLine);
                        }

                        if (line.X1 != line.X2)
                        {
                            currentPage.Children.Add(line);

                            string floatString = pageNumber.ToString() + "." + lineCounter.ToString();
                            textSelectIndexSequence.Add(float.Parse(floatString), line);
                            textSelectLineSequence.Add(line, float.Parse(floatString));
                            if (YAxis != backY)
                            {
                                if (YAxis < backY)
                                {
                                    textSelectionDifferentLineEnd.Remove(backY);
                                }
                                backY = YAxis;
                                if (textSelectionDifferentLineEnd.ContainsKey(YAxis))
                                    textSelectionDifferentLineEnd.Remove(YAxis);
                                if (lastWordInALine != null)
                                    textSelectionDifferentLineEnd.Add(YAxis, lastWordInALine);
                                else
                                    textSelectionDifferentLineEnd.Add(YAxis, line);
                            }

                            lastWordInALine = line;

                            lineCounter++;
                            if (lineCounter % 10 == 0)
                                lineCounter++;
                            txtSearchElement.CurrentLocation.X += wordWidth;

                            if (x < words.Length - 1)
                            {
                                Line spaceLine = new Line();
                                spaceLine.Name = pageNumber.ToString() + "_" + lineCounter.ToString() + "_ ";
                                X = (txtSearchElement.CurrentLocation.X);
                                transformMatrix = new global::Windows.UI.Xaml.Media.Matrix((float)txtSearchElement.TranformPoints.M11, (float)txtSearchElement.TranformPoints.M12, (float)txtSearchElement.TranformPoints.M21, (float)txtSearchElement.TranformPoints.M22, (float)txtSearchElement.TranformPoints.OffsetX, (float)txtSearchElement.TranformPoints.OffsetY);
                                foundTextInstance = new TargetTextProperties(transformMatrix, true, X, Y, spaceWidth, characterHeight);

                                scaleAndYvalue = foundTextInstance.matrix.M22 * foundTextInstance.Y;
                                currentOffsetY = scaleAndYvalue + foundTextInstance.matrix.OffsetY;
                                YAxis = currentOffsetY;

                                matrixTransform.Matrix = foundTextInstance.matrix;
                                spaceLine.RenderTransform = matrixTransform;
                                spaceLine.X1 = foundTextInstance.X;
                                spaceLine.X2 = foundTextInstance.X + foundTextInstance.width;
                                if (foundTextInstance.matrix.M11 == 1.33f && foundTextInstance.matrix.M22 == 1.33f)
                                {
                                    spaceLine.Y1 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                                    spaceLine.Y2 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                                }
                                else
                                {
                                    spaceLine.Y1 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                                    spaceLine.Y2 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                                }
                                spaceLine.StrokeThickness = foundTextInstance.height;
                                spaceLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 255, 255, 255));
                                currentPage.Children.Add(spaceLine);
                                txtSearchElement.CurrentLocation.X += spaceWidth;
                                floatString = pageNumber.ToString() + "." + lineCounter.ToString();
                                textSelectIndexSequence.Add(float.Parse(floatString), spaceLine);
                                textSelectLineSequence.Add(spaceLine, float.Parse(floatString));
                                lineCounter++;
                                if (lineCounter % 10 == 0)
                                    lineCounter++;
                            }
                        }
                    }
                    else
                    {
                        txtSearchElement.CurrentLocation.X += spaceWidth;
                    }
                }
            }
            if (textSelectionDifferentLineEnd.ContainsKey(YAxis))
                textSelectionDifferentLineEnd.Remove(YAxis);
            textSelectionDifferentLineEnd.Add(YAxis, lastWordInALine);
            YAxis = backY = 0;

            var items = from pair in textSelectionDifferentLineEnd
                        orderby pair.Key ascending
                        select pair;

            textSelectionDifferentLineEnd = new Dictionary<double, Line>();
            // Display results.
            foreach (KeyValuePair<double, Line> pair in items)
            {
                textSelectionDifferentLineEnd.Add(pair.Key, pair.Value);
            }
            return true;
        }

        private void SearchTextWhileScroll(int pageNo)
        {
            if (pageNo == PageIndex)
            {
                if (!highlightedPagesList.Contains(pageNo))
                {
                    if (searchText != string.Empty)
                    {
                        renderer.zoomFactor = zoomFactor;
                        renderer.IsZoomChanged = IsZoomChanged;
                        renderer.m_documentView = this;
                        renderer.PageIndex = pageNo;
                        renderer.targetText = searchText;
                        renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                        renderer.docPage = PdfDocumentPanel;
                        renderer.Render(pageNo);
                        List<TargetTextProperties> foundTextInstancesScroll = renderer.targetTextInstancesList;
                        if (foundTextInstancesScroll != null)
                        {
                            HighLightAllInstancesFromTimerScroll(foundTextInstancesScroll, pageNo);
                        }
                    }
                }
            }
        }
        internal bool SearchNextText(string targetText)
        {
            if (searchText == targetText && IsSearchRotatePages && targetedTxtSearchPageIndex >= txtSearchInitiationPageIndex)
                return true;

            if (searchText != targetText && targetedTxtSearchPageIndex < m_pageCount)
            {
                foreach (int pgeIndex in highlightedPagesList)
                {
                    RemoveTextSearchHighlightings(pgeIndex);
                }
                highlightedPagesList.Clear();
                targetedTxtSearchPageIndex = PageIndex;
                IsSearchRotatePages = false;
                txtSearchInitiationPageIndex = curPageNo;
                currentHighlightedIndex = -1;
                searchInstancesInPage = 0;
                renderer.zoomFactor = zoomFactor;
                renderer.IsZoomChanged = IsZoomChanged;
                renderer.m_documentView = this;
                renderer.PageIndex = targetedTxtSearchPageIndex;
                renderer.targetText = targetText;
                renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                renderer.docPage = PdfDocumentPanel;
                renderer.Render(targetedTxtSearchPageIndex);
                if (searchInstancesInPage == 0)
                {
                    searchText = targetText;
                    foundTextInstances = renderer.targetTextInstancesList;
                    if (foundTextInstances != null)
                    {
                        searchInstancesInPage = foundTextInstances.Count;
                        HighLightPdfPage(targetedTxtSearchPageIndex, targetText);
                        SearchTextWhileScroll(targetedTxtSearchPageIndex);
                        return true;
                    }
                    else
                    {
                        //Search next page
                        targetedTxtSearchPageIndex++;
                        return SearchNextPage(targetText);
                    }
                }
                SearchTextWhileScroll(targetedTxtSearchPageIndex);
            }
            else
            {
                if (currentHighlightedIndex < searchInstancesInPage - 1)
                {
                    if(foundTextInstances!=null && foundTextInstances.Count==0)
                    {
                        renderer.zoomFactor = zoomFactor;
                        renderer.IsZoomChanged = IsZoomChanged;
                        renderer.m_documentView = this;
                        renderer.PageIndex = targetedTxtSearchPageIndex;
                        renderer.targetText = targetText;
                        renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                        renderer.docPage = PdfDocumentPanel;
                        renderer.Render(targetedTxtSearchPageIndex);
                        foundTextInstances = renderer.targetTextInstancesList;
                    }
                    HighLightPdfPage(targetedTxtSearchPageIndex, targetText);
                    SearchTextWhileScroll(targetedTxtSearchPageIndex);
                    return true;
                }
                else
                {
                    //move to next page
                    targetedTxtSearchPageIndex++;
                    return SearchNextPage(targetText);
                }
            }
            return false;
        }

        private bool SearchNextPage(string targetText)
        {
            if (targetedTxtSearchPageIndex < m_pageCount)
            {
                currentHighlightedIndex = -1;
                searchInstancesInPage = 0;
                renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                renderer.docPage = PdfDocumentPanel;
                renderer.zoomFactor = zoomFactor;
                renderer.IsZoomChanged = IsZoomChanged;
                renderer.m_documentView = this;
                renderer.PageIndex = targetedTxtSearchPageIndex;
                renderer.targetText = targetText;

                renderer.Render(targetedTxtSearchPageIndex);
                if (searchInstancesInPage == 0)
                {
                    searchText = targetText;
                    foundTextInstances = renderer.targetTextInstancesList;
                    if (foundTextInstances != null && foundTextInstances.Count > 0)
                    {
                        searchInstancesInPage = foundTextInstances.Count;
                        this.SetScrollHeight(targetedTxtSearchPageIndex);
                        this.UpdateLayout();
                        HighLightPdfPage(targetedTxtSearchPageIndex, targetText);
                        SearchTextWhileScroll(targetedTxtSearchPageIndex);
                        return true;
                    }
                    else
                    {
                        //search next pages
                        targetedTxtSearchPageIndex++;
                        return SearchNextPage(targetText);
                    }
                }
            }
            else
            {
                //Search completed
                targetedTxtSearchPageIndex = 0;
                return RotateTextSearchPages(targetText);
            }
            return false;
        }


        private bool RotateTextSearchPages(string targetText)
        {
            IsSearchRotatePages = true;
            if (targetedTxtSearchPageIndex < txtSearchInitiationPageIndex)
            {
                currentHighlightedIndex = -1;
                searchInstancesInPage = 0;
                renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                renderer.docPage = PdfDocumentPanel;
                renderer.zoomFactor = zoomFactor;
                renderer.IsZoomChanged = IsZoomChanged;
                renderer.m_documentView = this;
                renderer.PageIndex = targetedTxtSearchPageIndex;
                renderer.targetText = targetText;

                renderer.Render(targetedTxtSearchPageIndex);
                if (searchInstancesInPage == 0)
                {
                    searchText = targetText;
                    foundTextInstances = renderer.targetTextInstancesList;
                    if (foundTextInstances != null && foundTextInstances.Count > 0)
                    {
                        searchInstancesInPage = foundTextInstances.Count;
                        this.SetScrollHeight(targetedTxtSearchPageIndex);
                        this.UpdateLayout();
                        HighLightPdfPage(targetedTxtSearchPageIndex, targetText);
                        return true;
                    }
                    else
                    {
                        //RemoveTextSearchHighlightings(targetedTxtSearchPageIndex);
                        //search next pages
                        targetedTxtSearchPageIndex++;
                        return RotateTextSearchPages(targetText);
                    }
                }
            }
            return false;
        }

        internal void RemoveTextSearchHighlightings(int pageIndex)
        {
            UIElement element = PdfDocumentPanel.Children[pageIndex];
            if (element is Canvas)
            {
                Canvas page = element as Canvas;

                List<UIElement> tempUIElements = new List<UIElement>(page.Children);

                foreach (UIElement child in tempUIElements)
                {
                    if (child.GetType().Name == "Line")
                    {
                        if (child is Line && (child as Line).Name == "")
                            page.Children.Remove(child);
                    }
                }
                tempUIElements.Clear();
            }
        }

        internal void ClearTextSearchHighlightings()
        {
            foreach (int pgeIndex in highlightedPagesList)
            {
                RemoveTextSearchHighlightings(pgeIndex);
            }
            highlightedPagesList.Clear();
            IsSearchRotatePages = false;
            currentHighlightedIndex = -1;
            searchText = string.Empty;
            searchInstancesInPage = 0;
            currentHighlightedIndex = -1;
            searchText = string.Empty;
            targetedTxtSearchPageIndex = 0;
        }

        private void HighLightPdfPage(int pageToBeRender, string targetText)
        {
            try
            {
                UIElement element = PdfDocumentPanel.Children[pageToBeRender];
                if (element is Canvas)
                {
                    Canvas page = element as Canvas;


                    currentHighlightedIndex++;
                    if (searchInstancesInPage > currentHighlightedIndex && currentHighlightedIndex > -1)
                    {
                        TargetTextProperties foundTextInstance = foundTextInstances[currentHighlightedIndex];
                        if (foundTextInstance != null)
                        {
                            double scaleAndYvalue = foundTextInstance.matrix.M22 * foundTextInstance.Y;
                            double currentOffsetY = scaleAndYvalue + foundTextInstance.matrix.OffsetY;
                            double YAxis = currentOffsetY;
                            if (this.documentScrollViewer.ViewportHeight < YAxis)
                            {
                                SetTextSearchScrollHeight(YAxis);
                            }
                            else
                            {
                                if ((m_pageLocation[pageToBeRender + 1] - page.Height) + YAxis < this.documentScrollViewer.VerticalOffset)
                                {
                                    SetTextSearchScrollHeight(YAxis);
                                }
                            }
                        }
                    }
                    else
                    {
                        searchInstancesInPage = 0;//repeat search with same word
                        currentHighlightedIndex = -1;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }

        private void HighLightPdfPageFromTimer(int pageToBeRender, string targetText)
        {
            try
            {
                UIElement element = PdfDocumentPanel.Children[pageToBeRender];
                if (element is Canvas)
                {
                    Canvas page = element as Canvas;

                    if (searchInstancesInPage > currentHighlightedIndex && currentHighlightedIndex > -1)
                    {
                        if (foundTextInstances.Count > 0)
                        {
                            TargetTextProperties foundTextInstance = foundTextInstances[currentHighlightedIndex];
                            //HighLightAllInstancesFromTimer(foundTextInstances, page);
                            if (foundTextInstance != null && (previousFoundTextInstance == null || previousFoundTextInstance != foundTextInstance))
                            {
                                PreviousTextHighLightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(100, 255, 229, 0));
                                Line line = new Line();
                                MatrixTransform matrixTransform = new MatrixTransform();
                                matrixTransform.Matrix = foundTextInstance.matrix;
                                line.RenderTransform = matrixTransform;
                                line.X1 = foundTextInstance.X;
                                line.X2 = foundTextInstance.X + foundTextInstance.width;
                                double strokeHeight = foundTextInstance.height;
                                if (foundTextInstance.matrix.M11 == 1.33f && foundTextInstance.matrix.M22 == 1.33f)
                                {
                                    line.Y1 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                                    line.Y2 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                                    strokeHeight = foundTextInstance.height * foundTextInstance.matrix.M22;
                                }
                                else
                                {
                                    line.Y1 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                                    line.Y2 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                                }
                                line.StrokeThickness = strokeHeight;
                                line.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(150, 255, 50, 0));
                                page.Children.Add(line);
                                PreviousTextHighLightLine = line;
                                previousFoundTextInstance = foundTextInstance;
                            }
                        }
                    }
                    else
                    {
                        searchInstancesInPage = 0;//repeat search with same word
                        currentHighlightedIndex = -1;
                    }
                    prevPageTextSearch = pageToBeRender;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }


        private void HighLightAllInstancesFromTimerScroll(List<TargetTextProperties> foundTextInstances, int pageIndx)
        {
            UIElement element = PdfDocumentPanel.Children[pageIndx];
            if (element is Canvas)
            {
                Canvas page = element as Canvas;
                if (!highlightedPagesList.Contains(pageIndx))
                {
                    foreach (TargetTextProperties foundTextInstance in foundTextInstances)
                    {
                        Line line = new Line();
                        //line.Name = "TextSearch";
                        MatrixTransform matrixTransform = new MatrixTransform();
                        matrixTransform.Matrix = foundTextInstance.matrix;
                        line.RenderTransform = matrixTransform;
                        line.X1 = foundTextInstance.X;
                        line.X2 = foundTextInstance.X + foundTextInstance.width;

                        if (foundTextInstance.matrix.M11 == 1.33f && foundTextInstance.matrix.M22 == 1.33f)
                        {
                            line.Y1 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                            line.Y2 = ((foundTextInstance.Y + (foundTextInstance.height / 2)) - 0.33f);
                        }
                        else
                        {
                            line.Y1 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                            line.Y2 = ((foundTextInstance.Y + (foundTextInstance.height)) - 0.33f);
                        }
                        line.StrokeThickness = foundTextInstance.height;
                        line.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(100, 255, 229, 0));
                        page.Children.Add(line);
                        if (!highlightedPagesList.Contains(pageIndx))
                            highlightedPagesList.Add(pageIndx);
                    }
                }
            }
        }
        #endregion

        #region PreviousSearch
        internal void SearchPrevText(string targetText)
        {
            if (currentHighlightedIndex != -1)
            {
                currentHighlightedIndex--;
            }
            if (currentHighlightedIndex == -1)
            {
                targetedTxtSearchPageIndex--;
                PreviousPageSearch(searchText);
            }
            HighLightPageForPreviosInstance(targetedTxtSearchPageIndex, searchText);
            SearchTextWhileScroll(targetedTxtSearchPageIndex);
        }

        private void HighLightPageForPreviosInstance(int pageToBeRender, string searchText)
        {
            try
            {
                UIElement element = PdfDocumentPanel.Children[pageToBeRender];
                if (element is Canvas)
                {
                    Canvas page = element as Canvas;

                    //foreach (UIElement child in page.Children)
                    //{
                    //    if (child.GetType().Name == "Line")
                    //    {
                    //        if (child is Line && (child as Line).Name == "")
                    //            page.Children.Remove(child);
                    //    }
                    //}
                    if (searchInstancesInPage > currentHighlightedIndex && currentHighlightedIndex > -1)
                    {
                        TargetTextProperties foundTextInstance = foundTextInstances[currentHighlightedIndex];
                        if (foundTextInstance != null)
                        {
                            double scaleAndYvalue = foundTextInstance.matrix.M22 * foundTextInstance.Y;
                            double currentOffsetY = scaleAndYvalue + foundTextInstance.matrix.OffsetY;
                            double YAxis = currentOffsetY;
                            if (this.documentScrollViewer.ViewportHeight < YAxis)
                            {
                                SetTextSearchScrollHeight(YAxis);
                            }
                            else
                            {
                                if ((m_pageLocation[pageToBeRender + 1] - page.Height) + YAxis < this.documentScrollViewer.VerticalOffset)
                                {
                                    SetTextSearchScrollHeight(YAxis);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }
        private void PreviousPageSearch(string targetText)
        {
            if (targetedTxtSearchPageIndex >= 0)
            {
                currentHighlightedIndex = -1;
                searchInstancesInPage = 0;
                renderer.m_lDoc = m_pdfViewer.textSearchLoadedDocument;
                renderer.docPage = PdfDocumentPanel;
                renderer.zoomFactor = zoomFactor;
                renderer.IsZoomChanged = IsZoomChanged;
                renderer.m_documentView = this;
                renderer.PageIndex = targetedTxtSearchPageIndex;
                renderer.targetText = targetText;
                renderer.Render(targetedTxtSearchPageIndex);
                if (searchInstancesInPage == 0)
                {
                    searchText = targetText;
                    foundTextInstances = renderer.targetTextInstancesList;
                    if (foundTextInstances != null && foundTextInstances.Count > 0)
                    {
                        searchInstancesInPage = foundTextInstances.Count;
                        currentHighlightedIndex = searchInstancesInPage;
                        this.SetScrollHeight(targetedTxtSearchPageIndex);
                        this.UpdateLayout();
                        HighLightPreviousPDFPage(targetedTxtSearchPageIndex, targetText);
                    }
                    else
                    {
                        //search next pages
                        targetedTxtSearchPageIndex--;
                        PreviousPageSearch(targetText);
                    }
                }
            }
            else
            {
                //Search completed
                targetedTxtSearchPageIndex = 0;
            }
            SearchTextWhileScroll(targetedTxtSearchPageIndex);
        }
        private void HighLightPreviousPDFPage(int pageToBeRender, string targetText)
        {
            try
            {
                UIElement element = PdfDocumentPanel.Children[pageToBeRender];
                if (element is Canvas)
                {
                    Canvas page = element as Canvas;

                    //foreach (UIElement child in page.Children)
                    //{
                    //    if (child.GetType().Name == "Line")
                    //    {
                    //        if (child is Line && (child as Line).Name == "")
                    //            page.Children.Remove(child);
                    //    }
                    //}

                    currentHighlightedIndex--;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }
        #endregion
        #endregion


        #region TextSelection

        bool TextSelctionMode = false;
        void documentview_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (!IsTextSelectionEnabled)
                    return;
                if (e.OriginalSource is Line)
                {
                    if ((e.OriginalSource as Line).Parent is Canvas)
                    {
                        curPageNo = int.Parse(((e.OriginalSource as Line).Parent as Canvas).Name);
                    }
                }
                else if (e.OriginalSource is Image)
                {
                    curPageNo = int.Parse(((e.OriginalSource as Image).Parent as Canvas).Name);
                }

                if (!textSelectionEnviCreatedList.Contains(curPageNo))
                {
                    bool IsEnviCreated = false;
                    try
                    {
                        IsEnviCreated = CreateTextSelectionEnvironment(curPageNo);
                    }
                    catch
                    { }
                    if (IsEnviCreated)
                    {
                        textSelectionEnviCreatedList.Add(curPageNo);
                        textSelectionPage = new TextSelectionPage();
                        textSelectionPage.WordCount = lineCounter - 1;
                        textSelectionPage.PageNumber = curPageNo;
                        Object keys = textSelectionDifferentLineEnd.Keys;
                        List<double> keyList = keys as List<double>;

                        //List<KeyValuePair<double,Line> myList=textSelectionDifferentLineEnd.tolic
                        textSelectionPage.TextSelectionDifferentLines = new Dictionary<double, Line>(textSelectionDifferentLineEnd);
                        TextSelectionPageCollection.Add(curPageNo, textSelectionPage);
                    }
                }
                if (e.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Mouse)
                {
                    if (e.OriginalSource is Line)
                    {
                        Line highlightLine = e.OriginalSource as Line;
                        highlightLine.Stroke = highlightBrush;

                        if (!highlightedIndex.Contains(textSelectLineSequence[highlightLine]))
                            highlightedIndex.Add(textSelectLineSequence[highlightLine]);
                    }
                }
            }
            catch
            { }
        }
        bool IsTextSelectionRemoved = false;
        private void RemoveAllSelection()
        {
            IsTextSelectionRemoved = true;
            UIElement element = PdfDocumentPanel.Children[PageIndex];//current page in viewport
            Canvas page = element as Canvas;
            IsSelectionRemoved = true;
            //page.Children.Remove(touchStart);
            //page.Children.Remove(touchEnd);
            foreach (float i in highlightedIndex)
            {
                Line highlightedLine = textSelectIndexSequence[i];
                highlightedLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));
            }
            highlightedIndex.Clear();
            if (page.Children.Contains(btnCopy))
                page.Children.Remove(btnCopy);
            startLineIndexBack = -1;
            startPageIndexBack = -1;
            endLineIndexBack = -1;
            endPageIndexBack = -1;
        }

        private void documentview_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (!IsTextSelectionEnabled)
                    return;
                if (e.OriginalSource is Line)
                {
                    if ((e.OriginalSource as Line).Parent is Canvas)
                    {
                        curPageNo = int.Parse(((e.OriginalSource as Line).Parent as Canvas).Name);
                    }
                }
                else if (e.OriginalSource is Image)
                {
                    curPageNo = int.Parse(((e.OriginalSource as Image).Parent as Canvas).Name);
                }

                if (!textSelectionEnviCreatedList.Contains(curPageNo))
                {
                    bool IsEnviCreated = false;
                    try
                    {
                        IsEnviCreated = CreateTextSelectionEnvironment(curPageNo);
                    }
                    catch
                    { }
                    if (IsEnviCreated)
                    {
                        textSelectionEnviCreatedList.Add(curPageNo);
                        textSelectionPage = new TextSelectionPage();
                        textSelectionPage.WordCount = lineCounter - 1;
                        textSelectionPage.PageNumber = curPageNo;
                        Object keys = textSelectionDifferentLineEnd.Keys;
                        List<double> keyList = keys as List<double>;

                        //List<KeyValuePair<double,Line> myList=textSelectionDifferentLineEnd.tolic
                        textSelectionPage.TextSelectionDifferentLines = new Dictionary<double, Line>(textSelectionDifferentLineEnd);
                        TextSelectionPageCollection.Add(curPageNo, textSelectionPage);
                    }
                }
                UIElement element = PdfDocumentPanel.Children[curPageNo];//current page in viewport
                Canvas page = element as Canvas;
                pointerReleasedLocation = e.GetCurrentPoint(page).Position;
                if (btnCopy != null)
                    btnCopy.Visibility = Visibility.Visible;
                if (IsTouchEndRemoved)
                {
                    Line lastLine = textSelectIndexSequence[textSelectionEndIndex];
                    if (lastLine.Parent is Canvas)
                    {
                        Canvas destCanvas = lastLine.Parent as Canvas;
                        global::Windows.UI.Xaml.Media.Matrix transf = (lastLine.RenderTransform as MatrixTransform).Matrix;
                        double scaleAndYvalue, currentOffsetY, YAxis, scaleAndXvalue, currentOffsetX, Xaxis;
                        if (textSelectionStartIndex > textSelectionEndIndex)
                        {
                            scaleAndYvalue = transf.M22 * lastLine.Y1;
                            currentOffsetY = scaleAndYvalue + transf.OffsetY;
                            YAxis = currentOffsetY;

                            scaleAndXvalue = transf.M11 * lastLine.X1;
                            currentOffsetX = scaleAndXvalue + transf.OffsetX;
                            Xaxis = currentOffsetX;
                        }
                        else
                        {
                            scaleAndYvalue = transf.M22 * lastLine.Y2;
                            currentOffsetY = scaleAndYvalue + transf.OffsetY;
                            YAxis = currentOffsetY;

                            scaleAndXvalue = transf.M11 * lastLine.X2;
                            currentOffsetX = scaleAndXvalue + transf.OffsetX;
                            Xaxis = currentOffsetX;
                        }
                        Canvas.SetZIndex(touchEnd, 2);
                        Canvas.SetLeft(touchEnd, Xaxis - (touchEnd.Width / 2));
                        Canvas.SetTop(touchEnd, YAxis + 10);

                        Canvas.SetZIndex(btnCopy, 3);

                        Canvas.SetLeft(btnCopy, Xaxis + 50);

                        if ((Xaxis + 50) >= page.Width)
                            Canvas.SetLeft(btnCopy, page.Width - btnCopy.Width);

                        Canvas.SetTop(btnCopy, YAxis);

                        Canvas previousCanvas = PdfDocumentPanel.Children[textEndPageIndex] as Canvas;

                        if (touchEndCanvas.Children.Contains(touchEnd))
                            touchEndCanvas.Children.Remove(touchEnd);

                        if (touchEndCanvas.Children.Contains(btnCopy))
                            touchEndCanvas.Children.Remove(btnCopy);

                        previousCanvas.Children.Remove(touchEnd);
                        previousCanvas.Children.Remove(btnCopy);

                        if (!destCanvas.Children.Contains(touchEnd))
                        {
                            touchEndCanvas = destCanvas;
                            destCanvas.Children.Add(touchEnd);
                        }
                        if (!destCanvas.Children.Contains(btnCopy))
                        {
                            touchEndCanvas = destCanvas;
                            if (btnCopy.Parent != null)
                            {
                                Canvas previousBtnCopyCanvas = btnCopy.Parent as Canvas;
                                if (previousBtnCopyCanvas.Children.Contains(btnCopy))
                                    previousBtnCopyCanvas.Children.Remove(btnCopy);
                            }
                            destCanvas.Children.Add(btnCopy);
                        }
                        IsTouchEndRemoved = false;
                    }
                }

                #region Touch
                if (e.Pointer.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    if (e.OriginalSource is Line && !IsEllipsePressed)
                    {
                        if ((e.OriginalSource as Line).Name == "")
                        {
                            return;
                        }
                        if (textSelectionTouchLine == (e.OriginalSource as Line))
                        {
                            if (element is Canvas)
                            {
                                if (!TextSelctionMode)
                                {
                                    TextSelctionMode = true;
                                    page.ManipulationMode = ManipulationModes.All;
                                }
                            }
                            if (documentScrollViewer.Visibility == global::Windows.UI.Xaml.Visibility.Visible)
                            {
                                if (e.OriginalSource is Line && !highlightedIndex.Contains(textSelectLineSequence[e.OriginalSource as Line]))
                                {
                                    Line highlightLine = e.OriginalSource as Line;
                                    //highlightLine.Stroke = highlightBrush;
                                    //if (!highlightedIndex.Contains(textSelectLineSequence[highlightLine]))
                                    //    highlightedIndex.Add(textSelectLineSequence[highlightLine]);
                                    textSelectionStartIndex = textSelectLineSequence[highlightLine];

                                    page = element as Canvas;
                                    if (!page.Children.Contains(touchStart))
                                    {
                                        touchStart = new Ellipse();
                                        touchStart.Name = "touchStart";
                                        touchStart.Height = 28;
                                        touchStart.Width = 28;
                                        touchStart.Visibility = Visibility.Visible;
                                        touchStart.Stroke = new SolidColorBrush(global::Windows.UI.Colors.Black);
                                        touchStart.StrokeThickness = 1.5;
                                        touchStart.Fill = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));
                                        global::Windows.UI.Xaml.Media.Matrix transf = (highlightLine.RenderTransform as MatrixTransform).Matrix;

                                        double scaleAndYvalue = transf.M22 * highlightLine.Y1;
                                        double currentOffsetY = scaleAndYvalue + transf.OffsetY;
                                        double YAxis = currentOffsetY;

                                        double scaleAndXvalue = transf.M11 * highlightLine.X1;
                                        double currentOffsetX = scaleAndXvalue + transf.OffsetX;
                                        double Xaxis = currentOffsetX;

                                        Canvas.SetZIndex(touchStart, 1);
                                        Canvas.SetLeft(touchStart, Xaxis - (touchStart.Width / 2));
                                        textSelectionEndIndex = textSelectLineSequence[highlightLine];

                                        if (element is Canvas)
                                            Canvas.SetTop(touchStart, YAxis + 10);

                                        touchStartCanvas = page;
                                        page.Children.Add(touchStart);

                                        touchEnd = new Ellipse();
                                        touchEnd.Name = "touchEnd";
                                        touchEnd.Height = 28;
                                        touchEnd.Width = 28;
                                        touchEnd.Visibility = Visibility.Visible;
                                        touchEnd.Stroke = new SolidColorBrush(global::Windows.UI.Colors.Black);
                                        touchEnd.StrokeThickness = 1.5;
                                        touchEnd.Fill = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));

                                        scaleAndYvalue = transf.M22 * highlightLine.Y2;
                                        currentOffsetY = scaleAndYvalue + transf.OffsetY;
                                        YAxis = currentOffsetY;

                                        scaleAndXvalue = transf.M11 * highlightLine.X2;
                                        currentOffsetX = scaleAndXvalue + transf.OffsetX;
                                        Xaxis = currentOffsetX;
                                        Canvas.SetZIndex(touchEnd, 2);
                                        Canvas.SetLeft(touchEnd, Xaxis - (touchEnd.Width / 2));
                                        Canvas.SetTop(touchEnd, YAxis + 10);
                                        touchEndCanvas = page;
                                        page.Children.Add(touchEnd);
                                        textStartPageIndex = PageIndex;
                                        textEndPageIndex = PageIndex;

                                        if (!page.Children.Contains(btnCopy))
                                        {
                                            btnCopy = new TextSelectionCopyButton();
                                            btnCopy.Tapped += btnCopy_Tapped;
                                            btnCopy.Height = 50;
                                            btnCopy.Width = 100;
                                            if (btnCopy.Parent != null)
                                            {
                                                Canvas previousBtnCopyCanvas = btnCopy.Parent as Canvas;
                                                if (previousBtnCopyCanvas.Children.Contains(btnCopy))
                                                    previousBtnCopyCanvas.Children.Remove(btnCopy);
                                            }
                                            page.Children.Add(btnCopy);
                                        }

                                        Canvas.SetZIndex(btnCopy, 3);
                                        Canvas.SetLeft(btnCopy, Xaxis + 50);
                                        Canvas.SetTop(btnCopy, YAxis);
                                    }

                                    e.Handled = true;
                                    try
                                    {
                                        HighlightIntermediate();
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                        else
                        {
                            if (highlightedIndex.Contains(textSelectLineSequence[e.OriginalSource as Line]))
                            {
                                PointerPoint pointerPoint = e.GetCurrentPoint(page);
                                Canvas.SetLeft(btnCopy, pointerPoint.Position.X);
                                Canvas.SetTop(btnCopy, pointerPoint.Position.Y);
                            }
                            else
                            {
                                TextSelctionMode = false;
                                page.ManipulationMode = ManipulationModes.System;
                                if (touchStart != null)
                                    touchStartCanvas.Children.Remove(touchStart);
                                if (touchEnd != null)
                                    touchEndCanvas.Children.Remove(touchEnd);
                                if (touchEnd != null && touchEndCanvas.Children.Contains(btnCopy))
                                    touchEndCanvas.Children.Remove(btnCopy);
                                RemoveTextSelctionControls(page);
                                RemoveAllSelection();
                            }

                        }
                    }
                    else
                    {
                        if (e.OriginalSource is Image && !IsEllipsePressed)
                        {
                            TextSelctionMode = false;
                            page.ManipulationMode = ManipulationModes.System;
                            if (touchStart != null)
                                touchStartCanvas.Children.Remove(touchStart);
                            if (touchEnd != null)
                                touchEndCanvas.Children.Remove(touchEnd);
                            if (touchEnd != null && touchEndCanvas.Children.Contains(btnCopy))
                                touchEndCanvas.Children.Remove(btnCopy);
                            RemoveTextSelctionControls(page);
                            RemoveAllSelection();
                        }
                        page.ManipulationMode = ManipulationModes.System;
                    }
                    if (e.OriginalSource is Image && IsEllipsePressed)
                    {
                        //element = PdfDocumentPanel.Children[curPageNo];
                        //page = element as Canvas;
                        //PointerPoint pointerPoint = e.GetCurrentPoint(page);
                        //Image destImage = e.OriginalSource as Image;
                        //double pagePointY = pointerPoint.Position.Y;

                        //TextSelectionPage currentPage = TextSelectionPageCollection[curPageNo];
                        //Dictionary<double, Line> currentPageLineHight = currentPage.TextSelectionDifferentLines;
                        //Line destLine = null;
                        //int i = 0;
                        //foreach (KeyValuePair<double, Line> element1 in currentPageLineHight)
                        //{
                        //    i += 1;
                        //    if (pagePointY > element1.Key)
                        //        destLine = element1.Value;
                        //    else
                        //        break;
                        //}
                        //if (destLine != null)
                        //{
                        //    textSelectionEndIndex = textSelectLineSequence[destLine];
                        //    HighlightIntermediate();
                        //}
                    }
                }
                #endregion
                else if (e.Pointer.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Mouse)
                {
                    IsMousePressed = false;
                    if (highlightedIndex.Count > 0)
                    {
                        if (!page.Children.Contains(btnCopy))
                        {
                            btnCopy = new TextSelectionCopyButton();
                            btnCopy.Height = 50;
                            btnCopy.Width = 100;
                            btnCopy.SelectionCopyButton.Click += btnCopy_Click;
                            if (btnCopy.Parent != null)
                            {
                                Canvas previousBtnCopyCanvas = btnCopy.Parent as Canvas;
                                if (previousBtnCopyCanvas.Children.Contains(btnCopy))
                                    previousBtnCopyCanvas.Children.Remove(btnCopy);
                            }
                            page.Children.Add(btnCopy);
                        }
                        PointerPoint pointerPoint = e.GetCurrentPoint(page);
                        Canvas.SetZIndex(btnCopy, 3);
                        Canvas.SetLeft(btnCopy, pointerPoint.Position.X + 50);
                        if (pointerPoint.Position.X + 50 > page.Width)
                        {
                            Canvas.SetLeft(btnCopy, page.Width - btnCopy.Width);
                        }
                        Canvas.SetTop(btnCopy, pointerPoint.Position.Y);
                    }
                }
            }
            catch
            {

            }
        }


        void page_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (IsEllipsePressed)
                {
                    UIElement element = PdfDocumentPanel.Children[PageIndex];//current page in viewport
                    if (element is Canvas)
                    {
                        Canvas page = element as Canvas;
                        Point loc = ReattachTouchEnd();
                        IsTouchEndRemoved = false;
                        if (loc != new Point(0, 0))
                        {
                            if (btnCopy.Parent != null)
                            {
                                Canvas btnCopyParentCanvas = btnCopy.Parent as Canvas;
                                btnCopyParentCanvas.Children.Remove(btnCopy);
                            }
                            btnCopy = new TextSelectionCopyButton();
                            btnCopy.Height = 50;
                            btnCopy.Width = 100;
                            btnCopy.SelectionCopyButton.Click += btnCopy_Click;
                            if (btnCopy.Parent != null)
                            {
                                Canvas previousBtnCopyCanvas = btnCopy.Parent as Canvas;
                                if (previousBtnCopyCanvas.Children.Contains(btnCopy))
                                    previousBtnCopyCanvas.Children.Remove(btnCopy);
                            }
                            page.Children.Add(btnCopy);
                            Canvas.SetLeft(btnCopy, loc.X + 50);
                            if (loc.X + 50 > page.Width)
                            {
                                Canvas.SetLeft(btnCopy, page.Width - btnCopy.Width);
                            }
                            Canvas.SetTop(btnCopy, loc.Y);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        private Point ReattachTouchEnd()
        {
            Point currentHighLightEndLocation = new Point();
            UIElement element = PdfDocumentPanel.Children[PageIndex];
            Canvas page = element as Canvas;

            if (IsTouchEndRemoved)
            {
                Line lastLine = textSelectIndexSequence[textSelectionEndIndex];
                if (lastLine.Parent is Canvas)
                {
                    Canvas destCanvas = lastLine.Parent as Canvas;
                    global::Windows.UI.Xaml.Media.Matrix transf = (lastLine.RenderTransform as MatrixTransform).Matrix;
                    double scaleAndYvalue, currentOffsetY, YAxis, scaleAndXvalue, currentOffsetX, Xaxis;
                    if (textSelectionStartIndex > textSelectionEndIndex)
                    {
                        scaleAndYvalue = transf.M22 * lastLine.Y1;
                        currentOffsetY = scaleAndYvalue + transf.OffsetY;
                        YAxis = currentOffsetY;

                        scaleAndXvalue = transf.M11 * lastLine.X1;
                        currentOffsetX = scaleAndXvalue + transf.OffsetX;
                        Xaxis = currentOffsetX;
                    }
                    else
                    {
                        scaleAndYvalue = transf.M22 * lastLine.Y2;
                        currentOffsetY = scaleAndYvalue + transf.OffsetY;
                        YAxis = currentOffsetY;

                        scaleAndXvalue = transf.M11 * lastLine.X2;
                        currentOffsetX = scaleAndXvalue + transf.OffsetX;
                        Xaxis = currentOffsetX;
                    }
                    Canvas.SetZIndex(touchEnd, 2);
                    Canvas.SetLeft(touchEnd, Xaxis - (touchEnd.Width / 2));
                    Canvas.SetTop(touchEnd, YAxis + 10);
                    currentHighLightEndLocation = new Point(Xaxis, YAxis);
                    Canvas previousCanvas = PdfDocumentPanel.Children[textEndPageIndex] as Canvas;

                    if (touchEndCanvas.Children.Contains(touchEnd))
                        touchEndCanvas.Children.Remove(touchEnd);

                    previousCanvas.Children.Remove(touchEnd);

                    if (!destCanvas.Children.Contains(touchEnd))
                    {
                        touchEndCanvas = destCanvas;
                        destCanvas.Children.Add(touchEnd);
                    }
                    IsTouchEndRemoved = false;
                }
            }
            return currentHighLightEndLocation;
        }

        string selectedText = string.Empty;
        void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            copyHighlightedText();
        }

        void btnCopy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            copyHighlightedText();
        }

        private void copyHighlightedText()
        {
            Char[] splitter = { '.' };
            selectedText = string.Empty;
            ReattachTouchEnd();
            highlightedIndex.Sort();
            List<int> highLightedIndexInt = new List<int>();

            int textSelectionStartPage = int.Parse((textSelectIndexSequence[textSelectionStartIndex]).Name.Split('_')[0]);
            int textSelectionEndPage = int.Parse((textSelectIndexSequence[textSelectionEndIndex]).Name.Split('_')[0]);

            List<float> copyOfhighlightedIndex = new List<float>(highlightedIndex);
            List<float> pageWiseHighlight = new List<float>();

            int copyStartPage, copyEndPage;

            if (textSelectionStartPage < textSelectionEndPage)
            {
                copyStartPage = textSelectionStartPage;
                copyEndPage = textSelectionEndPage;
            }
            else
            {
                copyStartPage = textSelectionEndPage;
                copyEndPage = textSelectionStartPage;
            }

            copyOfhighlightedIndex.Sort();

            for (int i = copyStartPage; i <= copyEndPage; i++)
            {
                pageWiseHighlight = new List<float>();
                foreach (float j in copyOfhighlightedIndex)
                {
                    string index = j.ToString();

                    int pageIdx = int.Parse(index.Split(splitter)[0]);
                    if (pageIdx == i)
                    {
                        pageWiseHighlight.Add(j);
                    }
                }

                int count = pageWiseHighlight.Count;

                copyOfhighlightedIndex.RemoveRange(0, count);

                highLightedIndexInt = new List<int>();
                pageWiseHighlight.Sort();
                foreach (float k in pageWiseHighlight)
                {
                    string idx = k.ToString();

                    int wordIdx = int.Parse(idx.Split(splitter)[1]);
                    highLightedIndexInt.Add(wordIdx);
                }
                highLightedIndexInt.Sort();

                foreach (int x in highLightedIndexInt)
                {
                    Line highlightedLine = textSelectIndexSequence[float.Parse(i.ToString() + "." + x.ToString())];
                    string text = highlightedLine.Name.Split('_')[2];
                    selectedText += text;
                }
            }
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(selectedText);
            try
            {
                global::Windows.ApplicationModel.DataTransfer.Clipboard.Clear();
                global::Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
            catch (Exception)
            {
            }
        }
        bool IsStartPageLessThanEndPage = true;
        bool IsStartPageLessThanEndPageBack = false;

        int startPageIndexBack = -1, startLineIndexBack = -1;
        int endPageIndexBack = -1, endLineIndexBack = -1;

        bool IsStartLineLessThanEndLine = true;
        bool IsStartLineLessThanEndLineBack = true;

        private void HighlightIntermediate()
        {
            List<float> highLightToBeRemoved = new List<float>();
            List<float> backHighlightIndex = new List<float>(highlightedIndex);
            //highlightedIndex.Clear();
            string startIndex = textSelectionStartIndex.ToString();
            string endIndex = textSelectionEndIndex.ToString();
            char[] splitter = { '.' };

            int startPageIndex = int.Parse(startIndex.Split(splitter)[0]);
            int endPageIndex = int.Parse(endIndex.Split(splitter)[0]);

            int startLineIndex = int.Parse(startIndex.Split(splitter)[1]);
            int endLineIndex = int.Parse(endIndex.Split(splitter)[1]);
            #region StartPageIndexEqualsEndPageIndex
            if (startPageIndex == endPageIndex)
            {
                if (endPageIndex != endPageIndexBack && endPageIndexBack != -1)
                {
                    int startLine, endLine;
                    if (startPageIndex < endPageIndexBack)
                    {
                        //int startLine, endLine;
                        if (endPageIndex > endPageIndexBack)
                        {
                            startLine = endLineIndexBack;
                            endLine = TextSelectionPageCollection[endPageIndexBack].WordCount;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                            startLine = 1;
                            endLine = endLineIndex;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                        }
                        else if (endPageIndex < endPageIndexBack)
                        {
                            startLine = 1;
                            endLine = endLineIndexBack;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0)); ;

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                            startLine = endLineIndex;
                            endLine = TextSelectionPageCollection[endPageIndex].WordCount;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0)); ;

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                        }
                    }
                    else
                    {
                        if (endPageIndex > endPageIndexBack)
                        {
                            startLine = endLineIndexBack;
                            endLine = TextSelectionPageCollection[endPageIndexBack].WordCount;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                            startLine = 1;
                            endLine = endLineIndex;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                        }
                        else if (endPageIndex < endPageIndexBack)
                        {
                            startLine = 1;
                            endLine = endLineIndexBack;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                            startLine = endLineIndex;
                            endLine = TextSelectionPageCollection[endPageIndex].WordCount;
                            for (float i = startLine; i <= endLine; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                        }
                    }
                }
                else if (startLineIndex < endLineIndex)
                {
                    if (!IsStartLineLessThanEndLine)
                    {
                        float highlightIndex;
                        Line highlightLine;
                        foreach (float element in highlightedIndex)
                        {
                            string startPage = (element.ToString().Split(splitter)[0]);
                            string startLine = (element.ToString().Split(splitter)[1]);

                            highlightIndex = float.Parse(startPage + "." + startLine);
                            highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));
                        }
                        highlightedIndex.Clear();
                        highlightIndex = float.Parse(startPageIndex.ToString() + "." + startLineIndex.ToString());
                        highlightLine = textSelectIndexSequence[highlightIndex];
                        highlightLine.Stroke = highlightBrush;
                        if (!highlightedIndex.Contains(highlightIndex))
                            highlightedIndex.Add(highlightIndex);
                        for (float i = startLineIndex; i <= endLineIndex; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            highlightIndex = float.Parse(startPageIndex.ToString() + "." + i.ToString());
                            highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = highlightBrush;

                            if (!highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Add(highlightIndex);
                        }
                    }
                    else if (endLineIndex > endLineIndexBack)
                    {
                        int difference = endLineIndex - endLineIndexBack;
                        for (float i = endLineIndexBack; i <= endLineIndex; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            if (i >= 0)
                            {
                                float highlightIndex = float.Parse(startPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                        }
                    }
                    else if (endLineIndex < endLineIndexBack)
                    {
                        int difference = endLineIndexBack - endLineIndex;
                        for (float i = endLineIndexBack; i >= endLineIndex; i--)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(startPageIndex.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0)); ;

                            if (highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Remove(highlightIndex);
                        }
                    }
                    IsStartLineLessThanEndLine = true;
                }
                else if (startLineIndex > endLineIndex)
                {
                    if (IsStartLineLessThanEndLine)
                    {
                        float highlightIndex;
                        Line highlightLine;
                        foreach (float element in highlightedIndex)
                        {
                            string startPage = (element.ToString().Split(splitter)[0]);
                            string startLine = (element.ToString().Split(splitter)[1]);

                            highlightIndex = float.Parse(startPage + "." + startLine);
                            highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                        }
                        highlightedIndex.Clear();
                        highlightIndex = float.Parse(startPageIndex.ToString() + "." + startLineIndex.ToString());
                        highlightLine = textSelectIndexSequence[highlightIndex];
                        highlightLine.Stroke = highlightBrush;
                        if (!highlightedIndex.Contains(highlightIndex))
                            highlightedIndex.Add(highlightIndex);
                        for (float i = endLineIndex; i <= startLineIndex; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            highlightIndex = float.Parse(startPageIndex.ToString() + "." + i.ToString());
                            highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = highlightBrush;

                            if (!highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Add(highlightIndex);
                        }
                    }
                    else if (endLineIndex > endLineIndexBack)
                    {
                        int difference = endLineIndex - endLineIndexBack;
                        for (float i = endLineIndexBack; i <= endLineIndex; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            if (i >= 0)
                            {
                                float highlightIndex = float.Parse(startPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                        }
                    }
                    else if (endLineIndex < endLineIndexBack)
                    {
                        int difference = endLineIndexBack - endLineIndex;
                        for (float i = endLineIndexBack; i >= endLineIndex; i--)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            if (i >= 0)
                            {
                                float highlightIndex = float.Parse(startPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                        }
                    }
                    IsStartLineLessThanEndLine = false;
                }
                else
                {
                    float highlightIndex = float.Parse(startPageIndex.ToString() + "." + startLineIndex.ToString());
                    Line highlightLine = textSelectIndexSequence[highlightIndex];
                    highlightLine.Stroke = highlightBrush;
                    if (!highlightedIndex.Contains(highlightIndex))
                        highlightedIndex.Add(highlightIndex);
                }
            }
            #endregion
            #region StartPageGreaterThanEndPage
            else if (startPageIndex < endPageIndex)
            {
                int startLine = 0, endLine = 0;
                if (startPageIndexBack == startPageIndex)
                {
                    if (endPageIndex == endPageIndexBack)
                    {
                        if (endLineIndex > endLineIndexBack)
                        {
                            int difference = endLineIndex - endLineIndexBack;
                            for (float i = endLineIndexBack; i <= endLineIndex; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                        }
                        else if (endLineIndex < endLineIndexBack)
                        {
                            int difference = endLineIndexBack - endLineIndex;
                            for (float i = endLineIndexBack; i >= endLineIndex; i--)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                        }
                    }
                    else if (endPageIndex > endPageIndexBack)
                    {
                        startLine = endLineIndexBack;
                        endLine = TextSelectionPageCollection[endPageIndexBack].WordCount;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = highlightBrush;

                            if (!highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Add(highlightIndex);
                        }
                        startLine = 1;
                        endLine = endLineIndex;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = highlightBrush;

                            if (!highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Add(highlightIndex);
                        }
                    }
                    else if (endPageIndex < endPageIndexBack)
                    {
                        startLine = 1;
                        endLine = endLineIndexBack;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0)); ;

                            if (highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Remove(highlightIndex);
                        }
                        startLine = endLineIndex;
                        endLine = TextSelectionPageCollection[endPageIndex].WordCount;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0)); ;

                            if (highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Remove(highlightIndex);
                        }
                    }
                }
            }
            #endregion
            #region StartPageLessThanEndPage
            else
            {
                IsStartPageLessThanEndPage = false;
                int startLine = 0, endLine = 0;
                if (startPageIndexBack == startPageIndex)
                {
                    if (endPageIndex == endPageIndexBack)
                    {
                        if (endLineIndex > endLineIndexBack)
                        {
                            int difference = endLineIndex - endLineIndexBack;
                            for (float i = endLineIndexBack; i <= endLineIndex; i++)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                                if (highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Remove(highlightIndex);
                            }
                        }
                        else if (endLineIndex < endLineIndexBack)
                        {
                            int difference = endLineIndexBack - endLineIndex;
                            for (float i = endLineIndexBack; i >= endLineIndex; i--)
                            {
                                if (i % 10 == 0)
                                {
                                    continue;
                                }
                                float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                                Line highlightLine = textSelectIndexSequence[highlightIndex];
                                highlightLine.Stroke = highlightBrush;

                                if (!highlightedIndex.Contains(highlightIndex))
                                    highlightedIndex.Add(highlightIndex);
                            }
                        }
                    }
                    else if (endPageIndex > endPageIndexBack)
                    {
                        startLine = endLineIndexBack;
                        endLine = TextSelectionPageCollection[endPageIndexBack].WordCount;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                            if (highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Remove(highlightIndex);
                        }
                        startLine = 1;
                        endLine = endLineIndex;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = new SolidColorBrush(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));

                            if (highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Remove(highlightIndex);
                        }
                    }
                    else if (endPageIndex < endPageIndexBack)
                    {
                        startLine = 1;
                        endLine = endLineIndexBack;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndexBack.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = highlightBrush;

                            if (!highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Add(highlightIndex);
                        }
                        startLine = endLineIndex;
                        endLine = TextSelectionPageCollection[endPageIndex].WordCount;
                        for (float i = startLine; i <= endLine; i++)
                        {
                            if (i % 10 == 0)
                            {
                                continue;
                            }
                            float highlightIndex = float.Parse(endPageIndex.ToString() + "." + i.ToString());
                            Line highlightLine = textSelectIndexSequence[highlightIndex];
                            highlightLine.Stroke = highlightBrush;

                            if (!highlightedIndex.Contains(highlightIndex))
                                highlightedIndex.Add(highlightIndex);
                        }
                    }
                }
            }
            #endregion
            IsTextSelectionRemoved = false;
            startPageIndexBack = startPageIndex;
            endPageIndexBack = endPageIndex;
            endLineIndexBack = endLineIndex;
            startLineIndexBack = startLineIndex;
        }


        bool IsTouchEndRemoved = false;
        Point pointerPressedLocation = new Point();
        Point pointerReleasedLocation = new Point();
        Line backDestLine = new Line();
        private void documentview_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                UIElement element = PdfDocumentPanel.Children[PageIndex];//current page in viewport
                Canvas page = element as Canvas;
                PointerPoint pointerPoint = e.GetCurrentPoint(page);
                if (!IsTextSelectionEnabled)
                    return;
                if (e.OriginalSource is Line)
                {
                    if ((e.OriginalSource as Line).Parent is Image)
                    {
                        curPageNo = int.Parse((((e.OriginalSource as Line).Parent as Image).Parent as Canvas).Name);
                    }
                }
                else if (e.OriginalSource is Image)
                {
                    curPageNo = int.Parse(((e.OriginalSource as Image).Parent as Canvas).Name);
                }
                if (!textSelectionEnviCreatedList.Contains(curPageNo))
                {
                    bool IsEnviCreated = false;
                    try
                    {
                        IsEnviCreated = CreateTextSelectionEnvironment(curPageNo);
                    }
                    catch
                    { }
                    if (IsEnviCreated)
                    {
                        textSelectionEnviCreatedList.Add(curPageNo);
                        textSelectionPage = new TextSelectionPage();
                        textSelectionPage.WordCount = lineCounter - 1;
                        textSelectionPage.PageNumber = curPageNo;
                        Object keys = textSelectionDifferentLineEnd.Keys;
                        List<double> keyList = keys as List<double>;

                        //List<KeyValuePair<double,Line> myList=textSelectionDifferentLineEnd.tolic
                        textSelectionPage.TextSelectionDifferentLines = new Dictionary<double, Line>(textSelectionDifferentLineEnd);
                        TextSelectionPageCollection.Add(curPageNo, textSelectionPage);
                    }
                }
                #region Touch
                if (e.Pointer.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    if (IsEllipsePressed)
                    {
                        if (element is Canvas)
                        {
                            double viewportHeight = this.documentScrollViewer.ViewportHeight;
                            double viewportWidth = this.documentScrollViewer.ViewportWidth;

                            if (!IsTouchEndRemoved)
                            {
                                Canvas previousCanvas = PdfDocumentPanel.Children[textEndPageIndex] as Canvas;
                                previousCanvas.Children.Remove(touchEnd);
                                IsTouchEndRemoved = true;
                            }

                            Canvas.SetLeft(touchEnd, pointerPoint.Position.X - (touchEnd.Width / 2));
                            Canvas.SetTop(touchEnd, pointerPoint.Position.Y);
                            PointerPoint scrollPointerPoint = e.GetCurrentPoint(this.documentScrollViewer);
                            if (viewportHeight - 30 < scrollPointerPoint.Position.Y)
                            {
                                double currentVerticalOffset = this.documentScrollViewer.VerticalOffset;
                                this.documentScrollViewer.ChangeView(null, currentVerticalOffset + 100, null);
                            }

                            if (scrollPointerPoint.Position.Y < 10 && this.documentScrollViewer.VerticalOffset > 0)
                            {
                                double currentVerticalOffset = this.documentScrollViewer.VerticalOffset;
                                if (currentVerticalOffset - 100 > 0)
                                    this.documentScrollViewer.ChangeView(null, currentVerticalOffset - 100, null);
                                else
                                    this.documentScrollViewer.ChangeView(null, 0, null);
                            }

                            if (viewportWidth - 30 < scrollPointerPoint.Position.X)
                            {
                                double currentHorizontalOffset = this.documentScrollViewer.HorizontalOffset;
                                this.documentScrollViewer.ChangeView(currentHorizontalOffset + 100, null, null);
                            }

                            if (scrollPointerPoint.Position.X < 10 && this.documentScrollViewer.HorizontalOffset > 0)
                            {
                                double currentHorizontalOffset = this.documentScrollViewer.HorizontalOffset;
                                if (currentHorizontalOffset - 100 > 0)
                                    this.documentScrollViewer.ChangeView(currentHorizontalOffset - 100, null, null);
                                else
                                    this.documentScrollViewer.ChangeView(null, 0, null);
                            }
                        }
                        if (e.OriginalSource is Image)
                        {
                            element = PdfDocumentPanel.Children[curPageNo];
                            page = element as Canvas;
                            pointerPoint = e.GetCurrentPoint(page);
                            Image destImage = e.OriginalSource as Image;
                            double pagePointY = pointerPoint.Position.Y;

                            TextSelectionPage currentPage = TextSelectionPageCollection[curPageNo];
                            Dictionary<double, Line> currentPageLineHight = currentPage.TextSelectionDifferentLines;
                            Line destLine = null;
                            int i = 0;
                            foreach (KeyValuePair<double, Line> element1 in currentPageLineHight)
                            {
                                i += 1;
                                if (pagePointY > element1.Key)
                                    destLine = element1.Value;
                                else
                                    break;
                            }
                            if (destLine != null)
                            {
                                if (backDestLine == destLine)
                                {
                                    if (textSelectLineSequence.ContainsKey(destLine))
                                    {
                                        textSelectionEndIndex = textSelectLineSequence[destLine];
                                        try
                                        {
                                            HighlightIntermediate();
                                        }
                                        catch
                                        { }
                                    }
                                }
                                backDestLine = destLine;
                            }
                        }
                        else if (e.OriginalSource is Line)
                        {
                            Line highlightLine = e.OriginalSource as Line;
                            textSelectionEndIndex = textSelectLineSequence[highlightLine];
                            global::Windows.UI.Xaml.Media.Matrix transf = (highlightLine.RenderTransform as MatrixTransform).Matrix;

                            double scaleAndYvalue;
                            double currentOffsetY;
                            double YAxis;

                            double scaleAndXvalue;
                            double currentOffsetX;
                            double Xaxis;

                            if (textSelectionStartIndex < textSelectionEndIndex)
                            {
                                scaleAndYvalue = transf.M22 * highlightLine.Y2;
                                currentOffsetY = scaleAndYvalue + transf.OffsetY;
                                YAxis = currentOffsetY;

                                scaleAndXvalue = transf.M11 * highlightLine.X2;
                                currentOffsetX = scaleAndXvalue + transf.OffsetX;
                                Xaxis = currentOffsetX;
                            }
                            else
                            {
                                scaleAndYvalue = transf.M22 * highlightLine.Y1;
                                currentOffsetY = scaleAndYvalue + transf.OffsetY;
                                YAxis = currentOffsetY;

                                scaleAndXvalue = transf.M11 * highlightLine.X1;
                                currentOffsetX = scaleAndXvalue + transf.OffsetX;
                                Xaxis = currentOffsetX;
                            }
                            try
                            {
                                HighlightIntermediate();
                            }
                            catch
                            { }
                        }
                    }
                }
                #endregion
                else if (e.Pointer.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Mouse)
                {
                    if (IsMousePressed)
                    {
                        if (e.OriginalSource is Line)
                        {
                            if (textSelectionTouchLineMouse == null)
                            {
                                textSelectionTouchLineMouse = e.OriginalSource as Line;
                            }
                            if (textSelectLineSequence.ContainsKey(textSelectionTouchLineMouse))
                            {
                                textSelectionStartIndex = textSelectLineSequence[textSelectionTouchLineMouse];
                                Line textEndLine = e.OriginalSource as Line;
                                if (textSelectLineSequence.ContainsKey(textEndLine))
                                {
                                    textSelectionEndIndex = textSelectLineSequence[textEndLine];
                                    try
                                    {
                                        HighlightIntermediate();
                                    }
                                    catch
                                    { }
                                }
                            }
                        }

                        if (e.OriginalSource is Image)
                        {
                            Image destImage = e.OriginalSource as Image;
                            double pagePointY = pointerPoint.Position.Y;

                            TextSelectionPage currentPage = TextSelectionPageCollection[curPageNo];
                            Dictionary<double, Line> currentPageLineHight = currentPage.TextSelectionDifferentLines;
                            Line destLine = null;
                            int i = 0;
                            foreach (KeyValuePair<double, Line> element1 in currentPageLineHight)
                            {
                                i += 1;
                                if (pagePointY > element1.Key)
                                    destLine = element1.Value;
                                else
                                    break;
                            }
                            if (destLine != null)
                            {
                                if (backDestLine == destLine)
                                {
                                    if (textSelectLineSequence.ContainsKey(destLine))
                                    {
                                        textSelectionEndIndex = textSelectLineSequence[destLine];
                                        try
                                        {
                                            HighlightIntermediate();
                                        }
                                        catch
                                        { }
                                    }
                                }
                                backDestLine = destLine;
                            }
                        }

                        PointerPoint scrollPointerPoint = e.GetCurrentPoint(this.documentScrollViewer);
                        double viewportHeight = this.documentScrollViewer.ViewportHeight;
                        double viewportWidth = this.documentScrollViewer.ViewportWidth;
                        if (viewportHeight - 30 < scrollPointerPoint.Position.Y)
                        {
                            double currentVerticalOffset = this.documentScrollViewer.VerticalOffset;
                            this.documentScrollViewer.ChangeView(null, currentVerticalOffset + 100, null);
                        }

                        if (scrollPointerPoint.Position.Y < 10 && this.documentScrollViewer.VerticalOffset > 0)
                        {
                            double currentVerticalOffset = this.documentScrollViewer.VerticalOffset;
                            if (currentVerticalOffset - 100 > 0)
                                this.documentScrollViewer.ChangeView(null, currentVerticalOffset - 100, null);
                            else
                                this.documentScrollViewer.ChangeView(null, 0, null);
                        }

                        if (viewportWidth - 30 < scrollPointerPoint.Position.X)
                        {
                            double currentHorizontalOffset = this.documentScrollViewer.HorizontalOffset;
                            this.documentScrollViewer.ChangeView(currentHorizontalOffset + 100, null, null);
                        }

                        if (scrollPointerPoint.Position.X < 10 && this.documentScrollViewer.HorizontalOffset > 0)
                        {
                            double currentHorizontalOffset = this.documentScrollViewer.HorizontalOffset;
                            if (currentHorizontalOffset - 100 > 0)
                                this.documentScrollViewer.ChangeView(currentHorizontalOffset - 100, null, null);
                            else
                                this.documentScrollViewer.ChangeView(null, 0, null);
                        }
                    }
                }
            }
            catch
            { }
        }

        private Ellipse touchStart;
        private Ellipse touchEnd;
        private TextSelectionCopyButton btnCopy;
        private bool IsEllipsePressed = false;

        Line textSelectionTouchLine;
        Line textSelectionTouchLineMouse;
        private bool IsMousePressed = false;
        private bool IsSelectionRemoved = false;

        private void documentview_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (!IsTextSelectionEnabled)
                    return;
                if (e.OriginalSource is Line)
                {
                    if ((e.OriginalSource as Line).Parent is Image)
                    {
                        curPageNo = int.Parse((((e.OriginalSource as Line).Parent as Image).Parent as Canvas).Name);
                    }
                }
                else if (e.OriginalSource is Image)
                {
                    curPageNo = int.Parse(((e.OriginalSource as Image).Parent as Canvas).Name);
                }
                if (!textSelectionEnviCreatedList.Contains(curPageNo))
                {
                    bool IsEnviCreated = false;
                    try
                    {
                        IsEnviCreated = CreateTextSelectionEnvironment(curPageNo);
                    }
                    catch
                    { }
                    if (IsEnviCreated)
                    {
                        textSelectionEnviCreatedList.Add(curPageNo);
                        textSelectionPage = new TextSelectionPage();
                        textSelectionPage.WordCount = lineCounter - 1;
                        textSelectionPage.PageNumber = curPageNo;
                        Object keys = textSelectionDifferentLineEnd.Keys;
                        List<double> keyList = keys as List<double>;

                        //List<KeyValuePair<double,Line> myList=textSelectionDifferentLineEnd.tolic
                        textSelectionPage.TextSelectionDifferentLines = new Dictionary<double, Line>(textSelectionDifferentLineEnd);
                        TextSelectionPageCollection.Add(curPageNo, textSelectionPage);
                    }
                }
                UIElement element = PdfDocumentPanel.Children[PageIndex];
                Canvas page = element as Canvas;
                pointerPressedLocation = e.GetCurrentPoint(page).Position;
                #region Touch
                if (e.Pointer.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    if (!TextSelctionMode)
                    {
                        if (e.OriginalSource is Line)
                        {
                            textSelectionTouchLine = e.OriginalSource as Line;
                        }
                    }
                    IsEllipsePressed = false;
                    if (TextSelctionMode)
                    {
                        if (e.OriginalSource is Ellipse)
                        {
                            IsEllipsePressed = true;
                            page.ManipulationMode = ManipulationModes.All;
                            if (btnCopy != null)
                                btnCopy.Visibility = Visibility.Collapsed;
                        }
                        //else if (e.OriginalSource is Image)
                        //{
                        //    RemoveAllSelection();
                        //    RemoveTextSelctionControls(page);
                        //}
                        e.Handled = true;
                    }
                }
                #endregion
                else if (e.Pointer.PointerDeviceType == global::Windows.Devices.Input.PointerDeviceType.Mouse)
                {
                    if (highlightedIndex.Count > 0)
                    {
                        RemoveAllSelection();
                        RemoveTextSelctionControls(page);
                    }
                    else if (e.OriginalSource is Line)
                    {
                        textSelectionTouchLineMouse = e.OriginalSource as Line;
                        IsMousePressed = true;
                    }
                    //IsMousePressed = true;
                }
            }
            catch
            { }
        }

        private void RemoveTextSelctionControls(Canvas page)
        {
            //if (page.Children.Contains(btnCopy))
            //    page.Children.Remove(btnCopy);

            //if (page.Children.Contains(touchEnd))
            //    page.Children.Remove(touchEnd);

            //if (page.Children.Contains(touchStart))
            //    page.Children.Remove(touchStart);
            //UIElement element = PdfDocumentPanel.Children[textStartPageIndex];
            //Canvas startPage = element as Canvas;
            //if (startPage.Children.Contains(touchEnd))
            //    startPage.Children.Remove(touchEnd);

            Canvas parentPage = new Canvas();
            if (btnCopy != null && btnCopy.Parent != null)
            {
                parentPage = btnCopy.Parent as Canvas;
                if (parentPage.Children.Contains(btnCopy))
                    parentPage.Children.Remove(btnCopy);
            }

            if (touchStart != null && touchStart.Parent != null)
            {
                parentPage = touchStart.Parent as Canvas;
                if (parentPage.Children.Contains(touchStart))
                    parentPage.Children.Remove(touchStart);
            }

            if (touchEnd != null && touchEnd.Parent != null)
            {
                parentPage = touchEnd.Parent as Canvas;
                if (parentPage.Children.Contains(touchEnd))
                    parentPage.Children.Remove(touchEnd);
            }
        }


        public void ResetTextSelection()
        {
            UIElement element = PdfDocumentPanel.Children[0];//current page in viewport
            Canvas page = element as Canvas;
            if (element is Canvas)
            {
                foreach (UIElement child in page.Children)
                {
                    if (child.GetType().Name == "Line")
                    {
                        if (child is Line && (child as Line).Name == "")
                            page.Children.Remove(child);
                    }
                }
            }



        }
        #endregion

#endif
    }

    // <summary>
    /// The UIDispatchr class is internally used for printing
    /// </summary>    
    internal class UIDispatcher
    {
        private static CoreDispatcher Dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        /// <summary>
        /// Executes the action using UIElement.
        /// </summary>
        /// <param name="action">An Action.</param>
        internal static void Execute(Action action)
        {
            if (CoreApplication.MainView.CoreWindow == null
                || Dispatcher.HasThreadAccess)
                action();
            else
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask().Wait();
        }
    }

    public class PdfTextCoordinates
    {
        int PageIndex;
        double Xaxis;
        double Yaxis;
        double Width;
        double Height;
        string Text;
        public PdfTextCoordinates(string Text, int PageIndex, double Xaxis, double Yaxis, double Width, double Height)
        {
            this.PageIndex = PageIndex;
            this.Text = Text;
            this.Xaxis = Xaxis;
            this.Yaxis = Yaxis;
            this.Height = Height;
            this.Width = Width;
        }
    }
}
