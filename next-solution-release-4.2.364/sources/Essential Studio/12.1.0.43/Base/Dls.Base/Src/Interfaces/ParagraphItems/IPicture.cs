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
using System.Drawing;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes picture functionality
  /// </summary>
  public interface IPicture : IParagraphItem
  {
    /// <summary>
    /// Gets / sets picture height.
    /// </summary>
    float Height { get; set; }
    /// <summary>
    /// Gets / sets picture width.
    /// </summary>
    float Width { get; set; }
    /// <summary>
    /// Gets / sets picture height scale factor in percent.
    /// </summary>
    float HeightScale { get; set; }
    /// <summary>
    /// Gets / sets picture width scale factor in percent.
    /// </summary>
    float WidthScale { get; set; }
//    /// <summary>
//    /// Get/set picture brightness.
//    /// </summary>
//    float Brightness{ get; set; }
//    /// <summary>
//    /// Get/set picture contrast.
//    /// </summary>
//    float Contrast { get; set; }
//    /// <summary>
//    /// Get/set picture color.
//    /// </summary>
//    PictureColor Color { get; set; }
//    /// <summary>
//    /// 
//    /// </summary>
//    float CropFromLeft { get; set; }
//    /// <summary>
//    /// 
//    /// </summary>
//    float CropFromRight { get; set; }
//    /// <summary>
//    /// 
//    /// </summary>
//    float CropFromTop { get; set; }
//    /// <summary>
//    /// 
//    /// </summary>
//    float CropFromBottom { get; set; }
    /// <summary>
    /// Loads System.Drawing.Image object.
    /// </summary>
    /// <param name="image"></param>
    void LoadImage( Image image );
    /// <summary>
    /// Gets internal System.Drawing.Image object.
    /// </summary>
    Image Image{ get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageBytes"></param>
    void LoadImage( byte[] imageBytes );
  }
}