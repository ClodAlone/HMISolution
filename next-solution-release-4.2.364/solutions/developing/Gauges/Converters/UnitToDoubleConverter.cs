using System;
using System.Globalization;
using System.Windows.Data;

namespace Gauges.Converters
{
    public class UnitToDoubleConverter : IMultiValueConverter
    {
        ActiproSoftware.Windows.UnitType UnitType = ActiproSoftware.Windows.UnitType.Percentage;
        #region IMultiValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //valus[0] is ActualValue
                //valus[1] is Percentiage value
                var unit = (ActiproSoftware.Windows.Unit)values[1];
                return (unit.Value * (double)values[0])/100d;
            }
            catch
            {
                return values[0];
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
