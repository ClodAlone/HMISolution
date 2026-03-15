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

namespace Syncfusion.UI.Xaml.Collections.ComponentModel
{
    public class SyncfusionHandledEventArgs : EventArgs
    {
        bool handled;

        /// <summary>
        /// Overloaded. Initializes a new instance of the SyncfusionHandledEventArgs class with the Handled property set to False.
        /// </summary>
        public SyncfusionHandledEventArgs()
        {
            handled = false;
        }

        /// <summary>
        /// Initializes a new instance of the SyncfusionHandledEventArgs class with the Handled property set to the given value.
        /// </summary>
        public SyncfusionHandledEventArgs(bool handled)
        {
            this.handled = handled;
        }

        /// <summary>
        /// Indicates whether the event has been handled and no further processing of the event should happen.
        /// </summary>
        public bool Handled
        {
            get
            {
                return handled;
            }
            set
            {
                handled = value;
            }
        }
    }
}
