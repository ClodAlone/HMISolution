using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Converters
{
    public class ThreeStateValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            object retVal = new object();
            int ret;
            if (int.TryParse(value.ToString(), out ret))
                switch (ret)
                {
                    case 1:
                        retVal = true;
                        break;
                    case 0:
                        retVal = false;
                        break;
                    default:
                        retVal = null;
                        break;
                }
            else
            {
                bool bret;
                bool.TryParse(value.ToString(), out bret);
                retVal = bret;
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 2;
            switch ((bool)value)
            {
                case true:
                    return 1;
                case false:
                    return 0;
                default:
                    return 2;
            }
            return value;
        }
        #endregion

    }
}
