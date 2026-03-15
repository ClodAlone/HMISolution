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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents an Autofilter in an Excel worksheet.
  /// </summary>
  public interface IAutoFilter
  {
    /// <summary>
    /// First condition of autofilter.
    /// </summary>
    IAutoFilterCondition FirstCondition { get; }
    /// <summary>
    /// Second condition of autofilter.
    /// </summary>
    IAutoFilterCondition SecondCondition { get; }
    /// <summary>
    /// False indicates that this autofilter was not used; otherwise True. Read-only.
    /// </summary>
    bool IsFiltered { get; }
    /// <summary>
    /// True means to use AND operation between conditions,
    /// False to use OR. Read-only.
    /// </summary>
    bool IsAnd { get; set; }
    /// <summary>
    /// True if the Top 10 AutoFilter shows percentage;
    /// False if it shows items. Read-only.
    /// </summary>
    bool IsPercent { get; }
    /// <summary>
    /// True if the first condition is a simple equality. Read-only.
    /// </summary>
    bool IsSimple1 { get; }
    /// <summary>
    /// True if the second condition is a simple equality. Read-only.
    /// </summary>
    bool IsSimple2 { get; }
    /// <summary>
    /// True if the Top 10 AutoFilter shows the top items;
    /// False if it shows the bottom items.
    /// </summary>
    bool IsTop { get; set; }
    /// <summary>
    /// True if the condition is a Top 10 AutoFilter.
    /// </summary>
    bool IsTop10 { get; set; }
    /// <summary>
    /// Number of elements to show in Top10 mode.
    /// </summary>
    int Top10Number { get; set; }
  }
}
