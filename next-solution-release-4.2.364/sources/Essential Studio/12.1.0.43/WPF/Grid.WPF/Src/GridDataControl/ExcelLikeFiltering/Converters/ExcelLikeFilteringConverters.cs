#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.Generic;
using Syncfusion.Windows.GridCommon;
using System.Globalization;

namespace Syncfusion.Windows.Controls.Grid
{
    #region TextVisibilityConverter

    
   
    /// <summary>
    /// Used to determine the TextBlock Visibility
    /// </summary>
    public class TextVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string.IsNullOrEmpty(value.ToString())))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
#if !SILVERLIGHT

    public class ItemsSourceCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] != null && (values[0] as List<FilterElement>).Count > 0)
            {
                if ((bool)values[1])
                {
                    return (values[0] as List<FilterElement>).Count > 0;
                }
            }
            if((bool)values[1])
                return false;
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
#else
    public class ItemsSourceCountConverter : IValueConverter
    {      

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Int32.Parse(value.ToString()) > 0)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


#endif

    #endregion

    #region ReverseVisibilityConverter

    /// <summary>
    /// ReverseVisibilityConverter
    /// </summary>
    public class ReverseVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && (Visibility)value == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }
            else
                return Visibility.Visible;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion     

    #region DatePickerVisibilityConverter
    
    /// <summary>
    /// Use to find the Visibility of DateTimePicker
    /// </summary>
    public class DatePickerVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString()=="Date Filters")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion

#if SILVERLIGHT

    #region MarginConverterForOldExcelLikeFiltering
   
    public class MarginConverterForOldExcelLikeFiltering : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "BorderThickness")
            {
                if ((bool)value == true)
                {
                    return new Thickness(0.5,0,0,0);
                }
                return new Thickness(0);
            }
            else if (parameter.ToString() == "Margin")
            {
                if ((bool)value == true)
                {
                    return new Thickness(27, 0, 0, 10);
                }
                return new Thickness(0);
            }
            else if (parameter.ToString() == "ListBoxPart")
            {
                if ((bool)value == true)
                {
                    return new Thickness(30, 0, 0, 0);
                }
                return new Thickness(0);
            }
            else if (parameter.ToString() == "ItemsControl")
            {
                if ((bool)value == true)
                {
                    return new Thickness(0, 2, 5, 2);
                }
                return new Thickness(0);
            }
            else
            {
                if ((bool)value == true)
                {
                    return new Thickness(-27, 0, 0, -10);
                }
                return new Thickness(0);
            }
            
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

     #endregion

#region IconColorConverter
    public class IconFillColorConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                if (parameter.ToString() == "FilterIcon")
                    return "#FF231F20"; 
                else
                {
                    return Brushes.Red;
                }
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SortIconBackGroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value.ToString() == "Ascending")
            //{
            //    return parameter;
            //}           
            return Brushes.Transparent;
            //if ((bool)value)
            //{
            //    if (parameter.ToString() == "Ascending")
            //        return "#FF231F20";
            //    else
            //    {
            //        return Brushes.Red;
            //    }
            //}
            //return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    

#endregion
#else
    /// <summary>
    /// This Converter used to prevent the opeing old excel Popup.
    /// </summary>
    public class IsOpenConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] != DependencyProperty.UnsetValue)
            {
                if (!(bool)values[1])
                {
                    return false;
                }
            }
            return (bool)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] val = new object[2];
            val[0] = value;
            return val;
        }
    }

#endif

}
