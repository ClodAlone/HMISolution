using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A <see cref="TypeConverter"/> for converting a string into a char array.
  /// </summary>
  public class CharArrayTypeConverter : TypeConverter
  {
    /// <summary>
    /// Returns true if the sourceType is a string.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
    /// <returns>True if the sourceType is a string.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return Type.GetTypeCode(sourceType) == TypeCode.String;
    }

    /// <summary>
    /// Converts the given string value into a char array.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="Object"/> to convert.</param>
    /// <returns>A char array.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string)
      {
        return ((string)value).ToCharArray();
      }

      return value;
    }
  }
}
