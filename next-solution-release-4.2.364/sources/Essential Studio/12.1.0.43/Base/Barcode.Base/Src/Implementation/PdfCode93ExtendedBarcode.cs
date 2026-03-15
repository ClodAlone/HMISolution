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
    /// Represents a code93 extended barcode.
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
    /// //Creates a new PdfCode93ExtendedBarcode.
    /// PdfCode93ExtendedBarcode code93 = new PdfCode93ExtendedBarcode();
    /// //Set the font to code93Ext.
    /// code93Ext.Font = font;
    /// //Set the barcode text.
    /// code93Ext.Text = "CODE39Ext";
    /// //Draw a barcode in the new Page.
    /// code93Ext.Draw(page, new PointF(25, 500));
    /// //Save the  document to disk.
    /// document.Save("Code93Ext.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Creates a new PdfCode93ExtendedBarcode.
    /// Dim code93Ext As PdfCode93ExtendedBarcode = New PdfCode93ExtendedBarcode()
    /// 'Set the font to code93Ext.
    /// code93Ext.Font = font 
    /// 'Set the barcode text.
    /// code93Ext.Text = "CODE39Ext"
    /// 'Draw a barcode in the new Page.
    /// code93Ext.Draw(page, new PointF(25, 500))
    /// 'Save the  document to disk.
    /// document.Save("Code93Ext.pdf")
    /// </code>
    /// </example> 
    /// <remarks> All 128 ASCII characters can be encoded in an extended Code 93 barcode. </remarks>
    /// <seealso cref="PdfCodabarBarcode"/> Class
    /// <seealso cref="PdfCode11Barcode"/> Class
    /// <seealso cref="PdfCode128ABarcode"/> Class
    /// <seealso cref="PdfCode128BBarcode"/> Class
    /// <seealso cref="PdfCode128CBarcode"/> Class
    /// <seealso cref="PdfCode32Barcode"/> Class
    /// <seealso cref="PdfCode39Barcode"/> Class
    /// <seealso cref="PdfCode39ExtendedBarcode"/> Class
    /// <seealso cref="PdfCode93Barcode"/> Class
    public class PdfCode93ExtendedBarcode : PdfCode93Barcode
#else
    public class Code93ExtendedBarcode : Code93Barcode
