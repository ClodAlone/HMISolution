using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFUtilities
{
    public sealed class GroupItemToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var viewGroup = value as CollectionViewGroup;
            if (viewGroup != null && (viewGroup.Name as String) == Properties.Resources.AdvancedGroupName)
                return new Thickness(-20, 0, 20, 0);
            else
                return new Thickness(0, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
