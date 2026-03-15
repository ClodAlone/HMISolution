using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  // TODO: test
  /// <summary>
  /// Converts a numeric <see cref="IFilter"/> into a string expression and vice versa.
  /// </summary>
  public class NumericFilterToStringConverter : IValueConverter
  {
    /// <summary>
    /// Converts a numeric <see cref="IFilter"/> to a string expression.
    /// </summary>
    /// <param name="value">The <see cref="IFilter"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string expression of the filter.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      IFilter filter = (IFilter)value;
      if (filter != null)
      {
        // TODO
      }
      return "";
    }

    /// <summary>
    /// Converts a string to an <see cref="IFilter"/>.
    /// </summary>
    /// <param name="value">The string value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An <see cref="IFilter"/> based on the string expression.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string str = (string)value;
      if (String.IsNullOrEmpty(str)) // TODO: check for white space.
      {
        return null;
      }

      string[] parts = str.Split(',');
      OrFilter mainFilter = new OrFilter();
      IFilter lastFilter = null;
      int count = 0;
      foreach (string part in parts)
      {
        IFilter filter = Parse(part);
        if (filter != null)
        {
          lastFilter = filter;
          mainFilter.Add(filter);
          count++;
        }
      }

      return count > 1 ? mainFilter : lastFilter;
    }

    private IFilter Parse(string str)
    {
      str = str.Replace(" ", "");

      if (str.StartsWith(">="))
      {
        str = str.Replace(">=", "");
        double? number = GetNumber(str);
        return number == null ? null : new GreaterThanOrEqualToFilter(number.Value);
      }
      if (str.StartsWith("<="))
      {
        str = str.Replace("<=", "");
        double? number = GetNumber(str);
        return number == null ? null : new LessThanOrEqualToFilter(number.Value);
      }
      if (str.StartsWith(">"))
      {
        str = str.Replace(">", "");
        double? number = GetNumber(str);
        return number == null ? null : new GreaterThanFilter(number.Value);
      }
      if (str.StartsWith("<"))
      {
        str = str.Replace("<", "");
        double? number = GetNumber(str);
        return number == null ? null : new LessThanFilter(number.Value);
      }

      if (str.Contains("-") && str.Length > 1)
      {
        int index = str.IndexOf('-', 1); // This character is also used for negative numbers, so start the search from index 1.
        if (index < str.Length - 1 && index > 0)
        {
          string left = str.Substring(0, index);
          string right = str.Substring(index + 1);
          double? leftNumber = GetNumber(left);
          double? rightNumber = GetNumber(right);
          if (leftNumber != null && rightNumber != null)
          {
            return new RangeFilter(leftNumber.Value, rightNumber.Value);
          }
        }
      }

      if (str.StartsWith("!") || str.StartsWith("not", StringComparison.OrdinalIgnoreCase))
      {
        str = str.Replace("!=", "").Replace("!", "").ToLower().Replace("not", "");
        double? number = GetNumber(str);
        return number == null ? null : new NotEqualFilter(number.Value);
      }

      if (str.StartsWith("="))
      {
        str = str.Replace("=", "");
      }

      while (str.EndsWith("-"))
      {
        str = str.Substring(0, str.Length - 1);
      }

      double? n = GetNumber(str);
      return n == null ? null : new EqualsFilter(n.Value);
    }

    private double? GetNumber(string str)
    {
      double number;
      bool success = Double.TryParse(str, out number);
      if (success)
      {
        return number;
      }
      return null;
    }
  }
}
