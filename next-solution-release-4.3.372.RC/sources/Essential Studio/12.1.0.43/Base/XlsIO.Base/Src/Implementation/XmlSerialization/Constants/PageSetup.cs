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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
  /// <summary>
  /// This class stores all constants connected with page setup settings.
  /// </summary>
  public sealed class PageSetup
  {
    #region Constants
    /// <summary>
    /// Print options for the sheet.
    /// </summary>
    public const string PrintOptionsTag = "printOptions";
    /// <summary>
    /// Used in conjunction with gridLinesSet. If both gridLines and gridlinesSet are true, then
    /// grid lines shall print. Otherwise, they shall not (i.e., one or both have false values).
    /// </summary>
    public const string GridLines = "gridLines";
    /// <summary>
    /// Used in conjunction with gridLines. If both gridLines and gridLinesSet are true, then
    /// grid lines shall print. Otherwise, they shall not (i.e., one or both have false values).
    /// </summary>
    public const string GridLinesSet = "gridLinesSet";
    /// <summary>
    /// Print row and column headings.
    /// </summary>
    public const string Headings = "headings";
    /// <summary>
    /// Center on page horizontally when printing.
    /// </summary>
    public const string HorizontalCentered = "horizontalCentered";
    /// <summary>
    /// Center on page vertically when printing.
    /// </summary>
    public const string VerticalCentered = "verticalCentered";
    /// <summary>
    /// Page margins for a sheet or a custom sheet view.
    /// </summary>
    public const string PageMarginsTag = "pageMargins";
    /// <summary>
    /// Bottom Page Margin in inches.
    /// </summary>
    public const string BottomMargin = "bottom";
    /// <summary>
    /// Footer Page Margin in inches.
    /// </summary>
    public const string FooterMargin = "footer";
    /// <summary>
    /// Header Page Margin in inches.
    /// </summary>
    public const string HeaderMargin = "header";
    /// <summary>
    /// Left Page Margin in inches.
    /// </summary>
    public const string LeftMargin = "left";
    /// <summary>
    /// Right Page Margin in inches.
    /// </summary>
    public const string RightMargin = "right";
    /// <summary>
    /// Top Page Margin in inches.
    /// </summary>
    public const string TopMargin = "top";
    /// <summary>
    /// Page setup settings for the worksheet.
    /// </summary>
    public const string PageSetupTag = "pageSetup";
    /// <summary>
    /// Print black and white.
    /// </summary>
    public const string BlackAndWhite = "blackAndWhite";
    /// <summary>
    /// This attribute specifies how to print cell comments.
    /// </summary>
    public const string CellComments = "cellComments";
    /// <summary>
    /// Number of copies to print.
    /// </summary>
    public const string Copies = "copies";
    /// <summary>
    /// Print without graphics.
    /// </summary>
    public const string Draft = "draft";
    /// <summary>
    /// Specifies how to print cell values for cells with errors.
    /// </summary>
    public const string Errors = "errors";
    /// <summary>
    /// Page number for first printed page. If no value is specified, then 'automatic' is assumed.
    /// </summary>
    public const string FirstPageNumber = "firstPageNumber";
    /// <summary>
    /// Number of vertical pages to fit on.
    /// </summary>
    public const string FitToHeight = "fitToHeight";
    /// <summary>
    /// Number of horizontal pages to fit on.
    /// </summary>
    public const string FitToWidth = "fitToWidth";
    /// <summary>
    /// Horizontal print resolution of the device.
    /// </summary>
    public const string HorizontalDpi = "horizontalDpi";
    /// <summary>
    /// Relationship Id of the devMode printer settings part.
    /// </summary>
    public const string Id = "id";
    /// <summary>
    /// Orientation of the page.
    /// </summary>
    public const string Orientation = "orientation";
    /// <summary>
    /// Order of printed pages.
    /// </summary>
    public const string PageOrder = "pageOrder";
    /// <summary>
    /// Paper size
    /// </summary>
    public const string PaperSize = "paperSize";
    /// <summary>
    /// Print scaling. Valid values range from 10 to 400.
    /// </summary>
    public const string Scale = "scale";
    /// <summary>
    /// Use firstPageNumber value for first page number, and do not auto number the pages.
    /// </summary>
    public const string UseFirstPageNumber = "useFirstPageNumber";
    /// <summary>
    /// Use the printer�s defaults settings for page setup values and don't use the default values
    /// specified in the schema. For example, if dpi is not present or specified in the XML, the
    /// application shall not assume 600dpi as specified in the schema as a default and instead
    /// shall let the printer specify the default dpi.
    /// </summary>
    public const string UsePrinterDefaults = "usePrinterDefaults";
    /// <summary>
    /// Vertical print resolution of the device.
    /// </summary>
    public const string VerticalDpi = "verticalDpi";
    /// <summary>
    /// Print cell comments as displayed.
    /// </summary>
    public const string CommentAsDisplayed = "asDisplayed";
    /// <summary>
    /// Do not print cell comments.
    /// </summary>
    public const string CommentNone = "none";
    /// <summary>
    /// Print cell comments at end of document.
    /// </summary>
    public const string CommentAtEnd = "atEnd";
    /// <summary>
    /// Display cell errors as blank.
    /// </summary>
    public const string ErrorsBlank = "blank";
    /// <summary>
    /// Display cell errors as dashes.
    /// </summary>
    public const string ErrorsDash = "dash";
    /// <summary>
    /// Display cell errors as displayed on screen.
    /// </summary>
    public const string ErrorsDisplayed = "displayed";
    /// <summary>
    /// Display cell errors as #N/A.
    /// </summary>
    public const string ErrorsNA = "NA";
    /// <summary>
    /// Header and footer settings.
    /// </summary>
    public const string HeaderFooterTag = "headerFooter";
    /// <summary>
    /// Odd header string.
    /// </summary>
    public const string OddHeaderTag = "oddHeader";
    /// <summary>
    /// Odd footer string.
    /// </summary>
    public const string OddFooterTag = "oddFooter";
    /// <summary>
    /// Header Footer Scales with Document string
    /// </summary>
    public const string ScaleWithDocTag = "scaleWithDoc";
    /// <summary>
    /// Header Footer Margins align with Document string
    /// </summary>
    public const string AlignWithMarginsTag = "alignWithMargins";
    /// <summary>
    /// Header Footer Odd/Even pages is different with the document string
    /// </summary>
    public const string DifferentOddEvenTag = "differentOddEven";
    /// <summary>
    /// Header Footer first page is different with document string
    /// </summary>
    public const string DifferentFirst = "differentFirst";

    #endregion

    #region Constructors
    /// <summary>
    /// Prevents a default instance of the PageSetup class from being created.
    /// </summary>
    private PageSetup()
    {
    }
    #endregion
  }
}
