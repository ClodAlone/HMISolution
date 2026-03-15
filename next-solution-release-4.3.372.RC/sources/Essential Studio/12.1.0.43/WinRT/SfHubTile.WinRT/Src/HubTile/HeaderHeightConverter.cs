#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if WINRT
using Windows.UI.Xaml.Data;
namespace Syncfusion.UI.Xaml.Controls.Notification

#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Data;
namespace Syncfusion.WP.Controls.Notification
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Notification
#endif
{
    public class HeaderHeightConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
         public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif       
        {
            return (double)value / 2;
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
         {
            throw new NotImplementedException();
        }
    }
}
