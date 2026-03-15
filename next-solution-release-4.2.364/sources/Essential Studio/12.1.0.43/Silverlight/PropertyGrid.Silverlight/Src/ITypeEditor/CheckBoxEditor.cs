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
using System.Reflection;

namespace Syncfusion.Windows.PropertyGrid
{
    public class CheckBoxEditor : ITypeEditor
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
                BindingOperations.SetBinding(checkBox, CheckBox.IsCheckedProperty, binding);
            }
            else
            {
                checkBox.IsHitTestVisible = false;
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
                BindingOperations.SetBinding(checkBox, CheckBox.IsCheckedProperty, binding);
            }
        }

        CheckBox checkBox;
        public object Create(PropertyInfo propertyInfo)
        {
            checkBox = new CheckBox();
            checkBox.Margin = new Thickness(3, 0, 0, 0);
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            return checkBox;
        }

        public void Detach(PropertyViewItem property)
        {
            if (checkBox != null)
            {
#if SILVERLIGHT
                checkBox.ClearValue(CheckBox.IsCheckedProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(checkBox, CheckBox.IsCheckedProperty);
#endif
            }
            checkBox = null;
        }
    }
}
