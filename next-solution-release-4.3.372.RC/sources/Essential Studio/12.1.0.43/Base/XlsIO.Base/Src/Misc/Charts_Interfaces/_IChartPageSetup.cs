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
using System.Collections;
#endregion

namespace Syncfusion.XlsIO.Interfaces.Charts
{
  /// <summary>
  /// Represents the page setup description. The PageSetup object
  /// contains all page setup attributes (left margin, bottom margin,
  /// paper size, and so on) as properties.
  /// </summary>
  public interface IChartPageSetup
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
    Graphic CenterFooterPicture { get; }
    Graphic CenterHeaderPicture { get; }
    Graphic LeftFooterPicture { get; }
    Graphic LeftHeaderPicture { get; }
    Graphic RightFooterPicture { get; }
    Graphic RightHeaderPicture { get; }
    XlObjectSize ChartSize { get; set; }
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns or sets the range to be printed, as a string using A1-style
    /// references in the language of the macro. Read/write String.
    /// </summary>
    string PrintArea { get; set; }
    /// <summary>
    /// Returns or sets the columns that contain the cells to be repeated
    /// on the left side of each page, as a string in A1-style notation
    /// in the language of the macro. Read/write String.
    /// </summary>
    string PrintTitleColumns { get; set; }
    /// <summary>
    /// Returns or sets the rows that contain the cells to be repeated at
    /// the top of each page, as a string in A1-style notation in the
    /// language of the macro. Read/write String.
    /// </summary>
    string PrintTitleRows { get; set; }
*/
#endif
    #endregion

