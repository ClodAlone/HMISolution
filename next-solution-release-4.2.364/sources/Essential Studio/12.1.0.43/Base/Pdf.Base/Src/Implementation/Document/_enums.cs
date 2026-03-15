#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Specifies the type of PDF file format.
    /// </summary>
    internal enum PdfFileFormat
    {
        /// <summary>
        /// Specifies plain PDF file format.
        /// </summary>
        Plain,

        /// <summary>
        /// Specifies Linearized PDF file format.
        /// </summary>
        Linearized
    }

    /// <summary>
    /// Specifies the different way of presenting the document at the client browser.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new document.
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document 
    /// PdfPage page = doc.Pages.Add();
    /// // Load an existing image            
    /// PdfBitmap bmp = new PdfBitmap("Logo.png");
    /// // Draw the image
    /// page.Graphics.DrawImage(bmp, 20, 20, 100, 200);
    /// doc.Save("Sample.pdf", Response, HttpReadType.Open);   
    /// </code>
    /// <code lang="VB">
    /// 'Create a new document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document 
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Load an existing image            
    /// Dim bmp As PdfBitmap = New PdfBitmap("Logo.png")
    /// ' Draw the image
    /// page.Graphics.DrawImage(bmp, 20, 20, 100, 200)
    /// doc.Save("Sample.pdf", Response, HttpReadType.Open)
    /// </code>
    /// </example>
    /// <seealso cref="Syncfusion.Pdf.Page"/> Class   
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfBitmap"/> Class   
    public enum HttpReadType
    {
        /// <summary>
        /// Send the generated document to the client browser and will open document inside browser or using application associated with .pdf extension externally.
        /// </summary>
        Open,

        /// <summary>
        /// Send the generated document to the client browser and presents an option to save the document to disk or open inside the browser.
        /// </summary>
        Save
    }

    /// <summary>
    /// Specifies the available PDF versions to save a PDF document.
    /// </summary>
    /// <example>
    /// <remarks>Default Value is Version1_5</remarks>
    /// <code lang="C#">
    /// //Create a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document 
    /// PdfPage page = doc.Pages.Add();    
    /// // Set the  pdf version as Version1_7
    /// doc.FileStructure.Version = PdfVersion.Version1_7;
    /// // Save the document
    /// doc.Save("FileStructure.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document 
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Set the  pdf version as Version1_7
    /// doc.FileStructure.Version = PdfVersion.Version1_7
    /// ' Save the document
    /// doc.Save("FileStructure.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="FileStructure"/> Class       
    public enum PdfVersion
    {
        /// <summary>
        /// PDF version 1.0.
        /// </summary>
        Version1_0,

        /// <summary>
        /// PDF version 1.1.
        /// </summary>
        Version1_1,

        /// <summary>
        /// PDF version 1.2.
        /// </summary>
        Version1_2,

        /// <summary>
        /// PDF version 1.3. Adobe Acrobat 4.
        /// </summary>
        Version1_3,

        /// <summary>
        /// PDF version 1.4. Adobe Acrobat 5.
        /// </summary>
        Version1_4,

        /// <summary>
        /// PDF version 1.5. Adobe Acrobat 6.
        /// </summary>
        Version1_5,

        /// <summary>
        /// PDF version 1.6. Adobe Acrobat 7.
        /// </summary>
        Version1_6,

        /// <summary>
        /// PDF version 1.7. Adobe Acrobat 8.
        /// </summary>
        Version1_7,
    }

    /// <summary>
    /// Specifies the type of the PDF cross-reference.
    /// </summary>
    /// <remarks>Default value is CrossReferenceStream</remarks>
    /// <example>
    /// <code lang="C#">
    /// //Create a new document
    /// PdfDocument doc = new PdfDocument();
    /// // Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// //Set the CrossReferenceType as CrossReferenceStream
    /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream;  
    /// // Save the document
    /// doc.Save("FileStructure.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Set the CrossReferenceType as CrossReferenceStream
    /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream    
    /// ' Save the document
    /// doc.Save("FileStructure.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="FileStructure"/> Class    
    public enum PdfCrossReferenceType
    {
        /// <summary>
        /// The cross-reference table contains information that permits random access to indirect objects within the file so that the entire file need not be read to locate any particular object. The structure is useful for incremental updates, since it allows a new cross-reference section to be added to the PDF file, containing entries only for objects that have been added or deleted. Cross-reference is represented by cross-reference table. The cross-reference table is the traditional way of representing reference type.
        /// </summary>
        CrossReferenceTable,

        /// <summary>
        /// Cross-reference is represented by cross-reference stream. Cross-reference streams are stream objects, and contain a dictionary and a data stream.
        /// This leads to more compact representation of the file data especially along with the compression enabled.
        /// This format is supported by PDF 1.5 version and higher only.
        /// </summary>
        CrossReferenceStream
    }

    /// <summary>
    /// Specifies the Pdf document's Conformance-level.
    /// </summary>
    /// <example>
    /// <remarks>Default value is None.</remarks>
    /// <code lang="C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A1B);
    /// //Creates a new page and adds it as the last page of the document to the document.
    /// PdfPage page = document.Pages.Add();
    /// // Create a 'Times New Roman' font
    /// Font font = new Font("Times New Roman", 10);
    /// // Create font with bold font style.
    /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
    /// //Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", pdfFont, PdfBrushes.Black, new PointF(10, 10));
    /// //Save document to disk.
    /// document.Save("ConformanceLevel.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument(PdfConformanceLevel.Pdf_A1B)
    /// ' Create a page to the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// ' Create a 'Times New Roman' font
    /// Dim font As Font = New Font("Times New Roman", 10)
    /// ' Create font with bold font style.
    /// Dim pdfFont As PdfFont = New PdfTrueTypeFont(font, False)
    /// 'Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", pdfFont, PdfBrushes.Black, New PointF(10, 10))
    /// 'Save document to disk.
    /// document.Save("ConformanceLevel.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="Syncfusion.Pdf.PdfDocument"/> Class    
    /// <seealso cref="Syncfusion.Pdf.Graphics.Font"/> Class    
    public enum PdfConformanceLevel
    {
        /// <summary>
        /// Specifies Default / No Conformance.
        /// </summary>
        None,

        /// <summary>
        /// This PDF/A ISO standard [ISO 19005-1:2005] is based on Adobe PDF version 1.4
        /// and This Level B conformance indicates minimal compliance to ensure that the 
        /// rendered visual appearance of a conforming file is preservable over the long term.
        /// </summary>
        Pdf_A1B,

        /// <summary>
        /// This PDF/X-1a:2001 ISO standard [ISO 15930-1] is based on Adobe PDF version 1.3
        /// which uses only CMYK + Spot Color and this compliance to ensure that the 
        /// contents will be reliably reproduced in the repress environment.
        /// </summary>
        Pdf_X1A2001
    }

    /// <summary>
    /// Specifies the different page scaling option that shall be selected when a print dialog is displayed for this document.
    /// </summary>
    /// <remarks>Default value is AppDefault.</remarks>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDF document
    /// PdfDocument document = new PdfDocument();
    /// // Set AppDefault mode as page`s scaling mode 
    /// document.ViewerPreferences.PageScaling = PageScalingMode.AppDefault;
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font with Bold font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
    /// //Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
    /// //Save documen to disk.
    /// document.Save("ScalingMode.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Set AppDefault mode as page`s scaling mode
    /// document.ViewerPreferences.PageScaling = PageScalingMode.AppDefault
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font with Bold font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
    /// 'Save documen to disk.
    /// document.Save("ScalingMode.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfViewerPreferences"/> Class    
    /// <seealso cref="PdfDocument"/> Class    
    public enum PageScalingMode
    {
        /// <summary>
        /// Indicates the conforming reader�s default print scaling.
        /// </summary>
        AppDefault,

        /// <summary>
        /// Indicates no page scaling.
        /// </summary>
        None
    }

    /// <summary>
    /// indicates the type of the portfolio schema field.
    /// </summary>
    public enum PdfPortfolioSchemaFieldType
    {
        String,
        Date,
        Number,
        FileName,
        Description,
        ModDate,
        CreationDate,
        Size,
    }

    /// <summary>
    /// indicates the type of the portfolio view mode.
    /// </summary>
    public enum PdfPortfolioViewMode
    {
        Details,
        Tile,
        Hidden
    }
}
