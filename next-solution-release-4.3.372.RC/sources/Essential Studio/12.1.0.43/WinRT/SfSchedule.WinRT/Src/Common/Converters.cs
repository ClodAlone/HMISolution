#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Linq;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
using System.Windows;
using System.Globalization;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region SelectedDatesToHeaderTextConverter

    /// <summary>
    /// Represents a converter that converts selected dates to and from header text.
    /// </summary>
    public class SelectedDatesToHeaderTextConverter : IValueConverter
    {
        #region Convert
        /// <summary>
        /// Converts selected dates to header text.
        /// </summary>
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            var selectedDates = value as ObservableCollection<DateTime>;
            if (selectedDates == null || selectedDates.Count <= 0)
            {
                return string.Empty;
            }
            var firstDate = selectedDates[0];
            var lastDate = selectedDates.OrderByDescending(x => x).Last();
            string result;
            if (selectedDates.Count == 1)
            {

                result = selectedDates[0].ToString("M").TrimEnd(';') + ", " + selectedDates[0].Year;
            }
            else if (selectedDates.Count > 1 && selectedDates.Count <= 7)
            {
                if (firstDate.Month == lastDate.Month && firstDate.Year == lastDate.Year)
                {
                    string lastDateString;
                    if (lastDate.Day < 10)
                        lastDateString = "0" + lastDate.Day;
                    else
                        lastDateString = lastDate.Day.ToString();
                    result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDateString + ", " + lastDate.Year;
                }

                else if (firstDate.Year == lastDate.Year)
                    result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDate.ToString("M").TrimEnd(';') + ", " + lastDate.Year;
                else
                    result = firstDate.ToString("M").TrimEnd(';') + ", " + firstDate.Year + " - " + lastDate.ToString("M").TrimEnd(';') + ", " + lastDate.Year;
            }
            else
            {
                if (firstDate.Month == lastDate.Month && firstDate.Year == lastDate.Year)
                    result = firstDate.ToString("MMMM") + " " + firstDate.Year;
                else if (firstDate.Year == lastDate.Year)
                    result = firstDate.ToString("MMMM") + " - " + lastDate.ToString("MMMM") + " " + lastDate.Year;
                else
                    result = firstDate.ToString("MMMM") + " " + firstDate.Year + " - " + lastDate.ToString("MMMM") + " " + lastDate.Year;
            }
            return result;
        }
        #endregion

        #region ConvertBack
        /// <summary>
        /// Converts header text to selected dates.
        /// </summary>
#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return value;
        }

        #endregion
    }

    #endregion

    #region BoolToVisibilityConverter

    /// <summary>
    /// Represents a converter that converts boolean values to and from visibility enumeration values. 
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a visibility enumeration value.
        /// </summary>
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            if (parameter != null && parameter.ToString() == "Inverse")
            {
                return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a visibility enumeration value to boolean value.
        /// </summary>
#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            if (parameter != null && parameter.ToString() == "Inverse")
            {
                return value is Visibility && (Visibility)value == Visibility.Collapsed;
            }
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }

    #endregion

    #region BoolToBoolConverter

    /// <summary>
    /// Represents a converter that converts boolean value to vice versa.
    /// </summary>
    public class BoolToBoolConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return (!(value is bool) || !((bool)value));
        }
#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return (!(value is bool) || !((bool)value));
        }


    }

    #endregion

    #region ValueToIndexConverter

    /// <summary>
    /// Represents a converter that converts value to index.
    /// </summary>
    public class ValueToIndexConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to index
        /// </summary>
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return (int)value - 1;
        }

        /// <summary>
        /// Converts an index to a value.
        /// </summary>
        /// <param name="value"></param>
#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return (int)value + 1;
        }
    }

    #endregion

    #region IsAlldaytoWordellipseConverter

    /// <summary>
    /// Represents a converter that converts all day panel to text trimming.
    /// </summary>
    public class IsAlldaytoWordellipseConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#endif
            var result = (bool)value;
            if (result)
            {
                return TextTrimming.None;
            }
            return TextTrimming.WordEllipsis;
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }

    }

    #endregion

    #region VisibileToHiddenConverter

#if WPF
    public class VisibileToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.Equals(Visibility.Visible))
                return Visibility.Hidden;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
#endif

    #endregion
}


