using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PropertyControl
{
    [ValueConversion(typeof(PropertyFilterEnum), typeof(Boolean))]
    public class EnumToFilterTypeConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (targetType != typeof(Nullable<Boolean>))
                throw new InvalidOperationException("The target must be a bool");

            var input = (PropertyFilterEnum)value;
            PropertyFilterEnum filter = PropertyFilterEnum.GroupByCategory;
            if (parameter != null && parameter is String)
                Enum.TryParse(parameter as String, out filter);

            return input == filter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return PropertyFilterEnum.GroupByCategory;

            if (targetType != typeof(PropertyFilterEnum))
                throw new InvalidOperationException("The target must be a PropertyFilterEnum");

            Boolean input = (Boolean)value;
            PropertyFilterEnum filter = PropertyFilterEnum.GroupByCategory;
            if (parameter != null && parameter is String)
                Enum.TryParse(parameter as String, out filter);

            if (input)
                return filter;

            if (filter != PropertyFilterEnum.SortAlphabetically)
                return PropertyFilterEnum.SortAlphabetically;

            return PropertyFilterEnum.GroupByCategory;
        }
        #endregion
    }
}
