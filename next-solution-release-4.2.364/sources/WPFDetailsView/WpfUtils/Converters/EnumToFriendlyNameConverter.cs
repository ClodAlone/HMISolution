using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace Monogram.WpfUtils
{
  [ValueConversion(typeof(object), typeof(String))]
  public class EnumToFriendlyNameConverter : IValueConverter
  {
    public static EnumToFriendlyNameConverter Instance
    {
      get
      {
        if (_Instance == null) _Instance = new EnumToFriendlyNameConverter();
        return _Instance;
      }
    }
    private static EnumToFriendlyNameConverter _Instance;

    #region IValueConverter implementation

    /// <summary>
    /// Convert value for binding from source object
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is int && parameter is Type)
      {
        //convert int value to enum defined in parameter
        value = Enum.ToObject((Type)parameter, (int)value);
      }

      //Convert enumvalue to string
      // To get around the stupid WPF designer bug
      if (value != null)
      {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        // To get around the stupid WPF designer bug
        if (fi != null)
        {
          //works with LocalizedDescriptionAttribute as well
          var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

          return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description)))
                     ? attributes[0].Description : value.ToString();
        }
      }

      return string.Empty;
    }

    /// <summary>
    /// ConvertBack value from binding back to source object
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new Exception("Cant convert back");
    }
    #endregion
  }
}
