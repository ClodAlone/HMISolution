using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RGBHSL
{
    static public class ColorToHSLConverter
    {
        static public HSL Convert(Color color)
        {
            HSL hsl = new HSL();
            RGB rgb = GetRGBFromColor(color);

            decimal max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
            decimal min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            decimal chroma = max - min;

            hsl.L = GetL(max, min);

            if (chroma != 0)
            {
                hsl.H = GetH(rgb, max, chroma);
                hsl.S = GetS(hsl.L, chroma);
            }
            return hsl;
        }



        static private RGB GetRGBFromColor(Color color)
        {
            RGB rgb = new RGB();
            rgb.R = color.R / 255M;
            rgb.G = color.G / 255M;
            rgb.B = color.B / 255M;
            return rgb;
        }



        static private decimal GetL(decimal max, decimal min)
        {
            return (max + min) / 2M;
        }



        static private decimal GetH(RGB rgb, decimal max, decimal chroma)
        {
            decimal h;
            if (rgb.R == max)
                h = ((rgb.G - rgb.B) / chroma);
            else if (rgb.G == max)
                h = ((rgb.B - rgb.R) / chroma) + 2M;
            else
                h = ((rgb.R - rgb.G) / chroma) + 4M;
            return 60M * ((h + 6M) % 6M);
        }



        static private decimal GetS(decimal l, decimal chroma)
        {
            return l <= 0.5M ? chroma / (l * 2M) : chroma / (2M - 2M * l);
        }
    }
}
