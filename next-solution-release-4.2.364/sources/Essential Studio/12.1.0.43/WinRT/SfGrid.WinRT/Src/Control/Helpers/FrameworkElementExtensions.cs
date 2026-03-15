#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    internal static class FrameworkElementExtensions
    {
        public static FrameworkElement FindRootParent(this Object element)
        {
            var ancestor = element as FrameworkElement;

            while (true)
            {
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(ancestor) as FrameworkElement;

                if (parent == null)
                { return ancestor; }

                ancestor = parent;
            }
        }
    }
}
