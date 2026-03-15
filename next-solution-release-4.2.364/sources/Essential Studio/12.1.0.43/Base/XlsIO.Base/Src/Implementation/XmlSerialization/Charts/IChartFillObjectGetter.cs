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

using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// Provides access to filling options (border, interior and fill) of some chart object.
  /// </summary>
  internal interface IChartFillObjectGetter
  {
    /// <summary>
    /// Gets border object. Read-only.
    /// </summary>
    ChartBorderImpl Border { get; }
    /// <summary>
    /// Gets interior object. Read-only.
    /// </summary>
    ChartInteriorImpl Interior { get; }
    /// <summary>
    /// Gets fill object. Read-only.
    /// </summary>
    IInternalFill Fill { get; }
    /// <summary>
    /// Gets Shadow object.Read-only
    /// </summary>
    ShadowImpl Shadow { get; }
    /// <summary>
    /// Gets the three_ D.Read-only
    /// </summary>      
    ThreeDFormatImpl ThreeD { get; }

  }
}
