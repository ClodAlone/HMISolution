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
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.IO;
using Windows.UI.Xaml;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;

using Syncfusion.Pdf.Parsing;
using System.Globalization;

namespace Syncfusion.Windows.PdfViewer
{
    [TemplatePart(Name = "ViewerGrid", Type = typeof(Grid))]
    public class SfPdfViewerControl : Control
    {
        PdfDocumentView m_documentView;
        private Grid ViewerGrid;
        PdfLoadedDocument m_loadedDocuemnt;
        private bool m_intialViewModeChanged;
        private bool m_enableThumbnailView = true;
        private bool m_isTextSelectionEnabled = true;
        private bool m_showPageNumber = true;
        internal bool IsAsync;
        internal PdfLoadedDocument textSearchLoadedDocument = new PdfLoadedDocument();
        public SfPdfViewerControl()
        {
            this.SizeChanged += SfPdfViewerControl_SizeChanged;
            DefaultStyleKey = typeof(SfPdfViewerControl);
            this.IsTabStop = true;
        }

        void SfPdfViewerControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_loadedDocuemnt != null)
                this.LoadDocument(m_loadedDocuemnt);
        }

        # region Events
        public event EventHandler<DocumentLoadedEventArgs> DocumentLoaded;
        public event EventHandler<ZoomChangedEventArgs> ZoomChanged;
        public event EventHandler<PageChangedEventArgs> PageChanged;

        internal void OnDocumentLoaded(DocumentLoadedEventArgs eventArgs)
        {
            if (DocumentLoaded != null && eventArgs != null)
                DocumentLoaded(this, eventArgs);
        }

        internal void OnZoomChanged(ZoomChangedEventArgs eventArgs)
        {
            if (ZoomChanged != null && eventArgs != null)
                ZoomChanged(this, eventArgs);
        }

        private void OnPageChanged(PageChangedEventArgs eventArgs)
        {
            if (PageChanged != null && eventArgs != null)
                PageChanged(this, eventArgs);
        }
        #endregion

        # region Properties
        public bool IsThumbnailViewEnabled
        {
            get
            {
                return m_enableThumbnailView;
            }
            set
            {
                m_enableThumbnailView = value;
            }
        }
