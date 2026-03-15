using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace Converters
{
    public class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            Visibility val = (Visibility)value;
            if (val == Visibility.Visible)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            throw new NotImplementedException();
        }
    }



    public class VisibilityToInvertionBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            Visibility val = (Visibility)value;
            if (val == Visibility.Visible)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            throw new NotImplementedException();
        }
    }

    public class MultiVisibilityToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            foreach (object value in values)
            {
                Visibility val = (Visibility)value;
                if (val != Visibility.Visible)
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            throw new NotImplementedException();
        }
    }

    public class MultiVisibilityToInvertionBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            foreach (object value in values)
            {
                Visibility val = (Visibility)value;
                if (val == Visibility.Visible)
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
