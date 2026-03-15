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
using System.Linq.Expressions;
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
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a class for Displaying the function keys.<see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FunctionsPane"/>
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/>
    public class FunctionsPane : Control
    {
        private InputPane PART_Input;

        private Grid PART_Root;

        /// <summary>
        /// Invoked when a calculator key is pressed.
        /// </summary>
        public event KeyInputEventHandler CalculatorKeyDown;

        /// <summary>
        /// Handles the events invoked due to functions
        /// </summary>
        public event FunctionEventHandler Function;

        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FunctionsPane"/> class.
        /// </summary>
        public FunctionsPane()
        {
            DefaultStyleKey = typeof (FunctionsPane);
        }

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FunctionsPane"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Input = GetTemplateChild("PART_Input") as InputPane;
            if (PART_Input != null)
            {
                PART_Input.CalculatorKeyDown += PartInputCalculatorKeyDown;
            } 

            PART_Root = GetTemplateChild("PART_Root") as Grid;
            if (PART_Root != null)
            {
                foreach (var child in PART_Root.Children)
                {
                    if (child is CalculatorButton)
                    {
                        ((CalculatorButton)child).Click += FunctionsPaneClick;
                    }
                }
            }
            base.OnApplyTemplate();
        }

        void FunctionsPaneClick(object sender, RoutedEventArgs e)
        {
            var calcbtn = sender as CalculatorButton;
            if (calcbtn != null)
            {
                OnFunction(calcbtn.Function);
            }
        }

        /// <summary>
        /// Occurs when a calculator key is pressed.
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
                CalculatorKeyDown(this, new KeyInputEventArgs { Key = key});
            }
        }

        /// <summary>
        /// Occurs when a function key is pressed.
        /// </summary>
        /// <param name="function"></param>
        protected virtual void OnFunction(CalculatorFunctions function)
        {
            if (Function != null)
            {
                Function(this, new FunctionEventArgs{Function = function});
            }
        }

        void PartInputCalculatorKeyDown(object sender, KeyInputEventArgs e)
        {
            OnCalculatorKeyDown(e.Key);
        }
    }
}
