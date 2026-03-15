using System;
using System.Globalization;
using System.Windows.Data;

namespace ActiproUtilities.Converters
{
    public class SpinEditUnitConverter : IValueConverter
    {
        public SpinEditUnitConverter(ActiproSoftware.Windows.UnitType unitType)
        {
            UnitType = unitType;
        }

        public SpinEditUnitConverter()
        {

        }

        ActiproSoftware.Windows.UnitType UnitType = ActiproSoftware.Windows.UnitType.Pixel;

        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var unit = (ActiproSoftware.Windows.Unit)value;
                return unit.Value;
            }
            catch
            {
                return ActiproSoftware.Windows.Unit.Empty.Value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var doubleVal = System.Convert.ToDouble(value);
                return new ActiproSoftware.Windows.Unit(doubleVal, UnitType);
            }
            catch
            {
                return ActiproSoftware.Windows.Unit.Empty;
            }
        }
        #endregion
    }
}
