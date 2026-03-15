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
  /// Summary description for IAxisGridlines.
  /// </summary>
  public interface IAxisGridlines
  {
    ILineStyle LineStyle { get; set; }
  
    #region Value Axis
    bool IsAuto { get; set; }
    bool IsMinimum { get; set; }
    bool IsMaximum { get; set; }
    bool IsMajorUnit { get; set; }
    bool IsMinorUnit { get; set; }
    bool IsCrossCategory { get; set; }
    int  DisplayUnits { get; set; }
    bool IsLogarithmicScale { get; set; }
    bool AreValuesReversed { get; set; }
    bool IsCatCrossedAtMaxVal { get; set; }
    #endregion

    #region CategoryAxis
    int ValueAxisCross { get; set; }
    int CategoryNumberBetweenTickMarksLabels { get; set; }
    int CategoryNumberBetweenTickMarks { get; set; }
    bool IsValueAxisCrossesBetween { get; set; }
    bool IsValueAxisCrossesAtMaximum  { get; set; }
    bool AreCategoriesReversed { get; set; }
    #endregion

  }
}
