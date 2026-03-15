#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif
#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Base interface for all page setups.
  /// </summary>
  public interface IPageSetupBase : IParentApplication
  {
    /// <summary>
    /// Indicates whether FirstPageNumber is set to Auto or not.
    /// </summary>
    bool AutoFirstPageNumber { get; set; }
    /// <summary>
    /// True if elements of the document will be printed in black and white.
    /// Read / write Boolean.
    /// </summary>
    bool    BlackAndWhite { get; set; }
    /// <summary>
    /// Returns or sets the size of the bottom margin, in inches.
    /// Read / write Double.
    /// </summary>
    double  BottomMargin { get; set; }
    /// <summary>
    /// Returns or sets the center part of the footer. Read / write String.
    /// </summary>
    string  CenterFooter { get; set; }
    /// <summary>
    /// Gets / set image for center part of the footer.
    /// </summary>
    Image CenterFooterImage { get; set; }
    /// <summary>
    /// Gets / set image for center part of the header.
    /// </summary>
    Image CenterHeaderImage { get; set; }
    /// <summary>
    /// Returns or sets the center part of the header. Read / write String.
    /// </summary>
    string  CenterHeader { get; set; }
    /// <summary>
    /// True if the sheet is centered horizontally on the page when it is
    /// printed. Read / write Boolean.
    /// </summary>
    bool    CenterHorizontally { get; set; }
    /// <summary>
    /// True if the sheet is centered vertically on the page when it is
    /// printed. Read / write Boolean.
    /// </summary>
    bool    CenterVertically { get; set; }
    /// <summary>
    /// Number of copies to print.
    /// </summary>
    int     Copies { get; set; }
    /// <summary>
    /// True if the sheet will be printed without graphics.
    /// Read / write Boolean.
    /// </summary>
    bool    Draft { get; set; }
    /// <summary>
    /// Returns or sets the first page number that will be used when
    /// this sheet is printed. If xlAutomatic, Microsoft Excel chooses the
    /// first page number. The default is xlAutomatic. Read / write Long.
    /// </summary>
    short     FirstPageNumber { get; set; }
    /// <summary>
    /// Returns or sets the distance from the bottom of the page to the footer,
    /// in inches. Read / write Double.
    /// </summary>
    double  FooterMargin { get; set; }
    /// <summary>
    /// Returns or sets the distance from the top of the page to the header,
    /// in inches. Read / write Double.
    /// </summary>
    double  HeaderMargin { get; set; }
    /// <summary>
    /// Returns or sets the left part of the footer. Read / write String.
    /// &amp;L Left aligns the characters that follow.
    /// &amp;C Centers the characters that follow.
    /// &amp;R Right aligns the characters that follow.
    /// &amp;E Turns double-underline printing on or off.
    /// &amp;X Turns superscript printing on or off.
    /// &amp;Y Turns subscript printing on or off.
    /// &amp;B Turns bold printing on or off.
    /// &amp;I Turns italic printing on or off.
    /// &amp;U Turns underline printing on or off.
    /// &amp;S Turns strikethrough printing on or off.
    /// &amp;D Prints the current date.
    /// &amp;T Prints the current time.
    /// &amp;F Prints the name of the document.
    /// &amp;A Prints the name of the workbook tab.
    /// &amp;P Prints the page number.
    /// &amp;P+number Prints the page number plus the specified number.
    /// &amp;P-number Prints the page number minus the specified number.
    /// &amp;&amp; Prints a single ampersand.
    /// &amp; "fontname" Prints the characters that follow in the specified font. Be sure to include the double quotation marks.
    /// &amp;nn Prints the characters that follow in the specified font size. Use a two-digit number to specify a size in points.
    /// &amp;N Prints the total number of pages in the document.
    /// </summary>
    string  LeftFooter { get; set; }
    /// <summary>
    /// Gets / set image for left part of the footer.
    /// </summary>
    Image LeftFooterImage { get; set; }
    /// <summary>
    /// Gets / set image for left part of the header.
    /// </summary>
    Image LeftHeaderImage { get; set; }
    /// <summary>
    /// Returns or sets the left part of the header. Read / write String.
    /// </summary>
    string  LeftHeader { get; set; }
    /// <summary>
    /// Returns or sets the size of the left margin, in inches.
    /// Read / write Double.
    /// </summary>
    double  LeftMargin { get; set; }
    /// <summary>
    /// Returns or sets the order that Microsoft Excel uses to number
    /// pages when printing a large worksheet. Read / write ExcelOrder.
    /// </summary>
    ExcelOrder Order { get; set; }
    /// <summary>
    /// Portrait or landscape printing mode. Read / write ExcelPageOrientation.
    /// </summary>
    ExcelPageOrientation Orientation { get; set; }
    /// <summary>
    /// Returns or sets the size of the paper. Read / write ExcelPaperSize.
    /// </summary>
    ExcelPaperSize PaperSize { get; set; }
    /// <summary>
    /// Returns or sets the way comments are printed with the sheet.
    /// Read / write ExcelPrintLocation.
    /// </summary>
    ExcelPrintLocation PrintComments { get; set; }
    /// <summary>
    /// Sets or returns an ExcelPrintErrors constant specifying the type of
    /// print error displayed. This feature allows users to suppress the
    /// display of error values when printing a worksheet. Read / write.
    /// </summary>
    ExcelPrintErrors PrintErrors { get; set; }
    /// <summary>
    /// True if cell notes are printed as end notes with the sheet. Applies
    /// only to worksheets. Read / write Boolean.
    /// </summary>
    bool    PrintNotes { get; set; }
    /// <summary>
    /// Returns or sets the print quality in the dpi. Read / write ushort.
    /// </summary>
    int     PrintQuality { get; set; }
    /// <summary>
    /// Returns or sets the right part of the footer. Read / write String.
    /// </summary>
    string  RightFooter { get; set; }
    /// <summary>
    /// Gets / set image for right part of the footer.
    /// </summary>
    Image RightFooterImage { get; set; }
    /// <summary>
    /// Gets / set image for right part of the header.
    /// </summary>
    Image RightHeaderImage { get; set; }
    /// <summary>
    /// Returns or sets the right part of the header. Read / write String.
    /// </summary>
    string  RightHeader { get; set; }
    /// <summary>
    /// Returns or sets the size of the right margin, in inches.
    /// Read / write Double.
    /// </summary>
    double  RightMargin { get; set; }
    /// <summary>
    /// Returns or sets the size of the top margin, in inches.
    /// Read / write Double.
    /// </summary>
    double  TopMargin { get; set; }
    /// <summary>
    /// Returns or sets a percentage (between 10 and 400 percent) by which
    /// Microsoft Excel will scale the worksheet for printing. Applies only
    /// to worksheets. Read / write ushort.
    /// </summary>
    int     Zoom { get; set; }
    /// <summary>
    /// Gets / sets the header and footer margins are aligned with page margins.
    /// </summary>
    bool AlignHFWithPageMargins { get; set; }
    /// <summary>
    /// Gets / sets the header and footer of the first page is different with other pages.
    /// </summary>
    bool DifferentFirstPageHF { get; set; }
    /// <summary>
    /// Gets / sets the header and footer odd pages are differed with even page.
    /// </summary>
    bool DifferentOddAndEvenPagesHF { get; set; }
    /// <summary>
    /// Gets / sets the header and footer are scaled with document scaling.
    /// </summary>
    bool HFScaleWithDoc { get; set; }

    /// <summary>
    /// Gets / sets background image.
    /// </summary>
#if !SILVERLIGHT && !WINRT && !WP
    Bitmap
#else
    Image
#endif
      BackgoundImage { get; set; }
  }
}
