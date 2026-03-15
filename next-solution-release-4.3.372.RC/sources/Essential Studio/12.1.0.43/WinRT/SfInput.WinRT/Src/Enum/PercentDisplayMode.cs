// <copyright file="PercentDisplayMode.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Specifies the PercentDisplayMode with NumericTextBox.
    /// </summary>
    public enum PercentDisplayMode
    {
        /// <summary>
        /// Return the text input by user.
        /// </summary>
        Value,

        /// <summary>
        /// Return the text input by user parsed with string "P".
        /// </summary>
        Compute
    }
}
