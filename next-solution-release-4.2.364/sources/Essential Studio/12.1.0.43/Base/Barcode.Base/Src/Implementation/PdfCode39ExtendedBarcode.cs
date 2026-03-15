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
    /// Represents a Code39 Extended barcode.
    /// Code 39 Extended is designed to encode 128 full ASCII characters.
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
    /// //Creates a new PdfCode39ExtendedBarcode.
    /// PdfCode39ExtendedBarcode code39Ext = new PdfCode39ExtendedBarcode();
    /// //Set the font to code39Ext.
    /// code39Ext.Font = font;
    /// //Set the barcode text.
    /// code39Ext.Text = "Code39Ext";
    /// //Draw a barcode in the new Page.
    /// code39Ext.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code39Ext.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode39ExtendedBarcode.
    /// Dim code39Ext As PdfCode39ExtendedBarcode = New PdfCode39ExtendedBarcode()
    /// 'Set the font to code39Ext.
    /// code39Ext.Font = font
    /// 'Set the barcode text.
    /// code39Ext.Text = "Code39Ext"
    /// 'Draw a barcode in the new Page.
    /// code39Ext.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code39Ext.pdf")
    /// </code>
    /// </example> 
    /// <remarks> All 128 ASCII characters can be encoded in an extended Code 39 barcode</remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    /// <seealso cref="PdfCode93ExtendedBarcode"/> Class
    public class PdfCode39ExtendedBarcode : PdfCode39Barcode
#else
    public class Code39ExtendedBarcode : Code39Barcode
