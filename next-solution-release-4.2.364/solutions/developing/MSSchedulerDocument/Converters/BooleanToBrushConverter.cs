using MSModel;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using WPFUtilities;
using System.Windows.Media;
using DevExpress.Xpf.Grid.Themes;
using System.Collections.Generic;

namespace MSSchedulerSettings.Converters
{
    public class BooleanToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int itemTag = (int)parameter;
                int actualIndex = (int)values[0];
                Brush selectionColor = (Brush)values[1];
                Brush minorColor = (Brush)values[2];
                Brush majorColor = (Brush)values[3];
                Brush defColor = itemTag % 4 == 0 ? majorColor : minorColor;
                return itemTag == actualIndex ? selectionColor : defColor;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BackgroundBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Brush defBackground = (Brush)values[0];
                Brush selectionColor = (Brush)values[1];
                Brush multSelectionColor = (Brush)values[3];
                List<Controls.CalendarItemDay> calendarItemDays = (List<Controls.CalendarItemDay>)values[2];
                if (calendarItemDays.Count > 1)
                {
                    return multSelectionColor;
                }
                else if (calendarItemDays.Count > 0)
                {
                    return selectionColor;
                }
                else
                    return defBackground;
            }
            catch (Exception ex)
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
