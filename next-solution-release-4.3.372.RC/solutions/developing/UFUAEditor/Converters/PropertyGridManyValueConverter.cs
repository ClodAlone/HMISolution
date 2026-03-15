using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Mindscape.WpfElements.PropertyEditing;

namespace UFUAEditor.Converters
{
    public class PropertyGridManyValueConverter : IValueConverter
    {
        #region Declarations

        Many lastValue;

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Many)
            {
                lastValue = value as Many;
                foreach (var v in lastValue.Values)
                    return v;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (lastValue != null && lastValue.GetType() == targetType)
            {
                (lastValue as dynamic).Value = value as dynamic;
                return lastValue;
            }

            return value;
        }

        #endregion
    }
}
