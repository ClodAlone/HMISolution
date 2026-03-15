// <copyright file="TextWrappingConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents TextWrapping converter.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TextWrappingConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is string))
            {
                throw new ArgumentException("Value of string type is expected.", "value");
            }


            string label = (string)value;
            label = label.Replace("\r\n", "\n");

            label = label.Replace("\r", "\n");


            ObservableCollection<object> result = new ObservableCollection<object>();
            bool split = true;
            if (parameter != null)
            {
                split = (bool)parameter;
            }

           
            if (split)
            {
                string[] splitLabel = label.Split(new string[0], StringSplitOptions.RemoveEmptyEntries);

                // Create a LabelTextBlock for each word

                foreach (string str in splitLabel)
                {

                    LabelTextBlock ltb = new LabelTextBlock();

                    ltb.Text = str + " ";

                    result.Add(ltb);

                }


          
            }

            else
            {
                label = label.Replace("\n", "\\n");
                result.Add(new LabelTextBlock() { Text = label, Margin = new Thickness(0, 0, 5, 0) });
            }
            return result;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
