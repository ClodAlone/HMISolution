#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SILVERLIGHT || NETFX_CORE || WP
#if !NETFX_CORE && !WP
using System.Windows.Media;
#endif
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
namespace System.Drawing
{
    #region ColorConverter
    internal class ColorConverter
    {
        public static Color ActiveBorder = Color.FromArgb(255, 180, 180, 180);
        public static Color ActiveCaption = Color.FromArgb(255, 153, 180, 209);
        public static Color ActiveCaptionText = Color.FromArgb(255, 0, 0, 0);
        public static Color AppWorkspace = Color.FromArgb(255, 171, 171, 171);
        public static Color Control = Color.FromArgb(255, 240, 240, 240);
        public static Color ControlDark = Color.FromArgb(255, 160, 160, 160);
        public static Color ControlDarkDark = Color.FromArgb(255, 105, 105, 105);
        public static Color ControlLight = Color.FromArgb(255, 227, 227, 227);
        public static Color ControlLightLight = Color.FromArgb(255, 255, 255, 255);
        public static Color ControlText = Color.FromArgb(255, 0, 0, 0);
        public static Color Desktop = Color.FromArgb(255, 0, 0, 0);
        public static Color GrayText = Color.FromArgb(255, 109, 109, 109);
        public static Color Highlight = Color.FromArgb(255, 51, 153, 255);
        public static Color HighlightText = Color.FromArgb(255, 255, 255, 255);
        public static Color HotTrack = Color.FromArgb(255, 0, 102, 204);
        public static Color InactiveBorder = Color.FromArgb(255, 244, 247, 252);
        public static Color InactiveCaption = Color.FromArgb(255, 191, 205, 219);
        public static Color InactiveCaptionText = Color.FromArgb(255, 67, 78, 84);
        public static Color Info = Color.FromArgb(255, 255, 255, 225);
        public static Color InfoText = Color.FromArgb(255, 0, 0, 0);
        public static Color Menu = Color.FromArgb(255, 240, 240, 240);
        public static Color MenuText = Color.FromArgb(255, 0, 0, 0);
        public static Color ScrollBar = Color.FromArgb(255, 200, 200, 200);
        public static Color Window = Color.FromArgb(255, 255, 255, 255);
        public static Color WindowFrame = Color.FromArgb(255, 100, 100, 100);
        public static Color WindowText = Color.FromArgb(255, 0, 0, 0);
        public static Color Transparent = Color.FromArgb(0, 255, 255, 255);
        public static Color AliceBlue = Color.FromArgb(255, 240, 248, 255);
        public static Color AntiqueWhite = Color.FromArgb(255, 250, 235, 215);
        public static Color Aqua = Color.FromArgb(255, 0, 255, 255);
        public static Color Aquamarine = Color.FromArgb(255, 127, 255, 212);
        public static Color Azure = Color.FromArgb(255, 240, 255, 255);
        public static Color Beige = Color.FromArgb(255, 245, 245, 220);
        public static Color Bisque = Color.FromArgb(255, 255, 228, 196);
        public static Color Black = Color.FromArgb(255, 0, 0, 0);
        public static Color BlanchedAlmond = Color.FromArgb(255, 255, 235, 205);
        public static Color Blue = Color.FromArgb(255, 0, 0, 255);
        public static Color BlueViolet = Color.FromArgb(255, 138, 43, 226);
        public static Color Brown = Color.FromArgb(255, 165, 42, 42);
        public static Color BurlyWood = Color.FromArgb(255, 222, 184, 135);
        public static Color CadetBlue = Color.FromArgb(255, 95, 158, 160);
        public static Color Chartreuse = Color.FromArgb(255, 127, 255, 0);
        public static Color Chocolate = Color.FromArgb(255, 210, 105, 30);
        public static Color Coral = Color.FromArgb(255, 255, 127, 80);
        public static Color CornflowerBlue = Color.FromArgb(255, 100, 149, 237);
        public static Color Cornsilk = Color.FromArgb(255, 255, 248, 220);
        public static Color Crimson = Color.FromArgb(255, 220, 20, 60);
        public static Color Cyan = Color.FromArgb(255, 0, 255, 255);
        public static Color DarkBlue = Color.FromArgb(255, 0, 0, 139);
        public static Color DarkCyan = Color.FromArgb(255, 0, 139, 139);
        public static Color DarkGoldenrod = Color.FromArgb(255, 184, 134, 11);
        public static Color DarkGray = Color.FromArgb(255, 169, 169, 169);
        public static Color DarkGreen = Color.FromArgb(255, 0, 100, 0);
        public static Color DarkKhaki = Color.FromArgb(255, 189, 183, 107);
        public static Color DarkMagenta = Color.FromArgb(255, 139, 0, 139);
        public static Color DarkOliveGreen = Color.FromArgb(255, 85, 107, 47);
        public static Color DarkOrange = Color.FromArgb(255, 255, 140, 0);
        public static Color DarkOrchid = Color.FromArgb(255, 153, 50, 204);
        public static Color DarkRed = Color.FromArgb(255, 139, 0, 0);
        public static Color DarkSalmon = Color.FromArgb(255, 233, 150, 122);
        public static Color DarkSeaGreen = Color.FromArgb(255, 143, 188, 139);
        public static Color DarkSlateBlue = Color.FromArgb(255, 72, 61, 139);
        public static Color DarkSlateGray = Color.FromArgb(255, 47, 79, 79);
        public static Color DarkTurquoise = Color.FromArgb(255, 0, 206, 209);
        public static Color DarkViolet = Color.FromArgb(255, 148, 0, 211);
        public static Color DeepPink = Color.FromArgb(255, 255, 20, 147);
        public static Color DeepSkyBlue = Color.FromArgb(255, 0, 191, 255);
        public static Color DimGray = Color.FromArgb(255, 105, 105, 105);
        public static Color DodgerBlue = Color.FromArgb(255, 30, 144, 255);
        public static Color Firebrick = Color.FromArgb(255, 178, 34, 34);
        public static Color FloralWhite = Color.FromArgb(255, 255, 250, 240);
        public static Color ForestGreen = Color.FromArgb(255, 34, 139, 34);
        public static Color Fuchsia = Color.FromArgb(255, 255, 0, 255);
        public static Color Gainsboro = Color.FromArgb(255, 220, 220, 220);
        public static Color GhostWhite = Color.FromArgb(255, 248, 248, 255);
        public static Color Gold = Color.FromArgb(255, 255, 215, 0);
        public static Color Goldenrod = Color.FromArgb(255, 218, 165, 32);
        public static Color Gray = Color.FromArgb(255, 128, 128, 128);
        public static Color Green = Color.FromArgb(255, 0, 128, 0);
        public static Color GreenYellow = Color.FromArgb(255, 173, 255, 47);
        public static Color Honeydew = Color.FromArgb(255, 240, 255, 240);
        public static Color HotPink = Color.FromArgb(255, 255, 105, 180);
        public static Color IndianRed = Color.FromArgb(255, 205, 92, 92);
        public static Color Indigo = Color.FromArgb(255, 75, 0, 130);
        public static Color Ivory = Color.FromArgb(255, 255, 255, 240);
        public static Color Khaki = Color.FromArgb(255, 240, 230, 140);
        public static Color Lavender = Color.FromArgb(255, 230, 230, 250);
        public static Color LavenderBlush = Color.FromArgb(255, 255, 240, 245);
        public static Color LawnGreen = Color.FromArgb(255, 124, 252, 0);
        public static Color LemonChiffon = Color.FromArgb(255, 255, 250, 205);
        public static Color LightBlue = Color.FromArgb(255, 173, 216, 230);
        public static Color LightCoral = Color.FromArgb(255, 240, 128, 128);
        public static Color LightCyan = Color.FromArgb(255, 224, 255, 255);
        public static Color LightGoldenrodYellow = Color.FromArgb(255, 250, 250, 210);
        public static Color LightGray = Color.FromArgb(255, 211, 211, 211);
        public static Color LightGreen = Color.FromArgb(255, 144, 238, 144);
        public static Color LightPink = Color.FromArgb(255, 255, 182, 193);
        public static Color LightSalmon = Color.FromArgb(255, 255, 160, 122);
        public static Color LightSeaGreen = Color.FromArgb(255, 32, 178, 170);
        public static Color LightSkyBlue = Color.FromArgb(255, 135, 206, 250);
        public static Color LightSlateGray = Color.FromArgb(255, 119, 136, 153);
        public static Color LightSteelBlue = Color.FromArgb(255, 176, 196, 222);
        public static Color LightYellow = Color.FromArgb(255, 255, 255, 224);
        public static Color Lime = Color.FromArgb(255, 0, 255, 0);
        public static Color LimeGreen = Color.FromArgb(255, 50, 205, 50);
        public static Color Linen = Color.FromArgb(255, 250, 240, 230);
        public static Color Magenta = Color.FromArgb(255, 255, 0, 255);
        public static Color Maroon = Color.FromArgb(255, 128, 0, 0);
        public static Color MediumAquamarine = Color.FromArgb(255, 102, 205, 170);
        public static Color MediumBlue = Color.FromArgb(255, 0, 0, 205);
        public static Color MediumOrchid = Color.FromArgb(255, 186, 85, 211);
        public static Color MediumPurple = Color.FromArgb(255, 147, 112, 219);
        public static Color MediumSeaGreen = Color.FromArgb(255, 60, 179, 113);
        public static Color MediumSlateBlue = Color.FromArgb(255, 123, 104, 238);
        public static Color MediumSpringGreen = Color.FromArgb(255, 0, 250, 154);
        public static Color MediumTurquoise = Color.FromArgb(255, 72, 209, 204);
        public static Color MediumVioletRed = Color.FromArgb(255, 199, 21, 133);
        public static Color MidnightBlue = Color.FromArgb(255, 25, 25, 112);
        public static Color MintCream = Color.FromArgb(255, 245, 255, 250);
        public static Color MistyRose = Color.FromArgb(255, 255, 228, 225);
        public static Color Moccasin = Color.FromArgb(255, 255, 228, 181);
        public static Color NavajoWhite = Color.FromArgb(255, 255, 222, 173);
        public static Color Navy = Color.FromArgb(255, 0, 0, 128);
        public static Color OldLace = Color.FromArgb(255, 253, 245, 230);
        public static Color Olive = Color.FromArgb(255, 128, 128, 0);
        public static Color OliveDrab = Color.FromArgb(255, 107, 142, 35);
        public static Color Orange = Color.FromArgb(255, 255, 165, 0);
        public static Color OrangeRed = Color.FromArgb(255, 255, 69, 0);
        public static Color Orchid = Color.FromArgb(255, 218, 112, 214);
        public static Color PaleGoldenrod = Color.FromArgb(255, 238, 232, 170);
        public static Color PaleGreen = Color.FromArgb(255, 152, 251, 152);
        public static Color PaleTurquoise = Color.FromArgb(255, 175, 238, 238);
        public static Color PaleVioletRed = Color.FromArgb(255, 219, 112, 147);
        public static Color PapayaWhip = Color.FromArgb(255, 255, 239, 213);
        public static Color PeachPuff = Color.FromArgb(255, 255, 218, 185);
        public static Color Peru = Color.FromArgb(255, 205, 133, 63);
        public static Color Pink = Color.FromArgb(255, 255, 192, 203);
        public static Color Plum = Color.FromArgb(255, 221, 160, 221);
        public static Color PowderBlue = Color.FromArgb(255, 176, 224, 230);
        public static Color Purple = Color.FromArgb(255, 128, 0, 128);
        public static Color Red = Color.FromArgb(255, 255, 0, 0);
        public static Color RosyBrown = Color.FromArgb(255, 188, 143, 143);
        public static Color RoyalBlue = Color.FromArgb(255, 65, 105, 225);
        public static Color SaddleBrown = Color.FromArgb(255, 139, 69, 19);
        public static Color Salmon = Color.FromArgb(255, 250, 128, 114);
        public static Color SandyBrown = Color.FromArgb(255, 244, 164, 96);
        public static Color SeaGreen = Color.FromArgb(255, 46, 139, 87);
        public static Color SeaShell = Color.FromArgb(255, 255, 245, 238);
        public static Color Sienna = Color.FromArgb(255, 160, 82, 45);
        public static Color Silver = Color.FromArgb(255, 192, 192, 192);
        public static Color SkyBlue = Color.FromArgb(255, 135, 206, 235);
        public static Color SlateBlue = Color.FromArgb(255, 106, 90, 205);
        public static Color SlateGray = Color.FromArgb(255, 112, 128, 144);
        public static Color Snow = Color.FromArgb(255, 255, 250, 250);
        public static Color SpringGreen = Color.FromArgb(255, 0, 255, 127);
        public static Color SteelBlue = Color.FromArgb(255, 70, 130, 180);
        public static Color Tan = Color.FromArgb(255, 210, 180, 140);
        public static Color Teal = Color.FromArgb(255, 0, 128, 128);
        public static Color Thistle = Color.FromArgb(255, 216, 191, 216);
        public static Color Tomato = Color.FromArgb(255, 255, 99, 71);
        public static Color Turquoise = Color.FromArgb(255, 64, 224, 208);
        public static Color Violet = Color.FromArgb(255, 238, 130, 238);
        public static Color Wheat = Color.FromArgb(255, 245, 222, 179);
        public static Color White = Color.FromArgb(255, 255, 255, 255);
        public static Color WhiteSmoke = Color.FromArgb(255, 245, 245, 245);
        public static Color Yellow = Color.FromArgb(255, 255, 255, 0);
        public static Color YellowGreen = Color.FromArgb(255, 154, 205, 50);
        public static Color ButtonFace = Color.FromArgb(255, 240, 240, 240);
        public static Color ButtonHighlight = Color.FromArgb(255, 255, 255, 255);
        public static Color ButtonShadow = Color.FromArgb(255, 160, 160, 160);
        public static Color GradientActiveCaption = Color.FromArgb(255, 185, 209, 234);
        public static Color GradientInactiveCaption = Color.FromArgb(255, 215, 228, 242);
        public static Color MenuBar = Color.FromArgb(255, 240, 240, 240);
        public static Color MenuHighlight = Color.FromArgb(255, 51, 153, 255);

