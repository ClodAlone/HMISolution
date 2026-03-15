#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a class for converting control to content
    /// </summary>
    public class ControlToContentConveter:IValueConverter
    {
        /// <summary>
        /// Converts control to content
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value !=null && value is FrameworkElement)
            {
                if (value is ContentControl)
                {
                    return (value as ContentControl).Content.ToString();
                }
                else if (value is ContentPresenter)
                {
                    return ((value as ContentPresenter).Content.ToString());
                }
                else if (value is TextBox)
                {
                    return (value as TextBox).Text.ToString();
                }
                else if (value is HeaderedItemsControl)
                {
                    return (value as HeaderedItemsControl).Header.ToString();
                }
                else
                {
                    return value.ToString();
                }
            }
            else if (value != null)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Converts the content back to its default form
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
