using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace CommonControls.Converters
{
    class IndexOfConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return Binding.DoNothing;

            var itemsControl = values[0] as ItemsControl;
            var item = values[1];
            var itemContainer = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

            if (itemContainer == null)
            {
                return Binding.DoNothing;
            }

            int itemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) + 1;
            return String.Format("{0}: ", itemIndex);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}
