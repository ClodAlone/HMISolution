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
    public class TransferManagerEventArgs:EventArgs
    {
        /// <summary>
        /// Gets or sets the previous manager.
        /// </summary>
        /// <value>The previous manager.</value>
        public DockingManager PreviousManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target manager.
        /// </summary>
        /// <value>The target manager.</value>
        public DockingManager TargetManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target element.
        /// </summary>
        /// <value>The target element.</value>
        public FrameworkElement TargetElement
        {
            get;
            set;
        }
    }
}
