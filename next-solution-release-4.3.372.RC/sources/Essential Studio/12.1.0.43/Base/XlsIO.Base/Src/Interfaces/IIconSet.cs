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

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// This interface provides access to Conditional formatting icon set condition.
  /// </summary>
  public interface IIconSet
  {
    /// <summary>
    /// Returns an IconCriteria collection which represents the set of criteria for
    /// an icon set conditional formatting rule.
    /// </summary>
    IList<IConditionValue> IconCriteria { get; }
    /// <summary>
    /// Returns or sets an IconSets collection which specifies the icon set used
    /// in the conditional format.
    /// </summary>
    ExcelIconSetType IconSet { get; set; }
    /// <summary>
    /// Returns or sets a Boolean value indicating if the thresholds for an icon
    /// set conditional format are determined using percentiles. 
    /// </summary>
    bool PercentileValues { get; set; }
    /// <summary>
    /// Returns or sets a Boolean value indicating if the order of icons is
    /// reversed for an icon set.
    /// </summary>
    bool ReverseOrder { get; set; }
    /// <summary>
    /// Returns or sets a Boolean value indicating if only the icon is displayed
    /// for an icon set conditional format.
    /// </summary>
    bool ShowIconOnly { get; set; }
  }
}
