#if !SILVERLIGHT
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if !XAML
using System.Drawing;
#endif

# if WPF || BARCODE_SILVERLIGHT || BARCODE_WINRT
namespace Syncfusion.UI.Xaml.Controls.Barcode
#elif WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Barcode
#elif ASPNET
namespace Syncfusion.Web.UI.WebControls.Barcode
#elif WINFORMS
namespace Syncfusion.Windows.Forms.Barcode
#elif MVC
namespace Syncfusion.Mvc.Barcode
#else
/// <summary>
/// The Syncfusion.Pdf.Barcode namespace contains classes for creating barcodes.
/// </summary>
namespace Syncfusion.Pdf.Barcode
#endif
{
    /// <summary>
    ///  Represents a Codabar barcode.
    /// </summary>
# if !XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Creates a new PdfCodabarBarcode.
    /// PdfCodabarBarcode codaBarcode = new PdfCodabarBarcode();
    /// //Set the font to codabarcode.
    /// codaBarcode.Font = font;
    /// //Set the barcode text.
    /// codaBarcode.Text = "0123";
    /// //Draw a barcode in the new Page.
    /// codaBarcode.Draw(page, new PointF(25, 500));
    /// //Save the document to disk.
    /// document.Save("CodaBarcode.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCodabarBarcode.
    /// Dim codaBarcode As PdfCodabarBarcode = New PdfCodabarBarcode()
    /// 'Set the font..
    /// codaBarcode.Font = font
    /// 'Set the barcode text.
    /// codaBarcode.Text = "0123"
    /// 'Draw a barcode in the new Page.
    /// codaBarcode.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("CodaBarcode.pdf")
    /// </code>
    /// </example>
    ///<remarks> This symbology allows the encoding of strings of up to 16 digits, 10 numeric digits (0 through 9) and 
    /// 6 special non alpha characters ("+", "-", "$", "/", ":", "."). 
    /// </remarks>
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCodabarBarcode : PdfUnidimensionalBarcode
#else
    public class CodabarBarcode : UnidimensionalBarcode
#endif
    {
        #region Constructor
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCodabarBarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCodabarBarcode.
        /// PdfCodabarBarcode codaBarcode = new PdfCodabarBarcode();
        /// //Set the font to codabarcode.
        /// codaBarcode.Font = font;
        /// //Set the barcode text.
        /// codaBarcode.Text = "0123";
        /// //Draw a barcode in the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("CodaBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCodabarBarcode.
        /// Dim codaBarcode As PdfCodabarBarcode = New PdfCodabarBarcode()
        /// 'Set the font..
        /// codaBarcode.Font = font
        /// 'Set the barcode text.
        /// codaBarcode.Text = "0123"
        /// 'Draw a barcode in the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("CodaBarcode.pdf")
        /// </code>
        /// </example>
        public PdfCodabarBarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CodabarBarcode"/> class.
        /// </summary>
        public CodabarBarcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCodabarBarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode Text.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCodabarBarcode.
        /// PdfCodabarBarcode codaBarcode = new PdfCodabarBarcode("0123");
        /// //Set the font to codabarcode.
        /// codaBarcode.Font = font;
        /// //Draw a barcode in the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("CodaBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCodabarBarcode.
        /// Dim codaBarcode As PdfCodabarBarcode = New PdfCodabarBarcode("0123")
        /// 'Set the font..
        /// codaBarcode.Font = font
        /// 'Draw a barcode in the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("CodaBarcode.pdf")
        /// </code>
        /// </example>
        public PdfCodabarBarcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CodabarBarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode Text.</param>
        public CodabarBarcode(string text)
#endif
            : this()
        {
            base.Text = text;
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            base.StartSymbol = 'A';
            base.StopSymbol = 'B';

            base.ValidatorExpression = @"^[\d\-\$\:\/\.\+]+$";

            base.BarcodeSymbols['0'] = new BarcodeSymbolTable('0', 0, new byte[] { 1, 1, 1, 1, 1, 2, 2 });
            base.BarcodeSymbols['1'] = new BarcodeSymbolTable('1', 0, new byte[] { 1, 1, 1, 1, 2, 2, 1 });
            base.BarcodeSymbols['2'] = new BarcodeSymbolTable('2', 0, new byte[] { 1, 1, 1, 2, 1, 1, 2 });
            base.BarcodeSymbols['3'] = new BarcodeSymbolTable('3', 0, new byte[] { 2, 2, 1, 1, 1, 1, 1 });
            base.BarcodeSymbols['4'] = new BarcodeSymbolTable('4', 0, new byte[] { 1, 1, 2, 1, 1, 2, 1 });
            base.BarcodeSymbols['5'] = new BarcodeSymbolTable('5', 0, new byte[] { 2, 1, 1, 1, 1, 2, 1 });
            base.BarcodeSymbols['6'] = new BarcodeSymbolTable('6', 0, new byte[] { 1, 2, 1, 1, 1, 1, 2 });
            base.BarcodeSymbols['7'] = new BarcodeSymbolTable('7', 0, new byte[] { 1, 2, 1, 1, 2, 1, 1 });
            base.BarcodeSymbols['8'] = new BarcodeSymbolTable('8', 0, new byte[] { 1, 2, 2, 1, 1, 1, 1 });
            base.BarcodeSymbols['9'] = new BarcodeSymbolTable('9', 0, new byte[] { 2, 1, 1, 2, 1, 1, 1 });
            base.BarcodeSymbols['-'] = new BarcodeSymbolTable('-', 0, new byte[] { 1, 1, 1, 2, 2, 1, 1 });
            base.BarcodeSymbols['$'] = new BarcodeSymbolTable('$', 0, new byte[] { 1, 1, 2, 2, 1, 1, 1 });
            base.BarcodeSymbols[':'] = new BarcodeSymbolTable(':', 0, new byte[] { 2, 1, 1, 1, 2, 1, 2 });
            base.BarcodeSymbols['/'] = new BarcodeSymbolTable('/', 0, new byte[] { 2, 1, 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['.'] = new BarcodeSymbolTable('.', 0, new byte[] { 2, 1, 2, 1, 2, 1, 1 });
            base.BarcodeSymbols['+'] = new BarcodeSymbolTable('+', 0, new byte[] { 1, 1, 2, 1, 2, 1, 2 });
            base.BarcodeSymbols['A'] = new BarcodeSymbolTable('A', 0, new byte[] { 1, 1, 2, 2, 1, 2, 1 });
            base.BarcodeSymbols['B'] = new BarcodeSymbolTable('B', 0, new byte[] { 1, 1, 1, 2, 1, 2, 2 });
        }
        #endregion


    }
}
#endif