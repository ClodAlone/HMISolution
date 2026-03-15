#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Windows;

#if !WinRT
namespace Syncfusion.Windows.ComponentModel
#else
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.ComponentModel
#endif
{

    public delegate void GridRoutedEventHandler(object sender, SyncfusionRoutedEventArgs args);
    /// <summary>
    /// This is a base class for events of the Syncfusion libraries. It supports writing
    /// properties in its ToString() method. 
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class SyncfusionRoutedEventArgs : RoutedEventArgs
    {
        public new static SyncfusionRoutedEventArgs Empty
        {
            get
            {
                return new SyncfusionRoutedEventArgs();
            }
        }

        public SyncfusionRoutedEventArgs()
        {
        }

        public SyncfusionRoutedEventArgs(object source)
        {
        }

        public bool Handled { get; set; }

 
    }

    public delegate void GridCancelRoutedEventHandler(object sender, SyncfusionCancelRoutedEventArgs args);
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class SyncfusionCancelRoutedEventArgs : SyncfusionRoutedEventArgs
    {
        public SyncfusionCancelRoutedEventArgs()
        {
        }

        public SyncfusionCancelRoutedEventArgs(object source)
            : base(source)
        {
        }

        public bool Cancel
        {
            get;
            set;
        }

    }

    /// <summary>
    /// This is a base class for events of the Syncfusion libraries. It supports writing
    /// properties in its ToString() method. 
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class SyncfusionEventArgs : EventArgs
    {
        public SyncfusionEventArgs()
        {
        }

    }

    public delegate void SyncfusionCancelEventHandler(object sender, SyncfusionCancelEventArgs args);

    /// <summary>
    /// Provides data for a cancellable event. 
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class SyncfusionCancelEventArgs : EventArgs
    {
        /// <summary>
        /// Overloaded. Initializes a new instance of the SyncfusionCancelEventArgs class.
        /// </summary>
        public SyncfusionCancelEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyncfusionCancelEventArgs class with the Cancel property set to the given value.
        /// </summary>
        public SyncfusionCancelEventArgs(bool cancel)
        {
            Cancel = cancel;
        }

        public bool Cancel { get; set; }

    }

 
    /// <summary>
    /// Provides data for a event that can be handled by a subscriber and overrides the event's default behavior.
    /// </summary>
    public class SyncfusionHandledEventArgs : SyncfusionEventArgs
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
