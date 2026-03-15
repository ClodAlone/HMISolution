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
    /// Represents a Code128A barcode.
    /// </summary>
#if !XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Creates a new PdfCode128ABarcode.
    /// PdfCode128ABarcode code128A = new PdfCode128ABarcode();
    /// //Set the font to code128A.
    /// code128A.Font = font;
    /// //Set the barcode text.
    /// code128A.Text = "CODE128A";
    /// //Draw a barcode in the new Page.
    /// code128A.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code128A.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode128ABarcode.
    /// Dim code32 As PdfCode128ABarcode = New PdfCode128ABarcode()
    /// 'Set the font to code128A.
    /// code128A.Font = font
    /// 'Set the barcode text.
    /// code128A.Text = "Code128A"
    /// 'Draw a barcode in the new Page.
    /// code128A.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code128A.pdf")
    /// </code>
    /// </example>
    /// <remarks> Only the following symbols are allowed in a Code 128 A barcode: NUL (\x00) SOH (\x01) STX (\x02) ETX (\x03) EOT (\x04) ENQ (\x05) ACK (\x06) BEL (\x07) BS (\x08) HT (\x09) LF (\x0A) VT (\x0B) FF (\x0C) CR (\x0D) SO (\x0E) SI (\x0F) DLE (\x10) DC1 (\x11) DC2 (\x12) DC3 (\x13) DC4 (\x14) NAK (\x15) SYN (\x16) ETB (\x17) CAN (\x18) EM (\x19) SUB (\x1A) ESC (\x1B) FS (\x1C) GS (\x1D) RS (\x1E) US (\x1F) SPACE !  # $ % ' * + , - . 0 1 2 3 4 5 6 7 8 9 : ; ? @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ \ ]^ _ FNC1 (\xF0) FNC2 (\xF1) FNC3 (\xF2) FNC4  </remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode128ABarcode : PdfUnidimensionalBarcode
#else
    public class Code128ABarcode : UnidimensionalBarcode
