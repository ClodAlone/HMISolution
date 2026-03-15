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

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the chart Frame Format options.
  /// </summary>
  public interface IChartFrameFormat : IChartFillBorder
  {
    #region Interface properties
    /// <summary>
    /// Rectangle style.
    /// </summary>
    ExcelRectangleStyle RectangleStyle { get; set; }
    /// <summary>
    /// Gets or sets flag if border corners is round.
    /// </summary>
    bool IsBorderCornersRound { get; set; }
    /// <summary>
    /// Represents chart border. Read-only.
    /// </summary>
    IChartBorder Border { get; }
    /// <summary>
    /// Represents the Layout settings of TextArea
    /// </summary>
    IChartLayout Layout { get; set; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Clear current plot area.
    /// </summary>
    void Clear();
    #endregion
  }
}
