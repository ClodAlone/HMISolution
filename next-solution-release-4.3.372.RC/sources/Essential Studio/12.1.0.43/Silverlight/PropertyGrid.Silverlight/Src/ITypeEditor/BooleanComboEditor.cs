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
using System.Reflection;

namespace Syncfusion.Windows.PropertyGrid
{
    public class BooleanComboEditor : ITypeEditor
    {
        private ComboBox comboBox;

        public BooleanComboEditor()
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
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(comboBox, ComboBox.SelectedValueProperty, binding);
            }
            else
            {
                comboBox.IsHitTestVisible = false;
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.OneWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(comboBox, ComboBox.SelectedValueProperty, binding);
            }
        }

        public object Create(PropertyInfo propertyInfo)
        {
            comboBox = new ComboBox();
            comboBox.ItemsSource = new List<BoolMembers>
            {
                new BoolMembers(){DispalyName = "True",Value = true},
                new BoolMembers(){DispalyName = "False",Value = false},
                new BoolMembers(){DispalyName = "{x:Null}",Value = null}
            };
            comboBox.DisplayMemberPath = "DispalyName";
            comboBox.SelectedValuePath = "Value";
            return comboBox;
        }

        public void Detach(PropertyViewItem property)
        {
            if (comboBox != null)
            {
#if SILVERLIGHT
                comboBox.ClearValue(ComboBox.SelectedValueProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(comboBox, ComboBox.SelectedValueProperty);
#endif
            }
            comboBox = null;

        }
    }

    public class BoolMembers
    {
        public string DispalyName
        {
            get;
            set;
        }

        public bool? Value
        {
            get;
            set;
        }

    }
}