#endif
    {
        #region Constructors
#if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode128ABarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode128ABarcode.
        /// PdfCode128ABarcode code128A = new PdfCode128ABarcode();
        /// //Set the font to code128A.
        /// code128A.Font = font;
        /// //Set the barcode text.
        /// code128A.Text = "CODE128A";
        /// //Draw a barcode in the new Page.
        /// code128A.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code128A.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode128ABarcode.
        /// Dim code32 As PdfCode128ABarcode = New PdfCode128ABarcode()
        /// 'Set the font to code128A.
        /// code128A.Font = font
        /// 'Set the barcode text.
        /// code128A.Text = "Code128A"
        /// 'Draw a barcode in the new Page.
        /// code128A.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code128A.pdf")
        /// </code>
        /// </example>
        public PdfCode128ABarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code128ABarcode"/> class.
        /// </summary>
        public Code128ABarcode()
#endif
            : base()
        {
            Initialize();
        }

#if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode128ABarcode"/> class.
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
        /// //Creates a new PdfCode128ABarcode.
        /// PdfCode128ABarcode code128A = new PdfCode128ABarcode("Code128A");
        /// //Set the font to code128A.
        /// code128A.Font = font;
        /// //Draw a barcode in the new Page.
        /// code128A.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code128A.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode128ABarcode.
        /// Dim code32 As PdfCode128ABarcode = New PdfCode128ABarcode("Code128A")
        /// 'Set the font to code128A.
        /// code128A.Font = font
        /// 'Draw a barcode in the new Page.
        /// code128A.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code128A.pdf")
        /// </code>
        /// </example>
        public PdfCode128ABarcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code128ABarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode Text.</param>
        public Code128ABarcode(string text)
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
            if (!base.EnableCheckDigit)
            {
                return null;
            }

            int checkValue = 0;
            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text : base.ExtendedText;
            int i = 0;
            foreach (char c in code)
            {
                BarcodeSymbolTable pattern = base.BarcodeSymbols[c] as BarcodeSymbolTable;
                if (pattern == null)
                {
#if !XAML && !NETFX_CORE && !GDI && !WP
                    throw new PdfBarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
#else
                    throw new BarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
#endif
                }

                checkValue += (pattern.CheckDigit * (i + 1));
                i++;
            }
            // char * should be eliminated while calculating the checksum
            checkValue += 103;
            checkValue = checkValue % 0x67;
            char[] ch = new char[1];
            ch[0] = GetSymbol(checkValue);
            return ch;
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
        /// Initializes the internal barcode symbol table
        /// </summary>
        protected void Initialize()
        {
            base.StartSymbol = '\x00f9';
            base.StopSymbol = '\x00ff';
            base.ValidatorExpression = "[\0\x0001\x0002\x0003\x0004\x0005\x0006\a\b\t\n\v\f\r\x000e\x000f\x0010\x0011\x0012\x0013\x0014\x0015\x0016\x0017\x0018\x0019\x001a\x001b\x001c\x001d\x001e\x001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_\x00f0\x00f1\x00f2\x00f3\x00f4]";

            base.BarcodeSymbols['\0'] = new BarcodeSymbolTable('\0', 0x40, new byte[] { 1, 1, 1, 4, 2, 2 });
            base.BarcodeSymbols['\x0001'] = new BarcodeSymbolTable('\x0001', 0x41, new byte[] { 1, 2, 1, 1, 2, 4 });
            base.BarcodeSymbols['\x0002'] = new BarcodeSymbolTable('\x0002', 0x42, new byte[] { 1, 2, 1, 4, 2, 1 });
            base.BarcodeSymbols['\x0003'] = new BarcodeSymbolTable('\x0003', 0x43, new byte[] { 1, 4, 1, 1, 2, 2 });
            base.BarcodeSymbols['\x0004'] = new BarcodeSymbolTable('\x0004', 0x44, new byte[] { 1, 4, 1, 2, 2, 1 });
            base.BarcodeSymbols['\x0005'] = new BarcodeSymbolTable('\x0005', 0x45, new byte[] { 1, 1, 2, 2, 1, 4 });
            base.BarcodeSymbols['\x0006'] = new BarcodeSymbolTable('\x0006', 70, new byte[] { 1, 1, 2, 4, 1, 2 });
            base.BarcodeSymbols['\a'] = new BarcodeSymbolTable('\a', 0x47, new byte[] { 1, 2, 2, 1, 1, 4 });
            base.BarcodeSymbols['\b'] = new BarcodeSymbolTable('\b', 0x48, new byte[] { 1, 2, 2, 4, 1, 1 });
            base.BarcodeSymbols['\t'] = new BarcodeSymbolTable('\t', 0x49, new byte[] { 1, 4, 2, 1, 1, 2 });
            base.BarcodeSymbols['\n'] = new BarcodeSymbolTable('\n', 0x4a, new byte[] { 1, 4, 2, 2, 1, 1 });
            base.BarcodeSymbols['\v'] = new BarcodeSymbolTable('\v', 0x4b, new byte[] { 2, 4, 1, 2, 1, 1 });
            base.BarcodeSymbols['\f'] = new BarcodeSymbolTable('\f', 0x4c, new byte[] { 2, 2, 1, 1, 1, 4 });
            base.BarcodeSymbols['\r'] = new BarcodeSymbolTable('\r', 0x4d, new byte[] { 4, 1, 3, 1, 1, 1 });
            base.BarcodeSymbols['\x000e'] = new BarcodeSymbolTable('\x000e', 0x4e, new byte[] { 2, 4, 1, 1, 1, 2 });
            base.BarcodeSymbols['\x000f'] = new BarcodeSymbolTable('\x000f', 0x4f, new byte[] { 1, 3, 4, 1, 1, 1 });
            base.BarcodeSymbols['\x0010'] = new BarcodeSymbolTable('\x0010', 80, new byte[] { 1, 1, 1, 2, 4, 2 });
            base.BarcodeSymbols['\x0011'] = new BarcodeSymbolTable('\x0011', 0x51, new byte[] { 1, 2, 1, 1, 4, 2 });
            base.BarcodeSymbols['\x0012'] = new BarcodeSymbolTable('\x0012', 0x52, new byte[] { 1, 2, 1, 2, 4, 1 });
            base.BarcodeSymbols['\x0013'] = new BarcodeSymbolTable('\x0013', 0x53, new byte[] { 1, 1, 4, 2, 1, 2 });
            base.BarcodeSymbols['\x0014'] = new BarcodeSymbolTable('\x0014', 0x54, new byte[] { 1, 2, 4, 1, 1, 2 });
            base.BarcodeSymbols['\x0015'] = new BarcodeSymbolTable('\x0015', 0x55, new byte[] { 1, 2, 4, 2, 1, 1 });
            base.BarcodeSymbols['\x0016'] = new BarcodeSymbolTable('\x0016', 0x56, new byte[] { 4, 1, 1, 2, 1, 2 });
            base.BarcodeSymbols['\x0017'] = new BarcodeSymbolTable('\x0017', 0x57, new byte[] { 4, 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['\x0018'] = new BarcodeSymbolTable('\x0018', 0x58, new byte[] { 4, 2, 1, 2, 1, 1 });
            base.BarcodeSymbols['\x0019'] = new BarcodeSymbolTable('\x0019', 0x59, new byte[] { 2, 1, 2, 1, 4, 1 });
            base.BarcodeSymbols['\x001a'] = new BarcodeSymbolTable('\x001a', 90, new byte[] { 2, 1, 4, 1, 2, 1 });
            base.BarcodeSymbols['\x001b'] = new BarcodeSymbolTable('\x001b', 0x5b, new byte[] { 4, 1, 2, 1, 2, 1 });
            base.BarcodeSymbols['\x001c'] = new BarcodeSymbolTable('\x001c', 0x5c, new byte[] { 1, 1, 1, 1, 4, 3 });
            base.BarcodeSymbols['\x001d'] = new BarcodeSymbolTable('\x001d', 0x5d, new byte[] { 1, 1, 1, 3, 4, 1 });
            base.BarcodeSymbols['\x001e'] = new BarcodeSymbolTable('\x001e', 0x5e, new byte[] { 1, 3, 1, 1, 4, 1 });
            base.BarcodeSymbols['\x001f'] = new BarcodeSymbolTable('\x001f', 0x5f, new byte[] { 1, 1, 4, 1, 1, 3 });
            base.BarcodeSymbols[' '] = new BarcodeSymbolTable(' ', 0, new byte[] { 2, 1, 2, 2, 2, 2 });
            base.BarcodeSymbols['!'] = new BarcodeSymbolTable('!', 1, new byte[] { 2, 2, 2, 1, 2, 2 });
            base.BarcodeSymbols['"'] = new BarcodeSymbolTable('"', 2, new byte[] { 2, 2, 2, 2, 2, 1 });
            base.BarcodeSymbols['#'] = new BarcodeSymbolTable('#', 3, new byte[] { 1, 2, 1, 2, 2, 3 });
            base.BarcodeSymbols['$'] = new BarcodeSymbolTable('$', 4, new byte[] { 1, 2, 1, 3, 2, 2 });
            base.BarcodeSymbols['%'] = new BarcodeSymbolTable('%', 5, new byte[] { 1, 3, 1, 2, 2, 2 });
            base.BarcodeSymbols['&'] = new BarcodeSymbolTable('&', 6, new byte[] { 1, 2, 2, 2, 1, 3 });
            base.BarcodeSymbols['\''] = new BarcodeSymbolTable('\'', 7, new byte[] { 1, 2, 2, 3, 1, 2 });
            base.BarcodeSymbols['('] = new BarcodeSymbolTable('(', 8, new byte[] { 1, 3, 2, 2, 1, 2 });
            base.BarcodeSymbols[')'] = new BarcodeSymbolTable(')', 9, new byte[] { 2, 2, 1, 2, 1, 3 });
            base.BarcodeSymbols['*'] = new BarcodeSymbolTable('*', 10, new byte[] { 2, 2, 1, 3, 1, 2 });
            base.BarcodeSymbols['+'] = new BarcodeSymbolTable('+', 11, new byte[] { 2, 3, 1, 2, 1, 2 });
            base.BarcodeSymbols[','] = new BarcodeSymbolTable(',', 12, new byte[] { 1, 1, 2, 2, 3, 2 });
            base.BarcodeSymbols['-'] = new BarcodeSymbolTable('-', 13, new byte[] { 1, 2, 2, 1, 3, 2 });
            base.BarcodeSymbols['.'] = new BarcodeSymbolTable('.', 14, new byte[] { 1, 2, 2, 2, 3, 1 });
            base.BarcodeSymbols['/'] = new BarcodeSymbolTable('/', 15, new byte[] { 1, 1, 3, 2, 2, 2 });
            base.BarcodeSymbols['0'] = new BarcodeSymbolTable('0', 0x10, new byte[] { 1, 2, 3, 1, 2, 2 });
            base.BarcodeSymbols['1'] = new BarcodeSymbolTable('1', 0x11, new byte[] { 1, 2, 3, 2, 2, 1 });
            base.BarcodeSymbols['2'] = new BarcodeSymbolTable('2', 0x12, new byte[] { 2, 2, 3, 2, 1, 1 });
            base.BarcodeSymbols['3'] = new BarcodeSymbolTable('3', 0x13, new byte[] { 2, 2, 1, 1, 3, 2 });
            base.BarcodeSymbols['4'] = new BarcodeSymbolTable('4', 20, new byte[] { 2, 2, 1, 2, 3, 1 });
            base.BarcodeSymbols['5'] = new BarcodeSymbolTable('5', 0x15, new byte[] { 2, 1, 3, 2, 1, 2 });
            base.BarcodeSymbols['6'] = new BarcodeSymbolTable('6', 0x16, new byte[] { 2, 2, 3, 1, 1, 2 });
            base.BarcodeSymbols['7'] = new BarcodeSymbolTable('7', 0x17, new byte[] { 3, 1, 2, 1, 3, 1 });
            base.BarcodeSymbols['8'] = new BarcodeSymbolTable('8', 0x18, new byte[] { 3, 1, 1, 2, 2, 2 });
            base.BarcodeSymbols['9'] = new BarcodeSymbolTable('9', 0x19, new byte[] { 3, 2, 1, 1, 2, 2 });
            base.BarcodeSymbols[':'] = new BarcodeSymbolTable(':', 0x1a, new byte[] { 3, 2, 1, 2, 2, 1 });
            base.BarcodeSymbols[';'] = new BarcodeSymbolTable(';', 0x1b, new byte[] { 3, 1, 2, 2, 1, 2 });
            base.BarcodeSymbols['<'] = new BarcodeSymbolTable('<', 0x1c, new byte[] { 3, 2, 2, 1, 1, 2 });
            base.BarcodeSymbols['='] = new BarcodeSymbolTable('=', 0x1d, new byte[] { 3, 2, 2, 2, 1, 1 });
            base.BarcodeSymbols['>'] = new BarcodeSymbolTable('>', 30, new byte[] { 2, 1, 2, 1, 2, 3 });
            base.BarcodeSymbols['?'] = new BarcodeSymbolTable('?', 0x1f, new byte[] { 2, 1, 2, 3, 2, 1 });
            base.BarcodeSymbols['@'] = new BarcodeSymbolTable('@', 0x20, new byte[] { 2, 3, 2, 1, 2, 1 });
            base.BarcodeSymbols['A'] = new BarcodeSymbolTable('A', 0x21, new byte[] { 1, 1, 1, 3, 2, 3 });
            base.BarcodeSymbols['B'] = new BarcodeSymbolTable('B', 0x22, new byte[] { 1, 3, 1, 1, 2, 3 });
            base.BarcodeSymbols['C'] = new BarcodeSymbolTable('C', 0x23, new byte[] { 1, 3, 1, 3, 2, 1 });
            base.BarcodeSymbols['D'] = new BarcodeSymbolTable('D', 0x24, new byte[] { 1, 1, 2, 3, 1, 3 });
            base.BarcodeSymbols['E'] = new BarcodeSymbolTable('E', 0x25, new byte[] { 1, 3, 2, 1, 1, 3 });
            base.BarcodeSymbols['F'] = new BarcodeSymbolTable('F', 0x26, new byte[] { 1, 3, 2, 3, 1, 1 });
            base.BarcodeSymbols['G'] = new BarcodeSymbolTable('G', 0x27, new byte[] { 2, 1, 1, 3, 1, 3 });
            base.BarcodeSymbols['H'] = new BarcodeSymbolTable('H', 40, new byte[] { 2, 3, 1, 1, 1, 3 });
            base.BarcodeSymbols['I'] = new BarcodeSymbolTable('I', 0x29, new byte[] { 2, 3, 1, 3, 1, 1 });
            base.BarcodeSymbols['J'] = new BarcodeSymbolTable('J', 0x2a, new byte[] { 1, 1, 2, 1, 3, 3 });
            base.BarcodeSymbols['K'] = new BarcodeSymbolTable('K', 0x2b, new byte[] { 1, 1, 2, 3, 3, 1 });
            base.BarcodeSymbols['L'] = new BarcodeSymbolTable('L', 0x2c, new byte[] { 1, 3, 2, 1, 3, 1 });
            base.BarcodeSymbols['M'] = new BarcodeSymbolTable('M', 0x2d, new byte[] { 1, 1, 3, 1, 2, 3 });
            base.BarcodeSymbols['N'] = new BarcodeSymbolTable('N', 0x2e, new byte[] { 1, 1, 3, 3, 2, 1 });
            base.BarcodeSymbols['O'] = new BarcodeSymbolTable('O', 0x2f, new byte[] { 1, 3, 3, 1, 2, 1 });
            base.BarcodeSymbols['P'] = new BarcodeSymbolTable('P', 0x30, new byte[] { 3, 1, 3, 1, 2, 1 });
            base.BarcodeSymbols['Q'] = new BarcodeSymbolTable('Q', 0x31, new byte[] { 2, 1, 1, 3, 3, 1 });
            base.BarcodeSymbols['R'] = new BarcodeSymbolTable('R', 50, new byte[] { 2, 3, 1, 1, 3, 1 });
            base.BarcodeSymbols['S'] = new BarcodeSymbolTable('S', 0x33, new byte[] { 2, 1, 3, 1, 1, 3 });
            base.BarcodeSymbols['T'] = new BarcodeSymbolTable('T', 0x34, new byte[] { 2, 1, 3, 3, 1, 1 });
            base.BarcodeSymbols['U'] = new BarcodeSymbolTable('U', 0x35, new byte[] { 2, 1, 3, 1, 3, 1 });
            base.BarcodeSymbols['V'] = new BarcodeSymbolTable('V', 0x36, new byte[] { 3, 1, 1, 1, 2, 3 });
            base.BarcodeSymbols['W'] = new BarcodeSymbolTable('W', 0x37, new byte[] { 3, 1, 1, 3, 2, 1 });
            base.BarcodeSymbols['X'] = new BarcodeSymbolTable('X', 0x38, new byte[] { 3, 3, 1, 1, 2, 1 });
            base.BarcodeSymbols['Y'] = new BarcodeSymbolTable('Y', 0x39, new byte[] { 3, 1, 2, 1, 1, 3 });
            base.BarcodeSymbols['Z'] = new BarcodeSymbolTable('Z', 0x3a, new byte[] { 3, 1, 2, 3, 1, 1 });
            base.BarcodeSymbols['['] = new BarcodeSymbolTable('[', 0x3b, new byte[] { 3, 3, 2, 1, 1, 1 });
            base.BarcodeSymbols['\\'] = new BarcodeSymbolTable('\\', 60, new byte[] { 3, 1, 4, 1, 1, 1 });
            base.BarcodeSymbols[']'] = new BarcodeSymbolTable(']', 0x3d, new byte[] { 2, 2, 1, 4, 1, 1 });
            base.BarcodeSymbols['^'] = new BarcodeSymbolTable('^', 0x3e, new byte[] { 4, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['_'] = new BarcodeSymbolTable('_', 0x3f, new byte[] { 1, 1, 1, 2, 2, 4 });
            base.BarcodeSymbols['\x00f0'] = new BarcodeSymbolTable('\x00f0', 0x66, new byte[] { 4, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['\x00f1'] = new BarcodeSymbolTable('\x00f1', 0x61, new byte[] { 4, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['\x00f2'] = new BarcodeSymbolTable('\x00f2', 0x60, new byte[] { 1, 1, 4, 3, 1, 1 });
            base.BarcodeSymbols['\x00f3'] = new BarcodeSymbolTable('\x00f3', 0x65, new byte[] { 3, 1, 1, 1, 4, 1 });
            base.BarcodeSymbols['\x00f4'] = new BarcodeSymbolTable('\x00f4', 0x62, new byte[] { 4, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['\x00fc'] = new BarcodeSymbolTable('\x00fc', 0x63, new byte[] { 1, 1, 3, 1, 4, 1 });
            base.BarcodeSymbols['\x00fb'] = new BarcodeSymbolTable('\x00fb', 100, new byte[] { 1, 1, 4, 1, 3, 1 });
            base.BarcodeSymbols['\x00f9'] = new BarcodeSymbolTable('\x00f9', 0x67, new byte[] { 2, 1, 1, 4, 1, 2 });
            base.BarcodeSymbols['\x00ff'] = new BarcodeSymbolTable('\x00ff', -1, new byte[] { 2, 3, 3, 1, 1, 1, 2 });
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        /// <param name="checkValue">The check value.</param>
        /// <returns>symbol</returns>
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