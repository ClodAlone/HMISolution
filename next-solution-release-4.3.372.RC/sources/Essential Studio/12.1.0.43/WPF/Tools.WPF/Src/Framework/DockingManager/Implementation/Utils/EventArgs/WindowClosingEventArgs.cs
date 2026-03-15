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

namespace Syncfusion.Windows.Tools.Controls
{
    public class WindowClosingEventArgs:EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WindowClosingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target item.
        /// </summary>
        /// <value>The target item.</value>
        public FrameworkElement TargetItem
        {
            get;
            set;
        }
    }
}
