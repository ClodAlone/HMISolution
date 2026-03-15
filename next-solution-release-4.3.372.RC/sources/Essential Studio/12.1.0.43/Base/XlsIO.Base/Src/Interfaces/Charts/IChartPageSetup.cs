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

using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the page setup description. The PageSetup object
  /// contains all page setup attributes (left margin, bottom margin,
  /// paper size, and so on) as properties.
  /// </summary>
  public interface IChartPageSetup
    : IPageSetupBase
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
    /// references in the language of the macro. Read / write String.
    /// </summary>
    string PrintArea { get; set; }
    /// <summary>
    /// Returns or sets the columns that contain the cells to be repeated
    /// on the left side of each page, as a string in A1-style notation
    /// in the language of the macro. Read / write String.
    /// </summary>
    string PrintTitleColumns { get; set; }
    /// <summary>
    /// Returns or sets the rows that contain the cells to be repeated at
    /// the top of each page, as a string in A1-style notation in the
    /// language of the macro. Read / write String.
    /// </summary>
    string PrintTitleRows { get; set; }
*/
#endif
    #endregion

    #region interface properties
    /// <summary>
    /// Returns or sets the number of pages tall the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read / write Boolean.
    /// </summary>
    bool FitToPagesTall { get; set; }
    /// <summary>
    /// Returns or sets the number of pages wide the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read / write Boolean.
    /// </summary>
    bool FitToPagesWide { get; set; }
    #endregion
  }
}
