#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents a booklet creator, which allows to create a booklet from a Pdf document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load a PDF document
    /// PdfLoadedDocument ldoc = new PdfLoadedDocument("SourceDoc.pdf");
    /// // Creates a booklet from the given PDF document         
    /// PdfDocument doc = PdfBookletCreator.CreateBooklet(ldoc, new SizeF(300, 500));
    /// //Save the document
    /// doc.Save("Booklet.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load a PDF document
    /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
    /// ' Creates a booklet from the given PDF document              
    /// Dim doc As PdfDocument = PdfBookletCreator.CreateBooklet(ldoc, New SizeF(300, 500))
    /// 'Save the document
    /// doc.Save("Booklet.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/>
    /// <seealso cref="PdfLoadedDocument"/>
    public sealed class PdfBookletCreator
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookletCreator"/> class.
        /// </summary>
        private PdfBookletCreator()
        {
            throw new NotSupportedException("Instantination of BookletCreator class is not supported");
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookletCreator"/> class.
        /// </summary>
        /// <param name="loadedDocument">The existing PDF document.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// The initialized PDF document, which could be saved.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing PDF document
        /// PdfLoadedDocument ldoc = new PdfLoadedDocument("SourceDoc.pdf");
        /// //Creates a booklet from the given PDF document          
        /// PdfDocument doc = PdfBookletCreator.CreateBooklet(ldoc, new SizeF(300, 500));
        /// //Save the document
        /// doc.Save("Booklet.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load a PDF document
        /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// 'Creates a booklet from the given PDF document          
        /// Dim doc As PdfDocument = PdfBookletCreator.CreateBooklet(ldoc, New SizeF(300, 500))
        /// 'Save the document
        /// doc.Save("Booklet.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/>
        /// <seealso cref="PdfLoadedDocument"/>
        public static PdfDocument CreateBooklet(PdfLoadedDocument loadedDocument, SizeF pageSize)
        {
            return CreateBooklet(loadedDocument, pageSize, false);
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookletCreator"/> class.
        /// </summary>
        /// <param name="from">The path to the file on the disk, which the booklet should be created from.</param>
        /// <param name="into">The path to the file on the disk, which the booklet should be saved into.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="twoSide">if set to <c>true</c> if the result in document should be printed
        /// on both sides of paper.</param>
        /// <returns>
        /// The initialized PDF document, which could be saved.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        ///  //Create booklet with two sides        
        /// PdfBookletCreator.CreateBooklet("SourceDocument.pdf","Booklet.pdf",new SizeF(300, 500), true);
        /// </code>
        /// <code lang="VB">
        ///  'Create booklet with two sides          
        /// PdfBookletCreator.CreateBooklet("SourceDocument.pdf","Booklet.pdf",New SizeF(300, 500), true)
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/>
        /// <seealso cref="PdfLoadedDocument"/>
        public static void CreateBooklet(string from, string into, SizeF pageSize, bool twoSide)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            if (from == string.Empty)
            {
                throw new ArgumentOutOfRangeException("from", "Parameter can not be empty");
            }

            if (into == null)
            {
                throw new ArgumentNullException("into");
            }

            if (into == string.Empty)
            {
                throw new ArgumentOutOfRangeException("into", "Parameter can not be empty");
            }

            if (pageSize == SizeF.Empty)
            {
                throw new ArgumentOutOfRangeException("pageSize", "Parameter can not be empty");
            }

            PdfLoadedDocument doc = new PdfLoadedDocument(from);
            PdfDocument booklet = CreateBooklet(doc, pageSize, twoSide);
            booklet.Save(into);
            booklet.Close();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookletCreator"/> class.
        /// </summary>
        /// <param name="from">The path to the file on the disk, which the booklet should be created from.</param>
        /// <param name="into">The path to the file on the disk, which the booklet should be saved into.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// The initialized PDF document, which could be saved.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        ///  //Creates a booklet from the given PDF document
        /// PdfBookletCreator.CreateBooklet("SourceDocument.pdf","Booklet.pdf",new SizeF(300, 500));
        /// </code>
        /// <code lang="VB">
        ///  'Creates a booklet from the given PDF document        
        /// PdfBookletCreator.CreateBooklet("SourceDocument.pdf","Booklet.pdf",New SizeF(300, 500))
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/>
        /// <seealso cref="PdfLoadedDocument"/>
        public static void CreateBooklet(string from, string into, SizeF pageSize)
        {
            CreateBooklet(from, into, pageSize, false);
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookletCreator"/> class.
        /// </summary>
        /// <param name="loadedDocument">The loaded document.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="twoSide">if set to <c>true</c> if the result in document should be printed
        /// on both sides of paper.</param>
        /// <returns>
        /// The initialized PDF document, which could be saved.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// //Load a PDF document
        /// PdfLoadedDocument ldoc = new PdfLoadedDocument("SourceDoc.pdf");
        /// //Creates a booklet from the given PDF document      
        /// PdfDocument doc = PdfBookletCreator.CreateBooklet(ldoc, new SizeF(300, 500), false);
        /// //Save the document
        /// doc.Save("Booklet.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load a PDF document
        /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// 'Creates a booklet from the given PDF document             
        /// Dim doc As PdfDocument = PdfBookletCreator.CreateBooklet(ldoc, New SizeF(300, 500), False)
        /// 'Save the document
        /// doc.Save("Booklet.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/>
        /// <seealso cref="PdfLoadedDocument"/>
        public static PdfDocument CreateBooklet(PdfLoadedDocument loadedDocument, SizeF pageSize, bool twoSide)
        {
            if (loadedDocument == null)
            {
                throw new ArgumentNullException("loadedDocument");
            }

            if (pageSize == SizeF.Empty)
            {
                throw new ArgumentOutOfRangeException("pageSize", "Parameter can not be empty");
            }

            PdfPage page;
            SizeF sheetSize = new SizeF(pageSize.Width / 2.0f, pageSize.Height);
            PointF location1 = PointF.Empty;
            PointF location2 = new PointF(sheetSize.Width, 0.0f);

            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Margins.All = 0;

            int pageCount = loadedDocument.Pages.Count;

            PdfLoadedPageCollection pc = loadedDocument.Pages;
            int count = (pageCount / 2) + (pageCount % 2);

            bool withCover = false;

            if (twoSide)
            {
                withCover = ((count % 2) == 0);
            }

            for (int i = 0; i < count; ++i)
            {
                PdfPageBase loadedPage;
                PdfTemplate content;

                int index;
                doc.PageSettings.Size = pageSize;
                if (pageSize.Width > pageSize.Height)
                    doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                page = doc.Pages.Add();
              

                int[] pages = GetNextPair(i, pageCount, twoSide);

                index = (twoSide && withCover) ? 1 : 0;
                index = pages[index];
                if (index >= 0)
                {
                    loadedPage = pc[index];
                    content = loadedPage.CreateTemplate();
                    page.Graphics.DrawPdfTemplate(content, location1, sheetSize);
                }

                index = (withCover) ? 0 : 1;
                index = pages[index];
                if (index >= 0)
                {
                    loadedPage = pc[index];
                    content = loadedPage.CreateTemplate();
                    page.Graphics.DrawPdfTemplate(content, location2, sheetSize);
                }
            }

            return doc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookletCreator"/> class.
        /// </summary>
        /// <param name="loadedDocument">The loaded document.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="twoSide">if set to <c>true</c> if the result in document should be printed
        /// on both sides of paper.</param>
        /// <param name="margin">The margin value for generated PDF document.</param>
        /// <returns>The initialized PDF document, which could be saved.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument srcDoc = new PdfLoadedDocument("sourceDoc.pdf");
        /// // Specify the margin.
        /// PdfMargins margin = new PdfMargins();
        /// margin.All = 10;
        /// //Creates a booklet from the given PDF document        
        /// PdfDocument doc = PdfBookletCreator.CreateBooklet(srcDoc,new SizeF(300, 500), false,margin);
        /// doc.Save("Booklet.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim srcDoc As PdfLoadedDocument = New PdfLoadedDocument("sourceDoc.pdf")
        /// ' Specify the margin.
        /// Dim margin As PdfMargins = New PdfMargins()
        /// margin.All = 10
        /// 'Creates a booklet from the given PDF document         
        /// Dim doc As PdfDocument = PdfBookletCreator.CreateBooklet(srcDoc,New SizeF(300, 500), False,margin)
        /// doc.Save("Booklet.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/>
        /// <seealso cref="PdfLoadedDocument"/>
        public static PdfDocument CreateBooklet(PdfLoadedDocument loadedDocument, SizeF pageSize, bool twoSide, PdfMargins margin)
        {
            if (loadedDocument == null)
            {
                throw new ArgumentNullException("loadedDocument");
            }

            if (pageSize == SizeF.Empty)
            {
                throw new ArgumentOutOfRangeException("pageSize", "Parameter can not be empty");
            }

            PdfPage page;
            SizeF sheetSize = new SizeF(pageSize.Width / 2.0f, pageSize.Height);
            PointF location1 = PointF.Empty;
            PointF location2 = new PointF(sheetSize.Width, 0.0f);

            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Margins = margin;

            int pageCount = loadedDocument.Pages.Count;

            PdfLoadedPageCollection pc = loadedDocument.Pages;
            int count = (pageCount / 2) + (pageCount % 2);

            bool withCover = false;

            if (twoSide)
            {
                withCover = ((count % 2) == 0);
            }

            for (int i = 0; i < count; ++i)
            {
                PdfPageBase loadedPage;
                PdfTemplate content;

                int index;
                doc.PageSettings.Size = pageSize;
                page = doc.Pages.Add();                
               

                int[] pages = GetNextPair(i, pageCount, twoSide);

                index = (twoSide && withCover) ? 1 : 0;
                index = pages[index];
                if (index >= 0)
                {
                    loadedPage = pc[index];
                    content = loadedPage.CreateTemplate();
                    page.Graphics.DrawPdfTemplate(content, location1, sheetSize);
                }

                index = (withCover) ? 0 : 1;
                index = pages[index];
                if (index >= 0)
                {
                    loadedPage = pc[index];
                    content = loadedPage.CreateTemplate();
                    page.Graphics.DrawPdfTemplate(content, location2, sheetSize);
                }
            }

            return doc;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the next pair of page indeces.
        /// </summary>
        /// <param name="index">The current iteration index.</param>
        /// <param name="count">The pages count.</param>
        /// <param name="twoSide">if set to <c>true</c> if the result in document should be printed
        /// on both sides of paper.</param>
        /// <returns>
        /// An array of integers that holds the indices.
        /// </returns>
        private static int[] GetNextPair(int index, int count, bool twoSide)
        {
            int[] pages = new int[2];
            int secondPage = count - index - ((count + 1) % 2);

            if (secondPage == count)
            {
                secondPage = -1;
            }

            if (twoSide && (index % 2) > 0)
            {
                pages[1] = index;
                pages[0] = secondPage;
            }
            else
            {
                pages[0] = index;
                pages[1] = secondPage;
            }

            return pages;
        }
        #endregion
    }
}
