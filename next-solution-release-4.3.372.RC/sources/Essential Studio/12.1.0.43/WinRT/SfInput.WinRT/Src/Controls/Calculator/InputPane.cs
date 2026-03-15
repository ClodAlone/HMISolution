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


#if !WINRT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

#if WINDOWS_PHONE || WINDOWS_PHONE_7

namespace Syncfusion.WP.Controls.Input
#elif WPF

namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a class for Displaying the input keys.<see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.InputPane"/>
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/>
    public class InputPane : Control
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.InputPane"/> class.
        /// </summary>
        public InputPane()
        {
            DefaultStyleKey = typeof (InputPane);
        }

        /// <summary>
        /// Invoked when the Calculator ky is pressed.
        /// </summary>
        public event KeyInputEventHandler CalculatorKeyDown;

        private Grid PART_Root;

        /// <summary>
        /// Initializes all the child elements of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.InputPane"/> of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Root = GetTemplateChild("PART_Root") as Grid;
            foreach (var element in PART_Root.Children)
            {
                if (element is CalculatorButton)
                {
                    ((CalculatorButton)element).Click += InputPaneClick;
                }
            }
            base.OnApplyTemplate();
        }

        void InputPaneClick(object sender, RoutedEventArgs e)
        {
            var calcbtn = sender as CalculatorButton;
            if (calcbtn != null)
            {
                OnCalculatorKeyDown(calcbtn.Key);
#if WINRT || WPFSILVERLIGHT
                SfCalculator parent= DisplayPane.FindVisualParent<SfCalculator>(this);
                if (parent != null)
#endif
#if WINRT
                    parent.Focus(Windows.UI.Xaml.FocusState.Keyboard);
#elif WPFSILVERLIGHT
                    parent.Focus();
#endif
            }
        }
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="key"></param>
#if !WINRT
        protected virtual void OnCalculatorKeyDown(Key key)
#else
        protected virtual void OnCalculatorKeyDown(VirtualKey key)
#endif
        {
            if (CalculatorKeyDown != null)
            {
                CalculatorKeyDown(this, new KeyInputEventArgs {Key = key});
            }
        }
    }
}
