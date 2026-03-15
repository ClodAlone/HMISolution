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
#if WINRT_USING
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; 
#else
using System.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Diagram.Utility
{
    public class FocusUtility : DependencyObject
    {
        public static bool GetFocusOnLoad(DependencyObject dp)
        {
            return (bool)dp.GetValue(FocusOnLoadProperty);
        }

        public static void SetFocusOnLoad(DependencyObject dp, bool value)
        {
            dp.SetValue(FocusOnLoadProperty, value);
        }

        // Using a DependencyProperty as the backing store for FocusOnLoad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached("FocusOnLoad", typeof(bool), typeof(FocusUtility), new PropertyMetadata(false, OnFocus));

        private static void OnFocus(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control element = d as Control;
            if (element != null)
            {
#if WINRT
                element.Loaded += (s, evt) => (s as Control).Focus(FocusState.Keyboard); 
#else
                element.Loaded += (s, evt) => (s as Control).Focus(); 
#endif
            }
        }
    }
}
