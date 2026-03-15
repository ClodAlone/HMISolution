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

using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the Calculation options in a Excel Workbook.
  /// </summary>
  public interface ICalculationOptions : IParentApplication
  {
    /// <summary>
    /// Specifies the maximum number of times the formulas should be iteratively calculated.
    /// This is a fail-safe against mutually recursive formulas locking up
    /// a spreadsheet application.
    /// </summary>
    int MaximumIteration { get; set; }
    /// <summary>
    /// Defines whether to recalculate before saving.
    /// </summary>
    bool RecalcOnSave { get; set; }
    /// <summary>
    /// Gets / sets maximum change of the result to the exit of an iteration.
    /// </summary>
    double MaximumChange { get; set; }
    /// <summary>
    /// Indicates whether iterations are turned on.
    /// </summary>
    bool IsIterationEnabled { get; set; }
    /// <summary>
    /// Indicates whether R1C1 reference mode is turned on.
    /// </summary>
    bool R1C1ReferenceMode { get; set; }
    /// <summary>
    /// Gets/sets calculation mode in Excel.
    /// </summary>
    ExcelCalculationMode CalculationMode { get; set; }
  }
}
