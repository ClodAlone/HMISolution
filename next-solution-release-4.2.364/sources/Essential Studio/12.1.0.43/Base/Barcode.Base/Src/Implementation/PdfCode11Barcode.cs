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
    /// Represents a Code11 barcode.
    /// </summary>
# if !XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create pdfFont and pdfFont style.
    /// PdfFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Creates a new PdfCode11Barcode.
    /// PdfCode11Barcode code11 = new PdfCode11Barcode();
    /// //Set the pdffont to code11 barcode.
    /// code11.Font = pdfFont;
    /// //Set the barcode text.
    /// code11.Text = "012345678";
    /// //Draw a barcode in the new Page.
    /// code11.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code11.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create pdfFont and pdfFont style.
    /// Dim pdfFont As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode11Barcode.
    /// Dim code11 As PdfCode11Barcode = New PdfCode11Barcode()
    /// 'Set the pdffont to code11 barcode.
    /// code11.Font = pdfFont;
    /// 'Set the barcode text.
    /// code11.Text = "012345678"
    /// 'Draw a barcode in the new Page.
    /// code11.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code11.pdf")
    /// </code>
    /// </example>
    /// <remarks> Only the following symbols are allowed in a Code 11 barcode: 0 1 2 3 4 5 6 7 8 9 -</remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode11Barcode : PdfUnidimensionalBarcode
#else
    public class Code11Barcode : UnidimensionalBarcode
#endif
    {
#region Constructor
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode11Barcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create pdfFont and pdfFont style.
        /// PdfFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode11Barcode.
        /// PdfCode11Barcode code11 = new PdfCode11Barcode();
        /// //Set the pdffont to code11 barcode.
        /// code11.Font = pdfFont;
        /// //Set the barcode text.
        /// code11.Text = "012345678";
        /// //Draw a barcode in the new Page.
        /// code11.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code11.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdfFont and pdfFont style.
        /// Dim pdfFont As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode11Barcode.
        /// Dim code11 As PdfCode11Barcode = New PdfCode11Barcode()
        /// 'Set the pdffont to code11 barcode.
        /// code11.Font = pdfFont;
        /// 'Set the barcode text.
        /// code11.Text = "012345678"
        /// 'Draw a barcode in the new Page.
        /// code11.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code11.pdf")
        /// </code>
        /// </example>
        public PdfCode11Barcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code11Barcode"/> class.
        /// </summary>
        public Code11Barcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode11Barcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode Text.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create pdfFont and pdfFont style.
        /// PdfFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode11Barcode.
        /// PdfCode11Barcode code11 = new PdfCode11Barcode("012345678");
        /// //Set the pdffont to code11 barcode.
        /// code11.Font = pdfFont;
        /// //Draw a barcode in the new Page.
        /// code11.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code11.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdfFont and pdfFont style.
        /// Dim pdfFont As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode11Barcode.
        /// Dim code11 As PdfCode11Barcode = New PdfCode11Barcode("012345678")
        /// 'Set the pdffont to code11 barcode.
        /// code11.Font = pdfFont
        /// 'Draw a barcode in the new Page.
        /// code11.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code11.pdf")
        /// </code>
        /// </example>
        /// <param name="text">The Barcode Text.</param>
        public PdfCode11Barcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code11Barcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode Text.</param>
        public Code11Barcode(string text)
#endif
            : this()
        {
            base.Text = text;
        }

        #endregion

#region Methods

        /// <summary>
        /// Calculates the check digit for this barcode specification.
        /// </summary>
        /// <returns>The Check digits.</returns>
        protected internal override char[] CalculateCheckDigit()
        {
            string code = base.Text;
            int checksumCount = 0;
            int w = 1;
            while (checksumCount != -1)
            {
                int[] weights = new int[code.Length];
                int length = code.Length;

                for (int i = length - 1; i >= 0; i--)
                {
                    weights[i] = w;
                    w += 1;
                    if (w == 11 && checksumCount == 0)
                    {
                        w = 1;
                    }

                    if (w == 10 && checksumCount == 1)
                    {
                        w = 1;
                    }
                }

                int num = 0;
                for (int j = code.Length - 1; j >= 0; j--)
                {
                    if (code[j] == '-')
                        num += 10 * weights[j];
                    else
                    {
                        int k = int.Parse(code[j].ToString());
                        num += k * weights[j];
                    }
                }

                num = num % 11;
                char CCheckDigit = GetSymbol(num);
                code = code + CCheckDigit.ToString();

                if (code.Length >= 10 && (code.Length - base.Text.Length <= 2) && checksumCount != 1)
                {
                    checksumCount++;
                }
                else
                {
                    checksumCount = -1;
                }
            }

#if !WINDOWS_PHONE && !BARCODE_SILVERLIGHT
            char[] checkSymbols = code.ToCharArray(base.Text.Length, code.Length - base.Text.Length);
#else
            char[] checkSymbols = code.Substring(base.Text.Length, code.Length - base.Text.Length).ToCharArray();
#endif

            return checkSymbols;
        }

        /// <summary>
        /// Initializes the internal barcode symbol table
        /// </summary>
        private void Initialize()
        {
            base.StartSymbol = '*';
            base.StopSymbol = '*';

            base.ValidatorExpression = @"^[0-9\-]*$";

            base.BarcodeSymbols['0'] = new BarcodeSymbolTable('0', 0, new byte[] { 1, 1, 1, 1, 2 });
            base.BarcodeSymbols['1'] = new BarcodeSymbolTable('1', 1, new byte[] { 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['2'] = new BarcodeSymbolTable('2', 2, new byte[] { 1, 2, 1, 1, 2 });
            base.BarcodeSymbols['3'] = new BarcodeSymbolTable('3', 3, new byte[] { 2, 2, 1, 1, 1 });
            base.BarcodeSymbols['4'] = new BarcodeSymbolTable('4', 4, new byte[] { 1, 1, 2, 1, 2 });
            base.BarcodeSymbols['5'] = new BarcodeSymbolTable('5', 5, new byte[] { 2, 1, 2, 1, 1 });
            base.BarcodeSymbols['6'] = new BarcodeSymbolTable('6', 6, new byte[] { 1, 2, 2, 1, 1 });
            base.BarcodeSymbols['7'] = new BarcodeSymbolTable('7', 7, new byte[] { 1, 1, 1, 2, 2 });
            base.BarcodeSymbols['8'] = new BarcodeSymbolTable('8', 8, new byte[] { 2, 1, 1, 2, 1 });
            base.BarcodeSymbols['9'] = new BarcodeSymbolTable('9', 9, new byte[] { 2, 1, 1, 1, 1 });
            base.BarcodeSymbols['-'] = new BarcodeSymbolTable('-', 10, new byte[] { 1, 1, 2, 1, 1 });
            base.BarcodeSymbols['*'] = new BarcodeSymbolTable('*', 0, new byte[] { 1, 1, 2, 2, 1 });
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        /// <param name="checkValue">The check value.</param>
        /// <returns> Symbol</returns>
        private char GetSymbol(int checkValue)
        {
            foreach (KeyValuePair<char, BarcodeSymbolTable> entry in base.BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.CheckDigit == checkValue)
                {
                    return pattern.Symbol;
                }
            }

            return '\0';
        }

        /// <summary>
        /// Gets the symbol row.
        /// </summary>
        /// <param name="checkValue">The check value.</param>
        /// <returns>barcode symbol table</returns>
        private BarcodeSymbolTable GetSymbolRow(int checkValue)
        {
            foreach (KeyValuePair<char,BarcodeSymbolTable> entry in base.BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.CheckDigit == checkValue)
                {
                    return pattern;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif