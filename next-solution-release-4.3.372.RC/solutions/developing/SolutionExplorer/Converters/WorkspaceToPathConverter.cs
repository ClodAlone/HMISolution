using DocumentManager.ComponentService;
using System;
using System.Windows.Data;
using UFInterfaces;

namespace UFProjectManager.Converters
{
    public class PathToIsEnableConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (targetType == typeof(string))
                return false;
            else if (targetType == typeof(bool))
                try
                {
                    return !XpoHelpers.XpoHelper.IsDataSource(value.ToString());
                }
                catch (Exception)
                {
                    return false;
                }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
