using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace ClientEditor.Converters
{
    public class DisplayNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Get display name for given instance type and property name
                var attribute = typeof(OPCUAViewModel.AppNameSettings)
                    .GetProperty(parameter.ToString())
                    .GetCustomAttributes(false)
                    .OfType<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                    .FirstOrDefault();

                return attribute != null ? attribute.GetName() : parameter.ToString();
            }
            catch
            {
                return parameter?.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
