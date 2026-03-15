#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Theming
{
    /// <summary>
    /// This class retrieves the default style key of a control.
    /// </summary>
    internal class DefaultStyleKeyRetriever : Control
    {
        /// <summary>
        /// Initializes a new instance of the DefaultStyleKeyRetriever class.
        /// </summary>
        private DefaultStyleKeyRetriever()
        {
        }

        /// <summary>
        /// This method retrieves the default style key of a control.
        /// </summary>
        /// <param name="control">The control to retrieve the default style key 
        /// from.</param>
        /// <returns>The default style key of the control.</returns>
        public static object GetDefaultStyleKey(Control control)
        {
            return control.GetValue(Control.DefaultStyleKeyProperty);
        }
    }
}
