#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

#if WINRT
using Windows.UI.Xaml.Media;
using Windows.UI;
#else
using System.Windows.Media;
using System.Windows;
#endif

namespace Syncfusion.RDL.Internal
{
    internal class ReportingColorCoverter
    {
        public static string GetColorString(Brush brush)
        {
            if (brush != null)
            {
                string color = brush.ToString();

                if (color.Contains("#"))
                {
                    if (color.Length == 9)
                    {
                        return "#" + color.Substring(3);
                    }

                    return color;
                }
            }
            return "Transparent";
        }
    }


    internal class ReportingBrushConverter
    {

#if !SILVERLIGHT
        BrushConverter converter = new BrushConverter();
#else
        internal static Dictionary<string, string> colors;


        public ReportingBrushConverter()
        {
            colors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            colors.Add("Transparent", "#00ffffff");
            colors.Add("AliceBlue", "#f0f8ff");
            colors.Add("AntiqueWhite", "#faebd7");
            colors.Add("Aqua", "#00ffff");
            colors.Add("Aquamarine", "#7fffd4");
            colors.Add("Azure", "#f0ffff");
            colors.Add("Beige", "#f5f5dc");
            colors.Add("Bisque", "#ffe4c4");
            colors.Add("Black", "#000000");
            colors.Add("BlanchedAlmond", "#ffebcd");
            colors.Add("Blue", "#0000ff");
            colors.Add("BlueViolet", "#8a2be2");
            colors.Add("Brown", "#a52a2a");
            colors.Add("BurlyWood", "#deb887");
            colors.Add("CadetBlue", "#5f9ea0");
            colors.Add("Chartreuse", "#7fff00");
            colors.Add("Chocolate", "#d2691e");
            colors.Add("Coral", "#ff7f50");
            colors.Add("CornflowerBlue", "#6495ed");
            colors.Add("Cornsilk", "#fff8dc");
            colors.Add("Crimson", "#dc143c");
            colors.Add("Cyan", "#00ffff");
            colors.Add("DarkBlue", "#00008b");
            colors.Add("DarkCyan", "#008b8b");
            colors.Add("DarkGoldenrod", "#b8860b");
            colors.Add("DarkGray", "#a9a9a9");
            colors.Add("DarkGreen", "#006400");
            colors.Add("DarkKhaki", "#bdb76b");
            colors.Add("DarkMagenta", "#8b008b");
            colors.Add("DarkOliveGreen", "#556b2f");
            colors.Add("DarkOrange", "#ff8c00");
            colors.Add("DarkOrchid", "#9932cc");
            colors.Add("DarkRed", "#8b0000");
            colors.Add("DarkSalmon", "#e9967a");
            colors.Add("DarkSeaGreen", "#8fbc8b");
            colors.Add("DarkSlateBlue", "#483d8b");
            colors.Add("DarkSlateGray", "#2f4f4f");
            colors.Add("DarkTurquoise", "#00ced1");
            colors.Add("DarkViolet", "#9400d3");
            colors.Add("DeepPink", "#ff1493");
            colors.Add("DeepSkyBlue", "#00bfff");
            colors.Add("DimGray", "#696969");
            colors.Add("DodgerBlue", "#1e90ff");
            colors.Add("Firebrick", "#b22222");
            colors.Add("FloralWhite", "#fffaf0");
            colors.Add("ForestGreen", "#228b22");
            colors.Add("Fuchsia", "#ff00ff");
            colors.Add("Gainsboro", "#dcdcdc");
            colors.Add("GhostWhite", "#f8f8ff");
            colors.Add("Gold", "#ffd700");
            colors.Add("Goldenrod", "#daa520");
            colors.Add("Gray", "#808080");
            colors.Add("Green", "#008000");
            colors.Add("GreenYellow", "#adff2f");
            colors.Add("Honeydew", "#f0fff0");
            colors.Add("HotPink", "#ff69b4");
            colors.Add("IndianRed", "#cd5c5c");
            colors.Add("Indigo", "#4b0082");
            colors.Add("Ivory", "#fffff0");
            colors.Add("Khaki", "#f0e68c");
            colors.Add("Lavender", "#e6e6fa");
            colors.Add("LavenderBlush", "#fff0f5");
            colors.Add("LawnGreen", "#7cfc00");
            colors.Add("LemonChiffon", "#fffacd");
            colors.Add("LightBlue", "#add8e6");
            colors.Add("LightCoral", "#f08080");
            colors.Add("LightCyan", "#e0ffff");
            colors.Add("LightGoldenrodYellow", "#fafad2");
            colors.Add("LightGreen", "#90ee90");
            colors.Add("LightGray", "#d3d3d3");
            colors.Add("LightGrey", "#d3d3d3");
            colors.Add("LightPink", "#ffb6c1");
            colors.Add("LightSalmon", "#ffa07a");
            colors.Add("LightSeaGreen", "#20b2aa");
            colors.Add("LightSkyBlue", "#87cefa");
            colors.Add("LightSlateGray", "#778899");
            colors.Add("LightSteelBlue", "#b0c4de");
            colors.Add("LightYellow", "#ffffe0");
            colors.Add("Lime", "#00ff00");
            colors.Add("LimeGreen", "#32cd32");
            colors.Add("Linen", "#faf0e6");
            colors.Add("Magenta", "#ff00ff");
            colors.Add("Maroon", "#800000");
            colors.Add("MediumAquamarine", "#66cdaa");
            colors.Add("MediumBlue", "#0000cd");
            colors.Add("MediumOrchid", "#ba55d3");
            colors.Add("MediumPurple", "#9370db");
            colors.Add("MediumSeaGreen", "#3cb371");
            colors.Add("MediumSlateBlue", "#7b68ee");
            colors.Add("MediumSpringGreen", "#00fa9a");
            colors.Add("MediumTurquoise", "#48d1cc");
            colors.Add("MediumVioletRed", "#c71585");
            colors.Add("MidnightBlue", "#191970");
            colors.Add("MintCream", "#f5fffa");
            colors.Add("MistyRose", "#ffe4e1");
            colors.Add("Moccasin", "#ffe4b5");
            colors.Add("NavajoWhite", "#ffdead");
            colors.Add("Navy", "#000080");
            colors.Add("OldLace", "#fdf5e6");
            colors.Add("Olive", "#808000");
            colors.Add("OliveDrab", "#6b8e23");
            colors.Add("Orange", "#ffa500");
            colors.Add("OrangeRed", "#ff4500");
            colors.Add("Orchid", "#da70d6");
            colors.Add("PaleGoldenrod", "#eee8aa");
            colors.Add("PaleGreen", "#98fb98");
            colors.Add("PaleTurquoise", "#afeeee");
            colors.Add("PaleVioletRed", "#db7093");
            colors.Add("PapayaWhip", "#ffefd5");
            colors.Add("PeachPuff", "#ffdab9");
            colors.Add("Peru", "#cd853f");
            colors.Add("Pink", "#ffc0cb");
            colors.Add("Plum", "#dda0dd");
            colors.Add("PowderBlue", "#b0e0e6");
            colors.Add("Purple", "#800080");
            colors.Add("Red", "#ff0000");
            colors.Add("RosyBrown", "#bc8f8f");
            colors.Add("RoyalBlue", "#4169e1");
            colors.Add("SaddleBrown", "#8b4513");
            colors.Add("Salmon", "#fa8072");
            colors.Add("SandyBrown", "#f4a460");
            colors.Add("SeaGreen", "#2e8b57");
            colors.Add("SeaShell", "#fff5ee");
            colors.Add("Sienna", "#a0522d");
            colors.Add("Silver", "#c0c0c0");
            colors.Add("SkyBlue", "#87ceeb");
            colors.Add("SlateBlue", "#6a5acd");
            colors.Add("SlateGray", "#708090");
            colors.Add("Snow", "#fffafa");
            colors.Add("SpringGreen", "#00ff7f");
            colors.Add("SteelBlue", "#4682b4");
            colors.Add("Tan", "#d2b48c");
            colors.Add("Teal", "#008080");
            colors.Add("Thistle", "#d8bfd8");
            colors.Add("Tomato", "#ff6347");
            colors.Add("Turquoise", "#40e0d0");
            colors.Add("Violet", "#ee82ee");
            colors.Add("Wheat", "#f5deb3");
            colors.Add("White", "#ffffff");
            colors.Add("WhiteSmoke", "#f5f5f5");
            colors.Add("Yellow", "#ffff00");
            colors.Add("YellowGreen", "#9acd32");

        }
#endif

