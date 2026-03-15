using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeViewerControl.Converters
{
    internal class EngineeringUnitValueConverter : IValueConverter
    {
        #region Declarations
        readonly double multiplierFactor;
        #endregion

        #region Constructor
        public EngineeringUnitValueConverter(double multiplierFactor)
        {
            this.multiplierFactor = multiplierFactor;
        }
        #endregion

        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble(value) * multiplierFactor;
            }
            catch 
            { }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble(value) / multiplierFactor;
            }
            catch
            { }

            return value;
        }
        #endregion
    }
}
