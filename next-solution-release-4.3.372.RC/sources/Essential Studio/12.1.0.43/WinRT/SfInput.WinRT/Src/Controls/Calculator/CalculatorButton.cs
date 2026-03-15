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
    /// Represents a class for the calculator <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control buttons 
    /// </summary>
    public class CalculatorButton : Button
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalculatorButton"/> class.
        /// </summary>
        public CalculatorButton()
        {
#if WPFSILVERLIGHT
            DefaultStyleKey = typeof (Button);
#else
            DefaultStyleKey = typeof (CalculatorButton);
#endif
        }

        /// <summary>
        /// Gets or sets the Opacity of the brush
        /// </summary>
        public double BrushOpacity
        {
            get { return (double)GetValue(BrushOpacityProperty); }
            set { SetValue(BrushOpacityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BrushOpacity.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BrushOpacityProperty =
            DependencyProperty.Register("BrushOpacity", typeof(double), typeof(CalculatorButton), new PropertyMetadata(0.2d));

        /// <summary>
        /// Gets or sets the Key
        /// </summary>
#if !WINRT
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(CalculatorButton), new PropertyMetadata(Key.None));
#else
        public VirtualKey Key
        {
            get { return (VirtualKey)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(VirtualKey), typeof(CalculatorButton), new PropertyMetadata(null));
#endif
       


        /// <summary>
        /// Gets or sets the Function
        /// </summary>
        public CalculatorFunctions Function
        {
            get { return (CalculatorFunctions)GetValue(FunctionProperty); }
            set { SetValue(FunctionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Function.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FunctionProperty =
            DependencyProperty.Register("Function", typeof(CalculatorFunctions), typeof(CalculatorButton), new PropertyMetadata(CalculatorFunctions.None));


    }
}
