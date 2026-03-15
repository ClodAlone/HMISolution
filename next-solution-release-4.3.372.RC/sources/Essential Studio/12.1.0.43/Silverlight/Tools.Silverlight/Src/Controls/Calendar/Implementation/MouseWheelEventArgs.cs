#region Copyright
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Mouse Wheel Event Args.
    /// </summary>
    public class MouseWheelEventArgs : EventArgs
    {
        private double delta;
        private bool handled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelEventArgs"/> class.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public MouseWheelEventArgs(double delta)
        {
            this.delta = delta;
        }

        /// <summary>
        /// Gets the delta.
        /// </summary>
        /// <value>The delta.</value>
        public double Delta
        {
            get { return this.delta; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MouseWheelEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get { return this.handled; }
            set { this.handled = value; }
        }
    }
}