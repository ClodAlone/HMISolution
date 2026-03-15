#if !SILVERLIGHT
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// The Syncfusion.Pdf.Barcode namespace contains classes for creating barcodes.
/// </summary>
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
namespace Syncfusion.Pdf.Barcode
#endif
{
    /// <summary>
    /// Specifies the barcode text display location.
    /// </summary>
# if !XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Creates a new PdfCode93Barcode.
    /// PdfCode93Barcode code93 = new PdfCode93Barcode();
    /// //Set the barcode text location.
    /// code93.TextDisplayLocation = TextLocation.Bottom;
    /// //Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500));
    /// //Save document to disk.
    /// document.Save("Barcode.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Creates a new PdfCode93Barcode.
    /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode()
    /// 'Set the barcode text location.
    /// code93.TextDisplayLocation = TextLocation.Bottom
    /// 'Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500))
    /// 'Save the document.
    /// document.Save("Barcode.pdf")
    /// </code>
    /// </example> 
    public enum TextLocation
#else
    public enum BarcodeTextLocation
#endif
    {
# if !XAML
        /// <summary>
        /// Displays, no text.
        /// </summary>
        None,
#endif
        /// <summary>
        /// Displays text, above the barcode.
        /// </summary>
        Top,

        /// <summary>
        ///  Displays text, at the bottom of the barcode.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Specifies the barcode text alignment.
    /// </summary>
# if !XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Creates a new PdfCode93Barcode.
    /// PdfCode93Barcode code93 = new PdfCode93Barcode();
    /// //Set the barcode text alignment
    /// code93.TextAlignment = PdfBarcodeTextAlignment.Center;
    /// //Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500));
    /// //Save document to disk.
    /// document.Save("Barcode.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Creates a new PdfCode93Barcode.
    /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode()
    /// 'Set the barcode text alignment
    /// code93.TextAlignment = PdfBarcodeTextAlignment.Center
    /// 'Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500))
    /// 'Save the document.
    /// document.Save("Barcode.pdf")
    /// </code>
    /// </example> 
    public enum PdfBarcodeTextAlignment
#else
    public enum BarcodeTextAlignment
#endif
    {
        /// <summary>
        /// Displays the readable text on the left side of the barcode. 
        /// </summary>
        Left,

        /// <summary>
        /// Displays the readable text at the center of the barcode. 
        /// </summary>
        Center,

        /// <summary>
        ///  Displays the readable text on the right side of the barcode. 
        /// </summary>
        Right
    }

# if !XAML && !GDI
    public enum PdfDataMatrixEncoding
#else
    public enum DataMatrixEncoding
#endif
    {
        /// <summary>
        /// Encoding is choosen based on the data.
        /// </summary>
        Auto,

        /// <summary>
        /// Encoding is done by ASCII encoder.
        /// </summary>
        ASCII,

        /// <summary>
        /// Encoding is done by Numeric encoder.
        /// </summary>
        ASCIINumeric,

        /// <summary>
        /// Encoding is done by Base256 encode.
        /// </summary>
        Base256
    }

# if !XAML && !GDI
    public enum PdfDataMatrixSize
#else
    public enum DataMatrixSize
#endif
    {
        /// <summary>
        /// Size is choosen based on the data.
        /// </summary>
        Auto,

        /// <summary>
        /// Square matrix with 10 rows and 10 columns.
        /// </summary>
        Size10x10,

        /// <summary>
        /// Square matrix with 12 rows and 12 columns.
        /// </summary>
        Size12x12,

        /// <summary>
        /// Square matrix with 14 rows and 14 columns.
        /// </summary>
        Size14x14,

        /// <summary>
        /// Square matrix with 16 rows and 16 columns.
        /// </summary>
        Size16x16,

        /// <summary>
        /// Square matrix with 18 rows and 18 columns.
        /// </summary>
        Size18x18,

        /// <summary>
        /// Square matrix with 20 rows and 20 columns.
        /// </summary>
        Size20x20,

        /// <summary>
        /// Square matrix with 22 rows and 22 columns.
        /// </summary>
        Size22x22,

        /// <summary>
        /// Square matrix with 24 rows and 24 columns.
        /// </summary>
        Size24x24,

        /// <summary>
        /// Square matrix with 26 rows and 26 columns.
        /// </summary>
        Size26x26,

        /// <summary>
        /// Square matrix with 32 rows and 32 columns.
        /// </summary>
        Size32x32,

        /// <summary>
        /// Square matrix with 36 rows and 36 columns.
        /// </summary>
        Size36x36,

        /// <summary>
        /// Square matrix with 40 rows and 40 columns.
        /// </summary>
        Size40x40,

        /// <summary>
        /// Square matrix with 44 rows and 44 columns.
        /// </summary>
        Size44x44,

        /// <summary>
        /// Square matrix with 48 rows and 48 columns.
        /// </summary>
        Size48x48,

        /// <summary>
        /// Square matrix with 52 rows and 52 columns.
        /// </summary>
        Size52x52,

        /// <summary>
        /// Square matrix with 64 rows and 64 columns.
        /// </summary>
        Size64x64,

        /// <summary>
        /// Square matrix with 72 rows and 72 columns.
        /// </summary>
        Size72x72,

        /// <summary>
        /// Square matrix with 80 rows and 80 columns.
        /// </summary>
        Size80x80,

        /// <summary>
        /// Square matrix with 88 rows and 88 columns.
        /// </summary>
        Size88x88,

        /// <summary>
        /// Square matrix with 96 rows and 96 columns.
        /// </summary>
        Size96x96,

        /// <summary>
        /// Square matrix with 104 rows and 104 columns.
        /// </summary>
        Size104x104,

        /// <summary>
        /// Square matrix with 120 rows and 120 columns.
        /// </summary>
        Size120x120,

        /// <summary>
        /// Square matrix with 132 rows and 132 columns.
        /// </summary>
        Size132x132,

        /// <summary>
        /// Square matrix with 144 rows and 144 columns.
        /// </summary>
        Size144x144,

        /// <summary>
        /// Rectangular matrix with 8 rows and 18 columns.
        /// </summary>
        Size8x18,

        /// <summary>
        /// Rectangular matrix with 8 rows and 32 columns.
        /// </summary>
        Size8x32,

        /// <summary>
        /// Rectangular matrix with 12 rows and 26 columns.
        /// </summary>
        Size12x26,

        /// <summary>
        /// Rectangular matrix with 12 rows and 36 columns.
        /// </summary>
        Size12x36,

        /// <summary>
        /// Rectangular matrix with 16 rows and 36 columns.
        /// </summary>
        Size16x36,

        /// <summary>
        /// Rectangular matrix with 16 rows and 48 columns.
        /// </summary>
        Size16x48
    }

    /// <summary>
    /// Specifies the Barcode Version.
    /// </summary>
# if !XAML && !GDI
    public enum QRCodeVersion
#else
    public enum QRBarcodeVersion
#endif
    {
        Auto,
        Version01 = 01,
        Version02 = 02,
        Version03 = 03,
        Version04 = 04,
        Version05 = 05,
        Version06 = 06,
        Version07 = 07,
        Version08 = 08,
        Version09 = 09,
        Version10 = 10,
        Version11 = 11,
        Version12 = 12,
        Version13 = 13,
        Version14 = 14,
        Version15 = 15,
        Version16 = 16,
        Version17 = 17,
        Version18 = 18,
        Version19 = 19,
        Version20 = 20,
        Version21 = 21,
        Version22 = 22,
        Version23 = 23,
        Version24 = 24,
        Version25 = 25,
        Version26 = 26,
        Version27 = 27,
        Version28 = 28,
        Version29 = 29,
        Version30 = 30,
        Version31 = 31,
        Version32 = 32,
        Version33 = 33,
        Version34 = 34,
        Version35 = 35,
        Version36 = 36,
        Version37 = 37,
        Version38 = 38,
        Version39 = 39,
        Version40 = 40,
    }

    /// <summary>
    /// Specifies the Barcode Error correction level.
    /// </summary>
# if !XAML && !GDI
    public enum PdfErrorCorrectionLevel
#else
    public enum ErrorCorrectionLevel
#endif
    {
        /// <summary>
        /// The Recovery capacity is 7%(approx.) 
        /// </summary>
        Low = 7,

        /// <summary>
        /// The Recovery capacity is 15%(approx.) 
        /// </summary>
        Medium = 15,

        /// <summary>
        /// The Recovery capacity is 25%(approx.) 
        /// </summary>
        Quartile = 25,

        /// <summary>
        /// The Recovery capacity is 30%(approx.) 
        /// </summary>
        High = 30
    }

    /// <summary>
    /// Specifies the Barcode Input Mode.
    /// </summary>
# if !XAML && !GDI
    public enum InputMode
#else
    public enum QRInputMode
#endif
    {
        /// <summary>
        /// The Input only contains the Numeric Values(0,1,2,3,4,5,6,7,8,9). 
        /// </summary>
        NumericMode,

        /// <summary>
        /// The Input may contain Numeric Values, Alphabets(Upper case only), SPACE, $, %, *, +, -, ., /, :
        /// </summary>
        AlphaNumericMode,

        /// <summary>
        /// The Input may contain all the ASCII values
        /// </summary>
        BinaryMode

        //KanjiMode
    }
}
#endif