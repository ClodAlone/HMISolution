// <copyright file="_Enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;

  /// <summary>
  /// Represents ChartAxisType
  /// </summary>
  public enum ChartAxisType
  { 
    /// <summary>
    /// The X value
    /// </summary>
    X,

    /// <summary>
    /// The Y value
    /// </summary>
    Y,

    /// <summary>
    /// The Z value
    /// </summary>
    Z
  }

  /// <summary>
  /// Specifies the mode in which the Chart Space values should be interpreted
  /// </summary>
  public enum ChartSpaceType
  { 
    /// <summary>
    /// The None Space value
    /// </summary>
    None = 0x00,
    
    /// <summary>
    /// The X Space value
    /// </summary>
    X = 0x01,
    
    /// <summary>
    /// The Y Space value
    /// </summary>
    Y = 0x02,
    
    /// <summary>
    /// The Z Space value
    /// </summary>
    Z = 0x04,

    /// <summary>
    /// The XYZ Space value
    /// </summary>
    XYZ = X | Y | Z,
    
    /// <summary>
    /// The XDepth Space value
    /// </summary>
    XDepth = 0x11,
    
    /// <summary>
    /// The YDepth Space value
    /// </summary>
    YDepth = 0x22,
    
    /// <summary>
    /// The ZDepth Space value
    /// </summary>
    ZDepth = 0x44
  }
}
