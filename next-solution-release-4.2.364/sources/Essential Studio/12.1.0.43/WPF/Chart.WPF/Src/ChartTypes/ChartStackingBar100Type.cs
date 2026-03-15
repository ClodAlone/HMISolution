// <copyright file="ChartStackingBar100Type.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;

  /// <summary>
  /// Represents Stacked 100 bar type.
  /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
  public class ChartStackingBar100Type : ChartStackingColumn100Type
  {
    #region Properties
    /// <summary>
    /// Gets chart type flags. This is a dependency property.
    /// </summary>
    protected override ChartTypeFlags Flags
    {
      get
      {
        return ChartTypeFlags.Rotated | ChartTypeFlags.Indexed;
      }
    }
    #endregion
    /// <summary>
    /// ChartBarType ToString method
    /// </summary>
    /// <returns>The string</returns>
    /// <seealso cref="ChartStackingBar100Type"/>
    public override string ToString()
    {
      return "StackingBar100";
    }
  }  
}
