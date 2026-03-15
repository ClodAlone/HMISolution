using DevExpress.Xpf.Grid;
using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace DataValidation.Converters
{
    class IsValidToBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Count() != 2 || values[0] as GridCellData == null || !(values[1] is bool))
                return Brushes.Transparent;

            DataValidation dv = ((values[0] as GridCellData).View as TableView)?.Tag as DataValidation;

            if (dv == null)
                return Brushes.Transparent;

            return (bool)values[1] ? Brushes.Transparent : new SolidColorBrush(dv.RowsInErrorColor);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