#endif
    {
        #region Fields
        private Dictionary<char, char[]> m_extendedCodes;
        #endregion

        #region Constructor

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode93ExtendedBarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode93ExtendedBarcode.
        /// PdfCode93ExtendedBarcode code93 = new PdfCode93ExtendedBarcode();
        /// //Set the font to code93Ext.
        /// code93Ext.Font = font;
        /// //Set the barcode text.
        /// code93Ext.Text = "CODE39Ext";
        /// //Draw a barcode in the new Page.
        /// code93Ext.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93Ext.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode93ExtendedBarcode.
        /// Dim code93Ext As PdfCode93ExtendedBarcode = New PdfCode93ExtendedBarcode()
        /// 'Set the font to code93Ext.
        /// code93Ext.Font = font 
        /// 'Set the barcode text.
        /// code93Ext.Text = "CODE39Ext"
        /// 'Draw a barcode in the new Page.
        /// code93Ext.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code93Ext.pdf")
        /// </code>
        /// </example> 
        public PdfCode93ExtendedBarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code93ExtendedBarcode"/> class.
        /// </summary>
        public Code93ExtendedBarcode()
#endif

            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCode93ExtendedBarcode"/> class.
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
        /// //Creates a new PdfCode93ExtendedBarcode.
        /// PdfCode93ExtendedBarcode code93 = new PdfCode93ExtendedBarcode("Code93Ext");
        /// //Set the font to code93Ext.
        /// code93Ext.Font = font;
        /// //Draw a barcode in the new Page.
        /// code93Ext.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93Ext.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode93ExtendedBarcode.
        /// Dim code93Ext As PdfCode93ExtendedBarcode = New PdfCode93ExtendedBarcode("Code93Ext")
        /// 'Set the font to code93Ext.
        /// code93Ext.Font = font 
        /// 'Draw a barcode in the new Page.
        /// code93Ext.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code93Ext.pdf")
        /// </code>
        /// </example> 
        public PdfCode93ExtendedBarcode(string text)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Code93ExtendedBarcode"/> class.
        /// </summary>
        /// <param name="text">The Barcode text.</param>
        public Code93ExtendedBarcode(string text)
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
            GetExtendedText();
            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text : base.ExtendedText;
            foreach (char c in code)
            {
                BarcodeSymbolTable pattern = base.BarcodeSymbols[c] as BarcodeSymbolTable;
                checkValue += pattern.CheckDigit;
            }

            char[] ch = new char[1];
            ch = this.GetCheckSumSymbols();
            return ch;
        }

        /// <summary>
        /// To get the Checksum value
        /// </summary>
        /// <returns>checksum symbols</returns>
        protected internal char[] GetCheckSumSymbols()
        {
            string text = this.ExtendedText;
            char[] charArray = new char[2];
            int checkValue = 0;
            string dataToEncode = text;
            int length = dataToEncode.Length;

            for (int i = 0; i < length; i++)
            {
                int num4 = (length - i) % 20;
                if (num4 == 0)
                {
                    num4 = 20;
                }

                int numi = (BarcodeSymbols[dataToEncode[i]] as BarcodeSymbolTable).CheckDigit;
                checkValue += numi * num4;
            }

            checkValue = checkValue % 0x2f;

            char char1 = ' ';
            foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.CheckDigit == checkValue)
                {
                    char1 = pattern.Symbol;
                    break;
                }
            }

            string data = this.ExtendedText;

            data = data + char1;
            charArray[0] = char1;
            text = data;
            checkValue = 0;
            dataToEncode = text;
            length = dataToEncode.Length;

            for (int i = 0; i < length; i++)
            {
                int num4 = (length - i) % 15;
                if (num4 == 0)
                {
                    num4 = 15;
                }

                int tempi = (BarcodeSymbols[dataToEncode[i]] as BarcodeSymbolTable).CheckDigit;
                checkValue += tempi * num4;
            }

            checkValue = checkValue % 0x2f;

            text = text + checkValue;

            char char2 = ' ';
            foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.CheckDigit == checkValue)
                {
                    char2 = pattern.Symbol;
                    break;
                }
            }

            data = data + char2;
            charArray[1] = char2;

            return charArray;
        }

        /// <summary>
        /// Initializes the internal barcode symbol table
        /// </summary>
        private void Initialize()
        {
            base.Initialize();

            base.ValidatorExpression = @"^[\x00-\x7F\x00fb\x00fd\x00fe\'�'\'�'\'�']+$";

            m_extendedCodes = new Dictionary<char, char[]>();

            m_extendedCodes['\0'] = new char[] { '\x00fc', 'U' };
            m_extendedCodes['\x0001'] = new char[] { '\x00fb', 'A' };
            m_extendedCodes['\x0002'] = new char[] { '\x00fb', 'B' };
            m_extendedCodes['\x0003'] = new char[] { '\x00fb', 'C' };
            m_extendedCodes['\x0004'] = new char[] { '\x00fb', 'D' };
            m_extendedCodes['\x0005'] = new char[] { '\x00fb', 'E' };
            m_extendedCodes['\x0006'] = new char[] { '\x00fb', 'F' };
            m_extendedCodes['\a'] = new char[] { '\x00fb', 'G' };
            m_extendedCodes['\b'] = new char[] { '\x00fb', 'H' };
            m_extendedCodes['\t'] = new char[] { '\x00fb', 'I' };
            m_extendedCodes['\n'] = new char[] { '\x00fb', 'J' };
            m_extendedCodes['\v'] = new char[] { '\x00fb', 'K' };
            m_extendedCodes['\f'] = new char[] { '\x00fb', 'L' };
            m_extendedCodes['\r'] = new char[] { '\x00fb', 'M' };
            m_extendedCodes['\x000e'] = new char[] { '\x00fb', 'N' };
            m_extendedCodes['\x000f'] = new char[] { '\x00fb', 'O' };
            m_extendedCodes['\x0010'] = new char[] { '\x00fb', 'P' };
            m_extendedCodes['\x0011'] = new char[] { '\x00fb', 'Q' };
            m_extendedCodes['\x0012'] = new char[] { '\x00fb', 'R' };
            m_extendedCodes['\x0013'] = new char[] { '\x00fb', 'S' };
            m_extendedCodes['\x0014'] = new char[] { '\x00fb', 'T' };
            m_extendedCodes['\x0015'] = new char[] { '\x00fb', 'U' };
            m_extendedCodes['\x0016'] = new char[] { '\x00fb', 'V' };
            m_extendedCodes['\x0017'] = new char[] { '\x00fb', 'W' };
            m_extendedCodes['\x0018'] = new char[] { '\x00fb', 'X' };
            m_extendedCodes['\x0019'] = new char[] { '\x00fb', 'Y' };
            m_extendedCodes['\x001a'] = new char[] { '\x00fb', 'Z' };
            m_extendedCodes['\x001b'] = new char[] { '\x00fc', 'A' };
            m_extendedCodes['\x001c'] = new char[] { '\x00fc', 'B' };
            m_extendedCodes['\x001d'] = new char[] { '\x00fc', 'C' };
            m_extendedCodes['\x001e'] = new char[] { '\x00fc', 'D' };
            m_extendedCodes['\x001f'] = new char[] { '\x00fc', 'E' };
            m_extendedCodes[' '] = new char[] { ' ' };
            m_extendedCodes['!'] = new char[] { '\x00fd', 'A' };
            m_extendedCodes['"'] = new char[] { '\x00fd', 'B' };
            m_extendedCodes['#'] = new char[] { '\x00fd', 'C' };
            m_extendedCodes['$'] = new char[] { '\x00fd', 'D' };
            m_extendedCodes['%'] = new char[] { '\x00fd', 'E' };
            m_extendedCodes['&'] = new char[] { '\x00fd', 'F' };
            m_extendedCodes['\''] = new char[] { '\x00fd', 'G' };
            m_extendedCodes['('] = new char[] { '\x00fd', 'H' };
            m_extendedCodes[')'] = new char[] { '\x00fd', 'I' };
            m_extendedCodes['*'] = new char[] { '\x00fd', 'J' };
            m_extendedCodes['+'] = new char[] { '\x00fd', 'K' };
            m_extendedCodes[','] = new char[] { '\x00fd', 'L' };
            m_extendedCodes['-'] = new char[] { '\x00fd', 'M' };
            m_extendedCodes['.'] = new char[] { '\x00fd', 'N' };
            m_extendedCodes['/'] = new char[] { '\x00fd', 'O' };
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
            m_extendedCodes[':'] = new char[] { '\x00fd', 'Z' };
            m_extendedCodes[';'] = new char[] { '\x00fc', 'F' };
            m_extendedCodes['<'] = new char[] { '\x00fc', 'G' };
            m_extendedCodes['='] = new char[] { '\x00fc', 'H' };
            m_extendedCodes['>'] = new char[] { '\x00fc', 'I' };
            m_extendedCodes['?'] = new char[] { '\x00fc', 'J' };
            m_extendedCodes['@'] = new char[] { '\x00fc', 'V' };
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
            m_extendedCodes['['] = new char[] { '\x00fc', 'K' };
            m_extendedCodes['\\'] = new char[] { '\x00fc', 'L' };
            m_extendedCodes[']'] = new char[] { '\x00fc', 'M' };
            m_extendedCodes['^'] = new char[] { '\x00fc', 'N' };
            m_extendedCodes['_'] = new char[] { '\x00fc', 'O' };
            m_extendedCodes['`'] = new char[] { '\x00fc', 'W' };
            m_extendedCodes['a'] = new char[] { '\x00fe', 'A' };
            m_extendedCodes['b'] = new char[] { '\x00fe', 'B' };
            m_extendedCodes['c'] = new char[] { '\x00fe', 'C' };
            m_extendedCodes['d'] = new char[] { '\x00fe', 'D' };
            m_extendedCodes['e'] = new char[] { '\x00fe', 'E' };
            m_extendedCodes['f'] = new char[] { '\x00fe', 'F' };
            m_extendedCodes['g'] = new char[] { '\x00fe', 'G' };
            m_extendedCodes['h'] = new char[] { '\x00fe', 'H' };
            m_extendedCodes['i'] = new char[] { '\x00fe', 'I' };
            m_extendedCodes['j'] = new char[] { '\x00fe', 'J' };
            m_extendedCodes['k'] = new char[] { '\x00fe', 'K' };
            m_extendedCodes['l'] = new char[] { '\x00fe', 'L' };
            m_extendedCodes['m'] = new char[] { '\x00fe', 'M' };
            m_extendedCodes['n'] = new char[] { '\x00fe', 'N' };
            m_extendedCodes['o'] = new char[] { '\x00fe', 'O' };
            m_extendedCodes['p'] = new char[] { '\x00fe', 'P' };
            m_extendedCodes['q'] = new char[] { '\x00fe', 'Q' };
            m_extendedCodes['r'] = new char[] { '\x00fe', 'R' };
            m_extendedCodes['s'] = new char[] { '\x00fe', 'S' };
            m_extendedCodes['t'] = new char[] { '\x00fe', 'T' };
            m_extendedCodes['u'] = new char[] { '\x00fe', 'U' };
            m_extendedCodes['v'] = new char[] { '\x00fe', 'V' };
            m_extendedCodes['w'] = new char[] { '\x00fe', 'W' };
            m_extendedCodes['x'] = new char[] { '\x00fe', 'X' };
            m_extendedCodes['y'] = new char[] { '\x00fe', 'Y' };
            m_extendedCodes['z'] = new char[] { '\x00fe', 'Z' };
            m_extendedCodes['{'] = new char[] { '\x00fc', 'P' };
            m_extendedCodes['|'] = new char[] { '\x00fc', 'Q' };
            m_extendedCodes['}'] = new char[] { '\x00fc', 'R' };
            m_extendedCodes['~'] = new char[] { '\x00fc', 'S' };
            m_extendedCodes['\x007f'] = new char[] { '\x00fc', 'T' };
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

        /// <summary>
        /// To get the Extended Text.
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