using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mindscape.WpfElements
{
  internal static class StringUtils
  {
    internal static string FormatCurrentCulture(string format, params object[] args)
    {
      return String.Format(CultureInfo.CurrentCulture, format, args);
    }

    internal static string FormatInvariant(string format, params object[] args)
    {
      return String.Format(CultureInfo.InvariantCulture, format, args);
    }

    internal static string Humanize(string name)
    {
      return Regex.Replace(name, "([A-Z][A-Z]*)", " $1", RegexOptions.Compiled).Trim();
    }
  }
}