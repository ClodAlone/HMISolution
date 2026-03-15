//# define DUMP_IMAGE
#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

#region file using directives
using System;
using System.Drawing;

using Syncfusion.DocIO.Rendering;
using Syncfusion.Layouting;
using Syncfusion.DocIO.DLS.Convertors;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Metafile = System.Drawing.Imaging.Metafile;

#endregion

namespace Syncfusion.DocIO.DLS.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DocumentLayouter : ILayoutProcessHandler
    {
        #region Constants
        /// <summary>
        /// The Default space between the footer and textbody
        /// </summary>
        internal const float SpaceBetweenTextbodyAndFooter = 0.5f;
        #endregion
        #region Fields
        private PageCollection m_pages = new PageCollection();
        private Page m_currPage = null;
        private IWidgetContainer m_docWidget = null;
        private IWSection m_currSection = null;
        private HeaderFooterLPHandler m_headerLPHandler;
        private HeaderFooterLPHandler m_footerLPHandler;
        private int m_columnIndex = 0;
        private float m_columnsWidth = 0f;
        private int m_nextPageIndex = 0;
        private bool m_bFirstPageForSection = true;
        private bool m_bIsLayoutingHeaderFooter = false;
        /// <summary>
        /// Specifies whether the footnote need to be restart for each page
        /// </summary>
        private bool m_bisNeedToRestartFootnote = true;
        /// <summary>
        /// Specifies whether the endnote need to be restart for each section
        /// </summary>
        private bool m_bisNeedToRestartEndnote = true;
        private bool m_bDirty = false;
        private int m_imageResolution = 100;
        private float m_totalColumnWidth = 0;
        private WordToPDFResult m_pageResult;
        /// <summary>
        /// Specifies the footnote count in each columns
        /// </summary>
        private int m_footnoteCount = 0;
        /// <summary>
        /// Specifies the Endnote count in each columns
        /// </summary>
        private int m_endnoteCount = 0;
        /// <summary>
        /// Height used in current page
        /// </summary>
        private float m_usedHeight = 0;
        /// <summary>
        /// Page Client height 
        /// </summary>
        private float m_clientHeight = 0;
        /// <summary>
        /// Total height layouted by the section
        /// </summary>
        private float m_totalHeight = 0;
        /// <summary>
        /// Fixed height for the continuous section
        /// </summary>
        private float m_sectionFixedHeight = 0;
        /// <summary>
        /// Current page top
        /// </summary>
        private float m_pageTop = 0;
        /// <summary>
        /// Current section index
        /// </summary>
        private int m_sectionIndex = -1;
        /// <summary>
        /// Is first page of the document.
        /// </summary>
        private bool m_isFirstPage = true;
        /// <summary>
        /// Is new page created for current section
        /// </summary>
        private bool m_sectionNewPage = false;
        /// <summary>
        /// Need to create new page
        /// </summary>
        private bool m_createNewPage = false;
        /// <summary>
        /// Is Continuous Section Layouted
        /// </summary>
        private bool m_isContinuousSectionLayouted = false;
        /// <summary>
        /// Line height of the section
        /// </summary>
        private List<float> m_lineHeights = new List<float>();
        /// <summary>
        /// Column height of the section
        /// </summary>
        private List<float> m_columnHeight = new List<float>();
        /// <summary>
        /// Column has break items like (Page break or Column break)
        /// </summary>
        private List<bool> m_columnHasBreakItem = new List<bool>();
        /// <summary>
        /// Previous columns width
        /// </summary>
        private List<float> m_prevColumnsWidth = new List<float>();
        private List<FloatingItem> m_prevFloatingItems;
        private List<float> m_absolutePositionedTableHeights;
        //Updating Toc field
        private Dictionary<int, List<string>> m_tocLevels;
        private List<int> m_tocEntryPageNumbers;
        private bool m_useTCFields;
        private WParagraph m_prevTocParagraph;
        internal bool m_UpdatingToc;
        internal bool m_UpdatingPageFields;
        internal List<FloatingItem> m_FloatingItems = new List<FloatingItem>();
        #endregion

        #region Static Fields
        [ThreadStatic]
        internal static DrawingContext m_dc;
        [ThreadStatic]
        internal static bool IsFirstLayouting;
        [ThreadStatic]
        internal static int PageNumber;
        [ThreadStatic]
        private static List<Dictionary<string, BookmarkHyperlink>> m_bookmarkHyperlinks = null;
        [ThreadStatic]
        private static List<BookmarkPosition> m_bookmarks = null;
        /// <summary>
        /// Footnote ID when the footnote restart with each page
        /// </summary>
        [ThreadStatic]
        internal static int m_footnoteIDRestartEachPage = 1;
        internal static int m_endnoteIDRestartEachPage = 1;
        /// <summary>
        /// Footnote ID when the footnote restart with each section
        /// </summary>
        [ThreadStatic]
        internal static int m_footnoteIDRestartEachSection = 1;
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the DrawingContext
        /// </summary>
        internal static DrawingContext DrawingContext
        {
            get
            {
                if (m_dc == null)
                    m_dc = new DrawingContext();
                return m_dc;
            }
        }
        /// <summary>
        /// Gets the bookmark hyperlinks.
        /// </summary>
        internal static List<Dictionary<string, BookmarkHyperlink>> BookmarkHyperlinks
        {
            get
            {
                if (m_bookmarkHyperlinks == null)
                    m_bookmarkHyperlinks = new List<Dictionary<string, BookmarkHyperlink>>();
                return m_bookmarkHyperlinks;
            }
        }
        /// <summary>
        /// Gets the bookmarks.
        /// </summary>
        internal static List<BookmarkPosition> Bookmarks
        {
            get
            {
                if (m_bookmarks == null)
                    m_bookmarks = new List<BookmarkPosition>();
                return m_bookmarks;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets pages collection
        /// </summary>
        public PageCollection Pages
        {
            get
            {
                return m_pages;
            }
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The current page.</value>
        internal Page CurrentPage
        {
            get
            {
                return m_currPage;
            }
        }

        /// <summary>
        /// Gets the current section.
        /// </summary>
        /// <value>The current section.</value>
        protected IWSection CurrentSection
        {
            get
            {
                return m_currSection;
            }
        }

        /// <summary>
        /// Gets the current column.
        /// </summary>
        /// <value>The current column.</value>
        protected Column CurrentColumn
        {
            get
            {
                return (CurrentSection.Columns.Count == 0)
                  ? null : CurrentSection.Columns[m_columnIndex];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is even page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is even page; otherwise, <c>false</c>.
        /// </value>
        protected bool IsEvenPage
        {
            get
            {
                return (m_nextPageIndex % 2) == 0;
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public WordToPDFResult PageResult
        {
            get
            {
                return m_pageResult;
            }
        }
        /// <summary>
        /// Sets the IsCreateNewPage for the Section with NoBreak.
        /// </summary>
        internal bool IsCreateNewPage
        {
            set
            {
                m_createNewPage = value;
            }
        }
        /// <summary>
        /// Gets or sets the TOC levels.
        /// </summary>
        /// <value>The TOC levels.</value>
        internal Dictionary<int, List<string>> TOCLevels
        {
            get
            {
                if (m_tocLevels == null)
                {
                    m_tocLevels = new Dictionary<int, List<string>>();
                }
                return m_tocLevels;
            }
            set
            {
                m_tocLevels = value;
            }
        }
        /// <summary>
        /// Gets the TOC entry page numbers.
        /// </summary>
        /// <value>The TOC entry page numbers.</value>
        internal List<int> TOCEntryPageNumbers
        {
            get
            {
                if (m_tocEntryPageNumbers == null)
                {
                    m_tocEntryPageNumbers = new List<int>();
                }
                return m_tocEntryPageNumbers;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to use TC fields.
        /// </summary>
        /// <value><c>true</c> if use TC fields; otherwise, <c>false</c>.</value>
        internal bool UseTCFields
        {
            get
            {
                return m_useTCFields;
            }
            set
            {
                m_useTCFields = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentLayouter"/> class.
        /// </summary>
        public DocumentLayouter()
        {
            m_pageResult = new WordToPDFResult();

            m_headerLPHandler = new HeaderFooterLPHandler(this, false /*as header*/ );
            m_footerLPHandler = new HeaderFooterLPHandler(this, true /*as footer*/ );
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Layouts the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="cg">The cg.</param>
        /// <returns></returns>
        public PageCollection Layout(IWordDocument doc)
        {
            if (doc.Sections.Count < 1)
                return null;

            m_docWidget = doc as IWidgetContainer;

            if (m_docWidget == null)
                throw new DLSException("Document can't support IWidgetContainer interface");

            m_currSection = doc.Sections[0];

            IsFirstLayouting = true;
            if (!LayoutPages())
            {
                m_bFirstPageForSection = true;
                IsFirstLayouting = false;
                for (int i = 0, len = m_pages.Count; i < len; i++)
                {
                    Page page = m_pages[i];
                    page.UpdateFieldsNumPages(len);
                }

                // Second pass
                m_currSection = doc.Sections[0];
                m_isFirstPage = true;
                LayoutPages();
                IsFirstLayouting = true;
            }

            return m_pages;
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        public void InitLayoutInfo()
        {
            for (int i =0; i < m_pages.Count; i++)
            {
                m_pages[i].InitLayoutInfo();
            }
        }
        /// <summary>
        /// Draws to image.
        /// </summary>
        /// <param name="startPageIndex">Start index of the page.</param>
        /// <param name="noOfPages">The no of pages.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public Image[] DrawToImage(int startPageIndex, int noOfPages, ImageType imageType, MemoryStream stream)
        {
            if (m_pages.Count < startPageIndex || startPageIndex < 0)
                return null;

            if (m_pages.Count < startPageIndex + noOfPages)
                noOfPages = m_pages.Count - startPageIndex;

            int pageCount = (noOfPages == -1) ? m_pages.Count : (startPageIndex + noOfPages);

            Image[] pageImages = new Image[pageCount];

            for (int i = startPageIndex; i < pageCount; i++)
            {
#if DUMP_IMAGE
                stream = new MemoryStream();
#endif
                Image image = CreateImage(m_pages[i].Setup, imageType, stream, m_imageResolution);
                PageNumber = i+1;
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.PageUnit = GraphicsUnit.Point;
                    
                    graphics.FillRectangle(Brushes.White, new System.Drawing.Rectangle(0, 0, (int)m_pages[i].Setup.PageSize.Width, (int)m_pages[i].Setup.PageSize.Height));
                    DrawingContext dc = new DrawingContext(graphics, GraphicsUnit.Point);
                    DrawPageBorder(i, dc);
                    m_pages[i].Draw(dc);
                    m_pageResult.Pages.Add(new PageResult(image, dc.Hyperlinks, dc.BookmarkHyperlinksList));
                    graphics.Dispose();
                }
                pageImages[i] = image;

#if DUMP_IMAGE
                if (i == 0)
                {
                    using (FileStream fstream = new FileStream("c:\\temp\\doctopdf.emf", FileMode.OpenOrCreate))
                    {
                        stream.WriteTo(fstream);
                    }
                }
#endif
            }
            InitLayoutInfo();
            return pageImages;
        }
        /// <summary>
        /// Draws to image.
        /// </summary>
        /// <param name="startPageIndex">Start index of the page.</param>
        /// <param name="noOfPages">The no of pages.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <returns></returns>
        internal void DrawToImage(int startPageIndex, int noOfPages, ImageType imageType)
        {
            if (m_pages.Count < startPageIndex || startPageIndex < 0)
                return;

            if (m_pages.Count < startPageIndex + noOfPages)
                noOfPages = m_pages.Count - startPageIndex;

            int pageCount = (noOfPages == -1) ? m_pages.Count : (startPageIndex + noOfPages);
            MemoryStream stream = null;
#if DUMP_IMAGE
            stream = new MemoryStream();
#endif
            Image image = CreateImage(m_pages[startPageIndex].Setup, imageType, stream, m_imageResolution);
            PageNumber = startPageIndex + 1;
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.PageUnit = GraphicsUnit.Point;
                
                graphics.FillRectangle(Brushes.White, new System.Drawing.Rectangle(0, 0, (int)m_pages[startPageIndex].Setup.PageSize.Width, (int)m_pages[startPageIndex].Setup.PageSize.Height));
                DrawingContext dc = new DrawingContext(graphics, GraphicsUnit.Point);
                DrawPageBorder(startPageIndex, dc);
                m_pages[startPageIndex].Draw(dc);
                m_pageResult.Pages.Add(new PageResult(image, dc.Hyperlinks, dc.BookmarkHyperlinksList));
                graphics.Dispose();
            }
#if DUMP_IMAGE
            if (startPageIndex == 0)
            {
                using (FileStream fstream = new FileStream("c:\\temp\\doctopdf.emf", FileMode.OpenOrCreate))
                {
                    stream.WriteTo(fstream);
                }
            }
#endif
        }
        /// Draw the image and return Stream
        /// </summary>
        /// <param name="startPageIndex">Start index of the page.</param>
        /// <param name="noOfPages">The no of pages.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public Stream DrawToStream(int startPageIndex, int noOfPages, ImageType imageType, MemoryStream stream)
        {
            if (m_pages.Count < startPageIndex || startPageIndex < 0)
                return null;

            if (m_pages.Count < startPageIndex + noOfPages)
                noOfPages = m_pages.Count - startPageIndex;

            int pageCount = (noOfPages == -1) ? m_pages.Count : (startPageIndex + noOfPages);

            Image[] pageImages = new Image[pageCount];

            for (int i = startPageIndex; i < pageCount; i++)
            {

                stream = new MemoryStream();
                Image image = CreateImage(m_pages[i].Setup, imageType, stream, m_imageResolution);
                PageNumber = i + 1;
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.PageUnit = GraphicsUnit.Point;
                    
                    graphics.FillRectangle(Brushes.White, new System.Drawing.Rectangle(0, 0, (int)m_pages[i].Setup.PageSize.Width, (int)m_pages[i].Setup.PageSize.Height));
                    DrawingContext dc = new DrawingContext(graphics, GraphicsUnit.Point);
                    DrawPageBorder(i, dc);
                    m_pages[i].Draw(dc);
                    m_pageResult.Pages.Add(new PageResult(image, dc.Hyperlinks, dc.BookmarkHyperlinksList));
                    graphics.Dispose();
                }
                pageImages[i] = image;
            }

            return stream;
        }
        /// <summary>
        /// Draw the page border
        /// </summary>
        /// <param name="i">Page number</param>
        /// <param name="dc"></param>
        private void DrawPageBorder(int pageNumber, DrawingContext dc)
        {
            bool drawPageBorder = false;
            switch (m_pages[pageNumber].Setup.PageBordersApplyType)
            {
                case PageBordersApplyType.AllExceptFirstPage:
                    if (pageNumber > 0 && m_pages[pageNumber].Setup.OwnerBase == m_pages[pageNumber - 1].Setup.OwnerBase)
                        drawPageBorder = true;
                    break;
                case PageBordersApplyType.AllPages:
                    drawPageBorder = true;
                    break;
                case PageBordersApplyType.FirstPage:
                    if (pageNumber == 0 || (pageNumber > 0 && m_pages[pageNumber].Setup.OwnerBase != m_pages[pageNumber - 1].Setup.OwnerBase))
                        drawPageBorder = true;
                    break;
            }
            if (drawPageBorder && m_pages[pageNumber].PageWidgets.Count != 0)
                dc.DrawPageBorder(m_pages[pageNumber].Setup, m_pages[pageNumber].PageWidgets[0].Bounds, m_pages[pageNumber].PageWidgets[1].Bounds);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Layouts the pages.
        /// </summary>
        /// <returns></returns>
        private bool LayoutPages()
        {
            // Prepare layout process
            m_bDirty = false;
            m_pages.Clear();
            BookmarkHyperlinks.Clear();
            Bookmarks.Clear();
            m_nextPageIndex = 0;
            // Start layout process
            CreateNewPage();

            Image image = CreateImage(m_currPage.Setup, ImageType.Metafile, null, m_imageResolution);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.PageUnit = GraphicsUnit.Point;
                
                graphics.FillRectangle(Brushes.White, new System.Drawing.Rectangle(0, 0, (int)m_currPage.Setup.PageSize.Width, (int)m_currPage.Setup.PageSize.Height));
                DrawingContext dc = new DrawingContext(graphics, GraphicsUnit.Point);
                m_dc = dc;

                LayoutHeaderFooter();
                ClearFields();
                m_bFirstPageForSection = false;
                // Layout page columns
                Layouter clsLayouter = new Layouter();
                clsLayouter.LeafLayoutAfter += new Layouter.LeafLayoutEventHandler(Layouter_LeafLayoutAfter);
                clsLayouter.Layout(m_docWidget, this, m_dc, m_pages.Count);
                clsLayouter.LeafLayoutAfter -= new Layouter.LeafLayoutEventHandler(Layouter_LeafLayoutAfter);
                graphics.Dispose();
            }
            // Clears the lists collection.
            (m_docWidget as WordDocument).ClearLists();

            return !m_bDirty;
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateNewPage()
        {
            m_currPage = new Page(CurrentSection, m_nextPageIndex);
            //Reset the footnote id while creating a new page
            if (m_bisNeedToRestartFootnote)
            {
                m_footnoteIDRestartEachPage = 1;
            }
            //Update Page number of First Page for the Current section
            if ((m_currSection.PageSetup.RestartPageNumbering
                && m_currSection.PreviousSibling != null
                && m_bFirstPageForSection)
                || (m_nextPageIndex == 0
                && m_currSection.PageSetup.PageStartingNumber > 0))
            {
                m_currPage.Number = m_currSection.PageSetup.PageStartingNumber - 1;
                m_nextPageIndex = m_currPage.Number;
            }
            //Clears the textbox bounds & wrapping style values in the lists
            m_FloatingItems.Clear();
            m_pages.Add(m_currPage);
            m_nextPageIndex++;
            m_columnsWidth = 0f;
            m_columnIndex = 0;
        }

        /// <summary>
        /// Creates the new section in the current page.
        /// </summary>
        private void CreateNewSection()
        {
            m_columnIndex = 0;
            m_columnsWidth = 0;
            m_columnHeight.Clear();
            m_columnHasBreakItem.Clear();
            m_prevColumnsWidth.Clear();
        }

        /// <summary>
        /// Checks the section break.
        /// </summary>
        /// <returns></returns>
        private bool CheckSectionBreak()
        {
            bool PageEnd = false;
            if (m_bFirstPageForSection)
            {
                m_usedHeight += m_sectionFixedHeight;
                switch (CurrentSection.BreakCode)
                {
                    case SectionBreakCode.NewPage:
                        PageEnd = true;
                        m_sectionFixedHeight = 0;
                        //Reset the footnote ID while intialize a new section
                        m_footnoteIDRestartEachSection = 1;
                        break;
                    case SectionBreakCode.Oddpage:
                        if (!IsEvenPage  && CurrentSection.PageSetup.PageStartingNumber == 0) //Skip to create a new page when the section have page number format
                            CreateNewPage();
                        PageEnd = true;
                        m_sectionFixedHeight = 0;
                        m_footnoteIDRestartEachSection = 1;
                        break;
                    case SectionBreakCode.EvenPage:
                        if (IsEvenPage && CurrentSection.PageSetup.PageStartingNumber == 0)
                            CreateNewPage();
                        PageEnd = true;
                        m_sectionFixedHeight = 0;
                        m_footnoteIDRestartEachSection = 1;
                        break;
                    case SectionBreakCode.NoBreak:
                        if (CurrentSection.PageSetup.Orientation == CurrentSection.Document.Sections[CurrentSection.Document.Sections.IndexOf(CurrentSection) - 1].PageSetup.Orientation
                             && (CurrentPage.FootnoteWidgets.Count == 0))
                        {
                            if (CurrentSection.Document.Sections[CurrentSection.Document.Sections.IndexOf(CurrentSection) - 1].Columns.Count == 1)
                            {
                                int cnt = CurrentPage.PageWidgets.Count;
                                //Get the current section index
                                int currentSectionIndex = CurrentSection.Document.Sections.IndexOf(CurrentSection);
                                m_usedHeight = CurrentPage.PageWidgets[cnt - 1].Bounds.Bottom - m_pageTop;
                                //Add the footnote height into the used height if availble in the previous section.
                                for (int i = 0; i < CurrentPage.FootnoteWidgets.Count; i++)
                                {
                                    if (currentSectionIndex-1 == CurrentPage.FootNoteSectionIndex[i])
                                        m_usedHeight += CurrentPage.FootnoteWidgets[i].Bounds.Height;
                                }
                                //Add the endnote height into the used height if availble in the previous section.
                                for (int i = 0; i < CurrentPage.EndnoteWidgets.Count; i++)
                                {
                                    if (currentSectionIndex - 1 == CurrentPage.EndNoteSectionIndex[i])
                                        m_usedHeight += CurrentPage.EndnoteWidgets[i].Bounds.Height;
                                }
                                m_sectionFixedHeight = m_usedHeight;
                            }
                            if (!m_createNewPage)
                            {
                                CreateNewSection();
                                m_bFirstPageForSection = false;
                                m_createNewPage = false;
                                PageEnd = false;
                            }

                            else
                                PageEnd = true;
                        }
                        else
                            PageEnd = true;
                        m_footnoteIDRestartEachSection = 1;
                        break;
                }
            }
            return (PageEnd);
        }

        /// <summary>
        /// Handles the page break.
        /// </summary>
        /// <returns></returns>
        private void HandlePageBreak()
        {
            if (CurrentPage.PageWidgets.Count > 2)
            {
                int widgetCount = CurrentPage.PageWidgets.Count;
                LayoutedWidget ltWidget = CurrentPage.PageWidgets[widgetCount - 1] as LayoutedWidget;
                while (ltWidget.ChildWidgets.Count != 0)
                {
                    ltWidget = ltWidget.ChildWidgets[ltWidget.ChildWidgets.Count - 1] as LayoutedWidget;
                }
                if (ltWidget.Widget is Syncfusion.DocIO.DLS.Break)
                {
                    if ((ltWidget.Widget as Syncfusion.DocIO.DLS.Break).BreakType == BreakType.PageBreak)
                        m_createNewPage = true;
                }
            }
        }
     
        /// <summary>
        /// 
        /// </summary>
        private void LayoutHeaderFooter()
        {
            m_bIsLayoutingHeaderFooter = true;
            IWidgetContainer hWidget = GetCurrentHeader();
            IWidgetContainer fWidget = GetCurrentFooter();

            Layouter headerLayouter = new Layouter();
            headerLayouter.LeafLayoutAfter += new Layouter.LeafLayoutEventHandler(Layouter_LeafLayoutAfter);
            headerLayouter.Layout(hWidget, m_headerLPHandler, m_dc, m_pages.Count);
            headerLayouter.LeafLayoutAfter -= new Layouter.LeafLayoutEventHandler(Layouter_LeafLayoutAfter);

            Layouter footerLayouter = new Layouter();
            footerLayouter.LeafLayoutAfter += new Layouter.LeafLayoutEventHandler(Layouter_LeafLayoutAfter);
            footerLayouter.Layout(fWidget, m_footerLPHandler, m_dc, m_pages.Count);
            footerLayouter.LeafLayoutAfter -= new Layouter.LeafLayoutEventHandler(Layouter_LeafLayoutAfter);
            m_bIsLayoutingHeaderFooter = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private WTextBody GetCurrentHeader()
        {
            WHeadersFooters hfs = CurrentSection.HeadersFooters;
            WSection prevSection = null;
            WTextBody header = GetHeaderFooter(CurrentSection, hfs.OddHeader);
            if (hfs.LinkToPrevious)
            {
                IEntity ent = CurrentSection as IEntity;
                if (ent != null && ent.PreviousSibling is WSection)
                {
                    int index = CurrentSection.Document.Sections.IndexOf(CurrentSection);
                    while (index > 0)
                    {
                        prevSection = CurrentSection.Document.Sections[index - 1];
                        header = GetHeaderFooter(prevSection, prevSection.HeadersFooters.OddHeader);
                        if (prevSection.HeadersFooters.LinkToPrevious)
                        {
                            index--;
                            continue;
                        }
                        else
                            break;
                    }
                }
                if (CurrentSection.PageSetup.DifferentOddAndEvenPages && IsEvenPage)
                {
                    header = GetHeaderFooter(prevSection, prevSection.HeadersFooters.EvenHeader);
                }

                if (CurrentSection.PageSetup.DifferentFirstPage && m_bFirstPageForSection)
                {
                    header = GetHeaderFooter(prevSection, prevSection.HeadersFooters.FirstPageHeader);
                }
            }
            else
            {
                if (CurrentSection.PageSetup.DifferentOddAndEvenPages && IsEvenPage)
                {
                    header = GetHeaderFooter(CurrentSection, hfs.EvenHeader);
                }

                if (CurrentSection.PageSetup.DifferentFirstPage && m_bFirstPageForSection)
                {
                    header = GetHeaderFooter(CurrentSection, hfs.FirstPageHeader);
                }
            }

            return header;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private WTextBody GetCurrentFooter()
        {
            WHeadersFooters hfs = CurrentSection.HeadersFooters;
            WSection prevSection = null;
            WTextBody footer = GetHeaderFooter(CurrentSection, hfs.OddFooter);
            if (hfs.LinkToPrevious)
            {
                IEntity ent = CurrentSection as IEntity;
                if (ent != null && ent.PreviousSibling is WSection)
                {
                    int index = CurrentSection.Document.Sections.IndexOf(CurrentSection);
                    while (index > 0)
                    {
                        prevSection = CurrentSection.Document.Sections[index - 1];
                        footer = GetHeaderFooter(prevSection, prevSection.HeadersFooters.OddFooter);
                        if (prevSection.HeadersFooters.LinkToPrevious)
                        {
                            index--;
                            continue;
                        }
                        else
                            break;
                    }

                }
                if (CurrentSection.PageSetup.DifferentOddAndEvenPages && IsEvenPage)
                {
                    footer = GetHeaderFooter(prevSection, prevSection.HeadersFooters.EvenFooter);
                }

                if (CurrentSection.PageSetup.DifferentFirstPage && m_bFirstPageForSection)
                {
                    footer = GetHeaderFooter(prevSection, prevSection.HeadersFooters.FirstPageFooter);
                }
            }
            else
            {
                if (CurrentSection.PageSetup.DifferentOddAndEvenPages && IsEvenPage)
                {
                    footer = GetHeaderFooter(CurrentSection, hfs.EvenFooter);
                }

                if (CurrentSection.PageSetup.DifferentFirstPage && m_bFirstPageForSection)
                {
                    footer = GetHeaderFooter(CurrentSection, hfs.FirstPageFooter);
                }
            }

            return footer;
        }
        /// <summary>
        /// Gets the header footer.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="headerFooter">The header footer.</param>
        /// <returns></returns>
        private WTextBody GetHeaderFooter(IWSection section, HeaderFooter headerFooter)
        {
            WTextBody txtBody = headerFooter;
            WSection prevSection = null;
            if (headerFooter.LinkToPrevious)
            {
                IEntity ent = section as IEntity;
                if (ent != null && ent.PreviousSibling is WSection)
                {
                    int index = section.Document.Sections.IndexOf(section);
                    while (index > 0)
                    {
                        prevSection = section.Document.Sections[index - 1];
                        txtBody = prevSection.HeadersFooters[headerFooter.Type];
                        if (prevSection.HeadersFooters[headerFooter.Type].LinkToPrevious)
                        {
                            index--;
                            continue;
                        }
                        else
                            break;
                    }
                }
            }
            return txtBody;
        }
        /// <summary>
        /// 
        /// </summary>
        private void OnNextSection()
        {
            m_bFirstPageForSection = true;
            m_sectionNewPage = false;
            m_isContinuousSectionLayouted = false;
            m_columnIndex = 0;
            m_columnsWidth = 0;
            m_totalHeight = 0;
        }
        #endregion

        #region Implementation TOC field updating
        /// <summary>
        /// Gets the TOC entry page numbers.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        internal List<int> GetTOCEntryPageNumbers(WordDocument doc)
        {
            m_docWidget = doc as IWidgetContainer;

            m_currSection = doc.Sections[0];

            m_UpdatingToc = true;
            IsFirstLayouting = true;
            //Layout the document.
            LayoutPages();

            return TOCEntryPageNumbers;
        }
        #endregion

        #region Implementation Update Page fields
        /// <summary>
        /// Updates page fields.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        internal void UpdatePageFields(WordDocument doc)
        {
            m_docWidget = doc as IWidgetContainer;

            m_currSection = doc.Sections[0];

            m_UpdatingPageFields = true;
            IsFirstLayouting = true;
            //Layout the document.
            LayoutPages();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private Image CreateImage(WPageSetup pageSetup, ImageType imageType, MemoryStream stream, int resolution)
        {
            int width = (int)UnitsConvertor.Instance.ConvertToPixels(pageSetup.PageSize.Width, PrintUnits.Point);
            int height = (int)UnitsConvertor.Instance.ConvertToPixels(pageSetup.PageSize.Height, PrintUnits.Point);

            Image result;
            switch (imageType)
            {
                case ImageType.Bitmap:
                    result = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                    (result as Bitmap).SetResolution(resolution, resolution);
                    break;

                case ImageType.Metafile:
                    if (stream == null)
                        stream = new MemoryStream();

                    using (Bitmap bitmap = new Bitmap((int)width, (int)height))
                    {
                        bitmap.SetResolution(resolution, resolution);
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            IntPtr hdc = g.GetHdc();
                            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, width, height);
                            result = new System.Drawing.Imaging.Metafile(stream, hdc, rect, MetafileFrameUnit.Pixel, EmfType.EmfPlusDual);
                            g.ReleaseHdc(hdc);
                            g.Dispose();
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("imageType");
            }
            return result;
        }
        /// <summary>
        /// Determines whether the layouted widget is TOC paragraph.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <returns>
        /// 	<c>true</c> if the specified widget is TOC paragraph; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTOCParagraph(IWidget widget)
        {
            bool isTOCParagraph = false;
            WParagraph para = null;
            if (widget is WParagraph && !string.IsNullOrEmpty((widget as WParagraph).Text))
            {
                para = widget as WParagraph;
                isTOCParagraph = true;
            }
            else if (widget is SplitWidgetContainer && (widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
            {
                para = (widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
                if (!string.IsNullOrEmpty(para.Text))
                    isTOCParagraph = true;
            }
            //Removes the duplicate page number entry.
            if (para != null && para == m_prevTocParagraph)
                TOCEntryPageNumbers.RemoveAt(TOCEntryPageNumbers.Count - 1);
            m_prevTocParagraph = para;
            return isTOCParagraph;
        }
        #endregion

        #region Events
        /// <summary>
        /// Layouter_s the leaf layout after.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="ltWidget">The lt widget.</param>
        private void Layouter_LeafLayoutAfter(object sender, LayoutedWidget ltWidget)
        {
            IWField field = ltWidget.Widget as IWField;
            if (field != null)
            {
                if (field.FieldType == FieldType.FieldPage)
                {
                    //Get page number based on PageNumberFormat
                    string pageNumber = CurrentSection.PageSetup.GetNumberFormatValue((byte)CurrentSection.PageSetup.PageNumberStyle, CurrentPage.Number + 1);
                    ltWidget.TextTag = pageNumber;
                    if (m_UpdatingPageFields)
                        (field as WField).UpdateFieldResult(pageNumber);
                    WTextRange tr = field as WTextRange;
                    WCharacterFormat charFormat = (field as WField).GetCharacterFormat();
                    if (tr != null)
                    {
                        tr.Text = ltWidget.TextTag;
                    }
                    SizeF size = DrawingContext.MeasureString(tr.Text, charFormat.Font, null, charFormat,false);
                    ltWidget.Bounds = new RectangleF(ltWidget.Bounds.Location, size);
                }
                else if (field.FieldType == FieldType.FieldNumPages)
                {
                    CurrentPage.AddCachedFields(field);
                    m_bDirty = true;
                }
                else if (field.FieldType == FieldType.FieldTOCEntry && !m_bIsLayoutingHeaderFooter && m_UpdatingToc && UseTCFields)
                {
                    TOCEntryPageNumbers.Add(CurrentPage.Number + 1);
                }
            }
            else if (m_UpdatingToc && !m_bIsLayoutingHeaderFooter && IsTOCParagraph(ltWidget.Widget))
            {
                TOCEntryPageNumbers.Add(CurrentPage.Number + 1);
            }
        }
        #endregion

        #region ILayoutProcessHandler members
        /// <summary>
        /// Gets the next area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="isContinuousSection">The isContinuousSection.</param>
        /// <returns></returns>
        bool ILayoutProcessHandler.GetNextArea(out RectangleF area, ref int columnIndex, ref bool isContinuousSection, bool isSplittedWidget,ref float topMargin)
        {
            int colCount = CurrentSection.Columns.Count;
            if (!isSplittedWidget)
            {
                m_columnIndex = 0;
                m_columnsWidth = 0;
            }
            // Is second pass and no columns 
            bool bPageEnd = (colCount == 0 && m_columnIndex > 0);
            // Or column index greate that max column index
            bPageEnd = bPageEnd || (colCount > 0 && m_columnIndex > colCount - 1);

            //Checks Section Break of the Current Section 
            bool temp = CheckSectionBreak();
            bPageEnd = bPageEnd || temp;

            //Handles page break
            HandlePageBreak();

            if (((bPageEnd && (m_usedHeight == m_clientHeight || !isContinuousSection)) || m_createNewPage))
            {
                CreateNewPage();
                LayoutHeaderFooter();
                ClearFields();
            }
            //Gets Section client area.
            area = GetSectionClientArea(isSplittedWidget);
            topMargin = m_pageTop;
            //Checks Section Break of the Next Section 
            isContinuousSection = CheckNextSectionBreakType(isContinuousSection);
            //Update the column width
            if (isContinuousSection && !m_isContinuousSectionLayouted && CurrentSection.Columns.Count > 1
                && m_columnIndex == 0 && isSplittedWidget
               && !(CurrentSection.PageSetup.EqualColumnWidth || IsEqualColumnWidth()))
            {
                float columnWidth = 0;
                for (int i = 0; i < CurrentSection.Columns.Count; i++)
                    columnWidth += CurrentSection.Columns[i].Width;
                area.Width = columnWidth;
                m_totalColumnWidth = columnWidth;
            }
            //Sets the fixed height for continuous section
            if (m_isContinuousSectionLayouted)
            {
                if (m_columnIndex == 0)
                    //Get fixed height for continuous section
                    m_sectionFixedHeight = GetRequiredHeightForContinuousSection();
                isContinuousSection = false;
                if(area.Height > m_sectionFixedHeight)
                    area.Height = m_sectionFixedHeight;
            }
            columnIndex = m_columnIndex;
            m_columnIndex++;
            return !area.Equals(RectangleF.Empty);
        }
        /// <summary>
        /// Get Section Client Area
        /// </summary>
        /// <returns></returns>
        private RectangleF GetSectionClientArea(bool isSplittedWidget)
        {
            //Gets column client area.
            RectangleF area = GetColumnClientArea(isSplittedWidget);
            //Gets first line height of the current section
            float height = GetFirstLineHeight();
            if (area.Height < height && CurrentSection.BreakCode==SectionBreakCode.NoBreak)
            {
                CreateNewPage();
                LayoutHeaderFooter();
                ClearFields();
                //Gets column client area.
                area = GetColumnClientArea(isSplittedWidget);
            }
            return area;
        }
        /// <summary>
        /// Gets column client area.
        /// </summary>
        /// <returns></returns>
        private RectangleF GetColumnClientArea(bool isSplittedWidget)
        {
            RectangleF area = RectangleF.Empty;
            if (CurrentSection.BreakCode == SectionBreakCode.NoBreak && !m_isFirstPage)
            {
                if (m_sectionNewPage)
                {
                    area = CurrentPage.GetColumnArea(m_columnIndex, ref m_columnsWidth);
                    m_pageTop = area.Y;
                    m_clientHeight = area.Height;
                }
                else
                {
                    //Get column area for Continuous section
                    area = CurrentPage.GetSectionArea(m_columnIndex, ref m_columnsWidth, m_isContinuousSectionLayouted,isSplittedWidget);
                }
                area.Y = m_pageTop + m_usedHeight;
                area.Height = m_clientHeight - m_usedHeight;
            }
            else
            {
                area = CurrentPage.GetColumnArea(m_columnIndex, ref m_columnsWidth);
                m_pageTop = area.Y;
                m_sectionNewPage = false;
                m_isFirstPage = false;
                m_clientHeight = area.Height;
            }
            //Get the current section index
            int currentSectionIndex = CurrentSection.Document.Sections.IndexOf(CurrentSection);
            //Update the end note bottom position as new section Y position.
            for (int i = 0; i < CurrentPage.EndnoteWidgets.Count; i++)
            {
                if (currentSectionIndex - 1 == CurrentPage.EndNoteSectionIndex[i])
                    area.Y = CurrentPage.EndnoteWidgets[i].Bounds.Bottom;
            }
                return area;
        }
        /// <summary>
        /// Clear Fields on new page.
        /// </summary>
        private void ClearFields()
        {
            m_columnHeight.Clear();
            m_lineHeights.Clear();
            m_columnHasBreakItem.Clear();
            m_prevColumnsWidth.Clear();
            m_totalHeight = 0;
            m_usedHeight = 0;
            m_sectionFixedHeight = 0;
            m_sectionNewPage = true;
            m_createNewPage = false;
            m_isContinuousSectionLayouted = false;
            m_bFirstPageForSection = false;
        }

        /// <summary>
        /// Get Required Height for Continuous Section.
        /// </summary>
        private float GetRequiredHeightForContinuousSection()
        {
            m_FloatingItems = m_prevFloatingItems;
            m_prevFloatingItems.Clear();
            int colCount = CurrentSection.Columns.Count;
            float height = 0;
            //Handles for columns of equal width
            if (CurrentSection.PageSetup.EqualColumnWidth || IsEqualColumnWidth())
            {
                foreach (float lineHeight in m_lineHeights)
                {
                    height += lineHeight;
                    if (height >= m_totalHeight / (colCount - m_columnIndex))
                        break;
                }
            }
            //Handles for columns of not-equal width
            else
            {
                height = GetRequiredHeightForUnEqualColumns();
            }
            if (m_absolutePositionedTableHeights != null)
            {
                foreach (float tableHeight in m_absolutePositionedTableHeights)
                {
                    height += tableHeight;
                }
                m_absolutePositionedTableHeights.Clear();
            }
            return (float)Math.Ceiling(height);
        }
        /// <summary>
        /// Get Required Height for UnEqual columns
        /// </summary>
        /// <returns></returns>
        private float GetRequiredHeightForUnEqualColumns()
        {
            float height = 0;
            //Get Column Index which have a minimum column Width
            int minColumnIndex = GetColumnIndexForMinColumnWidth();
            //Get Column Index which have a maximum column Width
            int maxColumnIndex = GetColumnIndexForMaxColumnWidth();
            //Get first line height of the current section
            float lineHeight = GetFirstLineHeight();
            //Differnce between the maximum and minimum columns width
            float diffColumnWidth = CurrentSection.Columns[maxColumnIndex].Width - CurrentSection.Columns[minColumnIndex].Width;
            //Calculate Required column height if the minimum column width is greater than the difference.
            if (diffColumnWidth < CurrentSection.Columns[minColumnIndex].Width)
            {
                float tempHeight = ((m_totalColumnWidth * m_totalHeight) / CurrentSection.Columns[minColumnIndex].Width) - m_totalHeight;
                float remainingColumnHeight = 0;
                for (int i = 0; i < CurrentSection.Columns.Count; i++)
                {
                    remainingColumnHeight += (((2 * CurrentSection.Columns.Count) - 1) * Math.Abs((CurrentSection.Columns[i].Width - CurrentSection.Columns[minColumnIndex].Width)));
                }
                height = Math.Abs(tempHeight - ((CurrentSection.Columns.Count - 1) * remainingColumnHeight));
            }
            //Calculate required column height if the minimum column width is greater than the difference
            else
            {
                if (maxColumnIndex < minColumnIndex)
                {
                    height = ((diffColumnWidth * m_totalHeight) / m_totalColumnWidth)
                    + ((CurrentSection.Columns.Count * CurrentSection.Columns[maxColumnIndex].Width)
                    / (diffColumnWidth * CurrentSection.Columns[minColumnIndex].Width));
                }
                else
                {
                    height = ((diffColumnWidth * m_totalHeight) / m_totalColumnWidth)
                    + ((diffColumnWidth * CurrentSection.Columns[minColumnIndex].Width)
                    / (CurrentSection.Columns.Count * CurrentSection.Columns[maxColumnIndex].Width));
                }
                float temp1 = CurrentSection.Columns[maxColumnIndex].Width / CurrentSection.Columns[minColumnIndex].Width;
                string[] split = temp1.ToString().Split('.');
                float temp2 = 0;
                if (split.Length > 1)
                    temp2 = (float)(Convert.ToDouble(split[1]) * m_totalHeight) / (CurrentSection.Columns.Count * (float)Math.Pow(10, split[1].Length));
                if (maxColumnIndex < minColumnIndex)
                {
                    height += temp2;
                    height += lineHeight;
                }
                else
                {
                    height -= temp2;
                    //Remove one line space from the required height if the minimum column Index is greater than the maximum column Index
                    height -= lineHeight;
                }
            }
            return height;
        }
        /// <summary>
        /// Get first line height of the section
        /// </summary>
        /// <returns></returns>
        private float GetFirstLineHeight()
        {
            float height = 0;
            //Gets first item height of the current section
            if (CurrentSection.Body.Items.FirstItem is WParagraph)
            {
                WParagraph paragraph = CurrentSection.Body.Items.FirstItem as WParagraph;
                height = m_dc.MeasureString(" ", paragraph.BreakCharacterFormat.Font, null, paragraph.BreakCharacterFormat,false).Height;
            }
            else if (CurrentSection.Body.Items.FirstItem is WTable)
            {
                WTable table = CurrentSection.Body.Items.FirstItem as WTable;
                if (table.Rows[0].Cells[0].LastParagraph != null)
                    height = m_dc.MeasureString(" ", table.Rows[0].Cells[0].LastParagraph.BreakCharacterFormat.Font, null, table.Rows[0].Cells[0].LastParagraph.BreakCharacterFormat,false).Height;
                //Gets first row height of the table
                float rowHeight = (table.Rows[0].Height >= 0 ? table.Rows[0].Height : -1 * table.Rows[0].Height);
                if (table.Rows[0].HeightType == TableRowHeightType.Exactly && rowHeight < height)
                    height = rowHeight;
                else if (table.Rows[0].HeightType == TableRowHeightType.AtLeast && rowHeight > height)
                    height = rowHeight;
            }
            return height;
        }
        /// <summary>
        /// Check whether the current section have multiple columns with equal column width
        /// </summary>
        /// <returns></returns>
        private bool IsEqualColumnWidth()
        {
            float colWidth = CurrentSection.Columns[0].Width;
            bool isEqualColumnWidth = true;
            //Checks whether columns have equal width or not
            foreach (Column column in CurrentSection.Columns)
            {
                if (colWidth != column.Width)
                {
                    isEqualColumnWidth = false;
                    break;
                }
                colWidth = column.Width;
            }
            return isEqualColumnWidth;
        }
        /// <summary>
        /// Checks Next Section BreakType.
        /// </summary>
        /// <param name="isContinuousSection">The isContinuousSection.</param>
        /// <returns></returns>
        private bool CheckNextSectionBreakType(bool isContinuousSection)
        {
            int index = CurrentSection.Document.Sections.IndexOf(CurrentSection);
            if (CurrentSection.Document.Sections.Count - 1 > index)
            {
                IWSection nextSection = CurrentSection.Document.Sections[index + 1];
                if (nextSection.BreakCode == SectionBreakCode.NoBreak && CurrentSection.Columns.Count > 1)
                {
                    isContinuousSection = true;
                    m_prevFloatingItems = m_FloatingItems;
                    if (m_columnIndex == 0 && m_sectionIndex != index)
                    {
                        m_createNewPage = false;
                        m_isContinuousSectionLayouted = false;
                        m_sectionIndex = index;
                    }
                }
                else
                {
                    isContinuousSection = false;
                    m_isContinuousSectionLayouted = false;
                }
            }
            else
            {
                isContinuousSection = false;
                m_isContinuousSectionLayouted = false;
            }
            return isContinuousSection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        void ILayoutProcessHandler.PushLayoutedWidget(LayoutedWidget ltWidget, RectangleF layoutArea, bool isNeedToRestartFootnote, bool isNeedToRestartEndnote)
        {
            UpdateCellBorderInTableLayoutedWidget(ltWidget);
            //Push Footnote layouted widgets
            if (CurrentPage.FootnoteWidgets.Count > 0)
            {
                if (CurrentSection.PageSetup.FootnotePosition == FootnotePosition.PrintImmediatelyBeneathText)
                {
                    FootnotePushLayoutedWidget(ltWidget.Bounds);
                }
                else
                {
                    FootnotePushLayoutedWidget(layoutArea);
                }
            }
            //Push Endnote layouted widgets
            if (CurrentPage.EndnoteWidgets.Count > 0)
            {
                EndnotePushLayoutedWidget(ltWidget.Bounds, ltWidget);
            }
            m_bisNeedToRestartFootnote = isNeedToRestartFootnote;
            m_bisNeedToRestartEndnote = isNeedToRestartFootnote;
            CurrentPage.PageWidgets.Add(ltWidget);
        }
        /// <summary>
        /// Checks and updates Cell Border in Table Layouted Widget.
        /// </summary>
        /// <param name="ltWidget"></param>
        private void UpdateCellBorderInTableLayoutedWidget(LayoutedWidget ltWidget)
        {
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                LayoutedWidget m_ltWidget = ltWidget.ChildWidgets[i];
                if (m_ltWidget != null)
                {
                    if (((m_ltWidget.Widget is WSection)
                        || ((m_ltWidget.Widget is SplitWidgetContainer) && ((m_ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WSection)))
                        && m_ltWidget.ChildWidgets.Count > 0 )
                    {
                        LayoutedWidget lastltWidget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];
                        if (lastltWidget.Widget is WTable && lastltWidget.ChildWidgets.Count>0)
                        {
                            LayoutedWidget lastRowltWidget = lastltWidget.ChildWidgets[lastltWidget.ChildWidgets.Count - 1];
                            UpdateCellBottomBorder(lastRowltWidget);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates Cell bottom border.
        /// </summary>
        /// <param name="ltWidget"></param>
        private void UpdateCellBottomBorder(LayoutedWidget ltWidget)
        {
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                LayoutedWidget m_ltWidget = ltWidget.ChildWidgets[i];
                if ((m_ltWidget != null) && (m_ltWidget.Widget is WTableCell))
                {
                    (ltWidget.ChildWidgets[i].Widget.LayoutInfo as WTableCell.LayoutCellInfo).SkipBottomBorder = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stWidgetContainer"></param>
        /// <param name="state"></param>
        /// <param name="ltWidget"></param>
        /// <param name="isLayoutedWidgetNeedToPushed"></param>
        bool ILayoutProcessHandler.HandleSplittedWidget(SplitWidgetContainer stWidgetContainer, LayoutState state, LayoutedWidget ltWidget, ref bool isLayoutedWidgetNeedToPushed)
        {
            if (stWidgetContainer == null)
                throw new ArgumentNullException("stWidgetContainer");

            if (stWidgetContainer.Count < 1)
                throw new DLSException("Split widget container (document) must contains at last one child element!");

            IWidget widget = stWidgetContainer[0];
            IWSection nextSection = widget as IWSection;

            //Checks whether to continue layouting with next section
            bool isContinueNextSection = IsContinueLayoutingNextSection(nextSection, ltWidget, isLayoutedWidgetNeedToPushed);
            if (!isContinueNextSection && nextSection != null && isLayoutedWidgetNeedToPushed)
                return false;

            if (nextSection == null)
            {
                SplitWidgetContainer swContainer = widget as SplitWidgetContainer;
                if (swContainer != null)
                {
                    nextSection = swContainer.RealWidgetContainer as IWSection;
                }
            }

            //Handles column break and page break
            isLayoutedWidgetNeedToPushed = HandleColumnAndPageBreakInLayoutedWidget(ltWidget, isLayoutedWidgetNeedToPushed, isContinueNextSection);

            if (nextSection == null)
                throw new DLSException("Child of SplitWidgetContainer object can't support ISecton interface!");

            if (m_currSection != nextSection)
            {
                m_currSection = nextSection;
                OnNextSection();
            }

            return true;
        }

        /// <summary>
        /// Checks whether to continue layouting with next section.
        /// </summary>
        /// <returns>If true; Then continue layouting with the next section. If false; Then continue with current section</returns>
        private bool IsContinueLayoutingNextSection(IWSection nextSection, LayoutedWidget ltWidget, bool isLayoutedWidgetNeedToPushed)
        {
            bool isContinueNextSection = false;
            if (nextSection != null && isLayoutedWidgetNeedToPushed)
            {
                isLayoutedWidgetNeedToPushed = false;
                m_isContinuousSectionLayouted = true;
                float width = m_columnsWidth;
                m_columnIndex = 0;
                m_columnsWidth = 0;
                for (int i = 0; i < m_columnHasBreakItem.Count; i++)
                {
                    if (m_columnHasBreakItem[i])
                    {
                        m_columnIndex = i + 1;
                        m_columnsWidth = m_prevColumnsWidth[i];
                        isLayoutedWidgetNeedToPushed = true;
                        isContinueNextSection = true;
                    }
                }
                if (isContinueNextSection)
                {
                    m_columnHeight.Insert(m_columnHeight.Count, ltWidget.Bounds.Height);
                    m_columnHasBreakItem.Insert(m_columnHasBreakItem.Count, false);
                    m_prevColumnsWidth.Insert(m_prevColumnsWidth.Count, width);
                    UpdateSectionHeight();
                }
            }
            else if (m_columnIndex == CurrentSection.Columns.Count && isLayoutedWidgetNeedToPushed)
            {
                isLayoutedWidgetNeedToPushed = true;
                isContinueNextSection = true;
                float width = m_columnsWidth;
                m_columnHeight.Insert(m_columnHeight.Count, ltWidget.Bounds.Height);
                m_columnHasBreakItem.Insert(m_columnHasBreakItem.Count, false);
                m_prevColumnsWidth.Insert(m_prevColumnsWidth.Count, width);
                UpdateSectionHeight();
            }
            return isContinueNextSection;
        }

        /// <summary>
        /// Handle Column Break and Page Break in the Continuous section.
        /// </summary>
        /// <returns>If true; Then push all the layouted widgets. If false; Then not push the layouted widgets</returns>
        private bool HandleColumnAndPageBreakInLayoutedWidget(LayoutedWidget ltWidget, bool isLayoutedWidgetNeedToPushed, bool isContinueNextSection)
        {
            if (CurrentPage.PageWidgets.Count >= 2 && isLayoutedWidgetNeedToPushed && !m_isContinuousSectionLayouted)
            {
                isLayoutedWidgetNeedToPushed = false;
                LayoutedWidget lastWidget = ltWidget;
                while (lastWidget.ChildWidgets.Count != 0)
                {
                    lastWidget = lastWidget.ChildWidgets[lastWidget.ChildWidgets.Count - 1] as LayoutedWidget;
                }
                if (lastWidget.Widget is Syncfusion.DocIO.DLS.Break)
                {
                    if ((lastWidget.Widget as Syncfusion.DocIO.DLS.Break).BreakType == BreakType.PageBreak || (lastWidget.Widget as Syncfusion.DocIO.DLS.Break).BreakType == BreakType.ColumnBreak)
                    {
                        isLayoutedWidgetNeedToPushed = true;
                        m_columnHeight.Insert(m_columnIndex - 1, ltWidget.Bounds.Height);
                        m_columnHasBreakItem.Insert(m_columnIndex - 1, true);
                        m_prevColumnsWidth.Insert(m_columnIndex - 1, m_columnsWidth);
                        m_createNewPage = (lastWidget.Widget as Syncfusion.DocIO.DLS.Break).BreakType == BreakType.PageBreak;
                    }
                }
                else
                {
                    m_columnHeight.Insert(m_columnIndex - 1, ltWidget.Bounds.Height);
                    m_columnHasBreakItem.Insert(m_columnIndex - 1, false);
                    m_prevColumnsWidth.Insert(m_columnIndex - 1, m_columnsWidth);
                }
            }
            else if (!isContinueNextSection)
                isLayoutedWidgetNeedToPushed = false;
            if (CurrentSection.Columns.Count > 1 && m_columnIndex == CurrentSection.Columns.Count)
                isLayoutedWidgetNeedToPushed = true;
            return isLayoutedWidgetNeedToPushed;
        }

        /// <summary>
        /// Update Section Height.
        /// </summary>
        private void UpdateSectionHeight()
        {
            m_sectionFixedHeight = 0;
            foreach (float colHeight in m_columnHeight)
            {
                m_sectionFixedHeight = Math.Max(m_sectionFixedHeight, Math.Min(m_clientHeight, colHeight));
            }
            if (m_usedHeight + m_sectionFixedHeight + 10 >= m_clientHeight)
                m_createNewPage = true;
        }

        /// <summary>
        /// Handle Layouted Widget.
        /// </summary>
        /// <param name="ltWidget"></param>
        void ILayoutProcessHandler.HandleLayoutedWidget(LayoutedWidget ltWidget)
        {
            m_totalHeight += ltWidget.Bounds.Height;
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                GetLinesHeight(ltWidget.ChildWidgets[i]);
            }
        }
        /// <summary>
        /// Get Lines Height.
        /// </summary>
        /// <param name="ltWidget"></param>
        private void GetLinesHeight(LayoutedWidget ltWidget)
        {
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                LayoutedWidget childWidget = ltWidget.ChildWidgets[i];
                if ((childWidget.Widget is Syncfusion.DocIO.DLS.WParagraph)
                    || (i == 0 && (childWidget.Widget is SplitWidgetContainer)
                    && (childWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
                    || childWidget.Widget is Syncfusion.DocIO.DLS.WTable)
                {
                    if (childWidget.Widget is Syncfusion.DocIO.DLS.WTable && (childWidget.Widget as WTable).TableFormat.WrapTextAround)
                    {
                        if (m_absolutePositionedTableHeights == null)
                            m_absolutePositionedTableHeights = new List<float>();
                        m_absolutePositionedTableHeights.Add(childWidget.Bounds.Height);
                        continue;
                    }
                    for (int j = 0; j < childWidget.ChildWidgets.Count; j++)
                    {
                        m_lineHeights.Insert(m_lineHeights.Count, childWidget.ChildWidgets[j].Bounds.Height);
                    }
                }
            }
        }
        /// <summary>
        /// Get Column index which have minimum column width
        /// </summary>
        /// <returns></returns>
        private int GetColumnIndexForMinColumnWidth()
        {
            int columnIndex = 0;
            float min = CurrentSection.Columns[0].Width;
            for (int j = 1; j < CurrentSection.Columns.Count; j++)
            {
                if (CurrentSection.Columns[j].Width < min)
                {
                    columnIndex = j;
                    min = CurrentSection.Columns[j].Width;
                }
            }
            return columnIndex;
        }
        /// <summary>
        /// Get Column index which have maximum column width
        /// </summary>
        /// <returns></returns>
        private int GetColumnIndexForMaxColumnWidth()
        {
            int columnIndex = 0;
            float max = CurrentSection.Columns[0].Width;
            for (int j = 1; j < CurrentSection.Columns.Count; j++)
            {
                if (CurrentSection.Columns[j].Width > max)
                {
                    columnIndex = j;
                    max = CurrentSection.Columns[j].Width;
                }
            }
            return columnIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool HeaderGetNextArea(out RectangleF area)
        {
            // LW as page header didn't add
            area = (CurrentPage.PageWidgets.Count != 0) ?
              RectangleF.Empty :
              CurrentPage.GetHeaderArea();

            return !area.Equals(RectangleF.Empty);;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        internal void HeaderPushLayoutedWidget(LayoutedWidget ltWidget)
        {
            CurrentPage.PageWidgets.Add(ltWidget);
        }
        /// <summary>
        /// Push Footnote layouted widgets into current page
        /// </summary>
        /// <param name="layoutArea"></param>
        /// <returns></returns>
        internal void FootnotePushLayoutedWidget(RectangleF layoutArea)
        {
            float totalHeight = CurrentPage.FootnoteWidgets[CurrentPage.FootnoteWidgets.Count - 1].Bounds.Bottom - layoutArea.Y;
            if (CurrentSection.PageSetup.FootnotePosition == FootnotePosition.PrintImmediatelyBeneathText)
            {
                totalHeight = 0;
            }
            for (int i = m_footnoteCount; i < CurrentPage.FootnoteWidgets.Count; i++)
            {
                CurrentPage.FootnoteWidgets[i].ShiftLocation(0, layoutArea.Height - totalHeight, true);
            }
            if (m_currSection.Columns.Count > 1)
                m_footnoteCount = m_currPage.FootnoteWidgets.Count;
            if (m_columnIndex == m_currSection.Columns.Count || m_currSection.Columns.Count == 1)
                m_footnoteCount = 0;
        }
        /// <summary>
        /// Push Endnote layouted widgets.
        /// </summary>
        /// <param name="layoutArea"></param>
        /// <returns></returns>
        internal void EndnotePushLayoutedWidget(RectangleF layoutArea, LayoutedWidget ltWidget)
        {
            Entity ent = ltWidget.Widget as Entity;

            if (ltWidget.Widget is SplitWidgetContainer)
                ent = (ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer as Entity;
            float footNoteHeight = 0;

            WSection section = ltWidget.ChildWidgets[0].Widget is SplitWidgetContainer ? (ltWidget.ChildWidgets[0].Widget as SplitWidgetContainer).RealWidgetContainer as WSection : ltWidget.ChildWidgets[0].Widget as WSection;
            //Get the current section index.
            int currentSectionIndex = ent.Document.Sections.IndexOf(section as WSection);
            //Add the total height of the foot notes if availble in the section. 
            for (int i = 0; i < CurrentPage.FootnoteWidgets.Count; i++)
            {
                if (currentSectionIndex == CurrentPage.FootNoteSectionIndex[i])
                    footNoteHeight += CurrentPage.FootnoteWidgets[i].Bounds.Height;
            }
            //Update the bounds points of the endnotes
            for (int i = m_endnoteCount; i < CurrentPage.EndnoteWidgets.Count; i++)
            {
                if (ent.Document.EndnotePosition == EndnotePosition.DisplayEndOfSection)
                {
                    if (currentSectionIndex == CurrentPage.EndNoteSectionIndex[i])
                        CurrentPage.EndnoteWidgets[i].ShiftLocation(0, layoutArea.Height + footNoteHeight, true);
                }
                else
                {
                    CurrentPage.EndnoteWidgets[i].ShiftLocation(0, layoutArea.Height + footNoteHeight, true);
                }
            }
            //Update the column count m_endnoteCount this property. 
            if (m_currSection.Columns.Count > 1)
                m_endnoteCount = m_currPage.EndnoteWidgets.Count;
            if (m_columnIndex == m_currSection.Columns.Count || m_currSection.Columns.Count == 1)
                m_endnoteCount = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool FooterGetNextArea(out RectangleF area)
        {
            // LW as page footer didn't add
            area = (CurrentPage.PageWidgets.Count != 1) ?
              RectangleF.Empty :
              CurrentPage.GetFooterArea();

            return !area.Equals(RectangleF.Empty);;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ltWidget"></param>
        /// <returns></returns>
        internal void FooterPushLayoutedWidget(LayoutedWidget ltWidget)
        {
            ltWidget.ShiftLocation(0, -(ltWidget.Bounds.Height + SpaceBetweenTextbodyAndFooter), true);
            CurrentPage.PageWidgets.Add(ltWidget);
        }
       
        #endregion

        #region Class internal declarations
        /// <summary>
        /// 
        /// </summary>
        internal class HeaderFooterLPHandler : ILayoutProcessHandler
        {
            #region Fields
            private DocumentLayouter m_dl;
            private bool m_bFooter = false;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="HeaderFooterLPHandler"/> class.
            /// </summary>
            /// <param name="dl">The dl.</param>
            /// <param name="bFooter">if set to <c>true</c> [b footer].</param>
            public HeaderFooterLPHandler(DocumentLayouter dl, bool bFooter)
            {
                m_dl = dl;
                m_bFooter = bFooter;
            }
            #endregion

            #region ILayoutProcessHandler members
            /// <summary>
            /// Gets the next area.
            /// </summary>
            /// <param name="area">The area.</param>
            /// <param name="isContinuousSection">The isContinuousSection.</param>
            /// <returns></returns>
            public bool GetNextArea(out RectangleF area, ref int columnIndex, ref bool isContinuousSection, bool isSplittedWidget, ref float topMargin)
            {
                if (!m_bFooter)
                    return m_dl.HeaderGetNextArea(out area);
                else
                    return m_dl.FooterGetNextArea(out area);
            }

            /// <summary>
            /// Pushes the LayoutedWidget to external holder.
            /// </summary>
            /// <param name="ltWidget">The LayoutedWidget.</param>
            public void PushLayoutedWidget(LayoutedWidget ltWidget, RectangleF layoutArea, bool isNeedToRestartFootnote, bool isNeedToRestartEndnote)
            {
                if (!m_bFooter)
                    m_dl.HeaderPushLayoutedWidget(ltWidget);
                else
                    m_dl.FooterPushLayoutedWidget(ltWidget);
            }

            /// <summary>
            /// Handles the splitted widget.
            /// </summary>
            /// <param name="stWidgetContainer">The splitted widget container.</param>
            /// <param name="state">The current state of layout context.</param>
            /// <param name="ltWidget">The LayoutedWidget.</param>
            /// <param name="isLayoutedWidgetNeedToPushed">isLayoutedWidgetNeedToPushed</param>
            /// <returns>
            /// True for continue layout process, False - for stopping
            /// </returns>
            public bool HandleSplittedWidget(SplitWidgetContainer stWidgetContainer, LayoutState state, LayoutedWidget ltWidget, ref bool isLayoutedWidgetNeedToPushed)
            {
                return false;
            }

            /// <summary>
            /// Handles the LayoutedWidget to external holder.
            /// </summary>
            /// <param name="ltWidget">The LayoutedWidget.</param>
            public void HandleLayoutedWidget(LayoutedWidget ltWidget)
            {
            }
            #endregion
        }
        #endregion
    }
}

#endif
