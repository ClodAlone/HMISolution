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
using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
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
  /// Interface that contains parent application.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public interface IParentApplication
  {
    /// <summary>
    /// Application object for this object.
    /// </summary>
    IApplication Application{ get; }
    /// <summary>
    /// Parent object for this object.
    /// </summary>
    object Parent{ get; }
  }
  /// <summary>
  /// 
  /// </summary>
  internal interface IReparse
  {
    /// <summary>
    /// 
    /// </summary>
    void Reparse();
  }
  /// <summary>
  /// Represents interface, that synchronize chart fill color properties and interior colors.
  /// </summary>
  internal interface IFillColor
  {
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    ColorObject ForeGroundColorObject { get; }
    /// <summary>
    /// Represents background color.
    /// </summary>
    ColorObject BackGroundColorObject { get; }
    /// <summary>
    /// Represents pattern.
    /// </summary>
    ExcelPattern Pattern  { get; set; }
    /// <summary>
    /// Represents is automatic format.
    /// </summary>
    bool IsAutomaticFormat { get; set; }
    /// <summary>
    /// Represents fill properties.
    /// </summary>
    IFill Fill{ get; }
    /// <summary>
    /// Represents visible.
    /// </summary>
    bool Visible { get; set; }
  }
}
