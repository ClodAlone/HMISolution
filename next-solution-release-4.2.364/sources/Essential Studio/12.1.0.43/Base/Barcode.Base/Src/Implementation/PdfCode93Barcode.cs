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
    /// Represents a Code93 barcode.
    /// </summary>
#if!XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Creates a new PdfCode93Barcode.
    /// PdfCode93Barcode code93 = new PdfCode93Barcode();
    /// //Set the font to code93.
    /// code93.Font = font;
    /// //Set the barcode text.
    /// code93.Text = "CODE93";
    /// //Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code93.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode93Barcode.
    /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode()
    /// 'Set the font to code93.
    /// code93.Font = font
    /// 'Set the barcode text.
    /// code93.Text = "CODE93"
    /// 'Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("code93.pdf")
    /// </code>
    /// </example> 
    /// <remarks> Only the following symbols are allowed in a Code 93 barcode: 1 2 3 4 5 6 7 8 9 0 A B C D E F G H I J K L M N O P Q R S T U V W X Y Z - . $ / + % SPACE
    /// All alphabetic characters are uppercase. If lowercase characters are required, then a Code 93 Extended barcode must be used.
    /// </remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCodPdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode93Barcode : PdfUnidimensionalBarcode
#else
    public class Code93Barcode : UnidimensionalBarcode
