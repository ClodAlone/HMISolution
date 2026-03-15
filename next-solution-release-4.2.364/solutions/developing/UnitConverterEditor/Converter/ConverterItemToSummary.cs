using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitConverterManager.Converters
{
    public class ConverterItemToSummary : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is UnitConverterModel.UFConverterItem)
                    return (value as UnitConverterModel.UFConverterItem).ConverterSummary;
            }
            catch (Exception ex)
            { }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
