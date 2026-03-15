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

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO
{
  public interface IShadow
  {

    #region Interface Properties
    /// <summary>
    /// Gets or sets the shadow outer presets.
    /// </summary>
    /// <value>The shadow outer presets.</value>
    Excel2007ChartPresetsOuter ShadowOuterPresets { get; set; }
    /// <summary>
    /// Gets or sets the shadow inner presets.
    /// </summary>
    /// <value>The shadow inner presets.</value>
    Excel2007ChartPresetsInner ShadowInnerPresets { get; set; }
    /// <summary>
    /// Gets or sets the shadow prespective presets.
    /// </summary>
    /// <value>The shadow prespective presets.</value>
    Excel2007ChartPresetsPrespective ShadowPrespectivePresets { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance has custom shadow style.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has custom shadow style; otherwise, <c>false</c>.
    /// </value>
    bool HasCustomShadowStyle { get; set; }
    /// <summary>
    /// Gets or sets the transparency of Shadow.
    /// </summary>
    /// <value>The transparency.</value>
    int Transparency { get; set; }
    /// <summary>
    /// Gets or sets the size of Shadow.
    /// </summary>
    /// <value>The size.</value>
    int Size { get; set; }
    /// <summary>
    /// Gets or sets the blur of Shadow.
    /// </summary>
    /// <value>The blur.</value>
    int Blur { get; set; }
    /// <summary>
    /// Gets or sets the angle of Shadow.
    /// </summary>
    /// <value>The angle.</value>
    int Angle { get; set; }
    /// <summary>
    /// Gets or sets the distance of Shadow.
    /// </summary>
    /// <value>The distance.</value>
    int Distance { get; set; }
    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>The color of the shadow.</value>
    Color ShadowColor { get; set; }
    #endregion

    #region Interface Methods

    /// <summary>
    /// Customs the outer shadow styles.
    /// </summary>
    /// <param name="iOuter">The Excel2007ChartPresetsOuter enumeration.</param>
    /// <param name="iTransparency">Transparency of the Shadow accepts the values between(0-100).</param>
    /// <param name="iSize">Size of the Shadow accepts the values between(0-200).</param>
    /// <param name="iBlur">Blur level of the Shadow accepts the values between(0-100).</param>
    /// <param name="iAngle">Angle or Direction of the Shadow accepts the values between(0-359).</param>
    /// <param name="iDistance">Distance of the Shadow accepts the values between(0-200).</param>
    /// <param name="iCustomShadowStyle">if set to <c>true</c> [custom shadow style].</param>
    void CustomShadowStyles( Excel2007ChartPresetsOuter iOuter, int iTransparency, int iSize, int iBlur, int iAngle, int iDistance, bool iCustomShadowStyle );

    /// <summary>
    /// Customs the inner shadow styles.
    /// </summary>
    /// <param name="iInner">The Excel2007ChartPresetsInner enumeration.</param>
    /// <param name="iTransparency">Transparency of the Shadow accepts the values between(0-100).</param>
    /// <param name="iBlur">Blur level of the Shadow accepts the values between(0-100).</param>
    /// <param name="iAngle">Angle or Direction of the Shadow accepts the values between(0-359).</param>
    /// <param name="iDistance">Distance of the Shadow accepts the values between(0-200).</param>
    /// <param name="iCustomShadowStyle">if set to <c>true</c> [custom shadow style].</param>
    void CustomShadowStyles( Excel2007ChartPresetsInner iInner, int iTransparency, int iBlur, int iAngle, int iDistance, bool iCustomShadowStyle );

    /// <summary>
    /// Customs the perspective shadow styles.
    /// </summary>
    /// <param name="iPerspective">The Excel2007ChartPresetsPerspective enumeration.</param>
    /// <param name="iTransparency">Transparency of the Shadow accepts the values between(0-100).</param>
    /// <param name="iSize">Size of the Shadow accepts the values between(0-200).</param>
    /// <param name="iBlur">Blur level of the Shadow accepts the values between(0-100).</param>
    /// <param name="iAngle">Angle or Direction of the Shadow accepts the values between(0-359).</param>
    /// <param name="iDistance">Distance of the Shadow accepts the values between(0-200).</param>
    /// <param name="iCustomShadowStyle">if set to <c>true</c> [i custom shadow style].</param>
    void CustomShadowStyles( Excel2007ChartPresetsPrespective iPerspective, int iTransparency, int iSize, int iBlur, int iAngle, int iDistance, bool iCustomShadowStyle );
    #endregion

  }
}
