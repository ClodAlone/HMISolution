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
  /// Represents pivot table data field.
  /// </summary>
  public interface IPivotDataField
  {
    /// <summary>
    /// Gets / sets name of the data field.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Gets/ sets subtotal function used for data field.
    /// </summary>
    PivotSubtotalTypes Subtotal { get; set; }
    /// <summary>
    /// Gets or sets the show data as.
    /// </summary>
    /// <value>The show data as.</value>
    PivotFieldDataFormat ShowDataAs { get; set; }
    /// <summary>
    /// Gets or sets the base item.
    /// </summary>
    /// <value>The base item.</value>
    int BaseItem { get; set; }
    /// <summary>
    /// Gets or sets the base field.
    /// </summary>
    /// <value>The base field.</value>
    int BaseField { get; set; }
  }
}
