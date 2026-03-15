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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.IO;

using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.Convertors;
using Syncfusion.DocIO.DLS.Rendering;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Primitives;
using System;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;

namespace Syncfusion.DocToPDFConverter
{
    /// <summary>
    /// This class provides support for converting WordDocument into an PDF Document.
    /// </summary>
    /// <seealso cref="WordDocument"/>
    /// <seealso cref="PdfDocument"/>
    /// <example>
    /// This example converts the specified Word Document in to PDF Document.
    /// <code lang="C#">
    /// void Convert()
    /// {
    ///     WordDocument wordDoc = new WordDocument(strFileName);
    ///     DocToPDFConverterControl converter = new DocToPDFConverterControl();
    ///     PdfDocument pdfDoc = converter.ConvertToPDF(wordDoc);
    /// }
    /// </code>
    /// </example>
    [ToolboxItem(false)]
    public class DocToPDFConverter : System.ComponentModel.Component
    {
        #region Constants
        /// <summary>
        /// Specifies the default image type.
        /// </summary>
        private const ImageType DEF_IMAGETYPE = ImageType.Metafile;
        #endregion

        #region Fields
        private List<WPageSetup> m_pageSettings;
        private WordDocument m_wordDocument;
        private PdfDocument m_pdfDocument;
        private PdfPage m_currentPage;
        /// <summary>
        /// converter settings to the document
        /// </summary>
        private DocToPDFConverterSettings m_settings = new DocToPDFConverterSettings();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the page settings.
        /// </summary>
        /// <value>The page settings.</value>
        private List<WPageSetup> PageSettings
        {
            get
            {
                return m_pageSettings;
            }
        }
        /// <summary>
        /// Gets or sets the quality.
        /// </summary>
        public DocToPDFConverterSettings Settings
        {
            get
            {
                return m_settings;
            }
            set
            {
                m_settings = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocToPDFConverterControl"/> class.
        /// </summary>
        public DocToPDFConverter()
        {
            m_pageSettings = new List<WPageSetup>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts the specified WordDocument to PDF.
        /// </summary>
        /// <param name="wordDocument">The word document.</param>
        /// <returns>The PdfDocument.</returns>
        public PdfDocument ConvertToPDF(WordDocument wordDocument)
        {
            m_wordDocument = wordDocument;

            DocumentLayouter layouter = new DocumentLayouter();
            layouter.Layout(wordDocument);
            return this.DrawToPDF(layouter);
        }

        /// <summary>
        /// Converts the specified WordDocument to PDF.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The PdfDocument.</returns>
        public PdfDocument ConvertToPDF(string fileName)
        {
            WordDocument wordDocument = new WordDocument(fileName, FormatType.Automatic);
            return this.ConvertToPDF(wordDocument);
        }

        /// <summary>
        /// Converts to PDF.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The PdfDocument.</returns>
        public PdfDocument ConvertToPDF(Stream stream)
        {
            WordDocument wordDocument = new WordDocument(stream, FormatType.Automatic);
            return this.ConvertToPDF(wordDocument);
        }
        #endregion

        #region Helper Methods
 
        /// <summary>
        /// Creates the PDF document.
        /// </summary>
        /// <returns></returns>
        private PdfDocument CreateDocument()
        {
            PdfDocument pdfDoc = new PdfDocument();
            pdfDoc.PageSettings.Margins.All = 0;
            pdfDoc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceTable;
            pdfDoc.FileStructure.Version = PdfVersion.Version1_4;
            return pdfDoc;
        }

        /// <summary>
        /// Adds the section.
        /// </summary>
        /// <param name="pageSetup">The page setup.</param>
        /// <returns></returns>
        private PdfSection AddSection(WPageSetup pageSetup)
        {
            PdfSection pdfSection = m_pdfDocument.Sections.Add();
            pdfSection.PageSettings.Margins.All = 0;
            pdfSection.PageSettings.Orientation = (pageSetup.Orientation == PageOrientation.Portrait || pageSetup.Orientation.ToString()=="1") ?
                PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
            //pdfSection.PageSettings.Size = pageSetup.PageSize;
            pdfSection.PageSettings.Height = pageSetup.PageSize.Height;
            pdfSection.PageSettings.Width = pageSetup.PageSize.Width;
            return pdfSection;
        }

        /// <summary>
        /// Sets the pages settings.
        /// </summary>
        /// <param name="layouter">The layouter.</param>
        private void InitPagesSettings(DocumentLayouter layouter)
        {
            for (int i = 0; i < layouter.Pages.Count; i++)
            {
                Page page = layouter.Pages[i];
                PageSettings.Add(page.Setup);
            }
        }

        /// <summary>
        /// Adds the document properties.
        /// </summary>
        /// <param name="docProperties">The doc properties.</param>
        private void AddDocumentProperties(BuiltinDocumentProperties docProperties)
        {
            m_pdfDocument.DocumentInformation.Author = docProperties.Author;
            m_pdfDocument.DocumentInformation.CreationDate = docProperties.CreateDate;
            m_pdfDocument.DocumentInformation.Creator = docProperties.Company;
            m_pdfDocument.DocumentInformation.Keywords = docProperties.Keywords;
            m_pdfDocument.DocumentInformation.Producer = docProperties.Company;
            m_pdfDocument.DocumentInformation.Subject = docProperties.Subject;
            m_pdfDocument.DocumentInformation.Title = docProperties.Title;
        }

        /// <summary>
        /// Adds the hyper links.
        /// </summary>
        /// <param name="hyperlinks">The hyperlinks.</param>
        private void AddHyperLinks(List<Dictionary<string, RectangleF>> hyperlinks)
        {
            for (int i = 0; i < hyperlinks.Count; i++)
            {
                foreach (KeyValuePair<string, RectangleF> uriAnnot in hyperlinks[i])
                {
                    RectangleF bounds = uriAnnot.Value;
                    string uri = uriAnnot.Key;

                    if (!uri.Equals(string.Empty))
                    {
                        PdfUriAnnotation annotation = new PdfUriAnnotation(bounds);
                        annotation.Uri = uri;
                        annotation.Border.Width = 0;
                        m_currentPage.Annotations.Add(annotation);
                    }
                }
            }
        }

        private void AddBookmarkHyperlinks(List<Dictionary<string, Syncfusion.DocIO.Rendering.BookmarkHyperlink>> bookmarkHyperlinks)
        {
            for (int i = 0; i < bookmarkHyperlinks.Count; i++)
            {
                foreach (KeyValuePair<string, Syncfusion.DocIO.Rendering.BookmarkHyperlink> linkAnnot in bookmarkHyperlinks[i])
                {
                    Syncfusion.DocIO.Rendering.BookmarkHyperlink hyperlink = linkAnnot.Value;
                    if (hyperlink.SourcePageNumber == (m_pdfDocument.Pages.IndexOf(m_currentPage) + 1))
                    {
                        string link = linkAnnot.Key;
                        if (!link.Equals(string.Empty))
                        {
                            PdfDocumentLinkAnnotation annotation = new PdfDocumentLinkAnnotation(hyperlink.SourceBounds);
                            annotation.Border = new PdfAnnotationBorder(0);
                            if (m_pdfDocument.Pages.Count >= hyperlink.TargetPageNumber && hyperlink.TargetPageNumber != 0)
                            {
                                annotation.Destination = new PdfDestination(m_pdfDocument.Pages[hyperlink.TargetPageNumber - 1]);
                                annotation.Destination.Location = hyperlink.TargetBounds.Location;
                            }
                            m_currentPage.Annotations.Add(annotation);
                            if (i == bookmarkHyperlinks.Count - 1)
                            {
                                AddBookmarks(bookmarkHyperlinks);
                            }
                        }
                    }
                }
            }
        }
        private void AddDocumentBookmarks(List<Syncfusion.DocIO.Rendering.BookmarkPosition> bookmarks)
        {
            foreach (Syncfusion.DocIO.Rendering.BookmarkPosition bookmark in bookmarks)
            {
                if (!string.IsNullOrEmpty(bookmark.BookmarkName) && bookmark.PageNumber != 0 && bookmark.PageNumber <= m_pdfDocument.Pages.Count)
                {
                    PdfBookmark pdfBookmark = m_pdfDocument.Bookmarks.Add(bookmark.BookmarkName);
                    pdfBookmark.Destination = new PdfDestination(m_pdfDocument.Pages[bookmark.PageNumber - 1]);
                    pdfBookmark.Destination.Location = bookmark.Bounds.Location;
                }
            }
        }
        /// <summary>
        /// Converts the TOC into Bookmark.
        /// </summary>
        private void AddBookmarks(List<Dictionary<string, Syncfusion.DocIO.Rendering.BookmarkHyperlink>> bookmarkHyperlinks)
        {
            int rootbookmarkIndex = -1;
            PdfBookmark[] pdfchild = new PdfBookmark[bookmarkHyperlinks.Count+1];
            
             for (int i = 0; i < bookmarkHyperlinks.Count; i++)
             {
                foreach (KeyValuePair<string, Syncfusion.DocIO.Rendering.BookmarkHyperlink> linkAnnot in bookmarkHyperlinks[i])
                {
                    Syncfusion.DocIO.Rendering.BookmarkHyperlink hyperlink = linkAnnot.Value;
                    if (i == 0)
                    {
                        if (hyperlink.TOCText != "")
                        {
                            rootbookmarkIndex = hyperlink.TOCLevel;
                        }
                    }
                    if (hyperlink.SourcePageNumber == (m_pdfDocument.Pages.IndexOf(m_currentPage) + 1))
                    {
                        string link = linkAnnot.Key;
                        if (!link.Equals(string.Empty))
                        {
                            if (m_pdfDocument.Pages.Count >= hyperlink.TargetPageNumber && hyperlink.TargetPageNumber != 0)
                            {
                                if (hyperlink.TOCLevel <= rootbookmarkIndex)
                                {
                                    pdfchild[hyperlink.TOCLevel] = m_pdfDocument.Bookmarks.Add(hyperlink.TOCText);
                                    PdfPage page = m_pdfDocument.Pages[hyperlink.TargetPageNumber - 1];
                                    pdfchild[hyperlink.TOCLevel].Destination = new PdfDestination(m_pdfDocument.Pages[hyperlink.TargetPageNumber - 1]);
                                    pdfchild[hyperlink.TOCLevel].Destination.Page = page;
                                    pdfchild[hyperlink.TOCLevel].Destination.Location = new PointF(hyperlink.TargetBounds.X, hyperlink.TargetBounds.Y);
                                }
                                else
                                {
                                    if (rootbookmarkIndex != -1)
                                    {
                                        if (pdfchild[hyperlink.TOCLevel - 1] != null)
                                        {
                                            pdfchild[hyperlink.TOCLevel] = pdfchild[hyperlink.TOCLevel - 1].Add(hyperlink.TOCText);
                                            PdfPage page = m_pdfDocument.Pages[hyperlink.TargetPageNumber - 1];
                                            pdfchild[hyperlink.TOCLevel].Destination = new PdfDestination(m_pdfDocument.Pages[hyperlink.TargetPageNumber - 1]);
                                            pdfchild[hyperlink.TOCLevel].Destination.Page = page;
                                            pdfchild[hyperlink.TOCLevel].Destination.Location = new PointF(hyperlink.TargetBounds.X, hyperlink.TargetBounds.Y);
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
        /// Draw To PDF
        /// </summary>
        /// <param name="layouter">The DocumentLayouter</param>
        /// <returns>PdfDocument</returns>
        private PdfDocument DrawToPDF(DocumentLayouter layouter)
        {
            InitPagesSettings(layouter);
            
            int totalPageCount = layouter.Pages.Count;
            m_pdfDocument = CreateDocument();

            for (int i = 0; i < totalPageCount; i++)
            {
                PdfSection sec = AddSection(PageSettings[i]);
                PdfPage page = sec.Pages.Add();
                m_currentPage = page;
                layouter.DrawToImage(i, -1, ImageType.Metafile);
                PdfMetafile pdfMetafile = (PdfMetafile)PdfImage.FromImage(layouter.PageResult.Pages[0].PageImage);
                if (m_settings.m_imageResolution >0)
                {
                    pdfMetafile.ImageResolution = m_settings.m_imageResolution;
                }
                else
                {
                    pdfMetafile.Quality = m_settings.ImageQuality;
                }
                pdfMetafile.Draw(page, new RectangleF(PointF.Empty, page.Size), true);
                AddHyperLinks(layouter.PageResult.Pages[0].Hyperlinks);
                layouter.PageResult.Pages[0].PageImage.Dispose();
                layouter.PageResult.Pages.Clear();
            }
            layouter.InitLayoutInfo();
            for (int i = 0; i < totalPageCount; i++)
            {
                m_currentPage = m_pdfDocument.Pages[i];
                AddBookmarkHyperlinks(DocumentLayouter.BookmarkHyperlinks);
            }
            AddDocumentBookmarks(DocumentLayouter.Bookmarks);
            AddDocumentProperties(m_wordDocument.BuiltinDocumentProperties);

            return m_pdfDocument;
        }
        #endregion
    }
}
