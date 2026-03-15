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

#region File using directives
using System;
using System.Collections.Generic;
using System.Text;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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


using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Interface used to get gradient fill effects.
  /// </summary>
  public interface IGradient
  {
    #region Interface properties
    /// <summary>
    /// Represents background color.
    /// </summary>
    ColorObject BackColorObject { get; }
    /// <summary>
    /// Represents background color.
    /// </summary>
    Color BackColor { get; set; }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    ExcelKnownColors BackColorIndex { get; set; }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    ColorObject ForeColorObject { get; }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    Color ForeColor { get; set; }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    ExcelKnownColors ForeColorIndex { get; set; }
    /// <summary>
    /// Represents gradient shading style.
    /// </summary>
    ExcelGradientStyle GradientStyle { get; set; }
    /// <summary>
    /// Represents gradient shading variant.
    /// </summary>
    ExcelGradientVariants GradientVariant { get; set; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Compares with gradient.
    /// </summary>
    /// <param name="gradient">Gradient to compare with.</param>
    /// <returns>Zero if gradients are equal.</returns>
    int CompareTo( IGradient gradient );
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    void TwoColorGradient();
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    void TwoColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant );
    #endregion
  }
}
