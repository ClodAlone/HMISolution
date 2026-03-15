using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace DBControls.Converters
{
    public class ColumnItemsToCaptionAlignmentConverter : IMultiValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter as String != null && (parameter as String == "1") ? true : false;

            if (values.Length != 3 || values[0] == null || string.IsNullOrEmpty(values[0] .ToString()) || values[1] as ImageThresholdCollection == null || values[2] as IValueConverter == null)
                return inverse ? Dock.Top : Dock.Bottom;

            //var colName = (String)values[1];
            //var itemsList = (ColumnItemList)values[0];

            //var item = itemsList[colName];

            var cellValue = values[0].ToString();
            var imgThresholds = values[1] as ImageThresholdCollection;
            var imgConverter = values[2] as IValueConverter;
            var thc = new ThresholdToImageConverter();
            var thr = thc.FindMatchingThreshold(cellValue, imgThresholds);
            if (thr != null)
            {
                switch (thr.ImageCaption.ToString())
                {
                    case "Left": return inverse ? Dock.Right : Dock.Left;
                    case "Top": return inverse ? Dock.Bottom : Dock.Top;
                    case "Right": return inverse ? Dock.Left : Dock.Right;
                    case "Bottom": return inverse ? Dock.Top : Dock.Bottom;
                }
            }
                    
            return inverse ? Dock.Top : Dock.Bottom;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
