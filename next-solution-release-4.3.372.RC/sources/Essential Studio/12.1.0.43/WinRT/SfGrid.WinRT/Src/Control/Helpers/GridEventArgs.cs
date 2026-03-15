#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridEventArgs : EventArgs
    {
        protected GridEventArgs(object originalSender)
        {
            OriginalSender = originalSender;
        }

        /// <summary>
        /// Gets the original reporting source that raising this event
        /// </summary>
        public object OriginalSender { get; private set; }
    }

    public class GridCancelEventArgs : CancelEventArgs
    {
        protected GridCancelEventArgs(object originalSender)
        {
            OriginalSender = originalSender;
        }

        /// <summary>
        /// Gets the original reporting source that raising this event
        /// </summary>
        public object OriginalSender { get; private set; }
    }

    public class GridHandledEventArgs : GridEventArgs
    {
        public GridHandledEventArgs(object originalSource)
            : base(originalSource)
        {
            Handled = false;
        }

        public GridHandledEventArgs(bool handled, object originalSource)
            : base(originalSource)
        {
            this.Handled = handled;
        }

        /// <summary>
        /// Indicates whether the event has been handled and no further processing of the event should happen.
        /// </summary>
        public bool Handled { get; set; }
    }
}
