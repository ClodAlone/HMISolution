using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Reflection;

namespace Mindscape.WpfElements.WpfDataGrid
{
  internal class BindingAdapterConverter : IMultiValueConverter
  {
    private readonly BindingPropertyInfoAdapter _adapter;

    public BindingAdapterConverter(BindingPropertyInfoAdapter adapter)
    {
      _adapter = adapter;
    }

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0] != null)
      {
        object invalidValue = null;
        _adapter.ValidationCache.TryGetValue(values[0], out invalidValue);
        if (invalidValue != null)
        {
          return invalidValue;
        }
      }
      /*if (values[1] != null && _adapter.PropertyType.Equals(typeof(double)))
      {
        return String.Format(_adapter.Binding.StringFormat, values[1]);
      }*/
      return values[1];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
