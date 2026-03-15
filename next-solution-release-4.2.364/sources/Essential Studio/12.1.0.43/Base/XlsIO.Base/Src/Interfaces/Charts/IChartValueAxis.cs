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
  /// Represents the chart value axis.
  /// </summary>
  public interface IChartValueAxis : IChartAxis
  {
    /// <summary>
    /// Minimum value on axis.
    /// </summary>
    double MinimumValue { get; set; }
    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    double MaximumValue { get; set; }
    /// <summary>
    /// Value of major increment.
    /// </summary>
    double MajorUnit { get; set; }
    /// <summary>
    /// Value of minor increment.
    /// </summary>
    double MinorUnit { get; set; }
    /// <summary>
    /// Represents the point on the axis another axis crosses it.
    /// </summary>
    [ Obsolete( "This property is obsolete. Please use CrossesAt instead of it" ) ]
    double CrossValue { get; set; }
    /// <summary>
    /// Represents the point on the axis another axis crosses it.
    /// </summary>
    double CrossesAt { get; set; }
    /// <summary>
    /// Automatic minimum selected.
    /// </summary>
    bool IsAutoMin { get; set; }
    /// <summary>
    /// Automatic maximum selected.
    /// </summary>
    bool IsAutoMax { get; set; }
    /// <summary>
    /// Automatic major selected.
    /// </summary>
    bool IsAutoMajor { get; set; }
    /// <summary>
    /// Automatic minor selected.
    /// </summary>
    bool IsAutoMinor { get; set; }
    /// <summary>
    /// Automatic category crossing point selected.
    /// </summary>
    bool IsAutoCross { get; set; }
    /// <summary>
    /// Logarithmic scale.
    /// </summary>
    bool IsLogScale { get; set; }
    /// <summary>
    /// Category axis to cross at maximum value.
    /// </summary>
    bool IsMaxCross { get; set; }
    /// <summary>
    /// Represents custom unit to display.
    /// </summary>
    double DisplayUnitCustom { get; set; }
    /// <summary>
    /// Returns or sets the unit label for the specified axis.
    /// </summary>
    ExcelChartDisplayUnit DisplayUnit { get; set; }
    /// <summary>
    /// True if the label is displayed on the specified axis.
    /// </summary>
    bool HasDisplayUnitLabel { get; set; }
    /// <summary>
    /// Returns the DisplayUnitLabel object for the specified axis.
    /// Returns Null if the HasDisplayUnitLabel property is set to False. Read-only.
    /// </summary>
    IChartTextArea DisplayUnitLabel { get; }
  }
}
