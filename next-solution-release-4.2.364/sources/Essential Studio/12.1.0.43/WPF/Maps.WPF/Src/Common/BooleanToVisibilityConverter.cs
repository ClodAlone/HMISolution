#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Syncfusion.Windows.Controls.Map
{
    /// <summary>
    ///  This helps to convert Boolean Value to Visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into bool type</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Visibility</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bval = (bool)value;
            if (bval)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : throw</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// This helps to convert Boolean Value to inverse Visibility
    /// </summary>
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into bool type</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Visibility</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bval = (bool)value;
            if (bval)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : throw</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
   
}
