using DevExpress.DocumentView;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using TranslationHelpers;

namespace AlarmWindow.Converters
{
    public class MessageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var message = values[1] as string;
            if (values.Length < 2 || String.IsNullOrEmpty(message))
                return String.Empty;
            
            if (values[0] is null)
                return message;
            
            if (!(values[0] is Dictionary<string, string>))
                return String.Empty;

            var stringlist = (Dictionary<string, string>)values[0];
            return TranslationHelper.TranslateComposedText(message, stringlist, message);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
