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

namespace Syncfusion.XlsIO.Interfaces.Charts
{
  /// <summary>
  /// Summary description for IChartErrorBars.
  /// </summary>
  public interface IChartErrorBars
  {
    bool IsFixedValue { get; set; }
    bool IsPercentage { get; set; }
    bool IsStandardDeviation { get; set; }
    bool IsStandardError { get; set; }
    bool IsCustom { get; set; }
    double FixedValue { get; set; }
    double Percentage { get; set; }
    double StandardDeviation { get; set; }
    string FormulaPlus { get; set; }
    string FormulaMinus { get; set; }
    ILineStyle LineStyle { get; set; }
  }
}
