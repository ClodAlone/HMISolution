#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;

using System.Windows.Forms;
using System.IO;
using Syncfusion.Windows.Forms.Edit.Enums;

using Syncfusion.Windows.Forms.Edit;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Text range background settings.
  /// </summary>
  public interface IBackgroundFormat
  {
    /// <summary>
    /// Color of hatch.
    /// </summary>
    Color ForeColor{ get; set; }
    /// <summary>
    /// Background color of snippet. If you want to draw rectangle over the snippet
    /// set Background Color.Empty value and Foreground property to needed rectangle
    /// border color. If both properties Background and Foreground set to not Empty
    /// value then for drawing used hatch brush according to BackStyle property value.
    /// If Foreground set to Color.Empty value then will be filled snippet rectangle
    /// by Background color.
    /// </summary>
    Color BackColor{ get; set; }
    /// <summary>
    /// Style of background brush. This property used only when Background and
    /// Foreground colors set to not Empty values.
    /// </summary>
    HatchStyle HatchStyle{ get; set; }
  }
}
