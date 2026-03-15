#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.PdfViewer
{
    internal static class VisualTreeHelperEx
    {
        public static T FindChild<T>(DependencyObject o) where T : DependencyObject
        {
            if (o is T)
                return (T)o;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = FindChild<T>(child);

                if (result != null)
                    return result;
            }

            return null;
        }
        /// <summary>
        ///     Returns the template element of the given name within the Control.
        /// </summary>
        internal static T FindName<T>(string name, Control control) where T : FrameworkElement
        {
            ControlTemplate template = control.Template;
            if (template != null)
            {
                return template.FindName(name, control) as T;
            }
            return null;
        }
    }
}