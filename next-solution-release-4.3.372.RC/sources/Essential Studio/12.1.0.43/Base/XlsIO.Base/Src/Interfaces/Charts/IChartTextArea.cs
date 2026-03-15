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

using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the Text Area in a chart.
  /// </summary>
  public interface IChartTextArea
    : IParentApplication
    , IFont
  {
    #region Class properties
    /// <summary>
    /// Area's text.
    /// </summary>
    string Text { get; set; }
    /// <summary>
    /// String with rich text formatting. Read-only.
    /// </summary>
    IChartRichTextString RichText { get; }
    /// <summary>
    /// Text rotation angle.
    /// </summary>
    int TextRotationAngle { get; set; }
    /// <summary>
    /// Return format of the text area.
    /// </summary>
    IChartFrameFormat FrameFormat { get; }
    /// <summary>
    /// Display mode of the background.
    /// </summary>
    ExcelChartBackgroundMode BackgroundMode{ get; set; }
    /// <summary>
    /// True if background is set to automatic.
    /// </summary>
    bool IsAutoMode{ get; set; }
    /// <summary>
    /// Represents the Layout settings of TextArea
    /// </summary>
    IChartLayout Layout { get; set; }
    #endregion
  }
}
