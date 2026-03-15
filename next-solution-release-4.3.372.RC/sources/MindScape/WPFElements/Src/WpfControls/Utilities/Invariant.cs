using System;

using Mindscape.WpfElements.Properties;

namespace Mindscape.WpfElements
{
  internal static class Invariant
  {
    internal static void ArgumentNotNull(object argument, string argumentName)
    {
      if (argument == null)
      {
        throw new ArgumentNullException(argumentName);
      }
    }

    internal static void ArgumentNotNegative(int argument, string argumentName)
    {
      if (argument < 0)
      {
        throw new ArgumentOutOfRangeException(
          StringUtils.FormatCurrentCulture(Resources.ExpectedNonNegativeArgument, argumentName));
      }
    }

    internal static void ArgumentNotEmpty(string argument, string argumentName)
    {
      if (argument == null)
      {
        throw new ArgumentNullException(argumentName);
      }

      if (argument.Length == 0)
      {
        throw new ArgumentOutOfRangeException(
          StringUtils.FormatCurrentCulture(Resources.StringCannotBeEmpty, argumentName));
      }
    }
  }
}