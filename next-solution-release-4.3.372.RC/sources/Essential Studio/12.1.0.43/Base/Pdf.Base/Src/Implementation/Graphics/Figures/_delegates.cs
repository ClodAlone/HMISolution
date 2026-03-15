#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

/// <summary>
/// The Syncfusion.Pdf.Graphics namespace contains classes used to create Graphics elements.
/// </summary>
namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the data for a cancelable event.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
    /// PdfPen pen = new PdfPen(Color.Black, 1f);
    /// //Creates a new  pdf font
    /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
    /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
    /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
    /// string path = @"..\..\Data\Essential studio.txt";
    /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
    /// string text = reader.ReadToEnd();
    /// reader.Close();
    /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
    /// //Create text element
    /// PdfTextElement element = new PdfTextElement(text, pdfFont);
    /// element.Brush = new PdfSolidBrush(Color.Black);
    /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
    /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
    /// layoutFormat.Layout = PdfLayoutType.Paginate;
    /// //Raise the event when the text flows to next page.
    /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
    /// //Get the remaining text that flows beyond the boundary.
    /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
    /// //Saves the document.
    /// doc.Save("Sample.pdf");
    /// //Begin Page Layout Event Handler
    /// private void BeginPageLayout2(object sender, BeginPageLayoutEventArgs e)
    /// {
    ///  e.Cancel=true;
    /// }
    /// </code>
    /// <code lang="VB">
	/// 'Create a PDF document
	/// Dim doc As New PdfDocument()
	/// 'Creates a new page and adds it as the last page of the document
	/// Dim page As PdfPage = doc.Pages.Add()
	/// Dim brush As New PdfSolidBrush(Color.Black)
	/// Dim pen As New PdfPen(Color.Black, 1f)
	/// 'Creates a new  pdf font
	/// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	/// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	/// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	/// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	/// Dim reader As New StreamReader(path, Encoding.ASCII)
	/// Dim text As string = reader.ReadToEnd()
	/// reader.Close()
	/// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	///	'Create text element
	///	Dim element As New PdfTextElement(text, pdfFont)
	///	element.Brush = New PdfSolidBrush(Color.Black)
	///	Dim layoutFormat As New PdfLayoutFormat()
	///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	///	layoutFormat.Layout = PdfLayoutType.Paginate
	///	'Raise the event when the text flows to next page.
	///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	///	'Get the remaining text that flows beyond the boundary.
    ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	///	'Saves the document.
	///	doc.Save("Sample.pdf")
    /// End Sub
    /// 'Begin Page Layout Event Handler
    /// Private Sub BeginPageLayout2(ByVal sender As object, ByVal e As BeginPageLayoutEventArgs)
	///	 e.Cancel=True
    /// End Sub
    /// </code>
    /// </example>
    public class PdfCancelEventArgs : EventArgs
    {
        #region Fields
        /// <summary>
        /// Indicates whether lay outing should be stopped.
        /// </summary>
        private bool m_cancel;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfCancelEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// // Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(Color.Black, 1f);
        /// //Creates a new  pdf font
        /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
        /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
        /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
        /// string path = @"..\..\Data\Essential studio.txt";
        /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
        /// string text = reader.ReadToEnd();
        /// reader.Close();
        /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
        /// bounds = column;
        /// //Create text element
        /// PdfTextElement element = new PdfTextElement(text, pdfFont);
        /// element.Brush = new PdfSolidBrush(Color.Black);
        /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
        /// layoutFormat.Layout = PdfLayoutType.Paginate;
        /// //Raise the event when the text flows to next page.
        /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
        /// //Get the remaining text that flows beyond the boundary.
        /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
        /// //Saves the document.
        /// doc.Save("Sample.pdf");
        /// //Begin Page Layout Event Handler
        /// private void BeginPageLayout2(object sender, BeginPageLayoutEventArgs e)
        /// {
        ///  e.Cancel=true;
        /// }
        /// </code>
        /// <code lang="VB">
	    /// 'Create a PDF document
	    /// Dim doc As New PdfDocument()
	    /// 'Creates a new page and adds it as the last page of the document
	    /// Dim page As PdfPage = doc.Pages.Add()
	    /// Dim rect As New RectangleF(0, 0, page.GetClientSize().Width, 50)
	    /// Dim brush As New PdfSolidBrush(Color.Black)
	    /// Dim pen As New PdfPen(Color.Black, 1f)
	    /// 'Creates a new  pdf font
	    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	    /// Dim reader As New StreamReader(path, Encoding.ASCII)
	    /// Dim text As string = reader.ReadToEnd()
	    /// reader.Close()
	    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	    ///	'Create text element
	    ///	Dim element As New PdfTextElement(text, pdfFont)
	    ///	element.Brush = New PdfSolidBrush(Color.Black)
	    ///	Dim layoutFormat As New PdfLayoutFormat()
	    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	    ///	layoutFormat.Layout = PdfLayoutType.Paginate
	    ///	'Raise the event when the text flows to next page.
	    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	    ///	'Get the remaining text that flows beyond the boundary.
        ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	    ///	'Saves the document.
	    ///	doc.Save("Sample.pdf")
        /// End Sub
        /// 'Begin Page Layout Event Handler
        /// Private Sub BeginPageLayout2(ByVal sender As object, ByVal e As BeginPageLayoutEventArgs)
	    ///	e.Cancel=True
        /// End Sub
        /// </code>
        /// </example>
        public bool Cancel
        {
            get
            {
                return m_cancel;
            }

            set
            {
                m_cancel = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Data for event before lay outing of the page.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    ///  private bool m_paginateStart = true;
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
    /// PdfPen pen = new PdfPen(Color.Black, 1f);
    /// //Creates a new  pdf font
    /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
    /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
    /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
    /// string path = @"..\..\Data\Essential studio.txt";
    /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
    /// string text = reader.ReadToEnd();
    /// reader.Close();
    /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
    /// //Create text element
    /// PdfTextElement element = new PdfTextElement(text, pdfFont);
    /// element.Brush = new PdfSolidBrush(Color.Black);
    /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
    /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
    /// layoutFormat.Layout = PdfLayoutType.Paginate;
    /// //Raise the event when the text flows to next page.
    /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
    /// //Get the remaining text that flows beyond the boundary.
    /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
    /// //Saves the document.
    /// doc.Save("Sample.pdf");
    /// //Begin Page Layout Event Handler
    /// private void BeginPageLayout2(object sender, BeginPageLayoutEventArgs e)
    /// {
    /// RectangleF bounds = e.Bounds;
    /// // First column.
    /// if (!m_paginateStart)
    /// {
    ///    bounds.X = bounds.Width + 20f;
    ///    bounds.Y = 10f;
    /// }
    /// e.Bounds = bounds;
    /// }
    /// </code>
    /// <code lang="VB">
    /// 'Create a PDF document
    /// Dim doc As New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim m_paginateStart As Bool= True
    /// Dim brush As New PdfSolidBrush(Color.Black)
    /// Dim pen As New PdfPen(Color.Black, 1f)
    /// 'Creates a new  pdf font
    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
    /// Dim reader As New StreamReader(path, Encoding.ASCII)
    /// Dim text As string = reader.ReadToEnd()
    /// reader.Close()
    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
    ///	'Create text element
    ///	Dim element As New PdfTextElement(text, pdfFont)
    ///	element.Brush = New PdfSolidBrush(Color.Black)
    ///	Dim layoutFormat As New PdfLayoutFormat()
    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
    ///	layoutFormat.Layout = PdfLayoutType.Paginate
    ///	'Raise the event when the text flows to next page.
    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
    ///	'Get the remaining text that flows beyond the boundary.
    ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
    ///	'Saves the document.
    ///	doc.Save("Sample.pdf")
    /// 'Begin Page Layout Event Handler
    /// Private Sub BeginPageLayout2(ByVal sender As object, ByVal e As BeginPageLayoutEventArgs)
    /// Dim bounds As RectangleF = e.Bounds
    /// ' First column.
    /// If (Not m_paginateStart) Then
    ///	bounds.X = bounds.Width + 20f
    ///	bounds.Y = 10f
    /// End If
    /// e.Bounds = bounds
    /// End Sub
    /// </code>
    /// </example>
    public class BeginPageLayoutEventArgs : PdfCancelEventArgs
    {
        #region Fields
        /// <summary>
        /// The bounds of the lay outing on the page.
        /// </summary>
        private RectangleF m_bounds;

        /// <summary>
        /// Page where the lay outing should start.
        /// </summary>
        private PdfPage m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value that indicates the lay outing bounds on the page.
        /// </summary>
         /// <example>
        /// <code lang="C#">
        /// // Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        ///  private bool m_paginateStart = true;
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(Color.Black, 1f);
        /// //Creates a new  pdf font
        /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
        /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
        /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
        /// string path = @"..\..\Data\Essential studio.txt";
        /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
        /// string text = reader.ReadToEnd();
        /// reader.Close();
        /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
        /// //Create text element
        /// PdfTextElement element = new PdfTextElement(text, pdfFont);
        /// element.Brush = new PdfSolidBrush(Color.Black);
        /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
        /// layoutFormat.Layout = PdfLayoutType.Paginate;
        /// //Raise the event when the text flows to next page.
        /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
        /// //Get the remaining text that flows beyond the boundary.
        /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
        /// //Saves the document.
        /// doc.Save("Sample.pdf");
        /// //Begin Page Layout Event Handler
        /// private void BeginPageLayout2(object sender, BeginPageLayoutEventArgs e)
        /// {
        /// RectangleF bounds = e.Bounds;
        /// // First column.
        /// if (!m_paginateStart)
        /// {
        ///    bounds.X = bounds.Width + 20f;
        ///    bounds.Y = 10f;
        /// }
        /// e.Bounds = bounds;
        /// }
        /// </code>
        /// <code lang="VB">
	    /// 'Create a PDF document
	    /// Dim doc As New PdfDocument()
	    /// 'Creates a new page and adds it as the last page of the document
	    /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim m_paginateStart As Bool= True
	    /// Dim brush As New PdfSolidBrush(Color.Black)
	    /// Dim pen As New PdfPen(Color.Black, 1f)
	    /// 'Creates a new  pdf font
	    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	    /// Dim reader As New StreamReader(path, Encoding.ASCII)
	    /// Dim text As string = reader.ReadToEnd()
	    /// reader.Close()
	    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	    ///	'Create text element
	    ///	Dim element As New PdfTextElement(text, pdfFont)
	    ///	element.Brush = New PdfSolidBrush(Color.Black)
	    ///	Dim layoutFormat As New PdfLayoutFormat()
	    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	    ///	layoutFormat.Layout = PdfLayoutType.Paginate
	    ///	'Raise the event when the text flows to next page.
	    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	    ///	'Get the remaining text that flows beyond the boundary.
        ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	    ///	'Saves the document.
	    ///	doc.Save("Sample.pdf")
        /// 'Begin Page Layout Event Handler
        /// Private Sub BeginPageLayout2(ByVal sender As object, ByVal e As BeginPageLayoutEventArgs)
	    /// Dim bounds As RectangleF = e.Bounds
        /// ' First column.
	    /// If (Not m_paginateStart) Then
	    ///	bounds.X = bounds.Width + 20f
	    ///	bounds.Y = 10f
	    /// End If
	    /// e.Bounds = bounds
        /// End Sub
        /// </code>
        /// </example>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }

            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets the page where the lay outing should start.
        /// </summary>
         /// <example>
        /// <code lang="C#">
        /// // Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        ///  private bool m_paginateStart = true;
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(Color.Black, 1f);
        /// //Creates a new  pdf font
        /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
        /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
        /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
        /// string path = @"..\..\Data\Essential studio.txt";
        /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
        /// string text = reader.ReadToEnd();
        /// reader.Close();
        /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
        /// //Create text element
        /// PdfTextElement element = new PdfTextElement(text, pdfFont);
        /// element.Brush = new PdfSolidBrush(Color.Black);
        /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
        /// layoutFormat.Layout = PdfLayoutType.Paginate;
        /// //Raise the event when the text flows to next page.
        /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
        /// //Get the remaining text that flows beyond the boundary.
        /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
        /// //Saves the document.
        /// doc.Save("Sample.pdf");
        /// //Begin Page Layout Event Handler
        /// private void BeginPageLayout2(object sender, BeginPageLayoutEventArgs e)
        /// {
        ///  PdfPage page = e.pAGE;
        /// }
        /// </code>
        /// <code lang="VB">
	    /// 'Create a PDF document
	    /// Dim doc As New PdfDocument()
	    /// 'Creates a new page and adds it as the last page of the document
	    /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim m_paginateStart As Bool= True
	    /// Dim brush As New PdfSolidBrush(Color.Black)
	    /// Dim pen As New PdfPen(Color.Black, 1f)
	    /// 'Creates a new  pdf font
	    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	    /// Dim reader As New StreamReader(path, Encoding.ASCII)
	    /// Dim text As string = reader.ReadToEnd()
	    /// reader.Close()
	    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	    ///	'Create text element
	    ///	Dim element As New PdfTextElement(text, pdfFont)
	    ///	element.Brush = New PdfSolidBrush(Color.Black)
	    ///	Dim layoutFormat As New PdfLayoutFormat()
	    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	    ///	layoutFormat.Layout = PdfLayoutType.Paginate
	    ///	'Raise the event when the text flows to next page.
	    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	    ///	'Get the remaining text that flows beyond the boundary.
        ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	    ///	'Saves the document.
	    ///	doc.Save("Sample.pdf")
        /// 'Begin Page Layout Event Handler
        /// Private Sub BeginPageLayout2(ByVal sender As object, ByVal e As BeginPageLayoutEventArgs)
        /// Dim page As PdfPage = e.Page
        /// End Sub
        /// </code>
        /// </example>
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
        /// Initializes a new instance of the <see cref="BeginPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="page">The page.</param>
        public BeginPageLayoutEventArgs(RectangleF bounds, PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            m_bounds = bounds;
            m_page = page;
        }
        #endregion
    }

    /// <summary>
    /// Contains information about layout`s element .
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    ///  private bool m_paginateStart = true;
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
    /// PdfPen pen = new PdfPen(Color.Black, 1f);
    /// //Creates a new  pdf font
    /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
    /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
    /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
    /// string path = @"..\..\Data\Essential studio.txt";
    /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
    /// string text = reader.ReadToEnd();
    /// reader.Close();
    /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
    /// m_columnBounds = column;
    /// //Create text element
    /// PdfTextElement element = new PdfTextElement(text, pdfFont);
    /// element.Brush = new PdfSolidBrush(Color.Black);
    /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
    /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
    /// layoutFormat.Layout = PdfLayoutType.Paginate;
    /// //Raise the event when the text flows to next page.
    /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
    /// //Get the remaining text that flows beyond the boundary.
    /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
    /// //Saves the document.
    /// doc.Save("Sample.pdf");
    /// //End Page Layout Event Handler
    /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
    /// {
    ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
    ///     PdfTextLayoutResult tlr = args.Result;
    ///     RectangleF bounds = tlr.Bounds;
    ///     args.NextPage = tlr.Page;   
    ///  }
    /// </code>
    /// <code lang="VB">
	/// 'Create a PDF document
	/// Dim doc As New PdfDocument()
	/// 'Creates a new page and adds it as the last page of the document
	/// Dim page As PdfPage = doc.Pages.Add()
    /// Dim m_paginateStart As Bool= True
	/// Dim brush As New PdfSolidBrush(Color.Black)
	/// Dim pen As New PdfPen(Color.Black, 1f)
	/// 'Creates a new  pdf font
	/// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	/// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	/// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	/// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	/// Dim reader As New StreamReader(path, Encoding.ASCII)
	/// Dim text As string = reader.ReadToEnd()
	/// reader.Close()
	/// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	///	'Create text element
	///	Dim element As New PdfTextElement(text, pdfFont)
	///	element.Brush = New PdfSolidBrush(Color.Black)
	///	Dim layoutFormat As New PdfLayoutFormat()
	///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	///	layoutFormat.Layout = PdfLayoutType.Paginate
	///	'Raise the event when the text flows to next page.
	///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	///	'Get the remaining text that flows beyond the boundary.
    ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	///	'Saves the document.
	///	doc.Save("Sample.pdf")
    /// 'End Page Layout Event Handler
	/// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
	///	Dim args As EndTextPageLayoutEventArgs = CType(e, EndTextPageLayoutEventArgs)
	///	Dim tlr As PdfTextLayoutResult = args.Result
	///	Dim bounds As RectangleF = tlr.Bounds
	///	args.NextPage = tlr.Page
    ///End Sub
    /// </code>
    /// </example>
    public class EndPageLayoutEventArgs : PdfCancelEventArgs
    {
        #region Fields
        /// <summary>
        /// Layout result.
        /// </summary>
        private PdfLayoutResult m_result;

        /// <summary>
        /// The next page for lay outing.
        /// </summary>
        private PdfPage m_nextPage;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a result of the lay outing on the page.
        /// </summary>
         /// <example>
        /// <code lang="C#">
        /// // Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        ///  private bool m_paginateStart = true;
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(Color.Black, 1f);
        /// //Creates a new  pdf font
        /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
        /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
        /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
        /// string path = @"..\..\Data\Essential studio.txt";
        /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
        /// string text = reader.ReadToEnd();
        /// reader.Close();
        /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
        /// //Create text element
        /// PdfTextElement element = new PdfTextElement(text, pdfFont);
        /// element.Brush = new PdfSolidBrush(Color.Black);
        /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
        /// layoutFormat.Layout = PdfLayoutType.Paginate;
        /// //Raise the event when the text flows to next page.
        /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
        /// //Get the remaining text that flows beyond the boundary.
        /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
        /// //Saves the document.
        /// doc.Save("Sample.pdf");
        /// //End Page Layout Event Handler
        /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
        /// {
        ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
        ///     PdfTextLayoutResult tlr = args.Result;
        ///     RectangleF bounds = tlr.Bounds;
        ///     args.NextPage = tlr.Page;   
        ///  }
        /// </code>
        /// <code lang="VB">
	    /// 'Create a PDF document
	    /// Dim doc As New PdfDocument()
	    /// 'Creates a new page and adds it as the last page of the document
	    /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim m_paginateStart As Bool= True
	    /// Dim brush As New PdfSolidBrush(Color.Black)
	    /// Dim pen As New PdfPen(Color.Black, 1f)
	    /// 'Creates a new  pdf font
	    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	    /// Dim reader As New StreamReader(path, Encoding.ASCII)
	    /// Dim text As string = reader.ReadToEnd()
	    /// reader.Close()
	    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	    ///	'Create text element
	    ///	Dim element As New PdfTextElement(text, pdfFont)
	    ///	element.Brush = New PdfSolidBrush(Color.Black)
	    ///	Dim layoutFormat As New PdfLayoutFormat()
	    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	    ///	layoutFormat.Layout = PdfLayoutType.Paginate
	    ///	'Raise the event when the text flows to next page.
	    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	    ///	'Get the remaining text that flows beyond the boundary.
        ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	    ///	'Saves the document.
	    ///	doc.Save("Sample.pdf")
        /// 'End Page Layout Event Handler
	    /// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
	    ///	Dim args As EndTextPageLayoutEventArgs = CType(e, EndTextPageLayoutEventArgs)
	    ///	Dim tlr As PdfTextLayoutResult = args.Result
	    ///	Dim bounds As RectangleF = tlr.Bounds
	    ///	args.NextPage = tlr.Page
        ///End Sub
        /// </code>
        /// </example>
        public PdfLayoutResult Result
        {
            get
            {
                return m_result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the next page where the element should be layout if the process is not finished or stopped.
        /// </summary>
        /// <remarks>The default value is null. In this case the element will be layout on the next page.</remarks>
        /// <example>
        /// <code lang="C#">
        /// // Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        ///  private bool m_paginateStart = true;
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(Color.Black, 1f);
        /// //Creates a new  pdf font
        /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
        /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
        /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
        /// string path = @"..\..\Data\Essential studio.txt";
        /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
        /// string text = reader.ReadToEnd();
        /// reader.Close();
        /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
        /// //Create text element
        /// PdfTextElement element = new PdfTextElement(text, pdfFont);
        /// element.Brush = new PdfSolidBrush(Color.Black);
        /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
        /// layoutFormat.Layout = PdfLayoutType.Paginate;
        /// //Raise the event when the text flows to next page.
        /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
        /// //Get the remaining text that flows beyond the boundary.
        /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
        /// //Saves the document.
        /// doc.Save("Sample.pdf");
        /// //End Page Layout Event Handler
        /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
        /// {
        ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
        ///     PdfTextLayoutResult tlr = args.Result;
        ///     args.NextPage = tlr.Page;   
        ///  }
        /// </code>
        /// <code lang="VB">
	    /// 'Create a PDF document
	    /// Dim doc As New PdfDocument()
	    /// 'Creates a new page and adds it as the last page of the document
	    /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim m_paginateStart As Bool= True
	    /// Dim brush As New PdfSolidBrush(Color.Black)
	    /// Dim pen As New PdfPen(Color.Black, 1f)
	    /// 'Creates a new  pdf font
	    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
	    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
	    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
	    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
	    /// Dim reader As New StreamReader(path, Encoding.ASCII)
	    /// Dim text As string = reader.ReadToEnd()
	    /// reader.Close()
	    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
	    ///	'Create text element
	    ///	Dim element As New PdfTextElement(text, pdfFont)
	    ///	element.Brush = New PdfSolidBrush(Color.Black)
	    ///	Dim layoutFormat As New PdfLayoutFormat()
	    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
	    ///	layoutFormat.Layout = PdfLayoutType.Paginate
	    ///	'Raise the event when the text flows to next page.
	    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
	    ///	'Get the remaining text that flows beyond the boundary.
        ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
	    ///	'Saves the document.
	    ///	doc.Save("Sample.pdf")
        /// 'End Page Layout Event Handler.
	    /// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
	    ///	Dim tlr As PdfTextLayoutResult = args.Result
	    ///	args.NextPage = tlr.Page
        ///End Sub
        /// </code>
        /// </example>
        public PdfPage NextPage
        {
            get
            {
                return m_nextPage;
            }

            set
            {
                m_nextPage = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public EndPageLayoutEventArgs(PdfLayoutResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            m_result = result;
        }
        #endregion
    }

    /// <summary>
    /// Contains information about layout`s element .
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    ///  private bool m_paginateStart = true;
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
    /// PdfPen pen = new PdfPen(Color.Black, 1f);
    /// //Creates a new  pdf font
    /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
    /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
    /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
    /// string path = @"..\..\Data\Essential studio.txt";
    /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
    /// string text = reader.ReadToEnd();
    /// reader.Close();
    /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
    /// //Create text element
    /// PdfTextElement element = new PdfTextElement(text, pdfFont);
    /// element.Brush = new PdfSolidBrush(Color.Black);
    /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
    /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
    /// layoutFormat.Layout = PdfLayoutType.Paginate;
    /// //Raise the event when the text flows to next page.
    /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
    /// //Get the remaining text that flows beyond the boundary.
    /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
    /// //Saves the document.
    /// doc.Save("Sample.pdf");
    /// //End Text Page Layout Event
    /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
    /// {
    ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
    ///     PdfTextLayoutResult tlr = args.Result;
    ///     RectangleF bounds = tlr.Bounds;
    ///     args.NextPage = tlr.Page;   
    ///  }
    /// </code>
    /// <code lang="VB">
    /// 'Create a PDF document
    /// Dim doc As New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim m_paginateStart As Bool= True
    /// Dim brush As New PdfSolidBrush(Color.Black)
    /// Dim pen As New PdfPen(Color.Black, 1f)
    /// 'Creates a new  pdf font
    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
    /// Dim reader As New StreamReader(path, Encoding.ASCII)
    /// Dim text As string = reader.ReadToEnd()
    /// reader.Close()
    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
    ///	'Create text element
    ///	Dim element As New PdfTextElement(text, pdfFont)
    ///	element.Brush = New PdfSolidBrush(Color.Black)
    ///	Dim layoutFormat As New PdfLayoutFormat()
    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
    ///	layoutFormat.Layout = PdfLayoutType.Paginate
    ///	'Raise the event when the text flows to next page.
    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
    ///	'Get the remaining text that flows beyond the boundary.
    ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
    ///	'Saves the document.
    ///	doc.Save("Sample.pdf")
    /// 'End Text Page Layout Event
    /// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
    ///	Dim args As EndTextPageLayoutEventArgs = CType(e, EndTextPageLayoutEventArgs)
    ///	Dim tlr As PdfTextLayoutResult = args.Result
    ///	Dim bounds As RectangleF = tlr.Bounds
    ///	args.NextPage = tlr.Page
    ///End Sub
    /// </code>
    /// </example>
    public class EndTextPageLayoutEventArgs : EndPageLayoutEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EndTextPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public EndTextPageLayoutEventArgs(PdfTextLayoutResult result)
            : base(result)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a result of the lay outing on the page.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        ///  private bool m_paginateStart = true;
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(Color.Black, 1f);
        /// //Creates a new  pdf font
        /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
        /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
        /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
        /// string path = @"..\..\Data\Essential studio.txt";
        /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
        /// string text = reader.ReadToEnd();
        /// reader.Close();
        /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
        /// //Create text element
        /// PdfTextElement element = new PdfTextElement(text, pdfFont);
        /// element.Brush = new PdfSolidBrush(Color.Black);
        /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
        /// layoutFormat.Layout = PdfLayoutType.Paginate;
        /// //Raise the event when the text flows to next page.
        /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
        /// //Get the remaining text that flows beyond the boundary.
        /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
        /// //Saves the document.
        /// doc.Save("Sample.pdf");
        /// //End Text Page Layout Event
        /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
        /// {
        ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
        ///     PdfTextLayoutResult tlr = args.Result;
        ///     RectangleF bounds = tlr.Bounds;
        ///  }
        /// </code>
        /// <code lang="VB">
        /// 'Create a PDF document
        /// Dim doc As New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim m_paginateStart As Bool= True
        /// Dim brush As New PdfSolidBrush(Color.Black)
        /// Dim pen As New PdfPen(Color.Black, 1f)
        /// 'Creates a new  pdf font
        /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
        /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
        /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
        /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
        /// Dim reader As New StreamReader(path, Encoding.ASCII)
        /// Dim text As string = reader.ReadToEnd()
        /// reader.Close()
        /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
        ///	'Create text element
        ///	Dim element As New PdfTextElement(text, pdfFont)
        ///	element.Brush = New PdfSolidBrush(Color.Black)
        ///	Dim layoutFormat As New PdfLayoutFormat()
        ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
        ///	layoutFormat.Layout = PdfLayoutType.Paginate
        ///	'Raise the event when the text flows to next page.
        ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
        ///	'Get the remaining text that flows beyond the boundary.
        ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
        ///	'Saves the document.
        ///	doc.Save("Sample.pdf")
        /// 'End Text Page Layout Event
        /// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
        ///	Dim args As EndTextPageLayoutEventArgs = CType(e, EndTextPageLayoutEventArgs)
        ///	Dim tlr As PdfTextLayoutResult = args.Result
        ///	Dim bounds As RectangleF = tlr.Bounds
        ///End Sub
        /// </code>
        /// </example>
        new public PdfTextLayoutResult Result
        {
            get
            {
                return (base.Result as PdfTextLayoutResult);
            }
        }
        #endregion
    }

    #region Delegates
    /// <summary>
    /// Delegate. Defines a type of the event before lay outing on the page.
    /// </summary>

    public delegate void BeginPageLayoutEventHandler(object sender, BeginPageLayoutEventArgs e);
    /// <summary>
    /// Delegate. Defines a type of the event after lay outing on the page.
    /// </summary>

    /// <example>
    /// <code lang="C#">
    /// // Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    ///  private bool m_paginateStart = true;
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
    /// PdfPen pen = new PdfPen(Color.Black, 1f);
    /// //Creates a new  pdf font
    /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
    /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
    /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
    /// string path = @"..\..\Data\Essential studio.txt";
    /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
    /// string text = reader.ReadToEnd();
    /// reader.Close();
    /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
    /// //Create text element
    /// PdfTextElement element = new PdfTextElement(text, pdfFont);
    /// element.Brush = new PdfSolidBrush(Color.Black);
    /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
    /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
    /// layoutFormat.Layout = PdfLayoutType.Paginate;
    /// //Raise the event when the text flows to next page.
    /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
    /// //Get the remaining text that flows beyond the boundary.
    /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
    /// //Saves the document.
    /// doc.Save("Sample.pdf");
    /// //End Page Layout Event Handler
    /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
    /// {
    ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
    ///     PdfTextLayoutResult tlr = args.Result;
    ///     RectangleF bounds = tlr.Bounds;
    ///     args.NextPage = tlr.Page;   
    ///  }
    /// </code>
    /// <code lang="VB">
    /// 'Create a PDF document
    /// Dim doc As New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim m_paginateStart As Bool= True
    /// Dim brush As New PdfSolidBrush(Color.Black)
    /// Dim pen As New PdfPen(Color.Black, 1f)
    /// 'Creates a new  pdf font
    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
    /// Dim reader As New StreamReader(path, Encoding.ASCII)
    /// Dim text As string = reader.ReadToEnd()
    /// reader.Close()
    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
    ///	'Create text element
    ///	Dim element As New PdfTextElement(text, pdfFont)
    ///	element.Brush = New PdfSolidBrush(Color.Black)
    ///	Dim layoutFormat As New PdfLayoutFormat()
    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
    ///	layoutFormat.Layout = PdfLayoutType.Paginate
    ///	'Raise the event when the text flows to next page.
    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
    ///	'Get the remaining text that flows beyond the boundary.
    ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
    ///	'Saves the document.
    ///	doc.Save("Sample.pdf")
    /// 'End Page Layout Event Handler
    /// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
    ///	Dim args As EndTextPageLayoutEventArgs = CType(e, EndTextPageLayoutEventArgs)
    ///	Dim tlr As PdfTextLayoutResult = args.Result
    ///	Dim bounds As RectangleF = tlr.Bounds
    ///	args.NextPage = tlr.Page
    ///End Sub
    /// </code>
    /// </example>
    public delegate void EndPageLayoutEventHandler(object sender, EndPageLayoutEventArgs e);
    /// <summary>
    /// Delegate. Defines a type of the event after the text lay outing on the page.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    ///  private bool m_paginateStart = true;
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
    /// PdfPen pen = new PdfPen(Color.Black, 1f);
    /// //Creates a new  pdf font
    /// PdfStandardFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11.5f);
    /// Font font = new Font("Calibri", 14f, FontStyle.Bold);
    /// PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(font, true);
    /// string path = @"..\..\Data\Essential studio.txt";
    /// StreamReader reader = new StreamReader(path, Encoding.ASCII);
    /// string text = reader.ReadToEnd();
    /// reader.Close();
    /// RectangleF column = new RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height);
    /// //Create text element
    /// PdfTextElement element = new PdfTextElement(text, pdfFont);
    /// element.Brush = new PdfSolidBrush(Color.Black);
    /// PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
    /// layoutFormat.Break = PdfLayoutBreakType.FitPage;
    /// layoutFormat.Layout = PdfLayoutType.Paginate;
    /// //Raise the event when the text flows to next page.
    /// element.BeginPageLayout += new BeginPageLayoutEventHandler(BeginPageLayout2);
    /// //Get the remaining text that flows beyond the boundary.
    /// PdfTextLayoutResult result = element.Draw(page, column, layoutFormat);
    /// //Saves the document.
    /// doc.Save("Sample.pdf");
    /// //End Text Page Layout Event Handler
    /// private void EndPageLayout2(object sender, EndPageLayoutEventArgs e)
    /// {
    ///     EndTextPageLayoutEventArgs args = (EndTextPageLayoutEventArgs)e;
    ///     PdfTextLayoutResult tlr = args.Result;
    ///     RectangleF bounds = tlr.Bounds;
    ///     args.NextPage = tlr.Page;   
    ///  }
    /// </code>
    /// <code lang="VB">
    /// 'Create a PDF document
    /// Dim doc As New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim m_paginateStart As Bool= True
    /// Dim brush As New PdfSolidBrush(Color.Black)
    /// Dim pen As New PdfPen(Color.Black, 1f)
    /// 'Creates a new  pdf font
    /// Dim pdfFont As New PdfStandardFont(PdfFontFamily.Helvetica, 11.5f)
    /// Dim font As New Font("Calibri", 14f, FontStyle.Bold)
    /// Dim trueTypeFont As New PdfTrueTypeFont(font, True)
    /// Dim path As string = "..\..\..\..\..\..\..\..\..\Common\Data\PDF\Essential studio.txt"
    /// Dim reader As New StreamReader(path, Encoding.ASCII)
    /// Dim text As string = reader.ReadToEnd()
    /// reader.Close()
    /// Dim column As New RectangleF(0, 20, page.Graphics.ClientSize.Width / 2f - 10f, page.Graphics.ClientSize.Height)
    ///	'Create text element
    ///	Dim element As New PdfTextElement(text, pdfFont)
    ///	element.Brush = New PdfSolidBrush(Color.Black)
    ///	Dim layoutFormat As New PdfLayoutFormat()
    ///	layoutFormat.Break = PdfLayoutBreakType.FitPage
    ///	layoutFormat.Layout = PdfLayoutType.Paginate
    ///	'Raise the event when the text flows to next page.
    ///	element.BeginPageLayout += New BeginPageLayoutEventHandler(BeginPageLayout2)
    ///	'Get the remaining text that flows beyond the boundary.
    ///	Dim result As PdfTextLayoutResult = element.Draw(page, column, layoutFormat)
    ///	'Saves the document.
    ///	doc.Save("Sample.pdf")
    /// 'End Text Page Layout Event Handler
    /// Private Sub EndPageLayout2(ByVal sender As object, ByVal e As EndPageLayoutEventArgs)
    ///	Dim args As EndTextPageLayoutEventArgs = CType(e, EndTextPageLayoutEventArgs)
    ///	Dim tlr As PdfTextLayoutResult = args.Result
    ///	Dim bounds As RectangleF = tlr.Bounds
    ///	args.NextPage = tlr.Page
    /// End Sub
    /// </code>
    /// </example>
    public delegate void EndTextPageLayoutEventHandler(object sender, EndTextPageLayoutEventArgs e);
    #endregion
}
