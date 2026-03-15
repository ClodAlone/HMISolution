#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Data;

namespace Syncfusion.UI.Xaml.Grid
{
    public class PageCountFormatConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : string.Format(" / {0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class ZoomFactorFormatConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : string.Format("{0:#}%", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class PrintOrientationConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PrintOrientation)
                return (PrintOrientation)value == PrintOrientation.Landscape ? 1 : 0;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value == 1 ? PrintOrientation.Landscape : PrintOrientation.Portrait;
        }
    }

    public class ScaleOptionsConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PrintScaleOptions)
            {
                switch ((PrintScaleOptions)value)
                {
                    case PrintScaleOptions.NoScaling:
                        return 0;

                    case PrintScaleOptions.FitViewonOnePage:
                        return 1;
                    case PrintScaleOptions.FitAllColumnsonOnePage:
                        return 2;
                    case PrintScaleOptions.FitAllRowsonOnePage:
                        return 3;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return PrintScaleOptions.NoScaling;
                case 1:
                    return PrintScaleOptions.FitViewonOnePage;
                case 2:
                    return PrintScaleOptions.FitAllColumnsonOnePage;
                case 3:
                    return PrintScaleOptions.FitAllRowsonOnePage;

            }
            return value;
        }
    }
}
