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
using System.Collections.Generic;
using System.Windows.Data;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public class ITypeItemsSourceControl:ITypeEditor    
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="info"></param>
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
                BindingOperations.SetBinding(ctrl, ItemsSourceControl.ItemsSourceProperty, binding);
            }
            else
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.OneWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(ctrl, ItemsSourceControl.ItemsSourceProperty, binding);
            }
        }

        ItemsSourceControl  ctrl;

        /// <summary>
        /// Creates ands initializes a new instance of the editor
        /// </summary>
        /// <param name="PropertyInfo"></param>
        /// <returns></returns>
        public object Create(PropertyInfo PropertyInfo)
        {
            ctrl = new ItemsSourceControl();
            return ctrl;
        }

        /// <summary>
        /// Detaches (releases) the editor that was attached with the property passed as parameter.
        /// </summary>
        /// <param name="property"></param>
        public void Detach(PropertyViewItem property)
        {
           
        }
    }
}