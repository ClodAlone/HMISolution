using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Converters
{
    public class EnumeratedValueConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[0] == null || values[1] == null)
                return null;
            
            IList _list = values[1] as IList;

            if (_list.Contains(values[0].ToString()))
                return _list.IndexOf(values[0].ToString());

            Opc.Ua.LocalizedText _value = new Opc.Ua.LocalizedText(values[0].ToString());
            if (_list.Contains(_value))
                return _list.IndexOf(_value);

            return  values[0].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[]{value.ToString(), Binding.DoNothing};
        }
        #endregion
    }
}
