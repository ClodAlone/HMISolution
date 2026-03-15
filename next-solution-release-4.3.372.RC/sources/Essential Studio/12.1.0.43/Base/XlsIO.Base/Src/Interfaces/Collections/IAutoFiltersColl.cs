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
  /// Represents a collection of Autofilters.
  /// </summary>
  public interface IAutoFilters : IParentApplication
  {
    /// <summary>
    /// Range to be filtered.
    /// </summary>
    IRange FilterRange { get; set; }
    /// <summary>
    /// Number of columns to be filtered. Read-only.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single autofilter object by column index. Read-only.
    /// </summary>
    IAutoFilter this[ int columnIndex ] { get; }
  }
}
