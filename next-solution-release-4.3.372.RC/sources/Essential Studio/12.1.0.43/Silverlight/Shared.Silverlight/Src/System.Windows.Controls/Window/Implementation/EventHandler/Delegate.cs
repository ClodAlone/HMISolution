#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the ClosedEventhandler.
    /// </summary>
    public delegate void ClosedEventHandler(object sender, ClosedEventArgs e);

    /// <summary>
    /// Represents the WindowStateChangingEventHadler
    /// </summary>
    public delegate void WindowStateChangingEventHadler(object sender, WindowStateChangingEventArgs e);

    /// <summary>
    /// Represents the SystemMenuOpeningEventHandler
    /// </summary>
    public delegate void SystemMenuOpeningEventHandler(object sender, SystemMenuOpeningEventArgs e);

    /// <summary>
    /// Represents the WindowStateChangingEventArgs
    /// </summary>
    public class WindowStateChangingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WindowStateChangingEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled{get; set;}

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SystemMenuOpeningEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WindowStateChangingEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

    }

    /// <summary>
    /// Represents the ClosedEventArgs
    /// </summary>
    public class ClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ClosedEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>The dialog result.</value>
        public DialogResult DialogResult { get; set; }

        /// <summary>
        /// Gets or sets the custom dialog result.
        /// </summary>
        /// <value>The custom dialog result.</value>
        public string CustomDialogResult { get; set; }

        /// <summary>
        /// Gets or sets the dialog value.
        /// </summary>
        /// <value>The dialog value.</value>
        public Object DialogValue { get; set; }
    }
}