#endif
    {
        #region Fields
        private Dictionary<char, char[]> m_extendedCodes;
        #endregion

        #region Constructor
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode39ExtendedBarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode39ExtendedBarcode.
        /// PdfCode39ExtendedBarcode code39Ext = new PdfCode39ExtendedBarcode();
        /// //Set the font to code39Ext.
        /// code39Ext.Font = font;
        /// //Set the barcode text.
        /// code39Ext.Text = "Code39Ext";
        /// //Draw a barcode in the new Page.
        /// code39Ext.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code39Ext.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode39ExtendedBarcode.
        /// Dim code39Ext As PdfCode39ExtendedBarcode = New PdfCode39ExtendedBarcode()
        /// 'Set the font to code39Ext.
        /// code39Ext.Font = font
        /// 'Set the barcode text.
        /// code39Ext.Text = "Code39Ext"
        /// 'Draw a barcode in the new Page.
        /// code39Ext.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code39Ext.pdf")
        /// </code>
        /// </example> 
        public PdfCode39ExtendedBarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code39ExtendedBarcode"/> class.
        /// </summary>
        public Code39ExtendedBarcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode39ExtendedBarcode"/> class.
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
        /// //Creates a new PdfCode39ExtendedBarcode.
        /// PdfCode39ExtendedBarcode code39Ext = new PdfCode39ExtendedBarcode("Code39Ext");
        /// //Set the font to code39Ext.
        /// code39Ext.Font = font;
        /// //Draw a barcode in the new Page.
        /// code39Ext.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code39Ext.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode39ExtendedBarcode.
        /// Dim code39Ext As PdfCode39ExtendedBarcode = New PdfCode39ExtendedBarcode("Code39Ext")
        /// 'Set the font to code39Ext.
        /// code39Ext.Font = font
        /// 'Draw a barcode in the new Page.
        /// code39Ext.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code39Ext.pdf")
        /// </code>
        /// </example> 
        public PdfCode39ExtendedBarcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code39ExtendedBarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code39ExtendedBarcode(string text)
#endif
            : this()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char[] chArray = (char[])m_extendedCodes[text[i]];
                if (chArray != null)
                {
                    for (int j = 0; j < chArray.Length; j++)
                    {
                        builder.Append(chArray[j]);
                    }
                }
            }

            base.ExtendedText = builder.ToString();
            base.Text = text;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Internal method to calculate the check-digit
        /// </summary>
        /// <returns>check digit</returns>
        protected internal override char[] CalculateCheckDigit()
        {
            if (!base.EnableCheckDigit)
            {
                return null;
            }

            int checkValue = 0;
            GetExtendedText();
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
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            base.Initialize();

            base.ValidatorExpression = @"^[\x00-\x7F]+$";

            m_extendedCodes = new Dictionary<char, char[]>();

            // http://www.cdrummond.qc.ca/cegep/informat/Professeurs/Alain/files/ascii.htm

            m_extendedCodes['\0'] = new char[] { '%', 'U' };
            m_extendedCodes['\x0001'] = new char[] { '$', 'A' };
            m_extendedCodes['\x0002'] = new char[] { '$', 'B' };
            m_extendedCodes['\x0003'] = new char[] { '$', 'C' };
            m_extendedCodes['\x0004'] = new char[] { '$', 'D' };
            m_extendedCodes['\x0005'] = new char[] { '$', 'E' };
            m_extendedCodes['\x0006'] = new char[] { '$', 'F' };
            m_extendedCodes['\a'] = new char[] { '$', 'G' };
            m_extendedCodes['\b'] = new char[] { '$', 'H' };
            m_extendedCodes['\t'] = new char[] { '$', 'I' };
            m_extendedCodes['\n'] = new char[] { '$', 'J' };
            m_extendedCodes['\v'] = new char[] { '$', 'K' };
            m_extendedCodes['\f'] = new char[] { '$', 'L' };
            m_extendedCodes['\r'] = new char[] { '$', 'M' };
            m_extendedCodes['\x000e'] = new char[] { '$', 'N' };
            m_extendedCodes['\x000f'] = new char[] { '$', 'O' };
            m_extendedCodes['\x0010'] = new char[] { '$', 'P' };
            m_extendedCodes['\x0011'] = new char[] { '$', 'Q' };
            m_extendedCodes['\x0012'] = new char[] { '$', 'R' };
            m_extendedCodes['\x0013'] = new char[] { '$', 'S' };
            m_extendedCodes['\x0014'] = new char[] { '$', 'T' };
            m_extendedCodes['\x0015'] = new char[] { '$', 'U' };
            m_extendedCodes['\x0016'] = new char[] { '$', 'V' };
            m_extendedCodes['\x0017'] = new char[] { '$', 'W' };
            m_extendedCodes['\x0018'] = new char[] { '$', 'X' };
            m_extendedCodes['\x0019'] = new char[] { '$', 'Y' };
            m_extendedCodes['\x001a'] = new char[] { '$', 'Z' };
            m_extendedCodes['\x001b'] = new char[] { '%', 'A' };
            m_extendedCodes['\x001c'] = new char[] { '%', 'B' };
            m_extendedCodes['\x001d'] = new char[] { '%', 'C' };
            m_extendedCodes['\x001e'] = new char[] { '%', 'D' };
            m_extendedCodes['\x001f'] = new char[] { '%', 'E' };
            m_extendedCodes[' '] = new char[] { ' ' };
            m_extendedCodes['!'] = new char[] { '/', 'A' };
            m_extendedCodes['"'] = new char[] { '/', 'B' };
            m_extendedCodes['#'] = new char[] { '/', 'C' };
            m_extendedCodes['$'] = new char[] { '/', 'D' };
            m_extendedCodes['%'] = new char[] { '/', 'E' };
            m_extendedCodes['&'] = new char[] { '/', 'F' };
            m_extendedCodes['\''] = new char[] { '/', 'G' };
            m_extendedCodes['('] = new char[] { '/', 'H' };
            m_extendedCodes[')'] = new char[] { '/', 'I' };
            m_extendedCodes['*'] = new char[] { '/', 'J' };
            m_extendedCodes['+'] = new char[] { '/', 'K' };
            m_extendedCodes[','] = new char[] { '/', 'L' };
            m_extendedCodes['-'] = new char[] { '/', 'M' };
            m_extendedCodes['.'] = new char[] { '/', 'N' };
            m_extendedCodes['/'] = new char[] { '/', 'O' };
            m_extendedCodes['0'] = new char[] { '0' };
            m_extendedCodes['1'] = new char[] { '1' };
            m_extendedCodes['2'] = new char[] { '2' };
            m_extendedCodes['3'] = new char[] { '3' };
            m_extendedCodes['4'] = new char[] { '4' };
            m_extendedCodes['5'] = new char[] { '5' };
            m_extendedCodes['6'] = new char[] { '6' };
            m_extendedCodes['7'] = new char[] { '7' };
            m_extendedCodes['8'] = new char[] { '8' };
            m_extendedCodes['9'] = new char[] { '9' };
            m_extendedCodes[':'] = new char[] { '/', 'Z' };
            m_extendedCodes[';'] = new char[] { '%', 'F' };
            m_extendedCodes['<'] = new char[] { '%', 'G' };
            m_extendedCodes['='] = new char[] { '%', 'H' };
            m_extendedCodes['>'] = new char[] { '%', 'I' };
            m_extendedCodes['?'] = new char[] { '%', 'J' };
            m_extendedCodes['@'] = new char[] { '%', 'V' };
            m_extendedCodes['A'] = new char[] { 'A' };
            m_extendedCodes['B'] = new char[] { 'B' };
            m_extendedCodes['C'] = new char[] { 'C' };
            m_extendedCodes['D'] = new char[] { 'D' };
            m_extendedCodes['E'] = new char[] { 'E' };
            m_extendedCodes['F'] = new char[] { 'F' };
            m_extendedCodes['G'] = new char[] { 'G' };
            m_extendedCodes['H'] = new char[] { 'H' };
            m_extendedCodes['I'] = new char[] { 'I' };
            m_extendedCodes['J'] = new char[] { 'J' };
            m_extendedCodes['K'] = new char[] { 'K' };
            m_extendedCodes['L'] = new char[] { 'L' };
            m_extendedCodes['M'] = new char[] { 'M' };
            m_extendedCodes['N'] = new char[] { 'N' };
            m_extendedCodes['O'] = new char[] { 'O' };
            m_extendedCodes['P'] = new char[] { 'P' };
            m_extendedCodes['Q'] = new char[] { 'Q' };
            m_extendedCodes['R'] = new char[] { 'R' };
            m_extendedCodes['S'] = new char[] { 'S' };
            m_extendedCodes['T'] = new char[] { 'T' };
            m_extendedCodes['U'] = new char[] { 'U' };
            m_extendedCodes['V'] = new char[] { 'V' };
            m_extendedCodes['W'] = new char[] { 'W' };
            m_extendedCodes['X'] = new char[] { 'X' };
            m_extendedCodes['Y'] = new char[] { 'Y' };
            m_extendedCodes['Z'] = new char[] { 'Z' };
            m_extendedCodes['['] = new char[] { '%', 'K' };
            m_extendedCodes['\\'] = new char[] { '%', 'L' };
            m_extendedCodes[']'] = new char[] { '%', 'M' };
            m_extendedCodes['^'] = new char[] { '%', 'N' };
            m_extendedCodes['_'] = new char[] { '%', 'O' };
            m_extendedCodes['`'] = new char[] { '%', 'W' };
            m_extendedCodes['a'] = new char[] { '+', 'A' };
            m_extendedCodes['b'] = new char[] { '+', 'B' };
            m_extendedCodes['c'] = new char[] { '+', 'C' };
            m_extendedCodes['d'] = new char[] { '+', 'D' };
            m_extendedCodes['e'] = new char[] { '+', 'E' };
            m_extendedCodes['f'] = new char[] { '+', 'F' };
            m_extendedCodes['g'] = new char[] { '+', 'G' };
            m_extendedCodes['h'] = new char[] { '+', 'H' };
            m_extendedCodes['i'] = new char[] { '+', 'I' };
            m_extendedCodes['j'] = new char[] { '+', 'J' };
            m_extendedCodes['k'] = new char[] { '+', 'K' };
            m_extendedCodes['l'] = new char[] { '+', 'L' };
            m_extendedCodes['m'] = new char[] { '+', 'M' };
            m_extendedCodes['n'] = new char[] { '+', 'N' };
            m_extendedCodes['o'] = new char[] { '+', 'O' };
            m_extendedCodes['p'] = new char[] { '+', 'P' };
            m_extendedCodes['q'] = new char[] { '+', 'Q' };
            m_extendedCodes['r'] = new char[] { '+', 'R' };
            m_extendedCodes['s'] = new char[] { '+', 'S' };
            m_extendedCodes['t'] = new char[] { '+', 'T' };
            m_extendedCodes['u'] = new char[] { '+', 'U' };
            m_extendedCodes['v'] = new char[] { '+', 'V' };
            m_extendedCodes['w'] = new char[] { '+', 'W' };
            m_extendedCodes['x'] = new char[] { '+', 'X' };
            m_extendedCodes['y'] = new char[] { '+', 'Y' };
            m_extendedCodes['z'] = new char[] { '+', 'Z' };
            m_extendedCodes['{'] = new char[] { '%', 'P' };
            m_extendedCodes['|'] = new char[] { '%', 'Q' };
            m_extendedCodes['}'] = new char[] { '%', 'R' };
            m_extendedCodes['~'] = new char[] { '%', 'S' };
            m_extendedCodes['\x007f'] = new char[] { '%', 'T' };
        }

        /// <summary>
        /// Internal method which retrieves the specified symbol from the symbol table.
        /// </summary>
        /// <param name="checkValue"></param>
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

        /// <summary>
        /// Gets the extended text.
        /// </summary>
        private void GetExtendedText()
        {
            string code = base.Text;
            string extendedCode = "";
            foreach (char c in code)
            {
                char[] extCodes = (char[])m_extendedCodes[c];
                foreach (char exChars in extCodes)
                {
                    extendedCode += exChars.ToString();
                }
            }

            base.ExtendedText = extendedCode;
        }
        #endregion
    }
}
#endif