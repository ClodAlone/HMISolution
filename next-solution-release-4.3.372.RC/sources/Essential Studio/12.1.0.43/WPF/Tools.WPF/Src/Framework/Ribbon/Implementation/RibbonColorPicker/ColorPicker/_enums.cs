// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Color selection mode enum.
    /// </summary>
    /// <property name="flag" value="Finished" />
    internal enum xColorSelectionMode
    {
        /// <summary>
        /// To select RGB mode.
        /// </summary>
        RGB,

        /// <summary>
        /// To select HSV mode.
        /// </summary>
        HSV
    }

    /// <summary>
    /// Known color enum.
    /// </summary>
    /// <property name="flag" value="Finished" />
    internal enum xKnownColor : uint
    {
        /// <summary>
        /// AliceBlue color enum.
        /// </summary>
        /// <property name="flag" value="Finished" />
        AliceBlue = 0xfff0f8ff,

        /// <summary>
        /// AntiqueWhite color enum.
        /// </summary>
        AntiqueWhite = 0xfffaebd7,

        /// <summary>
        /// Aqua color enum.
        /// </summary>
        Aqua = 0xff00ffff,

        /// <summary>
        /// Aquamarine color enum.
        /// </summary>
        Aquamarine = 0xff7fffd4,

        /// <summary>
        /// Azure color enum.
        /// </summary>
        Azure = 0xfff0ffff,

        /// <summary>
        /// Beige color enum.
        /// </summary>
        Beige = 0xfff5f5dc,

        /// <summary>
        /// Bisque color enum.
        /// </summary>
        Bisque = 0xffffe4c4,

        /// <summary>
        /// Black color enum.
        /// </summary>
        Black = 0xff000000,

        /// <summary>
        /// BlanchedAlmond color enum.
        /// </summary>
        BlanchedAlmond = 0xffffebcd,

        /// <summary>
        /// Blue color enum.
        /// </summary>
        Blue = 0xff0000ff,

        /// <summary>
        /// BlueViolet color enum.
        /// </summary>
        BlueViolet = 0xff8a2be2,

        /// <summary>
        /// Brown color enum.
        /// </summary>
        Brown = 0xffa52a2a,

        /// <summary>
        /// BurlyWood color enum.
        /// </summary>
        BurlyWood = 0xffdeb887,

        /// <summary>
        /// CadeBlue color enum.
        /// </summary>
        CadetBlue = 0xff5f9ea0,

        /// <summary>
        /// Chartreuse color enum.
        /// </summary>
        Chartreuse = 0xff7fff00,

        /// <summary>
        /// Chocolate color enum.
        /// </summary>
        Chocolate = 0xffd2691e,

        /// <summary>
        /// Coral color enum.
        /// </summary>
        Coral = 0xffff7f50,

        /// <summary>
        /// CornflowerBlue color enum.
        /// </summary>
        CornflowerBlue = 0xff6495ed,

        /// <summary>
        /// Corn silk color enum.
        /// </summary>
        Cornsilk = 0xfffff8dc,

        /// <summary>
        /// Crimson color enum.
        /// </summary>
        Crimson = 0xffdc143c,

        /// <summary>
        /// Cyan color enum.
        /// </summary>
        Cyan = 0xff00ffff,

        /// <summary>
        /// DarkBlue color enum.
        /// </summary>
        DarkBlue = 0xff00008b,

        /// <summary>
        /// DarkCyan color enum.
        /// </summary>
        DarkCyan = 0xff008b8b,

        /// <summary>
        /// DarkGoldenrod color enum.
        /// </summary>
        DarkGoldenrod = 0xffb8860b,

        /// <summary>
        /// DarkGray color enum.
        /// </summary>
        DarkGray = 0xffa9a9a9,

        /// <summary>
        /// DarkGreen color enum.
        /// </summary>
        DarkGreen = 0xff006400,

        /// <summary>
        /// DarkKhaki color enum.
        /// </summary>
        DarkKhaki = 0xffbdb76b,

        /// <summary>
        /// DarkMagenta color enum.
        /// </summary>
        DarkMagenta = 0xff8b008b,

        /// <summary>
        /// DarkOliveGreen color enum.
        /// </summary>
        DarkOliveGreen = 0xff556b2f,

        /// <summary>
        /// DarkOrange color enum.
        /// </summary>
        DarkOrange = 0xffff8c00,

        /// <summary>
        /// DarkOrchid color enum.
        /// </summary>
        DarkOrchid = 0xff9932cc,

        /// <summary>
        /// DarkRed color enum.
        /// </summary>
        DarkRed = 0xff8b0000,

        /// <summary>
        /// DarkSalmon color enum.
        /// </summary>
        DarkSalmon = 0xffe9967a,

        /// <summary>
        /// DarkSeaGreen color enum.
        /// </summary>
        DarkSeaGreen = 0xff8fbc8f,

        /// <summary>
        /// DarkSlateBlue color enum.
        /// </summary>
        DarkSlateBlue = 0xff483d8b,

        /// <summary>
        /// DarkSlateGray color enum.
        /// </summary>
        DarkSlateGray = 0xff2f4f4f,

        /// <summary>
        /// DarkTurquoise color enum.
        /// </summary>
        DarkTurquoise = 0xff00ced1,

        /// <summary>
        /// DarkViolet color enum.
        /// </summary>
        DarkViolet = 0xff9400d3,

        /// <summary>
        /// DeepPink color enum.
        /// </summary>
        DeepPink = 0xffff1493,

        /// <summary>
        /// DeepSkyBlue color enum.
        /// </summary>
        DeepSkyBlue = 0xff00bfff,

        /// <summary>
        /// DimGray color enum.
        /// </summary>
        DimGray = 0xff696969,

        /// <summary>
        /// DodgerBlue color enum.
        /// </summary>
        DodgerBlue = 0xff1e90ff,

        /// <summary>
        /// Firebrick color enum.
        /// </summary>
        Firebrick = 0xffb22222,

        /// <summary>
        /// FloralWhite color enum.
        /// </summary>
        FloralWhite = 0xfffffaf0,

        /// <summary>
        /// ForestGreen color enum.
        /// </summary>
        ForestGreen = 0xff228b22,

        /// <summary>
        /// Fuchsia color enum.
        /// </summary>
        Fuchsia = 0xffff00ff,

        /// <summary>
        /// Gainesboro color enum.
        /// </summary>
        Gainsboro = 0xffdcdcdc,

        /// <summary>
        /// GhostWhite color enum.
        /// </summary>
        GhostWhite = 0xfff8f8ff,

        /// <summary>
        /// Gold color enum.
        /// </summary>
        Gold = 0xffffd700,

        /// <summary>
        /// Goldenrod color enum.
        /// </summary>
        Goldenrod = 0xffdaa520,

        /// <summary>
        /// Gray color enum.
        /// </summary>
        Gray = 0xff808080,

        /// <summary>
        /// Green color enum.
        /// </summary>
        Green = 0xff008000,

        /// <summary>
        /// GreenYellow color enum.
        /// </summary>
        GreenYellow = 0xffadff2f,

        /// <summary>
        /// Honeydew color enum.
        /// </summary>
        Honeydew = 0xfff0fff0,

        /// <summary>
        /// HotPink color enum.
        /// </summary>
        HotPink = 0xffff69b4,

        /// <summary>
        /// IndianRed color enum.
        /// </summary>
        IndianRed = 0xffcd5c5c,

        /// <summary>
        /// Indigo color enum.
        /// </summary>
        Indigo = 0xff4b0082,

        /// <summary>
        /// Ivory color enum.
        /// </summary>
        Ivory = 0xfffffff0,

        /// <summary>
        /// Khaki color enum.
        /// </summary>
        Khaki = 0xfff0e68c,

        /// <summary>
        /// Lavender color enum.
        /// </summary>
        Lavender = 0xffe6e6fa,

        /// <summary>
        /// LavenderBlush color enum.
        /// </summary>
        LavenderBlush = 0xfffff0f5,

        /// <summary>
        /// LawnGreen color enum.
        /// </summary>
        LawnGreen = 0xff7cfc00,

        /// <summary>
        /// LemonChiffon color enum.
        /// </summary>
        LemonChiffon = 0xfffffacd,

        /// <summary>
        /// LightBlue color enum.
        /// </summary>
        LightBlue = 0xffadd8e6,

        /// <summary>
        /// LightCoral color enum.
        /// </summary>
        LightCoral = 0xfff08080,

        /// <summary>
        /// LightCyan color enum.
        /// </summary>
        LightCyan = 0xffe0ffff,

        /// <summary>
        /// LightGoldenrodYellow color enum.
        /// </summary>
        LightGoldenrodYellow = 0xfffafad2,

        /// <summary>
        /// LightGray color enum.
        /// </summary>
        LightGray = 0xffd3d3d3,

        /// <summary>
        /// LightGreen color enum.
        /// </summary>
        LightGreen = 0xff90ee90,

        /// <summary>
        /// LightPink color enum.
        /// </summary>
        LightPink = 0xffffb6c1,

        /// <summary>
        /// LightSalmon color enum.
        /// </summary>
        LightSalmon = 0xffffa07a,

        /// <summary>
        /// LightSeaGreen color enum.
        /// </summary>
        LightSeaGreen = 0xff20b2aa,

        /// <summary>
        /// LightSkyBlue color enum.
        /// </summary>
        LightSkyBlue = 0xff87cefa,

        /// <summary>
        /// LightSlateGray color enum.
        /// </summary>
        LightSlateGray = 0xff778899,

        /// <summary>
        /// LightSteelBlue color enum.
        /// </summary>
        LightSteelBlue = 0xffb0c4de,

        /// <summary>
        /// LightYellow color enum.
        /// </summary>
        LightYellow = 0xffffffe0,

        /// <summary>
        /// Lime color enum.
        /// </summary>
        Lime = 0xff00ff00,

        /// <summary>
        /// LimeGreen color enum.
        /// </summary>
        LimeGreen = 0xff32cd32,

        /// <summary>
        /// Linen color enum.
        /// </summary>
        Linen = 0xfffaf0e6,

        /// <summary>
        /// Magenta color enum.
        /// </summary>
        Magenta = 0xffff00ff,

        /// <summary>
        /// Maroon color enum.
        /// </summary>
        Maroon = 0xff800000,

        /// <summary>
        /// MediumAquamarine color enum.
        /// </summary>
        MediumAquamarine = 0xff66cdaa,

        /// <summary>
        /// MediumBlue color enum.
        /// </summary>
        MediumBlue = 0xff0000cd,

        /// <summary>
        /// MediumOrchid color enum.
        /// </summary>
        MediumOrchid = 0xffba55d3,

        /// <summary>
        /// MediumPurple color enum.
        /// </summary>
        MediumPurple = 0xff9370db,

        /// <summary>
        /// MediumSeaGreen color enum.
        /// </summary>
        MediumSeaGreen = 0xff3cb371,

        /// <summary>
        /// MediumSlateBlue color enum.
        /// </summary>
        MediumSlateBlue = 0xff7b68ee,

        /// <summary>
        /// MediumSpringGreen color enum.
        /// </summary>
        MediumSpringGreen = 0xff00fa9a,

        /// <summary>
        /// MediumTurquoise color enum.
        /// </summary>
        MediumTurquoise = 0xff48d1cc,

        /// <summary>
        /// MediumVioletRed color enum.
        /// </summary>
        MediumVioletRed = 0xffc71585,

        /// <summary>
        /// MidnightBlue color enum.
        /// </summary>
        MidnightBlue = 0xff191970,

        /// <summary>
        /// MintCream color enum.
        /// </summary>
        MintCream = 0xfff5fffa,

        /// <summary>
        /// MistyRose color enum.
        /// </summary>
        MistyRose = 0xffffe4e1,

        /// <summary>
        /// Moccasin color enum.
        /// </summary>
        Moccasin = 0xffffe4b5,

        /// <summary>
        /// NavajoWhite color enum.
        /// </summary>
        NavajoWhite = 0xffffdead,

        /// <summary>
        /// Navy color enum.
        /// </summary>
        Navy = 0xff000080,

        /// <summary>
        /// OldLace color enum.
        /// </summary>
        OldLace = 0xfffdf5e6,

        /// <summary>
        /// Olive color enum.
        /// </summary>
        Olive = 0xff808000,

        /// <summary>
        /// OliveDrab color enum.
        /// </summary>
        OliveDrab = 0xff6b8e23,

        /// <summary>
        /// Orange color enum.
        /// </summary>
        Orange = 0xffffa500,

        /// <summary>
        /// OrangeRed color enum.
        /// </summary>
        OrangeRed = 0xffff4500,

        /// <summary>
        /// Orchid color enum.
        /// </summary>
        Orchid = 0xffda70d6,

        /// <summary>
        /// PaleGoldenrod color enum.
        /// </summary>
        PaleGoldenrod = 0xffeee8aa,

        /// <summary>
        /// PaleGreen color enum.
        /// </summary>
        PaleGreen = 0xff98fb98,

        /// <summary>
        /// PaleTurquoise color enum.
        /// </summary>
        PaleTurquoise = 0xffafeeee,

        /// <summary>
        /// PaleVioletRed color enum.
        /// </summary>
        PaleVioletRed = 0xffdb7093,

        /// <summary>
        /// PapayaWhip color enum.
        /// </summary>
        PapayaWhip = 0xffffefd5,

        /// <summary>
        /// PeachPuff color enum.
        /// </summary>
        PeachPuff = 0xffffdab9,

        /// <summary>
        /// Peru color enum.
        /// </summary>
        Peru = 0xffcd853f,

        /// <summary>
        /// Pink color enum.
        /// </summary>
        Pink = 0xffffc0cb,

        /// <summary>
        /// Plum color enum.
        /// </summary>
        Plum = 0xffdda0dd,

        /// <summary>
        /// PowderBlue color enum.
        /// </summary>
        PowderBlue = 0xffb0e0e6,

        /// <summary>
        /// Purple color enum.
        /// </summary>
        Purple = 0xff800080,

        /// <summary>
        /// Red color enum.
        /// </summary>
        Red = 0xffff0000,

        /// <summary>
        /// RosyBrown color enum.
        /// </summary>
        RosyBrown = 0xffbc8f8f,

        /// <summary>
        /// RoyalBlue color enum.
        /// </summary>
        RoyalBlue = 0xff4169e1,

        /// <summary>
        /// SaddleBrown color enum.
        /// </summary>
        SaddleBrown = 0xff8b4513,

        /// <summary>
        /// Salmon color enum.
        /// </summary>
        Salmon = 0xfffa8072,

        /// <summary>
        /// SandyBrown color enum.
        /// </summary>
        SandyBrown = 0xfff4a460,

        /// <summary>
        /// SeaGreen color enum.
        /// </summary>
        SeaGreen = 0xff2e8b57,

        /// <summary>
        /// SeaShell color enum.
        /// </summary>
        SeaShell = 0xfffff5ee,

        /// <summary>
        /// Sienna color enum.
        /// </summary>
        Sienna = 0xffa0522d,

        /// <summary>
        /// Silver color enum.
        /// </summary>
        Silver = 0xffc0c0c0,

        /// <summary>
        /// SkyBlue color enum.
        /// </summary>
        SkyBlue = 0xff87ceeb,

        /// <summary>
        /// SlateBlue color enum.
        /// </summary>
        SlateBlue = 0xff6a5acd,

        /// <summary>
        /// SlateGray color enum.
        /// </summary>
        SlateGray = 0xff708090,

        /// <summary>
        /// Snow color enum.
        /// </summary>
        Snow = 0xfffffafa,

        /// <summary>
        /// SpringGreen color enum.
        /// </summary>
        SpringGreen = 0xff00ff7f,

        /// <summary>
        /// SteelBlue color enum.
        /// </summary>
        SteelBlue = 0xff4682b4,

        /// <summary>
        /// Tan color enum.
        /// </summary>
        Tan = 0xffd2b48c,

        /// <summary>
        /// Teal color enum.
        /// </summary>
        Teal = 0xff008080,

        /// <summary>
        /// Thistle color enum.
        /// </summary>
        Thistle = 0xffd8bfd8,

        /// <summary>
        /// Tomato color enum.
        /// </summary>
        Tomato = 0xffff6347,

        /// <summary>
        /// Transparent color enum.
        /// </summary>
        Transparent = 0xffffff,

        /// <summary>
        /// Turquoise color enum.
        /// </summary>
        Turquoise = 0xff40e0d0,

        /// <summary>
        /// UnknownColor color enum.
        /// </summary>
        UnknownColor = 1,

        /// <summary>
        /// Violet color enum.
        /// </summary>
        Violet = 0xffee82ee,

        /// <summary>
        /// Wheat color enum.
        /// </summary>
        Wheat = 0xfff5deb3,

        /// <summary>
        /// White color enum.
        /// </summary>
        White = 0xffffffff,

        /// <summary>
        /// WhiteSmoke color enum.
        /// </summary>
        WhiteSmoke = 0xfff5f5f5,

        /// <summary>
        /// Yellow color enum.
        /// </summary>
        Yellow = 0xffffff00,

        /// <summary>
        /// YellowGreem color enum.
        /// </summary>
        YellowGreen = 0xff9acd32
    }
}