        #region Implementation
        public static Color FromKnownColor(KnownColor color)
        {
#if SILVERLIGHT
            Object obj = typeof(ColorConverter).GetField(color.ToString()).GetValue(null);
#else
            ColorConverter converter= new ColorConverter();            
            TypeInfo info = converter.GetType().GetTypeInfo();
            FieldInfo propertyFieldInfo = info.GetDeclaredField(color.ToString());
            object obj = propertyFieldInfo.GetValue(null);   
#endif
            if (obj != null)
                return (Color)obj;

            return new Color();
        }
        #endregion
    }
    #endregion

    #region Enums
    internal enum KnownColor
    {
        ActiveBorder = 1,
        ActiveCaption = 2,
        ActiveCaptionText = 3,
        AliceBlue = 28,
        AntiqueWhite = 0x1d,
        AppWorkspace = 29,
        Aqua = 30,
        Aquamarine = 31,
        Azure = 32,
        Beige = 33,
        Bisque = 34,
        Black = 35,
        BlanchedAlmond = 36,
        Blue = 37,
        BlueViolet = 38,
        Brown = 39,
        BurlyWood = 40,
        ButtonFace = 168,
        ButtonHighlight = 169,
        ButtonShadow = 170,
        CadetBlue = 41,
        Chartreuse = 42,
        Chocolate = 43,
        Control = 5,
        ControlDark = 6,
        ControlDarkDark = 7,
        ControlLight = 8,
        ControlLightLight = 9,
        ControlText = 10,
        Coral = 44,
        CornflowerBlue = 45,
        Cornsilk = 46,
        Crimson = 47,
        Cyan = 48,
        DarkBlue = 49,
        DarkCyan = 50,
        DarkGoldenrod = 51,
        DarkGray = 52,
        DarkGreen = 53,
        DarkKhaki = 54,
        DarkMagenta = 55,
        DarkOliveGreen = 56,
        DarkOrange = 57,
        DarkOrchid = 58,
        DarkRed = 59,
        DarkSalmon = 60,
        DarkSeaGreen = 61,
        DarkSlateBlue = 62,
        DarkSlateGray = 63,
        DarkTurquoise = 64,
        DarkViolet = 65,
        DeepPink = 66,
        DeepSkyBlue = 67,
        Desktop = 11,
        DimGray = 68,
        DodgerBlue = 69,
        Firebrick = 70,
        FloralWhite = 71,
        ForestGreen = 72,
        Fuchsia = 73,
        Gainsboro = 74,
        GhostWhite = 75,
        Gold = 76,
        Goldenrod = 77,
        GradientActiveCaption = 171,
        GradientInactiveCaption = 172,
        Gray = 78,
        GrayText = 12,
        Green = 79,
        GreenYellow = 80,
        Highlight = 13,
        HighlightText = 14,
        Honeydew = 81,
        HotPink = 82,
        HotTrack = 15,
        InactiveBorder = 16,
        InactiveCaption = 17,
        InactiveCaptionText = 18,
        IndianRed = 83,
        Indigo = 84,
        Info = 19,
        InfoText = 20,
        Ivory = 0x55,
        Khaki = 0x56,
        Lavender = 0x57,
        LavenderBlush = 0x58,
        LawnGreen = 0x59,
        LemonChiffon = 90,
        LightBlue = 0x5b,
        LightCoral = 0x5c,
        LightCyan = 0x5d,
        LightGoldenrodYellow = 0x5e,
        LightGray = 0x5f,
        LightGreen = 0x60,
        LightPink = 0x61,
        LightSalmon = 0x62,
        LightSeaGreen = 0x63,
        LightSkyBlue = 100,
        LightSlateGray = 0x65,
        LightSteelBlue = 0x66,
        LightYellow = 0x67,
        Lime = 0x68,
        LimeGreen = 0x69,
        Linen = 0x6a,
        Magenta = 0x6b,
        Maroon = 0x6c,
        MediumAquamarine = 0x6d,
        MediumBlue = 110,
        MediumOrchid = 0x6f,
        MediumPurple = 0x70,
        MediumSeaGreen = 0x71,
        MediumSlateBlue = 0x72,
        MediumSpringGreen = 0x73,
        MediumTurquoise = 0x74,
        MediumVioletRed = 0x75,
        Menu = 0x15,
        MenuBar = 0xad,
        MenuHighlight = 0xae,
        MenuText = 0x16,
        MidnightBlue = 0x76,
        MintCream = 0x77,
        MistyRose = 120,
        Moccasin = 0x79,
        NavajoWhite = 0x7a,
        Navy = 0x7b,
        OldLace = 0x7c,
        Olive = 0x7d,
        OliveDrab = 0x7e,
        Orange = 0x7f,
        OrangeRed = 0x80,
        Orchid = 0x81,
        PaleGoldenrod = 130,
        PaleGreen = 0x83,
        PaleTurquoise = 0x84,
        PaleVioletRed = 0x85,
        PapayaWhip = 0x86,
        PeachPuff = 0x87,
        Peru = 0x88,
        Pink = 0x89,
        Plum = 0x8a,
        PowderBlue = 0x8b,
        Purple = 140,
        Red = 0x8d,
        RosyBrown = 0x8e,
        RoyalBlue = 0x8f,
        SaddleBrown = 0x90,
        Salmon = 0x91,
        SandyBrown = 0x92,
        ScrollBar = 0x17,
        SeaGreen = 0x93,
        SeaShell = 0x94,
        Sienna = 0x95,
        Silver = 150,
        SkyBlue = 0x97,
        SlateBlue = 0x98,
        SlateGray = 0x99,
        Snow = 0x9a,
        SpringGreen = 0x9b,
        SteelBlue = 0x9c,
        Tan = 0x9d,
        Teal = 0x9e,
        Thistle = 0x9f,
        Tomato = 160,
        Transparent = 0x1b,
        Turquoise = 0xa1,
        Violet = 0xa2,
        Wheat = 0xa3,
        White = 0xa4,
        WhiteSmoke = 0xa5,
        Window = 0x18,
        WindowFrame = 0x19,
        WindowText = 0x1a,
        Yellow = 0xa6,
        YellowGreen = 0xa7
    }
    #endregion

