// <copyright file="ChartBarType.cs" company="Syncfusion">
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
    /// Represents ChartBarType
  /// </summary>
    /// <seealso cref="ChartBarType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
  public sealed class ChartBarType : ChartColumnType
  {
    #region Properties
   /// <summary>
   /// Gets chart type flags. This is a dependency property.
   /// </summary>
    protected override ChartTypeFlags Flags
    {
      get
      {
        return ChartTypeFlags.SideBySide | ChartTypeFlags.Rotated | ChartTypeFlags.Indexed;
      }
    }
    #endregion
      /// <summary>
    /// ChartBarType ToString method
      /// </summary>
    /// <returns>The string</returns>
    /// <seealso cref="ChartBarType"/>
    public override string ToString()
    {
        return "Bar";
    }
  }
}
