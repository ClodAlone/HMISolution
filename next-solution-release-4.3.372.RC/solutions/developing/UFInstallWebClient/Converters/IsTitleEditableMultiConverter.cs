using System;
using System.Globalization;
using System.Windows.Data;

namespace UFInstallWebClient.Converters
{
    public class IsTitleEditableMultiConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null)
            {
                for (int ii = 0; ii < values.Length; ii++)
                {
                    try
                    {
                        var result = System.Convert.ToBoolean(values[ii]);
                        if (!result)
                            return false;
                    }
                    catch
                    { }
                }
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
