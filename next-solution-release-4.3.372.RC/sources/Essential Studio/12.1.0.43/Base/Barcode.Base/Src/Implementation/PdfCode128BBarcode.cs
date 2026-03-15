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
    /// Represents a Code128B Barcode.
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
    /// //Creates a new PdfCode128BBarcode.
    /// PdfCode128BBarcode code128B = new PdfCode128BBarcode();
    /// //Set the font to code128B.
    /// code128B.Font = font;
    /// //Set the barcode text.
    /// code128B.Text = "CODE128B";
    /// //Draw a barcode in the new Page.
    /// code128B.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code128B.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode128BBarcode.
    /// Dim code32 As PdfCode128BBarcode = New PdfCode128BBarcode()
    /// 'Set the font to code128B.
    /// code128B.Font = font;
    /// 'Set the barcode text.
    /// code128B.Text = "Code128B"
    /// 'Draw a barcode in the new Page.
    /// code128B.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code128B.pdf")
    /// </code>
    /// </example>
    /// <remarks> Only the following symbols are allowed in a Code 128 B barcode:SPACE ! " # $ % ' ( ) * + , - . / 0 12 3 4 5 6 7 8 9 : ; ? @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ \ ]^ _ ` a b c d e f g h i j k l m n o p q r s t u v w x y z { | } ~ DEL (\x7F) FNC1 (\xF0) FNC2 (\xF1) FNC3 (\xF2) FNC4 (\xF3) SHIFT (\xF4). </remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode128BBarcode : PdfUnidimensionalBarcode
#else
    public class Code128BBarcode : UnidimensionalBarcode
