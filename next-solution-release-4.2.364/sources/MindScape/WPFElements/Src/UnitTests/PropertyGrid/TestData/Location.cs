using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TypeConverter(typeof(LocationTypeConverter))]
  public class Location : Entity
  {
    private readonly string _city;

    public string City
    {
      get { return _city; }
    }

    private Location(string city)
    {
      _city = city;
    }

    public static Location Auckland = new Location("Auckland");
    public static Location Wellington = new Location("Wellington");
    public static Location Christchurch = new Location("Christchurch");

    public override string ToString()
    {
      return "{" + _city + "}";
    }
  }

  public class LocationTypeConverter : TypeConverter
  {
    private static Location[] _standardValues = new Location[]
    {
      Location.Auckland,
      Location.Wellington,
      Location.Christchurch,
    };

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(_standardValues);
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

      if (destinationType == typeof(string))
      {
        return ((Location)value).City;
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
        foreach (Location location in _standardValues)
        {
          if (location.City == valueText)
          {
            return location;
          }
        }
      }

      throw new NotSupportedException("unknown location");
    }
  }
}