#endif
    {
#region Constructor
#if!XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode93Barcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode();
        /// //Set the font to code93.
        /// code93.Font = font;
        /// //Set the barcode text.
        /// code93.Text = "CODE93";
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode()
        /// 'Set the font to code93.
        /// code93.Font = font
        /// 'Set the barcode text.
        /// code93.Text = "CODE93"
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
        public PdfCode93Barcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code93Barcode"/> class.
        /// </summary>
        public Code93Barcode()
#endif
            : base()
        {
            Initialize();
        }

#if!XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode93Barcode"/> class.
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
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the font to code93.
        /// code93.Font = font;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the font to code93.
        /// code93.Font = font
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
        public PdfCode93Barcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code93Barcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code93Barcode(string text)
#endif
            : this()
        {
            base.Text = text;
        }
        #endregion

#region Methods
        /// <summary>
        /// Initializes the internal barcode symbol table
        /// </summary>
        internal void Initialize()
        {
            base.StartSymbol = '*';
            base.StopSymbol = '\x00ff';

            base.ValidatorExpression = @"^[\x41-\x5A\x30-\x39\x20\-\.\$\/\+\%\ ]+$";

            base.BarcodeSymbols['0'] = new BarcodeSymbolTable('0', 0, new byte[] { 1, 3, 1, 1, 1, 2 });
            base.BarcodeSymbols['1'] = new BarcodeSymbolTable('1', 1, new byte[] { 1, 1, 1, 2, 1, 3 });
            base.BarcodeSymbols['2'] = new BarcodeSymbolTable('2', 2, new byte[] { 1, 1, 1, 3, 1, 2 });
            base.BarcodeSymbols['3'] = new BarcodeSymbolTable('3', 3, new byte[] { 1, 1, 1, 4, 1, 1 });
            base.BarcodeSymbols['4'] = new BarcodeSymbolTable('4', 4, new byte[] { 1, 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['5'] = new BarcodeSymbolTable('5', 5, new byte[] { 1, 2, 1, 2, 1, 2 });
            base.BarcodeSymbols['6'] = new BarcodeSymbolTable('6', 6, new byte[] { 1, 2, 1, 3, 1, 1 });
            base.BarcodeSymbols['7'] = new BarcodeSymbolTable('7', 7, new byte[] { 1, 1, 1, 1, 1, 4 });
            base.BarcodeSymbols['8'] = new BarcodeSymbolTable('8', 8, new byte[] { 1, 3, 1, 2, 1, 1 });
            base.BarcodeSymbols['9'] = new BarcodeSymbolTable('9', 9, new byte[] { 1, 4, 1, 1, 1, 1 });
            base.BarcodeSymbols['A'] = new BarcodeSymbolTable('A', 10, new byte[] { 2, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['B'] = new BarcodeSymbolTable('B', 11, new byte[] { 2, 1, 1, 2, 1, 2 });
            base.BarcodeSymbols['C'] = new BarcodeSymbolTable('C', 12, new byte[] { 2, 1, 1, 3, 1, 1 });
            base.BarcodeSymbols['D'] = new BarcodeSymbolTable('D', 13, new byte[] { 2, 2, 1, 1, 1, 2 });
            base.BarcodeSymbols['E'] = new BarcodeSymbolTable('E', 14, new byte[] { 2, 2, 1, 2, 1, 1 });
            base.BarcodeSymbols['F'] = new BarcodeSymbolTable('F', 15, new byte[] { 2, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['G'] = new BarcodeSymbolTable('G', 0x10, new byte[] { 1, 1, 2, 1, 1, 3 });
            base.BarcodeSymbols['H'] = new BarcodeSymbolTable('H', 0x11, new byte[] { 1, 1, 2, 2, 1, 2 });
            base.BarcodeSymbols['I'] = new BarcodeSymbolTable('I', 0x12, new byte[] { 1, 1, 2, 3, 1, 1 });
            base.BarcodeSymbols['J'] = new BarcodeSymbolTable('J', 0x13, new byte[] { 1, 2, 2, 1, 1, 2 });
            base.BarcodeSymbols['K'] = new BarcodeSymbolTable('K', 20, new byte[] { 1, 3, 2, 1, 1, 1 });
            base.BarcodeSymbols['L'] = new BarcodeSymbolTable('L', 0x15, new byte[] { 1, 1, 1, 1, 2, 3 });
            base.BarcodeSymbols['M'] = new BarcodeSymbolTable('M', 0x16, new byte[] { 1, 1, 1, 2, 2, 2 });
            base.BarcodeSymbols['N'] = new BarcodeSymbolTable('N', 0x17, new byte[] { 1, 1, 1, 3, 2, 1 });
            base.BarcodeSymbols['O'] = new BarcodeSymbolTable('O', 0x18, new byte[] { 1, 2, 1, 1, 2, 2 });
            base.BarcodeSymbols['P'] = new BarcodeSymbolTable('P', 0x19, new byte[] { 1, 3, 1, 1, 2, 1 });
            base.BarcodeSymbols['Q'] = new BarcodeSymbolTable('Q', 0x1a, new byte[] { 2, 1, 2, 1, 1, 2 });
            base.BarcodeSymbols['R'] = new BarcodeSymbolTable('R', 0x1b, new byte[] { 2, 1, 2, 2, 1, 1 });
            base.BarcodeSymbols['S'] = new BarcodeSymbolTable('S', 0x1c, new byte[] { 2, 1, 1, 1, 2, 2 });
            base.BarcodeSymbols['T'] = new BarcodeSymbolTable('T', 0x1d, new byte[] { 2, 1, 1, 2, 2, 1 });
            base.BarcodeSymbols['U'] = new BarcodeSymbolTable('U', 30, new byte[] { 2, 2, 1, 1, 2, 1 });
            base.BarcodeSymbols['V'] = new BarcodeSymbolTable('V', 0x1f, new byte[] { 2, 2, 2, 1, 1, 1 });
            base.BarcodeSymbols['W'] = new BarcodeSymbolTable('W', 0x20, new byte[] { 1, 1, 2, 1, 2, 2 });
            base.BarcodeSymbols['X'] = new BarcodeSymbolTable('X', 0x21, new byte[] { 1, 1, 2, 2, 2, 1 });
            base.BarcodeSymbols['Y'] = new BarcodeSymbolTable('Y', 0x22, new byte[] { 1, 2, 2, 1, 2, 1 });
            base.BarcodeSymbols['Z'] = new BarcodeSymbolTable('Z', 0x23, new byte[] { 1, 2, 3, 1, 1, 1 });
            base.BarcodeSymbols['-'] = new BarcodeSymbolTable('-', 0x24, new byte[] { 1, 2, 1, 1, 3, 1 });
            base.BarcodeSymbols['.'] = new BarcodeSymbolTable('.', 0x25, new byte[] { 3, 1, 1, 1, 1, 2 });
            base.BarcodeSymbols[' '] = new BarcodeSymbolTable(' ', 0x26, new byte[] { 3, 1, 1, 2, 1, 1 });
            base.BarcodeSymbols['$'] = new BarcodeSymbolTable('$', 0x27, new byte[] { 3, 2, 1, 1, 1, 1 });
            base.BarcodeSymbols['/'] = new BarcodeSymbolTable('/', 40, new byte[] { 1, 1, 2, 1, 3, 1 });
            base.BarcodeSymbols['+'] = new BarcodeSymbolTable('+', 0x29, new byte[] { 1, 1, 3, 1, 2, 1 });
            base.BarcodeSymbols['%'] = new BarcodeSymbolTable('%', 0x2a, new byte[] { 2, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['*'] = new BarcodeSymbolTable('*', 0, new byte[] { 1, 1, 1, 1, 4, 1 });
            base.BarcodeSymbols['\x00ff'] = new BarcodeSymbolTable('\x00ff', 0, new byte[] { 1, 1, 1, 1, 4, 1, 1 });
            base.BarcodeSymbols['\x00fb'] = new BarcodeSymbolTable('\x00fb', 0x2b, new byte[] { 1, 2, 1, 2, 2, 0 });
            base.BarcodeSymbols['\x00fc'] = new BarcodeSymbolTable('\x00fc', 0x2c, new byte[] { 3, 1, 2, 1, 1, 1 });
            base.BarcodeSymbols['\x00fd'] = new BarcodeSymbolTable('\x00fd', 0x2d, new byte[] { 3, 1, 1, 1, 2, 1 });
            base.BarcodeSymbols['\x00fe'] = new BarcodeSymbolTable('\x00fe', 0x2e, new byte[] { 1, 2, 2, 2, 1, 1 });
        }

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

            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text : base.ExtendedText;
            char[] ch = new char[2];
            ch = GetCheckSumSymbols();
            return ch;
        }

        /// <summary>
        /// Internal method to calculate the check-digit
        /// </summary>
        /// <returns>symbols</returns>
        protected internal char[] GetCheckSumSymbols()
        {
            string data = base.Text;
            char[] chArray = new char[2];
            int checkValue = 0;
            string dataToEncode = data;
            int length = dataToEncode.Length;
            for (int i = 0; i < length; i++)
            {
                int num4 = (length - i) % 20;
                if (num4 == 0)
                {
                    num4 = 20;
                }

                int temp = (BarcodeSymbols[Text[i]] as BarcodeSymbolTable).CheckDigit;
                checkValue += temp * num4;
            }

            checkValue = checkValue % 0x2f;

            chArray[0] = Convert.ToChar(checkValue);
            char c = ' ';
            foreach (KeyValuePair<char,BarcodeSymbolTable> entry in BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.CheckDigit == checkValue)
                {
                    c = pattern.Symbol;
                    break;
                }
            }

            string tempstr = base.Text;
            tempstr = tempstr + c;
            chArray[0] = c;
            data = tempstr;
            checkValue = 0;
            dataToEncode = data;
            length = dataToEncode.Length;
            for (int j = 0; j < length; j++)
            {
                int num6 = (length - j) % 15;
                if (num6 == 0)
                {
                    num6 = 15;
                }

                int value = (BarcodeSymbols[tempstr[j]] as BarcodeSymbolTable).CheckDigit;
                checkValue += value * num6;
            }

            checkValue = checkValue % 0x2f;
            data = data + checkValue;

            char k = ' ';
            foreach (KeyValuePair<char,BarcodeSymbolTable> entry in BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.CheckDigit == checkValue)
                {
                    k = pattern.Symbol;
                    break;
                }
            }

            tempstr = tempstr + k;
            chArray[1] = k;

            base.Text = base.Text + BarcodeSymbols[(char)checkValue];

            return chArray;
        }
        #endregion
    }
}
#endif