using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Windows.Media.Imaging;

namespace DBControls.Converters
{
    public class ColumnItemsToCaptionVisibilityConverter : IMultiValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[1] as ImageThresholdCollection == null || (values[1] as ImageThresholdCollection).Count == 0)
                return Visibility.Visible;

            bool bIsInEditMode = values.Length > 3 && values[3] is Boolean && (Boolean)values[3];
            if (bIsInEditMode)
                return Visibility.Collapsed;

            if (values.Length >= 3)
            {
                var cellValue = values[0]?.ToString();
                var imgThresholds = values[1] as ImageThresholdCollection;
                var imgConverter = values[2] as IValueConverter;
                var thc = new ThresholdToImageConverter();
                var thr = thc.FindMatchingThreshold(cellValue, imgThresholds);
                if (thr != null && thr.ImageCaption != CaptionAlignmentEnum.Hidden)
                    return Visibility.Visible;

                //Caption must be always visible if threshold image not found, even if set as Hidden
                var bHasImage = thc.GetImageFromThreshold(cellValue, imgThresholds, imgConverter, thr) as BitmapImage != null;
                if (!bHasImage)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
