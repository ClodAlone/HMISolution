using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Monogram.WpfUtils
{
  public class VisibilityConverter: IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool v = false;
      if (value is bool)
        v = (bool)value;
      else if (value is int)
      {
        v = (int)value != 0;
      }
      else if (value == null) v = false;
      else if (value is Visibility) v = (Visibility)value == Visibility.Visible;
      else v = true; // throw new ArgumentException("VisibilityConverter require boolean value.");

      Visibility fV = Visibility.Hidden;
      if (parameter != null)
      {
        string p = parameter.ToString();
        if (p.Length <= 2)
        {
          if (p.Contains("h"))
            fV = Visibility.Hidden;
          else if (p.Contains("c"))
            fV = Visibility.Collapsed;

          if (p.Contains("n")) //negation
            v = !v;
        }
      }


      if (v)
        return Visibility.Visible;
      else
        return fV;
      
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Visibility v = Visibility.Visible;
      if (value is Visibility)
        v = (Visibility)value;

      bool rv = false;
      if (v == Visibility.Visible)
        rv = true;

      if (parameter != null)
      {
        string p = parameter.ToString();
        if (p.Length <= 2)
        {
          if (p.Contains("n")) //negation
            rv = !rv;
        }
      }

      return rv;
    }

    #endregion
  }
}
