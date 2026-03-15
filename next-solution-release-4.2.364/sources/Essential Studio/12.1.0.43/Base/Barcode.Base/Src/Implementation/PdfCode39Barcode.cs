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
    /// Represents a Code39 barcode.
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
    /// //Creates a new PdfCode39Barcode.
    /// PdfCode39Barcode code39 = new PdfCode39Barcode();
    /// //Set the font to code39.
    /// code32.Font = font;
    /// //Set the barcode text.
    /// code39.Text = "CODE39";
    /// //Draw a barcode in the new Page.
    /// code39.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code39.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode39Barcode.
    /// Dim code39 As PdfCode39Barcode = New PdfCode39Barcode()
    /// 'Set the font to code39.
    /// code39.Font = font
    /// 'Set the barcode text.
    /// code39.Text = "CODE39"
    /// 'Draw a barcode in the new Page.
    /// code39.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code39.pdf")
    /// </code>
    /// </example> 
    /// <remarks> Only the following symbols are allowed in a Code 39 barcode:Only the following symbols are allowed in a Code 39 barcode: 1 2 3 4 5 6 7 8 9 0 A B C D E F G H I J K L M N O P Q R S T U V W X Y Z - . $ / + % SPACE
    /// All alphabetic characters are uppercase. If lowercase characters are required, then a Code 39 Extended barcode must be used.
    /// </remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode39Barcode : PdfUnidimensionalBarcode
#else
    public class Code39Barcode : UnidimensionalBarcode
