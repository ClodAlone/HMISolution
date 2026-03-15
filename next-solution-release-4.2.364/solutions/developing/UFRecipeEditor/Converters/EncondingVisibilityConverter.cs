using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFRecipeEditor.Converters
{
    class EncondingVisibilityConverter : System.Windows.Data.IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = true;
            foreach (object value in values)
            {
                if (value is bool)
                {
                    visible = visible && (bool)value;
                }
                else if (value is UFUAModel.DataType)
                {
                    visible = visible && ((UFUAModel.DataType)value == UFUAModel.DataType.String);
                }
            }

            return visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
