using System;
using System.Windows.Data;

namespace MSZui.Converters
{
    public class ExpiringDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var date = String.IsNullOrEmpty(value as String) ? Properties.Resources.NoDate : value as String;
            return String.Format("{0} : {1}", Properties.Resources.ExipingDate, date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SerialNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value as String) && !(value as String).Equals("0"))
                return String.Format("{0} : {1}", Properties.Resources.SerialNumber, value as String);
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IOTagsNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int tagsNumber = 0;
            if (int.TryParse(value as string, out tagsNumber) && tagsNumber >= Properties.Settings.Default.UnlimitedTagsNumber)
                return Properties.Resources.UnlimitedLabel;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NetStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2 || !(values[1] is bool))
                return null;

            string ret;
            if (!string.IsNullOrEmpty(values[0] as String))
                ret = Properties.Resources.NetLicenseFound;
            else if (MSZ.MSZView.IsRemoved)
                ret = Properties.Resources.DongleRemoved;
            else
                ret = Properties.Resources.licenseNotFound;

            if ((bool)values[1])
            {
                if (MSZ.MSZView.IsRemoved)
                    ret = Properties.Resources.DongleRemoved;
                else
                    ret = Properties.Resources.licenseNotFound;
            }
            else
            {
                if (!string.IsNullOrEmpty(values[0] as String))
                {
                    ret = Properties.Resources.NetLicenseFound;
                }
                else if (MSZ.MSZView.IsRemoved)
                    ret = Properties.Resources.DongleRemoved;
                else
                    ret = Properties.Resources.licenseFound;
            }
            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}