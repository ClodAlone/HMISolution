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
using System.Reflection;

#if SILVERLIGHT
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Data;
#endif
#if WPF
using Syncfusion.Windows.Shared;
using System.Windows.Data;
#endif

namespace Syncfusion.Windows.PropertyGrid
{
    public class DoubleTextBoxEditor : ITypeEditor
    {

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(doubleTextBox, DoubleTextBox.ValueProperty, binding);
            }
            else
            {
                doubleTextBox.IsReadOnly = true;
                var binding = new Binding("Value")
                {
#if WPF
                    Mode = BindingMode.OneWay,
#endif
#if SILVERLIGHT
                    Mode = BindingMode.OneWay,
#endif
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(doubleTextBox, DoubleTextBox.ValueProperty, binding);
            }
        }

        DoubleTextBox doubleTextBox;
        public object Create(PropertyInfo propertyInfo)
        {
            doubleTextBox = new DoubleTextBox()
            {
                IsScrollingOnCircle = true,
                ApplyNegativeForeground = false,
                ApplyZeroColor = false
            };

            if (propertyInfo.Name == "FontSize" || propertyInfo.Name == "MinWidth" || propertyInfo.Name == "MinHeight" || propertyInfo.Name == "MaxHeight" || propertyInfo.Name == "MaxWidth" ||
                propertyInfo.Name == "Height" || propertyInfo.Name == "Width" || propertyInfo.Name == "ActualWidth" || propertyInfo.Name == "ActualHeight")
            {
                doubleTextBox.MinValue = 0;
            }
            return doubleTextBox;
        }
        
        public void Detach(PropertyViewItem property)
        {
            if (doubleTextBox != null)
            {
#if SILVERLIGHT
                doubleTextBox.ClearValue(DoubleTextBox.ValueProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(doubleTextBox, DoubleTextBox.ValueProperty);
#endif
            }
            doubleTextBox = null;
        }
    }
}
