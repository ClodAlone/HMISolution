using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFUtilities.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DateTime dt = DateTime.MinValue;
                if (value != null && value is DateTime)
                    dt = (DateTime)value;

                return dt.ToString("HH:mm:ss");
            }
            catch
            {

            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (!String.IsNullOrEmpty(value as String))
                {
                    var clt = CultureInfo.InvariantCulture;
                    var val = String.Format("{0} {1}", DateTime.MinValue.ToString("d", clt), value as String);
                    DateTime date;

                    if (!DateTime.TryParseExact(val, "MM/dd/yyyy HH:mm:ss", clt, DateTimeStyles.None, out date))
                        return null;
                    return date;
                }
                else
                    return Binding.DoNothing;
            }
            catch (Exception)
            {
            }

            return Binding.DoNothing;
        }

        #endregion
    }
}
