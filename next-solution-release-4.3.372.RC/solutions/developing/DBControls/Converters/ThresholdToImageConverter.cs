using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using DocumentManager.ComponentService;
using WPFUtilities.Converters;
using System.Collections.Generic;
using System.Linq;

namespace DBControls.Converters
{
    public class ThresholdToImageConverter : IMultiValueConverter
    {
        #region IMultiValueConverter implementation
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 || values[0] == null || String.IsNullOrEmpty(values[0].ToString()) || values[1] as ImageThresholdCollection == null || values[2] as IValueConverter == null || values[3] == null)
                return null;

            var cellValue = values[0].ToString();
            var thresholdsMap = values[1] as ImageThresholdCollection;
            var columnName = values[3].ToString();

            var matchingThreshold = FindMatchingThreshold(cellValue, columnName, thresholdsMap);
            var imgConverter = values[2] as IValueConverter;

            return GetImageFromThreshold(cellValue, thresholdsMap, imgConverter, matchingThreshold);
        }

        public object GetImageFromThreshold(String cellValue, ImageThresholdCollection thresholdsMap, IValueConverter imgConverter, ImageThreshold matchingThreshold)
        {
            var imgUri = matchingThreshold?.Value;

            if (thresholdsMap.Count == 0 || imgUri == null)
                return cellValue;

            try
            {
                return imgConverter.Convert(imgUri.ToString(), typeof(ImageSource), imgUri.ToString(), null);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion

        public ImageThreshold FindMatchingThreshold(String cellValue, String columnName, ImageThresholdCollection thresholdsMap)
        {
            if (cellValue == null || thresholdsMap == null)
                return null;

            var matchingThreshold = (from ImageThreshold t in thresholdsMap.AsParallel() where t.ImageThresholdValue == cellValue && t.ColumnName == columnName select t).FirstOrDefault();
            return matchingThreshold;
        }
    }
}
