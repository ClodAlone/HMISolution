using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements
{
  internal static class NumericalUtils
  {
    internal static bool IsPrimitiveNumerical(object o)
    {
      return o is double || o is int || o is decimal || o is long || o is float;
    }

    internal static double? ConvertToDouble(object o)
    {
      if (o is double)
      {
        return (double)o;
      }
      if (o is int)
      {
        return (double)(int)o;
      }
      if (o is decimal)
      {
        return (double)(decimal)o;
      }
      if (o is long)
      {
        return (double)(long)o;
      }
      if (o is float)
      {
        return Double.Parse(o.ToString());
        //return (double)(float)o;
      }
      return null;
    }

    // The given spacing value should be the minimum to maximum range divided by some suitable value.
    // This method takes the current spacing and makes it more ideal for logical spacing between tick marks.
    internal static double CalculateTickMarkSpacing(double spacing)
    {
      if (Double.IsInfinity(spacing))
      {
        return Double.PositiveInfinity;
      }
      int count = 0;
      bool small = spacing < 1;
      while (spacing < 1)
      {
        spacing *= 10;
        count++;
      }
      while (spacing > 10)
      {
        spacing /= 10;
        count++;
      }

      if (spacing < 1.5)
      {
        spacing = 1;
      }
      else if (spacing < 2)
      {
        spacing = 2;
      }
      else if (spacing < 3.5)
      {
        spacing = 5;
      }
      else
      {
        spacing = 10;
      }
      if (small)
      {
        spacing /= Math.Pow(10, count);
      }
      else
      {
        spacing *= Math.Pow(10, count);
      }
      return spacing;
    }

    internal static int CoerceIndexToInteger(double index, int numberOfItems)
    {
      if (Double.IsNegativeInfinity(index))
      {
        return 0;
      }
      if (Double.IsPositiveInfinity(index))
      {
        return numberOfItems - 1;
      }
      int num = (int)index;
      return Math.Max(Math.Min(numberOfItems - 1, num), 0);
    }

    internal static bool GreaterThanOrClose(double value1, double value2)
    {
      if (value1 <= value2)
      {
        return AreClose(value1, value2);
      }
      return true;
    }

    internal static bool LessThan(double value1, double value2)
    {
      return value1 < value2 && !AreClose(value1, value2);
    }

    internal static bool LessThanOrClose(double value1, double value2)
    {
      if (value1 >= value2)
      {
        return AreClose(value1, value2);
      }
      return true;
    }

    internal static bool AreClose(Size size1, Size size2)
    {
      return AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height);
    }

    internal static bool AreClose(Vector vector1, Vector vector2)
    {
      return AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y);
    }

    internal static bool AreClose(double value1, double value2)
    {
      if (value1 == value2)
      {
        return true;
      }
      double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
      double num2 = value1 - value2;
      return -num < num2 && num > num2;
    }
  }
}
