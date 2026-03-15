// <copyright file="UriConverter.cs" company="$registerdorganization$">
// Copyright (c) 2010 Microsoft. All Right Reserved
// </copyright>
// <author>Claudio</author>
// <email></email>
// <date>2010-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>

namespace StatesChartControl.Controls
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using DocumentManager.ComponentService;
    using OPCUAViewModelService.ComponentService;
    using Utilities;
    using System.Windows.Media.Imaging;
    using System.Windows.Markup;
    using static ValueItem;

    /// <summary>
    /// A Value converter
    /// </summary>
    public class ValueItemToVisualBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ValueItem desired = value as ValueItem;
            if (desired != null) {
                //if (desired.value == null || desired.value.ToString().Length == 0)
                //    return new SolidColorBrush(Colors.Transparent);
                return desired._brush;
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
