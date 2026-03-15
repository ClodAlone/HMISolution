#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    public class FocusManagerHelper
    {
        public static readonly DependencyProperty FocusedElementProperty =
            DependencyProperty.RegisterAttached("FocusedElement", typeof (bool), typeof (FocusManagerHelper), null);

        public static void SetFocusedElement(UIElement element, bool value)
        {
            element.SetValue(FocusedElementProperty, value);
        }

        public static bool GetFocusedElement(UIElement element)
        {
            return (bool) element.GetValue(FocusedElementProperty);
        }
        
        /// <summary>
        /// The handle template element property
        /// </summary>
        public static readonly DependencyProperty WantsKeyInputProperty =
            DependencyProperty.RegisterAttached("WantsKeyInput", typeof(bool), typeof(FocusManagerHelper), new PropertyMetadata(false));

        public static void SetWantsKeyInput(GridTemplateColumn column, bool value)
        {
            column.SetValue(WantsKeyInputProperty, value);
        }

        public static bool GetWantsKeyInput(GridTemplateColumn column)
        {
            return (bool)column.GetValue(WantsKeyInputProperty);
        }


        /// <summary>
        /// Generic method to get the child object
        /// </summary>
        public static Control GetFocusedUIElement(UIElement obj)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var element = VisualTreeHelper.GetChild(obj, i) as UIElement;
                if (element == null) 
                    continue;
                var control = GetFocusedElement(element);
                if (control)
                    return element as Control;
                return GetFocusedUIElement(element);
            }
            return null;
        }
    }
}
