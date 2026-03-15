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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a shape.
  /// </summary>
  public interface IShape : IParentApplication
  {
    #region Interface properties
    /// <summary>
    /// Height of the shape.
    /// </summary>
    int Height { get; set; }
    /// <summary>
    /// Shape id.
    /// </summary>
    int Id { get; }
    /// <summary>
    /// Left position of the shape.
    /// </summary>
    int Left { get; set; }
    /// <summary>
    /// Name of the shape.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Top position of the shape.
    /// </summary>
    int Top { get; set; }
    /// <summary>
    /// Width of the shape.
    /// </summary>
    int Width { get; set; }
    /// <summary>
    /// Shape type.
    /// </summary>
    ExcelShapeType ShapeType { get; }
    /// <summary>
    /// Indicates whether shape is visible.
    /// </summary>
    bool IsShapeVisible { get; set; }
    /// <summary>
    /// Alternative text.
    /// </summary>
    string AlternativeText { get; set; }
    /// <summary>
    /// Indicates whether shape must be moved with cells.
    /// </summary>
    bool   IsMoveWithCell { get; set; }
    /// <summary>
    /// Indicates whether shape must be sized with cells.
    /// </summary>
    bool  IsSizeWithCell { get; set; }
    /// <summary>
    /// Represents fill properties. Read-only.
    /// </summary>
    IFill Fill { get; }
    /// <summary>
    /// Represents line format properties. Read-only.
    /// </summary>
    IShapeLineFormat Line { get; }
    /// <summary>
    /// Gets or sets macro associated with this shape
    /// </summary>
    string OnAction { get; set; }
    IShadow Shadow { get; }
    IThreeDFormat ThreeD { get;  }
    /// <summary>
    /// Returns or sets the rotation of the shape, in degrees.
    /// </summary>   
    int ShapeRotation { get; set; }
    /// <summary>
    /// Returns a TextFrame object that contains the 
    /// alignment and anchoring properties for the specified shape. Read-only.
    /// </summary>
    ITextFrame TextFrame { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Removes this shape from shapes collection.
    /// </summary>
    void Remove();
    /// <summary>
    /// Scales the shape.
    /// </summary>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    void Scale( int scaleWidth, int scaleHeight );
    #endregion
  }
}
