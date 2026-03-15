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
    /// Represents a Code32 barcode.
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
    /// //Creates a new PdfCode32Barcode.
    /// PdfCode32Barcode code32 = new PdfCode32Barcode();
    /// //Set the font to code32.
    /// code32.Font = font;
    /// //Set the barcode text.
    /// code32.Text = "01234567";
    /// //Draw a barcode in the new Page.
    /// code32.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code32.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode32Barcode.
    /// Dim code32 As PdfCode32Barcode = New PdfCode32Barcode()
    /// 'Set the font to code32.
    /// code32.Font = font
    /// 'Set the barcode text.
    /// code32.Text = "01234567"
    /// 'Draw a barcode in the new Page.
    /// code32.Draw(page, new PointF(25, 500))
    /// 'Save the document to disk.
    /// document.Save("Code32.pdf")
    /// </code>
    /// </example>
    /// <remarks> Only the following symbols are allowed in a Code 32 barcode: 1 2 3 4 5 6 7 8 9 0. The barcode length is 9 digits (8 user defined digits + 1 check digit).
    /// Code 32 barcodes are also known as Italian Pharmacode barcodes. 
    /// </remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode32Barcode : PdfCode39Barcode
#else
    public class Code32Barcode : Code39Barcode
#endif
    {
        #region Fields
        /// <summary>
        /// Local variable to store the Checksum character value.
        /// </summary>
        private char[] checkSumSymbols = null;
        #endregion

        #region Constructor
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode32Barcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode32Barcode.
        /// PdfCode32Barcode code32 = new PdfCode32Barcode();
        /// //Set the font to code32.
        /// code32.Font = font;
        /// //Set the barcode text.
        /// code32.Text = "01234567";
        /// //Draw a barcode in the new Page.
        /// code32.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code32.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode32Barcode.
        /// Dim code32 As PdfCode32Barcode = New PdfCode32Barcode()
        /// 'Set the font to code32.
        /// code32.Font = font
        /// 'Set the barcode text.
        /// code32.Text = "01234567"
        /// 'Draw a barcode in the new Page.
        /// code32.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code32.pdf")
        /// </code>
        /// </example>
        public PdfCode32Barcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code32Barcode"/> class.
        /// </summary>
        public Code32Barcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode32Barcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode32Barcode.
        /// PdfCode32Barcode code32 = new PdfCode32Barcode("01234567");
        /// //Set the font to code32.
        /// code32.Font = font;
        /// //Draw a barcode in the new Page.
        /// code32.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code32.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode32Barcode.
        /// Dim code32 As PdfCode32Barcode = New PdfCode32Barcode("01234567")
        /// 'Set the font to code32.
        /// code32.Font = font
        /// 'Draw a barcode in the new Page.
        /// code32.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code32.pdf")
        /// </code>
        /// </example>
        /// <param name="text">The Barcode Text.</param>
        public PdfCode32Barcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code32Barcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code32Barcode(string text)
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
            int num = base.Text[0] - '0';
            int num2 = 2 * (base.Text[1] - '0');
            int num3 = base.Text[2] - '0';
            int num4 = 2 * (base.Text[3] - '0');
            int num5 = base.Text[4] - '0';
            int num6 = 2 * (base.Text[5] - '0');
            int num7 = base.Text[6] - '0';
            int num8 = 2 * (base.Text[7] - '0');
            int num9 = (((((((num2 / 10) + (num4 / 10)) + (num6 / 10)) + (num8 / 10)) + (num2 % 10)) + (num4 % 10)) + (num6 % 10)) + (num8 % 10);
            int num10 = ((num + num3) + num5) + num7;
            int num11 = (num9 + num10) % 10;
            return new char[] { ((char)(num11 + 0x30)) };
        }

        /// <summary>
        /// Gets the barcode symbols.
        /// </summary>
        /// <returns>Encoded data</returns>
        protected string GetBarcodeSymbols()
        {
            string str = "";
            checkSumSymbols = this.CalculateCheckDigit();
            if (checkSumSymbols != null)
            {
                for (int i = 0; i < checkSumSymbols.Length; i++)
                {
                    if (EnableCheckDigit == true)
                    {
                        str = str + checkSumSymbols[i];
                    }
                }
            }

            return this.GetDataToEncode(base.Text + str);
        }

        /// <summary>
        /// To get the Actual Encoded Text from from original Text..
        /// </summary>
        /// <param name="originalData">The original data.</param>
        /// <returns>original Data</returns>
        protected string GetDataToEncode(string originalData)
        {
            string text = null;
            int num = int.Parse(originalData);
            while (num != 0)
            {
                int num2 = num % 0x20;
                num /= 0x20;

                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in this.BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.CheckDigit == num2)
                    {
                        char c = pattern.Symbol;
                        text += c;
                    }
                }
            }

            return "0" + text;
        }

        /// <summary>
        /// Returns the Actual text to encode.
        /// </summary>
        /// <returns>The Actual Text.</returns>
        protected override string GetTextToEncode()
        {
            if (base.Text.Length != 8)
            {
# if !XAML && !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException("Barcode Text Length that are not accepted by this barcode specification.");
#else
                throw new BarcodeException("Barcode Text Length that are not accepted by this barcode specification.");
#endif
            }

            if (!Validate(base.Text))
            {
# if !XAML && !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException("Barcode text contains characters that are not accepted by this barcode specification.");
#else
                throw new BarcodeException("Barcode text contains characters that are not accepted by this barcode specification.");
#endif
            }

            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text.Trim('*') : base.ExtendedText.Trim('*');

            code = GetBarcodeSymbols();
            if (ShowCheckDigit == true && EnableCheckDigit == true)
            {
                base.Text = base.Text + checkSumSymbols[0];
            }

            base.Text = "A" + base.Text;

            return code;
        }

        /// <summary>
        /// Initializes the internal barcode symbol table
        /// </summary>
        private void Initialize()
        {
            base.StartSymbol = '*';
            base.StopSymbol = '*';

            base.ValidatorExpression = @"^[\x41-\x5A\x30-\x39\x20\-\*\.\/\+\%]+$";

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
            base.BarcodeSymbols['B'] = new BarcodeSymbolTable('B', 10, new byte[] { 1, 1, 3, 1, 1, 3, 1, 1, 3 });
            base.BarcodeSymbols['C'] = new BarcodeSymbolTable('C', 11, new byte[] { 3, 1, 3, 1, 1, 3, 1, 1, 1 });
            base.BarcodeSymbols['D'] = new BarcodeSymbolTable('D', 12, new byte[] { 1, 1, 1, 1, 3, 3, 1, 1, 3 });
            base.BarcodeSymbols['F'] = new BarcodeSymbolTable('F', 13, new byte[] { 1, 1, 3, 1, 3, 3, 1, 1, 1 });
            base.BarcodeSymbols['G'] = new BarcodeSymbolTable('G', 14, new byte[] { 1, 1, 1, 1, 1, 3, 3, 1, 3 });
            base.BarcodeSymbols['H'] = new BarcodeSymbolTable('H', 15, new byte[] { 3, 1, 1, 1, 1, 3, 3, 1, 1 });
            base.BarcodeSymbols['J'] = new BarcodeSymbolTable('J', 0x10, new byte[] { 1, 1, 1, 1, 3, 3, 3, 1, 1 });
            base.BarcodeSymbols['K'] = new BarcodeSymbolTable('K', 0x11, new byte[] { 3, 1, 1, 1, 1, 1, 1, 3, 3 });
            base.BarcodeSymbols['L'] = new BarcodeSymbolTable('L', 0x12, new byte[] { 1, 1, 3, 1, 1, 1, 1, 3, 3 });
            base.BarcodeSymbols['M'] = new BarcodeSymbolTable('M', 0x13, new byte[] { 3, 1, 3, 1, 1, 1, 1, 3, 1 });
            base.BarcodeSymbols['N'] = new BarcodeSymbolTable('N', 20, new byte[] { 1, 1, 1, 1, 3, 1, 1, 3, 3 });
            base.BarcodeSymbols['P'] = new BarcodeSymbolTable('P', 0x15, new byte[] { 1, 1, 3, 1, 3, 1, 1, 3, 1 });
            base.BarcodeSymbols['Q'] = new BarcodeSymbolTable('Q', 0x16, new byte[] { 1, 1, 1, 1, 1, 1, 3, 3, 3 });
            base.BarcodeSymbols['R'] = new BarcodeSymbolTable('R', 0x17, new byte[] { 3, 1, 1, 1, 1, 1, 3, 3, 1 });
            base.BarcodeSymbols['S'] = new BarcodeSymbolTable('S', 0x18, new byte[] { 1, 1, 3, 1, 1, 1, 3, 3, 1 });
            base.BarcodeSymbols['T'] = new BarcodeSymbolTable('T', 0x19, new byte[] { 1, 1, 1, 1, 3, 1, 3, 3, 1 });
            base.BarcodeSymbols['U'] = new BarcodeSymbolTable('U', 0x1a, new byte[] { 3, 3, 1, 1, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['V'] = new BarcodeSymbolTable('V', 0x1b, new byte[] { 1, 3, 3, 1, 1, 1, 1, 1, 3 });
            base.BarcodeSymbols['W'] = new BarcodeSymbolTable('W', 0x1c, new byte[] { 3, 3, 3, 1, 1, 1, 1, 1, 1 });
            base.BarcodeSymbols['X'] = new BarcodeSymbolTable('X', 0x1d, new byte[] { 1, 3, 1, 1, 3, 1, 1, 1, 3 });
            base.BarcodeSymbols['Y'] = new BarcodeSymbolTable('Y', 30, new byte[] { 3, 3, 1, 1, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['Z'] = new BarcodeSymbolTable('Z', 0x1f, new byte[] { 1, 3, 3, 1, 3, 1, 1, 1, 1 });
            base.BarcodeSymbols['*'] = new BarcodeSymbolTable('*', 0, new byte[] { 1, 3, 1, 1, 3, 1, 3, 1, 1 });
        }
        #endregion
    }
}
#endif