        public Brush ConvertFromString(string color)
        {
            if (!string.IsNullOrEmpty(color))
            {
                if (color.Equals("LightGrey"))
                {
                    color = Colors.LightGray.ToString();
                }
            }

#if !SILVERLIGHT
            if (!string.IsNullOrEmpty(color))
            {
                return (Brush)converter.ConvertFromString(color);
            }

            return Brushes.Transparent;
#else
            if (color != null)
            {
                if (color.Contains("#"))
                {
                    return ConvertFromInvariantString(color);
                }
                else if (colors.Keys.Contains(color))
                {
                    return ConvertFromInvariantString(colors[color]);
                }
            }

            return new SolidColorBrush(Colors.Transparent);
#endif
        }
        public Brush ConvertFromInvariantString(string color)
        {
#if !SILVERLIGHT
            if (!string.IsNullOrEmpty(color))
            {
                if (color.Equals("LightGrey"))
                {
                    color = Colors.LightGray.ToString();
                }
            }
            if (!string.IsNullOrEmpty(color))
            {
                return (Brush)converter.ConvertFromInvariantString(color);
            }

            return Brushes.Transparent;
#else
            if (!string.IsNullOrEmpty(color) && color.Contains("#"))
            {
                byte colorStringPosition = 0;
                string convertColor = color.ToString();
                convertColor = convertColor.Replace("#", "");
                byte alphaValue = System.Convert.ToByte("ff", 16);
                if (convertColor.Length == 8)
                {
                    // get the alpha channel value
                    alphaValue = System.Convert.ToByte(convertColor.Substring(colorStringPosition, 2), 16);
                    colorStringPosition = 2;
                }

                // get the red value
                byte red = System.Convert.ToByte(convertColor.Substring(colorStringPosition, 2), 16);
                colorStringPosition += 2;
                // get the green value
                byte green = System.Convert.ToByte(convertColor.Substring(colorStringPosition, 2), 16);
                colorStringPosition += 2;
                // get the blue value
                byte blue = System.Convert.ToByte(convertColor.Substring(colorStringPosition, 2), 16);

                // create the SolidColorBrush object
                Brush brush = new SolidColorBrush(Color.FromArgb(alphaValue, red, green, blue));
                return brush;
            }
            else
                return this.ConvertFromString(color);
#endif
        }
    }
}
