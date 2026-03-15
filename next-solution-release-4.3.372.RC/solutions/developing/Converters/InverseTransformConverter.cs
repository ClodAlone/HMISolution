using System;
using System.Linq;
using System.Windows.Data;
using System.IO;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    [ValueConversion(typeof(Uri), typeof(String))]
    public class InverseTransformConverter : IMultiValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] as Boolean? != true)
                return Binding.DoNothing;
            
            var canvas = values[1] as Canvas;
            if (canvas == null)
                return Binding.DoNothing;

            Transform transform = null;
            if (canvas.LayoutTransform as TransformGroup != null)
                transform = (from tr in (canvas.LayoutTransform as TransformGroup).Children where tr is ScaleTransform select tr).FirstOrDefault();
            return transform != null ? transform.Inverse : Binding.DoNothing;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
