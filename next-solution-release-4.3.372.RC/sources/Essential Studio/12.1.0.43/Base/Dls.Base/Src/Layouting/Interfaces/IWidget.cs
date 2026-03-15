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
  #region Interface IWidget 
  /// <summary>
  /// Represents a base Widget interface
  /// </summary>
  public interface IWidget
  {
    /// <summary>
    /// Gets layout info.
    /// </summary>
    ILayoutInfo LayoutInfo{ get; }
    /// <summary>
    /// Draws widget to custom graphics.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="ltWidget"></param>
    void Draw( CustomGraphics g, LayoutedWidget ltWidget );
  }
  #endregion 
  
  #region Interface IWidgetContainer
  /// <summary>
  /// Represents a collection of Widget objects.
  /// </summary>
  public interface IWidgetContainer : IWidget
  {
    /// <summary>
    /// Gets count of child widgets.
    /// </summary>
    int Count{ get; }
    /// <summary>
    /// Gets child widget by index.
    /// </summary>
    IWidget this[ int index ]{ get; }
  }
  #endregion 
  
  #region Interface ILeafWidget
  /// <summary>
  /// Represent a widget that can MEASURE self.
  /// </summary>
  public interface ILeafWidget : IWidget
  {
    /// <summary>
    /// Measures self size. 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    SizeF Measure( CustomGraphics graphics );
  }
  /// <summary>
  /// Represents a LEAF Widget that can SPLIT self.
  /// </summary>
  public interface ISplitLeafWidget : ILeafWidget
  {
    /// <summary>
    /// Split self to two widgets that can also splitted
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="offset">Position from left side of widget where we slice it.</param>
    /// <returns></returns>
    ISplitLeafWidget[] SplitByOffset( CustomGraphics graphics,  SizeF offset );
  }
  #endregion 
}