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
  /// Summary description for ILabelText.
  /// </summary>
  public interface ILabelText
    : IChartTitle
  {
    string NumberFormat { get; set; }

    #region /* comments */
//    ILineStyle Border { get; set; }
//    IArea Area { get; set; }
//    IChartFont Font { get; set; }
//    ExcelChartHorzAlignment HAlignment { get; set; }
//    ExcelChartVertAlignment VAlignment { get; set; }
//    int LabelPos { get; set; }
//    int TextDirection { get; set; }
//    int TextOrientation { get; set; }
    #endregion
  }
}
