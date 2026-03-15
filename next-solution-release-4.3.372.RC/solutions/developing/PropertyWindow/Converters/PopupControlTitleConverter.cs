using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using PropertyControl.Localization;

namespace PropertyControl
{
    internal class PopupControlTitleConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tagNames = (string)value;
            int numberOfelements = tagNames.Split('|').Length - 1;

            if (numberOfelements >= 1)
                return (string)tagNames.Replace("|", ", ");
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    internal class PopupControlHelpConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var row = (Mindscape.WpfElements.WpfPropertyGrid.PropertyGridRow)value;
            if (row == null)
            {
                return String.Empty;
            }

            if (!row.Node.Parent.PropertyType.IsPrimitive &&
                row.Node.Parent.PropertyType.IsValueType &&
                row.Node.PropertyType.IsPrimitive)
            {
                var tempResourceManager = new PropertyResourceManager(row.Node.Parent.Property.PropertyType.FullName);
                return tempResourceManager.GetString(String.Format("{0}_Help", row.Node.Property.DisplayName));
            }
            else
                return row.Node.Property.Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
