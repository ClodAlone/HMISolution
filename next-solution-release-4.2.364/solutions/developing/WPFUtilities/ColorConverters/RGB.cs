using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RGBHSL
{
    public class RGB
    {
        public decimal R { get; set; }
        public decimal G { get; set; }
        public decimal B { get; set; }

        public Color ToColor()
        {
            int r = (int)Math.Round(R * 255M);
            int g = (int)Math.Round(G * 255M);
            int b = (int)Math.Round(B * 255M);
            return Color.FromArgb(255, r, g, b);
        }
    }
}
