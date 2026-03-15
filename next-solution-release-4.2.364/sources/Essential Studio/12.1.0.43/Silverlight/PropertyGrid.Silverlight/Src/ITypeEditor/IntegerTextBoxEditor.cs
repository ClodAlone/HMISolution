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
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Data;
using Syncfusion.Windows.Shared;
using System.Reflection;

namespace Syncfusion.Windows.PropertyGrid
{
    public class IntegerTextBoxEditor : ITypeEditor
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
                    ValidatesOnDataErrors = true,
                };

                if (info.PropertyType == typeof(Int32))
                    binding.Converter = new Int64ToInt32ConverterForIntegerTextBox();

                BindingOperations.SetBinding(integerTextBox, IntegerTextBox.ValueProperty, binding);
            }
            else
            {
                integerTextBox.IsReadOnly = true;
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.OneWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };

                if (info.PropertyType == typeof(Int32))
                    binding.Converter = new Int64ToInt32ConverterForIntegerTextBox();

                BindingOperations.SetBinding(integerTextBox, IntegerTextBox.ValueProperty, binding);
            }
        }

        IntegerTextBox integerTextBox;
        public object Create(PropertyInfo propertyInfo)
        {
            integerTextBox = new IntegerTextBox()
            {
                IsScrollingOnCircle = false,
                ApplyNegativeForeground = false,
                ApplyZeroColor = false,
                UseNullOption = true,
            };
            return integerTextBox;
        }

        public void Detach(PropertyViewItem property)
        {
            if (integerTextBox != null)
            {
#if SILVERLIGHT
                integerTextBox.ClearValue(IntegerTextBox.ValueProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(integerTextBox, IntegerTextBox.ValueProperty);
#endif

            }
            integerTextBox = null;
        }
    }

    public class Int64ToInt32ConverterForIntegerTextBox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int32 i;
            if (value != null)
            {
                if ((Int64)value > Int32.MaxValue)
                {
                    return Int32.MaxValue;
                }
                else if ((Int64)value < Int32.MinValue)
                {
                    return Int32.MinValue;
                }
                Int32.TryParse(value.ToString(), out i);

                return i;
            }
            return null;
        }
    }

}
