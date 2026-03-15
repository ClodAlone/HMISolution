using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class Department : Entity
  {
    private string _name;

    public string Name
    {
      get { return _name; }
      set { Set(ref _name, value, "Name"); }
    }

    private Location _location;

    public Location Location
    {
      get { return _location; }
      set { Set(ref _location, value, "Location"); }
    }

    private Puppy _mascot;

    [TypeConverter(typeof(DepartmentalMascotConverter))]
    public Puppy Mascot
    {
      get { return _mascot; }
      set { Set(ref _mascot, value, "Mascot"); }
    }
  }

  public class DepartmentalMascotConverter : TypeConverter
  {
    public static Puppy Fifi = new Puppy("Fifi", 123);
    public static Puppy Butch = new Puppy("Butch", 0);

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(new Puppy[] { Fifi, Butch });
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      string valueText = value as string;

      switch (valueText)
      {
        case "Fifi": return Fifi;
        case "Butch": return Butch;
        default: return null;
      }
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        return ((Puppy)value).Name;
      }
      return "(No Mascot)";
    }
  }
}
