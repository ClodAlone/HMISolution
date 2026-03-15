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
  /// Condition value for color scale conditional format.
  /// </summary>
  public interface IColorConditionValue : IConditionValue
  {
    /// <summary>
    /// The color assigned to the threshold of a color scale conditional format.
    /// </summary>
    Color FormatColorRGB{ get; set; }
    /// <summary>
    /// Returns one of the constants of the XlConditionValueTypes enumeration,
    /// which specifies how the threshold values for a data bar, color scale,
    /// or icon set conditional format are determined. Read-only.
    /// </summary>
    ConditionValueType Type { get; set; }
    /// <summary>
    /// Returns or sets the shortest bar or longest bar threshold value for a data
    /// bar conditional format.
    /// </summary>
    string Value { get; set; }
    /// <summary>
    /// Returns or sets one of the constants of the ConditionalFormatOperator enumeration, 
    /// which specifes if the threshold is "greater than" or "greater than or equal to" the threshold value.
    /// </summary>
    ConditionalFormatOperator Operator { get; set; }
  }
}
