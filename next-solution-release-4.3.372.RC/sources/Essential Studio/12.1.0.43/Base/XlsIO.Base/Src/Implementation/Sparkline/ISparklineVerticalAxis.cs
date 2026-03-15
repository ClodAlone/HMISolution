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
  public interface ISparklineVerticalAxis
  {
    /// <summary>
    /// Gets or sets the custom Value for the Vertical axis custom option.
    /// </summary>
    /// <value>The custom.</value>
    /// <exception cref=" NotSupportedException">If the VaerticalAxisOptions is not equal to Custom</exception>
    double CustomValue { get; set; }

    /// <summary>
    /// Gets or sets the vertical axis options.
    /// </summary>
    /// <value>The vertical axis options.</value>
    SparklineVerticalAxisOptions VerticalAxisOptions { get; set; }
  }
}