#endif
    {
        #region Constructors
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode128BBarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode128BBarcode.
        /// PdfCode128BBarcode code128B = new PdfCode128BBarcode();
        /// //Set the font to code128B.
        /// code128B.Font = font;
        /// //Set the barcode text.
        /// code128B.Text = "CODE128B";
        /// //Draw a barcode in the new Page.
        /// code128B.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code128B.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode128BBarcode.
        /// Dim code32 As PdfCode128BBarcode = New PdfCode128BBarcode()
        /// 'Set the font to code128B.
        /// code128B.Font = font;
        /// 'Set the barcode text.
        /// code128B.Text = "Code128B"
        /// 'Draw a barcode in the new Page.
        /// code128B.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code128B.pdf")
        /// </code>
        /// </example>
        public PdfCode128BBarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code128BBarcode"/> class.
        /// </summary>
        public Code128BBarcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode128BBarcode"/> class.
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
        /// //Creates a new PdfCode128BBarcode.
        /// PdfCode128BBarcode code128B = new PdfCode128BBarcode("Code128B");
        /// //Set the font to code128B.
        /// code128B.Font = font;
        /// //Draw a barcode in the new Page.
        /// code128B.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code128B.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode128BBarcode.
        /// Dim code32 As PdfCode128BBarcode = New PdfCode128BBarcode("Code128B")
        /// 'Set the font to code128B.
        /// code128B.Font = font;
        /// 'Draw a barcode in the new Page.
        /// code128B.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code128B.pdf")
        /// </code>
        /// </example>
        public PdfCode128BBarcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code128BBarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code128BBarcode(string text)
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
#if!XAML && !NETFX_CORE && !GDI && !WP
                    throw new PdfBarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
#else
                    throw new BarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
#endif
                }

                checkValue += (pattern.CheckDigit * (i + 1));
                i++;
            }

            checkValue += 104;
            checkValue = checkValue % 0x67;
            char[] ch = new char[1];
            ch[0] = GetSymbol(checkValue);
            return ch;
        }

        /// <summary>
        /// Initializes the internal barcode symbol table
        /// </summary>
        private void Initialize()
        {
            base.StartSymbol = '\x00fd';
            base.StopSymbol = '\x00ff';

            base.ValidatorExpression = @"^[\x00-\x7F]";

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
            base.BarcodeSymbols['`'] = new BarcodeSymbolTable('`', 0x40, new byte[] { 1, 1, 1, 4, 2, 2 });
            base.BarcodeSymbols['a'] = new BarcodeSymbolTable('a', 0x41, new byte[] { 1, 2, 1, 1, 2, 4 });
            base.BarcodeSymbols['b'] = new BarcodeSymbolTable('b', 0x42, new byte[] { 1, 2, 1, 4, 2, 1 });
            base.BarcodeSymbols['c'] = new BarcodeSymbolTable('c', 0x43, new byte[] { 1, 4, 1, 1, 2, 2 });
            base.BarcodeSymbols['d'] = new BarcodeSymbolTable('d', 0x44, new byte[] { 1, 4, 1, 2, 2, 1 });
            base.BarcodeSymbols['e'] = new BarcodeSymbolTable('e', 0x45, new byte[] { 1, 1, 2, 2, 1, 4 });
            base.BarcodeSymbols['f'] = new BarcodeSymbolTable('f', 70, new byte[] { 1, 1, 2, 4, 1, 2 });
            base.BarcodeSymbols['g'] = new BarcodeSymbolTable('g', 0x47, new byte[] { 1, 2, 2, 1, 1, 4 });
            base.BarcodeSymbols['h'] = new BarcodeSymbolTable('h', 0x48, new byte[] { 1, 2, 2, 4, 1, 1 });
            base.BarcodeSymbols['i'] = new BarcodeSymbolTable('i', 0x49, new byte[] { 1, 4, 2, 1, 1, 2 });
            base.BarcodeSymbols['j'] = new BarcodeSymbolTable('j', 0x4a, new byte[] { 1, 4, 2, 2, 1, 1 });
            base.BarcodeSymbols['k'] = new BarcodeSymbolTable('k', 0x4b, new byte[] { 2, 4, 1, 2, 1, 1 });
            base.BarcodeSymbols['l'] = new BarcodeSymbolTable('l', 0x4c, new byte[] { 2, 2, 1, 1, 1, 4 });
            base.BarcodeSymbols['m'] = new BarcodeSymbolTable('m', 0x4d, new byte[] { 4, 1, 3, 1, 1, 1 });
            base.BarcodeSymbols['n'] = new BarcodeSymbolTable('n', 0x4e, new byte[] { 2, 4, 1, 1, 1, 2 });
            base.BarcodeSymbols['o'] = new BarcodeSymbolTable('o', 0x4f, new byte[] { 1, 3, 4, 1, 1, 1 });
            base.BarcodeSymbols['p'] = new BarcodeSymbolTable('p', 80, new byte[] { 1, 1, 1, 2, 4, 2 });
            base.BarcodeSymbols['q'] = new BarcodeSymbolTable('q', 0x51, new byte[] { 1, 2, 1, 1, 4, 2 });
            base.BarcodeSymbols['r'] = new BarcodeSymbolTable('r', 0x52, new byte[] { 1, 2, 1, 2, 4, 1 });
            base.BarcodeSymbols['s'] = new BarcodeSymbolTable('s', 0x53, new byte[] { 1, 1, 4, 2, 1, 2 });
            base.BarcodeSymbols['t'] = new BarcodeSymbolTable('t', 0x54, new byte[] { 1, 2, 4, 1, 1, 2 });
            base.BarcodeSymbols['u'] = new BarcodeSymbolTable('u', 0x55, new byte[] { 1, 2, 4, 2, 1, 1 });
            base.BarcodeSymbols['v'] = new BarcodeSymbolTable('v', 0x56, new byte[] { 4, 1, 1, 2, 1, 2 });
            base.BarcodeSymbols['w'] = new BarcodeSymbolTable('w', 0x57, new byte[] { 4, 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['x'] = new BarcodeSymbolTable('x', 0x58, new byte[] { 4, 2, 1, 2, 1, 1 });
            base.BarcodeSymbols['y'] = new BarcodeSymbolTable('y', 0x59, new byte[] { 2, 1, 2, 1, 4, 1 });
            base.BarcodeSymbols['z'] = new BarcodeSymbolTable('z', 90, new byte[] { 2, 1, 4, 1, 2, 1 });
            base.BarcodeSymbols['{'] = new BarcodeSymbolTable('{', 0x5b, new byte[] { 4, 1, 2, 1, 2, 1 });
            base.BarcodeSymbols['|'] = new BarcodeSymbolTable('|', 0x5c, new byte[] { 1, 1, 1, 1, 4, 3 });
            base.BarcodeSymbols['}'] = new BarcodeSymbolTable('}', 0x5d, new byte[] { 1, 1, 1, 3, 4, 1 });
            base.BarcodeSymbols['~'] = new BarcodeSymbolTable('~', 0x5e, new byte[] { 1, 3, 1, 1, 4, 1 });
            base.BarcodeSymbols['\x007f'] = new BarcodeSymbolTable('\x007f', 0x5f, new byte[] { 1, 1, 4, 1, 1, 3 });
            base.BarcodeSymbols['\x00f0'] = new BarcodeSymbolTable('\x00f0', 0x66, new byte[] { 4, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['\x00f1'] = new BarcodeSymbolTable('\x00f1', 0x61, new byte[] { 4, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['\x00f2'] = new BarcodeSymbolTable('\x00f2', 0x60, new byte[] { 1, 1, 4, 3, 1, 1 });
            base.BarcodeSymbols['\x00f3'] = new BarcodeSymbolTable('\x00f3', 100, new byte[] { 1, 1, 4, 1, 3, 1 });
            base.BarcodeSymbols['\x00f4'] = new BarcodeSymbolTable('\x00f4', 0x62, new byte[] { 4, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['\x00fc'] = new BarcodeSymbolTable('\x00fc', 0x63, new byte[] { 1, 1, 3, 1, 4, 1 });
            base.BarcodeSymbols['\x00fa'] = new BarcodeSymbolTable('\x00fa', 0x65, new byte[] { 3, 1, 1, 1, 4, 1 });
            base.BarcodeSymbols['\x00fd'] = new BarcodeSymbolTable('\x00fd', 0x68, new byte[] { 2, 1, 1, 2, 1, 4 });
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