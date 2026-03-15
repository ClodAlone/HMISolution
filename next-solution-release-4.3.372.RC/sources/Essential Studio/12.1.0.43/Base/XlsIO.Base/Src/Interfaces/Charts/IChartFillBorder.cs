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
  /// This interface contains filling options for area: area fill and border formatting.
  /// </summary>
  public interface IChartFillBorder
  {
    /// <summary>
    /// This property indicates whether interior object was created. Read-only.
    /// </summary>
    bool HasInterior { get; }
    /// <summary>
    /// This property indicates whether line formatting object was created. Read-only.
    /// </summary>
    bool HasLineProperties { get; }
    /// <summary>
    /// Gets a value indicating whether [has3d properties].
    /// </summary>
    /// <value><c>true</c> if [has3d properties]; otherwise, <c>false</c>.</value>
    bool Has3dProperties { get; }
    /// <summary>
    /// Gets a value indicating whether this instance has shadow properties.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has shadow properties; otherwise, <c>false</c>.
    /// </value>
    bool HasShadowProperties { get; }
    /// <summary>
    /// Returns object, that represents line properties. Read-only.
    /// </summary>
    IChartBorder LineProperties { get; }
    /// <summary>
    /// Returns object, that represents area properties. Read-only.
    /// </summary>
    IChartInterior Interior { get; }
    /// <summary>
    /// Represents fill options. Read-only.
    /// </summary>
    IFill Fill { get; }
    /// <summary>
    /// Gets the chart3 D properties.
    /// </summary>
    /// <value>The chart3 D properties.</value>
    IThreeDFormat ThreeD{ get; }
    /// <summary>
    /// Gets the shadow properties.
    /// </summary>
    /// <value>The shadow properties.</value>
    IShadow Shadow{ get;  }
  }
}
