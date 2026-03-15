using System;
using System.Windows;
using System.Windows.Data;

namespace EditDisplay.Converters
{
    public class FontSizeToCellSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Size s = new Size();

            int val;  
            if(int.TryParse(value.ToString(), out val))
            {
                s.Width = val * 2;
                s.Height = val * 1.5;
            }
            else
            {
                //set default values, considering a font size = 12 (theoretically font size is not null so this is only to be sure that size is a valid value) 
                s.Width = 24;
                s.Height = 18;
            }                
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