#if !SyncfusionFramework4_5
        public bool IsTextSelectionEnabled
        {
            get { return ((bool)GetValue(IsTextSelectionEnabledProperty)); }
            set { SetValue(IsTextSelectionEnabledProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PageNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextSelectionEnabledProperty =
            DependencyProperty.Register("IsTextSelectionEnabled", typeof(bool), typeof(PdfDocumentView), new PropertyMetadata(true, new PropertyChangedCallback(TextSelectionCallBack)));

        private static void TextSelectionCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfPdfViewerControl viewer = (SfPdfViewerControl)d;
            if (viewer.m_documentView != null)
            {
                viewer.m_documentView.IsTextSelectionEnabled = (bool)e.NewValue;
            }
            viewer.m_isTextSelectionEnabled = (bool)e.NewValue;
        }
#endif
        public int PageNumber
        {
            get { return ((int)GetValue(PageNumberProperty) + 1); }
            internal set { SetValue(PageNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageNumberProperty =
            DependencyProperty.Register("PageNumber", typeof(int), typeof(PdfDocumentView), new PropertyMetadata(0, new PropertyChangedCallback(PageNumberCallBack)));

        private static void PageNumberCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfPdfViewerControl viewer = (SfPdfViewerControl)d;
            if (e.OldValue != e.NewValue)
            {
                PageChangedEventArgs args = new PageChangedEventArgs(int.Parse(((int)(e.OldValue) + 1).ToString(), CultureInfo.InvariantCulture), int.Parse(((int)(e.NewValue) + 1).ToString(), CultureInfo.InvariantCulture));
                viewer.OnPageChanged(args);
            }
        }

        /// <summary>
        /// Gets the total page count
        /// </summary>
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            internal set { SetValue(PageCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(SfPdfViewerControl), new PropertyMetadata(0));

        public PageViewMode ViewMode
        {
            get { return (PageViewMode)GetValue(ViewModeProperty); }
            set { SetValue(ViewModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register("ViewMode", typeof(PageViewMode), typeof(SfPdfViewerControl), new PropertyMetadata(PageViewMode.Normal, new PropertyChangedCallback(DisplayCallBack)));

        private static void DisplayCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfPdfViewerControl viewer = (SfPdfViewerControl)d;
            double zfactor = 100;
            switch (viewer.ViewMode)
            {
                case PageViewMode.FitWidth:
                    zfactor = (viewer.ActualWidth / viewer.m_documentView.OriginalWidth) * 100;
                    break;
                case PageViewMode.OnePage:
                    zfactor = viewer.m_documentView.documentScrollViewer.MinZoomFactor * 100;
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
            else
            {
                viewer.IsIntialViewModeChanged = true;
            }
        }
        public int Zoom
        {
            get { return (int)InternalZoom; }
        }

        internal int DocumentViewPageIndex
        {
            get { return PageNumber; }
            set { PageNumber = value; }
        }

        internal float InternalZoom
        {
            get { return (float)GetValue(InternalZoomProperty); }
            set { SetValue(InternalZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Zoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalZoomProperty =
            DependencyProperty.Register("InternalZoom", typeof(float), typeof(SfPdfViewerControl), new PropertyMetadata(100f, new PropertyChangedCallback(ZoomCallBack)));

        private static void ZoomCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfPdfViewerControl viewer = (SfPdfViewerControl)d;
            float zoom = viewer.InternalZoom;

            char[] separator = { '.', ',' };
            string oldVal, newVal;
            oldVal = e.OldValue.ToString().Split(separator)[0];
            newVal = e.NewValue.ToString().Split(separator)[0];

            ZoomChangedEventArgs args = new ZoomChangedEventArgs(int.Parse(oldVal.ToString(), CultureInfo.InvariantCulture), int.Parse(newVal.ToString(), CultureInfo.InvariantCulture));
            viewer.OnZoomChanged(args);
            if (viewer.m_documentView != null)
                viewer.m_documentView.SetZoom(zoom / 100);
        }

        public bool ShowPageNumber
        {
            get { return (bool)GetValue(ShowPageNumberProperty); }
            set
            {
                SetValue(ShowPageNumberProperty, value);
                if (m_documentView != null)
                    m_documentView.ShowPageNumber = value;
                this.m_showPageNumber = value;
            }
        }

        // Using a DependencyProperty as the backing store for ShowPageNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPageNumberProperty =
            DependencyProperty.Register("ShowPageNumber", typeof(bool), typeof(SfPdfViewerControl), new PropertyMetadata(true));

        public Object ItemsSource
        {
            get { return (Object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(Object), typeof(SfPdfViewerControl), new PropertyMetadata(null, new PropertyChangedCallback(DocumentLoadCallBack)));

        private static void DocumentLoadCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfPdfViewerControl viewer = (SfPdfViewerControl)d;
            global::Windows.Storage.StorageFile file = viewer.ItemsSource as global::Windows.Storage.StorageFile;
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument();
            loadedDocument.OpenAsync(file).Wait();
            viewer.LoadDocumentAsync(loadedDocument).Wait();
            viewer.PageCount = loadedDocument.PageCount;
        }

        internal bool IsIntialViewModeChanged
        {
            get
            {
                return m_intialViewModeChanged;
            }
            set
            {
                m_intialViewModeChanged = value;
            }
        }
        #endregion

        # region Commands

        public ICommand FirstPageCommand
        {
            get { return (ICommand)GetValue(FirstPageCommandProperty); }
            set { SetValue(FirstPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstPageCommandProperty =
            DependencyProperty.Register("FirstPageCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object FirstPageCommandParameter
        {
            get { return (object)GetValue(FirstPageCommandParameterProperty); }
            set { SetValue(FirstPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstPageCommandParameterProperty =
            DependencyProperty.Register("FirstPageCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public ICommand LastPageCommand
        {
            get { return (ICommand)GetValue(LastPageCommandProperty); }
            set { SetValue(LastPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastPageCommandProperty =
            DependencyProperty.Register("LastPageCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object LastPageCommandParameter
        {
            get { return (object)GetValue(LastPageCommandParameterProperty); }
            set { SetValue(LastPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastPageCommandParameterProperty =
            DependencyProperty.Register("LastPageCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public ICommand NextPageCommand
        {
            get { return (ICommand)GetValue(NextPageCommandProperty); }
            set { SetValue(NextPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register("NextPageCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object NextPageCommandParameter
        {
            get { return (object)GetValue(NextPageCommandParameterProperty); }
            set { SetValue(NextPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextPageCommandParameterProperty =
            DependencyProperty.Register("NextPageCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public ICommand PreviousPageCommand
        {
            get { return (ICommand)GetValue(PreviousPageCommandProperty); }
            set { SetValue(PreviousPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousPageCommandProperty =
            DependencyProperty.Register("PreviousPageCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object PreviousPageCommandParameter
        {
            get { return (object)GetValue(PreviousPageCommandParameterProperty); }
            set { SetValue(PreviousPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousPageCommandParameterProperty =
            DependencyProperty.Register("PreviousPageCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public ICommand IncreaseZoomCommand
        {
            get { return (ICommand)GetValue(IncreaseZoomCommandProperty); }
            set { SetValue(IncreaseZoomCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IncreaseZoomCommandProperty =
            DependencyProperty.Register("IncreaseZoomCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object IncreaseZoomCommandParameter
        {
            get { return (object)GetValue(IncreaseZoomCommandParameterProperty); }
            set { SetValue(IncreaseZoomCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IncreaseZoomCommandParameterProperty =
            DependencyProperty.Register("IncreaseZoomCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public ICommand DecreaseZoomCommand
        {
            get { return (ICommand)GetValue(DecreaseZoomCommandProperty); }
            set { SetValue(DecreaseZoomCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecreaseZoomCommandProperty =
            DependencyProperty.Register("DecreaseZoomCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object DecreaseZoomCommandParameter
        {
            get { return (object)GetValue(DecreaseZoomCommandParameterProperty); }
            set { SetValue(DecreaseZoomCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecreaseZoomCommandParameterProperty =
            DependencyProperty.Register("DecreaseZoomCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        //GoToPage
        public ICommand GoToPageCommand
        {
            get { return (ICommand)GetValue(GoToPageCommandProperty); }
            set { SetValue(GoToPageCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for DecreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoToPageCommandProperty =
            DependencyProperty.Register("GoToPageCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object GoToPageCommandParameter
        {
            get { return (object)GetValue(GoToPageCommandParameterProperty); }
            set { SetValue(GoToPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoToPageCommandParameterProperty =
            DependencyProperty.Register("GoToPageCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public ICommand ViewModeCommand
        {
            get { return (ICommand)GetValue(ViewModeCommandProperty); }
            set { SetValue(ViewModeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModeCommandProperty =
            DependencyProperty.Register("ViewModeCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object ViewModeCommandParameter
        {
            get { return (object)GetValue(ViewModeCommandParameterProperty); }
            set { SetValue(ViewModeCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModeCommandParameterProperty =
            DependencyProperty.Register("ViewModeCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        #region SwitchModeCommand
        public ICommand SwitchToThumbnailModeCommand
        {
            get { return (ICommand)GetValue(SwitchToThumbNailModeCommandProperty); }
            set { SetValue(SwitchToThumbNailModeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchToThumbNailModeCommandProperty =
            DependencyProperty.Register("SwitchModeCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object SwitchToThumbnailModeCommandParameter
        {
            get { return (object)GetValue(SwitchToThumnailModeCommandParameterProperty); }
            set { SetValue(SwitchToThumnailModeCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchToThumnailModeCommandParameterProperty =
            DependencyProperty.Register("SwitchModeCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));
        #endregion

        #region TextSearchCommands
        public ICommand SearchNextCommand
        {
            get { return (ICommand)GetValue(SearchNextCommandProperty); }
            set { SetValue(SearchNextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchNextCommandProperty =
            DependencyProperty.Register("SearchNextCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object SearchNextCommandParameter
        {
            get { return (object)GetValue(SearchNextCommandParameterProperty); }
            set { SetValue(SearchNextCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchNextCommandParameterProperty =
            DependencyProperty.Register("SearchNextCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        //Saerch Previous

        public ICommand SearchPreviousCommand
        {
            get { return (ICommand)GetValue(SearchPreviousCommandProperty); }
            set { SetValue(SearchPreviousCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchPreviousCommandProperty =
            DependencyProperty.Register("SearchPreviousCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object SearchPreviousCommandParameter
        {
            get { return (object)GetValue(SearchPreviousCommandParameterProperty); }
            set { SetValue(SearchPreviousCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchPreviousCommandParameterProperty =
            DependencyProperty.Register("SearchPreviousCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        //ClearTextSelection
        public ICommand ClearTextSelectionCommand
        {
            get { return (ICommand)GetValue(ClearTextSelectionCommandProperty); }
            set { SetValue(ClearTextSelectionCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearTextSelectionCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearTextSelectionCommandProperty =
            DependencyProperty.Register("ClearTextSelectionCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object ClearTextSelectionCommandParameter
        {
            get { return (object)GetValue(ClearTextSelectionCommandParameterProperty); }
            set { SetValue(ClearTextSelectionCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearTextSelectionCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearTextSelectionCommandParameterProperty =
            DependencyProperty.Register("ClearTextSelectionCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));
        #endregion

        //Printing
        public ICommand PrintCommand
        {
            get { return (ICommand)GetValue(PrintCommandProperty); }
            set { SetValue(PrintCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintCommandProperty =
            DependencyProperty.Register("PrintCommand", typeof(ICommand), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        public object PrintCommandParameter
        {
            get { return (object)GetValue(PrintCommandParameterProperty); }
            set { SetValue(PrintCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintCommandParameterProperty =
            DependencyProperty.Register("PrintCommandParameter", typeof(object), typeof(SfPdfViewerControl), new PropertyMetadata(null));

        #endregion

        # region Methods

        /// <summary>
        /// Prints all pages in the viewer.
        /// </summary>
        public void Print()
        {
            if (m_documentView != null && m_documentView.LoadedDocument != null)
                m_documentView.Print();
        }

        private void Initialize()
        {
            m_documentView = new PdfDocumentView(this);
            ViewerGrid.Children.Add(m_documentView);

            Binding bindProp = new Binding();
            bindProp.Path = new PropertyPath("PageIndex");
            bindProp.Source = m_documentView;
            SetBinding(SfPdfViewerControl.PageNumberProperty, bindProp);

            // Commands
            FirstPageCommand = new DelegateCommand(OnFirstPageCommand, CanExecute);
            LastPageCommand = new DelegateCommand(OnLastPageCommand, CanExecute);
            PreviousPageCommand = new DelegateCommand(OnPreviousPageCommand, CanExecute);
            NextPageCommand = new DelegateCommand(OnNextPageCommand, CanExecute);
            IncreaseZoomCommand = new DelegateCommand(OnIncreaseZoomCommand, CanExecute);
            DecreaseZoomCommand = new DelegateCommand(OnDecreaseZoomCommand, CanExecute);
            PrintCommand = new DelegateCommand(OnPrintCommand, CanExecute);
            GoToPageCommand = new DelegateCommand(OnGoToPageCommand, CanExecute);
            ViewModeCommand = new DelegateCommand(OnViewModeCommand, CanExecute);
#if !SyncfusionFramework4_5
            SwitchToThumbnailModeCommand = new DelegateCommand(OnSwitchThumnailModeCommand, CanExecute);
            SearchNextCommand = new DelegateCommand(OnSearchNextCommand, CanExecute);
            SearchPreviousCommand = new DelegateCommand(OnSearchPreviousCommand, CanExecute);
            ClearTextSelectionCommand=new DelegateCommand(OnClearTextSelectionCommand, CanExecute);
#endif
        }

        protected override void OnApplyTemplate()
        {
            ViewerGrid = GetTemplateChild("ViewerGrid") as Grid;
            Initialize();
            if (m_loadedDocuemnt != null)
                m_documentView.LoadedDocument = m_loadedDocuemnt;
        }

#if !SyncfusionFramework4_5
        /// <summary>
        /// Search for the existence of a text and returns the text positions in a PDF page 
        /// </summary>
        /// <param name="targetText">The text to be searched</param>
        /// <param name="pageIndex">The targeted page index in which search to be performed</param>
        /// <param name="coordinatesList">Returns the text coordinates available in the PDF page</param>
        public void GetTextCoordinates(string targetText, int pageIndex, out List<PdfTextCoordinates> coordinatesList)
        {
            m_documentView.textCoordinatesList.Clear();
            coordinatesList = new List<PdfTextCoordinates>();
            m_documentView.SearchTextCoordinates(targetText, pageIndex, out coordinatesList);
        }

        /// <summary>
        /// Search the next occurance of the text
        /// </summary>
        /// <param name="targetString">The text to be searched</param>
        public void SearchNextText(string targetString)
        {
            m_documentView.SearchNextText(targetString);
        }
        /// <summary>
        /// Search the previous occurance of the text
        /// </summary>
        /// <param name="targetString">The text to be searched</param>
        public void SearchPrevText(string targetString)
        {
            m_documentView.SearchPrevText(targetString);
        }
        internal void OnSwitchThumnailModeCommand(object swicthMode)
        {
            m_documentView.semanticZoom.ToggleActiveView();
            m_documentView.UpdateLayout();
        }
        /// <summary>
        /// Search the next occurance of the text
        /// </summary>
        /// <param name="targetString">The text to be searched</param>
        internal void OnSearchNextCommand(object targetString)
        {
            string destPageStr = targetString as String;
            int destPage = -1;
            bool result = Int32.TryParse(destPageStr, out destPage);

            SearchNextText(destPageStr);
        }
        /// <summary>
        /// Search the previous occurance of the text
        /// </summary>
        /// <param name="targetString">The text to be searched</param>
        internal void OnSearchPreviousCommand(object targetString)
        {
            string destStr = targetString as String;
            int destPage = -1;
            bool result = Int32.TryParse(destStr, out destPage);
            SearchPrevText(destStr);
        }

        /// <summary>
        /// Clears the text highlightings
        /// </summary>
        internal void OnClearTextSelectionCommand(object value)
        {
            m_documentView.ClearTextSearchHighlightings();
            for (int i = 0; i < PageCount; i++)
            {
                m_documentView.RemoveTextSearchHighlightings(i);
            }
        }

#endif
        /// <summary>
        /// Loads the PdfLoadedDocument into the PDF Viewer
        /// </summary>
        /// <param name="loadedDocument">PdfLoadedDocument to be loaded</param>
        public void LoadDocument(PdfLoadedDocument loadedDocument)
        {
#if !SyncfusionFramework4_5
            textSearchLoadedDocument = loadedDocument;
            textSearchLoadedDocument.IsPdfViewerDocumentDisable = false;
#endif
            if (m_documentView == null)
            {
                m_loadedDocuemnt = loadedDocument;
                IsAsync = false;
                return;
            }
            loadedDocument.IsPdfViewerDocumentDisable = false;
            if (m_documentView.LoadedDocument != null)
                m_documentView.Unload();
            m_loadedDocuemnt = loadedDocument;
            m_documentView.LoadedDocument = loadedDocument;
            DocumentLoadedEventArgs args = new DocumentLoadedEventArgs();
            OnDocumentLoaded(args);
#if !SyncfusionFramework4_5
            m_documentView.IsTextSelectionEnabled = this.m_isTextSelectionEnabled;
#endif
            m_documentView.ShowPageNumber = this.m_showPageNumber;
            m_documentView.LoadPages();

            m_documentView.controlName = Guid.NewGuid();
            m_documentView.ShowPageNumber = this.ShowPageNumber;
            PageCount = loadedDocument.PageCount;
        }

        /// <summary>
        /// Loads the PDF document created with the stream into the PDF Viewer
        /// </summary>
        /// <param name="stream">Document stream</param>
        public void LoadDocument(Stream stream)
        {
            LoadDocument(new PdfLoadedDocument(stream));
        }

        /// <summary>
        /// Loads the encrypted PDF document created from the stream into the PdfViewer control
        /// </summary>
        /// <param name="stream">Document Stream</param>
        /// <param name="password">Password</param>
        public void LoadDocument(Stream stream, string password)
        {
            LoadDocument(new PdfLoadedDocument(stream, password));
        }

        /// <summary>
        /// Load the PDF document into the PdfViewer control
        /// </summary>
        public async Task<bool> LoadDocumentAsync(PdfLoadedDocument loadedDocument)
        {
            loadedDocument.IsPdfViewerDocumentDisable = false;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (m_documentView == null)
            {
                m_loadedDocuemnt = loadedDocument;
                IsAsync = true;
                tcs.SetResult(true);
                return await tcs.Task;
            }
            if (m_documentView.LoadedDocument != null)
            {
                await Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_documentView.Unload();
                }).AsTask();
            }
            await Task.Run(() =>
            {
                try
                {
                    m_documentView.LoadedDocument = loadedDocument;
                    DocumentLoadedEventArgs args = new DocumentLoadedEventArgs();
                    OnDocumentLoaded(args);
#if !SyncfusionFramework4_5
                    m_documentView.IsTextSelectionEnabled = this.m_isTextSelectionEnabled;
#endif
                    m_documentView.ShowPageNumber = this.m_showPageNumber;
                    m_documentView.LoadPages();
                    Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        PageCount = loadedDocument.PageCount;
                        m_documentView.ShowPageNumber = this.ShowPageNumber;
                    });
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        public async Task<bool> LoadDocumentAsync(Stream stream)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            await Task.Run(() =>
            {
                try
                {
                    LoadDocumentAsync(new PdfLoadedDocument(stream)).Wait();
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }
#if DEBUG
        public void Dispose()
        {
            m_documentView.Dispose();
            PageCount = 0;
            m_documentView = null;
        }

        public List<Stream> ExportAsImages(int startIndex, int endIndex)
        {
            if (m_documentView == null)
            {
                m_documentView = new PdfDocumentView();
                m_documentView.LoadedDocument = m_loadedDocuemnt;
            }
            return m_documentView.ExportAsImage(startIndex, endIndex, 1);
        }
#endif
        /// <summary>
        /// Outputs an Image, which represents a Portable Document Format (PDF) page's contents
        /// </summary>
        /// <param name="pageIndex">Index of the page to be extracted as image</param>
        /// <returns>Content of the page as Image</returns>
        public Image GetPage(int pageIndex)
        {
            return GetPages(pageIndex, pageIndex)[0];
        }

        /// <summary>
        /// Outputs an Image, which represents a Portable Document Format (PDF)
        ///     page's contents with custom magnification
        /// </summary>
        /// <param name="pageIndex">Index of the page to be extracted as image</param>
        /// <param name="zoomFactor">The magnification factor</param>
        /// <returns>Content of the page with custom magnification as Image</returns>
        public Image GetPage(int pageIndex, int zoomFactor)
        {
            return GetPages(pageIndex, pageIndex, zoomFactor)[0];
        }

        /// <summary>
        /// Outputs an array of Image, which represents a Portable Document Format (PDF)
        ///     page's contents
        /// </summary>
        /// <param name="startIndex">Index of the starting page</param>
        /// <param name="endIndex">Index of the ending page</param>
        /// <returns>Content of the page as Image array</returns>
        public Image[] GetPages(int startIndex, int endIndex)
        {
            return GetPages(startIndex, endIndex, 100);
        }

        /// <summary>
        /// Outputs an array of Image, which represents a Portable Document Format (PDF)
        ///     page's contents with custom magnification
        /// </summary>
        /// <param name="startIndex">Index of the starting page</param>
        /// <param name="endIndex">Content of the page as Image array</param>
        /// <param name="zoomFactor">The magnification factor</param>
        /// <returns>Content of the page with custom magnification as Image array</returns>
        public Image[] GetPages(int startIndex, int endIndex, int zoomFactor)
        {
            if (zoomFactor < 100 || zoomFactor > 300)
            {
                throw new ArgumentOutOfRangeException("ZoomFactor", "Zoom factor should be in the range of 100-300");
            }

            if (startIndex > endIndex)
            {
                throw new ArgumentException("Start index must be greater than or equal to end index", "startIndex, ednIndex");
            }

            if (m_documentView == null)
            {

                m_documentView = new PdfDocumentView();
                m_documentView.LoadedDocument = m_loadedDocuemnt;
            }

            if (startIndex < 0 || startIndex > m_documentView.LoadedDocument.PageCount - 1)
                throw new ArgumentException("Start index should be beween 0 and page count - 1", "startIndex");
            if (endIndex < 0 || endIndex > m_documentView.LoadedDocument.PageCount - 1)
                throw new ArgumentException("End index should be between 0 to page count - 1", "endIndex");

            return m_documentView.GetPages(startIndex, endIndex, zoomFactor);
        }

        public void GotoPage(int pNumber)
        {
            if (m_documentView != null && m_documentView.LoadedDocument != null && pNumber <= PageCount)
            {
                m_documentView.SetScrollHeight(pNumber - 1);
                PageNumber = pNumber - 1;
            }
        }

        public void Unload()
        {
            m_documentView.Unload();
            PageCount = 0;
            ViewerGrid.Children.Remove(m_documentView);
            m_documentView = null;
            Initialize();
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        internal void OnFirstPageCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {
                GotoPage(1);
            }
        }

        internal void OnLastPageCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {
                GotoPage(PageCount);
            }
        }

        internal void OnPreviousPageCommand(object sender)
        {
            double pageStart;
            int pageIndex;
            m_documentView.GetPageByOffset(m_documentView.VerticalScrollPosition / m_documentView.ZoomFactor, out pageIndex, out pageStart);
            if (m_documentView != null && PageCount > 0)
            {
                if (pageIndex > 0)
                    GotoPage(pageIndex);
            }
        }

        internal void OnNextPageCommand(object sender)
        {
            double pageStart;
            int pageIndex;
            m_documentView.GetPageByOffset(m_documentView.VerticalScrollPosition / m_documentView.ZoomFactor, out pageIndex, out pageStart);
            if (m_documentView != null && PageCount > 0)
            {
                if (pageIndex < (PageCount - 1))
                {
                    GotoPage(pageIndex + 2);
                }
            }
        }

        internal void OnIncreaseZoomCommand(object sender)
        {
#if !SyncfusionFramework4_5
            if (m_documentView.semanticZoom.ZoomedOutView.IsActiveView)
            {
                m_documentView.semanticZoom.ToggleActiveView();
            }
            else if (m_documentView != null && PageCount > 0 && m_documentView.ZoomFactor < 3.0f)
            {
                InternalZoom = (float)((m_documentView.ZoomFactor * 100) + 25);
            }
#else
            if (m_documentView != null && PageCount > 0 && m_documentView.ZoomFactor < 3.0f)
            {
                InternalZoom = (float)((m_documentView.ZoomFactor * 100) + 25);
            }
#endif
        }

        internal void OnDecreaseZoomCommand(object sender)
        {
#if !SyncfusionFramework4_5
            if (m_documentView != null && PageCount > 0 && (float)m_documentView.ZoomFactor == (float)this.m_documentView.documentScrollViewer.MinZoomFactor && m_documentView.semanticZoom.ZoomedInView.IsActiveView)
            {
                m_documentView.semanticZoom.ToggleActiveView();
            }
            else if (m_documentView != null && PageCount > 0 && m_documentView.ZoomFactor > this.m_documentView.documentScrollViewer.MinZoomFactor)
            {
                InternalZoom = (float)((m_documentView.ZoomFactor * 100) - 25);
            }
#else
            if (m_documentView != null && PageCount > 0 && m_documentView.ZoomFactor > 1.0f)
            {
                InternalZoom = (float)((m_documentView.ZoomFactor * 100) - 25);
            }
#endif
        }

        internal void OnPrintCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {
                m_documentView.Print();
            }
        }

        internal void OnGoToPageCommand(object destPageParm)
        {
            string destPageStr = destPageParm as String;
            int destPage = -1;
            bool result = Int32.TryParse(destPageStr, out destPage);
            if (result && destPage > 0 && destPage <= PageCount)
            {
                GotoPage(destPage);
            }
        }
        internal void OnViewModeCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {

            }
        }

        /// <summary>
        /// Zooms the document to the specified percentage
        /// </summary>
        /// <param name="percentage">The percentage between 100 to 300</param>
        public void ZoomTo(int percentage)
        {
            if (percentage / 100 < m_documentView.documentScrollViewer.MinZoomFactor)
                percentage = 100;
            if (percentage / 100 > m_documentView.documentScrollViewer.MaxZoomFactor)
                percentage = 300;
            m_documentView.SetZoom((float)percentage / 100f);
        }
        # endregion
#if !SyncfusionFramework4_5
        public bool SearchText(string targetText)
        {
            return m_documentView.SearchNextText(targetText);
        }
#endif
    }

    public enum PageViewMode
    {
        Normal,
        OnePage,
        FitWidth,
    }

    public class DocumentLoadedEventArgs : EventArgs
    {
    }

    public class ZoomChangedEventArgs : EventArgs
    {
        private int m_oldZoomFactor;
        private int m_newZoomFactor;

        public int OldZoomFactor
        {
            get
            {
                return m_oldZoomFactor;
            }
        }

        public int NewZoomFactor
        {
            get
            {
                return m_newZoomFactor;
            }
        }

        public ZoomChangedEventArgs(int oldZF, int newZF)
        {
            m_oldZoomFactor = oldZF;
            m_newZoomFactor = newZF;
        }
    }

    public class PageChangedEventArgs : EventArgs
    {
        private int m_oldPageNumber;
        private int m_newPageNumber;

        public int OldPageNumber
        {
            get
            {
                return m_oldPageNumber;
            }
        }

        public int NewPageNumber
        {
            get
            {
                return m_newPageNumber;
            }
        }

        public PageChangedEventArgs(int oldPN, int newPN)
        {
            m_oldPageNumber = oldPN;
            m_newPageNumber = newPN;
        }
    }

    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        Func<object, bool> canExecute;
        Action<object> executeAction;
        bool canExecuteCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeAction">The execute action.</param>
        /// <param name="canExecute">The can execute.</param>
        public DelegateCommand(Action<object> executeAction,
                               Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        #region ICommand Members
        /// <summary>
        /// Defines the method that determines whether the command 
        /// can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            bool tempCanExecute = canExecute(parameter);

            if (canExecuteCache != tempCanExecute)
            {
                canExecuteCache = tempCanExecute;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }

            return canExecuteCache;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed, 
        /// this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion
    }
}
