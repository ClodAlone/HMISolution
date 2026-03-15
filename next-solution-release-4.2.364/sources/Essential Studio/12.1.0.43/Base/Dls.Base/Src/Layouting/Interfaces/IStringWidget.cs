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
using System.Drawing;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Represents a Widget with string-like layouting.
  /// </summary>
  public interface IStringWidget : ISplitLeafWidget, ITextMeasurable
  {
    /// <summary>
    /// Gets text string.
    /// </summary>
    string Text{ get; }
    /// <summary>
    /// Draw specified string to custom graphics.
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    /// <param name="text"></param>
    void Draw( CustomGraphics cg, LayoutedWidget ltWidget, string text );
    /// <summary>
    /// Gets position of specified symbol in specified string
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="offset"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    int OffsetToIndex( CustomGraphics graphics, double offset, string text );
    /// <summary>
    /// Gets text ascent.
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    double GetTextAscent( CustomGraphics graphics );
  }

  /// <summary>
  /// 
  /// </summary>
  public interface ITextMeasurable
  {
    /// <summary>
    /// Measures size of specified string.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    SizeF Measure( CustomGraphics graphics, string text );
  }
}