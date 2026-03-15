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

namespace Syncfusion.Windows.Tools.Controls
{
    public class WindowResizingEventArgs :EventArgs
    {
        /// <summary>
        /// Gets or sets the width of the desired.
        /// </summary>
        /// <value>The width of the desired.</value>
        public double DesiredWidth
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the height of the desired.
        /// </summary>
        /// <value>The height of the desired.</value>
        public double DesiredHeight
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public DockState State
        {
            get;
            internal set;
        }
    }
}
