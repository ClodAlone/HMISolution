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
using System.Windows.Input;


#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif WPF

namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
using Windows.System;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Handles the events invoked due to key input.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void KeyInputEventHandler(object sender, KeyInputEventArgs e);
    
    /// <summary>
    /// Represents a class for the Arguments of KeyInput events.
    /// </summary>
    public class KeyInputEventArgs : EventArgs
    {
        /// <summary>
        /// Getsor Sets the Key.
        /// </summary>
#if !WINRT
        public Key Key { get; set; }
#else
        public VirtualKey Key { get; set; }
#endif
    }

    /// <summary>
    /// Handles the events invoked due to Functions.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FunctionEventHandler(object sender, FunctionEventArgs e);

    /// <summary>
    /// Represents a class for the Arguments of Function events.
    /// </summary>
    public class FunctionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the functions <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FunctionsPane"/> 
        /// </summary>
        public CalculatorFunctions Function { get; set; }
    }
}
