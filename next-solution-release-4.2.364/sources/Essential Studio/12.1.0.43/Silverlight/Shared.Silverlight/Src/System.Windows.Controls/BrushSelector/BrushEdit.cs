#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// ColorSelectionMode is a enum for the Visualization style for the BrushEdit
    /// control.It can be RGB or HSV mode.
    /// </summary>
    public enum ColorSelectionMode
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
    internal enum KnownColor : uint
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
    /// <summary>
    /// BrushEdit and BrushSelector can be of two modes i.e. Solid or Gradient.This can
    /// be set using this enum.
    /// </summary>
    public enum BrushModes
    {
        /// <summary>
        /// SolidColorBrush Mode
        /// </summary>
        Solid,

        /// <summary>
        /// GradientBrush Mode
        /// </summary>
        Gradient
    }

    /// <summary>
    /// BrushEdit and GradientMode can be of two modes i.e. Liner or Radial.This can
    /// be set using this enum.
    /// </summary>
    public enum GradientModes
    {
        /// <summary>
        /// 
        /// </summary>
        Linear,
        /// <summary>
        /// 
        /// </summary>
        Radial
    }
    /// <summary>
    /// Represents a BrushEdit Control, used for picking the Brush in solid or gradient
    /// mode.
    /// </summary>
    /// <example>
    /// <para>The control can be added to the application in the following ways.</para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:BrushEdit Height=&quot;150&quot; 
    /// Width=&quot;300&quot;  Name=&quot;brushedit' VisualizationStyle=&quot;HSV&quot;
    /// SelectedBrush=&quot;BlueViolet&quot; /&gt;
    /// <para>                                </para></description></item></list>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C#</term></listheader>
    /// <item>
    /// <description>BrushEdit brushedit=new BrushEdit;
    /// <para>brushedit.VisualizationStyle=ColorSelectionMode.HSV;</para></description></item></list>
    /// </example>
    /// 
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Blend;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2007Black;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Default;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2003;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
      Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2010Black;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Windows7;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.VS2010;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Metro;component/BrushEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
      Type = typeof(BrushEdit), XamlResource = "/Syncfusion.Theming.Transparent;component/BrushEdit.xaml")]

    public class BrushEdit : Control, IDisposable
    {         
        #region Private and template variables
        private Image eyedrop, activeeyedrop, eyedropper;
        private double x_axis = 0.0, y_axis = 1.0;
        private Color m_selectedColor;
        private float m_selectedHue;
        private ColorSelectionMode m_visualizationStyle = ColorSelectionMode.HSV;
        private Color mcolor;
        private bool mouseDown, coloreditmove = false, disable = true;
        private Color prevcolor = Colors.Black;
        private ToggleButton rblinear, rbradial;
        private Button rbgradient, rbsolid;
        private RadioButton rbhue, rbsat, rbval;
        private Grid gradgrid;
        private bool enableactiveeyedrop, m_bColorUpdating = false, enableSlidervalue = true;
        private Rectangle selectcolor, ColorRect, gradrect, whiteRect, blackRect, huerect;
        private Canvas colorSelector;
        private Grid hsvValgrid, rgbValgrid, maingrid;
        internal Slider hueslider { get; set; }
        internal bool islinear { get; set; }
        internal bool enablehue { get; set; }
        internal GradientCollection items { get; set; }
        internal Canvas gradientBar { get; set; }
        internal bool enableSelectedBrush { get; set; }           
        internal Rectangle SelectedColor { get; set; }
        internal Rectangle CurrentColor { get; set; }
        internal TextBox hexValue { get; set; }
        internal Slider txtS { get; set; }
        internal Slider txtH { get; set; }
        internal Slider txtV { get; set; }
        private ColorConversions m_color;
        private bool isApplyTemplateCalled = false;
        private int gradflag = 0;        

        /// <summary>
        /// Alpha or opacity parameter for RGB model.
        /// </summary>
        private byte m_a;

        /// <summary>
        /// Blue parameter for RGB model.
        /// </summary>
        private byte m_b;
       
        /// <summary>
        /// Green parameter for RGB model.
        /// </summary>
        private byte m_g;

        /// <summary>
        /// Red parameter for RGB model.
        /// </summary>
        private byte m_r;      
        
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Alpha dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty AProperty =
            DependencyProperty.Register("A", typeof(float), typeof(BrushEdit), new PropertyMetadata(255f, new PropertyChangedCallback(OnAChanged)));
        /// <summary>
        /// Identifies the ColorText Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty ColorTextProperty =
           DependencyProperty.Register("ColorText", typeof(string), typeof(BrushEdit), new PropertyMetadata("Unknown"));
        /// <summary>
        /// Identifies the ColorName Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty ColorNameProperty =
         DependencyProperty.Register("ColorName", typeof(string), typeof(BrushEdit), new PropertyMetadata("Unknown"));

        /// <summary>
        /// BackgroundA dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </returns>
        internal static readonly DependencyProperty BackgroundAProperty =
            DependencyProperty.Register("BackgroundA", typeof(Brush), typeof(BrushEdit), new PropertyMetadata(null));

        /// <summary>
        /// BackgroundB dependency property.
        /// </summary>
        /// <value>
        /// Type:<see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </value>
        internal static readonly DependencyProperty BackgroundBProperty =
            DependencyProperty.Register("BackgroundB", typeof(Brush), typeof(BrushEdit), new PropertyMetadata(null));

        /// <summary>
        /// BackgroundG dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </value>
        internal static readonly DependencyProperty BackgroundGProperty =
            DependencyProperty.Register("BackgroundG", typeof(Brush), typeof(BrushEdit), new PropertyMetadata(null));

        /// <summary>
        /// BackgroundR dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </value>
        internal static readonly DependencyProperty BackgroundRProperty =
            DependencyProperty.Register("BackgroundR", typeof(Brush), typeof(BrushEdit), new PropertyMetadata(null));

        /// <summary>
        /// Blue dependency property.
        /// </summary>
        /// <returns>
        /// Type: <see cref="T:System.Decimal">float</see>
        /// </returns>
        internal static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B", typeof(float), typeof(BrushEdit), new PropertyMetadata(1f, new PropertyChangedCallback(OnBChanged)));

        /// <summary>
        /// Green dependency property.
        /// </summary>
        /// <returns>
        /// Type: <see cref="T:System.Decimal">float</see>
        /// </returns>
        internal static readonly DependencyProperty GProperty =
            DependencyProperty.Register("G", typeof(float), typeof(BrushEdit), new PropertyMetadata(1f, new PropertyChangedCallback(OnGChanged)));

        /// <summary>
        /// Gradient Height dependency property.
        /// </summary>
        /// <returns>
        /// Type: <see cref="T:System.Double">Double</see>
        /// </returns>
        internal static readonly DependencyProperty GradientHeightProperty = DependencyProperty.Register("GradientHeight", typeof(double), typeof(BrushEdit), new PropertyMetadata(0.0, null));

        /// <summary>
        /// Gradient width dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Double">Double</see>
        /// </value>
        internal static readonly DependencyProperty GradientWidthProperty = DependencyProperty.Register("GradientWidth", typeof(double), typeof(BrushEdit), new PropertyMetadata(0.0, null));
       
        /// <summary>
        /// Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Color">Color</see>
        /// </value>
        internal static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(BrushEdit), new PropertyMetadata(Colors.White, new PropertyChangedCallback(OnColorChanged)));

        /// <summary>
        /// Identifies ColorPicker. Red dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Decimal">float</see>
        /// </value>
        internal static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R", typeof(float), typeof(BrushEdit), new PropertyMetadata(1f, new PropertyChangedCallback(OnRChanged)));
        
        /// <summary>
        /// Hue Slider Height dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Double">Double</see>
        /// </value>
        internal static readonly DependencyProperty SliderHeightProperty = DependencyProperty.Register("SliderHeight", typeof(double), typeof(BrushEdit), new PropertyMetadata(0.0, null));

        /// <summary>
        /// Hue Slider width dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Double">Double</see>
        /// </value>
        internal static readonly DependencyProperty SliderWidthProperty = DependencyProperty.Register("SliderWidth", typeof(double), typeof(BrushEdit), new PropertyMetadata(20.0, null));
        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushEdit">BrushEdit</see> class
        /// </summary>
        public BrushEdit()
        {
            this.DefaultStyleKey = typeof(BrushEdit);
        }

        static BrushEdit()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }

        }
        #endregion

        /// <summary>
        /// Represenst the ColorSelected Handler.
        /// </summary>
        public delegate void ColorSelectedHandler(Color c);

        

        #region Events
        /// <summary>
        /// Event that is raised when SelectedBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedBrushChanged;

        /// <summary>
        /// Event that is raised when A property is changed.
        /// </summary>
        internal event PropertyChangedCallback AChanged;

        /// <summary>
        /// Event that is raised when B property is changed.
        /// </summary>        
        internal event PropertyChangedCallback BChanged;

        /// <summary>
        /// Event that is raised when Brushmode property is changed.
        /// </summary>
        public event PropertyChangedCallback BrushModeChanged;

        /// <summary>
        /// Event that is raised when Color property is changed.
        /// </summary>
        public event RoutedEventHandler ColorChanged;

        /// <summary>
        /// Event that is raised when ColorSelected property is changed.
        /// </summary>
        public event ColorSelectedHandler ColorSelected;

        /// <summary>
        /// Event that is raised when EnableGradientToSolidSwitch property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableGradientToSolidSwitchChanged;

        /// <summary>
        /// Event that is raised when G property is changed.
        /// </summary>
        internal event PropertyChangedCallback GChanged;

        /// <summary>
        /// Event that is raised when R property is changed.
        /// </summary>
        internal event PropertyChangedCallback RChanged;

        /// <summary>
        /// Event that is raised when VisualizationStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback VisualizationStyleChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Alpha or opacity parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Alpha.
        /// </value>
        internal float A
        {
            get
            {
                return m_a;
            }

            set
            {
                SetValue(AProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the name of the color.
        /// </summary>
        /// <value>The name of the color.</value>
        /// 

        internal string ColorName
        {
            get
            {
                return (string)GetValue(ColorNameProperty);
            }

            set
            {
                SetValue(ColorNameProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the color text.
        /// </summary>
        /// <value>The color text.</value>
        internal string ColorText
        {
            get
            {
                return (string)GetValue(ColorTextProperty);
            }

            set
            {
                SetValue(ColorTextProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the Blue parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Blue.
        /// </value>
        internal float B
        {
            get
            {
                return m_b;
            }

            set
            {
                SetValue(BProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Blue slider for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Transparent.
        /// </value>
        internal Brush BackgroundA
        {
            get
            {
                return (Brush)GetValue(BackgroundAProperty);
            }

            set
            {
                SetValue(BackgroundAProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Blue slider for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Blue.
        /// </value>
        internal Brush BackgroundB
        {
            get
            {
                return (Brush)GetValue(BackgroundBProperty);
            }

            set
            {
                SetValue(BackgroundBProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Green slider for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Green.
        /// </value>
        internal Brush BackgroundG
        {
            get
            {
                return (Brush)GetValue(BackgroundGProperty);
            }

            set
            {
                SetValue(BackgroundGProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Red slider for RGB model..
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Red.
        /// </value>
        internal Brush BackgroundR
        {
            get
            {
                return (Brush)GetValue(BackgroundRProperty);
            }

            set
            {
                SetValue(BackgroundRProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the GradientMode dependency property.
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.GradientModes">GradientModes</see>
        /// </value>
        public GradientModes GradientMode
        {
            get 
            {
                return (GradientModes)GetValue(GradientModeProperty);
            }
            set 
            {
                SetValue(GradientModeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the value of the BrushMode dependency property.
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushModes">BrushModes</see>
        /// </value>
        public BrushModes BrushMode
        {
            get
            {
                return (BrushModes)GetValue(BrushModeProperty);
            }

            set
            {
                SetValue(BrushModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        internal Color Color
        {
            get
            {
                return mcolor;
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableGradientToSolidSwitch dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Boolean">bool </see>
        /// </value>
        public bool EnableGradientToSolidSwitch
        {
            get
            {
                return (bool)GetValue(EnableGradientToSolidSwitchProperty);
            }

            set
            {
                SetValue(EnableGradientToSolidSwitchProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Green parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Green.
        /// </value>
        internal float G
        {
            get
            {
                return m_g;
            }

            set
            {
                SetValue(GProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Height of the Gradient Rectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/> 
        /// Red.
        /// </value>
        internal double GradientHeight
        {
            get
            {
                return (double)GetValue(GradientHeightProperty);
            }

            set
            {
                SetValue(GradientHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Width of the Gradient Rectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/> 
        /// Red.
        /// </value>
        internal double GradientWidth
        {
            get
            {
                return (double)GetValue(GradientWidthProperty);
            }

            set
            {
                SetValue(GradientWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Red parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/> 
        /// Red.
        /// </value>
        /// <property name="flag" value="Finished"/>
        internal float R
        {
            get
            {
                return m_r;
            }

            set
            {
                SetValue(RProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Selected Background dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush SelectedBackground
        {
            get
            {
                return (Brush)GetValue(SelectedBackgroundProperty);
            }

            set
            {
                SetValue(SelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Selected Brush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </value>
        public Brush SelectedBrush
        {
            get
            {
                return (Brush)GetValue(SelectedBrushProperty);
            }

            set
            {
                SetValue(SelectedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Height of the Slider Rectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/> 
        /// Red.
        /// </value>
        internal double SliderHeight
        {
            get
            {
                return (double)GetValue(SliderHeightProperty);
            }

            set
            {
                SetValue(SliderHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Width of the Slider Rectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/> 
        /// Red.
        /// </value>
        internal double SliderWidth
        {
            get
            {
                return (double)GetValue(SliderWidthProperty);
            }

            set
            {
                SetValue(SliderWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Visualization dependency property.
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.ColorSelectionMode">ColorSelectionMode</see>
        /// </value>
        public ColorSelectionMode VisualizationStyle
        {
            get
            {
                return m_visualizationStyle;
            }

            set
            {
                SetValue(VisualizationStyleProperty, value);
            }
        }
        
        #endregion

        #region Methods
        /// <summary>
        /// Handles the LayoutUpdated event of the BrushEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void BrushEdit_LayoutUpdated(object sender, EventArgs e)
        {
            if (double.IsNaN(this.Width))
            {
                if (this.ActualWidth < 275)
                {
                    this.Width = 275;
                }
                else
                {
                    this.Width = this.ActualWidth;
                }
            }
            else
            {
                if (this.Width < 275)
                {
                    this.Width = 275;
                }
                else
                {
                    this.Width = this.ActualWidth;
                }
            }
            if (double.IsNaN(this.Height))
            {
                if (this.ActualHeight < 200)
                {
                    this.Height = 200;
                }
                else
                {
                    this.Height = this.ActualHeight;
                }

              
            }
            else
            {
                if (this.Height < 200)
                {
                    this.Height = 200;
                }                    
            }
            if (gradflag == 1)
            {
                this.GradientHeight = this.Height - 95;
                this.SliderHeight = this.Height - 58;
            }
            else
            {
                this.GradientHeight = this.Height - 70;
                this.SliderHeight = this.Height - 33;
            }

            this.GradientWidth = (this.Width / 2) - 25;
            this.SliderWidth = 20;
            hexValue.Width = 2.5 * (this.Width / 2) / 7;
            UpdateColor();
            //RefreshBrushEdit(x_axis, y_axis);

            //OnApplyTemplate();
        }
        /// <summary>
        /// Called when parameter A is changed.
        /// </summary>
        /// <param name="e">The  routed event args  <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnAChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToInt16(e.NewValue) >= 0 && Convert.ToInt16(e.NewValue) <= 255)
            {
                if (m_a != Convert.ToByte(e.NewValue))
                {
                    m_a = Convert.ToByte(e.NewValue);
                    if (AChanged != null)
                    {                        
                        AChanged(this, e);
                    }

                    if (!m_bColorUpdating)
                    {
                        UpdateColor();
                    }

                    m_bColorUpdating = false;
                }
            }
        }

        /// <summary>
        /// Called when parameter A is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnAChanged(e);
        }
        Popup pop;

       

        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.LayoutUpdated -= new EventHandler(BrushEdit_LayoutUpdated);
            this.MouseMove -= new MouseEventHandler(BrushEdit_MouseMove);
            this.MouseLeftButtonUp -= new MouseButtonEventHandler(selectcolor_MouseLeftButtonUp);
            this.Loaded -= new RoutedEventHandler(BrushEdit_Loaded);
            if (hexValue != null)
            {
                hexValue.LostFocus -= new RoutedEventHandler(HexValue_LostFocus);
                hexValue.MouseMove -= new MouseEventHandler(HexValue_MouseMove);
            }
            if (hueslider != null)
            {
                hueslider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(hueslider_ValueChanged);
            }
            if (huerect != null)
            {
                huerect.MouseLeftButtonDown -= new MouseButtonEventHandler(huerect_MouseLeftButtonDown);
                huerect.MouseLeftButtonUp -= new MouseButtonEventHandler(huerect_MouseLeftButtonUp);
                huerect.MouseMove -= new MouseEventHandler(huerect_MouseMove);
            }
            if (eyedrop != null)
            {
                eyedrop.MouseLeftButtonDown -= new MouseButtonEventHandler(eyedrop_MouseLeftButtonDown);
                eyedrop.MouseLeftButtonUp -= new MouseButtonEventHandler(eyedrop_MouseLeftButtonUp);
            }
            if (rblinear != null)
            {
                rblinear.Click -= new RoutedEventHandler(rblinear_Click);
            }
            if (rbradial != null)
            {
                rbradial.Click -= new RoutedEventHandler(rblinear_Click);
            }
            if (rbgradient != null)
            {
                rbgradient.Click -= new RoutedEventHandler(rbgradient_Click);
                rbgradient.MouseMove -= new MouseEventHandler(rbsolid_MouseMove);
            }
            if (rbsolid != null)
            {
                rbsolid.Click -= new RoutedEventHandler(rbgradient_Click);
                rbsolid.MouseMove -= new MouseEventHandler(rbsolid_MouseMove);
            }
           
            if (gradrect != null)
            {
                gradrect.MouseLeftButtonDown -= new MouseButtonEventHandler(gradrect_MouseLeftButtonDown);
            }
            if (selectcolor != null)
            {
                selectcolor.MouseLeftButtonDown -= new MouseButtonEventHandler(selectcolor_MouseLeftButtonDown);
                selectcolor.MouseEnter -= new MouseEventHandler(selectcolor_MouseEnter);
                selectcolor.MouseLeftButtonUp -= new MouseButtonEventHandler(selectcolor_MouseLeftButtonUp);
                selectcolor.MouseMove -= new MouseEventHandler(selectcolor_MouseMove);
                selectcolor.MouseLeave -= new MouseEventHandler(selectcolor_MouseLeave);
            }
            base.OnApplyTemplate();
            isApplyTemplateCalled = true;
            m_color = new ColorConversions();
            maingrid = this.GetTemplateChild("MainGrid") as Grid;
            hsvValgrid = this.GetTemplateChild("HSVValue") as Grid;
            rgbValgrid = this.GetTemplateChild("RGBValue") as Grid;
            gradgrid = this.GetTemplateChild("GradientGrid") as Grid;
            selectcolor = this.GetTemplateChild("ColorSelectRect") as Rectangle;
            huerect = this.GetTemplateChild("HueRect") as Rectangle;
            colorSelector = this.GetTemplateChild("ColorSelector") as Canvas;           
            ColorRect = this.GetTemplateChild("ColorRect") as Rectangle;
            whiteRect = this.GetTemplateChild("WhiteGradientRect") as Rectangle;
            blackRect = this.GetTemplateChild("BlackGradientRect") as Rectangle;
            SelectedColor = this.GetTemplateChild("SelectedColor") as Rectangle;
            CurrentColor = this.GetTemplateChild("CurrentColor") as Rectangle;
            hexValue = this.GetTemplateChild("HexValue") as TextBox;
            hueslider = this.GetTemplateChild("HueSlider") as Slider;
            eyedrop = this.GetTemplateChild("EyeDrop") as Image;    
            activeeyedrop = this.GetTemplateChild("EyeDropColor") as Image;
            eyedropper = this.GetTemplateChild("EyeDropper") as Image;
            gradientBar = this.GetTemplateChild("GradientBar") as Canvas;
            gradrect = this.GetTemplateChild("GradRect") as Rectangle;
            rblinear = this.GetTemplateChild("linear") as ToggleButton;
            rbradial = this.GetTemplateChild("radial") as ToggleButton;           
            rbgradient = this.GetTemplateChild("gradient") as Button;
            pop = GetTemplateChild("pop") as Popup;
            rbsolid = this.GetTemplateChild("solid") as Button;
            hexValue.LostFocus += new RoutedEventHandler(HexValue_LostFocus);
            hexValue.MouseMove += new MouseEventHandler(HexValue_MouseMove);
            UpdateVisualizationStyle(this.VisualizationStyle);
            this.MouseMove += new MouseEventHandler(BrushEdit_MouseMove);
            hueslider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(hueslider_ValueChanged);
            huerect.MouseLeftButtonDown += new MouseButtonEventHandler(huerect_MouseLeftButtonDown);
            huerect.MouseLeftButtonUp += new MouseButtonEventHandler(huerect_MouseLeftButtonUp);
            eyedrop.MouseLeftButtonDown += new MouseButtonEventHandler(eyedrop_MouseLeftButtonDown);
            eyedrop.MouseLeftButtonUp += new MouseButtonEventHandler(eyedrop_MouseLeftButtonUp);
            huerect.MouseMove += new MouseEventHandler(huerect_MouseMove);
            rblinear.Click += new RoutedEventHandler(rblinear_Click);
            rbradial.Click += new RoutedEventHandler(rblinear_Click);            
            rbgradient.Click += new RoutedEventHandler(rbgradient_Click);
            rbsolid.Click += new RoutedEventHandler(rbgradient_Click);
            rbsolid.MouseMove += new MouseEventHandler(rbsolid_MouseMove);
            rbgradient.MouseMove += new MouseEventHandler(rbsolid_MouseMove);           
            gradrect.MouseLeftButtonDown += new MouseButtonEventHandler(gradrect_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(selectcolor_MouseLeftButtonUp);
            selectcolor.MouseLeftButtonDown += new MouseButtonEventHandler(selectcolor_MouseLeftButtonDown);
            selectcolor.MouseEnter+=new MouseEventHandler(selectcolor_MouseEnter);
            selectcolor.MouseLeftButtonUp += new MouseButtonEventHandler(selectcolor_MouseLeftButtonUp);
            selectcolor.MouseMove += new MouseEventHandler(selectcolor_MouseMove);
            selectcolor.MouseLeave += new MouseEventHandler(selectcolor_MouseLeave);
      
            this.MouseMove += new MouseEventHandler(BrushEdit_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(BrushEdit_MouseLeftButtonUp);

            //this.LayoutUpdated += new EventHandler(BrushEdit_LayoutUpdated);

            
            

            items = new GradientCollection();
            //if (this.SelectedBrush != null)
            //{
            //    if (this.SelectedBrush.GetType().ToString().Contains("Solid"))
            //    {
            //        this.BrushMode = BrushModes.Solid;
            //    }
            //    else
            //    {
            //        this.BrushMode = BrushModes.Gradient;
            //        if (this.SelectedBrush.GetType().ToString().Contains("Linear"))
            //        {
            //            islinear = true;
            //        }
            //    }
            //}

            this.GradientChange();
            if (this.EnableGradientToSolidSwitch)
            {
                this.rbsolid.Visibility = Visibility.Visible;
                this.rbgradient.Visibility = Visibility.Visible;
            }
            else
            {
                this.rbsolid.Visibility = Visibility.Collapsed;
                this.rbgradient.Visibility = Visibility.Collapsed;
            }
            if(CurrentColor!=null && prevcolor!=null)
            CurrentColor.Fill = new SolidColorBrush(prevcolor);
            UpdateColor();
            RefreshBrushEdit(x_axis, y_axis);
            this.Loaded += new RoutedEventHandler(BrushEdit_Loaded);
            this.Unloaded += new RoutedEventHandler(BrushEdit_Unloaded);
        }

        void BrushEdit_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Handles the MouseEnter event of the selectcolor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void selectcolor_MouseEnter(object sender, MouseEventArgs e)
        {
            pop.IsOpen = true;
            
        }

        /// <summary>
        /// Handles the MouseLeave event of the selectcolor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void selectcolor_MouseLeave(object sender, MouseEventArgs e)
        {
            pop.IsOpen = false;
        }

       


        /// <summary>
        /// Called when parameter B is changed.
        /// </summary>
        /// <param name="e">The  instance containing the event data.</param>
        private void OnBChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToInt16(e.NewValue) >= 0 && Convert.ToInt16(e.NewValue) <= 255)
            {
                if (m_b != Convert.ToByte(e.NewValue))
                {
                    m_b = Convert.ToByte(e.NewValue);
                    if (BChanged != null)
                    {
                        BChanged(this, e);
                    }

                    if (!m_bColorUpdating)
                    {
                        UpdateColor();
                    }

                    m_bColorUpdating = false;
                }
            }
        }

        /// <summary>
        /// Called when parameter B is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnBChanged(e);
        }

        /// <summary>
        /// Calls OnGradientModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGradientModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            if (instance.GradientMode == GradientModes.Linear)
            {
                instance.islinear = true;
            }
            else
            {
                instance.islinear = false;
            }
        }
        /// <summary>
        /// Calls OnBrushModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBrushModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnBrushModeChanged(e);
            if (instance.gradgrid != null)
            {
                if (instance.BrushMode == BrushModes.Gradient)
                {
                    if (instance.items.SelectedItem != null)
                    {
                        instance.items.SelectedItem.color = ((SolidColorBrush)instance.SelectedBrush).Color;
                    }

                    instance.gradgrid.Visibility = Visibility.Visible;
                    instance.enableSelectedBrush = true;
                    instance.GradientChange();
                    instance.enableSelectedBrush = false;
                }
                else
                {
                    instance.gradgrid.Visibility = Visibility.Collapsed;
                    if (instance.items.SelectedItem != null && !instance.SelectedBrush.GetType().ToString().Contains("Solid"))
                    {
                        instance.SelectedBrush = new SolidColorBrush(instance.items.SelectedItem.color);
                    }
                }
            }
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        private void OnBrushModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BrushModeChanged != null)
            {
                BrushModeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnColorChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnColorChanged(e);
        }

        /// <summary>
        /// Called when color is changed
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            mcolor = (Color)e.NewValue;
            m_bColorUpdating = true;
            m_r = mcolor.R;
            m_g = mcolor.G;
            m_b = mcolor.B;
            m_a = mcolor.A;
            R = mcolor.R;
            G = mcolor.G;
            B = mcolor.B;
            A = mcolor.A;
            if (VisualizationStyle == ColorSelectionMode.RGB)
            {
                CalculateBackground();
            }

            if (ColorChanged != null)
            {
                ColorChanged(this, new ColorSelectionEventArgs() { NewValue = e.NewValue, OldValue = e.OldValue });
            }

            m_bColorUpdating = false;
            if (this.BrushMode == BrushModes.Solid)
            {
                this.SelectedBrush = new SolidColorBrush(Color);
            }
            else
            {
                if (this.items != null)
                {
                    if (this.items.SelectedItem != null)
                    {
                        this.items.SelectedItem.color = Color;
                    }
                    else
                    {
                        this.items.SelectedItem = new GradientStops(Colors.Transparent, true, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        private void OnEnableGradientToSolidSwitchChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableGradientToSolidSwitchChanged != null)
            {
                EnableGradientToSolidSwitchChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEnableGradientToSolidSwitchChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableGradientToSolidSwitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnEnableGradientToSolidSwitchChanged(e);
            if (e.NewValue != e.OldValue)
            {
                if (instance.rbsolid != null)
                {
                    if (instance.EnableGradientToSolidSwitch)
                    {
                        instance.rbsolid.Visibility = Visibility.Visible;
                        instance.rbgradient.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        instance.rbsolid.Visibility = Visibility.Collapsed;
                        instance.rbgradient.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Called when parameter G is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnGChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnGChanged(e);
        }

        /// <summary>
        /// Called when parameter G is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnGChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToInt16(e.NewValue) >= 0 && Convert.ToInt16(e.NewValue) <= 255)
            {
                if (m_g != Convert.ToByte(e.NewValue))
                {
                    m_g = Convert.ToByte(e.NewValue);
                    if (GChanged != null)
                    {
                        GChanged(this, e);
                    }

                    if (!m_bColorUpdating)
                    {
                        UpdateColor();
                    }

                    m_bColorUpdating = false;
                }
            }
        }

        /// <summary>
        /// Called when parameter R is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnRChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToInt16(e.NewValue) >= 0 && Convert.ToInt16(e.NewValue) <= 255)
            {
                if (m_r != Convert.ToByte(e.NewValue))
                {
                    m_r = Convert.ToByte(e.NewValue);
                    if (RChanged != null)
                    {
                        RChanged(this, e);
                    }

                    if (!m_bColorUpdating)
                    {
                        UpdateColor();
                    }

                    m_bColorUpdating = false;
                }
            }
        }

        /// <summary>
        /// Called when parameter R is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnRChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnRChanged(e);
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        private void OnSelectedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedBrushChanged != null)
            {
                SelectedBrushChanged(this, e);
            }
            if (this.SelectedBrush != null)
            {
                if (this.SelectedBrush.GetType().ToString().Contains("Solid"))
                {
                    this.Color = (this.SelectedBrush as SolidColorBrush).Color;
                    this.BrushMode = BrushModes.Solid;
                    if (this.m_selectedColor != this.Color)
                    {
                        this.m_selectedColor = this.Color;
                        UpdateColor();
                    }
                }
                else
                {
                    if (this.items != null)
                    {
                        if (this.items.SelectedItem == null)
                        {
                            this.Color = Colors.Red;
                        }
                        else
                        {
                            this.Color = this.items.SelectedItem.color;
                        }

                        if (this.gradrect != null)
                        {
                            this.gradrect.Fill = this.SelectedBrush;
                        }
                    }

                    this.BrushMode = BrushModes.Gradient;
                    enableSelectedBrush = false;
                    GradientChange();
                    if (ColorChanged != null)
                    {
                        ColorChanged(this, new ColorSelectionEventArgs() { NewValue = e.NewValue, OldValue = e.OldValue });
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnSelectedBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnSelectedBrushChanged(e);
        }

        /// <summary>
        /// Calls OnVisualizationStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVisualizationStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushEdit instance = (BrushEdit)d;
            instance.OnVisualizationStyleChanged(e);
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        private void OnVisualizationStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            m_visualizationStyle = (ColorSelectionMode)e.NewValue;
            if (VisualizationStyleChanged != null)
            {
                VisualizationStyleChanged(this, e);
            }

            if (isApplyTemplateCalled && e.OldValue != e.NewValue)
            {
                UpdateVisualizationStyle((ColorSelectionMode)e.NewValue);
            }
        }

        #endregion

        #region Event Methods
        /// <summary>
        /// Handles the Loaded event of the BrushEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BrushEdit_Loaded(object sender, RoutedEventArgs e)
        {
            if (double.IsNaN(this.Width))
            {
                if (this.ActualWidth < 275)
                {
                    this.Width = 275;
                }
                else
                {
                    this.Width = this.ActualWidth;
                }
            }
            else
            {
                if (this.Width < 275)
                {
                    this.Width = 275;
                }
             
            }
            if (double.IsNaN(this.Height))
            {
                if (this.ActualHeight < 200)
                {
                    this.Height = 200;
                }
                else
                {
                    this.Height = this.ActualHeight;
                }
            }
            else
            {
                if (this.Height < 200)
                {
                    this.Height = 200;
                }
              
               
            }
         
            this.GradientHeight = this.Height - 60;          
            this.GradientWidth = (this.Width / 2) - 30;
            this.SliderWidth = 20;
            this.SliderHeight = this.Height-25;
            hexValue.Width = 2.5 * (this.Width / 2) / 7;
            UpdateColor();
            RefreshBrushEdit(x_axis, y_axis);
            Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
            Application.Current.RootVisual.MouseLeftButtonUp += new MouseButtonEventHandler(RootVisual_MouseLeftButtonUp);
            this.LayoutUpdated -= new EventHandler(BrushEdit_LayoutUpdated);
            this.LayoutUpdated += new EventHandler(BrushEdit_LayoutUpdated);

        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the BrushEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void BrushEdit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            activeeyedrop.Visibility = Visibility.Collapsed;
            this.Cursor = Cursors.Arrow;
            enableactiveeyedrop = false;
            enablehue = false;
        }

        /// <summary>
        /// Handles the MouseMove event of the BrushEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void BrushEdit_MouseMove(object sender, MouseEventArgs e)
        {
            if (enableactiveeyedrop)
            {
                Point point = e.GetPosition(this);
                Point pt = e.GetPosition((Canvas)activeeyedrop.Parent);
                WriteableBitmap bmp = new WriteableBitmap((UIElement)this, null);
                int pixheight = bmp.PixelHeight;
                int pixwidth = bmp.PixelWidth;
                int pix = (int)((pixwidth * point.Y) + point.X);
                if (pix < bmp.Pixels.GetLength(0) && pix > 0)
                {
                    int colorAsInt = bmp.Pixels[pix];
                    Color = Color.FromArgb((byte)((colorAsInt >> 0x18) & 0xff), (byte)((colorAsInt >> 0x10) & 0xff), (byte)((colorAsInt >> 8) & 0xff), (byte)(colorAsInt & 0xff));
                    PointCollection p = m_color.ConvertRGBToHSV(Color.R, Color.G, Color.B);
                    if (this.VisualizationStyle == ColorSelectionMode.HSV)
                    {
                        if ((bool)rbhue.IsChecked)
                        {
                            x_axis = p[0].X * this.ColorRect.Width;
                            y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                            hueslider.Value = p[1].X * hueslider.Maximum;
                        }
                        else if ((bool)rbsat.IsChecked)
                        {
                            x_axis = p[1].X * ColorRect.Width;
                            y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                            hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                        }
                        else if ((bool)rbval.IsChecked)
                        {
                            y_axis = (1 - p[0].Y) * ColorRect.Height;
                            x_axis = p[1].X * ColorRect.Width;
                            hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                        }
                    }
                    else
                    {
                        x_axis = p[0].X * this.ColorRect.Width;
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        hueslider.Value = p[1].X * hueslider.Maximum;
                    }

                    activeeyedrop.SetValue(Canvas.TopProperty, pt.Y);
                    activeeyedrop.SetValue(Canvas.LeftProperty, pt.X);
                    eyedropper.SetValue(Canvas.TopProperty, pt.Y);
                    eyedropper.SetValue(Canvas.LeftProperty, pt.X);
                }
            }

            if (coloreditmove)
            {
                Point pos = e.GetPosition((UIElement)sender);
                if (pos.Y < (double)this.selectcolor.GetValue(Canvas.TopProperty))
                {
                    x_axis = (double)pos.X;
                    y_axis = (double)this.selectcolor.GetValue(Canvas.TopProperty);
                }
                else if (pos.Y > (double)this.selectcolor.GetValue(Canvas.TopProperty) && pos.Y < ((double)this.selectcolor.GetValue(Canvas.TopProperty) + this.selectcolor.Height))
                {
                    if (pos.X < (double)this.selectcolor.GetValue(Canvas.LeftProperty))
                    {
                        y_axis = pos.Y;
                        x_axis = (double)this.selectcolor.GetValue(Canvas.LeftProperty);
                    }
                    else
                    {
                        y_axis = pos.Y;
                        x_axis = (double)this.selectcolor.GetValue(Canvas.LeftProperty) + this.selectcolor.Width;
                    }
                }
                else
                {
                    x_axis = (double)pos.X;
                    y_axis = (double)this.selectcolor.GetValue(Canvas.TopProperty) + this.selectcolor.Height;
                }

                RefreshBrushEdit(x_axis, y_axis);
            }

            if (enablehue)
            {
                Point p = e.GetPosition(huerect);
                hueslider.Value = p.Y;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the eyedrop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void eyedrop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Canvas)activeeyedrop.Parent);
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.None;
            this.Cursor = Cursors.None;
            activeeyedrop.SetValue(Canvas.LeftProperty, p.X);
            activeeyedrop.SetValue(Canvas.TopProperty, p.Y);
            activeeyedrop.Visibility = Visibility.Visible;
            eyedropper.SetValue(Canvas.LeftProperty, p.X);
            eyedropper.SetValue(Canvas.TopProperty, p.Y);
            eyedropper.Visibility = Visibility.Visible;
            enableactiveeyedrop = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the eyedrop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void eyedrop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
            activeeyedrop.Visibility = Visibility.Collapsed;
            eyedropper.Visibility = Visibility.Collapsed;
            enableactiveeyedrop = false;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the gradrect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void gradrect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double offset = e.GetPosition(this.gradientBar).X / gradrect.ActualWidth;
            GradientStops gs = new GradientStops(m_selectedColor, true, offset);
            gs.gradientitem.SetValue(Canvas.LeftProperty, e.GetPosition(this.gradientBar).X);
            gs.gradientitem.SetValue(Canvas.TopProperty, (double)gradrect.GetValue(Canvas.TopProperty) + gradrect.ActualHeight - 5);
            this.gradientBar.Children.Add(gs.gradientitem);
            items.Items.Add(gs);
            items.SelectedItem = gs;
            ApplyGradient(gs);
        }

        /// <summary>
        /// Handles the LostFocus event of the HexValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HexValue_LostFocus(object sender, RoutedEventArgs e)
        {
            updateHexValue();
        }

        private void HexValue_MouseMove(object sender, MouseEventArgs e)
        {
            hexValue.Background = new SolidColorBrush(Colors.White);
        }


        /// <summary>
        /// Handles the MouseLeftButtonDown event of the huerect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void huerect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwapColors();
            Point p = e.GetPosition((UIElement)sender);
            enablehue = true;
            hueslider.Value = p.Y;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the huerect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void huerect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((UIElement)sender);
            hueslider.Value = p.Y;
            enablehue = false;            
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
            enableactiveeyedrop = false;
            mouseDown = false;          
        }

        /// <summary>
        /// Handles the MouseMove event of the huerect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void huerect_MouseMove(object sender, MouseEventArgs e)
        {
            if (enablehue)
            {
                Point p = e.GetPosition((UIElement)sender);
                hueslider.Value = p.Y;
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of the hueslider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="double"/> instance containing the event data.</param>
        private void hueslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateSelection(hueslider.Value);
        }

        /// <summary>
        /// Handles the Click event of the rbgradient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void rbgradient_Click(object sender, RoutedEventArgs e)
        {
          
            Button btn = (Button)sender;
            if (btn.Name.ToString() == "gradient")
            {
                gradflag = 1;
                btn.Background = FromLinearGradient(FromHex("#FFB5B5B5"), FromHex("#FF4B4B4B"), 0, 0.981735);
                DependencyObject obj = VisualTreeHelper.GetParent(btn);
                obj = VisualTreeHelper.GetChild(obj, 0);
                Button button=obj as Button;
                button.Background = new SolidColorBrush(FromHex("#6987BF"));                                                         
                this.BrushMode = BrushModes.Gradient;                
                GradientChange();
            }
            else
            {
                gradflag = 0;
                btn.Background = new SolidColorBrush(FromHex("#7E7E7E"));
                DependencyObject obj = VisualTreeHelper.GetParent(btn);
                obj = VisualTreeHelper.GetChild(obj, 1);
                Button button=obj as Button;
                button.Background = FromLinearGradient(FromHex("#FFFFFFFF"), FromHex("#FF8DA1C9"), 0.0547945, 0.981735);          
                this.BrushMode = BrushModes.Solid;              
               
            }
        }   

        private void rbsolid_MouseMove(object sender,MouseEventArgs e)
        {           
            Button btn = (Button)sender;
            if (btn.Name.ToString() == "gradient")
            {
                btn.Background = FromLinearGradient(FromHex("#FFB5B5B5"), FromHex("#FF4B4B4B"), 0, 0.981735);
                DependencyObject obj = VisualTreeHelper.GetParent(btn);
                obj = VisualTreeHelper.GetChild(obj, 0);
                Button button = obj as Button;
                button.Background = new SolidColorBrush(FromHex("#6987BF"));                       
            }
            else
            {
                btn.Background = new SolidColorBrush(FromHex("#7E7E7E"));
                DependencyObject obj = VisualTreeHelper.GetParent(btn);
                obj = VisualTreeHelper.GetChild(obj, 1);
                Button button = obj as Button;
                button.Background = FromLinearGradient(FromHex("#FFFFFFFF"), FromHex("#FF8DA1C9"), 0.0547945,0.981735);             
            }
        }
        //Color name from Hexa value
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public Color FromHex(string hex)
        {
            string v = hex.TrimStart('#');
            if (v.Length > 8)
                return Colors.Blue;
            if (v.Length == 6)
                v = "FF" + v;
            if (v.Length < 6)
                v = "FF" + v;
            while (v.Length < 8)
                v += "0";
            Color c = new Color();
            c.A = (byte)System.Convert.ToInt32(v.Substring(0, 2), 16);
            c.R = (byte)System.Convert.ToInt32(v.Substring(2, 2), 16);
            c.G = (byte)System.Convert.ToInt32(v.Substring(4, 2), 16);
            c.B = (byte)System.Convert.ToInt32(v.Substring(6, 2), 16);
            return c;
        }

        //color name for linear gradient brush

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="color1"></param>
        /// <param name="offset"></param>
        /// <param name="offset1"></param>
        /// <returns></returns>
        public LinearGradientBrush FromLinearGradient(Color color,Color color1, double offset,double offset1)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
          
            GradientStop gradientStop1 = new GradientStop();
            gradientStop1.Offset = offset;
            gradientStop1.Color = color;
            brush.GradientStops.Add(gradientStop1);

            GradientStop gradientStop2 = new GradientStop();
            gradientStop2.Offset = offset1;
            gradientStop2.Color = color1;
            brush.GradientStops.Add(gradientStop2);

            return brush;
        }
   


        /// <summary>
        /// Handles the Checked event of the rbhue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void rbhue_Checked(object sender, RoutedEventArgs e)
        {
            LinearGradientBrush blackrectbrush = new LinearGradientBrush();
            GradientStop blackrect = new GradientStop();
            Color c = Colors.Transparent;
            c.R = Convert.ToByte(0);
            c.B = Convert.ToByte(0);
            c.G = Convert.ToByte(0);
            blackrect.Color = Colors.Black;
            blackrect.Offset = 0;
            blackrectbrush.GradientStops.Add(blackrect);
            GradientStop blackrect1 = new GradientStop();
            blackrect1.Color = c;
            blackrect1.Offset = 1;
            blackrectbrush.GradientStops.Add(blackrect1);
            blackrectbrush.StartPoint = new Point(0, 1);
            blackrectbrush.EndPoint = new Point(0, 0);
            blackRect.Fill = blackrectbrush;           
            LinearGradientBrush whiterectbrush = new LinearGradientBrush();
            GradientStop whiterect = new GradientStop();
            whiterect.Color = Colors.White;
            whiterect.Offset = 0;
            whiterectbrush.GradientStops.Add(whiterect);
            GradientStop whiterect1 = new GradientStop();
            whiterect1.Color = Colors.Transparent;
            whiterect1.Offset = 1;
            whiterectbrush.GradientStops.Add(whiterect1);
            whiterectbrush.StartPoint = new Point(0, 0);
            whiterectbrush.EndPoint = new Point(1, 0);
            this.whiteRect.Fill = whiterectbrush;            
            hueslider.Background = ColorRect.Resources["BrushEdithuesliderBackground"] as LinearGradientBrush;
            UpdatefromText();
            RefreshBrushEdit(x_axis, y_axis);          
        }

        /// <summary>
        /// Handles the Click event of the rblinear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void rblinear_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;   
         
            if (btn.Name.ToString() == "linear")
            {
                islinear = true;
                btn.IsChecked = true;
                DependencyObject obj = VisualTreeHelper.GetParent(btn);
                obj = VisualTreeHelper.GetChild(obj, 1);
                ToggleButton btn1 = obj as ToggleButton;
                btn1.Background = new SolidColorBrush(Colors.Transparent);
                btn.Background = new SolidColorBrush(Colors.White);
                btn.BorderBrush = new SolidColorBrush(Colors.Black);
                rbradial.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                islinear = false;
                btn.IsChecked = true;
                DependencyObject obj = VisualTreeHelper.GetParent(btn);
                obj = VisualTreeHelper.GetChild(obj, 0);
                ToggleButton btn1 = obj as ToggleButton;
                btn1.Background = new SolidColorBrush(Colors.Transparent);
                btn.Background = new SolidColorBrush(Colors.White);
                btn.BorderBrush = new SolidColorBrush(Colors.Black);
                rblinear.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            if (items.Items.Count > 0)
            {
                enableSelectedBrush = true;
                ApplyGradient(items.SelectedItem);
                enableSelectedBrush = false;
            }
        }       


        /// <summary>
        /// Handles the Checked event of the rbsat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void rbsat_Checked(object sender, RoutedEventArgs e)
        {
            LinearGradientBrush huesliderbrush = new LinearGradientBrush();
            GradientStop hue1 = new GradientStop();
            hue1.Color = Colors.White;
            hue1.Offset = 1;
            huesliderbrush.GradientStops.Add(hue1);
            GradientStop hue2 = new GradientStop();
            hue2.Color = (SelectedColor.Fill as SolidColorBrush).Color;
            hue2.Offset = 0;
            huesliderbrush.GradientStops.Add(hue2);
            huesliderbrush.StartPoint = new Point(0, 0);
            huesliderbrush.EndPoint = new Point(0, 1);
            hueslider.Background = huesliderbrush;
            blackRect.Fill = new SolidColorBrush(Colors.Transparent);
            LinearGradientBrush brush = new LinearGradientBrush();
            GradientStop n = new GradientStop();
            n.Color = Colors.Black;
            n.Offset = 1;
            brush.GradientStops.Add(n);
            GradientStop n1 = new GradientStop();
            n1.Color = Colors.Transparent;
            n1.Offset = 0;
            brush.GradientStops.Add(n1);
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            whiteRect.Fill = brush;
            ColorRect.Fill = ColorRect.Resources["SatValBackground"] as LinearGradientBrush;
            disable = false;
            enableSlidervalue = false;
            UpdatefromText();
            enableSlidervalue = true;
            disable = true;
            RefreshBrushEdit(x_axis, y_axis);
        }

        /// <summary>
        /// Handles the Checked event of the rbval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void rbval_Checked(object sender, RoutedEventArgs e)
        {
            LinearGradientBrush huesliderbrush = new LinearGradientBrush();
            GradientStop hue1 = new GradientStop();
            hue1.Color = Colors.Black;
            hue1.Offset = 1;
            huesliderbrush.GradientStops.Add(hue1);
            GradientStop hue2 = new GradientStop();
            hue2.Color = (SelectedColor.Fill as SolidColorBrush).Color;
            hue2.Offset = 0;
            huesliderbrush.GradientStops.Add(hue2);
            huesliderbrush.StartPoint = new Point(0, 0);
            huesliderbrush.EndPoint = new Point(0, 1);
            hueslider.Background = huesliderbrush;
            blackRect.Fill = new SolidColorBrush(Colors.Transparent);
            LinearGradientBrush brush = new LinearGradientBrush();
            GradientStop n = new GradientStop();
            n.Color = Colors.White;
            n.Offset = 1;
            brush.GradientStops.Add(n);
            GradientStop n1 = new GradientStop();
            n1.Color = Colors.Transparent;
            n1.Offset = 0;
            brush.GradientStops.Add(n1);
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            whiteRect.Fill = brush;
            ColorRect.Fill = ColorRect.Resources["SatValBackground"] as LinearGradientBrush;
            disable = false;
            enableSlidervalue = false;
            UpdatefromText();
            enableSlidervalue = true;
            disable = true;
            RefreshBrushEdit(x_axis, y_axis);
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the RootVisual control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void RootVisual_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
            activeeyedrop.Visibility = Visibility.Collapsed;
            this.Cursor = Cursors.Arrow;
            enableactiveeyedrop = false;            
            enablehue = false;
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the MouseMove event of the RootVisual control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void RootVisual_MouseMove(object sender, MouseEventArgs e)
        {
            if (enablehue)
            {
                Point p = e.GetPosition(this.huerect);
                hueslider.Value = p.Y;
            }

            if (enableactiveeyedrop)
            {
                Point point = e.GetPosition(null);
                Point pt = e.GetPosition((Canvas)activeeyedrop.Parent);
                WriteableBitmap bmp = new WriteableBitmap(Application.Current.RootVisual, null);
                int pixheight = bmp.PixelHeight;
                int pixwidth = bmp.PixelWidth;
                int colorAsInt = bmp.Pixels[(int)((pixwidth * point.Y) + point.X)];
                Color = Color.FromArgb((byte)((colorAsInt >> 0x18) & 0xff), (byte)((colorAsInt >> 0x10) & 0xff), (byte)((colorAsInt >> 8) & 0xff), (byte)(colorAsInt & 0xff));
                PointCollection p = m_color.ConvertRGBToHSV(Color.R, Color.G, Color.B);
                if (this.VisualizationStyle == ColorSelectionMode.HSV)
                {
                    if ((bool)rbhue.IsChecked)
                    {
                        x_axis = p[0].X * this.ColorRect.Width;
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        hueslider.Value = p[1].X * hueslider.Maximum;
                    }
                    else if ((bool)rbsat.IsChecked)
                    {
                        x_axis = p[1].X * ColorRect.Width;
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                    }
                    else if ((bool)rbval.IsChecked)
                    {
                        y_axis = (1 - p[0].Y) * ColorRect.Height;
                        x_axis = p[1].X * ColorRect.Width;
                        hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                    }
                }

                activeeyedrop.SetValue(Canvas.TopProperty, pt.Y);
                activeeyedrop.SetValue(Canvas.LeftProperty, pt.X);
                eyedropper.SetValue(Canvas.TopProperty, pt.Y);
                eyedropper.SetValue(Canvas.LeftProperty, pt.X);
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the selectcolor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void selectcolor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            SwapColors();
            mouseDown = true;
            Point pos = e.GetPosition((UIElement)sender);
            x_axis = pos.X;
            y_axis = pos.Y;
            RefreshBrushEdit(x_axis, y_axis);

            if (this.A <= Convert.ToByte(10))
                this.A = Convert.ToByte(255);
            if (y_axis != 0.0 && x_axis != 0.0)
            {
                pop.IsOpen = true;
            }
            //pop.SetValue(Canvas.LeftProperty, (x_axis - (colorSelector.Height / 2) + colorSelector.ActualWidth));
            //pop.SetValue(Canvas.TopProperty, (y_axis - (colorSelector.Height / 2) + colorSelector.ActualHeight));
           

        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the selectcolor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void selectcolor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            colorSelector.Visibility = Visibility.Visible;
            enableactiveeyedrop = false;
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the MouseMove event of the selectcolor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void selectcolor_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point pt = e.GetPosition(null);
                Point pos = e.GetPosition((UIElement)sender);
                double d = (double)this.selectcolor.GetValue(Canvas.TopProperty);
                if (pos.Y <= (double)this.selectcolor.GetValue(Canvas.TopProperty))
                {
                    x_axis = (int)pos.X;
                    y_axis = 0;
                }
                else if (pos.Y > (double)this.selectcolor.GetValue(Canvas.TopProperty) && pos.Y < this.selectcolor.Height - 1)
                {
                    if (pos.X == 0.0)
                    {
                        y_axis = pos.Y;
                        x_axis = 0;
                    }
                    else if ((int)pos.X == (int)this.selectcolor.Width)
                    {
                        y_axis = pos.Y;
                        x_axis = this.selectcolor.Width;
                    }
                    else
                    {
                        x_axis = pos.X;
                        y_axis = pos.Y;
                    }
                }
                else if (pos.Y >= (double)this.selectcolor.Height - 1)
                {
                    x_axis = pos.X;
                    y_axis = this.selectcolor.Height;
                }
                else
                {
                    x_axis = pos.X;
                    y_axis = pos.Y;
                }

                RefreshBrushEdit(x_axis, y_axis);
            }
            else
            {
               
            }

            if (enableactiveeyedrop)
            {
                Point point = e.GetPosition(null);
                Point pt = e.GetPosition((Canvas)activeeyedrop.Parent);
                WriteableBitmap bmp = new WriteableBitmap(Application.Current.RootVisual, null);
                int pixheight = bmp.PixelHeight;
                int pixwidth = bmp.PixelWidth;
                int colorAsInt = bmp.Pixels[(int)((pixwidth * point.Y) + point.X)];
                Color = Color.FromArgb((byte)((colorAsInt >> 0x18) & 0xff), (byte)((colorAsInt >> 0x10) & 0xff), (byte)((colorAsInt >> 8) & 0xff), (byte)(colorAsInt & 0xff));
                PointCollection p = m_color.ConvertRGBToHSV(Color.R, Color.G, Color.B);
                if (this.VisualizationStyle == ColorSelectionMode.HSV)
                {
                    if ((bool)rbhue.IsChecked)
                    {
                        x_axis = p[0].X * this.ColorRect.Width;
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        hueslider.Value = p[1].X * hueslider.Maximum;
                    }
                    else if ((bool)rbsat.IsChecked)
                    {
                        x_axis = p[1].X * ColorRect.Width;
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                    }
                    else if ((bool)rbval.IsChecked)
                    {
                        y_axis = (1 - p[0].Y) * ColorRect.Height;
                        x_axis = p[1].X * ColorRect.Width;
                        hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                    }
                }

                activeeyedrop.SetValue(Canvas.TopProperty, pt.Y);
                activeeyedrop.SetValue(Canvas.LeftProperty, pt.X);
                eyedropper.SetValue(Canvas.TopProperty, pt.Y);
                eyedropper.SetValue(Canvas.LeftProperty, pt.X);
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            PointCollection p = m_color.ConvertRGBToHSV(m_selectedColor.R, m_selectedColor.G, m_selectedColor.B);
            x_axis = p[0].X * this.ColorRect.Width;
            y_axis = (1 - p[0].Y) * this.ColorRect.Height;
            hueslider.Value = p[1].X * hueslider.Maximum;
        }

        /// <summary>
        /// Handles the ValueChanged event of the txtH control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="double"/> instance containing the event data.</param>
        private void txtH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (disable)
            {
                UpdatefromText();
                RefreshBrushEdit(x_axis, y_axis);
            }
        }        
        #endregion

        #region Public DP 
        /// <summary>
        /// GradientMode dependency property.The GradientMode can be Radial or Linear mode
        /// </summary>
        /// <returns>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushModes">BrushModes</see>
        /// </returns>
        public static readonly DependencyProperty GradientModeProperty = DependencyProperty.Register("GradientMode", typeof(GradientModes), typeof(BrushEdit), new PropertyMetadata(GradientModes.Radial, new PropertyChangedCallback(OnGradientModeChanged)));
        /// <summary>
        /// BrushMode dependency property.The BrushMode can be Gradient or Solid mode
        /// </summary>
        /// <returns>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushModes">BrushModes</see>
        /// </returns>
        public static readonly DependencyProperty BrushModeProperty =
         DependencyProperty.Register("BrushMode", typeof(BrushModes), typeof(BrushEdit), new PropertyMetadata(BrushModes.Solid, new PropertyChangedCallback(OnBrushModeChanged)));

        /// <summary>
        /// EnableGradientToSolidSwitch dependency property.It enables the swtich to toggle
        /// between Solid and Gradient.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">bool </see>
        /// </returns>
        public static readonly DependencyProperty EnableGradientToSolidSwitchProperty =
       DependencyProperty.Register("EnableGradientToSolidSwitch", typeof(bool), typeof(BrushEdit), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableGradientToSolidSwitchChanged)));

        /// <summary>
        /// SelectedBackground dependency property.The SelectedBackground is set to the gradient stop's selected background
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </value>
        public static readonly DependencyProperty SelectedBackgroundProperty =
          DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(BrushEdit), new PropertyMetadata(null));

        /// <summary>
        /// SelectedBrush dependency property.The SelectedBrush can be set as Gradient or
        /// Solid.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </returns>
        public static readonly DependencyProperty SelectedBrushProperty =
                 DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(BrushEdit), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnSelectedBrushChanged)));

        /// <summary>
        /// VisualizationStyle dependency property.The VisualizationStyle can be HSV or RGB
        /// mode
        /// </summary>
        /// <returns>
        /// Type:<see
        /// cref="T:Syncfusion.Windows.Tools.Controls.ColorSelectionMode">ColorSelectionMode</see>
        /// </returns>
        public static readonly DependencyProperty VisualizationStyleProperty =
           DependencyProperty.Register("VisualizationStyle", typeof(ColorSelectionMode), typeof(BrushEdit), new PropertyMetadata(ColorSelectionMode.HSV, new PropertyChangedCallback(OnVisualizationStyleChanged)));

        #endregion

        #region Internal properties Method
        /// <summary>
        /// This method is invoked when any selected item is changed in the gradient bar
        /// </summary>
        /// <param name="gradstop">selected gradient stop item</param>
        internal void ApplyGradient(GradientStops gradstop)
        {
            GradientBrush gb;
            if (islinear)
            {
                gb = new LinearGradientBrush();
            }
            else
            {
                gb = new RadialGradientBrush();
            }

            for (int i = 0; i < items.Items.Count; i++)
            {
                GradientStops gss = (GradientStops)items.Items[i];
                GradientStop gs = new GradientStop();
                if (gradstop == gss || this.items.SelectedItem == gss)
                {
                    gss.isselected = true;
                    this.items.SelectedItem = (GradientStops)items.Items[i];
                    ((System.Windows.Shapes.Path)gss.gradientitem.Children[0]).Stroke = new SolidColorBrush(Colors.Black);                    
                    gss.gradientitem.SetValue(Canvas.ZIndexProperty, 1);
                }
                else
                {
                    gss.isselected = false;
                    ((System.Windows.Shapes.Path)gss.gradientitem.Children[0]).Stroke = new SolidColorBrush(Colors.DarkGray);
                    gss.gradientitem.SetValue(Canvas.ZIndexProperty, 0);
                }

                gs.Color = gss.color;
                gs.Offset = gss.offset;
                gb.GradientStops.Add(gs);
            }

            gradrect.Fill = gb;
            if (enableSelectedBrush)
            {
                this.SelectedBrush = gb;
            }

            this.Color = this.items.SelectedItem.color;

            if (this.m_selectedColor != this.Color)
            {
                UpdateColor();
                RefreshBrushEdit(x_axis, y_axis);
            }

            if (this.A <= Convert.ToByte(10))
                this.A = Convert.ToByte(255);
        }

        /// <summary>
        /// This method applies the Background for the RGB sliders depending on the selected
        /// color.
        /// </summary>
        private void CalculateBackground()
        {
            GradientStop gs1 = new GradientStop();
            gs1.Color = Color.FromArgb(m_a, 0, m_g, m_b);
            gs1.Offset = 0;
            GradientStop gs11 = new GradientStop();
            gs11.Color = Color.FromArgb(m_a, 255, m_g, m_b);
            gs11.Offset = 1;
            GradientStopCollection gc = new GradientStopCollection();
            gc.Add(gs1);
            gc.Add(gs11);
            BackgroundR = new LinearGradientBrush(gc, 0);
            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(m_a, m_r, 0, m_b);
            gs2.Offset = 0;
            GradientStop gs21 = new GradientStop();
            gs21.Color = Color.FromArgb(m_a, m_r, 255, m_b);
            gs21.Offset = 1;
            GradientStopCollection gc1 = new GradientStopCollection();
            gc1.Add(gs2);
            gc1.Add(gs21);
            BackgroundG = new LinearGradientBrush(gc1, 0);
            GradientStop gs3 = new GradientStop();
            gs3.Color = Color.FromArgb(m_a, m_r, m_g, 0);
            gs3.Offset = 0;
            GradientStop gs31 = new GradientStop();
            gs31.Color = Color.FromArgb(m_a, m_r, m_g, 255);
            gs31.Offset = 1;
            GradientStopCollection gc2 = new GradientStopCollection();
            gc2.Add(gs3);
            gc2.Add(gs31);
            BackgroundB = new LinearGradientBrush(gc2, 0);
            GradientStop gs4 = new GradientStop();
            gs4.Color = Color.FromArgb(0, m_r, m_g, m_b);
            gs4.Offset = 0;
            GradientStop gs41 = new GradientStop();
            gs41.Color = Color.FromArgb(255, m_r, m_g, m_b);
            gs41.Offset = 1;
            GradientStopCollection gc3 = new GradientStopCollection();
            gc3.Add(gs4);
            gc3.Add(gs41);
            BackgroundA = new LinearGradientBrush(gc3, 0);
        }

        /// <summary>
        /// This method clears the selected item in the gradient bar
        /// </summary>
        internal void clearSelection()
        {
            for (int i = 0; i < items.Items.Count; i++)
            {
                GradientStops gss = (GradientStops)items.Items[i];
                gss.isselected = false;
                ((System.Windows.Shapes.Path)gss.gradientitem.Children[0]).Stroke = null;
                gss.gradientitem.SetValue(Canvas.ZIndexProperty, 0);
            }
        }

        /// <summary>
        /// This method invokes whenever a gradient stop is added or changed.
        /// </summary>
        internal void GradientChange()
        {
            if (gradgrid != null)
            {
                if (this.BrushMode == BrushModes.Gradient)
                {                    
                    gradgrid.Height = this.Height / 6;                   
                    gradrect.Width = this.Width/1.4;
                    gradientBar.Width = this.gradrect.Width;
                    gradrect.Height = this.SliderWidth;                                       
                }
                else
                {
                    gradgrid.Visibility = Visibility.Collapsed;
                }

                if (this.BrushMode == BrushModes.Gradient)
                {
                    if (this.SelectedBrush == null || this.SelectedBrush.GetType().ToString().Contains("Solid"))
                    {
                        GradientStops gs;
                        if (this.SelectedBrush == null)
                        {
                            gs = new GradientStops(Colors.Black, true, 1.0);
                        }
                        else
                        {
                            this.Color = (this.SelectedBrush as SolidColorBrush).Color;
                            gs = new GradientStops((this.SelectedBrush as SolidColorBrush).Color, true, 1.0);
                        }

                        if (gradientBar.Children.Count == 1)
                        {
                            gs.gradientitem.SetValue(Canvas.LeftProperty, gradrect.ActualWidth);
                            gs.gradientitem.SetValue(Canvas.TopProperty, (double)gradrect.GetValue(Canvas.TopProperty) + gradrect.ActualHeight-5);
                            GradientStops gs1 = new GradientStops(Colors.White, false, 0.0);
                            gs1.gradientitem.SetValue(Canvas.LeftProperty, 0.0);
                            gs1.gradientitem.SetValue(Canvas.TopProperty, (double)gradrect.GetValue(Canvas.TopProperty) + gradrect.ActualHeight-5);
                            gradientBar.Children.Add(gs.gradientitem);
                            items = new GradientCollection();
                            gradientBar.Children.Add(gs1.gradientitem);
                            items.Items.Add(gs1);
                            items.Items.Add(gs);
                            items.SelectedItem = gs;
                        }

                        ApplyGradient(gs);
                    }
                    else
                    {
                        if (items != null)
                        {
                            if (this.SelectedBrush.GetType().ToString().Contains("Gradient"))
                            {
                                int k = 0;
                                foreach (GradientStop i in (this.SelectedBrush as GradientBrush).GradientStops)
                                {
                                    GradientStops gs = new GradientStops(i.Color, true, i.Offset);
                                    gs.gradientitem.SetValue(Canvas.LeftProperty, gradrect.ActualWidth * i.Offset);
                                    gs.gradientitem.SetValue(Canvas.TopProperty, (double)gradrect.GetValue(Canvas.TopProperty) + gradrect.ActualHeight - 5);
                                    if (k < items.Items.Count)
                                    {
                                        if (((GradientStops)items.Items[k]).gradientitem.Parent == null)
                                        {
                                            gradientBar.Children.Add(((GradientStops)items.Items[k]).gradientitem);
                                        }

                                        ((GradientStops)items.Items[k]).isselected = gs.isselected;
                                        ((GradientStops)items.Items[k]).offset = gs.offset;
                                        ((System.Windows.Shapes.Path)((GradientStops)items.Items[k]).gradientitem.Children[0]).Fill = new SolidColorBrush(gs.color);
                                        ((GradientStops)items.Items[k]).gradientitem.SetValue(Canvas.LeftProperty, (double)gs.gradientitem.GetValue(Canvas.LeftProperty));
                                        ((GradientStops)items.Items[k]).gradientitem.SetValue(Canvas.TopProperty, (double)gs.gradientitem.GetValue(Canvas.TopProperty));
                                        ((GradientStops)items.Items[k++]).color = gs.color;
                                    }
                                    else
                                    {
                                        gradientBar.Children.Add(gs.gradientitem);
                                        items.Items.Add(gs);
                                        items.SelectedItem = gs;
                                    }                                  
                                }
                                
                                if (items.SelectedItem != null && items.Items.Count > 0)
                                {
                                    ApplyGradient(items.SelectedItem);
                                    this.Color = items.SelectedItem.color;
                                }

                                this.BrushMode = BrushModes.Gradient;
                            }
                        }
                    }
                }
                else
                {
                    this.Color = (this.SelectedBrush as SolidColorBrush).Color;
                }
            }
        }

        /// <summary>
        /// This method is invoked to change the background when the HSV color value is
        /// changed.
        /// </summary>
        /// <param name="color">Selected color</param>
        private void RefreshBackground(Color color)
        {
            txtH.Background = ColorRect.Resources["BrushEditSatValBackground"] as LinearGradientBrush;
            LinearGradientBrush huesliderbrush = new LinearGradientBrush();
            GradientStop hue1 = new GradientStop();
            hue1.Color = Colors.White;
            hue1.Offset = 0;
            huesliderbrush.GradientStops.Add(hue1);
            GradientStop hue2 = new GradientStop();
            hue2.Color = color;
            hue2.Offset = 1;
            huesliderbrush.GradientStops.Add(hue2);
            huesliderbrush.StartPoint = new Point(0, 1);
            huesliderbrush.EndPoint = new Point(1, 1);
            txtS.Background = huesliderbrush;
            LinearGradientBrush huesliderbrush1 = new LinearGradientBrush();
            GradientStop hue11 = new GradientStop();
            hue11.Color = Colors.Black;
            hue11.Offset = 0;
            huesliderbrush1.GradientStops.Add(hue11);
            GradientStop hue21 = new GradientStop();
            hue21.Color = color;
            hue21.Offset = 1;
            huesliderbrush1.GradientStops.Add(hue21);
            huesliderbrush1.StartPoint = new Point(0, 1);
            huesliderbrush1.EndPoint = new Point(1, 1);
            txtV.Background = huesliderbrush1;
        }

        /// <summary>
        /// This method is invoked whenever any slider value or text value is changes to
        /// change the color
        /// </summary>
        /// <param name="xPos">selected color x axis value</param>
        /// <param name="yPos">selected color y axis value</param>
        private void RefreshBrushEdit(double xPos, double yPos)
        {
            hexValue.SelectionBackground = new SolidColorBrush(FromHex("#91C8FD"));
            hexValue.SelectionForeground = new SolidColorBrush(Colors.Black);
            colorSelector.SetValue(Canvas.LeftProperty, xPos - (colorSelector.Height / 2));
            colorSelector.SetValue(Canvas.TopProperty, yPos - (colorSelector.Height / 2));
            
            pop.SetValue(Canvas.LeftProperty, (xPos - (colorSelector.Height / 2) + colorSelector.ActualWidth));
            pop.SetValue(Canvas.TopProperty, (yPos - (colorSelector.Height / 2) + colorSelector.ActualHeight));
           
            float yComponent = 1 - (float)(yPos / ColorRect.Height);
            float xComponent = (float)(xPos / ColorRect.Width);
            if (xComponent >=0)
            {
                if (this.VisualizationStyle == ColorSelectionMode.HSV)
                {
                    Color huecolor = Colors.Black;
                    if ((bool)rbhue.IsChecked)
                    {
                        huecolor = m_color.ConvertHSVToRGB((float)m_selectedHue, (float)1, (float)1);
                        ColorRect.Fill = new SolidColorBrush(huecolor);
                        m_selectedColor = m_color.ConvertHSVToRGB((float)m_selectedHue, xComponent, yComponent);
                        if (prevcolor != m_selectedColor)
                        {
                            disable = false;
                            txtH.Value = m_selectedHue * 360;
                            txtS.Value = xComponent;
                            disable = true;
                            txtV.Value = yComponent;
                        }                    
                    }

                    if ((bool)rbsat.IsChecked)
                    {
                        disable = false;
                        huecolor = m_color.ConvertHSVToRGB(xComponent, (float)1, (float)1);
                        (hueslider.Background as LinearGradientBrush).GradientStops[1].Color = huecolor;
                        m_selectedColor = m_color.ConvertHSVToRGB(xComponent, (float)(1 - m_selectedHue), yComponent);
                        txtH.Value = xComponent * 360;
                        txtS.Value = 1 - m_selectedHue;
                        txtV.Value = yComponent;
                    }

                    if ((bool)rbval.IsChecked)
                    {
                        disable = false;
                        huecolor = m_color.ConvertHSVToRGB(xComponent, (float)1, (float)1);
                        (hueslider.Background as LinearGradientBrush).GradientStops[1].Color = huecolor;
                        m_selectedColor = m_color.ConvertHSVToRGB(xComponent, yComponent, (float)(1 - m_selectedHue));
                        txtH.Value = xComponent * 360;
                        txtS.Value = yComponent;
                        txtV.Value = 1 - m_selectedHue;
                    }

                    if (prevcolor != huecolor)
                    {
                        RefreshBackground(huecolor);
                        disable = true;
                    }

                    prevcolor = huecolor;
                    hexValue.Text = m_selectedColor.ToString();
                    ColorText = hexValue.Text;
                }
                else
                {
                    m_selectedColor = m_color.ConvertHSVToRGB((float)m_selectedHue, xComponent, yComponent);
                    m_selectedColor.A = Convert.ToByte(A);
                    m_bColorUpdating = true;
                    R = Convert.ToByte(m_selectedColor.R);
                    m_bColorUpdating = true;
                    G = Convert.ToByte(m_selectedColor.G);
                    m_bColorUpdating = true;
                    B = Convert.ToByte(m_selectedColor.B);
                    hexValue.Text = m_selectedColor.ToString();
                    ColorText = hexValue.Text;
                    CalculateBackground();
                }

                SelectedColor.Fill = new SolidColorBrush(m_selectedColor);
                if (this.BrushMode == BrushModes.Solid)
                {
                    this.SelectedBrush = new SolidColorBrush(m_selectedColor);
                }
                else
                {
                    if (items.Items.Count > 0)
                    {
                        items.SelectedItem.color = m_selectedColor;
                        ((System.Windows.Shapes.Path)items.SelectedItem.gradientitem.Children[0]).Fill = new SolidColorBrush(m_selectedColor);
                        ((System.Windows.Shapes.Path)items.SelectedItem.gradientitem.Children[0]).Stroke = SelectedBackground;
                        if (this.BrushMode == BrushModes.Gradient)
                        {
                            enableSelectedBrush = true;
                            ApplyGradient(items.SelectedItem);
                            enableSelectedBrush = false;
                        }
                    }
                }

                if (ColorSelected != null)
                {
                    ColorSelected(m_selectedColor);
                }

                if (enableSlidervalue)
                {
                    disable = true;
                }
              
            }
            string[] colorString = SuchColor(m_selectedColor);
            ColorName = colorString[0];
        }
        /// <summary>
        /// Inits the color table.
        /// </summary>
        /// <returns></returns>
        private static uint[] InitColorTable()
        {
            uint[] numArray = new uint[142];
            numArray[0] = 0xfff0f8ff;
            numArray[1] = 0xfffaebd7;
            numArray[2] = 0xff00ffff;
            numArray[3] = 0xff7fffd4;
            numArray[4] = 0xfff0ffff;
            numArray[5] = 0xfff5f5dc;
            numArray[6] = 0xffffe4c4;
            numArray[7] = 0xff000000;
            numArray[8] = 0xffffebcd;
            numArray[9] = 0xff0000ff;
            numArray[10] = 0xff8a2be2;
            numArray[11] = 0xffa52a2a;
            numArray[12] = 0xffdeb887;
            numArray[13] = 0xff5f9ea0;
            numArray[14] = 0xff7fff00;
            numArray[15] = 0xffd2691e;
            numArray[16] = 0xffff7f50;
            numArray[17] = 0xff6495ed;
            numArray[18] = 0xfffff8dc;
            numArray[19] = 0xffdc143c;
            numArray[20] = 0xff00ffff;
            numArray[21] = 0xff00008b;
            numArray[22] = 0xff008b8b;
            numArray[23] = 0xffb8860b;
            numArray[24] = 0xffa9a9a9;
            numArray[25] = 0xff006400;
            numArray[26] = 0xffbdb76b;
            numArray[27] = 0xff8b008b;
            numArray[28] = 0xff556b2f;
            numArray[29] = 0xffff8c00;
            numArray[30] = 0xff9932cc;
            numArray[31] = 0xff8b0000;
            numArray[32] = 0xffe9967a;
            numArray[33] = 0xff8fbc8f;
            numArray[34] = 0xff483d8b;
            numArray[35] = 0xff2f4f4f;
            numArray[36] = 0xff00ced1;
            numArray[37] = 0xff9400d3;
            numArray[38] = 0xffff1493;
            numArray[39] = 0xff00bfff;
            numArray[40] = 0xff696969;
            numArray[41] = 0xff1e90ff;
            numArray[42] = 0xffb22222;
            numArray[43] = 0xfffffaf0;
            numArray[44] = 0xff228b22;
            numArray[45] = 0xffff00ff;
            numArray[46] = 0xffdcdcdc;
            numArray[47] = 0xfff8f8ff;
            numArray[48] = 0xffffd700;
            numArray[49] = 0xffdaa520;
            numArray[50] = 0xff808080;
            numArray[51] = 0xff008000;
            numArray[52] = 0xffadff2f;
            numArray[53] = 0xfff0fff0;
            numArray[54] = 0xffff69b4;
            numArray[55] = 0xffcd5c5c;
            numArray[56] = 0xff4b0082;
            numArray[57] = 0xfffffff0;
            numArray[58] = 0xfff0e68c;
            numArray[59] = 0xffe6e6fa;
            numArray[60] = 0xfffff0f5;
            numArray[61] = 0xff7cfc00;
            numArray[62] = 0xfffffacd;
            numArray[63] = 0xffadd8e6;
            numArray[64] = 0xfff08080;
            numArray[65] = 0xffe0ffff;
            numArray[66] = 0xfffafad2;
            numArray[67] = 0xffd3d3d3;
            numArray[68] = 0xff90ee90;
            numArray[69] = 0xffffb6c1;
            numArray[70] = 0xffffa07a;
            numArray[71] = 0xff20b2aa;
            numArray[72] = 0xff87cefa;
            numArray[73] = 0xff778899;
            numArray[74] = 0xffb0c4de;
            numArray[75] = 0xffffffe0;
            numArray[76] = 0xff00ff00;
            numArray[77] = 0xff32cd32;
            numArray[78] = 0xfffaf0e6;
            numArray[79] = 0xffff00ff;
            numArray[80] = 0xff800000;
            numArray[81] = 0xff66cdaa;
            numArray[82] = 0xff0000cd;
            numArray[83] = 0xffba55d3;
            numArray[84] = 0xff9370db;
            numArray[85] = 0xff3cb371;
            numArray[86] = 0xff7b68ee;
            numArray[87] = 0xff00fa9a;
            numArray[88] = 0xff48d1cc;
            numArray[89] = 0xffc71585;
            numArray[90] = 0xff191970;
            numArray[91] = 0xfff5fffa;
            numArray[92] = 0xffffe4e1;
            numArray[93] = 0xffffe4b5;
            numArray[94] = 0xffffdead;
            numArray[95] = 0xff000080;
            numArray[96] = 0xfffdf5e6;
            numArray[97] = 0xff808000;
            numArray[98] = 0xff6b8e23;
            numArray[99] = 0xffffa500;
            numArray[100] = 0xffff4500;
            numArray[101] = 0xffda70d6;
            numArray[102] = 0xffeee8aa;
            numArray[103] = 0xff98fb98;
            numArray[104] = 0xffafeeee;
            numArray[105] = 0xffdb7093;
            numArray[106] = 0xffffefd5;
            numArray[107] = 0xffffdab9;
            numArray[108] = 0xffcd853f;
            numArray[109] = 0xffffc0cb;
            numArray[110] = 0xffdda0dd;
            numArray[111] = 0xffb0e0e6;
            numArray[112] = 0xff800080;
            numArray[113] = 0xffff0000;
            numArray[114] = 0xffbc8f8f;
            numArray[115] = 0xff4169e1;
            numArray[116] = 0xff8b4513;
            numArray[117] = 0xfffa8072;
            numArray[118] = 0xfff4a460;
            numArray[119] = 0xff2e8b57;
            numArray[120] = 0xfffff5ee;
            numArray[121] = 0xffa0522d;
            numArray[122] = 0xffc0c0c0;
            numArray[123] = 0xff87ceeb;
            numArray[124] = 0xff6a5acd;
            numArray[125] = 0xff708090;
            numArray[126] = 0xfffffafa;
            numArray[127] = 0xff00ff7f;
            numArray[128] = 0xff4682b4;
            numArray[129] = 0xffd2b48c;
            numArray[130] = 0xff008080;
            numArray[131] = 0xffd8bfd8;
            numArray[132] = 0xffff6347;
            numArray[133] = 0xffffffff;
            numArray[134] = 0xff40e0d0;
            numArray[135] = 1;
            numArray[136] = 0xffee82ee;
            numArray[137] = 0xfff5deb3;
            numArray[138] = 0xffffffff;
            numArray[139] = 0xfff5f5f5;
            numArray[140] = 0xffffff00;
            numArray[141] = 0xff9acd32;

            return numArray;
        }
        /// <summary>
        /// Froms the U int32.
        /// </summary>
        /// <param name="argb">The ARGB.</param>
        /// <returns></returns>
        private static Color FromUInt32(UInt32 argb)
        {
            Color color1 = new Color();
            color1.A = (byte)((argb & 0xff000000) >> 0x18);
            color1.R = (byte)((argb & 0xff0000) >> 0x10);
            color1.G = (byte)((argb & 0xff00) >> 8);
            color1.B = (byte)(argb & 0xff);
            return color1;
        }
        /// <summary>
        /// Suches the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static string[] SuchColor(Color color)
        {
            int lastMinDiff = int.MaxValue;
            int red = int.MaxValue;
            int green = int.MaxValue;
            int blue = int.MaxValue;
            int resIdRed = 0;
            int resIdGreen = 0;
            int resIdBlue = 0;
            int resId = 0;
            uint[] wordKnownColors = InitColorTable();

            for (int i = 0; i < wordKnownColors.Length; i++)
            {
                Color iColor = FromUInt32(wordKnownColors[i]);

                int redDiff = Math.Abs(iColor.R - color.R);
                int greenDiff = Math.Abs(iColor.G - color.G);
                int blueDiff = Math.Abs(iColor.B - color.B);

                int rgbDiff = redDiff + blueDiff + greenDiff;

                if (rgbDiff < lastMinDiff)
                {
                    resId = i;
                    lastMinDiff = rgbDiff;
                }

                if (redDiff < red)
                {
                    resIdRed = i;
                    red = redDiff;
                }

                if (greenDiff < green)
                {
                    resIdGreen = i;
                    green = greenDiff;
                }

                if (blueDiff < blue)
                {
                    resIdBlue = i;
                    blue = blueDiff;
                }
            }

            // return FromUInt32(WordKnownColors[resId]);
            KnownColor color1 = (KnownColor)wordKnownColors[resId];
            KnownColor color2 = (KnownColor)wordKnownColors[resIdRed];
            KnownColor color3 = (KnownColor)wordKnownColors[resIdGreen];
            KnownColor color4 = (KnownColor)wordKnownColors[resIdBlue];

            string[] result = new string[4];
            result[0] = color1.ToString();
            result[1] = color2.ToString();
            result[2] = color3.ToString();
            result[3] = color4.ToString();

            return result;
        }

        /// <summary>
        /// Swaps the colors.
        /// </summary>
        internal void SwapColors()
        {
            CurrentColor.Fill = SelectedColor.Fill;           
        }

        /// <summary>
        /// This method is invoked whenever the color is selected
        /// </summary>
        internal void UpdateColor()
        {            
            Color = Color.FromArgb(m_a, m_r, m_g, m_b);
            if (SelectedColor != null)
            {
                SelectedColor.Fill = new SolidColorBrush(Color);
                //hexValue.Text = Color.ToString();
                ColorText = hexValue.Text;
                CalculateBackground();
                PointCollection p = m_color.ConvertRGBToHSV(Color.R, Color.G, Color.B);
                if (this.VisualizationStyle == ColorSelectionMode.HSV)
                {                    
                    if ((bool)rbhue.IsChecked)
                    {
                       
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        if (this.Color != Colors.Black)
                        {
                            x_axis = p[0].X * this.ColorRect.Width;
                            hueslider.Value = p[1].X * hueslider.Maximum;
                        }
                    }
                    else if ((bool)rbsat.IsChecked)
                    {
                       
                        y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                        if (this.Color != Colors.Black)
                        {
                            hueslider.Value = (1 - p[0].X) * hueslider.Maximum;
                            x_axis = p[1].X * ColorRect.Width;
                        }
                    }
                    else if ((bool)rbval.IsChecked)
                    {
                        y_axis = (1 - p[0].X) * ColorRect.Height;
                        if (this.Color != Colors.Black)
                        {
                            x_axis = p[1].X * ColorRect.Width;
                            hueslider.Value = (1 - p[0].Y) * hueslider.Maximum;
                        }
                    }
                }
                else
                {
                    x_axis = p[0].X * this.ColorRect.Width;
                    y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                    hueslider.Value = p[1].X * hueslider.Maximum;
                }                
            }            
        }

        /// <summary>
        /// This method is used to update the color from the given slider value
        /// </summary>
        internal void UpdatefromText()
        {
                x_axis = txtS.Value * this.ColorRect.Width;
                y_axis = (1 - txtV.Value) * this.ColorRect.Height;
                hueslider.Value = (txtH.Value / 360) * hueslider.Maximum;           
            if ((bool)rbsat.IsChecked)
            {
                double d = x_axis;
                x_axis = m_selectedHue * ColorRect.Width;
                y_axis = (1 - txtV.Value) * this.ColorRect.Height;
                hueslider.Value = ((ColorRect.Width - d) / ColorRect.Width) * hueslider.Maximum;
            }
            else if ((bool)rbval.IsChecked)
            {
                double d = y_axis;
                y_axis = ((ColorRect.Width - x_axis) / ColorRect.Width) * ColorRect.Height;
                x_axis = m_selectedHue * ColorRect.Width;
                hueslider.Value = (d / ColorRect.Height) * hueslider.Maximum;
            }
        }

        /// <summary>
        /// This method updates the color from the hexvalue textbox.
        /// </summary>
        internal void updateHexValue()
        {
            ValidateHexaValue();
            if (this.VisualizationStyle == ColorSelectionMode.HSV)
            {
                 Color = Color.FromArgb(byte.Parse("FF", System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));
                m_selectedColor = Color;
                PointCollection p = m_color.ConvertRGBToHSV(m_selectedColor.R, m_selectedColor.G, m_selectedColor.B);
                if ((bool)rbhue.IsChecked)
                {
                    x_axis = p[0].X * this.ColorRect.Width;
                    y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                    hueslider.Value = p[1].X * hueslider.Maximum;
                }
                else if ((bool)rbsat.IsChecked)
                {
                    x_axis = p[1].X * ColorRect.Width;
                    y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                    hueslider.Value = (1-p[0].X) * hueslider.Maximum;
                }
                else if ((bool)rbval.IsChecked)
                {
                    y_axis = (1-p[0].X) * ColorRect.Height;
                    x_axis = p[1].X * ColorRect.Width;
                    hueslider.Value = (1-p[0].Y)  * hueslider.Maximum;
                }
            }
            else
            {
                Color = Color.FromArgb(byte.Parse(hexValue.Text.Substring(1, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));
                m_selectedColor = Color;
                PointCollection p = m_color.ConvertRGBToHSV(m_selectedColor.R, m_selectedColor.G, m_selectedColor.B);
                x_axis = p[0].X * this.ColorRect.Width;
                y_axis = (1 - p[0].Y) * this.ColorRect.Height;
                hueslider.Value = p[1].X * hueslider.Maximum;
            }

            RefreshBrushEdit(x_axis, y_axis);
        }

        /// <summary>
        /// This method Validates the given HexaDecimal code.
        /// </summary>
        private void ValidateHexaValue()
        {           
            try
            {
                Color color;
                if (hexValue.Text == String.Empty)
                {
                    hexValue.Text = m_selectedColor.ToString();
                    ColorText = hexValue.Text;
                }
                else
                {
                    color = Color.FromArgb(byte.Parse(hexValue.Text.Substring(1, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));
                }
            }
            catch
            {
                Color color=Colors.Black;
                char hexachar = hexValue.Text[0];
                string hexastring=string.Empty;
                if (hexachar == '#')
                {
                    hexastring = hexValue.Text.Substring(1);
                }
                else 
                {
                    hexastring = hexValue.Text;                                  
                }

                switch (hexastring.Length)
                {
                    case 3: hexValue.Text = "#FF" + hexastring[0] + hexastring[0] + hexastring[1] + hexastring[1] + hexastring[2] + hexastring[2];
                        ColorText = hexValue.Text;
                        break;
                    case 4: hexValue.Text = "#" + hexastring[0] + hexastring[0] + hexastring[1] + hexastring[1] + hexastring[2] + hexastring[2] + hexastring[3] + hexastring[3];
                        ColorText = hexValue.Text;
                        break;
                    case 6: hexValue.Text = "#FF" + hexastring;
                        ColorText = hexValue.Text;
                        break;
                    case 8: hexValue.Text = "#"+hexastring;
                        ColorText = hexValue.Text;
                        break;

                    default:
                        hexValue.Text = m_selectedColor.ToString();
                        ColorText = hexValue.Text;
                        break;
                }

                try
                {
                    color = Color.FromArgb(byte.Parse(hexValue.Text.Substring(1, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(hexValue.Text.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));
                }
                catch 
                {
                    hexValue.Text = m_selectedColor.ToString();
                    ColorText = hexValue.Text;
                }                
            }
        }

        /// <summary>
        /// This method is invoked when the hue slider value is changed
        /// </summary>
        /// <param name="yPos">It is the Hueslider value</param>
        private void UpdateSelection(double yPos)
        {
            if (this.VisualizationStyle == ColorSelectionMode.HSV)
            {
                if ((bool)rbhue.IsChecked)
                {
                    float huePosition = (float)(hueslider.Value / hueslider.Maximum);
                    Color c = m_color.ConvertHSVToRGB(huePosition, 1, 1);
                    ColorRect.Fill = new SolidColorBrush(c);
                    disable = false;
                    txtH.Value = huePosition * 360;
                    disable = true;
                }
            }
            else if (this.VisualizationStyle == ColorSelectionMode.RGB)
            {
                if (disable)
                {
                    float huePosition = (float)(hueslider.Value / hueslider.Maximum);
                    Color c = m_color.ConvertHSVToRGB(huePosition, 1, 1);
                    c.A = Convert.ToByte(this.A);
                    ColorRect.Fill = new SolidColorBrush(c);
                }
            }

            m_selectedHue = (float)(hueslider.Value / hueslider.Maximum);
            RefreshBrushEdit(x_axis, y_axis);
        }

        /// <summary>
        /// This methos is invoked when the ColorSelectionMode is changed.
        /// </summary>
        /// <param name="csm">selected ColorSelection mode</param>
        private void UpdateVisualizationStyle(ColorSelectionMode csm)
        {
            if (csm == ColorSelectionMode.HSV)
            {
                enableSlidervalue = false;
                rgbValgrid.Visibility = Visibility.Collapsed;
                hsvValgrid.Visibility = Visibility.Visible;
                rbhue = this.GetTemplateChild("ButtonH") as RadioButton;
                rbsat = this.GetTemplateChild("ButtonS") as RadioButton;
                rbval = this.GetTemplateChild("ButtonV") as RadioButton;
                txtH = this.GetTemplateChild("TextBoxH") as Slider;
                txtS = this.GetTemplateChild("TextBoxS") as Slider;
                txtV = this.GetTemplateChild("TextBoxV") as Slider;
                rbhue.Checked += new RoutedEventHandler(rbhue_Checked);
                rbsat.Checked += new RoutedEventHandler(rbsat_Checked);
                rbval.Checked += new RoutedEventHandler(rbval_Checked);
                txtH.ValueChanged += new RoutedPropertyChangedEventHandler<double>(txtH_ValueChanged);
                txtS.ValueChanged += new RoutedPropertyChangedEventHandler<double>(txtH_ValueChanged);
                txtV.ValueChanged += new RoutedPropertyChangedEventHandler<double>(txtH_ValueChanged);
                RefreshBackground(this.Color);

                if (items != null)
                {
                    RefreshBrushEdit(x_axis, y_axis);
                }
            }
            else
            {
                if (hsvValgrid != null)
                {
                    if (rbhue != null)
                    {
                        rbhue.IsChecked = true;
                    }

                    hsvValgrid.Visibility = Visibility.Collapsed;
                    rgbValgrid.Visibility = Visibility.Visible;
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

            Application.Current.RootVisual.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);
            Application.Current.RootVisual.MouseLeftButtonUp -= new MouseButtonEventHandler(RootVisual_MouseLeftButtonUp);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ColorSelectionEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object OldValue { get; set; }
    }
}