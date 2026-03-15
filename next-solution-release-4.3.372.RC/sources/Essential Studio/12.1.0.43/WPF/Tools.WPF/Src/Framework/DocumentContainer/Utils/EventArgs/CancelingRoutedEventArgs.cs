// <copyright file="CancelingRoutedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents class args that for closing action.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CancelingRoutedEventArgs : RoutedEventArgs
    {
        #region Private members
        /// <summary>
        /// Presents value that indicates whether to need cancel current action.
        /// </summary>
        private bool m_cancel = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelingRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public CancelingRoutedEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelingRoutedEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get
            {
                return m_cancel;
            }

            set
            {
                m_cancel = value;
            }
        }
        #endregion
    }


    /// <summary>
    /// Represents argument of IsSelectedChanged event
    /// </summary>
    public class IsSelectedChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the target element.
        /// </summary>
        /// <value>The target element.</value>
        public FrameworkElement TargetElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [new value].
        /// </summary>
        /// <value><c>true</c> if [new value]; otherwise, <c>false</c>.</value>
        public bool NewValue
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [old value].
        /// </summary>
        /// <value><c>true</c> if [old value]; otherwise, <c>false</c>.</value>
        public bool OldValue
        {
            get;
            internal set;
        }
    }
}