    #region Extension Methods
    public static class Extensions
    {
        public static bool IsEmpty(this Color c)
        {
            return (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0);
        }

        public static int ToArgb(this Color c)
        {
            throw new NotImplementedException();
        }

        public static Color FromArgb(this Color c, int r, int g, int b)
        {
            Color color = new Color();
            color.R = (byte)r;
            color.G = (byte)g;
            color.B = (byte)b;

            return color;
        }

        public static Color FromArgb(this Color c, int argb)
        {
            byte b = (byte)argb;
            argb = argb >> 8;

            byte g = (byte)argb;
            argb = argb >> 8;

            byte r = (byte)argb;
            argb = argb >> 8;

            byte a = (byte)argb;

            return Color.FromArgb(a, r, g, b);
        }
    }
    #endregion

#if NETFX_CORE || WP
    #region Structs
    public struct PointF
    {
        #region Fields
        float x;
        float y;
        public static readonly PointF Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="PointF"/> class.
        /// </summary>
        static PointF()
        {
            Empty = new PointF();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointF"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is PointF))
            {
                return false;
            }

            PointF point = (PointF)obj;
            return ((point.x == this.X) && (point.y == this.Y));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + "}");
        }
        #endregion

        #region Overloads
        public static bool operator ==(PointF point1, PointF point2)
        {
            return ((point1.X == point2.X) && (point1.Y == point2.Y));
        }


