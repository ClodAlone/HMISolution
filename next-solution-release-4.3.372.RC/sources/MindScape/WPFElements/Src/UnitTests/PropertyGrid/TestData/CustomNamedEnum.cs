using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TypeConverter(typeof(CustomNamedEnumConverter))]
  public enum CustomNamedEnum
  {
    FirstValue,
    SecondValue,
  }

  public class CustomNamedEnumConverter : TypeConverter
  {
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(Enum.GetValues(typeof(CustomNamedEnum)));
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (value == null)
      {
        return value;
      }

      if (value is CustomNamedEnum && destinationType == typeof(string))
      {
        return ((CustomNamedEnum)value) == CustomNamedEnum.FirstValue ? "Value 1" : "Value 2";
      }

      return value;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      string valueText = value as string;
      if (valueText != null)
      {
        return valueText == "Value 1" ? CustomNamedEnum.FirstValue : CustomNamedEnum.SecondValue;
      }

      throw new NotSupportedException("unknown text");
    }
  }
}
