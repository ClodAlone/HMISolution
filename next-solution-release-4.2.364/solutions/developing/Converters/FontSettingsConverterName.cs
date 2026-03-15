using System;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Data;
#endif
#endif
using System.Globalization;
using Utilities;
using Utilities.Converters;

namespace Converters
{
    public class FontSettingsConverterName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

#if !WINDOWS_UWP
#if !NET_STANDARD
            if (value != null)
            {
                FontSettings fontsetting = value as FontSettings;
                if (fontsetting != null)
                    return String.Format("{0}({1})", fontsetting.FontFamily,fontsetting.FontSize);
            }

#endif
#endif
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