    #region interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an Application
    /// object that represents the Excel application.
    /// </summary>
    IApplication Application { get; }
    /// <summary>
    /// True if elements of the document will be printed in black and white.
    /// Read/write Boolean.
    /// </summary>
    bool BlackAndWhite { get; set; }
    /// <summary>
    /// Returns or sets the size of the bottom margin, in points.
    /// Read/write Double
    /// </summary>
    double BottomMargin { get; set; }
    /// <summary>
    /// Returns or sets the center part of the footer. Read/write String
    /// </summary>
    string CenterFooter { get; set; }
    /// <summary>
    /// Returns or sets the center part of the header. Read/write String
    /// </summary>
    string CenterHeader { get; set; }
    /// <summary>
    /// True if the sheet is centered horizontally on the page when it's
    /// printed. Read/write Boolean.
    /// </summary>
    bool CenterHorizontally { get; set; }
    /// <summary>
    /// True if the sheet is centered vertically on the page when it's
    /// printed. Read/write Boolean.
    /// </summary>
    bool CenterVertically { get; set; }
    /// <summary>
    /// Number of copies to print
    /// </summary>
    int  Copies { get; set; }
    /// <summary>
    /// True if the sheet will be printed without graphics.
    /// Read/write Boolean.
    /// </summary>
    bool Draft { get; set; }
    /// <summary>
    /// Returns or sets the first page number that will be used when
    /// this sheet is printed. If xlAutomatic, Microsoft Excel chooses the
    /// first page number. The default is xlAutomatic. Read/write Long.
    /// </summary>
    int FirstPageNumber { get; set; }
    /// <summary>
    /// Returns or sets the number of pages tall the worksheet will be scaled
    /// to when it's printed. Applies only to worksheets. Read/write Boolean.
    /// </summary>
    bool FitToPagesTall { get; set; }
    /// <summary>
    /// Returns or sets the number of pages wide the worksheet will be scaled
    /// to when it's printed. Applies only to worksheets. Read/write Boolean.
    /// </summary>
    bool FitToPagesWide { get; set; }
    /// <summary>
    /// Returns or sets the distance from the bottom of the page to the footer,
    /// in points. Read/write Double.
    /// </summary>
    double FooterMargin { get; set; }
    /// <summary>
    /// Returns or sets the distance from the top of the page to the header,
    /// in points. Read/write Double.
    /// </summary>
    double HeaderMargin { get; set; }
    /// <summary>
    /// Returns or sets the left part of the footer. Read/write String.
    /// &L Left aligns the characters that follow.
    /// &C Centers the characters that follow.
    /// &R Right aligns the characters that follow.
    /// &E Turns double-underline printing on or off.
    /// &X Turns superscript printing on or off.
    /// &Y Turns subscript printing on or off.
    /// &B Turns bold printing on or off.
    /// &I Turns italic printing on or off.
    /// &U Turns underline printing on or off.
    /// &S Turns strikethrough printing on or off.
    /// &D Prints the current date.
    /// &T Prints the current time.
    /// &F Prints the name of the document.
    /// &A Prints the name of the workbook tab.
    /// &P Prints the page number.
    /// &P+number Prints the page number plus the specified number.
    /// &P-number Prints the page number minus the specified number.
    /// && Prints a single ampersand.
    /// & "fontname" Prints the characters that follow in the specified font. Be sure to include the double quotation marks.
    /// &nn Prints the characters that follow in the specified font size. Use a two-digit number to specify a size in points.
    /// &N Prints the total number of pages in the document.
    /// </summary>
    string LeftFooter { get; set; }
    /// <summary>
    /// Returns or sets the left part of the header. Read/write String.
    /// </summary>
    string LeftHeader { get; set; }
    /// <summary>
    /// Returns or sets the size of the left margin, in points.
    /// Read/write Double.
    /// </summary>
    double LeftMargin { get; set; }
    /// <summary>
    /// Returns or sets the order that Microsoft Excel uses to number
    /// pages when printing a large worksheet. Read/write ExcelOrder
    /// </summary>
    ExcelOrder Order { get; set; }
    /// <summary>
    /// Portrait or landscape printing mode. Read/write ExcelPageOrientation
    /// </summary>
    ExcelPageOrientation Orientation { get; set; }
    /// <summary>
    /// Returns or sets the size of the paper. Read/write ExcelPaperSize
    /// </summary>
    ExcelPaperSize PaperSize { get; set; }
    /// <summary>
    /// Returns the parent object for the specified object
    /// </summary>
    object Parent { get; }
    /// <summary>
    /// Returns or sets the way comments are printed with the sheet.
    /// Read/write ExcelPrintLocation.
    /// </summary>
    ExcelPrintLocation PrintComments { get; set; }
    /// <summary>
    /// Sets or returns an ExcelPrintErrors contstant specifying the type of
    /// print error displayed. This feature allows users to suppress the
    /// display of error values when printing a worksheet. Read/write.
    /// </summary>
    ExcelPrintErrors PrintErrors { get; set; }
    /// <summary>
    /// True if cell notes are printed as end notes with the sheet. Applies
    /// only to worksheets. Read/write Boolean.
    /// </summary>
    bool PrintNotes { get; set; }
    /// <summary>
    /// Returns or sets the print quality. Read/write ushort
    /// </summary>
    int  PrintQuality { get; set; }
    /// <summary>
    /// Returns or sets the right part of the footer. Read/write String
    /// </summary>
    string RightFooter { get; set; }
    /// <summary>
    /// Returns or sets the right part of the header. Read/write String
    /// </summary>
    string RightHeader { get; set; }
    /// <summary>
    /// Returns or sets the size of the right margin, in points.
    /// Read/write Double.
    /// </summary>
    double RightMargin { get; set; }
    /// <summary>
    /// Returns or sets the size of the top margin, in points.
    /// Read/write Double.
    /// </summary>
    double TopMargin { get; set; }
    /// <summary>
    /// Returns or sets a percentage (between 10 and 400 percent) by which
    /// Excel will scale the worksheet for printing. Applies only
    /// to worksheets. Read/write ushort.
    /// </summary>
    int  Zoom { get; set; }
    #endregion
  }
}
