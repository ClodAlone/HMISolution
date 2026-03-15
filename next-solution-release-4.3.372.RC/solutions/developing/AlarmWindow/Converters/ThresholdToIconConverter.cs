using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using DocumentManager.ComponentService;
using WPFUtilities.Converters;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;

namespace AlarmWindow.Converters
{
    public class ThresholdToIconConverter : IMultiValueConverter
    {
        #region Public Method

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values.Length != 5 || 
                    values.Any(x => x == DependencyProperty.UnsetValue) ||
                    !(values[0] is ThresholdList thresholdsMap) ||
                    !(double.TryParse(values[1].ToString(), out var severityValue)) ||
                    !(values[2] is IValueConverter imgConverter))
                    return null;

                var thresholdsMapOrdered = thresholdsMap.OrderBy(x => x.ThresholdValue);
                var matchingThreshold =
                    thresholdsMapOrdered.LastOrDefault(t => t.ThresholdValue <= severityValue) ??
                    thresholdsMapOrdered.FirstOrDefault(t => t.ThresholdValue > severityValue);

                return GetImageFromThreshold(
                    imgConverter, 
                    matchingThreshold, 
                    severityValue, 
                    values[3], 
                    values[4]);
            }
            catch(Exception _)
            {
                return null;
            }
        }

        public object[] ConvertBack(
            object value, 
            Type[] targetType,
            object parameter, 
            CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }

        #endregion

        #region Private Methods

        private object GetImageFromThreshold(
            IValueConverter imgConverter, 
            ThresholdSettings matchingThreshold, 
            Double severityValue, 
            object alarmIcon, 
            object messageIcon)
        {
            var imgUri = matchingThreshold?.Value;

            if (imgUri == null)
                return severityValue == 0 
                    ? messageIcon 
                    : alarmIcon;

            return imgConverter.Convert(
                imgUri.ToString(), 
                typeof(ImageSource), 
                imgUri.ToString(), 
                null);
        }

        #endregion
    }
}
