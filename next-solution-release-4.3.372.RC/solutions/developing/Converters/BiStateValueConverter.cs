using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS_UWP
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace Converters
{
    public class BiStateValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
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

        public object ConvertBack(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            //if (value == null)
            //    return true;
            //return (bool)value ? 1 : 0;
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
    }
}
