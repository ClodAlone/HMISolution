using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StartupWelcome.Converters
{
    /// <summary>
    /// A Command converter
    /// </summary>    
    class CommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");
            if (value.Equals("NewProject"))
                return StartupWelcome.Properties.Resources.NewProject.ToString();
            else if (value.Equals("OpenProject"))
                return StartupWelcome.Properties.Resources.OpenProject.ToString();
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
