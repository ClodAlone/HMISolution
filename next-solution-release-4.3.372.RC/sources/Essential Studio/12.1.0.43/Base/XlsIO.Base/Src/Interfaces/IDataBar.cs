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


#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a data bar conditional formatting rule. Applying a data bar to a
  /// range helps you see the value of a cell relative to other cells.
  /// </summary>
  public interface IDataBar
  {
    /// <summary>
    /// Returns a ConditionValue object which specifies how the shortest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    IConditionValue MinPoint { get; }
    /// <summary>
    /// Returns a ConditionValue object which specifies how the longest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    IConditionValue MaxPoint { get; }
    /// <summary>
    /// Gets/sets the color of the bars in a data bar conditional format.
    /// </summary>
    Color BarColor { get; set; }
    /// <summary>
    /// Returns or sets a value that specifies the length of the longest
    /// data bar as a percentage of cell width.
    /// </summary>
    int PercentMax { get; set; }
    /// <summary>
    /// Returns or sets a value that specifies the length of the shortest
    /// data bar as a percentage of cell width.
    /// </summary>
    int PercentMin { get; set; }
    /// <summary>
    /// Returns or sets a Boolean value that specifies if the value in the cell
    /// is displayed if the data bar conditional format is applied to the range.
    /// </summary>
    bool ShowValue { get; set; }
    /// <summary>
    /// Gets or sets the axis color of the data bar. 
    /// This element MUST exist if and only if axisPosition does not equal "none".
    /// </summary>
    Color BarAxisColor { get; set; }
    /// <summary>
    /// Gets or sets the border color of the data bar. 
    /// This element MUST exist if and only if border equals "true".
    /// </summary>
    Color BorderColor { get; set; }
    /// <summary>
    /// Gets whether the data bar has a border
    /// </summary>
    bool HasBorder { get; }
    /// <summary>
    /// Gets or sets whether the data bar has a gradient fill.
    /// </summary>
    bool HasGradientFill { get; set; }
    /// <summary>
    /// Gets or sets the direction of the data bar.
    /// </summary>
    DataBarDirection DataBarDirection { get; set; }
    /// <summary>
    /// Gets or sets the negative border color of the data bar. 
    /// This element MUST exist if and only if negativeBarBorderColorSameAsPositive equals "false" and border equals "true".
    /// </summary>
    Color NegativeBorderColor { get; set; }
    /// <summary>
    /// Gest or sests the negative fill color of the data bar. 
    /// This element MUST exist if and only if negativeBarColorSameAsPositive equals "false".
    /// </summary>
    Color NegativeFillColor { get; set; }
    /// <summary>
    /// Gets or sets the axis position for the data bar
    /// </summary>
    DataBarAxisPosition DataBarAxisPosition { get; set; }
  }
}
