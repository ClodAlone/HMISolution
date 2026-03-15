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
  public interface ISparkline
  {
    /// <summary>
    ///Represents the data range of the sparkline.
    /// </summary>
    /// <value>The data range.</value>
    /// <exception cref="ArgumentOutOfRange">
    /// if the value.Rows.Length is not equal to 1.
    /// </exception>
    IRange DataRange { get; set; }
    /// <summary>
    /// Represents the reference range of the sparkline.
    /// </summary>
    /// <value>The reference range.</value>
    /// <exception cref="ArgumentOutOfRange">
    /// if the value.Rows.length and value.Columns.Length is not equal to 1;
    /// </exception>
    IRange ReferenceRange { get; set; }
    /// <summary>
    /// Gets the column index of a sparkline.
    /// </summary>
    /// <value>The column index.</value>
    int Column { get; }
    /// <summary>
    /// Gets the row index of a sparkline.
    /// </summary>
    /// <value>The row index.</value>
    int Row { get; }

  }
}
