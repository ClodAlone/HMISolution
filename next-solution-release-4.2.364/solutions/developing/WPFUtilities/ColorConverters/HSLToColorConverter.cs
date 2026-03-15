using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RGBHSL
{
    static public class HSLToColorConverter
    {
        static public Color Convert(HSL hsl)
        {
            RGB rgb = new RGB();

            if (hsl.S == 0M)
                rgb.R = rgb.G = rgb.B = hsl.L;
            else
                rgb = GetRGBFromHSLWithChroma(hsl);

            return rgb.ToColor();
        }



        static private RGB GetRGBFromHSLWithChroma(HSL hsl)
        {
            decimal min, max, h;

            h = hsl.H / 360M;

            max = hsl.L < 0.5M ? hsl.L * (1 + hsl.S) : (hsl.L + hsl.S) - (hsl.L * hsl.S);
            min = (hsl.L * 2M) - max;

            RGB rgb = new RGB();
            rgb.R = ComponentFromHue(min, max, h + (1M / 3M));
            rgb.G = ComponentFromHue(min, max, h);
            rgb.B = ComponentFromHue(min, max, h - (1M / 3M));
            return rgb;
        }



        static private decimal ComponentFromHue(decimal m1, decimal m2, decimal h)
        {
            h = (h + 1M) % 1M;
            if ((h * 6M) < 1)
                return m1 + (m2 - m1) * 6M * h;
            else if ((h * 2M) < 1)
                return m2;
            else if ((h * 3M) < 2)
                return m1 + (m2 - m1) * ((2M / 3M) - h) * 6M;
            else
                return m1;
        }
    }
}
