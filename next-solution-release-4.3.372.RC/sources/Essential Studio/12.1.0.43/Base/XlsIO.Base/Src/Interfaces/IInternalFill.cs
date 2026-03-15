#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;
namespace Syncfusion.XlsIO.Interfaces
{
  internal interface IInternalFill : IFill
  {
    /// <summary>
    /// Represents background color.
    /// </summary>
    ColorObject BackColorObject { get; }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    ColorObject ForeColorObject { get; }
    /// <summary>
    /// Represents whether picture is tiled or stretched.
    /// </summary>
    bool Tile
    {
      get;
      set;
    }
    GradientStops PreservedGradient
    {
      get;
      set;
    }
    bool IsGradientSupported { get; set;  }    
    /// <summary>
    /// It's define Alphamodfix for bilp(tranparency)
    /// </summary>
    float TransparencyColor
    {
        get;
        set;
    }
    /// <summary>
    /// It's define the Texture properties
    /// </summary>
    float TextureVerticalScale
    {
        get;
        set;
    }
    float TextureHorizontalScale
    {
        get;
        set;
    }
    float TextureOffsetX
    {
        get;
        set;
    }
    float TextureOffsetY
    {
        get;
        set;
    }
    string Alignment
    {
        get;
        set;
    }
    string TileFlipping
    {
        get;
        set;
    }
      
  }
}
