using Mindscape.WpfElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFUtilities.Converters
{
    #region Helpers
    public enum BytesUnit : int
    {
        Bytes,
        KBytes,
        MBytes,
        GBytes,
        TBytes
    }
    #endregion 

    public class BytesSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var doubleVal = System.Convert.ToDouble(value);
                return doubleVal / Math.Pow(1024, 2);
            }
            catch { }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var doubleVal = System.Convert.ToDouble(value);
                return doubleVal * Math.Pow(1024, 2);
            }
            catch { }
            return Binding.DoNothing;
        }
        #endregion
    }
}
