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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// Class used for representing chart axis scale.
  /// </summary>
  class ChartAxisScale
  {
    /// <summary>
    /// Represents logarithmic scale.
    /// </summary>
    public bool? LogScale = null;
    /// <summary>
    /// Indicates whether datapoint plot from last to first.
    /// </summary>
    public bool? Reversed;
    /// <summary>
    /// Represents maximum value.
    /// </summary>
    public double? MaximumValue = null;
    /// <summary>
    /// Represents minimum value.
    /// </summary>
    public double? MinimumValue = null;
    /// <summary>
    /// Method used to copy the axis scale.
    /// </summary>
    /// <param name="axis">Represents Chart value axis.</param>
    public void CopyTo( IScalable axis )
    {
      if( LogScale != null )
        axis.IsLogScale = ( bool )LogScale;

      if( Reversed != null )
        axis.IsReversed = ( bool )Reversed;

      if( MaximumValue != null )
        axis.MaximumValue = ( double )MaximumValue;

      if( MinimumValue != null )
        axis.MinimumValue = ( double )MinimumValue;
    }
  }
}
