#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.PdfViewer.Base;
using Syncfusion.PdfViewer.Windows;
using Syncfusion.Windows;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Diagnostics;
using Syncfusion.Pdf;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    /// <summary>
    /// PdfDocuemntView helps to view and print PDF files.
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PdfDocumentView), "icons.pagelayout.bmp")]
    public partial class PdfDocumentView : UserControl,
                            IPdfDocumentView
    {
        #region Events
        /// <summary>
        /// Delegate for KeyPressed event.
        /// </summary>
        public delegate void KeyPressedEventHandler(object sender, KeyPressEventArgs args);
        /// <summary>
        /// Delegate for NavigationButtonStateChanged event.
        /// </summary>
        public delegate void NavigationButtonStatesChangedEventHandler(object sender, EventArgs args);
        /// <summary>
        /// Occurs when state of the navigation button is changed.
        /// </summary>
        public event NavigationButtonStatesChangedEventHandler NavigationButtonStatesChanged;
        /// <summary>
        /// Delegate for the CurrentPageChanged event.
        /// </summary>
        public delegate void CurrentPageChangedEventHandler(object sender, EventArgs args);
        /// <summary>
        /// Occurs when current page is changed.
        /// </summary>
        public event CurrentPageChangedEventHandler CurrentPageChanged;
        /// <summary>
        /// Delegate for DocumentLoaded event.
        /// </summary>
        public delegate void DocumentLoadedEventHandler(object sender, EventArgs args);
        /// <summary>
        /// Delegate for HyperLinkClicked event.
        /// </summary>
        public delegate void HyperLinkClickedEventHandler(object sender, AnnotEventArgs args);
        /// <summary>
        /// Delegate for HyperLinkMouseOver event.
        /// </summary>
        public delegate void HyperLinkMouseoverEventHandler(object sender, AnnotEventArgs args);
        /// <summary>
        /// Delegate for ZoomChanged event.
        /// </summary>
        public delegate void ZoomChangedEventHandler(object sender, int zoomFactor);

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        public event KeyPressedEventHandler KeyPressed;
        /// <summary>
        /// Occurs when the Pdf document is loaded
        /// </summary>
        public event DocumentLoadedEventHandler DocumentLoaded;
        /// <summary>
        /// Occurs when hyperlink is clicked.
        /// </summary>
        public event HyperLinkClickedEventHandler HyperLinkClicked;
        /// <summary>
        /// Occurs when hyperlink is hovered.
        /// </summary>
        public event HyperLinkMouseoverEventHandler HyperlinkHover;
        /// <summary>
        /// Occurs when zoom value is changed.
        /// </summary>
        public event ZoomChangedEventHandler ZoomChanged;
        #endregion


        #region Members
        private bool IsVScrollBarChanged = false;
        bool IsDialogDisplayed = false;
        private List<PageText> m_textDictonary = new List<PageText>();
        private int m_currentPageMatchCount = 0;
        RectangleF[] matchTextRects;
        private int m_currentTextSearchPage = 0;
        private int m_matchFoundPage = -1;
        private int m_currentPageRendered = -1;
        private bool IsMatchFoundAtAnotherPage = false;
        private Dictionary<int, List<PageText>> m_pageTextsCollectionDict = new Dictionary<int, List<PageText>>();
        private Dictionary<RectangleF, PageAnnotation> m_txtLocationsMergedList = new Dictionary<RectangleF, PageAnnotation>();
        private int m_nextMatch = -1;
        private List<RectangleF> m_textMatchesList = new List<RectangleF>();
        private Dictionary<RectangleF, TextMatchRectangle> m_txtLocations = new Dictionary<RectangleF, TextMatchRectangle>();
        private Dictionary<object, int> m_pageKidsCollection = new Dictionary<object, int>();
        private List<PageAnnotation> m_pageAnnotList = new List<PageAnnotation>();
        private List<PageAnnotation> m_currentPageAnnotsList = new List<PageAnnotation>();
        private Dictionary<RectangleF, string> m_currentPageDest = new Dictionary<RectangleF, string>();
        private Dictionary<RectangleF, PdfArray> m_PageAnnotDest = new Dictionary<RectangleF, PdfArray>();
        private Dictionary<RectangleF, float> m_currentPageAnnotBorder = new Dictionary<RectangleF, float>();
        internal Dictionary<RectangleF, string> currentPageAnnotations = new Dictionary<RectangleF, string>();
        private InheritedLabel m_lblAnnotToolTip = new InheritedLabel();
        private PointF m_currentMousePoint = new PointF();
        TableLayoutPanel tableLayoutPanel;
        Page page;
        const int c_gapBetweenPages = 8;
        VScrollBar Vscrol = new VScrollBar();
        HScrollBar Hscrol = new HScrollBar();
        int m_currentPageIndex = 0;
        double m_liveHeight = 0;
        int m_currentScrollValue = 0;
        internal bool? ScrollDown = null;
        internal bool? ScrollRight = null;
        int pageCount = 0;
        int m_oldVValue = 0;
        int m_newVValue = 0;
        int m_oldHValue = 0;
        int m_newHValue = 0;
        ScrollEventType m_scrollType;
        double m_difference;
        Dictionary<int, Page> m_pageCollection;
        Dictionary<int, double> m_pageLocation;
        Dictionary<int, double> m_pageLocationUnaltered;
        VirtualizingPagePanel m_pagePanel;

        PdfLoadedDocument m_loadedDocument;
        ZoomMode m_zoomMode;
        float m_zoomFactor = 1;
        PrintDocument m_printDocument;
        PdfUnitConvertor m_unitConvertor = new PdfUnitConvertor();
        internal int m_currentPageOnPrint = 0;
        private bool m_canGoToFirstPage;
        private bool m_canGoToPreviousPage;
        private bool m_canGoToNextPage;
        private bool m_canGoToLastPage;
        private int m_pageCount;
        internal float m_printWidth;
        internal float m_printHeight;
        internal int m_printFromPage;
        internal int m_printToPage;
        DeviceCMYK m_cmyk = new DeviceCMYK();
        private NotificationBar notify;
        internal PdfViewerExceptions exceptions = new PdfViewerExceptions();
        private PdfViewerControl m_pdfViewerControl;
        InheritedPanel panelForDrawingPanel = new InheritedPanel();
        private SearchBox m_searchBox;
        private int m_ScrollChangeValue;
        private Dictionary<int, Bitmap> ImageDictionary = new Dictionary<int, Bitmap>();
        private int PreviousPosition = 0;
        private bool IsMousePressed = false;
        private bool IsValueChanging = false;
        private Image BlankImage;
        float m_PreviousZoom;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes PdfDocumentView class.
        /// </summary>
        public PdfDocumentView()
        {
            InitializeComponent();
            tableLayoutPanel = new TableLayoutPanel();
            m_pageCollection = new Dictionary<int, Page>();
            m_pageLocation = new Dictionary<int, double>();
            this.BackColor = Color.FromArgb(204, 204, 204);
            this.Scroll += new ScrollEventHandler(PdfDocumentView_Scroll);
            this.AutoScroll = false;

            if (!this.Controls.Contains(DrawingPanel))
            {
                DrawingPanel = new InheritedPanel();
            }
            #region TableLayoutPanel
            // tableLayoutPanel1
            tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 97F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 97F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel.TabIndex = 0;

            this.Controls.Add(tableLayoutPanel);

            this.tableLayoutPanel.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutPanel1_CellPaint);

            panelForDrawingPanel.Controls.Add(DrawingPanel);
            panelForDrawingPanel.Dock = DockStyle.Fill;

            tableLayoutPanel.Controls.Add(panelForDrawingPanel, 0, 0);
            tableLayoutPanel.Controls.Add(Vscrol, 1, 0);
            tableLayoutPanel.Controls.Add(Hscrol, 0, 1);

            #endregion
            DocumentLoaded += new DocumentLoadedEventHandler(PdfDocumentView_DocumentLoaded);
            #region Annotations
            DrawingPanel.MouseMove += new MouseEventHandler(DrawingPanel_MouseMove);
            DrawingPanel.MouseDown += new MouseEventHandler(DrawingPanel_MouseDown);
            #endregion

            m_lblAnnotToolTip.Visible = false;
            m_lblAnnotToolTip.BorderStyle = BorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.KeyPressed += new KeyPressedEventHandler(PdfDocumentView_KeyPressed);
        }

        void PdfDocumentView_KeyPressed(object sender, KeyPressEventArgs args)
        {
            if (this.Parent != null)
            {
                TableLayoutPanel panel = this.Parent as TableLayoutPanel;
                if (panel != null)
                {
                    m_pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    if (m_pdfViewerControl != null)
                    {
                        m_pdfViewerControl.OnKeyPressed(args);
                    }
                }
            }
        }
        void PdfDocumentView_HyperLinkClicked(object sender, AnnotEventArgs args)
        {
            if (this.Parent != null)
            {
                TableLayoutPanel panel = this.Parent as TableLayoutPanel;
                if (panel != null)
                {

                    PdfViewerControl pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    if (pdfViewerControl != null && pdfViewerControl.IslinkCliked == true)

                        m_pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    if (m_pdfViewerControl != null)
                    {

                        pdfViewerControl.OnClicked(args);
                    }
                }
            }
        }
        void PdfDocumentView_DocumentLoaded(object sender, EventArgs args)
        {
            if (this.Parent != null)
            {
                TableLayoutPanel panel = this.Parent as TableLayoutPanel;
                if (panel != null)
                {
                    m_pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    if (m_pdfViewerControl != null)
                    {
                        m_pdfViewerControl.OnLoaded(args);
                    }
                }
            }
        }
        void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            string destURL = string.Empty;
            PointF currentPoint = m_unitConvertor.ConvertFromPixels(e.Location, PdfGraphicsUnit.Point);
            foreach (KeyValuePair<RectangleF, string> item in m_currentPageDest)
            {
                if (item.Key.Contains(currentPoint))
                {
                    destURL = item.Value;
                    if (m_PageAnnotDest.ContainsKey(item.Key))
                    {
                        PdfArray destArray = m_PageAnnotDest[item.Key];
                        if (destArray.Count > 0)
                        {
                            PdfReferenceHolder destPageRef = destArray[0] as PdfReferenceHolder;
                            object pageRef = destPageRef.Reference;
                            int destPage = -1;
                            if (m_pageKidsCollection.ContainsKey(pageRef))
                            {
                                destPage = m_pageKidsCollection[pageRef];
                                if (m_pageCollection.ContainsKey(destPage))
                                {
                                    float destPageHeight = m_pageCollection[destPage].Height;
                                    float destAnnotLoc = (((m_PageAnnotDest[item.Key])[3]) as PdfNumber).FloatValue;
                                    float tempDest = m_unitConvertor.ConvertFromPixels((destPageHeight), PdfGraphicsUnit.Point);
                                    float dest = tempDest - destAnnotLoc;
                                    GoToPageAtIndex(destPage + 1);
                                    m_newVValue = (int)(dest * m_zoomFactor) + Vscrol.Value;
                                    UpdateByScroll();
                                }
                                else
                                {
                                    GoToPageAtIndex(destPage + 1);
                                    float destPageHeight = m_pageCollection[destPage].Height;
                                    float destAnnotLoc = (((m_PageAnnotDest[item.Key])[3]) as PdfNumber).FloatValue;
                                    float tempDest = m_unitConvertor.ConvertFromPixels((destPageHeight), PdfGraphicsUnit.Point);
                                    float dest = tempDest - destAnnotLoc;
                                    m_newVValue = (int)(dest * m_zoomFactor) + Vscrol.Value;
                                    UpdateByScroll();
                                }
                            }
                        }
                    }
                    break;
                }
            }


            if (!string.IsNullOrEmpty(destURL))
            {
                TableLayoutPanel panel = this.Parent as TableLayoutPanel;
                if (panel != null)
                {
                    PdfViewerControl pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    OpenURL(pdfViewerControl, destURL, e.Location);
                }
                else
                {
                    if (this.HyperLinkClicked != null)
                    {
                        Process.Start(destURL);
                    }
                }
            }
        }

        void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            bool defaultCursor = true;
            string toolTipURL = string.Empty;
            m_currentMousePoint = m_unitConvertor.ConvertFromPixels(e.Location, PdfGraphicsUnit.Point);
            foreach (KeyValuePair<RectangleF, string> item in m_currentPageDest)
            {
                if (item.Key.Contains(m_currentMousePoint))
                {
                    defaultCursor = false;
                    toolTipURL = item.Value;
                    break;
                }
            }
            if (defaultCursor == false)
            {
                if (!String.IsNullOrEmpty(toolTipURL))
                {
                    ShowToolTip(e.Location, toolTipURL);
                }
                DrawingPanel.Cursor = Cursors.Hand;
                TableLayoutPanel panel = this.Parent as TableLayoutPanel;
                if (panel != null)
                {
                    PdfViewerControl pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    if (pdfViewerControl != null && pdfViewerControl.IslinkCliked == true)
                    {
                        AnnotEventArgs arg = new AnnotEventArgs(toolTipURL, e.Location);
                        this.HyperlinkHover += new HyperLinkMouseoverEventHandler(PdfDocumentView_HyperlinkHover);
                        HyperlinkHover(this, arg);
                        this.HyperlinkHover = null;
                    }
                }
            }
            else
            {
                m_lblAnnotToolTip.Visible = false;
                DrawingPanel.Cursor = Cursors.Default;
            }
        }

        void PdfDocumentView_HyperlinkHover(object sender, AnnotEventArgs args)
        {
            if (this.Parent != null)
            {
                TableLayoutPanel panel = this.Parent as TableLayoutPanel;
                if (panel != null)
                {
                    PdfViewerControl pdfViewerControl = this.Parent.Parent as PdfViewerControl;
                    if (pdfViewerControl != null && pdfViewerControl.IslinkCliked == true)
                    {
                        pdfViewerControl.OnHover(args);
                    }
                }
            }
        }

        /// <summary>
        /// Opens the weblink in browser
        /// </summary>
        private void OpenURL(PdfViewerControl pdfViewerControl, string destURL, Point location)
        {
            if (pdfViewerControl != null && pdfViewerControl.IslinkCliked == true)
            {
                AnnotEventArgs arg = new AnnotEventArgs(destURL, location);
                this.HyperLinkClicked += new HyperLinkClickedEventHandler(PdfDocumentView_HyperLinkClicked);
                HyperLinkClicked(this, arg);
                this.HyperLinkClicked = null;
            }
            else
            {
                Process.Start(destURL);
            }
        }

        /// <summary>
        /// Shows tooltip while the mouse is over an URL
        /// </summary>
        private void ShowToolTip(Point location, string toolTipURL)
        {
            m_lblAnnotToolTip.Location = new Point(location.X, location.Y + DrawingPanel.Cursor.Size.Height);
            DrawingPanel.Controls.Add(m_lblAnnotToolTip);
            m_lblAnnotToolTip.Visible = true;
            m_lblAnnotToolTip.AutoSize = true;
            m_lblAnnotToolTip.BackColor = Color.WhiteSmoke;
            m_lblAnnotToolTip.Text = toolTipURL;
        }
        #endregion

        #region Properties
        internal PdfLoadedDocument LoadedDocument
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
                return m_pagePanel.Pages.ToArray();
            }
        }
        internal InheritedPanel DrawingPanel;
        /// <summary>
        /// Gets the print document
        /// </summary>
        [Browsable(false)]
        public PrintDocument PrintDocument
        {
            get
            {
                if (m_printFromPage == 0)
                {
                    m_currentPageOnPrint = 0;
                    m_printFromPage = 1;
                }
                else
                    m_currentPageOnPrint = m_printFromPage - 1;
                if (m_printDocument == null && m_pagePanel != null)
                {
                    m_printDocument = new PrintDocument();
                    m_printDocument.PrintPage += new PrintPageEventHandler(OnPrintPage);
                }
                return m_printDocument;
            }
        }
        internal int AltPageCount
        {
            get
            {
                return m_pageCount;
            }
            set
            {
                m_pageCount = value;
            }
        }
        /// <summary>
        /// Gets or sets the displacement value for scrolling.
        /// </summary>
        public int ScrollDisplacementValue
        {
            get
            {
                return m_ScrollChangeValue;
            }
            set
            {
                m_ScrollChangeValue = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        public void Load(string filePath)
        {
            exceptions.Exceptions.Length = 0;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Cannot able to locate the specified file");
            }
            m_loadedDocument = DocumentLoader.Instance.Load(filePath);
            LoadPages();
            OnLoaded(EventArgs.Empty);
        }
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        /// <param name="password">The password for opening the document.</param>
        public void Load(string filePath, string password)
        {
            exceptions.Exceptions.Length = 0;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Cannot able to locate the specified file");
            }
            m_loadedDocument = DocumentLoader.Instance.Load(filePath, password);
            LoadPages();
            OnLoaded(EventArgs.Empty);
        }
        /// <summary>
        /// Loads a Pdf document  in the Pdf viewer from the specified stream.
        /// </summary>
        /// <param name="stream">A stream that contains the data for the Pdf document</param>
        public void Load(Stream stream)
        {
            exceptions.Exceptions.Length = 0;
            m_loadedDocument = DocumentLoader.Instance.Load(stream);
            LoadPages();
            OnLoaded(EventArgs.Empty);
        }
        /// <summary>
        /// Loads a pdf document in the Pdf viewer from the specified PdfLoadedDocuemnt.
        /// </summary>
        /// <param name="loadedDocument">The PdfLoadedDocument to be viewed in the PdfViewer</param>
        public void Load(PdfLoadedDocument loadedDocument)
        {
            exceptions.Exceptions.Length = 0;
            if (loadedDocument == null)
            {
                throw new ArgumentNullException("Loaded document should not be null");
            }

            m_loadedDocument = loadedDocument;
            LoadPages();
            OnLoaded(EventArgs.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnLoaded(EventArgs eventArgs)
        {
            //Set initial zoom.
            SetZoom(m_zoomMode);
            m_printFromPage = 0;
            m_printToPage = PageCount;
            if (DocumentLoaded != null)
                DocumentLoaded(this, eventArgs);
        }

        void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Column == 1 && e.Row == 1)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), e.CellBounds);
            }
        }

        /// <summary>
        /// Unloads the Pdf document
        /// </summary>
        public void Unload()
        {
            if (m_loadedDocument != null)
                m_loadedDocument.Close();
            m_currentPageIndex = 1;
            Clear();
        }

        internal void Clear()
        {
            foreach (KeyValuePair<int, Bitmap> item in ImageDictionary)
                item.Value.Dispose();
            ImageDictionary.Clear();
            if (BlankImage != null)
                BlankImage.Dispose();
            
            m_currentPageAnnotBorder.Clear();
            m_currentPageAnnotsList.Clear();
            m_currentPageDest.Clear();
            m_PageAnnotDest.Clear();
            m_pageAnnotList.Clear();

            foreach (KeyValuePair<int, Page> item in m_pageCollection)
                item.Value.Clear();

            m_pageCollection.Clear();
            m_pageKidsCollection.Clear();
            m_pageLocation.Clear();
            if (m_pageLocationUnaltered != null)
                m_pageLocationUnaltered.Clear();
            if (m_pagePanel != null)
                m_pagePanel.Dispose();
            m_pageTextsCollectionDict.Clear();
            m_textDictonary.Clear();
            m_textMatchesList.Clear();
            m_txtLocations.Clear();
            m_txtLocationsMergedList.Clear();
            if (panelForDrawingPanel.Controls.Count > 0)
                panelForDrawingPanel.Controls.RemoveAt(0);
            DrawingPanel = new InheritedPanel();
            panelForDrawingPanel.Controls.Add(DrawingPanel);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            Unload();
            Dispose(true);
        }

        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <returns>Returns the specified page as Image</returns>
        public Bitmap ExportAsImage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= Pages.Length)
            {
                notify = new NotificationBar();
            }
            Page currentPage = Pages[pageIndex];
            Bitmap bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
            using (Graphics graphics = Graphics.FromImage(bitmapImage))
            {
                graphics.TranslateTransform(0, 0);
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, currentPage.Bounds.Width, currentPage.Bounds.Height));
                if (currentPage.RecordCollection == null)
                    currentPage.Initialize(m_loadedDocument.Pages[pageIndex], true);
                ImageRenderer renderer = new ImageRenderer(currentPage.RecordCollection, currentPage.Resources, graphics, true, m_cmyk);
                CultureInfo current = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                renderer.RenderAsImage();
                Thread.CurrentThread.CurrentCulture = current;
            }
            return bitmapImage;
        }

        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="keepAspectRatio">Whether need to keep the aspect ratio of the page</param>
        /// <returns>Returns the image with custom size</returns>
        public Bitmap ExportAsImage(int pageIndex, SizeF customSize, bool keepAspectRatio)
        {
            if (pageIndex < 0 || pageIndex >= Pages.Length)
            {
                notify = new NotificationBar();
            }
            Page currentPage = Pages[pageIndex];
            float scaleX = 1, scaleY = 1;
            int sizeX = currentPage.Bounds.Height, sizeY = currentPage.Bounds.Width;
            Bitmap bitmapImage = null;
            if (!keepAspectRatio)
            {
                if (currentPage.Width < customSize.Width && currentPage.Height < customSize.Height)
                {
                    scaleX = customSize.Width / currentPage.Width;
                    scaleY = customSize.Height / currentPage.Height;
                    sizeX = (int)(currentPage.Bounds.Width * scaleX);
                    sizeY = (int)(currentPage.Bounds.Height * scaleY);
                    bitmapImage = new Bitmap(sizeX, sizeY);
                }
                else
                {
                    bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                    sizeX = currentPage.Bounds.Height;
                    sizeY = currentPage.Bounds.Width;
                }
            }
            else
            {
                #region ExpandingImage
                if (currentPage.Width < customSize.Width && currentPage.Height < customSize.Height)
                {
                    if (customSize.Width > customSize.Height)
                    {
                        scaleX = customSize.Width / currentPage.Width;
                        scaleY = customSize.Height / currentPage.Height;
                        sizeX = (int)(currentPage.Bounds.Width * scaleX);
                        sizeY = (int)(currentPage.Bounds.Height * scaleY);
                        float incrementY = (sizeX - sizeY) + (currentPage.Bounds.Height - currentPage.Bounds.Width);
                        scaleY = (sizeY + incrementY) / currentPage.Height;
                        bitmapImage = new Bitmap(sizeX, (int)(sizeY + incrementY));
                        sizeY = (int)(sizeY + incrementY);
                    }
                    else if (customSize.Width < customSize.Height)
                    {
                        scaleX = customSize.Width / currentPage.Width;
                        scaleY = customSize.Height / currentPage.Height;
                        sizeX = (int)(currentPage.Bounds.Width * scaleX);
                        sizeY = (int)(currentPage.Bounds.Height * scaleY);
                        float incrementX = (sizeY - sizeX) + (currentPage.Bounds.Width - currentPage.Bounds.Height);
                        scaleX = (sizeX + incrementX) / currentPage.Width;
                        bitmapImage = new Bitmap((int)(sizeX + incrementX), sizeY);
                        sizeX = (int)(sizeX + incrementX);
                    }
                    else if (customSize.Width == customSize.Height)
                    {
                        float aspectRatio = (float)currentPage.Height / (float)currentPage.Width;
                        sizeX = (int)(customSize.Width);
                        sizeY = (int)(customSize.Height * aspectRatio);
                        scaleX = (float)sizeX / (float)currentPage.Width;
                        scaleY = (float)sizeY / (float)currentPage.Height;
                        bitmapImage = new Bitmap((int)(sizeX), (int)(sizeY));
                    }
                }
                else
                {
                    bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                    sizeX = currentPage.Bounds.Height;
                    sizeY = currentPage.Bounds.Width;
                }
                #endregion
            }

            using (Graphics graphics = Graphics.FromImage(bitmapImage))
            {
                graphics.ScaleTransform(scaleX, scaleY);
                graphics.TranslateTransform(0, 0);
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, sizeY, sizeX));
                if (currentPage.RecordCollection == null)
                    currentPage.Initialize(m_loadedDocument.Pages[pageIndex], true);
                ImageRenderer renderer = new ImageRenderer(currentPage.RecordCollection, currentPage.Resources, graphics, true, m_cmyk);
                CultureInfo current = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                renderer.RenderAsImage();
                Thread.CurrentThread.CurrentCulture = current;
            }
            if (currentPage.Width > currentPage.Height)
                bitmapImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            return bitmapImage;
        }

        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="dpiX">The horizondal resolution of the image</param>
        /// <param name="dpiY">The vertical resolution of the image</param>
        /// <param name="keepAspectRatio">Whether need to keep the aspect ratio of the page</param>
        /// <returns>Returns the specified page as image with custom size and resolution</returns>>
        public Bitmap ExportAsImage(int pageIndex, SizeF customSize, float dpiX, float dpiY, bool keepAspectRatio)
        {
            Bitmap bitmapImage = ExportAsImage(pageIndex, customSize, keepAspectRatio);
            bitmapImage.SetResolution(dpiX, dpiY);
            return bitmapImage;
        }

        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Returns the specified pages as Images</returns>
        public Bitmap[] ExportAsImage(int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= Pages.Length)
                throw new ArgumentOutOfRangeException("Starting Index should be greater than Zero and less than total number of pages; ending index should be less than total number of pages");

            if (startIndex > endIndex)
                throw new ArgumentException("Starting index should be less than ending index");

            int count = endIndex - startIndex + 1;
            Bitmap[] images = new Bitmap[count];

            for (int i = startIndex, j = 0; i <= endIndex; i++, j++)
            {
                Page currentPage = Pages[i];
                images[j] = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                using (Graphics graphics = Graphics.FromImage(images[j]))
                {
                    graphics.TranslateTransform(0, 0);
                    graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, currentPage.Bounds.Width, currentPage.Bounds.Height));
                    if (currentPage.RecordCollection == null)
                        currentPage.Initialize(m_loadedDocument.Pages[i], true);
                    ImageRenderer renderer = new ImageRenderer(currentPage.RecordCollection, currentPage.Resources, graphics, true, m_cmyk);
                    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    renderer.RenderAsImage();
                    Thread.CurrentThread.CurrentCulture = current;
                    if (currentPage.Resources.ContainsKey(DictionaryProperties.Rotate))
                    {
                        float rotation = (float)currentPage.Resources[DictionaryProperties.Rotate];
                        if (rotation == 90)
                            images[j].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        else if (rotation == 180)
                            images[j].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        else if (rotation == 270)
                            images[j].RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                }
            }
            return images;
        }

        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="keepAspectRatio">Whether need to keep the aspect raio of the page</param>
        /// <returns>Returns the specified pages as images with custom size</returns>
        public Bitmap[] ExportAsImage(int startIndex, int endIndex, SizeF customSize, bool keepAspectRatio)
        {
            if (startIndex < 0 || endIndex >= Pages.Length)
                throw new ArgumentOutOfRangeException("Starting Index should be greater than Zero and less than total number of pages; ending index should be less than total number of pages");

            if (startIndex > endIndex)
                throw new ArgumentException("Starting index should be less than ending index");

            int count = endIndex - startIndex + 1;
            Bitmap[] images = new Bitmap[count];

            for (int i = startIndex, j = 0; i <= endIndex; i++, j++)
            {
                float pgeHeight = m_unitConvertor.ConvertFromPixels(Pages[i].Height, PdfGraphicsUnit.Point);
                float pgeWidth = m_unitConvertor.ConvertFromPixels(Pages[i].Width, PdfGraphicsUnit.Point);

                Page currentPage = Pages[i];

                float scaleX = 1, scaleY = 1;
                int sizeX = currentPage.Bounds.Height, sizeY = currentPage.Bounds.Width;
                Bitmap bitmapImage = null;
                if (!keepAspectRatio)
                {
                    if (currentPage.Width < customSize.Width && currentPage.Height < customSize.Height)
                    {
                        scaleX = customSize.Width / currentPage.Width;
                        scaleY = customSize.Height / currentPage.Height;
                        sizeX = (int)(currentPage.Bounds.Width * scaleX);
                        sizeY = (int)(currentPage.Bounds.Height * scaleY);
                        //bitmapImage = new Bitmap(sizeX, sizeY);
                        images[j] = new Bitmap((int)sizeX, (int)sizeY);
                    }
                    else
                    {
                        bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                        sizeX = currentPage.Bounds.Height;
                        sizeY = currentPage.Bounds.Width;
                    }
                }
                else
                {
                    #region ExpandingImage
                    if (currentPage.Width < customSize.Width && currentPage.Height < customSize.Height)
                    {
                        if (customSize.Width > customSize.Height)
                        {
                            scaleX = customSize.Width / currentPage.Width;
                            scaleY = customSize.Height / currentPage.Height;
                            sizeX = (int)(currentPage.Bounds.Width * scaleX);
                            sizeY = (int)(currentPage.Bounds.Height * scaleY);
                            float incrementY = (sizeX - sizeY) + (currentPage.Bounds.Height - currentPage.Bounds.Width);
                            scaleY = (sizeY + incrementY) / currentPage.Height;
                            //bitmapImage = new Bitmap(sizeX, (int)(sizeY + incrementY));
                            images[j] = new Bitmap((int)sizeX, (int)(sizeY + incrementY));
                            sizeY = (int)(sizeY + incrementY);
                        }
                        else if (customSize.Width < customSize.Height)
                        {
                            scaleX = customSize.Width / currentPage.Width;
                            scaleY = customSize.Height / currentPage.Height;
                            sizeX = (int)(currentPage.Bounds.Width * scaleX);
                            sizeY = (int)(currentPage.Bounds.Height * scaleY);
                            float incrementX = (sizeY - sizeX) + (currentPage.Bounds.Width - currentPage.Bounds.Height);
                            scaleX = (sizeX + incrementX) / currentPage.Width;
                            //bitmapImage = new Bitmap((int)(sizeX + incrementX), sizeY);
                            images[j] = new Bitmap((int)(sizeX + incrementX), sizeY);
                            sizeX = (int)(sizeX + incrementX);
                        }
                        else if (customSize.Width == customSize.Height)
                        {
                            float aspectRatio = (float)currentPage.Height / (float)currentPage.Width;
                            sizeX = (int)(customSize.Width);
                            sizeY = (int)(customSize.Height * aspectRatio);
                            scaleX = (float)sizeX / (float)currentPage.Width;
                            scaleY = (float)sizeY / (float)currentPage.Height;
                            images[j] = new Bitmap((int)(sizeX), (int)(sizeY));
                        }
                    }
                    else
                    {
                        //bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                        images[j] = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                        sizeX = currentPage.Bounds.Height;
                        sizeY = currentPage.Bounds.Width;
                    }
                    #endregion
                }

                using (Graphics graphics = Graphics.FromImage(images[j]))
                {
                    graphics.ScaleTransform(scaleX, scaleY);
                    graphics.TranslateTransform(0, 0);
                    graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, sizeY, sizeX));
                    if (currentPage.RecordCollection == null)
                        currentPage.Initialize(m_loadedDocument.Pages[i], true);
                    ImageRenderer renderer = new ImageRenderer(currentPage.RecordCollection, currentPage.Resources, graphics, true, m_cmyk);
                    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    renderer.RenderAsImage();
                    Thread.CurrentThread.CurrentCulture = current;
                }
            }
            return images;
        }

        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="dpiX">The horizondal resolution of the image</param>
        /// <param name="dpiY">The Vertical resolution of the image</param>
        /// <param name="keepAspectRatio">Whether need to keep the aspect ratio of the page</param>
        /// <returns>Returns the specified pages as images with custom size and resolution</returns>
        public Bitmap[] ExportAsImage(int startIndex, int endIndex, SizeF customSize, float dpiX, float dpiY, bool keepAspectRatio)
        {
            Bitmap[] images = ExportAsImage(startIndex, endIndex, customSize, keepAspectRatio);
            foreach (Bitmap bmp in images)
            {
                bmp.SetResolution(dpiX, dpiY);
            }
            return images;
        }

        /// <summary>
        /// Exports the specified page as Metafile
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <returns>Metafile</returns>
        public Metafile ExportAsMetafile(int pageIndex)
        {
            Page currentPage = Pages[pageIndex];
            Metafile metafile;
            Bitmap bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
            Graphics g = Graphics.FromImage(bitmapImage);
            IntPtr hdc = g.GetHdc();
            using (MemoryStream stream = new MemoryStream())
            {
                int height = (int)m_unitConvertor.ConvertFromPixels(currentPage.Height, PdfGraphicsUnit.Point);

                metafile = new Metafile(stream, hdc, EmfType.EmfOnly);
                g.ReleaseHdc(hdc);
                using (Graphics graphics = Graphics.FromImage(metafile))
                {
                    graphics.TranslateTransform(0, 0);
                    if (currentPage.Width > currentPage.Height)
                        graphics.RotateTransform(-90);
                    if (currentPage.Resources != null)
                    {
                        if (currentPage.Resources.ContainsKey(DictionaryProperties.Rotate))
                        {
                            if ((float)currentPage.Resources[DictionaryProperties.Rotate] == 180 || (float)currentPage.Resources[DictionaryProperties.Rotate] == 270)
                                graphics.RotateTransform(180);
                        }
                    }
                    if (currentPage.Width > 300 && currentPage.Height > 100)
                    {
                        graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, currentPage.Bounds.Width, currentPage.Bounds.Height));
                    }

                    if (currentPage.RecordCollection == null)
                        currentPage.Initialize(m_loadedDocument.Pages[pageIndex], true);
                    ImageRenderer renderer = new ImageRenderer(currentPage.RecordCollection, currentPage.Resources, graphics, true, height, m_cmyk);
                    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    renderer.RenderAsImage();
                    Thread.CurrentThread.CurrentCulture = current;
                }
            }
            return metafile;
        }

        /// <summary>
        /// Exports the specified pages as Metafile
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Array of Metafile</returns>
        public Metafile[] ExportAsMetafile(int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= Pages.Length)
                throw new ArgumentOutOfRangeException("Starting Index should be greater than Zero and less than total number of pages; ending index should be less than total number of pages");

            if (startIndex > endIndex)
                throw new ArgumentException("Starting index should be less than ending index");

            int count = endIndex - startIndex + 1;
            MemoryStream[] stream = new MemoryStream[count];
            Metafile[] metafiles = new Metafile[count];
            for (int i = startIndex, j = 0; i <= endIndex; i++, j++)
            {
                Page currentPage = Pages[i];
                Bitmap bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                Graphics g = Graphics.FromImage(bitmapImage);
                IntPtr hdc = g.GetHdc();

                int height = (int)m_unitConvertor.ConvertFromPixels(currentPage.Height, PdfGraphicsUnit.Point);
                stream[j] = new MemoryStream();
                metafiles[j] = new Metafile(stream[j], hdc, EmfType.EmfPlusDual);
                g.ReleaseHdc(hdc);
                using (Graphics graphics = Graphics.FromImage(metafiles[j]))
                {
                    graphics.TranslateTransform(0, 0);
                    graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, currentPage.Bounds.Width, currentPage.Bounds.Height));

                    if (currentPage.RecordCollection == null)
                        currentPage.Initialize(m_loadedDocument.Pages[i], true);
                    ImageRenderer renderer = new ImageRenderer(currentPage.RecordCollection, currentPage.Resources, graphics, true, height, m_cmyk);
                    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    renderer.RenderAsImage();
                    Thread.CurrentThread.CurrentCulture = current;
                }
            }
            return metafiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (panelForDrawingPanel.Controls.Count == 2)
            {
                SearchBox currentSearchBox = panelForDrawingPanel.Controls[0] as SearchBox;
                if (currentSearchBox.Visible)
                {
                    currentSearchBox.Invalidate();
                }
            }
            if (DrawingPanel != null && m_pagePanel != null && m_pagePanel.Pages.Count > 0)
            {
                this.AutoScroll = true;
                GoToPageAtIndex(m_currentPageIndex + 1);
                Vscrol.Invalidate();
                DrawingPanel.Invalidate();
                UpdatePanelOnZoom();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.HorizontalScroll;
                        if (Hscrol.Value + 20 <= Hscrol.Maximum)
                        {
                            m_oldHValue = Hscrol.Value;
                            m_newHValue = Hscrol.Value + 20;
                        }
                        else
                            m_newHValue = Hscrol.Maximum;

                        Hscrol.Value = m_newHValue;
                        Hscrol.Value = m_newHValue;
                        return true;
                    }
                    break;
                case Keys.Left:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.HorizontalScroll;
                        if (Hscrol.Value - 20 >= Hscrol.Minimum)
                        {
                            m_oldHValue = Hscrol.Value;
                            m_newHValue = Hscrol.Value - 20;
                        }
                        else
                            m_newHValue = Hscrol.Minimum;

                        Hscrol.Value = m_newHValue;
                        Hscrol.Value = m_newHValue;
                        return true;
                    }
                    break;
                case Keys.Up:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
                        if (Vscrol.Value - 20 >= Vscrol.Minimum)
                            m_newVValue = Vscrol.Value - 20;
                        else
                            m_newVValue = 0;

                        Vscrol.Value = m_newVValue;
                        Vscrol.Value = m_newVValue;
                        UpdateByScroll();
                        return true;
                    }
                    break;
                case Keys.Down:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
                        if (Vscrol.Value + 20 <= Vscrol.Maximum)
                            m_newVValue = Vscrol.Value + 20;
                        else
                            m_newVValue = Vscrol.Maximum;

                        Vscrol.Value = m_newVValue;
                        Vscrol.Value = m_newVValue;
                        UpdateByScroll();
                        return true;
                    }
                    break;
                case Keys.PageUp:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
                        if (CanGoToPreviousPage)
                            GoToPreviousPage();
                        return true;
                    }
                    break;
                case Keys.PageDown:
                    {
                        if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                        {
                            DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
                            if (CanGoToNextPage)
                                GoToNextPage();
                            return true;
                        }
                    }
                    break;
                case Keys.Home:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
                        if (CanGoToFirstPage)
                            GoToFirstPage();
                        return true;
                    }
                    break;
                case Keys.End:
                    if (m_searchBox == null || !m_searchBox.searchInputTxtBox.Focused)
                    {
                        DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
                        if (CanGoToLastPage)
                            GoToLastPage();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.F:
                    if (m_pdfViewerControl != null)
                    {
                        SearchBox searchBox = new SearchBox();
                        AddSearchBox(searchBox);
                    }
                    break;
                case Keys.Enter:
                    if (m_searchBox != null)
                    {
                        if (m_searchBox.searchInputTxtBox.Focused)
                        {
                            searchNextBtn_Click(null, EventArgs.Empty);
                        }
                    }
                    break;
            }
            KeyPressEventArgs args = new KeyPressEventArgs(msg, keyData);
            if (KeyPressed != null)
            {
                if (m_searchBox != null && m_searchBox.Visible == true)
                {
                    return false;
                }
                else
                {
                    KeyPressed(this, args);
                }
            }
            return false;
        }

        internal void AddSearchBox(SearchBox searchBox)
        {
            if (panelForDrawingPanel.Controls.Count == 2)
            {
                m_searchBox = panelForDrawingPanel.Controls[0] as SearchBox;
                if (m_searchBox.Visible == false)
                    m_searchBox.Visible = true;
                m_searchBox.Invalidate();
                m_searchBox.Focus();
                return;
            }
            m_searchBox = searchBox;
            m_searchBox.searchNextBtn.Click += new EventHandler(searchNextBtn_Click);
            m_searchBox.searchPreviousBtn.Click += new EventHandler(searchPreviousBtn_Click);
            m_searchBox.searchInputTxtBox.TextChanged += new EventHandler(searchInputTxtBox_TextChanged);
            m_searchBox.searchInputTxtBox.GotFocus += new EventHandler(searchInputTxtBox_GotFocus);
            m_searchBox.Dock = DockStyle.Right;

            panelForDrawingPanel.Controls.Add(m_searchBox);
            panelForDrawingPanel.Controls.SetChildIndex(m_searchBox, 0);
            panelForDrawingPanel.Controls.SetChildIndex(DrawingPanel, 1);
            m_searchBox.Focus();
        }
        void searchInputTxtBox_GotFocus(object sender, EventArgs e)
        {
            m_nextMatch = -1;
            m_currentTextSearchPage = 0;
        }
        void searchInputTxtBox_TextChanged(object sender, EventArgs e)
        {
            IsDialogDisplayed = false;
            m_nextMatch = -1;
        }

        void searchPreviousBtn_Click(object sender, EventArgs e)
        {
            IsVScrollBarChanged = false;
            if (m_nextMatch > m_currentPageMatchCount - 1)
            {
                m_nextMatch = m_currentPageMatchCount;
            }
            if (m_nextMatch >= 1)
            {
                m_nextMatch--;
                UpdateByScroll();
            }
            else
            {
                m_currentTextSearchPage--;
                if (m_currentTextSearchPage >= 0)
                {
                    GoToNextPageAndSearch(false);
                    m_nextMatch = m_currentPageMatchCount - 1;
                    if (m_matchFoundPage != -1 && m_nextMatch != -1)
                    {
                        GoToPageAtIndex(m_matchFoundPage);
                    }
                }
            }
        }

        void searchNextBtn_Click(object sender, EventArgs e)
        {
            IsVScrollBarChanged = false;
            IsMatchFoundAtAnotherPage = false;
            m_matchFoundPage = -1;
            if (m_currentTextSearchPage < 0)
            {
                m_currentTextSearchPage = 0;
                m_nextMatch++;
            }
            GoToNextPageAndSearch(true);

            if (m_nextMatch == m_currentPageMatchCount - 1 && m_currentTextSearchPage == m_pagePanel.Pages.Count - 1)
            {
                MessageBox.Show(
                        string.Format("Reader has finished searching the document. No matches were found"), "Essential Pdf Viewer",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (m_nextMatch == m_currentPageMatchCount - 1 && m_nextMatch != -1)
            {
                //currentSearchInitiatedAt = currentTextSearchPage;
                m_nextMatch = -1;
                m_currentTextSearchPage++;
                if (m_matchFoundPage < m_pagePanel.Pages.Count)
                {
                    GoToNextPageAndSearch(true);
                }
            }

            if (m_matchFoundPage != -1)
            {
                if (m_matchFoundPage <= m_pagePanel.Pages.Count)
                {
                    //GoToPageAtIndex(m_matchFoundPage);
                    GoToPageAtIndexForTextSearch(m_matchFoundPage);
                }
            }
            m_nextMatch++;
        }

        void UpdatePanelOnZoom()
        {
            float currentHeight = 0;
            pageCount = 0;
            m_pageLocation = new Dictionary<int, double>();
            if (m_pagePanel != null)
            {
                foreach (Page page in m_pagePanel.Pages)
                {
                    m_pageLocation.Add(pageCount, currentHeight);
                    currentHeight = (float)RotatedPageHeight(page, (int)currentHeight, true);
                    pageCount++;
                }
                Vscrol.Maximum = m_ScrollChangeValue + (int)(currentHeight - m_unitConvertor.ConvertFromPixels(DrawingPanel.Height, PdfGraphicsUnit.Point)) + c_gapBetweenPages;
                DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
            }
            if (m_pageCollection != null)
            {
                int left;
                Page newPage;
                List<int> absLeftList = new List<int>();
                if (m_pageCollection.Count > 0)
                {
                    foreach (KeyValuePair<int, Page> pageElement in m_pageCollection)
                    {
                        newPage = pageElement.Value;
                        left = RotatedPagePosition(newPage);
                        if (left < 0)
                        {
                            if (m_zoomMode == PdfViewer.ZoomMode.FitWidth)
                                this.tableLayoutPanel.RowStyles[1].Height = 0F;
                            else
                                this.tableLayoutPanel.RowStyles[1].Height = 18F;
                            if (Hscrol.Visible == false)
                                Hscrol.Visible = true;
                            int absLeft = 2 * Math.Abs(left);
                            absLeftList.Add(absLeft);
                        }
                    }
                }
                if (absLeftList.Count > 0)
                {
                    int HMax = absLeftList[0];
                    foreach (int leftElement in absLeftList)
                    {
                        if (HMax < leftElement)
                            HMax = leftElement;
                    }
                    if (Hscrol.Value > HMax)
                    {
                        m_newHValue = HMax;
                        m_oldHValue = HMax;
                    }
                    Hscrol.Maximum = HMax;
                    Hscrol.Minimum = 0;
                }
            }
        }
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="index">The page index</param>
        public void GoToPageAtIndexForTextSearch(int index)
        {
            index -= 1;
            int destination = 0;

            DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
            destination = (int)m_pageLocation[index] + c_gapBetweenPages + 1;
            if (destination > Vscrol.Maximum)
                destination = Vscrol.Maximum;
            if (destination < Vscrol.Minimum)
                destination = Vscrol.Minimum;

            Vscrol.Value = destination;
            Vscrol.Value = destination;

            m_newVValue = Vscrol.Value;
            UpdateByScroll();
            ResetNavigationButtonStates();
        }

        private void GoToNextPageAndSearch(bool IsNext)
        {
            m_currentPageMatchCount = 0;
            m_txtLocations.Clear();
            bool matchFound = false;

            while (IsMatchFoundAtAnotherPage == false)
            {
                Graphics grapix = this.CreateGraphics();
                if (m_currentTextSearchPage < m_pagePanel.Pages.Count && m_currentTextSearchPage >= 0)
                {
                    Page page = m_pagePanel.Pages[m_currentTextSearchPage];
                    if (m_cmyk == null)
                        m_cmyk = new DeviceCMYK();
                    if (page.RecordCollection == null)
                        page.Initialize(m_loadedDocument.Pages[m_currentTextSearchPage], true);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    ImageRenderer renderer = new ImageRenderer(page.RecordCollection, page.Resources, grapix, false, page.Height, page.CurrentLeftLocation, m_cmyk);
                    ImageRenderer.textDictonary.Clear();
                    renderer.RenderAsImage();
                    m_textDictonary = ImageRenderer.textDictonary;
                    Dictionary<RectangleF, TextMatchRectangle> mergeHelperDict = new Dictionary<RectangleF, TextMatchRectangle>();
                    foreach (PageText txt in ImageRenderer.textDictonary)
                    {
                        float scaleX = 0, scaleY = 0;
                        Matrix transformMatrix = txt.TransformPoints;
                        float[] transformPoints = transformMatrix.Elements;
                        PointF transformLocation = txt.CurrentLocation;
                        if (transformPoints[0] != 0 && transformPoints[3] != 0)
                        {
                            scaleX = transformPoints[0];
                            scaleY = transformPoints[3];
                        }
                        float locationX = (transformPoints[4] + transformLocation.X * scaleX);
                        float locationY = (transformPoints[5] + transformLocation.Y * scaleY);
                        float width = txt.TextElementWidth * scaleX;
                        float height = txt.FontSize * scaleX;
                        page.pagetxtList.Clear();

                        TextMatchRectangle tempAnnots = new TextMatchRectangle(new RectangleF(locationX, locationY, width, height), txt.Text, width, scaleX, txt.TextFont);
                        page.GetTextProperties(page, tempAnnots);
                        foreach (TextMatchRectangle annots in page.pagetxtList)
                        {
                            if (!mergeHelperDict.ContainsKey(annots.Rect))
                            {
                                mergeHelperDict.Add(annots.Rect, annots);
                            }
                        }
                    }
                    MergeTextWithYaxis(mergeHelperDict);
                    foreach (KeyValuePair<RectangleF, TextMatchRectangle> item in mergeHelperDict)
                    {
                        if (((item.Value.Text.IndexOf(m_searchBox.searchInputTxtBox.Text, StringComparison.InvariantCultureIgnoreCase)) >= 0) && !string.IsNullOrEmpty(m_searchBox.searchInputTxtBox.Text))
                        {
                            m_matchFoundPage = m_currentTextSearchPage + 1;

                            if (!m_txtLocations.ContainsKey(item.Key))
                            {
                                m_txtLocations.Add(item.Key, item.Value);
                                m_currentPageMatchCount++;
                            }
                            matchFound = true;
                        }
                    }
                    if (matchFound)
                    {
                        return;
                    }
                    else
                    {
                        if (IsNext)
                        {
                            m_currentTextSearchPage++;
                        }
                        else
                        {
                            m_currentTextSearchPage--;
                        }
                    }
                }
                else
                {
                    if (IsDialogDisplayed == false)
                    {
                        DialogResult dialogResult = MessageBox.Show(
                            string.Format("Reader has finished searching the document. No matches were found"), "Essential Pdf Viewer",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsDialogDisplayed = true;
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Returns the rectangle postions of the text matches
        /// </summary>        
        /// <param name="text">The text which is to be searched</param>
        /// <param name="matchTextPositionsDict"></param>
        /// <returns></returns>
        private bool FindTextMatches(String text, out Dictionary<int, List<RectangleF>> matchTextPositionsDict)
        {
            bool IsMatchFound = false;
            int currentTextSearchPage = 0;
            matchTextPositionsDict = new Dictionary<int, List<RectangleF>>();
            int count = 0;
            foreach (Page p in m_pagePanel.Pages)
                p.matchTextPositions.Clear();
            while (currentTextSearchPage < m_pagePanel.Pages.Count)
            {
                Graphics grapix = this.CreateGraphics();
                if (currentTextSearchPage < m_pagePanel.Pages.Count && currentTextSearchPage >= 0)
                {
                    Page page = m_pagePanel.Pages[currentTextSearchPage];
                    if (m_cmyk == null)
                        m_cmyk = new DeviceCMYK();
                    if (page.RecordCollection == null)
                        page.Initialize(m_loadedDocument.Pages[currentTextSearchPage], true);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    ImageRenderer renderer = new ImageRenderer(page.RecordCollection, page.Resources, grapix, false, page.Height, page.CurrentLeftLocation, m_cmyk);
                    ImageRenderer.textDictonary.Clear();
                    renderer.RenderAsImage();
                    m_textDictonary = ImageRenderer.textDictonary;
                    Dictionary<RectangleF, TextMatchRectangle> mergeHelperDict = new Dictionary<RectangleF, TextMatchRectangle>();
                    foreach (PageText txt in ImageRenderer.textDictonary)
                    {
                        float scaleX = 0, scaleY = 0;
                        Matrix transformMatrix = txt.TransformPoints;
                        float[] transformPoints = transformMatrix.Elements;
                        PointF transformLocation = txt.CurrentLocation;
                        if (transformPoints[0] != 0 && transformPoints[3] != 0)
                        {
                            scaleX = transformPoints[0];
                            scaleY = transformPoints[3];
                        }
                        float locationX = (transformPoints[4] + transformLocation.X * scaleX);
                        float locationY = (transformPoints[5] + transformLocation.Y * scaleY);
                        float width = txt.TextElementWidth * scaleX;
                        float height = txt.FontSize * scaleX;
                        page.pagetxtList.Clear();

                        TextMatchRectangle tempAnnots = new TextMatchRectangle(new RectangleF(locationX, locationY, width, height), txt.Text, width, scaleX, txt.TextFont);
                        page.GetTextProperties(page, tempAnnots);
                        foreach (TextMatchRectangle annots in page.pagetxtList)
                        {
                            if (!mergeHelperDict.ContainsKey(annots.Rect))
                            {
                                mergeHelperDict.Add(annots.Rect, annots);
                            }
                        }
                    }
                    MergeTextWithYaxis(mergeHelperDict);
                    foreach (KeyValuePair<RectangleF, TextMatchRectangle> item in mergeHelperDict)
                    {
                        if (((item.Value.Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase)) >= 0) && !string.IsNullOrEmpty(text))
                        {
                            m_matchFoundPage = currentTextSearchPage + 1;

                            if (!page.matchTextPositions.Contains(item.Key))
                            {
                                Graphics graphics = DrawingPanel.CreateGraphics();
                                graphics.PageUnit = GraphicsUnit.Point;
                                if (((item.Value.Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase)) >= 0) && !string.IsNullOrEmpty(text))
                                {
                                    float subTxtWidth = 0;
                                    int startCharLoc = item.Value.Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase);
                                    float charWidth = item.Value.TextWidth / item.Value.Text.Length;
                                    SizeF temp = graphics.MeasureString(" ", item.Value.TextFont);
                                    float scalingWidth = item.Value.ScaleX;
                                    string preSubString = item.Value.Text.Substring(0, startCharLoc);
                                    SizeF preStringSize = graphics.MeasureString(preSubString, item.Value.TextFont);
                                    float preTxtWidth = preStringSize.Width * scalingWidth;
                                    SizeF stringSize = graphics.MeasureString(text, item.Value.TextFont);
                                    float txtWidth = stringSize.Width * scalingWidth;
                                    float incrementalSpace = 0;
                                    if (preTxtWidth > 0)
                                    {
                                        subTxtWidth = text.Length;
                                    }
                                    float newRectX = item.Key.X + preTxtWidth - subTxtWidth;
                                    if (preSubString.EndsWith(" "))
                                    {
                                        incrementalSpace += temp.Width * scalingWidth;
                                        incrementalSpace = m_unitConvertor.ConvertToPixels(incrementalSpace, PdfGraphicsUnit.Point);
                                        newRectX += incrementalSpace;
                                        txtWidth += incrementalSpace;
                                    }
                                    RectangleF rect = new RectangleF(newRectX, item.Key.Y, txtWidth, item.Key.Height);
                                    m_textMatchesList.Add(rect);
                                    bool isDrawingPanel = false;
                                    page.matchTextPositions.Add(page.GetTextRectProperties(page, rect, m_zoomFactor, isDrawingPanel));
                                    IsMatchFound = true;
                                }
                                count++;
                            }
                        }
                    }
                    if (page.matchTextPositions.Count > 0)
                        matchTextPositionsDict.Add(page.pageId, page.matchTextPositions);
                    currentTextSearchPage++;
                }

            }
            return IsMatchFound;
        }

        private void MergeTextWithYaxis(Dictionary<RectangleF, TextMatchRectangle> textDictonary)
        {
            List<KeyValuePair<RectangleF, TextMatchRectangle>> tempList = new List<KeyValuePair<RectangleF, TextMatchRectangle>>(textDictonary);
            try
            {
                foreach (KeyValuePair<RectangleF, TextMatchRectangle> dictEntry in tempList)
                {
                    foreach (KeyValuePair<RectangleF, TextMatchRectangle> nextDictEntry in tempList)
                    {
                        if (dictEntry.Key.Y == nextDictEntry.Key.Y && textDictonary.ContainsKey(dictEntry.Key) && textDictonary.ContainsKey(nextDictEntry.Key) && dictEntry.Value != nextDictEntry.Value)
                        {
                            if (dictEntry.Key.Y == nextDictEntry.Key.Y)
                            {
                                textDictonary.Remove(dictEntry.Key);
                                textDictonary.Remove(nextDictEntry.Key);
                                RectangleF newRect = new RectangleF();
                                newRect.X = dictEntry.Key.X;
                                newRect.Y = dictEntry.Key.Y;
                                newRect.Width = dictEntry.Key.Width + nextDictEntry.Key.Width;
                                newRect.Height = dictEntry.Key.Height;
                                TextMatchRectangle tempAnnots = new TextMatchRectangle(newRect, dictEntry.Value.Text + nextDictEntry.Value.Text, newRect.Width, dictEntry.Value.ScaleX, dictEntry.Value.TextFont);
                                textDictonary.Add(newRect, tempAnnots);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }


        void LoadPages()
        {
            List<Page> pages = new List<Page>();
            for (int i = 0; i < m_loadedDocument.Pages.Count; i++)
            {
                Page page = new Page(m_loadedDocument.Pages[i]);
                page.Initialize(m_loadedDocument.Pages[i], false);
                pages.Add(page);
            }
            m_pageLocation = new Dictionary<int, double>();
            m_pageCollection = new Dictionary<int, Page>();

            PdfCrossTable basePage = (m_loadedDocument.Pages[0] as PdfLoadedPage).CrossTable;
            PdfDictionary dict = basePage.DocumentCatalog;

            if (dict.ContainsKey("Pages"))
            {
                PdfDictionary basePge = (dict["Pages"] as PdfReferenceHolder).Object as PdfDictionary;
                PdfArray kids = basePge["Kids"] as PdfArray;

                for (int i = 0; i < kids.Count; i++)
                {
                    PdfReferenceHolder tempRef = kids[i] as PdfReferenceHolder;
                    if (!m_pageKidsCollection.ContainsKey(tempRef.Reference))
                    {
                        m_pageKidsCollection.Add(tempRef.Reference, i);
                    }
                }
            }
            InitializePagePanel(pages.ToArray());
        }

        void InitializePagePanel(Page[] pages)
        {
            ImageDictionary.Clear();
            IsMousePressed = false;
            IsValueChanging = false;
            tableLayoutPanel.Dock = DockStyle.Fill;
            m_pagePanel = new VirtualizingPagePanel();
            m_zoomFactor = 1.0f;
            m_zoomMode = ZoomMode.Default;
            int scrollHeight = 0;
            int pageCount = 0;
            float currentHeight = 0;

            foreach (Page page in pages)
            {
                scrollHeight += page.Height;
                m_pageLocation.Add(pageCount, currentHeight);
                currentHeight += m_unitConvertor.ConvertFromPixels(page.Height, PdfGraphicsUnit.Point) + c_gapBetweenPages;
                page.pageId = pageCount;
                pageCount++;
                m_pagePanel.Pages.Add(page);
            }
            m_pageLocationUnaltered = m_pageLocation;
            m_currentPageIndex = 0;
            #region PanelSettings

            DrawingPanel.Dock = DockStyle.Fill;
            DrawingPanel.VerticalScroll.SmallChange = 10;
            DrawingPanel.VerticalScroll.LargeChange = 10;
            DrawingPanel.BackColor = Color.FromArgb(86, 86, 86);
            DrawingPanel.Padding = new Padding(0, 0, 0, 0);
            DrawingPanel.Margin = new Padding(0, 0, 0, 0);
            DrawingPanel.Focus();
            #endregion

            #region Vscol Settings
            Vscrol.Dock = DockStyle.Right;
            Vscrol.Size = new Size(18, DrawingPanel.Height);
            Vscrol.SmallChange = 1000;
            Vscrol.Minimum = 0;
            Vscrol.LargeChange = 10;
            
            //Vscrol.Maximum = (int)(currentHeight - m_unitConvertor.ConvertFromPixels(DrawingPanel.Height, PdfGraphicsUnit.Point)) + c_gapBetweenPages;
            int maximumscroll = m_ScrollChangeValue + (int)(currentHeight - m_unitConvertor.ConvertFromPixels(DrawingPanel.Height, PdfGraphicsUnit.Point)) + c_gapBetweenPages;
            if (maximumscroll > Vscrol.Minimum)
                Vscrol.Maximum = maximumscroll;
            else
                Vscrol.Maximum = -(maximumscroll);
            if (m_ScrollChangeValue != 0)
                Vscrol.LargeChange = m_ScrollChangeValue;
            m_newVValue = 0;
            Vscrol.Value = m_newVValue;
            Vscrol.Value = m_newVValue;
            this.tableLayoutPanel.ColumnStyles[1].Width = 18F;
            #endregion

            #region Hscrol Settings
            Hscrol.Dock = DockStyle.Bottom;
            Hscrol.Visible = false;
            Hscrol.SmallChange = 1;
            Hscrol.Minimum = 0;
            Hscrol.LargeChange = 10;
            m_newHValue = 0;
            Hscrol.Value = m_newHValue;
            Hscrol.Value = m_newHValue;
            #endregion

            DrawingPanel.Paint -= new PaintEventHandler(DrawingPanel_Paint);
            DrawingPanel.MouseWheel -= new MouseEventHandler(DrawingPanel_MouseWheel);
            Vscrol.Scroll -= new ScrollEventHandler(Vscrol_Scroll);
            Vscrol.ValueChanged -= new EventHandler(Vscrol_ValueChanged);
            Hscrol.ValueChanged -= Vscrol_ValueChanged;
            Hscrol.Scroll -= Vscrol_Scroll;
            Vscrol.MouseCaptureChanged -= new EventHandler(Vscrol_MouseCaptureChanged);

            DrawingPanel.Paint += new PaintEventHandler(DrawingPanel_Paint);
            DrawingPanel.MouseWheel += new MouseEventHandler(DrawingPanel_MouseWheel);
            Vscrol.Scroll += new ScrollEventHandler(Vscrol_Scroll);
            Vscrol.ValueChanged += new EventHandler(Vscrol_ValueChanged);
            Hscrol.ValueChanged += Vscrol_ValueChanged;
            Hscrol.Scroll += Vscrol_Scroll;
            Vscrol.MouseCaptureChanged += new EventHandler(Vscrol_MouseCaptureChanged);
            
            UpdateByScroll();
            if (exceptions.Exceptions.Length != 0)
            {
                notify = new NotificationBar("Essential PDF Viewer could not load parts of the document", exceptions.Exceptions.ToString());
            }
        }

        void Vscrol_MouseCaptureChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (!IsValueChanging)
            {
                if (IsMousePressed)
                {
                    IsMousePressed = false;
                    UpdateByScroll();
                }
                else
                    IsMousePressed = true;
            }
            IsValueChanging = false;
        }

        void DrawingPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
            int wheelDifference = 70;
            if (ScrollDisplacementValue != 0)
                wheelDifference = ScrollDisplacementValue;
            if (e.Delta < 0)
            {
                m_oldVValue = Vscrol.Value;

                if (Vscrol.Value + wheelDifference <= Vscrol.Maximum - ScrollDisplacementValue)
                {
                    m_newVValue = Vscrol.Value + wheelDifference;
                    Vscrol.Value = m_newVValue;
                    Vscrol.Value = m_newVValue;
                }
                else
                {
                    m_newVValue = Vscrol.Maximum - ScrollDisplacementValue;
                    Vscrol.Value = m_newVValue;
                    Vscrol.Value = m_newVValue;
                }
            }
            else if (e.Delta > 0)
            {
                if (Vscrol.Minimum >= 0)
                {
                    m_oldVValue = Vscrol.Value;

                    if (Vscrol.Value - wheelDifference >= 0)
                    {
                        m_newVValue = Vscrol.Value - wheelDifference;
                        Vscrol.Value = m_newVValue;
                        Vscrol.Value = m_newVValue;
                    }
                    else
                    {
                        m_newVValue = 0;
                        Vscrol.Value = m_newVValue;
                        Vscrol.Value = m_newVValue;
                    }
                }
            }
            UpdateByScroll();
        }

        void Vscrol_ValueChanged(object sender, EventArgs e)
        {
            UpdateByScroll();
            if ((sender as VScrollBar) != null)
            {
                int change = PreviousPosition - (sender as VScrollBar).Value;

                if (change == 10 || change == -10)
                    IsValueChanging = true;
            }
        }

        private void UpdateByScroll()
        {
            DrawingPanel_MouseMove(new object(), new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            if (DrawingPanel.CurrentScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (m_oldVValue < m_newVValue)
                    ScrollDown = true;
                else if (m_newVValue < m_oldVValue)
                    ScrollDown = false;
                else
                    ScrollDown = null;



                m_currentScrollValue = m_newVValue;
                m_liveHeight -= m_newVValue - m_oldVValue;

                double startOfCurrentPage;
                GetPageByOffset(m_newVValue, out m_currentPageIndex, out startOfCurrentPage);
                int tempCurrentPage = m_currentPageIndex;
                if (m_newVValue >= Vscrol.Maximum - (c_gapBetweenPages + 1))
                    tempCurrentPage = pageCount - 1;
                int currentPage = m_currentPageIndex;
                int left;
                m_difference = m_newVValue - startOfCurrentPage;
                int panelWidth = DrawingPanel.Bounds.Width;
                int currentHeight = -(int)m_difference;
                m_pageCollection = new Dictionary<int, Page>();

                if (m_currentPageIndex >= 1)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        if (m_pagePanel.Pages.Count > currentPage + i)
                        {
                            Page newPage = m_pagePanel.Pages[currentPage + i];
                            if (i == -1)
                                currentHeight = RotatedPageHeight(newPage, currentHeight, false);
                            newPage.CurrentLocation = currentHeight;

                            left = RotatedPagePosition(newPage);
                            if (left < 0)
                            {
                                if (m_zoomMode == PdfViewer.ZoomMode.FitWidth)
                                    this.tableLayoutPanel.RowStyles[1].Height = 0F;
                                else
                                    this.tableLayoutPanel.RowStyles[1].Height = 18F;
                                if (Hscrol.Visible == false)
                                    Hscrol.Visible = true;
                                left = 0;
                                if (newPage.CurrentLeftLocation > 0)
                                    newPage.CurrentLeftLocation = left;
                                if (newPage.CurrentLeftLocation <= 0 && Hscrol.Value > 0)
                                {
                                    newPage.CurrentLeftLocation = -m_newHValue;
                                }
                            }
                            else
                            {
                                this.tableLayoutPanel.RowStyles[1].Height = 0F;
                            }
                            newPage.Initialize(m_loadedDocument.Pages[currentPage + i], true, m_zoomFactor);
                            if (!m_pageCollection.ContainsKey(m_currentPageIndex - 1))
                                m_pageCollection.Add(m_currentPageIndex - 1, newPage);
                            m_currentPageIndex++;
                            currentHeight = RotatedPageHeight(newPage, currentHeight, true);
                        }
                    }
                }
                else
                {
                    if (currentHeight == 0)
                        currentHeight = 8;
                    for (int i = 0; i < 2; i++)
                    {
                        if (m_pagePanel.Pages.Count > i)
                        {
                            Page newPage = m_pagePanel.Pages[currentPage + i];
                            if (i == -1)
                                currentHeight -= (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor);
                            newPage.CurrentLocation = currentHeight;

                            left = RotatedPagePosition(newPage);
                            if (left < 0)
                            {
                                if (m_zoomMode == PdfViewer.ZoomMode.FitWidth)
                                    this.tableLayoutPanel.RowStyles[1].Height = 0F;
                                else
                                    this.tableLayoutPanel.RowStyles[1].Height = 18F;
                                Hscrol.Visible = true;
                                left = 0;
                                if (newPage.CurrentLeftLocation > 0)
                                    newPage.CurrentLeftLocation = left;
                                if (newPage.CurrentLeftLocation <= 0 && Hscrol.Value > 0)
                                {
                                    newPage.CurrentLeftLocation = -m_newHValue;
                                }
                            }
                            else
                            {
                                this.tableLayoutPanel.RowStyles[1].Height = 0F;
                            }

                            newPage.Initialize(m_loadedDocument.Pages[currentPage + i], true, m_zoomFactor);

                            if (!m_pageCollection.ContainsKey(m_currentPageIndex))
                                m_pageCollection.Add(m_currentPageIndex, newPage);
                            m_currentPageIndex++;
                            currentHeight = RotatedPageHeight(newPage, currentHeight, true);
                        }
                    }
                }
                m_currentPageIndex = tempCurrentPage;
            }
            else if (DrawingPanel.CurrentScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                int hDifference = 0;
                if (m_oldHValue < m_newHValue)
                {
                    hDifference = m_newHValue - m_oldHValue;//-difference
                    ScrollRight = true;
                }
                else if (m_newHValue < m_oldHValue)
                {
                    hDifference = m_oldHValue - m_newHValue;//+difference
                    ScrollRight = false;
                }
                else
                    ScrollRight = null;

                foreach (KeyValuePair<int, Page> element in m_pageCollection)
                {
                    if (ScrollRight == true)
                        element.Value.CurrentLeftLocation -= hDifference;
                    else if (ScrollRight == false)
                        element.Value.CurrentLeftLocation += hDifference;
                }
                m_oldHValue = m_newHValue;
                DrawingPanel.Invalidate();
            }
            else
                ScrollDown = null;
            DrawingPanel.Invalidate();
            Vscrol.Invalidate();
            Hscrol.Invalidate();
            ResetNavigationButtonStates();
        }

        private int RotatedPageHeight(Page newPage, int currentHeight, bool nextPage)
        {
            if (nextPage)
            {
                if (newPage.Resources != null)
                {
                    if (newPage.Resources.ContainsKey(DictionaryProperties.Rotate))
                    {
                        if ((float)newPage.Resources[DictionaryProperties.Rotate] == 90 || (float)newPage.Resources[DictionaryProperties.Rotate] == 270)
                            currentHeight += (int)(m_unitConvertor.ConvertFromPixels(newPage.Width, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
                        else
                            currentHeight += (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
                    }
                    else
                        currentHeight += (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
                }
                else
                    currentHeight += (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
            }
            else
            {
                if (newPage.Resources != null)
                {
                    if (newPage.Resources.ContainsKey(DictionaryProperties.Rotate))
                    {
                        if ((float)newPage.Resources[DictionaryProperties.Rotate] == 90 || (float)newPage.Resources[DictionaryProperties.Rotate] == 270)
                            currentHeight -= (int)(m_unitConvertor.ConvertFromPixels(newPage.Width, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
                        else
                            currentHeight -= (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
                    }
                    else
                        currentHeight -= (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
                }
                else
                    currentHeight -= (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) * m_zoomFactor + c_gapBetweenPages);
            }
            return currentHeight;
        }

        private int RotatedPagePosition(Page newPage)
        {
            int left;
            if (newPage.Resources != null)
            {
                if (newPage.Resources.ContainsKey(DictionaryProperties.Rotate))
                {
                    if ((float)newPage.Resources[DictionaryProperties.Rotate] == 90 || (float)newPage.Resources[DictionaryProperties.Rotate] == 270)
                        left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - newPage.Height * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
                    else
                        left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - newPage.Width * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
                }
                else if (newPage.CropBox != null)
                {
                    if (newPage.CropBox[0] == 0 && newPage.CropBox[1] == 0 && (int)m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point) == (int)newPage.CropBox[3] && (int)m_unitConvertor.ConvertFromPixels(newPage.Width, PdfGraphicsUnit.Point) - (int)newPage.CropBox[2] > (int)m_unitConvertor.ConvertFromPixels(newPage.Width, PdfGraphicsUnit.Point) / 2)
                    {
                        left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - m_unitConvertor.ConvertToPixels(newPage.CropBox[2], PdfGraphicsUnit.Point) * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
                    }
                    else if (newPage.CropBox[0] != 0 && newPage.CropBox[1] == 0 && newPage.CropBox[2] != newPage.Width && (int)newPage.CropBox[3] == (int)(m_unitConvertor.ConvertFromPixels(newPage.Height, PdfGraphicsUnit.Point)))
                    {
                        left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - m_unitConvertor.ConvertToPixels(newPage.CropBox[2] - newPage.CropBox[0], PdfGraphicsUnit.Point) * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
                    }
                    else
                        left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - newPage.Width * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
                }
                else
                    left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - newPage.Width * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
            }
            else
                left = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - newPage.Width * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
            return left;
        }

        void GetPageByOffset(double offset, out int pageIndex, out double pageStart)
        {
            pageIndex = 0;
            pageStart = 0;
            foreach (KeyValuePair<int, double> element in m_pageLocation)
            {
                if (element.Value < offset - c_gapBetweenPages)
                {
                    pageIndex = element.Key;
                    pageStart = element.Value;
                }
                else
                {
                    return;
                }
            }
        }

        void Vscrol_Scroll(object sender, ScrollEventArgs e)
        {
            IsVScrollBarChanged = true;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                DrawingPanel.CurrentScrollOrientation = e.ScrollOrientation;
                m_scrollType = e.Type;
                m_oldVValue = e.OldValue;
                m_newVValue = e.NewValue;
            }
            else
            {
                DrawingPanel.CurrentScrollOrientation = e.ScrollOrientation;
                m_oldHValue = e.OldValue;
                m_newHValue = e.NewValue;
            }
        }

        void PdfDocumentView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                DrawingPanel.CurrentScrollOrientation = e.ScrollOrientation;
                DrawingPanel.Invalidate();
                Vscrol.Location = new Point(this.Right - Vscrol.Width);
                Vscrol.Invalidate();
                return;
            }
        }

        void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            m_currentPageDest.Clear();
            e.Graphics.PageUnit = GraphicsUnit.Point;
            //Page page;
            float actualHeight, actualWidth, x = 0;
            GraphicsState state;
            bool HScrollVisibility = true;
            foreach (KeyValuePair<int, Page> coll in m_pageCollection)
            {
                page = coll.Value as Page;
                x = (int)m_unitConvertor.ConvertFromPixels((DrawingPanel.Bounds.Width - page.Width * m_zoomFactor) / 2, PdfGraphicsUnit.Point);
                if (x < 0)
                {
                    HScrollVisibility = false;
                }
            }
            if (HScrollVisibility)
            {
                Hscrol.Visible = false;
                Hscrol.Value = 0;
                Hscrol.Value = 0;
                m_newHValue = 0;
            }

            if (IsMousePressed)
            {
                if (m_zoomFactor != m_PreviousZoom)
                {

                }
                else
                {
                    Page currentPage = page;
                    foreach (KeyValuePair<int, Page> coll in m_pageCollection)
                    {
                        currentPage = coll.Value;
                        if (ImageDictionary.ContainsKey(coll.Key))
                        {
                            e.Graphics.DrawImage(ImageDictionary[coll.Key], new Point((int)x, currentPage.CurrentLocation));
                        }
                        else
                        {
                            if (BlankImage == null)
                            {
                                Bitmap bitmapImage = new Bitmap(currentPage.Bounds.Width, currentPage.Bounds.Height);
                                if (m_zoomFactor > 1)
                                    bitmapImage = new Bitmap((int)(currentPage.Bounds.Width * m_zoomFactor), (int)(currentPage.Bounds.Height * m_zoomFactor));
                                using (Graphics graphics = Graphics.FromImage(bitmapImage))
                                {
                                    graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, (int)(bitmapImage.Width * m_zoomFactor), (int)(bitmapImage.Height * m_zoomFactor)));
                                }
                                e.Graphics.DrawImage(bitmapImage, new Point((int)x, currentPage.CurrentLocation));
                                BlankImage = bitmapImage;
                            }
                            else
                                e.Graphics.DrawImage(BlankImage, new Point((int)x, currentPage.CurrentLocation));
                        }
                    }
                    return;
                }
            }

            foreach (KeyValuePair<int, Page> coll in m_pageCollection)
            {
                page = coll.Value as Page;
                bool shift = false;
                actualHeight = m_unitConvertor.ConvertFromPixels(page.Height * m_zoomFactor, PdfGraphicsUnit.Point);
                actualWidth = m_unitConvertor.ConvertFromPixels(page.Width * m_zoomFactor, PdfGraphicsUnit.Point);

                x = RotatedPagePosition(page);
                if (x > 0)
                {
                    page.CurrentLeftLocation = (int)x;
                }

                state = e.Graphics.Save();

                if (page.Resources.ContainsKey(DictionaryProperties.Rotate))
                {
                    float rotation = (float)page.Resources[DictionaryProperties.Rotate];
                    if (rotation == 90 || rotation == 270)
                    {
                        e.Graphics.FillRectangle(Brushes.White, new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualHeight, actualWidth));
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(86, 86, 86)), new RectangleF(page.CurrentLeftLocation, page.CurrentLocation + actualWidth, actualHeight, 8));
                        e.Graphics.SetClip(new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualHeight, actualWidth));
                        e.Graphics.TranslateTransform(page.CurrentLeftLocation, page.CurrentLocation + actualWidth);
                        shift = true;

                        e.Graphics.RotateTransform(rotation);
                        if (rotation == 90)
                            e.Graphics.TranslateTransform(-e.Graphics.ClipBounds.Width, 0);
                        else
                            e.Graphics.TranslateTransform(0, e.Graphics.ClipBounds.Height);
                    }
                    else if (rotation == 180)
                    {
                        e.Graphics.FillRectangle(Brushes.White, new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth, actualHeight));
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(86, 86, 86)), new RectangleF(page.CurrentLeftLocation, page.CurrentLocation + actualHeight, actualWidth, 8));
                        e.Graphics.SetClip(new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth, actualHeight));
                        e.Graphics.TranslateTransform(page.CurrentLeftLocation, page.CurrentLocation + actualHeight);

                        e.Graphics.RotateTransform(rotation);
                        e.Graphics.TranslateTransform(-e.Graphics.ClipBounds.Width, e.Graphics.ClipBounds.Height);
                    }
                }
                else
                {
                    if (page.CropBox == null)
                    {
                        e.Graphics.FillRectangle(Brushes.White, new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth, actualHeight));
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(86, 86, 86)), new RectangleF(page.CurrentLeftLocation, page.CurrentLocation + actualHeight, actualWidth, 8));
                        e.Graphics.SetClip(new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth, actualHeight));
                        e.Graphics.TranslateTransform(page.CurrentLeftLocation, page.CurrentLocation + actualHeight);
                    }
                    #region CropBox conditions
                    else
                    {
                        if (page.CropBox[0] != 0 && page.CropBox[1] == 0 && page.CropBox[2] != page.Width && (int)page.CropBox[3] == (int)(m_unitConvertor.ConvertFromPixels(page.Height, PdfGraphicsUnit.Point)))
                        {
                            e.Graphics.FillRectangle(Brushes.White, new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth - page.CropBox[0] * m_zoomFactor, actualHeight));
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(86, 86, 86)), new RectangleF(page.CurrentLeftLocation, page.CurrentLocation + actualHeight, actualWidth - page.CropBox[0] * m_zoomFactor, 8));
                            e.Graphics.SetClip(new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth - page.CropBox[0] * m_zoomFactor, actualHeight));
                            e.Graphics.TranslateTransform(-(page.CropBox[0] * m_zoomFactor - page.CurrentLeftLocation), page.CurrentLocation + actualHeight);
                        }
                        else if (page.CropBox[0] == 0 && page.CropBox[1] == 0 && (int)m_unitConvertor.ConvertFromPixels(page.Height, PdfGraphicsUnit.Point) == (int)page.CropBox[3] && (int)m_unitConvertor.ConvertFromPixels(page.Width, PdfGraphicsUnit.Point) - (int)page.CropBox[2] > (int)m_unitConvertor.ConvertFromPixels(page.Width, PdfGraphicsUnit.Point)/2)
                        {
                            e.Graphics.FillRectangle(Brushes.White, new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, page.CropBox[2]*m_zoomFactor, actualHeight));
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(86, 86, 86)), new RectangleF(page.CurrentLeftLocation, page.CurrentLocation + actualHeight, page.CropBox[2] * m_zoomFactor, 8));
                            e.Graphics.SetClip(new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, page.CropBox[2] * m_zoomFactor, actualHeight));
                            e.Graphics.TranslateTransform(page.CurrentLeftLocation, page.CurrentLocation + actualHeight);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(Brushes.White, new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth, actualHeight));
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(86, 86, 86)), new RectangleF(page.CurrentLeftLocation, page.CurrentLocation + actualHeight, actualWidth, 8));
                            e.Graphics.SetClip(new RectangleF(page.CurrentLeftLocation, page.CurrentLocation, actualWidth, actualHeight));
                            e.Graphics.TranslateTransform(page.CurrentLeftLocation, page.CurrentLocation + actualHeight);
                            if (page.CropBox[0] != 0 && page.CropBox[1] != 0 && page.CropBox[2] != page.Width && (int)page.CropBox[3] == (int)(m_unitConvertor.ConvertFromPixels(page.Height, PdfGraphicsUnit.Point) + page.CropBox[1]))
                            {
                                if (m_zoomFactor <= 1)
                                    e.Graphics.TranslateTransform(-page.CropBox[0], page.CropBox[1]);
                                else if ((float)Math.Round(m_zoomFactor, 1) == (float)Math.Round((decimal)this.Width / page.Width, 1))
                                {
                                    e.Graphics.TranslateTransform(-(page.Bounds.X + page.Bounds.Width), page.CropBox[1] * m_zoomFactor);
                                }
                            }
                            else if (page.CropBox[0] != 0 && page.CropBox[1] == 0 && Math.Round((decimal)m_unitConvertor.ConvertToPixels(page.CropBox[2] - page.CropBox[0], PdfGraphicsUnit.Point), MidpointRounding.AwayFromZero) == page.Width && (int)m_unitConvertor.ConvertToPixels(page.CropBox[3], PdfGraphicsUnit.Point) == (int)page.Height)
                            {
                                if (m_zoomFactor <= 1)
                                    e.Graphics.TranslateTransform(-page.CropBox[0] * m_zoomFactor, 0);
                                else if (m_zoomFactor == (float)Math.Round((decimal)this.Width / page.Width, 2))
                                    e.Graphics.TranslateTransform(-actualWidth, 0);
                            }
                        }
                    }
                    #endregion
                }
                if (page.RecordCollection == null)
                    page.Initialize(m_loadedDocument.Pages[m_currentPageIndex], true);
                if (m_cmyk == null)
                    m_cmyk = new DeviceCMYK();

                if (!ImageDictionary.ContainsKey(coll.Key))
                {
                    Bitmap bitmapImage = new Bitmap((int)(page.Bounds.Width * m_zoomFactor), (int)(page.Bounds.Height * m_zoomFactor));
                    using (Graphics graphics = Graphics.FromImage(bitmapImage))
                    {
                        graphics.TranslateTransform(0, 0);

                        graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, (int)(page.Bounds.Width * m_zoomFactor), (int)(page.Bounds.Height * m_zoomFactor)));
                        graphics.ScaleTransform(m_zoomFactor, m_zoomFactor);
                        ImageRenderer renderer = new ImageRenderer(page.RecordCollection, page.Resources, graphics, true, m_cmyk);
                        CultureInfo current = Thread.CurrentThread.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                        renderer.RenderAsImage();
                        Thread.CurrentThread.CurrentCulture = current;
                    }
                    ImageDictionary.Add(coll.Key, bitmapImage);
                }
                e.Graphics.DrawImage(ImageDictionary[coll.Key], new Point(0, (int)(e.Graphics.ClipBounds.Y)));

                e.Graphics.Restore(state);
                state = e.Graphics.Save();
                if (!shift)
                    e.Graphics.DrawRectangle(new Pen(Brushes.Black, 1f), new Rectangle((int)page.CurrentLeftLocation, (int)page.CurrentLocation - 1, (int)actualWidth + 1, (int)actualHeight + 1));
                else
                    e.Graphics.DrawRectangle(new Pen(Brushes.Black, 1f), new Rectangle((int)page.CurrentLeftLocation, (int)page.CurrentLocation - 1, (int)actualHeight + 1, (int)actualWidth + 1));
                e.Graphics.Restore(state);

                #region Annots
                if (page.pageAnnotations.Count > 0)
                {
                    this.m_currentPageAnnotsList.Clear();
                    foreach (PageAnnotation annots in page.pageAnnotList)
                    {
                        float annotX = annots.Rect.X + page.CurrentLeftLocation;
                        float annotY = annots.Rect.Y + page.CurrentLocation;
                        float annotWidth = annots.Rect.Width;
                        float annotHeight = annots.Rect.Height;

                        RectangleF temprect = new RectangleF(annotX, annotY, annotWidth, annotHeight);
                        if (!m_currentPageDest.ContainsKey(temprect))
                        {
                            m_currentPageDest.Add(temprect, annots.URI);
                            if (!m_PageAnnotDest.ContainsKey(temprect))
                            {
                                m_PageAnnotDest.Add(temprect, annots.PageAnnotDestinations);
                            }
                        }
                        PageAnnotation tempAnnots = new PageAnnotation(temprect, annots.URI, annots.Border, annots.AnnotType, annots.PageAnnotDestinations);
                        m_currentPageAnnotsList.Add(tempAnnots);
                    }

                    foreach (PageAnnotation annots in this.m_currentPageAnnotsList)
                    {
                        ControlPaint.DrawBorder(e.Graphics, new Rectangle((int)annots.Rect.X, (int)annots.Rect.Y, (int)annots.Rect.Width, (int)annots.Rect.Height), Color.Black, (int)annots.Border, ButtonBorderStyle.Solid, Color.Black, (int)annots.Border, ButtonBorderStyle.Solid, Color.Black, (int)annots.Border, ButtonBorderStyle.Solid, Color.Black, (int)annots.Border, ButtonBorderStyle.Solid);
                    }
                }
                #endregion

                #region URLRecoganization
                if (ImageRenderer.URLDictonary.Count > 0)
                {
                    page.pageURLs.Clear();
                    foreach (PageURL url in ImageRenderer.URLDictonary)
                    {
                        float scaleX = 0, scaleY = 0;
                        Matrix transformMatrix = url.TransformPoints;
                        float[] transformPoints = transformMatrix.Elements;
                        PointF transformLocation = url.CurrentLocation;
                        if (transformPoints[0] != 0 && transformPoints[3] != 0)
                        {
                            scaleX = transformPoints[0];
                            scaleY = transformPoints[3];
                        }
                        float locationX = (transformPoints[4] + transformLocation.X * scaleX);
                        float locationY = (transformPoints[5] + transformLocation.Y * scaleY);
                        float width = url.TextElementWidth * scaleX;
                        float height = url.FontSize * scaleX;
                        page.pageURLList.Clear();
                        //e.Graphics.DrawRectangle(new Pen(Brushes.Black), locationX, locationY, width, height);
                        PageAnnotation tempAnnots = new PageAnnotation(new RectangleF(locationX, locationY, width, height), url.URI, 1, "Action");
                        page.GetURLProperties(page, tempAnnots);
                        foreach (PageAnnotation annots in page.pageURLList)
                        {
                            if (!m_currentPageDest.ContainsKey(annots.Rect))
                            {
                                m_currentPageDest.Add(annots.Rect, annots.URI);
                            }
                            this.m_currentPageAnnotsList.Add(tempAnnots);
                        }
                    }
                    ImageRenderer.URLDictonary.Clear();
                }
                #endregion

                #region TextSearch
                if (ImageRenderer.textDictonary.Count > 0)
                {
                    if (page.pageId == m_matchFoundPage - 1)
                    {
                        m_currentPageRendered = page.pageId;
                        bool isDrawingPanel = true;
                        HighlightTextMatchs(e.Graphics, isDrawingPanel);
                    }
                }
                #endregion
            }
            m_PreviousZoom = m_zoomFactor;
        }

        private void HighlightTextMatchs(Graphics graphics, bool isDrawingPanel)
        {
            m_textMatchesList.Clear();
            matchTextRects = new RectangleF[m_currentPageMatchCount];
            int i = 0;
            foreach (KeyValuePair<RectangleF, TextMatchRectangle> item in m_txtLocations)
            {
                if (((item.Value.Text.IndexOf(m_searchBox.searchInputTxtBox.Text, StringComparison.InvariantCultureIgnoreCase)) >= 0) && !string.IsNullOrEmpty(m_searchBox.searchInputTxtBox.Text))
                {
                    float subTxtWidth = 0;
                    int startCharLoc = item.Value.Text.IndexOf(m_searchBox.searchInputTxtBox.Text, StringComparison.InvariantCultureIgnoreCase);

                    float charWidth = item.Value.TextWidth / item.Value.Text.Length;

                    SizeF temp = graphics.MeasureString(" ", item.Value.TextFont);

                    float scalingWidth = item.Value.ScaleX;

                    string preSubString = item.Value.Text.Substring(0, startCharLoc);
                    SizeF preStringSize = graphics.MeasureString(preSubString, item.Value.TextFont);
                    float preTxtWidth = preStringSize.Width * scalingWidth;

                    SizeF stringSize = graphics.MeasureString(m_searchBox.searchInputTxtBox.Text, item.Value.TextFont);
                    float txtWidth = stringSize.Width * scalingWidth;
                    float incrementalSpace = 0;

                    if (preTxtWidth > 0)
                    {
                        subTxtWidth = m_searchBox.searchInputTxtBox.Text.Length;
                    }

                    float newRectX = item.Key.X + preTxtWidth - subTxtWidth;
                    if (preSubString.EndsWith(" "))
                    {
                        incrementalSpace += temp.Width * scalingWidth;
                        incrementalSpace = m_unitConvertor.ConvertToPixels(incrementalSpace, PdfGraphicsUnit.Point);
                        newRectX += incrementalSpace;
                        txtWidth += incrementalSpace;
                    }
                    RectangleF rect = new RectangleF(newRectX, item.Key.Y, txtWidth, item.Key.Height);
                    m_textMatchesList.Add(rect);
                    matchTextRects[i++] = page.GetTextRectProperties(page, rect, m_zoomFactor, isDrawingPanel);
                }
            }

            if (m_nextMatch != -1 && m_nextMatch < m_textMatchesList.Count)
            {
                RectangleF nextRect = m_textMatchesList[m_nextMatch];
                if (matchTextRects[m_nextMatch].Y > 0)
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(100, 255, 255, 0)))
                    {
                        graphics.FillRectangle(brush, matchTextRects[m_nextMatch].X, matchTextRects[m_nextMatch].Y, matchTextRects[m_nextMatch].Width, matchTextRects[m_nextMatch].Height);

                    }
                    //graphics.DrawRectangle(new Pen(Brushes.Red, 1), matchTextRects[m_nextMatch].X, matchTextRects[m_nextMatch].Y, matchTextRects[m_nextMatch].Width, matchTextRects[m_nextMatch].Height);

                    float visibleclipboundsBottom = m_unitConvertor.ConvertFromPixels(DrawingPanel.Bounds.Bottom, PdfGraphicsUnit.Point);
                    if ((visibleclipboundsBottom - 25) < matchTextRects[m_nextMatch].Y)
                    {
                        if (IsVScrollBarChanged == false)
                        {
                            m_newVValue += (int)(matchTextRects[m_nextMatch].Y / 2);
                            UpdateByScroll();
                            Vscrol.Value = m_newVValue;
                        }
                    }

                    float visibleclipboundsTop = m_unitConvertor.ConvertFromPixels(DrawingPanel.Bounds.Top, PdfGraphicsUnit.Point);
                }
                else
                {
                    if (IsVScrollBarChanged == false)
                    {
                        m_newVValue += (int)(matchTextRects[m_nextMatch].Y - 100);
                        UpdateByScroll();
                        Vscrol.Value = m_newVValue;
                    }
                }
            }

        }


        void SetZoom()
        {
            //..Step1 : Compute Zoom Factor.
            int index = this.CurrentPageIndex;
            float sx = 0;
            float sy = 0;

            Page page = m_pagePanel.Pages[index - 1];

            sx = (float)Size.Width / (float)page.Width;
            sy = (float)Size.Height / (float)page.Height;
            if (page.Resources != null)
                if (page.Resources.ContainsKey(DictionaryProperties.Rotate))
                    if ((float)page.Resources[DictionaryProperties.Rotate] == 90 || (float)page.Resources[DictionaryProperties.Rotate] == 270)
                    {
                        sx = (float)Size.Width / (float)page.Height;
                        sy = (float)Size.Height / (float)page.Width;
                    }

            if (sx < 0)
                sx = 1;

            if (sy < 0)
                sy = 1;

            if (m_zoomMode == ZoomMode.FitWidth)
            {
                m_zoomFactor = sx;
            }

            if (m_zoomMode == ZoomMode.FitPage)
            {
                m_zoomFactor = Math.Min(sx, sy);
            }
            if (ZoomChanged != null)
            {
                ZoomChanged(this, ((int)(m_zoomFactor * 100)));
                float zoom = m_zoomFactor * 100;
                SetZoom((int)zoom);
            }
            UpdatePanelOnZoom();
            GoToPageAtIndex(index);
            UpdateByScroll();
        }

        void SetZoom(ZoomMode mode)
        {
            ImageDictionary.Clear();
            BlankImage = null;
            //..Step1 : Compute Zoom Factor.
            int index = this.CurrentPageIndex;
            float sx = 0;
            float sy = 0;

            Page page = m_pagePanel.Pages[index - 1];

            sx = (float)Size.Width / (float)page.Width;
            sy = (float)Size.Height / (float)page.Height;

            if (sx < 0)
                sx = 1;

            if (sy < 0)
                sy = 1;

            if (mode == ZoomMode.FitWidth)
            {
                m_zoomFactor = sx;
            }

            if (mode == ZoomMode.FitPage)
            {
                m_zoomFactor = Math.Min(sx, sy);
            }

            this.GoToPageAtIndex(index);
            if (ZoomChanged != null)
            {
                float zoom = m_zoomFactor * 100;
                SetZoom((int)zoom);
            }
            UpdatePanelOnZoom();
            UpdateByScroll();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            //This is needed to ensure the scrolling.
            Matrix m = new Matrix();
            m.Translate(this.AutoScrollPosition.X, this.AutoScrollPosition.Y, MatrixOrder.Append);
            e.Graphics.Transform = m;
        }

        void SetZoom(int percentage)
        {
            ImageDictionary.Clear();
            BlankImage = null;
            int index = m_currentPageIndex;
            m_zoomFactor = (float)percentage / 100;
            if (ZoomChanged != null)
            {
                ZoomChanged(this, ((int)(m_zoomFactor * 100)));
            }
            UpdatePanelOnZoom();
            GoToPageAtIndex(index + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            PrintDocument document = sender as PrintDocument;

            if (document.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                int frompage = document.PrinterSettings.FromPage;
                int topage = document.PrinterSettings.ToPage;
                
                if (topage > m_printToPage)
                    throw new System.IndexOutOfRangeException("ToPage should be less than the page count");
                if (frompage < 1)
                    throw new System.IndexOutOfRangeException("FromPage should be greater than one");
                if (frompage > topage)
                    throw new System.Exception("ToPage should be greater than or equal to FromPage");
                
                if (m_currentPageOnPrint == 0)
                    m_currentPageOnPrint = frompage - 1;

                Image img = ExportAsImage(m_currentPageOnPrint, new SizeF(2000, 2000), true);
                CultureInfo current = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                e.Graphics.DrawImage(img, new Rectangle(0, 0, (int)e.Graphics.VisibleClipBounds.Width, (int)e.Graphics.VisibleClipBounds.Height));
                Thread.CurrentThread.CurrentCulture = current;
                m_currentPageOnPrint++;
                
                if (m_currentPageOnPrint <= (topage - 1))
                {
                    e.HasMorePages = true;
                }
            }
            else
            {
                if (m_printToPage == 0)
                {
                    m_printToPage = PageCount;
                }
                if (m_currentPageOnPrint < m_printToPage)
                {
                    //Image img = ExportAsMetafile(m_currentPageOnPrint);
                    Image img;
                    if (m_loadedDocument.Pages[m_currentPageOnPrint].Size.Width < 300 && m_loadedDocument.Pages[m_currentPageOnPrint].Size.Height < 100)
                    {
                        img = ExportAsMetafile(m_currentPageOnPrint);
                    }
                    else
                        img = ExportAsImage(m_currentPageOnPrint, new SizeF(m_loadedDocument.Pages[m_currentPageOnPrint].Size.Width + 2000, m_loadedDocument.Pages[m_currentPageOnPrint].Size.Height + 2000), true);
                    
                    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    if (m_loadedDocument.Pages[m_currentPageOnPrint].Size.Width < 300 && m_loadedDocument.Pages[m_currentPageOnPrint].Size.Height < 100)
                        e.Graphics.DrawImage(img, new Point(((int)e.Graphics.VisibleClipBounds.Width - (int)m_loadedDocument.Pages[m_currentPageOnPrint].Size.Height) / 2, ((int)e.Graphics.VisibleClipBounds.Height - (int)m_loadedDocument.Pages[m_currentPageOnPrint].Size.Width) / 2));
                    else
                        e.Graphics.DrawImage(img, new Rectangle(0, 0, (int)e.Graphics.VisibleClipBounds.Width, (int)e.Graphics.VisibleClipBounds.Height));
                    Thread.CurrentThread.CurrentCulture = current;
                    m_currentPageOnPrint++;
                    e.HasMorePages = m_currentPageOnPrint < (m_printToPage);
                }
            }
        }
        #endregion

        #region IDocumentView Members
        /// <summary>
        /// Gets the page count.
        /// </summary>
        public int PageCount
        {
            get
            {
                if (m_pagePanel == null)
                    return 0;

                return m_pagePanel.Pages.Count;
            }
        }
        /// <summary>
        /// Gets the current page index which is currently in view.
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return m_currentPageIndex + 1;
            }
        }
        /// <summary>
        /// Gets/Sets the Zoom mode.
        /// </summary>
        public ZoomMode ZoomMode
        {
            get
            {
                return m_zoomMode;
            }
            set
            {
                m_zoomMode = value;

                if (m_pagePanel != null && m_pagePanel.Pages.Count > 0)
                    SetZoom();
            }
        }

        /// <summary>
        /// Magnifies the page of the document to the provided zoom percentage.
        /// </summary>
        /// <param name="percentage">Zoom percentage</param>
        public void ZoomTo(int percentage)
        {
            SetZoom(percentage);
        }

        /// <summary>
        /// Magnifies the page of the document to the provided zoom mode.
        /// </summary>
        /// <param name="mode">zoom mode</param>
        public void ZoomTo(ZoomMode mode)
        {
            SetZoom(mode);
        }

        /// <summary>
        /// Navigates to the first page of the document.
        /// </summary>
        public void GoToFirstPage()
        {
            IsVScrollBarChanged = true;
            m_pageCount = 0;
            if (!CanGoToFirstPage)
            {
                notify = new NotificationBar();
            }
            GoToPageAtIndex(1);
        }

        /// <summary>
        /// Navigates to the last page of the document
        /// </summary>
        public void GoToLastPage()
        {
            IsVScrollBarChanged = true;
            m_pageCount = m_pagePanel.Pages.Count - 1;
            if (!CanGoToLastPage)
            {
                notify = new NotificationBar();
                return;
            }
            GoToPageAtIndex(this.PageCount);
        }

        /// <summary>
        /// Navigates to the previous page of the document.
        /// </summary>
        public void GoToPreviousPage()
        {
            IsVScrollBarChanged = true;
            if (m_pageCount <= m_pagePanel.Pages.Count - 1)
            {
                m_pageCount--;
            }
            if (!CanGoToPreviousPage)
            {
                notify = new NotificationBar();
            }
            GoToPageAtIndex(m_currentPageIndex);
        }

        /// <summary>
        /// Navigates to the next page of the document.
        /// </summary>
        public void GoToNextPage()
        {
            IsVScrollBarChanged = true;
            if (m_pageCount < m_pagePanel.Pages.Count - 1)
            {
                m_pageCount++;
            }
            if (!CanGoToNextPage)
            {
                notify = new NotificationBar();
            }
            GoToPageAtIndex(m_currentPageIndex + 2);
        }

        internal void ResetNavigationButtonStates()
        {
            if (m_currentPageIndex == 0 && PageCount == 1)
            {
                CanGoToFirstPage = false;
                CanGoToPreviousPage = false;
                CanGoToNextPage = false;
                CanGoToLastPage = false;
            }
            else if (m_currentPageIndex == 0 && PageCount > 1)
            {
                CanGoToFirstPage = false;
                CanGoToPreviousPage = false;
                CanGoToNextPage = true;
                CanGoToLastPage = true;
            }
            else if (m_currentPageIndex > 0 && m_currentPageIndex + 1 < PageCount)
            {
                CanGoToFirstPage = true;
                CanGoToPreviousPage = true;
                CanGoToNextPage = true;
                CanGoToLastPage = true;
            }
            else if (m_currentPageIndex + 1 == PageCount)
            {
                CanGoToFirstPage = true;
                CanGoToPreviousPage = true;
                CanGoToNextPage = false;
                CanGoToLastPage = false;
            }
            if (NavigationButtonStatesChanged != null)
                NavigationButtonStatesChanged(this, null);

            if (CurrentPageChanged != null)
                CurrentPageChanged(this, null);
        }

        /// <summary>
        /// Gets a boolean value indicating whether the control can navigate to the first page.
        /// </summary>
        public bool CanGoToFirstPage
        {
            get
            {
                return m_canGoToFirstPage;
            }
            internal set
            {
                m_canGoToFirstPage = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the control can navigate to the previous page.
        /// </summary>
        public bool CanGoToPreviousPage
        {
            get
            {
                return m_canGoToPreviousPage;
            }
            internal set
            {
                m_canGoToPreviousPage = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the control can navigate to the next page.
        /// </summary>
        public bool CanGoToNextPage
        {
            get
            {
                return m_canGoToNextPage;
            }
            internal set
            {
                m_canGoToNextPage = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the control can navigate to the last page.
        /// </summary>
        public bool CanGoToLastPage
        {
            get
            {
                return m_canGoToLastPage;
            }
            internal set
            {
                m_canGoToLastPage = value;
            }
        }
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="index">The page index</param>
        public void GoToPageAtIndex(int index)
        {
            index -= 1;
            int destination = 0;

            DrawingPanel.CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
            destination = (int)m_pageLocation[index] + c_gapBetweenPages + 1;
            if (destination > Vscrol.Maximum)
                destination = Vscrol.Maximum;
            if (destination < Vscrol.Minimum)
                destination = Vscrol.Minimum;

            Vscrol.Value = destination;
            Vscrol.Value = destination;

            m_newVValue = Vscrol.Value;
            UpdateByScroll();
            ResetNavigationButtonStates();
        }
        #endregion

        /// <summary>
        /// Returns the page number and rectangle positions of the text matchs
        /// </summary>
        /// <param name="text">The text to be searched</param>
        /// <param name="matchTextPosition">Holds the page number and rectangle positions of the text matches</param>
        internal bool FindText(String text, out Dictionary<int, List<RectangleF>> matchTextPosition)
        {
            bool IsMatchFound = FindTextMatches(text, out matchTextPosition);
            return IsMatchFound;
        }
    }
    internal class InheritedPanel : UserControl
    {
        internal ScrollOrientation CurrentScrollOrientation = ScrollOrientation.VerticalScroll;
        public InheritedPanel()
        {
            this.DoubleBuffered = true;
        }
    }

    /// <summary>
    /// Class to show the tooltip
    /// </summary>   
    internal class InheritedLabel : Label
    {
        public InheritedLabel()
        {
            this.DoubleBuffered = true;
        }
    }
}