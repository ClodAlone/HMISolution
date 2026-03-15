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
	/// Interface that represents chart wall or floor.
	/// </summary>
  public interface IChartWallOrFloor :
    IChartGridLine,
    IChartFillBorder
  {
    /// <summary>
    /// Gets or Sets the thickness of the walls or floor.
    /// </summary>
    uint Thickness { get; set; }
    /// <summary>
    /// Gets or Sets the pictureType in walls or floor
    /// </summary>
    ExcelChartPictureType PictureUnit { get; set; }      
    ///// <summary>
    ///// Represents chart interior.
    ///// </summary>
    //IChartInterior Interior { get; }
    ///// <summary>
    ///// Represents fill properties. Read-only.
    ///// </summary>
    //IFill Fill { get; }
  }
}
