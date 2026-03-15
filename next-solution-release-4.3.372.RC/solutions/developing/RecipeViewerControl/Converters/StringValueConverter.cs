using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Collections.Generic;
using TranslationHelpers;

namespace RecipeViewerControl.Converters
{
    public class StringValueConverter : IValueConverter
    {
        public IDictionary<String, String> stringlist;
        public String stringPlaceolder;

        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return value;

            var _value = TranslationHelper.TranlslateText($"_{stringPlaceolder}_{value}", stringlist, value.ToString());
            return _value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
