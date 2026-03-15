#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents error bars interface.
  /// </summary>
  public interface IChartErrorBars
  {
    #region Interface properties
    /// <summary>
    /// Represents border object. Read-only.
    /// </summary>
    IChartBorder Border { get; }
    /// <summary>
    /// Represents error bar include type.
    /// </summary>
    ExcelErrorBarInclude Include { get; set; }
    /// <summary>
    /// Indicates if error bar has cap.
    /// </summary>
    bool HasCap { get; set; }
    /// <summary>
    /// Represents excel error bar type.
    /// </summary>
    ExcelErrorBarType Type { get; set; }
    /// <summary>
    /// Represents number value.
    /// </summary>
    double NumberValue { get; set; }
    /// <summary>
    /// Represents custom plus value.
    /// </summary>
    IRange PlusRange{ get; set; }
    /// <summary>
    /// Represents custom minus value.
    /// </summary>
    IRange MinusRange{ get; set; }
    /// <summary>
    /// Gets the shadow.
    /// </summary>
    /// <value>The shadow.</value>
    IShadow Shadow { get; }
    /// <summary>
    /// Gets the chart3 D options.
    /// </summary>
    /// <value>The chart3 D options.</value>
    IThreeDFormat Chart3DOptions { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Clears current error bar.
    /// </summary>
    void ClearFormats();
    /// <summary>
    /// Delete current error bar.
    /// </summary>
    void Delete();
    #endregion
  }
}
