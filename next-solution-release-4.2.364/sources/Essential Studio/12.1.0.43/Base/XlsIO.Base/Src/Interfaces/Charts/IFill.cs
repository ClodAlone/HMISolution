#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT || WP) 
using System.Windows.Media;
using System.Drawing;
#elif !(WINRT )
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
	/// Represents fill format.
	/// </summary>
  public interface IFill
  {
    #region Interface properties
    /// <summary>
    /// Represents shape fill type.
    /// </summary>
    ExcelFillType FillType { get; set; }
    /// <summary>
    /// Represents gradient shading style.
    /// </summary>
    ExcelGradientStyle GradientStyle{ get; set; }
    /// <summary>
    /// Represents current shading variant.
    /// </summary>
    ExcelGradientVariants GradientVariant { get; set; }
    /// <summary>
    /// Returns or sets the degree of transparency of the specified fill as
    ///  a value from 0.0 (opaque) through 1.0 (clear).
    /// </summary>
    double TransparencyTo { get; set; }
    /// <summary>
    /// Returns or sets the degree of transparency of the specified fill as
    ///  a value from 0.0 (opaque) through 1.0 (clear).
    /// </summary>
    double TransparencyFrom { get; set; }
    /// <summary>
    /// Represents gradient style.
    /// </summary>
    ExcelGradientColor GradientColorType { get; set; }
    /// <summary>
    /// Represents gradient pattern
    /// </summary>
    ExcelGradientPattern Pattern { get; set; }
    /// <summary>
    /// Represents gradient texture
    /// </summary>
    ExcelTexture Texture { get; set; }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    ExcelKnownColors BackColorIndex { get; set; }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    ExcelKnownColors ForeColorIndex { get; set; }
    /// <summary>
    /// Represents background color.
    /// </summary>
    Color BackColor { get; set; }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    Color ForeColor { get; set; }
    /// <summary>
    /// Represents preset gradient type.
    /// </summary>
    ExcelGradientPreset PresetGradientType { get; set; }
    /// <summary>
    /// Gets or Sets the Transparency for specified picture_only. 
    /// </summary>
    float TransparencyColor
    {
        get;
        set;
    }
    /// <summary>
    /// Represents user defined picture or texture. Read-only.
    /// </summary>
    Image Picture { get; }    
    /// <summary>
    /// Returns user defined picture of texture name. Read-only.
    /// </summary>
    string PictureName { get; }
    /// <summary>
    /// Represents if fill style visible.
    /// </summary>
    bool Visible { get; set; }
    /// <summary>
    /// Returns the gradient degree of the specified one-color shaded fill as a floating-point
    /// value from 0.0 (dark) through 1.0 (light)
    /// </summary>
    double GradientDegree { get; set; }
    /// <summary>
    /// Returns the transparency level of the specified Solid color shaded fill as a floating-point
    /// value from 0.0 (Clear) through 1.0(Opaque)
    /// </summary>
    double Transparency { get; set; }
    /// <summary>
    /// Gets or Sets the TextureVerticalScale for specified fill
    /// </summary>
    float TextureVerticalScale
    {
        get;
        set;
    }
    /// <summary>
    /// Gets or Sets the TextureHorizontalScale for specified fill 
    /// </summary>
    float TextureHorizontalScale
    {
        get;
        set;
    }
    /// <summary>
    /// Represents  the offset X for the texture fill
    /// </summary>
    float TextureOffsetX
    {
        get;
        set;
    }
    /// <summary>
    /// Represents  the offset Y for the texture fill
    /// </summary>
    float TextureOffsetY
    {
        get;
        set;
    }
    #endregion

    #region Interface methods
#if !(WINRT )
    /// <summary>
    /// Sets user defined picture.
    /// </summary>
    /// <param name="path">Path to image.</param>
    void UserPicture( string path );
#endif
    /// <summary>
    /// Sets user defined picture.
    /// </summary>
    ///<param name="im">Represents user defined image.</param>
    ///<param name="name">Represents name of user defined image.</param>
    void UserPicture( Image im, string name );
    /// <summary>
    /// Sets user defined texture.
    /// </summary>
    ///<param name="im">Represents user defined texture.</param>
    ///<param name="name">Represents name of user defined texture.</param>
    void UserTexture( Image im, string name );
#if !(WINRT )
    /// <summary>
    /// Sets user defined texture.
    /// </summary>
    /// <param name="path">Path to image.</param>
    void UserTexture( string path );
#endif
    /// <summary>
    /// Sets the specified fill to a pattern.
    /// </summary>
    /// <param name="pattern">Pattern to set.</param>
    void Patterned( ExcelGradientPattern pattern );
    /// <summary>
    /// Sets the specified fill to a preset gradient.
    /// </summary>
    /// <param name="grad">Represents preset gradient type.</param>
    void PresetGradient( ExcelGradientPreset grad );
    /// <summary>
    /// Sets the specified fill to a preset gradient.
    /// </summary>
    /// <param name="grad">Represents preset gradient type.</param>
    /// <param name="shadStyle">Represents gradient style, for preset gradient.</param>
    void PresetGradient( ExcelGradientPreset grad, ExcelGradientStyle shadStyle );
    /// <summary>
    /// Sets the specified fill to a preset gradient.
    /// </summary>
    /// <param name="grad">Represents preset gradient type.</param>
    /// <param name="shadStyle">Represents gradient style, for preset gradient.</param>
    /// <param name="shadVar">Represents gradient variant for preset gradient.</param>
    void PresetGradient( ExcelGradientPreset grad, ExcelGradientStyle shadStyle
      , ExcelGradientVariants shadVar );
    /// <summary>
    /// Sets the specified fill format to a preset texture.
    /// </summary>
    /// <param name="texture">Represents texture to set.</param>
    void PresetTextured( ExcelTexture texture );
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    void TwoColorGradient();
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    void TwoColorGradient( ExcelGradientStyle style );
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    void TwoColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant );
    /// <summary>
    /// Sets the specified fill to a one-color gradient.
    /// </summary>
    void OneColorGradient();
    /// <summary>
    /// Sets the specified fill to a one-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    void OneColorGradient( ExcelGradientStyle style );
    /// <summary>
    /// Sets the specified fill to a one-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    void OneColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant );
    /// <summary>
    /// Sets the specified fill to a uniform color.
    /// </summary>
    void Solid();
    #endregion 
  }
}