        public static bool operator !=(PointF point1, PointF point2)
        {
            return !(point1 == point2);
        }
        #endregion
    }

    public struct RectangleF
    {
        #region Fields
        float x;
        float y;
        float width;
        float height;
        public static readonly RectangleF Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="RectangleF"/> class.
        /// </summary>
        static RectangleF()
        {
            Empty = new RectangleF();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleF"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleF"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="size">The size.</param>
        public RectangleF(PointF location, SizeF size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public PointF Location
        {
            get
            {
                return new PointF(this.x, this.y);
            }
            set
            {
                this.x = value.X;
                this.y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public SizeF Size
        {
            get
            {
                return new SizeF(this.width, this.height);
            }
            set
            {
                this.width = value.Width;
                this.height = value.Height;
            }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        public float Left
        {
            get
            {
                return this.x;
            }
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>The top.</value>
        public float Top
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public float Right
        {
            get
            {
                return (this.x + this.width);
            }
        }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public float Bottom
        {
            get
            {
                return (this.y + this.height);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is RectangleF))
            {
                return false;
            }
            RectangleF rect = (RectangleF)obj;
            return ((((rect.X == this.X) && (rect.Y == this.Y)) && (rect.Width == this.Width)) && (rect.Height == this.Height));

        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + ",Width=" + this.Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + this.Height.ToString(CultureInfo.CurrentCulture) + "}");
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Froms the LTRB.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right, bottom);
        }
        #endregion

        #region Overloads
        public static bool operator ==(RectangleF rectangle1, RectangleF rectangle2)
        {
            return ((rectangle1.X == rectangle2.X) && (rectangle1.Y == rectangle2.Y)
                && (rectangle1.Width == rectangle2.Width) && (rectangle1.Height == rectangle2.Height));
        }


        public static bool operator !=(RectangleF rectangle1, RectangleF rectangle2)
        {
            return !(rectangle1 == rectangle2);
        }

        public void Inflate(float x, float y)
        {
            this.X -= x;
            this.Y -= y;
            this.Width += 2f * x;
            this.Height += 2f * y;
        }

        public void Inflate(SizeF size)
        {
            this.Inflate(size.Width, size.Height);
        }
        #endregion
    }

    public struct SizeF
    {
        #region Fields
        float width;
        float height;
        public static readonly SizeF Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="SizeF"/> class.
        /// </summary>
        static SizeF()
        {
            Empty = new SizeF();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeF"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is SizeF))
            {
                return false;
            }

            SizeF size = (SizeF)obj;
            return ((size.Width == this.Width) && (size.Height == this.Height));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ("{Width=" + this.Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + this.Height.ToString(CultureInfo.CurrentCulture) + "}");
        }
        #endregion

        #region Overloads
        public static bool operator ==(SizeF size1, SizeF size2)
        {
            return ((size1.Width == size2.Width) && (size1.Height == size2.Height));
        }


        public static bool operator !=(SizeF size1, SizeF size2)
        {
            return !(size1 == size2);
        }
        #endregion
    }

    public struct Point
    {
        #region Fields
        int x;
        int y;
        public static readonly Point Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="PointF"/> class.
        /// </summary>
        static Point()
        {
            Empty = new Point();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointF"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is PointF))
            {
                return false;
            }

            Point point = (Point)obj;
            return ((point.x == this.X) && (point.y == this.Y));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + "}");
        }
        #endregion
    }

    public struct Rectangle
    {
        #region Fields
        int x;
        int y;
        int width;
        int height;
        public static readonly Rectangle Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="RectangleF"/> class.
        /// </summary>
        static Rectangle()
        {
            Empty = new Rectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleF"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get
            {
                return this.x;
            }
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public int Right
        {
            get
            {
                return (this.x + this.width);
            }
        }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom
        {
            get
            {
                return (this.y + this.height);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
            {
                return false;
            }
            Rectangle rect = (Rectangle)obj;
            return ((((rect.X == this.X) && (rect.Y == this.Y)) && (rect.Width == this.Width)) && (rect.Height == this.Height));

        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + ",Width=" + this.Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + this.Height.ToString(CultureInfo.CurrentCulture) + "}");
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Froms the LTRB.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        public static Rectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rectangle(left, top, right, bottom);
        }
        #endregion
    }

    public struct Size
    {
        #region Fields
        int width;
        int height;
        public static readonly Size Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="SizeF"/> class.
        /// </summary>
        static Size()
        {
            Empty = new Size();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeF"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is SizeF))
            {
                return false;
            }

            Size size = (Size)obj;
            return ((size.Width == this.Width) && (size.Height == this.Height));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ("{Width=" + this.Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + this.Height.ToString(CultureInfo.CurrentCulture) + "}");
        }
        #endregion
    }

    public class Color
    {
        internal byte A
        {
            get;
            set;
        }

        internal byte R
        {
            get;
            set;
        }

        internal byte G
        {
            get;
            set;
        }

        internal byte B
        {
            get;
            set;
        }

        internal static Color Empty
        {
            get
            {
                Color newColor = new Color();
                newColor.A = 0;
                newColor.R = 0;
                newColor.G = 0;
                newColor.B = 0;
                return newColor;
            }

        }

        public Color()
        {
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            Color argb = new Color();
            argb.A = a;
            argb.B = b;
            argb.R = r;
            argb.G = g;

            return argb;
        }

    }
    #endregion
#endif
}
#endif