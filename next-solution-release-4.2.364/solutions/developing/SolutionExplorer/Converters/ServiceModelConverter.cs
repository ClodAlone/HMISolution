using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UFInterfaces;
using Utilities.Converters;

namespace UFProjectManager.Converters
{
    public class ServiceModelConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string itemString = value?.ToString();
            try
            {
                ResourceEnumConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(value.GetType()) as ResourceEnumConverter;
                if (converter != null)
                    itemString = converter.ConvertTo(value, typeof(string)) as String;
            }
            catch { }

            return itemString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
