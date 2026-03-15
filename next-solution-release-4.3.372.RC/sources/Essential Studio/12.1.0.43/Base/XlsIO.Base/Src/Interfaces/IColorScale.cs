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
  /// This interface provides access to the color scale condition in the conditional format.
  /// </summary>
  public interface IColorScale
  {
    /// <summary>
    /// Returns a collection of individual IColorConditionValue objects.
    /// The IColorConditionValue object specifies the type, value, and the color
    /// of threshold criteria used in the color scale conditional format. Read-only.
    /// </summary>
    IList<IColorConditionValue> Criteria { get; }
    /// <summary>
    /// Sets number of IColorConditionValue objects in the collection. Supported values are 2 and 3.
    /// </summary>
    /// <param name="count">Number of conditions.</param>
    void SetConditionCount( int count );
  }
}
