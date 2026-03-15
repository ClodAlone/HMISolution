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
    public class DateTimeEditor : ITypeEditor
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
                BindingOperations.SetBinding(dateTimeEdit, DateTimeEdit.DateTimeProperty, binding);
            }
            else
            {
                dateTimeEdit.IsReadOnly = true;
                dateTimeEdit.IsHitTestVisible = false;
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
                BindingOperations.SetBinding(dateTimeEdit, DateTimeEdit.DateTimeProperty, binding);
            }
        }

        DateTimeEdit dateTimeEdit;
        public object Create(PropertyInfo propertyInfo)
        {
            dateTimeEdit = new DateTimeEdit()
            {
                IsScrollingOnCircle = false,
                EnableBackspaceKey=true,
                EnableDeleteKey=true
            
            };
            
            return dateTimeEdit;
        }

        public void Detach(PropertyViewItem property)
        {
            if (dateTimeEdit != null)
            {
#if SILVERLIGHT
                dateTimeEdit.ClearValue(DateTimeEdit.DateTimeProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(dateTimeEdit, DateTimeEdit.DateTimeProperty);
#endif
            }
            dateTimeEdit = null;
        }
    }
}