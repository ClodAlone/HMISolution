#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;


namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents the  method that executes on a PdfDocument when a new page is created.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A <see cref="PageAddedEventArgs"/> that contains the event data.</param>
    public delegate void PageAddedEventHandler(object sender, PageAddedEventArgs args);

    /// <summary>
    /// Provides data for PageAdded event.
    /// </summary>
    /// <remarks>
    /// This event raised on adding the pages. 
    /// </remarks> 
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// document.Pages.PageAdded += new PageAddedEventHandler(Pages_PageAdded);
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font with Bold font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
    /// //Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
    /// page = document.Pages.Add();      
    /// //Saves the document
    /// document.Save("Sample.pdf");
    /// //Event handler for PageAdded event
    /// void Pages_PageAdded(object sender, PageAddedEventArgs args)
    /// {   
    ///   PdfFont font = new PdfStandardFont(PdfFontFamily.Courier,10);
    ///   args.Page.Graphics.DrawString("New Page", font, PdfBrushes.Black, new PointF(100,100));
    /// }
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Private document As PdfDocument = New PdfDocument()
    /// Private document.Pages.PageAdded += New PageAddedEventHandler(AddressOf Pages_PageAdded)
    /// 'Creates a new page and adds it as the last page of the document
    /// Private page As PdfPage = document.Pages.Add()
    /// 'Create font with Bold font style.
    /// Private font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
    /// page = document.Pages.Add()
    /// 'Saves the document
    /// document.Save("Sample.pdf")
    /// 'Event handler for PageAdded event
    /// void Pages_PageAdded(Object sender, PageAddedEventArgs args)
    ///   Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier,10)
    ///   args.Page.Graphics.DrawString("New Page", font, PdfBrushes.Black, New PointF(100,100))
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PageAddedEventHandler"/> Delegate.      
    public class PageAddedEventArgs : EventArgs
    {
        #region Fields
        //Represents added page
        private PdfPage m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the newly added page.
        /// </summary>
        /// <value>a <see cref="PdfPage"/> object representing the page which is added in the document.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// document.Pages.PageAdded += new PageAddedEventHandler(Pages_PageAdded);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font with Bold font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
        /// page = document.Pages.Add();      
        /// //Saves the document
        /// document.Save("Sample.pdf");
        /// // Event handler for PageAdded event
        /// void Pages_PageAdded(object sender, PageAddedEventArgs args)
        /// {
        ///  PdfPage page = args.Page;
        ///  PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 10);
        ///  page.Graphics.DrawString("New Page", font, PdfBrushes.Black, new PointF(100, 100));
        /// }
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Private document As PdfDocument = New PdfDocument()
        /// Private document.Pages.PageAdded += New PageAddedEventHandler(AddressOf Pages_PageAdded)
        /// 'Creates a new page and adds it as the last page of the document
        /// Private page As PdfPage = document.Pages.Add()
        /// 'Create font with Bold font style.
        /// Private font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
        /// page = document.Pages.Add()
        /// 'Saves the document
        /// document.Save("Sample.pdf")
        /// ' Event handler for PageAdded event
        /// Private Sub Pages_PageAdded(ByVal sender As Object, ByVal args As PageAddedEventArgs)
        ///   Dim page As PdfPage = args.Page
        ///   Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier,10)
        ///   page.Graphics.DrawString("New Page", font, PdfBrushes.Black, New PointF(100,100))
        /// End Sub
        /// </code>
        /// </example>
        /// <seealso cref="PdfPage"/> Class.
        public PdfPage Page
        {
            get
            {
                return m_page;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageAddedEventArgs"/> class.
        /// </summary>
        private PageAddedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageAddedEventArgs"/> class.
        /// </summary>
        /// <param name="page">a <see cref="PdfPage"/> object representing the page which is added in the document.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// document.Pages.PageAdded += new PageAddedEventHandler(Pages_PageAdded);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font with Bold font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
        /// page = document.Pages.Add();      
        /// //Saves the document
        /// document.Save("Sample.pdf");
        /// // Event handler for PageAdded event
        /// void Pages_PageAdded(object sender, PageAddedEventArgs args)
        /// {   
        ///   PdfFont font = new PdfStandardFont(PdfFontFamily.Courier,10);
        ///   args.Page.Graphics.DrawString("New Page", font, PdfBrushes.Black, new PointF(100,100));
        /// }
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Private document As PdfDocument = New PdfDocument()
        /// Private document.Pages.PageAdded += New PageAddedEventHandler(AddressOf Pages_PageAdded)
        /// 'Creates a new page and adds it as the last page of the document
        /// Private page As PdfPage = document.Pages.Add()
        /// 'Create font with Bold font style.
        /// Private font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
        /// page = document.Pages.Add()
        /// 'Saves the document
        /// document.Save("Sample.pdf")
        /// ' Event handler for PageAdded event
        /// void Pages_PageAdded(Object sender, PageAddedEventArgs args)
        ///   Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier,10)
        ///   args.Page.Graphics.DrawString("New Page", font, PdfBrushes.Black, New PointF(100,100))
        /// End Sub
        /// </code>
        /// </example>
        /// <seealso cref="PdfPage"/> Class. 
        public PageAddedEventArgs(PdfPage page)
        {
            m_page = page;
        }
        #endregion
    }
}