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
using System.Text.RegularExpressions;

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
    /// Represents a Code128C barcode.
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
    /// //Creates a new PdfCode128CBarcode.
    /// PdfCode128CBarcode code128C = new PdfCode128CBarcode();
    /// //Set the font to code128C.
    /// code128C.Font = font;
    /// //Set the barcode text.
    /// code128C.Text = "Code128C";
    /// //Draw a barcode in the new Page.
    /// code128C.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code128C.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode128CBarcode.
    /// Dim code128C As PdfCode128CBarcode = New PdfCode128CBarcode()
    /// 'Set the font to code128C.
    /// code128C.Font = font
    /// 'Set the barcode text.
    /// code128C.Text = "Code128C"
    /// 'Draw a barcode in the new Page.
    /// code128C.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code128C.pdf")
    /// </code>
    /// </example>
    /// <remarks>Only the following symbols are allowed in a Code 128C barcode: 0 1 2 3 4 5 6 7 8 9 FNC1 (\xF0). Code 128 C encodes only numeric symbols at double density, each pair of digits is encoded using a single symbol.</remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode128CBarcode : PdfUnidimensionalBarcode
#else
    public class Code128CBarcode : UnidimensionalBarcode
#endif
    {
        #region Constructors
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode128CBarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode128CBarcode.
        /// PdfCode128CBarcode code128C = new PdfCode128CBarcode();
        /// //Set the font to code128C.
        /// code128C.Font = font;
        /// //Set the barcode text.
        /// code128C.Text = "Code128C";
        /// //Draw a barcode in the new Page.
        /// code128C.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code128C.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode128CBarcode.
        /// Dim code128C As PdfCode128CBarcode = New PdfCode128CBarcode()
        /// 'Set the font to code128C.
        /// code128C.Font = font
        /// 'Set the barcode text.
        /// code128C.Text = "Code128C"
        /// 'Draw a barcode in the new Page.
        /// code128C.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code128C.pdf")
        /// </code>
        /// </example>
        public PdfCode128CBarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code128CBarcode"/> class.
        /// </summary>
        public Code128CBarcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode128CBarcode"/> class.
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
        /// //Creates a new PdfCode128CBarcode.
        /// PdfCode128CBarcode code128C = new PdfCode128CBarcode("Code128C");
        /// //Set the font to code128C.
        /// code128C.Font = font;
        /// //Draw a barcode in the new Page.
        /// code128C.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code128C.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode128CBarcode.
        /// Dim code128C As PdfCode128CBarcode = New PdfCode128CBarcode("Code128C")
        /// 'Set the font to code128C.
        /// code128C.Font = font
        /// 'Draw a barcode in the new Page.
        /// code128C.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code128C.pdf")
        /// </code>
        /// </example>
        public PdfCode128CBarcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code128CBarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code128CBarcode(string text)
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
            string str = null;
            if (!base.EnableCheckDigit)
            {
                return null;
            }

            int checkValue = 0;
            string dataForChecksum = base.Text;
            if ((dataForChecksum.Length % 2) == 1)
            {
                dataForChecksum = "0" + dataForChecksum;
            }

            for (int i = 0; i < dataForChecksum.Length; i += 2)
            {
                int num3 = int.Parse(dataForChecksum.Substring(i, 2));
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in this.BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == (char)num3)
                    {
                        if (pattern != null)
                        {
                            checkValue += (pattern.CheckDigit * ((i / 2) + 1));
                        }

                        str = str + pattern.Symbol;
                        break;
                    }
                }
            }

            checkValue += 105;
            checkValue = checkValue % 0x67;
            char symbolByValue = this.GetSymbol(checkValue);
            base.Text = str;
            return new char[] { symbolByValue };
        }

        /// <summary>
        /// Internal method used to validate the given barcode text.
        /// </summary>
        /// <param name="data">The Text.</param>
        /// <returns>True if valid, Otherwise False.</returns>
        protected internal override bool Validate(string data)
        {
            Regex m_codeValidator;
            m_codeValidator = new Regex(base.ValidatorExpression
#if!BARCODE_WINRT && !NETFX_CORE && !BARCODE_SILVERLIGHT && !WP
                , RegexOptions.Compiled);
#else
);
#endif
            MatchCollection match = m_codeValidator.Matches(data);
            if (match.Count == data.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the Actual text to encode.
        /// </summary>
        /// <returns>The Actual Text.</returns>
        protected override string GetTextToEncode()
        {
            string temp = base.Text;
            if (!Validate(base.Text))
            {
#if!XAML && !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException("Barcode text contains characters that are not accepted by this barcode specification.");
#else
                throw new BarcodeException("Barcode text contains characters that are not accepted by this barcode specification.");
#endif
            }

            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text.Trim('*') : base.ExtendedText.Trim('*');

            if (isCheckDigitAdded || !EnableCheckDigit)
            {
                return code;
            }

            char[] checkDigit = CalculateCheckDigit();
            //Note : Some barcode symbologies does not have check digits.
            if (checkDigit == null || checkDigit.Length == 0)
            {
                return code;
            }
            //Changed from Revision 89384
            if (EnableCheckDigit /* && checkDigit[checkDigit.Length - 1] != '\0' */ && !isCheckDigitAdded)
            {
                foreach (char c in checkDigit)
                {
                    code += c.ToString();
                }
            }

            //Changed from Revision 89384
            if (ShowCheckDigit /* && checkDigit[checkDigit.Length - 1] != '\0' */ && !isCheckDigitAdded)
            {
                if (code[code.Length - 1] != checkDigit[checkDigit.Length - 1])
                {
                    foreach (char c in checkDigit)
                    {
                        code += c.ToString();
                    }
                }

                isCheckDigitAdded = true;

                if (base.ExtendedText.Equals(string.Empty))
                {
                    code = base.Text;
                    foreach (char c in checkDigit)
                    {
                        code += c.ToString();
                    }
                }
            }

            base.Text = temp;
            if (ShowCheckDigit)
            {
                foreach (char c in checkDigit)
                {
                    base.Text += c.ToString();
                }
            }

            isCheckDigitAdded = true;

            return code;
        }

        /// <summary>
        /// Gets the data to encode.
        /// </summary>
        /// <param name="originalData">The original data.</param>
        /// <returns>Encoded string.</returns>
        protected string GetDataToEncode(string originalData)
        {
            StringBuilder builder = new StringBuilder();
            string str = originalData;
            if ((originalData.Length % 2) == 1)
            {
                str = "0" + str;
            }

            for (int i = 0; i < str.Length; i += 2)
            {
                char ch = (char)int.Parse(str.Substring(i, 2));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Initializes the internal barcode symbol table
        /// </summary>
        private void Initialize()
        {
            base.StartSymbol = '\x00fe';
            base.StopSymbol = '\x00ff';

            base.ValidatorExpression = "[0-9]";

            base.BarcodeSymbols['\0'] = new BarcodeSymbolTable('\0', 0, new byte[] { 2, 1, 2, 2, 2, 2 });
            base.BarcodeSymbols['\x0001'] = new BarcodeSymbolTable('\x0001', 1, new byte[] { 2, 2, 2, 1, 2, 2 });
            base.BarcodeSymbols['\x0002'] = new BarcodeSymbolTable('\x0002', 2, new byte[] { 2, 2, 2, 2, 2, 1 });
            base.BarcodeSymbols['\x0003'] = new BarcodeSymbolTable('\x0003', 3, new byte[] { 1, 2, 1, 2, 2, 3 });
            base.BarcodeSymbols['\x0004'] = new BarcodeSymbolTable('\x0004', 4, new byte[] { 1, 2, 1, 3, 2, 2 });
            base.BarcodeSymbols['\x0005'] = new BarcodeSymbolTable('\x0005', 5, new byte[] { 1, 3, 1, 2, 2, 2 });
            base.BarcodeSymbols['\x0006'] = new BarcodeSymbolTable('\x0006', 6, new byte[] { 1, 2, 2, 2, 1, 3 });
            base.BarcodeSymbols['\a'] = new BarcodeSymbolTable('\a', 7, new byte[] { 1, 2, 2, 3, 1, 2 });
            base.BarcodeSymbols['\b'] = new BarcodeSymbolTable('\b', 8, new byte[] { 1, 3, 2, 2, 1, 2 });
            base.BarcodeSymbols['\t'] = new BarcodeSymbolTable('\t', 9, new byte[] { 2, 2, 1, 2, 1, 3 });
            base.BarcodeSymbols['\n'] = new BarcodeSymbolTable('\n', 10, new byte[] { 2, 2, 1, 3, 1, 2 });
            base.BarcodeSymbols['\v'] = new BarcodeSymbolTable('\v', 11, new byte[] { 2, 3, 1, 2, 1, 2 });
            base.BarcodeSymbols['\f'] = new BarcodeSymbolTable('\f', 12, new byte[] { 1, 1, 2, 2, 3, 2 });
            base.BarcodeSymbols['\r'] = new BarcodeSymbolTable('\r', 13, new byte[] { 1, 2, 2, 1, 3, 2 });
            base.BarcodeSymbols['\x000e'] = new BarcodeSymbolTable('\x000e', 14, new byte[] { 1, 2, 2, 2, 3, 1 });
            base.BarcodeSymbols['\x000f'] = new BarcodeSymbolTable('\x000f', 15, new byte[] { 1, 1, 3, 2, 2, 2 });
            base.BarcodeSymbols['\x0010'] = new BarcodeSymbolTable('\x0010', 0x10, new byte[] { 1, 2, 3, 1, 2, 2 });
            base.BarcodeSymbols['\x0011'] = new BarcodeSymbolTable('\x0011', 0x11, new byte[] { 1, 2, 3, 2, 2, 1 });
            base.BarcodeSymbols['\x0012'] = new BarcodeSymbolTable('\x0012', 0x12, new byte[] { 2, 2, 3, 2, 1, 1 });
            base.BarcodeSymbols['\x0013'] = new BarcodeSymbolTable('\x0013', 0x13, new byte[] { 2, 2, 1, 1, 3, 2 });
            base.BarcodeSymbols['\x0014'] = new BarcodeSymbolTable('\x0014', 20, new byte[] { 2, 2, 1, 2, 3, 1 });
            base.BarcodeSymbols['\x0015'] = new BarcodeSymbolTable('\x0015', 0x15, new byte[] { 2, 1, 3, 2, 1, 2 });
            base.BarcodeSymbols['\x0016'] = new BarcodeSymbolTable('\x0016', 0x16, new byte[] { 2, 2, 3, 1, 1, 2 });
            base.BarcodeSymbols['\x0017'] = new BarcodeSymbolTable('\x0017', 0x17, new byte[] { 3, 1, 2, 1, 3, 1 });
            base.BarcodeSymbols['\x0018'] = new BarcodeSymbolTable('\x0018', 0x18, new byte[] { 3, 1, 1, 2, 2, 2 });
            base.BarcodeSymbols['\x0019'] = new BarcodeSymbolTable('\x0019', 0x19, new byte[] { 3, 2, 1, 1, 2, 2 });
            base.BarcodeSymbols['\x001a'] = new BarcodeSymbolTable('\x001a', 0x1a, new byte[] { 3, 2, 1, 2, 2, 1 });
            base.BarcodeSymbols['\x001b'] = new BarcodeSymbolTable('\x001b', 0x1b, new byte[] { 3, 1, 2, 2, 1, 2 });
            base.BarcodeSymbols['\x001c'] = new BarcodeSymbolTable('\x001c', 0x1c, new byte[] { 3, 2, 2, 1, 1, 2 });
            base.BarcodeSymbols['\x001d'] = new BarcodeSymbolTable('\x001d', 0x1d, new byte[] { 3, 2, 2, 2, 1, 1 });
            base.BarcodeSymbols['\x001e'] = new BarcodeSymbolTable('\x001e', 30, new byte[] { 2, 1, 2, 1, 2, 3 });
            base.BarcodeSymbols['\x001f'] = new BarcodeSymbolTable('\x001f', 0x1f, new byte[] { 2, 1, 2, 3, 2, 1 });
            base.BarcodeSymbols[' '] = new BarcodeSymbolTable(' ', 0x20, new byte[] { 2, 3, 2, 1, 2, 1 });
            base.BarcodeSymbols['!'] = new BarcodeSymbolTable('!', 0x21, new byte[] { 1, 1, 1, 3, 2, 3 });
            base.BarcodeSymbols['"'] = new BarcodeSymbolTable('"', 0x22, new byte[] { 1, 3, 1, 1, 2, 3 });
            base.BarcodeSymbols['#'] = new BarcodeSymbolTable('#', 0x23, new byte[] { 1, 3, 1, 3, 2, 1 });
            base.BarcodeSymbols['$'] = new BarcodeSymbolTable('$', 0x24, new byte[] { 1, 1, 2, 3, 1, 3 });
            base.BarcodeSymbols['%'] = new BarcodeSymbolTable('%', 0x25, new byte[] { 1, 3, 2, 1, 1, 3 });
            base.BarcodeSymbols['&'] = new BarcodeSymbolTable('&', 0x26, new byte[] { 1, 3, 2, 3, 1, 1 });
            base.BarcodeSymbols['\''] = new BarcodeSymbolTable('\'', 0x27, new byte[] { 2, 1, 1, 3, 1, 3 });
            base.BarcodeSymbols['('] = new BarcodeSymbolTable('(', 40, new byte[] { 2, 3, 1, 1, 1, 3 });
            base.BarcodeSymbols[')'] = new BarcodeSymbolTable(')', 0x29, new byte[] { 2, 3, 1, 3, 1, 1 });
            base.BarcodeSymbols['*'] = new BarcodeSymbolTable('*', 0x2a, new byte[] { 1, 1, 2, 1, 3, 3 });
            base.BarcodeSymbols['+'] = new BarcodeSymbolTable('+', 0x2b, new byte[] { 1, 1, 2, 3, 3, 1 });
            base.BarcodeSymbols[','] = new BarcodeSymbolTable(',', 0x2c, new byte[] { 1, 3, 2, 1, 3, 1 });
            base.BarcodeSymbols['-'] = new BarcodeSymbolTable('-', 0x2d, new byte[] { 1, 1, 3, 1, 2, 3 });
            base.BarcodeSymbols['.'] = new BarcodeSymbolTable('.', 0x2e, new byte[] { 1, 1, 3, 3, 2, 1 });
            base.BarcodeSymbols['/'] = new BarcodeSymbolTable('/', 0x2f, new byte[] { 1, 3, 3, 1, 2, 1 });
            base.BarcodeSymbols['0'] = new BarcodeSymbolTable('0', 0x30, new byte[] { 3, 1, 3, 1, 2, 1 });
            base.BarcodeSymbols['1'] = new BarcodeSymbolTable('1', 0x31, new byte[] { 2, 1, 1, 3, 3, 1 });
            base.BarcodeSymbols['2'] = new BarcodeSymbolTable('2', 50, new byte[] { 2, 3, 1, 1, 3, 1 });
            base.BarcodeSymbols['3'] = new BarcodeSymbolTable('3', 0x33, new byte[] { 2, 1, 3, 1, 1, 3 });
            base.BarcodeSymbols['4'] = new BarcodeSymbolTable('4', 0x34, new byte[] { 2, 1, 3, 3, 1, 1 });
            base.BarcodeSymbols['5'] = new BarcodeSymbolTable('5', 0x35, new byte[] { 2, 1, 3, 1, 3, 1 });
            base.BarcodeSymbols['6'] = new BarcodeSymbolTable('6', 0x36, new byte[] { 3, 1, 1, 1, 2, 3 });
            base.BarcodeSymbols['7'] = new BarcodeSymbolTable('7', 0x37, new byte[] { 3, 1, 1, 3, 2, 1 });
            base.BarcodeSymbols['8'] = new BarcodeSymbolTable('8', 0x38, new byte[] { 3, 3, 1, 1, 2, 1 });
            base.BarcodeSymbols['9'] = new BarcodeSymbolTable('9', 0x39, new byte[] { 3, 1, 2, 1, 1, 3 });
            base.BarcodeSymbols[':'] = new BarcodeSymbolTable(':', 0x3a, new byte[] { 3, 1, 2, 3, 1, 1 });
            base.BarcodeSymbols[';'] = new BarcodeSymbolTable(';', 0x3b, new byte[] { 3, 3, 2, 1, 1, 1 });
            base.BarcodeSymbols['<'] = new BarcodeSymbolTable('<', 60, new byte[] { 3, 1, 4, 1, 1, 1 });
            base.BarcodeSymbols['='] = new BarcodeSymbolTable('=', 0x3d, new byte[] { 2, 2, 1, 4, 1, 1 });
            base.BarcodeSymbols['>'] = new BarcodeSymbolTable('>', 0x3e, new byte[] { 4, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['?'] = new BarcodeSymbolTable('?', 0x3f, new byte[] { 1, 1, 1, 2, 2, 4 });
            base.BarcodeSymbols['@'] = new BarcodeSymbolTable('@', 0x40, new byte[] { 1, 1, 1, 4, 2, 2 });
            base.BarcodeSymbols['A'] = new BarcodeSymbolTable('A', 0x41, new byte[] { 1, 2, 1, 1, 2, 4 });
            base.BarcodeSymbols['B'] = new BarcodeSymbolTable('B', 0x42, new byte[] { 1, 2, 1, 4, 2, 1 });
            base.BarcodeSymbols['C'] = new BarcodeSymbolTable('C', 0x43, new byte[] { 1, 4, 1, 1, 2, 2 });
            base.BarcodeSymbols['D'] = new BarcodeSymbolTable('D', 0x44, new byte[] { 1, 4, 1, 2, 2, 1 });
            base.BarcodeSymbols['E'] = new BarcodeSymbolTable('E', 0x45, new byte[] { 1, 1, 2, 2, 1, 4 });
            base.BarcodeSymbols['F'] = new BarcodeSymbolTable('F', 70, new byte[] { 1, 1, 2, 4, 1, 2 });
            base.BarcodeSymbols['G'] = new BarcodeSymbolTable('G', 0x47, new byte[] { 1, 2, 2, 1, 1, 4 });
            base.BarcodeSymbols['H'] = new BarcodeSymbolTable('H', 0x48, new byte[] { 1, 2, 2, 4, 1, 1 });
            base.BarcodeSymbols['I'] = new BarcodeSymbolTable('I', 0x49, new byte[] { 1, 4, 2, 1, 1, 2 });
            base.BarcodeSymbols['J'] = new BarcodeSymbolTable('J', 0x4a, new byte[] { 1, 4, 2, 2, 1, 1 });
            base.BarcodeSymbols['K'] = new BarcodeSymbolTable('K', 0x4b, new byte[] { 2, 4, 1, 2, 1, 1 });
            base.BarcodeSymbols['L'] = new BarcodeSymbolTable('L', 0x4c, new byte[] { 2, 2, 1, 1, 1, 4 });
            base.BarcodeSymbols['M'] = new BarcodeSymbolTable('M', 0x4d, new byte[] { 4, 1, 3, 1, 1, 1 });
            base.BarcodeSymbols['N'] = new BarcodeSymbolTable('N', 0x4e, new byte[] { 2, 4, 1, 1, 1, 2 });
            base.BarcodeSymbols['O'] = new BarcodeSymbolTable('O', 0x4f, new byte[] { 1, 3, 4, 1, 1, 1 });
            base.BarcodeSymbols['P'] = new BarcodeSymbolTable('P', 80, new byte[] { 1, 1, 1, 2, 4, 2 });
            base.BarcodeSymbols['Q'] = new BarcodeSymbolTable('Q', 0x51, new byte[] { 1, 2, 1, 1, 4, 2 });
            base.BarcodeSymbols['R'] = new BarcodeSymbolTable('R', 0x52, new byte[] { 1, 2, 1, 2, 4, 1 });
            base.BarcodeSymbols['S'] = new BarcodeSymbolTable('S', 0x53, new byte[] { 1, 1, 4, 2, 1, 2 });
            base.BarcodeSymbols['T'] = new BarcodeSymbolTable('T', 0x54, new byte[] { 1, 2, 4, 1, 1, 2 });
            base.BarcodeSymbols['U'] = new BarcodeSymbolTable('U', 0x55, new byte[] { 1, 2, 4, 2, 1, 1 });
            base.BarcodeSymbols['V'] = new BarcodeSymbolTable('V', 0x56, new byte[] { 4, 1, 1, 2, 1, 2 });
            base.BarcodeSymbols['W'] = new BarcodeSymbolTable('W', 0x57, new byte[] { 4, 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['X'] = new BarcodeSymbolTable('X', 0x58, new byte[] { 4, 2, 1, 2, 1, 1 });
            base.BarcodeSymbols['Y'] = new BarcodeSymbolTable('Y', 0x59, new byte[] { 2, 1, 2, 1, 4, 1 });
            base.BarcodeSymbols['Z'] = new BarcodeSymbolTable('Z', 90, new byte[] { 2, 1, 4, 1, 2, 1 });
            base.BarcodeSymbols['['] = new BarcodeSymbolTable('[', 0x5b, new byte[] { 4, 1, 2, 1, 2, 1 });
            base.BarcodeSymbols['\\'] = new BarcodeSymbolTable('\\', 0x5c, new byte[] { 1, 1, 1, 1, 4, 3 });
            base.BarcodeSymbols[']'] = new BarcodeSymbolTable(']', 0x5d, new byte[] { 1, 1, 1, 3, 4, 1 });
            base.BarcodeSymbols['^'] = new BarcodeSymbolTable('^', 0x5e, new byte[] { 1, 3, 1, 1, 4, 1 });
            base.BarcodeSymbols['_'] = new BarcodeSymbolTable('_', 0x5f, new byte[] { 1, 1, 4, 1, 1, 3 });
            base.BarcodeSymbols['`'] = new BarcodeSymbolTable('`', 0x60, new byte[] { 1, 1, 4, 3, 1, 1 });
            base.BarcodeSymbols['a'] = new BarcodeSymbolTable('a', 0x61, new byte[] { 4, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['b'] = new BarcodeSymbolTable('b', 0x62, new byte[] { 4, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['c'] = new BarcodeSymbolTable('c', 0x63, new byte[] { 1, 1, 3, 1, 4, 1 });
            base.BarcodeSymbols['\x00f0'] = new BarcodeSymbolTable('\x00f0', 0x66, new byte[] { 4, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['\x00fa'] = new BarcodeSymbolTable('\x00fa', 0x65, new byte[] { 3, 1, 1, 1, 4, 1 });
            base.BarcodeSymbols['\x00fb'] = new BarcodeSymbolTable('\x00fb', 100, new byte[] { 1, 1, 4, 1, 3, 1 });
            base.BarcodeSymbols['\x00fe'] = new BarcodeSymbolTable('\x00fe', 0x69, new byte[] { 2, 1, 1, 2, 3, 2 });
            base.BarcodeSymbols['\x00ff'] = new BarcodeSymbolTable('\x00ff', -1, new byte[] { 2, 3, 3, 1, 1, 1, 2 });
        }

        /// <summary>
        /// Internal method used for reading a symbol from barcode symbol table.
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