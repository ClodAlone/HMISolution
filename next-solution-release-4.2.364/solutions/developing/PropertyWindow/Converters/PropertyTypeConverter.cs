using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using Utilities.WPF;
using System.ComponentModel;
using Mindscape.WpfElements.WpfPropertyGrid;
using System.Collections.Specialized;
using System.Globalization;
using Mindscape.WpfElements.PropertyEditing;

namespace PropertyControl
{
    public class PropertyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Node node = (Node)value;
            return node.PropertyType.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