#endif
    {
        #region Constructor
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode39Barcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode39Barcode.
        /// PdfCode39Barcode code39 = new PdfCode39Barcode();
        /// //Set the font to code39.
        /// code32.Font = font;
        /// //Set the barcode text.
        /// code39.Text = "CODE39";
        /// //Draw a barcode in the new Page.
        /// code39.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code39.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode39Barcode.
        /// Dim code39 As PdfCode39Barcode = New PdfCode39Barcode()
        /// 'Set the font to code39.
        /// code39.Font = font
        /// 'Set the barcode text.
        /// code39.Text = "CODE39"
        /// 'Draw a barcode in the new Page.
        /// code39.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code39.pdf")
        /// </code>
        /// </example> 
        public PdfCode39Barcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code39Barcode"/> class.
        /// </summary>
        public Code39Barcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode39Barcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode39Barcode.
        /// PdfCode39Barcode code39 = new PdfCode39Barcode("CODE39");
        /// //Set the font to code39.
        /// code32.Font = font;
        /// //Draw a barcode in the new Page.
        /// code39.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code39.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode39Barcode.
        /// Dim code39 As PdfCode39Barcode = New PdfCode39Barcode("CODE39")
        /// 'Set the font to code39.
        /// code39.Font = font
        /// 'Draw a barcode in the new Page.
        /// code39.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code39.pdf")
        /// </code>
        /// </example> 
        public PdfCode39Barcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code39Barcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code39Barcode(string text)
#endif
            : this()
        {
            base.Text = text;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            base.StartSymbol = '*';
            base.StopSymbol = '*';

            base.ValidatorExpression = @"^[\x41-\x5A\x30-\x39\x20\-\.\$\/\+\%\ ]+$";

            base.BarcodeSymbols['0'] = new BarcodeSymbolTable('0', 0, new byte[] { 1, 1, 1, 3, 3, 1, 3, 1, 1 });
            base.BarcodeSymbols['1'] = new BarcodeSymbolTable('1', 1, new byte[] { 3, 1, 1, 3, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['2'] = new BarcodeSymbolTable('2', 2, new byte[] { 1, 1, 3, 3, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['3'] = new BarcodeSymbolTable('3', 3, new byte[] { 3, 1, 3, 3, 1, 1, 1, 1, 1 });
            base.BarcodeSymbols['4'] = new BarcodeSymbolTable('4', 4, new byte[] { 1, 1, 1, 3, 3, 1, 1, 1, 3 });
            base.BarcodeSymbols['5'] = new BarcodeSymbolTable('5', 5, new byte[] { 3, 1, 1, 3, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['6'] = new BarcodeSymbolTable('6', 6, new byte[] { 1, 1, 3, 3, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['7'] = new BarcodeSymbolTable('7', 7, new byte[] { 1, 1, 1, 3, 1, 1, 3, 1, 3 });
            base.BarcodeSymbols['8'] = new BarcodeSymbolTable('8', 8, new byte[] { 3, 1, 1, 3, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['9'] = new BarcodeSymbolTable('9', 9, new byte[] { 1, 1, 3, 3, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['A'] = new BarcodeSymbolTable('A', 10, new byte[] { 3, 1, 1, 1, 1, 3, 1, 1, 3 });
            base.BarcodeSymbols['B'] = new BarcodeSymbolTable('B', 11, new byte[] { 1, 1, 3, 1, 1, 3, 1, 1, 3 });
            base.BarcodeSymbols['C'] = new BarcodeSymbolTable('C', 12, new byte[] { 3, 1, 3, 1, 1, 3, 1, 1, 1 });
            base.BarcodeSymbols['D'] = new BarcodeSymbolTable('D', 13, new byte[] { 1, 1, 1, 1, 3, 3, 1, 1, 3 });
            base.BarcodeSymbols['E'] = new BarcodeSymbolTable('E', 14, new byte[] { 3, 1, 1, 1, 3, 3, 1, 1, 1 });
            base.BarcodeSymbols['F'] = new BarcodeSymbolTable('F', 15, new byte[] { 1, 1, 3, 1, 3, 3, 1, 1, 1 });
            base.BarcodeSymbols['G'] = new BarcodeSymbolTable('G', 16, new byte[] { 1, 1, 1, 1, 1, 3, 3, 1, 3 });
            base.BarcodeSymbols['H'] = new BarcodeSymbolTable('H', 17, new byte[] { 3, 1, 1, 1, 1, 3, 3, 1, 1 });
            base.BarcodeSymbols['I'] = new BarcodeSymbolTable('I', 18, new byte[] { 1, 1, 3, 1, 1, 3, 3, 1, 1 });
            base.BarcodeSymbols['J'] = new BarcodeSymbolTable('J', 19, new byte[] { 1, 1, 1, 1, 3, 3, 3, 1, 1 });
            base.BarcodeSymbols['K'] = new BarcodeSymbolTable('K', 20, new byte[] { 3, 1, 1, 1, 1, 1, 1, 3, 3 });
            base.BarcodeSymbols['L'] = new BarcodeSymbolTable('L', 21, new byte[] { 1, 1, 3, 1, 1, 1, 1, 3, 3 });
            base.BarcodeSymbols['M'] = new BarcodeSymbolTable('M', 22, new byte[] { 3, 1, 3, 1, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['N'] = new BarcodeSymbolTable('N', 23, new byte[] { 1, 1, 1, 1, 3, 1, 1, 3, 3 });
            base.BarcodeSymbols['O'] = new BarcodeSymbolTable('O', 24, new byte[] { 3, 1, 1, 1, 3, 1, 1, 3, 1 });
            base.BarcodeSymbols['P'] = new BarcodeSymbolTable('P', 25, new byte[] { 1, 1, 3, 1, 3, 1, 1, 3, 1 });
            base.BarcodeSymbols['Q'] = new BarcodeSymbolTable('Q', 26, new byte[] { 1, 1, 1, 1, 1, 1, 3, 3, 3 });
            base.BarcodeSymbols['R'] = new BarcodeSymbolTable('R', 27, new byte[] { 3, 1, 1, 1, 1, 1, 3, 3, 1 });
            base.BarcodeSymbols['S'] = new BarcodeSymbolTable('S', 28, new byte[] { 1, 1, 3, 1, 1, 1, 3, 3, 1 });
            base.BarcodeSymbols['T'] = new BarcodeSymbolTable('T', 29, new byte[] { 1, 1, 1, 1, 3, 1, 3, 3, 1 });
            base.BarcodeSymbols['U'] = new BarcodeSymbolTable('U', 30, new byte[] { 3, 3, 1, 1, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['V'] = new BarcodeSymbolTable('V', 31, new byte[] { 1, 3, 3, 1, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['W'] = new BarcodeSymbolTable('W', 32, new byte[] { 3, 3, 3, 1, 1, 1, 1, 1, 1 });
            base.BarcodeSymbols['X'] = new BarcodeSymbolTable('X', 33, new byte[] { 1, 3, 1, 1, 3, 1, 1, 1, 3 });
            base.BarcodeSymbols['Y'] = new BarcodeSymbolTable('Y', 34, new byte[] { 3, 3, 1, 1, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['Z'] = new BarcodeSymbolTable('Z', 35, new byte[] { 1, 3, 3, 1, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['-'] = new BarcodeSymbolTable('-', 36, new byte[] { 1, 3, 1, 1, 1, 1, 3, 1, 3 });
            base.BarcodeSymbols['.'] = new BarcodeSymbolTable('.', 37, new byte[] { 3, 3, 1, 1, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols[' '] = new BarcodeSymbolTable(' ', 38, new byte[] { 1, 3, 3, 1, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['$'] = new BarcodeSymbolTable('$', 39, new byte[] { 1, 3, 1, 3, 1, 3, 1, 1, 1 });
            base.BarcodeSymbols['/'] = new BarcodeSymbolTable('/', 40, new byte[] { 1, 3, 1, 3, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['+'] = new BarcodeSymbolTable('+', 41, new byte[] { 1, 3, 1, 1, 1, 3, 1, 3, 1 });
            base.BarcodeSymbols['%'] = new BarcodeSymbolTable('%', 42, new byte[] { 1, 1, 1, 3, 1, 3, 1, 3, 1 });
            base.BarcodeSymbols['*'] = new BarcodeSymbolTable('*', 0, new byte[] { 1, 3, 1, 1, 3, 1, 3, 1, 1 });
        }

        /// <summary>
        /// Internal method to calculate the check-digit
        /// </summary>
        /// <returns></returns>
        protected internal override char[] CalculateCheckDigit()
        {
            if (!base.EnableCheckDigit)
            {
                return null;
            }

            int checkValue = 0;
            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text : base.ExtendedText;
            foreach (char c in code)
            {
                BarcodeSymbolTable pattern = base.BarcodeSymbols[c] as BarcodeSymbolTable;
                checkValue += pattern.CheckDigit;
            }
            // char * should be eliminated while calculating the checksum
            checkValue = checkValue % (base.BarcodeSymbols.Count - 1);
            char[] ch = new char[1];
            ch[0] = GetSymbol(checkValue);
            return ch;
        }

        /// <summary>
        /// Internal method which retrieves the specified symbol from the symbol table.
        /// </summary>
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

        #endregion
    }
}
#endif