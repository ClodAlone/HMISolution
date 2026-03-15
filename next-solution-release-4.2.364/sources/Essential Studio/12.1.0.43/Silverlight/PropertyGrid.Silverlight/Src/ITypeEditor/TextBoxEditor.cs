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
    public class TextBoxEditor :ITypeEditor
    {
        public TextBoxEditor()
        {

        }

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
#if WPF
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
#endif
#if SILVERLIGHT
                    UpdateSourceTrigger=System.Windows.Data.UpdateSourceTrigger.Explicit,
#endif
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            }
            else
            {
                textBox.IsReadOnly = true;
                var binding = new Binding("Value")
                {
#if WPF
                    Mode = BindingMode.OneWay,
#endif
#if SILVERLIGHT
                    Mode = BindingMode.OneWay,
#endif
#if WPF
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
#endif
#if SILVERLIGHT
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.Explicit,
#endif
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            }
        }

        TextBox textBox;
        public object Create(PropertyInfo propertyInfo)
        {
            textBox = new TextBox();
#if SILVERLIGHT
            textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
#endif
            return textBox;
        }

#if SILVERLIGHT
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var expr = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            expr.UpdateSource();
        }
#endif

        public void Detach(PropertyViewItem property)
        {
            if (textBox != null)
            {
              
#if SILVERLIGHT
              textBox.TextChanged -= new TextChangedEventHandler(textBox_TextChanged);
              textBox.ClearValue(TextBox.TextProperty);
#endif
#if WPF
            BindingOperations.ClearBinding(textBox, TextBox.TextProperty);
#endif

            }
            textBox = null;
        }
    }

}
