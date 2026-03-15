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

namespace Syncfusion.XlsIO.Implementation.Charts
{
  interface IScalable
  {
    /// <summary>
    /// Represents logarithmic scale.
    /// </summary>
    bool IsLogScale { get; set; }
    /// <summary>
    /// Indicates whether datapoint plot from last to first.
    /// </summary>
    bool IsReversed { get; set; }
    /// <summary>
    /// Represents maximum value.
    /// </summary>
    double MaximumValue { get; set; }
    /// <summary>
    /// Represents minimum value.
    /// </summary>
    double MinimumValue { get; set; }
  }
}
