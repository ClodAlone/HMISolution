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
using System.Drawing;

namespace Syncfusion.XlsIO
{
  public interface IOleObject
  {
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>The location.</value>
    IRange Location { get; set; }
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    Size Size { get; set; }
    /// <summary>
    /// Gets or sets the picture.
    /// </summary>
    /// <value>The picture.</value>
    Image Picture { get; }
    /// <summary>
    /// Gets or sets picture shape object that defines look and position of the OleObject inside parent worksheet.
    /// </summary>
    IPictureShape Shape { get; }
    /// <summary>
    /// Gets or sets a value indicating whether [display as icon].
    /// </summary>
    /// <value><c>true</c> if [display as icon]; otherwise, <c>false</c>.</value>
    bool DisplayAsIcon { get; set; }
  }
}
