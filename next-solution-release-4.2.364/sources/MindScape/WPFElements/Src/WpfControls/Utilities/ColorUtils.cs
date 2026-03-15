using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;

namespace Mindscape.WpfElements
{
  internal static class ColorUtils
  {
    public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
    {
      /*double max = Math.Max(color.R, Math.Max(color.G, color.B));
      double min = Math.Min(color.R, Math.Min(color.G, color.B));

      double r = color.R / 255.0;
      double g = color.G / 255.0;
      double b = color.B / 255.0;

      if (max == color.R)
      {
        hue = (int)(60.0 * (g - b));
        if (hue < 0)
        {
          hue += 360.0;
        }
      }
      else if (max == color.G)
      {
        hue = (int)(120.0 + 60.0 * (b - r));
      }
      else
      {
        hue = (int)(240.0 + 60.0 * (r - g));
      }

      //hue = color.GetHue();
      saturation = (int)((max == 0) ? 0.0 : (1.0 - (1.0 * min / max)) * 100.0);
      value = (int)(max / 255.0 * 100.0);
      //Debug.WriteLine("H: " + hue + ", S: " + saturation + " ,V: " + value);*/
      double r = color.R / 255.0;
      double g = color.G / 255.0;
      double b = color.B / 255.0;

      double max = Math.Max(r, Math.Max(g, b));
      double min = Math.Min(r, Math.Min(g, b));
      double chroma = max - min;

      double h = Double.NaN;
      if (chroma != 0)
      {
        if (max == r)
        {
          h = ((g - b) / chroma);
          if (h < 0)
          {
            h += 6.0;
          }
        }
        else if (max == g)
        {
          h = (b - r) / chroma + 2.0;
        }
        else
        {
          h = (r - g) / chroma + 4.0;
        }
      }
      hue = 60.0 * h;
      value = max;
      saturation = chroma == 0 ? 0 : chroma / max;
    }

    public static Color ColorFromHSV(double hue, double saturation, double value)
    {
      int hi = Double.IsNaN(hue) ? 0 : Convert.ToInt32(Math.Floor(hue / 60.0)) % 6;
      double f = Double.IsNaN(hue) ? 0 : (hue / 60.0 - Math.Floor(hue / 60.0));

      //value = (value / 100.0);
      //saturation /= 100.0;
      byte v = (byte)Convert.ToInt32(value * 255.0);
      byte p = (byte)Convert.ToInt32(value * (1 - saturation) * 255.0);
      byte q = (byte)Convert.ToInt32(value * (1 - f * saturation) * 255.0);
      byte t = (byte)Convert.ToInt32(value * (1 - (1 - f) * saturation) * 255.0);

      if (hi == 0)
        return Color.FromArgb(255, v, t, p);
      else if (hi == 1)
        return Color.FromArgb(255, q, v, p);
      else if (hi == 2)
        return Color.FromArgb(255, p, v, t);
      else if (hi == 3)
        return Color.FromArgb(255, p, q, v);
      else if (hi == 4)
        return Color.FromArgb(255, t, p, v);
      else
        return Color.FromArgb(255, v, p, q);
    }

    public static Color? ColorFromHexString(string str)
    {
      str = str.Trim().Replace("#", "");
      if (str.Length == 3)
      {
        if (!AreValidHexValues(str))
        {
          return null;
        }
        byte r = GetChannelValue(str.Substring(0, 1));
        byte g = GetChannelValue(str.Substring(1, 1));
        byte b = GetChannelValue(str.Substring(2, 1));
        return Color.FromRgb(r, g, b);
      }
      else if (str.Length == 6)
      {
        if (!AreValidHexValues(str))
        {
          return null;
        }
        byte r = GetChannelValue(str.Substring(0, 2));
        byte g = GetChannelValue(str.Substring(2, 2));
        byte b = GetChannelValue(str.Substring(4, 2));
        return Color.FromRgb(r, g, b);
      }
      else if (str.Length == 8)
      {
        if (!AreValidHexValues(str))
        {
          return null;
        }
        byte a = GetChannelValue(str.Substring(0, 2));
        byte r = GetChannelValue(str.Substring(2, 2));
        byte g = GetChannelValue(str.Substring(4, 2));
        byte b = GetChannelValue(str.Substring(6, 2));
        return Color.FromArgb(a, r, g, b);
      }
      else
      {
        return null;
      }
    }

    private static bool AreValidHexValues(string str)
    {
      str = str.ToUpper();
      foreach (char ch in str)
      {
        if (!Char.IsDigit(ch) && !ch.Equals('A') && !ch.Equals('B') && !ch.Equals('C') && !ch.Equals('D') && !ch.Equals('E') && !ch.Equals('F'))
        {
          return false;
        }
      }
      return true;
    }

    private static byte GetChannelValue(string str)
    {
      if (str.Length == 1)
      {
        str = str + str;
      }
      return (byte)(Convert.ToUInt32(str, 16));
    }

    public static string ColorToHexString(Color color, bool isHashDisplayed, bool alwaysShowAlpha, bool minimizeIfPossible)
    {
      string str = color.ToString();
      str = str.Substring(1);
      if (!alwaysShowAlpha)
      {
        if (str.Substring(0, 2).Equals("FF"))
        {
          str = str.Substring(2);
          if (minimizeIfPossible)
          {
            if (str[0].Equals(str[1]) && str[2].Equals(str[3]) && str[4].Equals(str[5]))
            {
              str = str[0].ToString() + str[2].ToString() + str[4].ToString();
            }
          }
        }
      }
      if (isHashDisplayed)
      {
        str = '#' + str;
      }
      return str;
    }
  }
}
